using System;
using System.Windows;
using System.Windows.Controls;
using Caliburn.Micro;
using System.ComponentModel.Composition;
using aEMR.ViewContracts;
using DataEntities;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using aEMR.Infrastructure.Events;
using aEMR.Infrastructure;
using aEMR.Common;
using eHCMSLanguage;
using System.Threading;
using aEMR.ServiceClient;
using aEMR.CommonTasks;
using System.Linq;
using System.Windows.Media;
using System.Windows.Input;
using Castle.Windsor;

namespace aEMR.ConsultantEPrescription.ViewModels
{
    [Export(typeof(ISurgeryBooking)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class SurgeryBookingViewModel : Conductor<object>, ISurgeryBooking
        , IHandle<RegistrationSelectedForConsultation_K2>
        , IHandle<RoomSelectedEvent>
        , IHandle<RegistrationSelectedForConsultation_K1>
    {
        public int NumberOfCases
        {
            get
            {
                return gSurgeriesArray.Count;
            }
            set
            {
                NotifyOfPropertyChange(() => NumberOfCases);
            }
        }
        private bool SurgeryLoaded = false;
        private void LoadSurgeries()
        {
            if (!SurgeryLoaded)
            {
                if (gSurgeriesArray == null)
                    gSurgeriesArray = new ObservableCollection<Surgery>();
                gSurgeriesArray.Add(new Surgery { ExamDate = selectedConsultationRoomStaffAllocations.AllocationDate, TimeSegment = "Ca 1", Patient = "HUỲNH KIM TÙNG", ExamBy = "BS.Nguyễn Thị Mỹ Hạnh, BS. Hà Quốc Thanh, BS. Nguyễn Thị Ngọc Oanh, ĐD. Lê Thị Cẩm Thúy, ĐD. Trần Thị Lê Tuyết" });
                gSurgeriesArray.Add(new Surgery { ExamDate = selectedConsultationRoomStaffAllocations.AllocationDate, TimeSegment = "Ca 3", Patient = "LỮ THỊ BA", ExamBy = "BS. Trần Thị Tuyết Lan, BS. Trần Phi Quốc, ĐD. Nguyễn Thị Chi, ĐD. Đặng Thanh Diễm" });
                gSurgeriesArray.Add(new Surgery { ExamDate = selectedConsultationRoomStaffAllocations.AllocationDate, TimeSegment = "Ca 4", Patient = "NGUYỄN THỊ QUYÊN", ExamBy = "BS. Hồ Thanh Tuấn, ĐD. Phạm Thị Tuyết, ĐD. Lê Thị Hồng Nga" });
                gSurgeriesArray.Add(new Surgery { ExamDate = selectedConsultationRoomStaffAllocations.AllocationDate, TimeSegment = "Ca 1", Patient = "HÀ ĐỨC HIẾU", ExamBy = "BS. Phạm Thị Mai Hòa, BS. Nguyễn Thanh Tân, ĐD. Lê Phạm Minh Châu, ĐD. Nguyễn Thị Thu Oanh" });
                SurgeryLoaded = true;
            }
            NotifyOfPropertyChange(() => NumberOfCases);
        }
        private ObservableCollection<Surgery> _gSurgeriesArray = new ObservableCollection<Surgery>();
        public ObservableCollection<Surgery> gSurgeriesArray
        {
            get
            {
                return _gSurgeriesArray;
            }
            set
            {
                if (_gSurgeriesArray == value)
                    return;
                _gSurgeriesArray = value;
                NotifyOfPropertyChange(() => gSurgeriesArray);
            }
        }
        public void Handle(RegistrationSelectedForConsultation_K2 message)
        {
            if (this.GetView() != null && message != null && message.Source != null)
            {
                OpenRegistration(message.Source.PtRegistrationID);
            }
        }
        public void Handle(RegistrationSelectedForConsultation_K1 message)
        {
            if (this.GetView() != null && message != null && message.Source != null)
            {
                OpenRegistration(message.Source.PtRegistrationID);
            }
        }
        public void OpenRegistration(long regID)
        {
            RegistrationLoading = true;
            Coroutine.BeginExecute(DoOpenRegistration(regID), null, (o, e) => { RegistrationLoading = false; });
        }
        public IEnumerator<IResult> DoOpenRegistration(long regID)
        {
            LoadRegistrationSwitch LoadRegisSwitch = new LoadRegistrationSwitch();
            LoadRegisSwitch.IsGetAdmissionInfo = true;
            LoadRegisSwitch.IsGetBillingInvoices = true;
            var loadRegTask = new LoadRegistrationInfo_InPtTask(regID, (int)AllLookupValues.V_FindPatientType.NOI_TRU, LoadRegisSwitch);
            yield return loadRegTask;
            if (loadRegTask.Registration == null)
            {
                Globals.EventAggregator.Publish(new ShowMessageEvent { Message = "Error!(7)" });
            }
            else
            {
                CurRegistration = loadRegTask.Registration;
                PatientSummaryInfoContent.CurrentPatient = CurRegistration.Patient;
                ShowOldRegistration(CurRegistration);
            }
        }
        private void ShowOldRegistration(PatientRegistration regInfo)
        {
            IsLoadNoBill = false;
            if (regInfo.V_RegistrationType != AllLookupValues.RegistrationType.NOI_TRU)
            {
                MessageBox.Show(eHCMSResources.Z0085_G1_DayKhongPhaiDKNoiTru);
                return;
            }
            CurRegistration = regInfo;

            CurrentRegMode = RegistrationFormMode.OLD_REGISTRATION_OPENED;
            InitRegistration();

            if (PatientSummaryInfoContent != null)
            {                
                PatientSummaryInfoContent.CurrentPatient = CurPatient;

                PatientSummaryInfoContent.SetPatientHISumInfo(regInfo.PtHISumInfo);
            }
            //if (CurRegistration.PatientClassification == null && CurRegistration.PatientClassID.HasValue)
            //{
            //    CurClassification = PatientClassification.CreatePatientClassification(CurRegistration.PatientClassID.Value, "");
            //}
            //else
            //{
            //    CurClassification = CurRegistration.PatientClassification;
            //}
            //PatientSummaryInfoContent.CurrentPatientClassification = CreateDefaultClassification();
            //SetConditionWhenChangeSelectedItem();
            //Validate_RegistrationInfo(regInfo);
        }
        private void InitRegistration()
        {
            _curPatient = CurRegistration.Patient;
            NotifyOfPropertyChange(() => CurPatient);
        }
        private Patient _curPatient;
        public Patient CurPatient
        {
            get
            {
                return _curPatient;
            }
            set
            {
                _curPatient = value;
                NotifyOfPropertyChange(() => CurPatient);
            }
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
                }
            }
        }
        private bool IsLoadNoBill = false;
        private bool _registrationLoading = false;
        public bool RegistrationLoading
        {
            get
            {
                return _registrationLoading;
            }
            set
            {
                _registrationLoading = value;
                NotifyOfPropertyChange(() => RegistrationLoading);

                //NotifyWhenBusy();
            }
        }
        private PatientRegistration _curRegistration;
        public PatientRegistration CurRegistration
        {
            get
            {
                return _curRegistration;
            }
            set
            {
                if (_curRegistration != value)
                {
                    _curRegistration = value;
                    NotifyOfPropertyChange(() => CurRegistration);
                    PatientSummaryInfoContent.CurrentPatientRegistration = CurRegistration;
                }
            }
        }
        private bool _isLoading;
        public bool IsLoading
        {
            get { return _isLoading; }
            set
            {
                if (_isLoading != value)
                {
                    _isLoading = value;
                    NotifyOfPropertyChange(() => IsLoading);
                }
            }
        }
        private long StaffCatType = (long)V_StaffCatType.BacSi;
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
        IEventAggregator _eventArg;
        [ImportingConstructor]
        public SurgeryBookingViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg)
        {
            _eventArg = eventArg;
            var treeDept = Globals.GetViewModel<IRoomTree>();
            GetAllConsultationTimeSegments();
            treeDept.DeptOnly = true;
            RoomTree = treeDept;
            this.ActivateItem(treeDept);

            SearchRegistrationContent = Globals.GetViewModel<ISearchPatientAndRegistration>();
            SearchRegistrationContent.InitButtonVisibility(SearchRegButtonsVisibility.SHOW_SEARCH_REG_BTN);
            SearchRegistrationContent.SetDefaultButton(SearchRegistrationButtons.SEARCH_REGISTRATION);
            SearchRegistrationContent.PatientFindByVisibility = false;
            SearchRegistrationContent.PatientFindBy = AllLookupValues.PatientFindBy.NOITRU;
            Globals.PatientFindBy_ForConsultation = AllLookupValues.PatientFindBy.NOITRU;
            SearchRegistrationContent.mTimBN = true;
            SearchRegistrationContent.mThemBN = true;
            SearchRegistrationContent.mTimDangKy = true;
            SearchRegistrationContent.SearchAdmittedInPtRegOnly = true;
            PatientSummaryInfoContent = Globals.GetViewModel<IPatientSummaryInfoV2>();
            PatientSummaryInfoContent.mInfo_CapNhatThongTinBN = false;
            PatientSummaryInfoContent.mInfo_XacNhan = false;
            PatientSummaryInfoContent.mInfo_XoaThe = false;
            PatientSummaryInfoContent.mInfo_XemPhongKham = false;
            PatientSummaryInfoContent.DisplayButtons = false;

            //GetRefStaffCategories();
            selectedConsultationRoomTarget = new ConsultationRoomTarget();
            _tempAllStaff = new ObservableCollection<Staff>();
            _memAllStaff = new ObservableCollection<Staff>();
            _allConsultationRoomStaffAllocations = new ObservableCollection<ConsultationRoomStaffAllocations>();
            selectedConsultationRoomStaffAllocations = new ConsultationRoomStaffAllocations();
            selectedTempConsultRoomStaffAlloc = new ConsultationRoomStaffAllocations();
            CurDateTime = selectedConsultationRoomStaffAllocations.CurDate;
            //chon radio bac si dau tien
            _allRefStaffCategory = new ObservableCollection<RefStaffCategory>();
            selectedConsultTimeSeg = new ConsultationTimeSegments();
            _allRefStaffCategory.Clear();
            GetRefStaffCategoriesByType((long)V_StaffCatType.BacSi);

            authorization();
        }
        protected override void OnActivate()
        {
            base.OnActivate();
            Globals.EventAggregator.Subscribe(this);
        }
        public DateTime CurDateTime = DateTime.Now;

        private string _curDate = DateTime.Now.Date.ToShortDateString();
        public string curDate
        {
            get
            {
                return _curDate;
            }
            set
            {
                if (_curDate == value)
                    return;
                _curDate = value;
            }
        }

        private DateTime _appDate;
        public DateTime appDate
        {
            get
            {
                return _appDate;
            }
            set
            {
                if (_appDate == value)
                    return;
                _appDate = value;
            }
        }

        public object RoomTree { get; set; }

        private DeptLocation _curDeptLocation;
        public DeptLocation curDeptLocation
        {
            get
            {
                return _curDeptLocation;
            }
            set
            {
                if (_curDeptLocation == value)
                    return;
                _curDeptLocation = value;
            }
        }

        private RefDepartmentsTree _CurRefDepartmentsTree;
        public RefDepartmentsTree CurRefDepartmentsTree
        {
            get { return _CurRefDepartmentsTree; }
            set
            {
                _CurRefDepartmentsTree = value;
                NotifyOfPropertyChange(() => CurRefDepartmentsTree);
            }

        }
        #region properties
        private ObservableCollection<RefStaffCategory> _allRefStaffCategory;
        public ObservableCollection<RefStaffCategory> allRefStaffCategory
        {
            get
            {
                return _allRefStaffCategory;
            }
            set
            {
                if (_allRefStaffCategory == value)
                    return;
                _allRefStaffCategory = value;
                NotifyOfPropertyChange(() => allRefStaffCategory);
            }
        }

        private RefStaffCategory _SelectedRefStaffCategory;
        public RefStaffCategory SelectedRefStaffCategory
        {
            get
            {
                return _SelectedRefStaffCategory;
            }
            set
            {
                if (_SelectedRefStaffCategory == value)
                    return;
                _SelectedRefStaffCategory = value;
                NotifyOfPropertyChange(() => SelectedRefStaffCategory);
                if (SelectedRefStaffCategory != null)
                {
                    GetAllStaff(SelectedRefStaffCategory.StaffCatgID);
                }

            }
        }

        private ObservableCollection<Staff> _allStaff;
        public ObservableCollection<Staff> allStaff
        {
            get
            {
                return _allStaff;
            }
            set
            {
                if (_allStaff == value)
                    return;
                _allStaff = value;
                NotifyOfPropertyChange(() => allStaff);
            }
        }

        private Staff _SelectedStaff;
        public Staff SelectedStaff
        {
            get
            {
                return _SelectedStaff;
            }
            set
            {
                if (_SelectedStaff == value)
                    return;
                _SelectedStaff = value;
            }
        }

        private Staff _SelectedStaffGrid;
        public Staff SelectedStaffGrid
        {
            get
            {
                return _SelectedStaffGrid;
            }
            set
            {
                if (_SelectedStaffGrid == value)
                    return;
                _SelectedStaffGrid = value;
            }
        }

        private ObservableCollection<Staff> _tempAllStaff;
        public ObservableCollection<Staff> tempAllStaff
        {
            get
            {
                return _tempAllStaff;
            }
            set
            {
                if (_tempAllStaff == value)
                    return;
                _tempAllStaff = value;
                NotifyOfPropertyChange(() => tempAllStaff);
            }
        }

        private ObservableCollection<Staff> _memAllStaff;
        public ObservableCollection<Staff> memAllStaff
        {
            get
            {
                return _memAllStaff;
            }
            set
            {
                if (_memAllStaff == value)
                    return;
                _memAllStaff = value;
                NotifyOfPropertyChange(() => memAllStaff);
            }
        }

        private ObservableCollection<Staff> _importStaff;
        public ObservableCollection<Staff> importStaff
        {
            get
            {
                return _importStaff;
            }
            set
            {
                if (_importStaff == value)
                    return;
                _importStaff = value;
            }
        }

        private ObservableCollection<ConsultationRoomStaffAllocations> _exportStaff;
        public ObservableCollection<ConsultationRoomStaffAllocations> exportStaff
        {
            get
            {
                return _exportStaff;
            }
            set
            {
                if (_exportStaff == value)
                    return;
                _exportStaff = value;
            }
        }

        private ObservableCollection<ConsultationRoomStaffAllocations> _tempConsRoomStaffAlloc;
        public ObservableCollection<ConsultationRoomStaffAllocations> tempConsRoomStaffAlloc
        {
            get
            {
                return _tempConsRoomStaffAlloc;
            }
            set
            {
                if (_tempConsRoomStaffAlloc == value)
                    return;
                _tempConsRoomStaffAlloc = value;
            }
        }

        private ConsultationTimeSegments _selectedConsultTimeSeg;
        public ConsultationTimeSegments selectedConsultTimeSeg
        {
            get
            {
                return _selectedConsultTimeSeg;
            }
            set
            {
                if (_selectedConsultTimeSeg == value)
                    return;
                _selectedConsultTimeSeg = value;
                NotifyOfPropertyChange(() => selectedConsultTimeSeg);
                if (selectedConsultTimeSeg.ConsultationTimeSegmentID > 0)
                {
                    if (allConsulRoomStaffAlloc != null && allConsulRoomStaffAlloc.Count > 0)
                    {
                        tempConsRoomStaffAlloc = new ObservableCollection<ConsultationRoomStaffAllocations>();

                        foreach (var crt in allConsulRoomStaffAlloc)
                        {
                            if (crt.ConsultationTimeSegmentID == selectedConsultTimeSeg.ConsultationTimeSegmentID
                                && crt.Staff.RefStaffCategory.V_StaffCatType == StaffCatType)
                            {
                                tempConsRoomStaffAlloc.Add(crt);
                            }
                        }
                    }
                }
                NotifyOfPropertyChange(() => tempConsRoomStaffAlloc);
            }
        }

        private ObservableCollection<ConsultationTimeSegments> _lstConsultationTimeSegments;
        public ObservableCollection<ConsultationTimeSegments> lstConsultationTimeSegments
        {
            get
            {
                return _lstConsultationTimeSegments;
            }
            set
            {
                if (_lstConsultationTimeSegments == value)
                    return;
                _lstConsultationTimeSegments = value;
                NotifyOfPropertyChange(() => lstConsultationTimeSegments);
            }
        }


        private ConsultationRoomStaffAllocations _selectedConsultationRoomStaffAllocations;
        public ConsultationRoomStaffAllocations selectedConsultationRoomStaffAllocations
        {
            get
            {
                return _selectedConsultationRoomStaffAllocations;
            }
            set
            {
                if (_selectedConsultationRoomStaffAllocations == value)
                    return;
                _selectedConsultationRoomStaffAllocations = value;
                NotifyOfPropertyChange(() => selectedConsultationRoomStaffAllocations);
            }
        }

        private ConsultationRoomStaffAllocations _selectedTempConsultRoomStaffAlloc;
        public ConsultationRoomStaffAllocations selectedTempConsultRoomStaffAlloc
        {
            get
            {
                return _selectedTempConsultRoomStaffAlloc;
            }
            set
            {
                if (_selectedTempConsultRoomStaffAlloc == value)
                    return;
                _selectedTempConsultRoomStaffAlloc = value;
                NotifyOfPropertyChange(() => selectedTempConsultRoomStaffAlloc);
            }
        }

        private ObservableCollection<ConsultationRoomStaffAllocations> _allConsultationRoomStaffAllocations;
        public ObservableCollection<ConsultationRoomStaffAllocations> allConsultationRoomStaffAllocations
        {
            get
            {
                return _allConsultationRoomStaffAllocations;
            }
            set
            {
                if (_allConsultationRoomStaffAllocations == value)
                    return;
                _allConsultationRoomStaffAllocations = value;
                NotifyOfPropertyChange(() => allConsultationRoomStaffAllocations);
            }
        }

        private ObservableCollection<ConsultationRoomStaffAllocations> _allConsulRoomStaffAlloc;
        public ObservableCollection<ConsultationRoomStaffAllocations> allConsulRoomStaffAlloc
        {
            get
            {
                return _allConsulRoomStaffAlloc;
            }
            set
            {
                if (_allConsulRoomStaffAlloc == value)
                    return;
                _allConsulRoomStaffAlloc = value;
                NotifyOfPropertyChange(() => allConsulRoomStaffAlloc);
                GetCurRoomStaffAllocations();
                NotifyOfPropertyChange(() => curAllConsulRoomStaffAlloc);
            }
        }
        private ObservableCollection<ConsultationRoomStaffAllocations> _curAllConsulRoomStaffAlloc;
        public ObservableCollection<ConsultationRoomStaffAllocations> curAllConsulRoomStaffAlloc
        {
            get
            {
                return _curAllConsulRoomStaffAlloc;
            }
            set
            {
                if (_curAllConsulRoomStaffAlloc == value)
                    return;
                _curAllConsulRoomStaffAlloc = value;
                NotifyOfPropertyChange(() => curAllConsulRoomStaffAlloc);

            }
        }
        public void GetCurRoomStaffAllocations()
        {
            curAllConsulRoomStaffAlloc = new ObservableCollection<ConsultationRoomStaffAllocations>();
            foreach (var cts in lstConsultationTimeSegments)
            {
                if (cts.ConsultationTimeSegmentID < 1)
                    continue;
                bool flag = false;
                if (allConsulRoomStaffAlloc == null ||
                    allConsulRoomStaffAlloc.Count < 1)
                {
                    return;
                }
                foreach (var crt in allConsulRoomStaffAlloc)
                {
                    if (crt.ConsultationTimeSegmentID == cts.ConsultationTimeSegmentID
                        && crt.AllocationDate.Date <= CurDateTime.Date
                        && crt.Staff.RefStaffCategory.V_StaffCatType == StaffCatType)
                    {
                        crt.Status = eHCMSResources.G2355_G1_X.ToUpper();
                        flag = true;
                        curAllConsulRoomStaffAlloc.Add(ObjectCopier.DeepCopy(crt));
                        break;
                    }
                }
                if (!flag)
                {
                    ConsultationRoomStaffAllocations crt = new ConsultationRoomStaffAllocations();
                    crt.ConsultationTimeSegments = new ConsultationTimeSegments();
                    crt.ConsultationTimeSegments.SegmentName = cts.SegmentName;
                    curAllConsulRoomStaffAlloc.Add(ObjectCopier.DeepCopy(crt));
                }
            }
            NotifyOfPropertyChange(() => curAllConsulRoomStaffAlloc);
        }


        public ObservableCollection<ConsultationRoomStaffAllocations> GetCurRoomStaff(ObservableCollection<ConsultationRoomStaffAllocations> allocationses)
        {
            if (allocationses.Count < 1)
                return null;
            ObservableCollection<ConsultationRoomStaffAllocations> curAllConsul = new ObservableCollection<ConsultationRoomStaffAllocations>();
            int i = 0;
            while (i < allocationses.Count)
            {
                int j = i + 1;
                ConsultationRoomStaffAllocations temp = allocationses[i];
                temp.StaffList = temp.Staff.FullName;
                while (j < allocationses.Count)
                {
                    if (allocationses[i].ConsultationTimeSegmentID == allocationses[j].ConsultationTimeSegmentID
                        && allocationses[i].AllocationDate.Date == allocationses[j].AllocationDate.Date
                        && allocationses[i].Staff.RefStaffCategory.V_StaffCatType == allocationses[j].Staff.RefStaffCategory.V_StaffCatType)
                    {
                        temp.StaffList += ", " + allocationses[j].Staff.FullName;
                        allocationses.RemoveAt(j);
                    }
                    else
                    {
                        j++;
                    }
                }
                curAllConsul.Add(temp);
                i++;
            }

            return curAllConsul;
        }

        private ObservableCollection<ConsultationRoomTarget> _allConsultationRoomTarget;
        public ObservableCollection<ConsultationRoomTarget> allConsultationRoomTarget
        {
            get
            {
                return _allConsultationRoomTarget;
            }
            set
            {
                if (_allConsultationRoomTarget == value)
                    return;
                _allConsultationRoomTarget = value;
                NotifyOfPropertyChange(() => allConsultationRoomTarget);
                //Kiem tra chi tieu hien tai
                GetCurTimeSegment();
                NotifyOfPropertyChange(() => curAllConsultationRoomTarget);
            }
        }

        private ObservableCollection<ConsultationRoomTarget> _curAllConsultationRoomTarget;
        public ObservableCollection<ConsultationRoomTarget> curAllConsultationRoomTarget
        {
            get
            {
                return _curAllConsultationRoomTarget;
            }
            set
            {
                if (_curAllConsultationRoomTarget == value)
                    return;
                _curAllConsultationRoomTarget = value;
                NotifyOfPropertyChange(() => curAllConsultationRoomTarget);
            }
        }
        public void GetCurTimeSegment()
        {

        }

        private ConsultationRoomTarget _selectedConsultationRoomTarget;
        public ConsultationRoomTarget selectedConsultationRoomTarget
        {
            get
            {
                return _selectedConsultationRoomTarget;
            }
            set
            {
                if (_selectedConsultationRoomTarget == value)
                    return;
                _selectedConsultationRoomTarget = value;
                NotifyOfPropertyChange(() => selectedConsultationRoomTarget);

            }
        }
        #endregion

        private bool isDoctor = true;
        //---add loai nhan vien vao combobox
        public void GetRefStaffCategories()
        {
            allRefStaffCategory = new ObservableCollection<RefStaffCategory>();
            GetRefStaffCategoriesByType((long)V_StaffCatType.BacSi);
            GetRefStaffCategoriesByType((long)V_StaffCatType.PhuTa);
        }
        public void radBacSi_Click(object sender, RoutedEventArgs e)
        {
            StaffCatType = (long)V_StaffCatType.BacSi;
            allRefStaffCategory = new ObservableCollection<RefStaffCategory>();
            allRefStaffCategory.Clear();
            GetRefStaffCategoriesByType(StaffCatType);
            isDoctor = true;
            //tempAllStaff = new ObservableCollection<Staff>();
            memAllStaff = new ObservableCollection<Staff>();
            allStaff = new ObservableCollection<Staff>();
            tempConsRoomStaffAlloc = GetRoomStaffAlloByType(StaffCatType);
            NotifyOfPropertyChange(() => tempConsRoomStaffAlloc);
            GetCurRoomStaffAllocations();
            NotifyOfPropertyChange(() => curAllConsulRoomStaffAlloc);
        }
        public void radTroLy_Click(object sender, RoutedEventArgs e)
        {
            StaffCatType = (long)V_StaffCatType.PhuTa;
            allRefStaffCategory = new ObservableCollection<RefStaffCategory>();
            allRefStaffCategory.Clear();
            GetRefStaffCategoriesByType(StaffCatType);
            isDoctor = false;
            allStaff = new ObservableCollection<Staff>();
            //tempAllStaff = new ObservableCollection<Staff>();
            memAllStaff = new ObservableCollection<Staff>();
            tempConsRoomStaffAlloc = GetRoomStaffAlloByType(StaffCatType);
            NotifyOfPropertyChange(() => tempConsRoomStaffAlloc);
            GetCurRoomStaffAllocations();
            NotifyOfPropertyChange(() => curAllConsulRoomStaffAlloc);
        }
        public ObservableCollection<ConsultationRoomStaffAllocations> GetRoomStaffAlloByType(long StaffCatType)
        {
            ObservableCollection<ConsultationRoomStaffAllocations> tempConsRoom = new ObservableCollection<ConsultationRoomStaffAllocations>();
            if (allConsulRoomStaffAlloc != null)
            {
                foreach (var RoomStaffAllocation in allConsulRoomStaffAlloc)
                {
                    if (RoomStaffAllocation.Staff.RefStaffCategory.V_StaffCatType == StaffCatType)
                    {
                        //tempAllStaff.Add(RoomStaffAllocation.Staff);
                        //memAllStaff.Add(RoomStaffAllocation.Staff);
                        tempConsRoom.Add(RoomStaffAllocation);
                    }
                }
            }
            return tempConsRoom;
        }
        public bool CheckValidRoomStaBff(ConsultationRoomStaffAllocations selectedConsultationRoomStaffAllocations)
        {
            if (selectedConsultationRoomStaffAllocations.ConsultationTimeSegments == null
                || selectedConsultationRoomStaffAllocations.ConsultationTimeSegments.ConsultationTimeSegmentID < 1)
            {
                Globals.ShowMessage(eHCMSResources.Z1769_G1_ChuaChonCaKham, "");
                return false;
            }
            if (CurRefDepartmentsTree == null || CurRefDepartmentsTree.Parent == null || CurRefDepartmentsTree.Parent.NodeID < 1)
            {
                Globals.ShowMessage(eHCMSResources.Z1770_G1_ChuaChonPK, "");
                return false;
            }
            if (selectedConsultationRoomStaffAllocations.AllocationDate == null)
            {
                Globals.ShowMessage(eHCMSResources.Z1771_G1_ChuaChonTGianPBo, "");
                return false;
            }

            return true;
        }

        public void butGetAll()
        {
            //tempConsRoomStaffAlloc = new ObservableCollection<ConsultationRoomStaffAllocations>();
            //tempConsRoomStaffAlloc = GetRoomStaffAlloByType(StaffCatType);
            //NotifyOfPropertyChange(() => tempConsRoomStaffAlloc);
            LoadSurgeries();
            NotifyOfPropertyChange(() => gSurgeriesArray);
        }

        public void cboTimeSegment_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GetCurTimeSegmentChoice();
        }
        public void GetCurTimeSegmentChoice()
        {
            if (selectedConsultTimeSeg.ConsultationTimeSegmentID > 0)
            {
                if (allConsulRoomStaffAlloc != null && allConsulRoomStaffAlloc.Count > 0)
                {
                    tempConsRoomStaffAlloc = new ObservableCollection<ConsultationRoomStaffAllocations>();

                    foreach (var crt in allConsulRoomStaffAlloc)
                    {
                        if (crt.ConsultationTimeSegmentID == selectedConsultTimeSeg.ConsultationTimeSegmentID
                            && crt.Staff.RefStaffCategory.V_StaffCatType == StaffCatType)
                        {
                            tempConsRoomStaffAlloc.Add(crt);
                        }
                    }
                    NotifyOfPropertyChange(() => tempConsRoomStaffAlloc);
                }
            }
        }

        public void grdListStaffDoubleClick(object sender)
        {
            if (!checkExist(SelectedStaff))
            {
                return;
            }
            tempAllStaff.Add(SelectedStaff);
        }
        public void butLuu()
        {
            if (!CheckValidRoomStaBff(selectedConsultationRoomStaffAllocations))
            {
                return;
            }
            if (CurPatient != null)
            {
                LoadSurgeries();
                gSurgeriesArray.Add(new Surgery { ExamDate = selectedConsultationRoomStaffAllocations.AllocationDate, TimeSegment = selectedConsultationRoomStaffAllocations.ConsultationTimeSegments.SegmentName, Patient = CurPatient.FullName, ExamBy = string.Join(", ", tempAllStaff.OrderBy(x => x.SFirstName).ThenBy(x => x.SLastName).Select(x => x.FullName).ToArray()) });
                NotifyOfPropertyChange(() => gSurgeriesArray);
            }
            else
            {
                MessageBox.Show("Vui lòng chọn đăng ký cần lập ca phẫu thuật!", eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
            }
        }
        public void butReset()
        {
            tempAllStaff.Clear();
            tempAllStaff = ObjectCopier.DeepCopy(memAllStaff);
            selectedConsultationRoomStaffAllocations = new ConsultationRoomStaffAllocations();
            selectedConsultationRoomStaffAllocations.DeptLocationID = CurRefDepartmentsTree.NodeID;
        }

        private DatePicker dtTargetDay { get; set; }
        public void dtTargetDay_OnLoaded(object sender, RoutedEventArgs e)
        {
            dtTargetDay = sender as DatePicker;
        }

        public void DoubleClick(object sender)
        {
            if (selectedTempConsultRoomStaffAlloc.isEdit)
            {
                appDate = selectedTempConsultRoomStaffAlloc.AllocationDate;
                selectedConsultationRoomStaffAllocations = selectedTempConsultRoomStaffAlloc;
                tempAllStaff = new ObservableCollection<Staff>();
                memAllStaff = new ObservableCollection<Staff>();
                allConsultationRoomStaffAllocations = new ObservableCollection<ConsultationRoomStaffAllocations>();
                foreach (var temp in CurRefDepartmentsTree.LstConsultationRoomStaffAllocations)
                {
                    if (temp.ConsultationTimeSegments.ConsultationTimeSegmentID == selectedTempConsultRoomStaffAlloc.ConsultationTimeSegments.ConsultationTimeSegmentID
                        && temp.AllocationDate.Date == selectedTempConsultRoomStaffAlloc.AllocationDate.Date)
                    {
                        allConsultationRoomStaffAllocations.Add(temp);
                        tempAllStaff.Add(temp.Staff);
                        memAllStaff.Add(temp.Staff);
                    }
                }
            }
            //;
        }
        public void lnkDeleteClick(object sender)
        {
            if (MessageBox.Show(eHCMSResources.A0169_G1_Msg_ConfXoaPhanBo, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
            {
                return;
            }
            DeleteConsultationRoomStaffAllocations(selectedTempConsultRoomStaffAlloc.DeptLocationID
                                            , selectedTempConsultRoomStaffAlloc.ConsultationTimeSegmentID
                                            , selectedTempConsultRoomStaffAlloc.AllocationDate);
        }
        #region method
        public void grdListTarget_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            ConsultationRoomStaffAllocations objRows = e.Row.DataContext as ConsultationRoomStaffAllocations;
            if (objRows != null)
            {
                switch (objRows.isEdit)
                {
                    case true:
                        {
                            e.Row.Background = new SolidColorBrush(Colors.Green);
                            break;
                        }
                    case false:
                        {
                            e.Row.Background = new SolidColorBrush(Colors.Orange);
                            break;
                        }
                }
            }
        }

        private void InsertConsultationTimeSegments(string SegmentName, string SegmentDescription, DateTime StartTime,
                                            DateTime EndTime, bool IsActive)
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                IsLoading = true;

                using (var serviceFactory = new ClinicManagementServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginInsertConsultationTimeSegments(SegmentName, SegmentDescription, StartTime,
                                EndTime, null, null, IsActive, Globals.DispatchCallback((asyncResult) =>
                                {
                                    try
                                    {

                                        var results = contract.EndInsertConsultationTimeSegments(asyncResult);
                                        if (results)
                                        {

                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        IsLoading = false;
                                        Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                                    }
                                    finally
                                    {
                                        //Globals.IsBusy = false;
                                        IsLoading = false;
                                    }

                                }), null);

                }

            });

            t.Start();
        }

        private void GetRefStaffCategoriesByType(long V_StaffCatType)
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                IsLoading = true;

                using (var serviceFactory = new ResourcesManagementServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetRefStaffCategoriesByType(V_StaffCatType, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndGetRefStaffCategoriesByType(asyncResult);
                            if (results != null && results.Count > 0)
                            {
                                if (allRefStaffCategory == null)
                                {
                                    //allRefStaffCategory = new ObservableCollection<RefStaffCategory>();
                                }
                                else
                                {
                                    //allRefStaffCategory.Clear();
                                }
                                foreach (var p in results)
                                {
                                    allRefStaffCategory.Add(p);
                                }

                                NotifyOfPropertyChange(() => allRefStaffCategory);
                            }
                        }
                        catch (Exception ex)
                        {
                            IsLoading = false;
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            //Globals.IsBusy = false;
                            IsLoading = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }

        private void GetAllStaff(long StaffCatgID)
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                IsLoading = true;

                using (var serviceFactory = new ResourcesManagementServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetAllStaff(StaffCatgID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndGetAllStaff(asyncResult);
                            if (results != null && results.Count > 0)
                            {
                                if (allStaff == null)
                                {
                                    allStaff = new ObservableCollection<Staff>();
                                }
                                else
                                {
                                    allStaff.Clear();
                                }
                                foreach (var p in results)
                                {
                                    allStaff.Add(p);
                                }

                                NotifyOfPropertyChange(() => allStaff);
                            }
                        }
                        catch (Exception ex)
                        {
                            IsLoading = false;
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            //Globals.IsBusy = false;
                            IsLoading = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }

        private void GetAllConsultationTimeSegments()
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                IsLoading = true;

                using (var serviceFactory = new ClinicManagementServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetAllConsultationTimeSegments(Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndGetAllConsultationTimeSegments(asyncResult);
                            if (results != null && results.Count > 0)
                            {
                                lstConsultationTimeSegments = new ObservableCollection<ConsultationTimeSegments>();
                                foreach (var consTimeSeg in results)
                                {
                                    lstConsultationTimeSegments.Add(consTimeSeg);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            IsLoading = false;
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            //Globals.IsBusy = false;
                            IsLoading = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }
        private void DeleteConsultationRoomStaffAllocations(long DeptLocationID
                                                                , long ConsultationTimeSegmentID
                                                                , DateTime AllocationDate)
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                IsLoading = true;
                using (var serviceFactory = new ClinicManagementServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginDeleteConsultationRoomStaffAllocations(DeptLocationID
                                                                , ConsultationTimeSegmentID
                                                                , AllocationDate, Globals.DispatchCallback((asyncResult) =>
                                                                {
                                                                    try
                                                                    {

                                                                        var results = contract.EndDeleteConsultationRoomStaffAllocations(asyncResult);
                                                                        if (results)
                                                                        {
                                                                            Globals.ShowMessage(eHCMSResources.K0537_G1_XoaOk, "");
                                                                            Globals.EventAggregator.Publish(new AddNewRoomTargetEvent());
                                                                        }
                                                                    }
                                                                    catch (Exception ex)
                                                                    {
                                                                        IsLoading = false;
                                                                        Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                                                                    }
                                                                    finally
                                                                    {
                                                                        //Globals.IsBusy = false;
                                                                        IsLoading = false;
                                                                    }

                                                                }), null);

                }

            });

            t.Start();
        }
        private void InsertConsultationRoomTarget(ConsultationRoomTarget curConsultationRoomTarget)
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                IsLoading = true;

                using (var serviceFactory = new ClinicManagementServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginInsertConsultationRoomTarget(curConsultationRoomTarget, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {

                            var results = contract.EndInsertConsultationRoomTarget(asyncResult);
                            if (results)
                            {
                                Globals.ShowMessage(eHCMSResources.A0079_G1_Msg_InfoThemMoiOK, "");
                            }
                        }
                        catch (Exception ex)
                        {
                            IsLoading = false;
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            //Globals.IsBusy = false;
                            IsLoading = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }
        private void GetConsultationRoomTargetByDeptID(long DeptLocationID)
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                IsLoading = true;

                using (var serviceFactory = new ClinicManagementServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetConsultationRoomTargetByDeptID(DeptLocationID
                        , Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndGetConsultationRoomTargetByDeptID(asyncResult);
                                if (results != null && results.Count > 0)
                                {
                                    results[0].DeptLocationID = curDeptLocation.DeptLocationID;
                                    selectedConsultationRoomTarget = results[0];

                                }
                            }
                            catch (Exception ex)
                            {
                                IsLoading = false;
                                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                            }
                            finally
                            {
                                //Globals.IsBusy = false;
                                IsLoading = false;
                            }

                        }), null);

                }

            });

            t.Start();
        }

        private void GetConsultationRoomTargetTimeSegment(long DeptLocationID, long ConsultationTimeSegmentID)
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                IsLoading = true;

                using (var serviceFactory = new ClinicManagementServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetConsultationRoomTargetTimeSegment(DeptLocationID, ConsultationTimeSegmentID
                        , Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndGetConsultationRoomTargetTimeSegment(asyncResult);
                                //if (results != null && results.Count > 0)
                                //{
                                //    allConsultationRoomTarget = new ObservableCollection<ConsultationRoomTarget>();
                                //    foreach (var ConsultRoomTarget in results)
                                //    {
                                //        allConsultationRoomTarget.Add(ConsultRoomTarget);
                                //        if (ConsultRoomTarget.TargetDate <= DateTime.Now)
                                //        {
                                //            selectedConsultationRoomTarget.TargetDate =ConsultRoomTarget.TargetDate;
                                //            selectedConsultationRoomTarget.TargetNumberOfCases = ConsultRoomTarget.TargetNumberOfCases;
                                //            NotifyOfPropertyChange(() => selectedConsultationRoomTarget);
                                //        }
                                //    }
                                //}
                                if (results != null && results.Count > 0)
                                {
                                    NotifyOfPropertyChange(() => selectedConsultationRoomTarget);
                                }
                            }
                            catch (Exception ex)
                            {
                                IsLoading = false;
                                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                            }
                            finally
                            {
                                //Globals.IsBusy = false;
                                IsLoading = false;
                            }

                        }), null);

                }

            });

            t.Start();
        }


        private void InsertConsultationRoomStaffAllocations(long DeptLocationID
                                                  , long ConsultationTimeSegmentID
                                                  , long StaffID
                                                  , long StaffCatgID
                                                  , DateTime AllocationDate)
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                IsLoading = true;

                using (var serviceFactory = new ClinicManagementServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginInsertConsultationRoomStaffAllocations(DeptLocationID, ConsultationTimeSegmentID
                                                  , StaffID, StaffCatgID, AllocationDate, Globals.DispatchCallback((asyncResult) =>
                                                  {
                                                      try
                                                      {

                                                          var results = contract.EndInsertConsultationRoomStaffAllocations(asyncResult);
                                                          if (results)
                                                          {
                                                              Globals.ShowMessage(eHCMSResources.A0079_G1_Msg_InfoThemMoiOK, "");
                                                          }
                                                      }
                                                      catch (Exception ex)
                                                      {
                                                          IsLoading = false;
                                                          Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                                                      }
                                                      finally
                                                      {
                                                          //Globals.IsBusy = false;
                                                          IsLoading = false;
                                                      }

                                                  }), null);

                }

            });

            t.Start();
        }

        private void InsertConsultationRoomStaffAllocationsXML(IList<ConsultationRoomStaffAllocations> lstCRSA)
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                IsLoading = true;

                using (var serviceFactory = new ClinicManagementServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginInsertConsultationRoomStaffAllocationsXML(lstCRSA, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndInsertConsultationRoomStaffAllocationsXML(asyncResult);
                            if (results)
                            {
                                Globals.ShowMessage(eHCMSResources.A0079_G1_Msg_InfoThemMoiOK, "");
                                Globals.EventAggregator.Publish(new AddNewRoomTargetEvent());
                                //GetConsultationRoomStaffAllocations(selectedConsultationRoomTarget.DeptLocationID
                                //                        , selectedConsultationRoomTarget.ConsultationTimeSegmentID);
                            }
                        }
                        catch (Exception ex)
                        {
                            IsLoading = false;
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            //Globals.IsBusy = false;
                            IsLoading = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }


        private void UpdateConsultationRoomStaffAllocationsXML(IList<ConsultationRoomStaffAllocations> lstCRSA)
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                IsLoading = true;

                using (var serviceFactory = new ClinicManagementServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginUpdateConsultationRoomStaffAllocationsXML(lstCRSA, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {

                            var results = contract.EndUpdateConsultationRoomStaffAllocationsXML(asyncResult);
                            if (results)
                            {
                                Globals.ShowMessage(eHCMSResources.A0279_G1_Msg_InfoCNhatOK, "");
                                Globals.EventAggregator.Publish(new AddNewRoomTargetEvent());
                                //GetConsultationRoomStaffAllocations(selectedConsultationRoomTarget.DeptLocationID
                                //                        , selectedConsultationRoomTarget.ConsultationTimeSegmentID);
                            }
                        }
                        catch (Exception ex)
                        {
                            IsLoading = false;
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            //Globals.IsBusy = false;
                            IsLoading = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }

        private void UpdateConsultationRoomStaffAllocations(long ConsultationRoomStaffAllocID, bool IsActive)
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                IsLoading = true;

                using (var serviceFactory = new ClinicManagementServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginUpdateConsultationRoomStaffAllocations(ConsultationRoomStaffAllocID, IsActive
                        , Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {

                                var results = contract.EndUpdateConsultationRoomStaffAllocations(asyncResult);
                                if (results)
                                {
                                    Globals.ShowMessage(eHCMSResources.A0279_G1_Msg_InfoCNhatOK, "");
                                    Globals.EventAggregator.Publish(new AddNewRoomTargetEvent());
                                    //GetConsultationRoomStaffAllocations(selectedConsultationRoomTarget.DeptLocationID
                                    //                    ,selectedConsultationRoomTarget.ConsultationTimeSegmentID);
                                }
                            }
                            catch (Exception ex)
                            {
                                IsLoading = false;
                                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                            }
                            finally
                            {
                                //Globals.IsBusy = false;
                                IsLoading = false;
                            }

                        }), null);

                }

            });

            t.Start();
        }
        private void GetConsultationRoomStaffAllocations(long DeptLocationID, long ConsultationTimeSegmentID)
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                IsLoading = true;

                using (var serviceFactory = new ClinicManagementServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetConsultationRoomStaffAllocations(DeptLocationID, ConsultationTimeSegmentID
                        , Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                allConsultationRoomStaffAllocations = new ObservableCollection<ConsultationRoomStaffAllocations>();
                                var results = contract.EndGetConsultationRoomStaffAllocations(asyncResult);
                                if (results != null)
                                {

                                    tempAllStaff = new ObservableCollection<Staff>();
                                    memAllStaff = new ObservableCollection<Staff>();
                                    allConsultationRoomStaffAllocations.Clear();
                                    tempAllStaff.Clear();
                                    if (results.Count > 0)
                                    {
                                        appDate = results[0].AllocationDate;
                                        selectedConsultationRoomStaffAllocations = results[0];
                                        foreach (var RoomStaffAllocation in results)
                                        {
                                            if (RoomStaffAllocation.AllocationDate.ToShortDateString() == appDate.ToShortDateString())
                                            {
                                                allConsultationRoomStaffAllocations.Add(RoomStaffAllocation);
                                                if (isDoctor)
                                                {
                                                    if (RoomStaffAllocation.Staff.RefStaffCategory.V_StaffCatType ==
                                                        (long)V_StaffCatType.BacSi)
                                                    {
                                                        tempAllStaff.Add(RoomStaffAllocation.Staff);
                                                        memAllStaff.Add(RoomStaffAllocation.Staff);
                                                    }
                                                }
                                                else
                                                {
                                                    if (RoomStaffAllocation.Staff.RefStaffCategory.V_StaffCatType ==
                                                        (long)V_StaffCatType.PhuTa)
                                                    {
                                                        tempAllStaff.Add(RoomStaffAllocation.Staff);
                                                        memAllStaff.Add(RoomStaffAllocation.Staff);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        //selectedConsultationRoomStaffAllocations.AllocationDate = DBNull.Value;
                                    }

                                }
                            }
                            catch (Exception ex)
                            {
                                IsLoading = false;
                                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                            }
                            finally
                            {
                                //Globals.IsBusy = false;
                                IsLoading = false;
                            }

                        }), null);

                }

            });

            t.Start();
        }
        #endregion

        #region subcribe
        public void Handle(RoomSelectedEvent Obj)
        {
            if (Obj != null)
            {
                selectedConsultationRoomTarget = new ConsultationRoomTarget();
                selectedConsultationRoomStaffAllocations = new ConsultationRoomStaffAllocations();

                CurRefDepartmentsTree = (RefDepartmentsTree)Obj.curDeptLoc;
                if (CurRefDepartmentsTree.LstConsultationRoomTarget != null)
                {
                    allConsultationRoomTarget = new ObservableCollection<ConsultationRoomTarget>(CurRefDepartmentsTree.LstConsultationRoomTarget);
                    //lay ra danh sach da filter theo ca
                    allConsulRoomStaffAlloc = GetCurRoomStaff(new ObservableCollection<ConsultationRoomStaffAllocations>(CurRefDepartmentsTree.LstConsultationRoomStaffAllocations));
                    tempConsRoomStaffAlloc = new ObservableCollection<ConsultationRoomStaffAllocations>();
                    tempConsRoomStaffAlloc = GetRoomStaffAlloByType(StaffCatType);
                    NotifyOfPropertyChange(() => tempConsRoomStaffAlloc);
                }
                selectedConsultationRoomTarget.DeptLocationID = CurRefDepartmentsTree.NodeID;
                selectedConsultationRoomStaffAllocations.DeptLocationID = CurRefDepartmentsTree.NodeID;

                GetCurTimeSegmentChoice();
                curDeptLocation = new DeptLocation();
                curDeptLocation.DeptLocationID = CurRefDepartmentsTree.NodeID;
                curDeptLocation.Location = new Location();
                curDeptLocation.Location.LocationName = CurRefDepartmentsTree.NodeText;
                tempAllStaff.Clear();
                //if (selectedConsultationRoomTarget.DeptLocationID>0)
                //{
                //    
                //    GetConsultationRoomTargetByDeptID(selectedConsultationRoomTarget.DeptLocationID);
                //    //GetConsultationRoomStaffAllocations(selectedConsultationRoomTarget.DeptLocationID
                //    //    ,selectedConsultationRoomTarget.ConsultationTimeSegmentID);
                //}
            }
        }



        #endregion

        #region   animator

        private bool isOnGrid = false;
        Point midRec = new Point(0, 0);

        public Grid LayoutRoot { get; set; }
        public StackPanel ChildRec { get; set; }
        public TranslateTransform RecTranslateTransform { get; set; }
        public void initGrid()
        {
            for (int i = 0; i < 3; i++)
            {
                LayoutRoot.RowDefinitions.Add(new RowDefinition());
            }
            for (int i = 0; i < 3; i++)
            {
                LayoutRoot.ColumnDefinitions.Add(new ColumnDefinition());
            }
        }
        public void removeGrid()
        {
            try
            {
                List<UIElement> removedItems = new List<UIElement>();
                foreach (UIElement child in LayoutRoot.Children)
                {
                    if (child is StackPanel)
                        removedItems.Add(child);
                }
                foreach (var removedItem in removedItems)
                {
                    LayoutRoot.Children.Remove(removedItem);
                }
            }
            catch (Exception ex)
            {
                ClientLoggerHelper.LogInfo(ex.ToString());
            }
        }
        Point ply = new Point(0, 0);
        Point plys = new Point(0, 0);


        public void LayoutRoot_OnLoaded(object sender, RoutedEventArgs e)
        {
            LayoutRoot = sender as Grid;
            LayoutRoot.MouseMove += new MouseEventHandler(LayoutRoot_MouseMove);
            LayoutRoot.MouseLeftButtonUp += new MouseButtonEventHandler(LayoutRoot_MouseLeftButtonUp);
            LayoutRoot.MouseLeftButtonDown += new MouseButtonEventHandler(LayoutRoot_MouseLeftButtonDown);
            ply = new Point(LayoutRoot.RenderTransformOrigin.X + LayoutRoot.RenderSize.Width
                , LayoutRoot.RenderTransformOrigin.Y + LayoutRoot.RenderSize.Height);
        }

        void LayoutRoot_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (isChildRecMouseCapture)
            {
                PositionClick = e.GetPosition(LayoutRoot as UIElement);
                this.RecTranslateTransform.X = PositionClick.X;
                this.RecTranslateTransform.Y = PositionClick.Y;
            }
        }

        void LayoutRoot_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (isChildRecMouseCapture)
            {
                PositionClick = e.GetPosition(sender as UIElement);
                this.RecTranslateTransform.X = PositionClick.X;
                this.RecTranslateTransform.Y = PositionClick.Y;
                this.ChildRec.ReleaseMouseCapture();
                isChildRecMouseCapture = false;
                ChildRec.Visibility = Visibility.Collapsed;
                if (!isOnGrid)
                {
                    if (PositionClick.X < ply.X)
                    {
                        if (!checkExist(SelectedStaff))
                        {
                            return;
                        }
                        if (StaffCatType == (long)V_StaffCatType.BacSi)
                            SelectedStaff.SFirstName = "BS. ";
                        else
                            SelectedStaff.SFirstName = "ĐD. ";
                        SelectedStaff.SLastName = SelectedStaff.FullName.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries).Last();
                        SelectedStaff.FullName = SelectedStaff.SFirstName + SelectedStaff.FullName;
                        tempAllStaff.Add(SelectedStaff);
                    }
                }
                else
                {
                    if (PositionClick.Y > plys.Y)
                    {
                        tempAllStaff.Remove(SelectedStaffGrid);
                    }
                }
            }
        }
        public void StaffGrid_Loaded(object sender, RoutedEventArgs e)
        {
            plys = new Point(((UIElement)sender).RenderTransformOrigin.X + ((UIElement)sender).RenderSize.Width
                , ((UIElement)sender).RenderTransformOrigin.Y + ((UIElement)sender).RenderSize.Height);
        }
        public void LayoutRoot_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ply = new Point(((UIElement)sender).RenderTransformOrigin.X + ((UIElement)sender).RenderSize.Width
                , ((UIElement)sender).RenderTransformOrigin.Y + ((UIElement)sender).RenderSize.Height);
        }
        void LayoutRoot_MouseMove(object sender, MouseEventArgs e)
        {
            if (isChildRecMouseCapture)
            {
                PositionClick = e.GetPosition(sender as UIElement);
            }
        }

        public void ChildRec_OnLoaded(object sender, RoutedEventArgs e)
        {
            ChildRec = sender as StackPanel;
            RecTranslateTransform = ChildRec.FindName("RecTranslateTransform") as TranslateTransform;
            //ChildRec.MouseLeftButtonUp+=new MouseButtonEventHandler(ChildRec_MouseLeftButtonUp);
            ChildRec.MouseLeftButtonDown += new MouseButtonEventHandler(ChildRec_MouseLeftButtonDown);
            ChildRec.MouseMove += new MouseEventHandler(ChildRec_MouseMove);
            ChildRec.Visibility = Visibility.Collapsed;
        }

        void ChildRec_MouseMove(object sender, MouseEventArgs e)
        {
            if (isChildRecMouseCapture)
            {
                this.RecTranslateTransform.X = PositionClick.X - midRec.X;
                this.RecTranslateTransform.Y = PositionClick.Y - midRec.Y;
                ChildRec.Visibility = Visibility.Visible;
            }
        }

        void ChildRec_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {

                this.ChildRec.CaptureMouse();

                isChildRecMouseCapture = true;
                PositionClick = e.GetPosition(LayoutRoot as UIElement);
                this.RecTranslateTransform.X = PositionClick.X - midRec.X;
                this.RecTranslateTransform.Y = PositionClick.Y - midRec.Y;
                ChildRec.MouseMove += new MouseEventHandler(ChildRec_MouseMove);
            }
            catch (Exception ex)
            {
                ClientLoggerHelper.LogInfo(ex.ToString());
            }
        }


        public bool checkExist(Staff st)
        {
            foreach (var curStaff in tempAllStaff)
            {
                if (curStaff.StaffID == st.StaffID)
                {
                    Globals.ShowMessage("Đã có nhân viên này trong phòng khám", "");
                    return false;
                }
            }
            return true;
        }



        public bool isChildRecMouseCapture = false;
        private Point PositionClick;

        public StackPanel imageStaff { get; set; }
        public StackPanel ImageStaffGrid { get; set; }

        public void ImageStaff_Loaded(object sender, RoutedEventArgs e)
        {
            imageStaff = sender as StackPanel;
            imageStaff.MouseLeftButtonDown += new MouseButtonEventHandler(imageStaff_MouseLeftButtonDown);
        }

        void imageStaff_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            isOnGrid = false;

            midRec.Y = ((StackPanel)sender).RenderSize.Height / 2;
            midRec.X = ((StackPanel)sender).RenderSize.Width / 2;

            ChildRec.DataContext = ((StackPanel)sender).DataContext;
            ChildRec_MouseLeftButtonDown(sender, e);
        }

        public void ImageStaffGrid_Loaded(object sender, RoutedEventArgs e)
        {
            ImageStaffGrid = sender as StackPanel;
            ImageStaffGrid.MouseLeftButtonDown += new MouseButtonEventHandler(ImageStaffGrid_MouseLeftButtonDown);
        }
        void ImageStaffGrid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            isOnGrid = true;

            midRec.Y = ((StackPanel)sender).RenderSize.Height / 2;
            midRec.X = ((StackPanel)sender).RenderSize.Width / 2;
            ChildRec.DataContext = ((StackPanel)sender).DataContext;
            ChildRec_MouseLeftButtonDown(sender, e);
        }

        #endregion

        #region authoriztion
        public void authorization()
        {
            if (!Globals.isAccountCheck)
            {
                return;
            }
            bQuanEdit = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mClinicManagement
                                               , (int)eClinicManagement.mQuanLyPhongKham,
                                               (int)oClinicManagementEx.mQuanLySoLuong, (int)ePermission.mEdit);
            bQuanAdd = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mClinicManagement
                                               , (int)eClinicManagement.mQuanLyPhongKham,
                                               (int)oClinicManagementEx.mQuanLySoLuong, (int)ePermission.mAdd);
            bQuanDelete = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mClinicManagement
                                               , (int)eClinicManagement.mQuanLyPhongKham,
                                               (int)oClinicManagementEx.mQuanLySoLuong, (int)ePermission.mDelete);
            bQuanView = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mClinicManagement
                                               , (int)eClinicManagement.mQuanLyPhongKham,
                                               (int)oClinicManagementEx.mQuanLySoLuong, (int)ePermission.mView);

            bStaffEdit = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mClinicManagement
                                               , (int)eClinicManagement.mQuanLyPhongKham,
                                               (int)oClinicManagementEx.mPhanBoNhanVien, (int)ePermission.mEdit);
            bStaffAdd = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mClinicManagement
                                               , (int)eClinicManagement.mQuanLyPhongKham,
                                               (int)oClinicManagementEx.mPhanBoNhanVien, (int)ePermission.mAdd);
            bStaffDelete = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mClinicManagement
                                               , (int)eClinicManagement.mQuanLyPhongKham,
                                               (int)oClinicManagementEx.mPhanBoNhanVien, (int)ePermission.mDelete);
            bStaffView = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mClinicManagement
                                               , (int)eClinicManagement.mQuanLyPhongKham,
                                               (int)oClinicManagementEx.mPhanBoNhanVien, (int)ePermission.mView);

        }
        #region checking account

        private bool _bQuanEdit = true;
        private bool _bQuanAdd = true;
        private bool _bQuanDelete = true;
        private bool _bQuanView = true;

        private bool _bStaffEdit = true;
        private bool _bStaffAdd = true;
        private bool _bStaffDelete = true;
        private bool _bStaffView = true;

        public bool bQuanEdit
        {
            get
            {
                return _bQuanEdit;
            }
            set
            {
                if (_bQuanEdit == value)
                    return;
                _bQuanEdit = value;
            }
        }
        public bool bQuanAdd
        {
            get
            {
                return _bQuanAdd;
            }
            set
            {
                if (_bQuanAdd == value)
                    return;
                _bQuanAdd = value;
            }
        }
        public bool bQuanDelete
        {
            get
            {
                return _bQuanDelete;
            }
            set
            {
                if (_bQuanDelete == value)
                    return;
                _bQuanDelete = value;
            }
        }
        public bool bQuanView
        {
            get
            {
                return _bQuanView;
            }
            set
            {
                if (_bQuanView == value)
                    return;
                _bQuanView = value;
            }
        }

        public bool bStaffEdit
        {
            get
            {
                return _bStaffEdit;
            }
            set
            {
                if (_bStaffEdit == value)
                    return;
                _bStaffEdit = value;
            }
        }
        public bool bStaffAdd
        {
            get
            {
                return _bStaffAdd;
            }
            set
            {
                if (_bStaffAdd == value)
                    return;
                _bStaffAdd = value;
            }
        }
        public bool bStaffDelete
        {
            get
            {
                return _bStaffDelete;
            }
            set
            {
                if (_bStaffDelete == value)
                    return;
                _bStaffDelete = value;
            }
        }
        public bool bStaffView
        {
            get
            {
                return _bStaffView;
            }
            set
            {
                if (_bStaffView == value)
                    return;
                _bStaffView = value;
            }
        }

        #endregion
        #region binding visibilty


        #endregion
        #endregion
    }

    public class Surgery
    {
        public DateTime ExamDate { get; set; }
        public string Patient { get; set; }
        public string ExamBy { get; set; }
        public string TimeSegment { get; set; }
    }
}