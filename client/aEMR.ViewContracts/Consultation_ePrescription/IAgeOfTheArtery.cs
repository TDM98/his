using DataEntities;

namespace aEMR.ViewContracts
{
    public interface IAgeOfTheArtery
    {
        Patient Patient { get; set; }
        long PtRegistrationID { get; set; }
        long V_RegistrationType { get; set; }
        long PatientClassID { get; set; }
    }
}