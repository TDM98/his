using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;
using System.ComponentModel.DataAnnotations;


namespace DataEntities
{
    public partial class FeeDrug : NotifyChangedBase
    {
        public static FeeDrug CreateFeeDrug(Decimal sumnothi, Decimal sumhi, Decimal sumDifference, Decimal sumall, Decimal amountcopay, Decimal patientpay, long TransactionID, Decimal Amount, Decimal PriceDifference, Decimal HealthInsuranceRebate, Decimal PayAmount)
        {
            FeeDrug p = new FeeDrug();
            //money drug 
            p.SumNotHI = sumnothi;
            p.SumHI = sumhi;
            p.SumAll = sumall;
            p.AmountCoPay = amountcopay;
            p.PatientPay = patientpay;
            p.SumDifference = sumDifference;

            //money patient paid 
            p.TransactionID = TransactionID;
            p.Amount = Amount;
            p.PriceDifference = PriceDifference;
            p.HealthInsuranceRebate = HealthInsuranceRebate;
            p.PayAmount = PayAmount;
            return p;
        }

        private Decimal _SumNotHI;
        public Decimal SumNotHI
        {
            get
            {
                return _SumNotHI;
            }
            set
            {
                if (_SumNotHI != value)
                {
                    _SumNotHI = value;
                    RaisePropertyChanged("SumNotHI");
                    _SumAll = _SumNotHI + _SumHI+_SumDifference;
                    RaisePropertyChanged("SumAll");
                    _PatientPay = _PriceDifference + _SumNotHI + _AmountCoPay - _PayAmount;
                    RaisePropertyChanged("PatientPay");
                }
            }
        }

        private Decimal _SumHI;
        public Decimal SumHI
        {
            get
            {
                return _SumHI;
            }
            set
            {
                if (_SumHI != value)
                {
                    _SumHI = value;
                    RaisePropertyChanged("SumHI");
                    _SumAll = _SumNotHI + _SumHI + _SumDifference;
                    RaisePropertyChanged("SumAll");
                    _PatientPay = _PriceDifference + _SumNotHI + _AmountCoPay + _SumDifference - _PayAmount;
                    RaisePropertyChanged("PatientPay");
                }
            }
        }

        private Decimal _SumDifference;
        public Decimal SumDifference
        {
            get
            {
                return _SumDifference;
            }
            set
            {
                if (_SumDifference != value)
                {
                    _SumDifference = value;
                    RaisePropertyChanged("SumDifference");
                    _SumAll = _SumNotHI + _SumHI + _SumDifference;
                    RaisePropertyChanged("SumAll");
                    _PatientPay = _PriceDifference + _SumNotHI + _AmountCoPay +_SumDifference - _PayAmount;
                    RaisePropertyChanged("PatientPay");
                }
            }
        }

        private Decimal _SumAll;
        public Decimal SumAll
        {
            get
            {
                return _SumAll;
            }
            set
            {
                if (_SumAll != value)
                {
                    _SumAll = value;
                    RaisePropertyChanged("SumAll");
                    _PatientPay = _PriceDifference + _SumNotHI + _AmountCoPay + _SumDifference - _PayAmount;
                    RaisePropertyChanged("PatientPay");
                }
            }
        }

        private Decimal _AmountCoPay;
        public Decimal AmountCoPay
        {
            get
            {
                return _AmountCoPay;
            }
            set
            {
                if (_AmountCoPay != value)
                {
                    _AmountCoPay = value;
                    RaisePropertyChanged("AmountCoPay");
                    _PatientPay = _PriceDifference + _SumNotHI + _AmountCoPay + _SumDifference - _PayAmount;
                    RaisePropertyChanged("PatientPay");
                }
            }
        }

        private Decimal _PatientPay;
        public Decimal PatientPay
        {
            get
            {
                return _PatientPay;
            }
            set
            {
                if (_PatientPay != value)
                {
                    _PatientPay = value;
                    RaisePropertyChanged("PatientPay");

                }
            }
        }

        #region Primitive Properties

        [DataMemberAttribute()]
        public long TransactionID
        {
            get
            {
                return _TransactionID;
            }
            set
            {
                OnTransactionIDChanging(value);
                _TransactionID = value;
                RaisePropertyChanged("TransactionID");
                OnTransactionIDChanged();
            }
        }
        private long _TransactionID;
        partial void OnTransactionIDChanging(long value);
        partial void OnTransactionIDChanged();

        [DataMemberAttribute()]
        public Decimal Amount
        {
            get
            {
                return _Amount;
            }
            set
            {
                OnAmountChanging(value);
                _Amount = value;
                RaisePropertyChanged("Amount");
                OnAmountChanged();
            }
        }
        private Decimal _Amount;
        partial void OnAmountChanging(Decimal value);
        partial void OnAmountChanged();

        [DataMemberAttribute()]
        public Decimal PriceDifference
        {
            get
            {
                return _PriceDifference;
            }
            set
            {
                OnPriceDifferenceChanging(value);
                _PriceDifference = value;
                RaisePropertyChanged("PriceDifference");
                OnPriceDifferenceChanged();
            }
        }
        private Decimal _PriceDifference;
        partial void OnPriceDifferenceChanging(Decimal value);
        partial void OnPriceDifferenceChanged();

        [DataMemberAttribute()]
        public Decimal PayAmount
        {
            get
            {
                return _PayAmount;
            }
            set
            {
                OnPayAmountChanging(value);
                _PayAmount = value;
                RaisePropertyChanged("PayAmount");
                OnPayAmountChanged();
            }
        }
        private Decimal _PayAmount;
        partial void OnPayAmountChanging(Decimal value);
        partial void OnPayAmountChanged();

        [DataMemberAttribute()]
        public Decimal HealthInsuranceRebate
        {
            get
            {
                return _HealthInsuranceRebate;
            }
            set
            {
                OnHealthInsuranceRebateChanging(value);
                _HealthInsuranceRebate = value;
                RaisePropertyChanged("HealthInsuranceRebate");
                OnHealthInsuranceRebateChanged();
            }
        }
        private Decimal _HealthInsuranceRebate;
        partial void OnHealthInsuranceRebateChanging(Decimal value);
        partial void OnHealthInsuranceRebateChanged();
        #endregion
    }
}
