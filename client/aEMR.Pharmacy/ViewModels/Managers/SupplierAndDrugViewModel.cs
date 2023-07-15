using System.ComponentModel.Composition;
using aEMR.ViewContracts;
using Caliburn.Micro;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using aEMR.Infrastructure;


namespace aEMR.Pharmacy.ViewModels
{
    [Export(typeof(ISupplierAndDrug)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class SupplierAndDrugViewModel : Conductor<object>, ISupplierAndDrug
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public SupplierAndDrugViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            var uc1 = Globals.GetViewModel<IAddSupplierForDrug>();
            AddSupplierForDrug = uc1;
            ActivateItem(AddSupplierForDrug);

            var uc2 = Globals.GetViewModel<IAddDrugForSupplier>();
            AddDrugForSupplier = uc2;
            ActivateItem(AddDrugForSupplier);
        }
        public object AddSupplierForDrug
        {
            get;
            set;
        }
        public object AddDrugForSupplier
        {
            get;
            set;
        }

    }
}
