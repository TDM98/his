using DataEntities;
using System.Collections.Generic;
using System.Data;

namespace aEMR.ViewContracts
{
    public interface IElectronicPrescriptionPreview
    {
        //void ApplyPreviewHIReportSet(DataSet aPreviewHIReportSet, string aErrText);
        List<DTDT_don_thuoc> ListDTDT_don_thuoc { get; set; }
    }
}