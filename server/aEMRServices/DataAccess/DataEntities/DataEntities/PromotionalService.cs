using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{
    public partial class PromotionalService : NotifyChangedBase
    {
        #region Factory Method


        /// Create a new PromotionalService object.

        /// <param name="promServID">Initial value of the PromServID property.</param>
        /// <param name="promID">Initial value of the PromID property.</param>
        /// <param name="medSPackageID">Initial value of the MedSPackageID property.</param>
        public static PromotionalService CreatePromotionalService(Int64 promServID, Byte promID, long medSPackageID)
        {
            PromotionalService promotionalService = new PromotionalService();
            promotionalService.PromServID = promServID;
            promotionalService.PromID = promID;
            promotionalService.MedSPackageID = medSPackageID;
            return promotionalService;
        }

        #endregion
        #region Primitive Properties





        [DataMemberAttribute()]
        public Int64 PromServID
        {
            get
            {
                return _PromServID;
            }
            set
            {
                if (_PromServID != value)
                {
                    OnPromServIDChanging(value);
                    ////ReportPropertyChanging("PromServID");
                    _PromServID = value;
                    RaisePropertyChanged("PromServID");
                    OnPromServIDChanged();
                }
            }
        }
        private Int64 _PromServID;
        partial void OnPromServIDChanging(Int64 value);
        partial void OnPromServIDChanged();





        [DataMemberAttribute()]
        public Byte PromID
        {
            get
            {
                return _PromID;
            }
            set
            {
                OnPromIDChanging(value);
                ////ReportPropertyChanging("PromID");
                _PromID = value;
                RaisePropertyChanged("PromID");
                OnPromIDChanged();
            }
        }
        private Byte _PromID;
        partial void OnPromIDChanging(Byte value);
        partial void OnPromIDChanged();





        [DataMemberAttribute()]
        public long MedSPackageID
        {
            get
            {
                return _MedSPackageID;
            }
            set
            {
                OnMedSPackageIDChanging(value);
                ////ReportPropertyChanging("MedSPackageID");
                _MedSPackageID = value;
                RaisePropertyChanged("MedSPackageID");
                OnMedSPackageIDChanged();
            }
        }
        private long _MedSPackageID;
        partial void OnMedSPackageIDChanging(long value);
        partial void OnMedSPackageIDChanged();





        [DataMemberAttribute()]
        public Nullable<Double> PercentDiscount
        {
            get
            {
                return _PercentDiscount;
            }
            set
            {
                OnPercentDiscountChanging(value);
                ////ReportPropertyChanging("PercentDiscount");
                _PercentDiscount = value;
                RaisePropertyChanged("PercentDiscount");
                OnPercentDiscountChanged();
            }
        }
        private Nullable<Double> _PercentDiscount;
        partial void OnPercentDiscountChanging(Nullable<Double> value);
        partial void OnPercentDiscountChanged();





        [DataMemberAttribute()]
        public Nullable<Byte> ApplyOnWithPrice
        {
            get
            {
                return _ApplyOnWithPrice;
            }
            set
            {
                OnApplyOnWithPriceChanging(value);
                ////ReportPropertyChanging("ApplyOnWithPrice");
                _ApplyOnWithPrice = value;
                RaisePropertyChanged("ApplyOnWithPrice");
                OnApplyOnWithPriceChanged();
            }
        }
        private Nullable<Byte> _ApplyOnWithPrice;
        partial void OnApplyOnWithPriceChanging(Nullable<Byte> value);
        partial void OnApplyOnWithPriceChanged();

        #endregion

        #region Navigation Properties






        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_PROMOTIO_REL_HOSFM_PROMOTIO", "PromotionPlan")]
        public PromotionPlan PromotionPlan
        {
            get;
            set;
        }

        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_PROMOTIO_REL_HOSFM_REFMEDIC", "RefMedicalServicePackages")]
        public RefMedicalServicePackage RefMedicalServicePackage
        {
            get;
            set;
        }

        #endregion
    }
}
