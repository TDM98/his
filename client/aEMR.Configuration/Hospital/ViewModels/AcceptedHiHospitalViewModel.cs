using System.ComponentModel.Composition;
using aEMR.ViewContracts.Configuration;
using Caliburn.Micro;
using Castle.Windsor;
using Castle.Core.Logging;
using aEMR.ViewContracts;
using aEMR.Infrastructure.CachingUtils;

//namespace aEMR.Configuration.Hospitals.ViewModels
//Chỉnh namespace cho giống với xaml.
namespace aEMR.Hospitals.ViewModels
{
    [Export(typeof(IAcceptedHiHospital)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class AcceptedHiHospitalViewModel : Conductor<object>, IAcceptedHiHospital
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public AcceptedHiHospitalViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        { 
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;
        }
    }
}