using eHCMSLanguage;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Threading;
using System.Windows;
using Castle.Windsor;
using Castle.Core.Logging;
using aEMR.Infrastructure.CachingUtils;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using Caliburn.Micro;
using DataEntities;
using aEMR.Common;
using aEMR.Common.Collections;

/*
 * #001 20180921 TNHX: Apply BusyIndicator, remove ModuleLoadCompleteEvent, refactor code
*/
namespace aEMR.UserAccountManagement.ViewModels
{
    [Export(typeof(IUCModulesTreePer)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class UCModulesTreePerViewModel : Conductor<object>, IUCModulesTreePer
        , IHandle<ModuleTreeChangeEvent>
        , IHandle<ModuleTreeExChangeEvent>
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public UCModulesTreePerViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            eventAggregator.Subscribe(this);
            _allModulesTree = new ObservableCollection<ModulesTree>();
            //GetModulesTreeView();
            Globals.EventAggregator.Subscribe(this);
        }
        protected override void OnActivate()
        {
            base.OnActivate();
            //_allModulesTree = new ObservableCollection<ModulesTree>();
            //GetModulesTreeView();
            
            Globals.EventAggregator.Subscribe(this);
        }
        public override void DeactivateItem(object item, bool close)
        {
            base.DeactivateItem(item, close);
            Globals.EventAggregator.Unsubscribe(this);
        }
#region properties
        private ModulesTree _SelectedModulesTree;
        public ModulesTree SelectedModulesTree
        {
            get
            {
                return _SelectedModulesTree;
            }
            set
            {
                if (_SelectedModulesTree == value)
                    return;
                _SelectedModulesTree = value;
                NotifyOfPropertyChange(() => SelectedModulesTree);
                if (SelectedModulesTree.Level == 1)
                {
                    SelectedPermission = null;
                    Globals.EventAggregator.Publish(new OperationChangeEvent { curOperation = null });
                    Globals.EventAggregator.Publish(new FunctionChangeEvent { curFunction = null });
                    Globals.EventAggregator.Publish(new ModuleChangeEvent { curModuleTree = SelectedModulesTree });
                }
                else
                    if (SelectedModulesTree.Level == 2)
                    {
                        //SelectedModulesTree = ObjectCopier.DeepCopy(((ModulesTree)e.NewValue));
                        SelectedFunction = new Function();
                        SelectedFunction.FunctionID = Convert.ToInt32(SelectedModulesTree.NodeID);
                        SelectedFunction.FunctionName = (SelectedModulesTree.NodeText);
                        SelectedFunction.FunctionDescription = (SelectedModulesTree.Description);

                        Globals.EventAggregator.Publish(new OperationChangeEvent { curOperation = null });
                        Globals.EventAggregator.Publish(new FunctionChangeEvent { curFunction = SelectedFunction });

                        Globals.EventAggregator.Publish(new ModuleChangeEvent { curModuleTree = SelectedModulesTree });
                    }
                    else
                        if (SelectedModulesTree.Level == 3)
                        {
                            SelectedOperation = new Operation();
                            SelectedOperation.OperationID = Convert.ToInt32(SelectedModulesTree.NodeID);
                            SelectedOperation.OperationName = (SelectedModulesTree.NodeText);
                            SelectedOperation.Description = (SelectedModulesTree.Description);
                            SelectedPermission = new Permission();
                            SelectedPermission.OperationID = SelectedOperation.OperationID;
                            Globals.EventAggregator.Publish(new OperationChangeEvent { curOperation = SelectedOperation });
                            Globals.EventAggregator.Publish(new FunctionChangeEvent { curFunction = null });
                        }   
            }
        }

        private ModulesTree _SelectedModulesTreeTemp;
        public ModulesTree SelectedModulesTreeTemp
        {
            get
            {
                return _SelectedModulesTreeTemp;
            }
            set
            {
                if (_SelectedModulesTreeTemp == value)
                    return;
                _SelectedModulesTreeTemp = value;
                NotifyOfPropertyChange(() => SelectedModulesTreeTemp);
            }
        }
        

        private Permission _SelectedPermission;
        public Permission SelectedPermission
        {
            get
            {
                return _SelectedPermission;
            }
            set
            {
                if (_SelectedPermission == value)
                    return;
                _SelectedPermission = value;
                NotifyOfPropertyChange(() => SelectedPermission);
            }
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
#endregion

        public void treeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (e != null
                && e.NewValue != null)
            {
                /*▼====: #001*/
                if ((((ModulesTree)e.NewValue)).Level == 3)
                {
                    SelectedOperation = new Operation();
                    SelectedOperation.OperationID = Convert.ToInt32((((ModulesTree)e.NewValue)).NodeID);
                    SelectedOperation.OperationName = ((((ModulesTree)e.NewValue)).NodeText);
                    SelectedOperation.Description = ((((ModulesTree)e.NewValue)).Description);
                    SelectedPermission = new Permission();
                    SelectedPermission.OperationID = SelectedOperation.OperationID;
                    Globals.EventAggregator.Publish(new OperationChangeEvent { curOperation = SelectedOperation });
                    //Globals.EventAggregator.Publish(new FunctionChangeEvent { curFunction = null });
                }
                /*▲====: #001*/
            }
        }

        #region property

        private ObservableCollection<ModulesTree> _allModulesTree;
        public ObservableCollection<ModulesTree> allModulesTree
        {
            get
            {
                return _allModulesTree;
            }
            set
            {
                if (_allModulesTree == value)
                    return;
                _allModulesTree = value;
                NotifyOfPropertyChange(() => allModulesTree);
            }
        }

        #endregion
        public void Handle(ModuleTreeChangeEvent obj)
        {
            if(obj!=null)
            {
                GetModulesTreeView();
                SelectedModulesTreeTemp = obj.curModulesTree;
            }
        }
        public void Handle(ModuleTreeExChangeEvent obj)
        {
            if(obj!=null)
            {
                GetModulesTreeView();
                SelectedModulesTreeTemp = obj.curModulesTree;
            }
        }
        /*▼====: #001*/
        #region method
        private void GetModulesTreeView()
        {
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new UserAccountsServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetModulesTreeView(Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndGetModulesTreeView(asyncResult);
                                if (results != null)
                                {
                                    allModulesTree = results.ToObservableCollection();
                                    NotifyOfPropertyChange(() => allModulesTree);
                                    SelectedModulesTree = SelectedModulesTreeTemp;
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
        #endregion
    }
}
