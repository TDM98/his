using aEMR.Common;
using DataEntities;

namespace aEMR.ViewContracts
{
    public interface IPatientPCLRequestEdit
    {
        bool IsAppointment { get; set; }
        bool IsBreakStatus { get; set; }
        bool IsEdit { get; set; }
        void InitPatientInfo(Patient CurrentPatient);
        void StartEditing(long PatientID, PatientApptPCLRequests apptPCLRequest);
        long V_RegistrationType { get; set; }
        LeftModuleActive CallByPCLRequestViewModel { get; set; }
        bool IsShowSummaryContent { get; set; }
        bool btSaveIsEnabled { get; set; }
        PatientPCLRequest CurrentPclRequest { get; set; }
        ICS_DataStorage CS_DS { get; set; }
        bool IsShowCheckBoxPayAfter { get; set; }
        bool FormIsEnabled { get; set; }
        IRegistration_DataStorage Registration_DataStorage { get; set; }
    }
}