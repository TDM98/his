using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;
using System.Text;
using System.Collections.Generic;

namespace DataEntities
{
    [DataContract]
    public partial class PatientTransaction : NotifyChangedBase
    {
        #region Factory Method


        /// Create a new PatientTransaction object.

        /// <param name="transactionID">Initial value of the TransactionID property.</param>
        /// <param name="transactionBeginDate">Initial value of the TransactionBeginDate property.</param>
        public static PatientTransaction CreatePatientTransaction(long transactionID, DateTime transactionBeginDate)
        {
            PatientTransaction patientTransaction = new PatientTransaction();
            patientTransaction.TransactionID = transactionID;
            patientTransaction.TransactionBeginDate = transactionBeginDate;
            return patientTransaction;
        }

        #endregion
        #region Primitive Properties

        [DataMemberAttribute()]
        public long TransactionID
        {
            get
            {
                return _TransactionID;
            }
            set
            {
                if (_TransactionID != value)
                {
                    OnTransactionIDChanging(value);
                    _TransactionID = value;
                    RaisePropertyChanged("TransactionID");
                    OnTransactionIDChanged();
                }
            }
        }
        private long _TransactionID;
        partial void OnTransactionIDChanging(long value);
        partial void OnTransactionIDChanged();

        [DataMemberAttribute()]
        public Nullable<Int64> PtRegistrationID
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
        private Nullable<Int64> _PtRegistrationID;
        partial void OnPtRegistrationIDChanging(Nullable<Int64> value);
        partial void OnPtRegistrationIDChanged();

        [DataMemberAttribute()]
        public Nullable<long> TransactionTypeID
        {
            get
            {
                return _TransactionTypeID;
            }
            set
            {
                OnTransactionTypeIDChanging(value);
                _TransactionTypeID = value;
                RaisePropertyChanged("TransactionTypeID");
                OnTransactionTypeIDChanged();
            }
        }
        private Nullable<long> _TransactionTypeID;
        partial void OnTransactionTypeIDChanging(Nullable<long> value);
        partial void OnTransactionTypeIDChanged();

        [DataMemberAttribute()]
        public Nullable<long> BDID
        {
            get
            {
                return _BDID;
            }
            set
            {
                OnBDIDChanging(value);
                _BDID = value;
                RaisePropertyChanged("BDID");
                OnBDIDChanged();
            }
        }
        private Nullable<long> _BDID;
        partial void OnBDIDChanging(Nullable<long> value);
        partial void OnBDIDChanged();

        [DataMemberAttribute()]
        public DateTime TransactionBeginDate
        {
            get
            {
                return _TransactionBeginDate;
            }
            set
            {
                OnTransactionBeginDateChanging(value);
                _TransactionBeginDate = value;
                RaisePropertyChanged("TransactionBeginDate");
                OnTransactionBeginDateChanged();
            }
        }
        private DateTime _TransactionBeginDate;
        partial void OnTransactionBeginDateChanging(DateTime value);
        partial void OnTransactionBeginDateChanged();

        [DataMemberAttribute()]
        public Nullable<DateTime> TransactionEndDate
        {
            get
            {
                return _TransactionEndDate;
            }
            set
            {
                OnTransactionEndDateChanging(value);
                _TransactionEndDate = value;
                RaisePropertyChanged("TransactionEndDate");
                OnTransactionEndDateChanged();
            }
        }
        private Nullable<DateTime> _TransactionEndDate;
        partial void OnTransactionEndDateChanging(Nullable<DateTime> value);
        partial void OnTransactionEndDateChanged();

        [DataMemberAttribute()]
        public String TransactionRemarks
        {
            get
            {
                return _TransactionRemarks;
            }
            set
            {
                OnTransactionRemarksChanging(value);
                _TransactionRemarks = value;
                RaisePropertyChanged("TransactionRemarks");
                OnTransactionRemarksChanged();
            }
        }
        private String _TransactionRemarks;
        partial void OnTransactionRemarksChanging(String value);
        partial void OnTransactionRemarksChanged();

        [DataMemberAttribute()]
        public Nullable<Boolean> IsBalanced
        {
            get
            {
                return _IsBalanced;
            }
            set
            {
                OnIsBalancedChanging(value);
                _IsBalanced = value;
                RaisePropertyChanged("IsBalanced");
                OnIsBalancedChanged();
            }
        }
        private Nullable<Boolean> _IsBalanced;
        partial void OnIsBalancedChanging(Nullable<Boolean> value);
        partial void OnIsBalancedChanged();

        [DataMemberAttribute()]
        public Nullable<Boolean> IsConsolidated
        {
            get
            {
                return _IsConsolidated;
            }
            set
            {
                OnIsConsolidatedChanging(value);
                _IsConsolidated = value;
                RaisePropertyChanged("IsConsolidated");
                OnIsConsolidatedChanged();
            }
        }
        private Nullable<Boolean> _IsConsolidated;
        partial void OnIsConsolidatedChanging(Nullable<Boolean> value);
        partial void OnIsConsolidatedChanged();

        [DataMemberAttribute()]
        public Nullable<Boolean> IsAdjusted
        {
            get
            {
                return _IsAdjusted;
            }
            set
            {
                _IsAdjusted = value;
                RaisePropertyChanged("IsAdjusted");
            }
        }
        private Nullable<Boolean> _IsAdjusted;

        private Nullable<Boolean> _IsClosed;
        [DataMemberAttribute()]
        public Nullable<Boolean> IsClosed
        {
            get
            {
                return _IsClosed;
            }
            set
            {
                _IsClosed = value;
                RaisePropertyChanged("IsClosed");
            }
        }

        private long _V_TranHIPayment;
        [DataMemberAttribute()]
        public long V_TranHIPayment
        {
            get
            {
                return _V_TranHIPayment;
            }
            set
            {
                _V_TranHIPayment = value;
                RaisePropertyChanged("V_TranHIPayment");
            }
        }

        private AllLookupValues.TranHIPayment _TranHIPaymentStatus = AllLookupValues.TranHIPayment.OPENED;

        [DataMemberAttribute()]
        public AllLookupValues.TranHIPayment TranHIPaymentStatus
        {
            get
            {
                return _TranHIPaymentStatus;
            }
            set
            {
                _TranHIPaymentStatus = value;
                RaisePropertyChanged("TranHIPaymentStatus");
            }
        }


        private long _V_TranPatientPayment;
        [DataMemberAttribute()]
        public long V_TranPatientPayment
        {
            get
            {
                return _V_TranPatientPayment;
            }
            set
            {
                _V_TranPatientPayment = value;
                RaisePropertyChanged("V_TranPatientPayment");
            }
        }

        private AllLookupValues.TranPatientPayment _TranPatientPaymentStatus = AllLookupValues.TranPatientPayment.OPENED;
        [DataMemberAttribute()]
        public AllLookupValues.TranPatientPayment TranPatientPaymentStatus
        {
            get
            {
                return _TranPatientPaymentStatus;
            }
            set
            {
                _TranPatientPaymentStatus = value;
                RaisePropertyChanged("TranPatientPaymentStatus");
            }
        }

        #endregion

        #region Navigation Properties
        [DataMemberAttribute()]
        public BloodDonation BloodDonation
        {
            get;
            set;
        }

        [DataMemberAttribute()]
        public ObservableCollection<HIPaymentTransaction> HIPaymentTransactions
        {
            get;
            set;
        }

        [DataMemberAttribute()]
        public ObservableCollection<HIPremiumPaymentDetail> HIPremiumPaymentDetails
        {
            get;
            set;
        }

        [DataMemberAttribute()]
        public ObservableCollection<PatientInvoice> PatientInvoices
        {
            get;
            set;
        }

        
        //private ObservableCollection<PatientPayment> _PatientPayments;
        //[DataMemberAttribute()]
        //public ObservableCollection<PatientPayment> PatientPayments
        //{
        //    get
        //    {
        //        return _PatientPayments;
        //    }
        //    set
        //    { 
        //        _PatientPayments = value;
        //        RaisePropertyChanged("PatientPayments");
        //    }
        //}

        private ObservableCollection<PatientTransactionPayment> _PatientTransactionPayments;
        [DataMemberAttribute()]
        public ObservableCollection<PatientTransactionPayment> PatientTransactionPayments
        {
            get
            {
                return _PatientTransactionPayments;
            }
            set
            {
                _PatientTransactionPayments = value;
                RaisePropertyChanged("PatientTransactionPayments");
            }
        }

        [DataMemberAttribute()]
        public PatientRegistration PatientRegistration
        {
            get;
            set;
        }

        [DataMemberAttribute()]
        public ObservableCollection<PatientTransactionDetail> PatientTransactionDetails
        {
            get;
            set;
        }

        [DataMemberAttribute()]
        public RefTransactionType RefTransactionType
        {
            get;
            set;
        }

        #endregion

        private PayableSum _PayableSum;
     
        /// Thông tin tổng số tiền phải trả cho giao dịch này.
     
        [DataMemberAttribute()]
        public PayableSum PayableSum
        {
            get
            {
                return _PayableSum;
            }
            set
            {
                _PayableSum = value;
                RaisePropertyChanged("PayableSum");
            }
        }

        private ObservableCollection<OutwardDrugViewItem> _OutWardDrugItems;
        [DataMemberAttribute()]
        public ObservableCollection<OutwardDrugViewItem> OutWardDrugItems
        {
            get
            {
                return _OutWardDrugItems;
            }
            set
            {
                _OutWardDrugItems = value;
                RaisePropertyChanged("OutWardDrugItems");
            }
        }

        private ObservableCollection<OutwardDrug> _DrugItems;
        [DataMemberAttribute()]
        public ObservableCollection<OutwardDrug> DrugItems
        {
            get
            {
                return _DrugItems;
            }
            set
            {
                _DrugItems = value;
                RaisePropertyChanged("DrugItems");
            }
        }
        private RecordState _RecordState = RecordState.DETACHED;
        [DataMemberAttribute()]
        public RecordState RecordState
        {
            get
            {
                return _RecordState;
            }
            set
            {
                _RecordState = value;
                RaisePropertyChanged("RecordState");
            }
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

        private ObservableCollection<OutPatientCashAdvance> _PatientCashAdvances;
        [DataMemberAttribute()]
        public ObservableCollection<OutPatientCashAdvance> PatientCashAdvances
        {
            get
            {
                return _PatientCashAdvances;
            }
            set
            {
                _PatientCashAdvances = value;
                RaisePropertyChanged("PatientCashAdvances");
            }
        }

        public string ConvertPatientTransactionInfoToXml()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<PatientTransactionInfo>");
            sb.AppendFormat("<PtRegistrationID>{0}</PtRegistrationID>", PtRegistrationID);
            sb.AppendFormat("<TransactionTypeID>{0}</TransactionTypeID>", TransactionTypeID);                        
            sb.AppendFormat("<BDID>{0}</BDID>", BDID);
            sb.AppendFormat("<TransactionRemarks>{0}</TransactionRemarks>", TransactionRemarks);
            sb.AppendFormat("<V_TranHIPayment>{0}</V_TranHIPayment>", V_TranHIPayment);
            sb.AppendFormat("<V_TranPatientPayment>{0}</V_TranPatientPayment>", V_TranPatientPayment);
            sb.AppendFormat("<V_RegistrationType>{0}</V_RegistrationType>", (long)V_RegistrationType);
            sb.Append("</PatientTransactionInfo>");
            return sb.ToString();
        }
    }
}