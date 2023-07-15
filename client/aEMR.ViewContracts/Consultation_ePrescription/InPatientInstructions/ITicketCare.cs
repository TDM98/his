using DataEntities;
using DataEntities.MedicalInstruction;
using System.Collections.ObjectModel;

namespace aEMR.ViewContracts
{
    public interface ITicketCare
    {
        long PtRegistrationID { get; set; }
        long IntPtDiagDrInstructionID { get; set; }
        long V_LevelCare { get; set; }
        long V_RegistrationType { get; set; }
        TicketCare gTicketCare { get; set; }
        void GetTicketCare(long IntPtDiagDrInstructionID);
        string DoctorOrientedTreatment { get; set; }
    }
}
