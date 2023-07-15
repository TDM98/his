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
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using DevExpress.ReportServer.Printing;

namespace aEMR.StoreDept.Reports.ViewModels
{
    [Export(typeof(IClinicRptDanhSachXuatKhoaDuoc)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class ClinicRptDanhSachXuatKhoaDuocViewModel : Conductor<object>, IClinicRptDanhSachXuatKhoaDuoc
        , IHandle<SelectedObjectWithKey<Object, Object, Object, Object, Object,Object,Object, Object>>
    {
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
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public ClinicRptDanhSachXuatKhoaDuocViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            //eventAggregator.Subscribe(this);
            //Globals.EventAggregator.Subscribe(this);

            var ucDK = Globals.GetViewModel<ICriteriaA>();
            ucDK.VisibilityStore = Visibility.Visible;
            ucDK.isStoreDept = true;
            UCCriteriaA = ucDK;
            this.ActivateItem(ucDK);
        }
        protected override void OnActivate()
        {
            base.OnActivate();
            Globals.EventAggregator.Subscribe(this);
            UCCriteriaA.V_MedProductType = V_MedProductType;
            UCCriteriaA.GetListStore((long)AllLookupValues.StoreType.STORAGE_CLINIC);
        }

        public ICriteriaA UCCriteriaA
        {
            get;
            set;
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
            DateTime? FromDate=null; 
            DateTime? ToDate=null;
            int? Quy=0;
            int? Thang=0;
            int? Nam=0;
            RefStorageWarehouseLocation curStore;

            if(message!=null)
            {
                if(this.GetView()!=null)
                {
                    curStore = message.ObjF as RefStorageWarehouseLocation;
                    switch (message.ObjKey.ToString())
                    {
                        case "1":
                            {
                                FromDate =Convert.ToDateTime(message.ObjA);
                                ToDate = Convert.ToDateTime(message.ObjB);

                                strTieuDe = TieuDeRpt + " (" + Convert.ToDateTime(message.ObjA).ToString("dd/MM/yyyy") + " - " + Convert.ToDateTime(message.ObjB).ToString("dd/MM/yyyy") + ")";
                                break;
                            }
                        case "2":
                            {
                                Quy =Convert.ToInt32(message.ObjC);
                                Nam = Convert.ToInt32(message.ObjE);

                                strTieuDe = TieuDeRpt + " Quý " + message.ObjC.ToString() + " năm " + message.ObjE.ToString();
                                break;
                            }
                        case "3":
                            {
                                Thang = Convert.ToInt32(message.ObjD);
                                Nam = Convert.ToInt32(message.ObjE);

                                strTieuDe = TieuDeRpt + " Tháng " + message.ObjD.ToString() + " năm " + message.ObjE.ToString();
                                break;
                            }
                    }
                    DevExpress.DocumentServices.ServiceModel.DefaultValueParameterContainer rParams = new DevExpress.DocumentServices.ServiceModel.DefaultValueParameterContainer();
                    ReportModel = null;
                    ReportModel = new RptDanhSachXuatKhoPhongModel().PreviewModel;
                    rParams["parTieuDeRpt"].Value = strTieuDe;
                    rParams["parV_MedProductType"].Value = (int)V_MedProductType;
                    if (FromDate != null)
                        rParams["parFromDate"].Value = FromDate;
                    if (ToDate != null)
                        rParams["parToDate"].Value = ToDate;
                    rParams["parQuy"].Value =Quy.Value;
                    rParams["parThang"].Value = Thang.Value;
                    rParams["parNam"].Value =Nam.Value;
                    rParams["parViewBy"].Value = Convert.ToInt32(message.ObjKey);
                    rParams["Show"].Value = "Tất Cả Kho Nội Trú";
                    rParams["parStoreID"].Value = 0;
                    if (curStore != null)
                    {
                        if (curStore.StoreID > 0)
                        {
                            rParams["parStoreID"].Value = Convert.ToInt32(curStore.StoreID);
                            rParams["Show"].Value = curStore.swhlName;
                        }
                    }

                    rParams["parDepartmentOfHealth"].Value = Globals.ServerConfigSection.CommonItems.ReportDepartmentOfHealth;
                    rParams["parLogoUrl"].Value = Globals.ServerConfigSection.CommonItems.ReportLogoUrl;
                    // ReportModel.AutoShowParametersPanel = false;
                    ReportModel.CreateDocument(rParams);

                }
            }
        }

        #region checking acount
        private bool _mBaoCaoXuat_XemIn = true;
        public bool mBaoCaoXuat_XemIn
        {
            get
            {
                return _mBaoCaoXuat_XemIn;
            }
            set
            {
                if (_mBaoCaoXuat_XemIn == value)
                    return;
                _mBaoCaoXuat_XemIn = value;
                NotifyOfPropertyChange(() => mBaoCaoXuat_XemIn);
            }
        }
        #endregion
    }
}
