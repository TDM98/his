using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{
    public partial class UserGroup : NotifyChangedBase
    {
        #region Factory Method


        /// Create a new UserGroup object.

        /// <param name="uGID">Initial value of the UGID property.</param>
        public static UserGroup CreateUserGroup(long uGID)
        {
            UserGroup userGroup = new UserGroup();
            userGroup.UGID = uGID;
            return userGroup;
        }

        #endregion
        #region Primitive Properties





        [DataMemberAttribute()]
        public long UGID
        {
            get
            {
                return _UGID;
            }
            set
            {
                if (_UGID != value)
                {
                    OnUGIDChanging(value);
                    ////ReportPropertyChanging("UGID");
                    _UGID = value;
                    RaisePropertyChanged("UGID");
                    OnUGIDChanged();
                }
            }
        }
        private long _UGID;
        partial void OnUGIDChanging(long value);
        partial void OnUGIDChanged();





        [DataMemberAttribute()]
        public Nullable<long> AccountID
        {
            get
            {
                return _AccountID;
            }
            set
            {
                OnAccountIDChanging(value);
                ////ReportPropertyChanging("AccountID");
                _AccountID = value;
                RaisePropertyChanged("AccountID");
                OnAccountIDChanged();
            }
        }
        private Nullable<long> _AccountID;
        partial void OnAccountIDChanging(Nullable<long> value);
        partial void OnAccountIDChanged();





        [DataMemberAttribute()]
        public Nullable<Int32> GroupID
        {
            get
            {
                return _GroupID;
            }
            set
            {
                OnGroupIDChanging(value);
                ////ReportPropertyChanging("GroupID");
                _GroupID = value;
                RaisePropertyChanged("GroupID");
                OnGroupIDChanged();
            }
        }
        private Nullable<Int32> _GroupID;
        partial void OnGroupIDChanging(Nullable<Int32> value);
        partial void OnGroupIDChanged();

        #endregion

        #region Navigation Properties






        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_USERGROU_REL_UMGMT_GROUPS", "Groups")]
        public Group Group
        {
            get;
            set;
        }

        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_USERGROU_REL_UMGMT_USERACCO", "UserAccounts")]
        public UserAccount UserAccount
        {
            get;
            set;
        }



        #endregion

        public override bool Equals(object obj)
        {
            UserGroup info = obj as UserGroup;
            if (info == null)
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;

            return this.UGID > 0 && this.UGID == info.UGID;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
