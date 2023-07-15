using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{
    public partial class Role : NotifyChangedBase
    {
        #region Factory Method


        /// Create a new Role object.

        /// <param name="roleID">Initial value of the RoleID property.</param>
        /// <param name="roleName">Initial value of the RoleName property.</param>
        public static Role CreateRole(Int32 roleID, String roleName)
        {
            Role role = new Role();
            role.RoleID = roleID;
            role.RoleName = roleName;
            return role;
        }

        #endregion
        #region Primitive Properties





        [DataMemberAttribute()]
        public Int32 RoleID
        {
            get
            {
                return _RoleID;
            }
            set
            {
                if (_RoleID != value)
                {
                    OnRoleIDChanging(value);
                    ////ReportPropertyChanging("RoleID");
                    _RoleID = value;
                    RaisePropertyChanged("RoleID");
                    OnRoleIDChanged();
                }
            }
        }
        private Int32 _RoleID;
        partial void OnRoleIDChanging(Int32 value);
        partial void OnRoleIDChanged();





        [DataMemberAttribute()]
        public String RoleName
        {
            get
            {
                return _RoleName;
            }
            set
            {
                OnRoleNameChanging(value);
                ////ReportPropertyChanging("RoleName");
                _RoleName = value;
                RaisePropertyChanged("RoleName");
                OnRoleNameChanged();
            }
        }
        private String _RoleName;
        partial void OnRoleNameChanging(String value);
        partial void OnRoleNameChanged();





        [DataMemberAttribute()]
        public String Description
        {
            get
            {
                return _Description;
            }
            set
            {
                OnDescriptionChanging(value);
                ////ReportPropertyChanging("Description");
                _Description = value;
                RaisePropertyChanged("Description");
                OnDescriptionChanged();
            }
        }
        private String _Description;
        partial void OnDescriptionChanging(String value);
        partial void OnDescriptionChanged();

        #endregion

        #region Navigation Properties






        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_GROUPROL_REL_UMGMT_ROLES", "GroupRole")]
        public ObservableCollection<GroupRole> GroupRoles
        {
            get;
            set;
        }






        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_PERMISSI_REL_UMGMT_ROLES", "Permission")]
        public ObservableCollection<Permission> Permissions
        {
            get;
            set;
        }

        [DataMemberAttribute()]
        public ObservableCollection<Operation> Operations
        {
            get;
            set;
        }
        #endregion

        public override bool Equals(object obj)
        {
            Role info = obj as Role;
            if (info == null)
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;

            return this.RoleID > 0 && this.RoleID == info.RoleID;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
