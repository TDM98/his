using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{
    public partial class KeepTrackPriting : NotifyChangedBase
    {
        #region Factory Method


        /// Create a new KeepTrackPriting object.

        /// <param name="trackingItemID">Initial value of the TrackingItemID property.</param>
        /// <param name="invoiceID">Initial value of the InvoiceID property.</param>
        /// <param name="parsID">Initial value of the ParsID property.</param>
        public static KeepTrackPriting CreateKeepTrackPriting(long trackingItemID, String invoiceID, long parsID)
        {
            KeepTrackPriting keepTrackPriting = new KeepTrackPriting();
            keepTrackPriting.TrackingItemID = trackingItemID;
            keepTrackPriting.InvoiceID = invoiceID;
            keepTrackPriting.ParsID = parsID;
            return keepTrackPriting;
        }

        #endregion
        #region Primitive Properties





        [DataMemberAttribute()]
        public long TrackingItemID
        {
            get
            {
                return _TrackingItemID;
            }
            set
            {
                if (_TrackingItemID != value)
                {
                    OnTrackingItemIDChanging(value);
                    ////ReportPropertyChanging("TrackingItemID");
                    _TrackingItemID = value;
                    RaisePropertyChanged("TrackingItemID");
                    OnTrackingItemIDChanged();
                }
            }
        }
        private long _TrackingItemID;
        partial void OnTrackingItemIDChanging(long value);
        partial void OnTrackingItemIDChanged();





        [DataMemberAttribute()]
        public String InvoiceID
        {
            get
            {
                return _InvoiceID;
            }
            set
            {
                OnInvoiceIDChanging(value);
                ////ReportPropertyChanging("InvoiceID");
                _InvoiceID = value;
                RaisePropertyChanged("InvoiceID");
                OnInvoiceIDChanged();
            }
        }
        private String _InvoiceID;
        partial void OnInvoiceIDChanging(String value);
        partial void OnInvoiceIDChanged();





        [DataMemberAttribute()]
        public long ParsID
        {
            get
            {
                return _ParsID;
            }
            set
            {
                OnParsIDChanging(value);
                ////ReportPropertyChanging("ParsID");
                _ParsID = value;
                RaisePropertyChanged("ParsID");
                OnParsIDChanged();
            }
        }
        private long _ParsID;
        partial void OnParsIDChanging(long value);
        partial void OnParsIDChanged();





        [DataMemberAttribute()]
        public Nullable<Int16> Status
        {
            get
            {
                return _Status;
            }
            set
            {
                OnStatusChanging(value);
                ////ReportPropertyChanging("Status");
                _Status = value;
                RaisePropertyChanged("Status");
                OnStatusChanged();
            }
        }
        private Nullable<Int16> _Status;
        partial void OnStatusChanging(Nullable<Int16> value);
        partial void OnStatusChanged();





        [DataMemberAttribute()]
        public String Comment
        {
            get
            {
                return _Comment;
            }
            set
            {
                OnCommentChanging(value);
                ////ReportPropertyChanging("Comment");
                _Comment = value;
                RaisePropertyChanged("Comment");
                OnCommentChanged();
            }
        }
        private String _Comment;
        partial void OnCommentChanging(String value);
        partial void OnCommentChanged();

        #endregion

        #region Navigation Properties






        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_KEEPTRAC_REL_HOSFM_PATIENTI", "PatientInvoices")]
        public PatientInvoice PatientInvoice
        {
            get;
            set;
        }

        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_KEEPTRAC_REL_HOSFM_PRINTSTA", "PrintStationParameters")]
        public PrintStationParameter PrintStationParameter
        {
            get;
            set;
        }

        #endregion
    }
}
