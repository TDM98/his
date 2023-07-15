using aEMR.Common;
using aEMR.Common.BaseModel;
using aEMR.Common.ViewModels;
using aEMR.CommonTasks;
using aEMR.Infrastructure;
using aEMR.Infrastructure.CachingUtils;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.ServiceClient.Consultation_PCLs;
using aEMR.ViewContracts;
using Caliburn.Micro;
using Castle.Windsor;
using DataEntities;
using eHCMSLanguage;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

/*
 * 20190927 #001 TBL: BM 0016381: Fix lỗi cảnh báo không cho phép chỉ định DV CLS ở màn hình thông tin chung 
 * 20200708 #002 TTM: BM 0039309: Lỗi không tự động lấy chẩn đoán cuối cùng làm chẩn đoán xuất viện ở màn hình ra toa và màn hình lưu thông tin xuất viện
 * 20210416 #003 TNHX: Thêm tab Dsach BN trả hồ sơ + Ẩn dsach BN DT ngoại trú
 * 20210611 #004 TNHX: 346 Lấy danh sách Rule ICD10
 * 20210713 #005 TNHX: 260 Truyền biến để hiển thị chọn bsi mượn
 * 20210924 #006 TNHX: 571 Thêm màn hình Quản lý điều dưỡng nhập y lệnh
 * 20220126 #007 BLQ: 885 Ẩn tab kết quả hình ảnh và kết quả xét nghiệm. Thêm tab lịch sử bệnh án từ ngoại trú
 * 20220126 #008 BLQ: 885 chuyển 3 tab trong tab điều dưỡng thực hiện y lệnh ra tab điều dưỡng
 * 20221108 #009 BLQ: Thêm màn hình hiển thị kết quả CLS nhập từ màn hình nhập viện
 * 20230105 #010 BLQ: Thêm tab phiếu theo dõi dịch truyền
 * 20230502 #011 QTD: Thêm tab Sơ kết 15 ngày điều trị
 */

namespace aEMR.Consultation.ViewModels
{
    [Export(typeof(IConsultationsSummary_InPt)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class ConsultationsSummary_InPtViewModel : ViewModelBase, IConsultationsSummary_InPt
        , IHandle<ReloadDiagnosisTreatmentTree>
    {
        #region Propterties
        private Registration_DataStorage _Registration_DataStorage;
        public IDiagnosisTreatmentTree DiagnosisTreatmentTree { get; set; }
        public IConsultations_InPt UCConsultationsInPt { get; set; }
        public IPtDashboardSummary UCPtDashboardSummary { get; set; }
        public IeInPrescriptions UCInPrescriptions { get; set; }
        public IDischargeNew UCDischarge { get; set; }
        public IInPatientInstruction UCInPatientInstruction { get; set; }
        private IInPatientOutstandingTask UCInPatientOutstandingTask { get; set; }
        //private IInPatientInstructionOutstandingTask UCInPatientInstructionOutstandingTask { get; set; }
        public IPatientPCLRequest UCPatientPCLRequest { get; set; }
        public IPatientPCLRequestImage UCPatientPCLRequestImage { get; set; }
        //▼====: #007
        public IPatientTreeForm UCPatientTreeForm { get; set; }
        //public IPatientPCLLaboratoryResult UCPatientPCLLaboratoryResult { get; set; }
        //public IPatientPCLImagingResult UCPatientPCLImagingResult { get; set; }
        //▲====: #007
        public IPhysicalExamination UCPhysicalExamination { get; set; }
        public INutritionalRating UCNutritionalRating { get; set; }
        public IChildListing UCChildListing { get; set; }
        public IExamInformationPatients UCExamInformationPatient { get; set; }
        public ILoginInfo UCDoctorProfileInfo { get; set; }
        public IPatientTreeForm UCPatientTreeFormV2 { get; set; }
        //▼====: #010
        public ITransmissionMonitor UCTransmissionMonitor { get; set; }
        //▲====: #010
        //▼====: #011
        public ITreatmentProcessSummary UCTreatmentProcessSummary { get; set; }
        //▲====: #011
        public Registration_DataStorage Registration_DataStorage
        {
            get
            {
                return _Registration_DataStorage;
            }
            set
            {
                if (_Registration_DataStorage == value)
                {
                    return;
                }
                _Registration_DataStorage = value;
                NotifyOfPropertyChange(() => Registration_DataStorage);
                UCConsultationsInPt.Registration_DataStorage = Registration_DataStorage;
                UCInPrescriptions.Registration_DataStorage = Registration_DataStorage;
                UCDischarge.Registration_DataStorage = Registration_DataStorage;
                UCPatientPCLRequest.Registration_DataStorage = Registration_DataStorage;
                UCPatientPCLRequestImage.Registration_DataStorage = Registration_DataStorage;
                //▼====: #007
                UCPatientTreeForm.Registration_DataStorage = Registration_DataStorage;
                //UCPatientPCLLaboratoryResult.Registration_DataStorage = Registration_DataStorage;
                //UCPatientPCLImagingResult.Registration_DataStorage = Registration_DataStorage;
                //▲====: #007
                UCPtDashboardSummary.Registration_DataStorage = Registration_DataStorage;
                UCInPatientInstruction.Registration_DataStorage = Registration_DataStorage;
                if (DiagnosisTreatmentTree != null)
                {
                    DiagnosisTreatmentTree.Registration_DataStorage = Registration_DataStorage;
                }
                UCPatientProfileInfo.Registration_DataStorage = Registration_DataStorage;
                UCHeaderInfoPMR.Registration_DataStorage = Registration_DataStorage;
                UCPtRegDetailInfo.Registration_DataStorage = Registration_DataStorage;
                UCPhysicalExamination.Registration_DataStorage = Registration_DataStorage;
                UCNutritionalRating.Registration_DataStorage = Registration_DataStorage;
                UCChildListing.Registration_DataStorage = Registration_DataStorage;
                UCExamInformationPatient.Registration_DataStorage = Registration_DataStorage;
                UCCheckMedicalFileHistory.Registration_DataStorage = Registration_DataStorage;
                UCDocumentAccordingMOH.Registration_DataStorage = Registration_DataStorage;
                //▼====: #008
                //▼====: #006
                //UCExecuteDoctorInstruction.Registration_DataStorage = Registration_DataStorage;
                //▲====: #006
                UCTicketCareListFind.Registration_DataStorage = Registration_DataStorage;
                UCExecuteDrugListFind.Registration_DataStorage = Registration_DataStorage;
                UCVitalSignPreview.Registration_DataStorage = Registration_DataStorage;
                //▲====: #008
                UCPatientTreeFormV2.Registration_DataStorage = Registration_DataStorage;
                //▼====: #010
                UCTransmissionMonitor.Registration_DataStorage = Registration_DataStorage;
                //▲====: #010
            }
        }
        private object CurrentOutstandingTask { get; set; }
        public ISearchPatientAndRegistration UCSearchRegistrationContent { get; set; }
        public IPatientInfo UCPatientProfileInfo { get; set; }
        public IPatientMedicalRecords_ByPatientID UCHeaderInfoPMR { get; set; }
        public IPtRegDetailInfo UCPtRegDetailInfo { get; set; }
        public ICheckMedicalFileHistory UCCheckMedicalFileHistory { get; set; }
        public IDocumentAccordingMOH UCDocumentAccordingMOH { get; set; }
        //▼====: #008
        //▼====: #006
        //public IExecuteDoctorInstruction UCExecuteDoctorInstruction { get; set; }
        //▲====: #006
        public ITicketCareListFind UCTicketCareListFind { get; set; }
        public IExecuteDrugListFind UCExecuteDrugListFind { get; set; }
        public IVitalSignPreview UCVitalSignPreview { get; set; }
        //▲====: #008
        private bool _IsUpdateDiagConfirmInPT;
        public bool IsUpdateDiagConfirmInPT
        {
            get { return _IsUpdateDiagConfirmInPT; }
            set
            {
                if (_IsUpdateDiagConfirmInPT != value)
                {
                    _IsUpdateDiagConfirmInPT = value;
                    UCConsultationsInPt.IsUpdateDiagConfirmInPT = IsUpdateDiagConfirmInPT;
                    UCInPatientInstruction.IsUpdateDiagConfirmInPT = IsUpdateDiagConfirmInPT;
                    UCInPrescriptions.IsUpdateDiagConfirmInPT = IsUpdateDiagConfirmInPT;
                    NotifyOfPropertyChange(() => IsUpdateDiagConfirmInPT);
                }
            }
        }
        private string _Title;
        public string Title
        {
            get { return _Title; }
            set
            {
                if (_Title != value)
                {
                    _Title = value;
                    NotifyOfPropertyChange(() => Title);
                }
            }
        }
        #endregion
        #region Events
        [ImportingConstructor]
        public ConsultationsSummary_InPtViewModel(IWindsorContainer aContainer, INavigationService aNavigation, ISalePosCaching aCaching)
        {
            //Globals.LeftModuleActive = LeftModuleActive.KHAMBENH_CHANDOAN_NOITRU;
            UCPtDashboardSummary = Globals.GetViewModel<IPtDashboardSummary>();
            UCPtDashboardSummary.V_RegistrationType = (long)AllLookupValues.RegistrationType.NOI_TRU;
            Globals.LeftModuleActive = LeftModuleActive.KHAMBENH_CHANDOAN_NOITRU;
            UCConsultationsInPt = Globals.GetViewModel<IConsultations_InPt>();
            UCConsultationsInPt.IsDailyDiagnosis = true;
            UCConsultationsInPt.IsShowSummaryContent = false;
            UCConsultationsInPt.CallSetInPatientInfoAndRegistrationForePresciption_InPt += () =>
            {
                UCPatientProfileInfo.InitData();
            };
            UCInPrescriptions = Globals.GetViewModel<IeInPrescriptions>();
            UCDischarge = Globals.GetViewModel<IDischargeNew>();
            UCDischarge.IsConsultation = true;
            //UCDischarge.FromDoctorView = true;
            UCDischarge.InitView(true);
            UCInPatientInstruction = Globals.GetViewModel<IInPatientInstruction>();
            UCInPatientOutstandingTask = Globals.GetViewModel<IInPatientOutstandingTask>();
            UCInPatientOutstandingTask.WhichVM = SetOutStandingTask.KHAMBENH;
            //▼====: #003
            UCInPatientOutstandingTask.IsShowListInPatientReturnRecord = true;
            UCInPatientOutstandingTask.IsShowListOutPatientList = false;
            //▲====: #003
            //UCInPatientInstructionOutstandingTask = Globals.GetViewModel<IInPatientInstructionOutstandingTask>();
            UCPatientPCLRequest = Globals.GetViewModel<IPatientPCLRequest>();
            UCPatientPCLRequest.IsShowCheckBoxPayAfter = false;
            UCPatientPCLRequest.IsShowSummaryContent = false;
            UCPatientPCLRequest.V_RegistrationType = (long)AllLookupValues.RegistrationType.NOI_TRU; //20190927 TBL: Nếu vào màn hình khám bệnh nội trú thì set V_RegistrationType là nội trú
            UCPatientPCLRequestImage = Globals.GetViewModel<IPatientPCLRequestImage>();
            UCPatientPCLRequestImage.IsShowCheckBoxPayAfter = false;
            UCPatientPCLRequestImage.IsShowSummaryContent = false;
            UCPatientPCLRequestImage.V_RegistrationType = (long)AllLookupValues.RegistrationType.NOI_TRU; //20190927 TBL: Nếu vào màn hình khám bệnh nội trú thì set V_RegistrationType là nội trú
            //▼====: #007
            UCPatientTreeForm = Globals.GetViewModel<IPatientTreeForm>();
            UCPatientTreeForm.IsShowSummaryContent = false;
            //UCPatientPCLLaboratoryResult = Globals.GetViewModel<IPatientPCLLaboratoryResult>();
            //UCPatientPCLLaboratoryResult.IsShowSummaryContent = false;
            //UCPatientPCLImagingResult = Globals.GetViewModel<IPatientPCLImagingResult>();
            //UCPatientPCLImagingResult.V_RegistrationType = (long)AllLookupValues.RegistrationType.NOI_TRU;
            //UCPatientPCLImagingResult.IsShowSummaryContent = false;
            //▲====: #007
            UCSearchRegistrationContent = Globals.GetViewModel<ISearchPatientAndRegistration>();
            if (Globals.PatientFindBy_ForConsultation == null)
            {
                Globals.PatientFindBy_ForConsultation = AllLookupValues.PatientFindBy.NGOAITRU;
            }
            UCSearchRegistrationContent.PatientFindBy = Globals.PatientFindBy_ForConsultation.Value;
            UCSearchRegistrationContent.CloseRegistrationFormWhenCompleteSelection = false;
            UCSearchRegistrationContent.InitButtonVisibility(SearchRegButtonsVisibility.SHOW_SEARCH_REG_BTN);
            UCSearchRegistrationContent.SetDefaultButton(SearchRegistrationButtons.SEARCH_REGISTRATION);
            UCSearchRegistrationContent.IsSearchGoToKhamBenh = true;
            UCSearchRegistrationContent.PatientFindByVisibility = true;
            UCSearchRegistrationContent.CanSearhRegAllDept = true;
            UCSearchRegistrationContent.SearchAdmittedInPtRegOnly = true;
            if (!Globals.ServerConfigSection.ConsultationElements.IsAllowSearchingPtByName)
            {
                UCSearchRegistrationContent.IsAllowSearchingPtByName_Visible = true;
                UCSearchRegistrationContent.IsSearchPtByNameChecked = false;
            }
            UCSearchRegistrationContent.PatientFindBy = AllLookupValues.PatientFindBy.NOITRU;
            //▼====: #005
            UCSearchRegistrationContent.IsShowBtnChooseUserOfficial = Globals.ServerConfigSection.CommonItems.AllowToBorrowDoctorAccount;
            //▲====: #005

            UCPatientProfileInfo = Globals.GetViewModel<IPatientInfo>();
            UCDoctorProfileInfo = Globals.GetViewModel<ILoginInfo>();
            UCDoctorProfileInfo.isPreNoteTemp = true;
            UCHeaderInfoPMR = Globals.GetViewModel<IPatientMedicalRecords_ByPatientID>();
            UCPtRegDetailInfo = Globals.GetViewModel<IPtRegDetailInfo>();
            UCPhysicalExamination = Globals.GetViewModel<IPhysicalExamination>();
            UCPhysicalExamination.V_RegistrationType = (long)AllLookupValues.RegistrationType.NOI_TRU;
            UCNutritionalRating = Globals.GetViewModel<INutritionalRating>();
            UCChildListing = Globals.GetViewModel<IChildListing>();
            UCExamInformationPatient = Globals.GetViewModel<IExamInformationPatients>();
            UCCheckMedicalFileHistory = Globals.GetViewModel<ICheckMedicalFileHistory>();
            UCCheckMedicalFileHistory.V_RegistrationType = (long)AllLookupValues.RegistrationType.NOI_TRU;
            UCDocumentAccordingMOH = Globals.GetViewModel<IDocumentAccordingMOH>();
            //▼====: #008
            //▼====: #006
            //UCExecuteDoctorInstruction = Globals.GetViewModel<IExecuteDoctorInstruction>();
            //▲====: #006
            UCTicketCareListFind = Globals.GetViewModel<ITicketCareListFind>();
            UCExecuteDrugListFind = Globals.GetViewModel<IExecuteDrugListFind>();
            UCVitalSignPreview = Globals.GetViewModel<IVitalSignPreview>();
            //▲====: #008
            UCPatientTreeFormV2 = Globals.GetViewModel<IPatientTreeForm>();
            UCPatientTreeFormV2.IsShowSummaryContent = false;
            UCPatientTreeFormV2.IsCriterion_PCLResult = true;
            //▼====: #010
            UCTransmissionMonitor = Globals.GetViewModel<ITransmissionMonitor>();
            //▲====: #010
            //▼====: #011
            UCTreatmentProcessSummary = Globals.GetViewModel<ITreatmentProcessSummary>();
            //▲====: #011
            Registration_DataStorage = new Registration_DataStorage();
        }
        protected override void OnActivate()
        {
            base.OnActivate();
            Globals.EventAggregator.Subscribe(this);

            var HomeView = Globals.GetViewModel<IHome>();
            DiagnosisTreatmentTree = Globals.GetViewModel<IDiagnosisTreatmentTree>();
            DiagnosisTreatmentTree.gTreatmentCollectionOnChanged = new TreatmentCollectionOnChanged((aCollection) =>
            {
                UCDischarge.ApplyDiagnosisTreatmentCollection(aCollection);
                UCInPrescriptions.ApplyDiagnosisTreatmentCollection(aCollection);
            });
            HomeView.LeftMenu = DiagnosisTreatmentTree;
            ActivateItem(DiagnosisTreatmentTree);
            HomeView.IsEnableLeftMenu = true;
            HomeView.IsExpandLeftMenu = true;
            HomeView.OutstandingTaskContent = UCInPatientOutstandingTask;
            HomeView.IsExpandOST = true;
            CurrentOutstandingTask = UCInPatientOutstandingTask;

            ActivateItem(UCPtDashboardSummary);
            ActivateItem(UCConsultationsInPt);
            ActivateItem(UCInPrescriptions);
            ActivateItem(UCDischarge);
            ActivateItem(UCInPatientInstruction);
            ActivateItem(UCInPatientOutstandingTask);
            //ActivateItem(UCInPatientInstructionOutstandingTask);
            //▼====: #007
            ActivateItem(UCPatientTreeForm);
            //ActivateItem(UCPatientPCLLaboratoryResult);
            //ActivateItem(UCPatientPCLImagingResult);
            //▲====: #007
            ActivateItem(UCPatientPCLRequest);
            ActivateItem(UCPatientPCLRequestImage);
            ActivateItem(UCSearchRegistrationContent);
            ActivateItem(UCPatientProfileInfo);
            ActivateItem(UCDoctorProfileInfo);
            ActivateItem(UCHeaderInfoPMR);
            ActivateItem(UCPtRegDetailInfo);
            ActivateItem(UCPhysicalExamination);
            ActivateItem(UCNutritionalRating);
            ActivateItem(UCChildListing);
            ActivateItem(UCExamInformationPatient);
            ActivateItem(UCCheckMedicalFileHistory);
            ActivateItem(UCDocumentAccordingMOH);
            //▼====: #008
            //▼====: #006
            //ActivateItem(UCExecuteDoctorInstruction);
            //▲====: #006
            ActivateItem(UCTicketCareListFind);
            ActivateItem(UCExecuteDrugListFind);
            ActivateItem(UCVitalSignPreview);
            //▲====: #008
            ActivateItem(UCPatientTreeFormV2);
            //▼====: #010
            ActivateItem(UCTransmissionMonitor);
            //▲====: #010
            //▼====: #011
            ActivateItem(UCTreatmentProcessSummary);
            //▲====: #011
        }
        protected override void OnDeactivate(bool close)
        {
            Registration_DataStorage = null;
            var HomeView = Globals.GetViewModel<IHome>();
            HomeView.OutstandingTaskContent = null;
            Globals.EventAggregator.Unsubscribe(this);
            HomeView.OutstandingTaskContent = null;
            HomeView.IsEnableLeftMenu = false;
            HomeView.IsExpandLeftMenu = false;
            HomeView.IsExpandOST = false;
            base.OnDeactivate(close);
        }
        public void TCMain_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //if ((sender as TabControl).SelectedItem != null && ((sender as TabControl).SelectedItem as TabItem).Name == "TIInPatientInstruction" && !(CurrentOutstandingTask is IInPatientInstructionOutstandingTask))
            //{
            //    UCInPatientOutstandingTask.IsInstructionVisible = true;
            //}
            //else
            //{
            //    UCInPatientOutstandingTask.IsInstructionVisible = false;
            //}
            //▼===== #001
            if ((sender as TabControl).SelectedItem != null && ((sender as TabControl).SelectedItem as TabItem).Name == "PCLRequest")
            {
                UCPatientPCLRequest.CheckDeptLocation();
            }
            else if ((sender as TabControl).SelectedItem != null && ((sender as TabControl).SelectedItem as TabItem).Name == "PCLRequestImage")
            {
                UCPatientPCLRequestImage.CheckDeptLocation();
            }
            //▲===== #001
            //▼===== #002
            else if ((sender as TabControl).SelectedItem != null && ((sender as TabControl).SelectedItem as TabItem).Name == "TInPrescriptions")
            {
                UCInPrescriptions.SetLastDiagnosisForConfirm();
            }
            else if ((sender as TabControl).SelectedItem != null && ((sender as TabControl).SelectedItem as TabItem).Name == "TInDischarge")
            {
                UCDischarge.SetLastDiagnosisForConfirm();
            }
            //▲===== #002
        }
        #endregion
        #region Methods
        private IEnumerator<IResult> SetInPatientInfoForConsultation(PatientRegistration aRegistration, PatientRegistrationDetail aRegistrationDetail)
        {
            if (aRegistrationDetail == null)
            {
                aRegistrationDetail = new PatientRegistrationDetail();
            }
            Registration_DataStorage = new Registration_DataStorage();
            yield return GenericCoRoutineTask.StartTask(InitPhyExamAction, aRegistration);
            yield return GenericCoRoutineTask.StartTask(GetInPtServiceRecordForKhamBenhAction, aRegistration);
            yield return GenericCoRoutineTask.StartTask(GetRegistrationInPtAction, aRegistration, aRegistrationDetail);
            yield return GenericCoRoutineTask.StartTask(LoadGetListRequiredSubDiseasesReferences);
            yield return GenericCoRoutineTask.StartTask(LoadGetListRuleDiseasesReferences);
        }
        private IEnumerator<IResult> PatientServiceRecordsGetForKhamBenh_Ext()
        {
            yield return GenericCoRoutineTask.StartTask(GetInPtServiceRecordForKhamBenhAction, Registration_DataStorage.CurrentPatientRegistration);
            PublishEventGlobalsPSRLoad(true);
        }
        private void InitPhyExamAction(GenericCoRoutineTask genTask, object ObjPtRegistration)
        {
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new SummaryServiceClient())
                    {
                        bool bContinue = true;

                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetPhyExam_ByPtRegID(((PatientRegistration)ObjPtRegistration).PtRegistrationID, (long)((PatientRegistration)ObjPtRegistration).V_RegistrationType, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                Globals.curPhysicalExamination = contract.EndGetPhyExam_ByPtRegID(asyncResult);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
                                ClientLoggerHelper.LogError(ex.Message);
                                bContinue = false;
                            }
                            finally
                            {
                                genTask.ActionComplete(bContinue);
                                this.HideBusyIndicator();
                            }

                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
                    ClientLoggerHelper.LogError(ex.Message);
                    genTask.ActionComplete(false);
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }
        private void GetInPtServiceRecordForKhamBenhAction(GenericCoRoutineTask genTask, object ObjPtRegistration)
        {
            this.ShowBusyIndicator();
            PatientServiceRecord psrSearch = new PatientServiceRecord();
            psrSearch.PtRegistrationID = ((PatientRegistration)ObjPtRegistration).PtRegistrationID;
            psrSearch.V_RegistrationType = ((PatientRegistration)ObjPtRegistration).V_RegistrationType;
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new CommonUtilsServiceClient())
                    {
                        bool bContinue = true;
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginPatientServiceRecordsGetForKhamBenh_InPt(psrSearch,
                        Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var psr = contract.EndPatientServiceRecordsGetForKhamBenh_InPt(asyncResult);
                                Registration_DataStorage.PatientServiceRecordCollection = new ObservableCollection<PatientServiceRecord>(psr);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
                                ClientLoggerHelper.LogError(ex.Message);
                                bContinue = false;
                            }
                            finally
                            {
                                genTask.ActionComplete(bContinue);
                                this.HideBusyIndicator();
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
                    ClientLoggerHelper.LogError(ex.Message);
                    genTask.ActionComplete(false);
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }
        private void GetRegistrationInPtAction(GenericCoRoutineTask aGenTask, object ObjPtRegistration, object aPatientRegistrationDetail)
        {
            this.ShowBusyIndicator();
            LoadRegistrationSwitch CurrentSwitch = new LoadRegistrationSwitch();
            CurrentSwitch.IsGetAdmissionInfo = true;
            CurrentSwitch.IsGetBedAllocations = true;
            var CurrentThread = new Thread(() =>
            {
                try
                {
                    using (var aFactory = new PatientRegistrationServiceClient())
                    {
                        bool mIsContinue = true;
                        var aContract = aFactory.ServiceInstance;
                        //KMx: Chuyển hàm, lý do cần lấy thêm AdmissionInfo để khi ra toa xuất viện lọc lại các khoa BN đã nằm (07/03/2015 10:21)
                        //contract.BeginGetRegistration(((PatientRegistration)ObjPtRegistration).PtRegistrationID, (int)Globals.PatientFindBy_ForConsultation.Value,
                        //Globals.DispatchCallback((asyncResult) =>
                        aContract.BeginGetRegistrationInfo_InPt(((PatientRegistration)ObjPtRegistration).PtRegistrationID, (int)Globals.PatientFindBy_ForConsultation.Value, CurrentSwitch, false, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                PatientRegistration mRegistration = aContract.EndGetRegistrationInfo_InPt(asyncResult);
                                mRegistration.Patient.CurrentHealthInsurance = mRegistration.HealthInsurance;
                                Registration_DataStorage.CurrentPatientRegistration = mRegistration;
                                Registration_DataStorage.CurrentPatientRegistrationDetail = (aPatientRegistrationDetail as PatientRegistrationDetail);
                                UCInPatientInstruction.LoadRegistrationInfo(mRegistration);
                                DiagnosisTreatmentTree.LoadData(mRegistration.PtRegistrationID);
                                //▼====: #007
                                //▼===== 20190927 TTM:  Do thiếu set dữ liệu cho 2 màn hình kết quả => luôn lấy giá trị mặc định tìm kiếm => Sai => Bổ sung việc gọi hàm set giá trị.
                                //UCPatientPCLImagingResult.InitPatientInfo(mRegistration.Patient);
                                //UCPatientPCLLaboratoryResult.InitPatientInfo(mRegistration.Patient);
                                //▲===== 
                                //▲====: #007
                                UCPhysicalExamination.InitPatientInfo();
                                //Globals.SetInfoPatient(mRegistration.Patient, mRegistration, null);
                                UCCheckMedicalFileHistory.InitPatientInfo();
                                UCDocumentAccordingMOH.InitPatientInfo(mRegistration);
                                //▼====: #008
                                //▼====: #006
                                //UCExecuteDoctorInstruction.InitPatientInfo();
                                //▲====: #006
                                UCTicketCareListFind.InitPatientInfo();
                                UCExecuteDrugListFind.InitPatientInfo();
                                UCVitalSignPreview.InitPatientInfo();
                                //▲====: #008
                                //▼====: #010
                                UCTransmissionMonitor.LoadRegistrationInfo(mRegistration);
                                //▲====: #010
                                //KMx: Hiển thị thông tin bệnh nhân (10/10/2014 16:45).
                                Globals.EventAggregator.Publish(new ShowInPatientInfoForConsultation() { Patient = mRegistration.Patient, PtRegistration = mRegistration });
                                Globals.EventAggregator.Publish(new SetInPatientInfoAndRegistrationForConsultations_InPt() { MedServiceID = (aPatientRegistrationDetail as PatientRegistrationDetail).MedServiceID.GetValueOrDefault(0) });
                                Globals.EventAggregator.Publish(new SetInPatientInfoAndRegistrationForePresciption_InPt() { });
                                UCPatientProfileInfo.InitData();
                                Globals.EventAggregator.Publish(new SetInPatientInfoAndRegistrationForInPatientDischarge() { });
                                Globals.EventAggregator.Publish(new SetInPatientInfoAndRegistrationForPatientPCLRequest() { });
                                Globals.EventAggregator.Publish(new SetInPatientInfoAndRegistrationForPatientPCLRequestImage() { });
                                Globals.EventAggregator.Publish(new ShowSummaryInPT() { Patient = mRegistration.Patient });
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
                                ClientLoggerHelper.LogError(ex.Message);
                                mIsContinue = false;
                            }
                            finally
                            {
                                aGenTask.ActionComplete(mIsContinue);
                                this.HideBusyIndicator();
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
                    ClientLoggerHelper.LogError(ex.Message);
                    aGenTask.ActionComplete(false);
                    this.HideBusyIndicator();
                }
            });
            CurrentThread.Start();
        }
        public void CallSetInPatientInfoForConsultation(PatientRegistration aRegistration, PatientRegistrationDetail aRegistrationDetail)
        {
            if (aRegistration == null)
            {
                return;
            }
            Coroutine.BeginExecute(SetInPatientInfoForConsultation(aRegistration, aRegistrationDetail));
        }
        public void CallPatientServiceRecordsGetForKhamBenh_Ext()
        {
            Coroutine.BeginExecute(PatientServiceRecordsGetForKhamBenh_Ext());
        }
        public void PublishEventGlobalsPSRLoad(bool IsReloadPrescript, bool bAllowModifyPrescription = false)
        {
            Globals.EventAggregator.Publish(new GlobalCurPatientServiceRecordLoadComplete_Consult_InPt());
            Globals.EventAggregator.Publish(new LoadPrescriptionInPtAfterSaved());
        }
        #endregion
        //▼===== #002
        public void Handle(ReloadDiagnosisTreatmentTree message)
        {
            if (message != null && message.gRegistration != null)
            {
                DiagnosisTreatmentTree.LoadData(message.gRegistration.PtRegistrationID);
                DiagnosisTreatmentTree.gTreatmentCollectionOnChanged = new TreatmentCollectionOnChanged((aCollection) =>
                {
                    UCDischarge.ApplyDiagnosisTreatmentCollection(aCollection);
                    UCInPrescriptions.ApplyDiagnosisTreatmentCollection(aCollection);
                });
            }
        }
        //▲===== #002
        //▼===== #002
        private void LoadGetListRequiredSubDiseasesReferences(GenericCoRoutineTask aGenTask)
        {
            this.ShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
            var CurrentThread = new Thread(() =>
            {
                try
                {
                    using (var aFactory = new ePMRsServiceClient())
                    {
                        bool mIsContinue = true;
                        var aContract = aFactory.ServiceInstance;
                        aContract.BeginGetListRequiredSubDiseasesReferences("*", Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                ListRequiredSubDiseasesReferences = aContract.EndGetListRequiredSubDiseasesReferences(asyncResult).ToList();
                                if (Registration_DataStorage != null && ListRequiredSubDiseasesReferences != null)
                                {
                                    Registration_DataStorage.ListRequiredSubDiseasesReferences = ListRequiredSubDiseasesReferences;
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
                                mIsContinue = false;
                            }
                            finally
                            {
                                aGenTask.ActionComplete(mIsContinue);
                                this.HideBusyIndicator();
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
                    ClientLoggerHelper.LogError(ex.Message);
                    aGenTask.ActionComplete(false);
                    this.HideBusyIndicator();
                }
            });
            CurrentThread.Start();
        }
        public List<RequiredSubDiseasesReferences> ListRequiredSubDiseasesReferences { get; set; }
        //▲===== #002
        //▼===== #004
        private void LoadGetListRuleDiseasesReferences(GenericCoRoutineTask aGenTask)
        {
            this.ShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
            var CurrentThread = new Thread(() =>
            {
                try
                {
                    using (var aFactory = new ePMRsServiceClient())
                    {
                        bool mIsContinue = true;
                        var aContract = aFactory.ServiceInstance;
                        aContract.BeginGetListRuleDiseasesReferences("*", Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                ListRuleDiseasesReferences = aContract.EndGetListRuleDiseasesReferences(asyncResult).ToList();
                                if (Registration_DataStorage != null && ListRuleDiseasesReferences != null)
                                {
                                    Registration_DataStorage.ListRuleDiseasesReferences = ListRuleDiseasesReferences;
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
                                mIsContinue = false;
                            }
                            finally
                            {
                                aGenTask.ActionComplete(mIsContinue);
                                this.HideBusyIndicator();
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
                    ClientLoggerHelper.LogError(ex.Message);
                    aGenTask.ActionComplete(false);
                    this.HideBusyIndicator();
                }
            });
            CurrentThread.Start();
        }
        public List<RuleDiseasesReferences> ListRuleDiseasesReferences { get; set; }
        //▲===== #004
    }
}
