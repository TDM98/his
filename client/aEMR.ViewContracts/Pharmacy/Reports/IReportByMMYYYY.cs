using DataEntities;
using DevExpress.Xpf.Printing;
namespace aEMR.ViewContracts
{
    public interface IReportByMMYYYY
    {
        ReportName eItem { get; set; }
        ReportParameters RptParameters { get; set; }
        string pageTitle { get; set; }
        bool bXem { get; set; }
        bool bIn { get; set; }
    }
}
