using DataEntities;
namespace aEMR.ViewContracts
{
    public interface IDrugDeptReportDocumentPreview
    {
        long ID { get; set; }
        long PatientID { get; set; }
        ReportName eItem { get; set; }
        long PaymentID { get; set; }
        string StaffFullName { get; set; }
        string LyDo { get; set; }
        long V_TranRefType { get; set; }
        string TitleRpt { get; set; }
        string TitleRpt1 { get; set; }
        long EstimationCodeBegin { get; set; }
        long EstimationCodeEnd { get; set; }
        long V_MedProductType { get; set; }
        long StoreID { get; set; }
        bool IsLiquidation { get; set; }
        bool EstimationFromRequest { get; set; }
    }
}