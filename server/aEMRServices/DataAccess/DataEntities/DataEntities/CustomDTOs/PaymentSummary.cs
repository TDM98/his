using System.Runtime.Serialization;
using eHCMS.Services.Core.Base;
using System.Collections.Generic;

namespace DataEntities
{
    [DataContract]
    public class PayableSum : NotifyChangedBase, ITotalPrice
    {
        private int _TotalQty;
        [DataMemberAttribute()]
        public int TotalQty
        {
            get
            {
                return _TotalQty;
            }
            set
            {
                _TotalQty = value;
                RaisePropertyChanged("TotalQty");
            }
        }
        private decimal _TotalInvoicePrice;

        /// Tổng giá tiền của các dịch vụ

        [DataMemberAttribute()]
        public decimal TotalInvoicePrice
        {
            get
            {
                return _TotalInvoicePrice;
            }
            set
            {
                _TotalInvoicePrice = value;
                RaisePropertyChanged("TotalInvoicePrice");
            }
        }

        private decimal _TotalPrice;

        /// Tổng giá tiền của các dịch vụ

        /// 
        [DataMemberAttribute()]
        public decimal TotalPrice
        {
            get
            {
                return _TotalPrice;
            }
            set
            {
                _TotalPrice = value;
                RaisePropertyChanged("TotalPrice");
            }
        }
        private decimal _TotalHIServicePrice;

        /// Tổng giá tiền các dịch vụ bảo hiểm (giá tiền bào hiểm chịu trả)

        /// 
        [DataMemberAttribute()]
        public decimal TotalHIServicePrice
        {
            get
            {
                return _TotalHIServicePrice;
            }
            set
            {
                _TotalHIServicePrice = value;
                RaisePropertyChanged("TotalHIServicePrice");
            }
        }

        private decimal _TotalNonHIServicePrice;

        /// Tổng giá tiền các dịch vụ không thuộc danh mục bảo hiểm

        /// 
        [DataMemberAttribute()]
        public decimal TotalNonHIServicePrice
        {
            get
            {
                return _TotalNonHIServicePrice;
            }
            set
            {
                _TotalNonHIServicePrice = value;
                RaisePropertyChanged("TotalNonHIServicePrice");
            }
        }

        private decimal _TotalPriceDifference;

        /// Tổng giá tiền chênh lệch (Khoản này bệnh nhân phải trả)

        /// 
        [DataMemberAttribute()]
        public decimal TotalPriceDifference
        {
            get
            {
                return _TotalPriceDifference;
            }
            set
            {
                _TotalPriceDifference = value;
                RaisePropertyChanged("TotalPriceDifference");
            }
        }
        private decimal _TotalCoPayment;

        /// Tổng số tiền đồng chi trả (Tiền bệnh nhân đồng chi trả với bảo hiểm)

        /// 
        [DataMemberAttribute()]
        public decimal TotalCoPayment
        {
            get
            {
                return _TotalCoPayment;
            }
            set
            {
                _TotalCoPayment = value;
                RaisePropertyChanged("TotalCoPayment");
            }
        }
        private decimal _TotalHIPayment;

        /// Tổng số tiền bảo hiểm phải trả cho bệnh nhân

        /// 
        [DataMemberAttribute()]
        public decimal TotalHIPayment
        {
            get
            {
                return _TotalHIPayment;
            }
            set
            {
                _TotalHIPayment = value;
                RaisePropertyChanged("TotalHIPayment");
            }
        }
        private decimal _TotalPatientPayment;

        /// Tổng số tiền bệnh nhân phải trả

        /// 
        [DataMemberAttribute()]
        public decimal TotalPatientPayment
        {
            get
            {
                return _TotalPatientPayment;
            }
            set
            {
                _TotalPatientPayment = value;
                RaisePropertyChanged("TotalPatientPayment");
            }
        }

        private decimal _TotalPatientPaid;

        /// Tổng số tiền bệnh nhân đã trả

        /// 
        [DataMemberAttribute()]
        public decimal TotalPatientPaid
        {
            get
            {
                return _TotalPatientPaid;
            }
            set
            {
                _TotalPatientPaid = value;
                RaisePropertyChanged("TotalPatientPaid");
            }
        }
        private decimal _TotalPatientRemainingOwed;

        /// Tổng số tiền bệnh nhân còn nợ lại

        /// 
        [DataMemberAttribute()]
        public decimal TotalPatientRemainingOwed
        {
            get
            {
                return _TotalPatientRemainingOwed;
            }
            set
            {
                _TotalPatientRemainingOwed = value;
                RaisePropertyChanged("TotalPatientRemainingOwed");
            }
        }

        private decimal _totalPaymentForTransaction;
        /// <summary>
        /// Tổng số tiền của transaction hiện tại.
        /// </summary>
        [DataMemberAttribute()]
        public decimal TotalPaymentForTransaction
        {
            get
            {
                return _totalPaymentForTransaction;
            }
            set
            {
                _totalPaymentForTransaction = value;
                RaisePropertyChanged("TotalPaymentForTransaction");
            }
        }

        private decimal _totalAmtRegDetailServices;
        /// <summary>
        /// Tổng số tiền của Registration Details Services
        /// </summary>
        [DataMemberAttribute()]
        public decimal TotalAmtRegDetailServices
        {
            get
            {
                return _totalAmtRegDetailServices;
            }
            set
            {
                _totalAmtRegDetailServices = value;
                RaisePropertyChanged("TotalAmtRegDetailServices");
            }
        }

        private decimal _totalPaidForRegDetailServices;
        /// <summary>
        /// Tổng số tiền da tra cho Registration Details Services
        /// </summary>
        [DataMemberAttribute()]
        public decimal TotalPaidForRegDetailServices
        {
            get
            {
                return _totalPaidForRegDetailServices;
            }
            set
            {
                _totalPaidForRegDetailServices = value;
                RaisePropertyChanged("TotalPaidForRegDetailServices");
            }
        }

        private decimal _totalAmtRegPCLRequests;
        /// <summary>
        /// Tổng số tiền của PCL Requests
        /// </summary>
        [DataMemberAttribute()]
        public decimal TotalAmtRegPCLRequests
        {
            get
            {
                return _totalAmtRegPCLRequests;
            }
            set
            {
                _totalAmtRegPCLRequests = value;
                RaisePropertyChanged("TotalAmtRegPCLRequests");
            }
        }

        private decimal _totalPaidForRegPCLRequests;
        /// <summary>
        /// Tổng số tiền da tra cho PCL Requests
        /// </summary>
        [DataMemberAttribute()]
        public decimal TotalPaidForRegPCLRequests
        {
            get
            {
                return _totalPaidForRegPCLRequests;
            }
            set
            {
                _totalPaidForRegPCLRequests = value;
                RaisePropertyChanged("TotalPaidForRegPCLRequests");
            }
        }

        private decimal _totalAmtOutwardDrugInvoices;
        /// <summary>
        /// Tổng số tiền của Outward Drug Invoices
        /// </summary>
        [DataMemberAttribute()]
        public decimal TotalAmtOutwardDrugInvoices
        {
            get
            {
                return _totalAmtOutwardDrugInvoices;
            }
            set
            {
                _totalAmtOutwardDrugInvoices = value;
                RaisePropertyChanged("TotalAmtOutwardDrugInvoices");
            }
        }

        private decimal _totalPaidForOutwardDrugInvoices;
        /// <summary>
        /// Tổng số tiền da tra cho PCL Requests
        /// </summary>
        [DataMemberAttribute()]
        public decimal TotalPaidForOutwardDrugInvoices
        {
            get
            {
                return _totalPaidForOutwardDrugInvoices;
            }
            set
            {
                _totalPaidForOutwardDrugInvoices = value;
                RaisePropertyChanged("TotalPaidForOutwardDrugInvoices");
            }
        }


        private decimal _TotalPatientCashAdvance;
        [DataMemberAttribute()]
        public decimal TotalPatientCashAdvance
        {
            get
            {
                return _TotalPatientCashAdvance;
            }
            set
            {
                _TotalPatientCashAdvance = value;
                RaisePropertyChanged("TotalPatientCashAdvance");
            }
        }

        [DataMemberAttribute]
        public decimal TotalDiscountAmount
        {
            get => _TotalDiscountAmount; set
            {
                _TotalDiscountAmount = value;
                RaisePropertyChanged("TotalDiscountAmount");
            }
        }
        private decimal _TotalDiscountAmount = 0;

        [DataMemberAttribute]
        public decimal TotalDiscountedAmount
        {
            get => _TotalDiscountedAmount; set
            {
                _TotalDiscountedAmount = value;
                RaisePropertyChanged("TotalDiscountedAmount");
            }
        }
        private decimal _TotalDiscountedAmount = 0;

        private decimal _TotalCashAdvanceAmount = 0;
        [DataMemberAttribute]
        public decimal TotalCashAdvanceAmount
        {
            get
            {
                return _TotalCashAdvanceAmount;
            }
            set
            {
                _TotalCashAdvanceAmount = value;
            }
        }
    }

    public class PaymentSummary : NotifyChangedBase
    {
        private decimal _Total;
     
        /// Tong so tien benh nhan da tra.
     
        public decimal Total
        {
            get
            {
                return _Total;
            }
            set
            {
                _Total=value;
                RaisePropertyChanged("Total");
            }
        }

        private List<PatientTransactionPayment> _PatientPayments = null;
        public List<PatientTransactionPayment> PatientPayments
        {
            get
            {
                return _PatientPayments;
            }
            set
            {
                _PatientPayments = value;
                RaisePropertyChanged("PatientPayments");
            }
        }
    }
}
