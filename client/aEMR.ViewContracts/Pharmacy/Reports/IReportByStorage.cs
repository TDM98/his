using DataEntities;
using DevExpress.Xpf.Printing;
namespace aEMR.ViewContracts
{
    public interface IReportByStorage
    {
        ReportName eItem { get; set; }
        string TitleForm { get; set; }
    }
}
