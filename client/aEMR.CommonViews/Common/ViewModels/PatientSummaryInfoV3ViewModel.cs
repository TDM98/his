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
using System.Threading;
using aEMR.ServiceClient;
using System.Windows.Controls;
using System.Linq;
using aEMR.Controls;
using aEMR.Common.BaseModel;
using aEMR.Common.Collections;
/*
* 20170627 #002 CMN:   Added event for ConsultingDiagnosys
* 20181212 #003 TTM:   BM 0005413: Thêm trường BasicDiagTreatment, cho phép gõ tìm ICD để lưu và cập nhật Chẩn đoán ban đầu
* 20190107 #004 TTM:   BM 0006472: Tự động clear trường BasicDiagTreatment khi tìm bệnh nhân mới
* 20190831 #005 TTM:   BM 0013214: Thêm chức năng Kiểm tra Ngày thuốc
* 20190926 #006 TTM:   BM 0014354: Chuyển chức năng bổ sung ICD và bác sĩ chỉ định từ eHCMS => aEMR (Bổ sung thêm phần lưu ICD).
* 20191007 #007 TTM:   BM 0000000: BT #998: Sửa chữa chẩn đoán sơ bộ.
* 20200420 #008 TNHX:  Thêm địa chỉ full cho in thông tin bệnh nhân
* 20200923 #009 TNHX:  Thêm địa chỉ short cho in thông tin bệnh nhân
* 20210717 #010 TNHX:  Kiểm tra STT của BN
* 20211004 #011 TNHX: Lọc danh sách bsi theo cấu hình trách nhiệm + theo tích tạm khóa
*/
namespace aEMR.Common.ViewModels
{
    [Export(typeof(IPatientSummaryInfoV3)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class PatientSummaryInfoV3ViewModel : ViewModelBase, IPatientSummaryInfoV3
        , IHandle<ItemSelected<Patient>>
        , IHandle<ItemPatient<Patient>>
        , IHandle<PatientReloadEvent>
        , IHandle<UpdateCompleted<Patient>>
        , IHandle<SavePatientDetailsAndHI_OKEvent>
        , IHandle<ConfirmHiBenefitEvent>
        , IHandle<SelectPresenter>
        , IHandle<HiCardConfirmCompleted>
    {
        [ImportingConstructor]
        public PatientSummaryInfoV3ViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg, ISalePosCaching salePosCaching)
        {
            eventArg.Subscribe(this);
            LoadFamilyRelationshipList();
            LoadKVCode();
            authorization();
            refIDC10Code = new ObservableCollection<DiseasesReference>();
            LoadDoctorStaffCollection();
            if (Globals.ServerConfigSection.OutRegisElements.IsPerformingTMVFunctionsA)
            {
                LoadStaffCollection();
            }
            MedicalExaminationType = Globals.AllLookupValueList.Where(x => x.ObjectTypeID == (long)LookupValues.V_MedicalExaminationType).ToObservableCollection();
            ObjectMedicalExaminationList = Globals.AllLookupValueList.Where(x => x.ObjectTypeID == (long)LookupValues.V_ObjectMedicalExamination).ToObservableCollection();
            V_ReasonHospitalStayList = Globals.AllLookupValueList.Where(x => x.ObjectTypeID == (long)LookupValues.V_ReasonHospitalStay).ToObservableCollection();
        }
        protected override void OnActivate()
        {
            base.OnActivate();
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                Coroutine.BeginExecute(DoLoadDataForTheFirstTime());
            }
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
        public void SetPatientHISumInfo(PatientHI_SummaryInfo PtHISumInfo)
        {
            //ClearAuC();
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
                NotifyOfPropertyChange(() => HiBenefitString);
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
            NotifyOfPropertyChange(() => HiBenefitString);
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

                //strBenefitPercent += eHCMSResources.Z0028_G1_DauNgoacTrai + (IsCrossRegion ? eHCMSResources.G1687_G1_TraiTuyen.ToLower() : eHCMSResources.K3925_G1_DungTuyen.ToLower());
                //if (!string.IsNullOrEmpty(HiComment))
                //{
                //    strBenefitPercent += " - " + HiComment;
                //}

                //strBenefitPercent += eHCMSResources.Z0027_G1_DauNgoacPhai;
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
                        IsDiag = true;
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
                        CurrentPatient.GeneralInfoString += string.Format(" - {0}: ", eHCMSResources.K1209_G1_BH.ToUpper()) + ConfirmedHiItem.HICardNo;
                        NotifyOfPropertyChange(() => CurrentPatient.GeneralInfoString);
                    }
                }
                SetKVCode();
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

        private ObservableCollection<Lookup> _familyRelationshipList;
        public ObservableCollection<Lookup> FamilyRelationshipList
        {
            get { return _familyRelationshipList; }
            set
            {
                _familyRelationshipList = value;
                NotifyOfPropertyChange(() => FamilyRelationshipList);
            }
        }

        private ObservableCollection<string> _AllKVCode;
        public ObservableCollection<string> AllKVCode
        {
            get
            {
                return _AllKVCode;
            }
            set
            {
                _AllKVCode = value;
                NotifyOfPropertyChange(() => AllKVCode);
            }
        }

        private string _SelectedKV;
        public string SelectedKV
        {
            get
            {
                return _SelectedKV;
            }
            set
            {
                _SelectedKV = value;
                NotifyOfPropertyChange(() => SelectedKV);
            }
        }
        public void SetKVCode()
        {
            if (ConfirmedHiItem == null)
            {
                SelectedKV = null;
                return;
            }
            switch (ConfirmedHiItem.KVCode)
            {
                case (long)AllLookupValues.KVCode.KV1:
                    {
                        SelectedKV = AllKVCode[AllKVCode.IndexOf("KV1")];
                        break;
                    }
                case (long)AllLookupValues.KVCode.KV2:
                    {
                        SelectedKV = AllKVCode[AllKVCode.IndexOf("KV2")];
                        break;
                    }
                case (long)AllLookupValues.KVCode.KV3:
                    {
                        SelectedKV = AllKVCode[AllKVCode.IndexOf("KV3")];
                        break;
                    }
                default:
                    {
                        SelectedKV = null;
                        break;
                    }
            }
        }
        public void GetKVCode()
        {
            switch (SelectedKV)
            {
                case "KV1":
                    {
                        ConfirmedHiItem.KVCode = (long)AllLookupValues.KVCode.KV1;
                        break;
                    }
                case "KV2":
                    {
                        ConfirmedHiItem.KVCode = (long)AllLookupValues.KVCode.KV2;
                        break;
                    }
                case "KV3":
                    {
                        ConfirmedHiItem.KVCode = (long)AllLookupValues.KVCode.KV3;
                        break;
                    }
                default:
                    {
                        ConfirmedHiItem.KVCode = 0;
                        break;
                    }
            }

        }

        public void LoadKVCode()
        {
            AllKVCode = new ObservableCollection<string> { "---", "KV1", "KV2", "KV3" };
        }

        public void LoadFamilyRelationshipList()
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new CommonService_V2Client())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        try
                        {
                            contract.BeginGetAllLookupValuesByType(LookupValues.FAMILY_RELATIONSHIP,
                                    Globals.DispatchCallback(asyncResult =>
                                    {
                                        try
                                        {
                                            var allItems = contract.EndGetAllLookupValuesByType(asyncResult);

                                            FamilyRelationshipList = allItems != null ? new ObservableCollection<Lookup>(allItems) : null;
                                        }
                                        catch (Exception ex1)
                                        {
                                            ClientLoggerHelper.LogInfo(ex1.ToString());
                                        }
                                        finally
                                        {
                                            this.HideBusyIndicator();
                                        }
                                    }), null);
                        }
                        catch (Exception exc)
                        {
                            Globals.ShowMessage(exc.Message, eHCMSResources.T0432_G1_Error);
                            this.HideBusyIndicator();
                        }
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

            Coroutine.BeginExecute(OpenPatientDetailDialog());

            //Coroutine.BeginExecute(OpenPatientDetailAndHIDialog());

        }

        private IEnumerator<IResult> OpenPatientDetailDialog()
        {
            var patientDetailsVm = Globals.GetViewModel<IPatientDetails>();
            patientDetailsVm.CurrentAction = eHCMSResources.Z0971_G1_CNhatTTinBN;

            patientDetailsVm.FormState = FormState.EDIT;
            patientDetailsVm.CloseWhenFinish = true;
            patientDetailsVm.IsChildWindow = true;

            yield return new GenericCoRoutineTask(patientDetailsVm.InitLoadControlData_FromExt);

            yield return new GenericCoRoutineTask(patientDetailsVm.LoadPatientDetailsAndHI_GenAction, CurrentPatient, false);

            patientDetailsVm.ActiveTab = PatientInfoTabs.GENERAL_INFO;
            //patientDetailsVm.ActivationMode = CanConfirmHi ? ActivationMode.EDIT_PATIENT_FOR_REGISTRATION : ActivationMode.EDIT_PATIENT_GENERAL_INFO;
            //patientDetailsVm.ActivationMode = CanConfirmHi ? ActivationMode.EDIT_PATIENT_FOR_REGISTRATION : ActivationMode.PATIENT_GENERAL_HI_VIEW;
            patientDetailsVm.ActivationMode = ActivationMode.PATIENT_GENERAL_HI_VIEW;
            GlobalsNAV.ShowDialog_V3<IPatientDetails>(patientDetailsVm);

            yield break;

        }

        private IEnumerator<IResult> OpenPatientDetailAndHIDialog()
        {
            var patientDetailsVm = Globals.GetViewModel<IPatientDetailsAndHI>();
            patientDetailsVm.CurrentAction = eHCMSResources.Z0971_G1_CNhatTTinBN;

            patientDetailsVm.FormState = FormState.EDIT;
            patientDetailsVm.CloseWhenFinish = true;
            patientDetailsVm.IsChildWindow = true;

            yield return new GenericCoRoutineTask(patientDetailsVm.LoadPatientDetailsAndHI_GenAction, CurrentPatient, false);

            //patientDetailsVm.ActiveTab = PatientInfoTabs.GENERAL_INFO;
            //patientDetailsVm.ActivationMode = CanConfirmHi ? ActivationMode.EDIT_PATIENT_FOR_REGISTRATION : ActivationMode.EDIT_PATIENT_GENERAL_INFO;
            //patientDetailsVm.ActivationMode = CanConfirmHi ? ActivationMode.EDIT_PATIENT_FOR_REGISTRATION : ActivationMode.PATIENT_GENERAL_HI_VIEW;
            patientDetailsVm.ActivationMode = ActivationMode.PATIENT_GENERAL_HI_VIEW;
            GlobalsNAV.ShowDialog_V3<IPatientDetailsAndHI>(patientDetailsVm);

            yield break;

        }

        public void ConfirmHiCmd()
        {
            if (CurrentPatient == null)
            {
                return;
            }
            //▼====: #010
            if (GlobalsNAV.IsQMSEnable())
            {
                OrderDTO order = GlobalsNAV.GetOrderByDeptIdAndPatientCodeAndExcludeOrder(
                    Globals.DeptLocation.DeptID, CurrentPatient.PatientCode, OrderDTO.DONE_STATUS);
                if (order == null)
                {
                    Globals.ShowMessage("Bệnh nhân BH cần bắt số thứ tự!", eHCMSResources.T0432_G1_Error);
                    return;
                }
                if (order != null && CurrentPatient.OrderNumber > 0 &&  order.orderNumber != CurrentPatient.OrderNumber)
                {
                    Globals.ShowMessage("Số thứ tự không đúng với thông tin bệnh nhân. Vui lòng kiểm tra lại!", eHCMSResources.T0432_G1_Error);
                    return;
                }
            }
            //▲====: #010
            OpenHI_ConfirmDialog();
        }

        private void OpenHI_ConfirmDialog()
        {
            if (!Enable_ReConfirmHI_InPatientOnly)
            {
                return;
            }
            if (ConfirmedHiItem != null)
            {
                if (MessageBox.Show("Xac nhan lai Quyen loi the BHYT", "", MessageBoxButton.OKCancel) != MessageBoxResult.OK)
                {
                    return;
                }
            }

            //▼===== 20191113 TTM:  Chuyển việc gọi xác nhận ở màn hình đăng ký từ ShowDialog => ShowDialog_V3 để tận dụng biến IsDiaglogView của ShowDialog_V3. Và việc chuyển đổi 2 cách gọi này hoàn toàn không
            //                      ảnh hưởng đến code.
            //Action<IPatientDetails> onInitDlg = delegate (IPatientDetails patientDetailsVm)
            //{
            //    patientDetailsVm.IsChildWindow = true;
            //    patientDetailsVm.Enable_ReConfirmHI_InPatientOnly = true;
            //    patientDetailsVm.ActivationMode = ActivationMode.NEW_PATIENT_FOR_NEW_REGISTRATION;
            //    patientDetailsVm.CurrentAction = eHCMSResources.Z0971_G1_CNhatTTinBN;
            //    patientDetailsVm.FormState = FormState.EDIT;
            //    patientDetailsVm.CloseWhenFinish = true;
            //    patientDetailsVm.LoadPatientDetailsAndHI_V2(CurrentPatient, false);
            //    patientDetailsVm.ActiveTab = PatientInfoTabs.HEALTH_INSURANCE_INFO;
            //    patientDetailsVm.RegistrationType = AllLookupValues.RegistrationType.NGOAI_TRU;
            //    patientDetailsVm.CurrentlyUsed_ToConfirm_HI_Benefit = true;
            //    patientDetailsVm.ShowEmergInPtReExamination = true;
            //    patientDetailsVm.ShowExtraConfirmHI_Fields = true;
            //};
            //GlobalsNAV.ShowDialog<IPatientDetails>(onInitDlg);

            var patientDetailsVm = Globals.GetViewModel<IPatientDetails>();
            patientDetailsVm.IsChildWindow = true;
            patientDetailsVm.Enable_ReConfirmHI_InPatientOnly = true;
            patientDetailsVm.ActivationMode = ActivationMode.NEW_PATIENT_FOR_NEW_REGISTRATION;
            patientDetailsVm.CurrentAction = eHCMSResources.Z0971_G1_CNhatTTinBN;
            patientDetailsVm.FormState = FormState.EDIT;
            patientDetailsVm.CloseWhenFinish = true;
            patientDetailsVm.LoadPatientDetailsAndHI_V2(CurrentPatient, false);
            patientDetailsVm.ActiveTab = PatientInfoTabs.HEALTH_INSURANCE_INFO;
            patientDetailsVm.RegistrationType = AllLookupValues.RegistrationType.NGOAI_TRU;
            patientDetailsVm.CurrentlyUsed_ToConfirm_HI_Benefit = true;
            patientDetailsVm.ShowEmergInPtReExamination = true;
            patientDetailsVm.ShowExtraConfirmHI_Fields = true;
            patientDetailsVm.ShowSaveHIAndConfirmCmd = true;
            if(curRegistration != null)
            {
                patientDetailsVm.IsHICard_FiveYearsCont = curRegistration.IsHICard_FiveYearsCont;
                patientDetailsVm.FiveYearsAppliedDate = curRegistration.FiveYearsAppliedDate;
                patientDetailsVm.FiveYearsARowDate = curRegistration.FiveYearsARowDate;
                patientDetailsVm.V_ReceiveMethod = curRegistration.PtRegistrationID == 0 && curRegistration.Patient.LatestRegistration != null
                    ? curRegistration.Patient.LatestRegistration.V_ReceiveMethod : curRegistration.V_ReceiveMethod;

            }
            patientDetailsVm.LoginHIAPI();
            GlobalsNAV.ShowDialog_V3(patientDetailsVm);
            //▲===== 
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
                //20200225 TBL: Khi tìm bệnh nhân mới thì clear người và nhân viên giới thiệu
                RefByStaff = new Staff();
                RefByPatient = new Patient();
                PresenterName = "";
            }
        }

        public void Handle(PatientReloadEvent message)
        {
            if (message != null)
            {
                CurrentPatient = message.curPatient;
            }
        }

        public void XemPhongCmd()
        {
            GlobalsNAV.ShowDialog<IConsultationRoom>(null, null, false, true, Globals.GetDefaultDialogViewSize());
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

        public void Handle(SavePatientDetailsAndHI_OKEvent message)
        {
            if (message != null)
            {
                CurrentPatient = message.theSavedPatient;
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
            var view = GetView() as IPatientSummaryInfoV3View;
            if (view != null)
            {
                view.Switch(true);
            }
        }
        public void CollapseCmd()
        {
            var view = GetView() as IPatientSummaryInfoV3View;
            if (view != null)
            {
                view.Switch(false);
            }
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
        private RegistrationViewCase _ViewCase = RegistrationViewCase.RegistrationView;
        public RegistrationViewCase ViewCase
        {
            get
            {
                return _ViewCase;
            }
            set
            {
                if (_ViewCase == value)
                {
                    return;
                }
                _ViewCase = value;
                NotifyOfPropertyChange(() => ViewCase);
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

            if (ViewCase == RegistrationViewCase.RegistrationRequestView && Registration_DataStorage.CurrentPatientRegistration != null)
            {
                ApplySpecialPatientClass(Registration_DataStorage.CurrentPatientRegistration.PatientClassification);
            }

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

                if (Registration_DataStorage != null && Registration_DataStorage.CurrentPatientRegistration != null && Registration_DataStorage.CurrentPatientRegistration.AdmissionInfo != null
                        && Registration_DataStorage.CurrentPatientRegistration.AdmissionInfo.AdmissionDate != null
                        && Registration_DataStorage.CurrentPatientRegistration.AdmissionInfo.AdmissionDate.Value.Year > 2010)
                {
                    proAlloc.AdmissionDate = Registration_DataStorage.CurrentPatientRegistration.AdmissionInfo.AdmissionDate;
                }

                /*▼====: #001*/
                proAlloc.PatientCode = CurrentPatient.PatientCode;
                /*▲====: #001*/
                //▼====: #008
                proAlloc.PatientFullAddress = CurrentPatient.PatientFullStreetAddress;
                //▲====: #008
                //▼====: #009
                if (CurrentPatient.SuburbName != null && CurrentPatient.CitiesProvince != null)
                {
                    proAlloc.PatientShortAddress = (CurrentPatient.PatientStreetAddress ?? "") + ", " + CurrentPatient.SuburbName.SuburbName + ", " + CurrentPatient.CitiesProvince.CityProvinceName;
                }
                //▲====: #009
                proAlloc.eItem = ReportName.PATIENT_INFO;
            };
            GlobalsNAV.ShowDialog<ICommonPreviewView>(onInitDlg);
        }

        public void Handle(ConfirmHiBenefitEvent message)
        {
            if (message != null)
            {
                if (ConfirmedHiItem == null || ConfirmedHiItem.HIID != message.HiId)
                {
                    return;
                }
                ConfirmedHiItem.isDoing = true;
                ConfirmedHiItem.EditLocked = true;
            }
        }

        //▼====== #003
        private ObservableCollection<DiseasesReference> _refIDC10Code;
        public ObservableCollection<DiseasesReference> refIDC10Code
        {
            get
            {
                return _refIDC10Code;
            }
            set
            {
                if (_refIDC10Code != value)
                {
                    _refIDC10Code = value;
                }
                NotifyOfPropertyChange(() => refIDC10Code);
            }
        }
        private bool _IsDiag = false;
        public bool IsDiag
        {
            get
            {
                return _IsDiag;
            }
            set
            {
                if (_IsDiag != value)
                {
                    _IsDiag = value;
                }
                NotifyOfPropertyChange(() => IsDiag);
            }
        }
        private PatientRegistration _curRegistration;
        public PatientRegistration curRegistration
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
                }
                //▼===== #006: Load thông tin theo đăng ký.
                if (_curRegistration != null)
                {
                    //▼===== #007: Tìm bệnh nhân lên sẽ set Enable về false => khi nào thoả điều kiện đã lưu 1 dịch vụ bất kỳ => mở lên cho đưa chẩn đoán sơ bộ vào. Giống như eHCMs.
                    IsEnableForBasicDiag = false;
                    if (_curRegistration.AllSaveRegistrationDetails != null && _curRegistration.AllSavePCLRequestDetails != null)
                    {
                        if (_curRegistration.AllSaveRegistrationDetails.Count > 0 || _curRegistration.AllSavePCLRequestDetails.Count > 0)
                        {
                            IsEnableForBasicDiag = true;
                        }
                    }
                    //▲===== #007
                    if (_curRegistration.AdmissionICD10 != null)
                    {
                        aICD10Code = _curRegistration.AdmissionICD10;
                    }
                    else
                    {
                        aICD10Code = new DiseasesReference();
                    }
                    gSelectedDoctorStaff = _curRegistration.OutHosDiagStaff;
                    if(_curRegistration.V_MedicalExaminationType != null)
                    {
                        CurrentMedicalExaminationType = MedicalExaminationType.Where(x => x.LookupID == _curRegistration.V_MedicalExaminationType.LookupID).FirstOrDefault();
                    }
                    else if(_curRegistration.V_MedicalExaminationType == null)
                    {
                        CurrentMedicalExaminationType = null;
                    }
                    if (_curRegistration.V_ObjectMedicalExamination != null)
                    {
                        CurrentObjectMedicalExamination = ObjectMedicalExaminationList.Where(x => x.LookupID == _curRegistration.V_ObjectMedicalExamination.LookupID).FirstOrDefault();
                    }
                    else if (_curRegistration.V_ObjectMedicalExamination == null)
                    {
                        CurrentObjectMedicalExamination = ObjectMedicalExaminationList.Where(x => x.LookupID == (long)AllLookupValues.V_ObjectMedicalExamination.Kham_Chua_Benh_Dich_Vu_9).FirstOrDefault();
                    }
                    if (_curRegistration.V_ReasonHospitalStay != null)
                    {
                        CurrentV_ReasonHospitalStay = V_ReasonHospitalStayList.Where(x => x.LookupID == _curRegistration.V_ReasonHospitalStay.LookupID).FirstOrDefault();
                    }
                    else if(_curRegistration.V_ReasonHospitalStay == null)
                    {
                        CurrentV_ReasonHospitalStay = null;
                    }
                }
                //▲===== #006
                NotifyOfPropertyChange(() => curRegistration);
                NotifyOfPropertyChange(() => aICD10Code);
            }
        }
        private string _BasicDiagTreatment;
        public string BasicDiagTreatment
        {
            get
            {
                return _BasicDiagTreatment;
            }
            set
            {
                if (_BasicDiagTreatment != value)
                {
                    _BasicDiagTreatment = value;
                    if (CurrentPatient != null)
                    {
                        Globals.EventAggregator.Publish(new SetBasicDiagTreatmentForRegistrationSummaryV2 { });
                    }
                }
                NotifyOfPropertyChange(() => BasicDiagTreatment);
            }
        }
        AutoCompleteBox Acb_ICD10_Code = null;
        public void AcbICD10Code_Loaded(object sender, RoutedEventArgs e)
        {
            Acb_ICD10_Code = (AutoCompleteBox)sender;
        }
        public void aucICD10_Populating(object sender, PopulatingEventArgs e)
        {
            if(curRegistration == null)
            {
                return;
            }
            if (!string.IsNullOrEmpty(e.Parameter))
            {
                LoadRefDiseases(e.Parameter, 0, 0, 100);
            }
        }

        public void LoadRefDiseases(string name, byte type, int PageIndex, int PageSize)
        {
            var t = new Thread(() =>
            {
                using (var serviceFactory = new CommonUtilsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginSearchRefDiseases(name, PageIndex, PageSize, type
                        , curRegistration.PatientID == null ? 0 : (long)curRegistration.PatientID
                        , Globals.GetCurServerDateTime()
                        , Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            int Total = 10;
                            var results = contract.EndSearchRefDiseases(out Total, asyncResult);
                            refIDC10Code.Clear();
                            if (results != null)
                            {
                                foreach (DiseasesReference p in results)
                                {
                                    refIDC10Code.Add(p);
                                }
                            }
                            Acb_ICD10_Code.ItemsSource = refIDC10Code;
                            Acb_ICD10_Code.PopulateComplete();
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {

                        }

                    }), null);

                }

            });

            t.Start();
        }

        private bool isDropDown = false;
        public void AxAutoComplete_DropDownClosing(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            isDropDown = true;
        }
        public void AxAutoComplete_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            if (!isDropDown)
            {
                return;
            }
            isDropDown = false;
            if (sender != null)
            {
                if (curRegistration == null)
                {
                    curRegistration = new PatientRegistration();
                }
                aICD10Code = (sender as AxAutoComplete).SelectedItem as DiseasesReference;
                Globals.EventAggregator.Publish(new BasicDiagTreatmentChanged { aICD10Code = aICD10Code });
            }
        }
        public void UpdateBasicDiag()
        {
            if (CurrentPatient == null || curRegistration == null)
            {
                MessageBox.Show(eHCMSResources.Z0838_G1_DLieuKgHopLe);
                return;
            }
            if (aICD10Code == null)
            {
                MessageBox.Show(eHCMSResources.Z2299_G1_KhongCoGiDeLuu);
                return;
            }
            else
            {
                UpdateBasicDiagTreatment();
            }
        }

        public void UpdateBasicDiagTreatment()
        {
            this.ShowBusyIndicator(eHCMSResources.Z0172_G1_DangLuuDLieu);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginUpdateBasicDiagTreatment(curRegistration, aICD10Code, gSelectedDoctorStaff,
                            Globals.DispatchCallback(asyncResult =>
                            {
                                bool bOK;
                                try
                                {
                                    bOK = contract.EndUpdateBasicDiagTreatment(asyncResult);
                                    if (bOK)
                                    {
                                        MessageBox.Show(eHCMSResources.K2782_G1_DaCNhat);
                                    }
                                    else
                                    {
                                        MessageBox.Show(eHCMSResources.Z0656_G1_GhiDLieuKgThCong);
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
                }
            });
            t.Start();
        }
        //▲====== #003
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
                if (CurrentPatient != null)
                {
                    Globals.EventAggregator.Publish(new SetBasicDiagTreatmentForRegistrationSummaryV2 { });
                }
                NotifyOfPropertyChange(() => gSelectedDoctorStaff);
            }
        }
        AxAutoComplete AcbDoctorStaff { get; set; }
        private void LoadDoctorStaffCollection()
        {
            //▼====: #011
            DoctorStaffs = new ObservableCollection<Staff>(Globals.AllStaffs.Where(x => x.RefStaffCategory != null 
                                                                                    && x.RefStaffCategory.V_StaffCatType == Globals.ServerConfigSection.CommonItems.StaffCatTypeBAC_SI
                                                                                    && (!x.IsStopUsing)).ToList());
            //▲====: #011
        }
        public void DoctorStaff_Loaded(object sender, RoutedEventArgs e)
        {
            AcbDoctorStaff = (AxAutoComplete)sender;
        }
        public void DoctorStaff_Populating(object sender, PopulatingEventArgs e)
        {
            AutoCompleteBox cboContext = sender as AutoCompleteBox;
            e.Cancel = true;
            var AllItemsContext = new ObservableCollection<Staff>(DoctorStaffs.Where(x => Globals.RemoveVietnameseString(x.FullName).ToLower().Contains(Globals.RemoveVietnameseString(cboContext.SearchText).ToLower())));
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

        private bool IsPassChecked = false;
        public void ApplySpecialPatientClass(PatientClassification aPatientClass)
        {
            if (PatientClassifications == null)
            {
                return;
            }
            IsPassChecked = true;
            CurrentPatientClassification = PatientClassifications.FirstOrDefault(x => x.PatientClassID == aPatientClass.PatientClassID);
        }
        public void cboPatientClasses_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (IsPassChecked)
            {
                IsPassChecked = false;
                return;
            }
            if (CurrentPatientClassification != null && CurrentPatientClassification.PatientClassID == (long)ePatientClassification.CompanyHealthRecord)
            {
                CurrentPatientClassification = PatientClassifications.FirstOrDefault();
            }
        }
        //▼===== #005
        #region Kiểm tra ngày thuốc
        public void CheckOldConsultationPatientCmd()
        {
            if (CurrentPatient == null)
            {
                return;
            }
            CheckOldConsultationPatient(CurrentPatient);
        }
        public void CheckOldConsultationPatient(Patient CurrentPatient)
        {
            this.ShowBusyIndicator(eHCMSResources.K2871_G1_DangLoadDLieu);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginCheckOldConsultationPatient(CurrentPatient.PatientID,
                            Globals.DispatchCallback(asyncResult =>
                            {
                                Prescription PreInfo = null;
                                try
                                {
                                    PreInfo = contract.EndCheckOldConsultationPatient(asyncResult);
                                    if (PreInfo != null)
                                    {
                                        if (PreInfo.NDay == null)
                                        {
                                            PreInfo.NDay = 0;
                                        }
                                        DateTime DateNow = DateTime.Now;
                                        DateTime DateCreate = PreInfo.RecDateCreated.AddDays((long)PreInfo.NDay);
                                        if (eHCMS.Services.Core.AxHelper.CompareDate(DateNow, DateCreate) == 2)
                                        {
                                            TimeSpan DateCompare = DateCreate.Subtract(DateNow);
                                            int SoNgayThuocConLai = (int)DateCompare.TotalDays;
                                            //MessageBox.Show(string.Format(eHCMSResources.Z2807_G1_ThBaoLoiKiemTraLSKCB, PreInfo.MedNameUseOnlyForCheckConsultation, PreInfo.RecDateCreated, PreInfo.NDay, SoNgayThuocConLai + 1));
                                            Coroutine.BeginExecute(ShowError(string.Format(eHCMSResources.Z2807_G1_ThBaoLoiKiemTraLSKCB, PreInfo.MedNameUseOnlyForCheckConsultation, PreInfo.RecDateCreated, PreInfo.NDay, SoNgayThuocConLai + 1)));
                                        }
                                        else
                                        {
                                            MessageBox.Show(eHCMSResources.Z2808_G1_BenhNhanDKBinhThuong);
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    ClientLoggerHelper.LogInfo(ex.ToString());
                                    this.HideBusyIndicator();
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
                finally
                {
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }
        #endregion
        //▲===== #005
        //▼===== #006
        private IEnumerator<IResult> ShowError(string strMessage)
        {
            var dialog = new WarningWithConfirmMsgBoxTask(strMessage, eHCMSResources.G2363_G1_XNhan, true);
            yield return dialog;
            yield break;
        }
        private DiseasesReference _aICD10Code;
        public DiseasesReference aICD10Code
        {
            get
            {
                return _aICD10Code;
            }
            set
            {
                if (_aICD10Code != value)
                {
                    _aICD10Code = value;
                }
                NotifyOfPropertyChange(() => aICD10Code);
            }
        }
        private Staff _OutHosDiagStaffFullName;
        public Staff OutHosDiagStaffFullName
        {
            get
            {
                return _OutHosDiagStaffFullName;
            }
            set
            {
                if (_OutHosDiagStaffFullName != value)
                {
                    _OutHosDiagStaffFullName = value;
                }
                NotifyOfPropertyChange(() => OutHosDiagStaffFullName);
            }
        }
        //▲===== #006
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

        //▼===== #007: Chuyển nhập tên chẩn đoán sơ bộ từ text box (chỉ để hiển thị) thành AutoComplete.
        AutoCompleteBox Acb_ICD10_Name = null;
        public void AcbICD10Name_Loaded(object sender, RoutedEventArgs e)
        {
            Acb_ICD10_Name = (AutoCompleteBox)sender;
        }
        public void aucICD10Name_Populating(object sender, PopulatingEventArgs e)
        {
            if (curRegistration == null)
            {
                return;
            }
            if (!string.IsNullOrEmpty(e.Parameter))
            {
                LoadRefDiseasesName(e.Parameter, 1, 0, 100);
            }
        }

        public void LoadRefDiseasesName(string name, byte type, int PageIndex, int PageSize)
        {
            var t = new Thread(() =>
            {
                using (var serviceFactory = new CommonUtilsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginSearchRefDiseases(name, PageIndex, PageSize, type
                        , curRegistration.PatientID == null ? 0 : (long)curRegistration.PatientID
                        , Globals.GetCurServerDateTime()
                        , Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            int Total = 10;
                            var results = contract.EndSearchRefDiseases(out Total, asyncResult);
                            refIDC10Code.Clear();
                            if (results != null)
                            {
                                foreach (DiseasesReference p in results)
                                {
                                    refIDC10Code.Add(p);
                                }
                            }
                            Acb_ICD10_Name.ItemsSource = refIDC10Code;
                            Acb_ICD10_Name.PopulateComplete();
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                        }
                    }), null);
                }
            });
            t.Start();
        }

        private bool isDropDownName = false;
        public void aucICD10Name_DropDownClosing(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            isDropDownName = true;
        }
        public void aucICD10Name_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            if (!isDropDownName)
            {
                return;
            }
            isDropDownName = false;
            if (sender != null)
            {
                if (curRegistration == null)
                {
                    curRegistration = new PatientRegistration();
                }
                aICD10Code = (sender as AxAutoComplete).SelectedItem as DiseasesReference;
                Globals.EventAggregator.Publish(new BasicDiagTreatmentChanged { aICD10Code = aICD10Code });
            }
        }
        private bool _IsEnableForBasicDiag = false;
        public bool IsEnableForBasicDiag
        {
            get
            {
                return _IsEnableForBasicDiag;
            }
            set
            {
                _IsEnableForBasicDiag = value;
                NotifyOfPropertyChange(() => IsEnableForBasicDiag);
            }
        }
        //▲===== #007
        #region Người và nhân viên giới thiệu
        private bool _IsVisibility = Globals.ServerConfigSection.OutRegisElements.IsPerformingTMVFunctionsA;
        public bool IsVisibility
        {
            get { return _IsVisibility; }
            set
            {
                if (_IsVisibility != value)
                {
                    _IsVisibility = value;
                    NotifyOfPropertyChange(() => IsVisibility);
                }
            }
        }

        private Patient _RefByPatient;
        public Patient RefByPatient
        {
            get
            {
                return _RefByPatient;
            }
            set
            {
                if (_RefByPatient != value)
                {
                    _RefByPatient = value;
                    NotifyOfPropertyChange(() => RefByPatient);
                    if (RefByPatient != null && RefByPatient.PatientID > 0)
                    {
                        PresenterName = RefByPatient.FullName + " (" + RefByPatient.PatientCode + ") ";
                    }
                    else
                    {
                        PresenterName = "";
                    }
                }
            }
        }

        private Staff _RefByStaff;
        public Staff RefByStaff
        {
            get
            {
                return _RefByStaff;
            }
            set
            {
                if (_RefByStaff != value)
                {
                    _RefByStaff = value;
                    NotifyOfPropertyChange(() => RefByStaff);
                    Globals.EventAggregator.Publish(new SetRefByStaffForRegistrationSummaryV2 { });
                }
            }
        }
        private string _PresenterName;
        public string PresenterName
        {
            get { return _PresenterName; }
            set
            {
                if (_PresenterName != value)
                {
                    _PresenterName = value;
                    NotifyOfPropertyChange(() => PresenterName);
                }
            }
        }
        public void PresenterCmd()
        {
            if (CurrentPatient == null)
            {
                return;
            }
            IFindPatient findPatient = Globals.GetViewModel<IFindPatient>();
            Action<IFindPatient> onInitDlg = delegate (IFindPatient vm)
            {
                vm.IsPresenter = true;
            };
            GlobalsNAV.ShowDialog_V3(findPatient, onInitDlg, null);
        }
        public void Handle(SelectPresenter message)
        {
            if (message != null && message.PatientInfo != null)
            {
                RefByPatient = message.PatientInfo;
                PresenterName = RefByPatient.FullName + " (" + RefByPatient.PatientCode + ") ";
            }
        }
        public void Handle(HiCardConfirmCompleted message)
        {
            if (message != null 
                && message.RegistrationCode != null 
                && message.HICardNo != null
                && message.HIPCode != null
                && CurrentObjectMedicalExamination != null
                && CurrentObjectMedicalExamination.LookupID != (long)AllLookupValues.V_ObjectMedicalExamination.Cap_Cuu_2
                )
            {
                string prefixProvinceCode = Globals.ServerConfigSection.Hospitals.HospitalCode.Substring(0,2);
                //Kiểm tra nơi khám chữa bệnh ban đầu xem là đúng tuyến hay trái tuyến
                if (message.RegistrationCode.Substring(0, 2) == prefixProvinceCode)
                {
                    if (message.RegistrationCode == Globals.ServerConfigSection.Hospitals.HospitalCode)
                    {
                        CurrentObjectMedicalExamination = ObjectMedicalExaminationList
                            .Where(x => x.LookupID == (long)AllLookupValues.V_ObjectMedicalExamination.Dung_Tuyen_1_1).FirstOrDefault();
                    }
                    else if (message.HICardNo.Substring(3, 2) == prefixProvinceCode)
                    {
                        CurrentObjectMedicalExamination = ObjectMedicalExaminationList
                           .Where(x => x.LookupID == (long)AllLookupValues.V_ObjectMedicalExamination.Dung_Tuyen_1_2).FirstOrDefault();
                    }
                }
                else 
                {
                    CurrentObjectMedicalExamination = ObjectMedicalExaminationList
                               .Where(x => x.LookupID == (long)AllLookupValues.V_ObjectMedicalExamination.Trai_Tuyen_3_3).FirstOrDefault();
                    if (message.HIPCode == "DT" || message.HIPCode == "HN" || message.KVCode > 0)
                    {
                        CurrentObjectMedicalExamination = ObjectMedicalExaminationList
                               .Where(x => x.LookupID == (long)AllLookupValues.V_ObjectMedicalExamination.Trai_Tuyen_3_6).FirstOrDefault();
                    }
                }
            }
        }
        private ObservableCollection<Staff> _ObsStaffs;
        public ObservableCollection<Staff> ObsStaffs
        {
            get
            {
                return _ObsStaffs;
            }
            set
            {
                if (_ObsStaffs != value)
                {
                    _ObsStaffs = value;
                    NotifyOfPropertyChange(() => ObsStaffs);
                }
            }
        }
        AxAutoComplete AcbStaff { get; set; }
        private void LoadStaffCollection()
        {
            ObsStaffs = new ObservableCollection<Staff>(Globals.AllStaffs);
        }
        public void Staff_Loaded(object sender, RoutedEventArgs e)
        {
            AcbStaff = (AxAutoComplete)sender;
        }
        public void Staff_Populating(object sender, PopulatingEventArgs e)
        {
            AutoCompleteBox cboContext = sender as AutoCompleteBox;
            e.Cancel = true;
            var AllItemsContext = new ObservableCollection<Staff>(ObsStaffs.Where(x => Globals.RemoveVietnameseString(x.FullName).ToLower().Contains(Globals.RemoveVietnameseString(cboContext.SearchText).ToLower())));
            cboContext.ItemsSource = AllItemsContext;
            cboContext.PopulateComplete();
        }
        public void Staff_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            if (sender != null)
            {
                RefByStaff = ((AutoCompleteBox)sender).SelectedItem as Staff;
            }
        }
        #endregion

        private ObservableCollection<Lookup> _MedicalExaminationType;

        public ObservableCollection<Lookup> MedicalExaminationType
        {
            get { return _MedicalExaminationType; }
            set
            {
                _MedicalExaminationType = value;
                NotifyOfPropertyChange(() => MedicalExaminationType);
            }
        }
        private Lookup _CurrentMedicalExaminationType;

        public Lookup CurrentMedicalExaminationType
        {
            get { return _CurrentMedicalExaminationType; }
            set
            {
                _CurrentMedicalExaminationType = value;
                NotifyOfPropertyChange(() => CurrentMedicalExaminationType);
            }
        }
        public void cboMedicalExaminationType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(curRegistration == null)
            {
                return;
            }
            if(CurrentMedicalExaminationType != null)
            {
                curRegistration.V_MedicalExaminationType = CurrentMedicalExaminationType;
            }
        }
        private ObservableCollection<Lookup> _ObjectMedicalExaminationList;

        public ObservableCollection<Lookup> ObjectMedicalExaminationList
        {
            get { return _ObjectMedicalExaminationList; }
            set
            {
                _ObjectMedicalExaminationList = value;
                NotifyOfPropertyChange(() => ObjectMedicalExaminationList);
            }
        }
        private Lookup _CurrentObjectMedicalExamination;

        public Lookup CurrentObjectMedicalExamination
        {
            get { return _CurrentObjectMedicalExamination; }
            set
            {
                _CurrentObjectMedicalExamination = value;
                NotifyOfPropertyChange(() => CurrentObjectMedicalExamination);
            }
        }
        public void cboObjectMedicalExamination_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(curRegistration == null)
            {
                return;
            }
            if(CurrentObjectMedicalExamination != null)
            {
                curRegistration.V_ObjectMedicalExamination = CurrentObjectMedicalExamination;
            }
        }
        private ObservableCollection<Lookup> _V_ReasonHospitalStayList;

        public ObservableCollection<Lookup> V_ReasonHospitalStayList
        {
            get { return _V_ReasonHospitalStayList; }
            set
            {
                _V_ReasonHospitalStayList = value;
                NotifyOfPropertyChange(() => V_ReasonHospitalStayList);
            }
        }
        private Lookup _CurrentV_ReasonHospitalStay;

        public Lookup CurrentV_ReasonHospitalStay
        {
            get { return _CurrentV_ReasonHospitalStay; }
            set
            {
                _CurrentV_ReasonHospitalStay = value;
                NotifyOfPropertyChange(() => CurrentV_ReasonHospitalStay);
            }
        }
        public void cboReasonHospitalStay_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (curRegistration == null)
            {
                return;
            }
            if (CurrentV_ReasonHospitalStay != null)
            {
                curRegistration.V_ReasonHospitalStay = CurrentV_ReasonHospitalStay;
            }
        }
    }
}
