using eHCMSLanguage;
using System.ComponentModel.Composition;
using aEMR.ViewContracts;
using Caliburn.Micro;
using System.Windows;
using System.Windows.Controls;
using System;
using aEMR.Infrastructure;
using System.Collections.ObjectModel;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using System.Threading;
using DataEntities;
using System.Linq;
using System.Collections.Generic;
using DevExpress.Xpf.Printing;
using aEMR.ReportModel.ReportModels;
using aEMR.Common.Printing;
using aEMR.CommonTasks;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using DevExpress.ReportServer.Printing;

namespace aEMR.Pharmacy.ViewModels
{
    [Export(typeof(IReportByMMYYYY)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class ReportByMMYYYYViewModel : Conductor<object>, IReportByMMYYYY
    {
        /*==== #001 ====*/
        StaffPosition TruongNhaThuoc;
        /*==== #001 ====*/
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public ReportByMMYYYYViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;
            authorization();
            RptParameters = new ReportParameters();
            FillMonth();
            FillYear();

            Coroutine.BeginExecute(DoGetStore_EXTERNAL());
            /*==== #001 ====*/
            TruongNhaThuoc = Globals.allStaffPositions.Where(x => x.PositionRefID == (int)AllLookupValues.StaffPositions_Enum.TRUONG_NHA_THUOC && x.IsActive).FirstOrDefault();
            /*==== #001 ====*/
        }
        protected override void OnActivate()
        {
        }

        protected override void OnDeactivate(bool close)
        {
            //base.OnDeactivate(close);
            ReportModel = null;
            RptParameters = null;
            StoreCbx = null;
            ListMonth = null;
            ListYear = null;
            ListQuartar = null;
            Conditions = null;
        }

        private bool _bXem;
        public bool bXem
        {
            get { return _bXem; }
            set
            {
                if (_bXem == value)
                    return;
                _bXem = value;
            }
        }

        private bool _bIn;
        public bool bIn
        {
            get { return _bIn; }
            set
            {
                if (_bIn == value)
                    return;
                _bIn = value;
            }
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

        private string _pageTitle;
        public string pageTitle
        {
            get
            {
                return _pageTitle;
            }
            set
            {
                if (_pageTitle != value)
                {
                    _pageTitle = value;
                    NotifyOfPropertyChange(() => pageTitle);
                }
            }
        }

        public class Condition
        {
            private string _Text;
            private long _Value;
            public string Text { get { return _Text; } }
            public long Value { get { return _Value; } }
            public Condition(string theText, long theValue)
            {
                _Text = theText;
                _Value = theValue;
            }

        }

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


        private ReportParameters _RptParameters;
        public ReportParameters RptParameters
        {
            get { return _RptParameters; }
            set
            {
                if (_RptParameters != value)
                {
                    _RptParameters = value;
                    NotifyOfPropertyChange(() => RptParameters);
                }
            }
        }

        private ObservableCollection<int> _ListMonth;
        public ObservableCollection<int> ListMonth
        {
            get { return _ListMonth; }
            set
            {
                if (_ListMonth != value)
                {
                    _ListMonth = value;
                    NotifyOfPropertyChange(() => ListMonth);
                }
            }
        }

        private ObservableCollection<int> _ListQuartar;
        public ObservableCollection<int> ListQuartar
        {
            get { return _ListQuartar; }
            set
            {
                if (_ListQuartar != value)
                {
                    _ListQuartar = value;
                    NotifyOfPropertyChange(() => ListQuartar);
                }
            }
        }


        private ObservableCollection<int> _ListYear;
        public ObservableCollection<int> ListYear
        {
            get { return _ListYear; }
            set
            {
                if (_ListYear != value)
                {
                    _ListYear = value;
                    NotifyOfPropertyChange(() => ListYear);
                }
            }
        }

        private ObservableCollection<Condition> _Conditions;
        public ObservableCollection<Condition> Conditions
        {
            get
            {
                return _Conditions;
            }
            set
            {
                if (_Conditions != value)
                {
                    _Conditions = value;
                    NotifyOfPropertyChange(() => Conditions);
                }
            }
        }

        private Condition _CurrentCondition;
        public Condition CurrentCondition
        {
            get
            {
                return _CurrentCondition;
            }
            set
            {
                if (_CurrentCondition != value)
                {
                    _CurrentCondition = value;
                    NotifyOfPropertyChange(() => CurrentCondition);
                }
            }
        }

        private Visibility _IsMonth;
        public Visibility IsMonth
        {
            get
            { return _IsMonth; }
            set
            {
                if (_IsMonth != value)
                {
                    _IsMonth = value;
                    NotifyOfPropertyChange(() => IsMonth);
                }
            }
        }

        private Visibility _IsDate = Visibility.Collapsed;
        public Visibility IsDate
        {
            get
            { return _IsDate; }
            set
            {
                if (_IsDate != value)
                {
                    _IsDate = value;
                    NotifyOfPropertyChange(() => IsDate);
                }
            }
        }

        private Visibility _IsQuarter;
        public Visibility IsQuarter
        {
            get
            { return _IsQuarter; }
            set
            {
                if (_IsQuarter != value)
                {
                    _IsQuarter = value;
                    NotifyOfPropertyChange(() => IsQuarter);
                }
            }
        }

        private Visibility _IsYear;
        public Visibility IsYear
        {
            get
            { return _IsYear; }
            set
            {
                if (_IsYear != value)
                {
                    _IsYear = value;
                    NotifyOfPropertyChange(() => IsYear);
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
        #region FillData Member

        private void FillMonth()
        {
            if (ListMonth == null)
            {
                ListMonth = new ObservableCollection<int>();
            }
            else
            {
                ListMonth.Clear();
            }
            for (int i = 1; i < 13; i++)
            {
                ListMonth.Add(i);
            }
            RptParameters.Month = Globals.ServerDate.Value.Month;
        }

        private void FillQuarter()
        {
            if (ListQuartar == null)
            {
                ListQuartar = new ObservableCollection<int>();
            }
            else
            {
                ListQuartar.Clear();
            }
            for (int i = 1; i < 5; i++)
            {
                ListQuartar.Add(i);
            }
            int Month = Globals.ServerDate.Value.Month;
            if (Month <= 3)
            {
                // 1st Quarter = January 1 to March 31
                RptParameters.Quarter = 1;
            }
            else if ((Month >= 4) && (Month <= 6))
            {
                // 1st Quarter = January 1 to March 31
                RptParameters.Quarter = 2;
            }
            else if ((Month >= 7) && (Month <= 9))
            {
                // 1st Quarter = January 1 to March 31
                RptParameters.Quarter = 3;
            }
            else // 4th Quarter = October 1 to December 31
            {
                // 1st Quarter = January 1 to March 31
                RptParameters.Quarter = 4;
            }
        }

        private void FillYear()
        {
            if (ListYear == null)
            {
                ListYear = new ObservableCollection<int>();
            }
            else
            {
                ListYear.Clear();
            }
            int year = Globals.ServerDate.Value.Year;
            for (int i = year; i > year - 3; i--)
            {
                ListYear.Add(i);
            }
            RptParameters.Year = ListYear.FirstOrDefault();
        }

        private void FillCondition()
        {
            if (Conditions == null)
            {
                Conditions = new ObservableCollection<Condition>();
            }
            else
            {
                Conditions.Clear();
            }

            Conditions.Add(new Condition(eHCMSResources.Z0938_G1_TheoQuy, 0));
            Conditions.Add(new Condition(eHCMSResources.Z0939_G1_TheoTh, 1));
            Conditions.Add(new Condition(eHCMSResources.G0375_G1_TheoNg, 2));

            CurrentCondition = Conditions.FirstOrDefault();
            ByQuarter();
        }

        public void cbxCondition_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CurrentCondition != null)
            {
                switch (CurrentCondition.Value)
                {
                    case 0:
                        ByQuarter();
                        break;
                    case 1:
                        ByMonth();
                        break;
                    case 2:
                        ByDate();
                        break;
                }
            }
        }

        private void ByDate()
        {
            IsMonth = Visibility.Collapsed;
            IsDate = Visibility.Visible;
            IsQuarter = Visibility.Collapsed;
            IsYear = Visibility.Collapsed;
        }

        private void ByMonth()
        {
            IsMonth = Visibility.Visible;
            IsDate = Visibility.Collapsed;
            IsQuarter = Visibility.Collapsed;
            IsYear = Visibility.Visible;
        }

        private void ByQuarter()
        {
            IsMonth = Visibility.Collapsed;
            IsDate = Visibility.Collapsed;
            IsQuarter = Visibility.Visible;
            IsYear = Visibility.Visible;
        }

        #endregion

        private IEnumerator<IResult> DoGetStore_EXTERNAL()
        {
            var paymentTypeTask = new LoadStoreListTask((long)AllLookupValues.StoreType.STORAGE_EXTERNAL, false,null, false, false);
            yield return paymentTypeTask;
            StoreCbx = paymentTypeTask.LookupList;
            yield break;
        }

        private void GetReport()
        {
            DevExpress.DocumentServices.ServiceModel.DefaultValueParameterContainer rParams = new DevExpress.DocumentServices.ServiceModel.DefaultValueParameterContainer();
            switch (_eItem)
            {
                case ReportName.PHARMACY_BAOCAOTONGHOPDOANHTHU:
                    ReportModel = null;
                    ReportModel = new TongHopDoanhThuReportModel().PreviewModel;
                    rParams["Month"].Value = RptParameters.Month;
                    rParams["Year"].Value = RptParameters.Year;
                    /*==== #001 ====*/
                    rParams["TruongNhaThuoc"].Value = TruongNhaThuoc != null ? TruongNhaThuoc.FullNameString : "";
                    rParams["parLogoUrl"].Value = Globals.ServerConfigSection.CommonItems.ReportLogoUrl;
                    /*==== #001 ====*/
                    break;
                case ReportName.PHARMACY_BCKIEMKEVADUTRU:
                    ReportModel = null;
                    ReportModel = new PharmacyKiemKeVaDuTruReportModel().PreviewModel;
                    rParams["Month"].Value = RptParameters.Month;
                    rParams["Year"].Value = RptParameters.Year;
                    rParams["parLogoUrl"].Value = Globals.ServerConfigSection.CommonItems.ReportLogoUrl;
                    break;
            }

            // ReportModel.AutoShowParametersPanel = false;
            ReportModel.CreateDocument(rParams);
        }
     
        #region Print Member

        public void btnXemIn(object sender, EventArgs e)
        {
            if (GetParameters())
            {
                GetReport();
            }
        }

        public void btnIn(object sender, EventArgs e)
        {
            if (GetParameters())
            {
                switch (_eItem)
                {
                    case ReportName.PHARMACY_NHAPTHUOCHANGTHANG:
                        PrintSilient_NhapThuocHangThang();
                        break;
                    case ReportName.PHARMACY_NHAPHANGTHANGTHEOSOPHIEU:
                        PrintSilient_NhapThuocHangThangInvoice();
                        break;
                }
            }
        }

        private void PrintSilient_NhapThuocHangThang()
        {
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ReportServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginBaocaoNhapThuocHangThangPdfFormat(RptParameters.FromDate.GetValueOrDefault(), RptParameters.ToDate.GetValueOrDefault(), RptParameters.Show, RptParameters.Quarter, RptParameters.Month, RptParameters.Year, RptParameters.Flag, RptParameters.StoreName, RptParameters.StoreID, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndBaocaoNhapThuocHangThangPdfFormat(asyncResult);
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

        private void PrintSilient_NhapThuocHangThangInvoice()
        {
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ReportServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginBaocaoNhapThuocHangThangInvoicePdfFormat(RptParameters.FromDate.GetValueOrDefault(), RptParameters.ToDate.GetValueOrDefault(), RptParameters.Show, RptParameters.Quarter, RptParameters.Month, RptParameters.Year, RptParameters.Flag, RptParameters.StoreName, RptParameters.StoreID, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndBaocaoNhapThuocHangThangInvoicePdfFormat(asyncResult);
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

        #endregion

        ComboBox cbx_ChooseKho = null;
        public void cbx_ChooseKho_Loaded(object sender, RoutedEventArgs e)
        {
            cbx_ChooseKho = sender as ComboBox;
        }

        private bool GetParameters()
        {
            bool result = true;
            if (RptParameters == null)
            {
                return false;
            }

            if (RptParameters.HideStore)
            {
                if (RptParameters.IsTongKho)
                {
                    RptParameters.StoreName = eHCMSResources.Z0936_G1_TgKho;
                    RptParameters.StoreID = null;
                }
                else
                {
                    if (cbx_ChooseKho != null && cbx_ChooseKho.SelectedItem != null)
                    {
                        RptParameters.StoreName = ((RefStorageWarehouseLocation)cbx_ChooseKho.SelectedItem).swhlName;
                        RptParameters.StoreID = ((RefStorageWarehouseLocation)cbx_ChooseKho.SelectedItem).StoreID;
                    }
                    else
                    {
                        Globals.ShowMessage(eHCMSResources.Z1667_G1_ChonKhoCanXem, eHCMSResources.G0442_G1_TBao);
                        return false;
                    }
                }
            }
            return result;
        }
    }
}
