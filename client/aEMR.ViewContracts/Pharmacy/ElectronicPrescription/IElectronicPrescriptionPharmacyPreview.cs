using DataEntities;
using System.Collections.Generic;
using System.Data;

namespace aEMR.ViewContracts
{
    public interface IElectronicPrescriptionPharmacyPreview
    {
        //void ApplyPreviewHIReportSet(DataSet aPreviewHIReportSet, string aErrText);
        List<DQG_don_thuoc> ListDQG_don_thuoc { get; set; }
    }
}