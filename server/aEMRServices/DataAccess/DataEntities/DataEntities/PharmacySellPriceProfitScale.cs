using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;
using System.Text;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DataEntities
{
    public partial class PharmacySellPriceProfitScale : NotifyChangedBase
    {
        [DataMemberAttribute()]
        public Int64 PharmacySellPriceProfitScaleID
        {
            get
            {
                return _PharmacySellPriceProfitScaleID;
            }
            set
            {
                if (_PharmacySellPriceProfitScaleID != value)
                {
                    OnPharmacySellPriceProfitScaleIDChanging(value);
                    _PharmacySellPriceProfitScaleID = value;
                    RaisePropertyChanged("PharmacySellPriceProfitScaleID");
                    OnPharmacySellPriceProfitScaleIDChanged();
                }
            }
        }
        private Int64 _PharmacySellPriceProfitScaleID;
        partial void OnPharmacySellPriceProfitScaleIDChanging(Int64 value);
        partial void OnPharmacySellPriceProfitScaleIDChanged();


        [DataMemberAttribute()]
        public DateTime RecCreatedDate
        {
            get
            {
                return _RecCreatedDate;
            }
            set
            {
                if (_RecCreatedDate != value)
                {
                    OnRecCreatedDateChanging(value);
                    _RecCreatedDate = value;
                    RaisePropertyChanged("RecCreatedDate");
                    OnRecCreatedDateChanged();
                }
            }
        }
        private DateTime _RecCreatedDate;
        partial void OnRecCreatedDateChanging(DateTime value);
        partial void OnRecCreatedDateChanged();


          [Range(0, 99999999999.0, ErrorMessage = "Không Được < 0")]
        [DataMemberAttribute()]
        public Decimal BuyingCostFrom
        {
            get
            {
                return _BuyingCostFrom;
            }
            set
            {
                if (_BuyingCostFrom != value)
                {
                    OnBuyingCostFromChanging(value);
                    ValidateProperty("BuyingCostFrom", value);
                    _BuyingCostFrom = value;
                    RaisePropertyChanged("BuyingCostFrom");
                    OnBuyingCostFromChanged();
                }
            }
        }
        private Decimal _BuyingCostFrom;
        partial void OnBuyingCostFromChanging(Decimal value);
        partial void OnBuyingCostFromChanged();


        [Range(0, 99999999999.0, ErrorMessage = "Không Được < 0")]
        [DataMemberAttribute()]
        public Decimal BuyingCostTo
        {
            get
            {
                return _BuyingCostTo;
            }
            set
            {
                if (_BuyingCostTo != value)
                {
                    OnBuyingCostToChanging(value);
                    ValidateProperty("BuyingCostTo", value);
                    _BuyingCostTo = value;
                    RaisePropertyChanged("BuyingCostTo");
                    OnBuyingCostToChanged();
                }
            }
        }
        private Decimal _BuyingCostTo;
        partial void OnBuyingCostToChanging(Decimal value);
        partial void OnBuyingCostToChanged();


        [Range(0, 99999999999.0, ErrorMessage = "Không Được < 0")]
        [DataMemberAttribute()]
        public Double NormalProfitPercent
        {
            get
            {
                return _NormalProfitPercent;
            }
            set
            {
                if (_NormalProfitPercent != value)
                {
                    OnNormalProfitPercentChanging(value);
                    ValidateProperty("NormalProfitPercent", value);
                    _NormalProfitPercent = value;
                    RaisePropertyChanged("NormalProfitPercent");
                    OnNormalProfitPercentChanged();
                }
            }
        }
        private Double _NormalProfitPercent;
        partial void OnNormalProfitPercentChanging(Double value);
        partial void OnNormalProfitPercentChanged();


        [Range(0, 99999999999.0, ErrorMessage = "Không Được < 0")]
        [DataMemberAttribute()]
        public Double HIAllowProfitPercent
        {
            get
            {
                return _HIAllowProfitPercent;
            }
            set
            {
                if (_HIAllowProfitPercent != value)
                {
                    OnHIAllowProfitPercentChanging(value);
                    ValidateProperty("HIAllowProfitPercent", value);
                    _HIAllowProfitPercent = value;
                    RaisePropertyChanged("HIAllowProfitPercent");
                    OnHIAllowProfitPercentChanged();
                }
            }
        }
        private Double _HIAllowProfitPercent;
        partial void OnHIAllowProfitPercentChanging(Double value);
        partial void OnHIAllowProfitPercentChanged();


        [DataMemberAttribute()]
        public Boolean IsActive
        {
            get
            {
                return _IsActive;
            }
            set
            {
                OnIsActiveChanging(value);
                _IsActive = value;
                RaisePropertyChanged("IsActive");
                OnIsActiveChanged();
            }
        }
        private Boolean _IsActive;
        partial void OnIsActiveChanging(Boolean value);
        partial void OnIsActiveChanged();


    }
}
