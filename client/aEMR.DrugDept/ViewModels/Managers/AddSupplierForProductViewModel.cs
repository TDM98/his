using System.ComponentModel.Composition;
using aEMR.ViewContracts;
using Caliburn.Micro;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System;
using aEMR.Infrastructure;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using System.Collections.ObjectModel;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using System.Threading;
using DataEntities;
using aEMR.Common.Collections;
using System.Linq;
using System.Collections.Generic;
using aEMR.Common;
using System.Windows.Media;
using Service.Core.Common;
using aEMR.DrugDept.Views;
using System.Text;
using eHCMSLanguage;

namespace aEMR.DrugDept.ViewModels
{
    [Export(typeof(IAddSupplierForProduct)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class AddSupplierForProductViewModel : Conductor<object>, IAddSupplierForProduct
          , IHandle<DrugDeptCloseSearchDrugEvent>
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public AddSupplierForProductViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            authorization();
            _SupplierProduct = new SupplierGenMedProduct();

            SupplierCriteria = new SupplierSearchCriteria();
            SupplierCriteria.V_SupplierType = V_SupplierType;

            ListSupplierProduct = new PagedSortableCollectionView<SupplierGenMedProduct>();
            ListSupplierProduct.OnRefresh += ListSupplierProduct_OnRefresh;
            ListSupplierProduct.PageSize = Globals.PageSize;

            Suppliers = new PagedSortableCollectionView<Supplier>();
            Suppliers.OnRefresh += Suppliers_OnRefresh;
            Suppliers.PageSize = Globals.PageSize;

            eventAggregator.Subscribe(this);
        }

        void Suppliers_OnRefresh(object sender, RefreshEventArgs e)
        {
            SearchSupplierAuto(Suppliers.PageIndex, Suppliers.PageSize);
        }

        void ListSupplierProduct_OnRefresh(object sender, RefreshEventArgs e)
        {
            SupplierGenMedProduct_LoadDrugID(ListSupplierProduct.PageIndex, ListSupplierProduct.PageSize);
        }

        public long V_SupplierType = 7200;

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
        #region Properties Member

        private long _V_MedProductType = (long)AllLookupValues.MedProductType.THUOC; //11001 : thuoc, 11002 : y cu , 11003 :hoa chat
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

        private string _BrandName;
        public string BrandName
        {
            get { return _BrandName; }
            set
            {
                _BrandName = value;
                NotifyOfPropertyChange(() => BrandName);
            }
        }

        public bool IsSupplier = false; //neu false la load theo DrugID , nguoc lai load theo NCC

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
                NotifyOfPropertyChange(() => SupplierProduct);
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
                NotifyOfPropertyChange(() => ListSupplierProduct);
            }
        }
        #endregion

        public void GridSuppliers_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString() + ". ";
        }
        private Staff GetStaffLogin()
        {
            return Globals.LoggedUserAccount.Staff;
        }
        private void SetObjectValueForSupplierProduct(object item)
        {
            SupplierGenMedProduct p = item as SupplierGenMedProduct;
            SupplierProduct.SelectedSupplier = p.SelectedSupplier;
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
                SetObjectValueForSupplierProduct(GridSuppliers.SelectedItem);
            }
        }
        public void btnSearch(object sender, RoutedEventArgs e)
        {
            OpenChildWindow();
        }
        public void rdtDrug_Checked(object sender, RoutedEventArgs e)
        {
            V_MedProductType = (long)AllLookupValues.MedProductType.THUOC;
        }

        public void rdtDispMedItem_Checked(object sender, RoutedEventArgs e)
        {
            V_MedProductType = (long)AllLookupValues.MedProductType.Y_CU;
        }

        public void rdtChemical_Checked(object sender, RoutedEventArgs e)
        {
            V_MedProductType = (long)AllLookupValues.MedProductType.HOA_CHAT;
        }

        public void TextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                OpenChildWindow();
            }
        }

        private void OpenChildWindow()
        {
            //var proAlloc = Globals.GetViewModel<IDrugList>();
            //proAlloc.IsPopUp = true;
            //proAlloc.V_MedProductType = V_MedProductType;
            //if (V_MedProductType == (long)AllLookupValues.MedProductType.THUOC)
            //{
            //    proAlloc.TitleForm = eHCMSResources.K2906_G1_DMucThuoc;
            //    proAlloc.TextGroupTimKiem = eHCMSResources.G1230_G1_TimKiemThuoc;
            //    proAlloc.TextButtonThemMoi = eHCMSResources.Z0459_G1_ThemMoiThuoc;
            //    proAlloc.TextDanhSach = eHCMSResources.K3080_G1_DSThuoc;
            //    proAlloc.dgColumnExtOfThuoc_Visible = Visibility.Visible;
            //}
            //else if (V_MedProductType == (long)AllLookupValues.MedProductType.Y_CU)
            //{
            //    proAlloc.TitleForm = eHCMSResources.K2917_G1_DMucYCu;
            //    proAlloc.TextGroupTimKiem = eHCMSResources.Z0678_G1_TimKiemYCu;
            //    proAlloc.TextButtonThemMoi = eHCMSResources.Z0679_G1_ThemMoiYCu;
            //    proAlloc.TextDanhSach = eHCMSResources.Z0657_G1_DSYCu;
            //    proAlloc.dgColumnExtOfThuoc_Visible = Visibility.Collapsed;
            //}
            //else
            //{
            //    proAlloc.TitleForm = eHCMSResources.K2895_G1_DMucHChat;
            //    proAlloc.TextGroupTimKiem = eHCMSResources.Z0680_G1_TimKIemHChat;
            //    proAlloc.TextButtonThemMoi = eHCMSResources.Z0681_G1_ThemMoiHChat;
            //    proAlloc.TextDanhSach = eHCMSResources.Z0658_G1_DSHChat;
            //    proAlloc.dgColumnExtOfThuoc_Visible = Visibility.Collapsed;
            //}
            //var instance = proAlloc as Conductor<object>;
            //Globals.ShowDialog(instance, (o) => { });

            Action<IDrugList> onInitDlg = (proAlloc) =>
            {
                proAlloc.IsPopUp = true;
                proAlloc.V_MedProductType = V_MedProductType;
                if (V_MedProductType == (long)AllLookupValues.MedProductType.THUOC)
                {
                    proAlloc.TitleForm = eHCMSResources.K2906_G1_DMucThuoc;
                    proAlloc.TextGroupTimKiem = eHCMSResources.G1230_G1_TimKiemThuoc;
                    proAlloc.TextButtonThemMoi = eHCMSResources.Z0459_G1_ThemMoiThuoc;
                    proAlloc.TextDanhSach = eHCMSResources.K3080_G1_DSThuoc;
                    proAlloc.dgColumnExtOfThuoc_Visible = Visibility.Visible;
                }
                else if (V_MedProductType == (long)AllLookupValues.MedProductType.Y_CU)
                {
                    proAlloc.TitleForm = eHCMSResources.K2917_G1_DMucYCu;
                    proAlloc.TextGroupTimKiem = eHCMSResources.Z0678_G1_TimKiemYCu;
                    proAlloc.TextButtonThemMoi = eHCMSResources.Z0679_G1_ThemMoiYCu;
                    proAlloc.TextDanhSach = eHCMSResources.Z0657_G1_DSYCu;
                    proAlloc.dgColumnExtOfThuoc_Visible = Visibility.Collapsed;
                }
                else
                {
                    proAlloc.TitleForm = eHCMSResources.K2895_G1_DMucHChat;
                    proAlloc.TextGroupTimKiem = eHCMSResources.Z0680_G1_TimKIemHChat;
                    proAlloc.TextButtonThemMoi = eHCMSResources.Z0681_G1_ThemMoiHChat;
                    proAlloc.TextDanhSach = eHCMSResources.Z0658_G1_DSHChat;
                    proAlloc.dgColumnExtOfThuoc_Visible = Visibility.Collapsed;
                }
            };
            GlobalsNAV.ShowDialog<IDrugList>(onInitDlg);
        }

        public void btnAddNew(object senser, RoutedEventArgs e)
        {
            //var proAlloc = Globals.GetViewModel<IDrug_AddEdit>();
            //// proAlloc.IsAddFinishClosed = true;
            //var instance = proAlloc as Conductor<object>;
            //Globals.ShowDialog(instance, (o) => { });

            GlobalsNAV.ShowDialog<IDrug_AddEdit>();

        }

        private void SupplierGenMedProduct_LoadDrugID(int PageIndex, int PageSize)
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            int totalCount = 0;
            IsLoading = true;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyGenMedDeptDetailsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginSupplierGenMedProduct_LoadDrugID(SupplierProduct.GenMedProductID, PageSize, PageIndex, true, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            var results = contract.EndSupplierGenMedProduct_LoadDrugID(out totalCount, asyncResult);
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
                                SupplierGenMedProduct_LoadDrugID(ListSupplierProduct.PageIndex, ListSupplierProduct.PageSize);
                            }
                            else if (result == 1)
                            {
                                MessageBox.Show(string.Format("{0}!", eHCMSResources.Z0682_G1_NCCDaTonTai));
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

        public void btnAddCC()
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
            else if (SupplierProduct.SupplierPriorityOrderNum == null || SupplierProduct.SupplierPriorityOrderNum == 0)
            {
                MessageBox.Show(string.Format("{0}!", eHCMSResources.A0200_G1_Msg_InfoYCNhapThuTuUuTien));
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
                                SupplierGenMedProduct_LoadDrugID(ListSupplierProduct.PageIndex, ListSupplierProduct.PageSize);
                            }
                            else if (result == 1)
                            {
                                MessageBox.Show(string.Format("{0}!", eHCMSResources.Z0682_G1_NCCDaTonTai));
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

        public void btnUpdateCC()
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
                MessageBox.Show(eHCMSResources.K0410_G1_ChonThuoc);
                OK = false;
            }
            else if (SupplierProduct.SelectedSupplier == null || SupplierProduct.SupplierID == 0)
            {
                MessageBox.Show(eHCMSResources.K0347_G1_ChonNCC);
                OK = false;
            }
            else if (SupplierProduct.SupplierPriorityOrderNum == null || SupplierProduct.SupplierPriorityOrderNum == 0)
            {
                MessageBox.Show(string.Format("{0}!", eHCMSResources.A0200_G1_Msg_InfoYCNhapThuTuUuTien));
                OK = false;
            }
            if (OK)
            {
                SupplierGenMedProduct_Update();
            }
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
                            SupplierGenMedProduct_LoadDrugID(ListSupplierProduct.PageIndex, ListSupplierProduct.PageSize);
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
            if (MessageBox.Show(string.Format(eHCMSResources.Z0557_G1_CoChacMuonXoa, eHCMSResources.N0037_G1_NCC), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                if (SupplierProduct != null)
                {
                    SupplierGenMedProduct_Delete();
                }
                else
                {
                    MessageBox.Show(string.Format("{0}!", eHCMSResources.Z0636_G1_KgCNhatDuoc));
                }
            }
        }

        #region Auto for Supplier
        private SupplierSearchCriteria _SupplierCriteria;
        public SupplierSearchCriteria SupplierCriteria
        {
            get
            {
                return _SupplierCriteria;
            }
            set
            {
                _SupplierCriteria = value;
                NotifyOfPropertyChange(() => SupplierCriteria);
            }
        }

        private PagedSortableCollectionView<Supplier> _Suppliers;
        public PagedSortableCollectionView<Supplier> Suppliers
        {
            get
            {
                return _Suppliers;
            }
            set
            {
                if (_Suppliers != value)
                {
                    _Suppliers = value;
                }
                NotifyOfPropertyChange(() => Suppliers);
            }
        }

        private void SearchSupplierAuto(int PageIndex, int PageSize)
        {
            int totalCount = 0;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacySuppliersServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginSearchSupplierAutoPaging(SupplierCriteria, PageSize, PageIndex, true, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            var ListUnits = contract.EndSearchSupplierAutoPaging(out totalCount, asyncResult);
                            if (ListUnits != null)
                            {
                                Suppliers.Clear();
                                Suppliers.TotalItemCount = totalCount;
                                foreach (Supplier p in ListUnits)
                                {
                                    Suppliers.Add(p);
                                }
                                NotifyOfPropertyChange(() => Suppliers);
                            }
                            au.ItemsSource = Suppliers;
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
            SupplierCriteria.SupplierName = e.Parameter;
            Suppliers.PageIndex = 0;
            SearchSupplierAuto(Suppliers.PageIndex, Suppliers.PageSize);
        }

        public void btnSupplier(object sender, RoutedEventArgs e)
        {
            //var proAlloc = Globals.GetViewModel<ISupplierProduct>();
            //proAlloc.IsChildWindow = true;
            //var instance = proAlloc as Conductor<object>;
            //Globals.ShowDialog(instance, (o) => { });

            Action<ISupplierProduct> onInit = (proAlloc) =>
            {
                proAlloc.IsChildWindow = true;
            };

            GlobalsNAV.ShowDialog<ISupplierProduct>(onInit, null, false, true, Globals.GetDefaultDialogViewSize());

        }
        #endregion

        #region IHandle<PharmacyCloseSearchDrugEvent> Members

        //public void Handle(DrugDeptCloseSearchDrugEvent message)
        //{
        //    if (message != null)
        //    {
        //        SupplierProduct.SelectedGenMedProduct = message.SupplierProduct as RefGenericDrugDetail;
        //        ListSupplierProduct.PageIndex = 0;
        //        SupplierGenMedProduct_LoadDrugID(ListSupplierProduct.PageIndex, ListSupplierProduct.PageSize);
        //    }
        //}

        #endregion

        #region IHandle<PharmacyCloseFinishAddGenDrugEvent> Members

        //public void Handle(DrugDeptCloseFinishAddGenDrugEvent message)
        //{
        //    if (message != null)
        //    {
        //        SupplierProduct.SelectedGenMedProduct = message.SupplierProduct as RefGenericDrugDetail;
        //        ListSupplierProduct.PageIndex = 0;
        //         SupplierGenMedProduct_LoadDrugID(ListSupplierProduct.PageIndex,ListSupplierProduct.PageSize);
        //    }
        //}

        #endregion

        #region IHandle<PharmacyCloseSearchSupplierEvent> Members

        public void Handle(DrugDeptCloseSearchSupplierEvent message)
        {
            if (message != null)
            {
                SupplierProduct.SelectedSupplier = message.SelectedSupplier as DrugDeptSupplier;
                ListSupplierProduct.PageIndex = 0;
                SupplierGenMedProduct_LoadDrugID(ListSupplierProduct.PageIndex, ListSupplierProduct.PageSize);
            }
        }

        #endregion

        #region IHandle<DrugDeptCloseSearchDrugEvent> Members

        public void Handle(DrugDeptCloseSearchDrugEvent message)
        {
            if (message != null)
            {
                SupplierProduct.SelectedGenMedProduct = message.SupplierDrug as RefGenMedProductDetails;
                ListSupplierProduct.PageIndex = 0;
                SupplierGenMedProduct_LoadDrugID(ListSupplierProduct.PageIndex, ListSupplierProduct.PageSize);
            }
        }

        #endregion

        public void LinkPrice_Click(object sender, RoutedEventArgs e)
        {
            //Active = true;
            //SupplierGenericDrugPrice p = new SupplierGenericDrugPrice();
            //var typeInfo = Globals.GetViewModel<ISupplierGenericDrugPrice_ListPrice>();
            //typeInfo.ObjSupplierCurrent = SupplierDrug.SelectedSupplier;
            //typeInfo.ObjDrugCurrent = new SupplierGenericDrugPrice();
            //typeInfo.ObjDrugCurrent.ObjRefGenericDrugDetail = SupplierDrug.SelectedGenericDrug;
            //typeInfo.ObjDrugCurrent.DrugID = SupplierDrug.DrugID;

            //typeInfo.Criteria = new SupplierGenericDrugPriceSearchCriteria();
            //typeInfo.Criteria.SupplierID = SupplierDrug.SupplierID;
            //typeInfo.Criteria.DrugID = SupplierDrug.SelectedGenericDrug.DrugID;
            //typeInfo.Criteria.PriceType = 1;//Giá hiện hành

            //var instance = typeInfo as Conductor<object>;

            //Globals.ShowDialog(instance, (o) => { });
        }

       
    }
}
