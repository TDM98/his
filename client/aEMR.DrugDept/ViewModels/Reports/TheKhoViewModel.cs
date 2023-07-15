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
using DevExpress.Xpf.Printing;
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
using System.IO;
using DevExpress.XtraReports.UI;
/*
* 20211103 #001 QTD:   Lọc kho theo cấu hình trách nhiệm
*/
namespace aEMR.DrugDept.ViewModels
{
    [Export(typeof(IDrugDeptTheKho)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class TheKhoViewModel : Conductor<object>, IDrugDeptTheKho
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public TheKhoViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;
            authorization();
            
            RefGenMedProductDetailss = new PagedSortableCollectionView<RefGenMedProductSimple>();
            RefGenMedProductDetailss.OnRefresh += RefGenMedProductDetailss_OnRefresh;
            RefGenMedProductDetailss.PageSize = Globals.PageSize;
        }
        protected override void OnActivate()
        {
            base.OnActivate();
            Globals.EventAggregator.Subscribe(this);
            Coroutine.BeginExecute(DoGetStore_DrugDept());
        }
        void RefGenMedProductDetailss_OnRefresh(object sender, RefreshEventArgs e)
        {
            SearchRefDrugGenericDetails_AutoPaging(null, BrandName, RefGenMedProductDetailss.PageIndex, RefGenMedProductDetailss.PageSize);
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

        public void authorization()
        {
        }

        public long V_MedProductType { get; set; }

        public string strHienThi { get; set; }


        #region checking account

        private bool _mXem = true;
        private bool _mIn = true;
        public bool mXem
        {
            get
            {
                return _mXem;
            }
            set
            {
                if (_mXem == value)
                    return;
                _mXem = value;
                NotifyOfPropertyChange(() => mXem);
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
        #region Properties Member

        private RefGenMedProductSimple _CurrentRefGenMedProductDetails;
        public RefGenMedProductSimple CurrentRefGenMedProductDetails
        {
            get
            {
                return _CurrentRefGenMedProductDetails;
            }
            set
            {
                if (_CurrentRefGenMedProductDetails != value)
                {
                    _CurrentRefGenMedProductDetails = value;
                }
                NotifyOfPropertyChange(() => CurrentRefGenMedProductDetails);
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
        private DateTime? _FromDate = DateTime.Now.Date;
        public DateTime? FromDate
        {
            get
            {
                return _FromDate;
            }
            set
            {
                _FromDate = value;
                NotifyOfPropertyChange(() => FromDate);
            }
        }
        private DateTime? _ToDate = DateTime.Now.Date;
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
        #endregion

        private IEnumerator<IResult> DoGetStore_DrugDept()
        {
            var paymentTypeTask = new LoadStoreListTask((long)AllLookupValues.StoreType.STORAGE_DRUGDEPT, false,null, false, false);
            yield return paymentTypeTask;
            //StoreCbx = paymentTypeTask.LookupList.Where(x => (V_MedProductType != 0 && x.ListV_MedProductType != null && x.ListV_MedProductType.Contains(V_MedProductType.ToString()))).ToObservableCollection();
            //▼===== #001
            var StoreTemp = paymentTypeTask.LookupList.Where(x => (V_MedProductType != 0 && x.ListV_MedProductType != null && x.ListV_MedProductType.Contains(V_MedProductType.ToString()))).ToObservableCollection();
            StoreCbx = Globals.CheckDrugMedStoreWareHouse(StoreTemp);
            if(StoreCbx == null || StoreCbx.Count == 0)
            {
                MessageBox.Show(eHCMSResources.A0110_G1_Msg_InfoChuaCauHinhTNKho.Replace("\\n ", "\n"));
            }
            //▲===== #001
            yield break;
        }

        public void btn_View(object sender, RoutedEventArgs e)
        {
            if (CheckData())
            {
                ReportModel = null;
                ReportModel = new DrugDeptCardStorageReportModal().PreviewModel;
                DevExpress.DocumentServices.ServiceModel.DefaultValueParameterContainer rParams = new DevExpress.DocumentServices.ServiceModel.DefaultValueParameterContainer();
                //rParams["DrugName"].Value = CurrentRefGenMedProductDetails.BrandName;
                //rParams["GenMedProductID"].Value = (int)CurrentRefGenMedProductDetails.GenMedProductID;
                //rParams["StorageName"].Value = StoreName;
                //rParams["StoreID"].Value = (int)StoreID;
                //rParams["UnitName"].Value = CurrentRefGenMedProductDetails.UnitName;
                //rParams["FromDate"].Value = FromDate;
                //rParams["ToDate"].Value = ToDate;
                //rParams["ShowDate"].Value = string.Format("{0} :", eHCMSResources.G1933_G1_TuNg) + FromDate.GetValueOrDefault().ToString("dd/MM/yyyy") + string.Format(" _ {0} :", eHCMSResources.K3192_G1_DenNg) + ToDate.GetValueOrDefault().ToString("dd/MM/yyyy");
                //rParams["Code"].Value = CurrentRefGenMedProductDetails.Code;

                ////KMx: Thêm V_MedProductType để biết đang xem thẻ kho của loại nào (14/07/2014 17:23)
                //rParams["V_MedProductType"].Value = (int)V_MedProductType;
                //rParams["parLogoUrl"].Value = Globals.ServerConfigSection.CommonItems.ReportLogoUrl;

                // ReportModel.AutoShowParametersPanel = false;
                //ReportModel.CreateDocument(rParams);


                //20210806 QTD Change method view
                this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
                var dateshow = string.Format("{0} :", eHCMSResources.G1933_G1_TuNg) + FromDate.GetValueOrDefault().ToString("dd/MM/yyyy") + string.Format(" _ {0} :", eHCMSResources.K3192_G1_DenNg) + ToDate.GetValueOrDefault().ToString("dd/MM/yyyy");
                var t = new Thread(() =>
                {
                    try
                    {
                        using (var serviceFactory = new ReportServiceClient())
                        {
                            var contract = serviceFactory.ServiceInstance;
                            contract.BeginGetXRptDrugDeptCardStorage((int)StoreID
                                , CurrentRefGenMedProductDetails.GenMedProductID, (int)V_MedProductType
                                , CurrentRefGenMedProductDetails.BrandName
                                , StoreName, CurrentRefGenMedProductDetails.UnitName
                                , FromDate, ToDate, dateshow, CurrentRefGenMedProductDetails.Code
                                , Globals.ServerConfigSection.CommonItems.ReportLogoUrl
                                , Globals.DispatchCallback((asyncResult) =>
                                {
                                    try
                                    {
                                        var results = contract.EndGetXRptDrugDeptCardStorage(asyncResult);
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

        private void PrintCardStorageRpt(long GenMedProductID, string DrugName, long StoreID, string StorageName, string UnitName)
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            IsLoading = true;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new ReportServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginGetCardStorageInPdfFormat(GenMedProductID, DrugName, StoreID, StorageName, UnitName, Globals.DispatchCallback((asyncResult) =>
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
                            IsLoading = false;
                            Globals.IsBusy = false;
                        }

                    }), null);

                }

            });
            t.Start();
        }

        private bool CheckData()
        {
            if (CurrentRefGenMedProductDetails == null)
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
                PrintCardStorageRpt(CurrentRefGenMedProductDetails.GenMedProductID, CurrentRefGenMedProductDetails.BrandName, StoreID, StoreName, CurrentRefGenMedProductDetails.UnitName);
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

        private PagedSortableCollectionView<RefGenMedProductSimple> _RefGenMedProductDetailss;
        public PagedSortableCollectionView<RefGenMedProductSimple> RefGenMedProductDetailss
        {
            get
            {
                return _RefGenMedProductDetailss;
            }
            set
            {
                if (_RefGenMedProductDetailss != value)
                {
                    _RefGenMedProductDetailss = value;
                }
                NotifyOfPropertyChange(() => RefGenMedProductDetailss);
            }
        }


        private void SearchRefDrugGenericDetails_AutoPaging(bool? IsCode, string Name, int PageIndex, int PageSize)
        {
            int totalCount = 0;
            //Ghi chu: autocomplete khong nen dung indicator vi mat focus 
            //IsLoading = true;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyGenMedDeptDetailsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginRefGenMedProductDetails_SimpleAutoPaging(IsCode, Name, V_MedProductType, null, PageSize, PageIndex, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            var ListUnits = contract.EndRefGenMedProductDetails_SimpleAutoPaging(out totalCount, asyncResult);
                            if (IsCode.GetValueOrDefault())
                            {
                                if (ListUnits != null && ListUnits.Count > 0)
                                {
                                    CurrentRefGenMedProductDetails = ListUnits.FirstOrDefault();
                                }
                                else
                                {
                                    CurrentRefGenMedProductDetails = null;

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
                                    RefGenMedProductDetailss.Clear();
                                    RefGenMedProductDetailss.TotalItemCount = totalCount;
                                    RefGenMedProductDetailss.ItemCount = totalCount;
                                    
                                    //foreach (RefGenMedProductSimple p in ListUnits)
                                    //{
                                    //    RefGenMedProductDetailss.Add(p);
                                    //}
                                    RefGenMedProductDetailss.SourceCollection = ListUnits;
                                    NotifyOfPropertyChange(() => RefGenMedProductDetailss);
                                }
                                au.ItemsSource = RefGenMedProductDetailss;
                                au.PopulateComplete();
                            }
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            //IsLoading = false;
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
            RefGenMedProductDetailss.PageIndex = 0;
            SearchRefDrugGenericDetails_AutoPaging(null, e.Parameter, 0, RefGenMedProductDetailss.PageSize);
        }
        #endregion

        public void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            AxTextBox obj = sender as AxTextBox;

            if (obj == null || string.IsNullOrWhiteSpace(obj.Text))
            {
                return;
            }

            string Code = Globals.FormatCode(V_MedProductType, obj.Text);

            if (CurrentRefGenMedProductDetails != null)
            {
                if (CurrentRefGenMedProductDetails.Code.ToLower() != obj.Text.ToLower())
                {
                    SearchRefDrugGenericDetails_AutoPaging(true, Code, 0, RefGenMedProductDetailss.PageSize);
                }
            }
            else
            {
                SearchRefDrugGenericDetails_AutoPaging(true, Code, 0, RefGenMedProductDetailss.PageSize);
            }
        }

        public void AutoDrug_Text_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            RefGenMedProductSimple obj = (sender as AutoCompleteBox).SelectedItem as RefGenMedProductSimple;
            if (CurrentRefGenMedProductDetails != null)
            {
                if (CurrentRefGenMedProductDetails.BrandName != obj.BrandName)
                {
                    CurrentRefGenMedProductDetails = obj;
                }
            }
            else
            {
                CurrentRefGenMedProductDetails = obj;
            }
        }
    }
}
