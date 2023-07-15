using System.Collections.Generic;
using DataEntities;

namespace aEMR.ViewContracts
{
    public interface ISimplePay2
    {
        bool IsPaying { get; }
        PatientRegistration Registration { get; set; }

        long V_TradingPlaces { get; set; }

        void PayCmd();
        bool CanPayCmd { get;}

        void SetRegistration(object registrationInfo);

        /// <summary>
        /// Lấy đầy đủ thông tin đăng ký để tính tiền
        /// </summary>
        /// <param name="registrationID"></param>
        void LoadRegistrationByID(long registrationID);
        /// <summary>
        /// Danh sách chi tiết các dịch vụ cần tính tiền.
        /// </summary>
        IList<PatientRegistrationDetail> RegistrationDetails { get; set; }

        /// <summary>
        /// Danh sách các yêu cầu CLS cần tính tiền.
        /// </summary>
        IList<PatientPCLRequest> PclRequests { get; set; }

        /// <summary>
        /// Danh sách các toa thuốc cần tính tiền.
        /// </summary>
        IList<OutwardDrugInvoice> DrugInvoices { get; set; }

        void StartCalculating();

        /// <summary>
        /// Chỉ trả tiền cho những item được chọn. Không tính phần dư ra của 15% bảo hiểm sau khi tính toán.
        /// </summary>
        bool PayForSelectedItemOnly { get; set; }

        object ObjectState { get; set; }
    }
}
