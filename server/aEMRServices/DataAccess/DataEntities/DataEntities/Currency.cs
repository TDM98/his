
using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{
    public partial class Currency : NotifyChangedBase
    {

        #region Factory Method


        /// Create a new Currency object.

        /// <param name="currencyID">Initial value of the CurrencyID property.</param>
        /// <param name="currencyName">Initial value of the CurrencyName property.</param>
        /// <param name="currencySymbol">Initial value of the CurrencySymbol property.</param>
        public static Currency CreateCurrency(long currencyID, String currencyName, String currencySymbol)
        {
            Currency currency = new Currency();
            currency.CurrencyID = currencyID;
            currency.CurrencyName = currencyName;
            currency.CurrencySymbol = currencySymbol;
            return currency;
        }

        #endregion

        #region Primitive Properties

        [DataMemberAttribute()]
        public long CurrencyID
        {
            get
            {
                return _CurrencyID;
            }
            set
            {
                if (_CurrencyID != value)
                {
                    OnCurrencyIDChanging(value);
                    _CurrencyID = value;
                    RaisePropertyChanged("CurrencyID");
                    OnCurrencyIDChanged();
                }
            }
        }
        private long _CurrencyID;
        partial void OnCurrencyIDChanging(long value);
        partial void OnCurrencyIDChanged();

        [DataMemberAttribute()]
        public String CurrencyName
        {
            get
            {
                return _CurrencyName;
            }
            set
            {
                OnCurrencyNameChanging(value);
                _CurrencyName = value;
                RaisePropertyChanged("CurrencyName");
                OnCurrencyNameChanged();
            }
        }
        private String _CurrencyName;
        partial void OnCurrencyNameChanging(String value);
        partial void OnCurrencyNameChanged();

        [DataMemberAttribute()]
        public String CurrencySymbol
        {
            get
            {
                return _CurrencySymbol;
            }
            set
            {
                OnCurrencySymbolChanging(value);
                _CurrencySymbol = value;
                RaisePropertyChanged("CurrencySymbol");
                OnCurrencySymbolChanged();
            }
        }
        private String _CurrencySymbol;
        partial void OnCurrencySymbolChanging(String value);
        partial void OnCurrencySymbolChanged();

        [DataMemberAttribute()]
        public bool? IsActive
        {
            get
            {
                return _IsActive;
            }
            set
            {
                _IsActive = value;
                RaisePropertyChanged("IsActive");
            }
        }
        private bool? _IsActive;

        [DataMemberAttribute()]
        public int? OrderNumber
        {
            get
            {
                return _OrderNumber;
            }
            set
            {
                _OrderNumber = value;
                RaisePropertyChanged("OrderNumber");
            }
        }
        private int? _OrderNumber;

        #endregion

        #region Navigation Properties

        [DataMemberAttribute()]
        public ObservableCollection<PatientInvoice> PatientInvoices
        {
            get;
            set;
        }

        #endregion

        public override bool Equals(object obj)
        {
            Currency currentCurrency = obj as Currency;
            if (currentCurrency == null)
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;

            return this.CurrencyID == currentCurrency.CurrencyID;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
