using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Text;

namespace DataEntities
{
    public partial class SupplierGenMedProductsPrice : NotifyChangedBase
    {

        [DataMemberAttribute()]
        public Int64 PKID
        {
            get { return _PKID; }
            set 
            {
                if (_PKID != value)
                {
                    OnPKIDChanging(value);
                    _PKID = value;
                    RaisePropertyChanged("PKID");
                    OnPKIDChanged();
                }
            }
        }
        private Int64 _PKID;
        partial void OnPKIDChanging(Int64 value);
        partial void OnPKIDChanged();

        [DataMemberAttribute()]        
        public Int64 SupplierID
        {
            get { return _SupplierID; }
            set 
            {
                if (_SupplierID != value)
                {
                    OnSupplierIDChanging(value);
                    _SupplierID = value;
                    RaisePropertyChanged("SupplierID");
                    OnSupplierIDChanged();
                }

            }
        }
        private Int64 _SupplierID;
        partial void OnSupplierIDChanging(Int64 value);
        partial void OnSupplierIDChanged();

        [DataMemberAttribute()]        
        public DateTime RecCreatedDate
        {
            get { return _RecCreatedDate; }
            set 
            {
                OnRecCreatedDateChanging(value);
                _RecCreatedDate = value;
                RaisePropertyChanged("RecCreatedDate");
                OnRecCreatedDateChanged();
            }
        }
        private DateTime _RecCreatedDate;
        partial void OnRecCreatedDateChanging(DateTime value);
        partial void OnRecCreatedDateChanged();
              
        [DataMemberAttribute()]        
        public Int64 StaffID
        {
            get { return _StaffID; }
            set {
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
            set {
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


        [Required(ErrorMessage = "Nhập Đơn Giá!")]
        [Range(1.0, 99999999999.0, ErrorMessage = "Đơn Giá Phải >=1")]        
        [DataMemberAttribute()]                
        public Decimal UnitPrice
        {
            get { return _UnitPrice; }
            set 
            {
                OnUnitPriceChanging(value);
	            ValidateProperty("UnitPrice", value);
                _UnitPrice = value;
                RaisePropertyChanged("UnitPrice");
                OnUnitPriceChanged();
            }
        }
        private Decimal _UnitPrice;
        partial void OnUnitPriceChanging(Decimal value);
        partial void OnUnitPriceChanged();

        [Required(ErrorMessage = "Nhập Đơn Giá!")]
        [Range(1.0, 99999999999.0, ErrorMessage = "Đơn Giá Phải >=1")]        
        [DataMemberAttribute()]        
        public Decimal PackagePrice
        {
            get { return _PackagePrice; }
            set 
            {               
                
                OnPackagePriceChanging(value);
                ValidateProperty("PackagePrice", value);
                _PackagePrice = value;
                RaisePropertyChanged("PackagePrice");
                OnPackagePriceChanged();
                
            }
        }
        private Decimal _PackagePrice;
        partial void OnPackagePriceChanging(Decimal value);
        partial void OnPackagePriceChanged();

        
        [Required(ErrorMessage = "Nhập VAT!")]
        [Range(0.0, 99999999999.0, ErrorMessage = "Không Được < 0")]
        [DataMemberAttribute()]
        public Nullable<Double> VAT
        {
            get { return _VAT; }
            set
            {
                OnVATChanging(value);
                ValidateProperty("VAT", value);
                _VAT = value;
                RaisePropertyChanged("VAT");
                OnVATChanged();
            }
        }
        private Nullable<Double> _VAT;
        partial void OnVATChanging(Nullable<Double> value);
        partial void OnVATChanged();


        [DataMemberAttribute()]
        public Boolean IsActive
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
        private Boolean _IsActive;
        partial void OnIsActiveChanging(Boolean value);
        partial void OnIsActiveChanged();


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
        public Boolean IsDeleted
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
        private Boolean _IsDeleted;
        partial void OnIsDeletedChanging(Boolean value);
        partial void OnIsDeletedChanged();
        
        #region Navigate        

        [DataMemberAttribute()]
        private RefGenMedProductDetails _ObjRefGenMedProductDetails;
        public RefGenMedProductDetails ObjRefGenMedProductDetails
        {
            get { return _ObjRefGenMedProductDetails; }
            set 
            {
                if (_ObjRefGenMedProductDetails != value)
                {
                    OnObjRefGenMedProductDetailsChanging(value);
                    _ObjRefGenMedProductDetails = value;
                    RaisePropertyChanged("ObjRefGenMedProductDetails");
                    OnObjRefGenMedProductDetailsChanged();
                }
            }
        }
        partial void OnObjRefGenMedProductDetailsChanging(RefGenMedProductDetails value);
        partial void OnObjRefGenMedProductDetailsChanged();
    

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


        #endregion


      
    }
}
