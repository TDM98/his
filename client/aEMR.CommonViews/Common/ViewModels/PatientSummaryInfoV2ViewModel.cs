using eHCMSLanguage;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Windows;
using Caliburn.Micro;
using DataEntities;
using aEMR.ViewContracts;
using aEMR.Infrastructure.Events;
using Castle.Windsor;
using aEMR.Infrastructure.CachingUtils;
using aEMR.Common.Converters;
using aEMR.Infrastructure;
using aEMR.CommonTasks;
using System;
using aEMR.Common.BaseModel;
/*
* 20170627 #002 CMN:    Added event for ConsultingDiagnosys
* 20171004 #003 CMN:    Reverted changes make duplicate screen IFindAppointment.
* 20190727 #004 TTM:    BM 0012990: Bổ sung SCAN cho nội trú.
* 20200420 #005 TNHX:  Thêm địa chỉ full cho in thông tin bệnh nhân
*/
namespace aEMR.Common.ViewModels
{
    [Export(typeof(IPatientSummaryInfoV2)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class PatientSummaryInfoV2ViewModel : ViewModelBase, IPatientSummaryInfoV2
        , IHandle<ItemSelected<Patient>>
        , IHandle<ItemPatient<Patient>>
        , IHandle<PatientReloadEvent>
        /*▼====: #003*/
        //, IHandle<ResultFound<Patient>>
        /*▲====: #003*/
        , IHandle<UpdateCompleted<Patient>>

    {
        [ImportingConstructor]
        public PatientSummaryInfoV2ViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg, ISalePosCaching salePosCaching)
        {
            eventArg.Subscribe(this);

            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                Coroutine.BeginExecute(DoLoadDataForTheFirstTime());
            }
            authorization();

        }

        private PatientHI_SummaryInfo _PtHISumInfo;
        public PatientHI_SummaryInfo PtHISumInfo
        {
            get
            {
                return _PtHISumInfo;
            }
            set
            {
                _PtHISumInfo = value;
                NotifyOfPropertyChange(() => PtHISumInfo);
            }
        }
        private string _GeneralHeaderInfo;
        public string GeneralHeaderInfo
        {
            get { return _GeneralHeaderInfo; }
            set
            {
                if (_GeneralHeaderInfo != value)
                {
                    _GeneralHeaderInfo = value;
                    NotifyOfPropertyChange(() => GeneralHeaderInfo);
                }
            }
        }

        public void SetPatientHISumInfo(PatientHI_SummaryInfo PtHISumInfo)
        {
            if (PtHISumInfo == null)
            {
                IsCrossRegion = false;
                ConfirmedHiItem = null;
                ConfirmedHiItem_2 = null;
                ConfirmedHiItem_3 = null;
                ConfirmedPaperReferal = null;
                ConfirmedPaperReferral_2 = null;
                ConfirmedPaperReferral_3 = null;
                HiBenefit = null;
                HiBenefit_2 = null;
                HiBenefit_3 = null;
                return;
            }

            IsCrossRegion = PtHISumInfo.IsCrossRegion;
            ConfirmedHiItem = PtHISumInfo.ConfirmedHiItem;
            ConfirmedHiItem_2 = PtHISumInfo.ConfirmedHiItem_2;
            ConfirmedHiItem_3 = PtHISumInfo.ConfirmedHiItem_3;
            ConfirmedPaperReferal = PtHISumInfo.ConfirmedPaperReferal;
            ConfirmedPaperReferral_2 = PtHISumInfo.ConfirmedPaperReferral_2;
            ConfirmedPaperReferral_3 = PtHISumInfo.ConfirmedPaperReferral_3;
            HiBenefit = PtHISumInfo.HiBenefit;
            HiBenefit_2 = PtHISumInfo.HiBenefit_2;
            HiBenefit_3 = PtHISumInfo.HiBenefit_3;
            if (CurrentPatient != null)
            {
                GeneralHeaderInfo = CurrentPatient.GeneralHeaderInfoInPt;
                if (ConfirmedHiItem != null && ConfirmedHiItem.HICardNo.Length > 0)
                {
                    GeneralHeaderInfo += string.Format(" - {0}: ", eHCMSResources.K1209_G1_BH.ToUpper()) + ConfirmedHiItem.HICardNo;
                }
                if (HiBenefitString.Length > 0)
                {
                    GeneralHeaderInfo += string.Format(" - {0}: ", eHCMSResources.Q0421_G1_QL) + HiBenefitString;
                }
            }

        }

        private bool _ThongTuyen = false;
        public bool ThongTuyen
        {
            get
            {
                return _ThongTuyen;
            }
            set
            {
                _ThongTuyen = value;
                NotifyOfPropertyChange(() => ThongTuyen);
                NotifyOfPropertyChange(() => HiBenefitString);
            }
        }

        //public string HiBenefitString
        //{
        //    get
        //    {
        //        if(HiBenefit == null || HiBenefit.Value <= 0)
        //        {
        //            return string.Empty;
        //        }
        //        var converter = new PercentageConverter();
        //        var str = (string)converter.Convert(HiBenefit.Value, typeof(string), null, null);
        //        str += " (" + (ThongTuyen ? "thông tuyến" : (IsCrossRegion ? eHCMSResources.G1687_G1_TraiTuyen.ToLower() : eHCMSResources.K3925_G1_DungTuyen.ToLower()));
        //        //str += " (" + (IsCrossRegion ? eHCMSResources.G1687_G1_TraiTuyen.ToLower() : (ThongTuyen ? "thông tuyến" : eHCMSResources.K3925_G1_DungTuyen.ToLower()));
        //        if(!string.IsNullOrEmpty(HiComment))
        //        {
        //            str += " - " + HiComment;
        //        }
        //        str += eHCMSResources.Z0027_G1_DauNgoacPhai.ToUpper();
        //        return str;
        //    }
        //}

        public string HiBenefitString
        {
            get
            {
                if (HiBenefit == null || HiBenefit.Value <= 0)
                {
                    return string.Empty;
                }
                var converter = new PercentageConverter();
                string strBenefitPercent = (string)converter.Convert(HiBenefit.Value, typeof(string), null, null);
                if (HiBenefit_2.HasValue && HiBenefit_2.Value > 0)
                {
                    string strTmp = strBenefitPercent;
                    var strBenefitPercent2 = (string)converter.Convert(HiBenefit_2.Value, typeof(string), null, null);
                    strBenefitPercent = "[" + strTmp + ", " + strBenefitPercent2 + "]";
                }
                if (HiBenefit_3.HasValue && HiBenefit_3.Value > 0)
                {
                    string strTmp = strBenefitPercent;
                    var strBenefitPercent3 = (string)converter.Convert(HiBenefit_3.Value, typeof(string), null, null);
                    strBenefitPercent = "[" + strTmp + ", " + strBenefitPercent3 + "]";
                }

                strBenefitPercent += eHCMSResources.Z0028_G1_DauNgoacTrai + (IsCrossRegion ? eHCMSResources.G1687_G1_TraiTuyen.ToLower() : eHCMSResources.K3925_G1_DungTuyen.ToLower());
                if (!string.IsNullOrEmpty(HiComment))
                {
                    strBenefitPercent += " - " + HiComment;
                }

                strBenefitPercent += eHCMSResources.Z0027_G1_DauNgoacPhai;
                return strBenefitPercent;
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

        private bool _displayButtons = true;
        public bool DisplayButtons
        {
            get { return _displayButtons; }
            set { _displayButtons = value; NotifyOfPropertyChange(() => DisplayButtons); }
        }

        private Patient _currentPatient;
        public Patient CurrentPatient
        {
            get { return _currentPatient; }
            set
            {
                //if (_currentPatient == value)
                //{
                //    return;
                //}
                _currentPatient = value;
                NotifyOfPropertyChange(() => HiComment);
                NotifyOfPropertyChange(() => HiBenefit);

                NotifyOfPropertyChange(() => CurrentPatient);
                if (CurrentPatient != null)
                {
                    GeneralHeaderInfo = CurrentPatient.GeneralInfoString;
                }
                if (_displayButtons)
                {
                    if (_currentPatient != null)
                    {
                        if (mInfo_CapNhatThongTinBN)
                        {
                            GeneralInfoVisibility = Visibility.Visible;
                        }
                        if (mInfo_XacNhan)
                        {
                            ConfirmHiVisibility = Visibility.Visible;
                        }
                        if (mInfo_XoaThe)
                        {
                            DeleteHiVisibility = Visibility.Visible;
                        }

                        ConfirmPaperReferalVisibility = Visibility.Visible;
                    }
                    else
                    {
                        GeneralInfoVisibility = Visibility.Collapsed;
                        ConfirmHiVisibility = Visibility.Collapsed;
                        ConfirmPaperReferalVisibility = Visibility.Collapsed;
                        DeleteHiVisibility = Visibility.Collapsed;
                    }
                }
            }
        }

        private string _strConfirmedHICardNums = "";
        public string ConfirmedHICardNums
        {
            get
            {
                _strConfirmedHICardNums = "";
                if (ConfirmedHiItem != null)
                    _strConfirmedHICardNums = ConfirmedHiItem.HICardNo;
                if (ConfirmedHiItem_2 != null)
                    _strConfirmedHICardNums += "; " + ConfirmedHiItem_2.HICardNo;
                if (ConfirmedHiItem_3 != null)
                    _strConfirmedHICardNums += "; " + ConfirmedHiItem_3.HICardNo;
                return _strConfirmedHICardNums;
            }

        }

        private string _strPaperRefDetails = "";
        public string PaperRefDetails
        {
            get
            {
                _strPaperRefDetails = "";
                if (ConfirmedPaperReferal != null)
                    _strPaperRefDetails = ConfirmedPaperReferal.IssuerLocation;
                if (ConfirmedPaperReferral_2 != null)
                    _strPaperRefDetails += "; " + ConfirmedPaperReferral_2.IssuerLocation;
                if (ConfirmedPaperReferral_3 != null)
                    _strPaperRefDetails += "; " + ConfirmedPaperReferral_3.IssuerLocation;

                return _strPaperRefDetails;
            }
        }

        private HealthInsurance _confirmedHiItem;
        public HealthInsurance ConfirmedHiItem
        {
            get { return _confirmedHiItem; }
            set
            {
                _confirmedHiItem = value;
                NotifyOfPropertyChange(() => ConfirmedHiItem);
                if (ConfirmedHiItem != null && ConfirmedHiItem.HICardNo != "")
                {
                    if (CurrentPatient == null)
                    {
                        CurrentPatient = new Patient();
                    }
                    if (!CurrentPatient.GeneralInfoString.Contains(ConfirmedHiItem.HICardNo))
                    {
                        CurrentPatient.GeneralInfoString += string.Format(" - {0}:", eHCMSResources.K1209_G1_BH.ToUpper()) + ConfirmedHiItem.HICardNo;
                        NotifyOfPropertyChange(() => CurrentPatient.GeneralInfoString);
                        GeneralHeaderInfo = CurrentPatient.GeneralInfoString;
                    }
                }
                NotifyOfPropertyChange(() => CanRemoveConfirmedHiItem);
                NotifyOfPropertyChange(() => ConfirmedHICardNums);
            }
        }

        private HealthInsurance _confirmedHiItem_2;
        public HealthInsurance ConfirmedHiItem_2
        {
            get
            {
                return _confirmedHiItem_2;
            }
            set
            {
                _confirmedHiItem_2 = value;
                NotifyOfPropertyChange(() => ConfirmedHICardNums);
            }
        }

        private HealthInsurance _confirmedHiItem_3;
        public HealthInsurance ConfirmedHiItem_3
        {
            get
            {
                return _confirmedHiItem_3;
            }
            set
            {
                _confirmedHiItem_3 = value;
                NotifyOfPropertyChange(() => ConfirmedHICardNums);
            }
        }

        private PaperReferal _confirmedPaperReferal;
        public PaperReferal ConfirmedPaperReferal
        {
            get { return _confirmedPaperReferal; }
            set
            {
                _confirmedPaperReferal = value;
                NotifyOfPropertyChange(() => PaperRefDetails);
            }
        }
        private PaperReferal _confirmedPaperReferral_2;
        public PaperReferal ConfirmedPaperReferral_2
        {
            get { return _confirmedPaperReferral_2; }
            set
            {
                _confirmedPaperReferral_2 = value;
                NotifyOfPropertyChange(() => PaperRefDetails);
            }
        }
        private PaperReferal _confirmedPaperReferral_3;
        public PaperReferal ConfirmedPaperReferral_3
        {
            get { return _confirmedPaperReferral_3; }
            set
            {
                _confirmedPaperReferral_3 = value;
                NotifyOfPropertyChange(() => PaperRefDetails);
            }
        }

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
        private double? _hiBenefit_2;
        public double? HiBenefit_2
        {
            get { return _hiBenefit_2; }
            set
            {
                _hiBenefit_2 = value;
                NotifyOfPropertyChange(() => HiBenefit_2);
                NotifyOfPropertyChange(() => HiBenefitString);
            }
        }
        private double? _hiBenefit_3;
        public double? HiBenefit_3
        {
            get { return _hiBenefit_3; }
            set
            {
                _hiBenefit_3 = value;
                NotifyOfPropertyChange(() => HiBenefit_3);
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

        private ObservableCollection<PatientClassification> _patientClassifications;

        public ObservableCollection<PatientClassification> PatientClassifications
        {
            get { return _patientClassifications; }
            set
            {
                _patientClassifications = value;
                NotifyOfPropertyChange(() => PatientClassifications);
            }
        }

        private Visibility _generalInfoVisibility = Visibility.Collapsed;
        public Visibility GeneralInfoVisibility
        {
            get { return _generalInfoVisibility; }
            set
            {
                _generalInfoVisibility = value;
                NotifyOfPropertyChange(() => GeneralInfoVisibility);
            }
        }

        private Visibility _confirmHiVisibility = Visibility.Collapsed;
        public Visibility ConfirmHiVisibility
        {
            get { return _confirmHiVisibility; }
            set
            {
                _confirmHiVisibility = value;
                NotifyOfPropertyChange(() => ConfirmHiVisibility);
            }
        }

        private Visibility _deleteHiVisibility = Visibility.Collapsed;
        public Visibility DeleteHiVisibility
        {
            get { return _deleteHiVisibility; }
            set
            {
                _deleteHiVisibility = value;
                NotifyOfPropertyChange(() => DeleteHiVisibility);
            }
        }

        private Visibility _confirmPaperReferalVisibility = Visibility.Collapsed;
        public Visibility ConfirmPaperReferalVisibility
        {
            get { return _confirmPaperReferalVisibility; }
            set
            {
                _confirmPaperReferalVisibility = value;
                NotifyOfPropertyChange(() => ConfirmPaperReferalVisibility);
            }
        }

        public void lnkEditPatientInfo1()
        {
            EditGeneralInfoCmd();
        }
        public void EditGeneralInfoCmd()
        {
            if (CurrentPatient == null)
            {
                return;
            }

            // Txd 14/07/2014: Commented the following block of code and replaced 
            //                  with a a call to Coroutine method OpenHI_ConfirmDialog

            //var patientDetailsVm = Globals.GetViewModel<IPatientDetails>();
            //patientDetailsVm.CurrentAction = eHCMSResources.Z0971_G1_CNhatTTinBN;

            //patientDetailsVm.FormState = FormState.EDIT;
            //patientDetailsVm.CloseWhenFinish = true;
            //patientDetailsVm.IsChildWindow = true;            
            //patientDetailsVm.StartEditingPatientLazyLoad(CurrentPatient);
            //patientDetailsVm.ActiveTab = PatientInfoTabs.GENERAL_INFO;
            ////patientDetailsVm.ActivationMode = CanConfirmHi ? ActivationMode.EDIT_PATIENT_FOR_REGISTRATION : ActivationMode.EDIT_PATIENT_GENERAL_INFO;
            //patientDetailsVm.ActivationMode = CanConfirmHi ? ActivationMode.EDIT_PATIENT_FOR_REGISTRATION : ActivationMode.PATIENT_GENERAL_HI_VIEW;
            //Globals.ShowDialog(patientDetailsVm as Conductor<object>);

            Coroutine.BeginExecute(OpenPatientDetailDialog());

        }

        private IEnumerator<IResult> OpenPatientDetailDialog()
        {
            var patientDetailsVm = Globals.GetViewModel<IPatientDetails>();
            patientDetailsVm.CurrentAction = eHCMSResources.Z0971_G1_CNhatTTinBN;

            patientDetailsVm.FormState = FormState.EDIT;
            patientDetailsVm.CloseWhenFinish = true;
            patientDetailsVm.IsChildWindow = true;

            yield return new GenericCoRoutineTask(patientDetailsVm.LoadPatientDetailsAndHI_GenAction, CurrentPatient, false);

            patientDetailsVm.ActiveTab = PatientInfoTabs.GENERAL_INFO;
            //patientDetailsVm.ActivationMode = CanConfirmHi ? ActivationMode.EDIT_PATIENT_FOR_REGISTRATION : ActivationMode.EDIT_PATIENT_GENERAL_INFO;
            //patientDetailsVm.ActivationMode = CanConfirmHi ? ActivationMode.EDIT_PATIENT_FOR_REGISTRATION : ActivationMode.PATIENT_GENERAL_HI_VIEW;
            patientDetailsVm.ActivationMode = ActivationMode.PATIENT_GENERAL_HI_VIEW;
            GlobalsNAV.ShowDialog_V3<IPatientDetails>(patientDetailsVm);

            yield break;

        }

        public void ConfirmHiCmd()
        {
            if (CurrentPatient == null)
            {
                return;
            }

            // Txd 14/07/2014: Commented the following block of code and replaced 
            //                  with a a call to Coroutine method OpenHI_ConfirmDialog

            //var patientDetailsVm = Globals.GetViewModel<IPatientDetails>();
            //patientDetailsVm.IsChildWindow = true;
            //if (Enable_ReConfirmHI_InPatientOnly)
            //{
            //    patientDetailsVm.Enable_ReConfirmHI_InPatientOnly = true;
            //    patientDetailsVm.ActivationMode = ActivationMode.REGISTRATION_CONFIRM_HI;
            //}

            //patientDetailsVm.CurrentAction = eHCMSResources.Z0971_G1_CNhatTTinBN;

            //patientDetailsVm.FormState = FormState.EDIT;
            //patientDetailsVm.CloseWhenFinish = true;
            ////patientDetailsVm.IsChildWindow = true;            
            //patientDetailsVm.StartEditingPatientLazyLoad(CurrentPatient);
            //patientDetailsVm.ActiveTab = PatientInfoTabs.HEALTH_INSURANCE_INFO;
            //patientDetailsVm.ActivationMode = ActivationMode.CONFIRM_HEALTH_INSURANCE;

            //Globals.ShowDialog(patientDetailsVm as Conductor<object>);
            Coroutine.BeginExecute(OpenHI_ConfirmDialog());
        }

        private IEnumerator<IResult> OpenHI_ConfirmDialog()
        {
            if (Enable_ReConfirmHI_InPatientOnly)
            {
                //var patientDetailsVm = Globals.GetViewModel<IPatientDetails>();

                Action<IPatientDetails> onInitDlg = delegate (IPatientDetails patientDetailsVm)
                {
                    patientDetailsVm.IsChildWindow = true;
                    patientDetailsVm.Enable_ReConfirmHI_InPatientOnly = true;
                    patientDetailsVm.ActivationMode = ActivationMode.REGISTRATION_CONFIRM_HI;

                    patientDetailsVm.CurrentAction = eHCMSResources.Z0971_G1_CNhatTTinBN;

                    patientDetailsVm.FormState = FormState.EDIT;
                    patientDetailsVm.CloseWhenFinish = true;

                    patientDetailsVm.InitLoadControlData_FromExt(null);

                    patientDetailsVm.LoadPatientDetailsAndHI_V2(CurrentPatient, false);

                    patientDetailsVm.ShowExtraConfirmHI_Fields = true;

                    patientDetailsVm.RegistrationType = AllLookupValues.RegistrationType.NGOAI_TRU;

                    //patientDetailsVm.ActiveTab = PatientInfoTabs.HEALTH_INSURANCE_INFO;

                };
                GlobalsNAV.ShowDialog<IPatientDetails>(onInitDlg);
            }

            yield break;
        }

        //public void ConfirmPaperReferalCmd()
        //{
        //    var patientDetailsVm = Globals.GetViewModel<IPatientDetails>();

        //    patientDetailsVm.FormState = FormState.EDIT;
        //    patientDetailsVm.CloseWhenFinish = true;
        //    patientDetailsVm.StartEditingPatientLazyLoad(CurrentPatient);
        //    patientDetailsVm.ActiveTab = PatientInfoTabs.PAPER_REFERRAL_INFO;
        //    patientDetailsVm.ActivationMode = ActivationMode.CONFIRM_PAPER_REFERAL;

        //    Globals.ShowDialog(patientDetailsVm as Conductor<object>);
        //}

        public void Handle(ItemSelected<Patient> message)
        {
            if (message != null)
            {
                CurrentPatient = message.Item;
            }
        }

        public void Handle(PatientReloadEvent message)
        {
            if (message != null)
            {
                CurrentPatient = message.curPatient;
            }
        }


        private bool _canConfirmHi;

        public bool CanConfirmHi
        {
            get { return _canConfirmHi; }
            set
            {
                _canConfirmHi = value;
                NotifyOfPropertyChange(() => CanConfirmHi);
                NotifyOfPropertyChange(() => CanRemoveConfirmedHiItem);
            }
        }

        public bool CanRemoveConfirmedHiItem
        {
            get
            {
                return _canConfirmHi && ConfirmedHiItem != null;
            }
        }

        //private bool _canConfirmPaperReferal;
        //public bool CanConfirmPaperReferal
        //{
        //    get { return _canConfirmPaperReferal; }
        //    set
        //    {
        //        _canConfirmPaperReferal = value;
        //        NotifyOfPropertyChange(()=>CanConfirmPaperReferal);
        //    }
        //}

        public void Handle(UpdateCompleted<Patient> message)
        {
            if (message != null && message.Item != null)
            {
                var p = _currentPatient;
                if (p != null && p.PatientID == message.Item.PatientID)
                {
                    CurrentPatient = ObjectCopier.DeepCopy(message.Item);
                }
            }
        }
        /*▼====: #003*/
        //public void Handle(ResultFound<Patient> message)
        //{
        //    if (message != null)
        //    {

        //        CurrentPatient = message.Result;
        //        if (CurrentPatient != null)
        //        {
        //            //SetCurrentPatient(CurrentPatient);
        //            Globals.EventAggregator.Publish(new ItemSelected<Patient>() { Item = CurrentPatient });
        //        }
        //    }
        //}
        /*▲====: #003*/
        public void Handle(ItemPatient<Patient> message)
        {
            if (message != null)
            {
                CurrentPatient = message.Item;
            }
        }

        public void ExpandCmd()
        {
            var view = GetView() as IPatientSummaryInfoV2View;
            if (view != null)
            {
                view.Switch(true);
            }
        }
        public void CollapseCmd()
        {
            var view = GetView() as IPatientSummaryInfoV2View;
            if (view != null)
            {
                view.Switch(false);
            }
        }
        public void XemPhongCmd()
        {
            GlobalsNAV.ShowDialog<IConsultationRoom>();
        }


        public void authorization()
        {
            if (!Globals.isAccountCheck)
            {
                return;
            }
        }
        #region checking account

        private bool _mInfo_CapNhatThongTinBN = true;
        private bool _mInfo_XacNhan = true;
        private bool _mInfo_XoaThe = true;
        private bool _mInfo_XemPhongKham = true;

        public bool mInfo_CapNhatThongTinBN
        {
            get
            {
                return _mInfo_CapNhatThongTinBN;
            }
            set
            {
                //if (_mInfo_CapNhatThongTinBN == value)
                //    return;
                _mInfo_CapNhatThongTinBN = value;
                GeneralInfoVisibility = Globals.convertVisibility(mInfo_CapNhatThongTinBN);
                NotifyOfPropertyChange(() => mInfo_CapNhatThongTinBN);
            }
        }

        // TxD 09/07/2014 : Added the following Property to enable ReConfirm HI Benefit for InPatient ONLY
        private bool _Enable_ReConfirmHI_InPatientOnly = true;
        public bool Enable_ReConfirmHI_InPatientOnly
        {
            get
            {
                return _Enable_ReConfirmHI_InPatientOnly;
            }
            set
            {
                _Enable_ReConfirmHI_InPatientOnly = value;
            }
        }

        public bool mInfo_XacNhan
        {
            get
            {
                // TxD 09/07/2014 : Added the following to enable ReConfirm HI Benefit for InPatient ONLY
                if (Enable_ReConfirmHI_InPatientOnly)
                {
                    return _mInfo_XacNhan;
                }

                //return _mInfo_XacNhan && Globals.IsConfirmHI;
                // Txd 25/05/2014 Replaced ConfigList
                return _mInfo_XacNhan && Globals.ServerConfigSection.Hospitals.IsConfirmHI;
            }
            set
            {
                if (_mInfo_XacNhan == value)
                    return;
                _mInfo_XacNhan = value;
                ConfirmHiVisibility = Globals.convertVisibility(mInfo_XacNhan);
                NotifyOfPropertyChange(() => mInfo_XacNhan);
            }
        }


        public bool mInfo_XoaThe
        {
            get
            {
                return _mInfo_XoaThe;
            }
            set
            {
                if (_mInfo_XoaThe == value)
                    return;
                _mInfo_XoaThe = value;
                DeleteHiVisibility = Globals.convertVisibility(mInfo_XoaThe);
                NotifyOfPropertyChange(() => mInfo_XoaThe);
            }
        }


        public bool mInfo_XemPhongKham
        {
            get
            {
                return _mInfo_XemPhongKham;
            }
            set
            {
                if (_mInfo_XemPhongKham == value)
                    return;
                _mInfo_XemPhongKham = value;
                NotifyOfPropertyChange(() => mInfo_XemPhongKham);
            }
        }




        #endregion

        public void RemoveConfirmedHiItemCmd()
        {
            Globals.EventAggregator.Publish(new HiCardConfirmedEvent { HiProfile = null, PaperReferal = null });
        }


        private PatientClassification _currentPatientClassification;

        public PatientClassification CurrentPatientClassification
        {
            get { return _currentPatientClassification; }
            set
            {
                _currentPatientClassification = value;
                NotifyOfPropertyChange(() => CurrentPatientClassification);
            }
        }

        public IEnumerator<IResult> DoLoadDataForTheFirstTime()
        {
            yield return Loader.Show(string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.Z0669_G1_DangLayDLieu));
            //LoadDepartmentsTask departmentTask = new LoadDepartmentsTask();
            //yield return departmentTask;

            var patientClassificationTask = new LoadPatientClassificationsTask();
            yield return patientClassificationTask;

            //Departments = departmentTask.Departments;

            PatientClassifications = patientClassificationTask.PatientClassifications;
            //ResetDepartmentToDefaultValue();

            yield return Loader.Hide();
        }

        public void PrintPatientInfoCmd()
        {
            if (CurrentPatient == null)
            {
                //MessageBox.Show(string.Format("{0}.", eHCMSResources.A0666_G1_Msg_InfoKhCoTTinBN), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }

            Action<ICommonPreviewView> onInitDlg = delegate (ICommonPreviewView proAlloc)
            {
                if (Globals.ServerConfigSection.CommonItems.PrintPatientInfoOption == 1)
                {
                    proAlloc.FileCodeNumber = CurrentPatient.PatientCode;
                }
                else if (Globals.ServerConfigSection.CommonItems.PrintPatientInfoOption == 2)
                {
                    proAlloc.FileCodeNumber = CurrentPatient.FileCodeNumber;
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(CurrentPatient.FileCodeNumber))
                    {
                        proAlloc.FileCodeNumber = CurrentPatient.FileCodeNumber;
                    }
                    else
                    {
                        proAlloc.FileCodeNumber = CurrentPatient.PatientCode;
                    }
                }

                int MinAge = 2;
                int Age = 0;
                string strUnit = "";

                System.DateTime DOB = CurrentPatient.DOB.GetValueOrDefault();
                bool AgeOnly = CurrentPatient.AgeOnly.GetValueOrDefault();

                if (DOB != System.DateTime.MinValue && DOB <= Globals.GetCurServerDateTime())
                {
                    if (AgeOnly || Globals.GetCurServerDateTime().Year - DOB.Year >= MinAge)
                    {
                        Age = Globals.GetCurServerDateTime().Year - DOB.Year;
                        strUnit = eHCMSResources.G2057_G1_Tuoi.ToLower();
                    }
                    else
                    {
                        Age = (Globals.GetCurServerDateTime().Month + Globals.GetCurServerDateTime().Year * 12) - (DOB.Month + DOB.Year * 12);
                        strUnit = eHCMSResources.G0039_G1_Th.ToLower();
                    }

                    if (Age <= 0)
                    {
                        Age = 1;
                    }
                }

                proAlloc.PatientName = CurrentPatient.FullName;
                proAlloc.DOB = CurrentPatient.DOBText;
                if (Age > 0)
                {
                    proAlloc.Age = Age.ToString() + " " + strUnit;
                }
                else
                {
                    proAlloc.Age = "";
                }
                proAlloc.Gender = CurrentPatient.GenderObj.Name;

                if (CurrentPatientRegistration != null && CurrentPatientRegistration.AdmissionInfo != null
                        && CurrentPatientRegistration.AdmissionInfo.AdmissionDate != null
                        && CurrentPatientRegistration.AdmissionInfo.AdmissionDate.Value.Year > 2010)
                {
                    proAlloc.AdmissionDate = CurrentPatientRegistration.AdmissionInfo.AdmissionDate;
                }

                /*▼====: #001*/
                proAlloc.PatientCode = CurrentPatient.PatientCode;
                /*▲====: #001*/
                //▼====: #005
                proAlloc.PatientFullAddress = CurrentPatient.PatientFullStreetAddress;
                //▲====: #005
                //▼====: #006
                if (CurrentPatient.SuburbName != null && CurrentPatient.CitiesProvince != null)
                {
                    proAlloc.PatientShortAddress = (CurrentPatient.PatientStreetAddress ?? "") + ", " + CurrentPatient.SuburbName.SuburbName + ", " + CurrentPatient.CitiesProvince.CityProvinceName;
                }
                //▲====: #006
                proAlloc.eItem = ReportName.PATIENT_INFO;
            };
            GlobalsNAV.ShowDialog<ICommonPreviewView>(onInitDlg);
        }

        //▼===== #004
        IScanImageCapture theScanImageCaptureDlg = null;
        public void DoScanCmd()
        {
            if (CurrentPatient == null
                || CurrentPatient.PatientID == 0
                || string.IsNullOrEmpty(CurrentPatient.PatientCode))
            {
                return;
            }
            theScanImageCaptureDlg = Globals.GetViewModel<IScanImageCapture>();
            Action<IScanImageCapture> onInitDlg = delegate (IScanImageCapture vm)
            {
                vm.PatientID = (CurrentPatient != null ? CurrentPatient.PatientID : 0);
                vm.PatientCode = (CurrentPatient != null ? CurrentPatient.PatientCode : "");
                vm.PtRegistrationID = 0;
            };
            GlobalsNAV.ShowDialog_V3(theScanImageCaptureDlg, onInitDlg, null);
        }
        //▲===== #004
        private PatientRegistration _CurrentPatientRegistration;
        public PatientRegistration CurrentPatientRegistration
        {
            get
            {
                return _CurrentPatientRegistration;
            }
            set
            {
                if (_CurrentPatientRegistration == value)
                {
                    return;
                }
                _CurrentPatientRegistration = value;
                NotifyOfPropertyChange(() => CurrentPatientRegistration);
            }
        }
    }
}
