using eHCMSLanguage;
using System.ComponentModel.Composition;
using aEMR.ViewContracts;
using Caliburn.Micro;
using System.Windows;
using System.Windows.Controls;
using System;
using aEMR.Infrastructure;
using System.Collections.ObjectModel;
using DataEntities;
using System.Linq;
using System.Collections.Generic;
using DevExpress.Xpf.Printing;
using aEMR.ReportModel.ReportModels;
using aEMR.CommonTasks;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using DevExpress.ReportServer.Printing;
using aEMR.Common.Collections;

namespace aEMR.StoreDept.Reports.ViewModels
{
    [Export(typeof(IClinicCommonReport)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class ClinicCommonReportViewModel : Conductor<object>, IClinicCommonReport
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public ClinicCommonReportViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            RptParameters = new ReportParameters();
            FillCondition();
            FillMonth();
            FillQuarter();
            FillYear();
        }
        protected override void OnActivate()
        {
            //khi nao share thi dung cai nay
            Coroutine.BeginExecute(DoGetStore_DrugDept());
        }

        #region Properties Member

        private string _strHienThi;
        public string strHienThi
        {
            get
            {
                return _strHienThi;
            }
            set
            {
                _strHienThi = value;
                NotifyOfPropertyChange(() => strHienThi);
            }
        }

        private long _V_MedProductType = (long)AllLookupValues.MedProductType.THUOC; //11001 : thuoc, 11002 : y cu , 11003 :hoa chat
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

        public string strMedProductType
        {
            get
            {
                switch (V_MedProductType)
                {
                    case (long)AllLookupValues.MedProductType.THUOC:
                        return "THUỐC";
                    case (long)AllLookupValues.MedProductType.Y_CU:
                        return "Y CỤ";
                    case (long)AllLookupValues.MedProductType.HOA_CHAT:
                        return "HÓA CHẤT";
                    default:
                        return "";
                }
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

        private bool _IsDetail = true;
        public bool IsDetail
        {
            get
            {
                return _IsDetail;
            }
            set
            {
                _IsDetail = value;
                NotifyOfPropertyChange(() => IsDetail);
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
            RptParameters.Month = DateTime.Now.Month;
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
            int Month = DateTime.Now.Month;
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
            int year = DateTime.Now.Year;
            for (int i = year; i > year - 3; i--)
            {
                ListYear.Add(i);
            }
            RptParameters.Year = year;

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

            Conditions.Add(new Condition("Theo quý", 0));
            Conditions.Add(new Condition("Theo tháng", 1));
            Conditions.Add(new Condition("Theo ngày", 2));

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

        private bool _mXemIn = true;
        private bool _mIn = false;
        private bool _mDetail = false;

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

        public bool mDetail
        {
            get
            {
                return _mDetail;
            }
            set
            {
                if (_mDetail == value)
                    return;
                _mDetail = value;
                NotifyOfPropertyChange(() => mDetail);
            }
        }

        private IEnumerator<IResult> DoGetStore_DrugDept()
        {
            var paymentTypeTask = new LoadStoreListTask((long)AllLookupValues.StoreType.STORAGE_CLINIC, false, null, false, false);
            yield return paymentTypeTask;
            var StoreTemp = paymentTypeTask.LookupList.Where(x => (V_MedProductType != 0 && x.ListV_MedProductType != null && x.ListV_MedProductType.Contains(V_MedProductType.ToString()))).ToObservableCollection();
            StoreCbx = Globals.CheckDrugMedStoreWareHouse(StoreTemp);

            if (StoreCbx == null || StoreCbx.Count == 0)
            {
                MessageBox.Show(eHCMSResources.A0110_G1_Msg_InfoChuaCauHinhTNKho.Replace("\\n ", "\n"));
            }
            yield break;
        }

        private void GetReport()
        {
            DevExpress.DocumentServices.ServiceModel.DefaultValueParameterContainer rParams = new DevExpress.DocumentServices.ServiceModel.DefaultValueParameterContainer();
            switch (_eItem)
            {
                case ReportName.REPORT_CLINIC_OUTWARD_TO_PATIENT:
                    ReportModel = null;
                    if (IsDetail)
                    {
                        ReportModel = new GenericReportModel("eHCMS.ReportLib.RptClinicDepts.XtraReports.XRptOutwardToPatient_Detail").PreviewModel;
                        rParams["ReportTitle"].Value = "BÁO CÁO CHI TIẾT XUẤT " + strMedProductType + " CHO BỆNH NHÂN";
                    }
                    else
                    {
                        ReportModel = new GenericReportModel("eHCMS.ReportLib.RptClinicDepts.XtraReports.XRptOutwardToPatient").PreviewModel;
                        rParams["ReportTitle"].Value = "BÁO CÁO TỔNG HỢP XUẤT " + strMedProductType + " CHO BỆNH NHÂN";
                    }
                    rParams["Quarter"].Value = RptParameters.Quarter;
                    rParams["Month"].Value = RptParameters.Month;
                    rParams["Year"].Value = RptParameters.Year;
                    rParams["flag"].Value = RptParameters.Flag;
                    rParams["FromDate"].Value = RptParameters.FromDate;
                    rParams["ToDate"].Value = RptParameters.ToDate;
                    rParams["StoreID"].Value = (int)RptParameters.StoreID.GetValueOrDefault();
                    rParams["V_MedProductType"].Value = (int)V_MedProductType;

                    rParams["StoreName"].Value = RptParameters.StoreName;
                    rParams["ReportDate"].Value = RptParameters.Show;
                    rParams["parLogoUrl"].Value = Globals.ServerConfigSection.CommonItems.ReportLogoUrl;

                    break;
                case ReportName.XRptPhieuTreo:
                    ReportModel = null;
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.RptDrugDepts.XtraReports.Reports.XRptPhieuTreo").PreviewModel;
                    rParams["Quarter"].Value = RptParameters.Quarter;
                    rParams["Month"].Value = RptParameters.Month;
                    rParams["Year"].Value = RptParameters.Year;
                    rParams["Flag"].Value = RptParameters.Flag;
                    rParams["FromDate"].Value = RptParameters.FromDate;
                    rParams["ToDate"].Value = RptParameters.ToDate;
                    rParams["StoreID"].Value = (int)RptParameters.StoreID.GetValueOrDefault();
                    //rParams["parLogoUrl"].Value = Globals.ServerConfigSection.CommonItems.ReportLogoUrl;
                    //rParams["parStaffName"].Value = Globals.AllStaffs.Where(x => x.StaffID == (int)Globals.LoggedUserAccount.StaffID).FirstOrDefault().FullName.ToString();
                    //rParams["Show"].Value = RptParameters.Show;
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
                    RptParameters.StoreName = "Tổng kho";
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
            if (CurrentCondition == null)
            {
                CurrentCondition = new Condition(eHCMSResources.Z0938_G1_TheoQuy, 0);
            }
            if (CurrentCondition.Value == 0)
            {
                RptParameters.Flag = 0;
                RptParameters.Show = RptParameters.ShowFirst + string.Format(" {0} ", eHCMSResources.Q0486_G1_Quy.ToUpper()) + RptParameters.Quarter.ToString() + string.Format(" {0} ", eHCMSResources.N0033_G1_Nam.ToUpper()) + RptParameters.Year.ToString();

            }
            else if (CurrentCondition.Value == 1)
            {
                RptParameters.Flag = 1;
                RptParameters.Show = RptParameters.ShowFirst + string.Format(" {0} ", eHCMSResources.G0039_G1_Th.ToUpper()) + RptParameters.Month.ToString() + string.Format(" {0} ", eHCMSResources.N0033_G1_Nam.ToUpper()) + RptParameters.Year.ToString();
            }
            else
            {
                RptParameters.Flag = 2;
                if (RptParameters.FromDate == null || RptParameters.ToDate == null)
                {
                    Globals.ShowMessage(eHCMSResources.K0364_G1_ChonNgThCanXemBC, eHCMSResources.G0442_G1_TBao);
                    result = false;
                }
                else
                {
                    if (RptParameters.FromDate.GetValueOrDefault() > RptParameters.ToDate.GetValueOrDefault())
                    {
                        MessageBox.Show(eHCMSResources.A0857_G1_Msg_InfoNgThangKhHopLe2);
                        return false;
                    }
                    if (RptParameters.FromDate.GetValueOrDefault().Date == RptParameters.ToDate.GetValueOrDefault().Date)
                    {
                        RptParameters.Show = RptParameters.ShowFirst + string.Format(" {0} ", eHCMSResources.N0045_G1_Ng.ToUpper()) + RptParameters.FromDate.GetValueOrDefault().ToString("dd/MM/yyyy");

                    }
                    else
                    {
                        RptParameters.Show = RptParameters.ShowFirst + string.Format(" {0} ", eHCMSResources.G1933_G1_TuNg.ToUpper()) + RptParameters.FromDate.GetValueOrDefault().ToString("dd/MM/yyyy") + string.Format(" - {0} ", eHCMSResources.K3192_G1_DenNg.ToUpper()) + RptParameters.ToDate.GetValueOrDefault().ToString("dd/MM/yyyy");
                    }

                }
            }
            return result;
        }


        public void rdtChitiet_Checked(object sender, RoutedEventArgs e)
        {
            IsDetail = true;
        }

        public void rdtTongHop_Checked(object sender, RoutedEventArgs e)
        {
            IsDetail = false;
        }


    }
}
