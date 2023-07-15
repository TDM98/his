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
* 20200218 #001 TNHX: [] Thêm điều kiện lọc danh mục NT + Kho BHYT Ngoại trú
* 20211103 #002 QTD:  Lọc kho theo cấu hình trách nhiệm
*/
namespace aEMR.Common.ViewModels
{
    [Export(typeof(ITheKhoKT)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class TheKhoViewModel : Conductor<object>, ITheKhoKT
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
            Authorization();

            RefGenMedProductDetailss = new PagedSortableCollectionView<RefGenMedProductSimple>();
            RefGenMedProductDetailss.OnRefresh += RefGenMedProductDetailss_OnRefresh;
            RefGenMedProductDetailss.PageSize = Globals.PageSize;
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            Globals.EventAggregator.Subscribe(this);
        }

        void RefGenMedProductDetailss_OnRefresh(object sender, RefreshEventArgs e)
        {
            SearchRefDrugGenericDetails_AutoPaging(null, BrandName, RefGenMedProductDetailss.PageIndex, RefGenMedProductDetailss.PageSize);
        }

        public void Authorization()
        {
        }

        private string _strHienThi = Globals.PageName;
        public string StrHienThi
        {
            get
            {
                return _strHienThi;
            }
            set
            {
                _strHienThi = value;
                NotifyOfPropertyChange(() => StrHienThi);
            }
        }

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
        //▼====: #001
        private bool _IsHIStore = false;
        public bool IsHIStore
        {
            get
            {
                return _IsHIStore;
            }
            set
            {
                _IsHIStore = value;
                NotifyOfPropertyChange(() => IsHIStore);
            }
        }
        //▲====: #001

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
        #endregion

        public void btn_View(object sender, RoutedEventArgs e)
        {
            if (CheckData())
            {
                ReportModel = null;
                DevExpress.DocumentServices.ServiceModel.DefaultValueParameterContainer rParams = new DevExpress.DocumentServices.ServiceModel.DefaultValueParameterContainer();
                switch (_StoreType)
                {
                    case (long)AllLookupValues.StoreType.STORAGE_CLINIC:
                        ReportModel = new ClinicDeptCardStorageReportModal_KT().PreviewModel;
                        rParams["V_MedProductType"].Value = CurProductType.LookupID;
                        break;
                    case (long)AllLookupValues.StoreType.STORAGE_DRUGDEPT:
                        ReportModel = new DrugDeptCardStorageReportModal_KT().PreviewModel;
                        rParams["V_MedProductType"].Value = CurProductType.LookupID;
                        break;
                    case (long)AllLookupValues.StoreType.STORAGE_EXTERNAL:
                        ReportModel = new PharmacyCardStorageReportModal_KT().PreviewModel;
                        break;
                    default:
                        ReportModel = new DrugDeptCardStorageReportModal_KT().PreviewModel;
                        break;
                }
                rParams["DrugName"].Value = CurrentRefGenMedProductDetails.BrandName;
                rParams["GenMedProductID"].Value = CurrentRefGenMedProductDetails.GenMedProductID;
                rParams["StorageName"].Value = CurStore.swhlName;
                rParams["StoreID"].Value = CurStore.StoreID;
                rParams["UnitName"].Value = CurrentRefGenMedProductDetails.UnitName;
                rParams["FromDate"].Value = FromDate;
                rParams["ToDate"].Value = ToDate;
                rParams["ShowDate"].Value = string.Format("{0} :", eHCMSResources.G1933_G1_TuNg) + FromDate.GetValueOrDefault().ToString("dd/MM/yyyy") + string.Format(" _ {0} :", eHCMSResources.K3192_G1_DenNg) + ToDate.GetValueOrDefault().ToString("dd/MM/yyyy");
                rParams["Code"].Value = CurrentRefGenMedProductDetails.Code;
                rParams["parLogoUrl"].Value = Globals.ServerConfigSection.CommonItems.ReportLogoUrl;

                //20210806 QTD Add new Method for DrugDeptCardStorageReportModal_KT
                if (_StoreType == (long)AllLookupValues.StoreType.STORAGE_DRUGDEPT)
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
                                contract.BeginGetXRptDrugDeptCardStorage(CurStore.StoreID
                                    , CurrentRefGenMedProductDetails.GenMedProductID, CurProductType.LookupID
                                    , CurrentRefGenMedProductDetails.BrandName
                                    , CurStore.swhlName, CurrentRefGenMedProductDetails.UnitName
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

                else if (_StoreType == (long)AllLookupValues.StoreType.STORAGE_CLINIC)
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
                                contract.BeginGetXRptClinicDeptCardStorage(CurStore.StoreID
                                    , CurrentRefGenMedProductDetails.GenMedProductID, CurProductType.LookupID
                                    , CurrentRefGenMedProductDetails.BrandName
                                    , CurStore.swhlName, CurrentRefGenMedProductDetails.UnitName
                                    , FromDate, ToDate, dateshow, CurrentRefGenMedProductDetails.Code
                                    , Globals.ServerConfigSection.CommonItems.ReportLogoUrl
                                    , Globals.DispatchCallback((asyncResult) =>
                                    {
                                        try
                                        {
                                            var results = contract.EndGetXRptClinicDeptCardStorage(asyncResult);
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

                else if (_StoreType == (long)AllLookupValues.StoreType.STORAGE_EXTERNAL)
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
                                contract.BeginGetXRptPharmacyCardStorage(CurStore.StoreID
                                    , CurrentRefGenMedProductDetails.GenMedProductID
                                    , CurrentRefGenMedProductDetails.BrandName
                                    , CurStore.swhlName, CurrentRefGenMedProductDetails.UnitName
                                    , FromDate, ToDate, dateshow, CurrentRefGenMedProductDetails.Code
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
            }
        }

        private void PrintCardStorageRpt(long GenMedProductID, string DrugName, long StoreID, string StorageName, string UnitName)
        {
            this.ShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
            var t = new Thread(() =>
            {
                try
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
                                this.HideBusyIndicator();
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    this.HideBusyIndicator();
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
            if (CurStore == null)
            {
                MessageBox.Show(eHCMSResources.K0310_G1_ChonKhoCanXem);
                return false;
            }
            else
            {
                return true;
            }
        }

        public void btn_Print(object sender, RoutedEventArgs e)
        {
            if (CheckData())
            {
                PrintCardStorageRpt(CurrentRefGenMedProductDetails.GenMedProductID, CurrentRefGenMedProductDetails.BrandName, CurStore.StoreID, CurStore.swhlName, CurrentRefGenMedProductDetails.UnitName);
            }
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

        private void PharmacySearchRefDrugGenericDetails_AutoPaging(bool? IsCode, string Name, int PageIndex, int PageSize)
        {
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyDrugServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginSearchRefGenericDrugName_SimpleAutoPaging(IsCode, Name, PageIndex, PageSize, IsHIStore, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var ListUnits = contract.EndSearchRefGenericDrugName_SimpleAutoPaging(out int totalCount, asyncResult);
                                List<RefGenMedProductSimple> ListDrug = new List<RefGenMedProductSimple>();
                                if (IsCode.GetValueOrDefault())
                                {
                                    if (ListUnits != null && ListUnits.Count > 0)
                                    {
                                        foreach(RefGenericDrugSimple temp in ListUnits)
                                        {
                                            RefGenMedProductSimple a = new RefGenMedProductSimple
                                            {
                                                GenMedProductID = temp.DrugID,
                                                BrandName = temp.BrandName,
                                                GenericName = temp.GenericName,
                                                Code = temp.DrugCode,
                                                ProductCodeRefNum = null,
                                                UnitName = temp.UnitName
                                            };
                                            ListDrug.Add(a);
                                        }
                                        CurrentRefGenMedProductDetails = ListDrug.FirstOrDefault();
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
                                        foreach (RefGenericDrugSimple temp in ListUnits)
                                        {
                                            RefGenMedProductSimple a = new RefGenMedProductSimple
                                            {
                                                GenMedProductID = temp.DrugID,
                                                BrandName = temp.BrandName,
                                                GenericName = temp.GenericName,
                                                Code = temp.DrugCode,
                                                ProductCodeRefNum = null,
                                                UnitName = temp.UnitName
                                            };
                                            ListDrug.Add(a);
                                        }
                                        RefGenMedProductDetailss.Clear();
                                        RefGenMedProductDetailss.TotalItemCount = totalCount;
                                        RefGenMedProductDetailss.ItemCount = totalCount;
                                        RefGenMedProductDetailss.SourceCollection = ListDrug;
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
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                }
            });

            t.Start();
        }

        private void SearchRefDrugGenericDetails_AutoPaging(bool? IsCode, string Name, int PageIndex, int PageSize)
        {
            if (_StoreType == (long)AllLookupValues.StoreType.STORAGE_EXTERNAL)
            {
                PharmacySearchRefDrugGenericDetails_AutoPaging(IsCode, Name, PageIndex, PageSize);
            }
            else
            {
                int totalCount = 0;
                //Ghi chu: autocomplete khong nen dung indicator vi mat focus
                var t = new Thread(() =>
                {
                    using (var serviceFactory = new PharmacyGenMedDeptDetailsServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginRefGenMedProductDetails_SimpleAutoPaging(IsCode, Name, CurProductType.LookupID, null, PageSize, PageIndex, Globals.DispatchCallback((asyncResult) =>
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
                        }), null);
                    }
                });

                t.Start();
            }            
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

            string Code = Globals.FormatCode(CurProductType.LookupID, obj.Text);

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

        private long? _StoreType = 0;
        public void GetListStore(long? StoreType)
        {
            _StoreType = StoreType;
            Coroutine.BeginExecute(DoGetStore(StoreType));
        }

        private IEnumerator<IResult> DoGetStore(long? StoreType)
        {
            var paymentTypeTask = new LoadStoreListTask(StoreType, false, null, false, true);
            yield return paymentTypeTask;
            //StoreCbx = paymentTypeTask.LookupList.Where(x => (CurProductType != null && CurProductType.LookupID != 0 && x.ListV_MedProductType != null && x.ListV_MedProductType.Contains(CurProductType.LookupID.ToString()))).ToObservableCollection();
            var StoreTemp = paymentTypeTask.LookupList.Where(x => (CurProductType != null && CurProductType.LookupID != 0 && x.ListV_MedProductType != null && x.ListV_MedProductType.Contains(CurProductType.LookupID.ToString()))).ToObservableCollection();
            StoreCbx = Globals.CheckDrugMedStoreWareHouse(StoreTemp);
            if (StoreCbx != null && StoreCbx.Count > 0)
            {
                CurStore = StoreCbx.FirstOrDefault();
            }
            else
            {
                MessageBox.Show(eHCMSResources.A0110_G1_Msg_InfoChuaCauHinhTNKho.Replace("\\n ", "\n"));
            }
            yield break;
        }

        public void CbxV_MedProductType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CurProductType != null)
            {
                //▼====: #002
                //if (CurProductType.LookupID == (long)AllLookupValues.MedProductType.THUOC)
                //{
                //    IsShowGroupReportType = false;
                //    CanSelectedRefGenDrugCatID_1 = true;
                //    if (RefGenericDrugCategory_1s == null)
                //    {
                //        LoadRefGenericDrugCategory_1();
                //    }
                //}
                //else if (CurProductType.LookupID == (long)AllLookupValues.MedProductType.Y_CU)
                //{
                //    IsShowGroupReportType = true;
                //    CanSelectedRefGenDrugCatID_1 = false;
                //}
                //else
                //{
                //    IsShowGroupReportType = false;
                //    CanSelectedRefGenDrugCatID_1 = false;
                //}
                //▲====: #002
                GetListStore(_StoreType);
            }
        }

        private RefStorageWarehouseLocation _CurStore;
        public RefStorageWarehouseLocation CurStore
        {
            get { return _CurStore; }
            set
            {
                _CurStore = value;
                NotifyOfPropertyChange(() => CurStore);
            }
        }

        private Lookup _CurProductType = Globals.AllLookupValueList.Where(x => x.ObjectTypeID == (long)LookupValues.V_MedProductType).ToObservableCollection().FirstOrDefault();
        public Lookup CurProductType
        {
            get => _CurProductType; set
            {
                _CurProductType = value;
                //LoadRefGenericDrugCategory_1();
                NotifyOfPropertyChange(() => CurProductType);
            }
        }

        private ObservableCollection<Lookup> _AllMedProductTypeCollection = Globals.AllLookupValueList.Where(x => x.ObjectTypeID == (long)LookupValues.V_MedProductType).ToObservableCollection();
        public ObservableCollection<Lookup> AllMedProductTypeCollection
        {
            get
            {
                return _AllMedProductTypeCollection;
            }
            set
            {
                _AllMedProductTypeCollection = value;
                NotifyOfPropertyChange(() => AllMedProductTypeCollection);
            }
        }
    }
}
