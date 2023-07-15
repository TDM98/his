using System.Linq;
using System.ComponentModel.Composition;
using Caliburn.Micro;
using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using DataEntities;
using System.Threading;
using System;
using System.Text;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using eHCMSLanguage;
using System.ComponentModel;
using aEMR.ViewContracts;
using aEMR.Common.BaseModel;
using aEMR.Infrastructure.Events;
using Castle.Windsor;
using aEMR.Infrastructure.CachingUtils;
using aEMR.Infrastructure;
using aEMR.Common.Collections;
using aEMR.Common.Views;
using aEMR.CommonTasks;
using aEMR.ServiceClient;
using aEMR.ServiceClient.Consultation_PCLs;
using aEMR.Controls;
using aEMR.Common.Printing;
using aEMR.ViewContracts.Configuration;
using Service.Core.Common;
/*
* 20171220 #001 CMN:    Fixed Print Empty Prescriptions
* 20180914 #002 TTM: 
* 20180919 #003 TTM: 
* 20180920 #004 TBL:    Chinh sua chuc nang bug mantis ID 0000061, thay doi thuoc tinh IsObjectBeingUsedByClient, IsDataChanged, IsPrescriptionChanged
* 20180921 #005 TBL:    Khong the xem in toa thuoc mau
* 20180922 #006 TBL:    BM 0000073. Them listICD10Codes vao BeginGetDrugsInTreatmentRegimen
* 20180924 #007 TBL:    BM 0000077. Tao toa thuoc moi dua tren cu khi luu thanh cong thi khong dc chinh sua
* 20180924 #008 TTM:
* 20180927 #009 TBL:    BM 0000095. In toa thuoc theo so lan. Neu co loi gi lien quan thi can phai coi lai
* 20180210 #010 TTM:    BM 0000112: Fix việc popup hẹn bệnh hiện lên khi chỉ lưu chẩn đoán chưa có đụng chạm gì đến toa thuốc. 
* 20181009 #011 TBL:    BM 0000115: Fix lại vấn đề khi muốn lưu CĐ thì lại báo chưa chọn thuốc để ra toa
* 20181016 #012 TBL:    BM 0002170: Tu lay thuoc theo phac do khi qua tab Ra toa
* 20181029 #013 TTM:    BM 0004199: Thêm cảnh báo nếu bác sĩ ra toa thuốc có thuốc ngoài danh mục cần phải có sự xác nhận của bệnh nhân
* 20181101 #014 TTM:    BM 0004220: Fix lỗi query liên tục mặc dù không có gì để in (Toa hướng thần) và bổ sung thêm toa TPCN/MP
* 20181112 #015 TBL:    BM 0005204: Lay phac do dieu tri theo list ICD10
* 20180114 #016 TTM:    BM 0006473: Thêm kiểm tra nếu toa đã được duyệt thì không được phép chỉnh sửa
* 20190613 #017 TNHX:   [BM0006826] Apply config AllowTimeUpdatePrescription for edit prescription
* 20190620 #018 TTM:    BM 0011857: Xây dựng màn hình thông tin thuốc.
* 20190708 #019 TBL:    BM 0011924: Hẹn tự động theo cấu hình AppointmentAuto, ParamAppointmentAuto
* 20190806 #020 TTM:    BM 0011843: Tạo mới dựa trên cũ những bệnh nhân đăng ký không bảo hiểm (trước đó có bảo hiểm) thì vẫn load thuốc trong danh mục bảo hiểm 
*                       => Xem in sai, ra toa sai.
* 20191005 #021 TBL:    BM 0017423: Quản lý hẹn bệnh ngày lễ
* 20191008 #022 TBL:    BM 0017428: Cảnh báo khi toa có thuốc trùng nhóm thuốc
* 20191011 #023 TTM:    BM 0017421: Thêm thư ký y khoa cho ra toa
* 20191011 #024 TBL:    BM 0017452: Trong toa thuốc có thuốc trùng nhau thì hiển thị tên thuốc lên thông báo
* 20191028 #025 TBL:    BM 0018501: Nếu chẩn đoán và toa thuốc không phải 1 cặp thì hiện đỏ xung quanh ngày khám của toa thuốc
* 20191115 #026 TBL:    BM 0019585: Hiển thị danh sách thuốc ngoài phác đồ, ngoài danh mục BHYT khi cảnh báo
* 20191128 #027 TBL:    BM 0019595: Kiểm tra chống chỉ định trong cùng 1 đăng ký
* 20200120 #028 TBL:    BM 0022845: Thay đổi cách kiểm tra phác đồ điều trị
* 20200408 #029 TBL:    BM 0029098: Hiện thông báo toa thuốc cuối được tạo từ nội trú
* 20200616 #030 TBL:    BM 0039265: Không được phép lưu toa thuốc BHYT khi tổng tiền thuốc BH lơn hơn tổng tiền DV BH
* 20200626 #031 TTM:    BM 0039267: Thêm cuộc hẹn điều trị ngoại trú 1 đăng ký.
* 20200803 #032 TTM:   BM 0040439: Sửa lại liều dùng của toa thuốc đang sai.
* 20200902 #033 TTM:   BM 0038177: Về vấn đề thuốc đã bán và tạo toa mới. 
* 20200915 #034 TNHX:  Add parameter for print direct prescriptions
* 20200926 #035 TNHX:  Filter prescription has MaxHIPay was requestes by Dr Vu
* 20210430 #036 TNHX:  hiển thị thêm phiếu tư vấn theo danh sách ICD chính dựa vào cấu hình: ListICDShowAdvisoryVotes
* 20220530 #037 BLQ:  Kiểm tra thời gian thao tác của bác sĩ
* 20220531 #038 BLQ: Không tự tạo toa mới khi bệnh nhân chưa có toa thuóc
* 20220604 #039 QTD:   Không cho chỉnh sửa khi tìm ĐK ngoại trú đã nhập viện
* 20220806 #040 DatTB: [Khám bệnh] Ràng buộc nhập số ngày dùng thuốc
* 20220811 #041 BLQ: Ràng buộc nhập thông tin bệnh nhân khi lưu toa thuốc điều trị ngoại trú
*                   + Kiểm tra theo cấu hình CheckPatientInfoWhenSavePrescript nếu bật thì mới kiểm tra
* 20220818 #042 DatTB: Lấy thêm Số ngày ra toa từ liệu trình.
* 20220831 #043 BLQ: Fix Số ngày ra toa lấy lên luôn = 0
* 20220922 #044 BLQ: Thêm kiểm tra số ngày toa thuốc theo cấu hình
* 20220926 #045 BLQ: Thêm hàm check toa thuốc khi lưu chẩn đoán. Chuyển hàm check chống chỉ định và hàm check số ngày thuốc vào
* 20220928 #046 BLQ: Kiểm tra nhập lời dặn khi lưu toa thuốc
* 20221007 #047 TNHX: Thêm giá trị dvkt cho phép đánh thuốc BH dù không có giá bH cho điều trị dài ngày
* 20221007 #048 TNHX: 2344 Bỏ kiểm tra trần toa thuốc loại 1 + 2 theo cấu hình mã thẻ + bệnh mới trong năm
* 20221008 #049 BLQ: Kiểm tra các trường liên hệ phụ với trẻ nhỏ hơn 72 tháng
* 20221010 #050 BLQ: Kiểm tra trường cmnd đối với toa gây nghiện, hướng thần
* 20221118 #051 QTD: Xoá danh sách toa thuốc khi nhắc CLS
* 20230527 #052 BLQ: Bỏ cảnh báo thuốc mua ngoài danh mục. Tự động tích toa có thuốc mua ngoài khi check có thuốc mua ngoài
* 20230629 #053 BLQ: Kiểm tra lời dặn có ký tự '<' và '>'
*/
namespace aEMR.Common.ViewModels
{
    [Export(typeof(IePrescriptionOldNew)), PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class ePrescriptionOldNewViewModel : ViewModelBase, IePrescriptionOldNew
        , IHandle<ePrescriptionDoubleClickEvent>
        , IHandle<ReloadDataePrescriptionEvent>
        //, IHandle<SendPrescriptionDetailSchedulesEvent<ObservableCollection<PrescriptionDetailSchedules>, bool, double, double, string, int>>
        //, IHandle<SendPrescriptionDetailSchedulesFormEvent<ObservableCollection<PrescriptionDetailSchedules>, bool, double, double, string, int>>
        , IHandle<DiagnosisTreatmentSelectedEvent<DiagnosisTreatment>>
        , IHandle<DuocSi_EditingToaThuocEvent>
        , IHandle<SelectListDrugDoubleClickEvent>
        , IHandle<PrescriptionDrugNotInCatSelectedEvent<PrescriptionDetail, int>>
        , IHandle<PrescriptionNoteTempType_Change>
        , IHandle<PatientChange>
        //, IHandle<ShowPatientInfo_KHAMBENH_RATOA<Patient, PatientRegistration, PatientRegistrationDetail>>
        , IHandle<GlobalCurPatientServiceRecordLoadComplete_EPrescript>
        , IHandle<LoadPrescriptionAfterSaved>
        , IHandle<CheckV_TreatmentType>
        , IHandle<ConfirmOutPtTreatmentProgram>
        //▼==== #041
        , IHandle<UpdateCompleted<Patient>>
        //▲==== #041
        //▼==== #043
        , IHandle<SaveOutPtTreatmentProgramItem>
        //▲==== #043
        //▼==== #051
        , IHandle<PCLExamAccordingICD_Event>
        //▲==== #051

    {
        [ImportingConstructor]
        public ePrescriptionOldNewViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg, ISalePosCaching salePosCaching)
        {
            System.Diagnostics.Debug.WriteLine("========> ePrescriptionOldNewViewModel - Constructor");
            UCAllergiesWarningByPatientID = Globals.GetViewModel<IAllergiesWarning_ByPatientID>();
        }
        //==== 20161115 CMN Begin: Fix Choose reload handle
        protected override void OnActivate()
        {
            base.OnActivate();
            Globals.EventAggregator.Subscribe(this);
            GetInitDataInfo();
            GetAllHoliday();
            //GetAll
            //Globals.EventAggregator.Subscribe(this);
        }
        //==== 20161115 End.
        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);
            Globals.EventAggregator.Unsubscribe(this);
        }
        private void GetInitDataInfo()
        {
            this.ShowBusyIndicator();
            authorization();
            DrugTypes = new ObservableCollection<Lookup>();
            ChooseDoses = new ObservableCollection<ChooseDose>();
            LatestePrecriptions = new Prescription();
            LatestePrecriptions.PropertyChanged += LatestePrecriptions_PropertyChanged;
            GetAllDrugsContrainIndicator();
            GetAllRefGenericRelation();
            ObjPrescriptionNoteTemplates_GetAll = new ObservableCollection<PrescriptionNoteTemplates>();
            LieuDung = Globals.allPrescriptionDetailSchedulesLieuDung.ToObservableCollection();
            RefreshLookup();
            //GetAllLookupForDrugTypes();
            DrugTypes = Globals.AllLookupValueList.Where(x => x.ObjectTypeID == (long)LookupValues.V_DrugType).ToObservableCollection();

            Coroutine.BeginExecute(DoGetStore_EXTERNAL());

            DrugList = new PagedSortableCollectionView<GetDrugForSellVisitor>();
            DrugList.PageSize = 10;
            DrugList.OnRefresh += new EventHandler<aEMR.Common.Collections.RefreshEventArgs>(DrugList_OnRefresh);

            InitPatientInfo();

            this.HideBusyIndicator();
        }

        ~ePrescriptionOldNewViewModel()
        {
            System.Diagnostics.Debug.WriteLine("========> ePrescriptionOldNewViewModel - Destructor");
        }

        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);

            if (this.GetView() != null)
            {
                ePrescriptionOldNewView thisView = (ePrescriptionOldNewView)view;
                if (thisView.tbSoTuan != null)
                {
                    CtrtbSoTuan = thisView.tbSoTuan;
                }
            }
            // TxD 22/09/2018 The following error msg is ONLY display when this ViewModel is USED in TAB of the new VM ConsultationSummary
            //                  COMMENT it OUT for now if something HAPPENS then TO BE REVIEWED
            //else
            //{
            //    MessageBox.Show(eHCMSResources.A0545_G1_Msg_InfoEnglish3, eHCMSResources.G0449_G1_TBaoLoi, MessageBoxButton.OK);
            //}
        }


        void LatestePrecriptions_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "PrescriptionDetails")
            {
            }
        }

        //KMx: Sau khi kiểm tra, thấy Handle này không còn sử dụng nữa.
        //public void Handle(ShowPatientInfo_KHAMBENH_RATOA<Patient, PatientRegistration, PatientRegistrationDetail> message)
        //{
        //    //initPatientInfo();
        //}

        //KMx: Khi đứng ở MÀN HÌNH RA TOA, chọn 1 bệnh nhân từ Out Standing Task hoặc từ pop-up tìm đăng ký thì sẽ bắn event về đây.
        public void Handle(GlobalCurPatientServiceRecordLoadComplete_EPrescript message)
        {
            if (message.bJustCallAllowModifyPrescription)
            {
                AllowModifyPrescription();
            }
            else
            {
                InitPatientInfo();
            }
        }

        public void AllowModifyPrescription()
        {
            CheckBeforePrescrip();
        }
        private bool _IsViewOnly = false;
        public bool IsViewOnly
        {
            get => _IsViewOnly; set
            {
                _IsViewOnly = value;
                NotifyOfPropertyChange(() => IsViewOnly);
            }
        }
        public void CheckBeforePrescrip(long PrescriptID = 0)
        {
            listICD10Codes.Clear(); //TBL: Load dang ky moi thi clear
            this.ShowBusyIndicator();
            if (Registration_DataStorage != null && Registration_DataStorage.CurrentPatientRegistration != null && Globals.IsLockRegistration(Registration_DataStorage.CurrentPatientRegistration.RegLockFlag, "hiệu chỉnh toa thuốc", false))
            {
                //IsEnabledForm = false;
                HasDiagnosis = false;
                ObjDiagnosisTreatment_Current = new DiagnosisTreatment();
                btChonChanDoanIsEnabled = false;
                btnSaveAddNewIsEnabled = false;
                IsEnabledPrint = false;
                btnEditIsEnabled = false;
            }
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

            if (Globals.PatientFindBy_ForConsultation != null && Globals.PatientFindBy_ForConsultation.Value != AllLookupValues.PatientFindBy.NGOAITRU)
            {
                IsEnabledForm = false;
                MessageBox.Show(string.Format("{0}!", eHCMSResources.A0245_G1_Msg_InfoKhongPhaiNgTru_ChiXem), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                this.HideBusyIndicator();
                return;
            }

            allPrescriptionIssueHistory = Registration_DataStorage.PrescriptionIssueHistories;

            if (Registration_DataStorage.PatientServiceRecordCollection == null || Registration_DataStorage.PatientServiceRecordCollection.Count < 1) // Chưa có chẩn đoán            
            {
                if (IsShowSummaryContent)
                {
                    IsEnabledForm = false;
                    HasDiagnosis = false;

                    btChonChanDoanIsEnabled = false;
                    btnSaveAddNewIsEnabled = false;
                    IsEnabledPrint = false;
                }
                ObjDiagnosisTreatment_Current = new DiagnosisTreatment();

                //if (Globals.PatientAllDetails.RegistrationDetailInfo.PaidTime.Value.AddHours(Globals.EffectedDiagHours)
                //       < Globals.ServerDate.Value)

                // Txd 25/05/2014 Replaced ConfigList
                if (Registration_DataStorage != null && Registration_DataStorage.CurrentPatientRegistrationDetail != null && ((Registration_DataStorage.CurrentPatientRegistrationDetail.RefMedicalServiceItem == null || Registration_DataStorage.CurrentPatientRegistrationDetail.RefMedicalServiceItem.IsAllowToPayAfter == 0)
                    && ((Registration_DataStorage.CurrentPatientRegistrationDetail.PaidTime.HasValue && Registration_DataStorage.CurrentPatientRegistrationDetail.PaidTime != null && Registration_DataStorage.CurrentPatientRegistrationDetail.PaidTime.Value.AddHours(Globals.ServerConfigSection.Hospitals.EffectedDiagHours) < Globals.ServerDate.Value)
                        || ((!Registration_DataStorage.CurrentPatientRegistrationDetail.PaidTime.HasValue || Registration_DataStorage.CurrentPatientRegistrationDetail.PaidTime == null) && Registration_DataStorage.CurrentPatientRegistrationDetail.CreatedDate != null && Registration_DataStorage.CurrentPatientRegistrationDetail.CreatedDate.AddHours(Globals.ServerConfigSection.Hospitals.EffectedDiagHours) < Globals.ServerDate.Value)))
                        && !IsViewOnly)
                {
                    MessageBox.Show(string.Format(eHCMSResources.Z0414_G1_DKHetHieuLuc, Globals.ServerConfigSection.Hospitals.EffectedDiagHours.ToString()));
                    this.HideBusyIndicator();
                    return;
                }

                if (IsShowSummaryContent && !IsViewOnly)
                {
                    MessageBox.Show(eHCMSResources.A0403_G1_Msg_InfoChuaCoCDChoDK, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);

                    this.HideBusyIndicator();
                    return;
                }
            }
            if (IsShowSummaryContent && !IsViewOnly)
            {
                ObjDiagnosisTreatment_Current = Registration_DataStorage.PatientServiceRecordCollection[0].DiagnosisTreatments[0];
            }
            else if (Registration_DataStorage.PatientServiceRecordCollection != null)
            {
                if (Registration_DataStorage.PatientServiceRecordCollection.Count == 0)
                {
                    //btnCreateNew();
                    //Gọi vào hàm CreateNew vì nút tạo mới chờ kiểm tra cls chưa thực hiện nên CS_DS.DiagTreatment.ServiceRecID có giá trị sẽ thông báo chưa tạo chẩn đoán
                    // đăt IsHavePCLRequestUnfinished = false khi tìm bệnh nhân pass qua block kiểm tra cls chưa thực hiện để lấy toa thuốc cũ
                    IsHavePCLRequestUnfinished = false;
                    CreateNewFunc();
                }
                else
                    ObjDiagnosisTreatment_Current = Registration_DataStorage.PatientServiceRecordCollection[0].DiagnosisTreatments[0];
            }

            //HasDiagnosis = true;

            // TxD 11/04/2014 ToBeChecked the following and may be removed altogether 
            //                  because Globals.ObjGetDiagnosisTreatmentByPtID might have been initialised elsewhere already should not be reassigned here.

            //Globals.ObjGetDiagnosisTreatmentByPtID = ObjectCopier.DeepCopy(ObjDiagnosisTreatment_Current);

            // TxD 11/04/2014 Changed from HisIDVisibility to RegistrationCoverByHI
            if (RegistrationCoverByHI())    //neu dang ky co bao hiem
            {
                isHisID = true; //  mặc định toa có bảo hiểm
                LatestePrecriptions.PrescriptionIssueHistory.HisID = Registration_DataStorage.CurrentPatientRegistration.HisID.Value;
            }

            btnUndoIsEnabled = false;
            btnUpdateIsEnabled = false;

            //if (!IsShowSummaryContent && Registration_DataStorage.PatientServiceRecordCollection.Count == 0)
            //{
            //    PrescripState = PrescriptionState.NewPrescriptionState;
            //    GetPrescription(false);
            //}

            //chưa có toa thuốc
            if (IsViewOnly)
            {
                GetPrescription(true, PrescriptID);
            }
            else if (((!IsShowSummaryContent && Registration_DataStorage.PatientServiceRecordCollection != null && Registration_DataStorage.PatientServiceRecordCollection.Count == 0)) || Registration_DataStorage.PatientServiceRecordCollection == null || Registration_DataStorage.PatientServiceRecordCollection[0].PrescriptionIssueHistories == null || Registration_DataStorage.PatientServiceRecordCollection[0].PrescriptionIssueHistories.Count < 1)
            {
                PrescripState = PrescriptionState.NewPrescriptionState;
                //Đọc Toa Thuốc Cuối lên nếu có
                //TBL: Cho nay se chan viec load toa thuoc lai nhieu lan khi ma khong lam viec gi lien quan den toa thuoc
                GetPrescription(false);
            }
            else//đã có toa thuốc
            {
                //IsEnabledForm = false;

                //KMx: Dời 2 dòng bên dưới lên trên cái if, vì có toa hay không thì Enable của 2 nút này phải bằng false (29/05/2014 16:26).
                //btnUndoIsEnabled = false;
                //btnUpdateIsEnabled = false;

                //GetPrescriptionTypes_DaCo();
                GetPrescription(true, PrescriptID);
                if (!IsUpdateWithoutChangeDoctorIDAndDatetime)
                {
                    if (Registration_DataStorage.PatientServiceRecordCollection[0].PrescriptionIssueHistories[0].IssuerStaffID != Globals.LoggedUserAccount.StaffID)
                    {
                        //Chỉ Có Người Tạo Ra Toa Thuốc này mới được phép Chỉnh Sửa!
                        PrescripState = PrescriptionState.PublishNewPrescriptionState;
                    }
                    else
                    {
                        PrescripState = PrescriptionState.EditPrescriptionState;
                    }
                }
                else
                {
                    PrescripState = PrescriptionState.EditPrescriptionState;
                }
                //▼===== #025
                ModifyBorderExamDate();
                //▲===== #025
            }
            if (Registration_DataStorage.CurrentPatientRegistration.IsAdmission)
            {
                IsEnabledForm = false;
            }
            this.HideBusyIndicator();
        }
        //▼===== #025
        private void ModifyBorderExamDate()
        {
            if (LatestePrecriptions.PtRegistrationID != null && LatestePrecriptions.ServiceRecID != null && ServiceRecIDDiagTrmt != 0 && ServiceRecIDDiagTrmt != LatestePrecriptions.ServiceRecID.GetValueOrDefault() && PtRegistrationID != LatestePrecriptions.PtRegistrationID.GetValueOrDefault())
            {
                CheckDiagTrmtAndPrescription = 2;
            }
            else
            {
                CheckDiagTrmtAndPrescription = 0;
            }
        }
        //▲===== #025
        public void LoadPrescriptionInfo(long PrescriptID = 0)
        {
            //Kiem Tra Đăng ký có BH?
            this.ShowBusyIndicator();
            HisIDVisibility = RegistrationCoverByHI();

            //Kiem tra trang thai toa thuoc
            LatestePrecriptions = new Prescription();
            LatestePrecriptions.PropertyChanged += LatestePrecriptions_PropertyChanged;

            //if (Globals.ConfigList != null)
            //{
            //    xNgayBHToiDa_NgoaiTru = Convert.ToInt32(Globals.ConfigList[(int)AppConfigKeys.ConfigItemKey.PharmacyMaxDaysHIRebate_NgoaiTru]);
            //    xNgayBHToiDa_NoiTru = Convert.ToInt32(Globals.ConfigList[(int)AppConfigKeys.ConfigItemKey.PharmacyMaxDaysHIRebate_NoiTru]);
            //}

            // Txd 25/05/2014 Replaced ConfigList
            xNgayBHToiDa_NgoaiTru = Globals.ServerConfigSection.HealthInsurances.PharmacyMaxDaysHIRebate_NgoaiTru;
            xNgayBHToiDa_NoiTru = Globals.ServerConfigSection.HealthInsurances.PharmacyMaxDaysHIRebate_NoiTru;

            #region cu
            if (Registration_DataStorage.CurrentPatient != null)
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
                    //GetDiagnosisTreatmentByPtID(Registration_DataStorage.CurrentPatient.PatientID, Globals.PatientAllDetails.RegistrationInfo.PtRegistrationID, "", 1, true);
                    //CheckBeforePrescrip();
                    Coroutine.BeginExecute(InitPrescrip(PrescriptID));
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
        public void InitPatientInfo()
        {
            LoadPrescriptionInfo();
        }
        private void GetAllPrescription_TrongNgay()
        {
            //20190427 TBL: Moi lan thay doi benh nhan thi clear danh sach chi tiet toa thuoc trong ngay
            ListPrescriptionDetailTrongNgay = new List<IList<PrescriptionDetail>>();
            GetAllPrescription_TrongNgay_ByPatientID(Registration_DataStorage.CurrentPatient.PatientID);
        }
        private IEnumerator<IResult> InitPrescrip(long PrescriptID = 0)
        {
            if (Registration_DataStorage.CurrentPatientRegistrationDetail != null && listICD10Codes != null && listICD10Codes.Count > 0) //TBL: Neu la dang ky moi chua co ICD10 thi khong di load thuoc theo phac do
            {
                yield return GenericCoRoutineTask.StartTask(GetDrugsInTreatmentRegimen, Registration_DataStorage.CurrentPatientRegistrationDetail.PtRegDetailID);
                //▼===== #028
                yield return GenericCoRoutineTask.StartTask(LoadTreatmentRegimenCollection, Registration_DataStorage.CurrentPatientRegistrationDetail.PtRegDetailID);
                //▲===== #028
            }
            IsShowValidateExpiredPrescription = false; //20191021 TBL: Task #1104: Set lại IsShowValidateExpiredPrescription = false để ValidateExpiredPrescription chỉ hiện thông báo 1 lần
            CheckBeforePrescrip(PrescriptID);
            GetAllPrescription_TrongNgay();
            GetAllPrescription_BaoHiemConThuoc();
            GetRemainingMedicineDays();
            //▼===== #027
            LstDiagICD10Items_InDay = new List<DiagnosisIcd10Items>();
            //TBL: Lấy tất cả các ICD10Code trong ngày của đăng ký nhưng không lấy của dịch vụ hiện tại để không bị trùng khi thông báo
            long ServiceRecIDLocal = 0;
            if (Registration_DataStorage.PatientServiceRecordCollection != null && Registration_DataStorage.PatientServiceRecordCollection.Count > 0)
            {
                ServiceRecIDLocal = Registration_DataStorage.PatientServiceRecordCollection.First(x => x.PtRegDetailID == Registration_DataStorage.CurrentPatientRegistrationDetail.PtRegDetailID).ServiceRecID;
            }
            GetAllDiagnosisIcd10Items_InDay(Registration_DataStorage.CurrentPatient.PatientID, ServiceRecIDLocal, Registration_DataStorage.CurrentPatientRegistrationDetail.PtRegDetailID);
            //▼===== #030
            //Lấy tổng tiền BH các dịch vụ của đăng ký
            if (Globals.ServerConfigSection.ConsultationElements.PercentPrescriptionForHI > 0 && Registration_DataStorage.CurrentPatientRegistrationDetail.HIBenefit.GetValueOrDefault() > 0)
            {
                GetTotalHIPaymentForRegistration(Registration_DataStorage.CurrentPatientRegistration.PtRegistrationID);
            }
            //▲===== #030
            //▲===== #027
        }
        //====: #006
        public List<string> listICD10Codes = new List<string>();
        public void GetPhacDo()
        {
            if (Registration_DataStorage.CurrentPatientRegistration == null || Registration_DataStorage.CurrentPatientRegistration.PtRegistrationID == 0)
            {
                return;
            }
            if (CS_DS == null)
            {
                return;
            }
            if (CS_DS.refIDC10List == null || CS_DS.refIDC10List.Count == 0)
            {
                return;
            }
            bool bGetPhacDo = false;
            if (listICD10Codes.Count == 0)
            {
                bGetPhacDo = true;
            }
            else
            {
                foreach (var detail in CS_DS.refIDC10List)
                {
                    if (detail.ICD10Code != null)
                    {
                        bool bNotFoundInList = true;
                        for (int i = 0; i < listICD10Codes.Count; i++)
                        {
                            if (listICD10Codes[i] == detail.ICD10Code)
                            {
                                bNotFoundInList = false;
                                break;
                            }
                        }
                        if (bNotFoundInList) //TBL: chi can 1 dong khac la di lay lai phac do
                        {
                            bGetPhacDo = true;
                            break;
                        }
                    }
                }
            }
            if (bGetPhacDo)
            {
                AddListICD10Codes();
                if (listICD10Codes.Count == 0) //TBL: truong hop BN moi chua co chan doan thi khong the them vao listICD10Codes, nen khong can phai di load
                {
                    return;
                }
                long PtRegDetailID = Registration_DataStorage.CurrentPatientRegistrationDetail.PtRegDetailID;
                GetDrugsInTreatmentRegimen(null, PtRegDetailID);
                //▼===== #028
                LoadTreatmentRegimenCollection(null, PtRegDetailID);
                //▲===== #028
            }
        }
        //====: #006
        private void AddListICD10Codes()
        {
            if (CS_DS != null)
            {
                listICD10Codes.Clear();
                foreach (var detail in CS_DS.refIDC10List)
                {
                    if (detail.ICD10Code != null && detail.ICD10Code.Length > 0)
                    {
                        listICD10Codes.Add(detail.ICD10Code);
                    }
                }
            }
        }

        private void GetDrugsInTreatmentRegimen(GenericCoRoutineTask genTask, object PtRegDetailID)
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ePrescriptionsServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetDrugsInTreatmentRegimen((long)PtRegDetailID, listICD10Codes, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var DrugCollection = contract.EndGetDrugsInTreatmentRegimen(asyncResult);
                                DrugsInTreatmentRegimen = new ObservableCollection<GetDrugForSellVisitor>(DrugCollection);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                            }
                            finally
                            {
                                this.HideBusyIndicator();
                                if (genTask != null)
                                {
                                    genTask.ActionComplete(true);
                                }
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    this.HideBusyIndicator();
                    MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                    if (genTask != null)
                    {
                        genTask.ActionComplete(false);
                    }
                }
            });
            t.Start();
        }
        //▼===== #028
        private void LoadTreatmentRegimenCollection(GenericCoRoutineTask genTask, object PtRegDetailID)
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ePrescriptionsServiceClient())
                    {
                        bool bContinue = true;
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetRefTreatmentRegimensAndDetail((long)PtRegDetailID, listICD10Codes, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                RefTreatmentRegimenCollection = contract.EndGetRefTreatmentRegimensAndDetail(asyncResult).ToObservableCollection();
                                RefTreatmentRegimenDrugDetails = RefTreatmentRegimenCollection.SelectMany(x => x.RefTreatmentRegimenDrugDetails).ToObservableCollection();
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                                bContinue = false;
                            }
                            finally
                            {
                                if (genTask != null)
                                {
                                    genTask.ActionComplete(bContinue);
                                }
                                this.HideBusyIndicator();
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                    ClientLoggerHelper.LogError(ex.Message);
                    genTask.ActionComplete(false);
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }
        //▲===== #028
        #region busy indicator
        public override bool IsProcessing
        {
            get
            {
                return false;
                /*
                return _isWaitingSaveDuocSiEdit
                    || _IsWaitingGetPrescriptionDetailsByPrescriptID
                    || _IsWaitingGetLatestPrescriptionByPtID
                    || _IsWaitingChooseDose
                    || _IsWaitingGetMedConditionByPtID
                    || _IsWaitingAddPrescriptIssueHistory
                    || _IsWaitingCapNhatToaThuoc
                    || _IsWaitingPrescriptions_UpdateDoctorAdvice
                    || _IsWaitingTaoThanhToaMoi
                    || _IsWaitingGetAllContrainIndicatorDrugs
                    || _IsWaitingGetDiagnosisTreatmentByPtID
                    || _IsWaitingPrescriptionNoteTemplates_GetAll;
                 * */
                //|| _loadPrescript;
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
                    return string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.T2837_G1_LoaiToa);
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
        //▼====== #008: khai báo để sử dụng cho việc chuyển dữ liệu từ view con ra view cha (ConsultationSummary).
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
        //▲====== #008

        //KMx: Trường hợp bác sĩ kê toa 10 viên. Bệnh nhân uống 2 viên rồi bị dị ứng. Bệnh nhân yêu cầu bác sĩ kê toa khác.
        //Do bệnh nhân đã uống hết 2 viên, nên không thể trả lại 10 viên đã mua mà chỉ trả lại được 8 viên.
        //Biến này dùng để xác nhận bác sĩ có đồng ý cập nhật toa khi bệnh nhân không trả đủ thuốc hay không.
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

                    NotifyOfPropertyChange(() => CanbtDuocSiEdit);
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
        //▼===== #025
        private long _ServiceRecIDDiagTrmt;
        public long ServiceRecIDDiagTrmt
        {
            get { return _ServiceRecIDDiagTrmt; }
            set
            {
                if (_ServiceRecIDDiagTrmt != value)
                {
                    _ServiceRecIDDiagTrmt = value;
                    NotifyOfPropertyChange(() => ServiceRecIDDiagTrmt);
                    ModifyBorderExamDate();
                }
            }
        }

        private long _PtRegistrationID;
        public long PtRegistrationID
        {
            get { return _PtRegistrationID; }
            set
            {
                if (_PtRegistrationID != value)
                {
                    _PtRegistrationID = value;
                    NotifyOfPropertyChange(() => PtRegistrationID);
                    ModifyBorderExamDate();
                }
            }
        }

        private int _CheckDiagTrmtAndPrescription;
        public int CheckDiagTrmtAndPrescription
        {
            get { return _CheckDiagTrmtAndPrescription; }
            set
            {
                if (_CheckDiagTrmtAndPrescription != value)
                {
                    _CheckDiagTrmtAndPrescription = value;
                    NotifyOfPropertyChange(() => CheckDiagTrmtAndPrescription);
                }
            }
        }
        //▲===== #025
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
        //▼===== 20190620 TTM: Do có bổ sung vào cột thông tin thuốc ở đầu Grid nên cần phải thay đổi lại
        //                     index của các cột. Bổ sung thêm cột INFORMATION. Vì nếu không thay đổi sẽ dẫn đến
        //                     Không cập nhật số lượng khi thay đổi liều và ngày dùng.
        public enum DataGridCol
        {
            INFORMATION = 0,
            DEL = INFORMATION + 1,
            //Down = DEL + 1,
            //Up = Down + 1,
            HI = DEL + 1,
            Schedule = HI + 1,
            //DrugNotInCat = Schedule+1,
            NotInCat = Schedule + 1,
            DRUG_NAME = NotInCat + 1,
            //STRENGHT =DRUG_NAME+ 1,
            UNITS = DRUG_NAME + 1,
            UNITUSE = UNITS + 1,
            DRUG_TYPE = UNITUSE + 1,

            //DOSAGE = DRUG_TYPE + 1,
            //CHOOSE = DOSAGE + 1,

            //▼===== #032
            //MDOSE = DRUG_TYPE + 1,
            //NDOSE = MDOSE + 1,
            //ADOSE = NDOSE + 1,
            //EDOSE = ADOSE + 1,

            MDOSE = DRUG_TYPE + 1,
            ADOSE = MDOSE + 1,
            EDOSE = ADOSE + 1,
            NDOSE = EDOSE + 1,
            //▲===== #032

            DayTotalCol = NDOSE + 1,
            //DaytExtended = Dayts+1,

            QTY = DayTotalCol + 1,
            //UNITUSE = QTY+1,
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
        /*▼====: #009*/
        private bool _Print1 = false;
        public bool Print1
        {
            get
            {
                return _Print1;
            }
            set
            {
                _Print1 = value;
                if (_Print1)
                {
                    Print2 = !_Print1;
                    Print3 = !_Print1;
                    Print4 = !_Print1;
                }
                NotifyOfPropertyChange(() => Print1);
            }
        }
        private bool _Print2 = false;
        public bool Print2
        {
            get
            {
                return _Print2;
            }
            set
            {
                _Print2 = value;
                if (_Print2)
                {
                    Print1 = !_Print2;
                    Print3 = !_Print2;
                    Print4 = !_Print2;
                }
                NotifyOfPropertyChange(() => Print2);
            }
        }
        private bool _Print3 = false;
        public bool Print3
        {
            get
            {
                return _Print3;
            }
            set
            {
                _Print3 = value;
                if (_Print3)
                {
                    Print1 = !_Print3;
                    Print2 = !_Print3;
                    Print4 = !_Print3;
                }
                NotifyOfPropertyChange(() => Print3);
            }
        }
        private bool _Print4 = false;
        public bool Print4
        {
            get
            {
                return _Print4;
            }
            set
            {
                _Print4 = value;
                if (_Print4)
                {
                    Print1 = !_Print4;
                    Print2 = !_Print4;
                    Print3 = !_Print4;
                }
                NotifyOfPropertyChange(() => Print4);
            }
        }
        /*▲====: #009*/
        public enum NumberOfCopies
        {
            OneTime = 1,
            TwoTimes = 2,
            ThreeTimes = 3,
            Other = 0
        }

        public void GroupPrint_Loaded(object sender, RoutedEventArgs e)
        {
            /*▼====: #009*/
            //if (Registration_DataStorage.CurrentPatientRegistration != null && Registration_DataStorage.CurrentPatientRegistration.HisID.GetValueOrDefault() > 0)
            //{
            //((ePrescriptionOldNewView)this.GetView()).Print3.IsChecked = true;
            //}
            // TxD 22/09/2019: Commented OUT the following BECAUSE It caused an Error after adding ActivateItem to all Tabbed VMs in ConsultationSummary VM in OnActivate method
            //                  this HAS TO BE REVIEWED to REWRITE 
            //else
            //{
            //    ((ePrescriptionOldNewView)this.GetView()).Print1.IsChecked = true;
            //}
            //if (Registration_DataStorage.CurrentPatientRegistration != null && Registration_DataStorage.CurrentPatientRegistration.HisID.GetValueOrDefault() > 0)
            //{
            //    Print3 = true;
            //    NumberOfTimesPrint = ((int)NumberOfCopies.ThreeTimes).ToString();
            //}
            //else
            //{
            //    Print1 = true;
            //    NumberOfTimesPrint = ((int)NumberOfCopies.OneTime).ToString();
            //}
            /*▲====: #009*/

            //20190102 TTM: Mặc định số lần in là 1.
            if (Globals.ServerConfigSection.CommonItems.OrganizationUseSoftware > 0) //20190718 TBL: Neu cau hinh bs Huan thi mac dinh tick in 2 lan
            {
                Print2 = true;
                NumberOfTimesPrint = ((int)NumberOfCopies.TwoTimes).ToString();
            }
            else
            {
                Print1 = true;
                NumberOfTimesPrint = ((int)NumberOfCopies.OneTime).ToString();
            }

        }
        /*▼====: #009*/
        public void Print_Click(object sender, RoutedEventArgs e)
        {
            if (Print1)
            {
                IsTypingNumberOfCopies = false;
                NumberOfTimesPrint = ((int)NumberOfCopies.OneTime).ToString();
            }
            if (Print2)
            {
                IsTypingNumberOfCopies = false;
                NumberOfTimesPrint = ((int)NumberOfCopies.TwoTimes).ToString();
            }
            if (Print3)
            {
                IsTypingNumberOfCopies = false;
                NumberOfTimesPrint = ((int)NumberOfCopies.ThreeTimes).ToString();
            }
            if (Print4)
            {
                IsTypingNumberOfCopies = true;
                NumberOfTimesPrint = ((int)NumberOfCopies.Other).ToString();
            }
        }
        /*▲====: #009*/
        public void Print1_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            IsTypingNumberOfCopies = false;
            ((ePrescriptionOldNewView)this.GetView()).Print5.Text = "";
            NumberOfTimesPrint = ((int)NumberOfCopies.OneTime).ToString();

        }

        public void Print2_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            IsTypingNumberOfCopies = false;
            ((ePrescriptionOldNewView)this.GetView()).Print5.Text = "";
            NumberOfTimesPrint = ((int)NumberOfCopies.TwoTimes).ToString();
        }

        public void Print3_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            IsTypingNumberOfCopies = false;
            ((ePrescriptionOldNewView)this.GetView()).Print5.Text = "";
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

        private bool _DuocSi_IsEditingToaThuoc = false;

        public bool DuocSi_IsEditingToaThuoc
        {
            get { return _DuocSi_IsEditingToaThuoc; }
            set
            {
                if (_DuocSi_IsEditingToaThuoc != value)
                {
                    _DuocSi_IsEditingToaThuoc = value;
                    NotifyOfPropertyChange(() => DuocSi_IsEditingToaThuoc);
                }
            }
        }

        private ObservableCollection<DrugAndConTra> _allMedProductContraIndicatorRelToMedCond;
        public ObservableCollection<DrugAndConTra> allMedProductContraIndicatorRelToMedCond
        {
            get
            {
                return _allMedProductContraIndicatorRelToMedCond;
            }
            set
            {
                if (_allMedProductContraIndicatorRelToMedCond != value)
                {
                    _allMedProductContraIndicatorRelToMedCond = value;
                    NotifyOfPropertyChange(() => allMedProductContraIndicatorRelToMedCond);
                }
            }
        }

        private int xNgayBHToiDa_NgoaiTru = 30;
        private int xNgayBHToiDa_NoiTru = 5;
        private int xNgayBHToiDa = 0;
        private long? StoreID = 2;//tam thoi mac dinh kho ban(nha thuoc benh vien)
        private IEnumerator<IResult> DoGetStore_EXTERNAL()
        {
            var paymentTypeTask = new LoadStoreListTask((long)AllLookupValues.StoreType.STORAGE_EXTERNAL, false, null, false, false);
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

        public void Handle(PatientChange obj)
        {
            //Initialize();
        }

        void DrugList_OnRefresh(object sender, aEMR.Common.Collections.RefreshEventArgs e)
        {
            SearchDrugForPrescription_Paging(StoreID, DrugList.PageIndex, DrugList.PageSize, false);
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


        private ObservableCollection<PrescriptionIssueHistory> _allPrescriptionIssueHistory;
        public ObservableCollection<PrescriptionIssueHistory> allPrescriptionIssueHistory
        {
            get
            {
                return _allPrescriptionIssueHistory;
            }
            set
            {
                if (_allPrescriptionIssueHistory != value)
                {
                    _allPrescriptionIssueHistory = value;
                    NotifyOfPropertyChange(() => allPrescriptionIssueHistory);
                }
            }
        }
        //private PrescriptionDetail SelectedPrescriptionDetailCopy;

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

        private List<DiagnosisIcd10Items> _LstDiagICD10Items_InDay;
        public List<DiagnosisIcd10Items> LstDiagICD10Items_InDay
        {
            get { return _LstDiagICD10Items_InDay; }
            set
            {
                if (_LstDiagICD10Items_InDay != value)
                {
                    _LstDiagICD10Items_InDay = value;
                    NotifyOfPropertyChange(() => LstDiagICD10Items_InDay);
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
                    /*▼====: #004*/
                    if (_LatestePrecriptions.PtRegistrationID > 0)
                    {
                        LatestePrecriptions.IsObjectBeingUsedByClient = true;
                    }
                    /*▲====: #004*/
                    NotifyOfPropertyChange(() => LatestePrecriptions);
                    NotifyOfPropertyChange(() => IsInPtPrescription);
                    if (_LatestePrecriptions != null)
                    {
                        if (_LatestePrecriptions.PrescriptionIssueHistory == null)
                        {
                            _LatestePrecriptions.PrescriptionIssueHistory = new PrescriptionIssueHistory();
                        }

                        if (HisIDVisibility)
                        {
                            if (Registration_DataStorage.PatientServiceRecordCollection != null
                                && Registration_DataStorage.PatientServiceRecordCollection.Count > 0
                                && Registration_DataStorage.PatientServiceRecordCollection[0] != null
                                && Registration_DataStorage.PatientServiceRecordCollection[0].PrescriptionIssueHistories.Count > 0
                                && Registration_DataStorage.PatientServiceRecordCollection[0].PrescriptionIssueHistories[0] != null)
                            {
                                isHisID = Registration_DataStorage.PatientServiceRecordCollection[0].PrescriptionIssueHistories[0].HisID > 0 ? true : false;
                                //KMx: Nếu để dòng bên dưới. Sau khi Cập nhật toa bảo hiểm sẽ bị mất dấu check trong checkbox "Toa bảo hiểm".
                                //LatestePrecriptions.PrescriptionIssueHistory.HisID = 0;
                            }
                            else
                            {
                                isHisID = true;
                                LatestePrecriptions.PrescriptionIssueHistory.HisID = Registration_DataStorage.CurrentPatientRegistration == null ? 0 : Registration_DataStorage.CurrentPatientRegistration.HisID.Value;
                            }
                        }
                        else
                        {
                            isHisID = false;
                            LatestePrecriptions.PrescriptionIssueHistory.HisID = 0;
                        }
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

        private List<Prescription> _LstPrescription_TrongNgay;
        public List<Prescription> LstPrescription_TrongNgay
        {
            get
            {
                return _LstPrescription_TrongNgay;
            }
            set
            {
                if (_LstPrescription_TrongNgay != value)
                {
                    _LstPrescription_TrongNgay = value;
                    NotifyOfPropertyChange(() => LstPrescription_TrongNgay);
                }
            }
        }

        private Prescription _Prescriptions_TrongNgay;
        public Prescription Prescriptions_TrongNgay
        {
            get
            {
                return _Prescriptions_TrongNgay;
            }
            set
            {
                if (_Prescriptions_TrongNgay != value)
                {
                    _Prescriptions_TrongNgay = value;
                    NotifyOfPropertyChange(() => Prescriptions_TrongNgay);
                }
            }
        }

        private List<IList<PrescriptionDetail>> _ListPrescriptionDetailTrongNgay;
        public List<IList<PrescriptionDetail>> ListPrescriptionDetailTrongNgay
        {
            get
            {
                return _ListPrescriptionDetailTrongNgay;
            }
            set
            {
                if (_ListPrescriptionDetailTrongNgay != value)
                {
                    _ListPrescriptionDetailTrongNgay = value;
                    NotifyOfPropertyChange(() => ListPrescriptionDetailTrongNgay);
                }
            }
        }
        //▼===== #029
        public bool IsInPtPrescription
        {
            get
            {
                return LatestePrecriptions != null && LatestePrecriptions.V_RegistrationType == AllLookupValues.RegistrationType.NOI_TRU;
            }
        }
        //▲===== #029
        private decimal _TotalHIPaymentForRegistration;
        public decimal TotalHIPaymentForRegistration
        {
            get { return _TotalHIPaymentForRegistration; }
            set
            {
                if (_TotalHIPaymentForRegistration != value)
                {
                    _TotalHIPaymentForRegistration = value;
                    NotifyOfPropertyChange(() => TotalHIPaymentForRegistration);
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
        //▼===== #027        
        private void GetAllDiagnosisIcd10Items_InDay(long PatientID, long ServiceRecID, long PtRegDetailID)
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                using (var serviceFactory = new ePrescriptionsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetAllDiagnosisIcd10Items_InDay(PatientID, ServiceRecID, PtRegDetailID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            List<DiagnosisIcd10Items> LstResult = contract.EndGetAllDiagnosisIcd10Items_InDay(asyncResult);
                            if (LstResult != null && LstResult.Count > 0)
                            {
                                LstDiagICD10Items_InDay = LstResult;
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
        //▲===== #027
        private void GetAllPrescription_TrongNgay_ByPatientID(long PatientID)
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                using (var serviceFactory = new ePrescriptionsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginPrescriptions_TrongNgay_ByPatientID(PatientID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            List<Prescription> LstPrescriptions = contract.EndPrescriptions_TrongNgay_ByPatientID(asyncResult);
                            if (LstPrescriptions != null && LstPrescriptions.Count > 0)
                            {
                                LstPrescription_TrongNgay = LstPrescriptions;
                                GetAllPrescriptionDetails(LstPrescription_TrongNgay);
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
        //CMN: Lấy toa thuốc cũ dành cho chức năng tạo mới toa thuốc từ một toa thuốc đã được tạo trước đó
        private void GetLatestPrescriptionByPtID_New(long PatientID)
        {
            this.ShowBusyIndicator();
            //Danh cho truong hop la toa moi
            IsWaitingGetLatestPrescriptionByPtID = true;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new ePrescriptionsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetLatestPrescriptionByPtID(PatientID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var ToaThuoc_Cuoi = contract.EndGetLatestPrescriptionByPtID(asyncResult);
                            if (ToaThuoc_Cuoi != null && ToaThuoc_Cuoi.IssueID > 0)
                            {
                                //if (V_TreatmentTypeArray.Length == 0)
                                //{
                                //    btnCreateNewIsEnabled = true;
                                //    btnCreateAndCopyIsEnabled = true;
                                //}
                                //else
                                //{
                                //    if (IsEnableAddPrescription)
                                //    {
                                //        btnCreateNewIsEnabled = true;
                                //        btnCreateAndCopyIsEnabled = true;
                                //    }
                                //    else
                                //    {
                                //        btnCreateNewIsEnabled = false;
                                //        btnCreateAndCopyIsEnabled = false;
                                //    }
                                //}
                                btnCreateNewIsEnabled = true;
                                btnCreateAndCopyIsEnabled = true;
                                btnSaveAddNewIsEnabled = false;
                                //KMx: Khi load toa thuốc thì Enable của form phải bằng false, khi nào người dùng nhấn "Chỉnh sửa" thì Enable mới bằng true.
                                // Nếu không có dòng dưới thì sẽ bị sai khi đang "chỉnh sửa" toa cho BN A, chọn BN B từ out standing task, thì toa của BN B tự động Enable (29/05/2014 15:56).
                                IsEnabled = false;
                                if (CheckToaThuocDuocPhepCapNhat(DuocSi_IsEditingToaThuoc, ToaThuoc_Cuoi) == false)
                                {
                                    btnEditIsEnabled = false;
                                }
                                else
                                {
                                    btnEditIsEnabled = true;
                                }

                                btnCopyToIsEnabled = true;
                                IsEnabledPrint = true;
                                LatestePrecriptions = ToaThuoc_Cuoi;
                                //▼===== #025
                                ModifyBorderExamDate();
                                //▲===== #025
                                if (!IsShowSummaryContent && (Registration_DataStorage.PatientServiceRecordCollection == null || Registration_DataStorage.PatientServiceRecordCollection.Count == 0))
                                {
                                    LatestePrecriptions.ObjDiagnosisTreatment = new DiagnosisTreatment();
                                }
                                else
                                {
                                    LatestePrecriptions.ObjDiagnosisTreatment = Registration_DataStorage.PatientServiceRecordCollection[0].DiagnosisTreatments[0];
                                }
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
                                //CMN: Thêm cấu hình tự động tick hẹn bệnh khi ra toa
                                if (Globals.ServerConfigSection.ConsultationElements.IsCheckApmtOnPrescription)
                                {
                                    LatestePrecriptions.HasAppointment = true;
                                }
                                if (LatestePrecriptions.V_RegistrationType == AllLookupValues.RegistrationType.NOI_TRU)
                                {
                                    GetPrescriptionDetailsByPrescriptID_InPt(LatestePrecriptions.PrescriptID);
                                }
                                else
                                {
                                    GetPrescriptionDetailsByPrescriptID(LatestePrecriptions.PrescriptID);
                                }
                                ////Toa thuoc nay co roi
                                ////check xem neu la cung mot bac si thi chinh sua
                                ////khac bac si thi la phat hanh lai toa thuoc
                                //PrescripState = PrescriptionState.EditPrescriptionState;
                            }
                            else/*Toa Mới*/
                            {
                                LatestePrecriptions = NewPrecriptions.DeepCopy();
                                //20190718 TBL: Neu la benh nhan moi thi tu dong tick hen benh theo cau hinh BS Huan
                                if (Globals.ServerConfigSection.CommonItems.OrganizationUseSoftware > 0)
                                {
                                    LatestePrecriptions.HasAppointment = true;
                                    LatestePrecriptions.ResetDataChanged();
                                }
                                //CMN: Thêm cấu hình tự động tick hẹn bệnh khi ra toa
                                if (Globals.ServerConfigSection.ConsultationElements.IsCheckApmtOnPrescription)
                                {
                                    LatestePrecriptions.HasAppointment = true;
                                }
                                BackupCurPrescriptionItem();
                                //▼====: #038
                                //AddNewBlankDrugIntoPrescriptObjectNew();

                                IsEnabled = false;
                                btChonChanDoanIsEnabled = true;
                                btnCreateNewIsEnabled = true;
                                btnCreateAndCopyIsEnabled = false;
                                btnSaveAddNewIsEnabled = false;
                                btnUndoIsEnabled = false;
                                btnEditIsEnabled = false;
                                btnCopyToIsEnabled = false;
                                IsEnabledPrint = false;
                                //▲====: #038
                                //▼===== #023
                                if (Globals.ConfirmSecretaryLogin != null)
                                {
                                    LatestePrecriptions.SecretaryStaff = Globals.ConfirmSecretaryLogin.Staff;
                                }
                                //▲===== #023
                            }

                            //▼===== 20191011: loại bỏ Globals.SecretaryLogin. 
                            //if (Globals.SecretaryLogin != null)
                            //{
                            //    LatestePrecriptions.SecretaryStaff = Globals.SecretaryLogin.Staff;
                            //}
                            //▲===== #20191011
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                        finally
                        {
                            IsWaitingGetLatestPrescriptionByPtID = false;
                            this.HideBusyIndicator();
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
        public void GetPrescriptionDetailsByPrescriptID_V2(long prescriptID)
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                using (var serviceFactory = new ePrescriptionsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetPrescriptionDetailsByPrescriptID(prescriptID, false, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var Results = contract.EndGetPrescriptionDetailsByPrescriptID(asyncResult);
                            Prescriptions_TrongNgay = new Prescription();
                            if (ListPrescriptionDetailTrongNgay == null)
                            {
                                ListPrescriptionDetailTrongNgay = new List<IList<PrescriptionDetail>>();
                            }
                            Prescriptions_TrongNgay.PrescriptionDetails = Results.ToObservableCollection();
                            ListPrescriptionDetailTrongNgay.Add(Results);
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
                    contract.BeginGetPrescriptionDetailsByPrescriptID(prescriptID, GetRemaining, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var Results = contract.EndGetPrescriptionDetailsByPrescriptID(asyncResult);
                            LatestePrecriptions.PrescriptionDetails = Results.ToObservableCollection();
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
        public void GetPrescriptionDetailsByPrescriptID_InPt(long prescriptID, bool GetRemaining = false)
        {
            string msg;
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                using (var serviceFactory = new ePrescriptionsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetPrescriptionDetailsByPrescriptID_InPt(prescriptID, GetRemaining, true, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var Results = contract.EndGetPrescriptionDetailsByPrescriptID_InPt(asyncResult);
                            LatestePrecriptions.PrescriptionDetails = Results.ToObservableCollection();
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
                    contract.BeginGetPrescriptionDetailsByPrescriptID(prescriptID, GetRemaining, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var Results = contract.EndGetPrescriptionDetailsByPrescriptID(asyncResult);
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
        private void SearchDrugForPrescription_Paging(long? StoreID, int PageIndex, int PageSize, bool CountTotal)
        {
            System.Diagnostics.Debug.WriteLine(" ===>> SearchDrugForPrescription_Paging .................");
            DrugList.Clear();
            if (String.IsNullOrEmpty(BrandName) || String.IsNullOrWhiteSpace(BrandName))
            {
                return;
            }
            var t = new Thread(() =>
            {
                using (var serviceFactory = new ePrescriptionsServiceClient())
                {
                    var client = serviceFactory.ServiceInstance;
                    client.BeginSearchDrugForPrescription_Paging(BrandName, IsSearchByGenericName, IsInsurance, StoreID, PageIndex, PageSize, CountTotal, Globals.DispatchCallback((asyncResult) =>
                    {
                        int Total = 0;
                        IList<GetDrugForSellVisitor> allItems = null;
                        bool bOK = false;
                        try
                        {
                            allItems = client.EndSearchDrugForPrescription_Paging(out Total, asyncResult);
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
                                if (IsSearchByTreatmentRegimen)
                                {
                                    // TxD 30/09/2018: Added the following check JUST INCASE DrugsInTreatmentRegimen IS NULL for some unstable reason then atleast it's still working TBR...
                                    if (DrugsInTreatmentRegimen != null)
                                    {
                                        allItems = allItems.Where(x => DrugsInTreatmentRegimen.Any(tr => tr.DrugID == x.DrugID)).ToList();
                                    }
                                }
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
                                if (AutoThuoc != null)
                                {
                                    //Ny notes: tai sao phai dung DeepCopy???dung no se khong phan trang dc.
                                    // AutoThuoc.ItemsSource = ObjectCopier.DeepCopy(DrugList);
                                    AutoThuoc.ItemsSource = DrugList;
                                    AutoThuoc.PopulateComplete();
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

        public object GetChooseDose(object value)
        {
            PrescriptionDetail p = value as PrescriptionDetail;
            if (p != null)
            {
                return p.ChooseDose;
            }
            else
            {
                return null;
            }
        }

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

        public void AddNewBlankDrugIntoPrescriptObjectNew()
        {
            Application.Current.Dispatcher.Invoke(() =>
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

                //20180918 TBL: Tao dong rong khong can phai gan gia tri
                //ObjPrescriptionDetailForForm = NewReInitPrescriptionDetail(true, null);
                if (LatestePrecriptions.PrescriptionDetails.Count > 1)
                {
                    LatestePrecriptions.PreNoDrug = false;
                    NotifyOfPropertyChange(() => LatestePrecriptions.PreNoDrug);
                }
            });
        }


        private PrescriptionDetail NewReInitPrescriptionDetail(bool bForm, PrescriptionDetail existingPrescriptObj, bool bOnlyInitObj = false)
        {
            PrescriptionDetail prescriptDObj = existingPrescriptObj;
            if (!bOnlyInitObj)
            {
                if (prescriptDObj == null)
                {
                    prescriptDObj = new PrescriptionDetail();
                    /*▼====: #004*/
                    prescriptDObj.IsObjectBeingUsedByClient = true;
                    /*▲====: #004*/
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
            //20180918 TBL: Anh Tuan keu khog dc tu DeepCopy chinh no, chung nao chuong trinh co van de thi moi quay lai xem xet
            //prescriptDObj = ObjectCopier.DeepCopy(prescriptDObj);

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
                    // TxD 20203110: Check if item has NO DRUG selected then there is nothing to check for it
                    if (item.DrugID == null || item.DrugID == 0)
                    {
                        continue;
                    }

                    // 29/09/2013
                    // Txd : Do the following Check BH and re-assigning for DayRpts and DayExtended Just
                    //       in case they were missed (NOT BEING ASSIGNED) previously 
                    //       BECAUSE IT IS ALWAYS CORRECT IN ALL CASES (assume at this stage)
                    item.DayRpts = item.RealDay;
                    item.DayExtended = 0;
                    //KMx: Không phân biệt toa thường hay toa bảo hiểm (04/06/2014 10:45)
                    //if (BH && item.RealDay > xNgayBHToiDa)
                    bool IsDieuTruNgoaiTru = false;
                    if (Registration_DataStorage != null && Registration_DataStorage.CurrentPatientRegistration != null 
                        && Registration_DataStorage.CurrentPatientRegistration.OutPtTreatmentProgramID > 0)
                    {
                        IsDieuTruNgoaiTru = true;
                    }
                    if (item.RealDay > xNgayBHToiDa && !IsDieuTruNgoaiTru)
                    {
                        item.DayRpts = xNgayBHToiDa;
                        item.DayExtended = item.RealDay - xNgayBHToiDa;
                    }


                    if (item.V_DrugType == (long)AllLookupValues.V_DrugType.THUOC_UONGLICHTUAN)
                    {
                        float fTotalQty = 0;
                        if (item.ObjPrescriptionDetailSchedules != null)
                        {
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
                        }

                        if (fTotalQty <= 0)
                        {
                            if (item.SelectedDrugForPrescription != null && item.SelectedDrugForPrescription.BrandName != null) // TxD 20201031: Checking JUST INCASE it's the blank row that somehow got here
                            {
                                sb.AppendLine(string.Format(eHCMSResources.Z1055_G1_ThuocTheoLichChonSLgPhuHop, item.SelectedDrugForPrescription.BrandName.Trim()));
                            }
                            Result = false;
                        }
                        //KMx: Thuốc Lịch nếu số ngày <= 0 hoặc số lượng <= 0 thì báo lỗi (16/03/2014 10:54)
                        else
                        {
                            if (item.DayRpts <= 0)
                            {
                                sb.AppendLine(string.Format(eHCMSResources.Z0911_G1_Thuoc0NgDungLonHon0, item.SelectedDrugForPrescription.BrandName.Trim()));
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
                            sb.AppendLine(string.Format(eHCMSResources.Z1056_G1_ThuocNgDungLonHon0, item.SelectedDrugForPrescription.BrandName.Trim()));
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
            else if (IsShowSummaryContent)
            {
                if (!PreNoDrug)
                {
                    MessageBox.Show(string.Format("{0}!", eHCMSResources.A0401_G1_Msg_InfoChuaChonThuocDeRaToa), eHCMSResources.A0290_G1_Msg_LuuToaThuoc, MessageBoxButton.OK);
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
            /*▼====: #011*/
            //TBL: Kiem tra toa khong thuoc
            else if (LatestePrecriptions.IsPrescriptionDataChanged)
            {
                if (!PreNoDrug)
                {
                    MessageBox.Show(string.Format("{0}!", eHCMSResources.A0401_G1_Msg_InfoChuaChonThuocDeRaToa), eHCMSResources.A0290_G1_Msg_LuuToaThuoc, MessageBoxButton.OK);
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
            /*▲====: #011*/
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
                        //==== 20161205 CMN Begin: Fixed float variable not equal real
                        //if (Objtmp.Qty < Math.Ceiling(Tong))
                        if (Tong - Objtmp.Qty > 0.0001)
                        //==== 20161205 CMN End.
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        private string CheckThuocBiTrungHoatChatTrongNgay()
        {
            //TBL: Neu bat cau hinh thi moi kiem tra hoat chat
            if (!Globals.ServerConfigSection.ConsultationElements.CheckToaThuocBiTrungTheoHoatChat || bBlock)
            {
                return "";
            }
            StringBuilder sb = new StringBuilder();
            string err = "";
            if (LatestePrecriptions.PrescriptionDetails.Count > 1)
            {
                if (CheckToaThuocBiTrungTrongNgay_TheoHoatChat(ListPrescriptionDetailTrongNgay, LatestePrecriptions.PrescriptionDetails, out err))
                {
                    return string.Format(eHCMSResources.Z2650_G1_ToaThuocTrungHoatChatTrongNgay + Environment.NewLine + err);
                }
            }
            return "";
        }

        private string ErrCheckChongChiDinh()
        {
            //TBL: Neu bat cau hinh thi moi kiem tra chong chi dinh
            if (!Globals.ServerConfigSection.ConsultationElements.AllowBlockContraIndicator)
            {
                return "";
            }
            StringBuilder sb = new StringBuilder();
            if (LatestePrecriptions.PrescriptionDetails.Count > (btnSaveAddNewIsEnabled || btnUpdateIsEnabled ? 1 : 0))
            {
                foreach (PrescriptionDetail item in LatestePrecriptions.PrescriptionDetails)
                {
                    if (item.SelectedDrugForPrescription != null)
                    {
                        string err = "";
                        bool IsWarning = true;
                        if (CheckChongChiDinh1Drug(item.DrugID.Value, CS_DS.refIDC10List, out err, out IsWarning))
                        {
                            sb.Append(err);
                            item.IsContraIndicator = true;
                        }
                        else
                        {
                            item.IsContraIndicator = false;
                        }
                        if (!IsWarning)
                        {
                            bBlock = true;
                        }
                    }
                }
            }
            if (!string.IsNullOrEmpty(sb.ToString()))
            {
                return eHCMSResources.Z2927_G1_ToaCoCCD + ":" + Environment.NewLine + sb.ToString();
            }
            return "";
        }
        //▼===== #027
        private string ErrCheckChongChiDinh_TrongNgay()
        {
            //TBL: Neu bat cau hinh thi moi kiem tra chong chi dinh
            if (!Globals.ServerConfigSection.ConsultationElements.AllowBlockContraIndicatorInDay)
            {
                return "";
            }
            StringBuilder sb = new StringBuilder();
            if (LatestePrecriptions.PrescriptionDetails.Count > (btnSaveAddNewIsEnabled || btnUpdateIsEnabled ? 1 : 0) && LstDiagICD10Items_InDay.Count > 0)
            {
                foreach (PrescriptionDetail item in LatestePrecriptions.PrescriptionDetails)
                {
                    if (item.SelectedDrugForPrescription != null)
                    {
                        string err = "";
                        bool IsWarning = true;
                        if (CheckChongChiDinh1Drug(item.DrugID.Value, LstDiagICD10Items_InDay.ToObservableCollection(), out err, out IsWarning))
                        {
                            sb.Append(err);
                            item.IsContraIndicator = true;
                        }
                        else
                        {
                            item.IsContraIndicator = false;
                        }
                        if (!IsWarning)
                        {
                            bBlock = true;
                        }
                    }
                }
            }
            if (!string.IsNullOrEmpty(sb.ToString()))
            {
                return eHCMSResources.Z2928_G1_ToaCoCCDTrongNgay + ":" + Environment.NewLine + sb.ToString();
            }
            return "";
        }
        //▲===== #027
        private bool CheckChongChiDinh1Drug(long DrugID, ObservableCollection<DiagnosisIcd10Items> LstICD10Item, out string msg, out bool IsWarning)
        {
            double Age = Registration_DataStorage.CurrentPatientRegistration.Patient.Age.GetValueOrDefault();
            //20190923 TBL: Trường hợp bệnh nhân chỉ có tháng không có tuổi thì lấy tháng / 12 để ra được số tuổi
            if (Age == 0 && Registration_DataStorage.CurrentPatientRegistration.Patient.MonthsOld > 0)
            {
                Age = Convert.ToDouble(Registration_DataStorage.CurrentPatientRegistration.Patient.MonthsOld) / 12;
            }
            //double AgeMonth = Convert.ToDouble((Globals.GetCurServerDateTime().Year - Registration_DataStorage.CurrentPatientRegistration.Patient.DOB.Value.Year) * 12 + (Globals.GetCurServerDateTime().Month - Registration_DataStorage.CurrentPatientRegistration.Patient.DOB.Value.Month));
            //double AgeDay = Convert.ToDouble((Globals.GetCurServerDateTime() - Registration_DataStorage.CurrentPatientRegistration.Patient.DOB).Value.TotalDays);
            msg = "";
            string msgV_AgeUnit = "";
            IsWarning = true;
            if (allMedProductContraIndicatorRelToMedCond == null)
            {
                return false;
            }
            foreach (var DCR in allMedProductContraIndicatorRelToMedCond)
            {
                if (DrugID == DCR.DrugID)
                {
                    foreach (var LCT in DCR.ListConTraAndLstICDs)
                    {
                        switch (LCT.V_AgeUnit)
                        {
                            case (long)AllLookupValues.V_AgeUnit.Thang:
                                Age = Convert.ToDouble((Globals.GetCurServerDateTime().Year - Registration_DataStorage.CurrentPatientRegistration.Patient.DOB.Value.Year) * 12 
                                    + (Globals.GetCurServerDateTime().Month - Registration_DataStorage.CurrentPatientRegistration.Patient.DOB.Value.Month));
                                msgV_AgeUnit = " tháng ";
                                break;
                            case (long)AllLookupValues.V_AgeUnit.Ngay:
                                Age = Convert.ToDouble((Globals.GetCurServerDateTime() - Registration_DataStorage.CurrentPatientRegistration.Patient.DOB).Value.TotalDays);
                                msgV_AgeUnit = " ngày ";
                                break;
                        }

                        //CCD theo tuoi
                        if (LCT.ListICD10Code == null || LCT.ListICD10Code.Count == 0)
                        {
                            //Chong chi dinh theo do tuoi tu ... den...
                            if (LCT.AgeFrom <= Age && LCT.AgeTo >= Age && LCT.AgeFrom != 0 && LCT.AgeTo != 0)
                            {
                                msg += string.Format("- " + eHCMSResources.Z2629_G1_Thuoc0CCDVoiTuoiTu1Den2, DCR.BrandName.Trim(), LCT.AgeFrom.ToString() + msgV_AgeUnit, LCT.AgeTo.ToString() + msgV_AgeUnit);
                                IsWarning = LCT.IsWarning;
                                if (!IsWarning)
                                {
                                    msg += " (*)" + Environment.NewLine;
                                }
                                else
                                {
                                    msg += Environment.NewLine;
                                }
                            }
                            //Chong chi dinh theo do tuoi tren ...
                            else if (LCT.AgeFrom < Age && LCT.AgeTo == 0)
                            {
                                msg += string.Format("- " + eHCMSResources.Z2632_G1_Thuoc0CCDVoiTuoiTren1, DCR.BrandName.Trim(), LCT.AgeFrom.ToString() + msgV_AgeUnit);
                                IsWarning = LCT.IsWarning;
                                if (!IsWarning)
                                {
                                    msg += " (*)" + Environment.NewLine;
                                }
                                else
                                {
                                    msg += Environment.NewLine;
                                }
                            }
                            //Chong chi dinh theo do tuoi duoi ...
                            else if (LCT.AgeTo > Age && (LCT.AgeTo != null && LCT.AgeTo != 0) && (LCT.AgeFrom == null || LCT.AgeFrom == 0))
                            {
                                msg += string.Format("- " + eHCMSResources.Z2631_G1_Thuoc0CCDVoiTuoiDuoi1, DCR.BrandName.Trim(), LCT.AgeTo.ToString() + msgV_AgeUnit);
                                IsWarning = LCT.IsWarning;
                                if (!IsWarning)
                                {
                                    msg += " (*)" + Environment.NewLine;
                                }
                                else
                                {
                                    msg += Environment.NewLine;
                                }
                            }
                        }
                        //CCD theo ICD + tuoi
                        else
                        {
                            long indexICD = 0;
                            string ICDs = "";
                            List<string> ListICD = new List<string>();
                            foreach (string LstICD in LCT.ListICD10Code)
                            {
                                foreach (DiagnosisIcd10Items ICD10 in LstICD10Item)
                                {
                                    if (LstICD == ICD10.ICD10Code)
                                    {
                                        ListICD.Add(LstICD);
                                    }
                                }
                            }
                            foreach (string item in ListICD)
                            {
                                if (indexICD < ListICD.Count - 1)
                                {
                                    ICDs = ICDs + item + ", ";
                                }
                                else
                                {
                                    ICDs += item;
                                }
                                indexICD++;
                            }
                            //Chong chi dinh theo ICD10
                            if (ListICD.Count > 0 && ((LCT.AgeFrom == null && LCT.AgeTo == null) || (LCT.AgeFrom == 0 && LCT.AgeTo == 0)))
                            {
                                msg += string.Format("- " + eHCMSResources.Z1498_G1_Thuoc0CCDVoiDKienBenh1, DCR.BrandName.Trim(), ICDs, LCT.AgeFrom.ToString() + msgV_AgeUnit, LCT.AgeTo.ToString() + msgV_AgeUnit);
                                IsWarning = LCT.IsWarning;
                                if (!IsWarning)
                                {
                                    msg += " (*)" + Environment.NewLine;
                                }
                                else
                                {
                                    msg += Environment.NewLine;
                                }
                            }
                            //Chong chi dinh theo ICD10 va do tuoi tu ... den ...
                            else if (ListICD.Count > 0 && LCT.AgeFrom <= Age && LCT.AgeTo >= Age && LCT.AgeFrom != null && LCT.AgeTo != null && LCT.AgeFrom != 0 && LCT.AgeTo != 0)
                            {
                                msg += string.Format("- " + eHCMSResources.Z2630_G1_Thuoc0CCDVoiDKienBenh1VaDoTuoiTu2Den3, DCR.BrandName.Trim(), ICDs, LCT.AgeFrom.ToString() + msgV_AgeUnit, LCT.AgeTo.ToString() + msgV_AgeUnit);
                                IsWarning = LCT.IsWarning;
                                if (!IsWarning)
                                {
                                    msg += " (*)" + Environment.NewLine;
                                }
                                else
                                {
                                    msg += Environment.NewLine;
                                }
                            }
                            //Chong chi dinh theo ICD10 va do tuoi tren ...
                            else if (ListICD.Count > 0 && LCT.AgeFrom < Age && (LCT.AgeTo == null || LCT.AgeTo == 0))
                            {
                                msg += string.Format("- " + eHCMSResources.Z2660_G1_Thuoc0CCDVoiDKienBenh1VaDoTuoiTren2, DCR.BrandName.Trim(), ICDs, LCT.AgeFrom.ToString() + msgV_AgeUnit);
                                IsWarning = LCT.IsWarning;
                                if (!IsWarning)
                                {
                                    msg += " (*)" + Environment.NewLine;
                                }
                                else
                                {
                                    msg += Environment.NewLine;
                                }
                            }
                            //Chong chi dinh theo ICD10 va do tuoi duoi ...
                            else if (ListICD.Count > 0 && LCT.AgeTo > Age && (LCT.AgeFrom == null || LCT.AgeFrom == 0))
                            {
                                msg += string.Format("- " + eHCMSResources.Z2661_G1_Thuoc0CCDVoiDKienBenh1VaDoTuoiDuoi2, DCR.BrandName.Trim(), ICDs, LCT.AgeTo.ToString() + msgV_AgeUnit);
                                IsWarning = LCT.IsWarning;
                                if (!IsWarning)
                                {
                                    msg += " (*)" + Environment.NewLine;
                                }
                                else
                                {
                                    msg += Environment.NewLine;
                                }
                            }
                        }
                    }
                }
            }
            if (!string.IsNullOrEmpty(msg))
            {
                return true;
            }
            return false;
        }

        public void ClearForm()
        {
            AutoThuoc.Text = "";
            DrugAdministrationForm.Text = "";

        }

        //KMx: Sau khi kiểm tra, thấy hàm này không còn sử dụng nữa (13/06/2014 10:00).
        //private void AutoAdjust()
        //{
        //    AutoAdjustCancelDrugShortDays();
        //}


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
            if (Registration_DataStorage.CurrentPatientRegistrationDetail == null)
            {
                return false;
            }
            //20190330 TBL: Tick toa bao hiem se dua tren HisID cua dich vu chu khong dua vao cua dang ky nua
            //if (Registration_DataStorage.CurrentPatientRegistration.HisID.HasValue && Registration_DataStorage.CurrentPatientRegistration.HisID.Value > 0)
            //{
            //    return true;
            //}
            //▼====: #049
            if (Registration_DataStorage.CurrentPatientRegistrationDetail.HisID.HasValue && Registration_DataStorage.CurrentPatientRegistrationDetail.HisID.Value > 0
                && Registration_DataStorage.CurrentPatientRegistrationDetail.HIBenefit.GetValueOrDefault(0) > 0 
                    ||  (Registration_DataStorage.CurrentPatientRegistration.HisID.HasValue && Registration_DataStorage.CurrentPatientRegistration.HisID.Value > 0
                            && Registration_DataStorage.CurrentPatientRegistrationDetail.RefMedicalServiceItem.IsAllowDrugHIForOutPatient))
            {
                return true;
            }
            //▲====: #049
            return false;
        }

        private bool checkDrugType(PrescriptionDetail PrescriptDetail)
        {
            switch (PrescriptDetail.DrugType.LookupID)
            {
                case (long)AllLookupValues.V_DrugType.THUOC_UONGLICHTUAN:
                    MessageBox.Show(eHCMSResources.Z0967_G1_ThuocUongTheoLichTuan);
                    return false;
            }
            return true;
        }

        private void BackupCurPrescriptionItem()
        {
            /*▼====: #004*/
            LatestePrecriptions.IsObjectBeingUsedByClient = false;
            /*▲====: #004*/
            RetoreLatestePrecriptions = LatestePrecriptions.DeepCopy();
            PrecriptionsBeforeUpdate = LatestePrecriptions.DeepCopy();
            /*▼====: #004*/
            LatestePrecriptions.IsObjectBeingUsedByClient = true;
            /*▲====: #004*/
        }

        private void RestoreCurPrescriptionItem()
        {
            LatestePrecriptions = RetoreLatestePrecriptions;
            /*▼====: #004*/
            LatestePrecriptions.IsObjectBeingUsedByClient = true;
            /*▲====: #004*/
        }

        private void RestoreToaChonDeSua()
        {
            if (DuocSi_IsEditingToaThuoc)
            {
                LatestePrecriptions = ToaChonDeSua;
                GetPrescriptionDetailsByPrescriptID(LatestePrecriptions.PrescriptID);
            }
        }

        #region button member
        private void RemoveLastRowPrecriptionDetail()
        {
            if (LatestePrecriptions == null || LatestePrecriptions.PrescriptionDetails == null) return;
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


            //var BlankDrug = new ObservableCollection<PrescriptionDetail>(LatestePrecriptions.PrescriptionDetails);

            //foreach (var item in BlankDrug)
            //{
            //    if (item.SelectedDrugForPrescription == null || string.IsNullOrEmpty(item.SelectedDrugForPrescription.BrandName) || (!item.IsDrugNotInCat && item.SelectedDrugForPrescription.DrugID <= 0))
            //    {
            //        LatestePrecriptions.PrescriptionDetails.Remove(item);
            //    }
            //}
        }

        //private void RemoveLastRowPrecriptionDetail()
        //{
        //    if (LatestePrecriptions.PrescriptionDetails.Count > 0)
        //    {
        //        var listtmp = LatestePrecriptions.PrescriptionDetails.DeepCopy();

        //        var listPCLAdd = (from c in listtmp
        //                          where (c.IsDrugNotInCat) || (c.SelectedDrugForPrescription != null && c.SelectedDrugForPrescription.DrugID > 0)
        //                          select c);

        //        LatestePrecriptions.PrescriptionDetails.Clear();

        //        foreach (var item in listPCLAdd)
        //        {
        //            LatestePrecriptions.PrescriptionDetails.Add(ObjectCopier.DeepCopy(item));
        //        }
        //    }
        //}

        private void CheckpreApptCheck()
        {
            //tam thoi de o day
            //xem benh nhan co hen benh chua
            if (LatestePrecriptions.HasAppointment)
            {
                return;
            }
            //neu chua co hen benh thi bat popup chon hen benh len
            GlobalsNAV.ShowDialog<IPrescriptionApptCheck>();
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

            if (DuocSi_IsEditingToaThuoc)
            {
                RestoreToaChonDeSua();

                btnEditIsEnabled = true;
                btDuocSiEditIsEnabled = false;
                IsEnabledPrint = true;
            }
            else
            {
                btnCreateNewIsEnabled = true;
                btnSaveAddNewIsEnabled = false;
                btnUpdateIsEnabled = false;
                //bntSaveAsIsEnabled = false;

                RestoreCurPrescriptionItem();
                if (LatestePrecriptions != null && LatestePrecriptions.PrescriptID > 0)//Sửa
                {
                    btnCopyToIsEnabled = true;
                    if (CheckToaThuocDuocPhepCapNhat(DuocSi_IsEditingToaThuoc) == false)
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
            if (Globals.IsLockRegistration(Registration_DataStorage.CurrentPatientRegistration.RegLockFlag, eHCMSResources.Z1388_G1_PhatHanhLaiToa))
            {
                return;
            }
            //▼====== #016
            if (!KiemTraDaDuyetToaChua())
            {
                return;
            }
            //▲====== #016
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
                GetRemainingPrescriptionDetailsByPrescriptID(PrescriptID, true);
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

        private bool CheckToaThuocDuocPhepCapNhat(bool DuocSiDangSua)
        {
            if (DuocSiDangSua)
            {
                return true;
            }

            if (LatestePrecriptions.CreatorStaffID != Globals.LoggedUserAccount.StaffID)
            {
                //MessageBox.Show(string.Format("{0}!", eHCMSResources.Z0958_G1_ChiCoNguoiRaToaMoiDuocSua), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);/*khoi can thong bao*/
                return false;
            }
            //PrescripState = PrescriptionState.EditPrescriptionState;
            return true;
        }

        private bool CheckToaThuocDuocPhepCapNhat(bool DuocSiDangSua, Prescription ToaThuoc)
        {
            if (DuocSiDangSua)
            {
                return true;
            }

            if (ToaThuoc.CreatorStaffID != Globals.LoggedUserAccount.StaffID)
            {
                //MessageBox.Show(string.Format("{0}!", eHCMSResources.Z0958_G1_ChiCoNguoiRaToaMoiDuocSua), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);/*khoi can thong bao*/
                return false;
            }
            //PrescripState = PrescriptionState.EditPrescriptionState;
            return true;
        }

        private bool ValidateExpiredPrescription(Prescription ToaThuoc)
        {
            DateTime curDate = Globals.GetCurServerDateTime();
            //▼====== #017
            int DayAllowToUpdatePrescription = Globals.ServerConfigSection.ConsultationElements.AllowTimeUpdatePrescription / 24;
            if (ToaThuoc.PrescriptionIssueHistory.IssuedDateTime.Value.AddDays(DayAllowToUpdatePrescription) < curDate)
            {
                btnCopyToIsEnabled = false;
                btnEditIsEnabled = false;
                IsShowValidateExpiredPrescription = true; //20191021 TBL: Task #1104: Chỉ hiển thị thông báo 1 lần, vì GetToaThuocDaCo được gọi 2 lần không biết lý do
                Coroutine.BeginExecute(MessageWarningShowDialogTask(string.Format(eHCMSResources.Z3007_G1_ToaHetThoiGianChSua, DayAllowToUpdatePrescription)));
                return false;
            }
            //▲====== #017
            return true;
        }

        public void btnEdit()
        {
            if (Globals.IsLockRegistration(Registration_DataStorage.CurrentPatientRegistration.RegLockFlag, "chỉnh sửa toa thuốc"))
            {
                return;
            }
            //▼====== #016
            if (!KiemTraDaDuyetToaChua())
            {
                return;
            }
            if (!KiemTraDaXuatChua())
            {
                return;
            }
            //▲====== #016
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

            //Xét Phải Dược Sĩ Sửa Không
            if (DuocSi_IsEditingToaThuoc)
            {
                btDuocSiEditIsEnabled = true;
            }
            else
            {
                if (!IsUpdateWithoutChangeDoctorIDAndDatetime)
                {
                    if (CheckToaThuocDuocPhepCapNhat(DuocSi_IsEditingToaThuoc) == false)
                    {
                        btnUpdateIsEnabled = false;
                    }
                    else
                    {
                        btnUpdateIsEnabled = true;
                    }
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
            }

            IsWaitingSaveDuocSiEdit = false;

            BackupCurPrescriptionItem();
            GetRemainingPrescriptionDetailsByPrescriptID(LatestePrecriptions.PrescriptID, true);
            AddNewBlankDrugIntoPrescriptObjectNew();
            /*▼====: #004*/
            LatestePrecriptions.ResetDataChanged();
            /*▲====: #004*/
        }

        private void UpdateButtonsOnNewPrescriptBasedOnPrev(bool bPhatHanhLai)
        {
            IsEnabled = true;
            btnUndoIsEnabled = true;
            btnEditIsEnabled = false;
            btnCreateAndCopyIsEnabled = false;

            //Xét Phải Dược Sĩ Sửa Không
            if (DuocSi_IsEditingToaThuoc)
            {
                btDuocSiEditIsEnabled = true;
            }
            else
            {
                btnUpdateIsEnabled = false;
                btChonChanDoanIsEnabled = true;
                btnCreateNewIsEnabled = false;
                btnSaveAddNewIsEnabled = true;
                //bntSaveAsIsEnabled = true;
                btnCopyToIsEnabled = false;
                IsEnabledPrint = false;
            }

            IsWaitingSaveDuocSiEdit = false;

            BackupCurPrescriptionItem();
            //if (!bPhatHanhLai)
            //{
            //    LatestePrecriptions.Diagnosis = "";
            //}

            if (Globals.ServerConfigSection.CommonItems.OrganizationUseSoftware > 0)
            {
                LatestePrecriptions.HasAppointment = true;
            }
            LatestePrecriptions.ExamDate = Registration_DataStorage.CurrentPatientRegistration.ExamDate;

            AddNewBlankDrugIntoPrescriptObjectNew();

            if (bPhatHanhLai)
            {
                return;
            }

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
            if (!IsShowSummaryContent)
                LatestePrecriptions.PrescriptID = 0;

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

        //Bấm Nút sửa thì sẽ duyệt qua Toa, coi có vị phạm ràng buộc Số Ngày BH hay không. Nếu vi phạm (thẻ BH mang đến update sau) thì cảnh báo.
        //Yes thì tự động sửa. No thì không
        private bool CheckIsVuotNgayQuiDinhHI()
        {
            if (!BH)
            {
                return false;
            }

            ObservableCollection<PrescriptionDetail> PrescriptionDetails = LatestePrecriptions.PrescriptionDetails;
            if (PrescriptionDetails == null || PrescriptionDetails.Count < 1)
            {
                return false;
            }
            bool ViPhamBH = false;
            foreach (var item in PrescriptionDetails)
            {
                if ((item.IsDrugNotInCat == false) && (item.DayRpts > xNgayBHToiDa))
                {
                    ViPhamBH = true;
                    break;
                }
            }
            return ViPhamBH;
        }

        private void Prescriptions_Update()
        {
            //IsEnabledForm = false;

            //IsWaitingCapNhatToaThuoc = true;
            IList<PrescriptionIssueHistory> lstPrescriptionIssueHistory = new List<PrescriptionIssueHistory>();
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });

            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);

            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ePrescriptionsServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginPrescriptions_Update(ObjTaoThanhToaMoi, PrecriptionsBeforeUpdate, AllowUpdateThoughReturnDrugNotEnough, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                contract.EndPrescriptions_Update(out string Result, out long NewPrescriptionID, out long IssueID, out lstPrescriptionIssueHistory, asyncResult);
                                AllowUpdateThoughReturnDrugNotEnough = false;
                                allPrescriptionIssueHistory = lstPrescriptionIssueHistory.ToObservableCollection();
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

                                                consultVM.PatientServiceRecordsGetForKhamBenh_Ext();

                                                //KMx: Sau khi lưu toa thuốc, không cần load lại danh sách chẩn đoán (21/05/2014 15:17).
                                                //IConsultationList consultListVM = Globals.GetViewModel<IConsultationList>();

                                                //consultListVM.GetDiagTrmtsByPtID_Ext();

                                            }

                                            MessageBox.Show(string.Format("{0}!", eHCMSResources.Z0998_G1_DaCNhatToaThuoc), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);

                                            break;
                                        }
                                    case "Error":
                                        {
                                            //Cộng lại dòng trống
                                            AddNewBlankDrugIntoPrescriptObjectNew();
                                            //Cộng lại dòng trống

                                            MessageBox.Show(string.Format("{0}!", eHCMSResources.A0272_G1_Msg_InfoCNhatFail), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                            break;
                                        }
                                    case "PhaiThucHien-TraPhieuTruoc":
                                        {
                                            //▼===== #033
                                            //Cộng lại dòng trống
                                            AddNewBlankDrugIntoPrescriptObjectNew();
                                            //Cộng lại dòng trống

                                            //Coroutine.BeginExecute(WarningReturnDrugNotEnough());
                                            MessageBox.Show(eHCMSResources.Z3068_G1_ToaDaXuatKhongTheCapNhat, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                            break;
                                            //▲===== #033
                                        }
                                    case "ToaNay-DaTungPhatHanh-VaSuDung":
                                        {
                                            //Cộng lại dòng trống
                                            AddNewBlankDrugIntoPrescriptObjectNew();
                                            //Cộng lại dòng trống

                                            MessageBox.Show(string.Format("{0}!", eHCMSResources.Z0688_G1_ToaDaPhHanhRoi) + Environment.NewLine + string.Format("{0}!", eHCMSResources.Z0870_G1_KgTheCNhatToa), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
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

                                            MessageBox.Show(string.Format("{0}!", eHCMSResources.A0272_G1_Msg_InfoCNhatFail), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
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
            if (Globals.IsLockRegistration(Registration_DataStorage.CurrentPatientRegistration.RegLockFlag, eHCMSResources.Z1067_G1_CNhatLoiDanBSi))
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


        //Khóa mở S,Tr,C,T TextBox theo Combo chọn
        public void cbxChooseDose_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            KeyEnabledComboBox Ctr = (sender as KeyEnabledComboBox);
            if (Ctr != null)
            {
                if (Ctr.SelectedItemEx != null)
                {
                    PrescriptionDetail Objtmp = (Ctr.DataContext) as PrescriptionDetail;

                    if (Objtmp != null && Objtmp.HasSchedules == true)
                    {
                        MessageBox.Show(string.Format(eHCMSResources.Z0989_G1_ThuocDaCoCDinhUongTheoTuan, Objtmp.SelectedDrugForPrescription.BrandName.Trim()));
                        return;
                    }

                    ChooseDose ObjChooseDose = Ctr.SelectedItemEx as ChooseDose;
                    SetEnableDisalbeInputDose(ObjChooseDose, Objtmp);

                    if (ObjChooseDose.ID > 0)
                    {
                        SetValueFollowComboDose(ObjChooseDose, Objtmp);
                        if (Objtmp != null && Objtmp.DayRpts > 0)
                        {
                            CalcTotalQtyForDrugItem(Objtmp);
                        }
                    }
                }
            }
        }

        public void cbxChooseDoseForm_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            KeyEnabledComboBox Ctr = (sender as KeyEnabledComboBox);
            if (Ctr != null)
            {
                if (Ctr.SelectedItemEx != null)
                {
                    PrescriptionDetail Objtmp = (Ctr.DataContext) as PrescriptionDetail;

                    if (Objtmp != null && Objtmp.HasSchedules == true)
                    {
                        MessageBox.Show(string.Format(eHCMSResources.Z0989_G1_ThuocDaCoCDinhUongTheoTuan, Objtmp.SelectedDrugForPrescription.BrandName.Trim()));
                        return;
                    }

                    ChooseDose ObjChooseDose = Ctr.SelectedItemEx as ChooseDose;
                    SetEnableDisalbeInputDose(ObjChooseDose, Objtmp);

                    if (ObjChooseDose.ID > 0)
                    {
                        SetValueFollowComboDose(ObjChooseDose, Objtmp);
                        if (Objtmp != null && Objtmp.DayRpts > 0)
                        {
                            CalcTotalQtyForDrugItem(Objtmp);
                        }
                    }
                }
            }
        }


        private float ChangeDoseStringToFloat(string value)
        {
            float result = 0;
            try
            {
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
            }
            catch
            {
                Globals.ShowMessage(eHCMSResources.Z1069_G1_LieuDungKgHopLe, eHCMSResources.G0442_G1_TBao);
                return 0;
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

        private void ChangeDosage(string value, object Obj)
        {
            PrescriptionDetail Objtmp = Obj as PrescriptionDetail;
            if (Objtmp != null && (value == null || Objtmp.dosageStr.ToLower() != value.ToLower()))
            {
                if (Objtmp.Validate())
                {
                    if (Objtmp.HasSchedules == false)
                    {
                        Objtmp.dosage = ChangeDoseStringToFloat(Objtmp.dosageStr);
                        if (Objtmp.dosage == 0)
                        {
                            Objtmp.dosageStr = "0";
                        }
                        SetValueFollowComboDose(Objtmp.ChooseDose, Objtmp);
                        CalcTotalQtyForDrugItem(Objtmp);
                    }
                    else
                    {
                        Objtmp.dosageStr = "0";
                        Objtmp.dosage = 0;
                        MessageBox.Show(string.Format(eHCMSResources.Z0989_G1_ThuocDaCoCDinhUongTheoTuan, Objtmp.SelectedDrugForPrescription.BrandName.Trim()));
                    }
                }
            }
        }

        //public bool Valid

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

        private void ChangeMDose(string value, object Obj)
        {
            PrescriptionDetail Objtmp = (PrescriptionDetail)this.grdPrescription.SelectedItem;
            if (Objtmp != null && (value == null || Objtmp.MDoseStr.ToLower() != value.ToLower()))
            {
                Objtmp.MDose = ChangeDoseStringToFloat(Objtmp.MDoseStr);
                if (Objtmp.HasSchedules == false)
                {
                    CalcTotalQtyForDrugItem(Objtmp);
                }
                else
                {
                    Objtmp.MDoseStr = "0";
                    Objtmp.MDose = 0;
                    MessageBox.Show(string.Format(eHCMSResources.Z0989_G1_ThuocDaCoCDinhUongTheoTuan, Objtmp.SelectedDrugForPrescription.BrandName.Trim()));
                }
            }
        }

        private void ChangeNDose(string value, object Obj)
        {
            PrescriptionDetail Objtmp = (PrescriptionDetail)this.grdPrescription.SelectedItem;
            if (Objtmp != null && (value == null || Objtmp.NDoseStr.ToLower() != value.ToLower()))
            {
                Objtmp.NDose = ChangeDoseStringToFloat(Objtmp.NDoseStr);
                if (Objtmp.HasSchedules == false)
                {
                    CalcTotalQtyForDrugItem(Objtmp);
                }
                else
                {
                    Objtmp.NDoseStr = "0";
                    Objtmp.NDose = 0;
                    MessageBox.Show(string.Format(eHCMSResources.Z0989_G1_ThuocDaCoCDinhUongTheoTuan, Objtmp.SelectedDrugForPrescription.BrandName.Trim()));
                }
            }
        }

        private void ChangeEDose(string value, object Obj)
        {
            PrescriptionDetail Objtmp = (PrescriptionDetail)this.grdPrescription.SelectedItem;
            if (Objtmp != null && (value == null || Objtmp.EDoseStr.ToLower() != value.ToLower()))
            {
                Objtmp.EDose = ChangeDoseStringToFloat(Objtmp.EDoseStr);
                if (Objtmp.HasSchedules == false)
                {
                    CalcTotalQtyForDrugItem(Objtmp);
                }
                else
                {
                    Objtmp.EDoseStr = "0";
                    Objtmp.EDose = 0;
                    MessageBox.Show(string.Format(eHCMSResources.Z0989_G1_ThuocDaCoCDinhUongTheoTuan, Objtmp.SelectedDrugForPrescription.BrandName.Trim()));
                }
            }
        }

        private void ChangeADose(string value, object Obj)
        {
            PrescriptionDetail Objtmp = (PrescriptionDetail)this.grdPrescription.SelectedItem;
            if (Objtmp != null && (value == null || Objtmp.ADoseStr.ToLower() != value.ToLower()))
            {
                Objtmp.ADose = ChangeDoseStringToFloat(Objtmp.ADoseStr);
                if (Objtmp.HasSchedules == false)
                {
                    CalcTotalQtyForDrugItem(Objtmp);
                }
                else
                {
                    Objtmp.ADoseStr = "0";
                    Objtmp.ADose = 0;
                    MessageBox.Show(string.Format(eHCMSResources.Z0989_G1_ThuocDaCoCDinhUongTheoTuan, Objtmp.SelectedDrugForPrescription.BrandName.Trim()));
                }
            }
        }


        //KMx: Sau khi kiểm tra, thấy hàm này không còn sử dụng nữa (10/06/2014 15:21)
        //private void SetValueFollowComboDoseOld(ChooseDose ObjChooseDose, PrescriptionDetail Objtmp)
        //{
        //    if (Objtmp != null)
        //    {
        //        if (ObjChooseDose != null)
        //        {
        //            switch (ObjChooseDose.ID)
        //            {
        //                case 1:
        //                    Objtmp.MDose = Objtmp.dosage;
        //                    Objtmp.ADose = Objtmp.dosage;
        //                    Objtmp.EDose = Objtmp.dosage;
        //                    Objtmp.NDose = Objtmp.dosage;

        //                    Objtmp.MDoseStr = Objtmp.dosageStr;
        //                    Objtmp.ADoseStr = Objtmp.dosageStr;
        //                    Objtmp.EDoseStr = Objtmp.dosageStr;
        //                    Objtmp.NDoseStr = Objtmp.dosageStr;
        //                    break;
        //                case 2:
        //                    {
        //                        Objtmp.MDose = Objtmp.dosage;
        //                        Objtmp.ADose = Objtmp.dosage;
        //                        Objtmp.EDose = Objtmp.dosage;
        //                        Objtmp.NDose = 0;

        //                        Objtmp.MDoseStr = Objtmp.dosageStr;
        //                        Objtmp.ADoseStr = Objtmp.dosageStr;
        //                        Objtmp.EDoseStr = Objtmp.dosageStr;
        //                        Objtmp.NDoseStr = "0";
        //                        break;
        //                    }
        //                case 3:
        //                    Objtmp.MDose = Objtmp.dosage;
        //                    Objtmp.ADose = Objtmp.dosage;
        //                    Objtmp.EDose = 0;
        //                    Objtmp.NDose = Objtmp.dosage;

        //                    Objtmp.MDoseStr = Objtmp.dosageStr;
        //                    Objtmp.ADoseStr = Objtmp.dosageStr;
        //                    Objtmp.EDoseStr = "0";
        //                    Objtmp.NDoseStr = Objtmp.dosageStr;
        //                    break;
        //                case 4:
        //                    Objtmp.MDose = Objtmp.dosage;
        //                    Objtmp.ADose = 0;
        //                    Objtmp.EDose = Objtmp.dosage;
        //                    Objtmp.NDose = Objtmp.dosage;

        //                    Objtmp.MDoseStr = Objtmp.dosageStr;
        //                    Objtmp.ADoseStr = "0";
        //                    Objtmp.EDoseStr = Objtmp.dosageStr;
        //                    Objtmp.NDoseStr = Objtmp.dosageStr;
        //                    break;
        //                case 5:
        //                    Objtmp.MDose = 0;
        //                    Objtmp.ADose = Objtmp.dosage;
        //                    Objtmp.EDose = Objtmp.dosage;
        //                    Objtmp.NDose = Objtmp.dosage;

        //                    Objtmp.MDoseStr = "0";
        //                    Objtmp.ADoseStr = Objtmp.dosageStr;
        //                    Objtmp.EDoseStr = Objtmp.dosageStr;
        //                    Objtmp.NDoseStr = Objtmp.dosageStr;
        //                    break;
        //                case 6:
        //                    Objtmp.MDose = Objtmp.dosage;
        //                    Objtmp.ADose = Objtmp.dosage;
        //                    Objtmp.EDose = 0;
        //                    Objtmp.NDose = 0;

        //                    Objtmp.MDoseStr = Objtmp.dosageStr;
        //                    Objtmp.ADoseStr = Objtmp.dosageStr;
        //                    Objtmp.EDoseStr = "0";
        //                    Objtmp.NDoseStr = "0";
        //                    break;
        //                case 7:
        //                    Objtmp.MDose = Objtmp.dosage;
        //                    Objtmp.ADose = 0;
        //                    Objtmp.EDose = Objtmp.dosage;
        //                    Objtmp.NDose = 0;

        //                    Objtmp.MDoseStr = Objtmp.dosageStr;
        //                    Objtmp.ADoseStr = "0";
        //                    Objtmp.EDoseStr = Objtmp.dosageStr;
        //                    Objtmp.NDoseStr = "0";
        //                    break;
        //                case 8:
        //                    Objtmp.MDose = Objtmp.dosage;
        //                    Objtmp.ADose = 0;
        //                    Objtmp.EDose = 0;
        //                    Objtmp.NDose = Objtmp.dosage;

        //                    Objtmp.MDoseStr = Objtmp.dosageStr;
        //                    Objtmp.ADoseStr = "0";
        //                    Objtmp.EDoseStr = "0";
        //                    Objtmp.NDoseStr = Objtmp.dosageStr;
        //                    break;
        //                case 9:
        //                    Objtmp.MDose = 0;
        //                    Objtmp.ADose = Objtmp.dosage;
        //                    Objtmp.EDose = Objtmp.dosage;
        //                    Objtmp.NDose = 0;

        //                    Objtmp.MDoseStr = "0";
        //                    Objtmp.ADoseStr = Objtmp.dosageStr;
        //                    Objtmp.EDoseStr = Objtmp.dosageStr;
        //                    Objtmp.NDoseStr = "0";
        //                    break;
        //                case 10:
        //                    Objtmp.MDose = 0;
        //                    Objtmp.ADose = Objtmp.dosage;
        //                    Objtmp.EDose = 0;
        //                    Objtmp.NDose = Objtmp.dosage;

        //                    Objtmp.MDoseStr = "0";
        //                    Objtmp.ADoseStr = Objtmp.dosageStr;
        //                    Objtmp.EDoseStr = "0";
        //                    Objtmp.NDoseStr = Objtmp.dosageStr;
        //                    break;
        //                case 11:
        //                    Objtmp.MDose = 0;
        //                    Objtmp.ADose = 0;
        //                    Objtmp.EDose = Objtmp.dosage;
        //                    Objtmp.NDose = Objtmp.dosage;

        //                    Objtmp.MDoseStr = "0";
        //                    Objtmp.ADoseStr = "0";
        //                    Objtmp.EDoseStr = Objtmp.dosageStr;
        //                    Objtmp.NDoseStr = Objtmp.dosageStr;
        //                    break;
        //                case 12:
        //                    Objtmp.MDose = Objtmp.dosage;
        //                    Objtmp.ADose = 0;
        //                    Objtmp.EDose = 0;
        //                    Objtmp.NDose = 0;

        //                    Objtmp.MDoseStr = Objtmp.dosageStr;
        //                    Objtmp.ADoseStr = "0";
        //                    Objtmp.EDoseStr = "0";
        //                    Objtmp.NDoseStr = "0";
        //                    break;
        //                case 13:
        //                    Objtmp.MDose = 0;
        //                    Objtmp.ADose = Objtmp.dosage;
        //                    Objtmp.EDose = 0;
        //                    Objtmp.NDose = 0;

        //                    Objtmp.MDoseStr = "0";
        //                    Objtmp.ADoseStr = Objtmp.dosageStr;
        //                    Objtmp.EDoseStr = "0";
        //                    Objtmp.NDoseStr = "0";
        //                    break;
        //                case 14:
        //                    Objtmp.MDose = 0;
        //                    Objtmp.ADose = 0;
        //                    Objtmp.EDose = Objtmp.dosage;
        //                    Objtmp.NDose = 0;

        //                    Objtmp.MDoseStr = "0";
        //                    Objtmp.ADoseStr = "0";
        //                    Objtmp.EDoseStr = Objtmp.dosageStr;
        //                    Objtmp.NDoseStr = "0";
        //                    break;
        //                case 15:
        //                    Objtmp.MDose = 0;
        //                    Objtmp.ADose = 0;
        //                    Objtmp.EDose = 0;
        //                    Objtmp.NDose = Objtmp.dosage;

        //                    Objtmp.MDoseStr = "0";
        //                    Objtmp.ADoseStr = "0";
        //                    Objtmp.EDoseStr = "0";
        //                    Objtmp.NDoseStr = Objtmp.dosageStr;
        //                    break;
        //            }

        //        }
        //    }
        //}

        private void SetEnableDisalbeInputDose(ChooseDose ObjChooseDose, PrescriptionDetail Objtmp)
        {
            if (grdPrescription != null)
            {
                int indexRow = grdPrescription.SelectedIndex;
                if (indexRow < 0)
                {
                    return;
                }

                AxTextBox Sang = grdPrescription.Columns[(int)DataGridCol.MDOSE].GetCellContent(grdPrescription.SelectedItem) as AxTextBox;
                AxTextBox Trua = grdPrescription.Columns[(int)DataGridCol.NDOSE].GetCellContent(grdPrescription.SelectedItem) as AxTextBox;
                AxTextBox Chieu = grdPrescription.Columns[(int)DataGridCol.ADOSE].GetCellContent(grdPrescription.SelectedItem) as AxTextBox;
                AxTextBox Toi = grdPrescription.Columns[(int)DataGridCol.EDOSE].GetCellContent(grdPrescription.SelectedItem) as AxTextBox;

                if (indexRow >= 0)
                {
                    if (Objtmp != null)
                    {
                        if (Objtmp.ChooseDose != null)
                        {
                            if (Objtmp.dosage <= 0)
                            {
                                Objtmp.dosage = 0;
                            }
                            switch (ObjChooseDose.ID)
                            {
                                case 1:
                                    Objtmp.MDose = Objtmp.dosage;
                                    Objtmp.ADose = Objtmp.dosage;
                                    Objtmp.EDose = Objtmp.dosage;
                                    Objtmp.NDose = Objtmp.dosage;
                                    break;

                                case 2:
                                    Objtmp.MDose = Objtmp.dosage;
                                    Objtmp.ADose = Objtmp.dosage;
                                    Objtmp.EDose = Objtmp.dosage;
                                    Objtmp.NDose = 0;
                                    break;

                                case 3:
                                    Objtmp.MDose = Objtmp.dosage;
                                    Objtmp.ADose = Objtmp.dosage;
                                    Objtmp.EDose = 0;
                                    Objtmp.NDose = Objtmp.dosage;
                                    break;

                                case 4:
                                    Objtmp.MDose = Objtmp.dosage;
                                    Objtmp.ADose = 0;
                                    Objtmp.EDose = Objtmp.dosage;
                                    Objtmp.NDose = Objtmp.dosage;
                                    break;

                                case 5:
                                    Objtmp.MDose = 0;
                                    Objtmp.ADose = Objtmp.dosage;
                                    Objtmp.EDose = Objtmp.dosage;
                                    Objtmp.NDose = Objtmp.dosage;
                                    break;

                                case 6:
                                    Objtmp.MDose = Objtmp.dosage;
                                    Objtmp.ADose = Objtmp.dosage;
                                    Objtmp.EDose = 0;
                                    Objtmp.NDose = 0;
                                    break;

                                case 7:
                                    Objtmp.MDose = Objtmp.dosage;
                                    Objtmp.ADose = 0;
                                    Objtmp.EDose = Objtmp.dosage;
                                    Objtmp.NDose = 0;
                                    break;

                                case 8:
                                    Objtmp.MDose = Objtmp.dosage;
                                    Objtmp.ADose = 0;
                                    Objtmp.EDose = 0;
                                    Objtmp.NDose = Objtmp.dosage;
                                    break;

                                case 9:
                                    Objtmp.MDose = 0;
                                    Objtmp.ADose = Objtmp.dosage;
                                    Objtmp.EDose = Objtmp.dosage;
                                    Objtmp.NDose = 0;
                                    break;

                                case 10:
                                    Objtmp.MDose = 0;
                                    Objtmp.ADose = Objtmp.dosage;
                                    Objtmp.EDose = 0;
                                    Objtmp.NDose = Objtmp.dosage;
                                    break;

                                case 11:
                                    Objtmp.MDose = 0;
                                    Objtmp.ADose = 0;
                                    Objtmp.EDose = Objtmp.dosage;
                                    Objtmp.NDose = Objtmp.dosage;
                                    break;

                                case 12:
                                    Objtmp.MDose = Objtmp.dosage;
                                    Objtmp.ADose = 0;
                                    Objtmp.EDose = 0;
                                    Objtmp.NDose = 0;
                                    break;

                                case 13:
                                    Objtmp.MDose = 0;
                                    Objtmp.ADose = Objtmp.dosage;
                                    Objtmp.EDose = 0;
                                    Objtmp.NDose = 0;
                                    break;

                                case 14:
                                    Objtmp.MDose = 0;
                                    Objtmp.ADose = 0;
                                    Objtmp.EDose = Objtmp.dosage;
                                    Objtmp.NDose = 0;
                                    break;

                                case 15:
                                    Objtmp.MDose = 0;
                                    Objtmp.ADose = 0;
                                    Objtmp.EDose = 0;
                                    Objtmp.NDose = Objtmp.dosage;
                                    break;
                            }
                        }
                    }
                }
            }
        }

        private void SetValueFollowComboDose(ChooseDose ObjChooseDose, PrescriptionDetail Objtmp)
        {
            if (Objtmp != null)
            {
                if (ObjChooseDose != null)
                {
                    Objtmp.MDoseStr = "0";
                    Objtmp.NDoseStr = "0";
                    Objtmp.ADoseStr = "0";
                    Objtmp.EDoseStr = "0";
                    switch (ObjChooseDose.ID)
                    {
                        case 1://S

                            Objtmp.MDoseStr = Objtmp.dosageStr;
                            break;
                        case 2://Tr 
                            {
                                Objtmp.NDoseStr = Objtmp.dosageStr;
                                break;
                            }
                        case 3://C
                            {
                                Objtmp.ADoseStr = Objtmp.dosageStr;
                                break;
                            }
                        case 4://T
                            {
                                Objtmp.EDoseStr = Objtmp.dosageStr;
                                break;
                            }
                        case 5://S Tr
                            {
                                Objtmp.MDoseStr = Objtmp.dosageStr;
                                Objtmp.NDoseStr = Objtmp.dosageStr;
                                break;
                            }
                        case 6://S C
                            {
                                Objtmp.MDoseStr = Objtmp.dosageStr;
                                Objtmp.ADoseStr = Objtmp.dosageStr;
                                break;
                            }
                        case 7://S T
                            {
                                Objtmp.MDoseStr = Objtmp.dosageStr;
                                Objtmp.EDoseStr = Objtmp.dosageStr;
                                break;
                            }

                        case 8://Tr C
                            {
                                Objtmp.NDoseStr = Objtmp.dosageStr;
                                Objtmp.ADoseStr = Objtmp.dosageStr;
                                break;
                            }
                        case 9://Tr T
                            {
                                Objtmp.NDoseStr = Objtmp.dosageStr;
                                Objtmp.EDoseStr = Objtmp.dosageStr;
                                break;
                            }

                        case 10://C T
                            {
                                Objtmp.ADoseStr = Objtmp.dosageStr;
                                Objtmp.EDoseStr = Objtmp.dosageStr;
                                break;
                            }
                        case 11://S Tr C
                            {
                                Objtmp.MDoseStr = Objtmp.dosageStr;
                                Objtmp.NDoseStr = Objtmp.dosageStr;
                                Objtmp.ADoseStr = Objtmp.dosageStr;
                                break;
                            }

                        case 12://S Tr T
                            {
                                Objtmp.MDoseStr = Objtmp.dosageStr;
                                Objtmp.NDoseStr = Objtmp.dosageStr;
                                Objtmp.EDoseStr = Objtmp.dosageStr;
                                break;
                            }
                        case 13://S C T
                            {
                                Objtmp.MDoseStr = Objtmp.dosageStr;
                                Objtmp.ADoseStr = Objtmp.dosageStr;
                                Objtmp.EDoseStr = Objtmp.dosageStr;
                                break;
                            }
                        case 14://Tr C T
                            {
                                Objtmp.NDoseStr = Objtmp.dosageStr;
                                Objtmp.ADoseStr = Objtmp.dosageStr;
                                Objtmp.EDoseStr = Objtmp.dosageStr;
                                break;
                            }
                        case 15://S Tr C T
                            {
                                Objtmp.MDoseStr = Objtmp.dosageStr;
                                Objtmp.NDoseStr = Objtmp.dosageStr;
                                Objtmp.ADoseStr = Objtmp.dosageStr;
                                Objtmp.EDoseStr = Objtmp.dosageStr;
                                break;
                            }
                    }
                }
            }
        }


        #endregion

        #region Tạo Toa Mới

        //Chọn 1 chẩn đoán để ra toa
        public void Handle(DiagnosisTreatmentSelectedEvent<DiagnosisTreatment> message)
        {
            if (message != null)
            {
                ObjDiagnosisTreatment_Current = message.DiagnosisTreatment.DeepCopy();

                string cd = ObjDiagnosisTreatment_Current.DiagnosisFinal.Trim();
                if (string.IsNullOrEmpty(cd))
                {
                    cd = ObjDiagnosisTreatment_Current.Diagnosis.Trim();
                }

                if (LatestePrecriptions == null) LatestePrecriptions = new Prescription();

                LatestePrecriptions.Diagnosis = cd;
                LatestePrecriptions.ServiceRecID = ObjDiagnosisTreatment_Current.ServiceRecID;
                if (LatestePrecriptions.PrescriptionIssueHistory == null)
                {
                    LatestePrecriptions.PrescriptionIssueHistory = new PrescriptionIssueHistory();
                }

                // TxD 26/12/2014: The Bug is here ie. Registration_DataStorage.CurrentPatientRegistrationDetail is null
                //                  Check out why it is not set yet or is it ever set .... then fix it
                HisIDVisibility = Registration_DataStorage.CurrentPatientRegistrationDetail.HisID != null && Registration_DataStorage.CurrentPatientRegistrationDetail.HIBenefit > 0
                && Registration_DataStorage.CurrentPatientRegistrationDetail.HisID.Value > 0 ? true : false;
                isHisID = HisIDVisibility;

                LatestePrecriptions.PrescriptionIssueHistory.DeptLocID = Globals.DeptLocation.DeptLocationID;
                //Cụ thể  DV nào
                if (Registration_DataStorage.CurrentPatientRegistration != null)
                {
                    if (Registration_DataStorage.CurrentPatientRegistration.V_RegistrationType == AllLookupValues.RegistrationType.NGOAI_TRU)
                    {
                        if (Registration_DataStorage.CurrentPatientRegistrationDetail != null && Registration_DataStorage.CurrentPatientRegistrationDetail.PtRegDetailID > 0)
                        {
                            LatestePrecriptions.PrescriptionIssueHistory.PtRegDetailID = ObjDiagnosisTreatment_Current.PtRegDetailID;
                        }
                        else
                        {
                            MessageBox.Show(eHCMSResources.A0705_G1_Msg_KhTheRaToaNgTru);
                            return;
                        }
                    }
                    else/*Khám VIP, Khám Cho Nội Trú*/
                    {
                        LatestePrecriptions.PrescriptionIssueHistory.PtRegDetailID = 0;
                    }
                }
                else
                {
                    MessageBox.Show(eHCMSResources.A0704_G1_Msg_InfoKhTheRaToa);
                    return;
                }
                //Cụ thể  DV nào
            }
        }

        //Dinh them phan hen ngay tai kham
        private int CheckAppDate()
        {
            if (LatestePrecriptions.NDay.GetValueOrDefault() > 0)
            {
                return LatestePrecriptions.NDay.GetValueOrDefault();
            }
            else
            {
                int maxDay = 0;
                foreach (var item in LatestePrecriptions.PrescriptionDetails)
                {
                    if (maxDay < item.DayRpts)
                    {
                        maxDay = Convert.ToInt32(item.DayRpts);
                    }
                }
                return maxDay;
            }
        }

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
            ObjTaoThanhToaMoi.PrescriptionIssueHistory.HisID = LatestePrecriptions.PrescriptionIssueHistory.HisID;
            //▼====== #013
            ObjTaoThanhToaMoi.PrescriptionIssueHistory.IsOutCatConfirmed = IsOutCatConfirmed;
            //▲====== #013
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
                        contract.BeginPrescriptions_Add((short)Globals.ServerConfigSection.CommonItems.NumberTypePrescriptions_Rule, ObjTaoThanhToaMoi, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                if (contract.EndPrescriptions_Add(out PrescriptionID, out IssueID, out OutError, out lstPrescriptionIssueHistory, asyncResult))
                                {
                                    allPrescriptionIssueHistory = lstPrescriptionIssueHistory.ToObservableCollection();
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

                                        consultVM.PatientServiceRecordsGetForKhamBenh_Ext();
                                        var homeVm = Globals.GetViewModel<IHome>();
                                        if (homeVm.OutstandingTaskContent != null && homeVm.OutstandingTaskContent is IConsultationOutstandingTask)
                                        {
                                            ((IConsultationOutstandingTask)homeVm.OutstandingTaskContent).SearchRegistrationListForOST();
                                        }

                                        //KMx: Sau khi lưu toa thuốc, không cần load lại danh sách chẩn đoán (21/05/2014 15:17).
                                        //IConsultationList consultListVM = Globals.GetViewModel<IConsultationList>();

                                        //consultListVM.GetDiagTrmtsByPtID_Ext();
                                    }

                                    MessageBox.Show(eHCMSResources.K0175_G1_ToaThuocDaLuuOk);

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
        public void Handle(ePrescriptionDoubleClickEvent message)
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
                    if (message.SelectedPrescription.NDay > 0)
                    {
                        LatestePrecriptions.NDay = message.SelectedPrescription.NDay;
                        CtrtbSoTuan.Text = message.SelectedPrescription.NDay.ToString();
                        NotifyOfPropertyChange(() => LatestePrecriptions.NDay);
                    }

                }
                else
                {
                    LatestePrecriptions = message.SelectedPrescription.DeepCopy();
                }

                NotifyOfPropertyChange(() => LatestePrecriptions.PrescriptionDetails);

                //if (CheckToaThuocDuocPhepCapNhat(DuocSi_IsEditingToaThuoc) == false)
                //{
                //    btnEditIsEnabled = false;
                //}

                if (ObjDiagnosisTreatment_Current != null && LatestePrecriptions != null && ObjDiagnosisTreatment_Current.ServiceRecID == LatestePrecriptions.ServiceRecID
                    && CheckToaThuocDuocPhepCapNhat(DuocSi_IsEditingToaThuoc))
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
                if (PrescripState != PrescriptionState.NewPrescriptionState)
                {
                    ValidateExpiredPrescription(LatestePrecriptions);
                }


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

        public void Handle(SelectListDrugDoubleClickEvent message)
        {
            if (message != null)
            {
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

                NewPrecriptions = new Prescription();

                //▼====== #002 Thêm cảnh báo nếu như bệnh nhân không có DiagnosisFinal và Diagnosis mà lại tạo toa từ thuốc đã dùng.
                if (ObjDiagnosisTreatment_Current.Diagnosis == null || ObjDiagnosisTreatment_Current.DiagnosisFinal == null)
                {
                    MessageBox.Show(eHCMSResources.Z2298_G1_ChCoChanDoanKhongRaToa);
                    return;
                }
                //▲====== #002

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
                NewPrecriptions.V_PrescriptionType = CurrentPrescriptionType == null ? null : (long?)CurrentPrescriptionType.LookupID;

                NewPrecriptions.PrescriptionIssueHistory = new PrescriptionIssueHistory();
                NewPrecriptions.PrescriptionIssueHistory.DeptLocID = Globals.DeptLocation.DeptLocationID;

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

                LatestePrecriptions = NewPrecriptions.DeepCopy();

                for (int i = 0; i < message.GetDrugForSellVisitorList.Count; i++)
                {
                    AddNewBlankDrugIntoPrescriptObjectNew(i, message.GetDrugForSellVisitorList[i]);
                }

                AddNewBlankDrugIntoPrescriptObjectNew();
            }
        }


        #endregion

        #region printing member

        //▼====== #014
        public bool IsPsychotropicDrugs = false;
        public bool IsFuncfoodsOrCosmetics = false;
        //▲====== #014
        public void btnPreview()
        {
            /*▼====: #005*/
            if (PrecriptionsForPrint.IssueID <= 0)
            {
                MessageBox.Show(eHCMSResources.Z2300_G1_KhongCoToaThuocDeXemIn, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
            /*▲====: #005*/

            //▼====== #014: Kiểm tra toa thuốc có chứa thuốc hướng thần hoặc TPCN/MP ko (TPCN/MP = Thực phẩm chức năng/ Mỹ phẩm).
            //              RefPharmacyDrugCatID = 4 là thuốc hướng thần
            //              RefPharmacyDrugCatID = 5 là thực phẩm chức năng
            //              RefPharmacyDrugCatID = 6 là mỹ phẩm 
            //              Nằm trong bảng RefPharmacyDrugCategory và gom 5, 6 thành 1 toa
            //              => Nếu sau này có thay đổi trong bảng RefPharmacyDrugCategory thì chỉ cần thay đổi điều kiện if trong 2 cái foreach.
            // 20201204 TNHX: Thêm kiểm tra toa gây nghiện
            foreach (var PsychotropicDrugs in LatestePrecriptions.PrescriptionDetails)
            {
                if (PsychotropicDrugs.RefGenericDrugDetail.RefPharmacyDrugCatID == 2)
                {
                    IsAddictive = true;
                    break;
                }
            }

            foreach (var PsychotropicDrugs in LatestePrecriptions.PrescriptionDetails)
            {
                if (PsychotropicDrugs.RefGenericDrugDetail.RefPharmacyDrugCatID == 4)
                {
                    IsPsychotropicDrugs = true;
                    break;
                }
            }

            foreach (var FuncfoodsOrCosmetics in LatestePrecriptions.PrescriptionDetails)
            {
                if (FuncfoodsOrCosmetics.RefGenericDrugDetail.RefPharmacyDrugCatID == 5
                    || FuncfoodsOrCosmetics.RefGenericDrugDetail.RefPharmacyDrugCatID == 6)
                {
                    IsFuncfoodsOrCosmetics = true;
                    break;
                }
            }
            foreach (var FuncPharmacyDrugs in LatestePrecriptions.PrescriptionDetails)
            {
                if (!FuncPharmacyDrugs.BeOfHIMedicineList)
                {
                    HasPharmacyDrug = true;
                    break;
                }
            }
            
            //▲====== #014
            Action<ICommonPreviewView> onInitDlg = delegate (ICommonPreviewView proAlloc)
            {
                //proAlloc.IssueID = (int)PrecriptionsForPrint.IssueID;
                proAlloc.IssueID = PrecriptionsForPrint.IssueID;
                //▼====== #014
                proAlloc.IsPsychotropicDrugs = IsPsychotropicDrugs;
                proAlloc.IsFuncfoodsOrCosmetics = IsFuncfoodsOrCosmetics;
                proAlloc.HasPharmacyDrug = HasPharmacyDrug;
                proAlloc.IsYHCTPrescript = CheckYHCT();
                //▲====== #014
                if (Globals.ServerConfigSection.CommonItems.OrganizationUseSoftware == 0)
                {
                    proAlloc.eItem = ReportName.CONSULTATION_TOATHUOC;
                }
                else
                {
                    //Mẫu toa thuốc của phòng mạch bác Huân.
                    proAlloc.eItem = ReportName.CONSULTATION_TOATHUOC_PRIVATE;
                }
                proAlloc.parTypeOfForm = Globals.ServerConfigSection.ConsultationElements.IsSeparatePsychotropicPrescription == true ? 1 : 0;
                /*▼====: #001*/
                if (LatestePrecriptions == null || LatestePrecriptions.PrescriptionDetails == null || LatestePrecriptions.PrescriptionDetails.Count == 0)
                    proAlloc.parTypeOfForm = 0;
                /*▲====: #001*/
            };
            GlobalsNAV.ShowDialog<ICommonPreviewView>(onInitDlg, null, false, true, Globals.GetDefaultDialogViewSize());
            //▼====== #014: Sau khi xem in sẽ trả cờ lại giá trị mặc định, tránh trường hợp thay đổi toa thuốc nhưng cờ vẫn còn set true.
            IsPsychotropicDrugs = false;
            IsFuncfoodsOrCosmetics = false;
            IsAddictive = false;
            HasPharmacyDrug = false;
            //▲===== #014
            //▼====== #036
            if (Globals.ServerConfigSection.ConsultationElements.ListICDShowAdvisoryVotes != "")
            {
                string CurICD10Main = CS_DS.refIDC10List.Where(x => x.IsMain).FirstOrDefault().ICD10Code;
                if (Globals.ServerConfigSection.ConsultationElements.ListICDShowAdvisoryVotes.Contains(CurICD10Main))
                {
                    void onInitDlg1(ICommonPreviewView proAlloc)
                    {
                        proAlloc.eItem = ReportName.XRpt_AdvisoryVotes;
                        proAlloc.PatientName = Registration_DataStorage.CurrentPatient.FullName;
                        proAlloc.DOB = Registration_DataStorage.CurrentPatient.DOBText;
                        proAlloc.AdmissionDate = Registration_DataStorage.CurrentPatientRegistration.ExamDate;
                        proAlloc.PatientCode = Registration_DataStorage.CurrentPatient.PatientCode;
                        if (CS_DS != null && CS_DS.refIDC10List != null)
                        {
                            DiagnosisIcd10Items MainICD10 = CS_DS.refIDC10List.Where(x => x.IsMain).FirstOrDefault();
                            if (MainICD10 != null && MainICD10.DiseasesReference != null && MainICD10.DiseasesReference.DiseaseNameVN != null)
                            {
                                proAlloc.Result = MainICD10.DiseasesReference.DiseaseNameVN;
                            }
                            else if (CS_DS.DiagTreatment != null)
                            {
                                proAlloc.Result = CS_DS.DiagTreatment.DiagnosisFinal;
                            }
                            else proAlloc.Result = "Chưa xác định";
                        }
                        if (CS_DS != null && CS_DS.DiagTreatment != null && CS_DS.DiagTreatment.ObjDoctorStaffID != null)
                        {
                            proAlloc.StaffName = CS_DS.DiagTreatment.ObjDoctorStaffID.FullName ?? Globals.LoggedUserAccount.Staff.FullName;
                        }
                    }
                    GlobalsNAV.ShowDialog<ICommonPreviewView>(onInitDlg1, null, false, true, Globals.GetDefaultDialogViewSize());
                }
            }
            //▲===== #036
        }


        public bool IsShowPreviewPsychotropicPrescription = Globals.ServerConfigSection.ConsultationElements.IsSeparatePsychotropicPrescription;
        public bool IsAddictive = false;
        public bool HasPharmacyDrug = false;
        public void btnPreview_GN_HT()
        {
            if (PrecriptionsForPrint.IssueID <= 0)
            {
                MessageBox.Show(eHCMSResources.Z2300_G1_KhongCoToaThuocDeXemIn, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
            foreach (var PsychotropicDrugs in LatestePrecriptions.PrescriptionDetails)
            {
                if (PsychotropicDrugs.RefGenericDrugDetail.RefPharmacyDrugCatID == 2)
                {
                    IsAddictive = true;
                    break;
                }
            }

            foreach (var PsychotropicDrugs in LatestePrecriptions.PrescriptionDetails)
            {
                if (PsychotropicDrugs.RefGenericDrugDetail.RefPharmacyDrugCatID == 4)
                {
                    IsPsychotropicDrugs = true;
                    break;
                }
            }

            if (IsPsychotropicDrugs)
            {
                // in giấy cam kết
                //Action<ICommonPreviewView> onInitDlg = delegate (ICommonPreviewView proAlloc)
                //{
                //    proAlloc.IssueID = PrecriptionsForPrint.IssueID;
                //    proAlloc.IsPsychotropicDrugs = IsPsychotropicDrugs;
                //    proAlloc.IsFuncfoodsOrCosmetics = IsFuncfoodsOrCosmetics;
                //    proAlloc.IsAddictive = IsAddictive;
                //    proAlloc.eItem = ReportName.CONSULTATION_TOATHUOC_GN_HT;
                //};
                //GlobalsNAV.ShowDialog<ICommonPreviewView>(onInitDlg, null, false, true, Globals.GetDefaultDialogViewSize());                
                // in đơn thuốc hướng thần
                void onInitDlg(ICommonPreviewView proAlloc)
                {
                    proAlloc.IssueID = PrecriptionsForPrint.IssueID;
                    proAlloc.IsPsychotropicDrugs = IsPsychotropicDrugs;
                    proAlloc.IsFuncfoodsOrCosmetics = IsFuncfoodsOrCosmetics;
                    proAlloc.IsAddictive = false;
                    proAlloc.eItem = ReportName.CONSULTATION_TOATHUOC_GN_HT;
                }
                GlobalsNAV.ShowDialog<ICommonPreviewView>(onInitDlg, null, false, true, Globals.GetDefaultDialogViewSize());
            }
            if (IsAddictive)
            {
                // in giấy cam kết
                //Action<ICommonPreviewView> onInitDlg = delegate (ICommonPreviewView proAlloc)
                //{
                //    proAlloc.IssueID = PrecriptionsForPrint.IssueID;
                //    proAlloc.IsPsychotropicDrugs = IsPsychotropicDrugs;
                //    proAlloc.IsFuncfoodsOrCosmetics = IsFuncfoodsOrCosmetics;
                //    proAlloc.IsAddictive = IsAddictive;
                //    proAlloc.eItem = ReportName.CONSULTATION_TOATHUOC_GN_HT;
                //};
                //GlobalsNAV.ShowDialog<ICommonPreviewView>(onInitDlg, null, false, true, Globals.GetDefaultDialogViewSize());
                // in đơn thuốc gây nghiện
                void onInitDlg(ICommonPreviewView proAlloc)
                {
                    proAlloc.IssueID = PrecriptionsForPrint.IssueID;
                    proAlloc.IsAddictive = IsAddictive;
                    proAlloc.IsPsychotropicDrugs = false;
                    proAlloc.eItem = ReportName.CONSULTATION_TOATHUOC_GN_HT;
                }
                GlobalsNAV.ShowDialog<ICommonPreviewView>(onInitDlg, null, false, true, Globals.GetDefaultDialogViewSize());
            }

            IsPsychotropicDrugs = false;
            IsFuncfoodsOrCosmetics = false;
            IsAddictive = false;
            HasPharmacyDrug = false;
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

            //▼====== #014: Kiểm tra toa thuốc có chứa thuốc hướng thần hoặc TPCN/MP ko (TPCN/MP = Thực phẩm chức năng/ Mỹ phẩm).
            //              RefPharmacyDrugCatID = 4 là thuốc hướng thần
            //              RefPharmacyDrugCatID = 5 là thực phẩm chức năng
            //              RefPharmacyDrugCatID = 6 là mỹ phẩm 
            //              Nằm trong bảng RefPharmacyDrugCategory và gom 5, 6 thành 1 toa
            //              => Nếu sau này có thay đổi trong bảng RefPharmacyDrugCategory thì chỉ cần thay đổi điều kiện if trong 2 cái foreach.
            foreach (var PsychotropicDrugs in LatestePrecriptions.PrescriptionDetails)
            {
                if (PsychotropicDrugs.RefGenericDrugDetail.RefPharmacyDrugCatID == 4)
                {
                    IsPsychotropicDrugs = true;
                    break;
                }
            }

            foreach (var FuncfoodsOrCosmetics in LatestePrecriptions.PrescriptionDetails)
            {
                if (FuncfoodsOrCosmetics.RefGenericDrugDetail.RefPharmacyDrugCatID == 5
                    || FuncfoodsOrCosmetics.RefGenericDrugDetail.RefPharmacyDrugCatID == 6)
                {
                    IsFuncfoodsOrCosmetics = true;
                    break;
                }
            }
            var parTypeOfForm = Globals.ServerConfigSection.ConsultationElements.IsSeparatePsychotropicPrescription == true ? 1 : 0;
            /*▼====: #001*/
            if (LatestePrecriptions == null || LatestePrecriptions.PrescriptionDetails == null || LatestePrecriptions.PrescriptionDetails.Count == 0)
                parTypeOfForm = 0;
            //▲====== #014
            foreach (var FuncPharmacyDrugs in LatestePrecriptions.PrescriptionDetails)
            {
                if (!FuncPharmacyDrugs.BeOfHIMedicineList)
                {
                    HasPharmacyDrug = true;
                    break;
                }
            }
            //▼===== #034
            this.ShowBusyIndicator(eHCMSResources.Z1544_G1_DangIn);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ReportServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetXRptEPrescriptionInPdfFormat(PrecriptionsForPrint.IssueID, IsPsychotropicDrugs, IsFuncfoodsOrCosmetics
                            , parTypeOfForm, Globals.ServerConfigSection.CommonItems.OrganizationUseSoftware
                            , numOfCopies, Globals.ServerConfigSection.Hospitals.HospitalCode
                            , Globals.ServerConfigSection.ConsultationElements.PrescriptionOutPtVersion
                            , Globals.ServerConfigSection.Hospitals.PrescriptionMainRightHeader
                            , Globals.ServerConfigSection.Hospitals.PrescriptionSubRightHeader
                            , Globals.ServerConfigSection.CommonItems.ReportDepartmentOfHealth
                            , Globals.ServerConfigSection.CommonItems.ReportHospitalName
                            , Globals.ServerConfigSection.CommonItems.ReportHospitalAddress
                            //, Globals.ServerConfigSection.CommonItems.KBYTLink
                            , Globals.ServerConfigSection.CommonItems.LinkKhaoSatNgoaiTru
                            , HasPharmacyDrug
                            , Globals.ServerConfigSection.CommonItems.IsSeparatePrescription
                            , Globals.ServerConfigSection.CommonItems.ReportHospitalPhone
                            , Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndGetXRptEPrescriptionInPdfFormat(asyncResult);
                                //var printEvt = new ActiveXPrintEvt(this, PrinterType.IN_PHIEU, results, ActiveXPrintType.ByteArray, numOfCopies);
                                var printEvt = new ActiveXPrintEvt(this, PrinterType.IN_PHIEU, results, ActiveXPrintType.ByteArray, 1);
                                Globals.EventAggregator.Publish(printEvt);
                                //▼====== #014: Sau khi xem in sẽ trả cờ lại giá trị mặc định, tránh trường hợp thay đổi toa thuốc nhưng cờ vẫn còn set true.
                                IsPsychotropicDrugs = false;
                                IsFuncfoodsOrCosmetics = false;
                                IsAddictive = false;
                                HasPharmacyDrug = false;
                                //▲===== #014
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
        //▲===== #034

        #endregion

        private void CallAppointmentDialog(bool aIsPCLBookingView = false)
        {
            if (Globals.IsLockRegistration(Registration_DataStorage.CurrentPatientRegistration.RegLockFlag, eHCMSResources.T1455_G1_HBenh.ToLower()))
            {
                return;
            }
            //KMx: Không được dựa vào Registration_DataStorage.PatientServiceRecordCollection[0].PrescriptionIssueHistories. Vì nếu user chọn toa cũ thì khi hẹn bệnh sẽ bị lỗi hẹn cho toa của DK hiện tại (28/03/2016 15:34).
            //if (Registration_DataStorage.PatientServiceRecordCollection == null || Registration_DataStorage.PatientServiceRecordCollection.Count < 1
            //    || Registration_DataStorage.PatientServiceRecordCollection[0].PrescriptionIssueHistories == null || Registration_DataStorage.PatientServiceRecordCollection[0].PrescriptionIssueHistories.Count < 1)
            //{
            //    MessageBox.Show(string.Format("{0}!", eHCMSResources.A0485_G1_Msg_InfoDKChuaCoToa_KhTheHen), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
            //    return;
            //}
            //if (Registration_DataStorage.PatientServiceRecordCollection[0].PrescriptionIssueHistories[0].AppointmentID.GetValueOrDefault() <= 0)
            //{
            //    MessageBox.Show("Bạn chưa chọn hẹn tái khám nên không thể hẹn bệnh!", eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
            //    return;
            //}
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
                    contract.BeginGetAppointmentID(LatestePrecriptions.IssueID, false, Globals.DispatchCallback((asyncResult) =>
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
                            //curAppt.AppointmentID = Registration_DataStorage.PatientServiceRecordCollection[0].PrescriptionIssueHistories[0].AppointmentID.GetValueOrDefault();
                            curAppt.AppointmentID = LatestePrecriptions.AppointmentID.GetValueOrDefault();
                            if (aIsPCLBookingView && (curAppt == null || curAppt.AppointmentID == 0))
                            {
                                return;
                            }
                            Action<IAddEditAppointment> onInitDlg = delegate (IAddEditAppointment apptVm)
                            {
                                apptVm.Registration_DataStorage = Registration_DataStorage;
                                apptVm.SetCurrentAppointment(curAppt);
                                apptVm.IsPCLBookingView = aIsPCLBookingView;
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
        public void btnAppointment()
        {
            if (Globals.ServerConfigSection.OutRegisElements.IsPerformingTMVFunctionsA)
            {
                CallLoadAppoinmentDialogForTMV();
            }
            else
            {
                if (Globals.IsUse1192)
                {
                    CallLoadAppoinmentDialog();
                }
                else
                {
                    CallAppointmentDialog();
                }
            }
        }
        public void btnTreatmentProgramAppointment()
        {
            CallLoadAppoinmentDialog(true);
        }
        //▼===== #031
        public void btnTreatmentProgramAppointmentSameReg()
        {
            CallLoadAppoinmentDialog(true, true);
        }
        //▲===== #031
        //CMN: Mở Popup gõ chi tiết cuộc hẹn thay vì hẹn tự động từ toa thuốc
        private void CallLoadAppoinmentDialog(bool IsInTreatmentProgram = false, bool TreatmentAppointmentSameReg = false)
        {
            if (Globals.IsLockRegistration(Registration_DataStorage.CurrentPatientRegistration.RegLockFlag, eHCMSResources.T1455_G1_HBenh.ToLower()))
            {
                return;
            }
            if (LatestePrecriptions == null || LatestePrecriptions.IssueID <= 0)
            {
                MessageBox.Show(eHCMSResources.A0485_G1_Msg_InfoDKChuaCoToa_KhTheHen, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
            this.ShowBusyIndicator();
            var CurrentThread = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new AppointmentServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        //CMN: Lấy cuộc hẹn tái khám bằng ID toa thuốc
                        contract.BeginGetAppointmentByID(0, LatestePrecriptions.IssueID, Globals.DispatchCallback(asyncResult =>
                        {
                            try
                            {
                                var appt = contract.EndGetAppointmentByID(asyncResult);
                                PatientAppointment CurrentAppointment = new PatientAppointment();
                                if (appt != null)
                                {
                                    CurrentAppointment = appt;
                                }
                                else
                                {
                                    CurrentAppointment.DoctorStaffID = Globals.LoggedUserAccount.StaffID;
                                    CurrentAppointment.DoctorStaff = Globals.LoggedUserAccount.Staff;
                                }
                                CurrentAppointment.ApptDate = appt != null && appt.ApptDate.HasValue && appt.ApptDate != null ? appt.ApptDate.Value : LatestePrecriptions.IssuedDateTime.GetValueOrDefault(Globals.GetCurServerDateTime()).AddDays((int)LatestePrecriptions.NDay);
                                CurrentAppointment.NDay = LatestePrecriptions.NDay;
                                CurrentAppointment.Patient = Registration_DataStorage.CurrentPatient;
                                if (Registration_DataStorage.CurrentPatientRegistrationDetail != null)
                                {
                                    CurrentAppointment.ServiceRecID = Registration_DataStorage.CurrentPatientRegistrationDetail.ServiceRecID;
                                }
                                if (CurrentAppointment.ServiceRecID.GetValueOrDefault(0) == 0 &&
                                    Registration_DataStorage.CurrentPatientRegistrationDetail != null && Registration_DataStorage.PatientServiceRecordCollection != null &&
                                    Registration_DataStorage.PatientServiceRecordCollection.Any(x => x.PtRegDetailID == Registration_DataStorage.CurrentPatientRegistrationDetail.PtRegDetailID))
                                {
                                    CurrentAppointment.ServiceRecID = Registration_DataStorage.PatientServiceRecordCollection.First(x => x.PtRegDetailID == Registration_DataStorage.CurrentPatientRegistrationDetail.PtRegDetailID).ServiceRecID;
                                }
                                Action<IAddEditAppointment> onInitDlg = delegate (IAddEditAppointment apptVm)
                                {
                                    apptVm.IsInTreatmentProgram = IsInTreatmentProgram;
                                    //▼===== #031
                                    apptVm.TreatmentAppointmentSameReg = TreatmentAppointmentSameReg;
                                    //▲===== #031
                                    apptVm.Registration_DataStorage = Registration_DataStorage;
                                    apptVm.IsCreateApptFromConsultation = true;
                                    apptVm.RegistrationID = Registration_DataStorage.CurrentPatientRegistration.PtRegistrationID;
                                    if (CurrentAppointment.AppointmentID > 0)
                                    {
                                        apptVm.SetCurrentAppointment(CurrentAppointment);
                                    }
                                    else
                                    {
                                        apptVm.SetCurrentAppointment(CurrentAppointment, LatestePrecriptions.IssueID);
                                    }
                                };
                                GlobalsNAV.ShowDialog<IAddEditAppointment>(onInitDlg);
                            }
                            catch (Exception ex)
                            {
                                Globals.ShowMessage(ex.Message, eHCMSResources.G0442_G1_TBao);
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
                    Globals.ShowMessage(ex.Message, eHCMSResources.G0442_G1_TBao);
                    this.HideBusyIndicator();
                }
            });
            CurrentThread.Start();
        }
        //public void btnCLSAppointment()
        //{
        //    CallAppointmentDialog(true);
        //}
        //CMN: Thêm nút load danh sách cuộc hẹn để tạo CLS sổ
        public void btnCLSAppointmentCollection()
        {
            if (Registration_DataStorage == null || Registration_DataStorage.CurrentPatient == null ||
                LatestePrecriptions == null ||
                LatestePrecriptions.IssueID == 0)
            {
                return;
            }
            if (LatestePrecriptions.AppointmentID > 0)
            {
                CallPCLBookingView();
            }
            else
            {
                this.ShowBusyIndicator();
                var CurrentThread = new Thread(() =>
                {
                    using (var serviceFactory = new ePrescriptionsServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetAppointmentID(LatestePrecriptions.IssueID, false, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            long? OutAppointmentID = 0;
                            contract.EndGetAppointmentID(out OutAppointmentID, asyncResult);
                            LatestePrecriptions.AppointmentID = OutAppointmentID;
                            this.HideBusyIndicator();
                            CallPCLBookingView();
                        }
                        catch (Exception ex)
                        {
                            this.HideBusyIndicator();
                            MessageBox.Show(ex.Message);
                        }
                    }), null);
                    }
                });
                CurrentThread.Start();
            }
        }
        private void CallPCLBookingView()
        {
            IPatientAppointments DialogView = Globals.GetViewModel<IPatientAppointments>();
            DialogView.IsPCLBookingView = true;
            DialogView.Registration_DataStorage = Registration_DataStorage;
            if (LatestePrecriptions.AppointmentID.HasValue && LatestePrecriptions.AppointmentID > 0)
            {
                DialogView.CurrentPtRegDetailAppointmentID = LatestePrecriptions.AppointmentID.Value;
            }
            GlobalsNAV.ShowDialog_V3(DialogView, (aView) =>
            {
                if (LatestePrecriptions.AppointmentID.HasValue && LatestePrecriptions.AppointmentID > 0)
                {
                    aView.SetCurrentPatient(Registration_DataStorage.CurrentPatient, LatestePrecriptions);
                }
                else
                {
                    aView.SetCurrentPatient(Registration_DataStorage.CurrentPatient, LatestePrecriptions);
                }
            }, null, false, true, Globals.GetDefaultDialogViewSize());
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


        public void btChonChanDoan(object sender, RoutedEventArgs e)
        {
            Globals.ConsultationIsChildWindow = true;
            Action<IConsultations> onInitDlg = delegate (IConsultations proAlloc)
            {
                proAlloc.IsPopUp = true;
            };
            GlobalsNAV.ShowDialog<IConsultations>(onInitDlg);
        }


        //KMx: Sau khi kiểm tra, thấy Handle này không còn sử dụng nữa (16/05/2014 17:07)
        #region IHandle<ReloadDataePrescriptionEvent> Members

        public void Handle(ReloadDataePrescriptionEvent message)
        {
            MessageBox.Show(eHCMSResources.A0563_G1_Msg_HandleKhongSuDung, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
            //if (message != null)
            //{
            //    if (Registration_DataStorage.CurrentPatient != null)
            //    {
            //        GetLatestPrescriptionByPtID(Registration_DataStorage.CurrentPatient.PatientID);
            //    }
            //}
        }

        #endregion

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
                typeInfo.ParentVM = this;
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

            //selPrescriptionDetail.Qty = message.TongThuoc;
            selPrescriptionDetail.DrugInstructionNotes = note;
            //Chọn Lịch thì gán bên ngoài = 0

            // // Txd Commented out
            //SelectedPrescriptionDetail.ObjPrescriptionDetailSchedules = message.Data;
            //SelectedPrescriptionDetail.HasSchedules = message.HasSchedule;

            selPrescriptionDetail.ObjPrescriptionDetailSchedules = drugSchedule;
            selPrescriptionDetail.V_DrugType = (long)AllLookupValues.V_DrugType.THUOC_UONGLICHTUAN;

            CalQtyAndQtyMaxForSchedule(selPrescriptionDetail);
        }

        public void HandleDrugScheduleForm(ObservableCollection<PrescriptionDetailSchedules> drugSchedule, double numOfDay, string note)
        {
            if (ObjPrescriptionDetailForForm == null)
            {
                ObjPrescriptionDetailForForm = new PrescriptionDetail();
                ObjPrescriptionDetailForForm.isForm = true;
            }

            //Chọn Lịch thì gán bên ngoài = 0
            ObjPrescriptionDetailForForm.MDose = 0;
            ObjPrescriptionDetailForForm.ADose = 0;
            ObjPrescriptionDetailForForm.NDose = 0;
            ObjPrescriptionDetailForForm.EDose = 0;
            ObjPrescriptionDetailForForm.dosage = 0;

            ObjPrescriptionDetailForForm.MDoseStr = "0";
            ObjPrescriptionDetailForForm.ADoseStr = "0";
            ObjPrescriptionDetailForForm.NDoseStr = "0";
            ObjPrescriptionDetailForForm.EDoseStr = "0";
            ObjPrescriptionDetailForForm.dosageStr = "0";
            if (RegistrationCoverByHI())
            {
                if (IsBenhNhanNoiTru() == false)
                {
                    if (numOfDay > xNgayBHToiDa_NgoaiTru)
                    {
                        ObjPrescriptionDetailForForm.DayRpts = xNgayBHToiDa_NgoaiTru;
                        ObjPrescriptionDetailForForm.DayExtended = numOfDay - xNgayBHToiDa_NgoaiTru;
                    }
                    else
                    {
                        ObjPrescriptionDetailForForm.DayRpts = numOfDay;
                        ObjPrescriptionDetailForForm.DayExtended = 0;
                    }
                }
                else
                {
                    if (numOfDay > xNgayBHToiDa_NoiTru)
                    {
                        ObjPrescriptionDetailForForm.DayRpts = xNgayBHToiDa_NoiTru;
                        ObjPrescriptionDetailForForm.DayExtended = numOfDay - xNgayBHToiDa_NoiTru;
                    }
                    else
                    {
                        ObjPrescriptionDetailForForm.DayRpts = numOfDay;
                        ObjPrescriptionDetailForForm.DayExtended = 0;
                    }
                }
            }
            else
            {
                ObjPrescriptionDetailForForm.DayRpts = numOfDay;
                ObjPrescriptionDetailForForm.DayExtended = 0;
            }

            //ObjPrescriptionDetailForForm.Qty = message.TongThuoc;
            ObjPrescriptionDetailForForm.DrugInstructionNotes = note;
            //Chọn Lịch thì gán bên ngoài = 0

            ObjPrescriptionDetailForForm.ObjPrescriptionDetailSchedules = drugSchedule;
            ObjPrescriptionDetailForForm.V_DrugType = (long)AllLookupValues.V_DrugType.THUOC_UONGLICHTUAN;

            CalQtyAndQtyMaxForSchedule(ObjPrescriptionDetailForForm);
        }

        //▼===== 20190620 TTM: Do có bổ sung vào cột thông tin thuốc ở đầu Grid nên cần phải thay đổi lại
        //                     index của bắt đầu và Index kết thúc của Grid để xuống dòng.
        private enum PrescriptGridEditCellIdx { EditCellIdxBegin = 5, EditCellIdxEnd = 16 };
        EmrPrescriptionGrid grdPrescription;
        public void grdPrescription_Loaded(object sender, RoutedEventArgs e)
        {
            grdPrescription = sender as EmrPrescriptionGrid;
            grdPrescription.nFirstEditIdx = (int)PrescriptGridEditCellIdx.EditCellIdxBegin;
            grdPrescription.nLastEditIdx = (int)PrescriptGridEditCellIdx.EditCellIdxEnd;
        }

        #endregion


        #region CheckThuocBiTrungTrongToa
        private string CheckThuocBiTrungTrongToa()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var prescriptionDetail in LatestePrecriptions.PrescriptionDetails)
            {
                //▼===== #024
                string DrugName = "";
                if (CountDrug(prescriptionDetail.DrugID, out DrugName) >= 2)
                {
                    if (!sb.ToString().Contains(DrugName)) //TBL: Kiểm tra tên thuốc bị trùng đã có trong thông báo chưa, nếu có rồi thì bỏ qua để tránh bị trùng lắp khi thông báo
                    {
                        sb.AppendLine(DrugName);
                    }
                }
                //▲===== #024
            }
            if (!string.IsNullOrEmpty(sb.ToString()))
            {
                bBlock = true;
                return eHCMSResources.K0072_G1_TrungThuocTrongToa + ":" + Environment.NewLine + sb.ToString();
            }
            else
            {
                return "";
            }
        }
        private int CountDrug(Nullable<Int64> DrugID, out string DrugName)
        {
            DrugName = "";
            int d = 0;
            if (DrugID != null && DrugID > 0)
            {
                foreach (var prescriptionDetail in LatestePrecriptions.PrescriptionDetails)
                {
                    if (prescriptionDetail.DrugID == DrugID)
                    {
                        d++;
                    }
                    //▼===== #024
                    if (d == 2) //TBL: Nếu có đếm được 2 lần thuốc trùng nhau thì out BrandName ra để hiển thị
                    {
                        DrugName = " - " + prescriptionDetail.SelectedDrugForPrescription.BrandName;
                        break;
                    }
                    //▼===== #024
                }
            }
            return d;
        }
        private string CheckThuocBiTrungHoatChatTrongToa()
        {
            if (!Globals.ServerConfigSection.ConsultationElements.CheckToaThuocBiTrungTheoHoatChat || bBlock)
            {
                return "";
            }
            StringBuilder sb = new StringBuilder();
            foreach (var prescriptionDetail in LatestePrecriptions.PrescriptionDetails)
            {
                string DrugName = "";
                if (prescriptionDetail.SelectedDrugForPrescription != null
                    && CheckDrug_ForGeneric(prescriptionDetail.SelectedDrugForPrescription.GenericID, prescriptionDetail.SelectedDrugForPrescription.DrugID, out DrugName)
                    && !sb.ToString().Contains(prescriptionDetail.SelectedDrugForPrescription.BrandName)) //TBL: Tránh lặp lại tên thuốc A - B và B - A
                {
                    DrugName += " và " + prescriptionDetail.SelectedDrugForPrescription.BrandName;
                    sb.AppendLine(DrugName);
                }
            }
            if (!string.IsNullOrEmpty(sb.ToString()))
            {
                return eHCMSResources.Z2861_G1_TrungHoatChatTrongToa + ":" + Environment.NewLine + sb.ToString();
            }
            else
            {
                return "";
            }
        }
        private bool CheckDrug_ForGeneric(Nullable<Int64> GenericID, Nullable<Int64> DrugID, out string DrugName)
        {
            DrugName = "";
            if (GenericID != null && GenericID > 0)
            {
                foreach (var prescriptionDetail in LatestePrecriptions.PrescriptionDetails)
                {
                    if (prescriptionDetail.SelectedDrugForPrescription != null && prescriptionDetail.SelectedDrugForPrescription.GenericID == GenericID && prescriptionDetail.SelectedDrugForPrescription.DrugID != DrugID)
                    {
                        DrugName = " - " + prescriptionDetail.SelectedDrugForPrescription.BrandName;
                        return true;
                    }
                }
            }
            return false;
        }
        private string CheckThuocBiTrungNhomThuocTrongToa()
        {
            if (!Globals.ServerConfigSection.ConsultationElements.CheckToaThuocBiTrungNhomThuoc || bBlock)
            {
                return "";
            }
            StringBuilder sb = new StringBuilder();
            foreach (var prescriptionDetail in LatestePrecriptions.PrescriptionDetails)
            {
                string DrugName = "";
                if (prescriptionDetail.SelectedDrugForPrescription != null
                    && CheckDrug_ForDrugClass(prescriptionDetail.SelectedDrugForPrescription.DrugClassID, prescriptionDetail.SelectedDrugForPrescription.DrugID, out DrugName)
                    && !sb.ToString().Contains(prescriptionDetail.SelectedDrugForPrescription.BrandName)) //TBL: Tránh lặp lại tên thuốc A - B và B - A
                {
                    DrugName += " và " + prescriptionDetail.SelectedDrugForPrescription.BrandName + ": " + prescriptionDetail.SelectedDrugForPrescription.DrugClassName;
                    sb.AppendLine(DrugName);
                }
            }
            if (!string.IsNullOrEmpty(sb.ToString()))
            {
                return eHCMSResources.Z2862_G1_TrungNhomThuocTrongToa + ":" + Environment.NewLine + sb.ToString();
            }
            else
            {
                return "";
            }
        }
        private bool CheckDrug_ForDrugClass(Nullable<Int64> DrugClassID, Nullable<Int64> DrugID, out string DrugName)
        {
            DrugName = "";
            if (DrugClassID != null && DrugClassID > 0)
            {
                foreach (var prescriptionDetail in LatestePrecriptions.PrescriptionDetails)
                {
                    if (prescriptionDetail.SelectedDrugForPrescription != null && prescriptionDetail.SelectedDrugForPrescription.DrugClassID == DrugClassID && prescriptionDetail.SelectedDrugForPrescription.DrugID != DrugID)
                    {
                        DrugName = " - " + prescriptionDetail.SelectedDrugForPrescription.BrandName;
                        return true;
                    }
                }
            }
            return false;
        }
        private string CheckPhacDo()
        {
            string warning = "";
            LatestePrecriptions.IsOutsideRegimen = false;
            //TBL: Hien tai dang test phac do nen them dieu kien kiem tra DrugsInTreatmentRegimen
            if (RefTreatmentRegimenDrugDetails != null && RefTreatmentRegimenDrugDetails.Count > 0 && DrugsInTreatmentRegimen != null)
            {
                //20190603 TBL: Luu lan dau toa thuoc co thuoc ngoai phac do nhung khong thay thong bao
                if (Globals.ServerConfigSection.ConsultationElements.EnableTreatmentRegimen && !bBlock && DrugsInTreatmentRegimen.Count > 0 && LatestePrecriptions.PrescriptionDetails.Any(pd => pd.DrugID.GetValueOrDefault(0) > 0 && !DrugsInTreatmentRegimen.Any(tr => tr.DrugID == pd.DrugID)))
                {
                    LatestePrecriptions.IsOutsideRegimen = true;
                    warning += eHCMSResources.Z2263_G1_ToaChuaThuocNgoaiPD + ":" + Environment.NewLine;
                }
                //▼===== #026
                if (!string.IsNullOrEmpty(warning))
                {
                    foreach (var item in LatestePrecriptions.PrescriptionDetails)
                    {
                        if (item.DrugID.GetValueOrDefault() > 0 && !DrugsInTreatmentRegimen.Any(tr => tr.DrugID == item.DrugID))
                        {
                            warning += "- " + item.BrandName + Environment.NewLine;
                        }
                    }
                }
                //▲===== #026
            }
            if (!string.IsNullOrEmpty(warning))
            {
                return warning;
            }
            return "";
        }
        #endregion


        #region "Dược Sĩ Sửa Lại Toa Của Bác Sĩ"

        public void Handle(DuocSi_EditingToaThuocEvent message)
        {
            if (message != null)
            {
                //KMx: Lấy trực tiếp từ Globals, không về server lấy nữa (21/05/2014 14:39)
                //GetPrescriptionTypes_LoadList();

                GetPrescriptionTypeList();

                IsEnabledForm = true;

                //Các nút DS không cần nhìn thấy
                btnCreateNewVisibility = false;
                btnSaveAddNewVisibility = false;
                btnUpdateVisibility = false;
                bntSaveAsVisibility = false;
                btnCopyToVisible = false;
                btChonChanDoanVisibility = false;
                //Các nút DS không cần nhìn thấy


                IsEnabled = false;
                btnEditIsEnabled = true;
                btnCreateAndCopyIsEnabled = true;
                IsEnabledPrint = true;


                ToaChonDeSua = message.SelectedPrescription.DeepCopy();

                PrecriptionsForPrint = ToaChonDeSua;

                InitForDuocSi_EditingToaThuoc();

                LatestePrecriptions = message.SelectedPrescription;

                //HideButton = Visibility.Visible;
                GetPrescriptionDetailsByPrescriptID(LatestePrecriptions.PrescriptID);
            }
        }

        private void InitForDuocSi_EditingToaThuoc()
        {
            //Load UC Dị Ứng/Cảnh Báo
            var uc1 = Globals.GetViewModel<IAllergiesWarning_ByPatientID>();
            UCAllergiesWarningByPatientID = uc1;
            this.ActivateItem(uc1);
            //Load UC Dị Ứng/Cảnh Báo
        }

        private Prescription _ToaChonDeSua;
        public Prescription ToaChonDeSua
        {
            get { return _ToaChonDeSua; }
            set
            {
                if (_ToaChonDeSua != value)
                {
                    _ToaChonDeSua = value;
                    NotifyOfPropertyChange("ToaChonDeSua");
                }
            }
        }


        public bool CanbtDuocSiEdit
        {
            get
            {
                return (!IsWaitingSaveDuocSiEdit);
            }
        }
        public void btDuocSiEdit()
        {
            bBlock = false;
            StringBuilder sb = new StringBuilder();
            string error = "";
            LatestePrecriptions.Reason = "";
            if (string.IsNullOrEmpty(LatestePrecriptions.Diagnosis))
            {
                MessageBox.Show(string.Format("{0}!", eHCMSResources.Z0815_G1_ChiDinhCDoan), eHCMSResources.A0290_G1_Msg_LuuToaThuoc, MessageBoxButton.OK);
                return;
            }

            if (LatestePrecriptions.V_PrescriptionType == null)
            {
                MessageBox.Show(eHCMSResources.A0333_G1_Msg_InfoChonLoaiToa, eHCMSResources.A0290_G1_Msg_LuuToaThuoc, MessageBoxButton.OK);
                return;
            }

            if (grdPrescription == null)
            {
                MessageBox.Show(eHCMSResources.A0365_G1_Msg_InfoChonThuocDeRaToa, eHCMSResources.A0290_G1_Msg_LuuToaThuoc, MessageBoxButton.OK);
                return;
            }
            if (grdPrescription.IsValid == false)
            {
                MessageBox.Show(string.Format("{0} {1}", eHCMSResources.A0540_G1_Msg_InfoDataKhHopLe, eHCMSResources.A0541_G1_Msg_InfoDataToaThuocKhHopLe), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }

            if (!CheckHoiChan())
            {
                return;
            }
            //▼==== #040
            if (!CheckSoNgayThuoc())
            {
                return;
            }
            //▲==== #040
            //▼===== #041
            string msg;
            if(!CheckPatientInfoForOutPtTreatment(out msg))
            {
                MessageBox.Show(string.Format("Thông tin người bệnh chưa đầy đủ. Vui lòng bổ sung thêm các thông tin sau: {0}", msg));
            }
            //▲==== #041
            //TBL: Thay đổi cách hiển thị thông báo. Lúc trước hiển thị từng popup, bây giờ hiển thị 1 lúc
            error += CheckAllThuocBiDiUng();
            error += CheckThuocBiTrungTrongToa();
            error += ErrCheckChongChiDinh();
            error += ErrCheckChongChiDinh_TrongNgay();
            error += CheckPhacDo();
            error += KiemTraTuongTacHoatChatTrongNgay();
            error += CheckThuocBiTrungHoatChatTrongToa();
            error += CheckThuocBiTrungHoatChatTrongNgay();
            error += CheckThuocBiTrungHoatChatBaoHiemConThuoc(false);
            error += CheckThuocBiTrungHoatChatBaoHiemConThuoc(true);
            error += KiemTraTuongTuHoatChatTrongNgay();
            error += CheckThuocBiTrungNhomThuocTrongToa();

            if (!string.IsNullOrEmpty(error))
            {
                IErrorBold MessBox = Globals.GetViewModel<IErrorBold>();
                MessBox.FireOncloseEvent = true;
                if (bBlock)
                {
                    MessBox.isCheckBox = false;
                    MessBox.SetMessage(error + Environment.NewLine + eHCMSResources.Z2676_G1_KhongTheRaToa, "");
                }
                else
                {
                    MessBox.IsShowReason = true;
                    MessBox.SetMessage(error, eHCMSResources.Z0627_G1_TiepTucLuu);
                }
                GlobalsNAV.ShowDialog_V3(MessBox);
                if (!MessBox.IsAccept)
                {
                    return;
                }
                LatestePrecriptions.Reason = MessBox.Reason;
            }
            //▲===== #022
            if (!CheckThuocHopLe())
            {
                return;
            }
            //Bỏ dòng cuối cùng rỗng của Toa
            RemoveLastRowPrecriptionDetail();

            DuocSi_IsEditingToaThuoc = true;


            if (ConfirmSoLuongNotEnoughBeforeSave())
            {
                if (ToaChonDeSua.ModifierStaffID == null) /*DS Đang Sửa Toa BS*/
                {
                    SetValueToaOfDuocSi();
                    //Prescriptions_DuocSiEdit();
                    //Dinh sua o day
                    Coroutine.BeginExecute(CoroutinePrescriptions_DuocSiEdit());
                }
                else /*DS Sửa Toa DS*/
                {
                    SetValueToaOfDuocSi();
                    //Prescriptions_DuocSiEditDuocSi();
                    Coroutine.BeginExecute(CoroutinePrescriptions_DuocSiEditDuocSi());
                }
            }
            else
            {
                //Cộng lại dòng trống
                AddNewBlankDrugIntoPrescriptObjectNew();
                //Cộng lại dòng trống
            }
        }

        private void Prescriptions_DuocSiEdit()
        {
            IsEnabledForm = false;

            IsWaitingSaveDuocSiEdit = true;

            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            long PrescriptionID = 0;
            long IssueID = 0;
            string OutError = "";



            var t = new Thread(() =>
            {
                using (var serviceFactory = new ePrescriptionsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginPrescriptions_DuocSiEdit(ObjToaOfDuocSi, ToaChonDeSua, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            if (contract.EndPrescriptions_DuocSiEdit(out PrescriptionID, out IssueID, out OutError, asyncResult))
                            {
                                ObjToaOfDuocSi.IssueID = IssueID;
                                ObjToaOfDuocSi.PrescriptID = PrescriptionID;

                                PrecriptionsForPrint = ObjToaOfDuocSi;
                                ToaChonDeSua = ObjToaOfDuocSi;
                                LatestePrecriptions = ObjToaOfDuocSi;

                                IsEnabledAutoComplete = false;

                                IsEnabled = false;

                                btDuocSiEditIsEnabled = false;
                                btnUndoIsEnabled = false;
                                btnEditIsEnabled = true;
                                btnCreateAndCopyIsEnabled = true;
                                IsEnabledPrint = true;

                                MessageBox.Show(eHCMSResources.K0175_G1_ToaThuocDaLuuOk);

                                //phat su kien load lai toa thuoc cuoi
                                if (DuocSi_IsEditingToaThuoc)
                                {
                                    Globals.EventAggregator.Publish(new ReLoadToaOfDuocSiEditEvent<Prescription> { Prescription = ObjToaOfDuocSi });
                                    TryClose();
                                }

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
                            MessageBox.Show(ex.Message);
                        }
                        finally
                        {

                            IsEnabledForm = true;

                            IsWaitingSaveDuocSiEdit = false;


                        }

                    }), null);

                }

            });

            t.Start();
        }

        private void Prescriptions_DuocSiEditDuocSi()
        {

            IsEnabledForm = false;

            IsWaitingSaveDuocSiEdit = true;


            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            long PrescriptionID = 0;
            long IssueID = 0;
            string OutError = "";


            var t = new Thread(() =>
            {
                using (var serviceFactory = new ePrescriptionsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginPrescriptions_DuocSiEditDuocSi(ObjToaOfDuocSi, ToaChonDeSua, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            if (contract.EndPrescriptions_DuocSiEditDuocSi(out PrescriptionID, out IssueID, out OutError, asyncResult))
                            {
                                ObjToaOfDuocSi.IssueID = IssueID;
                                ObjToaOfDuocSi.PrescriptID = PrescriptionID;

                                PrecriptionsForPrint = ObjToaOfDuocSi;
                                LatestePrecriptions = ObjToaOfDuocSi;
                                ToaChonDeSua = ObjToaOfDuocSi;

                                IsEnabledAutoComplete = false;

                                IsEnabled = false;
                                btDuocSiEditIsEnabled = false;
                                btnUndoIsEnabled = false;
                                btnEditIsEnabled = true;
                                btnCreateAndCopyIsEnabled = true;
                                IsEnabledPrint = true;

                                MessageBox.Show(eHCMSResources.K0175_G1_ToaThuocDaLuuOk);

                                //phat su kien load lai toa thuoc cuoi
                                if (DuocSi_IsEditingToaThuoc)
                                {
                                    Globals.EventAggregator.Publish(new ReLoadToaOfDuocSiEditEvent<Prescription> { Prescription = ObjToaOfDuocSi });
                                    TryClose();
                                }
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
                            MessageBox.Show(ex.Message);
                        }
                        finally
                        {
                            IsEnabledForm = true;

                            IsWaitingSaveDuocSiEdit = false;
                        }

                    }), null);

                }

            });

            t.Start();
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

        public void GetAllDrugsContrainIndicator()
        {
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyDrugServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginGetAllDrugsContrainIndicator(Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndGetAllDrugsContrainIndicator(asyncResult);
                            if (results != null)
                            {
                                if (allMedProductContraIndicatorRelToMedCond == null)
                                {
                                    allMedProductContraIndicatorRelToMedCond = new ObservableCollection<DrugAndConTra>();
                                }
                                else
                                {
                                    allMedProductContraIndicatorRelToMedCond.Clear();
                                }
                                foreach (var p in results)
                                {
                                    allMedProductContraIndicatorRelToMedCond.Add(p);
                                }
                                NotifyOfPropertyChange(() => allMedProductContraIndicatorRelToMedCond);
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

            });

            t.Start();
        }

        public void GetAllRefGenericRelation()
        {
            if (Globals.MAPRefGenericRelation != null)
            {
                return;
            }
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyDrugServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetAllRefGenericRelation(Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndGetAllRefGenericRelation(asyncResult);
                            if (results != null)
                            {
                                Globals.MAPRefGenericRelation = results;
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
                        sb.AppendLine(string.Format(eHCMSResources.Z1073_G1_ThuocSLgTrongKhoKgDuBan, item.SelectedDrugForPrescription.BrandName.Trim()));
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
                if (Globals.ServerConfigSection.ConsultationElements.AllowSaveQuantityNotEnough)
                {
                    if (MessageBox.Show(msg + Environment.NewLine + eHCMSResources.I0943_G1_I, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
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
                    MessageBox.Show(msg, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                    return false;
                }
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
                    NewPrecriptions.ExamDate = ObjDiagnosisTreatment_Current.PatientServiceRecord == null && !IsShowSummaryContent ? Globals.GetCurServerDateTime() : ObjDiagnosisTreatment_Current.PatientServiceRecord.ExamDate;
                }
            }


            NewPrecriptions.V_PrescriptionNotes = (long)AllLookupValues.V_PrescriptionNotes.TOAGOC;

            NewPrecriptions.PrescriptionIssueHistory = new PrescriptionIssueHistory();
            NewPrecriptions.PrescriptionIssueHistory.DeptLocID = Globals.DeptLocation.DeptLocationID;
            if (HisIDVisibility)//neu dang ky co bao hiem
            {
                NewPrecriptions.PrescriptionIssueHistory.HisID = Registration_DataStorage.CurrentPatientRegistration.HisID.Value;
            }

            //Cụ thể  DV nào
            if (Registration_DataStorage != null && Registration_DataStorage.CurrentPatientRegistration != null)
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
                if (Registration_DataStorage != null 
                    && Registration_DataStorage.CurrentPatientRegistration != null 
                    && Registration_DataStorage.CurrentPatient != null 
                    )
                {
                    NewPrecriptions.PtRegistrationID = Registration_DataStorage.CurrentPatientRegistration.PtRegistrationID;
                    NewPrecriptions.PatientID = Registration_DataStorage.CurrentPatient.PatientID;
                }
                if(ObjDiagnosisTreatment_Current != null)
                {
                    NewPrecriptions.ServiceRecID = ObjDiagnosisTreatment_Current.ServiceRecID;
                    NewPrecriptions.Diagnosis = string.IsNullOrEmpty(ObjDiagnosisTreatment_Current.DiagnosisFinal) ? 
                        string.IsNullOrEmpty(ObjDiagnosisTreatment_Current.Diagnosis) ? null : ObjDiagnosisTreatment_Current.Diagnosis.Trim() : 
                        ObjDiagnosisTreatment_Current.DiagnosisFinal.Trim();
                }
                
                NewPrecriptions.NDay = 0;
                //▼===== 20191002 TTM:  Do không set lại PreNoDrug (Toa không thuốc nên khi 1 bệnh nhân trước đó có toa không thuốc load bệnh nhân mới lên 
                //                      => Toa không thuốc vấn được check => Có thể gây nhầm lẫn.
                PreNoDrug = false;
                //▲===== 
               
                NewPrecriptions.ConsultantDoctor = new Staff();
                if (CurrentPrescriptionType != null)
                {
                    NewPrecriptions.V_PrescriptionType = CurrentPrescriptionType.LookupID;
                }
                
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

        //private DiagnosisTreatment _ObjGetDiagnosisTreatmentByPtID;
        //public DiagnosisTreatment ObjGetDiagnosisTreatmentByPtID
        //{
        //    get
        //    {
        //        return _ObjGetDiagnosisTreatmentByPtID;
        //    }
        //    set
        //    {
        //        if (_ObjGetDiagnosisTreatmentByPtID != value)
        //        {
        //            _ObjGetDiagnosisTreatmentByPtID = value;
        //            NotifyOfPropertyChange(() => ObjGetDiagnosisTreatmentByPtID);
        //        }
        //    }
        //}


        //KMx: Sau khi kiểm tra, thấy hàm này không còn sử dụng nữa (16/05/2014 16:51)
        /*opt:-- 0: Query by PatientID, 1: Query by PtRegistrationID, 2: Query By NationalMedicalCode  */
        private void GetDiagnosisTreatmentByPtID(long? patientID, long? PtRegistrationID, string nationalMedCode, int opt, bool latest, long? V_RegistrationType)
        {
            MessageBox.Show(string.Format(eHCMSResources.Z1005_G1_KgConSD, "GetDiagnosisTreatmentByPtID()"), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.Z0536_G1_CDoanCuoi) });

            //IsWaitingGetDiagnosisTreatmentByPtID = true;

            //var t = new Thread(() =>
            //{
            //    using (var serviceFactory = new ePMRsServiceClient())
            //    {
            //        var contract = serviceFactory.ServiceInstance;
            //        contract.BeginGetDiagnosisTreatmentByPtID(patientID, PtRegistrationID, null, opt, true, V_RegistrationType, Globals.DispatchCallback((asyncResult) =>
            //        {
            //            try
            //            {
            //                var results = contract.EndGetDiagnosisTreatmentByPtID(asyncResult);

            //                if (results != null && results.Count > 0)
            //                {
            //                    ObjDiagnosisTreatment_Current = results.ToObservableCollection()[0];

            //                    Globals.ObjGetDiagnosisTreatmentByPtID = ObjectCopier.DeepCopy(ObjDiagnosisTreatment_Current);

            //                    HasDiagnosis = true;

            //                    GetPrescriptionTypes();
            //                }
            //                else
            //                {
            //                    IsEnabledForm = false;

            //                    HasDiagnosis = false;
            //                    ObjDiagnosisTreatment_Current = new DiagnosisTreatment();

            //                    btChonChanDoanIsEnabled = false;
            //                    btnSaveAddNewIsEnabled = false;
            //                    IsEnabledPrint = false;

            //                    MessageBox.Show(eHCMSResources.A0403_G1_Msg_InfoChuaCoCDChoDK, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
            //                }

            //            }
            //            catch (Exception ex)
            //            {
            //                MessageBox.Show(ex.Message);
            //            }
            //            finally
            //            {
            //                IsWaitingGetDiagnosisTreatmentByPtID = false;
            //            }

            //        }), null);

            //    }

            //});

            //t.Start();
        }

        private bool _bBlock;
        public bool bBlock
        {
            get { return _bBlock; }
            set
            {
                if (_bBlock != value)
                {
                    _bBlock = value;
                    NotifyOfPropertyChange(() => bBlock);
                }
            }
        }

        public bool CheckPrescriptionBeforeSave()
        {
            bBlock = false;
            StringBuilder sb = new StringBuilder();
            string error = "";
            if (string.IsNullOrEmpty(LatestePrecriptions.Diagnosis) && IsShowSummaryContent)
            {
                MessageBox.Show(eHCMSResources.A0289_G1_Msg_InfoCDinhCDDeRaToa, eHCMSResources.A0290_G1_Msg_LuuToaThuoc, MessageBoxButton.OK);
                return false;
            }

            if (LatestePrecriptions.V_PrescriptionType == null)
            {
                MessageBox.Show(eHCMSResources.A0333_G1_Msg_InfoChonLoaiToa, eHCMSResources.A0290_G1_Msg_LuuToaThuoc, MessageBoxButton.OK);
                return false;
            }

            if (grdPrescription == null && IsShowSummaryContent)
            {
                MessageBox.Show(eHCMSResources.A0365_G1_Msg_InfoChonThuocDeRaToa, eHCMSResources.A0290_G1_Msg_LuuToaThuoc, MessageBoxButton.OK);
                return false;
            }

            if (grdPrescription.IsValid == false)
            {
                MessageBox.Show(string.Format("{0} {1}", eHCMSResources.A0540_G1_Msg_InfoDataKhHopLe, eHCMSResources.A0541_G1_Msg_InfoDataToaThuocKhHopLe), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return false;
            }

            if (!CheckHoiChan())
            {
                return false;
            }
            ReCheckHICheckBox();
            //▼==== #046
            if (string.IsNullOrEmpty(LatestePrecriptions.DoctorAdvice))
            {
                MessageBox.Show("Không thể lưu toa. Bắt buộc phải nhập lời dặn hoặc chọn lời dặn từ mẫu.", eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return false;
            }
            //▲==== #046
            //▼==== #053
            if (LatestePrecriptions.DoctorAdvice.Contains("<") || LatestePrecriptions.DoctorAdvice.Contains(">"))
            {
                MessageBox.Show("Nội dung lời dặn không được sử dụng ký tự '<', '>'. Vui lòng kiểm tra lại!", eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return false;
            }
            //▲==== #053
            //▼==== #041
            string msg;
            if (!CheckPatientInfoForOutPtTreatment(out msg))
            {
                MessageBox.Show(string.Format("Thông tin người bệnh chưa đầy đủ. Vui lòng bổ sung thêm các thông tin sau: {0}", msg));
                return false;
            }
            //▲==== #041
            //▼==== #049
            //if (!CheckPatientInfoForPatient72Month(out msg))
            //{
            //    MessageBox.Show(string.Format("Thông tin người bệnh chưa đầy đủ. Vui lòng bổ sung thêm các thông tin sau: {0}", msg));
            //    return false;
            //}
            //▲==== #049
            //▼==== #050
            if (!CheckPatientInfoForToaGayNghien(out msg))
            {
                MessageBox.Show(string.Format("Bác sĩ vui lòng điền đầy đủ {0} tại Tab thông tin bệnh nhân", msg));
                return false;
            }
            //▲==== #050
            //▼===== #022
            //TBL: Thay đổi cách hiển thị thông báo. Lúc trước hiển thị từng popup, bây giờ hiển thị 1 lúc
            error += CheckAllThuocBiDiUng();
            error += CheckThuocBiTrungTrongToa();
            error += ErrCheckChongChiDinh();
            error += ErrCheckChongChiDinh_TrongNgay();
            error += CheckPhacDo();
            error += KiemTraTuongTacHoatChatTrongNgay();
            error += CheckThuocBiTrungHoatChatTrongToa();
            error += CheckThuocBiTrungHoatChatTrongNgay();
            error += CheckThuocBiTrungHoatChatBaoHiemConThuoc(false);
            error += CheckThuocBiTrungHoatChatBaoHiemConThuoc(true);
            error += KiemTraTuongTuHoatChatTrongNgay();
            error += CheckThuocBiTrungNhomThuocTrongToa();

            if (!string.IsNullOrEmpty(error))
            {
                IErrorBold MessBox = Globals.GetViewModel<IErrorBold>();
                MessBox.FireOncloseEvent = true;
                if (bBlock)
                {
                    MessBox.isCheckBox = false;
                    MessBox.SetMessage(error + Environment.NewLine + eHCMSResources.Z2676_G1_KhongTheRaToa, "");
                }
                else
                {
                    MessBox.IsShowReason = true;
                    MessBox.SetMessage(error, eHCMSResources.Z0627_G1_TiepTucLuu);
                    //20191028 TBL: BM 0018509: Toa thuốc có Reason thì khi cập nhật đem Reason lên để trên popup cho người dùng hiệu chỉnh
                    MessBox.Reason = LatestePrecriptions.Reason;
                }
                GlobalsNAV.ShowDialog_V3(MessBox);
                if (!MessBox.IsAccept)
                {
                    return false;
                }
                LatestePrecriptions.Reason = MessBox.Reason;
            }
            else //20191029 TBL: Trường hợp lúc đầu có cảnh báo và đã nhập lý do nhưng sau đó số lượng thuốc không đủ thì chỉnh sửa lại số lượng thuốc và không còn cảnh báo nữa thì set Reason = ""
            {
                LatestePrecriptions.Reason = "";
            }
            //▲===== #022
            if (!CheckThuocHopLe())
            {
                return false;
            }
            if (LatestePrecriptions != null && LatestePrecriptions.PrescriptionDetails != null && LatestePrecriptions.PrescriptionDetails.Any(x => x.RefGenericDrugDetail != null && x.RefGenericDrugDetail.V_CatDrugType == (long)AllLookupValues.V_CatDrugType.DrugDept))
            {
                MessageBox.Show(eHCMSResources.Z2294_G1_ToaCoChuaThuocKTDM, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return false;
            }
            //▼===== #030
            if (Globals.ServerConfigSection.ConsultationElements.PercentPrescriptionForHI > 0 && Registration_DataStorage.CurrentPatientRegistrationDetail.HIBenefit.GetValueOrDefault() > 0
                && LatestePrecriptions.PrescriptionDetails != null && LatestePrecriptions.PrescriptionDetails.Any(x => x.DrugID > 0 && x.SelectedDrugForPrescription != null))
            {
                decimal TotalHIPayment = 0;
                foreach (PrescriptionDetail detail in LatestePrecriptions.PrescriptionDetails)
                {
                    if (detail.DrugID > 0 && detail.SelectedDrugForPrescription != null)
                    {
                        TotalHIPayment += detail.SelectedDrugForPrescription.HIAllowedPrice * Convert.ToDecimal(Registration_DataStorage.CurrentPatientRegistration.PtInsuranceBenefit.GetValueOrDefault(0)) * Convert.ToDecimal(detail.Qty);
                    }
                }
                if (TotalHIPayment > ((Convert.ToDecimal(Globals.ServerConfigSection.ConsultationElements.PercentPrescriptionForHI) + 1) * TotalHIPaymentForRegistration))
                {
                    //20200623 TBL: Khi vượt quá số tiền BHYT cảu đăng ký thì hiện thông báo, nếu cancel thì không lưu
                    // 20200709 TNHX: Chuyển phần trăm thành số tiền chênh lệch cho Bsy dễ giảm tiền
                    decimal tempPayment = TotalHIPayment - ((Convert.ToDecimal(Globals.ServerConfigSection.ConsultationElements.PercentPrescriptionForHI) + 1) * TotalHIPaymentForRegistration);
                    if (MessageBox.Show(string.Format(eHCMSResources.Z3043_G1_MsgSoTienLechToaThuoc, tempPayment.ToString("#,#.##"), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel)) == MessageBoxResult.Cancel)
                    {
                        return false;
                    }
                    //Nếu đồng ý thì lưu xuống bảng phần trăm vượt
                    else
                    {
                        //LatestePrecriptions.PercentExceed = 
                        //    Convert.ToInt32(TotalHIPayment 
                        //    / (Convert.ToDecimal(Globals.ServerConfigSection.ConsultationElements.PercentPrescriptionForHI) 
                        //    * TotalHIPaymentForRegistration 
                        //    + TotalHIPaymentForRegistration) * 100);

                        LatestePrecriptions.PercentExceed =
                            Convert.ToInt32(TotalHIPayment
                            / ((Convert.ToDecimal(Globals.ServerConfigSection.ConsultationElements.PercentPrescriptionForHI) + 1)
                            * TotalHIPaymentForRegistration));
                    }
                }
            }
            //▲===== #030
            //▼====== #048
            DiagnosisIcd10Items CurICD10MainItem = CS_DS.refIDC10List.Where(x => x.IsMain).FirstOrDefault();
            string HIPCodeTemp = "";
            if (Registration_DataStorage != null && Registration_DataStorage.CurrentPatientRegistration != null && Registration_DataStorage.CurrentPatientRegistration.HealthInsurance != null)
            {
                HIPCodeTemp = Registration_DataStorage.CurrentPatientRegistration.HealthInsurance.HIPCode;
            }
            if (Globals.ServerConfigSection.ConsultationElements.PrescriptionMaxHIPay > 0
                && LatestePrecriptions.PrescriptionDetails != null
                && LatestePrecriptions.PrescriptionDetails.Any(x => x.DrugID > 0 && x.SelectedDrugForPrescription != null)
                && LatestePrecriptions.PrescriptionDetails.Where(x => x.DrugID > 0 && x.SelectedDrugForPrescription != null).Sum(x => x.SelectedDrugForPrescription.HIAllowedPrice * Convert.ToDecimal(x.Qty)) > Globals.ServerConfigSection.ConsultationElements.PrescriptionMaxHIPay
                //▼====== #035
                && !Globals.ServerConfigSection.ConsultationElements.ApplyFilterPrescriptionsHasHIPayTable
                && !CurICD10MainItem.DiseasesReference.IsNewInYear
                && !Globals.ServerConfigSection.ConsultationElements.TT5149ListHIPCode.Contains(HIPCodeTemp))
            //▲====== #035
            //▲====== #048
            {
                // 20200716 TNHX: Thêm cấu hình cảnh bảo hay chặn khi vượt trần của toa thuốc
                decimal tempPayment = LatestePrecriptions.PrescriptionDetails.Where(x => x.DrugID > 0 && x.SelectedDrugForPrescription != null).Sum(x => x.SelectedDrugForPrescription.HIAllowedPrice * Convert.ToDecimal(x.Qty)) - Globals.ServerConfigSection.ConsultationElements.PrescriptionMaxHIPay;
                if (Globals.ServerConfigSection.ConsultationElements.BlockPrescriptionMaxHIPay)
                {
                    MessageBox.Show(string.Format(eHCMSResources.Z3045_G1_ThuocTrongToaVuotTran, tempPayment.ToString("#,#.##")), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                    return false;
                }
                else
                {
                    if (MessageBox.Show(eHCMSResources.Z2792_G1_ThuocTrongToaVuotTran, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) != MessageBoxResult.OK)
                    {
                        return false;
                    }
                }
            }
            //▼====== #035
            else if (LatestePrecriptions.PrescriptionDetails != null
                && LatestePrecriptions.PrescriptionDetails.Any(x => x.DrugID > 0 && x.SelectedDrugForPrescription != null)
                && Globals.ServerConfigSection.ConsultationElements.ApplyFilterPrescriptionsHasHIPayTable
                && Registration_DataStorage.ListFilterPrescriptionsHasHIPay != null
                //▼====== #048
                && !CurICD10MainItem.DiseasesReference.IsNewInYear
                && !Globals.ServerConfigSection.ConsultationElements.TT5149ListHIPCode.Contains(HIPCodeTemp))
                //▲====== #048
            {
                int FilterType = 1;
                if (Registration_DataStorage.CurrentPatientRegistration.OutPtTreatmentProgramID != null)
                {
                    FilterType = 2;
                }
                List<FilterPrescriptionsHasHIPay> ListFilterPrescriptionsHasHIPay = Registration_DataStorage.ListFilterPrescriptionsHasHIPay.Where(x => x.FilterType == FilterType).ToList();
                if (ListFilterPrescriptionsHasHIPay == null || ListFilterPrescriptionsHasHIPay.Count == 0)
                {
                    return true;
                }
                decimal tempPayment = LatestePrecriptions.PrescriptionDetails.Where(x => x.DrugID > 0 && x.SelectedDrugForPrescription != null && !ListFilterPrescriptionsHasHIPay[0].ListDrugCodeSkip.Contains(x.SelectedDrugForPrescription.DrugCode)).Sum(x => x.SelectedDrugForPrescription.HIAllowedPrice * Convert.ToDecimal(x.Qty));
                string CurPatientCode = Registration_DataStorage.CurrentPatient.PatientCode;
                string CurICD10Main = CS_DS.refIDC10List.Where(x => x.IsMain).FirstOrDefault().ICD10Code;
                ListFilterPrescriptionsHasHIPay = ListFilterPrescriptionsHasHIPay.Where(x => 
                    tempPayment > x.GetHIPayFrom(CurICD10Main, LatestePrecriptions.PrescriptionDetails) && tempPayment < x.HIPayTo).ToList();
                if (ListFilterPrescriptionsHasHIPay == null || ListFilterPrescriptionsHasHIPay.Count == 0)
                {
                    return true;
                }
                if (ListFilterPrescriptionsHasHIPay[0].IsBlock)
                {
                    if ((ListFilterPrescriptionsHasHIPay[0].ListPatientCodeSkip != null && ListFilterPrescriptionsHasHIPay[0].ListPatientCodeSkip.Contains(CurPatientCode))
                        || (ListFilterPrescriptionsHasHIPay[0].ListICDSkip != null && ListFilterPrescriptionsHasHIPay[0].ListICDSkip.Contains(CurICD10Main)))
                    {
                        if (MessageBox.Show(eHCMSResources.Z2792_G1_ThuocTrongToaVuotTran, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) != MessageBoxResult.OK)
                        {
                            return false;
                        }
                    }
                    else
                    {
                        MessageBox.Show(string.Format(eHCMSResources.Z3045_G1_ThuocTrongToaVuotTran, (tempPayment - ListFilterPrescriptionsHasHIPay[0].GetHIPayFrom(CurICD10Main, LatestePrecriptions.PrescriptionDetails)).ToString("#,#.##")), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                        return false;
                    }
                }
                else
                {
                    if (MessageBox.Show(eHCMSResources.Z2792_G1_ThuocTrongToaVuotTran, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) != MessageBoxResult.OK)
                    {
                        return false;
                    }
                }
            }
            //▲====== #035

            return true;
        }
        public bool CheckChongChiDinhForDiagnosis()
        {
            bBlock = false;
            StringBuilder sb = new StringBuilder();
            string error = "";
           
            error += ErrCheckChongChiDinh();
            error += ErrCheckChongChiDinh_TrongNgay();

            if (!string.IsNullOrEmpty(error))
            {
                IErrorBold MessBox = Globals.GetViewModel<IErrorBold>();
                MessBox.FireOncloseEvent = true;
                if (bBlock)
                {
                    MessBox.isCheckBox = false;
                    MessBox.SetMessage(error + Environment.NewLine + eHCMSResources.A0028_G1_Msg_InfoKhTheTaoHoacCNhatCD, "");
                }
                else
                {
                    MessBox.IsShowReason = true;
                    MessBox.SetMessage(error, eHCMSResources.Z0627_G1_TiepTucLuu);
                    //20191028 TBL: BM 0018509: Toa thuốc có Reason thì khi cập nhật đem Reason lên để trên popup cho người dùng hiệu chỉnh
                    MessBox.Reason = LatestePrecriptions.Reason;
                }
                GlobalsNAV.ShowDialog_V3(MessBox);
                if (!MessBox.IsAccept)
                {
                    return false;
                }
                LatestePrecriptions.Reason = MessBox.Reason;
            }
            else //20191029 TBL: Trường hợp lúc đầu có cảnh báo và đã nhập lý do nhưng sau đó số lượng thuốc không đủ thì chỉnh sửa lại số lượng thuốc và không còn cảnh báo nữa thì set Reason = ""
            {
                LatestePrecriptions.Reason = "";
            }
            return true;
        }

        public void btnSaveAddNew()
        {
            if (Globals.IsLockRegistration(Registration_DataStorage.CurrentPatientRegistration.RegLockFlag, eHCMSResources.Z1079_G1_LuuVaPhatHanhToa))
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

            //this.HideBusyIndicator();
        }

        public void btnUpdate()
        {
            if (Globals.IsLockRegistration(Registration_DataStorage.CurrentPatientRegistration.RegLockFlag, eHCMSResources.Z1050_G1_CNhatToa.ToLower()))
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

            //this.HideBusyIndicator();

        }

        public bool checkHasHiPrescriptIssue()
        {
            //20190106 TTM: Không cần kiểm tra này nữa vì bệnh viện đa khoa không cần phải chặn 2 toa có BHYT (chuyên khoa mới cần).
            return false;
            //if (allPrescriptionIssueHistory == null
            //    || allPrescriptionIssueHistory.Count < 1
            //    || LatestePrecriptions.PrescriptionIssueHistory.HisID < 1)
            //{
            //    return false;
            //}
            //foreach (var item in allPrescriptionIssueHistory)
            //{
            //    if (LatestePrecriptions.PrescriptID != item.PrescriptID
            //        && LatestePrecriptions.PrescriptionIssueHistory.PtRegDetailID != item.PtRegDetailID
            //        && item.HisID > 0)
            //    {
            //        return true;
            //    }
            //}
            //return false;
        }
        public IEnumerator<IResult> CoroutinePrescriptions_Add()
        {
            if (checkHasHiPrescriptIssue())
            {
                var dialog = new MessageWarningShowDialogTask(string.Format("{0}!", eHCMSResources.Z1258_G1_DKDaCoToaBHYT), eHCMSResources.Z1259_G1_TiepTucDK);
                yield return dialog;
                if (!dialog.IsAccept)
                {
                    //Cộng lại dòng trống
                    AddNewBlankDrugIntoPrescriptObjectNew();
                    //Cộng lại dòng trống
                    yield break;
                }
            }

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


            // TxD 09/04/2014: Added the following Warning to Remind just in case the Tick Box was UNTICKED incidentally.
            if (RegistrationCoverByHI() && !isHisID)
            {
                warnConfDlg = new WarningWithConfirmMsgBoxTask(string.Format("{0}!", eHCMSResources.Z1266_G1_DKCoBHYTXNhanTaoMoiToa), eHCMSResources.Z1267_G1_XNhanTaoMoi);
                yield return warnConfDlg;
                if (!warnConfDlg.IsAccept)
                {
                    //Cộng lại dòng trống
                    AddNewBlankDrugIntoPrescriptObjectNew();
                    //Cộng lại dòng trống
                    warnConfDlg = null;
                    yield break;
                }
            }

            warnConfDlg = null;

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

        WarningWithConfirmMsgBoxTask warnConfDlg = null;
        WarningWithConfirmMsgBoxTask warnOfDrug = null;
        WarningWithConfirmMsgBoxTask confirmQtyBeforeSave = null;

        public IEnumerator<IResult> CoroutinePrescriptions_Update()
        {
            if (checkHasHiPrescriptIssue())
            {
                var dialog = new MessageWarningShowDialogTask(string.Format("{0}!", eHCMSResources.Z1258_G1_DKDaCoToaBHYT), eHCMSResources.Z1259_G1_TiepTucDK);
                yield return dialog;
                if (!dialog.IsAccept)
                {
                    //Cộng lại dòng trống
                    AddNewBlankDrugIntoPrescriptObjectNew();
                    //Cộng lại dòng trống
                    yield break;
                }
            }

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


            // TxD 09/04/2014: Added the following Warning to Remind just in case the Tick Box was UNTICKED incidentally.
            if (RegistrationCoverByHI() && !isHisID)
            {
                warnConfDlg = new WarningWithConfirmMsgBoxTask(string.Format("{0}!", eHCMSResources.Z1266_G1_DKCoBHYTXNhanTaoMoiToa), eHCMSResources.Z1267_G1_XNhanTaoMoi);
                yield return warnConfDlg;
                if (!warnConfDlg.IsAccept)
                {
                    //Cộng lại dòng trống
                    AddNewBlankDrugIntoPrescriptObjectNew();
                    //Cộng lại dòng trống
                    warnConfDlg = null;
                    yield break;
                }
            }

            warnConfDlg = null;

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

        public IEnumerator<IResult> MessageWarningShowDialogTask(string strMessage, bool IsCheckBoxShow = false, string CheckBoxText = null)
        {
            var dialog = new MessageWarningShowDialogTask(strMessage, CheckBoxText == null ? eHCMSResources.K1576_G1_CBao : CheckBoxText, IsCheckBoxShow);
            yield return dialog;
            yield break;
        }

        public IEnumerator<IResult> CoroutinePrescriptions_DuocSiEdit()
        {
            //KMx: Thêm cấu hình ShowApptCheck (trong bảng RefApplicationConfigs), vì 1 số phòng khám tư không dùng chức năng hẹn bệnh (02/05/2014 11:40).
            if (!LatestePrecriptions.HasAppointment && Globals.ServerConfigSection.CommonItems.ShowApptCheck)
            {
                var dialog = new PresApptShowDialogTask();
                yield return dialog;
                ObjToaOfDuocSi.HasAppointment = dialog.HasAppointment;
            }
            Prescriptions_DuocSiEdit();

            yield break;
        }

        public IEnumerator<IResult> CoroutinePrescriptions_DuocSiEditDuocSi()
        {
            //KMx: Thêm cấu hình ShowApptCheck (trong bảng RefApplicationConfigs), vì 1 số phòng khám tư không dùng chức năng hẹn bệnh (02/05/2014 11:40).
            if (!LatestePrecriptions.HasAppointment && Globals.ServerConfigSection.CommonItems.ShowApptCheck)
            {
                var dialog = new PresApptShowDialogTask();
                yield return dialog;
                ObjToaOfDuocSi.HasAppointment = dialog.HasAppointment;
            }
            Prescriptions_DuocSiEditDuocSi();
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
            ProcessCheckStatusPCLRequest(true);
        }
        private void CreateNewFunc()
        {
            //20191127 TBL: Nếu thấy ServiceRecID của chẩn đoán cũ trước đó thì không cho tạo mới toa thuốc để tránh bị lỗi 
            if (IsHavePCLRequestUnfinished)
            {
                MessageBox.Show(eHCMSResources.Z3252_G1_ChuaThucHienCLS, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
            if (CS_DS.DiagTreatment.ServiceRecID != ObjDiagnosisTreatment_Current.ServiceRecID)
            {
                MessageBox.Show(eHCMSResources.Z2925_G1_VuiLongNhapCD, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
            //20191127 
            //if (CS_DS.DiagTreatment.V_TreatmentType == 82508
            //  && (Registration_DataStorage.CurrentPatientRegistrationDetail.MedServiceID != 61685
            //      || Registration_DataStorage.CurrentPatientRegistration.HisID == null
            //      || Registration_DataStorage.PatientServiceRecordCollection.Count > 1)
            //  )
            //{
            //    return;
            //}
            ButtonCreateNew();

            NewPrecriptions = new Prescription();
            NewPrecriptions.ServiceRecID = ObjDiagnosisTreatment_Current.ServiceRecID;
            NewPrecriptions.PtRegistrationID = Registration_DataStorage.CurrentPatientRegistration == null ? null : (long?)Registration_DataStorage.CurrentPatientRegistration.PtRegistrationID;
            NewPrecriptions.Diagnosis = string.IsNullOrEmpty(ObjDiagnosisTreatment_Current.DiagnosisFinal) ? string.IsNullOrEmpty(ObjDiagnosisTreatment_Current.Diagnosis) ? null : ObjDiagnosisTreatment_Current.Diagnosis.Trim() : ObjDiagnosisTreatment_Current.DiagnosisFinal.Trim();
            NewPrecriptions.NDay = 0;

            NewPrecriptions.PatientID = Registration_DataStorage.CurrentPatient == null ? null : (long?)Registration_DataStorage.CurrentPatient.PatientID;

            NewPrecriptions.ConsultantDoctor = new Staff();

            NewPrecriptions.CreatorStaffID = Globals.LoggedUserAccount.Staff.StaffID;
            NewPrecriptions.ObjCreatorStaffID = new Staff();
            NewPrecriptions.ObjCreatorStaffID.StaffID = Globals.LoggedUserAccount.Staff.StaffID;
            NewPrecriptions.ObjCreatorStaffID.FullName = Globals.LoggedUserAccount.Staff.FullName;

            NewPrecriptions.ExamDate = ObjDiagnosisTreatment_Current.PatientServiceRecord == null ? null : (DateTime?)ObjDiagnosisTreatment_Current.PatientServiceRecord.ExamDate;

            NewPrecriptions.V_PrescriptionNotes = (long)AllLookupValues.V_PrescriptionNotes.TOAGOC;
            NewPrecriptions.V_PrescriptionType = CurrentPrescriptionType == null ? null : (long?)CurrentPrescriptionType.LookupID;

            NewPrecriptions.PrescriptionIssueHistory = new PrescriptionIssueHistory();
            NewPrecriptions.PrescriptionIssueHistory.DeptLocID = Globals.DeptLocation.DeptLocationID;

            if (Globals.ServerConfigSection.CommonItems.OrganizationUseSoftware > 0)
            {
                NewPrecriptions.HasAppointment = true;
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
            BackupCurPrescriptionItem();
            LatestePrecriptions = NewPrecriptions.DeepCopy();

            AddNewBlankDrugIntoPrescriptObjectNew();
            /*▼====: #004*/
            LatestePrecriptions.SetDataChanged();
            /*▲====: #004*/
            if (CtrtbSoTuan != null)
            {
                CtrtbSoTuan.Focus();
            }
        }
        //▼===== #020
        private ObservableCollection<PrescriptionDetail> _tmpPrescriptionDetails;
        public ObservableCollection<PrescriptionDetail> tmpPrescriptionDetails
        {
            get
            {
                return _tmpPrescriptionDetails;
            }
            set
            {
                if (_tmpPrescriptionDetails != value)
                {
                    _tmpPrescriptionDetails = value;
                    NotifyOfPropertyChange(() => tmpPrescriptionDetails);
                }
            }
        }
        private ObservableCollection<PrescriptionDetail> _tmpForShowPrescriptionDetails;
        public ObservableCollection<PrescriptionDetail> tmpForShowPrescriptionDetails
        {
            get
            {
                return _tmpForShowPrescriptionDetails;
            }
            set
            {
                if (_tmpForShowPrescriptionDetails != value)
                {
                    _tmpForShowPrescriptionDetails = value;
                    NotifyOfPropertyChange(() => tmpForShowPrescriptionDetails);
                }
            }
        }
        public void CheckHIBeforCreateNewAndCopy()
        {
            if (Registration_DataStorage.CurrentPatientRegistration != null &&
                (Registration_DataStorage.CurrentPatientRegistration.HisID == null || Registration_DataStorage.CurrentPatientRegistration.HisID == 0) &&
                LatestePrecriptions != null && LatestePrecriptions.PrescriptionDetails != null && LatestePrecriptions.PrescriptionDetails.Count > 0)
            {
                tmpForShowPrescriptionDetails = new ObservableCollection<PrescriptionDetail>();
                tmpPrescriptionDetails = new ObservableCollection<PrescriptionDetail>(LatestePrecriptions.PrescriptionDetails);
                if (Globals.ServerConfigSection.ConsultationElements.LevelWarningWhenCreateNewAndCopy == 1)
                {
                    foreach (var x in tmpPrescriptionDetails)
                    {
                        if (x.InsuranceCover == true)
                        {
                            LatestePrecriptions.PrescriptionDetails.Remove(x);
                        }
                    }
                }
                else if (Globals.ServerConfigSection.ConsultationElements.LevelWarningWhenCreateNewAndCopy == 2)
                {
                    foreach (var x in tmpPrescriptionDetails)
                    {
                        if (x.InsuranceCover == true)
                        {
                            tmpForShowPrescriptionDetails.Add(x);
                        }
                    }
                }
            }
        }
        //▲===== #020
        public void btnCreateNewAndCopy()
        {
            ProcessCheckStatusPCLRequest(false);
        }
        private void CreateNewAndCopyFunc()
        {
            if (IsHavePCLRequestUnfinished)
            {
                MessageBox.Show(eHCMSResources.Z3252_G1_ChuaThucHienCLS, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
            //20191127 TBL: Nếu thấy ServiceRecID của chẩn đoán cũ trước đó thì không cho tạo mới dựa trên cũ toa thuốc để tránh bị lỗi
            if (CS_DS.DiagTreatment.ServiceRecID != ObjDiagnosisTreatment_Current.ServiceRecID)
            {
                MessageBox.Show(eHCMSResources.Z2925_G1_VuiLongNhapCD, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
            //20191127 
            /*▼====: #004*/
            LatestePrecriptions.SetDataChanged();
            /*▲====: #004*/
            long PrescriptID = LatestePrecriptions.PrescriptID;
            IsPhatHanhLai = false;
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
                //▼===== #020
                if (Globals.ServerConfigSection.ConsultationElements.LevelWarningWhenCreateNewAndCopy > 0)
                {
                    CheckHIBeforCreateNewAndCopy();
                }
                //▲===== #020
                if (PrescriptID > 0 && LatestePrecriptions.V_RegistrationType == AllLookupValues.RegistrationType.NOI_TRU)
                {
                    GetPrescriptionDetailsByPrescriptID_InPt(PrescriptID, true);
                }
                else if (PrescriptID > 0)
                {
                    GetPrescriptionDetailsByPrescriptID(PrescriptID, true);
                }

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
                return _btnSaveAddNewVisibility && IsShowSummaryContent;
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
            if (grdPrescription.IsValid == false)
            {
                MessageBox.Show(string.Format("{0} {1}", eHCMSResources.A0540_G1_Msg_InfoDataKhHopLe, eHCMSResources.A0541_G1_Msg_InfoDataToaThuocKhHopLe), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
            Action<IePrescriptionCalcNotSave> onInitDlg = delegate (IePrescriptionCalcNotSave typeInfo)
            {
                typeInfo.ObjPrescription = LatestePrecriptions;
                typeInfo.StoreID = StoreID.Value;

                short LoaiBenhNhan = 0;//--1;bh,0;binh thuong


                if (RegistrationCoverByHI())
                {
                    LoaiBenhNhan = 1;
                }
                typeInfo.RegistrationType = LoaiBenhNhan;
                typeInfo.Registration_DataStorage = Registration_DataStorage;
                typeInfo.Calc();
            };
            GlobalsNAV.ShowDialog<IePrescriptionCalcNotSave>(onInitDlg);
        }

        public void btCalcAll()
        {
            if (grdPrescription == null)
            {
                MessageBox.Show(eHCMSResources.A0365_G1_Msg_InfoChonThuocDeRaToa, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
            if (grdPrescription.IsValid == false)
            {
                MessageBox.Show(string.Format("{0} {1}", eHCMSResources.A0540_G1_Msg_InfoDataKhHopLe, eHCMSResources.A0541_G1_Msg_InfoDataToaThuocKhHopLe), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
            Action<IePrescriptionCalcNotSave> onInitDlg = delegate (IePrescriptionCalcNotSave typeInfo)
            {
                typeInfo.ObjPrescription = LatestePrecriptions;
                typeInfo.StoreID = StoreID.Value;

                short LoaiBenhNhan = 0;//--1;bh,0;binh thuong


                if (RegistrationCoverByHI())
                {
                    LoaiBenhNhan = 1;
                }
                typeInfo.RegistrationType = LoaiBenhNhan;
                typeInfo.Registration_DataStorage = Registration_DataStorage;
                typeInfo.CalcAll();
            };
            GlobalsNAV.ShowDialog<IePrescriptionCalcNotSave>(onInitDlg);
        }
        #endregion

        public void Handle(PrescriptionDrugNotInCatSelectedEvent<PrescriptionDetail, int> message)
        {
            if (message != null && this.GetView() != null)
            {
                UpdateLatestePrecriptionsDrugNotInCat(message.PrescriptionDrugNotInCat, message.Index);
            }
        }

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

        private string CheckAllThuocBiDiUng()
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
                bBlock = true;
                return (("{0}:", eHCMSResources.A0504_G1_Msg_InfoDSThuocDiUng) + Environment.NewLine + sb.ToString());
            }
            return "";
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

        public void Handle(PrescriptionNoteTempType_Change obj)
        {
            RefreshLookup();
        }

        public bool PrintInfoBH
        {
            get { return Registration_DataStorage.CurrentPatientRegistration != null && Registration_DataStorage.CurrentPatientRegistration.HisID > 0; }
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
        public void Handle(LoadPrescriptionAfterSaved message)
        {
            if (Registration_DataStorage.PatientServiceRecordCollection == null || Registration_DataStorage.PatientServiceRecordCollection.Count <= 0
                || Registration_DataStorage.PatientServiceRecordCollection[0].DiagnosisTreatments == null || Registration_DataStorage.PatientServiceRecordCollection[0].DiagnosisTreatments.Count <= 0)
            {
                MessageBox.Show(eHCMSResources.A0730_G1_Msg_InfoKhTimThayCDCuaBN, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                return;
            }

            if ((Registration_DataStorage.PatientServiceRecordCollection[0].PrescriptionIssueHistories == null || Registration_DataStorage.PatientServiceRecordCollection[0].PrescriptionIssueHistories.Count <= 0) && IsShowSummaryContent)
            {
                MessageBox.Show(eHCMSResources.A0748_G1_Msg_InfoKhTimThayToaThuoc, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                return;
            }

            LatestePrecriptions = new Prescription();

            ObjDiagnosisTreatment_Current = Registration_DataStorage.PatientServiceRecordCollection[0].DiagnosisTreatments[0];

            btnUndoIsEnabled = false;
            btnUpdateIsEnabled = false;

            //GetToaThuocDaCo();
            CheckBeforePrescrip();
            GetAllPrescription_TrongNgay();
            GetAllPrescription_BaoHiemConThuoc();
            GetRemainingMedicineDays();
            if (!IsUpdateWithoutChangeDoctorIDAndDatetime)
            {
                if (Registration_DataStorage.PatientServiceRecordCollection[0].PrescriptionIssueHistories[0].IssuerStaffID != Globals.LoggedUserAccount.StaffID)
                {
                    //Chỉ Có Người Tạo Ra Toa Thuốc này mới được phép Chỉnh Sửa!
                    PrescripState = PrescriptionState.PublishNewPrescriptionState;
                }
                else
                {
                    PrescripState = PrescriptionState.EditPrescriptionState;
                }
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

        //==== 20161004 CMN Begin: Call services check drug interactions
        public void btnIntraction()
        {
            if (LatestePrecriptions.PrescriptionDetails.Count > 0)
            {
                string[] mGenericName = LatestePrecriptions.PrescriptionDetails.Where(x => x.DrugID != 0).Select(x => x.SelectedDrugForPrescription.GenericName).ToArray();
                string[] mBrandName = LatestePrecriptions.PrescriptionDetails.Where(x => x.DrugID != 0).Select(x => x.SelectedDrugForPrescription.BrandName).ToArray();
                var t = new Thread(() =>
                {
                    using (var serviceFactory = new ePrescriptionsServiceClient())
                    {
                        this.ShowBusyIndicator();
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginCheckDrugInteraction(mGenericName, mBrandName, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                string DrugIntractions = contract.EndCheckDrugInteraction(mGenericName, mBrandName, asyncResult);
                                this.HideBusyIndicator();
                                if (!String.IsNullOrEmpty(DrugIntractions))
                                    MessageBox.Show(DrugIntractions);
                                else
                                    MessageBox.Show(eHCMSResources.A0669_G1_Msg_InfoKhCoTuongTacThuoc);
                                //System.Windows.Threading.Dispatcher dispatcher = Deployment.Current.Dispatcher;
                                //dispatcher.BeginInvoke(() =>
                                //{
                                //    var msgVm = Globals.GetViewModel<IDrugIntractionMessageBox>();
                                //    msgVm.IntractionText = DrugIntractions;
                                //    Globals.ShowDialog(msgVm as Conductor<object>);
                                //});
                            }
                            catch (Exception ex)
                            {
                                this.HideBusyIndicator();
                                MessageBox.Show(ex.Message, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                            }
                            finally
                            {
                            }

                        }), null);
                    }
                });
                t.Start();
            }
        }
        //20161004 CMN End
        private ObservableCollection<RefTreatmentRegimen> _RefTreatmentRegimenCollection;
        public ObservableCollection<RefTreatmentRegimen> RefTreatmentRegimenCollection
        {
            get => _RefTreatmentRegimenCollection; set
            {
                _RefTreatmentRegimenCollection = value;
                NotifyOfPropertyChange(() => RefTreatmentRegimenCollection);
            }
        }
        private ObservableCollection<RefTreatmentRegimenDrugDetail> _RefTreatmentRegimenDrugDetails;
        public ObservableCollection<RefTreatmentRegimenDrugDetail> RefTreatmentRegimenDrugDetails
        {
            get { return _RefTreatmentRegimenDrugDetails; }
            set
            {
                _RefTreatmentRegimenDrugDetails = value;
                NotifyOfPropertyChange(() => RefTreatmentRegimenDrugDetails);
            }
        }
        private ObservableCollection<GetDrugForSellVisitor> _DrugsInTreatmentRegimen;
        public ObservableCollection<GetDrugForSellVisitor> DrugsInTreatmentRegimen
        {
            get => _DrugsInTreatmentRegimen;
            set
            {
                _DrugsInTreatmentRegimen = value;
                NotifyOfPropertyChange(() => DrugsInTreatmentRegimen);
            }
        }

        private bool _IsSearchByTreatmentRegimenVisibility = Globals.ServerConfigSection.ConsultationElements.EnableTreatmentRegimen;
        public bool IsSearchByTreatmentRegimenVisibility
        {
            get => _IsSearchByTreatmentRegimenVisibility; set
            {
                _IsSearchByTreatmentRegimenVisibility = value;
                NotifyOfPropertyChange(() => IsSearchByTreatmentRegimenVisibility);
            }
        }
        private bool _IsSearchByTreatmentRegimen = Globals.ServerConfigSection.ConsultationElements.CheckedTreatmentRegimen;
        public bool IsSearchByTreatmentRegimen
        {
            get => _IsSearchByTreatmentRegimen; set
            {
                _IsSearchByTreatmentRegimen = value;
                NotifyOfPropertyChange(() => IsSearchByTreatmentRegimen);
            }
        }
        public void btnShowTreatmentRegimen()
        {
            //===: #015
            AddListICD10Codes();
            if (listICD10Codes.Count == 0)
            {
                MessageBox.Show(eHCMSLanguage.eHCMSResources.Z2335_G1_KhongCoICD10);
                return;
            }
            GlobalsNAV.ShowDialog<ITreatmentRegimen>((ITreatmentRegimen aView) =>
            {
                aView.PtRegDetailID = Registration_DataStorage.CurrentPatientRegistrationDetail == null ? null : (long?)Registration_DataStorage.CurrentPatientRegistrationDetail.PtRegDetailID;
                aView.listICD10Codes = listICD10Codes;
            }, null, false, false, new Size(1000, 600));
            //===: #015
        }

        private bool _IsShowSummaryContent = true;
        public bool IsShowSummaryContent
        {
            get => _IsShowSummaryContent; set
            {
                _IsShowSummaryContent = value;
                NotifyOfPropertyChange(() => IsShowSummaryContent);
                NotifyOfPropertyChange(() => btnSaveAddNewVisibility);
                NotifyOfPropertyChange(() => mUpdatePrescriptionState);
            }
        }
        /*▼====: #004*/

        public void ResetPrescriptionInfoChanged()
        {
            LatestePrecriptions.ResetDataChanged();
        }

        //private bool _IsPrescriptionChanged = false;
        public bool IsPrescriptionInfoChanged
        {
            get
            {
                return LatestePrecriptions.IsPrescriptionDataChanged;
            }
        }
        /*▲====: #004*/

        public void ChangeStatesAfterUpdated(bool IsUpdate = false)
        {
            /*▼====: #007*/
            //PrescripState = PrescriptionState.EditPrescriptionState;
            /*▲====: #007*/
            PrecriptionsForPrint = ObjTaoThanhToaMoi;
            IsEnabledAutoComplete = false;
            IsEnabled = false;
            btnSaveAddNewIsEnabled = false;
            btnUndoIsEnabled = false;
            btnCreateNewIsEnabled = true;
            btnEditIsEnabled = true;
            btnCreateAndCopyIsEnabled = true;
            IsEnabledPrint = true;
            btChonChanDoanIsEnabled = false;
        }
        
        public bool CheckValidPrescription()
        {
            if (!btnSaveAddNewIsEnabled && !btnUpdateIsEnabled)
            {
                return true;
            }
            
            if (Globals.IsLockRegistration(Registration_DataStorage.CurrentPatientRegistration.RegLockFlag, eHCMSResources.Z1079_G1_LuuVaPhatHanhToa))
            {
                return false;
            }
            if (IsPrescriptionInfoChanged && !CheckPrescriptionBeforeSave())
            {
                return false;
            }
            if (!CheckHIValidDate())
            {
                return false;
            }
            if (tmpForShowPrescriptionDetails != null && tmpForShowPrescriptionDetails.Count > 0 && Globals.ServerConfigSection.ConsultationElements.LevelWarningWhenCreateNewAndCopy == 2)
            {
                MessageBox.Show("Bệnh nhân không bảo hiểm nhưng trong toa có thuốc nằm trong danh mục bảo hiểm (Tô hồng). Vui lòng xoá hết thuốc bảo hiểm ra trước khi lưu.");
                return false;
            }
            if (!IsEnableAddPrescription)
            {
                MessageBox.Show("Bệnh nhân có cách điều trị không thể ra toa thuốc. Vui lòng chọn lại cách điều trị");
                return false;
            }
            if (ConfirmSoLuongNotEnoughBeforeSave())
            {
                if (checkHasHiPrescriptIssue())
                {
                    var dialog = new MessageWarningShowDialogTask(string.Format("{0}!", eHCMSResources.Z1258_G1_DKDaCoToaBHYT), eHCMSResources.Z1259_G1_TiepTucDK);
                    dialog.Execute(null);
                    if (!dialog.IsAccept)
                    {
                        return false;
                    }
                }
                string WarningMsg = "";
                if (!CheckWarningOfDrug(out WarningMsg))
                {
                    var dialog = new WarningWithConfirmMsgBoxTask(WarningMsg, eHCMSResources.Z0627_G1_TiepTucLuu);
                    dialog.Execute(null);
                    if (!dialog.IsAccept)
                    {
                        return false;
                    }
                }
                if (!CheckQtyUserInput(out WarningMsg))
                {
                    var dialog = new WarningWithConfirmMsgBoxTask(WarningMsg, eHCMSResources.Z0627_G1_TiepTucLuu);
                    dialog.Execute(null);
                    if (!dialog.IsAccept)
                    {
                        return false;
                    }
                }
                if (RegistrationCoverByHI() && !isHisID)
                {
                    var dialog = new WarningWithConfirmMsgBoxTask(string.Format("{0}!", eHCMSResources.Z1266_G1_DKCoBHYTXNhanTaoMoiToa), eHCMSResources.Z1267_G1_XNhanTaoMoi);
                    dialog.Execute(null);
                    if (!dialog.IsAccept)
                    {
                        return false;
                    }
                }

                //▼====== #010: Do không đặt biến kiểm tra xem toa thuốc có thay đổi không mà để trống huơ trống hoắc dẫn đến việc không thay đổi toa thuốc, lưu chẩn 
                //              đoán không mà vẫn show popup hẹn bệnh.
                if (IsPrescriptionInfoChanged)
                //▲====== #010
                {
                    //▼====== #003: Thêm cảnh báo khi người dùng không tick vào hẹn bệnh cho bệnh nhân (cho giống với con đường cũ).
                    if (!LatestePrecriptions.HasAppointment && Globals.ServerConfigSection.CommonItems.ShowApptCheck && !Globals.IsUse1192)
                    {
                        IPrescriptionApptCheck prescriptionApptCheckVM = Globals.GetViewModel<IPrescriptionApptCheck>();
                        GlobalsNAV.ShowDialog_V3<IPrescriptionApptCheck>(prescriptionApptCheckVM);
                        LatestePrecriptions.HasAppointment = prescriptionApptCheckVM.HasAppointment;
                    }
                }
                //▲====== #003
                //▼====== #013
                //if (IsPrescriptionInfoChanged && Registration_DataStorage.CurrentPatientRegistration.HisID > 0)
                //{
                //    bool IsAllHIDrug = true;
                //    foreach (var tmpInsuranceCover in LatestePrecriptions.PrescriptionDetails)
                //    {
                //        if (!tmpInsuranceCover.InsuranceCover)
                //        {
                //            var dialog = new WarningWithConfirmMsgBoxTask(string.Format("{0}!", eHCMSResources.Z2326_G1_WarningThuocNgoaiDM), eHCMSResources.Z2327_G1_BNDongYMua);
                //            dialog.Execute(null);
                //            if (!dialog.IsAccept)
                //            {
                //                IsOutCatConfirmed = false;
                //                return false;
                //            }
                //            else
                //            {
                //                IsAllHIDrug = false;
                //                IsOutCatConfirmed = true;
                //            }
                //            break;
                //        }
                //    }
                //    if (IsAllHIDrug)
                //    {
                //        IsOutCatConfirmed = false;
                //    }
                //}
                //else
                //{
                //    IsOutCatConfirmed = false;
                //}
                //▲====== #013
                //▼===== #026 20200114 TTM: Chuyển kiểm tra thuốc ngoài danh mục bảo hiểm về kiểm tra dựa vào dịch vụ thay vì đăng ký.
                //if (IsPrescriptionInfoChanged && Registration_DataStorage.CurrentPatientRegistration.HisID > 0)
                if (IsPrescriptionInfoChanged && Registration_DataStorage.CurrentPatientRegistrationDetail.HisID != null && Registration_DataStorage.CurrentPatientRegistrationDetail.HisID > 0)
                {
                    string warning = "";
                    bool IsAllHIDrug = true;
                    foreach (var tmpInsuranceCover in LatestePrecriptions.PrescriptionDetails)
                    {
                        if (!tmpInsuranceCover.InsuranceCover)
                        {
                            warning += "- " + tmpInsuranceCover.BrandName + Environment.NewLine;
                        }
                    }
                    if (!string.IsNullOrEmpty(warning))
                    {
                        //▼===== #056
                        //var dialog = new WarningWithConfirmMsgBoxTask(string.Format("{0}: {1}", eHCMSResources.Z2326_G1_WarningThuocNgoaiDM, Environment.NewLine + warning), eHCMSResources.Z2327_G1_BNDongYMua);
                        //dialog.Execute(null);
                        //if (!dialog.IsAccept)
                        //{
                        //    IsOutCatConfirmed = false;
                        //    return false;
                        //}
                        //else
                        //{
                            IsAllHIDrug = false;
                            IsOutCatConfirmed = true;
                        //}
                        //▲====== #056
                    }
                    if (IsAllHIDrug)
                    {
                        IsOutCatConfirmed = false;
                    }
                }
                else
                {
                    IsOutCatConfirmed = false;
                }
                return true;
                //▲===== #026
            }
            return false;
        }
        //▼====== #013
        public bool IsOutCatConfirmed = false;
        //▲====== #013
        public void ChangeStatesBeforeUpdate()
        {
            RemoveLastRowPrecriptionDetail();
            UpdateDefaultValueForNewPrecription();
            SetValueTaoThanhToaMoi_New();
        }

        public void btnEditDiagnosis()
        {
            if (!IsViewOnly)
            {
                return;
            }
            GlobalsNAV.ShowDialog<IConsultationOld>((ConsultationVM) =>
            {
                //▼===== 20191020 TTM:  Trước đây không cần đưa giá trị vào màn hình do xài Globals. Nhưng đã chuyển từ Globals sang biến nội bộ là Registration_DataStorage
                //                      nên cần set giá trị lại để không bị System Null Exception.
                ConsultationVM.Registration_DataStorage = Registration_DataStorage;

                ConsultationVM.IsUpdateFromPresciption = true;
            }, null, false, true, new Size(900, 600));
        }
        public bool CheckHIValidDate()
        {
            if (Registration_DataStorage.CurrentPatientRegistration != null
                && Registration_DataStorage.CurrentPatientRegistration.HisID > 0)
            {
                if (LatestePrecriptions == null)
                {
                    return false;
                }
                DateTime CheckDate = Globals.GetCurServerDateTime().AddDays((int)LatestePrecriptions.NDay);
                DateTime ValidHIDate = (DateTime)Registration_DataStorage.CurrentPatientRegistration.HealthInsurance.ValidDateTo;
                TimeSpan AfterEquals = CheckDate.Subtract(ValidHIDate);
                if (AfterEquals.Days > 0)
                {
                    if (MessageBox.Show(eHCMSResources.Z2439_G1_NgayToaVuotQuaHanBHYT
                        + "\n" + eHCMSResources.R0257_G1_HanSD + ": " + Registration_DataStorage.CurrentPatientRegistration.HealthInsurance.ValidDateTo
                        + "\n" + eHCMSResources.Z2440_G1_RaToaVuotQua + ": " + AfterEquals.Days + " " + eHCMSResources.Z2441_G1_NgaySoVoiThe
                        + "\n" + eHCMSResources.Z0889_G1_KTraLai, eHCMSResources.Z1359_G1_Warning, MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        public bool KiemTraDaDuyetToaChua()
        {
            if (Registration_DataStorage.CurrentPatientRegistration != null && Registration_DataStorage.CurrentPatientRegistration.ConfirmHIStaffID > 0)
            {
                MessageBox.Show(eHCMSResources.Z2446_G1_KhongChinhSuaToaKhiDaDuyet);
                return false;
            }
            return true;
        }
        public bool KiemTraDaXuatChua()
        {
            //Globals.ServerConfigSection.
            //if (true)
            //{
            //    MessageBox.Show(eHCMSResources.K3971_G1_XacNhanChinhSua);
            //    return false;
            //}
            return true;
        }

        private void GetAllPrescriptionDetails(List<Prescription> lstprescriptions)
        {
            if (lstprescriptions != null && lstprescriptions.Count > 0)
            {
                foreach (var prescription in lstprescriptions)
                {
                    GetPrescriptionDetailsByPrescriptID_V2(prescription.PrescriptID);
                }
            }
        }

        private bool CheckToaThuocBiTrungTrongNgay_TheoHoatChat(List<IList<PrescriptionDetail>> ListPrescriptionDetailTrongNgay, IList<PrescriptionDetail> pNeedSave, out string msg)
        {
            StringBuilder sb = new StringBuilder();
            msg = "";
            if (pNeedSave != null && pNeedSave.Count > 0)
            {
                foreach (var prescriptDetaiil in pNeedSave)
                {
                    if (!CheckThuocBiTrungTrongNgay_TheoHoatChat(ListPrescriptionDetailTrongNgay, prescriptDetaiil) && prescriptDetaiil.SelectedDrugForPrescription != null && prescriptDetaiil.SelectedDrugForPrescription.GenericName != null)
                    {
                        sb.AppendLine(string.Format("- " + prescriptDetaiil.SelectedDrugForPrescription.GenericName.Trim()));
                    }
                }
                msg = sb.ToString();
            }
            if (!string.IsNullOrEmpty(msg))
            {
                return true;
            }
            return false;
        }

        private bool CheckThuocBiTrungTrongNgay_TheoHoatChat(List<IList<PrescriptionDetail>> ListPrescriptionDetailTrongNgay, PrescriptionDetail pNeedSave)
        {
            if (ListPrescriptionDetailTrongNgay != null && ListPrescriptionDetailTrongNgay.Count > 0)
            {
                foreach (IList<PrescriptionDetail> listdetail_i in ListPrescriptionDetailTrongNgay)
                {
                    foreach (var item in listdetail_i)
                    {
                        if (item.PrescriptID != LatestePrecriptions.PrescriptID && item.DrugID != 0 && item.DrugID != null && item.SelectedDrugForPrescription != null && pNeedSave.SelectedDrugForPrescription != null && item.SelectedDrugForPrescription.GenericID == pNeedSave.SelectedDrugForPrescription.GenericID)
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        private List<RefGenericRelation> GetRefGenericRelation(long GenericID)
        {
            List<RefGenericRelation> ListRefGenericRelation = new List<RefGenericRelation>();
            if (Globals.MAPRefGenericRelation != null && Globals.MAPRefGenericRelation.Count > 0 && Globals.MAPRefGenericRelation.ContainsKey(GenericID))
            {
                ListRefGenericRelation = Globals.MAPRefGenericRelation[GenericID];
            }
            return ListRefGenericRelation;
        }
        private string KiemTraTuongTacHoatChatTrongNgay()
        {
            if (!Globals.ServerConfigSection.ConsultationElements.KiemTraQuanHeHoatChat || bBlock)
            {
                return "";
            }
            //Tạo diction các hoạt chất với các giá trị và true or false cho toa thuốc đang chỉnh sửa và tên hoạt chất
            Dictionary<long, object[]> GenericCollection = new Dictionary<long, object[]>();
            //Thêm các hoạt chất từ toa thuốc đang chỉnh sửa vào diction
            foreach (var item in LatestePrecriptions.PrescriptionDetails.Select(x => x.SelectedDrugForPrescription).Where(x => x != null && x.DrugID > 0))
            {
                if (GenericCollection.ContainsKey(item.GenericID))
                {
                    continue;
                }
                GenericCollection.Add(item.GenericID, new object[] { true, item.GenericName });
            }
            //Thêm các hoạt chất từ những toa thuốc khác vào diction
            if (ListPrescriptionDetailTrongNgay.SelectMany(x => x).Any(x => LatestePrecriptions != null && LatestePrecriptions.PrescriptID != x.PrescriptID))
            {
                foreach (var item in ListPrescriptionDetailTrongNgay.SelectMany(x => x).Where(x => LatestePrecriptions != null && LatestePrecriptions.PrescriptID != x.PrescriptID).Select(x => x.SelectedDrugForPrescription).Where(x => x != null && x.DrugID > 0))
                {
                    if (GenericCollection.ContainsKey(item.GenericID))
                    {
                        continue;
                    }
                    GenericCollection.Add(item.GenericID, new object[] { false, item.GenericName });
                }
            }
            bool IsHasBlockInteraction = false;
            bool IsHasInteraction = false;
            List<long> ListAddedGenericID = new List<long>();
            StringBuilder mStringBuilder = new StringBuilder();
            //Lặp tất cả các hoạt chất của toa hiện tại
            foreach (var item in GenericCollection.Where(x => Convert.ToBoolean(x.Value[0])))
            {
                //Lấy danh sách các hoạt chất tương tác với hoạt chất đang kiểm tra
                List<RefGenericRelation> ListInteractionGeneric = GetRefGenericRelation(item.Key);
                //Kiểm tra có hoạt chất tương tác tồn tại trong diction (khác hoạt chất đang kiểm tra)
                if (ListInteractionGeneric.Any(x => x.IsInteraction && x.V_InteractionWarningLevel.LookupID != (long)AllLookupValues.V_WarningLevel.Normal && x.SecondGeneric.GenericID != item.Key && GenericCollection.ContainsKey(x.SecondGeneric.GenericID)))
                {
                    IsHasInteraction = true;
                    ListAddedGenericID.Add(item.Key);
                    //Lặp danh sách hoạt chất tương tác để tạo chuỗi thông báo lỗi
                    foreach (var seconditem in ListInteractionGeneric.Where(x => x.IsInteraction && x.V_InteractionWarningLevel.LookupID != (long)AllLookupValues.V_WarningLevel.Normal && x.SecondGeneric.GenericID != item.Key && GenericCollection.ContainsKey(x.SecondGeneric.GenericID)).ToList())
                    {
                        if (!IsHasBlockInteraction && seconditem.V_InteractionWarningLevel.LookupID == (long)AllLookupValues.V_WarningLevel.Block)
                        {
                            IsHasBlockInteraction = true;
                        }
                        //Bỏ qua hoạt chất đã được kiểm tra trước đó
                        if (ListAddedGenericID.Contains(seconditem.SecondGeneric.GenericID))
                        {
                            continue;
                        }
                        //Tạo chuỗi thông báo lỗi
                        mStringBuilder.AppendLine(string.Format("- " + item.Value[1].ToString().Trim() + " tương tác với " + seconditem.SecondGeneric.GenericName + " [" + seconditem.V_InteractionSeverityLevel.ObjectValue + "] " + (seconditem.V_InteractionWarningLevel.LookupID == (long)AllLookupValues.V_WarningLevel.Block ? " (*)" : "") + (!Convert.ToBoolean(GenericCollection[seconditem.SecondGeneric.GenericID][0]) ? " (Ngoài toa)" : "")));
                    }
                }
            }
            LatestePrecriptions.IsWarningInteraction = IsHasInteraction;
            if (!IsHasInteraction)
            {
                return "";
            }
            if (!IsHasBlockInteraction)
            {
                return string.Format(eHCMSResources.Z2674_G1_ToaThuocCoHoatChatTuongTac + Environment.NewLine + mStringBuilder.ToString());
            }
            else if (IsHasBlockInteraction)
            {
                bBlock = true;
                return string.Format(eHCMSResources.Z2674_G1_ToaThuocCoHoatChatTuongTac + Environment.NewLine + mStringBuilder.ToString());
            }
            return "";
        }

        private string KiemTraTuongTuHoatChatTrongNgay()
        {
            if (!Globals.ServerConfigSection.ConsultationElements.KiemTraQuanHeHoatChat || bBlock)
            {
                return "";
            }
            //Tạo diction các hoạt chất với các giá trị và true or false cho toa thuốc đang chỉnh sửa và tên hoạt chất
            Dictionary<long, object[]> GenericCollection = new Dictionary<long, object[]>();
            //Thêm các hoạt chất từ toa thuốc đang chỉnh sửa vào diction
            foreach (var item in LatestePrecriptions.PrescriptionDetails.Select(x => x.SelectedDrugForPrescription).Where(x => x != null && x.DrugID > 0))
            {
                if (GenericCollection.ContainsKey(item.GenericID))
                {
                    continue;
                }
                GenericCollection.Add(item.GenericID, new object[] { true, item.GenericName });
            }
            //Thêm các hoạt chất từ những toa thuốc khác vào diction
            if (ListPrescriptionDetailTrongNgay.SelectMany(x => x).Any(x => LatestePrecriptions != null && LatestePrecriptions.PrescriptID != x.PrescriptID))
            {
                foreach (var item in ListPrescriptionDetailTrongNgay.SelectMany(x => x).Where(x => LatestePrecriptions != null && LatestePrecriptions.PrescriptID != x.PrescriptID).Select(x => x.SelectedDrugForPrescription).Where(x => x != null && x.DrugID > 0))
                {
                    if (GenericCollection.ContainsKey(item.GenericID))
                    {
                        continue;
                    }
                    GenericCollection.Add(item.GenericID, new object[] { false, item.GenericName });
                }
            }
            bool IsHasSimilar = false;
            List<long> ListAddedGenericID = new List<long>();
            StringBuilder mStringBuilder = new StringBuilder();
            //Lặp tất cả các hoạt chất của toa hiện tại
            foreach (var item in GenericCollection.Where(x => Convert.ToBoolean(x.Value[0])))
            {
                //Lấy danh sách các hoạt chất tương tự với hoạt chất đang kiểm tra
                List<RefGenericRelation> ListInteractionGeneric = GetRefGenericRelation(item.Key);
                //Kiểm tra có hoạt chất tương tự tồn tại trong diction (khác hoạt chất đang kiểm tra)
                if (ListInteractionGeneric.Any(x => x.IsSimilar && x.SecondGeneric.GenericID != item.Key && GenericCollection.ContainsKey(x.SecondGeneric.GenericID)))
                {
                    IsHasSimilar = true;
                    ListAddedGenericID.Add(item.Key);
                    //Lặp danh sách hoạt chất tương tự để tạo chuỗi thông báo lỗi
                    foreach (var seconditem in ListInteractionGeneric.Where(x => x.IsSimilar && x.SecondGeneric.GenericID != item.Key && GenericCollection.ContainsKey(x.SecondGeneric.GenericID)).ToList())
                    {
                        //Bỏ qua hoạt chất đã được kiểm tra trước đó
                        if (ListAddedGenericID.Contains(seconditem.SecondGeneric.GenericID))
                        {
                            continue;
                        }
                        //Tạo chuỗi thông báo lỗi
                        mStringBuilder.AppendLine(string.Format("- " + item.Value[1].ToString().Trim() + " tương tự với " + seconditem.SecondGeneric.GenericName + (!Convert.ToBoolean(GenericCollection[seconditem.SecondGeneric.GenericID][0]) ? " (Ngoài toa)" : "")));
                    }
                }
            }
            LatestePrecriptions.IsWarningSimilar = IsHasSimilar;
            if (!IsHasSimilar)
            {
                return "";
            }
            else
            {
                return string.Format(eHCMSResources.Z2675_G1_ToaThuocCoHoatChatTuongTu + Environment.NewLine + mStringBuilder.ToString());
            }
        }

        private bool _IsUpdateWithoutChangeDoctorIDAndDatetime = false;
        public bool IsUpdateWithoutChangeDoctorIDAndDatetime
        {
            get
            {
                return _IsUpdateWithoutChangeDoctorIDAndDatetime;
            }
            set
            {
                if (_IsUpdateWithoutChangeDoctorIDAndDatetime != value)
                {
                    _IsUpdateWithoutChangeDoctorIDAndDatetime = value;
                    NotifyOfPropertyChange(() => IsUpdateWithoutChangeDoctorIDAndDatetime);
                }
            }
        }
        //▼===== #018
        public void hplCheckDrugInfor_Click(Object pSelectedItem)
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

            GetDrugInformation(selPrescriptionDetail);

        }

        public void GetDrugInformation(PrescriptionDetail selPrescriptionDetail)
        {
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyDrugServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginGetDrugInformation(selPrescriptionDetail.DrugID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndGetDrugInformation(asyncResult);
                            if (results == null || results.RefGenericDrugDetail == null || string.IsNullOrEmpty(results.RefGenericDrugDetail.SdlDescription))
                            {
                                MessageBox.Show(eHCMSResources.Z2738_G1_ThongTinThuocKhongHopLe);
                            }
                            else
                            {
                                Action<IDrugInformation> onInitDlg = delegate (IDrugInformation proAlloc)
                                {
                                    proAlloc.SelectedDrugInformation = results;
                                };
                                GlobalsNAV.ShowDialog<IDrugInformation>(onInitDlg);
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
        //▲===== #018
        //▼====== #019
        public void PatientAppointments_Save(bool PassCheckFullTarget = true)
        {
            PatientAppointment CurrentAppointment = new PatientAppointment();
            CurrentAppointment.StaffID = Globals.LoggedUserAccount.Staff.StaffID;
            CurrentAppointment.DoctorStaffID = Globals.LoggedUserAccount.Staff.StaffID;
            CurrentAppointment.Patient = Registration_DataStorage.CurrentPatient;
            CurrentAppointment.AppointmentID = LatestePrecriptions.AppointmentID.GetValueOrDefault();
            CurrentAppointment.V_ApptStatus = AllLookupValues.ApptStatus.BOOKED;
            CurrentAppointment.NDay = LatestePrecriptions.NDay;
            CurrentAppointment.ApptDate = Globals.GetCurServerDateTime().AddDays(CurrentAppointment.NDay.Value);
            if (!Globals.ServerConfigSection.ConsultationElements.AllowWorkingOnSunday && CurrentAppointment.ApptDate.Value.DayOfWeek == DayOfWeek.Sunday)
            {
                CurrentAppointment.ApptDate = CurrentAppointment.ApptDate.Value.AddDays(1);
            }
            CurrentAppointment.PtRegistrationID = Registration_DataStorage.CurrentPatientRegistration.PtRegistrationID;
            string[] ListStr = Globals.ServerConfigSection.ConsultationElements.ParamAppointmentAuto.Split(new char[] { ';' });
            PatientApptServiceDetails item = new PatientApptServiceDetails
            {
                V_AppointmentType = Convert.ToInt64(ListStr[0]),
                MedServiceID = Convert.ToInt64(ListStr[1]),
                DeptLocationID = Convert.ToInt64(ListStr[2]),
                ApptTimeSegmentID = Convert.ToInt16(ListStr[3]),
                EntityState = EntityState.DETACHED
            };
            CurrentAppointment.ObjApptServiceDetailsList_Add = new ObservableCollection<PatientApptServiceDetails>();
            CurrentAppointment.ObjApptServiceDetailsList_Add.Add(item);
            this.DlgShowBusyIndicator(eHCMSResources.Z1016_G1_DangLuuCuocHen);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new AppointmentServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginPatientAppointments_Save(CurrentAppointment, PassCheckFullTarget, null, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                long AppointmentID = 0;
                                string ErrorDetail = "";

                                string ListNotConfig = "";
                                string ListTargetFull = "";
                                string ListMax = "";
                                string ListRequestID = "";

                                var b = contract.EndPatientAppointments_Save(out AppointmentID, out ErrorDetail, out ListNotConfig, out ListTargetFull, out ListMax, out ListRequestID, asyncResult);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                            }
                            finally
                            {
                                this.DlgHideBusyIndicator();
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    this.DlgHideBusyIndicator();
                }
            });
            t.Start();
        }
        //▲====== #019
        private bool _IsChildControl = false;

        public bool IsChildControl
        {
            get
            {
                return _IsChildControl;
            }
            set
            {
                if (_IsChildControl == value)
                {
                    return;
                }
                _IsChildControl = value;
                NotifyOfPropertyChange(() => IsChildControl);
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
                NotifyOfPropertyChange(() => IsInTreatmentProgram);
            }
        }
        private bool _IsEnableHIStore = Globals.ServerConfigSection.CommonItems.EnableHIStore;
        public bool IsEnableHIStore
        {
            get
            {
                return _IsEnableHIStore;
            }
            set
            {
                if (_IsEnableHIStore == value)
                {
                    return;
                }
                _IsEnableHIStore = value;
                NotifyOfPropertyChange(() => IsEnableHIStore);
            }
        }
        //▼===== #021
        private void GetAllHoliday()
        {
            if (Globals.allHoliday == null)
            {
                Globals.allHoliday = new List<Holiday>();
            }
            if (Globals.allHoliday.Count > 0)
            {
                return;
            }
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new AppointmentServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetAllHoliday(Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                List<Holiday> results = contract.EndGetAllHoliday(asyncResult);
                                if (results != null && results.Count > 0)
                                {
                                    Globals.allHoliday = results;
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
                }
                catch (Exception ex)
                {
                    ClientLoggerHelper.LogInfo(ex.ToString());
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }
        //▲===== #021

        //▼===== #023
        private Staff _Secretary;
        public Staff Secretary
        {
            get
            {
                return _Secretary;
            }
            set
            {
                if (_Secretary != value)
                {
                    _Secretary = value;
                    if (LatestePrecriptions != null)
                    {
                        LatestePrecriptions.SecretaryStaff = _Secretary;
                    }
                    NotifyOfPropertyChange(() => Secretary);
                }
            }
        }
        //▲===== #023
        //20191016 TBL: Khi click vào grid toa thuốc thì đi lấy phác đồ
        public void grdPrescription_GotFocus(object sender, RoutedEventArgs e)
        {
            //20191127 TBL: Nếu thấy ServiceRecID của chẩn đoán cũ trước đó thì không cho tạo mới dựa trên cũ toa thuốc để tránh bị lỗi
            if (CS_DS.DiagTreatment.ServiceRecID != ObjDiagnosisTreatment_Current.ServiceRecID)
            {
                MessageBox.Show(eHCMSResources.Z2925_G1_VuiLongNhapCD, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
            //20191127 
            GetPhacDo();
        }
        private bool _IsCheckApmtOnPrescription = Globals.ServerConfigSection.ConsultationElements.IsCheckApmtOnPrescription;
        public bool IsCheckApmtOnPrescription
        {
            get
            {
                return _IsCheckApmtOnPrescription;
            }
            set
            {
                if (_IsCheckApmtOnPrescription == value)
                {
                    return;
                }
                _IsCheckApmtOnPrescription = value;
                NotifyOfPropertyChange(() => IsCheckApmtOnPrescription);
            }
        }
        public void NotifyViewDataChanged()
        {
            NotifyOfPropertyChange(() => Registration_DataStorage);
            NotifyOfPropertyChange(() => IsInTreatmentProgram);
        }
        public bool IsInTreatmentProgram
        {
            get
            {
                return Registration_DataStorage != null && Registration_DataStorage.CurrentPatientRegistration != null && Registration_DataStorage.CurrentPatientRegistration.OutPtTreatmentProgramID.HasValue && Registration_DataStorage.CurrentPatientRegistration.OutPtTreatmentProgramID > 0;
            }
        }

        private void CallLoadAppoinmentDialogForTMV(bool IsInTreatmentProgram = false)
        {
            if (Globals.IsLockRegistration(Registration_DataStorage.CurrentPatientRegistration.RegLockFlag, eHCMSResources.T1455_G1_HBenh.ToLower()))
            {
                return;
            }
            this.ShowBusyIndicator();
            var CurrentThread = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new AppointmentServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetAppointmentByID(0, LatestePrecriptions.IssueID, Globals.DispatchCallback(asyncResult =>
                        {
                            try
                            {
                                var appt = contract.EndGetAppointmentByID(asyncResult);
                                PatientAppointment CurrentAppointment = new PatientAppointment();
                                if (appt != null)
                                {
                                    CurrentAppointment = appt;
                                }
                                else
                                {
                                    CurrentAppointment.DoctorStaffID = Globals.LoggedUserAccount.StaffID;
                                    CurrentAppointment.DoctorStaff = Globals.LoggedUserAccount.Staff;
                                }
                                CurrentAppointment.ApptDate = appt != null && appt.ApptDate.HasValue && appt.ApptDate != null ? appt.ApptDate.Value : LatestePrecriptions.IssuedDateTime.GetValueOrDefault(Globals.GetCurServerDateTime()).AddDays((int)LatestePrecriptions.NDay);
                                CurrentAppointment.NDay = LatestePrecriptions.NDay;
                                CurrentAppointment.Patient = Registration_DataStorage.CurrentPatient;
                                if (Registration_DataStorage.CurrentPatientRegistrationDetail != null)
                                {
                                    CurrentAppointment.ServiceRecID = Registration_DataStorage.CurrentPatientRegistrationDetail.ServiceRecID;
                                }
                                if (CurrentAppointment.ServiceRecID.GetValueOrDefault(0) == 0 &&
                                    Registration_DataStorage.CurrentPatientRegistrationDetail != null && Registration_DataStorage.PatientServiceRecordCollection != null &&
                                    Registration_DataStorage.PatientServiceRecordCollection.Any(x => x.PtRegDetailID == Registration_DataStorage.CurrentPatientRegistrationDetail.PtRegDetailID))
                                {
                                    CurrentAppointment.ServiceRecID = Registration_DataStorage.PatientServiceRecordCollection.First(x => x.PtRegDetailID == Registration_DataStorage.CurrentPatientRegistrationDetail.PtRegDetailID).ServiceRecID;
                                }
                                Action<IAddEditAppointmentTMV> onInitDlg = delegate (IAddEditAppointmentTMV apptVm)
                                {
                                    apptVm.IsInTreatmentProgram = IsInTreatmentProgram;
                                    apptVm.Registration_DataStorage = Registration_DataStorage;
                                    apptVm.IsCreateApptFromConsultation = true;
                                    apptVm.RegistrationID = Registration_DataStorage.CurrentPatientRegistration.PtRegistrationID;
                                    if (CurrentAppointment.AppointmentID > 0)
                                    {
                                        apptVm.SetCurrentAppointment(CurrentAppointment);
                                    }
                                    else
                                    {
                                        apptVm.SetCurrentAppointment(CurrentAppointment, LatestePrecriptions.IssueID);
                                    }
                                };
                                GlobalsNAV.ShowDialog<IAddEditAppointmentTMV>(onInitDlg);
                            }
                            catch (Exception ex)
                            {
                                Globals.ShowMessage(ex.Message, eHCMSResources.G0442_G1_TBao);
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
                    Globals.ShowMessage(ex.Message, eHCMSResources.G0442_G1_TBao);
                    this.HideBusyIndicator();
                }
            });
            CurrentThread.Start();
        }
        //▼===== #030
        private void GetTotalHIPaymentForRegistration(long PtRegistrationID)
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                using (var serviceFactory = new ePrescriptionsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetTotalHIPaymentForRegistration(PtRegistrationID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            TotalHIPaymentForRegistration = contract.EndGetTotalHIPaymentForRegistration(asyncResult);
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
        //▲===== #030
        private bool _IsEnableAddPrescription;
        public bool IsEnableAddPrescription
        {
            get
            {
                return _IsEnableAddPrescription;
            }
            set
            {
                if (_IsEnableAddPrescription == value)
                {
                    return;
                }
                _IsEnableAddPrescription = value;
                NotifyOfPropertyChange(() => IsEnableAddPrescription);
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
        public void Handle(CheckV_TreatmentType message)
        {
            if (V_TreatmentTypeArray.Length == 0)
            {
                return;
            }
            else
            {
                IsEnableAddPrescription = message.IsEnableAddPrescription;
                //if (IsEnableAddPrescription)
                //{
                //    btnCreateNewIsEnabled = true;
                //    btnCreateAndCopyIsEnabled = true;
                //}
                //else
                //{
                //    btnCreateNewIsEnabled = false;
                //    btnCreateAndCopyIsEnabled = false;
                //}
            }
        }
        private bool _IsHavePCLRequestUnfinished = false;
        public bool IsHavePCLRequestUnfinished
        {
            get
            {
                return _IsHavePCLRequestUnfinished;
            }
            set
            {
                if (_IsHavePCLRequestUnfinished == value)
                {
                    return;
                }
                _IsHavePCLRequestUnfinished = value;
                NotifyOfPropertyChange(() => IsHavePCLRequestUnfinished);
            }
        }

        public void ProcessCheckStatusPCLRequest(bool IsCreateNew)
        {
            //▼====: #037
            if (!Globals.CheckDoctorContactPatientTime())
            {
                return;
            }
            else
            {
                GlobalsNAV.AddUpdateDoctorContactPatientTimeAction(Registration_DataStorage.CurrentPatient.PatientID
                    , Registration_DataStorage.CurrentPatientRegistration.PtRegistrationID
                    , Registration_DataStorage.CurrentPatientRegistrationDetail.PtRegDetailID
                    , Globals.StartDatetimeExam.Value
                    , "Toa"
                    , Globals.LoggedUserAccount.Staff.StaffID);
            }
            //▲====: #037
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ePrescriptionsServiceClient())
                    {
                        bool bContinue = true;
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginCheckStatusPCLRequestBeforeCreateNew(PtRegistrationID, false, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                IsHavePCLRequestUnfinished = contract.EndCheckStatusPCLRequestBeforeCreateNew(asyncResult);
                                bContinue = false;
                                if (IsCreateNew)
                                {
                                    CreateNewFunc();
                                }
                                else
                                {
                                    CreateNewAndCopyFunc();
                                }
                            }
                            catch (Exception ex)
                            {
                                bContinue = false;
                                MessageBox.Show(ex.Message);
                            }
                            finally
                            {
                                if (!bContinue)
                                {
                                    this.HideBusyIndicator();
                                }

                            }
                        }), null);
                    }

                }
                catch (Exception ex)
                {
                    ClientLoggerHelper.LogError(ex.Message);
                    this.HideBusyIndicator();
                }

            });
            t.Start();
        }

        private List<Prescription> _Prescriptions_BaoHiemConThuoc;
        public List<Prescription> Prescriptions_BaoHiemConThuoc
        {
            get
            {
                return _Prescriptions_BaoHiemConThuoc;
            }
            set
            {
                if (_Prescriptions_BaoHiemConThuoc != value)
                {
                    _Prescriptions_BaoHiemConThuoc = value;
                    NotifyOfPropertyChange(() => Prescriptions_BaoHiemConThuoc);
                }
            }
        }
        private List<IList<PrescriptionDetail>> _ListPrescriptionDetailBaoHiemConThuocCungChuyenKhoa;
        public List<IList<PrescriptionDetail>> ListPrescriptionDetailBaoHiemConThuocCungChuyenKhoa
        {
            get
            {
                return _ListPrescriptionDetailBaoHiemConThuocCungChuyenKhoa;
            }
            set
            {
                if (_ListPrescriptionDetailBaoHiemConThuocCungChuyenKhoa != value)
                {
                    _ListPrescriptionDetailBaoHiemConThuocCungChuyenKhoa = value;
                    NotifyOfPropertyChange(() => ListPrescriptionDetailBaoHiemConThuocCungChuyenKhoa);
                }
            }
        }
        private List<IList<PrescriptionDetail>> _ListPrescriptionDetailBaoHiemConThuocKhacChuyenKhoa;
        public List<IList<PrescriptionDetail>> ListPrescriptionDetailBaoHiemConThuocKhacChuyenKhoa
        {
            get
            {
                return _ListPrescriptionDetailBaoHiemConThuocKhacChuyenKhoa;
            }
            set
            {
                if (_ListPrescriptionDetailBaoHiemConThuocKhacChuyenKhoa != value)
                {
                    _ListPrescriptionDetailBaoHiemConThuocKhacChuyenKhoa = value;
                    NotifyOfPropertyChange(() => ListPrescriptionDetailBaoHiemConThuocKhacChuyenKhoa);
                }
            }
        }
        private void GetAllPrescription_BaoHiemConThuoc()
        {
            //20190427 TBL: Moi lan thay doi benh nhan thi clear danh sach chi tiet toa thuoc trong ngay
            ListPrescriptionDetailBaoHiemConThuocCungChuyenKhoa = new List<IList<PrescriptionDetail>>();
            GetAllPrescription_BaoHiemConThuoc_ByPatientID(Registration_DataStorage.CurrentPatient.PatientID
                , Registration_DataStorage.CurrentPatientRegistrationDetail.PtRegDetailID
                , true);
            ListPrescriptionDetailBaoHiemConThuocKhacChuyenKhoa = new List<IList<PrescriptionDetail>>();
            GetAllPrescription_BaoHiemConThuoc_ByPatientID(Registration_DataStorage.CurrentPatient.PatientID
                , Registration_DataStorage.CurrentPatientRegistrationDetail.PtRegDetailID
                , false);
        }
        private void GetAllPrescription_BaoHiemConThuoc_ByPatientID(long PatientID, long PtRegDetailID, bool CungChuyenKhoa)
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                using (var serviceFactory = new ePrescriptionsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginPrescriptions_BaoHiemConThuoc_ByPatientID(PatientID, PtRegDetailID, CungChuyenKhoa, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            if (Prescriptions_BaoHiemConThuoc == null)
                            {
                                Prescriptions_BaoHiemConThuoc = new List<Prescription>();
                            } 
                            List<Prescription> lstprescriptions = contract.EndPrescriptions_BaoHiemConThuoc_ByPatientID(asyncResult);
                            if (lstprescriptions != null && lstprescriptions.Count > 0)
                            {
                                foreach (var prescription in lstprescriptions)
                                {
                                    GetPrescriptionDetailsByPrescriptID_BaoHiemConThuoc(prescription.PrescriptID, CungChuyenKhoa);
                                    Prescriptions_BaoHiemConThuoc.Add(prescription);
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
        public void GetPrescriptionDetailsByPrescriptID_BaoHiemConThuoc(long prescriptID, bool CungChuyenKhoa)
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                using (var serviceFactory = new ePrescriptionsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetPrescriptionDetailsByPrescriptID(prescriptID, false, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var Results = contract.EndGetPrescriptionDetailsByPrescriptID(asyncResult);
                            //Prescriptions_BaoHiemConThuoc = new Prescription();
                            //Prescriptions_BaoHiemConThuoc.PrescriptionDetails = Results.ToObservableCollection();
                            if (CungChuyenKhoa)
                            {
                                if (ListPrescriptionDetailBaoHiemConThuocCungChuyenKhoa == null)
                                {
                                    ListPrescriptionDetailBaoHiemConThuocCungChuyenKhoa = new List<IList<PrescriptionDetail>>();
                                }
                                ListPrescriptionDetailBaoHiemConThuocCungChuyenKhoa.Add(Results);
                            }
                            else
                            {
                                if (ListPrescriptionDetailBaoHiemConThuocKhacChuyenKhoa == null)
                                {
                                    ListPrescriptionDetailBaoHiemConThuocKhacChuyenKhoa = new List<IList<PrescriptionDetail>>();
                                }
                                ListPrescriptionDetailBaoHiemConThuocKhacChuyenKhoa.Add(Results);
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
        private string CheckThuocBiTrungHoatChatBaoHiemConThuoc(bool CungChuyenKhoa)
        {
            //TBL: Neu bat cau hinh thi moi kiem tra hoat chat
            if (!Globals.ServerConfigSection.ConsultationElements.CheckToaThuocBiTrungTheoHoatChatVaNgayThuocBaoHiem || bBlock)
            {
                return "";
            }
            StringBuilder sb = new StringBuilder();
            string err = "";
            if (LatestePrecriptions.PrescriptionDetails.Count > 1)
            {
                if (CheckToaThuocBiTrungBaoHiemConThuoc_TheoHoatChat(CungChuyenKhoa ? ListPrescriptionDetailBaoHiemConThuocCungChuyenKhoa : 
                    ListPrescriptionDetailBaoHiemConThuocKhacChuyenKhoa, LatestePrecriptions.PrescriptionDetails, CungChuyenKhoa, out err))
                {
                    if (!CungChuyenKhoa)
                    {
                        bBlock = true;
                    }
                    return string.Format("Để tránh xuất toán BHYT, vui lòng không cho trùng hoạt chất do bệnh nhân chưa hết thuốc:" + Environment.NewLine + err + Environment.NewLine
                        + "Nếu cần thiết cho, nhấn đồng ý lưu, vui lòng in thêm phiếu thu tiền thừa thuốc, hướng dẫn BN trước khi lãnh thuốc," + Environment.NewLine 
                        + " gặp kế toán thu đóng tiền thừa thuốc. Trân trọng!");
                }

            }
            return "";
        }

        private bool CheckToaThuocBiTrungBaoHiemConThuoc_TheoHoatChat(List<IList<PrescriptionDetail>> ListPrescriptionDetailBaoHiemConThuoc, IList<PrescriptionDetail> pNeedSave, bool CungChuyenKhoa, out string msg)
        {
            StringBuilder sb = new StringBuilder();
            msg = "";
            if (pNeedSave != null && pNeedSave.Count > 0)
            {
                foreach (var prescriptDetaiil in pNeedSave)
                {
                    if (!CheckThuocBiTrungBaoHiemConThuoc_TheoHoatChat(ListPrescriptionDetailBaoHiemConThuoc, prescriptDetaiil, out string ChangePrice) && prescriptDetaiil.SelectedDrugForPrescription != null && prescriptDetaiil.SelectedDrugForPrescription.GenericName != null)
                    {
                        if (!CungChuyenKhoa)
                        {
                            sb.AppendLine(string.Format("- " + prescriptDetaiil.SelectedDrugForPrescription.GenericName.Trim()));
                        }
                        else
                        {
                            sb.AppendLine(string.Format("+ Hoạt chất: " + prescriptDetaiil.SelectedDrugForPrescription.GenericName.Trim() + ". Số tiền thừa: "+ ChangePrice));
                        }
                    }
                }
                msg = sb.ToString();
            }
            if (!string.IsNullOrEmpty(msg))
            {
                return true;
            }
            return false;
        }
        private string GetPrescriptChangePrice(PrescriptionDetail prescriptDetaiil)
        {
            if (prescriptDetaiil== null)
            {
                return "";
            }
            double changeDay = prescriptDetaiil.RealDay - Math.Round((Globals.GetCurServerDateTime()
                - Prescriptions_BaoHiemConThuoc.Where(x => x.PrescriptID == prescriptDetaiil.PrescriptID).FirstOrDefault().RecDateCreated).TotalDays, 0);
            double amoutOfDay = ((float)prescriptDetaiil.MDose + (float)prescriptDetaiil.ADose + (float)prescriptDetaiil.EDose + (float)prescriptDetaiil.NDose);
            double changePrice = Convert.ToDouble(prescriptDetaiil.SelectedDrugForPrescription.HIAllowedPrice) * changeDay * amoutOfDay;
            return changePrice.ToString("#,#");
        }
        private bool CheckThuocBiTrungBaoHiemConThuoc_TheoHoatChat(List<IList<PrescriptionDetail>> ListPrescriptionDetailBaoHiemConThuoc, PrescriptionDetail pNeedSave, out string ChangePrice)
        {
            if (ListPrescriptionDetailBaoHiemConThuoc != null && ListPrescriptionDetailBaoHiemConThuoc.Count > 0)
            {
                foreach (IList<PrescriptionDetail> listdetail_i in ListPrescriptionDetailBaoHiemConThuoc)
                {
                    foreach (var item in listdetail_i)
                    {
                        if (item.PrescriptID != LatestePrecriptions.PrescriptID && item.DrugID != 0 && item.DrugID != null && item.SelectedDrugForPrescription != null && pNeedSave.SelectedDrugForPrescription != null && item.SelectedDrugForPrescription.GenericID == pNeedSave.SelectedDrugForPrescription.GenericID && item.BeOfHIMedicineList && pNeedSave.BeOfHIMedicineList)
                        {
                            ChangePrice = GetPrescriptChangePrice(item);
                            return false;
                        }
                    }
                }
            }
            ChangePrice = "";
            return true;
        }
        private void ReCheckHICheckBox()
        {
            foreach (var item in LatestePrecriptions.PrescriptionDetails)
            {
                if (item.SelectedDrugForPrescription.HIAllowedPrice > 0
                    || !string.IsNullOrEmpty(item.SelectedDrugForPrescription.HIDrugCode)
                    || (item.SelectedDrugForPrescription.DrugID == 0 && isHisID))
                {
                    item.BeOfHIMedicineList = true;
                }
                else
                {
                    item.BeOfHIMedicineList = false;
                }
            }
        }

        private int _MinimumPopulateDelay = Globals.ServerConfigSection.CommonItems.MinimumPopulateDelay;
        public int MinimumPopulateDelay
        {
            get
            {
                return _MinimumPopulateDelay;
            }
            set
            {
                if (_MinimumPopulateDelay == value)
                {
                    return;
                }
                _MinimumPopulateDelay = value;
                NotifyOfPropertyChange(() => MinimumPopulateDelay);
            }
        }
        private int? _RemainingMedicineDays;
        public int? RemainingMedicineDays
        {
            get
            {
                return _RemainingMedicineDays;
            }
            set
            {
                if (_RemainingMedicineDays == value)
                {
                    return;
                }
                _RemainingMedicineDays = value;
                NotifyOfPropertyChange(() => RemainingMedicineDays);
            }
        }
        private int _MaxNumOfDayMedicine;
        public int MaxNumOfDayMedicine
        {
            get
            {
                return _MaxNumOfDayMedicine;
            }
            set
            {
                if (_MaxNumOfDayMedicine == value)
                {
                    return;
                }
                _MaxNumOfDayMedicine = value;
                NotifyOfPropertyChange(() => MaxNumOfDayMedicine);
            }
        }
        private int _MinNumOfDayMedicine;
        public int MinNumOfDayMedicine
        {
            get
            {
                return _MinNumOfDayMedicine;
            }
            set
            {
                if (_MinNumOfDayMedicine == value)
                {
                    return;
                }
                _MinNumOfDayMedicine = value;
                NotifyOfPropertyChange(() => MinNumOfDayMedicine);
            }
        }
        private void GetRemainingMedicineDays()
        {
            RemainingMedicineDays = null;
            MinNumOfDayMedicine = 0;
            MaxNumOfDayMedicine = 0;

            if (Registration_DataStorage != null
                && Registration_DataStorage.CurrentPatientRegistration != null 
                && Registration_DataStorage.CurrentPatientRegistration.OutPtTreatmentProgramID != null
                && Registration_DataStorage.CurrentPatientRegistration.OutpatientTreatmentTypeID != null)
            {
                GetRemainingMedicineDays_ByOutPtTreatmentProgramID((long)Registration_DataStorage.CurrentPatientRegistration.OutPtTreatmentProgramID,
                    (long)Registration_DataStorage.CurrentPatientRegistrationDetail.PtRegDetailID);
            }
           
        }
        private void GetRemainingMedicineDays_ByOutPtTreatmentProgramID(long OutPtTreatmentProgramID, long PtRegDetailID)
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                using (var serviceFactory = new ePrescriptionsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetRemainingMedicineDays(OutPtTreatmentProgramID, PtRegDetailID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            int tempMinNumOfDayMedicine = 0;
                            int tempMaxNumOfDayMedicine = 0;
                            RemainingMedicineDays = contract.EndGetRemainingMedicineDays(out tempMinNumOfDayMedicine, out tempMaxNumOfDayMedicine, asyncResult);
                            MinNumOfDayMedicine = tempMinNumOfDayMedicine;
                            MaxNumOfDayMedicine = tempMaxNumOfDayMedicine;
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
        public void Handle(ConfirmOutPtTreatmentProgram message)
        {
            if (message != null && message.Result == true)
            {
                GetRemainingMedicineDays();
            }
        }
        private bool CheckYHCT()
        {
            foreach (var detail in LatestePrecriptions.PrescriptionDetails)
            {
                if (detail.SelectedDrugForPrescription.DrugClassID == 122)
                {
                    return true;
                    
                }
            }
            return false;
        }

        //▼==== #040
        private int _NumOfDayMedicine;
        public int NumOfDayMedicine
        {
            get
            {
                return _NumOfDayMedicine;
            }
            set
            {
                if (_NumOfDayMedicine == value)
                {
                    return;
                }
                _NumOfDayMedicine = value;
                NotifyOfPropertyChange(() => NumOfDayMedicine);
            }
        }

        private bool CheckSoNgayThuoc()
        {
            //▼==== #042
            //20220917 BLQ:Fix lại điều kiện không lấy lớn hơn không vì tìm lại bệnh nhân số ngày thuốc là 0 và không set lại số ngày cấp toa
            if (_Registration_DataStorage != null && _Registration_DataStorage.CurrentPatientRegistration != null)
            {
                NumOfDayMedicine = _Registration_DataStorage.CurrentPatientRegistration.PrescriptionsAmount;
            }
            //▲==== #042
            if (LatestePrecriptions.NDay != null
                && CS_DS != null
                && CS_DS.DiagTreatment != null)
            {
                //▼====: #044
                if (NumOfDayMedicine != LatestePrecriptions.NDay && CS_DS.DiagTreatment.V_TreatmentType == (long)AllLookupValues.V_TreatmentType.OutPtTreatment)
                {
                    MessageBox.Show("Số ngày của toa thuốc hiện tại phải bằng số ngày ra toa của Đợt điều trị/liệu trình. Số ngày = " + NumOfDayMedicine + " ngày" + Environment.NewLine
                               + "Vui lòng liên hệ KHTH để biết thêm chi tiết.", eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                    return false;
                }
                else if(Globals.ServerConfigSection.CommonItems.MaxNumDayPrescriptAllow < LatestePrecriptions.NDay 
                    //&& CS_DS.DiagTreatment.V_TreatmentType == (long)AllLookupValues.V_TreatmentType.IssuedPrescription
                    && Registration_DataStorage != null
                    && Registration_DataStorage.CurrentPatientRegistration != null
                    && Registration_DataStorage.CurrentPatientRegistration.HisID.GetValueOrDefault(0) > 0
                    && Registration_DataStorage.CurrentPatientRegistration.OutPtTreatmentProgramID == null
                    && !LatestePrecriptions.PreNoDrug)
                {
                    MessageBox.Show("Số ngày của toa thuốc không được vượt quá " + Globals.ServerConfigSection.CommonItems.MaxNumDayPrescriptAllow + " ngày", eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                    return false;
                }
                //▲====: #044
            }
            else if (LatestePrecriptions.NDay <= 0)
            {
                if (!PreNoDrug)
                {
                    if (MessageBox.Show(eHCMSResources.Z3255_G1_ChNhapNgayThuoc, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK) == MessageBoxResult.OK)
                    {
                        CtrtbSoTuan.Focus();
                    }
                    return false;
                }
            }
            return true;
        }
        //▲==== #040
        //▼==== #041
        private bool CheckPatientInfoForOutPtTreatment(out string Message)
        {
            Message = "";
            if (Globals.ServerConfigSection.CommonItems.CheckPatientInfoWhenSavePrescript
                && Registration_DataStorage != null
                && Registration_DataStorage.CurrentPatientRegistration != null
                && Registration_DataStorage.CurrentPatientRegistration.OutPtTreatmentProgramID != null
                && Registration_DataStorage.CurrentPatientRegistration.OutpatientTreatmentTypeID != null
                && Registration_DataStorage.CurrentPatient != null
                && CS_DS != null
                && CS_DS.DiagTreatment != null
                && CS_DS.DiagTreatment.V_TreatmentType == (long)AllLookupValues.V_TreatmentType.OutPtTreatment)
            {
                if (Globals.ServerConfigSection.CommonItems.ApplyReport130)
                {
                    if (Registration_DataStorage.CurrentPatient.JobID130 <= 0)
                    {
                        Message += Environment.NewLine + " - Nghề nghiệp";
                    }
                }
                else
                {
                    if (Registration_DataStorage.CurrentPatient.V_Job <= 0)
                    {
                        Message += Environment.NewLine + " - Nghề nghiệp";
                    }
                }
               
                if (string.IsNullOrEmpty(Registration_DataStorage.CurrentPatient.EthnicName) || Registration_DataStorage.CurrentPatient.EthnicName.Contains("Chưa xác định"))
                {
                    Message += Environment.NewLine + " - Dân tộc";
                }
                if (Registration_DataStorage.CurrentPatient.V_FamilyRelationship <= 0)
                {
                    Message += Environment.NewLine + " - Liên hệ phụ - Gia đình";
                }
                if (string.IsNullOrEmpty(Registration_DataStorage.CurrentPatient.FContactFullName))
                {
                    Message += Environment.NewLine + " - Liên hệ phụ - Họ tên";
                }
                if (string.IsNullOrEmpty(Registration_DataStorage.CurrentPatient.FContactCellPhone))
                {
                    Message += Environment.NewLine + " - Liên hệ phụ - Số điện thoại";
                }
                if (string.IsNullOrEmpty(Message))
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
                return true;
            }
        }
       
        public void Handle(UpdateCompleted<Patient> message)
        {
            if (message != null && message.Item != null && Registration_DataStorage != null && Registration_DataStorage.CurrentPatient != null)
            {
                Registration_DataStorage.CurrentPatient.V_Job = message.Item.V_Job;
                Registration_DataStorage.CurrentPatient.JobID130 = message.Item.JobID130;
                Registration_DataStorage.CurrentPatient.EthnicName = message.Item.EthnicName;
                Registration_DataStorage.CurrentPatient.V_FamilyRelationship = message.Item.V_FamilyRelationship;
                Registration_DataStorage.CurrentPatient.FContactFullName = message.Item.FContactFullName;
                Registration_DataStorage.CurrentPatient.FContactCellPhone = message.Item.FContactCellPhone;
                Registration_DataStorage.CurrentPatient.Age = message.Item.Age;
                Registration_DataStorage.CurrentPatient.MonthsOld = message.Item.MonthsOld;
                Registration_DataStorage.CurrentPatient.IDNumber = message.Item.IDNumber;
                Registration_DataStorage.CurrentPatient.PatientCellPhoneNumber = message.Item.PatientCellPhoneNumber;
            }
        }
        //▲==== #041
        //▼==== #043
        public void Handle(SaveOutPtTreatmentProgramItem message)
        {
            if (message != null && message.PrescriptionsAmount > 0 && Registration_DataStorage != null && Registration_DataStorage.CurrentPatientRegistration != null)
            {
                Registration_DataStorage.CurrentPatientRegistration.PrescriptionsAmount = message.PrescriptionsAmount;
            }
        }
        //▲==== #043
        //▼==== #045
        public bool CheckValidPrescriptionWithDiagnosis()
        {
            //20221110 BLQ: Fix trường hợp bệnh nhân mới và chưa có toa thuốc thì không kiểm tra 
            if (mNewPrescriptionState && btnCreateNewIsEnabled)// && btnCreateAndCopyIsEnabled)
            {
                return true;
            }
            if (!CheckChongChiDinhForDiagnosis())
            {
                return false;
            }
            //▼==== #040
            if (!CheckSoNgayThuoc())
            {
                return false;
            }
            //▲==== #040
            return true;
        }
        //▲==== #045
        //▼==== #049
        private bool CheckPatientInfoForPatient72Month(out string Message)
        {
            Message = "";
            if (Registration_DataStorage != null
                && Registration_DataStorage.CurrentPatientRegistration != null
                && Registration_DataStorage.CurrentPatient != null
                && Registration_DataStorage.CurrentPatient.MonthsOld > 0
                && Registration_DataStorage.CurrentPatient.MonthsOld < 72)
            {
                if (Registration_DataStorage.CurrentPatient.V_FamilyRelationship <= 0)
                {
                    Message += Environment.NewLine + " - Liên hệ phụ - Gia đình";
                }
                if (string.IsNullOrEmpty(Registration_DataStorage.CurrentPatient.FContactFullName))
                {
                    Message += Environment.NewLine + " - Liên hệ phụ - Họ tên";
                }
                if (string.IsNullOrEmpty(Registration_DataStorage.CurrentPatient.FContactCellPhone))
                {
                    Message += Environment.NewLine + " - Liên hệ phụ - Số điện thoại";
                }
                if (string.IsNullOrEmpty(Message))
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
                return true;
            }
        }
        //▲==== #049
        //▼==== #050
        private bool CheckPatientInfoForToaGayNghien(out string Message)
        {
            Message = "";
            if (!LatestePrecriptions.PrescriptionDetails.Any(x=> x.RefGenericDrugDetail!= null && (x.RefGenericDrugDetail.RefPharmacyDrugCatID == 2 || x.RefGenericDrugDetail.RefPharmacyDrugCatID == 4)))
            {
                return true;
            }
            if (Registration_DataStorage != null
                && Registration_DataStorage.CurrentPatientRegistration != null
                && Registration_DataStorage.CurrentPatient != null)
            {
                if (string.IsNullOrEmpty(Registration_DataStorage.CurrentPatient.PatientCellPhoneNumber))
                {
                    Message +=  "số điện thoại người bệnh/ người nhận thuốc";
                }
                if (string.IsNullOrEmpty(Registration_DataStorage.CurrentPatient.IDNumber))
                {
                    Message += (string.IsNullOrEmpty(Message) ? "": " và ") + "CCCD, ngày cấp, nơi cấp người bệnh/ người nhận thuốc";
                }
               
                if (string.IsNullOrEmpty(Message))
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
                return true;
            }
        }
        //▲==== #050
        //▼==== #051
        public void Handle(PCLExamAccordingICD_Event message)
        {
            if (message != null && message.ListPCLExamAccordingICD != null)
            {
                if(btnUndoIsEnabled)
                {
                    btnUndo(null, null);
                }
            }
        }
        //▲==== #051
    }
}
