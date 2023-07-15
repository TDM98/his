using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using aEMR.CommonTasks;
using aEMR.ViewContracts;
using aEMR.ServiceClient;
using Caliburn.Micro;
using aEMR.Common.BaseModel;
using aEMR.Common.Collections;
using System.Collections.ObjectModel;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using DataEntities;
using System.Linq;
using aEMR.Controls;
using eHCMSLanguage;
using System.Windows.Media;
using aEMR.ServiceClient.Consultation_PCLs;
using Castle.Windsor;
using System.Windows.Input;
using aEMR.DataContracts;
using System.ServiceModel;
/*
* 20170410 CMN  #001:   Change back color of main ICD10 Row
* 20180913 TTM  #002:   Fixed cannot show information on dialog paperreferal full when user click on hyperlink
* 20180920 TBL  #003:   Chinh sua chuc nang bug mantis ID 0000061, thay doi thuoc tinh IsObjectBeingUsedByClient, IsDataChanged, IsDiagTrmentChanged
* 20180923 TBL  #004:   BM 0000066: khi thuc hien chan doan moi dua tren cu thi khong duoc hieu chinh ma phai load lai dang ky.
* 20180924 TTM  #005:   Chuyển dữ liệu của DiagnosisTreatment và refICD10list từ trong này ra ngoài chỗ đặt dữ liệu chung ở màn hình khám bệnh gộp (ConsultationSummary).
* 20181004 TBL  #006:   BM 0000125: Khi tạo chẩn đoán mới dựa trên cũ, dựa theo cấu hình mà copy các trường
* 20181005 TBL  #007:   BM 0000126: Valid giao diện ở màn hình chẩn đoán
* 20181005 TBL  #008:   BM 0000147: Thay doi list ICD, bam luu bao khong co gi de luu
* 20181015 TBL  #009:   BM 0002179: Valid các trường cần phải nhập ở màn hình chẩn đoán
* 20181023 TTM  #010:   BM 0002173: Thay đổi cách lưu, cập nhật và lấy lên của tình trạng thể chất => tất cả đều dựa vào lần đăng ký.
* 20181025 TBL  #011:   BM 0003231: Fix cho BM 0000126
* 20181027 TBL  #012:   BM 0000130: Cho nguoi dung chon khong can nhap truong cach dieu tri
* 20190403 TTM  #013:   BM 0006651: Hiển thị thông tin đề nghị nhập viện của bệnh nhân 
* 20190205 TTM  #014:   BM 0006823: Fix lỗi nếu bệnh nhân nhập viện từ đề nghị nhập viện thì thông tin ca đó là trái tuyến.
* 20190914 TBL  #015:   BM 0014336: Ràng buộc lại qui trình đề nghị nhập viện từ phiếu khám
* 
* 20191018 TTM  #017:   BM 0018471: Fix lỗi cách điều trị khi search bằng cách gõ vào trong combobox khi lưu lại sẽ bị mất ItemSource
* 20210701 TNHX #018:   260 thêm user bsi mượn vào chẩn đoán
*/
namespace aEMR.Common.ViewModels
{
    [Export(typeof(IConsultationOld)), PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class ConsultationOldViewModel : ViewModelBase, IConsultationOld
        , IHandle<ConsultationDoubleClickEvent>
        //, IHandle<ShowPatientInfo_KHAMBENH_CHANDOAN<Patient, PatientRegistration, PatientRegistrationDetail>>
        //, IHandle<ReloadDataConsultationEvent>
        , IHandle<EventKhamChoVIP<PatientRegistration>>
        , IHandle<GlobalCurPatientServiceRecordLoadComplete_Consult>
        , IHandle<CommonClosedPhysicalForDiagnosisEvent>
        , IHandle<Icd10CollectionSelected>
    {
        #region Busy Indicator binding
        public override bool IsProcessing
        {
            get
            {
                return false;
                //return _IsWaitingGetLatestDiagnosisTreatmentByPtID
                //    || _IsWaitingGetBlankDiagnosisTreatmentByPtID
                //    || _IsWaitingSaveAddNew
                //    || _IsWaitingUpdate
                //    || _IsWaitingLoadICD10;
            }
        }

        public override string StatusText
        {
            get
            {
                if (_IsWaitingGetLatestDiagnosisTreatmentByPtID)
                {
                    return eHCMSResources.Z0486_G1_LayChanDoanCuoi;
                }
                if (_IsWaitingGetBlankDiagnosisTreatmentByPtID)
                {
                    return eHCMSResources.K2882_G1_DangTaiDLieu;
                }
                if (_IsWaitingSaveAddNew)
                {
                    return string.Format(eHCMSResources.Z0487_G1_DangLuu, eHCMSResources.K1746_G1_CDoan);
                }
                if (_IsWaitingUpdate)
                {
                    return string.Format(eHCMSResources.Z0488_G1_DangCapNhat, eHCMSResources.K1746_G1_CDoan);
                }
                if (_IsWaitingLoadICD10)
                {
                    return string.Format(eHCMSResources.Z0489_G1_DangLoad, eHCMSResources.T1793_G1_ICD10);
                }

                return string.Empty;
            }
        }

        private bool _IsWaitingLoadICD10;
        public bool IsWaitingLoadICD10
        {
            get { return _IsWaitingLoadICD10; }
            set
            {
                if (_IsWaitingLoadICD10 != value)
                {
                    _IsWaitingLoadICD10 = value;
                    NotifyOfPropertyChange(() => IsWaitingLoadICD10);
                    NotifyWhenBusy();
                }
            }
        }

        private bool _IsWaitingUpdate;
        public bool IsWaitingUpdate
        {
            get { return _IsWaitingUpdate; }
            set
            {
                if (_IsWaitingUpdate != value)
                {
                    _IsWaitingUpdate = value;
                    NotifyOfPropertyChange(() => IsWaitingUpdate);
                    NotifyWhenBusy();
                }
            }
        }

        private bool _IsWaitingSaveAddNew;
        public bool IsWaitingSaveAddNew
        {
            get { return _IsWaitingSaveAddNew; }
            set
            {
                if (_IsWaitingSaveAddNew != value)
                {
                    _IsWaitingSaveAddNew = value;
                    NotifyOfPropertyChange(() => IsWaitingSaveAddNew);
                    NotifyWhenBusy();
                }
            }
        }

        private bool _IsWaitingGetLatestDiagnosisTreatmentByPtID;
        public bool IsWaitingGetLatestDiagnosisTreatmentByPtID
        {
            get { return _IsWaitingGetLatestDiagnosisTreatmentByPtID; }
            set
            {
                if (_IsWaitingGetLatestDiagnosisTreatmentByPtID != value)
                {
                    _IsWaitingGetLatestDiagnosisTreatmentByPtID = value;
                    NotifyOfPropertyChange(() => IsWaitingGetLatestDiagnosisTreatmentByPtID);
                    NotifyWhenBusy();
                }
            }
        }

        private bool _IsWaitingGetBlankDiagnosisTreatmentByPtID;
        public bool IsWaitingGetBlankDiagnosisTreatmentByPtID
        {
            get { return _IsWaitingGetBlankDiagnosisTreatmentByPtID; }
            set
            {
                if (_IsWaitingGetBlankDiagnosisTreatmentByPtID != value)
                {
                    _IsWaitingGetBlankDiagnosisTreatmentByPtID = value;
                    NotifyOfPropertyChange(() => IsWaitingGetBlankDiagnosisTreatmentByPtID);
                    NotifyWhenBusy();
                }
            }
        }

        private bool _IsShowEditTinhTrangTheChat = true;
        public bool IsShowEditTinhTrangTheChat
        {
            get
            {
                return _IsShowEditTinhTrangTheChat && !IsUpdateFromPresciption;
            }
            set
            {
                if(_IsShowEditTinhTrangTheChat != value)
                {
                    _IsShowEditTinhTrangTheChat = value;
                    NotifyOfPropertyChange(() => IsShowEditTinhTrangTheChat);
                }
            }
        }
        #endregion
        private Visibility _VisibilityOtherDiagnosis = Visibility.Collapsed;
        public Visibility VisibilityOtherDiagnosis
        {
            get { return _VisibilityOtherDiagnosis; }
            set
            {
                _VisibilityOtherDiagnosis = value;
                NotifyOfPropertyChange(() => VisibilityOtherDiagnosis);
            }
        }
        private Visibility _VisibilyDiagnosisType = Visibility.Collapsed;
        public Visibility VisibilyDiagnosisType
        {
            get { return _VisibilyDiagnosisType; }
            set
            {
                _VisibilyDiagnosisType = value;
                NotifyOfPropertyChange(() => VisibilyDiagnosisType);
            }
        }
        private string[] _V_TreatmentTypeArray = Globals.ServerConfigSection.CommonItems.ApplyCheckV_TreatmentType.Split('|');
        public string[] V_TreatmentTypeArray
        {
            get
            {
                return _V_TreatmentTypeArray;
            }
            set
            {
                if (_V_TreatmentTypeArray != value)
                {
                    _V_TreatmentTypeArray = value;
                    NotifyOfPropertyChange(() => V_TreatmentTypeArray);
                }
            }
        }
        public void cboTreatmentTypeItemChanged(object sender, EventArgs e)
        {

            if (V_TreatmentTypeArray.Length == 0)
            {
                return;
            }
            else
            {
                if (CheckV_TreatmentType(DiagTrmtItem.V_TreatmentType))
                {
                    Globals.EventAggregator.Publish(new CheckV_TreatmentType() { IsEnableAddPrescription = true });
                }
                else
                {
                    Globals.EventAggregator.Publish(new CheckV_TreatmentType() { IsEnableAddPrescription = false });
                }
            }
        }
        private bool CheckV_TreatmentType(long V_TreatmentType)
        {
            int temp = 0;
            for (int i = 0; i < V_TreatmentTypeArray.Length; i++)
            {
                if (DiagTrmtItem.V_TreatmentType == Convert.ToInt64(V_TreatmentTypeArray[i]))
                {
                    temp++;
                }
            }
            if (temp == 0)
            {
                return false;
            }
            else
            {
                return true;
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
        [ImportingConstructor]
        public ConsultationOldViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg)
        {
            System.Diagnostics.Debug.WriteLine("======> ConsultationOldViewModel - Constructor");

            authorization();

            LoadInitData();

            GetAllLookupValuesByType();

            if (Globals.LoggedUserAccount != null)
            {
                GetShortHandDictionaries(Globals.LoggedUserAccount.StaffID.GetValueOrDefault(0));
            }
            if (Globals.ServerConfigSection.CommonItems.ApplyOtherDiagnosis)
            {
                VisibilityOtherDiagnosis = Visibility.Visible;
            }
            else
            {
                VisibilityOtherDiagnosis = Visibility.Collapsed;
            }
        }

        ~ConsultationOldViewModel()
        {
            System.Diagnostics.Debug.WriteLine("======> ConsultationOldViewModel - Destructor");
        }

        protected override void OnActivate()
        {
            Globals.EventAggregator.Subscribe(this);
            base.OnActivate();
            InitPatientInfo();
        }

        protected override void OnDeactivate(bool close)
        {
            Globals.EventAggregator.Unsubscribe(this);
            base.OnDeactivate(close);
        }

        //public void Handle(ShowPatientInfo_KHAMBENH_CHANDOAN<Patient, PatientRegistration, PatientRegistrationDetail> message)
        //{
        //    //InitPatientInfo();
        //}

        public void Handle(GlobalCurPatientServiceRecordLoadComplete_Consult message)
        {
            //TTM 17092018
            //if (Registration_DataStorage.CurrentPatientRegistration == null)
            //{
            //    MessageBox.Show(eHCMSResources.Z0412_G1_DLieuKBChuaDuocCBi, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
            //    return;
            //}

            //InitPatientInfo();
        }

        public IEnumerator<IResult> MessageWarningShowDialogTask(string strMessage)
        {
            var dialog = new MessageWarningShowDialogTask(strMessage, eHCMSResources.K1576_G1_CBao, false);
            yield return dialog;
            yield break;
        }

        private bool ValidateExpiredDiagnosicTreatment(DiagnosisTreatment diagnosicTreatment)
        {
            DateTime curDate = Globals.GetCurServerDateTime();
            if (diagnosicTreatment.DiagnosisDate.AddDays(Globals.ServerConfigSection.Hospitals.EditDiagDays) < curDate)
            {
                btEditIsEnabled = false;
                Coroutine.BeginExecute(MessageWarningShowDialogTask(string.Format(eHCMSResources.Z2196_G1_CDoanHetHieuLucChinhSuaN, Globals.ServerConfigSection.Hospitals.EditDiagDays)));
                return false;
            }
            return true;
        }
        public void CheckBeforeConsult()
        {
            //Kiem tra trang thai cua dang ky nay
            if (Registration_DataStorage.PatientServiceRecordCollection != null && Registration_DataStorage.PatientServiceRecordCollection.Count > 0)
            {

                DiagTrmtItem = ObjectCopier.DeepCopy(Registration_DataStorage.PatientServiceRecordCollection[0].DiagnosisTreatments[0]);

                //TBL: khong biet deepcopy de lam gi
                //DiagTrmtItemCopy = ObjectCopier.DeepCopy(DiagTrmtItem);
                
                ConsultState = ConsultationState.EditConsultationState;
                //dang ky dich vu nay da co chan doan
                //gan lai cho dang ky nay tu Globals
                if (Globals.IsLockRegistration(Registration_DataStorage.CurrentPatientRegistration.RegLockFlag, eHCMSResources.Z0407_G1_CNhatCDoan))
                {
                    ButtonControlsEnable = false;
                }
                else
                {
                    StateEdit();
                }
                DiagnosisIcd10Items_Load(DiagTrmtItem.ServiceRecID, null, false);

                ValidateExpiredDiagnosicTreatment(DiagTrmtItem);
                return;
            }
            else
            {

                if (Registration_DataStorage.CurrentPatientRegistration == null)
                {
                    return;
                }

                ConsultState = ConsultationState.NewConsultationState;
                StateNew();
                GetLatesDiagTrmtByPtID(Registration_DataStorage.CurrentPatient.PatientID);
                //KMx: Sau khi kiểm tra, thấy hàm này không cần thiết (01/11/2014 08:27).
                //GetPtRegistrationIDInDiagnosisTreatment_Latest(Registration_DataStorage.CurrentPatient.PatientID, Registration_DataStorage.CurrentPatientRegistration.PtRegistrationID);
                //kiem tra xem Ptregisdetail nay da co chan doan chua?neu co roi thi chi cho chinh sua thoi,khong cho tao moi j het
                if (Registration_DataStorage.CurrentPatientRegistrationDetail != null && Registration_DataStorage.CurrentPatientRegistrationDetail.PtRegDetailID > 0)
                {
                    CheckDiagnosisTreatmentExists_PtRegDetailID(Registration_DataStorage.CurrentPatient.PatientID, Registration_DataStorage.CurrentPatientRegistrationDetail.PtRegDetailID);
                }
            }
        }

        public void InitPatientInfo()
        {
            //refIDC10 = new PagedSortableCollectionView<DiseasesReference>();
            //refIDC10.OnRefresh += new EventHandler<RefreshEventArgs>(refIDC10_OnRefresh);
            //refIDC10.PageSize = Globals.PageSize;
            refIDC10Code = new ObservableCollection<DiseasesReference>();
            refIDC10Name = new ObservableCollection<DiseasesReference>();
            //refIDC10.OnRefresh += new EventHandler<RefreshEventArgs>(refIDC10_OnRefresh);
            //refIDC10.PageSize = Globals.PageSize;

            refIDC10Item = new DiagnosisIcd10Items();

            refIDC10List = new ObservableCollection<DiagnosisIcd10Items>();
            if (Registration_DataStorage.CurrentPatient != null && Registration_DataStorage.CurrentPatient.PatientID > 0)
            {
                //▼====== #010
                if (Registration_DataStorage.CurrentPatientRegistration != null)
                {
                    //InitPhyExam(Registration_DataStorage.CurrentPatient.PatientID);
                    GetPhyExam_ByPtRegID(Registration_DataStorage.CurrentPatientRegistration.PtRegistrationID, (long)Registration_DataStorage.CurrentPatientRegistration.V_RegistrationType);
                }
                //▲====== #010
                if (CheckDangKyHopLe())
                {
                    ButtonControlsEnable = true;

                    //Kiem tra phan quyen
                    if (!mChanDoan_tabSuaKhamBenh_ThongTin)
                    {
                        Globals.ShowMessage(eHCMSResources.Z0413_G1_ChuaDuocPQuyenXemBA, "");
                        return;
                    }
                    if (!Globals.isConsultationStateEdit)
                    {
                        MessageBox.Show(eHCMSResources.A0232_G1_Msg_InfoKhTheSua_BNTuLSBA);
                        ButtonControlsEnable = false;
                    }
                    CheckBeforeConsult();
                }
                else
                {
                    this.HideBusyIndicator();
                    FormEditorIsEnabled = false;
                }
            }
            else
            {
                this.HideBusyIndicator();
                FormEditorIsEnabled = false;
            }

        }

        //void refIDC10_OnRefresh(object sender, RefreshEventArgs e)
        //{
        //    LoadRefDiseases(Name, Type, refIDC10.PageIndex, refIDC10.PageSize);
        //}

        private bool CheckDangKyHopLe()
        {
            return Globals.CheckValidRegistrationForConsultation(Registration_DataStorage.CurrentPatientRegistration, Registration_DataStorage.CurrentPatientRegistrationDetail, Registration_DataStorage.CurrentPatient, Registration_DataStorage.PatientServiceRecordCollection, true);
            //if (Registration_DataStorage.CurrentPatientRegistration == null)
            //{
            //    MessageBox.Show(eHCMSResources.Z0402_G1_KgBietDKLoaiNao);
            //    return false;
            //}
            //if (Registration_DataStorage.CurrentPatientRegistration.V_RegistrationType != AllLookupValues.RegistrationType.NGOAI_TRU)
            //{
            //    MessageBox.Show(eHCMSResources.A0245_G1_Msg_InfoKhongPhaiNgTru_ChiXem, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
            //    return false;
            //}
            //if (Registration_DataStorage.CurrentPatientRegistrationDetail != null && Registration_DataStorage.CurrentPatientRegistrationDetail.PtRegDetailID > 0) //Có đăng ký và có đăng ký DV khám
            //{
            //    if (Registration_DataStorage.CurrentPatientRegistrationDetail.RefMedicalServiceItem != null && Registration_DataStorage.CurrentPatientRegistrationDetail.RefMedicalServiceItem.IsAllowToPayAfter == 0 && Registration_DataStorage.CurrentPatientRegistrationDetail.PaidTime == null)
            //    {
            //        return false;
            //    }
            //    //if ((Registration_DataStorage.PatientServiceRecordCollection == null || Registration_DataStorage.PatientServiceRecordCollection.Count < 1)
            //    //    && Globals.PatientAllDetails.RegistrationDetailInfo.PaidTime.Value.AddHours(Globals.EffectedDiagHours)
            //    //    < DateTime.Now)

            //    // Txd 25/05/2014 Replaced ConfigList
            //    if ((Registration_DataStorage.PatientServiceRecordCollection == null || Registration_DataStorage.PatientServiceRecordCollection.Count < 1) && (Registration_DataStorage.CurrentPatientRegistrationDetail.RefMedicalServiceItem == null || Registration_DataStorage.CurrentPatientRegistrationDetail.RefMedicalServiceItem.IsAllowToPayAfter == 0)
            //        && Registration_DataStorage.CurrentPatientRegistrationDetail.PaidTime.Value.AddHours(Globals.ServerConfigSection.Hospitals.EffectedDiagHours) < Globals.GetCurServerDateTime())
            //    {
            //        MessageBox.Show(string.Format(eHCMSResources.Z0414_G1_DKHetHieuLuc, Globals.ServerConfigSection.Hospitals.EffectedDiagHours.ToString()));
            //        return false;
            //    }
            //    return true;
            //}
            //else
            //{
            //    MessageBox.Show(Registration_DataStorage.CurrentPatient.FullName.Trim() + string.Format("({0})", eHCMSResources.T3719_G1_Mau20NgTru) + Environment.NewLine + eHCMSResources.T1278_G1_ChuaDKDVKBNao, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
            //    return false;
            //}
        }



        public void hpkEditPhysicalExam()
        {
            Action<IcwPhysiscalExam> onInitDlg = delegate (IcwPhysiscalExam proAlloc)
            {
                proAlloc.PatientID = Registration_DataStorage.CurrentPatient.PatientID;

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
                //▼====== #010
                if (Registration_DataStorage.CurrentPatientRegistration != null)
                {
                    proAlloc.PtPhyExamItem.PtRegistrationID = Registration_DataStorage.CurrentPatientRegistration.PtRegistrationID;
                    proAlloc.PtPhyExamItem.V_RegistrationType = (long)Registration_DataStorage.CurrentPatientRegistration.V_RegistrationType;
                }
                //▲====== #010
            };
            GlobalsNAV.ShowDialog<IcwPhysiscalExam>(onInitDlg);
        }

        public void hpkMedServiceReq()
        {
            //Action<IPatientMedicalServiceRequest> onInitDlg = delegate (IPatientMedicalServiceRequest proAlloc)
            //{
            //    proAlloc.Diagnosis = DiagTrmtItem.DiagnosisFinal;
            //};
            //GlobalsNAV.ShowDialog<IPatientMedicalServiceRequest>(onInitDlg);
            GlobalsNAV.ShowDialog<IPatientMedicalServiceRequest>((proAlloc) =>
            {
                proAlloc.Diagnosis = DiagTrmtItem.DiagnosisFinal;
                proAlloc.Registration_DataStorage = Registration_DataStorage;
            }, null, false, true, new Size(1800, 600));
        }

        #region Properties member
        //▼====== #005
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
        //▲====== #005
        private ObservableCollection<Lookup> _RefBehaving;
        public ObservableCollection<Lookup> RefBehaving
        {
            get
            {
                return _RefBehaving;
            }
            set
            {
                if (_RefBehaving != value)
                {
                    _RefBehaving = value;
                    NotifyOfPropertyChange(() => RefBehaving);
                }
            }
        }

        private ObservableCollection<Lookup> _RefDiagnosis;
        public ObservableCollection<Lookup> RefDiagnosis
        {
            get
            {
                return _RefDiagnosis;
            }
            set
            {
                if (_RefDiagnosis != value)
                {
                    _RefDiagnosis = value;
                    NotifyOfPropertyChange(() => RefDiagnosis);
                }
            }
        }

        private ObservableCollection<MedicalRecordTemplate> _RefMedRecTemplate;
        public ObservableCollection<MedicalRecordTemplate> RefMedRecTemplate
        {
            get
            {
                return _RefMedRecTemplate;
            }
            set
            {
                if (_RefMedRecTemplate != value)
                {
                    _RefMedRecTemplate = value;
                    NotifyOfPropertyChange(() => RefMedRecTemplate);
                }
            }
        }

        private ObservableCollection<DiagnosisIcd10Items> _refIDC10ListCopy;
        public ObservableCollection<DiagnosisIcd10Items> refIDC10ListCopy
        {
            get
            {
                return _refIDC10ListCopy;
            }
            set
            {
                if (_refIDC10ListCopy != value)
                {
                    _refIDC10ListCopy = value;
                    NotifyOfPropertyChange(() => refIDC10ListCopy);
                }
            }
        }

        //▼====== #005: Nếu như màn hình này được gọi bởi left or top menu => CS_DS = null => vẫn theo lối cũ và ngược lại
        private DiagnosisTreatment _NewDiagTrmtItem;
        public DiagnosisTreatment DiagTrmtItem
        {
            get
            {
                if (CS_DS != null)
                {
                    return CS_DS.DiagTreatment;
                }
                return _NewDiagTrmtItem;
            }
            set
            {
                if (CS_DS != null)
                {

                    if (CS_DS.DiagTreatment != value)
                    {
                        CS_DS.DiagTreatment = value;
                        if (CS_DS.DiagTreatment != null)
                        {
                            CS_DS.DiagTreatment.IsObjectBeingUsedByClient = true;
                        }
                        NotifyOfPropertyChange(() => DiagTrmtItem);
                        NotifyOfPropertyChange(() => btCreateNewByOldIsEnabled);
                        NotifyOfPropertyChange(() => IsInPtDiagnosis);
                    }
                }
                else if (_NewDiagTrmtItem != value)
                {
                    _NewDiagTrmtItem = value;
                    /*▼====: #003*/
                    if (_NewDiagTrmtItem != null)
                    {
                        DiagTrmtItem.IsObjectBeingUsedByClient = true;
                    }
                    /*▲====: #003*/
                    NotifyOfPropertyChange(() => DiagTrmtItem);
                    NotifyOfPropertyChange(() => btCreateNewByOldIsEnabled);
                    NotifyOfPropertyChange(() => IsInPtDiagnosis);
                }
            }
        }
        //▼====== #005
        public bool IsInPtDiagnosis
        {
            get
            {
                //20191023 TBL: Khi nào BNTKSauXV = true thì mới cho hiển thị thông báo 
                //return DiagTrmtItem != null && DiagTrmtItem.V_RegistrationType == AllLookupValues.RegistrationType.NOI_TRU;
                return Registration_DataStorage != null && Registration_DataStorage.CurrentPatientRegistration != null && Registration_DataStorage.CurrentPatientRegistration.BNTKSauXV == true;
            }
        }

        private DiagnosisTreatment _DiagTrmtItemCopy;
        public DiagnosisTreatment DiagTrmtItemCopy
        {
            get
            {
                return _DiagTrmtItemCopy;
            }
            set
            {
                if (_DiagTrmtItemCopy != value)
                {
                    _DiagTrmtItemCopy = value;
                }
                NotifyOfPropertyChange(() => DiagTrmtItemCopy);
            }
        }

        private bool _ButtonControlsEnable = false;
        public bool ButtonControlsEnable
        {
            get
            {
                return _ButtonControlsEnable;
            }
            set
            {
                _ButtonControlsEnable = value;
                NotifyOfPropertyChange(() => ButtonControlsEnable);
            }
        }

        private bool _FormEditorIsEnabled = false;
        public bool FormEditorIsEnabled
        {
            get
            {
                return _FormEditorIsEnabled;
            }
            set
            {
                _FormEditorIsEnabled = value;
                NotifyOfPropertyChange(() => FormEditorIsEnabled);
            }
        }

        private bool _btSaveCreateNewIsEnabled = true;
        public bool btSaveCreateNewIsEnabled
        {
            get
            {
                return _btSaveCreateNewIsEnabled;
            }
            set
            {
                _btSaveCreateNewIsEnabled = value;
                NotifyOfPropertyChange(() => btSaveCreateNewIsEnabled);
            }
        }
        /*▼====: #003*/
        private bool _IsDiagTrmentChanged = false;
        public bool IsDiagTrmentChanged
        {
            get
            {
                return _IsDiagTrmentChanged;
            }
            set
            {
                _IsDiagTrmentChanged = value;
                NotifyOfPropertyChange(() => IsDiagTrmentChanged);
            }
        }
        /*▲====: #003*/
        private bool _btCreateNewIsEnabled;
        public bool btCreateNewIsEnabled
        {
            //get { return IsEnableButton && IsNotExistsDiagnosisTreatmentByPtRegDetailID; }
            get { return _btCreateNewIsEnabled; }
            set
            {
                _btCreateNewIsEnabled = value;
                NotifyOfPropertyChange(() => btCreateNewIsEnabled);
            }
        }

        private bool _hasDiag = true;
        public bool hasDiag
        {
            get { return _hasDiag; }
            set
            {
                if (_hasDiag != value)
                {
                    _hasDiag = value;
                    NotifyOfPropertyChange(() => hasDiag);
                    NotifyOfPropertyChange(() => btCreateNewByOldIsEnabled);
                }
            }
        }

        private bool _btCreateNewByOldIsEnabled;
        public bool btCreateNewByOldIsEnabled
        {
            //get
            //{
            //    return IsEnableButton && IsNotExistsDiagnosisTreatmentByPtRegDetailID
            //        && (Globals.PatientAllDetails.RegistrationInfo != null &&  DiagTrmtItem != null && DiagTrmtItem.PatientServiceRecord != null && (DiagTrmtItem.PatientServiceRecord.PtRegistrationID == Globals.PatientAllDetails.RegistrationInfo.PtRegistrationID || DiagTrmtItem.PatientServiceRecord.PtRegistrationID == PtRegistrationIDLatest.GetValueOrDefault(0)));
            //}
            get
            {
                return _btCreateNewByOldIsEnabled && hasDiag;
            }
            set
            {
                if (_btCreateNewByOldIsEnabled == value)
                {
                    return;
                }
                _btCreateNewByOldIsEnabled = value;
                NotifyOfPropertyChange(() => btCreateNewByOldIsEnabled);
            }
        }
        private bool _btEditIsEnabled;
        public bool btEditIsEnabled
        {
            get
            {
                return _btEditIsEnabled;
            }
            set
            {
                _btEditIsEnabled = value;
                NotifyOfPropertyChange(() => btEditIsEnabled);
            }
        }

        private bool _btCancelIsEnabled = true;
        public bool btCancelIsEnabled
        {
            get
            {
                return _btCancelIsEnabled;
            }
            set
            {
                _btCancelIsEnabled = value;
                NotifyOfPropertyChange(() => btCancelIsEnabled);
            }
        }

        /*TMA*/
        private bool _btGCTIsEnabled = true;
        public bool btGCTIsEnabled
        {
            get
            {
                return _btGCTIsEnabled && !IsUpdateFromPresciption;
            }
            set
            {
                _btGCTIsEnabled = value;
                NotifyOfPropertyChange(() => btGCTIsEnabled);
            }
        }

        private bool _btGCT_CLS_IsEnabled = true;
        public bool btGCT_CLS_IsEnabled
        {
            get
            {
                return _btGCT_CLS_IsEnabled && !IsUpdateFromPresciption;
            }
            set
            {
                _btGCT_CLS_IsEnabled = value;
                NotifyOfPropertyChange(() => btGCT_CLS_IsEnabled);
            }
        }
        /*TMA*/

        private bool _btUpdateIsEnabled = false;
        public bool btUpdateIsEnabled
        {
            get
            {
                return _btUpdateIsEnabled;
            }
            set
            {
                _btUpdateIsEnabled = value;
                NotifyOfPropertyChange(() => btUpdateIsEnabled);
            }
        }


        #endregion

        public void authorization()
        {
            if (!Globals.isAccountCheck)
            {
                return;
            }
            mChanDoan_tabSuaKhamBenh_ThongTin = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConsultation
                                               , (int)eConsultation.mPtPMRConsultationNew,
                                               (int)oConsultationEx.mChanDoan_tabSuaKhamBenh_ThongTin, (int)ePermission.mView);
            mChanDoan_tabSuaKhamBenh_HieuChinh = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConsultation
                                                   , (int)eConsultation.mPtPMRConsultationNew,
                                                   (int)oConsultationEx.mChanDoan_tabSuaKhamBenh_HieuChinh, (int)ePermission.mView);
            mChanDoan_ChiDinhXetNghiemCLS = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConsultation
                                                   , (int)eConsultation.mPtPMRConsultationNew,
                                                   (int)oConsultationEx.mChanDoan_ChiDinhXetNghiemCLS, (int)ePermission.mView);
            mChanDoan_RaToa = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConsultation
                                                   , (int)eConsultation.mPtPMRConsultationNew,
                                                   (int)oConsultationEx.mChanDoan_RaToa, (int)ePermission.mView);
            mChanDoan_TaoBenhAn = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConsultation
                                                   , (int)eConsultation.mPtPMRConsultationNew,
                                                   (int)oConsultationEx.mChanDoan_TaoBenhAn, (int)ePermission.mView);
            mChanDoan_XemKetQuaCLS = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConsultation
                                                   , (int)eConsultation.mPtPMRConsultationNew,
                                                   (int)oConsultationEx.mChanDoan_XemKetQuaCLS, (int)ePermission.mView);
            mChanDoan_XemToaThuoc_HienHanh = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConsultation
                                                   , (int)eConsultation.mPtPMRConsultationNew,
                                                   (int)oConsultationEx.mChanDoan_XemToaThuoc_HienHanh, (int)ePermission.mView);
            mChanDoan_XemBenhAn = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConsultation
                                                   , (int)eConsultation.mPtPMRConsultationNew,
                                                   (int)oConsultationEx.mChanDoan_XemBenhAn, (int)ePermission.mView);
        }

        #region account checking

        private bool _mChanDoan_tabSuaKhamBenh_ThongTin = true;
        private bool _mChanDoan_tabSuaKhamBenh_HieuChinh = true;
        private bool _mChanDoan_ChiDinhXetNghiemCLS = true;
        private bool _mChanDoan_RaToa = true;
        private bool _mChanDoan_TaoBenhAn = true;
        private bool _mChanDoan_XemKetQuaCLS = true;
        private bool _mChanDoan_XemToaThuoc_HienHanh = true;
        private bool _mChanDoan_XemBenhAn = true;

        public bool mChanDoan_tabSuaKhamBenh_ThongTin
        {
            get
            {
                return _mChanDoan_tabSuaKhamBenh_ThongTin;
            }
            set
            {
                if (_mChanDoan_tabSuaKhamBenh_ThongTin == value)
                    return;
                _mChanDoan_tabSuaKhamBenh_ThongTin = value;
            }
        }
        public bool mChanDoan_tabSuaKhamBenh_HieuChinh
        {
            get
            {
                return _mChanDoan_tabSuaKhamBenh_HieuChinh;
            }
            set
            {
                if (_mChanDoan_tabSuaKhamBenh_HieuChinh == value)
                    return;
                _mChanDoan_tabSuaKhamBenh_HieuChinh = value;
            }
        }
        public bool mChanDoan_ChiDinhXetNghiemCLS
        {
            get
            {
                return _mChanDoan_ChiDinhXetNghiemCLS;
            }
            set
            {
                if (_mChanDoan_ChiDinhXetNghiemCLS == value)
                    return;
                _mChanDoan_ChiDinhXetNghiemCLS = value;
            }
        }
        public bool mChanDoan_RaToa
        {
            get
            {
                return _mChanDoan_RaToa;
            }
            set
            {
                if (_mChanDoan_RaToa == value)
                    return;
                _mChanDoan_RaToa = value;
            }
        }
        public bool mChanDoan_TaoBenhAn
        {
            get
            {
                return _mChanDoan_TaoBenhAn;
            }
            set
            {
                if (_mChanDoan_TaoBenhAn == value)
                    return;
                _mChanDoan_TaoBenhAn = value;
            }
        }
        public bool mChanDoan_XemKetQuaCLS
        {
            get
            {
                return _mChanDoan_XemKetQuaCLS;
            }
            set
            {
                if (_mChanDoan_XemKetQuaCLS == value)
                    return;
                _mChanDoan_XemKetQuaCLS = value;
            }
        }
        public bool mChanDoan_XemToaThuoc_HienHanh
        {
            get
            {
                return _mChanDoan_XemToaThuoc_HienHanh;
            }
            set
            {
                if (_mChanDoan_XemToaThuoc_HienHanh == value)
                    return;
                _mChanDoan_XemToaThuoc_HienHanh = value;
            }
        }
        public bool mChanDoan_XemBenhAn
        {
            get
            {
                return _mChanDoan_XemBenhAn;
            }
            set
            {
                if (_mChanDoan_XemBenhAn == value)
                    return;
                _mChanDoan_XemBenhAn = value;
            }
        }
        #endregion


        private void CopyListICD10()
        {
            if (refIDC10List != null)
            {
                refIDC10ListCopy = refIDC10List.DeepCopy();
            }
            else
            {
                refIDC10ListCopy = null;
            }
            AddBlankRow();
        }

        private void CopyListICD10ForNew()
        {
            if (refIDC10List != null)
            {
                refIDC10ListCopy = refIDC10List.DeepCopy();
            }
            else
            {
                refIDC10ListCopy = null;
            }
            //refIDC10List = refIDC10ListLatestCopy.DeepCopy();
            refIDC10List = new ObservableCollection<DiagnosisIcd10Items>();
            AddBlankRow();

        }

        private bool NeedICD10()
        {
            //if (Globals.ConfigList != null && Convert.ToInt16(Globals.ConfigList[(int)AppConfigKeys.ConfigItemKey.NeedICD10]) > 0)

            // Txd 25/05/2014 Replaced ConfigList
            if (Globals.ServerConfigSection.Hospitals.NeedICD10 > 0)
            {
                if (refIDC10List != null)
                {
                    var temp = refIDC10List.Where(x => x.DiseasesReference != null);
                    if (temp == null || temp.Count() == 0)
                    {
                        MessageBox.Show(eHCMSResources.A0199_G1_Msg_YCNhapICD10);
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    return true;
                }
            }
            else
            { return true; }

        }

        private void UpdateDoctorStaffID()
        {
            DiagTrmtItem.DoctorStaffID = Globals.LoggedUserAccount.StaffID.Value;
            DiagTrmtItem.ObjDoctorStaffID.StaffID = Globals.LoggedUserAccount.StaffID.Value;
        }

        private bool _IsNotExistsDiagnosisTreatmentByPtRegDetailID = true;
        public bool IsNotExistsDiagnosisTreatmentByPtRegDetailID
        {
            get { return _IsNotExistsDiagnosisTreatmentByPtRegDetailID; }
            set
            {
                _IsNotExistsDiagnosisTreatmentByPtRegDetailID = value;
                NotifyOfPropertyChange(() => btCreateNewIsEnabled);
                NotifyOfPropertyChange(() => btCreateNewByOldIsEnabled);
            }
        }
        private void CheckDiagnosisTreatmentExists_PtRegDetailID(long patientID, long PtRegDetailID)
        {
            this.ShowBusyIndicator();
            //IsWaitingGetBlankDiagnosisTreatmentByPtID = true;

            var t = new Thread(() =>
            {
                using (var serviceFactory = new ePMRsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginCheckDiagnosisTreatmentExists_PtRegDetailID(patientID, PtRegDetailID, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            IsNotExistsDiagnosisTreatmentByPtRegDetailID = contract.EndCheckDiagnosisTreatmentExists_PtRegDetailID(asyncResult);
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            //IsWaitingGetBlankDiagnosisTreatmentByPtID = false;
                            this.HideBusyIndicator();
                        }

                    }), null);

                }

            });

            t.Start();
        }


        public void GetLatesDiagTrmtByPtID(long patientID)
        {
            this.ShowBusyIndicator();

            //IsWaitingGetLatestDiagnosisTreatmentByPtID = true;

            var t = new Thread(() =>
            {
                using (var serviceFactory = new ePMRsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginGetLatestDiagnosisTreatmentByPtID(patientID, null, "", 0, true, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            DiagTrmtItem = contract.EndGetLatestDiagnosisTreatmentByPtID(asyncResult);
                            NotifyOfPropertyChange(() => btCreateNewByOldIsEnabled);
                            //▼===== #017: Set ở đây cho các trường hợp chưa có chẩn đoán lần nào hết thì cũng lấy lại đc ItemsSource cho Hướng điều trị.
                            GetAllTreatmentByType();
                            //▲===== 
                            //20190713 TBL: Khi cau hinh la phong mach BS Huan thi mac dinh cach dieu tri la Cap toa cho ve
                            if (Globals.ServerConfigSection.CommonItems.OrganizationUseSoftware == 1 && RefTreatment.Count > 0 && DiagTrmtItem.V_TreatmentType == 0)
                            {
                                DiagTrmtItem.V_TreatmentType = RefTreatment.FirstOrDefault().LookupID;
                            }
                            if (DiagTrmtItem != null && DiagTrmtItem.DTItemID > 0)
                            {
                                //Có DiagnosisTreatment rồi
                                //FormEditorIsEnabled = false;
                                if (DiagTrmtItem.V_RegistrationType == AllLookupValues.RegistrationType.NOI_TRU && DiagTrmtItem.DTItemID > 0)
                                    DiagnosisIcd10Items_Load_InPt(DiagTrmtItem.DTItemID);
                                else
                                    DiagnosisIcd10Items_Load(DiagTrmtItem.ServiceRecID, null, false);
                                hasDiag = true;
                                ButtonForHasDiag();
                            }
                            else
                            {

                                hasDiag = false;
                                //Form Trạng Thái New

                                DiagnosisIcd10Items_Load(null, Registration_DataStorage.CurrentPatient.PatientID, true);

                                //KMx: Không cần gọi về server, chỉ cần sử dụng hàm ResetDefaultForBehaving() trong ruột GetBlankDiagnosisTreatmentByPtID() (01/11/2014 10:16).
                                //GetBlankDiagnosisTreatmentByPtID(Registration_DataStorage.CurrentPatient.PatientID);     
                                ResetDefaultForBehaving();

                                //FormEditorIsEnabled = true;
                                //ButtonForNotDiag(false);
                                StateNewWaiting();
                            }
                            DiagTrmtItemCopy = ObjectCopier.DeepCopy(DiagTrmtItem);
                            DiagTrmtItemCopy.IsDataChanged = false;
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            //Globals.IsBusy = false;
                            //IsWaitingGetLatestDiagnosisTreatmentByPtID = false;
                            this.HideBusyIndicator();
                        }
                    }), null);
                }
            });
            t.Start();
        }
        /*▼====: #012*/
        private ObservableCollection<Lookup> _RefTreatment;
        public ObservableCollection<Lookup> RefTreatment
        {
            get { return _RefTreatment; }
            set
            {
                if (_RefTreatment != value)
                {
                    _RefTreatment = value;
                    NotifyOfPropertyChange(() => RefTreatment);
                }
            }
        }
        //▼===== #017
        public void GetAllTreatmentByType()
        {
            //▼===== Đổi lại vị trí lấy RefTreatment với set dữ liệu cho tmpkscSelectedItem vì cần phải set lại Source trước khi đọc dữ liệu để đưa vào 
            //       tmpkscSelectedItem vì nếu không lấy từ DiagTrmtItem sẽ bị xót khi bệnh nhân 2 là cấp toa cho về, mà bệnh nhân 1 đã nhập SearchKey là NV
            //       dẫn ItemsSource không có cấp toa nữa => Không hiển thị lại đc lastest V_TreatmentType của đăng ký => Đăng ký mà không cần Hướng điều trị.
            RefTreatment = new ObservableCollection<Lookup>();
            foreach (var tmpLookup in Globals.AllLookupValueList)
            {
                if (tmpLookup.ObjectTypeID == (long)(LookupValues.V_TreatmentType))
                {
                    RefTreatment.Add(tmpLookup);
                }
            }

            object tmpkscSelectedItem = null;
            if (DiagTrmtItem != null)
            {
                if (kscV_TreatmentType != null && DiagTrmtItem.V_TreatmentType > 0)
                {
                    foreach (var tmpRefTreatment in RefTreatment)
                    {
                        if (tmpRefTreatment.LookupID == DiagTrmtItem.V_TreatmentType)
                        {
                            tmpkscSelectedItem = tmpRefTreatment;
                        }
                    }
                }
                else
                {
                    kscV_TreatmentType.ItemsSource = RefTreatment;
                }
            }

            //▼===== 20191810 TTM:  Chỗ này phải set lại ItemsSource 
            //                      Lý do: Khi gõ tay vào trong Trường hướng điều trị, KeySearchableComboBox đã giữ ItemsSource bằng việc hướng nó về 1
            //                             địa chỉ khác => Cho dù có set lại bình thường ở trên hay NotifyOfPropertyChange thì biding ItemsSource của xaml
            //                             cũng không hiểu. Cho nên phải set về lại thì nó mới hiểu.
            if (tmpkscSelectedItem != null)
            {
                kscV_TreatmentType.ItemsSource = RefTreatment;
                kscV_TreatmentType.SelectedItem = tmpkscSelectedItem;
            }
            //▲===== 
        }
        //▲===== #017

        /*▲====: #012*/
        //KMx: Sau khi kiểm tra, thấy hàm này không cần thiết (11/01/2014 08:35).
        //public void GetBlankDiagnosisTreatmentByPtID(long patientID)
        //{
        //    IsWaitingGetBlankDiagnosisTreatmentByPtID = true;

        //    var t = new Thread(() =>
        //    {

        //        using (var serviceFactory = new ePMRsServiceClient())
        //        {
        //            var contract = serviceFactory.ServiceInstance;

        //            contract.BeginGetBlankDiagnosisTreatmentByPtID(patientID, null, "", Globals.DispatchCallback((asyncResult) =>
        //            {
        //                try
        //                {
        //                    DiagTrmtItem = contract.EndGetBlankDiagnosisTreatmentByPtID(asyncResult);

        //                    //bangct01: chỗ này chưa hiểu ý nghĩa! chỉ biết bị lỗi null object thì ràng lại
        //                    if (DiagTrmtItem.PatientServiceRecord != null)
        //                    {
        //                        DiagTrmtItem.PatientServiceRecord.PtRegistrationID = Registration_DataStorage.CurrentPatientRegistration.PtRegistrationID;

        //                        ResetDefaultForBehaving();
        //                    }

        //                    //DiagTrmtItemCopy = DiagTrmtItem.DeepCopy();
        //                }
        //                catch (Exception ex)
        //                {
        //                    MessageBox.Show(ex.Message);
        //                }
        //                finally
        //                {
        //                    //Globals.IsBusy = false;
        //                    //IsLoading = false;
        //                    IsWaitingGetBlankDiagnosisTreatmentByPtID = false;
        //                }

        //            }), null);

        //        }

        //    });

        //    t.Start();
        //}

        private void ResetDefaultForBehaving()
        {
            if (DiagTrmtItem == null)
            {
                return;
            }

            //KMx: Vì không xài hàm GetBlankDiagnosisTreatmentByPtID() nên phải new, nếu không sẽ bị lỗi khi lưu chẩn đoán (01/11/2014 10:29).
            if (DiagTrmtItem.PatientServiceRecord == null)
            {
                DiagTrmtItem.PatientServiceRecord = new PatientServiceRecord();
                DiagTrmtItem.PatientServiceRecord.ExamDate = Globals.GetCurServerDateTime();
            }

            if (DiagTrmtItem.PatientServiceRecord.PatientMedicalRecord == null)
            {
                DiagTrmtItem.PatientServiceRecord.PatientMedicalRecord = new PatientMedicalRecord();
            }

            if (RefBehaving != null)
            {
                DiagTrmtItem.PatientServiceRecord.V_Behaving = RefBehaving.FirstOrDefault().LookupID;
            }
            else
            {
                DiagTrmtItem.PatientServiceRecord.V_Behaving = (long)AllLookupValues.Behaving.KHAM_DIEU_TRI;
            }

            if (RefDiagnosis != null && Registration_DataStorage.CurrentPatientRegistration != null && Registration_DataStorage.CurrentPatientRegistration.V_RegistrationType == AllLookupValues.RegistrationType.NOI_TRU)
            {
                if (RefDiagnosis.Any(x => x.LookupID == (long)AllLookupValues.V_DiagnosisType.DIAGNOSIS_OUTHOS))
                {
                    DiagTrmtItem.PatientServiceRecord.V_DiagnosisType = (long)AllLookupValues.V_DiagnosisType.DIAGNOSIS_OUTHOS;
                }
                else
                {
                    DiagTrmtItem.PatientServiceRecord.V_DiagnosisType = RefDiagnosis.FirstOrDefault().LookupID;
                }
            }
            else
            {
                DiagTrmtItem.PatientServiceRecord.V_DiagnosisType = (long)AllLookupValues.V_DiagnosisType.DIAGNOSIS_NORMAL;
            }

            if (RefMedRecTemplate != null)
            {
                DiagTrmtItem.MDRptTemplateID = RefMedRecTemplate.FirstOrDefault().MDRptTemplateID;
            }
            else
            {
                DiagTrmtItem.MDRptTemplateID = 1;
            }
        }

        private IEnumerator<IResult> AllCheck()
        {
            if (DiagTrmtItem == null)
            {
                yield break;
            }
            if (Registration_DataStorage.CurrentPatientRegistration != null && Globals.IsLockRegistration(Registration_DataStorage.CurrentPatientRegistration.RegLockFlag, eHCMSResources.Z0407_G1_CNhatCDoan))
            {
                yield break;
            }
            // da kiem tra trong ham CheckEmptyField roi, khong can them dong kiem tra nay nua                
            //if (!string.IsNullOrWhiteSpace(DiagTrmtItem.DiagnosisFinal) || !string.IsNullOrWhiteSpace(DiagTrmtItem.Diagnosis))
            if (!CheckEmptyFields())
            {
                yield break;
            }
            if (NeedICD10() && CheckedIsMain())
            {
                //UpdateDoctorStaffID();
                DiagTrmtItem.ICD10List = String.Join(",", from item in refIDC10List
                                                          where (!string.IsNullOrEmpty(item.ICD10Code) && item.DiseasesReference != null)
                                                          select item.ICD10Code);

                //KMx: Phải set loại đăng ký để service gọi stored update tương ứng (01/11/2014 10:30).
                DiagTrmtItem.PatientServiceRecord.V_RegistrationType = Registration_DataStorage.CurrentPatientRegistration.V_RegistrationType;
                DiagTrmtItem.PatientServiceRecord.V_DiagnosisType = (long)AllLookupValues.V_DiagnosisType.DIAGNOSIS_NORMAL;

                //DiagTrmtItem.PatientServiceRecord.PatientMedicalRecord.PatientID = Registration_DataStorage.CurrentPatient.PatientID;
                PatientRegistration CurRegistration = Registration_DataStorage.CurrentPatientRegistration;

                // Hpt 22/09/2015: Nếu bệnh nhân có quyền lợi về BHYT thì mới kiểm tra xem trong danh sách có ICD10 nào là Z10 hay không vì BHYT sẽ không chi trả cho những trường hợp này
                if (CurRegistration.PtInsuranceBenefit != null && CurRegistration.PtInsuranceBenefit > 0 && refIDC10List != null
                    && refIDC10List.Any(x => x.ICD10Code != null && x.DiseasesReference != null && x.DiseasesReference.ICD10Code.Contains("Z10")))
                {
                    warningtask = new MessageWarningShowDialogTask(content, eHCMSResources.Z0339_G1_TiepTucLuuCDoan);
                    yield return warningtask;
                    if (!warningtask.IsAccept)
                    {
                        yield break;
                    }
                }
                StateEditWaiting();
                UpdateDiagTrmt();
            }
        }


        public void UpdateDiagTrmt()
        {
            this.ShowBusyIndicator();

            //KMx: Lưu BS cập nhật sau cùng (28/03/2014 13:59).
            if (!IsUpdateFromPresciption)
            {
                DiagTrmtItem.DoctorStaffID = Globals.LoggedUserAccount.StaffID.Value;
            }
            IsWaitingUpdate = true;

            long listID = Compare2Object();

            var t = new Thread(() =>
            {
                using (var serviceFactory = new ePMRsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    DiagTrmtItem.DeptLocationID = Globals.DeptLocation.DeptLocationID;

                    contract.BeginUpdateDiagnosisTreatment(DiagTrmtItem, listID, refIDC10List, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            if (contract.EndUpdateDiagnosisTreatment(asyncResult))
                            {
                                FormEditorIsEnabled = false;

                                StateEdit();

                                if (refIDC10List != null)
                                {
                                    refIDC10List = refIDC10List.Where(x => x.DiseasesReference != null).ToObservableCollection();
                                }

                                //phat su kien reload lai danh sach 
                                //Globals.EventAggregator.Publish(new ReloadDataConsultationEvent { });

                                //KMx: Sau khi lưu chẩn đoán, reload Service Record (22/05/2014 09:48).
                                if (!IsUpdateFromPresciption)
                                {
                                    IConsultationModule consultVM = Globals.GetViewModel<IConsultationModule>();

                                    consultVM.PatientServiceRecordsGetForKhamBenh_Ext();

                                    //KMx: Sau khi lưu chẩn đoán, xóa danh sách chẩn đoán. Vì danh sách không tự động load lại.
                                    //Nếu không xóa thì chẩn đoán vừa lưu sẽ khác với chẩn đoán trong danh sách (22/05/2014 09:48).
                                    Globals.EventAggregator.Publish(new ClearAllDiagnosisListAfterUpdateEvent());
                                }
                                //Nếu đang là Popup thì phát event lấy cđ này gán vào khám bệnh
                                if (Globals.ConsultationIsChildWindow || IsUpdateFromPresciption)
                                {
                                    Globals.EventAggregator.Publish(new DiagnosisTreatmentSelectedEvent<DiagnosisTreatment> { DiagnosisTreatment = DiagTrmtItem.DeepCopy() });
                                }

                                MessageBox.Show(eHCMSResources.K2782_G1_DaCNhat);
                            }
                            else
                            {
                                if (DiagTrmtItem.PatientServiceRecord.V_DiagnosisType == (long)AllLookupValues.V_DiagnosisType.DIAGNOSIS_IN)
                                {
                                    MessageBox.Show(eHCMSResources.Z0403_G1_DaCoCDoanNV, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                }
                                else if (DiagTrmtItem.PatientServiceRecord.V_DiagnosisType == (long)AllLookupValues.V_DiagnosisType.DIAGNOSIS_OUTHOS)
                                {
                                    MessageBox.Show(eHCMSResources.Z0404_G1_DaCoCDoanXV, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                }
                                else
                                {
                                    MessageBox.Show(eHCMSResources.A0269_G1_Msg_InfoCNhatCDFail, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                        }
                        finally
                        {
                            //Globals.IsBusy = false;
                            //IsWaitingUpdate = false;
                            this.HideBusyIndicator();
                            if (IsUpdateFromPresciption)
                            {
                                TryClose();
                            }
                        }
                    }), null);
                }
            });
            t.Start();
        }

        //private ObservableCollection<DiagnosisIcd10Items> refIDC10ListLatestCopy;

        public void Handle(CommonClosedPhysicalForDiagnosisEvent message)
        {
            long tmpPtRegistrationID = 0;
            long tmpV_RegistrationType = 0;
            if (Registration_DataStorage.CurrentPatientRegistration != null)
            {
                tmpPtRegistrationID = Registration_DataStorage.CurrentPatientRegistration.PtRegistrationID;
                tmpV_RegistrationType = (long)Registration_DataStorage.CurrentPatientRegistration.V_RegistrationType;
            }
            GetPhyExam_ByPtRegID(tmpPtRegistrationID, tmpV_RegistrationType);
            //InitPhyExam(Registration_DataStorage.CurrentPatient.PatientID);
        }

        //private void InitPhyExam(long patientID)
        //{
        //    //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });

        //    var t = new Thread(() =>
        //    {
        //        using (var serviceFactory = new SummaryServiceClient())
        //        {
        //            var contract = serviceFactory.ServiceInstance;
        //            contract.BeginGetLastPhyExamByPtID(patientID, Globals.DispatchCallback((asyncResult) =>
        //            {

        //                try
        //                {
        //                    PtPhyExamItem = contract.EndGetLastPhyExamByPtID(asyncResult);
        //                    //KMx: Sau khi lấy PhysicalExamination thì phải gán lại vào Globals.
        //                    //Nếu không, khi chuyển qua trang thông tin chung, hoặc Ra toa thì vẫn còn hiển thị cái cũ (16/06/2014 10:55).
        //                    Globals.curPhysicalExamination = PtPhyExamItem;
        //                    //if(PtPhyExamItem==null)
        //                    //    PtPhyExamItem=new PhysicalExamination();

        //                }
        //                catch (Exception ex)
        //                {
        //                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
        //                }
        //                finally
        //                {
        //                    //Globals.IsBusy = false;

        //                }

        //            }), null);

        //        }

        //    });
        //    t.Start();
        //}
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

        private void DiagnosisIcd10Items_Load(long? ServiceRecID, long? PatientID, bool Last)
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            this.ShowBusyIndicator();
            //IsWaitingLoadICD10 = true;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new ePMRsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetDiagnosisIcd10Items_Load(ServiceRecID, PatientID, Last, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndGetDiagnosisIcd10Items_Load(asyncResult);
                            refIDC10List = results.ToObservableCollection();
                            refIDC10ListCopy = refIDC10List.DeepCopy();
                            //refIDC10ListLatestCopy = refIDC10List.DeepCopy();
                            if (ServiceRecID == null)/*Chưa có chẩn đoán nào*/
                            {
                                CopyListICD10();
                            }
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            //Globals.IsBusy = false;
                            //IsWaitingLoadICD10 = false;
                            this.HideBusyIndicator();
                        }
                    }), null);
                }
            });
            t.Start();
        }
        private void DiagnosisIcd10Items_Load_InPt(long DTItemID)
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                using (var serviceFactory = new ePMRsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetDiagnosisIcd10Items_Load_InPt(DTItemID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndGetDiagnosisIcd10Items_Load_InPt(asyncResult);
                            refIDC10List = results.ToObservableCollection();
                            refIDC10ListCopy = refIDC10List.DeepCopy();
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

        #region Event Double click từ ds chẩn đoán
        public void Handle(ConsultationDoubleClickEvent obj)
        {
            btCancel();
            //Globals.ClearPatientAllDetails();
            if (obj.DiagTrmtItem != null)
            {
                DiagTrmtItem = ObjectCopier.DeepCopy(obj.DiagTrmtItem);
                if (ConsultState != ConsultationState.NewConsultationState)
                {
                    ValidateExpiredDiagnosicTreatment(DiagTrmtItem);
                }

                refIDC10List = obj.refIDC10List;
                //ButtonForHasDiag();

                //Thủ tục hàm Edit
                DiagTrmtItemCopy = ObjectCopier.DeepCopy(DiagTrmtItem);
                refIDC10ListCopy = ObjectCopier.DeepCopy(refIDC10List);
                //Thủ tục hàm Edit
            }
        }
        #endregion

        #region List ICD10 member
        //▼====== #005: Nếu như màn hình này được gọi bởi left or top menu => CS_DS = null => vẫn theo lối cũ và ngược lại
        private ObservableCollection<DiagnosisIcd10Items> _NewrefIDC10List;
        public ObservableCollection<DiagnosisIcd10Items> refIDC10List
        {
            get
            {
                if (CS_DS != null)
                {
                    return CS_DS.refIDC10List;
                }
                return _NewrefIDC10List;
            }
            set
            {
                if (CS_DS != null)
                {
                    if (CS_DS.refIDC10List != value)
                    {
                        CS_DS.refIDC10List = value;
                    }
                }
                else if (_NewrefIDC10List != value)
                {
                    _NewrefIDC10List = value;
                }
                NotifyOfPropertyChange(() => refIDC10List);
            }
        }
        //▲====== #005
        private DiagnosisIcd10Items _refIDC10Item;
        public DiagnosisIcd10Items refIDC10Item
        {
            get
            {
                return _refIDC10Item;
            }
            set
            {
                if (_refIDC10Item != value)
                {
                    _refIDC10Item = value;
                    /*▼====: #003*/
                    if (_refIDC10Item != null)
                    {
                        refIDC10Item.IsObjectBeingUsedByClient = true;
                    }
                    /*▲====: #003*/
                }
                NotifyOfPropertyChange(() => refIDC10Item);
            }
        }

        //private PagedSortableCollectionView<DiseasesReference> _refIDC10;
        private ObservableCollection<DiseasesReference> _refIDC10Code;
        //public PagedSortableCollectionView<DiseasesReference> refIDC10
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

        private ObservableCollection<DiseasesReference> _refIDC10Name;
        
        public ObservableCollection<DiseasesReference> refIDC10Name
        {
            get
            {
                return _refIDC10Name;
            }
            set
            {
                if (_refIDC10Name != value)
                {
                    _refIDC10Name = value;
                }
                NotifyOfPropertyChange(() => refIDC10Name);
            }
        }

        public void LoadRefDiseases(string name, byte type, int PageIndex, int PageSize)
        {
            System.Diagnostics.Debug.WriteLine("======> LoadRefDiseases <<<<=======");
            var t = new Thread(() =>
            {
                using (var serviceFactory = new CommonUtilsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginSearchRefDiseases(name, PageIndex, PageSize, type
                        , Registration_DataStorage.CurrentPatient.PatientID
                        , Registration_DataStorage.CurrentPatientRegistrationDetail.PaidTime ?? Globals.GetCurServerDateTime()
                        , Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            int Total = 10;
                            var results = contract.EndSearchRefDiseases(out Total, asyncResult);

                            if (type == 0)
                            {
                                refIDC10Code.Clear();                                
                                if (results != null)
                                {
                                    foreach (DiseasesReference p in results)
                                    {
                                        refIDC10Code.Add(p);
                                    }
                                }
                                if (refIDC10Code.Count > 0)
                                {
                                    this.grdConsultation.bIcd10CodeAcbPopulated = true;
                                }
                                Acb_ICD10_Code.ItemsSource = refIDC10Code;
                                Acb_ICD10_Code.PopulateComplete();
                            }
                            else
                            {
                                refIDC10Name.Clear();
                                if (results != null)
                                {
                                    foreach (DiseasesReference p in results)
                                    {
                                        refIDC10Name.Add(p);
                                    }
                                }
                                if (refIDC10Code.Count > 0)
                                {
                                    this.grdConsultation.bIcd10CodeAcbPopulated = true;
                                }
                                Acb_ICD10_Name.ItemsSource = refIDC10Name;
                                Acb_ICD10_Name.PopulateComplete();
                            }

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

        AutoCompleteBox Acb_ICD10_Code = null;

        AutoCompleteBox Acb_ICD10_Name = null;

        private byte Type = 0;
        public void AcbICD10Code_Loaded(object sender, RoutedEventArgs e)
        {
            Acb_ICD10_Code = (AutoCompleteBox)sender;
        }
        public void AcbICD10Name_Loaded(object sender, RoutedEventArgs e)
        {
            Acb_ICD10_Name = (AutoCompleteBox)sender;
        }
        private string _typedText;
        public string TypedText
        {
            get { return _typedText; }
            set
            {
                _typedText = value.ToUpper();
                NotifyOfPropertyChange(() => TypedText);
            }
        }

        public void grdConsultation_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (grdConsultation != null && grdConsultation.SelectedItem != null)
            {
                grdConsultation.BeginEdit();
            }
        }

        public void aucICD10_Populating(object sender, PopulatingEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("======> aucICD10_Populating OUT <<<<=======");
            if (IsCode)
            {
                System.Diagnostics.Debug.WriteLine("======> aucICD10_Populating IN <<<<=======");
                e.Cancel = true;
                                
                Type = 0;
                //refIDC10.PageIndex = 0;
                //LoadRefDiseases(e.Parameter, 0, 0, refIDC10.PageSize);
                LoadRefDiseases(e.Parameter, 0, 0, 100);
            }
        }

        
        public void aucICD10Name_Populating(object sender, PopulatingEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("======> aucICD10Name_Populating OUT <<<<=======");
            if (!IsCode && ColumnIndex == 1)
            {
                System.Diagnostics.Debug.WriteLine("======> aucICD10Name_Populating IN <<<<=======");
                e.Cancel = true;                                
                Type = 1;                
                LoadRefDiseases(e.Parameter, 1, 0, 100);                
            }
        }

        public void AutoCompleteBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (IsCode)
            {                
                if (refIDC10Item != null && Acb_ICD10_Code != null)
                {
                    refIDC10Item.DiseasesReference = new DiseasesReference();

                    refIDC10Item.DiseasesReference = Acb_ICD10_Code.SelectedItem as DiseasesReference;
                }
            }
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
            
            if (refIDC10Item != null && Acb_ICD10_Code != null)
            {                
                refIDC10Item.DiseasesReference = new DiseasesReference();
                refIDC10Item.DiseasesReference = Acb_ICD10_Code.SelectedItem as DiseasesReference;
                if (CheckCountIsMain())
                {
                    refIDC10Item.IsMain = true;
                }
                if (CheckExists(refIDC10Item))
                {
                    GetDiagTreatmentFinal(refIDC10Item.DiseasesReference);
                }
            }
        }

        private bool isDiseaseDropDown = false;
        public void DiseaseName_DropDownClosing(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            System.Diagnostics.Debug.WriteLine(" <====== DiseaseName_DropDownClosing =====>");
            isDiseaseDropDown = true;
        }

        public void DiseaseName_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            if (!isDiseaseDropDown)
            {
                return;
            }
            System.Diagnostics.Debug.WriteLine(" <====== DiseaseName_DropDownClosed =====>");
            isDiseaseDropDown = false;
            
            refIDC10Item.DiseasesReference = ((AutoCompleteBox)sender).SelectedItem as DiseasesReference;
            if (CheckCountIsMain())
            {
                refIDC10Item.IsMain = true;
            }
            if (CheckExists(refIDC10Item))
            {
                GetDiagTreatmentFinal(refIDC10Item.DiseasesReference);
            }
        }
        public void AutoName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!IsCode)
            {                
                if (refIDC10Item != null)
                {
                    refIDC10Item.DiseasesReference = Acb_ICD10_Name.SelectedItem as DiseasesReference;
                }
            }
        }
        public void AutoName_DropDownClosing(object sender, RoutedPropertyChangingEventArgs<bool> e)
        {
            if (!IsCode)
            {                
                if (refIDC10Item != null)
                {
                    refIDC10Item.DiseasesReference = Acb_ICD10_Name.SelectedItem as DiseasesReference;
                }
            }
        }


        private void AddBlankRow()
        {
            if (refIDC10List != null
                && refIDC10List.LastOrDefault() != null
                && refIDC10List.LastOrDefault().DiseasesReference == null)
            {
                return;
            }
            DiagnosisIcd10Items ite = new DiagnosisIcd10Items();
            ite.V_DiagIcdStatus = (long)AllLookupValues.V_DiagIcdStatus.DANGDIEUTRI;
            ite.LookupStatus = new Lookup();
            ite.LookupStatus.LookupID = (long)AllLookupValues.V_DiagIcdStatus.DANGDIEUTRI;
            ite.LookupStatus.ObjectValue = eHCMSResources.Z0540_G1_DangDTri.ToUpper();
            refIDC10List.Add(ite);
        }

        private bool CheckExists(DiagnosisIcd10Items Item, bool HasMessage = true)
        {
            int i = 0;
            if (Item.DiseasesReference == null)
            {
                return true;
            }
            foreach (DiagnosisIcd10Items p in refIDC10List)
            {
                if (p.DiseasesReference != null)
                {
                    if (Item.DiseasesReference.ICD10Code == p.DiseasesReference.ICD10Code)
                    {
                        i++;
                    }
                }
            }
            if (i > 1)
            {
                Item.DiseasesReference = null;
                if (HasMessage)
                {
                    MessageBox.Show(eHCMSResources.A0810_G1_Msg_InfoMaICDDaTonTai);
                }
                return false;
            }
            else
            {
                return true;
            }
        }

        private DiseasesReference DiseasesReferenceCopy = null;

        bool IsCode = true;
        int ColumnIndex = 0;
        public void AxDataGridNyICD10_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DiagnosisIcd10Items item = ((DataGrid)sender).SelectedItem as DiagnosisIcd10Items;
            if (item != null && item.DiseasesReference != null)
            {                
                DiseasesReferenceCopy = item.DiseasesReference;
                DiagnosisFinalNew = DiagnosisFinalOld = ObjectCopier.DeepCopy(item.DiseasesReference.DiseaseNameVN);
                DiseasesReferenceCopy = ObjectCopier.DeepCopy(item.DiseasesReference);
            }
            else
            {
                DiagnosisFinalNew = DiagnosisFinalOld = "";
                DiseasesReferenceCopy = null;
            }
        }
        public void AxDataGridNy_PreparingCellForEdit(object sender, DataGridPreparingCellForEditEventArgs e)
        {
            ColumnIndex = e.Column.DisplayIndex;

            if (refIDC10Item != null)
            {
                DiseasesReferenceCopy = refIDC10Item.DiseasesReference.DeepCopy();
            }
            if (e.Column.DisplayIndex == 0)
            {
                IsCode = true;
            }
            else
            {
                IsCode = false;
            }
        }

        public void GetDiagTreatmentFinal(DiseasesReference diseasesReference)
        {
            if (diseasesReference != null)
            {
                DiagnosisFinalNew = diseasesReference.DiseaseNameVN;
                if (DiagnosisFinalOld != "")
                {
                    DiagTrmtItem.DiagnosisFinal = DiagTrmtItem.DiagnosisFinal.Replace(DiagnosisFinalOld, DiagnosisFinalNew);
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(DiagTrmtItem.DiagnosisFinal))
                    {
                        DiagTrmtItem.DiagnosisFinal += DiagnosisFinalNew;
                    }
                    else
                    {
                        DiagTrmtItem.DiagnosisFinal += "; " + DiagnosisFinalNew;
                    }
                }
                DiagnosisFinalOld = ObjectCopier.DeepCopy(DiagnosisFinalNew);
            }

        }
        #region get DiagnosisFinal
        private string DiagnosisFinalOld = "";
        private string DiagnosisFinalNew = "";

        #endregion


        public void AxDataGridNy_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            DiagnosisIcd10Items item = e.Row.DataContext as DiagnosisIcd10Items;
            if (ColumnIndex == 0 || ColumnIndex == 1)
            {
                if (refIDC10Item.DiseasesReference == null)
                {
                    if (DiseasesReferenceCopy != null)
                    {
                        refIDC10Item.DiseasesReference = ObjectCopier.DeepCopy(DiseasesReferenceCopy);
                        if (CheckExists(refIDC10Item, false))
                        {
                            GetDiagTreatmentFinal(refIDC10Item.DiseasesReference);
                        }
                    }
                }
            }
            if (refIDC10Item != null && refIDC10Item.DiseasesReference != null)
            {
                if (CheckExists(refIDC10Item, false))
                {
                    if (e.Row.GetIndex() == (refIDC10List.Count - 1) && e.EditAction == DataGridEditAction.Commit)
                    {
                        System.Windows.Application.Current.Dispatcher.Invoke(() => AddBlankRow());
                    }
                }
            }
            //gICD10Changed?.Invoke(refIDC10List);
            ICD10Changed(refIDC10List);
        }

        #endregion

        #region List Status Member

        private ObservableCollection<Lookup> _DiagIcdStatusList;
        public ObservableCollection<Lookup> DiagIcdStatusList
        {
            get
            {
                return _DiagIcdStatusList;
            }
            set
            {
                if (_DiagIcdStatusList != value)
                {
                    _DiagIcdStatusList = value;
                    NotifyOfPropertyChange(() => DiagIcdStatusList);
                }
            }
        }

        //private void GetAllLookupValuesByType()
        //{
        //    //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
        //    var t = new Thread(() =>
        //    {

        //        using (var serviceFactory = new CommonServiceClient())
        //        {
        //            var contract = serviceFactory.ServiceInstance;
        //            contract.BeginGetAllLookupValuesByType(LookupValues.V_DiagIcdStatus, Globals.DispatchCallback((asyncResult) =>
        //            {
        //                try
        //                {
        //                    var results = contract.EndGetAllLookupValuesByType(asyncResult);
        //                    DiagIcdStatusList = results.ToObservableCollection();
        //                }
        //                catch (Exception ex)
        //                {
        //                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
        //                }
        //                finally
        //                {
        //                    //Globals.IsBusy = false;
        //                }

        //            }), null);

        //        }

        //    });

        //    t.Start();
        //}


        private void GetAllLookupValuesByType()
        {
            ObservableCollection<Lookup> DiagICDSttLookupList = Globals.AllLookupValueList.Where(x => x.ObjectTypeID == (long)LookupValues.V_DiagIcdStatus).ToObservableCollection();

            if (DiagICDSttLookupList == null || DiagICDSttLookupList.Count <= 0)
            {
                MessageBox.Show(eHCMSResources.A0751_G1_Msg_InfoKhTimThayStatusICD11, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }

            DiagIcdStatusList = DiagICDSttLookupList;

        }


        public void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            ((ComboBox)sender).ItemsSource = DiagIcdStatusList;
            if (refIDC10Item != null && DiagIcdStatusList != null)
            {
                if (refIDC10Item.LookupStatus == null)
                {
                    ((ComboBox)sender).SelectedIndex = 0;
                }
                else
                {
                    ((ComboBox)sender).SelectedItem = refIDC10Item.LookupStatus;
                }
            }
        }

        #endregion

        private bool Equal(DiagnosisIcd10Items a, DiagnosisIcd10Items b)
        {
            return a.DiagIcd10ItemID == b.DiagIcd10ItemID
                && a.DiagnosisIcd10ListID == b.DiagnosisIcd10ListID
                && a.ICD10Code == b.ICD10Code
                && a.IsMain == b.IsMain
                && a.IsCongenital == b.IsCongenital
                && (a.LookupStatus != null && b.LookupStatus != null
                    && a.LookupStatus.LookupID == b.LookupStatus.LookupID);
        }

        public long Compare2Object()
        {
            long ListID = 0;
            ObservableCollection<DiagnosisIcd10Items> temp = refIDC10List.Where(x => x.DiseasesReference != null).ToObservableCollection();
            if (refIDC10ListCopy != null && refIDC10ListCopy.Count > 0 && refIDC10ListCopy.Count == temp.Count)
            {
                int icount = 0;
                for (int i = 0; i < refIDC10ListCopy.Count; i++)
                {
                    for (int j = 0; j < temp.Count; j++)
                    {
                        if (Equal(refIDC10ListCopy[i], refIDC10List[j]))
                        {
                            icount++;
                        }
                    }

                }
                if (icount == refIDC10ListCopy.Count)
                {
                    ListID = refIDC10ListCopy.FirstOrDefault().DiagnosisIcd10ListID;
                    return ListID;
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                return 0;
            }

        }

        private bool CheckedIsMain()
        {
            ObservableCollection<DiagnosisIcd10Items> temp = refIDC10List.Where(x => x.DiseasesReference != null).ToObservableCollection();
            if (temp != null && temp.Count > 0)
            {
                int bcount = 0;
                for (int i = 0; i < temp.Count; i++)
                {
                    if (temp[i].IsMain)
                    {
                        bcount++;
                    }
                }
                if (bcount == 0)
                {
                    Globals.ShowMessage(eHCMSResources.Z0509_G1_PhaiChonBenhChinh, eHCMSResources.G0442_G1_TBao);
                    return false;
                }
                else if (bcount == 1)
                {
                    return true;
                }
                else
                {
                    Globals.ShowMessage(eHCMSResources.Z0510_G1_I, eHCMSResources.G0442_G1_TBao);
                    return false;
                }
            }
            else
            {
                return true;
            }
        }

        private bool CheckCountIsMain()
        {
            ObservableCollection<DiagnosisIcd10Items> temp = refIDC10List.Where(x => x.DiseasesReference != null).ToObservableCollection();
            if (temp != null && temp.Count > 0)
            {
                int bcount = 0;
                for (int i = 0; i < temp.Count; i++)
                {
                    if (temp[i].IsMain)
                    {
                        bcount++;
                    }
                }
                if (bcount == 0)
                {
                    return true;
                }
            }
            return false;
        }
        AxDataGridNyICD10 grdConsultation { get; set; }
        public void grdConsultation_Loaded(object sender, RoutedEventArgs e)
        {
            grdConsultation = sender as AxDataGridNyICD10;
        }
        public void lnkDelete_Click(object sender, RoutedEventArgs e)
        {
            //var p = grdConsultation.SelectedItem as DiagnosisIcd10Items;
            //if (p == null)
            //{
            //    MessageBox.Show(eHCMSResources.Z0405_G1_KgTheXoaDongRong);
            //    return;
            //}
            if (IsUpdateFromPresciption)
            {
                return;
            }
            if (refIDC10Item == null
                || refIDC10Item.DiseasesReference == null)
            {
                MessageBox.Show(eHCMSResources.Z0405_G1_KgTheXoaDongRong);
                return;
            }

            int nSelIndex = grdConsultation.SelectedIndex;
            if (nSelIndex >= refIDC10List.Count - 1)
            {
                MessageBox.Show(eHCMSResources.Z0405_G1_KgTheXoaDongRong);
                return;
            }

            var item = refIDC10List[nSelIndex];

            if (item != null && item.ICD10Code != null && item.ICD10Code != "")
            {
                if (MessageBox.Show(eHCMSResources.Z0419_G1_CoMuonXoaMaICD10, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    if (item.DiseasesReference != null
                        && item.DiseasesReference.DiseaseNameVN != "")
                    {
                        DiagTrmtItem.DiagnosisFinal = DiagTrmtItem.DiagnosisFinal.Replace(item.DiseasesReference.DiseaseNameVN, "");
                    }
                    //refIDC10List.RemoveAt(nSelIndex);
                    refIDC10List.Remove(refIDC10List[nSelIndex]);
                    /*▼====: #008*/
                    IsDiagTrmentChanged = true;
                    /*▲====: #008*/
                }
            }
        }

        #region link member

        public void hpkCreatePrescription()
        {
            Globals.ConsultationIsChildWindow = true;
            Globals.PrescriptionIsChildWindow = false;

            var Conslt = Globals.GetViewModel<IConsultationModule>();
            var PrescriptionVM = Globals.GetViewModel<IePrescriptions>();
            PrescriptionVM.Registration_DataStorage = Registration_DataStorage;
            Conslt.MainContent = PrescriptionVM;
            (Conslt as Conductor<object>).ActivateItem(PrescriptionVM);
        }

        #endregion



        #region Các Button
        public void btEdit()
        {
            //FormEditorIsEnabled = true;
            //IsEnableButton = false;
            //btSaveCreateNewIsEnabled = false;
            //btUpdateIsEnabled = true;
            //btCancelIsEnabled = true;

            if (DiagTrmtItem == null || DiagTrmtItem.ServiceRecID.GetValueOrDefault() <= 0 || DiagTrmtItem.DTItemID <= 0)
            {
                MessageBox.Show(eHCMSResources.A0629_G1_Msg_InfoKhCoCDDeSua, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
            StateEditWaiting();
            //▼===== #017
            GetAllTreatmentByType();
            //▲===== #017
            DiagTrmtItemCopy = ObjectCopier.DeepCopy(DiagTrmtItem);
            /*▼====: #003*/
            DiagTrmtItemCopy.IsDataChanged = false;
            /*▲====: #003*/
            CopyListICD10();

        }

        public void btUpdate()
        {
            Coroutine.BeginExecute(AllCheck());
        }

        public void btCancel()
        {
            //FormEditorIsEnabled = false;

            DiagTrmtItem = ObjectCopier.DeepCopy(DiagTrmtItemCopy);
            /*▼====: #003*/
            if (DiagTrmtItem != null)
            {
                DiagTrmtItem.IsDataChanged = false;
                DiagTrmtItemCopy.IsDataChanged = false;
            }
            /*▲====: #003*/
            refIDC10List = refIDC10ListCopy;

            //if (DiagTrmtItem != null && DiagTrmtItem.DTItemID > 0)
            //{
            //    ButtonForHasDiag();
            //}
            //else
            //{
            //    ButtonForNotDiag(false);            
            //}
            switch (ConsultState)
            {
                case ConsultationState.NewConsultationState:
                    StateNew(); break;
                case ConsultationState.EditConsultationState:
                    StateEdit(); break;
                case ConsultationState.NewAndEditConsultationState:
                    StateNewEdit(); break;
            }
            /*▼====: #008*/
            IsDiagTrmentChanged = false;
            /*▲====: #008*/
        }
        /*▼====: #007*/
        private int CountText(string Text)
        {
            if (Text == null)
            {
                return 0;
            }
            string text = Text.Trim();
            int wordCount = 0, index = 0;
            if (text.Length > 2)
            {
                while (index < text.Length)
                {
                    while (index < text.Length && !char.IsWhiteSpace(text[index]))
                    {
                        index++;
                    }
                    wordCount++;
                    while (index < text.Length && char.IsWhiteSpace(text[index]))
                    {
                        index++;
                    }
                }
            }
            return wordCount;
        }
        private bool CheckCountText()
        {
            /*▼====: #011*/
            if (DiagTrmtItem.Diagnosis == null)
            {
                DiagTrmtItem.Diagnosis = "";
            }
            if (DiagTrmtItem.OrientedTreatment == null)
            {
                DiagTrmtItem.OrientedTreatment = "";
            }
            if (DiagTrmtItem.Treatment == null)
            {
                DiagTrmtItem.Treatment = "";
            }
            /*▲====: #011*/
            string strWarningMsg = "";
            if (CountText(DiagTrmtItem.DiagnosisFinal) < Globals.ServerConfigSection.ConsultationElements.MinNumOfChar)
            {
                strWarningMsg += string.Format("{0} - ", eHCMSResources.K1775_G1_CDoanXDinh2);
            }
            //TBL: Khi cau hinh rang buoc truong bat buoc nhap thi moi kiem tra so chu. Nhung khi nguoi dung nhap vao truong khong bat buoc thi cung kiem tra
            if (((Globals.ServerConfigSection.ConsultationElements.IsAllowInputDiagTrmt & IsAllowInputDiagnosis) == 0 && CountText(DiagTrmtItem.Diagnosis) < Globals.ServerConfigSection.ConsultationElements.MinNumOfChar) ||
                 (DiagTrmtItem.Diagnosis != "" && CountText(DiagTrmtItem.Diagnosis) < Globals.ServerConfigSection.ConsultationElements.MinNumOfChar))
            {
                strWarningMsg += string.Format("{0} - ", eHCMSResources.G1785_G1_TrieuChungDHieuLS);
            }
            if (((Globals.ServerConfigSection.ConsultationElements.IsAllowInputDiagTrmt & IsAllowInputOrientedTreatment) < 2 && CountText(DiagTrmtItem.OrientedTreatment) < Globals.ServerConfigSection.ConsultationElements.MinNumOfChar) ||
                 (DiagTrmtItem.OrientedTreatment != "" && CountText(DiagTrmtItem.OrientedTreatment) < Globals.ServerConfigSection.ConsultationElements.MinNumOfChar))
            {
                strWarningMsg += string.Format("{0} - ", eHCMSResources.Z3309_G1_DienBienBenh);
            }
            //TBL: Hien tai Cach dieu tri da chuyen thanh combobox nen khong can 
            //if (((Globals.ServerConfigSection.ConsultationElements.IsAllowInputDiagTrmt & IsAllowInputTreatment) < 4 && CountText(DiagTrmtItem.Treatment) < Globals.ServerConfigSection.ConsultationElements.MinNumOfChar) ||
            //     (DiagTrmtItem.Treatment != "" && CountText(DiagTrmtItem.Treatment) < Globals.ServerConfigSection.ConsultationElements.MinNumOfChar))
            //{
            //    strWarningMsg += string.Format("{0} - ", eHCMSResources.Z0021_G1_CachDTri);
            //}
            if (strWarningMsg != "")
            {
                MessageBox.Show(string.Format("{0}: ",eHCMSResources.Z2306_G1_SoChuKhongHopLe) + strWarningMsg);
                return false;
            }
            return true;
        }
        /*▲====: #007*/
        /*▼====: #009*/
        private const Int16 IsAllowInputDiagnosis = 1;
        private const Int16 IsAllowInputOrientedTreatment = 2;
        private const Int16 IsAllowInputTreatment = 4;
        /*▲====: #009*/
        private bool CheckEmptyFields()
        {
            string strWarningMsg = "";
            /*▼====: #009*/
            //TBL: Truong nay la truong quan trong nen code cung khong cho tuy chinh theo cau hinh
            if (DiagTrmtItem.DiagnosisFinal == null || DiagTrmtItem.DiagnosisFinal.Trim() == "")
            {
                strWarningMsg += string.Format("{0} - ", eHCMSResources.K1775_G1_CDoanXDinh2);
            }
            //if (DiagTrmtItem.Diagnosis == null || DiagTrmtItem.Diagnosis.Trim() == "")
            //{
            //    strWarningMsg += string.Format("{0} - ", eHCMSResources.G1785_G1_TrieuChungDHieuLS);
            //}
            //// Hpt 20/11/2015: Giờ có yêu cầu bỏ ra nên comment lại đây, khi nào cần thì mở ra
            ////if (DiagTrmtItem.OrientedTreatment == null || DiagTrmtItem.OrientedTreatment.Trim() == "")
            ////{
            ////    strWarningMsg += string.Format("{0} - ", eHCMSResources.Z3309_G1_DienBienBenh);
            ////}
            //if (DiagTrmtItem.Treatment == null || DiagTrmtItem.Treatment.Trim() == "")
            //{
            //    strWarningMsg += string.Format("{0} - ", eHCMSResources.Z0021_G1_CachDTri);
            //}
            if ((Globals.ServerConfigSection.ConsultationElements.IsAllowInputDiagTrmt & IsAllowInputDiagnosis) == 0 && (DiagTrmtItem.Diagnosis == null || DiagTrmtItem.Diagnosis.Trim() == ""))
            {
                strWarningMsg += string.Format("{0} - ", eHCMSResources.G1785_G1_TrieuChungDHieuLS);
            }
            if ((Globals.ServerConfigSection.ConsultationElements.IsAllowInputDiagTrmt & IsAllowInputOrientedTreatment) < 2 && (DiagTrmtItem.OrientedTreatment == null || DiagTrmtItem.OrientedTreatment.Trim() == ""))
            {
                strWarningMsg += string.Format("{0} - ", eHCMSResources.Z3309_G1_DienBienBenh);
            }
            if ((Globals.ServerConfigSection.ConsultationElements.IsAllowInputDiagTrmt & IsAllowInputTreatment) < 4 && (DiagTrmtItem.V_TreatmentType == 0))
            {
                strWarningMsg += string.Format("{0} - ", eHCMSResources.Z0021_G1_CachDTri);
            }
            /*▲====: #009*/
            if (strWarningMsg != "")
            {
                MessageBox.Show(string.Format("{0}: ", eHCMSResources.A0201_G1_Msg_InfoYCNhapSth) + strWarningMsg);
                return false;
            }
            return true;
        }
        MessageWarningShowDialogTask warningtask = null;
        string content = eHCMSResources.Z0420_G1_CDoanCoICDLaZ10;
        // Hpt 22/09/2015: Thêm hàm coroutine SaveNewDiagnosis ở đây, cắt thân hàm btSaveCreateNew bỏ vào, để có thể sử dụng được đối tượng MessageWarningShowDialogTask hiển thị cảnh báo nếu có ICD10 là Z10
        // Thân hàm btSaveCreateNew thay bằng code gọi đến coroutine SaveNewDiagnosis

        private IEnumerator<IResult> SaveNewDiagnosis()
        {
            if (refIDC10List!=null&&refIDC10List.Any(x => x.IsInvalid))
            {
                MessageBox.Show(string.Format(eHCMSResources.Z2205_G1_ICD10KhongHopLe, string.Join(",", refIDC10List.Where(x => x.IsInvalid).Select(x => x.ICD10Code).ToList())), eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                yield break;
            }
            if (!CheckEmptyFields())
            {
                yield break;
            }
            //if (!IsNotExistsDiagnosisTreatmentByPtRegDetailID)
            //{
            //    MessageBox.Show(eHCMSResources.A0449_G1_Msg_InfoDaCoCDChoBenh);
            //    return;
            //}

            long lBehaving = 0;
            try
            {
                lBehaving = DiagTrmtItem.PatientServiceRecord.V_Behaving.GetValueOrDefault();
            }
            catch
            {
                MessageBox.Show(eHCMSResources.A0367_G1_Msg_InfoChonTieuDe);
                yield break;
            }

            long lPMRTemplateID = 0;
            try
            {
                lPMRTemplateID = DiagTrmtItem.MDRptTemplateID;
            }
            catch
            {
                MessageBox.Show(eHCMSResources.A0337_G1_Msg_InfoChonMauBAn);
                yield break;
            }

            DiagTrmtItem.PatientServiceRecord.Staff = Globals.LoggedUserAccount.Staff;
            DiagTrmtItem.PatientServiceRecord.StaffID = Globals.LoggedUserAccount.Staff.StaffID;


            if (CheckedIsMain() && NeedICD10())
            {
                //Khám DV cụ thể nào
                if (Registration_DataStorage.CurrentPatientRegistrationDetail != null && Registration_DataStorage.CurrentPatientRegistrationDetail.PtRegDetailID > 0)
                {
                    DiagTrmtItem.PtRegDetailID = Registration_DataStorage.CurrentPatientRegistrationDetail.PtRegDetailID;
                }
                else
                {
                    DiagTrmtItem.PtRegDetailID = 0;
                }
                //Khám DV cụ thể nào

                DiagTrmtItem.PatientServiceRecord.PatientMedicalRecord.PatientID = Registration_DataStorage.CurrentPatient.PatientID;
                DiagTrmtItem.PatientServiceRecord.StaffID = Globals.LoggedUserAccount.Staff.StaffID;
                DiagTrmtItem.PatientServiceRecord.PtRegistrationID = Registration_DataStorage.CurrentPatientRegistration.PtRegistrationID;
                DiagTrmtItem.DeptLocationID = Globals.DeptLocation.DeptLocationID;

                DiagTrmtItem.PatientServiceRecord.V_RegistrationType = Registration_DataStorage.CurrentPatientRegistration.V_RegistrationType;

                DiagTrmtItem.PatientServiceRecord.V_DiagnosisType = (long)AllLookupValues.V_DiagnosisType.DIAGNOSIS_NORMAL;
                //KMx: Loại đăng ký phải dựa trên ĐK của BN, không dựa vào tiêu chí tìm kiếm đăng ký.
                //Nếu không sẽ bị sai khi tìm kiếm và chọn 1 BN ngoại trú, sau đó tick vào NỘI TRÚ (09/10/2014 10:12).
                //if (Globals.PatientFindBy_ForConsultation.Value == AllLookupValues.PatientFindBy.NGOAITRU)
                //{
                //    DiagTrmtItem.PatientServiceRecord.V_RegistrationType = AllLookupValues.RegistrationType.NGOAI_TRU;
                //}

                //if (Globals.PatientFindBy_ForConsultation.Value == AllLookupValues.PatientFindBy.NOITRU)
                //{
                //    DiagTrmtItem.PatientServiceRecord.V_RegistrationType = AllLookupValues.RegistrationType.NOI_TRU;
                //}

                // Hpt 22/09/2015: bắt buộc phải nhập Diễn tiến bệnh

                DiagTrmtItem.ICD10List = String.Join(",", from item in refIDC10List
                                                          where (!string.IsNullOrEmpty(item.ICD10Code) && item.DiseasesReference != null)
                                                          select item.ICD10Code);
                PatientRegistration CurRegistration = Registration_DataStorage.CurrentPatientRegistration;
                // Hpt 22/09/2015: Nếu bệnh nhân có quyền lợi về BHYT thì mới kiểm tra xem trong danh sách có ICD10 nào là Z10 hay không vì BHYT sẽ không chi trả cho những trường hợp này
                if (CurRegistration.PtInsuranceBenefit != null && CurRegistration.PtInsuranceBenefit > 0 && refIDC10List != null
                    && refIDC10List.Any(x => x.ICD10Code != null && x.DiseasesReference != null && x.DiseasesReference.ICD10Code.Contains("Z10")))
                {
                    warningtask = new MessageWarningShowDialogTask(content, eHCMSResources.Z0339_G1_TiepTucLuuCDoan);
                    yield return warningtask;
                    if (warningtask.IsAccept)
                    {
                        AddNewDiagTrmt();
                    }
                    else
                    {
                        yield break;
                    }
                }
                // Nếu là đăng ký ngoại trú không có BHYT thì cho lưu luôn không cần kiểm tra gì
                else
                {
                    AddNewDiagTrmt();
                }
            }
        }
        public void btSaveCreateNew()
        {
            Coroutine.BeginExecute(SaveNewDiagnosis());
        }
        public void btCreateNew()
        {
            if (Registration_DataStorage.CurrentPatientRegistration != null && Globals.IsLockRegistration(Registration_DataStorage.CurrentPatientRegistration.RegLockFlag, eHCMSResources.Z0407_G1_CNhatCDoan))
            {
                return;
            }
            //FormEditorIsEnabled = true;

            //ButtonForNotDiag(true);
            StateNewWaiting();
            //▼===== #017
            GetAllTreatmentByType();
            //▲===== #017
            DiagTrmtItemCopy = ObjectCopier.DeepCopy(DiagTrmtItem);

            //KMx: Không cần gọi về server, chỉ cần sử dụng hàm ResetDefaultForBehaving() trong ruột GetBlankDiagnosisTreatmentByPtID() (01/11/2014 10:16).
            //GetBlankDiagnosisTreatmentByPtID(Registration_DataStorage.CurrentPatient.PatientID);
            DiagTrmtItem = new DiagnosisTreatment();
            //▼====: #018
            DiagTrmtItem.UserOfficialAccountID = Globals.DoctorAccountBorrowed.StaffID;
            //▲====: #018
            ResetDefaultForBehaving();

            CopyListICD10ForNew();
            //CopyListICD10();
        }
        /*▼====: #006*/
        private const Int16 IsAllowCopyDiagnosis = 1;
        private const Int16 IsAllowCopyDiagnosisFinal = 2;
        private const Int16 IsAllowCopyOrientedTreatment = 4;
        private const Int16 IsAllowCopyTreatment = 8;
        /*▲====: #006*/
        public void btSaveNewByOld()
        {
            //FormEditorIsEnabled = true;
            //ButtonForNotDiag(true);
            StateNewWaiting();
            //▼===== #017
            GetAllTreatmentByType();
            //▲===== #017
            DiagTrmtItem.DTItemID = 0;
            DiagTrmtItemCopy = ObjectCopier.DeepCopy(DiagTrmtItem);
            /*▼====: #006*/
            //▼===== 20191117 TTM: Nếu như tạo mới dựa trên cũ thì version của đợt đăng ký này cũng phải set về 0. Nếu không set về 0 khi tạo mới dựa trên cũ sẽ giữ nguyên version cũ => cập nhật sẽ báo lỗi.
            if (DiagTrmtItem != null && DiagTrmtItem.VersionNumber > 0)
            {
                DiagTrmtItem.VersionNumber = 0;
            }
            //▲=====
            //TBL: Tuy theo cau hinh se xoa truong nao khong muon 
            //TBL: Truong trieu chung/dau hieu cls khi cau hinh = 1 se dc giu lai
            if ((Globals.ServerConfigSection.ConsultationElements.IsAllowCopyDiagTrmt & IsAllowCopyDiagnosis) == 0)
            {
                DiagTrmtItem.Diagnosis = "";
            }
            //TBL: Truong chan doan khi cau hinh = 2 se dc giu lai
            if ((Globals.ServerConfigSection.ConsultationElements.IsAllowCopyDiagTrmt & IsAllowCopyDiagnosisFinal) < 2)
            {
                DiagTrmtItem.DiagnosisFinal = "";
            }
            //TBL: Truong dien tien benh khi cau hinh = 4 se dc giu lai
            if ((Globals.ServerConfigSection.ConsultationElements.IsAllowCopyDiagTrmt & IsAllowCopyOrientedTreatment) < 4)
            {
                DiagTrmtItem.OrientedTreatment = "";
            }
            //TBL: Truong cach dieu tri khi cau hinh = 8 se dc giu lai
            if ((Globals.ServerConfigSection.ConsultationElements.IsAllowCopyDiagTrmt & IsAllowCopyTreatment) < 8)
            {
                DiagTrmtItem.Treatment = "";
            }
            /*▲====: #006*/
            /*▼====: #003*/
            //TBL: khi tao moi chan doan dua tren chan doan cu thi set IsObjectBeingUsedByClient = true de biet ma di luu
            DiagTrmtItemCopy.IsObjectBeingUsedByClient = true;
            /*▲====: #003*/
            //▼====: #018
            DiagTrmtItem.UserOfficialAccountID = Globals.DoctorAccountBorrowed.StaffID;
            //▲====: #018
            CopyListICD10();
            //gICD10Changed?.Invoke(refIDC10List);
            ICD10Changed(refIDC10List);
        }

        private void AddNewDiagTrmt()
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });

            this.ShowBusyIndicator();

            IsWaitingSaveAddNew = true;

            long ID = Compare2Object();

            var t = new Thread(() =>
            {

                using (var serviceFactory = new ePMRsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginAddDiagnosisTreatment(DiagTrmtItem, ID, refIDC10List, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            long ServiceID = 0;

                            if (contract.EndAddDiagnosisTreatment(out ServiceID, asyncResult))
                            {
                                StateEdit();

                                if (refIDC10List != null)
                                {
                                    refIDC10List = refIDC10List.Where(x => x.DiseasesReference != null).ToObservableCollection();
                                }


                                //phat su kien reload lai danh sach 
                                //Globals.EventAggregator.Publish(new ReloadDataConsultationEvent { });

                                //KMx: Sau khi lưu chẩn đoán, reload Service Record (22/05/2014 09:48).
                                IConsultationModule consultVM = Globals.GetViewModel<IConsultationModule>();

                                consultVM.PatientServiceRecordsGetForKhamBenh_Ext();
                                var homeVm = Globals.GetViewModel<IHome>();
                                if (homeVm.OutstandingTaskContent != null && homeVm.OutstandingTaskContent is IConsultationOutstandingTask)
                                {
                                    ((IConsultationOutstandingTask)homeVm.OutstandingTaskContent).SearchRegistrationListForOST();
                                }

                                //KMx: Sau khi lưu chẩn đoán, xóa danh sách chẩn đoán. Vì danh sách không tự động load lại.
                                //Nếu không xóa thì chẩn đoán vừa lưu sẽ khác với chẩn đoán trong danh sách (22/05/2014 09:48).
                                Globals.EventAggregator.Publish(new ClearAllDiagnosisListAfterAddNewEvent());


                                //Nếu đang là Popup thì phát event lấy cđ này gán vào khám bệnh
                                if (Globals.ConsultationIsChildWindow)
                                {
                                    Globals.EventAggregator.Publish(new DiagnosisTreatmentSelectedEvent<DiagnosisTreatment> { DiagnosisTreatment = DiagTrmtItem.DeepCopy() });
                                }


                                MessageBox.Show(eHCMSResources.A0468_G1_Msg_InfoLuuOK, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                            }
                            else
                            {
                                if (DiagTrmtItem.PatientServiceRecord.V_DiagnosisType == (long)AllLookupValues.V_DiagnosisType.DIAGNOSIS_IN)
                                {
                                    MessageBox.Show(eHCMSResources.Z0409_G1_DaCoCDoanNV, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                }
                                else if (DiagTrmtItem.PatientServiceRecord.V_DiagnosisType == (long)AllLookupValues.V_DiagnosisType.DIAGNOSIS_OUTHOS)
                                {
                                    MessageBox.Show(eHCMSResources.Z0410_G1_DaCoCDoanXV, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                }
                                else
                                {
                                    MessageBox.Show(eHCMSResources.Z0411_G1_LuuCDoanKgThCong, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                }
                            }

                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                        }
                        finally
                        {
                            //Globals.IsBusy = false;
                            //IsWaitingSaveAddNew = false;
                            this.HideBusyIndicator();
                        }

                    }), null);

                }

            });

            t.Start();
        }

        //private IEnumerator<IResult> LoadRefBehaving_MedRecTemplate()
        //{
        //    var resultBehavingTask = new LoadLookupBehavingTask();
        //    yield return resultBehavingTask;
        //    RefBehaving = resultBehavingTask.RefBehaving;

        //    var resultMedRecTemplateTask = new LoadMedRecTemplateTask();
        //    yield return resultMedRecTemplateTask;
        //    RefMedRecTemplate = resultMedRecTemplateTask.RefMedRecTemplate;


        //    var resultDiagnosisTask = new LoadLookupListTask(LookupValues.V_DiagnosisType, false, false);
        //    yield return resultDiagnosisTask;
        //    RefDiagnosis = resultDiagnosisTask.LookupList;

        //    yield break;
        //}

        private void LoadRefBehaving_MedRecTemplate()
        {
            // 1. Get Behaving.
            ObservableCollection<Lookup> BehavingLookupList = Globals.AllLookupValueList.Where(x => x.ObjectTypeID == (long)LookupValues.BEHAVING).ToObservableCollection();

            if (BehavingLookupList == null || BehavingLookupList.Count <= 0)
            {
                MessageBox.Show(eHCMSResources.A0740_G1_Msg_InfoKhTimThayLoaiKB, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }

            RefBehaving = BehavingLookupList;

            // 2. Get Medical Record Templates.
            RefMedRecTemplate = Globals.AllMedRecTemplates;

            // 3.Get DiagnosisType.
            //KMx: Không lấy loại chẩn đoán Thường. Vì BN nội trú chỉ có chẩn đoán Nhập viện hoặc Xuất viện (09/10/2014 18:05).
            ObservableCollection<Lookup> DiagnosisLookupList = Globals.AllLookupValueList.Where(x => x.ObjectTypeID == (long)LookupValues.V_DiagnosisType && x.LookupID != (long)AllLookupValues.V_DiagnosisType.DIAGNOSIS_NORMAL).ToObservableCollection();

            if (DiagnosisLookupList == null || DiagnosisLookupList.Count <= 0)
            {
                MessageBox.Show(eHCMSResources.A0739_G1_Msg_InfoKhTimThayLoaiCDoan, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }

            RefDiagnosis = DiagnosisLookupList;

        }

        private void LoadInitData()
        {
            //Coroutine.BeginExecute(LoadRefBehaving_MedRecTemplate());
            LoadRefBehaving_MedRecTemplate();
            /*▼====: #012*/
            GetAllTreatmentByType();
            /*▲====: #012*/
        }
        #endregion

        public PatientRegistration ObjPatientRegistrationVIP { get; set; }

        public void Handle(EventKhamChoVIP<PatientRegistration> message)
        {
            if (message != null)
            {
                ObjPatientRegistrationVIP = message.PtReg;
            }
        }

        //==== #001 ====
        public void grdConsultation_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            DiagnosisIcd10Items objRows = e.Row.DataContext as DiagnosisIcd10Items;
            if (objRows != null)
            {
                switch (objRows.IsMain)
                {
                    case true:
                        e.Row.Background = new SolidColorBrush(Color.FromArgb(128, 250, 155, 232));
                        break;
                    default:
                        e.Row.Background = new SolidColorBrush(Colors.White);
                        break;
                }
                if (objRows.IsInvalid)
                {
                    e.Row.Background = new SolidColorBrush(Color.FromArgb(115, 114, 113, 30));
                }
            }

        }
        //==== #001 ====

        /*TMA*/
        //btGCT
        //▼====== #002 
        //TTM:  Chuyển từ publish event sang gọi thông qua Interface và đặt hàm gọi vào Action.
        //      Thông tin bệnh nhân, thông tin thẻ bảo hiểm ... phải đc truyền vào trong lúc người dùng mở popup.
        //      Không lý do gì đóng popup lại rồi mới truyền dữ liệu.
        public void GetPaPerReferalFul(int V_TransferFormType, int PatientFindBy)
        {
            PatientRegistration CurRegistration = Registration_DataStorage.CurrentPatientRegistration;
            var mEvent = new TransferFormEvent();
            mEvent.Item = new TransferForm();
            mEvent.Item.PatientFindBy = PatientFindBy;
            mEvent.Item.CurPatientRegistration = new PatientRegistration();
            mEvent.Item.V_TransferFormType = V_TransferFormType;
            mEvent.Item.TransferFormID = (long)0;
            mEvent.Item.CurPatientRegistration.PtRegistrationID = (long)CurRegistration.PtRegistrationID;
            if (CurRegistration.HisID != null)
                mEvent.Item.CurPatientRegistration.HisID = (long)CurRegistration.HisID.Value;
            if (CurRegistration != null)
            {
                if (CurRegistration.HealthInsurance != null)
                    mEvent.Item.CurPatientRegistration.HealthInsurance = CurRegistration.HealthInsurance;
                if (CurRegistration.Patient != null)
                {
                    mEvent.Item.CurPatientRegistration.Patient = CurRegistration.Patient;
                }
                if (DiagTrmtItem != null)
                {
                    if (DiagTrmtItem.Diagnosis != null)
                        mEvent.Item.ClinicalSign = DiagTrmtItem.Diagnosis;
                    if (DiagTrmtItem.Treatment != null)
                        mEvent.Item.UsedServicesAndItems = DiagTrmtItem.Treatment;
                }

                if (DiagTrmtItem.DiagnosisFinal != null && DiagTrmtItem.ICD10List != null)
                {
                    mEvent.Item.ICD10Final = DiagTrmtItem.ICD10List;
                    mEvent.Item.ICD10 = DiagTrmtItem.ICD10List;
                    mEvent.Item.DiagnosisTreatment_Final = DiagTrmtItem.DiagnosisFinal;
                    mEvent.Item.DiagnosisTreatment = DiagTrmtItem.DiagnosisFinal;
                }
                if (DiagTrmtItem.PatientServiceRecord != null && DiagTrmtItem.PatientServiceRecord.ExamDate != null)
                    mEvent.Item.FromDate = DiagTrmtItem.PatientServiceRecord.ExamDate;
                mEvent.Item.ToDate = DateTime.Now;
                mEvent.Item.TransferDate = DateTime.Now;
            }
            mEvent.Item.TransferFromHos = new Hospital();
            mEvent.Item.TransferToHos = new Hospital();
            mEvent.Item.V_TransferTypeID = 62604;       //defalut : chuyễn giữa các cơ sở cùng tuyế 
            mEvent.Item.V_PatientStatusID = 63002;      //defalut : không cấp cứu
            mEvent.Item.V_TransferReasonID = 62902;     //default : yêu cầu chuyên môn
            mEvent.Item.V_TreatmentResultID = 62702;    //defalut : ko thuyên giảm. Nặng lên - V_TreatmentResult_V2 trong bảng lookup
            mEvent.Item.V_CMKTID = 62801;               //default : chuyển đúng tuyến, đúng chuyên môn kỹ thuật

            Action<IPaperReferalFull> onInitDlg = delegate (IPaperReferalFull TransferFromVm)
            {
                TransferFromVm.IsThisViewDialog = true;
                TransferFromVm.V_TransferFormType = V_TransferFormType;

                TransferFromVm.V_GetPaperReferalFullFromOtherView = true;
                if (CurRegistration != null && CurRegistration.PtRegistrationID != 0)
                {
                    TransferFromVm.CurrentTransferForm.CurPatientRegistration = new PatientRegistration();
                    TransferFromVm.CurrentTransferForm.CurPatientRegistration.PtRegistrationID = (long)CurRegistration.PtRegistrationID;
                    if (CurRegistration.HisID != null)
                        TransferFromVm.CurrentTransferForm.CurPatientRegistration.HisID = (long)CurRegistration.HisID.Value;
                }
                this.ActivateItem(TransferFromVm);
                TransferFromVm.SetCurrentInformation(mEvent);
            };
            GlobalsNAV.ShowDialog<IPaperReferalFull>(onInitDlg);
        }

        //public void GetPaPerReferalFul(int V_TransferFormType, int PatientFindBy)
        //{
        //    PatientRegistration CurRegistration = Registration_DataStorage.CurrentPatientRegistration;
        //    Action<IPaperReferalFull> onInitDlg = delegate (IPaperReferalFull TransferFromVm)
        //    {
        //        TransferFromVm.V_TransferFormType = V_TransferFormType;

        //        TransferFromVm.V_GetPaperReferalFullFromOtherView = true;
        //        if (CurRegistration != null && CurRegistration.PtRegistrationID != 0)
        //        {
        //            TransferFromVm.CurrentTransferForm.CurPatientRegistration = new PatientRegistration();
        //            TransferFromVm.CurrentTransferForm.CurPatientRegistration.PtRegistrationID = (long)CurRegistration.PtRegistrationID;
        //            if (CurRegistration.HisID != null)
        //                TransferFromVm.CurrentTransferForm.CurPatientRegistration.HisID = (long)CurRegistration.HisID.Value;
        //        }
        //        this.ActivateItem(TransferFromVm);
        //    };
        //    GlobalsNAV.ShowDialog<IPaperReferalFull>(onInitDlg);
        //    var mEvent = new TransferFormEvent();
        //    mEvent.Item = new TransferForm();

        //    mEvent.Item.PatientFindBy = PatientFindBy;

        //    mEvent.Item.CurPatientRegistration = new PatientRegistration();
        //    mEvent.Item.V_TransferFormType = V_TransferFormType;

        //    mEvent.Item.TransferFormID = (long)0;
        //    mEvent.Item.CurPatientRegistration.PtRegistrationID = (long)CurRegistration.PtRegistrationID;
        //    if (CurRegistration.HisID != null)
        //        mEvent.Item.CurPatientRegistration.HisID = (long)CurRegistration.HisID.Value;

        //    if (CurRegistration != null)
        //    {
        //        if (CurRegistration.HealthInsurance != null)
        //            mEvent.Item.CurPatientRegistration.HealthInsurance = CurRegistration.HealthInsurance;
        //        if (CurRegistration.Patient != null)
        //        {
        //            mEvent.Item.CurPatientRegistration.Patient = CurRegistration.Patient;
        //        }
        //        if (DiagTrmtItem != null)
        //        {
        //            if (DiagTrmtItem.Diagnosis != null)
        //                mEvent.Item.ClinicalSign = DiagTrmtItem.Diagnosis;
        //            if (DiagTrmtItem.Treatment != null)
        //                mEvent.Item.UsedServicesAndItems = DiagTrmtItem.Treatment;
        //        }

        //        if (DiagTrmtItem.DiagnosisFinal != null && DiagTrmtItem.ICD10List != null)
        //        {
        //            mEvent.Item.ICD10Final = DiagTrmtItem.ICD10List;
        //            mEvent.Item.ICD10 = DiagTrmtItem.ICD10List;
        //            mEvent.Item.DiagnosisTreatment_Final = DiagTrmtItem.DiagnosisFinal;
        //            mEvent.Item.DiagnosisTreatment = DiagTrmtItem.DiagnosisFinal;
        //        }
        //        //FromDate,
        //        if (DiagTrmtItem.PatientServiceRecord != null && DiagTrmtItem.PatientServiceRecord.ExamDate != null)
        //            mEvent.Item.FromDate = DiagTrmtItem.PatientServiceRecord.ExamDate;
        //        mEvent.Item.ToDate = DateTime.Now;
        //        mEvent.Item.TransferDate = DateTime.Now;
        //    }
        //    mEvent.Item.TransferFromHos = new Hospital();
        //    /*TMA 08/11/2017 Để trống bệnh viện tuyến trước theo yêu cầu của Mr Nguyên - Viện Tim --> bỏ dòng dưới*/
        //    //mEvent.Item.TransferFromHos.HICode = Globals.ServerConfigSection.Hospitals.HospitalCode;
        //    mEvent.Item.TransferToHos = new Hospital();

        //    /*TMA 08/11/2017 thay đổi giá trị khác vs chuyển đến theo yêu cầu của Mr Nguyên - Viện Tim*/
        //    mEvent.Item.V_TransferTypeID = 62604; // defalut : chuyễn giữa các cơ sở cùng tuyế 
        //    mEvent.Item.V_PatientStatusID = 63002;//defalut : không cấp cứu
        //    mEvent.Item.V_TransferReasonID = 62902;//default : yêu cầu chuyên môn
        //    mEvent.Item.V_TreatmentResultID = 62702;//defalut : ko thuyên giảm. Nặng lên - V_TreatmentResult_V2 trong bảng lookup
        //    mEvent.Item.V_CMKTID = 62801; // default : chuyển đúng tuyến, đúng chuyên môn kỹ thuật
        //    Globals.EventAggregator.Publish(mEvent);
        //}
        //▲====== #002
        public void btGCT()
        {
            GetPaPerReferalFul((int)AllLookupValues.V_TransferFormType.CHUYEN_Di,(int)AllLookupValues.PatientFindBy.NGOAITRU);
        }
        public void btGCT_CLS()
        {
            GetPaPerReferalFul((int)AllLookupValues.V_TransferFormType.CHUYEN_DI_CLS,(int)AllLookupValues.PatientFindBy.NGOAITRU);
        }

        //btGCT_CLS
        /*TMA*/

        public bool CheckValidDiagnosis()
        {
            //Khong the luu khi khong co dang ky
            if (DiagTrmtItem == null)
            {
                MessageBox.Show(eHCMSResources.A0633_G1_Msg_InfoKhongCoDKBN);
                return false;
            }
            if (refIDC10List != null && refIDC10List.Any(x => x.IsInvalid))
            {
                MessageBox.Show(string.Format(eHCMSResources.Z2205_G1_ICD10KhongHopLe, string.Join(",", refIDC10List.Where(x => x.IsInvalid).Select(x => x.ICD10Code).ToList())), eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                return false;
            }
            /*▼====: #003*/
            //TBL: set IsDiagTrmentChanged = true khi du lieu thay doi
            if (DiagTrmtItem.IsDataChanged)
            {
                IsDiagTrmentChanged = true;
                DiagTrmtItem.IsDataChanged = false;
            }
            if (DiagTrmtItemCopy != null)
            {
                if (DiagTrmtItemCopy.IsDataChanged)
                {
                    IsDiagTrmentChanged = true;
                    DiagTrmtItemCopy.IsDataChanged = false;
                }
            }
            if (refIDC10Item != null)
            {
                if (refIDC10Item.IsDataChanged)
                {
                    IsDiagTrmentChanged = true;
                    refIDC10Item.IsDataChanged = false;
                }
            }
            /*▲====: #003*/
            if (!CheckEmptyFields())
            {
                return false;
            }
            /*▼====: #007*/
            if (!CheckCountText())
            {
                return false;
            }
            /*▲====: #007*/
            long lBehaving = 0;
            try
            {
                lBehaving = DiagTrmtItem.PatientServiceRecord.V_Behaving.GetValueOrDefault();
            }
            catch
            {
                MessageBox.Show(eHCMSResources.A0367_G1_Msg_InfoChonTieuDe);
                return false;
            }

            long lPMRTemplateID = 0;
            try
            {
                lPMRTemplateID = DiagTrmtItem.MDRptTemplateID;
            }
            catch
            {
                MessageBox.Show(eHCMSResources.A0337_G1_Msg_InfoChonMauBAn);
                return false;
            }

            DiagTrmtItem.PatientServiceRecord.Staff = Globals.LoggedUserAccount.Staff;
            DiagTrmtItem.PatientServiceRecord.StaffID = Globals.LoggedUserAccount.Staff.StaffID;

            if (CheckedIsMain() && NeedICD10())
            {
                //Khám DV cụ thể nào
                if (Registration_DataStorage.CurrentPatientRegistrationDetail != null && Registration_DataStorage.CurrentPatientRegistrationDetail.PtRegDetailID > 0)
                {
                    DiagTrmtItem.PtRegDetailID = Registration_DataStorage.CurrentPatientRegistrationDetail.PtRegDetailID;
                }
                else
                {
                    DiagTrmtItem.PtRegDetailID = 0;
                }
                //Khám DV cụ thể nào

                DiagTrmtItem.PatientServiceRecord.PatientMedicalRecord.PatientID = Registration_DataStorage.CurrentPatient.PatientID;
                DiagTrmtItem.PatientServiceRecord.StaffID = Globals.LoggedUserAccount.Staff.StaffID;
                DiagTrmtItem.PatientServiceRecord.PtRegistrationID = Registration_DataStorage.CurrentPatientRegistration.PtRegistrationID;
                DiagTrmtItem.DeptLocationID = Globals.DeptLocation.DeptLocationID;

                DiagTrmtItem.PatientServiceRecord.V_RegistrationType = Registration_DataStorage.CurrentPatientRegistration.V_RegistrationType;

                DiagTrmtItem.PatientServiceRecord.V_DiagnosisType = (long)AllLookupValues.V_DiagnosisType.DIAGNOSIS_NORMAL;

                DiagTrmtItem.ICD10List = String.Join(",", from item in refIDC10List
                                                          where (!string.IsNullOrEmpty(item.ICD10Code) && item.DiseasesReference != null)
                                                          select item.ICD10Code);

                PatientRegistration CurRegistration = Registration_DataStorage.CurrentPatientRegistration;
                if (CurRegistration.PtInsuranceBenefit != null && CurRegistration.PtInsuranceBenefit > 0 && refIDC10List != null && refIDC10List.Any(x => x.ICD10Code != null && x.DiseasesReference != null && x.DiseasesReference.ICD10Code.Contains("Z10")))
                {
                    warningtask = new MessageWarningShowDialogTask(content, eHCMSResources.Z0339_G1_TiepTucLuuCDoan);
                    warningtask.Execute(null);
                    if (!warningtask.IsAccept)
                    {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }
        public void ChangeStatesAfterUpdated(bool IsUpdate = false)
        {
            if (IsUpdate)
                FormEditorIsEnabled = false;
            /*▼====: #004*/
            ConsultState = ConsultationState.EditConsultationState;
            /*▲====: #004*/
            StateEdit();
        }
        private bool _IsShowSummaryContent = true;
        public bool IsShowSummaryContent
        {
            get => _IsShowSummaryContent; set
            {
                _IsShowSummaryContent = value;
                NotifyOfPropertyChange(() => IsShowSummaryContent);
                NotifyOfPropertyChange(() => mSaveConsultationState);
                NotifyOfPropertyChange(() => mUpdateConsultationState);
            }
        }
        public void btnAdmRequest()
        {
            //▼===== #015
            if (DiagTrmtItem.V_TreatmentType != (long)AllLookupValues.V_TreatmentType.InPtAdmission)
            {
                MessageBox.Show(eHCMSResources.Z2818_G1_KhongTheDeNghiNV, eHCMSResources.G0442_G1_TBao);
                return;
            }
            //▲===== #015
            GlobalsNAV.ShowDialog<IConsultationOld_InPt>((ConsultationVM) =>
            {
                ConsultationVM.IsDailyDiagnosis = true;

                ConsultationVM.IsForCollectDiagnosis = true;

                ConsultationVM.InPtRegistrationID = Registration_DataStorage.CurrentPatientRegistration.InPtRegistrationID;

                ConsultationVM.Registration_DataStorage = Registration_DataStorage;

                //▼===== 20191017: Cờ để xác định màn hình chẩn đoán hiện tại đang được mở từ đề nghị nhập viện.
                ConsultationVM.IsAdmRequest = true;
                //▲===== 20191017

                ConsultationVM.InitPatientInfo();

                ConsultationVM.IsVisibleAdmRequest = Visibility.Collapsed;
                ConsultationVM.SetDepartment();

                if (DiagTrmtItem.DTItemID > 0 && ConsultationVM.InPtRegistrationID.GetValueOrDefault(0) == 0)
                {
                    var AdmissionDiagTrmtItem = DiagTrmtItem.DeepCopy();
                    AdmissionDiagTrmtItem.DTItemID = 0;
                    AdmissionDiagTrmtItem.PatientServiceRecord.V_Behaving = (long)AllLookupValues.Behaving.YEU_CAU_NHAP_VIEN;
                    AdmissionDiagTrmtItem.Treatment = eHCMSResources.N0221_G1_NhapVien;
                    AdmissionDiagTrmtItem.V_DiagnosisType = (long)AllLookupValues.V_DiagnosisType.DIAGNOSIS_DAILY;
                    var AdmissionIDC10List = refIDC10List.DeepCopy();
                    foreach (var item in AdmissionIDC10List)
                    {
                        item.DiagIcd10ItemID = 0;
                        item.DiagnosisIcd10ListID = 0;
                    }
                    ConsultationVM.UpdateDiagTrmtItemIntoLayout(AdmissionDiagTrmtItem, AdmissionIDC10List, new ObservableCollection<DiagnosisICD9Items>());
                }
            }, null, false, true, new Size(900, 600));
            //RegisterNewInPtRegistration();
        }
        public void Handle(Icd10CollectionSelected message)
        {
            if (message != null && message.Icd10Items != null && message.DiagnosisTreatment != null)
            {
                RegisterNewInPtRegistration(message.Icd10Items, message.DiagnosisTreatment);
            }
        }
        private bool ValidateRegistrationInfo(out ObservableCollection<System.ComponentModel.DataAnnotations.ValidationResult> aValidationResults)
        {
            aValidationResults = new ObservableCollection<System.ComponentModel.DataAnnotations.ValidationResult>();
            if (Registration_DataStorage.CurrentPatient == null)
            {
                var item = new System.ComponentModel.DataAnnotations.ValidationResult(eHCMSResources.Z0148_G1_HayChon1BN, new[] { "CurrentPatient" });
                aValidationResults.Add(item);
            }
            if (Registration_DataStorage.CurrentPatientRegistration.ExamDate == DateTime.MinValue)
            {
                var item = new System.ComponentModel.DataAnnotations.ValidationResult(eHCMSResources.Z0154_G1_NgDKKhongHopLe, new[] { "ExamDate" });
                aValidationResults.Add(item);
            }
            if (aValidationResults.Count > 0)
            {
                return false;
            }
            return true;
        }
        //▼===== #014
        private bool CheckAllowCrossRegion(PatientRegistration mRegistration)
        {
            if (mRegistration == null || mRegistration.HealthInsurance == null)
            {
                return false;
            }
            bool ThongTuyen = false;
            ThongTuyen = Globals.CheckAllowToCrossRegion(mRegistration.HealthInsurance, AllLookupValues.RegistrationType.NOI_TRU);
            if (!ThongTuyen)
            {
                return false;
            }
            return true;
        }
        //▲===== #014
        private void RegisterNewInPtRegistration(IList<DiagnosisIcd10Items> Icd10Items, DiagnosisTreatment aDiagnosisTreatment)
        {
            ObservableCollection<System.ComponentModel.DataAnnotations.ValidationResult> mValidationResults;
            bool isValid = ValidateRegistrationInfo(out mValidationResults);
            if (!isValid)
            {
                Action<IValidationError> onInitDlg = delegate (IValidationError errorVm)
                {
                    errorVm.SetErrors(mValidationResults);
                };
                GlobalsNAV.ShowDialog<IValidationError>(onInitDlg);
                return;
            }
            PatientRegistration mRegistration = Registration_DataStorage.CurrentPatientRegistration.DeepCopy();
            mRegistration.DiagnosisTreatment = aDiagnosisTreatment;
            mRegistration.PtRegistrationTransferID = mRegistration.PtRegistrationID;
            mRegistration.PtRegistrationID = 0;
            if (mRegistration.HealthInsurance != null)
                mRegistration.V_RegForPatientOfType = AllLookupValues.V_RegForPatientOfType.NBNT_BN_CO_BHYT;
            else
                mRegistration.V_RegForPatientOfType = AllLookupValues.V_RegForPatientOfType.NBNT_BN_KHONG_BHYT;
            if (mRegistration.HealthInsurance == null)
            {
                mRegistration.IsCrossRegion = null;
            }
            else
            {
                //▼====== #014
                if (!CheckAllowCrossRegion(mRegistration))
                {
                    mRegistration.IsCrossRegion = true;
                }
                else
                {
                    mRegistration.IsCrossRegion = false;
                    mRegistration.IsAllowCrossRegion = true;
                }
                //▲====== #014
            }
            mRegistration.RegTypeID = (byte)PatientRegistrationType.DK_KHAM_BENH_NOI_TRU;
            mRegistration.V_RegistrationType = AllLookupValues.RegistrationType.NOI_TRU;
            mRegistration.PatientClassification = mRegistration.HealthInsurance != null ? new PatientClassification { PatientClassID = 2 } : new PatientClassification { PatientClassID = 1 };
            mRegistration.RegistrationStatus = AllLookupValues.RegistrationStatus.PENDING_INPT;

            //20190402 TTM: Sửa code chỗ này lại vì đang gán sai giá trị. 
            //mRegistration.RefDepartment = mRegistration.RefDepartment;
            mRegistration.RefDepartment = aDiagnosisTreatment.Department;

            mRegistration.HIApprovedStaffID = Globals.LoggedUserAccount.StaffID;
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginSaveEmptyRegistration_V2(Globals.LoggedUserAccount.StaffID.GetValueOrDefault(),
                            Globals.DeptLocation.DeptLocationID, null, mRegistration, mRegistration.HealthInsurance != null ? (long)AllLookupValues.RegistrationType.NOI_TRU_BHYT : (long)AllLookupValues.RegistrationType.NOI_TRU,
                            Icd10Items, true,
                            Globals.DispatchCallback(asyncResult =>
                            {
                                try
                                {
                                    PatientRegistration mOutRegistration = null;
                                    contract.EndSaveEmptyRegistration_V2(out mOutRegistration, asyncResult);
                                    if (mOutRegistration != null && mOutRegistration.PtRegistrationID > 0)
                                    {
                                        //20190402 TTM: Vì hàm SaveEmptyRegistration_V2 đi lưu đăng ký nội trú nên mOutRegistration sẽ out ra đăng ký nội trú của đăng ký ngoại trú này
                                        //              Vì thế lấy biến mOutRegistration.PtRegistrationID gán vào InPtRegistationID là chính xác.
                                        //              Mục đích là để không cần load lại đăng ký vẫn có Id nội trú để người dùng click lại vào nút Đề nghị sẽ lấy thông tin chẩn đoán nhập viện của BN
                                        Registration_DataStorage.CurrentPatientRegistration.InPtRegistrationID = mOutRegistration.PtRegistrationID;
                                    }
                                    MessageBox.Show(eHCMSResources.A0468_G1_Msg_InfoLuuOK, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                }
                                catch (FaultException<AxException> fault)
                                {
                                    MessageBox.Show(fault.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
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

        private bool _IsUpdateFromPresciption = false;
        public bool IsUpdateFromPresciption
        {
            get => _IsUpdateFromPresciption; set
            {
                _IsUpdateFromPresciption = value;
                NotifyOfPropertyChange(() => IsUpdateFromPresciption);
                NotifyOfPropertyChange(() => IsShowEditTinhTrangTheChat);
                NotifyOfPropertyChange(() => btGCTIsEnabled);
                NotifyOfPropertyChange(() => btGCT_CLS_IsEnabled);
            }
        }
        private Dictionary<string, string> _ShortHandDictionaryObj;
        public Dictionary<string, string> ShortHandDictionaryObj
        {
            get => _ShortHandDictionaryObj; set
            {
                _ShortHandDictionaryObj = value;
                NotifyOfPropertyChange(() => ShortHandDictionaryObj);
            }
        }
        public void GetShortHandDictionaries(long StaffID)
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                using (var serviceFactory = new CommonService_V2Client())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetShortHandDictionariesByStaffID(StaffID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var mShortHandDictionaries = contract.EndGetShortHandDictionariesByStaffID(asyncResult);
                            if (mShortHandDictionaries == null)
                            {
                                ShortHandDictionaryObj = new Dictionary<string, string>();
                            }
                            else
                            {
                                ShortHandDictionaryObj = mShortHandDictionaries.ToDictionary(x => x.ShortHandDictionaryKey.ToString().ToLower(), x => x.ShortHandDictionaryValue.ToString());
                            }
                        }
                        catch (Exception ex)
                        {
                            GlobalsNAV.ShowMessagePopup(ex.Message);
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
        public ICD10Changed gICD10Changed { get; set; }
        public void ICD10Changed(ObservableCollection<DiagnosisIcd10Items> ICD10List)
        {
            gICD10Changed?.Invoke(ICD10List);
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
        //▼===== #017
        KeySearchableComboBox kscV_TreatmentType = null;
        public void KscV_TreatmentType_Loaded(object sender, RoutedEventArgs e)
        {
            kscV_TreatmentType = (KeySearchableComboBox)sender;
        }
        //▲===== #017}
    }
}