/*
 * 20180924 #001 TTM: Chuyển dữ liệu từ trong view con ra ngoài view cha
 */
using DataEntities;

namespace aEMR.ViewContracts
{
    public interface IConsultationsSummary_V2
    {
        bool IsShowEditTinhTrangTheChat { get; set; }
        bool IsUpdateWithoutChangeDoctorIDAndDatetime { get; set; }
        void CheckVisibleForTabControl();
        void GetAllRegistrationDetails_ForGoToKhamBenh_Ext(PatientRegistrationDetail PtRegDetail);
        bool IsOutPtTreatmentProgram { get; set; }
        bool IsSearchOnlyProcedure { get; set; }
    }
}