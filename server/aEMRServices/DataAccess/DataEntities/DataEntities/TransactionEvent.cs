using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;
namespace DataEntities
{
    public partial class TransactionEvent : NotifyChangedBase
    {
        #region Factory Method


        /// Create a new TransactionEvent object.

        /// <param name="transEventID">Initial value of the TransEventID property.</param>
        public static TransactionEvent CreateTransactionEvent(String transEventID)
        {
            TransactionEvent transactionEvent = new TransactionEvent();
            transactionEvent.TransEventID = transEventID;
            return transactionEvent;
        }

        #endregion
        #region Primitive Properties





        [DataMemberAttribute()]
        public String TransEventID
        {
            get
            {
                return _TransEventID;
            }
            set
            {
                if (_TransEventID != value)
                {
                    OnTransEventIDChanging(value);
                    ////ReportPropertyChanging("TransEventID");
                    _TransEventID = value;
                    RaisePropertyChanged("TransEventID");
                    OnTransEventIDChanged();
                }
            }
        }
        private String _TransEventID;
        partial void OnTransEventIDChanging(String value);
        partial void OnTransEventIDChanged();

        #endregion

        #region Navigation Properties






        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_BLOODDON_REL_BB02_TRANSACT", "BloodDonations")]
        public ObservableCollection<BloodDonation> BloodDonations
        {
            get;
            set;
        }

        #endregion
    }
}
