using System;
using System.Runtime.Serialization;
using eHCMS.Services.Core.Base;

namespace DataEntities
{
    public partial class PatientTransactionDetail : NotifyChangedBase
    {
        public PatientTransactionDetail()
        {
            V_TranRefType = AllLookupValues.V_TranRefType.NONE;
        }
        #region Factory Method
        /// Create a new PatientTransactionDetail object.
        /// <param name="transItemID">Initial value of the TransItemID property.</param>
        /// <param name="amount">Initial value of the Amount property.</param>
        public static PatientTransactionDetail CreatePatientTransactionDetail(long transItemID, Decimal amount)
        {
            PatientTransactionDetail patientTransactionDetail = new PatientTransactionDetail();
            patientTransactionDetail.TransItemID = transItemID;
            patientTransactionDetail.Amount = amount;
            return patientTransactionDetail;
        }

        #endregion

        #region Primitive Properties

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

        [DataMemberAttribute()]
        public Nullable<long> OutwBloodInvoiceID
        {
            get
            {
                return _OutwBloodInvoiceID;
            }
            set
            {
                OnOutwBloodInvoiceIDChanging(value);
                _OutwBloodInvoiceID = value;
                RaisePropertyChanged("OutwBloodInvoiceID");
                OnOutwBloodInvoiceIDChanged();
            }
        }
        private Nullable<long> _OutwBloodInvoiceID;
        partial void OnOutwBloodInvoiceIDChanging(Nullable<long> value);
        partial void OnOutwBloodInvoiceIDChanged();

        [DataMemberAttribute()]
        public Nullable<long> OutDMedRscrID
        {
            get
            {
                return _OutDMedRscrID;
            }
            set
            {
                OnOutDMedRscrIDChanging(value);
                _OutDMedRscrID = value;
                RaisePropertyChanged("OutDMedRscrID");
                OnOutDMedRscrIDChanged();
            }
        }
        private Nullable<long> _OutDMedRscrID;
        partial void OnOutDMedRscrIDChanging(Nullable<long> value);
        partial void OnOutDMedRscrIDChanged();

        [DataMemberAttribute()]
        public Nullable<Int64> StaffID
        {
            get
            {
                return _StaffID;
            }
            set
            {
                OnStaffIDChanging(value);
                _StaffID = value;
                RaisePropertyChanged("StaffID");
                OnStaffIDChanged();
            }
        }
        private Nullable<Int64> _StaffID;
        partial void OnStaffIDChanging(Nullable<Int64> value);
        partial void OnStaffIDChanged();

        [DataMemberAttribute()]
        public Nullable<Int64> PtRegDetailID
        {
            get
            {
                return _PtRegDetailID;
            }
            set
            {
                OnPtRegDetailIDChanging(value);
                _PtRegDetailID = value;
                RaisePropertyChanged("PtRegDetailID");
                OnPtRegDetailIDChanged();
            }
        }
        private Nullable<Int64> _PtRegDetailID;
        partial void OnPtRegDetailIDChanging(Nullable<Int64> value);
        partial void OnPtRegDetailIDChanged();

        [DataMemberAttribute()]
        public Nullable<long> outiID
        {
            get
            {
                return _outiID;
            }
            set
            {
                OnoutiIDChanging(value);
                _outiID = value;
                RaisePropertyChanged("outiID");
                OnoutiIDChanged();
            }
        }
        private Nullable<long> _outiID;
        partial void OnoutiIDChanging(Nullable<long> value);
        partial void OnoutiIDChanged();

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
                _TransactionID = value;
                RaisePropertyChanged("TransactionID");
                OnTransactionIDChanged();
            }
        }
        private Nullable<long> _TransactionID;
        partial void OnTransactionIDChanging(Nullable<long> value);
        partial void OnTransactionIDChanged();

        [DataMemberAttribute()]
        public Nullable<DateTime> TransactionDate
        {
            get
            {
                return _TransactionDate;
            }
            set
            {
                OnTransactionDateChanging(value);
                _TransactionDate = value;
                RaisePropertyChanged("TransactionDate");
                OnTransactionDateChanged();
            }
        }
        private Nullable<DateTime> _TransactionDate;
        partial void OnTransactionDateChanging(Nullable<DateTime> value);
        partial void OnTransactionDateChanged();

        [DataMemberAttribute()]
        public Decimal Amount
        {
            get
            {
                return _Amount;
            }
            set
            {
                OnAmountChanging(value);
                _Amount = value;
                RaisePropertyChanged("Amount");
                OnAmountChanged();
            }
        }
        private Decimal _Amount;
        partial void OnAmountChanging(Decimal value);
        partial void OnAmountChanged();

        [DataMemberAttribute()]
        public string FullName
        {
            get
            {
                return _FullName;
            }
            set
            {
                OnFullNameChanging(value);
                _FullName = value;
                RaisePropertyChanged("FullName");
                OnFullNameChanged();
            }
        }
        private string _FullName;
        partial void OnFullNameChanging(string value);
        partial void OnFullNameChanged();

        [DataMemberAttribute()]
        public string TransactionType
        {
            get
            {
                return _TransactionType;
            }
            set
            {
                OnTransactionTypeChanging(value);
                _TransactionType = value;
                RaisePropertyChanged("TransactionType");
                OnTransactionTypeChanged();
            }
        }
        private string _TransactionType;
        partial void OnTransactionTypeChanging(string value);
        partial void OnTransactionTypeChanged();

        [DataMemberAttribute()]
        public Nullable<Decimal> PriceDifference
        {
            get
            {
                return _PriceDifference;
            }
            set
            {
                OnPriceDifferenceChanging(value);
                _PriceDifference = value;
                RaisePropertyChanged("PriceDifference");
                OnPriceDifferenceChanged();
            }
        }
        private Nullable<Decimal> _PriceDifference;
        partial void OnPriceDifferenceChanging(Nullable<Decimal> value);
        partial void OnPriceDifferenceChanged();

        [DataMemberAttribute()]
        public Nullable<Decimal> AmountCoPay
        {
            get
            {
                return _AmountCoPay;
            }
            set
            {
                OnAmountCoPayChanging(value);
                _AmountCoPay = value;
                RaisePropertyChanged("AmountCoPay");
                OnAmountCoPayChanged();
            }
        }
        private Nullable<Decimal> _AmountCoPay;
        partial void OnAmountCoPayChanging(Nullable<Decimal> value);
        partial void OnAmountCoPayChanged();

        [DataMemberAttribute()]
        public Nullable<Decimal> HealthInsuranceRebate
        {
            get
            {
                return _HealthInsuranceRebate;
            }
            set
            {
                OnHealthInsuranceRebateChanging(value);
                _HealthInsuranceRebate = value;
                RaisePropertyChanged("HealthInsuranceRebate");
                OnHealthInsuranceRebateChanged();
            }
        }
        private Nullable<Decimal> _HealthInsuranceRebate;
        partial void OnHealthInsuranceRebateChanging(Nullable<Decimal> value);
        partial void OnHealthInsuranceRebateChanged();

        [DataMemberAttribute()]
        public Nullable<Double> Discount
        {
            get
            {
                return _Discount;
            }
            set
            {
                OnDiscountChanging(value);
                _Discount = value;
                RaisePropertyChanged("Discount");
                OnDiscountChanged();
            }
        }
        private Nullable<Double> _Discount;
        partial void OnDiscountChanging(Nullable<Double> value);
        partial void OnDiscountChanged();

        [DataMemberAttribute()]
        public Nullable<double> Qty
        {
            get
            {
                return _Qty;
            }
            set
            {
                _Qty = value;
                RaisePropertyChanged("Qty");
            }
        }
        private Nullable<double> _Qty;

        [DataMemberAttribute()]
        public Nullable<Int64> RefDocID
        {
            get
            {
                return _RefDocID;
            }
            set
            {
                OnRefDocIDChanging(value);
                _RefDocID = value;
                RaisePropertyChanged("RefDocID");
                OnRefDocIDChanged();
            }
        }
        private Nullable<Int64> _RefDocID;
        partial void OnRefDocIDChanging(Nullable<Int64> value);
        partial void OnRefDocIDChanged();

        [DataMemberAttribute()]
        public Nullable<Double> ExchangeRate
        {
            get
            {
                return _ExchangeRate;
            }
            set
            {
                OnExchangeRateChanging(value);
                _ExchangeRate = value;
                RaisePropertyChanged("ExchangeRate");
                OnExchangeRateChanged();
            }
        }
        private Nullable<Double> _ExchangeRate;
        partial void OnExchangeRateChanging(Nullable<Double> value);
        partial void OnExchangeRateChanged();

        [DataMemberAttribute()]
        public String TransItemRemarks
        {
            get
            {
                return _TransItemRemarks;
            }
            set
            {
                OnTransItemRemarksChanging(value);
                _TransItemRemarks = value;
                RaisePropertyChanged("TransItemRemarks");
                OnTransItemRemarksChanged();
            }
        }
        private String _TransItemRemarks;
        partial void OnTransItemRemarksChanging(String value);
        partial void OnTransItemRemarksChanged();

        #endregion

        #region Navigation Properties

        [DataMemberAttribute()]
        public OutwardDMedRscrInvoice OutwardDMedRscrInvoice
        {
            get;
            set;
        }

        [DataMemberAttribute()]
        public OutwardDrugInvoice OutwardDrugInvoice
        {
            get;
            set;
        }


        [DataMemberAttribute()]
        public OutwardInvoiceBlood OutwardInvoiceBlood
        {
            get;
            set;
        }

        [DataMemberAttribute()]
        public PatientRegistrationDetail PatientRegistrationDetail
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
        public Staff Staff
        {
            get;
            set;
        }

        #endregion

        //private RecordState _RecordState = RecordState.DETACHED;
        //[DataMemberAttribute()]
        //public RecordState RecordState
        //{
        //    get
        //    {
        //        return _RecordState;
        //    }
        //    set
        //    {
        //        _RecordState = value;
        //        RaisePropertyChanged("RecordState");
        //    }
        //}

        private long? _PCLRequestID;
        [DataMemberAttribute()]
        public long? PCLRequestID
        {
            get
            {
                return _PCLRequestID;
            }
            set
            {
                if (_PCLRequestID != value)
                {
                    _PCLRequestID = value;
                    RaisePropertyChanged("PCLRequestID");
                }
            }
        }

        public long? _TranRefID;
        [DataMemberAttribute()]
        public long? TranRefID
        {
            get
            {
                return _TranRefID;
            }
            set
            {
                _TranRefID = value;
                RaisePropertyChanged("TranRefID");
            }
        }
        private AllLookupValues.V_TranRefType _V_TranRefType;
        [DataMemberAttribute()]
        public AllLookupValues.V_TranRefType V_TranRefType
        {
            get
            {
                return _V_TranRefType;
            }
            set
            {
                _V_TranRefType = value;
                RaisePropertyChanged("V_TranRefType");
            }
        }
        private bool _IsPaided;
        [DataMemberAttribute()]
        public bool IsPaided
        {
            get
            {
                return _IsPaided;
            }
            set
            {
                if (_IsPaided != value)
                {
                    _IsPaided = value;
                    RaisePropertyChanged("IsPaided");
                }
            }
        }


        public decimal PatientPayment
        {
            get { return Amount - HealthInsuranceRebate.GetValueOrDefault(0); }
        }

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

        private decimal? _DiscountAmt;
        [DataMemberAttribute]
        public decimal? DiscountAmt
        {
            get => _DiscountAmt; set
            {
                _DiscountAmt = value;
                RaisePropertyChanged("DiscountAmt");
            }
        }

        private long? _InPatientBillingInvID;
        [DataMemberAttribute]
        public long? InPatientBillingInvID
        {
            get
            {
                return _InPatientBillingInvID;
            }
            set
            {
                if (_InPatientBillingInvID != value)
                {
                    _InPatientBillingInvID = value;
                    RaisePropertyChanged("InPatientBillingInvID");
                }
            }
        }

        private decimal? _OtherAmt;
        [DataMemberAttribute]
        public decimal? OtherAmt
        {
            get => _OtherAmt; set
            {
                _OtherAmt = value;
                RaisePropertyChanged("OtherAmt");
            }
        }
    }
}