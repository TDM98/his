using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;
using System.Text;
using System.Collections.Generic;
using Service.Core.Common;
using System.ComponentModel.DataAnnotations;

namespace DataEntities
{
    public partial class PharmacyStockTakeDetails : EntityBase
    {
        #region Factory Method
        public static PharmacyStockTakeDetails CreatePharmacyStockTakeDetails(Int64 PharmacyStockTakeDetailID, Int64 PharmacyStockTakeID, Int64 DrugID, Int32 outQtyPrevFirstMonth, Int32 outQtyPrevSecondMonth, Int32 outQtyPrevThirdMonth, Int32 outQtyPrevFourthMonth, Int32 CaculatedQty, Int32 ActualQty, Int32 adjustedQty, Int32 numberOfEstimatedMonths, Int32 outQtyLastTwelveMonths, Int32 toDateOutQty)
        {
            PharmacyStockTakeDetails PharmacyStockTakeDetails = new PharmacyStockTakeDetails();
            PharmacyStockTakeDetails.PharmacyStockTakeDetailID = PharmacyStockTakeDetailID;
            PharmacyStockTakeDetails.PharmacyStockTakeID = PharmacyStockTakeID;
            PharmacyStockTakeDetails.DrugID = DrugID;
            PharmacyStockTakeDetails.CaculatedQty = CaculatedQty;
            PharmacyStockTakeDetails.ActualQty = ActualQty;
            return PharmacyStockTakeDetails;
        }

        #endregion

        #region Primitive Properties

        [DataMemberAttribute()]
        public Int64 PharmacyStockTakeDetailID
        {
            get
            {
                return _PharmacyStockTakeDetailID;
            }
            set
            {
                if (_PharmacyStockTakeDetailID != value)
                {
                    OnPharmacyStockTakeDetailIDChanging(value);
                    _PharmacyStockTakeDetailID = value;
                    RaisePropertyChanged("PharmacyStockTakeDetailID");
                    OnPharmacyStockTakeDetailIDChanged();
                }
            }
        }
        private Int64 _PharmacyStockTakeDetailID;
        partial void OnPharmacyStockTakeDetailIDChanging(Int64 value);
        partial void OnPharmacyStockTakeDetailIDChanged();

        [DataMemberAttribute()]
        public Int64 PharmacyStockTakeID
        {
            get
            {
                return _PharmacyStockTakeID;
            }
            set
            {
                OnPharmacyStockTakeIDChanging(value);
                _PharmacyStockTakeID = value;
                RaisePropertyChanged("PharmacyStockTakeID");
                OnPharmacyStockTakeIDChanged();
            }
        }
        private Int64 _PharmacyStockTakeID;
        partial void OnPharmacyStockTakeIDChanging(Int64 value);
        partial void OnPharmacyStockTakeIDChanged();


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
                RaisePropertyChanged("TotalPrice");
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
                RaisePropertyChanged("TotalPrice");
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
        public string InBatchNumber
        {
            get
            {
                return _InBatchNumber;
            }
            set
            {
                _InBatchNumber = value;
                RaisePropertyChanged("InBatchNumber");
            }
        }
        private string _InBatchNumber;

        [DataMemberAttribute()]
        public DateTime? InExpiryDate
        {
            get
            {
                return _InExpiryDate;
            }
            set
            {
                _InExpiryDate = value;
                RaisePropertyChanged("InExpiryDate");
            }
        }
        private DateTime? _InExpiryDate;

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

        public decimal TotalPrice
        {
            get { return _Price * _ActualQty; }
        }
        #endregion

        #region Navigation Properties

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

    }
}
