using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{

    public partial class InwardResource : NotifyChangedBase
    {
        #region Factory Method


        /// Create a new InwardResource object.

        /// <param name="inwardRscrID">Initial value of the InwardRscrID property.</param>
        /// <param name="inQuantity">Initial value of the InQuantity property.</param>
        /// <param name="inBuyingPrice">Initial value of the InBuyingPrice property.</param>
        /// <param name="internalSellingPrice">Initial value of the InternalSellingPrice property.</param>
        public static InwardResource CreateInwardResource(long inwardRscrID, Double inQuantity, Decimal inBuyingPrice, Decimal internalSellingPrice)
        {
            InwardResource inwardResource = new InwardResource();
            inwardResource.InwardRscrID = inwardRscrID;
            inwardResource.InQuantity = inQuantity;
            inwardResource.InBuyingPrice = inBuyingPrice;
            inwardResource.InternalSellingPrice = internalSellingPrice;
            return inwardResource;
        }

        #endregion
        #region Primitive Properties





        [DataMemberAttribute()]
        public long InwardRscrID
        {
            get
            {
                return _InwardRscrID;
            }
            set
            {
                if (_InwardRscrID != value)
                {
                    OnInwardRscrIDChanging(value);
                    ////ReportPropertyChanging("InwardRscrID");
                    _InwardRscrID = value;
                    RaisePropertyChanged("InwardRscrID");
                    OnInwardRscrIDChanged();
                }
            }
        }
        private long _InwardRscrID;
        partial void OnInwardRscrIDChanging(long value);
        partial void OnInwardRscrIDChanged();





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
                ////ReportPropertyChanging("StaffID");
                _StaffID = value;
                RaisePropertyChanged("StaffID");
                OnStaffIDChanged();
            }
        }
        private Nullable<Int64> _StaffID;
        partial void OnStaffIDChanging(Nullable<Int64> value);
        partial void OnStaffIDChanged();





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
        public Nullable<Int64> RscrID
        {
            get
            {
                return _RscrID;
            }
            set
            {
                OnRscrIDChanging(value);
                ////ReportPropertyChanging("RscrID");
                _RscrID = value;
                RaisePropertyChanged("RscrID");
                OnRscrIDChanged();
            }
        }
        private Nullable<Int64> _RscrID;
        partial void OnRscrIDChanging(Nullable<Int64> value);
        partial void OnRscrIDChanged();





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
                ////ReportPropertyChanging("InBatchNumber");
                _InBatchNumber = value;
                RaisePropertyChanged("InBatchNumber");
                OnInBatchNumberChanged();
            }
        }
        private String _InBatchNumber;
        partial void OnInBatchNumberChanging(String value);
        partial void OnInBatchNumberChanged();





        [DataMemberAttribute()]
        public Nullable<Int64> SerialCode
        {
            get
            {
                return _SerialCode;
            }
            set
            {
                OnSerialCodeChanging(value);
                ////ReportPropertyChanging("SerialCode");
                _SerialCode = value;
                RaisePropertyChanged("SerialCode");
                OnSerialCodeChanged();
            }
        }
        private Nullable<Int64> _SerialCode;
        partial void OnSerialCodeChanging(Nullable<Int64> value);
        partial void OnSerialCodeChanged();





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
                ////ReportPropertyChanging("InProductionDate");
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
                ////ReportPropertyChanging("InExpiryDate");
                _InExpiryDate = value;
                RaisePropertyChanged("InExpiryDate");
                OnInExpiryDateChanged();
            }
        }
        private Nullable<DateTime> _InExpiryDate;
        partial void OnInExpiryDateChanging(Nullable<DateTime> value);
        partial void OnInExpiryDateChanged();





        [DataMemberAttribute()]
        public Nullable<Decimal> InFromStorage
        {
            get
            {
                return _InFromStorage;
            }
            set
            {
                OnInFromStorageChanging(value);
                ////ReportPropertyChanging("InFromStorage");
                _InFromStorage = value;
                RaisePropertyChanged("InFromStorage");
                OnInFromStorageChanged();
            }
        }
        private Nullable<Decimal> _InFromStorage;
        partial void OnInFromStorageChanging(Nullable<Decimal> value);
        partial void OnInFromStorageChanged();





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
                ////ReportPropertyChanging("InQuantity");
                _InQuantity = value;
                RaisePropertyChanged("InQuantity");
                OnInQuantityChanged();
            }
        }
        private Double _InQuantity;
        partial void OnInQuantityChanging(Double value);
        partial void OnInQuantityChanged();





        [DataMemberAttribute()]
        public Decimal InBuyingPrice
        {
            get
            {
                return _InBuyingPrice;
            }
            set
            {
                OnInBuyingPriceChanging(value);
                ////ReportPropertyChanging("InBuyingPrice");
                _InBuyingPrice = value;
                RaisePropertyChanged("InBuyingPrice");
                OnInBuyingPriceChanged();
            }
        }
        private Decimal _InBuyingPrice;
        partial void OnInBuyingPriceChanging(Decimal value);
        partial void OnInBuyingPriceChanged();





        [DataMemberAttribute()]
        public Decimal InternalSellingPrice
        {
            get
            {
                return _InternalSellingPrice;
            }
            set
            {
                OnInternalSellingPriceChanging(value);
                ////ReportPropertyChanging("InternalSellingPrice");
                _InternalSellingPrice = value;
                RaisePropertyChanged("InternalSellingPrice");
                OnInternalSellingPriceChanged();
            }
        }
        private Decimal _InternalSellingPrice;
        partial void OnInternalSellingPriceChanging(Decimal value);
        partial void OnInternalSellingPriceChanged();





        [DataMemberAttribute()]
        public Nullable<Decimal> InHIPriceAllow
        {
            get
            {
                return _InHIPriceAllow;
            }
            set
            {
                OnInHIPriceAllowChanging(value);
                ////ReportPropertyChanging("InHIPriceAllow");
                _InHIPriceAllow = value;
                RaisePropertyChanged("InHIPriceAllow");
                OnInHIPriceAllowChanged();
            }
        }
        private Nullable<Decimal> _InHIPriceAllow;
        partial void OnInHIPriceAllowChanging(Nullable<Decimal> value);
        partial void OnInHIPriceAllowChanged();





        [DataMemberAttribute()]
        public Nullable<Double> Remaining
        {
            get
            {
                return _Remaining;
            }
            set
            {
                OnRemainingChanging(value);
                ////ReportPropertyChanging("Remaining");
                _Remaining = value;
                RaisePropertyChanged("Remaining");
                OnRemainingChanged();
            }
        }
        private Nullable<Double> _Remaining;
        partial void OnRemainingChanging(Nullable<Double> value);
        partial void OnRemainingChanged();





        [DataMemberAttribute()]
        public Nullable<Boolean> IsReEntry
        {
            get
            {
                return _IsReEntry;
            }
            set
            {
                OnIsReEntryChanging(value);
                ////ReportPropertyChanging("IsReEntry");
                _IsReEntry = value;
                RaisePropertyChanged("IsReEntry");
                OnIsReEntryChanged();
            }
        }
        private Nullable<Boolean> _IsReEntry;
        partial void OnIsReEntryChanging(Nullable<Boolean> value);
        partial void OnIsReEntryChanged();





        [DataMemberAttribute()]
        public Nullable<Boolean> Malfunctioned
        {
            get
            {
                return _Malfunctioned;
            }
            set
            {
                OnMalfunctionedChanging(value);
                ////ReportPropertyChanging("Malfunctioned");
                _Malfunctioned = value;
                RaisePropertyChanged("Malfunctioned");
                OnMalfunctionedChanged();
            }
        }
        private Nullable<Boolean> _Malfunctioned;
        partial void OnMalfunctionedChanging(Nullable<Boolean> value);
        partial void OnMalfunctionedChanged();

        #endregion

        #region Navigation Properties






        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_ASSIGNME_REL_RM06_INWARDRE", "AssignMedEquip")]
        public ObservableCollection<AssignMedEquip> AssignMedEquips
        {
            get;
            set;
        }






        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_CONTRACT_REL_RM14_INWARDRE", "ContractDetails")]
        public ObservableCollection<ContractDetail> ContractDetails
        {
            get;
            set;
        }






        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_EQUIPHIS_REL_RM20_INWARDRE", "EquipHistory")]
        public ObservableCollection<EquipHistory> EquipHistories
        {
            get;
            set;
        }






        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_EXAMMAIN_REL_RM19_INWARDRE", "ExamMaintenanceHistory")]
        public ObservableCollection<ExamMaintenanceHistory> ExamMaintenanceHistories
        {
            get;
            set;
        }






        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_INWARDRE_REL_RM17_RESOURCE", "Resources")]
        public Resource Resource
        {
            get;
            set;
        }

        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_INWARDRE_REL_RM25_REFSTORA", "RefStorageWarehouseLocation")]
        public RefStorageWarehouseLocation RefStorageWarehouseLocation
        {
            get;
            set;
        }

        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_INWARDRE_REL_RM26_STAFFS", "Staffs")]
        public Staff Staff
        {
            get;
            set;
        }

        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_OUTWARDR_REL_RM24_INWARDRE", "OutwardResources")]
        public ObservableCollection<OutwardResource> OutwardResources
        {
            get;
            set;
        }

        #endregion
    }
}
