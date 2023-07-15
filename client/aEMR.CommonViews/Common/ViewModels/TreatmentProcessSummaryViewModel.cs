using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Caliburn.Micro;
using System.Collections.ObjectModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using DataEntities;
using System.Linq;
using System.ComponentModel;
using eHCMSLanguage;
using aEMR.Common.Collections;
using aEMR.ViewContracts;
using aEMR.Infrastructure.Events;
using aEMR.Infrastructure;
using aEMR.ServiceClient;
using aEMR.CommonTasks;
using Castle.Windsor;
using aEMR.Infrastructure.CachingUtils;
using static aEMR.Infrastructure.Events.TransferFormEvent;
using aEMR.Controls;
/*
* 20230502 #001 QTD: Tạo mới View
*/
namespace aEMR.Common.ViewModels
{
    [Export(typeof(ITreatmentProcessSummary)), PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class TreatmentProcessSummaryViewModel : Conductor<object>, ITreatmentProcessSummary
        , IHandle<ShowInPatientInfoForConsultation>
    {
        public void SetCurrentInformation(TreatmentProcessEvent aEvent)
        {
            if (aEvent != null && aEvent.Item != null)
            {
                {
                    SetCurrentTreatmentProcess(aEvent.Item);
                }
            }
        }
        private PhysicalExamination _pPhyExamItem;
        public PhysicalExamination PtPhyExamItem
        {
            get
            {
                return _pPhyExamItem;
            }
            set
            {
                if (_pPhyExamItem != value)
                {
                    _pPhyExamItem = value;
                    NotifyOfPropertyChange(() => PtPhyExamItem);
                }
            }
        }

        private TreatmentProcess _CurrentTreatmentProcess;
        public TreatmentProcess CurrentTreatmentProcess
        {
            get
            {
                return _CurrentTreatmentProcess;
            }
            set
            {
                _CurrentTreatmentProcess = value;
                NotifyOfPropertyChange(() => CurrentTreatmentProcess);
            }
        }

        private ObservableCollection<TreatmentProcess> _CurrentTreatmentProcessList;
        public ObservableCollection<TreatmentProcess> CurrentTreatmentProcessList
        {
            get
            {
                return _CurrentTreatmentProcessList;
            }
            set
            {
                _CurrentTreatmentProcessList = value;
                NotifyOfPropertyChange(() => CurrentTreatmentProcessList);
            }
        }

        private PaperReferal _ConfirmedPaperReferal;
        public PaperReferal ConfirmedPaperReferal
        {
            get
            {
                return _ConfirmedPaperReferal;
            }
            set
            {
                _ConfirmedPaperReferal = value;
                NotifyOfPropertyChange(() => _ConfirmedPaperReferal);
                NotifyOfPropertyChange(() => ConfirmedPaperReferal);
            }
        }

        private IHospitalAutoCompleteListing _FromHospitalAutoCnt;
        public IHospitalAutoCompleteListing FromHospitalAutoCnt
        {
            get { return _FromHospitalAutoCnt; }
            set
            {
                _FromHospitalAutoCnt = value;
                NotifyOfPropertyChange(() => FromHospitalAutoCnt);
            }
        }

        private IHospitalAutoCompleteListing _ToHospitalAutoCnt;
        public IHospitalAutoCompleteListing ToHospitalAutoCnt
        {
            get { return _ToHospitalAutoCnt; }
            set
            {
                _ToHospitalAutoCnt = value;
                NotifyOfPropertyChange(() => ToHospitalAutoCnt);
            }
        }

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

        public void ActiveContentCmd()
        {
            // HospitalAutoCompleteConten
            var hospitalAutoCompleteVm1 = Globals.GetViewModel<IHospitalAutoCompleteListing>();
            hospitalAutoCompleteVm1.IsPaperReferal = true;
            //hospitalAutoCompleteVm1.Parent = this;
            FromHospitalAutoCnt = hospitalAutoCompleteVm1;
            FromHospitalAutoCnt.InitBlankBindingValue();
            _eventArg.Subscribe(this);

            var hospitalAutoCompleteVm2 = Globals.GetViewModel<IHospitalAutoCompleteListing>();
            hospitalAutoCompleteVm2.IsPaperReferal = true;
            //hospitalAutoCompleteVm1.Parent = this;
            ToHospitalAutoCnt = hospitalAutoCompleteVm2;
            ToHospitalAutoCnt.InitBlankBindingValue();
            _eventArg.Subscribe(this);

            // SearchRegistrationContent
            var searchPatientAndRegVm = Globals.GetViewModel<ISearchPatientAndRegistration>();
            if (Globals.PatientFindBy_ForConsultation == null)
            {
                Globals.PatientFindBy_ForConsultation = AllLookupValues.PatientFindBy.NGOAITRU;
            }
            searchPatientAndRegVm.PatientFindBy = Globals.PatientFindBy_ForConsultation.Value;
            searchPatientAndRegVm.CloseRegistrationFormWhenCompleteSelection = false;

            searchPatientAndRegVm.InitButtonVisibility(SearchRegButtonsVisibility.SHOW_SEARCH_REG_BTN);
            searchPatientAndRegVm.SetDefaultButton(SearchRegistrationButtons.SEARCH_REGISTRATION);

            searchPatientAndRegVm.IsSearchGoToKhamBenh = true;
            searchPatientAndRegVm.PatientFindByVisibility = true;
            searchPatientAndRegVm.CanSearhRegAllDept = true;
            searchPatientAndRegVm.PatientFindByVisibility = true;
            searchPatientAndRegVm.SearchAdmittedInPtRegOnly = true;
            SearchRegistrationContent = searchPatientAndRegVm;
            V_PrognosisCollection = Globals.AllLookupValueList.Where(x => x.ObjectTypeID == (long)LookupValues.V_Prognosis).ToObservableCollection();
            V_ResultsEvaluationCollection = Globals.AllLookupValueList.Where(x => x.ObjectTypeID == (long)LookupValues.V_ResultsEvaluation).ToObservableCollection();
            LoadDoctorStaffCollection();
            ActivateItem(searchPatientAndRegVm);
        }

        private string _TitleForm = eHCMSResources.T1205_G1_GCVien;
        public string TitleForm
        {
            get
            {
                return _TitleForm;
            }
            set
            {
                _TitleForm = value;
                NotifyOfPropertyChange(() => TitleForm);
            }
        }
        IEventAggregator _eventArg;
        [ImportingConstructor]
        public TreatmentProcessSummaryViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg, ISalePosCaching salePosCaching)
        {
            _eventArg = eventArg;
            TitleForm = Globals.TitleForm;
            ActiveContentCmd();
            CurrentTreatmentProcess = new TreatmentProcess();
        }

        public void btnSave_Click()
        {
            if(CurrentTreatmentProcess == null)
            {
                return;
            }

            if(CurrentTreatmentProcess.TreatmentProcessID == 0 && IsUpdate)
            {
                return;
            }

            if (string.IsNullOrEmpty(CurrentTreatmentProcess.TreatmentsProcess))
            {
                MessageBox.Show("Chưa nhập quá trình điều trị!", eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }

            if (string.IsNullOrEmpty(CurrentTreatmentProcess.Treatments))
            {
                MessageBox.Show("Chưa nhập hướng điều trị!", eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }

            if (string.IsNullOrEmpty(CurrentTreatmentProcess.ResultsEvaluation))
            {
                MessageBox.Show("Chưa nhập đánh giá kết quả!", eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }

            if (string.IsNullOrEmpty(CurrentTreatmentProcess.Prognosis))
            {
                MessageBox.Show("Chưa nhập tiên lượng!", eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }

            if (CurrentTreatmentProcess.HeadOfDepartmentDoctorStaffID == 0)
            {
                MessageBox.Show("Chưa chọn bác sĩ Trưởng/Phó khoa!", eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }

            if (CurrentTreatmentProcess.DoctorStaffID == 0)
            {
                MessageBox.Show("Chưa chọn bác sĩ điều trị!", eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }

            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                using (var serviceFactory = new ePMRsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginSaveTreatmentProcess(CurrentTreatmentProcess, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            TreatmentProcess result = contract.EndSaveTreatmentProcess(asyncResult);
                            if (result != null)
                            {
                                CurrentTreatmentProcess = result.DeepCopy();
                                MessageBox.Show(eHCMSResources.A0468_G1_Msg_InfoLuuOK);
                                Globals.EventAggregator.Publish(new TreatmentProcessEvent.OnChangedTreatmentProcess() { TreatmentProcess = CurrentTreatmentProcess });
                                IsUpdate = true;
                                IsNeedTreatmentSummary = false;
                            }
                            if(CurrentTreatmentProcess != null && CurrentTreatmentProcess.CurPatientRegistration != null)
                            {
                                GetAllTreatmentProcess(CurrentTreatmentProcess.CurPatientRegistration.PtRegistrationID);
                            }
                            this.HideBusyIndicator();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                            this.HideBusyIndicator();
                        }

                    }), null);
                }
            });
            t.Start();
        }

        public void SetCurrentTreatmentProcess(TreatmentProcess item)
        {
            CurrentTreatmentProcess = item;
            if (CurrentTreatmentProcess.TreatmentProcessID == 0)
            {
                GetRegistationForCirculars56(CurrentTreatmentProcess.FromDate, CurrentTreatmentProcess.FromDate, 
                    (long)AllLookupValues.PatientFindBy.NOITRU, CurrentTreatmentProcess.CurPatientRegistration.Patient.PatientCode);
            }
        }

        private bool _V_GetPaperReferalFullFromOtherView = false;

        public bool V_GetPaperReferalFullFromOtherView
        {
            get
            {
                return _V_GetPaperReferalFullFromOtherView;
            }
            set
            {
                if (_V_GetPaperReferalFullFromOtherView == value)
                    return;
                _V_GetPaperReferalFullFromOtherView = value;
                NotifyOfPropertyChange(() => V_GetPaperReferalFullFromOtherView);
            }
        }
        /*TMA*/
        protected override void OnActivate()
        {
            base.OnActivate();
            if (V_GetPaperReferalFullFromOtherView == true)
            {
                _eventArg.Subscribe(this);

            }
            TitleForm = "PHIẾU SƠ KẾT 15 NGÀY ĐIỀU TRỊ";
        }
        //▼====== #003
        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);
            _eventArg.Unsubscribe(this);
        }
        //▲====== #003
        
        public void btnDelete_Click()
        {
            if (CurrentTreatmentProcess == null || CurrentTreatmentProcess.TreatmentProcessID == 0) return;
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ePMRsServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginDeleteTreatmentProcess(CurrentTreatmentProcess.TreatmentProcessID, Globals.LoggedUserAccount.Staff.StaffID, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                if (contract.EndDeleteTreatmentProcess(asyncResult))
                                {
                                    MessageBox.Show(eHCMSResources.K0537_G1_XoaOk, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                    //▼====== #004
                                    if (IsThisViewDialog)
                                    {
                                        TryClose();
                                    }
                                    else
                                    {
                                        //Sau khi xóa thì màn hình trắng tinh, người dùng sẽ tìm cái giấy chuyển khác làm việc
                                        //Vì sau khi xóa rồi thì không cần phải chừa thông tin nào lại cả.
                                        CurrentTreatmentProcess = new TreatmentProcess();
                                    }
                                    //▲====== #004
                                    Globals.EventAggregator.Publish(new OnChangedPaperReferal() { TransferForm = new TransferForm() });
                                }
                                else
                                    MessageBox.Show(eHCMSResources.T0074_G1_I, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                            }
                            catch (Exception ex)
                            {
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
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }

        private bool _IsThisViewDialog = false;
        public bool IsThisViewDialog
        {
            get
            {
                return _IsThisViewDialog;
            }
            set
            {
                _IsThisViewDialog = value;
                NotifyOfPropertyChange(() => IsThisViewDialog);
            }
        }
        public void GetRegistationForCirculars56(DateTime FromDate, DateTime ToDate, long PatientFindBy, string PatientCode)
        {
            this.ShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginSearchRegistrationForCirculars56(FromDate, ToDate, PatientFindBy, PatientCode
                            , Globals.DispatchCallback((asyncResult) =>
                            {
                                try
                                {
                                    ObservableCollection<PatientRegistration> ListTemp = new ObservableCollection<PatientRegistration>();
                                    List<PatientRegistration> allItems = client.EndSearchRegistrationForCirculars56(asyncResult);
                                    if (allItems != null && allItems.Count > 0)
                                    {
                                        foreach (var items in allItems)
                                        {
                                            ListTemp.Add(items);
                                        }
                                        PatientRegistration firstRegistration = ListTemp.FirstOrDefault();
                                        CurrentTreatmentProcess.PathologicalProcess = firstRegistration.DiagnosisTreatment.OrientedTreatment;
                                        CurrentTreatmentProcess.Diagnosis = firstRegistration.AdmissionInfo.DischargeNote;
                                        CurrentTreatmentProcess.Treatments = firstRegistration.DiagnosisTreatment.Treatment;
                                        CurrentTreatmentProcess.DischargedCondition = firstRegistration.AdmissionInfo.DischargeStatus;
                                        CurrentTreatmentProcess.Note = firstRegistration.AdmissionInfo.Comment;
                                        GetSummaryPCLResultByPtRegistrationID(firstRegistration.PtRegistrationID);
                                    }
                                    else
                                    {
                                        MessageBox.Show(String.Format("{0}", eHCMSResources.A0638_G1_Msg_InfoKhCoData));
                                    }
                                }
                                catch (Exception ex)
                                {
                                    this.HideBusyIndicator();
                                    MessageBox.Show(ex.Message);
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
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }
        private void GetSummaryPCLResultByPtRegistrationID(long PtRegistrationID)
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PatientRegistrationServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetSummaryPCLResultByPtRegistrationID(PtRegistrationID, (long)AllLookupValues.RegistrationType.NOI_TRU, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            string result = contract.EndGetSummaryPCLResultByPtRegistrationID(asyncResult);
                            if (result != null)
                            {
                                CurrentTreatmentProcess.PCLResult = result;
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                            this.HideBusyIndicator();
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

        public void GetAllTreatmentProcess(long PtRegistrationID)
        {
            if(PtRegistrationID == 0)
            {
                return;
            }
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ePMRsServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetAllTreatmentProcessByPtRegistrationID(PtRegistrationID, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var items = contract.EndGetAllTreatmentProcessByPtRegistrationID(asyncResult);
                                CurrentTreatmentProcessList = new ObservableCollection<TreatmentProcess>();
                                if (items != null)
                                {
                                    CurrentTreatmentProcessList = items.ToObservableCollection();
                                }
                            }
                            catch (Exception ex)
                            {
                                ClientLoggerHelper.LogError(ex.Message);
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
                    MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
                    ClientLoggerHelper.LogError(ex.Message);
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }

        private PatientRegistration _CurrentPatientRegistration;
        public PatientRegistration CurrentPatientRegistration
        {
            get
            {
                return _CurrentPatientRegistration;
            }
            set
            {
                _CurrentPatientRegistration = value;
                NotifyOfPropertyChange(() => CurrentPatientRegistration);
            }
        }

        private Staff _gSelectedDoctorStaff;
        public Staff gSelectedDoctorStaff
        {
            get
            {
                return _gSelectedDoctorStaff;
            }
            set
            {
                _gSelectedDoctorStaff = value;
                if (CurrentTreatmentProcess != null && _gSelectedDoctorStaff != null)
                {
                    CurrentTreatmentProcess.DoctorStaffID = _gSelectedDoctorStaff.StaffID;
                }
                NotifyOfPropertyChange(() => gSelectedDoctorStaff);
            }
        }

        private Staff _gSelectedHeadOfDepartmentDoctorStaff;
        public Staff gSelectedHeadOfDepartmentDoctorStaff
        {
            get
            {
                return _gSelectedHeadOfDepartmentDoctorStaff;
            }
            set
            {
                _gSelectedHeadOfDepartmentDoctorStaff = value;
                if (CurrentTreatmentProcess != null && _gSelectedHeadOfDepartmentDoctorStaff != null)
                {
                    CurrentTreatmentProcess.HeadOfDepartmentDoctorStaffID = _gSelectedHeadOfDepartmentDoctorStaff.StaffID;
                }
                NotifyOfPropertyChange(() => gSelectedHeadOfDepartmentDoctorStaff);
            }
        }

        private ObservableCollection<Staff> _DoctorStaffs;
        public ObservableCollection<Staff> DoctorStaffs
        {
            get
            {
                return _DoctorStaffs;
            }
            set
            {
                if (_DoctorStaffs != value)
                {
                    _DoctorStaffs = value;
                    NotifyOfPropertyChange(() => DoctorStaffs);
                }
            }
        }

        private void LoadDoctorStaffCollection()
        {
            DoctorStaffs = new ObservableCollection<Staff>(Globals.AllStaffs.Where(x => x.RefStaffCategory != null
                                                                                    && x.RefStaffCategory.V_StaffCatType == Globals.ServerConfigSection.CommonItems.StaffCatTypeBAC_SI
                                                                                    && (!x.IsStopUsing)).ToList());
            //&& x.PrintTitle != null && x.PrintTitle.Trim().ToLower() == "bs.").ToList());
            //&& Globals.ServerConfigSection.CommonItems.BacSiTruongPhoKhoa.Contains("|" + x.V_JobPosition.ToString() + "|")).ToList());
            if (DoctorStaffs.Count() > 0 && DoctorStaffs.Any(x => x.StaffID == Globals.LoggedUserAccount.Staff.StaffID))
            {
                gSelectedDoctorStaff = DoctorStaffs.Where(x => x.StaffID == Globals.LoggedUserAccount.Staff.StaffID).FirstOrDefault();
            }
            if (DoctorStaffs.Count() > 0 && DoctorStaffs.Any(x => Globals.ServerConfigSection.CommonItems.ThuTruongDonVi.Contains("|" + x.V_JobPosition.ToString() + "|")
                && x.StaffID == Globals.LoggedUserAccount.Staff.StaffID))
            {
                gSelectedHeadOfDepartmentDoctorStaff = DoctorStaffs.Where(x => x.StaffID == Globals.LoggedUserAccount.Staff.StaffID).FirstOrDefault();
            }
        }

        AxAutoComplete AcbDoctorStaff { get; set; }

        public void DoctorStaff_Loaded(object sender, RoutedEventArgs e)
        {
            AcbDoctorStaff = (AxAutoComplete)sender;
        }

        public void DoctorStaff_Populating(object sender, PopulatingEventArgs e)
        {
            AutoCompleteBox cboContext = sender as AutoCompleteBox;
            e.Cancel = true;
            var AllItemsContext = new ObservableCollection<Staff>(DoctorStaffs.Where(x => //Globals.ServerConfigSection.CommonItems.BacSiTruongPhoKhoa.Contains("|" + x.V_JobPosition.ToString() + "|") &&
                Globals.RemoveVietnameseString(x.FullName).ToLower().Contains(Globals.RemoveVietnameseString(cboContext.SearchText).ToLower())));
            cboContext.ItemsSource = AllItemsContext;
            cboContext.PopulateComplete();
        }

        public void DoctorStaff_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            if (sender != null)
            {
                gSelectedDoctorStaff = ((AutoCompleteBox)sender).SelectedItem as Staff;
            }
        }

        AxAutoComplete AcbHeadOfDepartmentDoctorStaff { get; set; }
        public void HeadOfDepartmentDoctorStaff_Loaded(object sender, RoutedEventArgs e)
        {
            AcbHeadOfDepartmentDoctorStaff = (AxAutoComplete)sender;
        }

        public void HeadOfDepartmentDoctorStaff_Populating(object sender, PopulatingEventArgs e)
        {
            AutoCompleteBox cboContext = sender as AutoCompleteBox;
            e.Cancel = true;
            var AllItemsContext = new ObservableCollection<Staff>(DoctorStaffs.Where(x => Globals.ServerConfigSection.CommonItems.ThuTruongDonVi.Contains("|" + x.V_JobPosition.ToString() + "|") &&
            Globals.RemoveVietnameseString(x.FullName).ToLower().Contains(Globals.RemoveVietnameseString(cboContext.SearchText).ToLower())));
                                                                                        //&& x.IsUnitLeader));
            cboContext.ItemsSource = AllItemsContext;
            cboContext.PopulateComplete();
        }

        public void HeadOfDepartmentDoctorStaff_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            if (sender != null)
            {
                gSelectedHeadOfDepartmentDoctorStaff = ((AutoCompleteBox)sender).SelectedItem as Staff;
            }
        }

        private ObservableCollection<Lookup> _V_PrognosisCollection;
        public ObservableCollection<Lookup> V_PrognosisCollection
        {
            get
            {
                return _V_PrognosisCollection;
            }
            set
            {
                _V_PrognosisCollection = value;
                NotifyOfPropertyChange(() => V_PrognosisCollection);
            }
        }      

        private ObservableCollection<Lookup> _V_ResultsEvaluationCollection;
        public ObservableCollection<Lookup> V_ResultsEvaluationCollection
        {
            get
            {
                return _V_ResultsEvaluationCollection;
            }
            set
            {
                _V_ResultsEvaluationCollection = value;
                NotifyOfPropertyChange(() => V_ResultsEvaluationCollection);
            }
        }

        private Lookup _V_ResultsEvaluationSelected;
        public Lookup V_ResultsEvaluationSelected
        {
            get { return _V_ResultsEvaluationSelected; }
            set
            {
                if (_V_ResultsEvaluationSelected == value)
                {
                    return;
                }
                _V_ResultsEvaluationSelected = value;
                NotifyOfPropertyChange(() => V_ResultsEvaluationSelected);
                if (_V_ResultsEvaluationSelected != null && _V_ResultsEvaluationSelected.LookupID > 0 && CurrentTreatmentProcess != null)
                {
                    CurrentTreatmentProcess.ResultsEvaluation = V_ResultsEvaluationSelected.ObjectValue;
                }
            }
        }

        private Lookup _V_PrognosisSelected;
        public Lookup V_PrognosisSelected
        {
            get { return _V_PrognosisSelected; }
            set
            {
                if (_V_PrognosisSelected == value)
                {
                    return;
                }
                _V_PrognosisSelected = value;
                NotifyOfPropertyChange(() => V_PrognosisSelected);
                if (_V_PrognosisSelected != null && _V_PrognosisSelected.LookupID > 0 && CurrentTreatmentProcess != null)
                {    
                    CurrentTreatmentProcess.Prognosis = V_PrognosisSelected.ObjectValue;
                }
            }
        }

        public void Handle(ShowInPatientInfoForConsultation message)
        {
            IsUpdate = false;
            if (message.PtRegistration != null && message.PtRegistration.PtRegistrationID > 0 && message.Patient != null)
            {
                CurrentPatientRegistration = message.PtRegistration;
                CurrentPatientRegistration.Patient = message.Patient;
                IsNeedTreatmentSummary = message.PtRegistration.AdmissionInfo != null && message.PtRegistration.AdmissionInfo.IsNeedTreatmentSummary;
                if (IsNeedTreatmentSummary)
                {
                    //CurrentTreatmentProcess.CurPatientRegistration = message.PtRegistration;
                    //CurrentTreatmentProcess.PathologicalProcess = message.PtRegistration.AdmissionInfo.PathologicalProcess;
                    //CurrentTreatmentProcess.CurPatientRegistration.Patient = message.Patient;
                    //GetSummaryPCLResultByPtRegistrationID(message.PtRegistration.PtRegistrationID);
                    Reset_Click(message.PtRegistration);
                }
                else
                {
                    CurrentTreatmentProcess = new TreatmentProcess();
                }
                GetAllTreatmentProcess(message.PtRegistration.PtRegistrationID);
                NotifyOfPropertyChange(() => Giuong);
            }
        }

        public string Giuong
        {
            get
            {
                return CurrentTreatmentProcess != null && CurrentTreatmentProcess.CurPatientRegistration != null
                    && CurrentTreatmentProcess.CurPatientRegistration.BedAllocations != null 
                    && CurrentTreatmentProcess.CurPatientRegistration.BedAllocations.Count > 0 ? CurrentTreatmentProcess.CurPatientRegistration.BedAllocations.FirstOrDefault().VBedAllocation.BedNumber : "";
            }
        }

        private void Reset_Click(PatientRegistration temp)
        {
            if(CurrentTreatmentProcess != null)
            {
                IsUpdate = false;
                gSelectedDoctorStaff = new Staff();
                gSelectedHeadOfDepartmentDoctorStaff = new Staff();
                AcbHeadOfDepartmentDoctorStaff.Text = "";
                AcbDoctorStaff.Text = "";
                V_ResultsEvaluationSelected = new Lookup();
                V_PrognosisSelected = new Lookup();
                PatientRegistration currentpt = temp != null ? temp : CurrentTreatmentProcess.CurPatientRegistration;
                CurrentTreatmentProcess = new TreatmentProcess();
                CurrentTreatmentProcess.CurPatientRegistration = currentpt;
                if(CurrentTreatmentProcess.CurPatientRegistration != null && CurrentTreatmentProcess.CurPatientRegistration.AdmissionInfo != null)
                {
                    CurrentTreatmentProcess.PathologicalProcess = CurrentTreatmentProcess.CurPatientRegistration.AdmissionInfo.PathologicalProcess;
                    CurrentTreatmentProcess.Diagnosis = CurrentTreatmentProcess.CurPatientRegistration.AdmissionInfo.DiagnosisTreatmentSummary;
                    if (CurrentTreatmentProcess.CurPatientRegistration.AdmissionInfo.InPatientDeptDetails != null)
                    {
                        DeptLocation deptLocation = CurrentTreatmentProcess.CurPatientRegistration.AdmissionInfo.InPatientDeptDetails.FirstOrDefault().DeptLocation;
                        if(deptLocation != null)
                        {
                            CurrentTreatmentProcess.DeptName = deptLocation.RefDepartment != null ? deptLocation.RefDepartment.DeptName : "";
                            CurrentTreatmentProcess.LocationName = deptLocation.Location != null ? deptLocation.Location.LocationName : "";
                        }
                    }
                    CurrentTreatmentProcess.BedNumber = Giuong;
                    GetSummaryPCLResultByPtRegistrationID(CurrentTreatmentProcess.CurPatientRegistration.PtRegistrationID);
                }
            }
        }

        public void btnReset_Click()
        {
            Reset_Click(null);
        }

        public void hplEdit_Click(object selectedItem)
        {
            IsUpdate = true;
            if (selectedItem != null)
            {
                TreatmentProcess treatmentProcess = ObjectCopier.DeepCopy(selectedItem as TreatmentProcess);
                CurrentTreatmentProcess = treatmentProcess;
                CurrentTreatmentProcess.CurPatientRegistration = CurrentPatientRegistration != null ? CurrentPatientRegistration : new PatientRegistration();
                gSelectedDoctorStaff = DoctorStaffs != null ? DoctorStaffs.FirstOrDefault(x => x.StaffID == CurrentTreatmentProcess.DoctorStaffID) : null;
                gSelectedHeadOfDepartmentDoctorStaff = DoctorStaffs != null ? DoctorStaffs.FirstOrDefault(x => x.StaffID == CurrentTreatmentProcess.HeadOfDepartmentDoctorStaffID) : null;
                NotifyOfPropertyChange(() => Giuong);
            }
        }

        public void btnPrint_Click(object selectedItem)
        {
            TreatmentProcess treatmentProcess = ObjectCopier.DeepCopy(selectedItem as TreatmentProcess);
            if (treatmentProcess.TreatmentProcessID > 0)
            {
                Action<ICommonPreviewView> onInitDlg = delegate (ICommonPreviewView proAlloc)
                {
                    proAlloc.TreatmentProcessID = treatmentProcess.TreatmentProcessID;
                    proAlloc.eItem = ReportName.XRpt_TreatmentProcessSummary;
                };
                GlobalsNAV.ShowDialog(onInitDlg, null, false, true, Globals.GetDefaultDialogViewSize());
            }
            else
            {
                MessageBox.Show("Chưa có phiếu sơ kết quá trình điều trị!", eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
            }
        }

        public void btnCancel_Click()
        {
            IsUpdate = false;
            gSelectedDoctorStaff = new Staff();
            gSelectedHeadOfDepartmentDoctorStaff = new Staff();
            AcbHeadOfDepartmentDoctorStaff.Text = "";
            AcbDoctorStaff.Text = "";
            V_ResultsEvaluationSelected = new Lookup();
            V_PrognosisSelected = new Lookup();
            CurrentTreatmentProcess = new TreatmentProcess();
            NotifyOfPropertyChange(() => Giuong);
        }

        private bool _IsNeedTreatmentSummary;
        public bool IsNeedTreatmentSummary
        {
            get
            {
                return _IsNeedTreatmentSummary;
            }
            set
            {
                _IsNeedTreatmentSummary = value;
                NotifyOfPropertyChange(() => IsNeedTreatmentSummary);
                NotifyOfPropertyChange(() => IsCanSave);
            }
        }

        private bool _IsUpdate;
        public bool IsUpdate
        {
            get
            {
                return _IsUpdate;
            }
            set
            {
                _IsUpdate = value;
                NotifyOfPropertyChange(() => IsUpdate);
                NotifyOfPropertyChange(() => IsCanSave);
            }
        }

        public bool IsCanSave
        {
            get
            {
                return IsNeedTreatmentSummary || IsUpdate;
            }
        }
    }
}
