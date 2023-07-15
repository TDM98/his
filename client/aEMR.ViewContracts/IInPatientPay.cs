using System.Collections.Generic;
using DataEntities;

namespace aEMR.ViewContracts
{
    public interface IInPatientPay
    {
        PatientRegistration Registration { get; set; }

        void PayCmd();
        bool CanPayCmd { get;}

        void SetValues(PatientRegistration regInfo, IList<InPatientBillingInvoice> billingInvoiceList);

        /// <summary>
        /// Lấy đầy đủ thông tin đăng ký để tính tiền
        /// </summary>
        /// <param name="registrationID"></param>
        //void LoadRegistrationByID(long registrationID);
        
        /// <summary>
        /// Danh sách các bill cần tính tiền.
        /// </summary>
        IList<InPatientBillingInvoice> BillingInvoices { get;}

        void StartCalculating();
        bool AutoPay { get; set; }
    }
}
