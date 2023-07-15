using System.Data;

namespace aEMR.ViewContracts
{
    public interface IPreviewHIReport
    {
        void ApplyPreviewHIReportSet(DataSet aPreviewHIReportSet, string aErrText);
    }
}