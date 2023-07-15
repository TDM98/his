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
using aEMR.Common;
using Caliburn.Micro;
using DataEntities;
using aEMR.Common.Collections;

namespace aEMR.UserAccountManagement.ViewModels
{
    [Export(typeof(IGridUserGroup)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class GridUserGroupViewModel : Conductor<object>, IGridUserGroup,IHandle<SelectGroupChangeEvent>
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public GridUserGroupViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;
            _allUserGroup =new PagedSortableCollectionView<UserGroup>();
            _allUserGroup.OnRefresh += new EventHandler<RefreshEventArgs>(_allUserGroup_OnRefresh);
            
        }

        void _allUserGroup_OnRefresh(object sender, RefreshEventArgs e)
        {
            GetAllUserGroupGetByID(0, groupID, allUserGroup.PageSize, allUserGroup.PageIndex, "", true);
        }
        protected override void  OnActivate()
        {
 	         base.OnActivate();
             Globals.EventAggregator.Subscribe(this);
             //==== 20161206 CMN Begin: Disable method called in onloaded
             //_allUserGroup = new PagedSortableCollectionView<UserGroup>();
             //_allUserGroup.OnRefresh += new EventHandler<RefreshEventArgs>(_allUserGroup_OnRefresh);
            //==== 20161206 CMN End.
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

        private PagedSortableCollectionView<UserGroup> _allUserGroup;
        public PagedSortableCollectionView<UserGroup> allUserGroup
        {
            get
            {
                return _allUserGroup;
            }
            set
            {
                if (_allUserGroup == value)
                    return;
                _allUserGroup = value;
                NotifyOfPropertyChange(() => allUserGroup);
            }
        }

        private UserGroup _SelectedUserGroup;
        public UserGroup SelectedUserGroup
        {
            get
            {
                return _SelectedUserGroup;
            }
            set
            {
                if (_SelectedUserGroup == value)
                    return;
                _SelectedUserGroup = value;
                NotifyOfPropertyChange(() => SelectedUserGroup);
            }
        }

        public long groupID = 0;
        #endregion
        public void DoubleClick(object sender)
        {

        }
        public void Handle(SelectGroupChangeEvent obj)
        {
            if (obj != null)
            {
                groupID = ((Group)obj.SelectedGroup).GroupID;
                allUserGroup.PageIndex = 0;
                GetAllUserGroupGetByID(0, groupID, allUserGroup.PageSize, allUserGroup.PageIndex, "", true);
            }
        }
        #region method
        private void GetAllUserGroupGetByID(long AccountID, long GroupID, int PageSize, int PageIndex, string OrderBy,
                             bool CountTotal)
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            IsLoading = true;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new UserAccountsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetAllUserGroupGetByID(AccountID, GroupID, PageSize, PageIndex, OrderBy,
                             CountTotal, Globals.DispatchCallback((asyncResult) =>
                             {
                                 try
                                 {
                                     int Total = 0;
                                     var results = contract.EndGetAllUserGroupGetByID(out Total, asyncResult);
                                     if (results != null)
                                     {
                                         if (allUserGroup == null)
                                         {
                                             allUserGroup = new PagedSortableCollectionView<UserGroup>();
                                         }
                                         else
                                         {
                                             allUserGroup.Clear();
                                         }
                                         if (!Globals.isEncrypt)
                                         {
                                             foreach (var p in results)
                                             {
                                                 allUserGroup.Add(p);
                                             }
                                         }
                                         else
                                         {
                                             foreach (var p in results)
                                             {
                                                 try
                                                 {
                                                     //if (Globals.isEncrypt)
                                                     {

                                                         p.UserAccount.AccountName =
                                                             EncryptExtension.Decrypt(p.UserAccount.AccountName,
                                                                                      Globals.AxonKey, Globals.AxonPass);
                                                         if (p.UserAccount.AccountName == "")
                                                         {
                                                             p.UserAccount.AccountName = "Chưa Encrypt";
                                                         }
                                                     }

                                                 }
                                                 catch
                                                 {
                                                     p.UserAccount.AccountName = "Chưa Encrypt";
                                                 }

                                                 allUserGroup.Add(p);
                                             }
                                         }
                                         allUserGroup.TotalItemCount = Total;
                                         NotifyOfPropertyChange(() => allUserGroup);
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
