using System.ComponentModel.Composition;
using aEMR.ViewContracts;
using Caliburn.Micro;
using aEMR.Infrastructure;
using System.Collections.ObjectModel;
using DataEntities;
using System.Windows;
using aEMR.Infrastructure.Events;
using System.Threading;
using aEMR.ServiceClient;
using System;
using aEMR.Common.Printing;
using DevExpress.Xpf.Printing;
using aEMR.ReportModel.ReportModels;
using System.Collections.Generic;
using aEMR.CommonTasks;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using eHCMSLanguage;
using DevExpress.ReportServer.Printing;
using System.IO;
using DevExpress.XtraReports.UI;

namespace aEMR.Pharmacy.ViewModels
{
    [Export(typeof(INhapXuatTon)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class NhapXuatTonViewModel : Conductor<object>, INhapXuatTon
    {
        public string TitleForm { get; set; }
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public NhapXuatTonViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;
            Authorization();
            Coroutine.BeginExecute(DoGetStore_EXTERNAL());
        }

        #region Properties Member

        private ObservableCollection<RefStorageWarehouseLocation> _StoreCbx;
        public ObservableCollection<RefStorageWarehouseLocation> StoreCbx
        {
            get
            {
                return _StoreCbx;
            }
            set
            {
                if (_StoreCbx != value)
                {
                    _StoreCbx = value;
                    NotifyOfPropertyChange(() => StoreCbx);
                }
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

        private DateTime? _FromDate=Globals.ServerDate;
        public DateTime? FromDate
        {
            get { return _FromDate; }
            set 
            {
                _FromDate = value;
                NotifyOfPropertyChange(()=>FromDate);
            }
        }
        private DateTime? _ToDate = Globals.ServerDate;
        public DateTime? ToDate
        {
            get { return _ToDate; }
            set
            {
                _ToDate = value;
                NotifyOfPropertyChange(() => ToDate);
            }
        }

   
        private RefStorageWarehouseLocation _Store;
        public RefStorageWarehouseLocation Store
        {
            get { return _Store; }
            set
            {
                _Store = value;
                NotifyOfPropertyChange(() => Store);
            }
        }
        private string DateShow;
        private string StorageName;

        private string _CheckPointName;
        public string CheckPointName
        {
            get { return _CheckPointName; }
            set
            {
                _CheckPointName = value;
                NotifyOfPropertyChange(() => CheckPointName);
            }
        }

        private bool _IsCheck;
        public bool IsCheck
        {
            get { return _IsCheck; }
            set
            {
                _IsCheck = value;
                NotifyOfPropertyChange(() => IsCheck);
            }
        }
        #endregion

        public void Authorization()
        {
            if (!Globals.isAccountCheck)
            {
                return;
            }
            bXem = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPharmacy
                                               , (int)ePharmacy.mBCKhac_NhapXuatTon,
                                               (int)oPharmacyEx.mBCKhac_NhapXuatTon_Xem, (int)ePermission.mView);
            bIn = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPharmacy
                                               , (int)ePharmacy.mBCKhac_NhapXuatTon,
                                               (int)oPharmacyEx.mBCKhac_NhapXuatTon_In, (int)ePermission.mView);
            bKetChuyen = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPharmacy
                                               , (int)ePharmacy.mBCKhac_NhapXuatTon,
                                               (int)oPharmacyEx.mBCKhac_NhapXuatTon_KetChuyen, (int)ePermission.mView);
        }

        #region checking account

        private bool _bXem = true;
        private bool _bIn = true;
        private bool _bKetChuyen = true;


        public bool bXem
        {
            get
            {
                return _bXem;
            }
            set
            {
                if (_bXem == value)
                    return;
                _bXem = value;
            }
        }
        public bool bIn
        {
            get
            {
                return _bIn;
            }
            set
            {
                if (_bIn == value)
                    return;
                _bIn = value;
            }
        }
        public bool bKetChuyen
        {
            get
            {
                return _bKetChuyen;
            }
            set
            {
                if (_bKetChuyen == value)
                    return;
                _bKetChuyen = value;
            }
        }

        #endregion

        private bool _HasValue = false;
        public bool HasValue
        {
            get
            {
                return _HasValue;
            }
            set
            {
                if (_HasValue != value)
                {
                    _HasValue = value;
                    NotifyOfPropertyChange(() => HasValue);
                }
            }
        }

        private bool _HasNormalPrice = false;
        public bool HasNormalPrice
        {
            get
            {
                return _HasNormalPrice;
            }
            set
            {
                if (_HasNormalPrice != value)
                {
                    _HasNormalPrice = value;
                    NotifyOfPropertyChange(() => HasNormalPrice);
                }
            }
        }

        private IEnumerator<IResult> DoGetStore_EXTERNAL()
        {
            var paymentTypeTask = new LoadStoreListTask((long)AllLookupValues.StoreType.STORAGE_EXTERNAL, false, null,false, false);
            yield return paymentTypeTask;
            StoreCbx = paymentTypeTask.LookupList;
            yield break;
        }

        //▼====: #005
        private XtraReport _XtraReportModel;
        public XtraReport XtraReportModel
        {
            get { return _XtraReportModel; }
            set
            {
                _XtraReportModel = value;
                NotifyOfPropertyChange(() => XtraReportModel);
            }
        }

        private DocumentPreviewControl DocumentPreview;
        public void Report_Loaded(object sender, RoutedEventArgs e)
        {
            DocumentPreview = sender as DocumentPreviewControl;
        }
        //▲====: #005

        private long StoreID = 0;
        public void btn_View(object sender, RoutedEventArgs e)
        {
            if (CheckData())
            {
                DevExpress.DocumentServices.ServiceModel.DefaultValueParameterContainer rParams = new DevExpress.DocumentServices.ServiceModel.DefaultValueParameterContainer();
                ReportModel = null;
                DateShow = "Từ ngày " + FromDate.GetValueOrDefault().ToShortDateString() + " đến ngày " + ToDate.GetValueOrDefault().ToShortDateString();
                if(HasNormalPrice)
                {
                    ReportModel = new PharmacyInOutStock_NTReportModel().PreviewModel;
                    rParams["StoreID"].Value = StoreID;
                }
                else if (HasValue)
                {
                    ReportModel = new PharmacyInOutStockValueReportModel().PreviewModel;
                    rParams["StoreID"].Value = StoreID;
                }
                else
                {
                    ReportModel = new PharmacyInOutStocksReportModel().PreviewModel;
                    rParams["parLogoUrl"].Value = Globals.ServerConfigSection.CommonItems.ReportLogoUrl;
                    rParams["StoreID"].Value = (int)StoreID;
                }
                rParams["FromDate"].Value = FromDate;
                rParams["ToDate"].Value = ToDate;
                rParams["StorageName"].Value = StorageName;
                rParams["DateShow"].Value = DateShow;
                if (Globals.ServerConfigSection.CommonItems.AllowSearchInReport && !HasNormalPrice)
                {
                    this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
                    if (HasValue)
                    {
                        var t = new Thread(() =>
                        {
                            try
                            {
                                using (var serviceFactory = new ReportServiceClient())
                                {
                                    var contract = serviceFactory.ServiceInstance;
                                    contract.BeginGetPharmacyXRptInOutStockValue_TV(
                                        FromDate, ToDate, StorageName, StoreID, DateShow
                                        , Globals.ServerConfigSection.CommonItems.ReportLogoUrl
                                        , Globals.ServerConfigSection.CommonItems.ReportHospitalName
                                        , Globals.DispatchCallback((asyncResult) =>
                                        {
                                            try
                                            {
                                                var results = contract.EndGetPharmacyXRptInOutStockValue_TV(asyncResult);
                                                MemoryStream memoryStream = new MemoryStream(results);
                                                XtraReportModel = new XtraReport();
                                                XtraReportModel.PrintingSystem.LoadDocument(memoryStream);
                                                DocumentPreview.DocumentSource = XtraReportModel;
                                            }
                                            catch (Exception ex)
                                            {
                                                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
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
                                this.HideBusyIndicator();
                                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                            }
                        });

                        t.Start();
                    }
                    else
                    {
                        var t = new Thread(() =>
                        {
                            try
                            {
                                using (var serviceFactory = new ReportServiceClient())
                                {
                                    var contract = serviceFactory.ServiceInstance;
                                    contract.BeginGetPharmacyXRptInOutStocks_TV(
                                        FromDate, ToDate, StorageName, StoreID
                                        , DateShow, Globals.ServerConfigSection.CommonItems.ReportLogoUrl
                                        , Globals.DispatchCallback((asyncResult) =>
                                        {
                                            try
                                            {
                                                var results = contract.EndGetPharmacyXRptInOutStocks_TV(asyncResult);
                                                MemoryStream memoryStream = new MemoryStream(results);
                                                XtraReportModel = new XtraReport();
                                                XtraReportModel.PrintingSystem.LoadDocument(memoryStream);
                                                DocumentPreview.DocumentSource = XtraReportModel;
                                            }
                                            catch (Exception ex)
                                            {
                                                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
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
                                this.HideBusyIndicator();
                                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                            }
                        });

                        t.Start();
                    }                    
                }
                else
                {
                    ReportModel.CreateDocument(rParams);
                }
            }
        }

        private void PrintInOutStocksRpt(DateTime FromDate, DateTime ToDate, string StorageName, long StoreID, string dateshow)
        {
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ReportServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetInOutStocksInPdfFormat(FromDate, ToDate, StorageName, StoreID, dateshow, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndGetInOutStocksInPdfFormat(asyncResult);
                                var printEvt = new ActiveXPrintEvt(this, PrinterType.IN_PHIEU, results, ActiveXPrintType.ByteArray);
                                Globals.EventAggregator.Publish(results);
                            }
                            catch (Exception ex)
                            {
                                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
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
                    this.HideBusyIndicator();
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                }
            });

            t.Start();
        }

        private string GetStorageName(object value)
        {
            RefStorageWarehouseLocation p = value as RefStorageWarehouseLocation;
            if (p != null)
                return p.swhlName;
            else
                return "";
        }

        private bool CheckData()
        {
            if (FromDate == null || ToDate == null)
            {

                MessageBox.Show(eHCMSResources.K0366_G1_ChonNgThCanXem);
                return false;
            }
            if (FromDate == null || ToDate == null)
            {
                MessageBox.Show(eHCMSResources.K0366_G1_ChonNgThCanXem);
                return false;
            }
            else
            {
                if (FromDate.GetValueOrDefault() > ToDate.GetValueOrDefault())
                {
                    MessageBox.Show(eHCMSResources.A0857_G1_Msg_InfoNgThangKhHopLe2);
                    return false;
                }
            }
            if (!IsCheck)
            {
                if (Store == null)
                {
                    MessageBox.Show(eHCMSResources.K0310_G1_ChonKhoCanXem);
                    return false;
                }
                else
                {
                    StoreID = Store.StoreID;
                    StorageName = Store.swhlName;
                    DateShow = StorageName + "( " + FromDate.GetValueOrDefault().ToShortDateString() + " - " + ToDate.GetValueOrDefault().ToShortDateString() + " )";
                    return true;
                }
            }
            else
            {
                StoreID = 0;
                StorageName = eHCMSResources.Z0936_G1_TgKho;
                DateShow = StorageName + "( " + FromDate.GetValueOrDefault().ToShortDateString() + " - " + ToDate.GetValueOrDefault().ToShortDateString() + " )";
                return true;
            }
        }

        public void btn_Print(object sender, RoutedEventArgs e)
        {
            if (CheckData())
            {
                PrintInOutStocksRpt(FromDate.GetValueOrDefault(), ToDate.GetValueOrDefault(), StorageName, StoreID, DateShow);
            }
        }

        private Staff GetStaffLogin()
        {
            return Globals.LoggedUserAccount.Staff;
        }

        public void btnKetChuyenTonDauKy()
        {
            if (Store == null)
            {
                Globals.ShowMessage(eHCMSResources.Z0937_G1_ChonKhoKC, eHCMSResources.G0442_G1_TBao);
            }
            else
            {
                this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
                var t = new Thread(() =>
                {
                    try
                    {
                        using (var serviceFactory = new PharmacyInwardDrugServiceClient())
                        {
                            var contract = serviceFactory.ServiceInstance;

                            contract.BeginKetChuyenTonKho(Store.StoreID, GetStaffLogin().StaffID, CheckPointName, Globals.DispatchCallback((asyncResult) =>
                            {
                                try
                                {
                                    contract.EndKetChuyenTonKho(asyncResult);
                                    MessageBox.Show(eHCMSResources.A0615_G1_Msg_InfoKCOK);
                                }
                                catch (Exception ex)
                                {
                                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
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
                        this.HideBusyIndicator();
                        Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    }
                });

                t.Start();
            }
        }
    }
}
