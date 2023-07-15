using DataEntities;
namespace aEMR.ViewContracts
{
    public interface IAllergies
    {
        long PatientID { get; set; }
        MDAllergy Allergy { get; set; }
        MDWarning Warning { get; set; }

        long CaseOfAllergyType { get; set; }
    }
}
