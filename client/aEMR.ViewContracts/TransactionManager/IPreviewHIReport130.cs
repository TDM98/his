using System.Data;

namespace aEMR.ViewContracts
{
    public interface IPreviewHIReport130
    {
        void ApplyPreviewHIReportSet(DataSet aPreviewHIReportSet, string aErrText);
    }
}