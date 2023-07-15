using eHCMSLanguage;
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
* 20181103 #001 TNHX: [BM0005214] Update report PhieuNhanThuoc base RefApplicationConfig.MixedHIPharmacyStores using PhieuNhanThuocSummary_XML
* 20190424 #002 TNHX: [BM0006716] Create PhieuNhanThuoc, PhieuNhanThuocBHYT, PhieuNhanThuocSummary for Thermal
* 20190929 #003 TNHX: [BM0017380] Add report InwardFromInternal of Pharmacy
* 20220608 #004 DatTB: Thêm in report hướng dẫn sử dụng theo toa thuốc
* 20220823 #005 DatTB: Chỉnh sửa màn hình chờ nhận thuốc
*/
namespace aEMR.Pharmacy.ViewModels
{
    [Export(typeof(IReportDocumentPreview)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class ReportDocumentPreviewViewModel : Conductor<object>, IReportDocumentPreview
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        public string TitleForm { get; set; }

        [ImportingConstructor]
        public ReportDocumentPreviewViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;
            eItem = new ReportName();
        }

        private bool _IsPatientCOVID;
        public bool IsPatientCOVID
        {
            get { return _IsPatientCOVID; }
            set
            {
                _IsPatientCOVID = value;
                NotifyOfPropertyChange(() => IsPatientCOVID);
            }
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

        private long _SupplierID;
        public long SupplierID
        {
            get { return _SupplierID; }
            set
            {
                _SupplierID = value;
                NotifyOfPropertyChange(() => SupplierID);
            }
        }

        private long _PCOID;
        public long PCOID
        {
            get { return _PCOID; }
            set
            {
                _PCOID = value;
                NotifyOfPropertyChange(() => PCOID);
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

        private string _ListID = "";
        public string ListID
        {
            get { return _ListID; }
            set
            {
                _ListID = value;
                NotifyOfPropertyChange(() => ListID);
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

        public bool IsInsurance { get; set; }

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

        private long _V_RegistrationType = (long)AllLookupValues.RegistrationType.NGOAI_TRU;

        public long V_RegistrationType { get => _V_RegistrationType; set
            {
                _V_RegistrationType = value;
                NotifyOfPropertyChange(() => V_RegistrationType);
            }
        }

        protected override void OnActivate()
        {
            base.OnActivate();

            //if (ReportModel != null)
            //{
            //    ReportModel.RequestDefaultParameterValues -= new WeakEventHandler<EventArgs>(_reportModel_RequestDefaultParameterValues).Handler;
            //}
            var GiamDoc = Globals.allStaffPositions.Where(x => x.PositionRefID == (int)AllLookupValues.StaffPositions_Enum.GIAM_DOC && x.IsActive).FirstOrDefault();
            var PhoGiamDoc = Globals.allStaffPositions.Where(x => x.PositionRefID == (int)AllLookupValues.StaffPositions_Enum.PHO_GIAM_DOC && x.IsActive).FirstOrDefault();
            var KeToanTruong = Globals.allStaffPositions.Where(x => x.PositionRefID == (int)AllLookupValues.StaffPositions_Enum.KE_TOAN_TRUONG && x.IsActive).FirstOrDefault();
            var PhoKhoaDuoc = Globals.allStaffPositions.Where(x => x.PositionRefID == (int)AllLookupValues.StaffPositions_Enum.TRUONG_NHA_THUOC && x.IsActive).FirstOrDefault();
            //KMx: Thời điểm làm TruongNhaThuoc là đã có PhoKhoaDuoc rồi (2 cái giống nhau). Muốn để TruongNhaThuoc cho dễ nhớ (09/11/2015 11:57).
            var TruongNhaThuoc = Globals.allStaffPositions.Where(x => x.PositionRefID == (int)AllLookupValues.StaffPositions_Enum.TRUONG_NHA_THUOC && x.IsActive).FirstOrDefault();
            var ThuKhoThuoc = Globals.allStaffPositions.Where(x => x.PositionRefID == (int)AllLookupValues.StaffPositions_Enum.THU_KHO_NHA_THUOC && x.IsActive).FirstOrDefault();
            var DieuDuongTruongKhoa = Globals.allStaffPositions.Where(x => x.PositionRefID == (int)AllLookupValues.StaffPositions_Enum.DIEU_DUONG_TRUONG_KHOA && x.IsActive).FirstOrDefault();
            var TPKeHoach = Globals.allStaffPositions.Where(x => x.PositionRefID == (int)AllLookupValues.StaffPositions_Enum.TP_KE_HOACH && x.IsActive).FirstOrDefault();
            DevExpress.DocumentServices.ServiceModel.DefaultValueParameterContainer theParams = new DevExpress.DocumentServices.ServiceModel.DefaultValueParameterContainer();
            string reportLogoUrl = Globals.ServerConfigSection.CommonItems.ReportLogoUrl;
            string reportDepartmentOfHealth = Globals.ServerConfigSection.CommonItems.ReportDepartmentOfHealth;
            string reportHospitalName = Globals.ServerConfigSection.CommonItems.ReportHospitalName;
            string reportHospitalAddess = Globals.ServerConfigSection.CommonItems.ReportHospitalAddress;
            string reportDQGUnitname = Globals.ServerConfigSection.CommonItems.DQGUnitname;
            switch (_eItem)
            {             
                case ReportName.PHARMACY_NHAPTHUOCTUNCC:
                    ReportModel = null;
                    ReportModel = new PharmacyInwardDrugSupplierModel().PreviewModel;
                    theParams["InvID"].Value = (int)ID;
                    //KMx: Kể từ ngày 1/9/2015 người ký tên phiếu nhập thuốc là Phó Giám Đốc (09/09/2015 16:50).
                    //ReportModel.Parameters["GiamDoc"].Value = GiamDoc != null ? GiamDoc.FullNameString : "";
                    if (Globals.ServerConfigSection.Hospitals.IsDirectorSignature)
                    {
                        theParams["ChucVu"].Value = eHCMSResources.Z0947_G1_ThuTruongDVi;
                        theParams["GiamDocHoacPhoGiamDoc"].Value = GiamDoc != null ? GiamDoc.FullNameString : "";
                    }
                    else
                    {
                        theParams["ChucVu"].Value = eHCMSResources.Z0948_G1_PhoThuTruongDVi;
                        theParams["GiamDocHoacPhoGiamDoc"].Value = PhoGiamDoc != null ? PhoGiamDoc.FullNameString : "";
                    }

                    theParams["KeToanTruong"].Value = KeToanTruong !=null ? KeToanTruong.FullNameString:"";
                    theParams["PhoKhoaDuoc"].Value = PhoKhoaDuoc != null ? PhoKhoaDuoc.FullNameString : "";
                    theParams["ThuKhoThuoc"].Value = ThuKhoThuoc != null ? ThuKhoThuoc.FullNameString : "";
                    theParams["parDQGUnitname"].Value = reportDQGUnitname;
                    break;
                case ReportName.PHARMACY_TRATHUOC:
                    ReportModel = null;
                    ReportModel = new PharmacyReturnDrugReportModel().PreviewModel;
                    theParams["OutiID"].Value = (int)ID;
                    theParams["parHospitalName"].Value = reportHospitalName;
                    break;
                case ReportName.PHARMACY_TRATHUOCBH:
                    ReportModel = null;
                    ReportModel = new PharmacyReturnDrugInsuranceReportModel().PreviewModel;
                    theParams["OutiID"].Value = (int)ID;
                    theParams["parHospitalName"].Value = reportHospitalName;
                    break;
                /*▼====: #001*/
                case ReportName.PHARMACY_PHIEUNHANTHUOC:
                    ReportModel = null;
                    ReportModel = new PharmacyCollectionDrugReportModel().PreviewModel;
                    theParams["OutiID"].Value = ID;
                    theParams["parHospitalName"].Value = reportHospitalName;
                    break;
                case ReportName.PHARMACY_PHIEUNHANTHUOC_PRIVATE:
                    ReportModel = null;
                    ReportModel = new PharmacyCollectionDrugPrivateReportModel().PreviewModel;
                    theParams["OutiID"].Value = (int)ID;
                    break;
                case ReportName.PHARMACY_PHIEUNHANTHUOC_BH:
                    ReportModel = null;
                    ReportModel = new PharmacyCollectionDrugBHReportModel().PreviewModel;
                    theParams["OutiID"].Value = ID;
                    theParams["parHospitalName"].Value = reportHospitalName;
                    break;
                case ReportName.PHARMACY_PHIEUNHANTHUOC_SUMMARY:
                    ReportModel = null;
                    ReportModel = new PharmacyCollectionDrugSummaryXMLReportModel().PreviewModel;
                    theParams["OutiID"].Value = ListID;
                    theParams["parHospitalName"].Value = reportHospitalName;
                    break;
                /*▲====: #001*/
                case ReportName.PHARMACY_VISITORPHIEUTHU:
                    ReportModel = null;
                    ReportModel = new PaymentVisistorReportModel().PreviewModel;
                    theParams["param_User"].Value = StaffFullName;
                    theParams["param_PaymentID"].Value = (int)PaymentID;
                    theParams["OutiID"].Value = (int)ID;
                    theParams["parameter_LyDo"].Value = LyDo;
                    theParams["V_TranRefType"].Value = (int)V_TranRefType;
                    break;
                case ReportName.PHARMACY_PHIEUCHI:
                    ReportModel = null;
                    ReportModel = new PaymentBillsReportModel().PreviewModel;
                    theParams["param_User"].Value = StaffFullName;
                    theParams["param_PaymentID"].Value = (int)PaymentID;
                    theParams["OutiID"].Value = (int)ID;
                    theParams["parameter_LyDo"].Value = LyDo;
                    theParams["V_TranRefType"].Value = (int)V_TranRefType;
                    break;
                case ReportName.PHARMACY_ESTIMATTION:
                    ReportModel = null;
                    ReportModel = new PharmacyEstimationModel().PreviewModel;
                    theParams["PharmacyEstimatePoID"].Value = (int)ID;
                    theParams["SupplierID"].Value = (int)SupplierID;
                    theParams["PCOID"].Value = (int)PCOID;
                    theParams["Show"].Value = LyDo;
                    theParams["GiamDoc"].Value = GiamDoc != null ? GiamDoc.FullNameString : "";
                    theParams["KeToanTruong"].Value = KeToanTruong != null ? KeToanTruong.FullNameString : "";
                    theParams["TPKeHoach"].Value = TPKeHoach != null ? TPKeHoach.FullNameString : "";
                    theParams["TruongNhaThuoc"].Value = TruongNhaThuoc != null ? TruongNhaThuoc.FullNameString : "";
                    theParams["parLogoUrl"].Value = reportLogoUrl;
                    break;
                case ReportName.PHARMACY_PHIEUKIEMKE:
                    ReportModel = null;
                    ReportModel = new InPhieuKiemKeModel().PreviewModel;
                    theParams["ID"].Value = (int)ID;
                    theParams["parLogoUrl"].Value = reportLogoUrl;
                    break;
                case ReportName.PHARMACY_PHIEUHUYHANG:
                    ReportModel = null;
                    ReportModel = new PharmacyDemageDrugReportModel().PreviewModel;
                    theParams["OutiID"].Value = (int)ID;
                    break;
                case ReportName.PHARMACY_XUATNOIBO:
                    ReportModel = null;
                    ReportModel = new OutwardInternalReportModel().PreviewModel;
                    theParams["OutiID"].Value = (int)ID;
                    theParams["parHospitalName"].Value = reportHospitalName;
                    theParams["parHospitalAddess"].Value = reportHospitalAddess;
                    break;
                case ReportName.PHARMACY_REQUESTDRUGPHARMACY:
                    ReportModel = null;
                    ReportModel = new RequestDrugPharmacyModel().PreviewModel;
                    theParams["RequestID"].Value = (int)ID;
                    break;
                case ReportName.PHARMACY_BANGKECHUNGTUTHANHTOAN:
                    ReportModel = null;
                    ReportModel = new BangKeChungTuThanhToanReportModel().PreviewModel;
                    theParams["ID"].Value = (int)ID;
                    theParams["GiamDoc"].Value = GiamDoc != null ? GiamDoc.FullNameString : "";
                    theParams["KeToanTruong"].Value = KeToanTruong !=null ? KeToanTruong.FullNameString:"";
                    theParams["PhoKhoaDuoc"].Value = PhoKhoaDuoc != null ? PhoKhoaDuoc.FullNameString : "";
                    theParams["parLogoUrl"].Value = reportLogoUrl;
                    break;
                case ReportName.PHARMACY_PHIEUDENGHITHANHTOAN:
                    ReportModel = null;
                    ReportModel = new PhieuDeNghiThanhToanReportModel().PreviewModel;
                    theParams["ID"].Value = (int)ID;
                    theParams["GiamDoc"].Value = GiamDoc != null ? GiamDoc.FullNameString : "";
                    theParams["KeToanTruong"].Value = KeToanTruong !=null ? KeToanTruong.FullNameString:"";
                    theParams["PhoKhoaDuoc"].Value = PhoKhoaDuoc != null ? PhoKhoaDuoc.FullNameString : "";
                    theParams["parLogoUrl"].Value = reportLogoUrl;
                    break;
                case ReportName.PHARMACY_PHIEUDATHANG:
                    ReportModel = null;
                    ReportModel = new PharmacyPurchaseOrderReportModel().PreviewModel;
                    theParams["ID"].Value = (int)ID;
                    theParams["parLogoUrl"].Value = reportLogoUrl;
                    break;
                case ReportName.PHARMACY_PHARMACYSUPPLIERTEMPLATE:
                    ReportModel = null;
                    ReportModel = new PharmacySupplierTemplateReportModel().PreviewModel;
                    theParams["parLogoUrl"].Value = reportLogoUrl;
                    break;
                case ReportName.PHARMACY_DUTRUDUATRENHESOANTOAN:
                    ReportModel = null;
                    ReportModel = new PharmacyDuTruDuaTrenHeSoAnToanReportModel().PreviewModel;
                    theParams["parLogoUrl"].Value = reportLogoUrl;
                    break;
                case ReportName.PHARMACY_BCHANGNGAY_NOPTIEN:
                    ReportModel = null;
                    ReportModel = new BaoCaoNopTienHangNgay().PreviewModel;
                    theParams["PharmacyOutRepID"].Value = ListID;
                    theParams["parLogoUrl"].Value = reportLogoUrl;
                    break;
                case ReportName.PHARMACY_BCHANGNGAY_NOPTIENCHITIET:
                    ReportModel = null;
                    ReportModel = new BaoCaoNopTienHangNgayChiTietModel().PreviewModel;
                    theParams["PharmacyOutRepID"].Value = ListID;
                    theParams["parLogoUrl"].Value = reportLogoUrl;
                    break;
                case ReportName.PHARMACY_BCHANGNGAY_PHATTHUOC:
                    ReportModel = null;
                    ReportModel = new BangKeChiTietPhatThuoc().PreviewModel;
                    theParams["PharmacyOutRepID"].Value = ListID;
                    theParams["IsInsurance"].Value = IsInsurance;
                    theParams["parLogoUrl"].Value = reportLogoUrl;
                    break;
                case ReportName.TEMP38a:
                    ReportModel = null;
                    ReportModel = new TransactionsTemplate38().PreviewModel;
                    theParams["TransactionID"].Value = 0;
                    theParams["PtRegistrationID"].Value = (int)ID;
                    theParams["StaffFullName"].Value = StaffFullName;
                    theParams["DieuDuongTruongKhoa"].Value = DieuDuongTruongKhoa != null ? DieuDuongTruongKhoa.FullNameString : "";
                    theParams["parHospitalName"].Value = reportHospitalName;
                    theParams["parDepartmentOfHealth"].Value = reportDepartmentOfHealth;
                    break;
                case ReportName.TEMP12:
                    ReportModel = null;
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.RptTransactions.XRpt_Temp12").PreviewModel;
                    theParams["TransactionID"].Value = 0;
                    theParams["PtRegistrationID"].Value = (int)ID;
                    theParams["FromDate"].Value = Globals.GetCurServerDateTime();
                    theParams["ToDate"].Value = Globals.GetCurServerDateTime();
                    theParams["ViewByDate"].Value = false;
                    theParams["StaffName"].Value = Globals.LoggedUserAccount != null && Globals.LoggedUserAccount.Staff != null ? Globals.LoggedUserAccount.Staff.FullName : "";
                    theParams["DeptID"].Value = 0;
                    theParams["DeptName"].Value = "";
                    theParams["RegistrationType"].Value = V_RegistrationType;
                    theParams["parHospitalName"].Value = Globals.ServerConfigSection.CommonItems.ReportHospitalName;
                    theParams["parHospitalAdress"].Value = Globals.ServerConfigSection.CommonItems.ReportHospitalAddress;
                    theParams["parDepartmentOfHealth"].Value = Globals.ServerConfigSection.CommonItems.ReportDepartmentOfHealth;
                    if (V_RegistrationType == (long)AllLookupValues.RegistrationType.NGOAI_TRU)
                    {
                        theParams["DeptName"].Value = "Khoa Khám Bệnh";
                    }
                    break;
                //▼====: #002
                case ReportName.PHIEUNHANTHUOC_THERMAL:
                    ReportModel = null;
                    ReportModel = new PharmacyCollectionDrugThermalReportModel().PreviewModel;
                    theParams["OutiID"].Value = ID;
                    break;
                case ReportName.PHIEUNHANTHUOC_BHYT_THERMAL:
                    ReportModel = null;
                    ReportModel = new PharmacyCollectionDrugBHYTThermalReportModel().PreviewModel;
                    theParams["OutiID"].Value = ID;
                    theParams["parHospitalName"].Value = reportHospitalName;
                    break;
                case ReportName.PHIEUNHANTHUOC_SUMMARY_THERMAL:
                    ReportModel = null;
                    ReportModel = new PharmacyCollectionDrugSummaryXMLThermalReportModel().PreviewModel;
                    theParams["OutiID"].Value = ListID;
                    theParams["parHospitalName"].Value = reportHospitalName;
                    break;
                //▲====: #002
                //▼====: #003
                case ReportName.Pharmacy_InwardFromInternal:
                    ReportModel = null;
                    ReportModel = new InwardFromInternalForPharmacyReportModel().PreviewModel;
                    theParams["InvID"].Value = ID;
                    break;
                //▲====: #003
                //▼====: #003
                case ReportName.TEMP12_TONGHOP:
                    ReportModel = null;
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.RptTransactions.XRpt_Temp12_TongHop").PreviewModel;
                    theParams["TransactionID"].Value = 0;
                    theParams["PtRegistrationID"].Value = (int)ID;
                    theParams["FromDate"].Value = Globals.GetCurServerDateTime();
                    theParams["ToDate"].Value = Globals.GetCurServerDateTime();
                    theParams["ViewByDate"].Value = false;
                    theParams["StaffName"].Value = Globals.LoggedUserAccount != null && Globals.LoggedUserAccount.Staff != null ? Globals.LoggedUserAccount.Staff.FullName : "";
                    theParams["DeptID"].Value = 0;
                    theParams["DeptName"].Value = "";
                    theParams["RegistrationType"].Value = V_RegistrationType;
                    theParams["parHospitalAdress"].Value = Globals.ServerConfigSection.CommonItems.ReportHospitalAddress;
                    theParams["parHospitalName"].Value = Globals.FirstCharToUpper(Globals.ServerConfigSection.CommonItems.ReportHospitalName.ToLower());
                    theParams["parDepartmentOfHealth"].Value = Globals.FirstCharToUpper(Globals.ServerConfigSection.CommonItems.ReportDepartmentOfHealth.ToLower());
                    if (V_RegistrationType == (long)AllLookupValues.RegistrationType.NGOAI_TRU)
                    {
                        theParams["DeptName"].Value = "Khoa Khám Bệnh";
                    }
                    if (IsPatientCOVID)
                    {
                        theParams["IsPatientCOVID"].Value = IsPatientCOVID;
                    }
                    break;
                case ReportName.TEMP12_6556:
                    ReportModel = null;
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.RptTransactions.XRpt_Temp12_6556").PreviewModel;
                    theParams["TransactionID"].Value = 0;
                    theParams["PtRegistrationID"].Value = (int)ID;
                    theParams["FromDate"].Value = Globals.GetCurServerDateTime();
                    theParams["ToDate"].Value = Globals.GetCurServerDateTime();
                    theParams["ViewByDate"].Value = false;
                    theParams["StaffName"].Value = Globals.LoggedUserAccount != null && Globals.LoggedUserAccount.Staff != null ? Globals.LoggedUserAccount.Staff.FullName : "";
                    theParams["DeptID"].Value = 0;
                    theParams["DeptName"].Value = "";
                    theParams["RegistrationType"].Value = V_RegistrationType;
                    theParams["parHospitalName"].Value = Globals.FirstCharToUpper(Globals.ServerConfigSection.CommonItems.ReportHospitalName.ToLower());
                    theParams["parHospitalAdress"].Value = Globals.FirstCharToUpper(Globals.ServerConfigSection.CommonItems.ReportHospitalAddress.ToLower());
                    theParams["parDepartmentOfHealth"].Value = Globals.FirstCharToUpper(Globals.ServerConfigSection.CommonItems.ReportDepartmentOfHealth.ToLower());
                    if (V_RegistrationType == (long)AllLookupValues.RegistrationType.NGOAI_TRU)
                    {
                        theParams["DeptName"].Value = "Khoa Khám Bệnh";
                    }
                    break;
                //▲====: #003
                //▼==== #004
                case ReportName.Huong_Dan_Su_Dung_Thuoc:
                    ReportModel = null;
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.RptPharmacies.RptHuongDanDungThuocPaging").PreviewModel;
                    theParams["PtRegistrationID"].Value = ID;
                    theParams["BeOfHIMedicineList"].Value = IsInsurance;
                    break;
                //▲==== #004
                //▲==== #005
                case ReportName.PhieuSoanThuocPaging:
                    ReportModel = null;
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.RptPharmacies.PhieuSoanThuocPaging").PreviewModel;
                    theParams["PtRegistrationID"].Value = ID;
                    theParams["StaffName"].Value = Globals.LoggedUserAccount != null && Globals.LoggedUserAccount.Staff != null ? Globals.LoggedUserAccount.Staff.FullName : "";
                    theParams["parHospitalName"].Value = reportHospitalName;
                    break;
                //▲==== #005
            }

            //ReportModel.RequestDefaultParameterValues += new WeakEventHandler<EventArgs>(_reportModel_RequestDefaultParameterValues).Handler;
            // ReportModel.AutoShowParametersPanel = false;
            ReportModel.CreateDocument(theParams);
        }

        public void btnClose()
        {
            TryClose();
        }
    }
}