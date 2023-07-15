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
using eHCMSLanguage;
using DevExpress.ReportServer.Printing;
/*
 * 20181226 #001 TTM: Bổ sung report cho phiếu yêu cầu kho BHYT - Nhà thuốc.
 * 20190620 #002 TNHX: [BM0011869] Them report phieu nhap tra tu kho khoa phong (mau, hoa chat, tiem ngua, VTYTTH, thanh trung)
 * 20200623 #003 TNHX: [BM] Them report BienBanGiaoNhanVaccine
 * 20230630 #004 QTD:  Thêm báo cáo dự trù tổng hợp theo khoa/phòng
 */
namespace aEMR.DrugDept.ViewModels
{
    [Export(typeof(IDrugDeptReportDocumentPreview)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class ReportDocumentPreviewViewModel : Conductor<object>, IDrugDeptReportDocumentPreview
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public ReportDocumentPreviewViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
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

        private bool _IsLiquidation = false;
        public bool IsLiquidation
        {
            get { return _IsLiquidation; }
            set
            {
                _IsLiquidation = value;
                NotifyOfPropertyChange(() => IsLiquidation);
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

        private long _estimationCodeBegin;
        public long EstimationCodeBegin
        {
            get
            {
                return _estimationCodeBegin;
            }
            set
            {
                _estimationCodeBegin = value;
                NotifyOfPropertyChange(() => EstimationCodeBegin);
            }
        }


        private long _estimationCodeEnd;
        public long EstimationCodeEnd
        {
            get
            {
                return _estimationCodeEnd;
            }
            set
            {
                _estimationCodeEnd = value;
                NotifyOfPropertyChange(() => EstimationCodeEnd);
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

        private long _StoreID;
        public long StoreID
        {
            get { return _StoreID; }
            set
            {
                _StoreID = value;
                NotifyOfPropertyChange(() => StoreID);
            }
        }

        private string _TitleRpt;
        public string TitleRpt
        {
            get { return _TitleRpt; }
            set
            {
                _TitleRpt = value;
                NotifyOfPropertyChange(() => TitleRpt);
            }
        }


        private string _TitleRpt1;
        public string TitleRpt1
        {
            get { return _TitleRpt1; }
            set
            {
                _TitleRpt1 = value;
                NotifyOfPropertyChange(() => TitleRpt1);
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
            string DeptDirectorSignTitle = string.Empty;
            string DeptStatsSignTitle = string.Empty;
            var GiamDoc = Globals.allStaffPositions.Where(x => x.PositionRefID == (int)AllLookupValues.StaffPositions_Enum.GIAM_DOC && x.IsActive).FirstOrDefault();
            var PhoGiamDoc = Globals.allStaffPositions.Where(x => x.PositionRefID == (int)AllLookupValues.StaffPositions_Enum.PHO_GIAM_DOC && x.IsActive).FirstOrDefault();
            var KeToanTruong = Globals.allStaffPositions.Where(x => x.PositionRefID == (int)AllLookupValues.StaffPositions_Enum.KE_TOAN_TRUONG && x.IsActive).FirstOrDefault();
            var TruongKhoaDuoc = Globals.allStaffPositions.Where(x => x.PositionRefID == (int)AllLookupValues.StaffPositions_Enum.TRUONG_KHOA_DUOC && x.IsActive).FirstOrDefault();
            var CurrentDrugDeptModule = Globals.GetViewModel<IDrugModule>();
            if (Globals.ServerConfigSection.MedDeptElements.UseDrugDeptAs2DistinctParts &&
                CurrentDrugDeptModule != null && Globals.AllPositionInHospital != null && CurrentDrugDeptModule.MenuVisibleCollection[1] &&
                Globals.AllPositionInHospital.Any(x => x.PositionRefID == (int)AllLookupValues.StaffPositions_Enum.TRUONG_KHOA_VATTU))
            {
                DeptDirectorSignTitle = Globals.AllPositionInHospital.First(x => x.PositionRefID == (int)AllLookupValues.StaffPositions_Enum.TRUONG_KHOA_VATTU).PositionName;
                if (Globals.AllPositionInHospital.Any(x => x.PositionRefID == (int)AllLookupValues.StaffPositions_Enum.THONG_KE_VATTU))
                {
                    DeptStatsSignTitle = Globals.AllPositionInHospital.First(x => x.PositionRefID == (int)AllLookupValues.StaffPositions_Enum.THONG_KE_VATTU).PositionName;
                }
                TruongKhoaDuoc = Globals.allStaffPositions.Where(x => x.PositionRefID == (int)AllLookupValues.StaffPositions_Enum.TRUONG_KHOA_VATTU && x.IsActive).FirstOrDefault();
            }
            var ThuKho = new StaffPosition();
            var ThongKeDuoc = new StaffPosition();
            if (V_MedProductType == (long)AllLookupValues.MedProductType.THUOC)
            {
                ThuKho = Globals.allStaffPositions.Where(x => x.PositionRefID == (int)AllLookupValues.StaffPositions_Enum.THU_KHO_THUOC_KHOA_dUOC && x.IsActive).FirstOrDefault();

                ThongKeDuoc = Globals.allStaffPositions.Where(x => x.PositionRefID == (int)AllLookupValues.StaffPositions_Enum.THONG_KE_DUOC_THUOC && x.IsActive).FirstOrDefault();
            }
            else if (V_MedProductType == (long)AllLookupValues.MedProductType.Y_CU)
            {
                ThuKho = Globals.allStaffPositions.Where(x => x.PositionRefID == (int)AllLookupValues.StaffPositions_Enum.THU_KHO_Y_CU && x.IsActive).FirstOrDefault();
                ThongKeDuoc = Globals.allStaffPositions.Where(x => x.PositionRefID == (int)AllLookupValues.StaffPositions_Enum.THONG_KE_DUOC_YCU && x.IsActive).FirstOrDefault();
            }
            else
            {
                ThuKho = Globals.allStaffPositions.Where(x => x.PositionRefID == (int)AllLookupValues.StaffPositions_Enum.THU_KHO_HOA_CHAT && x.IsActive).FirstOrDefault();
                ThongKeDuoc = Globals.allStaffPositions.Where(x => x.PositionRefID == (int)AllLookupValues.StaffPositions_Enum.THONG_KE_DUOC_HOACHAT && x.IsActive).FirstOrDefault();
            }
            //var ThongKeDuoc = Globals.allStaffPositions.Where(x => x.PositionRefID == (int)AllLookupValues.StaffPositions_Enum.THONG_KE_DUOC && x.IsActive).FirstOrDefault();
            var TPKeHoach = Globals.allStaffPositions.Where(x => x.PositionRefID == (int)AllLookupValues.StaffPositions_Enum.TP_KE_HOACH && x.IsActive).FirstOrDefault();
            DevExpress.DocumentServices.ServiceModel.DefaultValueParameterContainer rParams = new DevExpress.DocumentServices.ServiceModel.DefaultValueParameterContainer();
            string reportHospitalName = Globals.ServerConfigSection.CommonItems.ReportHospitalName;
            string reportLogoUrl = Globals.ServerConfigSection.CommonItems.ReportLogoUrl;
            string reportDepartmentOfHealth = Globals.ServerConfigSection.CommonItems.ReportDepartmentOfHealth;
            string reportHospitalAddress = Globals.ServerConfigSection.CommonItems.ReportHospitalAddress;
            string hospitalCode = Globals.ServerConfigSection.Hospitals.HospitalCode;
            base.OnActivate();
            switch (_eItem)
            {
                case ReportName.PHARMACY_VISITORPHIEUTHU:
                    ReportModel = null;
                    ReportModel = new PaymentVisistorReportModel().PreviewModel;
                    rParams["param_User"].Value = StaffFullName;
                    rParams["param_PaymentID"].Value = (int)PaymentID;
                    rParams["OutiID"].Value = (int)ID;
                    rParams["parameter_LyDo"].Value = LyDo;
                    rParams["V_TranRefType"].Value = (int)V_TranRefType;
                    break;
                case ReportName.PHARMACY_PHIEUCHI:
                    ReportModel = null;
                    ReportModel = new PaymentBillsReportModel().PreviewModel;
                    rParams["param_User"].Value = StaffFullName;
                    rParams["param_PaymentID"].Value = (int)PaymentID;
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
                    rParams["TruongKhoaDuoc"].Value = TruongKhoaDuoc != null ? TruongKhoaDuoc.FullNameString : "";
                    rParams["parHospitalName"].Value = reportHospitalName;
                    break;
                case ReportName.DRUGDEPT_REQUEST_APPROVED:
                    ReportModel = null;
                    ReportModel = new DrugDeptRequestApprovedReportModal().PreviewModel;
                    rParams["RequestID"].Value = (int)ID;
                    rParams["Show"].Value = LyDo;
                    rParams["TruongKhoaDuoc"].Value = TruongKhoaDuoc != null ? TruongKhoaDuoc.FullNameString : "";
                    rParams["parHospitalName"].Value = reportHospitalName;
                    if (!string.IsNullOrEmpty(DeptDirectorSignTitle))
                    {
                        rParams["pDeptDirectorSignTitle"].Value = DeptDirectorSignTitle == null ? "" : DeptDirectorSignTitle;
                    }
                    break;
                case ReportName.DRUGDEPT_REQUEST_DETAILS_APPROVED:
                    ReportModel = null;
                    ReportModel = new DrugDeptRequestDetailApprovedReportModal().PreviewModel;
                    rParams["RequestID"].Value = (int)ID;
                    rParams["Show"].Value = LyDo;
                    rParams["TruongKhoaDuoc"].Value = TruongKhoaDuoc != null ? TruongKhoaDuoc.FullNameString : "";
                    rParams["parHospitalName"].Value = reportHospitalName;
                    if (!string.IsNullOrEmpty(DeptDirectorSignTitle))
                    {
                        rParams["pDeptDirectorSignTitle"].Value = DeptDirectorSignTitle == null ? "" : DeptDirectorSignTitle;
                    }
                    break;
                case ReportName.DRUGDEPT_OUTINTERNAL:
                    ReportModel = null;
                    ReportModel = new DrugDeptOutInternalReportModal().PreviewModel;
                    rParams["OutiID"].Value = (int)ID;
                    rParams["V_MedProductType"].Value = (int)V_MedProductType;
                    rParams["Show"].Value = LyDo;
                    rParams["GiamDoc"].Value = GiamDoc != null ? GiamDoc.FullNameString : "";
                    rParams["TruongKhoaDuoc"].Value = TruongKhoaDuoc != null ? TruongKhoaDuoc.FullNameString : "";
                    rParams["ThongKeDuoc"].Value = ThongKeDuoc != null ? ThongKeDuoc.FullNameString : "";
                    rParams["parHospitalName"].Value = reportHospitalName;
                    break;
                case ReportName.DRUGDEPT_OUTINTERNAL_TOCLINICDEPT:
                    ReportModel = null;
                    if (V_MedProductType == (long)AllLookupValues.MedProductType.VTYT_TIEUHAO)
                    {
                        ReportModel = new DrugDeptOutInternalToClinicDeptReportModal("eHCMS.ReportLib.RptDrugDepts.XtraReports.OutInternal.XRptOutVTYTTHDrugDeptToClinicDept").PreviewModel;
                        rParams["parHospitalAddress"].Value = reportHospitalAddress;
                    }
                    else if (V_MedProductType == (long)AllLookupValues.MedProductType.TIEM_NGUA)
                    {
                        ReportModel = new DrugDeptOutInternalToClinicDeptReportModal("eHCMS.ReportLib.RptDrugDepts.XtraReports.OutInternal.XRptOutVaccineDrugDeptToClinicDept").PreviewModel;
                        rParams["parHospitalAddress"].Value = reportHospitalAddress;
                    }
                    else if (V_MedProductType == (long)AllLookupValues.MedProductType.HOA_CHAT)
                    {
                        ReportModel = new DrugDeptOutInternalToClinicDeptReportModal("eHCMS.ReportLib.RptDrugDepts.XtraReports.OutInternal.XRptOutChemicalDrugDeptToClinicDept").PreviewModel;
                        rParams["parHospitalAddress"].Value = reportHospitalAddress;
                    }
                    else if (V_MedProductType == (long)AllLookupValues.MedProductType.MAU)
                    {
                        ReportModel = new DrugDeptOutInternalToClinicDeptReportModal("eHCMS.ReportLib.RptDrugDepts.XtraReports.OutInternal.XRptOutBloodDrugDeptToClinicDept").PreviewModel;
                        rParams["parHospitalAddress"].Value = reportHospitalAddress;
                    }
                    else if (V_MedProductType == (long)AllLookupValues.MedProductType.THANHTRUNG)
                    {
                        ReportModel = new DrugDeptOutInternalToClinicDeptReportModal("eHCMS.ReportLib.RptDrugDepts.XtraReports.OutInternal.XRptOutThanhTrungDrugDeptToClinicDept").PreviewModel;
                        rParams["parHospitalAddress"].Value = reportHospitalAddress;
                    }
                    else if (V_MedProductType == (long)AllLookupValues.MedProductType.VAN_PHONG_PHAM)
                    {
                        ReportModel = new DrugDeptOutInternalToClinicDeptReportModal("eHCMS.ReportLib.RptDrugDepts.XtraReports.OutInternal.XRptOutVPPDrugDeptToClinicDept").PreviewModel;
                    }
                    else if (V_MedProductType == (long)AllLookupValues.MedProductType.VATTU_TIEUHAO)
                    {
                        ReportModel = new DrugDeptOutInternalToClinicDeptReportModal("eHCMS.ReportLib.RptDrugDepts.XtraReports.OutInternal.XRptOutVTTHDrugDeptToClinicDept").PreviewModel;
                    }
                    else
                    {
                        ReportModel = new DrugDeptOutInternalToClinicDeptReportModal("eHCMS.ReportLib.RptDrugDepts.XtraReports.OutInternal.XRptOutInternalDrugDeptToClinicDept").PreviewModel;
                        rParams["parHospitalAddress"].Value = reportHospitalAddress;
                    }
                    rParams["OutiID"].Value = (int)ID;
                    rParams["V_MedProductType"].Value = (int)V_MedProductType;
                    rParams["Show"].Value = LyDo;
                    rParams["GiamDoc"].Value = GiamDoc != null ? GiamDoc.FullNameString : "";
                    rParams["TruongKhoaDuoc"].Value = TruongKhoaDuoc != null ? TruongKhoaDuoc.FullNameString : "";
                    rParams["parHospitalName"].Value = reportHospitalName;
                    rParams["parIsLiquidation"].Value = IsLiquidation;
                    break;
                case ReportName.XRptRequestDrugDeptDetailsGroupByPatient:
                    ReportModel = null;
                    ReportModel = new DrugDeptOutInternalToClinicDeptReportModal("eHCMS.ReportLib.RptDrugDepts.XtraReports.Request.XRptRequestDrugDeptDetailsGroupByPatient").PreviewModel;
                    rParams["parHospitalAddress"].Value = reportHospitalAddress;
                    rParams["OutiID"].Value = (int)ID;
                    rParams["V_MedProductType"].Value = (int)V_MedProductType;
                    rParams["Show"].Value = LyDo;
                    rParams["GiamDoc"].Value = GiamDoc != null ? GiamDoc.FullNameString : "";
                    rParams["TruongKhoaDuoc"].Value = TruongKhoaDuoc != null ? TruongKhoaDuoc.FullNameString : "";
                    rParams["parHospitalName"].Value = reportHospitalName;
                    rParams["parIsLiquidation"].Value = IsLiquidation;
                    break;
                case ReportName.DRUGDEPT_HUYTHUOC:
                    ReportModel = null;
                    ReportModel = new DrugDeptOutDemageReportModal().PreviewModel;
                    rParams["OutiID"].Value = (int)ID;
                    rParams["V_MedProductType"].Value = (int)V_MedProductType;
                    rParams["Show"].Value = LyDo;
                    rParams["GiamDoc"].Value = GiamDoc != null ? GiamDoc.FullNameString : "";
                    rParams["TruongKhoaDuoc"].Value = TruongKhoaDuoc != null ? TruongKhoaDuoc.FullNameString : "";
                    rParams["ThongKeDuoc"].Value = ThongKeDuoc != null ? ThongKeDuoc.FullNameString : "";
                    rParams["parLogoUrl"].Value = reportLogoUrl;
                    break;
                case ReportName.DRUGDEPT_OUTADDICTIVE:
                    ReportModel = null;
                    ReportModel = new DrugDeptOutDrugDeptAddictiveReportModal().PreviewModel;
                    rParams["OutiID"].Value = (int)ID;
                    rParams["V_MedProductType"].Value = (int)V_MedProductType;
                    rParams["Show"].Value = LyDo;
                    rParams["GiamDoc"].Value = GiamDoc != null ? GiamDoc.FullNameString : "";
                    rParams["TruongKhoaDuoc"].Value = TruongKhoaDuoc != null ? TruongKhoaDuoc.FullNameString : "";
                    rParams["parLogoUrl"].Value = reportLogoUrl;
                    break;
                case ReportName.DRUGDEPT_ESTIMATION:
                    ReportModel = null;
                    //ReportModel = new DrugDeptEstimationReportModal().PreviewModel;
                    ReportModel = EstimationFromRequest ? new DrugDeptEstimationFromRequestReportModal().PreviewModel : new DrugDeptEstimationReportModal_V2().PreviewModel;
                    rParams["DrugDeptEstimatePoID"].Value = (int)ID;
                    rParams["EstimationCodeBegin"].Value = (int)EstimationCodeBegin;
                    rParams["EstimationCodeEnd"].Value = (int)EstimationCodeEnd;
                    rParams["V_MedProductType"].Value = (int)V_MedProductType;
                    rParams["EstimationFromRequest"].Value = EstimationFromRequest;
                    rParams["Show"].Value = LyDo;
                    rParams["GiamDoc"].Value = GiamDoc != null ? GiamDoc.FullNameString : "";
                    rParams["KeToanTruong"].Value = KeToanTruong != null ? KeToanTruong.FullNameString : "";
                    rParams["TPKeHoach"].Value = TPKeHoach != null ? TPKeHoach.FullNameString : "";
                    rParams["TruongKhoaDuoc"].Value = TruongKhoaDuoc != null ? TruongKhoaDuoc.FullNameString : "";
                    if (!string.IsNullOrEmpty(DeptDirectorSignTitle))
                    {
                        rParams["pDeptDirectorSignTitle"].Value = DeptDirectorSignTitle == null ? "" : DeptDirectorSignTitle.ToUpper();
                    }
                    if (!string.IsNullOrEmpty(DeptStatsSignTitle))
                    {
                        rParams["pDeptStatsSignTitle"].Value = DeptStatsSignTitle == null ? "" : DeptStatsSignTitle.ToUpper();
                    }
                    rParams["ThongKeDuoc"].Value = ThongKeDuoc != null ? ThongKeDuoc.FullNameString : "";
                    rParams["parLogoUrl"].Value = reportLogoUrl;
                    rParams["userStaffFullNameExportExcel"].Value = StaffFullName ?? "";
                    if(EstimationFromRequest)
                    {
                        rParams["parHospitalName"].Value = reportHospitalName;
                    }
                    break;
                case ReportName.DRUGDEPT_ESTIMATIONKETOAN:
                    ReportModel = null;
                    ReportModel = new DrugDeptEstimationKeToanReportModal().PreviewModel;
                    rParams["DrugDeptEstimatePoID"].Value = (int)ID;
                    rParams["Show"].Value = LyDo;
                    rParams["TruongKhoaDuoc"].Value = TruongKhoaDuoc != null ? TruongKhoaDuoc.FullNameString : "";
                    if (!string.IsNullOrEmpty(DeptDirectorSignTitle))
                    {
                        rParams["pDeptDirectorSignTitle"].Value = DeptDirectorSignTitle == null ? "" : DeptDirectorSignTitle.ToUpper();
                    }
                    if (!string.IsNullOrEmpty(DeptStatsSignTitle))
                    {
                        rParams["pDeptStatsSignTitle"].Value = DeptStatsSignTitle == null ? "" : DeptStatsSignTitle.ToUpper();
                    }
                    rParams["ThongKeDuoc"].Value = ThongKeDuoc != null ? ThongKeDuoc.FullNameString : "";
                    rParams["parLogoUrl"].Value = reportLogoUrl;
                    break;
                case ReportName.DRUGDEPT_ESTIMATIONTHUKHO:
                    ReportModel = null;
                    ReportModel = new DrugDeptEstimationThuKhoReportModal().PreviewModel;
                    rParams["DrugDeptEstimatePoID"].Value = (int)ID;
                    rParams["Show"].Value = LyDo;
                    rParams["TruongKhoaDuoc"].Value = TruongKhoaDuoc != null ? TruongKhoaDuoc.FullNameString : "";
                    if (!string.IsNullOrEmpty(DeptDirectorSignTitle))
                    {
                        rParams["pDeptDirectorSignTitle"].Value = DeptDirectorSignTitle == null ? "" : DeptDirectorSignTitle.ToUpper();
                    }
                    if (!string.IsNullOrEmpty(DeptStatsSignTitle))
                    {
                        rParams["pDeptStatsSignTitle"].Value = DeptStatsSignTitle == null ? "" : DeptStatsSignTitle.ToUpper();
                    }
                    rParams["ThongKeDuoc"].Value = ThongKeDuoc != null ? ThongKeDuoc.FullNameString : "";
                    rParams["parLogoUrl"].Value = reportLogoUrl;
                    break;
                case ReportName.DRUGDEPT_ORDER:
                    ReportModel = null;
                    ReportModel = new DrugDepOrderReportModal().PreviewModel;
                    rParams["ID"].Value = (int)ID;
                    rParams["GiamDoc"].Value = GiamDoc != null ? GiamDoc.FullNameString : "";
                    rParams["parLogoUrl"].Value = reportLogoUrl;
                    break;
                case ReportName.DRUGDEPT_INWARD_MEDFORCLINIC:
                    ReportModel = null;
                    ReportModel = new DrugDepInwardFromMedToClinicReportModal().PreviewModel;
                    rParams["InvID"].Value = (int)ID;
                    rParams["Show"].Value = LyDo;
                    rParams["GiamDoc"].Value = GiamDoc != null ? GiamDoc.FullNameString : "";
                    rParams["KeToanTruong"].Value = KeToanTruong != null ? KeToanTruong.FullNameString : "";
                    rParams["parLogoUrl"].Value = reportLogoUrl;
                    break;
                case ReportName.DRUGDEPT_INWARD_MEDDEPTFROMCLINICDEPT:
                    //▼====: #002
                    switch (V_MedProductType)
                    {
                        case (long)AllLookupValues.MedProductType.MAU:
                            ReportModel = null;
                            ReportModel = new DrugDepInwardBloodMedDeptFromClinicReportModal().PreviewModel;
                            break;
                        case (long)AllLookupValues.MedProductType.THANHTRUNG:
                            ReportModel = null;
                            ReportModel = new DrugDepInwardThanhTrungMedDeptFromClinicReportModal().PreviewModel;
                            break;
                        case (long)AllLookupValues.MedProductType.TIEM_NGUA:
                            ReportModel = null;
                            ReportModel = new DrugDepInwardTiemNguaMedDeptFromClinicReportModal().PreviewModel;
                            break;
                        case (long)AllLookupValues.MedProductType.VTYT_TIEUHAO:
                            ReportModel = null;
                            ReportModel = new DrugDepInwardVTYTTHMedDeptFromClinicReportModal().PreviewModel;
                            break;
                        case (long)AllLookupValues.MedProductType.HOA_CHAT:
                            ReportModel = null;
                            ReportModel = new DrugDepInwardChemicalMedDeptFromClinicReportModal().PreviewModel;
                            break;
                        default:
                            ReportModel = null;
                            ReportModel = new DrugDepInwardMedDeptFromClinicReportModal().PreviewModel;
                            break;
                    }
                    //▲====: #002
                    rParams["InvID"].Value = (int)ID;
                    rParams["Show"].Value = LyDo;
                    if (Globals.ServerConfigSection.Hospitals.IsDirectorSignature)
                    {
                        rParams["ChucVu"].Value = eHCMSResources.Z0947_G1_ThuTruongDVi;
                        rParams["GiamDocHoacPhoGiamDoc"].Value = GiamDoc != null ? GiamDoc.FullNameString : "";
                    }
                    else
                    {
                        rParams["ChucVu"].Value = eHCMSResources.Z0948_G1_PhoThuTruongDVi;
                        rParams["GiamDocHoacPhoGiamDoc"].Value = PhoGiamDoc != null ? PhoGiamDoc.FullNameString : "";
                    }
                    rParams["KeToanTruong"].Value = KeToanTruong != null ? KeToanTruong.FullNameString : "";
                    rParams["ThuKho"].Value = ThuKho != null && ThuKho.StaffPositionID > 0 ? ThuKho.FullNameString : "";
                    rParams["TruongKhoaDuoc"].Value = TruongKhoaDuoc != null ? TruongKhoaDuoc.FullNameString : "";
                    rParams["ThongKeDuoc"].Value = ThongKeDuoc != null ? ThongKeDuoc.FullNameString : "";
                    rParams["parHospitalName"].Value = reportHospitalName;
                    rParams["parDepartmentOfHealth"].Value = reportDepartmentOfHealth;
                    if (hospitalCode == "95076" || hospitalCode == "95078")
                    {
                        rParams["ViewerFullName"].Value = Globals.AllStaffs.Where(x => x.StaffID == (int)Globals.LoggedUserAccount.StaffID).FirstOrDefault().FullName.ToString();
                    }
                    break;
                case ReportName.DRUGDEPT_INWARD_MEDDEPTSUPPLIER:
                    ReportModel = null;
                    ReportModel = new DrugDepInwardMedDeptSupplierReportModal().PreviewModel;
                    rParams["InvID"].Value = (int)ID;
                    rParams["Show"].Value = LyDo;
                    //KMx: Kể từ ngày 1/9/2015 người ký tên phiếu nhập thuốc, y cụ, hóa chất là Phó Giám Đốc (09/09/2015 16:50).
                    //rParams["GiamDoc"].Value = GiamDoc != null ? GiamDoc.FullNameString : "";
                    if (Globals.ServerConfigSection.Hospitals.IsDirectorSignature)
                    {
                        rParams["ChucVu"].Value = eHCMSResources.Z0947_G1_ThuTruongDVi;
                        rParams["GiamDocHoacPhoGiamDoc"].Value = GiamDoc != null ? GiamDoc.FullNameString : "";
                    }
                    else
                    {
                        rParams["ChucVu"].Value = eHCMSResources.Z0948_G1_PhoThuTruongDVi;
                        rParams["GiamDocHoacPhoGiamDoc"].Value = PhoGiamDoc != null ? PhoGiamDoc.FullNameString : "";
                    }
                    rParams["KeToanTruong"].Value = KeToanTruong != null ? KeToanTruong.FullNameString : "";
                    rParams["ThuKho"].Value = ThuKho != null && ThuKho.StaffPositionID > 0 ? ThuKho.FullNameString : "";
                    rParams["TruongKhoaDuoc"].Value = TruongKhoaDuoc != null ? TruongKhoaDuoc.FullNameString : "";
                    rParams["ThongKeDuoc"].Value = ThongKeDuoc != null ? ThongKeDuoc.FullNameString : "";
                    rParams["parHospitalAddress"].Value = reportHospitalAddress;
                    rParams["parHospitalName"].Value = reportHospitalName;
                    rParams["parStaffName"].Value = StaffFullName ?? "";
                    break;
                case ReportName.DRUGDEPT_INWARD_MEDDEPTSUPPLIER_TRONGNUOC:
                    ReportModel = null;
                    ReportModel = new DrugDepInwardMedDeptSupplierTrongNuocReportModal().PreviewModel;
                    rParams["InvID"].Value = (int)ID;
                    rParams["Show"].Value = LyDo;
                    //KMx: Kể từ ngày 1/9/2015 người ký tên là Phó Giám Đốc (09/09/2015 16:50).

                    //rParams["GiamDoc"].Value = GiamDoc != null ? GiamDoc.FullNameString : "";
                    if (Globals.ServerConfigSection.Hospitals.IsDirectorSignature)
                    {
                        rParams["ChucVu"].Value = eHCMSResources.Z0947_G1_ThuTruongDVi;
                        rParams["GiamDocHoacPhoGiamDoc"].Value = GiamDoc != null ? GiamDoc.FullNameString : "";
                    }
                    else
                    {
                        rParams["ChucVu"].Value = eHCMSResources.Z0948_G1_PhoThuTruongDVi;
                        rParams["GiamDocHoacPhoGiamDoc"].Value = PhoGiamDoc != null ? PhoGiamDoc.FullNameString : "";
                    }
                    rParams["KeToanTruong"].Value = KeToanTruong != null ? KeToanTruong.FullNameString : "";
                    rParams["ThuKho"].Value = ThuKho != null && ThuKho.StaffPositionID > 0 ? ThuKho.FullNameString : "";
                    rParams["TruongKhoaDuoc"].Value = TruongKhoaDuoc != null ? TruongKhoaDuoc.FullNameString : "";
                    rParams["ThongKeDuoc"].Value = ThongKeDuoc != null ? ThongKeDuoc.FullNameString : "";
                    rParams["parHospitalAddress"].Value = reportHospitalAddress;
                    rParams["parHospitalName"].Value = reportHospitalName;
                    rParams["parHospitalAddress"].Value = reportHospitalAddress;
                    rParams["parStaffName"].Value = StaffFullName ?? "";
                    break;
                case ReportName.DRUGDEPT_INWARD_NHAPCHIPHI:
                    ReportModel = null;
                    ReportModel = new DrugDepInwardCostTableReportModal().PreviewModel;
                    rParams["InvID"].Value = (int)ID;
                    rParams["GiamDoc"].Value = GiamDoc != null ? GiamDoc.FullNameString : "";
                    rParams["KeToanTruong"].Value = KeToanTruong != null ? KeToanTruong.FullNameString : "";
                    rParams["ThuKho"].Value = ThuKho != null && ThuKho.StaffPositionID > 0 ? ThuKho.FullNameString : "";
                    rParams["TruongKhoaDuoc"].Value = TruongKhoaDuoc != null ? TruongKhoaDuoc.FullNameString : "";
                    rParams["ThongKeDuoc"].Value = ThongKeDuoc != null ? ThongKeDuoc.FullNameString : "";
                    break;
                case ReportName.DRUGDEPT_BANGKECHUNGTUTHANHTOAN:
                    ReportModel = null;
                    ReportModel = new DrugDepSupplierPaymentReportModal().PreviewModel;
                    rParams["ID"].Value = (int)ID;
                    rParams["Show"].Value = LyDo;
                    rParams["GiamDoc"].Value = GiamDoc != null ? GiamDoc.FullNameString : "";
                    rParams["KeToanTruong"].Value = KeToanTruong != null ? KeToanTruong.FullNameString : "";
                    rParams["TruongKhoaDuoc"].Value = TruongKhoaDuoc != null ? TruongKhoaDuoc.FullNameString : "";
                    break;
                case ReportName.DRUGDEPT_PHIEUDENGHITHANHTOAN:
                    ReportModel = null;
                    ReportModel = new DrugDepSuggestPaymentReportModal().PreviewModel;
                    rParams["ID"].Value = (int)ID;
                    rParams["Show"].Value = LyDo;
                    rParams["GiamDoc"].Value = GiamDoc != null ? GiamDoc.FullNameString : "";
                    rParams["KeToanTruong"].Value = KeToanTruong != null ? KeToanTruong.FullNameString : "";
                    rParams["TruongKhoaDuoc"].Value = TruongKhoaDuoc != null ? TruongKhoaDuoc.FullNameString : "";
                    break;
                case ReportName.DRUGDEPT_RETURN_MEDDEPT:
                    ReportModel = null;
                    ReportModel = new DrugDeptReturnMedDeptReportModal().PreviewModel;
                    rParams["OutiID"].Value = (int)ID;
                    rParams["Show"].Value = LyDo;
                    rParams["parLogoUrl"].Value = reportLogoUrl;
                    break;
                case ReportName.MEDDEPT_OUTWARD_FROM_PRESCRIPTION:
                    ReportModel = null;
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.RptDrugDepts.XtraReports.OutwardFromPrescription.XRptOutwardFromPrescription").PreviewModel;
                    rParams["OutiID"].Value = (int)ID;
                    rParams["Show"].Value = LyDo;
                    rParams["parHospitalName"].Value = reportHospitalName;
                    break;
                case ReportName.MEDDEPT_PRINT_BILL:
                    ReportModel = null;
                    //ReportModel = new GenericReportModel("eHCMS.ReportLib.RptPatientRegistration.XRpt_InPatientBillingInvoice").PreviewModel;
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.RptPatientRegistration.XRpt_InPatientBillingInvoiceNew").PreviewModel;
                    rParams["InPatientBillingInvID"].Value = (int)ID;
                    break;
                case ReportName.RptDrugDeptStockTakes_ThuocYCuHoaChatKhoaDuoc:
                    {
                        switch (V_MedProductType)
                        {
                            case (long)AllLookupValues.MedProductType.THUOC:
                                {
                                    ReportModel = null;
                                    ReportModel = new RptDrugDeptStockTakes_Get_Thuoc().PreviewModel;
                                    rParams["parTitleRpt"].Value = TitleRpt;
                                    rParams["parTitleRpt1"].Value = TitleRpt1;
                                    rParams["ID"].Value = (int)ID;
                                    rParams["ThuKho"].Value = ThuKho != null && ThuKho.StaffPositionID > 0 ? ThuKho.FullNameString : "";
                                    rParams["TruongKhoaDuoc"].Value = TruongKhoaDuoc != null ? TruongKhoaDuoc.FullNameString : "";
                                    rParams["ThongKeDuoc"].Value = ThongKeDuoc != null ? ThongKeDuoc.FullNameString : "";
                                    break;
                                }
                            case (long)AllLookupValues.MedProductType.Y_CU:
                                {
                                    ReportModel = null;
                                    ReportModel = new RptDrugDeptStockTakes_Get_YCu().PreviewModel;
                                    rParams["parTitleRpt"].Value = TitleRpt;
                                    rParams["parTitleRpt1"].Value = TitleRpt1;
                                    rParams["ID"].Value = (int)ID;
                                    rParams["ThuKho"].Value = ThuKho != null && ThuKho.StaffPositionID > 0 ? ThuKho.FullNameString : "";
                                    rParams["TruongKhoaDuoc"].Value = TruongKhoaDuoc != null ? TruongKhoaDuoc.FullNameString : "";
                                    rParams["ThongKeDuoc"].Value = ThongKeDuoc != null ? ThongKeDuoc.FullNameString : "";
                                    break;
                                }
                            case (long)AllLookupValues.MedProductType.HOA_CHAT:
                                {
                                    ReportModel = null;
                                    ReportModel = new RptDrugDeptStockTakes_Get_HoaChat().PreviewModel;
                                    rParams["parTitleRpt"].Value = TitleRpt;
                                    rParams["parTitleRpt1"].Value = TitleRpt1;
                                    rParams["ID"].Value = (int)ID;
                                    rParams["ThuKho"].Value = ThuKho != null && ThuKho.StaffPositionID > 0 ? ThuKho.FullNameString : "";
                                    rParams["TruongKhoaDuoc"].Value = TruongKhoaDuoc != null ? TruongKhoaDuoc.FullNameString : "";
                                    rParams["ThongKeDuoc"].Value = ThongKeDuoc != null ? ThongKeDuoc.FullNameString : "";
                                    break;
                                }
                        }
                        break;
                    }
                case ReportName.DRUGDEPT_KIEMKE_KHOADUOC:
                    {
                        switch (V_MedProductType)
                        {
                            case (long)AllLookupValues.MedProductType.THUOC:
                                {
                                    ReportModel = null;
                                    ReportModel = new DrugDeptPhieuKiemKeThuocReportModel().PreviewModel;
                                    rParams["ID"].Value = (int)ID;
                                    break;
                                }
                            case (long)AllLookupValues.MedProductType.Y_CU:
                                {
                                    ReportModel = null;
                                    ReportModel = new DrugDeptPhieuKiemKeYCuReportModel().PreviewModel;
                                    rParams["ID"].Value = (int)ID;
                                    rParams["parLogoUrl"].Value = reportLogoUrl;
                                    break;
                                }
                            case (long)AllLookupValues.MedProductType.HOA_CHAT:
                                {
                                    ReportModel = null;
                                    ReportModel = new DrugDeptPhieuKiemKeHoaChatReportModel().PreviewModel;
                                    rParams["ID"].Value = (int)ID;
                                    rParams["parLogoUrl"].Value = reportLogoUrl;
                                    break;
                                }
                            default:
                                {
                                    ReportModel = null;
                                    ReportModel = new DrugDeptPhieuKiemKeThuocReportModel().PreviewModel;
                                    rParams["ID"].Value = (int)ID;
                                    rParams["parLogoUrl"].Value = reportLogoUrl;
                                    break;
                                }
                        }
                        break;
                    }
                //▼====== #001
                case ReportName.DrugDept_Request_HIStore:
                    ReportModel = null;
                    ReportModel = new DrugDeptRequesHIStoretReportModel().PreviewModel;
                    rParams["RequestID"].Value = (int)ID;
                    rParams["Show"].Value = LyDo;
                    rParams["TruongKhoaDuoc"].Value = TruongKhoaDuoc != null ? TruongKhoaDuoc.FullNameString : "";
                    rParams["parHospitalName"].Value = reportHospitalName;
                    break;
                case ReportName.DrugDept_Request_HIStore_Approved:
                    ReportModel = null;
                    ReportModel = new DrugDeptRequestHIStoreApprovedReportModel().PreviewModel;
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
                case ReportName.DrugDept_Request_HIStore_Details_Approved:
                    ReportModel = null;
                    ReportModel = new DrugDeptRequestHIStoreDetailsApprovedReportModel().PreviewModel;
                    rParams["RequestID"].Value = (int)ID;
                    rParams["Show"].Value = LyDo;
                    rParams["TruongKhoaDuoc"].Value = TruongKhoaDuoc != null ? TruongKhoaDuoc.FullNameString : "";
                    rParams["parHospitalName"].Value = reportHospitalName;
                    break;
                //▲====== #001
                case ReportName.DRUGDEPT_INWARD_VTYTTHSUPPLIER:
                    ReportModel = null;
                    ReportModel = new DrugDepInwardVTYTTHSupplierReportModel().PreviewModel;
                    rParams["InvID"].Value = (int)ID;
                    rParams["Show"].Value = LyDo;
                    if (Globals.ServerConfigSection.Hospitals.IsDirectorSignature)
                    {
                        rParams["ChucVu"].Value = eHCMSResources.Z0947_G1_ThuTruongDVi;
                        rParams["GiamDocHoacPhoGiamDoc"].Value = GiamDoc != null ? GiamDoc.FullNameString : "";
                    }
                    else
                    {
                        rParams["ChucVu"].Value = eHCMSResources.Z0948_G1_PhoThuTruongDVi;
                        rParams["GiamDocHoacPhoGiamDoc"].Value = PhoGiamDoc != null ? PhoGiamDoc.FullNameString : "";
                    }
                    rParams["KeToanTruong"].Value = KeToanTruong != null ? KeToanTruong.FullNameString : "";
                    rParams["ThuKho"].Value = ThuKho != null && ThuKho.StaffPositionID > 0 ? ThuKho.FullNameString : "";
                    rParams["TruongKhoaDuoc"].Value = TruongKhoaDuoc != null ? TruongKhoaDuoc.FullNameString : "";
                    rParams["ThongKeDuoc"].Value = ThongKeDuoc != null ? ThongKeDuoc.FullNameString : "";
                    break;
                case ReportName.DRUGDEPT_INWARD_VTYTTHSUPPLIER_TRONGNUOC:
                    ReportModel = null;
                    ReportModel = new DrugDepInwardVTYTTHSupplierTrongNuocReportModel().PreviewModel;
                    rParams["InvID"].Value = (int)ID;
                    rParams["Show"].Value = LyDo;
                    if (Globals.ServerConfigSection.Hospitals.IsDirectorSignature)
                    {
                        rParams["ChucVu"].Value = eHCMSResources.Z0947_G1_ThuTruongDVi;
                        rParams["GiamDocHoacPhoGiamDoc"].Value = GiamDoc != null ? GiamDoc.FullNameString : "";
                    }
                    else
                    {
                        rParams["ChucVu"].Value = eHCMSResources.Z0948_G1_PhoThuTruongDVi;
                        rParams["GiamDocHoacPhoGiamDoc"].Value = PhoGiamDoc != null ? PhoGiamDoc.FullNameString : "";
                    }
                    rParams["KeToanTruong"].Value = KeToanTruong != null ? KeToanTruong.FullNameString : "";
                    rParams["ThuKho"].Value = ThuKho != null && ThuKho.StaffPositionID > 0 ? ThuKho.FullNameString : "";
                    rParams["TruongKhoaDuoc"].Value = TruongKhoaDuoc != null ? TruongKhoaDuoc.FullNameString : "";
                    rParams["ThongKeDuoc"].Value = ThongKeDuoc != null ? ThongKeDuoc.FullNameString : "";
                    rParams["parDepartmentOfHealth"].Value = reportDepartmentOfHealth;
                    rParams["parHospitalName"].Value = reportHospitalName;
                    rParams["parHospitalAddress"].Value = reportHospitalAddress;
                    rParams["parStaffName"].Value = StaffFullName ?? "";
                    break;
                case ReportName.DRUGDEPT_INWARD_TIEM_NGUA_SUPPLIER:
                    ReportModel = null;
                    ReportModel = new DrugDepInwardTiemNguaSupplierReportModel().PreviewModel;
                    rParams["InvID"].Value = (int)ID;
                    rParams["Show"].Value = LyDo;
                    if (Globals.ServerConfigSection.Hospitals.IsDirectorSignature)
                    {
                        rParams["ChucVu"].Value = eHCMSResources.Z0947_G1_ThuTruongDVi;
                        rParams["GiamDocHoacPhoGiamDoc"].Value = GiamDoc != null ? GiamDoc.FullNameString : "";
                    }
                    else
                    {
                        rParams["ChucVu"].Value = eHCMSResources.Z0948_G1_PhoThuTruongDVi;
                        rParams["GiamDocHoacPhoGiamDoc"].Value = PhoGiamDoc != null ? PhoGiamDoc.FullNameString : "";
                    }
                    rParams["KeToanTruong"].Value = KeToanTruong != null ? KeToanTruong.FullNameString : "";
                    rParams["ThuKho"].Value = ThuKho != null && ThuKho.StaffPositionID > 0 ? ThuKho.FullNameString : "";
                    rParams["TruongKhoaDuoc"].Value = TruongKhoaDuoc != null ? TruongKhoaDuoc.FullNameString : "";
                    rParams["ThongKeDuoc"].Value = ThongKeDuoc != null ? ThongKeDuoc.FullNameString : "";
                    break;
                case ReportName.DRUGDEPT_INWARD_TIEM_NGUA_SUPPLIER_TRONGNUOC:
                    ReportModel = null;
                    ReportModel = new DrugDepInwardTiemNguaSupplierTrongNuocReportModel().PreviewModel;
                    rParams["InvID"].Value = (int)ID;
                    rParams["Show"].Value = LyDo;
                    if (Globals.ServerConfigSection.Hospitals.IsDirectorSignature)
                    {
                        rParams["ChucVu"].Value = eHCMSResources.Z0947_G1_ThuTruongDVi;
                        rParams["GiamDocHoacPhoGiamDoc"].Value = GiamDoc != null ? GiamDoc.FullNameString : "";
                    }
                    else
                    {
                        rParams["ChucVu"].Value = eHCMSResources.Z0948_G1_PhoThuTruongDVi;
                        rParams["GiamDocHoacPhoGiamDoc"].Value = PhoGiamDoc != null ? PhoGiamDoc.FullNameString : "";
                    }
                    rParams["KeToanTruong"].Value = KeToanTruong != null ? KeToanTruong.FullNameString : "";
                    rParams["ThuKho"].Value = ThuKho != null && ThuKho.StaffPositionID > 0 ? ThuKho.FullNameString : "";
                    rParams["TruongKhoaDuoc"].Value = TruongKhoaDuoc != null ? TruongKhoaDuoc.FullNameString : "";
                    rParams["ThongKeDuoc"].Value = ThongKeDuoc != null ? ThongKeDuoc.FullNameString : "";
                    rParams["parDepartmentOfHealth"].Value = reportDepartmentOfHealth;
                    rParams["parHospitalName"].Value = reportHospitalName;
                    rParams["parHospitalAddress"].Value = reportHospitalAddress;
                    rParams["parStaffName"].Value = StaffFullName ?? "";
                    break;
                case ReportName.DRUGDEPT_INWARD_HOA_CHAT_SUPPLIER:
                    ReportModel = null;
                    ReportModel = new DrugDepInwardChemicalSupplierReportModel().PreviewModel;
                    rParams["InvID"].Value = (int)ID;
                    rParams["Show"].Value = LyDo;
                    if (Globals.ServerConfigSection.Hospitals.IsDirectorSignature)
                    {
                        rParams["ChucVu"].Value = eHCMSResources.Z0947_G1_ThuTruongDVi;
                        rParams["GiamDocHoacPhoGiamDoc"].Value = GiamDoc != null ? GiamDoc.FullNameString : "";
                    }
                    else
                    {
                        rParams["ChucVu"].Value = eHCMSResources.Z0948_G1_PhoThuTruongDVi;
                        rParams["GiamDocHoacPhoGiamDoc"].Value = PhoGiamDoc != null ? PhoGiamDoc.FullNameString : "";
                    }
                    rParams["KeToanTruong"].Value = KeToanTruong != null ? KeToanTruong.FullNameString : "";
                    rParams["ThuKho"].Value = ThuKho != null && ThuKho.StaffPositionID > 0 ? ThuKho.FullNameString : "";
                    rParams["TruongKhoaDuoc"].Value = TruongKhoaDuoc != null ? TruongKhoaDuoc.FullNameString : "";
                    rParams["ThongKeDuoc"].Value = ThongKeDuoc != null ? ThongKeDuoc.FullNameString : "";
                    break;
                case ReportName.DRUGDEPT_INWARD_HOA_CHAT_SUPPLIER_TRONGNUOC:
                    ReportModel = null;
                    ReportModel = new DrugDepInwardChemicalSupplierTrongNuocReportModel().PreviewModel;
                    rParams["InvID"].Value = (int)ID;
                    rParams["Show"].Value = LyDo;
                    if (Globals.ServerConfigSection.Hospitals.IsDirectorSignature)
                    {
                        rParams["ChucVu"].Value = eHCMSResources.Z0947_G1_ThuTruongDVi;
                        rParams["GiamDocHoacPhoGiamDoc"].Value = GiamDoc != null ? GiamDoc.FullNameString : "";
                    }
                    else
                    {
                        rParams["ChucVu"].Value = eHCMSResources.Z0948_G1_PhoThuTruongDVi;
                        rParams["GiamDocHoacPhoGiamDoc"].Value = PhoGiamDoc != null ? PhoGiamDoc.FullNameString : "";
                    }
                    rParams["KeToanTruong"].Value = KeToanTruong != null ? KeToanTruong.FullNameString : "";
                    rParams["ThuKho"].Value = ThuKho != null && ThuKho.StaffPositionID > 0 ? ThuKho.FullNameString : "";
                    rParams["TruongKhoaDuoc"].Value = TruongKhoaDuoc != null ? TruongKhoaDuoc.FullNameString : "";
                    rParams["ThongKeDuoc"].Value = ThongKeDuoc != null ? ThongKeDuoc.FullNameString : "";
                    rParams["parDepartmentOfHealth"].Value = reportDepartmentOfHealth;
                    rParams["parHospitalName"].Value = reportHospitalName;
                    rParams["parHospitalAddress"].Value = reportHospitalAddress;
                    rParams["parStaffName"].Value = StaffFullName ?? "";
                    break;

                case ReportName.DRUGDEPT_INWARD_MAU_SUPPLIER:
                    ReportModel = null;
                    ReportModel = new DrugDepInwardBloodSupplierReportModel().PreviewModel;
                    rParams["InvID"].Value = (int)ID;
                    rParams["Show"].Value = LyDo;
                    if (Globals.ServerConfigSection.Hospitals.IsDirectorSignature)
                    {
                        rParams["ChucVu"].Value = eHCMSResources.Z0947_G1_ThuTruongDVi;
                        rParams["GiamDocHoacPhoGiamDoc"].Value = GiamDoc != null ? GiamDoc.FullNameString : "";
                    }
                    else
                    {
                        rParams["ChucVu"].Value = eHCMSResources.Z0948_G1_PhoThuTruongDVi;
                        rParams["GiamDocHoacPhoGiamDoc"].Value = PhoGiamDoc != null ? PhoGiamDoc.FullNameString : "";
                    }
                    rParams["KeToanTruong"].Value = KeToanTruong != null ? KeToanTruong.FullNameString : "";
                    rParams["ThuKho"].Value = ThuKho != null && ThuKho.StaffPositionID > 0 ? ThuKho.FullNameString : "";
                    rParams["TruongKhoaDuoc"].Value = TruongKhoaDuoc != null ? TruongKhoaDuoc.FullNameString : "";
                    rParams["ThongKeDuoc"].Value = ThongKeDuoc != null ? ThongKeDuoc.FullNameString : "";
                    break;
                case ReportName.DRUGDEPT_INWARD_MAU_SUPPLIER_TRONGNUOC:
                    ReportModel = null;
                    ReportModel = new DrugDepInwardBloodSupplierTrongNuocReportModel().PreviewModel;
                    rParams["InvID"].Value = (int)ID;
                    rParams["Show"].Value = LyDo;
                    if (Globals.ServerConfigSection.Hospitals.IsDirectorSignature)
                    {
                        rParams["ChucVu"].Value = eHCMSResources.Z0947_G1_ThuTruongDVi;
                        rParams["GiamDocHoacPhoGiamDoc"].Value = GiamDoc != null ? GiamDoc.FullNameString : "";
                    }
                    else
                    {
                        rParams["ChucVu"].Value = eHCMSResources.Z0948_G1_PhoThuTruongDVi;
                        rParams["GiamDocHoacPhoGiamDoc"].Value = PhoGiamDoc != null ? PhoGiamDoc.FullNameString : "";
                    }
                    rParams["KeToanTruong"].Value = KeToanTruong != null ? KeToanTruong.FullNameString : "";
                    rParams["ThuKho"].Value = ThuKho != null && ThuKho.StaffPositionID > 0 ? ThuKho.FullNameString : "";
                    rParams["TruongKhoaDuoc"].Value = TruongKhoaDuoc != null ? TruongKhoaDuoc.FullNameString : "";
                    rParams["ThongKeDuoc"].Value = ThongKeDuoc != null ? ThongKeDuoc.FullNameString : "";
                    rParams["parDepartmentOfHealth"].Value = reportDepartmentOfHealth;
                    rParams["parHospitalName"].Value = reportHospitalName;
                    rParams["parHospitalAddress"].Value = reportHospitalAddress;
                    rParams["parStaffName"].Value = StaffFullName ?? "";
                    break;

                case ReportName.DRUGDEPT_INWARD_THANH_TRUNG_SUPPLIER:
                    ReportModel = null;
                    ReportModel = new DrugDepInwardThanhTrungSupplierReportModel().PreviewModel;
                    rParams["InvID"].Value = (int)ID;
                    rParams["Show"].Value = LyDo;
                    if (Globals.ServerConfigSection.Hospitals.IsDirectorSignature)
                    {
                        rParams["ChucVu"].Value = eHCMSResources.Z0947_G1_ThuTruongDVi;
                        rParams["GiamDocHoacPhoGiamDoc"].Value = GiamDoc != null ? GiamDoc.FullNameString : "";
                    }
                    else
                    {
                        rParams["ChucVu"].Value = eHCMSResources.Z0948_G1_PhoThuTruongDVi;
                        rParams["GiamDocHoacPhoGiamDoc"].Value = PhoGiamDoc != null ? PhoGiamDoc.FullNameString : "";
                    }
                    rParams["KeToanTruong"].Value = KeToanTruong != null ? KeToanTruong.FullNameString : "";
                    rParams["ThuKho"].Value = ThuKho != null && ThuKho.StaffPositionID > 0 ? ThuKho.FullNameString : "";
                    rParams["TruongKhoaDuoc"].Value = TruongKhoaDuoc != null ? TruongKhoaDuoc.FullNameString : "";
                    rParams["ThongKeDuoc"].Value = ThongKeDuoc != null ? ThongKeDuoc.FullNameString : "";
                    break;
                case ReportName.DRUGDEPT_INWARD_THANH_TRUNG_SUPPLIER_TRONGNUOC:
                    ReportModel = null;
                    ReportModel = new DrugDepInwardThanhTrungSupplierTrongNuocReportModel().PreviewModel;
                    rParams["InvID"].Value = (int)ID;
                    rParams["Show"].Value = LyDo;
                    if (Globals.ServerConfigSection.Hospitals.IsDirectorSignature)
                    {
                        rParams["ChucVu"].Value = eHCMSResources.Z0947_G1_ThuTruongDVi;
                        rParams["GiamDocHoacPhoGiamDoc"].Value = GiamDoc != null ? GiamDoc.FullNameString : "";
                    }
                    else
                    {
                        rParams["ChucVu"].Value = eHCMSResources.Z0948_G1_PhoThuTruongDVi;
                        rParams["GiamDocHoacPhoGiamDoc"].Value = PhoGiamDoc != null ? PhoGiamDoc.FullNameString : "";
                    }
                    rParams["KeToanTruong"].Value = KeToanTruong != null ? KeToanTruong.FullNameString : "";
                    rParams["ThuKho"].Value = ThuKho != null && ThuKho.StaffPositionID > 0 ? ThuKho.FullNameString : "";
                    rParams["TruongKhoaDuoc"].Value = TruongKhoaDuoc != null ? TruongKhoaDuoc.FullNameString : "";
                    rParams["ThongKeDuoc"].Value = ThongKeDuoc != null ? ThongKeDuoc.FullNameString : "";
                    rParams["parDepartmentOfHealth"].Value = reportDepartmentOfHealth;
                    rParams["parHospitalName"].Value = reportHospitalName;
                    rParams["parHospitalAddress"].Value = reportHospitalAddress;
                    rParams["parStaffName"].Value = StaffFullName ?? "";
                    break;
                case ReportName.DRUGDEPT_INWARD_VPP_SUPPLIER:
                    ReportModel = null;
                    ReportModel = new DrugDepInwardVPPSupplierReportModel().PreviewModel;
                    rParams["InvID"].Value = (int)ID;
                    rParams["Show"].Value = LyDo;
                    if (Globals.ServerConfigSection.Hospitals.IsDirectorSignature)
                    {
                        rParams["ChucVu"].Value = eHCMSResources.Z0947_G1_ThuTruongDVi;
                        rParams["GiamDocHoacPhoGiamDoc"].Value = GiamDoc != null ? GiamDoc.FullNameString : "";
                    }
                    else
                    {
                        rParams["ChucVu"].Value = eHCMSResources.Z0948_G1_PhoThuTruongDVi;
                        rParams["GiamDocHoacPhoGiamDoc"].Value = PhoGiamDoc != null ? PhoGiamDoc.FullNameString : "";
                    }
                    rParams["KeToanTruong"].Value = KeToanTruong != null ? KeToanTruong.FullNameString : "";
                    rParams["ThuKho"].Value = ThuKho != null && ThuKho.StaffPositionID > 0 ? ThuKho.FullNameString : "";
                    rParams["TruongKhoaDuoc"].Value = TruongKhoaDuoc != null ? TruongKhoaDuoc.FullNameString : "";
                    rParams["ThongKeDuoc"].Value = ThongKeDuoc != null ? ThongKeDuoc.FullNameString : "";
                    break;
                case ReportName.DRUGDEPT_INWARD_VPP_SUPPLIER_TRONGNUOC:
                    ReportModel = null;
                    ReportModel = new DrugDepInwardVPPSupplierTrongNuocReportModel().PreviewModel;
                    rParams["InvID"].Value = (int)ID;
                    rParams["Show"].Value = LyDo;
                    if (Globals.ServerConfigSection.Hospitals.IsDirectorSignature)
                    {
                        rParams["ChucVu"].Value = eHCMSResources.Z0947_G1_ThuTruongDVi;
                        rParams["GiamDocHoacPhoGiamDoc"].Value = GiamDoc != null ? GiamDoc.FullNameString : "";
                    }
                    else
                    {
                        rParams["ChucVu"].Value = eHCMSResources.Z0948_G1_PhoThuTruongDVi;
                        rParams["GiamDocHoacPhoGiamDoc"].Value = PhoGiamDoc != null ? PhoGiamDoc.FullNameString : "";
                    }
                    rParams["KeToanTruong"].Value = KeToanTruong != null ? KeToanTruong.FullNameString : "";
                    rParams["ThuKho"].Value = ThuKho != null && ThuKho.StaffPositionID > 0 ? ThuKho.FullNameString : "";
                    rParams["TruongKhoaDuoc"].Value = TruongKhoaDuoc != null ? TruongKhoaDuoc.FullNameString : "";
                    rParams["ThongKeDuoc"].Value = ThongKeDuoc != null ? ThongKeDuoc.FullNameString : "";
                    rParams["parDepartmentOfHealth"].Value = reportDepartmentOfHealth;
                    rParams["parHospitalName"].Value = reportHospitalName;
                    rParams["parStaffName"].Value = StaffFullName != null ? StaffFullName : "";
                    break;
                case ReportName.DRUGDEPT_INWARD_VTTH_SUPPLIER:
                    ReportModel = null;
                    ReportModel = new DrugDepInwardVTTHSupplierReportModel().PreviewModel;
                    rParams["InvID"].Value = (int)ID;
                    rParams["Show"].Value = LyDo;
                    if (Globals.ServerConfigSection.Hospitals.IsDirectorSignature)
                    {
                        rParams["ChucVu"].Value = eHCMSResources.Z0947_G1_ThuTruongDVi;
                        rParams["GiamDocHoacPhoGiamDoc"].Value = GiamDoc != null ? GiamDoc.FullNameString : "";
                    }
                    else
                    {
                        rParams["ChucVu"].Value = eHCMSResources.Z0948_G1_PhoThuTruongDVi;
                        rParams["GiamDocHoacPhoGiamDoc"].Value = PhoGiamDoc != null ? PhoGiamDoc.FullNameString : "";
                    }
                    rParams["KeToanTruong"].Value = KeToanTruong != null ? KeToanTruong.FullNameString : "";
                    rParams["ThuKho"].Value = ThuKho != null && ThuKho.StaffPositionID > 0 ? ThuKho.FullNameString : "";
                    rParams["TruongKhoaDuoc"].Value = TruongKhoaDuoc != null ? TruongKhoaDuoc.FullNameString : "";
                    rParams["ThongKeDuoc"].Value = ThongKeDuoc != null ? ThongKeDuoc.FullNameString : "";
                    break;
                case ReportName.DRUGDEPT_INWARD_VTTH_SUPPLIER_TRONGNUOC:
                    ReportModel = null;
                    ReportModel = new DrugDepInwardVTTHSupplierTrongNuocReportModel().PreviewModel;
                    rParams["InvID"].Value = (int)ID;
                    rParams["Show"].Value = LyDo;
                    if (Globals.ServerConfigSection.Hospitals.IsDirectorSignature)
                    {
                        rParams["ChucVu"].Value = eHCMSResources.Z0947_G1_ThuTruongDVi;
                        rParams["GiamDocHoacPhoGiamDoc"].Value = GiamDoc != null ? GiamDoc.FullNameString : "";
                    }
                    else
                    {
                        rParams["ChucVu"].Value = eHCMSResources.Z0948_G1_PhoThuTruongDVi;
                        rParams["GiamDocHoacPhoGiamDoc"].Value = PhoGiamDoc != null ? PhoGiamDoc.FullNameString : "";
                    }
                    rParams["KeToanTruong"].Value = KeToanTruong != null ? KeToanTruong.FullNameString : "";
                    rParams["ThuKho"].Value = ThuKho != null && ThuKho.StaffPositionID > 0 ? ThuKho.FullNameString : "";
                    rParams["TruongKhoaDuoc"].Value = TruongKhoaDuoc != null ? TruongKhoaDuoc.FullNameString : "";
                    rParams["ThongKeDuoc"].Value = ThongKeDuoc != null ? ThongKeDuoc.FullNameString : "";
                    rParams["parDepartmentOfHealth"].Value = reportDepartmentOfHealth;
                    rParams["parHospitalName"].Value = reportHospitalName;
                    rParams["parStaffName"].Value = StaffFullName != null ? StaffFullName : "";
                    break;
                //▼====: #003
                case ReportName.BBGiaoNhanVaccine:
                    ReportModel = null;
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.RptDrugDepts.XtraReports.OutInternal.XRptBienBanGiaoNhanVaccine").PreviewModel;
                    rParams["outiID"].Value = ID;
                    rParams["V_MedProductType"].Value = V_MedProductType;
                    break;
                //▲====: #003
                //▼====: #004
                case ReportName.DRUGDEPT_ESTIMATION_FOR_DEPT:
                    ReportModel = null;
                    ReportModel = new DrugDeptEstimationReportForDeptModal().PreviewModel;
                    rParams["DrugDeptEstimatePoID"].Value = (int)ID;
                    rParams["EstimationCodeBegin"].Value = (int)EstimationCodeBegin;
                    rParams["EstimationCodeEnd"].Value = (int)EstimationCodeEnd;
                    rParams["V_MedProductType"].Value = (int)V_MedProductType;
                    rParams["EstimationFromRequest"].Value = EstimationFromRequest;
                    rParams["Show"].Value = LyDo;
                    rParams["GiamDoc"].Value = GiamDoc != null ? GiamDoc.FullNameString : "";
                    rParams["KeToanTruong"].Value = KeToanTruong != null ? KeToanTruong.FullNameString : "";
                    rParams["TPKeHoach"].Value = TPKeHoach != null ? TPKeHoach.FullNameString : "";
                    rParams["TruongKhoaDuoc"].Value = TruongKhoaDuoc != null ? TruongKhoaDuoc.FullNameString : "";
                    if (!string.IsNullOrEmpty(DeptDirectorSignTitle))
                    {
                        rParams["pDeptDirectorSignTitle"].Value = DeptDirectorSignTitle == null ? "" : DeptDirectorSignTitle.ToUpper();
                    }
                    if (!string.IsNullOrEmpty(DeptStatsSignTitle))
                    {
                        rParams["pDeptStatsSignTitle"].Value = DeptStatsSignTitle == null ? "" : DeptStatsSignTitle.ToUpper();
                    }
                    rParams["ThongKeDuoc"].Value = ThongKeDuoc != null ? ThongKeDuoc.FullNameString : "";
                    rParams["parLogoUrl"].Value = reportLogoUrl;
                    rParams["userStaffFullNameExportExcel"].Value = StaffFullName ?? "";
                    if (EstimationFromRequest)
                    {
                        rParams["parHospitalName"].Value = reportHospitalName;
                    }
                    break;
                //▲====: #004
            }

            ReportModel.CreateDocument(rParams);
        }

        public void btnClose()
        {
            TryClose();
        }
        private bool _EstimationFromRequest;
        public bool EstimationFromRequest
        {
            get
            {
                return _EstimationFromRequest;
            }
            set
            {
                _EstimationFromRequest = value;
                NotifyOfPropertyChange(() => EstimationFromRequest);
            }
        }
    }
}
