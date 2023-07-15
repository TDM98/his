
using System;
using System.Runtime.Serialization;

namespace DataEntities
{
    [DataContract]
    public class RegistrationSummaryInfo
    {
        [DataMemberAttribute()]
        public long RegistrationID { get; set; }

        [DataMemberAttribute()]
        public string RegistrationCode { get; set; }

        [DataMemberAttribute()]
        public DateTime ExamDate { get; set; }

        [DataMemberAttribute()]
        public long StaffID { get; set; }

        [DataMemberAttribute()]
        public string StaffName { get; set; }

        [DataMemberAttribute()]
        public long PatientID { get; set; }

        [DataMemberAttribute()]
        public string PatientName { get; set; }

        [DataMemberAttribute()]
        public string PatientCode { get; set; }

        [DataMemberAttribute()]
        public long? TransactionID { get; set; }

        [DataMemberAttribute()]
        public decimal TotalPatientPaid { get; set; }

        [DataMemberAttribute()]
        public decimal TotalRefund { get; set; }

        [DataMemberAttribute()]
        public decimal TotalHI { get; set; }

        [DataMemberAttribute()]
        public decimal TotalReceivePatient { get; set; }

        /// <summary>
        /// Tổng số tiền bệnh viện thu được từ bệnh nhân.
        /// </summary>
        [DataMemberAttribute()]
        public decimal TotalReceivedFromPatient { get; set; }

        [DataMemberAttribute()]
        public AllLookupValues.RegistrationType V_RegistrationType { get; set; }
    }

    /// <summary>
    /// Tổng tiền cho danh sách các đăng ký.
    /// </summary>
    [DataContract]
    public class RegistrationsTotalSummary
    {
        [DataMemberAttribute()]
        public decimal TotalPatientPaid { get; set; }

        [DataMemberAttribute()]
        public decimal TotalRefund { get; set; }

        /// <summary>
        /// Tổng số tiền bệnh viện thu được từ bệnh nhân.
        /// </summary>
        [DataMemberAttribute()]
        public decimal TotalReceivedFromPatient { get; set; }

        /// <summary>
        /// Tổng số tiền BH trả
        /// </summary>
        [DataMemberAttribute()]
        public decimal TotalHI { get; set; }
    }
}
