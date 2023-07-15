using System.ComponentModel.Composition;
using aEMR.Infrastructure;
using aEMR.ViewContracts;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using Caliburn.Micro;
using System.Threading;
using System.Collections.Generic;
using DataEntities;
using aEMR.Common;
using aEMR.Common.Collections;
using aEMR.ServiceClient;
using System.Collections.ObjectModel;
using System;
using eHCMSLanguage;

/*
 * #001 20180921 TNHX: Add method for set AllGroup, allUserAccount, apply BusyIndicator
 */

namespace aEMR.UserAccountManagement.ViewModels
{
    [Export(typeof(IUserGroupTab)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class UserGroupTabViewModel : Conductor<object>, IUserGroupTab
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public UserGroupTabViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            var UCUserGroupFormVM = Globals.GetViewModel<IUCUserGroupForm>();
            UCUserGroupForm = UCUserGroupFormVM;
            this.ActivateItem(UCUserGroupFormVM);

            var UCUserGroupFormExVM = Globals.GetViewModel<IUCUserGroupFormEx>();
            UCUserGroupFormEx = UCUserGroupFormExVM;
            this.ActivateItem(UCUserGroupFormExVM);
            /*▼====: #001*/
            GetAllUserAccount(0);
            /*▲====: #001*/
        }
        protected override void OnActivate()
        {
            base.OnActivate();
            //==== 20161206 CMN Begin: Disable method called in onloaded
            //var UCUserGroupFormVM = Globals.GetViewModel<IUCUserGroupForm>();
            //UCUserGroupForm = UCUserGroupFormVM;
            //this.ActivateItem(UCUserGroupFormVM);

            //var UCUserGroupFormExVM = Globals.GetViewModel<IUCUserGroupFormEx>();
            //UCUserGroupFormEx = UCUserGroupFormExVM;
            //this.ActivateItem(UCUserGroupFormExVM);
            //==== 20161206 CMN End.
        }

        public object UCUserGroupFormEx { get; set; }
        public object UCUserGroupForm { get; set; }

        /*▼====: #001*/
        private ObservableCollection<UserAccount> _allUserAccount;
        public ObservableCollection<UserAccount> allUserAccount
        {
            get
            {
                return _allUserAccount;
            }
            set
            {
                if (_allUserAccount == value)
                    return;
                _allUserAccount = value;
                NotifyOfPropertyChange(() => allUserAccount);
            }
        }

        public void sortUserAccount()
        {
            for (int i = 0; i < allUserAccount.Count - 1; i++)
            {
                for (int j = i + 1; j < allUserAccount.Count; j++)
                {
                    if (String.Compare(allUserAccount[i].AccountName, allUserAccount[j].AccountName) > 0)
                    {
                        UserAccount temp = allUserAccount[i];
                        allUserAccount[i] = allUserAccount[j];
                        allUserAccount[j] = temp;
                    }
                }
            }            
        }

        private void GetAllUserAccount(long AccountID)
        {
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new UserAccountsServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetAllUserAccount(AccountID, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                List<UserAccount> results = contract.EndGetAllUserAccount(asyncResult);
                                if (results != null)
                                {
                                    allUserAccount = results.ToObservableCollection();

                                    foreach (var p in allUserAccount)
                                    {
                                        try
                                        {
                                            p.AccountName = EncryptExtension.Decrypt(p.AccountName, Globals.AxonKey, Globals.AxonPass);
                                            if (p.AccountName == "")
                                            {
                                                p.AccountName = "Chưa Encrypt";
                                                continue;
                                            }
                                        }
                                        catch
                                        {
                                            p.AccountName = "Chưa Encrypt";
                                        }
                                    }
                                    sortUserAccount();

                                    UserAccount usTemp = new UserAccount();
                                    usTemp.AccountID = -1;
                                    usTemp.AccountName = "-----Chọn Một User-----";
                                    allUserAccount.Insert(0, usTemp);

                                    ((IUCUserGroupForm)UCUserGroupForm).allUserAccount = allUserAccount;
                                    ((IUCUserGroupForm)UCUserGroupForm).SelectedUserAccount = usTemp;

                                    ((IUCUserGroupFormEx)UCUserGroupFormEx).allUserAccount = allUserAccount;
                                    ((IUCUserGroupFormEx)UCUserGroupFormEx).SelectedUserAccount = usTemp;
                                }
                            }
                            catch (Exception ex)
                            {
                                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                            }
                            finally
                            {
                                this.HideBusyIndicator();
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    this.HideBusyIndicator();
                }
            });

            t.Start();

        }

        private ObservableCollection<Group> _allGroup;
        public ObservableCollection<Group> allGroup
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
                ((IUCUserGroupForm)UCUserGroupForm).allGroup = allGroup;
                ((IUCUserGroupFormEx)UCUserGroupFormEx).allGroup = allGroup;

                ((IUCUserGroupForm)UCUserGroupForm).SelectedGroup = allGroup[0];
                ((IUCUserGroupFormEx)UCUserGroupFormEx).SelectedGroup = allGroup[0];
                NotifyOfPropertyChange(() => allGroup);
            }
        }
        /*▲====: #001*/
    }
}
