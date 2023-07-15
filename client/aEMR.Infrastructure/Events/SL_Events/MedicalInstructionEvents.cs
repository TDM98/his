using DataEntities;

namespace aEMR.Infrastructure.Events
{
    public class LoadMedicalInstructionEvent
    {
        public PatientRegistration gRegistration { get; set; }
    }
    public class ReloadLoadMedicalInstructionEvent
    {
        public InPatientInstruction gInPatientInstruction { get; set; }
    }
    public class ReloadDiagnosisTreatmentTree
    {
        public PatientRegistration gRegistration { get; set; }
    }
}