using eHCMSLanguage;
using System;
using System.Net;
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
    [Export(typeof(IcwdFunctionUpdate)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class cwdFunctionUpdateViewModel : Conductor<object>, IcwdFunctionUpdate
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public cwdFunctionUpdateViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;
        }
        protected override void  OnActivate()
        {
 	         base.OnActivate();
        }

        private Function _SelectedFunction;
        public Function SelectedFunction
        {
            get
            {
                return _SelectedFunction;
            }
            set
            {
                if (_SelectedFunction == value)
                    return;
                _SelectedFunction = value;
                NotifyOfPropertyChange(() => SelectedFunction);
            }
        }

        public void butUpdate()
        {
            UpdateFunction(SelectedFunction.FunctionID, SelectedFunction.eNum, (int)SelectedFunction.ModuleID,
                           SelectedFunction.FunctionName, SelectedFunction.FunctionDescription, 0);
            
            Globals.EventAggregator.Publish(new FunctionChangeCompletedEvent { });
            TryClose();
        }
        public void butCancel()
        {
            TryClose();
        }

        private void UpdateFunction(long FunctionID,int Enum,int moduleID, string FunctionName, string Description,byte idx)
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                using (var serviceFactory = new UserAccountsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginUpdateFunctions(FunctionID, Enum, moduleID, FunctionName, Description, idx, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndUpdateFunctions(asyncResult);
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

