using DataEntities;

namespace aEMR.ViewContracts
{
    public interface IConsultationDocumentPreview
    {
        long ID { get; set; }
        long PatientID { get; set; }
        ReportName eItem { get; set; }
    }
}