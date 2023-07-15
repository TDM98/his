using System.ComponentModel.Composition;
using aEMR.Infrastructure.Events;
using aEMR.ViewContracts;
using Caliburn.Micro;
using aEMR.Infrastructure;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;

namespace aEMR.ResourceMaintenance.ViewModels
{
    [Export(typeof(ITranfHome)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class TranfHomeViewModel : Conductor<object>, ITranfHome
        , IHandle<InfoTransViewModelEvent_ChooseRscrForMaintenance>
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public TranfHomeViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            eventAggregator.Subscribe(this);
            //Globals.EventAggregator.Subscribe(this);
            var treeDept = Globals.GetViewModel<IDepartmentTree>();
            var PropGrid = Globals.GetViewModel<IPropGrid>();
            var infoView = Globals.GetViewModel<IResourceInfoTrans>();
            infoView.IsChildWindowForChonDiBaoTri = IsChildWindowForChonDiBaoTri;

            leftContent = treeDept;
            this.ActivateItem(treeDept);
            gridContent = PropGrid;
            this.ActivateItem(PropGrid);
            infoContent = infoView;
            this.ActivateItem(infoView);
        }
        
        protected override void OnActivate()
        {
            base.OnActivate();
            var treeDept = Globals.GetViewModel<IDepartmentTree>();
            var PropGrid = Globals.GetViewModel<IPropGrid>();
            var infoView = Globals.GetViewModel<IResourceInfoTrans>();
            infoView.IsChildWindowForChonDiBaoTri = IsChildWindowForChonDiBaoTri;
            leftContent = treeDept;
            this.ActivateItem(treeDept);
            gridContent = PropGrid;
            this.ActivateItem(PropGrid);
            infoContent = infoView;
            this.ActivateItem(infoView);
        }

        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);
            Globals.EventAggregator.Unsubscribe(this);
        }
        
        private object _leftContent;

        private object _infoContent;

        private object _gridContent;

        public object leftContent
        {
            get
            {
                return _leftContent;
            }
            set
            {
                if (_leftContent == value)
                    return;
                _leftContent = value;
                NotifyOfPropertyChange(()=>leftContent);
            }
        }

        public object infoContent
        {
            get
            {
                return _infoContent;
            }
            set
            {
                if (_infoContent == value)
                    return;
                _infoContent = value;
                NotifyOfPropertyChange(() => infoContent);
            }
        }

        public object gridContent
        {
            get
            {
                return _gridContent;
            }
            set
            {
                if (_gridContent == value)
                    return;
                _gridContent = value;
                NotifyOfPropertyChange(() => gridContent);
            }
        }


        private bool _IsChildWindowForChonDiBaoTri=false;
        public bool IsChildWindowForChonDiBaoTri
        {
            get { return _IsChildWindowForChonDiBaoTri; }
            set
            {
                if(_IsChildWindowForChonDiBaoTri!=value)
                {
                    _IsChildWindowForChonDiBaoTri = value;
                    NotifyOfPropertyChange(()=>IsChildWindowForChonDiBaoTri);
                }
            }
        }


        public void Handle(InfoTransViewModelEvent_ChooseRscrForMaintenance message)
        {
            if(message!=null)
                TryClose();
        }
    }
}
