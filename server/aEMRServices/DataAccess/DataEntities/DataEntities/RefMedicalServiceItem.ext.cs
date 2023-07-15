using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{
    public partial class RefMedicalServiceItem : IChargeableItemPrice, IGenericService
    {
        public bool IsSelected
        {
            get
            {
                return _isSelected;
            }
            set
            {
                _isSelected = value;
                RaisePropertyChanged("IsSelected");
            }
        }
        private bool _isSelected = false;

        public override bool Equals(object obj)
        {
            RefMedicalServiceItem cond = obj as RefMedicalServiceItem;
            if (cond == null)
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;

            return this.MedServiceID == cond.MedServiceID;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        #region Health Insurance payment.
        private decimal _Price;
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

        private decimal _PriceDifference;
        public decimal PriceDifference
        {
            get
            {
                return _PriceDifference;
            }
            set
            {
                _PriceDifference = value;
                RaisePropertyChanged("PriceDifference");
            }
        }
        private decimal _CoPayment;
        public decimal CoPayment
        {
            get
            {
                return _CoPayment;
            }
            set
            {
                _CoPayment = value;
                RaisePropertyChanged("CoPayment");
            }
        }

        private decimal _HIPayment;
        public decimal HIPayment
        {
            get
            {
                return _HIPayment;
            }
            set
            {
                _HIPayment = value;
                RaisePropertyChanged("HIPayment");
            }
        }

        private decimal _PatientPayment;
        public decimal PatientPayment
        {
            get
            {
                return _PatientPayment;
            }
            set
            {
                _PatientPayment = value;
                RaisePropertyChanged("PatientPayment");
            }
        }

        private ServicePrice _CalculatedPrice;
        [DataMember]
        public ServicePrice CalculatedPrice
        {
            get
            {
                return _CalculatedPrice;
            }
            set
            {
                _CalculatedPrice = value;
                RaisePropertyChanged("CalculatedPrice");
            }
        }

        public void CalculatePayment(InsuranceBenefit benefit)
        {
            if (benefit != null && _HIAllowedPrice.HasValue && _HIAllowedPrice.Value > 0)
            {
                PriceDifference = NormalPrice - _HIAllowedPrice.Value;
                if (PriceDifference < 0)
                {
                    PriceDifference = 0;
                }
                decimal diff = NormalPrice - PriceDifference;

                HIPayment = Math.Round((decimal)benefit.RebatePercentage * diff);
                CoPayment = diff - HIPayment;

                PatientPayment = NormalPrice - HIPayment;
            }
            else
            {
                PriceDifference = 0;
                CoPayment = 0;
                HIPayment = 0;
                PatientPayment = NormalPrice;
            }
        }
        #endregion

       
        #region IChargeableItemPrice Members
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
        private decimal _NormalPrice;
       
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
        private decimal _HIPatientPrice;
 
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
        private decimal? _HIAllowedPrice;
        private ChargeableItemType _ChargeableItemType;
        public ChargeableItemType ChargeableItemType
        {
            get
            {
                return _ChargeableItemType;
            }
            set
            {
                _ChargeableItemType = value;
                RaisePropertyChanged("ChargeableItemType");
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
    }
}
