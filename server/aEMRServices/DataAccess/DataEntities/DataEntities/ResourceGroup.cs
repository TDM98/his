using System;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Windows;

using eHCMS.Services.Core.Base;
using System.ComponentModel;
using System.Runtime.Serialization;
using eHCMS.Services.Core;

namespace DataEntities
{
    public partial class ResourceGroup : NotifyChangedBase, IEditableObject
    {
        public ResourceGroup ():base()
        {

        }

        #region Primitive Properties
        private string _GroupName;
        private string _Description;
        private long _RscrGroupID;
        private long _V_ResGroupCategory;
        private ResourceGroup _tempResourceGroup;

        #endregion
        #region IEditableObject Members

        public void BeginEdit()
        {
            _tempResourceGroup = (ResourceGroup)this.MemberwiseClone();
        }

        public void CancelEdit()
        {
            if (null != _tempResourceGroup)
                CopyFrom(_tempResourceGroup);
            //_tempPatient = null;
        }

        public void EndEdit()
        {
        }

        public void CopyFrom(ResourceGroup p)
        {
            PropertyCopierHelper.CopyPropertiesTo(p, this);
        }

        [DataMemberAttribute()]
        [Required(ErrorMessage = "Bạn phải nhập Nhom Vật Tư!")]
        [StringLength(128, ErrorMessage = "Ten quá dài!")]
        public string GroupName 
        {
            get 
            {
                return _GroupName;
            }
            set 
            {
                if (_GroupName != value)
                {
                    OnGroupNameChanging(value);
                    _GroupName = value;
                    RaisePropertyChanged("GroupName");
                    OnGroupNameChanged();
                }
            }
        }
        partial void OnGroupNameChanging(string value);
        partial void OnGroupNameChanged();

        [DataMemberAttribute()]
        public string Description
        {
            get
            {
                return _Description;
            }
            set
            {
                if (_Description != value)
                {
                    OnDescriptionChanging(value);
                    _Description = value;
                    RaisePropertyChanged("Description");
                    OnDescriptionChanged();
                }
            }
        }
        partial void OnDescriptionChanging(string value);
        partial void OnDescriptionChanged();

        [DataMemberAttribute()]
        public long V_ResGroupCategory
        {
            get
            {
                return _V_ResGroupCategory;
            }
            set
            {
                if (_V_ResGroupCategory == value)
                    return;
                _V_ResGroupCategory = value;
            }
        }
        partial void OnV_ResGroupCategoryChanging(long value);
        partial void OnV_ResGroupCategoryChanged();

        [DataMemberAttribute()]
        public long RscrGroupID
        {
            get
            {
                return _RscrGroupID;
            }
            set
            {
                if (_RscrGroupID != value)
                {
                    OnRscrGroupIDChanging(value);
                    _RscrGroupID = value;
                    RaisePropertyChanged("RscrGroupID");
                    OnRscrGroupIDChanged();
                }
            }
        }
        partial void OnRscrGroupIDChanging(long value);
        partial void OnRscrGroupIDChanged();
        #endregion
        #region Factory Method
        /// Create a new Hospital object.

        /// <param name="RscrGroupID">Initial value of the RscrGroupID property.</param>
        /// <param name="GroupName">Initial value of the GroupName property.</param>
        /// <param name="Description">Initial value of the Description property.</param>
        public static ResourceGroup CreateResourceGroup(long RscrGroupID, String GroupName, String Description)
        {
            ResourceGroup resourceGroup = new ResourceGroup();
            resourceGroup.GroupName = GroupName;
            resourceGroup.Description = Description;
            resourceGroup.RscrGroupID = RscrGroupID;
            return resourceGroup;
        }
        #endregion
        #region Navigation Properties
        public override bool Equals(object obj)
        {
            ResourceGroup seletedUnit = obj as ResourceGroup;
            if (seletedUnit == null)
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;

            return this.RscrGroupID == seletedUnit.RscrGroupID;
        }

        [DataMemberAttribute()]
        public Lookup _VResGroupCategory;
        public Lookup VResGroupCategory
        {
            get
            {
                return _VResGroupCategory;
            }
            set
            {
                if (_VResGroupCategory != value)
                {
                    OnVResGroupCategoryChanging(value);
                    _VResGroupCategory = value;
                    RaisePropertyChanged("VResGroupCategory");
                    OnVResGroupCategoryChanged();
                }
            }
        }
        partial void OnVResGroupCategoryChanging(Lookup value);
        partial void OnVResGroupCategoryChanged();
        #endregion
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

    }
}

