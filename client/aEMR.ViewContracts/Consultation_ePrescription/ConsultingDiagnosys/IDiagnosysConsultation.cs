using DataEntities;
using System.Collections.Generic;
namespace aEMR.ViewContracts
{
    public interface IDiagnosysConsultation
    {
        void CallSetInPatientInfoForConsultation(PatientRegistration aRegistration, PatientRegistrationDetail aRegistrationDetail);
        void CallSetPatientInfoForConsultation(PatientRegistrationDetail aRegistrationDetail);  
    }
}