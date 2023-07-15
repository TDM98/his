using System.Collections.Generic;
using System.Collections.ObjectModel;
using DataEntities;

namespace aEMR.ViewContracts
{
    public interface IInPatientSelectPackage
    {
        long? DeptID { get; set; }
        InPatientAdmDisDetails CurrentInPatientAdmDisDetail { get; set; }
        bool IsRegimenChecked { get; set; }
        List<RefTreatmentRegimen> ListRegiment { get; set; }
    }
}
