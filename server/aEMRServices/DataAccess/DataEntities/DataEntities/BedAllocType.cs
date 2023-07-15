using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{
    public partial class BedAllocType : NotifyChangedBase, IEditableObject
    {
        public BedAllocType()
            : base()
        {

        }

        private BedAllocType _tempBedAllocType;
        #region IEditableObject Members

        public void BeginEdit()
        {
            _tempBedAllocType = (BedAllocType)this.MemberwiseClone();
        }

        public void CancelEdit()
        {
            if (null != _tempBedAllocType)
                CopyFrom(_tempBedAllocType);
            //_tempPatient = null;
        }

        public void EndEdit()
        {
        }

        public void CopyFrom(BedAllocType p)
        {
            PropertyCopierHelper.CopyPropertiesTo(p, this);
        }

        #endregion
        #region Factory Method


        /// Create a new BedAllocType object.

        /// <param name="bedLocNumber">Initial value of the BedLocNumber property.</param>
        /// <param name="allocationID">Initial value of the AllocationID property.</param>
        public static BedAllocType CreateBedAllocType(String bedLocNumber, long allocationID)
        {
            BedAllocType BedAllocType = new BedAllocType();
            //BedAllocType.BedLocNumber = bedLocNumber;
            //BedAllocType.AllocationID = allocationID;
            return BedAllocType;
        }

        #endregion
        #region Primitive Properties

        [DataMemberAttribute()]
        public long BedLocTypeID
        {
            get
            {
                return _BedLocTypeID;
            }
            set
            {
                OnBedLocTypeIDChanging(value);
                _BedLocTypeID = value;
                RaisePropertyChanged("BedLocTypeID");
                OnBedLocTypeIDChanged();
            }
        }
        private long _BedLocTypeID;
        partial void OnBedLocTypeIDChanging(long value);
        partial void OnBedLocTypeIDChanged();

        [DataMemberAttribute()]
        public string BedLocTypeName
        {
            get
            {
                return _BedLocTypeName;
            }
            set
            {
                OnBedLocTypeNameChanging(value);
                _BedLocTypeName = value;
                RaisePropertyChanged("BedLocTypeName");
                OnBedLocTypeNameChanged();
            }
        }
        private string _BedLocTypeName = "";
        partial void OnBedLocTypeNameChanging(string value);
        partial void OnBedLocTypeNameChanged();

        [DataMemberAttribute()]
        public string Description
        {
            get
            {
                return _Description;
            }
            set
            {
                OnDescriptionChanging(value);
                _Description = value;
                RaisePropertyChanged("Description");
                OnDescriptionChanged();
            }
        }
        private string _Description = "";
        partial void OnDescriptionChanging(string value);
        partial void OnDescriptionChanged();
        #endregion

        #region Navigation Properties

        
        #endregion
    }
}
