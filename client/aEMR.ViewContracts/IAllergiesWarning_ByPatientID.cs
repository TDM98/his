using System;
namespace aEMR.ViewContracts
{
    public interface IAllergiesWarning_ByPatientID
    {
        String Allergies { get; }
        String Warning { get; }
        IRegistration_DataStorage Registration_DataStorage { get; set; }
    }
}