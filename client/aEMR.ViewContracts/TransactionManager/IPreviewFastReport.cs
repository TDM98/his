using System.Data;

namespace aEMR.ViewContracts
{
    public interface IPreviewFastReport
    {
        DataSet FastReportDetails { get; set; }
        void ViewTable();
        long FastReportID { get; set; }
        long V_FastReportType { get; set; }
    }
}