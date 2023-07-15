using eHCMSLanguage;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Windows;
using aEMR.ViewContracts.Configuration;
using aEMR.ViewContracts;
using Caliburn.Micro;
using aEMR.Infrastructure;
using aEMR.ServiceClient;
using aEMR.Infrastructure.Events;
using System.Threading;
using DataEntities;
using Castle.Windsor;
using Castle.Core.Logging;
using aEMR.Infrastructure.CachingUtils;

namespace aEMR.Configuration.BedAllocations.ViewModels
{
    [Export(typeof(IDeptTree)),PartCreationPolicy(CreationPolicy.NonShared)]
    public class DeptTreeViewModel : Conductor<object>, IDeptTree
    {
        private bool _isLoading;
        public bool IsLoading
        {
            get { return _isLoading; }
            set
            {
                if (_isLoading != value)
                {
                    _isLoading = value;
                    NotifyOfPropertyChange(() => IsLoading);
                }
            }
        }

        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public DeptTreeViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;
            //GetDeptLocTreeViewBang("7000", true);
            _allRefDepartmentsTree =new ObservableCollection<RefDepartmentsTree>();
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            GetDeptLocTreeViewBang("7000", true);
        }

        private object _ActiveContent;
        public object ActiveContent
        {
            get
            {
                return _ActiveContent;
            }
            set
            {
                _ActiveContent = value;
                NotifyOfPropertyChange(()=>ActiveContent);
            }
        }

        private RefDepartmentsTree _CurRefDepartmentsTree;
        public RefDepartmentsTree CurRefDepartmentsTree
        {
            get { return _CurRefDepartmentsTree; }
            set
            {
                _CurRefDepartmentsTree = value;
                NotifyOfPropertyChange(() => CurRefDepartmentsTree);
                Globals.EventAggregator.Publish(new DeptLocSelectedEvent() { curDeptLoc = CurRefDepartmentsTree });
            }

        }
        private ObservableCollection<RefDepartmentsTree> _allRefDepartmentsTree;
        public ObservableCollection<RefDepartmentsTree> allRefDepartmentsTree
        {
            get
            {
                return _allRefDepartmentsTree;
            }
            set
            {
                if (_allRefDepartmentsTree == value)
                    return;
                _allRefDepartmentsTree = value;
                NotifyOfPropertyChange(() => allRefDepartmentsTree);
            }
        }
        public void treeView_SelectedItemChanged(RoutedPropertyChangedEventArgs<object> e)
        {
            if(e!=null
                && e.NewValue != null)
            {
                if (((RefDepartmentsTree)e.NewValue).IsDeptLocation==true)
                {
                    CurRefDepartmentsTree = (RefDepartmentsTree)e.NewValue;
                    //if (CurRefDepartmentsTree.IsDeptLocation == true)
                    //{
                    //    GetAllRoomPricesByDeptID(CurRefDepartmentsTree.NodeID);
                    //    GetAllBedAllocByDeptID(CurRefDepartmentsTree.NodeID, 1);
                    //    lstBedNumberEX.Clear();
                    //}
                    
                    Globals.EventAggregator.Publish(new DeptLocBedSelectedEvent { curDeptLoc = CurRefDepartmentsTree });
                }
            }
        }

        private void GetDeptLocTreeViewBang(string strV_DeptType, bool ShowDeptLocation)
        {
            // su dung ham nay ne RefDepartments_Tree(strV_DeptType, ShowDeptLocation);
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "Danh Sách RoomType" });

            var t = new Thread(() =>
            {
                IsLoading=true;

                using (var serviceFactory = new ConfigurationManagerServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginRefDepartments_Tree(strV_DeptType, ShowDeptLocation, null, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var items = contract.EndRefDepartments_Tree(asyncResult);
                            if (items != null)
                            {
                                allRefDepartmentsTree.Clear();
                                foreach (var DepartmentsTree in items)
                                {
                                    allRefDepartmentsTree.Add(DepartmentsTree);
                                }
                                
                            }
                            
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            //Globals.IsBusy = false;
                            IsLoading = false;
                        }
                    }), null);
                }


            });
            t.Start();
        }
    }
}
