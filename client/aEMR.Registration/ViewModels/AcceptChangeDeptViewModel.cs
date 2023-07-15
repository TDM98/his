using eHCMSLanguage;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Threading;
using System.Windows;
using aEMR.DataContracts;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using aEMR.ViewContracts.Configuration;
using Caliburn.Micro;
using DataEntities;
using aEMR.Common;
using aEMR.CommonTasks;
using eHCMS.CommonUserControls.CommonTasks;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using System.ServiceModel;
using Castle.Windsor;
using aEMR.Infrastructure.CachingUtils;
using aEMR.Common.BaseModel;
using aEMR.Common.Collections;
/*
* 20200718 #001 TNHX: Đổi input nhập ngày qua content IMinHourDateControl
*/
namespace aEMR.Registration.ViewModels
{
    [Export(typeof(IAcceptChangeDept)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class AcceptChangeDeptViewModel : ViewModelBase, IAcceptChangeDept
        , IHandle<AddCompleted<BedPatientAllocs>>
        , IHandle<BookingBedForAcceptChangeDeptEvent>
        //, IHandle<ItemSelected<IcwdBedPatientCommon, BedPatientAllocs>>
    {
        IEventAggregator _eventArg;
        [ImportingConstructor]
        public AcceptChangeDeptViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg, ISalePosCaching salePosCaching)
        {
            _eventArg = eventArg;
            Globals.EventAggregator.Subscribe(this);
            DepartmentContent = Globals.GetViewModel<IDepartmentListing>();
            //dtInDeptFromDate = Globals.GetCurServerDateTime();
            AdmissionDateTime = Globals.GetViewModel<IMinHourDateControl>();
            AdmissionDateTime.DateTime = Globals.GetCurServerDateTime();
            DischargeDateTime = Globals.GetViewModel<IMinHourDateControl>();
            (DepartmentContent as PropertyChangedBase).PropertyChanged += new PropertyChangedEventHandler(DepartmentContent_PropertyChanged);
            LoadDeptTransferDocTypeReq();
        }

        public void LoadDeptTransferDocTypeReq()
        {
            if (Globals.allDeptTransDocTypeReq != null && Globals.allDeptTransDocTypeReq.Count > 0)
            {
                return;
            }

            this.ShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new CommonService_V2Client())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginGetAllDocTypeRequire(Globals.DispatchCallback(asyncResult =>
                        {
                            IList<DeptTransferDocReq> allItems = null;
                            try
                            {
                                allItems = contract.EndGetAllDocTypeRequire(asyncResult);
                                if (Globals.allDeptTransDocTypeReq == null)
                                {
                                    Globals.allDeptTransDocTypeReq = new List<DeptTransferDocReq>(allItems);
                                }
                            }
                            catch (Exception ex)
                            {
                                ClientLoggerHelper.LogInfo(ex.ToString());
                                MessageBox.Show(eHCMSResources.A0691_G1_Msg_InfoKhTheLayDSTaiLieuYC);
                            }
                            finally
                            {
                                this.HideBusyIndicator();
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    this.HideBusyIndicator();
                }
            });

            t.Start();
        }

        private bool _IsTreatmentCOVID = false;
        public bool IsTreatmentCOVID
        {
            get
            {
                return _IsTreatmentCOVID;
            }
            set
            {
                if (_IsTreatmentCOVID != value)
                {
                    _IsTreatmentCOVID = value;
                    NotifyOfPropertyChange(() => IsTreatmentCOVID);
                }
            }
        }

        // Hpt 30/10/2015: Biến kiểm tra VM có được gọi từ đường dẫn xuất viện không
        private bool _isOpenToDischarge = false;
        public bool IsOpenToDischarge
        {
            get
            {
                return _isOpenToDischarge;
            }
            set
            {
                if (_isOpenToDischarge != value)
                {
                    _isOpenToDischarge = value;
                    ShowDischargeDateTime = IsOpenToDischarge;
                    NotifyOfPropertyChange(() => IsOpenToDischarge);
                    NotifyOfPropertyChange(() => ShowDischargeDateTime);
                }
            }
        }

        // Hpt 01/11/2015: Vì anh Tuấn muốn khi click Xuất khoa phòng sẽ mở Popup này lên nhưng chỉ hiển thị dòng cập nhật ngày giờ xuất khoa. Viết thêm hàm này để điều chỉnh tiêu đề VM, cho người dùng biết đang thực hiện xuất khoa nào
        // Đúng ra cũng có dòng hiển thị tên khoa/phòng nhưng các control đó đã được điều khiển trạng thái Visibility rồi và Vm được dùng chung nhiều nên không dám chỉnh sửa, chỉ tách ngày giờ xuất khoa ra rồi điều khiển riêng cho dễ
        public void setTittle()
        {
            if (IsOpenToDischarge)
            {
                Tittle = string.Format("{0} - ", eHCMSResources.Z0252_G1_TTinXuatKhoa) + modInPtDeptDetails.DeptLocation.RefDepartment.DeptName.ToUpper();
            }
            else
            {
                Tittle = eHCMSResources.Z0253_G1_CTietNXKhPhg;
            }
        }

        // Hpt
        private string _tittle = eHCMSResources.Z0253_G1_CTietNXKhPhg;
        public string Tittle
        {
            get
            {
                return _tittle;
            }
            set
            {
                if (_tittle != value)
                {
                    _tittle = value;
                    NotifyOfPropertyChange(() => Tittle);
                }
            }
        }
        
        private int _dlgUsageMode = 0;
        public int DlgUsageMode 
        {
            get { return _dlgUsageMode; }
            set
            {
                _dlgUsageMode = value;
                if (DlgUsageMode == 1)  // Editing InPatientDeptDetail 
                {
                    ShowDischargeDateTime = true;
                }
            }
        }

        //modInPtDeptDetails lưu thông tin chi tiết nhập khoa trước khi chỉnh sửa
        private InPatientDeptDetail _modInPtDeptDetails;
        public InPatientDeptDetail modInPtDeptDetails 
        {
            get { return _modInPtDeptDetails; }
            set
            {
                _modInPtDeptDetails = value;
            }
        }

        public void SetModInPtDeptDetails(InPatientDeptDetail modInPtDeptDets)
        {
            if (modInPtDeptDets == null)
            {
                return;
            }
            modInPtDeptDetails = new InPatientDeptDetail();
            modInPtDeptDetails.DeptLocation = modInPtDeptDets.DeptLocation;
            modInPtDeptDetails.DeptLocID = modInPtDeptDets.DeptLocID;
            modInPtDeptDetails.FromDate = modInPtDeptDets.FromDate;
            modInPtDeptDetails.ToDate = modInPtDeptDets.ToDate;
            modInPtDeptDetails.InPatientDeptDetailID = modInPtDeptDets.InPatientDeptDetailID;
            modInPtDeptDetails.V_InPatientDeptStatus = modInPtDeptDets.V_InPatientDeptStatus;
            modInPtDeptDetails.IsAdmittedRecord = modInPtDeptDets.IsAdmittedRecord;

            ShowBedAllocationCmd = false;
            EnableRecvDeptCtrl = false;
            if (DlgUsageMode != 1)  // double check just in case DlgUsageMode has to be == 1 for editting
            {
                DlgUsageMode = 1; // Modify InPatientDeptDetail
            }

            DepartmentContent.SetSelDeptExt = true;
            DepartmentContent.SelectedItem = modInPtDeptDetails.DeptLocation.RefDepartment;

            SetSelectedLocationExt = true;
            SelectedLocation = modInPtDeptDetails.DeptLocation;

            AdmissionDateTime.DateTime = modInPtDeptDets.FromDate;
            //dtInDeptFromDate = modInPtDeptDets.FromDate;
            if (modInPtDeptDets.ToDate.HasValue)
            {
                //dtOutDeptToDate = modInPtDeptDets.ToDate.Value;
                DischargeDateTime.DateTime = modInPtDeptDets.ToDate;
            }
            else
            {
                ShowDischargeDateTime = false;
            }          
        }

        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);
            Globals.EventAggregator.Unsubscribe(this);
        }

        private bool _isFromRequestPaper = true;
        public bool IsFromRequestPaper
        {
            get { return _isFromRequestPaper; }
            set
            {
                _isFromRequestPaper = value;
                NotifyOfPropertyChange(() => IsFromRequestPaper);
            }
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

        //private ObservableCollection<long> _LstRefDepartment;
        //public ObservableCollection<long> LstRefDepartment
        //{
        //    get
        //    {
        //        return _LstRefDepartment;
        //    }
        //    set
        //    {
        //        _LstRefDepartment = value;

        //        DepartmentContent.LstRefDepartment = LstRefDepartment;
        //        DepartmentContent.AddSelectOneItem = true;

        //        if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(new DependencyObject()))
        //        {
        //            DepartmentContent.LoadData();
        //        }

        //        NotifyOfPropertyChange(() => LstRefDepartment);
        //    }
        //}

        private PatientRegistration _currentRegistration;
        public PatientRegistration CurrentRegistration
        {
            get
            {
                return _currentRegistration;
            }
            set
            {
                _currentRegistration = value;
                NotifyOfPropertyChange(() => CurrentRegistration);
            }
        }

        private InPatientTransferDeptReq _curInPatientTransferDeptReq;
        public InPatientTransferDeptReq curInPatientTransferDeptReq
        {
            get { return _curInPatientTransferDeptReq; }
            set
            {
                _curInPatientTransferDeptReq = value;                
                NotifyOfPropertyChange(() => curInPatientTransferDeptReq);
            }
        }

        private ObservableCollection<InPatientTransferDeptReq> _allInPatientTransferDeptReq;
        public ObservableCollection<InPatientTransferDeptReq> allInPatientTransferDeptReq
        {
            get { return _allInPatientTransferDeptReq; }
            set
            {
                _allInPatientTransferDeptReq = value;
                NotifyOfPropertyChange(() => allInPatientTransferDeptReq);
            }
        }

        private InPatientAdmDisDetails _currentAdmission;
        public InPatientAdmDisDetails CurrentAdmission
        {
            get { return _currentAdmission; }
            set
            {
                _currentAdmission = value;
                NotifyOfPropertyChange(() => CurrentAdmission);
            }
        }

        //public void Handle(ItemSelected<IcwdBedPatientCommon, BedPatientAllocs> message)
        //{
        //    if (GetView() != null && message != null)
        //    {
        //        if (CurrentAdmission != null && CurrentAdmission.PatientRegistration != null)
        //        {
        //            if (CurrentAdmission.PatientRegistration.BedAllocations == null)
        //            {
        //                CurrentAdmission.PatientRegistration.BedAllocations = new BindableCollection<BedPatientAllocs>();
        //            }
        //            //Tam thoi lam the nay. Ben Dinh dua qua khong dung.
        //            message.Item.BedPatientID = 0;
        //            CurrentAdmission.PatientRegistration.BedAllocations.Add(message.Item);
        //        }
        //    }
        //}

        public void Handle(BookingBedForAcceptChangeDeptEvent message)
        {
            if (message == null || CurInPatientTransferDeptReq == null)
            {
                return;
            }

            if (CurInPatientTransferDeptReq.BedAllocation == null)
            {
                CurInPatientTransferDeptReq.BedAllocation = new BedPatientAllocs();
            }

            CurInPatientTransferDeptReq.BedAllocation = message.SelectedBedPatientAlloc;
        }

        private ObservableCollection<DeptLocation> _locations;
        public ObservableCollection<DeptLocation> Locations
        {
            get
            {
                return _locations;
            }
            set
            {
                _locations = value;
                NotifyOfPropertyChange(() => Locations);
            }
        }

        private bool SetSelectedLocationExt = false;

        private DeptLocation _selectedLocation;
        public DeptLocation SelectedLocation
        {
            get
            {
                return _selectedLocation;
            }
            set
            {
                _selectedLocation = value;
                NotifyOfPropertyChange(() => SelectedLocation);
            }
        }

        private bool _showDischargeDateTime = false;
        public bool ShowDischargeDateTime
        {
            get { return _showDischargeDateTime; }
            set
            {
                _showDischargeDateTime = value;
                NotifyOfPropertyChange(() => ShowDischargeDateTime);
            }
        }

        private bool _showBedAllocationCmd = true;
        public bool ShowBedAllocationCmd
        {
            get { return _showBedAllocationCmd; }
            set
            {
                _showBedAllocationCmd = value;
                NotifyOfPropertyChange(() => ShowBedAllocationCmd);
            }
        }

        private bool _enableRecvDeptCtrl = true;
        public bool EnableRecvDeptCtrl
        {
            get { return _enableRecvDeptCtrl; }
            set
            {
                _enableRecvDeptCtrl = value;
                NotifyOfPropertyChange(() => EnableRecvDeptCtrl);
            }
        }

        public InPatientTransferDeptReq CurInPatientTransferDeptReq
        {
            get { return _curInPatientTransferDeptReq; }
            set
            {
                _curInPatientTransferDeptReq = value;
                if (_curInPatientTransferDeptReq.ReqDeptLoc != null && _curInPatientTransferDeptReq.ReqDeptLoc.RefDepartment != null)
                {
                    IsTreatmentCOVID = _curInPatientTransferDeptReq.ReqDeptLoc.RefDepartment.IsTreatmentForCOVID;
                }
                GetListMedServiceInSurgeryDept(_curInPatientTransferDeptReq.PtRegistrationID);
                NotifyOfPropertyChange(() => CurInPatientTransferDeptReq);
            }
        }

        private IEnumerator<IResult> DoLoadLocations(long deptId)
        {
            var deptLoc = new LoadDeptLoctionByIDTask(deptId);
            yield return deptLoc;
            if (deptLoc.DeptLocations != null)
            {
                Locations = new ObservableCollection<DeptLocation>(deptLoc.DeptLocations);
            }
            else
            {
                Locations = new ObservableCollection<DeptLocation>();
            }

            var itemDefault = new DeptLocation();
            itemDefault.Location = new Location();
            itemDefault.Location.LID = -1;
            itemDefault.Location.LocationName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.Z0116_G1_HayChonPg);
            Locations.Insert(0, itemDefault);
            SelectedLocation = itemDefault;
            yield break;
        }

        public void LoadLocations(long? deptId)
        {
            this.ShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
            var list = new List<refModule>();

            var t = new Thread(() =>
            {
                using (var serviceFactory = new ConfigurationManagerServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetAllLocationsByDeptIDOld(deptId, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var allItems = contract.EndGetAllLocationsByDeptIDOld(asyncResult);

                            if (allItems != null)
                            {
                                Locations = new ObservableCollection<DeptLocation>(allItems);
                            }
                            else
                            {
                                Locations = new ObservableCollection<DeptLocation>();
                            }

                            var itemDefault = new DeptLocation();
                            itemDefault.DeptID = -1;
                            itemDefault.Location = new Location();
                            itemDefault.Location.LID = -1;
                            itemDefault.Location.LocationName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.Z0116_G1_HayChonPg);
                            Locations.Insert(0, itemDefault);

                            if (ProgramSetDeptLocation != null)
                            {
                                SelectedLocation = Locations.FirstOrDefault(x => x.DeptLocationID == ProgramSetDeptLocation.DeptLocationID);
                                ProgramSetDeptLocation = null;
                            }
                            else if (!SetSelectedLocationExt)
                            {
                                SelectedLocation = itemDefault;
                            }
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            //Globals.IsBusy = false;
                            this.HideBusyIndicator();
                        }
                    }), null);
                }
            });

            t.Start();
        }

        void DepartmentContent_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "SelectedItem")
            {
                return;
            }
            if (DepartmentContent.SelectedItem.DeptID > 0)
            {
                LoadLocations(DepartmentContent.SelectedItem.DeptID);
            }
            else
            {
                Locations = new ObservableCollection<DeptLocation>();
                var itemDefault = new DeptLocation();
                itemDefault.DeptID = -1;
                itemDefault.Location = new Location();
                itemDefault.Location.LID = -1;
                itemDefault.Location.LocationName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.Z0116_G1_HayChonPg);
                Locations.Insert(0, itemDefault);

                SelectedLocation = itemDefault;
            }    
        }

        public void cboLocation_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CurInPatientTransferDeptReq == null || CurInPatientTransferDeptReq.BedAllocation == null)
            {
                return;   
            }

            CurInPatientTransferDeptReq.BedAllocation = new BedPatientAllocs();
        }

        public void LoadData()
        {
            if (IsTemp)
                DepartmentContent.IsShowOnlyAllowableInTemp = true;
            DepartmentContent.LstRefDepartment = Globals.LoggedUserAccount.DeptIDResponsibilityList;
            if (ProgramSetDeptLocation == null)
            {
                DepartmentContent.AddSelectOneItem = true;
            }
            if (!DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                DepartmentContent.LoadData(ProgramSetDeptLocation == null ? 0 : ProgramSetDeptLocation.DeptID);
            }
            // Hpt: hàm setTittle sẽ hiển thị thêm thông tin khoa phòng trong tiêu để ViewModel vì khi xuất khoa không có các control hiển thị thông tin nào khác ngoài DatePicker chỉnh thời gian xuất khoa, người dùng khó theo dõi mình đang xuất khoa nào
            setTittle();
            // Hpt: Mặc định thời gian hiển thị trong datePicker khi vừa khởi động VM lên là ngày hiện tại 
            // TNHX: bỏ xài cái này chuyển qua Content mới AdmissionDateTime + DischargeDateTime
            if (DischargeDateTime == null && IsOpenToDischarge)
            {
                DischargeDateTime = Globals.GetViewModel<IMinHourDateControl>();
                DischargeDateTime.DateTime = Globals.GetCurServerDateTime();
            }
        }

        private bool _IsTemp;
        public bool IsTemp
        {
            get
            {
                return _IsTemp;
            }
            set
            {
                _IsTemp = value;
                NotifyOfPropertyChange(() => IsTemp);
            }
        }

        private IMinHourDateControl _AdmissionDateTime;
        public IMinHourDateControl AdmissionDateTime
        {
            get { return _AdmissionDateTime; }
            set
            {
                _AdmissionDateTime = value;
                NotifyOfPropertyChange(() => AdmissionDateTime);
            }
        }

        private IMinHourDateControl _DischargeDateTime;
        public IMinHourDateControl DischargeDateTime
        {
            get { return _DischargeDateTime; }
            set
            {
                _DischargeDateTime = value;
                NotifyOfPropertyChange(() => DischargeDateTime);
            }
        }

        #region COMMANDS

        public void ModInPtAdmDiscDeptDetails()
        {
            
            InPatientDeptDetail item = new InPatientDeptDetail();
            
            item.DeptLocID = SelectedLocation.DeptLocationID;
            item.InPatientDeptDetailID = modInPtDeptDetails.InPatientDeptDetailID;
            item.V_InPatientDeptStatus = modInPtDeptDetails.V_InPatientDeptStatus;
            
            //item.FromDate = dtInDeptFromDate;
            item.FromDate = AdmissionDateTime.DateTime.GetValueOrDefault(DateTime.MinValue);

            if (DischargeDateTime != null && ShowDischargeDateTime)
            {
                item.ToDate = DischargeDateTime.DateTime.GetValueOrDefault(DateTime.MinValue);
            }
            else
            {
                item.ToDate = null;
            }
            if (modInPtDeptDetails.IsAdmittedRecord== true)
            {
                if (modInPtDeptDetails.FromDate != item.FromDate || modInPtDeptDetails.DeptLocID != item.DeptLocID)
                {
                    MessageBox.Show(eHCMSResources.A1040_G1_Msg_InfoLienQuanCTietNpVien + string.Format("\n{0}", eHCMSResources.A1041_G1_Msg_InfoChiCNhatNgXKhoa));
                    //dtInDeptFromDate = modInPtDeptDetails.FromDate;
                    AdmissionDateTime.DateTime = modInPtDeptDetails.FromDate;
                    SelectedLocation = modInPtDeptDetails.DeptLocation;
                    return;
                }
                else
                {
                    DoModInPtAdmDiscDeptDetails(item);
                }
            }
            else
            {
                DoModInPtAdmDiscDeptDetails(item);
            }
        }

        public void btnSave()
        {
            if (Globals.IsLockRegistration(CurrentRegistration.RegLockFlag, eHCMSResources.Z0104_G1_LuuTTinNhapXuatKhoa))
            {
                return;
            }
            if (IsOpenToDischarge)
            {
                OutDepartment();
            }
            else
            {
                Coroutine.BeginExecute(AcceptChangeDeptCmd());                
            }
        }       

        WarningWithConfirmMsgBoxTask warnConfDlg = null;

        //HPT 28/07/2016: Sửa thành như vầy để show cảnh báo xác nhận
        //public void AcceptChangeDeptCmd()
        public IEnumerator<IResult> AcceptChangeDeptCmd()
        {            
            if (SelectedLocation == null || SelectedLocation.DeptLocationID <= 0)
            {
                MessageBox.Show(eHCMSResources.K0384_G1_ChonPg);
                yield break;
            }

            if (CurrentRegistration == null || CurrentRegistration.AdmissionInfo == null)
            {
                MessageBox.Show(eHCMSResources.A0636_G1_Msg_InfoKhCoDK_TTinNpVien, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                yield break;
            }

            //if (dtInDeptFromDate.Date > Globals.GetCurServerDateTime().Date)
            if (AdmissionDateTime.DateTime.GetValueOrDefault().Date > Globals.GetCurServerDateTime().Date)
            {
                MessageBox.Show(eHCMSResources.A0847_G1_Msg_InfoNgNpKhoaKhHopLe, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                AdmissionDateTime.DateTime = Globals.GetCurServerDateTime();
                yield break;
            }

            //if (dtInDeptFromDate.Date < CurrentRegistration.AdmissionInfo.AdmissionDate.GetValueOrDefault().Date)
            if (AdmissionDateTime.DateTime.GetValueOrDefault().Date < CurrentRegistration.AdmissionInfo.AdmissionDate.GetValueOrDefault().Date)
            {
                MessageBox.Show(eHCMSResources.A0849_G1_Msg_InfoNgNpKhoaKhHopLe3 + string.Format(". {0}: {1}", eHCMSResources.N0096_G1_NgNhapVien, CurrentRegistration.AdmissionInfo.AdmissionDate.GetValueOrDefault(DateTime.MinValue).ToString("dd/MM/yyyy hh:mm:ss tt")), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                yield break;
            }
            //if (dtOutDeptToDate != null && dtOutDeptToDate < modInPtDeptDetails.FromDate)
            if (DischargeDateTime != null && modInPtDeptDetails != null && DischargeDateTime.DateTime < modInPtDeptDetails.FromDate)
            {
                MessageBox.Show(eHCMSResources.A0848_G1_Msg_InfoNgNpKhoaKhHopLe2 + string.Format("\n {0} {1}", eHCMSResources.A0834_G1_Msg_NgNpKhoa, DischargeDateTime.DateTime.GetValueOrDefault(DateTime.MinValue).ToString("dd/MM/yyyy hh:mm:ss tt")), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                yield break;
            }

            // TxD 13/01/2015: ModInPtAdmDiscDeptDetails should be called before the following checking because that checking will stop it 
            //                  from modifying the InTime while patient has not got out of the current dept line ie. InPatientDeptDetail record
            if (DlgUsageMode == 1)
            {
                ModInPtAdmDiscDeptDetails();
                yield break;
            }

            //KMx: Không hạn chế nhập vào cùng khoa (nếu không thì không đổi phòng được) (07/09/2014 15:26).
            //if (CurrentRegistration != null && CurrentRegistration.AdmissionInfo != null && CurrentRegistration.AdmissionInfo.InPatientDeptDetails.Count > 0)
            if (CurrentRegistration.AdmissionInfo.InPatientDeptDetails.Count > 0)
            {
                //var result = CurrentRegistration.AdmissionInfo.InPatientDeptDetails.Where(x => x.V_InPatientDeptStatus == AllLookupValues.InPatientDeptStatus.NHAP_KHOA
                //                                                                                && x.DeptLocation.DeptID == SelectedLocation.DeptID);
                var result = CurrentRegistration.AdmissionInfo.InPatientDeptDetails.Where(x => x.V_InPatientDeptStatus == AllLookupValues.InPatientDeptStatus.NHAP_KHOA_PHONG
                    && !x.IsTemp && x.DeptLocID == SelectedLocation.DeptLocationID);

                if (result != null && result.Count() > 0
                    && CurrentRegistration.V_RegForPatientOfType != AllLookupValues.V_RegForPatientOfType.DKBN_DT_NGOAI_TRU)
                {
                    MessageBox.Show(string.Format("{0} ", eHCMSResources.A0230_G1_Msg_InfoBNDangDTriTrong) + SelectedLocation.RefDepartment.DeptName + ", " + SelectedLocation.Location.LocationName + string.Format(". {0}!", eHCMSResources.A0231_G1_Msg_InfoKhTheNpKhoaPg), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                    yield break;
                }
                if (!IsTemp)
                {
                    // HPT 03/04/2017: Thêm điều kiện IsTemp = false để bỏ qua các dòng nhập mang tính chất tạm thời
                    InPatientDeptDetail PreviousInPtDept = CurrentRegistration.AdmissionInfo.InPatientDeptDetails.Where(x=>x.IsTemp == false).OrderByDescending(x => x.InPatientDeptDetailID).FirstOrDefault(x => x.DeptLocation.DeptID != SelectedLocation.DeptID);
                    if (PreviousInPtDept != null)
                    {
                        long PreviousDeptID = PreviousInPtDept.DeptLocation.DeptID;
                        if (!IsFromRequestPaper && Globals.allDeptTransDocTypeReq.Any(x => x.FromDeptID == PreviousDeptID && x.ToDeptID == SelectedLocation.DeptID
                                && x.DocTypeRequired == (long)AllLookupValues.V_DocTypeRequired.DN_CHUYEN_KHOA))
                        {
                            MessageBox.Show("Quá trình chuyển đến khoa bạn chọn được yêu cầu đề nghị chuyển khoa! Vui lòng nhập khoa từ đề nghị chuyển khoa!");
                            yield break;
                        }
                        bool HasDiagnosis = PreviousInPtDept.CompletedRequiredFromDate != null;
                        if (Globals.allDeptTransDocTypeReq.Any(x => x.FromDeptID == PreviousDeptID && x.ToDeptID == SelectedLocation.DeptID
                                && x.DocTypeRequired == (long)AllLookupValues.V_DocTypeRequired.CD_XUAT_KHOA && !HasDiagnosis))
                        {
                            string FromDeptName = Globals.AllRefDepartmentList.FirstOrDefault(x => x.DeptID == PreviousDeptID).DeptName;
                            string ToDeptName = Globals.AllRefDepartmentList.FirstOrDefault(x => x.DeptID == SelectedLocation.DeptID).DeptName;
                            warnConfDlg = new WarningWithConfirmMsgBoxTask(string.Format("{0} {1} {2} {3}.\n{4}{5}!!!"
                                , eHCMSResources.Z0106_G1_BNKhiChuyenDen, ToDeptName, eHCMSResources.Z0107_G1_DuocYCCDoanXuat, FromDeptName, FromDeptName, eHCMSResources.Z0108_G1_ChuaCoCDoanXKhoa)
                                , eHCMSResources.Z0251_G1_XNhanTieptucNhapKhPhg);
                            yield return warnConfDlg;
                            if (!warnConfDlg.IsAccept)
                            {
                                yield break;
                            }
                        }
                    }
                }
            }

            if (CurInPatientTransferDeptReq.BedAllocation != null 
                && CurInPatientTransferDeptReq.BedAllocation.CheckInDate.GetValueOrDefault().Date < AdmissionDateTime.DateTime.GetValueOrDefault().Date)
            {
                MessageBox.Show(eHCMSResources.A0833_G1_Msg_InfoNgDatGiuong, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                yield break;
            }

            InPatientDeptDetail item = new InPatientDeptDetail();
            item.IsTemp = IsTemp;
            item.DeptLocID = SelectedLocation.DeptLocationID;
            item.InPatientAdmDisDetailID = CurInPatientTransferDeptReq.InPatientAdmDisDetailID;
            item.BedAllocation = CurInPatientTransferDeptReq.BedAllocation; 
            //item.FromDate = dtInDeptFromDate;
            item.FromDate = AdmissionDateTime.DateTime.GetValueOrDefault(DateTime.MinValue);
            item.V_InPatientDeptStatus = AllLookupValues.InPatientDeptStatus.NHAP_KHOA_PHONG;
            item.IsTreatmentCOVID = IsTreatmentCOVID;
            InPatientDeptDetailsTranfer(item, CurInPatientTransferDeptReq.InPatientTransferDeptReqID, CurrentRegistration != null && CurrentRegistration.V_RegForPatientOfType==AllLookupValues.V_RegForPatientOfType.DKBN_DT_NGOAI_TRU);
        }

        // Hpt 30/10/2015: Dời hàm OutDepartment từ InPatientDeptListing qua đây. Vì anh Tuấn yêu cầu khi click Xuất khoa phòng phải hiển thị Popup cho người dùng chọn giờ xuất khoa.
        public void OutDepartment()
        {
            if (DischargeDateTime == null)
            {
                MessageBox.Show(eHCMSResources.K0374_G1_ChonNgXV, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
            this.ShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
            
            InPatientDeptDetail item = new InPatientDeptDetail();
            item.InPatientDeptDetailID = modInPtDeptDetails.InPatientDeptDetailID;
            item.V_InPatientDeptStatus = modInPtDeptDetails.V_InPatientDeptStatus;
            item.FromDate = modInPtDeptDetails.FromDate;
            item.ToDate = DischargeDateTime.DateTime;
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginOutDepartment(item,
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                try
                                {
                                    InPatientDeptDetail InPtDeptDetail = null;
                                    var res = contract.EndOutDepartment(out InPtDeptDetail, asyncResult);
                                    if (res)
                                    {
                                        Globals.EventAggregator.Publish(new OutDepartmentSuccessEvent());
                                        MessageBox.Show(eHCMSResources.K0554_G1_XuatKhoiKhoaPhOk);
                                        TryClose();
                                    }
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(ex.Message, string.Format("{0}!",eHCMSResources.T0074_G1_I), MessageBoxButton.OK);
                                    ClientLoggerHelper.LogInfo(ex.Message);
                                }
                                finally
                                {
                                    this.HideBusyIndicator();
                                }
                            }), null);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, string.Format("{0}!", eHCMSResources.T0074_G1_I), MessageBoxButton.OK);
                    this.HideBusyIndicator();
                }
            });

            t.Start();
        }

        public void GetInPatientAdmissionByRegistrationID(long PtRegistrationID)
        {
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetInPatientAdmissionByRegistrationID(PtRegistrationID, Globals.DispatchCallback(asyncResult =>
                        {
                            try
                            {
                                var results = contract.EndGetInPatientAdmissionByRegistrationID(asyncResult);
                                if (CurrentRegistration != null)
                                {
                                    CurrentRegistration.AdmissionInfo = results;
                                }
                            }
                            catch (FaultException<AxException> fault)
                            {
                                ClientLoggerHelper.LogInfo(fault.ToString());
                                MessageBox.Show(eHCMSResources.A0432_G1_Msg_InfoChKhoaFail);
                            }
                            catch (Exception ex)
                            {
                                ClientLoggerHelper.LogInfo(ex.ToString());
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                }
            });

            t.Start();
        }

        public void DoModInPtAdmDiscDeptDetails(InPatientDeptDetail p)
        {
            var t = new Thread(() =>
            {                
                try
                {
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginUpdateDeleteInPatientDeptDetails(null, p, Globals.DispatchCallback(asyncResult =>
                        {
                            try
                            {
                                InPatientDeptDetail outInPtDeptDet = null;
                                bool bOK = contract.EndUpdateDeleteInPatientDeptDetails(out outInPtDeptDet, asyncResult);
                                if (bOK)
                                {
                                    MessageBox.Show(eHCMSResources.A0279_G1_Msg_InfoCNhatOK);
                                    //phat su kien de from o duoi bat
                                    // AcceptChangeDeptViewModelEvent
                                    Globals.EventAggregator.Publish(new AcceptChangeDeptViewModelEvent { });
                                    //load lai thong tin dang ky
                                    if (CurrentRegistration != null && CurrentRegistration.PtRegistrationID > 0)
                                    {
                                        GetInPatientAdmissionByRegistrationID(CurrentRegistration.PtRegistrationID);
                                    }
                                    TryClose();
                                }
                                else
                                {
                                    MessageBox.Show(eHCMSResources.A0272_G1_Msg_InfoCNhatFail);
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                                ClientLoggerHelper.LogInfo(ex.Message);
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                }
            });

            t.Start();
        }

        public void InPatientDeptDetailsTranfer(InPatientDeptDetail p, long InPatientTransferDeptReqID, bool IsAutoCreateOutDeptDiagnosis)
        {
            this.DlgShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
            var t = new Thread(() =>
            {
                long patientDeptDetailId;
                try
                {
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginInPatientDeptDetailsTranfer(p, InPatientTransferDeptReqID, IsAutoCreateOutDeptDiagnosis, Globals.DispatchCallback(asyncResult =>
                        {
                            try
                            {
                                bool bOK = contract.EndInPatientDeptDetailsTranfer(out patientDeptDetailId, asyncResult);
                                if (bOK)
                                {
                                    MessageBox.Show(eHCMSResources.A0870_G1_Msg_InfoNpKhoaOK);
                                    //phat su kien de from o duoi bat
                                   // AcceptChangeDeptViewModelEvent
                                    Globals.EventAggregator.Publish(new AcceptChangeDeptViewModelEvent { });
                                    //load lai thong tin dang ky
                                    if (CurrentRegistration != null && CurrentRegistration.PtRegistrationID > 0)
                                    {
                                        GetInPatientAdmissionByRegistrationID(CurrentRegistration.PtRegistrationID);
                                    }
                                    TryClose();
                                }
                                else
                                {
                                    MessageBox.Show(eHCMSResources.A0432_G1_Msg_InfoChKhoaFail);
                                }
                            }
                            //catch (FaultException<AxException> fault)
                            //{
                            //    ClientLoggerHelper.LogInfo(fault.ToString());
                            //    MessageBox.Show("Chuyển Khoa Thất Bại.");
                            //}
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                                ClientLoggerHelper.LogInfo(ex.Message);
                            }
                            finally
                            {
                                this.DlgHideBusyIndicator();
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                }
                finally
                {
                    this.DlgHideBusyIndicator();
                }
            });

            t.Start();
        }

        public void lnkSelectBedAllocationCmd_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e == null || e.ClickCount != 1)
            {
                return;
            }

            if (CurrentRegistration == null)
            {
                MessageBox.Show(eHCMSResources.A0381_G1_Msg_InfoChuaChonDK_KhTheDatGiuong);
                return;
            }
            if (SelectedLocation == null || SelectedLocation.DeptLocationID < 1)
            {
                MessageBox.Show(eHCMSResources.A0392_G1_Msg_InfoChuaChonPgDatGiuong);
                return;
            }

            Action<IBedPatientAlloc> onInitDlg = delegate (IBedPatientAlloc bedAllocVm)
            {
                bedAllocVm.curPatientRegistration = CurrentRegistration;

                InPatientDeptDetail InPtDeptDetail = new InPatientDeptDetail();
                InPtDeptDetail.FromDate = AdmissionDateTime.DateTime.GetValueOrDefault(DateTime.MinValue);

                bedAllocVm.InPtDeptDetail = InPtDeptDetail;

                //KMx: Đổi biến BookBedAllocOnly thành eFireBookingBedEventTo để bắn event (06/09/2014 17:20).
                //bedAllocVm.BookBedAllocOnly = false;//Dat giuong truc tiep luon.   
                bedAllocVm.eFireBookingBedEventTo = eFireBookingBedEvent.AcceptChangeDeptView;

                bedAllocVm.isLoadAllDept = false;
                //KMx: Sửa lại để cho "Nhập khoa không cần phiếu yêu cầu" cũng có thể đặt giường luôn (12/07/2014 11:24).
                //bedAllocVm.DefaultDepartment = CurInPatientTransferDeptReq.ReqDeptLoc.RefDepartment;
                //bedAllocVm.ResponsibleDepartment = CurInPatientTransferDeptReq.ReqDeptLoc.RefDepartment;
                bedAllocVm.DefaultDepartment = SelectedLocation.RefDepartment;
                bedAllocVm.ResponsibleDepartment = SelectedLocation.RefDepartment;
                bedAllocVm.SelectedDeptLocation = SelectedLocation;
            };
            GlobalsNAV.ShowDialog<IBedPatientAlloc>(onInitDlg);
        }

        public void Handle(AddCompleted<BedPatientAllocs> message)
        {
            if (GetView() != null && message.Item != null) 
            {
                AcceptChangeDeptCmd();
            }
        }

        public void CancelCmd()
        {
            TryClose();
            Globals.EventAggregator.Unsubscribe(this);
        }

        private DeptLocation ProgramSetDeptLocation = null;
        public void SetDesDepartment(DeptLocation aDeptLocation)
        {
            if (aDeptLocation == null || aDeptLocation.DeptID == 0)
            {
                return;
            }
            ProgramSetDeptLocation = aDeptLocation;
            //DepartmentContent.SetSelectedDeptItem(aDeptLocation.DeptID);
        }

        public void GetListMedServiceInSurgeryDept(long PtRegistrationID)
        {
            this.DlgShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginGetListMedServiceInSurgeryDept(PtRegistrationID,
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                allInPatientTransferDeptReq = new ObservableCollection<InPatientTransferDeptReq>();
                                try
                                {
                                    var res = contract.EndGetListMedServiceInSurgeryDept(asyncResult);
                                    ListMedServiceInSurgeryDept = res.ToObservableCollection();
                                }
                                catch (FaultException<AxException> fault)
                                {
                                    ClientLoggerHelper.LogInfo(fault.ToString());
                                }
                                finally
                                {
                                    this.DlgHideBusyIndicator();
                                }
                            }), null);
                    }
                }
                catch (Exception ex)
                {
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    this.DlgHideBusyIndicator();
                }
            });

            t.Start();
        }

        private ObservableCollection<PatientRegistrationDetail> _ListMedServiceInSurgeryDept;
        public ObservableCollection<PatientRegistrationDetail> ListMedServiceInSurgeryDept
        {
            get { return _ListMedServiceInSurgeryDept; }
            set
            {
                _ListMedServiceInSurgeryDept = value;
                NotifyOfPropertyChange(() => ListMedServiceInSurgeryDept);
            }
        }
        #endregion
    }
}