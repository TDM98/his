using eHCMSLanguage;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Windows;
using Caliburn.Micro;
using System.Threading;
using DataEntities;
using aEMR.ViewContracts;
using aEMR.Infrastructure.Events;
using Castle.Windsor;
using aEMR.Infrastructure.CachingUtils;
using aEMR.Infrastructure;
using aEMR.ServiceClient;
/*
* 20170407 #001 CMN: Enable get Department Only
*/
namespace aEMR.Common.ViewModels
{
    [Export(typeof(IRoomTree)),PartCreationPolicy(CreationPolicy.NonShared)]
    public class RoomTreeViewModel : Conductor<object>, IRoomTree, IHandle<AddNewRoomTargetEvent>
    {
        //==== #001
        private bool _DeptOnly = false;
        public bool DeptOnly
        {
            get { return _DeptOnly; }
            set
            {
                if (_DeptOnly != value)
                {
                    _DeptOnly = value;
                    NotifyOfPropertyChange(() => DeptOnly);
                }
            }
        }
        //==== #001

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

        [ImportingConstructor]
        public RoomTreeViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg, ISalePosCaching salePosCaching)
        {
            eventArg.Subscribe(this);
            allRefDepartmentsTree = new ObservableCollection<RefDepartmentsTree>();
            GetDeptLocTreeViewSegment(((long)VRoomType.Khoa).ToString(), true, (long)RoomFunction.KHAM_BENH);
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

        private RefDepartmentsTree _defaultDepartment;
        public RefDepartmentsTree DefaultDepartment
        {
            get { return _defaultDepartment; }
            set
            {
                if (_defaultDepartment != value)
                {
                    _defaultDepartment = value;
                    NotifyOfPropertyChange(() => DefaultDepartment);
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
                DefaultDepartment = CurRefDepartmentsTree;
                Globals.EventAggregator.Publish(new RoomSelectedEvent() { curDeptLoc = CurRefDepartmentsTree });
            }

        }

        private RefDepartmentsTree _CurDepartment;
        public RefDepartmentsTree CurDepartment
        {
            get
            {
                return _CurDepartment;
            }
            set
            {
                if (_CurDepartment == value)
                    return;
                _CurDepartment = value;
                NotifyOfPropertyChange(() => CurDepartment);
                Globals.EventAggregator.Publish(new DepartmentSelectedEvent() { curDepartment = CurDepartment });
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
                if (((RefDepartmentsTree)e.NewValue) != null)
                {
                    CurDepartment = (RefDepartmentsTree)e.NewValue;
                }
                if (((RefDepartmentsTree)e.NewValue).IsDeptLocation == true)
                {
                    CurRefDepartmentsTree = (RefDepartmentsTree)e.NewValue;
                }
                //==== #001
                else if (((RefDepartmentsTree)e.NewValue) != null && DeptOnly && ((RefDepartmentsTree)e.NewValue).Parent.NodeID > 0)
                {
                    CurRefDepartmentsTree = new RefDepartmentsTree();
                    CurRefDepartmentsTree.Parent = CurDepartment;
                    Globals.EventAggregator.Publish(new RoomSelectedEvent() { curDeptLoc = CurRefDepartmentsTree });
                }
                //==== #001
            }
        }

        private void GetDeptLocTreeViewSegment(string strV_DeptType, bool ShowDeptLocation,long RoomFunction)
        {
            var t = new Thread(() =>
            {
                IsLoading=true;

                using (var serviceFactory = new ConfigurationManagerServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginGetRefDepartments_TreeSegment(strV_DeptType, ShowDeptLocation,RoomFunction
                        , Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var items = contract.EndGetRefDepartments_TreeSegment(asyncResult);
                            if (items != null)
                            {
                                allRefDepartmentsTree=new ObservableCollection<RefDepartmentsTree>();
                                foreach (var DepartmentsTree in items)
                                {
                                    allRefDepartmentsTree.Add(DepartmentsTree);
                                }
                                allRefDepartmentsTree.ConvertaEMRImgIcon();
                                NotifyOfPropertyChange(() => allRefDepartmentsTree);
                                InitTree();
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
        private void InitTree()
        {
            if (DefaultDepartment != null && allRefDepartmentsTree != null && allRefDepartmentsTree.Count > 0)
            {
                //RefDepartmentsTree defaultNode = FindCorrespondingNode(DefaultDepartment, allRefDepartmentsTree[0]);
                RefDepartmentsTree defaultNode;
                FindCorrespondingNode(DefaultDepartment, allRefDepartmentsTree[0], out defaultNode);
                if (defaultNode != null)
                {
                    defaultNode.IsExpanded = true;
                    RefDepartmentsTree temp = defaultNode;
                    while (temp != null)
                    {
                        temp.IsExpanded = true;
                        temp = temp.Parent;
                    }
                    SelectTreeItemByDataItem(defaultNode);

                }
            }
        }
        private void FindCorrespondingNode(RefDepartmentsTree department, RefDepartmentsTree tree, out RefDepartmentsTree matchItem)
        {
            matchItem = null;
            if (!tree.IsDeptLocation)
            {
                {
                    if (tree.Children != null)
                    {
                        foreach (var item in tree.Children)
                        {
                            FindCorrespondingNode(department, item, out matchItem);
                            if (matchItem != null)
                            {
                                return;
                            }
                        }
                    }
                }
            }else
            {
                if (department.NodeID == tree.NodeID)
                {
                    matchItem = tree;
                    return;
                }
            }
            
        }
        IDeptTreeView _currentView;
        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            _currentView = view as IDeptTreeView;
        }
        private void SelectTreeItemByDataItem(RefDepartmentsTree node)
        {

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
        public void Handle(AddNewRoomTargetEvent obj)
        {
            GetDeptLocTreeViewSegment(((long)VRoomType.Khoa).ToString(), true, (long)RoomFunction.KHAM_BENH);
        }
    }
}