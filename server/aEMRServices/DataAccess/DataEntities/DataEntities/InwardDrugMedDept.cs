using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;
using System.ComponentModel.DataAnnotations;
using Service.Core.Common;

namespace DataEntities
{
    [KnownType(typeof(IChargeableItemPrice))]
    public partial class InwardDrugMedDept : EntityBase, IChargeableItemPrice
    {
        #region Factory Method


        /// Create a new InwardDrug object.

        /// <param name="inID">Initial value of the InID property.</param>
        /// <param name="inExpiryDate">Initial value of the InExpiryDate property.</param>
        /// <param name="inQuantity">Initial value of the InQuantity property.</param>
        /// <param name="inBuyingPrice">Initial value of the InBuyingPrice property.</param>
        /// <param name="inNormalSellingPrice">Initial value of the InNormalSellingPrice property.</param>
        /// <param name="inSoldout">Initial value of the InSoldout property.</param>
        public static InwardDrugMedDept CreateInwardDrugMedDept(long inID, decimal inQuantity, Decimal inBuyingPrice, Decimal inSellingPriceResident, Decimal inSellingPriceNonResident, string InbatchNumber)
        {
            InwardDrugMedDept inwardDrug = new InwardDrugMedDept();
            inwardDrug.InID = inID;
            inwardDrug.InBatchNumber = InbatchNumber;
            inwardDrug.InQuantity = inQuantity;
            inwardDrug.InBuyingPrice = inBuyingPrice;
            return inwardDrug;
        }

        #endregion

        #region Primitive Properties

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
        public Nullable<long> SdlID
        {
            get
            {
                return _SdlID;
            }
            set
            {
                OnSdlIDChanging(value);
                _SdlID = value;
                RaisePropertyChanged("SdlID");
                OnSdlIDChanged();
            }
        }
        private Nullable<long> _SdlID;
        partial void OnSdlIDChanging(Nullable<long> value);
        partial void OnSdlIDChanged();

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
        public long inviID
        {
            get
            {
                return _inviID;
            }
            set
            {
                OninviIDChanging(value);
                _inviID = value;
                RaisePropertyChanged("inviID");
                OninviIDChanged();
            }
        }
        private long _inviID;
        partial void OninviIDChanging(long value);
        partial void OninviIDChanged();

        [DataMemberAttribute()]
        public Nullable<long> GenMedProductID
        {
            get
            {
                return _GenMedProductID;
            }
            set
            {
                OnGenMedProductIDChanging(value);
                _GenMedProductID = value;
                RaisePropertyChanged("GenMedProductID");
                OnGenMedProductIDChanged();
            }
        }
        private Nullable<long> _GenMedProductID;
        partial void OnGenMedProductIDChanging(Nullable<long> value);
        partial void OnGenMedProductIDChanged();

        [CustomValidation(typeof(InwardDrugMedDept), "ValidateBatchNumber")]
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
                ValidateProperty("InBatchNumber", value);
                _InBatchNumber = value;
                RaisePropertyChanged("InBatchNumber");
                OnInBatchNumberChanged();
            }
        }
        private String _InBatchNumber;
        partial void OnInBatchNumberChanging(String value);
        partial void OnInBatchNumberChanged();

        [CustomValidation(typeof(InwardDrugMedDept), "ValidateProductionDate")]
        [DataMemberAttribute()]
        public Nullable<DateTime> InProductionDate
        {
            get
            {
                return _InProductionDate;
            }
            set
            {
                OnInProductionDateChanging(value);
                ValidateProperty("InProductionDate", value);
                _InProductionDate = value;
                RaisePropertyChanged("InProductionDate");
                OnInProductionDateChanged();
            }
        }
        private Nullable<DateTime> _InProductionDate;
        partial void OnInProductionDateChanging(Nullable<DateTime> value);
        partial void OnInProductionDateChanged();

        [CustomValidation(typeof(InwardDrugMedDept), "ValidateExpiryDate")]
        [DataMemberAttribute()]
        public Nullable<DateTime> InExpiryDate
        {
            get
            {
                return _InExpiryDate;
            }
            set
            {
                OnInExpiryDateChanging(value);
                ValidateProperty("InExpiryDate", value);
                _InExpiryDate = value;
                RaisePropertyChanged("InExpiryDate");
                OnInExpiryDateChanged();
            }
        }
        private Nullable<DateTime> _InExpiryDate;
        partial void OnInExpiryDateChanging(Nullable<DateTime> value);
        partial void OnInExpiryDateChanged();

        //[CustomValidation(typeof(InwardDrugMedDept), "ValidateQuantityInputFromMedDept")]
        [DataMemberAttribute()]
        public decimal InQuantity
        {
            get
            {
                return _InQuantity;
            }
            set
            {
                OnInQuantityChanging(value);
                ValidateProperty("InQuantity", value);
                _InQuantity = value;
                RaisePropertyChanged("InQuantity");
                //TotalPriceNotVAT = (decimal)_InQuantity * _InBuyingPrice;
                //RaisePropertyChanged("TotalPriceNotVAT");
                OnInQuantityChanged();
            }
        }
        private decimal _InQuantity;
        partial void OnInQuantityChanging(decimal value);
        partial void OnInQuantityChanged();

        [DataMemberAttribute()]
        public Double PackageQuantity
        {
            get
            {
                return _PackageQuantity;
            }
            set
            {
                _PackageQuantity = value;
                RaisePropertyChanged("PackageQuantity");
            }
        }
        private Double _PackageQuantity;

        [Range(0.0, 99999999999.0, ErrorMessage = "Giá mua không được nhỏ hơn 0")]
        [DataMemberAttribute()]
        public Decimal InBuyingPrice
        {
            get
            {
                return _InBuyingPrice;
            }
            set
            {
                if (_InBuyingPrice != value)
                {
                    OnInBuyingPriceChanging(value);
                    ValidateProperty("InBuyingPrice", value);
                    _InBuyingPrice = value;
                    RemainTotalSell = _remaining * _InBuyingPrice;
                    RaisePropertyChanged("RemainTotalSell");

                    RaisePropertyChanged("InBuyingPrice");
                    //TotalPriceNotVAT = (decimal)_InQuantity * _InBuyingPrice;
                    //RaisePropertyChanged("TotalPriceNotVAT");
                    OnInBuyingPriceChanged();

                }
            }
        }
        private Decimal _InBuyingPrice;
        partial void OnInBuyingPriceChanging(Decimal value);
        partial void OnInBuyingPriceChanged();

        [DataMemberAttribute()]
        public Decimal TotalPriceNotVAT
        {
            get
            {
                return _TotalPriceNotVAT;
            }
            set
            {
                _TotalPriceNotVAT = value;
                RaisePropertyChanged("TotalPriceNotVAT");
            }
        }
        private Decimal _TotalPriceNotVAT;


        [DataMemberAttribute()]
        public Decimal InCost
        {
            get
            {
                return _InCost;
            }
            set
            {
                _InCost = value;
                RaisePropertyChanged("InCost");
            }
        }
        private Decimal _InCost;

        [DataMemberAttribute()]
        public decimal PackagePrice
        {
            get
            {
                return _PackagePrice;
            }
            set
            {
                OnPackagePriceChanging(value);
                _PackagePrice = value;
                RaisePropertyChanged("PackagePrice");
                OnPackagePriceChanged();
            }
        }
        private decimal _PackagePrice;
        partial void OnPackagePriceChanging(decimal value);
        partial void OnPackagePriceChanged();

        [DataMemberAttribute()]
        public Decimal InBuyingPriceActual
        {
            get
            {
                return _InBuyingPriceActual;
            }
            set
            {
                if (_InBuyingPriceActual != value)
                {
                    _InBuyingPriceActual = value;
                    RaisePropertyChanged("InBuyingPriceActual");
                }
            }
        }
        private Decimal _InBuyingPriceActual;

        [DataMemberAttribute()]
        public decimal Remaining
        {
            get
            {
                return _remaining;
            }
            set
            {
                OnRemainingChanging(value);
                _remaining = value;
                RemainTotalSell = (decimal)_remaining * _InBuyingPrice;
                RaisePropertyChanged("RemainTotalSell");
                RaisePropertyChanged("Remaining");
                OnRemainingChanged();

            }
        }
        private decimal _remaining;
        partial void OnRemainingChanging(decimal value);
        partial void OnRemainingChanged();

        [DataMemberAttribute()]
        public Int16 IsLoad
        {
            get
            {
                return _IsLoad;
            }
            set
            {
                _IsLoad = value;
                RaisePropertyChanged("IsLoad");
            }
        }
        private Int16 _IsLoad;


        [DataMemberAttribute()]
        public Nullable<Boolean> IsPercentage
        {
            get
            {
                return _IsPercentage;
            }
            set
            {
                _IsPercentage = value;
                RaisePropertyChanged("IsPercentage");
            }
        }
        private Nullable<Boolean> _IsPercentage;

        [DataMemberAttribute()]
        public Nullable<Int64> DrugDeptPoDetailID
        {
            get
            {
                return _DrugDeptPoDetailID;
            }
            set
            {
                if (_DrugDeptPoDetailID != value)
                {
                    OnDrugDeptPoDetailIDChanging(value);
                    _DrugDeptPoDetailID = value;
                    RaisePropertyChanged("DrugDeptPoDetailID");
                    OnDrugDeptPoDetailIDChanged();
                }
            }
        }
        private Nullable<Int64> _DrugDeptPoDetailID;
        partial void OnDrugDeptPoDetailIDChanging(Nullable<Int64> value);
        partial void OnDrugDeptPoDetailIDChanged();

        private bool _isCheckedForDel = false;
        public bool IsCheckedForDel
        {
            get { return _isCheckedForDel; }
            set
            {
                _isCheckedForDel = value;
                RaisePropertyChanged("IsCheckedForDel");
            }
        }

        private int _DisplayGridRowNumber = 0;
        public int DisplayGridRowNumber
        {
            get { return _DisplayGridRowNumber; }
            set
            {
                _DisplayGridRowNumber = value;
                RaisePropertyChanged("DisplayGridRowNumber");
            }
        }

        private string _TempOutwardInvNum;
        [DataMemberAttribute()]
        public string TempOutwardInvNum
        {
            get { return _TempOutwardInvNum; }
            set
            {
                _TempOutwardInvNum = value;
                RaisePropertyChanged("TempOutwardInvNum");
            }
        }

        private long _GenMedVersionID;
        [DataMemberAttribute()]
        public long GenMedVersionID
        {
            get { return _GenMedVersionID; }
            set
            {
                _GenMedVersionID = value;
                RaisePropertyChanged("GenMedVersionID");
            }
        }
        #endregion

        #region Extension

        //so luong khoa phong xuat tra
        [DataMemberAttribute()]
        public decimal ClinicDeptQty
        {
            get
            {
                return _ClinicDeptQty;
            }
            set
            {

                _ClinicDeptQty = value;
                RaisePropertyChanged("ClinicDeptQty");
            }
        }
        private decimal _ClinicDeptQty;

        //Ma Dong Khoa Phong Xuat Tra
        [DataMemberAttribute()]
        public long? OutID
        {
            get
            {
                return _OutID;
            }
            set
            {
                if (_OutID != value)
                {
                    OnOutIDChanging(value);
                    _OutID = value;
                    RaisePropertyChanged("OutID");
                    OnOutIDChanged();
                }
            }
        }
        private long? _OutID;
        partial void OnOutIDChanging(long? value);
        partial void OnOutIDChanged();

        [DataMemberAttribute()]
        //ten kho de group cho bao cao thuoc het han dung
        public string swhlName
        {
            get
            {
                return _swhlName;
            }
            set
            {
                _swhlName = value;
                RaisePropertyChanged("swhlName");
            }
        }
        private string _swhlName;

        [DataMemberAttribute()]
        public Nullable<Int64> DrugDeptPoID
        {
            get
            {
                return _DrugDeptPoID;
            }
            set
            {
                OnDrugDeptPoIDChanging(value);
                _DrugDeptPoID = value;
                RaisePropertyChanged("DrugDeptPoID");
                OnDrugDeptPoIDChanged();
            }
        }
        private Nullable<Int64> _DrugDeptPoID;
        partial void OnDrugDeptPoIDChanging(Nullable<Int64> value);
        partial void OnDrugDeptPoIDChanged();

        [DataMemberAttribute()]
        public String PONumber
        {
            get
            {
                return _PONumber;
            }
            set
            {
                OnPONumberChanging(value);
                _PONumber = value;
                RaisePropertyChanged("PONumber");
                OnPONumberChanged();
            }
        }
        private String _PONumber;
        partial void OnPONumberChanging(String value);
        partial void OnPONumberChanged();

        [DataMemberAttribute()]
        public long V_GoodsType
        {
            get
            {
                return _V_GoodsType;
            }
            set
            {

                _V_GoodsType = value;
                RaisePropertyChanged("V_GoodsType");
            }
        }
        private long _V_GoodsType = 13001;


        [DataMemberAttribute()]
        public Lookup ObjV_GoodsType
        {
            get
            {
                return _ObjV_GoodsType;
            }
            set
            {
                OnObjV_GoodsTypeChanging(value);
                _ObjV_GoodsType = value;
                OnObjV_GoodsTypeChanged();
                RaisePropertyChanged("ObjV_GoodsType");
            }
        }
        private Lookup _ObjV_GoodsType;
        partial void OnObjV_GoodsTypeChanging(Lookup value);
        partial void OnObjV_GoodsTypeChanged();



        [DataMemberAttribute()]
        public string GoodsTypeName
        {
            get
            {
                return _GoodsTypeName;
            }
            set
            {

                _GoodsTypeName = value;
                RaisePropertyChanged("GoodsTypeName");
            }
        }
        private string _GoodsTypeName = "Hàng mua";

        [DataMemberAttribute()]
        public string SdlDescription
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
        private string _SdlDescription;

        [DataMemberAttribute()]
        public Decimal RemainTotalSell
        {
            get
            {
                return _RemainTotalSell;
            }
            set
            {
                _RemainTotalSell = value;
                RaisePropertyChanged("RemainTotalSell");
            }
        }
        private Decimal _RemainTotalSell;

        [Range(0.0, 99999999999.0, ErrorMessage = "Tiền chiết khấu không được < 0")]
        [DataMemberAttribute()]
        public decimal Discounting
        {
            get
            {
                return _Discounting;
            }
            set
            {
                ValidateProperty("Discounting", value);
                _Discounting = value;
                //if (!_IsPercent)
                //{
                //    if (_TotalPriceNotVAT != 0)
                //    {
                //        DiscountingByPercent = 1 + _Discounting / _TotalPriceNotVAT;
                //    }
                //    else
                //    {
                //        DiscountingByPercent = 1 + (_Discounting / (decimal)0.1);
                //    }
                //    RaisePropertyChanged("DiscountingByPercent");
                //}
                RaisePropertyChanged("Discounting");
            }
        }
        private decimal _Discounting;

        [DataMemberAttribute()]
        public decimal DiscountingByPercent
        {
            get
            {
                return _DiscountingByPercent;
            }
            set
            {
                _DiscountingByPercent = value;
                //if (_IsPercent)
                //{
                //    if (_DiscountingByPercent != 0)
                //    {
                //        Discounting = (_DiscountingByPercent - 1) * _TotalPriceNotVAT;
                //    }
                //    else
                //    {
                //        Discounting = 0;
                //    }
                //    RaisePropertyChanged("Discounting");
                //}
                RaisePropertyChanged("DiscountingByPercent");
            }
        }
        private decimal _DiscountingByPercent = 1;

        [DataMemberAttribute()]
        public bool IsPercent
        {
            get
            {
                return _IsPercent;
            }
            set
            {
                _IsPercent = value;

                //if (_IsPercent)
                //{
                //    if (_DiscountingByPercent != 0)
                //    {
                //        Discounting = (_DiscountingByPercent - 1) * _TotalPriceNotVAT;
                //    }
                //    else
                //    {
                //        Discounting = 0;
                //    }
                //    RaisePropertyChanged("Discounting");
                //}
                //else
                //{
                //    if (_TotalPriceNotVAT != 0)
                //    {
                //        if (_TotalPriceNotVAT != 0)
                //        {
                //            DiscountingByPercent = 1 + (_Discounting / _TotalPriceNotVAT);
                //        }
                //        else
                //        {
                //            DiscountingByPercent = 1 + (_Discounting / (decimal)0.1);
                //        }
                //        RaisePropertyChanged("DiscountingByPercent");
                //    }
                //}
                RaisePropertyChanged("CanPercentIsEnable");
                RaisePropertyChanged("IsPercent");
            }
        }
        private bool _IsPercent;

        [DataMemberAttribute()]
        public Nullable<Boolean> NoPrint
        {
            get
            {
                return _NoPrint;
            }
            set
            {
                _NoPrint = value;
                RaisePropertyChanged("NoPrint");
            }
        }
        private Nullable<Boolean> _NoPrint;

        [DataMemberAttribute()]
        public Nullable<Boolean> IsUnitPackage
        {
            get
            {
                return _IsUnitPackage;
            }
            set
            {
                _IsUnitPackage = value;
                RaisePropertyChanged("IsUnitPackage");
            }
        }
        private Nullable<Boolean> _IsUnitPackage = false;

        public bool CanPercentIsEnable
        {
            get { return IsPercent; }
        }

        [DataMemberAttribute()]
        public bool IsCanEdit
        {
            get
            {
                return _IsCanEdit;
            }
            set
            {
                if (_IsCanEdit != value)
                {
                    _IsCanEdit = value;
                    RaisePropertyChanged("IsCanEdit");
                }
            }
        }
        private bool _IsCanEdit = true;
        #endregion

        #region Navigation Properties

        [DataMemberAttribute()]
        public InwardDrugMedDeptInvoice SelectedInwardDrugMedDeptInvoice
        {
            get
            {
                return _selectedInwardDrugMedDeptInvoice;
            }
            set
            {
                OnSelectedInwardInvoiceDrugChanging(value);
                _selectedInwardDrugMedDeptInvoice = value;
                OnSelectedInwardInvoiceDrugChanged();
                RaisePropertyChanged("SelectedInwardDrugMedDeptInvoice");
            }
        }
        private InwardDrugMedDeptInvoice _selectedInwardDrugMedDeptInvoice;
        partial void OnSelectedInwardInvoiceDrugChanging(InwardDrugMedDeptInvoice value);
        partial void OnSelectedInwardInvoiceDrugChanged();

        [DataMemberAttribute()]
        public RefShelfDrugLocation SelectedShelfDrugLocation
        {
            get
            {
                return _selectedShelfDrugLocation;
            }
            set
            {
                OnSelectedShelfDrugLocationChanging(value);
                _selectedShelfDrugLocation = value;
                if (_selectedShelfDrugLocation != null)
                {
                    _SdlID = _selectedShelfDrugLocation.SdlID;
                }
                else
                {
                    _SdlID = 0;
                }
                RaisePropertyChanged("SdlID");
                OnSelectedShelfDrugLocationChanged();
                RaisePropertyChanged("SelectedShelfDrugLocation");
            }
        }
        private RefShelfDrugLocation _selectedShelfDrugLocation;
        partial void OnSelectedShelfDrugLocationChanging(RefShelfDrugLocation value);
        partial void OnSelectedShelfDrugLocationChanged();

        [DataMemberAttribute()]
        public Staff SelectedStaffInput
        {
            get
            {
                return _selectedStaffInput;
            }
            set
            {
                OnSelectedStaffInputChanging(value);
                _selectedStaffInput = value;
                OnSelectedStaffInputChanged();
                RaisePropertyChanged("SelectedStaffInput");
            }
        }
        private Staff _selectedStaffInput;
        partial void OnSelectedStaffInputChanging(Staff value);
        partial void OnSelectedStaffInputChanged();

        [Required(ErrorMessage = "Vui lòng chọn thuốc")]
        [DataMemberAttribute()]
        public RefGenMedProductDetails RefGenMedProductDetails
        {
            get
            {
                return _RefGenMedProductDetails;
            }
            set
            {
                ValidateProperty("RefGenMedProductDetails", value);
                _RefGenMedProductDetails = value;
                RaisePropertyChanged("RefGenMedProductDetails");
            }
        }
        private RefGenMedProductDetails _RefGenMedProductDetails;

        public static ValidationResult ValidateProductionDate(DateTime? value, ValidationContext context)
        {
            if (value > DateTime.Now)
            {
                return new ValidationResult("Ngày sản xuất không hợp lệ.Ngày sản xuất phải nhỏ hơn ngày hiện tại", new string[] { "InProductionDate" });
            }
            return ValidationResult.Success;
        }

        public static ValidationResult ValidateExpiryDate(DateTime? value, ValidationContext context)
        {
            InwardDrugMedDept obj = context.ObjectInstance as InwardDrugMedDept;
            if (obj.RefGenMedProductDetails != null && obj.RefGenMedProductDetails.V_MedProductType == (long)AllLookupValues.MedProductType.THUOC)
            {
                if (value == null)
                {
                    return new ValidationResult("Vui lòng nhập ngày hết hạn", new string[] { "InExpiryDate" });
                }
            }
            if (value < DateTime.Now)
            {
                return new ValidationResult("Ngày hết hạn phải lớn hơn ngày hiện tại", new string[] { "InExpiryDate" });
            }
            return ValidationResult.Success;
        }

        public static ValidationResult ValidateBatchNumber(String value, ValidationContext context)
        {
            InwardDrugMedDept obj = context.ObjectInstance as InwardDrugMedDept;
            if (obj.RefGenMedProductDetails != null && obj.RefGenMedProductDetails.V_MedProductType == (long)AllLookupValues.MedProductType.THUOC)
            {
                if (String.IsNullOrWhiteSpace(value))
                {
                    return new ValidationResult("Vui lòng nhập số lô", new string[] { "InBatchNumber" });
                }
            }
            return ValidationResult.Success;
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

        private ChargeableItemType _ChargeableItemType;
        [DataMemberAttribute()]
        public ChargeableItemType ChargeableItemType
        {
            get
            {
                return DataEntities.ChargeableItemType.DRUGS;
            }
            set
            {
                _ChargeableItemType = value;
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

        private decimal _NormalPrice_Orig;
        [DataMemberAttribute()]
        public decimal NormalPrice_Orig
        {
            get
            {
                return _NormalPrice_Orig;
            }
            set
            {
                _NormalPrice_Orig = value;
                RaisePropertyChanged("NormalPrice_Orig");
            }
        }

        private decimal _HIPatientPrice_Orig;
        [DataMemberAttribute()]
        public decimal HIPatientPrice_Orig
        {
            get
            {
                return _HIPatientPrice_Orig;
            }
            set
            {
                _HIPatientPrice_Orig = value;
                RaisePropertyChanged("HIPatientPrice_Orig");
            }
        }

        private decimal? _HIAllowedPrice_Orig;
        [DataMemberAttribute()]
        public decimal? HIAllowedPrice_Orig
        {
            get
            {
                return _HIAllowedPrice_Orig;
            }
            set
            {
                _HIAllowedPrice_Orig = value;
                RaisePropertyChanged("HIAllowedPrice_Orig");
            }
        }


        #endregion

        private EntityState _EntityState = EntityState.NEW;
        [DataMemberAttribute()]
        public override EntityState EntityState
        {
            get
            {
                return _EntityState;
            }
            set
            {
                _EntityState = value;
                RaisePropertyChanged("EntityState");
            }
        }

        public static ValidationResult ValidateQuantityInputFromMedDept(decimal value, ValidationContext context)
        {
            if (value <= 0)
            {
                return new ValidationResult("Số lượng nhập vào phải > 0", new string[] { "InQuantity" });
            }
            if (((InwardDrugMedDept)context.ObjectInstance).ClinicDeptQty > 0)
            {
                if (value > ((InwardDrugMedDept)context.ObjectInstance).ClinicDeptQty)
                {
                    return new ValidationResult("Số lượng nhập không được > Số lượng khoa phòng xuất", new string[] { "InQuantity" });
                }
            }
            return ValidationResult.Success;
        }


        //Xuất Tạm cái này phải kết với InwardDrugMedDeptInvoice thì mới ra SupplierID nào
        private DrugDeptSupplier _ObjSupplierID;
        [DataMemberAttribute()]
        public DrugDeptSupplier ObjSupplierID
        {
            get
            {
                return _ObjSupplierID;
            }
            set
            {
                _ObjSupplierID = value;
                RaisePropertyChanged("ObjSupplierID");
            }
        }
        //Xuất Tạm cái này phải kết với InwardDrugMedDeptInvoice thì mới ra SupplierID nào

        private decimal _ReturnQuantity;
        [DataMemberAttribute]
        public decimal ReturnQuantity
        {
            get => _ReturnQuantity; set
            {
                _ReturnQuantity = value;
                RaisePropertyChanged("ReturnQuantity");
            }
        }

        private long? _DrugDeptInIDOrig;
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
        private decimal? _VAT;
        [DataMemberAttribute]
        public decimal? VAT
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
                RaisePropertyChanged("IsNotVat");
            }
        }
    }
}