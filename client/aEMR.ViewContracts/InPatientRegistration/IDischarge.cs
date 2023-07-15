using DataEntities;
using System.Collections.Generic;

namespace aEMR.ViewContracts
{
    /// <summary>
    /// Form Ra Vien
    /// </summary>
    public interface IDischarge
    {
        ISearchPatientAndRegistration SearchRegistrationContent { get; set; }
        IPatientSummaryInfoV2 PatientSummaryInfoContent { get; set; }
        Patient CurrentPatient { get; set; }
        PatientRegistration CurrentRegistration { get; set; }
        IDischargeInfo DischargeInfoContent { get; set; }
        bool RegistrationLoading { get; set; }
        bool PatientLoading { get; set; }

        //bool DischargeInfoSaving { get; set; }

        void ResetForm();
        string DeptLocTitle { get; set; }

        bool EnableSearchAllDepts { get; set; }

        bool IsShowConfirmDischargeBtn { get; set; }
    }

    public interface IDischargeNew
    {
        ISearchPatientAndRegistration SearchRegistrationContent { get; set; }
        IPatientSummaryInfoV2 PatientSummaryInfoContent { get; set; }
        Patient CurrentPatient { get; set; }
        PatientRegistration CurrentRegistration { get; set; }
        IDischargeInfo DischargeInfoContent { get; set; }
        bool RegistrationLoading { get; set; }
        bool PatientLoading { get; set; }

        //bool DischargeInfoSaving { get; set; }

        void ResetForm();
        string DeptLocTitle { get; set; }

        bool EnableSearchAllDepts { get; set; }

        void InitView(bool usedInConsultationModule);

        bool IsShowConfirmDischargeBtn { get; set; }

        bool IsConsultation { get; set; }

        // Hpt 27/11/2015: Thêm biến này vào Interface để truyền vào VM, làm trung gian để gán giá trị cho PatientFindBy trong SearchPatientRegistrationViewModel, không dùng bắn sự kiện nữa
        AllLookupValues.PatientFindBy PatientFindBy { get; set; }
        IRegistration_DataStorage Registration_DataStorage { get; set; }
        void ApplyDiagnosisTreatmentCollection(IList<DiagnosisTreatment> aDiagnosisTreatmentCollection);
        void SetLastDiagnosisForConfirm();
        bool ShowPrintTemp01KBCB { get; }
    }
}
