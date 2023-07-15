using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;
namespace DataEntities
{
    public partial class RequestDetail : NotifyChangedBase
    {
        #region Factory Method


        /// Create a new RequestDetail object.

        /// <param name="requestDetailID">Initial value of the RequestDetailID property.</param>
        /// <param name="qty">Initial value of the Qty property.</param>
        public static RequestDetail CreateRequestDetail(long requestDetailID, Byte qty)
        {
            RequestDetail requestDetail = new RequestDetail();
            requestDetail.RequestDetailID = requestDetailID;
            requestDetail.Qty = qty;
            return requestDetail;
        }

        #endregion
        #region Primitive Properties





        [DataMemberAttribute()]
        public long RequestDetailID
        {
            get
            {
                return _RequestDetailID;
            }
            set
            {
                if (_RequestDetailID != value)
                {
                    OnRequestDetailIDChanging(value);
                    ////ReportPropertyChanging("RequestDetailID");
                    _RequestDetailID = value;
                    RaisePropertyChanged("RequestDetailID");
                    OnRequestDetailIDChanged();
                }
            }
        }
        private long _RequestDetailID;
        partial void OnRequestDetailIDChanging(long value);
        partial void OnRequestDetailIDChanged();





        [DataMemberAttribute()]
        public Nullable<Int64> RequestID
        {
            get
            {
                return _RequestID;
            }
            set
            {
                OnRequestIDChanging(value);
                ////ReportPropertyChanging("RequestID");
                _RequestID = value;
                RaisePropertyChanged("RequestID");
                OnRequestIDChanged();
            }
        }
        private Nullable<Int64> _RequestID;
        partial void OnRequestIDChanging(Nullable<Int64> value);
        partial void OnRequestIDChanged();





        [DataMemberAttribute()]
        public Nullable<long> DrugID
        {
            get
            {
                return _DrugID;
            }
            set
            {
                OnDrugIDChanging(value);
                ////ReportPropertyChanging("DrugID");
                _DrugID = value;
                RaisePropertyChanged("DrugID");
                OnDrugIDChanged();
            }
        }
        private Nullable<long> _DrugID;
        partial void OnDrugIDChanging(Nullable<long> value);
        partial void OnDrugIDChanged();





        [DataMemberAttribute()]
        public Byte Qty
        {
            get
            {
                return _Qty;
            }
            set
            {
                OnQtyChanging(value);
                ////ReportPropertyChanging("Qty");
                _Qty = value;
                RaisePropertyChanged("Qty");
                OnQtyChanged();
            }
        }
        private Byte _Qty;
        partial void OnQtyChanging(Byte value);
        partial void OnQtyChanged();





        [DataMemberAttribute()]
        public String DetailNotes
        {
            get
            {
                return _DetailNotes;
            }
            set
            {
                OnDetailNotesChanging(value);
                ////ReportPropertyChanging("DetailNotes");
                _DetailNotes = value;
                RaisePropertyChanged("DetailNotes");
                OnDetailNotesChanged();
            }
        }
        private String _DetailNotes;
        partial void OnDetailNotesChanging(String value);
        partial void OnDetailNotesChanged();

        #endregion

        #region Navigation Properties






        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_REQUESTD_REL_DMGMT_REFGENER", "RefGenericDrugDetails")]
        public RefGenericDrugDetail RefGenericDrugDetail
        {
            get;
            set;
        }


        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_REQUESTD_REL_DMGMT_REQUESTS", "Requests")]
        public Request Request
        {
            get;
            set;
        }

        #endregion
    }
}
