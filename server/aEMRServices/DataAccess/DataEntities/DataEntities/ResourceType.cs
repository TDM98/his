using System;

using System.Runtime.Serialization;
using eHCMS.Services.Core;
using eHCMS.Services.Core.Base;
using System.ComponentModel;

namespace DataEntities
{
    public partial class ResourceType: NotifyChangedBase, IEditableObject
    {
        public ResourceType()
            : base()
        {

        }

        #region Primitive Properties
        
        private ResourceType _tempResourceType;

        #endregion
        #region IEditableObject Members

        public void BeginEdit()
        {
            _tempResourceType = (ResourceType)this.MemberwiseClone();
        }

        public void CancelEdit()
        {
            if (null != _tempResourceType)
                CopyFrom(_tempResourceType);
            //_tempPatient = null;
        }

        public void EndEdit()
        {
        }

        public void CopyFrom(ResourceType p)
        {
            PropertyCopierHelper.CopyPropertiesTo(p, this);
        }

        [DataMemberAttribute()]
        public string TypeName 
        {
            get 
            {
                return _TypeName;
            }
            set 
            {
                if (_TypeName != value)
                {
                    OnTypeNameChanging(value);
                    _TypeName = value;
                    RaisePropertyChanged("TypeName");
                    OnTypeNameChanged();
                }
            }
        }
        private string _TypeName;
        partial void OnTypeNameChanging(string value);
        partial void OnTypeNameChanged();

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
        private string _Description;
        partial void OnDescriptionChanging(string value);
        partial void OnDescriptionChanged();

        [DataMemberAttribute()]
        public long RscrTypeID
        {
            get
            {
                return _RscrTypeID;
            }
            set
            {
                if (_RscrTypeID != value)
                {
                    OnRscrTypeIDChanging(value);
                    _RscrTypeID = value;
                    RaisePropertyChanged("RscrTypeID");
                    OnRscrTypeIDChanged();
                }
            }
        }
        private long _RscrTypeID;
        partial void OnRscrTypeIDChanging(long value);
        partial void OnRscrTypeIDChanged();
        #endregion
        #region Factory Method
        /// Create a new Hospital object.

        /// <param name="RscrTypeID">Initial value of the RscrTypeID property.</param>
        /// <param name="TypeName">Initial value of the TypeName property.</param>
        /// <param name="Description">Initial value of the Description property.</param>
        public static ResourceType CreateResourceType(long RscrTypeID, String TypeName, String Description)
        {
            ResourceType resourceType = new ResourceType();
            resourceType.TypeName = TypeName;
            resourceType.Description = Description;
            resourceType.RscrTypeID = RscrTypeID;
            return resourceType;
        }
        #endregion
        #region Navigation Properties
        public override bool Equals(object obj)
        {
            ResourceType seletedUnit = obj as ResourceType;
            if (seletedUnit == null)
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;

            return this.RscrTypeID == seletedUnit.RscrTypeID;
        }
        #endregion

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
