using aEMR.ServiceClient;
using aEMR.ViewContracts.Configuration;
using Caliburn.Micro;
using System.ComponentModel.Composition;
using aEMR.ViewContracts;
using DataEntities;
using System.Collections.ObjectModel;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using System.Threading;
using System;
using System.Windows;
using aEMR.Common;
using aEMR.Common.Collections;
using eHCMSLanguage;
using aEMR.ServiceClient.Consultation_PCLs;
using Castle.Windsor;
using aEMR.Common.BaseModel;

/*
 * 20180920 #001 TBL: Added IsDiagTrmentChanged
 * 20181023 #002 TTM: BM 0002173: Thay đổi cách lưu, cập nhật và lấy lên của tình trạng thể chất => tất cả đều dựa vào lần đăng ký.
 */
namespace aEMR.ConsultantEPrescription.ViewModels
{
    [Export(typeof(ISummary_V2)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class Summary_V2ViewModel : ViewModelBase, ISummary_V2
        , IHandle<Re_ReadAllergiesEvent>
        , IHandle<Re_ReadWarningEvent>
        , IHandle<RiskFactorSaveCompleteEvent>
        , IHandle<CommonClosedPhysicalForSummaryEvent>
        , IHandle<LoadGeneralInfoPageEvent>
    {
        private bool _isLoading;
        public bool IsLoading
        {
            get
            {
                //return _isLoading; 
                return false;
            }
            set
            {
                if (_isLoading != value)
                {
                    _isLoading = value;
                    NotifyOfPropertyChange(() => IsLoading);
                }
            }
        }
        [ImportingConstructor]
        public Summary_V2ViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg)
        {
            System.Diagnostics.Debug.WriteLine("====> Summary_V2ViewModel - Constructor");
            CreateSubVM();

            Authorization();
            Globals.EventAggregator.Subscribe(this);

            PtSumDiagList = new PagedSortableCollectionView<DiagnosisTreatment>();
            PtSumDiagList.PageSize = 15;
            PtSumDiagList.OnRefresh += PtSumDiagList_OnRefresh;
            curRiskFactors = new RiskFactors();
            
            if (Registration_DataStorage != null && Registration_DataStorage.CurrentPatient != null)
            {
                InitData(Registration_DataStorage.CurrentPatient);
            }
        }

        ~Summary_V2ViewModel()
        {
            System.Diagnostics.Debug.WriteLine("====> Summary_V2ViewModel - Destructor");
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
                UCConsultations.CS_DS = CS_DS;
            }
        }

        private void CreateSubVM()
        {
            UCConsultations = Globals.GetViewModel<IConsultations>();
            UCConsultations.IsShowSummaryContent = false;            
        }
        public void ApplySmallProcedure(SmallProcedure aSmallProcedureObj)
        {
            if (UCConsultations != null)
            {
                UCConsultations.ApplySmallProcedure(aSmallProcedureObj);
            }
        }
        public SmallProcedure UpdatedSmallProcedure
        {
            get
            {
                if (UCConsultations == null)
                {
                    return null;
                }
                return UCConsultations.UpdatedSmallProcedure;
            }
        }
        public SmallProcedure SmallProcedureObj
        {
            get
            {
                if (UCConsultations == null)
                {
                    return null;
                }
                return UCConsultations.SmallProcedureObj;
            }
        }
        private void ActivateSubVM()
        {            
            ActivateItem(UCConsultations);
        }

        protected override void OnActivate()
        {
            base.OnActivate();

            ActivateSubVM();
        }

        //27112018 TTM: Bổ sung Deactive cho Summary_V2.
        private void DeactivateSubVM(bool close)
        {
            DeactivateItem(UCConsultations, close);
        }
        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);
            DeactivateSubVM(close);
            Globals.EventAggregator.Unsubscribe(this);
        }

        private void PtSumDiagList_OnRefresh(object sender, aEMR.Common.Collections.RefreshEventArgs e)
        {
            InitData(PatientInfo);
        }

        #region Properties
        enum AllergyType
        {
            Drug = 0,
            DrugClass = 1,
            Others = 2
        };

        private Patient _PatientInfo;
        public Patient PatientInfo
        {
            get { return _PatientInfo; }
            set
            {
                if (_PatientInfo != value)
                {
                    _PatientInfo = value;
                    NotifyOfPropertyChange(() => PatientInfo);
                }
            }
        }

        private PatientRegistration _curPatientRegistration;
        public PatientRegistration curPatientRegistration
        {
            get
            {
                return _curPatientRegistration;
            }
            set
            {
                if (_curPatientRegistration == value)
                    return;
                _curPatientRegistration = value;
                NotifyOfPropertyChange(() => curPatientRegistration);
            }
        }

        private ObservableCollection<MDAllergy> _ptAllergyList;
        public ObservableCollection<MDAllergy> PtAllergyList
        {
            get
            {
                return _ptAllergyList;
            }
            set
            {
                if (_ptAllergyList != value)
                {
                    _ptAllergyList = value;
                    NotifyOfPropertyChange(() => PtAllergyList);
                }
            }
        }

        private MDWarning _ObjMDWarnings_ByPatientID;
        public MDWarning ObjMDWarnings_ByPatientID
        {
            get
            {
                return _ObjMDWarnings_ByPatientID;
            }
            set
            {
                if (_ObjMDWarnings_ByPatientID != value)
                {
                    _ObjMDWarnings_ByPatientID = value;
                    NotifyOfPropertyChange(() => ObjMDWarnings_ByPatientID);
                }
            }
        }

        private MDAllergy _selectedPtAllergies;
        public MDAllergy SelectedPtAllergies
        {
            get
            {
                return _selectedPtAllergies;
            }
            set
            {
                if (_selectedPtAllergies != value)
                {
                    _selectedPtAllergies = value;
                    NotifyOfPropertyChange(() => SelectedPtAllergies);
                }
            }
        }

        private string _sPtWarningList;
        public string SPtWarningList
        {
            get
            {
                return _sPtWarningList;
            }
            set
            {
                if (_sPtWarningList != value)
                {
                    _sPtWarningList = value;
                    NotifyOfPropertyChange(() => SPtWarningList);
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

        private PagedSortableCollectionView<DiagnosisTreatment> _PtSumDiagList;
        public PagedSortableCollectionView<DiagnosisTreatment> PtSumDiagList
        {
            get
            {
                return _PtSumDiagList;
            }
            set
            {
                if (_PtSumDiagList != value)
                {
                    _PtSumDiagList = value;
                    NotifyOfPropertyChange(() => PtSumDiagList);
                }
            }
        }

        private PatientServiceRecord _selectedPtConsultation;
        public PatientServiceRecord SelectedPtConsultation
        {
            get
            {
                return _selectedPtConsultation;
            }
            set
            {
                if (_selectedPtConsultation != value)
                {
                    _selectedPtConsultation = value;
                    NotifyOfPropertyChange(() => SelectedPtConsultation);
                }
            }
        }

        private PatientServiceRecord _SelectedPtDiagnosic;
        public PatientServiceRecord SelectedPtDiagnosic
        {
            get
            {
                return _SelectedPtDiagnosic;
            }
            set
            {
                if (_SelectedPtDiagnosic != value)
                {
                    _SelectedPtDiagnosic = value;
                    NotifyOfPropertyChange(() => SelectedPtDiagnosic);
                }
            }
        }

        private long _PatientID;
        public long PatientID
        {
            get
            {
                return _PatientID;
            }
            set
            {
                if (_PatientID != value)
                {
                    _PatientID = value;
                    NotifyOfPropertyChange(() => PatientID);
                }
            }
        }

        private bool _canSelectClearChk;
        public bool CanSelectClearChk
        {
            get
            {
                return _canSelectClearChk;
            }
            set
            {
                if (_canSelectClearChk != value)
                {
                    _canSelectClearChk = value;
                    NotifyOfPropertyChange(() => CanSelectClearChk);
                }
            }
        }

        private bool _canSelectUnknowChk;
        public bool CanSelectUnknowChk
        {
            get
            {
                return _canSelectUnknowChk;
            }
            set
            {
                if (_canSelectUnknowChk != value)
                {
                    _canSelectUnknowChk = value;
                    NotifyOfPropertyChange(() => CanSelectUnknowChk);
                }
            }
        }

        private bool _isUnknowChecked;
        public bool IsUnknowChecked
        {
            get
            {
                return _isUnknowChecked;
            }
            set
            {

                _isUnknowChecked = value;
                NotifyOfPropertyChange(() => IsUnknowChecked);
            }
        }

        private MDAllergy _allergy;
        public MDAllergy Allergy
        {
            get
            {
                return _allergy;
            }
            set
            {
                if (_allergy != value)
                {
                    _allergy = value;
                    NotifyOfPropertyChange(() => Allergy);
                }
            }
        }

        private ObservableCollection<RiskFactors> _allRiskFactors;
        public ObservableCollection<RiskFactors> allRiskFactors
        {
            get
            {
                return _allRiskFactors;
            }
            set
            {
                if (_allRiskFactors != value)
                {
                    _allRiskFactors = value;
                    NotifyOfPropertyChange(() => allRiskFactors);
                }
            }
        }

        private RiskFactors _curRiskFactors;
        public RiskFactors curRiskFactors
        {
            get
            {
                return _curRiskFactors;
            }
            set
            {
                if (_curRiskFactors != value)
                {
                    _curRiskFactors = value;
                    NotifyOfPropertyChange(() => curRiskFactors);
                }
            }
        }

        private bool _mThongTinChung_TimBN = true;
        private bool _mThongTinChung_TimDK = true;
        private bool _mThongTinChung_ThongTinBN = true;
        private bool _mThongTinChung_DiUngCanhBao_ThongTin = true;
        private bool _mThongTinChung_DiUngCanhBao_ChinhSua = true && Globals.isConsultationStateEdit;
        private bool _mThongTinChung_TinhTrangTheChat_ThongTin = true;
        private bool _mThongTinChung_TinhTrangTheChat_ChinhSua = true && Globals.isConsultationStateEdit;
        private bool _mThongTinChung_ThongTinLienHe_ThongTin = true;
        private bool _mThongTinChung_ThongTinLienHe_ChinhSua = true;
        private bool _mThongTinChung_BHYT_ThongTin = true;
        private bool _mThongTinChung_XemLanKhamBenh = true;

        public bool mThongTinChung_TimBN
        {
            get
            {
                return _mThongTinChung_TimBN;
            }
            set
            {
                if (_mThongTinChung_TimBN == value)
                    return;
                _mThongTinChung_TimBN = value;
            }
        }
        public bool mThongTinChung_TimDK
        {
            get
            {
                return _mThongTinChung_TimDK;
            }
            set
            {
                if (_mThongTinChung_TimDK == value)
                    return;
                _mThongTinChung_TimDK = value;
            }
        }
        public bool mThongTinChung_ThongTinBN
        {
            get
            {
                return _mThongTinChung_ThongTinBN;
            }
            set
            {
                if (_mThongTinChung_ThongTinBN == value)
                    return;
                _mThongTinChung_ThongTinBN = value;
            }
        }
        public bool mThongTinChung_DiUngCanhBao_ThongTin
        {
            get
            {
                return _mThongTinChung_DiUngCanhBao_ThongTin;
            }
            set
            {
                if (_mThongTinChung_DiUngCanhBao_ThongTin == value)
                    return;
                _mThongTinChung_DiUngCanhBao_ThongTin = value;

            }
        }
        public bool mThongTinChung_DiUngCanhBao_ChinhSua
        {
            get
            {
                return _mThongTinChung_DiUngCanhBao_ChinhSua;
            }
            set
            {
                if (_mThongTinChung_DiUngCanhBao_ChinhSua == value)
                    return;
                _mThongTinChung_DiUngCanhBao_ChinhSua = value && Globals.isConsultationStateEdit;
                NotifyOfPropertyChange(() => mThongTinChung_DiUngCanhBao_ChinhSua);
            }
        }
        public bool mThongTinChung_TinhTrangTheChat_ThongTin
        {
            get
            {
                return _mThongTinChung_TinhTrangTheChat_ThongTin;
            }
            set
            {
                if (_mThongTinChung_TinhTrangTheChat_ThongTin == value)
                    return;
                _mThongTinChung_TinhTrangTheChat_ThongTin = value;
            }
        }
        public bool mThongTinChung_TinhTrangTheChat_ChinhSua
        {
            get
            {
                return _mThongTinChung_TinhTrangTheChat_ChinhSua;
            }
            set
            {
                if (_mThongTinChung_TinhTrangTheChat_ChinhSua == value)
                    return;
                _mThongTinChung_TinhTrangTheChat_ChinhSua = value && Globals.isConsultationStateEdit;
                NotifyOfPropertyChange(() => mThongTinChung_TinhTrangTheChat_ChinhSua);
            }
        }
        public bool mThongTinChung_ThongTinLienHe_ThongTin
        {
            get
            {
                return _mThongTinChung_ThongTinLienHe_ThongTin;
            }
            set
            {
                if (_mThongTinChung_ThongTinLienHe_ThongTin == value)
                    return;
                _mThongTinChung_ThongTinLienHe_ThongTin = value;
            }
        }
        public bool mThongTinChung_ThongTinLienHe_ChinhSua
        {
            get
            {
                return _mThongTinChung_ThongTinLienHe_ChinhSua;
            }
            set
            {
                if (_mThongTinChung_ThongTinLienHe_ChinhSua == value)
                    return;
                _mThongTinChung_ThongTinLienHe_ChinhSua = value;
            }
        }
        public bool mThongTinChung_BHYT_ThongTin
        {
            get
            {
                return _mThongTinChung_BHYT_ThongTin;
            }
            set
            {
                if (_mThongTinChung_BHYT_ThongTin == value)
                    return;
                _mThongTinChung_BHYT_ThongTin = value;
            }
        }
        public bool mThongTinChung_XemLanKhamBenh
        {
            get
            {
                return _mThongTinChung_XemLanKhamBenh;
            }
            set
            {
                if (_mThongTinChung_XemLanKhamBenh == value)
                    return;
                _mThongTinChung_XemLanKhamBenh = value;
            }
        }

        public IConsultations UCConsultations { get; set; }

        public ObservableCollection<DiagnosisIcd10Items> refIDC10List
        {
            get
            {
                return UCConsultations.refIDC10List;
            }
            set
            {
                UCConsultations.refIDC10List = value;
            }
        }
        public DiagnosisTreatment DiagTrmtItem
        {
            get
            {
                return UCConsultations.DiagTrmtItem;
            }
            set
            {
                UCConsultations.DiagTrmtItem = value;
            }
        }

        public bool btUpdateIsEnabled
        {
            get { return UCConsultations.btUpdateIsEnabled; }
        }
        public bool btSaveCreateNewIsEnabled
        {
            get { return UCConsultations.btSaveCreateNewIsEnabled; }
        }
        /*▼====: #001*/
        public bool IsDiagTrmentChanged
        {
            get
            {
                return UCConsultations.IsDiagTrmentChanged;
            }
            set
            {
                UCConsultations.IsDiagTrmentChanged = value;
            }
        }
        /*▲====: #001*/
        public bool IsShowEditTinhTrangTheChat
        {
            get
            {
                return UCConsultations.IsShowEditTinhTrangTheChat;
            }
            set
            {
                UCConsultations.IsShowEditTinhTrangTheChat = value;
            }
        }
        public bool IsVisibility
        {
            get
            {
                return UCConsultations.IsVisibility;
            }
            set
            {
                UCConsultations.IsVisibility = value;
            }
        }
        public bool IsVisibilitySkip
        {
            get
            {
                return UCConsultations.IsVisibilitySkip;
            }
            set
            {
                UCConsultations.IsVisibilitySkip = value;
            }
        }
        public bool FormEditorIsEnabled
        {
            get
            {
                return UCConsultations.FormEditorIsEnabled;
            }
            set
            {
                UCConsultations.FormEditorIsEnabled = value;
            }
        }
        #endregion
        #region Handles
        public void Handle(CommonClosedPhysicalForSummaryEvent message)
        {
            //▼====== #002
            //InitPhyExam(PatientID);
            long tmpPtRegistrationID = 0;
            long tmpV_RegistrationType = 0;
            if (Registration_DataStorage.CurrentPatientRegistration != null)
            {
                tmpPtRegistrationID = Registration_DataStorage.CurrentPatientRegistration.PtRegistrationID;
                tmpV_RegistrationType = (long)Registration_DataStorage.CurrentPatientRegistration.V_RegistrationType;
            }
            GetPhyExam_ByPtRegID(tmpPtRegistrationID, tmpV_RegistrationType);
            //▲====== #002
        }
        public void Handle(RiskFactorSaveCompleteEvent message)
        {
            RiskFactorGet((long?)PatientInfo.PatientID);
        }
        public void Handle(LoadGeneralInfoPageEvent message)
        {
            if (message != null)
            {
                InitConsultationInfo(message.Patient);
            }
        }
        public void Handle(Re_ReadAllergiesEvent message)
        {
            //TBL: Khi them di ung thanh cong thi khong hien len ma phai load lai dang ky
            //if (this.GetView() != null)
            //{
            //    MDAllergies_ByPatientID(PatientInfo.PatientID, 1);/*1 la Active*/
            //}
            MDAllergies_ByPatientID(PatientInfo.PatientID, 1);
        }
        public void Handle(Re_ReadWarningEvent message)
        {
            //TBL: Khi them canh bao thanh cong thi khong hien len ma phai load lai dang ky
            //if (this.GetView() != null)
            //{
            //    MDWarnings_ByPatientID(PatientInfo.PatientID, 1);/*1 la Active*/
            //}
            MDWarnings_ByPatientID(PatientInfo.PatientID, 1);
        }
        #endregion
        #region Methods
        private Staff GetStaffLogin()
        {
            return Globals.LoggedUserAccount.Staff;
        }
        private void DeleteRiskFactor()
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                //IsLoading = true;

                using (var serviceFactory = new ComRecordsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginRiskFactorDelete(curRiskFactors.RiskFactorID, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            var results = contract.EndRiskFactorDelete(asyncResult);
                            RiskFactorGet((long?)PatientInfo.PatientID);
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            //Globals.IsBusy = false;
                            //IsLoading = false;
                            this.HideBusyIndicator();
                        }

                    }), null);

                }

            });
            t.Start();
        }
        private void MDAllergies_ByPatientID(Int64 PatientID, int flag)
        {
            this.ShowBusyIndicator();

            var t = new Thread(() =>
            {
                //IsLoading = true;

                using (var serviceFactory = new SummaryServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginMDAllergies_ByPatientID(PatientID, flag, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndMDAllergies_ByPatientID(asyncResult);

                            string str = "";

                            if (results != null)
                            {
                                if (PtAllergyList == null)
                                {
                                    PtAllergyList = new ObservableCollection<MDAllergy>();
                                }
                                else
                                {
                                    PtAllergyList.Clear();
                                }
                                foreach (MDAllergy p in results)
                                {
                                    PtAllergyList.Add(p);
                                    str += p.AllergiesItems.Trim() + ";";
                                }
                            }
                            if (!string.IsNullOrEmpty(str))
                            {
                                str = str.Substring(0, str.Length - 1);
                            }
                            Globals.Allergies = str;

                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            //IsLoading = false;
                            this.HideBusyIndicator();
                        }

                    }), null);

                }

            });
            t.Start();
        }
        private void MDWarnings_ByPatientID(Int64 PatientID, int flag)
        {
            this.ShowBusyIndicator();

            var t = new Thread(() =>
            {
                //IsLoading = true;

                using (var serviceFactory = new SummaryServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginMDWarnings_ByPatientID(PatientID, flag, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            var Res = contract.EndMDWarnings_ByPatientID(asyncResult);

                            if (Res.Count > 0)
                            {
                                ObjMDWarnings_ByPatientID = Res[0];
                                SPtWarningList = ObjMDWarnings_ByPatientID.WarningItems.Trim();
                                CanSelectClearChk = true;
                            }
                            else
                            {
                                ObjMDWarnings_ByPatientID = new MDWarning();
                                ObjMDWarnings_ByPatientID.PatientID = PatientID;
                                ObjMDWarnings_ByPatientID.StaffID = Globals.LoggedUserAccount.Staff.StaffID;
                                SPtWarningList = "";
                                CanSelectClearChk = false;
                            }

                            Globals.Warning = SPtWarningList.Trim();
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            //IsLoading = false;
                            this.HideBusyIndicator();
                        }

                    }), null);

                }

            });
            t.Start();
        }
        private void RiskFactorGet(long? PatientID)
        {
            this.ShowBusyIndicator();

            var t = new Thread(() =>
            {
                //IsLoading = true;

                using (var serviceFactory = new ComRecordsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginRiskFactorGet(PatientID, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            allRiskFactors = contract.EndRiskFactorGet(asyncResult).ToObservableCollection();
                            curRiskFactors = allRiskFactors != null && allRiskFactors.Count > 0 ?
                                allRiskFactors[0] : new RiskFactors();
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            //IsLoading = false;
                            this.HideBusyIndicator();
                        }

                    }), null);

                }

            });
            t.Start();
        }
        private void InitPhyExam(long patientID)
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            this.ShowBusyIndicator();

            var t = new Thread(() =>
            {
                //IsLoading = true;

                using (var serviceFactory = new SummaryServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetLastPhyExamByPtID(patientID, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            PtPhyExamItem = contract.EndGetLastPhyExamByPtID(asyncResult);
                            Globals.curPhysicalExamination = PtPhyExamItem;
                            //if(PtPhyExamItem==null)
                            //    PtPhyExamItem=new PhysicalExamination();

                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            //Globals.IsBusy = false;
                            //IsLoading = false;
                            this.HideBusyIndicator();
                        }

                    }), null);

                }

            });
            t.Start();
        }
        private void InitDiagnosic(long patientID, int pageIndex, int PageSize)
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            this.ShowBusyIndicator();
            int total = 0;
            var t = new Thread(() =>
            {
                //IsLoading = true;

                using (var serviceFactory = new ePMRsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetDiagnosisTreatmentListByPtID(patientID, 0, "", 0, null, pageIndex, PageSize, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            var results = contract.EndGetDiagnosisTreatmentListByPtID(out total, asyncResult);
                            
                            if (results != null)
                            {
                                if (PtSumDiagList == null)
                                {
                                    PtSumDiagList = new PagedSortableCollectionView<DiagnosisTreatment>();
                                }
                                else
                                {
                                    PtSumDiagList.Clear();
                                }
                                PtSumDiagList.TotalItemCount = total;
                                foreach (DiagnosisTreatment p in results)
                                {
                                    PtSumDiagList.Add(p);
                                }
                                NotifyOfPropertyChange(() => PtSumDiagList);
                            }
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            //Globals.IsBusy = false;
                            //IsLoading = false;
                            this.HideBusyIndicator();
                        }

                    }), null);

                }

            });
            t.Start();
        }
        private void InitData(Patient p)
        {
            if (p != null)
            {
                PatientInfo = p;
                PatientID = p.PatientID;
                MDAllergies_ByPatientID(PatientInfo.PatientID, 1);/*1 la Active*/
                MDWarnings_ByPatientID(PatientInfo.PatientID, 1);/*1 la Active*/

                //InitDiagnosic(PatientInfo.PatientID, 0, PtSumDiagList.PageSize);
                InitDiagnosic(PatientInfo.PatientID, PtSumDiagList.PageIndex, PtSumDiagList.PageSize);
                //InitPhyExam(PatientInfo.PatientID);
                //KMx: Khi chọn đăng ký để khám là đã load PhyExam rồi, không cần load lại (File: ConsultationModuleViewModel.cs (event ItemSelected<PatientRegistration, PatientRegistrationDetail>) 12/05/2014 17:12).
                PtPhyExamItem = Globals.curPhysicalExamination;
                RiskFactorGet((long?)PatientInfo.PatientID);
            }
        }
        private void MDWarnings_IsDeleted(MDWarning Obj)
        {
            this.ShowBusyIndicator();

            var t = new Thread(() =>
            {
                //IsLoading = true;

                using (var serviceFactory = new SummaryServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginMDWarnings_IsDeleted(ObjMDWarnings_ByPatientID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            string Result = "";
                            contract.EndMDWarnings_IsDeleted(out Result, asyncResult);

                            if (Result == "0")
                            {
                                MessageBox.Show(eHCMSResources.K0472_G1_XoaCBaoFail);
                            }
                            else
                            {
                                MDWarnings_ByPatientID(PatientInfo.PatientID, 1);/*1 la Active*/
                            }

                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            //Globals.IsBusy = false;
                            //IsLoading = false;
                            this.HideBusyIndicator();
                        }

                    }), null);

                }

            });
            t.Start();
        }
        public void Authorization()
        {
            if (!Globals.isAccountCheck)
            {
                return;
            }

            mThongTinChung_TimBN = Globals.CheckAuthorization(Globals.listRefModule
                        , (int)eModules.mConsultation
                        , (int)eConsultation.mPtDashboardSummary
                        , (int)oConsultationEx.mThongTinChung_TimBN
                        , (int)ePermission.mView);
            mThongTinChung_TimDK = Globals.CheckAuthorization(Globals.listRefModule
                        , (int)eModules.mConsultation
                        , (int)eConsultation.mPtDashboardSummary
                        , (int)oConsultationEx.mThongTinChung_TimDK
                        , (int)ePermission.mView);
            mThongTinChung_ThongTinBN = Globals.CheckAuthorization(Globals.listRefModule
                        , (int)eModules.mConsultation
                        , (int)eConsultation.mPtDashboardSummary
                        , (int)oConsultationEx.mThongTinChung_ThongTinBN
                        , (int)ePermission.mView);

            mThongTinChung_DiUngCanhBao_ChinhSua = Globals.CheckAuthorization(Globals.listRefModule
                        , (int)eModules.mConsultation
                        , (int)eConsultation.mPtDashboardSummary
                        , (int)oConsultationEx.mThongTinChung_DiUngCanhBao_ChinhSua
                        , (int)ePermission.mView);
            mThongTinChung_DiUngCanhBao_ThongTin = Globals.CheckAuthorization(Globals.listRefModule
                        , (int)eModules.mConsultation
                        , (int)eConsultation.mPtDashboardSummary
                        , (int)oConsultationEx.mThongTinChung_DiUngCanhBao_ThongTin
                        , (int)ePermission.mView) || mThongTinChung_DiUngCanhBao_ChinhSua;

            mThongTinChung_TinhTrangTheChat_ChinhSua = Globals.CheckAuthorization(Globals.listRefModule
                        , (int)eModules.mConsultation
                        , (int)eConsultation.mPtDashboardSummary
                        , (int)oConsultationEx.mThongTinChung_TinhTrangTheChat_ChinhSua
                        , (int)ePermission.mView);
            mThongTinChung_TinhTrangTheChat_ThongTin = Globals.CheckAuthorization(Globals.listRefModule
                        , (int)eModules.mConsultation
                        , (int)eConsultation.mPtDashboardSummary
                        , (int)oConsultationEx.mThongTinChung_TinhTrangTheChat_ThongTin
                        , (int)ePermission.mView) || mThongTinChung_TinhTrangTheChat_ChinhSua;

            mThongTinChung_ThongTinLienHe_ChinhSua = Globals.CheckAuthorization(Globals.listRefModule
                        , (int)eModules.mConsultation
                        , (int)eConsultation.mPtDashboardSummary
                        , (int)oConsultationEx.mThongTinChung_ThongTinLienHe_ChinhSua
                        , (int)ePermission.mView);
            mThongTinChung_ThongTinLienHe_ThongTin = Globals.CheckAuthorization(Globals.listRefModule
                        , (int)eModules.mConsultation
                        , (int)eConsultation.mPtDashboardSummary
                        , (int)oConsultationEx.mThongTinChung_ThongTinLienHe_ThongTin
                        , (int)ePermission.mView) || mThongTinChung_ThongTinLienHe_ChinhSua;
            mThongTinChung_BHYT_ThongTin = Globals.CheckAuthorization(Globals.listRefModule
                        , (int)eModules.mConsultation
                        , (int)eConsultation.mPtDashboardSummary
                        , (int)oConsultationEx.mThongTinChung_BHYT_ThongTin
                        , (int)ePermission.mView);
            mThongTinChung_XemLanKhamBenh = Globals.CheckAuthorization(Globals.listRefModule
                        , (int)eModules.mConsultation
                        , (int)eConsultation.mPtDashboardSummary
                        , (int)oConsultationEx.mThongTinChung_XemLanKhamBenh
                        , (int)ePermission.mView);
        }
        public void InitConsultationInfo(Patient patientInfo)
        {
            mThongTinChung_DiUngCanhBao_ChinhSua = true && Globals.isConsultationStateEdit;
            mThongTinChung_TinhTrangTheChat_ChinhSua = true && Globals.isConsultationStateEdit;
            Authorization();
            NotifyOfPropertyChange(() => mThongTinChung_DiUngCanhBao_ChinhSua);
            NotifyOfPropertyChange(() => mThongTinChung_TinhTrangTheChat_ChinhSua);

            if (patientInfo != null)
            {
                InitData(patientInfo);
                UCConsultations.InitPatientInfo();
            }
        }
        public void hplDelete_Click(object selectedItem)
        {
            MDAllergy p = (selectedItem as MDAllergy);

            if (p != null && p.AItemID > 0)
            {
                if (MessageBox.Show(string.Format(eHCMSResources.Z0476_G1_BanMuonXoa, p.AllergiesItems.Trim()), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    MDAllergies_IsDeleted(p);
                }
            }
        }
        private void MDAllergies_IsDeleted(MDAllergy Obj)
        {
            this.ShowBusyIndicator();

            var t = new Thread(() =>
            {
                //IsLoading = true;

                using (var serviceFactory = new SummaryServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginMDAllergies_IsDeleted(Obj, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            string Result = "";
                            contract.EndMDAllergies_IsDeleted(out Result, asyncResult);

                            if (Result == "0")
                            {
                                MessageBox.Show(eHCMSResources.K0473_G1_XoaDiUngFail);
                            }
                            else
                            {
                                MDAllergies_ByPatientID(PatientInfo.PatientID, 1);/*1 la Active*/
                            }

                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            //Globals.IsBusy = false;
                            //IsLoading = false;
                            this.HideBusyIndicator();
                        }

                    }), null);

                }

            });
            t.Start();
        }
        public bool CheckValidDiagnosis()
        {
            return UCConsultations.CheckValidDiagnosis();
        }
        public long Compare2Object()
        {
            return UCConsultations.Compare2Object();
        }
        public void ChangeStatesAfterUpdated(bool IsUpdate = false)
        {
            UCConsultations.ChangeStatesAfterUpdated();
        }
        #endregion
        #region Events
        public void hplDelete_Click()
        {
            DeleteRiskFactor();
        }
        public void hpkEditAllergiesWarning()
        {
            Action<IAllergies> onInitDlg = delegate (IAllergies vm)
            {
                vm.Allergy = new MDAllergy();
                vm.Allergy.PatientID = Registration_DataStorage.CurrentPatient.PatientID;
                vm.Allergy.StaffID = Globals.LoggedUserAccount.Staff.StaffID;
                vm.Warning = ObjectCopier.DeepCopy(ObjMDWarnings_ByPatientID);
                vm.PatientID = PatientID;
                vm.CaseOfAllergyType = (long)AllergyType.Drug;
            };
            GlobalsNAV.ShowDialog<IAllergies>(onInitDlg);
        }
        public void hpkClearWarning(object sender, RoutedEventArgs e)
        {
            if (ObjMDWarnings_ByPatientID.WItemID <= 0)
                return;

            if (MessageBox.Show(eHCMSResources.A0153_G1_Msg_ConfXoaCBao, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                MDWarnings_IsDeleted(ObjMDWarnings_ByPatientID);
            }
        }
        public void hpkNewRiskFactor(object sender, RoutedEventArgs e)
        {
            if (PatientID < 0)
            {
                MessageBox.Show(eHCMSResources.A0378_G1_Msg_InfoChuaChonBN);
                return;
            }
            Action<IRiskFactors> onInitDlg = delegate (IRiskFactors vm)
            {
                vm.PatientID = PatientID;
                vm.curRiskFactors = ObjectCopier.DeepCopy(curRiskFactors);
            };
            GlobalsNAV.ShowDialog<IRiskFactors>(onInitDlg);
        }
        public void hpkEditPhysicalExam()
        {
            Action<IcwPhysiscalExam> onInitDlg = delegate (IcwPhysiscalExam proAlloc)
            {
                proAlloc.PatientID = PatientID;
                
                if (PtPhyExamItem == null)
                {
                    proAlloc.IsVisibility = Visibility.Collapsed;
                    proAlloc.isEdit = false;
                }
                else
                {
                    proAlloc.PtPhyExamItem = ObjectCopier.DeepCopy(PtPhyExamItem);
                    proAlloc.IsVisibility = Visibility.Visible;
                    proAlloc.isEdit = true;
                }
                //▼====== #002
                if (Registration_DataStorage.CurrentPatientRegistration != null)
                {
                    proAlloc.PtPhyExamItem.PtRegistrationID = Registration_DataStorage.CurrentPatientRegistration.PtRegistrationID;
                    proAlloc.PtPhyExamItem.V_RegistrationType = (long)Registration_DataStorage.CurrentPatientRegistration.V_RegistrationType;
                }
                //▲====== #002
            };
            GlobalsNAV.ShowDialog<IcwPhysiscalExam>(onInitDlg);
        }
        public void hpkNewPhysicalExam()
        {
            Action<IcwPhysiscalExam> onInitDlg = delegate (IcwPhysiscalExam proAlloc)
            {
                proAlloc.PatientID = PatientID;
                //▼====== #002
                if (Registration_DataStorage.CurrentPatientRegistration != null)
                {
                    proAlloc.PtPhyExamItem.PtRegistrationID = Registration_DataStorage.CurrentPatientRegistration.PtRegistrationID;
                    proAlloc.PtPhyExamItem.V_RegistrationType = (long)Registration_DataStorage.CurrentPatientRegistration.V_RegistrationType;
                }
                //▲====== #002
                proAlloc.IsVisibility = Visibility.Collapsed;
                proAlloc.PtPhyExamItem = new PhysicalExamination();
            };
            GlobalsNAV.ShowDialog<IcwPhysiscalExam>(onInitDlg);
        }

        //▼====== #002
        private void GetPhyExam_ByPtRegID(long PtRegistrationID, long V_RegistrationType)
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                using (var serviceFactory = new SummaryServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetPhyExam_ByPtRegID(PtRegistrationID, V_RegistrationType, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            PtPhyExamItem = contract.EndGetPhyExam_ByPtRegID(asyncResult);
                            Globals.curPhysicalExamination = PtPhyExamItem;
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
        //▲====== #002
        public void hpkBedPatientAlloc(object sender, RoutedEventArgs e)
        {
            Action<IBedPatientAlloc> onInitDlg = delegate (IBedPatientAlloc BedPatientAllocVM)
            {
                //goi toi childwindow dat giuong
                BedPatientAllocVM.PatientInfo = PatientInfo;
                BedPatientAllocVM.curPatientRegistration = curPatientRegistration;
                this.ActivateItem(BedPatientAllocVM);
                //Globals.ShowDialog(BedPatientAllocVM as Conductor<object>);
            };
            GlobalsNAV.ShowDialog<IBedPatientAlloc>(onInitDlg);

            //Globals.LoadDynamicModule<IBedPatientAlloc>("eHCMS.Configuration.xap");
        }
        #endregion
        public string ProcedureDescription
        {
            get
            {
                return UCConsultations == null ? null : UCConsultations.ProcedureDescription;
            }
        }
        public string ProcedureDescriptionContent
        {
            get
            {
                return UCConsultations == null ? null : UCConsultations.ProcedureDescriptionContent;
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
                UCConsultations.Registration_DataStorage = Registration_DataStorage;
            }
        }
        public void ICD10Changed(ObservableCollection<DiagnosisIcd10Items> ICD10List)
        {
            UCConsultations.ICD10Changed(ICD10List);
        }
    }
}