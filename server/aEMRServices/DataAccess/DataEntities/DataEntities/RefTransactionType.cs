using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{
    public partial class RefTransactionType : NotifyChangedBase
    {
        #region Factory Method


        /// Create a new RefTransactionType object.

        /// <param name="transactionTypeID">Initial value of the TransactionTypeID property.</param>
        /// <param name="transactionTypeDes">Initial value of the TransactionTypeDes property.</param>
        public static RefTransactionType CreateRefTransactionType(long transactionTypeID, String transactionTypeDes)
        {
            RefTransactionType refTransactionType = new RefTransactionType();
            refTransactionType.TransactionTypeID = transactionTypeID;
            refTransactionType.TransactionTypeDes = transactionTypeDes;
            return refTransactionType;
        }

        #endregion
        #region Primitive Properties





        [DataMemberAttribute()]
        public long TransactionTypeID
        {
            get
            {
                return _TransactionTypeID;
            }
            set
            {
                if (_TransactionTypeID != value)
                {
                    OnTransactionTypeIDChanging(value);
                    ////ReportPropertyChanging("TransactionTypeID");
                    _TransactionTypeID = value;
                    RaisePropertyChanged("TransactionTypeID");
                    OnTransactionTypeIDChanged();
                }
            }
        }
        private long _TransactionTypeID;
        partial void OnTransactionTypeIDChanging(long value);
        partial void OnTransactionTypeIDChanged();





        [DataMemberAttribute()]
        public String TransactionTypeDes
        {
            get
            {
                return _TransactionTypeDes;
            }
            set
            {
                OnTransactionTypeDesChanging(value);
                ////ReportPropertyChanging("TransactionTypeDes");
                _TransactionTypeDes = value;
                RaisePropertyChanged("TransactionTypeDes");
                OnTransactionTypeDesChanged();
            }
        }
        private String _TransactionTypeDes;
        partial void OnTransactionTypeDesChanging(String value);
        partial void OnTransactionTypeDesChanged();

        #endregion

        #region Navigation Properties






        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_PATIENTT_REL_HOSFM_REFTRANS", "PatientTransaction")]
        public ObservableCollection<PatientTransaction> PatientTransactions
        {
            get;
            set;
        }

        #endregion
    }
}
