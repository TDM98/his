/*
 * 20181022 #001 TTM: Không sử dụng nên comment lại (Fix BM0003214)
 */
using DataEntities;

namespace aEMR.ViewContracts
{
    public delegate void CallSetInPatientInfoAndRegistrationForePresciption_InPt();
    public interface IConsultations_InPt
    {
        bool IsPopUp { get; set; }
        bool IsDailyDiagnosis { get; set; }
        bool IsDiagnosisOutHospital { get; set; }
        //▼====== #001
        //void activeControl();
        //▲====== #001
        bool IsProcedureEdit { get; set; }
        IRegistration_DataStorage Registration_DataStorage { get; set; }
        void CallSetInPatientInfoForConsultation(PatientRegistration aRegistration, PatientRegistrationDetail aRegistrationDetail);
        CallSetInPatientInfoAndRegistrationForePresciption_InPt CallSetInPatientInfoAndRegistrationForePresciption_InPt { get; set; }
        bool IsShowSummaryContent { get; set; }
        bool IsPhysicalTherapy { get; set; }
        bool IsUpdateDiagConfirmInPT { get; set; }
    }
}