using System.ComponentModel.Composition;
using aEMR.ViewContracts;
using Caliburn.Micro;
using DataEntities;
using Castle.Windsor;
using Castle.Core.Logging;
using aEMR.Infrastructure.CachingUtils;

namespace aEMR.Pharmacy.ViewModels
{
    [Export(typeof(IRefGenDrug_Show)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class RefGenDrug_ShowViewModel : Conductor<object>, IRefGenDrug_Show
    {

        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public RefGenDrug_ShowViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

        }

        // SelectedDrug
        private RefGenericDrugDetail _SelectedDrug;
        public RefGenericDrugDetail SelectedDrug
        {
            get
            {
                return _SelectedDrug;
            }
            set
            {
                if (_SelectedDrug != value)
                {
                    _SelectedDrug = value;
                    NotifyOfPropertyChange(() => SelectedDrug);
                }
            }
        }
    }
}
