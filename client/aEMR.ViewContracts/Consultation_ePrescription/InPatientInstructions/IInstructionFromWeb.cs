using DataEntities;
using DataEntities.MedicalInstruction;
using System.Collections.ObjectModel;

namespace aEMR.ViewContracts
{
    public interface IInstructionFromWeb
    {
        long PtRegistrationID { get; set; }
        void GetInstructionList();
    }
}
