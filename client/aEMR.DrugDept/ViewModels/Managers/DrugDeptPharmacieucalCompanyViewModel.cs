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
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using System.Threading;
using aEMR.Common.SynchronousDispatcher;
using System.Windows.Input;
using System.Windows;
using aEMR.Common;
using System.Collections.Generic;
using aEMR.CommonTasks;
using eHCMSLanguage;

namespace aEMR.DrugDept.ViewModels
{
    [Export(typeof(IDrugDeptPharmacieucalCompany)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class DrugDeptPharmacieucalCompanyViewModel : Conductor<object>, IDrugDeptPharmacieucalCompany
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public DrugDeptPharmacieucalCompanyViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;


            authorization();
            DrugDeptPharmaceuticalCompanies = new PagedSortableCollectionView<DrugDeptPharmaceuticalCompany>();
            DrugDeptPharmaceuticalCompanies.PageSize = Globals.PageSize;
            DrugDeptPharmaceuticalCompanies.OnRefresh += new EventHandler<RefreshEventArgs>(DrugDeptPharmaceuticalCompanies_OnRefresh);
            Content = ADD;
            Contenttitle = ADDTITLE;
            SelectedDrugDeptPharmaceuticalCompany = new DrugDeptPharmaceuticalCompany();
            Coroutine.BeginExecute(DoCountryListList());
        }

        void DrugDeptPharmaceuticalCompanies_OnRefresh(object sender, RefreshEventArgs e)
        {
            SearchDrugDeptPharmaceuticalCompanies(DrugDeptPharmaceuticalCompanies.PageIndex, DrugDeptPharmaceuticalCompanies.PageSize);
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

        private void InitSelectedDrugDeptPharmaceuticalCompany()
        {
            SelectedDrugDeptPharmaceuticalCompany = null;
            SelectedDrugDeptPharmaceuticalCompany = new DrugDeptPharmaceuticalCompany();
        }
        public void authorization()
        {
            if (!Globals.isAccountCheck)
            {
                return;
            }
        }
        #region checking account
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
        public Button lnkView { get; set; }

        public void lnkDelete_Loaded(object sender)
        {
            lnkDelete = sender as Button;
            lnkDelete.Visibility = Globals.convertVisibility(mChinhSua);
        }
        public void lnkView_Loaded(object sender)
        {
            lnkView = sender as Button;
            lnkView.Visibility = Globals.convertVisibility(mChinhSua);
        }
        #endregion
        #region Public Properties

        private string ADD = eHCMSResources.G0156_G1_Them;
        private string UPDATE = eHCMSResources.K1599_G1_CNhat;

        private string ADDTITLE = eHCMSResources.Z0092_G1_ThemMoiNSX;
        private string UPDATETITLE = eHCMSResources.Z0093_G1_CNhatNSX;

        private Visibility _Visibility = Visibility.Collapsed;
        public Visibility Visibility
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
                if (Visibility == Visibility.Visible)
                {
                    NotExpanded = false;
                }
                else
                {
                    NotExpanded = true;
                }
            }
        }

        private bool _IsChildWindow = true;
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

        private bool _NotExpanded = true;
        public bool NotExpanded
        {
            get
            {
                return _NotExpanded;
            }
            set
            {
                if (_NotExpanded != value)
                {
                    _NotExpanded = value;
                    NotifyOfPropertyChange(() => NotExpanded);
                }
            }
        }

        private string _PCOName;
        public string PCOName
        {
            get
            {
                return _PCOName;
            }
            set
            {
                if (_PCOName != value)
                {
                    _PCOName = value;
                    NotifyOfPropertyChange(() => PCOName);
                }
            }
        }

        private string _Content;
        public string Content
        {
            get
            {
                return _Content;
            }
            set
            {
                if (_Content != value)
                {
                    _Content = value;
                    NotifyOfPropertyChange(() => Content);
                }
            }
        }

        private string _Contenttitle;
        public string Contenttitle
        {
            get
            {
                return _Contenttitle;
            }
            set
            {
                if (_Contenttitle != value)
                {
                    _Contenttitle = value;
                    NotifyOfPropertyChange(() => Contenttitle);
                }
            }
        }

        private DrugDeptPharmaceuticalCompany _selectedDrugDeptPharmaceuticalCompany;
        public DrugDeptPharmaceuticalCompany SelectedDrugDeptPharmaceuticalCompany
        {
            get
            {
                return _selectedDrugDeptPharmaceuticalCompany;
            }
            set
            {
                if (_selectedDrugDeptPharmaceuticalCompany != value)
                {
                    _selectedDrugDeptPharmaceuticalCompany = value;
                    NotifyOfPropertyChange(() => SelectedDrugDeptPharmaceuticalCompany);
                }
            }
        }

        private PagedSortableCollectionView<DrugDeptPharmaceuticalCompany> _DrugDeptPharmaceuticalCompanies;
        public PagedSortableCollectionView<DrugDeptPharmaceuticalCompany> DrugDeptPharmaceuticalCompanies
        {
            get
            {
                return _DrugDeptPharmaceuticalCompanies;
            }
            set
            {
                if (_DrugDeptPharmaceuticalCompanies != value)
                {
                    _DrugDeptPharmaceuticalCompanies = value;
                }
                NotifyOfPropertyChange(() => DrugDeptPharmaceuticalCompanies);
            }
        }

        private ObservableCollection<RefCountry> _countries;
        public ObservableCollection<RefCountry> Countries
        {
            get
            {
                return _countries;
            }
            set
            {
                if (_countries != value)
                {
                    _countries = value;
                    NotifyOfPropertyChange(() => Countries);
                }
            }
        }
        #endregion

        #region Function Member

        DataGrid GridDrugDeptPharmaceuticalCompanies = null;
        public void GridPharmaceuticalCompanies_Loaded(object sender, RoutedEventArgs e)
        {
            GridDrugDeptPharmaceuticalCompanies = sender as DataGrid;
        }

        private IEnumerator<IResult> DoCountryListList()
        {
            var paymentTypeTask = new LoadCountryListTask(false, true);
            yield return paymentTypeTask;
            Countries = paymentTypeTask.RefCountryList;
            yield break;
        }

        public void SearchDrugDeptPharmaceuticalCompanies(int PageIndex, int PageSize)
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            int totalCount;
            IsLoading = true;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacySuppliersServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginDrugDeptPharmaceuticalCompany_SearchPaging(PCOName, PageSize, PageIndex, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndDrugDeptPharmaceuticalCompany_SearchPaging(out totalCount, asyncResult);
                            if (results != null)
                            {
                                DrugDeptPharmaceuticalCompanies.Clear();
                                DrugDeptPharmaceuticalCompanies.TotalItemCount = totalCount;
                                foreach (DrugDeptPharmaceuticalCompany p in results)
                                {
                                    DrugDeptPharmaceuticalCompanies.Add(p);
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

        public void DrugDeptPharmaceuticalCompany_Insert(DrugDeptPharmaceuticalCompany S)
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            IsLoading = true;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacySuppliersServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginDrugDeptPharmaceuticalCompany_Insert(S, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            int results = contract.EndDrugDeptPharmaceuticalCompany_Insert(asyncResult);
                            if (results == 0)
                            {
                                Globals.ShowMessage(eHCMSResources.A0079_G1_Msg_InfoThemMoiOK, eHCMSResources.G0442_G1_TBao);
                                Visibility = Visibility.Collapsed;
                                InitSelectedDrugDeptPharmaceuticalCompany();
                                btnSearch();
                            }
                            else
                            {
                                Globals.ShowMessage(string.Format("{0}!", eHCMSResources.Z0683_G1_NSXDaTonTai), eHCMSResources.G0442_G1_TBao);
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

        public void DrugDeptPharmaceuticalCompany_Update(DrugDeptPharmaceuticalCompany S)
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            IsLoading = true;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacySuppliersServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginDrugDeptPharmaceuticalCompany_Update(S, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            int results = contract.EndDrugDeptPharmaceuticalCompany_Update(asyncResult);
                            if (results == 0)
                            {
                                Globals.ShowMessage(eHCMSResources.A0279_G1_Msg_InfoCNhatOK, eHCMSResources.G0442_G1_TBao);
                                Visibility = Visibility.Collapsed;
                                Content = ADD;
                                Contenttitle = ADDTITLE;
                                InitSelectedDrugDeptPharmaceuticalCompany();
                                btnSearch();
                            }
                            else
                            {
                                Globals.ShowMessage(string.Format("{0}!", eHCMSResources.Z0683_G1_NSXDaTonTai), eHCMSResources.G0442_G1_TBao);
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

        public void DrugDeptPharmaceuticalCompany_Delete(long PCOID)
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            IsLoading = true;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacySuppliersServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginDrugDeptPharmaceuticalCompany_Delete(PCOID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndDrugDeptPharmaceuticalCompany_Delete(asyncResult);
                            Globals.ShowMessage(eHCMSResources.K0537_G1_XoaOk, eHCMSResources.G0442_G1_TBao);
                            btnSearch();
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



        public void KeyUpSearch(object sender, KeyEventArgs e)
        {
            //perform your task with parameter e.    
            if (e.Key == Key.Enter)
            {
                PCOName = (sender as TextBox).Text;
                btnSearch();
            }
        }

        public void btnSearch()
        {
            DrugDeptPharmaceuticalCompanies.PageIndex = 0;
            SearchDrugDeptPharmaceuticalCompanies(0, DrugDeptPharmaceuticalCompanies.PageSize);
        }

        public void ViewClick(object sender, RoutedEventArgs e)
        {
            if (GridDrugDeptPharmaceuticalCompanies != null && GridDrugDeptPharmaceuticalCompanies.SelectedItem != null)
            {
                Content = UPDATE;
                Contenttitle = UPDATETITLE;

                Visibility = Visibility.Visible;
                SelectedDrugDeptPharmaceuticalCompany = (GridDrugDeptPharmaceuticalCompanies.SelectedItem as DrugDeptPharmaceuticalCompany).DeepCopy();
            }
        }

        public void DeletedClick(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show(string.Format(eHCMSResources.Z0476_G1_BanMuonXoa, eHCMSResources.N0273_G1_NSX), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                if (GridDrugDeptPharmaceuticalCompanies.SelectedItem != null)
                    DrugDeptPharmaceuticalCompany_Delete(((DrugDeptPharmaceuticalCompany)GridDrugDeptPharmaceuticalCompanies.SelectedItem).PCOID);
            }
        }

        public void GridDrugDeptPharmaceuticalCompanies_DblClick(object sender, Common.EventArgs<object> e)
        {
            //neu la childwindow thi phat ra su kien ben duoi

        }

        public void LoadData(object sender, EventArgs e)
        {
            btnSearch();
        }
        #endregion

        public void btn_Print()
        {
            //var proAlloc = Globals.GetViewModel<IReportDocumentPreview>();
            //proAlloc.eItem = ReportName.PHARMACY_PHARMACYSUPPLIERTEMPLATE;
            //var instance = proAlloc as Conductor<object>;
            //Globals.ShowDialog(instance, (o) => { });

            Action<IReportDocumentPreview> onInitDlg = (proAlloc) =>
            {
                proAlloc.eItem = ReportName.PHARMACY_PHARMACYSUPPLIERTEMPLATE;
            };
            GlobalsNAV.ShowDialog<IReportDocumentPreview>(onInitDlg);
        }

        public void btnCancel()
        {
            Content = ADD;
            Contenttitle = ADDTITLE;
            InitSelectedDrugDeptPharmaceuticalCompany();
            Visibility = Visibility.Collapsed;
        }
        public bool CheckValid(object temp)
        {
            DrugDeptPharmaceuticalCompany u = temp as DrugDeptPharmaceuticalCompany;
            if (u == null)
            {
                return false;
            }
            return u.Validate();
        }
        public void btnAddOrUpdate()
        {
            if (CheckValid(SelectedDrugDeptPharmaceuticalCompany))
            {
                if (Content == ADD)
                {
                    DrugDeptPharmaceuticalCompany_Insert(SelectedDrugDeptPharmaceuticalCompany);
                }
                else
                {
                    DrugDeptPharmaceuticalCompany_Update(SelectedDrugDeptPharmaceuticalCompany);
                }
            }
        }
        public void hlbAdd()
        {
            Visibility = Visibility.Visible;
        }

        public void GridPharmaceuticalCompanies_DblClick(object sender, Common.EventArgs<object> e)
        {
            //neu la childwindow thi phat ra su kien ben duoi
            if (IsChildWindow)
            {
                TryClose();
                Globals.EventAggregator.Publish(new DrugDeptCloseSearchPharmaceuticalCompanyEvent { SelectedPharmaceuticalCompany = e.Value });
            }
        }
        public void GridSuppliers_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString() + ". ";
        }

        public void lnkPCOID_Click(object sender, RoutedEventArgs e)
        {
            //var proAlloc = Globals.GetViewModel<IDrugDeptViewPharmacieucalCompany>();
            //if (GridDrugDeptPharmaceuticalCompanies != null)
            //{
            //    proAlloc.SelectedDrugDeptPharmaceuticalCompany = (GridDrugDeptPharmaceuticalCompanies.SelectedItem as DrugDeptPharmaceuticalCompany).DeepCopy();
            //}
            //proAlloc.RefGenMedProductDetails_ByPCOID(0, Globals.PageSize);
            //var instance = proAlloc as Conductor<object>;
            //Globals.ShowDialog(instance, (o) => { });


            Action<IDrugDeptViewPharmacieucalCompany> onInitDlg = (proAlloc) =>
            {
                if (GridDrugDeptPharmaceuticalCompanies != null)
                {
                    proAlloc.SelectedDrugDeptPharmaceuticalCompany = (GridDrugDeptPharmaceuticalCompanies.SelectedItem as DrugDeptPharmaceuticalCompany).DeepCopy();
                }
                proAlloc.RefGenMedProductDetails_ByPCOID(0, Globals.PageSize);
            };
            GlobalsNAV.ShowDialog<IDrugDeptViewPharmacieucalCompany>(onInitDlg);
        }
    }
}
