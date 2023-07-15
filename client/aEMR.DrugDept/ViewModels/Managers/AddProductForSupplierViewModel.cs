using System.ComponentModel.Composition;
using Caliburn.Micro;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System;
using aEMR.Infrastructure.Events;
using aEMR.Infrastructure;
using aEMR.Infrastructure.CachingUtils;
using aEMR.ViewContracts;
using aEMR.ServiceClient;
using aEMR.Common.Collections;
using Castle.Core.Logging;
using Castle.Windsor;
using System.Threading;
using DataEntities;
using eHCMSLanguage;


namespace aEMR.DrugDept.ViewModels
{
    [Export(typeof(IAddProductForSupplier)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class AddProductForSupplierViewModel : Conductor<object>, IAddProductForSupplier
    , IHandle<DrugDeptCloseFinishAddSupplierEvent>, IHandle<DrugDeptCloseSearchSupplierEvent>
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public AddProductForSupplierViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            eventAggregator.Subscribe(this);

            authorization();
            ListSupplierProduct = new PagedSortableCollectionView<SupplierGenMedProduct>();
            ListSupplierProduct.OnRefresh += ListSupplierProduct_OnRefresh;
            ListSupplierProduct.PageSize = Globals.PageSize;

            RefGenMedProductDetailss = new PagedSortableCollectionView<RefGenMedProductDetails>();
            RefGenMedProductDetailss.OnRefresh += RefGenMedProductDetailss_OnRefresh;
            RefGenMedProductDetailss.PageSize = Globals.PageSize;

            _SupplierProduct = new SupplierGenMedProduct();
        }

        void RefGenMedProductDetailss_OnRefresh(object sender, RefreshEventArgs e)
        {
            SearchRefDrugGenericDetails_AutoPaging(BrandName, RefGenMedProductDetailss.PageIndex, RefGenMedProductDetailss.PageSize);
        }

        void ListSupplierProduct_OnRefresh(object sender, RefreshEventArgs e)
        {
            SupplierGenMedProduct_LoadSupplierID(ListSupplierProduct.PageIndex, ListSupplierProduct.PageSize);
        }

        private bool _IsLoading = false;
        public bool IsLoading
        {
            get { return _IsLoading; }
            set
            {
                if (_IsLoading != value)
                {
                    _IsLoading = value;
                    NotifyOfPropertyChange(() => IsLoading);
                }
            }
        }

        public void authorization()
        {
            if (!Globals.isAccountCheck)
            {
                return;
            }

        }
        #region checking account

        private bool _bEdit = true;
        private bool _bAdd = true;
        private bool _bDelete = true;
        private bool _bView = true;
        private bool _bPrint = true;
        private bool _bReport = true;

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
        public bool bPrint
        {
            get
            {
                return _bPrint;
            }
            set
            {
                if (_bPrint == value)
                    return;
                _bPrint = value;
            }
        }

        public bool bReport
        {
            get
            {
                return _bReport;
            }
            set
            {
                if (_bReport == value)
                    return;
                _bReport = value;
            }
        }
        #endregion
        #region binding visibilty

        public Button lnkDelete { get; set; }

        public void lnkDelete_Loaded(object sender)
        {
            lnkDelete = sender as Button;
            lnkDelete.Visibility = Globals.convertVisibility(bDelete);
        }

        #endregion
        #region 1. Property Member

        public bool IsSupplier = false;

        private long _V_MedProductType = 11001; //11001 : thuoc, 11002 : y cu , 11003 :hoa chat
        public long V_MedProductType
        {
            get
            {
                return _V_MedProductType;
            }
            set
            {
                if (_V_MedProductType != value)
                {
                    _V_MedProductType = value;
                    NotifyOfPropertyChange(() => V_MedProductType);
                }

            }
        }

        public long V_SupplierType = 7200;

        private SupplierGenMedProduct _SupplierProduct;
        public SupplierGenMedProduct SupplierProduct
        {
            get
            {
                return _SupplierProduct;
            }
            set
            {
                if (_SupplierProduct != value)
                {
                    _SupplierProduct = value;
                }
                NotifyOfPropertyChange(()=>SupplierProduct);
            }
        }

        private PagedSortableCollectionView<SupplierGenMedProduct> _ListSupplierProduct;
        public PagedSortableCollectionView<SupplierGenMedProduct> ListSupplierProduct
        {
            get
            {
                return _ListSupplierProduct;
            }
            set
            {
                if (_ListSupplierProduct != value)
                {
                    _ListSupplierProduct = value;
                }
                NotifyOfPropertyChange(()=>ListSupplierProduct);
            }
        }

        #endregion

        #region autoProduct Member
        private string BrandName;

        private PagedSortableCollectionView<RefGenMedProductDetails> _RefGenMedProductDetailss;
        public PagedSortableCollectionView<RefGenMedProductDetails> RefGenMedProductDetailss
        {
            get
            {
                return _RefGenMedProductDetailss;
            }
            set
            {
                if (_RefGenMedProductDetailss != value)
                {
                    _RefGenMedProductDetailss = value;
                }
                NotifyOfPropertyChange(() => RefGenMedProductDetailss);
            }
        }

        private void SearchRefDrugGenericDetails_AutoPaging(string Name, int PageIndex, int PageSize)
        {
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyDrugServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetRefGenMedProductDetails_Auto(null,Name, V_MedProductType, PageSize, PageIndex, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            int Total;
                            var results = contract.EndGetRefGenMedProductDetails_Auto(out Total, asyncResult);
                            RefGenMedProductDetailss.Clear();
                            RefGenMedProductDetailss.TotalItemCount = Total;
                            foreach (RefGenMedProductDetails p in results)
                            {
                                RefGenMedProductDetailss.Add(p);
                            }
                            au.ItemsSource = RefGenMedProductDetailss;
                            au.PopulateComplete();
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {

                        }

                    }), null);

                }

            });

            t.Start();
        }

        AutoCompleteBox au;
        public void AutoCompleteBox_Populating(object sender, PopulatingEventArgs e)
        {
            au = sender as AutoCompleteBox;
            BrandName = e.Parameter;
            RefGenMedProductDetailss.PageIndex = 0;
            SearchRefDrugGenericDetails_AutoPaging(e.Parameter, 0, RefGenMedProductDetailss.PageSize);
        }

        #endregion
        #region client member

        // Executes when the user navigates to this page.
        public void btnAddNew(object sender, RoutedEventArgs e)
        {
            //var proAlloc = Globals.GetViewModel<ISupplierProduct_Add>();
            //proAlloc.IsAddFinishClosed = true;
            //var instance = proAlloc as Conductor<object>;
            //Globals.ShowDialog(instance, (o) => { });

            Action<ISupplierProduct_Add> onInitDlg = (proAlloc) =>
            {
                proAlloc.IsAddFinishClosed = true;
            };
            GlobalsNAV.ShowDialog<ISupplierProduct_Add>(onInitDlg);
        }

        public void btnSearch(object sender, RoutedEventArgs e)
        {
            OpenPopUpSearchSupplier();
        }

        private void OpenPopUpSearchSupplier()
        {
            //var proAlloc = Globals.GetViewModel<ISupplierProduct>();
            //proAlloc.IsChildWindow = true;
            //var instance = proAlloc as Conductor<object>;
            //Globals.ShowDialog(instance, (o) => { });

            Action<ISupplierProduct> onInitDlg = (proAlloc) =>
            {
                proAlloc.IsChildWindow = true;
            };
            GlobalsNAV.ShowDialog<ISupplierProduct>(onInitDlg, null, false, true, Globals.GetDefaultDialogViewSize());
        }

        public void TextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                OpenPopUpSearchSupplier();
            }
        }

        public void rdtDrug_Click(object sender, RoutedEventArgs e)
        {
             V_MedProductType = (long)AllLookupValues.MedProductType.THUOC;
        }
        public void rdtYCu_Click(object sender, RoutedEventArgs e)
        {
            V_MedProductType = (long)AllLookupValues.MedProductType.Y_CU;
        }
        public void rdtHoaChat_Click(object sender, RoutedEventArgs e)
        {
            V_MedProductType = (long)AllLookupValues.MedProductType.HOA_CHAT;
        }
        private void SupplierGenMedProduct_LoadSupplierID(int PageIndex, int PageSize)
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            int totalCount = 0;
            IsLoading = true;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyGenMedDeptDetailsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginSupplierGenMedProduct_LoadSupplierID(SupplierProduct.SupplierID, PageSize, PageIndex, true, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            var results = contract.EndSupplierGenMedProduct_LoadSupplierID(out totalCount, asyncResult);
                            if (results != null)
                            {
                                ListSupplierProduct.Clear();
                                ListSupplierProduct.TotalItemCount = totalCount;
                                foreach (SupplierGenMedProduct p in results)
                                {
                                    ListSupplierProduct.Add(p);
                                }
                                NotifyOfPropertyChange(() => ListSupplierProduct);
                            }
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            IsLoading = false;
                            Globals.IsBusy = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }

        private void SupplierGenMedProduct_Delete()
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            IsLoading = true;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyGenMedDeptDetailsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginSupplierGenMedProduct_Delete(SupplierProduct.SupGenMedID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            contract.EndSupplierGenMedProduct_Delete(asyncResult);
                            ListSupplierProduct.PageIndex = 0;
                            SupplierGenMedProduct_LoadSupplierID(0, ListSupplierProduct.PageSize);
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            IsLoading = false;
                            Globals.IsBusy = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }

        public void btnDeleteCC_Click(object sender, RoutedEventArgs e)
        {
            if (SupplierProduct != null)
            {
                SupplierGenMedProduct_Delete();
            }
        }

        public void GridSuppliers_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString() + ". ";
        }

        private Staff GetStaffLogin()
        {
            return Globals.LoggedUserAccount.Staff;
        }

        public void SetObjectValueForSupplierProductDrug(object item)
        {
            SupplierGenMedProduct p = item as SupplierGenMedProduct;
            SupplierProduct.SelectedGenMedProduct = p.SelectedGenMedProduct;
            SupplierProduct.SupGenMedID = p.SupGenMedID;
            SupplierProduct.SupplierID = p.SupplierID;
            SupplierProduct.SupplierPriorityOrderNum = p.SupplierPriorityOrderNum;
            SupplierProduct.IsMain = p.IsMain;
            SupplierProduct.UnitPrice = p.UnitPrice;
            SupplierProduct.PackagePrice = p.PackagePrice;
            SupplierProduct.StaffID = GetStaffLogin().StaffID;
        }

        public void GridSuppliers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid GridSuppliers = sender as DataGrid;
            if (GridSuppliers.SelectedItem != null)
            {
                SetObjectValueForSupplierProductDrug(GridSuppliers.SelectedItem);
            }
        }

        private void SupplierGenMedProduct_Insert()
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            IsLoading = true;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyGenMedDeptDetailsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginSupplierGenMedProduct_Insert(SupplierProduct, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            int result = contract.EndSupplierGenMedProduct_Insert(asyncResult);
                            if (result == 0)
                            {
                                ListSupplierProduct.PageIndex = 0;
                                SupplierGenMedProduct_LoadSupplierID(0, ListSupplierProduct.PageSize);
                            }
                            else if (result == 1)
                            {
                                MessageBox.Show(eHCMSResources.A0992_G1_Msg_InfoSPDaTonTai);
                            }
                            else if (result == 2)
                            {
                                MessageBox.Show(string.Format("{0}!", eHCMSResources.A0477_G1_Msg_InfoDaTonTaiNCCChinh));
                            }
                            else if (result == 3)
                            {
                                MessageBox.Show(eHCMSResources.A0984_G1_Msg_InfoSTTUuTienDaTonTai);
                            }
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            IsLoading = false;
                            Globals.IsBusy = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }

        public void btnAdd(object sender, RoutedEventArgs e)
        {
            //kiem tra
            //Neu thuoc nay voi nhaCC nay da co thi thong bao da ton tai
            bool OK = true;
            if (SupplierProduct.SelectedGenMedProduct == null || SupplierProduct.GenMedProductID == 0)
            {
                MessageBox.Show(eHCMSResources.K0410_G1_ChonThuoc);
                OK = false;
            }
            else if (SupplierProduct.SelectedSupplier == null || SupplierProduct.SupplierID == 0)
            {
                MessageBox.Show(eHCMSResources.K0347_G1_ChonNCC);
                OK = false;
            }
            
            if (OK)
            {
                SupplierGenMedProduct_Insert();
            }
           
        }
        private void SupplierGenMedProduct_Update()
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            IsLoading = true;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyGenMedDeptDetailsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginSupplierGenMedProduct_Update(SupplierProduct, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            int result = contract.EndSupplierGenMedProduct_Update(asyncResult);
                            if (result == 0)
                            {
                                ListSupplierProduct.PageIndex = 0;
                                SupplierGenMedProduct_LoadSupplierID(0, ListSupplierProduct.PageSize);
                            }
                            else if (result == 1)
                            {
                                MessageBox.Show(eHCMSResources.A0992_G1_Msg_InfoSPDaTonTai);
                            }
                            else if (result == 2)
                            {
                                MessageBox.Show(string.Format("{0}!", eHCMSResources.A0477_G1_Msg_InfoDaTonTaiNCCChinh));
                            }
                            else if (result == 3)
                            {
                                MessageBox.Show(eHCMSResources.A0984_G1_Msg_InfoSTTUuTienDaTonTai);
                            }
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            IsLoading = false;
                            Globals.IsBusy = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }

        public void btnUpdate(object sender, RoutedEventArgs e)
        {
            //kiem tra
            //Neu thuoc nay voi nhaCC nay da co thi thong bao da ton tai
            bool OK = true;
            if (SupplierProduct == null || SupplierProduct.SupGenMedID == 0)
            {
                MessageBox.Show(string.Format("{0}!", eHCMSResources.Z0676_G1_TTinKgCoTrongDLieu));
                OK = false;
            }
            else if (SupplierProduct.SelectedGenMedProduct == null || SupplierProduct.GenMedProductID == 0)
            {
                MessageBox.Show(eHCMSResources.K0388_G1_ChonSp);
                OK = false;
            }
            else if (SupplierProduct.SelectedSupplier == null || SupplierProduct.SupplierID == 0)
            {
                MessageBox.Show(eHCMSResources.K0347_G1_ChonNCC);
                OK = false;
            }
          
            if (OK)
            {
                SupplierGenMedProduct_Update();
            }
        }

        #endregion

        #region IHandle<DrugDeptCloseFinishAddSupplierEvent> Members

        public void Handle(DrugDeptCloseFinishAddSupplierEvent message)
        {
            if (message != null && this.IsActive)
            {
                SupplierProduct.SelectedSupplier = message.SelectedSupplier as DrugDeptSupplier;
                ListSupplierProduct.PageIndex = 0;
                SupplierGenMedProduct_LoadSupplierID(0, ListSupplierProduct.PageSize);
            }
        }

        #endregion

        #region IHandle<DrugDeptCloseSearchSupplierEvent> Members

        public void Handle(DrugDeptCloseSearchSupplierEvent message)
        {
            if (message != null && this.IsActive)
            {
                SupplierProduct.SelectedSupplier = message.SelectedSupplier as DrugDeptSupplier;
                ListSupplierProduct.PageIndex = 0;
                SupplierGenMedProduct_LoadSupplierID(0, ListSupplierProduct.PageSize);
            }
        }

        #endregion
    }
}
