using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;
using System.ComponentModel.DataAnnotations;

namespace DataEntities
{
    public partial class SellingPriceFormular : NotifyChangedBase
    {
        #region Factory Method


        /// Create a new SellingPriceFormular object.

        /// <param name="sPFItemID">Initial value of the SPFItemID property.</param>
        /// <param name="sPFModifedDate">Initial value of the SPFModifedDate property.</param>
        public static SellingPriceFormular CreateSellingPriceFormular(long sPFItemID, DateTime sPFModifedDate)
        {
            SellingPriceFormular sellingPriceFormular = new SellingPriceFormular();
            sellingPriceFormular.SPFItemID = sPFItemID;
            sellingPriceFormular.SPFModifedDate = sPFModifedDate;
            return sellingPriceFormular;
        }

        #endregion

        #region Primitive Properties
        [DataMemberAttribute()]
        public long SPFItemID
        {
            get
            {
                return _SPFItemID;
            }
            set
            {
                if (_SPFItemID != value)
                {
                    OnSPFItemIDChanging(value);
                    _SPFItemID = value;
                    RaisePropertyChanged("SPFItemID");
                    OnSPFItemIDChanged();
                }
            }
        }
        private long _SPFItemID;
        partial void OnSPFItemIDChanging(long value);
        partial void OnSPFItemIDChanged();

        [DataMemberAttribute()]
        public DateTime SPFModifedDate
        {
            get
            {
                return _SPFModifedDate;
            }
            set
            {
                OnSPFModifedDateChanging(value);
                _SPFModifedDate = value;
                RaisePropertyChanged("SPFModifedDate");
                OnSPFModifedDateChanged();
            }
        }
        private DateTime _SPFModifedDate;
        partial void OnSPFModifedDateChanging(DateTime value);
        partial void OnSPFModifedDateChanged();


        [Required(ErrorMessage = "InternalProfitPrice is Required")]
        [Range(0.0, 99999999999999.0, ErrorMessage = "Lợi nhuận không được < 0")]
        [DataMemberAttribute()]
        public Nullable<Double> InternalProfitPrice
        {
            get
            {
                return _InternalProfitPrice;
            }
            set
            {
                OnInternalProfitPriceChanging(value);
                ValidateProperty("InternalProfitPrice", value);
                _InternalProfitPrice = value;
                RaisePropertyChanged("InternalProfitPrice");
                OnInternalProfitPriceChanged();
            }
        }
        private Nullable<Double> _InternalProfitPrice;
        partial void OnInternalProfitPriceChanging(Nullable<Double> value);
        partial void OnInternalProfitPriceChanged();

        [Required(ErrorMessage = "ExternalProfitPrice is Required")]
        [Range(0.0, 99999999999999.0, ErrorMessage = "Lợi nhuận không được < 0")]
        [DataMemberAttribute()]
        public Nullable<Double> ExternalProfitPrice
        {
            get
            {
                return _ExternalProfitPrice;
            }
            set
            {
                OnExternalProfitPriceChanging(value);
                ValidateProperty("ExternalProfitPrice", value);
                _ExternalProfitPrice = value;
                RaisePropertyChanged("ExternalProfitPrice");
                OnExternalProfitPriceChanged();
            }
        }
        private Nullable<Double> _ExternalProfitPrice;
        partial void OnExternalProfitPriceChanging(Nullable<Double> value);
        partial void OnExternalProfitPriceChanged();

        [Required(ErrorMessage = "HIProfitPrice is Required")]
        [Range(0.0, 99999999999999.0, ErrorMessage = "Lợi nhuận không được < 0")]
        [DataMemberAttribute()]
        public Nullable<Double> HIProfitPrice
        {
            get
            {
                return _HIProfitPrice;
            }
            set
            {
                OnHIProfitPriceChanging(value);
                ValidateProperty("HIProfitPrice", value);
                _HIProfitPrice = value;
                RaisePropertyChanged("HIProfitPrice");
                OnHIProfitPriceChanged();
            }
        }
        private Nullable<Double> _HIProfitPrice;
        partial void OnHIProfitPriceChanging(Nullable<Double> value);
        partial void OnHIProfitPriceChanged();

        [DataMemberAttribute()]
        public Nullable<Boolean> IsPercentage
        {
            get
            {
                return _IsPercentage;
            }
            set
            {
                OnIsPercentageChanging(value);
                _IsPercentage = value;
                RaisePropertyChanged("IsPercentage");
                OnIsPercentageChanged();
            }
        }
        private Nullable<Boolean> _IsPercentage;
        partial void OnIsPercentageChanging(Nullable<Boolean> value);
        partial void OnIsPercentageChanged();

        [DataMemberAttribute()]
        public Nullable<Boolean> IsActive
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
        private Nullable<Boolean> _IsActive;
        partial void OnIsActiveChanging(Nullable<Boolean> value);
        partial void OnIsActiveChanged();

        #endregion

    }
}
