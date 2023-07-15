using eHCMSLanguage;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using aEMR.CommonTasks;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ViewContracts;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using Caliburn.Micro;
using DataEntities;
using System.Windows;
using aEMR.Common.BaseModel;

namespace aEMR.Common.ViewModels
{
    [Export(typeof(ISimplePayPharmacy)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class SimplePayForDrugViewModel : ViewModelBase, ISimplePayPharmacy
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public SimplePayForDrugViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            //if (!DesignerProperties.IsInDesignTool)
            bool designTime = System.ComponentModel.DesignerProperties.GetIsInDesignMode(new DependencyObject());
            if (!designTime)
            {
                Coroutine.BeginExecute(LoadPaymentTypes());
                Coroutine.BeginExecute(LoadPaymentModes());
                Coroutine.BeginExecute(LoadCurrency());
                Coroutine.BeginExecute(LoadPatientPaymentAccounts());
            }
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            Globals.EventAggregator.Subscribe(this);
        }
        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);
            Globals.EventAggregator.Unsubscribe(this);
        }

        private ObservableCollection<PatientPaymentAccount> _PatientPaymentAccounts;
        public ObservableCollection<PatientPaymentAccount> PatientPaymentAccounts
        {
            get { return _PatientPaymentAccounts; }
            set
            {
                _PatientPaymentAccounts = value;
                NotifyOfPropertyChange(() => PatientPaymentAccounts);
            }
        }

        private ObservableCollection<Lookup> _paymentModeList;
        public ObservableCollection<Lookup> PaymentModeList
        {
            get { return _paymentModeList; }
            set
            {
                _paymentModeList = value;
                NotifyOfPropertyChange(() => PaymentModeList);
            }
        }

        private ObservableCollection<Lookup> _paymentTypeList;
        public ObservableCollection<Lookup> PaymentTypeList
        {
            get { return _paymentTypeList; }
            set
            {
                _paymentTypeList = value;
                NotifyOfPropertyChange(() => PaymentTypeList);
            }
        }

        private ObservableCollection<Lookup> _currencyList;
        public ObservableCollection<Lookup> CurrencyList
        {
            get { return _currencyList; }
            set
            {
                _currencyList = value;
                NotifyOfPropertyChange(() => CurrencyList);
            }
        }

        public long V_TradingPlaces { get; set; }

        private PatientTransactionPayment _currentPayment;
        public PatientTransactionPayment CurrentPayment
        {
            get
            {
                return _currentPayment;
            }
            set
            {
                _currentPayment = value;
                NotifyOfPropertyChange(() => CurrentPayment);
            }
        }
        private void ResetPatientPayment()
        {
            CurrentPayment = new PatientTransactionPayment();
            CurrentPayment.PaymentMode = new Lookup() { LookupID = DefaultPaymentModeID };
            CurrentPayment.PaymentType = new Lookup() { LookupID = DefaultPaymentTypeID };
            CurrentPayment.Currency = new Lookup() { LookupID = DefaultCurrencyID };
            CurrentPayment.PtPmtAccID = 1;
            CurrentPayment.PayAmount = TotalPaySuggested;// TotalPayForSelectedItem > 0 ? TotalPayForSelectedItem : TotalPaySuggested;
        }

        public long DefaultCurrencyID
        {
            get
            {
                return (long)AllLookupValues.Currency.VND;
            }
        }

        public long DefaultPaymentModeID
        {
            get
            {
                return (long)AllLookupValues.PaymentMode.TIEN_MAT;
            }
        }

        public long DefaultPaymentTypeID
        {
            get
            {
                return (long)AllLookupValues.PaymentType.TRA_DU;
            }
        }

        public object ObjectState { get; set; }

        private bool _payForSelectedItemOnly;
        public bool PayForSelectedItemOnly
        {
            get { return _payForSelectedItemOnly; }
            set
            {
                _payForSelectedItemOnly = value;
            }
        }

        public override string ChildWindowTitle
        {
            get
            {
                return eHCMSResources.Z1250_G1_ThuTienThuoc;
            }
        }

        public override bool IsProcessing
        {
            get
            {
                return _isPaying;
            }
        }
        public override string StatusText
        {
            get
            {
                if (_isPaying)
                {
                    return eHCMSResources.Z1243_G1_DangLuuTTinTinhTien;
                }
                return "";
            }
        }

        private bool _isPaying;
        public bool IsPaying
        {
            get
            {
                return _isPaying;
            }
            set
            {
                _isPaying = value;
                NotifyOfPropertyChange(() => IsPaying);
                NotifyWhenBusy();
            }
        }

      
        //private bool _canPayCmd;
        public bool CanPayCmd
        {
            get { return !IsPaying; }
        }

        public void PayCmd()
        {
            if (CurrentPayment.PayAmount != TotalPaySuggested)
            {
                if (TotalPaySuggested < 0)
                {
                    MessageBox.Show(string.Format("{0}!", eHCMSResources.A0987_G1_Msg_InfoTienTToanBangTienDNTToan));
                    return;
                }
                else
                {
                    MessageBox.Show(string.Format("{0}!", eHCMSResources.A0989_G1_Msg_InfoTienThuBangTienDNTToan));
                    return;
                }
            }
            else if (CurrentPayment.PayAmount < 0)
            {
                if (CurrentPayment.PaymentType == null || CurrentPayment.PaymentType.LookupID != (long)AllLookupValues.PaymentType.HOAN_TIEN)
                {
                    MessageBox.Show(eHCMSResources.K0343_G1_ChonLoaiTToanLaHTien);
                    return;
                }
                CurrentPayment.CreditOrDebit = -1;
            }
            else
            {
                if (CurrentPayment.PaymentType == null || CurrentPayment.PaymentType.LookupID != (long)AllLookupValues.PaymentType.TRA_DU)
                {
                    MessageBox.Show(eHCMSResources.K0344_G1_ChonLoaiTToanLaTraDu);
                    return;
                }
            }
            Globals.EventAggregator.Publish(new PharmacyPayEvent { CurPatientPayment = CurrentPayment });
            TryClose();
        }

        public void item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsChecked")
            {
                // CurrentPayment.PayAmount = CalcPatientPayment();
            }
        }

        private decimal _totalPayForSelectedItem;
        public decimal TotalPayForSelectedItem
        {
            get
            {
                return _totalPayForSelectedItem;
            }
            set
            {
                if (_totalPayForSelectedItem != value)
                {
                    _totalPayForSelectedItem = value;
                    NotifyOfPropertyChange(() => TotalPayForSelectedItem);
                }
            }
        }

        private decimal _totalPaySuggested;
        public decimal TotalPaySuggested
        {
            get
            {
                return _totalPaySuggested;
            }
            set
            {
                if (_totalPaySuggested != value)
                {
                    _totalPaySuggested = value;
                    NotifyOfPropertyChange(() => TotalPaySuggested);
                }
            }
        }

        public void CancelCmd()
        {
            TryClose();
        }

        public void StartCalculating()
        {
            ResetPatientPayment();
        }

        #region COROUTINES
        private IEnumerator<IResult> LoadPatientPaymentAccounts()
        {
            var patientpaymentaccounts = new LoadPatientPaymentAccountListTask();
            yield return patientpaymentaccounts;
            PatientPaymentAccounts = patientpaymentaccounts.PatientPaymentAccountList;
            yield break;
        }
        private IEnumerator<IResult> LoadPaymentTypes()
        {
            var paymentTypeTask = new LoadLookupListTask(LookupValues.PAYMENT_TYPE);
            yield return paymentTypeTask;
            PaymentTypeList = paymentTypeTask.LookupList;
            yield break;
        }
        private IEnumerator<IResult> LoadPaymentModes()
        {
            var paymentModeTask = new LoadLookupListTask(LookupValues.PAYMENT_MODE);
            yield return paymentModeTask;
            PaymentModeList = paymentModeTask.LookupList;
            yield break;
        }
        private IEnumerator<IResult> LoadCurrency()
        {
            var currencyTask = new LoadLookupListTask(LookupValues.CURRENCY);
            yield return currencyTask;
            CurrencyList = currencyTask.LookupList;
            yield break;
        }

        #endregion
    }
}