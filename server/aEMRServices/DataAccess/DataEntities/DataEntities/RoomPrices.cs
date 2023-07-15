using System;

using System.Runtime.Serialization;
using eHCMS.Services.Core;
using eHCMS.Services.Core.Base;
using System.ComponentModel;

namespace DataEntities
{
    public partial class RoomPrices: NotifyChangedBase, IEditableObject
    {
        public RoomPrices()
            : base()
        {

        }

        #region Primitive Properties

        private RoomPrices _tempRoomPrices;

        #endregion
        #region IEditableObject Members

        public void BeginEdit()
        {
            _tempRoomPrices = (RoomPrices)this.MemberwiseClone();
        }

        public void CancelEdit()
        {
            if (null != _tempRoomPrices)
                CopyFrom(_tempRoomPrices);
            //_tempPatient = null;
        }

        public void EndEdit()
        {
        }

        public void CopyFrom(RoomPrices p)
        {
            PropertyCopierHelper.CopyPropertiesTo(p, this);
        }

        #region Crimitive 

        [DataMemberAttribute()]
        public long RoomPriceID
        {
            get
            {
                return _RoomPriceID;
            }
            set
            {
                OnRoomPriceIDChanging(value);
                _RoomPriceID = value;
                RaisePropertyChanged("RoomPriceID");
                OnRoomPriceIDChanged();
            }
        }
        private long _RoomPriceID;
        partial void OnRoomPriceIDChanging(long value);
        partial void OnRoomPriceIDChanged();




        [DataMemberAttribute()]
        public DateTime RecDateCreated
        {
            get
            {
                return _RecDateCreated;
            }
            set
            {
                OnRecDateCreatedChanging(value);
                _RecDateCreated = value;
                RaisePropertyChanged("RecDateCreated");
                OnRecDateCreatedChanged();
            }
        }
        private DateTime _RecDateCreated;
        partial void OnRecDateCreatedChanging(DateTime value);
        partial void OnRecDateCreatedChanged();




        [DataMemberAttribute()]
        public long DeptLocationID
        {
            get
            {
                return _DeptLocationID;
            }
            set
            {
                OnDeptLocationIDChanging(value);
                _DeptLocationID = value;
                RaisePropertyChanged("DeptLocationID");
                OnDeptLocationIDChanged();
            }
        }
        private long _DeptLocationID;
        partial void OnDeptLocationIDChanging(long value);
        partial void OnDeptLocationIDChanged();




        [DataMemberAttribute()]
        public long StaffID
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
        private long _StaffID;
        partial void OnStaffIDChanging(long value);
        partial void OnStaffIDChanged();




        [DataMemberAttribute()]
        public Nullable<DateTime> EffectiveDate
        {
            get
            {
                return _EffectiveDate;
            }
            set
            {
                OnEffectiveDateChanging(value);
                _EffectiveDate = value;
                RaisePropertyChanged("EffectiveDate");
                OnEffectiveDateChanged();
            }
        }
        private Nullable<DateTime> _EffectiveDate;
        partial void OnEffectiveDateChanging(Nullable<DateTime> value);
        partial void OnEffectiveDateChanged();




        [DataMemberAttribute()]
        public Decimal NormalPrice
        {
            get
            {
                return _NormalPrice;
            }
            set
            {
                OnNormalPriceChanging(value);
                _NormalPrice = value;
                RaisePropertyChanged("NormalPrice");
                OnNormalPriceChanged();
            }
        }
        private Decimal _NormalPrice;
        partial void OnNormalPriceChanging(Decimal value);
        partial void OnNormalPriceChanged();




        [DataMemberAttribute()]
        public Decimal PriceForHIPatient
        {
            get
            {
                return _PriceForHIPatient;
            }
            set
            {
                OnPriceForHIPatientChanging(value);
                _PriceForHIPatient = value;
                RaisePropertyChanged("PriceForHIPatient");
                OnPriceForHIPatientChanged();
            }
        }
        private Decimal _PriceForHIPatient;
        partial void OnPriceForHIPatientChanging(Decimal value);
        partial void OnPriceForHIPatientChanged();




        [DataMemberAttribute()]
        public Decimal HIAllowedPrice
        {
            get
            {
                return _HIAllowedPrice;
            }
            set
            {
                OnHIAllowedPriceChanging(value);
                _HIAllowedPrice = value;
                RaisePropertyChanged("HIAllowedPrice");
                OnHIAllowedPriceChanged();
            }
        }
        private Decimal _HIAllowedPrice;
        partial void OnHIAllowedPriceChanging(Decimal value);
        partial void OnHIAllowedPriceChanged();




        [DataMemberAttribute()]
        public string Note
        {
            get
            {
                return _Note;
            }
            set
            {
                OnNoteChanging(value);
                _Note = value;
                RaisePropertyChanged("Note");
                OnNoteChanged();
            }
        }
        private string _Note;
        partial void OnNoteChanging(string value);
        partial void OnNoteChanged();




        [DataMemberAttribute()]
        public bool IsActive
        {
            get
            {
                return _IsActive;
            }
            set
            {
                OnIsActiveChanging(value);
                _IsActive = value;
                RaisePropertyChanged("IsActive");
                OnIsActiveChanged();
            }
        }
        private bool _IsActive;
        partial void OnIsActiveChanging(bool value);
        partial void OnIsActiveChanged();

#endregion

        #endregion
        #region Factory Method
        /// Create a new Hospital object.

        /// <param name="RscrTypeID">Initial value of the RscrTypeID property.</param>
        /// <param name="TypeName">Initial value of the TypeName property.</param>
        /// <param name="Description">Initial value of the Description property.</param>
        public static RoomPrices CreateResourceType(long RscrTypeID, String TypeName, String Description)
        {
            RoomPrices resourceType = new RoomPrices();
            //resourceType.TypeName = TypeName;
            //resourceType.Description = Description;
            //resourceType.RscrTypeID = RscrTypeID;
            return resourceType;
        }
        #endregion

        #region Navigation Properties
        public override bool Equals(object obj)
        {
            RoomPrices seletedUnit = obj as RoomPrices;
            if (seletedUnit == null)
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;

            return this.RoomPriceID == seletedUnit.RoomPriceID;
        }

        [DataMemberAttribute()]
        public Staff VStaff
        {
            get
            {
                return _VStaff;
            }
            set
            {
                OnVStaffChanging(value);
                _VStaff = value;
                RaisePropertyChanged("VStaff");
                OnVStaffChanged();
            }
        }
        private Staff _VStaff;
        partial void OnVStaffChanging(Staff value);
        partial void OnVStaffChanged();


        [DataMemberAttribute()]
        public DeptLocation VDeptLocation
        {
            get
            {
                return _VDeptLocation;
            }
            set
            {
                OnVDeptLocationChanging(value);
                _VDeptLocation = value;
                RaisePropertyChanged("VDeptLocation");
                OnVDeptLocationChanged();
            }
        }
        private DeptLocation _VDeptLocation;
        partial void OnVDeptLocationChanging(DeptLocation value);
        partial void OnVDeptLocationChanged();


        #endregion

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
