using System;
using System.ComponentModel.Composition;
using aEMR.Infrastructure;
using aEMR.ViewContracts;
using DataEntities;
using Caliburn.Micro;
using aEMR.Infrastructure.Events;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using aEMR.ServiceClient.Consultation_PCLs;
using eHCMSLanguage;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.Windows;
using aEMR.ServiceClient;
using aEMR.CommonTasks;
using aEMR.Common;
using aEMR.ViewContracts.Configuration;
using aEMR.Infrastructure.CachingUtils;
using aEMR.Controls;
using Castle.Windsor;
using Castle.Core.Logging;
/*
 * 20180529 #001 TBLD: Them rang buoc trong tab thong tin ho so 
 * 20180913 #002 TBL: Khi bam enter tim benh nhan
 * 20211004 #003 TNHX: Lọc danh sách bsi theo cấu hình trách nhiệm + theo tích tạm khóa
*/
namespace aEMR.ConsultantEPrescription.ViewModels
{
    [Export(typeof(IConsultingDiagnosys)), PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class ConsultingDiagnosysViewModel : Conductor<object>, IConsultingDiagnosys
        , IHandle<RegistrationSelectedToHoiChan>
        , IHandle<ConsultingDiagnosys<Patient>>
        , IHandle<ConsultingDiagnosys<ConsultingDiagnosys>>
        , IHandle<ItemSelected<Patient>>
    {
        [ImportingConstructor]
        public ConsultingDiagnosysViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            Globals.EventAggregator.Subscribe(this);

            CheckAuthorization();

            SearchRegistrationContent = Globals.GetViewModel<ISearchPatientAndRegistration>();
            if (Globals.PatientFindBy_ForConsultation == null)
            {
                Globals.PatientFindBy_ForConsultation = AllLookupValues.PatientFindBy.NGOAITRU;
            }
            SearchRegistrationContent.PatientFindBy = AllLookupValues.PatientFindBy.NOITRU;
            SearchRegistrationContent.CloseRegistrationFormWhenCompleteSelection = false;
            SearchRegistrationContent.InitButtonVisibility(SearchRegButtonsVisibility.SHOW_SEARCH_PATIENT_BTN);
            /*▼====: #002*/
            SearchRegistrationContent.SetDefaultButton(SearchRegistrationButtons.SEARCH_PATIENT);
            /*▲====: #002*/
            SearchRegistrationContent.IsSearchGoToKhamBenh = true;
            SearchRegistrationContent.PatientFindByVisibility = false;
            SearchRegistrationContent.CanSearhRegAllDept = true;
            SearchRegistrationContent.SearchAdmittedInPtRegOnly = true;
            SearchRegistrationContent.EnableSerchConsultingDiagnosy = true;
            SearchRegistrationContent.IsConsultingHistoryView = true;
            ActivateItem(SearchRegistrationContent);

            PatientSummaryInfoContent = Globals.GetViewModel<IPatientSummaryInfoV2>();
            PatientSummaryInfoContent.mInfo_CapNhatThongTinBN = true;
            PatientSummaryInfoContent.DisplayButtons = false;
            ActivateItem(PatientSummaryInfoContent);

            DiagnosticTypeArray = Globals.AllLookupValueList.Where(x => x.ObjectName == "V_DiagnosticType").ToList();
            TreatmentMethodArray = Globals.AllLookupValueList.Where(x => x.ObjectName == "V_TreatmentMethod").ToList();
            HeartSurgicalTypeArray = Globals.AllLookupValueList.Where(x => x.ObjectName == "V_HeartSurgicalType").ToList();
            ValveTypeArray = Globals.AllLookupValueList.Where(x => x.ObjectName == "V_ValveType").ToList();
            DischargeTypeArray = Globals.AllLookupValueList.Where(x => x.ObjectName == "V_DischargeType").ToList();

            UCGeneral = Globals.GetViewModel<IConsultingDiagnosysGeneral>();
            UCGeneral.gConsultingDiagnosys = gConsultingDiagnosys;
            UCGeneral.DiagnosticTypeArray = DiagnosticTypeArray;
            UCGeneral.TreatmentMethodArray = TreatmentMethodArray;
            UCGeneral.HeartSurgicalTypeArray = HeartSurgicalTypeArray;
            UCGeneral.ValveTypeArray = ValveTypeArray;
            //▼====: #003
            DoctorStaffs = Globals.AllStaffs.Where(x => x.StaffCatgID == (long)StaffCatg.Bs && !x.IsStopUsing).ToList();
            //▲====: #003

            UCResult = Globals.GetViewModel<IConsultingDiagnosysHistory>();
            UCResult.gConsultingDiagnosys = gConsultingDiagnosys;
            UCResult.DischargeTypeArray = DischargeTypeArray;
            UCResult.DoctorStaffs = DoctorStaffs;
        }
        protected override void OnActivate()
        {
            Globals.EventAggregator.Subscribe(this);
            base.OnActivate();
        }
        protected override void OnDeactivate(bool close)
        {
            Globals.EventAggregator.Unsubscribe(this);
            base.OnDeactivate(close);
        }
        #region Properties
        public ISearchPatientAndRegistration SearchRegistrationContent { get; set; }
        public IPatientSummaryInfoCommon PatientSummaryInfoContent { get; set; }
        /*▼====: #001*/
        private ConsultingDiagnosys origConsultingDiagnosys = null;
        /*▲====: #001*/
        private ConsultingDiagnosys _ConsultingDiagnosys = new ConsultingDiagnosys();
        public ConsultingDiagnosys gConsultingDiagnosys
        {
            get
            {
                return _ConsultingDiagnosys;
            }
            set
            {
                _ConsultingDiagnosys = value;
                NotifyOfPropertyChange(() => gConsultingDiagnosys);
            }
        }
        private List<Lookup> _DiagnosticTypeArray;
        public List<Lookup> DiagnosticTypeArray
        {
            get
            {
                return _DiagnosticTypeArray;
            }
            set
            {
                _DiagnosticTypeArray = value;
                NotifyOfPropertyChange(() => DiagnosticTypeArray);
            }
        }
        private List<Lookup> _TreatmentMethodArray;
        public List<Lookup> TreatmentMethodArray
        {
            get
            {
                return _TreatmentMethodArray;
            }
            set
            {
                _TreatmentMethodArray = value;
                NotifyOfPropertyChange(() => TreatmentMethodArray);
            }
        }
        private List<Lookup> _HeartSurgicalTypeArray;
        public List<Lookup> HeartSurgicalTypeArray
        {
            get
            {
                return _HeartSurgicalTypeArray;
            }
            set
            {
                _HeartSurgicalTypeArray = value;
                NotifyOfPropertyChange(() => HeartSurgicalTypeArray);
            }
        }
        private List<Lookup> _ValveTypeArray;
        public List<Lookup> ValveTypeArray
        {
            get
            {
                return _ValveTypeArray;
            }
            set
            {
                _ValveTypeArray = value;
                NotifyOfPropertyChange(() => ValveTypeArray);
            }
        }
        private List<Lookup> _DischargeTypeArray;
        public List<Lookup> DischargeTypeArray
        {
            get
            {
                return _DischargeTypeArray;
            }
            set
            {
                _DischargeTypeArray = value;
                NotifyOfPropertyChange(() => DischargeTypeArray);
            }
        }
        public IConsultingDiagnosysGeneral UCGeneral { get; set; }
        public IConsultingDiagnosysHistory UCResult { get; set; }
        private List<Staff> _DoctorStaffs;
        public List<Staff> DoctorStaffs
        {
            get
            {
                return _DoctorStaffs;
            }
            set
            {
                _DoctorStaffs = value;
                NotifyOfPropertyChange(() => DoctorStaffs);
            }
        }
        public ValidationSummary gValidationSummary { get; set; }
        private bool _mConsultingDiagnosysEditAuth = true;
        public bool mConsultingDiagnosysEditAuth
        {
            get
            {
                return _mConsultingDiagnosysEditAuth;
            }
            set
            {
                if (_mConsultingDiagnosysEditAuth == value) return;
                _mConsultingDiagnosysEditAuth = value;
                NotifyOfPropertyChange(() => mConsultingDiagnosysEditAuth);
            }
        }
        private bool _mConsultingDiagnosysFullOpAuth = true;
        public bool mConsultingDiagnosysFullOpAuth
        {
            get
            {
                return _mConsultingDiagnosysFullOpAuth;
            }
            set
            {
                if (_mConsultingDiagnosysFullOpAuth == value) return;
                _mConsultingDiagnosysFullOpAuth = value;
                NotifyOfPropertyChange(() => mConsultingDiagnosysFullOpAuth);
            }
        }
        private string _ViewTitle = eHCMSResources.Z2047_G1_KHCNoiNgoai;
        public string ViewTitle
        {
            get
            {
                return _ViewTitle;
            }
            set
            {
                _ViewTitle = value;
                NotifyOfPropertyChange(() => ViewTitle);
            }
        }
        #endregion
        #region Handles
        public void Handle(ItemSelected<Patient> message)
        {
            CreateNewConsultingDiagnosys();
            if (message != null)
            {
                gConsultingDiagnosys.Patient = message.Item;
                LoadConsultingDiagnosysFromPatient();
            }
        }
        public void Handle(ConsultingDiagnosys<Patient> message)
        {
            CreateNewConsultingDiagnosys();
            if (message != null)
            {
                gConsultingDiagnosys.Patient = message.Item;
                LoadConsultingDiagnosysFromPatient();
            }
        }
        public void Handle(ConsultingDiagnosys<ConsultingDiagnosys> message)
        {
            CreateNewConsultingDiagnosys();
            if (message != null)
            {
                gConsultingDiagnosys = message.Item as ConsultingDiagnosys;
                PatientSummaryInfoContent.CurrentPatient = gConsultingDiagnosys.Patient;
                LoadConsultingDiagnosysFromPatient();
            }
        }
        public void Handle(RegistrationSelectedToHoiChan message)
        {
        }
        #endregion
        #region Methods
        private void CreateNewConsultingDiagnosys()
        {
            gConsultingDiagnosys = new ConsultingDiagnosys();
            UCGeneral.gConsultingDiagnosys = gConsultingDiagnosys;
            UCResult.gConsultingDiagnosys = gConsultingDiagnosys;
            //UCResult.SelectedSurgerySchedule = new SurgerySchedule();
            //20180816 TBL: SelectedSurgerySchedule = null vì khi SelectedSurgerySchedule null thì ExpSurgeryDate mới bị enable
            UCResult.SelectedSurgerySchedule = null;
            UCResult.SurgeryDoctorCollection = new ObservableCollection<Staff>();
            UCResult.ExpSurgeryDate = new DateTime?();
            gConsultingDiagnosys.ConsultingDate = Globals.GetCurServerDateTime();
            gConsultingDiagnosys.V_HeartSurgicalType = HeartSurgicalTypeArray.Where(x => x.LookupID == (long)AllLookupValues.V_HeartSurgicalType.Closed).FirstOrDefault();
            gConsultingDiagnosys.V_DiagnosticType = DiagnosticTypeArray.Where(x => x.LookupID == (long)AllLookupValues.V_DiagnosticType.Congenital).FirstOrDefault();
            gConsultingDiagnosys.V_TreatmentMethod = TreatmentMethodArray.Where(x => x.LookupID == (long)AllLookupValues.V_TreatmentMethod.Surgery).FirstOrDefault();
        }
        private void LoadConsultingDiagnosysFromPatient()
        {
            GetConsultingDiagnosys(gConsultingDiagnosys);
        }
        private void GetPhyExamByPtID()
        {
            if (gConsultingDiagnosys == null) return;
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                using (var serviceFactory = new SummaryServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetPhyExamByPtID(gConsultingDiagnosys.PatientID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var items = contract.EndGetPhyExamByPtID(asyncResult);
                            if (items != null && items.Count > 0)
                                gConsultingDiagnosys.PhysicalExamination = items.FirstOrDefault();
                            UCGeneral.gConsultingDiagnosys = gConsultingDiagnosys;
                            UCResult.gConsultingDiagnosys = gConsultingDiagnosys;
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
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
        private void GetConsultingDiagnosys(ConsultingDiagnosys aConsultingDiagnosys)
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new CommonUtilsServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetConsultingDiagnosys(gConsultingDiagnosys,
                        Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var ConsultingDiagnosys = contract.EndGetConsultingDiagnosys(asyncResult);
                                if (ConsultingDiagnosys == null)
                                    MessageBox.Show(eHCMSResources.Z2094_G1_KhongThayTTHChan, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
                                else
                                {
                                    gConsultingDiagnosys = ConsultingDiagnosys;
                                    gConsultingDiagnosys.Patient = PatientSummaryInfoContent.CurrentPatient;
                                    if (gConsultingDiagnosys.ConsultingDoctor > 0)
                                    {
                                        gConsultingDiagnosys.ConsultingDoctorStaff = DoctorStaffs.Where(x => x.StaffID == gConsultingDiagnosys.ConsultingDoctor).FirstOrDefault();
                                    }
                                    if (gConsultingDiagnosys.OutPtConsultingDoctor > 0)
                                    {
                                        gConsultingDiagnosys.OutPtConsultingDoctorStaff = DoctorStaffs.Where(x => x.StaffID == gConsultingDiagnosys.OutPtConsultingDoctor).FirstOrDefault();
                                    }
                                    if (gConsultingDiagnosys.PtRegistrationID.GetValueOrDefault(0) > 0)
                                    {
                                        GetRegistrationInfo(gConsultingDiagnosys);
                                    }
                                }
                                GetPhyExamByPtID();
                                if (gConsultingDiagnosys.FirstExamDate == null)
                                    GetFirstExamDate();
                                GetNextAppointment();
                                UCGeneral.gConsultingDiagnosys = gConsultingDiagnosys;
                                UCResult.gConsultingDiagnosys = gConsultingDiagnosys;
                                UCGeneral.InitDataView();
                                UCResult.InitDataView();
                                /*▼====: #001*/
                                // Truoc khi gConsultingDiagnosys bi doi
                                origConsultingDiagnosys = ObjectCopier.DeepCopy(gConsultingDiagnosys);
                                /*▲====: #001*/
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
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
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }
        private void GetRegistrationInfo(ConsultingDiagnosys aConsultingDiagnosys)
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetRegistrationInfo_InPt(aConsultingDiagnosys.PtRegistrationID.Value, 1, new LoadRegistrationSwitch { IsGetAdmissionInfo = true }, false,
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                try
                                {
                                    aConsultingDiagnosys.Registration = contract.EndGetRegistrationInfo_InPt(asyncResult);
                                    UCGeneral.gConsultingDiagnosys = gConsultingDiagnosys;
                                    UCResult.gConsultingDiagnosys = gConsultingDiagnosys;
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
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
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }
        private void GetFirstExamDate()
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new CommonUtilsServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetFirstExamDate(gConsultingDiagnosys.PatientID,
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                try
                                {
                                    gConsultingDiagnosys.FirstExamDate = contract.EndGetFirstExamDate(asyncResult);
                                    UCGeneral.gConsultingDiagnosys = gConsultingDiagnosys;
                                    UCResult.gConsultingDiagnosys = gConsultingDiagnosys;
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
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
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }
        private void GetNextAppointment()
        {
            long mMedServiceID = 412;
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new CommonUtilsServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetNextAppointment(gConsultingDiagnosys.PatientID, mMedServiceID, Globals.GetCurServerDateTime(),
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                try
                                {
                                    gConsultingDiagnosys.NextExamDate = contract.EndGetNextAppointment(asyncResult);
                                    UCGeneral.gConsultingDiagnosys = gConsultingDiagnosys;
                                    UCResult.gConsultingDiagnosys = gConsultingDiagnosys;
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
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
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }
        private void OpenPatientDetailDialog_New()
        {
            Action<IPatientDetails> onInitDlg = (patientDetailsVm) =>
            {
                patientDetailsVm.CurrentAction = eHCMSResources.Z0971_G1_CNhatTTinBN;

                patientDetailsVm.FormState = FormState.EDIT;
                patientDetailsVm.CloseWhenFinish = true;
                patientDetailsVm.IsChildWindow = true;
                patientDetailsVm.InitLoadControlData_FromExt(null);

                var patient = new GenericCoRoutineTask(patientDetailsVm.LoadPatientDetailsAndHI_GenAction, gConsultingDiagnosys.Patient, false);

                patientDetailsVm.ActiveTab = PatientInfoTabs.GENERAL_INFO;
                patientDetailsVm.ActivationMode = ActivationMode.PATIENT_GENERAL_HI_VIEW;
            };
            GlobalsNAV.ShowDialog<IPatientDetails>(onInitDlg);
        }
        private IEnumerator<IResult> OpenPatientDetailDialog()
        {
            var patientDetailsVm = Globals.GetViewModel<IPatientDetails>();
            patientDetailsVm.CurrentAction = eHCMSResources.Z0971_G1_CNhatTTinBN;

            patientDetailsVm.FormState = FormState.EDIT;
            patientDetailsVm.CloseWhenFinish = true;
            patientDetailsVm.IsChildWindow = true;

            yield return new GenericCoRoutineTask(patientDetailsVm.LoadPatientDetailsAndHI_GenAction, gConsultingDiagnosys.Patient, true);

            patientDetailsVm.ActiveTab = PatientInfoTabs.GENERAL_INFO;
            patientDetailsVm.ActivationMode = ActivationMode.PATIENT_GENERAL_HI_VIEW;
            GlobalsNAV.ShowDialog_V3<IPatientDetails>(patientDetailsVm);
            yield break;
        }
        private void CheckAuthorization()
        {
            if (!Globals.isAccountCheck)
                return;
            mConsultingDiagnosysEditAuth = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConsultation
                                                   , (int)eConsultation.mConsultingDiagnosy
                                                   , (int)oConsultationEx.mConsultingDiagnosys_ConsultingEdit, (int)ePermission.mView);
            mConsultingDiagnosysFullOpAuth = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConsultation
                                               , (int)eConsultation.mConsultingDiagnosy
                                               , (int)oConsultationEx.mConsultingDiagnosys_Full, (int)ePermission.mView);
            if (mConsultingDiagnosysFullOpAuth) mConsultingDiagnosysEditAuth = true;
        }

        #endregion
        #region Events

        public void ResetView()
        {
            PatientSummaryInfoContent.CurrentPatient = new Patient();
            CreateNewConsultingDiagnosys();
        }
        /*▼====: #001*/
        // Ham load lai du lieu da co truoc khi sua
        private void RestoreToOrigValues()
        {
            gConsultingDiagnosys = ObjectCopier.DeepCopy(origConsultingDiagnosys);
            UCGeneral.gConsultingDiagnosys = gConsultingDiagnosys;
            UCResult.gConsultingDiagnosys = gConsultingDiagnosys;
            UCGeneral.InitDataView();
            UCResult.InitDataView();
        }
        /*▲====: #001*/
        public void btnSave()
        {
            if (gValidationSummary != null && gValidationSummary.HasDisplayedErrors)
            {
                return;
            }
            if (gConsultingDiagnosys.PatientID == 0)
            {
                MessageBox.Show(eHCMSResources.K0290_G1_ChonBN, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
            bool IsAddNew = false;
            if (gConsultingDiagnosys.ConsultingDiagnosysID == 0)
            {
                gConsultingDiagnosys.Createdby = Globals.LoggedUserAccount.StaffID.GetValueOrDefault(0);
                IsAddNew = true;
            }
            try
            {
                gConsultingDiagnosys.SurgeryScheduleDetail = UCResult.GetSurgeryScheduleDetail();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
                return;
            }

            /*▼====: #001*/
            if (!CheckValid())
            {
                //RestoreToOrigValues(); //20190502 TBL: Khong can load lai du lieu da co truoc khi sua
                return;
            }
            /*▲====: #001*/
            var t = new Thread(() =>
            {
                try
                {
                    this.ShowBusyIndicator();
                    using (var serviceFactory = new CommonUtilsServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginUpdateConsultingDiagnosys(gConsultingDiagnosys,
                        Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                long ConsultingDiagnosysID;
                                if (contract.EndUpdateConsultingDiagnosys(out ConsultingDiagnosysID, asyncResult))
                                {
                                    if (IsAddNew)
                                    {
                                        MessageBox.Show(eHCMSResources.A0079_G1_Msg_InfoThemMoiOK, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                        gConsultingDiagnosys.ConsultingDiagnosysID = ConsultingDiagnosysID;
                                    }
                                    else
                                    {
                                        MessageBox.Show(eHCMSResources.A0279_G1_Msg_InfoCNhatOK, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                    }
                                    /*▼====: #001*/
                                    // Truoc khi gConsultingDiagnosys bi doi
                                    origConsultingDiagnosys = ObjectCopier.DeepCopy(gConsultingDiagnosys);
                                    /*▲====: #001*/
                                }
                                else
                                    MessageBox.Show(eHCMSResources.G0140_G1_ThatBai, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
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
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }
        public void cboDoctor_Populating(object sender, PopulatingEventArgs e)
        {
            AutoCompleteBox cboContext = sender as AutoCompleteBox;
            e.Cancel = true;
            var AllItemsContext = new ObservableCollection<Staff>(DoctorStaffs.Where(x => x.FullName.ToLower().Contains(cboContext.SearchText.ToLower())).ToList());
            cboContext.ItemsSource = AllItemsContext;
            cboContext.PopulateComplete();
        }
        public void ValidationSummary_Loaded(object sender, RoutedEventArgs e)
        {
            gValidationSummary = sender as ValidationSummary;
        }
        public void btnEditPatientInfoCmd()
        {
            if (gConsultingDiagnosys.Patient == null)
            {
                return;
            }
            Coroutine.BeginExecute(OpenPatientDetailDialog());
        }
        public void btnEditPhysicalConditionCmd()
        {
            if (gConsultingDiagnosys.Patient == null)
            {
                return;
            }
            Action<IcwPhysiscalExam> onInitDlg = (proAlloc) =>
            {
                proAlloc.PatientID = gConsultingDiagnosys.Patient.PatientID;

                if (gConsultingDiagnosys.PhysicalExamination == null)
                {
                    proAlloc.IsVisibility = Visibility.Collapsed;
                    proAlloc.isEdit = false;
                }
                else
                {
                    proAlloc.PtPhyExamItem = ObjectCopier.DeepCopy(gConsultingDiagnosys.PhysicalExamination);
                    proAlloc.IsVisibility = Visibility.Visible;
                    proAlloc.isEdit = true;
                }
            };
            Action<Common.Enums.MsgBoxOptions, IScreen> onCallbackDig = (m, s) =>
            {
                GetPhyExamByPtID();
            };
            GlobalsNAV.ShowDialog<IcwPhysiscalExam>(onInitDlg, onCallbackDig);
        }
        public void btnEditBloodInfo()
        {
            if (gConsultingDiagnosys.Patient == null)
            {
                return;
            }
            //var frmBloodTypeVM = Globals.GetViewModel<IfrmBloodType>();
            //this.ActivateItem(frmBloodTypeVM);
            //frmBloodTypeVM.PatientInfo = gConsultingDiagnosys.Patient;
            //var instance = frmBloodTypeVM as Conductor<object>;
            //Globals.ShowDialog(instance, (o) => { });

            Action<IfrmBloodType> onInitDlg = (frmBloodTypeVM) =>
            {
                this.ActivateItem(frmBloodTypeVM);
                frmBloodTypeVM.PatientInfo = gConsultingDiagnosys.Patient;
            };
            GlobalsNAV.ShowDialog<IfrmBloodType>(onInitDlg);
        }
        public void btnPrintEstimationPriceReport()
        {
            //var proAlloc = Globals.GetViewModel<ICommonPreviewView>();
            //proAlloc.PrimaryID = (long)gConsultingDiagnosys.ConsultingDiagnosysID;
            //proAlloc.eItem = ReportName.EstimationPriceReport;
            //var instance = proAlloc as Conductor<object>;
            //Globals.ShowDialog(instance, (o) => { });

            Action<ICommonPreviewView> onInitDlg = (proAlloc) =>
            {
                proAlloc.PrimaryID = (long)gConsultingDiagnosys.ConsultingDiagnosysID;
                proAlloc.eItem = ReportName.EstimationPriceReport;
            };
            GlobalsNAV.ShowDialog<ICommonPreviewView>(onInitDlg);
        }
        #endregion
        /*▼====: #001*/
        public bool CheckValid()
        {
            if (gConsultingDiagnosys.AdminProcessApprovedDate == null && gConsultingDiagnosys.IsAllExamCompleted)
            {
                if (origConsultingDiagnosys.AdminProcessApprovedDate == null)
                {
                    MessageBox.Show(eHCMSResources.Z2222_G1_ChuaCoNgNhanHS);
                }
                else
                {
                    MessageBox.Show(eHCMSResources.Z2233_G1_KhogTheXoaNgNhanHS);
                }
                return false;
            }
            if (!gConsultingDiagnosys.IsAllExamCompleted && gConsultingDiagnosys.SurgeryScheduleDetail != null)
            {
                if (!origConsultingDiagnosys.IsAllExamCompleted)
                {
                    MessageBox.Show(eHCMSResources.Z2101_G1_ChuaHTatXetNghiem);
                }
                else
                {
                    MessageBox.Show(eHCMSResources.Z2230_G1_KhogThayDoiTTXN);
                }
                return false;
            }
            if (gConsultingDiagnosys.SurgeryScheduleDetail == null && gConsultingDiagnosys.SurgeryDate != null)
            {
                if (origConsultingDiagnosys.SurgeryScheduleDetail == null || origConsultingDiagnosys.SurgeryScheduleDetail.SSD_Date == null)
                {
                    MessageBox.Show(eHCMSResources.Z2223_G1_ChuaCoNgMoDuKien);
                }
                else if (origConsultingDiagnosys.SurgeryScheduleDetail.SSD_Date != null)
                {
                    MessageBox.Show(eHCMSResources.Z2231_G1_KhogTheXoaNgMoDK);
                }
                return false;
            }
            if (gConsultingDiagnosys.ExpAdmissionDate == null && gConsultingDiagnosys.SurgeryDate != null)
            {
                if (origConsultingDiagnosys.ExpAdmissionDate == null)
                {
                    MessageBox.Show(eHCMSResources.Z2229_G1_ChuaCoNgDuKienNV);
                }
                else
                {
                    MessageBox.Show(eHCMSResources.Z2232_G1_KhogTheXoaNgDKNV);
                }
                return false;
            }
            else
                return true;
        }
        /*▲====: #001*/

        private Visibility _VisibilitySearch = Visibility.Visible;
        public Visibility VisibilitySearch
        {
            get { return _VisibilitySearch; }
            set
            {
                _VisibilitySearch = value;
                NotifyOfPropertyChange(() => VisibilitySearch);
            }
        }
    }
}
