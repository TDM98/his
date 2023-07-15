using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;
/*
 * 20170524 #001 CMN: Added Attribute for Adding more item on HT Bill
 * 20180319 #002 CMN: Added method to change dose string format to does float value
 * 20180412 #003 TTM: Added Volume double value. 
 * 20180505 #004 TBLD: Added XML for Service.
 * 20180801 #005 TTM: Add new Property V_CatDrugType & DrugCode
 * 20181101 #006 TBL: Added GenericID, SelectedGeneric
 * 20210803 #007 TNHX: Added NgoaiDinhSuat
 * 20220609 #008 BLQ: Thêm ghi chú trong danh mục thuốc
 * 20220729 #009 BLQ: Thêm trường phiếu nhập cho hàng ký gửi
 * 20230322 #010 QTD: Thêm dữ liệu 130
*/
namespace DataEntities
{
    [DataContract]
    public partial class RefGenMedProductDetails : NotifyChangedBase, IChargeableItemPrice, IDosage
    {
        #region Factory Method


        /// Create a new RefGenericDrugDetail object.

        /// <param name="drugID">Initial value of the DrugID property.</param>
        /// <param name="brandName">Initial value of the BrandName property.</param>
        /// <param name="genericName">Initial value of the GenericName property.</param>
        public static RefGenMedProductDetails CreateRefGenericDrugDetail(long drugID, String brandName, String genericName)
        {
            RefGenMedProductDetails refGenericDrugDetail = new RefGenMedProductDetails();
            refGenericDrugDetail.GenMedProductID = drugID;
            refGenericDrugDetail.BrandName = brandName;
            refGenericDrugDetail.GenericName = genericName;
            return refGenericDrugDetail;
        }

        #endregion

        [DataMemberAttribute()]
        public Lookup CatDrugType
        {
            get
            {
                return _CatDrugType;
            }
            set
            {
                _CatDrugType = value;
                RaisePropertyChanged("CatDrugType");
            }
        }
        private Lookup _CatDrugType;

        private Int64 _GenMedProductID;
        [DataMemberAttribute()]
        public Int64 GenMedProductID
        {
            get 
            { 
                return _GenMedProductID; 
            }
            set 
            {
                if (_GenMedProductID != value)
                {
                    OnGenMedProductIDChanging(value);
                    _GenMedProductID = value;
                    RaisePropertyChanged("GenMedProductID");
                }
            }
        }
        partial void OnGenMedProductIDChanging(Int64 value);
        partial void OnGenMedProductIDChanged();

        [Required(ErrorMessage = "Nhập Tên Thương Mại!")]
        [StringLength(512, ErrorMessage = "Tên Thương Mại Phải <= 512 Ký Tự")]
        [DataMemberAttribute()]
        public String BrandName
        {
            get
            {
                return _BrandName;
            }
            set
            {
                if (_BrandName != value)
                {
                    OnBrandNameChanging(value);
                    ValidateProperty("BrandName", value);
                    _BrandName = value;
                    RaisePropertyChanged("BrandName");
                    OnBrandNameChanged();
                }
            }
        }
        private String _BrandName;
        partial void OnBrandNameChanging(String value);
        partial void OnBrandNameChanged();

        //[Required(ErrorMessage = "Nhập Tên Chung!")]
        [DataMemberAttribute()]
        public String GenericName
        {
            get
            {
                return _GenericName;
            }
            set
            {
                OnGenericNameChanging(value);
                ValidateProperty("GenericName", value);
                _GenericName = value;
                RaisePropertyChanged("GenericName");
                OnGenericNameChanged();
            }
        }
        private String _GenericName;
        partial void OnGenericNameChanging(String value);
        partial void OnGenericNameChanged();
        
        [DataMemberAttribute()]
        public String Code
        {
            get
            {
                return _Code;
            }
            set
            {
                OnCodeChanging(value);
                _Code = value;
                RaisePropertyChanged("Code");
                OnCodeChanged();
            }
        }
        private String _Code;
        partial void OnCodeChanging(String value);
        partial void OnCodeChanged();               

        [DataMemberAttribute()]        
        public string HICode
        {
            get 
            { 
                return _HICode; 
            }
            set 
            {
                if (_HICode != value)
                {
                    OnHICodeChanging(value);
                    _HICode = value;
                    RaisePropertyChanged("HICode");
                    OnHICodeChanged();
                }
            }
        }
        private string _HICode;
        partial void OnHICodeChanging(String value);
        partial void OnHICodeChanged();

        [DataMemberAttribute()]        
        public string Functions
        {
            get 
            {
                return _Functions; 
            }
            set 
            {
                if (_Functions != value)
                {
                    OnFunctionsChanging(value);
                    _Functions = value;
                    RaisePropertyChanged("Functions");
                    OnFunctionsChanged();
                }
            }
        }
        private string _Functions;
        partial void OnFunctionsChanging(String value);
        partial void OnFunctionsChanged();
        
        [DataMemberAttribute()]        
        public string TechInfo
        {
            get 
            { 
                return _TechInfo; 
            }
            set 
            {
                if (_TechInfo != value)
                {
                    OnTechInfoChanging(value);
                    _TechInfo = value;
                    RaisePropertyChanged("TechInfo");
                    OnTechInfoChanged();
                }
            }
        }
        private string _TechInfo;
        partial void OnTechInfoChanging(String value);
        partial void OnTechInfoChanged();

        [DataMemberAttribute()]        
        public string Material
        {
            get 
            { 
                return _Material; 
            }
            set 
            {
                if (_Material != value)
                {
                    OnMaterialChanging(value);
                    _Material = value;
                    RaisePropertyChanged("Material");
                    OnMaterialChanged();
                }
            }
        }
        private string _Material;
        partial void OnMaterialChanging(String value);
        partial void OnMaterialChanged();

        [DataMemberAttribute()]        
        public string Visa
        {
            get 
            { 
                return _Visa; 
            }
            set 
            {
                if (_Visa != value)
                {
                    OnVisaChanging(value);
                    _Visa = value;
                    RaisePropertyChanged("Visa");
                    OnVisaChanged();
                }
            }
        }
        private string _Visa;
        partial void OnVisaChanging(String value);
        partial void OnVisaChanged();

        [DataMemberAttribute()]
        public Nullable<long> PCOID
        {
            get
            {
                return _PCOID;
            }
            set
            {
                _PCOID = value;
                RaisePropertyChanged("PCOID");
            }
        }
        private Nullable<long> _PCOID;


        //[Required(ErrorMessage = "Chọn Quốc Gia!")]
        [DataMemberAttribute()]        
        public Int64 CountryID
        {
            get 
            { 
                return _CountryID; 
            }
            set 
            {
                OnCountryIDChanging(value);
                //ValidateProperty("CountryID",value);
                _CountryID = value;
                RaisePropertyChanged("CountryID");
                OnCountryIDChanged();                
            }
        }
        private Int64 _CountryID;
        partial void OnCountryIDChanging(Int64 value);
        partial void OnCountryIDChanged();


        [DataMemberAttribute()]        
        public string Packaging
        {
            get 
            { 
                return _Packaging; 
            }
            set 
            {
                if (_Packaging != value)
                {
                    OnPackagingChanging(value);
                    _Packaging = value;
                    RaisePropertyChanged("Packaging");
                    OnPackagingChanged();
                }
            }
        }
        private string _Packaging;
        partial void OnPackagingChanging(String value);
        partial void OnPackagingChanged();
        
        [DataMemberAttribute()]        
        public Nullable<Int64> UnitID
        {
            get 
            { 
                return _UnitID; 
            }
            set 
            {                
                OnUnitIDChanging(value);
                ValidateProperty("UnitID", value);
                _UnitID = value;
                RaisePropertyChanged("UnitID");
                OnUnitIDChanged();                
            }
        }
        private Nullable<Int64> _UnitID;
        partial void OnUnitIDChanging(Nullable<Int64> value);
        partial void OnUnitIDChanged();


        //[Required(ErrorMessage = "Đơn Vị Dùng!")]
        [DataMemberAttribute()]        
        public Nullable<Int64> UnitUseID
        {
            get 
            { 
                return _UnitUseID; 
            }
            set 
            {                
                OnUnitUseIDChanging(value);
                //ValidateProperty("UnitUseID",value);
                _UnitUseID = value;
                RaisePropertyChanged("UnitUseID");
                OnUnitUseIDChanged();                
            }
        }
        private Nullable<Int64> _UnitUseID;
        partial void OnUnitUseIDChanging(Nullable<Int64> value);
        partial void OnUnitUseIDChanged();


        [Required(ErrorMessage = "Nhập Số Lượng Quy Cách!")]
        [Range(1, 99999999, ErrorMessage = "Số Lượng Quy Cách Phải >=1!")]
        [DataMemberAttribute()]        
        public Nullable<int> UnitPackaging
        {
            get 
            { 
                return _UnitPackaging; 
            }
            set 
            {
                OnUnitPackagingChanging(value);
                ValidateProperty("UnitPackaging", value);
                _UnitPackaging = value;
                RaisePropertyChanged("UnitPackaging");
                OnUnitPackagingChanged();
            }
        }
        private Nullable<int> _UnitPackaging;
        partial void OnUnitPackagingChanging(Nullable<int> value);
        partial void OnUnitPackagingChanged();

        //Hàm kiểm tra
        public static ValidationResult ValidCustom_IntGreaterThanEqual0(object objValue, ValidationContext context)
        {
            if (objValue != null && objValue != DBNull.Value)
            {
                if (IsNumeric(objValue) == false)
                {
                    return new ValidationResult("Vui Lòng Nhập Số!", new string[] { "UnitPackaging" });
                }
                else
                {
                    if (Convert.ToInt64(objValue) < 0)
                    {
                        return new ValidationResult("Phải >= 0!", new string[] { "UnitPackaging"});
                    }
                }
            }
            return ValidationResult.Success;
        }
        private static bool IsNumeric(object Expression)
        {
            bool isNum;
            double retNum;
            isNum = Double.TryParse(Convert.ToString(Expression), System.Globalization.NumberStyles.Any, System.Globalization.NumberFormatInfo.InvariantInfo, out retNum);
            return isNum;
        }
        //Hàm kiểm tra

        [DataMemberAttribute()]        
        public string Description
        {
            get 
            { 
                return _Description; 
            }
            set 
            {
                if (_Description != value)
                {
                    OnDescriptionChanging(value);
                    _Description = value;
                    RaisePropertyChanged("Description");
                    OnDescriptionChanged();
                }
            }
        }
        private string _Description;
        partial void OnDescriptionChanging(string value);
        partial void OnDescriptionChanged();

        
        [Required(ErrorMessage = "Nhập Cách Dùng!")]
        [DataMemberAttribute()]        
        public string Administration
        {
            get 
            { 
                return _Administration; 
            }
            set 
            {
                
                    OnAdministrationChanging(value);
                    ValidateProperty("Administration", value);
                    _Administration = value;
                    RaisePropertyChanged("Administration");
                    OnAdministrationChanged();
                
            }
        }
        private string _Administration;
        partial void OnAdministrationChanging(string value);
        partial void OnAdministrationChanged();

        private RefStorageWarehouseLocation _storage;
        [DataMemberAttribute()]
        public RefStorageWarehouseLocation Storage
        {
            get
            {
                return _storage;
            }
            set
            {
                _storage = value;
                RaisePropertyChanged("Storage");
            }
        }
        private long? _storeID;
        [DataMemberAttribute()]
        public long? StoreID
        {
            get
            {
                return _storeID;
            }
            set
            {
                _storeID = value;
                RaisePropertyChanged("StoreID");
            }
        }

        private string _storeName;
        [DataMemberAttribute()]       
        public string StoreName
        {
            get
            {
                return _storeName;
            }
            set
            {
                _storeName = value;
                RaisePropertyChanged("StoreName");
            }
        }
        
        [DataMemberAttribute()]        
        public Nullable<bool> InsuranceCover
        {
            get 
            { 
                return _InsuranceCover; 
            }
            set 
            {
                if (_InsuranceCover != value)
                {
                    OnInsuranceCoverChanging(value);
                    _InsuranceCover = value;
                    RaisePropertyChanged("InsuranceCover");
                    OnInsuranceCoverChanged();
                }
            }
        }
        private Nullable<bool> _InsuranceCover;
        partial void OnInsuranceCoverChanging(Nullable<bool> value);
        partial void OnInsuranceCoverChanged();

        
        [DataMemberAttribute()]        
        public Nullable<bool> IsActive
        {
            get 
            { 
                return _IsActive; 
            }
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
        private Nullable<bool> _IsActive;
        partial void OnIsActiveChanging(Nullable<bool> value);
        partial void OnIsActiveChanged();


        [DataMemberAttribute()]        
        public long V_MedProductType
        {
            get 
            {
                return _V_MedProductType; 
            }
            set 
            {
                OnV_MedProductTypeChanging(value);                
                _V_MedProductType = value;
                RaisePropertyChanged("V_MedProductType");
                OnV_MedProductTypeChanged();
            }
        }
        private long _V_MedProductType;
        partial void OnV_MedProductTypeChanging(long value);
        partial void OnV_MedProductTypeChanged();


        //[Required(ErrorMessage = "Nhập Hệ Số Nhân!")]
        //[Range(1, 99999999, ErrorMessage = "Hệ Số Nhân Phải >=1!")]
        [DataMemberAttribute()]        
        public double NumberOfEstimatedMonths_F
        {
            get
            {
                return _NumberOfEstimatedMonths_F;
            }
            set
            {
                OnNumberOfEstimatedMonths_FChanging(value);
                //ValidateProperty("NumberOfEstimatedMonths_F", value);
                _NumberOfEstimatedMonths_F = value;
                RaisePropertyChanged("NumberOfEstimatedMonths_F");
                OnNumberOfEstimatedMonths_FChanged();
            }
        }
        private double _NumberOfEstimatedMonths_F;
        partial void OnNumberOfEstimatedMonths_FChanging(double value);
        partial void OnNumberOfEstimatedMonths_FChanged();

        [Required(ErrorMessage = "Bạn phải nhập Hệ Số Nhân!")]
        [Range(1, 9999999, ErrorMessage = "Hệ Số Nhân phải > 1")]

        private int _FactorSafety;
        [DataMemberAttribute()]
        public int FactorSafety
        {
            get
            {
                return _FactorSafety;
            }
            set
            {
                ValidateProperty("FactorSafety", value);
                _FactorSafety = value;
                RaisePropertyChanged("FactorSafety");
            }
        }

        
        private string _ActiveIngredientCode;
        [DataMemberAttribute()]
        public string ActiveIngredientCode
        {
            get
            {
                return _ActiveIngredientCode;
            }
            set
            {
                _ActiveIngredientCode = value;
                RaisePropertyChanged("ActiveIngredientCode");
            }
        }

        
        private string _ProductCodeRefNum;
        [DataMemberAttribute()]
        public string ProductCodeRefNum
        {
            get
            {
                return _ProductCodeRefNum;
            }
            set
            {
                _ProductCodeRefNum = value;
                RaisePropertyChanged("ProductCodeRefNum");
            }
        }

        private string _ShelfName;
        [DataMemberAttribute()]
        public string ShelfName
        {
            get
            {
                return _ShelfName;
            }
            set
            {
                _ShelfName = value;
                RaisePropertyChanged("ShelfName");
            }
        }
        
        private long? _HosID;
        [DataMemberAttribute()]
        public long? HosID
        {
            get
            {
                return _HosID;
            }
            set
            {
                _HosID = value;
                RaisePropertyChanged("HosID");
            }
        }

        private string _HIReportGroupCode;
        [DataMemberAttribute()]
        public string HIReportGroupCode
        {
            get
            {
                return _HIReportGroupCode;
            }
            set
            {
                _HIReportGroupCode = value;
                RaisePropertyChanged("HIReportGroupCode");
            }
        }
        
        private long? _RefGenDrugCatID_1 = 0;
        [DataMemberAttribute()]
        public long? RefGenDrugCatID_1
        {
            get
            {
                return _RefGenDrugCatID_1;
            }
            set
            {
                _RefGenDrugCatID_1 = value;
                RaisePropertyChanged("RefGenDrugCatID_1");
            }
        }

        private long? _RefGenDrugCatID_2 = 0;
        [DataMemberAttribute()]
        public long? RefGenDrugCatID_2
        {
            get
            {
                return _RefGenDrugCatID_2;
            }
            set
            {
                _RefGenDrugCatID_2 = value;
                RaisePropertyChanged("RefGenDrugCatID_2");
            }
        }


        [Required(ErrorMessage = "Nhập DispenseVolume!")]
        [Range(0, 99999999999.0, ErrorMessage = "DispenseVolume > 0")]
        [DataMemberAttribute()]
        public double DispenseVolume
        {
            get
            {
                return _DispenseVolume;
            }
            set
            {
                ValidateProperty("DispenseVolume", value);
                _DispenseVolume = value;
                RaisePropertyChanged("DispenseVolume");
            }
        }
        private double _DispenseVolume = 1.0;

        /*==== #003 ====*/
        [DataMemberAttribute()]
        public double? Volume
        {
            get
            {
                return _Volume;
            }
            set
            {
                ValidateProperty("Volume", value);
                _Volume = value;
                RaisePropertyChanged("Volume");
            }
        }
        private double? _Volume = null;

        private long _StaffID;
        [DataMemberAttribute()]
        public long StaffID
        {
            get
            {
                return _StaffID;
            }
            set
            {
                _StaffID = value;
                RaisePropertyChanged("StaffID");
            }
        }

        //[Required(ErrorMessage = "Chọn Lớp Thuốc!")]
        [DataMemberAttribute()]
        public Nullable<Int64> DrugClassID
        {
            get
            {
                return _DrugClassID;
            }
            set
            {
                OnDrugClassIDChanging(value);
                //ValidateProperty("DrugClassID", value);
                _DrugClassID = value;
                RaisePropertyChanged("DrugClassID");
                OnDrugClassIDChanged();
            }
        }
        private Nullable<Int64> _DrugClassID;
        partial void OnDrugClassIDChanging(Nullable<Int64> value);
        partial void OnDrugClassIDChanged();

        [DataMemberAttribute()]
        public string DrugClassName
        {
            get
            {
                return _DrugClassName;
            }
            set
            {
                _DrugClassName = value;
                RaisePropertyChanged("DrugClassName");
            }
        }
        private string _DrugClassName;


        [DataMemberAttribute()]
        public string MDoseStr
        {
            get
            {
                return _MDoseStr;
            }
            set
            {
                if (_MDoseStr != value)
                {
                    _MDoseStr = value;
                    RaisePropertyChanged("MDoseStr");
                }
            }
        }
        private string _MDoseStr = "0";


        [DataMemberAttribute()]
        public string ADoseStr
        {
            get
            {
                return _ADoseStr;
            }
            set
            {
                if (_ADoseStr != value)
                {
                    _ADoseStr = value;
                    RaisePropertyChanged("ADoseStr");
                }
            }
        }
        private string _ADoseStr = "0";

        [DataMemberAttribute()]
        public string EDoseStr
        {
            get
            {
                return _EDoseStr;
            }
            set
            {
                if (_EDoseStr != value)
                {
                    _EDoseStr = value;
                    RaisePropertyChanged("EDoseStr");
                }
            }
        }
        private string _EDoseStr = "0";

        [DataMemberAttribute()]
        public string NDoseStr
        {
            get
            {
                return _NDoseStr;
            }
            set
            {
                if (_NDoseStr != value)
                {
                    _NDoseStr = value;
                    RaisePropertyChanged("NDoseStr");
                }
            }
        }
        private string _NDoseStr = "0";


        [DataMemberAttribute()]
        public Single MDose
        {
            get
            {
                return _MDose;
            }
            set
            {
                _MDose = value;
                RaisePropertyChanged("MDose");

            }
        }
        private Single _MDose;

        [DataMemberAttribute()]
        public Single ADose
        {
            get
            {
                return _ADose;
            }
            set
            {
                _ADose = value;
                RaisePropertyChanged("ADose");

            }
        }
        private Single _ADose;

        [DataMemberAttribute()]
        public Single EDose
        {
            get
            {
                return _EDose;
            }
            set
            {
                _EDose = value;
                RaisePropertyChanged("EDose");
            }
        }
        private Single _EDose;

        [DataMemberAttribute()]
        public Single NDose
        {
            get
            {
                return _NDose;
            }
            set
            {
                _NDose = value;
                RaisePropertyChanged("NDose");
            }
        }
        private Single _NDose;

        private string _HIProductCode5084;
        [DataMemberAttribute()]
        public string HIProductCode5084
        {
            get
            {
                return _HIProductCode5084;
            }
            set
            {
                _HIProductCode5084 = value;
                RaisePropertyChanged("HIProductCode5084");
            }
        }

        [DataMemberAttribute]
        public string BidName
        {
            get
            {
                return _BidName;
            }
            set
            {
                _BidName = value;
                RaisePropertyChanged("BidName");
            }
        }
        private string _BidName;

        [DataMemberAttribute]
        public long BidID
        {
            get
            {
                return _BidID;
            }
            set
            {
                _BidID = value;
                RaisePropertyChanged("BidID");
            }
        }
        private long _BidID;

        [DataMemberAttribute]
        public long HITTypeID
        {
            get
            {
                return _HITTypeID;
            }
            set
            {
                _HITTypeID = value;
                RaisePropertyChanged("HITTypeID");
            }
        }
        private long _HITTypeID;

        [DataMemberAttribute]
        public string BidCode
        {
            get
            {
                return _BidCode;
            }
            set
            {
                _BidCode = value;
                RaisePropertyChanged("BidCode");
            }
        }
        private string _BidCode;

        [DataMemberAttribute]
        public long V_ProductScope
        {
            get
            {
                return _V_ProductScope;
            }
            set
            {
                _V_ProductScope = value;
                RaisePropertyChanged("V_ProductScope");
            }
        }
        private long _V_ProductScope;

        //▼===== #005
        [DataMemberAttribute]
        public long V_CatDrugType
        {
            get
            {
                return _V_CatDrugType;
            }
            set
            {
                _V_CatDrugType = value;
                RaisePropertyChanged("V_CatDrugType");
            }
        }
        private long _V_CatDrugType;

        [DataMemberAttribute()]
        public string DrugCode
        {
            get
            {
                return _DrugCode;
            }
            set
            {
                _DrugCode = value;
                RaisePropertyChanged("DrugCode");
            }
        }
        private string _DrugCode;
        //▲===== #005

        [DataMemberAttribute]
        public bool AppliedMedItemFollow04Cer
        {
            get
            {
                return _AppliedMedItemFollow04Cer;
            }
            set
            {
                _AppliedMedItemFollow04Cer = value;
                RaisePropertyChanged("AppliedMedItemFollow04Cer");
            }
        }
        private bool _AppliedMedItemFollow04Cer;

        private DateTime? _DSPTModifiedDate;
        [DataMemberAttribute()]
        public DateTime? DSPTModifiedDate
        {
            get
            {
                return _DSPTModifiedDate;
            }
            set
            {
                _DSPTModifiedDate = value;
                RaisePropertyChanged("DSPTModifiedDate");
            }
        }
        #region Navigation Properties

        [DataMemberAttribute()]
        public DrugDeptSupplier SupplierMain
        {
            get
            {
                return _SupplierMain;
            }
            set
            {
                _SupplierMain = value;
                RaisePropertyChanged("SupplierMain");
            }
        }
        private DrugDeptSupplier _SupplierMain;

        private RefGenMedDrugDetails _RefGenMedDrugDetails;
        [DataMemberAttribute()]
        public RefGenMedDrugDetails RefGenMedDrugDetails
        {
            get
            {
                return _RefGenMedDrugDetails;
            }
            set
            {
                if (_RefGenMedDrugDetails != value)
                {
                    _RefGenMedDrugDetails = value;
                    RaisePropertyChanged("RefGenMedDrugDetails");
                }
            }
        }

        [Required(ErrorMessage = "Chọn Đơn Vị Tính!")]
        [DataMemberAttribute()]
        public RefUnit SelectedUnit
        {
            get
            {
                return _selectedUnit;
            }
            set
            {
                OnSelectedUnitChanging(value);
                ValidateProperty("SelectedUnit", value);
                _selectedUnit = value;
                RaisePropertyChanged("SelectedUnit");
                OnSelectedUnitChanged();
            }
        }
        private RefUnit _selectedUnit;
        partial void OnSelectedUnitChanging(RefUnit unit);
        partial void OnSelectedUnitChanged();

        [Required(ErrorMessage = "Chọn Đơn Vị Dùng!")]
        [DataMemberAttribute()]
        public RefUnit SelectedUnitUse
        {
            get
            {
                return _SelectedUnitUse;
            }
            set
            {
                OnSelectedUnitUseChanging(value);
                ValidateProperty("SelectedUnitUse", value);
                _SelectedUnitUse = value;
                RaisePropertyChanged("SelectedUnitUse");
                OnSelectedUnitUseChanged();
            }
        }
        private RefUnit _SelectedUnitUse;
        partial void OnSelectedUnitUseChanging(RefUnit unit);
        partial void OnSelectedUnitUseChanged();


        [Required(ErrorMessage = "Chọn Quốc Gia!")]
        [DataMemberAttribute()]        
        public RefCountry SelectedCountry
        {
            get
            {
                return _selectedCountry;
            }
            set
            {   
                OnCountryChanging(value);
                ValidateProperty("SelectedCountry", value);
                _selectedCountry = value;
                RaisePropertyChanged("SelectedCountry");
                OnCountryChanged();                
            }
        }
        private RefCountry _selectedCountry;
        partial void OnCountryChanging(RefCountry unit);
        partial void OnCountryChanged();


        [Required(ErrorMessage = "Vui lòng chọn họ thuốc")]
        [DataMemberAttribute()]
        public DrugClass SelectedDrugClass
        {
            get
            {
                return _SelectedDrugClass;
            }
            set
            {
                OnFamilyTherapyChanging(value);
                ValidateProperty("SelectedDrugClass", value);
                _SelectedDrugClass = value;
                RaisePropertyChanged("SelectedDrugClass");
                OnFamilyTherapyChanged();
            }
        }
        private DrugClass _SelectedDrugClass;
        partial void OnFamilyTherapyChanging(DrugClass unit);
        partial void OnFamilyTherapyChanged();


        [Required(ErrorMessage = "Vui lòng chọn NSX")]
        [DataMemberAttribute()]
        public DrugDeptPharmaceuticalCompany PharmaceuticalCompany
        {
            get
            {
                return _PharmaceuticalCompany;
            }
            set
            {
                if (_PharmaceuticalCompany != value)
                {
                    OnPharmaceuticalCompanyChanging(value);
                    ValidateProperty("PharmaceuticalCompany", value);
                    _PharmaceuticalCompany = value;
                    RaisePropertyChanged("PharmaceuticalCompany");
                    OnPharmaceuticalCompanyChanged();
                }
            }
        }
        private DrugDeptPharmaceuticalCompany _PharmaceuticalCompany;
        partial void OnPharmaceuticalCompanyChanging(DrugDeptPharmaceuticalCompany unit);
        partial void OnPharmaceuticalCompanyChanged();
        /*▼====: #006*/
        [DataMemberAttribute()]
        public long GenericID
        {
            get
            {
                return _GenericID;
            }
            set
            {
                if (_GenericID != value)
                {
                    _GenericID = value;
                    RaisePropertyChanged("GenericID");
                }
            }
        }
        private long _GenericID;
        [DataMemberAttribute()]
        public DrugClass SelectedGeneric
        {
            get
            {
                return _SelectedGeneric;
            }
            set
            {
                _SelectedGeneric = value;
                RaisePropertyChanged("SelectedGeneric");
            }
        }
        private DrugClass _SelectedGeneric;
        /*▲====: #006*/
        [DataMemberAttribute()]
        public Hospital CurrentHospital
        {
            get
            {
                return _CurrentHospital;
            }
            set
            {
                if (_CurrentHospital != value)
                {
                    OnCurrentHospitalChanging(value);
                    _CurrentHospital = value;
                    RaisePropertyChanged("CurrentHospital");
                    OnCurrentHospitalChanged();
                }
            }
        }
        private Hospital _CurrentHospital;
        partial void OnCurrentHospitalChanging(Hospital CurrentHospital);
        partial void OnCurrentHospitalChanged();


        [DataMemberAttribute()]
        public string WinningHospitals
        {
            get
            {
                return _winningHospitals;
            }
            set
            {
                if (_winningHospitals != value)
                {
                    OnWinningHospitalsChanging(value);
                    _winningHospitals = value;
                    RaisePropertyChanged("WinningHospitals");
                    OnWinningHospitalsChanged();
                }
            }
        }
        private string _winningHospitals;
        partial void OnWinningHospitalsChanging(string WinningHospitals);
        partial void OnWinningHospitalsChanged();
        //▼====: #007
        private bool _NgoaiDinhSuat;
        [DataMemberAttribute()]
        public bool NgoaiDinhSuat
        {
            get
            {
                return _NgoaiDinhSuat;
            }
            set
            {
                _NgoaiDinhSuat = value;
                RaisePropertyChanged("NgoaiDinhSuat");
            }
        }
        //▲====: #007
        private bool _InCategoryCOVID;
        [DataMemberAttribute()]
        public bool InCategoryCOVID
        {
            get
            {
                return _InCategoryCOVID;
            }
            set
            {
                _InCategoryCOVID = value;
                RaisePropertyChanged("InCategoryCOVID");
            }
        }
        #endregion

        #region IChargeableItemPrice Members
        private decimal _NormalPrice;
        [DataMemberAttribute()]
        public decimal NormalPrice
        {
            get
            {
                return _NormalPrice;
            }
            set
            {
                _NormalPrice = value;
                RaisePropertyChanged("NormalPrice");
            }
        }

        private decimal _HIPatientPrice;
        [DataMemberAttribute()]
        public decimal HIPatientPrice
        {
            get
            {
                return _HIPatientPrice;
            }
            set
            {
                _HIPatientPrice = value;
                RaisePropertyChanged("HIPatientPrice");
            }
        }
        private decimal? _HIAllowedPrice;
        [DataMemberAttribute()]
        public decimal? HIAllowedPrice
        {
            get
            {
                return _HIAllowedPrice;
            }
            set
            {
                _HIAllowedPrice = value;
                RaisePropertyChanged("HIAllowedPrice");
            }
        }
        private ChargeableItemType _ChargeableItemType = ChargeableItemType.NONE;
        public virtual ChargeableItemType ChargeableItemType
        {
            get
            {
                return _ChargeableItemType;
            }
            set
            {
                _ChargeableItemType = value;
                RaisePropertyChanged("ChargeableItemType");
            }
        }

        [DataMemberAttribute()]
        public decimal NormalPriceNew
        {
            get
            {
                return _NormalPriceNew;
            }
            set
            {
                _NormalPriceNew = value;
                RaisePropertyChanged("NormalPriceNew");
            }
        }
        private decimal _NormalPriceNew;

        [DataMemberAttribute()]
        public decimal HIPatientPriceNew
        {
            get
            {
                return _HIPatientPriceNew;
            }
            set
            {
                _HIPatientPriceNew = value;
                RaisePropertyChanged("HIPatientPriceNew");
            }
        }
        private decimal _HIPatientPriceNew;

        [DataMemberAttribute()]
        public decimal? HIAllowedPriceNew
        {
            get
            {
                return _HIAllowedPriceNew;
            }
            set
            {
                _HIAllowedPriceNew = value;
                RaisePropertyChanged("HIAllowedPriceNew");
            }
        }
        private decimal? _HIAllowedPriceNew;
        #endregion
        //▼====: #002
        private float ChangeDoseStringToFloat(string value)
        {
            float result = 0;
            if (!string.IsNullOrEmpty(value))
            {
                if (value.Contains("/"))
                {
                    string pattern = @"\b[\d]+/[\d]+\b";
                    if (!Regex.IsMatch(value, pattern))
                    {
                        return 0;
                    }
                    else
                    {
                        string[] items = null;
                        items = value.Split('/');
                        if (items.Count() > 2 || items.Count() == 0)
                        {
                            return 0;
                        }
                        else if (float.Parse(items[1]) == 0)
                        {
                            return 0;
                        }
                        result = (float.Parse(items[0]) / float.Parse(items[1]));
                        if (result < 0)
                        {
                            return 0;
                        }
                    }
                }
                else
                {
                    try
                    {
                        result = float.Parse(value);
                        if (result < 0)
                        {
                            return 0;
                        }
                    }
                    catch
                    {
                        return 0;
                    }
                }
            }
            return result;
        }
        public void GetDoesFromDoseString()
        {
            this.MDose = ChangeDoseStringToFloat(this.MDoseStr);
            this.ADose = ChangeDoseStringToFloat(this.ADoseStr);
            this.EDose = ChangeDoseStringToFloat(this.EDoseStr);
            this.NDose = ChangeDoseStringToFloat(this.NDoseStr);
        }
        //▲====: #002
        public override bool Equals(object obj)
        {
            RefGenMedProductDetails seletedDrug = obj as RefGenMedProductDetails;
            if (seletedDrug == null)
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;
            if (this.GenMedProductID > 0)
            {
                //if (this.GenMedProductID == seletedDrug.GenMedProductID)
                if (this.GenMedProductID == seletedDrug.GenMedProductID && this.BrandName.Equals(seletedDrug.BrandName))
                {
                    return this.StoreID.GetValueOrDefault(-1) == seletedDrug.StoreID.GetValueOrDefault(-1)
                            && this.InBatchNumber == seletedDrug.InBatchNumber;//Cùng store và cùng lô.
                }
                return false;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        #region Extention Member

        [DataMemberAttribute()]
        public Int64? OutClinicDeptReqID
        {
            get
            {
                return _OutClinicDeptReqID;
            }
            set
            {
                if (_OutClinicDeptReqID != value)
                {
                    OnOutClinicDeptReqIDChanging(value);
                    _OutClinicDeptReqID = value;
                    RaisePropertyChanged("OutClinicDeptReqID");
                    OnOutClinicDeptReqIDChanged();
                }
            }
        }
        private Int64? _OutClinicDeptReqID;
        partial void OnOutClinicDeptReqIDChanging(Int64? value);
        partial void OnOutClinicDeptReqIDChanged();

        //gia von
        [DataMemberAttribute()]
        public decimal InCost
        {
            get
            {
                return _InCost;
            }
            set
            {
                if (_InCost != value)
                {
                    _InCost = value;
                    RaisePropertyChanged("InCost");
                }
            }
        }
        private decimal _InCost;
        //gia mua gan nhat
        [DataMemberAttribute()]
        public decimal InBuyingPrice
        {
            get
            {
                return _InBuyingPrice;
            }
            set
            {
                if (_InBuyingPrice != value)
                {
                    _InBuyingPrice = value;
                    RaisePropertyChanged("InBuyingPrice");
                }
            }
        }
        private decimal _InBuyingPrice;

        [DataMemberAttribute()]
        public decimal UnitPrice
        {
            get
            {
                return _UnitPrice;
            }
            set
            {
                if (_UnitPrice != value)
                {
                    _UnitPrice = value;
                    RaisePropertyChanged("UnitPrice");
                }
            }
        }
        private decimal _UnitPrice;

        [DataMemberAttribute()]
        public decimal PackagePrice
        {
            get
            {
                return _PackagePrice;
            }
            set
            {
                if (_PackagePrice != value)
                {
                    _PackagePrice = value;
                    RaisePropertyChanged("PackagePrice");
                }
            }
        }
        private decimal _PackagePrice;

        [DataMemberAttribute()]
        public decimal RequestQty
        {
            get
            {
                return _RequestQty;
            }
            set
            {
                if (_RequestQty != value)
                {
                    _RequestQty = value;
                    RaisePropertyChanged("RequestQty");
                }
            }
        }
        private decimal _RequestQty;

        //dung cho dat hang,so luong da dat
        [DataMemberAttribute()]
        public int? Ordered
        {
            get
            {
                return _Ordered;
            }
            set
            {
                if (_Ordered != value)
                {
                    _Ordered = value;
                    RaisePropertyChanged("Ordered");
                }
            }
        }
        private int? _Ordered;

        //ton kho tai thoi diem dat
        [DataMemberAttribute()]
        public decimal Remaining
        {
            get
            {
                return _Remaining;
            }
            set
            {
                if (_Remaining != value)
                {
                    _Remaining = value;
                    RaisePropertyChanged("Remaining");
                }
            }
        }
        private decimal _Remaining;

        [DataMemberAttribute()]
        public decimal RemainingFirst
        {
            get
            {
                return _RemainingFirst;
            }
            set
            {
                if (_RemainingFirst != value)
                {
                    _RemainingFirst = value;
                    RaisePropertyChanged("RemainingFirst");
                }
            }
        }
        private decimal _RemainingFirst;

        [DataMemberAttribute()]
        public decimal RequiredNumber
        {
            get
            {
                return _RequiredNumber;
            }
            set
            {
                    OnRequiredNumberChanging(value);
                    _RequiredNumber = value;
                    RaisePropertyChanged("RequiredNumber");
                    OnRequiredNumberChanged();
            }
        }
        private decimal _RequiredNumber;
        partial void OnRequiredNumberChanging(decimal value);
        partial void OnRequiredNumberChanged();

        [DataMemberAttribute()]
        public long TypID
        {
            get
            {
                return _TypID;
            }
            set
            {
                _TypID = value;
                RaisePropertyChanged("TypID");
            }
        }
        private long _TypID;

        //KMx: Không dùng hàm này nữa, nếu không sẽ không xuất thuốc có DispenseVolume cho bệnh nhân được. Trường hợp thuốc còn 100 chai, người dùng nhập 300 đơn vị dùng thì không được (15/11/2014 10:49).
        //partial void OnRequiredNumberChanging(decimal value)
        //{
        //    if (value > Remaining && TypID !=(long)AllLookupValues.RefOutputType.XUAT_HANGKYGOI)
        //    {
        //        if (Remaining == 0)
        //        {
        //            AddError("RequiredNumber", "Hàng không còn trong kho", false);
        //        }
        //        else
        //        {
        //            AddError("RequiredNumber", "Số lượng xuất phải <= " + Remaining.ToString(), false);
        //        }
        //    }
        //    else
        //    {
        //        RemoveError("RequiredNumber", "Hàng không còn trong kho");
        //        RemoveError("RequiredNumber", "Số lượng xuất phải <= " + Remaining.ToString());
        //    }
        //}

        [DataMemberAttribute()]
        public int WaitingDeliveryQty
        {
            get
            {
                return _WaitingDeliveryQty;
            }
            set
            {
                if (_WaitingDeliveryQty != value)
                {
                    _WaitingDeliveryQty = value;
                    RaisePropertyChanged("WaitingDeliveryQty");
                }
            }
        }
        private int _WaitingDeliveryQty;

        [DataMemberAttribute()]
        public long InID
        {
            get
            {
                return _InID;
            }
            set
            {
                if (_InID != value)
                {
                    OnInIDChanging(value);
                    _InID = value;
                    RaisePropertyChanged("InID");
                    OnInIDChanged();
                }
            }
        }
        private long _InID;
        partial void OnInIDChanging(long value);
        partial void OnInIDChanged();

        [DataMemberAttribute()]
        public String InBatchNumber
        {
            get
            {
                return _InBatchNumber;
            }
            set
            {
                OnInBatchNumberChanging(value);
                _InBatchNumber = value;
                RaisePropertyChanged("InBatchNumber");
                OnInBatchNumberChanged();
            }
        }
        private String _InBatchNumber;
        partial void OnInBatchNumberChanging(String value);
        partial void OnInBatchNumberChanged();


        [DataMemberAttribute()]
        public DateTime InwardDate
        {
            get
            {
                return _InwardDate;
            }
            set
            {
                _InwardDate = value;
                RaisePropertyChanged("InwardDate");
            }
        }
        private DateTime _InwardDate;

        [DataMemberAttribute()]
        public DateTime? InExpiryDate
        {
            get
            {
                return _InExpiryDate;
            }
            set
            {
                OnInExpiryDateChanging(value);
                _InExpiryDate = value;
                RaisePropertyChanged("InExpiryDate");
                OnInExpiryDateChanged();
            }
        }
        private DateTime? _InExpiryDate;
        partial void OnInExpiryDateChanging(DateTime? value);
        partial void OnInExpiryDateChanged();

        [DataMemberAttribute()]
        public Decimal OutPrice
        {
            get
            {
                return _OutPrice;
            }
            set
            {
                OnOutPriceChanging(value);
                _OutPrice = value;
                RaisePropertyChanged("OutPrice");
                OnOutPriceChanged();
            }
        }
        private Decimal _OutPrice;
        partial void OnOutPriceChanging(Decimal value);
        partial void OnOutPriceChanged();


        [DataMemberAttribute()]
        public long STT
        {
            get
            {
                return _STT;
            }
            set
            {
                _STT = value;
                RaisePropertyChanged("STT");
            }
        }
        private long _STT;

        [DataMemberAttribute()]
        public String SdlDescription
        {
            get
            {
                return _SdlDescription;
            }
            set
            {
                _SdlDescription = value;
                RaisePropertyChanged("SdlDescription");
            }
        }
        private String _SdlDescription;

        public string TypeName
        {
            get
            {
                switch (V_MedProductType)
                {
                    case (long)AllLookupValues.MedProductType.HOA_CHAT:
                        return "Hóa Chất";
                    case (long)AllLookupValues.MedProductType.THUOC:
                        return "Thuốc";
                    case (long)AllLookupValues.MedProductType.Y_CU:
                        return "Y Cụ";
                    case (long)AllLookupValues.MedProductType.CAN_LAM_SANG:
                        return "CLS";
                    case (long)AllLookupValues.MedProductType.KCB:
                        return "KCB";
                    default:
                        return string.Empty;
                }
            }
        }

        [DataMemberAttribute()]
        public long V_MedicalMaterial
        {
            get
            {
                return _V_MedicalMaterial;
            }
            set
            {
                _V_MedicalMaterial = value;
                RaisePropertyChanged("V_MedicalMaterial");
            }
        }
        private long _V_MedicalMaterial;

        //KMx: Chỉ sử dụng HICode, không dùng HICode2 (31/08/2015 15:35).
        //[DataMemberAttribute()]
        //public string HICode2
        //{
        //    get
        //    {
        //        return _HICode2;
        //    }
        //    set
        //    {
        //        _HICode2 = value;
        //        RaisePropertyChanged("HICode2");
        //    }
        //}
        //private string _HICode2;

        [DataMemberAttribute()]
        public string BrandNameAndCode
        {
            get
            {
                return this.BrandName + " ( "+ this.Code +" ) ";
            }
            set { }
          
        }


        [DataMemberAttribute()]
        public double? HIPaymentPercent
        {
            get
            {
                return _HIPaymentPercent;
            }
            set
            {
                _HIPaymentPercent = value;
                RaisePropertyChanged("HIPaymentPercent");
            }
        }
        private double? _HIPaymentPercent = 1;

        [DataMemberAttribute()]
        public string ReportBrandName
        {
            get
            {
                return _ReportBrandName;
            }
            set
            {
                _ReportBrandName = value;
                RaisePropertyChanged("ReportBrandName");
            }
        }
        private string _ReportBrandName;

        [DataMemberAttribute()]
        public ObservableCollection<Lookup> ListRouteOfAdministration
        {
            get
            {
                return _ListRouteOfAdministration;
            }
            set
            {
                _ListRouteOfAdministration = value;
                RaisePropertyChanged("ListRouteOfAdministration");
            }
        }
        private ObservableCollection<Lookup> _ListRouteOfAdministration;

        [DataMemberAttribute()]
        public Lookup RouteOfAdministration
        {
            get
            {
                return _RouteOfAdministration;
            }
            set
            {
                _RouteOfAdministration = value;
                RaisePropertyChanged("RouteOfAdministration");
            }
        }
        private Lookup _RouteOfAdministration;

        [DataMemberAttribute()]
        public string BiddingHospital
        {
            get
            {
                return _BiddingHospital;
            }
            set
            {
                _BiddingHospital = value;
                RaisePropertyChanged("BiddingHospital");
            }
        }
        private string _BiddingHospital;

        [DataMemberAttribute()]
        public string BidDecisionNumAndOrdinalNum
        {
            get
            {
                return _BidDecisionNumAndOrdinalNum;
            }
            set
            {
                _BidDecisionNumAndOrdinalNum = value;
                RaisePropertyChanged("BidDecisionNumAndOrdinalNum");
            }
        }
        private string _BidDecisionNumAndOrdinalNum;

        [DataMemberAttribute()]
        public string BidDecisionNumAndEffectiveDate
        {
            get
            {
                return _BidDecisionNumAndEffectiveDate;
            }
            set
            {
                _BidDecisionNumAndEffectiveDate = value;
                RaisePropertyChanged("BidDecisionNumAndEffectiveDate");
            }
        }
        private string _BidDecisionNumAndEffectiveDate;

        [DataMemberAttribute()]
        public string OtherDecisionNumAndEffectiveDate
        {
            get
            {
                return _OtherDecisionNumAndEffectiveDate;
            }
            set
            {
                _OtherDecisionNumAndEffectiveDate = value;
                RaisePropertyChanged("OtherDecisionNumAndEffectiveDate");
            }
        }
        private string _OtherDecisionNumAndEffectiveDate;

        [DataMemberAttribute()]
        public string BidEffectiveDate
        {
            get
            {
                return _BidEffectiveDate;
            }
            set
            {
                _BidEffectiveDate = value;
                RaisePropertyChanged("BidEffectiveDate");
            }
        }
        private string _BidEffectiveDate;

        [DataMemberAttribute()]
        public string BidExpirationDate
        {
            get
            {
                return _BidExpirationDate;
            }
            set
            {
                _BidExpirationDate = value;
                RaisePropertyChanged("BidExpirationDate");
            }
        }
        private string _BidExpirationDate;

        [DataMemberAttribute()]
        public bool IsHighTechService
        {
            get
            {
                return _IsHighTechService;
            }
            set
            {
                _IsHighTechService = value;
                RaisePropertyChanged("IsHighTechService");
            }
        }
        private bool _IsHighTechService;

        /*==== #001 ====*/
        [DataMemberAttribute()]
        public Int32? MaxQtyHIAllowItem
        {
            get
            {
                return _MaxQtyHIAllowItem;
            }
            set
            {
                if (_MaxQtyHIAllowItem != value)
                {
                    _MaxQtyHIAllowItem = value;
                    RaisePropertyChanged("MaxQtyHIAllowItem");
                }
            }
        }
        private Int32? _MaxQtyHIAllowItem;

        [DataMemberAttribute()]
        public Double? PaymentRateOfHIAddedItem
        {
            get
            {
                return _PaymentRateOfHIAddedItem;
            }
            set
            {
                if (_PaymentRateOfHIAddedItem != value)
                {
                    _PaymentRateOfHIAddedItem = value;
                    RaisePropertyChanged("PaymentRateOfHIAddedItem");
                }
            }
        }
        private Double? _PaymentRateOfHIAddedItem;
        /*==== #001 ====*/

        [DataMemberAttribute()]
        public string TechServiceCode
        {
            get
            {
                return _TechServiceCode;
            }
            set
            {
                _TechServiceCode = value;
                RaisePropertyChanged("TechServiceCode");
            }
        }
        private string _TechServiceCode;

        [DataMemberAttribute()]
        public bool IsStamp
        {
            get
            {
                return _IsStamp;
            }
            set
            {
                _IsStamp = value;
                RaisePropertyChanged("IsStamp");
            }
        }
        private bool _IsStamp;

        [DataMemberAttribute()]
        public Double NumOfUse
        {
            get
            {
                return _NumOfUse;
            }
            set
            {
                _NumOfUse = value;
                RaisePropertyChanged("NumOfUse");
            }
        }
        private Double _NumOfUse = 1;

        [DataMemberAttribute()]
        public decimal MaxHIPay
        {
            get
            {
                return _MaxHIPay;
            }
            set
            {
                _MaxHIPay = value;
                RaisePropertyChanged("MaxHIPay");
            }
        }
        private decimal _MaxHIPay;

        [DataMemberAttribute()]
        public string Note
        {
            get
            {
                return _Note;
            }
            set
            {
                _Note = value;
                RaisePropertyChanged("Note");
            }
        }
        private string _Note;


        [DataMemberAttribute()]
        public bool IsWatchOutQty
        {
            get
            {
                return _IsWatchOutQty;
            }
            set
            {
                if (_IsWatchOutQty != value)
                {
                    _IsWatchOutQty = value;
                    RaisePropertyChanged("IsWatchOutQty");
                }
            }
        }
        private bool _IsWatchOutQty;


        [DataMemberAttribute()]
        public int LimitedOutQty
        {
            get
            {
                return _limitedOutQty;
            }
            set
            {
                _limitedOutQty = value;
                RaisePropertyChanged("LimitedOutQty");
            }
        }
        private int _limitedOutQty;


        [DataMemberAttribute()]
        public int RemainWarningLevel1
        {
            get
            {
                return _remainWarningLevel1;
            }
            set
            {
                _remainWarningLevel1 = value;
                RaisePropertyChanged("RemainWarningLevel1");
            }
        }
        private int _remainWarningLevel1;


        [DataMemberAttribute()]
        public int RemainWarningLevel2
        {
            get
            {
                return _remainWarningLevel2;
            }
            set
            {
                _remainWarningLevel2 = value;
                RaisePropertyChanged("RemainWarningLevel2");
            }
        }
        private int _remainWarningLevel2;

        [DataMemberAttribute()]
        public bool IsDonatedGoods
        {
            get
            {
                return _IsDonatedGoods;
            }
            set
            {
                _IsDonatedGoods = value;
                RaisePropertyChanged("IsDonatedGoods");
            }
        }
        private bool _IsDonatedGoods;

        #endregion

        #region Supplier member

        [DataMemberAttribute()]
        public ObservableCollection<SupplierGenMedProduct> SupplierGenMedProducts
        {
            get
            {
                return _SupplierGenMedProducts;
            }
            set
            {
                _SupplierGenMedProducts = value;
                RaisePropertyChanged("SupplierGenMedProducts");
            }
        }
        private ObservableCollection<SupplierGenMedProduct> _SupplierGenMedProducts;

        [DataMemberAttribute()]
        public decimal TLThanhToan
        {
            get
            {
                return _TLThanhToan;
            }
            set
            {
                if (_TLThanhToan != value)
                {
                    _TLThanhToan = value;
                    RaisePropertyChanged("TLThanhToan");
                }
            }
        }
        private decimal _TLThanhToan;
        #region Convert XML

        public string ConvertDetailsListToXml()
        {
            return ConvertDetailsListToXml(_SupplierGenMedProducts);
        }
        public string ConvertDetailsListToXml(IEnumerable<SupplierGenMedProduct> items)
        {
            if (items != null)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("<SupplierGenMedProducts>");
                foreach (SupplierGenMedProduct details in items)
                {

                    if (details.SupplierID > 0)
                    {
                        sb.Append("<RecInfo>");
                        sb.AppendFormat("<SupplierID>{0}</SupplierID>", details.SupplierID);
                        sb.AppendFormat("<GenMedProductID>{0}</GenMedProductID>", details.GenMedProductID);
                        sb.AppendFormat("<IsMain>{0}</IsMain>", details.IsMain);
                        sb.AppendFormat("<UnitPrice>{0}</UnitPrice>", details.UnitPrice);
                        sb.AppendFormat("<VAT>{0}</VAT>", details.VAT);
                        sb.AppendFormat("<PackagePrice>{0}</PackagePrice>", details.PackagePrice);
                        sb.Append("</RecInfo>");
                    }
                }
                sb.Append("</SupplierGenMedProducts>");
                return sb.ToString();
            }
            else
            {
                return null;
            }
        }

        #endregion
        #endregion
        /*▼====: #004*/
        #region ObservableCollection for Service
        [DataMemberAttribute()]
        public ObservableCollection<RefMedicalServiceItem> RefMedicalServiceItems
        {
            get
            {
                return _RefMedicalServiceItems;
            }
            set
            {
                _RefMedicalServiceItems = value;
                RaisePropertyChanged("RefMedicalServiceItems");
            }
        }
        private ObservableCollection<RefMedicalServiceItem> _RefMedicalServiceItems;
        public string ConvertDetailsListToXmlForService()
        {
            return ConvertDetailsListToXmlForService(_RefMedicalServiceItems);
        }
        public string ConvertDetailsListToXmlForService(IEnumerable<RefMedicalServiceItem> items)
        {
            if (items != null)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("<RefMedicalServiceItems>");
                foreach (RefMedicalServiceItem details in items)
                {
                    if (details.MedServiceID > 0)
                    {
                        sb.Append("<RecInfo>");
                        sb.AppendFormat("<MedServiceID>{0}</MedServiceID>", details.MedServiceID);
                        sb.Append("</RecInfo>");
                    }
                }
                sb.Append("</RefMedicalServiceItems>");
                return sb.ToString();
            }
            else
                return null;
        }
        #endregion
        /*▲====: #004*/
        private Nullable<bool> _IsNotShow;
        private int _BidRemainingQty;
        private int _ApprovedQty;
        private decimal _BidInCost;
        [DataMemberAttribute]
        public Nullable<bool> IsNotShow
        {
            get
            {
                return _IsNotShow;
            }
            set
            {
                _IsNotShow = value;
                RaisePropertyChanged("IsNotShow");
            }
        }
        [DataMemberAttribute]
        public int BidRemainingQty
        {
            get
            {
                return _BidRemainingQty;
            }
            set
            {
                if (_BidRemainingQty == value)
                {
                    return;
                }
                _BidRemainingQty = value;
                RaisePropertyChanged("BidRemainingQty");
            }
        }
        [DataMemberAttribute]
        public int ApprovedQty
        {
            get
            {
                return _ApprovedQty;
            }
            set
            {
                if (_ApprovedQty == value)
                {
                    return;
                }
                _ApprovedQty = value;
                RaisePropertyChanged("ApprovedQty");
            }
        }
        [DataMemberAttribute]
        public decimal BidInCost
        {
            get
            {
                return _BidInCost;
            }
            set
            {
                _BidInCost = value;
                RaisePropertyChanged("BidInCost");
            }
        }

        [DataMemberAttribute]
        public long LastID
        {
            get
            {
                return _LastID;
            }
            set
            {
                if (_LastID == value)
                {
                    return;
                }
                _LastID = value;
                RaisePropertyChanged("LastID");
            }
        }
        private long _LastID;

        [DataMemberAttribute]
        public string LastCode
        {
            get
            {
                return _LastCode;
            }
            set
            {
                if (_LastCode == value)
                {
                    return;
                }
                _LastCode = value;
                RaisePropertyChanged("LastCode");
            }
        }
        private string _LastCode;

        private decimal? _CeilingPrice1stItem;
        private decimal? _CeilingPrice2ndItem;
        private decimal? _CeilingPrice3rdItem;
        private BidDetail _BidDetail;
        private long? _DrugDeptInIDOrig;
        [DataMemberAttribute]
        public decimal? CeilingPrice1stItem
        {
            get
            {
                return _CeilingPrice1stItem;
            }
            set
            {
                if (_CeilingPrice1stItem == value)
                {
                    return;
                }
                _CeilingPrice1stItem = value;
                RaisePropertyChanged("CeilingPrice1stItem");
            }
        }
        [DataMemberAttribute]
        public decimal? CeilingPrice2ndItem
        {
            get
            {
                return _CeilingPrice2ndItem;
            }
            set
            {
                if (_CeilingPrice2ndItem == value)
                {
                    return;
                }
                _CeilingPrice2ndItem = value;
                RaisePropertyChanged("CeilingPrice2ndItem");
            }
        }
        [DataMemberAttribute]
        public decimal? CeilingPrice3rdItem
        {
            get
            {
                return _CeilingPrice3rdItem;
            }
            set
            {
                if (_CeilingPrice3rdItem == value)
                {
                    return;
                }
                _CeilingPrice3rdItem = value;
                RaisePropertyChanged("CeilingPrice3rdItem");
            }
        }
        [DataMemberAttribute]
        public BidDetail BidDetail
        {
            get
            {
                return _BidDetail;
            }
            set
            {
                if (_BidDetail == value)
                {
                    return;
                }
                _BidDetail = value;
                RaisePropertyChanged("BidDetail");
            }
        }
        [DataMemberAttribute]
        public long? DrugDeptInIDOrig
        {
            get
            {
                return _DrugDeptInIDOrig;
            }
            set
            {
                if (_DrugDeptInIDOrig == value)
                {
                    return;
                }
                _DrugDeptInIDOrig = value;
                RaisePropertyChanged("DrugDeptInIDOrig");
            }
        }
        private long _V_InstructionOrdinalType = 84600;
        [DataMemberAttribute]
        public long V_InstructionOrdinalType
        {
            get
            {
                return _V_InstructionOrdinalType;
            }
            set
            {
                if (_V_InstructionOrdinalType == value)
                {
                    return;
                }
                _V_InstructionOrdinalType = value;
                RaisePropertyChanged("V_InstructionOrdinalType");
            }
        }
        private Int16 _MinDayOrdinalContinueIsAllowable;
        [DataMemberAttribute]
        public Int16 MinDayOrdinalContinueIsAllowable
        {
            get
            {
                return _MinDayOrdinalContinueIsAllowable;
            }
            set
            {
                if (_MinDayOrdinalContinueIsAllowable == value)
                {
                    return;
                }
                _MinDayOrdinalContinueIsAllowable = value;
                RaisePropertyChanged("MinDayOrdinalContinueIsAllowable");
            }
        }

        private double? _VAT;
        [DataMemberAttribute]
        public double? VAT
        {
            get
            {
                return _VAT;
            }
            set
            {
                if (_VAT == value)
                {
                    return;
                }
                _VAT = value;
                RaisePropertyChanged("VAT");
            }
        }
        private bool _IsNotVat;
        [DataMemberAttribute]
        public bool IsNotVat
        {
            get
            {
                return _IsNotVat;
            }
            set
            {
                if (_IsNotVat == value)
                {
                    return;
                }
                _IsNotVat = value;
                if (_IsNotVat)
                {
                    VAT = null;
                }
                else
                {
                    VAT = 0;
                }
                RaisePropertyChanged("IsNotVat");
                RaisePropertyChanged("VAT");
            }
        }

        private LimQtyHiItemMaxPaymtPerc _LimQtyAndHIPrice;
        [DataMemberAttribute]
        public LimQtyHiItemMaxPaymtPerc LimQtyAndHIPrice
        {
            get
            {
                return _LimQtyAndHIPrice;
            }
            set
            {
                if (_LimQtyAndHIPrice != value)
                {
                    _LimQtyAndHIPrice = value;
                    RaisePropertyChanged("LimQtyAndHIPrice");
                }
            }
        }
        /// <summary>
        /// Staff dùng trong trường hợp tạo phiếu lĩnh từ toa xuất viện.
        /// </summary>
        private Staff _Staff;
        [DataMemberAttribute]
        public Staff Staff
        {
            get
            {
                return _Staff;
            }
            set
            {
                if (_Staff != value)
                {
                    _Staff = value;
                    RaisePropertyChanged("Staff");
                }
            }
        }
        /// <summary>
        /// Ngày y lệnh dành cho toa thuốc nội trú lập phiếu lĩnh.
        /// </summary>
        private DateTime _MedicalInstructionDate;
        [DataMemberAttribute]
        public DateTime MedicalInstructionDate
        {
            get
            {
                return _MedicalInstructionDate;
            }
            set
            {
                if (_MedicalInstructionDate != value)
                {
                    _MedicalInstructionDate = value;
                    RaisePropertyChanged("MedicalInstructionDate");
                }
            }
        }

        private long? _GenMedVersionID;
        [DataMemberAttribute]
        public long? GenMedVersionID
        {
            get
            {
                return _GenMedVersionID;
            }
            set
            {
                if (_GenMedVersionID != value)
                {
                    _GenMedVersionID = value;
                    RaisePropertyChanged("GenMedVersionID");
                }

            }
        }
        //▼====: #008
        private string _Notes;
        [DataMemberAttribute()]
        public string Notes
        {
            get
            {
                return _Notes;
            }
            set
            {
                _Notes = value;
                RaisePropertyChanged("Notes");
            }
        }
        //▲====: #008
        //▼====: #009
        private long _inviID;
        [DataMemberAttribute()]
        public long inviID
        {
            get
            {
                return _inviID;
            }
            set
            {
                _inviID = value;
                RaisePropertyChanged("inviID");
            }
        }
        private string _InvID;
        [DataMemberAttribute()]
        public string InvID
        {
            get
            {
                return _InvID;
            }
            set
            {
                _InvID = value;
                RaisePropertyChanged("InvID");
            }
        }
        //▲====: #009

        //▼===== #010
        private int? _PartVT;
        [DataMemberAttribute()]
        public int? PartVT
        {
            get
            {
                return _PartVT;
            }
            set
            {
                _PartVT = value;
                RaisePropertyChanged("PartVT");
            }
        }

        private string _BinomialName;
        [DataMemberAttribute()]
        public string BinomialName
        {
            get
            {
                return _BinomialName;
            }
            set
            {
                _BinomialName = value;
                RaisePropertyChanged("BinomialName");
            }
        }

        private string _Origin;
        [DataMemberAttribute()]
        public string Origin
        {
            get
            {
                return _Origin;
            }
            set
            {
                _Origin = value;
                RaisePropertyChanged("Origin");
            }
        }

        private long _V_DrugFormulationMethod;
        [DataMemberAttribute()]
        public long V_DrugFormulationMethod
        {
            get
            {
                return _V_DrugFormulationMethod;
            }
            set
            {
                _V_DrugFormulationMethod = value;
                RaisePropertyChanged("V_DrugFormulationMethod");
            }
        }

        private string _PharmaceuticalCode;
        [DataMemberAttribute()]
        public string PharmaceuticalCode
        {
            get
            {
                return _PharmaceuticalCode;
            }
            set
            {
                _PharmaceuticalCode = value;
                RaisePropertyChanged("PharmaceuticalCode");
            }
        }

        private double? _AverageAttritionRate;
        [DataMemberAttribute()]
        public double? AverageAttritionRate
        {
            get
            {
                return _AverageAttritionRate;
            }
            set
            {
                _AverageAttritionRate = value;
                RaisePropertyChanged("AverageAttritionRate");
            }
        }

        private double? _BalancedAttritionRate;
        [DataMemberAttribute()]
        public double? BalancedAttritionRate
        {
            get
            {
                return _BalancedAttritionRate;
            }
            set
            {
                _BalancedAttritionRate = value;
                RaisePropertyChanged("BalancedAttritionRate");
            }
        }

        private long _V_PaymentSource;
        [DataMemberAttribute()]
        public long V_PaymentSource
        {
            get
            {
                return _V_PaymentSource;
            }
            set
            {
                _V_PaymentSource = value;
                RaisePropertyChanged("V_PaymentSource");
            }
        }
        //▲===== #010

        private string _ModifiedLog;
        [DataMemberAttribute()]
        public string ModifiedLog
        {
            get
            {
                return _ModifiedLog;
            }
            set
            {
                _ModifiedLog = value;
                RaisePropertyChanged("ModifiedLog");
            }
        }

        public override string ToString()
        {
            return string.Format("[{0}] {1}", GenMedProductID, BrandName);
        }
    }
    
    public partial class RefGenMedProductSimple : NotifyChangedBase
    {
        #region Factory Method


        /// Create a new RefGenericDrugDetail object.

        /// <param name="drugID">Initial value of the DrugID property.</param>
        /// <param name="brandName">Initial value of the BrandName property.</param>
        /// <param name="genericName">Initial value of the GenericName property.</param>
        public static RefGenMedProductSimple CreateRefGenMedProductSimple(long drugID)
        {
            RefGenMedProductSimple refGenericDrugDetail = new RefGenMedProductSimple();
            refGenericDrugDetail.GenMedProductID = drugID;
            return refGenericDrugDetail;
        }

        #endregion

        private Int64 _GenMedProductID;
        [DataMemberAttribute()]
        public Int64 GenMedProductID
        {
            get
            {
                return _GenMedProductID;
            }
            set
            {
                if (_GenMedProductID != value)
                {
                    OnGenMedProductIDChanging(value);
                    _GenMedProductID = value;
                    RaisePropertyChanged("GenMedProductID");
                }
            }
        }
        partial void OnGenMedProductIDChanging(Int64 value);
        partial void OnGenMedProductIDChanged();

        [DataMemberAttribute()]
        public String BrandName
        {
            get
            {
                return _BrandName;
            }
            set
            {
                if (_BrandName != value)
                {
                    OnBrandNameChanging(value);
                    ValidateProperty("BrandName", value);
                    _BrandName = value;
                    RaisePropertyChanged("BrandName");
                    OnBrandNameChanged();
                }
            }
        }
        private String _BrandName;
        partial void OnBrandNameChanging(String value);
        partial void OnBrandNameChanged();

        [DataMemberAttribute()]
        public String GenericName
        {
            get
            {
                return _GenericName;
            }
            set
            {
                OnGenericNameChanging(value);
                ValidateProperty("GenericName", value);
                _GenericName = value;
                RaisePropertyChanged("GenericName");
                OnGenericNameChanged();
            }
        }
        private String _GenericName;
        partial void OnGenericNameChanging(String value);
        partial void OnGenericNameChanged();

        [DataMemberAttribute()]
        public String Code
        {
            get
            {
                return _Code;
            }
            set
            {
                OnCodeChanging(value);
                _Code = value;
                RaisePropertyChanged("Code");
                OnCodeChanged();
            }
        }
        private String _Code;
        partial void OnCodeChanging(String value);
        partial void OnCodeChanged();

        [DataMemberAttribute()]
        public String UnitName
        {
            get
            {
                return _UnitName;
            }
            set
            {
                _UnitName = value;
                RaisePropertyChanged("UnitName");
            }
        }
        private String _UnitName;

        [DataMemberAttribute()]
        public long V_MedProductType
        {
            get
            {
                return _V_MedProductType;
            }
            set
            {
                OnV_MedProductTypeChanging(value);
                _V_MedProductType = value;
                RaisePropertyChanged("V_MedProductType");
                OnV_MedProductTypeChanged();
            }
        }
        private long _V_MedProductType;
        partial void OnV_MedProductTypeChanging(long value);
        partial void OnV_MedProductTypeChanged();

        private long? _RefGenDrugCatID_1;
        [DataMemberAttribute()]
        public long? RefGenDrugCatID_1
        {
            get
            {
                return _RefGenDrugCatID_1;
            }
            set
            {
                _RefGenDrugCatID_1 = value;
                RaisePropertyChanged("RefGenDrugCatID_1");
            }
        }

        private long? _RefGenDrugCatID_2;
        [DataMemberAttribute()]
        public long? RefGenDrugCatID_2
        {
            get
            {
                return _RefGenDrugCatID_2;
            }
            set
            {
                _RefGenDrugCatID_2 = value;
                RaisePropertyChanged("RefGenDrugCatID_2");
            }
        }


        //dung thao tac tren man hinh thoi
        [DataMemberAttribute()]
        public decimal RequestQty
        {
            get
            {
                return _RequestQty;
            }
            set
            {
                if (_RequestQty != value)
                {
                    _RequestQty = value;
                    RaisePropertyChanged("RequestQty");
                }
            }
        }
        private decimal _RequestQty;

        private string _ProductCodeRefNum;
        [DataMemberAttribute()]
        public string ProductCodeRefNum
        {
            get
            {
                return _ProductCodeRefNum;
            }
            set
            {
                _ProductCodeRefNum = value;
                RaisePropertyChanged("ProductCodeRefNum");
            }
        }

        private string _Administration;
        [DataMemberAttribute()]
        public string Administration
        {
            get
            {
                return _Administration;
            }
            set
            {
                _Administration = value;
                RaisePropertyChanged("Administration");
            }
        }
        
        public override bool Equals(object obj)
        {
            RefGenMedProductSimple seletedDrug = obj as RefGenMedProductSimple;
            if (seletedDrug == null)
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;
            return (this.GenMedProductID > 0 && this.GenMedProductID == seletedDrug.GenMedProductID);
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
