using System.ComponentModel.Composition;
using aEMR.Infrastructure;
using aEMR.ViewContracts;
using aEMR.ViewContracts.Configuration;
using Caliburn.Micro;
using DataEntities;
using Castle.Windsor;
using Castle.Core.Logging;
using aEMR.Infrastructure.CachingUtils;

namespace aEMR.Configuration.BedAllocations.ViewModels
{
    [Export(typeof(IUCRoomEdit)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class UCRoomEditViewModel : Conductor<object>, IUCRoomEdit
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public UCRoomEditViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;
            authorization();
        }
        protected override void OnActivate()
        {
            base.OnActivate();
            authorization();
            //Khi khoi tao module thi load menu ben trai luon.
            var shell = Globals.GetViewModel<IHome>();
            var leftMenu = Globals.GetViewModel<IConfigurationLeftMenu>();
            shell.LeftMenu = leftMenu;
            (shell as Conductor<object>).ActivateItem(leftMenu);
        }

        public void authorization()
        {
            if (!Globals.isAccountCheck)
            {
                return;
            }
            bbutAddBed = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConfiguration_Management
                                               , (int)eConfiguration_Management.mQuanLyGiuongBenh,
                                               (int)oConfigurationEx.mDatGiuongChoPhong, (int)ePermission.mAdd);

        }
        #region checking account

        private bool _bbutAddBed = true;
        public bool bbutAddBed
        {
            get
            {
                return _bbutAddBed;
            }
            set
            {
                if (_bbutAddBed == value)
                    return;
                _bbutAddBed = value;
            }
        }
        #endregion
        private object _mainContent;

        public object MainContent
        {
            get { return _mainContent; }
            set
            {
                _mainContent = value;
                NotifyOfPropertyChange(() => MainContent);
            }
        }
    }
}
