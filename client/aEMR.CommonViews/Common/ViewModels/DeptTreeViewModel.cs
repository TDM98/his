using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using aEMR.ViewContracts;
using aEMR.ViewContracts.Configuration;
using aEMR.Infrastructure;
using aEMR.ServiceClient;
using aEMR.Infrastructure.Events;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using Caliburn.Micro;
using System.Threading;
using DataEntities;
using System.Linq;
using eHCMSLanguage;

namespace aEMR.Common.ViewModels
{
    [Export(typeof(IDeptTreeCommon)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class DeptTreeViewModel : Conductor<object>, IDeptTreeCommon
    {

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
            //GetDeptLocTreeViewBang("7000", true, DefaultDepartment.DeptID);
        }

        public void GetDeptLocTreeAll()
        {
            GetDeptLocTreeViewBang("7000", true);
        }
        public void GetDeptLocTreeDeptID()
        {
            GetDeptLocTreeViewBang("7000", true, DefaultDepartment.DeptID, DefaultDepartment.DeptLocations[0].DeptLocationID);
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

        //private ObservableCollection<long> _LstRefDepartment;
        //public ObservableCollection<long> LstRefDepartment
        //{
        //    get { return _LstRefDepartment; }
        //    set
        //    {
        //        if (_LstRefDepartment != value)
        //        {
        //            _LstRefDepartment = value;
        //            NotifyOfPropertyChange(() => LstRefDepartment);
        //        }
        //    }
        //}

        private RefDepartment _defaultDepartment;
        public RefDepartment DefaultDepartment
        {
            get { return _defaultDepartment; }
            set
            {
                if (_defaultDepartment != value)
                {
                    _defaultDepartment = value;
                    NotifyOfPropertyChange(() => DefaultDepartment);
                    InitTree();
                }
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
                RefDepartmentsTree curItem = null;
                if(e.NewValue is TreeViewItem)
                {
                    curItem = ((TreeViewItem)e.NewValue).DataContext as RefDepartmentsTree;
                }
                else
                {
                    curItem = (RefDepartmentsTree)e.NewValue;
                }
                if (curItem != null && curItem.IsDeptLocation == true)
                {
                    CurRefDepartmentsTree = (RefDepartmentsTree)e.NewValue;
                    
                    Globals.EventAggregator.Publish(new DeptLocBedSelectedEvent { curDeptLoc = CurRefDepartmentsTree });
                }
            }
        }

        private void GetDeptLocTreeViewBang(string strV_DeptType, bool ShowDeptLocation)
        {
            // su dung ham nay ne RefDepartments_Tree(strV_DeptType, ShowDeptLocation);
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z1026_G1_DSRoomType });

            var t = new Thread(() =>
            {
                using (var serviceFactory = new ConfigurationManagerServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginRefDepartments_Tree(strV_DeptType, ShowDeptLocation, null, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var items = contract.EndRefDepartments_Tree(asyncResult);
                            if (items == null)
                            {
                                return;
                            }

                            allRefDepartmentsTree.Clear();
                            foreach (var DepartmentsTree in items)
                            {
                                if (Globals.isAccountCheck && DepartmentsTree.V_DeptType == (long)AllLookupValues.V_DeptType.Khoa && DepartmentsTree.Children != null)
                                {
                                    //KMx: Chỉ lấy các khoa mà nhân viên được cấu hình trách nhiệm
                                    DepartmentsTree.Children = DepartmentsTree.Children.Where(x => Globals.LoggedUserAccount.DeptIDResponsibilityList.Any(y => y == x.NodeID)).ToList();
                                }
                                allRefDepartmentsTree.Add(DepartmentsTree);
                            }

                            InitTree();
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
        private void GetDeptLocTreeViewBang(string strV_DeptType, bool ShowDeptLocation, long DeptID, long DeptLocationID)
        {
            // su dung ham nay ne RefDepartments_Tree(strV_DeptType, ShowDeptLocation);
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z1026_G1_DSRoomType });

            var t = new Thread(() =>
            {
                using (var serviceFactory = new ConfigurationManagerServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginRefDepartments_Tree_ByDeptID(strV_DeptType, ShowDeptLocation, DeptID, DeptLocationID,Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var items = contract.EndRefDepartments_Tree_ByDeptID(asyncResult);
                            if (items != null)
                            {
                                allRefDepartmentsTree.Clear();
                                foreach (var DepartmentsTree in items)
                                {
                                    allRefDepartmentsTree.Add(DepartmentsTree);
                                }

                            }
                            InitTree();
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

        private void InitTree()
        {
            if (DefaultDepartment != null && allRefDepartmentsTree != null && allRefDepartmentsTree.Count > 0)
            {
                //RefDepartmentsTree defaultNode = FindCorrespondingNode(DefaultDepartment, allRefDepartmentsTree[0]);
                RefDepartmentsTree defaultNode;
                FindCorrespondingNode(DefaultDepartment, allRefDepartmentsTree[0], out defaultNode);
                if(defaultNode != null)
                {
                    defaultNode.IsExpanded = true;
                    RefDepartmentsTree temp = defaultNode.Parent;
                    while (temp != null)
                    {
                        temp.IsExpanded = true;
                        temp = temp.Parent;
                    }
                    SelectTreeItemByDataItem(defaultNode);

                }
            }
        }
        private void FindCorrespondingNode(RefDepartment department, RefDepartmentsTree tree, out RefDepartmentsTree matchItem)
        {
            matchItem = null;
            if (!tree.IsDeptLocation)
            {

                if (department.DeptID == tree.NodeID)
                {
                    matchItem = tree;
                    return;
                }
                else
                {
                    if(tree.Children != null)
                    {
                        foreach (var item in tree.Children)
                        {
                            FindCorrespondingNode(department, item, out matchItem);
                            if(matchItem != null)
                            {
                                return;
                            }
                        }
                    }
                }
            }
            //else
            //{
            //    return FindCorrespondingNode(department, tree);
            //}
           

            //if (!tree.IsDeptLocation && department.DeptID == tree.NodeID)
            //{
            //    return tree;
            //}

            //if (tree.Children != null)
            //{
            //    foreach (var item in tree.Children)
            //    {
            //        if(!item.IsDeptLocation)
            //        {
            //            return FindCorrespondingNode(department, item);
            //        }
                    
            //    } 
            //}
            //return null;
        }
        IDeptTreeView _currentView;
        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            _currentView = view as IDeptTreeView;
        }
        private void SelectTreeItemByDataItem(RefDepartmentsTree node)
        {
            //if(_currentView != null)
            //{
            //    _currentView.SelectTreeItemByDataItem(node);
            //}


            var t = new Thread(() =>
            {

                System.Threading.Thread.Sleep(500);
                System.Windows.Threading.Dispatcher dispatcher = System.Windows.Application.Current.Dispatcher;
                dispatcher.Invoke(() => 
                {
                    node.IsSelected = true;


                    if (_currentView != null)
                    {
                        _currentView.SelectTreeItemByDataItem(node);
                    }

                });
            });
            t.Start();

        }
        public  void treeView_Loaded(TreeView source,object eventArgs)
        {
            
        }
        
    }
}
