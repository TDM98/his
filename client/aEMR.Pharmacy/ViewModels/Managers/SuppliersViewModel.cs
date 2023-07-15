using System;
using System.ComponentModel.Composition;
using System.Windows.Controls;
using aEMR.ViewContracts;
using aEMR.ServiceClient;
using Caliburn.Micro;
using DataEntities;
using aEMR.Common.Collections;
using System.Collections.ObjectModel;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using System.Threading;
using aEMR.Common.SynchronousDispatcher;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using System.Windows.Input;
using System.Windows;
using eHCMSLanguage;
using aEMR.Common.BaseModel;
using aEMR.Common.ExportExcel;
/*
* 20220404 #001 DatTB: Phân loại NNC: Cho cập nhật loại "Dùng chung" ở màn hình nhà thuốc
* 20220516 #002 DatTB: Nếu là ChildWindow thì không cho thêm mới NCC
* 20230707 #003 QTD:   Thêm nút xuất Excel danh mục NCC
*/
namespace aEMR.Pharmacy.ViewModels
{
    [Export(typeof(ISuppliers)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class SuppliersViewModel : ViewModelBase, ISuppliers, IHandle<PharmacyCloseAddSupplierEvent>, IHandle<PharmacyCloseEditSupplierEvent>
    {
        public string TitleForm { get; set; }

        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public SuppliersViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            authorization();
            SupplierCriteria = new SupplierSearchCriteria();
            SupplierCriteria.V_SupplierType = GetCurrentSupplierType();

            Suppliers = new PagedSortableCollectionView<Supplier>();
            Suppliers.OnRefresh += Suppliers_OnRefresh;
            Suppliers.PageSize = Globals.PageSize;

            _selectedSupplier = new Supplier();
            Globals.EventAggregator.Subscribe(this);
        }

        void Suppliers_OnRefresh(object sender, RefreshEventArgs e)
        {
            SearchSuppliers(Suppliers.PageIndex, Suppliers.PageSize);
        }
        public void authorization()
        {
            if (!Globals.isAccountCheck)
            {
                return;
            }
            bTim = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPharmacy
                                               , (int)ePharmacy.mQuanLyNhaCungCap,
                                               (int)oPharmacyEx.mQuanLyNhaCungCap_Tim, (int)ePermission.mView);
            bThem = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPharmacy
                                               , (int)ePharmacy.mQuanLyNhaCungCap,
                                               (int)oPharmacyEx.mQuanLyNhaCungCap_Them, (int)ePermission.mView);
            bChinhSua = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPharmacy
                                               , (int)ePharmacy.mQuanLyNhaCungCap,
                                               (int)oPharmacyEx.mQuanLyNhaCungCap_ChinhSua, (int)ePermission.mView);
            bInMau = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPharmacy
                                               , (int)ePharmacy.mQuanLyNhaCungCap,
                                               (int)oPharmacyEx.mQuanLyNhaCungCap_InMau, (int)ePermission.mView);


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

        private eFirePharmacySupplierEvent _ePharmacySupplierEvent;
        public eFirePharmacySupplierEvent ePharmacySupplierEvent 
        {
            get { return _ePharmacySupplierEvent; }
            set
            {
                if (_ePharmacySupplierEvent != value)
                {
                    _ePharmacySupplierEvent = value;
                    NotifyOfPropertyChange(() => ePharmacySupplierEvent);
                }
            }
        }
        #region checking account

        private bool _bTim = true;
        private bool _bThem = true;
        private bool _bChinhSua = true;
        private bool _bInMau = true;

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
                //▼==== #002
                return _bThem && !IsChildWindow;
                //▲==== #002
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
        public bool bInMau
        {
            get
            {
                return _bInMau;
            }
            set
            {
                if (_bInMau == value)
                    return;
                _bInMau = value;
            }
        }
        #endregion
        #region binding visibilty

        public Button lnkDelete { get; set; }
        public Button lnkView { get; set; }

        public void lnkDelete_Loaded(object sender)
        {
            lnkDelete = sender as Button;
            lnkDelete.Visibility = Globals.convertVisibility(bChinhSua);
        }
        public void lnkView_Loaded(object sender)
        {
            lnkView = sender as Button;
            lnkView.Visibility = Globals.convertVisibility(bChinhSua);
        }
        #endregion
        #region Public Properties

        private bool _IsChildWindow = false;
        public bool IsChildWindow
        {
            get
            {
                return _IsChildWindow;
            }
            set
            {
                if (_IsChildWindow != value)
                {
                    _IsChildWindow = value;
                    NotifyOfPropertyChange(() => IsChildWindow);
                }
            }
        }

        private Supplier _selectedSupplier;
        public Supplier SelectedSupplier
        {
            get
            {
                return _selectedSupplier;
            }
            set
            {
                if (_selectedSupplier != value)
                {
                    _selectedSupplier = value;
                    NotifyOfPropertyChange(() => SelectedSupplier);
                }
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


        private PagedSortableCollectionView<Supplier> _suppliers;
        public PagedSortableCollectionView<Supplier> Suppliers
        {
            get
            {
                return _suppliers;
            }
            set
            {
                if (_suppliers != value)
                {
                    _suppliers = value;
                }
                NotifyOfPropertyChange(() => Suppliers);
            }
        }


        public bool Flag = true;

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

        #region Function Member

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

        public void DeleteSupplier()
        {
            this.DlgShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            //this.ShowBusyIndicator();
            IsLoading = true;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacySuppliersServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginDeleteSupplierByID(SelectedSupplier.SupplierID, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            contract.EndDeleteSupplierByID(asyncResult);
                            Suppliers.PageIndex = 0;
                            SearchSuppliers(0, Suppliers.PageSize);
                            MessageBox.Show(eHCMSResources.K0537_G1_XoaOk, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                            _logger.Info(ex.Message);
                        }
                        finally
                        {
                            IsLoading = false;
                            //this.HideBusyIndicator();
                            this.DlgHideBusyIndicator();
                        }

                    }), null);

                }

            });

            t.Start();
        }

        public void SearchSuppliers(int PageIndex, int PageSize)
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            this.DlgShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            int totalCount;
            //IsLoading = true;
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
                            if (results != null)
                            {
                                if (Suppliers == null)
                                {
                                    Suppliers = new PagedSortableCollectionView<Supplier>();
                                }
                                else
                                {
                                    Suppliers.Clear();
                                }
                                Suppliers.TotalItemCount = totalCount;
                                foreach (Supplier p in results)
                                {
                                    Suppliers.Add(p);
                                }
                                NotifyOfPropertyChange(() => Suppliers);
                            }
                        }
                        catch (Exception ex)
                        {
                            this.DlgHideBusyIndicator();
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                            _logger.Info(ex.Message);
                        }
                        finally
                        {
                            this.DlgHideBusyIndicator();
                            //IsLoading = false;
                            //Globals.IsBusy = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }

        public bool CheckValid(object temp)
        {
            Supplier u = temp as Supplier;
            if (u == null)
            {
                return false;
            }
            return u.Validate();
        }

        public void KeyUpSearch(object sender, KeyEventArgs e)
        {
            //perform your task with parameter e.    
            if (e.Key == Key.Enter)
            {
                if (SupplierCriteria != null)
                {
                    SupplierCriteria.SupplierName = (sender as TextBox).Text;
                }
                btnSearch();
            }
        }

        public void btnSearch()
        {
            Suppliers.PageIndex = 0;
            SearchSuppliers(0, Suppliers.PageSize);
        }

        public void DeletedClick(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show(eHCMSResources.A0119_G1_Msg_ConfXoaNCC, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                DeleteSupplier();
            }
        }

        public void ViewClick(object sender, RoutedEventArgs e)
        {
            //var proAlloc = Globals.GetViewModel<ISupplier_Edit>();
            //proAlloc.SelectedSupplier = SelectedSupplier;
            //proAlloc.LoadDsHangCC();
            //var instance = proAlloc as Conductor<object>;
            //instance.DisplayName = string.Format("{0}: {1}", eHCMSResources.K1885_G1_ChSuaNCC, SelectedSupplier.SupplierName);
            //Globals.ShowDialog(instance, (o) => { });

            void onInitDlg(ISupplier_Edit proAlloc)
            {
                proAlloc.SelectedSupplier = SelectedSupplier;
                //▼====: #001
                //if (proAlloc.SelectedSupplier.SupplierDrugDeptPharmOthers != 2)
                //{
                //    proAlloc.bOKButton = Visibility.Collapsed;
                //}
                //▲====: #001
                proAlloc.LoadDsHangCC();
                //var instance = proAlloc as Conductor<object>;
                //instance.DisplayName = string.Format("{0}: {1}", eHCMSResources.K1885_G1_ChSuaNCC, SelectedSupplier.SupplierName);
            }
            GlobalsNAV.ShowDialog<ISupplier_Edit>(onInitDlg, null, false, true, Globals.GetDefaultDialogViewSize());

        }

        public void GridSuppliers_DblClick(object sender, Common.EventArgs<object> e)
        {
            //neu la childwindow thi phat ra su kien ben duoi
            if (IsChildWindow)
            {
                TryClose();
                switch (ePharmacySupplierEvent)
                {
                    case (eFirePharmacySupplierEvent.EstimationPharmacy):
                        {
                            Globals.EventAggregator.Publish(new PharmacySupplierToEstimationEvent { SelectedSupplier = e.Value });
                            break;
                        }
                    default:
                        {
                            Globals.EventAggregator.Publish(new PharmacyCloseSearchSupplierEvent { SelectedSupplier = e.Value });
                            break;
                        }
                }

                
            }
            else
            {
                //mo popup de chinh sua
                SelectedSupplier = e.Value as Supplier;
                if (SelectedSupplier != null)
                {
                    //var proAlloc = Globals.GetViewModel<ISupplier_Edit>();
                    //proAlloc.SelectedSupplier = SelectedSupplier;
                    //proAlloc.LoadDsHangCC();
                    //var instance = proAlloc as Conductor<object>;
                    //instance.DisplayName = string.Format("{0}: {1}", eHCMSResources.K1885_G1_ChSuaNCC, SelectedSupplier.SupplierName);
                    //Globals.ShowDialog(instance, (o) => { });


                    Action<ISupplier_Edit> onInitDlg = (proAlloc) =>
                    {
                        proAlloc.SelectedSupplier = SelectedSupplier;
                        proAlloc.LoadDsHangCC();
                        //var instance = proAlloc as Conductor<object>;
                        //instance.DisplayName = string.Format("{0}: {1}", eHCMSResources.K1885_G1_ChSuaNCC, SelectedSupplier.SupplierName);
                    };
                    GlobalsNAV.ShowDialog<ISupplier_Edit>(onInitDlg);
                }
            }
        }

        public void LoadData(object sender, EventArgs e)
        {
            btnSearch();
        }

        public void btn_Add(object sender, RoutedEventArgs e)
        {
            //var proAlloc = Globals.GetViewModel<ISupplier_Add>();
            //var instance = proAlloc as Conductor<object>;
            //instance.DisplayName = eHCMSResources.Z1495_G1_ThemNCCVaoDMuc;
            //Globals.ShowDialog(instance, (o) => { });


            GlobalsNAV.ShowDialog<ISupplier_Add>();
        }

        #endregion

        #region IHandle<PharmacyCloseAddSupplierEvent> Members

        public void Handle(PharmacyCloseAddSupplierEvent message)
        {
            if (this.IsActive)
            {
                btnSearch();
            }
        }

        #endregion

        #region IHandle<PharmacyCloseEditSupplierEvent> Members

        public void Handle(PharmacyCloseEditSupplierEvent message)
        {
            if (this.IsActive)
            {
                btnSearch();
            }
        }

        #endregion

        public void btn_Print()
        {
            //var proAlloc = Globals.GetViewModel<IReportDocumentPreview>();
            //proAlloc.eItem = ReportName.PHARMACY_PHARMACYSUPPLIERTEMPLATE;
            //var instance = proAlloc as Conductor<object>;
            //Globals.ShowDialog(instance, (o) => { });

            //Action<IReportDocumentPreview> onInitDlg = (proAlloc) =>
            //{
            //    proAlloc.eItem = ReportName.PHARMACY_PHARMACYSUPPLIERTEMPLATE;
            //};
            //GlobalsNAV.ShowDialog<IReportDocumentPreview>(onInitDlg);

            IReportDocumentPreview DialogView = Globals.GetViewModel<IReportDocumentPreview>();
            DialogView.eItem = ReportName.PHARMACY_PHARMACYSUPPLIERTEMPLATE;
            GlobalsNAV.ShowDialog_V3(DialogView, null, null, false, true, Globals.GetDefaultDialogViewSize());

        }

        //▼===== #003
        private string strNameExcel = "";
        private long _ExportFor = (long)AllLookupValues.SupplierDrugDeptPharmOthers.PHARMACY;
        public long ExportFor
        {
            get { return _ExportFor; }
            set
            {
                _ExportFor = value;
                NotifyOfPropertyChange(() => ExportFor);
            }
        }
        public void btn_ExportExcel()
        {
            this.ShowBusyIndicator(eHCMSResources.Z0669_G1_DangLayDLieu);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacySuppliersServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginExportExcelSupplier(ExportFor,
                            Globals.DispatchCallback(asyncResult =>
                            {
                                try
                                {
                                    strNameExcel = string.Format("{0} ", eHCMSResources.N0177_G1_NCC);
                                    var results = contract.EndExportExcelSupplier(asyncResult);
                                    ExportToExcelFileAllData.Export(results, strNameExcel);
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
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
                    MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }
        //▲===== #003
    }
}
