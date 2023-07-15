using System.ComponentModel.Composition;
using aEMR.Infrastructure;
using aEMR.ViewContracts;
using aEMR.ViewContracts.Configuration;
using Caliburn.Micro;
using Castle.Windsor;
using Castle.Core.Logging;
using aEMR.Infrastructure.CachingUtils;

namespace aEMR.Configuration.BedAllocations.ViewModels
{
    [Export(typeof(IPtBedAllocations)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class PtBedAllocationsViewModel : Conductor<object>, IPtBedAllocations
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public PtBedAllocationsViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

        }
        protected override void OnActivate()
        {
            base.OnActivate();

            var DepartmentTreeVM = Globals.GetViewModel<IDeptTree>();
            UCDepartmentTree= DepartmentTreeVM;
            this.ActivateItem(DepartmentTreeVM);

            var UCBedAllocGridVM = Globals.GetViewModel<IUCBedAllocGrid>();
            UCBedAllocGrid= UCBedAllocGridVM;
            this.ActivateItem(UCBedAllocGridVM);

            //var UCRoomEditVM = Globals.GetViewModel<IUCRoomEdit>();
            //UCRoomEdit = UCRoomEditVM;
            //this.ActivateItem(UCRoomEditVM);

            var UCRoomInfoVM = Globals.GetViewModel<IUCRoomInfo>();
            UCRoomInfo = UCRoomInfoVM;
            this.ActivateItem(UCRoomInfoVM);
        }


        private object _UCDepartmentTree;

        public object UCDepartmentTree
        {
            get { return _UCDepartmentTree; }
            set
            {
                _UCDepartmentTree = value;
                NotifyOfPropertyChange(() => UCDepartmentTree);
            }
        }

        private object _UCBedAllocGrid;
        public object UCBedAllocGrid
        {
            get
            {
                return _UCBedAllocGrid;
            }
            set
            {
                if (_UCBedAllocGrid == value)
                    return;
                _UCBedAllocGrid = value;
                NotifyOfPropertyChange(() => UCBedAllocGrid);
            }
        }

        private object _UCRoomEdit;
        private object _UCRoomInfo;
        public object UCRoomEdit
        {
            get
            {
                return _UCRoomEdit;
            }
            set
            {
                if (_UCRoomEdit == value)
                    return;
                _UCRoomEdit = value;
                NotifyOfPropertyChange(() => UCRoomEdit);
            }
        }
        public object UCRoomInfo
        {
            get
            {
                return _UCRoomInfo;
            }
            set
            {
                if (_UCRoomInfo == value)
                    return;
                _UCRoomInfo = value;
                NotifyOfPropertyChange(() => UCRoomInfo);
            }
        }
    }
}
