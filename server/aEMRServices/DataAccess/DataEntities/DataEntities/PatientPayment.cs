using System;
using System.Runtime.Serialization;
using eHCMS.Services.Core.Base;

namespace DataEntities
{
    [DataContract]
    public partial class PatientPayment : NotifyChangedBase
    {
        public PatientPayment()
        {
            _creditOrDebit = 1;
        }
        #region Factory Method


        /// Create a new PatientPayment object.

        /// <param name="ptPmtID">Initial value of the PtPmtID property.</param>
        /// <param name="payAmount">Initial value of the PayAmount property.</param>
        public static PatientPayment CreatePatientPayment(long ptPmtID, Decimal payAmount)
        {
            var patientPayment = new PatientPayment {PtPmtID = ptPmtID, PayAmount = payAmount};
            return patientPayment;
        }

        #endregion
        #region Primitive Properties


        [DataMemberAttribute()]
        public long PtPmtID
        {
            get
            {
                return _ptPmtID;
            }
            set
            {
                if (_ptPmtID != value)
                {
                    OnPtPmtIDChanging(value);
                    ////ReportPropertyChanging("PtPmtID");
                    _ptPmtID = value;
                    RaisePropertyChanged("PtPmtID");
                    OnPtPmtIDChanged();
                }
            }
        }
        private long _ptPmtID;
        partial void OnPtPmtIDChanging(long value);
        partial void OnPtPmtIDChanged();

        [DataMemberAttribute()]
        public String InvoiceID
        {
            get
            {
                return _invoiceID;
            }
            set
            {
                OnInvoiceIDChanging(value);
                ////ReportPropertyChanging("InvoiceID");
                _invoiceID = value;
                RaisePropertyChanged("InvoiceID");
                OnInvoiceIDChanged();
            }
        }
        private String _invoiceID;
        partial void OnInvoiceIDChanging(String value);
        partial void OnInvoiceIDChanged();





        [DataMemberAttribute()]
        public Nullable<long> TransactionID
        {
            get
            {
                return _TransactionID;
            }
            set
            {
                OnTransactionIDChanging(value);
                ////ReportPropertyChanging("TransactionID");
                _TransactionID = value;
                RaisePropertyChanged("TransactionID");
                OnTransactionIDChanged();
            }
        }
        private Nullable<long> _TransactionID;
        partial void OnTransactionIDChanging(Nullable<long> value);
        partial void OnTransactionIDChanged();


        [DataMemberAttribute()]
        public Nullable<Int64> IntRcptTypeID
        {
            get
            {
                return _IntRcptTypeID;
            }
            set
            {
                OnIntRcptTypeIDChanging(value);
                ////ReportPropertyChanging("IntRcptTypeID");
                _IntRcptTypeID = value;
                RaisePropertyChanged("IntRcptTypeID");
                OnIntRcptTypeIDChanged();
            }
        }
        private Nullable<Int64> _IntRcptTypeID;
        partial void OnIntRcptTypeIDChanging(Nullable<Int64> value);
        partial void OnIntRcptTypeIDChanged();



        [DataMemberAttribute()]
        public Nullable<DateTime> PaymentDate
        {
            get
            {
                return _PaymentDate;
            }
            set
            {
                OnPaymentDateChanging(value);
                ////ReportPropertyChanging("PaymentDate");
                _PaymentDate = value;
                RaisePropertyChanged("PaymentDate");
                OnPaymentDateChanged();
            }
        }
        private Nullable<DateTime> _PaymentDate;
        partial void OnPaymentDateChanging(Nullable<DateTime> value);
        partial void OnPaymentDateChanged();





        [DataMemberAttribute()]
        public Decimal PayAmount
        {
            get
            {
                return _PayAmount;
            }
            set
            {
                OnPayAmountChanging(value);
                ////ReportPropertyChanging("PayAmount");
                _PayAmount = value;
                RaisePropertyChanged("PayAmount");
                OnPayAmountChanged();
            }
        }
        private Decimal _PayAmount;
        partial void OnPayAmountChanging(Decimal value);
        partial void OnPayAmountChanged();





        [DataMemberAttribute()]
        public String ReceiptNumber
        {
            get
            {
                return _ReceiptNumber;
            }
            set
            {
                OnReceiptNumberChanging(value);
                ////ReportPropertyChanging("ReceiptNumber");
                _ReceiptNumber = value;
                RaisePropertyChanged("ReceiptNumber");
                OnReceiptNumberChanged();
            }
        }
        private String _ReceiptNumber;
        partial void OnReceiptNumberChanging(String value);
        partial void OnReceiptNumberChanged();





        [DataMemberAttribute()]
        public String ManualReceiptNumber
        {
            get
            {
                return _ManualReceiptNumber;
            }
            set
            {
                OnManualReceiptNumberChanging(value);
                ////ReportPropertyChanging("ManualReceiptNumber");
                _ManualReceiptNumber = value;
                RaisePropertyChanged("ManualReceiptNumber");
                OnManualReceiptNumberChanged();
            }
        }
        private String _ManualReceiptNumber;
        partial void OnManualReceiptNumberChanging(String value);
        partial void OnManualReceiptNumberChanged();

        private long? _V_Currency;
        [DataMemberAttribute()]
        public long? V_Currency
        {
            get
            {
                return _V_Currency;
            }
            set
            {
                _V_Currency = value;
                RaisePropertyChanged("V_Currency");
            }
        }

        private long? _V_PaymentMode;
        [DataMemberAttribute()]
        public long? V_PaymentMode
        {
            get
            {
                return _V_PaymentMode;
            }
            set
            {
                _V_PaymentMode = value;
                RaisePropertyChanged("V_PaymentMode");
            }
        }

        private long? _V_PaymentType;
        [DataMemberAttribute()]
        public long? V_PaymentType
        {
            get
            {
                return _V_PaymentType;
            }
            set
            {
                _V_PaymentType = value;
                RaisePropertyChanged("V_PaymentType");
            }
        }

        [DataMemberAttribute()]
        public long? PtPmtAccID
        {
            get
            {
                return _PtPmtAccID;
            }
            set
            {
                if (_PtPmtAccID != value)
                {
                    OnPtPmtAccIDChanging(value);
                    _PtPmtAccID = value;
                    RaisePropertyChanged("PtPmtAccID");
                    OnPtPmtAccIDChanged();
                }
            }
        }
        private long? _PtPmtAccID;
        partial void OnPtPmtAccIDChanging(long? value);
        partial void OnPtPmtAccIDChanged();
        #endregion

        #region Navigation Properties

        private Lookup _Currency;
        [DataMemberAttribute()]
        public Lookup Currency
        {
            get
            {
                return _Currency;
            }
            set
            {
                _Currency = value;
                RaisePropertyChanged("Currency");
            }
        }

        private Lookup _PaymentType;
        [DataMemberAttribute()]
        public Lookup PaymentType
        {
            get
            {
                return _PaymentType;
            }
            set
            {
                _PaymentType = value;
                RaisePropertyChanged("PaymentType");
            }
        }

        private Lookup _PaymentMode;
        [DataMemberAttribute()]
        public Lookup PaymentMode
        {
            get
            {
                return _PaymentMode;
            }
            set
            {
                _PaymentMode = value;
                RaisePropertyChanged("PaymentMode");
            }
        }





        [DataMemberAttribute()]
        public PatientInvoice PatientInvoice
        {
            get;
            set;
        }



        [DataMemberAttribute()]
        public PatientTransaction PatientTransaction
        {
            get;
            set;
        }

        [DataMemberAttribute()]
        public RefInternalReceiptType RefInternalReceiptType
        {
            get;
            set;
        }



        


        #endregion

        private Int16 _creditOrDebit;
        [DataMemberAttribute()]
        public Int16 CreditOrDebit
        {
            get
            {
                return _creditOrDebit;
            }
            set
            {
                if (_creditOrDebit != value)
                {
                    _creditOrDebit = value;
                    RaisePropertyChanged("CreditOrDebit");
                }
            }
        }

        private string _paymentReason;
        [DataMemberAttribute]
        public string PaymentReason
        {
            get
            {
                return _paymentReason;
            }
            set
            {
                _paymentReason = value;
                RaisePropertyChanged("PaymentReason");
            }
        }

        private long _staffID;
        [DataMemberAttribute]
        public long StaffID
        {
            get
            {
                return _staffID;
            }
            set
            {
                _staffID = value;
                RaisePropertyChanged("StaffID");
            }
        }

        /// <summary>
        /// Trong truong hop tong so tien thanh toan = 0 thi bien nay phai duoc set = true.
        /// (De cho bao hiem tinh)
        /// </summary>
        private bool _hiDelegation;
        [DataMemberAttribute()]
        public bool HiDelegation
        {
            get
            {
                return _hiDelegation;
            }
            set
            {
                _hiDelegation = value;
                RaisePropertyChanged("HiDelegation");
            }
        }
        public override bool Equals(object obj)
        {
            PatientPayment info = obj as PatientPayment;
            if (info == null)
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;

            return this.PtPmtID == info.PtPmtID;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }


   
}
