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
using System.Threading;
using DataEntities;
using aEMR.Common.Collections;
using System.Linq;
using aEMR.Common;
using aEMR.Infrastructure.CachingUtils;
using Castle.Core.Logging;
using Castle.Windsor;

using eHCMSLanguage;

namespace aEMR.Pharmacy.ViewModels
{
    [Export(typeof(IPharmacieucalAndSupplier)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class PharmacieucalAndSupplierViewModel : Conductor<object>, IPharmacieucalAndSupplier
    {
        public string TitleForm { get; set; }

        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public PharmacieucalAndSupplierViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            authorization();
            GetPharmaceuticalCompanyCbx();

            SupplierNotPCOs = new PagedSortableCollectionView<Supplier>();
            SupplierNotPCOs.PageSize = Globals.PageSize;
            SupplierNotPCOs.OnRefresh += new EventHandler<RefreshEventArgs>(SupplierNotPCOs_OnRefresh);

            SupplierPCOs = new PagedSortableCollectionView<Supplier>();
            SupplierPCOs.PageSize = Globals.PageSize;
            SupplierPCOs.OnRefresh += new EventHandler<RefreshEventArgs>(SupplierPCOs_OnRefresh);
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
            bTim = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPharmacy
                                               , (int)ePharmacy.mQuanLyNCCVaNSX,
                                               (int)oPharmacyEx.mQuanLyNCCVaNSX_Tim, (int)ePermission.mView);
            bThem = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPharmacy
                                               , (int)ePharmacy.mQuanLyNCCVaNSX,
                                               (int)oPharmacyEx.mQuanLyNCCVaNSX_ChinhSua, (int)ePermission.mView);
            bChinhSua = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPharmacy
                                               , (int)ePharmacy.mQuanLyNCCVaNSX,
                                               (int)oPharmacyEx.mQuanLyNCCVaNSX_ChinhSua, (int)ePermission.mView);
            
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

        #endregion
        #region binding visibilty

        public Button lnkDelete { get; set; }

        public void lnkDelete_Loaded(object sender)
        {
            lnkDelete = sender as Button;
            lnkDelete.Visibility = Globals.convertVisibility(bChinhSua);
        }

        #endregion

        #region Properties Member

        private ObservableCollection<PharmaceuticalCompany> _pharmaceuticalCompanies;
        public ObservableCollection<PharmaceuticalCompany> PharmaceuticalCompanies
        {
            get { return _pharmaceuticalCompanies; }
            set
            {
                if (_pharmaceuticalCompanies != value)
                    _pharmaceuticalCompanies = value;
                NotifyOfPropertyChange(() => PharmaceuticalCompanies);
            }
        }

        private PharmaceuticalCompany _SelectedPharmaceutical;
        public PharmaceuticalCompany SelectedPharmaceutical
        {
            get { return _SelectedPharmaceutical; }
            set
            {
                if (_SelectedPharmaceutical != value)
                    _SelectedPharmaceutical = value;
                NotifyOfPropertyChange(() => SelectedPharmaceutical);
            }
        }

        private PagedSortableCollectionView<Supplier> _SupplierNotPCOs;
        public PagedSortableCollectionView<Supplier> SupplierNotPCOs
        {
            get { return _SupplierNotPCOs; }
            set
            {
                if (_SupplierNotPCOs != value)
                    _SupplierNotPCOs = value;
                NotifyOfPropertyChange(() => SupplierNotPCOs);
            }
        }

        private PagedSortableCollectionView<Supplier> _SupplierPCOs;
        public PagedSortableCollectionView<Supplier> SupplierPCOs
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

        private Supplier _LeftSupplier;
        public Supplier LeftSupplier
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

        private Supplier _RightSupplier;
        public Supplier RightSupplier
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

        #endregion

        private void GetPharmaceuticalCompanyCbx()
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            IsLoading = true;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyDrugServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetPharmaceuticalCompanyCbx(Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndGetPharmaceuticalCompanyCbx(asyncResult);
                            PharmaceuticalCompanies = results.ToObservableCollection();
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
                    contract.BeginGetSupplier_NotPCOID(PCOID,SupplierType,PageSize,PageIndex ,Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            int total = 0;
                            var results = contract.EndGetSupplier_NotPCOID(out total,asyncResult);
                            if (results != null)
                            {
                                SupplierNotPCOs.Clear();
                                SupplierNotPCOs.TotalItemCount = total;
                                foreach (Supplier p in results)
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
                    contract.BeginGetSupplier_ByPCOID(PCOID, SupplierType, PageSize, PageIndex, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            int total = 0;
                            var results = contract.EndGetSupplier_ByPCOID(out total, asyncResult);
                            if (results != null)
                            {
                                SupplierPCOs.Clear();
                                SupplierPCOs.TotalItemCount = total;
                                foreach (Supplier p in results)
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

        private void PharmaceuticalSuppliers_Insert(PharmaceuticalSupplier s)
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            IsLoading = true;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacySuppliersServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginPharmaceuticalSuppliers_Insert(s, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                          
                            int results = contract.EndPharmaceuticalSuppliers_Insert(asyncResult);
                            if (results == 0)
                            {
                                ReloadGrid();
                            }
                            else
                            {
                                Globals.ShowMessage(eHCMSResources.T1987_G1_DaTonTai, eHCMSResources.G0442_G1_TBao);
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

        private void PharmaceuticalSuppliers_Delete(PharmaceuticalSupplier s)
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            IsLoading = true;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacySuppliersServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginPharmaceuticalSuppliers_Delete(s, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {

                            bool results = contract.EndPharmaceuticalSuppliers_Delete(asyncResult);
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
                PharmaceuticalSupplier item = new PharmaceuticalSupplier();
                item.PCOID = SelectedPharmaceutical.PCOID;
                item.SupplierID = LeftSupplier.SupplierID;

                PharmaceuticalSuppliers_Insert(item);
            }
            else
            {
                Globals.ShowMessage(eHCMSResources.Z0854_G1_ChuaChonNSX, eHCMSResources.G0442_G1_TBao);
            }
        }

        public void btDelete()
        {
            if (RightSupplier != null && SelectedPharmaceutical != null)
            {
                PharmaceuticalSupplier item = new PharmaceuticalSupplier();
                item.PCOID = SelectedPharmaceutical.PCOID;
                item.SupplierID = RightSupplier.SupplierID;

                PharmaceuticalSuppliers_Delete(item);
            }
            else
            {
                Globals.ShowMessage(eHCMSResources.Z0854_G1_ChuaChonNSX, eHCMSResources.G0442_G1_TBao);
            }
        }
    }
}
