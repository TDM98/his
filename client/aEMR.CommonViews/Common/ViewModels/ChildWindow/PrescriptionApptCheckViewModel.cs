using System.ComponentModel.Composition;
using Caliburn.Micro;
using aEMR.ViewContracts;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using aEMR.Infrastructure.Events;
using aEMR.Infrastructure;

namespace aEMR.Common.ViewModels
{
    [Export(typeof(IPrescriptionApptCheck)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class PrescriptionApptCheckViewModel : Conductor<object>, IPrescriptionApptCheck
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public PrescriptionApptCheckViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;
        }

        private bool _HasAppointment=false;
        public bool HasAppointment
        {
            get { return _HasAppointment; }
            set
            {
                _HasAppointment = value;
                NotifyOfPropertyChange(() => HasAppointment);
            }
        }

        
        public void OkCmd()
        {
            Globals.EventAggregator.Publish(new WarningConfirmMsgBoxClose() { IsConfirmed = HasAppointment });
            TryClose();
        }
    }
}
