using DataEntities;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace aEMR.ViewContracts
{
    public interface IExecuteDoctorInstruction
    {
        void InitPatientInfo(PatientRegistration CurrentPatientRegistration = null);
        IRegistration_DataStorage Registration_DataStorage { get; set; }
    }
}
