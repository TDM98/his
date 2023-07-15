using System.Collections.Generic;
using DataEntities;

namespace aEMR.ViewContracts
{
    public interface ISimplePay
    {
        bool IsPaying { get; }
        PatientRegistration Registration { get; set; }
        PaymentFormMode FormMode { get; set; }

        long V_TradingPlaces { get; set; }
        void PayCmd();
        bool CanPayCmd { get;}

        bool AllowZeroPayment { get; set; }

        bool Refundable { get; set; }

        bool AllowZeroRefund { get; set; }
        //void SaveAndPayCmd();
        
        //void SetRegistration(object registrationInfo);

        /// <summary>
        /// Lấy đầy đủ thông tin đăng ký để tính tiền
        /// </summary>
        /// <param name="registrationID"></param>
        //void LoadRegistrationByID(long registrationID);

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
        decimal TotalPaySuggested { get; set; }
        decimal TotalPayForSelectedItem { get; set; }
        /// <summary>
        /// Trả tiền cho dịch vụ mới hay đã tính tiền rồi
        /// </summary>
        bool PayNewService { get; set; }

        //20181113TNHX: [BM0003235] Doesn't print PhieuChiDinh if Doctor already printed it and When call by IsConfirmHIView
        /// <summary>
        /// Was called by ConfirmHIView
        /// </summary>
        bool IsConfirmHIView { get; set; }
        bool IsViewOnly { get; set; }
        bool IsReported { get; set; }
        bool IsUpdateHisID { get; set; }
        bool IsSaveRegisDetailsThenPay { get; set; }
        bool CallComfirmOnPay { get; set; }
        bool IsRefundFromPharmacy { get; set; }
        bool IsRefundBilling { get; set; }
        bool IsProcess { get; set; }
        bool IsUpdateStatus { get; set; }
        bool IsOutPtTreatmentPrescription { get; set; }
        TicketIssue TicketIssueObj { get; set; }
    }
}