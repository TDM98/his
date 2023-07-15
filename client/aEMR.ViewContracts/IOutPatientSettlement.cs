using DataEntities;
using Caliburn.Micro;
using System.Collections.Generic;

namespace aEMR.ViewContracts
{
    public interface IOutPatientSettlement
    {
        ISearchPatientAndRegistration SearchRegistrationContent { get; set; }
        AllLookupValues.PatientFindBy PatientFindBy { get; set; }
      
    }
}
