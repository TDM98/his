using eHCMSLanguage;
using System;
using System.ComponentModel.Composition;
using System.Threading;
using System.Windows;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using Caliburn.Micro;
using DataEntities;
using aEMR.Common.Collections;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using System.Collections.ObjectModel;

/*
 * #001 20180921 TNHX: Apply BusyIndicator, add allModulesTree for Parent's set then it set for child, refactor code
 */
namespace aEMR.UserAccountManagement.ViewModels
{
    [Export(typeof(IOperationForm)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class OperationFormViewModel : Conductor<object>, IOperationForm
        , IHandle<FunctionChangeEvent>
        , IHandle<ModuleChangeEvent>
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public OperationFormViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            eventAggregator.Subscribe(this);
            var UCModulesTreeVM = Globals.GetViewModel<IUCModulesTree>();
            UCModulesTreeView = UCModulesTreeVM;
            this.ActivateItem(UCModulesTreeVM);

            _allOperation=new PagedSortableCollectionView<Operation>();
            _SelectedModulesTree = new ModulesTree();
            allOperation.OnRefresh += new EventHandler<RefreshEventArgs>(allOperation_OnRefresh);
            //Globals.EventAggregator.Subscribe(this);
        }

        void allOperation_OnRefresh(object sender, RefreshEventArgs e)
        {
            GetAllOperationsByFuncIDPaging(SelectedFunction.FunctionID,allOperation.PageSize
                , allOperation.PageIndex,"",true);
        }
        protected override void OnActivate()
        {
            base.OnActivate();
            //==== 20161206 CMN Begin: Disable method called in onloaded
            //var UCModulesTreeVM = Globals.GetViewModel<IUCModulesTree>();
            //IsLoading5 = true;
            //UCModulesTreeView = UCModulesTreeVM;
            //this.ActivateItem(UCModulesTreeVM);
            //==== 20161206 CMN End.
            //_SelectedModulesTree=new ModulesTree();
            //_allOperation = new PagedSortableCollectionView<Operation>();
            //allOperation.OnRefresh += new EventHandler<RefreshEventArgs>(allOperation_OnRefresh);
            
            Globals.EventAggregator.Subscribe(this);
        }
        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);
            Globals.EventAggregator.Unsubscribe(this);
        }

        public object UCModulesTreeView { get; set; }
#region properties

        private ModulesTree _SelectedModulesTree ;
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
                NotifyOfPropertyChange(()=>CanbutSave);
            }
        }

        private PagedSortableCollectionView<Operation> _allOperation;
        public PagedSortableCollectionView<Operation> allOperation
        {
            get
            {
                return _allOperation;
            }
            set
            {
                if (_allOperation == value)
                    return;
                _allOperation = value;
                NotifyOfPropertyChange(() => allOperation);
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
                if (SelectedFunction!=null)
                {
                    GetAllOperationsByFuncIDPaging(SelectedFunction.FunctionID, allOperation.PageSize
                            , allOperation.PageIndex, "", true);
                }
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

        public string txtOperationName { get; set; }
        public string txtDescription { get; set; }
        #endregion
        public void Handle(FunctionChangeEvent obj)
        {
            if(obj!=null)
            {
                SelectedFunction = obj.curFunction;
                NotifyOfPropertyChange(() => SelectedFunction);
                NotifyOfPropertyChange(()=>CanbutSave);
            }
        }
        public void Handle(ModuleChangeEvent obj)
        {
            if (obj != null)
            {
                SelectedModulesTree= obj.curModuleTree;
                SelectedFunction = null;
                NotifyOfPropertyChange(() => CanbutSave);
                
            }
        }
        public void Handle(OperationChangeEvent obj)
        {
            if (obj != null)
            {
                SelectedFunction = null;
                NotifyOfPropertyChange(() => CanbutSave);
            }
        }
        
        public void butSave()
        {
            if (txtOperationName=="")
            {
                MessageBox.Show(eHCMSResources.A0423_G1_Msg_InfoChuaNhapTenOperation);
                return;
            }
            if (SelectedFunction.FunctionID==0)
            {
                MessageBox.Show("Chưa chọn Function!");
                return;
            }
            AddNewOperations(SelectedFunction.FunctionID, txtOperationName, txtDescription);
        }

        public bool CanbutSave
        {
            get { return SelectedFunction != null ? true : false; }
        }        

        public void lnkDeleteClick(object sender,RoutedEventArgs e)
        {
            if (MessageBox.Show("Bạn có muốn xoá không?", eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                DeleteOperations(SelectedOperation.OperationID);
            }
        }
        public void butSaveEnum()
        {
            for (int i = 0; i < allOperation.Count; i++)
            {
                UpdateOperations(allOperation[i].OperationID
                    , allOperation[i].Enum
                    , allOperation[i].OperationName
                    , allOperation[i].Description);
            }
        }
        public void lnkUpdateClick(object sender, RoutedEvent e)
        {
            //var cwdOperationUpdate = Globals.GetViewModel<IcwdOperationUpdate>();
            //cwdOperationUpdate.SelectedOperation = SelectedOperation;
            //var instance = cwdOperationUpdate as Conductor<object>;
            //this.ActivateItem(cwdOperationUpdate);
            //Globals.ShowDialog(instance, (o) => { });

            Action<IcwdOperationUpdate> onInitDlg = (cwdOperationUpdate) =>
            {
                cwdOperationUpdate.SelectedOperation = SelectedOperation;
                this.ActivateItem(cwdOperationUpdate);
            };
            GlobalsNAV.ShowDialog<IcwdOperationUpdate>(onInitDlg);
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
                ((IUCModulesTree)UCModulesTreeView).allModulesTree = _allModulesTree;
                NotifyOfPropertyChange(() => allModulesTree);
            }
        }

        #region method
        private void GetAllOperationsByFuncIDPaging(long FunctionID
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
                        contract.BeginGetAllOperationsByFuncIDPaging(FunctionID, PageSize
                            , PageIndex, OrderBy, CountTotal, Globals.DispatchCallback((asyncResult) =>
                         {
                            try
                            {
                                int Total = 0;
                                var results = contract.EndGetAllOperationsByFuncIDPaging(out Total, asyncResult);
                                if (results != null)
                                {
                                    if (allOperation == null)
                                    {
                                        allOperation = new PagedSortableCollectionView<Operation>();
                                    }
                                    else
                                    {
                                        allOperation.Clear();
                                    }
                                    foreach (var p in results)
                                    {
                                        allOperation.Add(p);
                                    }
                                    allOperation.TotalItemCount = Total;
                                    NotifyOfPropertyChange(() => allOperation);
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

        //ModulesTreeVM.AddNewOperations(ModulesTreeVM.SelectedModulesTree.NodeID, stName, stDescript);
        private void AddNewOperations(long FunctionID, string OperationName, string Description)
        {
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new UserAccountsServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginAddNewOperations(FunctionID, OperationName, Description, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndAddNewOperations(asyncResult);
                                if (results == true)
                                {
                                    allOperation.PageIndex = 0;
                                    GetAllOperationsByFuncIDPaging(SelectedFunction.FunctionID, allOperation.PageSize
                                                    , allOperation.PageIndex, "", true);
                                    txtDescription = "";
                                    txtOperationName = "";
                                    NotifyOfPropertyChange(() => txtDescription);
                                    NotifyOfPropertyChange(() => txtOperationName);
                                    Globals.EventAggregator.Publish(new ModuleTreeOperationChangeEvent { });
                                    Globals.ShowMessage(eHCMSResources.Z1753_G1_ThemMoiOperationThCong, "");
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

        private void DeleteOperations(long OperationID)
        {
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new UserAccountsServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginDeleteOperations(OperationID, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndDeleteOperations(asyncResult);
                                if (results == true)
                                {
                                    allOperation.PageIndex = 0;
                                    GetAllOperationsByFuncIDPaging(SelectedFunction.FunctionID, allOperation.PageSize
                                                    , allOperation.PageIndex, "", true);
                                    txtDescription = "";
                                    txtOperationName = "";
                                    Globals.EventAggregator.Publish(new ModuleTreeOperationChangeEvent { curModulesTree = SelectedModulesTree });
                                    Globals.ShowMessage(eHCMSResources.Z1754_G1_XoaOperationThCong, "");
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

        private void UpdateOperations(long OperationID, int Enum, string OperationName, string Description)
        {
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
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
                                    allOperation.PageIndex = 0;
                                    GetAllOperationsByFuncIDPaging(SelectedFunction.FunctionID, allOperation.PageSize
                                                    , allOperation.PageIndex, "", true);
                                    txtDescription = "";
                                    txtOperationName = "";
                                    Globals.EventAggregator.Publish(new ModuleTreeOperationChangeEvent { curModulesTree = SelectedModulesTree });
                                    Globals.ShowMessage(eHCMSResources.Z1755_G1_UpdateOperationEnum, "");
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
        #endregion
        /*▲====: #001*/
    }
}
