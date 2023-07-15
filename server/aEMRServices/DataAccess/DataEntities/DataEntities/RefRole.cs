using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{
    public partial class RefRole : NotifyChangedBase
    {
        #region Factory Method


        /// Create a new RefRole object.

        /// <param name="roleCode">Initial value of the RoleCode property.</param>
        /// <param name="roleDescription">Initial value of the RoleDescription property.</param>
        public static RefRole CreateRefRole(long roleCode, String roleDescription)
        {
            RefRole refRole = new RefRole();
            refRole.RoleCode = roleCode;
            refRole.RoleDescription = roleDescription;
            return refRole;
        }

        #endregion
        #region Primitive Properties





        [DataMemberAttribute()]
        public long RoleCode
        {
            get
            {
                return _RoleCode;
            }
            set
            {
                if (_RoleCode != value)
                {
                    OnRoleCodeChanging(value);
                    ////ReportPropertyChanging("RoleCode");
                    _RoleCode = value;
                    RaisePropertyChanged("RoleCode");
                    OnRoleCodeChanged();
                }
            }
        }
        private long _RoleCode;
        partial void OnRoleCodeChanging(long value);
        partial void OnRoleCodeChanged();





        [DataMemberAttribute()]
        public String RoleDescription
        {
            get
            {
                return _RoleDescription;
            }
            set
            {
                OnRoleDescriptionChanging(value);
                ////ReportPropertyChanging("RoleDescription");
                _RoleDescription = value;
                RaisePropertyChanged("RoleDescription");
                OnRoleDescriptionChanged();
            }
        }
        private String _RoleDescription;
        partial void OnRoleDescriptionChanging(String value);
        partial void OnRoleDescriptionChanged();

        #endregion

        #region Navigation Properties






        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_STAFFS_REL_HR04_REFROLES", "Staffs")]
        public ObservableCollection<Staff> Staffs
        {
            get;
            set;
        }

        #endregion
    }
}
