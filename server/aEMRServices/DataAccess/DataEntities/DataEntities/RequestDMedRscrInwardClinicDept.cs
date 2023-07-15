using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Text;
namespace DataEntities
{
    public partial class RequestDMedRscrInwardClinicDept : NotifyChangedBase
    {
        #region Factory Method

       
        /// Create a new RequestDMedRscrInwardClinicDept object.
     
        /// <param name="reqDMedRscrInClinicDeptID">Initial value of the ReqDMedRscrInClinicDeptID property.</param>
        /// <param name="staffID">Initial value of the StaffID property.</param>
        /// <param name="reqDate">Initial value of the ReqDate property.</param>
        public static RequestDMedRscrInwardClinicDept CreateRequestDMedRscrInwardClinicDept(Int64 reqDMedRscrInClinicDeptID, Int64 staffID, DateTime reqDate)
        {
            RequestDMedRscrInwardClinicDept requestDMedRscrInwardClinicDept = new RequestDMedRscrInwardClinicDept();
            requestDMedRscrInwardClinicDept.ReqDMedRscrInClinicDeptID = reqDMedRscrInClinicDeptID;
            requestDMedRscrInwardClinicDept.StaffID = staffID;
            requestDMedRscrInwardClinicDept.ReqDate = reqDate;
            return requestDMedRscrInwardClinicDept;
        }

        #endregion
        #region Primitive Properties

        [DataMemberAttribute()]
        public Int64 ReqDMedRscrInClinicDeptID
        {
            get
            {
                return _ReqDMedRscrInClinicDeptID;
            }
            set
            {
                if (_ReqDMedRscrInClinicDeptID != value)
                {
                    OnReqDMedRscrInClinicDeptIDChanging(value);
                    //ReportPropertyChanging("ReqDMedRscrInClinicDeptID");
                    _ReqDMedRscrInClinicDeptID = value;
                    RaisePropertyChanged("ReqDMedRscrInClinicDeptID");
                    OnReqDMedRscrInClinicDeptIDChanged();
                }
            }
        }
        private Int64 _ReqDMedRscrInClinicDeptID;
        partial void OnReqDMedRscrInClinicDeptIDChanging(Int64 value);
        partial void OnReqDMedRscrInClinicDeptIDChanged();

        [DataMemberAttribute()]
        public Int64 StaffID
        {
            get
            {
                return _StaffID;
            }
            set
            {
                OnStaffIDChanging(value);
                //ReportPropertyChanging("StaffID");
                _StaffID = value;
                RaisePropertyChanged("StaffID");
                OnStaffIDChanged();
            }
        }
        private Int64 _StaffID;
        partial void OnStaffIDChanging(Int64 value);
        partial void OnStaffIDChanged();

        [DataMemberAttribute()]
        public Nullable<Int64> DeptID
        {
            get
            {
                return _DeptID;
            }
            set
            {
                OnDeptIDChanging(value);
                //ReportPropertyChanging("DeptID");
                _DeptID = value;
                RaisePropertyChanged("DeptID");
                OnDeptIDChanged();
            }
        }
        private Nullable<Int64> _DeptID;
        partial void OnDeptIDChanging(Nullable<Int64> value);
        partial void OnDeptIDChanged();

        [DataMemberAttribute()]
        public DateTime ReqDate
        {
            get
            {
                return _ReqDate;
            }
            set
            {
                OnReqDateChanging(value);
                //ReportPropertyChanging("ReqDate");
                _ReqDate = value;
                RaisePropertyChanged("ReqDate");
                OnReqDateChanged();
            }
        }
        private DateTime _ReqDate;
        partial void OnReqDateChanging(DateTime value);
        partial void OnReqDateChanged();

        [DataMemberAttribute()]
        public String ReqNumCode
        {
            get
            {
                return _ReqNumCode;
            }
            set
            {
                OnReqNumCodeChanging(value);
                //ReportPropertyChanging("ReqNumCode");
                _ReqNumCode = value;
                RaisePropertyChanged("ReqNumCode");
                OnReqNumCodeChanged();
            }
        }
        private String _ReqNumCode;
        partial void OnReqNumCodeChanging(String value);
        partial void OnReqNumCodeChanged();

        [DataMemberAttribute()]
        public Nullable<Int64> OutFromStoreID
        {
            get
            {
                return _OutFromStoreID;
            }
            set
            {
                OnOutFromStoreIDChanging(value);
                //ReportPropertyChanging("OutFromStoreID");
                _OutFromStoreID = value;
                RaisePropertyChanged("OutFromStoreID");
                OnOutFromStoreIDChanged();
            }
        }
        private Nullable<Int64> _OutFromStoreID;
        partial void OnOutFromStoreIDChanging(Nullable<Int64> value);
        partial void OnOutFromStoreIDChanged();

        [DataMemberAttribute()]
        public Nullable<Int64> InDeptStoreID
        {
            get
            {
                return _InDeptStoreID;
            }
            set
            {
                OnInDeptStoreIDChanging(value);
                //ReportPropertyChanging("InDeptStoreID");
                _InDeptStoreID = value;
                RaisePropertyChanged("InDeptStoreID");
                OnInDeptStoreIDChanged();
            }
        }
        private Nullable<Int64> _InDeptStoreID;
        partial void OnInDeptStoreIDChanging(Nullable<Int64> value);
        partial void OnInDeptStoreIDChanged();

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
                //ReportPropertyChanging("Comment");
                _Comment = value;
                RaisePropertyChanged("Comment");
                OnCommentChanged();
            }
        }
        private String _Comment;
        partial void OnCommentChanging(String value);
        partial void OnCommentChanged();

        [DataMemberAttribute()]
        public Nullable<Int64> ReqStatus
        {
            get
            {
                return _ReqStatus;
            }
            set
            {
                OnReqStatusChanging(value);
                //ReportPropertyChanging("ReqStatus");
                _ReqStatus = value;
                RaisePropertyChanged("ReqStatus");
                OnReqStatusChanged();
            }
        }
        private Nullable<Int64> _ReqStatus;
        partial void OnReqStatusChanging(Nullable<Int64> value);
        partial void OnReqStatusChanged();

        [DataMemberAttribute()]
        public Nullable<Boolean> IsDeleted
        {
            get
            {
                return _IsDeleted;
            }
            set
            {
                OnIsDeletedChanging(value);
                //ReportPropertyChanging("IsDeleted");
                _IsDeleted = value;
                RaisePropertyChanged("IsDeleted");
                OnIsDeletedChanged();
            }
        }
        private Nullable<Boolean> _IsDeleted;
        partial void OnIsDeletedChanging(Nullable<Boolean> value);
        partial void OnIsDeletedChanged();

        #endregion

        #region Navigation Properties

        [DataMemberAttribute()]
        public ObservableCollection<RequestDMedRscrInwardClinicDeptDetail> RequestDetails
        {
            get
            {
                return _RequestDetails;
            }
            set
            {
                if (_RequestDetails != value)
                {
                    _RequestDetails = value;
                    RaisePropertyChanged("RequestDetails");
                }
            }
        }
        private ObservableCollection<RequestDMedRscrInwardClinicDeptDetail> _RequestDetails;

        [DataMemberAttribute()]
        public Staff SelectedStaff
        {
            get
            {
                return _SelectedStaff;
            }
            set
            {
                if (_SelectedStaff != value)
                {
                    _SelectedStaff = value;
                    if (_SelectedStaff != null)
                    {
                        _StaffID = _SelectedStaff.StaffID;
                    }
                    RaisePropertyChanged("SelectedStaff");
                }
            }
        }
        private Staff _SelectedStaff;

        [DataMemberAttribute()]
        public RefStorageWarehouseLocation OutFromStoreObject
        {
            get
            {
                return _OutFromStoreObject;
            }
            set
            {
                if (_OutFromStoreObject != value)
                {
                    _OutFromStoreObject = value;
                    if (_OutFromStoreObject != null)
                    {
                        _OutFromStoreID = _OutFromStoreObject.StoreID;
                    }
                    RaisePropertyChanged("OutFromStoreObject");
                }
            }
        }
        private RefStorageWarehouseLocation _OutFromStoreObject;
       

        [DataMemberAttribute()]
        public RefStorageWarehouseLocation InDeptStoreObject
        {
            get
            {
                return _InDeptStoreObject;
            }
            set
            {
                if (_InDeptStoreObject != value)
                {
                    _InDeptStoreObject = value;
                    if (_InDeptStoreObject != null)
                    {
                        _InDeptStoreID = _InDeptStoreObject.StoreID;
                    }
                    RaisePropertyChanged("InDeptStoreObject");
                }
            }
        }
        private RefStorageWarehouseLocation _InDeptStoreObject;
        #endregion

        public string ConvertDetailsListToXml()
        {
            return ConvertDetailsListToXml(_RequestDetails);
        }
        public string ConvertDetailsListToXml(IEnumerable<RequestDMedRscrInwardClinicDeptDetail> items)
        {
            if (items != null)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("<RequestDetails>");
                foreach (RequestDMedRscrInwardClinicDeptDetail details in items)
                {
                    int EntityState=(int)details.EntityState;
                    sb.Append("<RecInfo>");

                    sb.AppendFormat("<ReqDMedRscrDetailID>{0}</ReqDMedRscrDetailID>", details.ReqDMedRscrDetailID);
                    sb.AppendFormat("<ReqDMedRscrInClinicDeptID>{0}</ReqDMedRscrInClinicDeptID>", details.ReqDMedRscrInClinicDeptID);
                    sb.AppendFormat("<DMedRscrID>{0}</DMedRscrID>", details.DMedRscrID);
                    sb.AppendFormat("<Qty>{0}</Qty>", details.Qty);
                    sb.AppendFormat("<Note>{0}</Note>", details.Note);
                    sb.AppendFormat("<EntityState>{0}</EntityState>", EntityState);
                    sb.Append("</RecInfo>");
                }
                sb.Append("</RequestDetails>");
                return sb.ToString(); 
            }
            else
            {
                return null;
            }
        }

    }
}
