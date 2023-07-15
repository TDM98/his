using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;
using Service.Core.Common;
using System.ComponentModel.DataAnnotations;

namespace DataEntities
{
    public partial class DrugDeptPurchaseOrderDetail : EntityBase
    {
        #region Factory Method


        /// Create a new DrugDeptPurchaseOrderDetail object.

        /// <param name="DrugDeptPoDetailID">Initial value of the DrugDeptPoDetailID property.</param>
        /// <param name="GenMedProductID">Initial value of the GenMedProductID property.</param>
        /// <param name="PoUnitQty">Initial value of the PoUnitQty property.</param>
        public static DrugDeptPurchaseOrderDetail CreateDrugDeptPurchaseOrderDetail(Int64 DrugDeptPoDetailID, Int64 GenMedProductID, Int32 PoUnitQty)
        {
            DrugDeptPurchaseOrderDetail DrugDeptPurchaseOrderDetail = new DrugDeptPurchaseOrderDetail();
            DrugDeptPurchaseOrderDetail.DrugDeptPoDetailID = DrugDeptPoDetailID;
            DrugDeptPurchaseOrderDetail.GenMedProductID = GenMedProductID;
            DrugDeptPurchaseOrderDetail.PoUnitQty = PoUnitQty;
            return DrugDeptPurchaseOrderDetail;
        }

        #endregion

        #region Primitive Properties

        [DataMemberAttribute()]
        public Int64 DrugDeptPoDetailID
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
        private Int64 _DrugDeptPoDetailID;
        partial void OnDrugDeptPoDetailIDChanging(Int64 value);
        partial void OnDrugDeptPoDetailIDChanged();

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
        public Int64 GenMedProductID
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
        private Int64 _GenMedProductID;
        partial void OnGenMedProductIDChanging(Int64 value);
        partial void OnGenMedProductIDChanged();


        [DataMemberAttribute()]
        public Int32 PoUnitQty
        {
            get
            {
                return _PoUnitQty;
            }
            set
            {
                if (_PoUnitQty != value)
                {
                    OnPoUnitQtyChanging(value);
                    _PoUnitQty = value;
                    RaisePropertyChanged("PoUnitQty");
                    //if (RefGenMedProductDetail != null)
                    //{
                    //    if (RefGenMedProductDetail.UnitPackaging > 0)
                    //    {
                    //        _PoPackageQty = _PoUnitQty / RefGenMedProductDetail.UnitPackaging.GetValueOrDefault(1);
                    //    }
                    //    else
                    //    {
                    //        _PoPackageQty = _PoUnitQty;
                    //    }
                    //    RaisePropertyChanged("PoPackageQty");
                    //}
                    RaisePropertyChanged("PoTotalPriceNotVAT");
                    OnPoUnitQtyChanged();
                }
            }
        }
        private Int32 _PoUnitQty;
        partial void OnPoUnitQtyChanging(Int32 value);
        partial void OnPoUnitQtyChanged();

        [DataMemberAttribute()]
        public double PoPackageQty
        {
            get
            {
                return _PoPackageQty;
            }
            set
            {
                if (_PoPackageQty != value)
                {
                    OnPoPackageQtyChanging(value);
                    _PoPackageQty = value;
                    RaisePropertyChanged("PoPackageQty");
                    //if (RefGenMedProductDetail != null)
                    //{
                    //    _PoUnitQty = _PoPackageQty * RefGenMedProductDetail.UnitPackaging.GetValueOrDefault(1);
                    //    RaisePropertyChanged("PoUnitQty");
                    //}
                    OnPoPackageQtyChanged();
                }
            }
        }
        private double _PoPackageQty;
        partial void OnPoPackageQtyChanging(double value);
        partial void OnPoPackageQtyChanged();

        [DataMemberAttribute()]
        public Int32 EstimateQty
        {
            get
            {
                return _EstimateQty;
            }
            set
            {
                _EstimateQty = value;
                RaisePropertyChanged("EstimateQty");
            }
        }
        private Int32 _EstimateQty;

        [DataMemberAttribute()]
        public Int32 WaitingDeliveryQty
        {
            get
            {
                return _WaitingDeliveryQty;
            }
            set
            {
                OnWaitingDeliveryQtyChanging(value);
                _WaitingDeliveryQty = value;
                RaisePropertyChanged("WaitingDeliveryQty");
                RaisePropertyChanged("CanWaiting");
                OnWaitingDeliveryQtyChanged();
            }
        }
        private Int32 _WaitingDeliveryQty;
        partial void OnWaitingDeliveryQtyChanging(Int32 value);
        partial void OnWaitingDeliveryQtyChanged();


        [DataMemberAttribute()]
        public Int32 OrderedQty
        {
            get
            {
                return _OrderedQty;
            }
            set
            {
                _OrderedQty = value;
                RaisePropertyChanged("OrderedQty");
            }
        }
        private Int32 _OrderedQty;

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
                    OnUnitPriceChanging(value);
                    _UnitPrice = value;

                    RaisePropertyChanged("UnitPrice");
                    //if (RefGenMedProductDetail != null)
                    //{
                    //    _PackagePrice = _UnitPrice * RefGenMedProductDetail.UnitPackaging.GetValueOrDefault(0);
                    //    RaisePropertyChanged("PackagePrice");
                    //}
                    ////TotalPriceNotVAT = (decimal)_InQuantity * _UnitPrice;
                    ////RaisePropertyChanged("TotalPriceNotVAT");
                    RaisePropertyChanged("PoTotalPriceNotVAT");
                    OnUnitPriceChanged();
                }
            }
        }
        private decimal _UnitPrice;
        partial void OnUnitPriceChanging(decimal value);
        partial void OnUnitPriceChanged();

        /// <summary>
        /// Gia Da Bao Gom Thue VAT
        /// VuTTM
        /// </summary>
        [DataMemberAttribute()]
        public decimal PriceIncludeVAT
        {
            get
            {
                return _PriceIncludeVAT;
            }
            set
            {
                if (_PriceIncludeVAT != value)
                {
                    OnPriceIncludeVATChanging(value);
                    _PriceIncludeVAT = value;

                    RaisePropertyChanged("PriceIncludeVAT");
                    OnPriceIncludeVATChanged();
                }
            }
        }
        private decimal _PriceIncludeVAT;
        partial void OnPriceIncludeVATChanging(decimal value);
        partial void OnPriceIncludeVATChanged();

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
                    OnPackagePriceChanging(value);
                    _PackagePrice = value;
                  
                    //if (RefGenMedProductDetail != null && IsUnitPackage.GetValueOrDefault())
                    //{
                    //    if (RefGenMedProductDetail.UnitPackaging > 0)
                    //    {
                    //        _UnitPrice = _PackagePrice / RefGenMedProductDetail.UnitPackaging.GetValueOrDefault(1);
                    //    }
                    //    else
                    //    {
                    //        _UnitPrice = _PackagePrice;
                    //    }
                    //    RaisePropertyChanged("UnitPrice");
                       
                    //}
                    RaisePropertyChanged("PackagePrice");
                    OnPackagePriceChanged();
                }
            }
        }
        private decimal _PackagePrice;
        partial void OnPackagePriceChanging(decimal value);
        partial void OnPackagePriceChanged();

        [DataMemberAttribute()]
        public string PoNotes
        {
            get
            {
                return _PoNotes;
            }
            set
            {
                OnPoNotesChanging(value);
                _PoNotes = value;
                RaisePropertyChanged("PoNotes");
                OnPoNotesChanged();
            }
        }
        private string _PoNotes;
        partial void OnPoNotesChanging(string value);
        partial void OnPoNotesChanged();

        [DataMemberAttribute()]
        public long DrugDeptEstPoDetailID
        {
            get
            {
                return _DrugDeptEstPoDetailID;
            }
            set
            {
                _DrugDeptEstPoDetailID = value;
                RaisePropertyChanged("DrugDeptEstPoDetailID");
            }
        }
        private long _DrugDeptEstPoDetailID;

        
        [DataMemberAttribute()]
        public bool IsCheckedForDel
        {
            get { return _isCheckedForDel; }
            set
            {
                _isCheckedForDel = value;
                RaisePropertyChanged("IsCheckedForDel");
            }
        }
        private bool _isCheckedForDel = false;

        #endregion

        public decimal PoTotalPriceNotVAT
        {
            get { return (decimal)PoUnitQty * UnitPrice; }
        }


        #region Navigation Properties

        [DataMemberAttribute()]
        public RefGenMedProductDetails RefGenMedProductDetail
        {
            get
            {
                return _RefGenMedProductDetail;
            }
            set
            {
                if (_RefGenMedProductDetail != value)
                {
                    _RefGenMedProductDetail = value;
                    if (_RefGenMedProductDetail != null)
                    {
                        GenMedProductID = _RefGenMedProductDetail.GenMedProductID;
                    }
                    else
                    {
                        GenMedProductID = 0;
                    }
                    RaisePropertyChanged("GenMedProductID");
                    RaisePropertyChanged("RefGenMedProductDetail");
                }
            }
        }
        private RefGenMedProductDetails _RefGenMedProductDetail;

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

        #region Extention Member

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

        [DataMemberAttribute()]
        public Double InQuantity
        {
            get
            {
                return _InQuantity;
            }
            set
            {
                OnInQuantityChanging(value);
                _InQuantity = value;
                RaisePropertyChanged("InQuantity");
                //TotalPriceNotVAT = (decimal)_InQuantity * _UnitPrice;
                //RaisePropertyChanged("TotalPriceNotVAT");
                OnInQuantityChanged();
            }
        }
        private Double _InQuantity;
        partial void OnInQuantityChanging(Double value);
        partial void OnInQuantityChanged();

        //KMx: Không sử dụng InQty nữa, vì khi set vào InQty thì nó tự động set InQuantity. Bây giờ tính toán ở Database rồi set thẳng vào InQuantity luôn (04/12/2014 10:47).
        //[DataMemberAttribute()]
        //public int InQty
        //{
        //    get
        //    {
        //        return _InQty;
        //    }
        //    set
        //    {
        //        _InQty = value;
        //        RaisePropertyChanged("InQty");
        //        InQuantity = (double)(_PoUnitQty - _InQty);
        //        RaisePropertyChanged("InQuantity");
        //    }
        //}
        //private int _InQty;

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
        public Nullable<DateTime> InProductionDate
        {
            get
            {
                return _InProductionDate;
            }
            set
            {
                OnInProductionDateChanging(value);
                _InProductionDate = value;
                RaisePropertyChanged("InProductionDate");
                OnInProductionDateChanged();
            }
        }
        private Nullable<DateTime> _InProductionDate;
        partial void OnInProductionDateChanging(Nullable<DateTime> value);
        partial void OnInProductionDateChanged();

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
                _InExpiryDate = value;
                RaisePropertyChanged("InExpiryDate");
                OnInExpiryDateChanged();
            }
        }
        private Nullable<DateTime> _InExpiryDate;
        partial void OnInExpiryDateChanging(Nullable<DateTime> value);
        partial void OnInExpiryDateChanged();

        [DataMemberAttribute()]
        public Lookup V_GoodsType
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
        private Lookup _V_GoodsType;

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

        public static ValidationResult ValidateProductionDate(DateTime? value, ValidationContext context)
        {
            if (value == null)
            {
                return new ValidationResult("Vui lòng nhập ngày sản xuất", new string[] { "InProductionDate" });
            }
            if (value > DateTime.Now)
            {
                return new ValidationResult("Ngày sản xuất không hợp lệ.Ngày sản xuất phải nhỏ hơn ngày hiện tại", new string[] { "InProductionDate" });
            }
            return ValidationResult.Success;
        }

        public static ValidationResult ValidateExpiryDate(DateTime? value, ValidationContext context)
        {
            if (value == null)
            {
                return new ValidationResult("Vui lòng nhập ngày hết hạn", new string[] { "InExpiryDate" });
            }
            if (value < DateTime.Now)
            {
                return new ValidationResult("Ngày hết hạn phải lớn hơn ngày hiện tại", new string[] { "InExpiryDate" });
            }
            return ValidationResult.Success;
        }


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
        public decimal Discounting
        {
            get
            {
                return _Discounting;
            }
            set
            {
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
        private decimal _DiscountingByPercent;

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
                RaisePropertyChanged("IsPercent");
                //if (_IsPercent)
                //{
                //    Discounting = (_DiscountingByPercent - 1) * _TotalPriceNotVAT;
                //    RaisePropertyChanged("Discounting");
                //}
                //else
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

        #endregion

        [DataMemberAttribute()]
        public Nullable<Boolean> NoWaiting
        {
            get
            {
                return _NoWaiting;
            }
            set
            {
                _NoWaiting = value;
                RaisePropertyChanged("NoWaiting");
                RaisePropertyChanged("CanWaiting");
            }
        }
        private Nullable<Boolean> _NoWaiting;

        public bool CanWaiting
        {
            get { return (WaitingDeliveryQty > 0 && NoWaiting != true && DrugDeptPoDetailID > 0); }
        }

        private string _strVAT = "";
        [DataMemberAttribute()]
        public string StrVAT
        {
            get
            {
                return _strVAT;
            }
            set
            {
                _strVAT = value;
                _VAT = 0;
                if (_strVAT != null && _strVAT.Length > 0)
                {                    
                    try
                    {
                        if (_strVAT.IndexOf(".") == 0) // example when user types .05 then make it 0.05
                        {
                            _strVAT = "0" + _strVAT;
                        }
                        _VAT = Convert.ToDouble(_strVAT);
                    }
                    catch(Exception ex)
                    {
                    }
                }
                RaisePropertyChanged("StrVAT");
            }
        }

        private double? _VAT;
        [DataMemberAttribute()]
        public double? VAT
        {
            get
            {
                return _VAT;
            }
            set
            {
                _VAT = value;
                _strVAT = "";
                if (VAT.HasValue)
                {
                    _strVAT = _VAT.ToString();
                }
                RaisePropertyChanged("StrVAT");
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
