using eHCMSLanguage;
using System.ComponentModel.Composition;
using aEMR.Infrastructure.Events;
using aEMR.ViewContracts;
using Caliburn.Micro;
using aEMR.Infrastructure;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using DataEntities;
using System;

namespace aEMR.ResourceMaintenance.ViewModels
{
    [Export(typeof(IResourceInfo)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class InfoViewModel : Conductor<object>, IResourceInfo, IHandle<ResourceSelectedEvent>, IHandle<DeptLocSelectedEvent>
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public InfoViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            eventAggregator.Subscribe(this);
            //Globals.EventAggregator.Subscribe(this);

        }
        protected override void OnActivate()
        {
            base.OnActivate();
            authorization();
        }

        private object _ActiveContent;
        public object ActiveContent
        {
            get
            {
                return _ActiveContent;
            }
            set
            {
                _ActiveContent = value;
                NotifyOfPropertyChange(()=>ActiveContent);
            }
        }

        private Resources _CurrentResource;
        public Resources CurrentResource
        {
            get
            {
                return _CurrentResource;
            }
            set
            {
                if (_CurrentResource == value)
                    return;
                _CurrentResource = value;
                NotifyOfPropertyChange(() => CurrentResource);
            }
        }

        private RefDepartmentsTree _CurrentDeptLoc;
        public RefDepartmentsTree CurrentDeptLoc
        {
            get
            {
                return _CurrentDeptLoc;
            }
            set
            {
                if (_CurrentDeptLoc == value)
                    return;
                _CurrentDeptLoc = value;
                NotifyOfPropertyChange(() => CurrentDeptLoc);
            }
        }

#region overide event in Interface
        public void Handle(ResourceSelectedEvent Obj)
        {
            if (Obj != null)
            {
                CurrentResource = (Resources) Obj.curResource;
            }
        }
        public void Handle(DeptLocSelectedEvent Obj)
        {
            if (Obj != null)
            {
                CurrentDeptLoc = (RefDepartmentsTree)Obj.curDeptLoc;
            }
        }

        public void authorization()
        {
            if (!Globals.isAccountCheck)
            {
                return;
            }
            bResourceAlloc = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mResources
                                                      , (int)eResources.mPtDashboardNewAllocations
                                                      , (int)oResourcesEx.mResourceAllocation
                                                      , (int)ePermission.mAdd); 
        }
#region checking account

        private bool _bResourceAlloc = true;
        public bool bResourceAlloc
        {
            get
            {
                return _bResourceAlloc;
            }
            set
            {
                if (_bResourceAlloc == value)
                    return;
                _bResourceAlloc = value;
            }
        }

#endregion
        
        public void ResourceAlloc()
        {
            if (CurrentDeptLoc != null && CurrentResource!=null)
            {
                //var proAlloc = Globals.GetViewModel<IPropertyAllocations>();
                //var instance = proAlloc as Conductor<object>;
                //Globals.ShowDialog(instance,(o)=> { });

                Action<IPropertyAllocations> onInitDlg = (Alloc) =>
                {
                  
                };
                GlobalsNAV.ShowDialog<IPropertyAllocations>(onInitDlg);

                Globals.EventAggregator.Publish(new PropDeptEvent(CurrentResource,CurrentDeptLoc));
            }
            else
            {
                Globals.ShowMessage(eHCMSResources.Z1747_G1_ChuaChonPhgHoacVtuPBo, eHCMSResources.G0442_G1_TBao, (o) => { });
            }
        }

        #endregion
    }
}
