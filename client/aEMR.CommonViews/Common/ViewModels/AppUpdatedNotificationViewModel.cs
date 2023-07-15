using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using aEMR.ViewContracts;
using aEMR.Infrastructure.CachingUtils;
using Caliburn.Micro;
using DataEntities;
using Castle.Windsor;
using Castle.Core.Logging;
using aEMR.Common.BaseModel;
using eHCMSLanguage;

namespace aEMR.Common.ViewModels
{
    [Export(typeof(IAppUpdatedNotification)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class AppUpdatedNotificationViewModel : ViewModelBase, IAppUpdatedNotification
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public AppUpdatedNotificationViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAgr, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            eventAgr.Subscribe(this);
        }
        public void OkCmd()
        {
            TryClose();
        }

        public override string ChildWindowTitle
        {
            get
            {
                return eHCMSResources.Z1043_G1_Notification;
            }
        }
        private string _content;

        public string Content
        {
            get { return _content; }
            set
            {
                _content = value;
                NotifyOfPropertyChange(() => Content);
            }
        }

        private string _header;

        public string Header
        {
            get { return _header; }
            set
            {
                _header = value;
                NotifyOfPropertyChange(() => Header);
            }
        }
    }
}
