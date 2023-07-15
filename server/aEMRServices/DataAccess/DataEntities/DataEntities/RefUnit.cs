using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;
using System.ComponentModel.DataAnnotations;

namespace DataEntities
{
    public partial class RefUnit:NotifyChangedBase,IEditableObject
    {
        public RefUnit()
            : base()
        {

        }

        private RefUnit _tempRefUnit;
        #region IEditableObject Members

        public void BeginEdit()
        {
            _tempRefUnit = (RefUnit)this.MemberwiseClone();
        }

        public void CancelEdit()
        {
            if (null != _tempRefUnit)
                CopyFrom(_tempRefUnit);
            //_tempPatient = null;
        }

        public void EndEdit()
        {
        }

        public void CopyFrom(RefUnit p)
        {
            PropertyCopierHelper.CopyPropertiesTo(p, this);
        }

        #endregion

        #region Factory Method

     
        /// Create a new RefUnit object.
     
        /// <param name="unitID">Initial value of the UnitID property.</param>
        /// <param name="unitName">Initial value of the UnitName property.</param>
        /// <param name="unitActive">Initial value of the UnitActive property.</param>
        public static RefUnit CreateRefUnit(long unitID, String unitName, Boolean unitActive)
        {
            RefUnit refUnit = new RefUnit();
            refUnit.UnitID = unitID;
            refUnit.UnitName = unitName;
            refUnit.UnitActive = unitActive;
            return refUnit;
        }

        #endregion
        #region Primitive Properties

        [DataMemberAttribute()]
        public long UnitID
        {
            get
            {
                return _UnitID;
            }
            set
            {
                if (_UnitID != value)
                {
                    OnUnitIDChanging(value);
                    _UnitID = value;
                    OnUnitIDChanged();
                }
            }
        }
        private long _UnitID;
        partial void OnUnitIDChanging(long value);
        partial void OnUnitIDChanged();

        [Required(ErrorMessage = "Bạn phải nhập Mã ĐVT")]
        [DataMemberAttribute()]
        public String UnitCode
        {
            get
            {
                return _UnitCode;
            }
            set
            {
                OnUnitCodeChanging(value);
                ValidateProperty("UnitCode",value);
                _UnitCode = value;
                RaisePropertyChanged("UnitCode");
                OnUnitCodeChanged();
            }
        }
        private String _UnitCode;
        partial void OnUnitCodeChanging(String value);
        partial void OnUnitCodeChanged();

        [Required(ErrorMessage = "Bạn phải nhập Tên ĐVT")]
        [DataMemberAttribute()]
        public String UnitName
        {
            get
            {
                return _UnitName;
            }
            set
            {
                OnUnitNameChanging(value);
                ValidateProperty("UnitName",value);
                _UnitName = value;
                RaisePropertyChanged("UnitName");
                OnUnitNameChanged();
            }
        }
        private String _UnitName;
        partial void OnUnitNameChanging(String value);
        partial void OnUnitNameChanged();

        [DataMemberAttribute()]
        public Boolean UnitActive
        {
            get
            {
                return _UnitActive;
            }
            set
            {
                OnUnitActiveChanging(value);
                _UnitActive = value;
                OnUnitActiveChanged();
            }
        }
        private Boolean _UnitActive;
        partial void OnUnitActiveChanging(Boolean value);
        partial void OnUnitActiveChanged();


        [Required(ErrorMessage = "Nhập UnitVolume!")]
        [Range(1.0, 99999999999.0, ErrorMessage = "UnitVolume >=1")]
        [DataMemberAttribute()]
        public double UnitVolume
        {
            get
            {
                return _UnitVolume;
            }
            set
            {
                ValidateProperty("UnitVolume", value);
                _UnitVolume = value;
                RaisePropertyChanged("UnitVolume");
            }
        }
        private double _UnitVolume;


        #endregion

        public override bool Equals(object obj)
        {
            RefUnit seletedUnit = obj as RefUnit;
            if (seletedUnit == null)
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;

            return this.UnitID == seletedUnit.UnitID;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
