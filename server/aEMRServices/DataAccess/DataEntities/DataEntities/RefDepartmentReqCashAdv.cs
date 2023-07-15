using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{
    public partial class RefDepartmentReqCashAdv : NotifyChangedBase
    {
        public RefDepartmentReqCashAdv()
            : base()
        {

        }

        #region Factory Method

        /// Create a new RefDepartmentReqCashAdv object.

        /// <param name="RefDepartmentReqCashAdvID">Initial value of the RefDepartmentReqCashAdvID property.</param>
        /// <param name="status">Initial value of the Status property.</param>
        public static RefDepartmentReqCashAdv CreateRefDepartmentReqCashAdv(Int32 RefDepartmentReqCashAdvID)
        {
            RefDepartmentReqCashAdv RefDepartmentReqCashAdv = new RefDepartmentReqCashAdv();
            RefDepartmentReqCashAdv.RefDepartmentReqCashAdvID = RefDepartmentReqCashAdvID;
            return RefDepartmentReqCashAdv;
        }

        #endregion

        #region Primitive Properties
        [DataMemberAttribute()]
        public Int64 RefDepartmentReqCashAdvID
        {
            get
            {
                return _RefDepartmentReqCashAdvID;
            }
            set
            {
                if (_RefDepartmentReqCashAdvID != value)
                {
                    OnRefDepartmentReqCashAdvIDChanging(value);
                    _RefDepartmentReqCashAdvID = value;
                    RaisePropertyChanged("RefDepartmentReqCashAdvID");
                    OnRefDepartmentReqCashAdvIDChanged();
                }
            }
        }
        private Int64 _RefDepartmentReqCashAdvID;
        partial void OnRefDepartmentReqCashAdvIDChanging(Int64 value);
        partial void OnRefDepartmentReqCashAdvIDChanged();

        [DataMemberAttribute()]
        public Int64 DeptID
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
        private Int64 _DeptID;
        partial void OnDeptIDChanging(Int64 value);
        partial void OnDeptIDChanged();

        [DataMemberAttribute()]
        public decimal CashAdvAmtReq
        {
            get
            {
                return _CashAdvAmtReq;
            }
            set
            {
                OnCashAdvAmtReqChanging(value);
                _CashAdvAmtReq = value;
                RaisePropertyChanged("CashAdvAmtReq");
                OnCashAdvAmtReqChanged();
            }
        }
        private decimal _CashAdvAmtReq;
        partial void OnCashAdvAmtReqChanging(decimal value);
        partial void OnCashAdvAmtReqChanged();

        #endregion


        [DataMemberAttribute()]
        public String DeptName
        {
            get
            {
                return _DeptName;
            }
            set
            {
                OnDeptNameChanging(value);
                _DeptName = value;
                RaisePropertyChanged("DeptName");
                OnDeptNameChanged();
            }
        }
        private String _DeptName;
        partial void OnDeptNameChanging(String value);
        partial void OnDeptNameChanged();
    }

}
