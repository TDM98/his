using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;
namespace DataEntities
{
    public partial class Permission : NotifyChangedBase
    {
        #region Factory Method


        /// Create a new Permission object.

        /// <param name="permissionItemID">Initial value of the PermissionItemID property.</param>
        /// <param name="roleID">Initial value of the RoleID property.</param>
        /// <param name="operationID">Initial value of the OperationID property.</param>
        public static Permission CreatePermission(long permissionItemID, Int32 roleID, Int32 operationID)
        {
            Permission permission = new Permission();
            permission.PermissionItemID = permissionItemID;
            permission.RoleID = roleID;
            permission.OperationID = operationID;
            return permission;
        }

        #endregion
        #region Primitive Properties





        [DataMemberAttribute()]
        public long PermissionItemID
        {
            get
            {
                return _PermissionItemID;
            }
            set
            {
                if (_PermissionItemID != value)
                {
                    OnPermissionItemIDChanging(value);
                    ////ReportPropertyChanging("PermissionItemID");
                    _PermissionItemID = value;
                    RaisePropertyChanged("PermissionItemID");
                    OnPermissionItemIDChanged();
                }
            }
        }
        private long _PermissionItemID;
        partial void OnPermissionItemIDChanging(long value);
        partial void OnPermissionItemIDChanged();





        [DataMemberAttribute()]
        public Int32 RoleID
        {
            get
            {
                return _RoleID;
            }
            set
            {
                OnRoleIDChanging(value);
                ////ReportPropertyChanging("RoleID");
                _RoleID = value;
                RaisePropertyChanged("RoleID");
                OnRoleIDChanged();
            }
        }
        private Int32 _RoleID;
        partial void OnRoleIDChanging(Int32 value);
        partial void OnRoleIDChanged();





        [DataMemberAttribute()]
        public Int32 OperationID
        {
            get
            {
                return _OperationID;
            }
            set
            {
                OnOperationIDChanging(value);
                ////ReportPropertyChanging("OperationID");
                _OperationID = value;
                RaisePropertyChanged("OperationID");
                OnOperationIDChanged();
            }
        }
        private Int32 _OperationID;
        partial void OnOperationIDChanging(Int32 value);
        partial void OnOperationIDChanged();

        [DataMemberAttribute()]
        public bool pFullControl
        {
            get
            {
                return _pFullControl;
            }
            set
            {
                OnpFullControlChanging(value);
                ////ReportPropertyChanging("pFullControl");
                _pFullControl = value;
                RaisePropertyChanged("pFullControl");
                OnpFullControlChanged();
            }
        }
        private bool _pFullControl=false;
        partial void OnpFullControlChanging(bool value);
        partial void OnpFullControlChanged();


        [DataMemberAttribute()]
        public bool pView
        {
            get
            {
                return _pView;
            }
            set
            {
                OnpViewChanging(value);
                ////ReportPropertyChanging("pView");
                _pView = value;
                RaisePropertyChanged("pView");
                OnpViewChanged();
            }
        }
        private bool _pView = false;
        partial void OnpViewChanging(bool value);
        partial void OnpViewChanged();

        [DataMemberAttribute()]
        public bool pAdd
        {
            get
            {
                return _pAdd;
            }
            set
            {
                OnpAddChanging(value);
                ////ReportPropertyChanging("pAdd");
                _pAdd = value;
                RaisePropertyChanged("pAdd");
                OnpAddChanged();
            }
        }
        private bool _pAdd = false;
        partial void OnpAddChanging(bool value);
        partial void OnpAddChanged();

        [DataMemberAttribute()]
        public bool pUpdate
        {
            get
            {
                return _pUpdate;
            }
            set
            {
                OnpUpdateChanging(value);
                ////ReportPropertyChanging("pUpdate");
                _pUpdate = value;
                RaisePropertyChanged("pUpdate");
                OnpUpdateChanged();
            }
        }
        private bool _pUpdate = false;
        partial void OnpUpdateChanging(bool value);
        partial void OnpUpdateChanged();

        [DataMemberAttribute()]
        public bool pDelete
        {
            get
            {
                return _pDelete;
            }
            set
            {
                OnpDeleteChanging(value);
                ////ReportPropertyChanging("pDelete");
                _pDelete = value;
                RaisePropertyChanged("pDelete");
                OnpDeleteChanged();
            }
        }
        private bool _pDelete = false;
        partial void OnpDeleteChanging(bool value);
        partial void OnpDeleteChanged();

        [DataMemberAttribute()]
        public bool pReport
        {
            get
            {
                return _pReport;
            }
            set
            {
                OnpReportChanging(value);
                ////ReportPropertyChanging("pReport");
                _pReport = value;
                RaisePropertyChanged("pReport");
                OnpReportChanged();
            }
        }
        private bool _pReport = false;
        partial void OnpReportChanging(bool value);
        partial void OnpReportChanged();

        [DataMemberAttribute()]
        public bool pPrint
        {
            get
            {
                return _pPrint;
            }
            set
            {
                OnpPrintChanging(value);
                ////ReportPropertyChanging("pPrint");
                _pPrint = value;
                RaisePropertyChanged("pReport");
                OnpPrintChanged();
            }
        }
        private bool _pPrint = false;
        partial void OnpPrintChanging(bool value);
        partial void OnpPrintChanged();
        #endregion

        #region Navigation Properties






        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_PERMISSI_REL_UMGMT_OPERATIO", "Operation")]
        public Operation Operation
        {
            get;
            set;
        }

        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_PERMISSI_REL_UMGMT_ROLES", "Roles")]
        public Role Role
        {
            get;
            set;
        }

        #endregion
        public override bool Equals(object obj)
        {
            Permission info = obj as Permission;
            if (info == null)
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;

            return this.PermissionItemID > 0 && this.PermissionItemID == info.PermissionItemID;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
