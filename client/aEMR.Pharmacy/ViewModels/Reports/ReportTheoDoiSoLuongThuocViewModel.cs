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
using eHCMSLanguage;
using DevExpress.ReportServer.Printing;
/*
* 20171117 #001 CMN: Fixed load only 3 year in combobox year.
*/
namespace aEMR.Pharmacy.ViewModels
{
    [Export(typeof(IReportTheoDoiSoLuongThuoc)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class ReportTheoDoiSoLuongThuocViewModel : Conductor<object>, IReportTheoDoiSoLuongThuoc
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public ReportTheoDoiSoLuongThuocViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            authorization();
            RptParameters = new ReportParameters();
            FillMonth();
            FillYear();

            Coroutine.BeginExecute(DoGetStore_EXTERNAL());
            Coroutine.BeginExecute(DoDrugClassList());
            Coroutine.BeginExecute(DoRefGenericDrugCategory_2List());
        }

        protected override void OnDeactivate(bool close)
        {
            //base.OnDeactivate(close);
            ReportModel = null;
            RptParameters = null;
            StoreCbx = null;
            ListMonth = null;
            ListYear = null;
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

        private Visibility _IsDuocLy = Visibility.Visible;
        public Visibility IsDuocLy
        {
            get
            { return _IsDuocLy; }
            set
            {
                if (_IsDuocLy != value)
                {
                    _IsDuocLy = value;
                    NotifyOfPropertyChange(() => IsDuocLy);
                    NotifyOfPropertyChange(() => IsDuocChinh);
                }
            }
        }

        public Visibility IsDuocChinh
        {
            get
            {
                if (IsDuocLy == Visibility.Collapsed)
                {
                    return Visibility.Visible;
                }
                else
                {
                    return Visibility.Collapsed;
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
            /*▼====: #001*/
            //for (int i = year; i > year - 3; i--)
            for (int i = year; i >= 2014; i--)
            /*▲====: #001*/
            {
                ListYear.Add(i);
            }
            RptParameters.Year = ListYear.FirstOrDefault();

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
                case ReportName.PHARMACY_THEODOISOLUONGTHUOC:
                    ReportModel = null;
                    ReportModel = new PharmacyTheoDoiSoLuongThuocReportModel().PreviewModel;
                    rParams["Month"].Value = RptParameters.Month;
                    rParams["Year"].Value = RptParameters.Year;
                    if (RptParameters.SelectedDrugClass != null)
                    {
                        rParams["DrugClassID"].Value = (int)RptParameters.SelectedDrugClass.DrugClassID;
                    }
                    else
                    {
                        rParams["DrugClassID"].Value = 0;
                    }
                    rParams["RefGenDrugCatID_1"].Value = (int)RptParameters.RefGenDrugCatID_1.GetValueOrDefault();
                    rParams["StoreID"].Value = (int)RptParameters.StoreID.GetValueOrDefault();
                    rParams["ShowTilte"].Value = RptParameters.ShowTitle;
                    rParams["Show"].Value = RptParameters.Show;
                    rParams["parHospitalName"].Value = Globals.ServerConfigSection.CommonItems.ReportHospitalName;
                    rParams["parDepartmentOfHealth"].Value = Globals.ServerConfigSection.CommonItems.ReportDepartmentOfHealth;
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
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            IsLoading = true;
            var t = new Thread(() =>
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
                            IsLoading = false;
                            Globals.IsBusy = false;
                        }

                    }), null);

                }

            });
            t.Start();
        }

        private void PrintSilient_NhapThuocHangThangInvoice()
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            IsLoading = true;
            var t = new Thread(() =>
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
                            IsLoading = false;
                            Globals.IsBusy = false;
                        }

                    }), null);

                }

            });
            t.Start();
        }

        #endregion

        private ObservableCollection<DrugClass> _familytherapies;
        public ObservableCollection<DrugClass> FamilyTherapies
        {
            get
            {
                return _familytherapies;
            }
            set
            {
                if (_familytherapies != value)
                {
                    _familytherapies = value;
                    NotifyOfPropertyChange(() => FamilyTherapies);
                }
            }
        }

        private ObservableCollection<RefGenericDrugCategory_2> _RefGenericDrugCategory_2s;
        public ObservableCollection<RefGenericDrugCategory_2> RefGenericDrugCategory_2s
        {
            get
            {
                return _RefGenericDrugCategory_2s;
            }
            set
            {
                if (_RefGenericDrugCategory_2s != value)
                {
                    _RefGenericDrugCategory_2s = value;
                    NotifyOfPropertyChange(() => RefGenericDrugCategory_2s);
                }
            }
        }


        private long V_MedProductType = (long)AllLookupValues.MedProductType.THUOC;
        private IEnumerator<IResult> DoDrugClassList()
        {
            var paymentTypeTask1 = new LoadDrugClassListTask(V_MedProductType, false, false);
            yield return paymentTypeTask1;
            FamilyTherapies = paymentTypeTask1.DrugClassList;
            yield break;


        }

        private IEnumerator<IResult> DoRefGenericDrugCategory_2List()
        {
            var paymentTypeTask2 = new LoadRefGenericDrugCategory_2ListTask(false, false);
            yield return paymentTypeTask2;
            RefGenericDrugCategory_2s = paymentTypeTask2.RefGenericDrugCategory_2List;
            yield break;
        }
        ComboBox cbx_ChooseKho = null;
        public void cbx_ChooseKho_Loaded(object sender, RoutedEventArgs e)
        {
            cbx_ChooseKho = sender as ComboBox;
        }

        ComboBox cbxChooseRefGenDrugCatID = null;
        public void cbxChooseRefGenDrugCatID_Loaded(object sender, RoutedEventArgs e)
        {
            cbxChooseRefGenDrugCatID = sender as ComboBox;
        }

        public void chkDuocLy_Checked(object sender, RoutedEventArgs e)
        {
            IsDuocLy = Visibility.Visible;
           
            cbxChooseRefGenDrugCatID.SelectedItem = null;
            RptParameters.RefGenDrugCatID_1 = null;
        }

        public void chkDuocChinh_Checked(object sender, RoutedEventArgs e)
        {
            IsDuocLy = Visibility.Collapsed;
          
            RptParameters.SelectedDrugClass = null;
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
            if (IsDuocLy == Visibility.Visible)
            {
                if (RptParameters.SelectedDrugClass == null || RptParameters.SelectedDrugClass.DrugClassID == 0)
                {
                    Globals.ShowMessage(eHCMSResources.Z1668_G1_ChonDuocLy, eHCMSResources.G0442_G1_TBao);
                    return false;
                }
                RptParameters.ShowTitle = eHCMSResources.G0363_G1_TheoDoiSLggThuocTheoDuocLy;
                RptParameters.Show = RptParameters.SelectedDrugClass.FaName.ToUpper() + " - " + RptParameters.SelectedDrugClass.FaDescription.ToUpper();
            }
            else
            {
                if (cbxChooseRefGenDrugCatID == null || cbxChooseRefGenDrugCatID.SelectedItem == null)
                {
                    Globals.ShowMessage(eHCMSResources.Z1669_G1_ChonDuocChinh, eHCMSResources.G0442_G1_TBao);
                    return false;
                }
                RptParameters.ShowTitle = eHCMSResources.G0362_G1_TheoDoiSLggThuocTheoDuocChinh;
                RptParameters.Show = (cbxChooseRefGenDrugCatID.SelectedItem as RefGenericDrugCategory_2).CategoryDescription.ToUpper();
            }
            return result;
        }
    }
}
