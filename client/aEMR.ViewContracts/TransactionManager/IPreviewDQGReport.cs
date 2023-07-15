using System.Data;

namespace aEMR.ViewContracts
{
    public interface IPreviewDQGReport
    {
        DataSet gReportDetails { get; set; }
        void ViewTable();
    }
}