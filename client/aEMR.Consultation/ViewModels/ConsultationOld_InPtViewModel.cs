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
using aEMR.Common;
using System.Linq;
using aEMR.Controls;
using System.ComponentModel;
using eHCMSLanguage;
using aEMR.ServiceClient.Consultation_PCLs;
using Castle.Windsor;
using System.Windows.Media;
using System.Windows.Input;
using static aEMR.Infrastructure.Events.TransferFormEvent;
using aEMR.ViewContracts.Consultation_ePrescription;
using DevExpress.Data.Extensions;
/*
* 20161224 #001 CMN:    Check department for dept change only
* 20171002 #002 CMN:    Added button for save Catastrophe Inf.
* 20171124 #003 CMN:    Added Daily diagnostic
* 20180913 #004 TTM:    Fixed cannot show information on dialog paperreferal full when user click on hyperlink  
* 20190731 #005 TTM:    BM 0013035: Fix lỗi AutoComplete ICD lỗi cho chẩn đoán nội trú.
* 20190802 #006 TTM:    BM 0013054: Bổ sung thêm tick tự động ICD9 chính và chuyển combo chọn khoa về chọn theo khoa bệnh nhân đã từng nhập khoa không lấy tất cả.
* 20191025 #007 TTM:    BM 0018490: Fix lỗi nếu khoa ban đầu bác sĩ chọn là 1 trong các khoa nội trú thì không lưu đề nghị đc. Lý do: Được mặc định giá trị nhưng không gán vào DiagTrmtItem kiểm tra bị thiếu, ẩn ICD 9 nếu là đề nghị nhập viện.
* 20191111 #008 TBL:    BM 0018511: Cho người dùng chọn BS chẩn đoán để lưu cho chẩn đoán
* 20191121 #009 TTM:    BM 0019594: Fix lỗi không thêm được dị ứng, tình trạng thể chất
* 20191207 #010 TTM:    BM 0019704: [Đề nghị nhập viện] Không hạn chế khoa đề nghị của bác sĩ để bác sĩ khoa Nội được phép đề nghị vào khoa Nhi mặc dù không cấu hình trách nhiệm.
* 20200415 #011 TBL:    BM 0030109: Kiểm tra danh sách ICD10 có tồn tại ICD10 nào ngưng sử dụng khi bấm tạo mới dựa trên cũ
* 20201211 #012 TNHX:   BM: Lọc danh sách bsi theo khoa phòng cấu hình (FilterDoctorByDeptResponsibilitiesInPt)
* 20210422 #013 TNHX:   Chặn ICD không làm bệnh chính + Thêm chẩn đoán khác
* 20210427 #014 TNHX:   Chọn ICD sao kèm theo nếu ICD chính có dấu găm
* 20210611 #015 TNHX:   346 Chọn ICD theo quy tắc
* 20211004 #016 TNHX: Lọc danh sách bsi theo cấu hình trách nhiệm + theo tích tạm khóa
* 20230109 #017 DatTB: Thêm phiếu tự khai và cam kết điều trị
* 20230403 #018 QTD:   Thêm mã máy
* 20230603 #019 DatTB: Upgrade Devexpress: Cập nhật thêm thư viện using DevExpress.Data.Extensions;
*/
namespace aEMR.ConsultantEPrescription.ViewModels
{
    [Export(typeof(IConsultationOld_InPt)), PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class ConsultationOld_InPtViewModel : ViewModelBase, IConsultationOld_InPt
        , IHandle<ConsultationDoubleClickEvent_InPt_2>
        , IHandle<GlobalCurPatientServiceRecordLoadComplete_Consult_InPt>
        , IHandle<CommonClosedPhysicalForDiagnosis_InPtEvent>
        , IHandle<OnChangedPaperReferal>
        , IHandle<SelectedRequireSubICDForDiagnosisTreatment>
        , IHandle<SelectedRuleICDForDiagnosisTreatment>
        , IHandle<DiseaseProgression_Event>
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
        #endregion

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
        public ConsultationOld_InPtViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg)
        {
            //Globals.EventAggregator.Subscribe(this);

            authorization();

            LoadInitData();

            GetAllLookupValuesByType();

            //▼===== #010:  Chuyển code xuống OnActivate để làm công việc tương đương thay vì đặt trên hàm khởi tạo. 
            //              Do khi get view từ Globals Nav hàm khởi tạo chạy trước các giá trị được set vào cho view => các giá trị set vào sẽ không đúng giá trị cần thiết
            DepartmentContent = Globals.GetViewModel<IDepartmentListing>();
            DepartmentContent.AddSelectOneItem = true;
            DepartmentContent.LoadData();
            (DepartmentContent as PropertyChangedBase).PropertyChanged += new PropertyChangedEventHandler(DepartmentContent_PropertyChanged);
            //▲===== #010

            //KMx: Sau khi view cha (Consultations_InPtViewModel) set IsDiagnosisOutHospital xong, sau đó mới gọi hàm InitPatientInfo(), vì trong hàm có sử dụng IsDiagnosisOutHospital (10/06/2015 09:33).
            //InitPatientInfo();
            LoadDoctorStaffCollection();
            if (gSelectedDoctorStaff == null)
            {
                gSelectedDoctorStaff = new Staff();
            }
            //▼====: #013
            if (Globals.ServerConfigSection.CommonItems.ApplyOtherDiagnosis)
            {
                VisibilityOtherDiagnosis = Visibility.Visible;
            }
            else
            {
                VisibilityOtherDiagnosis = Visibility.Collapsed;
            }
            //▲====: #013


        }
        //▼===== #010
        protected override void OnActivate()
        {
            base.OnActivate();
            Globals.EventAggregator.Subscribe(this);
            //▼===== 20190105 TTM: Nếu là đề nghị nhập viện thì không hạn chế thông tin các khoa được load.
            if (IsAdmRequest)
            {
                DepartmentContent.IsAdmRequest = IsAdmRequest;
                DepartmentContent.LoadData();
            }

        }
        //▲===== #010
        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);
            Globals.EventAggregator.Unsubscribe(this);
        }
        private bool _IsDiagnosisOutHospital;
        public bool IsDiagnosisOutHospital
        {
            get { return _IsDiagnosisOutHospital; }
            set
            {
                if (_IsDiagnosisOutHospital != value)
                {
                    _IsDiagnosisOutHospital = value;
                    NotifyOfPropertyChange(() => IsDiagnosisOutHospital);
                }
            }
        }
        /*▼====: #003*/
        private bool _IsShowCheckCOVID;
        public bool IsShowCheckCOVID
        {
            get { return _IsShowCheckCOVID; }
            set
            {
                if (_IsShowCheckCOVID != value)
                {
                    _IsShowCheckCOVID = value;
                    NotifyOfPropertyChange(() => IsShowCheckCOVID);
                }
            }
        }
        /*▲====: #003*/
        /*▼====: #003*/
        private bool _IsDailyDiagnosis;
        public bool IsDailyDiagnosis
        {
            get { return _IsDailyDiagnosis; }
            set
            {
                if (_IsDailyDiagnosis != value)
                {
                    _IsDailyDiagnosis = value;
                    NotifyOfPropertyChange(() => IsDailyDiagnosis);
                }
            }
        }
        /*▲====: #003*/

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

        private void DepartmentContent_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (DiagTrmtItem == null)
            {
                return;
            }

            if (e.PropertyName == "SelectedItem")
            {
                //▼===== 20191012 TTM:  Bổ sung điều kiện để by pass kiểm tra khác khoa khi đề nghị nhập viện. Do không còn loại mà 
                //                      nên khi đề nghị nhập viện bị báo chẩn đoán hàng ngày không thể đổi khoa.
                //                      
                if (!IsAdmRequest)
                {
                    if (DiagTrmtItem.DTItemID > 0
                        && DiagTrmtItem.Department != null
                        && DepartmentContent.SelectedItem != null
                        && DiagTrmtItem.Department.DeptID != DepartmentContent.SelectedItem.DeptID
                        && !IsDiagnosisOutHospital)
                    {
                        /*▼====: #003*/
                        if (IsDailyDiagnosis)
                        {
                            MessageBox.Show(eHCMSResources.Z2159_G1_Msg_HChinhCDoanHangNgay);
                        }
                        else
                        {
                            MessageBox.Show(eHCMSResources.Z0401_G1_HChinhCDoanXK);
                        }
                        /*▲====: #003*/
                        DepartmentContent.SelectedItem = DiagTrmtItem.Department;
                        return;
                    }
                }
                if (IsAdmRequest && DepartmentContent.SelectedItem.IsTreatmentForCOVID)
                {
                    IsShowCheckCOVID = true;
                    DiagTrmtItem.IsTreatmentCOVID = true;
                } else
                {
                    IsShowCheckCOVID = false;
                    DiagTrmtItem.IsTreatmentCOVID = false;
                }
                DiagTrmtItem.Department = DepartmentContent.SelectedItem;
                //▲===== 20191012
            }
        }

        public void Handle(GlobalCurPatientServiceRecordLoadComplete_Consult_InPt message)
        {
            if (Registration_DataStorage == null || Registration_DataStorage.CurrentPatientRegistration == null)
            {
                MessageBox.Show(eHCMSResources.Z0412_G1_DLieuKBChuaDuocCBi, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
            InitPatientInfo();
        }

        public IEnumerator<IResult> MessageWarningShowDialogTask(string strMessage)
        {
            var dialog = new MessageWarningShowDialogTask(strMessage, eHCMSResources.K1576_G1_CBao, false);
            yield return dialog;
            yield break;
        }

        //KMx: Nội trú không cần kiểm tra 7 ngày (17/12/2014 17:00).
        //private bool ValidateExpiredDiagnosicTreatment(DiagnosisTreatment diagnosicTreatment)
        //{
        //    DateTime curDate = Globals.GetCurServerDateTime();
        //    if (diagnosicTreatment.DiagnosisDate.AddDays(7) < curDate)
        //    {
        //        btEditIsEnabled = false;
        //        Coroutine.BeginExecute(MessageWarningShowDialogTask("Chẩn Đoán Này Đã Hết Hiệu Lực Cho Chỉnh Sửa (>7 Ngày)!"));
        //        return false;
        //    }
        //    return true;
        //}

        //public void CheckBeforeConsult()
        //{
        //    //Kiem tra trang thai cua dang ky nay
        //    if (Registration_DataStorage.PatientServiceRecordCollection != null
        //        && Registration_DataStorage.PatientServiceRecordCollection.Count > 0
        //        && Registration_DataStorage.PatientServiceRecordCollection[0].DiagnosisTreatments != null
        //        && Registration_DataStorage.PatientServiceRecordCollection[0].DiagnosisTreatments.Count > 0)
        //    {


        //        //KMx: Nếu có chẩn đoán nhập viện và xuất viện rồi thì không hiện nút tạo mới nữa (06/11/2014 15:15).
        //        //if (Registration_DataStorage.PatientServiceRecordCollection.Any(x => x.V_DiagnosisType == (long)AllLookupValues.V_DiagnosisType.DIAGNOSIS_IN) && Registration_DataStorage.PatientServiceRecordCollection.Any(x => x.V_DiagnosisType == (long)AllLookupValues.V_DiagnosisType.DIAGNOSIS_OUTHOS))
        //        //KMx: Nếu là màn hình chẩn đoán nhập/xuất viện và BN đã có chẩn đoán nhập viện và xuất viện rồi thì không hiện nút tạo mới (10/06/2015 09:13).
        //        if (IsDiagnosisOutHospital
        //            && Registration_DataStorage.PatientServiceRecordCollection[0].DiagnosisTreatments.Any(x => x.V_DiagnosisType == (long)AllLookupValues.V_DiagnosisType.DIAGNOSIS_IN)
        //            && Registration_DataStorage.PatientServiceRecordCollection[0].DiagnosisTreatments.Any(x => x.V_DiagnosisType == (long)AllLookupValues.V_DiagnosisType.DIAGNOSIS_OUTHOS))

        //        {
        //            ConsultState = ConsultationState.EditConsultationState;
        //            StateEdit();
        //        }
        //        else
        //        {
        //            ConsultState = ConsultationState.NewAndEditConsultationState;
        //            StateNewEdit();
        //        }

        //        DiagTrmtItem = ObjectCopier.DeepCopy(Registration_DataStorage.PatientServiceRecordCollection[0].DiagnosisTreatments[0]);
        //        DiagTrmtItemCopy = ObjectCopier.DeepCopy(DiagTrmtItem);

        //        //DiagnosisIcd10Items_Load_InPt(DiagTrmtItem.ServiceRecID.GetValueOrDefault());
        //        DiagnosisIcd10Items_Load_InPt(DiagTrmtItem.DTItemID);

        //        //KMx: Nội trú không cần kiểm tra 7 ngày (17/12/2014 17:00).
        //        //ValidateExpiredDiagnosicTreatment(DiagTrmtItem);
        //        return;
        //    }
        //    else
        //    {

        //        if (Registration_DataStorage.CurrentPatientRegistration == null)
        //        {
        //            return;
        //        }

        //        ConsultState = ConsultationState.NewAndEditConsultationState;
        //        StateNewEdit();
        //        GetLatesDiagTrmtByPtID_InPt(Registration_DataStorage.CurrentPatient.PatientID);

        //    }
        //}

        //▼====== #006
        public void setDepartmentForPatient()
        {
            DepartmentContent.Departments = new ObservableCollection<RefDepartment>();
            foreach (var item in Registration_DataStorage.CurrentPatientRegistration.AdmissionInfo.InPatientDeptDetails)
            {
                if (item.ToDate == null || item.V_InPatientDeptStatus == AllLookupValues.InPatientDeptStatus.NHAP_KHOA_PHONG)
                {
                    DepartmentContent.Departments.Add(item.DeptLocation.RefDepartment);
                }
            }
        }
        //▲====== #006
        public void CheckBeforeConsult()
        {
            refICD9ListCopy = new ObservableCollection<DiagnosisICD9Items>();
            refIDC10ListCopy = new ObservableCollection<DiagnosisIcd10Items>();
            if (Registration_DataStorage.CurrentPatientRegistration == null)
            {
                return;
            }
            //▼====== #006: Chỉ hiển thị danh sách khoa bệnh nhân đã từng nằm không hiển thị toàn bộ theo yêu cầu của anh Tuân
            if (Registration_DataStorage.CurrentPatientRegistration.AdmissionInfo != null)
            {
                setDepartmentForPatient();
            }
            //▲====== #006
            if (IsProcedureEdit) //20200410 TBL: Nếu là màn hình thủ thuật thì mặc định ConsultState là NewConsultationState
            {
                ConsultState = ConsultationState.NewConsultationState;
            }
            else
            {
                ConsultState = ConsultationState.NewAndEditConsultationState;
            }
            StateNewEdit();

            if (this.IsForCollectDiagnosis)
            {
                if (DiagTrmtItem == null) DiagTrmtItem = new DiagnosisTreatment();
                //▼===== 20190928 TTM:  Do chẩn đoán đã thay đổi không còn loại nữa nên tất cả đưa về chẩn đoán hàng ngày.
                //                      Và do đã sửa đề nghị nhập viện thành loại hàng ngày nên cần sửa load đề nghị thành loại hàng ngày.
                //DiagTrmtItem.V_DiagnosisType = (long)AllLookupValues.V_DiagnosisType.DIAGNOSIS_IN;
                DiagTrmtItem.V_DiagnosisType = (long)AllLookupValues.V_DiagnosisType.DIAGNOSIS_DAILY;
                //▲===== 
                if (DiagTrmtItem.PatientServiceRecord == null)
                {
                    DiagTrmtItem.PatientServiceRecord = new PatientServiceRecord();
                    DiagTrmtItem.PatientServiceRecord.PatientMedicalRecord = new PatientMedicalRecord();
                }
                DiagTrmtItem.PatientServiceRecord.V_Behaving = (long)AllLookupValues.Behaving.YEU_CAU_NHAP_VIEN;
                if (InPtRegistrationID > 0)
                {
                    //▼===== 20190928 TTM:  Do chẩn đoán đã thay đổi không còn loại nữa nên tất cả đưa về chẩn đoán hàng ngày.
                    //                      Và do đã sửa đề nghị nhập viện thành loại hàng ngày nên cần sửa load đề nghị thành loại hàng ngày.
                    //GetLatesDiagTrmtByPtID_InPt_OnlyForDia(InPtRegistrationID, (long)AllLookupValues.V_DiagnosisType.DIAGNOSIS_IN);
                    GetLatesDiagTrmtByPtID_InPt_OnlyForDia(InPtRegistrationID, (long)AllLookupValues.V_DiagnosisType.DIAGNOSIS_DAILY);
                    //▲===== 
                }
                else
                {
                    StatePopupNewWaiting();
                    AddBlankRow();
                }
                return;
            }

            if (Registration_DataStorage.PatientServiceRecordCollection == null
                    || Registration_DataStorage.PatientServiceRecordCollection.Count <= 0
                    || Registration_DataStorage.PatientServiceRecordCollection[0].DiagnosisTreatments == null
                    || Registration_DataStorage.PatientServiceRecordCollection[0].DiagnosisTreatments.Count <= 0)
            {
                if (IsDiagnosisOutHospital)
                {
                    GetLatesDiagTrmtByPtID_InPt(Registration_DataStorage.CurrentPatient.PatientID, (long)AllLookupValues.V_DiagnosisType.DIAGNOSIS_OUTHOS);
                }
                else
                {
                    GetLatesDiagTrmtByPtID_InPt(Registration_DataStorage.CurrentPatient.PatientID, (long)AllLookupValues.V_DiagnosisType.DIAGNOSIS_OUTDEPT);
                }
                return;
            }

            //KMx: Màn hình chẩn đoán nhập/xuất viện 
            if (IsDiagnosisOutHospital)
            {
                if (Registration_DataStorage.PatientServiceRecordCollection.Count > 0
                    && Registration_DataStorage.PatientServiceRecordCollection[0].DiagnosisTreatments.Any(x => x.V_DiagnosisType == (long)AllLookupValues.V_DiagnosisType.DIAGNOSIS_IN)
                    && Registration_DataStorage.PatientServiceRecordCollection[0].DiagnosisTreatments.Any(x => x.V_DiagnosisType == (long)AllLookupValues.V_DiagnosisType.DIAGNOSIS_OUTHOS))
                {
                    ConsultState = ConsultationState.EditConsultationState;
                    StateEdit();
                }

                //KMx: Nếu đăng ký hiện tại có chẩn đoán Nhập/ Xuất viện thì hiển thị chẩn đoán Nhập/ Xuất viện lên màn hình.
                if (Registration_DataStorage.PatientServiceRecordCollection[0].DiagnosisTreatments.Any(x => x.V_DiagnosisType == (long)AllLookupValues.V_DiagnosisType.DIAGNOSIS_IN)
                    || Registration_DataStorage.PatientServiceRecordCollection[0].DiagnosisTreatments.Any(x => x.V_DiagnosisType == (long)AllLookupValues.V_DiagnosisType.DIAGNOSIS_OUTHOS))
                {

                    DiagnosisTreatment item = Registration_DataStorage.PatientServiceRecordCollection[0].DiagnosisTreatments.Where(x => x.V_DiagnosisType == (long)AllLookupValues.V_DiagnosisType.DIAGNOSIS_IN
                                                                                                                || x.V_DiagnosisType == (long)AllLookupValues.V_DiagnosisType.DIAGNOSIS_OUTHOS).OrderByDescending(x => x.DTItemID).FirstOrDefault();

                    DiagTrmtItem = ObjectCopier.DeepCopy(item);
                    DiagTrmtItemCopy = ObjectCopier.DeepCopy(item);
                    DiagnosisIcd10Items_Load_InPt(DiagTrmtItem.DTItemID);
                    DiagnosisICD9Items_Load_InPt(DiagTrmtItem.DTItemID);
                      

                }
                //KMx: Nếu đăng ký hiện tại không có chẩn đoán Nhập/ Xuất viện thì lấy chẩn đoán xuất viện cuối cùng của đăng ký trước.
                else
                {
                    GetLatesDiagTrmtByPtID_InPt(Registration_DataStorage.CurrentPatient.PatientID, (long)AllLookupValues.V_DiagnosisType.DIAGNOSIS_OUTHOS);
                }
            }
            else
            {
                //KMx: Nếu đăng ký hiện tại có chẩn đoán xuất khoa thì hiển thị chẩn đoán xuất khoa cuối cùng lên màn hình.
                /*▼====: #003*/
                long mDiagnosisType = (long)AllLookupValues.V_DiagnosisType.DIAGNOSIS_OUTDEPT;
                if (IsProcedureEdit)
                    mDiagnosisType = (long)AllLookupValues.V_DiagnosisType.Diagnosis_SmallProcedure;
                else if (IsDailyDiagnosis)
                    mDiagnosisType = (long)AllLookupValues.V_DiagnosisType.DIAGNOSIS_DAILY;
                /*▲====: #003*/
                if (Registration_DataStorage.PatientServiceRecordCollection[0].DiagnosisTreatments.Any(x => x.V_DiagnosisType == mDiagnosisType && (!IsProcedureEdit || (Registration_DataStorage.CurrentPatientRegistrationDetail != null && x.PtRegDetailID == Registration_DataStorage.CurrentPatientRegistrationDetail.PtRegDetailID))))
                {
                    DiagnosisTreatment item = Registration_DataStorage.PatientServiceRecordCollection[0].DiagnosisTreatments.Where(x => (x.V_DiagnosisType == mDiagnosisType || (mDiagnosisType == (long)AllLookupValues.V_DiagnosisType.DIAGNOSIS_OUTDEPT && x.V_DiagnosisType == (long)AllLookupValues.V_DiagnosisType.DIAGNOSIS_INDEPT)) && (!IsProcedureEdit || (Registration_DataStorage.CurrentPatientRegistrationDetail != null && x.PtRegDetailID == Registration_DataStorage.CurrentPatientRegistrationDetail.PtRegDetailID))).OrderByDescending(x => x.DTItemID).FirstOrDefault();
                    DiagTrmtItem = ObjectCopier.DeepCopy(item);
                    DiagTrmtItemCopy = ObjectCopier.DeepCopy(item);
                    DiagnosisIcd10Items_Load_InPt(DiagTrmtItem.DTItemID);
                    DiagnosisICD9Items_Load_InPt(DiagTrmtItem.DTItemID);
                    /*▼====: #008*/
                    //TBL: Lấy BS ra chẩn đoán 
                    gSelectedDoctorStaff = DiagTrmtItem.ObjDoctorStaffID;
                    /*▲====: #008*/
                    if (IsProcedureEdit) //20200410 TBL: Nếu là màn hình nhập thủ thuật thì ẩn nút Tạo mới và Tạo mới dựa trên cũ
                    {
                        mNewConsultationState = false;
                        ConsultState = ConsultationState.EditConsultationState;
                    }
                }
                else if (IsProcedureEdit)
                {
                    DiagTrmtItem = new DiagnosisTreatment();
                    hasDiag = false;
                    AddBlankRow();
                    AddICD9BlankRow();
                    ResetDefaultForBehaving();
                    StateNewWaiting();
                    DiagTrmtItemCopy = ObjectCopier.DeepCopy(DiagTrmtItem);
                }
                //KMx: Nếu đăng ký hiện tại không có chẩn đoán xuất khoa thì lấy chẩn đoán xuất khoa cuối cùng của đăng ký trước.
                else
                {
                    GetLatesDiagTrmtByPtID_InPt(Registration_DataStorage.CurrentPatient.PatientID, mDiagnosisType);
                }
            }
        }



        public void InitPatientInfo()
        {
            refIDC10 = new PagedSortableCollectionView<DiseasesReference>();
            refIDC10.OnRefresh += RefIDC10_OnRefresh;
            refIDC10.PageSize = Globals.PageSize;
            refIDC10Item = new DiagnosisIcd10Items();
            refIDC10List = new ObservableCollection<DiagnosisIcd10Items>();
            pageIDC9 = new PagedSortableCollectionView<RefICD9>();
            pageIDC9.OnRefresh += PageIDC9_OnRefresh;
            pageIDC9.PageSize = Globals.PageSize;
            refICD9Item = new DiagnosisICD9Items();
            refICD9List = new ObservableCollection<DiagnosisICD9Items>();
            if (Registration_DataStorage != null && Registration_DataStorage.CurrentPatient != null && Registration_DataStorage.CurrentPatient.PatientID > 0)
            {
                InitPhyExam(Registration_DataStorage.CurrentPatient.PatientID);
                if (CheckDangKyHopLe(Registration_DataStorage.CurrentPatientRegistration.DeepCopy()))
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
            GetInjuryCertificates_ByPtRegID(Registration_DataStorage.CurrentPatientRegistration.PtRegistrationID, (long)Registration_DataStorage.CurrentPatientRegistration.V_RegistrationType);
            if (IsAdmRequest)
            {
                GetListAdmissionCriteria();
            }
            if (Registration_DataStorage != null 
                && Registration_DataStorage.CurrentPatientRegistration != null 
                && Registration_DataStorage.CurrentPatientRegistration.AdmissionInfo != null
                && Registration_DataStorage.CurrentPatientRegistration.AdmissionInfo.AdmissionDate != null)
            {
                NgaySinhCon = Registration_DataStorage.CurrentPatientRegistration.AdmissionInfo.AdmissionDate;
            }
            if (IsConsultationKhoaSanEdit)
            {
                GetSoConChet_KhoaSan();
            }
        }

        private void PageIDC9_OnRefresh(object sender, aEMR.Common.Collections.RefreshEventArgs e)
        {
            SearchICD9(procName, ICD9SearchType, pageIDC9.PageIndex, pageIDC9.PageSize);
        }

        private void RefIDC10_OnRefresh(object sender, aEMR.Common.Collections.RefreshEventArgs e)
        {
            LoadRefDiseases(Name, Type, refIDC10.PageIndex, refIDC10.PageSize);
        }

        private bool CheckDangKyHopLe(PatientRegistration aRegistration)
        {
            if (this.IsForCollectDiagnosis)
            {
                aRegistration.V_RegistrationType = AllLookupValues.RegistrationType.NOI_TRU;
            }
            return Globals.CheckValidRegistrationForConsultation(aRegistration, null, Registration_DataStorage.CurrentPatient, null, false);
            //if (aRegistration == null)
            //{
            //    MessageBox.Show(eHCMSResources.Z0402_G1_KgBietDKLoaiNao);
            //    return false;
            //}

            //if (aRegistration.V_RegistrationType != AllLookupValues.RegistrationType.NOI_TRU)
            //{
            //    MessageBox.Show(eHCMSResources.A0246_G1_Msg_InfoBNKhongPhaiNoiTru_ChiXem, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
            //    return false;
            //}


            //if ((aRegistration.V_RegistrationStatus != (long)AllLookupValues.RegistrationStatus.COMPLETED)
            //    && (aRegistration.V_RegistrationStatus != (long)AllLookupValues.RegistrationStatus.INVALID)
            //    && (aRegistration.V_RegistrationStatus != (long)AllLookupValues.RegistrationStatus.PENDING))                
            //{
            //    return true;
            //}
            //else
            //{
            //    switch (aRegistration.V_RegistrationStatus)
            //    {
            //        case (long)AllLookupValues.RegistrationStatus.COMPLETED:
            //            {
            //                MessageBox.Show("'" + Registration_DataStorage.CurrentPatient.FullName.Trim() + "'" + eHCMSResources.A0065_G1_Msg_InfoKhongCDDc_DKDaDong);
            //                break;
            //            }
            //        case (long)AllLookupValues.RegistrationStatus.INVALID:
            //            {
            //                MessageBox.Show("'" + Registration_DataStorage.CurrentPatient.FullName.Trim() + "'" + eHCMSResources.A0066_G1_Msg_InfoKhongCDDc_DKKhHopLe);
            //                break;
            //            }
            //        case (long)AllLookupValues.RegistrationStatus.PENDING:
            //            {
            //                MessageBox.Show("'" + Registration_DataStorage.CurrentPatient.FullName.Trim() + "'" + eHCMSResources.A0064_G1_Msg_InfoKhongCDDc_DKChuaHTat);
            //                break;
            //            }
            //    }
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
            };
            GlobalsNAV.ShowDialog<IcwPhysiscalExam>(onInitDlg);
        }

        #region Properties member
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

        private ObservableCollection<DiagnosisICD9Items> _refICD9ListCopy;
        public ObservableCollection<DiagnosisICD9Items> refICD9ListCopy
        {
            get
            {
                return _refICD9ListCopy;
            }
            set
            {
                if (_refICD9ListCopy != value)
                {
                    _refICD9ListCopy = value;
                    NotifyOfPropertyChange(() => refICD9ListCopy);
                }
            }
        }
        private ObservableCollection<AdmissionCriteria> _AdmissionCriteriaListA;
        public ObservableCollection<AdmissionCriteria> AdmissionCriteriaListA
        {
            get
            {
                return _AdmissionCriteriaListA;
            }
            set
            {
                if (_AdmissionCriteriaListA != value)
                {
                    _AdmissionCriteriaListA = value;
                    NotifyOfPropertyChange(() => AdmissionCriteriaListA);
                }
            }
        }
        private ObservableCollection<AdmissionCriteria> _AdmissionCriteriaListB;
        public ObservableCollection<AdmissionCriteria> AdmissionCriteriaListB
        {
            get
            {
                return _AdmissionCriteriaListB;
            }
            set
            {
                if (_AdmissionCriteriaListB != value)
                {
                    _AdmissionCriteriaListB = value;
                    NotifyOfPropertyChange(() => AdmissionCriteriaListB);
                }
            }
        }


        private DiagnosisTreatment _DiagTrmtItem;
        public DiagnosisTreatment DiagTrmtItem
        {
            get
            {
                return _DiagTrmtItem;
            }
            set
            {
                _DiagTrmtItem = value;


                //Để code bên dưới vào trong hàm SetDeparment() để cho nút "Tạo mới dựa trên chẩn đoán cũ" dùng chung (03/08/2015 16:43).
                //if (_DiagTrmtItem != null)
                //{
                //    if (_DiagTrmtItem.Department != null && _DiagTrmtItem.Department.DeptID > 0)
                //    {
                //        DepartmentContent.SelectedItem = _DiagTrmtItem.Department;
                //    }
                //    else
                //    {
                //        DepartmentContent.SelectedItem = DepartmentContent.Departments != null ? DepartmentContent.Departments.FirstOrDefault() : null;
                //    }
                //}

                SetDepartment();

                NotifyOfPropertyChange(() => DiagTrmtItem);
                NotifyOfPropertyChange(() => btCreateNewByOldIsEnabled);
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
                return _btSaveCreateNewIsEnabled && !IsProcedureEdit;
            }
            set
            {
                _btSaveCreateNewIsEnabled = value;
                NotifyOfPropertyChange(() => btSaveCreateNewIsEnabled);
            }
        }

        private bool _btCreateNewIsEnabled;
        public bool btCreateNewIsEnabled
        {
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
            get { return _btCreateNewByOldIsEnabled; }
            set
            {
                _btCreateNewByOldIsEnabled = value && hasDiag;
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


        private bool _btUpdateIsEnabled = false;
        public bool btUpdateIsEnabled
        {
            get
            {
                return _btUpdateIsEnabled && !IsProcedureEdit;
            }
            set
            {
                _btUpdateIsEnabled = value;
                NotifyOfPropertyChange(() => btUpdateIsEnabled);
            }
        }
        private bool _IsUpdateDiagConfirmInPT;
        public bool IsUpdateDiagConfirmInPT
        {
            get { return _IsUpdateDiagConfirmInPT; }
            set
            {
                if (_IsUpdateDiagConfirmInPT = value)
                {
                    _IsUpdateDiagConfirmInPT = value;
                    NotifyOfPropertyChange(() => IsUpdateDiagConfirmInPT);
                }
            }
        }
        private bool _AdmissionCriteriaListBIsChecked;
        public bool AdmissionCriteriaListBIsChecked
        {
            get { return _AdmissionCriteriaListBIsChecked; }
            set
            {
                _AdmissionCriteriaListBIsChecked = value;
                NotifyOfPropertyChange(() => AdmissionCriteriaListBIsChecked);
            }
        }
        private bool _AdmissionCriteriaListAIsChecked;
        public bool AdmissionCriteriaListAIsChecked
        {
            get { return _AdmissionCriteriaListAIsChecked; }
            set
            {
                _AdmissionCriteriaListAIsChecked = value;
                NotifyOfPropertyChange(() => AdmissionCriteriaListAIsChecked);
            }
        }
        private string _AdmissionCriteriaList;
        public string AdmissionCriteriaList
        {
            get { return _AdmissionCriteriaList; }
            set
            {
                _AdmissionCriteriaList = value;
                NotifyOfPropertyChange(() => AdmissionCriteriaList);
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

        private void CopyListICD9()
        {
            if (refICD9List != null)
            {
                refICD9ListCopy = refICD9List.DeepCopy();
            }
            else
            {
                RefICD9Copy = null;
            }
            AddICD9BlankRow();
        }

        private void CopyListICD9ForNew()
        {
            if (refICD9List != null)
            {
                refICD9ListCopy = refICD9List.DeepCopy();
            }
            else
            {
                refICD9ListCopy = null;
            }
            refICD9List = new ObservableCollection<DiagnosisICD9Items>();
            AddICD9BlankRow();
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


        public void GetLatesDiagTrmtByPtID_InPt(long patientID, long? V_DiagnosisType)
        {
            this.ShowBusyIndicator();
            //IsWaitingGetLatestDiagnosisTreatmentByPtID = true;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new ePMRsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetLatestDiagnosisTreatmentByPtID_InPt(patientID, V_DiagnosisType, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            DiagTrmtItem = contract.EndGetLatestDiagnosisTreatmentByPtID_InPt(asyncResult);
                            NotifyOfPropertyChange(() => btCreateNewByOldIsEnabled);
                            if (DiagTrmtItem != null && DiagTrmtItem.DTItemID > 0)
                            {
                                //Có DiagnosisTreatment rồi
                                //FormEditorIsEnabled = false;
                                //DiagnosisIcd10Items_Load_InPt(DiagTrmtItem.ServiceRecID.GetValueOrDefault());
                                DiagnosisIcd10Items_Load_InPt(DiagTrmtItem.DTItemID);
                                DiagnosisICD9Items_Load_InPt(DiagTrmtItem.DTItemID);
                                hasDiag = true;
                                ButtonForHasDiag();
                                /*▼====: #008*/
                                //TBL: Lấy BS ra chẩn đoán 
                                gSelectedDoctorStaff = DiagTrmtItem.ObjDoctorStaffID;
                                /*▲====: #008*/
                            }
                            else
                            {
                                hasDiag = false;
                                AddBlankRow();
                                AddICD9BlankRow();
                                ResetDefaultForBehaving();
                                StateNewWaiting();
                            }
                            DiagTrmtItemCopy = ObjectCopier.DeepCopy(DiagTrmtItem);
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
        private void SetDefaultDiagnosisType()
        {
            if (DiagTrmtItem == null)
            {
                return;
            }

            if (RefDiagnosis != null && Registration_DataStorage.CurrentPatientRegistration != null && Registration_DataStorage.CurrentPatientRegistration.V_RegistrationType == AllLookupValues.RegistrationType.NOI_TRU)
            {
                //KMx: Màn hình chẩn đoán chia làm 2 link. 1 link "Chẩn đoán xuất khoa" và 1 link "Chẩn đoán xuất viện", tùy theo link mà chọn loại chẩn đoán phù hợp (09/06/2015 17:38).
                //Dời V_DiagnosisType từ PatientServiceRecord sang DiagnosisTreatment.
                //if (RefDiagnosis.Any(x => x.LookupID == (long)AllLookupValues.V_DiagnosisType.DIAGNOSIS_OUTHOS))
                //{
                //    DiagTrmtItem.PatientServiceRecord.V_DiagnosisType = (long)AllLookupValues.V_DiagnosisType.DIAGNOSIS_OUTHOS;
                //}
                //else
                //{
                //    DiagTrmtItem.PatientServiceRecord.V_DiagnosisType = RefDiagnosis.FirstOrDefault().LookupID;
                //}
                if (IsDiagnosisOutHospital)
                {
                    if (RefDiagnosis.Any(x => x.LookupID == (long)AllLookupValues.V_DiagnosisType.DIAGNOSIS_OUTHOS))
                    {
                        DiagTrmtItem.V_DiagnosisType = (long)AllLookupValues.V_DiagnosisType.DIAGNOSIS_OUTHOS;
                    }
                    else
                    {
                        DiagTrmtItem.V_DiagnosisType = RefDiagnosis.FirstOrDefault().LookupID;
                    }
                }
                else if (IsProcedureEdit)
                {
                    if (RefDiagnosis.Any(x => x.LookupID == (long)AllLookupValues.V_DiagnosisType.Diagnosis_SmallProcedure))
                    {
                        DiagTrmtItem.V_DiagnosisType = (long)AllLookupValues.V_DiagnosisType.Diagnosis_SmallProcedure;
                    }
                    else
                    {
                        DiagTrmtItem.V_DiagnosisType = RefDiagnosis.FirstOrDefault().LookupID;
                    }
                }
                /*▼====: #003*/
                else if (IsDailyDiagnosis)
                {
                    if (RefDiagnosis.Any(x => x.LookupID == (long)AllLookupValues.V_DiagnosisType.DIAGNOSIS_DAILY))
                    {
                        DiagTrmtItem.V_DiagnosisType = (long)AllLookupValues.V_DiagnosisType.DIAGNOSIS_DAILY;
                    }
                    else
                    {
                        DiagTrmtItem.V_DiagnosisType = RefDiagnosis.FirstOrDefault().LookupID;
                    }
                }
                /*▲====: #003*/
                else
                {
                    if (RefDiagnosis.Any(x => x.LookupID == (long)AllLookupValues.V_DiagnosisType.DIAGNOSIS_OUTDEPT))
                    {
                        DiagTrmtItem.V_DiagnosisType = (long)AllLookupValues.V_DiagnosisType.DIAGNOSIS_OUTDEPT;
                    }
                    else
                    {
                        DiagTrmtItem.V_DiagnosisType = RefDiagnosis.FirstOrDefault().LookupID;
                    }
                }
            }
            else
            {
                //DiagTrmtItem.PatientServiceRecord.V_DiagnosisType = (long)AllLookupValues.V_DiagnosisType.DIAGNOSIS_NORMAL;

                DiagTrmtItem.V_DiagnosisType = (long)AllLookupValues.V_DiagnosisType.DIAGNOSIS_NORMAL;
            }
        }

        private void ResetDefaultForBehaving()
        {
            if (DiagTrmtItem == null)
            {
                return;
            }

            DiagTrmtItem.DiagnosisDate = Globals.GetCurServerDateTime();

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

            //if (RefDiagnosis != null && Registration_DataStorage.CurrentPatientRegistration != null && Registration_DataStorage.CurrentPatientRegistration.V_RegistrationType == AllLookupValues.RegistrationType.NOI_TRU)
            //{
            //    //KMx: Màn hình chẩn đoán chia làm 2 link. 1 link "Chẩn đoán xuất khoa" và 1 link "Chẩn đoán xuất viện", tùy theo link mà chọn loại chẩn đoán phù hợp (09/06/2015 17:38).
            //    //Dời V_DiagnosisType từ PatientServiceRecord sang DiagnosisTreatment.
            //    //if (RefDiagnosis.Any(x => x.LookupID == (long)AllLookupValues.V_DiagnosisType.DIAGNOSIS_OUTHOS))
            //    //{
            //    //    DiagTrmtItem.PatientServiceRecord.V_DiagnosisType = (long)AllLookupValues.V_DiagnosisType.DIAGNOSIS_OUTHOS;
            //    //}
            //    //else
            //    //{
            //    //    DiagTrmtItem.PatientServiceRecord.V_DiagnosisType = RefDiagnosis.FirstOrDefault().LookupID;
            //    //}
            //    if (IsDiagnosisOutHospital)
            //    {
            //        if (RefDiagnosis.Any(x => x.LookupID == (long)AllLookupValues.V_DiagnosisType.DIAGNOSIS_OUTHOS))
            //        {
            //            DiagTrmtItem.V_DiagnosisType = (long)AllLookupValues.V_DiagnosisType.DIAGNOSIS_OUTHOS;
            //        }
            //        else
            //        {
            //            DiagTrmtItem.V_DiagnosisType = RefDiagnosis.FirstOrDefault().LookupID;
            //        }
            //    }
            //    else
            //    {
            //        if (RefDiagnosis.Any(x => x.LookupID == (long)AllLookupValues.V_DiagnosisType.DIAGNOSIS_OUTDEPT))
            //        {
            //            DiagTrmtItem.V_DiagnosisType = (long)AllLookupValues.V_DiagnosisType.DIAGNOSIS_OUTDEPT;
            //        }
            //        else
            //        {
            //            DiagTrmtItem.V_DiagnosisType = RefDiagnosis.FirstOrDefault().LookupID;
            //        }
            //    }
            //}
            //else
            //{
            //    //DiagTrmtItem.PatientServiceRecord.V_DiagnosisType = (long)AllLookupValues.V_DiagnosisType.DIAGNOSIS_NORMAL;

            //    DiagTrmtItem.V_DiagnosisType = (long)AllLookupValues.V_DiagnosisType.DIAGNOSIS_NORMAL;
            //}

            //KMx: Cắt code phía trên bỏ vào trong hàm này để nút "Tạo mới dựa trên cũ" sử dụng chung (25/06/2015 17:14).
            SetDefaultDiagnosisType();

            if (RefMedRecTemplate != null)
            {
                DiagTrmtItem.MDRptTemplateID = RefMedRecTemplate.FirstOrDefault().MDRptTemplateID;
            }
            else
            {
                DiagTrmtItem.MDRptTemplateID = 1;
            }
        }


        // Doi ham nay thanh Coroutine de dung canh bao bigsize khi chan doan co ma "Z10"
        private IEnumerator<IResult> AllCheck(SmallProcedure aSmallProcedure, List<Resources> resourceList = null)
        {
            if (!CheckEditDiagnosis())
            {
                yield break;
            }

            if (!CheckDiagnosisType())
            {
                yield break;
            }

            if (!CheckDepartment())
            {
                yield break;
            }
            // HPT 26/09/2016: Đổi luật đối với chẩn đoán xuất khoa. Trước đây phải lấy Guid từ dòng nhập bỏ vào chẩn đoán nên mới làm như vầy. 
            // Theo luật mới, Guid sẽ lấy từ chẩn đoán bỏ vào dòng nhập nên không cần code này nữa. Comment lại nếu test chạy ổn sẽ bỏ đi luôn
            // Hpt 06/11/2015: chỉ kiểm tra khi hiệu chỉnh chẩn đoán xuất khoa và có thay đổi phòng
            //if (DiagTrmtItem.V_DiagnosisType == (long)AllLookupValues.V_DiagnosisType.DIAGNOSIS_OUTDEPT && DiagTrmtItemCopy.Department.DeptID != DiagTrmtItem.Department.DeptID)
            //{
            //    if (!CheckToGetGuid())
            //    {
            //        yield break;
            //    }
            //}
            if (!CheckEmptyFields())
            {
                yield break;
            }

            //20190402 TTM: Kiểm tra xem bệnh nhân đã nhập viện chưa nếu nhập viện rồi và đang ở màn hình đề nghị thì ngăn lại không cho thao tác cập nhật đề nghị nhập viện
            if (!CheckAdmission() && this.IsForCollectDiagnosis)
            {
                yield break;
            }

            if (gSelectedDoctorStaff == null || gSelectedDoctorStaff.StaffID == 0)
            {
                MessageBox.Show(eHCMSResources.A0376_G1_Msg_InfoChuaChonBS, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                yield break;
            }
            if (IsAdmRequest && String.IsNullOrEmpty(DiagTrmtItem.ReasonHospitalStay))
            {
                MessageBox.Show("Chưa nhập lý do vào nội trú", eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                yield break;
            }
            if (IsAdmRequest && !CheckAdmissionCriteria())
            {
                MessageBox.Show("Chưa chọn tiêu chí nhập viện", eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                yield break;
            }
            if (IsConsultationKhoaSanEdit && SoConChet > 0 && NgayConChet == null)
            {
                MessageBox.Show("Người dùng chưa chọn Ngày con chết. Vui lòng kiểm tra lại", eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                yield break;
            }
            if (NeedICD10() && CheckedIsMain() && CheckedICD9IsMain())
            {
                //UpdateDoctorStaffID();
                DiagTrmtItem.ICD10List = String.Join(",", from item in refIDC10List
                                                          where (!string.IsNullOrEmpty(item.ICD10Code) && item.DiseasesReference != null)
                                                          select item.ICD10Code);

                //KMx: Phải set loại đăng ký để service gọi stored update tương ứng (01/11/2014 10:30).
                DiagTrmtItem.PatientServiceRecord.V_RegistrationType = Registration_DataStorage.CurrentPatientRegistration.V_RegistrationType;

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
                UpdateDiagTrmt(aSmallProcedure, resourceList);
                if (IsConsultationKhoaSanEdit)
                {
                    AddNewDiagDetal_KhoaSan();
                }
            }
        }

        public void UpdateDiagTrmt(SmallProcedure aSmallProcedure = null, List<Resources> resourceList = null)
        {
            this.ShowBusyIndicator();
            //KMx: Lưu BS cập nhật sau cùng (28/03/2014 13:59).
            //DiagTrmtItem.DoctorStaffID = Globals.LoggedUserAccount.StaffID.Value;
            DiagTrmtItem.DoctorStaffID = gSelectedDoctorStaff.StaffID;
            IsWaitingUpdate = true;
            long listID = Compare2Object();
            long DiagnosisICD9ListID = Compare2ICD9List();
            var t = new Thread(() =>
            {
                using (var serviceFactory = new ePMRsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    DiagTrmtItem.DeptLocationID = Globals.DeptLocation.DeptLocationID;
                    contract.BeginUpdateDiagnosisTreatment_InPt_V2(DiagTrmtItem, listID, refIDC10List, DiagnosisICD9ListID, refICD9List
                    //▼===== #018
                    , resourceList
                    //▲===== #018
                    , aSmallProcedure, IsUpdateDiagConfirmInPT, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            List<InPatientDeptDetail> ReloadInPatientDeptDetails = new List<InPatientDeptDetail>();
                            int VersionNumberOut = 0;
                            if (contract.EndUpdateDiagnosisTreatment_InPt_V2(out ReloadInPatientDeptDetails, out VersionNumberOut, asyncResult))
                            {
                                FormEditorIsEnabled = false;

                                if (this.IsForCollectDiagnosis)
                                {
                                    StatePopupUpdate();
                                }
                                else
                                {
                                    StateNewEdit();
                                }
                                if (refIDC10List != null)
                                {
                                    refIDC10List = refIDC10List.Where(x => x.DiseasesReference != null).ToObservableCollection();
                                }

                                if (refICD9List != null)
                                {
                                    refICD9List = refICD9List.Where(x => x.RefICD9 != null).ToObservableCollection();
                                }
                                //20190402 TTM: Thêm điều kiện kiểm tra thông tin nhập viện
                                //              Lý do: Bệnh nhân chỉ đề nghị nhập viện => Chưa nhập viện => Không có thông tin nhập viện => Lỗi nếu bệnh nhân muốn cập nhật thông tin đề nghị nhập viện.
                                if (Registration_DataStorage.CurrentPatientRegistration.AdmissionInfo != null && IsForCollectDiagnosis)
                                {
                                    Registration_DataStorage.CurrentPatientRegistration.AdmissionInfo.InPatientDeptDetails = new ObservableCollection<InPatientDeptDetail>(ReloadInPatientDeptDetails);
                                }

                                //KMx: Sau khi lưu chẩn đoán, reload Service Record (22/05/2014 09:48).
                                IConsultationModule consultVM = Globals.GetViewModel<IConsultationModule>();
                                if (consultVM.MainContent is IConsultationsSummary_InPt)
                                {
                                    (consultVM.MainContent as IConsultationsSummary_InPt).CallSetInPatientInfoForConsultation(Registration_DataStorage.CurrentPatientRegistration, Registration_DataStorage.CurrentPatientRegistrationDetail);
                                }

                                //▼===== 20190930: IConsultationModule sẽ bị loại bỏ, hiện tại hàm PatientServiceRecordsGetForKhamBenh_Ext không còn sử dụng nữa.
                                //else
                                //{
                                //    consultVM.PatientServiceRecordsGetForKhamBenh_Ext();
                                //}
                                //▲===== 20190930

                                //KMx: Sau khi lưu chẩn đoán, xóa danh sách chẩn đoán. Vì danh sách không tự động load lại.
                                //Nếu không xóa thì chẩn đoán vừa lưu sẽ khác với chẩn đoán trong danh sách (22/05/2014 09:48).
                                DiagTrmtItem.VersionNumber = VersionNumberOut;
                                Globals.EventAggregator.Publish(new ClearAllDiagnosisListAfterUpdateEvent_InPt());
                                MessageBox.Show(eHCMSResources.K2782_G1_DaCNhat);
                            }
                            else
                            {
                                //KMx: Dời V_DiagnosisType từ PatientServiceRecord sang DiagnosisTreatment (09/06/2015 17:42).
                                //if (DiagTrmtItem.PatientServiceRecord.V_DiagnosisType == (long)AllLookupValues.V_DiagnosisType.DIAGNOSIS_IN)
                                if (DiagTrmtItem.V_DiagnosisType == (long)AllLookupValues.V_DiagnosisType.DIAGNOSIS_IN)
                                {
                                    MessageBox.Show(eHCMSResources.Z0403_G1_DaCoCDoanNV, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                }
                                //else if (DiagTrmtItem.PatientServiceRecord.V_DiagnosisType == (long)AllLookupValues.V_DiagnosisType.DIAGNOSIS_OUTHOS)
                                else if (DiagTrmtItem.V_DiagnosisType == (long)AllLookupValues.V_DiagnosisType.DIAGNOSIS_OUTHOS)
                                {
                                    MessageBox.Show(eHCMSResources.Z0404_G1_DaCoCDoanXV, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                }
                                else
                                {
                                    MessageBox.Show(eHCMSResources.A0272_G1_Msg_InfoCNhatFail, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                            if (ex.Message.Contains("[ERROR-TBL]"))
                            {
                                ClientLoggerHelper.LogError("ConsultationsSummary_V2ViewModel UpdateDiagTrmt - " + Globals.GetCurServerDateTime() + " -  [" + Globals.LoggedUserAccount.StaffID + " - "
                                + Globals.LoggedUserAccount.Staff.FullName + "] - " + "PatientID: " + (Registration_DataStorage != null && Registration_DataStorage.CurrentPatient != null ? Registration_DataStorage.CurrentPatient.PatientID : 0).ToString()
                                + " PtRegistrationID: " + (Registration_DataStorage != null && Registration_DataStorage.CurrentPatientRegistration != null ? Registration_DataStorage.CurrentPatientRegistration.PtRegistrationID : 0).ToString()
                                + " PtRegDetailID: " + (aSmallProcedure != null ? aSmallProcedure.PtRegDetailID : 0).ToString()
                                + " SmallProcedureID: " + (aSmallProcedure != null ? aSmallProcedure.SmallProcedureID : 0).ToString() + ex.Message);
                            }
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


        public void Handle(CommonClosedPhysicalForDiagnosis_InPtEvent message)
        {
            InitPhyExam(Registration_DataStorage.CurrentPatient.PatientID);
        }

        private void InitPhyExam(long patientID)
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                using (var serviceFactory = new SummaryServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetLastPhyExamByPtID(patientID, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            PtPhyExamItem = contract.EndGetLastPhyExamByPtID(asyncResult);
                            //KMx: Sau khi lấy PhysicalExamination thì phải gán lại vào Globals.
                            //Nếu không, khi chuyển qua trang thông tin chung, hoặc Ra toa thì vẫn còn hiển thị cái cũ (16/06/2014 10:55).
                            Globals.curPhysicalExamination = PtPhyExamItem;
                            //▼===== #009: Bắn sự kiện về màn hình thông tin chung để hiển thị sau khi lưu và cập nhật.
                            Globals.EventAggregator.Publish(new SetPhysicalForSummary_InPtEvent { });
                            //▲===== #009
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

        private void DiagnosisIcd10Items_Load_InPt(long DTItemID)
        {
            if (DTItemID <= 0)
            {
                return;
            }

            this.ShowBusyIndicator();

            var t = new Thread(() =>
            {
                using (var serviceFactory = new ePMRsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    //contract.BeginGetDiagnosisIcd10Items_Load_InPt(ServiceRecID, Globals.DispatchCallback((asyncResult) =>
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

        private void DiagnosisICD9Items_Load_InPt(long DTItemID)
        {
            if (DTItemID <= 0)
            {
                return;
            }

            this.ShowBusyIndicator();

            var t = new Thread(() =>
            {
                using (var serviceFactory = new ePMRsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginGetDiagnosisICD9Items_Load_InPt(DTItemID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndGetDiagnosisICD9Items_Load_InPt(asyncResult);
                            refICD9List = results.ToObservableCollection();
                            refICD9ListCopy = refICD9List.DeepCopy();

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
        public void Handle(ConsultationDoubleClickEvent_InPt_2 obj)
        {
            //KMx: Vào màn hình chẩn đoán xuất khoa, BN không có bất kỳ chẩn đoán xuất khoa (kể cả DK cũ). Qua tab các chẩn đoán cũ, double click vào chẩn đoán xuất viện cũ, không hiện lên nút "Tạo mới dựa trên chẩn đoán cũ".
            //Lý do: nút "Tạo mới dựa trên chẩn đoán cũ" phụ thuộc vào btCreateNewByOldIsEnabled, mà btCreateNewByOldIsEnabled lại phụ thuộc vào hasDiag nên set hasDiag = true (10/08/2015 15:40).
            hasDiag = true;
            btCancel();
            //Globals.ClearPatientAllDetails();
            if (obj.DiagTrmtItem != null)
            {
                UpdateDiagTrmtItemIntoLayout(obj.DiagTrmtItem, obj.refIDC10List, obj.refICD9List);
            }
        }
        #endregion

        #region List ICD10 member
        public ICD10Changed gICD10Changed { get; set; }

        private ObservableCollection<DiagnosisIcd10Items> _refIDC10List;
        public ObservableCollection<DiagnosisIcd10Items> refIDC10List
        {
            get
            {
                return _refIDC10List;
            }
            set
            {
                if (_refIDC10List != value)
                {
                    _refIDC10List = value;
                }
                NotifyOfPropertyChange(() => refIDC10List);
            }
        }

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
                }
                NotifyOfPropertyChange(() => refIDC10Item);
            }
        }

        private PagedSortableCollectionView<DiseasesReference> _refIDC10;
        public PagedSortableCollectionView<DiseasesReference> refIDC10
        {
            get
            {
                return _refIDC10;
            }
            set
            {
                if (_refIDC10 != value)
                {
                    _refIDC10 = value;
                }
                NotifyOfPropertyChange(() => refIDC10);
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
                        , Registration_DataStorage.CurrentPatient.PatientID
                        , Globals.GetCurServerDateTime()
                        , Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            int Total = 10;
                            var results = contract.EndSearchRefDiseases(out Total, asyncResult);
                            refIDC10.Clear();
                            refIDC10.TotalItemCount = Total;
                            if (results != null)
                            {
                                foreach (DiseasesReference p in results)
                                {
                                    refIDC10.Add(p);
                                }
                            }
                            if (type == 0)
                            {
                                Acb_ICD10_Code.ItemsSource = refIDC10;
                                Acb_ICD10_Code.PopulateComplete();
                            }
                            else
                            {
                                Acb_ICD10_Name.ItemsSource = refIDC10;
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

        AutoCompleteBox Auto;
        AutoCompleteBox DiseasesName;
        private string Name = "";
        private byte Type = 0;
        public void DiseaseName_Loaded(object sender, RoutedEventArgs e)
        {
            DiseasesName = (AutoCompleteBox)sender;
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
        //public void aucICD10_Populating(object sender, PopulatingEventArgs e)
        //{
        //    if (IsCode)
        //    {
        //        e.Cancel = true;
        //        Auto = (AutoCompleteBox)sender;
        //        Name = e.Parameter;
        //        Type = 0;
        //        refIDC10.PageIndex = 0;
        //        LoadRefDiseases(e.Parameter, 0, 0, refIDC10.PageSize);
        //    }
        //}

        AutoCompleteBox AutoName;
        //public void aucICD10Name_Populating(object sender, PopulatingEventArgs e)
        //{
        //    if (!IsCode && ColumnIndex == 1)
        //    {
        //        e.Cancel = true;
        //        AutoName = (AutoCompleteBox)sender;
        //        Name = e.Parameter;
        //        Type = 1;
        //        refIDC10.PageIndex = 0;
        //        LoadRefDiseases(e.Parameter, 1, 0, refIDC10.PageSize);
        //    }
        //}

        public void AutoCompleteBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (IsCode)
            {
                Auto = (AutoCompleteBox)sender;
                if (refIDC10Item != null)
                {
                    refIDC10Item.DiseasesReference = Auto.SelectedItem as DiseasesReference;
                }
            }
        }

        //private bool isDropDown = false;
        //public void AxAutoComplete_DropDownClosing(object sender, RoutedPropertyChangedEventArgs<bool> e)
        //{
        //    isDropDown = true;
        //}
        //public void AxAutoComplete_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        //{
        //    if (!isDropDown)
        //    {
        //        return;
        //    }
        //    isDropDown = false;
        //    Auto = (AutoCompleteBox)sender;
        //    if (refIDC10Item != null)
        //    {
        //        refIDC10Item.DiseasesReference = Auto.SelectedItem as DiseasesReference;
        //        if (CheckExists(refIDC10Item))
        //        {
        //            GetDiagTreatmentFinal(refIDC10Item.DiseasesReference);
        //        }
        //    }
        //}

        //private bool isDiseaseDropDown = false;
        //public void DiseaseName_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        //{
        //    isDiseaseDropDown = true;
        //}

        //public void DiseaseName_DropDownClosing(object sender, RoutedPropertyChangedEventArgs<bool> e)
        //{
        //    if (!isDiseaseDropDown)
        //    {
        //        return;
        //    }
        //    isDiseaseDropDown = false;
        //    DiseasesName = (AutoCompleteBox)sender;
        //    refIDC10Item.DiseasesReference = ((AutoCompleteBox)sender).SelectedItem as DiseasesReference;
        //    if (CheckExists(refIDC10Item))
        //    {
        //        GetDiagTreatmentFinal(refIDC10Item.DiseasesReference);
        //    }
        //}
        public void AutoName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!IsCode)
            {
                AutoName = (AutoCompleteBox)sender;
                if (refIDC10Item != null)
                {
                    refIDC10Item.DiseasesReference = AutoName.SelectedItem as DiseasesReference;
                }
            }
        }
        public void AutoName_DropDownClosing(object sender, RoutedPropertyChangingEventArgs<bool> e)
        {
            if (!IsCode)
            {
                AutoName = (AutoCompleteBox)sender;
                if (refIDC10Item != null)
                {
                    refIDC10Item.DiseasesReference = AutoName.SelectedItem as DiseasesReference;
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
            if (gICD10Changed != null)
            {
                gICD10Changed(refIDC10List);
            }
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

        private void GetAllLookupValuesByType()
        {
            ObservableCollection<Lookup> DiagICDSttLookupList = Globals.AllLookupValueList.Where(x => x.ObjectTypeID == (long)LookupValues.V_DiagIcdStatus).ToObservableCollection();

            if (DiagICDSttLookupList == null || DiagICDSttLookupList.Count <= 0)
            {
                MessageBox.Show(eHCMSResources.A0750_G1_Msg_InfoKhTimThayStatusICD10, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
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

        private bool ICD9Equal(DiagnosisICD9Items a, DiagnosisICD9Items b)
        {
            return a.DiagICD9ItemID == b.DiagICD9ItemID
                && a.DiagnosisICD9ListID == b.DiagnosisICD9ListID
                && a.ICD9Code == b.ICD9Code
                && a.IsMain == b.IsMain
                && a.IsCongenital == b.IsCongenital;
        }

        private long Compare2Object()
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

        private long Compare2ICD9List()
        {
            long ListID = 0;
            ObservableCollection<DiagnosisICD9Items> temp = refICD9List.Where(x => x.RefICD9 != null).ToObservableCollection();
            if (refICD9ListCopy != null && refICD9ListCopy.Count > 0 && refICD9ListCopy.Count == temp.Count)
            {
                int icount = 0;
                for (int i = 0; i < refICD9ListCopy.Count; i++)
                {
                    for (int j = 0; j < temp.Count; j++)
                    {
                        if (ICD9Equal(refICD9ListCopy[i], refICD9List[j]))
                        {
                            icount++;
                        }
                    }

                }
                if (icount == refICD9ListCopy.Count)
                {
                    ListID = refICD9ListCopy.FirstOrDefault().DiagnosisICD9ListID;
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
            //▼====: #015
            if (refIDC10List != null && refIDC10List.Where(x => x.ICD10Code != null && x.ICD10Code.Contains("†") && !x.IsMain).ToObservableCollection().Count() > 0)
            {
                Globals.ShowMessage("Tồn tại mã bệnh có dấu găm đang là mã bệnh kèm theo. Vui lòng kiểm tra lại hoặc liên hệ KHTH!", eHCMSResources.G0442_G1_TBao);
                return false;
            }
            //▲====: #015
            //▼====: #013
            bool HasICDCannotBeMain = false;
            string NameOfICDCannotBeMain = "Danh sách ICD không được làm mã bệnh chính:";
            ObservableCollection<DiagnosisIcd10Items> temp = refIDC10List.Where(x => x.DiseasesReference != null).ToObservableCollection();
            if (temp != null && temp.Count > 0)
            {
                int bcount = 0;
                for (int i = 0; i < temp.Count; i++)
                {
                    if (temp[i].IsMain)
                    {
                        bcount++;
                        if (temp[i].DiseasesReference != null && temp[i].DiseasesReference.NotBeMain)
                        {
                            HasICDCannotBeMain = true;
                            NameOfICDCannotBeMain += " \n\t" + temp[i].DiseasesReference.ICD10Code + " - " + temp[i].DiseasesReference.DiseaseNameVN;
                        }
                    }
                }
                if (bcount == 0)
                {
                    Globals.ShowMessage(eHCMSResources.Z0509_G1_PhaiChonBenhChinh, eHCMSResources.G0442_G1_TBao);
                    return false;
                }
                if (HasICDCannotBeMain)
                {
                    Globals.ShowMessage(NameOfICDCannotBeMain, eHCMSResources.G0442_G1_TBao);
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
            //▲====: #013
        }

        private bool CheckedICD9IsMain()
        {
            ObservableCollection<DiagnosisICD9Items> temp = refICD9List.Where(x => x.RefICD9 != null).ToObservableCollection();
            if (temp != null && temp.Count > 0)
            {
                int bcount = temp.Where(x => x.IsMain).Count();

                if (bcount == 0)
                {
                    Globals.ShowMessage(eHCMSResources.Z1907_G1_ChonCachDTriChinh, eHCMSResources.G0442_G1_TBao);
                    return false;
                }
                else if (bcount == 1)
                {
                    return true;
                }
                else
                {
                    Globals.ShowMessage(eHCMSResources.Z1908_G1_NhieuHon1CachDTriChinh, eHCMSResources.G0442_G1_TBao);
                    return false;
                }
            }
            else
            {
                return true;
            }
        }

        AxDataGridNyICD10 grdConsultation { get; set; }
        public void grdConsultation_Loaded(object sender, RoutedEventArgs e)
        {
            grdConsultation = sender as AxDataGridNyICD10;
        }
        public void lnkDelete_Click(object sender, RoutedEventArgs e)
        {
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
                if (MessageBox.Show(eHCMSResources.A0202_G1_Msg_ConfXoaMaICD10, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    if (item.DiseasesReference != null
                        && item.DiseasesReference.DiseaseNameVN != "")
                    {
                        DiagTrmtItem.DiagnosisFinal = DiagTrmtItem.DiagnosisFinal.Replace(item.DiseasesReference.DiseaseNameVN, "");
                    }
                    //refIDC10List.RemoveAt(nSelIndex);
                    refIDC10List.Remove(refIDC10List[nSelIndex]);
                }
            }
        }

        private bool CheckCreateDiagnosis()
        {
            InPatientAdmDisDetails admission = Registration_DataStorage.CurrentPatientRegistration.AdmissionInfo;
            if (admission != null && admission.DischargeDate != null)
            {
                MessageBox.Show(eHCMSResources.Z0406_G1_BNDaXVKgTheCNhatCDoan, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return false;
            }
            if (Globals.IsLockRegistration(Registration_DataStorage.CurrentPatientRegistration.RegLockFlag, eHCMSResources.Z0407_G1_CNhatCDoan))
            {
                return false;
            }
            return true;
        }

        private bool CheckEditDiagnosis()
        {
            if (!CheckCreateDiagnosis())
            {
                return false;
            }
            else
            {
                //20190402 TTM: Nếu đang ở màn hình đề nghị nhập viện thì by pass bước kiểm tra này. 
                //              Lý do: Sẽ thay đổi nội trú không còn sử dụng PatientServiceRecord nữa. Nên không cần thiết kiểm tra làm gì.
                if (!this.IsForCollectDiagnosis)
                {
                    if (DiagTrmtItem == null || Registration_DataStorage.PatientServiceRecordCollection == null || Registration_DataStorage.PatientServiceRecordCollection.Count <= 0)
                    {
                        MessageBox.Show(eHCMSResources.A0731_G1_Msg_InfoKhTimThayCDHaySerRec, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                        return false;
                    }
                    if (DiagTrmtItem.DTItemID > 0 && DiagTrmtItem.ServiceRecID != Registration_DataStorage.PatientServiceRecordCollection[0].ServiceRecID)
                    {
                        MessageBox.Show(eHCMSResources.A0671_G1_Msg_InfoKhDcCNhatCDCuaDKCu, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                        return false;
                    }
                }
            }
            return true;
        }

        #region Các Button
        public void btEdit()
        {

            if (DiagTrmtItem == null || DiagTrmtItem.ServiceRecID.GetValueOrDefault() <= 0 || DiagTrmtItem.DTItemID <= 0)
            {
                MessageBox.Show(eHCMSResources.A0629_G1_Msg_InfoKhCoCDDeSua, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }

            if (!CheckEditDiagnosis())
            {
                return;
            }

            StateEditWaiting();
            DiagTrmtItemCopy = ObjectCopier.DeepCopy(DiagTrmtItem);
            /*▼====: #008*/
            //TBL: Lấy user đăng nhập nếu là BS
            gSelectedDoctorStaff = DoctorStaffs != null ? DoctorStaffs.FirstOrDefault(x => x.StaffID == Globals.LoggedUserAccount.StaffID) : null;
            /*▲====: #008*/
            CopyListICD10();
            CopyListICD9();

        }

        private bool CheckDiagnosisType()
        {
            if (DiagTrmtItem == null)
            {
                return false;
            }
            if (IsDiagnosisOutHospital)
            {
                if (DiagTrmtItem.V_DiagnosisType != (long)AllLookupValues.V_DiagnosisType.DIAGNOSIS_IN && DiagTrmtItem.V_DiagnosisType != (long)AllLookupValues.V_DiagnosisType.DIAGNOSIS_OUTHOS)
                {
                    MessageBox.Show(eHCMSResources.A0176_G1_Msg_InfoChiDcChonLoaiCDoanNhapXV, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                    return false;
                }
            }
            else if (IsProcedureEdit)
            {
                if (DiagTrmtItem.V_DiagnosisType != (long)AllLookupValues.V_DiagnosisType.Diagnosis_SmallProcedure)
                {
                    MessageBox.Show(eHCMSResources.Z2158_G1_Msg_InfoChiDcChonLoaiCDoanHangNgay, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                    return false;
                }
            }
            /*▼====: #003*/
            else if (IsDailyDiagnosis && !IsUpdateDiagConfirmInPT)
            {
                if (DiagTrmtItem.V_DiagnosisType != (long)AllLookupValues.V_DiagnosisType.DIAGNOSIS_DAILY)
                {
                    MessageBox.Show(eHCMSResources.Z2158_G1_Msg_InfoChiDcChonLoaiCDoanHangNgay, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                    return false;
                }
            }
            /*▲====: #003*/
            else if (!IsUpdateDiagConfirmInPT)
            {
                if (DiagTrmtItem.V_DiagnosisType != (long)AllLookupValues.V_DiagnosisType.DIAGNOSIS_OUTDEPT && DiagTrmtItem.V_DiagnosisType != (long)AllLookupValues.V_DiagnosisType.DIAGNOSIS_INDEPT)
                {
                    MessageBox.Show(eHCMSResources.A0177_G1_Msg_InfoChiDcChonLoaiCDoanXuatKhoa, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                    return false;
                }
            }
            return true;
        }

        // Hpt
        public InPatientDeptDetail GetCurDeptDetails(ObservableCollection<InPatientDeptDetail> CurDeptDetails)
        {
            if (!CurDeptDetails.Any(x => x.DeptLocation.RefDepartment.DeptID == DepartmentContent.SelectedItem.DeptID))
            {
                return null;
            }
            if (CurDeptDetails.Any(x => x.DeptLocation.RefDepartment.DeptID == DepartmentContent.SelectedItem.DeptID && x.DocTypeRequired == (long)AllLookupValues.V_DocTypeRequired.CD_XUAT_KHOA))
            {
                return CurDeptDetails.Where(x => x.DeptLocation.RefDepartment.DeptID == DepartmentContent.SelectedItem.DeptID && x.DocTypeRequired == (long)AllLookupValues.V_DocTypeRequired.CD_XUAT_KHOA).OrderBy(y => y.InPatientDeptDetailID).FirstOrDefault();
            }
            if (CurDeptDetails.Any(x => x.DeptLocation.RefDepartment.DeptID == DepartmentContent.SelectedItem.DeptID && x.InPtDeptGuid != null))
            {
                return CurDeptDetails.Where(x => x.DeptLocation.RefDepartment.DeptID == DepartmentContent.SelectedItem.DeptID && x.InPtDeptGuid != null).OrderBy(y => y.InPatientDeptDetailID).FirstOrDefault();
            }
            return CurDeptDetails.Where(x => x.DeptLocation.RefDepartment.DeptID == DepartmentContent.SelectedItem.DeptID).OrderBy(y => y.InPatientDeptDetailID).FirstOrDefault();

        }

        public bool CheckToGetGuid()
        {
            // Hpt 04/11/2015: Vì người tạo chẩn đoán (Bác sĩ) không quan tâm chẩn đoán đó là xuất khoa để chuyển đến khoa nào nên việc chọn Guid của đợt nhập khoa nào để lưu vào chẩn đoán được làm tự động theo thứ tự như sau:
            // 1. Ưu tiên các dòng nhập có đòi hỏi chẩn đoán xuất khoa làm trước(tra cứu từ bảng DeptTransferDocReq anh Tuấn đã tạo - đang để dữ liệu test)
            // 2. Nếu có nhiều dòng nhập cùng dòi hỏi chẩn đoán xuất khoa, chọn theo thứ tự thời gian phát sinh đòi hỏi đó.
            // 3. Các dòng không có đòi hỏi chẩn đoán vẫn được phép tạo, nhưng phải sau khi tất cả các đòi hỏi chẩn đoán xuất khoa của khoa đó đã được làm đủ thì mới tới những dòng không đòi hỏi (cũng của khoa đó)
            // 4. Mỗi đợt nhập vào khoa chỉ được tạo một chẩn đoán xuất khoa
            ObservableCollection<InPatientDeptDetail> DeptList = Registration_DataStorage.CurrentPatientRegistration.AdmissionInfo.InPatientDeptDetails.DeepCopy();
            PatientServiceRecord CurServiceRec = Registration_DataStorage.PatientServiceRecordCollection.Where(x => x.PtRegistrationID == Registration_DataStorage.CurrentPatientRegistration.PtRegistrationID).FirstOrDefault();
            InPatientDeptDetail CurInPtDeptDetail = new InPatientDeptDetail();

            CurInPtDeptDetail = GetCurDeptDetails(DeptList);
            if (CurInPtDeptDetail == null)
            {
                MessageBox.Show(eHCMSResources.A0214_G1_Msg_InfoBNChuaNhapVaoKhoa + DepartmentContent.SelectedItem.DeptName + string.Format(". {0}", eHCMSResources.A0215_G1_Msg_InfoKhTheTaoCD), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return false;
            }
            while (DeptList.Count > 0 && CurServiceRec.DiagnosisTreatments.Any(x => x.InPtDeptGuid == CurInPtDeptDetail.InPtDeptGuid && x.V_DiagnosisType == (long)AllLookupValues.V_DiagnosisType.DIAGNOSIS_OUTDEPT)) // trừ dòng cuối, dòng cuối cho làm bao nhiêu cũng được
            {
                if (CurInPtDeptDetail.InPtDeptGuid == null)
                {
                    DiagTrmtItem.InPtDeptGuid = null;
                    return true;
                }
                DeptList.Remove(CurInPtDeptDetail);
                CurInPtDeptDetail = GetCurDeptDetails(DeptList);
                if (CurInPtDeptDetail == null)
                {
                    MessageBox.Show(DepartmentContent.SelectedItem.DeptName + string.Format(" {0}", eHCMSResources.K3310_G1_I), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                    return false;
                }
            }
            DiagTrmtItem.InPtDeptGuid = CurInPtDeptDetail.InPtDeptGuid;
            return true;
        }

        private bool CheckDepartment()
        {
            if (DiagTrmtItem == null || DiagTrmtItem.Department == null || DiagTrmtItem.Department.DeptID <= 0)
            {
                MessageBox.Show(eHCMSResources.Z0493_G1_HayChonKhoa, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return false;
            }

            InPatientAdmDisDetails admission = Registration_DataStorage.CurrentPatientRegistration.AdmissionInfo;

            if (DiagTrmtItem.V_DiagnosisType == (long)AllLookupValues.V_DiagnosisType.DIAGNOSIS_IN)
            {
                if (!this.IsForCollectDiagnosis && DiagTrmtItem.Department.DeptID != admission.Department.DeptID)
                {
                    MessageBox.Show(string.Format("{0} ", eHCMSResources.A0284_G1_Msg_InfoChonKhoaNpVien) + admission.Department.DeptName + string.Format(" {0}", eHCMSResources.A0285_G1_Msg_InfoViBNNpVienVaoKhoaNay), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                    return false;
                }
            }
            else if (DiagTrmtItem.V_DiagnosisType == (long)AllLookupValues.V_DiagnosisType.DIAGNOSIS_OUTHOS)
            {
                int CountInDept = 0;

                //Đếm số lần nhập khoa.
                if (admission.InPatientDeptDetails != null && admission.InPatientDeptDetails.Count > 0)
                {
                    CountInDept = admission.InPatientDeptDetails.Where(x => x.DeptLocation.DeptID == DiagTrmtItem.Department.DeptID).Count();
                }
                if (CountInDept <= 0)
                {
                    MessageBox.Show(eHCMSResources.A0216_G1_Msg_InfoBNChuaNhapVaoKhoaBanChon + " (" + DiagTrmtItem.Department.DeptName + ") ! ", eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                    return false;
                }
            }
            /*▼====: #003*/
            else if (/*DiagTrmtItem.V_DiagnosisType == (long)AllLookupValues.V_DiagnosisType.DIAGNOSIS_DAILY ||*/ DiagTrmtItem.V_DiagnosisType == (long)AllLookupValues.V_DiagnosisType.Diagnosis_SmallProcedure)
            {
                if (admission == null || admission.InPatientDeptDetails == null || admission.InPatientDeptDetails.Count() <= 0)
                {
                    return false;
                }
                if (!admission.InPatientDeptDetails.Any(x => x.DeptLocation.DeptID == DiagTrmtItem.Department.DeptID))
                {
                    MessageBox.Show(eHCMSResources.A0216_G1_Msg_InfoBNChuaNhapVaoKhoaBanChon + " (" + DiagTrmtItem.Department.DeptName + ") ! ", eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                    return false;
                }
            }
            /*▲====: #003*/
            else if (DiagTrmtItem.V_DiagnosisType == (long)AllLookupValues.V_DiagnosisType.DIAGNOSIS_OUTDEPT)
            {
                if (admission == null || admission.InPatientDeptDetails == null || admission.InPatientDeptDetails.Count() <= 0)
                {
                    return false;
                }
                //HPT 26/09/2016: Khi đếm những dòng nhập khoa đã có chẩn đoán xuất khoa , phải trừ dòng nhập khoa nào gắn với chẩn đoán xuất khoa đang hiệu chỉnh. Nếu không sẽ không hiệu chỉnh được
                /*Ví dụ:
                 * Nhập khoa A hai lần, tạo hai chẩn đoán có GUID1 và GUID2 ứng với hai lần nhập
                 * Cập nhật chẩn đoán GUID1, đếm số dòng nhập khoa đã có chẩn đoán, nếu không trừ dòng có GUID1 ra thì sẽ thấy có hai dòng nhập khoa đều đã có chẩn đoán --> không cho lưu cập nhật
                 */
                if (!admission.InPatientDeptDetails.Any(x => x.DeptLocation.DeptID == DiagTrmtItem.Department.DeptID))
                {
                    MessageBox.Show(eHCMSResources.A0216_G1_Msg_InfoBNChuaNhapVaoKhoaBanChon + " (" + DiagTrmtItem.Department.DeptName + ") ! ", eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                    return false;
                }
                if (DiagTrmtItem.DTItemID <= 0 && !admission.InPatientDeptDetails.Any(x => x.DeptLocation.DeptID == DiagTrmtItem.Department.DeptID && x.CompletedRequiredFromDate == null))
                {
                    MessageBox.Show(eHCMSResources.A0222_G1_Msg_InfoBNCoDuCDXK + " " + DiagTrmtItem.Department.DeptName + string.Format("\n {0}", eHCMSResources.Z0408_G1_KgTheTaoThemCDoanXK), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                    return false;
                }
            }

            return true;
        }

        public void btUpdate()
        {
            Coroutine.BeginExecute(AllCheck(null));
        }

        public void btCancel()
        {
            //▼===== 20191112 TTM:  Kiểm tra trước khi thực hiện vì thằng DiagTrmtItemCopy sẽ bằng null khi btCancel được gọi từ ConsultationDoubleClickEvent_InPt_2 event.
            //                      và UpdateDiagTrmtItemIntoLayout thực hiện giống như btcancel khi load vào các biến DiagTrmtItem, refIDC10List, refICD9List
            if (DiagTrmtItemCopy == null)
            {
                return;
            }
            //▲===== 
            DiagTrmtItem = ObjectCopier.DeepCopy(DiagTrmtItemCopy);
            /*▼====: #008*/
            //TBL: Khi hủy bỏ chỉnh sửa thì lấy lại BS ra chẩn đoán trước khi sửa
            gSelectedDoctorStaff = DiagTrmtItem.ObjDoctorStaffID;
            /*▲====: #008*/
            refIDC10List = refIDC10ListCopy;
            refICD9List = refICD9ListCopy;


            switch (ConsultState)
            {
                case ConsultationState.NewConsultationState:
                    StateNew(); break;
                case ConsultationState.EditConsultationState:
                    StateEdit(); break;
                case ConsultationState.NewAndEditConsultationState:
                    StateNewEdit(); break;
            }
        }

        // TxD 12/102015: Check for empty fields as requested by Dr. Bang
        private bool CheckEmptyFields()
        {
            string strWarningMsg = "";

            if (DiagTrmtItem.DiagnosisFinal == null || DiagTrmtItem.DiagnosisFinal.Trim() == "")
            {
                strWarningMsg += string.Format("{0} - ", eHCMSResources.K1775_G1_CDoanXDinh2);
            }
            if (DiagTrmtItem.Diagnosis == null || DiagTrmtItem.Diagnosis.Trim() == "")
            {
                strWarningMsg += string.Format("{0} - ", eHCMSResources.G1785_G1_TrieuChungDHieuLS);
            }

            // Hpt 20/11/2015: Giờ có yêu cầu bỏ ra nên comment lại đây, khi nào cần thì mở ra
            //if (DiagTrmtItem.OrientedTreatment == null || DiagTrmtItem.OrientedTreatment.Trim() == "")
            //{
            //    strWarningMsg += string.Format("{0} - ", eHCMSResources.Z3309_G1_DienBienBenh);
            //}
            if (DiagTrmtItem.Treatment == null || DiagTrmtItem.Treatment.Trim() == "")
            {
                strWarningMsg += string.Format("{0} - ", eHCMSResources.Z0021_G1_CachDTri);
            }
            if (strWarningMsg != "")
            {
                MessageBox.Show(eHCMSResources.A0201_G1_Msg_InfoYCNhapSth + ": " + strWarningMsg);
                return false;
            }
            return true;
        }

        MessageWarningShowDialogTask warningtask = null;
        string content = eHCMSResources.Z0420_G1_CDoanCoICDLaZ10;
        // Hpt 22/09/2015: Thêm hàm coroutine SaveNewDiagnosis ở đây, cắt thân hàm btSaveCreateNew bỏ vào, để có thể sử dụng được đối tượng MessageWarningShowDialogTask hiển thị cảnh báo nếu có ICD10 là Z10
        // Thân hàm btSaveCreateNew thay bằng code gọi đến coroutine SaveNewDiagnosis

        private bool _IsRequireConfirmZ10 = true;
        public bool IsRequireConfirmZ10
        {
            get
            {
                return _IsRequireConfirmZ10;
            }
            set
            {
                _IsRequireConfirmZ10 = value;
                NotifyOfPropertyChange(() => IsRequireConfirmZ10);
            }
        }

        private IEnumerator<IResult> SaveNewDiagnosis(SmallProcedure aSmallProcedure, List<Resources> resourceList = null)
        {
            if (refIDC10List != null && refIDC10List.Any(x => x.IsInvalid))
            {
                MessageBox.Show(string.Format(eHCMSResources.Z2205_G1_ICD10KhongHopLe, string.Join(",", refIDC10List.Where(x => x.IsInvalid).Select(x => x.ICD10Code).ToList())), eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                yield break;
            }
            if (!this.IsForCollectDiagnosis && !CheckCreateDiagnosis())
            {
                yield break;
            }
            if (!CheckDiagnosisType())
            {
                yield break;
            }
            if (!CheckDepartment())
            {
                yield break;
            }
            // HPT 26/09/2016: Đổi luật đối với chẩn đoán xuất khoa. Trước đây phải lấy Guid từ dòng nhập bỏ vào chẩn đoán nên mới làm như vầy. 
            // Theo luật mới, Guid sẽ lấy từ chẩn đoán bỏ vào dòng nhập nên không cần code này nữa. Comment lại nếu test chạy ổn sẽ bỏ đoạn code này + hàm CheckToGetGuid() và hàm GetCurDeptDetails() đi luôn
            //if (DiagTrmtItem.V_DiagnosisType == (long)AllLookupValues.V_DiagnosisType.DIAGNOSIS_OUTDEPT)
            //{
            //    if (!CheckToGetGuid())
            //    {
            //        yield break;
            //    }
            //}
            if (!CheckEmptyFields())
            {
                yield break;
            }
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
            /*▼====: #008*/
            //DiagTrmtItem.PatientServiceRecord.Staff = Globals.LoggedUserAccount.Staff;
            //DiagTrmtItem.PatientServiceRecord.StaffID = Globals.LoggedUserAccount.Staff.StaffID;
            if (gSelectedDoctorStaff == null || gSelectedDoctorStaff.StaffID == 0)
            {
                MessageBox.Show(eHCMSResources.A0376_G1_Msg_InfoChuaChonBS, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                yield break;
            }

            DiagTrmtItem.PatientServiceRecord.Staff = gSelectedDoctorStaff;
            DiagTrmtItem.PatientServiceRecord.StaffID = gSelectedDoctorStaff.StaffID;
            /*▲====: #008*/
            if (IsAdmRequest && String.IsNullOrEmpty(DiagTrmtItem.ReasonHospitalStay))
            {
                MessageBox.Show("Chưa nhập lý do vào nội trú", eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                yield break;
            }
            if (IsAdmRequest && !CheckAdmissionCriteria())
            {
                MessageBox.Show("Chưa chọn tiêu chí nhập viện", eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                yield break;
            }
            if (IsConsultationKhoaSanEdit && SoConChet > 0 && NgayConChet == null)
            {
                MessageBox.Show("Người dùng chưa chọn Ngày con chết. Vui lòng kiểm tra lại", eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                yield break;
            }
            if (IsAdmRequest && String.IsNullOrWhiteSpace(DiagTrmtItem.OrientedTreatment))
            {
                MessageBox.Show("Trường quá trình bệnh lý bắt buộc nhập, yêu cầu kiểm tra lại", eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                yield break;
            }
            if (CheckedIsMain() && NeedICD10() && CheckedICD9IsMain())
            {
                //Khám DV cụ thể nào
                if (Registration_DataStorage.CurrentPatientRegistrationDetail != null && Registration_DataStorage.CurrentPatientRegistrationDetail.PtRegDetailID > 0)
                {
                    DiagTrmtItem.PtRegDetailID = Registration_DataStorage.CurrentPatientRegistrationDetail.PtRegDetailID;
                }
                else if (aSmallProcedure != null && aSmallProcedure.PtRegDetailID > 0)
                {
                    DiagTrmtItem.PtRegDetailID = aSmallProcedure.PtRegDetailID;
                }
                else
                {
                    DiagTrmtItem.PtRegDetailID = 0;
                }

                DiagnosisIcd10Items MainICD = refIDC10List.Where(x => x.IsMain && x.ICD10Code.Contains("†")).FirstOrDefault();
                if (MainICD != null)
                {
                    //▼====: #015
                    if (!CheckRequireSubICd(MainICD) || !CheckRuleICd())
                    {
                        yield break;
                    }
                    //▲====: #015
                }

                //Khám DV cụ thể nào
                DiagTrmtItem.PatientServiceRecord.PatientMedicalRecord.PatientID = Registration_DataStorage.CurrentPatient.PatientID;
                DiagTrmtItem.PatientServiceRecord.StaffID = gSelectedDoctorStaff.StaffID;
                DiagTrmtItem.PatientServiceRecord.PtRegistrationID = Registration_DataStorage.CurrentPatientRegistration.PtRegistrationID;
                DiagTrmtItem.DeptLocationID = Globals.DeptLocation.DeptLocationID;
                DiagTrmtItem.PatientServiceRecord.V_RegistrationType = Registration_DataStorage.CurrentPatientRegistration.V_RegistrationType;
                //KMx: Loại đăng ký phải dựa trên ĐK của BN, không dựa vào tiêu chí tìm kiếm đăng ký.                
                DiagTrmtItem.ICD10List = String.Join(",", from item in refIDC10List
                                                          where (!string.IsNullOrEmpty(item.ICD10Code) && item.DiseasesReference != null)
                                                          select item.ICD10Code);
                PatientRegistration CurRegistration = Registration_DataStorage.CurrentPatientRegistration;
                // Hpt 22/09/2015: Nếu bệnh nhân có quyền lợi về BHYT thì mới kiểm tra xem trong danh sách có ICD10 nào là Z10 hay không vì BHYT sẽ không chi trả cho những trường hợp này
                if (IsRequireConfirmZ10 && CurRegistration.PtInsuranceBenefit != null && CurRegistration.PtInsuranceBenefit > 0 && refIDC10List != null
                    && refIDC10List.Any(x => x.ICD10Code != null && x.DiseasesReference != null && x.DiseasesReference.ICD10Code.Contains("Z10")))
                {
                    warningtask = new MessageWarningShowDialogTask(content, eHCMSResources.Z0339_G1_TiepTucLuuCDoan);
                    yield return warningtask;
                    if (!warningtask.IsAccept)
                    {
                        IsRequireConfirmZ10 = true;
                        yield break;
                    }
                    IsRequireConfirmZ10 = false;
                }
                // Nếu là đăng ký không có BHYT thì cho lưu luôn không cần kiểm tra gì
                //else
                //{
                //    AddNewDiagTrmt();
                //}
                if (DiagTrmtItem.V_DiagnosisType == (long)AllLookupValues.V_DiagnosisType.DIAGNOSIS_OUTDEPT)
                {
                    string confirmcontent = string.Format(eHCMSResources.K1006_G1_BanDangTaoCDXKhoaCho, DiagTrmtItem.Department.DeptName) + string.Format("\n{0}!", eHCMSResources.Z0538_G1_KgThayDoiKhoaSauKhiLuuCDoanXK);
                    warningtask = new MessageWarningShowDialogTask(confirmcontent, eHCMSResources.Z0339_G1_TiepTucLuuCDoan);
                    yield return warningtask;
                    if (!warningtask.IsAccept)
                    {
                        yield break;
                    }
                }

                AddNewDiagTrmt(aSmallProcedure, resourceList);
                if (IsConsultationKhoaSanEdit)
                {
                    AddNewDiagDetal_KhoaSan();
                }
            }
        }

        public void btSaveCreateNew()
        {
            Coroutine.BeginExecute(SaveNewDiagnosis(null));
        }
        public void SaveCreateNew(SmallProcedure aSmallProcedure, List<Resources> resourceList)
        {
            if (DiagTrmtItem != null && DiagTrmtItem.DTItemID > 0)
            {
                Coroutine.BeginExecute(AllCheck(aSmallProcedure, resourceList));
            }
            else
            {
                Coroutine.BeginExecute(SaveNewDiagnosis(aSmallProcedure, resourceList));
            }
        }
        public void btCreateNew()
        {
            if (!CheckCreateDiagnosis())
            {
                return;
            }
            //FormEditorIsEnabled = true;

            //ButtonForNotDiag(true);
            StateNewWaiting();

            DiagTrmtItemCopy = ObjectCopier.DeepCopy(DiagTrmtItem);
            /*▼====: #008*/
            //TBL: Khi tạo mới lấy user đăng nhập nếu là BS
            gSelectedDoctorStaff = DoctorStaffs != null ? DoctorStaffs.FirstOrDefault(x => x.StaffID == Globals.LoggedUserAccount.StaffID) : null;
            /*▲====: #008*/
            //KMx: Không cần gọi về server, chỉ cần sử dụng hàm ResetDefaultForBehaving() trong ruột GetBlankDiagnosisTreatmentByPtID() (01/11/2014 10:16).
            //GetBlankDiagnosisTreatmentByPtID(Registration_DataStorage.CurrentPatient.PatientID);
            DiagTrmtItem = new DiagnosisTreatment();
            ResetDefaultForBehaving();

            CopyListICD10ForNew();
            CopyListICD9ForNew();
        }

        public void SetDepartment()
        {
            if (DiagTrmtItem == null || DepartmentContent == null || DepartmentContent.Departments == null)
            {
                return;
            }

            if (_DiagTrmtItem.Department != null && _DiagTrmtItem.Department.DeptID > 0)
            {
                DepartmentContent.SelectedItem = _DiagTrmtItem.Department;
            }
            else
            {
                if (Globals.ObjRefDepartment != null && Globals.ObjRefDepartment.DeptID > 0 && DepartmentContent.Departments.Any(x => x.DeptID == Globals.ObjRefDepartment.DeptID))
                {
                    DepartmentContent.SelectedItem = DepartmentContent.Departments.Where(x => x.DeptID == Globals.ObjRefDepartment.DeptID).FirstOrDefault();
                }
                else
                {
                    //DepartmentContent.SelectedItem = DepartmentContent.Departments != null ? DepartmentContent.Departments.FirstOrDefault() : null;
                    DepartmentContent.SelectedItem = DepartmentContent.Departments.FirstOrDefault();
                }
            }
            //▼===== #007: Sửa thêm 1 lần nữa theo ý anh Tuân ngày 20191031: Nếu như chưa có đề nghị nhập khoa thì luôn mặc định ở đề nghị là hãy chọn khoa
            if (IsAdmRequest && DiagTrmtItem.DTItemID == 0)
            {
                DepartmentContent.SelectedItem = DepartmentContent.Departments.FirstOrDefault();
            }
            else
            {
                DiagTrmtItem.Department = DepartmentContent.SelectedItem;
                //DepartmentContent.SelectedItem.IsTreatmentForCOVID)
                if (IsAdmRequest && DiagTrmtItem.Department.IsTreatmentForCOVID)
                {
                    IsShowCheckCOVID = true;
                    DiagTrmtItem.IsTreatmentCOVID = true;
                }
                else
                {
                    IsShowCheckCOVID = false;
                    DiagTrmtItem.IsTreatmentCOVID = false;
                }
            }
            //▲===== #007
        }

        public void btSaveNewByOld()
        {
            if (!CheckCreateDiagnosis())
            {
                return;
            }
            //▼===== #011
            CheckListICD10Code();
            //▲===== #011
            StateNewWaiting();
            if (Globals.ServerConfigSection.CommonItems.ApplyOtherDiagnosis)
            {
                if (Registration_DataStorage != null && Registration_DataStorage.CurrentPatientRegistration != null
                    && Registration_DataStorage.CurrentPatientRegistration.AdmissionInfo != null
                    && Registration_DataStorage.CurrentPatientRegistration.AdmissionInfo.IsTreatmentCOVID
                    && !Globals.ServerConfigSection.InRegisElements.AllowEditDiagnosisFinalForPatientCOVID)
                {
                    DiagTrmtItem.DiagnosisFinal = String.Join("; ", from item in refIDC10List
                                                                    where (!string.IsNullOrEmpty(item.ICD10Code) && item.DiseasesReference != null)
                                                                    select item.DiseasesReference.DiseaseNameVN);
                }
            }
            DiagTrmtItemCopy = ObjectCopier.DeepCopy(DiagTrmtItem);
            /*▼====: #008*/
            //TBL: Khi tạo mới dựa trên cũ lấy user đăng nhập nếu là BS
            gSelectedDoctorStaff = DoctorStaffs != null ? DoctorStaffs.FirstOrDefault(x => x.StaffID == Globals.LoggedUserAccount.StaffID) : null;
            /*▲====: #008*/
            //KMx: Khi tạo mới dựa trên chẩn đoán cũ, thì phải set DTItemID của chẩn đoán mới = 0. Để hàm CheckDepartment() biết đây là tạo mới, không phải cập nhật (10/06/2015 17:47).
            //KMx: Khi tạo mới dựa trên chẩn đoán cũ, bắt người dùng chọn Khoa để không bị lộn (03/08/2015 16:45).
            if (DiagTrmtItem != null)
            {
                DiagTrmtItem.DTItemID = 0;
                DiagTrmtItem.DiagnosisDate = Globals.GetCurServerDateTime();
                DiagTrmtItem.IntPtDiagDrInstructionID = 0; //20191120 TBL: BM 0019612: Nếu tạo mới dựa trên cũ của chẩn đoán y lệnh thì set lại IntPtDiagDrInstructionID = 0 để chẩn đoán này không phải là chẩn đoán của y lệnh
                DiagTrmtItem.Department = new RefDepartment();
                SetDepartment();
            }

            SetDefaultDiagnosisType();

            CopyListICD10();
            gICD10Changed?.Invoke(refIDC10List);
            CopyListICD9();
        }

        private void AddNewDiagTrmt(SmallProcedure aSmallProcedure, List<Resources> resourceList = null)
        {
            if (this.IsForCollectDiagnosis)
            {
                Globals.EventAggregator.Publish(new Icd10CollectionSelected { Icd10Items = refIDC10List, DiagnosisTreatment = DiagTrmtItem });
                TryClose();
                return;
            }
            this.ShowBusyIndicator();
            IsWaitingSaveAddNew = true;
            long ID = Compare2Object();
            long DiagnosisICD9ListID = Compare2ICD9List();
            if (aSmallProcedure != null && aSmallProcedure.PtRegDetailID == 0)
            {
                aSmallProcedure.PtRegDetailID = DiagTrmtItem.PtRegDetailID.GetValueOrDefault(0);
            }
            var t = new Thread(() =>
            {
                using (var serviceFactory = new ePMRsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginAddDiagnosisTreatment_InPt_V2(DiagTrmtItem, ID, refIDC10List, DiagnosisICD9ListID, refICD9List
                    //▼===== #018
                    , resourceList
                    //▲===== #018
                    , aSmallProcedure, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            List<InPatientDeptDetail> ReloadInPatientDeptDetails = new List<InPatientDeptDetail>();
                            long SmallProcedureID;
                            if (contract.EndAddDiagnosisTreatment_InPt_V2(out ReloadInPatientDeptDetails, out SmallProcedureID, asyncResult))
                            {
                                if (aSmallProcedure != null)
                                {
                                    aSmallProcedure.SmallProcedureID = SmallProcedureID;
                                }
                                StateNewEdit();
                                if (refIDC10List != null)
                                {
                                    refIDC10List = refIDC10List.Where(x => x.DiseasesReference != null).ToObservableCollection();
                                }
                                if (refICD9List != null)
                                {
                                    refICD9List = refICD9List.Where(x => x.RefICD9 != null).ToObservableCollection();
                                }
                                Registration_DataStorage.CurrentPatientRegistration.AdmissionInfo.InPatientDeptDetails = new ObservableCollection<InPatientDeptDetail>(ReloadInPatientDeptDetails);
                                //phat su kien reload lai danh sach 
                                //Globals.EventAggregator.Publish(new ReloadDataConsultationEvent { });
                                //KMx: Sau khi lưu chẩn đoán, reload Service Record (22/05/2014 09:48).
                                IConsultationModule consultVM = Globals.GetViewModel<IConsultationModule>();
                                if (consultVM.MainContent is IConsultationsSummary_InPt)
                                {
                                    (consultVM.MainContent as IConsultationsSummary_InPt).CallSetInPatientInfoForConsultation(Registration_DataStorage.CurrentPatientRegistration, Registration_DataStorage.CurrentPatientRegistrationDetail);
                                }
                                else if (consultVM.MainContent is IConsultations_InPt) //20200410 TBL: Nếu không load lại thì không thể hiệu chỉnh khi đã lưu thành công
                                {
                                    (consultVM.MainContent as IConsultations_InPt).CallSetInPatientInfoForConsultation(Registration_DataStorage.CurrentPatientRegistration, Registration_DataStorage.CurrentPatientRegistrationDetail);
                                }

                                //▼===== 20190930: IConsultationModule sẽ bị loại bỏ, hiện tại hàm PatientServiceRecordsGetForKhamBenh_Ext không còn sử dụng nữa.
                                //else
                                //{
                                //    consultVM.PatientServiceRecordsGetForKhamBenh_Ext();
                                //}
                                //▲===== 20190930

                                //KMx: Sau khi lưu chẩn đoán, xóa danh sách chẩn đoán. Vì danh sách không tự động load lại.
                                //Nếu không xóa thì chẩn đoán vừa lưu sẽ khác với chẩn đoán trong danh sách (22/05/2014 09:48).
                                Globals.EventAggregator.Publish(new ClearAllDiagnosisListAfterAddNewEvent_InPt());
                                MessageBox.Show(eHCMSResources.A0468_G1_Msg_InfoLuuOK, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                if (IsProcedureEdit) //20200410 TBL: Sau khi lưu thành công thì set ConsultState để chỉ có thể hiệu chỉnh
                                {
                                    ConsultState = ConsultationState.EditConsultationState;
                                }
                            }
                            else
                            {
                                //KMx: Dời V_DiagnosisType từ PatientServiceRecord sang DiagnosisTreatment (09/06/2015 17:42).
                                //if (DiagTrmtItem.PatientServiceRecord.V_DiagnosisType == (long)AllLookupValues.V_DiagnosisType.DIAGNOSIS_IN)
                                if (DiagTrmtItem.V_DiagnosisType == (long)AllLookupValues.V_DiagnosisType.DIAGNOSIS_IN)
                                {
                                    MessageBox.Show(eHCMSResources.Z0409_G1_DaCoCDoanNV, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                }
                                //else if (DiagTrmtItem.PatientServiceRecord.V_DiagnosisType == (long)AllLookupValues.V_DiagnosisType.DIAGNOSIS_OUTHOS)
                                else if (DiagTrmtItem.V_DiagnosisType == (long)AllLookupValues.V_DiagnosisType.DIAGNOSIS_OUTHOS)
                                {
                                    MessageBox.Show(eHCMSResources.Z0410_G1_DaCoCDoanXV, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                }
                                else
                                {
                                    MessageBox.Show(eHCMSResources.A0802_G1_Msg_InfoLuuCDFail, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                            if (ex.Message.Contains("[ERROR-TBL]"))
                            {
                                ClientLoggerHelper.LogError("ConsultationsSummary_V2ViewModel AddNewDiagTrmt - " + Globals.GetCurServerDateTime() + " -  [" + Globals.LoggedUserAccount.StaffID + " - "
                                + Globals.LoggedUserAccount.Staff.FullName + "] - " + "PatientID: " + (Registration_DataStorage != null && Registration_DataStorage.CurrentPatient != null ? Registration_DataStorage.CurrentPatient.PatientID : 0).ToString()
                                + " PtRegistrationID: " + (Registration_DataStorage != null && Registration_DataStorage.CurrentPatientRegistration != null ? Registration_DataStorage.CurrentPatientRegistration.PtRegistrationID : 0).ToString()
                                + " PtRegDetailID: " + (aSmallProcedure != null ? aSmallProcedure.PtRegDetailID : 0).ToString()
                                + " SmallProcedureID: " + (aSmallProcedure != null ? aSmallProcedure.SmallProcedureID : 0).ToString() + ex.Message);
                            }
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

            LoadRefBehaving_MedRecTemplate();
        }

        #endregion


        #region button control

        public enum ConsultationState
        {
            //Tao moi chan doan
            NewConsultationState = 1,
            //Hieu chinh chan doan
            EditConsultationState = 2,
            //tao moi va chinh sua chan doan cua noi tru
            NewAndEditConsultationState = 3,

        }


        private ConsultationState _ConsultState = ConsultationState.NewConsultationState;
        public ConsultationState ConsultState
        {
            get
            {
                return _ConsultState;
            }
            set
            {

                _ConsultState = value;
                NotifyOfPropertyChange(() => ConsultState);
                switch (ConsultState)
                {
                    case ConsultationState.NewConsultationState:
                        mNewConsultationState = true;
                        mEditConsultationState = false;
                        break;
                    case ConsultationState.EditConsultationState:
                        mNewConsultationState = false;
                        mEditConsultationState = true;
                        break;
                    case ConsultationState.NewAndEditConsultationState:
                        mNewConsultationState = true;
                        mEditConsultationState = true;
                        break;
                }

            }
        }

        private bool _mNewConsultationState;
        public bool mNewConsultationState
        {
            get
            {
                return _mNewConsultationState;
            }
            set
            {
                if (_mNewConsultationState != value)
                {
                    _mNewConsultationState = value;
                    NotifyOfPropertyChange(() => mNewConsultationState);
                }
            }
        }

        private bool _mEditConsultationState;
        public bool mEditConsultationState
        {
            get
            {
                return _mEditConsultationState;
            }
            set
            {
                if (_mEditConsultationState != value)
                {
                    _mEditConsultationState = value;
                    NotifyOfPropertyChange(() => mEditConsultationState);
                }
            }
        }

        public void StateNew()
        {
            btCreateNewIsEnabled = true;
            btCreateNewByOldIsEnabled = true;
            btSaveCreateNewIsEnabled = false;
            btCancelIsEnabled = false;
            FormEditorIsEnabled = false;

            NotifyOfPropertyChange(() => btCreateNewIsEnabled);
            NotifyOfPropertyChange(() => btCreateNewByOldIsEnabled);
            NotifyOfPropertyChange(() => btSaveCreateNewIsEnabled);
            NotifyOfPropertyChange(() => btCancelIsEnabled);
            NotifyOfPropertyChange(() => FormEditorIsEnabled);

        }

        public void StateNewEdit()
        {
            //▼===== 20191012 TTM:  Vì lý do đề nghị nhập viện không được phép tạo mới hoặc tạo mới dựa trên cũ (Vì thông tin được lấy từ chẩn đoán ngoại trú)
            //                      Nên 2 nút này không được phép visible khi bấm nút bỏ qua.    
            if (!IsAdmRequest)
            {
                btCreateNewIsEnabled = true;
                btCreateNewByOldIsEnabled = true;
            }
            //▲===== 
            btEditIsEnabled = true;
            btUpdateIsEnabled = false;
            btSaveCreateNewIsEnabled = false;
            btCancelIsEnabled = false;
            FormEditorIsEnabled = false;

            NotifyOfPropertyChange(() => btCreateNewIsEnabled);
            NotifyOfPropertyChange(() => btCreateNewByOldIsEnabled);
            NotifyOfPropertyChange(() => btSaveCreateNewIsEnabled);
            NotifyOfPropertyChange(() => btCancelIsEnabled);
            NotifyOfPropertyChange(() => FormEditorIsEnabled);
        }

        public void StateNewWaiting()
        {
            btCreateNewIsEnabled = false;
            btCreateNewByOldIsEnabled = false;
            btSaveCreateNewIsEnabled = true;
            btCancelIsEnabled = true;
            FormEditorIsEnabled = true;

            btEditIsEnabled = false;
            //▼====: #013
            if (Globals.ServerConfigSection.CommonItems.ApplyOtherDiagnosis)
            {
                if (Registration_DataStorage != null && Registration_DataStorage.CurrentPatientRegistration != null
                    && Registration_DataStorage.CurrentPatientRegistration.AdmissionInfo != null
                    && Registration_DataStorage.CurrentPatientRegistration.AdmissionInfo.IsTreatmentCOVID
                    && Globals.ServerConfigSection.InRegisElements.AllowEditDiagnosisFinalForPatientCOVID)
                {
                    DiagnosisFinalIsEnabled = true;
                }
                else
                {
                    DiagnosisFinalIsEnabled = false;
                }
            }
            else
            {
                DiagnosisFinalIsEnabled = true;
            }
            NotifyOfPropertyChange(() => DiagnosisFinalIsEnabled);
            //▲====: #013

            NotifyOfPropertyChange(() => btCreateNewIsEnabled);
            NotifyOfPropertyChange(() => btCreateNewByOldIsEnabled);
            NotifyOfPropertyChange(() => btSaveCreateNewIsEnabled);
            NotifyOfPropertyChange(() => btCancelIsEnabled);
            NotifyOfPropertyChange(() => FormEditorIsEnabled);
        }

        private void StatePopupNewWaiting()
        {
            btCreateNewIsEnabled = false;
            btCreateNewByOldIsEnabled = false;
            btSaveCreateNewIsEnabled = true;
            btCancelIsEnabled = false;
            FormEditorIsEnabled = true;

            btEditIsEnabled = false;

            NotifyOfPropertyChange(() => btCreateNewIsEnabled);
            NotifyOfPropertyChange(() => btCreateNewByOldIsEnabled);
            NotifyOfPropertyChange(() => btSaveCreateNewIsEnabled);
            NotifyOfPropertyChange(() => btCancelIsEnabled);
            NotifyOfPropertyChange(() => FormEditorIsEnabled);
        }

        public void StateEdit()
        {
            btEditIsEnabled = true;
            btUpdateIsEnabled = false;
            btCancelIsEnabled = false;
            FormEditorIsEnabled = false;

            NotifyOfPropertyChange(() => btEditIsEnabled);
            NotifyOfPropertyChange(() => btUpdateIsEnabled);
            NotifyOfPropertyChange(() => btCancelIsEnabled);
            NotifyOfPropertyChange(() => FormEditorIsEnabled);
        }

        public void StateEditWaiting()
        {
            btEditIsEnabled = false;
            btUpdateIsEnabled = true;
            btCancelIsEnabled = true;
            FormEditorIsEnabled = true;
            btCreateNewIsEnabled = false;
            btCreateNewByOldIsEnabled = false;
            //▼====: #013
            if (Globals.ServerConfigSection.CommonItems.ApplyOtherDiagnosis)
            {
                if (Registration_DataStorage != null && Registration_DataStorage.CurrentPatientRegistration != null
                    && Registration_DataStorage.CurrentPatientRegistration.AdmissionInfo != null
                    && Registration_DataStorage.CurrentPatientRegistration.AdmissionInfo.IsTreatmentCOVID
                    && Globals.ServerConfigSection.InRegisElements.AllowEditDiagnosisFinalForPatientCOVID)
                {
                    DiagnosisFinalIsEnabled = true;
                }
                else
                {
                    DiagnosisFinalIsEnabled = false;
                }
            }
            else
            {
                DiagnosisFinalIsEnabled = true;
            }
            NotifyOfPropertyChange(() => DiagnosisFinalIsEnabled);
            //▲====: #013

            NotifyOfPropertyChange(() => btEditIsEnabled);
            NotifyOfPropertyChange(() => btUpdateIsEnabled);
            NotifyOfPropertyChange(() => btCancelIsEnabled);
            NotifyOfPropertyChange(() => FormEditorIsEnabled);
        }

        #endregion

        #region Old Button Control

        private bool _IsEnableButton;
        public bool IsEnableButton
        {
            get { return _IsEnableButton; }
            set
            {
                _IsEnableButton = value;
                NotifyOfPropertyChange(() => IsEnableButton);
                NotifyOfPropertyChange(() => btCreateNewIsEnabled);
                NotifyOfPropertyChange(() => btCreateNewByOldIsEnabled);
                NotifyOfPropertyChange(() => btEditIsEnabled);
            }
        }

        private void ButtonForNotDiag(bool bCancel)
        {
            //ban dau neu benh nhan chua co 1 chan doan bat ky nao het va bCancel =false
            //co the dung lai cho khi nhan nut tao moi hoac tao moi dua tren chan doan cu va bCancel=true
            IsEnableButton = false;
            // btCreateNewIsEnabled = false;
            // btCreateNewByOldIsEnabled = false;
            btSaveCreateNewIsEnabled = true;
            //btEditIsEnabled = false;
            btUpdateIsEnabled = false;
            btCancelIsEnabled = bCancel;
        }

        private void ButtonForHasDiag()
        {
            IsEnableButton = true;
            //btCreateNewIsEnabled = true && IsNotExistsDiagnosisTreatmentByPtRegDetailID;
            //btCreateNewByOldIsEnabled = true && IsNotExistsDiagnosisTreatmentByPtRegDetailID && (DiagTrmtItem != null && Globals.PatientAllDetails.RegistrationInfo != null && DiagTrmtItem.PatientServiceRecord != null && (DiagTrmtItem.PatientServiceRecord.PtRegistrationID == Globals.PatientAllDetails.RegistrationInfo.PtRegistrationID || DiagTrmtItem.PatientServiceRecord.PtRegistrationID == PtRegistrationIDLatest.GetValueOrDefault(0)));
            btSaveCreateNewIsEnabled = false;
            //btEditIsEnabled = true && (DiagTrmtItem != null && (DiagTrmtItem.DoctorStaffID == Globals.LoggedUserAccount.StaffID.GetValueOrDefault(0) || PermissionManager.IsAdminUser()));
            btUpdateIsEnabled = false;
            btCancelIsEnabled = false;
        }
        #endregion

        #region ICD9 control 

        bool IsICD9Code = true;

        private DiagnosisICD9Items _refICD9Item;
        public DiagnosisICD9Items refICD9Item
        {
            get
            {
                return _refICD9Item;
            }
            set
            {
                if (_refICD9Item != value)
                {
                    _refICD9Item = value;
                }
                NotifyOfPropertyChange(() => refICD9Item);
            }
        }

        private ObservableCollection<DiagnosisICD9Items> _refICD9List;
        public ObservableCollection<DiagnosisICD9Items> refICD9List
        {
            get
            {
                return _refICD9List;
            }
            set
            {
                if (_refICD9List != value)
                {
                    _refICD9List = value;
                }
                NotifyOfPropertyChange(() => refICD9List);
            }
        }

        private void AddICD9BlankRow()
        {
            if (refICD9List != null
                && refICD9List.LastOrDefault() != null
                && refICD9List.LastOrDefault().RefICD9 == null)
            {
                return;
            }
            DiagnosisICD9Items ite = new DiagnosisICD9Items();
            refICD9List.Add(ite);
        }

        private RefICD9 RefICD9Copy = null;
        private string TreatmentOld = "";
        private string TreatmentNew = "";



        AxDataGridNyICD10 grdICD9 { get; set; }
        public void grdICD9_Loaded(object sender, RoutedEventArgs e)
        {
            grdICD9 = sender as AxDataGridNyICD10;
        }

        public void grdICD9_PreparingCellForEdit(object sender, DataGridPreparingCellForEditEventArgs e)
        {
            ColumnIndex = e.Column.DisplayIndex;

            if (refICD9Item != null)
            {
                RefICD9Copy = refICD9Item.RefICD9.DeepCopy();
            }
            if (e.Column.DisplayIndex == 0)
            {
                IsICD9Code = true;
            }
            else
            {
                IsICD9Code = false;
            }
        }


        public void grdICD9_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            DiagnosisICD9Items item = e.Row.DataContext as DiagnosisICD9Items;

            if (ColumnIndex == 0 || ColumnIndex == 1)
            {
                if (refICD9Item.RefICD9 == null)
                {
                    if (RefICD9Copy != null)
                    {
                        refICD9Item.RefICD9 = ObjectCopier.DeepCopy(RefICD9Copy);
                        if (CheckExistsICD9(refICD9Item, false))
                        {
                            GetTreatment(refICD9Item.RefICD9);
                        }
                    }
                }
            }
            if (refICD9Item != null && refICD9Item.RefICD9 != null)
            {
                if (CheckExistsICD9(refICD9Item, false))
                {
                    if (e.Row.GetIndex() == (refICD9List.Count - 1) && e.EditAction == DataGridEditAction.Commit)
                    {
                        System.Windows.Application.Current.Dispatcher.Invoke(() => AddICD9BlankRow());
                    }
                }
            }

        }


        private bool CheckExistsICD9(DiagnosisICD9Items Item, bool HasMessage = true)
        {
            int i = 0;
            if (Item.RefICD9 == null)
            {
                return true;
            }
            foreach (DiagnosisICD9Items p in refICD9List)
            {
                if (p.RefICD9 != null)
                {
                    if (Item.RefICD9.ICD9Code == p.RefICD9.ICD9Code)
                    {
                        i++;
                    }
                }
            }
            if (i > 1)
            {
                Item.RefICD9 = null;
                if (HasMessage)
                {
                    //Remind change the message
                    MessageBox.Show(eHCMSResources.Z1909_G1_ICD9DaTonTai);
                }
                return false;
            }
            else
            {
                return true;
            }
        }

        public void GetTreatment(RefICD9 refICD9)
        {
            if (refICD9 != null)
            {
                TreatmentNew = refICD9.ProcedureName;
                if (TreatmentOld != "")
                {
                    DiagTrmtItem.Treatment = DiagTrmtItem.Treatment.Replace(TreatmentOld, TreatmentNew);
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(DiagTrmtItem.Treatment))
                    {
                        DiagTrmtItem.Treatment += TreatmentNew;
                    }
                    else
                    {
                        DiagTrmtItem.Treatment += "- " + TreatmentNew;
                    }
                }
                TreatmentOld = ObjectCopier.DeepCopy(TreatmentNew);
            }

        }

        public void grdICD9_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DiagnosisICD9Items item = ((DataGrid)sender).SelectedItem as DiagnosisICD9Items;
            if (item != null && item.RefICD9 != null)
            {
                RefICD9Copy = item.RefICD9;
                TreatmentNew = TreatmentOld = ObjectCopier.DeepCopy(item.RefICD9.ProcedureName);
                RefICD9Copy = ObjectCopier.DeepCopy(item.RefICD9);
            }
            else
            {
                TreatmentNew = TreatmentOld = "";
                RefICD9Copy = null;
            }
        }

        public void lnkDeleteICD9_Click(object sender, RoutedEventArgs e)
        {
            if (refICD9Item == null
                || refICD9Item.RefICD9 == null)
            {
                MessageBox.Show(eHCMSResources.Z0405_G1_KgTheXoaDongRong);
                return;
            }

            int nSelIndex = grdICD9.SelectedIndex;
            if (nSelIndex >= refICD9List.Count - 1)
            {
                MessageBox.Show(eHCMSResources.Z0405_G1_KgTheXoaDongRong);
                return;
            }

            var item = refICD9List[nSelIndex];

            if (item != null && item.ICD9Code != null && item.ICD9Code != "")
            {
                if (MessageBox.Show(eHCMSResources.Z1910_G1_BanMuonXoaICD9, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    if (refICD9Item.RefICD9.ProcedureName != "")
                    {
                        DiagTrmtItem.Treatment = DiagTrmtItem.Treatment.Replace(refICD9Item.RefICD9.ProcedureName, "");
                    }
                    refICD9List.Remove(refICD9List[nSelIndex]);
                }
            }
        }


        #endregion

        #region Autocomplete ICD9-Code

        AutoCompleteBox AutoICD9Code;

        private PagedSortableCollectionView<RefICD9> _pageIDC9;
        public PagedSortableCollectionView<RefICD9> pageIDC9
        {
            get
            {
                return _pageIDC9;
            }
            set
            {
                if (_pageIDC9 != value)
                {
                    _pageIDC9 = value;
                }
                NotifyOfPropertyChange(() => pageIDC9);
            }
        }

        public void aucICD9_Populating(object sender, PopulatingEventArgs e)
        {
            if (IsICD9Code)
            {
                e.Cancel = true;
                procName = e.Parameter;
                ICD9SearchType = 0;
                pageIDC9.PageIndex = 0;
                SearchICD9(e.Parameter, 0, 0, pageIDC9.PageSize);
            }
        }

        public void aucICD9_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            if (!isDropDownICD9)
            {
                return;
            }
            isDropDownICD9 = false;
            if (refICD9Item != null)
            {
                refICD9Item.RefICD9 = ((AutoCompleteBox)sender).SelectedItem as RefICD9;
                //▼====== #006
                if (CheckIsMainForICD9())
                {
                    refICD9Item.IsMain = true;
                }
                //▲====== #006
                if (CheckExistsICD9(refICD9Item))
                {
                    GetTreatment(refICD9Item.RefICD9);
                }
            }
        }

        private bool isDropDownICD9 = false;
        public void aucICD9_DropDownClosing(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            isDropDownICD9 = true;
        }

        public void SearchICD9(string name, byte type, int PageIndex, int PageSize)
        {
            var t = new Thread(() =>
            {
                using (var serviceFactory = new CommonUtilsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginSearchRefICD9(name, PageIndex, PageSize, type, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            int Total = 10;
                            var results = contract.EndSearchRefICD9(out Total, asyncResult);
                            pageIDC9.Clear();
                            pageIDC9.TotalItemCount = Total;
                            if (results != null)
                            {
                                foreach (RefICD9 p in results)
                                {
                                    pageIDC9.Add(p);
                                }
                            }
                            if (ICD9SearchType == 0)
                            {
                                Acb_ICD9_Code.ItemsSource = pageIDC9;
                                Acb_ICD9_Code.PopulateComplete();
                            }
                            else
                            {
                                Acb_ICD9_Name.ItemsSource = pageIDC9;
                                Acb_ICD9_Name.PopulateComplete();
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



        #endregion

        #region ICD 9 - Procedure Name

        //AutoCompleteBox auProcedureName;
        private string procName = "";
        private byte ICD9SearchType = 0;
        //public void ProcedureName_Loaded(object sender, RoutedEventArgs e)
        //{
        //    auProcedureName = (AutoCompleteBox)sender;
        //}

        public void ProcedureName_Populating(object sender, PopulatingEventArgs e)
        {
            if (!IsICD9Code && ColumnIndex == 1)
            {
                e.Cancel = true;
                procName = e.Parameter;
                ICD9SearchType = 1;
                pageIDC9.PageIndex = 0;
                SearchICD9(e.Parameter, 1, 0, pageIDC9.PageSize);
            }
        }

        private bool isICD9DropDown = false;
        public void ProcedureName_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            if (!isICD9DropDown)
            {
                return;
            }
            isICD9DropDown = false;
            refICD9Item.RefICD9 = ((AutoCompleteBox)sender).SelectedItem as RefICD9;
            //▼====== #006
            if (CheckIsMainForICD9())
            {
                refICD9Item.IsMain = true;
            }
            //▲====== #006
            if (CheckExistsICD9(refICD9Item))
            {
                GetTreatment(refICD9Item.RefICD9);
            }
        }

        public void ProcedureName_DropDownClosing(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            isICD9DropDown = true;
        }

        #endregion

        /*TMA*/
        private bool _btGCTIsEnabled = true;
        public bool btGCTIsEnabled
        {
            get
            {
                return _btGCTIsEnabled;
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
                return _btGCT_CLS_IsEnabled;
            }
            set
            {
                _btGCT_CLS_IsEnabled = value;
                NotifyOfPropertyChange(() => btGCT_CLS_IsEnabled);
            }
        }

        /*TMA*/


        /*TMA*/
        //btGCT
        //▼====== #004
        //TTM:  Chuyển từ publish event sang gọi thông qua Interface và đặt hàm gọi vào Action.
        //      Thông tin bệnh nhân, thông tin thẻ bảo hiểm ... phải đc truyền vào trong lúc người dùng mở popup.
        //      Không lý do gì đóng popup lại rồi mới truyền dữ liệu.

        //public void GetPaPerReferalFul(int V_TransferFormType, int PatientFindBy)
        //{
        //    PatientRegistration CurRegistration = Registration_DataStorage.CurrentPatientRegistration;
        //    Action<IPaperReferalFull> onInitDlg = delegate (IPaperReferalFull TransferFromVm)
        //    {
        //        TransferFromVm.V_TransferFormType = V_TransferFormType;

        //        TransferFromVm.V_GetPaperReferalFullFromOtherView = true;
        //        if (CurRegistration != null && CurRegistration.PtRegistrationID > 0)
        //        {
        //            TransferFromVm.CurrentTransferForm.CurPatientRegistration = new PatientRegistration();
        //            TransferFromVm.CurrentTransferForm.CurPatientRegistration.PtRegistrationID = (long)CurRegistration.PtRegistrationID;
        //            if (CurRegistration.HisID_2 != null)
        //                TransferFromVm.CurrentTransferForm.CurPatientRegistration.HisID = (long)CurRegistration.HisID_2.Value;
        //            else if (CurRegistration.HisID != null)
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
        //    if (CurRegistration.HisID_2 != null)
        //        mEvent.Item.CurPatientRegistration.HisID = (long)CurRegistration.HisID_2.Value;
        //    else if (CurRegistration.HisID != null)
        //        mEvent.Item.CurPatientRegistration.HisID = (long)CurRegistration.HisID.Value;

        //    if (CurRegistration != null)
        //    {
        //        if (CurRegistration.HealthInsurance_2 != null)
        //            mEvent.Item.CurPatientRegistration.HealthInsurance = CurRegistration.HealthInsurance_2;
        //        else if (CurRegistration.HealthInsurance != null)
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

        public void GetPaPerReferalFul(int V_TransferFormType, int PatientFindBy)
        {
            PatientRegistration CurRegistration = Registration_DataStorage.CurrentPatientRegistration;
            var mEvent = new TransferFormEvent();
            if (CurrentTransferForm != null && CurrentTransferForm.TransferFormID > 0)
            {
                mEvent.Item = CurrentTransferForm;

            }
            else
            {

                mEvent.Item = new TransferForm();
                mEvent.Item.PatientFindBy = PatientFindBy;
                mEvent.Item.CurPatientRegistration = new PatientRegistration();
                mEvent.Item.V_TransferFormType = V_TransferFormType;
                mEvent.Item.TransferFormID = (long)0;
                mEvent.Item.CurPatientRegistration.PtRegistrationID = (long)CurRegistration.PtRegistrationID;
                if (CurRegistration.HisID_2 != null)
                    mEvent.Item.CurPatientRegistration.HisID = (long)CurRegistration.HisID_2.Value;
                else if (CurRegistration.HisID != null)
                    mEvent.Item.CurPatientRegistration.HisID = (long)CurRegistration.HisID.Value;
                if (CurRegistration != null)
                {
                    if (CurRegistration.HealthInsurance_2 != null)
                        mEvent.Item.CurPatientRegistration.HealthInsurance = CurRegistration.HealthInsurance_2;
                    else if (CurRegistration.HealthInsurance != null)
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
                    }
                    if (DiagTrmtItem.PatientServiceRecord != null && DiagTrmtItem.PatientServiceRecord.ExamDate != null)
                        mEvent.Item.FromDate = DiagTrmtItem.PatientServiceRecord.ExamDate;
                    mEvent.Item.ToDate = DateTime.Now;
                    mEvent.Item.TransferDate = DateTime.Now;
                }
                mEvent.Item.TransferFromHos = new Hospital();
                mEvent.Item.TransferToHos = new Hospital();
                mEvent.Item.V_TransferTypeID = 62604;           // defalut : chuyễn giữa các cơ sở cùng tuyế 
                mEvent.Item.V_PatientStatusID = 63002;          //defalut : không cấp cứu
                mEvent.Item.V_TransferReasonID = 62902;         //default : yêu cầu chuyên môn
                mEvent.Item.V_TreatmentResultID = 62702;        //defalut : ko thuyên giảm. Nặng lên - V_TreatmentResult_V2 trong bảng lookup
                mEvent.Item.V_CMKTID = 62801;                   // default : chuyển đúng tuyến, đúng chuyên môn kỹ thuật
            }
            Action<IPaperReferalFull> onInitDlg = delegate (IPaperReferalFull TransferFromVm)
            {
                TransferFromVm.IsThisViewDialog = true;
                TransferFromVm.V_TransferFormType = V_TransferFormType;
                TransferFromVm.V_GetPaperReferalFullFromOtherView = true;
                if (CurRegistration != null && CurRegistration.PtRegistrationID > 0)
                {
                    TransferFromVm.CurrentTransferForm.CurPatientRegistration = new PatientRegistration();
                    TransferFromVm.CurrentTransferForm.CurPatientRegistration.PtRegistrationID = (long)CurRegistration.PtRegistrationID;
                    if (CurRegistration.HisID_2 != null)
                        TransferFromVm.CurrentTransferForm.CurPatientRegistration.HisID = (long)CurRegistration.HisID_2.Value;
                    else if (CurRegistration.HisID != null)
                        TransferFromVm.CurrentTransferForm.CurPatientRegistration.HisID = (long)CurRegistration.HisID.Value;
                }
                this.ActivateItem(TransferFromVm);
                TransferFromVm.SetCurrentInformation(mEvent);
            };
            GlobalsNAV.ShowDialog<IPaperReferalFull>(onInitDlg);
        }
        //▲====== #004
        public TransferForm CurrentTransferForm { get; set; } = new TransferForm();
        public void btGCT()
        {
            PatientRegistration CurRegistration = Registration_DataStorage.CurrentPatientRegistration;
            if (CurrentTransferForm.CurPatientRegistration != null && CurRegistration.PatientID != CurrentTransferForm.CurPatientRegistration.PatientID)
            {
                CurrentTransferForm = new TransferForm();
            }

            if (CurRegistration.AdmissionInfo.TransferFormID > 0 && CurrentTransferForm.TransferFormID == 0)
            {
                GetTransferFormByID(CurRegistration.AdmissionInfo.TransferFormID);
            }
            else
            {
                GetPaPerReferalFul((int)AllLookupValues.V_TransferFormType.CHUYEN_Di, (int)AllLookupValues.PatientFindBy.NOITRU);
            }
            //GetPaPerReferalFul((int)AllLookupValues.V_TransferFormType.CHUYEN_Di,(int)AllLookupValues.PatientFindBy.NOITRU);
        }
        public void Handle(OnChangedPaperReferal message)
        {
            PatientRegistration CurRegistration = Registration_DataStorage.CurrentPatientRegistration;
            CurrentTransferForm = message.TransferForm;
            CurRegistration.AdmissionInfo.TransferFormID = CurrentTransferForm.TransferFormID;
        }
        private void GetTransferFormByID(long transferFormID)
        {
            var t = new Thread(() =>
            {
                using (var serviceFactory = new ePMRsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginGetTransferFormByID(transferFormID, (int)AllLookupValues.PatientFindBy.NOITRU, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            CurrentTransferForm = contract.EndGetTransferFormByID(asyncResult);
                            GetPaPerReferalFul((int)AllLookupValues.V_TransferFormType.CHUYEN_Di, (int)AllLookupValues.PatientFindBy.NOITRU);
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
        public void btGCT_CLS()
        {
            GetPaPerReferalFul((int)AllLookupValues.V_TransferFormType.CHUYEN_DI_CLS, (int)AllLookupValues.PatientFindBy.NOITRU);
        }

        //btGCT_CLS
        /*TMA*/
        /*▼====: #002*/
        public void btnCatastropheHistory()
        {
            Action<ICatastropheHistory> onInitDlg = delegate (ICatastropheHistory mView)
            {
                if (Registration_DataStorage.CurrentPatientRegistration != null)
                    mView.PtRegistrationID = Registration_DataStorage.CurrentPatientRegistration.PtRegistrationID;
            };
            GlobalsNAV.ShowDialog<ICatastropheHistory>(onInitDlg);
        }
        /*▲====: #002*/

        private bool _IsForCollectDiagnosis = false;
        public bool IsForCollectDiagnosis
        {
            get => _IsForCollectDiagnosis; set
            {
                _IsForCollectDiagnosis = value;
                NotifyOfPropertyChange(() => IsForCollectDiagnosis);
            }
        }
        private bool _IsProcedureEdit = false;
        public bool IsProcedureEdit
        {
            get => _IsProcedureEdit;
            set
            {
                _IsProcedureEdit = value;
                NotifyOfPropertyChange(() => IsForCollectDiagnosis);
                NotifyOfPropertyChange(() => btUpdateIsEnabled);
                NotifyOfPropertyChange(() => btSaveCreateNewIsEnabled);
                NotifyOfPropertyChange(() => IsConsultationEdit);
                NotifyOfPropertyChange(() => IsConsultationKhoaSanEdit);
            }
        }

        public void GetLatesDiagTrmtByPtID_InPt_OnlyForDia(long? InPtRegistrationID, long? V_DiagnosisType)
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                using (var serviceFactory = new ePMRsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetLatestDiagnosisTreatmentByPtID_InPt_ForDiag(InPtRegistrationID, V_DiagnosisType, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            DiagTrmtItem = contract.EndGetLatestDiagnosisTreatmentByPtID_InPt_ForDiag(asyncResult);
                            NotifyOfPropertyChange(() => btCreateNewByOldIsEnabled);
                            if (DiagTrmtItem != null && DiagTrmtItem.DTItemID > 0)
                            {
                                DiagnosisIcd10Items_Load_InPt(DiagTrmtItem.DTItemID);
                                DiagnosisICD9Items_Load_InPt(DiagTrmtItem.DTItemID);
                                hasDiag = true;
                                StatePopupUpdate();
                                /*▼====: #008*/
                                //TBL: Lấy BS ra chẩn đoán
                                gSelectedDoctorStaff = DiagTrmtItem.ObjDoctorStaffID;
                                /*▲====: #008*/

                            }
                            else
                            {

                                hasDiag = false;

                                AddBlankRow();

                                AddICD9BlankRow();

                                ResetDefaultForBehaving();

                                StateNewWaiting();
                            }

                            DiagTrmtItemCopy = ObjectCopier.DeepCopy(DiagTrmtItem);

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
        private long? _InPtRegistrationID = 0;
        public long? InPtRegistrationID
        {
            get
            {
                return _InPtRegistrationID;
            }
            set
            {
                if (_InPtRegistrationID != value)
                {
                    _InPtRegistrationID = value;
                    NotifyOfPropertyChange(() => InPtRegistrationID);
                }
            }
        }
        public bool CheckAdmission()
        {
            if (DiagTrmtItem != null && DiagTrmtItem.AdmissionInfo != null && DiagTrmtItem.AdmissionInfo.AdmissionDate != null)
            {
                MessageBox.Show(eHCMSResources.Z2634_G1_KhongCapNhatDeNghiDaNhapVien);
                return false;
            }
            return true;
        }

        private void StatePopupUpdate()
        {
            btCreateNewIsEnabled = false;
            btCreateNewByOldIsEnabled = false;
            btSaveCreateNewIsEnabled = false;
            btCancelIsEnabled = false;
            btUpdateIsEnabled = false;
            FormEditorIsEnabled = false;
            btEditIsEnabled = true;

            NotifyOfPropertyChange(() => btCreateNewIsEnabled);
            NotifyOfPropertyChange(() => btCreateNewByOldIsEnabled);
            NotifyOfPropertyChange(() => btSaveCreateNewIsEnabled);
            NotifyOfPropertyChange(() => btCancelIsEnabled);
            NotifyOfPropertyChange(() => FormEditorIsEnabled);
        }
        //▼====== #005
        #region ICD 10
        AutoCompleteBox Acb_ICD10_Code = null;

        AutoCompleteBox Acb_ICD10_Name = null;

        public void AcbICD10Code_Loaded(object sender, RoutedEventArgs e)
        {
            Acb_ICD10_Code = (AutoCompleteBox)sender;
        }
        public void AcbICD10Name_Loaded(object sender, RoutedEventArgs e)
        {
            Acb_ICD10_Name = (AutoCompleteBox)sender;
        }

        public void aucICD10_Populating(object sender, PopulatingEventArgs e)
        {
            if (IsCode)
            {
                e.Cancel = true;
                Name = e.Parameter;
                Type = 0;
                refIDC10.PageIndex = 0;
                LoadRefDiseases(e.Parameter, 0, 0, 100);
            }
        }

        public void aucICD10Name_Populating(object sender, PopulatingEventArgs e)
        {
            if (!IsCode && ColumnIndex == 1)
            {
                e.Cancel = true;
                Type = 1;
                Name = e.Parameter;
                refIDC10.PageIndex = 0;
                LoadRefDiseases(e.Parameter, 1, 0, 100);
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
                //▼====== #015
                //▼====== #014
                if (refIDC10Item.IsMain && CheckRequireSubICd(refIDC10Item) && CheckRuleICd(refIDC10Item)) { }
                //▲====== #014
                //▲====== #015
            }
        }

        private bool isDiseaseDropDown = false;
        public void DiseaseName_DropDownClosing(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            isDiseaseDropDown = true;
        }
        public void DiseaseName_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            if (!isDiseaseDropDown)
            {
                return;
            }
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
            //▼====== #015
            //▼====== #014
            if (refIDC10Item.IsMain && CheckRequireSubICd(refIDC10Item) && CheckRuleICd(refIDC10Item)) { }
            //▲====== #014
            //▲====== #015
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
                //20200415 TBL: BM 0030109: Khi ICD10 ngưng sử dụng thì sẽ tô màu đỏ
                if (objRows.DiseasesReference != null && !objRows.DiseasesReference.IsActive)
                {
                    e.Row.Background = new SolidColorBrush(Color.FromRgb(255, 69, 0));
                }
            }
        }
        public void grdConsultation_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (grdConsultation != null && grdConsultation.SelectedItem != null)
            {
                grdConsultation.BeginEdit();
            }
        }
        #endregion
        AutoCompleteBox Acb_ICD9_Code = null;
        public void aucICD9_Code_Loaded(object sender, RoutedEventArgs e)
        {
            Acb_ICD9_Code = (AutoCompleteBox)sender;
        }
        AutoCompleteBox Acb_ICD9_Name = null;
        public void aucICD9_Name_Loaded(object sender, RoutedEventArgs e)
        {
            Acb_ICD9_Name = (AutoCompleteBox)sender;
        }
        //▲===== #005
        //▼====== #006: Kiểm tra check ICD 9 chính cho cái đầu tiên
        private bool CheckIsMainForICD9()
        {
            ObservableCollection<DiagnosisICD9Items> temp = refICD9List.Where(x => x.ICD9Code != null).ToObservableCollection();
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
        //▲====== #006
        public void UpdateDiagTrmtItemIntoLayout(DiagnosisTreatment aDiagTrmtItem
            , ObservableCollection<DiagnosisIcd10Items> aRefIDC10List
            , ObservableCollection<DiagnosisICD9Items> aRefICD9List)
        {
            DiagTrmtItem = ObjectCopier.DeepCopy(aDiagTrmtItem);
            //KMx: Nội trú không cần kiểm tra 7 ngày (17/12/2014 17:00).
            //if (ConsultState != ConsultationState.NewConsultationState)
            //{
            //    ValidateExpiredDiagnosicTreatment(DiagTrmtItem);
            //}
            refIDC10List = aRefIDC10List;
            refICD9List = aRefICD9List;
            /*▼====: #008*/
            //TBL: Lấy BS ra chẩn đoán
            if (DiagTrmtItem.ObjDoctorStaffID != null)
            {
                gSelectedDoctorStaff = DiagTrmtItem.ObjDoctorStaffID;
            }
            else
            {
                gSelectedDoctorStaff.StaffID = DiagTrmtItem.DoctorStaffID;
            }
            /*▲====: #008*/
            //ButtonForHasDiag();
            //Thủ tục hàm Edit
            DiagTrmtItemCopy = ObjectCopier.DeepCopy(DiagTrmtItem);
            refIDC10ListCopy = ObjectCopier.DeepCopy(refIDC10List);
            refICD9ListCopy = ObjectCopier.DeepCopy(refICD9List);
            //Thủ tục hàm Edit
            AddBlankRow();
            AddICD9BlankRow();
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
        private bool _IsAdmRequest = false;
        public bool IsAdmRequest
        {
            get
            {
                return _IsAdmRequest;
            }
            set
            {
                _IsAdmRequest = value;
                NotifyOfPropertyChange(() => IsAdmRequest);
                NotifyOfPropertyChange(() => IsConsultationEdit);
                NotifyOfPropertyChange(() => IsConsultationKhoaSanEdit);
            }
        }
        //▼===== #007
        private Visibility _IsVisibleAdmRequest = Visibility.Visible;
        public Visibility IsVisibleAdmRequest
        {
            get
            {
                return _IsVisibleAdmRequest;
            }
            set
            {
                if (_IsVisibleAdmRequest != value)
                {
                    _IsVisibleAdmRequest = value;
                }
                NotifyOfPropertyChange(() => IsVisibleAdmRequest);
            }
        }
        //▲===== #007
        //▼===== #013
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

        private bool _DiagnosisFinalIsEnabled = true;
        public bool DiagnosisFinalIsEnabled
        {
            get { return _DiagnosisFinalIsEnabled; }
            set
            {
                _DiagnosisFinalIsEnabled = value;
                NotifyOfPropertyChange(() => DiagnosisFinalIsEnabled);
            }
        }
        //▲===== #013
        public void HideAllButton()
        {
            mChanDoan_tabSuaKhamBenh_HieuChinh = false;
        }
        /*▼====: #008*/
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
                if (_gSelectedDoctorStaff == value) return;
                _gSelectedDoctorStaff = value;
                NotifyOfPropertyChange(() => gSelectedDoctorStaff);
            }
        }
        private void LoadDoctorStaffCollection()
        {
            //▼====: #016
            DoctorStaffs = new ObservableCollection<Staff>(Globals.AllStaffs.Where(x => x.RefStaffCategory != null
                                                                && x.RefStaffCategory.V_StaffCatType == Globals.ServerConfigSection.CommonItems.StaffCatTypeBAC_SI
                                                                && !x.IsStopUsing).ToList());
            //▲====: #016
            gSelectedDoctorStaff = DoctorStaffs != null ? DoctorStaffs.FirstOrDefault(x => x.StaffID == Globals.LoggedUserAccount.StaffID) : null;
        }
        public void DoctorStaff_Populating(object sender, PopulatingEventArgs e)
        {
            AutoCompleteBox cboContext = sender as AutoCompleteBox;
            e.Cancel = true;
            //▼====: #016
            //▼====: #012
            var AllItemsContext = new ObservableCollection<Staff>();
            if (Globals.ServerConfigSection.ConsultationElements.FilterDoctorByDeptResponsibilitiesInPt && !Globals.IsUserAdmin)
            {
                string tempCurDeptID = "";
                if (IsAdmRequest)
                {
                    tempCurDeptID = DepartmentContent.SelectedItem.DeptID.ToString();
                }
                else
                {
                    tempCurDeptID = DiagTrmtItem != null && DiagTrmtItem.Department != null ? DiagTrmtItem.Department.DeptID.ToString() : "";
                }
                AllItemsContext = DoctorStaffs.Where(x => x.ListDeptResponsibilities != null
                                                    && ((x.ListDeptResponsibilities.Contains(tempCurDeptID) || tempCurDeptID == ""))
                                                    && Globals.RemoveVietnameseString(x.FullName).ToLower().Contains(Globals.RemoveVietnameseString(cboContext.SearchText).ToLower())).ToObservableCollection();
            }
            else
            {
                AllItemsContext = DoctorStaffs.Where(x => Globals.RemoveVietnameseString(x.FullName).ToLower().Contains(Globals.RemoveVietnameseString(cboContext.SearchText).ToLower())).ToObservableCollection();
            }
            //▲====: #012
            //▲====: #016
            cboContext.ItemsSource = AllItemsContext;
            cboContext.PopulateComplete();
        }
        public void DoctorStaff_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            gSelectedDoctorStaff = ((AutoCompleteBox)sender).SelectedItem as Staff;
        }
        /*▲====: #008*/
        //▼===== #011
        private void CheckListICD10Code()
        {
            string msg = "";
            foreach (DiagnosisIcd10Items item in refIDC10List)
            {
                if (item.DiseasesReference != null && !item.DiseasesReference.IsActive)
                {
                    msg += string.Format(", " + item.DiseasesReference.ICD10Code);
                }
            }
            if (!string.IsNullOrEmpty(msg))
            {
                MessageBox.Show(string.Format(eHCMSResources.Z3010_G1_MsgICD10, msg.Substring(2)), eHCMSResources.G0442_G1_TBao);
            }
        }
        //▲===== #011
        //▼===== #014
        List<RequiredSubDiseasesReferences> ListRequiredSubDiseasesReferences { get; set; }

        public void Handle(SelectedRequireSubICDForDiagnosisTreatment message)
        {
            if (message != null && message.SubICDInfo != null)
            {
                if (message.MainICDIndex > 0)
                {
                    // theo yêu cầu 20210508 là sắp xếp lại thứ tự. Găm đầu tiên, Sao tiếp theo, rồi tiếp các ICD / reload lại chẩn đoán
                    ObservableCollection<DiagnosisIcd10Items> refIDC10ListBackup = refIDC10List.DeepCopy();
                    ObservableCollection<DiagnosisIcd10Items> refIDC10ListNew = new ObservableCollection<DiagnosisIcd10Items>();

                    DiagnosisIcd10Items icd10ItemsMain = refIDC10List.ElementAt(message.MainICDIndex);
                    refIDC10ListNew.Add(icd10ItemsMain);

                    DiseasesReference temp = message.SubICDInfo;
                    DiagnosisIcd10Items icd10Items = new DiagnosisIcd10Items();
                    icd10Items.ICD10Code = temp.ICD10Code;
                    icd10Items.IsRequireSubICD = true;
                    icd10Items.V_DiagIcdStatus = (long)AllLookupValues.V_DiagIcdStatus.DANGDIEUTRI;
                    icd10Items.LookupStatus = new Lookup
                    {
                        LookupID = (long)AllLookupValues.V_DiagIcdStatus.DANGDIEUTRI,
                        ObjectValue = eHCMSResources.Z0540_G1_DangDTri.ToUpper()
                    };
                    icd10Items.DiseasesReference = temp;
                    refIDC10ListNew.Add(icd10Items);
                    foreach (var ICD10Item in refIDC10List)
                    {
                        if (ICD10Item.ICD10Code != icd10ItemsMain.ICD10Code && ICD10Item.ICD10Code != icd10Items.ICD10Code)
                        {
                            refIDC10ListNew.Add(ICD10Item);
                        }
                    }

                    refIDC10List = refIDC10ListNew.DeepCopy();
                    // lọc lại dữ liệu trên danh sách mới
                    DiagTrmtItem.DiagnosisFinal = string.Join("; ", from item in refIDC10List
                                                                    where (!string.IsNullOrEmpty(item.ICD10Code) && item.DiseasesReference != null)
                                                                    select item.DiseasesReference.DiseaseNameVN);
                    DiagTrmtItem.ICD10List = string.Join("; ", from item in refIDC10List
                                                               where (!string.IsNullOrEmpty(item.ICD10Code) && item.DiseasesReference != null)
                                                               select item.ICD10Code);
                }
                else
                {
                    DiseasesReference temp = message.SubICDInfo;
                    DiagnosisIcd10Items icd10Items = new DiagnosisIcd10Items();
                    icd10Items.ICD10Code = temp.ICD10Code;
                    icd10Items.IsRequireSubICD = true;
                    icd10Items.V_DiagIcdStatus = (long)AllLookupValues.V_DiagIcdStatus.DANGDIEUTRI;
                    icd10Items.LookupStatus = new Lookup
                    {
                        LookupID = (long)AllLookupValues.V_DiagIcdStatus.DANGDIEUTRI,
                        ObjectValue = eHCMSResources.Z0540_G1_DangDTri.ToUpper()
                    };
                    icd10Items.DiseasesReference = temp;
                    refIDC10List.Insert(message.MainICDIndex + 1, icd10Items);
                    // lọc lại dữ liệu trên danh sách mới
                    DiagTrmtItem.DiagnosisFinal = string.Join("; ", from item in refIDC10List
                                                                    where (!string.IsNullOrEmpty(item.ICD10Code) && item.DiseasesReference != null)
                                                                    select item.DiseasesReference.DiseaseNameVN);
                    DiagTrmtItem.ICD10List = string.Join("; ", from item in refIDC10List
                                                               where (!string.IsNullOrEmpty(item.ICD10Code) && item.DiseasesReference != null)
                                                               select item.ICD10Code);
                }
            }
        }

        public bool CheckRequireSubICd(DiagnosisIcd10Items MainICD)
        {
            int MainICDIndex = 0;
            ObservableCollection<RequiredSubDiseasesReferences> requiredSubDiseasesReferences = new ObservableCollection<RequiredSubDiseasesReferences>();
            if (MainICD != null && Registration_DataStorage.ListRequiredSubDiseasesReferences != null && refIDC10List.Where(x => x.IsRequireSubICD).Count() == 0)
            {
                requiredSubDiseasesReferences = Registration_DataStorage.ListRequiredSubDiseasesReferences.Where(x => x.MainICD10 == MainICD.ICD10Code).ToObservableCollection();
                MainICDIndex = refIDC10List.FindIndex(x => x.ICD10Code == MainICD.ICD10Code);
            }
            if (requiredSubDiseasesReferences.Count() == 1)
            {
                DiseasesReference temp = requiredSubDiseasesReferences.FirstOrDefault().SubICDInfo;
                DiagnosisIcd10Items icd10Items = new DiagnosisIcd10Items();
                icd10Items.ICD10Code = temp.ICD10Code;
                icd10Items.IsRequireSubICD = true;
                icd10Items.V_DiagIcdStatus = (long)AllLookupValues.V_DiagIcdStatus.DANGDIEUTRI;
                icd10Items.LookupStatus = new Lookup
                {
                    LookupID = (long)AllLookupValues.V_DiagIcdStatus.DANGDIEUTRI,
                    ObjectValue = eHCMSResources.Z0540_G1_DangDTri.ToUpper()
                };
                icd10Items.DiseasesReference = temp;
                refIDC10List.Insert(MainICDIndex + 1, icd10Items);
                DiagTrmtItem.DiagnosisFinal = string.Join("; ", from item in refIDC10List
                                                                where (!string.IsNullOrEmpty(item.ICD10Code) && item.DiseasesReference != null)
                                                                select item.DiseasesReference.DiseaseNameVN);
                DiagTrmtItem.ICD10List = string.Join("; ", from item in refIDC10List
                                                           where (!string.IsNullOrEmpty(item.ICD10Code) && item.DiseasesReference != null)
                                                           select item.ICD10Code);
                return true;
            }
            else if (requiredSubDiseasesReferences.Count() > 1)
            {
                void onInitDlg(ISelectRequireSubICD typeInfo)
                {
                    typeInfo.TitleForm = "Chọn bệnh sao kèm theo bệnh chính có dấu găm";
                    typeInfo.ListRequiredSubDiseasesReferences = requiredSubDiseasesReferences;
                    typeInfo.MainICDIndex = MainICDIndex;
                }
                GlobalsNAV.ShowDialog<ISelectRequireSubICD>(onInitDlg, null, false, true, Globals.GetHalfHeightAndThreeFourthWidthDefaultDialogViewSize());
                return false;
            }
            else
            {
                return true;
            }
        }

        public void ckbIsChecked_Click(object source, object sender)
        {
            CheckBox ckbIsChecked = source as CheckBox;
            if (!(ckbIsChecked.DataContext is DiagnosisIcd10Items))
            {
                return;
            }
            DiagnosisIcd10Items item = (ckbIsChecked.DataContext as DiagnosisIcd10Items);
            bool ItemIsMainBackup = item.IsMain;
            item.IsMain = ckbIsChecked.IsChecked.GetValueOrDefault(true);

            if (refIDC10List.Where(x => x.DiseasesReference != null && x.IsMain).ToObservableCollection().Count() > 1)
            {
                Globals.ShowMessage(eHCMSResources.Z0510_G1_I, eHCMSResources.G0442_G1_TBao);
                item.IsMain = ItemIsMainBackup;
                return;
            }

            if (item.IsMain)
            {
                if (CheckRequireSubICd(item))
                {
                }
                if (!CheckRuleICd(item))
                {
                    item.IsMain = ItemIsMainBackup;
                    return;
                }
            }
        }

        private void LoadGetListRequiredSubDiseasesReferences()
        {
            this.ShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ePMRsServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetListRequiredSubDiseasesReferences("*", Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                ListRequiredSubDiseasesReferences = contract.EndGetListRequiredSubDiseasesReferences(asyncResult).ToList();
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
        //▲===== #014

        //▼===== #015
        List<RuleDiseasesReferences> ListRuleDiseasesReferences { get; set; }
        private void LoadGetListRuleDiseasesReferences()
        {
            this.ShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ePMRsServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetListRuleDiseasesReferences("*", Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                ListRuleDiseasesReferences = contract.EndGetListRuleDiseasesReferences(asyncResult).ToList();
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

        public void Handle(SelectedRuleICDForDiagnosisTreatment message)
        {
            if (message != null && message.SubICDInfo != null)
            {
                ObservableCollection<DiagnosisIcd10Items> refIDC10ListBackup = refIDC10List.DeepCopy();
                ObservableCollection<DiagnosisIcd10Items> refIDC10ListNew = new ObservableCollection<DiagnosisIcd10Items>();
                DiseasesReference temp = message.SubICDInfo;
                DiagnosisIcd10Items icd10Items = new DiagnosisIcd10Items();
                icd10Items.ICD10Code = temp.ICD10Code;
                icd10Items.IsRequireSubICD = true;
                icd10Items.IsMain = message.IsException ? false : true;
                icd10Items.V_DiagIcdStatus = (long)AllLookupValues.V_DiagIcdStatus.DANGDIEUTRI;
                icd10Items.LookupStatus = new Lookup
                {
                    LookupID = (long)AllLookupValues.V_DiagIcdStatus.DANGDIEUTRI,
                    ObjectValue = eHCMSResources.Z0540_G1_DangDTri.ToUpper()
                };
                icd10Items.DiseasesReference = temp;
                refIDC10ListNew.Add(icd10Items);

                DiagnosisIcd10Items icd10ItemsMain = refIDC10List.ElementAt(message.MainICDIndex);
                icd10ItemsMain.IsMain = false;
                refIDC10ListNew.Add(icd10ItemsMain);

                foreach (var ICD10Item in refIDC10List)
                {
                    if (ICD10Item.ICD10Code != icd10ItemsMain.ICD10Code && ICD10Item.ICD10Code != icd10Items.ICD10Code)
                    {
                        refIDC10ListNew.Add(ICD10Item);
                    }
                }

                refIDC10List = refIDC10ListNew.DeepCopy();
                // lọc lại dữ liệu trên danh sách mới
                DiagTrmtItem.DiagnosisFinal = string.Join("; ", from item in refIDC10List
                                                                where (!string.IsNullOrEmpty(item.ICD10Code) && item.DiseasesReference != null)
                                                                select item.DiseasesReference.DiseaseNameVN);
                DiagTrmtItem.ICD10List = string.Join("; ", from item in refIDC10List
                                                           where (!string.IsNullOrEmpty(item.ICD10Code) && item.DiseasesReference != null)
                                                           select item.ICD10Code);
            }
        }

        public bool CheckRuleICd(DiagnosisIcd10Items MainICD = null)
        {
            // MainICD = null là recheck lại nguyên danh sách. chỉ xài khi lưu chẩn đoán
            if (MainICD == null)
            {
                int MainICDIndex = 0;
                ObservableCollection<RuleDiseasesReferences> ruleDiseasesReferences = new ObservableCollection<RuleDiseasesReferences>();
                string tempICD10List = string.Join("; ", from item in refIDC10List
                                                         where (!string.IsNullOrEmpty(item.ICD10Code) && item.DiseasesReference != null)
                                                         select item.ICD10Code);
                foreach (var itemICD10 in refIDC10List)
                {
                    // lấy danh sách ICD10Code dấu sao là bệnh kèm theo
                    if (itemICD10.ICD10Code != null && itemICD10.ICD10Code.Contains("*") && !itemICD10.IsMain)
                    {
                        ObservableCollection<RuleDiseasesReferences> temp1 = Registration_DataStorage.ListRuleDiseasesReferences.Where(x => x.MainICD10 == itemICD10.ICD10Code
                            && tempICD10List.Contains(x.SubICD10)).ToObservableCollection();
                        // danh sách ICD không có tồn tại mã đi kèm thì bắt chọn
                        if (temp1 == null || temp1.Count() == 0)
                        {
                            ruleDiseasesReferences = Registration_DataStorage.ListRuleDiseasesReferences.Where(x => x.MainICD10 == itemICD10.ICD10Code).ToObservableCollection();
                            MainICDIndex = refIDC10List.FindIndex(x => x.ICD10Code == itemICD10.ICD10Code);
                            if (ruleDiseasesReferences.Count() == 1)
                            {
                                ObservableCollection<DiagnosisIcd10Items> refIDC10ListBackup = refIDC10List.DeepCopy();
                                ObservableCollection<DiagnosisIcd10Items> refIDC10ListNew = new ObservableCollection<DiagnosisIcd10Items>();
                                DiseasesReference temp = ruleDiseasesReferences.FirstOrDefault().SubICDInfo;
                                DiagnosisIcd10Items icd10Items = new DiagnosisIcd10Items();
                                icd10Items.ICD10Code = temp.ICD10Code;
                                icd10Items.IsMain = ruleDiseasesReferences.FirstOrDefault().IsException ? false : true;
                                icd10Items.IsRequireSubICD = true;
                                icd10Items.V_DiagIcdStatus = (long)AllLookupValues.V_DiagIcdStatus.DANGDIEUTRI;
                                icd10Items.LookupStatus = new Lookup
                                {
                                    LookupID = (long)AllLookupValues.V_DiagIcdStatus.DANGDIEUTRI,
                                    ObjectValue = eHCMSResources.Z0540_G1_DangDTri.ToUpper()
                                };
                                icd10Items.DiseasesReference = temp;
                                refIDC10ListNew.Add(icd10Items);

                                DiagnosisIcd10Items icd10ItemsMain = refIDC10List.ElementAt(MainICDIndex);
                                icd10ItemsMain.IsMain = false;
                                refIDC10ListNew.Add(icd10ItemsMain);

                                foreach (var ICD10Item in refIDC10List)
                                {
                                    if (ICD10Item.ICD10Code != icd10ItemsMain.ICD10Code && ICD10Item.ICD10Code != icd10Items.ICD10Code)
                                    {
                                        refIDC10ListNew.Add(ICD10Item);
                                    }
                                }

                                refIDC10List = refIDC10ListNew.DeepCopy();
                                // lọc lại dữ liệu trên danh sách mới
                                DiagTrmtItem.DiagnosisFinal = string.Join("; ", from item in refIDC10List
                                                                                where (!string.IsNullOrEmpty(item.ICD10Code) && item.DiseasesReference != null)
                                                                                select item.DiseasesReference.DiseaseNameVN);
                                DiagTrmtItem.ICD10List = string.Join("; ", from item in refIDC10List
                                                                           where (!string.IsNullOrEmpty(item.ICD10Code) && item.DiseasesReference != null)
                                                                           select item.ICD10Code);
                                return true;
                            }
                            else if (ruleDiseasesReferences.Count() > 1)
                            {
                                void onInitDlg(ISelectRuleICD typeInfo)
                                {
                                    typeInfo.TitleForm = "Chọn bệnh găm để làm bệnh chính";
                                    typeInfo.ListRuleDiseasesReferences = ruleDiseasesReferences;
                                    typeInfo.MainICDIndex = MainICDIndex;
                                }
                                GlobalsNAV.ShowDialog<ISelectRuleICD>(onInitDlg, null, false, true, Globals.GetHalfHeightAndThreeFourthWidthDefaultDialogViewSize());
                                return false;
                            }
                        }
                    }
                }
                return true;
            }
            // Nếu mã bệnh chính có dấu * , hiển thị các mã găm của nó rồi cho găm làm chính, nó kèm theo
            else if (MainICD.ICD10Code != null && MainICD.ICD10Code.Contains("*"))
            {
                int MainICDIndex = 0;
                ObservableCollection<RuleDiseasesReferences> ruleDiseasesReferences = new ObservableCollection<RuleDiseasesReferences>();
                if (Registration_DataStorage.ListRuleDiseasesReferences != null)
                {
                    // kiểm tra danh sách đã bỏ vào cho ICD đang chọn chưa đã có thì không cần hiển thị nữa
                    if (refIDC10List != null)
                    {
                        string tempICD10List = string.Join("; ", from item in refIDC10List
                                                                 where (!string.IsNullOrEmpty(item.ICD10Code) && item.DiseasesReference != null)
                                                                 select item.ICD10Code);
                        ObservableCollection<RuleDiseasesReferences> temp = Registration_DataStorage.ListRuleDiseasesReferences.Where(x => x.MainICD10 == MainICD.ICD10Code && tempICD10List.Contains(x.SubICD10)).ToObservableCollection();
                        if (temp == null || temp.Count() == 0)
                        {
                            ruleDiseasesReferences = Registration_DataStorage.ListRuleDiseasesReferences.Where(x => x.MainICD10 == MainICD.ICD10Code).ToObservableCollection();
                            MainICDIndex = refIDC10List.FindIndex(x => x.ICD10Code == MainICD.ICD10Code);
                        }
                        else
                        {
                            Globals.ShowMessage("Mã bệnh này không được làm bệnh chính! Vui lòng liên hệ KHTH", eHCMSResources.T0432_G1_Error);
                            return false;
                        }
                    }
                    else
                    {
                        ruleDiseasesReferences = Registration_DataStorage.ListRuleDiseasesReferences.Where(x => x.MainICD10 == MainICD.ICD10Code).ToObservableCollection();
                        MainICDIndex = refIDC10List.FindIndex(x => x.ICD10Code == MainICD.ICD10Code);
                    }
                }
                if (ruleDiseasesReferences.Count() == 1)
                {
                    ObservableCollection<DiagnosisIcd10Items> refIDC10ListBackup = refIDC10List.DeepCopy();
                    ObservableCollection<DiagnosisIcd10Items> refIDC10ListNew = new ObservableCollection<DiagnosisIcd10Items>();
                    DiseasesReference temp = ruleDiseasesReferences.FirstOrDefault().SubICDInfo;
                    DiagnosisIcd10Items icd10Items = new DiagnosisIcd10Items();
                    icd10Items.ICD10Code = temp.ICD10Code;
                    icd10Items.IsMain = ruleDiseasesReferences.FirstOrDefault().IsException ? false : true;
                    icd10Items.IsRequireSubICD = true;
                    icd10Items.V_DiagIcdStatus = (long)AllLookupValues.V_DiagIcdStatus.DANGDIEUTRI;
                    icd10Items.LookupStatus = new Lookup
                    {
                        LookupID = (long)AllLookupValues.V_DiagIcdStatus.DANGDIEUTRI,
                        ObjectValue = eHCMSResources.Z0540_G1_DangDTri.ToUpper()
                    };
                    icd10Items.DiseasesReference = temp;
                    refIDC10ListNew.Add(icd10Items);

                    DiagnosisIcd10Items icd10ItemsMain = refIDC10List.ElementAt(MainICDIndex);
                    icd10ItemsMain.IsMain = false;
                    refIDC10ListNew.Add(icd10ItemsMain);

                    foreach (var ICD10Item in refIDC10List)
                    {
                        if (ICD10Item.ICD10Code != icd10ItemsMain.ICD10Code && ICD10Item.ICD10Code != icd10Items.ICD10Code)
                        {
                            refIDC10ListNew.Add(ICD10Item);
                        }
                    }

                    refIDC10List = refIDC10ListNew.DeepCopy();
                    // lọc lại dữ liệu trên danh sách mới
                    DiagTrmtItem.DiagnosisFinal = string.Join("; ", from item in refIDC10List
                                                                    where (!string.IsNullOrEmpty(item.ICD10Code) && item.DiseasesReference != null)
                                                                    select item.DiseasesReference.DiseaseNameVN);
                    DiagTrmtItem.ICD10List = string.Join("; ", from item in refIDC10List
                                                               where (!string.IsNullOrEmpty(item.ICD10Code) && item.DiseasesReference != null)
                                                               select item.ICD10Code);
                    return true;
                }
                else if (ruleDiseasesReferences.Count() > 1)
                {
                    void onInitDlg(ISelectRuleICD typeInfo)
                    {
                        typeInfo.TitleForm = "Chọn bệnh găm để làm bệnh chính";
                        typeInfo.ListRuleDiseasesReferences = ruleDiseasesReferences;
                        typeInfo.MainICDIndex = MainICDIndex;
                    }
                    GlobalsNAV.ShowDialog<ISelectRuleICD>(onInitDlg, null, false, true, Globals.GetHalfHeightAndThreeFourthWidthDefaultDialogViewSize());
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
        //▲===== #015
        public void btICDList()
        {
            Action<IICD_ListFindForConsultation> onInitDlg = delegate (IICD_ListFindForConsultation listFind)
            {
                this.ActivateItem(listFind);
            };
            GlobalsNAV.ShowDialog<IICD_ListFindForConsultation>(onInitDlg);
        }
        public void GetListAdmissionCriteria()
        {
            this.DlgShowBusyIndicator(eHCMSResources.Z0492_G1_DangXoa);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ConfigurationManagerServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetListAdmissionCriteria(Globals.DispatchCallback((asyncResult) =>
                       {
                           try
                           {
                               var Result = contract.EndGetListAdmissionCriteria(asyncResult);
                               if (Result != null)
                               {

                                   AdmissionCriteriaListA = Result.Where(x => x.V_AdmissionCriteriaType == (long)AllLookupValues.V_AdmissionCriteriaType.Loai_A).ToObservableCollection();
                                   AdmissionCriteriaListB = Result.Where(x => x.V_AdmissionCriteriaType == (long)AllLookupValues.V_AdmissionCriteriaType.Loai_B).ToObservableCollection();
                                   GetChekedAdmissionCriteria(DiagTrmtItem.AdmissionCriteriaList);
                               }

                           }
                           catch (Exception ex)
                           {
                               Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
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
        public void ckbAChecked_Click(object source, object sender)
        {

        }
        public void ckbBChecked_Click(object source, object sender)
        {

        }
        private bool CheckAdmissionCriteria()
        {
            if (AdmissionCriteriaListA.Where(x => x.IsChecked == true).ToList().Count == 0 && AdmissionCriteriaListB.Where(x => x.IsChecked == true).ToList().Count == 0)
            {
                return false;
            }
            else
            {
                string tempA = String.Join(",", from item in AdmissionCriteriaListA where item.IsChecked == true select item.AdmissionCriteriaCode);
                string tempB = String.Join(",", from item in AdmissionCriteriaListB where item.IsChecked == true select item.AdmissionCriteriaCode);
                if (string.IsNullOrEmpty(tempB))
                {
                    DiagTrmtItem.AdmissionCriteriaList = tempA;
                }
                else if (string.IsNullOrEmpty(tempA))
                {
                    DiagTrmtItem.AdmissionCriteriaList = tempB;
                }
                else
                {
                    DiagTrmtItem.AdmissionCriteriaList = tempA + "," + tempB;
                }

                return true;
            }
        }
        private void GetChekedAdmissionCriteria(string AdmissionCriteriaList)
        {
            if (string.IsNullOrEmpty(AdmissionCriteriaList))
            {
                return;
            }

            if (DiagTrmtItem != null)
            {
                if (AdmissionCriteriaList.Contains("IA"))
                {
                    AdmissionCriteriaListAIsChecked = true;
                }
                if (AdmissionCriteriaList.Contains("IB"))
                {
                    AdmissionCriteriaListBIsChecked = true;
                }
                foreach (var item in AdmissionCriteriaListA)
                {
                    if (AdmissionCriteriaList.Contains(item.AdmissionCriteriaCode))
                    {
                        item.IsChecked = true;
                    }
                }
                foreach (var item in AdmissionCriteriaListB)
                {
                    if (AdmissionCriteriaList.Contains(item.AdmissionCriteriaCode))
                    {
                        item.IsChecked = true;
                    }
                }
            }
        }
        private InjuryCertificates _CurInjuryCertificates_NT;
        public InjuryCertificates CurInjuryCertificates_NT
        {
            get
            {
                return _CurInjuryCertificates_NT;
            }
            set
            {
                if (_CurInjuryCertificates_NT != value)
                {
                    _CurInjuryCertificates_NT = value;
                    NotifyOfPropertyChange(() => CurInjuryCertificates_NT);
                }
            }
        }
        private int _LastCodeCNTT = 0;
        public int LastCodeCNTT
        {
            get
            {
                return _LastCodeCNTT;
            }
            set
            {
                if (_LastCodeCNTT != value)
                {
                    _LastCodeCNTT = value;
                    NotifyOfPropertyChange(() => LastCodeCNTT);
                }
            }
        }
        public void btGCNTT()
        {
            if (Registration_DataStorage == null || Registration_DataStorage.CurrentPatientRegistration == null || Registration_DataStorage.PatientServiceRecordCollection[0].DiagnosisTreatments.Count == 0)
            {
                return;
            }

            Action<IInjuryCertificates> onInitDlg = delegate (IInjuryCertificates injuryCertificates)
            {
                this.ActivateItem(injuryCertificates);
                injuryCertificates.IsOpenFromConsult = true;
                injuryCertificates.PtRegistration = ObjectCopier.DeepCopy(Registration_DataStorage.CurrentPatientRegistration);
                injuryCertificates.PtRegistration.DiagnosisTreatment = ObjectCopier.DeepCopy(Registration_DataStorage.PatientServiceRecordCollection[0].DiagnosisTreatments[0]);
                injuryCertificates.SetCurrentInformation(CurInjuryCertificates_NT.DeepCopy());

            };
            GlobalsNAV.ShowDialog<IInjuryCertificates>(onInitDlg);
            GetInjuryCertificates_ByPtRegID(Registration_DataStorage.CurrentPatientRegistration.PtRegistrationID, (long)Registration_DataStorage.CurrentPatientRegistration.V_RegistrationType);
        }
        private void GetInjuryCertificates_ByPtRegID(long PtRegistrationID, long V_RegistrationType)
        {
            //this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetInjuryCertificates_ByPtRegID(PtRegistrationID, V_RegistrationType, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                int tempCode;
                                InjuryCertificates result = contract.EndGetInjuryCertificates_ByPtRegID(out tempCode, asyncResult);
                                CurInjuryCertificates_NT = result ?? new InjuryCertificates();
                                if (Globals.ServerConfigSection.CommonItems.ApplyAutoCodeForCirculars56)
                                {
                                    LastCodeCNTT = tempCode;
                                }
                            }
                            catch (Exception ex)
                            {
                                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                            }
                            finally
                            {
                                //this.HideBusyIndicator();
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

            t.Start();
        }
        public void btPKVV()
        {
            if (InPtRegistrationID == null || InPtRegistrationID == 0)
            {
                MessageBox.Show("Chưa có thông tin nhập viện", "Thông báo");
                return;
            }
            GlobalsNAV.ShowDialog<IAdmissionExamination>((VM) =>
            {
                VM.InPtRegistrationID = Convert.ToInt64(InPtRegistrationID);
                VM.LoadAdmissionExamination(Convert.ToInt64(InPtRegistrationID), DiagTrmtItem.OrientedTreatment, false);
            }, null, false, true, new Size(1200, 700));
        }
        public void btPreviewPKVV()
        {
            if (IsAdmRequest)
            {
                if (InPtRegistrationID == null || InPtRegistrationID == 0)
                {
                    MessageBox.Show("Chưa có thông tin nhập viện", "Thông báo");
                    return;
                }
                Action<ICommonPreviewView> onInitDlg = delegate (ICommonPreviewView proAlloc)
                {
                    //proAlloc.ID = Convert.ToInt64(InPtRegistrationID);
                    proAlloc.RegistrationID = Convert.ToInt64(InPtRegistrationID);
                    proAlloc.eItem = ReportName.XRpt_AdmissionExamination;
                };
                GlobalsNAV.ShowDialog<ICommonPreviewView>(onInitDlg);
            }
            else
            {
                if (Registration_DataStorage.CurrentPatientRegistration == null || Registration_DataStorage.CurrentPatientRegistration.PtRegistrationID == 0)
                {
                    MessageBox.Show("Chưa chọn bệnh nhân", "Thông báo");
                    return;
                }
                Action<ICommonPreviewView> onInitDlg = delegate (ICommonPreviewView proAlloc)
                {
                    //proAlloc.ID = Convert.ToInt64(InPtRegistrationID);
                    proAlloc.RegistrationID = Registration_DataStorage.CurrentPatientRegistration.PtRegistrationID;
                    proAlloc.eItem = ReportName.XRpt_AdmissionExamination;
                };
                GlobalsNAV.ShowDialog<ICommonPreviewView>(onInitDlg);
            }
        }
        public void btDiseaseProgression()
        {
            GlobalsNAV.ShowDialog<IDiseaseProgression>((proAlloc) => { }, null, false, true, new Size(950, 950));
        }
        public void Handle(DiseaseProgression_Event message)
        {
            if (message != null)
            {
                if (String.IsNullOrEmpty(DiagTrmtItem.OrientedTreatment))
                {
                    DiagTrmtItem.OrientedTreatment += "Bệnh tỉnh, tiếp xúc tốt, da niêm hồng, tim đều, phổi trong, bụng mềm" + message.SelectedDiseaseProgression;

                }
                else
                {
                    DiagTrmtItem.OrientedTreatment += message.SelectedDiseaseProgression;
                }
                if (String.IsNullOrEmpty(DiagTrmtItem.Diagnosis))
                {
                    DiagTrmtItem.Diagnosis += message.SelectedDiseaseProgression.Substring(2, message.SelectedDiseaseProgression.Length-2);
                }
                else
                {
                    DiagTrmtItem.Diagnosis += message.SelectedDiseaseProgression;
                }
            }
        }

        //▼==== #017
        public void btPTKVCK()
        {
            if (InPtRegistrationID == null || InPtRegistrationID == 0)
            {
                MessageBox.Show("Chưa có thông tin nhập viện", "Thông báo");
                return;
            }
            GlobalsNAV.ShowDialog<ISelfDeclaration>((VM) =>
            {
                VM.PtRegistrationID = Convert.ToInt64(InPtRegistrationID);
                VM.PatientID = Convert.ToInt64(Registration_DataStorage.CurrentPatient.PatientID);
                VM.V_RegistrationType = Convert.ToInt64(AllLookupValues.RegistrationType.NOI_TRU);
                VM.GetSelfDeclarationByPtRegistrationID();
            }, null, false, true, new Size(1200, 700));
        }
        public void btPreviewPTKVCK()
        {
            if (IsAdmRequest)
            {
                if (InPtRegistrationID == null || InPtRegistrationID == 0)
                {
                    MessageBox.Show("Chưa có thông tin nhập viện", "Thông báo");
                    return;
                }
                Action<ICommonPreviewView> onInitDlg = delegate (ICommonPreviewView proAlloc)
                {
                    //proAlloc.ID = Convert.ToInt64(InPtRegistrationID);
                    proAlloc.RegistrationID = Convert.ToInt64(InPtRegistrationID);
                    proAlloc.V_RegistrationType = Convert.ToInt64(AllLookupValues.RegistrationType.NOI_TRU);
                    proAlloc.eItem = ReportName.XRptSelfDeclaration;
                };
                GlobalsNAV.ShowDialog<ICommonPreviewView>(onInitDlg);
            }
            else
            {
                if (Registration_DataStorage.CurrentPatientRegistration == null || Registration_DataStorage.CurrentPatientRegistration.PtRegistrationID == 0)
                {
                    MessageBox.Show("Chưa chọn bệnh nhân", "Thông báo");
                    return;
                }
                Action<ICommonPreviewView> onInitDlg = delegate (ICommonPreviewView proAlloc)
                {
                    //proAlloc.ID = Convert.ToInt64(InPtRegistrationID);
                    proAlloc.RegistrationID = Registration_DataStorage.CurrentPatientRegistration.PtRegistrationID;
                    proAlloc.V_RegistrationType = Convert.ToInt64(AllLookupValues.RegistrationType.NOI_TRU);
                    proAlloc.eItem = ReportName.XRptSelfDeclaration;
                };
                GlobalsNAV.ShowDialog<ICommonPreviewView>(onInitDlg);
            }
        }
        //▲==== #017

        private bool _IsConsultationEdit = false;
        public bool IsConsultationEdit
        {
            get => _IsConsultationEdit = !IsAdmRequest && !IsProcedureEdit;
            set
            {
                _IsConsultationEdit = value;
                NotifyOfPropertyChange(() => IsConsultationEdit);
            }
        }
        private bool _IsConsultationKhoaSanEdit = false;
        public bool IsConsultationKhoaSanEdit
        {
            get => _IsConsultationKhoaSanEdit = !IsAdmRequest && !IsProcedureEdit && Globals.DeptLocation.DeptID == Globals.ServerConfigSection.CommonItems.DeptIDKhoaSan;
            set
            {
                _IsConsultationKhoaSanEdit = value;
                NotifyOfPropertyChange(() => IsConsultationKhoaSanEdit);
            }
        }
        private int? _SoConChet = null;
        public int? SoConChet
        {
            get => _SoConChet;
            set
            {
                _SoConChet = value;
                if(SoConChet != null && SoConChet > 0)
                {
                    IsEnableSoConChet = true;
                }
                else
                {
                    IsEnableSoConChet = false;
                }
                NotifyOfPropertyChange(() => SoConChet);
            }
        }
        private bool _IsEnableSoConChet = false;
        public bool IsEnableSoConChet
        {
            get => _IsEnableSoConChet;
            set
            {
                _IsEnableSoConChet = value;
                NotifyOfPropertyChange(() => IsEnableSoConChet);
            }

        }
        private DateTime? _NgaySinhCon;
        public DateTime? NgaySinhCon
        {
            get => _NgaySinhCon;
            set
            {
                _NgaySinhCon = value;
                NotifyOfPropertyChange(() => NgaySinhCon);
            }
        }
        private DateTime? _NgayConChet;
        public DateTime? NgayConChet
        {
            get => _NgayConChet;
            set
            {
                _NgayConChet = value;
                NotifyOfPropertyChange(() => NgayConChet);
            }
        }
        private void AddNewDiagDetal_KhoaSan()
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                using (var serviceFactory = new ePMRsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginAddNewDiagDetal_KhoaSan(Registration_DataStorage.CurrentPatientRegistration.PtRegistrationID, SoConChet, NgayConChet, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var result = contract.EndAddNewDiagDetal_KhoaSan(asyncResult);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                            this.HideBusyIndicator();
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
        private void GetSoConChet_KhoaSan()
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                using (var serviceFactory = new ePMRsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetSoConChet_KhoaSan(Registration_DataStorage.CurrentPatientRegistration.PtRegistrationID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var result = contract.EndGetSoConChet_KhoaSan(out int? resultSoConChet, out DateTime? resultNgayConChet, asyncResult);
                            if (result)
                            {
                                SoConChet = resultSoConChet;
                                NgayConChet = resultNgayConChet;
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                            this.HideBusyIndicator();
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
    }
}
