using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;
namespace DataEntities
{
    public partial class Request : NotifyChangedBase
    {
        #region Factory Method


        /// Create a new Request object.

        /// <param name="requestID">Initial value of the RequestID property.</param>
        /// <param name="requestDateTime">Initial value of the RequestDateTime property.</param>
        public static Request CreateRequest(Int64 requestID, DateTime requestDateTime)
        {
            Request request = new Request();
            request.RequestID = requestID;
            request.RequestDateTime = requestDateTime;
            return request;
        }

        #endregion
        #region Primitive Properties





        [DataMemberAttribute()]
        public Int64 RequestID
        {
            get
            {
                return _RequestID;
            }
            set
            {
                if (_RequestID != value)
                {
                    OnRequestIDChanging(value);
                    ////ReportPropertyChanging("RequestID");
                    _RequestID = value;
                    RaisePropertyChanged("RequestID");
                    OnRequestIDChanged();
                }
            }
        }
        private Int64 _RequestID;
        partial void OnRequestIDChanging(Int64 value);
        partial void OnRequestIDChanged();





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
                ////ReportPropertyChanging("StaffID");
                _StaffID = value;
                RaisePropertyChanged("StaffID");
                OnStaffIDChanged();
            }
        }
        private Nullable<Int64> _StaffID;
        partial void OnStaffIDChanging(Nullable<Int64> value);
        partial void OnStaffIDChanged();





        [DataMemberAttribute()]
        public Nullable<long> StoreID
        {
            get
            {
                return _StoreID;
            }
            set
            {
                OnStoreIDChanging(value);
                ////ReportPropertyChanging("StoreID");
                _StoreID = value;
                RaisePropertyChanged("StoreID");
                OnStoreIDChanged();
            }
        }
        private Nullable<long> _StoreID;
        partial void OnStoreIDChanging(Nullable<long> value);
        partial void OnStoreIDChanged();





        [DataMemberAttribute()]
        public Nullable<long> Ref_StoreID
        {
            get
            {
                return _Ref_StoreID;
            }
            set
            {
                OnRef_StoreIDChanging(value);
                ////ReportPropertyChanging("Ref_StoreID");
                _Ref_StoreID = value;
                RaisePropertyChanged("Ref_StoreID");
                OnRef_StoreIDChanged();
            }
        }
        private Nullable<long> _Ref_StoreID;
        partial void OnRef_StoreIDChanging(Nullable<long> value);
        partial void OnRef_StoreIDChanged();





        [DataMemberAttribute()]
        public DateTime RequestDateTime
        {
            get
            {
                return _RequestDateTime;
            }
            set
            {
                OnRequestDateTimeChanging(value);
                ////ReportPropertyChanging("RequestDateTime");
                _RequestDateTime = value;
                RaisePropertyChanged("RequestDateTime");
                OnRequestDateTimeChanged();
            }
        }
        private DateTime _RequestDateTime;
        partial void OnRequestDateTimeChanging(DateTime value);
        partial void OnRequestDateTimeChanged();





        [DataMemberAttribute()]
        public String RequestNotes
        {
            get
            {
                return _RequestNotes;
            }
            set
            {
                OnRequestNotesChanging(value);
                ////ReportPropertyChanging("RequestNotes");
                _RequestNotes = value;
                RaisePropertyChanged("RequestNotes");
                OnRequestNotesChanged();
            }
        }
        private String _RequestNotes;
        partial void OnRequestNotesChanging(String value);
        partial void OnRequestNotesChanged();

        #endregion

        #region Navigation Properties






        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_REQ_REL_DMGMT_REFSTORL", "RefStorageWarehouseLocation")]
        public RefStorageWarehouseLocation RefStorageWarehouseLocation
        {
            get;
            set;
        }

        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_REQ_REL_DMGMT_REFSTORW", "RefStorageWarehouseLocation")]
        public RefStorageWarehouseLocation RefStorageWarehouseLocation1
        {
            get;
            set;
        }



        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_REQUESTD_REL_DMGMT_REQUESTS", "RequestDetails")]
        public ObservableCollection<RequestDetail> RequestDetails
        {
            get;
            set;
        }






        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_REQUESTS_REL_DMGMT_STAFFS", "Staffs")]
        public Staff Staff
        {
            get;
            set;
        }

        #endregion
    }
}
