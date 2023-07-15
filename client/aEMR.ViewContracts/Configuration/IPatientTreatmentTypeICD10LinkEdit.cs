using DataEntities;

namespace aEMR.ViewContracts.Configuration
{
    public interface IPatientTreatmentTypeICD10LinkEdit
    {
        string TitleForm { get; set; }
        OutpatientTreatmentTypeICD10Link ObjICD10Link_Current { get; set; }
        void InitializeNewItem();
    }
}
