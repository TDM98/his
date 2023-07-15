using System.ComponentModel.Composition;
using aEMR.ViewContracts;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using Caliburn.Micro;

namespace aEMR.PCLDepartment.ViewModels
{
    [Export(typeof (ISATGSDobuQuyTrinh)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class SATGSDobuQuyTrinhViewModel : Conductor<object>, ISATGSDobuQuyTrinh
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public SATGSDobuQuyTrinhViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;
        }
    }
}