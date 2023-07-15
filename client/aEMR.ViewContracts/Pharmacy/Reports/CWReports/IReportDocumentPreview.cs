using DataEntities;
namespace aEMR.ViewContracts
{
    public interface IReportDocumentPreview
    {
        long ID { get; set; }
        long PatientID { get; set; }
        ReportName eItem { get; set; }
        long PaymentID { get; set; }
        string StaffFullName { get; set; }
        string LyDo { get; set; }
        long V_TranRefType { get; set; }

        string TitleForm { get; set; }

        bool IsInsurance { get; set; }

        string ListID { get; set; }
        long SupplierID { get; set; }
        long PCOID { get; set; }
        long V_RegistrationType { get; set; }
        bool IsPatientCOVID { get; set; }
    }
}