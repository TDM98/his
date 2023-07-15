using aEMR.ReportModel.BaseModels;
/*
 * 20171226 #001 CMN: Added Temp Inward Report
 * 20190514 #002 TNHX:  BM 0006872: Create BC_KhangSinh report, BC_SuDungThuoc, BC_SuDungHoaChat
 * 20190620 #003 TNHX: [BM0011869] Them report phieu nhap tra tu kho khoa phong (mau, hoa chat, tiem ngua, VTYTTH, thanh trung)
 * 20190827 #004 TNHX: [BM0013276] Create report InOut of DrugDept + ClinicDept for Accountant
 * 20210831 #006 QTD: XRptEstimateDrugDept_V2
 * 20230110 #007 QTD: Report tổng hợp dự trù từ phiếu lĩnh
*/
namespace aEMR.ReportModel.ReportModels
{
    //▼====: #005
    public class DrugDeptCardStorageReportModal_KT : ReportModelBase
    {
        public DrugDeptCardStorageReportModal_KT()
            : base("eHCMS.ReportLib.RptTransactions.XtraReports.Accountant.XRptDrugDeptCardStorage")
        {
        }
    }

    public class ClinicDeptCardStorageReportModal_KT : ReportModelBase
    {
        public ClinicDeptCardStorageReportModal_KT()
            : base("eHCMS.ReportLib.RptTransactions.XtraReports.Accountant.XRptClinicDeptCardStorage")
        {
        }
    }

    public class PharmacyCardStorageReportModal_KT : ReportModelBase
    {
        public PharmacyCardStorageReportModal_KT()
            : base("eHCMS.ReportLib.RptTransactions.XtraReports.Accountant.XRptPharmacyCardStorage")
        {
        }
    }
    //▲====: #005
    //▼====: #004
    public class DrugDeptInOutStockValuesReportModal_KT : ReportModelBase
    {
        public DrugDeptInOutStockValuesReportModal_KT()
            : base("eHCMS.ReportLib.RptDrugDepts.XtraReports.Reports.XRptInOutStockValueDrugDept_KT")
        {
        }
    }

    public class ClinicDeptInOutStocksReportModal_KT : ReportModelBase
    {
        public ClinicDeptInOutStocksReportModal_KT()
            : base("eHCMS.ReportLib.RptDrugDepts.XtraReports.Reports.XRptInOutStockValueClinicDept_KT")
        {
        }
    }

    public class PharmacyInOutStocksReportModal_KT : ReportModelBase
    {
        public PharmacyInOutStocksReportModal_KT()
            : base("eHCMS.ReportLib.RptPharmacies.XRptInOutStockValue_KT")
        {
        }
    }
    //▲====: #004

    public class DrugDeptRequestReportModal : ReportModelBase
    {
        public DrugDeptRequestReportModal()
            : base("eHCMS.ReportLib.RptDrugDepts.XtraReports.Request.XRptRequestDrugDept")
        {

        }
    }
    public class DrugDeptRequestApprovedReportModal : ReportModelBase
    {
        public DrugDeptRequestApprovedReportModal()
            : base("eHCMS.ReportLib.RptDrugDepts.XtraReports.Request.XRptRequestDrugDeptApproved")
        {

        }
    }
    public class DrugDeptRequestDetailReportModal : ReportModelBase
    {
        public DrugDeptRequestDetailReportModal()
            : base("eHCMS.ReportLib.RptDrugDepts.XtraReports.Request.XRptRequestDrugDeptDetails")
        {

        }
    }
    public class DrugDeptRequestDetailApprovedReportModal : ReportModelBase
    {
        public DrugDeptRequestDetailApprovedReportModal()
            : base("eHCMS.ReportLib.RptDrugDepts.XtraReports.Request.XRptRequestDrugDeptDetailApproved")
        {

        }
    }
    public class DrugDeptOutInternalReportModal : ReportModelBase
    {
        public DrugDeptOutInternalReportModal()
            : base("eHCMS.ReportLib.RptDrugDepts.XtraReports.OutInternal.XRptOutInternalDrugDept")
        {

        }
    }
    public class DrugDeptOutInternalToClinicDeptReportModal : ReportModelBase
    {
        //public DrugDeptOutInternalToClinicDeptReportModal()
        //    : base("eHCMS.ReportLib.RptDrugDepts.XtraReports.OutInternal.XRptOutInternalDrugDeptToClinicDept")
        public DrugDeptOutInternalToClinicDeptReportModal(string reportName)
            : base(reportName)
        {

        }
    }
    public class DrugDeptOutDrugDeptAddictiveReportModal : ReportModelBase
    {
        public DrugDeptOutDrugDeptAddictiveReportModal()
            : base("eHCMS.ReportLib.RptDrugDepts.XtraReports.OutInternal.XRptOutDrugDeptAddictive")
        {

        }
    }
    public class DrugDeptOutDemageReportModal : ReportModelBase
    {
        public DrugDeptOutDemageReportModal()
            : base("eHCMS.ReportLib.RptDrugDepts.XtraReports.OutInternal.XRptOutDemageDrugDept")
        {

        }
    }
    public class DrugDeptEstimationReportModal : ReportModelBase
    {
        public DrugDeptEstimationReportModal()
            : base("eHCMS.ReportLib.RptDrugDepts.XtraReports.Estimation.XRptEstimateDrugDept")
        {

        }
    }
    public class DrugDeptEstimationKeToanReportModal : ReportModelBase
    {
        public DrugDeptEstimationKeToanReportModal()
            : base("eHCMS.ReportLib.RptDrugDepts.XtraReports.Estimation.XRptEstimateDrugDeptKeToan")
        {

        }
    }
    public class DrugDeptEstimationThuKhoReportModal : ReportModelBase
    {
        public DrugDeptEstimationThuKhoReportModal()
            : base("eHCMS.ReportLib.RptDrugDepts.XtraReports.Estimation.XRptEstimateDrugDeptThuKho")
        {

        }
    }
    public class DrugDeptCardStorageReportModal : ReportModelBase
    {
        public DrugDeptCardStorageReportModal()
            : base("eHCMS.ReportLib.RptDrugDepts.XtraReports.Reports.XRptCardStorageDrugDept")
        {

        }
    }
    public class ClinicDeptCardStorageReportModal : ReportModelBase
    {
        public ClinicDeptCardStorageReportModal()
            : base("eHCMS.ReportLib.RptDrugDepts.XtraReports.Reports.XRptCardStorageClinicDept")
        {

        }
    }
    public class DrugDeptReturnMedDeptReportModal : ReportModelBase
    {
        public DrugDeptReturnMedDeptReportModal()
            : base("eHCMS.ReportLib.RptDrugDepts.XtraReports.Returns.XptReturnMedDept")
        {

        }
    }

    public class DrugDeptInOutStocksReportModal : ReportModelBase
    {
        public DrugDeptInOutStocksReportModal()
            : base("eHCMS.ReportLib.RptDrugDepts.XtraReports.Reports.XRptInOutStockDrugDept")
        {

        }
    }

    public class DrugDeptInOutStockValuesReportModal : ReportModelBase
    {
        public DrugDeptInOutStockValuesReportModal()
            : base("eHCMS.ReportLib.RptDrugDepts.XtraReports.Reports.XRptInOutStockValueDrugDept")
        {

        }
    }

    //▼====: #004
    public class DrugDeptInOutStockValuesDetailsReportModal_KT : ReportModelBase
    {
        public DrugDeptInOutStockValuesDetailsReportModal_KT()
            : base("eHCMS.ReportLib.RptDrugDepts.XtraReports.Reports.XRptInOutStockValueDrugDeptDetails_KT")
        {

        }
    }
    //▲====: #004
    public class DrugDeptInOutStockValuesReportModal_TV : ReportModelBase
    {
        public DrugDeptInOutStockValuesReportModal_TV()
            : base("eHCMS.ReportLib.RptDrugDepts.XtraReports.Reports.XRptInOutStockValueDrugDept_TV")
        {

        }
    }

    public class ClinicDeptInOutStocksReportModal : ReportModelBase
    {
        public ClinicDeptInOutStocksReportModal()
            : base("eHCMS.ReportLib.RptDrugDepts.XtraReports.Reports.XRptInOutStockClinicDept")
        {

        }
    }

    public class ClinicDeptInOutStockValueReportModal : ReportModelBase
    {
        public ClinicDeptInOutStockValueReportModal()
            : base("eHCMS.ReportLib.RptDrugDepts.XtraReports.Reports.XRptInOutStockValueClinicDept_KT")
        {

        }
    }

    //▼====: #004
    public class ClinicDeptInOutStocksDetailsReportModal_KT : ReportModelBase
    {
        public ClinicDeptInOutStocksDetailsReportModal_KT()
            : base("eHCMS.ReportLib.RptDrugDepts.XtraReports.Reports.XRptInOutStockClinicDeptDetails_KT")
        {

        }
    }
    //▲====: #004

    public class DrugDepOrderReportModal : ReportModelBase
    {
        public DrugDepOrderReportModal()
            : base("eHCMS.ReportLib.RptDrugDepts.XtraReports.Order.XRpt_PurchaseOrderDrugDept")
        {

        }
    }
    public class DrugDepInwardFromMedToClinicReportModal : ReportModelBase
    {
        public DrugDepInwardFromMedToClinicReportModal()
            : base("eHCMS.ReportLib.RptDrugDepts.XtraReports.Inwards.XRptInwardFromMedDeptForClinicDept")
        {

        }
    }
    public class DrugDepInwardMedDeptFromClinicReportModal : ReportModelBase
    {
        public DrugDepInwardMedDeptFromClinicReportModal()
            : base("eHCMS.ReportLib.RptDrugDepts.XtraReports.Inwards.XRptInwardMedDeptFromClinicDept")
        {
        }
    }

    //▼====: #003
    public class DrugDepInwardBloodMedDeptFromClinicReportModal : ReportModelBase
    {
        public DrugDepInwardBloodMedDeptFromClinicReportModal()
            : base("eHCMS.ReportLib.RptDrugDepts.XtraReports.Inwards.XRptInwardBloodMedDeptFromClinicDept")
        {
        }
    }
    public class DrugDepInwardChemicalMedDeptFromClinicReportModal : ReportModelBase
    {
        public DrugDepInwardChemicalMedDeptFromClinicReportModal()
            : base("eHCMS.ReportLib.RptDrugDepts.XtraReports.Inwards.XRptInwardChemicalMedDeptFromClinicDept")
        {
        }
    }
    public class DrugDepInwardThanhTrungMedDeptFromClinicReportModal : ReportModelBase
    {
        public DrugDepInwardThanhTrungMedDeptFromClinicReportModal()
            : base("eHCMS.ReportLib.RptDrugDepts.XtraReports.Inwards.XRptInwardThanhTrungMedDeptFromClinicDept")
        {
        }
    }

    public class DrugDepInwardTiemNguaMedDeptFromClinicReportModal : ReportModelBase
    {
        public DrugDepInwardTiemNguaMedDeptFromClinicReportModal()
            : base("eHCMS.ReportLib.RptDrugDepts.XtraReports.Inwards.XRptInwardTiemNguaMedDeptFromClinicDept")
        {
        }
    }

    public class DrugDepInwardVTYTTHMedDeptFromClinicReportModal : ReportModelBase
    {
        public DrugDepInwardVTYTTHMedDeptFromClinicReportModal()
            : base("eHCMS.ReportLib.RptDrugDepts.XtraReports.Inwards.XRptInwardVTYTTHMedDeptFromClinicDept")
        {
        }
    }
    //▲====: #003

    public class DrugDepInwardMedDeptSupplierReportModal : ReportModelBase
    {
        public DrugDepInwardMedDeptSupplierReportModal()
            : base("eHCMS.ReportLib.RptDrugDepts.XtraReports.Inwards.XRptInwardMedDeptSupplier")
        {

        }
    }

    public class DrugDepInwardMedDeptSupplierTrongNuocReportModal : ReportModelBase
    {
        public DrugDepInwardMedDeptSupplierTrongNuocReportModal()
            : base("eHCMS.ReportLib.RptDrugDepts.XtraReports.Inwards.XRptInwardMedDeptSupplierTrongNuoc")
        {

        }
    }
    public class DrugDepInwardCostTableReportModal : ReportModelBase
    {
        public DrugDepInwardCostTableReportModal()
            : base("eHCMS.ReportLib.RptDrugDepts.XtraReports.Inwards.XRptInwardCostTable")
        {

        }
    }

    public class DrugDepSuggestPaymentReportModal : ReportModelBase
    {
        public DrugDepSuggestPaymentReportModal()
            : base("eHCMS.ReportLib.RptDrugDepts.XtraReports.SupplierDrugDeptPaymentReqs.PhieuDeNghiThanhToan")
        {

        }
    }

    public class DrugDepSupplierPaymentReportModal : ReportModelBase
    {
        public DrugDepSupplierPaymentReportModal()
            : base("eHCMS.ReportLib.RptDrugDepts.XtraReports.SupplierDrugDeptPaymentReqs.SupplierDrugDeptPaymentReqs")
        {

        }
    }

    public class RptBangGiaThuocYCuHoaChatKhoaDuoc_AutoCreate : ReportModelBase
    {
        public RptBangGiaThuocYCuHoaChatKhoaDuoc_AutoCreate()
            : base("eHCMS.ReportLib.RptDrugDepts.XtraReports.Managers.DrugDeptSellingPriceList.XRptDrugDeptSellingPriceList_AutoCreate")
        {

        }
    }

    public class RptBangGiaThuocKhoaDuoc : ReportModelBase
    {
        public RptBangGiaThuocKhoaDuoc()
            : base("eHCMS.ReportLib.RptDrugDepts.XtraReports.Managers.DrugDeptSellingPriceList.XRptDrugDeptSellingPriceList_Detail")
        {

        }
    }

    public class RptBangGiaYCuHoaChatKhoaDuoc : ReportModelBase
    {
        public RptBangGiaYCuHoaChatKhoaDuoc()
            : base("eHCMS.ReportLib.RptDrugDepts.XtraReports.Managers.DrugDeptSellingPriceList.XRptDrugDeptSellingPriceListYCuHoaChat_Detail")
        {

        }
    }

    public class RptBangGiaYCuHoaChatKhoaDuoc_AutoCreate : ReportModelBase
    {
        public RptBangGiaYCuHoaChatKhoaDuoc_AutoCreate()
            : base("eHCMS.ReportLib.RptDrugDepts.XtraReports.Managers.DrugDeptSellingPriceList.XRptDrugDeptSellingPriceListYCuHoaChat_Detail_AutoCreate")
        {

        }
    }

    /*RptDrugMedDept TMA 23/10/2017*/
    public class RptDrugMedDept : ReportModelBase
    {
        public RptDrugMedDept()
            : base("eHCMS.ReportLib.RptDrugDepts.XtraReports.Inwards.XRptDrugMedDept")
        {

        }
    }
    /*RptDrugMedDept*/

    /*▼====: #001*/
    public class RptDrugDeptTempInwardReport : ReportModelBase
    {
        public RptDrugDeptTempInwardReport()
            : base("eHCMS.ReportLib.RptDrugDepts.XtraReports.Inwards.XRptDrugDeptTempInwardReport")
        {
        }
    }
    /*▲====: #001*/

    public class RptDanhSachXuatKhoaDuoc: ReportModelBase
    {
        public RptDanhSachXuatKhoaDuoc()
            : base("eHCMS.ReportLib.RptDrugDepts.XtraReports.Outward.XRptDanhSachXuatKhoaDuoc")
        {

        }
    }

    public class RptClinicNhapXuatTon : ReportModelBase
    {
        public RptClinicNhapXuatTon()
            : base("eHCMS.ReportLib.RptDrugDepts.XtraReports.Outward.XRptDanhSachXuatKhoaDuoc")
        {

        }
    }

    public class DrugDeptNhapHangHangThangMedDeptInvoiceReportModal : ReportModelBase
    {
        public DrugDeptNhapHangHangThangMedDeptInvoiceReportModal()
            : base(" eHCMS.ReportLib.RptDrugDepts.XtraReports.Reports.BaoCaoNhapHangHangThangMedDeptInvoice")
        {
   
        }
    }

    public class DrugDeptInOutStockAddictiveReportModal : ReportModelBase
    {
        public DrugDeptInOutStockAddictiveReportModal()
            : base(" eHCMS.ReportLib.RptDrugDepts.XtraReports.Reports.XRptInOutStockDrugDeptAddictiveNew")
        {

        }
    }

    public class DrugDeptTheoDoiCongNoReportModal : ReportModelBase
    {
        public DrugDeptTheoDoiCongNoReportModal()
            : base(" eHCMS.ReportLib.RptDrugDepts.XtraReports.Reports.BaoCaoTheoDoiCongNoMedDept")
        {

        }
    }

    public class RptDrugDeptStockTakes_Get_Thuoc : ReportModelBase
    {
        public RptDrugDeptStockTakes_Get_Thuoc()
            : base("eHCMS.ReportLib.RptDrugDepts.XtraReports.StockTakes.XRptDrugDeptStockTakes_Get_Thuoc")
        {

        }
    }
    public class RptDrugDeptStockTakes_Get_YCu : ReportModelBase
    {
        public RptDrugDeptStockTakes_Get_YCu()
            : base("eHCMS.ReportLib.RptDrugDepts.XtraReports.StockTakes.XRptDrugDeptStockTakes_Get_YCu")
        {

        }
    }

    public class RptDrugDeptStockTakes_Get_HoaChat : ReportModelBase
    {
        public RptDrugDeptStockTakes_Get_HoaChat()
            : base("eHCMS.ReportLib.RptDrugDepts.XtraReports.StockTakes.XRptDrugDeptStockTakes_Get_HoaChat")
        {

        }
    }

    public class DrugDeptPhieuKiemKeThuocReportModel : ReportModelBase
    {
        public DrugDeptPhieuKiemKeThuocReportModel()
            : base("eHCMS.ReportLib.RptDrugDepts.XtraReports.StockTakes.XRptPhieuKiemKeThuoc")
        {

        }
    }
    public class DrugDeptPhieuKiemKeYCuReportModel : ReportModelBase
    {
        public DrugDeptPhieuKiemKeYCuReportModel()
            : base("eHCMS.ReportLib.RptDrugDepts.XtraReports.StockTakes.XRptPhieuKiemKeYCu")
        {

        }
    }
    public class DrugDeptPhieuKiemKeHoaChatReportModel : ReportModelBase
    {
        public DrugDeptPhieuKiemKeHoaChatReportModel()
            : base("eHCMS.ReportLib.RptDrugDepts.XtraReports.StockTakes.XRptPhieuKiemKeHoaChat")
        {

        }
    }

    public class DrugDeptRequesHIStoretReportModel : ReportModelBase
    {
        public DrugDeptRequesHIStoretReportModel()
            : base("eHCMS.ReportLib.RptDrugDepts.XtraReports.Request.XRptRequestHIStore")
        {

        }
    }

    public class DrugDeptRequestHIStoreApprovedReportModel : ReportModelBase
    {
        public DrugDeptRequestHIStoreApprovedReportModel()
            : base("eHCMS.ReportLib.RptDrugDepts.XtraReports.Request.XRptRequestHIStoreApproved")
        {

        }
    }
    public class DrugDeptRequesHIStoretDetailsReportModel : ReportModelBase
    {
        public DrugDeptRequesHIStoretDetailsReportModel()
            : base("eHCMS.ReportLib.RptDrugDepts.XtraReports.Request.XRptRequestHIStoreDetails")
        {

        }
    }
    public class DrugDeptRequestHIStoreDetailsApprovedReportModel : ReportModelBase
    {
        public DrugDeptRequestHIStoreDetailsApprovedReportModel()
            : base("eHCMS.ReportLib.RptDrugDepts.XtraReports.Request.XRptRequestHIStoreDetailApproved")
        {

        }
    }
    public class DrugDepInwardVTYTTHSupplierTrongNuocReportModel : ReportModelBase
    {
        public DrugDepInwardVTYTTHSupplierTrongNuocReportModel()
            : base("eHCMS.ReportLib.RptDrugDepts.XtraReports.Inwards.XRptInwardVTYTTHSupplierTrongNuoc")
        {

        }
    }
    public class DrugDepInwardVTYTTHSupplierReportModel : ReportModelBase
    {
        public DrugDepInwardVTYTTHSupplierReportModel()
            : base("eHCMS.ReportLib.RptDrugDepts.XtraReports.Inwards.XRptInwardVTYTTHSupplier")
        {

        }
    }

    public class DrugDepInwardTiemNguaSupplierTrongNuocReportModel : ReportModelBase
    {
        public DrugDepInwardTiemNguaSupplierTrongNuocReportModel()
            : base("eHCMS.ReportLib.RptDrugDepts.XtraReports.Inwards.XRptInwardTiemNguaSupplierTrongNuoc")
        {

        }
    }
    public class DrugDepInwardTiemNguaSupplierReportModel : ReportModelBase
    {
        public DrugDepInwardTiemNguaSupplierReportModel()
            : base("eHCMS.ReportLib.RptDrugDepts.XtraReports.Inwards.XRptInwardTiemNguaSupplier")
        {

        }
    }
    public class DrugDepInwardChemicalSupplierTrongNuocReportModel : ReportModelBase
    {
        public DrugDepInwardChemicalSupplierTrongNuocReportModel()
            : base("eHCMS.ReportLib.RptDrugDepts.XtraReports.Inwards.XRptInwardChemicalSupplierTrongNuoc")
        {

        }
    }
    public class DrugDepInwardChemicalSupplierReportModel : ReportModelBase
    {
        public DrugDepInwardChemicalSupplierReportModel()
            : base("eHCMS.ReportLib.RptDrugDepts.XtraReports.Inwards.XRptInwardChemicalSupplier")
        {

        }
    }

    public class DrugDepInwardBloodSupplierTrongNuocReportModel : ReportModelBase
    {
        public DrugDepInwardBloodSupplierTrongNuocReportModel()
            : base("eHCMS.ReportLib.RptDrugDepts.XtraReports.Inwards.XRptInwardBloodSupplierTrongNuoc")
        {

        }
    }
    public class DrugDepInwardBloodSupplierReportModel : ReportModelBase
    {
        public DrugDepInwardBloodSupplierReportModel()
            : base("eHCMS.ReportLib.RptDrugDepts.XtraReports.Inwards.XRptInwardBloodSupplier")
        {

        }
    }

    public class DrugDepInwardThanhTrungSupplierTrongNuocReportModel : ReportModelBase
    {
        public DrugDepInwardThanhTrungSupplierTrongNuocReportModel()
            : base("eHCMS.ReportLib.RptDrugDepts.XtraReports.Inwards.XRptInwardThanhTrungSupplierTrongNuoc")
        {

        }
    }
    public class DrugDepInwardThanhTrungSupplierReportModel : ReportModelBase
    {
        public DrugDepInwardThanhTrungSupplierReportModel()
            : base("eHCMS.ReportLib.RptDrugDepts.XtraReports.Inwards.XRptInwardThanhTrungSupplier")
        {

        }
    }
    public class DrugDepInwardVPPSupplierTrongNuocReportModel : ReportModelBase
    {
        public DrugDepInwardVPPSupplierTrongNuocReportModel()
            : base("eHCMS.ReportLib.RptDrugDepts.XtraReports.Inwards.XRptInwardVPPSupplierTrongNuoc")
        {

        }
    }
    public class DrugDepInwardVPPSupplierReportModel : ReportModelBase
    {
        public DrugDepInwardVPPSupplierReportModel()
            : base("eHCMS.ReportLib.RptDrugDepts.XtraReports.Inwards.XRptInwardVPPSupplier")
        {

        }
    }
    public class DrugDepInwardVTTHSupplierTrongNuocReportModel : ReportModelBase
    {
        public DrugDepInwardVTTHSupplierTrongNuocReportModel()
            : base("eHCMS.ReportLib.RptDrugDepts.XtraReports.Inwards.XRptInwardVTTHSupplierTrongNuoc")
        {

        }
    }
    public class DrugDepInwardVTTHSupplierReportModel : ReportModelBase
    {
        public DrugDepInwardVTTHSupplierReportModel()
            : base("eHCMS.ReportLib.RptDrugDepts.XtraReports.Inwards.XRptInwardVTTHSupplier")
        {

        }
    }

    public class InOutStocksDrugsGeneralReportModal : ReportModelBase
    {
        public InOutStocksDrugsGeneralReportModal()
            : base("eHCMS.ReportLib.RptDrugDepts.XtraReports.Reports.XRptInOutStockGeneral")
        {

        }
    }

    //▼====: QTD NXT Thuốc toàn Khoa DƯỢC
    public class InOutStocksDrugsGeneralReportModal_V2 : ReportModelBase
    {
        public InOutStocksDrugsGeneralReportModal_V2()
            : base("eHCMS.ReportLib.RptDrugDepts.XtraReports.Reports.XRptInOutStockGeneral_BHYT")
        {

        }
    }

    //▼====: #002
    public class KhangSinhReportModal : ReportModelBase
    {
        public KhangSinhReportModal()
            : base("eHCMS.ReportLib.RptDrugDepts.XtraReports.Circulars22.XRpt_Antibiotics")
        {

        }
    }
    public class HospitalUseOfDrugsReportModal : ReportModelBase
    {
        public HospitalUseOfDrugsReportModal()
            : base("eHCMS.ReportLib.RptDrugDepts.XtraReports.Circulars22.XRpt_HospitalUseOfDrugs")
        {

        }
    }
    //▲====: #002

    //▼====== #006
    public class DrugDeptEstimationReportModal_V2 : ReportModelBase
    {
        public DrugDeptEstimationReportModal_V2()
            : base("eHCMS.ReportLib.RptDrugDepts.XtraReports.Estimation.XRptEstimateDrugDept_V2")
        {

        }
    }
    //▲====== #006
    public class DrugDeptEstimationFromRequestReportModal : ReportModelBase
    {
        public DrugDeptEstimationFromRequestReportModal()
            : base("eHCMS.ReportLib.RptDrugDepts.XtraReports.Estimation.XRptEstimateDrugDeptFromRequest")
        {

        }
    }
    public class DrugDeptRequestForTechnicalServiceReportModal : ReportModelBase
    {
        public DrugDeptRequestForTechnicalServiceReportModal()
            : base("eHCMS.ReportLib.RptDrugDepts.XtraReports.Request.XRptRequestDrugDeptForTechnicalService")
        {

        }
    }

    //▼====== #006
    public class DrugDeptEstimationReportForDeptModal : ReportModelBase
    {
        public DrugDeptEstimationReportForDeptModal()
            : base("eHCMS.ReportLib.RptDrugDepts.XtraReports.Estimation.XRptEstimateDrugDeptFromRequestForDept")
        {

        }
    }
    //▲====== #006
}
