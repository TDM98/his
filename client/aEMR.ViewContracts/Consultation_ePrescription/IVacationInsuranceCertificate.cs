using aEMR.Infrastructure.Events;
using DataEntities;

/*
 * 20180913 TTM #001: Thêm mới hàm để nhận dữ liệu từ ViewModel cha và thêm cờ để xác định xem nó có phải là Dialog hay không
 */

namespace aEMR.ViewContracts
{
    public interface IVacationInsuranceCertificate
    {
        VacationInsuranceCertificates CurrentVacationInsuranceCertificates { get; set; }
        PatientRegistration PtRegistration { get; set; }
        void SetCurrentInformation(VacationInsuranceCertificates item);
        string TitleForm { get; set; }
        bool IsPrenatal { get; set; }
        bool IsOpenFromConsult { get; set; }
    }
}