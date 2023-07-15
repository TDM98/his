using DataEntities;

namespace aEMR.ViewContracts
{
    public interface IDischargePapersEdit
    {
        PatientRegistration CurrentRegistration { get; set; }
        long OutPtTreatmentProgramID { get; set; }
        string Notes { get; set; }
        string DischargeDiagnostic { get; set; }
        void GetDischargePapersInfo(long PtRegistrationID, long V_RegistrationType);
    }
}