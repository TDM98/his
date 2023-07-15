using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Linq;
using System.Collections.Generic;
using aEMR.Common;
using aEMR.Controls;
using aEMR.CommonTasks;
using aEMR.Common.Collections;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.Infrastructure.CachingUtils;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using Castle.Core.Logging;
using Castle.Windsor;
using DataEntities;
using Caliburn.Micro;
using eHCMSLanguage;

namespace aEMR.DrugDept.ViewModels
{
    [Export(typeof(IInwardListCost)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class InwardListCostViewModel : Conductor<object>, IInwardListCost
        , IHandle<DrugDeptCloseSearchSupplierEvent>
        , IHandle<DrugDeptCloseSearchInwardCostListEvent>
        , IHandle<DrugDeptCloseSearchInwardIncoiceEvent>
    {

        #region Indicator Member
        private bool _isLoadingCurrency = false;
        public bool isLoadingCurrency
        {
            get { return _isLoadingCurrency; }
            set
            {
                if (_isLoadingCurrency != value)
                {
                    _isLoadingCurrency = value;
                    NotifyOfPropertyChange(() => isLoadingCurrency);
                    NotifyOfPropertyChange(() => IsLoading);
                }
            }
        }

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
            get { return (isLoadingCurrency || isLoadingFullOperator || isLoadingGetID || isLoadingSearch); }
        }

        #endregion
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public InwardListCostViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            eventAggregator.Subscribe(this);
            SupplierCriteria = new SupplierSearchCriteria();
            SupplierCriteria.V_SupplierType = (long)AllLookupValues.SupplierType.CUNG_CAP_THIET_BI_Y_TE;

            SearchCriteria = new InwardInvoiceSearchCriteria();
            SearchCriteriaInvoice = new InwardInvoiceSearchCriteria();

            _CurrentCostTableMedDeptList = new CostTableMedDeptList();

            InitCostTableMedDept();
            GetAllCurrency(true);

            Suppliers = new PagedSortableCollectionView<DrugDeptSupplier>();
            Suppliers.OnRefresh += new EventHandler<RefreshEventArgs>(Suppliers_OnRefresh);
            Suppliers.PageSize = Globals.PageSize;
        }

        private void InitCostTableMedDept()
        {
            CurrentCostTableMedDept = null;
            CurrentCostTableMedDept = new CostTableMedDept();
            CurrentCostTableMedDept.InvoiceDate = Globals.ServerDate.Value;
            try
            {
                CurrentCostTableMedDept.StaffID = Globals.LoggedUserAccount.StaffID.GetValueOrDefault();
            }
            catch { }
            if (Currencies != null)
            {
                CurrentCostTableMedDept.SelectedCurrency = Currencies.FirstOrDefault();
            }
            InitCurrentCostTableForMedDeptInvoice();
        }
        void Suppliers_OnRefresh(object sender, RefreshEventArgs e)
        {
            SearchSupplierAuto(Suppliers.PageIndex, Suppliers.PageSize);
        }
        #region Properties Member

        private string _strHienThi;
        public string strHienThi
        {
            get
            {
                return _strHienThi;
            }
            set
            {
                _strHienThi = value;
                NotifyOfPropertyChange(() => strHienThi);
            }
        }

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


        private IList<Currency> _Currencies;
        public IList<Currency> Currencies
        {
            get
            {
                return _Currencies;
            }
            set
            {
                if (_Currencies != value)
                {
                    _Currencies = value;
                    NotifyOfPropertyChange(() => Currencies);
                }
            }
        }

        private CostTableMedDept _CurrentCostTableMedDept;
        public CostTableMedDept CurrentCostTableMedDept
        {
            get
            {
                return _CurrentCostTableMedDept;
            }
            set
            {
                if (_CurrentCostTableMedDept != value)
                {
                    _CurrentCostTableMedDept = value;
                    NotifyOfPropertyChange(() => CurrentCostTableMedDept);
                }

            }
        }

        private CostTableMedDeptList _CurrentCostTableMedDeptList;
        public CostTableMedDeptList CurrentCostTableMedDeptList
        {
            get
            {
                return _CurrentCostTableMedDeptList;
            }
            set
            {
                if (_CurrentCostTableMedDeptList != value)
                {
                    _CurrentCostTableMedDeptList = value;
                    NotifyOfPropertyChange(() => CurrentCostTableMedDeptList);
                }

            }
        }


        private bool _isEnableExchangeRate;
        public bool IsEnableExchangeRate
        {
            get
            {
                return _isEnableExchangeRate;
            }
            set
            {
                if (_isEnableExchangeRate != value)
                {
                    _isEnableExchangeRate = value;
                    NotifyOfPropertyChange(() => IsEnableExchangeRate);
                }
            }
        }

        #endregion

        #region check invisible

        private bool _mTim = true;
        private bool _mIn = true;
        private bool _mChinhSua_Them = true;

        public bool mTim
        {
            get
            {
                return _mTim;
            }
            set
            {
                if (_mTim == value)
                    return;
                _mTim = value;
                NotifyOfPropertyChange(() => mTim);
            }
        }
        public bool mIn
        {
            get
            {
                return _mIn;
            }
            set
            {
                if (_mIn == value)
                    return;
                _mIn = value;
                NotifyOfPropertyChange(() => mIn);
            }
        }
        public bool mChinhSua_Them
        {
            get
            {
                return _mChinhSua_Them;
            }
            set
            {
                if (_mChinhSua_Them == value)
                    return;
                _mChinhSua_Them = value;
                NotifyOfPropertyChange(() => mChinhSua_Them);
            }
        }
        #endregion

        private void GetAllCurrency(bool value)
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            isLoadingCurrency = true;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new CommonService_V2Client())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetAllCurrency(value, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            Currencies = contract.EndGetAllCurrency(asyncResult);
                            CurrentCostTableMedDept.SelectedCurrency = Currencies.FirstOrDefault();
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            isLoadingCurrency = false;
                            //Globals.IsBusy = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }

        DataGrid grdInvoices = null;
        public void grdInvoices_Loaded(object sender, RoutedEventArgs e)
        {
            grdInvoices = sender as DataGrid;
        }

        public void lnkDeleteInvoice_Click(object sender, RoutedEventArgs e)
        {
            if (grdInvoices != null)
            {
                CurrentCostTableMedDept.InwardDrugMedDeptInvoices.RemoveAt(grdInvoices.SelectedIndex);
            }
        }

        public void AddItem()
        {
            if (CurrentCostTableMedDept != null && CurrentCostTableMedDeptList != null)
            {
                if (string.IsNullOrEmpty(CurrentCostTableMedDeptList.CoListName))
                {
                    Globals.ShowMessage(string.Format("{0}!", eHCMSResources.Z0625_G1_ChuaNhapTenCPhi), eHCMSResources.G0442_G1_TBao);
                    return;
                }
                if (CurrentCostTableMedDeptList.TotalValue <= 0)
                {
                    Globals.ShowMessage(string.Format("{0}!", eHCMSResources.Z0671_G1_GTriKgHopLe), eHCMSResources.G0442_G1_TBao);
                    return;
                }


                if (CurrentCostTableMedDept.CostTableMedDeptLists == null)
                {
                    CurrentCostTableMedDept.CostTableMedDeptLists = new ObservableCollection<CostTableMedDeptList>();
                }
                CostTableMedDeptList temp = CurrentCostTableMedDeptList.DeepCopy();
                var CheckExist = CurrentCostTableMedDept.CostTableMedDeptLists.Where(x => x.CoListName == temp.CoListName);
                if (CheckExist != null && CheckExist.Count() > 0)
                {
                    if (MessageBox.Show(eHCMSResources.A0292_G1_Msg_ConfCoMuonCNhat, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                    {
                        CheckExist.ToList()[0].CoListNotes = temp.CoListNotes;
                        CheckExist.ToList()[0].TotalValue = temp.TotalValue;
                        InitCurrentCostTableForMedDeptInvoice();
                        SumTotal();
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    CurrentCostTableMedDept.CostTableMedDeptLists.Add(temp);
                    InitCurrentCostTableForMedDeptInvoice();

                    SumTotal();
                }
            }
            else
            {
                Globals.ShowMessage(string.Format("{0}!", eHCMSResources.T0074_G1_I), eHCMSResources.G0442_G1_TBao);
            }
        }

        private void InitCurrentCostTableForMedDeptInvoice()
        {
            if (CurrentCostTableMedDeptList != null)
            {
                CurrentCostTableMedDeptList.CoListName = "";
                CurrentCostTableMedDeptList.CoListNotes = "";
                CurrentCostTableMedDeptList.TotalValue = 0;
            }
        }

        DataGrid grdCostList = null;
        public void grdCostList_Loaded(object sender, RoutedEventArgs e)
        {
            grdCostList = sender as DataGrid;
        }
        public void lnkDelete_Click(object sender, RoutedEventArgs e)
        {
            if (grdCostList != null)
            {
                CurrentCostTableMedDept.CostTableMedDeptLists.RemoveAt(grdCostList.SelectedIndex);

                SumTotal();
            }
        }

        #region Auto Suppliers

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

        private PagedSortableCollectionView<DrugDeptSupplier> _Suppliers;
        public PagedSortableCollectionView<DrugDeptSupplier> Suppliers
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
                    contract.BeginDrugDeptSupplier_SearchAutoPaging(SupplierCriteria, PageSize, PageIndex, true, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            var ListUnits = contract.EndDrugDeptSupplier_SearchAutoPaging(out totalCount, asyncResult);
                            // Suppliers = ListUnits.ToObservableCollection();
                            if (ListUnits != null)
                            {
                                Suppliers.Clear();
                                Suppliers.TotalItemCount = totalCount;
                                foreach (DrugDeptSupplier p in ListUnits)
                                {
                                    Suppliers.Add(p);
                                }
                                NotifyOfPropertyChange(() => Suppliers);
                            }
                            auSupplier.ItemsSource = Suppliers;
                            auSupplier.PopulateComplete();
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

        AxAutoComplete auSupplier;
        public void AutoSuppliers_Populating(object sender, PopulatingEventArgs e)
        {
            auSupplier = sender as AxAutoComplete;
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

            void onInitDlg(ISupplierProduct proAlloc)
            {
                proAlloc.IsChildWindow = true;
            }
            GlobalsNAV.ShowDialog<ISupplierProduct>(onInitDlg, null, false, true, Globals.GetDefaultDialogViewSize());
        }

        #endregion

        #region IHandle<DrugDeptCloseSearchSupplierEvent> Members

        public void Handle(DrugDeptCloseSearchSupplierEvent message)
        {
            if (message != null && this.IsActive)
            {
                CurrentCostTableMedDept.SelectedSupplier = message.SelectedSupplier as DrugDeptSupplier;
            }
        }

        #endregion

        private void SumTotal()
        {
            if (CurrentCostTableMedDept != null && CurrentCostTableMedDept.CostTableMedDeptLists != null)
            {
                CurrentCostTableMedDept.TotalNotVAT = 0;
                CurrentCostTableMedDept.TotalVAT = 0;
                CurrentCostTableMedDept.VATValue = 0;
                for (int i = 0; i < CurrentCostTableMedDept.CostTableMedDeptLists.Count; i++)
                {
                    CurrentCostTableMedDept.TotalNotVAT += CurrentCostTableMedDept.CostTableMedDeptLists[i].TotalValue;
                }
            }
        }

        public void btnNew()
        {
            InitCostTableMedDept();
        }


        WarningWithConfirmMsgBoxTask confirmBeforeDischarge = null;

        public IEnumerator<IResult> Coroutine_Save()
        {
            if (CurrentCostTableMedDept == null)
            {
                yield break;
            }
            if (CurrentCostTableMedDept.CostTableMedDeptLists == null || CurrentCostTableMedDept.CostTableMedDeptLists.Count <= 0)
            {
                MessageBox.Show(eHCMSResources.K0450_G1_NhapTTinTruocKhiLuu, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                yield break;
            }

            //if (CurrentCostTableMedDept.InvoiceDate.Year < (DateTime.Now.Year - 50))
            if (CurrentCostTableMedDept.InvoiceDate.Date > Globals.ServerDate.Value.Date)
            {
                MessageBox.Show(eHCMSResources.A0843_G1_Msg_InfoNgHDKhHopLe, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                yield break;
            }
            if (CurrentCostTableMedDept.ExchangeRates < 0)
            {
                MessageBox.Show(eHCMSResources.K0251_G1_TyGiaKhongHopLe, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                yield break;
            }
            if (CurrentCostTableMedDept.VAT < 0 && CurrentCostTableMedDept.VAT > 2)
            {
                MessageBox.Show(eHCMSResources.K0262_G1_VATKhongHopLe, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                yield break;
            }
            if (CurrentCostTableMedDept.SelectedCurrency != null)
            {
                if (CurrentCostTableMedDept.SelectedCurrency.CurrencyID == (long)AllLookupValues.CurrencyTable.VND && CurrentCostTableMedDept.ExchangeRates > 0)
                {
                    MessageBox.Show(eHCMSResources.A0521_G1_Msg_InfoKhDcDatTyGia, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                    yield break;
                }
                CurrentCostTableMedDept.CurrencyID = CurrentCostTableMedDept.SelectedCurrency.CurrencyID;
            }

            if (CurrentCostTableMedDept.InwardDrugMedDeptInvoices == null || CurrentCostTableMedDept.InwardDrugMedDeptInvoices.Count <= 0)
            {
                MessageBox.Show(eHCMSResources.A0100_G1_Msg_InfoChuaChonPhNhap, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                yield break;
            }
            //if (CurrentCostTableMedDept.SelectedSupplier == null)
            //{
            //    Globals.ShowMessage(string.Format("{0}!", eHCMSResources.Z0817_G1_ChonNCC), eHCMSResources.G0442_G1_TBao);
            //    return;
            //}
            if (CurrentCostTableMedDept.SelectedSupplier != null)
            {
                CurrentCostTableMedDept.SupplierID = CurrentCostTableMedDept.SelectedSupplier.SupplierID;
            }

            CurrentCostTableMedDept.V_MedProductType = V_MedProductType;

            if (string.IsNullOrWhiteSpace(CurrentCostTableMedDept.InvoiceNumber))
            {
                confirmBeforeDischarge = new WarningWithConfirmMsgBoxTask(eHCMSResources.Z0626_G1_ChuaNhapMaHD, eHCMSResources.Z0627_G1_TiepTucLuu);
                yield return confirmBeforeDischarge;
                if (!confirmBeforeDischarge.IsAccept)
                {
                    confirmBeforeDischarge = null;
                    yield break;
                }
                confirmBeforeDischarge = null;
            }

            CostTable_Save();

        }

        public void btnSave()
        {
            Coroutine.BeginExecute(Coroutine_Save());
        }

        private void CostTable_Save()
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            //isLoadingFullOperator = true;
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyMedDeptServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginCostTableMedDept_Insert(CurrentCostTableMedDept, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            long id = 0;
                            string StrCoNumber = "";
                            var results = contract.EndCostTableMedDept_Insert(out id, out StrCoNumber, asyncResult);
                            
                            MessageBox.Show(string.Format("{0}!", eHCMSResources.A0468_G1_Msg_InfoLuuOK), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);

                            if (id > 0)
                            {
                                CurrentCostTableMedDept.CoID = id;
                                CurrentCostTableMedDept.CoNumber = StrCoNumber;
                            }
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            //isLoadingFullOperator = false;
                            //Globals.IsBusy = false;
                            this.HideBusyIndicator();
                        }

                    }), null);

                }

            });

            t.Start();
        }

        private void CostTableList_Load(long CoID)
        {
            // Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            isLoadingGetID = true;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyMedDeptServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginCostTableMedDeptDetails_ByID(CoID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndCostTableMedDeptDetails_ByID(asyncResult);
                            CurrentCostTableMedDept.CostTableMedDeptLists = results.CostTableMedDeptLists;
                            CurrentCostTableMedDept.InwardDrugMedDeptInvoices = results.InwardDrugMedDeptInvoices;

                            SumTotal();
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            isLoadingGetID = false;
                            //Globals.IsBusy = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }

        #region Search Chi Phí Member

        private InwardInvoiceSearchCriteria _searchCriteria;
        public InwardInvoiceSearchCriteria SearchCriteria
        {
            get
            {
                return _searchCriteria;
            }
            set
            {
                if (_searchCriteria != value)
                {
                    _searchCriteria = value;
                    NotifyOfPropertyChange(() => SearchCriteria);
                }
            }
        }

        private void InnitSearchCriteria()
        {
            if (SearchCriteria == null)
            {
                SearchCriteria = new InwardInvoiceSearchCriteria();
            }
            SearchCriteria.InvoiceNumber = "";
            SearchCriteria.InwardID = "";
            SearchCriteria.FromDate = null;
            SearchCriteria.ToDate = null;
            SearchCriteria.DateInvoice = null;
            SearchCriteria.SupplierID = 0;
        }

        public void OpenPopUpSearchInwardListCostInvoice(IList<CostTableMedDept> results, int Totalcount)
        {
            //var proAlloc = Globals.GetViewModel<IInwardListCostSearch>();
            //proAlloc.SearchCriteria = SearchCriteria.DeepCopy();
            //proAlloc.V_MedProductType = V_MedProductType;
            //if (V_MedProductType == (long)AllLookupValues.MedProductType.THUOC)
            //{
            //    proAlloc.strHienThi = eHCMSResources.Z0672_G1_TimHDonNhapCPhiThuoc.ToUpper();
            //}
            //else if (V_MedProductType == (long)AllLookupValues.MedProductType.Y_CU)
            //{
            //    proAlloc.strHienThi = eHCMSResources.Z0673_G1_TimHDonNhapCPhiYCu.ToUpper();
            //}
            //else
            //{
            //    proAlloc.strHienThi = eHCMSResources.Z0674_G1_TimHDonNhapCPhiHChat.ToUpper();
            //}
            //proAlloc.InwardInvoiceList.Clear();
            //proAlloc.InwardInvoiceList.TotalItemCount = Totalcount;
            //proAlloc.InwardInvoiceList.PageIndex = 0;
            //if (results != null && results.Count > 0)
            //{
            //    foreach (CostTableMedDept p in results)
            //    {
            //        proAlloc.InwardInvoiceList.Add(p);
            //    }
            //}
            //var instance = proAlloc as Conductor<object>;
            //Globals.ShowDialog(instance, (o) => { });

            Action<IInwardListCostSearch> onInitDlg = (proAlloc) =>
            {
                proAlloc.SearchCriteria = SearchCriteria.DeepCopy();
                proAlloc.V_MedProductType = V_MedProductType;
                if (V_MedProductType == (long)AllLookupValues.MedProductType.THUOC)
                {
                    proAlloc.strHienThi = eHCMSResources.Z0672_G1_TimHDonNhapCPhiThuoc.ToUpper();
                }
                else if (V_MedProductType == (long)AllLookupValues.MedProductType.Y_CU)
                {
                    proAlloc.strHienThi = eHCMSResources.Z0673_G1_TimHDonNhapCPhiYCu.ToUpper();
                }
                else
                {
                    proAlloc.strHienThi = eHCMSResources.Z0674_G1_TimHDonNhapCPhiHChat.ToUpper();
                }
                proAlloc.InwardInvoiceList.Clear();
                proAlloc.InwardInvoiceList.TotalItemCount = Totalcount;
                proAlloc.InwardInvoiceList.PageIndex = 0;
                if (results != null && results.Count > 0)
                {
                    foreach (CostTableMedDept p in results)
                    {
                        proAlloc.InwardInvoiceList.Add(p);
                    }
                }
            };
            GlobalsNAV.ShowDialog<IInwardListCostSearch>(onInitDlg);

        }

        private void SearchInwardInvoiceDrug(int PageIndex, int PageSize)
        {
            if (SearchCriteria == null)
            {
                return;
            }

            isLoadingSearch = true;
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });

            if (string.IsNullOrEmpty(SearchCriteria.InwardID) && string.IsNullOrEmpty(SearchCriteria.InvoiceNumber))
            {
                SearchCriteria.FromDate = Globals.GetCurServerDateTime().AddDays(-7);
                SearchCriteria.ToDate = Globals.GetCurServerDateTime();
            }
            else
            {
                SearchCriteria.FromDate = null;
                SearchCriteria.ToDate = null;
            }


            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyMedDeptServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginCostTableMedDept_Search(SearchCriteria, V_MedProductType, PageIndex, PageSize, true, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            int Totalcount;
                            var results = contract.EndCostTableMedDept_Search(out Totalcount, asyncResult);
                            if (results != null && results.Count > 0)
                            {
                                if (results.Count > 1)
                                {
                                    //mo pop up tim
                                    OpenPopUpSearchInwardListCostInvoice(results.ToList(), Totalcount);
                                }
                                else
                                {
                                    CurrentCostTableMedDept = results.FirstOrDefault();
                                    CostTableList_Load(CurrentCostTableMedDept.CoID);
                                }
                            }
                            else
                            {
                                if (MessageBox.Show(eHCMSResources.Z0568_G1_KgTimThay, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                                {
                                    SearchCriteria.FromDate = Globals.GetCurServerDateTime().AddDays(-7);
                                    SearchCriteria.ToDate = Globals.GetCurServerDateTime();
                                    OpenPopUpSearchInwardListCostInvoice(null, 0);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            isLoadingSearch = false;
                            //Globals.IsBusy = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }

        public void tbx_SearchByInvoiceNumber_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (SearchCriteria != null)
                {
                    SearchCriteria.InvoiceNumber = (sender as TextBox).Text;
                }
                btnSearch();

            }
        }

        public void tbx_SearchByCoNumber_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (SearchCriteria != null)
                {
                    SearchCriteria.InwardID = (sender as TextBox).Text;
                }
                btnSearch();

            }
        }

        public void btnSearch()
        {
            SearchInwardInvoiceDrug(0, Globals.PageSize);
        }

        #region IHandle<DrugDeptCloseSearchInwardCostListEvent> Members

        public void Handle(DrugDeptCloseSearchInwardCostListEvent message)
        {
            if (message != null && this.IsActive)
            {
                CurrentCostTableMedDept = message.SelectedInwardInvoice as CostTableMedDept;
                CostTableList_Load(CurrentCostTableMedDept.CoID);
            }
        }

        #endregion
        #endregion

        #region Search HOA DON HANG Member

        private InwardInvoiceSearchCriteria _searchCriteriaInvoice;
        public InwardInvoiceSearchCriteria SearchCriteriaInvoice
        {
            get
            {
                return _searchCriteriaInvoice;
            }
            set
            {
                if (_searchCriteriaInvoice != value)
                {
                    _searchCriteriaInvoice = value;
                    NotifyOfPropertyChange(() => SearchCriteriaInvoice);
                }
            }
        }

        private void InnitSearchCriteriaInvoice()
        {
            if (SearchCriteriaInvoice == null)
            {
                SearchCriteriaInvoice = new InwardInvoiceSearchCriteria();
            }
            SearchCriteriaInvoice.InvoiceNumber = "";
            SearchCriteriaInvoice.InwardID = "";
            SearchCriteriaInvoice.FromDate = null;
            SearchCriteriaInvoice.ToDate = null;
            SearchCriteriaInvoice.DateInvoice = null;
            SearchCriteriaInvoice.SupplierID = 0;
        }

        public void OpenPopUpSearchInwardInvoice(IList<InwardDrugMedDeptInvoice> results, int Totalcount)
        {
            //mo pop up tim
            //var proAlloc = Globals.GetViewModel<IDrugDeptInwardDrugSupplierSearch>();
            //proAlloc.SearchCriteria = SearchCriteriaInvoice.DeepCopy();
            //proAlloc.TypID = TypID;
            //proAlloc.V_MedProductType = V_MedProductType;
            //if (V_MedProductType == (long)AllLookupValues.MedProductType.THUOC)
            //{
            //    proAlloc.strHienThi = eHCMSResources.Z0618_G1_TimHDonNhapThuoc.ToUpper();
            //}
            //else if (V_MedProductType == (long)AllLookupValues.MedProductType.Y_CU)
            //{
            //    proAlloc.strHienThi = eHCMSResources.Z0619_G1_TimHDonNhapYCu.ToUpper();
            //}
            //else
            //{
            //    proAlloc.strHienThi = eHCMSResources.Z0620_G1_TimHDonNhapHChat.ToUpper();
            //}
            //proAlloc.InwardInvoiceList.Clear();
            //proAlloc.InwardInvoiceList.TotalItemCount = Totalcount;
            //proAlloc.InwardInvoiceList.PageIndex = 0;
            //if (results != null && results.Count > 0)
            //{
            //    foreach (InwardDrugMedDeptInvoice p in results)
            //    {
            //        proAlloc.InwardInvoiceList.Add(p);
            //    }
            //}
            //var instance = proAlloc as Conductor<object>;
            //Globals.ShowDialog(instance, (o) => { });

            Action<IDrugDeptInwardDrugSupplierSearch> onInitDlg = (proAlloc) =>
            {
                proAlloc.SearchCriteria = SearchCriteriaInvoice.DeepCopy();
                proAlloc.TypID = TypID;
                proAlloc.V_MedProductType = V_MedProductType;
                if (V_MedProductType == (long)AllLookupValues.MedProductType.THUOC)
                {
                    proAlloc.strHienThi = eHCMSResources.Z0618_G1_TimHDonNhapThuoc.ToUpper();
                }
                else if (V_MedProductType == (long)AllLookupValues.MedProductType.Y_CU)
                {
                    proAlloc.strHienThi = eHCMSResources.Z0619_G1_TimHDonNhapYCu.ToUpper();
                }
                else
                {
                    proAlloc.strHienThi = eHCMSResources.Z0620_G1_TimHDonNhapHChat.ToUpper();
                }
                proAlloc.InwardInvoiceList.Clear();
                proAlloc.InwardInvoiceList.TotalItemCount = Totalcount;
                proAlloc.InwardInvoiceList.PageIndex = 0;
                if (results != null && results.Count > 0)
                {
                    foreach (InwardDrugMedDeptInvoice p in results)
                    {
                        proAlloc.InwardInvoiceList.Add(p);
                    }
                }
            };
            GlobalsNAV.ShowDialog<IDrugDeptInwardDrugSupplierSearch>(onInitDlg);
        }

        private long TypID = (long)AllLookupValues.RefOutputType.NHAP_TU_NCC;
        private void SearchInwardInvoice(int PageIndex, int PageSize)
        {

            if (SearchCriteriaInvoice == null)
            {
                return;
            }

            isLoadingSearch = true;
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });

            if (string.IsNullOrEmpty(SearchCriteriaInvoice.InwardID) && string.IsNullOrEmpty(SearchCriteriaInvoice.InvoiceNumber))
            {
                SearchCriteriaInvoice.FromDate = Globals.GetCurServerDateTime().AddDays(-7);
                SearchCriteriaInvoice.ToDate = Globals.GetCurServerDateTime();
            }
            else
            {
                SearchCriteriaInvoice.FromDate = null;
                SearchCriteriaInvoice.ToDate = null;
            }

            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyMedDeptServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginSearchInwardDrugMedDeptInvoice(SearchCriteriaInvoice, TypID, V_MedProductType, PageIndex, PageSize, true, false, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            int Totalcount;
                            var results = contract.EndSearchInwardDrugMedDeptInvoice(out Totalcount, asyncResult);
                            if (results != null && results.Count > 0)
                            {
                                if (results.Count > 1)
                                {
                                    //mo pop up tim
                                    OpenPopUpSearchInwardInvoice(results.ToList(), Totalcount);
                                }
                                else
                                {
                                    //add cai phieu nay vao trong luoi
                                    AddInvoice(results.FirstOrDefault());
                                }
                            }
                            else
                            {
                                if (MessageBox.Show(eHCMSResources.Z0568_G1_KgTimThay, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                                {
                                    SearchCriteriaInvoice.FromDate = Globals.GetCurServerDateTime().AddDays(-7);
                                    SearchCriteriaInvoice.ToDate = Globals.GetCurServerDateTime();
                                    OpenPopUpSearchInwardInvoice(null, 0);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            isLoadingSearch = false;
                            //Globals.IsBusy = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }

        public void tbxSearchInvoiceByInvoiceNumber_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (SearchCriteriaInvoice != null)
                {
                    SearchCriteriaInvoice.InvoiceNumber = (sender as TextBox).Text;
                }
                btnSearchInvoice();
            }
        }

        public void tbxSearchInvoiceByInwardID_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (SearchCriteriaInvoice != null)
                {
                    SearchCriteriaInvoice.InwardID = (sender as TextBox).Text;
                }
                btnSearchInvoice();
            }
        }

        public void btnSearchInvoice()
        {
            SearchInwardInvoice(0, Globals.PageSize);
        }

        #region IHandle<DrugDeptCloseSearchInwardIncoiceEvent> Members

        public void Handle(DrugDeptCloseSearchInwardIncoiceEvent message)
        {
            if (this.IsActive && message != null)
            {
                AddInvoice(message.SelectedInwardInvoice as InwardDrugMedDeptInvoice);
            }
        }

        #endregion

        private void AddInvoice(InwardDrugMedDeptInvoice Item)
        {
            if (CurrentCostTableMedDept.InwardDrugMedDeptInvoices == null)
            {
                CurrentCostTableMedDept.InwardDrugMedDeptInvoices = new ObservableCollection<InwardDrugMedDeptInvoice>();
            }
            //kiem tra item nay da ton tai chua?
            var checkexists = CurrentCostTableMedDept.InwardDrugMedDeptInvoices.Where(x => x.inviID == Item.inviID);
            if (checkexists != null && checkexists.Count() > 0)
            {
                Globals.ShowMessage(string.Format("{0}!", eHCMSResources.Z0675_G1_HDonNayDaDuocChon), eHCMSResources.G0442_G1_TBao);
            }
            else
            {
                CurrentCostTableMedDept.InwardDrugMedDeptInvoices.Add(Item);
            }
        }

        #endregion

        #region printing member

        public void btnPreview()
        {
            //var proAlloc = Globals.GetViewModel<IDrugDeptReportDocumentPreview>();
            //proAlloc.ID = CurrentCostTableMedDept.CoID;
            //proAlloc.V_MedProductType = V_MedProductType;
            //proAlloc.eItem = ReportName.DRUGDEPT_INWARD_NHAPCHIPHI;

            //var instance = proAlloc as Conductor<object>;
            //Globals.ShowDialog(instance, (o) => { });

            Action<IDrugDeptReportDocumentPreview> onInitDlg = (proAlloc) =>
            {
                proAlloc.ID = CurrentCostTableMedDept.CoID;
                proAlloc.V_MedProductType = V_MedProductType;
                proAlloc.eItem = ReportName.DRUGDEPT_INWARD_NHAPCHIPHI;
            };
            GlobalsNAV.ShowDialog<IDrugDeptReportDocumentPreview>(onInitDlg);
        }

        #endregion

        string value = "";
        public enum DataGridCol
        {
            TenCP = 1,
            TotalValue = 2,
            Notes = 3
        }
        public void grdCostList_PreparingCellForEdit(object sender, DataGridPreparingCellForEditEventArgs e)
        {
            TextBox tbl = grdCostList.CurrentColumn.GetCellContent(grdCostList.SelectedItem) as TextBox;
            if (tbl != null)
            {
                value = tbl.Text;
            }
        }

        public void grdCostList_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.Column.DisplayIndex == (int)DataGridCol.TotalValue)
            {
                CostTableMedDeptList item = e.Row.DataContext as CostTableMedDeptList;
                if (item != null)
                {
                    if (item.TotalValue <= 0)
                    {
                        Globals.ShowMessage(eHCMSResources.Z0628_G1_GTriPhaiLonHon0, eHCMSResources.T0074_G1_I);
                        item.TotalValue = Convert.ToDecimal(value);
                    }
                }
            }
            if (e.Column.DisplayIndex == (int)DataGridCol.TenCP)
            {
                CostTableMedDeptList item = e.Row.DataContext as CostTableMedDeptList;
                var lst = CurrentCostTableMedDept.CostTableMedDeptLists.Where(x => x.CoListName == item.CoListName);
                if (lst != null && lst.Count() > 1)
                {
                    Globals.ShowMessage(string.Format("{0}!", eHCMSResources.Z0629_G1_PhiNayDaTonTai), eHCMSResources.T0074_G1_I);
                    item.CoListName = value;
                }
            }
            SumTotal();
            //chua can,khi nao cho nhap hoan toan tren luoi moi can toi
            //if (e.Row.GetIndex() == (CurrentCostTableMedDept.CostTableMedDeptLists.Count - 1) && e.EditAction == DataGridEditAction.Commit)
            //{
            //    AddBlankRow();
            //}
        }

        private void AddBlankRow()
        {
            if (CheckBlankRow())
            {
                if (CurrentCostTableMedDept.CostTableMedDeptLists == null)
                {
                    CurrentCostTableMedDept.CostTableMedDeptLists = new ObservableCollection<CostTableMedDeptList>();
                }
                CostTableMedDeptList p = new CostTableMedDeptList();
                p.CoListName = "";
                p.TotalValue = 0;
                CurrentCostTableMedDept.CostTableMedDeptLists.Add(p);
            }
        }

        private bool CheckBlankRow()
        {
            if (CurrentCostTableMedDept.CostTableMedDeptLists != null && CurrentCostTableMedDept.CostTableMedDeptLists.Count > 0)
            {
                for (int i = 0; i < CurrentCostTableMedDept.CostTableMedDeptLists.Count; i++)
                {
                    if (string.IsNullOrEmpty(CurrentCostTableMedDept.CostTableMedDeptLists[i].CoListName) || CurrentCostTableMedDept.CostTableMedDeptLists[i].TotalValue <=0)
                    {
                        return false;
                    }
                }
            }
            return true;
        }


        public void cbxCurrency_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CurrentCostTableMedDept == null || CurrentCostTableMedDept.SelectedCurrency == null)
            {
                IsEnableExchangeRate = false;
                return;
            }

            if (CurrentCostTableMedDept.SelectedCurrency.CurrencyID == (long)AllLookupValues.CurrencyTable.VND)
            {
                CurrentCostTableMedDept.ExchangeRates = 0;
                IsEnableExchangeRate = false;
            }
            else
            {
                IsEnableExchangeRate = true;
            }
        }
    }
}
