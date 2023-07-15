
namespace aEMR.ViewContracts
{
    public interface IePrescriptionTemplateDoctor
    {
        void InitPatientInfo();
        IRegistration_DataStorage Registration_DataStorage { get; set; }
    }
}
