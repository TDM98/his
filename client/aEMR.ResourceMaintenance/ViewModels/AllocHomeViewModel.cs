using System.ComponentModel.Composition;
using aEMR.ViewContracts;
using Caliburn.Micro;
using aEMR.Infrastructure;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;

namespace aEMR.ResourceMaintenance.ViewModels
{
    [Export(typeof(IAllocHome)),PartCreationPolicy(CreationPolicy.NonShared)]
    public class AllocHomeViewModel : Conductor<object>, IAllocHome
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public AllocHomeViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;
            //dua cac Interface vao cac content
            var treeDept = Globals.GetViewModel<IDepartmentTree>();
            leftContent = treeDept;
            (this as Conductor<object>).ActivateItem(treeDept);

            var resListGrid = Globals.GetViewModel<IResourcesListGridEx>();
            resListGrid.ResourceCategoryEnum = ResourceCategoryEnum;
            gridContent = resListGrid;
            this.ActivateItem(resListGrid);

            var info = Globals.GetViewModel<IResourceInfo>();
            infoContent = info;
            this.ActivateItem(info);
        }
        protected override void OnActivate()
        {
            base.OnActivate();
            var treeDept = Globals.GetViewModel<IDepartmentTree>();
            leftContent = treeDept;
            (this as Conductor<object>).ActivateItem(treeDept);

            var resListGrid = Globals.GetViewModel<IResourcesListGridEx>();
            resListGrid.ResourceCategoryEnum = ResourceCategoryEnum;
            gridContent = resListGrid;
            this.ActivateItem(resListGrid);

            var info = Globals.GetViewModel<IResourceInfo>();
            infoContent = info;
            this.ActivateItem(info);
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

        private long _ResourceCategoryEnum;
        public long ResourceCategoryEnum
        {
            get
            {
                return _ResourceCategoryEnum;
            }
            set
            {
                if (_ResourceCategoryEnum == value)
                    return;
                _ResourceCategoryEnum = value;
                NotifyOfPropertyChange(() => ResourceCategoryEnum);
                
            }
        }

    }
}
