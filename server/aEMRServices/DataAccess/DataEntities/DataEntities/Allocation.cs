using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{
    public partial class Allocation : NotifyChangedBase, IEditableObject
    {
        public Allocation()
            : base()
        {

        }

        private Allocation _tempAllocation;
        #region IEditableObject Members

        public void BeginEdit()
        {
            _tempAllocation = (Allocation)this.MemberwiseClone();
        }

        public void CancelEdit()
        {
            if (null != _tempAllocation)
                CopyFrom(_tempAllocation);
            //_tempPatient = null;
        }

        public void EndEdit()
        {
        }

        public void CopyFrom(Allocation p)
        {
            PropertyCopierHelper.CopyPropertiesTo(p, this);
        }

        #endregion

        #region Factory Method
        /// Create a new Allocation object.

        /// <param name="allocationID">Initial value of the AllocationID property.</param>
        /// <param name="lID">Initial value of the LID property.</param>
        /// <param name="pARescrID">Initial value of the PARescrID property.</param>
        public static Allocation CreateAllocation(long allocationID, long lID, Int64 pARescrID)
        {
            Allocation allocation = new Allocation();
            allocation.AllocationID = allocationID;
            allocation.LID = lID;
            allocation.PARescrID = pARescrID;
            return allocation;
        }

        #endregion

        #region Primitive Properties

        [DataMemberAttribute()]
        public long AllocationID
        {
            get
            {
                return _AllocationID;
            }
            set
            {
                if (_AllocationID != value)
                {
                    OnAllocationIDChanging(value);
                    _AllocationID = value;
                    RaisePropertyChanged("AllocationID");
                    OnAllocationIDChanged();
                }
            }
        }
        private long _AllocationID;
        partial void OnAllocationIDChanging(long value);
        partial void OnAllocationIDChanged();

        [DataMemberAttribute()]
        public Nullable<Int64> StaffID
        {
            get
            {
                return _StaffID;
            }
            set
            {
                OnStaffIDChanging(value);
                _StaffID = value;
                RaisePropertyChanged("StaffID");
                OnStaffIDChanged();
            }
        }
        private Nullable<Int64> _StaffID;
        partial void OnStaffIDChanging(Nullable<Int64> value);
        partial void OnStaffIDChanged();

        [DataMemberAttribute()]
        public long LID
        {
            get
            {
                return _LID;
            }
            set
            {
                OnLIDChanging(value);
                _LID = value;
                RaisePropertyChanged("LID");
                OnLIDChanged();
            }
        }
        private long _LID;
        partial void OnLIDChanging(long value);
        partial void OnLIDChanged();

        [DataMemberAttribute()]
        public Int64 PARescrID
        {
            get
            {
                return _PARescrID;
            }
            set
            {
                OnPARescrIDChanging(value);
                _PARescrID = value;
                RaisePropertyChanged("PARescrID");
                OnPARescrIDChanged();
            }
        }
        private Int64 _PARescrID;
        partial void OnPARescrIDChanging(Int64 value);
        partial void OnPARescrIDChanged();

        [DataMemberAttribute()]
        public Nullable<DateTime> SetupDate
        {
            get
            {
                return _SetupDate;
            }
            set
            {
                OnSetupDateChanging(value);
                _SetupDate = value;
                RaisePropertyChanged("SetupDate");
                OnSetupDateChanged();
            }
        }
        private Nullable<DateTime> _SetupDate;
        partial void OnSetupDateChanging(Nullable<DateTime> value);
        partial void OnSetupDateChanged();

        [DataMemberAttribute()]
        public String Comments
        {
            get
            {
                return _Comments;
            }
            set
            {
                OnCommentsChanging(value);
                _Comments = value;
                RaisePropertyChanged("Comments");
                OnCommentsChanged();
            }
        }
        private String _Comments;
        partial void OnCommentsChanging(String value);
        partial void OnCommentsChanged();

        #endregion

        #region Navigation Properties

        [DataMemberAttribute()]
        public Location Location
        {
            get;
            set;
        }

        [DataMemberAttribute()]
        public Staff Staff
        {
            get;
            set;
        }

        [DataMemberAttribute()]
        public ArchitecturalResource ArchitecturalResource
        {
            get;
            set;
        }

        [DataMemberAttribute()]
        public ObservableCollection<AssignMedEquip> AssignMedEquips
        {
            get;
            set;
        }

        [DataMemberAttribute()]
        public ObservableCollection<BedLocation> BedLocations
        {
            get;
            set;
        }

        #endregion

    }
}
