using DataEntities;
using DataEntities.MedicalInstruction;
using System.Collections.ObjectModel;

namespace aEMR.ViewContracts
{
    public interface IExecuteDrug
    {
        long ExecuteDrugID { get; set; }
        long ExecuteDrugDetailID { get; set; }
        long StaffID { get; set; }
    }
}
