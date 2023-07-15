using System.ComponentModel.Composition;
using aEMR.ViewContracts;
using Caliburn.Micro;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System;
using aEMR.Infrastructure;
using System.Collections.ObjectModel;
using aEMR.ServiceClient;
using System.Threading;
using DataEntities;
using eHCMSLanguage;
using aEMR.Common.BaseModel;
using System.Windows.Media;
using System.Windows.Controls.Primitives;
using System.Linq;
using aEMR.Common.Collections;
using aEMR.Controls;

/*
 * 20181002 #001 TBL: BM 0000070. Fix khi tu QL Hoat chat qua cac QL khac thi hien thong bao loi
 */
namespace aEMR.DrugDept.ViewModels
{
    [Export(typeof(IGenericClass)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class GenericClassViewModel : ViewModelBase, IGenericClass
    {
        public string TitleForm { get; set; }

        #region Indicator Member
      

        private bool _isLoadingFullOperator = false;
        public bool isLoadingFullOperator
        {
            get { return _isLoadingFullOperator; }
            set
            {
                if (_isLoadingFullOperator != value)
                {
                    _isLoadingFullOperator = value;
                    NotifyOfPropertyChange(() => isLoadingFullOperator);
                    NotifyOfPropertyChange(() => IsLoading);
                }
            }
        }

    

        private bool _isLoadingGetID = false;
        public bool isLoadingGetID
        {
            get { return _isLoadingGetID; }
            set
            {
                if (_isLoadingGetID != value)
                {
                    _isLoadingGetID = value;
                    NotifyOfPropertyChange(() => isLoadingGetID);
                    NotifyOfPropertyChange(() => IsLoading);
                }
            }
        }

        private bool _isLoadingSearch = false;
        public bool isLoadingSearch
        {
            get { return _isLoadingSearch; }
            set
            {
                if (_isLoadingSearch != value)
                {
                    _isLoadingSearch = value;
                    NotifyOfPropertyChange(() => isLoadingSearch);
                    NotifyOfPropertyChange(() => IsLoading);
                }
            }
        }

   
      

        public bool IsLoading
        {
            get { return ( isLoadingFullOperator  || isLoadingGetID || isLoadingSearch); }
        }

        #endregion

        private long _V_MedProductType;
        public long V_MedProductType
        {
            get { return _V_MedProductType; }
            set {
                _V_MedProductType = value;
                NotifyOfPropertyChange(()=>V_MedProductType);
            }
        }
        public GenericClassViewModel()
        {
            authorization();
            _TreeViewTherapies = new ObservableCollection<TherapyTree>();
            _therapiesall = new ObservableCollection<TherapyTree>();
            _ObsRefGenericRelation = new ObservableCollection<TherapyTree>();
            _seletedfamilyTherapy = new DrugClass();

            WarningLevel = new ObservableCollection<Lookup>();
            WarningLevel = Globals.AllLookupValueList.Where(x => x.ObjectTypeID == (long)LookupValues.V_WarningLevel).ToObservableCollection();
            InteractionSeverityLevel = new ObservableCollection<Lookup>();
            InteractionSeverityLevel = Globals.AllLookupValueList.Where(x => x.ObjectTypeID == (long)LookupValues.V_InteractionSeverityLevel).ToObservableCollection();
        }

        #region Properties Member
        private ObservableCollection<TherapyTree> _TreeViewTherapies;
        public ObservableCollection<TherapyTree> TreeViewTherapies
        {
            get
            {
                return _TreeViewTherapies;
            }
            set
            {
                if (_TreeViewTherapies != value)
                {
                    _TreeViewTherapies = value;
                    NotifyOfPropertyChange(() => TreeViewTherapies);
                }
            }
        }

        private ObservableCollection<TherapyTree> _therapiesall;
        public ObservableCollection<TherapyTree> TherapiesAll
        {
            get
            {
                return _therapiesall;
            }
            set
            {
                if (_therapiesall != value)
                {
                    _therapiesall = value;
                    NotifyOfPropertyChange(() => TherapiesAll);
                }
            }
        }

        private ObservableCollection<TherapyTree> _ObsRefGenericRelation;
        public ObservableCollection<TherapyTree> ObsRefGenericRelation
        {
            get
            {
                return _ObsRefGenericRelation;
            }
            set
            {
                if (_ObsRefGenericRelation != value)
                {
                    _ObsRefGenericRelation = value;
                    NotifyOfPropertyChange(() => ObsRefGenericRelation);
                }
            }
        }

        private ObservableCollection<DrugClass> _parentDrugClass;
        public ObservableCollection<DrugClass> ParentDrugClass
        {
            get
            {
                return _parentDrugClass;
            }
            set
            {
                if (_parentDrugClass != value)
                {
                    _parentDrugClass = value;
                    NotifyOfPropertyChange(() => ParentDrugClass);
                }
            }
        }

        private string _faname;
        public string FaName
        {
            get
            {
                return _faname;
            }
            set
            {
                if (_faname != value)
                {
                    _faname = value;
                    NotifyOfPropertyChange(() => FaName);
                }
            }
        }

        private bool _IsEnabled = true;
        public bool IsEnabled
        {
            get
            {
                return _IsEnabled;
            }
            set
            {
                if (_IsEnabled != value)
                {
                    _IsEnabled = value;
                    NotifyOfPropertyChange(() => IsEnabled);
                }
            }
        }
        private bool _IsEnabledTxt = false;
        public bool IsEnabledTxt
        {
            get
            {
                return _IsEnabledTxt;
            }
            set
            {
                if (_IsEnabledTxt != value)
                {
                    _IsEnabledTxt = value;
                    NotifyOfPropertyChange(() => IsEnabledTxt);
                }
            }
        }
        private bool _Visibility = false;
        public bool Visibility
        {
            get
            {
                return _Visibility;
            }
            set
            {
                if (_Visibility != value)
                {
                    _Visibility = value;
                    NotifyOfPropertyChange(() => Visibility);
                }
            }
        }

        private DrugClass _seletedfamilyTherapy;
        public DrugClass SeletedFamilyTherapy
        {
            get
            {
                return _seletedfamilyTherapy;
            }
            set
            {
                if (_seletedfamilyTherapy != value)
                {
                    _seletedfamilyTherapy = value;
                    NotifyOfPropertyChange(() => SeletedFamilyTherapy);
                }
            }
        }

        private TherapyTree _CurrentTherapyTree;
        public TherapyTree CurrentTherapyTree
        {
            get
            {
                return _CurrentTherapyTree;
            }
            set
            {
                if (_CurrentTherapyTree != value)
                {
                    _CurrentTherapyTree = value;
                    NotifyOfPropertyChange(() => CurrentTherapyTree);
                }
            }
        }

        private TherapyTree _GenericFunc;
        public TherapyTree GenericFunc
        {
            get
            {
                return _GenericFunc;
            }
            set
            {
                if (_GenericFunc != value)
                {
                    _GenericFunc = value;
                    NotifyOfPropertyChange(() => GenericFunc);
                }
            }
        }

        #endregion


        public void authorization()
        {
            if (!Globals.isAccountCheck)
            {
                return;
            }

        }
        #region checking account

        private bool _bTim = true;
        private bool _bThem = true;
        private bool _bChinhSua = true;

        public bool bTim
        {
            get
            {
                return _bTim;
            }
            set
            {
                if (_bTim == value)
                    return;
                _bTim = value;
            }
        }
        public bool bThem
        {
            get
            {
                return _bThem;
            }
            set
            {
                if (_bThem == value)
                    return;
                _bThem = value;
            }
        }
        public bool bChinhSua
        {
            get
            {
                return _bChinhSua;
            }
            set
            {
                if (_bChinhSua == value)
                    return;
                _bChinhSua = value;
            }
        }

        public bool FlagRelation
        {
            get
            {
                return _FlagRelation;
            }
            set
            {
                if (_FlagRelation != value)
                {
                    _FlagRelation = value;
                    NotifyOfPropertyChange(() => FlagRelation);
                }
            }
        }
        private bool _FlagRelation;

        public bool VisiRelation
        {
            get
            {
                return _VisiRelation;
            }
            set
            {
                if (_VisiRelation != value)
                {
                    _VisiRelation = value;
                    NotifyOfPropertyChange(() => VisiRelation);
                }
            }
        }
        private bool _VisiRelation;

        public bool VisiAddRelation
        {
            get
            {
                return _VisiAddRelation;
            }
            set
            {
                if (_VisiAddRelation != value)
                {
                    _VisiAddRelation = value;
                    NotifyOfPropertyChange(() => VisiAddRelation);
                }
            }
        }
        private bool _VisiAddRelation = true;

        private ObservableCollection<Lookup> _WarningLevel;
        public ObservableCollection<Lookup> WarningLevel
        {
            get
            {
                return _WarningLevel;
            }
            set
            {
                if (_WarningLevel != value)
                {
                    _WarningLevel = value;
                    NotifyOfPropertyChange(() => WarningLevel);
                }
            }
        }

        private ObservableCollection<Lookup> _InteractionSeverityLevel;
        public ObservableCollection<Lookup> InteractionSeverityLevel
        {
            get
            {
                return _InteractionSeverityLevel;
            }
            set
            {
                if (_InteractionSeverityLevel != value)
                {
                    _InteractionSeverityLevel = value;
                    NotifyOfPropertyChange(() => InteractionSeverityLevel);
                }
            }
        }

        #endregion
        #region binding visibilty

        public Button lnkDelete { get; set; }

        public void lnkDelete_Loaded(object sender)
        {
            lnkDelete = sender as Button;
            lnkDelete.Visibility = Globals.convertVisibility(bChinhSua);
        }

        #endregion
        private void GetAllChildrenNode(TherapyTree p)
        {
            foreach (TherapyTree pp in p.Children)
            {
                TherapiesAll.Add(pp);
                GetAllChildrenNode(pp);
            }
        }
        public void GetSearchTreeView(long V_MedProductType)
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            this.DlgShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            isLoadingSearch = true;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyDrugServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetSearchTreeView_DrugDept(FaName,V_MedProductType, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndGetSearchTreeView_DrugDept(asyncResult);
                            if (results != null)
                            {
                                TreeViewTherapies.Clear();
                                TherapiesAll.Clear();
                                foreach (TherapyTree p in results)
                                {
                                    TreeViewTherapies.Add(p);
                                    TherapiesAll.Add(p);
                                    GetAllChildrenNode(p);
                                }
                                NotifyOfPropertyChange(() => TreeViewTherapies);
                                /*▼====: #001*/
                                LoadFamilyParent(V_MedProductType);
                                /*▲====: #001*/
                            }
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            isLoadingSearch = false;
                            // Globals.IsBusy = false;
                            this.DlgHideBusyIndicator();
                        }

                    }), null);

                }

            });

            t.Start();
        }

        public void LoadFamilyParent(long V_MedProductType)
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            this.DlgShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            isLoadingGetID = true;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyDrugServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetDrugDeptClassesParent(V_MedProductType, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            var results = contract.EndGetDrugDeptClassesParent(asyncResult);
                            if (results != null)
                            {
                                if (ParentDrugClass == null)
                                {
                                    ParentDrugClass = new ObservableCollection<DrugClass>();
                                }
                                else
                                {
                                    ParentDrugClass.Clear();
                                }
                                DrugClass all = new DrugClass();
                                all.DrugClassID = 0;
                                all.FaName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.Z0637_G1_NoSelect);
                                ParentDrugClass.Add(all);

                                foreach (DrugClass p in results)
                                {
                                    ParentDrugClass.Add(p);
                                }
                                NotifyOfPropertyChange(() => ParentDrugClass);
                                cbx.ItemsSource = ParentDrugClass;
                            }
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            isLoadingGetID = false;
                            //Globals.IsBusy = false;
                            this.DlgHideBusyIndicator();
                        }

                    }), null);

                }

            });

            t.Start();
            //_therapyCatalog.GetFamilytherapyParent();
        }

        private void ConvertTherapyTreeToRef(TherapyTree tree)
        {
            if (tree != null)
            {
                if (tree.Parent == null)
                {
                    tree.Parent = new DrugClass();
                }
                SeletedFamilyTherapy.DrugClassID = tree.NodeID;
                SeletedFamilyTherapy.FaDescription = tree.Description;
                SeletedFamilyTherapy.FaName = tree.NodeText;
                SeletedFamilyTherapy.DrugClassCode = tree.Code;
                SeletedFamilyTherapy.ParDrugClassID = tree.Parent.DrugClassID;
                SeletedFamilyTherapy.FaActive = true;
                NotifyOfPropertyChange(() => SeletedFamilyTherapy);
            }
        }

        private void AddDrugClass()
        {
           // Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            SeletedFamilyTherapy.V_MedProductType = V_MedProductType;

            isLoadingFullOperator = true;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyDrugServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginAddNewDrugDeptClasses(SeletedFamilyTherapy, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            string StrError = "";
                            contract.EndAddNewDrugDeptClasses(out StrError,asyncResult);
                            if (string.IsNullOrEmpty(StrError))
                            {
                                Globals.ShowMessage(string.Format("{0}!", eHCMSResources.A0079_G1_Msg_InfoThemMoiOK), eHCMSResources.G0442_G1_TBao);
                                IsEnabled = true;
                                GetSearchTreeView(V_MedProductType);
                                /*▼====: #001*/
                                //LoadFamilyParent(V_MedProductType);
                                /*▲====: #001*/
                                CurrentTherapyTree = null;
                            }
                            else
                            {
                                Globals.ShowMessage(StrError,eHCMSResources.G0442_G1_TBao);
                            }
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            isLoadingFullOperator = false;
                           // Globals.IsBusy = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }

        private void AddDrugClass(object newFamilyTherapy)
        {
            if(!bThem)
            {
                Globals.ShowMessage(string.Format("{0}!", eHCMSResources.Z0638_G1_ChuaCapQuyenThemLopThuoc),"");
                return;
            }
            ConvertTherapyTreeToRef((TherapyTree)newFamilyTherapy);
            AddDrugClass();
        }

        private void UpdateDrugClass()
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            isLoadingFullOperator = true;
            SeletedFamilyTherapy.V_MedProductType = V_MedProductType;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyDrugServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginUpdateDrugDeptClasses(SeletedFamilyTherapy, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            string StrError = "";
                            contract.EndUpdateDrugDeptClasses(out StrError,asyncResult);
                            if (string.IsNullOrEmpty(StrError))
                            {
                                Globals.ShowMessage(string.Format("{0}!", eHCMSResources.A0279_G1_Msg_InfoCNhatOK), eHCMSResources.G0442_G1_TBao);
                                IsEnabled = true;
                                GetSearchTreeView(V_MedProductType);
                                /*▼====: #001*/
                                //LoadFamilyParent(V_MedProductType);
                                /*▲====: #001*/
                                CurrentTherapyTree = null;
                            }
                            else
                            {
                                Globals.ShowMessage(StrError, eHCMSResources.G0442_G1_TBao);
                            }
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            isLoadingFullOperator = false;
                            //Globals.IsBusy = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }

        private void UpdateDrugClass(object newFamilyTherapy)
        {
            if (!bChinhSua)
            {
                Globals.ShowMessage(string.Format("{0}!", eHCMSResources.Z0639_G1_ChuaCapQuyenSuaLopThuoc), "");
                return;
            }
            ConvertTherapyTreeToRef((TherapyTree)newFamilyTherapy);
            UpdateDrugClass();
        }

        private void DeleteDrugClass()
        {
           // Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            isLoadingFullOperator = true;
            SeletedFamilyTherapy.V_MedProductType = V_MedProductType;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyDrugServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginDeleteDrugDeptClasses(SeletedFamilyTherapy, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            string StrError = "";
                            contract.EndDeleteDrugDeptClasses(out StrError, asyncResult);
                            if (string.IsNullOrEmpty(StrError))
                            {
                                Globals.ShowMessage(string.Format("{0}!", eHCMSResources.K0537_G1_XoaOk), eHCMSResources.G0442_G1_TBao);
                                IsEnabled = true;
                                GetSearchTreeView(V_MedProductType);
                                /*▼====: #001*/
                                //LoadFamilyParent(V_MedProductType);
                                /*▲====: #001*/
                                CurrentTherapyTree = null;
                            }
                            else
                            {
                                Globals.ShowMessage(StrError, eHCMSResources.G0442_G1_TBao);
                            }
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            isLoadingFullOperator = false;
                            //Globals.IsBusy = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }

        private void DeleteDrugClass(object Current)
        {
            if (!bChinhSua)
            {
                Globals.ShowMessage(string.Format("{0}!", eHCMSResources.Z0640_G1_ChuaCapQuyenXoaLopThuoc), "");
                return;
            }
            ConvertTherapyTreeToRef((TherapyTree)Current);
            DeleteDrugClass();
        }

        public void treeView1_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (e.NewValue != null && !FlagRelation)
            {
                CurrentTherapyTree = (TherapyTree)e.NewValue;
                GetRefGenericRelation_ForGenericID(CurrentTherapyTree.NodeID);
            }
        }

        public void txt_search_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                FaName = (sender as TextBox).Text;
                GetSearchTreeView(V_MedProductType);
            }
        }
        public void Search(object sender, RoutedEventArgs e)
        {
            GetSearchTreeView(V_MedProductType);
        }
        int flag = 0;
        ComboBox cbx;
        public void cbxFamilyTherapy_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            cbx = sender as ComboBox;
        }
        public void btnAdd(object sender, RoutedEventArgs e)
        {
            CurrentTherapyTree = null;
            CurrentTherapyTree = new TherapyTree();
            flag = 2;
            EnableBtn();
        }
        public void btnEdit(object sender, RoutedEventArgs e)
        {
            if (CurrentTherapyTree == null)
            {
                MessageBox.Show(eHCMSResources.Z2257_G1_CCDongDeChinhSua, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
            flag = 1;
            EnableBtn();
        }
        public void btnDelete(object sender, RoutedEventArgs e)
        {
            if (CurrentTherapyTree != null)
            {
                if (MessageBox.Show(eHCMSResources.A0166_G1_Msg_ConfXoaLopThuoc, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    DeleteDrugClass(CurrentTherapyTree);
                }
            }
            else
            {
                MessageBox.Show(eHCMSResources.Z2256_G1_CCDongDeXoa, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
            }
        }
        public void btnSave(object sender, RoutedEventArgs e)
        {
            VisibilityBtn();
            if (flag == 1)
            {
                UpdateDrugClass(CurrentTherapyTree);
                flag = 0;
            }
            else if (flag == 2)
            {
                AddDrugClass(CurrentTherapyTree);
                flag = 0;
            }
        }
        public void btnCancel(object sender, RoutedEventArgs e)
        {
            CurrentTherapyTree = null;
            flag = 0;
            VisibilityBtn();
        }

        private void EnableBtn()
        {
            IsEnabledTxt = true;
            IsEnabled = false;
            Visibility = true;
        }

        private void VisibilityBtn()
        {
            IsEnabledTxt = false;
            IsEnabled = true;
            Visibility = false;
        }

        const string itemTuongTu = "Tương tự";
        const string itemTuongTac = "Tương tác";

        Border brdRightClickZone;
        public void brdRightClickZone_Loaded(object sender, RoutedEventArgs e)
        {
            brdRightClickZone = sender as Border;
            brdRightClickZone.MouseRightButtonDown += new MouseButtonEventHandler(btnRightClick_MouseRightButtonDown);
            brdRightClickZone.MouseRightButtonUp += new MouseButtonEventHandler(brdRightClickZone_MouseRightButtonUp);
        }

        public void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            brdRightClickZone.MouseRightButtonDown -= new MouseButtonEventHandler(btnRightClick_MouseRightButtonDown);
            brdRightClickZone.MouseRightButtonUp -= new MouseButtonEventHandler(brdRightClickZone_MouseRightButtonUp);
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
            if (!FlagRelation)
            {
                return;
            }
            Point currentMousePosition = e.GetPosition(AreaRL);
            FrameworkElement obj = treeView1.InputHitTest(currentMousePosition) as FrameworkElement;
            TreeViewItem item = obj.FindParentOfType<TreeViewItem>();
            if (item != null)
            {
                item.Focus();
                item.IsSelected = true;
                string[] contextMenuItemsText = CreateItemsMenuContextByLevel();
                Matrix matrix = ((MatrixTransform)AreaRL.TransformToVisual(Application.Current.MainWindow)).Matrix;
                currentMousePosition = new Point(currentMousePosition.X + matrix.OffsetX, currentMousePosition.Y + matrix.OffsetY);
                ShowPopup(currentMousePosition, contextMenuItemsText);
            }
        }

        private string[] CreateItemsMenuContextByLevel()
        {
            string[] contextMenuItemsText;
            contextMenuItemsText = new string[] { itemTuongTu, itemTuongTac };
            return contextMenuItemsText;
        }

        Popup popup = null;
        private void ShowPopup(Point currentMousePosition, string[] contextMenuItemsText)
        {
            double scrollHeight = 0.0;
            double scrollWidth = 0.0;
            
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

            popup.Child = CreateContextMenuItems(currentMousePosition, contextMenuItemsText);

            popup.HorizontalOffset = currentMousePosition.X + 20;
            popup.VerticalOffset = currentMousePosition.Y + 10;
            popup.Placement = PlacementMode.Bottom;

            return popup;
        }

        private FrameworkElement CreateContextMenuItems(Point currentMousePosition, string[] contextMenuItemsText)
        {
            ListBox lstContextMenu = new ListBox();
            foreach (string str in contextMenuItemsText)
            {
                TextBlock txb = new TextBlock() { Text = str };
                txb.MouseLeftButtonUp += new MouseButtonEventHandler(txb_MouseLeftButtonUp);
                lstContextMenu.Items.Add(txb);
            }
            return lstContextMenu;
        }
        TherapyTree pTree;
        protected void txb_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            string s = (sender as TextBlock).Text.ToString();
            if (treeView1.SelectedItem == null)
            {
                return;
            }
            pTree = (treeView1.SelectedItem as TherapyTree);
            
            if (!CheckIdentical(pTree))
            {
                pTree.V_InteractionSeverityLevel = new Lookup
                {
                    LookupID = (long)AllLookupValues.V_InteractionSeverityLevel.Level0,
                    ObjectValue = "Cấp 0"
                };
                pTree.V_InteractionWarningLevel = new Lookup
                {
                    LookupID = (long)AllLookupValues.V_WarningLevel.Normal,
                    ObjectValue = eHCMSResources.K1265_G1_BThg
                };
                switch (s)
                {
                    case itemTuongTu:
                        {
                            pTree.IsSimilar = true;
                            ObsRefGenericRelation.Add(pTree);
                            break;
                        }
                    case itemTuongTac:
                        {
                            pTree.IsInteraction = true;
                            ObsRefGenericRelation.Add(pTree);
                            break;
                        }
                }
            }
            HidePopup();
        }

        public void Root_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            HidePopup();
        }

        private void HidePopup()
        {
            if (popup != null)
                popup.IsOpen = false;
        }

        DataGrid GridGenericFunc = null;
        public void GridGenericFunc_Loaded(object sender, RoutedEventArgs e)
        {
            GridGenericFunc = sender as DataGrid;
        }
        public void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show(eHCMSResources.A0118_G1_Msg_ConfXoaDong, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                if (GridGenericFunc != null && GridGenericFunc.SelectedItem != null)
                {
                    (GridGenericFunc.SelectedItem as TherapyTree).IsInteraction = false;
                    (GridGenericFunc.SelectedItem as TherapyTree).IsSimilar = false;
                    ObsRefGenericRelation.Remove(GridGenericFunc.SelectedItem as TherapyTree);
                }
            }
        }

        bool CheckIdentical(TherapyTree SelectedItem)
        {
            if (CurrentTherapyTree.NodeID == SelectedItem.NodeID)
            {
                MessageBox.Show(eHCMSResources.Z2668_G1_TrungHoatChatDangThemQH);
                return true;
            }
            foreach (TherapyTree GenericItem in ObsRefGenericRelation)
            {
                if (GenericItem.NodeID == SelectedItem.NodeID)
                {
                    MessageBox.Show(eHCMSResources.T1987_G1_DaTonTai);
                    return true;
                }
            }
            return false;
        }

        public void btnRelation()
        {
            FlagRelation = true;
            VisiRelation = true;
            VisiAddRelation = false;
        }

        public void btnCancelGenericRelation()
        {
            FlagRelation = false;
            VisiRelation = false;
            VisiAddRelation = true;
        }

        public void btnSaveGenericRelation()
        {
            if (ObsRefGenericRelation == null)
            {
                return;
            }
            InsertRefGenericRelation(CurrentTherapyTree, ObsRefGenericRelation);
        }

        public void InsertRefGenericRelation(TherapyTree CurrentTherapyTree, ObservableCollection<TherapyTree> ObsRefGenericRelation)
        {
            this.DlgShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyDrugServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginInsertRefGenericRelation(CurrentTherapyTree, ObsRefGenericRelation, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndInsertRefGenericRelation(asyncResult);
                            if (results)
                            {
                                MessageBox.Show(eHCMSResources.A0468_G1_Msg_InfoLuuOK);
                                btnCancelGenericRelation();
                            }
                            else
                            {
                                MessageBox.Show(eHCMSResources.Z0477_G1_LuuKhongThanhCong);
                            }
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            this.DlgHideBusyIndicator();
                        }
                    }), null);
                }
            });
            t.Start();
        }

        public void cbxWarningLevel_Loaded(object sender, RoutedEventArgs e)
        {
            var kbEnabledComboBox = sender as KeyEnabledComboBox;
            if (kbEnabledComboBox != null)
            {
                kbEnabledComboBox.ItemsSource = WarningLevel;
            }
        }

        public void cbxInteractionSeverityLevel_Loaded(object sender, RoutedEventArgs e)
        {
            var cbb = sender as KeyEnabledComboBox;
            if (cbb != null)
            {
                cbb.ItemsSource = InteractionSeverityLevel;
            }
        }

        public void GetRefGenericRelation_ForGenericID(long GenericID)
        {
            this.DlgShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyDrugServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetRefGenericRelation_ForGenericID(GenericID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var lstRefGenericRelation = contract.EndGetRefGenericRelation_ForGenericID(asyncResult);
                            if (lstRefGenericRelation != null && lstRefGenericRelation.Count > 0)
                            {
                                ObsRefGenericRelation = lstRefGenericRelation.ToObservableCollection();
                            }
                            else
                            {
                                ObsRefGenericRelation = new ObservableCollection<TherapyTree>();
                            }
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            this.DlgHideBusyIndicator();
                        }
                    }), null);

                }
            });
            t.Start();
        }
    }
}
