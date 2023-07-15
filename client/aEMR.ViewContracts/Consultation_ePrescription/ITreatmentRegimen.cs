using System.Collections.Generic;

namespace aEMR.ViewContracts
{
    public interface ITreatmentRegimen
    {
        long? PtRegDetailID { get; set; }
        List<string> listICD10Codes { get; set; }
    }
}