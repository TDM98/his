using System;
using System.ComponentModel.Composition;
using System.Windows;
using Caliburn.Micro;
using Castle.Windsor;
using aEMR.Infrastructure.Events;
using System.Reflection;
using System.Deployment.Application;
using aEMR.ViewContracts;

namespace aEMRClient.ViewModels
{
    [Export(typeof(aEMR.ViewContracts.IBusyIndicatorPopupView)), PartCreationPolicy(CreationPolicy.Shared)]
    public class BusyIndicatorPopupViewModel : Conductor<object>, IBusyIndicatorPopupView,
                                               IHandle<HideBusyIndicatorEvent>
    {
        private readonly INavigationService _navigationService;
        
        [ImportingConstructor]
        public BusyIndicatorPopupViewModel(IWindsorContainer container, IEventAggregator eventAggregator, INavigationService navigationService)           
        {
            _navigationService = navigationService;
            eventAggregator.Subscribe(this);            
        }

        public string strBusyMsg { get; set; }

        
        private bool _isPopupBusy;
        public bool IsPopupBusy
        {
            get { return _isPopupBusy; }
            set
            {
                _isPopupBusy = value;
                NotifyOfPropertyChange(() => IsPopupBusy);
            }
        }
     
        public void Initial()
        {
        }        

        Window _myBusyWindow = null;
        public void BusyWindowLoaded(object source)
        {
            _myBusyWindow = (Window)source;
        }

        public void Handle(HideBusyIndicatorEvent message)
        {
            IsPopupBusy = false;                       
            TryClose();
        }

    }
}
