using eHCMS.Services.Core.Base;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
namespace DataEntities
{
    [DataContract]
    public partial class OutPtTransactionFinalization : NotifyChangedBase
    {
        private long _TranFinalizationID;
        private DateTime _DateFinalize;
        private long _PtRegistrationID;
        private long _StaffID;
        private DateTime _TranDateFrom;
        private DateTime _TranDateTo;
        private long _V_TranFinalizationType;
        private long _V_RegistrationType;
        private string _FinalizedReceiptNum;
        private decimal _Amount;
        private decimal _TotalHasVATAmount;
        private double? _VATPercent;
        private string _TaxMemberName;
        private string _TaxMemberAddress;
        private string _TaxCode;
        private long _V_PaymentMode;
        private string _BankAccountNumber;
        private string _Denominator;
        private string _InvoiceNumb;
        private string _Symbol;
        private List<long> _PtRegistrationIDCollection;
        private long _TransactionFinalizationSummaryInfoID;
        private Staff _CreatedStaff;
        private DateTime? _DateInvoice;
        private string _eInvoiceToken;
        private bool _IsSelected = false;
        private bool _IsDeleted = false;
        private string _eInvoiceKey;
        private string _PatientFullName;
        private long? _GenericPaymentID;
        private long? _outiID;
        private string _ReceiptDetails;
        private decimal _TotalAmount;
        private string _Buyer;
        [DataMemberAttribute]
        public string Buyer
        {
            get => _Buyer; set
            {
                _Buyer = value;
                RaisePropertyChanged("Buyer");
            }
        }
        [DataMemberAttribute]
        public decimal TotalAmount
        {
            get => _TotalAmount; set
            {
                _TotalAmount = value;
                RaisePropertyChanged("TotalAmount");
            }
        }
        [DataMemberAttribute]
        public long TranFinalizationID
        {
            get => _TranFinalizationID; set
            {
                _TranFinalizationID = value;
                RaisePropertyChanged("TranFinalizationID");
            }
        }
        [DataMemberAttribute]
        public DateTime DateFinalize
        {
            get => _DateFinalize; set
            {
                _DateFinalize = value;
                RaisePropertyChanged("DateFinalize");
            }
        }
        [DataMemberAttribute]
        public long PtRegistrationID
        {
            get
            {
                return _PtRegistrationID;
            }
            set
            {
                _PtRegistrationID = value;
                RaisePropertyChanged("PtRegistrationID");
                RaisePropertyChanged("InvoiceKey");
            }
        }
        [DataMemberAttribute]
        public long StaffID
        {
            get => _StaffID; set
            {
                _StaffID = value;
                RaisePropertyChanged("StaffID");
            }
        }
        [DataMemberAttribute]
        public DateTime TranDateFrom
        {
            get => _TranDateFrom; set
            {
                _TranDateFrom = value;
                RaisePropertyChanged("TranDateFrom");
            }
        }
        [DataMemberAttribute]
        public DateTime TranDateTo
        {
            get => _TranDateTo; set
            {
                _TranDateTo = value;
                RaisePropertyChanged("TranDateTo");
            }
        }
        [DataMemberAttribute]
        public long V_TranFinalizationType
        {
            get => _V_TranFinalizationType; set
            {
                _V_TranFinalizationType = value;
                RaisePropertyChanged("V_TranFinalizationType");
            }
        }
        [DataMemberAttribute]
        public long V_RegistrationType
        {
            get => _V_RegistrationType; set
            {
                _V_RegistrationType = value;
                RaisePropertyChanged("V_RegistrationType");
            }
        }
        [DataMemberAttribute]
        public string FinalizedReceiptNum
        {
            get => _FinalizedReceiptNum; set
            {
                _FinalizedReceiptNum = value;
                RaisePropertyChanged("FinalizedReceiptNum");
            }
        }
        [DataMemberAttribute]
        public decimal Amount
        {
            get => _Amount; set
            {
                _Amount = value;
                RaisePropertyChanged("Amount");
            }
        }
        [DataMemberAttribute]
        public decimal TotalHasVATAmount
        {
            get => _TotalHasVATAmount; set
            {
                _TotalHasVATAmount = value;
                RaisePropertyChanged("TotalHasVATAmount");
            }
        }
        [DataMemberAttribute]
        public double? VATPercent
        {
            get => _VATPercent; set
            {
                _VATPercent = value;
                RaisePropertyChanged("VATPercent");
            }
        }
        [DataMemberAttribute]
        public string TaxMemberName
        {
            get => _TaxMemberName; set
            {
                _TaxMemberName = value;
                RaisePropertyChanged("TaxMemberName");
                RaisePropertyChanged("IsTaxMemberSameWithPatient");
            }
        }
        [DataMemberAttribute]
        public string TaxMemberAddress
        {
            get => _TaxMemberAddress; set
            {
                _TaxMemberAddress = value;
                RaisePropertyChanged("TaxMemberAddress");
            }
        }
        [DataMemberAttribute]
        public string TaxCode
        {
            get => _TaxCode; set
            {
                _TaxCode = value;
                RaisePropertyChanged("TaxCode");
            }
        }
        [DataMemberAttribute]
        public long V_PaymentMode
        {
            get => _V_PaymentMode; set
            {
                _V_PaymentMode = value;
                RaisePropertyChanged("V_PaymentMode");
            }
        }
        [DataMemberAttribute]
        public string BankAccountNumber
        {
            get => _BankAccountNumber; set
            {
                _BankAccountNumber = value;
                RaisePropertyChanged("BankAccountNumber");
            }
        }
        [DataMemberAttribute]
        public string Denominator
        {
            get => _Denominator; set
            {
                _Denominator = value;
                RaisePropertyChanged("Denominator");
            }
        }
        [DataMemberAttribute]
        public string InvoiceNumb
        {
            get => _InvoiceNumb; set
            {
                _InvoiceNumb = value;
                RaisePropertyChanged("InvoiceNumb");
            }
        }
        [DataMemberAttribute]
        public string Symbol
        {
            get => _Symbol; set
            {
                _Symbol = value;
                RaisePropertyChanged("Symbol");
            }
        }
        [DataMemberAttribute]
        public List<long> PtRegistrationIDCollection
        {
            get => _PtRegistrationIDCollection; set
            {
                _PtRegistrationIDCollection = value;
                RaisePropertyChanged("PtRegistrationIDCollection");
            }
        }
        [DataMemberAttribute]
        public long TransactionFinalizationSummaryInfoID
        {
            get => _TransactionFinalizationSummaryInfoID; set
            {
                _TransactionFinalizationSummaryInfoID = value;
                RaisePropertyChanged("TransactionFinalizationSummaryInfoID");
            }
        }
        [DataMemberAttribute]
        public Staff CreatedStaff
        {
            get => _CreatedStaff; set
            {
                _CreatedStaff = value;
                RaisePropertyChanged("CreatedStaff");
            }
        }
        [DataMemberAttribute]
        public DateTime? DateInvoice
        {
            get => _DateInvoice; set
            {
                _DateInvoice = value;
                RaisePropertyChanged("DateInvoice");
            }
        }
        [DataMemberAttribute]
        public string eInvoiceToken
        {
            get => _eInvoiceToken; set
            {
                _eInvoiceToken = value;
                RaisePropertyChanged("eInvoiceToken");
            }
        }
        [DataMemberAttribute]
        public bool IsSelected
        {
            get
            {
                return _IsSelected;
            }
            set
            {
                _IsSelected = value;
                RaisePropertyChanged("IsSelected");
            }
        }
        [DataMemberAttribute]
        public bool IsDeleted
        {
            get
            {
                return _IsDeleted;
            }
            set
            {
                _IsDeleted = value;
                RaisePropertyChanged("IsDeleted");
            }
        }
        [DataMemberAttribute]
        public string eInvoiceKey
        {
            get
            {
                return _eInvoiceKey;
            }
            set
            {
                _eInvoiceKey = value;
                RaisePropertyChanged("eInvoiceKey");
                RaisePropertyChanged("InvoiceKey");
            }
        }
        [DataMemberAttribute]
        public string PatientFullName
        {
            get
            {
                return _PatientFullName;
            }
            set
            {
                _PatientFullName = value;
                RaisePropertyChanged("PatientFullName");
            }
        }
        [DataMemberAttribute]
        public long? GenericPaymentID
        {
            get
            {
                return _GenericPaymentID;
            }
            set
            {
                _GenericPaymentID = value;
                RaisePropertyChanged("GenericPaymentID");
            }
        }
        public string InvoiceKey
        {
            get
            {
                return !string.IsNullOrEmpty(eInvoiceKey) ? eInvoiceKey : ((V_RegistrationType == (long)AllLookupValues.RegistrationType.NGOAI_TRU ? "1-" : "3-") + PtRegistrationID.ToString());
            }
        }
        [DataMemberAttribute]
        public long? outiID
        {
            get
            {
                return _outiID;
            }
            set
            {
                _outiID = value;
                RaisePropertyChanged("outiID");
            }
        }
        [DataMemberAttribute]
        public string ReceiptDetails
        {
            get
            {
                return _ReceiptDetails;
            }
            set
            {
                _ReceiptDetails = value;
                RaisePropertyChanged("ReceiptDetails");
            }
        }
        #region Methods
        public bool IsTaxMemberSameWithPatient
        {
            get
            {
                if (!string.IsNullOrEmpty(PatientFullName)
                    && !PatientFullName.Equals(TaxMemberName))
                {
                    return false;
                }
                return true;
            }
        }
        #endregion
        public bool IsOrganization { get; set; } = false;
    }
}