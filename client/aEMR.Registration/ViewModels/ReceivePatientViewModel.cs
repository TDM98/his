using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.ComponentModel.DataAnnotations;
using System.ServiceModel;
using System.Threading;
using System.Windows;
using aEMR.CommonTasks;
using aEMR.DataContracts;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using Caliburn.Micro;
using DataEntities;
using aEMR.Common.BaseModel;
using eHCMS.Services.Core;
using aEMR.Common;
using aEMR.Common.Printing;
using eHCMSLanguage;
using Castle.Windsor;
using System.Windows.Input;
using aEMR.Common.HotKeyManagement;
using System.Windows.Controls;
using System.Linq;
/*
* 20170113 #001 CMN: Add QRCode
* 20170220 #002 CMN: Add Cancel Staff ID
* 20170522 #003 CMN: Added variable to check InPt 5 year HI without paid enough
* 20170927 #004 CMN: Reverted Code
* 20171026 #005 CMN: Added Find ConsultingDiagnosys for Surgery Registrations
* 20180116 #006 CMN: Added FiveYearsAppliedDate
* 20180524 #007 TTM:
* 28052018 #008 TxD:
* 20180926 #009 TTM: Thêm điều kiện nếu bệnh nhân có thẻ thuộc bệnh viện phát hành thẻ thì không cần kiểm tra giấy chuyển viện.
* 20180927 #010 TTM: Comment việc show popup xác nhận thông tin. Hiện tại ngoại trú không cần để confirm. Vì có hiện lên cũng chỉ click đồng ý mà thôi.
* 20181026 #011 TTM: Bổ sung Popup thông báo đã hoàn thành nhận bệnh bảo hiểm cho bệnh nhân.
* 20181026 #011 TTM: BM 0003236: Bổ sung Popup thông báo đã hoàn thành nhận bệnh bảo hiểm cho bệnh nhân.
* 20181026 #012 TTM: BM 0014314: Bổ sung điều kiện kiểm tra trẻ dưới 6 tuổi vào nút đăng ký nội trú => Vì đã Comment yield break trong lúc load thông tin bệnh nhân (Phải cho load để sửa chữa dữ liệu vào view)
* 20191105 #013 TBL: Task #1260: Thêm danh sách bệnh nhân vào OutstandingTask
* 20191126 #014 TBL: BM 0019644: Fix lỗi khi cập nhật thông tin bệnh nhân trong màn hình Xác nhận lại BHYT thì không thể Xác nhận BHYT được
* 20191205 #015 TBL: BM 0019684: Fix lỗi lấy mã đăng ký lên sai bên nhận bệnh nội trú
* 20200718 #016 TTM:   BM 0039388: Cấp số lần 2 cho bệnh nhân huỷ đăng ký.
* 20220823 #017 QTD: Tự đánh dấu Xác nhận cấp cứu nếu BN nhập viện với tình trạng cấp cứu màn hình xác nhận BHYT
*/

namespace aEMR.Registration.ViewModels
{
    [Export(typeof(IReceivePatient)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class ReceivePatientViewModel : ViewModelBase, IReceivePatient        // Conductor<object>
        , IHandle<ItemSelected<Patient>>
        , IHandle<CreateNewPatientEvent>
        , IHandle<HiCardConfirmedEvent>
        , IHandle<HiCardReloadEvent>
        , IHandle<AddCompleted<Patient>>
        , IHandle<ResultFound<Patient>>
        , IHandle<ResultNotFound<Patient>>
        , IHandle<ItemSelecting<object, PatientAppointment>>
        , IHandle<ItemSelected<PatientRegistration>>
        , IHandle<ConfirmHiBenefitEvent>
        //---------- DPT
        , IHandle<UpdateCompleted<Patient>>
        , IHandle<SaveHIAndInPtConfirmHICmd>
        , IHandle<InPatientRegistrationSelectedForConfirmHI>
        , IHandle<ReloadPtRegistrationCode>
        , IHandle<SetTicketIssueForPatientRegistrationView>
        , IHandle<SetTicketForNewRegistrationAgain>
    {
        IEventAggregator _eventArg;
        [ImportingConstructor]
        public ReceivePatientViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg)
        {
            base.HasInputBindingCmd = true;

            _eventArg = eventArg;
            // 02/12/13 TxD: ExamDate NOW get from Globals
            ExamDate = Globals.GetCurServerDateTime();
            _examDateServerInitVal = ExamDate;

            authorization();

            SearchRegistrationContent = Globals.GetViewModel<ISearchPatientAndRegistration>();
            SearchRegistrationContent.IsShowCallQMS = Globals.ServerConfigSection.CommonItems.UseQMSSystem;


            PatientDetailsContent = Globals.GetViewModel<IPatientDetails>();
            PatientDetailsContent.Parent = this;
            PatientDetailsContent.IsReceivePatient = true;

            /*  HPT: Không sử dụng LoginViewModel để lấy giá trị khoa phòng nữa vì rất rườm rà (05/09/2015)
              - Khi người dùng chọn khoa - phòng làm việc trong SelectLocationViewModel và click OK, VM này sẽ gán giá trị DeptLocation vào thuộc tính DeptLocation của LoginVM đồng thời bắn một sự kiện cho ShellViewModel, gán giá trị Globals.ObjRefDepartment và Globals.DeptLocation thông qua hàm 'public void Handle(LocationSelected message)'
              - [ShellViewModel là vm nền, được khởi tạo khi bắt đầu chương trình và vòng đời của nó chỉ kết thúc khi chương trình tắt đi. Do đó những biến lưu trong vm này sẽ được giữ và làm việc được trong suốt thời gian mở chương trình]            
              - Hàm dưới đây sử dụng loginVM để lấy ra giá trị DeptLocation rất dài dòng trong khi có thể lấy trực tiếp từ Globals sẽ ngắn gọn hơn nhiều
             */
            if (Globals.ObjRefDepartment != null)
            {
                _department = Globals.ObjRefDepartment;
            }
            else
            {
                _department = null;
            }
        }
        // HPT 04/09/2015: Thêm thuộc tính danh sách các khoa nội trú
        // Bệnh nhân đăng ký vãng lai và tiền giải phẫu phải chọn một khoa ngay khi đăng ký
        // Các bệnh nhân đăng ký nội trú khác không cần chọn, sẽ giấu combobox khoa đi
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
        // Hpt 01/10/2015: Cac dang ky Vang Lai va Tien Giai Phau khong duoc phep xac nhan cap cuu
        // Visibility_CheckboxCasualOrPreOp = false khi ViewModel duoc goi tu duong dan Vang Lai Hoac Tien Giai Phau, dieu khien an checkbox Xac nhan BN Cap cuu
        private bool _visibility_CheckboxCasualOrPreOp = true;
        public bool Visibility_CheckboxCasualOrPreOp
        {
            get
            {
                return _visibility_CheckboxCasualOrPreOp;
            }
            set
            {
                _visibility_CheckboxCasualOrPreOp = value;
                NotifyOfPropertyChange(() => Visibility_CheckboxCasualOrPreOp);
            }
        }

        private bool _IsRegForCasualOrPreOp;
        public bool IsRegForCasualOrPreOp
        {
            get
            {
                return _IsRegForCasualOrPreOp;
            }
            set
            {
                _IsRegForCasualOrPreOp = value;
                NotifyOfPropertyChange(() => IsRegForCasualOrPreOp);
            }
        }

        // HPT: Thêm thuộc tính để hỗ trợ tìm kiếm bệnh nhân theo loại đăng ký nội trú - vãng lai hoặc tiền giải phẫu)
        // Đăng ký vãng lai hoặc tiền giải phẫu sẽ có V_RegForPatientOfType là một số nguyên > 0, và đăng ký nội trú khác có V_RegForPatientOfType = 0
        private AllLookupValues.V_RegForPatientOfType _V_RegForPatientOfType = AllLookupValues.V_RegForPatientOfType.Unknown;
        public AllLookupValues.V_RegForPatientOfType V_RegForPatientOfType
        {
            get
            {
                return _V_RegForPatientOfType;
            }
            set
            {
                _V_RegForPatientOfType = value;
                NotifyOfPropertyChange(() => V_RegForPatientOfType);
            }
        }

        private AllLookupValues.PatientFindBy _PatientFindBy;
        public AllLookupValues.PatientFindBy PatientFindBy
        {
            get
            {
                return _PatientFindBy;
            }
            set
            {
                _PatientFindBy = value;
                NotifyOfPropertyChange(() => PatientFindBy);
                // Hpt 27/11/2015: Đã gán giá trị trong hàm khởi tạo rồi nhưng không có thời gian xem lại nên cứ để thêm một lần nữa ở đây, có thời gian sẽ xem lại và điều chỉnh 
                if (SearchRegistrationContent != null)
                {
                    SearchRegistrationContent.PatientFindBy = PatientFindBy;
                }
                PatientDetailsContent.PatientFindBy = (int)PatientFindBy; /*tma - 02/10/2017*/
            }
        }

        private InPatientAdmDisDetails _AdmDisDetails;
        public InPatientAdmDisDetails AdmDisDetails
        {
            get { return _AdmDisDetails; }
            set
            {
                if (_AdmDisDetails != value)
                {
                    _AdmDisDetails = value;
                    NotifyOfPropertyChange(() => AdmDisDetails);
                }
            }
        }

        // Hpt 02/10/2015: Vì bệnh nhân Vãng Lai và Tiền Giải Phẫu không được xác nhận cấp cứu nên khi làm việc với các đăng ký loại VL hoặc TGP thì phải giấu checkbox BN cấp cứu đi
        public void CanConfirmEmergency()
        {
            // hai duong dan duoc set V_RegForPatientOfType > 0 la Nhan Benh Vang Lai va Nhan Benh Tien Giai Phau
            // Cac duong dan khac goi den ViewModel nay deu co V_RegForPatientOfType = 0 (<=> unknown)
            if (RegistrationType == AllLookupValues.RegistrationType.NOI_TRU && V_RegForPatientOfType == AllLookupValues.V_RegForPatientOfType.DKBN_VANG_LAI)
            {
                PatientDetailsContent.ShowConfirmedEmergencyPatient = false;
            }
            // man hinh xac nhan BHYT dung xac nhan cho ca dang ky Vang Lai va tien Giai Phau nen phai dua vao V_RegForPatientOfType cua dang ky dang duoc thao tac de xac dinh co an checkbox di hay khong
            else if (RegistrationType == AllLookupValues.RegistrationType.XAC_NHAN_LAI_BHYT && CurRegistration != null && V_RegForPatientOfType == AllLookupValues.V_RegForPatientOfType.DKBN_VANG_LAI)
            {
                PatientDetailsContent.ShowConfirmedEmergencyPatient = false;
            }
            // Ngoai ra thi phai hien checkbox xac nhan Cap cuu len
            else
            {
                PatientDetailsContent.ShowConfirmedEmergencyPatient = true;
            }
        }
        public void GetDeptListContent()
        {

            //HPT: BN đăng ký Vãng Lai và Tiền Giải Phẫu phải chọn một khoa để được phục vụ (04/09/2015)
            //HPT: Vì đăng ký VL và TGP có thể được phục vụ cả khi không nhập viện nên phải chọn khoa ngay khi đăng ký
            //Đoạn cude dưới đây mở danh sách khoa cho người dùng chọn
            //Đăng ký nội trú được thực hiện bởi điều dưỡng khoa, điều dưỡng khoa nào chỉ được đăng ký cho BN vào khoa mà mình đã được cấu hình trách nhiệm
            //Vì vậy danh sách khoa cho người dùng chọn được lấy theo cấu hình trách nhiệm ngay khi người dùng vừa đăng nhập

            DepartmentContent = Globals.GetViewModel<IDepartmentListing>();
            DepartmentContent.AddSelectOneItem = true;
            DepartmentContent.LstRefDepartment = Globals.LoggedUserAccount.DeptIDResponsibilityList;
            DepartmentContent.LoadData();
        }
        // TxD 18/05/2015: The following method was added to allow for setting properties of the Content Controls after this ViewModel has been created ie. Constructor has been called
        public void InitViewContent()
        {
            var searchPatientAndRegVm = (ISearchPatientAndRegistration)SearchRegistrationContent;
            searchPatientAndRegVm.mTimBN = mPatient_TimBN;
            searchPatientAndRegVm.mThemBN = mPatient_ThemBN;
            searchPatientAndRegVm.mTimDangKy = mPatient_TimDangKy;
            searchPatientAndRegVm.InitButtonVisibility(SearchRegButtonsVisibility.SHOW_SEARCH_PATIENT_BTN | SearchRegButtonsVisibility.SHOW_NEW_PATIENT_BTN | SearchRegButtonsVisibility.SHOW_SEARCH_APPOINTMENT);
            searchPatientAndRegVm.SetDefaultButton(SearchRegistrationButtons.SEARCH_PATIENT);

            // TxD 08/01/2015: Set SearchAdmittedInPtRegOnly to null to allow for searching regardless of admitting status
            searchPatientAndRegVm.SearchAdmittedInPtRegOnly = null;

            if (RegistrationType == AllLookupValues.RegistrationType.NGOAI_TRU)
            {
                //PatientDetailsContent.ShowEmergInPtReExamination = true;
                PatientDetailsContent.ShowEmergInPtReExamination = Globals.ServerConfigSection.InRegisElements.ShowEmergInPtReExamination == false ? false : true;
            }
            else
            {
                mInfo_XemPhongKham = false;
                //PatientDetailsContent.ShowConfirmedForeignerPatient = true;
                PatientDetailsContent.ShowEmergInPtReExamination = false;
                CanConfirmEmergency();
                ISearchPatientAndRegistration vmSearReg = (ISearchPatientAndRegistration)SearchRegistrationContent;

                if (RegistrationType == AllLookupValues.RegistrationType.NOI_TRU)
                {
                    PatientDetailsContent.HITabVisible = false;
                }
                if (RegistrationType == AllLookupValues.RegistrationType.NOI_TRU || RegistrationType == AllLookupValues.RegistrationType.NOI_TRU_BHYT)
                {
                    vmSearReg.InitButtonVisibility(SearchRegButtonsVisibility.SHOW_SEARCH_PATIENT_BTN | SearchRegButtonsVisibility.SHOW_NEW_PATIENT_BTN);
                }

                if (RegistrationType == AllLookupValues.RegistrationType.XAC_NHAN_LAI_BHYT)
                {
                    vmSearReg.InitButtonVisibility(SearchRegButtonsVisibility.SHOW_SEARCH_REG_BTN);
                    vmSearReg.SetDefaultButton(SearchRegistrationButtons.SEARCH_REGISTRATION);
                    vmSearReg.PatientFindBy = AllLookupValues.PatientFindBy.NOITRU;
                    SearchRegistrationContent.SearchAdmittedInPtRegOnly = true;
                    SearchRegistrationContent.CanSearhRegAllDept = true;
                }
            }
            /*▼====: #005*/
            if (this.V_RegForPatientOfType == AllLookupValues.V_RegForPatientOfType.NBNT_BN_TIEN_PHAU_KHONG_BHYT || this.V_RegForPatientOfType == AllLookupValues.V_RegForPatientOfType.DKBN_DT_NGOAI_TRU)
            {
                searchPatientAndRegVm.EnableSerchConsultingDiagnosy = true;
            }
            /*▲====: #005*/
            ActivateItem(searchPatientAndRegVm);

            if (RegistrationType == AllLookupValues.RegistrationType.XAC_NHAN_LAI_BHYT)
            {
                PatientSummaryInfoContent = Globals.GetViewModel<IInPatientSummaryDetails>();
                PatientDetailsContent.LoginHIAPI();
            }
            else
            {
                PatientSummaryInfoContent = Globals.GetViewModel<IPatientSummaryInfoV2>();
            }
            GetDeptListContent();

            PatientSummaryInfoContent.mInfo_CapNhatThongTinBN = mInfo_CapNhatThongTinBN;
            PatientSummaryInfoContent.mInfo_XacNhan = mInfo_XacNhan;
            PatientSummaryInfoContent.mInfo_XoaThe = mInfo_XoaThe;
            PatientSummaryInfoContent.mInfo_XemPhongKham = mInfo_XemPhongKham;
            PatientSummaryInfoContent.DisplayButtons = false;
            ActivateItem(PatientSummaryInfoContent);

            PatientDetailsContent.ShowCloseFormButton = false;

            PatientDetailsContent.mNhanBenh_ThongTin_Sua = mNhanBenh_ThongTin_Sua;
            PatientDetailsContent.mNhanBenh_TheBH_ThemMoi = mNhanBenh_TheBH_ThemMoi;
            PatientDetailsContent.mNhanBenh_TheBH_XacNhan = mNhanBenh_TheBH_XacNhan;
            PatientDetailsContent.mNhanBenh_DangKy = mNhanBenh_DangKy;
            PatientDetailsContent.mNhanBenh_TheBH_Sua = mNhanBenh_TheBH_Sua;

            ActivateItem(PatientDetailsContent);
        }

        private void ResetView()
        {
            //PatientSummaryInfoContent.ConfirmedPaperReferal = null;            
            //PatientSummaryInfoContent.ConfirmedHiItem = null;
            //PatientSummaryInfoContent.HiBenefit = null;

            PatientSummaryInfoContent.SetPatientHISumInfo(null);
        }

        // 02/12/13 TxD: ONLY SET ExamDate at the very last just before Saving
        //public void getdate()
        //{
        //    Coroutine.BeginExecute(DoGetTime());
        //}

        //public IEnumerator<IResult> DoGetTime()
        //{
        //    var loadCurrentDate = new LoadCurrentDateTask();
        //    yield return loadCurrentDate;
        //    DateTime today;
        //    if (loadCurrentDate.CurrentDate == DateTime.MinValue)
        //    {
        //        today = DateTime.Now.Date;
        //        ExamDate = DateTime.Now.Date;
        //        _msgTask = new MessageBoxTask(string.Format("{0}.", eHCMSResources.Z0144_G1_KhongTheLayNgThTuServer), eHCMSResources.G0442_G1_TBao);
        //        yield return _msgTask;
        //    }
        //    else
        //    {
        //        today = loadCurrentDate.CurrentDate;
        //        ExamDate = loadCurrentDate.CurrentDate;
        //    }
        //}

        protected override void OnActivate()
        {
            base.OnActivate();

            // 02/12/13 TxD: ExamDate is NOW get from Globals
            ExamDate = Globals.GetCurServerDateTime();
            _examDateServerInitVal = ExamDate;
            //getdate();

            authorization();
            Globals.EventAggregator.Subscribe(this);
            //▼===== #013
            if (RegistrationType == AllLookupValues.RegistrationType.XAC_NHAN_LAI_BHYT)
            {
                var homeVm = Globals.GetViewModel<IHome>();
                IInPatientOutstandingTask ostvm = Globals.GetViewModel<IInPatientOutstandingTask>();
                homeVm.OutstandingTaskContent = ostvm;
                homeVm.IsExpandOST = true;
                ostvm.WhichVM = SetOutStandingTask.XACNHAN_BHYT;
            }
            //▲===== #013
        }
        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);
            Globals.EventAggregator.Unsubscribe(this);
            //▼===== #013: Bổ sung task #1260 huỷ OST khi ra khỏi màn hình (BM 0019590: Lỗi hiển thị danh sách BN điều trị nội trú(OutstandingTask) sai chổ. Là BM bổ sung lỗi khi phát hiện)
            if (RegistrationType == AllLookupValues.RegistrationType.XAC_NHAN_LAI_BHYT)
            {
                var homeVm = Globals.GetViewModel<IHome>();
                homeVm.OutstandingTaskContent = null;
                homeVm.IsExpandOST = false;
            }
            //▲===== #013
        }

        private ISearchPatientAndRegistration _searchRegistrationContent;

        public ISearchPatientAndRegistration SearchRegistrationContent
        {
            get { return _searchRegistrationContent; }
            set
            {
                _searchRegistrationContent = value;
                NotifyOfPropertyChange(() => SearchRegistrationContent);
            }
        }

        public bool ConfirmRemoveHIChecked
        {
            get;
            set;
        }

        private string _pageTitle;
        public string PageTitle
        {
            get
            {
                return _pageTitle;
            }
            set
            {
                _pageTitle = value;
                NotifyOfPropertyChange(() => PageTitle);
            }
        }

        private string _DeptLocTitle;
        public string DeptLocTitle
        {
            get
            {
                return _DeptLocTitle;
            }
            set
            {
                _DeptLocTitle = value;
                NotifyOfPropertyChange(() => DeptLocTitle);
            }
        }

        public string Temp38
        {
            get
            {
                if (RegistrationType == AllLookupValues.RegistrationType.NGOAI_TRU)
                {
                    return eHCMSResources.Z0641_G1_InMau01BV;
                }
                else
                {

                    return eHCMSResources.Z0642_G1_InMau02BV;
                }
            }

        }
        //Temp38
        //private IPatientSummaryInfoV2 _patientSummaryInfoContent;

        //public IPatientSummaryInfoV2 PatientSummaryInfoContent
        //{
        //    get { return _patientSummaryInfoContent; }
        //    set
        //    {
        //        _patientSummaryInfoContent = value;
        //        NotifyOfPropertyChange(() => PatientSummaryInfoContent);
        //    }
        //}


        private IPatientSummaryInfoCommon _patientSummaryInfoContent;

        public IPatientSummaryInfoCommon PatientSummaryInfoContent
        {
            get { return _patientSummaryInfoContent; }
            set
            {
                _patientSummaryInfoContent = value;
                NotifyOfPropertyChange(() => PatientSummaryInfoContent);
            }
        }

        private IPatientDetails _patientDetailsContent;
        public IPatientDetails PatientDetailsContent
        {
            get { return _patientDetailsContent; }
            set
            {
                _patientDetailsContent = value;
                NotifyOfPropertyChange(() => PatientDetailsContent);
            }
        }

        private bool _mCuocHen_TraiHen;
        public bool mCuocHen_TraiHen
        {
            get { return _mCuocHen_TraiHen; }
            set
            {
                _mCuocHen_TraiHen = value;
                NotifyOfPropertyChange(() => mCuocHen_TraiHen);
            }
        }

        private bool _mCuocHen_DungHen;
        public bool mCuocHen_DungHen
        {
            get { return _mCuocHen_DungHen; }
            set
            {
                _mCuocHen_DungHen = value;
                NotifyOfPropertyChange(() => mCuocHen_DungHen);
            }
        }

        private Status _TitleStatus = Status.None;
        public Status TitleStatus
        {
            get { return _TitleStatus; }
            set
            {
                _TitleStatus = value;
                NotifyOfPropertyChange(() => TitleStatus);
                switch (TitleStatus)
                {
                    case Status.None:
                        mCuocHen_DungHen = false;
                        mCuocHen_TraiHen = false;
                        break;
                    case Status.CuocHen_DungHen:
                        mCuocHen_DungHen = true;
                        mCuocHen_TraiHen = false;
                        break;
                    case Status.CuocHen_TraiHen:
                        mCuocHen_DungHen = false;
                        mCuocHen_TraiHen = true;
                        break;
                }
            }
        }

        /// <summary>
        /// Loại đăng ký (Nhận bệnh cho đăng ký nội trú hay ngoại trú)
        /// </summary>
        private AllLookupValues.RegistrationType _registrationType = AllLookupValues.RegistrationType.Unknown;
        public AllLookupValues.RegistrationType RegistrationType
        {
            get
            {
                return _registrationType;
            }
            set
            {
                _registrationType = value;
                if (PatientDetailsContent != null)
                {
                    PatientDetailsContent.RegistrationType = value;

                    // TxD 18/05/2015: The following commented out block of code has been moved to the newly added method InitViewContent
                    //      
                    //if (value == AllLookupValues.RegistrationType.NGOAI_TRU)
                    //{
                    //    PatientDetailsContent.ShowEmergInPtReExamination = true;    
                    //}

                    //// TxD: Nhan Benh Noi Tru or Cap Cuu khong BHYT initially
                    //if (value == AllLookupValues.RegistrationType.NOI_TRU)
                    //{
                    //    //PatientDetailsContent.ActivationMode = ActivationMode.EDIT_PATIENT_GENERAL_INFO;
                    //    PatientDetailsContent.HITabVisible = false;                        
                    //}

                    //if (value == AllLookupValues.RegistrationType.NOI_TRU || value == AllLookupValues.RegistrationType.NOI_TRU_BHYT)
                    //{
                    //    ISearchPatientAndRegistration vmSearReg = (ISearchPatientAndRegistration)SearchRegistrationContent;
                    //    vmSearReg.InitButtonVisibility(SearchRegButtonsVisibility.SHOW_SEARCH_PATIENT_BTN | SearchRegButtonsVisibility.SHOW_NEW_PATIENT_BTN);
                    //    PatientDetailsContent.ShowConfirmedEmergencyPatient = true;
                    //    if (value == AllLookupValues.RegistrationType.NOI_TRU)
                    //    {
                    //        PatientDetailsContent.ShowConfirmedForeignerPatient = true;
                    //    }
                    //    mInfo_XemPhongKham = false;
                    //}

                    //if (value == AllLookupValues.RegistrationType.XAC_NHAN_LAI_BHYT)
                    //{
                    //    ISearchPatientAndRegistration vmSearReg = (ISearchPatientAndRegistration)SearchRegistrationContent;
                    //    vmSearReg.InitButtonVisibility(SearchRegButtonsVisibility.SHOW_SEARCH_REG_BTN);
                    //    vmSearReg.SetDefaultButton(SearchRegistrationButtons.SEARCH_REGISTRATION);
                    //    vmSearReg.PatientFindBy = AllLookupValues.PatientFindBy.NOITRU;
                    //    mInfo_XemPhongKham = false;
                    //    PatientDetailsContent.ShowConfirmedEmergencyPatient = true;
                    //    ((ISearchPatientAndRegistration)SearchRegistrationContent).CanSearhRegAllDept = true;
                    //    ((ISearchPatientAndRegistration)SearchRegistrationContent).SearchAdmittedInPtRegOnly = true;
                    //}

                }
            }
        }

        public bool IsEmergency { get; set; }
        public bool IsForeigner { get; set; }
        public bool EmergInPtReExamination { get; set; }
        public long V_ObjectMedicalExamination { get; set; }

        private PatientAppointment _currentAppointment;

        private Patient _currentPatient;

        public Patient CurrentPatient
        {
            get { return _currentPatient; }
            set
            {
                _currentPatient = value;
                NotifyOfPropertyChange(() => CurrentPatient);
                NotifyOfPropertyChange(() => CanCreateRegistrationCmd);

                // TxD 28/09/2017: PatientDetailsContent.CurrentPatient is NOT required to be SET here because it is SET by another method
                /*▼====: #004*/
                PatientSummaryInfoContent.CurrentPatient = _currentPatient;
                /*▲====: #004*/
                //ExamDate = DateTime.Now;
                NotifyOfPropertyChange(() => CanReportRegistrationInfoInsuranceCmd);
                NotifyOfPropertyChange(() => CanInTemplate38Cmd);
            }
        }

        private HealthInsurance _confirmedHiItem;
        /// <summary>
        /// Thông tin thẻ bảo hiểm đã được confirm
        /// </summary>
        public HealthInsurance ConfirmedHiItem
        {
            get
            {
                return _confirmedHiItem;
            }
            set
            {
                _confirmedHiItem = value;
                NotifyOfPropertyChange(() => ConfirmedHiItem);
                if (PatientSummaryInfoContent != null)/*TMA*/
                {
                    //PatientSummaryInfoContent.ConfirmedHiItem = _confirmedHiItem;

                    //PatientSummaryInfoContent.HiBenefit = null;
                    NotifyOfPropertyChange(() => CanCreateRegistrationCmd);
                    PatientSummaryInfoContent.CurrentPatientClassification = CreateDefaultClassification();
                }
            }
        }
        private PatientClassification CreateDefaultClassification()
        {
            if (ConfirmedHiItem != null)
            {
                return PatientClassification.CreatePatientClassification((long)PatientType.INSUARED_PATIENT, "");
            }
            return PatientClassification.CreatePatientClassification((long)PatientType.NORMAL_PATIENT, "");
        }
        private PaperReferal _confirmedPaperReferal;
        /// <summary>
        /// Thông tin giấy chuyển viện đã được confirm
        /// </summary>
        public PaperReferal ConfirmedPaperReferal
        {
            get
            {
                return _confirmedPaperReferal;
            }
            set
            {
                _confirmedPaperReferal = value;
                NotifyOfPropertyChange(() => ConfirmedPaperReferal);
                ///*TMA*/
                //    if (PatientSummaryInfoContent != null) PatientSummaryInfoContent.ConfirmedPaperReferal = _confirmedPaperReferal;
                ///*TMA*/
            }
        }

        private RegistrationFormMode _currentRegMode = RegistrationFormMode.PATIENT_NOT_SELECTED;
        public RegistrationFormMode CurrentRegMode
        {
            get
            {
                return _currentRegMode;
            }
            set
            {
                if (_currentRegMode != value)
                {
                    _currentRegMode = value;
                    NotifyOfPropertyChange(() => CurrentRegMode);
                    NotifyOfPropertyChange(() => CanCreateRegistrationCmd);
                }
            }
        }

        public void ConfirmHIItem(object hiItem)
        {
            ConfirmedHiItem = hiItem as HealthInsurance;
        }
        public void ConfirmPaperReferal(object referal)
        {
            ConfirmedPaperReferal = referal as PaperReferal;
        }

        public void Handle(ItemSelected<Patient> message)
        {
            if (this.IsActive && message != null)
            {
                IsEmergency = false;
                IsForeigner = false;
                EmergInPtReExamination = false;
                // TxD 28/09/2017 : No NEED to SET CurrentPatient here because it is SET in another method afterward
                CurrentPatient = message.Item; /*TMA - 28/09/2017 : mở lại vì ko load được thông tin bệnh nhân vào detail */
                if (RegistrationType == AllLookupValues.RegistrationType.NGOAI_TRU)
                {
                    Coroutine.BeginExecute(DoSetCurrentPatient(message.Item));
                }
                else
                {
                    Coroutine.BeginExecute(DoSetCurrentPatient_InPt(message.Item));
                }
            }
        }

        MessageBoxTask _msgTask;
        WarningWithConfirmMsgBoxTask warnConfDlg = null;
        private IEnumerator<IResult> DoSetCurrentPatient(Patient p, bool bFromCancelRegistration = false)
        {
            // TxD 12/03/2018: Added the following to clear HI Details of any previous Patient 
            // If NOT working completely then REVIEW is required ....
            PatientSummaryInfoContent.SetPatientHISumInfo(null);

            CanCreateNewRegistration = true;
            CanCancelRegistrationInfoCmd = false;

            if (PatientDetailsContent != null && PatientDetailsContent.ShowConfirmedEmergencyPatient)
            {
                PatientDetailsContent.IsConfirmedEmergencyPatient = false;
            }
            if (PatientDetailsContent != null && PatientDetailsContent.ShowConfirmedForeignerPatient)
            {
                PatientDetailsContent.IsConfirmedForeignerPatient = false;
            }
            if (PatientDetailsContent != null && PatientDetailsContent.ShowEmergInPtReExamination)
            {
                PatientDetailsContent.EmergInPtReExamination = false;
            }

            // 02/12/13 TxD: ExamDate NOW get from Globals
            ExamDate = Globals.ServerDate.Value;
            _examDateServerInitVal = ExamDate;

            ConfirmedHiItem = null;
            ConfirmedPaperReferal = null;
            if (p == null || p.PatientID <= 0)
            {
                _currentAppointment = null;
                CurrentPatient = null;
                CurrentRegMode = RegistrationFormMode.PATIENT_NOT_SELECTED;
                yield break;
            }
            else
            {
                //KMx: Nếu không phải từ cuộc hẹn gọi thì set _currentAppointment = null. Tránh trường hợp khi tìm bệnh nhân, người sau lấy cuộc hẹn của người trước đi lưu (16/04/2014 17:06).
                if (!p.FromAppointment)
                {
                    _currentAppointment = null;
                }
            }

            PatientLoaded = false;
            PatientLoading = true;
            var loadPatient = new LoadPatientTask(p.PatientID, false, false, true);
            yield return loadPatient;
            //HPT 24/08/2015 : vì đăng ký ngoại trú không set giá trị cho CurRegistration nên phải dùng hàm này để gán giá trị cho biến IsHICard_FiveYearsCont sau khi thực hiện loadPatient nếu không giá trị của biến sẽ sai dẫn đến checkbox Xác nhận BN có BHYT 5 năm liên tiếp cũng sẽ sai 
            if (loadPatient.CurrentPatient != null && loadPatient.CurrentPatient.latestHIRegistration != null)
            {
                IsHICard_FiveYearsCont = loadPatient.CurrentPatient.latestHIRegistration.IsHICard_FiveYearsCont;
                IsChildUnder6YearsOld = loadPatient.CurrentPatient.latestHIRegistration.IsChildUnder6YearsOld;
                /*==== #003 ====*/
                IsHICard_FiveYearsCont_NoPaid = loadPatient.CurrentPatient.latestHIRegistration.IsHICard_FiveYearsCont_NoPaid;
                FiveYearsAppliedDate = loadPatient.CurrentPatient.latestHIRegistration.FiveYearsAppliedDate;
                FiveYearsARowDate = loadPatient.CurrentPatient.latestHIRegistration.FiveYearsARowDate;
                /*==== #003 ====*/
            }

            Globals.EventAggregator.Publish(new PatientReloadEvent { curPatient = loadPatient.CurrentPatient });
            // 03/12/2013 TxD : The following Code lines have been commented out except today
            // because Examdate NOW get from Globals.
            DateTime today = Globals.ServerDate.Value.Date;

            if (RegistrationType == AllLookupValues.RegistrationType.NGOAI_TRU)
            {

                if (loadPatient.CurrentPatient == null)
                {
                    CurrentPatient = null;
                    CurrentRegMode = RegistrationFormMode.PATIENT_NOT_SELECTED;

                    _msgTask = new MessageBoxTask(eHCMSResources.Z0146_G1_KhongTheLayTTinBN, eHCMSResources.G0442_G1_TBao);
                    yield return _msgTask;
                    yield break;
                }


                bool bAlreadyRegistered = false;

                if (loadPatient.CurrentPatient.latestHIRegistration != null && loadPatient.CurrentPatient.latestHIRegistration.EmergInPtReExamination.HasValue)
                {
                    if (PatientDetailsContent != null && PatientDetailsContent.ShowEmergInPtReExamination)
                    {
                        PatientDetailsContent.EmergInPtReExamination = loadPatient.CurrentPatient.latestHIRegistration.EmergInPtReExamination.GetValueOrDefault(false);
                        IsAllowCrossRegion = loadPatient.CurrentPatient.LatestRegistration.IsAllowCrossRegion;
                    }
                }

                if (loadPatient.CurrentPatient.latestHIRegistration != null && loadPatient.CurrentPatient.latestHIRegistration.RegistrationStatus != AllLookupValues.RegistrationStatus.REFUND)
                {
                    var regDate = loadPatient.CurrentPatient.latestHIRegistration.ExamDate;

                    //Kiểm tra xem có được phép tạo mới ĐKBH hay không?
                    if (AxHelper.CompareDate(regDate, today) != 1 && AxHelper.CompareDate(regDate.AddDays(ConfigValues.PatientRegistrationTimeout), today) != 2)
                    {
                        bAlreadyRegistered = true;

                        string sLoaiDK = Globals.GetTextV_RegistrationType((long)loadPatient.CurrentPatient.latestHIRegistration.V_RegistrationType);
                        if (!string.IsNullOrEmpty(sLoaiDK))
                        {
                            sLoaiDK = eHCMSResources.Z0028_G1_DauNgoacTrai.ToUpper() + sLoaiDK + ")";
                        }

                        //chi thong bao thoi
                        _msgTask = new MessageBoxTask(string.Format("{0} {1}!", eHCMSResources.Z0643_G1_BNDaDK, sLoaiDK), eHCMSResources.G0442_G1_TBao, Infrastructure.MessageBoxOptions.Ok);
                        yield return _msgTask;

                        CanCreateNewRegistration = false; //dinh them nho bo ra
                        CanCancelRegistrationInfoCmd = true;
                    }
                }
                //KMx: (16/04/2014 12:03)
                //FromAppointment: Có phải là pop-up Danh sách cuộc hẹn gọi hay không? Nếu là pop-up gọi thì không load cuộc hẹn nữa.
                //bAlreadyRegistered: Trong thời gian cho phép, đã có ĐKBH chưa? Nếu đã có ĐKBH rồi thì không load cuộc hẹn.
                //bFromCancelRegistration: Sau khi hủy đăng ký thì không load cuộc hẹn nữa.

                //▼===== 20200611 TTM: Bổ sung điều kiện để khi trong màn hình nhận bệnh tư vấn chăm sóc khách hàng tìm bệnh nhân sẽ không load cuộc hẹn.
                //IsShowGetTicketButton: Biến này để nhận biến là ReceivePatient đang cho trường hợp nhận bệnh tư vấn chăm sóc khách hàng => nên không cần load cuộc hẹn của bệnh nhân

                if (!IsShowGetTicketButton && !p.FromAppointment && !bAlreadyRegistered && !bFromCancelRegistration && loadPatient.CurrentPatient.AppointmentList != null && loadPatient.CurrentPatient.AppointmentList.Count > 0)
                {
                    //Nếu có 1 cuộc hẹn đã xác nhận và đúng hẹn.
                    if (loadPatient.CurrentPatient.AppointmentList.Count == 1 && loadPatient.CurrentPatient.AppointmentList[0].ApptDate.Value.Date == today)
                    {
                        //var result = MessageBox.Show("Đăng kí bảo hiểm cho cuộc hẹn hay đăng kí mới? \n Chọn 'OK' để đăng kí bảo hiểm cho cuộc hẹn. \n Chọn 'Cancel' để đăng kí mới.", "Thông báo quan trọng", MessageBoxButton.OKCancel);
                        //if (result == MessageBoxResult.OK)
                        //{
                        //    _currentAppointment = loadPatient.CurrentPatient.AppointmentList[0].DeepCopy();
                        //    _currentAppointment.Patient = new Patient();
                        //    _currentAppointment.Patient.PatientID = loadPatient.CurrentPatient.PatientID;
                        //}
                        string message = "";
                        if (loadPatient.CurrentPatient.AppointmentList[0].IsEmergInPtReExamApp)
                        {
                            message = eHCMSResources.Z0205_G1_XNhanTaoDKBNCapCuuTKCoBHYT;
                        }
                        else
                        {
                            message = eHCMSResources.Z0206_G1_XNhanTaoDKBHYTTuCuocHen;
                        }
                        warnConfDlg = new WarningWithConfirmMsgBoxTask(message, eHCMSResources.Z0207_G1_XNhanDKBHYT);
                        yield return warnConfDlg;
                        if (warnConfDlg.IsAccept)
                        {
                            _currentAppointment = loadPatient.CurrentPatient.AppointmentList[0].DeepCopy();
                            _currentAppointment.Patient = new Patient();
                            _currentAppointment.Patient.PatientID = loadPatient.CurrentPatient.PatientID;
                            EmergInPtReExamination = _currentAppointment.IsEmergInPtReExamApp;
                        }
                        warnConfDlg = null;
                    }
                    else    //Nếu có 1 cuộc hẹn không đúng hẹn hoặc nhiều hơn 1 cuộc hẹn đã xác nhận
                    {
                        //Open pop-up cho user chọn cuộc hẹn
                        Action<IFindAppointment> onInitDlg = delegate (IFindAppointment vm)
                        {
                            if (!string.IsNullOrEmpty(_currentPatient.PatientCode))
                            {
                                vm.SearchCriteria.PatientCode = _currentPatient.PatientCode;
                                vm.SearchCriteria.V_ApptStatus = (long)AllLookupValues.ApptStatus.BOOKED;
                                vm.SearchCmd();
                            }
                        };
                        GlobalsNAV.ShowDialog<IFindAppointment>(onInitDlg);
                    }
                }
                //if (TicketIssueObj != null && TicketIssueObj.V_TicketStatus == (int)V_TicketStatus_Enum.TKT_ALREADY_REGIS)
                //{
                //    TicketIssueObj = null;
                //}
            }
            else // In Patient Registration
            {
                if (loadPatient.CurrentPatient.LatestRegistration_InPt != null)
                {
                    ShowOldRegistration(loadPatient.CurrentPatient.LatestRegistration_InPt);
                    if (RegistrationType != AllLookupValues.RegistrationType.XAC_NHAN_LAI_BHYT && loadPatient.CurrentPatient.LatestRegistration_InPt.DischargeDate == null)
                    {
                        MessageBox.Show(eHCMSResources.A0261_G1_Msg_InfoBNDaDKNoiTru);
                        CanCreateNewRegistration = false;
                        if (PatientDetailsContent != null && PatientDetailsContent.ShowConfirmedEmergencyPatient)
                        {
                            PatientDetailsContent.IsConfirmedEmergencyPatient = (loadPatient.CurrentPatient.LatestRegistration_InPt.EmergRecID > 0 ? true : false);
                        }
                        if (PatientDetailsContent != null && PatientDetailsContent.ShowConfirmedForeignerPatient)
                        {
                            PatientDetailsContent.IsConfirmedForeignerPatient = loadPatient.CurrentPatient.LatestRegistration_InPt.IsForeigner.GetValueOrDefault(false);
                        }
                    }
                    else
                    {
                        CanCreateNewRegistration = true;
                    }
                }
            }

            CurrentPatient = ObjectCopier.DeepCopy(loadPatient.CurrentPatient);

            // TxD 29/09/2018: For some unknow reason, could well be negligence that CurRegistration is not SET HERE 
            //                  so JUST SET IT if something weird happens than TBR
            CurRegistration = loadPatient.CurrentPatient.LatestRegistration;

            CurrentRegMode = RegistrationFormMode.PATIENT_SELECTED;

            if (_currentAppointment != null && _currentAppointment.AppointmentID > 0 && _currentAppointment.ApptDate != null)
            {
                if (_currentAppointment.ApptDate.Value.Date == today)
                {
                    TitleStatus = Status.CuocHen_DungHen;
                }
                else
                {
                    TitleStatus = Status.CuocHen_TraiHen;
                }
            }
            else
            {
                TitleStatus = Status.None;
            }

            // Txd 14/07/2014: Commented the following and replaced with GenericCoRoutineTask            
            // PatientDetailsContent.StartEditingPatientLazyLoad(loadPatient.CurrentPatient);
            //==== #001
            CurrentPatient.QRCode = p.QRCode;
            //==== #001
            var LoadPtDetailsAndHI = new GenericCoRoutineTask(PatientDetailsContent.LoadPatientDetailsAndHI_GenAction, CurrentPatient, true);

            yield return LoadPtDetailsAndHI;

            //KMx: Trong quá trình load thẻ BH bị lỗi thì set Patient = null, tránh trường hợp thông tin của người A, mà thẻ BH người B. Dẫn đến đăng ký bị lỗi (29/10/2014 10:46).
            if (LoadPtDetailsAndHI.Error != null)
            {
                CurrentPatient = null;
                yield break;
            }

            ConfirmedHiItem = null;
            ConfirmedPaperReferal = null;

            if (CurrentPatient != null)
            {
                if (CurrentPatient.latestHIRegistration != null && CurrentPatient.latestHIRegistration.PtRegistrationID > 0 && CanCreateNewRegistration == false)
                {
                    Globals.HIRegistrationForm = "";
                }
                else
                {
                    Globals.HIRegistrationForm = string.Format("{0} ", eHCMSResources.Z0208_G1_ChưaDKChoBN) + CurrentPatient.FullName + string.Format(" {0}", eHCMSResources.A0138_G1_Msg_ConfBoQua);
                }

                // TxD 28/09/2017 NOTE: PatientDetailsContent.CurrentPatient is NOT required to be SET here because it is SET by another method
            }
        }

        private IEnumerator<IResult> DoSetCurrentPatient_InPt(Patient p, bool bFromCancelRegistration = false)
        {
            CanCreateNewRegistration = true;
            CanCancelRegistrationInfoCmd = false;
            CurRegistration = new PatientRegistration();
            if (PatientDetailsContent != null && PatientDetailsContent.ShowConfirmedEmergencyPatient)
            {
                PatientDetailsContent.IsConfirmedEmergencyPatient = false;
            }
            if (PatientDetailsContent != null && PatientDetailsContent.ShowConfirmedForeignerPatient)
            {
                PatientDetailsContent.IsConfirmedForeignerPatient = false;
            }

            // 02/12/13 TxD: ExamDate NOW get from Globals
            ExamDate = Globals.ServerDate.Value;
            _examDateServerInitVal = ExamDate;

            ConfirmedHiItem = null;
            ConfirmedPaperReferal = null;
            _currentAppointment = null;

            if (p == null || p.PatientID <= 0)
            {
                CurrentPatient = null;
                CurrentRegMode = RegistrationFormMode.PATIENT_NOT_SELECTED;
                yield break;
            }
            if (p.PatientID > 0)
            {
                //---------DPT trẻ em <72 tháng tuổi
                DateTime loadCurrentDate = Globals.ServerDate.Value;
                int monthnew = 0;
                monthnew = (loadCurrentDate.Month + loadCurrentDate.Year * 12) - (Convert.ToDateTime(CurrentPatient.DOB).Month + Convert.ToDateTime(CurrentPatient.DOB).Year * 12);
                if (CurrentPatient.AgeOnly == true && ((loadCurrentDate.Year - Convert.ToDateTime(CurrentPatient.DOB).Year) <= 6))
                {
                    MessageBox.Show(eHCMSResources.Z2643_G1_KhongDuThongTinTreDuoi6Tuoi);
                    //▼===== #012: 20190907 TTM: Chỉ cần thông báo lên cho người ta biết không cần phải ngăn lại nếu ngăn lại sẽ 
                    //                      => load đc PatientInfo mà không load thông tin vào view để user chỉnh sửa dữ liệu.
                    //yield break;
                }
                else if (CurrentPatient.AgeOnly == false && monthnew <= 72)
                {
                    if (CurrentPatient.FContactFullName == null && (CurrentPatient.V_FamilyRelationship == null || CurrentPatient.V_FamilyRelationship == 0))
                    {
                        MessageBox.Show(eHCMSResources.Z2644_G1_KhongDuThongTinNguoiThanTreDuoi6Tuoi);
                        //▼===== #012: 20190907 TTM: Chỉ cần thông báo lên cho người ta biết không cần phải ngăn lại nếu ngăn lại sẽ 
                        //                      => load đc PatientInfo mà không load thông tin vào view để user chỉnh sửa dữ liệu.
                        //yield break;
                    }

                }
            }


            TitleStatus = Status.None;

            var LoadPtDetailsAndHI = new GenericCoRoutineTask(PatientDetailsContent.LoadPatientDetailsAndHI_GenAction, CurrentPatient, false);

            yield return LoadPtDetailsAndHI;

            //KMx: Trong quá trình load thẻ BH bị lỗi thì set Patient = null, tránh trường hợp thông tin của người A, mà thẻ BH người B. Dẫn đến đăng ký bị lỗi (29/10/2014 10:46).
            if (LoadPtDetailsAndHI.Error != null)
            {
                CurrentPatient = null;
                yield break;
            }

            PatientLoaded = false;
            PatientLoading = true;
            var loadPatient = new LoadPatientTask(p.PatientID);
            yield return loadPatient;

            CurrentPatient = loadPatient.CurrentPatient;
            /*▼====: #005*/
            if (p.ConsultingDiagnosysID > 0)
                CurrentPatient.ConsultingDiagnosysID = p.ConsultingDiagnosysID;
            /*▲====: #005*/

            if (loadPatient.CurrentPatient.LatestRegistration_InPt != null
                && loadPatient.CurrentPatient.LatestRegistration_InPt.RegistrationStatus != AllLookupValues.RegistrationStatus.REFUND
                && loadPatient.CurrentPatient.LatestRegistration_InPt.RegistrationStatus != AllLookupValues.RegistrationStatus.COMPLETED)
            {
                CurrentRegMode = RegistrationFormMode.PATIENT_NOT_SELECTED;
                //▼===== #014
                //20191126 TBL: Khi đăng ký được load lại nhưng không lấy thông tin nhập viện thì set cho để khi xác nhận BHYT có kiểm tra nhập viện
                if (loadPatient.CurrentPatient.LatestRegistration_InPt.AdmissionInfo == null && AdmDisDetails != null && AdmDisDetails.InPatientAdmDisDetailID > 0)
                {
                    loadPatient.CurrentPatient.LatestRegistration_InPt.AdmissionInfo = AdmDisDetails;
                }
                //▲===== #014
                ShowOldRegistration(loadPatient.CurrentPatient.LatestRegistration_InPt);
                // TxD 14/05/2015 Mod the following because Xac Nhan BHYT DOES NOT GET INTO this method AT ALL
                //if (RegistrationType != AllLookupValues.RegistrationType.XAC_NHAN_LAI_BHYT &&
                //        (loadPatient.CurrentPatient.LatestRegistration_InPt.DischargeDate == null && loadPatient.CurrentPatient.LatestRegistration_InPt.TempDischargeDate == null))

                // 20190927 TNHX: BN đã có đăng ký nội trú thì không hiển thị thông báo nữa
                // Dang ky cuoi cung duoc load len la dang ky noi tru hoac dang ky tien giai phau da nhap vien - chua xuat 
                if (CurRegistration.V_RegForPatientOfType == AllLookupValues.V_RegForPatientOfType.Unknown && RegistrationType != AllLookupValues.RegistrationType.XAC_NHAN_LAI_BHYT) //20191126 TBL: Nếu ở màn hình Xác nhận lại BHYT thì không cần vào hàm này
                {
                    // (1) Nếu đăng ký chưa xuất viện thì thông báo và không cho phép tạo mới đăng ký nữa
                    if (CurRegistration.DischargeDate == null)
                    {
                        MessageBox.Show(eHCMSResources.A0261_G1_Msg_InfoBNDaDKNoiTru);
                        CanCreateNewRegistration = false;
                        CanCancelRegistrationInfoCmd = true;
                        if (PatientDetailsContent != null && PatientDetailsContent.ShowConfirmedEmergencyPatient)
                        {
                            PatientDetailsContent.IsConfirmedEmergencyPatient = (loadPatient.CurrentPatient.LatestRegistration_InPt.EmergRecID > 0 ? true : false);
                        }
                        if (PatientDetailsContent != null && PatientDetailsContent.ShowConfirmedForeignerPatient)
                        {
                            PatientDetailsContent.IsConfirmedForeignerPatient = loadPatient.CurrentPatient.LatestRegistration_InPt.IsForeigner.GetValueOrDefault(false);
                        }
                        Globals.HIRegistrationForm = "";
                    }
                    // Ngược lại thì mở đăng ký lên cho phép đăng ký mới
                    else
                    {
                        CurrentRegMode = RegistrationFormMode.PATIENT_SELECTED;
                        Globals.HIRegistrationForm = string.Format("{0} {1}. \n{2}", eHCMSResources.Z0208_G1_ChưaDKChoBN, CurrentPatient.FullName, eHCMSResources.A0138_G1_Msg_ConfBoQua);
                    }
                    yield break;
                }
                if (CurRegistration.V_RegForPatientOfType == AllLookupValues.V_RegForPatientOfType.NBNT_BN_TIEN_PHAU_KHONG_BHYT || CurRegistration.V_RegForPatientOfType == AllLookupValues.V_RegForPatientOfType.DKBN_DT_NGOAI_TRU)
                {
                    if (CurRegistration.AdmissionDate == null && !Globals.Check_CasualAndPreOpReg_StillValid(CurRegistration))
                    {
                        warnConfDlg = new WarningWithConfirmMsgBoxTask(eHCMSResources.Z0209_G1_BNDangCóDKGPhauKTCQuaHan, eHCMSResources.Z0213_G1_XNhanTaoDKKhac);
                        yield return warnConfDlg;
                        if (warnConfDlg.IsAccept)
                        {
                            CurrentRegMode = RegistrationFormMode.PATIENT_SELECTED;
                            Globals.HIRegistrationForm = string.Format("{0} {1}. \n{2}", eHCMSResources.Z0208_G1_ChưaDKChoBN, CurrentPatient.FullName, eHCMSResources.A0138_G1_Msg_ConfBoQua);
                        }
                        yield break;
                    }
                    if (CurRegistration.AdmissionDate == null && Globals.Check_CasualAndPreOpReg_StillValid(CurRegistration)
                        || (CurRegistration.AdmissionDate != null && CurRegistration.DischargeDate == null))
                    {
                        string message = "";
                        if (CurRegistration.AdmissionDate == null)
                        {
                            message += eHCMSResources.Z0210_G1_BNCoDKGPhauKTCNhapVien;
                        }
                        else
                        {
                            message += string.Format("\n{0}", eHCMSResources.Z0211_G1_BNDangDTriKTC);
                        }
                        MessageBox.Show(message);
                        CanCreateNewRegistration = false;
                        CanCancelRegistrationInfoCmd = true;
                        if (PatientDetailsContent != null && PatientDetailsContent.ShowConfirmedEmergencyPatient)
                        {
                            PatientDetailsContent.IsConfirmedEmergencyPatient = (loadPatient.CurrentPatient.LatestRegistration_InPt.EmergRecID > 0 ? true : false);
                        }
                        if (PatientDetailsContent != null && PatientDetailsContent.ShowConfirmedForeignerPatient)
                        {
                            PatientDetailsContent.IsConfirmedForeignerPatient = loadPatient.CurrentPatient.LatestRegistration_InPt.IsForeigner.GetValueOrDefault(false);
                        }
                        yield break;
                    }
                }
                if (CurRegistration.V_RegForPatientOfType == AllLookupValues.V_RegForPatientOfType.DKBN_VANG_LAI && CurRegistration.AdmissionDate == null
                    && Globals.Check_CasualAndPreOpReg_StillValid(CurRegistration))
                {
                    string warning = string.Format("{0} ", eHCMSResources.Z0212_G1_BNVLChuaHetHan);
                    warnConfDlg = new WarningWithConfirmMsgBoxTask(warning, eHCMSResources.Z0213_G1_XNhanTaoDKKhac);
                    yield return warnConfDlg;
                    if (!warnConfDlg.IsAccept)
                    {
                        CanCancelRegistrationInfoCmd = true;
                        yield break;
                    }
                }
                CurrentRegMode = RegistrationFormMode.PATIENT_SELECTED;
                Globals.HIRegistrationForm = string.Format("{0} {1}. \n{2}", eHCMSResources.Z0208_G1_ChưaDKChoBN, CurrentPatient.FullName, eHCMSResources.A0138_G1_Msg_ConfBoQua);
            }
            else
            {
                CurrentRegMode = RegistrationFormMode.PATIENT_SELECTED;
                Globals.HIRegistrationForm = string.Format("{0} {1}. \n{2}", eHCMSResources.Z0208_G1_ChưaDKChoBN, CurrentPatient.FullName, eHCMSResources.A0138_G1_Msg_ConfBoQua);
            }

        }

        // TxD 14/07/2014: The following method has been added to get rid of the call to StartEditingPatientLazyLoad
        //                  in method SetCurrentPatient when it is called with parameter patient == null
        //                  because StartEditingPatientLazyLoad has now been removed from PatientDetailsViewModel due to RACING condition that may occur
        public void SetCurrentPatientToNull()
        {
            IsCrossRegion = null;

            CurrentPatient = null;
            CurrentRegMode = RegistrationFormMode.PATIENT_NOT_SELECTED;

            ConfirmedHiItem = null;
            ConfirmedPaperReferal = null;
        }


        private IEnumerator<IResult> SetCurrentPatient_CoRoutine(Patient patient)
        {
            IsCrossRegion = null;

            var p = patient;
            if (p == null || p.PatientID <= 0)
            {
                CurrentPatient = null;
                CurrentRegMode = RegistrationFormMode.PATIENT_NOT_SELECTED;
            }
            else
            {
                CurrentPatient = patient;
                CurrentRegMode = RegistrationFormMode.PATIENT_SELECTED;
            }

            yield return new GenericCoRoutineTask(PatientDetailsContent.LoadPatientDetailsAndHI_GenAction, patient, false);

            ConfirmedHiItem = null;
            ConfirmedPaperReferal = null;

            yield break;
        }

        public void Handle(CreateNewPatientEvent message)
        {
            if (GetView() != null && message != null)
            {
                // TxD 03/12/13
                ExamDate = Globals.GetCurServerDateTime();
                _examDateServerInitVal = ExamDate;

                if (string.IsNullOrEmpty(Globals.HIRegistrationForm))
                {
                    // TxD 14/07/2014: replaced the following with SetPatientToNull
                    //SetCurrentPatient(null);
                    SetCurrentPatientToNull();

                    PatientDetailsContent.CreateNewPatient();
                    ResetView();
                }
                else
                {
                    Coroutine.BeginExecute(GlobalsNAV.DoMessageBoxHIRegis(), null, (o, e) =>
                    {
                        if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                        {
                            // TxD 14/07/2014: replaced the following with SetPatientToNull
                            //SetCurrentPatient(null);
                            SetCurrentPatientToNull();

                            PatientDetailsContent.CreateNewPatient();
                            ResetView();
                            GlobalsNAV.msgb = null;
                            Globals.HIRegistrationForm = "";
                        }
                    });
                }
                //if (PatientDetailsContent.GeneralInfoChanged || PatientDetailsContent.HealthInsuranceContent.InfoHasChanged
                //    || PatientDetailsContent.HealthInsuranceContent.PaperReferalContent.InfoHasChanged)
                //{
                //    if (MessageBox.Show("Thông tin đã thay đổi. Bạn có muốn bỏ qua?", eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                //    {
                //        return;
                //    }
                //}

            }
        }

        public void Handle(HiCardConfirmedEvent message)
        {
            if (message != null)
            {
                ConfirmHIItem(message.HiProfile);
                //PatientSummaryInfoContent.ConfirmedHiItem = message.HiProfile;

                ConfirmPaperReferal(message.PaperReferal);

                //PatientSummaryInfoContent.ConfirmedPaperReferal = message.PaperReferal;

                //HPT 24/08/2015: Hàm này bắt sự kiện được bắn từ PatientDetailsViewModel (ConfirmHICoroutine)
                //Cast giá trị của object Source trong HiCardConfirmedEvent để lấy ra giá trị thuộc tính IsConfirmedEmergencyPatient trong PatientDetailsViewModel, tương ứng với giá trị checkbox BN cấp cứu trong PatientDetailsView.xaml
                //Gán giá trị thuộc tính vừa lấy ra cho biến IsEmergency trong ReceivePatientViewModel để sử dụng tính toán quyền lợi BHYT
                //nếu không làm bước này, khi click xem quyền lợi BHYT, giá trị hiển thị trong Dialog quyền lợi sẽ được lấy từ lần xác nhận cuối chứ không cập nhật giá trị từ giao diện, dễ dẫn đến hiểu lầm
                if (message.Source != null)
                {
                    IPatientDetails vm = message.Source as IPatientDetails;
                    if (vm != null)
                    {
                        IsEmergency = vm.IsConfirmedEmergencyPatient;
                    }
                }
                //HPT_20160706: Lấy giá trị checkbox bệnh nhân tái khám từ ViewModel con
                EmergInPtReExamination = PatientDetailsContent.EmergInPtReExamination;

                //Tinh lai quyen loi bao hiem.
                Coroutine.BeginExecute(DoCalcHiBenefit(message.HiProfile, message.PaperReferal));
            }
        }

        public void Handle(HiCardReloadEvent message)
        {
            //ConfirmedHiItem = null;            
        }

        private bool _registrationInfoHasChanged;
        /// <summary>
        /// Cho biet thong tin dang ky tren form da duoc thay doi chua.
        /// </summary>
        public bool RegistrationInfoHasChanged
        {
            get
            {
                return _registrationInfoHasChanged;
            }
            set
            {
                if (_registrationInfoHasChanged != value)
                {
                    _registrationInfoHasChanged = value;
                    NotifyOfPropertyChange(() => RegistrationInfoHasChanged);

                    //NotifyButtonBehaviourChanges();
                }
            }
        }

        public bool IsProcessing
        {
            get
            {
                return _patientLoading;
            }
        }

        private bool _patientLoading;
        /// <summary>
        /// Dang trong qua trinh lay thong tin benh nhan tu server.
        /// </summary>
        public bool PatientLoading
        {
            get
            {
                return _patientLoading;
            }
            set
            {
                _patientLoading = value;
                NotifyOfPropertyChange(() => PatientLoading);

                NotifyWhenBusy();
            }
        }
        private void NotifyWhenBusy()
        {
            NotifyOfPropertyChange(() => IsProcessing);
        }
        private bool _patientLoaded;
        public bool PatientLoaded
        {
            get
            {
                return _patientLoaded;
            }
            set
            {
                _patientLoaded = value;
                NotifyOfPropertyChange(() => PatientLoaded);
            }
        }

        public void Handle(ResultFound<Patient> message)
        {
            if (message != null)
            {
                CurrentPatient = message.Result;
                if (CurrentPatient != null)
                {
                    // txd: Comment out the following 2 lines 14/09/2013 but not sure why Event was fired here
                    // If problem found later on please REVIEW again!!!! 

                    // SetCurrentPatient(CurrentPatient);
                    // Globals.EventAggregator.Publish(new ItemSelected<Patient> { Item = CurrentPatient });
                    if (RegistrationType == AllLookupValues.RegistrationType.NGOAI_TRU)
                    {
                        Coroutine.BeginExecute(DoSetCurrentPatient(CurrentPatient));
                    }
                    else
                    {
                        Coroutine.BeginExecute(DoSetCurrentPatient_InPt(CurrentPatient));
                    }
                }
            }
        }
        //------------------ DPT
        public void Handle(UpdateCompleted<Patient> message)
        {
            if (message != null && message.Item != null)
            {
                CurrentPatient = message.Item;
                if (CurrentPatient != null)
                {
                    // txd: Comment out the following 2 lines 14/09/2013 but not sure why Event was fired here
                    // If problem found later on please REVIEW again!!!! 

                    // SetCurrentPatient(CurrentPatient);
                    // Globals.EventAggregator.Publish(new ItemSelected<Patient> { Item = CurrentPatient });
                    if (RegistrationType == AllLookupValues.RegistrationType.NGOAI_TRU)
                    {
                        Coroutine.BeginExecute(DoSetCurrentPatient(CurrentPatient));
                    }
                    else
                    {
                        Coroutine.BeginExecute(DoSetCurrentPatient_InPt(CurrentPatient));
                    }
                }
            }
        }

        private PatientRegistration _curRegistration;
        public PatientRegistration CurRegistration
        {
            get { return _curRegistration; }
            set
            {
                _curRegistration = value;
                //HealthInsurance 
                NotifyOfPropertyChange(() => CurRegistration);
                //if (_curRegistration != null && PatientSummaryInfoContent != null)
                //{
                //    PatientSummaryInfoContent.IsCrossRegion = CurRegistration.IsCrossRegion.GetValueOrDefault();
                //}
            }
        }

        public IEnumerator<IResult> DoOpenRegistration(long regID)
        {
            //KMx: Chỉ lấy những thông tin cần thiết của đăng ký thôi, không load hết (17/09/2014 17:31).
            LoadRegistrationSwitch LoadRegisSwitch = new LoadRegistrationSwitch();
            LoadRegisSwitch.IsGetAdmissionInfo = true;
            LoadRegisSwitch.IsGetBillingInvoices = true;
            ConfirmRemoveHIChecked = false;

            var loadRegTask = new LoadRegistrationInfo_InPtTask(regID, (int)AllLookupValues.V_FindPatientType.NOI_TRU, LoadRegisSwitch);
            yield return loadRegTask;
            if (loadRegTask != null && loadRegTask.Registration != null)
            {
                IsHICard_FiveYearsCont = loadRegTask.Registration.IsHICard_FiveYearsCont;
                IsChildUnder6YearsOld = loadRegTask.Registration.IsChildUnder6YearsOld;
                /*==== #003 ====*/
                IsHICard_FiveYearsCont_NoPaid = loadRegTask.Registration.IsHICard_FiveYearsCont_NoPaid;
                FiveYearsAppliedDate = loadRegTask.Registration.FiveYearsAppliedDate;
                FiveYearsARowDate = loadRegTask.Registration.FiveYearsARowDate;
                /*==== #003 ====*/
            }
            if (loadRegTask.Registration == null)
            {
                //Thong bao khong load duoc dang ky
                Globals.EventAggregator.Publish(new ShowMessageEvent { Message = "Error!(9)" });
            }
            else
            {
                // Các loại sau đây không được xác nhận BHYT
                // 1. Đăng ký đã hủy hoặc đã đóng
                // 2. Đăng ký Vãng Lai/Tiền Giải Phẫu chưa nhập viện - quá hạn
                // 3. Đăng ký nội trú/tiền giải phẫu đã xuất viện
                if (loadRegTask.Registration.AdmissionInfo == null)
                {
                    if (Globals.IsCasuaOrPreOpPt(loadRegTask.Registration.V_RegForPatientOfType) && loadRegTask.Registration.RegistrationStatus == AllLookupValues.RegistrationStatus.PENDING_INPT)
                    {
                        if (!Globals.Check_CasualAndPreOpReg_StillValid(loadRegTask.Registration))
                        {
                            if (loadRegTask.Registration.V_RegForPatientOfType == AllLookupValues.V_RegForPatientOfType.DKBN_VANG_LAI)
                            {
                                MessageBox.Show(eHCMSResources.A0220_G1_Msg_InfoKhTheXNhanBHYT_DKVLaiHetHan, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                            }
                            else if (loadRegTask.Registration.V_RegForPatientOfType == AllLookupValues.V_RegForPatientOfType.NBNT_BN_TIEN_PHAU_KHONG_BHYT || loadRegTask.Registration.V_RegForPatientOfType == AllLookupValues.V_RegForPatientOfType.DKBN_DT_NGOAI_TRU)
                            {
                                MessageBox.Show(eHCMSResources.A0219_G1_Msg_InfoKhTheXNhanBHYT_DKKTCConHan, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show(string.Format("{0} {1}", eHCMSResources.K0223_G1_TThaiDKKhongHopLe, eHCMSResources.K0224_G1_KhongTheXNhanBHYT), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                    }
                }
                // TxD 14/05/2015 Added the following
                else
                {
                    if (loadRegTask.Registration.RegistrationStatus != AllLookupValues.RegistrationStatus.OPENED)
                    {
                        MessageBox.Show(string.Format("{0} {1}", eHCMSResources.K0223_G1_TThaiDKKhongHopLe, eHCMSResources.K0224_G1_KhongTheXNhanBHYT), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                    }
                    else
                    {
                        if (loadRegTask.Registration.AdmissionInfo != null && (loadRegTask.Registration.AdmissionInfo.DischargeDate != null || loadRegTask.Registration.AdmissionInfo.TempDischargeDate != null))
                        {
                            MessageBox.Show(eHCMSResources.A0242_G1_Msg_InfoBNDaXV_KTheXNhanBHYT);
                        }
                    }
                }
                if (PatientDetailsContent != null)
                {
                    yield return GenericCoRoutineTask.StartTask(PatientDetailsContent.LoadPatientDetailsAndHI_GenAction, loadRegTask.Registration.Patient, false);

                }
                //▼===== #014
                //20191126 TBL: Khi tìm đăng ký nội trú lên để xác nhận lại BHYT thì lấy thông tin nhập viện để set cho AdmDisDetails để trường hợp khi cập nhật lại thông tin bệnh nhân
                //              thì load lại đăng ký nhưng không lấy thông tin nhập viện nên không thể xác nhận BHYT được  
                AdmDisDetails = new InPatientAdmDisDetails();
                if (loadRegTask.Registration != null && loadRegTask.Registration.AdmissionInfo != null)
                {
                    AdmDisDetails = loadRegTask.Registration.AdmissionInfo;
                }
                //▲===== #014
                CurrentPatient = loadRegTask.Registration.Patient;
                ShowOldRegistration(loadRegTask.Registration);
                // Hpt 04/10/2015: Hàm kiểm tra có hiển thị checkbox Xác nhận bệnh nhân cấp cứu hay không
                CanConfirmEmergency();
            }

            yield break;
        }

        //private long PtRegistrationID = 0;

        public void OpenRegistration(long regID)
        {
            Coroutine.BeginExecute(DoOpenRegistration(regID));
        }

        public void Handle(ItemSelected<PatientRegistration> message)
        {
            if (message == null || message.Item == null || message.Item.Patient == null)
                return;

            if (RegistrationType != AllLookupValues.RegistrationType.XAC_NHAN_LAI_BHYT)
            {
                return;
            }

            OpenRegistration(message.Item.PtRegistrationID);
        }

        public void Handle(ResultNotFound<Patient> message)
        {
            if (message != null)
            {
                //Thông báo không tìm thấy bệnh nhân.
                MessageBoxResult result = MessageBox.Show(eHCMSResources.A0727_G1_Msg_ConfThemMoiBN, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel);
                if (result == MessageBoxResult.OK)
                {
                    // TxD 03/12/13
                    ExamDate = Globals.GetCurServerDateTime();
                    _examDateServerInitVal = ExamDate;

                    // TxD 14/07/2014: replaced the following with SetPatientToNull
                    //SetCurrentPatient(null);
                    SetCurrentPatientToNull();

                    var criteria = message.SearchCriteria as PatientSearchCriteria;
                    PatientDetailsContent.CreateNewPatient();
                    if (criteria != null)
                    {
                        PatientDetailsContent.CurrentPatient.FullName = criteria.FullName;
                        //==== #001
                        try
                        {
                            PatientDetailsContent.QRCode = criteria.QRCode;
                        }
                        catch (Exception ex)
                        {
                            throw new Exception(ex.Message);
                        }
                        //==== #001
                    }
                }
            }
        }

        public void OldRegistrationsCmd()
        {
            Action<IRegistrationList> onInitDlg = delegate (IRegistrationList vm)
            {
                vm.IsInPtRegistration = PatientFindBy == AllLookupValues.PatientFindBy.NOITRU;
                vm.CurrentPatient = CurrentPatient;
            };
            GlobalsNAV.ShowDialog<IRegistrationList>(onInitDlg);
        }

        private bool _isRegistering;
        public bool IsRegistering
        {
            get
            {
                return _isRegistering;
            }
            set
            {
                _isRegistering = value;
                NotifyOfPropertyChange(() => IsRegistering);

                NotifyWhenBusy();
            }
        }

        private double HIBenefit = 0;
        private IEnumerator<IResult> DoInPtConfirmHI_Routine()
        {
            if (CurRegistration.RegistrationStatus == AllLookupValues.RegistrationStatus.REFUND || CurRegistration.RegistrationStatus == AllLookupValues.RegistrationStatus.COMPLETED)
            {
                MessageBox.Show(eHCMSResources.Z0505_G1_DangKyDaNgungSD);
                yield break;
            }
            // Dang ky Vang Lai hoac Tien Giai Phau chua nhap vien da qua han khong duoc phep xac nhan BHYT
            if (CurRegistration.AdmissionInfo == null)
            {
                if (Globals.IsCasuaOrPreOpPt(CurRegistration.V_RegForPatientOfType) && !Globals.Check_CasualAndPreOpReg_StillValid(CurRegistration))
                {
                    MessageBox.Show(eHCMSResources.A0488_G1_Msg_InfoDKDong_KhTheXNhanBHYT);
                    yield break;
                }
                if (!Globals.IsCasuaOrPreOpPt(CurRegistration.V_RegForPatientOfType))
                {
                    MessageBox.Show(eHCMSResources.A0496_G1_Msg_InfoKhTheXNhanBHYT_ChuaNpVien);
                    yield break;
                }
            }
            else
            {   // Dang ky da xuat vien khong duoc xac nhan BHYT
                if (CurRegistration.AdmissionInfo.DischargeDate != null || CurRegistration.AdmissionInfo.TempDischargeDate != null)
                {
                    MessageBox.Show(eHCMSResources.A0242_G1_Msg_InfoBNDaXV_KTheXNhanBHYT);
                    yield break;
                }
            }
            //if (PatientDetailsContent != null && PatientDetailsContent.ShowConfirmedEmergencyPatient)
            if (PatientDetailsContent != null)
            {
                IsEmergency = PatientDetailsContent.IsConfirmedEmergencyPatient;
            }
            //if (PatientDetailsContent != null && PatientDetailsContent.ShowConfirmedForeignerPatient)
            if (PatientDetailsContent != null)
            {
                IsForeigner = PatientDetailsContent.IsConfirmedForeignerPatient;
            }
            if (IsHICard_FiveYearsCont)
            {
                PatientDetailsContent.FiveYearsARowDate = FiveYearsARowDate;
                PatientDetailsContent.FiveYearsAppliedDate = FiveYearsAppliedDate;
            }
            _isRegisterPatient = PatientDetailsContent.ConfirmHIBeforeRegister();
            if (!_isRegisterPatient)
            {
                CanCreateNewRegistration = true;
                yield break;
            }
            // Hpt 07/12/2015: Không cùng lúc check vào cả hai checkbox Xác nhận bệnh nhân có BHYT 5 năm liên tiếp và trẻ em dưới 6 tuổi (lưu ý: quyền lợi cho hai hình thức xác nhận này là khác nhau)
            if (IsHICard_FiveYearsCont && IsChildUnder6YearsOld)
            {
                MessageBox.Show(eHCMSResources.A0288_G1_Msg_Info1in2QL);
                yield break;
            }


            //HPT 24/08/2015 BEGIN: Hàm if kiểm tra nếu có check vào xác nhận BN có BHYT 5 năm liên tiếp thì show cảnh báo yêu cầu người dùng xác nhận lại cho chắc chắn
            //Nếu người dùng check chọn Tiếp tục lưu và OK thì thực hiện xác nhận bảo hiểm với mức quyền lợi 100% nếu đăng ký BHYT đã hợp lệ
            //Nếu người dùng không check mà click OK thì thực hiện xác nhận bảo hiểm với mức quyền lợi 80% nếu đăng ký BHYT đã hợp lệ
            //đăng ký BHYT cho BN nội trú hợp lệ khi có thẻ bảo hiểm hợp lệ và giấy chuyển viện hợp lệ hoặc không có giấy chuyển viện trong trường hợp cấp cứu (IsEmergency = true)
            if (IsHICard_FiveYearsCont)
            {
                CalcHiBenefitAndRegister_ErrorMsg = eHCMSResources.Z0214_G1_XNhanBNHuongBHYT5Nam;
                warnConfDlg = new WarningWithConfirmMsgBoxTask(CalcHiBenefitAndRegister_ErrorMsg, eHCMSResources.Z0217_G1_TiepTucLuuTTin);
                yield return warnConfDlg;
                if (!warnConfDlg.IsAccept)
                {
                    IsHICard_FiveYearsCont = false;
                    yield break;
                }
            }
            if (IsChildUnder6YearsOld)
            {
                string ErrorMsg = "";
                string warningMsg = "";
                if (!CheckResult_ChildUnder6YearsOld(out ErrorMsg) && ErrorMsg != "")
                {
                    warningMsg = string.Format("{0}: ", eHCMSResources.Z0215_G1_BNChuaThoaManYC) + ErrorMsg;
                }
                else
                {
                    warningMsg = eHCMSResources.Z0216_G1_XNhanBNTreEmDuoi6Tuoi;
                }

                warnDlgTask = new MessageWarningShowDialogTask(warningMsg, eHCMSResources.Z0217_G1_TiepTucLuuTTin);
                yield return warnDlgTask;
                if (!warnDlgTask.IsAccept)
                {
                    IsChildUnder6YearsOld = false;
                    yield break;
                }
            }

            ConfirmHIItem(PatientDetailsContent.hiCardConfirmedEvent.HiProfile);
            ConfirmPaperReferal(PatientDetailsContent.hiCardConfirmedEvent.PaperReferal);

            IsAllowCrossRegion = Globals.CheckAllowToCrossRegion(ConfirmedHiItem, AllLookupValues.RegistrationType.NOI_TRU);
            // TxD 28/01/2018 DO NOT Set HI Details for PatientSummaryInfoContent HERE. 
            // Instead method SetHIDetails_For_PtSummaryInfoCtrl is called after HI Confirmation is DONE and Registration is RELOADED from SERVER
            //PatientSummaryInfoContent.ConfirmedHiItem = PatientDetailsContent.hiCardConfirmedEvent.HiProfile;
            //PatientSummaryInfoContent.ConfirmedPaperReferal = PatientDetailsContent.hiCardConfirmedEvent.PaperReferal;
            /*==== #003 ====*/
            var calcHiTask = new CalcHiBenefitTask(PatientDetailsContent.hiCardConfirmedEvent.HiProfile, PatientDetailsContent.hiCardConfirmedEvent.PaperReferal, (long)AllLookupValues.RegistrationType.NOI_TRU, IsEmergency, IsAllowCrossRegion, IsHICard_FiveYearsCont, IsChildUnder6YearsOld, IsAllowCrossRegion, IsHICard_FiveYearsCont_NoPaid);
            /*==== #003 ====*/
            yield return calcHiTask;
            if (calcHiTask.Error != null)
            {
                yield break;
            }
            //PatientSummaryInfoContent.HiBenefit = calcHiTask.HiBenefit;
            //PatientSummaryInfoContent.IsCrossRegion = calcHiTask.IsCrossRegion;
            HIBenefit = calcHiTask.HiBenefit;
            IsCrossRegion = calcHiTask.IsCrossRegion;

            int ShowOpts_To_OJR_Card = 0;
            int PreSelected_OJR_Option = 0;
            //▼====: #008
            //Thay đổi điều kiện từ luôn luôn show popup ConfirmHiBenefit sang chỉ show khi giá trị trả về của hàm SetJoiningCardsOption = true
            if (!SetJoiningCardsOption(ref ShowOpts_To_OJR_Card, ref PreSelected_OJR_Option))
            {
                yield break;
            }
            //▲====: #008
            var confirmHI = new ConfirmHiBenefitDialogTask(calcHiTask.HiBenefit, calcHiTask.HiBenefit
                , calcHiTask.HiItem.HIID, CurrentPatient.PatientID, calcHiTask.IsCrossRegion, (long)AllLookupValues.RegistrationType.NOI_TRU, IsAllowCrossRegion, Globals.ServerConfigSection.HealthInsurances.AllowInPtCrossRegion, ShowOpts_To_OJR_Card, PreSelected_OJR_Option);
            yield return confirmHI;

            switch (confirmHI.confirmHiBenefitEvent.confirmHiBenefitEnum)
            {
                case ConfirmHiBenefitEnum.ConfirmHiBenefit:
                    ConfirmHiBenefit(confirmHI.confirmHiBenefitEvent);
                    break;
                case ConfirmHiBenefitEnum.NoChangeConfirmHiBenefit:
                    NoChangeConfirmHiBenefit(confirmHI.confirmHiBenefitEvent);
                    break;
                case ConfirmHiBenefitEnum.RemoveConfirmedHiCard:
                    RemoveConfirmedHiCard(confirmHI.confirmHiBenefitEvent);
                    break;
            }
            if (Globals.ServerConfigSection.HealthInsurances.ValidateApplyingHIBenefit == true && CurRegistration != null && CurRegistration.HealthInsurance != null && CurRegistration.PtInsuranceBenefit.GetValueOrDefault() > 0)
            {
                string ErrorMsg = "";
                if (CurRegistration.HealthInsurance.HICardNo != ConfirmedHiItem.HICardNo)
                {
                    ErrorMsg = eHCMSResources.Z1948_G1_BNDaXNBHYTBang1TheKhac;
                }
                else
                {
                    //if (CurRegistration.PtInsuranceBenefit != PatientSummaryInfoContent.HiBenefit)
                    //{
                    //    ErrorMsg = eHCMSResources.Z1949_G1_BNDaXNBHYTVoi1MucQLoiKhac;
                    //}
                }
                if (!string.IsNullOrWhiteSpace(ErrorMsg) && !string.IsNullOrEmpty(ErrorMsg))
                {
                    ErrorMsg += string.Format("\n{0}", eHCMSResources.Z1950_G1_CanhBaoVeThaoTacThayDoiBHYT);
                    warnConfDlg = new WarningWithConfirmMsgBoxTask(ErrorMsg, eHCMSResources.Z0217_G1_TiepTucLuuTTin);
                    yield return warnConfDlg;
                    if (!warnConfDlg.IsAccept)
                    {
                        yield break;
                    }
                }
            }

            //▼====: #007 TTM 21/5/2018: 
            //Thêm điều kiện: Ngày nhập viện của bệnh nhân không được bé hơn ngày đến của thẻ (Trong các trường hợp: Thêm mới, đổi thẻ hoặc nối thẻ.)
            //Xác nhận thì sẽ gọi InPtConfirmHICmd() tiếp tục gọi DoInPtConfirmHI_Routine() tiếp tục gọi ConfirmHIBeforeRegister() tiếp tục gọi ConfirmHIBenefit() xong gọi CheckValidationAndGetConfirmedItem();
            //Khi người sử dụng click xác nhận BHYT sẽ xuất hiện popup cho xác nhận thẻ 1 (nối thẻ, đổi thẻ, không làm gì nếu là thẻ 2,3) hàm này sẽ được gọi.

            int nSelOJR = confirmHI.confirmHiBenefitEvent.Selected_OJR_Option;
            // Trường hợp xác nhận thẻ mới, chồng thẻ hoặc nối thẻ.
            if (nSelOJR == 0 || nSelOJR == 1 || nSelOJR == 2)
            {
                if (CurRegistration == null || CurRegistration.AdmissionInfo == null || ConfirmedHiItem == null)
                    yield break;
                if (CurRegistration.AdmissionInfo.AdmissionDate.Value > ConfirmedHiItem.ValidDateTo.Value)
                {
                    MessageBox.Show(eHCMSResources.Z2209_G1_TheHaiBaVaNgayNV);
                    yield break;
                }
            }
            //*▲====: #007

            yield return GenericCoRoutineTask.StartTask(InPtConfirmHI_Action, confirmHI.confirmHiBenefitEvent.Selected_OJR_Option);

        }
        //▼====: #008 
        //Thay đổi hàm SetJoiningCardsOption từ void sang bool => Có giá trị trả về true false để xác định xem có cần phải show popup ConfirmHIBenefit hay không.
        //Thêm điều kiện:   Thẻ được nối vào thẻ thứ 2 không được có ngày đến bé hơn ngày đến thẻ thứ 2.
        //                  Thẻ được nối thứ thẻ thứ 1 nếu có ngày đến bé hơn thẻ 1 thì chỉ được chồng thẻ (đổi thẻ), ngược lại thì được nối thẻ và đổi thẻ.
        //
        private bool SetJoiningCardsOption(ref int ShowOpts_To_OJR_Card, ref int PreSelectedOption)
        {
            // NOTE: Default Value of ShowOpts_To_OJR_Card == 0 => No Show OJR
            // Case 1: New HI confirmation ie. first time the Registration is confirmed with a HI Card  => No Show OJR          
            if (CurRegistration.HisID == null || CurRegistration.HisID.GetValueOrDefault() == 0)
            {
                return true;
            }

            // Case 2: The Registration has been confirm with 3 Cards joined together ie. UP to HisID_3
            //         So the ONLY option is to remove the third HI Card added then rejoin with another Card or leave it with just 2 cards
            if (CurRegistration.HisID_3.HasValue && CurRegistration.HisID_3.Value > 0)
            {
                ShowOpts_To_OJR_Card = 2;   // Remove + NoChange
                PreSelectedOption = 2;      // Pre-Select No Change               
                return true;
            }

            // Case 3: The Registration has been confirmed with one or two HI Cards joined together
            HealthInsurance lastHICardConfirmed = null;
            if (CurRegistration.HisID_2.HasValue && CurRegistration.HisID_2.Value > 0)
            {
                // A newly confirmed Card has to be different to the existing ones in order to Join with the last Card Added or Joined
                //  ie. Diff to Both HisID_2 & HisID
                lastHICardConfirmed = CurRegistration.HealthInsurance_2;
                if (lastHICardConfirmed.HIID != ConfirmedHiItem.HIID && CurRegistration.HealthInsurance.HIID != ConfirmedHiItem.HIID)
                {
                    if (ConfirmedHiItem.ValidDateTo > lastHICardConfirmed.ValidDateTo)
                    {
                        ShowOpts_To_OJR_Card = 3;   // Join + NoChange
                    }
                    else
                    {
                        MessageBox.Show(eHCMSResources.Z2221_G1_NgDenTheKgHopLe);
                        return false;
                    }
                }
                else
                {
                    ShowOpts_To_OJR_Card = 2;   // Remove + NoChange
                }
            }
            else
            {
                lastHICardConfirmed = CurRegistration.HealthInsurance;
                if (lastHICardConfirmed.HIID != ConfirmedHiItem.HIID)
                {
                    if (ConfirmedHiItem.ValidDateTo > lastHICardConfirmed.ValidDateTo)
                    {
                        ShowOpts_To_OJR_Card = 4;   // Override + Join + NoChange
                    }
                    else
                    {
                        ShowOpts_To_OJR_Card = 1;   // Override + NoChange
                    }
                }
            }
            return true;
        }
        //▲====: #008
        public void InPtConfirmHI_Action(GenericCoRoutineTask genTask, object Selected_OJR_Option)
        {
            if (ConfirmedHiItem == null)
            {
                MessageBox.Show(eHCMSResources.A0399_G1_Msg_InfoChuaChonTheBHYTDeXN);
                return;
            }
            long? PaperReferalID = null;
            if (ConfirmedPaperReferal != null)
            {
                PaperReferalID = ConfirmedPaperReferal.RefID;
            }

            //var benefit = PatientSummaryInfoContent.HiBenefit;

            TypesOf_ConfirmHICardForInPt eConfirmType = TypesOf_ConfirmHICardForInPt.eConfirmNew;
            if (CurRegistration.HisID > 0)
            {
                int nSel_OJR = (int)Selected_OJR_Option;
                if (nSel_OJR == 1)
                {
                    eConfirmType = TypesOf_ConfirmHICardForInPt.eConfirmReplace;
                }
                else if (nSel_OJR == 2)
                {
                    eConfirmType = TypesOf_ConfirmHICardForInPt.eConfirmAdd;
                }
                else if (nSel_OJR == 3)
                {
                    eConfirmType = TypesOf_ConfirmHICardForInPt.eConfirmRemoveLastAdded;
                }
                else if (nSel_OJR == 4)
                {
                    eConfirmType = TypesOf_ConfirmHICardForInPt.eConfirmNoChange;
                }
            }

            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        bool bContinue = true;
                        contract.BeginApplyHiToInPatientRegistration_V3(CurRegistration.PtRegistrationID, ConfirmedHiItem.HIID, HIBenefit
                            , IsCrossRegion.GetValueOrDefault(), PaperReferalID, (int)AllLookupValues.V_FindPatientType.NOI_TRU, IsEmergency
                            , Globals.LoggedUserAccount.StaffID.GetValueOrDefault(), IsHICard_FiveYearsCont, IsChildUnder6YearsOld, IsHICard_FiveYearsCont_NoPaid
                            , FiveYearsAppliedDate, FiveYearsARowDate, eConfirmType,
                            IsAllowCrossRegion,
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                try
                                {
                                    var bOK = contract.EndApplyHiToInPatientRegistration_V3(asyncResult);
                                    MessageBox.Show(eHCMSResources.K0461_G1_XNhanBHOk);
                                    //ConfirmHi_OnEnd();
                                    //TxD17/12/2014 => KMx: Sau khi xác nhận BH thành công phải load lại Registration để có HisID, mới in giấy xác nhận BHYT được (02/12/2014 16:05).
                                    //▼====: #017
                                    UpdateIsConfirmedEmergencyPatient();
                                    //OpenRegistration(CurRegistration.PtRegistrationID);
                                    //▲====: #017
                                    if (IsEmergency)
                                    {
                                        GetAllInPatientBillingInvoices();
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                                    ClientLoggerHelper.LogInfo(ex.ToString());
                                    bContinue = false;
                                }
                                finally
                                {
                                    genTask.ActionComplete(bContinue);
                                }
                            }), null);
                    }
                }
                catch (Exception ex)
                {
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    ClientLoggerHelper.LogInfo(ex.ToString());
                    genTask.ActionComplete(false);
                }
            });

            t.Start();
        }

        public void InPtConfirmHICmd()
        {
            if (CurRegistration != null && Globals.IsLockRegistration(CurRegistration.RegLockFlag, eHCMSResources.Z0218X_G1_XNhanBHYT))
            {
                return;
            }
            //20181205 TBL: BM 0005381 Validation cho truong Ngay duoc mien CCT
            if (!IsHICard_FiveYearsCont_NoPaid && IsHICard_FiveYearsCont && FiveYearsAppliedDate == null)
            {
                MessageBox.Show(eHCMSResources.Z2364_G1_NhapNgayMienCCT);
                return;
            }
            if (!IsHICard_FiveYearsCont_NoPaid && IsHICard_FiveYearsCont && FiveYearsARowDate == null)
            {
                MessageBox.Show("Vui lòng nhập ngày đủ 5 năm liên tục");
                return;
            }
            if (ConfirmRemoveHIChecked)
            {
                Coroutine.BeginExecute(DoInPtRemoveHI_Routine());
            }
            else
            {
                Coroutine.BeginExecute(DoInPtConfirmHI_Routine());
            }
        }

        private IEnumerator<IResult> DoInPtRemoveHI_Routine()
        {
            if (CurRegistration != null && CurRegistration.AdmissionInfo.DischargeDate != null || CurRegistration.AdmissionInfo.TempDischargeDate != null)
            {
                MessageBox.Show(eHCMSResources.A0242_G1_Msg_InfoBNDaXV_KTheXNhanBHYT);
                yield break;
            }

            warnConfDlg = new WarningWithConfirmMsgBoxTask(eHCMSResources.Z0220_G1_XNhanXoaBHYTChoDK, eHCMSResources.Z0219_G1_XNhanXoaBHYT);
            yield return warnConfDlg;
            if (!warnConfDlg.IsAccept)
            {
                warnConfDlg = null;
                yield break;
            }

            yield return GenericCoRoutineTask.StartTask(InPtRemoveHI_Action);
        }

        public void InPtRemoveHI_Action(GenericCoRoutineTask genTask)
        {
            long? OldPaperReferalID = null;
            if (CurRegistration.PaperReferalID.HasValue && CurRegistration.PaperReferalID > 0)
            {
                OldPaperReferalID = CurRegistration.PaperReferalID;
            }

            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        bool bContinue = true;
                        contract.BeginRemoveHiFromInPatientRegistration(CurRegistration.PtRegistrationID,
                            IsEmergency, Globals.LoggedUserAccount.StaffID.GetValueOrDefault(), OldPaperReferalID,
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                try
                                {
                                    var bOK = contract.EndRemoveHiFromInPatientRegistration(asyncResult);
                                    MessageBox.Show(eHCMSResources.K0469_G1_XoaBHYTChoDKOk);
                                    //ConfirmHi_OnEnd();
                                    //TxD17/12/2014 => KMx: Sau khi xác nhận BH thành công phải load lại Registration để có HisID, mới in giấy xác nhận BHYT được (02/12/2014 16:05).
                                    OpenRegistration(CurRegistration.PtRegistrationID);
                                }
                                catch (Exception ex)
                                {
                                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                                    ClientLoggerHelper.LogInfo(ex.ToString());
                                    bContinue = false;
                                }
                                finally
                                {
                                    genTask.ActionComplete(bContinue);
                                }
                            }), null);
                    }
                }
                catch (Exception ex)
                {
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    ClientLoggerHelper.LogInfo(ex.ToString());
                    genTask.ActionComplete(false);
                }
            });

            t.Start();
        }

        private bool _isRegisterPatient = false;
        private long? RegistrationID = 0;
        public void CreateRegistrationCmd()
        {
            if (Globals.IsCasuaOrPreOpPt(V_RegForPatientOfType) && (DepartmentContent.SelectedItem == null || DepartmentContent.SelectedItem.DeptID <= 0))
            {
                MessageBox.Show(eHCMSResources.A0093_G1_Msg_InfoChuaChonKhoaChoDK);
                return;
            }
            //20181205 TBL: BM 0005381 Validation cho truong Ngay duoc mien CCT
            if (!IsHICard_FiveYearsCont_NoPaid && IsHICard_FiveYearsCont && FiveYearsAppliedDate == null)
            {
                MessageBox.Show(eHCMSResources.Z2364_G1_NhapNgayMienCCT);
                return;
            }
            if (!IsHICard_FiveYearsCont_NoPaid && IsHICard_FiveYearsCont && FiveYearsARowDate == null)
            {
                MessageBox.Show("Vui lòng nhập ngày đủ 5 năm liên tục");
                return;
            }
            //▼===== #012: Do Comment yield break ở trên rồi nên phải kiểm tra lại trước khi đăng ký nội trú 
            //             vì nếu không kiểm tra khi báo cáo BHYT sẽ thiếu thông tin ngay tháng năm sinh cho trẻ em dưới 6 tuổi
            if (CurrentPatient != null && CurrentPatient.PatientID > 0)
            {
                if (!CheckChildUnderSixAge(CurrentPatient))
                {
                    return;
                }
            }
            //▲===== #012
            CanCreateNewRegistration = false;
            RegisterPatientNew();
        }

        private void RegisterPatientNew()
        {
            if (PatientDetailsContent != null && PatientDetailsContent.ShowConfirmedEmergencyPatient)
            {
                IsEmergency = PatientDetailsContent.IsConfirmedEmergencyPatient;
                V_ObjectMedicalExamination = PatientDetailsContent.IsConfirmedEmergencyPatient ? 
                    (long)AllLookupValues.V_ObjectMedicalExamination.Cap_Cuu_2 : 
                    (long)AllLookupValues.V_ObjectMedicalExamination.Kham_Chua_Benh_Dich_Vu_9;
            }
            if (PatientDetailsContent != null && PatientDetailsContent.ShowConfirmedForeignerPatient)
            {
                IsForeigner = PatientDetailsContent.IsConfirmedForeignerPatient;
            }

            if (PatientDetailsContent != null && PatientDetailsContent.ShowEmergInPtReExamination)
            {
                EmergInPtReExamination = PatientDetailsContent.EmergInPtReExamination;
            }

            // TxD: Nhan Benh Noi Tru & Cap Cuu Khong Co BHYT ==> Go ahead and Register without VALIDATING & CALCULATING HI BENEFIT
            if (RegistrationType == AllLookupValues.RegistrationType.NOI_TRU || RegistrationType == AllLookupValues.RegistrationType.CAP_CUU)
            {
                RegisterPatient();
                return;
            }
            _isRegisterPatient = PatientDetailsContent.ConfirmHIBeforeRegister();
            if (!_isRegisterPatient)
            {
                CanCreateNewRegistration = true;
                return;
            }
            ConfirmHIItem(PatientDetailsContent.hiCardConfirmedEvent.HiProfile);
            ConfirmPaperReferal(PatientDetailsContent.hiCardConfirmedEvent.PaperReferal);

            // TxD 25/01/2015: Added new condition for Emrgency InPt ReExamination (benh nhan cap cuu tai kham)

            IsAllowCrossRegion = (PatientDetailsContent != null ? PatientDetailsContent.IsAllowCrossRegion : false);

            //▼====== #009
            string strHospitalCodeConfirm = "";
            if (ConfirmedHiItem != null)
            {
                strHospitalCodeConfirm = ConfirmedHiItem.RegistrationCode;
            }
            bool TheBHYT_KhongDoBV_CapPhat = Globals.ServerConfigSection.Hospitals.HospitalCode != strHospitalCodeConfirm;
            //▲====== #009
            bool Conditions_Not_Allow_OutPt_TraiTuyen = (!EmergInPtReExamination && !IsChildUnder6YearsOld && !IsAllowCrossRegion && RegistrationType == AllLookupValues.RegistrationType.NGOAI_TRU);
            bool PaperRefferal_NotValid = (PatientDetailsContent.hiCardConfirmedEvent.PaperReferal == null || PatientDetailsContent.hiCardConfirmedEvent.PaperReferal.IsChecked == false);
            if (Conditions_Not_Allow_OutPt_TraiTuyen && PaperRefferal_NotValid && TheBHYT_KhongDoBV_CapPhat)
            {
                MessageBox.Show(eHCMSResources.A0251_G1_Msg_InfoBNNgTruKhDcTraiTuyen);
                CanCreateNewRegistration = true;
                return;
            }

            if (ConfirmedHiItem != null && TicketIssueObj != null
                && !string.IsNullOrEmpty(ConfirmedHiItem.HICardNo)
                && !string.IsNullOrEmpty(TicketIssueObj.HICardNo)
                && !ConfirmedHiItem.HICardNo.Equals(TicketIssueObj.HICardNo))
            {
                MessageBox.Show(eHCMSResources.Z2746_G1_BenhNhanKhongDungSTT);
                return;
            }

            Coroutine.BeginExecute(DoCalcHiBenefitAndRegister(PatientDetailsContent.hiCardConfirmedEvent.HiProfile, PatientDetailsContent.hiCardConfirmedEvent.PaperReferal));
        }

        private bool _IsAllowCrossRegion;
        public bool IsAllowCrossRegion
        {
            get
            {
                return _IsAllowCrossRegion;
            }
            set
            {
                _IsAllowCrossRegion = value;
                NotifyOfPropertyChange(() => IsAllowCrossRegion);
            }
        }

        private void RegisterPatient()
        {
            ObservableCollection<System.ComponentModel.DataAnnotations.ValidationResult> validationResults;
            bool isValid = Validate(out validationResults);
            if (!isValid)
            {
                Action<IValidationError> onInitDlg = delegate (IValidationError errorVm)
                {
                    errorVm.SetErrors(validationResults);
                };
                GlobalsNAV.ShowDialog<IValidationError>(onInitDlg);
                CanCreateNewRegistration = true;
                return;
            }

            var regInfo = CreateRegistrationFromUserInput();

            //▼===== 20200430 TTM: set giá trị cho hàng đợi tại đây trước khi đi lưu.
            if (TicketIssueObj != null && regInfo != null)
            {
                regInfo.TicketIssue = TicketIssueObj;
            }
            //▲===== 

            this.ShowBusyIndicator();

            var t = new Thread(() =>
            {
                AxErrorEventArgs error = null;
                try
                {
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        // TxD 03/12/13 : Double Check to see IF ExamDate HAS NOT BEEN MANUALLY modified by USER
                        //                Then RESET IT to ensure it has the most RECENT time before SAVING

                        if (ExamDate == _examDateServerInitVal)
                        {
                            ExamDate = Globals.GetCurServerDateTime();
                            _examDateServerInitVal = ExamDate;
                        }

                        contract.BeginSaveEmptyRegistration(Globals.LoggedUserAccount.StaffID.GetValueOrDefault(),
                            Globals.DeptLocation.DeptLocationID, null, regInfo, (long)RegistrationType,
                            Globals.DispatchCallback(asyncResult =>
                            {
                                bool bOK;
                                PatientRegistration registration = null;
                                try
                                {
                                    contract.EndSaveEmptyRegistration(out registration, asyncResult);
                                    bOK = true;
                                    _currentAppointment = null;
                                }
                                catch (FaultException<AxException> fault)
                                {
                                    bOK = false;
                                    error = new AxErrorEventArgs(fault);
                                    this.HideBusyIndicator();
                                    MessageBox.Show(fault.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                                }
                                catch (Exception ex)
                                {
                                    //There was an error deserializing the object of type DataEntities.PatientRegistration. Unexpected end of file while parsing Name has occurred. Line 1, position 16352
                                    //System.Activator.CreateInstance(registration,true);
                                    bOK = false;
                                    error = new AxErrorEventArgs(ex);
                                    this.HideBusyIndicator();
                                }

                                if (bOK)
                                {
                                    Globals.HIRegistrationForm = "";
                                    //IsRegistering = false;
                                    RegistrationID = registration.PtRegistrationID;
                                    CurrentPatient.latestHIRegistration = registration;
                                    ShowOldRegistration(registration);

                                    //MessageBox.Show("Đăng ký thành công.");
                                    CanCreateNewRegistration = false;
                                    if (RegistrationType != AllLookupValues.RegistrationType.NOI_TRU)
                                    {
                                        CanCancelRegistrationInfoCmd = true;
                                    }
                                    NotifyOfPropertyChange(() => CanReportRegistrationInfoInsuranceCmd);

                                    //ReportRegistrationInfoInsuranceCmd();

                                    // TxD 21/04/2014: At this moment ONLY FOR out Patient that we Print the Confirmation Paper for the Patients to take with them 
                                    //                  to the Consultation Room
                                    if (RegistrationType == AllLookupValues.RegistrationType.NGOAI_TRU)
                                    {
                                        ProcessConfirmHIReport();
                                    }
                                    else
                                    {
                                        // TxD 22/04/2014: If Confirmation Paper is not PRINTED in the Case of InPatient then Display the following Message
                                        MessageBox.Show(eHCMSResources.A0499_G1_Msg_InfoDKOK);
                                    }

                                    //20200430 TTM:     Sau khi đã lưu giá trị hàng đợi xong sẽ cập nhật số TT đó sang tình trạng đã đăng ký => Chỉ làm khi cấu hình bật.
                                    //                  Dùng IssueDateTime vì khi đăng ký cho nhận bệnh bảo hiểm không có đọc ngược lại dữ liệu nên TicketGetTime sẽ không có dữ liệu.
                                    if (registration.TicketIssue != null && Globals.ServerConfigSection.CommonItems.UpdateTicketStatusAfterRegister)
                                    {
                                        UpdateTicketStatusAfterRegister(registration.TicketIssue.TicketNumberText, registration.TicketIssue.IssueDateTime);
                                    }

                                    this.HideBusyIndicator();

                                    //luu thanh cong thi reload lại
                                    //Deployment.Current.Dispatcher.BeginInvoke(
                                    //    () => Coroutine.BeginExecute(DoSetCurrentPatient(CurrentPatient)));
                                    //PatientDetailsContent.LoadPatientDetailsAndHI_GenAction(null, CurrentPatient);
                                    //Deployment.Current.Dispatcher.BeginInvoke(
                                    //    () => SetCurrentPatient(CurrentPatient)); 
                                    //▼====== #011
                                    GlobalsNAV.ShowMessagePopup(eHCMSResources.Z2320_G1_NhanBenhOK);
                                    //▲====== #011
                                    System.Windows.Application.Current.Dispatcher.Invoke(() =>
                                    {
                                        PatientDetailsContent.LoadPatientDetailsAndHI_GenAction(null, CurrentPatient, false);
                                    });
                                }
                            }), null);
                    }
                }
                catch (Exception ex)
                {
                    error = new AxErrorEventArgs(ex);
                }

                if (error != null)
                {
                    Globals.EventAggregator.Publish(new ErrorOccurred { CurrentError = error });
                }
            });
            t.Start();
        }
        //▼====: #015
        //TBL: Nhận event của PatientDetailsViewModel để hiển thị đúng
        public void Handle(ReloadPtRegistrationCode message)
        {
            if (message != null && CurRegistration != null)
            {
                CurRegistration.PtRegistrationCode = message.PtRegistrationCode;
            }
        }
        //▲====: #015
        private void ProcessConfirmHIReport()
        {
            // 20181020 TNHX: [BM0003199] Add config for HiConfirmationPrintingMode
            var printingMode = Globals.ServerConfigSection.CommonItems.HiConfirmationPrintingMode;
            switch (printingMode)
            {
                case 1:
                    ShowHiConfirmationReport(CurrentPatient.latestHIRegistration.PtRegistrationID);
                    break;
                case 2:
                    PrintConfirmHIReportSilently(CurrentPatient.latestHIRegistration.PtRegistrationID);
                    break;
                case 3:
                    {
                        ShowHiConfirmationReport(CurrentPatient.latestHIRegistration.PtRegistrationID);
                        PrintConfirmHIReportSilently(CurrentPatient.latestHIRegistration.PtRegistrationID);
                    }
                    break;
            }
        }

        public void ShowHiConfirmationReport(long registrationID)
        {
            Action<ICommonPreviewView> onInitDlg = delegate (ICommonPreviewView reportVm)
            {
                reportVm.RegistrationID = registrationID;
                if (RegistrationType == AllLookupValues.RegistrationType.NGOAI_TRU)
                {
                    reportVm.eItem = ReportName.REGISTRATION_OUT_PATIENT_HI_CONFIRMATION;
                }
                else
                {
                    reportVm.eItem = ReportName.REGISTRATION_IN_PATIENT_HI_CONFIRMATION;
                }
            };
            GlobalsNAV.ShowDialog<ICommonPreviewView>(onInitDlg);
        }

        private bool _canCreateNewRegistration;
        public bool CanCreateNewRegistration
        {
            get { return _canCreateNewRegistration; }
            set
            {
                _canCreateNewRegistration = value;
                NotifyOfPropertyChange(() => CanCreateRegistrationCmd);
                NotifyOfPropertyChange(() => CanReportRegistrationInfoInsuranceCmd);
                NotifyOfPropertyChange(() => CanInTemplate38Cmd);
            }
        }

        private bool _CanCancelRegistrationInfoCmd = false;
        public bool CanCancelRegistrationInfoCmd
        {
            get { return _CanCancelRegistrationInfoCmd; }
            set
            {
                _CanCancelRegistrationInfoCmd = value;
                NotifyOfPropertyChange(() => CanCancelRegistrationInfoCmd);
            }
        }

        public bool CanCreateRegistrationCmd
        {
            get
            {
                if (RegistrationType == AllLookupValues.RegistrationType.Unknown)
                {
                    return false;
                }
                return _currentPatient != null && CurrentRegMode == RegistrationFormMode.PATIENT_SELECTED && CanCreateNewRegistration;
            }
        }

        private DateTime _examDateServerInitVal;
        private DateTime _examDate;
        public DateTime ExamDate
        {
            get
            {
                return _examDate;
            }
            set
            {
                if (_examDate != value)
                {
                    _examDate = value;
                    NotifyOfPropertyChange(() => ExamDate);
                }
            }
        }

        private readonly RefDepartment _department;

        // TxD
        public bool Validate(out ObservableCollection<System.ComponentModel.DataAnnotations.ValidationResult> result)
        {
            result = new ObservableCollection<System.ComponentModel.DataAnnotations.ValidationResult>();
            if (_currentPatient == null)
            {
                var item = new System.ComponentModel.DataAnnotations.ValidationResult(eHCMSResources.Z0148_G1_HayChon1BN, new[] { "CurrentPatient" });
                result.Add(item);
            }

            if (ExamDate == DateTime.MinValue)
            {
                var item = new System.ComponentModel.DataAnnotations.ValidationResult(eHCMSResources.Z0154_G1_NgDKKhongHopLe, new[] { "ExamDate" });
                result.Add(item);
            }

            // TxD: Added to the following condition if It's NOT Nhan Benh Noi Tru & Cap Cuu Khong co BHYT
            if (RegistrationType == AllLookupValues.RegistrationType.NGOAI_TRU || (RegistrationType != AllLookupValues.RegistrationType.NOI_TRU && RegistrationType != AllLookupValues.RegistrationType.CAP_CUU))
            {
                if (ConfirmedHiItem == null)
                {
                    var item = new System.ComponentModel.DataAnnotations.ValidationResult(eHCMSResources.Z0157_G1_ChuaKTraTheBH, new[] { "ConfirmedHiItem" });
                    result.Add(item);
                }
            }
            if (result.Count > 0)
            {
                return false;
            }
            return true;
        }

        private void ShowOldRegistration(PatientRegistration regInfo)
        {
            CurRegistration = regInfo;

            //Chuyen sang mode giong nhu mo lai dang ky cu
            CurrentRegMode = RegistrationFormMode.OLD_REGISTRATION_OPENED;
            _confirmedHiItem = regInfo.HealthInsurance;
            _confirmedPaperReferal = regInfo.PaperReferal;
            NotifyOfPropertyChange(() => ConfirmedHiItem);
            NotifyOfPropertyChange(() => ConfirmedPaperReferal);

            if (PatientSummaryInfoContent != null)
            {
                if (RegistrationType != AllLookupValues.RegistrationType.NGOAI_TRU && PatientDetailsContent != null && regInfo.AdmissionInfo != null)
                {
                    // TxD 29/09/2017: Modify the following to DeepCopy so the Summary details will refer to a different Object 
                    //                  NOT the SAME as CurrentPatient in PatientDetailsContent
                    //                  In the pass somehow it used to work OK even WITHOUT the DeepCopy, To Be Investigated and 
                    //                  reason should be provided further HERE.....
                    PatientSummaryInfoContent.CurrentPatient = ObjectCopier.DeepCopy(PatientDetailsContent.CurrentPatient);

                    if (regInfo.AdmissionInfo.AdmissionDate.HasValue)
                    {
                        PatientSummaryInfoContent.CurrentPatient.InPtAdmissionDate = regInfo.AdmissionInfo.AdmissionDate.Value;
                        PatientSummaryInfoContent.CurrentPatient.InPtAdmittedDeptName = regInfo.AdmissionInfo.Department.DeptShortName;
                    }
                    if (regInfo.AdmissionInfo.DischargeDate.HasValue)
                    {
                        PatientSummaryInfoContent.CurrentPatient.InPtDischargeDate = regInfo.AdmissionInfo.DischargeDate.Value;
                    }
                    else
                    {
                        if (regInfo.AdmissionInfo.DischargeDetailRecCreatedDate.HasValue)
                        {
                            PatientSummaryInfoContent.CurrentPatient.InPtDischargeDate = regInfo.AdmissionInfo.DischargeDetailRecCreatedDate.Value;
                        }
                    }
                }

                // TxD 05/03/2018 : Begin TBR

                PatientSummaryInfoContent.SetPatientHISumInfo(regInfo.PtHISumInfo);

                //PatientSummaryInfoContent.ConfirmedPaperReferal = _confirmedPaperReferal;

                //SetHIDetails_For_PtSummaryInfoCtrl();
                //PatientSummaryInfoContent.ConfirmedHiItem = _confirmedHiItem;
                //PatientSummaryInfoContent.HiBenefit = null;
                if (regInfo.PtInsuranceBenefit.HasValue)
                {
                    //PatientSummaryInfoContent.HiBenefit = regInfo.PtInsuranceBenefit;
                    //PatientSummaryInfoContent.IsCrossRegion = IsCrossRegion.GetValueOrDefault(true);//regInfo.IsCrossRegion.Value;
                }

                //if (regInfo.Patient != null && regInfo.Patient.CurrentHealthInsurance != null)
                //{
                //    PatientSummaryInfoContent.ConfirmedHiItem = regInfo.Patient.CurrentHealthInsurance;                    
                //}

                //if (regInfo.PaperReferal != null)
                //{
                //    PatientSummaryInfoContent.ConfirmedPaperReferal = regInfo.PaperReferal;
                //}

                PatientSummaryInfoContent.SetPatientHISumInfo(regInfo.PtHISumInfo);

                // TxD 05/03/2018 : End TBR
                //▼====: #017
                PatientDetailsContent.IsConfirmedEmergencyPatient = (regInfo.EmergRecID > 0 ? true : false)
                    || (regInfo.AdmissionInfo != null && regInfo.AdmissionInfo.IsConfirmEmergencyTreatment != null
                        && (bool)regInfo.AdmissionInfo.IsConfirmEmergencyTreatment);
                //▲====: #017
                PatientDetailsContent.IsConfirmedForeignerPatient = regInfo.IsForeigner.GetValueOrDefault(false);

                //if (PatientDetailsContent.ShowConfirmedEmergencyPatient)
                //{
                //    PatientDetailsContent.IsConfirmedEmergencyPatient = (regInfo.EmergRecID > 0 ? true : false);
                //}
                //if (PatientDetailsContent.ShowConfirmedForeignerPatient)
                //{
                //    PatientDetailsContent.IsConfirmedForeignerPatient = regInfo.IsForeigner.GetValueOrDefault(false);
                //}
                /*==== #003 ====*/
                IsHICard_FiveYearsCont = regInfo.IsHICard_FiveYearsCont;
                IsHICard_FiveYearsCont_NoPaid = regInfo.IsHICard_FiveYearsCont_NoPaid;
                FiveYearsAppliedDate = regInfo.FiveYearsAppliedDate;
                FiveYearsARowDate = regInfo.FiveYearsARowDate;
                /*==== #003 ====*/
            }
        }

        private bool? IsCrossRegion;
        private PatientRegistration CreateRegistrationFromUserInput()
        {
            var registrationInfo = new PatientRegistration
            {
                PaperReferal = ConfirmedPaperReferal,
                HealthInsurance = ConfirmedHiItem,
                ExamDate = ExamDate,
                StaffID = Globals.LoggedUserAccount.StaffID,
                IsEmergency = IsEmergency,
                IsForeigner = IsForeigner,
                IsHICard_FiveYearsCont = IsHICard_FiveYearsCont,
                IsChildUnder6YearsOld = IsChildUnder6YearsOld,
                EmergInPtReExamination = EmergInPtReExamination,
                IsAllowCrossRegion = IsAllowCrossRegion,
                IsHICard_FiveYearsCont_NoPaid = IsHICard_FiveYearsCont_NoPaid,
                FiveYearsAppliedDate = FiveYearsAppliedDate,
                FiveYearsARowDate = FiveYearsARowDate
            };

            // HPT: Vì thêm trường hợp đăng ký tiền giải phẫu và vãng lai nên phải thêm giá trị V_RegForPatientOfType vào thông tin đăng ký để lưu xuống cho đúng
            registrationInfo.V_RegForPatientOfType = V_RegForPatientOfType;
            // Chỉ khi Nhận bệnh Vãng Lai hoặc Nhận bệnh tiền giải phẫu mới cần chọn khoa
            // Màn hình nhận bệnh nội trú không có combobox chọn khoa mà khoa được chọn là khoa người dùng chọn vào khi đăng nhập
            if (V_RegForPatientOfType == AllLookupValues.V_RegForPatientOfType.DKBN_VANG_LAI || V_RegForPatientOfType == AllLookupValues.V_RegForPatientOfType.NBNT_BN_TIEN_PHAU_KHONG_BHYT || V_RegForPatientOfType == AllLookupValues.V_RegForPatientOfType.DKBN_DT_NGOAI_TRU)
            {
                registrationInfo.DeptID = DepartmentContent.SelectedItem.DeptID;
            }
            if (ConfirmedHiItem == null)
            {
                registrationInfo.IsCrossRegion = null;
            }
            else
            {
                registrationInfo.IsCrossRegion = IsCrossRegion;
            }

            long? hisID = null;
            if (ConfirmedHiItem != null
                && ConfirmedHiItem.HealthInsuranceHistories != null
                && ConfirmedHiItem.HealthInsuranceHistories.Count > 0)
            {
                hisID = ConfirmedHiItem.HealthInsuranceHistories[0].HisID;
            }
            registrationInfo.HIComment = HIComment;
            registrationInfo.HisID = hisID;

            registrationInfo.Patient = _currentPatient;
            registrationInfo.PatientID = _currentPatient.PatientID;
            if (_currentAppointment != null
                && _currentAppointment.AppointmentID > 0
                && _currentAppointment.Patient != null
                && _currentAppointment.Patient.PatientID == registrationInfo.PatientID)
            {
                registrationInfo.AppointmentID = _currentAppointment.AppointmentID;
            }
            if (RegistrationType == AllLookupValues.RegistrationType.NGOAI_TRU)
            {
                registrationInfo.RegTypeID = (byte)PatientRegistrationType.DK_KHAM_BENH_NGOAI_TRU;
                registrationInfo.V_RegistrationType = AllLookupValues.RegistrationType.NGOAI_TRU;
                registrationInfo.PatientClassification = new PatientClassification { PatientClassID = 2 };

                // TxD 22/07/2014 : For In-Patient SET RegistrationStatus according to RegistrationType.
                registrationInfo.RegistrationStatus = AllLookupValues.RegistrationStatus.PENDING;
            }
            else
            {
                registrationInfo.RegTypeID = (byte)PatientRegistrationType.DK_KHAM_BENH_NOI_TRU;

                // TxD 22/07/2014 : For In-Patient JUST USE this ONE AllLookupValues.RegistrationType.NOI_TRU. The other with BHYT and CAP CUU will be of Sub Type                
                registrationInfo.V_RegistrationType = AllLookupValues.RegistrationType.NOI_TRU;

                registrationInfo.PatientClassification = ConfirmedHiItem != null ? new PatientClassification { PatientClassID = 2 } : new PatientClassification { PatientClassID = 1 };

                // TxD 22/07/2014 : For In-Patient SET RegistrationStatus according to RegistrationType.
                registrationInfo.RegistrationStatus = AllLookupValues.RegistrationStatus.PENDING_INPT;

                // TxD 13/12/2014 : Emergency InPatient
                if (PatientDetailsContent.IsConfirmedEmergencyPatient && PatientDetailsContent.ShowConfirmedEmergencyPatient)
                {
                    registrationInfo.EmergRecID = 1;
                    IsEmergency = true;
                }

                // TxD 26/12/2014 : Foreigner InPatient
                if (PatientDetailsContent.IsConfirmedForeignerPatient && PatientDetailsContent.ShowConfirmedForeignerPatient)
                {
                    registrationInfo.IsForeigner = true;
                    IsForeigner = true;
                }

                // TxD 25/01/2015 : 
                if (PatientDetailsContent.EmergInPtReExamination && PatientDetailsContent.ShowEmergInPtReExamination)
                {
                    registrationInfo.EmergInPtReExamination = true;
                    EmergInPtReExamination = true;
                }
                if(V_ObjectMedicalExamination > 0)
                {
                    registrationInfo.V_ObjectMedicalExamination = new Lookup { LookupID = V_ObjectMedicalExamination } ;
                }
            }

            registrationInfo.RefDepartment = _department;

            registrationInfo.HIApprovedStaffID = Globals.LoggedUserAccount.StaffID;

            // TxD 22/07/2014 Commented out the following and set it according to RegistrationType
            //registrationInfo.RegistrationStatus = AllLookupValues.RegistrationStatus.PENDING;

            return registrationInfo;
        }

        public void Handle(AddCompleted<Patient> message)
        {
            if (message != null)
            {
                CanCreateNewRegistration = true;

                // TxD 14/07/2014 The following has been replaced with a call to an equivalent CoRoutine method
                //SetCurrentPatient(message.Item);
                Coroutine.BeginExecute(SetCurrentPatient_CoRoutine(message.Item));
            }
        }
        #region COROUTINES
        public IEnumerator<IResult> DoCalcHiBenefit(HealthInsurance hiItem, PaperReferal referal)
        {
            hiItem.isDoing = false;
            //var calcHiTask = new CalcHiBenefitTask(hiItem, referal, (long)AllLookupValues.RegistrationType.NOI_TRU, IsEmergency, false, IsHICard_FiveYearsCont, IsChildUnder6YearsOld);

            //HPT_20160706: gán loại đăng ký vào chứ không lấy mặc định là nội trú nữa và EmergInPtReExamination lấy đúng giá trị chứ không mặc định là false
            bool tempEmergInPtReExamination = false;
            long tempV_RegistrationType = (long)AllLookupValues.RegistrationType.NOI_TRU;
            if (RegistrationType == AllLookupValues.RegistrationType.NGOAI_TRU)
            {
                tempV_RegistrationType = (long)AllLookupValues.RegistrationType.NGOAI_TRU;
                tempEmergInPtReExamination = EmergInPtReExamination;
            }
            /*==== #003 ====*/
            var calcHiTask = new CalcHiBenefitTask(hiItem, referal, tempV_RegistrationType, IsEmergency, tempEmergInPtReExamination, IsHICard_FiveYearsCont, IsChildUnder6YearsOld, IsAllowCrossRegion, IsHICard_FiveYearsCont_NoPaid);
            /*==== #003 ====*/
            yield return calcHiTask;
            IsAllowCrossRegion = Globals.CheckAllowToCrossRegion(hiItem, RegistrationType);
            if (calcHiTask.Error == null)
            {
                //PatientSummaryInfoContent.HiBenefit = calcHiTask.HiBenefit;
                //PatientSummaryInfoContent.IsCrossRegion = calcHiTask.IsCrossRegion;
                HIBenefit = calcHiTask.HiBenefit;
                IsCrossRegion = calcHiTask.IsCrossRegion;

                Action<IConfirmHiBenefit> onInitDlg = delegate (IConfirmHiBenefit vm)
                {
                    vm.VisibilityCbxAllowCrossRegion = VisibilityCbxAllowCrossRegion;
                    vm.IsAllowCrossRegion = IsAllowCrossRegion;
                    vm.CanEdit = false;
                    vm.CanPressOKButton = false;
                    vm.OriginalHiBenefit = calcHiTask.HiBenefit;
                    vm.HiBenefit = calcHiTask.HiBenefit;

                    if (RegistrationType != AllLookupValues.RegistrationType.NGOAI_TRU)
                    {
                        vm.RebatePercentageLevel1 = calcHiTask.HiBenefit;
                    }

                    vm.HiId = calcHiTask.HiItem.HIID;
                    vm.PatientId = CurrentPatient.PatientID;
                    vm.OriginalIsCrossRegion = calcHiTask.IsCrossRegion;
                    vm.SetCrossRegion(calcHiTask.IsCrossRegion);
                };
                GlobalsNAV.ShowDialog<IConfirmHiBenefit>(onInitDlg);
            }
        }

        string CalcHiBenefitAndRegister_ErrorMsg = "";
        MessageWarningShowDialogTask warnDlgTask = null;
        public IEnumerator<IResult> DoCalcHiBenefitAndRegister(HealthInsurance hiItem, PaperReferal referal)
        {

            if (Globals.ServerConfigSection.CommonItems.UseQMSSystem && PatientFindBy == AllLookupValues.PatientFindBy.NGOAITRU)
            {
                //▼===== 20200603 TTM:
                //Tạo ra đăng ký giả dựa trên thông tin đã nhập của bệnh nhân để kiểm tra vì lúc này CurRegistration vẫn chưa có giá trị (null).
                var regInfoTemp = CreateRegistrationFromUserInput();
                if ((TicketIssueObj == null || TicketIssueObj.TicketNumberSeq <= 0)
                  && regInfoTemp.PtRegistrationID == 0)
                {
                    //▼====: #018
                    if (Globals.ServerConfigSection.CommonItems.BlockRegNoTicket == 2)
                    {
                        string warning = string.Format("{0} ", "Bệnh nhân đang đăng ký không có thông tin số thứ tự");
                        warnConfDlg = new WarningWithConfirmMsgBoxTask(warning, "Xác Nhận");
                        yield return warnConfDlg;
                        if (!warnConfDlg.IsAccept)
                        {
                            yield break;
                        }
                    }
                    else if (Globals.ServerConfigSection.CommonItems.BlockRegNoTicket == 1)
                    {
                        MessageBox.Show("Bệnh nhân đang đăng ký không có thông tin số thứ tự, không thể đăng ký.");
                        yield break;
                    }
                    //▲====: #018
                }

                string ErrStr = "";
                int WarningType = 0;
                if (Globals.ServerConfigSection.CommonItems.CheckPatientInfoQMSSystem
                    && !Globals.CheckValidForTicketQMS(TicketIssueObj, regInfoTemp, out ErrStr, out WarningType))
                {
                    if (WarningType == 2)
                    {
                        if (MessageBox.Show(string.Format(ErrStr), eHCMSResources.K1576_G1_CBao, MessageBoxButton.OK) == MessageBoxResult.OK)
                        {
                            yield break;
                        }
                    }
                    else
                    {
                        if (MessageBox.Show(string.Format(ErrStr), eHCMSResources.K1576_G1_CBao, MessageBoxButton.OKCancel) != MessageBoxResult.OK)
                        {
                            yield break;
                        }
                        IsDiffBetweenRegistrationAndTicket = true;
                    }
                }
            }
            //▲===== 
            hiItem.isDoing = false;

            IsAllowCrossRegion = Globals.CheckAllowToCrossRegion(hiItem, RegistrationType);

            // Hpt 07/12/2015: Không cùng lúc check vào cả hai checkbox Xác nhận bệnh nhân có BHYT 5 năm liên tiếp và trẻ em dưới 6 tuổi (lưu ý: quyền lợi cho hai hình thức xác nhận này là khác nhau)
            if ((IsHICard_FiveYearsCont && IsChildUnder6YearsOld && EmergInPtReExamination) ||
                 (IsHICard_FiveYearsCont && IsChildUnder6YearsOld && !EmergInPtReExamination) ||
                 (IsHICard_FiveYearsCont && !IsChildUnder6YearsOld && EmergInPtReExamination) ||
                 (!IsHICard_FiveYearsCont && IsChildUnder6YearsOld && EmergInPtReExamination))
            {
                MessageBox.Show(eHCMSResources.A0213_G1_Msg_InfoBNHuong1in3QLBHYT);
                yield break;
            }
            /*==== #003 ====*/
            //if (IsHICard_FiveYearsCont)
            if (IsHICard_FiveYearsCont || IsHICard_FiveYearsCont_NoPaid)
            /*==== #003 ====*/
            {
                CalcHiBenefitAndRegister_ErrorMsg = eHCMSResources.Z0221_G1_DKHuongBHYT5Nam;
                warnDlgTask = new MessageWarningShowDialogTask(CalcHiBenefitAndRegister_ErrorMsg, eHCMSResources.Z0222_G1_TiepTucLuuDK);
                yield return warnDlgTask;
                if (!warnDlgTask.IsAccept)
                {
                    IsHICard_FiveYearsCont = false;
                }
            }
            else if (IsChildUnder6YearsOld)
            {
                string ErrorMsg = "";
                string warningMsg = "";
                if (!CheckResult_ChildUnder6YearsOld(out ErrorMsg) && ErrorMsg != "")
                {
                    warningMsg = string.Format("{0}: ", eHCMSResources.Z0215_G1_BNChuaThoaManYC) + ErrorMsg;
                }
                else
                {
                    warningMsg = eHCMSResources.Z0216_G1_XNhanBNTreEmDuoi6Tuoi;
                }

                warnDlgTask = new MessageWarningShowDialogTask(warningMsg, eHCMSResources.Z0222_G1_TiepTucLuuDK);
                yield return warnDlgTask;
                if (!warnDlgTask.IsAccept)
                {
                    IsChildUnder6YearsOld = false;
                }
            }
            else if (EmergInPtReExamination)
            {
                string warningMsg = "";
                string strInvalid = "";
                warningMsg = eHCMSResources.Z0223_G1_XNhanLuuDKCapCuuTK;
                if (CurrentPatient == null || CurrentPatient.LatestRegistration_InPt == null)
                {
                    MessageBox.Show(eHCMSResources.A0746_G1_Msg_InfoKhTheXNhanBNCapCuuTK3);
                    EmergInPtReExamination = false;
                    CanCreateNewRegistration = true;
                    yield break;
                }
                if (CurrentPatient.LatestRegistration_InPt.V_RegForPatientOfType == AllLookupValues.V_RegForPatientOfType.DKBN_VANG_LAI)
                {
                    strInvalid = eHCMSResources.Z0224_G1_XNhanTaoDKBNCapCuuTK;
                }
                else
                {
                    if ((CurrentPatient.LatestRegistration_InPt.HisID.HasValue == false || CurrentPatient.LatestRegistration_InPt.HisID.Value <= 0)
                            || (CurrentPatient.LatestRegistration_InPt.PtInsuranceBenefit.HasValue == false || CurrentPatient.LatestRegistration_InPt.PtInsuranceBenefit <= 0))
                    {
                        MessageBox.Show(eHCMSResources.A0498_G1_Msg_InfoKhTheXNhanBNCapCuuTK2);
                        EmergInPtReExamination = false;
                        CanCreateNewRegistration = true;
                        yield break;
                    }
                    if (CurrentPatient.LatestRegistration_InPt.DischargeDate == null)
                    {
                        MessageBox.Show(eHCMSResources.A0497_G1_Msg_InfoKhTheXNhanBNCapCuuTK);
                        EmergInPtReExamination = false;
                        CanCreateNewRegistration = true;
                        yield break;
                    }
                    if (CurrentPatient.LatestRegistration_InPt.EmergRecID.GetValueOrDefault() > 0 || CurrentPatient.LatestRegistration_InPt.AdmDeptID == Globals.ServerConfigSection.InRegisElements.EmerDeptID)
                    {
                        if (_currentAppointment != null && !_currentAppointment.IsEmergInPtReExamApp)
                        {
                            strInvalid = eHCMSResources.Z0225_G1_DKTaoTuCuocHenKgPhaiCCTK;
                        }
                        else
                        {
                            strInvalid = eHCMSResources.Z0224_G1_XNhanTaoDKBNCapCuuTK;
                        }
                    }
                    else
                    {
                        strInvalid = string.Format("{0}: \n- {1} \n- {2}", eHCMSResources.Z0515_G1_DKNoiTruSauCungCuaBN, eHCMSResources.Z0226_G1_NVVaoKhoaCapCuu, eHCMSResources.Z0227_G1_KgDuocXNhanBNCC);
                    }
                }
                warnDlgTask = new MessageWarningShowDialogTask(strInvalid, warningMsg);
                yield return warnDlgTask;
                if (!warnDlgTask.IsAccept)
                {
                    EmergInPtReExamination = false;
                }
            }
            //if (PatientDetailsContent.IsAllowCrossRegion && PatientDetailsContent.HealthInsuranceContent.ConfirmedItem != null && !Globals.CrossRegionHospital.Any(x=>x.HosID == (PatientDetailsContent.HealthInsuranceContent.ConfirmedItem).HosID))
            //{
            //    warnDlgTask = new MessageWarningShowDialogTask("Nơi đăng ký KCBBD không được cấu hình thông tuyến!", "Tiếp tục lưu đăng ký thông tuyến");
            //    yield return warnDlgTask;
            //    if (!warnDlgTask.IsAccept)
            //    {
            //        PatientDetailsContent.IsAllowCrossRegion = false;
            //    }
            //}

            // Hpt 10/12/2015: Đã kiểm tra ngoại trú trái tuyến hai lần trước khi vào đây, tuy nhiên, người dùng có thể đổi ý không xác nhận khi hiện cảnh báo lên nên phải kiểm tra thêm lần nữa ở đây
            // nếu không thì khi người dùng đổi ý, dấu tick được thu hồi nhưng phần mềm không kiểm tra lại giấy chuyển viện sẽ dẫn đến sai sót

            //▼====== #009
            string strHospitalCodeConfirm = "";
            if (ConfirmedHiItem != null)
            {
                strHospitalCodeConfirm = ConfirmedHiItem.RegistrationCode;
            }
            bool TheBHYT_KhongDoBV_CapPhat = Globals.ServerConfigSection.Hospitals.HospitalCode != strHospitalCodeConfirm;
            //▲====== #009

            bool Conditions_Not_Allow_OutPt_TraiTuyen = !EmergInPtReExamination && !IsChildUnder6YearsOld && !IsAllowCrossRegion && (referal == null || referal.IsChecked == false);
            if (Conditions_Not_Allow_OutPt_TraiTuyen && TheBHYT_KhongDoBV_CapPhat)
            {
                MessageBox.Show(eHCMSResources.A0251_G1_Msg_InfoBNNgTruKhDcTraiTuyen);
                CanCreateNewRegistration = true;
                yield break;
            }

            /*==== #003 ====*/
            //var calcHiTask = new CalcHiBenefitTask(hiItem, referal, (long)AllLookupValues.RegistrationType.NGOAI_TRU, IsEmergency, EmergInPtReExamination, IsHICard_FiveYearsCont, IsChildUnder6YearsOld, IsAllowCrossRegion);
            var calcHiTask = new CalcHiBenefitTask(hiItem, referal, (long)AllLookupValues.RegistrationType.NGOAI_TRU, IsEmergency, EmergInPtReExamination, IsHICard_FiveYearsCont, IsChildUnder6YearsOld, IsAllowCrossRegion, IsHICard_FiveYearsCont_NoPaid);
            /*==== #003 ====*/
            yield return calcHiTask;
            if (calcHiTask.Error != null)
            {
                yield break;
            }
            PatientSummaryInfoContent.ThongTuyen = IsAllowCrossRegion;

            //PatientSummaryInfoContent.HiBenefit = calcHiTask.HiBenefit;
            //PatientSummaryInfoContent.IsCrossRegion = calcHiTask.IsCrossRegion;

            HIBenefit = calcHiTask.HiBenefit;
            IsCrossRegion = calcHiTask.IsCrossRegion;

            //▼====== #010
            //var confirmHI = new ConfirmHiBenefitDialogTask(calcHiTask.HiBenefit, calcHiTask.HiBenefit
            //    , calcHiTask.HiItem.HIID, CurrentPatient.PatientID, calcHiTask.IsCrossRegion, (long)AllLookupValues.RegistrationType.NGOAI_TRU, IsAllowCrossRegion, VisibilityCbxAllowCrossRegion);
            //yield return confirmHI;

            //switch (confirmHI.confirmHiBenefitEvent.confirmHiBenefitEnum)
            //{
            //    case ConfirmHiBenefitEnum.ConfirmHiBenefit:
            //        ConfirmHiBenefit(confirmHI.confirmHiBenefitEvent);
            //        break;
            //    case ConfirmHiBenefitEnum.NoChangeConfirmHiBenefit:
            //        NoChangeConfirmHiBenefit(confirmHI.confirmHiBenefitEvent);
            //        break;
            //    case ConfirmHiBenefitEnum.RemoveConfirmedHiCard:
            //        RemoveConfirmedHiCard(confirmHI.confirmHiBenefitEvent);
            //        break;
            //}
            //▲====== #010
            RegisterPatient();

        }
        #endregion

        public void authorization()
        {
            if (!Globals.isAccountCheck)
            {
                return;
            }
        }
        #region checking account

        private bool _mNhanBenh_ThongTin_Sua = true;
        private bool _mNhanBenh_TheBH_ThemMoi = true;
        private bool _mNhanBenh_TheBH_XacNhan = true;
        private bool _mNhanBenh_DangKy = true;
        private bool _mNhanBenh_TheBH_Sua = true;

        public bool mNhanBenh_ThongTin_Sua
        {
            get
            {
                return _mNhanBenh_ThongTin_Sua;
            }
            set
            {
                if (_mNhanBenh_ThongTin_Sua == value)
                    return;
                _mNhanBenh_ThongTin_Sua = value;
                NotifyOfPropertyChange(() => mNhanBenh_ThongTin_Sua);
            }
        }


        public bool mNhanBenh_TheBH_ThemMoi
        {
            get
            {
                return _mNhanBenh_TheBH_ThemMoi;
            }
            set
            {
                if (_mNhanBenh_TheBH_ThemMoi == value)
                    return;
                _mNhanBenh_TheBH_ThemMoi = value;
                NotifyOfPropertyChange(() => mNhanBenh_TheBH_ThemMoi);
            }
        }

        private bool _mInPt_ConfirmHI_Only = false;
        public bool mInPt_ConfirmHI_Only
        {
            get { return _mInPt_ConfirmHI_Only; }
            set
            {
                _mInPt_ConfirmHI_Only = value;
                if (_mInPt_ConfirmHI_Only)
                {
                    mNhanBenh_DangKy = false;
                    if (PatientDetailsContent != null)
                    {
                        PatientDetailsContent.mNhanBenh_DangKy = false;
                    }
                }
                NotifyOfPropertyChange(() => mInPt_ConfirmHI_Only);
                NotifyOfPropertyChange(() => IsCanApplyFiveYearsDate);
                //tma/ PatientDetailsContent.PatientFindBy = mInPt_ConfirmHI_Only == true ? (int)AllLookupValues.PatientFindBy.NOITRU : (int)AllLookupValues.PatientFindBy.NGOAITRU;
            }
        }
        //HPT 24/08/2015: Khai báo thuộc tính lưu giá trị checkbox Xác nhận BN có BHYT 5 năm liên tiếp phục vụ cho việc tính quyền lợi bảo hiểm
        //Không set giá trị mặc định <=> giá trị mặc định =  false
        private bool _IsHICard_FiveYearsCont;
        public bool IsHICard_FiveYearsCont
        {
            get { return _IsHICard_FiveYearsCont; }
            set
            {
                _IsHICard_FiveYearsCont = value;
                NotifyOfPropertyChange(() => IsHICard_FiveYearsCont);
                PatientDetailsContent.IsHICard_FiveYearsCont = IsHICard_FiveYearsCont;
                if (!IsHICard_FiveYearsCont)
                {
                    IsHICard_FiveYearsCont_NoPaid = false;
                    NotifyOfPropertyChange(() => IsHICard_FiveYearsCont_NoPaid);
                }
                NotifyOfPropertyChange(() => IsCanApplyFiveYearsDate);
            }
        }
        //▼====: #003
        private bool _IsHICard_FiveYearsCont_NoPaid = false;
        public bool IsHICard_FiveYearsCont_NoPaid
        {
            get { return _IsHICard_FiveYearsCont_NoPaid; }
            set
            {
                _IsHICard_FiveYearsCont_NoPaid = value;
                NotifyOfPropertyChange(() => IsHICard_FiveYearsCont_NoPaid);
                if (IsHICard_FiveYearsCont_NoPaid)
                {
                    IsHICard_FiveYearsCont = true;
                    PatientDetailsContent.IsHICard_FiveYearsCont = IsHICard_FiveYearsCont;
                    NotifyOfPropertyChange(() => IsHICard_FiveYearsCont);
                }
                NotifyOfPropertyChange(() => IsCanApplyFiveYearsDate);
            }
        }
        //▲====: #003
        //▼====: #006
        private DateTime? _FiveYearsAppliedDate = Globals.GetCurServerDateTime();
        public DateTime? FiveYearsAppliedDate
        {
            get
            {
                return _FiveYearsAppliedDate;
            }
            set
            {
                _FiveYearsAppliedDate = value;
                NotifyOfPropertyChange(() => FiveYearsAppliedDate);
            }
        }
        private DateTime? _FiveYearsARowDate = Globals.GetCurServerDateTime();
        public DateTime? FiveYearsARowDate
        {
            get
            {
                return _FiveYearsARowDate;
            }
            set
            {
                _FiveYearsARowDate = value;
                NotifyOfPropertyChange(() => FiveYearsARowDate);
            }
        }
        public bool IsCanApplyFiveYearsDate
        {
            get { return IsHICard_FiveYearsCont && !IsHICard_FiveYearsCont_NoPaid && !_IsShowGetTicketButton /*&& mInPt_ConfirmHI_Only*/; }
        }
        //▲====: #006


        public bool mNhanBenh_TheBH_XacNhan
        {
            get
            {
                return _mNhanBenh_TheBH_XacNhan;
            }
            set
            {
                if (_mNhanBenh_TheBH_XacNhan == value)
                    return;
                _mNhanBenh_TheBH_XacNhan = value;
                NotifyOfPropertyChange(() => mNhanBenh_TheBH_XacNhan);
            }
        }

        public bool mNhanBenh_TheBH_Sua
        {
            get
            {
                return _mNhanBenh_TheBH_Sua;
            }
            set
            {
                if (_mNhanBenh_TheBH_Sua == value)
                    return;
                _mNhanBenh_TheBH_Sua = value;
                NotifyOfPropertyChange(() => mNhanBenh_TheBH_Sua);
            }
        }

        public bool mNhanBenh_DangKy
        {
            get
            {
                return _mNhanBenh_DangKy;
            }
            set
            {
                if (_mNhanBenh_DangKy == value)
                    return;
                _mNhanBenh_DangKy = value;
                NotifyOfPropertyChange(() => mNhanBenh_DangKy);
            }
        }


        //phan nay nam trong module chung
        private bool _mPatient_TimBN = true;
        private bool _mPatient_ThemBN = true;
        private bool _mPatient_TimDangKy = true;

        private bool _mInfo_CapNhatThongTinBN = true;
        private bool _mInfo_XacNhan = true;
        private bool _mInfo_XoaThe = true;
        private bool _mInfo_XemPhongKham = true;

        public bool mPatient_TimBN
        {
            get
            {
                return _mPatient_TimBN;
            }
            set
            {
                if (_mPatient_TimBN == value)
                    return;
                _mPatient_TimBN = value;
                NotifyOfPropertyChange(() => mPatient_TimBN);
            }
        }

        public bool mPatient_ThemBN
        {
            get
            {
                return _mPatient_ThemBN;
            }
            set
            {
                if (_mPatient_ThemBN == value)
                    return;
                _mPatient_ThemBN = value;
                NotifyOfPropertyChange(() => mPatient_ThemBN);
            }
        }

        public bool mPatient_TimDangKy
        {
            get
            {
                return _mPatient_TimDangKy;
            }
            set
            {
                if (_mPatient_TimDangKy == value)
                    return;
                _mPatient_TimDangKy = value;
                NotifyOfPropertyChange(() => mPatient_TimDangKy);
            }
        }

        public bool mInfo_CapNhatThongTinBN
        {
            get
            {
                return _mInfo_CapNhatThongTinBN;
            }
            set
            {
                if (_mInfo_CapNhatThongTinBN == value)
                    return;
                _mInfo_CapNhatThongTinBN = value;
                NotifyOfPropertyChange(() => mInfo_CapNhatThongTinBN);
            }
        }

        public bool mInfo_XacNhan
        {
            get
            {
                return _mInfo_XacNhan;
            }
            set
            {
                if (_mInfo_XacNhan == value)
                    return;
                _mInfo_XacNhan = value;
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

        public void Handle(ItemSelecting<object, PatientAppointment> message)
        {
            if (GetView() == null)
                return;

            _currentAppointment = message.Item;
            message.Item.Patient.FromAppointment = true;
            Globals.EventAggregator.Publish(new ResultFound<Patient> { Result = message.Item.Patient });
            if (message.Sender is IFindAppointment)
            {
                ((ViewModelBase)message.Sender).TryClose();
            }
        }

        public IEnumerator<IResult> DoCheckAppointment(ItemSelecting<object, PatientAppointment> message)
        {
            if (message.Item.V_ApptStatus != AllLookupValues.ApptStatus.BOOKED)
            {
                var dialog = new MessageWarningShowDialogTask(eHCMSResources.Z0192_G1_CuocHenNayDaDuocXNhan, eHCMSResources.Z0195_G1_VanMoCuocHen);
                yield return dialog;
                if (dialog.IsAccept)
                {
                    //tiep tuc lam cong chuyen nay      
                    CheckAppointment(message);
                }
                yield break;
            }
            CheckAppointment(message);
        }

        public void CheckAppointment(ItemSelecting<object, PatientAppointment> message)
        {
            if (_currentPatient != null)
            {
                if (message.Item.StaffID != _currentPatient.PatientID)
                {
                    var str = string.Format("{0} {1}. \n{2} {3}?", eHCMSResources.Z0175_G1_BanDangThaoTacBN, _currentPatient.FullName
                        , eHCMSResources.Z0191_G1_BanCoMuonChSangDKBN, message.Item.Patient.FullName);

                    if (MessageBox.Show(str, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                    {
                        message.Cancel = true;
                        return;
                    }
                }
            }
            if (message.Sender is IFindAppointment)
            {
                ((ViewModelBase)message.Sender).TryClose();
            }

            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginGetPatientByAppointment(message.Item,
                            Globals.DispatchCallback(asyncResult =>
                            {
                                bool bOK;
                                PatientRegistration regInfo = null;
                                try
                                {
                                    regInfo = contract.EndGetPatientByAppointment(asyncResult);
                                    bOK = true;
                                }
                                catch (FaultException<AxException> fault)
                                {
                                    ClientLoggerHelper.LogInfo(fault.ToString());
                                    bOK = false;
                                }
                                catch (Exception ex)
                                {
                                    ClientLoggerHelper.LogInfo(ex.ToString());
                                    bOK = false;
                                }
                                finally
                                {
                                    //IsLoadingAppointment = false;
                                }
                                if (bOK)
                                {
                                    CurrentPatient = regInfo.Patient;
                                    CurrentPatient.latestHIRegistration = regInfo;
                                }
                            }), null);
                    }
                }
                catch (Exception ex)
                {
                    //IsLoadingAppointment = false;
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                }
            });
            t.Start();
        }

        public void ApplyHiToInPatientRegistration()
        {
            if (ConfirmedHiItem == null)
            {
                MessageBox.Show(eHCMSResources.A0399_G1_Msg_InfoChuaChonTheBHYTDeXN);
                return;
            }
            long? PaperReferalID = null;
            if (ConfirmedPaperReferal != null)
            {
                PaperReferalID = ConfirmedPaperReferal.RefID;
            }

            //var benefit = PatientSummaryInfoContent.HiBenefit;

            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginApplyHiToInPatientRegistration(RegistrationID.GetValueOrDefault(0),
                            //HIID,HisID,HiBenefit,IsCrossRegion,PaperReferalID
                            ConfirmedHiItem.HIID, HIBenefit, IsCrossRegion.GetValueOrDefault(), PaperReferalID, (int)AllLookupValues.V_FindPatientType.NGOAI_TRU, IsEmergency,
                            Globals.LoggedUserAccount.StaffID.GetValueOrDefault(), IsHICard_FiveYearsCont, IsChildUnder6YearsOld, (TypesOf_ConfirmHICardForInPt)1,
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                try
                                {
                                    var bOK = contract.EndApplyHiToInPatientRegistration(asyncResult);
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
            });
            t.Start();
        }

        string HIComment = "";
        public void Handle(ConfirmHiBenefit message)
        {
            if (ConfirmedHiItem == null || ConfirmedHiItem.HIID != message.HiId)
            {
                return;
            }

            IsCrossRegion = message.IsCrossRegion.GetValueOrDefault();
            //PatientSummaryInfoContent.HiBenefit = message.HiBenefit;
            //PatientSummaryInfoContent.IsCrossRegion = message.IsCrossRegion.GetValueOrDefault(true);
            HIBenefit = (double)message.HiBenefit;
            IsCrossRegion = message.IsCrossRegion.GetValueOrDefault(true);

            HIComment = message.HIComment;

            if (CurrentRegMode == RegistrationFormMode.OLD_REGISTRATION_OPENED && RegistrationID > 0)
            {
                ApplyHiToInPatientRegistration();
            }
        }

        public void Handle(RemoveConfirmedHiCard message)
        {
            if (GetView() == null || message.HiId <= 0)
            {
                return;
            }

            if (ConfirmedHiItem != null && ConfirmedHiItem.HIID == message.HiId)
            {
                ConfirmedHiItem = null;
                ConfirmedPaperReferal = null;

                //PatientSummaryInfoContent.HiBenefit = null;
                //PatientSummaryInfoContent.ConfirmedHiItem = null;
                //PatientSummaryInfoContent.ConfirmedPaperReferal = null;
                //SetHIDetails_For_PtSummaryInfoCtrl(true); 
                PatientSummaryInfoContent.SetPatientHISumInfo(null);
            }
        }

        public void NoChangeConfirmHiBenefit(ConfirmHiBenefitEvent message)
        {
            if (message != null)
            {
                HIComment = message.HIComment;

                if (ConfirmedHiItem == null || ConfirmedHiItem.HIID != message.HiId)
                {
                    return;
                }
                if (CurrentRegMode == RegistrationFormMode.OLD_REGISTRATION_OPENED && RegistrationID > 0)
                {
                    ApplyHiToInPatientRegistration();
                }
                ConfirmedHiItem.isDoing = true;
                ConfirmedHiItem.EditLocked = true;
            }
        }

        public void ConfirmHiBenefit(ConfirmHiBenefitEvent message)
        {
            if (ConfirmedHiItem == null || ConfirmedHiItem.HIID != message.HiId)
            {
                return;
            }
            ConfirmedHiItem.isDoing = true;
            ConfirmedHiItem.EditLocked = false;
            ConfirmedHiItem.HIPatientBenefit = Double.Parse(message.HiBenefit.GetValueOrDefault(0).ToString());
            IsCrossRegion = message.IsCrossRegion;
            //PatientSummaryInfoContent.HiBenefit = message.HiBenefit;
            //PatientSummaryInfoContent.IsCrossRegion = message.IsCrossRegion.GetValueOrDefault(true);
            HIComment = message.HIComment;

            if (CurrentRegMode == RegistrationFormMode.OLD_REGISTRATION_OPENED && RegistrationID > 0)
            {
                ApplyHiToInPatientRegistration();
            }
        }

        public void RemoveConfirmedHiCard(ConfirmHiBenefitEvent message)
        {
            if (GetView() == null || message.HiId <= 0)
            {
                return;
            }

            if (ConfirmedHiItem != null && ConfirmedHiItem.HIID == message.HiId)
            {
                ConfirmedHiItem = null;
                ConfirmedPaperReferal = null;
                //PatientSummaryInfoContent.HiBenefit = null;
                //PatientSummaryInfoContent.ConfirmedHiItem = null;
                //PatientSummaryInfoContent.ConfirmedPaperReferal = null;
                //SetHIDetails_For_PtSummaryInfoCtrl(true);     
                PatientSummaryInfoContent.SetPatientHISumInfo(null);
            }
        }

        //--dinh viet lai cho nay
        public void Handle(ConfirmHiBenefitEvent message)
        {
            if (message != null)
            {
                PatientSummaryInfoContent.HiComment = message.HIComment;
                switch (message.confirmHiBenefitEnum)
                {
                    case ConfirmHiBenefitEnum.ConfirmHiBenefit:
                        ConfirmHiBenefit(message);
                        break;
                    case ConfirmHiBenefitEnum.NoChangeConfirmHiBenefit:
                        NoChangeConfirmHiBenefit(message);
                        break;
                    case ConfirmHiBenefitEnum.RemoveConfirmedHiCard:
                        RemoveConfirmedHiCard(message);
                        break;
                }

            }
        }

        public bool CanReportRegistrationInfoInsuranceCmd
        {
            get
            {
                if (RegistrationType == AllLookupValues.RegistrationType.NGOAI_TRU)
                {
                    return !CanCreateNewRegistration && CurrentPatient != null && CurrentPatient.latestHIRegistration != null && CurrentPatient.latestHIRegistration.HisID.GetValueOrDefault(0) > 0;
                }
                else
                {
                    //return !CanCreateNewRegistration && CurrentPatient != null && CurrentPatient.LatestRegistration_InPt != null && CurrentPatient.LatestRegistration_InPt.HisID.GetValueOrDefault(0) > 0;
                    // TxD 19/12/2014 Just return true here and check when button is actually pressed.
                    return true;
                }
            }
        }

        public bool CanInTemplate38Cmd
        {
            get
            {
                if (RegistrationType == AllLookupValues.RegistrationType.NGOAI_TRU)
                {
                    return !CanCreateNewRegistration && CurrentPatient != null && CurrentPatient.latestHIRegistration != null && CurrentPatient.latestHIRegistration.HisID.GetValueOrDefault(0) > 0;
                }
                else
                {
                    return false;
                }
            }
        }

        public void ReportRegistrationInfoInsuranceCmd()
        {
            if (RegistrationType == AllLookupValues.RegistrationType.NGOAI_TRU)
            {
                if (CurrentPatient != null && CurrentPatient.latestHIRegistration != null && CurrentPatient.latestHIRegistration.HisID.GetValueOrDefault(0) > 0)
                {
                    ShowHiConfirmationReport(CurrentPatient.latestHIRegistration.PtRegistrationID);
                }
            }
            else
            {
                //return !CanCreateNewRegistration && CurrentPatient != null && CurrentPatient.LatestRegistration_InPt != null && CurrentPatient.LatestRegistration_InPt.HisID.GetValueOrDefault(0) > 0;
                if (CurRegistration == null || CurRegistration.PtRegistrationID <= 0)
                {
                    MessageBox.Show(eHCMSResources.A0733_G1_Msg_InfoKhTimThayDK, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                    return;
                }
                if (CurRegistration.HisID.GetValueOrDefault() <= 0)
                {
                    MessageBox.Show(eHCMSResources.A0493_G1_Msg_InfoKhTheInGiayXNhanBHYT, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                    return;
                }
                Action<ICommonPreviewView> onInitDlg = delegate (ICommonPreviewView reportVm)
                {
                    reportVm.RegistrationID = CurRegistration.PtRegistrationID;
                    reportVm.eItem = ReportName.REGISTRATION_IN_PATIENT_HI_CONFIRMATION;
                };
                GlobalsNAV.ShowDialog<ICommonPreviewView>(onInitDlg);
            }
        }

        private void PrintConfirmHIReportSilently(long ptRegistrationID)
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ReportServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginGetConfirmHIReportInPdfFormat(ptRegistrationID,
                                Globals.DispatchCallback(asyncResult =>
                                {
                                    try
                                    {
                                        var data = contract.EndGetConfirmHIReportInPdfFormat(asyncResult);

                                        var printEvt = new ActiveXPrintEvt(this, PrinterType.IN_PHIEU, data, ActiveXPrintType.ByteArray);
                                        Globals.EventAggregator.Publish(printEvt);
                                    }
                                    catch (Exception ex)
                                    {
                                        ClientLoggerHelper.LogInfo(ex.ToString());
                                        MessageBox.Show(eHCMSResources.A0694_G1_Msg_InfoKhTheLayDataInHD);
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

        public void InTemplate38Cmd()
        {
            if (RegistrationType == AllLookupValues.RegistrationType.NGOAI_TRU)
            {
                if (CurrentPatient != null && CurrentPatient.latestHIRegistration != null && CurrentPatient.latestHIRegistration.HisID.GetValueOrDefault(0) > 0)
                {
                    Action<ICommonPreviewView> onInitDlg = delegate (ICommonPreviewView proAlloc)
                    {
                        proAlloc.ID = CurrentPatient.latestHIRegistration.PtRegistrationID;
                        proAlloc.eItem = ReportName.TEMP38a;
                        proAlloc.parTypeOfForm = Globals.ServerConfigSection.OutRegisElements.AllowToChooseTypeOf01Form == true ? 1 : 0;
                    };
                    GlobalsNAV.ShowDialog<ICommonPreviewView>(onInitDlg);
                }
            }
            //else
            //{
            //    if (CurrentPatient != null && CurrentPatient.LatestRegistration_InPt != null && CurrentPatient.LatestRegistration_InPt.HisID.GetValueOrDefault(0) > 0)
            //    {
            //        var proAlloc = Globals.GetViewModel<ICommonPreviewView>();
            //        proAlloc.ID = CurrentPatient.LatestRegistration_InPt.PtRegistrationID;
            //        proAlloc.eItem = ReportName.TEMP38aNoiTru;
            //        var instance = proAlloc as Conductor<object>;
            //        Globals.ShowDialog(instance, (o) => { });
            //    }
            //}
        }
        public IEnumerator<IResult> DoCancelRegistration_InPt(PatientRegistration CurRegistration)
        {
            if (CurRegistration == null)
            {
                _msgTask = new MessageBoxTask(string.Format("{0}?", eHCMSResources.Z0609_G1_ChuaCoDK), eHCMSResources.G0442_G1_TBao);
                yield return _msgTask;
                yield break;
            }
            //truoc khi huy load lai dang ky xem co duoc dang ki dich vu chua
            LoadRegistrationSwitch loadtask = new LoadRegistrationSwitch();
            loadtask.IsGetPatient = true;
            loadtask.IsGetRegistration = true;
            loadtask.IsGetAdmissionInfo = true;
            var loadRegTask = new LoadRegistrationInfo_InPtTask(CurRegistration.PtRegistrationID, CurRegistration.FindPatient, loadtask);
            yield return loadRegTask;
            CurRegistration = loadRegTask.Registration;
            if (CurRegistration != null && Globals.IsLockRegistration(CurRegistration.RegLockFlag, eHCMSResources.T1743_G1_HuyDK.ToLower()))
            {
                yield break;
            }
            System.Windows.Application.Current.Dispatcher.Invoke(() => { RegistrationCancelling = true; });
            //==== #002
            CurRegistration.RegCancelStaffID = Globals.LoggedUserAccount.StaffID.Value;
            //==== #002
            var cancelRegTask = new CancelRegistrationTask(CurRegistration);
            yield return cancelRegTask;

            if (cancelRegTask.Error == null && cancelRegTask.RegistrationInfo != null)
            {
                Coroutine.BeginExecute(DoSetCurrentPatient_InPt(CurrentPatient, true));
                //System.Windows.Application.Current.Dispatcher.Invoke(() => MessageBox.Show(eHCMSResources.A0613_G1_Msg_InfoHuyOK));
                System.Windows.Application.Current.Dispatcher.Invoke(() => GlobalsNAV.ShowMessagePopup(eHCMSResources.A0613_G1_Msg_InfoHuyOK));
                CanCancelRegistrationInfoCmd = false;
                if (ApplyCheckInPtRegistration)
                {
                    UpdateInPtRegistrationID_PtRegistration(CurRegistration.PtRegistrationID);
                }
            }

            System.Windows.Application.Current.Dispatcher.Invoke(() => { RegistrationCancelling = false; });
        }
        public IEnumerator<IResult> DoCancelRegistration(PatientRegistration CurRegistration)
        {
            if (CurRegistration == null)
            {
                _msgTask = new MessageBoxTask(string.Format("{0}?", eHCMSResources.Z0609_G1_ChuaCoDK), eHCMSResources.G0442_G1_TBao);
                yield return _msgTask;
                yield break;
            }
            //truoc khi huy load lai dang ky xem co duoc dang ki dich vu chua
            var loadRegTask = new LoadRegistrationInfoTask(CurRegistration.PtRegistrationID, true);
            yield return loadRegTask;
            CurRegistration = loadRegTask.Registration;
            if (CurRegistration != null && Globals.IsLockRegistration(CurRegistration.RegLockFlag, eHCMSResources.T1743_G1_HuyDK.ToLower()))
            {
                yield break;
            }
            if (RegistrationInfoHasChanged)
            {
                _msgTask = new MessageBoxTask(string.Format("{0}.", eHCMSResources.Z0196_G1_TTinThayDoiPhaiLuuTruoc), eHCMSResources.G0442_G1_TBao);
                yield return _msgTask;
                yield break;
            }

            if (CurRegistration.RegistrationStatus != AllLookupValues.RegistrationStatus.OPENED &&
                CurRegistration.RegistrationStatus != AllLookupValues.RegistrationStatus.PENDING)
            {
                _msgTask = new MessageBoxTask(string.Format("{0}.", eHCMSResources.Z0197_G1_KgTheHuyDKNay), eHCMSResources.G0442_G1_TBao);
                yield return _msgTask;
                yield break;
            }


            if (CurRegistration.DrugInvoices != null && CurRegistration.DrugInvoices.Count > 0)
            {
                throw new Exception(eHCMSResources.Z0198_G1_DaLayThuocKgTheHuyDK);
            }

            if (CurRegistration.PatientRegistrationDetails != null)
            {
                foreach (var regDetail in CurRegistration.PatientRegistrationDetails)
                {
                    if (regDetail.ExamRegStatus == AllLookupValues.ExamRegStatus.HOAN_TAT)
                    {
                        _msgTask = new MessageBoxTask(string.Format("{0}.", eHCMSResources.Z0199_G1_DaKBKgTheHuyDK), eHCMSResources.G0442_G1_TBao);
                        yield return _msgTask;
                        yield break;
                    }
                }
            }
            if (CurRegistration.PCLRequests != null)
            {
                foreach (var request in CurRegistration.PCLRequests)
                {
                    if (request.PatientPCLRequestIndicators != null)
                    {
                        request.ExamRegStatus = AllLookupValues.ExamRegStatus.NGUNG_TRA_TIEN_LAI;
                        foreach (var requestDetails in request.PatientPCLRequestIndicators)
                        {
                            if (requestDetails.ExamRegStatus == AllLookupValues.ExamRegStatus.HOAN_TAT)
                            {
                                _msgTask = new MessageBoxTask(string.Format("{0}.", eHCMSResources.Z0200_G1_DaLamCLSKgTheHuyDK), eHCMSResources.G0442_G1_TBao);
                                yield return _msgTask;
                                yield break;
                            }
                        }
                    }
                }
            }
            //Kiểm tra các dịch vụ có được hủy và trả tiền lại hết chưa

            if (CurRegistration.PatientTransaction != null)
            {
                if (CurRegistration.PatientTransaction.PatientTransactionPayments != null
                    && CurRegistration.PatientTransaction.PatientTransactionPayments.Count > 0)
                    foreach (var item in CurRegistration.PatientTransaction.PatientTransactionPayments)
                    {
                        if (item.IsDeleted == null
                            || item.IsDeleted == false)
                        {
                            //_msgTask = new MessageBoxTask(eHCMSResources.Z0201_G1_DKConDVChuaHuyHoacChuaHoanTien
                            //    + "\nHủy và hoàn tiền tất cả các dịch vụ trước khi hủy đăng ký!", eHCMSResources.G0442_G1_TBao);
                            //yield return _msgTask;
                            //yield break;

                            var dialog = new MessageWarningShowDialogTask(string.Format("{0} \n{1}"
                                , eHCMSResources.Z0201_G1_DKConDVChuaHuyHoacChuaHoanTien, eHCMSResources.Z0202_G1_HuyVaHoanTienDVTruocKhiHuyDK), eHCMSResources.G0442_G1_TBao, false);
                            yield return dialog;
                            yield break;
                        }
                    }
            }

            System.Windows.Application.Current.Dispatcher.Invoke(() => { RegistrationCancelling = true; });
            //==== #002
            CurRegistration.RegCancelStaffID = Globals.LoggedUserAccount.StaffID.Value;
            //==== #002
            var cancelRegTask = new CancelRegistrationTask(CurRegistration);
            yield return cancelRegTask;

            if (cancelRegTask.Error == null && cancelRegTask.RegistrationInfo != null)
            {
                Coroutine.BeginExecute(DoSetCurrentPatient(CurrentPatient, true));
                //System.Windows.Application.Current.Dispatcher.Invoke(() => MessageBox.Show(eHCMSResources.A0613_G1_Msg_InfoHuyOK));
                System.Windows.Application.Current.Dispatcher.Invoke(() => GlobalsNAV.ShowMessagePopup(eHCMSResources.A0613_G1_Msg_InfoHuyOK));
            }

            System.Windows.Application.Current.Dispatcher.Invoke(() => { RegistrationCancelling = false; });

        }
        //public IEnumerator<IResult> DoCancelRegistration(PatientRegistration CurRegistration)
        //{
        //    if (CurRegistration == null)
        //    {
        //        _msgTask = new MessageBoxTask(string.Format("{0}?", eHCMSResources.Z0609_G1_ChuaCoDK), eHCMSResources.G0442_G1_TBao);
        //        yield return _msgTask;
        //        yield break;
        //    }

        //    if (CurRegistration.RegistrationStatus != AllLookupValues.RegistrationStatus.OPENED &&
        //        CurRegistration.RegistrationStatus != AllLookupValues.RegistrationStatus.PENDING)
        //    {
        //        _msgTask = new MessageBoxTask(string.Format("{0}.", eHCMSResources.Z0197_G1_KgTheHuyDKNay), eHCMSResources.G0442_G1_TBao);
        //        yield return _msgTask;
        //        yield break;
        //    }


        //    if (CurRegistration.DrugInvoices != null && CurRegistration.DrugInvoices.Count > 0)
        //    {
        //        throw new Exception("Đã lấy thuốc. Không thể hủy đăng ký.");
        //    }

        //    if (CurRegistration.PatientRegistrationDetails != null)
        //    {
        //        foreach (var regDetail in CurRegistration.PatientRegistrationDetails)
        //        {
        //            if (regDetail.ExamRegStatus == AllLookupValues.ExamRegStatus.HOAN_TAT)
        //            {
        //                _msgTask = new MessageBoxTask(string.Format("{0}.", eHCMSResources.Z0199_G1_DaKBKgTheHuyDK), eHCMSResources.G0442_G1_TBao);
        //                yield return _msgTask;
        //                yield break;
        //            }
        //        }
        //    }
        //    if (CurRegistration.PCLRequests != null)
        //    {
        //        foreach (var request in CurRegistration.PCLRequests)
        //        {
        //            if (request.PatientPCLRequestIndicators != null)
        //            {
        //                request.ExamRegStatus = AllLookupValues.ExamRegStatus.NGUNG_TRA_TIEN_LAI;
        //                foreach (var requestDetails in request.PatientPCLRequestIndicators)
        //                {
        //                    if (requestDetails.ExamRegStatus == AllLookupValues.ExamRegStatus.HOAN_TAT)
        //                    {
        //                        _msgTask = new MessageBoxTask(string.Format("{0}.", eHCMSResources.Z0200_G1_DaLamCLSKgTheHuyDK), eHCMSResources.G0442_G1_TBao);
        //                        yield return _msgTask;
        //                        yield break;
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    //Kiểm tra các dịch vụ có được hủy và trả tiền lại hết chưa

        //    if (CurRegistration.PatientTransaction != null)
        //    {
        //        if (CurRegistration.PatientTransaction.PatientTransactionPayments != null
        //            && CurRegistration.PatientTransaction.PatientTransactionPayments.Count > 0)
        //            foreach (var item in CurRegistration.PatientTransaction.PatientTransactionPayments)
        //            {
        //                if (item.IsDeleted == null
        //                    || item.IsDeleted == false)
        //                {
        //                    _msgTask = new MessageBoxTask(eHCMSResources.Z0201_G1_DKConDVChuaHuyHoacChuaHoanTien
        //                        + "\nHủy và hoàn tiền tất cả các dịch vụ trước khi hủy đăng ký!", eHCMSResources.G0442_G1_TBao);
        //                    yield return _msgTask;
        //                    yield break;
        //                }
        //            }
        //    }
        //    CancelRegistration(CurRegistration);
        //    yield break;

        //}

        private bool RegistrationCancelling { get; set; }

        public void CancelRegistrationInfoCmd()
        {
            if (MessageBox.Show(eHCMSResources.A0611_G1_Msg_ConfHuyDK, "", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                if (RegistrationType == AllLookupValues.RegistrationType.NGOAI_TRU)
                {
                    Coroutine.BeginExecute(DoCancelRegistration(CurrentPatient.latestHIRegistration));
                }
                if (RegistrationType == AllLookupValues.RegistrationType.NOI_TRU)
                {
                    Coroutine.BeginExecute(DoCancelRegistration_InPt(CurrentPatient.LatestRegistration_InPt));
                }
            }
        }

        public void CancelRegistration(PatientRegistration CurRegistration)
        {
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginCancelRegistration(CurRegistration,
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                PatientRegistration registration = null;
                                try
                                {
                                    bool bOK = contract.EndCancelRegistration(out registration, asyncResult);
                                    CurRegistration = registration;
                                    MessageBox.Show(eHCMSResources.A0465_G1_Msg_InfoDaHuy);
                                    CanCreateNewRegistration = true;
                                    CanCancelRegistrationInfoCmd = false;
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
                    ClientLoggerHelper.LogInfo(ex.ToString());
                }
            });
            t.Start();
        }
        #region Quyền lợi Trẻ em TP dưới 6 tuổi có thẻ BHYT

        // Hpt 03/12/2015: Biến này biding vào checkbox xác nhận Trẻ em TP dưới 6 tuổi

        private bool _IsChildUnder6YearsOld = false;
        public bool IsChildUnder6YearsOld
        {
            get { return _IsChildUnder6YearsOld; }
            set
            {
                _IsChildUnder6YearsOld = value;
                NotifyOfPropertyChange(() => IsChildUnder6YearsOld);
                PatientDetailsContent.IsChildUnder6YearsOld = IsChildUnder6YearsOld;
            }
        }


        // Hpt 03/12/2015: Hàm kiểm tra bệnh nhân đăng ký có thỏa điều kiện ở TP + dưới 6 tuổi
        // Anh Tuấn nói không thể chỉ kiểm tra dựa vào mã thẻ vì thẻ BHYT có thể sử dụng lại, đề phòng trường hợp thẻ có dạng TE179 nhưng bệnh nhân đã quá 6 tuổi
        public bool CheckResult_ChildUnder6YearsOld(out string Error)
        {
            Error = "";
            if (PatientDetailsContent == null || PatientDetailsContent.CurrentPatient == null || PatientDetailsContent.HealthInsuranceContent == null)
            {
                return false;
            }
            int YearOfBirth = PatientDetailsContent.CurrentPatient.YOB.GetValueOrDefault();
            if (YearOfBirth <= 0)
            {
                return false;
            }
            if ((Globals.GetCurServerDateTime().Year - YearOfBirth) > 6)
            {
                Error += string.Format("\n - {0}", eHCMSResources.Z0230_G1_LaTreEmDuoi6Tuoi);
            }

            HealthInsurance SelectedHI = PatientDetailsContent.HealthInsuranceContent.ConfirmedItem;
            if (SelectedHI == null)
            {
                return false;
            }
            if (SelectedHI.strProvince != "79")
            {
                Error += string.Format("\n - {0}", eHCMSResources.Z0231_G1_OTPho);
            }

            if (Error != "")
            {
                return false;
            }
            return true;
        }
        #endregion
        private IReceivePatient _ParentOfPatient;
        public IReceivePatient ParentOfPatient
        {
            get
            {
                return _ParentOfPatient;
            }
            set
            {
                _ParentOfPatient = value;
            }
        }

        public bool VisibilityCbxAllowCrossRegion
        {
            get
            {
                return ((RegistrationType == AllLookupValues.RegistrationType.NOI_TRU || RegistrationType == AllLookupValues.RegistrationType.XAC_NHAN_LAI_BHYT) && Globals.ServerConfigSection.HealthInsurances.AllowInPtCrossRegion)
                       || (RegistrationType == AllLookupValues.RegistrationType.NGOAI_TRU && Globals.ServerConfigSection.HealthInsurances.AllowOutPtCrossRegion);
            }
        }

        IScanImageCapture theScanImageCaptureDlg = null;
        private List<ScanImageFileStorageDetail> NewScanImageFilesToBeSave = null;
        private List<ScanImageFileStorageDetail> ScanImageFilesToDeleted = new List<ScanImageFileStorageDetail>();

        private long GetPtRegistrationID_ForScanningStuff()
        {
            long PtRegistrationID = 0;
            DateTime dtToday = Globals.GetCurServerDateTime();
            bool bReload_Of_Registration_DoneToday = CurRegistration == null && CurrentPatient != null && CurrentPatient.LatestRegistration != null && (CurrentPatient.LatestRegistration.ExamDate.Date == dtToday.Date);
            if (bReload_Of_Registration_DoneToday)
            {
                PtRegistrationID = CurrentPatient.LatestRegistration.PtRegistrationID;
            }
            else
            {
                // Registration has just been done
                PtRegistrationID = (CurRegistration != null && (CurRegistration.PtRegistrationID > 0 && CurRegistration.ExamDate.Date == dtToday.Date) ? CurRegistration.PtRegistrationID : 0);
            }

            return PtRegistrationID;
        }

        public void DoScanCmd()
        {
            long PtRegistrationID = GetPtRegistrationID_ForScanningStuff();

            if (PtRegistrationID == 0)
                return;

            theScanImageCaptureDlg = Globals.GetViewModel<IScanImageCapture>();
            Action<IScanImageCapture> onInitDlg = delegate (IScanImageCapture vm)
            {
                vm.PatientID = (CurrentPatient != null ? CurrentPatient.PatientID : 0);
                vm.PtRegistrationID = PtRegistrationID;
            };
            GlobalsNAV.ShowDialog_V3(theScanImageCaptureDlg, onInitDlg, null);

            NewScanImageFilesToBeSave = theScanImageCaptureDlg.ScanImageFileToBeSaved;
            ScanImageFilesToDeleted = theScanImageCaptureDlg.ScanImageFileToBeDeleted;
        }

        public void SaveScanCmd()
        {
            long PtRegistrationID = GetPtRegistrationID_ForScanningStuff();

            if (PtRegistrationID == 0)
                return;

            SaveScanImageFiletoServer(PtRegistrationID);
        }

        private void SaveScanImageFiletoServer(long PtRegistrationID)
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PCLsImportClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    try
                    {
                        contract.BeginAddScanFileStorageDetails(Globals.LoggedUserAccount.StaffID.GetValueOrDefault(), CurrentPatient.PatientID, PtRegistrationID,
                            CurrentPatient.PatientCode, NewScanImageFilesToBeSave, ScanImageFilesToDeleted, Globals.DispatchCallback((asyncResult) =>
                            {
                                try
                                {
                                    bool resOK = contract.EndAddScanFileStorageDetails(asyncResult);
                                    if (resOK)
                                    {
                                        GlobalsNAV.ShowMessagePopup(eHCMSResources.Z1562_G1_DaLuu);
                                        // MessageBox.Show(eHCMSResources.Z1562_G1_DaLuu);
                                        Globals.EventAggregator.Publish(new ReloadOutStandingStaskPCLRequest());
                                    }
                                }
                                catch (Exception ex)
                                {
                                    ClientLoggerHelper.LogError(ex.ToString());
                                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                                }
                                finally
                                {
                                    this.HideBusyIndicator();
                                }
                            }), null);
                    }
                    catch (Exception ex)
                    {
                        ClientLoggerHelper.LogError(ex.ToString());
                        this.HideBusyIndicator();
                    }
                }
            });
            t.Start();

        }

        public override void HandleHotKey_Action_New(object sender, LocalHotKeyEventArgs e)
        {
            foreach (var inputBindingCommand in ListInputBindingCmds)
            {
                if (inputBindingCommand.HotKey_Registered_Name == e.HotKey.Name)
                {
                    inputBindingCommand._executeDelegate.Invoke(this);
                    break;
                }
            }
        }

        protected override IEnumerable<InputBindingCommand> GetInputBindingCommands()
        {
            yield return new InputBindingCommand(DoScanCmd)
            {
                HotKey_Registered_Name = "ghkDoScan",
                GestureModifier = ModifierKeys.Control,
                GestureKey = (Key)Keys.X
            };
            yield return new InputBindingCommand(SaveScanCmd)
            {
                HotKey_Registered_Name = "ghkSaveScan",
                GestureModifier = ModifierKeys.Control,
                GestureKey = (Key)Keys.Y
            };
        }
        public void Handle(SaveHIAndInPtConfirmHICmd message)
        {
            if (message != null)
            {
                InPtConfirmHICmd();
            }
        }
        //▼===== #012
        public bool CheckChildUnderSixAge(Patient CurPatient)
        {
            DateTime loadCurrentDate = Globals.ServerDate.Value;
            int monthnew = 0;
            monthnew = (loadCurrentDate.Month + loadCurrentDate.Year * 12) - (Convert.ToDateTime(CurPatient.DOB).Month + Convert.ToDateTime(CurPatient.DOB).Year * 12);
            if (CurPatient.AgeOnly == true && ((loadCurrentDate.Year - Convert.ToDateTime(CurPatient.DOB).Year) <= 6))
            {
                MessageBox.Show(eHCMSResources.Z2643_G1_KhongDuThongTinTreDuoi6Tuoi);
                return false;
            }
            else if (CurPatient.AgeOnly == false && monthnew <= 72)
            {
                if (CurPatient.FContactFullName == null && (CurPatient.V_FamilyRelationship == null || CurPatient.V_FamilyRelationship == 0))
                {
                    MessageBox.Show(eHCMSResources.Z2644_G1_KhongDuThongTinNguoiThanTreDuoi6Tuoi);
                    return false;
                }
            }
            return true;
        }
        //▲===== #012
        //▼===== #013
        public void Handle(InPatientRegistrationSelectedForConfirmHI message)
        {
            if (message != null && message.Source != null)
            {
                OpenRegistration(message.Source.PtRegistrationID);
            }
        }
        //▲===== #013

        #region Ticket
        private bool IsDiffBetweenRegistrationAndTicket = false;
        private TicketIssue _TicketIssue;
        public TicketIssue TicketIssueObj
        {
            get
            {
                return _TicketIssue;
            }
            set
            {
                _TicketIssue = value;
                NotifyOfPropertyChange(() => TicketIssueObj);
            }
        }
        public void Handle(SetTicketIssueForPatientRegistrationView message)
        {
            TicketIssueObj = new TicketIssue();
            if (message != null && message.Item != null)
            {
                TicketIssueObj = message.Item;
                if (message.IsLoadPatientInfo)
                {
                    //if (!string.IsNullOrEmpty(TicketIssueObj.HICardNo))
                    //{
                    //    SearchPatientByHICardNo(TicketIssueObj.HICardNo);
                    //}
                    //else 
                    if (!string.IsNullOrEmpty(TicketIssueObj.PatientCode))
                    {
                        SearchPatientByPatientCode(TicketIssueObj.PatientCode);
                    }
                }
            }
        }
        public void Handle(SetTicketForNewRegistrationAgain message)
        {
            TicketIssueObj = new TicketIssue();
            if (message != null && message.Item != null)
            {
                TicketIssueObj = message.Item;
                if (!string.IsNullOrEmpty(TicketIssueObj.PatientCode))
                {
                    SearchPatientByPatientCode(TicketIssueObj.PatientCode);
                }
            }
        }
        private void SearchPatientByHICardNo(string aHICardNo)
        {
            this.ShowBusyIndicator(string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.Z1097_G1_DangTimBN));
            var t = new Thread(() =>
            {
                AxErrorEventArgs error = null;
                try
                {
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        PatientSearchCriteria mSearchCriteria = new PatientSearchCriteria { PatientNameString = aHICardNo, InsuranceCard = aHICardNo };
                        client.BeginSearchPatients(mSearchCriteria, 0, 5, false, Globals.DispatchCallback((asyncResult) =>
                        {
                            int totalCount = 0;
                            IList<Patient> allItems = null;
                            bool bOK = false;
                            try
                            {
                                allItems = client.EndSearchPatients(out totalCount, asyncResult);
                                bOK = true;
                            }
                            catch (FaultException<AxException> fault)
                            {
                                error = new AxErrorEventArgs(fault);
                            }
                            catch (Exception ex)
                            {
                                error = new AxErrorEventArgs(ex);
                            }
                            if (bOK)
                            {
                                if (allItems.Count == 1)
                                {
                                    Globals.EventAggregator.Publish(new ResultFound<Patient>() { Result = allItems[0], SearchCriteria = mSearchCriteria });
                                }
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    error = new AxErrorEventArgs(ex);
                }
                finally
                {
                    this.HideBusyIndicator();
                }
                if (error != null)
                {
                    this.HideBusyIndicator();
                    Globals.EventAggregator.Publish(new ErrorOccurred() { CurrentError = error });
                }
            });
            t.Start();
        }

        private void SearchPatientByPatientCode(string aPatientCode)
        {
            this.ShowBusyIndicator(string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.Z1097_G1_DangTimBN));
            var t = new Thread(() =>
            {
                AxErrorEventArgs error = null;
                try
                {
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        PatientSearchCriteria mSearchCriteria = new PatientSearchCriteria { PatientCode = aPatientCode };
                        client.BeginSearchPatients(mSearchCriteria, 0, 5, false, Globals.DispatchCallback((asyncResult) =>
                        {
                            int totalCount = 0;
                            IList<Patient> allItems = null;
                            bool bOK = false;
                            try
                            {
                                allItems = client.EndSearchPatients(out totalCount, asyncResult);
                                bOK = true;
                            }
                            catch (FaultException<AxException> fault)
                            {
                                error = new AxErrorEventArgs(fault);
                            }
                            catch (Exception ex)
                            {
                                error = new AxErrorEventArgs(ex);
                            }
                            if (bOK)
                            {
                                if (allItems.Count == 1)
                                {
                                    Globals.EventAggregator.Publish(new ResultFound<Patient>() { Result = allItems[0], SearchCriteria = mSearchCriteria });
                                }
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    error = new AxErrorEventArgs(ex);
                }
                finally
                {
                    this.HideBusyIndicator();
                }
                if (error != null)
                {
                    this.HideBusyIndicator();
                    Globals.EventAggregator.Publish(new ErrorOccurred() { CurrentError = error });
                }
            });
            t.Start();
        }

        //▼===== 20200430 TTM: Hàm gọi sang QMSService để đi cập nhật lại tình trạng của số thứ tự
        private void UpdateTicketStatusAfterRegister(string TicketNumberText, DateTime TicketGetTime)
        {
            var t = new Thread(() =>
            {
                this.ShowBusyIndicator();
                using (var mFactory = new QMSService.QMSServiceClient())
                {
                    try
                    {
                        var mContract = mFactory.ServiceInstance;
                        mContract.BeginUpdateTicketStatusAfterRegister(TicketNumberText, TicketGetTime, Globals.DeptLocation.DeptLocationID, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                bool IsOK = mContract.EndUpdateTicketStatusAfterRegister(asyncResult);
                                if (IsOK)
                                {
                                    TicketIssueObj.V_TicketStatus = (int)V_TicketStatus_Enum.TKT_ALREADY_REGIS;
                                    IsDiffBetweenRegistrationAndTicket = false;
                                    NotifyOfPropertyChange(() => TicketIssueObj);
                                }
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
                    catch (Exception ex)
                    {
                        Globals.ShowMessage(ex.Message, eHCMSResources.T0074_G1_I);
                        this.HideBusyIndicator();
                    }
                }
            });
            t.Start();
        }

        public int TicketType = (int)V_TicketType_Enum.DV;
        public HealthInsurance CurHealthForQMS = null;
        public void Handle(NotifyAddCurHealthInsuranceToQMS message)
        {
            //Đã được kiểm tra dữ liệu trước khi bắn.
            CurHealthForQMS = message.Item;
        }
        public void GetBarcodeToPrintTicket()
        {
            if (CurrentPatient == null)
            {
                return;
            }
            if (!string.IsNullOrEmpty(CurrentPatient.PatientCode))
            {
                Action<IBarcodeQMS> onInitDlg = delegate (IBarcodeQMS vm)
                {
                    vm.BarcodeText = CurrentPatient.PatientCode;
                };
                GlobalsNAV.ShowDialog<IBarcodeQMS>(onInitDlg);
            }
        }
        public void GetNewTicketUTCmd()
        {
            if (CurrentPatient == null)
            {
                return;
            }
            if (MessageBox.Show(string.Format("Xác nhận cấp số ưu tiên."), eHCMSResources.K1576_G1_CBao, MessageBoxButton.OKCancel, MessageBoxImage.None, MessageBoxResult.Cancel) == MessageBoxResult.Cancel)
            {
                return;
            }
            string HICardNo = "";
            CallGetSeqNumber((int)V_TicketType_Enum.UT, (int)V_IssueBy_Enum.CUS_CARE_SCAN, CurrentPatient.PatientCode, HICardNo, CurrentPatient.FullName, false);
        }
        public void GetNewTicketCmd()
        {
            if (CurrentPatient == null)
            {
                return;
            }
            string HICardNo = "";

            if (CurHealthForQMS != null)
            {
                HICardNo = CurHealthForQMS.HICardNo;
            }
            if (!string.IsNullOrEmpty(HICardNo))
            {
                TicketType = (int)V_TicketType_Enum.BH;
            }
            CallGetSeqNumber(TicketType, (int)V_IssueBy_Enum.CUS_CARE_SCAN, CurrentPatient.PatientCode, HICardNo, CurrentPatient.FullName, false);
        }
        public void GetTicketIssueAgainCmd()
        {
            if (CurrentPatient == null)
            {
                return;
            }
            string HICardNo = "";

            if (CurHealthForQMS != null)
            {
                HICardNo = CurHealthForQMS.HICardNo;
            }
            if (!string.IsNullOrEmpty(HICardNo))
            {
                TicketType = (int)V_TicketType_Enum.BH;
            }
            CallGetSeqNumber(TicketType, (int)V_IssueBy_Enum.CUS_CARE_SCAN, CurrentPatient.PatientCode, HICardNo, CurrentPatient.FullName, true);
        }
        public void CallGetSeqNumber(int aType, int V_IssueBy, string PatientCode, string HICardNo, string PatientName, bool IsGetTicketIssueAgain)
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                using (var mQMSServiceClient = new QMSService.QMSServiceClient())
                {
                    try
                    {
                        string CounterName = "";
                        var mContract = mQMSServiceClient.ServiceInstance;
                        mContract.BeginCallGetSeqNumberByTypeForCusCare(aType.ToString(), PatientCode, HICardNo, PatientName, V_IssueBy, IsGetTicketIssueAgain, Globals.LoggedUserAccount.StaffID.GetValueOrDefault()
                            , Globals.DispatchCallback((asyncResult) =>
                            {
                                try
                                {
                                    TicketIssue mQMSTicket = mContract.EndCallGetSeqNumberByTypeForCusCare(out CounterName, asyncResult);
                                    if (mQMSTicket != null && !string.IsNullOrEmpty(mQMSTicket.TicketNumberText))
                                    {
                                        Print(string.IsNullOrEmpty(CounterName) ? "" : CounterName.ToUpper()
                                            , mQMSTicket.TicketNumberText
                                            , mQMSTicket.RecCreatedDate.ToString("dd/MM/yyyy")
                                            , mQMSTicket.RecCreatedDate.ToString("HH:mm")
                                            , mQMSTicket.PatientCode
                                            , mQMSTicket.HICardNo
                                            , mQMSTicket.SerialTicket
                                            , mQMSTicket.PatientName
                                            , mQMSTicket.PrintTimes);
                                    }
                                    else if (mQMSTicket != null && string.IsNullOrEmpty(mQMSTicket.TicketNumberText) && !IsGetTicketIssueAgain)
                                    {
                                        MessageBox.Show("Không thể cấp số cho bệnh nhân. Bệnh nhân đã được cấp số rồi hoặc cần kiểm tra lại các tình trạng đóng/ mở của tất cả các quầy.");
                                    }
                                    else if (mQMSTicket != null && string.IsNullOrEmpty(mQMSTicket.TicketNumberText) && IsGetTicketIssueAgain)
                                    {
                                        MessageBox.Show("Bệnh nhân đã được cấp quá số lần quy định. Không thể tiếp tục cấp lại số cho bệnh nhân.");
                                    }
                                    else
                                    {
                                        MessageBox.Show("Không thể cấp số cho bệnh nhân.");
                                    }
                                }
                                catch (Exception ex)
                                {
                                    ClientLoggerHelper.LogInfo(" CallGetSeqNumber =====>>>> EX Error: " + ex.ToString());
                                }
                                finally
                                {
                                    this.HideBusyIndicator();
                                }
                            }), null);
                    }
                    catch (Exception ex)
                    {
                        ClientLoggerHelper.LogInfo(" CallGetSeqNumber =====>>>> EX Error: " + ex.ToString());
                    }
                }
            });
            t.Start();
        }
        private bool _IsPriorityTicket = false;
        public bool IsPriorityTicket
        {
            get { return _IsPriorityTicket; }
            set
            {
                if (_IsPriorityTicket != value)
                {
                    _IsPriorityTicket = value;
                    NotifyOfPropertyChange(() => IsPriorityTicket);
                }
            }
        }
        private bool _IsReceivePatientView = true;
        public bool IsReceivePatientView
        {
            get { return _IsReceivePatientView; }
            set
            {
                if (_IsReceivePatientView != value)
                {
                    _IsReceivePatientView = value;
                    NotifyOfPropertyChange(() => IsReceivePatientView);
                }
            }
        }
        private bool _IsShowGetTicketButton = false;
        public bool IsShowGetTicketButton
        {
            get { return _IsShowGetTicketButton; }
            set
            {
                if (_IsShowGetTicketButton != value)
                {
                    _IsShowGetTicketButton = value;
                }
                if (_IsShowGetTicketButton)
                {
                    SearchRegistrationContent.IsShowCallQMS = false;
                    SearchRegistrationContent.IsShowGetTicketButton = IsShowGetTicketButton;
                    mNhanBenh_DangKy = false;
                    mInPt_ConfirmHI_Only = false;
                    IsReceivePatientView = false;
                }
                NotifyOfPropertyChange(() => IsShowGetTicketButton);
            }
        }
        public void chk_GetTicketUT_Click(object source, object sender)
        {
            CheckBox ckbIsChecked = source as CheckBox;
            if (ckbIsChecked == null)
            {
                return;
            }
            if (ckbIsChecked.IsChecked == true)
            {
                IsPriorityTicket = true;
            }
            else
            {
                IsPriorityTicket = false;
            }
        }
        //▲====== 
        private const string strJScript =
            @"
            <html>
            <head>
            <meta http-equiv=""X-UA-Compatible"" content=""IE=edge"" />
            <script type=""text/javascript"">
            !function(t){function e(r){if(n[r])return n[r].exports;var o=n[r]={i:r,l:!1,exports:{}};return t[r].call(o.exports,o,o.exports,e),o.l=!0,o.exports}var n={};return e.m=t,e.c=n,e.i=function(t){return t},e.d=function(t,e,n){Object.defineProperty(t,e,{configurable:!1,enumerable:!0,get:n})},e.n=function(t){var n=t&&t.__esModule?function(){return t[""default""]}:function(){return t};return e.d(n,""a"",n),n},e.o=function(t,e){return Object.prototype.hasOwnProperty.call(t,e)},e.p="""",e(e.s=22)}([function(t,e){""use strict"";function n(t,e){var n,r={};for(n in t)t.hasOwnProperty(n)&&(r[n]=t[n]);for(n in e)e.hasOwnProperty(n)&&""undefined""!=typeof e[n]&&(r[n]=e[n]);return r}Object.defineProperty(e,""__esModule"",{value:!0}),e[""default""]=n},function(t,e,n){""use strict"";function r(t){return t&&t.__esModule?t:{""default"":t}}function o(t,e){if(!(t instanceof e))throw new TypeError(""Cannot call a class as a function"")}function i(t,e){if(!t)throw new ReferenceError(""this hasn't been initialised - super() hasn't been called"");return!e||""object""!=typeof e&&""function""!=typeof e?t:e}function a(t,e){if(""function""!=typeof e&&null!==e)throw new TypeError(""Super expression must either be null or a function, not ""+typeof e);t.prototype=Object.create(e&&e.prototype,{constructor:{value:t,enumerable:!1,writable:!0,configurable:!0}}),e&&(Object.setPrototypeOf?Object.setPrototypeOf(t,e):t.__proto__=e)}Object.defineProperty(e,""__esModule"",{value:!0});var s=n(11),u=r(s),f=function(t){function e(n,r){o(this,e);var a=i(this,t.call(this,n.substring(1),r));a.bytes=[];for(var s=0;s<n.length;++s)a.bytes.push(n.charCodeAt(s));return a.encodings=[740,644,638,176,164,100,224,220,124,608,604,572,436,244,230,484,260,254,650,628,614,764,652,902,868,836,830,892,844,842,752,734,590,304,112,94,416,128,122,672,576,570,464,422,134,496,478,142,910,678,582,768,762,774,880,862,814,896,890,818,914,602,930,328,292,200,158,68,62,424,412,232,218,76,74,554,616,978,556,146,340,212,182,508,268,266,956,940,938,758,782,974,400,310,118,512,506,960,954,502,518,886,966,668,680,692,5379],a}return a(e,t),e.prototype.encode=function(){var t,e=this.bytes,n=e.shift()-105;if(103===n)t=this.nextA(e,1);else if(104===n)t=this.nextB(e,1);else{if(105!==n)throw new c;t=this.nextC(e,1)}return{text:this.text==this.data?this.text.replace(/[^\x20-\x7E]/g,""""):this.text,data:this.getEncoding(n)+t.result+this.getEncoding((t.checksum+n)%103)+this.getEncoding(106)}},e.prototype.getEncoding=function(t){return this.encodings[t]?(this.encodings[t]+1e3).toString(2):""""},e.prototype.valid=function(){return this.data.search(/^[\x00-\x7F\xC8-\xD3]+$/)!==-1},e.prototype.nextA=function(t,e){if(t.length<=0)return{result:"""",checksum:0};var n,r;if(t[0]>=200)r=t[0]-105,t.shift(),99===r?n=this.nextC(t,e+1):100===r?n=this.nextB(t,e+1):98===r?(t[0]=t[0]>95?t[0]-96:t[0],n=this.nextA(t,e+1)):n=this.nextA(t,e+1);else{var o=t[0];r=o<32?o+64:o-32,t.shift(),n=this.nextA(t,e+1)}var i=this.getEncoding(r),a=r*e;return{result:i+n.result,checksum:a+n.checksum}},e.prototype.nextB=function(t,e){if(t.length<=0)return{result:"""",checksum:0};var n,r;t[0]>=200?(r=t[0]-105,t.shift(),99===r?n=this.nextC(t,e+1):101===r?n=this.nextA(t,e+1):98===r?(t[0]=t[0]<32?t[0]+96:t[0],n=this.nextB(t,e+1)):n=this.nextB(t,e+1)):(r=t[0]-32,t.shift(),n=this.nextB(t,e+1));var o=this.getEncoding(r),i=r*e;return{result:o+n.result,checksum:i+n.checksum}},e.prototype.nextC=function(t,e){if(t.length<=0)return{result:"""",checksum:0};var n,r;t[0]>=200?(r=t[0]-105,t.shift(),n=100===r?this.nextB(t,e+1):101===r?this.nextA(t,e+1):this.nextC(t,e+1)):(r=10*(t[0]-48)+t[1]-48,t.shift(),t.shift(),n=this.nextC(t,e+1));var o=this.getEncoding(r),i=r*e;return{result:o+n.result,checksum:i+n.checksum}},e}(u[""default""]),c=function(t){function e(){o(this,e);var n=i(this,t.call(this));return n.name=""InvalidStartCharacterException"",n.message=""The encoding does not start with a start character."",n}return a(e,t),e}(Error);e[""default""]=f},function(t,e){""use strict"";function n(t,e){if(!(t instanceof e))throw new TypeError(""Cannot call a class as a function"")}function r(t,e){if(!t)throw new ReferenceError(""this hasn't been initialised - super() hasn't been called"");return!e||""object""!=typeof e&&""function""!=typeof e?t:e}function o(t,e){if(""function""!=typeof e&&null!==e)throw new TypeError(""Super expression must either be null or a function, not ""+typeof e);t.prototype=Object.create(e&&e.prototype,{constructor:{value:t,enumerable:!1,writable:!0,configurable:!0}}),e&&(Object.setPrototypeOf?Object.setPrototypeOf(t,e):t.__proto__=e)}Object.defineProperty(e,""__esModule"",{value:!0});var i=function(t){function e(o,i){n(this,e);var a=r(this,t.call(this));return a.name=""InvalidInputException"",a.symbology=o,a.input=i,a.message='""'+a.input+'"" is not a valid input for '+a.symbology,a}return o(e,t),e}(Error),a=function(t){function e(){n(this,e);var o=r(this,t.call(this));return o.name=""InvalidElementException"",o.message=""Not supported type to render on"",o}return o(e,t),e}(Error),s=function(t){function e(){n(this,e);var o=r(this,t.call(this));return o.name=""NoElementException"",o.message=""No element to render on."",o}return o(e,t),e}(Error);e.InvalidInputException=i,e.InvalidElementException=a,e.NoElementException=s},function(t,e){""use strict"";function n(t){var e=[""width"",""height"",""textMargin"",""fontSize"",""margin"",""marginTop"",""marginBottom"",""marginLeft"",""marginRight""];for(var n in e)e.hasOwnProperty(n)&&(n=e[n],""string""==typeof t[n]&&(t[n]=parseInt(t[n],10)));return""string""==typeof t.displayValue&&(t.displayValue=""false""!=t.displayValue),t}Object.defineProperty(e,""__esModule"",{value:!0}),e[""default""]=n},function(t,e){""use strict"";Object.defineProperty(e,""__esModule"",{value:!0});var n={width:1,height:25,format:""auto"",displayValue:!0,fontOptions:"""",font:""monospace"",text:void 0,textAlign:""center"",textPosition:""bottom"",textMargin:2,fontSize:20,background:""#ffffff"",lineColor:""#000000"",margin:10,marginTop:void 0,marginBottom:void 0,marginLeft:void 0,marginRight:void 0,valid:function(){}};e[""default""]=n},function(t,e,n){""use strict"";function r(t){return t&&t.__esModule?t:{""default"":t}}function o(t,e){return e.height+(e.displayValue&&t.text.length>0?e.fontSize+e.textMargin:0)+e.marginTop+e.marginBottom}function i(t,e,n){if(n.displayValue&&e<t){if(""center""==n.textAlign)return Math.floor((t-e)/2);if(""left""==n.textAlign)return 0;if(""right""==n.textAlign)return Math.floor(t-e)}return 0}function a(t,e,n){for(var r=0;r<t.length;r++){var a,s=t[r],u=(0,l[""default""])(e,s.options);a=u.displayValue?f(s.text,u,n):0;var c=s.data.length*u.width;s.width=Math.ceil(Math.max(a,c)),s.height=o(s,u),s.barcodePadding=i(a,c,u)}}function s(t){for(var e=0,n=0;n<t.length;n++)e+=t[n].width;return e}function u(t){for(var e=0,n=0;n<t.length;n++)t[n].height>e&&(e=t[n].height);return e}function f(t,e,n){var r;r=""undefined""==typeof n?document.createElement(""canvas"").getContext(""2d""):n,r.font=e.fontOptions+"" ""+e.fontSize+""px ""+e.font;var o=r.measureText(t).width;return o}Object.defineProperty(e,""__esModule"",{value:!0}),e.getTotalWidthOfEncodings=e.calculateEncodingAttributes=e.getBarcodePadding=e.getEncodingHeight=e.getMaximumHeightOfEncodings=void 0;var c=n(0),l=r(c);e.getMaximumHeightOfEncodings=u,e.getEncodingHeight=o,e.getBarcodePadding=i,e.calculateEncodingAttributes=a,e.getTotalWidthOfEncodings=s},function(t,e,n){""use strict"";Object.defineProperty(e,""__esModule"",{value:!0});var r=n(16);e[""default""]={CODE128:r.CODE128,CODE128A:r.CODE128A,CODE128B:r.CODE128B,CODE128C:r.CODE128C}},function(t,e){""use strict"";function n(t,e){if(!(t instanceof e))throw new TypeError(""Cannot call a class as a function"")}Object.defineProperty(e,""__esModule"",{value:!0});var r=function(){function t(e){n(this,t),this.api=e}return t.prototype.handleCatch=function(t){if(""InvalidInputException""!==t.name)throw t;if(this.api._options.valid===this.api._defaults.valid)throw t.message;this.api._options.valid(!1),this.api.render=function(){}},t.prototype.wrapBarcodeCall=function(t){try{var e=t.apply(void 0,arguments);return this.api._options.valid(!0),e}catch(n){return this.handleCatch(n),this.api}},t}();e[""default""]=r},function(t,e){""use strict"";function n(t){return t.marginTop=t.marginTop||t.margin,t.marginBottom=t.marginBottom||t.margin,t.marginRight=t.marginRight||t.margin,t.marginLeft=t.marginLeft||t.margin,t}Object.defineProperty(e,""__esModule"",{value:!0}),e[""default""]=n},function(t,e,n){""use strict"";function r(t){return t&&t.__esModule?t:{""default"":t}}function o(t){if(""string""==typeof t)return i(t);if(Array.isArray(t)){for(var e=[],n=0;n<t.length;n++)e.push(o(t[n]));return e}if(""undefined""!=typeof HTMLCanvasElement&&t instanceof HTMLImageElement)return a(t);if(""undefined""!=typeof SVGElement&&t instanceof SVGElement)return{element:t,options:(0,f[""default""])(t),renderer:l[""default""].SVGRenderer};if(""undefined""!=typeof HTMLCanvasElement&&t instanceof HTMLCanvasElement)return{element:t,options:(0,f[""default""])(t),renderer:l[""default""].CanvasRenderer};if(t&&t.getContext)return{element:t,renderer:l[""default""].CanvasRenderer};if(t&&""object""===(""undefined""==typeof t?""undefined"":s(t))&&!t.nodeName)return{element:t,renderer:l[""default""].ObjectRenderer};throw new d.InvalidElementException}function i(t){var e=document.querySelectorAll(t);if(0!==e.length){for(var n=[],r=0;r<e.length;r++)n.push(o(e[r]));return n}}function a(t){var e=document.createElement(""canvas"");return{element:e,options:(0,f[""default""])(t),renderer:l[""default""].CanvasRenderer,afterRender:function(){t.setAttribute(""src"",e.toDataURL())}}}Object.defineProperty(e,""__esModule"",{value:!0});var s=""function""==typeof Symbol&&""symbol""==typeof Symbol.iterator?function(t){return typeof t}:function(t){return t&&""function""==typeof Symbol&&t.constructor===Symbol?""symbol"":typeof t},u=n(17),f=r(u),c=n(19),l=r(c),d=n(2);e[""default""]=o},function(t,e){""use strict"";function n(t){function e(t){if(Array.isArray(t))for(var r=0;r<t.length;r++)e(t[r]);else t.text=t.text||"""",t.data=t.data||"""",n.push(t)}var n=[];return e(t),n}Object.defineProperty(e,""__esModule"",{value:!0}),e[""default""]=n},function(t,e){""use strict"";function n(t,e){if(!(t instanceof e))throw new TypeError(""Cannot call a class as a function"")}Object.defineProperty(e,""__esModule"",{value:!0});var r=function o(t,e){n(this,o),this.data=t,this.text=e.text||t,this.options=e};e[""default""]=r},function(t,e,n){""use strict"";function r(t){return t&&t.__esModule?t:{""default"":t}}function o(t,e){if(!(t instanceof e))throw new TypeError(""Cannot call a class as a function"")}function i(t,e){if(!t)throw new ReferenceError(""this hasn't been initialised - super() hasn't been called"");return!e||""object""!=typeof e&&""function""!=typeof e?t:e}function a(t,e){if(""function""!=typeof e&&null!==e)throw new TypeError(""Super expression must either be null or a function, not ""+typeof e);t.prototype=Object.create(e&&e.prototype,{constructor:{value:t,enumerable:!1,writable:!0,configurable:!0}}),e&&(Object.setPrototypeOf?Object.setPrototypeOf(t,e):t.__proto__=e)}Object.defineProperty(e,""__esModule"",{value:!0});var s=n(1),u=r(s),f=function(t){function e(n,r){return o(this,e),i(this,t.call(this,String.fromCharCode(208)+n,r))}return a(e,t),e.prototype.valid=function(){return this.data.search(/^[\x00-\x5F\xC8-\xCF]+$/)!==-1},e}(u[""default""]);e[""default""]=f},function(t,e,n){""use strict"";function r(t){return t&&t.__esModule?t:{""default"":t}}function o(t,e){if(!(t instanceof e))throw new TypeError(""Cannot call a class as a function"")}function i(t,e){if(!t)throw new ReferenceError(""this hasn't been initialised - super() hasn't been called"");return!e||""object""!=typeof e&&""function""!=typeof e?t:e}function a(t,e){if(""function""!=typeof e&&null!==e)throw new TypeError(""Super expression must either be null or a function, not ""+typeof e);t.prototype=Object.create(e&&e.prototype,{constructor:{value:t,enumerable:!1,writable:!0,configurable:!0}}),e&&(Object.setPrototypeOf?Object.setPrototypeOf(t,e):t.__proto__=e)}Object.defineProperty(e,""__esModule"",{value:!0});var s=n(1),u=r(s),f=function(t){function e(n,r){return o(this,e),i(this,t.call(this,String.fromCharCode(209)+n,r))}return a(e,t),e.prototype.valid=function(){return this.data.search(/^[\x20-\x7F\xC8-\xCF]+$/)!==-1},e}(u[""default""]);e[""default""]=f},function(t,e,n){""use strict"";function r(t){return t&&t.__esModule?t:{""default"":t}}function o(t,e){if(!(t instanceof e))throw new TypeError(""Cannot call a class as a function"")}function i(t,e){if(!t)throw new ReferenceError(""this hasn't been initialised - super() hasn't been called"");return!e||""object""!=typeof e&&""function""!=typeof e?t:e}function a(t,e){if(""function""!=typeof e&&null!==e)throw new TypeError(""Super expression must either be null or a function, not ""+typeof e);t.prototype=Object.create(e&&e.prototype,{constructor:{value:t,enumerable:!1,writable:!0,configurable:!0}}),e&&(Object.setPrototypeOf?Object.setPrototypeOf(t,e):t.__proto__=e)}Object.defineProperty(e,""__esModule"",{value:!0});var s=n(1),u=r(s),f=function(t){function e(n,r){return o(this,e),i(this,t.call(this,String.fromCharCode(210)+n,r))}return a(e,t),e.prototype.valid=function(){return this.data.search(/^(\xCF*[0-9]{2}\xCF*)+$/)!==-1},e}(u[""default""]);e[""default""]=f},function(t,e,n){""use strict"";function r(t){return t&&t.__esModule?t:{""default"":t}}function o(t,e){if(!(t instanceof e))throw new TypeError(""Cannot call a class as a function"")}function i(t,e){if(!t)throw new ReferenceError(""this hasn't been initialised - super() hasn't been called"");return!e||""object""!=typeof e&&""function""!=typeof e?t:e}function a(t,e){if(""function""!=typeof e&&null!==e)throw new TypeError(""Super expression must either be null or a function, not ""+typeof e);t.prototype=Object.create(e&&e.prototype,{constructor:{value:t,enumerable:!1,writable:!0,configurable:!0}}),e&&(Object.setPrototypeOf?Object.setPrototypeOf(t,e):t.__proto__=e)}function s(t){var e,n=t.match(/^[\x00-\x5F\xC8-\xCF]*/)[0].length,r=t.match(/^[\x20-\x7F\xC8-\xCF]*/)[0].length,o=t.match(/^(\xCF*[0-9]{2}\xCF*)*/)[0].length;return e=o>=2?String.fromCharCode(210)+c(t):n>r?String.fromCharCode(208)+u(t):String.fromCharCode(209)+f(t),e=e.replace(/[\xCD\xCE]([^])[\xCD\xCE]/,function(t,e){return String.fromCharCode(203)+e})}function u(t){var e=t.match(/^([\x00-\x5F\xC8-\xCF]+?)(([0-9]{2}){2,})([^0-9]|$)/);if(e)return e[1]+String.fromCharCode(204)+c(t.substring(e[1].length));var n=t.match(/^[\x00-\x5F\xC8-\xCF]+/);return n[0].length===t.length?t:n[0]+String.fromCharCode(205)+f(t.substring(n[0].length))}function f(t){var e=t.match(/^([\x20-\x7F\xC8-\xCF]+?)(([0-9]{2}){2,})([^0-9]|$)/);if(e)return e[1]+String.fromCharCode(204)+c(t.substring(e[1].length));var n=t.match(/^[\x20-\x7F\xC8-\xCF]+/);return n[0].length===t.length?t:n[0]+String.fromCharCode(206)+u(t.substring(n[0].length))}function c(t){var e=t.match(/^(\xCF*[0-9]{2}\xCF*)+/)[0],n=e.length;if(n===t.length)return t;t=t.substring(n);var r=t.match(/^[\x00-\x5F\xC8-\xCF]*/)[0].length,o=t.match(/^[\x20-\x7F\xC8-\xCF]*/)[0].length;return r>=o?e+String.fromCharCode(206)+u(t):e+String.fromCharCode(205)+f(t)}Object.defineProperty(e,""__esModule"",{value:!0});var l=n(1),d=r(l),h=function(t){function e(n,r){if(o(this,e),n.search(/^[\x00-\x7F\xC8-\xD3]+$/)!==-1)var a=i(this,t.call(this,s(n),r));else var a=i(this,t.call(this,n,r));return i(a)}return a(e,t),e}(d[""default""]);e[""default""]=h},function(t,e,n){""use strict"";function r(t){return t&&t.__esModule?t:{""default"":t}}Object.defineProperty(e,""__esModule"",{value:!0}),e.CODE128C=e.CODE128B=e.CODE128A=e.CODE128=void 0;var o=n(15),i=r(o),a=n(12),s=r(a),u=n(13),f=r(u),c=n(14),l=r(c);e.CODE128=i[""default""],e.CODE128A=s[""default""],e.CODE128B=f[""default""],e.CODE128C=l[""default""]},function(t,e,n){""use strict"";function r(t){return t&&t.__esModule?t:{""default"":t}}function o(t){var e={};for(var n in u[""default""])u[""default""].hasOwnProperty(n)&&(t.hasAttribute(""jsbarcode-""+n.toLowerCase())&&(e[n]=t.getAttribute(""jsbarcode-""+n.toLowerCase())),t.hasAttribute(""data-""+n.toLowerCase())&&(e[n]=t.getAttribute(""data-""+n.toLowerCase())));return e.value=t.getAttribute(""jsbarcode-value"")||t.getAttribute(""data-value""),e=(0,a[""default""])(e)}Object.defineProperty(e,""__esModule"",{value:!0});var i=n(3),a=r(i),s=n(4),u=r(s);e[""default""]=o},function(t,e,n){""use strict"";function r(t){return t&&t.__esModule?t:{""default"":t}}function o(t,e){if(!(t instanceof e))throw new TypeError(""Cannot call a class as a function"")}Object.defineProperty(e,""__esModule"",{value:!0});var i=n(0),a=r(i),s=n(5),u=function(){function t(e,n,r){o(this,t),this.canvas=e,this.encodings=n,this.options=r}return t.prototype.render=function(){if(!this.canvas.getContext)throw new Error(""The browser does not support canvas."");this.prepareCanvas();for(var t=0;t<this.encodings.length;t++){var e=(0,a[""default""])(this.options,this.encodings[t].options);this.drawCanvasBarcode(e,this.encodings[t]),this.drawCanvasText(e,this.encodings[t]),this.moveCanvasDrawing(this.encodings[t])}this.restoreCanvas()},t.prototype.prepareCanvas=function(){var t=this.canvas.getContext(""2d"");t.save(),(0,s.calculateEncodingAttributes)(this.encodings,this.options,t);var e=(0,s.getTotalWidthOfEncodings)(this.encodings),n=(0,s.getMaximumHeightOfEncodings)(this.encodings);this.canvas.width=e+this.options.marginLeft+this.options.marginRight,this.canvas.height=n,t.clearRect(0,0,this.canvas.width,this.canvas.height),this.options.background&&(t.fillStyle=this.options.background,t.fillRect(0,0,this.canvas.width,this.canvas.height)),t.translate(this.options.marginLeft,0)},t.prototype.drawCanvasBarcode=function(t,e){var n,r=this.canvas.getContext(""2d""),o=e.data;n=""top""==t.textPosition?t.marginTop+t.fontSize+t.textMargin:t.marginTop,r.fillStyle=t.lineColor;for(var i=0;i<o.length;i++){var a=i*t.width+e.barcodePadding;""1""===o[i]?r.fillRect(a,n,t.width,t.height):o[i]&&r.fillRect(a,n,t.width,t.height*o[i])}},t.prototype.drawCanvasText=function(t,e){var n=this.canvas.getContext(""2d""),r=t.fontOptions+"" ""+t.fontSize+""px ""+t.font;if(t.displayValue){var o,i;i=""top""==t.textPosition?t.marginTop+t.fontSize-t.textMargin:t.height+t.textMargin+t.marginTop+t.fontSize,n.font=r,""left""==t.textAlign||e.barcodePadding>0?(o=0,n.textAlign=""left""):""right""==t.textAlign?(o=e.width-1,n.textAlign=""right""):(o=e.width/2,n.textAlign=""center""),n.fillText(e.text,o,i)}},t.prototype.moveCanvasDrawing=function(t){var e=this.canvas.getContext(""2d"");e.translate(t.width,0)},t.prototype.restoreCanvas=function(){var t=this.canvas.getContext(""2d"");t.restore()},t}();e[""default""]=u},function(t,e,n){""use strict"";function r(t){return t&&t.__esModule?t:{""default"":t}}Object.defineProperty(e,""__esModule"",{value:!0});var o=n(18),i=r(o),a=n(21),s=r(a),u=n(20),f=r(u);e[""default""]={CanvasRenderer:i[""default""],SVGRenderer:s[""default""],ObjectRenderer:f[""default""]}},function(t,e){""use strict"";function n(t,e){if(!(t instanceof e))throw new TypeError(""Cannot call a class as a function"")}Object.defineProperty(e,""__esModule"",{value:!0});var r=function(){function t(e,r,o){n(this,t),this.object=e,this.encodings=r,this.options=o}return t.prototype.render=function(){this.object.encodings=this.encodings},t}();e[""default""]=r},function(t,e,n){""use strict"";function r(t){return t&&t.__esModule?t:{""default"":t}}function o(t,e){if(!(t instanceof e))throw new TypeError(""Cannot call a class as a function"")}function i(t,e,n){var r=document.createElementNS(l,""g"");return r.setAttribute(""transform"",""translate(""+t+"", ""+e+"")""),n.appendChild(r),r}function a(t,e){t.setAttribute(""style"",""fill:""+e.lineColor+"";"")}function s(t,e,n,r,o){var i=document.createElementNS(l,""rect"");return i.setAttribute(""x"",t),i.setAttribute(""y"",e),i.setAttribute(""width"",n),i.setAttribute(""height"",r),o.appendChild(i),i}Object.defineProperty(e,""__esModule"",{value:!0});var u=n(0),f=r(u),c=n(5),l=""http://www.w3.org/2000/svg"",d=function(){function t(e,n,r){o(this,t),this.svg=e,this.encodings=n,this.options=r}return t.prototype.render=function(){var t=this.options.marginLeft;this.prepareSVG();for(var e=0;e<this.encodings.length;e++){var n=this.encodings[e],r=(0,f[""default""])(this.options,n.options),o=i(t,r.marginTop,this.svg);a(o,r),this.drawSvgBarcode(o,r,n),this.drawSVGText(o,r,n),t+=n.width}},t.prototype.prepareSVG=function(){for(;this.svg.firstChild;)this.svg.removeChild(this.svg.firstChild);(0,c.calculateEncodingAttributes)(this.encodings,this.options);var t=(0,c.getTotalWidthOfEncodings)(this.encodings),e=(0,c.getMaximumHeightOfEncodings)(this.encodings),n=t+this.options.marginLeft+this.options.marginRight;this.setSvgAttributes(n,e),this.options.background&&s(0,0,n,e,this.svg).setAttribute(""style"",""fill:""+this.options.background+"";"")},t.prototype.drawSvgBarcode=function(t,e,n){var r,o=n.data;r=""top""==e.textPosition?e.fontSize+e.textMargin:0;for(var i=0,a=0,u=0;u<o.length;u++)a=u*e.width+n.barcodePadding,""1""===o[u]?i++:i>0&&(s(a-e.width*i,r,e.width*i,e.height,t),i=0);i>0&&s(a-e.width*(i-1),r,e.width*i,e.height,t)},t.prototype.drawSVGText=function(t,e,n){var r=document.createElementNS(l,""text"");if(e.displayValue){var o,i;r.setAttribute(""style"",""font:""+e.fontOptions+"" ""+e.fontSize+""px ""+e.font),i=""top""==e.textPosition?e.fontSize-e.textMargin:e.height+e.textMargin+e.fontSize,""left""==e.textAlign||n.barcodePadding>0?(o=0,r.setAttribute(""text-anchor"",""start"")):""right""==e.textAlign?(o=n.width-1,r.setAttribute(""text-anchor"",""end"")):(o=n.width/2,r.setAttribute(""text-anchor"",""middle"")),r.setAttribute(""x"",o),r.setAttribute(""y"",i),r.appendChild(document.createTextNode(n.text)),t.appendChild(r)}},t.prototype.setSvgAttributes=function(t,e){var n=this.svg;n.setAttribute(""width"",t+""px""),n.setAttribute(""height"",e+""px""),n.setAttribute(""x"",""0px""),n.setAttribute(""y"",""0px""),n.setAttribute(""viewBox"",""0 0 ""+t+"" ""+e),n.setAttribute(""xmlns"",l),n.setAttribute(""version"",""1.1""),n.style.transform=""translate(0,0)""},t}();e[""default""]=d},function(t,e,n){""use strict"";function r(t){return t&&t.__esModule?t:{""default"":t}}function o(t,e){O.prototype[e]=O.prototype[e.toUpperCase()]=O.prototype[e.toLowerCase()]=function(n,r){var o=this;return o._errorHandler.wrapBarcodeCall(function(){r.text=""undefined""==typeof r.text?void 0:""""+r.text;var a=(0,l[""default""])(o._options,r);a=(0,m[""default""])(a);var s=t[e],u=i(n,s,a);return o._encodings.push(u),o})}}function i(t,e,n){t=""""+t;var r=new e(t,n);if(!r.valid())throw new w.InvalidInputException(r.constructor.name,t);var o=r.encode();o=(0,h[""default""])(o);for(var i=0;i<o.length;i++)o[i].options=(0,l[""default""])(n,o[i].options);return o}function a(){return f[""default""].CODE128?""CODE128"":Object.keys(f[""default""])[0]}function s(t,e,n){e=(0,h[""default""])(e);for(var r=0;r<e.length;r++)e[r].options=(0,l[""default""])(n,e[r].options),(0,g[""default""])(e[r].options);(0,g[""default""])(n);var o=t.renderer,i=new o(t.element,e,n);i.render(),t.afterRender&&t.afterRender()}var u=n(6),f=r(u),c=n(0),l=r(c),d=n(10),h=r(d),p=n(8),g=r(p),v=n(9),y=r(v),x=n(3),m=r(x),b=n(7),C=r(b),w=n(2),_=n(4),E=r(_),O=function(){},A=function(t,e,n){var r=new O;if(""undefined""==typeof t)throw Error(""No element to render on was provided."");return r._renderProperties=(0,y[""default""])(t),r._encodings=[],r._options=E[""default""],r._errorHandler=new C[""default""](r),""undefined""!=typeof e&&(n=n||{},n.format||(n.format=a()),r.options(n)[n.format](e,n).render()),r};A.getModule=function(t){return f[""default""][t]};for(var P in f[""default""])f[""default""].hasOwnProperty(P)&&o(f[""default""],P);O.prototype.options=function(t){return this._options=(0,l[""default""])(this._options,t),this},O.prototype.blank=function(t){var e=""0"".repeat(t);return this._encodings.push({data:e}),this},O.prototype.init=function(){if(this._renderProperties){Array.isArray(this._renderProperties)||(this._renderProperties=[this._renderProperties]);var t;for(var e in this._renderProperties){t=this._renderProperties[e];var n=(0,l[""default""])(this._options,t.options);""auto""==n.format&&(n.format=a()),this._errorHandler.wrapBarcodeCall(function(){var e=n.value,r=f[""default""][n.format.toUpperCase()],o=i(e,r,n);s(t,o,n)})}}},O.prototype.render=function(){if(!this._renderProperties)throw new w.NoElementException;if(Array.isArray(this._renderProperties))for(var t=0;t<this._renderProperties.length;t++)s(this._renderProperties[t],this._encodings,this._options);else s(this._renderProperties,this._encodings,this._options);return this},O.prototype._defaults=E[""default""],""undefined""!=typeof window&&(window.JsBarcode=A),""undefined""!=typeof jQuery&&(jQuery.fn.JsBarcode=function(t,e){var n=[];return jQuery(this).each(function(){n.push(this)}),A(n,t,e)}),t.exports=A}]);            
            </script>
            </head>
            ";
        private void Print(string CounterName, string SeqNumber, string SeqDate, string SeqTime, string PatientCode, string HICardNo, string SerialTicket, string PatientName, int PrintTimes)
        {
            try
            {
                System.Windows.Forms.WebBrowser gMainWB = new System.Windows.Forms.WebBrowser();
                gMainWB.DocumentCompleted += (s, e) =>
                {
                    gMainWB.Print();
                    (s as System.Windows.Forms.WebBrowser).Dispose();
                };
                string strPatientName = "";
                string qmsSerialTicket = "";
                strPatientName = string.Format("<div> {0} </div>", PatientName);
                qmsSerialTicket = string.Concat("qms", SerialTicket);
                string strBarCodeUnique = string.Format("<div> <svg id =\"barcodeUQI\" ></svg> <script> JsBarcode(\"#barcodeUQI\", \"{0}\",[width:1, height:30]); </script> </div>", qmsSerialTicket);
                strBarCodeUnique = strBarCodeUnique.Replace("[", "{");
                strBarCodeUnique = strBarCodeUnique.Replace("]", "}");
                gMainWB.DocumentText = string.Format("{0}  <div style=\"width:80mm;text-align:center;font-size:12pt;margin:0;padding:0;\"> VIỆN TIM TP.HỒ CHÍ MINH<br/><div style=\"font-weight:bold;\"><font style=\"font-size:24pt;\">{1}</font><br/><font style=\"font-size:24pt;\">{2}</font> {6} {7}</div><div style=\"text-align:justify;\">Xin quý khách vui lòng chờ đến lượt theo số thứ tự trên màn hình thông tin trung tâm.<br/>Cảm ơn! </div> {5} <p><i> In lần thứ: {8}<i/></p>Ngày: {3}&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Giờ: {4}</div> </html>", strJScript, CounterName, SeqNumber, SeqDate, SeqTime, strBarCodeUnique, strPatientName, PatientCode, PrintTimes);
            }
            catch (Exception ex)
            {
                ClientLoggerHelper.LogInfo(" Print =====>>>> EX Error: " + ex.ToString());
            }
        }
        #endregion
        public void DeletedTicketCmd()
        {
            if (CurrentPatient == null)
            {
                return;
            }
            if (MessageBox.Show(string.Format(eHCMSResources.Z3053_G1_BanCoMuonHuySoThuTuCuaBenhNhanKhong), eHCMSResources.K1576_G1_CBao, MessageBoxButton.OKCancel, MessageBoxImage.None, MessageBoxResult.Cancel) == MessageBoxResult.Cancel)
            {
                return;
            }
            DeletedTicket(CurrentPatient.PatientCode);
        }
        public void DeletedTicket(string PatientCode)
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                using (var mQMSServiceClient = new QMSService.QMSServiceClient())
                {
                    try
                    {
                        var mContract = mQMSServiceClient.ServiceInstance;
                        mContract.BeginDeletedTicketForCusCare(PatientCode, Globals.LoggedUserAccount.StaffID.GetValueOrDefault()
                            , Globals.DispatchCallback((asyncResult) =>
                            {
                                try
                                {
                                    string TicketNumberText = "";
                                    bool bOK = mContract.EndDeletedTicketForCusCare(out TicketNumberText, asyncResult);
                                    if (bOK)
                                    {
                                        MessageBox.Show(eHCMSResources.Z3054_G1_HuySTTThanhCong);
                                    }
                                    else if (TicketNumberText == "")
                                    {
                                        MessageBox.Show(eHCMSResources.Z3055_G1_HuySTTKhongThanhCong);
                                    }
                                    else
                                    {
                                        MessageBox.Show(string.Format(eHCMSResources.Z3060_G1_SoDaDangKyKhongHuyDuoc, TicketNumberText));
                                    }
                                }
                                catch (Exception ex)
                                {
                                    ClientLoggerHelper.LogInfo(" DeletedTicket =====>>>> EX Error: " + ex.ToString());
                                }
                                finally
                                {
                                    this.HideBusyIndicator();
                                }
                            }), null);
                    }
                    catch (Exception ex)
                    {
                        ClientLoggerHelper.LogInfo(" DeletedTicket =====>>>> EX Error: " + ex.ToString());
                    }
                }
            });
            t.Start();
        }
        private bool _ApplyCheckInPtRegistration = Globals.ServerConfigSection.CommonItems.ApplyCheckInPtRegistration;
        public bool ApplyCheckInPtRegistration
        {
            get
            {
                return _ApplyCheckInPtRegistration;
            }
            set
            {
                if (_ApplyCheckInPtRegistration != value)
                {
                    _ApplyCheckInPtRegistration = value;
                    NotifyOfPropertyChange(() => ApplyCheckInPtRegistration);
                }
            }
        }
        private void UpdateInPtRegistrationID_PtRegistration(long PtRegistrationID)
        {
            this.ShowBusyIndicator();
            //IsWaitingGetBlankDiagnosisTreatmentByPtID = true;

            var t = new Thread(() =>
            {
                using (var serviceFactory = new ePMRsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginUpdateInPtRegistrationID_PtRegistration(PtRegistrationID, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            contract.EndUpdateInPtRegistrationID_PtRegistration(asyncResult);
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

        private ObservableCollection<InPatientBillingInvoice> _BillingInvoices;
        public ObservableCollection<InPatientBillingInvoice> BillingInvoices
        {
            get
            {
                return _BillingInvoices;
            }
            set
            {
                if (_BillingInvoices != value)
                {
                    _BillingInvoices = value;
                    NotifyOfPropertyChange(() => BillingInvoices);
                }
            }
        }
        private int _RegLockFlag = 0;
        public int RegLockFlag
        {
            get
            {
                return _RegLockFlag;
            }
            set
            {
                _RegLockFlag = value;
                NotifyOfPropertyChange(() => RegLockFlag);
            }
        }
        private readonly bool ReCalBillingInv = true;
        private bool _ReplaceMaxHIPay;

        public bool ReplaceMaxHIPay
        {
            get { return _ReplaceMaxHIPay; }
            set
            {
                _ReplaceMaxHIPay = value;
                NotifyOfPropertyChange(() => ReplaceMaxHIPay);
            }
        }
        public void GetAllInPatientBillingInvoices()
        {
            //if (!UsedByTaiVuOffice && (Globals.isAccountCheck && DeptID.GetValueOrDefault() <= 0))
            //{
            //    return;
            //}

            var t = new Thread(() =>
            {
                AxErrorEventArgs error = null;
                try
                {
                    using (var serviceFactory = new BillingPaymentWcfServiceLibClient())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginGetAllInPatientBillingInvoices(CurRegistration.PtRegistrationID, 0, (long)AllLookupValues.RegistrationType.NOI_TRU,
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                try
                                {
                                    var inv = contract.EndGetAllInPatientBillingInvoices(asyncResult);
                                    if (BillingInvoices == null)
                                    {
                                        BillingInvoices = new ObservableCollection<InPatientBillingInvoice>();
                                    }
                                    BillingInvoices.Clear();
                                    if (inv != null && inv.Count > 0)
                                    {
                                        foreach (InPatientBillingInvoice item in inv)
                                        {
                                            BillingInvoices.Add(item);
                                        }
                                        RecalcAllHiCmd();
                                    }
                                }
                                catch (FaultException<AxException> fault)
                                {
                                    error = new AxErrorEventArgs(fault);
                                }
                                catch (Exception ex)
                                {
                                    error = new AxErrorEventArgs(ex);
                                }
                                finally
                                {
                                    Globals.IsBusy = false;
                                }


                            }), null);
                    }
                }
                catch (Exception ex)
                {
                    error = new AxErrorEventArgs(ex);
                }
                finally
                {
                    if (error != null)
                    {
                        Globals.EventAggregator.Publish(new ErrorOccurred() { CurrentError = error });
                    }
                }

            });
            t.Start();
        }
        public void RecalcAllHiCmd()
        {
            if (BillingInvoices != null && BillingInvoices.Count > 0)
            {
                //IsLoading = true;
            }
            foreach (var item in BillingInvoices)
            {
                if (Globals.IsLockRegistration(RegLockFlag, eHCMSResources.G1293_G1_TinhLaiBills.ToLower()))
                {
                    return;
                }
                if (item.BillingInvIsFinalized && (CurRegistration == null || CurRegistration.V_RegistrationType != AllLookupValues.RegistrationType.NGOAI_TRU))
                {
                    MessageBox.Show(string.Format("{0}.", eHCMSResources.A0257_G1_Msg_InfoKhTheTinhLaiBillDaQToan));
                    return;
                }
                if (item.BillingInvIsFinalized && (CurRegistration == null || (CurRegistration.V_RegistrationType == AllLookupValues.RegistrationType.NGOAI_TRU && CurRegistration.PtInsuranceBenefit != item.HIBenefit)))
                {
                    MessageBox.Show(string.Format("{0}.", eHCMSResources.A0257_G1_Msg_InfoKhTheTinhLaiBillDaQToan));
                    return;
                }
                bool IsRecalcAllHiDone;
                if (item.Equals(BillingInvoices.LastOrDefault()))
                {
                    IsRecalcAllHiDone = true;
                }
                else
                {
                    IsRecalcAllHiDone = false;
                }
                GetBillingInvoiceDetailsForRecal(item, false, false, false, IsRecalcAllHiDone);
            }
        }
        public void GetBillingInvoiceDetailsForRecal(InPatientBillingInvoice inv, bool WithPriceList = true, bool IsRecalSecondTime = false
           , bool IsPassCheckNonBlockValidPCLExamDate = false, bool IsRecalcAllHiDone = true)
        {
            var t = new Thread(() =>
            {
                this.ShowBusyIndicator();
                AxErrorEventArgs error = null;
                try
                {
                    using (var serviceFactory = new BillingPaymentWcfServiceLibClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetInPatientBillingInvoiceDetails(inv.InPatientBillingInvID, false, IsRecalSecondTime, IsPassCheckNonBlockValidPCLExamDate,
                            Globals.DispatchCallback(asyncResult =>
                            {
                                try
                                {
                                    contract.EndGetInPatientBillingInvoiceDetails(out List<PatientRegistrationDetail> regDetails
                                        , out List<PatientPCLRequest> pclRequestList
                                        , out List<OutwardDrugClinicDeptInvoice> allInPatientInvoices, asyncResult);
                                    if (asyncResult.AsyncState is InPatientBillingInvoice tempInv)
                                    {
                                        tempInv.RegistrationDetails = regDetails?.ToObservableCollection();
                                        tempInv.PclRequests = pclRequestList?.ToObservableCollection();
                                        tempInv.OutwardDrugClinicDeptInvoices = allInPatientInvoices?.ToObservableCollection();

                                        //if (WithPriceList)
                                        //{
                                        //    RecalcHiWithPriceListAfterLoadBill(tempInv, IsPassCheckNonBlockValidPCLExamDate);
                                        //}
                                        //else 
                                        RecalcHiAfterLoadBill(tempInv, IsRecalSecondTime, IsPassCheckNonBlockValidPCLExamDate, IsRecalcAllHiDone);
                                    }
                                }
                                //catch (FaultException<AxException> fault)
                                //{
                                //    error = new AxErrorEventArgs(fault);
                                //}
                                catch (Exception ex)
                                {
                                    //IsLoading = false;
                                    error = new AxErrorEventArgs(ex);
                                    if (!IsRecalSecondTime && ex.Message.Contains("19090601"))
                                    {
                                        if (MessageBox.Show(ex.Message, eHCMSResources.G0442_G1_TBao, MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.Yes)
                                        {
                                            GetBillingInvoiceDetailsForRecal(inv, WithPriceList, true, true);
                                        }
                                        else
                                        {
                                            GetBillingInvoiceDetailsForRecal(inv, WithPriceList, true, false);
                                        }
                                    }
                                    else
                                    {
                                        Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                                    }
                                }
                                finally
                                {
                                    this.HideBusyIndicator();
                                }
                            }), inv);
                    }
                }
                catch (Exception ex)
                {
                    error = new AxErrorEventArgs(ex);
                    this.HideBusyIndicator();
                }
                if (error != null)
                {
                    Globals.EventAggregator.Publish(new ErrorOccurred { CurrentError = error });
                }
            });

            t.Start();
        }
        private void RecalcHiAfterLoadBill(InPatientBillingInvoice billInv, bool IsNotCheckInvalid, bool IsPassCheckNonBlockValidPCLExamDate, bool IsRecalcAllHiDone = true)
        {
            //▼===== #004
            //if (billInv.IsValidForBill > 1)
            //{
            //    if (billInv == null || ((billInv.RegistrationDetails == null && billInv.PclRequests == null) ||
            //    ((billInv.RegistrationDetails != null && billInv.RegistrationDetails.Count() > 0) || (billInv.PclRequests != null && billInv.PclRequests.Count() > 0))))
            //    {
            //        MessageBox.Show(eHCMSResources.Z2900_G1_DkyDoiDoiTuongTinhLaiBill);
            //        return;
            //    }
            //}
            //▲===== #004
            bool IsUsePriceByNewCer = false;
            if (CurRegistration != null && CurRegistration.AdmissionInfo != null
                && (CurRegistration.AdmissionInfo.MedServiceItemPriceListID > 0 || CurRegistration.AdmissionInfo.PCLExamTypePriceListID > 0))
            {
                if (MessageBox.Show(string.Format("{0}?", eHCMSResources.Z2791_G1_SuDungBangGiaMoi), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    IsUsePriceByNewCer = true;
                }
            }
            //▼===== #003
            //▼====: #006
            bool IsAutoCheckCountHI = true;
            //▲====: #006
            //▼====: #005
            //if (AutoCheckCountHI(billInv) && billInv.TotalHIPayment == 0)
            //{
            //    if(MessageBox.Show(string.Format("{0}", eHCMSResources.Z2975_G1_HoiTuDongCheck), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            //    {
            //        IsAutoCheckCountHI = true;
            //    }
            //}
            //▲===== #003
            //▲====: #005
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                AxErrorEventArgs error = null;
                try
                {
                    using (var serviceFactory = new BillingPaymentWcfServiceLibClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginRecalcInPatientBillingInvoice(Globals.LoggedUserAccount.StaffID, billInv, IsUsePriceByNewCer, IsAutoCheckCountHI, ReplaceMaxHIPay, ReCalBillingInv
                            , IsNotCheckInvalid
                            , IsPassCheckNonBlockValidPCLExamDate
                            , Globals.DispatchCallback(asyncResult =>
                            {
                                try
                                {
                                    contract.EndRecalcInPatientBillingInvoice(asyncResult);
                                }
                                //catch (FaultException<AxException> fault)
                                //{
                                //    error = new AxErrorEventArgs(fault);
                                //}
                                catch (Exception ex)
                                {
                                    //IsLoading = false;
                                    error = new AxErrorEventArgs(ex);
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
                    error = new AxErrorEventArgs(ex);
                    this.HideBusyIndicator();
                }
                if (error != null)
                {
                    Globals.EventAggregator.Publish(new ErrorOccurred { CurrentError = error });
                }
            });

            t.Start();
        }
        //▼===== #017
        public void UpdateIsConfirmedEmergencyPatient()
        {
            if(PatientDetailsContent == null || CurRegistration == null || CurRegistration.AdmissionInfo == null)
            {
                return;
            }
            if(PatientDetailsContent.IsConfirmedEmergencyPatient == CurRegistration.AdmissionInfo.IsConfirmEmergencyTreatment)
            {
                OpenRegistration(CurRegistration.PtRegistrationID);
                return;
            }
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PatientRegistrationServiceClient())
                {
                    try
                    {
                        var mContract = serviceFactory.ServiceInstance;
                        mContract.BeginUpdateIsConfirmedEmergencyPatient(CurRegistration.PtRegistrationID, PatientDetailsContent.IsConfirmedEmergencyPatient
                            , Globals.DispatchCallback((asyncResult) =>
                            {
                                try
                                {
                                    bool bOK = mContract.EndUpdateIsConfirmedEmergencyPatient(asyncResult);
                                    if (!bOK)
                                    {
                                        MessageBox.Show(eHCMSResources.T0432_G1_Error);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    ClientLoggerHelper.LogInfo("Update IsConfirmedEmergencyPatient Error " + ex.ToString());
                                }
                                finally
                                {
                                    OpenRegistration(CurRegistration.PtRegistrationID);
                                    this.HideBusyIndicator();
                                }
                            }), null);
                    }
                    catch (Exception ex)
                    {
                        ClientLoggerHelper.LogInfo("Update IsConfirmedEmergencyPatient Error " + ex.ToString());
                    }
                }
            });
            t.Start();
        }
        //▲===== #017
    }

    public enum Status
    {
        None = 1,
        CuocHen_DungHen = 2,
        CuocHen_TraiHen = 3
    }
}