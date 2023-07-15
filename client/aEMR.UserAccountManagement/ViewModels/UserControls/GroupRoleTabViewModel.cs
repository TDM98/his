using System.ComponentModel.Composition;
using Castle.Windsor;
using Castle.Core.Logging;
using aEMR.Infrastructure.CachingUtils;
using aEMR.Infrastructure;
using aEMR.ViewContracts;
using Caliburn.Micro;
using System.Collections.ObjectModel;
using DataEntities;
using eHCMSLanguage;
using System.Threading;
using System;
using aEMR.ServiceClient;
using aEMR.Common.Collections;

/*
 * #001 20180921 TNHX: Add method for set AllGroup, allRole, apply BusyIndicator, refactor code
 */

namespace aEMR.UserAccountManagement.ViewModels
{
    [Export(typeof(IGroupRoleTab)),PartCreationPolicy(CreationPolicy.NonShared)]
    public class GroupRoleTabViewModel : Conductor<object>, IGroupRoleTab
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public GroupRoleTabViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            var UCGroupRoleFormExVM = Globals.GetViewModel<IUCGroupRoleFormEx>();
            UCGroupRoleFormEx = UCGroupRoleFormExVM;
            this.ActivateItem(UCGroupRoleFormExVM);

            var UCGroupRoleFormVM = Globals.GetViewModel<IUCGroupRoleForm>();
            UCGroupRoleForm = UCGroupRoleFormVM;
            this.ActivateItem(UCGroupRoleFormVM);

            /*▼====: #001*/
            GetAllRoles();
            /*▲====: #001*/
        }
        protected override void OnActivate()
        {
            base.OnActivate();
            //==== 20161206 CMN Begin: Disable method called in onloaded
            //var UCGroupRoleFormExVM = Globals.GetViewModel<IUCGroupRoleFormEx>();
            //UCGroupRoleFormEx = UCGroupRoleFormExVM;
            //this.ActivateItem(UCGroupRoleFormExVM);

            //var UCGroupRoleFormVM = Globals.GetViewModel<IUCGroupRoleForm>();
            //UCGroupRoleForm = UCGroupRoleFormVM;
            //this.ActivateItem(UCGroupRoleFormVM);
            //==== 20161206 CMN End.


        }

        public object UCGroupRoleFormEx { get; set; }
        public object UCGroupRoleForm { get; set; }

        /*▼====: #001*/

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
                ((IUCGroupRoleFormEx)UCGroupRoleFormEx).allGroup = _allGroup;
                ((IUCGroupRoleForm)UCGroupRoleForm).allGroup = _allGroup;

                ((IUCGroupRoleFormEx)UCGroupRoleFormEx).SelectedGroup = _allGroup[0];
                ((IUCGroupRoleForm)UCGroupRoleForm).SelectedGroup = _allGroup[0];
                NotifyOfPropertyChange(() => allGroup);
            }
        }

        private ObservableCollection<Role> _allRole;
        public ObservableCollection<Role> allRole
        {
            get
            {
                return _allRole;
            }
            set
            {
                if (_allRole == value)
                    return;
                _allRole = value;
                NotifyOfPropertyChange(() => allRole);
            }
        }

        private void GetAllRoles()
        {
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new UserAccountsServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetAllRoles(Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndGetAllRoles(asyncResult);
                                if (results != null)
                                {
                                    allRole = results.ToObservableCollection();
                                    Role usTemp = new Role();
                                    usTemp.RoleID = -1;
                                    usTemp.RoleName = "-----Chọn Một Role-----";
                                    allRole.Insert(0, usTemp);

                                    ((IUCGroupRoleFormEx)UCGroupRoleFormEx).allRole = allRole;
                                    ((IUCGroupRoleForm)UCGroupRoleForm).allRole = allRole;

                                    ((IUCGroupRoleFormEx)UCGroupRoleFormEx).SelectedRole = allRole[0];
                                    ((IUCGroupRoleForm)UCGroupRoleForm).SelectedRole = allRole[0];

                                    NotifyOfPropertyChange(() => allRole);
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
        /*▲====: #001*/
    }
}
