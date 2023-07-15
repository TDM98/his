using eHCMSLanguage;
using System;
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
using aEMR.Common.Collections;
using System.Collections.ObjectModel;

/*
 * #001 20180921 TNHX: Apply BusyIndicator, add allModulesTree for Parent's set then it set for child, refactor code
 */
namespace aEMR.UserAccountManagement.ViewModels
{
    [Export(typeof(IFunctionForm)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class FunctionFormViewModel : Conductor<object>, IFunctionForm
        , IHandle<ModuleTreeChangeExEvent>
        , IHandle<DeleteModuleEvent>
        , IHandle<UpdateModuleEnumEvent>
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public FunctionFormViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            eventAggregator.Subscribe(this);

            var UCModulesTreeViewExVM = Globals.GetViewModel<IUCModulesTreeEx>();
            UCModulesTreeViewEx = UCModulesTreeViewExVM;
            this.ActivateItem(UCModulesTreeViewExVM);

            _allFunction=new PagedSortableCollectionView<Function>();
            allFunction.OnRefresh += new EventHandler<RefreshEventArgs>(allFunction_OnRefresh);
        }

        void allFunction_OnRefresh(object sender, RefreshEventArgs e)
        {
            GetAllFunctionsPaging(SelectedModulesTree.NodeID, allFunction.PageSize, allFunction.PageIndex, "", true);
        }
        protected override void OnActivate()
        {
            base.OnActivate();
            Globals.EventAggregator.Subscribe(this);
            //==== 20161206 CMN Begin: Disable method called in onloaded
            //var UCModulesTreeViewExVM = Globals.GetViewModel<IUCModulesTreeEx>();
            //UCModulesTreeViewEx = UCModulesTreeViewExVM;
            //this.ActivateItem(UCModulesTreeViewExVM);

            ////_allFunction = new PagedSortableCollectionView<Function>();
            ////allFunction.OnRefresh += new EventHandler<RefreshEventArgs>(allFunction_OnRefresh);
            //Globals.EventAggregator.Subscribe(this);
            //==== 20161206 CMN End
        }

        public override void DeactivateItem(object item, bool close)
        {
            base.DeactivateItem(item, close);
            Globals.EventAggregator.Unsubscribe(this);
        }

        public object UCModulesTreeViewEx { get; set; }


        public object UCModulesTreeView { get; set; }

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
                NotifyOfPropertyChange(() => CanbutSave);
            }
        }

        private PagedSortableCollectionView<Function> _allFunction;
        public PagedSortableCollectionView<Function> allFunction
        {
            get
            {
                return _allFunction;
            }
            set
            {
                if (_allFunction == value)
                    return;
                _allFunction = value;
                NotifyOfPropertyChange(() => allFunction);
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
            }
        }

        public string txtModuleName { get; set; }
        public string txtModuleDescription { get; set; }        
        public string txtFunctionName{ get; set; }
        public string txtFunctionDescription { get; set; }
        #endregion

        public void Handle(ModuleTreeChangeExEvent obj)
        {
            if (obj != null)
            {
                SelectedModulesTree = obj.curModulesTree;
                allFunction.PageIndex = 0;
                GetAllFunctionsPaging(SelectedModulesTree.NodeID, allFunction.PageSize, allFunction.PageIndex, "", true);
            }
        }

        public void Handle(DeleteModuleEvent obj)
        {
            if (obj != null)
            {
                DeleteModules(SelectedModulesTree.NodeID);
            }
        }

        public void Handle(UpdateModuleEnumEvent obj)
        {
            if (obj != null)
            {
                //var cwdModuleEnumVM = Globals.GetViewModel<IcwdModuleEnum>();
                //cwdModuleEnumVM.SelectedModule = SelectedModulesTree;
                //var instance = cwdModuleEnumVM as Conductor<object>;
                //this.ActivateItem(cwdModuleEnumVM);
                //Globals.ShowDialog(instance, (o) => { });

                Action<IcwdModuleEnum> onInitDlg = (cwdModuleEnumVM) =>
                {
                    cwdModuleEnumVM.SelectedModule = SelectedModulesTree;
                    var instance = cwdModuleEnumVM as Conductor<object>;
                    this.ActivateItem(cwdModuleEnumVM);
                };
                GlobalsNAV.ShowDialog<IcwdModuleEnum>(onInitDlg);

            }
        }
        public void butSaveFunc()
        {
            AddNewFunctions((int)SelectedModulesTree.NodeID, 0, txtFunctionName, txtFunctionDescription, 0);;
        }

        public void butSaveModule()
        {
            AddNewModules(txtModuleName, 0, txtModuleDescription, 0);
        }
        public bool CanbutSave
        {
            get { return SelectedModulesTree.NodeText != "" ? true : false; }
        }

        /*▼====: #001*/
        private ObservableCollection<ModulesTree> _allModulesTree;

        public ObservableCollection<ModulesTree> allModulesTree
        {
            get { return _allModulesTree; }
            set
            {
                if (_allModulesTree == value)
                    return;
                _allModulesTree = value;
                ((IUCModulesTreeEx)UCModulesTreeViewEx).allModulesTree = _allModulesTree;
                NotifyOfPropertyChange(() => allModulesTree);
            }
        }
        /*▲====: #001*/

        public void lnkDeleteClick(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Bạn có muốn xoá không?", eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                DeleteFunction(SelectedFunction.FunctionID);
            }
        }
        public void butSaveEnum()
        {
            for (int i = 0; i < allFunction.Count; i++)
            {
                UpdateFunctionsEnum(allFunction[i].FunctionID
                    , allFunction[i].eNum);
            }
        }
        public void lnkUpdateClick(object sender, RoutedEvent e)
        {
            //var cwdFunctionUpdateVM = Globals.GetViewModel<IcwdFunctionUpdate>();
            //cwdFunctionUpdateVM.SelectedFunction= SelectedFunction;
            //var instance = cwdFunctionUpdateVM as Conductor<object>;
            //this.ActivateItem(cwdFunctionUpdateVM);
            //Globals.ShowDialog(instance, (o) => { });

            Action<IcwdFunctionUpdate> onInitDlg = (cwdFunctionUpdateVM) =>
            {
                cwdFunctionUpdateVM.SelectedFunction = SelectedFunction;
                var instance = cwdFunctionUpdateVM as Conductor<object>;
                this.ActivateItem(cwdFunctionUpdateVM);
            };
            GlobalsNAV.ShowDialog<IcwdFunctionUpdate>(onInitDlg);

        }
        #region method
        /*▼====: #001*/
        private void GetAllFunctionsPaging(long ModuleID
            , int PageSize, int PageIndex, string OrderBy, bool CountTotal)
        {
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new UserAccountsServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetAllFunctionsPaging(ModuleID, PageSize
                            , PageIndex, OrderBy, CountTotal, Globals.DispatchCallback((asyncResult) =>
                            {
                                try
                                {
                                    var results = contract.EndGetAllFunctionsPaging(out int Total, asyncResult);
                                    if (results != null)
                                    {
                                        if (allFunction == null)
                                        {
                                            allFunction = new PagedSortableCollectionView<Function>();
                                        }
                                        else
                                        {
                                            allFunction.Clear();
                                        }
                                        foreach (var p in results)
                                        {
                                            allFunction.Add(p);
                                        }
                                        allFunction.TotalItemCount = Total;
                                        NotifyOfPropertyChange(() => allFunction);
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

        private void AddNewFunctions(int ModuleID, int eNum, string FunctionName, string FunctionDescription, int Idx)
        {
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new UserAccountsServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginAddNewFunctions(ModuleID, eNum, FunctionName, FunctionDescription, Idx, Globals.DispatchCallback((asyncResult) =>
                         {
                             try
                             {
                                 var results = contract.EndAddNewFunctions(asyncResult);
                                 if (results == true)
                                 {
                                     txtFunctionName = "";
                                     txtFunctionDescription = "";
                                     NotifyOfPropertyChange(() => txtFunctionName);
                                     NotifyOfPropertyChange(() => txtFunctionDescription);
                                     allFunction.PageIndex = 0;
                                     GetAllFunctionsPaging(SelectedModulesTree.NodeID, allFunction.PageSize, allFunction.PageIndex, "", true);
                                     Globals.ShowMessage(eHCMSResources.Z1749_G1_ThemMoiFunction, "");
                                     Globals.EventAggregator.Publish(new ModuleTreeExChangeEvent { curModulesTree = SelectedModulesTree });
                                     //Globals.EventAggregator.Publish(new allFunctionChangeEvent() { });
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

        private void DeleteFunction(long FunctionID)
        {
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new UserAccountsServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginDeleteFunctions(FunctionID, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndDeleteFunctions(asyncResult);
                                if (results == true)
                                {
                                    allFunction.PageIndex = 0;
                                    GetAllFunctionsPaging(SelectedModulesTree.NodeID, allFunction.PageSize, allFunction.PageIndex, "", true);
                                    Globals.ShowMessage(eHCMSResources.Z1750_G1_XoaFunctionThCong, "");
                                    Globals.EventAggregator.Publish(new ModuleTreeExChangeEvent { curModulesTree = SelectedModulesTree });
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

        private void UpdateFunctionsEnum(long FunctionID, int eNum)
        {
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new UserAccountsServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginUpdateFunctionsEnum(FunctionID, eNum, Globals.DispatchCallback((asyncResult) =>
                      {
                          try
                          {
                              var results = contract.EndUpdateFunctionsEnum(asyncResult);
                              if (results == true)
                              {
                                  allFunction.PageIndex = 0;
                                  GetAllFunctionsPaging(SelectedModulesTree.NodeID, allFunction.PageSize, allFunction.PageIndex, "", true);
                                  Globals.ShowMessage("Update Function Enum thành công!", "");
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

        private void AddNewModules(string ModuleName, int eNum, string Description, int Idx)
        {
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new UserAccountsServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginAddNewModules(ModuleName, eNum, Description, Idx, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndAddNewModules(asyncResult);
                                if (results == true)
                                {
                                    txtModuleName = "";
                                    txtModuleDescription = "";
                                    NotifyOfPropertyChange(() => txtModuleName);
                                    NotifyOfPropertyChange(() => txtModuleDescription);
                                    allFunction.PageIndex = 0;
                                    Globals.ShowMessage(eHCMSResources.Z1751_G1_ThemMoiModuleThCong, "");
                                    Globals.EventAggregator.Publish(new ModuleTreeExChangeEvent { curModulesTree = SelectedModulesTree });
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

        private void DeleteModules(long ModuleID)
        {
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new UserAccountsServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginDeleteModules(ModuleID, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndDeleteModules(asyncResult);
                                if (results == true)
                                {
                                    Globals.ShowMessage(eHCMSResources.Z1752_G1_XoaModuleThCong, "");
                                    Globals.EventAggregator.Publish(new ModuleTreeExChangeEvent { curModulesTree = SelectedModulesTree });
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

        private void UpdateModulesEnum(long ModuleID, int eNum)
        {
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
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
                                  Globals.ShowMessage(eHCMSResources.K0252_G1_CNhatModuleEnumOk, "");
                                  Globals.EventAggregator.Publish(new ModuleTreeExChangeEvent { curModulesTree = SelectedModulesTree });
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

