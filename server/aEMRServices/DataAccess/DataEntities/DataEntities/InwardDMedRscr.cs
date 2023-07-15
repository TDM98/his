using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{
    public partial class InwardDMedRscr : NotifyChangedBase
    {
        #region Factory Method


        /// Create a new InwardDMedRscr object.

        /// <param name="inwDMedRscrID">Initial value of the InwDMedRscrID property.</param>
        /// <param name="onWarehouseDateTime">Initial value of the OnWarehouseDateTime property.</param>
        /// <param name="productionDate">Initial value of the ProductionDate property.</param>
        /// <param name="expiryDate">Initial value of the ExpiryDate property.</param>
        /// <param name="buyingPrice">Initial value of the BuyingPrice property.</param>
        /// <param name="evenQuantity">Initial value of the EvenQuantity property.</param>
        public static InwardDMedRscr CreateInwardDMedRscr(long inwDMedRscrID, DateTime onWarehouseDateTime, DateTime productionDate, DateTime expiryDate, Decimal buyingPrice, Double evenQuantity)
        {
            InwardDMedRscr inwardDMedRscr = new InwardDMedRscr();
            inwardDMedRscr.InwDMedRscrID = inwDMedRscrID;
            inwardDMedRscr.OnWarehouseDateTime = onWarehouseDateTime;
            inwardDMedRscr.ProductionDate = productionDate;
            inwardDMedRscr.ExpiryDate = expiryDate;
            inwardDMedRscr.BuyingPrice = buyingPrice;
            inwardDMedRscr.EvenQuantity = evenQuantity;
            return inwardDMedRscr;
        }

        #endregion
        #region Primitive Properties





        [DataMemberAttribute()]
        public long InwDMedRscrID
        {
            get
            {
                return _InwDMedRscrID;
            }
            set
            {
                if (_InwDMedRscrID != value)
                {
                    OnInwDMedRscrIDChanging(value);
                    ////ReportPropertyChanging("InwDMedRscrID");
                    _InwDMedRscrID = value;
                    RaisePropertyChanged("InwDMedRscrID");
                    OnInwDMedRscrIDChanged();
                }
            }
        }
        private long _InwDMedRscrID;
        partial void OnInwDMedRscrIDChanging(long value);
        partial void OnInwDMedRscrIDChanged();





        [DataMemberAttribute()]
        public Nullable<long> InvDMedRscrID
        {
            get
            {
                return _InvDMedRscrID;
            }
            set
            {
                OnInvDMedRscrIDChanging(value);
                ////ReportPropertyChanging("InvDMedRscrID");
                _InvDMedRscrID = value;
                RaisePropertyChanged("InvDMedRscrID");
                OnInvDMedRscrIDChanged();
            }
        }
        private Nullable<long> _InvDMedRscrID;
        partial void OnInvDMedRscrIDChanging(Nullable<long> value);
        partial void OnInvDMedRscrIDChanged();





        [DataMemberAttribute()]
        public Nullable<long> StoreID
        {
            get
            {
                return _StoreID;
            }
            set
            {
                OnStoreIDChanging(value);
                ////ReportPropertyChanging("StoreID");
                _StoreID = value;
                RaisePropertyChanged("StoreID");
                OnStoreIDChanged();
            }
        }
        private Nullable<long> _StoreID;
        partial void OnStoreIDChanging(Nullable<long> value);
        partial void OnStoreIDChanged();





        [DataMemberAttribute()]
        public Nullable<long> DMedRscrID
        {
            get
            {
                return _DMedRscrID;
            }
            set
            {
                OnDMedRscrIDChanging(value);
                ////ReportPropertyChanging("DMedRscrID");
                _DMedRscrID = value;
                RaisePropertyChanged("DMedRscrID");
                OnDMedRscrIDChanged();
            }
        }
        private Nullable<long> _DMedRscrID;
        partial void OnDMedRscrIDChanging(Nullable<long> value);
        partial void OnDMedRscrIDChanged();





        [DataMemberAttribute()]
        public DateTime OnWarehouseDateTime
        {
            get
            {
                return _OnWarehouseDateTime;
            }
            set
            {
                OnOnWarehouseDateTimeChanging(value);
                ////ReportPropertyChanging("OnWarehouseDateTime");
                _OnWarehouseDateTime = value;
                RaisePropertyChanged("OnWarehouseDateTime");
                OnOnWarehouseDateTimeChanged();
            }
        }
        private DateTime _OnWarehouseDateTime;
        partial void OnOnWarehouseDateTimeChanging(DateTime value);
        partial void OnOnWarehouseDateTimeChanged();





        [DataMemberAttribute()]
        public DateTime ProductionDate
        {
            get
            {
                return _ProductionDate;
            }
            set
            {
                OnProductionDateChanging(value);
                ////ReportPropertyChanging("ProductionDate");
                _ProductionDate = value;
                RaisePropertyChanged("ProductionDate");
                OnProductionDateChanged();
            }
        }
        private DateTime _ProductionDate;
        partial void OnProductionDateChanging(DateTime value);
        partial void OnProductionDateChanged();





        [DataMemberAttribute()]
        public DateTime ExpiryDate
        {
            get
            {
                return _ExpiryDate;
            }
            set
            {
                OnExpiryDateChanging(value);
                ////ReportPropertyChanging("ExpiryDate");
                _ExpiryDate = value;
                RaisePropertyChanged("ExpiryDate");
                OnExpiryDateChanged();
            }
        }
        private DateTime _ExpiryDate;
        partial void OnExpiryDateChanging(DateTime value);
        partial void OnExpiryDateChanged();





        [DataMemberAttribute()]
        public Decimal BuyingPrice
        {
            get
            {
                return _BuyingPrice;
            }
            set
            {
                OnBuyingPriceChanging(value);
                ////ReportPropertyChanging("BuyingPrice");
                _BuyingPrice = value;
                RaisePropertyChanged("BuyingPrice");
                OnBuyingPriceChanged();
            }
        }
        private Decimal _BuyingPrice;
        partial void OnBuyingPriceChanging(Decimal value);
        partial void OnBuyingPriceChanged();





        [DataMemberAttribute()]
        public Double EvenQuantity
        {
            get
            {
                return _EvenQuantity;
            }
            set
            {
                OnEvenQuantityChanging(value);
                ////ReportPropertyChanging("EvenQuantity");
                _EvenQuantity = value;
                RaisePropertyChanged("EvenQuantity");
                OnEvenQuantityChanged();
            }
        }
        private Double _EvenQuantity;
        partial void OnEvenQuantityChanging(Double value);
        partial void OnEvenQuantityChanged();





        [DataMemberAttribute()]
        public Nullable<Decimal> RecommendedSellingPrice
        {
            get
            {
                return _RecommendedSellingPrice;
            }
            set
            {
                OnRecommendedSellingPriceChanging(value);
                ////ReportPropertyChanging("RecommendedSellingPrice");
                _RecommendedSellingPrice = value;
                RaisePropertyChanged("RecommendedSellingPrice");
                OnRecommendedSellingPriceChanged();
            }
        }
        private Nullable<Decimal> _RecommendedSellingPrice;
        partial void OnRecommendedSellingPriceChanging(Nullable<Decimal> value);
        partial void OnRecommendedSellingPriceChanged();





        [DataMemberAttribute()]
        public Nullable<Decimal> PRecommendedSellingPrice
        {
            get
            {
                return _PRecommendedSellingPrice;
            }
            set
            {
                OnPRecommendedSellingPriceChanging(value);
                ////ReportPropertyChanging("PRecommendedSellingPrice");
                _PRecommendedSellingPrice = value;
                RaisePropertyChanged("PRecommendedSellingPrice");
                OnPRecommendedSellingPriceChanged();
            }
        }
        private Nullable<Decimal> _PRecommendedSellingPrice;
        partial void OnPRecommendedSellingPriceChanging(Nullable<Decimal> value);
        partial void OnPRecommendedSellingPriceChanged();





        [DataMemberAttribute()]
        public Nullable<Double> InventoryRemaining
        {
            get
            {
                return _InventoryRemaining;
            }
            set
            {
                OnInventoryRemainingChanging(value);
                ////ReportPropertyChanging("InventoryRemaining");
                _InventoryRemaining = value;
                RaisePropertyChanged("InventoryRemaining");
                OnInventoryRemainingChanged();
            }
        }
        private Nullable<Double> _InventoryRemaining;
        partial void OnInventoryRemainingChanging(Nullable<Double> value);
        partial void OnInventoryRemainingChanged();

        #endregion

        #region Navigation Properties






        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_INWARDDM_REL_DMEDR_INWARDDM", "InwardDMedRscrInvoices")]
        public InwardDMedRscrInvoice InwardDMedRscrInvoice
        {
            get;
            set;
        }

        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_INWARDDM_REL_DMEDR_REFDISPO", "RefDisposableMedicalResources")]
        public RefDisposableMedicalResource RefDisposableMedicalResource
        {
            get;
            set;
        }

        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_INWARDDM_REL_DMEDR_REFSTORA", "RefStorageWarehouseLocation")]
        public RefStorageWarehouseLocation RefStorageWarehouseLocation
        {
            get;
            set;
        }

        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_OUTWARDD_REL_DMEDR_INWARDDM", "OutwardDMedRscrs")]
        public ObservableCollection<OutwardDMedRscr> OutwardDMedRscrs
        {
            get;
            set;
        }

        #endregion
    }
}
