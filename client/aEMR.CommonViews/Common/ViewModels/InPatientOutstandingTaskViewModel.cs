using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ServiceModel;
using System.Threading;
using aEMR.DataContracts;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Caliburn.Micro;
using DataEntities;
using System.Windows;
using System.Collections.ObjectModel;
using System.Windows.Input;
using aEMR.Controls;
using eHCMSLanguage;
using aEMR.Common.BaseModel;
using System.Windows.Controls;
using System.Linq;
/*
* 20190926 #001 TTM:   BM 0016375: [aEMR]: Thêm cảnh báo khi chuyển bệnh nhân bằng out standing task nội trú 
* 20191115 #002 TTM:   BM 0019569: Bổ sung thêm 1 danh sách loại BN điều trị ngoại trú
* 20210416 #003 TNHX:   Bổ sung thêm 1 danh sách BN nội trú bị trả hồ sơ 
* 20220321 #004 QTD:   Bổ sung thêm 1 danh sách quản lý BN tạm ứng
* 20220331 #005 DatTB:   Thêm điều kiện lấy hồ sơ mới gửi, đã duyệt lên danh sách.
* 20220331 #006 DatTB:   Thêm điều kiện hiển thị ghi chú riêng cho tab DS BN kiểm duyệt hồ sơ.
* 20220711 #007 DatTB: 
* + Thay đổi trạng thái khi gửi lần 2.
* + Màn hình nội trú thêm điều kiện gửi lần 2.
* 20221020 #008 QTD:   Bổ sung sự kiện Double sách BN bên phải màn hình Tính lại bill viện phí
*/
namespace aEMR.Common.ViewModels
{
    [Export(typeof(IInPatientOutstandingTask)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class InPatientOutstandingTaskViewModel : ViewModelBase, IInPatientOutstandingTask
    {
        [ImportingConstructor]
        public InPatientOutstandingTaskViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAgr, ISalePosCaching salePosCaching)
        {
            DepartmentContent = Globals.GetViewModel<IDepartmentListing>();
            DepartmentContent.AddSelectOneItem = true;
            DepartmentContent.LoadData();
            //▼====== 21-11-2018: TTM Do không còn chọn nội ngoại trú bằng radio button nữa nên set cứng giá trị là nội trú khi out standing task nội trú đc bật.
            Globals.PatientFindBy_ForConsultation = AllLookupValues.PatientFindBy.NOITRU;
            //▲======
        }
        protected override void OnActivate()
        {
            base.OnActivate();
            Globals.EventAggregator.Subscribe(this);
            DepartmentContent = Globals.GetViewModel<IDepartmentListing>();
            if(IsShowListPatientCashAdvance)
            {
                if (IsEnableDepartmentContent)
                {
                    DepartmentContent.AddSelectOneItem = false;
                }
                else
                {
                    DepartmentContent.AddSelectedAllItem = true;
                }
            }          
            DepartmentContent.LoadData();
            DepartmentContent.SelectedItem = DepartmentContent.Departments.FirstOrDefault();
            LoadPatientListCashAdvanceByDefault();
        }
        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);
            Globals.EventAggregator.Unsubscribe(this);
        }
        private IDepartmentListing _departmentContent;
        public IDepartmentListing DepartmentContent
        {
            get { return _departmentContent; }
            set
            {
                _departmentContent = value;
                NotifyOfPropertyChange(() => DepartmentContent);
            }
        }
        private SetOutStandingTask _WhichVM = 0;
        public SetOutStandingTask WhichVM
        {
            get { return _WhichVM; }
            set
            {
                if (_WhichVM != value)
                {
                    _WhichVM = value;
                }
                NotifyOfPropertyChange(() => WhichVM);
            }
        }
        private ObservableCollection<PatientRegistrationDetail> _InPatientList;
        public ObservableCollection<PatientRegistrationDetail> InPatientList
        {
            get { return _InPatientList; }
            private set
            {
                _InPatientList = value;
                if (_InPatientList != null || _InPatientList.Count > 0)
                {
                    IsEnableSearch = true;
                }
                else
                {
                    IsEnableSearch = false;
                }
                NotifyOfPropertyChange(() => InPatientList);
            }
        }
      
        private PatientRegistrationDetail _SelectedItem ;
        public PatientRegistrationDetail SelectedItem
        {
            get { return _SelectedItem; }
            set
            {
                if (_SelectedItem != value)
                {
                    _SelectedItem = value;
                }
                NotifyOfPropertyChange(() => SelectedItem);
            }
        }
        public void SearchInPatientRegistrationListForOST()
        {
            this.ShowBusyIndicator(eHCMSResources.K2871_G1_DangLoadDLieu);
            var t = new Thread(() =>
            {
                AxErrorEventArgs error = null;
                try
                {
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginSearchInPatientRegistrationListForOST(DepartmentContent.SelectedItem.DeptID, IsSearchForListPatientCashAdvance, Globals.DispatchCallback((asyncResult) =>
                        {
                            IList<PatientRegistrationDetail> allItems = null;
                            InPatientList = new ObservableCollection<PatientRegistrationDetail>();
                            OutPatientList = new ObservableCollection<PatientRegistrationDetail>();
                            InPatientListReturnRecords = new ObservableCollection<PatientRegistrationDetail>();
                            try
                            {
                                allItems = client.EndSearchInPatientRegistrationListForOST(asyncResult);
                            }
                            catch (FaultException<AxException> fault)
                            {
                                error = new AxErrorEventArgs(fault);
                                this.HideBusyIndicator();
                            }
                            catch (Exception ex)
                            {
                                error = new AxErrorEventArgs(ex);
                                this.HideBusyIndicator();
                            }
                            if (allItems != null && allItems.Count > 0)
                            {
                                foreach (var tmpList in allItems)
                                {
                                    //▼===== #002: AllLookupValues.V_RegForPatientOfType.Unknown là bệnh nhân nhập viện bình thường từ nhận bệnh hoặc đề nghị.
                                    if (tmpList.PatientRegistration.V_RegForPatientOfType == AllLookupValues.V_RegForPatientOfType.Unknown)
                                    {
                                        InPatientList.Add(tmpList);
                                    }
                                    else if(tmpList.PatientRegistration.V_RegForPatientOfType == AllLookupValues.V_RegForPatientOfType.DKBN_DT_NGOAI_TRU)
                                    {
                                        OutPatientList.Add(tmpList);
                                    }
                                    //▲===== #002
                                    //▼====: #003
                                    if (tmpList.PatientRegistration.V_RegForPatientOfType == AllLookupValues.V_RegForPatientOfType.Unknown 
                                        && IsShowListInPatientReturnRecord 
                                        && tmpList.PatientRegistration != null
                                        //▼====: #004, #007
                                        && (tmpList.PatientRegistration.V_CheckMedicalFilesStatus == (long)AllLookupValues.V_CheckMedicalFilesStatus.Tao_Moi
                                            || tmpList.PatientRegistration.DLSReject == true
                                            || tmpList.PatientRegistration.V_CheckMedicalFilesStatus == (long)AllLookupValues.V_CheckMedicalFilesStatus.Tra_Ho_So
                                            || tmpList.PatientRegistration.V_CheckMedicalFilesStatus == (long)AllLookupValues.V_CheckMedicalFilesStatus.Da_Duyet
                                            || tmpList.PatientRegistration.V_CheckMedicalFilesStatus == (long)AllLookupValues.V_CheckMedicalFilesStatus.Cho_Duyet_Lai))
                                        //▲====: #004, #007
                                    {
                                        InPatientListReturnRecords.Add(tmpList);
                                    }
                                    //▲====: #003
                                }
                            }
                            tmpInPatientList = InPatientList.DeepCopy();
                            tmpOutPatientList = OutPatientList.DeepCopy();
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    error = new AxErrorEventArgs(ex);
                }
                finally
                {
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }
        public void DoubleClick(object args)
        {
            EventArgs<object> eventArgs = args as EventArgs<object>;
            //▼===== #001
            //if (SelectedItem != null && SelectedItem.PatientRegistration != null
            //     && SelectedItem.PatientRegistration.PatientID > 0 && SelectedItem.PatientRegistration.Patient != null
            //     && eventArgs != null && eventArgs.Value != null && (eventArgs.Value as PatientRegistrationDetail).PatientRegistration != null
            //     && (eventArgs.Value as PatientRegistrationDetail).PatientRegistration.PatientID > 0 && (eventArgs.Value as PatientRegistrationDetail).PatientRegistration.Patient != null)
            //{
            //    if (!EqualsPatient(SelectedItem, (eventArgs.Value as PatientRegistrationDetail)))
            //    {
            //        var str = string.Format("{0} '{1}'.\n{2} '{3}'", eHCMSResources.Z0175_G1_BanDangThaoTacBN, SelectedItem.PatientRegistration.Patient.FullName, eHCMSResources.Z0191_G1_BanCoMuonChSangDKBN, (eventArgs.Value as PatientRegistrationDetail).PatientRegistration.Patient.FullName);

            //        if (MessageBox.Show(str, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
            //        {
            //            return;
            //        }
            //    }
            //}
            //▲===== #001
            SelectedItem = eventArgs.Value as PatientRegistrationDetail;
            

            if (WhichVM == SetOutStandingTask.KHAMBENH || WhichVM == SetOutStandingTask.RATOA 
                || WhichVM == SetOutStandingTask.PHIEU_YEU_CAU_CLS || WhichVM == SetOutStandingTask.PHIEU_YEU_CAU_XET_NGHIEM || WhichVM == SetOutStandingTask.XUATVIEN)
            {
                Globals.EventAggregator.Publish(new InPatientRegistrationSelectedForConsultation() { Source = SelectedItem.PatientRegistration });
            }
            else if (WhichVM == SetOutStandingTask.QUANLY_BENHNHAN_NOITRU)
            {
                Globals.EventAggregator.Publish(new InPatientRegistrationSelectedForInPatientAdmission() { Source = SelectedItem.PatientRegistration });
            }
            else if (WhichVM == SetOutStandingTask.TAO_BILL_VP)
            {
                Globals.EventAggregator.Publish(new InPatientRegistrationSelectedForInPatientRegistration() { Source = SelectedItem.PatientRegistration });
            }
            else if (WhichVM == SetOutStandingTask.XUATTHUOC)
            {
                Globals.EventAggregator.Publish(new InPatientRegistrationSelectedForOutwardToPatient_V2() { Source = SelectedItem.PatientRegistration });
            }
            else if (WhichVM == SetOutStandingTask.THANHTOAN)
            {
                Globals.EventAggregator.Publish(new InPatientRegistrationSelectedForInPatientPayment() { Source = SelectedItem.PatientRegistration });
            }
            else if (WhichVM == SetOutStandingTask.DIEU_TRI_NHIEM_KHUAN)
            {
                Globals.EventAggregator.Publish(new InPatientRegistrationSelectedForInfectionCase() { Source = SelectedItem.PatientRegistration });
            }
            else if (WhichVM == SetOutStandingTask.XACNHAN_BHYT)
            {
                Globals.EventAggregator.Publish(new InPatientRegistrationSelectedForConfirmHI() { Source = SelectedItem.PatientRegistration });
            }
            //▼===== #008
            else if (WhichVM == SetOutStandingTask.TINH_LAI_BILL_VP)
            {
                Globals.EventAggregator.Publish(new InPatientRegistrationSelectedForInPatientBillingInvoiceListing() { Source = SelectedItem.PatientRegistration });
            }
            //▲===== #008
        }
        public void hplRefresh()
        {
            if (DepartmentContent.SelectedItem.DeptID == 0 && IsEnableDepartmentContent == true)
            {
                MessageBox.Show(eHCMSResources.K0340_G1_ChonKhoa, eHCMSResources.K1576_G1_CBao);
                return;
            }
            else
            {
                SearchInPatientRegistrationListForOST();
            }
        }
        private bool _IsEnableSearch = false;
        public bool IsEnableSearch
        {
            get { return _IsEnableSearch; }
            set
            {
                _IsEnableSearch = value;
                NotifyOfPropertyChange(() => IsEnableSearch);
            }
        }
        private ObservableCollection<PatientRegistrationDetail> _tmpInPatientList;
        public ObservableCollection<PatientRegistrationDetail> tmpInPatientList
        {
            get { return _tmpInPatientList; }
            private set
            {
                _tmpInPatientList = value;
                NotifyOfPropertyChange(() => tmpInPatientList);
            }
        }
        public void hplSearch()
        {
            if (string.IsNullOrEmpty(txtSearchName.Text))
            {
                if (InPatientList.Count < tmpInPatientList.Count)
                {
                    InPatientList = tmpInPatientList;
                }
                txtSearchName.Clear();
                return;
            }
            //▼===== #002: sử dụng TCRegistrationInfo.SelectedIndex == 0 vì SelectionChanged của TabControl lỗi gọi 2 lần không biết nguyên nhân.
            if (TCRegistrationInfo == null)
            {
                return;
            }
            if (TCRegistrationInfo.SelectedIndex == 0 || TCRegistrationInfo.SelectedIndex == 3)
            {
                if (InPatientList == null)
                {
                    return;
                }
                else
                {
                    string tmpStrEqual = Globals.RemoveVietnameseString(txtSearchName.Text);
                    if (InPatientList.Count < tmpInPatientList.Count)
                    {
                        InPatientList = tmpInPatientList;
                    }
                    ObservableCollection<PatientRegistrationDetail> tmpListPatient = new ObservableCollection<PatientRegistrationDetail>();
                    foreach (var tmpPatient in InPatientList)
                    {
                        string tmpName = Globals.RemoveVietnameseString(tmpPatient.PatientRegistration.Patient.FullName.ToUpper());
                        if (tmpName.ToUpper().Contains(tmpStrEqual.ToUpper()))
                        {
                            tmpListPatient.Add(tmpPatient);
                        }
                    }
                    InPatientList = tmpListPatient;
                }
            }
            else
            {
                if (OutPatientList == null)
                {
                    return;
                }
                else
                {
                    string tmpStrEqualOutPatient = Globals.RemoveVietnameseString(txtSearchName.Text);
                    if (OutPatientList.Count < tmpOutPatientList.Count)
                    {
                        OutPatientList = tmpOutPatientList;
                    }
                    ObservableCollection<PatientRegistrationDetail> tmpListPatientOutPatient = new ObservableCollection<PatientRegistrationDetail>();
                    foreach (var tmpPatient in OutPatientList)
                    {
                        string tmpName = Globals.RemoveVietnameseString(tmpPatient.PatientRegistration.Patient.FullName.ToUpper());
                        if (tmpName.ToUpper().Contains(tmpStrEqualOutPatient.ToUpper()))
                        {
                            tmpListPatientOutPatient.Add(tmpPatient);
                        }
                    }
                    OutPatientList = tmpListPatientOutPatient;
                }
            }
            //▲===== #002
        }

        AxTextBox txtSearchName = null;
        public void txtSearchName_Loaded(object sender, RoutedEventArgs e)
        {
            txtSearchName = sender as AxTextBox;
        }

        public void txtSearchName_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                hplSearch();
            }
        }
        //▼===== #001
        public bool EqualsPatient(PatientRegistrationDetail PatientRegistrationDetail_1st, PatientRegistrationDetail PatientRegistrationDetail_2nd)
        {
            if (PatientRegistrationDetail_1st.PatientRegistration.PatientID != PatientRegistrationDetail_2nd.PatientRegistration.PatientID)
            {
                return false;
            }
            return true;
        }
        //▲===== #001

        //▼===== #002
        #region Danh sách bệnh nhân điều trị ngoại trú
        private ObservableCollection<PatientRegistrationDetail> _tmpOutPatientList;
        public ObservableCollection<PatientRegistrationDetail> tmpOutPatientList
        {
            get { return _tmpOutPatientList; }
            private set
            {
                _tmpOutPatientList = value;
                NotifyOfPropertyChange(() => tmpOutPatientList);
            }
        }
        private ObservableCollection<PatientRegistrationDetail> _OutPatientList;
        public ObservableCollection<PatientRegistrationDetail> OutPatientList
        {
            get { return _OutPatientList; }
            private set
            {
                _OutPatientList = value;
                if (_OutPatientList != null || _OutPatientList.Count > 0)
                {
                    IsEnableSearch = true;
                }
                else
                {
                    IsEnableSearch = false;
                }
                NotifyOfPropertyChange(() => OutPatientList);
            }
        }
        public void DoubleClickForOutPatientList(object args)
        {
            EventArgs<object> eventArgs = args as EventArgs<object>;
            if (SelectedItem != null && SelectedItem.PatientRegistration != null
                 && SelectedItem.PatientRegistration.PatientID > 0 && SelectedItem.PatientRegistration.Patient != null
                 && eventArgs != null && eventArgs.Value != null && (eventArgs.Value as PatientRegistrationDetail).PatientRegistration != null
                 && (eventArgs.Value as PatientRegistrationDetail).PatientRegistration.PatientID > 0 && (eventArgs.Value as PatientRegistrationDetail).PatientRegistration.Patient != null)
            {
                if (!EqualsPatient(SelectedItem, (eventArgs.Value as PatientRegistrationDetail)))
                {
                    var str = string.Format("{0} '{1}'.\n{2} '{3}'", eHCMSResources.Z0175_G1_BanDangThaoTacBN, SelectedItem.PatientRegistration.Patient.FullName, eHCMSResources.Z0191_G1_BanCoMuonChSangDKBN, (eventArgs.Value as PatientRegistrationDetail).PatientRegistration.Patient.FullName);

                    if (MessageBox.Show(str, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                    {
                        return;
                    }
                }
            }
            SelectedItem = eventArgs.Value as PatientRegistrationDetail;


            if (WhichVM == SetOutStandingTask.KHAMBENH || WhichVM == SetOutStandingTask.RATOA
                || WhichVM == SetOutStandingTask.PHIEU_YEU_CAU_CLS || WhichVM == SetOutStandingTask.PHIEU_YEU_CAU_XET_NGHIEM || WhichVM == SetOutStandingTask.XUATVIEN)
            {
                Globals.EventAggregator.Publish(new InPatientRegistrationSelectedForConsultation() { Source = SelectedItem.PatientRegistration });
            }
            else if (WhichVM == SetOutStandingTask.QUANLY_BENHNHAN_NOITRU)
            {
                Globals.EventAggregator.Publish(new InPatientRegistrationSelectedForInPatientAdmission() { Source = SelectedItem.PatientRegistration });
            }
            else if (WhichVM == SetOutStandingTask.TAO_BILL_VP)
            {
                Globals.EventAggregator.Publish(new InPatientRegistrationSelectedForInPatientRegistration() { Source = SelectedItem.PatientRegistration });
            }
            else if (WhichVM == SetOutStandingTask.XUATTHUOC)
            {
                Globals.EventAggregator.Publish(new InPatientRegistrationSelectedForOutwardToPatient_V2() { Source = SelectedItem.PatientRegistration });
            }
            else if (WhichVM == SetOutStandingTask.THANHTOAN)
            {
                Globals.EventAggregator.Publish(new InPatientRegistrationSelectedForInPatientPayment() { Source = SelectedItem.PatientRegistration });
            }
            else if (WhichVM == SetOutStandingTask.DIEU_TRI_NHIEM_KHUAN)
            {
                Globals.EventAggregator.Publish(new InPatientRegistrationSelectedForInfectionCase() { Source = SelectedItem.PatientRegistration });
            }
            else if (WhichVM == SetOutStandingTask.XACNHAN_BHYT)
            {
                Globals.EventAggregator.Publish(new InPatientRegistrationSelectedForConfirmHI() { Source = SelectedItem.PatientRegistration });
            }
        }
        TabControl TCRegistrationInfo { get; set; }
        public void TCRegistrationInfo_Loaded(object sender, RoutedEventArgs e)
        {
            TCRegistrationInfo = sender as TabControl;
        }
        #endregion
        //▲===== #002
        
        //▼===== #003
        #region Danh sách bệnh nhân điều trị ngoại trú
        private ObservableCollection<PatientRegistrationDetail> _TmpInPatientListReturnRecords;
        public ObservableCollection<PatientRegistrationDetail> TmpInPatientListReturnRecords
        {
            get { return _TmpInPatientListReturnRecords; }
            private set
            {
                _TmpInPatientListReturnRecords = value;
                NotifyOfPropertyChange(() => TmpInPatientListReturnRecords);
            }
        }

        private ObservableCollection<PatientRegistrationDetail> _InPatientListReturnRecords;
        public ObservableCollection<PatientRegistrationDetail> InPatientListReturnRecords
        {
            get { return _InPatientListReturnRecords; }
            private set
            {
                _InPatientListReturnRecords = value;
                if (_InPatientListReturnRecords != null || _InPatientListReturnRecords.Count > 0)
                {
                    IsEnableSearch = true;
                }
                else
                {
                    IsEnableSearch = false;
                }
                NotifyOfPropertyChange(() => InPatientListReturnRecords);
            }
        }

        private bool _IsShowListOutPatientList = true;
        public bool IsShowListOutPatientList
        {
            get
            {
                return _IsShowListOutPatientList;
            }
            set
            {
                if (_IsShowListOutPatientList != value)
                {
                    _IsShowListOutPatientList = value;
                    NotifyOfPropertyChange(() => IsShowListOutPatientList);
                }
            }
        }

        private bool _IsShowListInPatientReturnRecord = false;
        public bool IsShowListInPatientReturnRecord
        {
            get
            {
                return _IsShowListInPatientReturnRecord;
            }
            set
            {
                if (_IsShowListInPatientReturnRecord != value)
                {
                    _IsShowListInPatientReturnRecord = value;
                    NotifyOfPropertyChange(() => IsShowListInPatientReturnRecord);
                }
            }
        }

        //▼====: #005
        private bool _IsShowListInPatientReturnRecordSelected = false;
        public bool IsShowListInPatientReturnRecordSelected
        {
            get
            {
                return _IsShowListInPatientReturnRecordSelected;
            }
            set
            {
                if (_IsShowListInPatientReturnRecordSelected != value)
                {
                    _IsShowListInPatientReturnRecordSelected = value;
                    NotifyOfPropertyChange(() => IsShowListInPatientReturnRecordSelected);
                }
            }
        }
        //▲====: #005

        public void DoubleClickInPatientReturnRecord(object args)
        {
            EventArgs<object> eventArgs = args as EventArgs<object>;
            if (SelectedItem != null && SelectedItem.PatientRegistration != null
                 && SelectedItem.PatientRegistration.PatientID > 0 && SelectedItem.PatientRegistration.Patient != null
                 && eventArgs != null && eventArgs.Value != null && (eventArgs.Value as PatientRegistrationDetail).PatientRegistration != null
                 && (eventArgs.Value as PatientRegistrationDetail).PatientRegistration.PatientID > 0 && (eventArgs.Value as PatientRegistrationDetail).PatientRegistration.Patient != null)
            {
                if (!EqualsPatient(SelectedItem, (eventArgs.Value as PatientRegistrationDetail)))
                {
                    var str = string.Format("{0} '{1}'.\n{2} '{3}'", eHCMSResources.Z0175_G1_BanDangThaoTacBN, SelectedItem.PatientRegistration.Patient.FullName, eHCMSResources.Z0191_G1_BanCoMuonChSangDKBN, (eventArgs.Value as PatientRegistrationDetail).PatientRegistration.Patient.FullName);

                    if (MessageBox.Show(str, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                    {
                        return;
                    }
                }
            }

            SelectedItem = eventArgs.Value as PatientRegistrationDetail;

            if (WhichVM == SetOutStandingTask.KHAMBENH || WhichVM == SetOutStandingTask.RATOA
                || WhichVM == SetOutStandingTask.PHIEU_YEU_CAU_CLS || WhichVM == SetOutStandingTask.PHIEU_YEU_CAU_XET_NGHIEM || WhichVM == SetOutStandingTask.XUATVIEN)
            {
                Globals.EventAggregator.Publish(new InPatientRegistrationSelectedForConsultation() { Source = SelectedItem.PatientRegistration });
            }
            else if (WhichVM == SetOutStandingTask.QUANLY_BENHNHAN_NOITRU)
            {
                Globals.EventAggregator.Publish(new InPatientRegistrationSelectedForInPatientAdmission() { Source = SelectedItem.PatientRegistration });
            }
            else if (WhichVM == SetOutStandingTask.TAO_BILL_VP)
            {
                Globals.EventAggregator.Publish(new InPatientRegistrationSelectedForInPatientRegistration() { Source = SelectedItem.PatientRegistration });
            }
            else if (WhichVM == SetOutStandingTask.XUATTHUOC)
            {
                Globals.EventAggregator.Publish(new InPatientRegistrationSelectedForOutwardToPatient_V2() { Source = SelectedItem.PatientRegistration });
            }
            else if (WhichVM == SetOutStandingTask.THANHTOAN)
            {
                Globals.EventAggregator.Publish(new InPatientRegistrationSelectedForInPatientPayment() { Source = SelectedItem.PatientRegistration });
            }
            else if (WhichVM == SetOutStandingTask.DIEU_TRI_NHIEM_KHUAN)
            {
                Globals.EventAggregator.Publish(new InPatientRegistrationSelectedForInfectionCase() { Source = SelectedItem.PatientRegistration });
            }
            else if (WhichVM == SetOutStandingTask.XACNHAN_BHYT)
            {
                Globals.EventAggregator.Publish(new InPatientRegistrationSelectedForConfirmHI() { Source = SelectedItem.PatientRegistration });
            }
        }
        #endregion
        //▲===== #003

        //▼===== #004
        private bool _IsShowListInPatient = true;
        public bool IsShowListInPatient
        {
            get { return _IsShowListInPatient; }
            set
            {
                if(_IsShowListInPatient != value)
                {
                    _IsShowListInPatient = value;
                    NotifyOfPropertyChange(() => IsShowListInPatient);
                }
            }
        }

        //▼====: #005
        private bool _IsShowListInPatientSelected = true;
        public bool IsShowListInPatientSelected
        {
            get { return _IsShowListInPatientSelected; }
            set
            {
                if (_IsShowListInPatientSelected != value)
                {
                    _IsShowListInPatientSelected = value;
                    NotifyOfPropertyChange(() => IsShowListInPatientSelected);
                }
            }
        }
        //▲====: #005

        private bool _IsEnableDepartmentContent = true;
        public bool IsEnableDepartmentContent
        {
            get { return _IsEnableDepartmentContent; }
            set
            {
                if (_IsEnableDepartmentContent != value)
                {
                    _IsEnableDepartmentContent = value;
                    NotifyOfPropertyChange(() => IsEnableDepartmentContent);
                }
            }
        }

        private bool _IsShowListPatientCashAdvance = false;
        public bool IsShowListPatientCashAdvance
        {
            get
            {
                return _IsShowListPatientCashAdvance;
            }
            set
            {
                if(_IsShowListPatientCashAdvance != value)
                {
                    _IsShowListPatientCashAdvance = value;   
                    NotifyOfPropertyChange(() => IsShowListPatientCashAdvance);
                }
            }
        }

        private bool _IsSearchForListPatientCashAdvance = false;
        public bool IsSearchForListPatientCashAdvance
        {
            get
            {
                return _IsSearchForListPatientCashAdvance;
            }
            set
            {
                if (_IsSearchForListPatientCashAdvance != value)
                {
                    _IsSearchForListPatientCashAdvance = value;
                    NotifyOfPropertyChange(() => IsSearchForListPatientCashAdvance);
                }
            }
        }

        public void DoubleClickForAdvancePatientList(object args)
        {
            EventArgs<object> eventArgs = args as EventArgs<object>;
            //if (SelectedItem != null && SelectedItem.PatientRegistration != null
            //     && SelectedItem.PatientRegistration.PatientID > 0 && SelectedItem.PatientRegistration.Patient != null
            //     && eventArgs != null && eventArgs.Value != null && (eventArgs.Value as PatientRegistrationDetail).PatientRegistration != null
            //     && (eventArgs.Value as PatientRegistrationDetail).PatientRegistration.PatientID > 0 && (eventArgs.Value as PatientRegistrationDetail).PatientRegistration.Patient != null)
            //{
            //    if (!EqualsPatient(SelectedItem, (eventArgs.Value as PatientRegistrationDetail)))
            //    {
            //        var str = string.Format("{0} '{1}'.\n{2} '{3}'", eHCMSResources.Z0175_G1_BanDangThaoTacBN, SelectedItem.PatientRegistration.Patient.FullName, eHCMSResources.Z0191_G1_BanCoMuonChSangDKBN, (eventArgs.Value as PatientRegistrationDetail).PatientRegistration.Patient.FullName);

            //        if (MessageBox.Show(str, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
            //        {
            //            return;
            //        }
            //    }
            //}
            SelectedItem = eventArgs.Value as PatientRegistrationDetail;
            if (WhichVM == SetOutStandingTask.THUTIEN_TAMUNG || WhichVM == SetOutStandingTask.DENGHI_TAMUNG)
            {
                Globals.EventAggregator.Publish(new InPatientRegistrationSelectedForInPatientCashAdvance() { Source = SelectedItem.PatientRegistration });
            }
            else
            {
                return;
            }
        }

        public void LoadPatientListCashAdvanceByDefault()
        {
            if(IsShowListPatientCashAdvance)
            {
                if (DepartmentContent == null || (IsEnableDepartmentContent && DepartmentContent.SelectedItem.DeptID == 0))
                {
                    return;
                }
                SearchInPatientRegistrationListForOST();
            }
            return;
        }
        //▲===== #004
    }
}
