using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;
using System.Collections.Generic;
using System.Text;

namespace DataEntities
{
    public partial class RequestDrugInward : NotifyChangedBase
    {
        #region Factory Method

       
        /// Create a new RequestDrugInward object.
     
        /// <param name="ReqDrugInID">Initial value of the ReqDrugInID property.</param>
        /// <param name="staffID">Initial value of the StaffID property.</param>
        /// <param name="reqDate">Initial value of the ReqDate property.</param>
        public static RequestDrugInward CreateRequestDrugInward(Int64 ReqDrugInID, Int64 staffID, DateTime reqDate)
        {
            RequestDrugInward RequestDrugInward = new RequestDrugInward();
            RequestDrugInward.ReqDrugInID = ReqDrugInID;
            RequestDrugInward.StaffID = staffID;
            RequestDrugInward.ReqDate = reqDate;
            return RequestDrugInward;
        }

        #endregion
        #region Primitive Properties

        [DataMemberAttribute()]
        public Int64 ReqDrugInID
        {
            get
            {
                return _ReqDrugInID;
            }
            set
            {
                if (_ReqDrugInID != value)
                {
                    OnReqDrugInIDChanging(value);
                    //ReportPropertyChanging("ReqDrugInID");
                    _ReqDrugInID = value;
                    RaisePropertyChanged("ReqDrugInID");
                    OnReqDrugInIDChanged();

                    RaisePropertyChanged("CanSave");
                    RaisePropertyChanged("CanDelete");
                    RaisePropertyChanged("CanPrint");
                }
            }
        }
        private Int64 _ReqDrugInID;
        partial void OnReqDrugInIDChanging(Int64 value);
        partial void OnReqDrugInIDChanged();

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
                _StaffID = value;
                RaisePropertyChanged("StaffID");
                OnStaffIDChanged();
            }
        }
        private Int64 _StaffID;
        partial void OnStaffIDChanging(Int64 value);
        partial void OnStaffIDChanged();


        [DataMemberAttribute()]
        public Nullable<Int64> StaffIDAllow
        {
            get
            {
                return _StaffIDAllow;
            }
            set
            {
                _StaffIDAllow = value;
                RaisePropertyChanged("StaffIDAllow");
            }
        }
        private Nullable<Int64> _StaffIDAllow;

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
        public ObservableCollection<RequestDrugInwardDetail> RequestDetails
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
        private ObservableCollection<RequestDrugInwardDetail> _RequestDetails;

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
        public Staff SelectedStaffAllow
        {
            get
            {
                return _SelectedStaffAllow;
            }
            set
            {
                if (_SelectedStaffAllow != value)
                {
                    _SelectedStaffAllow = value;
                    if (_SelectedStaffAllow != null)
                    {
                        _StaffIDAllow = _SelectedStaffAllow.StaffID;
                    }
                    RaisePropertyChanged("SelectedStaffAllow");
                }
            }
        }
        private Staff _SelectedStaffAllow;

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

        [DataMemberAttribute()]
        public bool? DaNhanHang
        {
            get
            {
                return _DaNhanHang;
            }
            set
            {

                _DaNhanHang = value;
                RaisePropertyChanged("DaNhanHang");
                RaisePropertyChanged("CanSave");
                RaisePropertyChanged("CanDelete");
            }
        }
        private bool? _DaNhanHang;

        public bool CanSave
        {
            get { return (ReqDrugInID == 0 || ReqDrugInID > 0) && DaNhanHang.GetValueOrDefault() == false; }
        }
        public bool CanDelete
        {
            get { return ReqDrugInID > 0 && DaNhanHang.GetValueOrDefault() == false; }
        }
        public bool CanPrint
        {
            get { return ReqDrugInID > 0; }
        }

        public string ConvertDetailsListToXml()
        {
            return ConvertDetailsListToXml(_RequestDetails);
        }
        public string ConvertDetailsListToXml(IEnumerable<RequestDrugInwardDetail> items)
        {
            if (items != null)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("<RequestDetails>");
                foreach (RequestDrugInwardDetail details in items)
                {
                    if (details.RefGenericDrugDetail != null &&  details.RefGenericDrugDetail.DrugID > 0)
                    {
                        int EntityState = (int)details.EntityState;
                        sb.Append("<RecInfo>");
                        sb.AppendFormat("<ReqDetailID>{0}</ReqDetailID>", details.ReqDetailID);
                        sb.AppendFormat("<ReqDrugInID>{0}</ReqDrugInID>", details.ReqDrugInID);
                        sb.AppendFormat("<DrugID>{0}</DrugID>", details.DrugID);
                        sb.AppendFormat("<Qty>{0}</Qty>", details.Qty);
                        sb.AppendFormat("<Note>{0}</Note>", details.Note);
                        sb.AppendFormat("<EntityState>{0}</EntityState>", EntityState);
                        sb.Append("</RecInfo>");
                    }
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
