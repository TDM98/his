using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Windows.Controls;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ViewContracts;
using Caliburn.Micro;
using DataEntities;
using aEMR.ViewContracts.Configuration;
using System.Windows;
using System.Linq;
using aEMR.Controls;
using aEMR.CommonTasks;
using System.Collections.Generic;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using eHCMSLanguage;

namespace aEMR.Common.ViewModels
{
    [Export(typeof(IInPatientDeptListing)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class InPatientDeptListingViewModel : Conductor<object>, IInPatientDeptListing
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public InPatientDeptListingViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;
        }
        private bool _showDeleteEditColumn = false;
        public bool ShowDeleteEditColumn 
        {
            get { return _showDeleteEditColumn; }
            set
            {
                _showDeleteEditColumn = value;
            }
        }

        private bool _showBookingBedColumn;
        public bool ShowBookingBedColumn
        {
            get
            {
                return _showBookingBedColumn;
            }
            set
            {
                _showBookingBedColumn = value;
                NotifyOfPropertyChange(() => ShowBookingBedColumn);
            }
        }

        private bool _showOutDeptColumn;
        public bool ShowOutDeptColumn
        {
            get
            {
                return _showOutDeptColumn;
            }
            set
            {
                _showOutDeptColumn = value;
                NotifyOfPropertyChange(() => ShowOutDeptColumn);
            }
        }


        private ObservableCollection<InPatientDeptDetail> _allItems;

        public ObservableCollection<DataEntities.InPatientDeptDetail> AllItems
        {
            get { return _allItems; }
            set 
            { 
                _allItems = value;
                NotifyOfPropertyChange(() => AllItems);
            }
        }


        private PatientRegistration _currentRegistration;
        public PatientRegistration CurrentRegistration
        {
            get
            {
                return _currentRegistration;
            }
            set
            {
                if (_currentRegistration != value)
                {
                    _currentRegistration = value;
                    NotifyOfPropertyChange(() => CurrentRegistration);
                }
            }
        }

        //KMx: Trong CurrentRegistration có chứa AdmissionInfo rồi, khi nào có thời gian thì xem xét và bỏ AdmissionInfo đi (11/09/2014 10:23).
        private InPatientAdmDisDetails _admissionInfo;
        public InPatientAdmDisDetails AdmissionInfo
        {
            get { return _admissionInfo; }
            set 
            {
                _admissionInfo = value;
                NotifyOfPropertyChange(() => AdmissionInfo);
                AllItems = _admissionInfo == null? null: _admissionInfo.InPatientDeptDetails;
            }
        }

        public void LoadData()
        {
            
        }

        private bool _isLoading;

        public bool IsLoading
        {
            get { return _isLoading; }
            set { _isLoading = value;
            NotifyOfPropertyChange(() => IsLoading);
            }
        }

        //private ObservableCollection<long> _LstRefDepartment;
        //public ObservableCollection<long> LstRefDepartment
        //{
        //    get { return _LstRefDepartment; }
        //    set
        //    {
        //        _LstRefDepartment = value;
        //        NotifyOfPropertyChange(() => LstRefDepartment);
        //    }
        //}

        public void ReturnBedAllocItem(InPatientDeptDetail datacontext, object eventArgs)
        {
            Globals.EventAggregator.Publish(new ReturnItem<InPatientDeptDetail, object> { Item = datacontext, Source = this });
        }

        public bool CheckResponsibility(long refDepartmentID)
        {
            if (!Globals.isAccountCheck)
            {
                return true;
            }

            if (Globals.LoggedUserAccount.DeptIDResponsibilityList == null || Globals.LoggedUserAccount.DeptIDResponsibilityList.Count <= 0 || refDepartmentID <= 0)
            {
                return false;
            }

            var result = Globals.LoggedUserAccount.DeptIDResponsibilityList.Where(x => x == refDepartmentID).ToList();

            if (result == null || result.Count <= 0)
            {
                return false;
            }

            return true;
        }

        public void hplBookingBed_Click(InPatientDeptDetail selectItem)
        {
            if (AdmissionInfo == null || AdmissionInfo.PatientRegistration == null || selectItem == null || selectItem.DeptLocation == null || selectItem.DeptLocation.RefDepartment == null)
            {
                return;
            }

            if (!CheckResponsibility(selectItem.DeptLocation.DeptID))
            {
                MessageBox.Show(string.Format("{0} {1}. {2}", eHCMSResources.A0182_G1_Msg_SubKhDcCauHinhTNKhoa, selectItem.DeptLocation.RefDepartment.DeptName, eHCMSResources.A0183_G1_Msg_InfoKhTheDatGiuong), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }

            if (CurrentRegistration != null && Globals.IsLockRegistration(CurrentRegistration.RegLockFlag, eHCMSResources.K3103_G1_DatGiuong.ToLower()))
            {
                return;
            }

            if (selectItem.V_InPatientDeptStatus != AllLookupValues.InPatientDeptStatus.NHAP_KHOA_PHONG && !Globals.ServerConfigSection.CommonItems.AllowReSelectRoomWhenLeaveDept)
            {
                MessageBox.Show(string.Format("{0}.", eHCMSResources.A0686_G1_Msg_InfoKhTheDatGiuong), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
            bool IsReSelectBed = false;
            if (selectItem.V_InPatientDeptStatus != AllLookupValues.InPatientDeptStatus.NHAP_KHOA_PHONG)
            {
                IsReSelectBed = true;
            }

            //var bedAllocVm = Globals.GetViewModel<IBedPatientAlloc>();

            //bedAllocVm.curPatientRegistration = AdmissionInfo.PatientRegistration;

            //bedAllocVm.InPtDeptDetail = selectItem;

            ////KMx: Đổi biến BookBedAllocOnly thành eFireBookingBedEventTo để bắn event (06/09/2014 17:20).
            ////bedAllocVm.BookBedAllocOnly = false;//Dat giuong truc tiep luon.  
            //bedAllocVm.eFireBookingBedEventTo = eFireBookingBedEvent.NONE;

            //bedAllocVm.isLoadAllDept = false;
            //bedAllocVm.DefaultDepartment = selectItem.DeptLocation.RefDepartment;
            //bedAllocVm.ResponsibleDepartment = selectItem.DeptLocation.RefDepartment;
            //bedAllocVm.SelectedDeptLocation = selectItem.DeptLocation;

            //Globals.ShowDialog(bedAllocVm as Conductor<object>);

            Action<IBedPatientAlloc> onInitDlg = (Alloc) =>
            {
                Alloc.curPatientRegistration = AdmissionInfo.PatientRegistration;
                Alloc.InPtDeptDetail = selectItem;
                Alloc.eFireBookingBedEventTo = eFireBookingBedEvent.NONE;
                Alloc.isLoadAllDept = false;
                Alloc.DefaultDepartment = selectItem.DeptLocation.RefDepartment;
                Alloc.ResponsibleDepartment = selectItem.DeptLocation.RefDepartment;
                Alloc.SelectedDeptLocation = selectItem.DeptLocation;
                Alloc.SelectedDeptLocation = selectItem.DeptLocation;
                Alloc.IsReSelectBed = IsReSelectBed;
            };
            GlobalsNAV.ShowDialog<IBedPatientAlloc>(onInitDlg);
        }

        //public void hplOutDepartment_Click(InPatientDeptDetail selectItem)
        //{
        //    if (AdmissionInfo == null || selectItem == null || selectItem.DeptLocation == null || selectItem.DeptLocation.RefDepartment == null)
        //    {
        //        return;
        //    }

        //    if (!CheckResponsibility(selectItem.DeptLocation.DeptID))
        //    {
        //        MessageBox.Show(string.Format("{0} ", eHCMSResources.A0182_G1_Msg_SubKhDcCauHinhTNKhoa) + selectItem.DeptLocation.RefDepartment.DeptName + ", nên không thể xuất hoặc chuyển khoa.", eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
        //        return;
        //    }

        //    if (selectItem.V_InPatientDeptStatus == AllLookupValues.InPatientDeptStatus.CHUYEN_KHOA)
        //    {
        //        MessageBox.Show(string.Format("{0}.", eHCMSResources.A0221_G1_Msg_InfoKhTheXuat_ChKhoa_BNDaCK), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
        //        return;
        //    }

        //    var vm = Globals.GetViewModel<IChangeDept>();
        //    vm.CurInPatientDeptDetail = selectItem.DeepCopy();
        //    vm.OriginalDeptLocation = selectItem.DeptLocation;
        //    vm.CurrentAdmission = AdmissionInfo;
        //    vm.LoadData();
        //    Globals.ShowDialog(vm as Conductor<object>);
        //}

        public bool CheckBedReturn(long DeptLocID)
        {
            if (CurrentRegistration == null || CurrentRegistration.BedAllocations == null || CurrentRegistration.BedAllocations.Count <= 0)
            {
                return true;
            }

            //KMx: Lấy những giường chưa trả.
            var result = CurrentRegistration.BedAllocations.Where(x => x.ResponsibleDeptLocation.DeptLocationID == DeptLocID && x.IsActive).ToList();

            if (result != null && result.Count > 0)
            {
                return false;
            }

            return true;
        }

        public void hplOutDepartment_Click(InPatientDeptDetail selectItem)
        {
            if (selectItem == null || selectItem.DeptLocation == null || selectItem.DeptLocation.RefDepartment == null)
            {
                return;
            }

            if (!CheckResponsibility(selectItem.DeptLocation.DeptID))
            {
                MessageBox.Show(string.Format("{0} ", eHCMSResources.A0182_G1_Msg_SubKhDcCauHinhTNKhoa) + selectItem.DeptLocation.RefDepartment.DeptName + string.Format(". {0}!", eHCMSResources.A0184_G1_Msg_InfoKhTheThaoTac), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }

            if (CurrentRegistration != null && Globals.IsLockRegistration(CurrentRegistration.RegLockFlag, eHCMSResources.Z1424_G1_XuatKhoaPhg))
            {
                return;
            }

            if (!CheckBedReturn(selectItem.DeptLocation.DeptLocationID))
            {
                MessageBox.Show(string.Format("{0}.", eHCMSResources.A0237_G1_Msg_InfoBNChuaTraGiuong_KTheXKhoa), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }

            if (selectItem.V_InPatientDeptStatus == AllLookupValues.InPatientDeptStatus.XUAT_KHOA_PHONG)
            {
                MessageBox.Show(string.Format("{0}.", eHCMSResources.A0225_G1_Msg_InfoKhTheXuatKhoa_BNDaXuatKhoa), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }

            if (MessageBox.Show(eHCMSResources.A0175_G1_Msg_ConfXuatBNKhoiKhoaPg, eHCMSResources.G2363_G1_XNhan, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                Coroutine.BeginExecute(new_OutDepartment(selectItem));
            }

        }

        MessageWarningShowDialogTask warningtask = null;
        private IEnumerator<IResult> new_OutDepartment(InPatientDeptDetail InPtDeptDetail)
        {
            // Hpt 05/11/2015: Anh tuấn nói không được ngăn người ta lại. Kể cả khi không có chẩn đoán, vẫn cho phép xuất khoa. Chỉ cần hiện thông báo yêu cầu người dùng xác nhận
            // Kiểm tra chẩn đoán xuất khoa
            if (InPtDeptDetail.DocTypeRequired == (long)AllLookupValues.V_DocTypeRequired.CD_XUAT_KHOA && InPtDeptDetail.DocTypeRequired_Status == (long)AllLookupValues.DocTypeRequired_Status.CHUA_HOAN_THANH)
            {
                warningtask = new MessageWarningShowDialogTask(string.Format("{0}!", eHCMSResources.Z0337_G1_ChuaCoCDoanXKhoa), string.Format("{0}!", eHCMSResources.Z0338_G1_XNhanTiepTucXKhoa));
                yield return warningtask;
                if (!warningtask.IsAccept)
                {
                    yield break;
                }
            }

            //var vm = Globals.GetViewModel<IAcceptChangeDept>();
            //vm.CurrentRegistration = CurrentRegistration;
            //vm.SetModInPtDeptDetails(InPtDeptDetail);
            //vm.IsOpenToDischarge = true;
            //vm.LoadData();            
            //Globals.ShowDialog(vm as Conductor<object>);

            Action<IAcceptChangeDept> onInitDlg = (Alloc) =>
            {
                Alloc.CurrentRegistration = CurrentRegistration;
                Alloc.SetModInPtDeptDetails(InPtDeptDetail);
                Alloc.IsOpenToDischarge = true;
                Alloc.LoadData();
            };
            GlobalsNAV.ShowDialog<IAcceptChangeDept>(onInitDlg);
        }


        DataGrid grid = null;
        public void grid_Loaded(object sender, RoutedEventArgs e)
        {
            grid = sender as DataGrid;

            if (grid == null)
            {
                return;
            }

            var colBookingBed = grid.GetColumnByName("colBookingBed");

            var colOutDept = grid.GetColumnByName("colOutDept");

            var colDelDept = grid.GetColumnByName("colDelDeptDetail");

            var colEditDept = grid.GetColumnByName("colEditDeptDetail");

            if (colBookingBed == null || colOutDept == null)
            {
                return;
            }

            if (ShowBookingBedColumn)
            {
                colBookingBed.Visibility = Visibility.Visible;
            }
            else
            {
                colBookingBed.Visibility = Visibility.Collapsed;
            }

            if (ShowOutDeptColumn)
            {
                colOutDept.Visibility = Visibility.Visible;
            }
            else
            {
                colOutDept.Visibility = Visibility.Collapsed;
            }

            if (colDelDept != null)
            {
                if (ShowDeleteEditColumn)
                {
                    colDelDept.Visibility = Visibility.Visible;
                }
                else
                {
                    colDelDept.Visibility = Visibility.Collapsed;
                }
            }

            if (ShowDeleteEditColumn)
            {
                if (colDelDept != null)
                {
                    colDelDept.Visibility = Visibility.Visible;
                }
                if (colEditDept != null)
                {
                    colEditDept.Visibility = Visibility.Visible;
                }                
            }
            else
            {
                if (colDelDept != null)
                {
                    colDelDept.Visibility = Visibility.Collapsed;
                }
                if (colEditDept != null)
                {
                    colEditDept.Visibility = Visibility.Collapsed;
                }
            }
        }

        public void DeleteDeptLocationCmd(InPatientDeptDetail selectItem, object eventArgs)
        {
            if (!CheckResponsibility(selectItem.DeptLocation.DeptID))
            {
                MessageBox.Show(string.Format("{0} ", eHCMSResources.A0182_G1_Msg_SubKhDcCauHinhTNKhoa) + selectItem.DeptLocation.RefDepartment.DeptName + string.Format(". {0}!", eHCMSResources.A0184_G1_Msg_InfoKhTheThaoTac), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
            if (Globals.IsLockRegistration(CurrentRegistration.RegLockFlag, eHCMSResources.Z1422_G1_XoaCTietNhapKhoa))
            {
                return;
            }
        }

        public void EditDeptLocationCmd(InPatientDeptDetail selectItem, object eventArgs)
        {
            if (!CheckResponsibility(selectItem.DeptLocation.DeptID))
            {
                MessageBox.Show(string.Format("{0} ", eHCMSResources.A0182_G1_Msg_SubKhDcCauHinhTNKhoa) + selectItem.DeptLocation.RefDepartment.DeptName + string.Format(". {0}!", eHCMSResources.A0184_G1_Msg_InfoKhTheThaoTac), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
            if (Globals.IsLockRegistration(CurrentRegistration.RegLockFlag, eHCMSResources.Z1423_G1_CNhatCTietNXKhoa))
            {
                return;
            }
           //Huyen 11/08/2015: thuộc tính IsAdmittedRecord đánh dấu dòng thông tin nhập viện.
           // Đoạn lệnh bên dưới kiểm tra xem dòng thông tin đang thao tác có phải là dòng thông tin nhập viện hay không
           //Nếu không là dòng nhập viện, tức thuộc tính IsAdmittedRecord = true, không được phép chỉnh sửa  từ chi tiết nhập khoa mà phải thay đổi từ chi tiết nhập viện

            if ( selectItem.IsAdmittedRecord == true && selectItem.ToDate.HasValue == false)
            {
                MessageBox.Show(string.Format("{0}\n{1}", eHCMSResources.A1040_G1_Msg_InfoLienQuanCTietNpVien, eHCMSResources.A1042_G1_Msg_InfoSuaCTietNpVien), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
            //var vm = Globals.GetViewModel<IAcceptChangeDept>();
            //vm.DlgUsageMode = 1;
            //vm.CurrentRegistration = CurrentRegistration;
            //vm.IsFromRequestPaper = false;
            //vm.SetModInPtDeptDetails(selectItem);
            ////Danh sách Khoa của nhân viên được cấu hình trách nhiệm (11/07/2014 16:23).
            ////vm.LstRefDepartment = LstRefDepartment;
            //vm.LoadData();
            //vm.CurInPatientTransferDeptReq = new InPatientTransferDeptReq();
            //vm.CurInPatientTransferDeptReq.InPatientAdmDisDetailID = CurrentRegistration.AdmissionInfo.InPatientAdmDisDetailID;
            //vm.CurInPatientTransferDeptReq.InPatientTransferDeptReqID = 0;
            //Globals.ShowDialog(vm as Conductor<object>);

            Action<IAcceptChangeDept> onInitDlg = (Alloc) =>
            {
                Alloc.DlgUsageMode = 1;
                Alloc.CurrentRegistration = CurrentRegistration;
                Alloc.IsFromRequestPaper = false;
                Alloc.SetModInPtDeptDetails(selectItem);
                Alloc.LoadData();
                Alloc.CurInPatientTransferDeptReq = new InPatientTransferDeptReq();
                Alloc.CurInPatientTransferDeptReq.InPatientAdmDisDetailID = CurrentRegistration.AdmissionInfo.InPatientAdmDisDetailID;
                Alloc.CurInPatientTransferDeptReq.InPatientTransferDeptReqID = 0;
            };
            GlobalsNAV.ShowDialog<IAcceptChangeDept>(onInitDlg);
        }

    }
}
