using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;
using System.ComponentModel.DataAnnotations; //validation data
namespace DataEntities
{
    public partial class VitalSign : EntityBase, IEditableObject
    {
        #region Factory Method

        /// Create a new VitalSign object.

        /// <param name="vSignID">Initial value of the VSignID property.</param>
        /// <param name="vSignName">Initial value of the VSignName property.</param>
        /// <param name="v_VSignDataType">Initial value of the V_VSignDataType property.</param>
        /// <param name="medUnit">Initial value of the MedUnit property.</param>
        public static VitalSign CreateVitalSign(Byte vSignID, String vSignName, Int64 v_VSignDataType, String medUnit)
        {
            VitalSign vitalSign = new VitalSign();
            vitalSign.VSignID = vSignID;
            vitalSign.VSignName = vSignName;
            vitalSign.V_VSignDataType = v_VSignDataType;
            vitalSign.MedUnit = medUnit;
            return vitalSign;
        }

        #endregion
        #region Primitive Properties

        [DataMemberAttribute()]
        public Byte VSignID
        {
            get
            {
                return _VSignID;
            }
            set
            {
                if (_VSignID != value)
                {
                    OnVSignIDChanging(value);
                    _VSignID = value;
                    RaisePropertyChanged("VSignID");
                    OnVSignIDChanged();
                }
            }
        }
        private Byte _VSignID;
        partial void OnVSignIDChanging(Byte value);
        partial void OnVSignIDChanged();

        /*[CustomValidation(typeof(VitalSign), "VSignName")]*/
        [Required(ErrorMessage = "Required field")]
        //[StringLength(80, ErrorMessage = "Cannot exceed 40")]
        //[Display(Name = "VitalSign Name", Description = "Name of VitalSign")]
        [DataMemberAttribute()]
        public String VSignName
        {
            get
            {
                return _VSignName;
            }
            set
            {
                if(_VSignName!=value)
                {
                    OnVSignNameChanging(value);
                    ValidateProperty("VSignName", value);
                    _VSignName = value;
                    RaisePropertyChanged("VSignName");
                    OnVSignNameChanged();
                }
            }
        }
        private String _VSignName;
        partial void OnVSignNameChanging(String value);
        partial void OnVSignNameChanged();

        [DataMemberAttribute()]
        public String VSignDescription
        {
            get
            {
                return _VSignDescription;
            }
            set
            {
                OnVSignDescriptionChanging(value);
                _VSignDescription = value;
                RaisePropertyChanged("VSignDescription");
                OnVSignDescriptionChanged();
            }
        }
        private String _VSignDescription;
        partial void OnVSignDescriptionChanging(String value);
        partial void OnVSignDescriptionChanged();

        [DataMemberAttribute()]
        public Nullable<Boolean> IsPrimaryVitalSigns
        {
            get
            {
                return _IsPrimaryVitalSigns;
            }
            set
            {
                OnIsPrimaryVitalSignsChanging(value);
                _IsPrimaryVitalSigns = value;
                RaisePropertyChanged("IsPrimaryVitalSigns");
                OnIsPrimaryVitalSignsChanged();
            }
        }
        private Nullable<Boolean> _IsPrimaryVitalSigns;
        partial void OnIsPrimaryVitalSignsChanging(Nullable<Boolean> value);
        partial void OnIsPrimaryVitalSignsChanged();

        [Required(ErrorMessage = "Required field")]
        //[Display(Name = "VitalSign DataType", Description = "Data Type of VitalSign")]
        [DataMemberAttribute()]
        public Int64 V_VSignDataType
        {
            get
            {
                return _V_VSignDataType;
            }
            set
            {
                OnV_VSignDataTypeChanging(value);
                _V_VSignDataType = value;
                RaisePropertyChanged("V_VSignDataType");
                OnV_VSignDataTypeChanged();
            }
        }
        private Int64 _V_VSignDataType;
        partial void OnV_VSignDataTypeChanging(Int64 value);
        partial void OnV_VSignDataTypeChanged();

        [Required(ErrorMessage = "Required field")]
        //[StringLength(36, ErrorMessage = "Cannot exceed 36 - defauli 'Unkonw'")]
        //[Display(Name = "Medical Unit", Description = "Medical Unit of VitalSign")]
        [DataMemberAttribute()]
        public String MedUnit
        {
            get
            {
                return _MedUnit;
            }
            set
            {
                OnMedUnitChanging(value);
                _MedUnit = value;
                RaisePropertyChanged("MedUnit");
                OnMedUnitChanged();
            }
        }
        private String _MedUnit;
        partial void OnMedUnitChanging(String value);
        partial void OnMedUnitChanged();

        #endregion

        #region Navigation Properties
        private ObservableCollection<PatientVitalSign> _PatientVitalSigns;
        [DataMemberAttribute()]
        public ObservableCollection<PatientVitalSign> PatientVitalSigns
        {
            get
            {
                return _PatientVitalSigns;
            }
            set
            {
                _PatientVitalSigns = value;
                RaisePropertyChanged("PatientVitalSigns");
            }
        }

     
        
     
        private Lookup _LookupVSignDataType;
        [DataMemberAttribute()]
        public Lookup LookupVSignDataType
        {
            get
            {
                return _LookupVSignDataType;
            }
            set
            {
                if(_LookupVSignDataType!=value)
                {
                    _LookupVSignDataType = value;
                    RaisePropertyChanged("LookupVSignDataType");
                }
            }
        }

        #endregion

        #region IEditableObject Members
        private VitalSign _tempVitalSign;
        public void BeginEdit()
        {
            _tempVitalSign = (VitalSign)this.MemberwiseClone();
        }

        public void CancelEdit()
        {
            if (null != _tempVitalSign)
                CopyFrom(_tempVitalSign);
        }

        public void EndEdit()
        {
        }

        public void CopyFrom(VitalSign p)
        {
            PropertyCopierHelper.CopyPropertiesTo(p, this);
        }

        #endregion

    }
}
