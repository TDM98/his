using DataEntities;
using DataEntities.MedicalInstruction;
using System.Collections.ObjectModel;

namespace aEMR.ViewContracts
{
    public interface IInstructionList
    {
        long PtRegistrationID { get; set; }
        void GetInstructionList();
    }
}
