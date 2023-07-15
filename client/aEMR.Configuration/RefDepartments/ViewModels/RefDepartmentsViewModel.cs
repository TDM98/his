using eHCMSLanguage;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using aEMR.ViewContracts.Configuration;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using Caliburn.Micro;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using System.Threading;
using System;
using DataEntities;
using aEMR.Controls;
/*
 * #001 20180921 TNHX: Apply BusyIndicator, refactor code
 */
namespace aEMR.Configuration.RefDepartments.ViewModels
{
    [Export(typeof(IRefDepartments)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class RefDepartmentsViewModel : Conductor<object>, IRefDepartments
        , IHandle<RefDepartments_SaveEvent>
        , IHandle<DeptLocation_XMLInsert_Save_Event>
        , IHandle<DeptLocation_MarkDeleted_Event>
        , IHandle<SaveEvent<bool>>
    {
        Popup popup = null;
        const string itemThem = "Thêm            ";
        const string itemHieuChinh = "Hiệu Chỉnh     ";
        const string itemXoa = "Xóa               ";
        const string itemThemPhong = "Thêm Phòng     ";
        const string itemXoaPhong = "Xóa Phòng     ";

        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public RefDepartmentsViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            eventAggregator.Subscribe(this);
            //Globals.EventAggregator.Subscribe(this);

            ObjRefDepartments_Tree = new ObservableCollection<RefDepartmentsTree>();

            RefDepartments_Tree("", true);

            SearchCriteria = new RefDepartmentsSearchCriteria
            {
                DeptName = ""
            };
        }

        public void Root_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            HidePopup();
        }

        Border brdRightClickZone;
        public void brdRightClickZone_Loaded(object sender, RoutedEventArgs e)
        {
            brdRightClickZone = sender as Border;
            brdRightClickZone.MouseRightButtonDown += new MouseButtonEventHandler(btnRightClick_MouseRightButtonDown);
            brdRightClickZone.MouseRightButtonUp += new MouseButtonEventHandler(brdRightClickZone_MouseRightButtonUp);
        }

        private void btnRightClick_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        Grid AreaRL;
        public void AreaRL_Loaded(object sender, RoutedEventArgs e)
        {
            AreaRL = sender as Grid;
        }

        TreeView treeView1;
        public void treeView1_Loaded(object sender, RoutedEventArgs e)
        {
            treeView1 = sender as TreeView;
        }

        private void brdRightClickZone_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            Point currentMousePosition = e.GetPosition(AreaRL);
            FrameworkElement obj = treeView1.InputHitTest(currentMousePosition) as FrameworkElement;
            TreeViewItem item = obj.FindParentOfType<TreeViewItem>();
            if (item != null)
            {
                item.Focus();
                item.IsSelected = true;
                string[] contextMenuItemsText = CreateItemsMenuContextByLevel(item);
                Matrix matrix = ((MatrixTransform)AreaRL.TransformToVisual(Application.Current.MainWindow)).Matrix;
                currentMousePosition = new Point(currentMousePosition.X + matrix.OffsetX, currentMousePosition.Y + matrix.OffsetY);
                ShowPopup(currentMousePosition, contextMenuItemsText);
            }

            //List<UIElement> allItems = VisualTreeHelper.FindElementsInHostCoordinates(currentMousePosition, treeView1).ToList();

            //Matrix matrix = ((MatrixTransform)AreaRL.TransformToVisual(Application.Current.RootVisual)).Matrix;


            //UIElement curItem = VisualTreeHelper.FindElementsInHostCoordinates(
            //    new Point(matrix.OffsetX + currentMousePosition.X, matrix.OffsetY + currentMousePosition.Y), treeView1)
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

        private string[] CreateItemsMenuContextByLevel(TreeViewItem item)
        {
            RefDepartmentsTree Node = (RefDepartmentsTree)(((HeaderedItemsControl)(item)).Header);

            string[] contextMenuItemsText;

            if (Node.ParentID == null)
            {
                contextMenuItemsText = new string[] { itemThem, itemHieuChinh };
            }
            else
            {
                if (Node.IsDeptLocation)//Là Nút Lá
                {
                    contextMenuItemsText = new string[] { itemHieuChinh, itemXoaPhong };
                }
                else//Không Phải Lá
                {
                    if (Node.HasDeptLocation)//Có Phòng rồi,sửa thông tin về nó, không thêm con nó, chỉ thêm phòng
                    {
                        contextMenuItemsText = new string[] { itemHieuChinh, itemThemPhong };
                    }
                    else//Chưa có phòng
                    {
                        if (Node.Children.Count > 0)//nó có con bên trong thì không thêm thêm phòng cho nó
                        {
                            contextMenuItemsText = new string[] { itemThem, itemHieuChinh };
                        }
                        else//Nó là út rồi,Thêm Phòng Nếu Muốn
                        {
                            contextMenuItemsText = new string[] { itemThem, itemHieuChinh, itemXoa, itemThemPhong };
                        }
                    }
                }
            }

            return contextMenuItemsText;
        }

        private void ShowPopup(Point currentMousePosition, string[] contextMenuItemsText)
        {
            double scrollHeight = 0.0;
            double scrollWidth = 0.0;

            //IEnumerable<DependencyObject> allParents = VisualTreeExtensions.GetVisualAncestorsAndSelf(treeView1);
            //IEnumerable<DependencyObject> allScrollers = allParents.Where((obj) => { return obj.GetType() == typeof(ScrollViewer); });
            //foreach (DependencyObject obj in allScrollers)
            //{
            //    scrollHeight += ((ScrollViewer)obj).VerticalOffset;
            //    scrollWidth += ((ScrollViewer)obj).HorizontalOffset;
            //}

            if (popup != null)
            {
                popup.IsOpen = false;
                popup = null;
            }
            currentMousePosition.Y -= scrollHeight;
            currentMousePosition.X -= scrollWidth;
            popup = CreateContextMenu(currentMousePosition, contextMenuItemsText);
            popup.IsOpen = true;
        }

        private Popup CreateContextMenu(Point currentMousePosition, string[] contextMenuItemsText)
        {
            Popup popup = new Popup();
            //Grid popupGrid = new Grid();
            //Canvas popupCanvas = new Canvas();

            popup.Child = CreateContextMenuItems(currentMousePosition, contextMenuItemsText);
            //popupCanvas.MouseLeftButtonDown += (sender, e) => { HidePopup(); };
            //popupCanvas.MouseRightButtonDown += (sender, e) => { e.Handled = true; HidePopup(); };
            //SolidColorBrush mSolidColorBrush = new SolidColorBrush(Colors.Transparent);
            //mSolidColorBrush.Opacity = 1;
            //popupCanvas.Background = mSolidColorBrush;
            //popupGrid.Children.Add(popupCanvas);
            //popupGrid.Children.Add(CreateContextMenuItems(currentMousePosition, contextMenuItemsText));

            //popupGrid.Width = Application.Current.Host.Content.ActualWidth;
            //popupGrid.Height = Application.Current.Host.Content.ActualHeight;
            //popupCanvas.Width = popupGrid.Width;
            //popupCanvas.Height = popupGrid.Height;

            popup.HorizontalOffset = currentMousePosition.X + 20;
            popup.VerticalOffset = currentMousePosition.Y + 10;
            popup.Placement = PlacementMode.Bottom;

            return popup;
        }

        private void HidePopup()
        {
            if (popup != null)
                popup.IsOpen = false;
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
            //Grid rootGrid = new Grid()
            //{
            //    HorizontalAlignment = HorizontalAlignment.Left,
            //    VerticalAlignment = VerticalAlignment.Top,
            //    Margin = new Thickness(currentMousePosition.X + 270, currentMousePosition.Y + 130, 0, 0)
            //};
            //rootGrid.Children.Add(lstContextMenu);
            return lstContextMenu;
        }

        protected void txb_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //txt_search.Text = string.Format("{0}: ", eHCMSResources.G2963_G1_YouClickedOn) + (sender as TextBlock).Text;
            string s = (sender as TextBlock).Text.ToString();
            switch (s)
            {
                case itemXoa:
                    {
                        if (treeView1.SelectedItem != null)
                        {
                            RefDepartmentsTree pTree = (treeView1.SelectedItem as RefDepartmentsTree);

                            if (pTree.Children.Count > 0)
                            {
                                Globals.ShowMessage(string.Format(eHCMSResources.Z1313_G1_CoDLieuKgTheXoa, pTree.NodeText), eHCMSResources.G2617_G1_Xoa);
                            }
                            else
                            {
                                if (MessageBox.Show(string.Format(eHCMSResources.Z0476_G1_BanMuonXoa, pTree.NodeText), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                                {
                                    long DeptID = pTree.NodeID;
                                    DeleteRefDepartments(DeptID);
                                }
                            }
                        }
                        else
                        {
                            MessageBox.Show(eHCMSResources.Z1314_G1_ChonKhoaVPMuonXoa, eHCMSResources.G2617_G1_Xoa, MessageBoxButton.OK);
                        }
                        break;
                    }
                case itemXoaPhong:
                    {
                        if (treeView1.SelectedItem != null)
                        {
                            RefDepartmentsTree pTree = (treeView1.SelectedItem as RefDepartmentsTree);

                            if (pTree.IsDeptLocation)//Là DeptLocation
                            {
                                //Xóa Phòng--> Locations, DeptLocation
                                //MessageBox.Show(eHCMSResources.K0553_G1_XuLyCheckXoaLocations);
                                if (MessageBox.Show(string.Format(eHCMSResources.Z1315_G1_CoMuonXoaPhg0, pTree.NodeText), eHCMSResources.G2617_G1_Xoa, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                                {
                                    DeptLocation_MarkDeleted(pTree.NodeID);
                                }
                            }
                        }
                        else
                        {
                            MessageBox.Show(eHCMSResources.A0354_G1_Msg_InfoChonPgMuonXoa, eHCMSResources.G2617_G1_Xoa, MessageBoxButton.OK);
                        }
                        break;
                    }
                case itemHieuChinh:
                    {
                        if (treeView1.SelectedItem != null)
                        {
                            RefDepartmentsTree pTree = (treeView1.SelectedItem as RefDepartmentsTree);

                            if (pTree.ParentID == null)//Gốc
                            {
                                //var typeInfo = Globals.GetViewModel<IRefDepartments_Info>();
                                //typeInfo.ObjRefDepartments_Current = pTree;

                                //typeInfo.TitleForm = string.Format("{0}: ({1})", eHCMSResources.T1484_G1_HChinh, pTree.NodeText.Trim());

                                //typeInfo.SetInfo_ObjRefDepartments_Current(pTree);

                                //var instance = typeInfo as Conductor<object>;

                                //Globals.ShowDialog(instance, (o) =>
                                //{
                                //    //làm gì đó
                                //});

                                void onInitDlg(IRefDepartments_Info typeInfo)
                                {
                                    typeInfo.ObjRefDepartments_Current = pTree;
                                    typeInfo.TitleForm = string.Format("{0}: ({1})", eHCMSResources.T1484_G1_HChinh, pTree.NodeText.Trim());
                                    typeInfo.SetInfo_ObjRefDepartments_Current(pTree);
                                }
                                GlobalsNAV.ShowDialog<IRefDepartments_Info>(onInitDlg);
                            }
                            else
                            {
                                if (pTree.IsDeptLocation)//Sửa Lại Cấu Hình DeptLocation
                                {
                                    //Sửa phòng
                                    //MessageBox.Show(eHCMSResources.A0994_G1_Msg_InfoSuaPg);
                                    RefDepartmentsTree pTreeKhoaCha = pTree.Parent;

                                    //var typeInfo = Globals.GetViewModel<IDeptLocation_ByDeptID>();
                                    //typeInfo.ObjKhoa_Current = pTreeKhoaCha;
                                    //typeInfo.TitleForm = string.Format("Quản Lý Phòng Của Khoa: {0}", pTreeKhoaCha.NodeText);

                                    //typeInfo.DeptLocation_GetRoomTypeByDeptID(pTreeKhoaCha.NodeID);
                                    //typeInfo.DeptLocation_ByDeptID(pTreeKhoaCha.NodeID, -1, "");

                                    //DataEntities.RoomType ItemDefault = new DataEntities.RoomType();
                                    //ItemDefault.RmTypeID = -1;
                                    //typeInfo.RoomType_SelectFind = ItemDefault;
                                    //typeInfo.LocationName = "";

                                    //var instance = typeInfo as Conductor<object>;

                                    //Globals.ShowDialog(instance, (o) =>
                                    //{
                                    //    //làm gì đó
                                    //});

                                    void onInitDlg(IDeptLocation_ByDeptID typeInfo)
                                    {
                                        typeInfo.ObjKhoa_Current = pTreeKhoaCha;
                                        typeInfo.TitleForm = string.Format(eHCMSResources.Z1453_G1_QLyPhgCuaKhoa, pTreeKhoaCha.NodeText);

                                        typeInfo.DeptLocation_GetRoomTypeByDeptID(pTreeKhoaCha.NodeID);
                                        typeInfo.DeptLocation_ByDeptID(pTreeKhoaCha.NodeID, -1, "");

                                        DataEntities.RoomType ItemDefault = new DataEntities.RoomType();
                                        ItemDefault.RmTypeID = -1;
                                        typeInfo.RoomType_SelectFind = ItemDefault;
                                        typeInfo.LocationName = "";
                                    }
                                    GlobalsNAV.ShowDialog<IDeptLocation_ByDeptID>(onInitDlg);
                                }
                                else//Sửa hiệu Chỉnh Node Các Node Trung Gian
                                {
                                    //var typeInfo = Globals.GetViewModel<IRefDepartments_Info>();
                                    //typeInfo.ObjRefDepartments_Current = pTree;

                                    //typeInfo.TitleForm = string.Format("{0}: ({1})", eHCMSResources.T1484_G1_HChinh, pTree.NodeText.Trim());

                                    //typeInfo.SetInfo_ObjRefDepartments_Current(pTree);
                                    //typeInfo.RefDepartment_SubtractAllChild_ByDeptID((typeInfo.ObjRefDepartments_Current as DataEntities.RefDepartments).DeptID);

                                    //var instance = typeInfo as Conductor<object>;

                                    //Globals.ShowDialog(instance, (o) =>
                                    //{
                                    //    //làm gì đó
                                    //});

                                    void onInitDlg(IRefDepartments_Info typeInfo)
                                    {
                                        typeInfo.ObjRefDepartments_Current = pTree;

                                        typeInfo.TitleForm = string.Format("{0}: ({1})", eHCMSResources.T1484_G1_HChinh, pTree.NodeText.Trim());

                                        typeInfo.SetInfo_ObjRefDepartments_Current(pTree);
                                        typeInfo.RefDepartment_SubtractAllChild_ByDeptID((typeInfo.ObjRefDepartments_Current as DataEntities.RefDepartments).DeptID);
                                    }
                                    GlobalsNAV.ShowDialog<IRefDepartments_Info>(onInitDlg);
                                }
                            }
                        }
                        else
                        {
                            MessageBox.Show(eHCMSResources.A0323_G1_Msg_InfoChonKhoa_VP_KhoDeHChinh, eHCMSResources.T1484_G1_HChinh, MessageBoxButton.OK);
                        }
                        break;
                    }
                case itemThem:
                    {
                        if (treeView1.SelectedItem != null)
                        {
                            RefDepartmentsTree pTree = (treeView1.SelectedItem as RefDepartmentsTree);

                            //var typeInfo = Globals.GetViewModel<IRefDepartments_Info>();
                            //typeInfo.InitializeRefDepartments_New(pTree);

                            //if (pTree.ParentID == null)
                            //{
                            //    typeInfo.TitleForm = string.Format("Thêm vào: ({0})", (typeInfo.ObjRefDepartments_ParDeptID_Current as DataEntities.RefDepartments).DeptName.Trim());
                            //}
                            //else
                            //{
                            //    typeInfo.TitleForm = string.Format("{0}:", eHCMSResources.G0276_G1_ThemMoi);
                            //}

                            //typeInfo.RefDepartment_SubtractAllChild_ByDeptID(0);//Không Ngắt bỏ nhánh nào cả chọn tùy ý thêm vào

                            //var instance = typeInfo as Conductor<object>;

                            //Globals.ShowDialog(instance, (o) =>
                            //{
                            //    //làm gì đó
                            //});

                            void onInitDlg(IRefDepartments_Info typeInfo)
                            {
                                typeInfo.InitializeRefDepartments_New(pTree);

                                if (pTree.ParentID == null)
                                {
                                    typeInfo.TitleForm = string.Format("Thêm vào: ({0})", (typeInfo.ObjRefDepartments_ParDeptID_Current as DataEntities.RefDepartments).DeptName.Trim());
                                }
                                else
                                {
                                    typeInfo.TitleForm = string.Format("{0}:", eHCMSResources.G0276_G1_ThemMoi);
                                }

                                typeInfo.RefDepartment_SubtractAllChild_ByDeptID(0);
                            }
                            GlobalsNAV.ShowDialog<IRefDepartments_Info>(onInitDlg);
                        }
                        else
                        {
                            MessageBox.Show("Chọn Khoa-Văn Phòng-Kho Để Thêm Vào!", eHCMSResources.G0276_G1_ThemMoi, MessageBoxButton.OK);
                        }
                        break;
                    }
                case itemThemPhong:
                    {
                        //Sửa phòng
                        //MessageBox.Show(eHCMSResources.A0994_G1_Msg_InfoSuaPg);
                        DataEntities.RefDepartmentsTree pTree = (treeView1.SelectedItem as DataEntities.RefDepartmentsTree);
                        //var typeInfo = Globals.GetViewModel<IDeptLocation_ByDeptID>();
                        //typeInfo.ObjKhoa_Current = pTree;
                        //typeInfo.TitleForm = string.Format("Quản lý phòng của khoa: {0}", pTree.NodeText);

                        //typeInfo.DeptLocation_GetRoomTypeByDeptID(pTree.NodeID);
                        //typeInfo.DeptLocation_ByDeptID(pTree.NodeID, -1, "");

                        //DataEntities.RoomType ItemDefault = new DataEntities.RoomType();
                        //ItemDefault.RmTypeID = -1;
                        //ItemDefault.RmTypeName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.T0822_G1_TatCa);
                        //typeInfo.RoomType_SelectFind = ItemDefault;
                        //typeInfo.LocationName = "";

                        //var instance = typeInfo as Conductor<object>;

                        //Globals.ShowDialog(instance, (o) =>
                        //{
                        //    //làm gì đó
                        //});

                        void onInitDlg(IDeptLocation_ByDeptID typeInfo)
                        {
                            typeInfo.ObjKhoa_Current = pTree;
                            typeInfo.TitleForm = string.Format(eHCMSResources.Z1453_G1_QLyPhgCuaKhoa, pTree.NodeText);

                            typeInfo.DeptLocation_GetRoomTypeByDeptID(pTree.NodeID);
                            typeInfo.DeptLocation_ByDeptID(pTree.NodeID, -1, "");

                            DataEntities.RoomType ItemDefault = new DataEntities.RoomType();
                            ItemDefault.RmTypeID = -1;
                            ItemDefault.RmTypeName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.T0822_G1_TatCa);
                            typeInfo.RoomType_SelectFind = ItemDefault;
                            typeInfo.LocationName = "";
                        }
                        GlobalsNAV.ShowDialog<IDeptLocation_ByDeptID>(onInitDlg);
                        break;
                    }
            }
            HidePopup();
        }

        public void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            brdRightClickZone.MouseRightButtonDown -= new MouseButtonEventHandler(btnRightClick_MouseRightButtonDown);
            brdRightClickZone.MouseRightButtonUp -= new MouseButtonEventHandler(brdRightClickZone_MouseRightButtonUp);
        }

        private ObservableCollection<RefDepartmentsTree> _ObjRefDepartments_Tree;
        public ObservableCollection<RefDepartmentsTree> ObjRefDepartments_Tree
        {
            get
            {
                return _ObjRefDepartments_Tree;
            }
            set
            {
                _ObjRefDepartments_Tree = value;
                NotifyOfPropertyChange(() => ObjRefDepartments_Tree);
            }
        }

        public void RefDepartments_Tree(string pstrV_DeptType, bool pShowDeptLocation)
        {
            ObjRefDepartments_Tree.Clear();

            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "Danh Sách Khoa-Văn Phòng-Kho..." });
            this.ShowBusyIndicator(eHCMSResources.Z0149_G1_DangLayDSCacKhoa);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ConfigurationManagerServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginRefDepartments_Tree(pstrV_DeptType, pShowDeptLocation, null, Globals.DispatchCallback((asyncResult) =>
                        {
                             try
                             {
                                 var items = contract.EndRefDepartments_Tree(asyncResult);
                                 if (items != null)
                                 {
                                     ObjRefDepartments_Tree = new ObservableCollection<RefDepartmentsTree>(items);
                                     ObjRefDepartments_Tree.ConvertaEMRImgIcon();
                                 }
                                 else
                                 {
                                     ObjRefDepartments_Tree = null;
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

        public void Handle(RefDepartments_SaveEvent message)
        {
            if (message != null)
            {
                RefDepartments_Tree("", true);
            }
        }

        private RefDepartmentsSearchCriteria _SearchCriteria;
        public RefDepartmentsSearchCriteria SearchCriteria
        {
            get
            {
                return _SearchCriteria;
            }
            set
            {
                _SearchCriteria = value;
                NotifyOfPropertyChange(() => SearchCriteria);
            }
        }

        public void authorization()
        {
            if (!Globals.isAccountCheck)
            {
                return;
            }
            bEdit = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConfiguration_Management
                                               , (int)eConfiguration_Management.mKhoa_VanPhong_Kho,
                                               (int)oConfigurationEx.mQuanLyKhoa_Phong, (int)ePermission.mEdit);
            bAdd = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConfiguration_Management
                                               , (int)eConfiguration_Management.mKhoa_VanPhong_Kho,
                                               (int)oConfigurationEx.mQuanLyKhoa_Phong, (int)ePermission.mAdd);
            bDelete = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConfiguration_Management
                                               , (int)eConfiguration_Management.mKhoa_VanPhong_Kho,
                                               (int)oConfigurationEx.mQuanLyKhoa_Phong, (int)ePermission.mDelete);
            bView = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConfiguration_Management
                                               , (int)eConfiguration_Management.mKhoa_VanPhong_Kho,
                                               (int)oConfigurationEx.mQuanLyKhoa_Phong, (int)ePermission.mView);
        }

        #region checking account
        private bool _bEdit = true;
        private bool _bAdd = true;
        private bool _bDelete = true;
        private bool _bView = true;
        public bool bEdit
        {
            get
            {
                return _bEdit;
            }
            set
            {
                if (_bEdit == value)
                    return;
                _bEdit = value;
            }
        }

        public bool bAdd
        {
            get
            {
                return _bAdd;
            }
            set
            {
                if (_bAdd == value)
                    return;
                _bAdd = value;
            }
        }

        public bool bDelete
        {
            get
            {
                return _bDelete;
            }
            set
            {
                if (_bDelete == value)
                    return;
                _bDelete = value;
            }
        }

        public bool bView
        {
            get
            {
                return _bView;
            }
            set
            {
                if (_bView == value)
                    return;
                _bView = value;
            }
        }
        #endregion

        public void btSearch()
        {
            SearchCriteria.ShowDeptLocation = true;
            GetSearchTreeView();
        }

        private void GetSearchTreeView()
        {
            this.ShowBusyIndicator(eHCMSResources.Z1125_G1_DangTimKiem);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ConfigurationManagerServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetSearchTreeView(SearchCriteria, Globals.DispatchCallback((asyncResult) =>
                        {
                             try
                             {
                                 var items = contract.EndGetSearchTreeView(asyncResult);
                                 if (items != null)
                                 {
                                     ObjRefDepartments_Tree = new ObservableCollection<DataEntities.RefDepartmentsTree>(items);
                                 }
                                 else
                                 {
                                     ObjRefDepartments_Tree = null;
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

        //public void DeleteRefDepartments(RefDepartmentsTree item)
        public void DeleteRefDepartments(long DeptID)
        {
            this.ShowBusyIndicator(eHCMSResources.Z0492_G1_DangXoa);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ConfigurationManagerServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginDeleteRefDepartments(DeptID, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                if (contract.EndDeleteRefDepartments(asyncResult))
                                {
                                    RefDepartments_Tree("", true);
                                    MessageBox.Show(eHCMSResources.A0478_G1_Msg_InfoXoaOK, eHCMSResources.G2617_G1_Xoa, MessageBoxButton.OK);
                                }
                                else
                                {
                                    MessageBox.Show(eHCMSResources.K0484_G1_XoaFail, eHCMSResources.G2617_G1_Xoa, MessageBoxButton.OK);
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

        public void DeptLocation_MarkDeleted(Int64 DeptLocationID)
        {
            this.ShowBusyIndicator(eHCMSResources.Z0343_G1_DangLuu);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ConfigurationManagerServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginDeptLocation_MarkDeleted(DeptLocationID, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                string Result = "";
                                contract.EndDeptLocation_MarkDeleted(out Result, asyncResult);
                                if (Result == "0")
                                {
                                    MessageBox.Show(eHCMSResources.K0484_G1_XoaFail, eHCMSResources.G2617_G1_Xoa, MessageBoxButton.OK);
                                }
                                if (Result == "1")
                                {
                                    RefDepartments_Tree("", true);
                                    MessageBox.Show(eHCMSResources.A0478_G1_Msg_InfoXoaOK, eHCMSResources.G2617_G1_Xoa, MessageBoxButton.OK);
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

        public void Handle(DeptLocation_XMLInsert_Save_Event message)
        {
            if (message != null)
            {
                RefDepartments_Tree("", true);
            }
        }

        public void Handle(DeptLocation_MarkDeleted_Event message)
        {
            if (message != null)
            {
                RefDepartments_Tree("", true);
            }
        }

        public void Handle(SaveEvent<bool> message)
        {
            if (message != null)
            {
                if (message.Result)
                {
                    RefDepartments_Tree("", true);
                }
            }
        }

        Button ctrbtSearch = new Button();
        public void btSearch_Loaded(object sender, RoutedEventArgs e)
        {
            ctrbtSearch = sender as Button;
        }

        public void txt_search_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ctrbtSearch.Focus();
                SearchCriteria.DeptName = (sender as TextBox).Text.Trim();
                btSearch();
            }
        }
    }
}
