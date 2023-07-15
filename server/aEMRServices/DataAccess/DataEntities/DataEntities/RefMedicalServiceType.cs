using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;
using System.ComponentModel.DataAnnotations;

namespace DataEntities
{

    public partial class RefMedicalServiceType : NotifyChangedBase
    {

        #region Primitive Properties
        [DataMemberAttribute()]
        public long MedicalServiceTypeID
        {
            get
            {
                return _MedicalServiceTypeID;
            }
            set
            {
                if (_MedicalServiceTypeID != value)
                {
                    OnMedicalServiceTypeIDChanging(value);
                    _MedicalServiceTypeID = value;
                    RaisePropertyChanged("MedicalServiceTypeID");
                    OnMedicalServiceTypeIDChanged();
                }
            }
        }
        private long _MedicalServiceTypeID;
        partial void OnMedicalServiceTypeIDChanging(long value);
        partial void OnMedicalServiceTypeIDChanged();


        [DataMemberAttribute()]
        public Int64 MedicalServiceGroupID
        {
            get
            {
                return _MedicalServiceGroupID;
            }
            set
            {
                OnMedicalServiceGroupIDChanging(value);
                _MedicalServiceGroupID = value;
                RaisePropertyChanged("MedicalServiceGroupID");
                OnMedicalServiceGroupIDChanged();
            }
        }
        private Int64 _MedicalServiceGroupID;
        partial void OnMedicalServiceGroupIDChanging(Int64 value);
        partial void OnMedicalServiceGroupIDChanged();

        [DataMemberAttribute()]
        public RefMedicalServiceGroups ObjMedicalServiceGroupID
        {
            get
            {
                return _ObjMedicalServiceGroupID;
            }
            set
            {
                OnObjMedicalServiceGroupIDChanging(value);
                _ObjMedicalServiceGroupID = value;
                RaisePropertyChanged("ObjMedicalServiceGroupID");
                OnObjMedicalServiceGroupIDChanged();
            }
        }
        private RefMedicalServiceGroups _ObjMedicalServiceGroupID;
        partial void OnObjMedicalServiceGroupIDChanging(RefMedicalServiceGroups value);
        partial void OnObjMedicalServiceGroupIDChanged();


        [Required(ErrorMessage = "Nhập Mã Loại Dịch Vụ")]
        [StringLength(15, MinimumLength = 0, ErrorMessage = "Phải <= 15 Ký Tự")]
        [DataMemberAttribute()]
        public String MedicalServiceTypeCode
        {
            get
            {
                return _MedicalServiceTypeCode;
            }
            set
            {
                OnMedicalServiceTypeCodeChanging(value);
                ValidateProperty("MedicalServiceTypeCode", value);
                _MedicalServiceTypeCode = value;
                RaisePropertyChanged("MedicalServiceTypeCode");
                OnMedicalServiceTypeCodeChanged();
            }
        }
        private String _MedicalServiceTypeCode;
        partial void OnMedicalServiceTypeCodeChanging(String value);
        partial void OnMedicalServiceTypeCodeChanged();


        [Required(ErrorMessage = "Nhập Tên Loại Dịch Vụ")]
        [StringLength(125, MinimumLength = 0, ErrorMessage = "Phải <= 125 Ký Tự")]
        [DataMemberAttribute()]
        public String MedicalServiceTypeName
        {
            get
            {
                return _MedicalServiceTypeName;
            }
            set
            {
                OnMedicalServiceTypeNameChanging(value);
                ValidateProperty("MedicalServiceTypeName", value);
                _MedicalServiceTypeName = value;
                RaisePropertyChanged("MedicalServiceTypeName");
                OnMedicalServiceTypeNameChanged();
            }
        }
        private String _MedicalServiceTypeName;
        partial void OnMedicalServiceTypeNameChanging(String value);
        partial void OnMedicalServiceTypeNameChanged();





        [DataMemberAttribute()]
        public String MedicalServiceTypeDescription
        {
            get
            {
                return _MedicalServiceTypeDescription;
            }
            set
            {
                OnMedicalServiceTypeDescriptionChanging(value);
                ////ReportPropertyChanging("MedicalServiceTypeDescription");
                _MedicalServiceTypeDescription = value;
                RaisePropertyChanged("MedicalServiceTypeDescription");
                OnMedicalServiceTypeDescriptionChanged();
            }
        }
        private String _MedicalServiceTypeDescription;
        partial void OnMedicalServiceTypeDescriptionChanging(String value);
        partial void OnMedicalServiceTypeDescriptionChanged();

        //V_RefMedicalServiceInOutOthers
        [DataMemberAttribute()]
        public Int64 V_RefMedicalServiceInOutOthers
        {
            get
            {
                return _V_RefMedicalServiceInOutOthers;
            }
            set
            {
                OnV_RefMedicalServiceInOutOthersChanging(value);
                _V_RefMedicalServiceInOutOthers = value;
                RaisePropertyChanged("V_RefMedicalServiceInOutOthers");
                OnV_RefMedicalServiceInOutOthersChanged();
            }
        }
        private Int64 _V_RefMedicalServiceInOutOthers;
        partial void OnV_RefMedicalServiceInOutOthersChanging(Int64 value);
        partial void OnV_RefMedicalServiceInOutOthersChanged();

        [DataMemberAttribute()]
        public Lookup ObjV_RefMedicalServiceInOutOthers
        {
            get { return _ObjV_RefMedicalServiceInOutOthers; }
            set
            {
                OnObjV_RefMedicalServiceInOutOthersChanging(value);
                _ObjV_RefMedicalServiceInOutOthers = value;
                RaisePropertyChanged("ObjV_RefMedicalServiceInOutOthers");
                OnObjV_RefMedicalServiceInOutOthersChanged();
            }
        }
        private Lookup _ObjV_RefMedicalServiceInOutOthers;
        partial void OnObjV_RefMedicalServiceInOutOthersChanging(Lookup value);
        partial void OnObjV_RefMedicalServiceInOutOthersChanged();
        //V_RefMedicalServiceInOutOthers


        [DataMemberAttribute()]
        public Int64 V_RefMedicalServiceTypes
        {
            get
            {
                return _V_RefMedicalServiceTypes;
            }
            set
            {
                OnV_RefMedicalServiceTypesChanging(value);
                _V_RefMedicalServiceTypes = value;
                RaisePropertyChanged("V_RefMedicalServiceTypes");
                OnV_RefMedicalServiceTypesChanged();
            }
        }
        private Int64 _V_RefMedicalServiceTypes;
        partial void OnV_RefMedicalServiceTypesChanging(Int64 value);
        partial void OnV_RefMedicalServiceTypesChanged();

        [DataMemberAttribute()]
        public Lookup ObjV_RefMedicalServiceTypes
        {
            get { return _ObjV_RefMedicalServiceTypes; }
            set
            {
                OnObjV_RefMedicalServiceTypesChanging(value);
                _ObjV_RefMedicalServiceTypes = value;
                RaisePropertyChanged("ObjV_RefMedicalServiceTypes");
                OnObjV_RefMedicalServiceTypesChanged();
            }
        }
        private Lookup _ObjV_RefMedicalServiceTypes;
        partial void OnObjV_RefMedicalServiceTypesChanging(Lookup value);
        partial void OnObjV_RefMedicalServiceTypesChanged();

        [DataMemberAttribute()]
        public Nullable<bool> IsActive
        {
            get { return _IsActive; }
            set
            {
                OnIsActiveChanging(value);
                _IsActive = value;
                RaisePropertyChanged("IsActive");
                OnIsActiveChanged();
            }
        }
        private Nullable<bool> _IsActive;
        partial void OnIsActiveChanging(Nullable<bool> value);
        partial void OnIsActiveChanged();

        #endregion

        #region Navigation Properties

        //[DataMemberAttribute()]
        //// [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_REFMEDIC_REL_HOSFM_REFMEDIC", "RefMedicalServiceItems")]
        //public ObservableCollection<RefMedicalServiceItem> RefMedicalServiceItems
        //{
        //    get;
        //    set;
        //}
        #endregion

        public override bool Equals(object obj)
        {
            RefMedicalServiceType cond = obj as RefMedicalServiceType;
            if (cond == null)
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;

            return this._MedicalServiceTypeID == cond.MedicalServiceTypeID;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return MedicalServiceTypeName;
        }
    }
}