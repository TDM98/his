using DataEntities;
namespace aEMR.ViewContracts
{
    public interface IClinicDeptReportDocumentPreview
    {
        long ID { get; set; }
        long PatientID { get; set; }
        ReportName eItem { get; set; }
        long PaymentID { get; set; }
        string StaffFullName { get; set; }
        string LyDo { get; set; }
        long V_TranRefType { get; set; }
        long V_MedProductType { get; set; }
        long IntPtDiagDrInstructionID { get; set; }
    }
}
