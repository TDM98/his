using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Threading;
using System.Windows;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using Caliburn.Micro;
using DataEntities;
using aEMR.CommonTasks;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using eHCMSLanguage;
using aEMR.Common.BaseModel;
using System.IO;
using System.Collections.ObjectModel;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using aEMR.Controls;
using System.Linq;
using System.Windows.Input;
using aEMR.Common;
using PCLsProxy;
using aEMR.Common.HotKeyManagement;
using aEMR.Common.Printing;
/*
* 20180615 #001 TBLD:   Luu 2 tab Hinh Anh va Hinh Capture
* 20182608 #002 
* 20181001 #003 TBL:    BM 0000106. Fix PatientInfo không hiển thị thông tin 
* 20181019 TxD:         Removed some commented out Code causing the remove of above 2 Comments #002 & #003 
*                       They were about CreateSubVM and ActivateSubVm
* 20181207 #004 TBL:    BM 0005378. Xoa ket qua CLS
* 20181207 #005 TTM:    BM 0005339: Thêm trường BS thực hiện và dời Mã máy ra mà hình Template
* 20190611 #006 TTM:    BM 0010772: Capture là lưu ảnh tại máy trạm (có cấu hình)
* 20210115 #007 TNHX:   Gửi kết quả CLS cho PAC dựa vào cấu hình AutoCreatePACWorklist
* 20210701 #008 TNHX:   260 thêm user bsi mượn vào màn hình nhập kết quả
* 20210818 #009 BLQ:  Truyền thêm biến IsReadOnly để set cho nút lưu và xóa
* 20210908 #010 BLQ:  Chỉnh lại các trường bác sĩ thực hiện, bsi dọc kết quả, mã máy, đề nghị lấy từ màn hình mới
* 20210913 #011 BLQ:  Thêm config để chạy kết quả theo mẫu mới
* 20220523 #012 BLQ:  Thêm biến để ẩn nút lịch sử chẩn đoán hình ảnh của nội trú
* 20220829 #013 BLQ:  Thêm phân quyền cập nhật kết quả xét nghiệm
* 20220901 #014 BLQ: Issue:2174 Chỉnh lại mẫu hình ảnh theo cách mới
* 20220912 #015 BLQ: Chỉnh lại nút in lấy theo nút xem in mới
* 20220921 #016 BLQ: Thêm chức năng tự check In số lượng hình theo cấu hình loại report
* 20230311 #017 DatTB: Thêm report phiếu kết quả xét nghiệm mới
* 20230314 #018 DatTB: Tách mẫu report kết quả xét nghiệm sử dụng chung mẫu CDHA
* 20230424 #019 QTD: Truyền biến lọc Người thực hiện
* 20230608 #020 DatTB: Thêm các trường lưu bệnh phẩm xét nghiệm
*/
namespace aEMR.PCLDepartment.ViewModels
{
    [Export(typeof(IPCLDeptImagingResult)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class PCLDeptImagingResultViewModel : ViewModelBase, IPCLDeptImagingResult,
        /*#002: Vẫn giữ để chụp lấy sự kiện load thông tin từ view Search đưa xuống.*/
        IHandle<ReaderInfoPatientFromPatientPCLReqEvent<PatientPCLRequest>>,
        IHandle<LoadPatientPCLImagingResultDataCompletedEvent>,
        IHandle<CallCaptureEvent>
    {
        #region Properties
        public object UCDoctorProfileInfo { get; set; }

        private IPatientInfo _UCPatientProfileInfo;
        public IPatientInfo UCPatientProfileInfo
        {
            get { return _UCPatientProfileInfo; }
            set
            {
                _UCPatientProfileInfo = value;
                NotifyOfPropertyChange(() => UCPatientProfileInfo);
            }
        }

        public object UCPCLDepartmentSearchPCLRequest { get; set; }
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        private IPCLDepartmentSearchPCLRequest _vmSearchPCLRequest;
        public IPCLDepartmentSearchPCLRequest vmSearchPCLRequest
        {
            get
            {
                return _vmSearchPCLRequest;
            }
            set
            {
                _vmSearchPCLRequest = value;
                NotifyOfPropertyChange(() => vmSearchPCLRequest);
            }
        }

        private IPatientPCLDeptImagingResult _UCPatientPCLImageResults;
        public IPatientPCLDeptImagingResult UCPatientPCLImageResults
        {
            get { return _UCPatientPCLImageResults; }
            set
            {
                _UCPatientPCLImageResults = value;
                NotifyOfPropertyChange(() => UCPatientPCLImageResults);
            }
        }

        private IImageCapture_V4 _UCPatientPCLImageCapture;
        public IImageCapture_V4 UCPatientPCLImageCapture
        {
            get { return _UCPatientPCLImageCapture; }
            set
            {
                _UCPatientPCLImageCapture = value;
                NotifyOfPropertyChange(() => UCPatientPCLImageCapture);
            }
        }

        private PatientPCLImagingResult _ObjPatientPCLImagingResult;

        public PatientPCLImagingResult ObjPatientPCLImagingResult
        {
            get { return _ObjPatientPCLImagingResult; }
            set
            {
                if (_ObjPatientPCLImagingResult != value)
                {
                    _ObjPatientPCLImagingResult = value;
                    NotifyOfPropertyChange(() => ObjPatientPCLImagingResult);
                }
            }
        }
        public string _ParamName;
        public string ParamName
        {
            get
            {
                return _ParamName;
            }
            set
            {
                if(_ParamName != value)
                {
                    _ParamName = value;
                    NotifyOfPropertyChange(() => ParamName);
                    //▼==== #020
                    if (UCPatientPCLImagingItemResult != null)
                    {
                        UCPatientPCLImagingItemResult.ParamName = ParamName;
                    }
                    if (ParamName.Contains("Xét Nghiệm"))
                    {
                        IsLaboratory = true;
                    }
                    else
                    {
                        IsLaboratory = false;
                    }
                    //▲==== #020
                }
            }
        }

        /*▼====: #001*/
        private long _PatientPCLReqID;
        public long PatientPCLReqID
        {
            get { return _PatientPCLReqID; }
            set
            {
                if (_PatientPCLReqID != value)
                {
                    _PatientPCLReqID = value;
                    NotifyOfPropertyChange(() => PatientPCLReqID);
                }
            }
        }

        private IPatientPCLGeneralResult _UCPatientPCLGeneralResult;
        public IPatientPCLGeneralResult UCPatientPCLGeneralResult
        {
            get => _UCPatientPCLGeneralResult; set
            {
                _UCPatientPCLGeneralResult = value;
                NotifyOfPropertyChange(() => UCPatientPCLGeneralResult);
            }
        }
        private IPatientPCLImagingItemResult _UCPatientPCLImagingItemResult;
        public IPatientPCLImagingItemResult UCPatientPCLImagingItemResult
        {
            get => _UCPatientPCLImagingItemResult; set
            {
                _UCPatientPCLImagingItemResult = value;
                NotifyOfPropertyChange(() => UCPatientPCLImagingItemResult);
            }
        }
        /*▲====: #001*/

        private string _Content;
        public string Content
        {
            get
            {
                return _Content;
            }
            set
            {
                if (_Content != value)
                {
                    _Content = value;
                    NotifyOfPropertyChange(() => Content);
                }
            }
        }
        private string SAVE = eHCMSResources.T2937_G1_Luu;
        private string UPDATE = eHCMSResources.K1599_G1_CNhat;
        //▼====== #009
        private bool _IsReadOnly = false;
        public bool IsReadOnly
        {
            get
            {
                return _IsReadOnly;
            }
            set
            {
                _IsReadOnly = value;
                NotifyOfPropertyChange(() => IsReadOnly);
            }
        }
        //▲====== #009

        #endregion

        #region Events
        [ImportingConstructor]
        public PCLDeptImagingResultViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            base.HasInputBindingCmd = true;
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;
            CreateSubVM();
            Content = SAVE;
            authorization();
            //▼====: #016
            GetLookupV_ReportForm();
            //▲====: #016
            
        }

        public void SetVideoImageCaptureSourceVM(object theVideoVM)
        {
            UCPatientPCLImageCapture = (IImageCapture_V4)theVideoVM;
        }

        //▼====== #002: Thêm OnActive và DeActive để không bị chụp bắn sự kiện liên tục
        protected override void OnActivate()
        {            
            ActivateSubVM();
            
            base.OnActivate();
            Globals.EventAggregator.Subscribe(this);
            //this.ActivateItem(UCPatientPCLImageResults);
            if (Globals.PatientPCLRequest_Result != null)
            {
                Coroutine.BeginExecute(GetPatient());
                PatientPCLRequest pcl = Globals.PatientPCLRequest_Result;
                //▼====== #002  Sự kiện ReaderInfoPatientFromPatientPCLReqEvent ở đây đang thực hiện việc tự mình bắn mình, không cần thiết
                //              Anh Tuấn bảo chỉ cần gọi không cần phải bắn. Nếu tự mình bắn mình thì tương tự như tự giết mình.
                //Globals.EventAggregator.Publish(new ReaderInfoPatientFromPatientPCLReqEvent<PatientPCLRequest>() { PCLRequest = pcl });
                //▲====== #002
                ReLoad(pcl);
            }
            if (UCPatientPCLImageCapture != null)
            {
                UCPatientPCLImageCapture.ClearAllCapturedImage();
            }
        }

        protected override void OnDeactivate(bool close)
        {
            /*▼====: #003*/
            DeActivateSubVM(close);
            /*▲====: #003*/
            base.OnDeactivate(close);
            Globals.EventAggregator.Unsubscribe(this);
        }
        //▲====== #002
        #endregion

        #region Methods
        /*▼====: #003*/
        public void CreateSubVM()
        {
            ObjPatientPCLImagingResult = new PatientPCLImagingResult();
            ObjPatientPCLImagingResult.StaffID = Globals.LoggedUserAccount.Staff != null ? Globals.LoggedUserAccount.Staff.StaffID : (Globals.LoggedUserAccount.StaffID.HasValue ? Globals.LoggedUserAccount.StaffID.Value : -1);
            ObjPatientPCLImagingResult.PatientPCLReqID = -1;
            ObjPatientPCLImagingResult.PCLExamTypeID = -1;
            ObjPatientPCLImagingResult.PCLExamDate = Globals.ServerDate.GetValueOrDefault(DateTime.Now);
            ObjPatientPCLImagingResult.PCLExamForOutPatient = true;
            ObjPatientPCLImagingResult.IsExternalExam = false;
            var uc1 = Globals.GetViewModel<ILoginInfo>();
            UCDoctorProfileInfo = uc1;
            var uc2 = Globals.GetViewModel<IPatientInfo>();
            uc2.IsShowPCL = Visibility.Visible;
            UCPatientProfileInfo = uc2;
            vmSearchPCLRequest = Globals.GetViewModel<IPCLDepartmentSearchPCLRequest>();
            vmSearchPCLRequest.V_PCLMainCategory = (long)AllLookupValues.V_PCLMainCategory.Imaging;
            vmSearchPCLRequest.IsShowBtnChooseUserOfficial = Globals.ServerConfigSection.CommonItems.AllowToBorrowDoctorAccount;
            UCPatientPCLImageResults = Globals.GetViewModel<IPatientPCLDeptImagingResult>();
            UCPatientPCLImageResults.ObjPatientPCLImagingResult = ObjPatientPCLImagingResult;
            UCPatientPCLGeneralResult = Globals.GetViewModel<IPatientPCLGeneralResult>();
            UCPatientPCLImagingItemResult = Globals.GetViewModel<IPatientPCLImagingItemResult>();
            //TBL: Hien thi ten cls theo loai cls da chon o top menu
            ParamName = Globals.PCLDepartment.ObjPCLResultParamImpID.ParamName;
            if(ParamName.Contains("Siêu Âm"))
            {
                UCPatientPCLImagingItemResult.MoveInDataGridWithArrow = true;
            }
            //▼====: #019
            if (Globals.ServerConfigSection.CommonItems.IsEnableFilterPerformStaff && Globals.PCLDepartment.ObjPCLExamTypeSubCategoryID != null
                && Globals.PCLDepartment.ObjPCLExamTypeSubCategoryID.PCLExamTypeSubCategoryID != (long)AllLookupValues.V_ExamTypeSubCategoryID.ULTRASOUND
                && Globals.PCLDepartment.ObjPCLExamTypeSubCategoryID.PCLExamTypeSubCategoryID != (long)AllLookupValues.V_ExamTypeSubCategoryID.NOISOI
                && Globals.PCLDepartment.ObjPCLExamTypeSubCategoryID.PCLExamTypeSubCategoryID != (long)AllLookupValues.V_ExamTypeSubCategoryID.XETNGHIEM)
            {
                UCPatientPCLImagingItemResult.aucDoctorResult.PCLResultParamImpID = Globals.PCLDepartment.ObjPCLResultParamImpID.PCLResultParamImpID;
            }
            //▲====: #019
        }
        public void ActivateSubVM()
        {
            ActivateItem(UCPatientProfileInfo);
            ActivateItem(UCPatientPCLImageResults);
            
            ActivateItem(UCPatientPCLGeneralResult);
            ActivateItem(UCPatientPCLImagingItemResult);
        }

        public void DeActivateSubVM(bool close)
        {
            DeactivateItem(UCPatientProfileInfo, close);
            DeactivateItem(UCPatientPCLImageResults, close);
            
            DeactivateItem(UCPatientPCLGeneralResult, close);
            DeactivateItem(UCPatientPCLImagingItemResult, close);
        }

        private IEnumerator<IResult> GetPatient()
        {
            var loadPatients = new LoadPatientTask(Globals.PatientPCLRequest_Result.PatientID);
            yield return loadPatients;
            Patient CurrentPatient = loadPatients.CurrentPatient;
            if (CurrentPatient != null)
            {
                Globals.EventAggregator.Publish(new ItemPatient1<Patient>() { Item = CurrentPatient });
            }
        }
        /*▲====: #003*/

        /*▼====: #001*/
        public void Handle(ReaderInfoPatientFromPatientPCLReqEvent<PatientPCLRequest> message)
        {
            if (message != null)
            {
                //▼====== #009
                IsReadOnly = !message.IsReadOnly;
                //▲====== #009
                (UCPatientPCLImageCapture as IImageCapture_V4).ClearAllCapturedImage();
                PatientPCLReqID = message.PCLRequest.PatientPCLReqID;
                //▼====== #006: vì có cấu hình khi capture là lưu ảnh vào máy trạm => cần phải truyền PatientCode của bệnh nhân để lấy PatientCode => Khi Capture có PatientCode trong tên.
                UCPatientPCLImageCapture.PatientCode = (UCPatientProfileInfo.CurrentPatient != null ? UCPatientProfileInfo.CurrentPatient.PatientCode : "AUnkCode");
                //▲====== #006
            }
        }
        //#002: Tạo mới hàm ReLoad dựa trên Handle(ReaderInfoPatientFromPatientPCLReqEvent). Để không phải tự bắn chính mình.
        //Bổ sung thêm việc load ảnh từ con, nếu không có thì từ màn hình KQ -> các màn hình khác -> KQ sẽ không có dữ liệu.
        public void ReLoad(PatientPCLRequest patientPCLrequest)
        {
            if (patientPCLrequest != null)
            {
                (UCPatientPCLImageCapture as IImageCapture_V4).ClearAllCapturedImage();
                PatientPCLReqID = patientPCLrequest.PatientPCLReqID;
                ((IPatientPCLDeptImagingResult)UCPatientPCLImageResults).GetImageInAnotherViewModel(Globals.PatientPCLRequest_Result);
                Globals.PatientPCLRequest_Result = patientPCLrequest;
            }
        }
        public void Handle(LoadPatientPCLImagingResultDataCompletedEvent message)
        {
            if (message != null && UCPatientPCLImageResults != null
                && ((IPatientPCLDeptImagingResult)UCPatientPCLImageResults).ObjPatientPCLImagingResult != null)
            {
                ObjPatientPCLImagingResult = ((IPatientPCLDeptImagingResult)UCPatientPCLImageResults).ObjPatientPCLImagingResult;
                //TBL: Khi tim phieu cls len de thuc hien se hien them ten cls
                ParamName = string.Format("{0}: {1}", Globals.PCLDepartment.ObjPCLResultParamImpID.ParamName, ObjPatientPCLImagingResult.PCLExamType == null ? null : ObjPatientPCLImagingResult.PCLExamType.PCLExamTypeName);
                //UCPatientPCLGeneralResult.ApplyElementValues(ObjPatientPCLImagingResult.TemplateResultString, ObjPatientPCLImagingResult.PatientPCLRequest, ((IPatientPCLDeptImagingResult)UCPatientPCLImageResults).PCLResultFileStorageDetailCollection, ObjPatientPCLImagingResult.PtRegistrationCode, ObjPatientPCLImagingResult.Staff == null ? "" : ObjPatientPCLImagingResult.Staff.FullName);
                //20190220 TBL: BS thuc hien CDHA duoc lay tu nguoi thuc hien
                // 20200514 TNHX: Lấy cột HiddenFullNameOnReport để ẩn bsy thực hiện kết quả CLS (HA)
                Staff PerfromStaff = Globals.AllStaffs.Where(x => x.StaffID == ObjPatientPCLImagingResult.PerformStaffID.GetValueOrDefault(0)).FirstOrDefault();
                string PerformStaffFullName = ObjPatientPCLImagingResult.PerformStaffFullName ?? "";
                if (PerfromStaff != null && PerfromStaff.HiddenFullNameOnReport)
                {
                    PerformStaffFullName = "";
                }
                UCPatientPCLGeneralResult.ApplyElementValues(ObjPatientPCLImagingResult.TemplateResultString, ObjPatientPCLImagingResult.PatientPCLRequest, ((IPatientPCLDeptImagingResult)UCPatientPCLImageResults).PCLResultFileStorageDetailCollection, ObjPatientPCLImagingResult.PtRegistrationCode, PerformStaffFullName, ObjPatientPCLImagingResult.Suggest);
            }
            //▼====== #011
            if (!string.IsNullOrEmpty(ObjPatientPCLImagingResult.TemplateResultString))
            {
                //▼====== #005
                if (ObjPatientPCLImagingResult != null && ObjPatientPCLImagingResult.HIRepResourceCode != null)
                {
                    UCPatientPCLGeneralResult.ObjPatientPCLImagingResult_General.HIRepResourceCode = ObjPatientPCLImagingResult.HIRepResourceCode;
                    //20181213 TTM: Đổi vị trí set lại, vì khi thực hiện hàm setDefault sẽ set giá trị StaffID về 0 => lưu lần thứ 2 sẽ báo chưa chọn bác sĩ thực hiện.
                    //UCPatientPCLGeneralResult.aucHoldConsultDoctor.StaffID = (long)ObjPatientPCLImagingResult.PerformStaffID;
                    //UCPatientPCLGeneralResult.aucHoldConsultDoctor.setDefault(ObjPatientPCLImagingResult.PerformStaffFullName);
                }
                //20192704 TTM: Không nên xoá dữ liệu vì Bác sĩ thực hiện chỉ có 1 người cho cả ca nếu xoá mỗi khi thực hiện 1 lần CĐHA lại phải nhập lại => Chậm.
                else
                {
                    //UCPatientPCLGeneralResult.aucHoldConsultDoctor.setDefault();
                    //UCPatientPCLGeneralResult.aucHoldConsultDoctor.StaffID = 0;
                    //20190205 TTM: Clear dữ liệu trường đề nghị khi load cas mới để thực hiện
                    //UCPatientPCLGeneralResult.ObjPatientPCLImagingResult_General.Suggest = "";
                }
                UCPatientPCLGeneralResult.ObjPatientPCLImagingResult_General.Suggest = ObjPatientPCLImagingResult.Suggest;
                if (ObjPatientPCLImagingResult != null && ObjPatientPCLImagingResult.PerformStaffID != null && ObjPatientPCLImagingResult.PerformStaffFullName != null)
                {
                    UCPatientPCLGeneralResult.aucHoldConsultDoctor.setDefault(ObjPatientPCLImagingResult.PerformStaffFullName);
                    UCPatientPCLGeneralResult.aucHoldConsultDoctor.StaffID = (long)ObjPatientPCLImagingResult.PerformStaffID;
                }
                //20190926 TBL: Không cần phải xóa tên bác sĩ thực hiện nếu phiếu CLS đó chưa làm, vì chỉ có 1 bác sĩ thực hiện tránh cho người dùng phải nhập nhiều lần
                //else
                //{
                //    UCPatientPCLGeneralResult.aucHoldConsultDoctor.setDefault();
                //    UCPatientPCLGeneralResult.aucHoldConsultDoctor.StaffID = 0;
                //}
                //▲====== #005
                if (ObjPatientPCLImagingResult != null && ObjPatientPCLImagingResult.ResultStaffID != null && ObjPatientPCLImagingResult.ResultStaffFullName != null)
                {
                    UCPatientPCLGeneralResult.aucDoctorResult.setDefault(ObjPatientPCLImagingResult.ResultStaffFullName);
                    UCPatientPCLGeneralResult.aucDoctorResult.StaffID = (long)ObjPatientPCLImagingResult.ResultStaffID;
                }
                else
                {
                    UCPatientPCLGeneralResult.aucDoctorResult.setDefault();
                    UCPatientPCLGeneralResult.aucDoctorResult.StaffID = 0;
                }
            }
            //▼====== #010
            if (Globals.ServerConfigSection.CommonItems.ApplyTemplatePCLResultNew)
            {
                if (ObjPatientPCLImagingResult != null && ObjPatientPCLImagingResult.HIRepResourceCode != null)
                {
                    UCPatientPCLImagingItemResult.ObjPatientPCLImagingResult_General.HIRepResourceCode = ObjPatientPCLImagingResult.HIRepResourceCode;
                }
                UCPatientPCLImagingItemResult.ObjPatientPCLImagingResult_General.Suggest = ObjPatientPCLImagingResult.Suggest;
                if (ObjPatientPCLImagingResult != null && ObjPatientPCLImagingResult.PerformStaffID != null && ObjPatientPCLImagingResult.PerformStaffFullName != null)
                {
                    UCPatientPCLImagingItemResult.aucHoldConsultDoctor.setDefault(ObjPatientPCLImagingResult.PerformStaffFullName);
                    UCPatientPCLImagingItemResult.aucHoldConsultDoctor.StaffID = (long)ObjPatientPCLImagingResult.PerformStaffID;
                }
                if (ObjPatientPCLImagingResult != null && ObjPatientPCLImagingResult.ResultStaffID != null && ObjPatientPCLImagingResult.ResultStaffFullName != null)
                {
                    UCPatientPCLImagingItemResult.aucDoctorResult.setDefault(ObjPatientPCLImagingResult.ResultStaffFullName);
                    UCPatientPCLImagingItemResult.aucDoctorResult.StaffID = (long)ObjPatientPCLImagingResult.ResultStaffID;
                }
                else
                {
                    UCPatientPCLImagingItemResult.aucDoctorResult.setDefault();
                    UCPatientPCLImagingItemResult.aucDoctorResult.StaffID = 0;
                }
                if (ObjPatientPCLImagingResult != null && ObjPatientPCLImagingResult.PCLImgResultID > 0 && !string.IsNullOrEmpty(ObjPatientPCLImagingResult.TemplateResult))
                {
                    UCPatientPCLImagingItemResult.ObjPatientPCLImagingResult_General.TemplateResult = ObjPatientPCLImagingResult.TemplateResult;
                }
                else
                {
                    UCPatientPCLImagingItemResult.ObjPatientPCLImagingResult_General.TemplateResult = ObjPatientPCLImagingResult.PCLExamType.PCLExamTypeTemplateResult;
                }
            }
            //▲====== #010
            //▼===== #008
            if (ObjPatientPCLImagingResult != null)
            {
                UCPatientPCLImagingItemResult.ObjPatientPCLImagingResult_General.SpecimenID = ObjPatientPCLImagingResult.SpecimenID;
                UCPatientPCLImagingItemResult.ObjPatientPCLImagingResult_General.SampleQuality = "Đạt";
                if (ObjPatientPCLImagingResult.PatientPCLRequest != null)
                {
                    if (UCPatientPCLImagingItemResult.ObjPatientPCLImagingResult_General.PatientPCLRequest == null)
                    {
                        UCPatientPCLImagingItemResult.ObjPatientPCLImagingResult_General.PatientPCLRequest = new PatientPCLRequest();
                    }
                    UCPatientPCLImagingItemResult.ObjPatientPCLImagingResult_General.PatientPCLRequest.ReceptionTime = ObjPatientPCLImagingResult.PatientPCLRequest.ReceptionTime;
                }
            }
            //▲===== #008
            //▲====== #011
            if (ObjPatientPCLImagingResult != null && ObjPatientPCLImagingResult.PCLImgResultID > 0)
            {
                Content = UPDATE;
            }
            else if(ObjPatientPCLImagingResult.PCLImgResultID  == 0)
            {
                Content = SAVE;
            }
            //▼====== #013
            CheckAuthorbtnUpdate();
            //▲====== #013
            if (UCPatientPCLImagingItemResult != null && UCPatientPCLImagingItemResult.allPatientPCLImagingResultDetail != null && UCPatientPCLImagingItemResult.allPatientPCLImagingResultDetail.Count>0)
            {
                UCPatientPCLGeneralResult.ObjPatientPCLImagingResult_General.PatientPCLImagingResultDetail = UCPatientPCLImagingItemResult.allPatientPCLImagingResultDetail.ToList();
            }
            if(ObjPatientPCLImagingResult.PCLExamType.V_ReportForm == (long)AllLookupValues.V_ReportForm.Mau_1_Hinh ||
                ObjPatientPCLImagingResult.PCLExamType.V_ReportForm == (long)AllLookupValues.V_ReportForm.Mau_Test_Nhanh ||
                ObjPatientPCLImagingResult.PCLExamType.V_ReportForm == (long)AllLookupValues.V_ReportForm.Mau_Realtime_PCR ||
                ObjPatientPCLImagingResult.PCLExamType.V_ReportForm == (long)AllLookupValues.V_ReportForm.Mau_Helicobacter_Pylori ||
                ObjPatientPCLImagingResult.PCLExamType.V_ReportForm == (long)AllLookupValues.V_ReportForm.Mau_Xet_Nghiem ||
                //▼==== #017
                ObjPatientPCLImagingResult.PCLExamType.V_ReportForm == (long)AllLookupValues.V_ReportForm.Mau_Xet_Nghiem_V2 ||
                //▲==== #017
                //▼==== #018
                ObjPatientPCLImagingResult.PCLExamType.V_ReportForm == (long)AllLookupValues.V_ReportForm.Mau_1_Hinh_XN)
                //▲==== #018
            {
                IsShowUpload = true;
            }
            else
            {
                IsShowUpload = false;
            }
            //▼====: #012
            if (ObjPatientPCLImagingResult.PatientPCLRequest.V_RegistrationType == (long)AllLookupValues.RegistrationType.NGOAI_TRU)
            {
                IsShowBtnHistory = true;
            }
            else
            {
                IsShowBtnHistory = false;
            }
            //▲====: #012
        }
        public void Handle(CallCaptureEvent message)
        {
            if (message == null)
            {
                return;
            }
            CallCapture();
        }

        private bool CheckValid()
        {
            if (!Globals.ServerConfigSection.CommonItems.ApplyTemplatePCLResultNew)
            {
                //▼====== #005
                if (UCPatientPCLImageResults.ObjPatientPCLImagingResult != null && UCPatientPCLGeneralResult.ObjPatientPCLImagingResult_General.HIRepResourceCode != null)
                {
                    UCPatientPCLImageResults.ObjPatientPCLImagingResult.HIRepResourceCode = UCPatientPCLGeneralResult.ObjPatientPCLImagingResult_General.HIRepResourceCode;
                }
                //Kiểm tra xem BS thực hiện đã chọn chưa.
                if (UCPatientPCLGeneralResult.aucHoldConsultDoctor.StaffID == 0 && UCPatientPCLGeneralResult.aucHoldConsultDoctor != null || UCPatientPCLGeneralResult.aucHoldConsultDoctor == null)
                {
                    //MessageBox.Show(eHCMSResources.Z2376_G1_ChuaChonBSThucHien);
                    MessageBox.Show("Chưa chọn bác sĩ đọc kết quả!!!");
                    return false;
                }
                //▲====== #005
                //▼===== #008
                if (UCPatientPCLGeneralResult.ObjPatientPCLImagingResult_General != null)
                {
                    UCPatientPCLImageResults.ObjPatientPCLImagingResult.SpecimenID = UCPatientPCLImagingItemResult.ObjPatientPCLImagingResult_General.SpecimenID;
                }
                //▲===== #008
            }
            else
            {
                //▼====== #010
                if (UCPatientPCLImageResults.ObjPatientPCLImagingResult != null && UCPatientPCLImagingItemResult.ObjPatientPCLImagingResult_General.HIRepResourceCode != null)
                {
                    UCPatientPCLImageResults.ObjPatientPCLImagingResult.HIRepResourceCode = UCPatientPCLImagingItemResult.ObjPatientPCLImagingResult_General.HIRepResourceCode;
                }
                //Kiểm tra xem BS thực hiện đã chọn chưa.
                if (UCPatientPCLImagingItemResult.aucHoldConsultDoctor.StaffID == 0 && UCPatientPCLImagingItemResult.aucHoldConsultDoctor != null || UCPatientPCLImagingItemResult.aucHoldConsultDoctor == null)
                {
                    //MessageBox.Show(eHCMSResources.Z2376_G1_ChuaChonBSThucHien);
                    MessageBox.Show("Chưa chọn bác sĩ đọc kết quả!!!");
                    return false;
                }
                
                //▲====== #010
                if (UCPatientPCLImagingItemResult.allPatientPCLImagingResultDetail != null && UCPatientPCLImagingItemResult.allPatientPCLImagingResultDetail.Count > 0
                    && UCPatientPCLImagingItemResult.allPatientPCLImagingResultDetail.Where(x => x.Value != null).ToList().Count > 0)
                {
                    UCPatientPCLImageResults.ObjPatientPCLImagingResult.PatientPCLImagingResultDetail = UCPatientPCLImagingItemResult.allPatientPCLImagingResultDetail.ToList();
                }
                else
                {
                    MessageBox.Show("Cận lâm sàng chưa được tạo mẫu kết quả (TestItem)");
                    return false;
                }
                //▼===== #008
                if (UCPatientPCLGeneralResult.ObjPatientPCLImagingResult_General != null)
                {
                    UCPatientPCLImageResults.ObjPatientPCLImagingResult.SpecimenID = UCPatientPCLImagingItemResult.ObjPatientPCLImagingResult_General.SpecimenID;
                }
                //▲===== #008
            }
            //Kiem tra da chon ma may chua
            if (Globals.ServerConfigSection.CommonItems.OrganizationUseSoftware == 0
                && (UCPatientPCLImageResults.ObjPatientPCLImagingResult.PCLExamType == null || UCPatientPCLImageResults.ObjPatientPCLImagingResult.PCLExamType.V_PCLMainCategory != (long)AllLookupValues.V_PCLMainCategory.GeneralSugery)
                && (UCPatientPCLImageResults.ObjPatientPCLImagingResult.HIRepResourceCode == null || UCPatientPCLImageResults.ObjPatientPCLImagingResult.HIRepResourceCode == string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.Z0111_G1_HayChon1GTri)))
            {
                MessageBox.Show(eHCMSResources.Z2242_G1_ChuaChonMaMay);
                return false;
            }
            if (string.IsNullOrEmpty(UCPatientPCLImagingItemResult.ObjPatientPCLImagingResult_General.TemplateResult))
            {
                MessageBox.Show("Chưa nhập kết luận không thể lưu!");
                return false;
            }
            return true;
        }
        public void btnSaveCmd()
        {
            //Kiem tra da chon benh nhan chua
            if (PatientPCLReqID <= 0)
            {
                MessageBox.Show(eHCMSResources.A0301_G1_Msg_InfoChonBN);
                return;
            }
            if (!CheckValid())
            {
                return;
            }
            InsertPatientPCLImagingResults();
        }
        public void btnViewPrint()
        {
            Globals.EventAggregator.Publish(new PrintEventActionView());
        }
        public void btnPrint()
        {
            //Globals.EventAggregator.Publish(new PrintEventActionView() { IsDirectPrint = true });
            if (ObjPatientPCLImagingResult.PatientPCLRequest == null)
            {
                return;
            }
            if (ObjPatientPCLImagingResult.PCLExamType == null || ObjPatientPCLImagingResult.PCLExamType.V_ReportForm == 0)
            {
                return;
            }
            string reportDepartmentOfHealth = Globals.ServerConfigSection.CommonItems.ReportDepartmentOfHealth;
            string reportHospitalName = Globals.ServerConfigSection.CommonItems.ReportHospitalName;
            string reportHospitalAddress = Globals.ServerConfigSection.CommonItems.ReportHospitalAddress;
            string hospitalCode = Globals.ServerConfigSection.Hospitals.HospitalCode;
            bool isApplyPCRDual = Globals.ServerConfigSection.CommonItems.IsApplyPCRDual;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new ReportServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetPCLImagingResultInPdfFormat(ObjPatientPCLImagingResult.PCLExamType.V_ReportForm, ObjPatientPCLImagingResult.PCLImgResultID,
                        (long)ObjPatientPCLImagingResult.PatientPCLRequest.V_PCLRequestType, reportHospitalName, hospitalCode, reportDepartmentOfHealth,
                        reportHospitalAddress, isApplyPCRDual, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndGetPCLImagingResultInPdfFormat(asyncResult);
                                var printEvt = new ActiveXPrintEvt(this, PrinterType.IN_PHIEU, results, ActiveXPrintType.ByteArray, 1);
                                Globals.EventAggregator.Publish(printEvt);
                            }
                            catch (Exception ex)
                            {
                                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
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
        private void InsertPatientPCLImagingResults()
        {
            this.ShowBusyIndicator();
            List<PCLResultFileStorageDetail> mFileForDelete = ((IPatientPCLDeptImagingResult)UCPatientPCLImageResults).FileForDelete;
            SaveImageToLocalDrv_WhenCapturing = Globals.ServerConfigSection.Pcls.SaveImgWhenCapturing_Local;
            _FileForStore = this.FileForStore;
            try
            {
                if ((((IPatientPCLDeptImagingResult)UCPatientPCLImageResults).TotalFile + _FileForStore.Count - mFileForDelete.Count) > Globals.ServerConfigSection.Pcls.MaxEchogramImageFile)
                {
                    MessageBox.Show(eHCMSResources.K0457_G1_VuotQuaSLgFileToiDaChoPhep);
                    this.HideBusyIndicator();
                    return;
                }
                if (((IPatientPCLDeptImagingResult)UCPatientPCLImageResults).FileForStore != null)
                {
                    _FileForStore.AddRange(((IPatientPCLDeptImagingResult)UCPatientPCLImageResults).FileForStore);
                }
                _FileForStore = GenerateFileNameForImage(_FileForStore);
            }
            catch (Exception ex)
            {
                MessageBox.Show("PatientPCLImagingResults_Insert Get File: " + ex.Message);
            }
          
            if (!Globals.ServerConfigSection.CommonItems.ApplyTemplatePCLResultNew)
            {
                string TemplateResultDescription;
                string TemplateResult;
                string TemplateResultString = UCPatientPCLGeneralResult.GetBodyValue(_FileForStore, mFileForDelete, out TemplateResultDescription, out TemplateResult);
                if (UCPatientPCLGeneralResult != null && !string.IsNullOrEmpty(TemplateResultString))
                {
                    ((IPatientPCLDeptImagingResult)UCPatientPCLImageResults).ObjPatientPCLImagingResult.TemplateResult = TemplateResult;
                    ((IPatientPCLDeptImagingResult)UCPatientPCLImageResults).ObjPatientPCLImagingResult.TemplateResultDescription = TemplateResultDescription;
                    ((IPatientPCLDeptImagingResult)UCPatientPCLImageResults).ObjPatientPCLImagingResult.TemplateResultFileName = ((IPatientPCLDeptImagingResult)UCPatientPCLImageResults).ObjPatientPCLImagingResult.TemplateResultString;
                    ((IPatientPCLDeptImagingResult)UCPatientPCLImageResults).ObjPatientPCLImagingResult.TemplateResultString = TemplateResultString;
                }
                //▼====== #005
                if (UCPatientPCLGeneralResult.aucHoldConsultDoctor != null)
                {
                    ((IPatientPCLDeptImagingResult)UCPatientPCLImageResults).ObjPatientPCLImagingResult.PerformStaffID = UCPatientPCLGeneralResult.aucHoldConsultDoctor.StaffID;
                    //▼====: #007
                    ((IPatientPCLDeptImagingResult)UCPatientPCLImageResults).ObjPatientPCLImagingResult.PerformStaffFullName = UCPatientPCLGeneralResult.aucHoldConsultDoctor.StaffName;
                    //▲====: #007
                    ((IPatientPCLDeptImagingResult)UCPatientPCLImageResults).ObjPatientPCLImagingResult.PerformedDate = Globals.GetCurServerDateTime();
                }
                if (UCPatientPCLGeneralResult.ObjPatientPCLImagingResult_General.Suggest != null)
                {
                    ((IPatientPCLDeptImagingResult)UCPatientPCLImageResults).ObjPatientPCLImagingResult.Suggest = UCPatientPCLGeneralResult.ObjPatientPCLImagingResult_General.Suggest;
                }
                if (UCPatientPCLGeneralResult.aucDoctorResult != null)
                {
                    ((IPatientPCLDeptImagingResult)UCPatientPCLImageResults).ObjPatientPCLImagingResult.ResultStaffID = UCPatientPCLGeneralResult.aucDoctorResult.StaffID;
                }
                //▲====== #005
            }
            else
            {
                //▼====: #010
                if (UCPatientPCLImagingItemResult.aucHoldConsultDoctor != null)
                {
                    ((IPatientPCLDeptImagingResult)UCPatientPCLImageResults).ObjPatientPCLImagingResult.PerformStaffID = UCPatientPCLImagingItemResult.aucHoldConsultDoctor.StaffID;
                    ((IPatientPCLDeptImagingResult)UCPatientPCLImageResults).ObjPatientPCLImagingResult.PerformStaffFullName = UCPatientPCLImagingItemResult.aucHoldConsultDoctor.StaffName;
                    ((IPatientPCLDeptImagingResult)UCPatientPCLImageResults).ObjPatientPCLImagingResult.PerformedDate = Globals.GetCurServerDateTime();
                }
                if (UCPatientPCLImagingItemResult.ObjPatientPCLImagingResult_General.Suggest != null)
                {
                    ((IPatientPCLDeptImagingResult)UCPatientPCLImageResults).ObjPatientPCLImagingResult.Suggest = UCPatientPCLImagingItemResult.ObjPatientPCLImagingResult_General.Suggest;
                }
                if (UCPatientPCLImagingItemResult.aucDoctorResult != null)
                {
                    ((IPatientPCLDeptImagingResult)UCPatientPCLImageResults).ObjPatientPCLImagingResult.ResultStaffID = UCPatientPCLImagingItemResult.aucDoctorResult.StaffID;
                }
                if (!string.IsNullOrEmpty(UCPatientPCLImagingItemResult.ObjPatientPCLImagingResult_General.TemplateResult))
                {
                    ((IPatientPCLDeptImagingResult)UCPatientPCLImageResults).ObjPatientPCLImagingResult.TemplateResult = UCPatientPCLImagingItemResult.ObjPatientPCLImagingResult_General.TemplateResult;
                }
                //▲====: #010

            }
            //▼====: #008
            ((IPatientPCLDeptImagingResult)UCPatientPCLImageResults).ObjPatientPCLImagingResult.UserOfficialAccountID = Globals.DoctorAccountBorrowed.StaffID;
            //▲====: #008
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PCLsImportClient())
                {
                    try
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginAddPCLResultFileStorageDetails(((IPatientPCLDeptImagingResult)UCPatientPCLImageResults).ObjPatientPCLImagingResult, _FileForStore, mFileForDelete, FileForUpdate,
                                Globals.DispatchCallback((asyncResult) =>
                                {
                                    try
                                    {
                                        bool res = contract.EndAddPCLResultFileStorageDetails(asyncResult);
                                        if (res)
                                        {
                                            MessageBox.Show(eHCMSResources.Z1562_G1_DaLuu);
                                            IPatientPCLDeptImagingResult vImagingResult = UCPatientPCLImageResults as IPatientPCLDeptImagingResult;
                                            vImagingResult.LoadData(((IPatientPCLDeptImagingResult)UCPatientPCLImageResults).ObjPatientPCLImagingResult.PatientPCLRequest);
                                            IImageCapture_V4 vImageCapture = UCPatientPCLImageCapture as IImageCapture_V4;
                                            vImageCapture.ClearSelectedImage();
                                            //ClearSelectedImage();
                                            if (Content == SAVE && Globals.ServerConfigSection.CommonItems.IsEnableSendSMSLab 
                                                && Globals.PCLDepartment.ObjPCLExamTypeSubCategoryID.PCLExamTypeSubCategoryID == (long)AllLookupValues.V_ExamTypeSubCategoryID.XETNGHIEM)
                                            {
                                                bool send = GlobalsNAV.SaveDataForSmsLab(UCPatientPCLImageResults.ObjPatientPCLImagingResult.PatientPCLRequest.PatientPCLReqID, ObjPatientPCLImagingResult.PatientPCLRequest.PatientID, null);
                                                if(send)
                                                {
                                                    MessageBox.Show("Đã gửi SMS thông báo thành công!");
                                                }
                                                else
                                                {
                                                    MessageBox.Show("Chưa gửi được SMS thông báo, xem báo cáo để biết chi tiết lỗi!");
                                                }
                                            }
                                            Content = UPDATE;
                                            //▼====== #013
                                            CheckAuthorbtnUpdate();
                                            //▲====== #013
                                            //▼====: #007
                                            if (Globals.ServerConfigSection.Pcls.AutoCreatePACWorklist)
                                            {
                                                PatientPCLImagingResult temp = ((IPatientPCLDeptImagingResult)UCPatientPCLImageResults).ObjPatientPCLImagingResult.DeepCopy();
                                                temp.PatientPCLRequest.V_PCLRequestStatus = AllLookupValues.V_PCLRequestStatus.CLOSE;
                                                PACResult PACResult_Items = new PACResult(temp);
                                                GlobalsNAV.AddPCLResultToPAC(PACResult_Items);
                                            }

                                            if (null != ((IPatientPCLDeptImagingResult)UCPatientPCLImageResults).ObjPatientPCLImagingResult
                                                && null != ((IPatientPCLDeptImagingResult)UCPatientPCLImageResults).ObjPatientPCLImagingResult.PatientPCLRequest
                                                && (long) AllLookupValues.RegistrationType.NGOAI_TRU == ((IPatientPCLDeptImagingResult)UCPatientPCLImageResults).ObjPatientPCLImagingResult.PatientPCLRequest.V_RegistrationType
                                                && null != ((IPatientPCLDeptImagingResult)UCPatientPCLImageResults).ObjPatientPCLImagingResult.PatientPCLRequest.Patient
                                                && GlobalsNAV.IsQMSEnable() && Globals.ServerConfigSection.CommonItems.IsEnableQMSForPCL)
                                            {
                                                GlobalsNAV.UpdateOrder(GlobalsNAV.InitOrder(
                                                    ((IPatientPCLDeptImagingResult)UCPatientPCLImageResults).ObjPatientPCLImagingResult.PatientPCLRequest.Patient,
                                                    Globals.DeptLocation.DeptID, Globals.DeptLocation.DeptLocationID, OrderDTO.DONE_STATUS));
                                            }
                                            //▲====: #007
                                            Globals.EventAggregator.Publish(new PCLResultReloadEvent { PatientPCLRequest_Imaging = ObjPatientPCLImagingResult.PatientPCLRequest });
                                        }
                                        else
                                        {
                                            MessageBox.Show(eHCMSResources.Z0477_G1_LuuKhongThanhCong);
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        MessageBox.Show("PatientPCLImagingResults_Insert EndCall: " + ex.Message);
                                    }
                                    finally
                                    {
                                        this.HideBusyIndicator();
                                    }
                                }), null);

                    }
                    catch (Exception ex)
                    {
                        this.HideBusyIndicator();
                        MessageBox.Show("PatientPCLImagingResults_Insert BeginCall: " + ex.Message);
                    }
                }
            });
            t.Start();
        }
        private List<PCLResultFileStorageDetail> GenerateFileNameForImage(List<PCLResultFileStorageDetail> images)
        {

            if (images == null || images.Count == 0)
            {
                return images;
            }
            for (int i = 0; i < images.Count; i++)
            {
                images[i].PCLResultFileName = string.Join("-", ObjPatientPCLImagingResult.PatientPCLRequest.PatientCode, Globals.GetCurServerDateTime().ToString("yyMMddHHmmss"), Guid.NewGuid()) + ".jpg";
            }

            return images;
        }
        /*▲====: #001*/
        #endregion

        private ObservableCollection<WriteableBitmap> gImages { get { return UCPatientPCLImageCapture == null ? null : UCPatientPCLImageCapture.gImages; } }
        private ListBox gSnapshots { get; set; }
        
        public void Snapshots_Loaded(object sender, RoutedEventArgs e)
        {
            gSnapshots = sender as ListBox;
            gSnapshots.ItemsSource = gImages;
        }


        private List<WriteableBitmap> GetSelectedImage(ListBox aSnapshots)
        {
            List<WriteableBitmap> SelectedImage = new List<WriteableBitmap>();
            if (aSnapshots != null)
            {
                for (int i = 0; i < aSnapshots.Items.Count; i++)
                {
                    ListBoxItem mListBoxItem = (ListBoxItem)(aSnapshots.ItemContainerGenerator.ContainerFromItem(aSnapshots.Items[i]));
                    List<Control> mChildren = aSnapshots.GetChildrenByType<Control>();
                    CheckBox mCheckBox = mChildren.OfType<CheckBox>().First(x => x.Name.Equals("cbImage"));
                    if (mCheckBox.IsChecked == true)
                    {
                        SelectedImage.Add(gImages[i]);
                    }
                }
            }
            return SelectedImage;
        }

        private List<PCLResultFileStorageDetail> GetSelectedImageForStore_New(bool SaveImageToLocalDrv_WhenCapturing)
        {
            string strPatientCode = (UCPatientProfileInfo.CurrentPatient != null ? UCPatientProfileInfo.CurrentPatient.PatientCode : "AUnkCode");
            //▼====: #016
            List<PCLResultFileStorageDetail> mPCLResultFileStorageDetail = UCPatientPCLImageCapture.GetFileForStore(strPatientCode, true, SaveImageToLocalDrv_WhenCapturing, GetCountImageFromReportForm());
            //▲====: #016
            if (UCPatientPCLImageCapture == null || mPCLResultFileStorageDetail == null || mPCLResultFileStorageDetail.Count == 0)
                return new List<PCLResultFileStorageDetail>();
                                    
            return mPCLResultFileStorageDetail;            
        }

        private List<PCLResultFileStorageDetail> GetSelectedImageForStore(ListBox aSnapshots)
        {
            List<PCLResultFileStorageDetail> mPCLResultFileStorageDetail = new List<PCLResultFileStorageDetail>();
            if (aSnapshots != null)
            {
                for (int i = 0; i < aSnapshots.Items.Count; i++)
                {
                    ListBoxItem mListBoxItem = (ListBoxItem)(aSnapshots.ItemContainerGenerator.ContainerFromItem(aSnapshots.Items[i]));
                    List<CheckBox> mChildren = mListBoxItem.GetChildrenByType<CheckBox>();
                    CheckBox mCheckBox = mChildren.OfType<CheckBox>().First(x => x.Name.Equals("cbImage"));
                    if (mCheckBox.IsChecked == true)
                    {
                        var ImageArray = new MemoryStream();
                        BmpBitmapEncoder mEncoder = new BmpBitmapEncoder();
                        mEncoder.Frames.Add(BitmapFrame.Create(gImages[i]));
                        mEncoder.Save(ImageArray);
                        PCLResultFileStorageDetail aPCLResultFileStorageDetail = new PCLResultFileStorageDetail();
                        aPCLResultFileStorageDetail.IsImage = true;
                        aPCLResultFileStorageDetail.File = ImageArray.ToArray();
                        aPCLResultFileStorageDetail.V_ResultType = (long)AllLookupValues.FileStorageResultType.IMAGES;
                        mPCLResultFileStorageDetail.Add(aPCLResultFileStorageDetail);
                    }
                }
            }
            return mPCLResultFileStorageDetail;
        }
        private List<PCLResultFileStorageDetail> _FileForStore = new List<PCLResultFileStorageDetail>();
        private List<PCLResultFileStorageDetail> FileForStore
        {
            //get { return GetSelectedImageForStore(gSnapshots); }
            get { return GetSelectedImageForStore_New(SaveImageToLocalDrv_WhenCapturing); }
        }
        private bool _SaveImageToLocalDrv_WhenCapturing = false;
        public bool SaveImageToLocalDrv_WhenCapturing
        {
            get { return _SaveImageToLocalDrv_WhenCapturing; }
            set
            {
                if (_SaveImageToLocalDrv_WhenCapturing != value)
                {
                    _SaveImageToLocalDrv_WhenCapturing = value;
                }
                NotifyOfPropertyChange(() => SaveImageToLocalDrv_WhenCapturing);
            }
        }
        private List<PCLResultFileStorageDetail> FileForUpdate
        {
            get
            {
                return UCPatientPCLImageResults != null && UCPatientPCLImageResults is IPatientPCLDeptImagingResult ? (UCPatientPCLImageResults as IPatientPCLDeptImagingResult).FileForUpdate : null;
            }
        }
        private void DeleteSelectedImage(List<WriteableBitmap> SelectedImage)
        {
            foreach (WriteableBitmap mImage in SelectedImage)
                gImages.Remove(mImage);
        }
        private void ClearSelectedImage()
        {
            List<WriteableBitmap> mSelectedImage = GetSelectedImage(gSnapshots);
            DeleteSelectedImage(mSelectedImage);
        }
        private void CallCapture()
        {
            UCPatientPCLImageCapture.CaptureVideoImage();
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
            yield return new InputBindingCommand(CallCapture)
            {
                HotKey_Registered_Name = "ghkCallCap",
                //20190411 TTM: Thay đổi hot key chụp hình từ Ctrl + Q => F2.
                //GestureModifier = ModifierKeys.Control,                
                //GestureKey = (Key)Keys.Q
                GestureModifier = ModifierKeys.None,
                GestureKey = (Key)Keys.F2
            };
        }
        //==== #004 ====
        public void btnDelete()
        {
            if (PatientPCLReqID <= 0)
            {
                MessageBox.Show(eHCMSResources.A0301_G1_Msg_InfoChonBN);
                return;
            }
            if (MessageBox.Show(eHCMSResources.K0483_G1_BanCoChacChanMuonXoa, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                DeletePatientPCLImagingResult(PatientPCLReqID, (long)ObjPatientPCLImagingResult.PatientPCLRequest.V_PCLRequestType, Globals.LoggedUserAccount.Staff.StaffID);
            }
        }
        private void DeletePatientPCLImagingResult(long PatientPCLReqID, long PCLRequestTypeID, long CancelStaffID)
        {
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PCLsClient())
                    {
                        IPCLs contract = serviceFactory.ServiceInstance;
                        contract.BeginDeletePatientPCLImagingResult(PatientPCLReqID, PCLRequestTypeID, CancelStaffID, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndDeletePatientPCLImagingResult(asyncResult);
                                if (results)
                                {
                                    MessageBox.Show(eHCMSResources.K0537_G1_XoaOk);
                                    Content = SAVE;
                                    //▼====== #013
                                    CheckAuthorbtnUpdate();
                                    //▲====== #013
                                }
                                else
                                {
                                    MessageBox.Show(eHCMSResources.K0484_G1_XoaFail);
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
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    this.HideBusyIndicator();
                }
            });

            t.Start();
        }
        //==== #004 ====
        //▼====== #005
        private PatientPCLImagingResult _ObjPatientPCLImagingResult_General;
        public PatientPCLImagingResult ObjPatientPCLImagingResult_General
        {
            get { return _ObjPatientPCLImagingResult_General; }
            set
            {
                if (_ObjPatientPCLImagingResult_General != value)
                {
                    _ObjPatientPCLImagingResult_General = value;
                    NotifyOfPropertyChange(() => ObjPatientPCLImagingResult_General);
                }
            }
        }
        //▲====== #005
        public void btnFormulaMedicine()
        {
            Action<IFormulaMedicine> onInitDlg = delegate (IFormulaMedicine proAlloc)
            {
            };
            GlobalsNAV.ShowDialog<IFormulaMedicine>(onInitDlg);
        }
        public void btHistory()
        {
            if (PatientPCLReqID <= 0)
            {
                MessageBox.Show(eHCMSResources.A0301_G1_Msg_InfoChonBN);
                return;
            }
            Action<IPCLRequestHistoryByPCLExamType> onInitDlg = (typeInfo) =>
            {
                typeInfo.PatientID = ObjPatientPCLImagingResult.PatientPCLRequest.PatientID;
                typeInfo.PCLExamTypeID = (long)ObjPatientPCLImagingResult.PatientPCLRequest.PCLExamTypeID;
                typeInfo.LoadData();
            };
            GlobalsNAV.ShowDialog<IPCLRequestHistoryByPCLExamType>(onInitDlg);
        }

        //▼==== #020
        public bool _IsLaboratory;
        public bool IsLaboratory
        {
            get
            {
                return _IsLaboratory;
            }
            set
            {
                if (_IsLaboratory != value)
                {
                    _IsLaboratory = value;
                    NotifyOfPropertyChange(() => IsLaboratory);
                }
            }
        }

        public void BtnPrintBilingual()
        {
            btnViewPrintNew(true);
        }
        //▲==== #020

        public void btnViewPrintNew(bool IsBilingual = false)
        {
            if(ObjPatientPCLImagingResult.PatientPCLRequest == null)
            {
                return;
            }
            if (ObjPatientPCLImagingResult.PCLExamType == null || ObjPatientPCLImagingResult.PCLExamType.V_ReportForm == 0)
            {
                return;
            }
            Action<ICommonPreviewView> onInitDlg = delegate (ICommonPreviewView proAlloc)
            {
                proAlloc.ID = ObjPatientPCLImagingResult.PCLImgResultID;
                //▼==== #020
                proAlloc.IsBilingual = IsBilingual;
                //▲==== #020
                //proAlloc.RegistrationID = Registration_DataStorage.CurrentPatientRegistration.PtRegistrationID;
                switch (ObjPatientPCLImagingResult.PCLExamType.V_ReportForm)
                {
                    case (long)AllLookupValues.V_ReportForm.Mau_0_Hinh:
                        proAlloc.eItem = ReportName.XRpt_PCLImagingResult_New_0_Hinh;
                        break;
                    case (long)AllLookupValues.V_ReportForm.Mau_1_Hinh:
                        proAlloc.eItem = ReportName.XRpt_PCLImagingResult_New_1_Hinh;
                        break;
                    case (long)AllLookupValues.V_ReportForm.Mau_2_Hinh:
                        proAlloc.eItem = ReportName.XRpt_PCLImagingResult_New;
                        break;
                    case (long)AllLookupValues.V_ReportForm.Mau_3_Hinh:
                        proAlloc.eItem = ReportName.XRpt_PCLImagingResult_New_3_Hinh;
                        break;
                    case (long)AllLookupValues.V_ReportForm.Mau_4_Hinh:
                        proAlloc.eItem = ReportName.XRpt_PCLImagingResult_New_4_Hinh;
                        break;
                    case (long)AllLookupValues.V_ReportForm.Mau_6_Hinh:
                        proAlloc.eItem = ReportName.XRpt_PCLImagingResult_New_6_Hinh;
                        break;
                    case (long)AllLookupValues.V_ReportForm.Mau_Realtime_PCR:
                        proAlloc.eItem = ReportName.XRpt_PCLImagingResult_New_Realtime_PCR_Cov;
                        break;
                    case (long)AllLookupValues.V_ReportForm.Mau_Test_Nhanh:
                        proAlloc.eItem = ReportName.XRpt_PCLImagingResult_New_Test_Nhanh_Cov;
                        break;
                    case (long)AllLookupValues.V_ReportForm.Mau_Xet_Nghiem:
                        proAlloc.eItem = ReportName.XRpt_PCLImagingResult_New_Xet_Nghiem;
                        break;
                    case (long)AllLookupValues.V_ReportForm.Mau_Helicobacter_Pylori:
                        proAlloc.eItem = ReportName.XRpt_PCLImagingResult_New_Helicobacter_Pylori;
                        break;
                    case (long)AllLookupValues.V_ReportForm.Mau_Sieu_Am_Tim:
                        proAlloc.eItem = ReportName.XRpt_PCLImagingResult_New_Sieu_Am_Tim;
                        break;
                    case (long)AllLookupValues.V_ReportForm.Mau_Sieu_Am_San_4D:
                        proAlloc.eItem = ReportName.XRpt_PCLImagingResult_New_Sieu_Am_San_4D;
                        break;
                    case (long)AllLookupValues.V_ReportForm.Mau_Dien_Tim:
                        proAlloc.eItem = ReportName.XRpt_PCLImagingResult_New_Dien_Tim;
                        break;
                    case (long)AllLookupValues.V_ReportForm.Mau_Dien_Nao:
                        proAlloc.eItem = ReportName.XRpt_PCLImagingResult_New_Dien_Nao;
                        break;
                    case (long)AllLookupValues.V_ReportForm.Mau_ABI:
                        proAlloc.eItem = ReportName.XRpt_PCLImagingResult_New_ABI;
                        break;
                    case (long)AllLookupValues.V_ReportForm.Mau_Dien_Tim_Gang_Suc:
                        proAlloc.eItem = ReportName.XRpt_PCLImagingResult_New_Dien_Tim_Gang_Suc;
                        break;
                    //▼====: #013
                    case (long)AllLookupValues.V_ReportForm.Mau_CLVT_Hai_Ham:
                        proAlloc.eItem = ReportName.XRpt_PCLImagingResult_New_CLVT_Hai_Ham;
                        break;
                    case (long)AllLookupValues.V_ReportForm.Mau_Sieu_Am_San_4D_New:
                        proAlloc.eItem = ReportName.XRpt_PCLImagingResult_New_Sieu_Am_San_4D_New;
                        break;
                    case (long)AllLookupValues.V_ReportForm.Mau_6_Hinh_2_New:
                        proAlloc.eItem = ReportName.XRpt_PCLImagingResult_New_6_Hinh_2_New;
                        break;
                    case (long)AllLookupValues.V_ReportForm.Mau_6_Hinh_1_New:
                        proAlloc.eItem = ReportName.XRpt_PCLImagingResult_New_6_Hinh_1_New;
                        break;
                    case (long)AllLookupValues.V_ReportForm.Mau_Sieu_Am_Tim_New:
                        proAlloc.eItem = ReportName.XRpt_PCLImagingResult_New_Sieu_Am_Tim_New;
                        break;
                    case (long)AllLookupValues.V_ReportForm.Mau_Noi_Soi_9_Hinh:
                        proAlloc.eItem = ReportName.XRpt_PCLImagingResult_New_Noi_Soi_9_Hinh;
                        break;

                    //▲====: #013
                    //▼==== #017
                    case (long)AllLookupValues.V_ReportForm.Mau_0_Hinh_V2:
                        proAlloc.eItem = ReportName.XRpt_PCLImagingResult_New_0_Hinh_V2;
                        break;
                    case (long)AllLookupValues.V_ReportForm.Mau_Xet_Nghiem_V2:
                        proAlloc.eItem = ReportName.XRpt_PCLImagingResult_New_Xet_Nghiem_V2;
                        break;
                    //▲==== #017
                    //▼==== #018
                    case (long)AllLookupValues.V_ReportForm.Mau_0_Hinh_XN:
                        proAlloc.eItem = ReportName.XRpt_PCLImagingResult_New_0_Hinh_XN;
                        break;
                    case (long)AllLookupValues.V_ReportForm.Mau_1_Hinh_XN:
                        proAlloc.eItem = ReportName.XRpt_PCLImagingResult_New_1_Hinh_XN;
                        break;
                    //▲==== #018
                    default:
                        proAlloc.eItem = ReportName.XRpt_PCLImagingResult_New;
                        break;
                    
                }
                proAlloc.V_PCLRequestType = (long)ObjPatientPCLImagingResult.PatientPCLRequest.V_PCLRequestType;
                //proAlloc.TieuDeRpt = eHCMSResources.P0383_G1_PhYeuCauXetNghiem;
            };
            GlobalsNAV.ShowDialog<ICommonPreviewView>(onInitDlg);
        }
        public void btnExportPDF()
        {
            if (ObjPatientPCLImagingResult.PatientPCLRequest == null)
            {
                return;
            }
            if (ObjPatientPCLImagingResult.PCLExamType == null || ObjPatientPCLImagingResult.PCLExamType.V_ReportForm == 0)
            {
                return;
            }
            string reportDepartmentOfHealth = Globals.ServerConfigSection.CommonItems.ReportDepartmentOfHealth;
            string reportHospitalName = Globals.ServerConfigSection.CommonItems.ReportHospitalName;
            string reportHospitalAddress = Globals.ServerConfigSection.CommonItems.ReportHospitalAddress;
            string hospitalCode = Globals.ServerConfigSection.Hospitals.HospitalCode;
            bool isApplyPCRDual = Globals.ServerConfigSection.CommonItems.IsApplyPCRDual;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new ReportServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetPCLImagingResultInPdfFormat(ObjPatientPCLImagingResult.PCLExamType.V_ReportForm, ObjPatientPCLImagingResult.PCLImgResultID,
                        (long)ObjPatientPCLImagingResult.PatientPCLRequest.V_PCLRequestType, reportHospitalName, hospitalCode, reportDepartmentOfHealth,
                        reportHospitalAddress, isApplyPCRDual, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndGetPCLImagingResultInPdfFormat(asyncResult);
                            //UploadFile(results);
                            SavePCLImagingResultPDF(results);
                            //using (FileStream fs = new FileStream(@"\\172.25.200.229\AppTest\test.pdf", FileMode.Create, FileAccess.Write))
                            //{
                            //    fs.Write(results, 0, results.Length);
                            //    fs.Close();
                            //}
                            //var printEvt = new ActiveXPrintEvt(this, PrinterType.IN_PHIEU, results, ActiveXPrintType.ByteArray);
                            //Globals.EventAggregator.Publish(results);
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
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
        private void SavePCLImagingResultPDF(byte[] FileExport)
        {
            string FileName = "";
            FileName = string.Join("_", ObjPatientPCLImagingResult.PatientPCLRequest.PatientCode,Globals.ReplaceVietnameseSign(ObjPatientPCLImagingResult.PatientPCLRequest.FullName)
                , ObjPatientPCLImagingResult.PatientPCLRequest.ExamDate.Value.ToString("yyyyMMddHHmmss"))+".pdf";
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PCLsImportClient())
                {
                    try
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginSavePCLImagingResultPDF(FileExport, FileName,
                                Globals.DispatchCallback((asyncResult) =>
                                {
                                    try
                                    {
                                        bool res = contract.EndSavePCLImagingResultPDF(asyncResult);
                                        if (res)
                                        {
                                            MessageBox.Show("Đã upload kết quả!");
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        MessageBox.Show("SavePCLImagingResultPDF EndCall: " + ex.Message);
                                    }
                                    finally
                                    {
                                        this.HideBusyIndicator();
                                    }
                                }), null);

                    }
                    catch (Exception ex)
                    {
                        this.HideBusyIndicator();
                        MessageBox.Show("SavePCLImagingResultPDF BeginCall: " + ex.Message);
                    }
                }
            });
            t.Start();
        }
        private bool _IsShowUpload = false;
        public bool IsShowUpload
        {
            get { return _IsShowUpload; }
            set
            {
                if (_IsShowUpload != value)
                {
                    _IsShowUpload = value;
                    NotifyOfPropertyChange(() => IsShowUpload);
                }
            }
        }
        //▼====: #012
        private bool _IsShowBtnHistory = false;
        public bool IsShowBtnHistory
        {
            get { return _IsShowBtnHistory; }
            set
            {
                if (_IsShowBtnHistory != value)
                {
                    _IsShowBtnHistory = value;
                    NotifyOfPropertyChange(() => IsShowBtnHistory);
                }
            }
        }
        //▲====: #012
        //▼====: #013
        #region authorization
        public void authorization()
        {
            if (!Globals.isAccountCheck)
            {
                return;
            }
            mCapNhatKetQua = Globals.PCLDepartment.ObjPCLResultParamImpID.ParamName.ToUpper().Contains(eHCMSResources.G2600_G1_XN.ToUpper()) ?  
                Globals.CheckOperation(Globals.listRefModule, (int)eModules.mCLSLaboratory, (int)eCLSLaboratory.mXetNghiem, 
                                            (int)oCLSLaboratoryEx.mXetNghiem_CapNhatKetQua)
                : true;
        }
        private bool _mCapNhatKetQua = true;
        public bool mCapNhatKetQua
        {
            get
            {
                return _mCapNhatKetQua;
            }
            set
            {
                if (_mCapNhatKetQua == value)
                    return;
                _mCapNhatKetQua = value;
                NotifyOfPropertyChange(() => mCapNhatKetQua);
            }
        }
        #endregion
        private void CheckAuthorbtnUpdate()
        {
            if (!mCapNhatKetQua && Content == UPDATE && ObjPatientPCLImagingResult.PCLExamType.V_PCLMainCategory == (long)AllLookupValues.V_PCLMainCategory.Laboratory)
            {
                IsReadOnly = false;
            }
            else
            {
                IsReadOnly = true;
            }
        }
        //▲====: #013
        //▼====: #016
        private ObservableCollection<Lookup> _V_ReportForm;
        public ObservableCollection<Lookup> V_ReportForm
        {
            get { return _V_ReportForm; }
            set
            {
                _V_ReportForm = value;
                NotifyOfPropertyChange(() => V_ReportForm);
            }
        }
        public void GetLookupV_ReportForm()
        {
            V_ReportForm = new ObservableCollection<Lookup>();
            foreach (var tmpLookup in Globals.AllLookupValueList)
            {
                if (tmpLookup.ObjectTypeID == (long)(LookupValues.V_ReportForm))
                {
                    V_ReportForm.Add(tmpLookup);
                }
            }
        }
        private int GetCountImageFromReportForm()
        {
            if (ObjPatientPCLImagingResult.PatientPCLRequest == null)
            {
                return 0;
            }
            if (ObjPatientPCLImagingResult.PCLExamType == null || ObjPatientPCLImagingResult.PCLExamType.V_ReportForm == 0)
            {
                return 0;
            }
            if (Content == UPDATE)
            {
                return 0;
            }

            return Convert.ToInt32(V_ReportForm.Where(x=>x.LookupID == ObjPatientPCLImagingResult.PCLExamType.V_ReportForm).FirstOrDefault().Code);
        }
        //▲====: #016
    }
}
