using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;
using System.ComponentModel.DataAnnotations;

namespace DataEntities
{
    public partial class RefGenMedProductSellingPrices : NotifyChangedBase
    {
        [DataMemberAttribute()]
        public Int64 GenMedSellPriceID
        {
            get
            {
                return _GenMedSellPriceID;
            }
            set
            {
                if (_GenMedSellPriceID != value)
                {
                    OnGenMedSellPriceIDChanging(value);
                    _GenMedSellPriceID = value;
                    RaisePropertyChanged("GenMedSellPriceID");
                    OnGenMedSellPriceIDChanged();
                }
            }
        }
        private Int64 _GenMedSellPriceID;
        partial void OnGenMedSellPriceIDChanging(Int64 value);
        partial void OnGenMedSellPriceIDChanged();


        [DataMemberAttribute()]
        public Int64 GenMedProductID
        {
            get { return _GenMedProductID; }
            set
            {
                if (_GenMedProductID != value)
                {
                    OnGenMedProductIDChanging(value);
                    _GenMedProductID = value;
                    RaisePropertyChanged("GenMedProductID");
                    OnGenMedProductIDChanged();
                }
            }
        }
        private Int64 _GenMedProductID;
        partial void OnGenMedProductIDChanging(Int64 value);
        partial void OnGenMedProductIDChanged();


        [DataMemberAttribute()]
        public DateTime RecDateCreated
        {
            get { return _RecDateCreated; }
            set
            {
                if (_RecDateCreated != value)
                {
                    OnRecDateCreatedChanging(value);
                    _RecDateCreated = value;
                    RaisePropertyChanged("RecDateCreated");
                    OnRecDateCreatedChanged();
                }
            }
        }
        private DateTime _RecDateCreated;
        partial void OnRecDateCreatedChanging(DateTime value);
        partial void OnRecDateCreatedChanged();


        [DataMemberAttribute()]
        public Int64 StaffID
        {
            get { return _StaffID; }
            set
            {
                if (_StaffID != value)
                {
                    OnStaffIDChanging(value);
                    _StaffID = value;
                    RaisePropertyChanged("StaffID");
                    OnStaffIDChanged();
                }
            }
        }
        private Int64 _StaffID;
        partial void OnStaffIDChanging(Int64 value);
        partial void OnStaffIDChanged();


        [DataMemberAttribute()]
        public Nullable<Int64> ApprovedStaffID
        {
            get { return _ApprovedStaffID; }
            set
            {
                if (_ApprovedStaffID != value)
                {
                    OnApprovedStaffIDChanging(value);
                    _ApprovedStaffID = value;
                    RaisePropertyChanged("ApprovedStaffID");
                    OnApprovedStaffIDChanged();
                }
            }
        }
        private Nullable<Int64> _ApprovedStaffID;
        partial void OnApprovedStaffIDChanging(Nullable<Int64> value);
        partial void OnApprovedStaffIDChanged();


        [Range(0.0, 99999999999.0, ErrorMessage = "Phải >=0")]
        [DataMemberAttribute()]
        public Nullable<double> VATRate
        {
            get { return _VATRate; }
            set
            {
                OnVATRateChanging(value);
                ValidateProperty("VATRate", value);
                _VATRate = value;
                RaisePropertyChanged("VATRate");
                OnVATRateChanged();
            }
        }
        private Nullable<double> _VATRate;
        partial void OnVATRateChanging(Nullable<double> value);
        partial void OnVATRateChanged();


        [Range(1.0, 99999999999.0, ErrorMessage = "Đơn Giá Phải >=1")]
        [Required(ErrorMessage = "Nhập Đơn Giá!")]
        [DataMemberAttribute()]
        public decimal NormalPrice
        {
            get { return _NormalPrice; }
            set
            {
                OnNormalPriceChanging(value);
                ValidateProperty("NormalPrice", value);
                _NormalPrice = value;
                RaisePropertyChanged("NormalPrice");
                OnNormalPriceChanged();
            }
        }
        private decimal _NormalPrice;
        partial void OnNormalPriceChanging(decimal value);
        partial void OnNormalPriceChanged();


        [Range(0, 99999999999.0, ErrorMessage = "Phải >= 0")]
        [DataMemberAttribute()]
        public Nullable<decimal> PriceForHIPatient
        {
            get { return _PriceForHIPatient; }
            set
            {
                OnPriceForHIPatientChanging(value);
                ValidateProperty("PriceForHIPatient", value);
                _PriceForHIPatient = value;
                RaisePropertyChanged("PriceForHIPatient");
                OnPriceForHIPatientChanged();
            }
        }
        private Nullable<decimal> _PriceForHIPatient;
        partial void OnPriceForHIPatientChanging(Nullable<decimal> value);
        partial void OnPriceForHIPatientChanged();

        [Range(0, 99999999999.0, ErrorMessage = "Không Được < 0")]
        [DataMemberAttribute()]
        public Nullable<decimal> HIAllowedPrice
        {
            get { return _HIAllowedPrice; }
            set
            {
                OnHIAllowedPriceChanging(value);
                ValidateProperty("HIAllowedPrice", value);
                _HIAllowedPrice = value;
                RaisePropertyChanged("HIAllowedPrice");
                OnHIAllowedPriceChanged();
            }
        }
        private Nullable<decimal> _HIAllowedPrice;
        partial void OnHIAllowedPriceChanging(Nullable<decimal> value);
        partial void OnHIAllowedPriceChanged();


        [Required(ErrorMessage = "Nhập Ngày Áp Dụng!")]
        [DataMemberAttribute()]
        public Nullable<DateTime> EffectiveDate
        {
            get { return _EffectiveDate; }
            set
            {
                OnEffectiveDateChanging(value);
                ValidateProperty("EffectiveDate", value);
                _EffectiveDate = value;
                RaisePropertyChanged("EffectiveDate");
                OnEffectiveDateChanged();
            }
        }
        private Nullable<DateTime> _EffectiveDate;
        partial void OnEffectiveDateChanging(Nullable<DateTime> value);
        partial void OnEffectiveDateChanged();


        [DataMemberAttribute()]
        public bool IsActive
        {
            get { return _IsActive; }
            set
            {
                if (_IsActive != value)
                {
                    OnIsActiveChanging(value);
                    _IsActive = value;
                    RaisePropertyChanged("IsActive");
                    OnIsActiveChanged();
                }
            }
        }
        private bool _IsActive;
        partial void OnIsActiveChanging(bool value);
        partial void OnIsActiveChanged();


        [DataMemberAttribute()]
        public bool IsDeleted
        {
            get { return _IsDeleted; }
            set
            {
                if (_IsDeleted != value)
                {
                    OnIsDeletedChanging(value);
                    _IsDeleted = value;
                    RaisePropertyChanged("IsDeleted");
                    OnIsDeletedChanged();
                }
            }
        }
        private bool _IsDeleted;
        partial void OnIsDeletedChanging(bool value);
        partial void OnIsDeletedChanged();


        [DataMemberAttribute()]
        public string Remark
        {
            get { return _Remark; }
            set
            {
                if (_Remark != value)
                {
                    OnRemarkChanging(value);
                    _Remark = value;
                    RaisePropertyChanged("Remark");
                    OnRemarkChanged();
                }
            }
        }
        private string _Remark;
        partial void OnRemarkChanging(string value);
        partial void OnRemarkChanged();


        #region Navigate

        [DataMemberAttribute()]
        public RefGenMedProductDetails ObjGenMedProductID
        {
            get { return _ObjGenMedProductID; }
            set
            {
                if (_ObjGenMedProductID != value)
                {
                    OnObjGenMedProductIDChanging(value);
                    _ObjGenMedProductID = value;
                    RaisePropertyChanged("ObjGenMedProductID");
                    OnObjGenMedProductIDChanged();
                }
            }
        }
        private RefGenMedProductDetails _ObjGenMedProductID;
        partial void OnObjGenMedProductIDChanging(RefGenMedProductDetails value);
        partial void OnObjGenMedProductIDChanged();

        [DataMemberAttribute()]
        public Staff ObjStaffID
        {
            get { return _ObjStaffID; }
            set
            {
                if (_ObjStaffID != value)
                {
                    OnObjStaffIDChanging(value);
                    _ObjStaffID = value;
                    RaisePropertyChanged("ObjStaffID");
                    OnObjStaffIDChanged();
                }
            }
        }
        private Staff _ObjStaffID;
        partial void OnObjStaffIDChanging(Staff value);
        partial void OnObjStaffIDChanged();


        [DataMemberAttribute()]
        public Staff ObjApprovedStaffID
        {
            get { return _ObjApprovedStaffID; }
            set
            {
                if (_ObjApprovedStaffID != value)
                {
                    OnObjApprovedStaffIDChanging(value);
                    _ObjApprovedStaffID = value;
                    RaisePropertyChanged("ObjApprovedStaffID");
                    OnObjApprovedStaffIDChanged();
                }
            }
        }
        private Staff _ObjApprovedStaffID;
        partial void OnObjApprovedStaffIDChanging(Staff value);
        partial void OnObjApprovedStaffIDChanged();


        [DataMemberAttribute()]
        public Nullable<bool> CanEdit
        {
            get { return _CanEdit; }
            set
            {
                if (_CanEdit != value)
                {
                    OnCanEditChanging(value);
                    _CanEdit = value;
                    RaisePropertyChanged("CanEdit");
                    OnCanEditChanged();
                }
            }
        }
        private Nullable<bool> _CanEdit;
        partial void OnCanEditChanging(Nullable<bool> value);
        partial void OnCanEditChanged();


        [DataMemberAttribute()]
        public Nullable<bool> CanDelete
        {
            get { return _CanDelete; }
            set
            {
                if (_CanDelete != value)
                {
                    OnCanDeleteChanging(value);
                    _CanDelete = value;
                    RaisePropertyChanged("CanDelete");
                    OnCanDeleteChanged();
                }
            }
        }
        private Nullable<bool> _CanDelete;
        partial void OnCanDeleteChanging(Nullable<bool> value);
        partial void OnCanDeleteChanged();
        

        [DataMemberAttribute()]
        public string PriceType
        {
            get { return _PriceType; }
            set
            {
                if (_PriceType != value)
                {
                    OnPriceTypeChanging(value);
                    _PriceType = value;
                    RaisePropertyChanged("PriceType");
                    OnPriceTypeChanged();
                }
            }
        }
        private string _PriceType;
        partial void OnPriceTypeChanging(string value);
        partial void OnPriceTypeChanged();
        
        
        //Thuộc tính ext
        [DataMemberAttribute()]
        public string lbTitleInfo
        {
            get { return _lbTitleInfo; }
            set 
            {
                if (_lbTitleInfo != value)
                {
                    OnlbTitleInfoChanging(value);
                    _lbTitleInfo = value;
                    RaisePropertyChanged("lbTitleInfo");
                    OnlbTitleInfoChanged();
                }
            }
        }
        private string _lbTitleInfo;
        partial void OnlbTitleInfoChanging(string value);
        partial void OnlbTitleInfoChanged();


        [DataMemberAttribute()]
        public string lbDrugClassName
        {
            get { return _lbDrugClassName; }
            set 
            {
                if (_lbDrugClassName != value)
                {
                    OnlbDrugClassNameChanging(value);
                    _lbDrugClassName = value;
                    RaisePropertyChanged("lbDrugClassName");
                    OnlbDrugClassNameChanged();
                }
            }
        }
        private string _lbDrugClassName;
        partial void OnlbDrugClassNameChanging(string value);
        partial void OnlbDrugClassNameChanged();

        [DataMemberAttribute()]
        public string DrugClassIDName
        {
            get { return _DrugClassIDName; }
            set 
            {
                if (_DrugClassIDName != value)
                {
                    OnDrugClassIDNameChanging(value);
                    _DrugClassIDName = value;
                    RaisePropertyChanged("DrugClassIDName");
                    OnDrugClassIDNameChanged();
                }
            }
        }
        private string _DrugClassIDName;
        partial void OnDrugClassIDNameChanging(string value);
        partial void OnDrugClassIDNameChanged();      
        


        [DataMemberAttribute()]
        public string CountryName
        {
            get { return _CountryName; }
            set 
            {
                if (_CountryName != value)
                {
                    OnCountryNameChanging(value);
                    _CountryName = value;
                    RaisePropertyChanged("CountryName");
                    OnCountryNameChanged();
                }
            }
        }
        private string _CountryName;
        partial void OnCountryNameChanging(string value);
        partial void OnCountryNameChanged();


        private string _UnitIDName;
        public string UnitIDName
        {
            get { return _UnitIDName; }
            set 
            {
                if (_UnitIDName != value)
                {
                    OnUnitIDNameChanging(value);
                    _UnitIDName = value;
                    RaisePropertyChanged("UnitIDName");
                    OnUnitIDNameChanged();
                }
            }
        }
        partial void OnUnitIDNameChanging(string value);
        partial void OnUnitIDNameChanged();


        private string _UnitUseIDName;
        public string UnitUseIDName
        {
            get { return _UnitUseIDName; }
            set 
            {
                if (_UnitUseIDName != value)
                {
                    OnUnitUseIDNameChanging(value);
                    _UnitUseIDName = value;
                    RaisePropertyChanged("UnitUseIDName");
                    OnUnitUseIDNameChanged();
                }
            }
        }
        partial void OnUnitUseIDNameChanging(string value);
        partial void OnUnitUseIDNameChanged();

        //Thuộc tính ext

        
        #endregion



    }
}
