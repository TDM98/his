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
    [Export(typeof(IcwdOperationUpdate)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class cwdOperationUpdateViewModel : Conductor<object>, IcwdOperationUpdate
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public cwdOperationUpdateViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

        }
        protected override void  OnActivate()
        {
 	         base.OnActivate();
        }

        private Operation _SelectedOperation;
        public Operation SelectedOperation
        {
            get
            {
                return _SelectedOperation;
            }
            set
            {
                if (_SelectedOperation == value)
                    return;
                _SelectedOperation = value;
                NotifyOfPropertyChange(() => SelectedOperation);
            }
        }

        public void butUpdate()
        {
            UpdateOperation(SelectedOperation.OperationID, SelectedOperation.Enum,SelectedOperation.OperationName, SelectedOperation.Description);
            Globals.EventAggregator.Publish(new OperationChangeCompletedEvent { });
            TryClose();
        }
        public void butCancel()
        {
            TryClose();
        }

        private void UpdateOperation(long OperationID, int Enum, string OperationName, string Description)
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                using (var serviceFactory = new UserAccountsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginUpdateOperations(OperationID, Enum, OperationName, Description, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndUpdateOperations(asyncResult);
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

