using eHCMSLanguage;
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
using System.Linq;
using aEMR.Common.Printing;
using aEMR.ReportModel.ReportModels;
using System.Collections.Generic;
using aEMR.CommonTasks;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using DevExpress.ReportServer.Printing;
using DevExpress.XtraReports.UI;
using DevExpress.Xpf.Printing;
using System.IO;

namespace aEMR.StoreDept.Reports.ViewModels
{
    [Export(typeof(IClinicDeptNhapXuatTon)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class ClinicNhapXuatTonViewModel : Conductor<object>, IClinicDeptNhapXuatTon
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public ClinicNhapXuatTonViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;
            Authorization();
            //Coroutine.BeginExecute(DoGetStore_DrugDept());
            FromDate = Globals.GetCurServerDateTime();
            ToDate = Globals.GetCurServerDateTime();
            CheckPointDate = Globals.GetCurServerDateTime();
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            Globals.EventAggregator.Subscribe(this);
            StoreCbx = Globals.checkStoreWareHouse(V_MedProductType, false, false);
            if (StoreCbx == null || StoreCbx.Count < 1)
            {
                MessageBox.Show(eHCMSResources.A0110_G1_Msg_InfoChuaCauHinhTNKho);
            }
        }

        private long _V_MedProductType;
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

        private bool _CanSelectedRefGenDrugCatID_1;
        public bool CanSelectedRefGenDrugCatID_1
        {
            get
            {
                return _CanSelectedRefGenDrugCatID_1;
            }
            set
            {
                if (_CanSelectedRefGenDrugCatID_1 != value)
                {
                    _CanSelectedRefGenDrugCatID_1 = value;
                    NotifyOfPropertyChange(() => CanSelectedRefGenDrugCatID_1);
                }
            }
        }

        private string _StrHienThi = Globals.PageName;
        public string StrHienThi
        {
            get
            {
                return _StrHienThi;
            }
            set
            {
                if (_StrHienThi != value)
                {
                    _StrHienThi = value;
                    NotifyOfPropertyChange(() => StrHienThi);
                }
            }
        }

        private bool _IsGetValue = false;
        public bool IsGetValue
        {
            get
            {
                return _IsGetValue;
            }
            set
            {
                if (_IsGetValue != value)
                {
                    _IsGetValue = value;
                    NotifyOfPropertyChange(() => IsGetValue);
                }
            }
        }

        #region Properties Member

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

        private DateTime? _FromDate;
        public DateTime? FromDate
        {
            get { return _FromDate; }
            set
            {
                _FromDate = value;
                NotifyOfPropertyChange(() => FromDate);
            }
        }

        private DateTime? _ToDate;
        public DateTime? ToDate
        {
            get { return _ToDate; }
            set
            {
                _ToDate = value;
                NotifyOfPropertyChange(() => ToDate);
            }
        }

        private DateTime _CheckPointDate;
        public DateTime CheckPointDate
        {
            get { return _CheckPointDate; }
            set
            {
                _CheckPointDate = value;
                NotifyOfPropertyChange(() => CheckPointDate);
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

        private bool _ViewBefore20150331;
        public bool ViewBefore20150331
        {
            get { return _ViewBefore20150331; }
            set
            {
                _ViewBefore20150331 = value;
                NotifyOfPropertyChange(() => ViewBefore20150331);
            }
        }

        private long _RefGenDrugCatID_1;
        public long RefGenDrugCatID_1
        {
            get { return _RefGenDrugCatID_1; }
            set
            {
                _RefGenDrugCatID_1 = value;
                NotifyOfPropertyChange(() => RefGenDrugCatID_1);
            }
        }

        private ObservableCollection<RefGenericDrugCategory_1> _RefGenericDrugCategory_1s;
        public ObservableCollection<RefGenericDrugCategory_1> RefGenericDrugCategory_1s
        {
            get
            {
                return _RefGenericDrugCategory_1s;
            }
            set
            {
                if (_RefGenericDrugCategory_1s != value)
                {
                    _RefGenericDrugCategory_1s = value;
                    NotifyOfPropertyChange(() => RefGenericDrugCategory_1s);
                }
            }
        }
        #endregion

        public void Authorization()
        {
            //if (!Globals.isAccountCheck)
            //{
            //    return;
            //}
        }

        #region checking account
        private bool _mBaoCaoXuatNhapTon_XemIn = true;
        private bool _mBaoCaoXuatNhapTon_KetChuyen = true;

        public bool mBaoCaoXuatNhapTon_XemIn
        {
            get
            {
                return _mBaoCaoXuatNhapTon_XemIn;
            }
            set
            {
                if (_mBaoCaoXuatNhapTon_XemIn == value)
                    return;
                _mBaoCaoXuatNhapTon_XemIn = value;
                NotifyOfPropertyChange(() => mBaoCaoXuatNhapTon_XemIn);
            }
        }

        public bool mBaoCaoXuatNhapTon_KetChuyen
        {
            get
            {
                return _mBaoCaoXuatNhapTon_KetChuyen;
            }
            set
            {
                if (_mBaoCaoXuatNhapTon_KetChuyen == value)
                    return;
                _mBaoCaoXuatNhapTon_KetChuyen = value;
                NotifyOfPropertyChange(() => mBaoCaoXuatNhapTon_KetChuyen);
            }
        }

        #endregion

        public void LoadRefGenericDrugCategory_1()
        {
            Coroutine.BeginExecute(DoGetRefGenericDrugCategory_1List());
        }

        private IEnumerator<IResult> DoGetRefGenericDrugCategory_1List()
        {
            var paymentTypeTask = new LoadRefGenericDrugCategory_1ListTask(V_MedProductType, false, true);
            yield return paymentTypeTask;
            RefGenericDrugCategory_1s = paymentTypeTask.RefGenericDrugCategory_1List;
            SetDefultRefGenericDrugCategory();
            yield break;
        }

        private void SetDefultRefGenericDrugCategory()
        {
            if (RefGenericDrugCategory_1s != null)
            {
                RefGenDrugCatID_1 = RefGenericDrugCategory_1s.FirstOrDefault().RefGenDrugCatID_1;
            }
        }

        private IEnumerator<IResult> DoGetStore_DrugDept()
        {
            var paymentTypeTask = new LoadStoreListTask((long)AllLookupValues.StoreType.STORAGE_CLINIC, false, null, false, false);
            yield return paymentTypeTask;
            StoreCbx = paymentTypeTask.LookupList;
            yield break;
        }

        public void Btn_View(object sender, RoutedEventArgs e)
        {
            if (CheckData())
            {
                DevExpress.DocumentServices.ServiceModel.DefaultValueParameterContainer rParams = new DevExpress.DocumentServices.ServiceModel.DefaultValueParameterContainer();
                string ReportTitle = "";

                if (V_MedProductType == (long)AllLookupValues.MedProductType.THUOC)
                {
                    if (RefGenDrugCatID_1 == (long)AllLookupValues.RefGenDrugCatID_1.GAYNGHIEN)
                    {
                        ReportTitle = eHCMSResources.N0227_G1_NXTThuocGN.ToUpper();
                    }
                    else if (RefGenDrugCatID_1 == (long)AllLookupValues.RefGenDrugCatID_1.HUONGTHAN)
                    {
                        ReportTitle = eHCMSResources.N0228_G1_NXTThuocHTT.ToUpper();
                    }
                    else
                    {
                        ReportTitle = eHCMSResources.N0226_G1_NXTThuoc.ToUpper();
                    }

                }
                else if (V_MedProductType == (long)AllLookupValues.MedProductType.Y_CU)
                {
                    ReportTitle = eHCMSResources.N0229_G1_NXTYCu.ToUpper();
                }
                else if (V_MedProductType == (long)AllLookupValues.MedProductType.NUTRITION)
                {
                    ReportTitle = eHCMSResources.Z3221_G1_NXTDDuong.ToUpper();
                }
                else if (V_MedProductType == (long)AllLookupValues.MedProductType.HOA_CHAT)
                {
                    ReportTitle = eHCMSResources.N0224_G1_NXTHoaChat.ToUpper();
                }

                ReportModel = null;
                if (IsGetValue)
                {
                    ReportModel = new ClinicDeptInOutStockValueReportModal().PreviewModel;
                }
                else
                {
                    ReportModel = new ClinicDeptInOutStocksReportModal().PreviewModel;
                    rParams["ViewBefore20150331"].Value = ViewBefore20150331;
                    rParams["ReportTitle"].Value = ReportTitle;
                }
                rParams["FromDate"].Value = FromDate;
                rParams["ToDate"].Value = ToDate;
                rParams["StorageName"].Value = StorageName;
                rParams["StoreID"].Value = StoreID;
                rParams["DateShow"].Value = DateShow;
                rParams["V_MedProductType"].Value = V_MedProductType;
                rParams["RefGenDrugCatID_1"].Value = RefGenDrugCatID_1;
                rParams["parLogoUrl"].Value = Globals.ServerConfigSection.CommonItems.ReportLogoUrl;

                if (Globals.ServerConfigSection.CommonItems.AllowSearchInReport)
                {
                    this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
                    if (IsGetValue)
                    {
                        var t = new Thread(() =>
                        {
                            try
                            {
                                using (var serviceFactory = new ReportServiceClient())
                                {
                                    var contract = serviceFactory.ServiceInstance;
                                    contract.BeginGetXRptInOutStockValueClinicDept_TV(
                                        ReportTitle, FromDate, ToDate, StorageName, StoreID
                                        , DateShow, V_MedProductType, RefGenDrugCatID_1
                                        , 0
                                        , Globals.ServerConfigSection.CommonItems.ReportHospitalName
                                        , Globals.ServerConfigSection.CommonItems.ReportLogoUrl
                                        , 0
                                        , Globals.DispatchCallback((asyncResult) =>
                                        {
                                            try
                                            {
                                                var results = contract.EndGetXRptInOutStockValueClinicDept_TV(asyncResult);
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
                                    contract.BeginGetXRptInOutStockClinicDept_TV(
                                        ReportTitle, FromDate, ToDate, StorageName, StoreID
                                        , DateShow, V_MedProductType, RefGenDrugCatID_1, ViewBefore20150331
                                        , Globals.ServerConfigSection.CommonItems.ReportLogoUrl
                                        , Globals.DispatchCallback((asyncResult) =>
                                        {
                                            try
                                            {
                                                var results = contract.EndGetXRptInOutStockClinicDept_TV(asyncResult);
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

        private long StoreID = 0;
        private bool CheckData()
        {
            if (FromDate == null || ToDate == null)
            {
                MessageBox.Show(eHCMSResources.K0366_G1_ChonNgThCanXem);
                return false;
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
                StorageName = "Tổng kho";
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
                CheckPointDate = CheckPointDate.Date.AddHours(23).AddMinutes(59).AddSeconds(59);

                if (MessageBox.Show(eHCMSResources.Z0891_G1_KgConLoNaoKhac + CheckPointDate.ToShortDateString(), eHCMSResources.G2363_G1_XNhan, MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                {
                    return;
                }

                this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
                var t = new Thread(() =>
                {
                    try
                    {
                        using (var serviceFactory = new PharmacyMedDeptServiceClient())
                        {
                            var contract = serviceFactory.ServiceInstance;
                            contract.BeginKetChuyenTonKho_ClinicDept(Store.StoreID, GetStaffLogin().StaffID, CheckPointName, V_MedProductType, CheckPointDate, Globals.DispatchCallback((asyncResult) =>
                              {
                                  try
                                  {
                                      contract.EndKetChuyenTonKho_ClinicDept(asyncResult);
                                      Globals.ShowMessage(eHCMSResources.Z1773_G1_DaKCXong, eHCMSResources.G0442_G1_TBao);
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
                        this.HideBusyIndicator();
                        MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                    }
                });

                t.Start();
            }
        }
    }
}
