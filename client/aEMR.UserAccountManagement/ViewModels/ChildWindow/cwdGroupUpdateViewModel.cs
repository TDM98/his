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
    [Export(typeof(IcwdGroupUpdate)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class cwdGroupUpdateViewModel : Conductor<object>, IcwdGroupUpdate
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public cwdGroupUpdateViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

        }
        protected override void  OnActivate()
        {
 	         base.OnActivate();
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

        public void butUpdate()
        {
            UpdateGroup(SelectedGroup.GroupID, SelectedGroup.GroupName, SelectedGroup.Description);
            Globals.EventAggregator.Publish(new GroupChangeCompletedEvent{});
            TryClose();
        }
        public void butCancel()
        {
            TryClose();
        }

        private void UpdateGroup(int GroupID, string GroupName, string Description)
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                using (var serviceFactory = new UserAccountsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginUpdateGroup( GroupID,  GroupName,  Description, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndUpdateGroup(asyncResult);
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

