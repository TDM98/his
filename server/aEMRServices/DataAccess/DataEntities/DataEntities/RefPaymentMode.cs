using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{
    public partial class RefPaymentMode : NotifyChangedBase
    {
        #region Factory Method


        /// Create a new RefPaymentMode object.

        /// <param name="payModeID">Initial value of the PayModeID property.</param>
        /// <param name="payModeName">Initial value of the PayModeName property.</param>
        public static RefPaymentMode CreateRefPaymentMode(long payModeID, String payModeName)
        {
            RefPaymentMode refPaymentMode = new RefPaymentMode();
            refPaymentMode.PayModeID = payModeID;
            refPaymentMode.PayModeName = payModeName;
            return refPaymentMode;
        }

        #endregion
        #region Primitive Properties

        [DataMemberAttribute()]
        public long PayModeID
        {
            get
            {
                return _PayModeID;
            }
            set
            {
                if (_PayModeID != value)
                {
                    OnPayModeIDChanging(value);
                    ////ReportPropertyChanging("PayModeID");
                    _PayModeID = value;
                    RaisePropertyChanged("PayModeID");
                    OnPayModeIDChanged();
                }
            }
        }
        private long _PayModeID;
        partial void OnPayModeIDChanging(long value);
        partial void OnPayModeIDChanged();





        [DataMemberAttribute()]
        public String PayModeName
        {
            get
            {
                return _PayModeName;
            }
            set
            {
                OnPayModeNameChanging(value);
                ////ReportPropertyChanging("PayModeName");
                _PayModeName = value;
                RaisePropertyChanged("PayModeName");
                OnPayModeNameChanged();
            }
        }
        private String _PayModeName;
        partial void OnPayModeNameChanging(String value);
        partial void OnPayModeNameChanged();

        #endregion

        #region Navigation Properties

        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_PATIENTI_REL_HOSFM_REFPAYME", "PatientInvoices")]
        public ObservableCollection<PatientInvoice> PatientInvoices
        {
            get;
            set;
        }
        #endregion
    }
}
