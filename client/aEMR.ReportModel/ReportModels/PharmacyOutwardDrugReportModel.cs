using aEMR.ReportModel.BaseModels;
/*
* 20181103 #001 TNHX: [BM0005214] Create PhieuNhanThuocSummary_XML && PhieuNhanThuocSummar to use when RefApplicationConfig.MixedHIPharmacyStores turn on
* 20190308 #002 TNHX: Create XRpt_BCThuTienNTTaiQuayBHYT
* 20190424 #003 TNHX: [BM0006716] Create PhieuNhanThuoc for Thermal
* 20190503 #004 TNHX: [BM0006812] [BM0006813] Create XRpt_TKTheoDoiTTChiTietKH_NT, TKTheoDoiNXTThuocKhac_NT
* 20190610 #005 TNHX: [BM0010768] Create XRpt_BCXuatDuocNoiBo_NT
* 20190704 #006 TNHX: [BM0011926] Create XRpt_BCBanThuocLe
* 20190929 #007 TNHX: [BM0017380] Create XRptInwardFromInternalForPharmacy
*/
namespace aEMR.ReportModel.ReportModels
{
    //▼====: #007
    public class InwardFromInternalForPharmacyReportModel : ReportModelBase
    {
        public InwardFromInternalForPharmacyReportModel()
            : base("eHCMS.ReportLib.RptPharmacies.XtraReports.Inwards.XRptInwardFromInternalForPharmacy")
        {

        }
    }
    //▲====: #007

    public class PharmacyCollectionDrugReportModel : ReportModelBase
    {
        public PharmacyCollectionDrugReportModel()
            : base("eHCMS.ReportLib.RptPharmacies.PhieuNhanThuoc")
        {

        }
    }

    public class PharmacyCollectionDrugPrivateReportModel : ReportModelBase
    {
        public PharmacyCollectionDrugPrivateReportModel()
            : base("eHCMS.ReportLib.RptPharmacies.PhieuNhanThuocPrivate")
        {

        }
    }

    public class PharmacyCollectionDrugBHReportModel : ReportModelBase
    {
        public PharmacyCollectionDrugBHReportModel()
            : base("eHCMS.ReportLib.RptPharmacies.PhieuNhanThuocBH")
        {

        }
    }

    /*▼====: #001*/
    public class PharmacyCollectionDrugSummaryReportModel : ReportModelBase
    {
        public PharmacyCollectionDrugSummaryReportModel()
            : base("eHCMS.ReportLib.RptPharmacies.PhieuNhanThuocSummary")
        {
        }
    }

    public class PharmacyCollectionDrugSummaryXMLReportModel : ReportModelBase
    {
        public PharmacyCollectionDrugSummaryXMLReportModel()
            : base("eHCMS.ReportLib.RptPharmacies.PhieuNhanThuocSummary_XML")
        {
        }
    }
    /*▲====: #001*/
    //▼====: #003
    public class PharmacyCollectionDrugThermalReportModel : ReportModelBase
    {
        public PharmacyCollectionDrugThermalReportModel()
            : base("eHCMS.ReportLib.RptPharmacies.PhieuNhanThuocInNhiet")
        {

        }
    }

    public class PharmacyCollectionDrugBHYTThermalReportModel : ReportModelBase
    {
        public PharmacyCollectionDrugBHYTThermalReportModel()
            : base("eHCMS.ReportLib.RptPharmacies.PhieuNhanThuocBHYTInNhiet")
        {

        }
    }

    public class PharmacyCollectionDrugSummaryXMLThermalReportModel : ReportModelBase
    {
        public PharmacyCollectionDrugSummaryXMLThermalReportModel()
            : base("eHCMS.ReportLib.RptPharmacies.PhieuNhanThuocSummary_XML_InNhiet")
        {
        }
    }
    //▲====: #003

    public class PaymentVisistorReportModel : ReportModelBase
    {
        public PaymentVisistorReportModel()
            : base("eHCMS.ReportLib.RptPharmacies.XRptPatientPaymentVisitor")
        {

        }
    }
    public class PaymentBillsReportModel : ReportModelBase
    {
        public PaymentBillsReportModel()
            : base("eHCMS.ReportLib.RptPharmacies.XRptPaymentBills")
        {

        }
    }
    public class PharmacyDemageDrugReportModel : ReportModelBase
    {
        public PharmacyDemageDrugReportModel()
            : base("eHCMS.ReportLib.RptPharmacies.XRpt_PharmacyDemageDrug")
        {

        }
    }
    public class OutwardInternalReportModel : ReportModelBase
    {
        public OutwardInternalReportModel()
            //: base("eHCMS.ReportLib.RptPharmacies.XRpt_XuatNoiBo")
            : base("eHCMS.ReportLib.RptPharmacies.XtraReports.OutInternal.XRpt_XuatNoiBo_KT")
        {

        }
    }

    public class XuatNoiBoTheoNguoiMuaReportModel : ReportModelBase
    {
        public XuatNoiBoTheoNguoiMuaReportModel()
            : base("eHCMS.ReportLib.RptPharmacies.XtraReports.OutInternal.XRpt_XuatNoiBoTheoNguoiMua")
        {

        }
    }

    public class XuatNoiBoTheoTenThuocReportModel : ReportModelBase
    {
        public XuatNoiBoTheoTenThuocReportModel()
            : base("eHCMS.ReportLib.RptPharmacies.XtraReports.OutInternal.XRpt_XuatNoiBoTheoTenThuoc")
        {

        }
    }

    public class XuatThuocChoBHReportModel : ReportModelBase
    {
        public XuatThuocChoBHReportModel()
            : base("eHCMS.ReportLib.RptPharmacies.XtraReports.OutInsurance.XRpt_XuatBH")
        {

        }
    }
    public class BanThuocTongHopReportModel : ReportModelBase
    {
        public BanThuocTongHopReportModel()
            : base("eHCMS.ReportLib.RptPharmacies.XtraReports.ReportGeneral.XRpt_SellDrugGeneral")
        {

        }
    }
    public class TraThuocTongHopReportModel : ReportModelBase
    {
        public TraThuocTongHopReportModel()
            : base("eHCMS.ReportLib.RptPharmacies.XtraReports.ReportGeneral.XRpt_ReturnDrugGeneral")
        {

        }
    }

    //▼====: #002
    public class XRpt_BCThuTienNTTaiQuayBHYT : ReportModelBase
    {
        public XRpt_BCThuTienNTTaiQuayBHYT()
            : base("eHCMS.ReportLib.RptPharmacies.XtraReports.ReportGeneral.XRpt_BCThuTienNTTaiQuayBHYT")
        {
        }
    }

    public class XRpt_BCThuTienNTTaiQuayBHYTDetail : ReportModelBase
    {
        public XRpt_BCThuTienNTTaiQuayBHYTDetail()
            : base("eHCMS.ReportLib.RptPharmacies.XtraReports.ReportGeneral.XRpt_BCThuTienNTTaiQuayBHYT_Detail")
        {
        }
    }
    public class XRpt_BCThuTienNTTaiQuayBHYT_TheoBienLai : ReportModelBase
    {
        public XRpt_BCThuTienNTTaiQuayBHYT_TheoBienLai()
            : base("eHCMS.ReportLib.RptPharmacies.XtraReports.ReportGeneral.XRpt_BCThuTienNTTaiQuayBHYT_TheoBienLai")
        {
        }
    }
    //▲====: #002
    //▼====: #004
    public class XRpt_TKTheoDoiTTChiTietKH_NT : ReportModelBase
    {
        public XRpt_TKTheoDoiTTChiTietKH_NT()
            : base("eHCMS.ReportLib.RptPharmacies.XtraReports.MinistryOfHealth.XRpt_TKTheoDoiTTChiTietKH_NT")
        {
        }
    }

    public class XRpt_TKTheoDoiNXTThuocKhac_NT : ReportModelBase
    {
        public XRpt_TKTheoDoiNXTThuocKhac_NT()
            : base("eHCMS.ReportLib.RptPharmacies.XtraReports.MinistryOfHealth.XRpt_TKTheoDoiNXTThuocKhac_NT")
        {
        }
    }
    //▲====: #004
    //▼====: #005
    public class XRpt_BCXuatDuocNoiBo_NT : ReportModelBase
    {
        public XRpt_BCXuatDuocNoiBo_NT()
            : base("eHCMS.ReportLib.RptPharmacies.XtraReports.ReportGeneral.XRpt_BCXuatDuocNoiBo_NT")
        {
        }
    }
    //▲====: #005
    //▼====: #006
    public class XRpt_BCBanThuocLe : ReportModelBase
    {
        public XRpt_BCBanThuocLe()
            : base("eHCMS.ReportLib.RptPharmacies.XtraReports.ReportGeneral.XRpt_BCBanThuocLe")
        {
        }
    }
    //▲====: #006
}
