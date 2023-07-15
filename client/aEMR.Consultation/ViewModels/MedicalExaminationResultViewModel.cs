using aEMR.Common.BaseModel;
using aEMR.Common.Collections;
using aEMR.Common.ViewModels;
using aEMR.CommonTasks;
using aEMR.Infrastructure;
using aEMR.Infrastructure.CachingUtils;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
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

/*
 * 20230501 #001 DatTB: Thêm nút popup tính tuổi động mạch
 * 20230506 #002 DatTB: Fix lỗi check null
 * 20230527 #003 DatTB:
 * + Thêm giá trị mặc định các trường KSK
 * + Thêm service xóa kết quả KSK
 * + Fix lỗi chỉ cập nhật trạng thái phiếu KSK lần đầu tiên
 * + Clone model tab tình trạng thể chất - dấu hiệu sinh tồn vì model đã chung với màn hình khám truyền nhiều dữ liệu không cần thiết.
 * + Thêm Tab nhập tình trạng thể chất- dấu hiệu sinh tồn
 */
namespace aEMR.ConsultantEPrescription.ViewModels
{
    [Export(typeof(IMedicalExaminationResult)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class MedicalExaminationResultViewModel : ViewModelBase, IMedicalExaminationResult
        //, IHandle<ShowPatientInfo_KHAMBENH_THONGTINCHUNG<Patient, PatientRegistration, PatientRegistrationDetail>>
        , IHandle<RegDetailSelectedForConsultation>
        , IHandle<LocationSelected_New>
    {
        #region Properties
        public ISearchPatientAndRegistration SearchRegistrationContent { get; set; }
        public IPatientInfo UCPatientProfileInfo { get; set; }
        public ILoginInfo UCDoctorProfileInfo { get; set; }
        public IPtRegDetailInfo UCPtRegDetailInfo { get; set; }
        private bool[] _EnableByDeptLocationID = new bool[6];
        private ObservableCollection<Lookup> _V_HealthClassCollection;
        private MedicalExaminationResult _CurrentMedicalExaminationResult;
        private bool _IsMedicalExamination;
        public bool[] EnableByDeptLocationID
        {
            get
            {
                return _EnableByDeptLocationID;
            }
            set
            {
                _EnableByDeptLocationID = value;
                NotifyOfPropertyChange(() => EnableByDeptLocationID);
            }
        }

        public ObservableCollection<Lookup> V_HealthClassCollection
        {
            get
            {
                return _V_HealthClassCollection;
            }
            set
            {
                _V_HealthClassCollection = value;
                NotifyOfPropertyChange(() => V_HealthClassCollection);
            }
        }

        public MedicalExaminationResult CurrentMedicalExaminationResult
        {
            get
            {
                return _CurrentMedicalExaminationResult;
            }
            set
            {
                _CurrentMedicalExaminationResult = value;
                NotifyOfPropertyChange(() => CurrentMedicalExaminationResult);
            }
        }

        public bool IsMedicalExamination
        {
            get
            {
                return _IsMedicalExamination;
            }
            set
            {
                _IsMedicalExamination = value;
                NotifyOfPropertyChange(() => IsMedicalExamination);
            }
        }

        private Registration_DataStorage _Registration_DataStorage;
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
                UCPtRegDetailInfo.Registration_DataStorage = Registration_DataStorage;
            }
        }
        #endregion

        #region Events
        [ImportingConstructor]
        public MedicalExaminationResultViewModel(IWindsorContainer aContainer, INavigationService aNavigation, IEventAggregator aEventAgrs, ISalePosCaching aCaching)
        {
            InitEnableControls();
            V_HealthClassCollection = Globals.AllLookupValueList.Where(x => x.ObjectTypeID == (long)LookupValues.V_HealthyClassification).ToObservableCollection();
            ISearchPatientAndRegistration mSearchPatientAndRegistrationView = Globals.GetViewModel<ISearchPatientAndRegistration>();
            if (Globals.PatientFindBy_ForConsultation == null)
            {
                Globals.PatientFindBy_ForConsultation = AllLookupValues.PatientFindBy.NGOAITRU;
            }
            mSearchPatientAndRegistrationView.PatientFindBy = Globals.PatientFindBy_ForConsultation.Value;
            mSearchPatientAndRegistrationView.CloseRegistrationFormWhenCompleteSelection = false;
            mSearchPatientAndRegistrationView.InitButtonVisibility(SearchRegButtonsVisibility.SHOW_SEARCH_REG_BTN);
            mSearchPatientAndRegistrationView.SetDefaultButton(SearchRegistrationButtons.SEARCH_REGISTRATION);
            mSearchPatientAndRegistrationView.IsSearchGoToKhamBenh = true;
            mSearchPatientAndRegistrationView.PatientFindByVisibility = true;
            mSearchPatientAndRegistrationView.CanSearhRegAllDept = true;
            mSearchPatientAndRegistrationView.SearchAdmittedInPtRegOnly = true;
            mSearchPatientAndRegistrationView.IsSearchPhysicalExaminationOnly = true;
            if (!Globals.ServerConfigSection.ConsultationElements.IsAllowSearchingPtByName)
            {
                mSearchPatientAndRegistrationView.IsAllowSearchingPtByName_Visible = false;
                mSearchPatientAndRegistrationView.IsSearchPtByNameChecked = true;
            }
            SearchRegistrationContent = mSearchPatientAndRegistrationView;
            SearchRegistrationContent.PatientFindBy = AllLookupValues.PatientFindBy.NGOAITRU;
            UCPatientProfileInfo = Globals.GetViewModel<IPatientInfo>();
            UCDoctorProfileInfo = Globals.GetViewModel<ILoginInfo>();
            UCDoctorProfileInfo.isPreNoteTemp = true;
            UCPtRegDetailInfo = Globals.GetViewModel<IPtRegDetailInfo>();
            //▼==== #003
            UCPhysicalVitalSigns = Globals.GetViewModel<IPhysicalVitalSigns>();
            //▲==== #003
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            Globals.EventAggregator.Subscribe(this);
            ActivateItem(SearchRegistrationContent);
            ActivateItem(UCPatientProfileInfo);
            ActivateItem(UCDoctorProfileInfo);
            ActivateItem(UCPtRegDetailInfo);
            //▼==== #003
            ActivateItem(UCPhysicalVitalSigns);
            //▲==== #003
        }

        protected override void OnDeactivate(bool close)
        {
            DeactivateItem(SearchRegistrationContent, close);
            DeactivateItem(UCPatientProfileInfo, close);
            DeactivateItem(UCDoctorProfileInfo, close);
            DeactivateItem(UCPtRegDetailInfo, close);
            //▼==== #003
            DeactivateItem(UCPhysicalVitalSigns, close);
            //▲==== #003
            base.OnDeactivate(close);
        }

        public void btnSave()
        {
            if (CurrentMedicalExaminationResult == null || CurrentMedicalExaminationResult.PtRegistrationID == 0)
            {
                return;
            }
            if (EnableByDeptLocationID[0])
            {
                CurrentMedicalExaminationResult.InternalMedicalResultStaff = Globals.LoggedUserAccount.Staff;
            }
            if (EnableByDeptLocationID[1])
            {
                CurrentMedicalExaminationResult.GeneralSugeryResultStaff = Globals.LoggedUserAccount.Staff;
            }
            if (EnableByDeptLocationID[2])
            {
                CurrentMedicalExaminationResult.OptometryResultStaff = Globals.LoggedUserAccount.Staff;
            }
            if (EnableByDeptLocationID[3])
            {
                CurrentMedicalExaminationResult.EarAndThroatResultStaff = Globals.LoggedUserAccount.Staff;
            }
            if (EnableByDeptLocationID[4])
            {
                CurrentMedicalExaminationResult.DentalAndJawResultStaff = Globals.LoggedUserAccount.Staff;
            }
            if (EnableByDeptLocationID[5])
            {
                CurrentMedicalExaminationResult.DermatologyResultStaff = Globals.LoggedUserAccount.Staff;
            }
            if (EnableByDeptLocationID[6])
            {
                CurrentMedicalExaminationResult.ObstetricResultStaff = Globals.LoggedUserAccount.Staff;
            }
            this.ShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
            var mThread = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ePMRsServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginUpdateMedicalExaminationResult(CurrentMedicalExaminationResult, Globals.DispatchCallback(asyncResult =>
                        {
                            try
                            {
                                contract.EndUpdateMedicalExaminationResult(asyncResult);
                                //LoadMedicalExaminationResult(CurrentMedicalExaminationResult.PtRegDetailID);
                                LoadMedicalExaminationResultNew(CurrentMedicalExaminationResult.PtRegistrationID, CurrentMedicalExaminationResult.PtRegDetailID);
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
                }
                catch (Exception ex)
                {
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    this.HideBusyIndicator();
                }
            });

            mThread.Start();
        }

        //public void Handle(ShowPatientInfo_KHAMBENH_THONGTINCHUNG<Patient, PatientRegistration, PatientRegistrationDetail> message)
        //{
        //    Registration_DataStorage = new Registration_DataStorage();
        //    if (message != null && message.PtRegDetail != null && message.PtRegDetail.PtRegDetailID > 0)
        //    {
        //        Registration_DataStorage.CurrentPatientRegistration = message.PtReg;
        //        Registration_DataStorage.CurrentPatientRegistrationDetail = message.PtRegDetail;
        //        IsMedicalExamination = message.PtRegDetail != null && message.PtRegDetail.RefMedicalServiceItem != null && message.PtRegDetail.RefMedicalServiceItem.IsMedicalExamination;
        //        LoadMedicalExaminationResult(message.PtRegDetail.PtRegDetailID);
        //        InitEnableControls();
        //    }
        //}

        public void Handle(LocationSelected_New message)
        {
            InitEnableControls();
        }

        public void Handle(RegDetailSelectedForConsultation message)
        {
            //OpenRegDetailForConsultation(message.Source);
            PatientRegistrationDetail tempPtRegDetail = message.Source;
            Coroutine.BeginExecute(SetPatientInfoForConsultation(message.Source.PatientRegistration, message.Source));
            IsMedicalExamination = tempPtRegDetail != null && tempPtRegDetail.RefMedicalServiceItem != null && tempPtRegDetail.RefMedicalServiceItem.IsMedicalExamination;
            //LoadMedicalExaminationResult(tempPtRegDetail.PtRegDetailID);
            LoadMedicalExaminationResultNew(tempPtRegDetail.PatientRegistration.PtRegistrationID, tempPtRegDetail.PtRegDetailID);
            //▼==== #003
            Registration_DataStorage = new Registration_DataStorage();
            //▲==== #003
            InitEnableControls();
        }
        #endregion

        #region Methods
        private IEnumerator<IResult> SetPatientInfoForConsultation(PatientRegistration PtRegistration, PatientRegistrationDetail PtRegDetail)
        {
            if (PtRegistration == null || PtRegDetail == null)
            {
                MessageBox.Show(string.Format("{0}.", eHCMSResources.Z0400_G1_KgNhanDuocDLieuLamViec), eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                yield break;
            }
            //Globals.isConsultationStateEdit = true;
            //Globals.EventAggregator.Publish(new isConsultationStateEditEvent { isConsultationStateEdit = true });
            //yield return GenericCoRoutineTask.StartTask(InitPhyExamAction, PtRegistration);
            //yield return GenericCoRoutineTask.StartTask(GetPtServiceRecordForKhamBenhAction, PtRegistration, PtRegDetail);
            //Lấy thông tin đăng ký đầy đủ để lưu lại trong module Khám Bệnh
            yield return GenericCoRoutineTask.StartTask(GetRegistrationAction, PtRegistration, PtRegDetail);
            //PublishEventGlobalsPSRLoad(false);
        }

        private void GetRegistrationAction(GenericCoRoutineTask genTask, object ObjPtRegistration, object ObjPtRegDetail)
        {
            this.ShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        bool bContinue = true;
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetRegistration(((PatientRegistration)ObjPtRegistration).PtRegistrationID, (int)Globals.PatientFindBy_ForConsultation.Value,
                        Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                PatientRegistration regInfo = contract.EndGetRegistration(asyncResult);
                                UCPatientProfileInfo.CurrentRegistration = regInfo;
                                UCPatientProfileInfo.CurrentPatient = regInfo.Patient;
                                // TNHX UPDATE ten dich vu vào Patients Info
                                UCPatientProfileInfo.CurrentPatient.GeneralInfoString = UCPatientProfileInfo.CurrentPatient.GeneralInfoString + " - " + ((PatientRegistrationDetail)ObjPtRegDetail).MedServiceName;

                                //▼==== #003
                                UCPhysicalVitalSigns.CurrentPatientRegistration = regInfo;
                                UCPhysicalVitalSigns.PatientInfo = regInfo.Patient;
                                //▲==== #003
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
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
                    genTask.ActionComplete(false);
                    this.HideBusyIndicator();
                }
            });

            t.Start();
        }

        private void LoadMedicalExaminationResult(long aPtRegDetailID)
        {
            CurrentMedicalExaminationResult = new MedicalExaminationResult { PtRegDetailID = aPtRegDetailID };
            if (aPtRegDetailID == 0)
            {
                return;
            }
            this.ShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
            var mThread = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ePMRsServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetMedicalExaminationResultByPtRegDetailID(aPtRegDetailID, Globals.DispatchCallback(asyncResult =>
                        {
                            try
                            {
                                CurrentMedicalExaminationResult = contract.EndGetMedicalExaminationResultByPtRegDetailID(asyncResult);
                                if (CurrentMedicalExaminationResult == null || CurrentMedicalExaminationResult.PtRegDetailID == 0)
                                {
                                    CurrentMedicalExaminationResult = new MedicalExaminationResult { PtRegDetailID = aPtRegDetailID };
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
                }
                catch (Exception ex)
                {
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    this.HideBusyIndicator();
                }
            });

            mThread.Start();
        }

        private void LoadMedicalExaminationResultNew(long aPtRegistrationID, long aPtRegDetailID)
        {
            if (aPtRegistrationID == 0 || aPtRegDetailID == 0)
            {
                return;
            }
            CurrentMedicalExaminationResult = new MedicalExaminationResult { PtRegistrationID = aPtRegistrationID, PtRegDetailID = aPtRegDetailID };
            this.ShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
            var mThread = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ePMRsServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetMedicalExaminationResultByPtRegistrationID(aPtRegistrationID, Globals.DispatchCallback(asyncResult =>
                        {
                            try
                            {
                                CurrentMedicalExaminationResult = contract.EndGetMedicalExaminationResultByPtRegistrationID(asyncResult);
                                if (CurrentMedicalExaminationResult == null || CurrentMedicalExaminationResult.PtRegistrationID == 0)
                                {
                                    CurrentMedicalExaminationResult = new MedicalExaminationResult { PtRegistrationID = aPtRegistrationID, PtRegDetailID = aPtRegDetailID };
                                }
                                //▼==== #003
                                else
                                {
                                    CurrentMedicalExaminationResult.PtRegDetailID = aPtRegDetailID;
                                    CanDeleteMedicalExamination = true;
                                }

                                if (EnableByDeptLocationID[0])
                                {
                                    CurrentMedicalExaminationResult.CirculationTestResult = string.IsNullOrEmpty(CurrentMedicalExaminationResult.CirculationTestResult) 
                                                                                                ? "Tim đều" : CurrentMedicalExaminationResult.CirculationTestResult;
                                    CurrentMedicalExaminationResult.RespiratoryTestResult = string.IsNullOrEmpty(CurrentMedicalExaminationResult.RespiratoryTestResult) 
                                                                                                ? "Phổi trong" : CurrentMedicalExaminationResult.RespiratoryTestResult;
                                    CurrentMedicalExaminationResult.DigestionTestResult = string.IsNullOrEmpty(CurrentMedicalExaminationResult.DigestionTestResult) 
                                                                                                ? "Bụng mềm" : CurrentMedicalExaminationResult.DigestionTestResult;
                                    CurrentMedicalExaminationResult.UrologyTestResult = string.IsNullOrEmpty(CurrentMedicalExaminationResult.UrologyTestResult) 
                                                                                                ? "Chạm thận (-)" : CurrentMedicalExaminationResult.UrologyTestResult;
                                    CurrentMedicalExaminationResult.EndocrineTestResult = string.IsNullOrEmpty(CurrentMedicalExaminationResult.EndocrineTestResult) 
                                                                                                ? "Chưa ghi nhận bệnh lý" : CurrentMedicalExaminationResult.EndocrineTestResult;
                                    CurrentMedicalExaminationResult.OrthopaedicsTestResult = string.IsNullOrEmpty(CurrentMedicalExaminationResult.OrthopaedicsTestResult) 
                                                                                                ? "Vận động bình thường" : CurrentMedicalExaminationResult.OrthopaedicsTestResult;
                                    CurrentMedicalExaminationResult.NeurologyTestResult = string.IsNullOrEmpty(CurrentMedicalExaminationResult.NeurologyTestResult) 
                                                                                                ? "Không dấu thần kinh khu trú" : CurrentMedicalExaminationResult.NeurologyTestResult;
                                    CurrentMedicalExaminationResult.NeuropsychiatricTestResult = string.IsNullOrEmpty(CurrentMedicalExaminationResult.NeuropsychiatricTestResult) 
                                                                                                ? "Chưa ghi nhận" : CurrentMedicalExaminationResult.NeuropsychiatricTestResult;
                                }
                                if (EnableByDeptLocationID[1])
                                {
                                    CurrentMedicalExaminationResult.GeneralSugeryTestResult = string.IsNullOrEmpty(CurrentMedicalExaminationResult.GeneralSugeryTestResult) 
                                                                                                ? "Chưa ghi nhận bệnh lý" : CurrentMedicalExaminationResult.GeneralSugeryTestResult;
                                }
                                if (EnableByDeptLocationID[5])
                                {
                                    CurrentMedicalExaminationResult.DermatologyTestResult = string.IsNullOrEmpty(CurrentMedicalExaminationResult.DermatologyTestResult) 
                                                                                                ? "Chưa ghi nhận bệnh lý" : CurrentMedicalExaminationResult.DermatologyTestResult;
                                }
                                if (EnableByDeptLocationID[6])
                                {
                                    CurrentMedicalExaminationResult.ObstetricTestResult = string.IsNullOrEmpty(CurrentMedicalExaminationResult.ObstetricTestResult) 
                                                                                                ? "Chưa ghi nhận bệnh lý" : CurrentMedicalExaminationResult.ObstetricTestResult;
                                }
                                //▲==== #003

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
                }
                catch (Exception ex)
                {
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    this.HideBusyIndicator();
                }
            });

            mThread.Start();
        }

        private void InitEnableControls()
        {
            EnableByDeptLocationID = new bool[7] { IsMedicalExamination && Globals.ServerConfigSection.ConsultationElements.HealthExamDeptLocIDArray.Length > 0 && Globals.DeptLocation.DeptLocationID == Globals.ServerConfigSection.ConsultationElements.HealthExamDeptLocIDArray[0]
                , IsMedicalExamination && Globals.ServerConfigSection.ConsultationElements.HealthExamDeptLocIDArray.Length > 1 && Globals.DeptLocation.DeptLocationID == Globals.ServerConfigSection.ConsultationElements.HealthExamDeptLocIDArray[1]
                , IsMedicalExamination && Globals.ServerConfigSection.ConsultationElements.HealthExamDeptLocIDArray.Length > 2 && Globals.DeptLocation.DeptLocationID == Globals.ServerConfigSection.ConsultationElements.HealthExamDeptLocIDArray[2]
                , IsMedicalExamination && Globals.ServerConfigSection.ConsultationElements.HealthExamDeptLocIDArray.Length > 3 && Globals.DeptLocation.DeptLocationID == Globals.ServerConfigSection.ConsultationElements.HealthExamDeptLocIDArray[3]
                , IsMedicalExamination && Globals.ServerConfigSection.ConsultationElements.HealthExamDeptLocIDArray.Length > 4 && Globals.DeptLocation.DeptLocationID == Globals.ServerConfigSection.ConsultationElements.HealthExamDeptLocIDArray[4]
                , IsMedicalExamination && Globals.ServerConfigSection.ConsultationElements.HealthExamDeptLocIDArray.Length > 5 && Globals.DeptLocation.DeptLocationID == Globals.ServerConfigSection.ConsultationElements.HealthExamDeptLocIDArray[5]
                , IsMedicalExamination && Globals.ServerConfigSection.ConsultationElements.HealthExamDeptLocIDArray.Length > 6 && Globals.DeptLocation.DeptLocationID == Globals.ServerConfigSection.ConsultationElements.HealthExamDeptLocIDArray[6]};
        }

        public void btnPrint()
        {
            void onInitDlg(ICommonPreviewView proAlloc)
            {
                proAlloc.RegistrationDetailID = CurrentMedicalExaminationResult.PtRegDetailID;
                proAlloc.ID = CurrentMedicalExaminationResult.PtRegistrationID;
                proAlloc.eItem = ReportName.Kham_Suc_Khoe;
            }
            GlobalsNAV.ShowDialog<ICommonPreviewView>(onInitDlg);
        }

        //▼==== #001, #002
        public void btnAgeOfTheArtery()
        {
            if (UCPatientProfileInfo != null && UCPatientProfileInfo.CurrentRegistration != null)
            {
                void onInitDlg(IAgeOfTheArtery proAlloc)
                {
                    proAlloc.Patient = UCPatientProfileInfo.CurrentRegistration.Patient;
                    proAlloc.PtRegistrationID = UCPatientProfileInfo.CurrentRegistration.PtRegistrationID;
                    proAlloc.V_RegistrationType = (long)UCPatientProfileInfo.CurrentRegistration.V_RegistrationType;
                    proAlloc.PatientClassID = (long)UCPatientProfileInfo.CurrentRegistration.PatientClassID;
                }
                GlobalsNAV.ShowDialog<IAgeOfTheArtery>(onInitDlg);
            }
        }
        //▲==== #001, #002

        //▼==== #003
        private bool _CanDeleteMedicalExamination;
        public bool CanDeleteMedicalExamination
        {
            get
            {
                return _CanDeleteMedicalExamination;
            }
            set
            {
                _CanDeleteMedicalExamination = value;
                NotifyOfPropertyChange(() => CanDeleteMedicalExamination);
            }
        }

        public void btnDelete()
        {
            if (CurrentMedicalExaminationResult == null || CurrentMedicalExaminationResult.PtRegistrationID == 0 || !CanDeleteMedicalExamination)
            {
                return;
            }
            if (MessageBox.Show(eHCMSResources.Z2682_G1_XacNhanXoaPhieuKham, eHCMSResources.G0442_G1_TBao, MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                if (EnableByDeptLocationID[0])
                {
                    CurrentMedicalExaminationResult.CirculationTestResult = null;
                    CurrentMedicalExaminationResult.Circulation_V_HealthClass = 0;
                    CurrentMedicalExaminationResult.RespiratoryTestResult = null;
                    CurrentMedicalExaminationResult.Respiratory_V_HealthClass = 0;
                    CurrentMedicalExaminationResult.DigestionTestResult = null;
                    CurrentMedicalExaminationResult.Digestion_V_HealthClass = 0;
                    CurrentMedicalExaminationResult.UrologyTestResult = null;
                    CurrentMedicalExaminationResult.Urology_V_HealthClass = 0;
                    CurrentMedicalExaminationResult.EndocrineTestResult = null;
                    CurrentMedicalExaminationResult.Endocrine_V_HealthClass = 0;
                    CurrentMedicalExaminationResult.OrthopaedicsTestResult = null;
                    CurrentMedicalExaminationResult.Orthopaedics_V_HealthClass = 0;
                    CurrentMedicalExaminationResult.NeurologyTestResult = null;
                    CurrentMedicalExaminationResult.Neurology_V_HealthClass = 0;
                    CurrentMedicalExaminationResult.NeuropsychiatricTestResult = null;
                    CurrentMedicalExaminationResult.Neuropsychiatric_V_HealthClass = 0;
                    CurrentMedicalExaminationResult.InternalMedicalResultStaff = null;
                }
                if (EnableByDeptLocationID[1])
                {
                    CurrentMedicalExaminationResult.GeneralSugeryTestResult = null;
                    CurrentMedicalExaminationResult.GeneralSugery_V_HealthClass = 0;
                    CurrentMedicalExaminationResult.GeneralSugeryResultStaff = null;
                }
                if (EnableByDeptLocationID[2])
                {
                    CurrentMedicalExaminationResult.RightOptometryTestResult = null;
                    CurrentMedicalExaminationResult.LeftOptometryTestResult = null;
                    CurrentMedicalExaminationResult.RightSightedOptometryTestResult = null;
                    CurrentMedicalExaminationResult.LeftSightedOptometryTestResult = null;
                    CurrentMedicalExaminationResult.OptometryDecreases = null;
                    CurrentMedicalExaminationResult.Optometry_V_HealthClass = 0;
                    CurrentMedicalExaminationResult.OptometryResultStaff = null;
                }
                if (EnableByDeptLocationID[3])
                {
                    CurrentMedicalExaminationResult.LeftHearingTestResult = null;
                    CurrentMedicalExaminationResult.LeftSilentlyHearingTestResult = null;
                    CurrentMedicalExaminationResult.RightHearingTestResult = null;
                    CurrentMedicalExaminationResult.RightSilentlyHearingTestResult = null;
                    CurrentMedicalExaminationResult.EarAndThroatDecreases = null;
                    CurrentMedicalExaminationResult.EarAndThroatDecreases_V_HealthClass = 0;
                    CurrentMedicalExaminationResult.EarAndThroatResultStaff = null;
                }
                if (EnableByDeptLocationID[4])
                {
                    CurrentMedicalExaminationResult.UpperJawTestResult = null;
                    CurrentMedicalExaminationResult.LowerJawTestResult = null;
                    CurrentMedicalExaminationResult.DentalAndJawDecreases = null;
                    CurrentMedicalExaminationResult.DentalAndJaw_V_HealthClass = 0;
                    CurrentMedicalExaminationResult.DentalAndJawResultStaff = null;
                }
                if (EnableByDeptLocationID[5])
                {
                    CurrentMedicalExaminationResult.DermatologyTestResult = null;
                    CurrentMedicalExaminationResult.Dermatology_V_HealthClass = 0;
                    CurrentMedicalExaminationResult.DermatologyResultStaff = null;
                }
                if (EnableByDeptLocationID[6])
                {
                    CurrentMedicalExaminationResult.ObstetricTestResult = null;
                    CurrentMedicalExaminationResult.Obstetric_V_HealthClass = 0;
                    CurrentMedicalExaminationResult.ObstetricResultStaff = null;
                }
                this.ShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
                var mThread = new Thread(() =>
                {
                    try
                    {
                        using (var serviceFactory = new ePMRsServiceClient())
                        {
                            var contract = serviceFactory.ServiceInstance;
                            contract.BeginDeleteMedicalExaminationResult(CurrentMedicalExaminationResult, Globals.DispatchCallback(asyncResult =>
                            {
                                try
                                {
                                    contract.EndDeleteMedicalExaminationResult(asyncResult);
                                }
                                catch (Exception ex)
                                {
                                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                                }
                                finally
                                {
                                    this.HideBusyIndicator();
                                    LoadMedicalExaminationResultNew(CurrentMedicalExaminationResult.PtRegistrationID, CurrentMedicalExaminationResult.PtRegDetailID);
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

                mThread.Start();
            }
        }
            
        public IPhysicalVitalSigns UCPhysicalVitalSigns { get; set; }
        //▲==== #003


        #endregion
    }
}
