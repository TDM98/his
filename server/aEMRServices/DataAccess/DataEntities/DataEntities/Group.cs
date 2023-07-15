using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;
using System.ComponentModel.DataAnnotations;

namespace DataEntities
{
    public partial class Group:NotifyChangedBase
    {
        #region Factory Method

     
        /// Create a new Group object.
     
        /// <param name="groupID">Initial value of the GroupID property.</param>
        /// <param name="groupName">Initial value of the GroupName property.</param>
        public static Group CreateUserGroup(Int32 groupID, String groupName)
        {
            Group userGroup = new Group();
            userGroup.GroupID = groupID;
            userGroup.GroupName = groupName;
            return userGroup;
        }

        #endregion
        #region Primitive Properties

     
        
     
        
        [DataMemberAttribute()]
        public Int32 GroupID
        {
            get
            {
                return _GroupID;
            }
            set
            {
                if (_GroupID != value)
                {
                    OnGroupIDChanging(value);
                    _GroupID = value;
                    OnGroupIDChanged();
                }
            }
        }
        private Int32 _GroupID;
        partial void OnGroupIDChanging(Int32 value);
        partial void OnGroupIDChanged();

     
        
     
        /// 
        [Required(ErrorMessage="Group name is required")]
        [DataMemberAttribute()]
        public String GroupName
        {
            get
            {
                return _GroupName;
            }
            set
            {
                OnGroupNameChanging(value);
                ValidateProperty("GroupName", value);
                _GroupName = value;
                OnGroupNameChanged();
            }
        }
        private String _GroupName;
        partial void OnGroupNameChanging(String value);
        partial void OnGroupNameChanged();

     
        
     
        
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
                _Description = value;
                OnDescriptionChanged();
            }
        }
        private String _Description;
        partial void OnDescriptionChanging(String value);
        partial void OnDescriptionChanged();

        public bool IsSelected
        {
            get
            {
                return _isSelected;
            }
            set
            {
                _isSelected = value;
                RaisePropertyChanged("IsSelected");
            }
        }
        private bool _isSelected = false;
        #endregion

        #region Navigation Properties

     
        
     


        [DataMemberAttribute()]
        public ObservableCollection<UserAccount> Users
        {
            get;
            set;
        }


        [DataMemberAttribute()]
        public ObservableCollection<Role> Roles
        {
            get;
            set;
        }

        #endregion
        public override bool Equals(object obj)
        {
            Group info = obj as Group;
            if (info == null)
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;

            return this.GroupID > 0 && this.GroupID == info.GroupID;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
