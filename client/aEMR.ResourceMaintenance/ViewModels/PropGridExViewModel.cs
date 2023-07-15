using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Windows;
using aEMR.Infrastructure.Events;
using aEMR.ViewContracts;
using Caliburn.Micro;
using aEMR.Infrastructure;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using DataEntities;

namespace aEMR.ResourceMaintenance.ViewModels
{
    [Export(typeof(IPropGridEx)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class PropGridExViewModel : Conductor<object>, IPropGridEx, IHandle<lstResourcePropLocationsEvent>
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public PropGridExViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            eventAggregator.Subscribe(this);
            //Globals.EventAggregator.Subscribe(this);
        }

        private ObservableCollection<ResourcePropLocations> _lstNewResourcePropLocations;
        public ObservableCollection<ResourcePropLocations> lstNewResourcePropLocations
        {
            get
            {
                return _lstNewResourcePropLocations;
            }
            set
            {
                if (_lstNewResourcePropLocations == value)
                    return;
                _lstNewResourcePropLocations = value;
                NotifyOfPropertyChange(() => lstNewResourcePropLocations);
            }
        }
        private ResourcePropLocations _selectedNewResourcePropLocations;
        public ResourcePropLocations selectedNewResourcePropLocations
        {
            get
            {
                return _selectedNewResourcePropLocations;
            }
            set
            {
                if (_selectedNewResourcePropLocations == value)
                    return;
                _selectedNewResourcePropLocations = value;
                NotifyOfPropertyChange(() => selectedNewResourcePropLocations);
            }
        }

        public void Handle(lstResourcePropLocationsEvent obj)
        {
            if (obj!=null)
            {
                lstNewResourcePropLocations = (ObservableCollection<ResourcePropLocations>)obj.lstResPropLocations;
            }
        }
        public void lnkDelete_Click(object sender,RoutedEventArgs e)
        {
            lstNewResourcePropLocations.Remove(selectedNewResourcePropLocations);
            Globals.EventAggregator.Publish(new lstResourcePropLocationsGridToFormEvent { lstResPropLocations = lstNewResourcePropLocations });

        }
    
    }
}
