using System.ComponentModel.Composition;
using aEMR.ViewContracts;
using Caliburn.Micro;
using aEMR.Infrastructure;
using DataEntities;
using DevExpress.Xpf.Printing;
using aEMR.ReportModel.ReportModels;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using System.Linq;
using DevExpress.ReportServer.Printing;
/*
* 20210109 #001 TNHX:  BM: Thêm report suat an
* 20220910 #002 DatTB: Thêm phiếu công khai thuốc KK
*/
namespace aEMR.StoreDept.Reports.ViewModels
{
    [Export(typeof(IClinicDeptReportDocumentPreview)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class ClinicReportDocumentPreviewViewModel : Conductor<object>, IClinicDeptReportDocumentPreview
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public ClinicReportDocumentPreviewViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;
            eItem = new ReportName();
        }
        private RemoteDocumentSource _reportModel;
        public RemoteDocumentSource ReportModel
        {
            get { return _reportModel; }
            set
            {
                _reportModel = value;
                NotifyOfPropertyChange(() => ReportModel);
            }
        }
        private long _ID = 0;
        public long ID
        {
            get { return _ID; }
            set
            {
                _ID = value;
                NotifyOfPropertyChange(() => ID);
            }
        }
        private long _PaymentID = 0;
        public long PaymentID
        {
            get { return _PaymentID; }
            set
            {
                _PaymentID = value;
                NotifyOfPropertyChange(() => PaymentID);
            }
        }
        private string _StaffFullName = "";
        public string StaffFullName
        {
            get { return _StaffFullName; }
            set
            {
                _StaffFullName = value;
                NotifyOfPropertyChange(() => StaffFullName);
            }
        }

        private string _LyDo = "";
        public string LyDo
        {
            get { return _LyDo; }
            set
            {
                _LyDo = value;
                NotifyOfPropertyChange(() => LyDo);
            }
        }

        private long _PatientID = 0;
        public long PatientID
        {
            get { return _PatientID; }
            set
            {
                _PatientID = value;
                NotifyOfPropertyChange(() => PatientID);
            }
        }

        private long _V_TranRefType = 0;
        public long V_TranRefType
        {
            get { return _V_TranRefType; }
            set
            {
                _V_TranRefType = value;
                NotifyOfPropertyChange(() => V_TranRefType);
            }
        }

        private long _V_MedProductType;
        public long V_MedProductType
        {
            get { return _V_MedProductType; }
            set
            {
                _V_MedProductType = value;
                NotifyOfPropertyChange(() => V_MedProductType);
            }
        }

        private long _IntPtDiagDrInstructionID;
        public long IntPtDiagDrInstructionID
        {
            get { return _IntPtDiagDrInstructionID; }
            set
            {
                _IntPtDiagDrInstructionID = value;
                NotifyOfPropertyChange(() => IntPtDiagDrInstructionID);
            }
        }

        private ReportName _eItem;
        public ReportName eItem
        {
            get
            {
                return _eItem;
            }
            set
            {
                _eItem = value;
                NotifyOfPropertyChange(() => eItem);
            }
        }

        protected override void OnActivate()
        {
            base.OnActivate();

            var TruongKhoaDuoc = Globals.allStaffPositions.Where(x => x.PositionRefID == (int)AllLookupValues.StaffPositions_Enum.TRUONG_KHOA_DUOC && x.IsActive).FirstOrDefault();
            DevExpress.DocumentServices.ServiceModel.DefaultValueParameterContainer rParams = new DevExpress.DocumentServices.ServiceModel.DefaultValueParameterContainer();
            string reportHospitalName = Globals.ServerConfigSection.CommonItems.ReportHospitalName;
            string reportDepartmentOfHealth = Globals.ServerConfigSection.CommonItems.ReportDepartmentOfHealth;
            string reportLogoUrl = Globals.ServerConfigSection.CommonItems.ReportLogoUrl;
            switch (_eItem)
            {
              
                case ReportName.PHARMACY_VISITORPHIEUTHU:
                    ReportModel = null;
                    ReportModel = new PaymentVisistorReportModel().PreviewModel;
                    rParams["param_User"].Value = StaffFullName;
                    rParams["param_PaymentID"].Value =(int)PaymentID;
                    rParams["OutiID"].Value = (int)ID;
                    rParams["parameter_LyDo"].Value = LyDo;
                    rParams["V_TranRefType"].Value = (int)V_TranRefType;
                    break;
                case ReportName.PHARMACY_PHIEUCHI:
                    ReportModel = null;
                    ReportModel = new PaymentBillsReportModel().PreviewModel;
                    rParams["param_User"].Value = StaffFullName;
                    rParams["param_PaymentID"].Value =(int)PaymentID;
                    rParams["OutiID"].Value = (int)ID;
                    rParams["parameter_LyDo"].Value = LyDo;
                    rParams["V_TranRefType"].Value = (int)V_TranRefType;
                    break;
                case ReportName.DRUGDEPT_REQUEST:
                    ReportModel = null;
                    ReportModel = new DrugDeptRequestReportModal().PreviewModel;
                    rParams["RequestID"].Value = (int)ID;
                    rParams["Show"].Value = LyDo;
                    rParams["TruongKhoaDuoc"].Value = TruongKhoaDuoc != null ? TruongKhoaDuoc.FullNameString : "";
                    rParams["parHospitalName"].Value = reportHospitalName;
                    break;
                case ReportName.DRUGDEPT_REQUEST_DETAILS:
                    ReportModel = null;
                    ReportModel = new DrugDeptRequestDetailReportModal().PreviewModel;
                    rParams["RequestID"].Value = (int)ID;
                    rParams["Show"].Value = LyDo;
                    rParams["parHospitalName"].Value = reportHospitalName;
                    break;
                case ReportName.DRUGDEPT_OUTINTERNAL:
                    ReportModel = null;
                    ReportModel = new DrugDeptOutInternalReportModal().PreviewModel;
                    rParams["OutiID"].Value = (int)ID;
                    rParams["Show"].Value = LyDo;
                    rParams["parHospitalName"].Value = reportHospitalName;
                    break;
                case ReportName.CLINICDEPT_OUTWARD_DRUGDEPT:
                    ReportModel = null;
                    ReportModel = new RptOutwardFromClinicDeptToDrugDeptModel().PreviewModel;
                    rParams["OutiID"].Value = (int)ID;
                    rParams["Show"].Value = LyDo;
                    rParams["TruongKhoaDuoc"].Value = TruongKhoaDuoc != null ? TruongKhoaDuoc.FullNameString : "";
                    rParams["parHospitalName"].Value = reportHospitalName;
                    rParams["V_MedProductType"].Value = V_MedProductType;
                    break;
                case ReportName.CLINICDEPT_OUTWARD_PATIENT:
                    ReportModel = null;
                    ReportModel = new RptOutwardFromClinicDeptToPatientModel().PreviewModel;
                    rParams["OutiID"].Value = (int)ID;
                    rParams["Show"].Value = LyDo;
                    rParams["parHospitalName"].Value = reportHospitalName;
                    break;
                case ReportName.DRUGDEPT_OUTADDICTIVE:
                    ReportModel = null;
                    ReportModel = new DrugDeptOutDrugDeptAddictiveReportModal().PreviewModel;
                    rParams["OutiID"].Value = (int)ID;
                    rParams["Show"].Value = LyDo;
                    rParams["parLogoUrl"].Value = reportLogoUrl;
                    break;
                case ReportName.DRUGDEPT_ESTIMATION:
                    ReportModel = null;
                    ReportModel = new DrugDeptEstimationReportModal().PreviewModel;
                    rParams["DrugDeptEstimatePoID"].Value = (int)ID;
                    rParams["Show"].Value = LyDo;
                    break;
                case ReportName.DRUGDEPT_ORDER:
                    ReportModel = null;
                    ReportModel = new DrugDepOrderReportModal().PreviewModel;
                    rParams["ID"].Value = (int)ID;
                    break;
                case ReportName.DRUGDEPT_INWARD_MEDFORCLINIC:
                    ReportModel = null;
                    ReportModel = new DrugDepInwardFromMedToClinicReportModal().PreviewModel;
                    rParams["InvID"].Value = (int)ID;
                    rParams["Show"].Value = LyDo;
                    rParams["parLogoUrl"].Value = reportLogoUrl;
                    rParams["V_MedProductType"].Value = V_MedProductType;
                    break;
                case ReportName.DRUGDEPT_INWARD_MEDDEPTSUPPLIER:
                    ReportModel = null;
                    ReportModel = new DrugDepInwardMedDeptSupplierReportModal().PreviewModel;
                    rParams["InvID"].Value = (int)ID;
                    rParams["Show"].Value = LyDo;
                    break;
                case ReportName.DRUGDEPT_BANGKECHUNGTUTHANHTOAN:
                    ReportModel = null;
                    ReportModel = new DrugDepSupplierPaymentReportModal().PreviewModel;
                    rParams["ID"].Value = (int)ID;
                    rParams["Show"].Value = LyDo;
                    break;
                case ReportName.DRUGDEPT_PHIEUDENGHITHANHTOAN:
                    ReportModel = null;
                    ReportModel = new DrugDepSuggestPaymentReportModal().PreviewModel;
                    rParams["ID"].Value = (int)ID;
                    rParams["Show"].Value = LyDo;
                    break;
                case ReportName.CLINICDEPT_KIEMKE_KHOAPHONG:
                    {
                        switch (V_MedProductType)
                        {
                            case (long)AllLookupValues.MedProductType.THUOC:
                                {
                                    ReportModel = null;
                                    ReportModel = new GenericReportModel("eHCMS.ReportLib.RptDrugDepts.XtraReports.StockTakes.XRptPhieuKiemKeThuoc_ClinicDept").PreviewModel;
                                    rParams["ID"].Value = (int)ID;
                                    rParams["parLogoUrl"].Value = reportLogoUrl;
                                    break;
                                }
                            case (long)AllLookupValues.MedProductType.Y_CU:
                                {
                                    ReportModel = null;
                                    ReportModel = new GenericReportModel("eHCMS.ReportLib.RptDrugDepts.XtraReports.StockTakes.XRptPhieuKiemKeYCu_ClinicDept").PreviewModel;
                                    rParams["ID"].Value = (int)ID;
                                    rParams["parLogoUrl"].Value = reportLogoUrl;
                                    break;
                                }
                            case (long)AllLookupValues.MedProductType.HOA_CHAT:
                                {
                                    ReportModel = null;
                                    ReportModel = new GenericReportModel("eHCMS.ReportLib.RptDrugDepts.XtraReports.StockTakes.XRptPhieuKiemKeHoaChat_ClinicDept").PreviewModel;
                                    rParams["ID"].Value = (int)ID;
                                    rParams["parLogoUrl"].Value = reportLogoUrl;
                                    break;
                                }
                        }
                        break;
                    }
                case ReportName.DrugDept_Request_HIStore:
                    ReportModel = null;
                    ReportModel = new DrugDeptRequesHIStoretReportModel().PreviewModel;
                    rParams["RequestID"].Value = (int)ID;
                    rParams["Show"].Value = LyDo;
                    rParams["TruongKhoaDuoc"].Value = TruongKhoaDuoc != null ? TruongKhoaDuoc.FullNameString : "";
                    rParams["parHospitalName"].Value = reportHospitalName;
                    break;
                case ReportName.DrugDept_Request_HIStore_Details:
                    ReportModel = null;
                    ReportModel = new DrugDeptRequesHIStoretDetailsReportModel().PreviewModel;
                    rParams["RequestID"].Value = (int)ID;
                    rParams["Show"].Value = LyDo;
                    rParams["TruongKhoaDuoc"].Value = TruongKhoaDuoc != null ? TruongKhoaDuoc.FullNameString : "";
                    rParams["parHospitalName"].Value = reportHospitalName;
                    break;
                //▼====== 20190110 TTM: Bổ sung phiếu công khai thuốc
                case ReportName.REPORT_PHIEU_CONG_KHAI_THUOC:
                    ReportModel = null;
                    ReportModel = new XRptFromClinicDeptToPatientDetails().PreviewModel;
                    rParams["OutiID"].Value = (int)ID;
                    rParams["Show"].Value = LyDo;
                    rParams["parHospitalName"].Value = reportHospitalName;
                    break;
                //▲====== 
                //▼====== 20201110 TNHX: Bổ sung phiếu sao thuốc
                case ReportName.XRptPhieuSaoThuoc:
                    ReportModel = null;
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.RptDrugDepts.XtraReports.Outward.XRptPhieuSaoThuoc").PreviewModel;
                    rParams["OutiID"].Value = (int)ID;
                    rParams["parIntPtDiagDrInstructionID"].Value = IntPtDiagDrInstructionID;
                    rParams["V_MedProductType"].Value = V_MedProductType;
                    rParams["Show"].Value = LyDo;
                    rParams["parHospitalName"].Value = reportHospitalName;
                    break;
                //▲====== 
                //▼==== #002
                case ReportName.KK_PhieuCongKhaiThuoc:
                    ReportModel = null;
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.RptDrugDepts.XtraReports.Outward.XRptKKPhieuCongKhaiThuoc").PreviewModel;
                    rParams["parIntPtDiagDrInstructionID"].Value = IntPtDiagDrInstructionID;
                    rParams["Show"].Value = LyDo;
                    rParams["parHospitalName"].Value = reportHospitalName;
                    break;
                //▲==== #002
                //▼====== 20201213 TNHX: Bổ sung phiếu truyền máu
                case ReportName.PHIEU_TRUYEN_MAU:
                    ReportModel = null;
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.RptStoreDept.XtraReports.Outward.XRptPhieuTruyenMau").PreviewModel;
                    rParams["OutiID"].Value = ID;
                    rParams["parHospitalName"].Value = reportHospitalName;
                    rParams["parDepartmentOfHealth"].Value = reportDepartmentOfHealth;
                    break;
                //▲====== 
                //▼====: #001
                case ReportName.XRptClinicDeptRequestFood:
                    ReportModel = null;
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.RptStoreDept.XtraReports.Request.XRptFoodRequestDrugDept").PreviewModel;
                    rParams["RequestID"].Value = (int)ID;
                    rParams["parHospitalName"].Value = reportHospitalName;
                    break;
                //▲====: #001
                case ReportName.DRUGDEPT_REQUEST_FOR_TECHNICALSERVICE:
                    ReportModel = null;
                    ReportModel = new DrugDeptRequestForTechnicalServiceReportModal().PreviewModel;
                    rParams["RequestID"].Value = (int)ID;
                    rParams["Show"].Value = LyDo;
                    rParams["TruongKhoaDuoc"].Value = TruongKhoaDuoc != null ? TruongKhoaDuoc.FullNameString : "";
                    rParams["parHospitalName"].Value = reportHospitalName;
                    break;
            }

            // ReportModel.AutoShowParametersPanel = false;
            ReportModel.CreateDocument(rParams);
        }
    }
}
