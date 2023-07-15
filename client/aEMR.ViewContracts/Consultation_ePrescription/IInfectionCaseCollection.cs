using DataEntities;

namespace aEMR.ViewContracts
{
    public interface IInfectionCaseCollection
    {
        void LoadOldInfectionCase(InfectionCase aInfectionCase);
        bool IsUpdatedCompleted { get; set; }
    }
    public interface IAntibioticTreatmentEdit
    {
        AntibioticTreatment CurrentAntibioticTreatment { get; set; }
        long DeptID { get; set; }
        bool IsUpdatedCompleted { get; set; }
        long PtRegistrationID { get; set; }
    }
}