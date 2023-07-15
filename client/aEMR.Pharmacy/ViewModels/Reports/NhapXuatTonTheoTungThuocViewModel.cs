using System.ComponentModel.Composition;
using aEMR.ViewContracts;
using Caliburn.Micro;
using aEMR.Infrastructure;
using DataEntities;
using System.Windows;
using aEMR.Infrastructure.Events;
using System.Threading;
using aEMR.ServiceClient;
using System;
using aEMR.Common.Collections;
using aEMR.Common.Printing;
using DevExpress.Xpf.Printing;
using aEMR.ReportModel.ReportModels;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using System.Windows.Controls;
using eHCMSLanguage;
using DevExpress.ReportServer.Printing;

namespace aEMR.Pharmacy.ViewModels
{
    [Export(typeof(INhapXuatTonTheoTungThuoc)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class NhapXuatTonTheoTungThuocViewModel: Conductor<object>, INhapXuatTonTheoTungThuoc
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public NhapXuatTonTheoTungThuocViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;
            authorization();
            RefGenericDrugDetails = new PagedSortableCollectionView<RefGenericDrugDetail>();
            RefGenericDrugDetails.OnRefresh += RefGenericDrugDetails_OnRefresh;
            RefGenericDrugDetails.PageSize = Globals.PageSize;
        }

        void RefGenericDrugDetails_OnRefresh(object sender, RefreshEventArgs e)
        {
            SearchRefDrugGenericDetails_AutoPaging(BrandName, RefGenericDrugDetails.PageIndex, RefGenericDrugDetails.PageSize);
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

        #region Properties Member

        private RefGenericDrugDetail _CurrentRefGenericDrugDetail;
        public RefGenericDrugDetail CurrentRefGenericDrugDetail
        {
            get
            {
                return _CurrentRefGenericDrugDetail;
            }
            set
            {
                if (_CurrentRefGenericDrugDetail != value)
                {
                    _CurrentRefGenericDrugDetail = value;
                }
                NotifyOfPropertyChange(() => CurrentRefGenericDrugDetail);
            }
        }

        private RemoteDocumentSource _reportModel;
        public RemoteDocumentSource ReportModel
        {
            get { return _reportModel; }
            set
            {
                _reportModel = value;
                NotifyOfPropertyChange(() => ReportModel);
            }
        }

        private DateTime? _FromDate;
        public DateTime? FromDate
        {
            get
            {
                return _FromDate;
            }
            set
            {
                if (_FromDate != value)
                {
                    _FromDate = value;
                    NotifyOfPropertyChange(()=>FromDate);
                }
            }
        }

        private DateTime? _ToDate;
        public DateTime? ToDate
        {
            get
            {
                return _ToDate;
            }
            set
            {
                if (_ToDate != value)
                {
                    _ToDate = value;
                    NotifyOfPropertyChange(() => ToDate);
                }
            }
        }

        #endregion

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

        public void btn_View(object sender, RoutedEventArgs e)
        {
            if (CheckData())
            {
                DevExpress.DocumentServices.ServiceModel.DefaultValueParameterContainer rParams = new DevExpress.DocumentServices.ServiceModel.DefaultValueParameterContainer();
                ReportModel = null;
                ReportModel = new PharmacyStockByEveryStorage().PreviewModel;
                rParams["DrugID"].Value = (int)CurrentRefGenericDrugDetail.DrugID;
                rParams["DrugName"].Value = CurrentRefGenericDrugDetail.BrandName;
                rParams["UnitName"].Value = CurrentRefGenericDrugDetail.SeletedUnit.UnitName;
                rParams["FromDate"].Value = FromDate;
                rParams["ToDate"].Value = ToDate;
                rParams["DateShow"].Value = ShowDate;
                // ReportModel.AutoShowParametersPanel = false;
               
                // ReportModel.AutoShowParametersPanel = false;
                ReportModel.CreateDocument(rParams);
            }
        }

        private void PrintGroupStorageRpt(long DrugID, string DrugName, string UnitName, DateTime Fromdate, DateTime ToDate, string dateshow)
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            IsLoading = true;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new ReportServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginGetGroupStorageInPdfFormat(DrugID, DrugName, UnitName, Fromdate, ToDate, ShowDate, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndGetGroupStorageInPdfFormat(asyncResult);
                            var printEvt = new ActiveXPrintEvt(this, PrinterType.IN_PHIEU, results, ActiveXPrintType.ByteArray);
                            Globals.EventAggregator.Publish(results);
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
        string ShowDate = "";
        private bool CheckData()
        {
            if (FromDate == null || ToDate == null)
            {
                MessageBox.Show(eHCMSResources.K0366_G1_ChonNgThCanXem);
                return false;
            }
            if (CurrentRefGenericDrugDetail == null)
            {
                MessageBox.Show(eHCMSResources.K0404_G1_ChonThuocCanXem);
                return false;
            }
            else
            {
                ShowDate="( " + FromDate.Value.ToString("dd/MM/yyyy") + " - " + ToDate.Value.ToString("dd/MM/yyyy") + " )"; 
                return true;
            }
        }

        public void btn_Print(object sender, RoutedEventArgs e)
        {
            if (CheckData())
            {
                PrintGroupStorageRpt(CurrentRefGenericDrugDetail.DrugID, CurrentRefGenericDrugDetail.BrandName, CurrentRefGenericDrugDetail.SeletedUnit.UnitName,FromDate.Value,ToDate.Value,ShowDate);
            }
        }

      
        #region Auto for Drug Member

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
        #endregion

    }
}
