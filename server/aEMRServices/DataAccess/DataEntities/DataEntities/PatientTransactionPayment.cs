/*
20161231 #001 CMN: Add variable for VAT
*/
using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{
    public partial class PatientTransactionPayment : NotifyChangedBase
    {
        public PatientTransactionPayment()
        {
            _creditOrDebit = 1;
        }
        #region Factory Method


        /// Create a new PatientTransactionPayment object.

        /// <param name="PtTranPaymtID">Initial value of the PtTranPaymtID property.</param>
        /// <param name="payAmount">Initial value of the PayAmount property.</param>
        public static PatientTransactionPayment CreatePatientTransactionPayment(long PtTranPaymtID, Decimal payAmount)
        {
            var PatientTransactionPayment = new PatientTransactionPayment { PtTranPaymtID = PtTranPaymtID, PayAmount = payAmount };
            return PatientTransactionPayment;
        }

        #endregion
        #region Primitive Properties

        [DataMemberAttribute()]
        public long PtTranPaymtID
        {
            get
            {
                return _PtTranPaymtID;
            }
            set
            {
                if (_PtTranPaymtID != value)
                {
                    OnPtTranPaymtIDChanging(value);
                    _PtTranPaymtID = value;
                    RaisePropertyChanged("PtTranPaymtID");
                    OnPtTranPaymtIDChanged();
                }
            }
        }
        private long _PtTranPaymtID;
        partial void OnPtTranPaymtIDChanging(long value);
        partial void OnPtTranPaymtIDChanged();


        [DataMemberAttribute()]
        public DateTime RecCreatedDate
        {
            get
            {
                return _RecCreatedDate;
            }
            set
            {
                if (_RecCreatedDate != value)
                {
                    OnRecCreatedDateChanging(value);
                    _RecCreatedDate = value;
                    RaisePropertyChanged("RecCreatedDate");
                    OnRecCreatedDateChanged();
                }
            }
        }
        private DateTime _RecCreatedDate;
        partial void OnRecCreatedDateChanging(DateTime value);
        partial void OnRecCreatedDateChanged();

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

        private long? _TransItemID;
        [DataMemberAttribute()]
        public long? TransItemID
        {
            get
            {
                return _TransItemID;
            }
            set
            {
                _TransItemID = value;
                RaisePropertyChanged("TransItemID");
            }
        }


        partial void OnPtPmtAccIDChanging(long? value);
        partial void OnPtPmtAccIDChanged();

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
        public long? TranFinalizationID
        {
            get
            {
                return _TranFinalizationID;
            }
            set
            {
                if (_TranFinalizationID != value)
                {
                    OnTranFinalizationIDChanging(value);
                    _TranFinalizationID = value;
                    RaisePropertyChanged("TranFinalizationID");
                    OnTranFinalizationIDChanged();
                }
            }
        }
        private long? _TranFinalizationID;
        partial void OnTranFinalizationIDChanging(long? value);
        partial void OnTranFinalizationIDChanged();

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
        public Decimal PayAdvance
        {
            get
            {
                return _PayAdvance;
            }
            set
            {
                _PayAdvance = value;
                RaisePropertyChanged("PayAdvance");
            }
        }
        private Decimal _PayAdvance;

        [DataMemberAttribute()]
        public Decimal HIAmount
        {
            get
            {
                return _HIAmount;
            }
            set
            {
                _HIAmount = value;
                RaisePropertyChanged("HIAmount");
            }
        }
        private Decimal _HIAmount;

        [DataMemberAttribute()]
        public Decimal TotalSupport
        {
            get
            {
                return _TotalSupport;
            }
            set
            {
                _TotalSupport = value;
                RaisePropertyChanged("TotalSupport");
            }
        }
        private Decimal _TotalSupport;

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

        private long _CollectorDeptLocID;
        [DataMemberAttribute]
        public long CollectorDeptLocID
        {
            get
            {
                return _CollectorDeptLocID;
            }
            set
            {
                _CollectorDeptLocID = value;
                RaisePropertyChanged("CollectorDeptLocID");
            }
        }

        private long _DeletedStaffID;
        [DataMemberAttribute]
        public long DeletedStaffID
        {
            get
            {
                return _DeletedStaffID;
            }
            set
            {
                _DeletedStaffID = value;
                RaisePropertyChanged("DeletedStaffID");
            }
        }


        private string _DeletedStaffName;
        [DataMemberAttribute]
        public string DeletedStaffName
        {
            get
            {
                return _DeletedStaffName;
            }
            set
            {
                _DeletedStaffName = value;
                RaisePropertyChanged("DeletedStaffName");
            }
        }

        private String _TranPaymtNote;
        [DataMemberAttribute]
        public String TranPaymtNote
        {
            get
            {
                return _TranPaymtNote;
            }
            set
            {
                _TranPaymtNote = value;
                RaisePropertyChanged("TranPaymtNote");
            }
        }

        [DataMemberAttribute()]
        public Int16 TranPaymtStatus
        {
            get
            {
                return _TranPaymtStatus;
            }
            set
            {
                if (_TranPaymtStatus != value)
                {
                    OnTranPaymtStatusChanging(value);
                    _TranPaymtStatus = value;
                    RaisePropertyChanged("TranPaymtStatus");
                    OnTranPaymtStatusChanged();
                }
            }
        }
        private Int16 _TranPaymtStatus;
        partial void OnTranPaymtStatusChanging(Int16 value);
        partial void OnTranPaymtStatusChanged();

        private bool? _IsDeleted;
        [DataMemberAttribute()]
        public bool? IsDeleted
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

        private bool _Reported;
        [DataMemberAttribute()]
        public bool Reported
        {
            get
            {
                return _Reported;
            }
            set
            {
                _Reported = value;
                RaisePropertyChanged("Reported");
            }
        }

        private bool _hasDetail;//bien nay dung de xac dinh payment co dich vu nao de tinh tien hay ko
        [DataMemberAttribute()]
        public bool hasDetail
        {
            get
            {
                return _hasDetail;
            }
            set
            {
                _hasDetail = value;
                RaisePropertyChanged("hasDetail");
            }
        }


        //chua tam thoi
        public string XMLLink { get; set; }

        public string XMLService { get; set; }
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
            PatientTransactionPayment info = obj as PatientTransactionPayment;
            if (info == null)
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;

            return this.PtTranPaymtID == info.PtTranPaymtID;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public long V_TradingPlaces { get; set; }

        [DataMemberAttribute()]
        public Lookup V_RefundPaymentReasonInPt
        {
            get
            {
                return _V_RefundPaymentReasonInPt;
            }
            set
            {
                _V_RefundPaymentReasonInPt = value;
                RaisePropertyChanged("V_RefundPaymentReasonInPt");
            }
        }
        private Lookup _V_RefundPaymentReasonInPt;


        private long? _OutPtCashAdvanceID;
        [DataMemberAttribute()]
        public long? OutPtCashAdvanceID
        {
            get
            {
                return _OutPtCashAdvanceID;
            }
            set
            {
                _OutPtCashAdvanceID = value;
                RaisePropertyChanged("OutPtCashAdvanceID");
            }
        }


        private decimal _DiscountAmount = 0;
        [DataMemberAttribute]
        public decimal DiscountAmount
        {
            get => _DiscountAmount; set
            {
                _DiscountAmount = value;
                RaisePropertyChanged("DiscountAmount");
            }
        }
    }

    public partial class PatientTranacsionPaymentLink : NotifyChangedBase
    {
        #region Factory Method

        /// Create a new PatientTranacsionPaymentLink object.

        /// <param name="PtTranPmtLinkID">Initial value of the PtTranPmtLinkID property.</param>
        /// <param name="payAmount">Initial value of the PayAmount property.</param>
        public static PatientTranacsionPaymentLink CreatePatientTranacsionPaymentLink(long PtTranPmtLinkID)
        {
            var PatientTranacsionPaymentLink = new PatientTranacsionPaymentLink { PtTranPmtLinkID = PtTranPmtLinkID };
            return PatientTranacsionPaymentLink;
        }

        #endregion
        #region Primitive Properties

        [DataMemberAttribute()]
        public long PtTranPmtLinkID
        {
            get
            {
                return _PtTranPmtLinkID;
            }
            set
            {
                if (_PtTranPmtLinkID != value)
                {
                    OnPtTranPmtLinkIDChanging(value);
                    _PtTranPmtLinkID = value;
                    RaisePropertyChanged("PtTranPmtLinkID");
                    OnPtTranPmtLinkIDChanged();
                }
            }
        }
        private long _PtTranPmtLinkID;
        partial void OnPtTranPmtLinkIDChanging(long value);
        partial void OnPtTranPmtLinkIDChanged();

        [DataMemberAttribute()]
        public long PtTranPaymtID
        {
            get
            {
                return _PtTranPaymtID;
            }
            set
            {
                if (_PtTranPaymtID != value)
                {
                    OnPtTranPaymtIDChanging(value);
                    _PtTranPaymtID = value;
                    RaisePropertyChanged("PtTranPaymtID");
                    OnPtTranPaymtIDChanged();
                }
            }
        }
        private long _PtTranPaymtID;
        partial void OnPtTranPaymtIDChanging(long value);
        partial void OnPtTranPaymtIDChanged();

        [DataMemberAttribute()]
        public long TransItemID
        {
            get
            {
                return _TransItemID;
            }
            set
            {
                if (_TransItemID != value)
                {
                    OnTransItemIDChanging(value);
                    _TransItemID = value;
                    RaisePropertyChanged("TransItemID");
                    OnTransItemIDChanged();
                }
            }
        }
        private long _TransItemID;
        partial void OnTransItemIDChanging(long value);
        partial void OnTransItemIDChanged();

        #endregion

        #region Navigation Properties

        private PatientTransactionPayment _CurPatientTransactionPayment;
        [DataMemberAttribute()]
        public PatientTransactionPayment CurPatientTransactionPayment
        {
            get
            {
                return _CurPatientTransactionPayment;
            }
            set
            {
                _CurPatientTransactionPayment = value;
                RaisePropertyChanged("CurPatientTransactionPayment");
            }
        }

        private PatientTransaction _CurPatientTransaction;
        [DataMemberAttribute()]
        public PatientTransaction CurPatientTransaction
        {
            get
            {
                return _CurPatientTransaction;
            }
            set
            {
                _CurPatientTransaction = value;
                RaisePropertyChanged("CurPatientTransaction");
            }
        }

        #endregion

        public override bool Equals(object obj)
        {
            PatientTranacsionPaymentLink info = obj as PatientTranacsionPaymentLink;
            if (info == null)
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;

            return this.PtTranPmtLinkID == info.PtTranPmtLinkID;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    [DataContract]
    public partial class PatientPaymentAccount : NotifyChangedBase
    {
        #region Factory Method


        /// Create a new PatientPayment object.

        /// <param name="ptPmtID">Initial value of the PtPmtID property.</param>
        /// <param name="payAmount">Initial value of the PayAmount property.</param>
        public static PatientPaymentAccount CreatePatientPaymentAccount(long ptPmtAccID, Decimal payAmount)
        {
            var patientPayment = new PatientPaymentAccount { PtPmtAccID = ptPmtAccID };
            return patientPayment;
        }

        #endregion
        #region Primitive Properties

        [DataMemberAttribute()]
        public long PtPmtAccID
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
        private long _PtPmtAccID;
        partial void OnPtPmtAccIDChanging(long value);
        partial void OnPtPmtAccIDChanged();

        [DataMemberAttribute()]
        public String AccCode
        {
            get
            {
                return _AccCode;
            }
            set
            {
                OnAccCodeChanging(value);
                _AccCode = value;
                RaisePropertyChanged("AccCode");
                OnAccCodeChanged();
            }
        }
        private String _AccCode;
        partial void OnAccCodeChanging(String value);
        partial void OnAccCodeChanged();

        [DataMemberAttribute()]
        public String AccName
        {
            get
            {
                return _AccName;
            }
            set
            {
                OnAccNameChanging(value);
                _AccName = value;
                RaisePropertyChanged("AccName");
                OnAccNameChanged();
            }
        }
        private String _AccName;
        partial void OnAccNameChanging(String value);
        partial void OnAccNameChanged();

        [DataMemberAttribute()]
        public DateTime RecCreatedDate
        {
            get
            {
                return _RecCreatedDate;
            }
            set
            {
                OnRecCreatedDateChanging(value);
                _RecCreatedDate = value;
                RaisePropertyChanged("RecCreatedDate");
                OnRecCreatedDateChanged();
            }
        }
        private DateTime _RecCreatedDate;
        partial void OnRecCreatedDateChanging(DateTime value);
        partial void OnRecCreatedDateChanged();

        [DataMemberAttribute()]
        public DateTime? AccOpenDate
        {
            get
            {
                return _AccOpenDate;
            }
            set
            {
                OnAccOpenDateChanging(value);
                _AccOpenDate = value;
                RaisePropertyChanged("AccOpenDate");
                OnAccOpenDateChanged();
            }
        }
        private DateTime? _AccOpenDate;
        partial void OnAccOpenDateChanging(DateTime? value);
        partial void OnAccOpenDateChanged();

        [DataMemberAttribute()]
        public String AccOwner
        {
            get
            {
                return _AccOwner;
            }
            set
            {
                OnAccOwnerChanging(value);
                _AccOwner = value;
                RaisePropertyChanged("AccOwner");
                OnAccOwnerChanged();
            }
        }
        private String _AccOwner;
        partial void OnAccOwnerChanging(String value);
        partial void OnAccOwnerChanged();

        [DataMemberAttribute()]
        public String AccAddress
        {
            get
            {
                return _AccAddress;
            }
            set
            {
                OnAccAddressChanging(value);
                _AccAddress = value;
                RaisePropertyChanged("AccAddress");
                OnAccAddressChanged();
            }
        }
        private String _AccAddress;
        partial void OnAccAddressChanging(String value);
        partial void OnAccAddressChanged();

        [DataMemberAttribute()]
        public String AccTelephone
        {
            get
            {
                return _AccTelephone;
            }
            set
            {
                OnAccTelephoneChanging(value);
                _AccTelephone = value;
                RaisePropertyChanged("AccTelephone");
                OnAccTelephoneChanged();
            }
        }
        private String _AccTelephone;
        partial void OnAccTelephoneChanging(String value);
        partial void OnAccTelephoneChanged();

        [DataMemberAttribute()]
        public String ContactName
        {
            get
            {
                return _ContactName;
            }
            set
            {
                OnContactNameChanging(value);
                _ContactName = value;
                RaisePropertyChanged("ContactName");
                OnContactNameChanged();
            }
        }
        private String _ContactName;
        partial void OnContactNameChanging(String value);
        partial void OnContactNameChanged();

        [DataMemberAttribute()]
        public String ContactTelephone
        {
            get
            {
                return _ContactTelephone;
            }
            set
            {
                OnContactTelephoneChanging(value);
                _ContactTelephone = value;
                RaisePropertyChanged("ContactTelephone");
                OnContactTelephoneChanged();
            }
        }
        private String _ContactTelephone;
        partial void OnContactTelephoneChanging(String value);
        partial void OnContactTelephoneChanged();

        [DataMemberAttribute()]
        public bool? IsActive
        {
            get
            {
                return _IsActive;
            }
            set
            {
                OnIsActiveChanging(value);
                _IsActive = value;
                RaisePropertyChanged("IsActive");
                OnIsActiveChanged();
            }
        }
        private bool? _IsActive;
        partial void OnIsActiveChanging(bool? value);
        partial void OnIsActiveChanged();

        [DataMemberAttribute()]
        public String AccNote
        {
            get
            {
                return _AccNote;
            }
            set
            {
                OnAccNoteChanging(value);
                _AccNote = value;
                RaisePropertyChanged("AccNote");
                OnAccNoteChanged();
            }
        }
        private String _AccNote;
        partial void OnAccNoteChanging(String value);
        partial void OnAccNoteChanged();
        #endregion

        public override bool Equals(object obj)
        {
            PatientPaymentAccount info = obj as PatientPaymentAccount;
            if (info == null)
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;

            return this.PtPmtAccID == info.PtPmtAccID;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    [DataContract]
    public partial class PatientCashAdvance : NotifyChangedBase
    {
        #region Factory Method


        /// Create a new PatientTransaction object.

        /// <param name="PtCashAdvanceID">Initial value of the TransactionID property.</param>
        /// <param name="RecCreatedDate">Initial value of the RecCreatedDate property.</param>
        public static PatientCashAdvance CreatePatientCashAdvance(long PtCashAdvanceID, DateTime RecCreatedDate)
        {
            PatientCashAdvance patientTransaction = new PatientCashAdvance();
            patientTransaction.PtCashAdvanceID = PtCashAdvanceID;
            patientTransaction.RecCreatedDate = RecCreatedDate;
            return patientTransaction;
        }

        #endregion
        #region Primitive Properties

        [DataMemberAttribute()]
        public long PtCashAdvanceID
        {
            get
            {
                return _PtCashAdvanceID;
            }
            set
            {
                if (_PtCashAdvanceID != value)
                {
                    OnPtCashAdvanceIDChanging(value);
                    _PtCashAdvanceID = value;
                    RaisePropertyChanged("PtCashAdvanceID");
                    OnPtCashAdvanceIDChanged();
                }
            }
        }
        private long _PtCashAdvanceID;
        partial void OnPtCashAdvanceIDChanging(long value);
        partial void OnPtCashAdvanceIDChanged();

        [DataMemberAttribute()]
        public Int64 PtRegistrationID
        {
            get
            {
                return _PtRegistrationID;
            }
            set
            {
                OnPtRegistrationIDChanging(value);
                _PtRegistrationID = value;
                RaisePropertyChanged("PtRegistrationID");
                OnPtRegistrationIDChanged();
            }
        }
        private Int64 _PtRegistrationID;
        partial void OnPtRegistrationIDChanging(Int64 value);
        partial void OnPtRegistrationIDChanged();


        [DataMemberAttribute()]
        public String CashAdvReceiptNum
        {
            get
            {
                return _CashAdvReceiptNum;
            }
            set
            {
                OnCashAdvReceiptNumChanging(value);
                _CashAdvReceiptNum = value;
                RaisePropertyChanged("CashAdvReceiptNum");
                OnCashAdvReceiptNumChanged();
            }
        }
        private String _CashAdvReceiptNum;
        partial void OnCashAdvReceiptNumChanging(String value);
        partial void OnCashAdvReceiptNumChanged();

        [DataMemberAttribute()]
        public DateTime RecCreatedDate
        {
            get
            {
                return _RecCreatedDate;
            }
            set
            {
                OnRecCreatedDateChanging(value);
                _RecCreatedDate = value;
                RaisePropertyChanged("RecCreatedDate");
                OnRecCreatedDateChanged();
            }
        }
        private DateTime _RecCreatedDate;
        partial void OnRecCreatedDateChanging(DateTime value);
        partial void OnRecCreatedDateChanged();

        [DataMemberAttribute()]
        public DateTime PaymentDate
        {
            get
            {
                return _PaymentDate;
            }
            set
            {
                OnPaymentDateChanging(value);
                _PaymentDate = value;
                RaisePropertyChanged("PaymentDate");
                OnPaymentDateChanged();
            }
        }
        private DateTime _PaymentDate;
        partial void OnPaymentDateChanging(DateTime value);
        partial void OnPaymentDateChanged();

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


        [DataMemberAttribute()]
        public String GeneralNote
        {
            get
            {
                return _GeneralNote;
            }
            set
            {
                OnGeneralNoteChanging(value);
                _GeneralNote = value;
                RaisePropertyChanged("GeneralNote");
                OnGeneralNoteChanged();
            }
        }
        private String _GeneralNote;
        partial void OnGeneralNoteChanging(String value);
        partial void OnGeneralNoteChanged();


        private long _V_CashAdvanceType;
        [DataMemberAttribute()]
        public long V_CashAdvanceType
        {
            get
            {
                return _V_CashAdvanceType;
            }
            set
            {
                _V_CashAdvanceType = value;
                RaisePropertyChanged("V_CashAdvanceType");
            }
        }

        private Lookup _V_PaymentReason;
        [DataMemberAttribute()]
        public Lookup V_PaymentReason
        {
            get
            {
                return _V_PaymentReason;
            }
            set
            {
                _V_PaymentReason = value;
                RaisePropertyChanged("V_PaymentReason");
            }
        }

        private Lookup _V_PaymentMode;
        [DataMemberAttribute()]
        public Lookup V_PaymentMode
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

        private long? _RptPtCashAdvRemID;
        [DataMemberAttribute]
        public long? RptPtCashAdvRemID
        {
            get
            {
                return _RptPtCashAdvRemID;
            }
            set
            {
                _RptPtCashAdvRemID = value;
                RaisePropertyChanged("RptPtCashAdvRemID");
            }
        }
        #endregion

        /// <summary>
        /// 31-08-2012 Dinh
        /// Thêm trạng thái để phân biệt nội trú và ngoại trú
        /// </summary>
        private RegistrationType _RegistrationType;
        [DataMemberAttribute()]
        public RegistrationType RegistrationType
        {
            get
            {
                return _RegistrationType;
            }
            set
            {
                _RegistrationType = value;
                RaisePropertyChanged("RegistrationType");
            }
        }


        private AllLookupValues.RegistrationType _V_RegistrationType = AllLookupValues.RegistrationType.NGOAI_TRU;
        [DataMemberAttribute()]
        public AllLookupValues.RegistrationType V_RegistrationType
        {
            get
            {
                return _V_RegistrationType;
            }
            set
            {
                _V_RegistrationType = value;
                RaisePropertyChanged("V_RegistrationType");
            }
        }

        [DataMemberAttribute()]
        public Decimal PaymentAmount
        {
            get
            {
                return _PaymentAmount;
            }
            set
            {
                OnPayAmountChanging(value);
                _PaymentAmount = value;
                RaisePropertyChanged("PaymentAmount");
                OnPayAmountChanged();
            }
        }
        private Decimal _PaymentAmount;
        partial void OnPayAmountChanging(Decimal value);
        partial void OnPayAmountChanged();

        [DataMemberAttribute()]
        public Decimal BalanceAmount
        {
            get
            {
                return _BalanceAmount;
            }
            set
            {
                OnBalanceAmountChanging(value);
                _BalanceAmount = value;
                RaisePropertyChanged("BalanceAmount");
                OnBalanceAmountChanged();
            }
        }
        private Decimal _BalanceAmount;
        partial void OnBalanceAmountChanging(Decimal value);
        partial void OnBalanceAmountChanged();

        [DataMemberAttribute()]
        public String FullName
        {
            get
            {
                return _FullName;
            }
            set
            {
                _FullName = value;
                RaisePropertyChanged("FullName");
            }
        }
        private String _FullName;

        private RptPatientCashAdvReminder _RptPatientCashAdvReminder;
        [DataMemberAttribute()]
        public RptPatientCashAdvReminder RptPatientCashAdvReminder
        {
            get
            {
                return _RptPatientCashAdvReminder;
            }
            set
            {
                _RptPatientCashAdvReminder = value;
                RaisePropertyChanged("RptPatientCashAdvReminder");
            }
        }


        [DataMemberAttribute()]
        public long InPatientBillingInvID
        {
            get
            {
                return _InPatientBillingInvID;
            }
            set
            {
                _InPatientBillingInvID = value;
                RaisePropertyChanged("InPatientBillingInvID");
            }
        }
        private long _InPatientBillingInvID;


        [DataMemberAttribute()]
        public string BillingInvNum
        {
            get
            {
                return _BillingInvNum;
            }
            set
            {
                _BillingInvNum = value;
                RaisePropertyChanged("BillingInvNum");
            }
        }
        private string _BillingInvNum;

        private string _RemCode;
        [DataMemberAttribute]
        public string RemCode
        {
            get
            {
                return _RemCode;
            }
            set
            {
                if (_RemCode == value)
                {
                    return;
                }
                _RemCode = value;
                RaisePropertyChanged("RemCode");
            }
        }
		// VuTTM Begin
		[DataMemberAttribute()]
		public long? BankingTrxId
		{
			get
			{
				return _BankingTrxId;
			}
			set
			{
				_BankingTrxId = value;
				RaisePropertyChanged("BankingTrxId");
			}
		}
		private long? _BankingTrxId;

		[DataMemberAttribute()]
		public long? BankingRefundTrxId
		{
			get
			{
				return _BankingRefundTrxId;
			}
			set
			{
				_BankingRefundTrxId = value;
				RaisePropertyChanged("BankingRefundTrxId");
			}
		}
		private long? _BankingRefundTrxId;
		// VuTTM End
	}
	[DataContract]
    public partial class GenericPayment : NotifyChangedBase
    {
        [DataMemberAttribute()]
        public string PatientCode
        {
            get
            {
                return _patientCode;
            }
            set
            {
                _patientCode = value;
                RaisePropertyChanged("PatientCode");
            }
        }
        private string _patientCode;

        [DataMemberAttribute()]
        public string FileCodeNumber
        {
            get
            {
                return _fileCodeNumber;
            }
            set
            {
                _fileCodeNumber = value;
                RaisePropertyChanged("FileCodeNumber");
            }
        }
        private string _fileCodeNumber;


        [DataMemberAttribute()]
        public string PersonName
        {
            get
            {
                return _PersonName;
            }
            set
            {
                _PersonName = value;
                RaisePropertyChanged("PersonName");
            }
        }
        private string _PersonName;

        [DataMemberAttribute()]
        public string OrgName
        {
            get
            {
                return _OrgName;
            }
            set
            {
                _OrgName = value;
                RaisePropertyChanged("OrgName");
            }
        }
        private string _OrgName;

        [DataMemberAttribute()]
        public string PersonAddress
        {
            get
            {
                return _PersonAddress;
            }
            set
            {
                _PersonAddress = value;
                RaisePropertyChanged("PersonAddress");
            }
        }
        private string _PersonAddress;

        [DataMemberAttribute()]
        public Decimal PaymentAmount
        {
            get
            {
                return _PaymentAmount;
            }
            set
            {
                _PaymentAmount = value;
                RaisePropertyChanged("PaymentAmount");
            }
        }
        private Decimal _PaymentAmount;

        [DataMemberAttribute()]
        public long? PatientID
        {
            get
            {
                return _PatientID;
            }
            set
            {
                _PatientID = value;
                RaisePropertyChanged("PatientID");
            }
        }
        private long? _PatientID;

        [DataMemberAttribute()]
        public string DOB
        {
            get
            {
                return _DOB;
            }
            set
            {
                _DOB = value;
                RaisePropertyChanged("DOB");
            }
        }
        private string _DOB;

        [DataMemberAttribute()]
        public string V_GenericPaymentReason
        {
            get
            {
                return _V_GenericPaymentReason;
            }
            set
            {
                _V_GenericPaymentReason = value;
                RaisePropertyChanged("V_GenericPaymentReason");
            }
        }
        private string _V_GenericPaymentReason;

        [DataMemberAttribute()]
        public long LookupReasonID
        {
            get
            {
                return _LookupReasonID;
            }
            set
            {
                _LookupReasonID = value;
                RaisePropertyChanged("LookupReasonID");
            }
        }
        private long _LookupReasonID;

        [DataMemberAttribute()]
        public string LookupReasonName
        {
            get
            {
                return _LookupReasonName;
            }
            set
            {
                _LookupReasonName = value;
                RaisePropertyChanged("LookupReasonName");
            }
        }
        private string _LookupReasonName;


        [DataMemberAttribute()]
        public string ReasonDetail
        {
            get
            {
                return _ReasonDetail;
            }
            set
            {
                _ReasonDetail = value;
                RaisePropertyChanged("ReasonDetail");
            }
        }
        private string _ReasonDetail;


        [DataMemberAttribute()]
        public string GeneralNote
        {
            get
            {
                return _GeneralNote;
            }
            set
            {
                _GeneralNote = value;
                RaisePropertyChanged("GeneralNote");
            }
        }
        private string _GeneralNote;

        [DataMemberAttribute()]
        public string PhoneNumber
        {
            get
            {
                return _PhoneNumber;
            }
            set
            {
                _PhoneNumber = value;
                RaisePropertyChanged("PhoneNumber");
            }
        }
        private string _PhoneNumber;

        //HPT: Phiếu thu khác sử dụng cho hai mục đích: thu tiền và đổi biên lai
        [DataMemberAttribute()]
        public long V_GenericPaymentType
        {
            get
            {
                return _V_GenericPaymentType;
            }
            set
            {
                _V_GenericPaymentType = value;
                RaisePropertyChanged("V_GenericPaymentType");
            }
        }
        private long _V_GenericPaymentType;

        [DataMemberAttribute()]
        public string V_GenericPaymentTypeName
        {
            get
            {
                return _V_GenericPaymentTypeName;
            }
            set
            {
                _V_GenericPaymentTypeName = value;
                RaisePropertyChanged("V_GenericPaymentTypeName");
            }
        }
        private string _V_GenericPaymentTypeName;


        [DataMemberAttribute()]
        public DateTime PaymentDate
        {
            get
            {
                return _PaymentDate;
            }
            set
            {
                _PaymentDate = value;
                RaisePropertyChanged("PaymentDate");
            }
        }
        private DateTime _PaymentDate;

        [DataMemberAttribute()]
        public string GenericPaymentCode
        {
            get
            {
                return _GenericPaymentCode;
            }
            set
            {
                _GenericPaymentCode = value;
                RaisePropertyChanged("GenericPaymentCode");
            }
        }
        private string _GenericPaymentCode;

        [DataMemberAttribute()]
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
        private long? _GenericPaymentID;

        [DataMemberAttribute()]
        public long? StaffID
        {
            get
            {
                return _StaffID;
            }
            set
            {
                _StaffID = value;
                RaisePropertyChanged("StaffID");
            }
        }
        private long? _StaffID;

        [DataMemberAttribute()]
        public string StaffName
        {
            get
            {
                return _StaffName;
            }
            set
            {
                _StaffName = value;
                RaisePropertyChanged("StaffName");
            }
        }
        private string _StaffName;


        [DataMemberAttribute()]
        public DateTime RecCreatedDate
        {
            get
            {
                return _RecCreatedDate;
            }
            set
            {
                _RecCreatedDate = value;
                RaisePropertyChanged("RecCreatedDate");
            }
        }
        private DateTime _RecCreatedDate;

        [DataMemberAttribute()]
        public string V_Status
        {
            get
            {
                return _V_Status;
            }
            set
            {
                _V_Status = value;
                RaisePropertyChanged("V_Status");
            }
        }
        private string _V_Status;

        [DataMemberAttribute()]
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
        private bool _IsDeleted;

        //==== #001
        [DataMemberAttribute()]
        public Decimal? VATAmount
        {
            get
            {
                return _VATAmount;
            }
            set
            {
                _VATAmount = value;
                RaisePropertyChanged("VATAmount");
            }
        }
        private Decimal? _VATAmount;

        [DataMemberAttribute()]
        public double? VATPercent
        {
            get
            {
                return _VATPercent;
            }
            set
            {
                _VATPercent = value;
                RaisePropertyChanged("VATPercent");
            }
        }
        private double? _VATPercent;
        //==== #001

        [DataMemberAttribute]
        public string InvoiceNumber
        {
            get
            {
                return _InvoiceNumber;
            }
            set
            {
                _InvoiceNumber = value;
                RaisePropertyChanged("InvoiceNumber");
            }
        }
        private string _InvoiceNumber;
    }

    public partial class PatientCashAdvTranPaymentLink : NotifyChangedBase
    {
        #region Factory Method

        /// Create a new PatientTranacsionPaymentLink object.

        /// <param name="PtTranPmtLinkID">Initial value of the PtTranPmtLinkID property.</param>
        /// <param name="payAmount">Initial value of the PayAmount property.</param>
        public static PatientCashAdvTranPaymentLink CreatePatientCashAdvTranPaymentLink(long PtCashAdvTranPayLinkID)
        {
            var PatientCashAdvTranPaymentLink = new PatientCashAdvTranPaymentLink { PtCashAdvTranPayLinkID = PtCashAdvTranPayLinkID };
            return PatientCashAdvTranPaymentLink;
        }

        #endregion
        #region Primitive Properties

        [DataMemberAttribute()]
        public long PtCashAdvTranPayLinkID
        {
            get
            {
                return _PtCashAdvTranPayLinkID;
            }
            set
            {
                if (_PtCashAdvTranPayLinkID != value)
                {
                    OnPtCashAdvTranPayLinkIDChanging(value);
                    _PtCashAdvTranPayLinkID = value;
                    RaisePropertyChanged("PtCashAdvTranPayLinkID");
                    OnPtCashAdvTranPayLinkIDChanged();
                }
            }
        }
        private long _PtCashAdvTranPayLinkID;
        partial void OnPtCashAdvTranPayLinkIDChanging(long value);
        partial void OnPtCashAdvTranPayLinkIDChanged();

        [DataMemberAttribute()]
        public long PtTranPaymtID
        {
            get
            {
                return _PtTranPaymtID;
            }
            set
            {
                if (_PtTranPaymtID != value)
                {
                    OnPtTranPaymtIDChanging(value);
                    _PtTranPaymtID = value;
                    RaisePropertyChanged("PtTranPaymtID");
                    OnPtTranPaymtIDChanged();
                }
            }
        }
        private long _PtTranPaymtID;
        partial void OnPtTranPaymtIDChanging(long value);
        partial void OnPtTranPaymtIDChanged();

        [DataMemberAttribute()]
        public long PtCashAdvanceID
        {
            get
            {
                return _PtCashAdvanceID;
            }
            set
            {
                if (_PtCashAdvanceID != value)
                {
                    OnPtCashAdvanceIDChanging(value);
                    _PtCashAdvanceID = value;
                    RaisePropertyChanged("PtCashAdvanceID");
                    OnPtCashAdvanceIDChanged();
                }
            }
        }
        private long _PtCashAdvanceID;
        partial void OnPtCashAdvanceIDChanging(long value);
        partial void OnPtCashAdvanceIDChanged();

        #endregion

        #region Navigation Properties

        private PatientTransactionPayment _CurPatientTransactionPayment;
        [DataMemberAttribute()]
        public PatientTransactionPayment CurPatientTransactionPayment
        {
            get
            {
                return _CurPatientTransactionPayment;
            }
            set
            {
                _CurPatientTransactionPayment = value;
                RaisePropertyChanged("CurPatientTransactionPayment");
            }
        }

        private PatientCashAdvance _CurPatientCashAdvance;
        [DataMemberAttribute()]
        public PatientCashAdvance CurPatientCashAdvance
        {
            get
            {
                return _CurPatientCashAdvance;
            }
            set
            {
                _CurPatientCashAdvance = value;
                RaisePropertyChanged("CurPatientCashAdvance");
            }
        }

        #endregion

        public override bool Equals(object obj)
        {
            PatientCashAdvTranPaymentLink info = obj as PatientCashAdvTranPaymentLink;
            if (info == null)
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;

            return this.PtCashAdvTranPayLinkID == info.PtCashAdvTranPayLinkID;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    [DataContract]
    public partial class RptPatientCashAdvReminder : NotifyChangedBase
    {
        #region Factory Method


        /// Create a new PatientTransaction object.

        /// <param name="RptPtCashAdvRemID">Initial value of the TransactionID property.</param>
        /// <param name="RecCreatedDate">Initial value of the RecCreatedDate property.</param>
        public static RptPatientCashAdvReminder CreateRptPatientCashAdvReminder(long RptPtCashAdvRemID, DateTime RecCreatedDate)
        {
            RptPatientCashAdvReminder patientTransaction = new RptPatientCashAdvReminder();
            patientTransaction.RptPtCashAdvRemID = RptPtCashAdvRemID;
            patientTransaction.RecCreatedDate = RecCreatedDate;
            return patientTransaction;
        }

        #endregion
        #region Primitive Properties

        [DataMemberAttribute()]
        public long RptPtCashAdvRemID
        {
            get
            {
                return _RptPtCashAdvRemID;
            }
            set
            {
                if (_RptPtCashAdvRemID != value)
                {
                    OnRptPtCashAdvRemIDChanging(value);
                    _RptPtCashAdvRemID = value;
                    RaisePropertyChanged("RptPtCashAdvRemID");
                    OnRptPtCashAdvRemIDChanged();
                    RaisePropertyChanged("CanNew");
                    RaisePropertyChanged("CanUpdate");
                }
            }
        }
        private long _RptPtCashAdvRemID;
        partial void OnRptPtCashAdvRemIDChanging(long value);
        partial void OnRptPtCashAdvRemIDChanged();

        [DataMemberAttribute()]
        public Int64 PtRegistrationID
        {
            get
            {
                return _PtRegistrationID;
            }
            set
            {
                OnPtRegistrationIDChanging(value);
                _PtRegistrationID = value;
                RaisePropertyChanged("PtRegistrationID");
                OnPtRegistrationIDChanged();
            }
        }
        private Int64 _PtRegistrationID;
        partial void OnPtRegistrationIDChanging(Int64 value);
        partial void OnPtRegistrationIDChanged();


        [DataMemberAttribute()]
        public String RemCode
        {
            get
            {
                return _RemCode;
            }
            set
            {
                OnRemCodeChanging(value);
                _RemCode = value;
                RaisePropertyChanged("RemCode");
                OnRemCodeChanged();
            }
        }
        private String _RemCode;
        partial void OnRemCodeChanging(String value);
        partial void OnRemCodeChanged();

        [DataMemberAttribute()]
        public DateTime RecCreatedDate
        {
            get
            {
                return _RecCreatedDate;
            }
            set
            {
                OnRecCreatedDateChanging(value);
                _RecCreatedDate = value;
                RaisePropertyChanged("RecCreatedDate");
                OnRecCreatedDateChanged();
            }
        }
        private DateTime _RecCreatedDate;
        partial void OnRecCreatedDateChanging(DateTime value);
        partial void OnRecCreatedDateChanged();

        [DataMemberAttribute()]
        public DateTime RemDate
        {
            get
            {
                return _RemDate;
            }
            set
            {
                OnRemDateChanging(value);
                _RemDate = value;
                RaisePropertyChanged("RemDate");
                OnRemDateChanged();
            }
        }
        private DateTime _RemDate;
        partial void OnRemDateChanging(DateTime value);
        partial void OnRemDateChanged();

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


        [DataMemberAttribute()]
        public String RemNote
        {
            get
            {
                return _RemNote;
            }
            set
            {
                OnRemNoteChanging(value);
                _RemNote = value;
                RaisePropertyChanged("RemNote");
                OnRemNoteChanged();
            }
        }
        private String _RemNote;
        partial void OnRemNoteChanging(String value);
        partial void OnRemNoteChanged();

        private long _V_CashAdvanceType;
        [DataMemberAttribute()]
        public long V_CashAdvanceType
        {
            get
            {
                return _V_CashAdvanceType;
            }
            set
            {
                _V_CashAdvanceType = value;
                RaisePropertyChanged("V_CashAdvanceType");
            }
        }

        private Lookup _V_PaymentReason;
        [DataMemberAttribute()]
        public Lookup V_PaymentReason
        {
            get
            {
                return _V_PaymentReason;
            }
            set
            {
                _V_PaymentReason = value;
                RaisePropertyChanged("V_PaymentReason");
            }
        }


        #endregion

        /// <summary>
        /// 31-08-2012 Dinh
        /// Thêm trạng thái để phân biệt nội trú và ngoại trú
        /// </summary>
        private RegistrationType _RegistrationType;
        [DataMemberAttribute()]
        public RegistrationType RegistrationType
        {
            get
            {
                return _RegistrationType;
            }
            set
            {
                _RegistrationType = value;
                RaisePropertyChanged("RegistrationType");
            }
        }


        private AllLookupValues.RegistrationType _V_RegistrationType = AllLookupValues.RegistrationType.NGOAI_TRU;
        [DataMemberAttribute()]
        public AllLookupValues.RegistrationType V_RegistrationType
        {
            get
            {
                return _V_RegistrationType;
            }
            set
            {
                _V_RegistrationType = value;
                RaisePropertyChanged("V_RegistrationType");
            }
        }

        [DataMemberAttribute()]
        public Decimal RemAmount
        {
            get
            {
                return _RemAmount;
            }
            set
            {
                OnRemAmountChanging(value);
                _RemAmount = value;
                RaisePropertyChanged("RemAmount");
                OnRemAmountChanged();
            }
        }
        private Decimal _RemAmount;
        partial void OnRemAmountChanging(Decimal value);
        partial void OnRemAmountChanged();

        [DataMemberAttribute()]
        public RefDepartment DepartmentSuggest
        {
            get
            {
                return _DepartmentSuggest;
            }
            set
            {
                if (_DepartmentSuggest != value)
                {
                    _DepartmentSuggest = value;
                    RaisePropertyChanged("DepartmentSuggest");
                }

            }
        }
        private RefDepartment _DepartmentSuggest;

        [DataMemberAttribute()]
        public Boolean Checked
        {
            get
            {
                return _Checked;
            }
            set
            {
                _Checked = value;
                RaisePropertyChanged("Checked");
            }
        }
        private Boolean _Checked;

        public bool CanNew
        {
            get { return RptPtCashAdvRemID <= 0; }
        }
        public bool CanUpdate
        {
            get { return RptPtCashAdvRemID > 0; }
        }

        [DataMemberAttribute()]
        public Lookup V_RefundPaymentReasonInPt
        {
            get
            {
                return _V_RefundPaymentReasonInPt;
            }
            set
            {
                _V_RefundPaymentReasonInPt = value;
                RaisePropertyChanged("V_RefundPaymentReasonInPt");
            }
        }
        private Lookup _V_RefundPaymentReasonInPt;
    }



    //PaymentID bigint,ServiceID bigint,ServiceTypeID bigint
    [DataContract]
    public partial class PaymentAndReceipt : NotifyChangedBase
    {
        #region Primitive Properties
        [DataMemberAttribute()]
        public long PaymentID
        {
            get
            {
                return _PaymentID;
            }
            set
            {
                if (_PaymentID != value)
                {
                    OnPaymentIDChanging(value);
                    _PaymentID = value;
                    RaisePropertyChanged("PaymentID");
                    OnPaymentIDChanged();
                }
            }
        }
        private long _PaymentID;
        partial void OnPaymentIDChanging(long value);
        partial void OnPaymentIDChanged();

        [DataMemberAttribute()]
        public Int64 ServiceID
        {
            get
            {
                return _ServiceID;
            }
            set
            {
                OnServiceIDChanging(value);
                _ServiceID = value;
                RaisePropertyChanged("ServiceID");
                OnServiceIDChanged();
            }
        }
        private Int64 _ServiceID;
        partial void OnServiceIDChanging(Int64 value);
        partial void OnServiceIDChanged();

        [DataMemberAttribute()]
        public long ServiceItemType
        {
            get
            {
                return _ServiceItemType;
            }
            set
            {
                _ServiceItemType = value;
                RaisePropertyChanged("ServiceItemType");
            }
        }
        private long _ServiceItemType;

        [DataMemberAttribute()]
        public long V_TradingPlaces
        {
            get
            {
                return _V_TradingPlaces;
            }
            set
            {
                _V_TradingPlaces = value;
                RaisePropertyChanged("V_TradingPlaces");
            }
        }
        private long _V_TradingPlaces;

        [DataMemberAttribute()]
        public int IsBalance
        {
            get
            {
                return _IsBalance;
            }
            set
            {
                _IsBalance = value;
                RaisePropertyChanged("IsBalance");
            }
        }
        private int _IsBalance;

        [DataMemberAttribute()]
        public decimal Amount
        {
            get
            {
                return _Amount;
            }
            set
            {
                _Amount = value;
                RaisePropertyChanged("Amount");
            }
        }
        private decimal _Amount;

        [DataMemberAttribute()]
        public bool IsPrintReceiptForPT
        {
            get
            {
                return _IsPrintReceiptForPT;
            }
            set
            {
                _IsPrintReceiptForPT = value;
                RaisePropertyChanged("IsPrintReceiptForPT");
            }
        }
        private bool _IsPrintReceiptForPT;

        [DataMemberAttribute()]
        public bool IsPrintReceiptForHI
        {
            get
            {
                return _IsPrintReceiptForHI;
            }
            set
            {
                _IsPrintReceiptForHI = value;
                RaisePropertyChanged("IsPrintReceiptForHI");
            }
        }
        private bool _IsPrintReceiptForHI;

        private long? _OutPtCashAdvanceID;
        [DataMemberAttribute()]
        public long? OutPtCashAdvanceID
        {
            get
            {
                return _OutPtCashAdvanceID;
            }
            set
            {
                _OutPtCashAdvanceID = value;
                RaisePropertyChanged("OutPtCashAdvanceID");
            }
        }

        private decimal _DiscountAmount = 0;
        [DataMemberAttribute]
        public decimal DiscountAmount
        {
            get => _DiscountAmount; set
            {
                _DiscountAmount = value;
                RaisePropertyChanged("DiscountAmount");
            }
        }

        private long[] _ServiceDetailsID;
        [DataMemberAttribute]
        public long[] ServiceDetailsID
        {
            get => _ServiceDetailsID; set
            {
                _ServiceDetailsID = value;
                RaisePropertyChanged("ServiceDetailsID");
            }
        }
        #endregion

        private decimal _HealthInsuranceRebate;
        public decimal HealthInsuranceRebate
        {
            get => _HealthInsuranceRebate; set
            {
                _HealthInsuranceRebate = value;
                RaisePropertyChanged("HealthInsuranceRebate");
            }
        }

        private long? _BillingInvoiceID;
        public long? BillingInvoiceID
        {
            get
            {
                return _BillingInvoiceID;
            }
            set
            {
                _BillingInvoiceID = value;
                RaisePropertyChanged("BillingInvoiceID");
            }
        }
    }
}