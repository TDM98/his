using DataEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/*
 * 20220308 #001 QTD:   Thêm trường Cho điều trị tại khoa, Ghi chú, Lý do vào viện
 * 20221231 #002 BLQ: Truyền thông tin từ màn hình chẩn đoán vào phiếu khám vào viện từ màn hình Hồ sơ bệnh án
 * 20230110 #003 DatTB: Thêm biến lấy tiểu sử bản thân bệnh án mãn tính
 */
namespace aEMR.ViewContracts.Consultation_ePrescription
{
    public interface IAdmissionExamination
    {
        long InPtRegistrationID { get; set; }
        //▼====: #001
        //void LoadAdmissionExamination(long PtRegistrationID, string OrientedTreatment);
        string Diagnosis { get; set; }
        void LoadRefDeparments();
        bool IsShowReferralDiagnosis { get; set; }
        bool IsShowDepartment { get; set; }
        void LoadAdmissionExamination(long PtRegistrationID, string OrientedTreatment, bool pkvvrhm);
        bool IsShowNotes { get; set; }
        bool IsShowReasonAdmission { get; set; }
        bool IsShowDiagnosisResult { get; set; }
        bool IsShowPclResult { get; set; }
        long V_RegistrationType { get; set; }
        //▲====: #001
        //▼====: #002
        bool IsChronic { get; set; }
        DiagnosisTreatment DiagTrmtItem { get; set; }
        //▲====: #002

        //▼==== #003
        string PastMedicalHistory { get; set; }
        //▲==== #003
    }
}
