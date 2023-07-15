using DataEntities;

namespace aEMR.ViewContracts
{
    public interface IInPatientInstruction
    {
        void LoadRegistrationInfo(PatientRegistration aRegistration, bool LoadInstructionAlso = true);
        IRegistration_DataStorage Registration_DataStorage { get; set; }
        void ReloadLoadInPatientInstruction(long aIntPtDiagDrInstructionID);
        bool FromInPatientAdmView { get; set; }
        bool IsUpdateDiagConfirmInPT { get; set; }
    }
}