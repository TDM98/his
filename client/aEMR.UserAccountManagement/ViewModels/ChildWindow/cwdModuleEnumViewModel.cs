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
    [Export(typeof(IcwdModuleEnum)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class cwdModuleEnumViewModel : Conductor<object>, IcwdModuleEnum
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public cwdModuleEnumViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

        }
        protected override void  OnActivate()
        {
 	         base.OnActivate();
        }

        private ModulesTree _SelectedModule;
        public ModulesTree SelectedModule
        {
            get
            {
                return _SelectedModule;
            }
            set
            {
                if (_SelectedModule == value)
                    return;
                _SelectedModule = value;
                NotifyOfPropertyChange(() => SelectedModule);
            }
        }

        public void butUpdate()
        {
            UpdateModulesEnum(SelectedModule.NodeID, (int)SelectedModule.eNum );
            TryClose();
        }
        public void butCancel()
        {
            TryClose();
        }
        private void UpdateModulesEnum(long ModuleID, int eNum)
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                using (var serviceFactory = new UserAccountsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginUpdateModulesEnum(ModuleID, eNum, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndUpdateModulesEnum(asyncResult);
                            if (results == true)
                            {
                                //Globals.ShowMessage("Update Module Enum thành công!", "");
                                MessageBox.Show(eHCMSResources.K0252_G1_CNhatModuleEnumOk);
                                //Globals.EventAggregator.Publish(new ModuleTreeExChangeEvent { curModulesTree = SelectedModulesTree });
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

