using System.ComponentModel.Composition;
using aEMR.ViewContracts;
using aEMR.Common.BaseModel;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;

namespace aEMR.Common.ViewModels
{
    [Export(typeof(ITotalBillInvoice)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class TotalBillInvoiceViewModel : ViewModelBase, ITotalBillInvoice
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public TotalBillInvoiceViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;
        }
        private decimal _TotalPatientPayment;
        public decimal TotalPatientPayment
        {
            get { return _TotalPatientPayment; }
            set
            {
                _TotalPatientPayment = value;
                NotifyOfPropertyChange(() => TotalPatientPayment);
            }
        }

        private decimal _TotalHIPayment;
        public decimal TotalHIPayment
        {
            get { return _TotalHIPayment; }
            set
            {
                _TotalHIPayment = value;
                NotifyOfPropertyChange(() => TotalHIPayment);
            }
        }

        private decimal _TotalBillInvoice;
        public decimal TotalBillInvoice
        {
            get { return _TotalBillInvoice; }
            set
            {
                _TotalBillInvoice = value;
                NotifyOfPropertyChange(() => TotalBillInvoice);
            }
        }

        private decimal _TotalRealHIPayment;
        public decimal TotalRealHIPayment
        {
            get { return _TotalRealHIPayment; }
            set
            {
                _TotalRealHIPayment = value;
                if (_TotalRealHIPayment > 0)
                {
                    ShowTotalRealHIPayment = true;
                }
                else
                {
                    ShowTotalRealHIPayment = false;
                }
                NotifyOfPropertyChange(() => TotalRealHIPayment);
            }
        }

        private bool _showTotalRealHIPayment;
        public bool ShowTotalRealHIPayment
        {
            get { return _showTotalRealHIPayment; }
            set
            {
                _showTotalRealHIPayment = value;
                NotifyOfPropertyChange(() => ShowTotalRealHIPayment);
            }
        }

        private bool _showErrorMessage;
        public bool ShowErrorMessage
        {
            get { return _showErrorMessage; }
            set
            {
                _showErrorMessage = value;
                NotifyOfPropertyChange(() => ShowErrorMessage);
            }
        }

        public void OKCmd()
        {
            this.TryClose();
        }

        //HPT_20160625: hiển thị tổng quỹ hỗ trợ cho bill. Chỉ nhìn thấy khi là bill kỹ thuật cao.
        private decimal _TotalCharitySupportFund;
        public decimal TotalCharitySupportFund
        {
            get { return _TotalCharitySupportFund; }
            set
            {
                _TotalCharitySupportFund = value;
                if (_TotalCharitySupportFund > 0)
                {
                    ShowTotalCharitySupportFund = true;
                }
                else
                {
                    ShowTotalCharitySupportFund = false;
                }
                NotifyOfPropertyChange(() => TotalCharitySupportFund);
            }
        }

        private bool _ShowTotalCharitySupportFund = true;
        public bool ShowTotalCharitySupportFund
        {
            get { return _ShowTotalCharitySupportFund; }
            set
            {
                _ShowTotalCharitySupportFund = value;
                NotifyOfPropertyChange(() => ShowTotalCharitySupportFund);
            }
        }
    }
}
