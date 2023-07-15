using eHCMSLanguage;
using System;
using System.Threading;
using System.Windows;
using Castle.Windsor;
using Castle.Core.Logging;
using aEMR.Infrastructure.CachingUtils;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using Caliburn.Micro;
using aEMR.ViewContracts;
using System.ComponentModel.Composition;
using DataEntities;

namespace aEMR.UserAccountManagement.ViewModels
{
    [Export(typeof(IcwdRoleUpdate)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class cwdRoleUpdateViewModel : Conductor<object>, IcwdRoleUpdate
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public cwdRoleUpdateViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;
        }
        protected override void  OnActivate()
        {
 	         base.OnActivate();
        }

        private Role _SelectedRole;
        public Role SelectedRole
        {
            get
            {
                return _SelectedRole;
            }
            set
            {
                if (_SelectedRole == value)
                    return;
                _SelectedRole = value;
                NotifyOfPropertyChange(() => SelectedRole);
            }
        }

        public void butUpdate()
        {
            UpdateRoles(SelectedRole.RoleID, SelectedRole.RoleName, SelectedRole.Description);
            Globals.EventAggregator.Publish(new RoleChangeCompletedEvent{});
            TryClose();
        }
        public void butCancel()
        {
            TryClose();
        }
        private void UpdateRoles(int RoleID, string RoleName, string Description)
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                using (var serviceFactory = new UserAccountsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginUpdateRoles( RoleID,  RoleName,  Description, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndUpdateRoles(asyncResult);
                            if (results == true)
                            {
                                //Globals.ShowMessage("Thêm thành công!", eHCMSResources.T0432_G1_Error);
                                MessageBox.Show(eHCMSResources.A0279_G1_Msg_InfoCNhatOK);
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

