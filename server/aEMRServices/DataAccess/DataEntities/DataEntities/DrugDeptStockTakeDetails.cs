using System;
using System.Runtime.Serialization;
using eHCMS.Services.Core.Base;
using Service.Core.Common;
using System.ComponentModel.DataAnnotations;

namespace DataEntities
{
    public partial class DrugDeptStockTakeDetails : EntityBase
    {
        #region Factory Method
        public static DrugDeptStockTakeDetails CreateDrugDeptStockTakeDetails(Int64 DrugDeptStockTakeDetailID, Int64 DrugDeptStockTakeID, Int64 GenMedProductID, Int32 outQtyPrevFirstMonth, Int32 outQtyPrevSecondMonth, Int32 outQtyPrevThirdMonth, Int32 outQtyPrevFourthMonth, Int32 CaculatedQty, Int32 ActualQty, Int32 adjustedQty, Int32 numberOfEstimatedMonths, Int32 outQtyLastTwelveMonths, Int32 toDateOutQty)
        {
            DrugDeptStockTakeDetails DrugDeptStockTakeDetails = new DrugDeptStockTakeDetails();
            DrugDeptStockTakeDetails.DrugDeptStockTakeDetailID = DrugDeptStockTakeDetailID;
            DrugDeptStockTakeDetails.DrugDeptStockTakeID = DrugDeptStockTakeID;
            DrugDeptStockTakeDetails.GenMedProductID = GenMedProductID;
            DrugDeptStockTakeDetails.CaculatedQty = CaculatedQty;
            DrugDeptStockTakeDetails.ActualQty = ActualQty;
            return DrugDeptStockTakeDetails;
        }

        #endregion

        #region Primitive Properties

        [DataMemberAttribute()]
        public Int64 DrugDeptStockTakeDetailID
        {
            get
            {
                return _DrugDeptStockTakeDetailID;
            }
            set
            {
                if (_DrugDeptStockTakeDetailID != value)
                {
                    OnDrugDeptStockTakeDetailIDChanging(value);
                    _DrugDeptStockTakeDetailID = value;
                    RaisePropertyChanged("DrugDeptStockTakeDetailID");
                    OnDrugDeptStockTakeDetailIDChanged();
                }
            }
        }
        private Int64 _DrugDeptStockTakeDetailID;
        partial void OnDrugDeptStockTakeDetailIDChanging(Int64 value);
        partial void OnDrugDeptStockTakeDetailIDChanged();

        [DataMemberAttribute()]
        public Int64 DrugDeptStockTakeID
        {
            get
            {
                return _DrugDeptStockTakeID;
            }
            set
            {
                OnDrugDeptStockTakeIDChanging(value);
                _DrugDeptStockTakeID = value;
                RaisePropertyChanged("DrugDeptStockTakeID");
                OnDrugDeptStockTakeIDChanged();
            }
        }
        private Int64 _DrugDeptStockTakeID;
        partial void OnDrugDeptStockTakeIDChanging(Int64 value);
        partial void OnDrugDeptStockTakeIDChanged();


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
        public Int32 CaculatedQty
        {
            get
            {
                return _CaculatedQty;
            }
            set
            {
                OnCaculatedQtyChanging(value);
                _CaculatedQty = value;
                RaisePropertyChanged("CaculatedQty");
                OnCaculatedQtyChanged();
            }
        }
        private Int32 _CaculatedQty;
        partial void OnCaculatedQtyChanging(Int32 value);
        partial void OnCaculatedQtyChanged();

        [Range(0.0, 99999999999.0, ErrorMessage = "Số lượng thực tế không được nhỏ hơn 0")]
        [DataMemberAttribute()]
        public Int32 ActualQty
        {
            get
            {
                return _ActualQty;
            }
            set
            {
                OnActualQtyChanging(value);
                ValidateProperty("ActualQty", value);
                _ActualQty = value;
                AdjustQty = _ActualQty - CaculatedQty;
                RaisePropertyChanged("AdjustQty");
                RaisePropertyChanged("ActualQty");
                OnActualQtyChanged();
            }
        }
        private Int32 _ActualQty;
        partial void OnActualQtyChanging(Int32 value);
        partial void OnActualQtyChanged();

        [DataMemberAttribute()]
        public Int32 AdjustQty
        {
            get
            {
                return _AdjustQty;
            }
            set
            {
                _AdjustQty = value;
                RaisePropertyChanged("AdjustQty");
            }
        }
        private Int32 _AdjustQty;

        [DataMemberAttribute()]
        public decimal Price
        {
            get
            {
                return _Price;
            }
            set
            {
                _Price = value;
                RaisePropertyChanged("Price");
            }
        }
        private decimal _Price;

        [DataMemberAttribute()]
        public decimal NewestInwardPrice
        {
            get
            {
                return _NewestInwardPrice;
            }
            set
            {
                _NewestInwardPrice = value;
                RaisePropertyChanged("NewestInwardPrice");
            }
        }
        private decimal _NewestInwardPrice;

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
        private string _Notes;

        [DataMemberAttribute()]
        public Int16 RowActionStatusFlag
        {
            get
            {
                return _RowActionStatusFlag;
            }
            set
            {
                OnRowActionStatusFlagChanging(value);
                _RowActionStatusFlag = value;
                RaisePropertyChanged("RowActionStatusFlag");
                OnRowActionStatusFlagChanged();
            }
        }
        private Int16 _RowActionStatusFlag;
        partial void OnRowActionStatusFlagChanging(Int64 value);
        partial void OnRowActionStatusFlagChanged();


        #endregion

        #region Navigation Properties

        [DataMemberAttribute()]
        public string Code
        {
            get
            {
                return _Code;
            }
            set
            {
                _Code = value;
                RaisePropertyChanged("Code");
            }
        }
        private string _Code;

        [DataMemberAttribute()]
        public string BrandName
        {
            get
            {
                return _BrandName;
            }
            set
            {
                _BrandName = value;
                RaisePropertyChanged("BrandName");
            }
        }
        private string _BrandName;

        [DataMemberAttribute()]
        public string GenericName
        {
            get
            {
                return _GenericName;
            }
            set
            {
                _GenericName = value;
                RaisePropertyChanged("GenericName");
            }
        }
        private string _GenericName;

        [DataMemberAttribute()]
        public string Packaging
        {
            get
            {
                return _Packaging;
            }
            set
            {
                _Packaging = value;
                RaisePropertyChanged("Packaging");
            }
        }
        private string _Packaging;

        [DataMemberAttribute()]
        public string UnitName
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
        private string _UnitName;

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
        private string _ShelfName;

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
        private string _ProductCodeRefNum;
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
        [DataMemberAttribute()]
        public Decimal FinalAmount
        {
            get
            {
                return _FinalAmount;
            }
            set
            {
                _FinalAmount = value;
                RaisePropertyChanged("FinalAmount");
            }
        }
        private Decimal _FinalAmount;

        private long _BidDetailID;
        private string _BidCode;
        [DataMemberAttribute]
        public long BidDetailID
        {
            get
            {
                return _BidDetailID;
            }
            set
            {
                if (_BidDetailID == value)
                {
                    return;
                }
                _BidDetailID = value;
                RaisePropertyChanged("BidDetailID");
            }
        }
        [DataMemberAttribute]
        public string BidCode
        {
            get
            {
                return _BidCode;
            }
            set
            {
                if (_BidCode == value)
                {
                    return;
                }
                _BidCode = value;
                RaisePropertyChanged("BidCode");
            }
        }
    }
}