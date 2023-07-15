using System;
using System.ComponentModel.Composition;
using System.Threading;
using System.Collections.Generic;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using aEMR.ViewContracts.Configuration;
using aEMR.Infrastructure;
using aEMR.Infrastructure.CachingUtils;
using aEMR.Common.Converters;
using aEMR.CommonTasks;
using aEMR.Common;
using Caliburn.Micro;
using Castle.Windsor;
using Castle.Core.Logging;
using System.Windows;
using aEMR.Infrastructure.Events;
using DataEntities;
using eHCMSLanguage;
using aEMR.Common.BaseModel;

/*
 * 20181001 #001 TBL: BM 0000110. Fix PatientInfo từ màn hình khám bệnh mới mà qua màn hình khám bệnh cũ rồi quay lại thì màn hình khám bệnh mới mất PatientInfo
 * 20181022 #002 TTM:
 * 20191205 #003 TBL: BM 0019685: Fix lỗi khi cập nhật thông tin bệnh nhân thì trên màn hình không load lại thông tin mới nhất
 */
namespace aEMR.Common.ViewModels
{
    [Export(typeof(IPatientInfo)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class PatientInfoViewModel : ViewModelBase, IPatientInfo
        //, IHandle<ShowPatientInfo<Patient, PatientRegistration, PatientRegistrationDetail>>
        , IHandle<PatientSelectedGoToKhamBenh_InPt<PatientRegistration>>
        , IHandle<PatientReloadEvent>
        , IHandle<ShowPatientInfo<Patient>>
        , IHandle<ShowPatientInfoFromPCLOutStandingTask>
        , IHandle<ShowPatientInfoFromPopUpSearchPCLRequest>
        , IHandle<ShowPatientInfoFromTextBoxSearchPCLRequest>
        , IHandle<ShowPatientInfoForConsultation>
        , IHandle<ShowInPatientInfoForConsultation>
        , IHandle<ItemPatient1<Patient>>
        , IHandle<UpdateCompleted<Patient>>
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;
        [ImportingConstructor]
        public PatientInfoViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAgr, ISalePosCaching salePosCaching)
        {
            System.Diagnostics.Debug.WriteLine("================> PatientInfoViewModel - Constructor");
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            //Globals.EventAggregator.Subscribe(this);
            /*▼====: #001*/
            //if (Registration_DataStorage.CurrentPatient != null)
            //{
            //    InitData();
            //}
            /*▲====: #001*/
        }

        ~PatientInfoViewModel()
        {
            System.Diagnostics.Debug.WriteLine("================> PatientInfoViewModel - Destructor");
        }

        // TxD 22/09/2018 Added the following to ALLOW child VM of each Tab to get ACCESS to the CENTRAL Data stored in the MainKB (Main KHAM BENH) Screen
        private ICS_DataStorage _CS_DS = null;
        public ICS_DataStorage CS_DS
        {
            get
            {
                return _CS_DS;
            }
            set
            {
                _CS_DS = value;
            }
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            Globals.EventAggregator.Subscribe(this);
            /*▼====: #001*/
            if (Registration_DataStorage != null && Registration_DataStorage.CurrentPatient != null)
            {
                InitData();
            }
            /*▲====: #001*/
            System.Diagnostics.Debug.WriteLine("PatientInfoView ====> OnActivate..");
        }

        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);
            Globals.EventAggregator.Unsubscribe(this);
            System.Diagnostics.Debug.WriteLine("PatientInfoView ====> On DeActivate..");
        }


        #region Properties Member
        //private string _currentHealthInsuranceNo;
        //public string CurrentHealthInsuranceNo
        //{
        //    get { return _currentHealthInsuranceNo; }
        //    set
        //    {
        //        _currentHealthInsuranceNo = value;
        //        NotifyOfPropertyChange(() => CurrentHealthInsuranceNo);
        //    }
        //}


        private double? _hiBenefit;
        public double? HiBenefit
        {
            get { return _hiBenefit; }
            set
            {
                _hiBenefit = value;
                NotifyOfPropertyChange(() => HiBenefit);
                NotifyOfPropertyChange(() => HiBenefitString);
            }
        }

        private bool _isCrossRegion;
        public bool IsCrossRegion
        {
            get { return _isCrossRegion; }
            set
            {
                _isCrossRegion = value;
                NotifyOfPropertyChange(() => IsCrossRegion);
                NotifyOfPropertyChange(() => HiBenefitString);
            }
        }

        private string _HiComment;
        public string HiComment
        {
            get { return _HiComment; }
            set
            {
                if (_HiComment != value)
                {
                    _HiComment = value;
                    NotifyOfPropertyChange(() => HiBenefitString);
                    NotifyOfPropertyChange(() => HiComment);
                }
            }
        }

        public string HiBenefitString
        {
            get
            {
                if (HiBenefit == null || HiBenefit.Value <= 0)
                {
                    return string.Empty;
                }
                var converter = new PercentageConverter();
                var str = (string)converter.Convert(HiBenefit.Value, typeof(string), null, null);
                str += " (" + (IsCrossRegion ? eHCMSResources.G1687_G1_TraiTuyen.ToLower() : eHCMSResources.K3925_G1_DungTuyen.ToLower());
                if (!string.IsNullOrEmpty(HiComment))
                {
                    str += " - " + HiComment;
                }
                str += eHCMSResources.Z0027_G1_DauNgoacPhai;
                return str;
            }
        }

        private PaperReferal _confirmedPaperReferal;
        public PaperReferal ConfirmedPaperReferal
        {
            get { return _confirmedPaperReferal; }
            set
            {
                _confirmedPaperReferal = value;
                NotifyOfPropertyChange(() => ConfirmedPaperReferal);
            }
        }

        private Patient _newCurPatient = null;
        public Patient CurrentPatient
        {
            get
            {
                if (CS_DS != null)
                {
                    return CS_DS.CurrentPatient;
                }
                return _newCurPatient;
            }
            set
            {
                if (CS_DS != null)
                {
                    CS_DS.CurrentPatient = value;
                    NotifyOfPropertyChange(() => CurrentPatient);
                }
                else
                {
                    _newCurPatient = value;
                    NotifyOfPropertyChange(() => CurrentPatient);
                    //NotifyOfPropertyChange(() => CanhpNhomMau); //20181113 TBL: Comment ra de nhom mau luon duoc dieu chinh
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
                if (_currentRegistration != value)
                {
                    _currentRegistration = value;
                    NotifyOfPropertyChange(() => CurrentRegistration);
                }
            }
        }

        private PatientPCLRequest _CurrentPCLReq;
        public PatientPCLRequest CurrentPCLReq
        {
            get { return _CurrentPCLReq; }
            set
            {
                if (_CurrentPCLReq != value)
                {
                    _CurrentPCLReq = value;
                    NotifyOfPropertyChange(() => CurrentPCLReq);
                }
            }
        }

        #endregion


        private string _V_RegistrationTypeText;
        public string V_RegistrationTypeText
        {
            get { return _V_RegistrationTypeText; }
            set
            {
                if (_V_RegistrationTypeText != value)
                {
                    _V_RegistrationTypeText = value;
                    NotifyOfPropertyChange(() => V_RegistrationTypeText);
                }
            }
        }

        private string _PCLNum = "";
        public string PCLNum
        {
            get { return _PCLNum; }
            set
            {
                if (_PCLNum != value)
                {
                    _PCLNum = value;
                    NotifyOfPropertyChange(() => PCLNum);
                }
            }
        }

        private Visibility _IsShowPCL = Visibility.Collapsed;
        public Visibility IsShowPCL
        {
            get { return _IsShowPCL; }
            set
            {
                if (_IsShowPCL != value)
                {
                    _IsShowPCL = value;
                    NotifyOfPropertyChange(() => IsShowPCL);
                }
            }
        }

        public void SetPCLNum(string pCLNum)
        {
            PCLNum = pCLNum;
        }


        //KMx: Sự kiện này được nhận từ Out Standing Task bên Chẩn đoán hình ảnh. (24/05/2014 14:08).
        public void Handle(ShowPatientInfoFromPCLOutStandingTask message)
        {
            Patient curPatient = message.Patient;
            PatientRegistration curRegistration = message.PtRegistration;
            ShowPatientInfo(curPatient, curRegistration);
        }

        //KMx: Sự kiện này được nhận từ Pop-up tìm kiếm PCL Request. (24/05/2014 14:08).
        public void Handle(ShowPatientInfoFromPopUpSearchPCLRequest message)
        {
            Patient curPatient = message.Patient;
            PatientRegistration curRegistration = message.PtRegistration;
            ShowPatientInfo(curPatient, curRegistration);

        }

        //KMx: Sự kiện này được nhận từ TextBox tìm kiếm PCL Request. (24/05/2014 14:08).
        public void Handle(ShowPatientInfoFromTextBoxSearchPCLRequest message)
        {
            Patient curPatient = message.Patient;
            PatientRegistration curRegistration = message.PtRegistration;
            ShowPatientInfo(curPatient, curRegistration);

        }

        //KMx: Sự kiện này được nhận từ Module Khám bệnh (Ngoại Trú). (25/05/2014 11:33).
        public void Handle(ShowPatientInfoForConsultation message)
        {
            Patient curPatient = message.Patient;
            PatientRegistration curRegistration = message.PtRegistration;
            ShowPatientInfo(curPatient, curRegistration, true);
        }

        //KMx: Sự kiện này được nhận từ Module Khám bệnh (Nội Trú). (07/10/2014 15:53).
        public void Handle(ShowInPatientInfoForConsultation message)
        {
            Patient curPatient = message.Patient;
            PatientRegistration curRegistration = message.PtRegistration;
            ShowPatientInfo(curPatient, curRegistration, true);
        }
        public void Handle(ItemPatient1<Patient> message)
        {
            if (message != null)
            {
                CurrentPatient = message.Item;
            }
        }

        private void ShowPatientInfo(Patient curPatient, PatientRegistration curRegistration, bool IsConsultation = false)
        {
            //if (this.GetView() == null)
            //{
            //    //MessageBox.Show(string.Format("{0}. ", eHCMSResources.Z1392_G1_GetView) + eHCMSResources.Z1072_G1_TBaoQLyChTrKhiThayLoiNay, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
            //    return;
            //}

            if (curPatient == null)
            {
                return;
            }
            CurrentPatient = curPatient;
            CurrentRegistration = curRegistration;
            if (Globals.PatientPCLRequest_Result != null)
            {
                CurrentPCLReq = Globals.PatientPCLRequest_Result;
            }
            if (curRegistration != null && curRegistration.PtRegistrationID > 0)
            {
                //CurrentHealthInsuranceNo = curRegistration.HealthInsurance != null ? curRegistration.HealthInsurance.HICardNo : "";
                HiComment = curRegistration.HIComment;
                HiBenefit = curRegistration.PtInsuranceBenefit;
                IsCrossRegion = curRegistration.IsCrossRegion.GetValueOrDefault();
                SetPatientType(curPatient, curRegistration);
            }
            if (IsConsultation)
            {
                if (CS_DS != null)
                {
                    if (CS_DS.PatientMedicalRecordInfo == null && CS_DS.Getting_PatientMedicalRecordInfo == false)
                    {
                        CS_DS.Getting_PatientMedicalRecordInfo = true;
                        //▼====== #002 Comment hàm này ra thử xem có bị ảnh hưởng gì đến chương trình không vì thấy hàm này đi ko cần thiết.
                        //PatientMedicalRecords_ByPatientID(curPatient.PatientID, curRegistration);
                        //▲====== #002
                    }
                }
                else
                {
                    //▼====== #002
                    //PatientMedicalRecords_ByPatientID(curPatient.PatientID, curRegistration);
                    //▲====== #002
                }
            }
        }

        private void SetPatientType(Patient Patient, PatientRegistration PtRegistration)
        {
            if (Patient == null || PtRegistration == null || PtRegistration.PtRegistrationID <= 0)
            {
                V_RegistrationTypeText = "";
                return;
            }

            switch (PtRegistration.V_RegistrationType)
            {

                case AllLookupValues.RegistrationType.NGOAI_TRU:
                    {
                        V_RegistrationTypeText = eHCMSResources.K1194_G1_BNNgTru;
                        break;
                    }

                case AllLookupValues.RegistrationType.NOI_TRU:
                    {
                        V_RegistrationTypeText = eHCMSResources.K1197_G1_BNNoiTru;
                        break;
                    }
                    //case AllLookupValues.RegistrationType.DANGKY_VIP:
                    //{
                    //    V_RegistrationTypeText = eHCMSResources.K2869_G1_DKVip;
                    //    break;
                    //}

                    //case AllLookupValues.RegistrationType.NGOAI_TRU_NOI_TRU:
                    //    {
                    //        V_RegistrationTypeText = eHCMSResources.N0267_G1_NoiTru_TuNgTru;
                    //        break;
                    //    }
            }

        }

        //public void Handle(ShowPatientInfo<Patient, PatientRegistration, PatientRegistrationDetail> message)
        //{
        //    if (this.GetView() == null)
        //    {
        //        MessageBox.Show("GetView() of PatientInfoViewModel is null. Vui lòng thông báo cho người quản lý chương trình khi thấy lỗi này!", eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
        //        return;
        //    }
        //    if (message != null)
        //    {
        //        Globals.SetInfoPatient(message.Pt, message.PtReg, message.PtRegDetail);

        //        CurrentPatient = Registration_DataStorage.CurrentPatient;
        //        CurrentHealthInsuranceNo = Globals.CurrentHealthInsuranceNo;

        //        if (Registration_DataStorage.CurrentPatient != null)
        //        {
        //            //Khi chọn 1 BN thì Insert PMR  Nếu Chưa Insert
        //            bool HasRegistration = false;

        //            if (message.PtReg != null)
        //            {
        //                if (message.PtReg.PtRegistrationID > 0)
        //                {
        //                    HiComment = Registration_DataStorage.CurrentPatientRegistration.HIComment;
        //                    HiBenefit = Registration_DataStorage.CurrentPatientRegistration.PtInsuranceBenefit;
        //                    if (Registration_DataStorage.CurrentPatientRegistration.IsCrossRegion != null)
        //                    {
        //                        IsCrossRegion = Registration_DataStorage.CurrentPatientRegistration.IsCrossRegion.Value;
        //                    }
        //                    //SetTextLoaiBenhNhan();
        //                    SetPatientType(Registration_DataStorage.CurrentPatient, Registration_DataStorage.CurrentPatientRegistration);

        //                    HasRegistration = true;
        //                }
        //            }

        //            //if (Globals.ConsultationFormMode != ConsultationFormMode.ModeRUD)
        //            //{
        //            //----? ko biet de day lam gi

        //            //KMx: Khi đăng ký dịch vụ là đã tạo PMR rồi, không cần gọi hàm này để tạo nữa, chỉ cần load lên thôi. (13/05/2014 13:40).
        //            //GetNewPrescription(Registration_DataStorage.CurrentPatient.PatientID, HasRegistration);

        //            PatientMedicalRecords_ByPatientID(Registration_DataStorage.CurrentPatient.PatientID, HasRegistration);

        //            //KMx: Sau khi kiểm tra, thấy event bên dưới không còn sử dụng nữa nên comment ra (12/05/2014 15:05)
        //            //Globals.EventAggregator.Publish(new PatientChange());
        //            //}
        //        }
        //    }
        //}

        public void Handle(ShowPatientInfo<Patient> message)
        {
            CurrentPatient = (Patient)message.Pt;
            NotifyOfPropertyChange(() => CurrentPatient.PCLNum);
        }

        //private void SetTextLoaiBenhNhan()
        //{
        //    if (Registration_DataStorage.CurrentPatient != null)
        //    {
        //        if (Registration_DataStorage.CurrentPatientRegistration != null && Registration_DataStorage.CurrentPatientRegistration.PtRegistrationID > 0)
        //        {
        //            switch (Registration_DataStorage.CurrentPatientRegistration.V_RegistrationType)
        //            {
        //                //case AllLookupValues.RegistrationType.DANGKY_VIP:
        //                //{
        //                //    V_RegistrationTypeText = eHCMSResources.K2869_G1_DKVip;
        //                //    break;
        //                //}
        //                case AllLookupValues.RegistrationType.NGOAI_TRU:
        //                    {
        //                        V_RegistrationTypeText = eHCMSResources.K1194_G1_BNNgTru;
        //                        break;
        //                    }
        //                case AllLookupValues.RegistrationType.NOI_TRU:
        //                    {
        //                        V_RegistrationTypeText = eHCMSResources.K1197_G1_BNNoiTru;
        //                        break;
        //                    }
        //                //case AllLookupValues.RegistrationType.NGOAI_TRU_NOI_TRU:
        //                //    {
        //                //        V_RegistrationTypeText = eHCMSResources.N0267_G1_NoiTru_TuNgTru;
        //                //        break;
        //                //    }

        //            }
        //        }
        //        else
        //        {
        //            V_RegistrationTypeText = "";
        //        }
        //    }
        //    else
        //    {
        //        V_RegistrationTypeText = "";
        //    }
        //}

        public void hpNhomMau(object sender, RoutedEventArgs e)
        {
            //20181113 TBL: Tranh truong hop khong co BN ma lai bam vao 
            if (CurrentPatient == null)
            {
                return;
            }
            Action<IfrmBloodType> onInitDlg = delegate (IfrmBloodType frmBloodTypeVM)
            {
                this.ActivateItem(frmBloodTypeVM);
                frmBloodTypeVM.PatientInfo = CurrentPatient;
            };
            GlobalsNAV.ShowDialog<IfrmBloodType>(onInitDlg);
        }
        //20181113 TBL: Comment ra de nhom mau luon duoc dieu chinh
        //public bool CanhpNhomMau
        //{
        //    get { return CurrentPatient != null ? true : false; }
        //}

        public void InitData()
        {
            if (Registration_DataStorage == null)
            {
                return;
            }
            CurrentPatient = Registration_DataStorage.CurrentPatient;
            CurrentRegistration = Registration_DataStorage.CurrentPatientRegistration;
            HiComment = CurrentRegistration.HIComment;
            HiBenefit = CurrentRegistration.PtInsuranceBenefit;
            IsCrossRegion = CurrentRegistration.IsCrossRegion.GetValueOrDefault();

            //SetTextLoaiBenhNhan();
            SetPatientType(Registration_DataStorage.CurrentPatient, Registration_DataStorage.CurrentPatientRegistration);
        }

        //public void InitData()
        //{
        //    CurrentPatient = Registration_DataStorage.CurrentPatient;
        //    CurrentHealthInsuranceNo = Globals.CurrentHealthInsuranceNo;
        //    HiComment = Registration_DataStorage.CurrentPatientRegistration.HIComment;
        //    HiBenefit = Registration_DataStorage.CurrentPatientRegistration.PtInsuranceBenefit;
        //    if (Registration_DataStorage.CurrentPatientRegistration.IsCrossRegion != null)
        //    {
        //        IsCrossRegion = Registration_DataStorage.CurrentPatientRegistration.IsCrossRegion.Value;
        //    }

        //    //SetTextLoaiBenhNhan();
        //    SetPatientType(Registration_DataStorage.CurrentPatient, Registration_DataStorage.CurrentPatientRegistration);
        //}

        #region "Khi chọn 1 BN thì Insert PMR  Nếu Chưa Insert"
        //private void GetNewPrescription(long PatientID, bool HasRegistration)
        //{
        //    Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "Tạo PatientMedicalRecords..." });
        //    var t = new Thread(() =>
        //    {
        //        using (var serviceFactory = new ePrescriptionsServiceClient())
        //        {
        //            var contract = serviceFactory.ServiceInstance;
        //            contract.BeginGetNewPrescriptionByPtID(PatientID, 0, Globals.DispatchCallback((asyncResult) =>
        //            {
        //                try
        //                {
        //                    contract.EndGetNewPrescriptionByPtID(asyncResult);

        //                    PatientMedicalRecords_ByPatientID(Registration_DataStorage.CurrentPatient.PatientID, HasRegistration);

        //                }
        //                catch (Exception ex)
        //                {
        //                    MessageBox.Show(ex.Message);
        //                }
        //                finally
        //                {
        //                    Globals.IsBusy = false;
        //                }

        //            }), null);

        //        }

        //    });

        //    t.Start();
        //}
        #endregion

        #region Đọc thông tin PMR

        private void PatientMedicalRecords_ByPatientID(long PatientID, PatientRegistration PtRegistration)
        {
            this.ShowBusyIndicator();

            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ePMRsServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginPatientMedicalRecords_ByPatientID(PatientID, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                Globals.PatientMedicalRecordInfo = contract.EndPatientMedicalRecords_ByPatientID(asyncResult);

                                if (PtRegistration != null && PtRegistration.PtRegistrationID > 0)
                                {
                                    Globals.PatientMedicalRecordInfo.PtRegistrationID = PtRegistration.PtRegistrationID;
                                }
                                else
                                {
                                    Globals.PatientMedicalRecordInfo.PtRegistrationID = 0;
                                }

                                if (CS_DS != null)
                                {
                                    CS_DS.PatientMedicalRecordInfo = Globals.PatientMedicalRecordInfo;
                                }
                                Globals.EventAggregator.Publish(new ShowPMRForConsultation());

                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
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

        #endregion


        public void EditGeneralInfoCmd()
        {
            if (CurrentPatient == null)
                return;

            // Txd 14/07/2014: Commented the following block of code and replaced 
            //                  with a a call to Coroutine method OpenPatientDetailDialog

            //var patientDetailsVm = Globals.GetViewModel<IPatientDetails>();
            //patientDetailsVm.CurrentAction = eHCMSResources.Z0971_G1_CNhatTTinBN;

            //patientDetailsVm.FormState = FormState.EDIT;
            //patientDetailsVm.CloseWhenFinish = true;
            //patientDetailsVm.StartEditingPatientLazyLoad(CurrentPatient);
            //patientDetailsVm.ActiveTab = PatientInfoTabs.GENERAL_INFO;
            ////patientDetailsVm.ActivationMode = ActivationMode.EDIT_PATIENT_GENERAL_INFO;
            //patientDetailsVm.ActivationMode = ActivationMode.PATIENT_GENERAL_HI_VIEW;
            //Globals.ShowDialog(patientDetailsVm as Conductor<object>);

            Coroutine.BeginExecute(OpenPatientDetailDialog());
        }

        private IEnumerator<IResult> OpenPatientDetailDialog()
        {
            var patientDetailsVm = Globals.GetViewModel<IPatientDetails>();
            patientDetailsVm.CurrentAction = eHCMSResources.Z0971_G1_CNhatTTinBN;

            patientDetailsVm.FormState = FormState.EDIT;
            patientDetailsVm.CloseWhenFinish = true;
            patientDetailsVm.InitLoadControlData_FromExt(null);

            yield return new GenericCoRoutineTask(patientDetailsVm.LoadPatientDetailsAndHI_GenAction, CurrentPatient, true);

            patientDetailsVm.ActiveTab = PatientInfoTabs.GENERAL_INFO;
            //patientDetailsVm.ActivationMode = ActivationMode.EDIT_PATIENT_GENERAL_INFO;
            patientDetailsVm.ActivationMode = ActivationMode.PATIENT_GENERAL_HI_VIEW;
            //Globals.ShowDialog(patientDetailsVm as Conductor<object>);
            GlobalsNAV.ShowDialog_V3<IPatientDetails>(patientDetailsVm);
            yield break;
        }


        public void Handle(PatientSelectedGoToKhamBenh_InPt<PatientRegistration> message)
        {
            if (this.GetView() != null && message != null && message.Item != null)
            {
                CurrentPatient = message.Item.Patient;
                Registration_DataStorage.CurrentPatientRegistration = message.Item;
                Registration_DataStorage.CurrentPatientRegistration.Patient = message.Item.Patient;
                Registration_DataStorage.CurrentPatientRegistrationDetail = null;
                NotifyOfPropertyChange(() => CurrentPatient);
            }
        }

        public void Handle(PatientReloadEvent message)
        {
            if (message != null)
            {
                CurrentPatient = message.curPatient;
            }
        }

        private IRegistration_DataStorage _Registration_DataStorage;
        public IRegistration_DataStorage Registration_DataStorage
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
            }
        }

        /*▼====: #003*/
        //TBL: Sau khi cập nhật thông tin thì nhận event từ PatientDetailsViewModel để load lại thông tin mới nhất của bệnh nhân
        public void Handle(UpdateCompleted<Patient> message)
        {
            if (message != null && message.Item != null)
            {
                var p = _newCurPatient;
                if (p != null && p.PatientID == message.Item.PatientID)
                {
                    CurrentPatient = ObjectCopier.DeepCopy(message.Item);
                    NotifyOfPropertyChange(() => CurrentPatient);
                }
            }
        }
        /*▲====: #003*/

        private Visibility _IsShowPCL_V2 = Visibility.Collapsed;
        public Visibility IsShowPCL_V2
        {
            get { return _IsShowPCL_V2; }
            set
            {
                if (_IsShowPCL_V2 != value)
                {
                    _IsShowPCL_V2 = value;
                    NotifyOfPropertyChange(() => IsShowPCL_V2);
                }
            }
        }

        public Visibility IsShowPCLSumary
        {
            get
            {
                if(IsShowPCL == Visibility.Visible || IsShowPCL_V2 == Visibility.Visible)
                {
                    return Visibility.Visible;
                }
                else
                {
                    return Visibility.Collapsed;
                }
            }
        }
    }
}