using DataEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/*
 * 20220308 #001 QTD:   Thêm trường Cho điều trị tại khoa, Ghi chú, Lý do vào viện
 * 20221231 #002 BLQ: Truyền thông tin từ màn hình chẩn đoán vào phiếu khám vào viện từ màn hình Hồ sơ bệnh án
 */
namespace aEMR.ViewContracts.Consultation_ePrescription
{
    public interface ISelfDeclaration
    {
        long PtRegistrationID { get; set; }
        long PatientID { get; set; }
        long V_RegistrationType { get; set; }
        void GetSelfDeclarationByPtRegistrationID();
    }
}
