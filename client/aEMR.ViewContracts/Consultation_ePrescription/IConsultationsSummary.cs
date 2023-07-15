using DataEntities;
using System.Collections.Generic;
using System.Collections.ObjectModel;
/*
 * 20180924 #001 TTM: Chuyển dữ liệu từ trong view con ra ngoài view cha
 */
namespace aEMR.ViewContracts
{
    public interface ICS_DataStorage
    {
        PatientMedicalRecord PatientMedicalRecordInfo { get; set; }
        bool Getting_PatientMedicalRecordInfo { get; set; }

        Patient CurrentPatient { get; set; }

        //▼====== #001
        DiagnosisTreatment DiagTreatment { get; set; }
        ObservableCollection<DiagnosisIcd10Items> refIDC10List { get; set; }
        //▲====== #001
        List<RefTreatmentRegimen> TreatmentRegimenCollection { get; set; }
    }

    public interface IConsultationsSummary
    {
        bool IsShowEditTinhTrangTheChat { get; set; }
        bool IsUpdateWithoutChangeDoctorIDAndDatetime { get; set; }
        void CheckVisibleForTabControl();
        void GetAllRegistrationDetails_ForGoToKhamBenh_Ext(PatientRegistrationDetail PtRegDetail);
    }
}