using System.ComponentModel.Composition;
using aEMR.ViewContracts;
using Caliburn.Micro;
using System.Windows.Input;
using System.Windows.Controls;
using System;
using aEMR.Infrastructure;
using System.Collections.ObjectModel;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using System.Threading;
using DataEntities;
using aEMR.Common.Collections;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using eHCMSLanguage;

namespace aEMR.DrugDept.ViewModels
{
    [Export(typeof(IDrugDeptPharmacieucalAndSupplier)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class DrugDeptPharmacieucalAndSupplierViewModel : Conductor<object>, IDrugDeptPharmacieucalAndSupplier
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public DrugDeptPharmacieucalAndSupplierViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;
            
            authorization();
            GetDrugDeptPharmaceuticalCompanyCbx();

            SupplierNotPCOs = new PagedSortableCollectionView<DrugDeptSupplier>();
            SupplierNotPCOs.PageSize = Globals.PageSize;
            SupplierNotPCOs.OnRefresh += new EventHandler<RefreshEventArgs>(SupplierNotPCOs_OnRefresh);

            SupplierPCOs = new PagedSortableCollectionView<DrugDeptSupplier>();
            SupplierPCOs.PageSize = Globals.PageSize;
            SupplierPCOs.OnRefresh += new EventHandler<RefreshEventArgs>(SupplierPCOs_OnRefresh);
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

        void SupplierPCOs_OnRefresh(object sender, RefreshEventArgs e)
        {
            GetSupplierPCOs(SupplierPCOs.PageIndex, SupplierPCOs.PageSize);

        }

        void SupplierNotPCOs_OnRefresh(object sender, RefreshEventArgs e)
        {
            GetSupplierNotPCOs(SupplierNotPCOs.PageIndex, SupplierNotPCOs.PageSize);

        }

        public void authorization()
        {
            if (!Globals.isAccountCheck)
            {
                return;
            }
        }

        #region check invisible
        private bool _mTim = true;
        private bool _mThemMoi = true;
        private bool _mChinhSua = true;

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

        public bool mThemMoi
        {
            get
            {
                return _mThemMoi;
            }
            set
            {
                if (_mThemMoi == value)
                    return;
                _mThemMoi = value;
                NotifyOfPropertyChange(() => mThemMoi);
            }
        }
        
        public bool mChinhSua
        {
            get
            {
                return _mChinhSua;
            }
            set
            {
                if (_mChinhSua == value)
                    return;
                _mChinhSua = value;
                NotifyOfPropertyChange(() => mChinhSua);
            }
        }
        #endregion
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

     
        private ObservableCollection<DrugDeptPharmaceuticalCompany> _DrugDeptPharmaceuticalCompanies;
        public ObservableCollection<DrugDeptPharmaceuticalCompany> DrugDeptPharmaceuticalCompanies
        {
            get { return _DrugDeptPharmaceuticalCompanies; }
            set
            {
                if (_DrugDeptPharmaceuticalCompanies != value)
                    _DrugDeptPharmaceuticalCompanies = value;
                NotifyOfPropertyChange(() => DrugDeptPharmaceuticalCompanies);
            }
        }

        private DrugDeptPharmaceuticalCompany _SelectedPharmaceutical;
        public DrugDeptPharmaceuticalCompany SelectedPharmaceutical
        {
            get { return _SelectedPharmaceutical; }
            set
            {
                if (_SelectedPharmaceutical != value)
                    _SelectedPharmaceutical = value;
                NotifyOfPropertyChange(() => SelectedPharmaceutical);
            }
        }

        private PagedSortableCollectionView<DrugDeptSupplier> _SupplierNotPCOs;
        public PagedSortableCollectionView<DrugDeptSupplier> SupplierNotPCOs
        {
            get { return _SupplierNotPCOs; }
            set
            {
                if (_SupplierNotPCOs != value)
                    _SupplierNotPCOs = value;
                NotifyOfPropertyChange(() => SupplierNotPCOs);
            }
        }

        private PagedSortableCollectionView<DrugDeptSupplier> _SupplierPCOs;
        public PagedSortableCollectionView<DrugDeptSupplier> SupplierPCOs
        {
            get { return _SupplierPCOs; }
            set
            {
                if (_SupplierPCOs != value)
                    _SupplierPCOs = value;
                NotifyOfPropertyChange(() => SupplierPCOs);
            }
        }

        private bool _LeftEnable;
        public bool LeftEnable
        {
            get { return _LeftEnable; }
            set
            {
                _LeftEnable = value;
                NotifyOfPropertyChange(()=>LeftEnable);
            }
        }

        private bool _RightEnable;
        public bool RightEnable
        {
            get { return _RightEnable; }
            set
            {
                _RightEnable = value;
                NotifyOfPropertyChange(() => RightEnable);
            }
        }

        private DrugDeptSupplier _LeftSupplier;
        public DrugDeptSupplier LeftSupplier
        {
            get { return _LeftSupplier; }
            set
            {
                _LeftSupplier = value;
                NotifyOfPropertyChange(() => LeftSupplier);
                if (LeftSupplier != null)
                {
                    LeftEnable = true;
                    RightEnable = false;
                    RightSupplier = null;
                }
            }
        }

        private DrugDeptSupplier _RightSupplier;
        public DrugDeptSupplier RightSupplier
        {
            get { return _RightSupplier; }
            set
            {
                _RightSupplier = value;
                NotifyOfPropertyChange(() => RightSupplier);
                if (RightSupplier != null)
                {
                    LeftEnable = false;
                    RightEnable = true;
                    LeftSupplier = null;
                }
            }
        }

        private long SupplierType = (long)AllLookupValues.SupplierType.CUNG_CAP_THIET_BI_Y_TE;

        private string _LeftSearch;
        public string LeftSearch
        {
            get { return _LeftSearch; }
            set
            {
                _LeftSearch = value;
                NotifyOfPropertyChange(() => LeftSearch);
            }
        }

        private string _RightSearch;
        public string RightSearch
        {
            get { return _RightSearch; }
            set
            {
                _RightSearch = value;
                NotifyOfPropertyChange(() => RightSearch);
            }
        }

    

        #endregion

        private void GetDrugDeptPharmaceuticalCompanyCbx()
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            IsLoading = true;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacySuppliersServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetDrugDeptPharmaceuticalCompanyCbx(Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndGetDrugDeptPharmaceuticalCompanyCbx(asyncResult);
                            DrugDeptPharmaceuticalCompanies = results.ToObservableCollection();
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

        private void GetSupplierNotPCOs(int PageIndex,int PageSize)
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            long? PCOID = 0;
            if (SelectedPharmaceutical != null)
            {
                PCOID = SelectedPharmaceutical.PCOID;
            }
            IsLoading = true;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacySuppliersServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetSupplierDrugDept_NotPCOID(LeftSearch, PCOID,SupplierType,PageSize,PageIndex ,Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            int total = 0;
                            var results = contract.EndGetSupplierDrugDept_NotPCOID(out total, asyncResult);
                            if (results != null)
                            {
                                SupplierNotPCOs.Clear();
                                SupplierNotPCOs.TotalItemCount = total;
                                foreach (DrugDeptSupplier p in results)
                                {
                                    SupplierNotPCOs.Add(p);
                                }
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

        private void GetSupplierPCOs(int PageIndex, int PageSize)
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            long? PCOID = 0;
            if (SelectedPharmaceutical != null)
            {
                PCOID = SelectedPharmaceutical.PCOID;
            }
            IsLoading = true;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacySuppliersServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetSupplierDrugDept_ByPCOID(RightSearch, PCOID, SupplierType, PageSize, PageIndex, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            int total = 0;
                            var results = contract.EndGetSupplierDrugDept_ByPCOID(out total, asyncResult);
                            if (results != null)
                            {
                                SupplierPCOs.Clear();
                                SupplierPCOs.TotalItemCount = total;
                                foreach (DrugDeptSupplier p in results)
                                {
                                    SupplierPCOs.Add(p);
                                }
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

        private void DrugDeptPharmaceuticalSuppliers_Insert(DrugDeptPharmaceuticalSupplier s)
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            IsLoading = true;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacySuppliersServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginDrugDeptPharmaceuticalSuppliers_Insert(s, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                          
                            int results = contract.EndDrugDeptPharmaceuticalSuppliers_Insert(asyncResult);
                            if (results == 0)
                            {
                                ReloadGrid();
                            }
                            else
                            {
                                Globals.ShowMessage(eHCMSResources.T1987_G1_DaTonTai,eHCMSResources.G0442_G1_TBao);
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

        private void DrugDeptPharmaceuticalSuppliers_Delete(DrugDeptPharmaceuticalSupplier s)
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            IsLoading = true;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacySuppliersServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginDrugDeptPharmaceuticalSuppliers_Delete(s, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {

                            bool results = contract.EndDrugDeptPharmaceuticalSuppliers_Delete(asyncResult);
                            if (results)
                            {
                                ReloadGrid();
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

        public void cbx_ChooseKho_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ReloadGrid();
        }
        private void ReloadGrid()
        {
            SupplierNotPCOs.PageIndex = 0;
            GetSupplierNotPCOs(SupplierNotPCOs.PageIndex, SupplierNotPCOs.PageSize);

            SupplierPCOs.PageIndex = 0;
            GetSupplierPCOs(SupplierPCOs.PageIndex, SupplierPCOs.PageSize);
        }

        public void btAdd()
        {
            if (LeftSupplier != null && SelectedPharmaceutical != null)
            {
                DrugDeptPharmaceuticalSupplier item = new DrugDeptPharmaceuticalSupplier();
                item.PCOID = SelectedPharmaceutical.PCOID;
                item.SupplierID = LeftSupplier.SupplierID;

                DrugDeptPharmaceuticalSuppliers_Insert(item);
            }
            else
            {
                Globals.ShowMessage(string.Format(eHCMSResources.Z0579_G1_VuiLongChon, eHCMSResources.N0273_G1_NSX), eHCMSResources.G0442_G1_TBao);
            }
        }

        public void btDelete()
        {
            if (RightSupplier != null && SelectedPharmaceutical != null)
            {
                DrugDeptPharmaceuticalSupplier item = new DrugDeptPharmaceuticalSupplier();
                item.PCOID = SelectedPharmaceutical.PCOID;
                item.SupplierID = RightSupplier.SupplierID;

                DrugDeptPharmaceuticalSuppliers_Delete(item);
            }
            else
            {
                Globals.ShowMessage(string.Format(eHCMSResources.Z0579_G1_VuiLongChon, eHCMSResources.N0273_G1_NSX), eHCMSResources.G0442_G1_TBao);
            }
        }

        public void TextBox_KeyUp_LeftSearch(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                LeftSearch = (sender as TextBox).Text;
                btnLeftSearch();
            }
        }

        public void btnLeftSearch()
        {
            SupplierNotPCOs.PageIndex = 0;
            GetSupplierNotPCOs(SupplierNotPCOs.PageIndex, SupplierNotPCOs.PageSize);
        }

        public void TextBox_KeyUp_RightSearch(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                RightSearch = (sender as TextBox).Text;
                btnRightSearch();
            }
        }
        
        public void btnRightSearch()
        {
            SupplierPCOs.PageIndex = 0;
            GetSupplierPCOs(SupplierPCOs.PageIndex, SupplierPCOs.PageSize);
        }

   

    }
}
