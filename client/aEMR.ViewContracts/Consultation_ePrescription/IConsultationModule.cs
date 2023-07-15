using DataEntities;

namespace aEMR.ViewContracts
{
    public interface IConsultationModule
    {
        object MainContent { get; set; }
        Patient CurrentPatient { get; set; }
        PatientRegistration Registration { get; set; }
        void PatientServiceRecordsGetForKhamBenh_Ext(bool bAllowModifyPrescription = false);    // This AllowModifyPrescription flag is to indicate that AllowModifyPrescription should be called or NOT
        bool IsReloadPrescription { get; set; }
    }
}