using System;
using System.ComponentModel.Composition;
using System.Threading;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using Caliburn.Micro;
using aEMR.Infrastructure;
using DataEntities;
using eHCMSLanguage;
using Castle.Windsor;
using Castle.Core.Logging;
using aEMR.Infrastructure.CachingUtils;

namespace aEMR.ResourceMaintenance.ViewModels
{
    [Export(typeof(IPropMoveHistory)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class PropMoveHistoryViewModel : Conductor<object>, IPropMoveHistory
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public PropMoveHistoryViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;
            _allRsrcPropLocMove =new BindableCollection<ResourcePropLocations>();
            
        }
        protected override void OnActivate()
        {
            base.OnActivate();
            GetResourcePropLocationsPagingByMove(RscrID, 10, 0, "", true);
        }
        private long _RscrID;
        public long RscrID
        {
            get
            {
                return _RscrID;
            }
            set
            {
                _RscrID = value;
                NotifyOfPropertyChange(() => RscrID);
            }
        }

        private IObservableCollection<ResourcePropLocations> _allRsrcPropLocMove;
        public IObservableCollection<ResourcePropLocations> allRsrcPropLocMove
        {
            get
            {
                return _allRsrcPropLocMove;
            }
            set
            {
                if (_allRsrcPropLocMove == value)
                    return;
                _allRsrcPropLocMove = value;
                NotifyOfPropertyChange(() => allRsrcPropLocMove);
            }
        }
        public void Exit()
        {
            TryClose();
        }
        
        private void GetResourcePropLocationsPagingByMove(long RscrPropertyID
                    , int PageSize, int PageIndex, string OrderBy, bool CountTotal)
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "" });

            var t = new Thread(() =>
            {
                using (var serviceFactory = new ResourcesManagementServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginGetResourcePropLocationsPagingByMove(RscrPropertyID
                                , PageSize, PageIndex, OrderBy, CountTotal,Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            int total = 0;
                            var items = contract.EndGetResourcePropLocationsPagingByMove(out total, asyncResult);
                            if(items!=null)
                            {
                                foreach (var tp in items)
                                {
                                    allRsrcPropLocMove.Add(tp);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            Globals.IsBusy = false;
                        }
                    }), null);
                }


            });
            t.Start();
        }
    }
}
