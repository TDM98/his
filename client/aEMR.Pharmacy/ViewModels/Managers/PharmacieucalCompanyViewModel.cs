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
using System.Windows.Input;
using System.Windows;
using aEMR.Common;
using aEMR.CommonTasks;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using System.Collections.Generic;
using eHCMSLanguage;
using aEMR.Common.BaseModel;

namespace aEMR.Pharmacy.ViewModels
{
    [Export(typeof(IPharmacieucalCompany)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class PharmacieucalCompanyViewModel : ViewModelBase, IPharmacieucalCompany, IHandle<PharmacyCloseAddSupplierEvent>, IHandle<PharmacyCloseEditSupplierEvent>
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public PharmacieucalCompanyViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            authorization();
            PharmaceuticalCompanies = new PagedSortableCollectionView<PharmaceuticalCompany>();
            PharmaceuticalCompanies.PageSize = Globals.PageSize;
            PharmaceuticalCompanies.OnRefresh += new EventHandler<RefreshEventArgs>(PharmaceuticalCompanies_OnRefresh);
            Content = ADD;
            Contenttitle = ADDTITLE;
            SelectedPharmaceuticalCompany = new PharmaceuticalCompany();
            Coroutine.BeginExecute(DoCountryListList());
        }

        void PharmaceuticalCompanies_OnRefresh(object sender, RefreshEventArgs e)
        {
            SearchPharmaceuticalCompanies(PharmaceuticalCompanies.PageIndex, PharmaceuticalCompanies.PageSize);
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
        private void InitSelectedPharmaceuticalCompany()
        {
            SelectedPharmaceuticalCompany = null;
            SelectedPharmaceuticalCompany = new PharmaceuticalCompany();
        }
        public void authorization()
        {
            if (!Globals.isAccountCheck)
            {
                return;
            }
            bTim = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPharmacy
                                               , (int)ePharmacy.mQuanLyNhaSanXuat,
                                               (int)oPharmacyEx.mQuanLyNhaSanXuat_Tim, (int)ePermission.mView);
            bThem = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPharmacy
                                               , (int)ePharmacy.mQuanLyNhaSanXuat,
                                               (int)oPharmacyEx.mQuanLyNhaSanXuat_Them, (int)ePermission.mView);
            bChinhSua = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPharmacy
                                               , (int)ePharmacy.mQuanLyNhaSanXuat,
                                               (int)oPharmacyEx.mQuanLyNhaSanXuat_ChinhSua, (int)ePermission.mView);
            

        }

        private eFirePharmacieucalCompanyEvent _ePharmacieucalCompany;
        public eFirePharmacieucalCompanyEvent ePharmacieucalCompany
        {
            get { return _ePharmacieucalCompany; }
            set
            {
                if (_ePharmacieucalCompany != value)
                {
                    _ePharmacieucalCompany = value;
                    NotifyOfPropertyChange(() => ePharmacieucalCompany);
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

        public string TitleForm { get; set; }

        private string ADD = eHCMSResources.G0156_G1_Them;
        private string UPDATE = eHCMSResources.K1599_G1_CNhat;

        private string ADDTITLE = eHCMSResources.Z0092_G1_ThemMoiNSX;
        private string UPDATETITLE = eHCMSResources.Z0093_G1_CNhatNSX;

        private Visibility _Visibility = Visibility.Collapsed;
        public Visibility Visibility
        {
            get {
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

        private PharmaceuticalCompany _selectedPharmaceuticalCompany;
        public PharmaceuticalCompany SelectedPharmaceuticalCompany
        {
            get
            {
                return _selectedPharmaceuticalCompany;
            }
            set
            {
                if (_selectedPharmaceuticalCompany != value)
                {
                    _selectedPharmaceuticalCompany = value;
                    NotifyOfPropertyChange(() => SelectedPharmaceuticalCompany);
                }
            }
        }

        private PagedSortableCollectionView<PharmaceuticalCompany> _PharmaceuticalCompanies;
        public PagedSortableCollectionView<PharmaceuticalCompany> PharmaceuticalCompanies
        {
            get
            {
                return _PharmaceuticalCompanies;
            }
            set
            {
                if (_PharmaceuticalCompanies != value)
                {
                    _PharmaceuticalCompanies = value;
                }
                NotifyOfPropertyChange(()=>PharmaceuticalCompanies);
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

        DataGrid GridPharmaceuticalCompanies = null;
        public void GridPharmaceuticalCompanies_Loaded(object sender, RoutedEventArgs e)
        {
            GridPharmaceuticalCompanies=sender as DataGrid;
        }

        private IEnumerator<IResult> DoCountryListList()
        {
            var paymentTypeTask = new LoadCountryListTask(false, true);
            yield return paymentTypeTask;
            Countries = paymentTypeTask.RefCountryList;
            yield break;
        }

        private void SearchPharmaceuticalCompanies(int PageIndex,int PageSize)
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            this.DlgShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            int totalCount;
            IsLoading = true;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacySuppliersServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginPharmaceuticalCompany_SearchPaging(PCOName, PageSize, PageIndex, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndPharmaceuticalCompany_SearchPaging(out totalCount, asyncResult);
                            if (results != null)
                            {
                                PharmaceuticalCompanies.Clear();
                                PharmaceuticalCompanies.TotalItemCount = totalCount;
                                foreach (PharmaceuticalCompany p in results)
                                {
                                    PharmaceuticalCompanies.Add(p);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                            _logger.Info(ex.Message);
                        }
                        finally
                        {
                            this.DlgHideBusyIndicator();
                            IsLoading = false;
                            //Globals.IsBusy = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }

        public void PharmaceuticalCompany_Insert(PharmaceuticalCompany S)
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            this.DlgShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            IsLoading = true;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacySuppliersServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginPharmaceuticalCompany_Insert(S, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            int results = contract.EndPharmaceuticalCompany_Insert(asyncResult);
                            if (results == 0)
                            {
                                Globals.ShowMessage(eHCMSResources.A0079_G1_Msg_InfoThemMoiOK, eHCMSResources.G0442_G1_TBao);
                                Visibility = Visibility.Collapsed;
                                InitSelectedPharmaceuticalCompany();
                                btnSearch();
                            }
                            else
                            {
                                Globals.ShowMessage(eHCMSResources.Z0683_G1_NSXDaTonTai, eHCMSResources.G0442_G1_TBao);
                            }
                                
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                            _logger.Info(ex.Message);
                        }
                        finally
                        {
                            this.DlgHideBusyIndicator();
                            IsLoading = false;
                            //Globals.IsBusy = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }

        public void PharmaceuticalCompany_Update(PharmaceuticalCompany S)
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            this.DlgShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            IsLoading = true;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacySuppliersServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginPharmaceuticalCompany_Update(S, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            int results = contract.EndPharmaceuticalCompany_Update(asyncResult);
                            if (results == 0)
                            {
                                Globals.ShowMessage(eHCMSResources.A0279_G1_Msg_InfoCNhatOK, eHCMSResources.G0442_G1_TBao);
                                Visibility = Visibility.Collapsed;
                                Content = ADD;
                                Contenttitle = ADDTITLE;
                                InitSelectedPharmaceuticalCompany();
                                btnSearch();
                            }
                            else
                            {
                                Globals.ShowMessage(eHCMSResources.Z0683_G1_NSXDaTonTai, eHCMSResources.G0442_G1_TBao);
                            }

                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                            _logger.Info(ex.Message);
                        }
                        finally
                        {
                            this.DlgHideBusyIndicator();
                            IsLoading = false;
                            //Globals.IsBusy = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }

        public void PharmaceuticalCompany_Delete(long PCOID)
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            this.DlgShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            IsLoading = true;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacySuppliersServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginPharmaceuticalCompany_Delete(PCOID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndPharmaceuticalCompany_Delete(asyncResult);
                            Globals.ShowMessage(eHCMSResources.K0537_G1_XoaOk, eHCMSResources.G0442_G1_TBao);
                            btnSearch();
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                            _logger.Info(ex.Message);
                        }
                        finally
                        {
                            this.DlgHideBusyIndicator();
                            IsLoading = false;
                            //Globals.IsBusy = false;
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
            PharmaceuticalCompanies.PageIndex = 0;
            SearchPharmaceuticalCompanies(0, PharmaceuticalCompanies.PageSize);
        }

        public void ViewClick(object sender, RoutedEventArgs e)
        {
            if (GridPharmaceuticalCompanies != null && GridPharmaceuticalCompanies.SelectedItem != null)
            {
                Content = UPDATE;
                Contenttitle = UPDATETITLE;

                Visibility = Visibility.Visible;
                SelectedPharmaceuticalCompany = (GridPharmaceuticalCompanies.SelectedItem as PharmaceuticalCompany).DeepCopy();
            }
        }

        public void DeletedClick(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show(eHCMSResources.A0168_G1_Msg_ConfXoaNSX, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                if(GridPharmaceuticalCompanies.SelectedItem !=null)
                    PharmaceuticalCompany_Delete(((PharmaceuticalCompany)GridPharmaceuticalCompanies.SelectedItem).PCOID);
            }
        }

        public void GridPharmaceuticalCompanies_DblClick(object sender,  aEMR.Common.EventArgs<object> e)
        {
            //neu la childwindow thi phat ra su kien ben duoi
            if (IsChildWindow)
            {
                TryClose();
                switch (ePharmacieucalCompany)
                {
                    case (eFirePharmacieucalCompanyEvent.EstimationPharmacy):
                        {
                            Globals.EventAggregator.Publish(new PharmaceuticalCompanyToEstimationEvent { SelectedPharmaceuticalCompany = e.Value });
                            break;
                        }
                    default:
                        {
                            Globals.EventAggregator.Publish(new PharmacyCloseSearchPharmaceuticalCompanyEvent { SelectedPharmaceuticalCompany = e.Value });
                            break;
                        }
                }
                
            
            }
            else
            {
                //open khung chinh sua
                SelectedPharmaceuticalCompany = (e.Value as PharmaceuticalCompany).DeepCopy();
                if (SelectedPharmaceuticalCompany != null)
                {
                    Content = UPDATE;
                    Contenttitle = UPDATETITLE;

                    Visibility = Visibility.Visible;
                }
            }
        }

        public void LoadData(object sender, EventArgs e)
        {
            btnSearch();
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
            InitSelectedPharmaceuticalCompany();
            Visibility = Visibility.Collapsed;
        }
        public bool CheckValid(object temp)
        {
            PharmaceuticalCompany u = temp as PharmaceuticalCompany;
            if (u == null)
            {
                return false;
            }
            return u.Validate();
        }
        public void btnAddOrUpdate()
        {
            if (CheckValid(SelectedPharmaceuticalCompany))
            {
                if (Content == ADD)
                {
                    PharmaceuticalCompany_Insert(SelectedPharmaceuticalCompany);
                }
                else
                {
                    PharmaceuticalCompany_Update(SelectedPharmaceuticalCompany);
                }
            }
        }
        public void hlbAdd()
        {
            Visibility = Visibility.Visible;
        }
    }
}

