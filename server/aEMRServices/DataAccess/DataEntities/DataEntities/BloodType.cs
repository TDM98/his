using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{
    public partial class BloodType : NotifyChangedBase, IEditableObject
    {
        public BloodType()
            : base()
        {

        }
        public override bool Equals(object obj)
        {
            BloodType info = obj as BloodType;
            if (info == null)
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;

            return this.BloodTypeID > 0 && this.BloodTypeID == info.BloodTypeID;
        }
        private BloodType _tempBloodType;
        #region IEditableObject Members

        public void BeginEdit()
        {
            _tempBloodType = (BloodType)this.MemberwiseClone();
        }

        public void CancelEdit()
        {
            if (null != _tempBloodType)
                CopyFrom(_tempBloodType);
            //_tempPatient = null;
        }

        public void EndEdit()
        {
        }

        public void CopyFrom(BloodType p)
        {
            PropertyCopierHelper.CopyPropertiesTo(p, this);
        }

        #endregion
        #region Factory Method


        /// Create a new BloodType object.

        /// <param name="bloodTypeID">Initial value of the BloodTypeID property.</param>
        /// <param name="bloodTypeName">Initial value of the BloodTypeName property.</param>
        public static BloodType CreateBloodType(Byte bloodTypeID, String bloodTypeName)
        {
            BloodType bloodType = new BloodType();
            bloodType.BloodTypeID = bloodTypeID;
            bloodType.BloodTypeName = bloodTypeName;
            return bloodType;
        }

        #endregion
        #region Primitive Properties

        [DataMemberAttribute()]
        public Byte BloodTypeID
        {
            get
            {
                return _BloodTypeID;
            }
            set
            {
                if (_BloodTypeID != value)
                {
                    OnBloodTypeIDChanging(value);
                    _BloodTypeID = value;
                    RaisePropertyChanged("BloodTypeID");
                    OnBloodTypeIDChanged();
                }
            }
        }
        private Byte _BloodTypeID;
        partial void OnBloodTypeIDChanging(Byte value);
        partial void OnBloodTypeIDChanged();

        [DataMemberAttribute()]
        public String BloodTypeName
        {
            get
            {
                return _BloodTypeName;
            }
            set
            {
                OnBloodTypeNameChanging(value);
                _BloodTypeName = value;
                RaisePropertyChanged("BloodTypeName");
                OnBloodTypeNameChanged();
            }
        }
        private String _BloodTypeName;
        partial void OnBloodTypeNameChanging(String value);
        partial void OnBloodTypeNameChanged();

        [DataMemberAttribute()]
        public String RhType
        {
            get
            {
                return _RhType;
            }
            set
            {
                OnRhTypeChanging(value);
                _RhType = value;
                RaisePropertyChanged("RhType");
                OnRhTypeChanged();
            }
        }
        private String _RhType;
        partial void OnRhTypeChanging(String value);
        partial void OnRhTypeChanged();

        [DataMemberAttribute()]
        public String Descript
        {
            get
            {
                return _Descript;
            }
            set
            {
                OnDescriptChanging(value);
                _Descript = value;
                RaisePropertyChanged("Descript");
                OnDescriptChanged();
            }
        }
        private String _Descript;
        partial void OnDescriptChanging(String value);
        partial void OnDescriptChanged();
        #endregion

        #region Navigation Properties
        [DataMemberAttribute()]
        public ObservableCollection<Donor> Donors
        {
            get;
            set;
        }

        [DataMemberAttribute()]
        public ObservableCollection<InwardBlood> InwardBloods
        {
            get;
            set;
        }

        [DataMemberAttribute()]
        public ObservableCollection<OutwardBlood> OutwardBloods
        {
            get;
            set;
        }

        #endregion

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
