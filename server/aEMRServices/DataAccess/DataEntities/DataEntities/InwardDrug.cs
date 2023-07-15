using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using Service.Core.Common;

namespace DataEntities
{
    [KnownType(typeof(IChargeableItemPrice))]
    public partial class InwardDrug : EntityBase, IEditableObject, IChargeableItemPrice
    {
        public InwardDrug()
            : base()
        {

        }

        private InwardDrug _tempInwardDrug;
        #region IEditableObject Members

        public void BeginEdit()
        {
            _tempInwardDrug = (InwardDrug)this.MemberwiseClone();
        }

        public void CancelEdit()
        {
            if (null != _tempInwardDrug)
                CopyFrom(_tempInwardDrug);
            //_tempPatient = null;
        }

        public void EndEdit()
        {
        }

        public void CopyFrom(InwardDrug p)
        {
            PropertyCopierHelper.CopyPropertiesTo(p, this);
        }

        #endregion

        #region Factory Method


        /// Create a new InwardDrug object.

        /// <param name="inID">Initial value of the InID property.</param>
        /// <param name="inExpiryDate">Initial value of the InExpiryDate property.</param>
        /// <param name="inQuantity">Initial value of the InQuantity property.</param>
        /// <param name="inBuyingPrice">Initial value of the InBuyingPrice property.</param>
        /// <param name="inNormalSellingPrice">Initial value of the InNormalSellingPrice property.</param>
        /// <param name="inSoldout">Initial value of the InSoldout property.</param>
        public static InwardDrug CreateInwardDrug(long inID, Double inQuantity, Decimal inBuyingPrice, Decimal inSellingPriceResident, Decimal inSellingPriceNonResident, string InbatchNumber)
        {
            InwardDrug inwardDrug = new InwardDrug();
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
        public Nullable<long> DrugID
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
        private Nullable<long> _DrugID;
        partial void OnDrugIDChanging(Nullable<long> value);
        partial void OnDrugIDChanged();

        [Required(ErrorMessage = "Vui lòng nhập số lô")]
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

        [CustomValidation(typeof(InwardDrug), "ValidateProductionDate")]
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

        [CustomValidation(typeof(InwardDrug), "ValidateExpiryDate")]
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

        [Range(0.0, 99999999999.0, ErrorMessage = "Số lượng không được nhỏ hơn 0")]
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
                ValidateProperty("InQuantity", value);
                _InQuantity = value;
                //TotalPriceNotVAT = (decimal)_InQuantity * _InBuyingPrice;
                RaisePropertyChanged("InQuantity");
                //RaisePropertyChanged("TotalPriceNotVAT");
                OnInQuantityChanged();
            }
        }
        private Double _InQuantity;
        partial void OnInQuantityChanging(Double value);
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
                    RemainTotalSell = (decimal)_remaining * _InBuyingPrice;
                    RaisePropertyChanged("RemainTotalSell");

                    RaisePropertyChanged("InBuyingPrice");
                    //TotalPriceNotVAT = (decimal)_InQuantity * _InBuyingPrice;
                    //RaisePropertyChanged("TotalPriceNotVAT");
                    //20190212 TBL: Khi sua Don gia le thi khong tu dong tinh thanh tien chua VAT
                    OnInBuyingPriceChanged();

                }
            }
        }
        private Decimal _InBuyingPrice;
        partial void OnInBuyingPriceChanging(Decimal value);
        partial void OnInBuyingPriceChanged();

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
        public Double Remaining
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
        private Double _remaining;
        partial void OnRemainingChanging(Double value);
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
        public Nullable<Int64> PharmacyPoDetailID
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
        private Nullable<Int64> _PharmacyPoDetailID;
        partial void OnPharmacyPoDetailIDChanging(Nullable<Int64> value);
        partial void OnPharmacyPoDetailIDChanged();

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

        private long _DrugVersionID;
        [DataMemberAttribute()]
        public long DrugVersionID
        {
            get { return _DrugVersionID; }
            set
            {
                _DrugVersionID = value;
                RaisePropertyChanged("DrugVersionID");
            }
        }
        private decimal _InCost;
        [DataMemberAttribute()]
        public decimal InCost
        {
            get { return _InCost; }
            set
            {
                _InCost = value;
                RaisePropertyChanged("InCost");
            }
        }
        #endregion

        #region Extension
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
                //        RaisePropertyChanged("DiscountingByPercent");
                //    }
                //    else
                //    {
                //        DiscountingByPercent = 1 + _Discounting / (decimal)0.1;
                //        RaisePropertyChanged("DiscountingByPercent");
                //    }
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
        public InwardDrugInvoice SelectedInwardInvoiceDrug
        {
            get
            {
                return _selectedInwardInvoiceDrug;
            }
            set
            {
                OnSelectedInwardInvoiceDrugChanging(value);
                _selectedInwardInvoiceDrug = value;
                OnSelectedInwardInvoiceDrugChanged();
                RaisePropertyChanged("SelectedInwardInvoiceDrug");
            }
        }
        private InwardDrugInvoice _selectedInwardInvoiceDrug;
        partial void OnSelectedInwardInvoiceDrugChanging(InwardDrugInvoice value);
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
        public RefGenericDrugDetail SelectedDrug
        {
            get
            {
                return _selectedDrug;
            }
            set
            {
                OnSelectedDrugChanging(value);
                ValidateProperty("SelectedDrug", value);
                _selectedDrug = value;
                OnSelectedDrugChanged();
                RaisePropertyChanged("SelectedDrug");
            }
        }
        private RefGenericDrugDetail _selectedDrug;
        partial void OnSelectedDrugChanging(RefGenericDrugDetail value);
        partial void OnSelectedDrugChanged();


        [DataMemberAttribute()]
        public ObservableCollection<OutwardDrug> OutwardDrugs
        {
            get;
            set;
        }

        public static ValidationResult ValidateProductionDate(DateTime? value, ValidationContext context)
        {
            if (AxHelper.CompareDate(value.GetValueOrDefault(),DateTime.Now)== 1)
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
            if (AxHelper.CompareDate(value.GetValueOrDefault(), DateTime.Now) == 2)
            {
                return new ValidationResult("Ngày hết hạn phải lớn hơn ngày hiện tại", new string[] { "InExpiryDate" });
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

        private Int16 _SellingPriceVATDef;
        [DataMemberAttribute]
        public Int16 SellingPriceVATDef
        {
            get
            {
                return _SellingPriceVATDef;
            }
            set
            {
                _SellingPriceVATDef = value;
                RaisePropertyChanged("SellingPriceVATDef");
            }
        }
        private decimal? _VAT;
        //[Range(0.0, 1.0, ErrorMessage = "VAT nằm trong khoảng 0 đến 1")]
        [DataMemberAttribute]
        public decimal? VAT
        {
            get
            {
                return _VAT;
            }
            set
            {
                _VAT = value;
                ValidateProperty("VAT", value);
                RaisePropertyChanged("VAT");
            }
        }
        private long _DrugDeptInIDOrig;
        [DataMemberAttribute]
        public long DrugDeptInIDOrig
        {
            get
            {
                return _DrugDeptInIDOrig;
            }
            set
            {
                _DrugDeptInIDOrig = value;
                RaisePropertyChanged("DrugDeptInIDOrig");
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
                _IsNotVat = value;
                ValidateProperty("IsNotVat", value);
                RaisePropertyChanged("IsNotVat");
            }
        }
        private long _OutID;
        [DataMemberAttribute]
        public long OutID
        {
            get
            {
                return _OutID;
            }
            set
            {
                _OutID = value;
                ValidateProperty("OutID", value);
                RaisePropertyChanged("OutID");
            }
        }
    }
}