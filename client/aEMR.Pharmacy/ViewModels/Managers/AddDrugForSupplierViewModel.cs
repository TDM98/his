using System.ComponentModel.Composition;
using aEMR.ViewContracts;
using Caliburn.Micro;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System;
using aEMR.Infrastructure;
using System.Collections.ObjectModel;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.Infrastructure.CachingUtils;
using Castle.Core.Logging;
using Castle.Windsor;
using System.Threading;
using DataEntities;
using aEMR.Common.Collections;
using System.Linq;
using aEMR.Common;
using eHCMSLanguage;

namespace aEMR.Pharmacy.ViewModels
{
    [Export(typeof(IAddDrugForSupplier)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class AddDrugForSupplierViewModel : Conductor<object>, IAddDrugForSupplier
         , IHandle<PharmacyCloseFinishAddSupplierEvent>, IHandle<PharmacyCloseSearchSupplierEvent>
         , IHandle<ClosePriceEvent>, IHandle<PharmacyCloseSearchDrugEvent>
    {
        private bool Active = false;

        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public AddDrugForSupplierViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            authorization();
            SupplierCriteria = new SupplierSearchCriteria();
            SupplierCriteria.V_SupplierType = GetCurrentSupplierType();

            ListSupplierDrug = new PagedSortableCollectionView<SupplierGenericDrug>();
            ListSupplierDrug.OnRefresh += ListSupplierDrug_OnRefresh;
            ListSupplierDrug.PageSize = Globals.PageSize;

            RefGenericDrugDetails = new PagedSortableCollectionView<RefGenericDrugDetail>();
            RefGenericDrugDetails.OnRefresh += RefGenericDrugDetails_OnRefresh;
            RefGenericDrugDetails.PageSize = Globals.PageSize;

            _SupplierDrug = new SupplierGenericDrug();
            Active = false;

            eventAggregator.Subscribe(this);
        }

        void RefGenericDrugDetails_OnRefresh(object sender, RefreshEventArgs e)
        {
            SearchRefDrugGenericDetails_AutoPaging(BrandName, RefGenericDrugDetails.PageIndex, RefGenericDrugDetails.PageSize);
        }

        void ListSupplierDrug_OnRefresh(object sender, RefreshEventArgs e)
        {
            SupplierGenericDrug_LoadSupplierID(ListSupplierDrug.PageIndex, ListSupplierDrug.PageSize);
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

        private string BrandName;

        private PagedSortableCollectionView<RefGenericDrugDetail> _RefGenericDrugDetails;
        public PagedSortableCollectionView<RefGenericDrugDetail> RefGenericDrugDetails
        {
            get
            {
                return _RefGenericDrugDetails;
            }
            set
            {
                if (_RefGenericDrugDetails != value)
                {
                    _RefGenericDrugDetails = value;
                }
                NotifyOfPropertyChange(() => RefGenericDrugDetails);
            }
        }

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

        private int _SupplierType;
        public int SupplierType
        {
            get { return _SupplierType; }
            set
            {
                if (_SupplierType != value)
                {
                    _SupplierType = value;
                    NotifyOfPropertyChange(() => SupplierType);
                }
            }
        }
        #endregion
        private int GetCurrentSupplierType()
        {
            if (Enum.IsDefined(typeof(AllLookupValues.SupplierType), SupplierType))
            {
                return (int)SupplierType;
            }
            else
            {
                return (int)AllLookupValues.SupplierType.CUNG_CAP_THIET_BI_Y_TE;
            }
        }
        #region Auto for Drug Member
        private void SearchRefDrugGenericDetails_AutoPaging(string Name, int PageIndex, int PageSize)
        {
            int totalCount = 0;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyDrugServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginSearchRefDrugGenericDetails_AutoPaging(null,Name, 0, PageIndex, PageSize, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            var ListUnits = contract.EndSearchRefDrugGenericDetails_AutoPaging(out totalCount, asyncResult);
                            if (ListUnits != null)
                            {
                                RefGenericDrugDetails.Clear();
                                RefGenericDrugDetails.TotalItemCount = totalCount;
                                RefGenericDrugDetails.ItemCount = totalCount;
                                foreach (RefGenericDrugDetail p in ListUnits)
                                {
                                    RefGenericDrugDetails.Add(p);
                                }
                                NotifyOfPropertyChange(() => RefGenericDrugDetails);
                            }
                            au.ItemsSource = RefGenericDrugDetails;
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
            RefGenericDrugDetails.PageIndex = 0;
            SearchRefDrugGenericDetails_AutoPaging(e.Parameter, 0, RefGenericDrugDetails.PageSize);
        }

        public void btnSearchDrug()
        {
            //var proAlloc = Globals.GetViewModel<IRefGenDrugList>();
            //proAlloc.IsPopUp = true;
            //var instance = proAlloc as Conductor<object>;
            //Globals.ShowDialog(instance, (o) => { });

            Action<IRefGenDrugList> onInitDlg = (proAlloc) =>
            {
                proAlloc.IsPopUp = true;
            };

            Action<IRefGenDrugListNew> onInitDlgNew = (proAlloc) =>
            {
                proAlloc.IsPopUp = true;
            };

            if (Globals.ServerConfigSection.CommonItems.EnableHIStore)
            {
                GlobalsNAV.ShowDialog<IRefGenDrugListNew>(onInitDlgNew);
            }
            else
            {
                GlobalsNAV.ShowDialog<IRefGenDrugList>(onInitDlg);
            }
            //GlobalsNAV.ShowDialog<IRefGenDrugList>(onInitDlg
        }
        #endregion

        public void btnAddNew(object sender, RoutedEventArgs e)
        {
            //var proAlloc = Globals.GetViewModel<ISupplier_Add>();
            //proAlloc.IsAddFinishClosed = true;
            //var instance = proAlloc as Conductor<object>;
            //Globals.ShowDialog(instance, (o) => { });

            Action<ISupplier_Add> onInitDlg = (proAlloc) =>
            {
                proAlloc.IsAddFinishClosed = true;
            };
            GlobalsNAV.ShowDialog<ISupplier_Add>(onInitDlg);
        }


        public void btnSearch(object sender, RoutedEventArgs e)
        {
            OpenPopUpSearchSupplier();
        }

        private void SearchSuppliers(int PageIndex, int PageSize)
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            int totalCount;
            IsLoading = true;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacySuppliersServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginSearchSupplierAutoPaging(SupplierCriteria, PageSize, PageIndex, true, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndSearchSupplierAutoPaging(out totalCount, asyncResult);
                            if (results != null && results.Count > 0)
                            {
                                if (results.Count > 1)
                                {
                                    //mo popup tim
                                    //var proAlloc = Globals.GetViewModel<ISuppliers>();
                                    //proAlloc.IsChildWindow = true;
                                    //proAlloc.SupplierCriteria = SupplierCriteria.DeepCopy();
                                    //proAlloc.Suppliers.Clear();
                                    //proAlloc.Suppliers.TotalItemCount = totalCount;
                                    //proAlloc.Suppliers.PageIndex = 0;
                                    //foreach (Supplier p in results)
                                    //{
                                    //    proAlloc.Suppliers.Add(p);
                                    //}
                                    //var instance = proAlloc as Conductor<object>;
                                    //Globals.ShowDialog(instance, (o) => { });

                                    Action<ISuppliers> onInitDlg = (proAlloc) =>
                                    {
                                        proAlloc.IsChildWindow = true;
                                        proAlloc.SupplierCriteria = SupplierCriteria.DeepCopy();
                                        proAlloc.Suppliers.Clear();
                                        proAlloc.Suppliers.TotalItemCount = totalCount;
                                        proAlloc.Suppliers.PageIndex = 0;
                                        foreach (Supplier p in results)
                                        {
                                            proAlloc.Suppliers.Add(p);
                                        }
                                    };
                                    GlobalsNAV.ShowDialog<ISuppliers>(onInitDlg);
                                }
                                else
                                {
                                    SupplierDrug.SelectedSupplier = results.FirstOrDefault();
                                    ListSupplierDrug.PageIndex = 0;
                                    SupplierGenericDrug_LoadSupplierID(0, ListSupplierDrug.PageSize);
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

        private void OpenPopUpSearchSupplier()
        {
            SearchSuppliers(0, Globals.PageSize);
        }

        public void TextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SupplierCriteria.SupplierName = (sender as TextBox).Text;
                OpenPopUpSearchSupplier();
            }
        }
        private void SupplierGenericDrug_LoadSupplierID(int PageIndex, int PageSize)
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            int totalCount = 0;
            IsLoading = false;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyGenMedDeptDetailsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginSupplierGenericDrug_LoadSupplierID(SupplierDrug.SupplierID, PageSize, PageIndex, true, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            var results = contract.EndSupplierGenericDrug_LoadSupplierID(out totalCount, asyncResult);
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
                            MessageBox.Show(eHCMSResources.K0537_G1_XoaOk);
                            ListSupplierDrug.PageIndex = 0;
                            SupplierGenericDrug_LoadSupplierID(0, ListSupplierDrug.PageSize);
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

        public void GridSuppliers_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString() + ". ";
        }

        private void SetObjectValueForDrug(object item)
        {
            SupplierGenericDrug p = item as SupplierGenericDrug;
            SupplierDrug.SelectedGenericDrug = p.SelectedGenericDrug;
            SupplierDrug.SupGenDrugID = p.SupGenDrugID;
            SupplierDrug.SupplierID = p.SupplierID;
            SupplierDrug.IsMain = p.IsMain;
        }
        public void GridSuppliers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid GridSuppliers = sender as DataGrid;
            if (GridSuppliers.SelectedItem != null)
            {
                SetObjectValueForDrug(GridSuppliers.SelectedItem);
            }
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
                                SupplierGenericDrug_LoadSupplierID(0, ListSupplierDrug.PageSize);
                            }
                            else if (result == 1)
                            {
                                MessageBox.Show(eHCMSResources.K0042_G1_ThuocDaCoTrongDMucNCC, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
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
                                SupplierGenericDrug_LoadSupplierID(0, ListSupplierDrug.PageSize);
                            }
                            else if (result == 1)
                            {
                                MessageBox.Show(eHCMSResources.K0042_G1_ThuocDaCoTrongDMucNCC, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
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
                MessageBox.Show(eHCMSResources.Z0676_G1_TTinKgCoTrongDLieu);
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

        #region IHandle<PharmacyCloseFinishAddSupplierEvent> Members

        public void Handle(PharmacyCloseFinishAddSupplierEvent message)
        {
            if (message != null)
            {
                SupplierDrug.SelectedSupplier = message.SelectedSupplier as Supplier;
                ListSupplierDrug.PageIndex = 0;
                SupplierGenericDrug_LoadSupplierID(0, ListSupplierDrug.PageSize);
            }
        }

        #endregion

        #region IHandle<PharmacyCloseSearchSupplierEvent> Members

        public void Handle(PharmacyCloseSearchSupplierEvent message)
        {
            if (message != null && this.IsActive)
            {
                SupplierDrug.SelectedSupplier = message.SelectedSupplier as Supplier;
                ListSupplierDrug.PageIndex = 0;
                SupplierGenericDrug_LoadSupplierID(0, ListSupplierDrug.PageSize);
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

            //Globals.ShowDialog(instance, (o) =>{});

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
                Active = false;
                SupplierGenericDrug_LoadSupplierID(0, ListSupplierDrug.PageSize);
            }
        }

        #endregion

        #region IHandle<PharmacyCloseSearchDrugEvent> Members

        public void Handle(PharmacyCloseSearchDrugEvent message)
        {
            if (message != null)
            {
                SupplierDrug.SelectedGenericDrug = message.SupplierDrug as RefGenericDrugDetail;
            }
        }

        #endregion
    }
}
