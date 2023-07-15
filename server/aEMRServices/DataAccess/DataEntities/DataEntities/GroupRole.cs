using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{
    public partial class GroupRole : NotifyChangedBase
    {
        #region Factory Method


        /// Create a new GroupRole object.

        /// <param name="groupRoleID">Initial value of the GroupRoleID property.</param>
        /// <param name="groupID">Initial value of the GroupID property.</param>
        /// <param name="roleID">Initial value of the RoleID property.</param>
        public static GroupRole CreateGroupRole(long groupRoleID, Int32 groupID, Int32 roleID)
        {
            GroupRole groupRole = new GroupRole();
            groupRole.GroupRoleID = groupRoleID;
            groupRole.GroupID = groupID;
            groupRole.RoleID = roleID;
            return groupRole;
        }

        #endregion
        #region Primitive Properties
        [DataMemberAttribute()]
        public long GroupRoleID
        {
            get
            {
                return _GroupRoleID;
            }
            set
            {
                if (_GroupRoleID != value)
                {
                    OnGroupRoleIDChanging(value);
                    _GroupRoleID = value;
                    RaisePropertyChanged("GroupRoleID");
                    OnGroupRoleIDChanged();
                }
            }
        }
        private long _GroupRoleID;
        partial void OnGroupRoleIDChanging(long value);
        partial void OnGroupRoleIDChanged();





        [DataMemberAttribute()]
        public Int32 GroupID
        {
            get
            {
                return _GroupID;
            }
            set
            {
                OnGroupIDChanging(value);
                _GroupID = value;
                RaisePropertyChanged("GroupID");
                OnGroupIDChanged();
            }
        }
        private Int32 _GroupID;
        partial void OnGroupIDChanging(Int32 value);
        partial void OnGroupIDChanged();

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
                _RoleID = value;
                RaisePropertyChanged("RoleID");
                OnRoleIDChanged();
            }
        }
        private Int32 _RoleID;
        partial void OnRoleIDChanging(Int32 value);
        partial void OnRoleIDChanged();

        #endregion

        #region Navigation Properties






        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_GROUPROL_REL_UMGMT_GROUPS", "Groups")]
        public Group Group
        {
            get;
            set;
        }

        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_GROUPROL_REL_UMGMT_ROLES", "Roles")]
        public Role Role
        {
            get;
            set;
        }
        #endregion

        public override bool Equals(object obj)
        {
            GroupRole info = obj as GroupRole;
            if (info == null)
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;

            return this.GroupRoleID > 0 && this.GroupRoleID == info.GroupRoleID;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
