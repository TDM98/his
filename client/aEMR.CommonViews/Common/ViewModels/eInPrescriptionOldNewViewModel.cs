using eHCMSLanguage;
using System.Linq;
using System.ComponentModel.Composition;
using System.ServiceModel;
using Caliburn.Micro;
using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using DataEntities;
using System.Threading;
using System;
using System.Windows.Media;
using System.Windows.Input;
using System.Text;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.ComponentModel;
using aEMR.Infrastructure.Events;
using aEMR.Common.BaseModel;
using aEMR.ViewContracts;
using Castle.Windsor;
using aEMR.Infrastructure.CachingUtils;
using aEMR.Infrastructure;
using aEMR.Common.Views;
using aEMR.Common.Collections;
using aEMR.CommonTasks;
using aEMR.ServiceClient;
using aEMR.ServiceClient.Consultation_PCLs;
using aEMR.Common.Printing;
using aEMR.Controls;
using aEMR.DataContracts;
using static aEMR.Infrastructure.Events.TransferFormEvent;

/*
 * 20171220 #001 CMN: Fixed Print Empty Prescriptions
 * 20191107 #002 TBL: Task #1278: Nếu BN đã có toa thuốc xuất viện cũ thì lúc load toa thuốc cũ lên thì phải lấy chẩn đoán và cách điều trị của toa cũ lên 
 *                                chứ không lấy chẩn đoán và cách điều trị cuối cùng của đợt điều trị nội trú hiện tại
 * 20200110 #003 TBL: BM 0021795: Cho phép người dùng chọn lại chẩn đoán xác nhận khi đã có chẩn đoán xác nhận rồi
 * 20200708 #004 TTM: BM 0039309: Lỗi không tự động lấy chẩn đoán cuối cùng làm chẩn đoán xuất viện ở màn hình ra toa và màn hình lưu thông tin xuất viện
 * 20200808 #005 TTM: BM 0039412: Fix lỗi chết chương trình khi đang gõ autocomplete hiện ra nhưng bấm liền nút bỏ qua => Chết chương trình.
 *                                Nguyên nhân: Lúc bấm nút bỏ qua selectedIndex của Grid chuyển về -1 => Chết do out of range index.
 * 20200813 #006 TTM: BM 0041453: Lỗi lấy lấy sai chẩn đoán XV.
 * 20200915 #007 TNHX: Add parameter for print direct prescriptions
 * 20220909 #008 DatTB: Thêm event load "Lời dặn" qua tab Toa thuốc xuất viện
 * 20220922 #009 BLQ: Kiểm tra số ngày cấp toa theo cấu hình
 * 20220928 #010 BLQ: Kiểm tra nhập lời dặn khi lưu toa thuốc
 * 20221006 #011 BLQ: Đổi kiểm tra thuốc trong kho từ cảnh báo sang chặn không cho phát hành toa thuốc
 * 20221216 #012 BLQ: Thêm đường dùng và khoảng cách dùng
 * 20230130 #013 QTD: Xem/In thuốc HT/GN
 * 20230629 #014 BLQ: Kiểm tra lời dặn có ký tự '<' và '>'
*/
namespace aEMR.Common.ViewModels
{
    [Export(typeof(IeInPrescriptionOldNew)), PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class eInPrescriptionOldNewViewModel : ViewModelBase, IeInPrescriptionOldNew
        , IHandle<ePrescriptionDoubleClickEvent_InPt_2>
        , IHandle<SendPrescriptionDetailSchedulesEvent<ObservableCollection<PrescriptionDetailSchedules>, bool, double, double, string, int>>
        , IHandle<LoadPrescriptionInPtAfterSaved>
        , IHandle<LoadDiagnosisTreatmentConfirmedForDischarge>
        , IHandle<OnChangedUpdateAdmDisDetails> //#008
    {
        IEventAggregator _eventArg;
        [ImportingConstructor]
        public eInPrescriptionOldNewViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg, ISalePosCaching salePosCaching)
        {
            _eventArg = eventArg;
            UCAllergiesWarningByPatientID = Globals.GetViewModel<IAllergiesWarning_ByPatientID>();
            DepartmentContent = Globals.GetViewModel<IDepartmentListing>();
            //▼===== 20191017 TTM: Đọc dữ liệu 1 lần khi vào màn hình thay vì đọc nhiều lần khi tìm bệnh nhân
            DataSetOneTimesForView();
            //▲===== 
            DepartmentContent.AddSelectOneItem = true;
            (DepartmentContent as PropertyChangedBase).PropertyChanged += new PropertyChangedEventHandler(DepartmentContent_PropertyChanged);
            //▼====: #012
            ListV_ReconmendTimeUsageDistance = Globals.AllLookupValueList.Where(x => x.ObjectTypeID == (long)LookupValues.V_ReconmendTimeUsageDistance).ToObservableCollection();
            //▲====: #012
        }
        protected override void OnActivate()
        {
            base.OnActivate();
            _eventArg.Subscribe(this);
        }
        protected override void OnDeactivate(bool close)
        {
            _eventArg.Unsubscribe(this);
            base.OnDeactivate(close);
        }
        private void DepartmentContent_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SelectedItem")
            {
                LatestePrecriptions.Department = DepartmentContent.SelectedItem;
            }
        }
        //▼===== 20191017 TTM: Những dữ liệu chỉ cần đọc 1 lần khi vào màn hình khám bệnh nội trú.
        //                     Thay vì đọc nhiều lần khi load bệnh nhân
        private void DataSetOneTimesForView()
        {
            authorization();
            DrugTypes = new ObservableCollection<Lookup>();
            ChooseDoses = new ObservableCollection<ChooseDose>();
            GetAllContrainIndicatorDrugs();
            ObjPrescriptionNoteTemplates_GetAll = new ObservableCollection<PrescriptionNoteTemplates>();
            LieuDung = Globals.allPrescriptionDetailSchedulesLieuDung.ToObservableCollection();
            RefreshLookup();
            GetAllLookupForDrugTypes();

            Coroutine.BeginExecute(DoGetStore_EXTERNAL());
        }
        //▲====== 20191017 
        public void GetInitDataInfo()
        {
            this.ShowBusyIndicator();

            //▼===== 20191017 TTM
            //authorization();
            //DrugTypes = new ObservableCollection<Lookup>();
            //ChooseDoses = new ObservableCollection<ChooseDose>();
            LatestePrecriptions = new Prescription();

            //GetAllContrainIndicatorDrugs();
            //ObjPrescriptionNoteTemplates_GetAll = new ObservableCollection<PrescriptionNoteTemplates>();
            //LieuDung = Globals.allPrescriptionDetailSchedulesLieuDung.ToObservableCollection();
            //RefreshLookup();
            //GetAllLookupForDrugTypes();

            //Coroutine.BeginExecute(DoGetStore_EXTERNAL());
            //▲===== 20191017 TTM
            DrugList = new PagedSortableCollectionView<GetDrugForSellVisitor>();
            DrugList.PageSize = 10;
            DrugList.OnRefresh += new EventHandler<aEMR.Common.Collections.RefreshEventArgs>(DrugList_OnRefresh);

            initPatientInfo();

            this.HideBusyIndicator();

        }
        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            if (this.GetView() != null)
            {
                eInPrescriptionOldNewView thisView = (eInPrescriptionOldNewView)view;
                if (thisView.tbSoTuan != null)
                {
                    CtrtbSoTuan = thisView.tbSoTuan;
                }

                GetInitDataInfo();
            }
            else
            {
                MessageBox.Show(eHCMSResources.A0542_G1_Msg_InfoEnglish1, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
            }
        }
        private bool HasDiagnosisOutHospital()
        {
            if (Registration_DataStorage == null || Registration_DataStorage.PatientServiceRecordCollection == null || Registration_DataStorage.PatientServiceRecordCollection.Count <= 0) // Chưa có chẩn đoán xuất viện             
            {
                return false;
            }

            PatientServiceRecord ptServiceRecord = Registration_DataStorage.PatientServiceRecordCollection.FirstOrDefault();

            if (ptServiceRecord.DiagnosisTreatments == null || ptServiceRecord.DiagnosisTreatments.Count <= 0)
            {
                return false;
            }

            if (ptServiceRecord.DiagnosisTreatments.Any(x => x.V_DiagnosisType == (long)AllLookupValues.V_DiagnosisType.DIAGNOSIS_OUTHOS)
                || (Globals.ServerConfigSection.ConsultationElements.UseOnlyDailyDiagnosis && ptServiceRecord.DiagnosisTreatments != null && ptServiceRecord.DiagnosisTreatments.Any(x => !string.IsNullOrEmpty(x.DiagnosisFinal))))
            {
                return true;
            }

            return false;
        }

        public void CheckBeforePrescrip()
        {
            this.ShowBusyIndicator();
            if (!Globals.isConsultationStateEdit)
            {
                MessageBox.Show(string.Format("{0}.", eHCMSResources.Z0391_G1_BNDuocChonTuLSBA));
                IsEnabledForm = false;
                HasDiagnosis = false;
                ObjDiagnosisTreatment_Current = new DiagnosisTreatment();

                btChonChanDoanIsEnabled = false;
                btnSaveAddNewIsEnabled = false;
                IsEnabledPrint = false;
                //return;
            }

            if (Globals.PatientFindBy_ForConsultation.Value != AllLookupValues.PatientFindBy.NOITRU)
            {
                IsEnabledForm = false;

                MessageBox.Show(eHCMSResources.Z0677_G1_BNKgPhaiNoiTru, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);

                this.HideBusyIndicator();

                return;
            }

            //KMx: Kiểm tra chẩn đoán xuất viện theo cách mới, vì sửa lại 1 service record có thể chứa nhiều chẩn đoán (09/06/2015 17:55).
            //if (Registration_DataStorage.PatientServiceRecordCollection == null || Registration_DataStorage.PatientServiceRecordCollection.Count < 1) // Chưa có chẩn đoán            
            //if (Registration_DataStorage.PatientServiceRecordCollection == null || !Registration_DataStorage.PatientServiceRecordCollection.Any(x => x.V_DiagnosisType == (long)AllLookupValues.V_DiagnosisType.DIAGNOSIS_OUTHOS)) // Chưa có chẩn đoán xuất viện             

            //KMx: Mỗi đăng ký nội trú chỉ có 1 service record chứa tất cả chẩn đoán (08/06/2015 10:31).
            //KMx: Phải để DeepCopy, nếu không thì dữ liệu gốc sẽ bị thay đổi khi người dùng nhấn vào link Ra Toa nhiều lần (21/05/2015 17:30).
            //ObjPatientServiceRecord_Current = Registration_DataStorage.PatientServiceRecordCollection.Where(x => x.V_DiagnosisType == (long)AllLookupValues.V_DiagnosisType.DIAGNOSIS_OUTHOS).FirstOrDefault().DeepCopy();

            ObjPatientServiceRecord_Current = Registration_DataStorage.PatientServiceRecordCollection == null ? null : Registration_DataStorage.PatientServiceRecordCollection.FirstOrDefault().DeepCopy();

            //KMx: Đã có kiểm tra trong function HasDiagnosisOutHospital().
            //if (ObjPatientServiceRecord_Current.DiagnosisTreatments == null || ObjPatientServiceRecord_Current.DiagnosisTreatments.Count <= 0)
            //{
            //    MessageBox.Show(eHCMSResources.A0730_G1_Msg_InfoKhTimThayCDCuaBN, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
            //    return;
            //}
            //20191120 TBL: Nếu đăng ký này đã có chẩn đoán xuất viện thì lấy chẩn đoán xuất viện đó cho xác nhận chẩn đoán toa thuốc
            if(ObjPatientServiceRecord_Current != null)
            {
                if (ObjPatientServiceRecord_Current.DiagnosisTreatments != null
                    && ObjPatientServiceRecord_Current.DiagnosisTreatments.Count > 0)
                {
                    foreach (var dt in ObjPatientServiceRecord_Current.DiagnosisTreatments)
                    {
                        if (dt.ConfimedForPreAndDischarge.GetValueOrDefault(0) > 0)
                        {
                            CurrentDiagnosisTreatment = dt;
                            break;
                        }
                    }
                }
                //ObjDiagnosisTreatment_Current = ObjPatientServiceRecord_Current.DiagnosisTreatments[0];
                if (Globals.ServerConfigSection.ConsultationElements.UseOnlyDailyDiagnosis
                    && ObjPatientServiceRecord_Current.DiagnosisTreatments != null)
                {
                    ObjDiagnosisTreatment_Current = ObjPatientServiceRecord_Current.DiagnosisTreatments.Count > 0 ?
                        ObjPatientServiceRecord_Current.DiagnosisTreatments.OrderBy(x => x.DTItemID).LastOrDefault() :
                        ObjPatientServiceRecord_Current.DiagnosisTreatments.Where(x => x.V_DiagnosisType == (long)AllLookupValues.V_DiagnosisType.DIAGNOSIS_OUTHOS).FirstOrDefault();
                }

                HasDiagnosis = true;

                //KMx: Không cần set HisID, để khi lưu set 1 lần luôn (21/05/2015 16:18)
                // TxD 11/04/2014 Changed from HisIDVisibility to RegistrationCoverByHI
                //if (RegistrationCoverByHI())    //neu dang ky co bao hiem
                //{
                //    LatestePrecriptions.PrescriptionIssueHistory.HisID = Registration_DataStorage.CurrentPatientRegistration.HisID.Value;
                //}

                btnUndoIsEnabled = false;
                btnUpdateIsEnabled = false;

                //chưa có toa thuốc
                if (ObjPatientServiceRecord_Current.PrescriptionIssueHistories == null
                    || ObjPatientServiceRecord_Current.PrescriptionIssueHistories.Count < 1)
                {
                    PrescripState = PrescriptionState.NewPrescriptionState;

                    //Đọc Toa Thuốc Cuối lên nếu có
                    //GetPrescriptionTypes_New();
                    GetPrescription(false);
                }
                else//đã có toa thuốc
                {

                    GetPrescription(true);

                    if (ObjPatientServiceRecord_Current.PrescriptionIssueHistories[0].IssuerStaffID != Globals.LoggedUserAccount.StaffID)
                    {
                        //Chỉ Có Người Tạo Ra Toa Thuốc này mới được phép Chỉnh Sửa!
                        PrescripState = PrescriptionState.PublishNewPrescriptionState;
                    }
                    else
                    {
                        PrescripState = PrescriptionState.EditPrescriptionState;
                    }

                }
            }
            this.HideBusyIndicator();
        }

        private void LoadDepartment()
        {
            ObservableCollection<long> ListDeptID = new ObservableCollection<long>();

            if (Registration_DataStorage.CurrentPatientRegistration.AdmissionInfo != null && Registration_DataStorage.CurrentPatientRegistration.AdmissionInfo.InPatientDeptDetails.Count > 0)
            {
                foreach (var inDeptItem in Registration_DataStorage.CurrentPatientRegistration.AdmissionInfo.InPatientDeptDetails)
                {
                    if (!ListDeptID.Any(x => x == inDeptItem.DeptLocation.DeptID))
                    {
                        ListDeptID.Add(inDeptItem.DeptLocation.DeptID);
                    }
                }
            }

            DepartmentContent.LstRefDepartment = ListDeptID;
            DepartmentContent.LoadData();
        }

        public void initPatientInfo()
        {
            //Kiem Tra Đăng ký có BH?
            this.ShowBusyIndicator();

            xNgayBHToiDa_NgoaiTru = Globals.ServerConfigSection.HealthInsurances.PharmacyMaxDaysHIRebate_NgoaiTru;
            xNgayBHToiDa_NoiTru = Globals.ServerConfigSection.HealthInsurances.PharmacyMaxDaysHIRebate_NoiTru;

            #region cu
            if (Registration_DataStorage != null && Registration_DataStorage.CurrentPatient != null)
            {
                NoiTru = IsBenhNhanNoiTru();
                BH = RegistrationCoverByHI();

                if (NoiTru)
                {
                    xNgayBHToiDa = xNgayBHToiDa_NoiTru;
                }
                else
                {
                    xNgayBHToiDa = xNgayBHToiDa_NgoaiTru;
                }
                IsEnabledForm = true;
                loadPrescript = true;

                GetPrescriptionTypeList();

                //Danh sach Di Ung
                MDAllergies_ByPatientID(Registration_DataStorage.CurrentPatient.PatientID, 1);
                //Danh sach Di Ung

                InitChooseDose();

                GetMedConditionByPtID(Registration_DataStorage.CurrentPatient.PatientID, -1);

                ObjPrescriptionNoteTemplates_Selected = new PrescriptionNoteTemplates();
                ObjPrescriptionNoteTemplates_Selected.PrescriptNoteTemplateID = -1;

                if (Registration_DataStorage.CurrentPatientRegistration != null && Registration_DataStorage.CurrentPatientRegistration.PtRegistrationID > 0)
                {
                    //KMx: Load những khoa mà BN đã nằm (07/03/2015 10:58).
                    LoadDepartment();

                    //GetDiagnosisTreatmentByPtID(Registration_DataStorage.CurrentPatient.PatientID, Registration_DataStorage.CurrentPatientRegistration.PtRegistrationID, "", 1, true);
                    CheckBeforePrescrip();
                }

                loadPrescript = true;
            }
            else
            {
                IsEnabledForm = false;
            }
            #endregion
            this.HideBusyIndicator();
        }

        #region busy indicator
        public override bool IsProcessing
        {
            get
            {
                return false;
            }
        }
        public override string StatusText
        {
            get
            {
                if (_isWaitingSaveDuocSiEdit)
                {
                    return eHCMSResources.Z0343_G1_DangLuu;
                }
                if (_IsWaitingGetPrescriptionDetailsByPrescriptID)
                {
                    return eHCMSResources.Z1047_G1_TTinCTietToaCuoi;
                }
                if (_IsWaitingGetLatestPrescriptionByPtID)
                {
                    return eHCMSResources.Z1048_G1_TTinToaCuoi;
                }
                if (_IsWaitingGetPrescriptionTypes)
                {
                    return eHCMSResources.T2837_G1_LoaiToa;
                }
                if (_IsWaitingGetMedConditionByPtID)
                {
                    return eHCMSResources.K2871_G1_DangLoadDLieu;
                }
                if (_IsWaitingAddPrescriptIssueHistory)
                {
                    return eHCMSResources.Z1049_G1_PhatHanhLaiToa;
                }
                if (_IsWaitingCapNhatToaThuoc)
                {
                    return eHCMSResources.Z1050_G1_CNhatToa;
                }
                if (_IsWaitingPrescriptions_UpdateDoctorAdvice)
                {
                    return eHCMSResources.K1661_G1_CNhatLoiDan;
                }
                if (_IsWaitingTaoThanhToaMoi)
                {
                    return eHCMSResources.Z1051_G1_DangLuuToa;
                }
                if (_IsWaitingGetAllContrainIndicatorDrugs)
                {
                    return eHCMSResources.K2882_G1_DangTaiDLieu;
                }
                if (_IsWaitingGetDiagnosisTreatmentByPtID)
                {
                    return eHCMSResources.Z1052_G1_DangLayTTinCDoan;
                }
                if (_IsWaitingPrescriptionNoteTemplates_GetAll)
                {
                    return eHCMSResources.Z1053_G1_DangLayDSLoiDan;
                }
                if (_IsWaitingMDAllergies_ByPatientID)
                {
                    return eHCMSResources.Z1054_G1_DangLayDSThuocDiUng;
                }
                return string.Empty;
            }
        }

        private bool _AllowUpdateThoughReturnDrugNotEnough = false;
        public bool AllowUpdateThoughReturnDrugNotEnough
        {
            get { return _AllowUpdateThoughReturnDrugNotEnough; }
            set { _AllowUpdateThoughReturnDrugNotEnough = value; }
        }

        private bool _loadPrescript = false;
        public bool loadPrescript
        {
            get { return _loadPrescript; }
            set
            {
                if (_loadPrescript != value)
                {
                    _loadPrescript = value;
                    NotifyWhenBusy();
                    NotifyOfPropertyChange(() => loadPrescript);
                    NotifyOfPropertyChange(() => IsProcessing);
                }
            }
        }

        private bool _IsWaitingMDAllergies_ByPatientID;
        public bool IsWaitingMDAllergies_ByPatientID
        {
            get { return _IsWaitingMDAllergies_ByPatientID; }
            set
            {
                if (_IsWaitingMDAllergies_ByPatientID != value)
                {
                    _IsWaitingMDAllergies_ByPatientID = value;
                    NotifyOfPropertyChange(() => IsWaitingMDAllergies_ByPatientID);
                    NotifyWhenBusy();
                }
            }
        }


        private bool _IsWaitingPrescriptionNoteTemplates_GetAll;
        public bool IsWaitingPrescriptionNoteTemplates_GetAll
        {
            get { return _IsWaitingPrescriptionNoteTemplates_GetAll; }
            set
            {
                if (_IsWaitingPrescriptionNoteTemplates_GetAll != value)
                {
                    _IsWaitingPrescriptionNoteTemplates_GetAll = value;
                    NotifyOfPropertyChange(() => IsWaitingPrescriptionNoteTemplates_GetAll);
                    NotifyWhenBusy();
                }
            }
        }


        private bool _isWaitingSaveDuocSiEdit;
        public bool IsWaitingSaveDuocSiEdit
        {
            get { return _isWaitingSaveDuocSiEdit; }
            set
            {
                if (_isWaitingSaveDuocSiEdit != value)
                {
                    _isWaitingSaveDuocSiEdit = value;
                    NotifyOfPropertyChange(() => IsWaitingSaveDuocSiEdit);
                    NotifyWhenBusy();

                }
            }
        }

        private bool _IsWaitingGetPrescriptionDetailsByPrescriptID;
        public bool IsWaitingGetPrescriptionDetailsByPrescriptID
        {
            get { return _IsWaitingGetPrescriptionDetailsByPrescriptID; }
            set
            {
                if (_IsWaitingGetPrescriptionDetailsByPrescriptID != value)
                {
                    _IsWaitingGetPrescriptionDetailsByPrescriptID = value;
                    NotifyOfPropertyChange(() => IsWaitingGetPrescriptionDetailsByPrescriptID);
                    NotifyWhenBusy();
                }
            }
        }


        private bool _IsWaitingGetLatestPrescriptionByPtID;
        public bool IsWaitingGetLatestPrescriptionByPtID
        {
            get { return _IsWaitingGetLatestPrescriptionByPtID; }
            set
            {
                if (_IsWaitingGetLatestPrescriptionByPtID != value)
                {
                    _IsWaitingGetLatestPrescriptionByPtID = value;
                    NotifyOfPropertyChange(() => IsWaitingGetLatestPrescriptionByPtID);
                    NotifyWhenBusy();
                }
            }
        }


        private bool _IsWaitingChooseDose;
        public bool IsWaitingChooseDose
        {
            get { return _IsWaitingChooseDose; }
            set
            {
                if (_IsWaitingChooseDose != value)
                {
                    _IsWaitingChooseDose = value;
                    NotifyOfPropertyChange(() => IsWaitingChooseDose);
                    NotifyWhenBusy();
                }
            }
        }


        private bool _IsWaitingGetPrescriptionTypes;
        public bool IsWaitingGetPrescriptionTypes
        {
            get { return _IsWaitingGetPrescriptionTypes; }
            set
            {
                if (_IsWaitingGetPrescriptionTypes != value)
                {
                    _IsWaitingGetPrescriptionTypes = value;
                    NotifyOfPropertyChange(() => IsWaitingGetPrescriptionTypes);
                    NotifyWhenBusy();
                }
            }
        }

        private bool _IsWaitingGetMedConditionByPtID;
        public bool IsWaitingGetMedConditionByPtID
        {
            get { return _IsWaitingGetMedConditionByPtID; }
            set
            {
                if (_IsWaitingGetMedConditionByPtID != value)
                {
                    _IsWaitingGetMedConditionByPtID = value;
                    NotifyOfPropertyChange(() => IsWaitingGetMedConditionByPtID);
                    NotifyWhenBusy();
                }
            }
        }


        private bool _IsWaitingAddPrescriptIssueHistory;
        public bool IsWaitingAddPrescriptIssueHistory
        {
            get { return _IsWaitingAddPrescriptIssueHistory; }
            set
            {
                if (_IsWaitingAddPrescriptIssueHistory != value)
                {
                    _IsWaitingAddPrescriptIssueHistory = value;
                    NotifyOfPropertyChange(() => IsWaitingAddPrescriptIssueHistory);
                    NotifyWhenBusy();
                }
            }
        }

        private bool _IsWaitingCapNhatToaThuoc;
        public bool IsWaitingCapNhatToaThuoc
        {
            get { return _IsWaitingCapNhatToaThuoc; }
            set
            {
                if (_IsWaitingCapNhatToaThuoc != value)
                {
                    _IsWaitingCapNhatToaThuoc = value;
                    NotifyOfPropertyChange(() => IsWaitingCapNhatToaThuoc);
                    NotifyWhenBusy();
                }
            }
        }

        private bool _IsWaitingPrescriptions_UpdateDoctorAdvice;
        public bool IsWaitingPrescriptions_UpdateDoctorAdvice
        {
            get { return _IsWaitingPrescriptions_UpdateDoctorAdvice; }
            set
            {
                if (_IsWaitingPrescriptions_UpdateDoctorAdvice != value)
                {
                    _IsWaitingPrescriptions_UpdateDoctorAdvice = value;
                    NotifyOfPropertyChange(() => IsWaitingPrescriptions_UpdateDoctorAdvice);
                    NotifyWhenBusy();
                }
            }
        }

        private bool _IsWaitingTaoThanhToaMoi;
        public bool IsWaitingTaoThanhToaMoi
        {
            get { return _IsWaitingTaoThanhToaMoi; }
            set
            {
                if (_IsWaitingTaoThanhToaMoi != value)
                {
                    _IsWaitingTaoThanhToaMoi = value;
                    NotifyOfPropertyChange(() => IsWaitingTaoThanhToaMoi);
                    NotifyWhenBusy();
                }
            }
        }

        private bool _IsWaitingGetAllContrainIndicatorDrugs;
        public bool IsWaitingGetAllContrainIndicatorDrugs
        {
            get { return _IsWaitingGetAllContrainIndicatorDrugs; }
            set
            {
                if (_IsWaitingGetAllContrainIndicatorDrugs != value)
                {
                    _IsWaitingGetAllContrainIndicatorDrugs = value;
                    NotifyOfPropertyChange(() => IsWaitingGetAllContrainIndicatorDrugs);
                    NotifyWhenBusy();
                }
            }
        }

        #endregion

        private IePrescriptionCommentaryAutoComplete _ePreComAutoCompleteContent;

        public IePrescriptionCommentaryAutoComplete ePreComAutoCompleteContent
        {
            get { return _ePreComAutoCompleteContent; }
            set
            {
                if (_ePreComAutoCompleteContent != value)
                {
                    _ePreComAutoCompleteContent = value;
                    NotifyOfPropertyChange(() => ePreComAutoCompleteContent);
                }
            }
        }

        public enum DataGridCol
        {
            DEL = 0,
            HI = DEL + 1,
            Schedule = HI + 1,
            NotInCat = Schedule + 1,
            DRUG_NAME = NotInCat + 1,
            UNITS = DRUG_NAME + 1,
            UNITUSE = UNITS + 1,
            DRUG_TYPE = UNITUSE + 1,

            //MDOSE = DRUG_TYPE + 1,
            //NDOSE = MDOSE + 1,
            //ADOSE = NDOSE + 1,
            //EDOSE = ADOSE + 1,

            MDOSE = DRUG_TYPE + 1,
            ADOSE = MDOSE + 1,
            EDOSE = ADOSE + 1,
            NDOSE = EDOSE + 1,

            DayTotalCol = NDOSE + 1,

            QTY = DayTotalCol + 1,
            USAGE = QTY + 1,
            INSTRUCTION = USAGE + 1
        }

        #region Radio Button Number Of Copies

        private string _numberOfTimesPrint;

        private bool _numberOfTimesPrintVisibility = false;

        private bool _isTypingNumberOfCopies = false;


        //KMx: Không để NotifyOfPropertyChange. Lý do: Để khi tick In 1 lần -> 3 lần, thì không hiện số lần vào ô "Khác" (14/01/2016 09:32).
        public string NumberOfTimesPrint
        {
            get
            {
                return _numberOfTimesPrint;
            }
            set
            {
                if (_numberOfTimesPrint != value && value != "")
                {
                    _numberOfTimesPrint = value;
                }
            }
        }

        public bool NumberOfTimesPrintVisibility
        {
            get
            {
                return _numberOfTimesPrintVisibility && mToaThuocDaPhatHanh_In;
            }
            set
            {
                if (_numberOfTimesPrintVisibility != value)
                {
                    _numberOfTimesPrintVisibility = value;
                    NotifyOfPropertyChange(() => NumberOfTimesPrintVisibility);
                }
            }
        }

        public bool IsTypingNumberOfCopies
        {
            get
            {
                return _isTypingNumberOfCopies;
            }
            set
            {
                if (_isTypingNumberOfCopies != value)
                {
                    _isTypingNumberOfCopies = value;
                    NotifyOfPropertyChange(() => IsTypingNumberOfCopies);
                }
            }
        }

        public enum NumberOfCopies
        {
            OneTime = 1,
            TwoTimes = 2,
            ThreeTimes = 3,
            Other = 0
        }

        public void GroupPrint_Loaded(object sender, RoutedEventArgs e)
        {
            if (Registration_DataStorage.CurrentPatientRegistration != null && Registration_DataStorage.CurrentPatientRegistration.HisID.GetValueOrDefault() > 0)
            {
                if (Globals.ServerConfigSection.CommonItems.DefaultNumOfCopyPrescriptHIPT <= 0)
                {
                    return;
                }
                else if (Globals.ServerConfigSection.CommonItems.DefaultNumOfCopyPrescriptHIPT == 1)
                {
                    ((eInPrescriptionOldNewView)this.GetView()).Print1.IsChecked = true;
                    NumberOfTimesPrint = ((int)NumberOfCopies.OneTime).ToString();
                }
                else if (Globals.ServerConfigSection.CommonItems.DefaultNumOfCopyPrescriptHIPT == 2)
                {
                    ((eInPrescriptionOldNewView)this.GetView()).Print2.IsChecked = true;
                    NumberOfTimesPrint = ((int)NumberOfCopies.TwoTimes).ToString();
                }
                else if (Globals.ServerConfigSection.CommonItems.DefaultNumOfCopyPrescriptHIPT == 3)
                {
                    ((eInPrescriptionOldNewView)this.GetView()).Print3.IsChecked = true;
                    NumberOfTimesPrint = ((int)NumberOfCopies.ThreeTimes).ToString();
                }
                else
                {
                    ((eInPrescriptionOldNewView)this.GetView()).Print4.IsChecked = true;
                    ((eInPrescriptionOldNewView)this.GetView()).Print5.Text = Globals.ServerConfigSection.CommonItems.DefaultNumOfCopyPrescriptHIPT.ToString();
                }
            }
            else
            {
                if (Globals.ServerConfigSection.CommonItems.DefaultNumOfCopyPrescriptNormalPT <= 0)
                {
                    return;
                }
                else if (Globals.ServerConfigSection.CommonItems.DefaultNumOfCopyPrescriptNormalPT == 1)
                {
                    ((eInPrescriptionOldNewView)this.GetView()).Print1.IsChecked = true;
                    NumberOfTimesPrint = ((int)NumberOfCopies.OneTime).ToString();
                }
                else if (Globals.ServerConfigSection.CommonItems.DefaultNumOfCopyPrescriptNormalPT == 2)
                {
                    ((eInPrescriptionOldNewView)this.GetView()).Print2.IsChecked = true;
                    NumberOfTimesPrint = ((int)NumberOfCopies.TwoTimes).ToString();
                }
                else if (Globals.ServerConfigSection.CommonItems.DefaultNumOfCopyPrescriptNormalPT == 3)
                {
                    ((eInPrescriptionOldNewView)this.GetView()).Print3.IsChecked = true;
                    NumberOfTimesPrint = ((int)NumberOfCopies.ThreeTimes).ToString();
                }
                else
                {
                    ((eInPrescriptionOldNewView)this.GetView()).Print4.IsChecked = true;
                    ((eInPrescriptionOldNewView)this.GetView()).Print5.Text = Globals.ServerConfigSection.CommonItems.DefaultNumOfCopyPrescriptNormalPT.ToString();
                }
            }
        }

        public void Print1_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            IsTypingNumberOfCopies = false;
            ((eInPrescriptionOldNewView)this.GetView()).Print5.Text = "";
            NumberOfTimesPrint = ((int)NumberOfCopies.OneTime).ToString();

        }

        public void Print2_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            IsTypingNumberOfCopies = false;
            ((eInPrescriptionOldNewView)this.GetView()).Print5.Text = "";
            NumberOfTimesPrint = ((int)NumberOfCopies.TwoTimes).ToString();
        }

        public void Print3_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            IsTypingNumberOfCopies = false;
            ((eInPrescriptionOldNewView)this.GetView()).Print5.Text = "";
            NumberOfTimesPrint = ((int)NumberOfCopies.ThreeTimes).ToString();
        }

        public void Print4_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            IsTypingNumberOfCopies = true;
            NumberOfTimesPrint = ((int)NumberOfCopies.Other).ToString();
        }

        #endregion


        public List<long> lstMCTypeID;

        public IAllergiesWarning_ByPatientID UCAllergiesWarningByPatientID
        {
            get;
            set;
        }

        private int xNgayBHToiDa_NgoaiTru = 30;
        private int xNgayBHToiDa_NoiTru = 5;
        private int xNgayBHToiDa = 0;
        private long? StoreID = 1;//tam thoi mac dinh kho ban(Khoa Dược)
        private IEnumerator<IResult> DoGetStore_EXTERNAL()
        {
            var paymentTypeTask = new LoadStoreListTask((long)AllLookupValues.StoreType.STORAGE_DRUGDEPT, false, null, false, false);
            yield return paymentTypeTask;
            if (paymentTypeTask != null && paymentTypeTask.LookupList != null)
            {
                StoreID = paymentTypeTask.LookupList.Where(x => x.IsMain == true).FirstOrDefault().StoreID;
                if (StoreID != null && StoreID > 0)
                {
                    yield break;
                }
                else
                {
                    StoreID = paymentTypeTask.LookupList.FirstOrDefault().StoreID;
                }
            }
            yield break;
        }


        void DrugList_OnRefresh(object sender, aEMR.Common.Collections.RefreshEventArgs e)
        {
            SearchDrugForPrescription_Paging(StoreID, DrugList.PageIndex, DrugList.PageSize, false, IsSearchByGenericName);
        }


        #region Properties member

        private bool _btDuocSiEditVisibility = false;
        public bool btDuocSiEditVisibility
        {
            get
            {
                return _btDuocSiEditVisibility;
            }
            set
            {
                if (_btDuocSiEditVisibility != value)
                {
                    _btDuocSiEditVisibility = value;
                    NotifyOfPropertyChange(() => btDuocSiEditVisibility);
                }
            }
        }

        private bool _IsEnabledForm = false;
        public bool IsEnabledForm
        {
            get
            {
                return _IsEnabledForm;
            }
            set
            {
                if (_IsEnabledForm != value)
                {
                    _IsEnabledForm = value;
                    NotifyOfPropertyChange(() => IsEnabledForm);
                }
            }
        }

        //KMx: Thêm ngày 09/10/2014 15:20.
        //Lý do: Ngoại trú chỉ có 1 Service Record chứa 1 chẩn đoán (Registration_DataStorage.PatientServiceRecordCollection[0]). Nhưng nội trú có thể có 2 Service Record chứa 2 chẩn đoán Nhập và Xuất viện.
        //Mà toa thuốc chỉ sử dụng chẩn đoán xuất viện thôi. Nên đối tượng bên dưới sẽ chứa Service Record nào có chẩn đoán xuất viện.
        private PatientServiceRecord _ObjPatientServiceRecord_Current;
        public PatientServiceRecord ObjPatientServiceRecord_Current
        {
            get
            {
                return _ObjPatientServiceRecord_Current;
            }
            set
            {
                if (_ObjPatientServiceRecord_Current != value)
                {
                    CurrentDiagnosisTreatment = null;
                    _ObjPatientServiceRecord_Current = value;
                    NotifyOfPropertyChange(() => ObjPatientServiceRecord_Current);
                }
            }
        }

        private DiagnosisTreatment _ObjDiagnosisTreatment_Current;
        public DiagnosisTreatment ObjDiagnosisTreatment_Current
        {
            get
            {
                return _ObjDiagnosisTreatment_Current;
            }
            set
            {
                if (_ObjDiagnosisTreatment_Current != value)
                {
                    _ObjDiagnosisTreatment_Current = value;
                    NotifyOfPropertyChange(() => ObjDiagnosisTreatment_Current);
                }
            }
        }

        private ObservableCollection<ChooseDose> _ChooseDoses;
        public ObservableCollection<ChooseDose> ChooseDoses
        {
            get
            {
                return _ChooseDoses;
            }
            set
            {
                if (_ChooseDoses != value)
                {
                    _ChooseDoses = value;
                    NotifyOfPropertyChange(() => ChooseDoses);
                }
            }
        }


        private PrescriptionDetail _SelectedPrescriptionDetail;
        public PrescriptionDetail SelectedPrescriptionDetail
        {
            get
            {
                return _SelectedPrescriptionDetail;
            }
            set
            {
                if (_SelectedPrescriptionDetail != value)
                {
                    _SelectedPrescriptionDetail = value;
                    NotifyOfPropertyChange(() => SelectedPrescriptionDetail);
                }
            }
        }

        private PrescriptionDetail _PrescriptionDetailForForm;
        public PrescriptionDetail ObjPrescriptionDetailForForm
        {
            get
            {
                return _PrescriptionDetailForForm;
            }
            set
            {
                if (_PrescriptionDetailForForm != value)
                {
                    _PrescriptionDetailForForm = value;
                    NotifyOfPropertyChange(() => ObjPrescriptionDetailForForm);
                }
            }
        }

        private PagedSortableCollectionView<GetDrugForSellVisitor> _Drugs;
        public PagedSortableCollectionView<GetDrugForSellVisitor> DrugList
        {
            get
            {
                return _Drugs;
            }
            set
            {
                if (_Drugs != value)
                {
                    _Drugs = value;
                    NotifyOfPropertyChange(() => DrugList);
                }
            }
        }

        private ObservableCollection<Staff> _StaffCatgs;
        public ObservableCollection<Staff> StaffCatgs
        {
            get
            {
                return _StaffCatgs;
            }
            set
            {
                if (_StaffCatgs != value)
                {
                    _StaffCatgs = value;
                    NotifyOfPropertyChange(() => StaffCatgs);
                }
            }
        }

        private bool _IsEnabledAutoComplete;
        public bool IsEnabledAutoComplete
        {
            get
            {
                return _IsEnabledAutoComplete;
            }
            set
            {
                if (_IsEnabledAutoComplete != value)
                {
                    _IsEnabledAutoComplete = value;
                    NotifyOfPropertyChange(() => IsEnabledAutoComplete);
                }
            }
        }

        private Prescription _LatestePrecriptions;
        public Prescription LatestePrecriptions
        {
            get
            {
                return _LatestePrecriptions;
            }
            set
            {
                if (_LatestePrecriptions != value)
                {
                    _LatestePrecriptions = value;
                    NotifyOfPropertyChange(() => LatestePrecriptions);
                    if (_LatestePrecriptions != null && DepartmentContent != null && DepartmentContent.Departments != null && DepartmentContent.Departments.Count > 0)
                    {
                        if (_LatestePrecriptions.Department != null && _LatestePrecriptions.Department.DeptID > 0)
                        {
                            DepartmentContent.SelectedItem = _LatestePrecriptions.Department;
                        }
                        else
                        {
                            if (ObjDiagnosisTreatment_Current != null && ObjDiagnosisTreatment_Current.Department != null && ObjDiagnosisTreatment_Current.Department.DeptID > 0
                                && DepartmentContent.Departments.Any(x => x.DeptID == ObjDiagnosisTreatment_Current.Department.DeptID))
                            {
                                DepartmentContent.SelectedItem = DepartmentContent.Departments.Where(x => x.DeptID == ObjDiagnosisTreatment_Current.Department.DeptID).FirstOrDefault();
                                _LatestePrecriptions.Department = DepartmentContent.SelectedItem;
                            }
                            else
                            {
                                //DepartmentContent.SelectedItem = DepartmentContent.Departments != null ? DepartmentContent.Departments.FirstOrDefault() : null;
                                DepartmentContent.SelectedItem = DepartmentContent.Departments.FirstOrDefault();
                                _LatestePrecriptions.Department = DepartmentContent.SelectedItem;
                            }
                        }

                        //KMx: Khi đi lưu thì set 1 lần luôn (21/05/2015 17:32)
                        //if (_LatestePrecriptions.PrescriptionIssueHistory == null)
                        //{
                        //    _LatestePrecriptions.PrescriptionIssueHistory = new PrescriptionIssueHistory();
                        //}

                        //if (RegistrationCoverByHI())
                        //{
                        //    LatestePrecriptions.PrescriptionIssueHistory.HisID = Registration_DataStorage.CurrentPatientRegistration.HisID.Value;
                        //}
                        //else
                        //{
                        //    LatestePrecriptions.PrescriptionIssueHistory.HisID = 0;
                        //}
                    }

                }
            }
        }

        private Prescription _PrecriptionsForPrint;
        public Prescription PrecriptionsForPrint
        {
            get
            {
                return _PrecriptionsForPrint;
            }
            set
            {
                if (_PrecriptionsForPrint != value)
                {
                    _PrecriptionsForPrint = value;
                    NotifyOfPropertyChange(() => PrecriptionsForPrint);
                }
            }
        }

        private Prescription _RetoreLatestePrecriptions;
        public Prescription RetoreLatestePrecriptions
        {
            get
            {
                return _RetoreLatestePrecriptions;
            }
            set
            {
                if (_RetoreLatestePrecriptions != value)
                {
                    _RetoreLatestePrecriptions = value;
                    NotifyOfPropertyChange(() => RetoreLatestePrecriptions);
                }
            }
        }

        private int _NumOfDays = 7;
        public int NumOfDays
        {
            get
            {
                return _NumOfDays;
            }
            set
            {
                if (_NumOfDays != value)
                {
                    _NumOfDays = value;
                    NotifyOfPropertyChange(() => NumOfDays);
                }
            }
        }

        private ObservableCollection<Lookup> _PrescriptionType;
        public ObservableCollection<Lookup> PrescriptionTypeList
        {
            get
            {
                return _PrescriptionType;
            }
            set
            {
                if (_PrescriptionType != value)
                {
                    _PrescriptionType = value;
                    NotifyOfPropertyChange(() => PrescriptionTypeList);
                }
            }
        }

        private Lookup _CurrentPrescriptionType;
        public Lookup CurrentPrescriptionType
        {
            get
            {
                return _CurrentPrescriptionType;
            }
            set
            {
                if (_CurrentPrescriptionType != value)
                {
                    _CurrentPrescriptionType = value;
                    NotifyOfPropertyChange(() => CurrentPrescriptionType);
                }
            }
        }


        private bool _IsEnabled;
        public bool IsEnabled
        {
            get
            {
                return _IsEnabled;
            }
            set
            {
                _IsEnabled = value;
                NotifyOfPropertyChange(() => IsEnabled);
                //NotifyOfPropertyChange(() => txtDaysAfterIsEnabled);
            }
        }

        private bool _BH;
        public bool BH
        {
            get
            {
                return _BH;
            }
            set
            {
                _BH = value;
                NotifyOfPropertyChange(() => BH);
            }
        }

        private bool _NoiTru;
        public bool NoiTru
        {
            get
            {
                return _NoiTru;
            }
            set
            {
                _NoiTru = value;
                NotifyOfPropertyChange(() => NoiTru);
            }
        }


        private bool _IsEnabledPrint = true;
        public bool IsEnabledPrint
        {
            get
            {
                return _IsEnabledPrint;
            }
            set
            {
                if (_IsEnabledPrint != value)
                {
                    _IsEnabledPrint = value;
                    NotifyOfPropertyChange(() => IsEnabledPrint);
                }
            }
        }


        private bool _CanPrint;
        public bool CanPrint
        {
            get
            {
                return _CanPrint;
            }
            set
            {
                if (_CanPrint != value)
                {
                    _CanPrint = value;
                    //CanUndo = !CanPrint;
                    NotifyOfPropertyChange(() => CanPrint);
                }
            }
        }

        private int _ClassificationPatient;
        public int ClassificationPatient
        {
            get
            {
                return _ClassificationPatient;
            }
            set
            {
                _ClassificationPatient = value;
                NotifyOfPropertyChange(() => ClassificationPatient);
            }
        }

        private ObservableCollection<MedicalConditionRecord> _PtMedCond;
        public ObservableCollection<MedicalConditionRecord> PtMedCond
        {
            get
            {
                return _PtMedCond;
            }
            set
            {
                if (_PtMedCond == value)
                    return;
                _PtMedCond = value;
                NotifyOfPropertyChange(() => PtMedCond);
            }
        }
        private bool _IsUpdateDiagConfirmInPT;
        public bool IsUpdateDiagConfirmInPT
        {
            get { return _IsUpdateDiagConfirmInPT; }
            set
            {
                if (_IsUpdateDiagConfirmInPT != value)
                {
                    _IsUpdateDiagConfirmInPT = value;
                    NotifyOfPropertyChange(() => IsUpdateDiagConfirmInPT);
                }
            }
        }
        #endregion

        public void authorization()
        {
            if (!Globals.isAccountCheck)
            {
                return;
            }
        }
        #region account checking

        private bool _mToaThuocDaPhatHanh_ThongTin = true;
        private bool _mToaThuocDaPhatHanh_ChinhSua = true;
        private bool _mToaThuocDaPhatHanh_TaoToaMoi = true;
        private bool _mToaThuocDaPhatHanh_PhatHanhLai = true;
        private bool _mToaThuocDaPhatHanh_In = true;
        private bool _mToaThuocDaPhatHanh_ChonChanDoan = true;
        private bool _hasTitle = true;

        public bool mToaThuocDaPhatHanh_ThongTin
        {
            get
            {
                return _mToaThuocDaPhatHanh_ThongTin;
            }
            set
            {
                if (_mToaThuocDaPhatHanh_ThongTin == value)
                    return;
                _mToaThuocDaPhatHanh_ThongTin = value;
            }
        }
        public bool mToaThuocDaPhatHanh_ChinhSua
        {
            get
            {
                return _mToaThuocDaPhatHanh_ChinhSua;
            }
            set
            {
                if (_mToaThuocDaPhatHanh_ChinhSua == value)
                    return;
                _mToaThuocDaPhatHanh_ChinhSua = value;
            }
        }
        public bool mToaThuocDaPhatHanh_TaoToaMoi
        {
            get
            {
                return _mToaThuocDaPhatHanh_TaoToaMoi;
            }
            set
            {
                if (_mToaThuocDaPhatHanh_TaoToaMoi == value)
                    return;
                _mToaThuocDaPhatHanh_TaoToaMoi = value;
            }
        }
        public bool mToaThuocDaPhatHanh_PhatHanhLai
        {
            get
            {
                return _mToaThuocDaPhatHanh_PhatHanhLai;
            }
            set
            {
                if (_mToaThuocDaPhatHanh_PhatHanhLai == value)
                    return;
                _mToaThuocDaPhatHanh_PhatHanhLai = value;
            }
        }
        public bool mToaThuocDaPhatHanh_In
        {
            get
            {
                return _mToaThuocDaPhatHanh_In;
            }
            set
            {
                if (_mToaThuocDaPhatHanh_In == value)
                    return;
                _mToaThuocDaPhatHanh_In = value;
            }
        }
        public bool mToaThuocDaPhatHanh_ChonChanDoan
        {
            get
            {
                return _mToaThuocDaPhatHanh_ChonChanDoan;
            }
            set
            {
                if (_mToaThuocDaPhatHanh_ChonChanDoan == value)
                    return;
                _mToaThuocDaPhatHanh_ChonChanDoan = value;
            }
        }
        public bool hasTitle
        {
            get
            {
                return _hasTitle;
            }
            set
            {
                if (_hasTitle == value)
                    return;
                _hasTitle = value;
            }
        }
        #endregion


        private bool _ContentKhungTaiKhamIsEnabled = true;
        public bool ContentKhungTaiKhamIsEnabled
        {
            get
            {
                return _ContentKhungTaiKhamIsEnabled;
            }
            set
            {
                if (_ContentKhungTaiKhamIsEnabled != value)
                {
                    _ContentKhungTaiKhamIsEnabled = value;
                    NotifyOfPropertyChange(() => ContentKhungTaiKhamIsEnabled);
                }
            }
        }

        #region service function

        private void GetLatestPrescriptionByPtID_New(long PatientID)
        {
            //Danh cho truong hop la toa moi
            IsWaitingGetLatestPrescriptionByPtID = true;

            var t = new Thread(() =>
            {
                using (var serviceFactory = new ePrescriptionsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetLatestPrescriptionByPtID_InPt(PatientID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var ToaThuoc_Cuoi = contract.EndGetLatestPrescriptionByPtID_InPt(asyncResult);

                            if (ToaThuoc_Cuoi != null && ToaThuoc_Cuoi.IssueID > 0)
                            {
                                LatestePrecriptions = ToaThuoc_Cuoi;
                                LatestePrecriptions.Department = DepartmentContent.SelectedItem;

                                btnCreateNewIsEnabled = true;
                                btnSaveAddNewIsEnabled = false;

                                //KMx: Khi load toa thuốc thì Enable của form phải bằng false, khi nào người dùng nhấn "Chỉnh sửa" thì Enable mới bằng true.
                                // Nếu không có dòng dưới thì sẽ bị sai khi đang "chỉnh sửa" toa cho BN A, chọn BN B từ out standing task, thì toa của BN B tự động Enable (29/05/2014 15:56).
                                IsEnabled = false;

                                btnCreateAndCopyIsEnabled = true;
                                btnCopyToIsEnabled = true;
                                IsEnabledPrint = true;

                                if (CheckToaThuocDuocPhepCapNhat() == false)
                                {
                                    btnEditIsEnabled = false;
                                }
                                else
                                {
                                    btnEditIsEnabled = true;
                                }
                                /*▼====: #002*/
                                if (LatestePrecriptions.ObjDiagnosisTreatment == null || LatestePrecriptions.ObjDiagnosisTreatment.DiagnosisFinal.Length == 0)
                                {
                                    LatestePrecriptions.ObjDiagnosisTreatment = ObjDiagnosisTreatment_Current;
                                }
                                /*▲====: #002*/
                                PrecriptionsForPrint = LatestePrecriptions;

                                ContentKhungTaiKhamIsEnabled = LatestePrecriptions.IsAllowEditNDay;

                                if (LatestePrecriptions.NDay > 0)
                                {
                                    chkHasAppointmentValue = true;
                                }
                                else
                                {
                                    chkHasAppointmentValue = false;
                                }

                                GetPrescriptionDetailsByPrescriptID(LatestePrecriptions.PrescriptID);
                                ////Toa thuoc nay co roi
                                ////check xem neu la cung mot bac si thi chinh sua
                                ////khac bac si thi la phat hanh lai toa thuoc
                                //PrescripState = PrescriptionState.EditPrescriptionState;
                            }
                            else/*Toa Mới*/
                            {
                                LatestePrecriptions = NewPrecriptions.DeepCopy();
                                //20191120 TBL: Nếu đăng ký này đã có chẩn đoán xuất viện thì lấy chẩn đoán xuất viện đó cho xác nhận chẩn đoán toa thuốc
                                if (CurrentDiagnosisTreatment != null && CurrentDiagnosisTreatment.DTItemID > 0)
                                {
                                    if (LatestePrecriptions.ObjDiagnosisTreatment == null)
                                    {
                                        LatestePrecriptions.ObjDiagnosisTreatment = new DiagnosisTreatment();
                                    }
                                    LatestePrecriptions.ObjDiagnosisTreatment.DiagnosisFinal = CurrentDiagnosisTreatment.DiagnosisFinal;
                                    LatestePrecriptions.ObjDiagnosisTreatment.Diagnosis = string.IsNullOrEmpty(CurrentDiagnosisTreatment.DiagnosisFinal) ? CurrentDiagnosisTreatment.Diagnosis.Trim() : CurrentDiagnosisTreatment.DiagnosisFinal.Trim();
                                    LatestePrecriptions.Diagnosis = LatestePrecriptions.ObjDiagnosisTreatment.Diagnosis;
                                    LatestePrecriptions.ObjDiagnosisTreatment.Treatment = CurrentDiagnosisTreatment.Treatment;
                                }
                                BackupCurPrescriptionItem();
                                AddNewBlankDrugIntoPrescriptObjectNew();

                                IsEnabled = true;

                                btChonChanDoanIsEnabled = true;
                                //▼===== 20191019 TTM: Set về false khi load bệnh nhân không có bất cứ toa nào
                                btnCreateAndCopyIsEnabled = false;
                                //▲===== 
                                btnCreateNewIsEnabled = false;
                                btnSaveAddNewIsEnabled = true;
                                btnUndoIsEnabled = true;
                                btnEditIsEnabled = false;
                                btnCopyToIsEnabled = false;
                                IsEnabledPrint = false;
                            }
                            //▼===== 20191011 TTM: Loại bỏ vì không sử dụng.
                            //if (Globals.SecretaryLogin != null)
                            //{
                            //    LatestePrecriptions.SecretaryStaff = Globals.SecretaryLogin.Staff;
                            //}
                            //▲===== 
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                        finally
                        {
                            IsWaitingGetLatestPrescriptionByPtID = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }

        private void InitChooseDose()
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            this.ShowBusyIndicator();
            IsWaitingChooseDose = true;

            var t = new Thread(() =>
            {
                using (var serviceFactory = new ePrescriptionsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginInitChooseDoses(Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            var results = contract.EndInitChooseDoses(asyncResult);
                            if (results != null)
                            {
                                if (ChooseDoses == null)
                                {
                                    ChooseDoses = new ObservableCollection<ChooseDose>();
                                }
                                else
                                {
                                    ChooseDoses.Clear();
                                }
                                foreach (ChooseDose p in results)
                                {
                                    ChooseDoses.Add(p);
                                }
                                NotifyOfPropertyChange(() => ChooseDoses);
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                        finally
                        {
                            IsWaitingChooseDose = false;
                            this.HideBusyIndicator();
                        }

                    }), null);

                }

            });

            t.Start();
        }

        public void GetPrescriptionDetailsByPrescriptID(long prescriptID, bool GetRemaining = false)
        {
            //IsWaitingGetPrescriptionDetailsByPrescriptID = true;

            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            string msg;
            this.ShowBusyIndicator();

            var t = new Thread(() =>
            {
                using (var serviceFactory = new ePrescriptionsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetPrescriptionDetailsByPrescriptID_InPt(prescriptID, GetRemaining, false, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var Results = contract.EndGetPrescriptionDetailsByPrescriptID_InPt(asyncResult);
                            LatestePrecriptions.PrescriptionDetails = Results.ToObservableCollection();
                            //▼====: #012
                            foreach (var item in LatestePrecriptions.PrescriptionDetails)
                            {
                                GetRouteOfAdministrationList(item,0,(long)item.DrugID);
                            }
                            //▲====: #012
                            if (!GetRemaining)
                            {
                                BackupCurPrescriptionItem();
                            }
                            else
                            {
                                AddNewBlankDrugIntoPrescriptObjectNew();
                            }
                            if (GetRemaining && CheckSoLuongThuocDeBan(out msg) == false)
                            {
                                MessageBox.Show(msg, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                        finally
                        {
                            //Globals.IsBusy = false;
                            //IsWaitingGetPrescriptionDetailsByPrescriptID = false;
                            this.HideBusyIndicator();
                        }

                    }), null);

                }

            });

            t.Start();
        }
        private void GetRemainingPrescriptionDetailsByPrescriptID(long prescriptID, bool GetRemaining = false)
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                using (var serviceFactory = new ePrescriptionsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetPrescriptionDetailsByPrescriptID_InPt(prescriptID, GetRemaining, false, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var Results = contract.EndGetPrescriptionDetailsByPrescriptID_InPt(asyncResult);
                            foreach (PrescriptionDetail detail in LatestePrecriptions.PrescriptionDetails)
                            {
                                foreach (var item in Results)
                                {
                                    if (item.DrugID == detail.DrugID)
                                    {
                                        detail.SelectedDrugForPrescription.Remaining = item.SelectedDrugForPrescription.Remaining;
                                    }
                                }
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
                    }), null);
                }
            });
            t.Start();
        }

        private void SearchDrugForPrescription_Paging(long? StoreID, int PageIndex, int PageSize, bool CountTotal, bool IsSearchByGenericName)
        {
            DrugList.Clear();

            var t = new Thread(() =>
            {
                using (var serviceFactory = new ePrescriptionsServiceClient())
                {
                    var client = serviceFactory.ServiceInstance;
                    client.BeginSearchGenMedProductForPrescription_Paging(BrandName, IsInsurance, StoreID, PageIndex, PageSize, CountTotal, IsSearchByGenericName, Globals.DispatchCallback((asyncResult) =>
                    {
                        int Total = 0;
                        IList<GetDrugForSellVisitor> allItems = null;
                        bool bOK = false;
                        try
                        {
                            allItems = client.EndSearchGenMedProductForPrescription_Paging(out Total, asyncResult);
                            bOK = true;
                        }
                        catch (Exception innerEx)
                        {
                            MessageBox.Show(innerEx.ToString());
                        }

                        if (bOK)
                        {
                            if (CountTotal)
                            {
                                DrugList.Clear();
                                DrugList.TotalItemCount = Total;
                                DrugList.ItemCount = Total;
                            }
                            if (allItems != null)
                            {
                                foreach (var item in allItems)
                                {
                                    DrugList.Add(item);
                                }
                                if (AutoGenMedProduct != null)
                                {
                                    //AutoGenMedProduct.ItemsSource = ObjectCopier.DeepCopy(DrugList);
                                    AutoGenMedProduct.ItemsSource = DrugList;
                                    AutoGenMedProduct.PopulateComplete();
                                }
                            }
                        }
                    }), null);

                }

            });

            t.Start();
        }


        private void GetMedConditionByPtID(long patientID, int mcTypeID)
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "" });
            this.ShowBusyIndicator();
            IsWaitingGetMedConditionByPtID = true;

            var t = new Thread(() =>
            {
                using (var serviceFactory = new ComRecordsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginGetMedConditionByPtID(patientID, mcTypeID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {

                            var items = contract.EndGetMedConditionByPtID(asyncResult);
                            if (items != null)
                            {
                                PtMedCond = new ObservableCollection<MedicalConditionRecord>(items);
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                        finally
                        {
                            IsWaitingGetMedConditionByPtID = false;
                            this.HideBusyIndicator();
                        }
                    }), null);
                }


            });
            t.Start();
        }
        #endregion

        #region minifunction

        private void ClearDataRow(PrescriptionDetail ObjRow)
        {
            if (ObjRow != null)
            {
                if (ObjRow.DrugID > 0)
                {
                    ObjRow.dosageStr = "0";
                    ObjRow.dosage = 0;
                    ObjRow.MDoseStr = "0";
                    ObjRow.MDose = 0;
                    ObjRow.ADoseStr = "0";
                    ObjRow.ADose = 0;
                    ObjRow.EDoseStr = "0";
                    ObjRow.EDose = 0;
                    ObjRow.NDoseStr = "0";
                    ObjRow.NDose = 0;
                    //ObjRow.DayRpts = 0;//cai này vô là sai vi cứ reset ND=0 miết
                    //ObjRow.DayExtended =0;//cai này vô là sai vi cứ reset ND=0 miết
                    ObjRow.Qty = 0;
                    ObjRow.ChooseDose = new ChooseDose();
                    ObjRow.DrugInstructionNotes = "";
                    ObjRow.Administration = "";
                    int nDayVal = LatestePrecriptions.NDay == null ? 0 : LatestePrecriptions.NDay.Value;
                    SetDefaultDay(ObjRow, nDayVal);
                    ObjRow.ObjPrescriptionDetailSchedules = new ObservableCollection<PrescriptionDetailSchedules>();
                    ObjRow.V_RouteOfAdministration = 0;
                }
            }
        }


        private void AddNewBlankDrugIntoPrescriptObjectNew(int index, GetDrugForSellVisitor item)
        {
            if (LatestePrecriptions == null)
                LatestePrecriptions = new Prescription();

            if (LatestePrecriptions.PrescriptionDetails == null)
                LatestePrecriptions.PrescriptionDetails = new ObservableCollection<PrescriptionDetail>();

            int i = LatestePrecriptions.PrescriptionDetails.Count + 1;
            PrescriptionDetail prescriptDObj = new PrescriptionDetail();
            prescriptDObj.DrugID = 0;
            prescriptDObj.IsInsurance = null;
            prescriptDObj.Strength = "";
            prescriptDObj.Qty = 0;
            prescriptDObj.MDose = 0;
            prescriptDObj.ADose = 0;
            prescriptDObj.EDose = 0;
            prescriptDObj.NDose = 0;
            if (BH)
            {
                prescriptDObj.BeOfHIMedicineList = true;
                prescriptDObj.InsuranceCover = true;
            }
            else
            {
                prescriptDObj.BeOfHIMedicineList = false;
                prescriptDObj.InsuranceCover = false;
            }

            prescriptDObj.DrugInstructionNotes = "";

            if (LatestePrecriptions.PrescriptionDetails.Count > 0)
            {
                prescriptDObj.PrescriptDetailID = LatestePrecriptions.PrescriptionDetails[LatestePrecriptions.PrescriptionDetails.Count - 1].PrescriptDetailID + 1;
            }
            else
            {
                prescriptDObj.PrescriptDetailID = 0;
            }

            prescriptDObj.SelectedDrugForPrescription = item;
            LatestePrecriptions.PrescriptionDetails.Insert(index, prescriptDObj);
            NotifyOfPropertyChange(() => LatestePrecriptions.PrescriptionDetails);
        }

        private bool BlankDrugLineAlreadyExist()
        {
            if (LatestePrecriptions.PrescriptionDetails == null)
                return true;

            int nCount = LatestePrecriptions.PrescriptionDetails.Count;
            if (nCount == 0)
                return false;

            if (nCount > 0)
            {
                // Txd 12/10/2013 : The current Last line has been selected with DrugID or it is a Drug outside of Catalog
                PrescriptionDetail LastPrescriptDetail = LatestePrecriptions.PrescriptionDetails[nCount - 1];
                if (LastPrescriptDetail.DrugID > 0 || (LastPrescriptDetail.IsDrugNotInCat && !string.IsNullOrEmpty(LastPrescriptDetail.SelectedDrugForPrescription.BrandName)))
                {
                    return false;
                }
            }
            return true;
        }

        private void AddNewBlankDrugIntoPrescriptObjectNew()
        {
            if (LatestePrecriptions == null)
            {
                LatestePrecriptions = new Prescription();
            }

            if (LatestePrecriptions.PrescriptionDetails == null)
            {
                LatestePrecriptions.PrescriptionDetails = new ObservableCollection<PrescriptionDetail>();
            }

            if (BlankDrugLineAlreadyExist())
            {
                return;
            }

            PrescriptionDetail prescriptDObj = NewReInitPrescriptionDetail(false, null);

            LatestePrecriptions.PrescriptionDetails.Add(prescriptDObj);
            NotifyOfPropertyChange(() => LatestePrecriptions.PrescriptionDetails);

            ObjPrescriptionDetailForForm = NewReInitPrescriptionDetail(true, null);
            if (LatestePrecriptions.PrescriptionDetails.Count > 1)
            {
                LatestePrecriptions.PreNoDrug = false;
                NotifyOfPropertyChange(() => LatestePrecriptions.PreNoDrug);
            }
        }


        private PrescriptionDetail NewReInitPrescriptionDetail(bool bForm, PrescriptionDetail existingPrescriptObj, bool bOnlyInitObj = false)
        {
            PrescriptionDetail prescriptDObj = existingPrescriptObj;
            if (!bOnlyInitObj)
            {
                if (prescriptDObj == null)
                {
                    prescriptDObj = new PrescriptionDetail();
                }
                prescriptDObj.isForm = bForm;
                prescriptDObj.SelectedDrugForPrescription = new GetDrugForSellVisitor();
                prescriptDObj.DrugID = 0;
                if (BH)
                {
                    prescriptDObj.BeOfHIMedicineList = true;
                    prescriptDObj.InsuranceCover = true;
                }
                else
                {
                    prescriptDObj.BeOfHIMedicineList = false;
                    prescriptDObj.InsuranceCover = false;
                }
                prescriptDObj.Index = LatestePrecriptions.PrescriptionDetails.Count;
            }

            prescriptDObj.IsInsurance = null;
            prescriptDObj.Strength = "";
            prescriptDObj.Qty = 0;
            prescriptDObj.MDoseStr = "0";
            prescriptDObj.ADoseStr = "0";
            prescriptDObj.EDoseStr = "0";
            prescriptDObj.NDoseStr = "0";
            prescriptDObj.DrugType = new Lookup
            {
                LookupID = (long)AllLookupValues.V_DrugType.THUOC_THONGTHUONG,
                ObjectValue = eHCMSResources.T0748_G1_T.ToUpper()
            };
            prescriptDObj.V_DrugType = (long)AllLookupValues.V_DrugType.THUOC_THONGTHUONG;

            int nDayVal = LatestePrecriptions.NDay == null ? 0 : LatestePrecriptions.NDay.Value;
            SetDefaultDay(prescriptDObj, nDayVal);
            //GetDayRptNormal(prescriptDObj, xNgayBHToiDa, nDayVal);

            prescriptDObj.DrugInstructionNotes = "";
            prescriptDObj.Administration = "";
            if (LatestePrecriptions.PrescriptionDetails != null && LatestePrecriptions.PrescriptionDetails.Count > 0)
            {
                prescriptDObj.PrescriptDetailID = LatestePrecriptions.PrescriptionDetails[LatestePrecriptions.PrescriptionDetails.Count - 1].PrescriptDetailID + 1;
            }
            else
            {
                prescriptDObj.PrescriptDetailID = 0;
            }

            prescriptDObj = ObjectCopier.DeepCopy(prescriptDObj);

            return prescriptDObj;
        }


        private bool CheckThuocHopLe()
        {
            StringBuilder sb = new StringBuilder();

            bool Result = true;

            if (LatestePrecriptions.PrescriptionDetails.Count > 1)
            {
                foreach (PrescriptionDetail item in LatestePrecriptions.PrescriptionDetails)
                {
                    // 29/09/2013
                    // Txd : Do the following Check BH and re-assigning for DayRpts and DayExtended Just
                    //       in case they were missed (NOT BEING ASSIGNED) previously 
                    //       BECAUSE IT IS ALWAYS CORRECT IN ALL CASES (assume at this stage)
                    item.DayRpts = item.RealDay;
                    item.DayExtended = 0;
                    //KMx: Không phân biệt toa thường hay toa bảo hiểm (04/06/2014 10:45)
                    //if (BH && item.RealDay > xNgayBHToiDa)
                    if (item.RealDay > xNgayBHToiDa)
                    {
                        item.DayRpts = xNgayBHToiDa;
                        item.DayExtended = item.RealDay - xNgayBHToiDa;
                    }


                    if (item.V_DrugType == (long)AllLookupValues.V_DrugType.THUOC_UONGLICHTUAN)
                    {
                        float fTotalQty = 0;

                        foreach (PrescriptionDetailSchedules schedItem in item.ObjPrescriptionDetailSchedules)
                        {
                            fTotalQty += (schedItem.Sunday.HasValue ? schedItem.Sunday.Value : 0);
                            fTotalQty += (schedItem.Monday.HasValue ? schedItem.Monday.Value : 0);
                            fTotalQty += (schedItem.Tuesday.HasValue ? schedItem.Tuesday.Value : 0);
                            fTotalQty += (schedItem.Wednesday.HasValue ? schedItem.Wednesday.Value : 0);
                            fTotalQty += (schedItem.Thursday.HasValue ? schedItem.Thursday.Value : 0);
                            fTotalQty += (schedItem.Friday.HasValue ? schedItem.Friday.Value : 0);
                            fTotalQty += (schedItem.Saturday.HasValue ? schedItem.Saturday.Value : 0);
                        }

                        if (fTotalQty <= 0)
                        {
                            sb.AppendLine(string.Format(eHCMSResources.Z1055_G1_ThuocTheoLichChonSLgPhuHop, item.SelectedDrugForPrescription.BrandName.Trim()));
                            Result = false;
                        }
                        //KMx: Thuốc Lịch nếu số ngày <= 0 hoặc số lượng <= 0 thì báo lỗi (16/03/2014 10:54)
                        else
                        {
                            if (item.DayRpts <= 0)
                            {
                                sb.AppendLine(string.Format(eHCMSResources.Z1056_G1_ThuocNgDungLonHon0, item.SelectedDrugForPrescription.BrandName.Trim()));
                                Result = false;
                            }
                            if (item.Qty <= 0)
                            {
                                sb.AppendLine(string.Format(eHCMSResources.Z1057_G1_ThuocSLgLonHon0, item.SelectedDrugForPrescription.BrandName.Trim()));
                                Result = false;
                            }
                        }
                    }

                    if (item == LatestePrecriptions.PrescriptionDetails.LastOrDefault())
                    {
                        continue;
                    }

                    if (item.SelectedDrugForPrescription == null
                        || item.SelectedDrugForPrescription == null
                        || item.SelectedDrugForPrescription.BrandName == null
                        || item.SelectedDrugForPrescription.BrandName == "")
                    {
                        sb.AppendLine(string.Format(eHCMSResources.Z0908_G1_ThuocDong0KgHopLe, (item.Index + 1).ToString()));
                        Result = false;
                        continue;
                    }

                    if (!item.IsDrugNotInCat
                        && item.SelectedDrugForPrescription.DrugID < 1)
                    {
                        sb.AppendLine(string.Format(eHCMSResources.Z0908_G1_ThuocDong0KgHopLe, (item.Index + 1).ToString()));
                        Result = false;
                        continue;
                    }

                    if (item.SelectedDrugForPrescription == null || item.SelectedDrugForPrescription.BrandName == null)
                    {
                        continue;
                    }


                    if (item.HasSchedules)//Có Lịch mà bên ngoài sáng chưa chiều tối còn có thì báo lỗi
                    {
                        if (item.MDose > 0 ||
                            item.ADose > 0 ||
                            item.EDose > 0 ||
                            item.NDose > 0)
                        {
                            sb.AppendLine(string.Format(eHCMSResources.Z1059_G1_ThuocDaCoChiDinhLich, item.SelectedDrugForPrescription.BrandName.Trim()));
                            item.dosage = 0;
                            item.MDose = 0;
                            item.ADose = 0;
                            item.EDose = 0;
                            item.NDose = 0;
                            Result = false;
                        }
                    }
                    else//Không Lịch
                    {
                        if (item.Qty <= 0)
                        {
                            sb.AppendLine(string.Format(eHCMSResources.Z1057_G1_ThuocSLgLonHon0, item.SelectedDrugForPrescription.BrandName.Trim()));
                            Result = false;
                        }

                        if (item.V_DrugType == (long)AllLookupValues.V_DrugType.THUOC_UONGKHICAN)
                        {
                            continue;
                        }

                        //Thuốc thường.
                        if (item.MDose == 0 &&
                        item.ADose.GetValueOrDefault() == 0 &&
                        item.EDose.GetValueOrDefault() == 0 &&
                        item.NDose.GetValueOrDefault() == 0)
                        {
                            sb.AppendLine(string.Format(eHCMSResources.Z0910_G1_Thuoc0SangTruaChieuToi, item.SelectedDrugForPrescription.BrandName.Trim()));
                            Result = false;
                        }


                        if (item.DayRpts <= 0)
                        {
                            sb.AppendLine(string.Format(eHCMSResources.Z0911_G1_Thuoc0NgDungLonHon0, item.SelectedDrugForPrescription.BrandName.Trim()));
                            Result = false;
                        }

                        //if (item.SelectedDrugForPrescription.MaxDayPrescribed != null
                        //    && item.SelectedDrugForPrescription.MaxDayPrescribed > 0
                        //    && item.SelectedDrugForPrescription.MaxDayPrescribed < (item.DayExtended + item.DayRpts))
                        //{
                        //    sb.AppendLine("Thuốc: " + item.SelectedDrugForPrescription.BrandName.Trim() + ": Ngày dùng của bạn vượt quá số ngày ra toa tối đa (" + item.SelectedDrugForPrescription.MaxDayPrescribed + ").");
                        //    item.DayExtended = (double)item.SelectedDrugForPrescription.MaxDayPrescribed - item.DayRpts;
                        //    Result = false;
                        //}

                        if (CheckQtyLessThanQtyAutoCalc(item))
                        {
                            sb.AppendLine(string.Format(eHCMSResources.Z0914_G1_Thuoc0CanKTraLai, item.SelectedDrugForPrescription.BrandName.Trim()));
                            Result = false;
                        }
                    }

                    //KMx: Thuốc lịch cũng phải kiểm tra thuốc ngắn ngày luôn (12/06/2014 10:14). Nhưng không hiểu tính lại DayExtended để làm gì?
                    if (item.V_DrugType != (long)AllLookupValues.V_DrugType.THUOC_UONGKHICAN
                        && item.SelectedDrugForPrescription.MaxDayPrescribed != null
                        && item.SelectedDrugForPrescription.MaxDayPrescribed > 0
                        && item.SelectedDrugForPrescription.MaxDayPrescribed < (item.DayExtended + item.DayRpts))
                    {
                        sb.AppendLine(string.Format(eHCMSResources.Z0912_G1_Thuoc0NgDungVuotSoNgToiDa, item.SelectedDrugForPrescription.BrandName.Trim(), item.SelectedDrugForPrescription.MaxDayPrescribed));
                        item.DayExtended = (double)item.SelectedDrugForPrescription.MaxDayPrescribed - item.DayRpts;
                        Result = false;
                    }
                }
                if (Result == false)
                {
                    MessageBox.Show(sb.ToString() + Environment.NewLine + eHCMSResources.I0945_G1_I, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                    return false;
                }
            }
            else
            {
                if (!PreNoDrug)
                {
                    MessageBox.Show(eHCMSResources.A0401_G1_Msg_InfoChuaChonThuocDeRaToa, eHCMSResources.A0290_G1_Msg_LuuToaThuoc, MessageBoxButton.OK);
                    return false;
                }
                else
                {
                    if (MessageBox.Show(eHCMSResources.I0943_G1_I, eHCMSResources.A0290_G1_Msg_LuuToaThuoc, MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private bool CheckQtyLessThanQtyAutoCalc(PrescriptionDetail Objtmp)
        {
            if (Objtmp != null && Objtmp.IsDrugNotInCat == false)
            {
                if (Objtmp.HasSchedules == false)
                {
                    Nullable<float> TongThuoc = 0;
                    float Tong = 0;

                    if (Objtmp != null)
                    {
                        TongThuoc = Objtmp.MDose + Objtmp.ADose.GetValueOrDefault() + Objtmp.NDose.GetValueOrDefault() + Objtmp.EDose.GetValueOrDefault();
                        Tong = (float)(TongThuoc.Value * (Objtmp.DayRpts + Objtmp.DayExtended) * Objtmp.SelectedDrugForPrescription.UnitVolume) / (float)Objtmp.SelectedDrugForPrescription.DispenseVolume;
                        if (Objtmp.Qty < Math.Ceiling(Tong))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        private bool ErrCheckChongChiDinh()
        {
            StringBuilder sb = new StringBuilder();

            bool Result = false;

            if (LatestePrecriptions.PrescriptionDetails.Count > 1)
            {
                foreach (PrescriptionDetail item in LatestePrecriptions.PrescriptionDetails)
                {
                    if (item.SelectedDrugForPrescription != null)
                    {
                        string err = "";

                        if (CheckChongChiDinh1Drug(item.DrugID.Value, out err))
                        {
                            sb.AppendLine(err);
                            Result = true;
                        }
                    }
                }
            }
            if (Result)
            {
                MessageBox.Show(sb.ToString(), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
            }
            return Result;
        }

        private bool CheckChongChiDinh1Drug(long DrugID, out string msg)
        {
            msg = "";

            if (Globals.allContraIndicatorDrugsRelToMedCond == null)
                return false;

            foreach (var MedCond in PtMedCond)
            {
                long mcType = (long)MedCond.RefMedicalCondition.MedContraTypeID;
                foreach (var cDR in Globals.allContraIndicatorDrugsRelToMedCond)
                {
                    if (cDR.RefMedicalConditionType.MedContraTypeID == mcType && cDR.DrugID == DrugID)
                    {
                        msg = string.Format(eHCMSResources.Z1498_G1_Thuoc0CCDVoiDKienBenh1, cDR.RefGenericDrugDetail.BrandName.Trim(), cDR.RefMedicalConditionType.MedContraIndicationType.Trim());
                        return true;
                    }
                }
            }
            return false;
        }


        /// <summary>
        /// Cap nhat lai ngay va so luong trong toa thuoc hien hanh
        /// </summary>
        private void AutoAdjustCancelDrugShortDays()
        {
            if (LatestePrecriptions.PrescriptionDetails == null || LatestePrecriptions.PrescriptionDetails.Count < 1)
            {
                return;
            }
            int nDayVal = LatestePrecriptions.NDay == null ? 0 : LatestePrecriptions.NDay.Value;
            foreach (var item in LatestePrecriptions.PrescriptionDetails)
            {
                //KMx: Khi thay đổi ngày dùng hàng loạt, thì không cần tính lại ngày dùng và số lượng cho thuốc cần (10/06/2014 10:56).
                if (item.isNeedToUse)
                {
                    continue;
                }

                GetDayRpt(item, nDayVal);
                // Txd 30/09/2013
                // Added the following to Calculate Qty for Drug NOT IN CATALOG
                // Because GetDayRpt DOES NOT DO IT
                //if (item.IsDrugNotInCat)
                //{
                //    CalcTotalQtyForDrugItem(item);
                //}
            }
        }

        private void SetDefaultDay(PrescriptionDetail item, int nDayTotal)
        {
            item.DayRpts = nDayTotal;
            item.DayExtended = 0;
            item.RealDay = nDayTotal;
        }


        private void AdjustQtyMaxAllowed(PrescriptionDetail drugItem)
        {
            if (drugItem == null || drugItem.RealDay <= 0 || drugItem.SelectedDrugForPrescription == null)
            {
                return;
            }

            float QtyTotal = 0;
            float QtyHIAllowed = 0;

            if (drugItem.V_DrugType == (long)AllLookupValues.V_DrugType.THUOC_THONGTHUONG)
            {
                QtyTotal = CalQtyForNormalDrug(drugItem, drugItem.RealDay);

                QtyHIAllowed = CalQtyForNormalDrug(drugItem, xNgayBHToiDa);
            }

            if (drugItem.V_DrugType == (long)AllLookupValues.V_DrugType.THUOC_UONGLICHTUAN)
            {
                QtyTotal = (float)CalQtyForScheduleDrug(drugItem, drugItem.RealDay);

                QtyHIAllowed = (float)CalQtyForScheduleDrug(drugItem, xNgayBHToiDa);
            }

            if (drugItem.Qty <= QtyHIAllowed)
            {
                drugItem.QtyMaxAllowed = drugItem.Qty;
            }
            else
            {
                drugItem.QtyMaxAllowed = QtyHIAllowed;
            }
        }

        private float CalQtyForDay(PrescriptionDetail drugItem)
        {
            if (drugItem == null || drugItem.SelectedDrugForPrescription == null)
            {
                return 0;
            }

            float QtyAllDose = 0;
            float Result = 0;

            QtyAllDose = drugItem.MDose + drugItem.ADose.GetValueOrDefault() + drugItem.NDose.GetValueOrDefault() + drugItem.EDose.GetValueOrDefault();

            Result = QtyAllDose / ((float)drugItem.SelectedDrugForPrescription.DispenseVolume == 0 ? 1 : (float)drugItem.SelectedDrugForPrescription.DispenseVolume);

            return Result;
        }

        private float CalQtyForNormalDrug(PrescriptionDetail drugItem, int nNumDayPrescribed)
        {
            if (drugItem == null || drugItem.SelectedDrugForPrescription == null)
            {
                return 0;
            }

            float QtyAllDose = 0;
            float Result = 0;

            QtyAllDose = drugItem.MDose + drugItem.ADose.GetValueOrDefault() + drugItem.NDose.GetValueOrDefault() + drugItem.EDose.GetValueOrDefault();

            //KMx: Phải nhân trước rồi chia sau để hạn chế kết quả có số lẻ (06/11/2014 11:11).
            Result = (QtyAllDose * nNumDayPrescribed) / ((float)drugItem.SelectedDrugForPrescription.DispenseVolume == 0 ? 1 : (float)drugItem.SelectedDrugForPrescription.DispenseVolume);

            //KMx: Phải Round trước rồi mới Ceiling sau, nếu không sẽ bị sai trong trường hợp kết quả có nhiều số lẻ. VD: 5.00001
            return (float)Math.Ceiling(Math.Round(Result, 2));
        }

        private void CalcTotalQtyForDrugItem(PrescriptionDetail drugItem)
        {
            if (drugItem.HasSchedules)
            {
                // Only calculate for item without Weekly Taking Schedule (Lich Tuan)
                return;
            }

            if (drugItem == null || drugItem.SelectedDrugForPrescription == null)
            {
                return;
            }

            drugItem.Qty = CalQtyForNormalDrug(drugItem, drugItem.RealDay);

            //Hàm này chỉ dùng cho thuốc thường, cần và phải nằm trong DMBH, nếu như có thuốc ngoài DM lọt vào hàm này thì chỉ tính Qty rồi return (13/06/2014 15:14).
            if (drugItem.IsDrugNotInCat)
            {
                return;
            }

            //Thuốc cần.
            //KMx: Nếu là thuốc cần thì QtyMaxAllowed = Qty (A.Tuấn quyết định) (05/06/2014 16:61).
            if (drugItem.isNeedToUse)
            {
                drugItem.QtyMaxAllowed = drugItem.Qty;
                return;
            }

            //Thuốc thường.
            drugItem.QtyForDay = CalQtyForDay(drugItem);

            //KMx: Tính số lượng thuốc (thuốc thường) mà BH đồng ý chi trả (05/06/2014 14:00).
            if (drugItem.RealDay <= xNgayBHToiDa)
            {
                drugItem.QtyMaxAllowed = drugItem.Qty;
            }
            else
            {
                drugItem.QtyMaxAllowed = CalQtyForNormalDrug(drugItem, xNgayBHToiDa);
            }
        }


        /// <summary>
        /// Tinh ngay cho thuoc dung khi can
        /// </summary>
        /// <param name="item"></param>
        private void InitUsageDaysForDrugTakenAsRequired(PrescriptionDetail item)
        {
            item.DayRpts = 0;
            item.DayExtended = 0;
            item.RealDay = 0;
        }

        private void GetDayRpt(PrescriptionDetail item, int NDay)
        {
            //neu ngay dua xuong < 1 thi ko tinh lai
            //if (NDay < 1)
            //{
            //    return;
            //}


            if (item == null || item.SelectedDrugForPrescription == null)
            {
                return;
            }

            if (item.SelectedDrugForPrescription.MaxDayPrescribed > 0 && NDay > item.SelectedDrugForPrescription.MaxDayPrescribed)
            {
                SetDefaultDay(item, (int)item.SelectedDrugForPrescription.MaxDayPrescribed);
            }
            else
            {
                SetDefaultDay(item, NDay);
            }

            if (item.V_DrugType == (long)AllLookupValues.V_DrugType.THUOC_THONGTHUONG
                || item.V_DrugType == (long)AllLookupValues.V_DrugType.THUOC_UONGKHICAN)
            {
                //Nếu thuốc ngoài danh mục thì chỉ cần tính Qty thôi. Ngược lại, phải tính thêm QtyForDay (thuốc thường), QtyMaxAllowed (thuốc thường, cần, lịch để in lên report) (13/06/2014 14:27).
                if (item.IsDrugNotInCat)
                {
                    item.Qty = CalQtyForNormalDrug(item, item.RealDay);
                }
                else
                {
                    CalcTotalQtyForDrugItem(item);
                }
            }
            else if (item.V_DrugType == (long)AllLookupValues.V_DrugType.THUOC_UONGLICHTUAN)
            {
                if (item.IsDrugNotInCat)
                {
                    item.Qty = CalQtyForScheduleDrug(item, item.RealDay);
                }
                else
                {
                    CalQtyAndQtyMaxForSchedule(item);
                }
            }
        }

        #endregion
        private string BrandName;
        private int IsInsurance;

        #region autocomplete doctor
        private void SearchStaffCatgs(string SearchKeys)
        {
            var t = new Thread(() =>
            {
                using (var serviceFactory = new CommonUtilsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginSearchStaffFullName(SearchKeys, 0, Globals.PageSize, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            var results = contract.EndSearchStaffFullName(asyncResult);
                            if (results != null)
                            {
                                StaffCatgs = new ObservableCollection<Staff>();
                                foreach (Staff p in results)
                                {
                                    StaffCatgs.Add(p);
                                }
                                NotifyOfPropertyChange(() => StaffCatgs);
                            }
                            aucHoldConsultDoctor.ItemsSource = StaffCatgs;
                            aucHoldConsultDoctor.PopulateComplete();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                        finally
                        {
                            Globals.IsBusy = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }


        AutoCompleteBox aucHoldConsultDoctor;

        public void aucHoldConsultDoctor_Populating(object sender, PopulatingEventArgs e)
        {
            aucHoldConsultDoctor = sender as AutoCompleteBox;
            this.SearchStaffCatgs(e.Parameter);
        }

        public void aucHoldConsultDoctor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            aucHoldConsultDoctor = sender as AutoCompleteBox;

            if (aucHoldConsultDoctor.SelectedItem != null)
            {
                LatestePrecriptions.ConsultantID = (aucHoldConsultDoctor.SelectedItem as Staff).StaffID;
                if (LatestePrecriptions.ConsultantDoctor != null)
                {
                    LatestePrecriptions.ConsultantDoctor.FullName = (aucHoldConsultDoctor.SelectedItem as Staff).FullName;
                }
            }
            else
            {
                if (LatestePrecriptions != null)
                {
                    if (LatestePrecriptions.ConsultantDoctor != null)
                    {
                        LatestePrecriptions.ConsultantDoctor.FullName = "";
                    }
                }
            }
        }

        public void chkNeedToHold_Check(object sender, RoutedEventArgs e)
        {
            IsEnabledAutoComplete = true;
        }

        public void chkNeedToHold_UnCheck(object sender, RoutedEventArgs e)
        {
            IsEnabledAutoComplete = false;
            LatestePrecriptions.ConsultantID = null;
            if (LatestePrecriptions.ConsultantDoctor != null)
            {
                LatestePrecriptions.ConsultantDoctor.FullName = "";
            }
        }


        #endregion



        private bool _chkHasAppointmentValue;
        public bool chkHasAppointmentValue
        {
            get
            {
                return _chkHasAppointmentValue;
            }
            set
            {
                if (_chkHasAppointmentValue != value)
                {
                    _chkHasAppointmentValue = value;

                    NotifyOfPropertyChange(() => chkHasAppointmentValue);
                    //NotifyOfPropertyChange(() => txtDaysAfterIsEnabled);
                }
            }
        }

        private bool CheckValidationEditor1Row(PrescriptionDetail item)
        {
            if (item != null
                && item.SelectedDrugForPrescription != null
                && item.SelectedDrugForPrescription.BrandName != ""
                )
            {
                return true;
            }
            else
            {
                MessageBox.Show(eHCMSResources.A0365_G1_Msg_InfoChonThuocDeRaToa, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return false;
            }
        }

        private bool ischanged(object item)
        {
            PrescriptionDetail p = item as PrescriptionDetail;
            if (p != null)
            {
                //if ((p.DrugID != null && p.DrugID != 0)
                //    || (p.IsDrugNotInCat// && p.SelectedDrugForPrescription!=null
                //    && p.BrandName != ""))
                if ((p.IsDrugNotInCat && !string.IsNullOrEmpty(p.SelectedDrugForPrescription.BrandName))
                    || (!p.IsDrugNotInCat && p.DrugID > 0))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        // TxD 11/04/2014: Added the following method to replace methods: CheckRegHasHI & IsPatientInsurance
        //                  and uniform the checking for HI Cover of Patient's Registration
        private bool RegistrationCoverByHI()
        {
            if (Registration_DataStorage.CurrentPatientRegistration == null)
            {
                return false;
            }
            if (Registration_DataStorage.CurrentPatientRegistration.HisID.HasValue && Registration_DataStorage.CurrentPatientRegistration.HisID.Value > 0)
            {
                return true;
            }
            return false;
        }

        private void BackupCurPrescriptionItem()
        {
            RetoreLatestePrecriptions = LatestePrecriptions.DeepCopy();
            PrecriptionsBeforeUpdate = LatestePrecriptions.DeepCopy();
        }

        private void RestoreCurPrescriptionItem()
        {
            LatestePrecriptions = RetoreLatestePrecriptions;
        }


        #region button member

        private void RemoveLastRowPrecriptionDetail()
        {
            int nCount = LatestePrecriptions.PrescriptionDetails.Count;

            if (nCount <= 0)
            {
                return;
            }

            PrescriptionDetail LastPrescriptDetail = LatestePrecriptions.PrescriptionDetails[nCount - 1];

            if (LastPrescriptDetail == null || LastPrescriptDetail.SelectedDrugForPrescription == null)
            {
                return;
            }

            if (LastPrescriptDetail.DrugID > 0 || (LastPrescriptDetail.IsDrugNotInCat && !string.IsNullOrEmpty(LastPrescriptDetail.SelectedDrugForPrescription.BrandName)))
            {
                return;
            }
            else
            {
                LatestePrecriptions.PrescriptionDetails.RemoveAt(nCount - 1);
            }

        }


        private bool CheckHoiChan()
        {
            if (IsEnabledAutoComplete)
            {
                if (LatestePrecriptions.ConsultantID == null)
                {
                    MessageBox.Show(eHCMSResources.A0341_G1_Msg_InfoChonNguoiHoiChan);
                    return false;
                }
            }
            return true;
        }

        public void btnUndo(object sender, RoutedEventArgs e)
        {
            PrescriptionNoteTemplates tmp = new PrescriptionNoteTemplates();
            tmp.PrescriptNoteTemplateID = -1;
            tmp.NoteDetails = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.K0616_G1_ChonMau);
            ObjPrescriptionNoteTemplates_Selected = tmp;


            IsEnabled = false;

            btnUndoIsEnabled = false;

            btChonChanDoanIsEnabled = false;

            btnCreateNewIsEnabled = true;
            btnSaveAddNewIsEnabled = false;

            btnUpdateIsEnabled = false;
            //bntSaveAsIsEnabled = false;

            RestoreCurPrescriptionItem();
            if (LatestePrecriptions != null && LatestePrecriptions.PrescriptID > 0)//Sửa
            {
                btnCopyToIsEnabled = true;
                if (CheckToaThuocDuocPhepCapNhat() == false)
                {
                    btnEditIsEnabled = false;
                }
                else
                {
                    btnEditIsEnabled = true;
                }
                btnCreateAndCopyIsEnabled = true;
                IsEnabledPrint = true;
            }
            else
            {
                LatestePrecriptions.DoctorAdvice = "";
                btnCopyToIsEnabled = false;
                IsEnabledPrint = false;
                btnEditIsEnabled = false;
                btnCreateAndCopyIsEnabled = false;
            }

        }

        //bool TaoThanhToaMoi = false;
        bool CapNhatToaThuoc = false;


        private Prescription _PhatHanhLai;
        public Prescription PhatHanhLai
        {
            get
            {
                return _PhatHanhLai;
            }
            set
            {
                if (_PhatHanhLai != value)
                {
                    _PhatHanhLai = value;
                    NotifyOfPropertyChange(() => PhatHanhLai);
                }
            }
        }

        private bool _isExpired = true;
        public bool isExpired
        {
            get { return _isExpired; }
            set
            {
                if (_isExpired != value)
                {
                    _isExpired = value;
                    NotifyOfPropertyChange(() => btnCopyToIsEnabled);
                    NotifyOfPropertyChange(() => isExpired);
                }
            }

        }

        private bool _btnCopyToIsEnabled;
        public bool btnCopyToIsEnabled
        {
            get { return _btnCopyToIsEnabled; }
            set
            {
                if (_btnCopyToIsEnabled != value)
                {
                    _btnCopyToIsEnabled = value;
                    NotifyOfPropertyChange(() => btnCopyToIsEnabled);
                }
            }

        }

        private bool IsPhatHanhLai = false;
        public void btnCopyTo(object sender, RoutedEventArgs e)
        {
            IsPhatHanhLai = true;
            long PrescriptID = LatestePrecriptions.PrescriptID;
            if (LatestePrecriptions.CanEdit)
            {
                //KMx: A.Tuân nói không kiểm tra ngày dùng vượt ngày BH qui định. Vì bác sĩ muốn ra bao nhiêu ngày cũng được (07/04/2014 16:02).
                //if (CheckIsVuotNgayQuiDinhHI())
                //{
                //    if (MessageBox.Show(eHCMSResources.A0838_G1_Msg_ConfTuDChinhNgDung, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                //    {
                //        //Tự động điều chỉnh
                //        AutoAdjust();
                //    }
                //}

                //Trạng thái đợi sửa 
                UpdateButtonsOnNewPrescriptBasedOnPrev(IsPhatHanhLai);
                GetPrescriptionDetailsByPrescriptID(PrescriptID, true);
            }
            else
            {
                switch (LatestePrecriptions.ReasonCanEdit)
                {
                    case "PhaiThucHien-TraPhieuTruoc":
                        {
                            MessageBox.Show(eHCMSResources.Z0999_G1_ToaDaBan, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                            break;
                        }
                    case "ToaNay-DaTungPhatHanh-VaSuDung":
                        {
                            MessageBox.Show(eHCMSResources.K0199_G1_KhongTheCNhatToaThuoc, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                            break;
                        }
                }

            }
            //if (MessageBox.Show(eHCMSResources.A0191_G1_Msg_ConfPHanhLaiToaThuoc, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            //{
            //    SetValueForPhatHanhLaiToaThuoc();
            //    AddPrescriptIssueHistory();
            //}
        }


        private bool _btDuocSiEditIsEnabled;
        public bool btDuocSiEditIsEnabled
        {
            get { return _btDuocSiEditIsEnabled; }
            set
            {
                if (_btDuocSiEditIsEnabled != value)
                {
                    _btDuocSiEditIsEnabled = value;
                    NotifyOfPropertyChange(() => btDuocSiEditIsEnabled);
                }
            }
        }


        private bool _btnUpdateIsEnabled;
        public bool btnUpdateIsEnabled
        {
            get { return _btnUpdateIsEnabled; }
            set
            {
                if (_btnUpdateIsEnabled != value)
                {
                    _btnUpdateIsEnabled = value;
                    NotifyOfPropertyChange(() => btnUpdateIsEnabled);
                }
            }
        }


        private bool _btnEditIsEnabled;
        public bool btnEditIsEnabled
        {
            get { return _btnEditIsEnabled; }
            set
            {
                if (_btnEditIsEnabled != value)
                {
                    _btnEditIsEnabled = value;
                    NotifyOfPropertyChange(() => btnEditIsEnabled);
                }
            }
        }
        private bool _btnCreateAndCopyIsEnabled;
        public bool btnCreateAndCopyIsEnabled
        {
            get { return _btnCreateAndCopyIsEnabled; }
            set
            {
                if (_btnCreateAndCopyIsEnabled != value)
                {
                    _btnCreateAndCopyIsEnabled = value;
                    NotifyOfPropertyChange(() => btnCreateAndCopyIsEnabled);
                }
            }
        }

        private bool CheckToaThuocDuocPhepCapNhat()
        {

            if (LatestePrecriptions.CreatorStaffID != Globals.LoggedUserAccount.StaffID)
            {
                //MessageBox.Show(string.Format("{0}!", eHCMSResources.Z0958_G1_ChiCoNguoiRaToaMoiDuocSua), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);/*khoi can thong bao*/
                return false;
            }
            //PrescripState = PrescriptionState.EditPrescriptionState;
            return true;
        }


        //KMx: Nội trú không cần kiểm tra (17/12/2014 17:00).
        //private bool ValidateExpiredPrescription(Prescription ToaThuoc)
        //{
        //    DateTime curDate = Globals.GetCurServerDateTime();
        //    if (ToaThuoc.PrescriptionIssueHistory.IssuedDateTime.Value.AddDays(7) < curDate)
        //    {
        //        btnCopyToIsEnabled = false;
        //        btnEditIsEnabled = false;
        //        Coroutine.BeginExecute(MessageWarningShowDialogTask(string.Format("{0}!", eHCMSResources.Z0991_G1_ToaHetHieuLucChSua)));
        //        return false;
        //    }
        //    return true;
        //}

        public void btnEdit()
        {
            if (!CheckEditPrescription())
            {
                return;
            }
            if (LatestePrecriptions.CanEdit)
            {
                //KMx: A.Tuân nói không kiểm tra ngày dùng vượt ngày BH qui định. Vì bác sĩ muốn ra bao nhiêu ngày cũng được (07/04/2014 16:02).
                //if (CheckIsVuotNgayQuiDinhHI())
                //{
                //    if (MessageBox.Show(eHCMSResources.A0838_G1_Msg_ConfTuDChinhNgDung, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                //    {
                //        //Tự động điều chỉnh
                //        AutoAdjust();
                //    }
                //}

                //Enable & Disable Buttons
                UpdateButtonsOnEditPrescription();

            }
            else
            {
                switch (LatestePrecriptions.ReasonCanEdit)
                {
                    case "PhaiThucHien-TraPhieuTruoc":
                        MessageBox.Show(eHCMSResources.Z0999_G1_ToaDaBan, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                        break;
                    case "ToaNay-DaTungPhatHanh-VaSuDung":
                        MessageBox.Show(eHCMSResources.K0199_G1_KhongTheCNhatToaThuoc, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                        break;
                    default:
                        MessageBox.Show(eHCMSResources.Z0690_G1_ReportBugsToBoss, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                        break;
                }
            }
        }

        private void UpdateButtonsOnEditPrescription()
        {
            IsEnabled = true;
            btnUndoIsEnabled = true;
            btnEditIsEnabled = false;
            btnCreateAndCopyIsEnabled = false;


            if (CheckToaThuocDuocPhepCapNhat() == false)
            {
                btnUpdateIsEnabled = false;
            }
            else
            {
                btnUpdateIsEnabled = true;
            }

            btChonChanDoanIsEnabled = true;

            btnCreateNewIsEnabled = false;
            btnSaveAddNewIsEnabled = false;

            //bntSaveAsIsEnabled = false;

            btnCopyToIsEnabled = false;
            IsEnabledPrint = false;

            IsWaitingSaveDuocSiEdit = false;

            BackupCurPrescriptionItem();
            GetRemainingPrescriptionDetailsByPrescriptID(LatestePrecriptions.PrescriptID, true);
            AddNewBlankDrugIntoPrescriptObjectNew();
        }

        private void UpdateButtonsOnNewPrescriptBasedOnPrev(bool bPhatHanhLai)
        {
            IsEnabled = true;
            btnUndoIsEnabled = true;
            btnEditIsEnabled = false;
            btnCreateAndCopyIsEnabled = false;


            btnUpdateIsEnabled = false;

            btChonChanDoanIsEnabled = true;

            btnCreateNewIsEnabled = false;
            btnSaveAddNewIsEnabled = true;

            //bntSaveAsIsEnabled = true;

            btnCopyToIsEnabled = false;
            IsEnabledPrint = false;

            IsWaitingSaveDuocSiEdit = false;

            BackupCurPrescriptionItem();
            //if (!bPhatHanhLai)
            //{
            //    LatestePrecriptions.Diagnosis = "";
            //}
            LatestePrecriptions.ExamDate = Registration_DataStorage.CurrentPatientRegistration.ExamDate;

            AddNewBlankDrugIntoPrescriptObjectNew();

            if (bPhatHanhLai)
            {
                return;
            }
            //20191107 TBL: Khi tạo mới dựa trên cũ toa thuốc thì clear trường chẩn đoán và cách điều trị của toa thuốc cũ
            LatestePrecriptions.ObjDiagnosisTreatment = new DiagnosisTreatment();
            //KMx: Khi bấm "Tạo mới dựa trên toa cũ" thì phải tính lại số lượng cho thuốc Thường. Do thay đổi công thức tính số lượng, nên một số toa cũ có thể bị lỗi (12/06/2014 16:27).
            ObservableCollection<PrescriptionDetail> NormalDrugList = LatestePrecriptions.PrescriptionDetails.Where(x => !x.IsDrugNotInCat && x.V_DrugType == (long)AllLookupValues.V_DrugType.THUOC_THONGTHUONG).ToObservableCollection();

            if (NormalDrugList.Count > 0)
            {
                foreach (PrescriptionDetail prescriptDetail in NormalDrugList)
                {
                    CalcTotalQtyForDrugItem(prescriptDetail);
                }
            }

            //KMx: Nếu tạo mới dựa trên toa cũ thì phải set lại OrigIssuedDateTime = null, nếu không sẽ tính thuốc lịch sai vì lấy ngày ra toa của toa cũ (04/06/2014 15:21).
            LatestePrecriptions.OrigIssuedDateTime = null;

            //KMx: Khi bấm "Tạo mới dựa trên toa cũ" thì phải tính lại số lượng cho thuốc Lịch. Vì có thể toa cũ được tạo vào thứ 3, nhưng ngày dựa trên toa cũ có thể không phải là thứ 3 (04/06/2014 14:48).
            ObservableCollection<PrescriptionDetail> ScheduleDrugList = LatestePrecriptions.PrescriptionDetails.Where(x => !x.IsDrugNotInCat && x.HasSchedules).ToObservableCollection();

            if (ScheduleDrugList.Count <= 0)
            {
                return;
            }

            //byte Today = Globals.GetDayOfWeek(Globals.ServerDate.Value);

            foreach (PrescriptionDetail prescriptDetail in ScheduleDrugList)
            {
                //KMx: QtyMaxAllowed của những toa cũ có thể bị sai, cho nên khi "Tạo mới dựa trên toa cũ" thì phải đi tính lại, bất chấp SchedBeginDOW có bằng Today hay không (14/06/2014 10:58).
                //if (prescriptDetail.SchedBeginDOW == Today)
                //{
                //    return;
                //}

                CalQtyAndQtyMaxForSchedule(prescriptDetail);
            }

        }

        private void Prescriptions_Update()
        {
            //IsEnabledForm = false;

            //IsWaitingCapNhatToaThuoc = true;
            IList<PrescriptionIssueHistory> lstPrescriptionIssueHistory = new List<PrescriptionIssueHistory>();
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            if (UseOnlyDailyDiagnosis && CurrentDiagnosisTreatment != null && CurrentDiagnosisTreatment.DTItemID > 0)
            {
                ObjTaoThanhToaMoi.ConfirmedDTItemID = CurrentDiagnosisTreatment.DTItemID;
            }
            this.ShowBusyIndicator();

            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ePrescriptionsServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginPrescriptions_InPt_Update(ObjTaoThanhToaMoi, PrecriptionsBeforeUpdate, AllowUpdateThoughReturnDrugNotEnough
                            , Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                string Result = "";

                                long NewPrescriptionID = 0;
                                long IssueID = 0;
                                string ServerError = "";
                                contract.EndPrescriptions_InPt_Update(out Result, out NewPrescriptionID, out IssueID, out ServerError, asyncResult);
                                AllowUpdateThoughReturnDrugNotEnough = false;
                                string druglist = Result.IndexOf("@") > 0 ?
                                    Result.Substring(Result.IndexOf("@"), Result.Length - Result.IndexOf("@")) : "None";
                                Result = Result.Replace(druglist, "");
                                switch (Result)
                                {
                                    case "OK":
                                        {
                                            ObjTaoThanhToaMoi.PrescriptID = NewPrescriptionID;
                                            ObjTaoThanhToaMoi.IssueID = IssueID;

                                            PrecriptionsForPrint = ObjTaoThanhToaMoi;
                                            IsEnabledAutoComplete = false;

                                            //bntSaveAsVisibility = Visibility.Collapsed;
                                            //btnUpdateVisibility = true;


                                            IsEnabled = false;
                                            btnUpdateIsEnabled = false;
                                            btnSaveAddNewIsEnabled = false;
                                            btnUndoIsEnabled = false;
                                            btnCreateNewIsEnabled = true;
                                            btnEditIsEnabled = true;
                                            btnCreateAndCopyIsEnabled = true;
                                            IsEnabledPrint = true;

                                            btChonChanDoanIsEnabled = false;

                                            //phát sự kiện load lại danh sách toa thuốc
                                            //Globals.EventAggregator.Publish(new ReloadDataePrescriptionEvent { });
                                            ////phát sự kiện load lại danh sách toa thuốc
                                            ////đọc lại toa thuốc cuối
                                            //GetLatestPrescriptionByPtID(Registration_DataStorage.CurrentPatient.PatientID);
                                            ////đọc lại toa thuốc cuối

                                            //Dinh sua
                                            //Globals.EventAggregator.Publish(new ReloadDataConsultationEvent { });


                                            // Txd 06/11/2013:
                                            // DO NOT PUBLISH Event ReloadDataConsultationEvent anymore
                                            // instead call Methods of the View Model directly.
                                            if (Registration_DataStorage.CurrentPatient != null)
                                            {
                                                //KMx: Không gọi hàm GetLatestPrescriptionByPtID() nữa (17/05/2014 09:49).
                                                //Vì trong hàm consultVM.PatientServiceRecordsGetForKhamBenh_Ext() sẽ bắn event về VM này và làm những việc tương tự. 
                                                //GetLatestPrescriptionByPtID(Registration_DataStorage.CurrentPatient.PatientID);

                                                Globals.EventAggregator.Publish(new ClearPrescriptionListAfterUpdateEvent());

                                                Globals.EventAggregator.Publish(new ClearDrugUsedAfterUpdateEvent());

                                                IConsultationModule consultVM = Globals.GetViewModel<IConsultationModule>();
                                                if (consultVM.MainContent is IConsultationsSummary_InPt)
                                                {
                                                    (consultVM.MainContent as IConsultationsSummary_InPt).CallPatientServiceRecordsGetForKhamBenh_Ext();
                                                }
                                                else
                                                {
                                                    consultVM.PatientServiceRecordsGetForKhamBenh_Ext();
                                                }
                                                //KMx: Sau khi lưu toa thuốc, không cần load lại danh sách chẩn đoán (21/05/2014 15:17).
                                                //IConsultationList consultListVM = Globals.GetViewModel<IConsultationList>();

                                                //consultListVM.GetDiagTrmtsByPtID_Ext();

                                            }

                                            MessageBox.Show(eHCMSResources.A0279_G1_Msg_InfoCNhatOK, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                            //20191120 TBL: Nếu lưu toa thuốc thành công thì bắn sự kiện để màn hình Xuất viện BN lấy xác nhận chẩn đoán của toa thuốc cho xuất viện
                                            Globals.EventAggregator.Publish(new LoadDiagnosisTreatmentConfirmedForPrescript());
                                            break;
                                        }
                                    case "Error":
                                        {
                                            //Cộng lại dòng trống
                                            AddNewBlankDrugIntoPrescriptObjectNew();
                                            //Cộng lại dòng trống

                                            MessageBox.Show(eHCMSResources.A0272_G1_Msg_InfoCNhatFail, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                            break;
                                        }
                                    case "PhaiThucHien-TraPhieuTruoc":
                                        {
                                            //Cộng lại dòng trống
                                            AddNewBlankDrugIntoPrescriptObjectNew();
                                            //Cộng lại dòng trống

                                            Coroutine.BeginExecute(WarningReturnDrugNotEnough());
                                            //MessageBox.Show(string.Format("{0}!", eHCMSResources.Z0999_G1_ToaDaBan), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                            break;
                                        }
                                    case "ToaNay-DaTungPhatHanh-VaSuDung":
                                        {
                                            //Cộng lại dòng trống
                                            AddNewBlankDrugIntoPrescriptObjectNew();
                                            //Cộng lại dòng trống

                                            MessageBox.Show(eHCMSResources.K0199_G1_KhongTheCNhatToaThuoc, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                            break;
                                        }
                                    case "Duplex-Prescriptions_PrescriptionsInDay":
                                        {
                                            //Cộng lại dòng trống
                                            AddNewBlankDrugIntoPrescriptObjectNew();
                                            //Cộng lại dòng trống

                                            MessageBox.Show(eHCMSResources.K0152_G1_ToaCoThuocBiTrungTrongNg
                                                + Environment.NewLine + druglist.Replace("@", "")
                                                + Environment.NewLine + eHCMSResources.K0151_G1_KTraThuocTrongToa, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                            break;
                                        }
                                    case "Error-Exception":
                                        {
                                            //Cộng lại dòng trống
                                            AddNewBlankDrugIntoPrescriptObjectNew();
                                            //Cộng lại dòng trống
                                            if (!string.IsNullOrEmpty(ServerError))
                                            {
                                                MessageBox.Show(ServerError, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                            }
                                            else
                                            {
                                                MessageBox.Show(eHCMSResources.A0272_G1_Msg_InfoCNhatFail, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                            }
                                            break;
                                        }
                                }

                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.ToString());
                            }
                            finally
                            {
                                //IsEnabledForm = true;
                                //IsWaitingCapNhatToaThuoc = false;
                                this.HideBusyIndicator();
                            }

                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
                finally
                {
                    this.HideBusyIndicator();
                }
            });

            t.Start();
        }

        public IEnumerator<IResult> WarningReturnDrugNotEnough()
        {
            //var dialog = new MessageWarningShowDialogTask(eHCMSResources.Z1066_G1_ToaDaBanDongYCNhatKg, "Đồng ý");
            //yield return dialog;

            var dialog = new WarningWithConfirmMsgBoxTask(eHCMSResources.Z1066_G1_ToaDaBanDongYCNhatKg, eHCMSResources.K3847_G1_DongY, true);
            yield return dialog;

            if (dialog.IsAccept)
            {
                AllowUpdateThoughReturnDrugNotEnough = true;
                Prescriptions_Update();
            }
            yield break;
        }

        public void btUpdateDoctorAdvice()
        {
            if (Registration_DataStorage.CurrentPatientRegistration != null && Globals.IsLockRegistration(Registration_DataStorage.CurrentPatientRegistration.RegLockFlag, eHCMSResources.Z1067_G1_CNhatLoiDanBSi))
            {
                return;
            }
            Prescriptions_UpdateDoctorAdvice();
        }

        private void Prescriptions_UpdateDoctorAdvice()
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });

            IsEnabledForm = false;

            IsWaitingPrescriptions_UpdateDoctorAdvice = true;

            var t = new Thread(() =>
            {
                using (var serviceFactory = new ePrescriptionsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginPrescriptions_UpdateDoctorAdvice(LatestePrecriptions, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            string Result = "";
                            contract.EndPrescriptions_UpdateDoctorAdvice(out Result, asyncResult);

                            if (Result == "1")
                            {
                                //IsEnabledAutoComplete = false;
                                //IsEnabled = false;
                                //btnEditIsEnabled = true;
                                //btnCreateAndCopyIsEnabled = true;
                                //IsEnabledPrint = true;
                                RetoreLatestePrecriptions.DoctorAdvice = LatestePrecriptions.DoctorAdvice;
                                btnUndo(null, null);
                                MessageBox.Show(eHCMSResources.A0279_G1_Msg_InfoCNhatOK, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                            }
                            else
                            {
                                MessageBox.Show(eHCMSResources.A0272_G1_Msg_InfoCNhatFail, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                        finally
                        {
                            IsEnabledForm = true;

                            IsWaitingPrescriptions_UpdateDoctorAdvice = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }


        private float ChangeDoseStringToFloat(string value)
        {
            float result = 0;
            if (!string.IsNullOrEmpty(value))
            {
                if (value.Contains("/"))
                {
                    string pattern = @"\b[\d]+/[\d]+\b";
                    if (!Regex.IsMatch(value, pattern))
                    {
                        Globals.ShowMessage(eHCMSResources.Z1069_G1_LieuDungKgHopLe, eHCMSResources.G0442_G1_TBao);
                        return 0;
                    }
                    else
                    {
                        string[] items = null;
                        items = value.Split('/');
                        if (items.Count() > 2 || items.Count() == 0)
                        {
                            Globals.ShowMessage(eHCMSResources.Z1069_G1_LieuDungKgHopLe, eHCMSResources.G0442_G1_TBao);
                            return 0;
                        }
                        else if (float.Parse(items[1]) == 0)
                        {
                            Globals.ShowMessage(eHCMSResources.Z1069_G1_LieuDungKgHopLe, eHCMSResources.G0442_G1_TBao);
                            return 0;
                        }

                        //KMx: Không được Round số lượng. Nếu không sẽ bị sai trong trường hợp thuốc 1/7 viên * 35 ngày.
                        //Kết quả không Round là 5, kết quả sau khi Round là 6.
                        //result = (float)Math.Round((float.Parse(items[0]) / float.Parse(items[1])), 3);

                        result = (float.Parse(items[0]) / float.Parse(items[1]));

                        if (result < 0)
                        {
                            Globals.ShowMessage(eHCMSResources.Z1071_G1_LieuDungKgNhoHon0, eHCMSResources.G0442_G1_TBao);
                            return 0;
                        }
                    }
                }
                else
                {
                    try
                    {
                        result = float.Parse(value);
                        if (result < 0)
                        {
                            Globals.ShowMessage(eHCMSResources.Z1071_G1_LieuDungKgNhoHon0, eHCMSResources.G0442_G1_TBao);
                            return 0;
                        }
                    }
                    catch
                    {
                        Globals.ShowMessage(eHCMSResources.Z1069_G1_LieuDungKgHopLe, eHCMSResources.G0442_G1_TBao);
                        return 0;
                    }
                }
            }
            return result;
        }

        private bool IsBenhNhanNoiTru()
        {
            //cho nay can coi lai vi ben nha thuoc sua toa thuoc se khong co RegistrationInfo
            if (Registration_DataStorage.CurrentPatientRegistration != null && Registration_DataStorage.CurrentPatient != null && Registration_DataStorage.CurrentPatient.PatientID > 0)
            {
                if (Registration_DataStorage.CurrentPatientRegistration.V_RegistrationType ==
                    AllLookupValues.RegistrationType.NOI_TRU)
                {
                    return true;
                }
                return false;
            }
            return false;
        }


        private void ChangeAnyDoseQty(int nDoseType, string strDoseQty, PrescriptionDetail presDetailObj)
        {
            if (presDetailObj == null || presDetailObj.SelectedDrugForPrescription == null)
            {
                MessageBox.Show(eHCMSResources.A0545_G1_Msg_InfoEnglish3);
                return;
            }

            int nDayVal = LatestePrecriptions.NDay == null ? 0 : LatestePrecriptions.NDay.Value;
            if (presDetailObj.RealDay <= 0 && nDayVal > 0)
            {
                //KMx: Không gọi hàm GetDayRpt nữa, vì trong hàm này có gọi hàm tính Qty rồi (13/06/2014 11:12). 
                //GetDayRpt(presDetailObj, nDayVal);

                if (presDetailObj.SelectedDrugForPrescription.MaxDayPrescribed > 0 && nDayVal > presDetailObj.SelectedDrugForPrescription.MaxDayPrescribed)
                {
                    SetDefaultDay(presDetailObj, (int)presDetailObj.SelectedDrugForPrescription.MaxDayPrescribed);
                }
                else
                {
                    SetDefaultDay(presDetailObj, nDayVal);
                }

            }

            bool bHasSchedule = presDetailObj.HasSchedules;
            float fDoseQty = 0;
            if (strDoseQty != null && strDoseQty.Length > 0)
            {
                fDoseQty = ChangeDoseStringToFloat(strDoseQty);
            }
            switch (nDoseType)
            {
                case 1:
                    Debug.WriteLine(" =====> MORNING Dose END EDITING .........");
                    if (bHasSchedule)
                    {
                        presDetailObj.MDoseStr = "0";
                        presDetailObj.MDose = 0;
                    }
                    else
                    {
                        presDetailObj.MDose = fDoseQty;
                    }
                    break;
                case 2:
                    Debug.WriteLine(" =====> NOON Dose END EDITING .........");
                    if (bHasSchedule)
                    {
                        presDetailObj.NDoseStr = "0";
                        presDetailObj.NDose = 0;
                    }
                    else
                    {
                        presDetailObj.NDose = fDoseQty;
                    }
                    break;
                case 3:
                    Debug.WriteLine(" =====> AFTERNOON Dose END EDITING .........");
                    if (bHasSchedule)
                    {
                        presDetailObj.ADoseStr = "0";
                        presDetailObj.ADose = 0;
                    }
                    else
                    {
                        presDetailObj.ADose = fDoseQty;
                    }
                    break;
                case 4:
                    Debug.WriteLine(" =====> EVENING Dose END EDITING .........");
                    if (bHasSchedule)
                    {
                        presDetailObj.EDoseStr = "0";
                        presDetailObj.EDose = 0;
                    }
                    else
                    {
                        presDetailObj.EDose = fDoseQty;
                    }
                    break;

            }

            if (presDetailObj.HasSchedules)
            {
                MessageBox.Show(string.Format(eHCMSResources.Z0989_G1_ThuocDaCoCDinhUongTheoTuan, presDetailObj.SelectedDrugForPrescription.BrandName.Trim()));
            }
            else
            {
                //Nếu thuốc ngoài danh mục thì chỉ cần tính Qty thôi. Ngược lại, phải tính thêm QtyForDay (thuốc thường), QtyMaxAllowed (thuốc thường, cần, lịch để in lên report) (13/06/2014 14:27).
                if (presDetailObj.IsDrugNotInCat)
                {
                    presDetailObj.Qty = CalQtyForNormalDrug(presDetailObj, presDetailObj.RealDay);
                }
                else
                {
                    CalcTotalQtyForDrugItem(presDetailObj);
                }
            }

        }

        #endregion


        #region Tạo Toa Mới


        //KMx: Hàm này được kết hợp từ 2 hàm SetValueTaoThanhToaMoi() và SetValueTaoThanhToaMoi_CreateNew() (01/06/2014 10:58).
        //Lý do: 2 hàm đó giống nhau.
        private void SetValueTaoThanhToaMoi_New()
        {
            if (Registration_DataStorage.CurrentPatientRegistration == null)
            {
                MessageBox.Show(eHCMSResources.A0704_G1_Msg_InfoKhTheRaToa);
                return;
            }

            ObjTaoThanhToaMoi = LatestePrecriptions.DeepCopy();
            //ObjTaoThanhToaMoi.NDay = CheckAppDate();
            ObjTaoThanhToaMoi.CreatorStaffID = Globals.LoggedUserAccount.StaffID.GetValueOrDefault();

            if (CapNhatToaThuoc)
            {
                ObjTaoThanhToaMoi.V_PrescriptionNotes = (long)AllLookupValues.V_PrescriptionNotes.EDITTUTOAKHAC;
            }
            else
            {
                ObjTaoThanhToaMoi.V_PrescriptionNotes = (long)AllLookupValues.V_PrescriptionNotes.TOAGOC;
            }

            //KMx: OriginalPrescriptID sẽ do Stored quyết định. Không cần set ở đây (01/06/2014 16:24).
            //ObjTaoThanhToaMoi.OriginalPrescriptID = LatestePrecriptions.OriginalPrescriptID;/*Gốc Của Phát Hành Này*/

            ObjTaoThanhToaMoi.PrescriptionIssueHistory = new PrescriptionIssueHistory();
            ObjTaoThanhToaMoi.PrescriptionIssueHistory.DeptLocID = Globals.DeptLocation.DeptLocationID;

            //KMx: Khi cập nhật thì nhìn vào HisID của đăng ký, không nhìn vào HisID của toa trước khi cập nhật.
            //Sửa lỗi: Khi BS ra toa, BN chưa có BH, sau đó BN đi xác nhận BH, BS cập nhật lại toa thì toa đó phải là toa BH.
            //ObjTaoThanhToaMoi.PrescriptionIssueHistory.HisID = LatestePrecriptions.PrescriptionIssueHistory.HisID;

            if (RegistrationCoverByHI())
            {
                ObjTaoThanhToaMoi.PrescriptionIssueHistory.HisID = Registration_DataStorage.CurrentPatientRegistration.HisID.Value;
            }
            else
            {
                ObjTaoThanhToaMoi.PrescriptionIssueHistory.HisID = 0;
            }



            //Gán lại ServiceRecID, PtRegDetailID
            ObjTaoThanhToaMoi.ServiceRecID = ObjDiagnosisTreatment_Current.ServiceRecID;

            //KMx: Phải set lại những thuộc tính bên dưới, nếu không sẽ bị lỗi lưu lại PtRegistrationID cũ khi "Tạo mới dựa trên toa cũ".
            ObjTaoThanhToaMoi.PtRegistrationCode = Registration_DataStorage.CurrentPatientRegistration.PtRegistrationCode;
            ObjTaoThanhToaMoi.PtRegistrationID = Registration_DataStorage.CurrentPatientRegistration.PtRegistrationID;

            //Cụ thể  DV nào

            if (Registration_DataStorage.CurrentPatientRegistration.V_RegistrationType == AllLookupValues.RegistrationType.NGOAI_TRU)
            {
                if (Registration_DataStorage.CurrentPatientRegistrationDetail != null && Registration_DataStorage.CurrentPatientRegistrationDetail.PtRegDetailID > 0)
                {
                    ObjTaoThanhToaMoi.PrescriptionIssueHistory.PtRegDetailID = Registration_DataStorage.CurrentPatientRegistrationDetail.PtRegDetailID;
                }
                else
                {
                    MessageBox.Show(eHCMSResources.A0705_G1_Msg_KhTheRaToaNgTru);
                    return;
                }
            }
            else/*Khám VIP, Khám Cho Nội Trú*/
            {
                ObjTaoThanhToaMoi.PrescriptionIssueHistory.PtRegDetailID = 0;
            }
            //Cụ thể  DV nào

            //Gán lại ServiceRecID, PtRegDetailID
        }


        private void SetValueToaOfDuocSi()
        {
            ObjToaOfDuocSi = LatestePrecriptions.DeepCopy();
            //ObjToaOfDuocSi.NDay = CheckAppDate();
            ObjToaOfDuocSi.V_PrescriptionNotes = (long)AllLookupValues.V_PrescriptionNotes.TOAGOC;
            ObjToaOfDuocSi.OriginalPrescriptID = LatestePrecriptions.OriginalPrescriptID;/*Gốc Của Phát Hành Này*/

            ObjToaOfDuocSi.PrescriptionIssueHistory = new PrescriptionIssueHistory();
            ObjToaOfDuocSi.PrescriptionIssueHistory.DeptLocID = Globals.DeptLocation.DeptLocationID;
            //Gán lại ServiceRecID, PtRegDetailID
            ObjToaOfDuocSi.ServiceRecID = LatestePrecriptions.ObjDiagnosisTreatment.ServiceRecID;
            ObjToaOfDuocSi.PrescriptionIssueHistory.PtRegDetailID = LatestePrecriptions.ObjDiagnosisTreatment.PtRegDetailID;

            //Cụ thể  DV nào  Notes Ny: Cho nay can xem lai ti,hinh nhu ko can cai nay do dsi chi sua toa thuoc
            //nen chi can lay lai PtRegDetailID cu la dc
            if (Registration_DataStorage.CurrentPatientRegistration != null)
            {
                if (Registration_DataStorage.CurrentPatientRegistration.V_RegistrationType == AllLookupValues.RegistrationType.NGOAI_TRU)
                {
                    if (Registration_DataStorage.CurrentPatientRegistrationDetail != null && Registration_DataStorage.CurrentPatientRegistrationDetail.PtRegDetailID > 0)
                    {
                        ObjToaOfDuocSi.PrescriptionIssueHistory.PtRegDetailID = LatestePrecriptions.ObjDiagnosisTreatment.PtRegDetailID;
                    }
                    else
                    {
                        MessageBox.Show(eHCMSResources.A0705_G1_Msg_KhTheRaToaNgTru);
                        return;
                    }
                }
                else/*Khám VIP, Khám Cho Nội Trú*/
                {
                    ObjToaOfDuocSi.PrescriptionIssueHistory.PtRegDetailID = 0;
                }
            }
            //else
            //{
            //    MessageBox.Show(eHCMSResources.A0704_G1_Msg_InfoKhTheRaToa);
            //    return;
            //}
            //Cụ thể  DV nào

            ObjToaOfDuocSi.ModifierStaffID = Globals.LoggedUserAccount.Staff.StaffID;
            //Gán lại ServiceRecID, PtRegDetailID
        }

        private Prescription _ObjTaoThanhToaMoi;
        public Prescription ObjTaoThanhToaMoi
        {
            get
            {
                return _ObjTaoThanhToaMoi;
            }
            set
            {
                if (_ObjTaoThanhToaMoi != value)
                {
                    _ObjTaoThanhToaMoi = value;
                    NotifyOfPropertyChange(() => ObjTaoThanhToaMoi);
                }
            }
        }

        private Prescription _ObjToaOfDuocSi;
        public Prescription ObjToaOfDuocSi
        {
            get
            {
                return _ObjToaOfDuocSi;
            }
            set
            {
                if (_ObjToaOfDuocSi != value)
                {
                    _ObjToaOfDuocSi = value;
                    NotifyOfPropertyChange(() => ObjToaOfDuocSi);
                }
            }
        }

        private void Prescriptions_Add()
        {
            if (UseOnlyDailyDiagnosis)
            {
                ObjTaoThanhToaMoi.ConfirmedDTItemID = CurrentDiagnosisTreatment.DTItemID;
            }
            //TaoThanhToaMoi = false;
            //IsEnabledForm = false;
            //IsWaitingTaoThanhToaMoi = true;
            long IssueID = 0;
            long PrescriptionID = 0;
            string OutError = "";
            IList<PrescriptionIssueHistory> lstPrescriptionIssueHistory = new List<PrescriptionIssueHistory>();
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ePrescriptionsServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        //contract.BeginPrescriptions_Add(Globals.NumberTypePrescriptions_Rule, ObjTaoThanhToaMoi, Globals.DispatchCallback((asyncResult) =>
                        // Txd 25/05/2014 Replaced ConfigList
                        contract.BeginPrescriptions_InPt_Add(ObjTaoThanhToaMoi, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                if (contract.EndPrescriptions_InPt_Add(out PrescriptionID, out IssueID, out OutError, asyncResult))
                                {
                                    ObjTaoThanhToaMoi.PrescriptID = PrescriptionID;
                                    ObjTaoThanhToaMoi.IssueID = IssueID;

                                    PrecriptionsForPrint = ObjTaoThanhToaMoi;
                                    IsEnabledAutoComplete = false;

                                    //bntSaveAsVisibility = Visibility.Collapsed;
                                    //btnUpdateVisibility = Visibility.Collapsed;

                                    IsEnabled = false;
                                    btnSaveAddNewIsEnabled = false;
                                    btnUndoIsEnabled = false;
                                    //bntSaveAsIsEnabled = false;
                                    btnCreateNewIsEnabled = true;
                                    btnEditIsEnabled = true;
                                    btnCreateAndCopyIsEnabled = true;
                                    IsEnabledPrint = true;

                                    btChonChanDoanIsEnabled = false;

                                    //MessageBox.Show(string.Format("{0}!", eHCMSResources.A0468_G1_Msg_InfoLuuOK));

                                    ////phat su kien load lai toa thuoc cuoi
                                    //Globals.EventAggregator.Publish(new ReloadDataePrescriptionEvent { });
                                    ////PrescripState = PrescriptionState.EditPrescriptionState;

                                    //Dinh sua
                                    //Globals.EventAggregator.Publish(new ReloadDataConsultationEvent { });

                                    // Txd 06/11/2013:
                                    // DO NOT PUBLISH Event ReloadDataConsultationEvent anymore
                                    // instead call Methods of the View Model directly.
                                    if (Registration_DataStorage.CurrentPatient != null)
                                    {
                                        //KMx: Không gọi hàm GetLatestPrescriptionByPtID() nữa (17/05/2014 09:49).
                                        //Vì trong hàm consultVM.PatientServiceRecordsGetForKhamBenh_Ext() sẽ bắn event về VM này và làm những việc tương tự.
                                        //GetLatestPrescriptionByPtID(Registration_DataStorage.CurrentPatient.PatientID);

                                        Globals.EventAggregator.Publish(new ClearPrescriptionListAfterAddNewEvent());

                                        Globals.EventAggregator.Publish(new ClearDrugUsedAfterAddNewEvent());

                                        IConsultationModule consultVM = Globals.GetViewModel<IConsultationModule>();
                                        if (consultVM.MainContent is IConsultationsSummary_InPt)
                                        {
                                            (consultVM.MainContent as IConsultationsSummary_InPt).CallPatientServiceRecordsGetForKhamBenh_Ext();
                                        }
                                        else
                                        {
                                            consultVM.PatientServiceRecordsGetForKhamBenh_Ext();
                                        }
                                        //KMx: Sau khi lưu toa thuốc, không cần load lại danh sách chẩn đoán (21/05/2014 15:17).
                                        //IConsultationList consultListVM = Globals.GetViewModel<IConsultationList>();

                                        //consultListVM.GetDiagTrmtsByPtID_Ext();

                                    }

                                    MessageBox.Show(eHCMSResources.K0175_G1_ToaThuocDaLuuOk);
                                    PrescripState = PrescriptionState.EditPrescriptionState;
                                    //20191120 TBL: Nếu lưu toa thuốc thành công thì bắn sự kiện để màn hình Xuất viện BN lấy xác nhận chẩn đoán của toa thuốc cho xuất viện
                                    Globals.EventAggregator.Publish(new LoadDiagnosisTreatmentConfirmedForPrescript());

                                }
                                else
                                {
                                    //Cộng lại dòng trống
                                    AddNewBlankDrugIntoPrescriptObjectNew();
                                    //Cộng lại dòng trống
                                    if (OutError.Contains("Duplex-Prescriptions_PrescriptionsInDay"))
                                    {
                                        MessageBox.Show(eHCMSResources.K0152_G1_ToaCoThuocBiTrungTrongNg
                                            + Environment.NewLine + OutError.Replace("Duplex-Prescriptions_PrescriptionsInDay@", "")
                                            + Environment.NewLine + eHCMSResources.K0151_G1_KTraThuocTrongToa, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                    }
                                    else
                                    {
                                        MessageBox.Show(OutError + Environment.NewLine + eHCMSResources.I0944_G1_I);
                                    }
                                }

                            }
                            catch (Exception ex)
                            {
                                //20191018 TBL: Nếu có lỗi khi lưu toa thuốc thì thêm dòng rỗng lại
                                AddNewBlankDrugIntoPrescriptObjectNew();
                                MessageBox.Show(ex.Message, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                            }
                            finally
                            {
                                //IsEnabledForm = true;

                                //IsWaitingTaoThanhToaMoi = false;
                                this.HideBusyIndicator();
                            }

                        }), null);

                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                }
                finally
                {
                    this.HideBusyIndicator();
                }
            });

            t.Start();
        }
        #endregion


        #region IHandle<ePrescriptionSelectedEvent> Members

        private Prescription _PrecriptionsBeforeUpdate;
        public Prescription PrecriptionsBeforeUpdate
        {
            get
            {
                return _PrecriptionsBeforeUpdate;
            }
            set
            {
                if (_PrecriptionsBeforeUpdate != value)
                {
                    _PrecriptionsBeforeUpdate = value;
                    NotifyOfPropertyChange(() => PrecriptionsBeforeUpdate);
                }
            }
        }

        public Staff ObjStaff
        {
            get
            {
                return Globals.LoggedUserAccount.Staff;
            }
        }


        /*Đọc chi tiết 1 Toa khi Double Click chọn từ danh sách Toa Thuốc*/
        public void Handle(ePrescriptionDoubleClickEvent_InPt_2 message)
        {
            if (message != null)
            {
                IsEnabled = false;

                btnSaveAddNewIsEnabled = false;
                btnUndoIsEnabled = false;
                btnCreateNewIsEnabled = true;
                btnCopyToIsEnabled = true;
                IsEnabledPrint = true;
                //btnEditIsEnabled = true;
                btnUpdateIsEnabled = false;

                if (message.isTemplate)
                {
                    LatestePrecriptions.PrescriptionDetails = ObjectCopier.DeepCopy(message.SelectedPrescription.PrescriptionDetails);
                }
                else
                {
                    LatestePrecriptions = message.SelectedPrescription.DeepCopy();
                }
                NotifyOfPropertyChange(() => LatestePrecriptions.PrescriptionDetails);
                //if (CheckToaThuocDuocPhepCapNhat() == false)
                //{
                //    btnEditIsEnabled = false;
                //}

                if (ObjDiagnosisTreatment_Current != null && LatestePrecriptions != null && ObjDiagnosisTreatment_Current.ServiceRecID == LatestePrecriptions.ServiceRecID
                    && CheckToaThuocDuocPhepCapNhat())
                {
                    btnEditIsEnabled = true;
                }
                else
                {
                    btnEditIsEnabled = false;
                }

                btnCreateAndCopyIsEnabled = true;

                if (!LatestePrecriptions.PrescriptionIssueHistory.IssuedDateTime.HasValue)
                {
                    LatestePrecriptions.PrescriptionIssueHistory.IssuedDateTime = LatestePrecriptions.IssuedDateTime;
                }

                //KMx: Nội trú không cần kiểm tra (17/12/2014 17:00).
                //if (PrescripState != PrescriptionState.NewPrescriptionState)
                //{
                //    ValidateExpiredPrescription(LatestePrecriptions);
                //}


                if (LatestePrecriptions.NDay > 0)
                {
                    chkHasAppointmentValue = true;
                }
                else
                {
                    chkHasAppointmentValue = false;
                }

                //NotifyOfPropertyChange(() => txtDaysAfterIsEnabled);

                PrecriptionsBeforeUpdate = message.SelectedPrescription.DeepCopy();

                //HideButton = Visibility.Visible;
                //GetPrescriptionDetailsByPrescriptID(LatestePrecriptions.PrescriptID);
                BackupCurPrescriptionItem();

                PrecriptionsForPrint = message.SelectedPrescription.DeepCopy();

                ContentKhungTaiKhamIsEnabled = LatestePrecriptions.IsAllowEditNDay;

            }

        }


        #endregion

        #region printing member

        public void btnPreview()
        {
            Action<ICommonPreviewView> onInitDlg = delegate (ICommonPreviewView proAlloc)
            {
                //proAlloc.IssueID = (int)PrecriptionsForPrint.IssueID;
                proAlloc.IssueID = PrecriptionsForPrint.IssueID;
                proAlloc.eItem = ReportName.CONSULTATION_TOATHUOC_INPT;
                proAlloc.parTypeOfForm = Globals.ServerConfigSection.ConsultationElements.IsSeparatePsychotropicPrescription_Inpt == true ? 1 : 0;
                /*▼====: #001*/
                if (LatestePrecriptions == null || LatestePrecriptions.PrescriptionDetails == null || LatestePrecriptions.PrescriptionDetails.Count == 0)
                    proAlloc.parTypeOfForm = 0;
                /*▲====: #001*/
            };
            GlobalsNAV.ShowDialog<ICommonPreviewView>(onInitDlg);
        }

        public void btnPrint()
        {

            if (PrecriptionsForPrint.IssueID <= 0)
            {
                MessageBox.Show(eHCMSResources.A0668_G1_Msg_InfoKhCoToaThuocDeIn, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }

            int numOfCopies = 0;

            if (Globals.ServerConfigSection.CommonItems.NumberOfCopiesPrescription > 0)
            {
                numOfCopies = Globals.ServerConfigSection.CommonItems.NumberOfCopiesPrescription;
            }
            else
            {
                if (Int32.TryParse(NumberOfTimesPrint, out numOfCopies))
                {
                    if (numOfCopies <= 0)
                    {
                        MessageBox.Show(eHCMSResources.A0969_G1_Msg_InfoSoLanInKhHopLe, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                        return;
                    }
                    else
                    {
                        if (numOfCopies > 5 && MessageBox.Show(eHCMSResources.A0970_G1_Msg_ConfIn, eHCMSResources.K1576_G1_CBao, MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                        {
                            return;
                        }
                    }
                }
                else
                {
                    MessageBox.Show(eHCMSResources.A0969_G1_Msg_InfoSoLanInKhHopLe, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                    return;
                }
            }
            var parTypeOfForm = Globals.ServerConfigSection.ConsultationElements.IsSeparatePsychotropicPrescription_Inpt == true ? 1 : 0;
            //▼====: #001
            if (LatestePrecriptions == null || LatestePrecriptions.PrescriptionDetails == null || LatestePrecriptions.PrescriptionDetails.Count == 0)
                parTypeOfForm = 0;
            //▲====: #001

            //▼===== #007
            this.ShowBusyIndicator(eHCMSResources.Z1544_G1_DangIn);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ReportServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginGetXRptEPrescriptionInPdfFormat_InPt(PrecriptionsForPrint.IssueID, parTypeOfForm, numOfCopies
                            , Globals.ServerConfigSection.Hospitals.HospitalCode
                            , Globals.ServerConfigSection.ConsultationElements.PrescriptionInPtVersion
                            , Globals.ServerConfigSection.Hospitals.PrescriptionMainRightHeader
                            , Globals.ServerConfigSection.Hospitals.PrescriptionSubRightHeader
                            , Globals.ServerConfigSection.CommonItems.ReportDepartmentOfHealth
                            , Globals.ServerConfigSection.CommonItems.ReportHospitalName
                            , Globals.ServerConfigSection.CommonItems.ReportHospitalAddress
                            //, Globals.ServerConfigSection.CommonItems.KBYTLink
                            , Globals.ServerConfigSection.CommonItems.LinkKhaoSatNoiTru
                            , Globals.ServerConfigSection.CommonItems.ReportHospitalPhone
                            , Globals.DispatchCallback((asyncResult) =>
                            {
                                try
                                {
                                    var results = contract.EndGetXRptEPrescriptionInPdfFormat_InPt(asyncResult);
                                    //var printEvt = new ActiveXPrintEvt(this, PrinterType.IN_PHIEU, results, ActiveXPrintType.ByteArray, numOfCopies);
                                    var printEvt = new ActiveXPrintEvt(this, PrinterType.IN_PHIEU, results, ActiveXPrintType.ByteArray, 1);
                                    Globals.EventAggregator.Publish(printEvt);

                                }
                                catch (Exception ex)
                                {
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
                    ClientLoggerHelper.LogInfo(ex.ToString());
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }
        //▲===== #007

        #endregion

        public void btnAppointment()
        {
            //KMx: Không được dựa vào ObjPatientServiceRecord_Current.PrescriptionIssueHistories[0]. Vì nếu user chọn toa cũ thì khi hẹn bệnh sẽ bị lỗi hẹn cho toa của DK hiện tại (28/03/2016 15:34).
            //if (ObjPatientServiceRecord_Current == null || ObjPatientServiceRecord_Current.PrescriptionIssueHistories == null || ObjPatientServiceRecord_Current.PrescriptionIssueHistories.Count < 1)
            //{
            //    MessageBox.Show(string.Format("{0}!", eHCMSResources.A0485_G1_Msg_InfoDKChuaCoToa_KhTheHen), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
            //    return;
            //}

            //if (ObjPatientServiceRecord_Current.PrescriptionIssueHistories[0].AppointmentID.GetValueOrDefault() <= 0)
            //{
            //    MessageBox.Show("Bạn chưa chọn hẹn tái khám nên không thể hẹn bệnh!", eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
            //    return;
            //}
            if (Registration_DataStorage.CurrentPatientRegistration != null && Globals.IsLockRegistration(Registration_DataStorage.CurrentPatientRegistration.RegLockFlag, "hẹn tái khám"))
            {
                return;
            }
            if (LatestePrecriptions == null || LatestePrecriptions.IssueID <= 0)
            {
                MessageBox.Show(eHCMSResources.A0485_G1_Msg_InfoDKChuaCoToa_KhTheHen, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }

            this.ShowBusyIndicator();

            long? appointmentID = 0;

            var t = new Thread(() =>
            {
                using (var serviceFactory = new ePrescriptionsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetAppointmentID(LatestePrecriptions.IssueID, true, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            contract.EndGetAppointmentID(out appointmentID, asyncResult);

                            //KMx: Khi đổi ngày cuộc hẹn thì ID cuộc hẹn cũ bị xóa, ID cuộc hẹn mới được update vào toa thuốc nhưng toa thuốc không load lại. Dẫn tới dữ liệu không đồng bộ nên mỗi lần hẹn bệnh phải load lại AppointmentID (15/05/2016 17:52). 
                            LatestePrecriptions.AppointmentID = appointmentID;

                            if (LatestePrecriptions.AppointmentID.GetValueOrDefault() <= 0)
                            {
                                MessageBox.Show(eHCMSResources.K0214_G1_ToaThuocKhongHenTK, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                return;
                            }

                            PatientAppointment curAppt = new PatientAppointment();
                            curAppt.Patient = Registration_DataStorage.CurrentPatient;
                            //curAppt.AppointmentID = ObjPatientServiceRecord_Current.PrescriptionIssueHistories[0].AppointmentID.GetValueOrDefault();
                            curAppt.AppointmentID = LatestePrecriptions.AppointmentID.GetValueOrDefault();

                            Action<IAddEditAppointment> onInitDlg = delegate (IAddEditAppointment apptVm)
                            {
                                //20190929 TTM: Không được deepCopy interface.
                                //apptVm.Registration_DataStorage = Registration_DataStorage.DeepCopy();
                                apptVm.Registration_DataStorage = Registration_DataStorage;
                                apptVm.SetCurrentAppointment(curAppt);
                            };
                            GlobalsNAV.ShowDialog<IAddEditAppointment>(onInitDlg);


                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
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

        private bool _btChonChanDoanIsEnabled;
        public bool btChonChanDoanIsEnabled
        {
            get
            {
                return _btChonChanDoanIsEnabled;
            }
            set
            {
                if (_btChonChanDoanIsEnabled != value)
                {
                    _btChonChanDoanIsEnabled = value;
                    NotifyOfPropertyChange(() => btChonChanDoanIsEnabled);
                }
            }
        }


        //public void btChonChanDoan(object sender, RoutedEventArgs e)
        //{
        //    Globals.ConsultationIsChildWindow = true;
        //    var proAlloc = Globals.GetViewModel<IConsultations>();
        //    proAlloc.IsPopUp = true;
        //    var instance = proAlloc as Conductor<object>;
        //    Globals.ShowDialog(instance, (o) => { });
        //}


        #region Schedule
        private int _IndexRow;
        public int IndexRow
        {
            get { return _IndexRow; }
            set
            {
                if (_IndexRow != value)
                {
                    _IndexRow = value;
                    NotifyOfPropertyChange(() => IndexRow);
                }
            }
        }

        public bool SelectedPrescriptionDetailIsValid()
        {
            if (grdPrescription.SelectedIndex < 0 || grdPrescription.SelectedIndex > (LatestePrecriptions.PrescriptionDetails.Count - 1))
            {
                return false;
            }
            PrescriptionDetail selPrescriptionDetail = LatestePrecriptions.PrescriptionDetails[grdPrescription.SelectedIndex];
            if (selPrescriptionDetail == null)
            {
                return false;
            }
            if ((selPrescriptionDetail.DrugID > 0) || (selPrescriptionDetail.IsDrugNotInCat == true && selPrescriptionDetail.BrandName.Length > 1))
            {
                return true;
            }
            return false;
        }

        public void hplEditSchedules_Click(Object pSelectedItem)
        {
            if (pSelectedItem == null)
            {
                MessageBox.Show(eHCMSResources.A0365_G1_Msg_InfoChonThuocDeRaToa, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }

            if (!SelectedPrescriptionDetailIsValid())
            {
                MessageBox.Show(eHCMSResources.A0534_G1_Msg_InfoDongThuocKhHopLe, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }

            PrescriptionDetail selPrescriptionDetail = LatestePrecriptions.PrescriptionDetails[grdPrescription.SelectedIndex];

            if (selPrescriptionDetail.DrugType.LookupID != (long)AllLookupValues.V_DrugType.THUOC_UONGLICHTUAN)
            {
                MessageBox.Show(eHCMSResources.A0533_G1_Msg_InfoThuocKhUongTheoLichTuan, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }

            hplEditSchedules(selPrescriptionDetail);

        }

        public void hplEditSchedules(PrescriptionDetail pSelectedItem)
        {
            if (!CheckValidationEditor1Row(pSelectedItem))
            {
                return;
            }

            IndexRow = grdPrescription.SelectedIndex;


            Action<IPrescriptionDetailSchedulesNew> onInitDlg = delegate (IPrescriptionDetailSchedulesNew typeInfo)
            {
                //==== 20161115 CMN Begin: Fix Drug Week Calendar
                typeInfo.ParentVM = this;
                //==== 20161115 CMN End.
                typeInfo.ObjPrescriptionDetail = pSelectedItem;

                typeInfo.ModeForm = LatestePrecriptions.IssueID > 0 ? 1 : 0;/*Update*//*AddNew*/

                typeInfo.IsMaxDay = typeInfo.ObjPrescriptionDetail.SelectedDrugForPrescription.DayRpts == 0
                    || typeInfo.ObjPrescriptionDetail.SelectedDrugForPrescription.MaxDayPrescribed > 0 ? true : false;


                int nScheduleDays = Convert.ToInt32(typeInfo.ObjPrescriptionDetail.DayRpts + typeInfo.ObjPrescriptionDetail.DayExtended);

                if (nScheduleDays <= 0 && LatestePrecriptions.NDay.HasValue)
                {
                    nScheduleDays = LatestePrecriptions.NDay.Value;
                }

                typeInfo.NDay = nScheduleDays;

                typeInfo.ObjPrescriptionDetailSchedules_ByPrescriptDetailID = pSelectedItem.ObjPrescriptionDetailSchedules.DeepCopy();

                typeInfo.Initialize();
            };
            GlobalsNAV.ShowDialog<IPrescriptionDetailSchedulesNew>(onInitDlg);

        }
        public void Handle(SendPrescriptionDetailSchedulesEvent<ObservableCollection<PrescriptionDetailSchedules>, bool, double, double, string, int> message)
        {
            if (message != null)
            {

                PrescriptionDetail selPrescriptionDetail = LatestePrecriptions.PrescriptionDetails[grdPrescription.SelectedIndex];

                if (selPrescriptionDetail == null || selPrescriptionDetail.SelectedDrugForPrescription == null)
                {
                    return;
                }

                //Chọn Lịch thì gán bên ngoài = 0
                selPrescriptionDetail.MDose = 0;
                selPrescriptionDetail.ADose = 0;
                selPrescriptionDetail.NDose = 0;
                selPrescriptionDetail.EDose = 0;
                selPrescriptionDetail.dosage = 0;

                selPrescriptionDetail.MDoseStr = "0";
                selPrescriptionDetail.ADoseStr = "0";
                selPrescriptionDetail.NDoseStr = "0";
                selPrescriptionDetail.EDoseStr = "0";
                selPrescriptionDetail.dosageStr = "0";
                if (RegistrationCoverByHI())
                {
                    if (IsBenhNhanNoiTru() == false)
                    {
                        if (message.SoNgayDung > xNgayBHToiDa_NgoaiTru)
                        {
                            selPrescriptionDetail.DayRpts = xNgayBHToiDa_NgoaiTru;
                            selPrescriptionDetail.DayExtended = message.SoNgayDung - xNgayBHToiDa_NgoaiTru;
                        }
                        else
                        {
                            selPrescriptionDetail.DayRpts = message.SoNgayDung;
                            selPrescriptionDetail.DayExtended = 0;
                        }
                    }
                    else
                    {
                        if (message.SoNgayDung > xNgayBHToiDa_NoiTru)
                        {
                            selPrescriptionDetail.DayRpts = xNgayBHToiDa_NoiTru;
                            selPrescriptionDetail.DayExtended = message.SoNgayDung - xNgayBHToiDa_NoiTru;
                        }
                        else
                        {
                            selPrescriptionDetail.DayRpts = message.SoNgayDung;
                            selPrescriptionDetail.DayExtended = 0;
                        }
                    }
                }
                else
                {
                    selPrescriptionDetail.DayRpts = message.SoNgayDung;
                    selPrescriptionDetail.DayExtended = 0;
                }

                //selPrescriptionDetail.Qty = message.TongThuoc;
                selPrescriptionDetail.DrugInstructionNotes = message.GhiChu;
                //Chọn Lịch thì gán bên ngoài = 0

                // // Txd Commented out
                //SelectedPrescriptionDetail.ObjPrescriptionDetailSchedules = message.Data;
                //SelectedPrescriptionDetail.HasSchedules = message.HasSchedule;

                selPrescriptionDetail.ObjPrescriptionDetailSchedules = message.Data;
                selPrescriptionDetail.V_DrugType = (long)AllLookupValues.V_DrugType.THUOC_UONGLICHTUAN;

                CalQtyAndQtyMaxForSchedule(selPrescriptionDetail);

            }
        }
        //==== 20161115 CMN Begin Add: Fix Drug Week Calendar
        public void HandleDrugSchedule(ObservableCollection<PrescriptionDetailSchedules> drugSchedule, double numOfDay, string note)
        {
            PrescriptionDetail selPrescriptionDetail = LatestePrecriptions.PrescriptionDetails[grdPrescription.SelectedIndex];
            if (selPrescriptionDetail == null || selPrescriptionDetail.SelectedDrugForPrescription == null)
            {
                return;
            }
            //Chọn Lịch thì gán bên ngoài = 0
            selPrescriptionDetail.MDose = 0;
            selPrescriptionDetail.ADose = 0;
            selPrescriptionDetail.NDose = 0;
            selPrescriptionDetail.EDose = 0;
            selPrescriptionDetail.dosage = 0;

            selPrescriptionDetail.MDoseStr = "0";
            selPrescriptionDetail.ADoseStr = "0";
            selPrescriptionDetail.NDoseStr = "0";
            selPrescriptionDetail.EDoseStr = "0";
            selPrescriptionDetail.dosageStr = "0";
            if (RegistrationCoverByHI())
            {
                if (IsBenhNhanNoiTru() == false)
                {
                    if (numOfDay > xNgayBHToiDa_NgoaiTru)
                    {
                        selPrescriptionDetail.DayRpts = xNgayBHToiDa_NgoaiTru;
                        selPrescriptionDetail.DayExtended = numOfDay - xNgayBHToiDa_NgoaiTru;
                    }
                    else
                    {
                        selPrescriptionDetail.DayRpts = numOfDay;
                        selPrescriptionDetail.DayExtended = 0;
                    }
                }
                else
                {
                    if (numOfDay > xNgayBHToiDa_NoiTru)
                    {
                        selPrescriptionDetail.DayRpts = xNgayBHToiDa_NoiTru;
                        selPrescriptionDetail.DayExtended = numOfDay - xNgayBHToiDa_NoiTru;
                    }
                    else
                    {
                        selPrescriptionDetail.DayRpts = numOfDay;
                        selPrescriptionDetail.DayExtended = 0;
                    }
                }
            }
            else
            {
                selPrescriptionDetail.DayRpts = numOfDay;
                selPrescriptionDetail.DayExtended = 0;
            }
            selPrescriptionDetail.DrugInstructionNotes = note;
            //Chọn Lịch thì gán bên ngoài = 0
            selPrescriptionDetail.ObjPrescriptionDetailSchedules = drugSchedule;
            selPrescriptionDetail.V_DrugType = (long)AllLookupValues.V_DrugType.THUOC_UONGLICHTUAN;
            CalQtyAndQtyMaxForSchedule(selPrescriptionDetail);
        }
        //==== 20161115 CMN End Add.

        private enum PrescriptGridEditCellIdx { EditCellIdxBegin = 4, EditCellIdxEnd = 15 };
        EmrPrescriptionGrid grdPrescription;
        public void grdPrescription_Loaded(object sender, RoutedEventArgs e)
        {
            grdPrescription = sender as EmrPrescriptionGrid;
            grdPrescription.nFirstEditIdx = (int)PrescriptGridEditCellIdx.EditCellIdxBegin;
            grdPrescription.nLastEditIdx = (int)PrescriptGridEditCellIdx.EditCellIdxEnd;
        }

        #endregion


        #region CheckThuocBiTrungTrongToa
        private bool CheckThuocBiTrungTrongToa()
        {
            foreach (var prescriptionDetail in LatestePrecriptions.PrescriptionDetails)
            {
                if (CountDrug(prescriptionDetail.DrugID) >= 2)
                {
                    MessageBox.Show(string.Format("{0} {1}", eHCMSResources.K0072_G1_TrungThuocTrongToa, eHCMSResources.I0945_G1_I), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                    return true;
                }
            }
            return false;
        }
        private int CountDrug(Nullable<Int64> DrugID)
        {
            int d = 0;
            if (DrugID != null && DrugID > 0)
            {
                foreach (var prescriptionDetail in LatestePrecriptions.PrescriptionDetails)
                {
                    if (prescriptionDetail.DrugID == DrugID)
                    {
                        d++;
                    }
                }
            }
            return d;
        }
        #endregion


        public void GetAllContrainIndicatorDrugs()
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });

            IsWaitingGetAllContrainIndicatorDrugs = true;

            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyDrugServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginGetAllContrainIndicatorDrugs(Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndGetAllContrainIndicatorDrugs(asyncResult);
                            if (results != null)
                            {
                                if (Globals.allContraIndicatorDrugsRelToMedCond == null)
                                {
                                    Globals.allContraIndicatorDrugsRelToMedCond = new ObservableCollection<ContraIndicatorDrugsRelToMedCond>();
                                }
                                else
                                {
                                    Globals.allContraIndicatorDrugsRelToMedCond.Clear();
                                }
                                foreach (var p in results)
                                {
                                    Globals.allContraIndicatorDrugsRelToMedCond.Add(p);
                                }
                                NotifyOfPropertyChange(() => Globals.allContraIndicatorDrugsRelToMedCond);
                            }
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            IsWaitingGetAllContrainIndicatorDrugs = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }

        private bool CheckSoLuongThuocDeBan(out string msg)
        {
            StringBuilder sb = new StringBuilder();

            msg = "";

            bool Result = true;

            if (LatestePrecriptions.PrescriptionDetails.Count > 0)
            {
                foreach (PrescriptionDetail item in LatestePrecriptions.PrescriptionDetails)
                {
                    if (item.IsDrugNotInCat || item.SelectedDrugForPrescription == null)
                    {
                        continue;
                    }

                    if (item.SelectedDrugForPrescription.Remaining < item.Qty)
                    {
                        //▼====: #009
                        sb.AppendLine(string.Format(eHCMSResources.Z1073_G1_ThuocSLgTrongKhoKgDuBan, item.SelectedDrugForPrescription.BrandName.Trim())
                            + " Số thuốc còn lại trong kho: " + item.SelectedDrugForPrescription.Remaining + " " + item.SelectedDrugForPrescription.UnitName);
                        //▲====: #009
                        Result = false;
                    }
                }
            }

            msg = sb.ToString();

            return Result;
        }


        private bool ConfirmSoLuongNotEnoughBeforeSave()
        {
            string msg = "";
            if (CheckSoLuongThuocDeBan(out msg) == false)
            {
                //▼====: #009
                //if (MessageBox.Show(msg + string.Format("\n{0}", eHCMSResources.A0190_G1_Msg_ConfLuuToaThuoc), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                //{
                //    return true;
                //}
                //else
                //{
                //    return false;
                //}
                MessageBox.Show(msg, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return false;
                //▲====: #009
            }
            else
            {
                return true;
            }
        }

        private bool CheckWarningOfDrug(out string msg)
        {
            StringBuilder sb = new StringBuilder();

            msg = "";

            bool Result = true;

            ObservableCollection<PrescriptionDetail> CheckList = new ObservableCollection<PrescriptionDetail>();

            //Nếu là BN thường thì chỉ lấy những thuốc có cảnh báo cho BN thường, không lấy thuốc có cảnh báo cho BN BHYT.
            //Nếu BN BHYT thì lấy TẤT CẢ. (03/06/2014 14:47)
            if (!RegistrationCoverByHI())
            {
                CheckList = LatestePrecriptions.PrescriptionDetails.Where(x => x.SelectedDrugForPrescription != null && x.SelectedDrugForPrescription.IsWarningHI == false).ToObservableCollection();
            }
            else
            {
                CheckList = LatestePrecriptions.PrescriptionDetails;
            }

            if (CheckList.Count > 0)
            {
                int STT = 0;

                foreach (PrescriptionDetail item in CheckList)
                {
                    if (item.IsDrugNotInCat || item.SelectedDrugForPrescription == null || item.SelectedDrugForPrescription.Precaution_Warn == null)
                    {
                        continue;
                    }

                    string WarningOfDrug = item.SelectedDrugForPrescription.Precaution_Warn.Trim();

                    if (WarningOfDrug.Length > 0)
                    {
                        STT++;
                        sb.AppendLine(STT + ". " + item.SelectedDrugForPrescription.BrandName.Trim() + ": " + WarningOfDrug);
                        Result = false;
                    }
                }
            }

            if (!Result)
            {
                msg = string.Format(eHCMSResources.Z1074_G1_CBaoChonThuocTiepTucLuu, sb.ToString());
            }
            else
            {
                msg = sb.ToString();
            }

            return Result;
        }


        private bool CheckQtyUserInput(out string msg)
        {
            StringBuilder sb = new StringBuilder();

            msg = "";

            bool Result = true;

            ObservableCollection<PrescriptionDetail> CheckList = new ObservableCollection<PrescriptionDetail>();

            CheckList = LatestePrecriptions.PrescriptionDetails.Where(x => x.V_DrugType == (long)AllLookupValues.V_DrugType.THUOC_THONGTHUONG || x.V_DrugType == (long)AllLookupValues.V_DrugType.THUOC_UONGLICHTUAN).ToObservableCollection();

            if (CheckList.Count <= 0)
            {
                return Result;
            }

            int STT = 0;

            foreach (PrescriptionDetail item in CheckList)
            {
                float QtyReal = 0;

                if (item.V_DrugType == (long)AllLookupValues.V_DrugType.THUOC_THONGTHUONG)
                {
                    QtyReal = CalQtyForNormalDrug(item, item.RealDay);
                }

                if (item.V_DrugType == (long)AllLookupValues.V_DrugType.THUOC_UONGLICHTUAN)
                {
                    QtyReal = (float)CalQtyForScheduleDrug(item, item.RealDay);
                }

                if (item.Qty > QtyReal)
                {
                    STT++;
                    sb.AppendLine(string.Format(eHCMSResources.Z1077_G1_I, STT, item.SelectedDrugForPrescription.BrandName.Trim(), item.RealDay, QtyReal, item.Qty));
                    Result = false;
                }

            }

            if (!Result)
            {
                msg = string.Format(eHCMSResources.Z1078_G1_XNhanSLgTrcKhiLuu, sb.ToString());
            }
            else
            {
                msg = sb.ToString();
            }

            return Result;
        }

        #region "Ra Toa Mới"

        private bool _btnUndoIsEnabled;
        public bool btnUndoIsEnabled
        {
            get
            {
                return _btnUndoIsEnabled;
            }
            set
            {
                if (_btnUndoIsEnabled != value)
                {
                    _btnUndoIsEnabled = value;
                    NotifyOfPropertyChange(() => btnUndoIsEnabled);
                }
            }
        }

        private bool _btnCreateNewIsEnabled;
        public bool btnCreateNewIsEnabled
        {
            get
            {
                return _btnCreateNewIsEnabled;
            }
            set
            {
                if (_btnCreateNewIsEnabled != value)
                {
                    _btnCreateNewIsEnabled = value;
                    NotifyOfPropertyChange(() => btnCreateNewIsEnabled);
                }
            }
        }

        private bool _btnSaveAddNewIsEnabled = true;
        public bool btnSaveAddNewIsEnabled
        {
            get
            {
                return _btnSaveAddNewIsEnabled;
            }
            set
            {
                if (_btnSaveAddNewIsEnabled != value)
                {
                    _btnSaveAddNewIsEnabled = value;
                    NotifyOfPropertyChange(() => btnSaveAddNewIsEnabled);
                }
            }
        }

        private void SetToaBaoHiem_KhongBaoHiem()
        {
            if (Registration_DataStorage.CurrentPatientRegistrationDetail != null && Registration_DataStorage.CurrentPatient != null)
            {
                if (Registration_DataStorage.CurrentPatient.CurrentHealthInsurance != null)
                {
                    NewPrecriptions.HICardNo = Registration_DataStorage.CurrentPatient.CurrentHealthInsurance.HICardNo;
                }
                //else
                //{
                //    NewPrecriptions.HICardNo = "";
                //}
            }
        }

        public void InitDataGridNew()
        {
            if (NewPrecriptions == null)
            {
                NewPrecriptions = new Prescription();
            }
            if (NewPrecriptions.PrescriptionDetails == null || NewPrecriptions.PrescriptionDetails.Count == 0)
            {
                AddNewBlankDrugIntoPrescriptObjectNew();
            }
        }

        private bool _HasDiagnosis = false;
        public bool HasDiagnosis
        {
            get
            {
                return _HasDiagnosis;
            }
            set
            {
                if (_HasDiagnosis != value)
                {
                    _HasDiagnosis = value;
                    NotifyOfPropertyChange(() => HasDiagnosis);
                }
            }
        }

        private void DefaultValueForNewPrecription()
        {
            NewPrecriptions.CreatorStaffID = Globals.LoggedUserAccount.Staff.StaffID;
            NewPrecriptions.ObjCreatorStaffID = new Staff();
            NewPrecriptions.ObjCreatorStaffID.StaffID = Globals.LoggedUserAccount.Staff.StaffID;
            NewPrecriptions.ObjCreatorStaffID.FullName = Globals.LoggedUserAccount.Staff.FullName;

            if (HasDiagnosis)
            {
                if (ObjDiagnosisTreatment_Current != null)
                {
                    NewPrecriptions.ExamDate = ObjDiagnosisTreatment_Current.PatientServiceRecord.ExamDate;
                }
            }


            NewPrecriptions.V_PrescriptionNotes = (long)AllLookupValues.V_PrescriptionNotes.TOAGOC;

            NewPrecriptions.PrescriptionIssueHistory = new PrescriptionIssueHistory();
            NewPrecriptions.PrescriptionIssueHistory.DeptLocID = Globals.DeptLocation.DeptLocationID;
            if (RegistrationCoverByHI())//neu dang ky co bao hiem
            {
                NewPrecriptions.PrescriptionIssueHistory.HisID = Registration_DataStorage.CurrentPatientRegistration.HisID.Value;
            }

            //Cụ thể  DV nào
            if (Registration_DataStorage.CurrentPatientRegistration != null)
            {
                if (Registration_DataStorage.CurrentPatientRegistration.V_RegistrationType == AllLookupValues.RegistrationType.NGOAI_TRU)
                {
                    if (Registration_DataStorage.CurrentPatientRegistrationDetail != null && Registration_DataStorage.CurrentPatientRegistrationDetail.PtRegDetailID > 0)
                    {
                        NewPrecriptions.PrescriptionIssueHistory.PtRegDetailID = Registration_DataStorage.CurrentPatientRegistrationDetail.PtRegDetailID;
                    }
                    else
                    {
                        MessageBox.Show(eHCMSResources.A0705_G1_Msg_KhTheRaToaNgTru);
                        return;
                    }
                }
                else/*Khám VIP, Khám Cho Nội Trú*/
                {
                    NewPrecriptions.PrescriptionIssueHistory.PtRegDetailID = 0;
                }
            }
            else
            {
                MessageBox.Show(eHCMSResources.A0704_G1_Msg_InfoKhTheRaToa);
                return;
            }
            //Cụ thể  DV nào

        }

        private Prescription _NewPrecriptions;
        public Prescription NewPrecriptions
        {
            get
            {
                return _NewPrecriptions;
            }
            set
            {
                if (_NewPrecriptions != value)
                {
                    _NewPrecriptions = value;
                    NotifyOfPropertyChange(() => NewPrecriptions);
                }
            }
        }

        private void InitialNewPrescription()
        {
            try
            {
                NewPrecriptions = new Prescription();

                if (NewPrecriptions.PrescriptionDetails == null)
                {
                    NewPrecriptions.PrescriptionDetails = new ObservableCollection<PrescriptionDetail>();
                }

                NewPrecriptions.PrescriptionDetails.Clear();

                NewPrecriptions.ServiceRecID = ObjDiagnosisTreatment_Current.ServiceRecID;
                NewPrecriptions.PtRegistrationID = Registration_DataStorage.CurrentPatientRegistration.PtRegistrationID;
                NewPrecriptions.Diagnosis = string.IsNullOrEmpty(ObjDiagnosisTreatment_Current.DiagnosisFinal) ? ObjDiagnosisTreatment_Current.Diagnosis.Trim() : ObjDiagnosisTreatment_Current.DiagnosisFinal.Trim();

                //KMx: Không chỉ gán Diagnosis như trên, mà lấy hết object, vì cần lấy thêm Cách điều trị (22/04/2015 16:20).

                NewPrecriptions.ObjDiagnosisTreatment = ObjDiagnosisTreatment_Current;
                //20200418 TBL: Lấy chẩn đoán cuối cùng làm chẩn đoán xác nhận
                CurrentDiagnosisTreatment = ObjDiagnosisTreatment_Current;
                NewPrecriptions.NDay = 0;

                NewPrecriptions.PatientID = Registration_DataStorage.CurrentPatient.PatientID;

                NewPrecriptions.ConsultantDoctor = new Staff();

                NewPrecriptions.V_PrescriptionType = CurrentPrescriptionType.LookupID;

                DefaultValueForNewPrecription();

                InitDataGridNew();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private bool _IsWaitingGetDiagnosisTreatmentByPtID;
        public bool IsWaitingGetDiagnosisTreatmentByPtID
        {
            get { return _IsWaitingGetDiagnosisTreatmentByPtID; }
            set
            {
                if (_IsWaitingGetDiagnosisTreatmentByPtID != value)
                {
                    _IsWaitingGetDiagnosisTreatmentByPtID = value;
                    NotifyOfPropertyChange(() => IsWaitingGetDiagnosisTreatmentByPtID);
                    NotifyWhenBusy();
                }
            }
        }


        public bool CheckPrescriptionBeforeSave()
        {
            if (string.IsNullOrEmpty(LatestePrecriptions.Diagnosis))
            {
                MessageBox.Show(eHCMSResources.A0289_G1_Msg_InfoCDinhCDDeRaToa, eHCMSResources.A0290_G1_Msg_LuuToaThuoc, MessageBoxButton.OK);
                return false;
            }

            if (LatestePrecriptions.Department == null || LatestePrecriptions.Department.DeptID <= 0)
            {
                MessageBox.Show(eHCMSResources.Z0493_G1_HayChonKhoa, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return false;
            }

            if (ObjDiagnosisTreatment_Current == null || ObjDiagnosisTreatment_Current.Department == null)
            {
                MessageBox.Show(string.Format("{0}. {1}", eHCMSResources.A0630_G1_Msg_InfoKTraCDXV, eHCMSResources.A1028_G1_Msg_YCKtraTTin), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return false;
            }


            //if (LatestePrecriptions.Department.DeptID != ObjDiagnosisTreatment_Current.Department.DeptID)
            //{
            //    MessageBox.Show(string.Format("{0} {1}", eHCMSResources.A0622_G1_Msg_InfoKhoaTao_ToaThuoc, ObjDiagnosisTreatment_Current.Department.DeptName), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
            //    return false;
            //}
            if (LatestePrecriptions.Department.DeptID != CurrentDiagnosisTreatment.Department.DeptID)
            {
                MessageBox.Show(string.Format("{0} {1}", eHCMSResources.A0622_G1_Msg_InfoKhoaTao_ToaThuoc, ObjDiagnosisTreatment_Current.Department.DeptName), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return false;
            }

            if (LatestePrecriptions.V_PrescriptionType == null)
            {
                MessageBox.Show(eHCMSResources.A0333_G1_Msg_InfoChonLoaiToa, eHCMSResources.A0290_G1_Msg_LuuToaThuoc, MessageBoxButton.OK);
                return false;
            }

            if (grdPrescription == null)
            {
                MessageBox.Show(eHCMSResources.A0365_G1_Msg_InfoChonThuocDeRaToa, eHCMSResources.A0290_G1_Msg_LuuToaThuoc, MessageBoxButton.OK);
                return false;
            }
            //▼====: #009
            if (Globals.ServerConfigSection.CommonItems.MaxNumDayPrescriptAllow_InPt < LatestePrecriptions.NDay.GetValueOrDefault(0))
            {
                MessageBox.Show("Số ngày cho toa không được vượt quá " + Globals.ServerConfigSection.CommonItems.MaxNumDayPrescriptAllow_InPt + " ngày");
                return false;
            }
            //▲====: #009
            //▼====: #010
            if (string.IsNullOrEmpty(LatestePrecriptions.DoctorAdvice))
            {
                MessageBox.Show("Không thể lưu toa. Bắt buộc phải nhập lời dặn hoặc chọn lời dặn từ mẫu.", eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return false;
            }
            //▲====: #010
            //▼====: #014
            if (LatestePrecriptions.DoctorAdvice.Contains("<") || LatestePrecriptions.DoctorAdvice.Contains(">"))
            {
                MessageBox.Show("Nội dung lời dặn không được sử dụng ký tự '<', '>'. Vui lòng kiểm tra lại!", eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return false;
            }
            //▲====: #014
            if (LatestePrecriptions.PrescriptionDetails.Any(x => x.DrugID.GetValueOrDefault(0) > 0 && string.IsNullOrEmpty(x.UsageDistance)))
            {
                Globals.ShowMessage("Khoảng cách dùng không được để trống", eHCMSResources.G0442_G1_TBao);
                return false;
            }
            if (CheckAllThuocBiDiUng())
            {
                return false;
            }

            if (ErrCheckChongChiDinh())
            {
                return false;
            }

            if (grdPrescription.IsValid == false)
            {
                MessageBox.Show(string.Format("{0} {1}", eHCMSResources.A0540_G1_Msg_InfoDataKhHopLe, eHCMSResources.A0541_G1_Msg_InfoDataToaThuocKhHopLe), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return false;
            }

            if (CheckThuocHopLe() == false)
            {
                return false;
            }

            if (CheckHoiChan() == false)
            {
                return false;
            }

            if (CheckThuocBiTrungTrongToa())
            {
                return false;
            }
            if (UseOnlyDailyDiagnosis && (CurrentDiagnosisTreatment == null || CurrentDiagnosisTreatment.DTItemID == 0 || CurrentDiagnosisTreatment.IsAdmission))
            {
                MessageBox.Show(eHCMSResources.Z2895_G1_Msg, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return false;
            }
            if (UseOnlyDailyDiagnosis)
            {
                if (LatestePrecriptions != null && LatestePrecriptions.Department != null 
                    && LatestePrecriptions.Department.DeptID != CurrentDiagnosisTreatment.Department.DeptID)
                {
                    MessageBox.Show(eHCMSResources.Z2896_G1_Msg, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                    return false;
                }
                string[] ListStr = Globals.ServerConfigSection.ConsultationElements.ConsultMinTimeReqBeforeExit.Split(new char[] { ';' });
                int MinTimeReq = Convert.ToInt16(ListStr[1]);
                if (MinTimeReq > 0 && (Globals.GetCurServerDateTime() - CurrentDiagnosisTreatment.DiagnosisDate).TotalMinutes > MinTimeReq * 60)
                {
                    MessageBox.Show(string.Format(eHCMSResources.Z2896_G1_ChanDoanQuaTGXacNhan, string.Format("{0} {1}", MinTimeReq, eHCMSResources.T1209_G1_GioL)), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                    return false;
                }
            }
            return true;
        }


        private bool CheckCreatePrescription()
        {
            InPatientAdmDisDetails admission = Registration_DataStorage.CurrentPatientRegistration.AdmissionInfo;
            if (admission.DischargeDate != null)
            {
                MessageBox.Show(eHCMSResources.A0226_G1_Msg_InfoBNDaXV + ". " + eHCMSResources.A0229_G1_Msg_KhongtTheTaoHoacCNhatToaThuoc, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return false;
            }
            if (Registration_DataStorage.CurrentPatientRegistration != null && Globals.IsLockRegistration(Registration_DataStorage.CurrentPatientRegistration.RegLockFlag, "lưu và phát hành toa thuốc"))
            {
                return false;
            }
            if (!HasDiagnosisOutHospital())
            {
                IsEnabledForm = false;
                HasDiagnosis = false;
                ObjDiagnosisTreatment_Current = new DiagnosisTreatment();
                btChonChanDoanIsEnabled = false;
                btnSaveAddNewIsEnabled = false;
                IsEnabledPrint = false;
                MessageBox.Show(eHCMSResources.A0404_G1_Msg_InfoChuaCoCDXVChoDK, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return false;
            }
            return true;
        }


        private bool CheckEditPrescription()
        {
            if (!CheckCreatePrescription())
            {
                return false;
            }
            else
            {
                if (LatestePrecriptions == null || Registration_DataStorage.PatientServiceRecordCollection == null || Registration_DataStorage.PatientServiceRecordCollection.Count <= 0)
                {
                    MessageBox.Show(eHCMSResources.A0749_G1_Msg_InfoKhTimThayToaThuocHaySerRec, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                    return false;
                }
                if (LatestePrecriptions.IssueID > 0 && LatestePrecriptions.ServiceRecID != Registration_DataStorage.PatientServiceRecordCollection[0].ServiceRecID)
                {
                    MessageBox.Show(eHCMSResources.A0672_G1_Msg_InfoKhDcCNhatToaThuocCuaDKCu, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                    return false;
                }
            }
            return true;
        }


        public void btnSaveAddNew()
        {
            if (!CheckCreatePrescription())
            {
                return;
            }

            if (IsPhatHanhLai)
            {
                btnUpdate();
                return;
            }

            //this.ShowBusyIndicator();

            //try
            //{
            //    if (!CheckPrescriptionBeforeSave())
            //    {
            //        this.HideBusyIndicator();
            //        return;
            //    }
            //}
            //catch (Exception ex)
            //{
            //    string msg = "btnSaveAddNew CheckPrescriptionBeforeSave Exception: " + ex.Message;
            //    MessageBox.Show(msg);
            //}

            if (!CheckPrescriptionBeforeSave())
            {
                return;
            }

            //Bỏ dòng cuối cùng rỗng của Toa
            RemoveLastRowPrecriptionDetail();

            UpdateDefaultValueForNewPrecription();

            if (ConfirmSoLuongNotEnoughBeforeSave())
            {
                //SetValueTaoThanhToaMoi_CreateNew();
                SetValueTaoThanhToaMoi_New();
                //Dinh sua o day
                Coroutine.BeginExecute(CoroutinePrescriptions_Add());
            }
            else
            {
                //Cộng lại dòng trống
                AddNewBlankDrugIntoPrescriptObjectNew();
                //Cộng lại dòng trống
            }

            //▼==== #008
            Globals.EventAggregator.Publish(new OnChangedUpdatePrescription() { AdmissionInfoComment = LatestePrecriptions.DoctorAdvice });
            //▲==== #008
            //this.HideBusyIndicator();

        }

        public void btnUpdate()
        {
            if (!CheckEditPrescription())
            {
                return;
            }

            //this.ShowBusyIndicator();

            //try
            //{
            //    if (!CheckPrescriptionBeforeSave())
            //    {
            //        this.HideBusyIndicator();
            //        return;
            //    }
            //}
            //catch (Exception ex)
            //{
            //    string msg = "btnUpdate CheckPrescriptionBeforeSave Exception: " + ex.Message;
            //    MessageBox.Show(msg);
            //}

            if (!CheckPrescriptionBeforeSave())
            {
                return;
            }

            //Bỏ dòng cuối cùng rỗng của Toa
            RemoveLastRowPrecriptionDetail();

            CapNhatToaThuoc = true;

            if (ConfirmSoLuongNotEnoughBeforeSave())
            {
                //SetValueTaoThanhToaMoi();
                SetValueTaoThanhToaMoi_New();
                //Prescriptions_Update();
                //Dinh sua o day
                Coroutine.BeginExecute(CoroutinePrescriptions_Update());
            }
            else
            {
                //Cộng lại dòng trống
                AddNewBlankDrugIntoPrescriptObjectNew();
                //Cộng lại dòng trống
            }

            //▼==== #008
            Globals.EventAggregator.Publish(new OnChangedUpdatePrescription() { AdmissionInfoComment = LatestePrecriptions.DoctorAdvice });
            //▲==== #008
            //this.HideBusyIndicator();

        }

        public IEnumerator<IResult> CoroutinePrescriptions_Add()
        {

            string msg = "";
            if (!CheckWarningOfDrug(out msg))
            {
                warnOfDrug = new WarningWithConfirmMsgBoxTask(msg, eHCMSResources.Z0627_G1_TiepTucLuu);
                yield return warnOfDrug;
                if (!warnOfDrug.IsAccept)
                {
                    //Cộng lại dòng trống
                    AddNewBlankDrugIntoPrescriptObjectNew();
                    //Cộng lại dòng trống
                    warnOfDrug = null;
                    yield break;
                }
            }
            warnOfDrug = null;


            string WarningMsg = "";
            if (!CheckQtyUserInput(out WarningMsg))
            {
                confirmQtyBeforeSave = new WarningWithConfirmMsgBoxTask(WarningMsg, eHCMSResources.Z0627_G1_TiepTucLuu);
                yield return confirmQtyBeforeSave;
                if (!confirmQtyBeforeSave.IsAccept)
                {
                    //Cộng lại dòng trống
                    AddNewBlankDrugIntoPrescriptObjectNew();
                    //Cộng lại dòng trống
                    confirmQtyBeforeSave = null;
                    yield break;
                }
            }
            confirmQtyBeforeSave = null;

            //KMx: Thêm cấu hình ShowApptCheck (trong bảng RefApplicationConfigs), vì 1 số phòng khám tư không dùng chức năng hẹn bệnh (02/05/2014 11:40).
            if (!LatestePrecriptions.HasAppointment && Globals.ServerConfigSection.CommonItems.ShowApptCheck)
            {
                var dialog = new PresApptShowDialogTask();
                yield return dialog;
                ObjTaoThanhToaMoi.HasAppointment = dialog.HasAppointment;
            }
            Prescriptions_Add();
            yield break;
        }

        WarningWithConfirmMsgBoxTask warnOfDrug = null;
        WarningWithConfirmMsgBoxTask confirmQtyBeforeSave = null;

        public IEnumerator<IResult> CoroutinePrescriptions_Update()
        {
            string msg = "";
            if (!CheckWarningOfDrug(out msg))
            {
                warnOfDrug = new WarningWithConfirmMsgBoxTask(msg, eHCMSResources.Z0627_G1_TiepTucLuu);
                yield return warnOfDrug;
                if (!warnOfDrug.IsAccept)
                {
                    //Cộng lại dòng trống
                    AddNewBlankDrugIntoPrescriptObjectNew();
                    //Cộng lại dòng trống
                    warnOfDrug = null;
                    yield break;
                }
            }
            warnOfDrug = null;

            string WarningMsg = "";
            if (!CheckQtyUserInput(out WarningMsg))
            {
                confirmQtyBeforeSave = new WarningWithConfirmMsgBoxTask(WarningMsg, eHCMSResources.Z0627_G1_TiepTucLuu);
                yield return confirmQtyBeforeSave;
                if (!confirmQtyBeforeSave.IsAccept)
                {
                    //Cộng lại dòng trống
                    AddNewBlankDrugIntoPrescriptObjectNew();
                    //Cộng lại dòng trống
                    confirmQtyBeforeSave = null;
                    yield break;
                }
            }
            confirmQtyBeforeSave = null;

            //KMx: Thêm cấu hình ShowApptCheck (trong bảng RefApplicationConfigs), vì 1 số phòng khám tư không dùng chức năng hẹn bệnh (02/05/2014 11:40).
            if (!LatestePrecriptions.HasAppointment && Globals.ServerConfigSection.CommonItems.ShowApptCheck)
            {
                var dialog = new PresApptShowDialogTask();
                yield return dialog;
                ObjTaoThanhToaMoi.HasAppointment = dialog.HasAppointment;
            }
            Prescriptions_Update();
            yield break;
        }

        public IEnumerator<IResult> MessageWarningShowDialogTask(string strMessage)
        {
            var dialog = new MessageWarningShowDialogTask(strMessage, eHCMSResources.K1576_G1_CBao, false);
            yield return dialog;
            yield break;
        }


        public void UpdateDefaultValueForNewPrecription()
        {
            NewPrecriptions.TimesNumberIsPrinted = 0;
            NewPrecriptions.V_PrescriptionType = CurrentPrescriptionType.LookupID;
        }

        private void ButtonCreateNew()
        {
            IsPhatHanhLai = false;

            ContentKhungTaiKhamIsEnabled = true;
            IsEnabled = true;

            btnCreateNewIsEnabled = false;

            btnSaveAddNewIsEnabled = true;
            btnUndoIsEnabled = true;

            btnEditIsEnabled = false;
            btnCreateAndCopyIsEnabled = false;

            btnUpdateIsEnabled = false;
            //bntSaveAsIsEnabled = false;
            btDuocSiEditIsEnabled = false;
            btnCopyToIsEnabled = false;

            btChonChanDoanIsEnabled = true;

            IsEnabledPrint = false;
            //
            //App
            chkHasAppointmentValue = false;
            //App
        }


        public void btnCreateNew()
        {
            if (!CheckCreatePrescription())
            {
                return;
            }

            ButtonCreateNew();

            NewPrecriptions = new Prescription();
            NewPrecriptions.ServiceRecID = ObjDiagnosisTreatment_Current.ServiceRecID;
            NewPrecriptions.PtRegistrationID = Registration_DataStorage.CurrentPatientRegistration.PtRegistrationID;
            NewPrecriptions.Diagnosis = string.IsNullOrEmpty(ObjDiagnosisTreatment_Current.DiagnosisFinal) ? ObjDiagnosisTreatment_Current.Diagnosis.Trim() : ObjDiagnosisTreatment_Current.DiagnosisFinal.Trim();
            NewPrecriptions.NDay = 0;

            NewPrecriptions.PatientID = Registration_DataStorage.CurrentPatient.PatientID;

            NewPrecriptions.ConsultantDoctor = new Staff();

            NewPrecriptions.CreatorStaffID = Globals.LoggedUserAccount.Staff.StaffID;
            NewPrecriptions.ObjCreatorStaffID = new Staff();
            NewPrecriptions.ObjCreatorStaffID.StaffID = Globals.LoggedUserAccount.Staff.StaffID;
            NewPrecriptions.ObjCreatorStaffID.FullName = Globals.LoggedUserAccount.Staff.FullName;

            NewPrecriptions.ExamDate = ObjDiagnosisTreatment_Current.PatientServiceRecord.ExamDate;

            NewPrecriptions.V_PrescriptionNotes = (long)AllLookupValues.V_PrescriptionNotes.TOAGOC;
            NewPrecriptions.V_PrescriptionType = CurrentPrescriptionType.LookupID;

            NewPrecriptions.PrescriptionIssueHistory = new PrescriptionIssueHistory();
            NewPrecriptions.PrescriptionIssueHistory.DeptLocID = Globals.DeptLocation.DeptLocationID;
            NewPrecriptions.Department = DepartmentContent.SelectedItem;
            //Cụ thể  DV nào
            if (Registration_DataStorage.CurrentPatientRegistration != null)
            {
                if (Registration_DataStorage.CurrentPatientRegistration.V_RegistrationType == AllLookupValues.RegistrationType.NGOAI_TRU)
                {
                    if (Registration_DataStorage.CurrentPatientRegistrationDetail != null && Registration_DataStorage.CurrentPatientRegistrationDetail.PtRegDetailID > 0)
                    {
                        NewPrecriptions.PrescriptionIssueHistory.PtRegDetailID = Registration_DataStorage.CurrentPatientRegistrationDetail.PtRegDetailID;
                    }
                    else
                    {
                        MessageBox.Show(eHCMSResources.A0705_G1_Msg_KhTheRaToaNgTru);
                        return;
                    }
                }
                else/*Khám VIP, Khám Cho Nội Trú*/
                {
                    NewPrecriptions.PrescriptionIssueHistory.PtRegDetailID = 0;
                }
            }
            else
            {
                MessageBox.Show(eHCMSResources.A0704_G1_Msg_InfoKhTheRaToa);
                return;
            }
            //Cụ thể  DV nào
            BackupCurPrescriptionItem();
            LatestePrecriptions = NewPrecriptions.DeepCopy();

            AddNewBlankDrugIntoPrescriptObjectNew();
            if (CtrtbSoTuan != null)
            {
                CtrtbSoTuan.Focus();
            }
        }

        public void btnCreateNewAndCopy()
        {
            if (!CheckCreatePrescription())
            {
                return;
            }

            IsPhatHanhLai = false;
            long PrescriptID = LatestePrecriptions.PrescriptID;
            //if (LatestePrecriptions.CanEdit)
            {
                //KMx: A.Tuân nói không kiểm tra ngày dùng vượt ngày BH qui định. Vì bác sĩ muốn ra bao nhiêu ngày cũng được (07/04/2014 16:02).
                //if (CheckIsVuotNgayQuiDinhHI())
                //{
                //    if (MessageBox.Show(eHCMSResources.A0838_G1_Msg_ConfTuDChinhNgDung, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                //    {
                //        //Tự động điều chỉnh
                //        AutoAdjust();
                //    }
                //}
                //Enable & Disable Buttons
                UpdateButtonsOnNewPrescriptBasedOnPrev(IsPhatHanhLai);
                GetPrescriptionDetailsByPrescriptID(PrescriptID, true);
                if (CtrtbSoTuan != null)
                {
                    CtrtbSoTuan.Focus();
                }
            }

        }

        #region "Dãy Nút Dược Sĩ Không Cần Phải Thấy"
        private bool _btnCreateNewVisibility = true;
        public bool btnCreateNewVisibility
        {
            get
            {
                return _btnCreateNewVisibility;
            }
            set
            {
                if (_btnCreateNewVisibility != value)
                {
                    _btnCreateNewVisibility = value;
                    NotifyOfPropertyChange(() => btnCreateNewVisibility);
                }
            }
        }

        private bool _btnSaveAddNewVisibility = true;
        public bool btnSaveAddNewVisibility
        {
            get
            {
                return _btnSaveAddNewVisibility;
            }
            set
            {
                if (_btnSaveAddNewVisibility != value)
                {
                    _btnSaveAddNewVisibility = value;
                    NotifyOfPropertyChange(() => btnSaveAddNewVisibility);
                }
            }
        }


        private bool _bntSaveAsVisibility = true;
        public bool bntSaveAsVisibility
        {
            get
            {
                return _bntSaveAsVisibility;
            }
            set
            {
                if (_bntSaveAsVisibility != value)
                {
                    _bntSaveAsVisibility = value;
                    NotifyOfPropertyChange(() => bntSaveAsVisibility);
                }
            }
        }


        private bool _btnUpdateVisibility = true;
        public bool btnUpdateVisibility
        {
            get
            {
                return _btnUpdateVisibility;
            }
            set
            {
                if (_btnUpdateVisibility != value)
                {
                    _btnUpdateVisibility = value;
                    NotifyOfPropertyChange(() => btnUpdateVisibility);
                }
            }
        }

        private bool _btnCopyToVisible = true;
        public bool btnCopyToVisible
        {
            get
            {
                return _btnCopyToVisible;
            }
            set
            {
                if (_btnCopyToVisible != value)
                {
                    _btnCopyToVisible = value;
                    NotifyOfPropertyChange(() => btnCopyToVisible);
                }
            }
        }

        private bool _btChonChanDoanVisibility = true;
        public bool btChonChanDoanVisibility
        {
            get
            {
                return _btChonChanDoanVisibility;
            }
            set
            {
                if (_btChonChanDoanVisibility != value)
                {
                    _btChonChanDoanVisibility = value;
                    NotifyOfPropertyChange(() => btChonChanDoanVisibility);
                }
            }
        }


        #endregion


        #endregion


        #region Tính Tiền Thuốc
        public void btCalc()
        {

            if (grdPrescription == null)
            {
                MessageBox.Show(eHCMSResources.A0365_G1_Msg_InfoChonThuocDeRaToa, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }

            if (CheckAllThuocBiDiUng())
            {
                return;
            }

            if (ErrCheckChongChiDinh())
                return;

            if (grdPrescription.IsValid == false)
            {
                MessageBox.Show(string.Format("{0} {1}", eHCMSResources.A0540_G1_Msg_InfoDataKhHopLe, eHCMSResources.A0541_G1_Msg_InfoDataToaThuocKhHopLe), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }

            if (CheckThuocHopLe() == false)
                return;

            if (CheckThuocBiTrungTrongToa())
                return;

            short LoaiBenhNhan = 0;//--1;bh,0;binh thuong

            if (RegistrationCoverByHI())
            {
                LoaiBenhNhan = 1;
            }

            Action<IePrescriptionCalcNotSave> onInitDlg = delegate (IePrescriptionCalcNotSave typeInfo)
            {
                typeInfo.ObjPrescription = LatestePrecriptions;
                typeInfo.StoreID = StoreID.Value;
                typeInfo.RegistrationType = LoaiBenhNhan;
                typeInfo.Registration_DataStorage = Registration_DataStorage;
                typeInfo.Calc();
            };
            GlobalsNAV.ShowDialog<IePrescriptionCalcNotSave>(onInitDlg);

        }



        #endregion


        public void UpdateLatestePrecriptionsDrugNotInCat(PrescriptionDetail ObjPrescriptionDetail, int Index)
        {
            LatestePrecriptions.PrescriptionDetails[Index] = ObjPrescriptionDetail.DeepCopy();

            if (
                (LatestePrecriptions.PrescriptionDetails[LatestePrecriptions.PrescriptionDetails.Count - 1].DrugID == null
                ||
                LatestePrecriptions.PrescriptionDetails[LatestePrecriptions.PrescriptionDetails.Count - 1].DrugID == 0)
                &&
                LatestePrecriptions.PrescriptionDetails[LatestePrecriptions.PrescriptionDetails.Count - 1].IsDrugNotInCat == true
                )
            {
                AddNewBlankDrugIntoPrescriptObjectNew();
            }
        }



        #region "Check Thuoc Di Ung"
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
        private void MDAllergies_ByPatientID(Int64 PatientID, int flag)
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                IsWaitingMDAllergies_ByPatientID = true;

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
                            curPhysicalExamination = Globals.curPhysicalExamination;

                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            IsWaitingMDAllergies_ByPatientID = false;
                            this.HideBusyIndicator();
                        }

                    }), null);

                }

            });
            t.Start();
        }

        private bool Check1ThuocBiDiUng(string DrugName)
        {
            bool Res = false;

            foreach (var item in PtAllergyList)
            {
                if (DrugName.ToLower().Trim() == item.AllergiesItems.ToLower().Trim())
                {
                    Res = true;
                }
            }
            return Res;
        }

        private bool CheckAllThuocBiDiUng()
        {
            StringBuilder sb = new StringBuilder();

            bool Result = false;

            if (LatestePrecriptions.PrescriptionDetails.Count > 1)
            {
                foreach (PrescriptionDetail item in LatestePrecriptions.PrescriptionDetails)
                {
                    if (!CheckThuocHopLe(item))
                        continue;
                    if (item.SelectedDrugForPrescription != null && !string.IsNullOrEmpty(item.BrandName))
                    {
                        if (item.IsDrugNotInCat == true)
                        {
                            if (Check1ThuocBiDiUng(item.BrandName.Trim()))
                            {
                                sb.AppendLine("-" + item.BrandName);
                                Result = true;
                            }
                        }
                        else
                        {
                            if (Check1ThuocBiDiUng(item.SelectedDrugForPrescription.BrandName.Trim()))
                            {
                                sb.AppendLine("-" + item.SelectedDrugForPrescription.BrandName.Trim());
                                Result = true;
                            }
                        }
                    }
                }
            }
            if (Result)
            {
                MessageBox.Show(eHCMSResources.A0504_G1_Msg_InfoDSThuocDiUng + Environment.NewLine + sb.ToString(), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
            }
            return Result;
        }

        /// <summary>
        /// Kiem tra thuoc co hop le hay ko
        /// neu trong danh muc phai co drugID, ngoai dm phai co brandName
        /// /// </summary>
        /// <returns></returns>
        private bool CheckThuocHopLe(PrescriptionDetail item)
        {
            if (item.SelectedDrugForPrescription == null)
            {
                return false;
            }
            if (!item.IsDrugNotInCat && item.SelectedDrugForPrescription.DrugID < 1)
                return false;
            if (item.IsDrugNotInCat &&
                (item.SelectedDrugForPrescription.BrandName == null
                || item.SelectedDrugForPrescription.BrandName == ""))
            {
                return false;
            }
            return true;
        }

        #endregion


        public bool PrintInfoBH
        {
            get { return Registration_DataStorage != null && Registration_DataStorage.CurrentPatientRegistration != null && Registration_DataStorage.CurrentPatientRegistration.HisID > 0; }
        }

        public void ShowHiConfirmationReport()
        {
            Action<ICommonPreviewView> onInitDlg = delegate (ICommonPreviewView reportVm)
            {
                reportVm.RegistrationID = Registration_DataStorage.CurrentPatientRegistration.PtRegistrationID;

                if (Registration_DataStorage.CurrentPatientRegistration.V_RegistrationType == AllLookupValues.RegistrationType.NOI_TRU)
                {
                    reportVm.eItem = ReportName.REGISTRATION_IN_PATIENT_HI_CONFIRMATION;
                }
                else
                {
                    reportVm.eItem = ReportName.REGISTRATION_OUT_PATIENT_HI_CONFIRMATION;
                    //reportVm.eItem = ReportName.REGISTRATION_HI_APPOINTMENT;
                    reportVm.ServiceRecID = LatestePrecriptions != null ? LatestePrecriptions.ServiceRecID : 0;
                }
            };
            GlobalsNAV.ShowDialog<ICommonPreviewView>(onInitDlg);
        }


        public void ApptDate_CalendarClosed(object sender, System.Windows.RoutedEventArgs e)
        {
            DatePicker datePickCtrl = (DatePicker)sender;
            if (datePickCtrl.SelectedDate.HasValue == false)
            {
                return;
            }
            if (LatestePrecriptions.SelApptDate.HasValue && LatestePrecriptions.SelApptDate.Value > Globals.GetCurServerDateTime())
            {
                TimeSpan diffDateTime = LatestePrecriptions.SelApptDate.Value - Globals.GetCurServerDateTime();
                int nNumDayDiff = diffDateTime.Days + 1;
                if (CtrcboDonVi.SelectedIndex == 1 && CtrtbSoTuan != null)
                {
                    CtrtbSoTuan.Text = nNumDayDiff.ToString();
                }
                LatestePrecriptions.NDay = nNumDayDiff;
            }
        }

        private void GetPrescriptionTypeList()
        {
            ObservableCollection<Lookup> PrescriptLookupList = Globals.AllLookupValueList.Where(x => x.ObjectTypeID == (long)LookupValues.PRESCRIPTION_TYPE).ToObservableCollection();

            if (PrescriptLookupList == null || PrescriptLookupList.Count <= 0)
            {
                MessageBox.Show(eHCMSResources.A0742_G1_Msg_InfoKhTimThayLoaiToaThuoc, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }

            PrescriptionTypeList = new ObservableCollection<Lookup>(PrescriptLookupList);

            if (PrescriptionTypeList.Count > 0)
            {
                CurrentPrescriptionType = PrescriptionTypeList[0];
            }
        }

        //KMx: Sau khi lưu toa thuốc, load patient service record xong sẽ bắn event về đây (19/05/2014 17:25)
        public void Handle(LoadPrescriptionInPtAfterSaved message)
        {
            //KMx: Dời V_DiagnosisType từ PatientServiceRecord vào trong DiagnosisTreatment (08/06/2015 10:41).
            //KMx: Chưa có Service Record chứa chẩn đoán xuất viện (09/10/2014 15:37). 
            //if (Registration_DataStorage.PatientServiceRecordCollection == null || !Registration_DataStorage.PatientServiceRecordCollection.Any(x => x.V_DiagnosisType == (long)AllLookupValues.V_DiagnosisType.DIAGNOSIS_OUTHOS))
            if (!HasDiagnosisOutHospital())
            {
                MessageBox.Show(eHCMSResources.A0732_G1_Msg_InfoKhTimThayCDXV, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                return;
            }

            //KMx: Mỗi đăng ký nội trú chỉ có 1 service record chứa tất cả chẩn đoán (08/06/2015 10:31).
            //ObjPatientServiceRecord_Current = Registration_DataStorage.PatientServiceRecordCollection.Where(x => x.V_DiagnosisType == (long)AllLookupValues.V_DiagnosisType.DIAGNOSIS_OUTHOS).FirstOrDefault();
            ObjPatientServiceRecord_Current = Registration_DataStorage.PatientServiceRecordCollection.FirstOrDefault().DeepCopy();

            //KMx: Đã có kiểm tra trong function HasDiagnosisOutHospital() 
            //if (ObjPatientServiceRecord_Current.DiagnosisTreatments == null || ObjPatientServiceRecord_Current.DiagnosisTreatments.Count <= 0)
            //{
            //    MessageBox.Show(eHCMSResources.A0730_G1_Msg_InfoKhTimThayCDCuaBN, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
            //    return;
            //}

            if (ObjPatientServiceRecord_Current.PrescriptionIssueHistories == null || ObjPatientServiceRecord_Current.PrescriptionIssueHistories.Count <= 0)
            {
                MessageBox.Show(eHCMSResources.A0748_G1_Msg_InfoKhTimThayToaThuoc, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                return;
            }

            LatestePrecriptions = new Prescription();

            //ObjDiagnosisTreatment_Current = ObjPatientServiceRecord_Current.DiagnosisTreatments[0];
            if (Globals.ServerConfigSection.ConsultationElements.UseOnlyDailyDiagnosis && ObjPatientServiceRecord_Current.DiagnosisTreatments != null)
            {
                ObjDiagnosisTreatment_Current = ObjPatientServiceRecord_Current.DiagnosisTreatments.OrderBy(x => x.DTItemID).LastOrDefault();
            }
            else
            {
                ObjDiagnosisTreatment_Current = ObjPatientServiceRecord_Current.DiagnosisTreatments.Where(x => x.V_DiagnosisType == (long)AllLookupValues.V_DiagnosisType.DIAGNOSIS_OUTHOS).FirstOrDefault();
            }
            btnUndoIsEnabled = false;
            btnUpdateIsEnabled = false;

            GetToaThuocDaCo();

            if (ObjPatientServiceRecord_Current.PrescriptionIssueHistories[0].IssuerStaffID != Globals.LoggedUserAccount.StaffID)
            {
                //Chỉ Có Người Tạo Ra Toa Thuốc này mới được phép Chỉnh Sửa!
                PrescripState = PrescriptionState.PublishNewPrescriptionState;
            }
            else
            {
                PrescripState = PrescriptionState.EditPrescriptionState;
            }
        }

        private byte GetSchedBeginDOW()
        {
            //KMx: Nếu "tạo mới dựa trên toa cũ" thì OrigIssuedDateTime = NULL. Xuống dưới set lại là ngày hiện tại (06/06/2014 15:30).
            DateTime OrigIssuedDateTime = LatestePrecriptions.OrigIssuedDateTime.GetValueOrDefault();

            if (OrigIssuedDateTime == DateTime.MinValue)
            {
                OrigIssuedDateTime = Globals.ServerDate.Value;
            }

            return Globals.GetDayOfWeek(OrigIssuedDateTime);
        }

        public float[] CalDrugForDayFromSchedule(PrescriptionDetail prescriptDetail)
        {
            float[] WeeklySchedule = new float[7];

            if (prescriptDetail == null || prescriptDetail.ObjPrescriptionDetailSchedules == null || prescriptDetail.ObjPrescriptionDetailSchedules.Count <= 0)
            {
                return WeeklySchedule;
            }

            foreach (PrescriptionDetailSchedules item in prescriptDetail.ObjPrescriptionDetailSchedules)
            {
                WeeklySchedule[0] += item.Monday.GetValueOrDefault(0);
                WeeklySchedule[1] += item.Tuesday.GetValueOrDefault(0);
                WeeklySchedule[2] += item.Wednesday.GetValueOrDefault(0);
                WeeklySchedule[3] += item.Thursday.GetValueOrDefault(0);
                WeeklySchedule[4] += item.Friday.GetValueOrDefault(0);
                WeeklySchedule[5] += item.Saturday.GetValueOrDefault(0);
                WeeklySchedule[6] += item.Sunday.GetValueOrDefault(0);
            }

            prescriptDetail.QtySchedMon = WeeklySchedule[0];
            prescriptDetail.QtySchedTue = WeeklySchedule[1];
            prescriptDetail.QtySchedWed = WeeklySchedule[2];
            prescriptDetail.QtySchedThu = WeeklySchedule[3];
            prescriptDetail.QtySchedFri = WeeklySchedule[4];
            prescriptDetail.QtySchedSat = WeeklySchedule[5];
            prescriptDetail.QtySchedSun = WeeklySchedule[6];

            return WeeklySchedule;
        }

        private double CalQtyForScheduleDrug(PrescriptionDetail prescriptDetail, int nNumDayPrescribed)
        {
            prescriptDetail.SchedBeginDOW = GetSchedBeginDOW();

            float[] WeeklySchedule = CalDrugForDayFromSchedule(prescriptDetail);

            return Globals.CalcWeeklySchedulePrescription(prescriptDetail.SchedBeginDOW, nNumDayPrescribed, WeeklySchedule, (float)prescriptDetail.SelectedDrugForPrescription.DispenseVolume);
        }

        //KMx: Tính số lượng thuốc lịch (04/06/2014 15:39).
        private void CalQtyAndQtyMaxForSchedule(PrescriptionDetail prescriptDetail)
        {
            prescriptDetail.Qty = CalQtyForScheduleDrug(prescriptDetail, prescriptDetail.RealDay);

            if (prescriptDetail.IsDrugNotInCat)
            {
                return;
            }

            //KMx: Tính số lượng thuốc tối đa (thuốc lịch) mà BH đồng ý chi trả (05/06/2014 14:00).
            if (prescriptDetail.RealDay <= xNgayBHToiDa)
            {
                prescriptDetail.QtyMaxAllowed = prescriptDetail.Qty;
            }
            else
            {
                prescriptDetail.QtyMaxAllowed = CalQtyForScheduleDrug(prescriptDetail, xNgayBHToiDa);
            }
        }









        private ObservableCollection<Lookup> _DrugTypes;
        public ObservableCollection<Lookup> DrugTypes
        {
            get
            {
                return _DrugTypes;
            }
            set
            {
                if (_DrugTypes != value)
                {
                    _DrugTypes = value;
                    NotifyOfPropertyChange(() => DrugTypes);
                }
            }
        }

        private ObservableCollection<PrescriptionDetailSchedulesLieuDung> _LieuDung;
        public ObservableCollection<PrescriptionDetailSchedulesLieuDung> LieuDung
        {
            get
            {
                return _LieuDung;
            }
            set
            {
                if (_LieuDung != value)
                {
                    _LieuDung = value;
                    NotifyOfPropertyChange(() => _LieuDung);
                }
            }
        }

        private PhysicalExamination _curPhysicalExamination;
        public PhysicalExamination curPhysicalExamination
        {
            get { return _curPhysicalExamination; }
            set
            {
                if (_curPhysicalExamination != value)
                {
                    _curPhysicalExamination = value;
                    NotifyWhenBusy();
                    NotifyOfPropertyChange(() => curPhysicalExamination);
                }
            }
        }

        private bool _PreNoDrug;
        public bool PreNoDrug
        {
            get { return _PreNoDrug; }
            set
            {
                if (_PreNoDrug != value)
                {
                    _PreNoDrug = value;
                    NotifyOfPropertyChange(() => PreNoDrug);
                }
            }
        }

        private bool _isSearchByGenericName;
        public bool IsSearchByGenericName
        {
            get { return _isSearchByGenericName; }
            set
            {
                if (_isSearchByGenericName != value)
                {
                    _isSearchByGenericName = value;
                    NotifyOfPropertyChange(() => IsSearchByGenericName);
                }
            }
        }


        private void GetAllLookupForDrugTypes()
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {

                using (var serviceFactory = new CommonService_V2Client())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetAllLookupValuesByType(LookupValues.V_DrugType, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndGetAllLookupValuesByType(asyncResult);
                            DrugTypes = results.ToObservableCollection();
                            NotifyOfPropertyChange(() => DrugTypes);
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            //Globals.IsBusy = false;
                            this.HideBusyIndicator();
                        }

                    }), null);

                }

            });

            t.Start();
        }

        public void chkSearchByGenericName_Loaded(object sender, RoutedEventArgs e)
        {
            var chkSearchByGenericName = sender as CheckBox;

            if (Globals.ServerConfigSection.ConsultationElements.DefSearchByGenericName)
            {
                chkSearchByGenericName.IsChecked = true;
            }
            else
            {
                chkSearchByGenericName.IsChecked = false;
            }
        }

        public void cbxDrugType_Loaded(object sender, RoutedEventArgs e)
        {
            var kbEnabledComboBox = sender as KeyEnabledComboBox;
            if (kbEnabledComboBox != null)
            {
                kbEnabledComboBox.ItemsSource = DrugTypes;
            }
        }

        public void cbxDrugType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Debug.WriteLine(" ===>>> cbxDrugType_SelectionChanged : {0}", ((Lookup)((ComboBox)sender).SelectedItem).LookupID);
            if (LatestePrecriptions.PrescriptionDetails.Count <= 0)
            {
                // Txd: This check is ALMOST IMPOSSIBLE to happen but JUST IN CASE IT MAY
                //      HAPPEN IN THIS CRAZY PLACE
                return;
            }

            PrescriptionDetail selPrescriptionDetail = LatestePrecriptions.PrescriptionDetails[grdPrescription.SelectedIndex];
            if (selPrescriptionDetail == null)
                return;

            long nSelDrugTypeLookupID = ((Lookup)((ComboBox)sender).SelectedItem).LookupID;

            Debug.WriteLine(" ===>>> cbxDrugType_SelectionChanged: SELECTED DRUG TYPE = [{0}] --AND-- SAVED V_DRUGTYPE = [{1}].", nSelDrugTypeLookupID, selPrescriptionDetail.V_DrugType);

            if (nSelDrugTypeLookupID != selPrescriptionDetail.V_DrugType)
            {
                Debug.WriteLine(" ===>>> cbxDrugType_SelectionChanged: SELECTED DRUG TYPE = [{0}] --DIFF TO-- SAVED V_DRUGTYPE = [{1}].", nSelDrugTypeLookupID, selPrescriptionDetail.V_DrugType);

                if (selPrescriptionDetail.MDose > 0 || selPrescriptionDetail.NDose > 0 || selPrescriptionDetail.ADose > 0 || selPrescriptionDetail.EDose > 0 || selPrescriptionDetail.Qty > 0)
                {
                    if (MessageBox.Show(eHCMSResources.A1012_G1_Msg_ConfDoiLoaiThuoc, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                    {
                        ComboBox cbxTmp = (ComboBox)sender;
                        cbxTmp.SelectedItem = e.RemovedItems[0];
                        return;
                    }
                }
            }
            else
            {
                Debug.WriteLine(" ===>>> cbxDrugType_SelectionChanged: SELECTED DRUG TYPE = [{0}] --SAME-- AS SAVED V_DRUGTYPE = [{1}] ==> NOTHING TO DO HERE.", nSelDrugTypeLookupID, selPrescriptionDetail.V_DrugType);
                return;
            }

            ClearDataRow(selPrescriptionDetail);

            // Txd 30/09/2013
            // For some unknown reason at this stage that the BINDING of this COMBOBOX is NOT QUITE RIGHT 
            // So we just have to assign V_DrugType manually just incase
            selPrescriptionDetail.V_DrugType = nSelDrugTypeLookupID;

            switch (nSelDrugTypeLookupID)
            {
                case (long)AllLookupValues.V_DrugType.THUOC_UONGLICHTUAN:
                    Debug.WriteLine(" ===>>> cbxDrugType_SelectionChanged: LICH TUAN SELECTED.");
                    // Txd 23/09/2013 Note 
                    // For some reasons that are unknown at this stage, the DrugType property of PrescriptionDetail object 
                    // eventhough BOUND to the CTDrugType TextBlock but not yet UPDATED on this VERY Event 
                    // Thus we have to call hplEditSchedules DIRECTLY to avoid the check in of DrugType in method hplEditSchedules_Click                    

                    // Txd 27/09/2013 For some reason this method cbxDrugType_SelectionChangedis being called twice upon a selection change using 
                    // the keyboard to select so we need to double check to make sure the Schedule Dialog is not being shown twice
                    //if (selPrescriptionDetail.DrugType.LookupID != (long)AllLookupValues.V_DrugType.THUOC_UONGLICHTUAN)
                    //{
                    Debug.WriteLine(" ===>>> cbxDrugType_SelectionChanged: LICH TUAN SELECTED AND SHOW SHEDULE Dialog.");
                    hplEditSchedules(selPrescriptionDetail);
                    //}
                    break;
                case (long)AllLookupValues.V_DrugType.THUOC_UONGKHICAN:
                    Debug.WriteLine(" ===>>> cbxDrugType_SelectionChanged: UONG KHI CAN SELECTED called InitUsageDaysForDrugTakenAsRequired.");
                    InitUsageDaysForDrugTakenAsRequired(selPrescriptionDetail);
                    break;
            }


            //if (((Lookup)((ComboBox)sender).SelectedItem).LookupID == (long)AllLookupValues.V_DrugType.THUOC_UONGLICHTUAN)
            //{
            //    //SelectedPrescriptionDetail.DrugType = (Lookup)((ComboBox)sender).SelectedItem;
            //    //
            //    // Txd 23/09/2013 Note 
            //    // For some reasons that are unknown at this stage, the DrugType property of PrescriptionDetail object 
            //    // eventhough BOUND to the CTDrugType TextBlock but not yet UPDATED on this VERY Event 
            //    // Thus we have to call hplEditSchedules DIRECTLY to avoid the check in of DrugType in method hplEditSchedules_Click
            //    //
            //    // hplEditSchedules_Click(selPrescriptionDetail);
            //    // Txd 27/09/2013 For some reason this method cbxDrugType_SelectionChangedis being called twice upon a selection change using 
            //    // the keyboard to select so we need to double check to make sure the Schedule Dialog is not being shown twice
            //    if (selPrescriptionDetail.DrugType.LookupID != (long)AllLookupValues.V_DrugType.THUOC_UONGLICHTUAN)
            //    {
            //        hplEditSchedules(selPrescriptionDetail);
            //    }
            //}
            //else if (((Lookup)((ComboBox)sender).SelectedItem).LookupID == (long)AllLookupValues.V_DrugType.THUOC_UONGKHICAN)
            //{
            //    //SelectedPrescriptionDetail.DrugType = (Lookup)((ComboBox)sender).SelectedItem;
            //    InitUsageDaysForDrugTakenAsRequired(selPrescriptionDetail);                
            //}

        }


        private void MovePresDetailRowUpDown(bool bMoveDown)
        {
            if (!SelectedPrescriptionDetailIsValid())
            {
                MessageBox.Show(eHCMSResources.A0338_G1_Msg_InfoChonMotThuoc, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
            PrescriptionDetail selPrescriptionDetail = LatestePrecriptions.PrescriptionDetails[grdPrescription.SelectedIndex];
            int selIndex = grdPrescription.SelectedIndex;
            // Txd: The following is just a double check for the above just in case. TO BE REMOVED EVENTUALLY
            if (selPrescriptionDetail == null || (selIndex >= LatestePrecriptions.PrescriptionDetails.Count - 1))
            {
                // This is the empty Row CANNOT Move UP or DOWN
                MessageBox.Show(eHCMSResources.A0338_G1_Msg_InfoChonMotThuoc);
                return;
            }


            if (selPrescriptionDetail.SelectedDrugForPrescription.BrandName.Length < 2)
            {
                MessageBox.Show(string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.Z0442_G1_SomethingIsWrongHere));
                return;
            }

            if (bMoveDown)
            {
                if (selIndex >= (LatestePrecriptions.PrescriptionDetails.Count - 2))
                {
                    MessageBox.Show(eHCMSResources.K0270_G1_ViTriThuocKhongThayDoi);
                    return;
                }
            }
            else
            {
                if (selIndex <= 0)
                {
                    MessageBox.Show(eHCMSResources.K0270_G1_ViTriThuocKhongThayDoi);
                    return;
                }
            }

            var newDetails = new ObservableCollection<PrescriptionDetail>();
            var selObj = new PrescriptionDetail();
            selObj = ObjectCopier.DeepCopy(LatestePrecriptions.PrescriptionDetails[selIndex]);

            int nIdxToAddSelObj = (bMoveDown ? (selIndex + 1) : (selIndex - 1));
            int nIdx = 0;
            foreach (var itemPresDetail in LatestePrecriptions.PrescriptionDetails)
            {
                if (!bMoveDown && nIdx == nIdxToAddSelObj)
                {
                    newDetails.Add(selObj);
                }

                if (nIdx != selIndex)
                {
                    var newObj = new PrescriptionDetail();
                    newObj = ObjectCopier.DeepCopy(itemPresDetail);
                    newDetails.Add(newObj);
                }

                if (bMoveDown && nIdx == nIdxToAddSelObj)
                {
                    newDetails.Add(selObj);
                }

                ++nIdx;
            }

            LatestePrecriptions.PrescriptionDetails = newDetails;
            grdPrescription.SelectedIndex = nIdxToAddSelObj;

        }


        public void btnDown()
        {
            MovePresDetailRowUpDown(true);
        }

        public void btnUp()
        {
            MovePresDetailRowUpDown(false);
        }

        public void chk_NotInCat_Click(object sender, RoutedEventArgs e)
        {
            if (LatestePrecriptions.PrescriptionDetails.Count <= 0)
                return;

            PrescriptionDetail selPrescriptionDetail = LatestePrecriptions.PrescriptionDetails[grdPrescription.SelectedIndex];
            if (selPrescriptionDetail == null)
                return;

            CheckBox ckbNotIncat = (CheckBox)sender;
            if (ckbNotIncat.IsChecked == true)
            {
                if (selPrescriptionDetail.DrugID > 0)
                {
                    // Thuoc Trong Danh muc doi sang ngoai danh muc
                    if (MessageBox.Show(eHCMSResources.A0189_G1_Msg_InfoVuaDoiThuocThanhNgoaiDM, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                    {
                        //CreateNewDrugForSellVisitor(selPrescriptionDetail);
                        selPrescriptionDetail = NewReInitPrescriptionDetail(false, null);
                        selPrescriptionDetail.IsDrugNotInCat = true;
                    }
                    else
                    {
                        ckbNotIncat.IsChecked = false;
                    }
                }
            }
            else if (ckbNotIncat.IsChecked == false)
            {
                if (selPrescriptionDetail.DrugID > 0)
                {
                    MessageBox.Show(eHCMSResources.Z0442_G1_SomethingIsWrongHere);
                    //return;
                }

                // Thuoc Ngoai Danh muc doi sang trong danh muc
                if (MessageBox.Show(eHCMSResources.A0188_G1_Msg_InfoVuaDoiThuocThanhTrongDM, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    //CreateNewDrugForSellVisitor(selPrescriptionDetail);
                    selPrescriptionDetail = NewReInitPrescriptionDetail(false, null);
                    selPrescriptionDetail.IsDrugNotInCat = false;
                }
                else
                {
                    ckbNotIncat.IsChecked = true;
                }
            }

        }


        //KMx: Khi dùng chuột chọn 1 loại thuốc trong Drop Down List thì hàm này sẽ được gọi 2 lần. Còn nếu dùng phím Enter để chọn thì chỉ gọi 1 lần (03/06/2014 10:46).
        public void acbDrug_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            if (grdPrescription == null)
            {
                return;
            }
            //▼===== #005
            if (grdPrescription.SelectedIndex < 0)
            {
                return;
            }
            //▲===== #005
            PrescriptionDetail selPrescriptionDetail = LatestePrecriptions.PrescriptionDetails[grdPrescription.SelectedIndex];

            if (selPrescriptionDetail.IsDrugNotInCat)
            {
                selPrescriptionDetail.SelectedDrugForPrescription = new GetDrugForSellVisitor();
                selPrescriptionDetail.SelectedDrugForPrescription.BrandName = AutoGenMedProduct.SearchText;
                ClearDataRow(selPrescriptionDetail);
            }
            else
            {
                if (grdPrescription.SelectedItem != null && AutoGenMedProduct != null && AutoGenMedProduct.SelectedItem != null
                    && selPrescriptionDetail.SelectedDrugForPrescription.DrugID != (AutoGenMedProduct.SelectedItem as GetDrugForSellVisitor).DrugID)
                {
                    selPrescriptionDetail.SelectedDrugForPrescription = ObjectCopier.DeepCopy(AutoGenMedProduct.SelectedItem as GetDrugForSellVisitor);
                    ClearDataRow(selPrescriptionDetail);
                    //▼====: #012
                    GetRouteOfAdministrationList(selPrescriptionDetail, 0, selPrescriptionDetail.SelectedDrugForPrescription.DrugID);
                    //▲====: #012
                }
            }
        }
        #region

        public void ChangeDay(int num)
        {
            AutoAdjustCancelDrugShortDays();

        }

        public void tbDayTotal_LostFocus(object sender, RoutedEventArgs e)
        {
            //SplitDayTotal(ObjPrescriptionDetailForForm);

            GetDayRpt(ObjPrescriptionDetailForForm, ObjPrescriptionDetailForForm.RealDay);
        }

        private AxComboBox CtrcboDonVi;
        public void cboDonVi_Loaded(object sender, RoutedEventArgs e)
        {
            CtrcboDonVi = sender as AxComboBox;
        }

        private AxTextBox CtrtbSoTuan;

        public void tbSoTuan_KeyUp(object sender, KeyEventArgs e)
        {
            string v = (sender as TextBox).Text;
            if (!string.IsNullOrEmpty(v))
            {
                int num = 0;
                int.TryParse(v, out num);

                ((System.Windows.Controls.TextBox)(sender)).Text = num.ToString();//show ra giao diện

                if (CtrcboDonVi.SelectedIndex == 0)
                {
                    LatestePrecriptions.NDay = num * 7;
                }
                else
                {
                    LatestePrecriptions.NDay = num;
                }
            }

        }
        public void tbSoTuan_Loaded(object sender, KeyEventArgs e)
        {
            CtrtbSoTuan = sender as AxTextBox;
            CtrtbSoTuan.Focus();
        }

        public void cboDonVi_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            AxComboBox Ctr = sender as AxComboBox;

            string v = CtrtbSoTuan.Text;
            if (!string.IsNullOrEmpty(v))
            {
                int num = 0;
                int.TryParse(v, out num);

                if (Ctr.SelectedIndex == 0)
                {
                    LatestePrecriptions.NDay = num * 7;
                }
                else
                {
                    LatestePrecriptions.NDay = num;
                }
                // NDayGen = LatestePrecriptions.NDay.Value;
            }
        }



        public void btUpdateNgayDung()
        {
            int nDayVal = LatestePrecriptions.NDay == null ? 0 : LatestePrecriptions.NDay.Value;
            ChangeDay(nDayVal);
        }

        #endregion


        #region DataGrid
        public void grdPrescription_LoadingRow(object sender, DataGridRowEventArgs e)
        {

            e.Row.Header = (e.Row.GetIndex() + 1).ToString() + ". ";

            PrescriptionDetail objRows = e.Row.DataContext as PrescriptionDetail;

            if (objRows.HasSchedules)
            {
                e.Row.Background = new SolidColorBrush(Color.FromArgb(205, 180, 200, 120));
            }
            else
            {
                e.Row.Background = new SolidColorBrush(Color.FromArgb(255, 248, 248, 248));
            }

            switch (objRows.V_DrugType)
            {
                case (long)AllLookupValues.V_DrugType.THUOC_UONGKHICAN:
                    e.Row.Background = new SolidColorBrush(Color.FromArgb(205, 180, 200, 120)); break;
            }
            if (objRows.SelectedDrugForPrescription != null
                            && objRows.SelectedDrugForPrescription.MaxDayPrescribed != null
                            && objRows.SelectedDrugForPrescription.MaxDayPrescribed > 0)
            {
                e.Row.Background = new SolidColorBrush(Color.FromArgb(200, 224, 130, 228));//pink
                objRows.BackGroundColor = "#E79DEA";
                NotifyOfPropertyChange(() => e.Row.Background);
            }
        }


        public void grdPrescription_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {

            if (LatestePrecriptions.PrescriptionDetails == null)
            {
                return;
            }

            // Txd 28/092013 
            // For some reason the Grid call this function when focus was outside of the bottom row, so we have to check this to ensure that
            // Removelastrow does not stuff it up.
            if (LatestePrecriptions.PrescriptionDetails.Count <= 0)
            {
                return;
            }

            //KMx: Click any cell of the last row, then click on space of the gird (click out of grid, error not appear).
            //When you click the save button, the function RemoveLastRowPrecriptionDetail() is called and it remove the last row of LatestePrecriptions.PrescriptionDetails.
            //After remove the last row, caliburn will call grdPrescription_CellEditEnded() and exception occurs at line LatestePrecriptions.PrescriptionDetails[grdPrescription.SelectedIndex] because index was out of range.
            //So we need to check index is valid (04/10/2016 11:43).
            if (grdPrescription.SelectedIndex > LatestePrecriptions.PrescriptionDetails.Count - 1)
            {
                return;
            }

            PrescriptionDetail selPrescriptionDetail = LatestePrecriptions.PrescriptionDetails[grdPrescription.SelectedIndex];
            if (selPrescriptionDetail == null)
            {
                return;
            }

            int nDayVal = LatestePrecriptions.NDay == null ? 0 : LatestePrecriptions.NDay.Value;

            if (selPrescriptionDetail.SelectedDrugForPrescription != null
                            && selPrescriptionDetail.SelectedDrugForPrescription.MaxDayPrescribed != null
                            && selPrescriptionDetail.SelectedDrugForPrescription.MaxDayPrescribed > 0)
            {
                e.Row.Background = new SolidColorBrush(Color.FromArgb(200, 224, 130, 228));
                selPrescriptionDetail.BackGroundColor = "#E79DEA";
                NotifyOfPropertyChange(() => selPrescriptionDetail.BackGroundColor);
            }
            if (e.Column.DisplayIndex == (int)DataGridCol.DRUG_NAME)
            {
                if (selPrescriptionDetail.IsDrugNotInCat && AutoGenMedProduct.SearchText != "")
                {
                    selPrescriptionDetail.SelectedDrugForPrescription.BrandName = AutoGenMedProduct.SearchText;
                    ClearDataRow(selPrescriptionDetail);
                }
            }
            if (ischanged(grdPrescription.SelectedItem))
            {
                if (e.Row.GetIndex() == (LatestePrecriptions.PrescriptionDetails.Count - 1) && e.EditAction == DataGridEditAction.Commit)
                {
                    AddNewBlankDrugIntoPrescriptObjectNew();
                }

                if (e.Column.DisplayIndex == (int)DataGridCol.DRUG_NAME)
                {
                    if (((PrescriptionDetail)this.grdPrescription.SelectedItem).IsDrugNotInCat == true)
                    {
                        if (Check1ThuocBiDiUng(((PrescriptionDetail)this.grdPrescription.SelectedItem).BrandName))
                        {
                            MessageBox.Show(string.Format(eHCMSResources.Z0990_G1_Thuoc0DiUngBN, ((PrescriptionDetail)this.grdPrescription.SelectedItem).BrandName.Trim()));
                            return;
                        }
                    }
                    else
                    {
                        if (Check1ThuocBiDiUng(((PrescriptionDetail)this.grdPrescription.SelectedItem).SelectedDrugForPrescription.BrandName))
                        {
                            MessageBox.Show(string.Format(eHCMSResources.Z0990_G1_Thuoc0DiUngBN, ((PrescriptionDetail)this.grdPrescription.SelectedItem).SelectedDrugForPrescription.BrandName.Trim()));
                            return;
                        }
                    }


                    if (((PrescriptionDetail)this.grdPrescription.SelectedItem).DrugID != null && (long)((PrescriptionDetail)this.grdPrescription.SelectedItem).DrugID > 0)
                    {
                        Globals.CheckContrain(PtMedCond, (long)((PrescriptionDetail)this.grdPrescription.SelectedItem).DrugID);
                    }
                }
                else if (e.Column.DisplayIndex == (int)DataGridCol.QTY)
                {
                    if (((PrescriptionDetail)this.grdPrescription.SelectedItem).IsDrugNotInCat)
                    {
                        return;
                    }

                    //KMx: Nếu là thuốc cần thì QtyMaxAllowed = Qty (A.Tuấn quyết định) (05/06/2014 16:61).
                    if (selPrescriptionDetail.isNeedToUse)
                    {
                        selPrescriptionDetail.QtyMaxAllowed = selPrescriptionDetail.Qty;
                    }

                    else
                    {
                        AdjustQtyMaxAllowed(selPrescriptionDetail);
                    }

                    if (((PrescriptionDetail)this.grdPrescription.SelectedItem).SelectedDrugForPrescription.Remaining < ((PrescriptionDetail)this.grdPrescription.SelectedItem).Qty)
                    {
                        MessageBox.Show(eHCMSResources.A0977_G1_Msg_InfoSLgKhDuBan);
                    }
                }

                else if (e.Column.DisplayIndex == (int)DataGridCol.MDOSE)
                {
                    ChangeAnyDoseQty(1, selPrescriptionDetail != null ? selPrescriptionDetail.MDoseStr : "0", selPrescriptionDetail);
                }
                else if (e.Column.DisplayIndex == (int)DataGridCol.NDOSE)
                {
                    ChangeAnyDoseQty(2, selPrescriptionDetail != null ? selPrescriptionDetail.NDoseStr : "0", selPrescriptionDetail);
                }
                else if (e.Column.DisplayIndex == (int)DataGridCol.ADOSE)
                {
                    ChangeAnyDoseQty(3, selPrescriptionDetail != null ? selPrescriptionDetail.ADoseStr : "0", selPrescriptionDetail);
                }
                else if (e.Column.DisplayIndex == (int)DataGridCol.EDOSE)
                {
                    ChangeAnyDoseQty(4, selPrescriptionDetail != null ? selPrescriptionDetail.EDoseStr : "0", selPrescriptionDetail);
                }

                else if (e.Column.DisplayIndex == (int)DataGridCol.DayTotalCol)
                {
                    GetDayRpt((PrescriptionDetail)this.grdPrescription.SelectedItem, ((PrescriptionDetail)this.grdPrescription.SelectedItem).RealDay);
                }

                else if (e.Column.DisplayIndex == (int)DataGridCol.USAGE)
                {
                    if (selPrescriptionDetail.SelectedDrugForPrescription != null && selPrescriptionDetail.SelectedDrugForPrescription.Administration != null)
                    {
                        selPrescriptionDetail.Administration = selPrescriptionDetail.SelectedDrugForPrescription.Administration;
                    }
                }
                else if (e.Column.DisplayIndex == (int)DataGridCol.NotInCat)
                {

                }
            }
        }



        AutoCompleteBox AutoGenMedProduct;
        bool bRePopulateDrugList = true;

        public void acbDrug_Loaded(object sender, RoutedEventArgs e)
        {
            AutoGenMedProduct = sender as AutoCompleteBox;
        }

        public void acbDrug_Populating(object sender, PopulatingEventArgs e)
        {
            if (BrandName != e.Parameter)
            {
            }

            if (!bRePopulateDrugList)
            {
                bRePopulateDrugList = true;
                AutoGenMedProduct.PopulateComplete();
                return;
            }

            e.Cancel = true;
            DrugList.PageIndex = 0;
            BrandName = e.Parameter;
            PrescriptionDetail selPrescriptionDetail = LatestePrecriptions.PrescriptionDetails[grdPrescription.SelectedIndex];
            if (selPrescriptionDetail != null)
            {
                if (selPrescriptionDetail.IsDrugNotInCat)
                {
                    return;
                }
                CheckBox check = grdPrescription.Columns[(int)DataGridCol.HI].GetCellContent(grdPrescription.SelectedItem) as CheckBox;
                AutoCompleteBox completebox = grdPrescription.Columns[(int)DataGridCol.DRUG_NAME].GetCellContent(grdPrescription.SelectedItem) as AutoCompleteBox;

                if (selPrescriptionDetail.BeOfHIMedicineList)
                {
                    IsInsurance = 1;
                    SearchDrugForPrescription_Paging(StoreID, DrugList.PageIndex, DrugList.PageSize, true, IsSearchByGenericName);
                }
                else
                {
                    IsInsurance = 0;
                    SearchDrugForPrescription_Paging(StoreID, DrugList.PageIndex, DrugList.PageSize, true, IsSearchByGenericName);
                }
            }

        }

        public void acbDrug_LostFocus(object sender, RoutedEventArgs e)
        {
        }

        public void acbDrug_MouseLeftButtonUp(object sender, RoutedEventArgs e)
        {
            bRePopulateDrugList = false;
            //AutoGenMedProduct.IsDropDownOpen = true;
        }

        public void acbDrug_TextChanged(object sender, RoutedEventArgs e)
        {
            bRePopulateDrugList = true;
        }

        public void lnkDelete_Click(object sender, RoutedEventArgs e)
        {
            PrescriptionDetail p = grdPrescription.SelectedItem as PrescriptionDetail;
            if (p == null)
            {
                MessageBox.Show(eHCMSResources.Z0442_G1_SomethingIsWrongHere);
                return;
            }

            int nSelIndex = grdPrescription.SelectedIndex;
            if (nSelIndex >= LatestePrecriptions.PrescriptionDetails.Count - 1)
            {
                MessageBox.Show(eHCMSResources.A0678_G1_Msg_InfoKhDcXoaDongRong);
                return;
            }

            if (p.DrugID > 0 || p.IsDrugNotInCat)
            {
                //string strMsg = "Bạn Có Chắc Muốn Xóa Dòng Thuoc [" + p.BrandName +"] Này Không ?";
                string strMsg = eHCMSResources.Z0554_G1_CoChacMuonXoaDongNayKg;
                if (MessageBox.Show(strMsg, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) != MessageBoxResult.OK)
                {
                    return;
                }
            }

            LatestePrecriptions.PrescriptionDetails.Remove(p);
            if (LatestePrecriptions.PrescriptionDetails.Count < 2)
            {
                LatestePrecriptions.PreNoDrug = true;
                PreNoDrug = false;
                NotifyOfPropertyChange(() => LatestePrecriptions.PreNoDrug);
            }
        }

        public void grdPrescription_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (grdPrescription != null)
            {
                if (grdPrescription.SelectedItem != null)
                {
                    grdPrescription.BeginEdit();
                }
            }
        }

        #endregion


        #region autocomplete box for DVT, Cach Dung, Ghi Chu

        //KMx: Code này được copy từ PatientDetailsViewModel.cs chỗ tìm kiếm Tp/Tỉnh (31/05/2014 11:35).
        public string ConvertString(string stringInput)
        {
            stringInput = stringInput.ToUpper();
            string convert = "ĂÂÀẰẦÁẮẤẢẲẨÃẴẪẠẶẬỄẼỂẺÉÊÈỀẾẸỆÔÒỒƠỜÓỐỚỎỔỞÕỖỠỌỘỢƯÚÙỨỪỦỬŨỮỤỰÌÍỈĨỊỲÝỶỸỴĐăâàằầáắấảẳẩãẵẫạặậễẽểẻéêèềếẹệôòồơờóốớỏổởõỗỡọộợưúùứừủửũữụựìíỉĩịỳýỷỹỵđ";
            string To = "AAAAAAAAAAAAAAAAAEEEEEEEEEEEOOOOOOOOOOOOOOOOOUUUUUUUUUUUIIIIIYYYYYDaaaaaaaaaaaaaaaaaeeeeeeeeeeeooooooooooooooooouuuuuuuuuuuiiiiiyyyyyd";
            for (int i = 0; i < To.Length; i++)
            {
                stringInput = stringInput.Replace(convert[i], To[i]);
            }
            return stringInput;
        }

        public void DrugUnitUse_Loaded(object sender, PopulatingEventArgs e)
        {
            ((AxAutoComplete)sender).ItemsSource = DonViTinh;
        }
        public void DrugUnitUse_Populating(object sender, PopulatingEventArgs e)
        {
            if (SelectedPrescriptionDetailIsValid())
            {
                PrescriptionDetail selPrescriptionDetail = LatestePrecriptions.PrescriptionDetails[grdPrescription.SelectedIndex];
                selPrescriptionDetail.UnitUse = e.Parameter;
            }
        }
        public void DrugUnitUse_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!SelectedPrescriptionDetailIsValid())
            {
                MessageBox.Show(eHCMSResources.A0534_G1_Msg_InfoDongThuocKhHopLe, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
            PrescriptionDetail selPrescriptionDetail = LatestePrecriptions.PrescriptionDetails[grdPrescription.SelectedIndex];
            if (grdPrescription != null && grdPrescription.SelectedItem != null && ((AxAutoComplete)sender) != null)
            {
                if (((AxAutoComplete)sender).SelectedItem != null)
                {
                    selPrescriptionDetail.UnitUse = ((PrescriptionNoteTemplates)((AxAutoComplete)sender).SelectedItem).NoteDetails;
                }
                else
                {
                    selPrescriptionDetail.UnitUse = ((AxAutoComplete)sender).Text;
                }
            }
        }

        public void DrugInstructionNotes_Loaded(object sender, PopulatingEventArgs e)
        {
            ((AxAutoComplete)sender).ItemsSource = GhiChu;
        }

        private ObservableCollection<PrescriptionNoteTemplates> _GhiChuAfterFiltering;
        public ObservableCollection<PrescriptionNoteTemplates> GhiChuAfterFiltering
        {
            get { return _GhiChuAfterFiltering; }
            set
            {
                _GhiChuAfterFiltering = value;
                NotifyOfPropertyChange(() => GhiChuAfterFiltering);
            }
        }

        //KMx: Code này được copy từ PatientDetailsViewModel.cs chỗ tìm kiếm Tp/Tỉnh (31/05/2014 11:35).
        public void DrugInstructionNotes_Populating(object sender, PopulatingEventArgs e)
        {
            if (sender != null)
            {
                string SearchText = ((AutoCompleteBox)sender).SearchText;
                GhiChuAfterFiltering = new ObservableCollection<PrescriptionNoteTemplates>(GhiChu.Where(item => ConvertString(item.NoteDetails)
                     .IndexOf(ConvertString(SearchText), StringComparison.InvariantCultureIgnoreCase) >= 0));
                ((AutoCompleteBox)sender).ItemsSource = GhiChuAfterFiltering;
                ((AutoCompleteBox)sender).PopulateComplete();
            }
        }


        public void DrugAdministration_Loaded(object sender, PopulatingEventArgs e)
        {
            ((AxAutoComplete)sender).ItemsSource = CachDung;
        }

        private ObservableCollection<PrescriptionNoteTemplates> _CachDungAfterFiltering;
        public ObservableCollection<PrescriptionNoteTemplates> CachDungAfterFiltering
        {
            get { return _CachDungAfterFiltering; }
            set
            {
                _CachDungAfterFiltering = value;
                NotifyOfPropertyChange(() => CachDungAfterFiltering);
            }
        }

        //KMx: Code này được copy từ PatientDetailsViewModel.cs chỗ tìm kiếm Tp/Tỉnh (31/05/2014 11:35).
        public void DrugAdministration_Populating(object sender, PopulatingEventArgs e)
        {
            if (sender != null)
            {
                string SearchText = ((AutoCompleteBox)sender).SearchText;
                CachDungAfterFiltering = new ObservableCollection<PrescriptionNoteTemplates>(CachDung.Where(item => ConvertString(item.NoteDetails)
                     .IndexOf(ConvertString(SearchText), StringComparison.InvariantCultureIgnoreCase) >= 0));
                ((AutoCompleteBox)sender).ItemsSource = CachDungAfterFiltering;
                ((AutoCompleteBox)sender).PopulateComplete();
            }
        }


        #endregion
        #region PrescriptionNoteTemplates

        private PrescriptionNoteTemplates _ObjPrescriptionNoteTemplates_Selected;
        public PrescriptionNoteTemplates ObjPrescriptionNoteTemplates_Selected
        {
            get { return _ObjPrescriptionNoteTemplates_Selected; }
            set
            {
                if (_ObjPrescriptionNoteTemplates_Selected == value)
                {
                    return;
                }
                _ObjPrescriptionNoteTemplates_Selected = value;
                NotifyOfPropertyChange(() => ObjPrescriptionNoteTemplates_Selected);

                if (_ObjPrescriptionNoteTemplates_Selected != null && _ObjPrescriptionNoteTemplates_Selected.PrescriptNoteTemplateID > 0)
                {
                    string str = LatestePrecriptions.DoctorAdvice;
                    if (string.IsNullOrEmpty(str))
                    {
                        str = ObjPrescriptionNoteTemplates_Selected.DetailsTemplate;
                    }
                    else
                    {
                        str = str + Environment.NewLine + ObjPrescriptionNoteTemplates_Selected.DetailsTemplate;
                    }

                    LatestePrecriptions.DoctorAdvice = str;
                }
            }
        }

        private ObservableCollection<PrescriptionNoteTemplates> _ObjPrescriptionNoteTemplates_GetAll;
        public ObservableCollection<PrescriptionNoteTemplates> ObjPrescriptionNoteTemplates_GetAll
        {
            get { return _ObjPrescriptionNoteTemplates_GetAll; }
            set
            {
                _ObjPrescriptionNoteTemplates_GetAll = value;
                NotifyOfPropertyChange(() => ObjPrescriptionNoteTemplates_GetAll);
            }
        }

        private ObservableCollection<PrescriptionNoteTemplates> _DonViTinh;
        public ObservableCollection<PrescriptionNoteTemplates> DonViTinh
        {
            get { return _DonViTinh; }
            set
            {
                _DonViTinh = value;
                NotifyOfPropertyChange(() => DonViTinh);
            }
        }

        private ObservableCollection<PrescriptionNoteTemplates> _GhiChu;
        public ObservableCollection<PrescriptionNoteTemplates> GhiChu
        {
            get { return _GhiChu; }
            set
            {
                _GhiChu = value;
                NotifyOfPropertyChange(() => GhiChu);
            }
        }

        private ObservableCollection<PrescriptionNoteTemplates> _CachDung;
        public ObservableCollection<PrescriptionNoteTemplates> CachDung
        {
            get { return _CachDung; }
            set
            {
                _CachDung = value;
                NotifyOfPropertyChange(() => CachDung);
            }
        }

        public void PrescriptionNoteTemplates_GetAllIsActive()
        {
            var t = new Thread(() =>
            {
                IsWaitingPrescriptionNoteTemplates_GetAll = true;

                try
                {
                    using (var serviceFactory = new ePrescriptionsServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        var pnt = new PrescriptionNoteTemplates();
                        pnt.V_PrescriptionNoteTempType = AllLookupValues.V_PrescriptionNoteTempType.PrescriptionNoteGen;
                        contract.BeginPrescriptionNoteTemplates_GetAllIsActive(pnt, Globals.DispatchCallback((asyncResult) =>
                        {
                            IList<PrescriptionNoteTemplates> allItems = new ObservableCollection<PrescriptionNoteTemplates>();
                            try
                            {
                                allItems = contract.EndPrescriptionNoteTemplates_GetAllIsActive(asyncResult);

                                ObjPrescriptionNoteTemplates_GetAll = new ObservableCollection<PrescriptionNoteTemplates>(allItems);

                                PrescriptionNoteTemplates firstItem = new PrescriptionNoteTemplates();
                                firstItem.PrescriptNoteTemplateID = -1;
                                firstItem.NoteDetails = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.K0616_G1_ChonMau);
                                ObjPrescriptionNoteTemplates_GetAll.Insert(0, firstItem);

                            }
                            catch (FaultException<AxException> fault)
                            {
                                ClientLoggerHelper.LogInfo(fault.ToString());
                            }
                            catch (Exception ex)
                            {
                                ClientLoggerHelper.LogInfo(ex.ToString());
                            }

                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                }
                finally
                {
                    IsWaitingPrescriptionNoteTemplates_GetAll = false;
                }
            });
            t.Start();
        }
        public void PrescriptionNoteTemplates_GetAllIsActiveItem()
        {
            var t = new Thread(() =>
            {
                IsWaitingPrescriptionNoteTemplates_GetAll = true;

                try
                {
                    using (var serviceFactory = new ePrescriptionsServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        var pnt = new PrescriptionNoteTemplates();
                        pnt.V_PrescriptionNoteTempType = AllLookupValues.V_PrescriptionNoteTempType.PrescriptionNoteItem;
                        contract.BeginPrescriptionNoteTemplates_GetAllIsActive(pnt, Globals.DispatchCallback((asyncResult) =>
                        {
                            IList<PrescriptionNoteTemplates> allItems = new ObservableCollection<PrescriptionNoteTemplates>();
                            try
                            {
                                allItems = contract.EndPrescriptionNoteTemplates_GetAllIsActive(asyncResult);
                                GhiChu = new ObservableCollection<PrescriptionNoteTemplates>(allItems.OrderBy(x => x.NoteDetails));
                                NotifyOfPropertyChange(() => GhiChu);
                            }
                            catch (FaultException<AxException> fault)
                            {
                                ClientLoggerHelper.LogInfo(fault.ToString());
                            }
                            catch (Exception ex)
                            {
                                ClientLoggerHelper.LogInfo(ex.ToString());
                            }

                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                }
                finally
                {
                    IsWaitingPrescriptionNoteTemplates_GetAll = false;
                }
            });
            t.Start();
        }

        public void PrescriptionNoteTemplates_GetAllIsActiveAdm()
        {
            var t = new Thread(() =>
            {
                IsWaitingPrescriptionNoteTemplates_GetAll = true;

                try
                {
                    using (var serviceFactory = new ePrescriptionsServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        var pnt = new PrescriptionNoteTemplates();
                        pnt.V_PrescriptionNoteTempType = AllLookupValues.V_PrescriptionNoteTempType.PrescriptionAdministration;
                        contract.BeginPrescriptionNoteTemplates_GetAllIsActive(pnt, Globals.DispatchCallback((asyncResult) =>
                        {
                            IList<PrescriptionNoteTemplates> allItems = new ObservableCollection<PrescriptionNoteTemplates>();
                            try
                            {
                                allItems = contract.EndPrescriptionNoteTemplates_GetAllIsActive(asyncResult);
                                CachDung = new ObservableCollection<PrescriptionNoteTemplates>(allItems.OrderBy(x => x.NoteDetails));
                                NotifyOfPropertyChange(() => CachDung);
                            }
                            catch (FaultException<AxException> fault)
                            {
                                ClientLoggerHelper.LogInfo(fault.ToString());
                            }
                            catch (Exception ex)
                            {
                                ClientLoggerHelper.LogInfo(ex.ToString());
                            }

                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                }
                finally
                {
                    IsWaitingPrescriptionNoteTemplates_GetAll = false;
                }
            });
            t.Start();
        }


        #endregion

        public void RefreshLookup()
        {
            PrescriptionNoteTemplates_GetAllIsActive();
            PrescriptionNoteTemplates_GetAllIsActiveItem();
            PrescriptionNoteTemplates_GetAllIsActiveAdm();
        }

        public void GetToaThuocDaCo()
        {
            btnCreateNewIsEnabled = true;
            btnSaveAddNewIsEnabled = false;

            btnCreateAndCopyIsEnabled = true;
            btnCopyToIsEnabled = true;
            IsEnabledPrint = true;

            //KMx: Khi load toa thuốc thì Enable của form phải bằng false, khi nào người dùng nhấn "Chỉnh sửa" thì Enable mới bằng true.
            // Nếu không có dòng dưới thì sẽ bị sai khi đang "chỉnh sửa" toa cho BN A, chọn BN B từ out standing task, thì toa của BN B tự động Enable (29/05/2014 15:56).
            IsEnabled = false;

            btnEditIsEnabled = true;

            //KMx: Phải để DeepCopy, vì set LatestePrecriptions (cha) = PrescriptionIssueHistories.Prescription (con), rồi lại set LatestePrecriptions.PrescriptionIssueHistory (con) = PrescriptionIssueHistories (cha), dữ liệu bị thay đổi (21/05/2015 17:28).
            LatestePrecriptions = ObjPatientServiceRecord_Current.PrescriptionIssueHistories[0].Prescription.DeepCopy();
            LatestePrecriptions.PrescriptionIssueHistory = ObjPatientServiceRecord_Current.PrescriptionIssueHistories[0].DeepCopy();
            //CMN: Set lại chẩn đoán bằng chẩn đoán đã được xác nhận
            if (ObjPatientServiceRecord_Current != null && ObjPatientServiceRecord_Current.DiagnosisTreatments != null &&
                ObjPatientServiceRecord_Current.DiagnosisTreatments.Any(x => x.ConfirmedForPrescriptID == LatestePrecriptions.PrescriptID))
            {
                CurrentDiagnosisTreatment = ObjPatientServiceRecord_Current.DiagnosisTreatments.First(x => x.ConfirmedForPrescriptID == LatestePrecriptions.PrescriptID);
                ObjDiagnosisTreatment_Current = ObjPatientServiceRecord_Current.DiagnosisTreatments.First(x => x.ConfirmedForPrescriptID == LatestePrecriptions.PrescriptID);
            }
            if (!CheckToaThuocDuocPhepCapNhat())
            {
                btnEditIsEnabled = false;
            }

            //KMx: Nội trú không cần kiểm tra (17/12/2014 17:00).
            //if (LatestePrecriptions.PrescriptionIssueHistory.IssuedDateTime.HasValue)
            //{
            //    ValidateExpiredPrescription(LatestePrecriptions);
            //}

            LatestePrecriptions.PrescriptionIssueHistory.DeptLocID = Globals.DeptLocation.DeptLocationID;


            if (LatestePrecriptions.ObjCreatorStaffID.StaffID != ObjStaff.StaffID)
            {
                LatestePrecriptions.PrescriptionIssueHistory.OrigCreatorDoctorNames += " - " + ObjStaff.FullName;
            }

            //▼===== 20191011 TTM: Loại bỏ Globals.SecretaryLogin
            //if (Globals.SecretaryLogin != null)
            //{
            //    LatestePrecriptions.SecretaryStaff = Globals.SecretaryLogin.Staff;
            //}
            //▲===== 

            PrecriptionsForPrint = LatestePrecriptions;

            ContentKhungTaiKhamIsEnabled = LatestePrecriptions.IsAllowEditNDay;

            if (LatestePrecriptions.NDay > 0)
            {
                chkHasAppointmentValue = true;
            }
            else
            {
                chkHasAppointmentValue = false;
            }
            LatestePrecriptions.CanEdit = true;

            if (LatestePrecriptions.NDay >= 0)
            {
                CtrtbSoTuan.Text = LatestePrecriptions.NDay.ToString();
            }

            GetPrescriptionDetailsByPrescriptID(LatestePrecriptions.PrescriptID);
        }

        //KMx: Hàm này được kết hợp từ 2 hàm GetPrescriptionTypes_New() và GetPrescriptionTypes_DaCo() (17/05/2014 11:15).
        //Lý do: Bỏ bước đi về server lấy Lookup và 2 hàm đó giống nhau.
        private void GetPrescription(bool IsRegDetailHasPrescript)
        {

            InitialNewPrescription();

            SetToaBaoHiem_KhongBaoHiem();

            if (IsRegDetailHasPrescript)
            {
                GetToaThuocDaCo();
            }
            else
            {
                GetLatestPrescriptionByPtID_New(Registration_DataStorage.CurrentPatient.PatientID);
            }

        }


        void deActive_Deactivated(object sender, DeactivationEventArgs e)
        {
            if (e.WasClosed)
            {
                Completed(this, new ResultCompletionEventArgs());
            }
        }

        public object Dialog { get; private set; }

        public event EventHandler<ResultCompletionEventArgs> Completed;

        #region button control

        public enum PrescriptionState
        {
            //Tao moi toa thuoc
            NewPrescriptionState = 1,
            //Hieu chinh toa thuoc voi bac si da ra toa
            EditPrescriptionState = 2,
            //Phat hanh lai toa thuoc neu la voi bac si khac
            //cho chinh sua va mark delete doi voi toa cu
            PublishNewPrescriptionState = 3,
        }


        private PrescriptionState _PrescripState = PrescriptionState.NewPrescriptionState;
        public PrescriptionState PrescripState
        {
            get
            {
                return _PrescripState;
            }
            set
            {
                //if (_PrescripState != value)
                {
                    _PrescripState = value;
                    NotifyOfPropertyChange(() => PrescripState);
                    switch (PrescripState)
                    {
                        case PrescriptionState.NewPrescriptionState:
                            mNewPrescriptionState = true && btnCreateNewVisibility;
                            mEditPrescriptionState = false;
                            mPublishNewPrescriptionState = false;
                            break;
                        case PrescriptionState.EditPrescriptionState:
                            mNewPrescriptionState = false;
                            mEditPrescriptionState = true && btnUpdateVisibility;
                            mPublishNewPrescriptionState = false;
                            break;
                        case PrescriptionState.PublishNewPrescriptionState:
                            mNewPrescriptionState = false;
                            mEditPrescriptionState = false;
                            mPublishNewPrescriptionState = true && btnCopyToVisible;
                            break;
                    }
                }
            }
        }

        private bool _mNewPrescriptionState;
        public bool mNewPrescriptionState
        {
            get
            {
                return _mNewPrescriptionState;
            }
            set
            {
                if (_mNewPrescriptionState != value)
                {
                    _mNewPrescriptionState = value;
                    NotifyOfPropertyChange(() => mNewPrescriptionState);
                }
            }
        }

        private bool _mEditPrescriptionState;
        public bool mEditPrescriptionState
        {
            get
            {
                return _mEditPrescriptionState;
            }
            set
            {
                if (_mEditPrescriptionState != value)
                {
                    _mEditPrescriptionState = value;
                    NotifyOfPropertyChange(() => mEditPrescriptionState);
                }
            }
        }

        private bool _mPublishNewPrescriptionState;
        public bool mPublishNewPrescriptionState
        {
            get
            {
                return _mPublishNewPrescriptionState;
            }
            set
            {
                if (_mPublishNewPrescriptionState != value)
                {
                    _mPublishNewPrescriptionState = value;
                    NotifyOfPropertyChange(() => mPublishNewPrescriptionState);
                }
            }
        }
        #endregion
        private IDepartmentListing _departmentContent;
        public IDepartmentListing DepartmentContent
        {
            get { return _departmentContent; }
            set
            {
                _departmentContent = value;
                NotifyOfPropertyChange(() => DepartmentContent);
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
                UCAllergiesWarningByPatientID.Registration_DataStorage = Registration_DataStorage;
            }
        }
        private bool _UseOnlyDailyDiagnosis = Globals.ServerConfigSection.ConsultationElements.UseOnlyDailyDiagnosis;
        public bool UseOnlyDailyDiagnosis
        {
            get
            {
                return _UseOnlyDailyDiagnosis;
            }
            set
            {
                if (_UseOnlyDailyDiagnosis == value)
                {
                    return;
                }
                _UseOnlyDailyDiagnosis = value;
                NotifyOfPropertyChange(() => UseOnlyDailyDiagnosis);
            }
        }
        private DiagnosisTreatment CurrentDiagnosisTreatment { get; set; }
        private IList<DiagnosisTreatment> DiagnosisTreatmentCollection { get; set; }
        public void ConfirmDiagnosisTreatmentCmd()
        {
            if (Registration_DataStorage != null
                    && Registration_DataStorage.CurrentPatientRegistration != null
                    && Registration_DataStorage.CurrentPatientRegistration.AdmissionInfo != null
                    && Registration_DataStorage.CurrentPatientRegistration.AdmissionInfo.DischargeDetailRecCreatedDate != null
                    && Registration_DataStorage.CurrentPatientRegistration.AdmissionInfo.DischargeDetailRecCreatedDate != DateTime.MinValue)
            {
                MessageBox.Show(eHCMSResources.Z3052_G1_DaXuatVienKhongTheXacNhanCD);
                return;
            }
            //CurrentDiagnosisTreatment = null;
            IConfirmDiagnosisTreatment DialogView = Globals.GetViewModel<IConfirmDiagnosisTreatment>();
            DialogView.ApplyDiagnosisTreatmentCollection(DiagnosisTreatmentCollection);
            //20191104 TBL: Lấy khoa đang ra toa thuốc
            if (LatestePrecriptions != null && LatestePrecriptions.Department != null)
            {
                //▼===== 20200113 TTM: Đưa Linq thành câu bình thường vì Linq sử dụng First mà còn . lấy dữ liệu khi mà Linq không có giá trị thoả điều kiện => Crash chương trình.
                //DialogView.DeptID = Registration_DataStorage.CurrentPatientRegistration.AdmissionInfo.InPatientDeptDetails.Where(x => !x.IsTemp && x.V_InPatientDeptStatus == AllLookupValues.InPatientDeptStatus.NHAP_KHOA_PHONG).First().DeptLocation.RefDepartment.DeptID;
                foreach (var detail in Registration_DataStorage.CurrentPatientRegistration.AdmissionInfo.InPatientDeptDetails)
                {
                    if (!detail.IsTemp && detail.V_InPatientDeptStatus == AllLookupValues.InPatientDeptStatus.NHAP_KHOA_PHONG)
                    {
                        DialogView.DeptID = detail.DeptLocation.RefDepartment.DeptID;
                    }
                }
                if (DialogView.DeptID == 0)
                {
                    MessageBox.Show(eHCMSResources.Z2966_G1_TuChoiXacNhanChanDoan);
                    return;
                }
                //▲===== 
            }
            //20191115 TBL: Chỉ truyền cờ này khi là màn hình Toa thuốc xuất viện hoặc Xuất viện BN
            DialogView.IsPreAndDischargeView = true;
            GlobalsNAV.ShowDialog_V3(DialogView, null, null, false, true, Globals.GetDefaultDialogViewSize());
            //20200418 TBL: BM 0032140: Nếu xác nhận chẩn đoán không có chọn chẩn đoán thì không cần set lại, nếu không khi cập nhật toa thuốc sẽ báo chưa có chẩn đoán
            if (DialogView.CurrentDiagnosisTreatment != null)
            {
                CurrentDiagnosisTreatment = DialogView.CurrentDiagnosisTreatment;
            }
            //CMN: Cập nhật giá trị chẩn đoán xác nhận lên thông tin toa thuốc
            if (LatestePrecriptions.ObjDiagnosisTreatment == null)
            {
                LatestePrecriptions.ObjDiagnosisTreatment = new DiagnosisTreatment();
            }
            if (CurrentDiagnosisTreatment != null)
            {
                LatestePrecriptions.ObjDiagnosisTreatment.DiagnosisFinal = CurrentDiagnosisTreatment.DiagnosisFinal;
                LatestePrecriptions.ObjDiagnosisTreatment.Diagnosis = string.IsNullOrEmpty(CurrentDiagnosisTreatment.DiagnosisFinal) ? CurrentDiagnosisTreatment.Diagnosis.Trim() : CurrentDiagnosisTreatment.DiagnosisFinal.Trim();
                LatestePrecriptions.Diagnosis = LatestePrecriptions.ObjDiagnosisTreatment.Diagnosis;
                LatestePrecriptions.ObjDiagnosisTreatment.Treatment = CurrentDiagnosisTreatment.Treatment;
                LatestePrecriptions.ObjDiagnosisTreatment.DeptLocationID = CurrentDiagnosisTreatment.DeptLocationID;
                LatestePrecriptions.ObjDiagnosisTreatment.Department = CurrentDiagnosisTreatment.Department;
            }
        }
        /*▼====: #003*/
        public void ConfirmAgainDiagnosisTreatmentCmd()
        {
            CurrentDiagnosisTreatment = null;
            IConfirmDiagnosisTreatment DialogView = Globals.GetViewModel<IConfirmDiagnosisTreatment>();
            DialogView.ApplyDiagnosisTreatmentCollection(DiagnosisTreatmentCollection);
            //20191104 TBL: Lấy khoa đang ra toa thuốc
            if (LatestePrecriptions != null && LatestePrecriptions.Department != null)
            {
                DialogView.DeptID = Registration_DataStorage.CurrentPatientRegistration.AdmissionInfo.InPatientDeptDetails.Where(x => !x.IsTemp && x.V_InPatientDeptStatus == AllLookupValues.InPatientDeptStatus.NHAP_KHOA_PHONG).First().DeptLocation.RefDepartment.DeptID;
            }
            //20191115 TBL: Chỉ truyền cờ này khi là màn hình Toa thuốc xuất viện hoặc Xuất viện BN
            DialogView.IsPreAndDischargeView = true;
            DialogView.IsConfirmAgainDiagnosisTreatment = true;
            GlobalsNAV.ShowDialog_V3(DialogView, null, null, false, true, new Size(1600, 600));
            if (DialogView.CurrentDiagnosisTreatment != null)
            {
                CurrentDiagnosisTreatment = DialogView.CurrentDiagnosisTreatment;
            }
            //CMN: Cập nhật giá trị chẩn đoán xác nhận lên thông tin toa thuốc
            if (LatestePrecriptions.ObjDiagnosisTreatment == null)
            {
                LatestePrecriptions.ObjDiagnosisTreatment = new DiagnosisTreatment();
            }
            if (CurrentDiagnosisTreatment != null)
            {
                LatestePrecriptions.ObjDiagnosisTreatment.DiagnosisFinal = CurrentDiagnosisTreatment.DiagnosisFinal;
                LatestePrecriptions.ObjDiagnosisTreatment.Diagnosis = string.IsNullOrEmpty(CurrentDiagnosisTreatment.DiagnosisFinal) ? CurrentDiagnosisTreatment.Diagnosis.Trim() : CurrentDiagnosisTreatment.DiagnosisFinal.Trim();
                LatestePrecriptions.Diagnosis = LatestePrecriptions.ObjDiagnosisTreatment.Diagnosis;
                LatestePrecriptions.ObjDiagnosisTreatment.Treatment = CurrentDiagnosisTreatment.Treatment;
            }
        }
        /*▲====: #003*/
        public void ApplyDiagnosisTreatmentCollection(IList<DiagnosisTreatment> aDiagnosisTreatmentCollection)
        {
            CurrentDiagnosisTreatment = null;
            DiagnosisTreatmentCollection = aDiagnosisTreatmentCollection;
            //▼===== #006
            if (DiagnosisTreatmentCollection.Any(x => x.ConfirmedForPrescriptID > 0
                                || x.V_DiagnosisType == (long)AllLookupValues.V_DiagnosisType.DIAGNOSIS_OUTHOS))
            {
                CurrentDiagnosisTreatment = DiagnosisTreatmentCollection.Where(x => x.ConfirmedForPrescriptID > 0
                                    || x.V_DiagnosisType == (long)AllLookupValues.V_DiagnosisType.DIAGNOSIS_OUTHOS).FirstOrDefault();
            }
            //▲===== #006
        }
        //20191120 TBL: Khi bên màn hình Xuất viện BN được lưu thành công thì chẩn đoán của xuất viện sẽ thành xác nhận chẩn đoán của toa thuốc
        public void Handle(LoadDiagnosisTreatmentConfirmedForDischarge message)
        {
            if (message != null)
            {
                if (LatestePrecriptions.ObjDiagnosisTreatment == null)
                {
                    LatestePrecriptions.ObjDiagnosisTreatment = new DiagnosisTreatment();
                }
                CurrentDiagnosisTreatment = message.DiagnosisTreatment;
                LatestePrecriptions.ObjDiagnosisTreatment.DiagnosisFinal = CurrentDiagnosisTreatment.DiagnosisFinal;
                LatestePrecriptions.ObjDiagnosisTreatment.Diagnosis = string.IsNullOrEmpty(CurrentDiagnosisTreatment.DiagnosisFinal) ? CurrentDiagnosisTreatment.Diagnosis.Trim() : CurrentDiagnosisTreatment.DiagnosisFinal.Trim();
                LatestePrecriptions.Diagnosis = LatestePrecriptions.ObjDiagnosisTreatment.Diagnosis;
                LatestePrecriptions.ObjDiagnosisTreatment.Treatment = CurrentDiagnosisTreatment.Treatment;
            }
        }

        //▼===== #004
        public void SetLastDiagnosisForConfirm()
        {
            if (DiagnosisTreatmentCollection != null && DiagnosisTreatmentCollection.Count > 0 && !DiagnosisTreatmentCollection.Any(x => x.ConfirmedForPrescriptID > 0
                                || x.V_DiagnosisType == (long)AllLookupValues.V_DiagnosisType.DIAGNOSIS_OUTHOS)
                                && (CurrentDiagnosisTreatment == null || CurrentDiagnosisTreatment.DTItemID == 0))
            {
                CurrentDiagnosisTreatment = DiagnosisTreatmentCollection.OrderByDescending(x => x.DTItemID).FirstOrDefault();
                if (LatestePrecriptions.ObjDiagnosisTreatment == null)
                {
                    LatestePrecriptions.ObjDiagnosisTreatment = new DiagnosisTreatment();
                }
                if (CurrentDiagnosisTreatment != null)
                {
                    LatestePrecriptions.ObjDiagnosisTreatment.DiagnosisFinal = CurrentDiagnosisTreatment.DiagnosisFinal;
                    LatestePrecriptions.ObjDiagnosisTreatment.Diagnosis = string.IsNullOrEmpty(CurrentDiagnosisTreatment.DiagnosisFinal) ? CurrentDiagnosisTreatment.Diagnosis.Trim() : CurrentDiagnosisTreatment.DiagnosisFinal.Trim();
                    LatestePrecriptions.Diagnosis = LatestePrecriptions.ObjDiagnosisTreatment.Diagnosis;
                    LatestePrecriptions.ObjDiagnosisTreatment.Treatment = CurrentDiagnosisTreatment.Treatment;
                }
            }
        }
        //▲===== #004

        //▼==== #008
        public void Handle(OnChangedUpdateAdmDisDetails message)
        {
            if (message != null && message.PrescriptionDoctorAdvice != null)
            {
                if (LatestePrecriptions != null)
                {
                    LatestePrecriptions.DoctorAdvice = message.PrescriptionDoctorAdvice;
                }
            }
        }
        //▲==== #008
        //▼==== #012
        private ObservableCollection<Lookup> _ListV_ReconmendTimeUsageDistance;
        public ObservableCollection<Lookup> ListV_ReconmendTimeUsageDistance
        {
            get { return _ListV_ReconmendTimeUsageDistance; }
            set
            {
                _ListV_ReconmendTimeUsageDistance = value;
                NotifyOfPropertyChange(() => ListV_ReconmendTimeUsageDistance);
            }
        }
        public void GetRouteOfAdministrationList(PrescriptionDetail refGen, long DrugROAID, long DrugID)
        {
            
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyGenMedDeptDetailsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetRouteOfAdministrationList(DrugROAID, DrugID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndGetRouteOfAdministrationList(asyncResult);
                            if (results != null)
                            {
                                refGen.ListRouteOfAdministration = new ObservableCollection<Lookup>();
                                var obsRMCT = new ObservableCollection<Lookup>();
                                foreach (var p in results)
                                {
                                    if (p.RouteOfAdministration == null)
                                    {
                                        p.RouteOfAdministration = new Lookup();
                                    }
                                    //p.RefMedicalConditionType.IsWarning = p.IsWarning;
                                    obsRMCT.Add(p.RouteOfAdministration);
                                }
                                if(obsRMCT.Count > 0)
                                {
                                    refGen.ListRouteOfAdministration = obsRMCT;
                                } 
                                if (refGen.V_RouteOfAdministration == 0 && refGen.ListRouteOfAdministration.Count >0)
                                {
                                    refGen.V_RouteOfAdministration = refGen.ListRouteOfAdministration.FirstOrDefault().LookupID;
                                }
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
        public void RouteOfAdministrationLoaded(object DataContext)
        {
            
        }
        //▲==== #012

        //▼==== #013
        public bool IsShowPreviewPsychotropicPrescription = Globals.ServerConfigSection.ConsultationElements.IsSeparatePsychotropicPrescription;
        public bool IsAddictive = false;
        public bool IsPsychotropicDrugs = false;
        public void btnPreview_GN_HT()
        {
            if (PrecriptionsForPrint.IssueID <= 0)
            {
                MessageBox.Show(eHCMSResources.Z2300_G1_KhongCoToaThuocDeXemIn, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
            foreach (var PsychotropicDrugs in LatestePrecriptions.PrescriptionDetails)
            {
                if (PsychotropicDrugs.RefGenDrugCatID_1 == 3)
                {
                    IsAddictive = true;
                    break;
                }
            }

            foreach (var PsychotropicDrugs in LatestePrecriptions.PrescriptionDetails)
            {
                if (PsychotropicDrugs.RefGenDrugCatID_1 == 2)
                {
                    IsPsychotropicDrugs = true;
                    break;
                }
            }

            if (IsPsychotropicDrugs)
            {
                Action<ICommonPreviewView> onInitDlg = delegate (ICommonPreviewView proAlloc)
                {
                    proAlloc.IssueID = PrecriptionsForPrint.IssueID;
                    proAlloc.IsPsychotropicDrugs = IsPsychotropicDrugs;
                    proAlloc.IsAddictive = false;
                    proAlloc.eItem = ReportName.CONSULTATION_TOATHUOC_INPT_GN_HT;
                    proAlloc.parTypeOfForm = Globals.ServerConfigSection.ConsultationElements.IsSeparatePsychotropicPrescription_Inpt == true ? 1 : 0;
                    if (LatestePrecriptions == null || LatestePrecriptions.PrescriptionDetails == null || LatestePrecriptions.PrescriptionDetails.Count == 0)
                        proAlloc.parTypeOfForm = 0;
                };
                GlobalsNAV.ShowDialog(onInitDlg);
            }
            if (IsAddictive)
            {
                Action<ICommonPreviewView> onInitDlg = delegate (ICommonPreviewView proAlloc)
                {
                    proAlloc.IssueID = PrecriptionsForPrint.IssueID;
                    proAlloc.IsPsychotropicDrugs = false;
                    proAlloc.IsAddictive = IsAddictive;
                    proAlloc.eItem = ReportName.CONSULTATION_TOATHUOC_INPT_GN_HT;
                    proAlloc.parTypeOfForm = Globals.ServerConfigSection.ConsultationElements.IsSeparatePsychotropicPrescription_Inpt == true ? 1 : 0;
                    if (LatestePrecriptions == null || LatestePrecriptions.PrescriptionDetails == null || LatestePrecriptions.PrescriptionDetails.Count == 0)
                        proAlloc.parTypeOfForm = 0;
                };
                GlobalsNAV.ShowDialog(onInitDlg);
            }

            IsPsychotropicDrugs = false;
            IsAddictive = false;
        }
        //▲==== #013
    }
}
