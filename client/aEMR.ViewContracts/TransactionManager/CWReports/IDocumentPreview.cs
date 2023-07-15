using DataEntities;
namespace aEMR.ViewContracts
{
    public interface IDocumentPreview
    {
        long ID { get; set; }
        ReportName eItem { get; set; }
        string StaffFullName { get; set; }

    }
}
