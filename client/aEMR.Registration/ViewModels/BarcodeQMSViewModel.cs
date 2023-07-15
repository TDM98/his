using System.ComponentModel.Composition;
using aEMR.ViewContracts;
using Caliburn.Micro;
using aEMR.Common.BaseModel;
using Castle.Windsor;
using aEMR.Infrastructure;
using Castle.Core.Logging;
using aEMR.Infrastructure.CachingUtils;
using System.Windows.Controls;
using System.Windows;
using aEMR.Controls;

namespace aEMR.Registration.ViewModels
{
    [Export(typeof(IBarcodeQMS)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class BarcodeQMSViewModel : ViewModelBase, IBarcodeQMS
    {
        IEventAggregator _eventArg;
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;
        [ImportingConstructor]
        public BarcodeQMSViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;
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

        private string _BarcodeText = "";
        public string BarcodeText
        {
            get { return _BarcodeText; }
            set
            {
                if (_BarcodeText != value)
                {
                    _BarcodeText = value;
                    NotifyOfPropertyChange(() => BarcodeText);
                }
            }
        }
        AxBarcode QMSBarcode = null;
        public void QMSBarcode_Loaded(object sender, RoutedEventArgs e)
        {
            QMSBarcode = sender as AxBarcode;
            SetValueForBarcode();
        }
        public void SetValueForBarcode()
        {
            if (QMSBarcode != null && !string.IsNullOrEmpty(BarcodeText))
            {
                QMSBarcode.Code = BarcodeText;
            }
        }
    }
}
