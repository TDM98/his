using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{
    public partial class BedLocation : NotifyChangedBase, IEditableObject
    {
        public BedLocation()
            : base()
        {

        }
        public override bool Equals(object obj)
        {
            BedLocation info = obj as BedLocation;
            if (info == null)
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;

            return this.AllocationID > 0 && this.AllocationID == info.AllocationID;
        }

        public override int GetHashCode()
        {
            return this.AllocationID.GetHashCode();
        }

        private BedLocation _tempBedLocation;
        #region IEditableObject Members

        public void BeginEdit()
        {
            _tempBedLocation = (BedLocation)this.MemberwiseClone();
        }

        public void CancelEdit()
        {
            if (null != _tempBedLocation)
                CopyFrom(_tempBedLocation);
            //_tempPatient = null;
        }

        public void EndEdit()
        {
        }

        public void CopyFrom(BedLocation p)
        {
            PropertyCopierHelper.CopyPropertiesTo(p, this);
        }

        #endregion
        #region Factory Method


        /// Create a new BedLocation object.

        /// <param name="bedLocNumber">Initial value of the BedLocNumber property.</param>
        /// <param name="allocationID">Initial value of the AllocationID property.</param>
        public static BedLocation CreateBedLocation(String bedLocNumber, long allocationID)
        {
            BedLocation bedLocation = new BedLocation();
            bedLocation.BedLocNumber = bedLocNumber;
            bedLocation.AllocationID = allocationID;
            return bedLocation;
        }

        #endregion
        #region Primitive Properties

        [DataMemberAttribute()]
        public String BedLocNumber
        {
            get
            {
                return _BedLocNumber;
            }
            set
            {
                if (_BedLocNumber != value)
                {
                    OnBedLocNumberChanging(value);
                    _BedLocNumber = value;
                    RaisePropertyChanged("BedLocNumber");
                    OnBedLocNumberChanged();
                }
            }
        }
        private String _BedLocNumber;
        partial void OnBedLocNumberChanging(String value);
        partial void OnBedLocNumberChanged();
        [DataMemberAttribute()]
        public Nullable<long> MedServiceID
        {
            get
            {
                return _MedServiceID;
            }
            set
            {
                OnMedServiceIDChanging(value);
                _MedServiceID = value;
                RaisePropertyChanged("MedServiceID");
                OnMedServiceIDChanged();
            }
        }
        private Nullable<long> _MedServiceID;
        partial void OnMedServiceIDChanging(Nullable<long> value);
        partial void OnMedServiceIDChanged();

        [DataMemberAttribute()]
        public long AllocationID
        {
            get
            {
                return _AllocationID;
            }
            set
            {
                OnAllocationIDChanging(value);
                _AllocationID = value;
                RaisePropertyChanged("AllocationID");
                OnAllocationIDChanged();
            }
        }
        private long _AllocationID;
        partial void OnAllocationIDChanging(long value);
        partial void OnAllocationIDChanged();

        [DataMemberAttribute()]
        public Nullable<Int64> V_BedLocType
        {
            get
            {
                return _V_BedLocType;
            }
            set
            {
                OnV_BedLocTypeChanging(value);
                _V_BedLocType = value;
                RaisePropertyChanged("V_BedLocType");
                OnV_BedLocTypeChanged();
            }
        }
        private Nullable<Int64> _V_BedLocType;
        partial void OnV_BedLocTypeChanging(Nullable<Int64> value);
        partial void OnV_BedLocTypeChanged();

        [DataMemberAttribute()]
        public Nullable<Boolean> IsSemiPrivateBed
        {
            get
            {
                return _IsSemiPrivateBed;
            }
            set
            {
                OnIsSemiPrivateBedChanging(value);
                _IsSemiPrivateBed = value;
                RaisePropertyChanged("IsSemiPrivateBed");
                OnIsSemiPrivateBedChanged();
            }
        }
        private Nullable<Boolean> _IsSemiPrivateBed;
        partial void OnIsSemiPrivateBedChanging(Nullable<Boolean> value);
        partial void OnIsSemiPrivateBedChanged();

        [DataMemberAttribute()]
        public Nullable<Boolean> IsIntensiveCareUnit
        {
            get
            {
                return _IsIntensiveCareUnit;
            }
            set
            {
                OnIsIntensiveCareUnitChanging(value);
                _IsIntensiveCareUnit = value;
                RaisePropertyChanged("IsIntensiveCareUnit");
                OnIsIntensiveCareUnitChanged();
            }
        }
        private Nullable<Boolean> _IsIntensiveCareUnit;
        partial void OnIsIntensiveCareUnitChanging(Nullable<Boolean> value);
        partial void OnIsIntensiveCareUnitChanged();

        #endregion

        #region Navigation Properties

        [DataMemberAttribute()]
        public Allocation Allocation
        {
            get;
            set;
        }

        [DataMemberAttribute()]
        public RefMedicalServiceItem RefMedicalServiceItem
        {
            get;
            set;
        }

        [DataMemberAttribute()]
        public ObservableCollection<HospitalizationHistoryDetail> HospitalizationHistoryDetails
        {
            get;
            set;
        }

        #endregion
    }
}
