using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{
    public partial class InwardBlood : NotifyChangedBase
    {
        #region Factory Method


        /// Create a new InwardBlood object.

        /// <param name="inwBloodID">Initial value of the InwBloodID property.</param>
        /// <param name="inwNumOfBloodUnits">Initial value of the InwNumOfBloodUnits property.</param>
        public static InwardBlood CreateInwardBlood(long inwBloodID, Double inwNumOfBloodUnits)
        {
            InwardBlood inwardBlood = new InwardBlood();
            inwardBlood.InwBloodID = inwBloodID;
            inwardBlood.InwNumOfBloodUnits = inwNumOfBloodUnits;
            return inwardBlood;
        }

        #endregion
        #region Primitive Properties





        [DataMemberAttribute()]
        public long InwBloodID
        {
            get
            {
                return _InwBloodID;
            }
            set
            {
                if (_InwBloodID != value)
                {
                    OnInwBloodIDChanging(value);
                    ////ReportPropertyChanging("InwBloodID");
                    _InwBloodID = value;
                    RaisePropertyChanged("InwBloodID");
                    OnInwBloodIDChanged();
                }
            }
        }
        private long _InwBloodID;
        partial void OnInwBloodIDChanging(long value);
        partial void OnInwBloodIDChanged();





        [DataMemberAttribute()]
        public Nullable<long> BDID
        {
            get
            {
                return _BDID;
            }
            set
            {
                OnBDIDChanging(value);
                ////ReportPropertyChanging("BDID");
                _BDID = value;
                RaisePropertyChanged("BDID");
                OnBDIDChanged();
            }
        }
        private Nullable<long> _BDID;
        partial void OnBDIDChanging(Nullable<long> value);
        partial void OnBDIDChanged();





        [DataMemberAttribute()]
        public Nullable<Byte> BloodTypeID
        {
            get
            {
                return _BloodTypeID;
            }
            set
            {
                OnBloodTypeIDChanging(value);
                ////ReportPropertyChanging("BloodTypeID");
                _BloodTypeID = value;
                RaisePropertyChanged("BloodTypeID");
                OnBloodTypeIDChanged();
            }
        }
        private Nullable<Byte> _BloodTypeID;
        partial void OnBloodTypeIDChanging(Nullable<Byte> value);
        partial void OnBloodTypeIDChanged();





        [DataMemberAttribute()]
        public Double InwNumOfBloodUnits
        {
            get
            {
                return _InwNumOfBloodUnits;
            }
            set
            {
                OnInwNumOfBloodUnitsChanging(value);
                ////ReportPropertyChanging("InwNumOfBloodUnits");
                _InwNumOfBloodUnits = value;
                RaisePropertyChanged("InwNumOfBloodUnits");
                OnInwNumOfBloodUnitsChanged();
            }
        }
        private Double _InwNumOfBloodUnits;
        partial void OnInwNumOfBloodUnitsChanging(Double value);
        partial void OnInwNumOfBloodUnitsChanged();





        [DataMemberAttribute()]
        public Nullable<Double> InwUnitPrice
        {
            get
            {
                return _InwUnitPrice;
            }
            set
            {
                OnInwUnitPriceChanging(value);
                ////ReportPropertyChanging("InwUnitPrice");
                _InwUnitPrice = value;
                RaisePropertyChanged("InwUnitPrice");
                OnInwUnitPriceChanged();
            }
        }
        private Nullable<Double> _InwUnitPrice;
        partial void OnInwUnitPriceChanging(Nullable<Double> value);
        partial void OnInwUnitPriceChanged();

        #endregion

        #region Navigation Properties






        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_INWARDBL_REL_BB05_BLOODDON", "BloodDonations")]
        public BloodDonation BloodDonation
        {
            get;
            set;
        }



        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_INWARDBL_REL_BB07_BLOODTYP", "BloodTypes")]
        public BloodType BloodType
        {
            get;
            set;
        }

        #endregion
    }
}
