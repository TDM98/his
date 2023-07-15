using eHCMSLanguage;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Net;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
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
 * #001 20180921 TNHX: Apply BusyIndicator, refactor code
*/
namespace aEMR.UserAccountManagement.ViewModels
{
    //[Export(typeof (IUCModulesTreeEx)),PartCreationPolicy(CreationPolicy.NonShared)]
    [Export(typeof(IUCModulesTreeEx))]
    public class UCModulesTreeExViewModel : Conductor<object>, IUCModulesTreeEx
        , IHandle<ModuleTreeExChangeEvent>
        , IHandle<ModuleTreeChangeEvent>
    {
        public UCModulesTreeExViewModel()
        {
            //_allModulesTree = new ObservableCollection<ModulesTree>();
            _allModulesTree = new ObservableCollection<ModulesTree>();
            //GetModulesTreeView();
            Globals.EventAggregator.Subscribe(this);
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            Globals.EventAggregator.Subscribe(this);
            //_allModulesTree = new ObservableCollection<ModulesTree>();
            //GetModulesTreeView();
        }
       
        #region property

        private ObservableCollection<ModulesTree> _allModulesTree;

        public ObservableCollection<ModulesTree> allModulesTree
        {
            get { return _allModulesTree; }
            set
            {
                if (_allModulesTree == value)
                    return;
                _allModulesTree = value;
                NotifyOfPropertyChange(() => allModulesTree);
            }
        }

        private ModulesTree _SelectedModulesTree;

        public ModulesTree SelectedModulesTree
        {
            get { return _SelectedModulesTree; }
            set
            {
                if (_SelectedModulesTree == value)
                    return;
                _SelectedModulesTree = value;
                NotifyOfPropertyChange(() => SelectedModulesTree);
                if (SelectedModulesTree != null)
                {
                    if (SelectedModulesTree.Level == 1)
                    {
                        Globals.EventAggregator.Publish(new ModuleTreeChangeExEvent
                                                            {curModulesTree = SelectedModulesTree});
                    }
                }
            }
        }

        private bool _chkMenu;

        public bool chkMenu
        {
            get { return _chkMenu; }
            set
            {
                if (_chkMenu == value)
                    return;
                _chkMenu = value;
                NotifyOfPropertyChange(() => chkMenu);
            }
        }

        #endregion

        public void treeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (e != null
                && e.NewValue != null)
            {
                if ((((ModulesTree) e.NewValue)) != null)
                {
                    if ((((ModulesTree) e.NewValue)).Level == 1)
                    {
                        Globals.EventAggregator.Publish(new ModuleTreeChangeExEvent
                                                            {curModulesTree = (((ModulesTree) e.NewValue))});
                    }
                }
            }
        }

        #region method
        /*▼====: #001*/
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

        public void Handle(ModuleTreeExChangeEvent obj)
        {
            if (obj != null)
            {
                GetModulesTreeView();
            }
        }
        public void Handle(ModuleTreeChangeEvent obj)
        {
            if (obj != null)
            {
                GetModulesTreeView();
            }
        }
        
        #region Popup Menu
        private const string itemUpdateEnum = "  Update Enum  ";
        private const string itemDelete = "  Delete Module  ";
        private const string itemCancel = "  Cancel  ";
        private Popup popup = null;
        public Border brdRightClickZone { get; set; }
        public Grid GrdTree { get; set; }
        public TreeView treeView { get; set; }
        public void GrdTree_Loaded(object sender,RoutedEventArgs e)
        {
            GrdTree = sender as Grid;
        }
        public void brdRightClickZone_Loaded(object sender, RoutedEventArgs e)
        {
            brdRightClickZone = sender as Border;
            brdRightClickZone.MouseRightButtonDown += new MouseButtonEventHandler(btnRightClick_MouseRightButtonDown);
            brdRightClickZone.MouseRightButtonUp += new MouseButtonEventHandler(brdRightClickZone_MouseRightButtonUp);
        }

        public void treeView_Loaded(object sender, RoutedEventArgs e)
        {
            treeView = sender as TreeView;
        }
        

        void brdRightClickZone_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            Point currentMousePosition = e.GetPosition(GrdTree);

            //List<UIElement> allItems = VisualTreeHelper.FindElementsInHostCoordinates(currentMousePosition, treeView).ToList();

            //Matrix matrix = ((MatrixTransform)GrdTree.TransformToVisual(Application.Current.RootVisual)).Matrix;


            //UIElement curItem = VisualTreeHelper.FindElementsInHostCoordinates(
            //    new Point(matrix.OffsetX + currentMousePosition.X, matrix.OffsetY + currentMousePosition.Y), treeView)
            //    .Where((obj) => { return obj is TreeViewItem; }).FirstOrDefault();
            //if (curItem != null)
            //{
            //    TreeViewItem item = (TreeViewItem)curItem;
            //    item.Focus();
            //    item.IsSelected = true;

            //    string[] contextMenuItemsText = CreateItemsMenuContextByLevel(item);

            //    ShowPopup(currentMousePosition, contextMenuItemsText);
            //}
        }
        private void btnRightClick_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }
        private string[] CreateItemsMenuContextByLevel(TreeViewItem item)
        {
            int? iLevel = ((ModulesTree)treeView.SelectedItem).Level;
            string[] contextMenuItemsText;
            if (iLevel == 1)
            {
                contextMenuItemsText = new string[] { itemUpdateEnum, itemDelete, itemCancel };
            }
            else
            {
                contextMenuItemsText = new string[] { itemCancel };
            }
            return contextMenuItemsText;
        }

        private void ShowPopup(Point currentMousePosition, string[] contextMenuItemsText)
        {
            double scrollHeight = 0.0;
            double scrollWidth = 0.0;

            //IEnumerable<DependencyObject> allParents = VisualTreeExtensions.GetVisualAncestorsAndSelf(treeView);
            //IEnumerable<DependencyObject> allScrollers = allParents.Where((obj) => { return obj.GetType() == typeof(ScrollViewer); });
            //foreach (DependencyObject obj in allScrollers)
            //{
            //    scrollHeight += ((ScrollViewer)obj).VerticalOffset;
            //    scrollWidth += ((ScrollViewer)obj).HorizontalOffset;
            //}

            //if (popup != null)
            //{
            //    popup.IsOpen = false;
            //    popup = null;
            //}
            currentMousePosition.Y -= scrollHeight;
            currentMousePosition.X -= scrollWidth;
            popup = CreateContextMenu(currentMousePosition, contextMenuItemsText);
            popup.IsOpen = true;
        }

        private void HidePopup()
        {
            popup.IsOpen = false;
        }

        private Popup CreateContextMenu(Point currentMousePosition, string[] contextMenuItemsText)
        {
            Popup popup = new Popup();
            Grid popupGrid = new Grid();
            Canvas popupCanvas = new Canvas();

            popup.Child = popupGrid;
            popupCanvas.MouseLeftButtonDown += (sender, e) => { HidePopup(); };
            popupCanvas.MouseRightButtonDown += (sender, e) => { e.Handled = true; HidePopup(); };
            popupCanvas.Background = new SolidColorBrush(Colors.Transparent);
            popupGrid.Children.Add(popupCanvas);
            popupGrid.Children.Add(CreateContextMenuItems(currentMousePosition, contextMenuItemsText));

            //popupGrid.Width = Application.Current.Host.Content.ActualWidth;
            //popupGrid.Height = Application.Current.Host.Content.ActualHeight;
            popupCanvas.Width = popupGrid.Width;
            popupCanvas.Height = popupGrid.Height;

            return popup;
        }

        protected void txb_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            string s = (sender as TextBlock).Text.ToString();
            switch (s)
            {
                case itemUpdateEnum:
                    {
                        //var cw = new eHCMSApp.Views.ConfigurationManager.UserAccount.ChildWindows.cwdModuleEnum();
                        //cw.DataContext = ModulesTreeVM;
                        //cw.Show();
                        Globals.EventAggregator.Publish(new UpdateModuleEnumEvent{});
                        break;
                    }
                case itemDelete:
                    {
                        Globals.EventAggregator.Publish(new DeleteModuleEvent{ });
                        break;
                    }
                case itemCancel:
                    {

                        break;
                    }
            }
            HidePopup();
        }
        private FrameworkElement CreateContextMenuItems(Point currentMousePosition, string[] contextMenuItemsText)
        {
            //string[] contextMenuItemsText = { itemThem,itemHieuChinh,itemXoa };

            ListBox lstContextMenu = new ListBox();
            foreach (string str in contextMenuItemsText)
            {
                TextBlock txb = new TextBlock() { Text = str };
                txb.MouseLeftButtonUp += new MouseButtonEventHandler(txb_MouseLeftButtonUp);
                lstContextMenu.Items.Add(txb);
            }

            Grid rootGrid = new Grid()
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                //Margin = new Thickness(currentMousePosition.X + 270, currentMousePosition.Y + 180, 0, 0)
                Margin = new Thickness(currentMousePosition.X +200, currentMousePosition.Y+ 150 , 0, 0)
            };
            rootGrid.Children.Add(lstContextMenu);

            return rootGrid;
        }
        #endregion
    }
}
