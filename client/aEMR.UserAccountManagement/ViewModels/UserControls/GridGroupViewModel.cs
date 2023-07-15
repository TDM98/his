using eHCMSLanguage;
using System;
using System.ComponentModel.Composition;
using System.Net;
using System.Threading;
using Castle.Windsor;
using Castle.Core.Logging;
using aEMR.Infrastructure.CachingUtils;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using Caliburn.Micro;
using DataEntities;
using aEMR.Common.Collections;

namespace aEMR.UserAccountManagement.ViewModels
{
    [Export(typeof(IGridGroup)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class GridGroupViewModel : Conductor<object>, IGridGroup
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public GridGroupViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            _allGroup = new PagedSortableCollectionView<Group>();
            allGroup.OnRefresh += new EventHandler<RefreshEventArgs>(allGroup_OnRefresh);
            _allGroup.PageIndex = 0;
            GetAllGroupAllPaging(0, _allGroup.PageSize, _allGroup.PageIndex, "", true);
            
        }

        void allGroup_OnRefresh(object sender, RefreshEventArgs e)
        {
            GetAllGroupAllPaging(0, _allGroup.PageSize, _allGroup.PageIndex, "", true);
        }
        protected override void  OnActivate()
        {
 	         base.OnActivate();
             //GetAllGroupAllPaging(0, _allGroup.PageSize, _allGroup.PageIndex, "", true);
             //allGroup.OnRefresh += new EventHandler<RefreshEventArgs>(allGroup_OnRefresh);
             
        }
        private bool _IsLoading = false;
        public bool IsLoading
        {
            get { return _IsLoading; }
            set
            {
                if (_IsLoading != value)
                {
                    _IsLoading = value;
                    
                    NotifyOfPropertyChange(() => IsLoading);
                }
            }
        }

        #region property

        private PagedSortableCollectionView<Group> _allGroup;
        public PagedSortableCollectionView<Group> allGroup
        {
            get
            {
                return _allGroup;
            }
            set
            {
                if (_allGroup == value)
                    return;
                _allGroup = value;
                NotifyOfPropertyChange(() => allGroup);
            }
        }

        private Group _SelectedGroup;
        public Group SelectedGroup
        {
            get
            {
                return _SelectedGroup;
            }
            set
            {
                if (_SelectedGroup == value)
                    return;
                _SelectedGroup = value;
                NotifyOfPropertyChange(() => SelectedGroup);
            }
        }
        #endregion
        public void DoubleClick(object sender)
        {
            Globals.EventAggregator.Publish(new SelectGroupChangeEvent { SelectedGroup = SelectedGroup });
        }

        #region method
        private void GetAllGroupAllPaging(long GroupID, int PageSize, int PageIndex, string OrderBy,
                             bool CountTotal)
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            IsLoading = true;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new UserAccountsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetAllGroupAllPaging(GroupID, PageSize, PageIndex, OrderBy,
                             CountTotal, Globals.DispatchCallback((asyncResult) =>
                             {
                                 try
                                 {
                                     int Total = 0;
                                     var results = contract.EndGetAllGroupAllPaging(out Total, asyncResult);
                                     if (results != null)
                                     {
                                         if (allGroup == null)
                                         {
                                             allGroup = new PagedSortableCollectionView<Group>();
                                         }
                                         else
                                         {
                                             allGroup.Clear();
                                         }
                                         foreach (var p in results)
                                         {
                                             allGroup.Add(p);
                                         }
                                         allGroup.TotalItemCount = Total;
                                         NotifyOfPropertyChange(() => allGroup);
                                     }
                                 }
                                 catch (Exception ex)
                                 {
                                     Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                                 }
                                 finally
                                 {
                                     //Globals.IsBusy = false;
                                     IsLoading = false;
                                 }

                             }), null);

                }

            });

            t.Start();
        }

        #endregion
    }
}
