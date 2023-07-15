using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{
    public partial class HIPaymentTransaction : NotifyChangedBase, IEditableObject
    {
        public HIPaymentTransaction()
            : base()
        {

        }

        private HIPaymentTransaction _tempHIPaymentTransaction;
        #region IEditableObject Members

        public void BeginEdit()
        {
            _tempHIPaymentTransaction = (HIPaymentTransaction)this.MemberwiseClone();
        }

        public void CancelEdit()
        {
            if (null != _tempHIPaymentTransaction)
                CopyFrom(_tempHIPaymentTransaction);
            //_tempPatient = null;
        }

        public void EndEdit()
        {
        }

        public void CopyFrom(HIPaymentTransaction p)
        {
            PropertyCopierHelper.CopyPropertiesTo(p, this);
        }

        #endregion
        #region Factory Method


        /// Create a new HIPaymentTransaction object.

        /// <param name="hIPtTransID">Initial value of the HIPtTransID property.</param>
        /// <param name="transactionID">Initial value of the TransactionID property.</param>
        /// <param name="hIPPmtID">Initial value of the HIPPmtID property.</param>
        public static HIPaymentTransaction CreateHIPaymentTransaction(long hIPtTransID, long transactionID, long hIPPmtID)
        {
            HIPaymentTransaction hIPaymentTransaction = new HIPaymentTransaction();
            hIPaymentTransaction.HIPtTransID = hIPtTransID;
            hIPaymentTransaction.TransactionID = transactionID;
            hIPaymentTransaction.HIPPmtID = hIPPmtID;
            return hIPaymentTransaction;
        }

        #endregion
        #region Primitive Properties





        [DataMemberAttribute()]
        public long HIPtTransID
        {
            get
            {
                return _HIPtTransID;
            }
            set
            {
                if (_HIPtTransID != value)
                {
                    OnHIPtTransIDChanging(value);
                    ////ReportPropertyChanging("HIPtTransID");
                    _HIPtTransID = value;
                    RaisePropertyChanged("HIPtTransID");
                    OnHIPtTransIDChanged();
                }
            }
        }
        private long _HIPtTransID;
        partial void OnHIPtTransIDChanging(long value);
        partial void OnHIPtTransIDChanged();





        [DataMemberAttribute()]
        public long TransactionID
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
        private long _TransactionID;
        partial void OnTransactionIDChanging(long value);
        partial void OnTransactionIDChanged();





        [DataMemberAttribute()]
        public long HIPPmtID
        {
            get
            {
                return _HIPPmtID;
            }
            set
            {
                OnHIPPmtIDChanging(value);
                ////ReportPropertyChanging("HIPPmtID");
                _HIPPmtID = value;
                RaisePropertyChanged("HIPPmtID");
                OnHIPPmtIDChanged();
            }
        }
        private long _HIPPmtID;
        partial void OnHIPPmtIDChanging(long value);
        partial void OnHIPPmtIDChanged();





        [DataMemberAttribute()]
        public Nullable<Double> Percentage_Paid
        {
            get
            {
                return _Percentage_Paid;
            }
            set
            {
                OnPercentage_PaidChanging(value);
                ////ReportPropertyChanging("Percentage_Paid");
                _Percentage_Paid = value;
                RaisePropertyChanged("Percentage_Paid");
                OnPercentage_PaidChanged();
            }
        }
        private Nullable<Double> _Percentage_Paid;
        partial void OnPercentage_PaidChanging(Nullable<Double> value);
        partial void OnPercentage_PaidChanged();

        #endregion

        #region Navigation Properties






        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_HIPAYMEN_REL_HOSFM_HIPREMIU", "HIPremiumPayments")]
        public HIPremiumPayment HIPremiumPayment
        {
            get;
            set;
        }

        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_HIPAYMEN_REL_HOSFM_PATIENTT", "PatientTransaction")]
        public PatientTransaction PatientTransaction
        {
            get;
            set;
        }

        #endregion
    }
}
