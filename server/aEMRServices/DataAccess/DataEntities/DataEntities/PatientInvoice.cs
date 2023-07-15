using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;
namespace DataEntities
{
    public partial class PatientInvoice : NotifyChangedBase
    {
        #region Factory Method


        /// Create a new PatientInvoice object.

        /// <param name="invoiceID">Initial value of the InvoiceID property.</param>
        /// <param name="transactionID">Initial value of the TransactionID property.</param>
        /// <param name="taxInvoiceNumber">Initial value of the TaxInvoiceNumber property.</param>
        public static PatientInvoice CreatePatientInvoice(String invoiceID, long transactionID, String taxInvoiceNumber)
        {
            PatientInvoice patientInvoice = new PatientInvoice();
            patientInvoice.InvoiceID = invoiceID;
            patientInvoice.TransactionID = transactionID;
            patientInvoice.TaxInvoiceNumber = taxInvoiceNumber;
            return patientInvoice;
        }

        #endregion
        #region Primitive Properties





        [DataMemberAttribute()]
        public String InvoiceID
        {
            get
            {
                return _InvoiceID;
            }
            set
            {
                if (_InvoiceID != value)
                {
                    OnInvoiceIDChanging(value);
                    ////ReportPropertyChanging("InvoiceID");
                    _InvoiceID = value;
                    RaisePropertyChanged("InvoiceID");
                    OnInvoiceIDChanged();
                }
            }
        }
        private String _InvoiceID;
        partial void OnInvoiceIDChanging(String value);
        partial void OnInvoiceIDChanged();





        [DataMemberAttribute()]
        public Nullable<long> PtInvoiceTypeID
        {
            get
            {
                return _PtInvoiceTypeID;
            }
            set
            {
                OnPtInvoiceTypeIDChanging(value);
                ////ReportPropertyChanging("PtInvoiceTypeID");
                _PtInvoiceTypeID = value;
                RaisePropertyChanged("PtInvoiceTypeID");
                OnPtInvoiceTypeIDChanged();
            }
        }
        private Nullable<long> _PtInvoiceTypeID;
        partial void OnPtInvoiceTypeIDChanging(Nullable<long> value);
        partial void OnPtInvoiceTypeIDChanged();





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
                ////ReportPropertyChanging("TransactionID");
                _TransactionID = value;
                RaisePropertyChanged("TransactionID");
                OnTransactionIDChanged();
            }
        }
        private long _TransactionID;
        partial void OnTransactionIDChanging(long value);
        partial void OnTransactionIDChanged();





        [DataMemberAttribute()]
        public Nullable<long> CurrencyID
        {
            get
            {
                return _CurrencyID;
            }
            set
            {
                OnCurrencyIDChanging(value);
                ////ReportPropertyChanging("CurrencyID");
                _CurrencyID = value;
                RaisePropertyChanged("CurrencyID");
                OnCurrencyIDChanged();
            }
        }
        private Nullable<long> _CurrencyID;
        partial void OnCurrencyIDChanging(Nullable<long> value);
        partial void OnCurrencyIDChanged();





        [DataMemberAttribute()]
        public Nullable<long> PayModeID
        {
            get
            {
                return _PayModeID;
            }
            set
            {
                OnPayModeIDChanging(value);
                ////ReportPropertyChanging("PayModeID");
                _PayModeID = value;
                RaisePropertyChanged("PayModeID");
                OnPayModeIDChanged();
            }
        }
        private Nullable<long> _PayModeID;
        partial void OnPayModeIDChanging(Nullable<long> value);
        partial void OnPayModeIDChanged();





        [DataMemberAttribute()]
        public Nullable<DateTime> InvoiceDate
        {
            get
            {
                return _InvoiceDate;
            }
            set
            {
                OnInvoiceDateChanging(value);
                ////ReportPropertyChanging("InvoiceDate");
                _InvoiceDate = value;
                RaisePropertyChanged("InvoiceDate");
                OnInvoiceDateChanged();
            }
        }
        private Nullable<DateTime> _InvoiceDate;
        partial void OnInvoiceDateChanging(Nullable<DateTime> value);
        partial void OnInvoiceDateChanged();





        [DataMemberAttribute()]
        public String TaxInvoiceNumber
        {
            get
            {
                return _TaxInvoiceNumber;
            }
            set
            {
                OnTaxInvoiceNumberChanging(value);
                ////ReportPropertyChanging("TaxInvoiceNumber");
                _TaxInvoiceNumber = value;
                RaisePropertyChanged("TaxInvoiceNumber");
                OnTaxInvoiceNumberChanged();
            }
        }
        private String _TaxInvoiceNumber;
        partial void OnTaxInvoiceNumberChanging(String value);
        partial void OnTaxInvoiceNumberChanged();





        [DataMemberAttribute()]
        public String TaxCode
        {
            get
            {
                return _TaxCode;
            }
            set
            {
                OnTaxCodeChanging(value);
                ////ReportPropertyChanging("TaxCode");
                _TaxCode = value;
                RaisePropertyChanged("TaxCode");
                OnTaxCodeChanged();
            }
        }
        private String _TaxCode;
        partial void OnTaxCodeChanging(String value);
        partial void OnTaxCodeChanged();





        [DataMemberAttribute()]
        public Nullable<Boolean> IsPrinted
        {
            get
            {
                return _IsPrinted;
            }
            set
            {
                OnIsPrintedChanging(value);
                ////ReportPropertyChanging("IsPrinted");
                _IsPrinted = value;
                RaisePropertyChanged("IsPrinted");
                OnIsPrintedChanged();
            }
        }
        private Nullable<Boolean> _IsPrinted;
        partial void OnIsPrintedChanging(Nullable<Boolean> value);
        partial void OnIsPrintedChanged();





        [DataMemberAttribute()]
        public Nullable<DateTime> FromDate
        {
            get
            {
                return _FromDate;
            }
            set
            {
                OnFromDateChanging(value);
                ////ReportPropertyChanging("FromDate");
                _FromDate = value;
                RaisePropertyChanged("FromDate");
                OnFromDateChanged();
            }
        }
        private Nullable<DateTime> _FromDate;
        partial void OnFromDateChanging(Nullable<DateTime> value);
        partial void OnFromDateChanged();





        [DataMemberAttribute()]
        public Nullable<DateTime> ToDate
        {
            get
            {
                return _ToDate;
            }
            set
            {
                OnToDateChanging(value);
                ////ReportPropertyChanging("ToDate");
                _ToDate = value;
                RaisePropertyChanged("ToDate");
                OnToDateChanged();
            }
        }
        private Nullable<DateTime> _ToDate;
        partial void OnToDateChanging(Nullable<DateTime> value);
        partial void OnToDateChanged();

        #endregion

        #region Navigation Properties

        [DataMemberAttribute()]
        public Currency Currency
        {
            get;
            set;
        }

        [DataMemberAttribute()]
        public ObservableCollection<KeepTrackPriting> KeepTrackPritings
        {
            get;
            set;
        }

        [DataMemberAttribute()]
        public PatientInvoiceType PatientInvoiceType
        {
            get;
            set;
        }

        [DataMemberAttribute()]
        public PatientTransaction PatientTransaction
        {
            get;
            set;
        }

        [DataMemberAttribute()]
        public RefPaymentMode RefPaymentMode
        {
            get;
            set;
        }

        #endregion
    }
}
