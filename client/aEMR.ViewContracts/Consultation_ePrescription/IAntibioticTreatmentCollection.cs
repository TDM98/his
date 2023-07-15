using DataEntities;

namespace aEMR.ViewContracts
{
    public interface IAntibioticTreatmentCollection
    {
        PatientRegistration CurrentRegistration { get; set; }
        AntibioticTreatment SelectedAntibioticTreatment { get; set; }
    }
}