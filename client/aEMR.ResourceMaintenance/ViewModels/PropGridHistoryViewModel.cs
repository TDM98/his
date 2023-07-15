using System;
using System.ComponentModel.Composition;
using System.Windows;
using aEMR.ViewContracts;
using Caliburn.Micro;
using aEMR.Infrastructure;
using DataEntities;
using aEMR.Common.Collections;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;

namespace aEMR.ResourceMaintenance.ViewModels
{
    [Export(typeof(IPropGridHistory)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class PropGridHistoryViewModel : Conductor<object>, IPropGridHistory
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public PropGridHistoryViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;
            _allRsrcPropLocBreak = new PagedSortableCollectionView<ResourcePropLocations>();
            allRsrcPropLocBreak.OnRefresh += new EventHandler<RefreshEventArgs>(allRsrcPropLocBreak_OnRefresh);
        }

        void allRsrcPropLocBreak_OnRefresh(object sender, RefreshEventArgs e)
        {
            
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
        private PagedSortableCollectionView<ResourcePropLocations> _allRsrcPropLocBreak;
        public PagedSortableCollectionView<ResourcePropLocations> allRsrcPropLocBreak
        {
            get
            {
                return _allRsrcPropLocBreak;
            }
            set
            {
                if (_allRsrcPropLocBreak == value)
                    return;
                _allRsrcPropLocBreak = value;
                NotifyOfPropertyChange(() => allRsrcPropLocBreak);
            }
        }

        private ResourcePropLocations _selectedRsrcPropLocMove;
        public ResourcePropLocations selectedRsrcPropLocMove
        {
            get
            {
                return _selectedRsrcPropLocMove;
            }
            set
            {
                if (_selectedRsrcPropLocMove == value)
                    return;
                _selectedRsrcPropLocMove = value;
                NotifyOfPropertyChange(() => selectedRsrcPropLocMove);
            }
        }
    
        public void lnkMoveHistoryClick(RoutedEventArgs e)
        {
            if (selectedRsrcPropLocMove != null)
            {
                //var PropGridMoveHis= Globals.GetViewModel<IPropMoveHistory>();
                //PropGridMoveHis.RscrID = selectedRsrcPropLocMove.VRscrProperty.RscrPropertyID;
                //this.ActivateItem(PropGridMoveHis);
                //var instance = PropGridMoveHis as Conductor<object>;
                //Globals.ShowDialog(instance, (o) => { });

                Action<IPropMoveHistory> onInitDlg = (PropGridMoveHis) =>
                {
                    PropGridMoveHis.RscrID = selectedRsrcPropLocMove.VRscrProperty.RscrPropertyID;
                    this.ActivateItem(PropGridMoveHis);
                };
                GlobalsNAV.ShowDialog<IPropMoveHistory>(onInitDlg);
            }

        }
    }
}
