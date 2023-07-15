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
using aEMR.Common.Collections;
using aEMR.Common.Printing;
using aEMR.ReportModel.ReportModels;
using System.Windows.Controls;
using System.Collections.Generic;
using aEMR.CommonTasks;
using aEMR.Controls;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using System.Linq;
using DevExpress.ReportServer.Printing;
using DevExpress.XtraReports.UI;
using DevExpress.Xpf.Printing;
using System.IO;

/*
 * 20190503 #001 TNHX: [BM0006813] Apply TKTheoDoiNXTThuocKhac_NT
 * 20200218 #002 TNHX: [] Thêm điều kiện lọc danh mục NT + Kho BHYT Ngoại trú
 * 20200831 #003 TNHX: [] Thêm điều kiện lọc tên thuốc cho báo cáo TKTheoDoiNXTThuocKhac_NT
 */
namespace aEMR.Pharmacy.ViewModels
{
    [Export(typeof(ITheKho)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class TheKhoViewModel : Conductor<object>, ITheKho
    {
        public string TitleForm { get; set; }
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public TheKhoViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            Authorization();

            Coroutine.BeginExecute(DoGetStore_EXTERNAL());

            RefGenericDrugDetails = new PagedSortableCollectionView<RefGenericDrugSimple>();
            RefGenericDrugDetails.OnRefresh += RefGenericDrugDetails_OnRefresh;
            RefGenericDrugDetails.PageSize = Globals.PageSize;
        }

        void RefGenericDrugDetails_OnRefresh(object sender, RefreshEventArgs e)
        {
            SearchRefDrugGenericDetails_AutoPaging(null,BrandName, RefGenericDrugDetails.PageIndex, RefGenericDrugDetails.PageSize);
        }

        public void Authorization()
        {
            if (!Globals.isAccountCheck)
            {
                return;
            }
            bXem = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPharmacy
                                               , (int)ePharmacy.mBCNhap_TheKho,
                                               (int)oPharmacyEx.mBCNhap_TheKho_Xem, (int)ePermission.mView);
            bIn = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPharmacy
                                               , (int)ePharmacy.mBCNhap_TheKho,
                                               (int)oPharmacyEx.mBCNhap_TheKho_In, (int)ePermission.mView);
        }

        #region checking account

        private bool _bXem = true;
        private bool _bIn = true;
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
        
        #endregion

        #region Properties Member

        private RefGenericDrugSimple _CurrentRefGenericDrugDetail;
        public RefGenericDrugSimple CurrentRefGenericDrugDetail
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

        private RefStorageWarehouseLocation _CurrentStore;
        public RefStorageWarehouseLocation CurrentStore
        {
            get
            {
                return _CurrentStore;
            }
            set
            {
                if (_CurrentStore != value)
                {
                    _CurrentStore = value;
                    NotifyOfPropertyChange(() => CurrentStore);
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

        private DateTime? _FromDate = Globals.ServerDate.Value.Date;
        public DateTime? FromDate
        {
            get
            {
                return _FromDate;
            }
            set
            {
                _FromDate = value;
                NotifyOfPropertyChange(()=>FromDate);
            }
        }

        private DateTime? _ToDate = Globals.ServerDate.Value.Date;
        public DateTime? ToDate
        {
            get
            {
                return _ToDate;
            }
            set
            {
                _ToDate = value;
                NotifyOfPropertyChange(() => ToDate);
            }
        }

        bool IsTongKho = false;
        string StoreName;
        long StoreID;

        //▼====: #001
        private ReportName _eItem;
        public ReportName eItem
        {
            get
            {
                return _eItem;
            }
            set
            {
                _eItem = value;
                NotifyOfPropertyChange(() => eItem);
            }
        }
        //▲====: #001

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
        #endregion

        private IEnumerator<IResult> DoGetStore_EXTERNAL()
        {
            var paymentTypeTask = new LoadStoreListTask((long)AllLookupValues.StoreType.STORAGE_EXTERNAL, false,null, false, false);
            yield return paymentTypeTask;
            StoreCbx = paymentTypeTask.LookupList;
            yield break;
        }

        //▼====: #001
        public void btn_View(object sender, RoutedEventArgs e)
        {
            if (CheckData())
            {
                DevExpress.DocumentServices.ServiceModel.DefaultValueParameterContainer rParams = new DevExpress.DocumentServices.ServiceModel.DefaultValueParameterContainer();
                switch (_eItem)
                {
                    case ReportName.TKTheoDoiNXTThuocKhac_NT:
                        ReportModel = null;
                        ReportModel = new XRpt_TKTheoDoiNXTThuocKhac_NT().PreviewModel;
                        rParams["DrugName"].Value = CurrentRefGenericDrugDetail.BrandName;
                        rParams["DrugID"].Value = (int)CurrentRefGenericDrugDetail.DrugID;
                        //rParams["DrugCode"].Value = CurrentRefGenericDrugDetail.DrugCode;
                        //rParams["StorageName"].Value = StoreName;
                        rParams["StoreID"].Value = (int)StoreID;
                        rParams["UnitName"].Value = CurrentRefGenericDrugDetail.UnitName;
                        rParams["FromDate"].Value = FromDate;
                        rParams["ToDate"].Value = ToDate;
                        break;
                    //▼====: #003
                    case ReportName.TKTheoDoiTTChiTietKH_NTTheoThuoc:
                        ReportModel = null;
                        ReportModel = new XRpt_TKTheoDoiTTChiTietKH_NT().PreviewModel;
                        rParams["DrugID"].Value = CurrentRefGenericDrugDetail.DrugID;
                        rParams["StoreID"].Value = (int)StoreID;
                        rParams["FromDate"].Value = FromDate;
                        rParams["ToDate"].Value = ToDate;
                        rParams["parHospitalName"].Value = Globals.ServerConfigSection.CommonItems.ReportHospitalName;
                        break;
                    //▲====: #003
                    default:
                        ReportModel = null;
                        ReportModel = new PharmacyCardStorageReportModal().PreviewModel;
                        rParams["DrugName"].Value = CurrentRefGenericDrugDetail.BrandName;
                        rParams["DrugID"].Value = (int)CurrentRefGenericDrugDetail.DrugID;
                        rParams["DrugCode"].Value = CurrentRefGenericDrugDetail.DrugCode;
                        rParams["StorageName"].Value = StoreName;
                        rParams["StoreID"].Value = (int)StoreID;
                        rParams["UnitName"].Value = CurrentRefGenericDrugDetail.UnitName;
                        rParams["FromDate"].Value = FromDate;
                        rParams["ToDate"].Value = ToDate;
                        break;
                }

                if(_eItem == ReportName.None)
                {
                    this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
                    var dateshow = string.Format("{0} :", eHCMSResources.G1933_G1_TuNg) + FromDate.GetValueOrDefault().ToString("dd/MM/yyyy") + string.Format(" _ {0} :", eHCMSResources.K3192_G1_DenNg) + ToDate.GetValueOrDefault().ToString("dd/MM/yyyy");
                    var t = new Thread(() =>
                    {
                        try
                        {
                            using (var serviceFactory = new ReportServiceClient())
                            {
                                var contract = serviceFactory.ServiceInstance;
                                contract.BeginGetXRptPharmacyCardStorage((int)StoreID
                                    , (int)CurrentRefGenericDrugDetail.DrugID
                                    , CurrentRefGenericDrugDetail.BrandName
                                    , StoreName, CurrentRefGenericDrugDetail.UnitName
                                    , FromDate, ToDate, dateshow, CurrentRefGenericDrugDetail.DrugCode
                                    , Globals.ServerConfigSection.CommonItems.ReportLogoUrl
                                    , Globals.DispatchCallback((asyncResult) =>
                                    {
                                        try
                                        {
                                            var results = contract.EndGetXRptPharmacyCardStorage(asyncResult);
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
                    ReportModel.CreateDocument(rParams);
                }
                // ReportModel.AutoShowParametersPanel = false;
            }
        }
        //▲====: #001

        private void PrintCardStorageRpt(long DrugID, string DrugName, long StoreID, string StorageName, string UnitName)
        {
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ReportServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetCardStorageInPdfFormat(DrugID, DrugName, StoreID, StorageName, UnitName, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndGetCardStorageInPdfFormat(asyncResult);
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

        private bool CheckData()
        {
            if (CurrentRefGenericDrugDetail == null)
            {
                MessageBox.Show(eHCMSResources.K0404_G1_ChonThuocCanXem);
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
            if (!IsTongKho)
            {
                if (CurrentStore == null)
                {
                    MessageBox.Show(eHCMSResources.K0310_G1_ChonKhoCanXem);
                    return false;
                }
                else
                {
                    StoreName = CurrentStore.swhlName;
                    StoreID = CurrentStore.StoreID;
                    return true;
                }
            }
            else
            {
                StoreName = eHCMSResources.Z0936_G1_TgKho;
                StoreID = 0;
                return true;
            }
        }

        public void btn_Print(object sender, RoutedEventArgs e)
        {
            if (CheckData())
            {
                PrintCardStorageRpt(CurrentRefGenericDrugDetail.DrugID, CurrentRefGenericDrugDetail.BrandName, StoreID, StoreName, CurrentRefGenericDrugDetail.UnitName);
            }
        }

        public void chk_TongKho_Checked(object sender, RoutedEventArgs e)
        {
            IsTongKho = true;
        }

        public void chk_TongKho_Unchecked(object sender, RoutedEventArgs e)
        {
            IsTongKho = false;
        }
        #region Auto for Drug Member

        private string BrandName;

        private PagedSortableCollectionView<RefGenericDrugSimple> _RefGenericDrugDetails;
        public PagedSortableCollectionView<RefGenericDrugSimple> RefGenericDrugDetails
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

        private void SearchRefDrugGenericDetails_AutoPaging(bool? IsCode, string Name, int PageIndex, int PageSize)
        {
            //this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyDrugServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginSearchRefGenericDrugName_SimpleAutoPaging(IsCode, Name, PageIndex, PageSize, false, Globals.DispatchCallback((asyncResult) =>
                         {
                             try
                             {
                                 var ListUnits = contract.EndSearchRefGenericDrugName_SimpleAutoPaging(out int totalCount, asyncResult);
                                 if (IsCode.GetValueOrDefault())
                                 {
                                     if (ListUnits != null && ListUnits.Count > 0)
                                     {
                                         CurrentRefGenericDrugDetail = ListUnits.FirstOrDefault();
                                     }
                                     else
                                     {
                                         CurrentRefGenericDrugDetail = null;

                                         if (au != null)
                                         {
                                             au.Text = "";
                                         }
                                         MessageBox.Show(eHCMSResources.A0752_G1_Msg_InfoKhTimThay);
                                     }
                                 }
                                 else
                                 {
                                     if (ListUnits != null)
                                     {
                                         RefGenericDrugDetails.Clear();
                                         RefGenericDrugDetails.TotalItemCount = totalCount;
                                         RefGenericDrugDetails.ItemCount = totalCount;
                                        //foreach (RefGenericDrugSimple p in ListUnits)
                                        //{
                                        //    RefGenericDrugDetails.Add(p);
                                        //}
                                        RefGenericDrugDetails.SourceCollection = ListUnits;
                                        NotifyOfPropertyChange(() => RefGenericDrugDetails);
                                     }
                                     au.ItemsSource = RefGenericDrugDetails;
                                     au.PopulateComplete();
                                 }
                             }
                             catch (Exception ex)
                             {
                                 Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                             }
                             finally
                             {
                                 //this.HideBusyIndicator();
                             }
                         }), null);
                    }
                }
                catch (Exception ex)
                {
                    //this.HideBusyIndicator();
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
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
            SearchRefDrugGenericDetails_AutoPaging(null,e.Parameter, 0, RefGenericDrugDetails.PageSize);
        }
        #endregion

        public void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            AxTextBox obj = sender as AxTextBox;
            if (obj != null && !string.IsNullOrEmpty(obj.Text))
            {
                if (CurrentRefGenericDrugDetail != null)
                {
                    if (CurrentRefGenericDrugDetail.DrugCode != obj.Text)
                    {
                        SearchRefDrugGenericDetails_AutoPaging(true, obj.Text, 0, RefGenericDrugDetails.PageSize);
                    }
                }
                else
                {
                    SearchRefDrugGenericDetails_AutoPaging(true, obj.Text, 0, RefGenericDrugDetails.PageSize);
                }
            }
        }

        public void AutoDrug_Text_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            RefGenericDrugSimple obj = (sender as AutoCompleteBox).SelectedItem as RefGenericDrugSimple;
            if (CurrentRefGenericDrugDetail != null)
            {
                if (CurrentRefGenericDrugDetail.BrandName != obj.BrandName)
                {
                    CurrentRefGenericDrugDetail = obj;
                }
            }
        }
    }
}
