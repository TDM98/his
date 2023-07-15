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
    public partial class PharmacyPurchaseOrderDetail : EntityBase
    {
        #region Factory Method


        /// Create a new PharmacyPurchaseOrderDetail object.

        /// <param name="PharmacyPoDetailID">Initial value of the PharmacyPoDetailID property.</param>
        /// <param name="DrugID">Initial value of the DrugID property.</param>
        /// <param name="PoUnitQty">Initial value of the PoUnitQty property.</param>
        public static PharmacyPurchaseOrderDetail CreatePharmacyPurchaseOrderDetail(Int64 PharmacyPoDetailID, Int64 DrugID, Int32 PoUnitQty)
        {
            PharmacyPurchaseOrderDetail PharmacyPurchaseOrderDetail = new PharmacyPurchaseOrderDetail();
            PharmacyPurchaseOrderDetail.PharmacyPoDetailID = PharmacyPoDetailID;
            PharmacyPurchaseOrderDetail.DrugID = DrugID;
            PharmacyPurchaseOrderDetail.PoUnitQty = PoUnitQty;
            return PharmacyPurchaseOrderDetail;
        }

        #endregion

        #region Primitive Properties

        [DataMemberAttribute()]
        public Int64 PharmacyPoDetailID
        {
            get
            {
                return _PharmacyPoDetailID;
            }
            set
            {
                if (_PharmacyPoDetailID != value)
                {
                    OnPharmacyPoDetailIDChanging(value);
                    _PharmacyPoDetailID = value;
                    RaisePropertyChanged("PharmacyPoDetailID");
                    OnPharmacyPoDetailIDChanged();
                }
            }
        }
        private Int64 _PharmacyPoDetailID;
        partial void OnPharmacyPoDetailIDChanging(Int64 value);
        partial void OnPharmacyPoDetailIDChanged();

        [DataMemberAttribute()]
        public Nullable<Int64> PharmacyPoID
        {
            get
            {
                return _PharmacyPoID;
            }
            set
            {
                OnPharmacyPoIDChanging(value);
                _PharmacyPoID = value;
                RaisePropertyChanged("PharmacyPoID");
                OnPharmacyPoIDChanged();
            }
        }
        private Nullable<Int64> _PharmacyPoID;
        partial void OnPharmacyPoIDChanging(Nullable<Int64> value);
        partial void OnPharmacyPoIDChanged();

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
        public String DrugCode
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
        private String _DrugCode;

        [DataMemberAttribute()]
        public Int64 DrugID
        {
            get
            {
                return _DrugID;
            }
            set
            {
                OnDrugIDChanging(value);
                _DrugID = value;
                RaisePropertyChanged("DrugID");
                OnDrugIDChanged();
            }
        }
        private Int64 _DrugID;
        partial void OnDrugIDChanging(Int64 value);
        partial void OnDrugIDChanged();


        [DataMemberAttribute()]
        public Int32 PoUnitQty
        {
            get
            {
                return _PoUnitQty;
            }
            set
            {
                OnPoUnitQtyChanging(value);
                _PoUnitQty = value;
                RaisePropertyChanged("PoUnitQty");
                RaisePropertyChanged("PoTotalPriceNotVAT");
                OnPoUnitQtyChanged();
            }
        }
        private Int32 _PoUnitQty;
        partial void OnPoUnitQtyChanging(Int32 value);
        partial void OnPoUnitQtyChanged();

        [DataMemberAttribute()]
        public Double PoPackageQty
        {
            get
            {
                return _PoPackageQty;
            }
            set
            {
                OnPoPackageQtyChanging(value);
                _PoPackageQty = value;
                RaisePropertyChanged("PoPackageQty");
                OnPoPackageQtyChanged();
            }
        }
        private Double _PoPackageQty;
        partial void OnPoPackageQtyChanging(Double value);
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
                OnUnitPriceChanging(value);
                _UnitPrice = value;
                RaisePropertyChanged("UnitPrice");
                //TotalPriceNotVAT = (decimal)_InQuantity * _UnitPrice;
                //RaisePropertyChanged("TotalPriceNotVAT");
                RaisePropertyChanged("PoTotalPriceNotVAT");
                OnUnitPriceChanged();
            }
        }
        private decimal _UnitPrice;
        partial void OnUnitPriceChanging(decimal value);
        partial void OnUnitPriceChanged();

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

        #endregion

        public decimal PoTotalPriceNotVAT
        {
            get { return (decimal)PoUnitQty * UnitPrice; }
        }

        #region Navigation Properties

        [DataMemberAttribute()]
        public RefGenericDrugDetail RefGenericDrugDetail
        {
            get
            {
                return _RefGenericDrugDetail;
            }
            set
            {
                if (_RefGenericDrugDetail != value)
                {
                    _RefGenericDrugDetail = value;
                    if (_RefGenericDrugDetail != null)
                    {
                        DrugID = _RefGenericDrugDetail.DrugID;
                    }
                    else
                    {
                        DrugID = 0;
                    }
                    RaisePropertyChanged("DrugID");
                    RaisePropertyChanged("RefGenericDrugDetail");
                }
            }
        }
        private RefGenericDrugDetail _RefGenericDrugDetail;

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

        [DataMemberAttribute()]
        public int InQty
        {
            get
            {
                return _InQty;
            }
            set
            {
                _InQty = value;
                RaisePropertyChanged("InQty");

                InQuantity = (double)(_PoUnitQty - _InQty);
                if (RefGenericDrugDetail != null)
                {
                    PackageQuantity = (double)(_PoUnitQty - _InQty) / RefGenericDrugDetail.UnitPackaging.GetValueOrDefault(1);
                    RaisePropertyChanged("PackageQuantity");
                }
                RaisePropertyChanged("InQuantity");
               
            }
        }
        private int _InQty;

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
                //        DiscountingByPercent = 1 + (_Discounting / (_TotalPriceNotVAT));
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
                //        Discounting = (_DiscountingByPercent - 1) * (_TotalPriceNotVAT);
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
                //if (_IsPercent)
                //{
                //    if (_DiscountingByPercent != 0)
                //    {
                //        Discounting = (_DiscountingByPercent - 1) * (_TotalPriceNotVAT);
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
                //        DiscountingByPercent = 1 + (_Discounting / (_TotalPriceNotVAT));
                //    }
                //    else
                //    {
                //        DiscountingByPercent = 1 + (_Discounting / (decimal)0.1);
                //    }
                //    RaisePropertyChanged("DiscountingByPercent");
                //}
                RaisePropertyChanged("IsPercent");

                //if (_IsPercent)
                //{
                //    Discounting = (_DiscountingByPercent -1) * _TotalPriceNotVAT;
                //    RaisePropertyChanged("Discounting");
                //}
                //else
                //{
                //    if (_TotalPriceNotVAT != 0)
                //    {
                //        DiscountingByPercent = 1 + _Discounting / _TotalPriceNotVAT;
                //        RaisePropertyChanged("DiscountingByPercent");
                //    }
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
        private Nullable<Boolean> _IsUnitPackage=false;

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
            get { return (WaitingDeliveryQty > 0 && NoWaiting !=true && PharmacyPoDetailID > 0); }
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
