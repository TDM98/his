using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.ServiceModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using aEMR.CommonTasks;
using aEMR.DataContracts;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using aEMR.ViewContracts.Configuration;
using Caliburn.Micro;
using DataEntities;
using aEMR.Common;
using eHCMSLanguage;
using Castle.Windsor;
using aEMR.Common.ViewModels;
using aEMR.Common.Collections;
using aEMR.ViewContracts.Consultation_ePrescription;
/*
* 20171220 #001 CMN:    Added configurations for change dept request function.
* 20181119 #002 TTM:    BM 0005257: Tạo out standing task tìm kiếm bệnh nhân nằm tại khoa và sự kiện chụp lại khi chọn bệnh nhân từ Out standing task.
* 20191002 #003 TTM:    BM 0017405: [Đề nghị nhập viện] Mặc định khoa nhập viện từ đề nghị nhập viện 
* 20191104 #004 TTM:    BM 0018528: [Left Menu] Bổ sung cây y lệnh chẩn đoán vào màn hình quản lý bệnh nhân nội trú
* 20191119 #005 TTM:    BM 0019591: Thêm 1 danh sách BN chờ nhập viện bên OutstandingTask.
* 20200615 #006 TBL:    BM 0038208: Sáp nhập ca khám ngoại trú vào nội trú.
*/
namespace aEMR.Registration.ViewModels
{
    [Export(typeof(IInPatientAdmission)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class InPatientAdmissionViewModel : Conductor<object>, IInPatientAdmission
        , IHandle<ItemSelected<Patient>>
        , IHandle<ResultFound<Patient>>
        , IHandle<ResultNotFound<Patient>>
        , IHandle<ItemSelected<PatientRegistration>>
        , IHandle<UpdateCompleted<InPatientAdmDisDetails>>
        , IHandle<AddCompleted<BedPatientAllocs>>
        , IHandle<AddCompleted<InPatientDeptDetail>>
        , IHandle<RemoveItem<BedPatientAllocs, object>>
        , IHandle<ReturnItem<BedPatientAllocs, object>>
        , IHandle<ReturnItem<InPatientTransferDeptReq, object>>
        , IHandle<AcceptChangeDeptViewModelEvent>
        , IHandle<OutDepartmentSuccessEvent>
        , IHandle<InPatientRegistrationSelectedForInPatientAdmission>
        , IHandle<CallBeginNewOutPtTreatment>
        , IHandle<InPatientSelectedForInPatientAdmission>
    {

        private string _TitleForm;
        public string TitleForm
        {
            get { return _TitleForm; }
            set
            {
                _TitleForm = value;
                NotifyOfPropertyChange(() => TitleForm);
            }
        }

        private string _DeptLocTitle;
        public string DeptLocTitle
        {
            get
            {
                return _DeptLocTitle;
            }
            set
            {
                _DeptLocTitle = value;
                NotifyOfPropertyChange(() => DeptLocTitle);
            }
        }

        private bool _IsProcessing;
        public bool IsProcessing
        {
            get
            {
                return _IsProcessing;
            }
            set
            {
                _IsProcessing = value;
                NotifyOfPropertyChange(() => IsProcessing);
            }
        }

        private bool _isAdmision = true;
        public bool isAdmision
        {
            get { return _isAdmision; }
            set
            {
                _isAdmision = value;

                if (_isAdmision)    // ViewModel used for Nhap Vien
                {
                    SearchRegistrationContent.SearchAdmittedInPtRegOnly = false;
                }
                else                // ViewModel used for Quan Ly BN Noi Tru
                {
                    SearchRegistrationContent.SearchAdmittedInPtRegOnly = true;
                }

                mNhapVien_TimDeNghiChuyenKhoa = mNhapVien_TimDeNghiChuyenKhoa && !isAdmision;
                ShowAdmissionCmd = ShowAdmissionCmd && isAdmision;
                NotifyOfPropertyChange(() => isAdmision);
                NotifyOfPropertyChange(() => mNhapVien_TimDeNghiChuyenKhoa);
                NotifyOfPropertyChange(() => ShowAdmissionCmd);
            }
        }

        private AllLookupValues.PatientFindBy _PatientFindBy;
        public AllLookupValues.PatientFindBy PatientFindBy
        {
            get
            {
                return _PatientFindBy;
            }
            set
            {
                _PatientFindBy = value;
                NotifyOfPropertyChange(() => PatientFindBy);
                // Hpt 27/11/2015: Đã gán giá trị trong hàm khởi tạo rồi nhưng không có thời gian xem lại nên cứ để thêm một lần nữa ở đây, có thời gian sẽ xem lại và điều chỉnh 
                if (SearchRegistrationContent != null)
                {
                    SearchRegistrationContent.PatientFindBy = PatientFindBy;
                }
            }
        }
        IEventAggregator _eventArg;
        [ImportingConstructor]
        public InPatientAdmissionViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg)
        {
            _eventArg = eventArg;
            var searchPatientAndRegVm = Globals.GetViewModel<ISearchPatientAndRegistration>();
            searchPatientAndRegVm.mTimBN = mNhapVien_Patient_TimBN;
            searchPatientAndRegVm.mThemBN = mNhapVien_Patient_ThemBN;
            searchPatientAndRegVm.mTimDangKy = mNhapVien_Patient_TimDangKy;

            searchPatientAndRegVm.SearchAdmittedInPtRegOnly = false;

            searchPatientAndRegVm.InitButtonVisibility(SearchRegButtonsVisibility.SHOW_SEARCH_REG_BTN);
            searchPatientAndRegVm.SetDefaultButton(SearchRegistrationButtons.SEARCH_REGISTRATION);
            searchPatientAndRegVm.PatientFindBy = AllLookupValues.PatientFindBy.NOITRU;

            searchPatientAndRegVm.CanSearhRegAllDept = true;

            SearchRegistrationContent = searchPatientAndRegVm;

            ActivateItem(searchPatientAndRegVm);

            var patientInfoVm = Globals.GetViewModel<IPatientSummaryInfoV2>();
            patientInfoVm.mInfo_CapNhatThongTinBN = mNhapVien_Info_CapNhatThongTinBN;
            patientInfoVm.mInfo_XacNhan = mNhapVien_Info_XacNhan;
            patientInfoVm.mInfo_XoaThe = mNhapVien_Info_XoaThe;
            patientInfoVm.mInfo_XemPhongKham = false;
            PatientSummaryInfoContent = patientInfoVm;
            PatientSummaryInfoContent.DisplayButtons = false;
            ActivateItem(patientInfoVm);
            InPatientTransferDeptReqsearch = new InPatientTransferDeptReq();

            authorization();

            // TxD 24/05/2015 moved the following block of code to InitViewContent method below
            //InPatientAdmissionInfoContent = Globals.GetViewModel<IInPatientAdmissionInfo>();
            //InPatientAdmissionInfoContent.isAdmision = isAdmision;
            ////InPatientAdmissionInfoContent.LstRefDepartment = LstRefDepartment;
            //InPatientAdmissionInfoContent.LoadData();

            InPatientDeptListingContent = Globals.GetViewModel<IInPatientDeptListing>();
            InPatientDeptListingContent.ShowBookingBedColumn = true;
            InPatientDeptListingContent.ShowOutDeptColumn = true;
            InPatientDeptListingContent.ShowDeleteEditColumn = true;

            //InPatientDeptListingContent.LstRefDepartment = LstRefDepartment;

            PatientAllocListingContent = Globals.GetViewModel<IInPatientBedPatientAllocListing>();
            PatientAllocListingContent.ShowDeleteColumn = true;
            PatientAllocListingContent.ShowReturnBedColumn = true;

            //PatientAllocListingContent.LstRefDepartment = LstRefDepartment;

            (PatientSummaryInfoContent as PropertyChangedBase).PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(InPatientAdmissionViewModel_PropertyChanged);
            //authorization();
        }
        public IDiagnosisTreatmentTree DiagnosisTreatmentTree { get; set; }
        public void InitViewContent()
        {
            InPatientAdmissionInfoContent = Globals.GetViewModel<IInPatientAdmissionInfo>();
            InPatientAdmissionInfoContent.isAdmision = isAdmision;
            InPatientAdmissionInfoContent.LoadData();
        }

        void InPatientAdmissionViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == eHCMSResources.Z0289_G1_CurrentPatientClassification)
            {
                if (CurrentRegistration != null)
                {
                    CurrentRegistration.PatientClassification = PatientSummaryInfoContent.CurrentPatientClassification;
                }
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
                _currentRegistration = value;
                NotifyOfPropertyChange(() => CurrentRegistration);
                NotifyOfPropertyChange(() => ShowAdmissionCmd);
                NotifyOfPropertyChange(() => ShowMergerCmd);
                NotifyOfPropertyChange(() => ShowChangeDeptCmd);
                NotifyOfPropertyChange(() => IsEnalbeInDeptChangeLocFucn);
                NotifyOfPropertyChange(() => IsEnalbeTempInDeptFuction);
                // Hpt 15/10/2015: Cu mac dinh gia tri isDischarged o day, o duoi neu co isDischarged co gia tri thi set lai sau
                isDischarged = false;
                // Hpt 15/10/2015: Khong biet logic o day ntn nhung neu co truong hop lam cho CurrentRegistration = null thi doan code ben duoi se bi loi nen viet them code kiem tra o day
                // Sau nay neu can thiet se xem xet lai
                if (CurrentRegistration != null)
                {
                    //KMx: Trong CurrentRegistration có chứa AdmissionInfo rồi, khi nào có thời gian thì xem xét và bỏ AdmissionInfo đi (11/09/2014 10:23).
                    InPatientDeptListingContent.CurrentRegistration = CurrentRegistration;
                    InPatientDeptListingContent.AdmissionInfo = CurrentRegistration.AdmissionInfo;

                    if (CurrentRegistration.AdmissionInfo == null)
                    {
                        CurrentRegistration.AdmissionInfo = new InPatientAdmDisDetails { AdmissionDate = Globals.GetCurServerDateTime() };
                    }

                    InPatientAdmissionInfoContent.CurrentAdmission = CurrentRegistration.AdmissionInfo;
                    InPatientAdmissionInfoContent.RegLockFlag = CurrentRegistration.RegLockFlag;
                    if (CurrentRegistration.AdmissionInfo.InPatientDeptDetails != null && CurrentRegistration.AdmissionInfo.InPatientDeptDetails.Count() > 0
                           && CurrentRegistration.AdmissionInfo.DischargeDate == null)
                    {
                        PatientAllocListingContent.InPtDeptDetail = CurrentRegistration.AdmissionInfo.InPatientDeptDetails.Where(x => !x.IsTemp && x.V_InPatientDeptStatus == AllLookupValues.InPatientDeptStatus.NHAP_KHOA_PHONG).First();
                    }


                    // TxD 23/05/2015: WHY the following line was there, NO IDEA, comment out for NOW if something happens then REVIEW
                    //InPatientAdmissionInfoContent.DepartmentChange();

                    if (CurrentRegistration.AdmissionInfo.InPatientAdmDisDetailID <= 0)
                    {
                        InPatientAdmissionInfoContent.BeginEditCmd();
                        //PatientAllocListingContent.ShowDeleteColumn = true;
                        GetLatesDiagTrmtByPtID_InPt_OnlyForDia(CurrentRegistration.PtRegistrationID, (long)AllLookupValues.V_DiagnosisType.DIAGNOSIS_DAILY);
                    }
                    else
                    {
                        //PatientAllocListingContent.ShowDeleteColumn = false;
                    }

                    if (CurrentRegistration.AdmissionInfo != null)
                    {
                        if (CurrentRegistration.AdmissionInfo.DischargeDate != null)
                        {
                            isDischarged = true;
                            NotifyOfPropertyChange(() => isDischarged);
                        }
                    }

                }
                else
                {
                    if (InPatientAdmissionInfoContent != null)
                    {
                        InPatientAdmissionInfoContent.CurrentAdmission = new InPatientAdmDisDetails();
                        InPatientAdmissionInfoContent.DepartmentChange();
                    }

                    if (InPatientDeptListingContent != null)
                    {
                        //KMx: Trong CurrentRegistration có chứa AdmissionInfo rồi, khi nào có thời gian thì xem xét và bỏ AdmissionInfo đi (11/09/2014 10:23).
                        InPatientDeptListingContent.CurrentRegistration = null;
                        InPatientDeptListingContent.AdmissionInfo = null;
                    }
                }

                if (PatientAllocListingContent != null)
                {
                    PatientAllocListingContent.Registration = CurrentRegistration;
                }
                // Ham NotifyOfPropertyChange chi co nhiem vu thay doi gia tri cua mot doi tuong tren giao dien khi doi tuong do co su thay doi, ngoai ra khong lam anh huong du lieu
                // Neu khong viet NotifyOfPropertyChange cho cac thuoc tinh khi khai bao, ham van chay chi co dieu neu gia tri do co hien thi tren giao dien thi se gay ra nham lan cho nguoi dung
                NotifyOfPropertyChange(() => isDischarged);
                PatientSummaryInfoContent.CurrentPatientRegistration = CurrentRegistration;
            }
        }

        private InPatientTransferDeptReq _InPatientTransferDeptReqsearch;
        public InPatientTransferDeptReq InPatientTransferDeptReqsearch
        {
            get { return _InPatientTransferDeptReqsearch; }
            set
            {
                _InPatientTransferDeptReqsearch = value;
                NotifyOfPropertyChange(() => InPatientTransferDeptReqsearch);
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

        private bool _isNhapKhoa = true;
        public bool isNhapKhoa
        {
            get { return _isNhapKhoa; }
            set
            {
                _isNhapKhoa = value;
                NotifyOfPropertyChange(() => isNhapKhoa);
            }
        }

        private void ResetView()
        {
            PatientSummaryInfoContent.SetPatientHISumInfo(null);
        }

        //▼====== #002
        protected override void OnActivate()
        {
            base.OnActivate();

            Globals.EventAggregator.Subscribe(this);
            if (!isAdmision)
            {
                var homeVm = Globals.GetViewModel<IHome>();
                IInPatientOutstandingTask ostvm = Globals.GetViewModel<IInPatientOutstandingTask>();
                homeVm.OutstandingTaskContent = ostvm;
                homeVm.IsExpandOST = true;
                ostvm.WhichVM = SetOutStandingTask.QUANLY_BENHNHAN_NOITRU;
                //▼===== #004: Khởi tạo LeftMenu chẩn đoán.
                DiagnosisTreatmentTree = Globals.GetViewModel<IDiagnosisTreatmentTree>();
                DiagnosisTreatmentTree.FromInPatientAdmView = true;
                homeVm.LeftMenu = DiagnosisTreatmentTree;
                ActivateItem(DiagnosisTreatmentTree);
                homeVm.IsEnableLeftMenu = true;
                homeVm.IsExpandLeftMenu = true;
                Globals.isConsultationStateEdit = true;
                //▲===== #004
            }
            else
            {
                //▼===== #005
                var homeVm = Globals.GetViewModel<IHome>();
                IInPatientAdmissionOutstandingTask ostvm = Globals.GetViewModel<IInPatientAdmissionOutstandingTask>();
                homeVm.OutstandingTaskContent = ostvm;
                homeVm.IsExpandOST = true;
                //▲===== #005
            }
        }
        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);
            Globals.EventAggregator.Unsubscribe(this);
            var homeVm = Globals.GetViewModel<IHome>();
            homeVm.OutstandingTaskContent = null;
            homeVm.IsExpandOST = false;
            //▼===== #004: Dispose khi rời khỏi màn hình
            homeVm.IsEnableLeftMenu = false;
            homeVm.IsExpandLeftMenu = false;
            //▲===== #004
        }
        //▲====== #002
        private ISearchPatientAndRegistration _searchRegistrationContent;

        public ISearchPatientAndRegistration SearchRegistrationContent
        {
            get { return _searchRegistrationContent; }
            set
            {
                _searchRegistrationContent = value;
                NotifyOfPropertyChange(() => SearchRegistrationContent);
            }
        }

        private IPatientSummaryInfoV2 _patientSummaryInfoContent;

        public IPatientSummaryInfoV2 PatientSummaryInfoContent
        {
            get { return _patientSummaryInfoContent; }
            set
            {
                _patientSummaryInfoContent = value;
                NotifyOfPropertyChange(() => PatientSummaryInfoContent);
            }
        }

        private IInPatientAdmissionInfo _inPatientAdmissionInfoContent;

        public IInPatientAdmissionInfo InPatientAdmissionInfoContent
        {
            get { return _inPatientAdmissionInfoContent; }
            set
            {
                _inPatientAdmissionInfoContent = value;
                NotifyOfPropertyChange(() => InPatientAdmissionInfoContent);
            }
        }

        private IInPatientBedPatientAllocListing _patientAllocListingContent;
        public IInPatientBedPatientAllocListing PatientAllocListingContent
        {
            get { return _patientAllocListingContent; }
            set
            {
                _patientAllocListingContent = value;
                NotifyOfPropertyChange(() => PatientAllocListingContent);
            }
        }

        private IInPatientDeptListing _inPatientDeptListingContent;
        public IInPatientDeptListing InPatientDeptListingContent
        {
            get { return _inPatientDeptListingContent; }
            set
            {
                _inPatientDeptListingContent = value;
                NotifyOfPropertyChange(() => InPatientDeptListingContent);
            }
        }

        private bool _isChangeDept = true;
        public bool isChangeDept
        {
            get { return _isChangeDept; }
            set
            {
                _isChangeDept = value;
                NotifyOfPropertyChange(() => isChangeDept);
            }
        }

        private bool _isDischarged;
        public bool isDischarged
        {
            get { return _isDischarged; }
            set
            {
                _isDischarged = value;
                NotifyOfPropertyChange(() => isDischarged);
                isChangeDept = !isDischarged;
            }
        }

        private Patient _currentPatient;

        public Patient CurrentPatient
        {
            get { return _currentPatient; }
            set
            {
                _currentPatient = value;
                NotifyOfPropertyChange(() => CurrentPatient);
                NotifyOfPropertyChange(() => CanCreateRegistrationCmd);
                PatientSummaryInfoContent.CurrentPatient = _currentPatient;
            }
        }

        private void ResetPatientSummaryInfo()
        {
            PatientSummaryInfoContent.SetPatientHISumInfo(null);
            PatientSummaryInfoContent.CurrentPatientClassification = null;
        }

        private void ShowPatientSummaryInfo(PatientRegistration regInfo)
        {
            PatientSummaryInfoContent.CurrentPatient = regInfo.Patient;

            PatientSummaryInfoContent.SetPatientHISumInfo(regInfo.PtHISumInfo);

            PatientSummaryInfoContent.CurrentPatientClassification = regInfo.PatientClassification;
        }

        private RegistrationFormMode _currentRegMode = RegistrationFormMode.PATIENT_NOT_SELECTED;
        public RegistrationFormMode CurrentRegMode
        {
            get
            {
                return _currentRegMode;
            }
            set
            {
                if (_currentRegMode != value)
                {
                    _currentRegMode = value;
                    NotifyOfPropertyChange(() => CurrentRegMode);
                    NotifyOfPropertyChange(() => CanCreateRegistrationCmd);
                }
            }
        }
        public void Handle(ItemSelected<Patient> message)
        {
            if (message != null)
            {
                CurrentPatient = message.Item;
                Coroutine.BeginExecute(DoSetCurrentPatient(message.Item));
            }
        }
        MessageBoxTask _msgTask;
        public IEnumerator<IResult> DoSetCurrentPatient(Patient p)
        {
            ////Tam thoi de cho nay de test
            //CreateNewAdmissionInfo();

            CanCreateNewRegistration = true;

            ResetPatientSummaryInfo();
            if (p == null || p.PatientID <= 0)
            {
                CurrentPatient = null;
                CurrentRegMode = RegistrationFormMode.PATIENT_NOT_SELECTED;
                yield break;
            }

            yield return Loader.Show(eHCMSResources.Z0119_G1_DangLayTTinBN);
            PatientLoaded = false;
            PatientLoading = true;
            var loadPatient = new LoadPatientTask(p.PatientID);
            yield return loadPatient;

            // TxD 02/08/2014 Use Global's Server Date instead
            //var loadCurrentDate = new LoadCurrentDateTask();
            //yield return loadCurrentDate;
            DateTime today;
            //if (loadCurrentDate.CurrentDate == DateTime.MinValue)
            //{
            //    today = DateTime.Now.Date;
            //    _msgTask = new MessageBoxTask(string.Format("{0}.", eHCMSResources.Z0144_G1_KhongTheLayNgThTuServer), eHCMSResources.G0442_G1_TBao);
            //    yield return _msgTask;
            //}
            //else
            //{
            //    today = loadCurrentDate.CurrentDate.Date;
            //}

            today = Globals.GetCurServerDateTime().Date;

            if (loadPatient.CurrentPatient != null)
            {
                if (loadPatient.CurrentPatient.LatestRegistration_InPt != null)
                {
                    var regDate = loadPatient.CurrentPatient.LatestRegistration_InPt.ExamDate.Date;
                    CurrentPatient = loadPatient.CurrentPatient;
                    CurrentRegMode = RegistrationFormMode.PATIENT_SELECTED;

                    OpenRegistration(loadPatient.CurrentPatient.LatestRegistration_InPt.PtRegistrationID);
                }
                else/*Ngoai Tru*/
                {
                    if (loadPatient.CurrentPatient.LatestRegistration != null)
                    {

                        if (
                            (loadPatient.CurrentPatient.LatestRegistration.V_RegistrationType != AllLookupValues.RegistrationType.NOI_TRU)
                            && (loadPatient.CurrentPatient.LatestRegistration.V_RegistrationStatus == (long)AllLookupValues.RegistrationStatus.OPENED)
                            )
                        {
                            //string message = "Bệnh nhân chưa được chuẩn bị đăng ký nhập viện." + Environment.NewLine + "Không thể load đăng ký.";
                            string message = eHCMSResources.Z0145_G1_BNDangCoDKNgoaiTru + string.Format(". \n{0}", eHCMSResources.Z0084_H1_KhongTheLoadDK);
                            msgTask = new MessageBoxTask(message, eHCMSResources.G0442_G1_TBao);
                            yield return msgTask;
                            yield return Loader.Hide();
                            yield break;
                        }
                    }
                }
            }
            else
            {
                CurrentPatient = null;
                CurrentRegMode = RegistrationFormMode.PATIENT_NOT_SELECTED;

                _msgTask = new MessageBoxTask(eHCMSResources.Z0146_G1_KhongTheLayTTinBN, eHCMSResources.G0442_G1_TBao);
                yield return _msgTask;
            }

            yield return Loader.Hide();
        }

        private bool _registrationInfoHasChanged;
        /// <summary>
        /// Cho biet thong tin dang ky tren form da duoc thay doi chua.
        /// </summary>
        public bool RegistrationInfoHasChanged
        {
            get
            {
                return _registrationInfoHasChanged;
            }
            set
            {
                if (_registrationInfoHasChanged != value)
                {
                    _registrationInfoHasChanged = value;
                    NotifyOfPropertyChange(() => RegistrationInfoHasChanged);

                    //NotifyButtonBehaviourChanges();
                }
            }
        }

        //public bool IsProcessing
        //{
        //    get
        //    {
        //        return _patientLoading;
        //    }
        //}

        private bool _patientLoading;
        /// <summary>
        /// Dang trong qua trinh lay thong tin benh nhan tu server.
        /// </summary>
        public bool PatientLoading
        {
            get
            {
                return _patientLoading;
            }
            set
            {
                _patientLoading = value;
                NotifyOfPropertyChange(() => PatientLoading);

                NotifyWhenBusy();
            }
        }
        private void NotifyWhenBusy()
        {
            NotifyOfPropertyChange(() => IsProcessing);
        }
        private bool _patientLoaded;
        public bool PatientLoaded
        {
            get
            {
                return _patientLoaded;
            }
            set
            {
                _patientLoaded = value;
                NotifyOfPropertyChange(() => PatientLoaded);
            }
        }

        public void Handle(ResultFound<Patient> message)
        {
            if (message != null)
            {
                CurrentPatient = message.Result;
                if (CurrentPatient != null)
                {
                    //SetCurrentPatient(CurrentPatient);
                    Globals.EventAggregator.Publish(new ItemSelected<Patient> { Item = CurrentPatient });
                }
            }
        }

        public void Handle(ResultNotFound<Patient> message)
        {
            if (message != null)
            {
                //Thông báo không tìm thấy bệnh nhân.
                MessageBoxResult result = MessageBox.Show(eHCMSResources.A0727_G1_Msg_ConfThemMoiBN,
                                                          eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel);
                if (result == MessageBoxResult.OK)
                {

                }
            }
        }

        public void OldRegistrationsCmd()
        {
            Action<IRegistrationList> onInitDlg = delegate (IRegistrationList vm)
            {
                vm.IsInPtRegistration = true;
                vm.CurrentPatient = CurrentPatient;
            };
            GlobalsNAV.ShowDialog<IRegistrationList>(onInitDlg);
        }

        private bool _isRegistering;
        public bool IsRegistering
        {
            get
            {
                return _isRegistering;
            }
            set
            {
                _isRegistering = value;
                NotifyOfPropertyChange(() => IsRegistering);

                NotifyWhenBusy();
            }
        }

        private bool _canCreateNewRegistration;

        public bool CanCreateNewRegistration
        {
            get { return _canCreateNewRegistration; }
            set
            {
                _canCreateNewRegistration = value;
                NotifyOfPropertyChange(() => CanCreateRegistrationCmd);
            }
        }

        public bool CanCreateRegistrationCmd
        {
            get
            {
                return _currentPatient != null && CurrentRegistration.V_RegistrationStatus == (long)AllLookupValues.RegistrationStatus.PENDING
                    && CurrentRegMode == RegistrationFormMode.PATIENT_SELECTED && CanCreateNewRegistration;
            }
        }

        public bool ValidateRegistrationInfo(out ObservableCollection<System.ComponentModel.DataAnnotations.ValidationResult> result)
        {
            result = new ObservableCollection<System.ComponentModel.DataAnnotations.ValidationResult>();
            if (_currentPatient == null)
            {
                var item = new System.ComponentModel.DataAnnotations.ValidationResult(string.Format("{0}.", eHCMSResources.Z0148_G1_HayChon1BN), new[] { "CurrentPatient" });
                result.Add(item);
            }

            if (result.Count > 0)
            {
                return false;
            }
            return true;
        }
        private List<DiagnosisTreatment> _DiagnosisTreatmentCollection;
        public List<DiagnosisTreatment> DiagnosisTreatmentCollection
        {
            get
            {
                return _DiagnosisTreatmentCollection;
            }
            set
            {
                if (_DiagnosisTreatmentCollection == value)
                {
                    return;
                }
                _DiagnosisTreatmentCollection = value;
                NotifyOfPropertyChange(() => DiagnosisTreatmentCollection);
            }
        }
        public void LoadDiagnosisTreatmentCollectionTask(GenericCoRoutineTask aGenTask, object aPtRegistrationID)
        {
            long PtRegistrationID = Convert.ToInt64(aPtRegistrationID);
            this.ShowBusyIndicator();
            var CurrentThread = new Thread(() =>
            {
                using (var mFactory = new ePMRsServiceClient())
                {
                    var mContract = mFactory.ServiceInstance;
                    mContract.BeginGetDiagnosisTreatment_InPt_ByPtRegID(PtRegistrationID, null, null, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var ItemCollection = mContract.EndGetDiagnosisTreatment_InPt_ByPtRegID(asyncResult);
                            if (ItemCollection == null)
                            {
                                DiagnosisTreatmentCollection = new List<DiagnosisTreatment>();
                            }
                            else
                            {
                                DiagnosisTreatmentCollection = ItemCollection.ToList();
                            }
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            this.HideBusyIndicator();
                            aGenTask.ActionComplete(true);
                        }
                    }), null);
                }
            });
            CurrentThread.Start();
        }
        private void ShowOldRegistration(PatientRegistration regInfo)
        {
            if (regInfo.AdmissionInfo == null)
            {
                var info = new InPatientAdmDisDetails { PatientRegistration = regInfo };
                regInfo.AdmissionInfo = info;
            }

            if (regInfo.BedAllocations != null)
            {
                foreach (var bed in regInfo.BedAllocations)
                {
                    if (!bed.CheckOutDate.HasValue)
                    {
                        bed.CanDelete = true;
                    }
                    else
                    {
                        bed.CanDelete = false;
                    }
                }
            }
            DiagnosisTreatmentCollection = new List<DiagnosisTreatment>();
            CurrentRegistration = regInfo;

            //Chuyen sang mode giong nhu mo lai dang ky cu
            CurrentRegMode = RegistrationFormMode.OLD_REGISTRATION_OPENED;
            CurrentPatient = regInfo.Patient;
            ShowPatientSummaryInfo(regInfo);
            //HPT 17/01/2017: Tìm luôn danh sách đề nghị chuyển khoa được tạo bởi khoa đang thao tác (nếu có)
            SearchCmd();
        }

        #region COROUTINES
        //public IEnumerator<IResult> DoCalcHiBenefit(HealthInsurance hiItem, PaperReferal referal)
        //{
        //    bool isEmergency = CurrentRegistration.EmergRecID > 0 ? true : false;
        //    var calcHiTask = new CalcHiBenefitTask(hiItem, referal, (long)AllLookupValues.RegistrationType.NOI_TRU, isEmergency);
        //    yield return calcHiTask;
        //    if (calcHiTask.Error == null)
        //    {
        //        PatientSummaryInfoContent.HiBenefit = calcHiTask.HiBenefit;
        //    }
        //}
        #endregion

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////


        public void Next()
        {

        }
        public void Previous()
        {

        }
        private bool _ShowAdmissionCmd;
        public bool ShowAdmissionCmd
        {
            get
            {
                return CurrentRegistration != null
                    && CurrentRegistration.AdmissionInfo != null
                    && CurrentRegistration.AdmissionInfo.InPatientAdmDisDetailID <= 0
                    && mNhapVien_NhapVien;
            }
            set
            {
                _ShowAdmissionCmd = value;
                NotifyOfPropertyChange(() => ShowAdmissionCmd);
            }
        }
        public bool ShowMergerCmd
        {
            get
            {
                return !isAdmision && Globals.ServerConfigSection.InRegisElements.MergerPatientRegistration == 2
                    && CurrentRegistration != null && CurrentRegistration.OutPtRegistrationID > 0 && CurrentRegistration.AdmissionInfo != null
                        && CurrentRegistration.AdmissionInfo.InPatientAdmDisDetailID > 0;
            }
            //set
            //{
            //    _ShowMergerCmd = value;
            //    NotifyOfPropertyChange(() => ShowMergerCmd);
            //}
        }
        public void AdmissionCmd()
        {
            // Hpt 15/10/2015: Khong biet co truong hop nao lam cho CurrentRegistration = null hay khong nhung biet chac neu CurrentRegistration = null thi code ben duoi bi loi (?? Khong hieu ly do)
            // Vi the phai chac chan CurrentRegistration != null truoc khi thuc hien cac buoc tiep theo
            if (CurrentRegistration == null)
            {
                return;
            }
            // Hpt 13/10/2015: Khong nhap vien tu dang ky VANG LAI. ATUAN noi se phat sinh nhieu van de??
            if (CurrentRegistration.V_RegForPatientOfType == AllLookupValues.V_RegForPatientOfType.DKBN_VANG_LAI && InPatientAdmissionInfoContent.IsNotGuestEmergencyAdmission)
            {
                MessageBox.Show(eHCMSResources.A0500_G1_Msg_InfoDKVLaiKhongKhaDung);
                return;
            }
            if (Globals.IsLockRegistration(CurrentRegistration.RegLockFlag, eHCMSResources.N0221_G1_NhapVien.ToLower()))
            {
                return;
            }
            if (CurrentRegistration.V_RegForPatientOfType == AllLookupValues.V_RegForPatientOfType.NBNT_BN_TIEN_PHAU_KHONG_BHYT)
            {
                if (CurrentRegistration.AdmissionInfo.AdmissionDate.HasValue && CurrentRegistration.AdmissionInfo.AdmissionDate.GetValueOrDefault() < CurrentRegistration.ExamDate)
                {
                    MessageBox.Show(eHCMSResources.A0853_G1_Msg_InfoNgNpVienKhHopLe, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                    return;
                }
                int NumOfDayAllowPending = Globals.ServerConfigSection.InRegisElements.NumOfDayAllowPending_PreOpReg;
                // Neu khong kiem tra dieu kien ngay nhap vien co gia tri truoc thi khi ngay nhap vien bang null, cac dieu kien ben duoi se vo nghia va ket qua ham nay se sai
                if (CurrentRegistration.AdmissionInfo.AdmissionDate.HasValue)
                {
                    TimeSpan NumOfDaysAfterRegis = CurrentRegistration.AdmissionInfo.AdmissionDate.GetValueOrDefault() - CurrentRegistration.ExamDate;
                    if (NumOfDaysAfterRegis.Days > NumOfDayAllowPending)
                    {
                        MessageBoxResult confirm = MessageBox.Show(eHCMSResources.A0852_G1_Msg_ConfNpVienDKKTC, eHCMSResources.G2363_G1_XNhan, MessageBoxButton.OKCancel);
                        if (confirm == MessageBoxResult.Cancel)
                        {
                            return;
                        }
                    }
                }
            }
            ObservableCollection<System.ComponentModel.DataAnnotations.ValidationResult> validationResults;
            bool isValid = ValidateFullInfo(out validationResults);
            if (!isValid)
            {
                Globals.EventAggregator.Publish(new ValidateFailedEvent { ValidationResults = validationResults });
                return;
            }
            if (CurrentRegistration.AdmissionInfo.InPatientDeptDetails == null)
            {
                CurrentRegistration.AdmissionInfo.InPatientDeptDetails = new BindableCollection<InPatientDeptDetail>();
            }
            InPatientDeptDetail deptDetail = new InPatientDeptDetail();
            deptDetail.DeptLocation = InPatientAdmissionInfoContent.SelectedLocation;
            deptDetail.FromDate = CurrentRegistration.AdmissionInfo.AdmissionDate.Value;
            deptDetail.V_InPatientDeptStatus = AllLookupValues.InPatientDeptStatus.NHAP_KHOA_PHONG;
            CurrentRegistration.AdmissionInfo.IsGuestEmergencyAdmission = !InPatientAdmissionInfoContent.IsNotGuestEmergencyAdmission;
            //CurrentRegistration.AdmissionInfo.InPatientDeptDetails.Add(deptDetail);
            this.ShowBusyIndicator();
            //OK roi thi Nhap vien luon.
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginAddInPatientAdmission(CurrentRegistration, deptDetail, Globals.LoggedUserAccount.StaffID.GetValueOrDefault(), Globals.DeptLocation.DeptLocationID, Globals.DispatchCallback(asyncResult =>
                        {
                            try
                            {
                                long savedRegInfoID;
                                contract.EndAddInPatientAdmission(out savedRegInfoID, asyncResult);
                                //ShowOldRegistration(savedRegInfo);
                                OpenRegistration(savedRegInfoID);
                                MessageBox.Show(eHCMSResources.A0888_G1_Msg_InfoNpVienOK);
                                //▼===== #006
                                if (Globals.ServerConfigSection.InRegisElements.MergerPatientRegistration == 1)
                                {
                                    MergerCmd();
                                }
                                //▲===== #006
                            }
                            //catch (FaultException<AxException> fault)
                            //{
                            //    ClientLoggerHelper.LogInfo(fault.ToString());
                            //}
                            catch (Exception ex)
                            {
                                ClientLoggerHelper.LogInfo(ex.ToString());
                                MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
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
                    MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                }
                finally
                {
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }
        bool ValidateFullInfo(out ObservableCollection<System.ComponentModel.DataAnnotations.ValidationResult> validationResults)
        {
            validationResults = null;

            ObservableCollection<System.ComponentModel.DataAnnotations.ValidationResult> admissionValidationResults;
            bool admissionValid = InPatientAdmissionInfoContent.ValidateInfo(out admissionValidationResults);

            ObservableCollection<System.ComponentModel.DataAnnotations.ValidationResult> regInfoValidationResults;
            bool regInfoValid = ValidateRegistrationInfo(out regInfoValidationResults);

            bool valid = admissionValid & regInfoValid;
            if (valid)
            {
                return true;
            }

            validationResults = new ObservableCollection<System.ComponentModel.DataAnnotations.ValidationResult>();
            if (admissionValidationResults != null)
            {
                foreach (var item in admissionValidationResults)
                {
                    validationResults.Add(item);
                }
            }
            if (regInfoValidationResults != null)
            {
                foreach (var item in regInfoValidationResults)
                {
                    validationResults.Add(item);
                }
            }
            return false;
        }

        public void Handle(ItemSelected<PatientRegistration> message)
        {
            if (message != null && message.Item != null)
            {
                InPatientAdmissionInfoContent.IsAdmissionFromSuggestion = false;
                OpenRegistration(message.Item.PtRegistrationID);
            }
        }
        //▼====== #002
        public void Handle(InPatientRegistrationSelectedForInPatientAdmission message)
        {
            if (message != null && message.Source != null)
            {
                InPatientAdmissionInfoContent.IsAdmissionFromSuggestion = false;
                OpenRegistration(message.Source.PtRegistrationID);
            }
        }
        //▲====== #002
        //▼===== #005
        public void Handle(InPatientSelectedForInPatientAdmission message)
        {
            if (message != null && message.Source != null)
            {
                InPatientAdmissionInfoContent.IsAdmissionFromSuggestion = false;
                OpenRegistration(message.Source.PtRegistrationID);
            }
        }
        //▲===== #005
        /// <summary>
        /// Mở đăng ký đã có sẵn
        /// Lên server lấy đầy đủ thông tin của đăng ký
        /// </summary>
        /// <param name="regID">ID của đăng ký</param>
        public void OpenRegistration(long regID)
        {
            IsProcessing = true;
            Coroutine.BeginExecute(DoOpenRegistration(regID), null, (o, e) => { IsProcessing = false; });
            allInPatientTransferDeptReq = new ObservableCollection<InPatientTransferDeptReq>();
        }
        //▼===== #003
        private void DefaultValueDepartmentForRequest(long PtRegistrationID)
        {
            LoadDeptAdmissionRequest(PtRegistrationID);
        }

        public void LoadDeptAdmissionRequest(long PtRegistrationID)
        {
            if (InPatientAdmissionInfoContent == null)
            {
                return;
            }
            if (InPatientAdmissionInfoContent.CurrentDepartment == null)
            {
                return;
            }
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginLoadDeptAdmissionRequest(PtRegistrationID, Globals.DispatchCallback(asyncResult =>
                        {
                            try
                            {
                                RefDepartment department = contract.EndLoadDeptAdmissionRequest(asyncResult);
                                if (department != null)
                                {
                                    System.Diagnostics.Debug.WriteLine("-------------------- Chỗ thứ 2: Set khoa phòng hiện tại cho nhập viện ----------------------");
                                    InPatientAdmissionInfoContent.CurrentDepartment = department;
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
        //▲===== #003
        // Vi VM nay chua nhung VM con co kha nang tu load du lieu nen can mot ham thuc hien xoa tat ca nhung du lieu con luu khi xay ra loi de tranh truong hop nham lan, dau nay duoi no
        // Khi load dang ky bi loi, thi thong tin ve dang ky cu con tren giao dien phai duoc xoa sach
        private void ClearAllInfoData()
        {
            CurrentRegistration = null;
            CurrentPatient = null;
            PatientSummaryInfoContent.CurrentPatient = null;
            InPatientAdmissionInfoContent.CurrentAdmission = null;
            InPatientAdmissionInfoContent.RegLockFlag = 0;
            InPatientDeptListingContent.AllItems = null;
        }

        MessageBoxTask msgTask;
        public IEnumerator<IResult> DoOpenRegistration(long regID)
        {

            //KMx: Chỉ lấy những thông tin cần thiết của đăng ký thôi, không load hết (17/09/2014 17:31).
            LoadRegistrationSwitch LoadRegisSwitch = new LoadRegistrationSwitch();
            LoadRegisSwitch.IsGetBedAllocations = true;
            LoadRegisSwitch.IsGetAdmissionInfo = true;

            var loadRegTask = new LoadRegistrationInfo_InPtTask(regID, (int)AllLookupValues.V_FindPatientType.NOI_TRU, LoadRegisSwitch);
            yield return loadRegTask;

            if (loadRegTask.Registration == null)
            {
                //Thong bao khong load duoc dang ky
                Globals.EventAggregator.Publish(new ShowMessageEvent() { Message = eHCMSResources.A0733_G1_Msg_InfoKhTimThayDK });
                Globals.EventAggregator.Publish(new ItemLoaded<PatientRegistration, long>() { Item = null, ID = regID });
            }
            else
            {
                if (loadRegTask.Registration.V_RegistrationType != AllLookupValues.RegistrationType.NOI_TRU && loadRegTask.Registration.V_RegistrationType != AllLookupValues.RegistrationType.NOI_TRU_BHYT &&
                    loadRegTask.Registration.V_RegistrationType != AllLookupValues.RegistrationType.CAP_CUU && loadRegTask.Registration.V_RegistrationType != AllLookupValues.RegistrationType.CAP_CUU_BHYT)
                {
                    string message = eHCMSResources.Z0085_G1_DayKhongPhaiDKNoiTru + Environment.NewLine + eHCMSResources.Z0084_H1_KhongTheLoadDK;
                    msgTask = new MessageBoxTask(message, eHCMSResources.G0442_G1_TBao);
                    yield return msgTask;
                    yield break;
                }
                if (loadRegTask.Registration.RegistrationStatus == AllLookupValues.RegistrationStatus.REFUND || loadRegTask.Registration.RegistrationStatus == AllLookupValues.RegistrationStatus.COMPLETED)
                {
                    MessageBox.Show(eHCMSResources.A0490_G1_Msg_KhTheNpVien_Status.ToUpper(), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                    ClearAllInfoData();
                    yield break;
                }
                if (loadRegTask.Registration.V_RegForPatientOfType == AllLookupValues.V_RegForPatientOfType.DKBN_VANG_LAI && loadRegTask.Registration.DeptID != Globals.ServerConfigSection.InRegisElements.EmerDeptID)
                {
                    MessageBox.Show(eHCMSResources.A0500_G1_Msg_InfoDKVLaiKhongKhaDung, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                    ClearAllInfoData();
                    yield break;
                }
                //if (_currentPatient != null && loadRegTask.Registration.PatientID != CurrentPatient.PatientID)
                //{
                //    string newPatientName = loadRegTask.Registration != null &&
                //                            loadRegTask.Registration.Patient != null
                //                            ? loadRegTask.Registration.Patient.FullName : "";

                //    string message = string.Format("Bạn đang thao tác với bệnh nhân '{0}'." + Environment.NewLine + "Bạn có muốn chuyển sang đăng ký của bệnh nhân '{1}'?", _currentPatient.FullName, newPatientName);
                //    msgTask = new MessageBoxTask(message, eHCMSResources.G0442_G1_TBao, MessageBoxOptions.OkCancel);
                //    yield return msgTask;
                //    if (msgTask.Result == AxMessageBoxResult.Ok)
                //    {
                //        ShowOldRegistration(loadRegTask.Registration);

                //        PatientSummaryInfoContent.CurrentPatient = _currentPatient;
                //    }
                //}
                //else if (CurrentRegistration != loadRegTask.Registration)
                //{

                ShowOldRegistration(loadRegTask.Registration);
                //kiem tra dang o khoa phong nao
                if (CurrentRegistration.AdmissionInfo != null
                    && CurrentRegistration.AdmissionInfo.InPatientDeptDetails != null
                    && CurrentRegistration.AdmissionInfo.InPatientDeptDetails.Count > 1)
                {
                    CurrentRegistration.DeptID = CurrentRegistration.AdmissionInfo.InPatientDeptDetails[0].DeptLocation.DeptID;
                }
                if (loadRegTask.Registration.V_RegForPatientOfType == AllLookupValues.V_RegForPatientOfType.DKBN_VANG_LAI && loadRegTask.Registration.DeptID == Globals.ServerConfigSection.InRegisElements.EmerDeptID)
                {
                    InPatientAdmissionInfoContent.SetEmergencyAmissionInfo(Globals.ServerConfigSection.InRegisElements.EmerDeptID);
                }
                else
                {
                    InPatientAdmissionInfoContent.SetEmergencyAmissionInfo(0);
                    System.Diagnostics.Debug.WriteLine("-------------------- Chỗ thứ 1: Gọi set giá trị mặc định cho khoa ----------------------");
                    //▼===== #003
                    //DefaultValueDepartmentForRequest(CurrentRegistration.PtRegistrationID);
                    //▲===== #003
                }
                Globals.EventAggregator.Publish(new ItemLoaded<PatientRegistration, long>() { Item = CurrentRegistration, ID = regID });
                // }
                if (!isAdmision)
                {
                    if (DiagnosisTreatmentTree.Registration_DataStorage == null)
                    {
                        DiagnosisTreatmentTree.Registration_DataStorage = new Registration_DataStorage();
                    }
                    DiagnosisTreatmentTree.Registration_DataStorage.CurrentPatientRegistration = CurrentRegistration;
                    DiagnosisTreatmentTree.LoadData(CurrentRegistration.PtRegistrationID);
                }
            }
        }
        public bool ShowChangeDeptCmd
        {
            get
            {
                return CurrentRegistration != null
                    && CurrentRegistration.AdmissionInfo != null
                    && CurrentRegistration.AdmissionInfo.InPatientAdmDisDetailID > 0
                    && mNhapVien_DeNghiChuyenKhoa;
            }
        }

        public void ChangeDeptCmd()
        {
            if (CurrentRegistration == null || CurrentRegistration.AdmissionInfo == null || CurrentRegistration.PtRegistrationID == 0)
            {
                return;
            }
            if (Globals.IsLockRegistration(CurrentRegistration.RegLockFlag, eHCMSResources.N0208_G1_Nhap_ChuyenKP.ToLower()))
            {
                return;
            }
            if (CurrentRegistration.AdmissionInfo.InPatientDeptDetails == null || CurrentRegistration.AdmissionInfo.InPatientDeptDetails.Count <= 0)
            {
                MessageBox.Show(eHCMSResources.Z0302_G1_KgLamDNghiChKhoaDuoc, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
            Coroutine.BeginExecute(CallChangeDept_Routine());
        }
        private IEnumerator<IResult> CallChangeDept_Routine()
        {
            //Load các chẩn đoán của đăng ký
            if (DiagnosisTreatmentCollection == null || DiagnosisTreatmentCollection.Count == 0)
            {
                var CurrentTask = new GenericCoRoutineTask(LoadDiagnosisTreatmentCollectionTask, CurrentRegistration.PtRegistrationID);
                yield return CurrentTask;
            }
            ObservableCollection<long> ListDepartment = new ObservableCollection<long>();
            ObservableCollection<long> AvailableDepartments = new ObservableCollection<long>();
            if (Globals.isAccountCheck)
            {
                //KMx: 1. Danh sách Khoa bệnh nhân đã từng ở (10/09/2014 16:28).
                foreach (InPatientDeptDetail item in CurrentRegistration.AdmissionInfo.InPatientDeptDetails)
                {
                    if (item.DeptLocation == null || item.DeptLocation.DeptID <= 0)
                    {
                        continue;
                    }
                    //KMx: 2. Nếu nhân viên có cấu hình trách nhiệm cho Khoa mà bệnh nhân đã từng ở thì add vào (Nếu đã có trong List thì không add) (10/09/2014 16:38).
                    if (Globals.LoggedUserAccount.DeptIDResponsibilityList.Contains(item.DeptLocation.DeptID))
                    {
                        if (!ListDepartment.Any(x => x == item.DeptLocation.DeptID))
                        {
                            ListDepartment.Add(item.DeptLocation.DeptID);
                        }
                        if (item.ToDate == null && item.V_InPatientDeptStatus != AllLookupValues.InPatientDeptStatus.XUAT_KHOA_PHONG
                            && !AvailableDepartments.Any(x => x == item.DeptLocation.DeptID))
                        {
                            AvailableDepartments.Add(item.DeptLocation.DeptID);
                        }
                    }
                }
                if (ListDepartment == null || ListDepartment.Count <= 0)
                {
                    MessageBox.Show(eHCMSResources.A0104_G1_Msg_InfoKhTheLamDNChKhoa, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                    yield break;
                }
                /*▼====: #001*/
                //if (AvailableDepartments == null || AvailableDepartments.Count() <= 0)
                //{
                //    MessageBox.Show("Bệnh nhân đã xuất khỏi khoa! Không thể làm đề nghị chuyển khoa!", eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                //    return;
                //}
                /*▲====: #001*/
            }

            Action<IChangeDept> onInitDlg = delegate (IChangeDept vm)
            {
                //vm.CurInPatientDeptDetail = selectItem.DeepCopy();
                //vm.OriginalDeptLocation = selectItem.DeptLocation;
                vm.CurrentAdmission = CurrentRegistration.AdmissionInfo;
                vm.LstRefDepartment = AvailableDepartments;
                //▼===== 20200103 TTM: Thêm điều kiện kiểm tra nếu là màn hình chọn khoa để đề nghị chuyển thì không lấy khoa đã xoá.
                //              Vì anh Tuấn nói để lại để xem các trường hợp cũ. Ở chức năng khác.
                vm.IsChangedDept = true;
                //▲=====
                vm.LoadData();
                vm.DiagnosisTreatmentCollection = DiagnosisTreatmentCollection;
                //20191104 TBL: Lấy khoa đang nằm không phải là khoa tạm và chưa được xuất khoa
                if (InPatientDeptListingContent.AllItems != null && InPatientDeptListingContent.AllItems.Count > 0)
                {
                    vm.CurrentDeptID = InPatientDeptListingContent.AllItems.Where(x => !x.IsTemp && x.V_InPatientDeptStatus == AllLookupValues.InPatientDeptStatus.NHAP_KHOA_PHONG).First().DeptLocation.RefDepartment.DeptID;
                }
            };
            GlobalsNAV.ShowDialog<IChangeDept>(onInitDlg);
        }
        //public void ChangeDeptCmd()
        //{
        //    DeptLocation currentDeptLoc = null;

        //    if (CurrentRegistration != null && CurrentRegistration.AdmissionInfo != null
        //        && CurrentRegistration.AdmissionInfo.InPatientDeptDetails != null
        //        && CurrentRegistration.AdmissionInfo.InPatientDeptDetails.Count > 0)
        //    {
        //        var lastItem = CurrentRegistration.AdmissionInfo.InPatientDeptDetails.FirstOrDefault();
        //        if (lastItem != null)
        //        {
        //            currentDeptLoc = lastItem.DeptLocation;
        //        }
        //    }
        //    if (currentDeptLoc == null || currentDeptLoc.DeptLocationID <= 0)
        //    {
        //        MessageBox.Show("Chưa có phòng hiện tại.");
        //        return;
        //    }

        //    // Kiểm Tra xem nhân viên này có trách nhiệm chuyển khoa hay không
        //    if (Globals.isAccountCheck)
        //    {
        //        if (LstRefDepartment == null || LstRefDepartment.Count < 1)
        //        {
        //            MessageBox.Show("Nhân viên này chưa được phân bố trách nhiệm cho khoa phòng nào. \nLiên hệ người quản trị để nhận thêm thông tin.");
        //            return;
        //        }
        //        else
        //        {
        //            bool flag = false;
        //            for (int i = 0; i < LstRefDepartment.Count; i++)
        //            {
        //                if (LstRefDepartment[i] == CurrentRegistration.AdmissionInfo.Department.DeptID)
        //                {
        //                    flag = true;
        //                    break;
        //                }
        //            }
        //            if (!flag)
        //            {
        //                MessageBox.Show("Nhân viên này chưa được phân bố trách nhiệm cho khoa phòng này. \nLiên hệ người quản trị để nhận thêm thông tin.");
        //                return;
        //            }
        //        }
        //    }

        //    var vm = Globals.GetViewModel<IChangeDept>();
        //    vm.OriginalDeptLocation = currentDeptLoc;
        //    if (CurrentRegistration != null)
        //    {
        //        vm.CurrentAdmission = CurrentRegistration.AdmissionInfo;
        //    }
        //    vm.LoadData();
        //    Globals.ShowDialog(vm as Conductor<object>);
        //}

        public void Handle(CallBeginNewOutPtTreatment message)
        {
            AddDeptCmd(false, true);
        }
        public void AddDeptCmd(bool IsTemp, bool IsChangeToCurrentDeptLocation = false)
        {
            if (CurrentRegistration == null || CurrentRegistration.AdmissionInfo == null)
            {
                MessageBox.Show(eHCMSResources.K0283_G1_ChonBNDeChKhoa, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
            if (Globals.IsLockRegistration(CurrentRegistration.RegLockFlag, eHCMSResources.N0208_G1_Nhap_ChuyenKP.ToLower()))
            {
                return;
            }
            Action<IAcceptChangeDept> onInitDlg = delegate (IAcceptChangeDept vm)
            {
                vm.IsTemp = IsTemp;
                vm.CurrentRegistration = CurrentRegistration;
                vm.IsFromRequestPaper = false;
                //Danh sách Khoa của nhân viên được cấu hình trách nhiệm (11/07/2014 16:23).
                //vm.LstRefDepartment = LstRefDepartment;
                if (IsChangeToCurrentDeptLocation && InPatientDeptListingContent != null && InPatientDeptListingContent.AllItems != null
                    && InPatientDeptListingContent.AllItems.Count > 0)
                {
                    vm.SetDesDepartment(InPatientDeptListingContent.AllItems.First().DeptLocation);
                }
                vm.LoadData();
                vm.CurInPatientTransferDeptReq = new InPatientTransferDeptReq();
                vm.CurInPatientTransferDeptReq.InPatientAdmDisDetailID = CurrentRegistration.AdmissionInfo.InPatientAdmDisDetailID;
                vm.CurInPatientTransferDeptReq.InPatientTransferDeptReqID = 0;
            };
            GlobalsNAV.ShowDialog<IAcceptChangeDept>(onInitDlg);
        }

        public void AddDept()
        {
            AddDeptCmd(false);
        }

        public void AddDept_Temp()
        {
            AddDeptCmd(true);
        }

        public void SearchCmd()
        {
            //if (!Globals.isAccountCheck)
            //{
            //KMx: Bệnh nhân đăng ký ở khoa B, nhưng người dùng được cấu hình trách nhiệm ở khoa A.
            //Dẫn đến nhân viên khoa A load phiếu yêu cầu chuyển khoa của bệnh nhân không được. Nên mark code ra cho tìm được (11/07/2014 16:40).
            if (CurrentRegistration != null && CurrentRegistration.PtRegistrationID > 0)
            {
                InPatientTransferDeptReqsearch.PtRegistrationID = CurrentRegistration.PtRegistrationID;
                GetInPatientTransferDeptReq(InPatientTransferDeptReqsearch);
            }
            //}
            //else
            //    if (LstRefDepartment != null)
            //    {
            //        InPatientTransferDeptReqsearch.LstRefDepartment = new ObservableCollection<long>(LstRefDepartment);
            //        bool flag = false;
            //        for (int i = 0; i < LstRefDepartment.Count; i++)
            //        {
            //            if (LstRefDepartment[i] == CurrentRegistration.DeptID)
            //            {
            //                flag = true;
            //                break;
            //            }
            //        }
            //        if (!flag)
            //        {
            //            MessageBox.Show("Nhân viên này chưa được phân bố trách nhiệm cho khoa phòng này. \nLiên hệ người quản trị để nhận thêm thông tin.");
            //            return;
            //        }
            //        //InPatientTransferDeptReqsearch.InPatientAdmDisDetails=new InPatientAdmDisDetails();
            //        if (CurrentRegistration != null && CurrentRegistration.PtRegistrationID > 0)
            //        {
            //            InPatientTransferDeptReqsearch.PtRegistrationID = CurrentRegistration.PtRegistrationID;
            //            GetInPatientTransferDeptReq(InPatientTransferDeptReqsearch);
            //        }
            //    }

        }

        public void ResetHplnk()
        {
            InPatientTransferDeptReqsearch = new InPatientTransferDeptReq();
        }

        public void RefreshHplnk()
        {
            if (CurrentRegistration != null)
            {
                OpenRegistration(CurrentRegistration.PtRegistrationID);
            }
        }

        public void SelectBedAllocationCmd()
        {
            //20180715 TBL: CurrentRegistration bi null nen ct bi vang
            if (CurrentRegistration == null)
            {
                return;
            }
            //KMx: Hàm đặt giường này sửa lại chỉ được xem thôi, không được chỉnh sửa. Nếu muốn cho chỉnh sửa thì sử dụng lại hàm cũ (05/09/2014 17:55).
            if (Globals.isAccountCheck && (Globals.LoggedUserAccount.DeptIDResponsibilityList == null || Globals.LoggedUserAccount.DeptIDResponsibilityList.Count <= 0))
            {
                MessageBox.Show(eHCMSResources.Z0303_G1_ChuaPBoTrachNhiemKhPhg, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
            if (Globals.IsLockRegistration(CurrentRegistration.RegLockFlag, "xem và đặt giường"))
            {
                return;
            }
            Action<IBedPatientAlloc> onInitDlg = delegate (IBedPatientAlloc bedAllocVm)
            {
                bedAllocVm.IsShowPatientInfo = false;
                bedAllocVm.IsReadOnly = true;
                bedAllocVm.isLoadAllDept = true;
                bedAllocVm.DefaultDepartment = Globals.ObjRefDepartment;
                //bedAllocVm.LstRefDepartment = LstRefDepartment;
            };
            GlobalsNAV.ShowDialog<IBedPatientAlloc>(onInitDlg);
        }

        //public void SelectBedAllocationCmd()
        //{
        //    if (CurrentRegistration == null)
        //    {
        //        MessageBox.Show("Chưa chọn đăng ký. Không thể đặt giường");
        //        return;
        //    }
        //    var bedAllocVm = Globals.GetViewModel<IBedPatientAlloc>();

        //    bedAllocVm.curPatientRegistration = CurrentRegistration;
        //    if (CurrentRegistration.AdmissionInfo != null && CurrentRegistration.AdmissionInfo.InPatientAdmDisDetailID > 0)
        //    {
        //        //KMx: BookBedAllocOnly = true: Khi bấm đặt giường là lưu xuống Database luôn, ngược lại thì khi form cha lưu thì mới lưu giường sau (05/09/2014 17:11). 
        //        bedAllocVm.BookBedAllocOnly = false;//Dat giuong truc tiep luon.    
        //        bedAllocVm.isLoadAllDept = false;
        //        var items = CurrentRegistration.AdmissionInfo.InPatientDeptDetails;
        //        bedAllocVm.DefaultDepartment = InPatientAdmissionInfoContent.CurrentDepartment;
        //        bedAllocVm.ResponsibleDepartment = InPatientAdmissionInfoContent.CurrentDepartment;
        //        bedAllocVm.SelectedDeptLocation = InPatientAdmissionInfoContent.SelectedLocation;
        //    }
        //    else
        //    {
        //        bedAllocVm.BookBedAllocOnly = true;
        //        bedAllocVm.isLoadAllDept = true;
        //        if (InPatientAdmissionInfoContent.CurrentDepartment==null
        //            || InPatientAdmissionInfoContent.CurrentDepartment.DeptID<1)
        //        {
        //            MessageBox.Show(eHCMSResources.A0318_G1_Msg_InfoChonKhoaNpVienTruocKhiDatGiuong);
        //            return;
        //        }
        //        bedAllocVm.DefaultDepartment = InPatientAdmissionInfoContent.CurrentDepartment;
        //        bedAllocVm.ResponsibleDepartment = InPatientAdmissionInfoContent.CurrentDepartment;
        //    }
        //    Globals.ShowDialog(bedAllocVm as Conductor<object>);
        //}

        public void Handle(UpdateCompleted<InPatientAdmDisDetails> message)
        {
            if (GetView() == null || message.Item == null)
            {
                return;
            }

            if (CurrentRegistration != null && CurrentRegistration.PtRegistrationID == message.Item.PtRegistrationID)
            {
                OpenRegistration(message.Item.PtRegistrationID);
            }

        }

        public void Handle(AddCompleted<BedPatientAllocs> message)
        {
            if (GetView() != null && message.Item != null)
            {
                if (CurrentRegistration != null && CurrentRegistration.PtRegistrationID == message.Item.PtRegistrationID)
                {
                    OpenRegistration(message.Item.PtRegistrationID);
                }
            }
        }

        public void Handle(AddCompleted<InPatientDeptDetail> message)
        {
            if (this.GetView() != null && message.Item != null)
            {
                if (CurrentRegistration != null
                    && CurrentRegistration.AdmissionInfo != null
                    && CurrentRegistration.AdmissionInfo.InPatientAdmDisDetailID == message.Item.InPatientAdmDisDetailID)
                {
                    OpenRegistration(CurrentRegistration.PtRegistrationID);
                }
            }
        }

        public void Handle(RemoveItem<BedPatientAllocs, object> message)
        {
            if (this.GetView() != null && CurrentRegistration != null)
            {
                if (message.Item.BedPatientID <= 0)
                {
                    CurrentRegistration.BedAllocations.Remove(message.Item);
                }
                else
                {
                    if (MessageBox.Show(eHCMSResources.A0161_G1_Msg_ConfXoaGiuong, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                    {
                        RemoveBedPatientAllocs(message.Item.BedPatientID);
                    }
                }
            }
        }

        public void Handle(ReturnItem<BedPatientAllocs, object> message)
        {
            if (GetView() != null && CurrentRegistration != null)
            {
                if (message.Item.IsEditing)
                {
                }
                else
                {
                    if (message.Item.BedPatientID > 0 && message.Item.CheckOutDate == null)
                    {
                        if (MessageBox.Show(eHCMSResources.A0151_G1_Msg_ConfTraGiuong, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                        {
                            ReturnBedPatientAllocs(message.Item.BedPatientID);
                        }
                    }
                    else
                    {
                        MessageBox.Show(eHCMSResources.A0714_G1_Msg_InfoTraGiuongFail);
                    }
                }
            }
        }

        public void ReloadRegistration()
        {
            if (this.GetView() != null && CurrentRegistration != null)
            {
                if (CurrentRegistration != null)
                {
                    OpenRegistration(CurrentRegistration.PtRegistrationID);
                }
            }
        }

        public void Handle(ReturnItem<InPatientTransferDeptReq, object> message)
        {
            ReloadRegistration();
        }

        public void Handle(OutDepartmentSuccessEvent message)
        {
            ReloadRegistration();
        }

        public void AcceptDeptTranferClick(object sender)
        {
            if (Globals.IsLockRegistration(CurrentRegistration.RegLockFlag, "chấp nhận đề nghị chuyển khoa"))
            {
                return;
            }
            InPatientTransferDeptReq cur = sender as InPatientTransferDeptReq;
            if (cur != null)
            {
                void onInitDlg(IAcceptChangeDept vm)
                {
                    vm.CurrentRegistration = CurrentRegistration;
                    vm.CurInPatientTransferDeptReq = cur;
                    vm.LoadLocations(cur.ReqDeptID);
                }
                GlobalsNAV.ShowDialog<IAcceptChangeDept>(onInitDlg);
            }
        }

        public void DeletedTranferClick(object sender)
        {
            if (Globals.IsLockRegistration(CurrentRegistration.RegLockFlag, "xóa đề nghị chuyển khoa"))
            {
                return;
            }
            var elem = sender as FrameworkElement;
            if (elem != null && elem.DataContext != null)
            {
                InPatientTransferDeptReq cur = elem.DataContext as InPatientTransferDeptReq;
                if (cur != null)
                {
                    DeleteInPatientTransferDeptReq(cur);
                }
            }
        }

        public void DeleteInPatientTransferDeptReq(InPatientTransferDeptReq p)
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginDeleteInPatientTransferDeptReq(p.InPatientTransferDeptReqID,
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                allInPatientTransferDeptReq = new ObservableCollection<InPatientTransferDeptReq>();
                                try
                                {
                                    var res = contract.EndDeleteInPatientTransferDeptReq(asyncResult);
                                    if (res)
                                    {
                                        MessageBox.Show(eHCMSResources.K0537_G1_XoaOk);
                                        SearchCmd();
                                    }
                                }
                                catch (FaultException<AxException> fault)
                                {
                                    MessageBox.Show(fault.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                                    ClientLoggerHelper.LogInfo(fault.ToString());
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

        public void AcceptDeptTranferLoaded(object sender)
        {
            ((Button)sender).Visibility = Globals.convertVisibility(mNhapVien_ChapNhanChuyenKhoa);
        }

        public void InPatientDeptDetailsTranfer(InPatientDeptDetail p, long InPatientTransferDeptReqID)
        {
            var t = new Thread(() =>
            {
                long patientDeptDetailId;
                try
                {
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginInPatientDeptDetailsTranfer(p, InPatientTransferDeptReqID, false, Globals.DispatchCallback(asyncResult =>
                        {
                            try
                            {
                                bool bOK = contract.EndInPatientDeptDetailsTranfer(out patientDeptDetailId, asyncResult);
                                if (bOK)
                                {
                                    MessageBox.Show(eHCMSResources.A0431_G1_Msg_InfoChKhoaOK);
                                    isNhapKhoa = true;
                                    SearchCmd();
                                    if (CurrentRegistration != null)
                                    {
                                        OpenRegistration(CurrentRegistration.PtRegistrationID);
                                    }
                                }
                                else
                                {
                                    MessageBox.Show(eHCMSResources.A0432_G1_Msg_InfoChKhoaFail);
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
        private void ReturnBedPatientAllocs(long BedPatientID)
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                using (var serviceFactory = new BedAllocationsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginDeleteBedPatientAllocs(BedPatientID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndDeleteBedPatientAllocs(asyncResult);
                            if (results == true)
                            {
                                MessageBox.Show(eHCMSResources.K0220_G1_TraGiuongOk);
                                if (CurrentRegistration != null)
                                {
                                    OpenRegistration(CurrentRegistration.PtRegistrationID);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            Globals.IsBusy = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }

        private void RemoveBedPatientAllocs(long BedPatientID)
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                using (var serviceFactory = new BedAllocationsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginMarkDeleteBedPatientAlloc(BedPatientID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndMarkDeleteBedPatientAlloc(asyncResult);
                            if (results == true)
                            {
                                MessageBox.Show(eHCMSResources.K0475_G1_XoaGiuongOk);
                                if (CurrentRegistration != null)
                                {
                                    OpenRegistration(CurrentRegistration.PtRegistrationID);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            Globals.IsBusy = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }

        public void GetInPatientTransferDeptReq(InPatientTransferDeptReq p)
        {

            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginGetInPatientTransferDeptReq(p,
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                allInPatientTransferDeptReq = new ObservableCollection<InPatientTransferDeptReq>();
                                try
                                {
                                    var res = contract.EndGetInPatientTransferDeptReq(asyncResult);
                                    if (res != null && res.Count > 0)
                                    {
                                        foreach (var inPatientTransferDeptReq in res)
                                        {
                                            /*▼====: #001*/
                                            //if (!Globals.isAccountCheck || (Globals.LoggedUserAccount.DeptIDResponsibilityList.Any(x => x == inPatientTransferDeptReq.CurDept.DeptID)))
                                            //{
                                            //    allInPatientTransferDeptReq.Add(inPatientTransferDeptReq);
                                            //}
                                            if (!Globals.isAccountCheck)
                                            {
                                                inPatientTransferDeptReq.IsReceiveDept = true;
                                                inPatientTransferDeptReq.IsChangedDept = true;
                                            }
                                            if (Globals.LoggedUserAccount.DeptIDResponsibilityList.Any(x => x == inPatientTransferDeptReq.CurDept.DeptID))
                                            {
                                                inPatientTransferDeptReq.IsChangedDept = true;
                                            }
                                            if (Globals.LoggedUserAccount.DeptIDResponsibilityList.Any(x => x == inPatientTransferDeptReq.ReqDeptID))
                                            {
                                                inPatientTransferDeptReq.IsReceiveDept = true && mNhapVien_ChapNhanChuyenKhoa;
                                            }
                                            allInPatientTransferDeptReq.Add(inPatientTransferDeptReq);
                                            /*▲====: #001*/
                                        }
                                    }
                                    else
                                    {
                                        if (!isNhapKhoa)
                                        {
                                            MessageBox.Show(eHCMSResources.A0736_G1_Msg_InfoKhTimThayDNChKhoa);
                                        }
                                        isNhapKhoa = true;
                                    }
                                }
                                catch (FaultException<AxException> fault)
                                {
                                    ClientLoggerHelper.LogInfo(fault.ToString());
                                }

                            }), null);
                    }
                }
                catch (Exception ex)
                {
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                }
                finally
                {
                    Globals.IsBusy = false;
                }
            });
            t.Start();
        }

        public void authorization()
        {
            if (!Globals.isAccountCheck)
            {
                //InPatientAdmissionInfoContent = Globals.GetViewModel<IInPatientAdmissionInfo>();
                return;
            }
            CheckResponsibility();
            if (isAdmision)
            {
                mNhapVien_NhapVien = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPatient
                                               , (int)ePatient.mInPatientAdmission,
                                               (int)oRegistrionEx.mNhapVien_NhapVien, (int)ePermission.mView);
                mNhapVien_DatGiuong = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPatient
                                                   , (int)ePatient.mInPatientAdmission,
                                                   (int)oRegistrionEx.mNhapVien_DatGiuong, (int)ePermission.mView);
                mNhapVien_DeNghiChuyenKhoa = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPatient
                                                   , (int)ePatient.mInPatientAdmission,
                                                   (int)oRegistrionEx.mNhapVien_DeNghiChuyenKhoa, (int)ePermission.mView);
                mNhapVien_ChapNhanChuyenKhoa = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPatient
                                                   , (int)ePatient.mInPatientAdmission,
                                                   (int)oRegistrionEx.mNhapVien_ChapNhanChuyenKhoa, (int)ePermission.mView);

            }
            else
            {
                mNhapVien_DatGiuong = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPatient
                                                   , (int)ePatient.mInPatientAdmissionManage,
                                                   (int)oRegistrionEx.mQLyBNNoitTru_DatGiuong, (int)ePermission.mView);
                mNhapVien_DeNghiChuyenKhoa = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPatient
                                                   , (int)ePatient.mInPatientAdmissionManage,
                                                   (int)oRegistrionEx.mQLyBNNoitTru_DeNghiChuyenKhoa, (int)ePermission.mView);
                mNhapVien_ChapNhanChuyenKhoa = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPatient
                                                   , (int)ePatient.mInPatientAdmissionManage,
                                                   (int)oRegistrionEx.mQLyBNNoitTru_ChapNhanChuyenKhoa, (int)ePermission.mView);

            }


            //phan nay nam trong module chung ne
            mNhapVien_Patient_TimBN = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPatient
                                             , (int)ePatient.mRegister,
                                             (int)oRegistrionEx.mNhapVien_Patient_TimBN, (int)ePermission.mView);
            mNhapVien_Patient_ThemBN = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPatient
                                                 , (int)ePatient.mRegister,
                                                 (int)oRegistrionEx.mNhapVien_Patient_ThemBN, (int)ePermission.mView);
            mNhapVien_Patient_TimDangKy = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPatient
                                                 , (int)ePatient.mRegister,
                                                 (int)oRegistrionEx.mNhapVien_Patient_TimDangKy, (int)ePermission.mView);

            mNhapVien_Info_CapNhatThongTinBN = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPatient
                                                 , (int)ePatient.mRegister,
                                                 (int)oRegistrionEx.mNhapVien_Info_CapNhatThongTinBN, (int)ePermission.mView);
            mNhapVien_Info_XacNhan = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPatient
                                                 , (int)ePatient.mRegister,
                                                 (int)oRegistrionEx.mNhapVien_Info_XacNhan, (int)ePermission.mView);
            mNhapVien_Info_XoaThe = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPatient
                                                 , (int)ePatient.mRegister,
                                                 (int)oRegistrionEx.mNhapVien_Info_XoaThe, (int)ePermission.mView);
            mNhapVien_Info_XemPhongKham = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPatient
                                                 , (int)ePatient.mRegister,
                                                 (int)oRegistrionEx.mNhapVien_Info_XemPhongKham, (int)ePermission.mView);
            mNhapVien_TimDeNghiChuyenKhoa = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPatient
                                                     , (int)ePatient.mInPatientAdmissionManage,
                                                     (int)oRegistrionEx.mQLyBNNoitTru_DeNghiChuyenKhoa, (int)ePermission.mView);

        }
        #region checking account

        private bool _mNhapVien_NhapVien = true;
        private bool _mNhapVien_DatGiuong = true;
        private bool _mNhapVien_DeNghiChuyenKhoa = true;
        private bool _mNhapVien_TimDeNghiChuyenKhoa = true;
        private bool _mNhapVien_ChapNhanChuyenKhoa = true;
        public bool mNhapVien_NhapVien
        {
            get
            {
                return _mNhapVien_NhapVien;
            }
            set
            {
                if (_mNhapVien_NhapVien == value)
                    return;
                _mNhapVien_NhapVien = value;
                NotifyOfPropertyChange(() => mNhapVien_NhapVien);
            }
        }

        public bool mNhapVien_DatGiuong
        {
            get
            {
                return _mNhapVien_DatGiuong;
            }
            set
            {
                if (_mNhapVien_DatGiuong == value)
                    return;
                _mNhapVien_DatGiuong = value;
                NotifyOfPropertyChange(() => mNhapVien_DatGiuong);
            }
        }

        public bool mNhapVien_DeNghiChuyenKhoa
        {
            get
            {
                return _mNhapVien_DeNghiChuyenKhoa;
            }
            set
            {
                if (_mNhapVien_DeNghiChuyenKhoa == value)
                    return;
                _mNhapVien_DeNghiChuyenKhoa = value;
                NotifyOfPropertyChange(() => mNhapVien_DeNghiChuyenKhoa);
                NotifyOfPropertyChange(() => IsEnalbeInDeptChangeLocFucn);
                NotifyOfPropertyChange(() => IsEnalbeTempInDeptFuction);
            }
        }

        public bool mNhapVien_TimDeNghiChuyenKhoa
        {
            get
            {
                return _mNhapVien_TimDeNghiChuyenKhoa;
            }
            set
            {
                if (_mNhapVien_TimDeNghiChuyenKhoa == value)
                    return;
                _mNhapVien_TimDeNghiChuyenKhoa = value;
                NotifyOfPropertyChange(() => mNhapVien_TimDeNghiChuyenKhoa);
            }
        }

        public bool mNhapVien_ChapNhanChuyenKhoa
        {
            get
            {
                return _mNhapVien_ChapNhanChuyenKhoa;
            }
            set
            {
                if (_mNhapVien_ChapNhanChuyenKhoa == value)
                    return;
                _mNhapVien_ChapNhanChuyenKhoa = value;
                NotifyOfPropertyChange(() => mNhapVien_ChapNhanChuyenKhoa);
            }
        }

        //phan nay nam trong module chung
        private bool _mNhapVien_Patient_TimBN = true;
        private bool _mNhapVien_Patient_ThemBN = true;
        private bool _mNhapVien_Patient_TimDangKy = true;

        private bool _mNhapVien_Info_CapNhatThongTinBN = true;
        private bool _mNhapVien_Info_XacNhan = true;
        private bool _mNhapVien_Info_XoaThe = true;
        private bool _mNhapVien_Info_XemPhongKham = true;

        public bool mNhapVien_Patient_TimBN
        {
            get
            {
                return _mNhapVien_Patient_TimBN;
            }
            set
            {
                if (_mNhapVien_Patient_TimBN == value)
                    return;
                _mNhapVien_Patient_TimBN = value;
                NotifyOfPropertyChange(() => mNhapVien_Patient_TimBN);
            }
        }

        public bool mNhapVien_Patient_ThemBN
        {
            get
            {
                return _mNhapVien_Patient_ThemBN;
            }
            set
            {
                if (_mNhapVien_Patient_ThemBN == value)
                    return;
                _mNhapVien_Patient_ThemBN = value;
                NotifyOfPropertyChange(() => mNhapVien_Patient_ThemBN);
            }
        }

        public bool mNhapVien_Patient_TimDangKy
        {
            get
            {
                return _mNhapVien_Patient_TimDangKy;
            }
            set
            {
                if (_mNhapVien_Patient_TimDangKy == value)
                    return;
                _mNhapVien_Patient_TimDangKy = value;
                NotifyOfPropertyChange(() => mNhapVien_Patient_TimDangKy);
            }
        }

        public bool mNhapVien_Info_CapNhatThongTinBN
        {
            get
            {
                return _mNhapVien_Info_CapNhatThongTinBN;
            }
            set
            {
                if (_mNhapVien_Info_CapNhatThongTinBN == value)
                    return;
                _mNhapVien_Info_CapNhatThongTinBN = value;
                NotifyOfPropertyChange(() => mNhapVien_Info_CapNhatThongTinBN);
            }
        }

        public bool mNhapVien_Info_XacNhan
        {
            get
            {
                return _mNhapVien_Info_XacNhan;
            }
            set
            {
                if (_mNhapVien_Info_XacNhan == value)
                    return;
                _mNhapVien_Info_XacNhan = value;
                NotifyOfPropertyChange(() => mNhapVien_Info_XacNhan);
            }
        }

        public bool mNhapVien_Info_XoaThe
        {
            get
            {
                return _mNhapVien_Info_XoaThe;
            }
            set
            {
                if (_mNhapVien_Info_XoaThe == value)
                    return;
                _mNhapVien_Info_XoaThe = value;
                NotifyOfPropertyChange(() => mNhapVien_Info_XoaThe);
            }
        }

        public bool mNhapVien_Info_XemPhongKham
        {
            get
            {
                return _mNhapVien_Info_XemPhongKham;
            }
            set
            {
                if (_mNhapVien_Info_XemPhongKham == value)
                    return;
                _mNhapVien_Info_XemPhongKham = value;
                NotifyOfPropertyChange(() => mNhapVien_Info_XemPhongKham);
            }
        }
        #endregion


        #region Dinh them phan phan nay de kiem tra trach nhiem cua nhan vien

        private StaffDeptResponsibilities _curStaffDeptResponsibilities;
        public StaffDeptResponsibilities curStaffDeptResponsibilities
        {
            get
            {
                return _curStaffDeptResponsibilities;
            }
            set
            {
                if (_curStaffDeptResponsibilities == value)
                    return;
                _curStaffDeptResponsibilities = value;
                NotifyOfPropertyChange(() => curStaffDeptResponsibilities);

            }
        }

        private ObservableCollection<StaffDeptResponsibilities> _allStaffDeptResponsibilities;
        public ObservableCollection<StaffDeptResponsibilities> allStaffDeptResponsibilities
        {
            get
            {
                return _allStaffDeptResponsibilities;
            }
            set
            {
                if (_allStaffDeptResponsibilities == value)
                    return;
                _allStaffDeptResponsibilities = value;
                NotifyOfPropertyChange(() => allStaffDeptResponsibilities);
            }
        }


        public void CheckResponsibility()
        {
            //KMx: DeptIDResponsibilityList = Danh sách khoa mà nhân viên được cấu hình trách nhiệm (15/09/2014 10:47).
            if (Globals.LoggedUserAccount.DeptIDResponsibilityList != null && Globals.LoggedUserAccount.DeptIDResponsibilityList.Count > 0)
            {
                isChangeDept = !isDischarged;
            }
            else
            {
                MessageBox.Show(eHCMSResources.A0109_G1_Msg_InfoChuaCauHinhTNKhoaPg);
                isChangeDept = false;
                NotifyOfPropertyChange(() => isChangeDept);

            }
        }






        //public void CheckResponsibility()
        //{
        //    List<StaffDeptResponsibilities> results = Globals.LoggedUserAccount.AllStaffDeptResponsibilities;

        //    allStaffDeptResponsibilities = new ObservableCollection<StaffDeptResponsibilities>();
        //    LstRefDepartment = new ObservableCollection<long>();
        //    if (results != null && results.Count > 0)
        //    {
        //        isChangeDept = !isDischarged;
        //        foreach (var item in results)
        //        {
        //            allStaffDeptResponsibilities.Add(item);
        //            if (item.ResNhapVien)
        //            {
        //                LstRefDepartment.Add(item.DeptID);
        //            }
        //        }
        //        NotifyOfPropertyChange(() => allStaffDeptResponsibilities);
        //    }
        //    else
        //    {
        //        isChangeDept = false;
        //        NotifyOfPropertyChange(() => isChangeDept);

        //    }
        //    if (LstRefDepartment.Count < 1)
        //    {
        //        MessageBox.Show("Bạn chưa được phân công trách nhiệm với khoa phòng nào. " +
        //                            "\nLiên hệ với người quản trị để được phân bổ khoa phòng chịu trách nhiệm.");
        //    }

        //    //InPatientAdmissionInfoContent.LstRefDepartment = LstRefDepartment;
        //    //InPatientDeptListingContent.LstRefDepartment = LstRefDepartment;
        //    //PatientAllocListingContent.LstRefDepartment = LstRefDepartment;

        //    //NotifyOfPropertyChange(() => InPatientAdmissionInfoContent);
        //}

        #endregion

        public void Handle(AcceptChangeDeptViewModelEvent message)
        {
            if (this.IsActive && message != null)
            {
                if (CurrentRegistration != null)
                {
                    OpenRegistration(CurrentRegistration.PtRegistrationID);
                }
                SearchCmd();
            }
        }
        /*▼====: #001*/
        private bool _IsEnalbeInDeptChangeLocFucn = Globals.ServerConfigSection.CommonItems.IsEnalbeInDeptChangeLocFucn;
        public bool IsEnalbeInDeptChangeLocFucn
        {
            get
            {
                return _IsEnalbeInDeptChangeLocFucn && ShowChangeDeptCmd;
            }
            set
            {
                _IsEnalbeInDeptChangeLocFucn = value;
                NotifyOfPropertyChange(() => ShowChangeDeptCmd);
                NotifyOfPropertyChange(() => IsEnalbeInDeptChangeLocFucn);
            }
        }
        private bool _IsEnalbeTempInDeptFuction = Globals.ServerConfigSection.CommonItems.IsEnalbeTempInDeptFuction;
        public bool IsEnalbeTempInDeptFuction
        {
            get
            {
                return _IsEnalbeTempInDeptFuction && ShowChangeDeptCmd;
            }
            set
            {
                _IsEnalbeTempInDeptFuction = value;
                NotifyOfPropertyChange(() => ShowChangeDeptCmd);
                NotifyOfPropertyChange(() => IsEnalbeTempInDeptFuction);
            }
        }
        /*▲====: #001*/
        //▼===== #006
        public void MergerCmd()
        {
            if (MessageBox.Show(eHCMSResources.Z3038_G1_CanhBaoSapNhap, eHCMSResources.K1576_G1_CBao, MessageBoxButton.YesNo, MessageBoxImage.None, MessageBoxResult.No) == MessageBoxResult.No)
            {
                return;
            }
            if (CurrentRegistration == null)
            {
                MessageBox.Show(eHCMSResources.Z1793_G1_KgCoDK);
                return;
            }
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginMergerPatientRegistration(CurrentRegistration, Globals.LoggedUserAccount.Staff.StaffID, Globals.DispatchCallback(asyncResult =>
                        {
                            try
                            {
                                contract.EndMergerPatientRegistration(asyncResult);
                                MessageBox.Show(eHCMSResources.K2823_G1_DaThien);
                            }
                            catch (Exception ex)
                            {
                                ClientLoggerHelper.LogInfo(ex.ToString());
                                MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
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
                    MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                }
                finally
                {
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }
        //▲===== #006
        public void GetLatesDiagTrmtByPtID_InPt_OnlyForDia(long? InPtRegistrationID, long? V_DiagnosisType)
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                using (var serviceFactory = new ePMRsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetLatestDiagnosisTreatmentByPtID_InPt_ForDiag(InPtRegistrationID, V_DiagnosisType, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            DiagnosisTreatment temp = new DiagnosisTreatment();
                            temp = contract.EndGetLatestDiagnosisTreatmentByPtID_InPt_ForDiag(asyncResult);

                            if (temp != null && temp.DTItemID > 0)
                            {
                                InPatientAdmissionInfoContent.CurrentAdmission.AdmissionNote = temp.ReasonHospitalStay;
                                InPatientAdmissionInfoContent.CurrentAdmission.IsTreatmentCOVID = temp.IsTreatmentCOVID;
                                InPatientAdmissionInfoContent.IsAdmissionFromSuggestion = false;
                            }
                            else
                            {
                                InPatientAdmissionInfoContent.IsAdmissionFromSuggestion = true;
                            }
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            this.HideBusyIndicator();
                        }

                    }), null);
                }
            });
            t.Start();
        }
        public void btPTKVCK()
        {
            if (InPatientAdmissionInfoContent == null
                || InPatientAdmissionInfoContent.CurrentAdmission == null
                || InPatientAdmissionInfoContent.CurrentAdmission.PatientRegistration == null
                || InPatientAdmissionInfoContent.CurrentAdmission.PatientRegistration.PtRegistrationID == 0)
            {
                MessageBox.Show("Chưa có thông tin nhập viện", "Thông báo");
                return;
            }
            GlobalsNAV.ShowDialog<ISelfDeclaration>((VM) =>
            {
                VM.PtRegistrationID = Convert.ToInt64(InPatientAdmissionInfoContent.CurrentAdmission.PatientRegistration.PtRegistrationID);
                VM.PatientID = Convert.ToInt64(InPatientAdmissionInfoContent.CurrentAdmission.PatientRegistration.PatientID);
                VM.V_RegistrationType = Convert.ToInt64(AllLookupValues.RegistrationType.NOI_TRU);
                VM.GetSelfDeclarationByPtRegistrationID();
            }, null, false, true, new Size(1200, 700));
        }
        public void btnDeNghiTamUng()
        {
            if (InPatientAdmissionInfoContent == null
               || InPatientAdmissionInfoContent.CurrentAdmission == null
               
               || InPatientAdmissionInfoContent.CurrentAdmission.PtRegistrationID == 0)
            {
                MessageBox.Show("Chưa có thông tin nhập viện", "Thông báo");
                return;
            }
            Globals.PageName = Globals.TitleForm;
            //LeftMenuByPTType = eLeftMenuByPTType.IN_PT;
            var regModule = Globals.GetViewModel<IRegistrationModule>();
            var vm = Globals.GetViewModel<ISuggestCashAdvance>();
            Globals.IsAdmission = true;
            vm.DeptLocTitle = Globals.ObjRefDepartment.DeptName;
            vm.UsedByTaiVuOffice = true;
            vm.PatientFindBy = AllLookupValues.PatientFindBy.NOITRU;
            vm.SearchRegistrationContent.IsSearchForCashAdvance = true;
            regModule.MainContent = vm;
            ((Conductor<object>) regModule).ActivateItem(vm);
            Globals.PatientFindBy_ForConsultation = AllLookupValues.PatientFindBy.NOITRU;

            Globals.EventAggregator.Publish(new PatientSelectedGoToKhamBenh_InPt<PatientRegistration>()
                { Item = new PatientRegistration { PtRegistrationID = InPatientAdmissionInfoContent.CurrentAdmission.PtRegistrationID } });
        }
    }
}