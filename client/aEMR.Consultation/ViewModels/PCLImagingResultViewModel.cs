using System.ComponentModel.Composition;
using aEMR.ViewContracts;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using Caliburn.Micro;

namespace aEMR.ConsultantEPrescription.ViewModels
{
    [Export(typeof(IPCLImagingResult)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class PCLImagingResultViewModel : Conductor<object>, IPCLImagingResult
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public PCLImagingResultViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;
        }
    }
}
