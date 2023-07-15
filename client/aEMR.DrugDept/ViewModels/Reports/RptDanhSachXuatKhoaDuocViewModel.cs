using System.ComponentModel.Composition;
using aEMR.ViewContracts;
using Caliburn.Micro;
using aEMR.Infrastructure;
using DataEntities;
using System.Windows;
using aEMR.Infrastructure.Events;
using System;
using DevExpress.Xpf.Printing;
using aEMR.ReportModel.ReportModels;
using aEMR.CommonTasks;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using eHCMSLanguage;
using Microsoft.Win32;
using DevExpress.ReportServer.Printing;
/*
* 20171226 #001 CMN: Added Temp Inward Report
*/
namespace aEMR.DrugDept.ViewModels
{
    [Export(typeof(IRptDanhSachXuatKhoaDuoc)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class RptDanhSachXuatKhoaDuocViewModel : Conductor<object>, IRptDanhSachXuatKhoaDuoc
        , IHandle<SelectedObjectWithKey<Object, Object, Object, Object, Object, Object, Object, Object>>
        , IHandle<SelectedObjectWithKeyExcel<Object, Object, Object, Object, Object, Object, Object, Object>>
        , IHandle<PrintEventActionView>
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

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

        [ImportingConstructor]
        public RptDanhSachXuatKhoaDuocViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            eventAggregator.Subscribe(this);
            //Globals.EventAggregator.Subscribe(this);

            var ucDK = Globals.GetViewModel<ICriteriaA>();

            if (mChonKho == true)
            {
                ucDK.VisibilityStore = Visibility.Visible;
                ucDK.VisibilityExcel = Visibility.Collapsed;
            }
            else
            {
                ucDK.VisibilityStore = Visibility.Collapsed;
                ucDK.VisibilityExcel = Visibility.Visible;
            }
            ucDK.GetStoreFollowV_MedProductType = true;
            UCCriteriaA = ucDK;
            ActivateItem(ucDK);
        }

        public object UCCriteriaA
        {
            get;
            set;
        }
        /*TMA - 23/10/2017*/

        protected override void OnActivate()
        {
            base.OnActivate();
            if (mChonKho == true)
            {
                (UCCriteriaA as ICriteriaA).VisibilityStore = Visibility.Visible;
                (UCCriteriaA as ICriteriaA).VisibilityExcel = Visibility.Collapsed;
            }
            else
            {
                (UCCriteriaA as ICriteriaA).VisibilityStore = Visibility.Collapsed;
                (UCCriteriaA as ICriteriaA).VisibilityExcel = Visibility.Visible;
            }
            (UCCriteriaA as ICriteriaA).VisibilityOutputType = VisibilityOutputType;
            (UCCriteriaA as ICriteriaA).V_MedProductType = V_MedProductType;
            (UCCriteriaA as ICriteriaA).GetListStore((long)AllLookupValues.StoreType.STORAGE_DRUGDEPT);
        }

        private bool _mChonKho = true;
        public bool mChonKho
        {
            get
            {
                return _mChonKho;
            }
            set
            {
                if (_mChonKho == value)
                    return;
                _mChonKho = value;
                NotifyOfPropertyChange(() => mChonKho);
            }
        }

        /*TMA - 23/10/2017*/

        /*▼====: #001*/
        private bool _IsTempInwardReport = false;
        public bool IsTempInwardReport
        {
            get
            {
                return _IsTempInwardReport;
            }
            set
            {
                _IsTempInwardReport = value;
                NotifyOfPropertyChange(() => IsTempInwardReport);
            }
        }
        /*▲====: #001*/

        private bool _mXemIn = true;
        public bool mXemIn
        {
            get
            {
                return _mXemIn;
            }
            set
            {
                if (_mXemIn == value)
                    return;
                _mXemIn = value;
                NotifyOfPropertyChange(() => mXemIn);
            }
        }

        public string TieuDeRpt { get; set; }

        public long V_MedProductType { get; set; }

        //public void btn_View(object sender, RoutedEventArgs e)
        //{
        //    //if (CheckData())
        //    //{
        //    //    ReportModel = null;
        //    //    ReportModel = new DrugDeptInOutStocksReportModal().PreviewModel;

        //    //    rParams["FromDate"].Value = FromDate;
        //    //    rParams["ToDate"].Value = ToDate;
        //    //    rParams["StorageName"].Value = StorageName;
        //    //    rParams["StoreID"].Value = (int)StoreID;
        //    //    rParams["DateShow"].Value = DateShow;
        //    //    rParams["V_MedProductType"].Value = Convert.ToInt32(V_MedProductType);

        //    //    // ReportModel.AutoShowParametersPanel = false;
        //    //    ReportModel.CreateDocument();
        //    //}
        //}

        public void Handle(SelectedObjectWithKey<object, object, object, object, object, object, object, object> message)
        {
            string strTieuDe = "";
            DateTime? FromDate = null;
            DateTime? ToDate = null;
            int? Quy = 0;
            int? Thang = 0;
            int? Nam = 0;
            RefStorageWarehouseLocation curStore;
            int? TypID = 0;

            if (message != null)
            {
                if (this.GetView() != null)
                {
                    DevExpress.DocumentServices.ServiceModel.DefaultValueParameterContainer rParams = new DevExpress.DocumentServices.ServiceModel.DefaultValueParameterContainer();
                    curStore = message.ObjF as RefStorageWarehouseLocation;
                    switch (message.ObjKey.ToString())
                    {
                        case "1":
                            {
                                FromDate = Convert.ToDateTime(message.ObjA);
                                ToDate = Convert.ToDateTime(message.ObjB);
                                strTieuDe = TieuDeRpt + " (" + Convert.ToDateTime(message.ObjA).ToString("dd/MM/yyyy") + " - " + Convert.ToDateTime(message.ObjB).ToString("dd/MM/yyyy") + ")";
                                break;
                            }
                        case "2":
                            {
                                Quy = Convert.ToInt32(message.ObjC);
                                Nam = Convert.ToInt32(message.ObjE);

                                strTieuDe = TieuDeRpt + string.Format(" {0} ", eHCMSResources.Q0486_G1_Quy) + message.ObjC.ToString() + string.Format(" {0} ", eHCMSResources.N0033_G1_Nam.ToLower()) + message.ObjE.ToString();
                                break;
                            }
                        case "3":
                            {
                                Thang = Convert.ToInt32(message.ObjD);
                                Nam = Convert.ToInt32(message.ObjE);

                                strTieuDe = TieuDeRpt + string.Format(" {0} ", eHCMSResources.G0039_G1_Th) + message.ObjD.ToString() + string.Format(" {0} ", eHCMSResources.N0033_G1_Nam.ToLower()) + message.ObjE.ToString();
                                break;
                            }

                    }
                    TypID = Convert.ToInt32(message.ObjG);
                    ReportModel = null;
                    if (mChonKho == true) // là báo cáo xuất
                    {
                        ReportModel = new RptDanhSachXuatKhoaDuoc().PreviewModel;
                        rParams["parTieuDeRpt"].Value = strTieuDe;
                        rParams["parV_MedProductType"].Value = (int)V_MedProductType;
                        if (FromDate != null)
                            rParams["parFromDate"].Value = FromDate;
                        if (ToDate != null)
                            rParams["parToDate"].Value = ToDate;
                        rParams["parQuy"].Value = Quy.Value;
                        rParams["parThang"].Value = Thang.Value;
                        rParams["parNam"].Value = Nam.Value;
                        rParams["parViewBy"].Value = Convert.ToInt32(message.ObjKey);
                        rParams["Show"].Value = "Tất Cả Kho Dược";
                        rParams["parStoreID"].Value = 0;
                        rParams["pTypID"].Value = TypID;
                        if (curStore != null)
                        {
                            if (curStore.StoreID > 0)
                            {
                                rParams["parStoreID"].Value = Convert.ToInt32(curStore.StoreID);
                                rParams["Show"].Value = curStore.swhlName;
                            }
                        }
                        rParams["parLogoUrl"].Value = Globals.ServerConfigSection.CommonItems.ReportLogoUrl;
                        rParams["parDepartmentOfHealth"].Value = Globals.ServerConfigSection.CommonItems.ReportDepartmentOfHealth;
                    }
                    /*▼====: #001*/
                    else if (IsTempInwardReport)
                    {
                        ReportModel = new RptDrugDeptTempInwardReport().PreviewModel;
                        rParams["parTieuDeRpt"].Value = strTieuDe;
                        rParams["parV_MedProductType"].Value = (int)V_MedProductType;
                        if (FromDate != null)
                            rParams["parFromDate"].Value = FromDate;
                        if (ToDate != null)
                            rParams["parToDate"].Value = ToDate;
                        rParams["parQuy"].Value = Quy.Value;
                        rParams["parThang"].Value = Thang.Value;
                        rParams["parNam"].Value = Nam.Value;
                        rParams["parViewBy"].Value = Convert.ToInt32(message.ObjKey);
                        rParams["parLogoUrl"].Value = Globals.ServerConfigSection.CommonItems.ReportLogoUrl;
                    }
                    /*▲====: #001*/
                    else // là báo cáo nhập cũa Miss Ái 23/10/2017
                    {
                        ReportModel = new RptDrugMedDept().PreviewModel;
                        rParams["parTieuDeRpt"].Value = strTieuDe;
                        rParams["parV_MedProductType"].Value = (int)V_MedProductType;
                        if (FromDate != null)
                            rParams["parFromDate"].Value = FromDate;
                        if (ToDate != null)
                            rParams["parToDate"].Value = ToDate;
                        rParams["parQuy"].Value = Quy.Value;
                        rParams["parThang"].Value = Thang.Value;
                        rParams["parNam"].Value = Nam.Value;
                        rParams["parViewBy"].Value = Convert.ToInt32(message.ObjKey);
                        rParams["parLogoUrl"].Value = Globals.ServerConfigSection.CommonItems.ReportLogoUrl;
                    }

                    // ReportModel.AutoShowParametersPanel = false;
                    ReportModel.CreateDocument(rParams);
                }
            }
        }

        /*TMA 24/10/2017*/
        public void Handle(SelectedObjectWithKeyExcel<object, object, object, object, object, object, object, object> message)
        {
            DateTime? FromDate = null;
            DateTime? ToDate = null;
            int? Quy = 0;
            int? Thang = 0;
            int? Nam = 0;
            RefStorageWarehouseLocation curStore;

            if (message != null)
            {
                if (GetView() != null)
                {
                    curStore = message.ObjF as RefStorageWarehouseLocation;
                    switch (message.ObjKey.ToString())
                    {
                        case "1":
                            {
                                FromDate = Convert.ToDateTime(message.ObjA);
                                ToDate = Convert.ToDateTime(message.ObjB);
                                break;
                            }
                        case "2":
                            {
                                Quy = Convert.ToInt32(message.ObjC);
                                Nam = Convert.ToInt32(message.ObjE);
                                break;
                            }
                        case "3":
                            {
                                Thang = Convert.ToInt32(message.ObjD);
                                Nam = Convert.ToInt32(message.ObjE);
                                break;
                            }
                    }
                    /*▼====: #001*/
                    if (IsTempInwardReport)
                    {
                        SaveFileDialog objSFD = new SaveFileDialog()
                        {
                            DefaultExt = ".xls",
                            Filter = "Excel xls (*.xls)|*.xls",
                            FilterIndex = 1
                        };
                        if (objSFD.ShowDialog() != true)
                        {
                            return;
                        }
                        ReportParameters RptParameters = new ReportParameters();
                        RptParameters.ReportType = ReportType.SOKIEMNHAP;
                        RptParameters.reportName = ReportName.THEODOIHANGXUATKYGOI;
                        RptParameters.FromDate = FromDate;
                        RptParameters.ToDate = ToDate;
                        RptParameters.Quarter = Quy.Value;
                        RptParameters.Month = Thang.Value;
                        RptParameters.Year = Nam.Value;
                        RptParameters.Flag = Convert.ToInt32(message.ObjKey);
                        RptParameters.V_MedProductType = (long)V_MedProductType;

                        ExportToExcelGeneric.Action(RptParameters, objSFD, this);
                    }
                    /*▲====: #001*/
                    else if (mChonKho == false) // là báo cáo nhập
                    {
                        SaveFileDialog objSFD = new SaveFileDialog()
                        {
                            DefaultExt = ".xls",
                            Filter = "Excel xls (*.xls)|*.xls",
                            //Filter = "Excel (2003)(.xls)|*.xls|Excel (2010) (.xlsx)|*.xlsx |RichText File (.rtf)|*.rtf |Pdf File (.pdf)|*.pdf |Html File (.html)|*.html",
                            FilterIndex = 1
                        };
                        if (objSFD.ShowDialog() != true)
                        {
                            return;
                        }
                        ReportParameters RptParameters = new ReportParameters();
                        RptParameters.ReportType = ReportType.SOKIEMNHAP;
                        RptParameters.reportName = ReportName.RptDrugMedDept;
                        RptParameters.FromDate = FromDate;
                        RptParameters.ToDate = ToDate;
                        RptParameters.Quarter = Quy.Value;
                        RptParameters.Month = Thang.Value;
                        RptParameters.Year = Nam.Value;
                        RptParameters.Flag = Convert.ToInt32(message.ObjKey);
                        RptParameters.V_MedProductType = (long)V_MedProductType;

                        ExportToExcelGeneric.Action(RptParameters, objSFD, this);
                    }
                }
            }
        }
        public void Handle(PrintEventActionView message)
        {
            if (message != null && IsDrugDeptExportDetail)
            {
                if (this.GetView() != null && UCCriteriaA is ICriteriaA && (UCCriteriaA as ICriteriaA).CurStore != null && (UCCriteriaA as ICriteriaA).OutStore != null)
                {
                    DevExpress.DocumentServices.ServiceModel.DefaultValueParameterContainer rParams = new DevExpress.DocumentServices.ServiceModel.DefaultValueParameterContainer();
                    ReportModel = null;
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.RptDrugDepts.XtraReports.XRptDrugDeptLocalOutput").PreviewModel;
                    rParams["StoreID"].Value = (UCCriteriaA as ICriteriaA).CurStore.StoreID;
                    rParams["StoreName"].Value = (UCCriteriaA as ICriteriaA).CurStore.swhlName;
                    rParams["OutputStoreID"].Value = (UCCriteriaA as ICriteriaA).OutStore.StoreID;
                    rParams["OutputStoreName"].Value = (UCCriteriaA as ICriteriaA).OutStore.swhlName;
                    rParams["FromDate"].Value = (UCCriteriaA as ICriteriaA).FromDate;
                    rParams["ToDate"].Value = (UCCriteriaA as ICriteriaA).ToDate;
                    ReportModel.CreateDocument(rParams);
                }
            }
            else if (message != null && gReportName == DataEntities.ReportName.TK_NX_THEOMUCDICH)
            {
                if (this.GetView() != null && UCCriteriaA is ICriteriaA)
                {
                    DevExpress.DocumentServices.ServiceModel.DefaultValueParameterContainer rParams = new DevExpress.DocumentServices.ServiceModel.DefaultValueParameterContainer();
                    ReportModel = null;
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.RptDrugDepts.XtraReports.XRptDrugDeptInOutStatistics").PreviewModel;
                    rParams["FromDate"].Value = (UCCriteriaA as ICriteriaA).FromDate;
                    rParams["ToDate"].Value = (UCCriteriaA as ICriteriaA).ToDate;
                    if ((UCCriteriaA as ICriteriaA).CurStore != null)
                    {
                        rParams["StoreID"].Value = (UCCriteriaA as ICriteriaA).CurStore.StoreID;
                        rParams["StoreName"].Value = (UCCriteriaA as ICriteriaA).CurStore.swhlName;
                    }
                    ReportModel.CreateDocument(rParams);
                }
            }
        }

        private bool _VisibilityOutputType;
        public bool VisibilityOutputType
        {
            get
            {
                return _VisibilityOutputType;
            }
            set
            {
                _VisibilityOutputType = value;
                NotifyOfPropertyChange(() => VisibilityOutputType);
            }
        }

        public bool MedProductTypeVisible
        {
            get
            {
                if (UCCriteriaA == null)
                {
                    return false;
                }
                return (UCCriteriaA as ICriteriaA).MedProductTypeVisible;
            }
            set
            {
                if (UCCriteriaA != null)
                {
                    (UCCriteriaA as ICriteriaA).MedProductTypeVisible = value;
                }
            }
        }
        public bool IsDrugDeptExportDetail
        {
            get
            {
                if (UCCriteriaA == null)
                {
                    return false;
                }
                return (UCCriteriaA as ICriteriaA).IsDrugDeptExportDetail;
            }
            set
            {
                if (UCCriteriaA != null)
                {
                    (UCCriteriaA as ICriteriaA).IsDrugDeptExportDetail = value;
                }
            }
        }
        public bool IsViewByVisible
        {
            get
            {
                if (UCCriteriaA == null)
                {
                    return false;
                }
                return (UCCriteriaA as ICriteriaA).IsViewByVisible;
            }
            set
            {
                if (UCCriteriaA != null)
                {
                    (UCCriteriaA as ICriteriaA).IsViewByVisible = value;
                }
            }
        }

        public void InitForIsDrugDeptExportDetail()
        {
            if (UCCriteriaA != null)
            {
                (UCCriteriaA as ICriteriaA).InitForIsDrugDeptExportDetail();
            }
        }

        public Nullable<ReportName> gReportName
        {
            get
            {
                if (UCCriteriaA == null)
                {
                    return null;
                }
                return (UCCriteriaA as ICriteriaA).gReportName;
            }
            set
            {
                if (UCCriteriaA != null && value != null)
                {
                    (UCCriteriaA as ICriteriaA).gReportName = value.Value;
                }
            }
        }
    }
}