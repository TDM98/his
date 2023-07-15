using System.ComponentModel.Composition;
using aEMR.ViewContracts;
using Caliburn.Micro;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.Infrastructure.CachingUtils;
using Castle.Core.Logging;
using Castle.Windsor;
using aEMR.ServiceClient;
using System.Threading;
using DataEntities;
using aEMR .Common.Collections;
using System.Linq;
using eHCMSLanguage;

namespace aEMR.Pharmacy.ViewModels
{
    [Export(typeof(IAddSupplierForDrug)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class AddSupplierForDrugViewModel : Conductor<object>, IAddSupplierForDrug, IHandle<PharmacyCloseSearchDrugEvent>
        , IHandle<PharmacyCloseFinishAddGenDrugEvent>, IHandle<PharmacyCloseSearchSupplierEvent>
        , IHandle<ClosePriceEvent>
    {
        private bool Active = false;
        public object SuppliersSearchPaging
        {
            get;
            set;
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
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public AddSupplierForDrugViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            authorization();

            eventAggregator.Subscribe(this);

            _SupplierDrug = new SupplierGenericDrug();
            SearchCriteria = new DrugSearchCriteria();

            SupplierCriteria = new SupplierSearchCriteria();
            SupplierCriteria.V_SupplierType = V_SupplierType;

            ListSupplierDrug = new PagedSortableCollectionView<SupplierGenericDrug>();
            ListSupplierDrug.OnRefresh += ListSupplierDrug_OnRefresh;
            ListSupplierDrug.PageSize = Globals.PageSize;

            Suppliers = new PagedSortableCollectionView<Supplier>();
            Suppliers.OnRefresh += Suppliers_OnRefresh;
            Suppliers.PageSize = Globals.PageSize;

            Active = false;
        }
        void Suppliers_OnRefresh(object sender, RefreshEventArgs e)
        {
            SearchSupplierAuto(Suppliers.PageIndex, Suppliers.PageSize);
        }

        void ListSupplierDrug_OnRefresh(object sender, RefreshEventArgs e)
        {
            SupplierGenericDrug_LoadDrugID(ListSupplierDrug.PageIndex,ListSupplierDrug.PageSize);
        }

        public long V_SupplierType = 7200;
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

        private DrugSearchCriteria _searchCriteria;
        public DrugSearchCriteria SearchCriteria
        {
            get
            {
                return _searchCriteria;
            }
            set
            {
                _searchCriteria = value;
                NotifyOfPropertyChange(() => SearchCriteria);
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

        private SupplierGenericDrug _SupplierDrug;
        public SupplierGenericDrug SupplierDrug
        {
            get
            {
                return _SupplierDrug;
            }
            set
            {
                if (_SupplierDrug != value)
                {
                    _SupplierDrug = value;
                }
                NotifyOfPropertyChange(() => SupplierDrug);
            }
        }

        private PagedSortableCollectionView<SupplierGenericDrug> _ListSupplierDrug;
        public PagedSortableCollectionView<SupplierGenericDrug> ListSupplierDrug
        {
            get
            {
                return _ListSupplierDrug;
            }
            set
            {
                if (_ListSupplierDrug != value)
                {
                    _ListSupplierDrug = value;
                }
                NotifyOfPropertyChange(() => ListSupplierDrug);
            }
        }
        #endregion

        public void GridSuppliers_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString() + ". ";
        }

        private void SetObjectValueForSupplierDrug(object item)
        {
            SupplierGenericDrug p = item as SupplierGenericDrug;
            SupplierDrug.SelectedSupplier = p.SelectedSupplier;
            SupplierDrug.SupGenDrugID = p.SupGenDrugID;
            SupplierDrug.SupplierID = p.SupplierID;
            SupplierDrug.IsMain = p.IsMain;
            SupplierDrug.UnitPrice = p.UnitPrice;
        }

        public void GridSuppliers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid GridSuppliers = sender as DataGrid;
            if (GridSuppliers.SelectedItem != null)
            {
                SetObjectValueForSupplierDrug(GridSuppliers.SelectedItem);
            }
        }
       
        public void btnSearch(object sender, RoutedEventArgs e)
        {
            OpenChildWindow();
        }

        public void TextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                OpenChildWindow();
            }
        }

        public void SearchDrugs(int PageIndex, int PageSize)
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            int totalCount = 0;
            IsLoading = true;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyDrugServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginSearchDrugs(SearchCriteria, PageIndex, PageSize, true, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            var results = contract.EndSearchDrugs(out totalCount, asyncResult);
                            if (results != null && results.Count > 0)
                            {
                                if (results.Count > 1)
                                {
                                    //mo pop up tim
                                    //var proAlloc = Globals.GetViewModel<IRefGenDrugList>();
                                    //proAlloc.IsPopUp = true;
                                    //proAlloc.SearchCriteria = SearchCriteria;
                                    //proAlloc.DrugsResearch.Clear();
                                    //proAlloc.DrugsResearch.TotalItemCount = totalCount;
                                    //proAlloc.DrugsResearch.PageIndex = 0;
                                    //foreach (RefGenericDrugDetail p in results)
                                    //{
                                    //    proAlloc.DrugsResearch.Add(p);
                                    //}
                                    //var instance = proAlloc as Conductor<object>;
                                    //Globals.ShowDialog(instance, (o) => { });

                                    Action<IRefGenDrugList> onInitDlg = (proAlloc) =>
                                    {
                                        proAlloc.IsPopUp = true;
                                        proAlloc.SearchCriteria = SearchCriteria;
                                        proAlloc.DrugsResearch.Clear();
                                        proAlloc.DrugsResearch.TotalItemCount = totalCount;
                                        proAlloc.DrugsResearch.PageIndex = 0;
                                        foreach (RefGenericDrugDetail p in results)
                                        {
                                            proAlloc.DrugsResearch.Add(p);
                                        }
                                    };

                                    Action<IRefGenDrugListNew> onInitDlgNew = (proAlloc) =>
                                    {
                                        proAlloc.IsPopUp = true;
                                        proAlloc.SearchCriteria = SearchCriteria;
                                        proAlloc.DrugsResearch.Clear();
                                        proAlloc.DrugsResearch.TotalItemCount = totalCount;
                                        proAlloc.DrugsResearch.PageIndex = 0;
                                        foreach (RefGenericDrugDetail p in results)
                                        {
                                            proAlloc.DrugsResearch.Add(p);
                                        }
                                    };

                                    if (Globals.ServerConfigSection.CommonItems.EnableHIStore)
                                    {
                                        GlobalsNAV.ShowDialog<IRefGenDrugListNew>(onInitDlgNew);
                                    }
                                    else
                                    {
                                        GlobalsNAV.ShowDialog<IRefGenDrugList>(onInitDlg);
                                    }
                                    //GlobalsNAV.ShowDialog<IRefGenDrugList>(onInitDlg);
                                }
                                else
                                {
                                    SupplierDrug.SelectedGenericDrug = results.FirstOrDefault();
                                    ListSupplierDrug.PageIndex = 0;
                                    SupplierGenericDrug_LoadDrugID(ListSupplierDrug.PageIndex, ListSupplierDrug.PageSize);
                                }
                            }
                            else
                            {
                                MessageBox.Show(eHCMSResources.A0752_G1_Msg_InfoKhTimThay);
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

        private void OpenChildWindow()
        {
            SearchCriteria.BrandName = BrandName;
            SearchDrugs(0,Globals.PageSize);
        }

        public void btnAddNew(object senser, RoutedEventArgs e)
        {
            //var proAlloc = Globals.GetViewModel<IRefGenDrug_Add>();
            //proAlloc.IsAddFinishClosed = true;
            //var instance = proAlloc as Conductor<object>;
            //Globals.ShowDialog(instance, (o) => { });

            Action<IRefGenDrug_Add> onInitDlg = (proAlloc) =>
            {
                proAlloc.IsAddFinishClosed = true;
            };

            GlobalsNAV.ShowDialog<IRefGenDrug_Add>(onInitDlg);
        }

        public void SupplierGenericDrug_LoadDrugID(int PageIndex, int PageSize)
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            int totalCount = 0;
            IsLoading = true;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyGenMedDeptDetailsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginSupplierGenericDrug_LoadDrugID(SupplierDrug.DrugID, PageSize, PageIndex, true, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            var results = contract.EndSupplierGenericDrug_LoadDrugID(out totalCount, asyncResult);
                            if (results != null)
                            {

                                ListSupplierDrug.Clear();
                                ListSupplierDrug.TotalItemCount = totalCount;
                                foreach (SupplierGenericDrug p in results)
                                {
                                    ListSupplierDrug.Add(p);
                                }
                                NotifyOfPropertyChange(() => ListSupplierDrug);
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

        private void SupplierGenericDrug_Insert()
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            IsLoading = true;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyGenMedDeptDetailsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginSupplierGenericDrug_Insert(SupplierDrug, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            int result = contract.EndSupplierGenericDrug_Insert(asyncResult);
                            if (result == 0)
                            {
                                ListSupplierDrug.PageIndex = 0;
                                SupplierGenericDrug_LoadDrugID(ListSupplierDrug.PageIndex,ListSupplierDrug.PageSize);
                            }
                            else if (result == 1)
                            {
                                MessageBox.Show(eHCMSResources.A0828_G1_Msg_InfoNCCDaTonTai, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                            }
                            else if (result == 2)
                            {
                                MessageBox.Show(eHCMSResources.K0038_G1_ThuocDaCoNCCchinh, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
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
                            if (au != null)
                            {
                                au.Focus();
                            }
                        }

                    }), null);

                }

            });

            t.Start();
        }

        public void btnAddCC(object sender, RoutedEventArgs e)
        {
            //kiem tra
            //Neu thuoc nay voi nhaCC nay da co thi thong bao da ton tai
            bool OK = true;
            if (SupplierDrug.SelectedGenericDrug == null || SupplierDrug.DrugID == 0)
            {
                MessageBox.Show(eHCMSResources.K0410_G1_ChonThuoc);
                OK = false;
            }
            else if (SupplierDrug.SelectedSupplier == null || SupplierDrug.SupplierID == 0)
            {
                MessageBox.Show(eHCMSResources.K0347_G1_ChonNCC);
                OK = false;
            }
          
            if (OK)
            {
                SupplierGenericDrug_Insert();
            }
        }

        private void SupplierGenericDrug_Update()
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            IsLoading = true;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyGenMedDeptDetailsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginSupplierGenericDrug_Update(SupplierDrug, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            int result = contract.EndSupplierGenericDrug_Update(asyncResult);
                            if (result == 0)
                            {
                                ListSupplierDrug.PageIndex = 0;
                                SupplierGenericDrug_LoadDrugID(ListSupplierDrug.PageIndex, ListSupplierDrug.PageSize);
                            }
                            else if (result == 1)
                            {
                                MessageBox.Show(eHCMSResources.A0828_G1_Msg_InfoNCCDaTonTai, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                            }
                            else if (result == 2)
                            {
                                MessageBox.Show(eHCMSResources.K0038_G1_ThuocDaCoNCCchinh, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
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

        public void btnUpdateCC(object sender, RoutedEventArgs e)
        {
            //kiem tra
            //Neu thuoc nay voi nhaCC nay da co thi thong bao da ton tai
            bool OK = true;
            if (SupplierDrug == null || SupplierDrug.SupGenDrugID == 0)
            {
                MessageBox.Show(eHCMSResources.A1043_G1_Msg_InfoTTinKhCoTrongData);
                OK = false;
            }
            else if (SupplierDrug.SelectedGenericDrug == null || SupplierDrug.DrugID == 0)
            {
                MessageBox.Show(eHCMSResources.K0410_G1_ChonThuoc);
                OK = false;
            }
            else if (SupplierDrug.SelectedSupplier == null || SupplierDrug.SupplierID == 0)
            {
                MessageBox.Show(eHCMSResources.K0347_G1_ChonNCC);
                OK = false;
            }
            if (OK)
            {
                SupplierGenericDrug_Update();
            }
        }

        private void SupplierGenericDrug_Delete()
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            IsLoading = true;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyGenMedDeptDetailsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginSupplierGenericDrug_Delete(SupplierDrug.SupGenDrugID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            contract.EndSupplierGenericDrug_Delete(asyncResult);
                            ListSupplierDrug.PageIndex = 0;
                            SupplierGenericDrug_LoadDrugID(ListSupplierDrug.PageIndex,ListSupplierDrug.PageSize);
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
            if (MessageBox.Show(eHCMSResources.A0119_G1_Msg_ConfXoaNCC, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                if (SupplierDrug != null)
                {
                    SupplierGenericDrug_Delete();
                }
                else
                {
                    MessageBox.Show(eHCMSResources.Z0636_G1_KgCNhatDuoc);
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

        private void SearchSupplierAuto(int PageIndex,int PageSize)
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
            SearchSupplierAuto(Suppliers.PageIndex,Suppliers.PageSize);
        }

        public void btnSupplier(object sender, RoutedEventArgs e)
        {
            //var proAlloc = Globals.GetViewModel<ISuppliers>();
            //proAlloc.IsChildWindow = true;
            //var instance = proAlloc as Conductor<object>;
            //Globals.ShowDialog(instance, (o) => { });

            Action<ISuppliers> onInitDlg = (proAlloc) =>
            {
                proAlloc.IsChildWindow = true;
            };

            GlobalsNAV.ShowDialog<ISuppliers>(onInitDlg);
        }
        #endregion

        #region IHandle<PharmacyCloseSearchDrugEvent> Members

        public void Handle(PharmacyCloseSearchDrugEvent message)
        {
            if (message != null)
            {
                SupplierDrug.SelectedGenericDrug = message.SupplierDrug as RefGenericDrugDetail;
                ListSupplierDrug.PageIndex = 0;
                SupplierGenericDrug_LoadDrugID(ListSupplierDrug.PageIndex, ListSupplierDrug.PageSize);
            }
        }

        #endregion

        #region IHandle<PharmacyCloseFinishAddGenDrugEvent> Members

        public void Handle(PharmacyCloseFinishAddGenDrugEvent message)
        {
            if (message != null)
            {
                SupplierDrug.SelectedGenericDrug = message.SupplierDrug as RefGenericDrugDetail;
                ListSupplierDrug.PageIndex = 0;
                 SupplierGenericDrug_LoadDrugID(ListSupplierDrug.PageIndex,ListSupplierDrug.PageSize);
            }
        }

        #endregion

        #region IHandle<PharmacyCloseSearchSupplierEvent> Members

        public void Handle(PharmacyCloseSearchSupplierEvent message)
        {
            if (message != null)
            {
                SupplierDrug.SelectedSupplier = message.SelectedSupplier as Supplier;
                ListSupplierDrug.PageIndex = 0;
                SupplierGenericDrug_LoadDrugID(ListSupplierDrug.PageIndex, ListSupplierDrug.PageSize);
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

            Action<ISupplierGenericDrugPrice_ListPrice> onInitDlg = (typeInfo) =>
            {
                Active = true;
                SupplierGenericDrugPrice p = new SupplierGenericDrugPrice();
                typeInfo.ObjSupplierCurrent = SupplierDrug.SelectedSupplier;
                typeInfo.ObjDrugCurrent = new SupplierGenericDrugPrice();
                typeInfo.ObjDrugCurrent.ObjRefGenericDrugDetail = SupplierDrug.SelectedGenericDrug;
                typeInfo.ObjDrugCurrent.DrugID = SupplierDrug.DrugID;

                typeInfo.Criteria = new SupplierGenericDrugPriceSearchCriteria();
                typeInfo.Criteria.SupplierID = SupplierDrug.SupplierID;
                typeInfo.Criteria.DrugID = SupplierDrug.SelectedGenericDrug.DrugID;
                typeInfo.Criteria.PriceType = 1;//Giá hiện hành
            };

            GlobalsNAV.ShowDialog<ISupplierGenericDrugPrice_ListPrice>(onInitDlg);

        }

        #region IHandle<ClosePriceEvent> Members

        public void Handle(ClosePriceEvent message)
        {
            if (this.Active)
            {
                this.Active = false;
                SupplierGenericDrug_LoadDrugID(0, ListSupplierDrug.PageSize);

            }
        }

        #endregion
    }
}
