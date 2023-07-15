using System.ComponentModel.Composition;
using aEMR.ViewContracts;
using Caliburn.Micro;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System;
using aEMR.Infrastructure;
using System.Collections.ObjectModel;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using System.Threading;
using DataEntities;
using aEMR.Common.Collections;
using System.Linq;
using System.Collections.Generic;
using DevExpress.Xpf.Printing;
using aEMR.Common.Printing;
using aEMR.CommonTasks;
using eHCMSLanguage;
using aEMR.ReportModel.ReportModels;
using Microsoft.Win32;
using Castle.Windsor;
using DevExpress.ReportServer.Printing;
/*
* 20171117 #001 CMN: Fixed load only 3 year in combobox year.
* 20190308 #002 TNHX: Create BC_THUTIEN_NT_TAI_QUAY_BHYT
* 20190503 #003 TNHX: [BM0006812] [BM0006813] Create XRpt_TKTheoDoiTTChiTietKH_NT, TKTheoDoiNXTThuocKhac_NT
* 20190610 #004 TNHX: [BM0010768] Create XRpt_BCXuatDuocNoiBo_NT
* 20190704 #005 TNHX: [BM0011926] Create XRpt_BCBanThuocLe
* 20200105 #006 TNHX: [] Create XRpt_BCThuTienNTTaiQuayBHYT_TheoBienLai
* 20230224 #007 QTD:  Thêm BC Huỷ đẩy cổng đơn thuốc quốc gia
*/
namespace aEMR.Pharmacy.ViewModels
{
    [Export(typeof(IReportByDDMMYYYY)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class ReportByDDMMYYYYViewModel : Conductor<object>, IReportByDDMMYYYY
    {
        [ImportingConstructor]
        public ReportByDDMMYYYYViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg)
        {
            authorization();
            RptParameters = new ReportParameters();
            RptParameters.TypID = 0;
            FillCondition();
            FillMonth();
            FillQuarter();
            FillYear();
            FillInwardSource();

            Lookup firstItem = new Lookup
            {
                LookupID = 0,
                ObjectValue = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.T0822_G1_TatCa)
            };

            //▼====20210321 QTD Thêm danh sách phương thức thanh toán
            AllPaymentMode = Globals.AllLookupValueList.Where(x => x.ObjectTypeID == (long)LookupValues.PAYMENT_MODE).ToObservableCollection();      
            AllPaymentMode.Insert(0, firstItem);
            SetDefaultPaymentMode();
            //▲====20210321

            Coroutine.BeginExecute(DoGetStore_EXTERNAL());
            Coroutine.BeginExecute(DoRefOutputType_All());
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
            InwardSources = null;
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

        //▼====: #003
        private bool _XemChiTiet = true;
        public bool BXemChiTiet
        {
            get { return _XemChiTiet; }
            set
            {
                if (_XemChiTiet == value)
                    return;
                _XemChiTiet = value;
            }
        }
        //▲====: #003

        private bool _bXuatExcel;
        public bool bXuatExcel
        {
            get { return _bXuatExcel; }
            set
            {
                if (_bXuatExcel == value)
                    return;
                _bXuatExcel = value;
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

        public class InwardSource
        {
            private string _Text;
            private int _Value;
            public string Text { get { return _Text; } }
            public int Value { get { return _Value; } }
            public InwardSource(string theText, int theValue)
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


        private ObservableCollection<InwardSource> _InwardSources;
        public ObservableCollection<InwardSource> InwardSources
        {
            get
            {
                return _InwardSources;
            }
            set
            {
                if (_InwardSources != value)
                {
                    _InwardSources = value;
                    NotifyOfPropertyChange(() => InwardSources);
                }
            }
        }

        private InwardSource _CurrentInwardSource;
        public InwardSource CurrentInwardSource
        {
            get
            {
                return _CurrentInwardSource;
            }
            set
            {
                if (_CurrentInwardSource != value)
                {
                    _CurrentInwardSource = value;
                    NotifyOfPropertyChange(() => CurrentInwardSource);
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
            /*▼====: #001*/
            //for (int i = year; i > year - 3; i--)
            for (int i = year; i >= 2014; i--)
            /*▲====: #001*/
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

        private void FillInwardSource()
        {
            if (InwardSources == null)
            {
                InwardSources = new ObservableCollection<InwardSource>();
            }
            else
            {
                InwardSources.Clear();
            }

            InwardSources.Add(new InwardSource(eHCMSResources.T0822_G1_TatCa, 0));
            InwardSources.Add(new InwardSource(eHCMSResources.A0826_G1_Msg_NCC, 1));
            InwardSources.Add(new InwardSource(eHCMSResources.N0170_G1_NguonKhac, 2));
            

            CurrentInwardSource = InwardSources[1];
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

        private ObservableCollection<RefOutputType> _RefOutputTypeList;
        public ObservableCollection<RefOutputType> RefOutputTypeList
        {
            get { return _RefOutputTypeList; }
            set
            {
                _RefOutputTypeList = value;
                NotifyOfPropertyChange(() => RefOutputTypeList);
            }
        }

        private IEnumerator<IResult> DoRefOutputType_All()
        {
            var paymentTypeTask = new LoadOutputListTask(false, false, true);
            yield return paymentTypeTask;
            RefOutputTypeList = paymentTypeTask.RefOutputTypeList.Where(x => x.IsSelectedPharmacyInternal == true).ToObservableCollection();
            RefOutputTypeList.Insert(0, new RefOutputType { TypID = 0, TypName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.T0822_G1_TatCa)
                , TypNamePharmacy = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.T0822_G1_TatCa)
            });
          
            //SetDefaultForStore();
            yield break;
        }

        private IEnumerator<IResult> DoGetStore_EXTERNAL()
        {
            var paymentTypeTask = new LoadStoreListTask((long)AllLookupValues.StoreType.STORAGE_EXTERNAL, false,null, false, false);
            yield return paymentTypeTask;
            StoreCbx = paymentTypeTask.LookupList;
            yield break;
        }

        //▼====20210321 QTD Thêm danh sách phương thức thanh toán
        private ObservableCollection<Lookup> _AllPaymentMode;
        public ObservableCollection<Lookup> AllPaymentMode
        {
            get
            {
                return _AllPaymentMode;
            }
            set
            {
                if (_AllPaymentMode != value)
                {
                    _AllPaymentMode = value;
                    NotifyOfPropertyChange(() => AllPaymentMode);
                }
            }
        }

        private void SetDefaultPaymentMode()
        {
            if (AllPaymentMode != null)
            {
                RptParameters.V_PaymentMode = AllPaymentMode.FirstOrDefault(); //--27/01/2021 DatTB Fix Select default
            }
        }

        private bool _IsShowPaymentMode;
        public bool IsShowPaymentMode
        {
            get
            {
                return _IsShowPaymentMode;
            }
            set
            {
                if (_IsShowPaymentMode == value)
                    return;
                _IsShowPaymentMode = value;
                NotifyOfPropertyChange(() => IsShowPaymentMode);
            }
        }
        //▲====20210321


        private void GetReport(bool isDetail = false)
        {
            //if (ReportModel != null)
            //{
            //    ReportModel.RequestDefaultParameterValues -= new WeakEventHandler<EventArgs>(_reportModel_RequestDefaultParameterValues).Handler;
            //}
            var GiamDoc = Globals.allStaffPositions.Where(x => x.PositionRefID == (int)AllLookupValues.StaffPositions_Enum.GIAM_DOC && x.IsActive).FirstOrDefault();
            var KeToanTruong = Globals.allStaffPositions.Where(x => x.PositionRefID == (int)AllLookupValues.StaffPositions_Enum.KE_TOAN_TRUONG && x.IsActive).FirstOrDefault();
            var TruongKhoaDuoc = Globals.allStaffPositions.Where(x => x.PositionRefID == (int)AllLookupValues.StaffPositions_Enum.TRUONG_KHOA_DUOC && x.IsActive).FirstOrDefault();
            var TruongNhaThuoc = Globals.allStaffPositions.Where(x => x.PositionRefID == (int)AllLookupValues.StaffPositions_Enum.TRUONG_NHA_THUOC && x.IsActive).FirstOrDefault();
            var ThongKeDuoc = Globals.allStaffPositions.Where(x => x.PositionRefID == (int)AllLookupValues.StaffPositions_Enum.THONG_KE_DUOC_NHA_THUOC && x.IsActive).FirstOrDefault();
            var ThuKho = Globals.allStaffPositions.Where(x => x.PositionRefID == (int)AllLookupValues.StaffPositions_Enum.THU_KHO_NHA_THUOC && x.IsActive).FirstOrDefault();
            DevExpress.DocumentServices.ServiceModel.DefaultValueParameterContainer rParams = new DevExpress.DocumentServices.ServiceModel.DefaultValueParameterContainer();
            switch (_eItem)
            {
                case ReportName.PHARMACY_NHAPTHUOCHANGTHANG:
                    ReportModel = null;
                    ReportModel = new NhapThuocHangThangReportModel().PreviewModel;
                    rParams["Quarter"].Value = RptParameters.Quarter;
                    rParams["Month"].Value = RptParameters.Month;
                    rParams["Year"].Value = RptParameters.Year;
                    rParams["Flag"].Value = RptParameters.Flag;
                    rParams["FromDate"].Value = RptParameters.FromDate;
                    rParams["ToDate"].Value = RptParameters.ToDate;
                    rParams["Show"].Value = RptParameters.Show;
                    rParams["StoreName"].Value = RptParameters.StoreName;
                    rParams["StoreID"].Value = (int)RptParameters.StoreID.GetValueOrDefault();
                    rParams["InwardSource"].Value = RptParameters.InwardSource;
                    rParams["parLogoUrl"].Value = Globals.ServerConfigSection.CommonItems.ReportLogoUrl;
                    break;
                case ReportName.PHARMACY_NHAPHANGTHANGTHEOSOPHIEU:
                    ReportModel = null;
                    ReportModel = new NhapHangThangTheoSoPhieuReportModel().PreviewModel;
                    rParams["Quarter"].Value = RptParameters.Quarter;
                    rParams["Month"].Value = RptParameters.Month;
                    rParams["Year"].Value = RptParameters.Year;
                    rParams["Flag"].Value = RptParameters.Flag;
                    rParams["FromDate"].Value = RptParameters.FromDate;
                    rParams["ToDate"].Value = RptParameters.ToDate;
                    rParams["Show"].Value = RptParameters.Show;
                    rParams["StoreName"].Value = RptParameters.StoreName;
                    rParams["StoreID"].Value = (int)RptParameters.StoreID.GetValueOrDefault();
                    rParams["InwardSource"].Value = RptParameters.InwardSource;
                    rParams["parLogoUrl"].Value = Globals.ServerConfigSection.CommonItems.ReportLogoUrl;
                    break;
                case ReportName.PHARMACY_SOKIEMNHAPTHUOC:
                    ReportModel = null;
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.RptPharmacies.SoKiemNhapThuoc").PreviewModel;
                    rParams["Quarter"].Value = RptParameters.Quarter;
                    rParams["Month"].Value = RptParameters.Month;
                    rParams["Year"].Value = RptParameters.Year;
                    rParams["Flag"].Value = RptParameters.Flag;
                    rParams["FromDate"].Value = RptParameters.FromDate;
                    rParams["ToDate"].Value = RptParameters.ToDate;
                    rParams["Show"].Value = RptParameters.Show;
                    rParams["StoreName"].Value = RptParameters.StoreName;
                    rParams["StoreID"].Value = (int)RptParameters.StoreID.GetValueOrDefault();
                    rParams["KeToanTruong"].Value = KeToanTruong != null ? KeToanTruong.FullNameString : "";
                    rParams["TruongKhoaDuoc"].Value = TruongKhoaDuoc != null ? TruongKhoaDuoc.FullNameString : "";
                    rParams["TruongNhaThuoc"].Value = TruongNhaThuoc != null ? TruongNhaThuoc.FullNameString : "";
                    rParams["ThongKeDuoc"].Value = ThongKeDuoc != null ? ThongKeDuoc.FullNameString : "";
                    rParams["ThuKho"].Value = ThuKho != null ? ThuKho.FullNameString : "";
                    rParams["parLogoUrl"].Value = Globals.ServerConfigSection.CommonItems.ReportLogoUrl;
                    break;
                case ReportName.PHARMACY_XUATNOIBOTHEONGUOIBAN:
                    ReportModel = null;
                    ReportModel = new XuatNoiBoTheoNguoiMuaReportModel().PreviewModel;
                    rParams["Quarter"].Value = RptParameters.Quarter;
                    rParams["Month"].Value = RptParameters.Month;
                    rParams["Year"].Value = RptParameters.Year;
                    rParams["Flag"].Value = RptParameters.Flag;
                    rParams["FromDate"].Value = RptParameters.FromDate;
                    rParams["ToDate"].Value = RptParameters.ToDate;
                    rParams["Show"].Value = RptParameters.Show;
                    rParams["StoreName"].Value = RptParameters.StoreName;
                    rParams["StoreID"].Value = (int)RptParameters.StoreID.GetValueOrDefault();
                    rParams["OutTo"].Value = (int)RptParameters.OutTo.GetValueOrDefault(0);
                    rParams["TypID"].Value = (int)RptParameters.TypID.GetValueOrDefault();
                    rParams["parLogoUrl"].Value = Globals.ServerConfigSection.CommonItems.ReportLogoUrl;
                    break;
                case ReportName.PHARMACY_XUATNOIBOTHEOTENTHUOC:
                    ReportModel = null;
                    ReportModel = new XuatNoiBoTheoTenThuocReportModel().PreviewModel;
                    rParams["Quarter"].Value = RptParameters.Quarter;
                    rParams["Month"].Value = RptParameters.Month;
                    rParams["Year"].Value = RptParameters.Year;
                    rParams["Flag"].Value = RptParameters.Flag;
                    rParams["FromDate"].Value = RptParameters.FromDate;
                    rParams["ToDate"].Value = RptParameters.ToDate;
                    rParams["Show"].Value = RptParameters.Show;
                    rParams["StoreName"].Value = RptParameters.StoreName;
                    rParams["StoreID"].Value = (int)RptParameters.StoreID.GetValueOrDefault();
                    rParams["OutTo"].Value = (int)RptParameters.OutTo.GetValueOrDefault(0);
                    rParams["TypID"].Value = (int)RptParameters.TypID.GetValueOrDefault();
                    rParams["parLogoUrl"].Value = Globals.ServerConfigSection.CommonItems.ReportLogoUrl;
                    break;
                case ReportName.PHARMACY_XUATTHUOCCHOBH:
                    ReportModel = null;
                    ReportModel = new XuatThuocChoBHReportModel().PreviewModel;
                    rParams["Quarter"].Value = RptParameters.Quarter;
                    rParams["Month"].Value = RptParameters.Month;
                    rParams["Year"].Value = RptParameters.Year;
                    rParams["Flag"].Value = RptParameters.Flag;
                    rParams["FromDate"].Value = RptParameters.FromDate;
                    rParams["ToDate"].Value = RptParameters.ToDate;
                    rParams["Show"].Value = RptParameters.Show;
                    rParams["StoreName"].Value = RptParameters.StoreName;
                    rParams["StoreID"].Value = (int)RptParameters.StoreID.GetValueOrDefault();
                    rParams["parLogoUrl"].Value = Globals.ServerConfigSection.CommonItems.ReportLogoUrl;
                    break;
                case ReportName.PHARMACY_THEODOITHUOCCOGIOIHANSL:
                    ReportModel = null;
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.RptPharmacies.XRptTheoDoiThuocCoGioiHanSL").PreviewModel;
                    rParams["Quarter"].Value = RptParameters.Quarter;
                    rParams["Month"].Value = RptParameters.Month;
                    rParams["Year"].Value = RptParameters.Year;
                    rParams["Flag"].Value = RptParameters.Flag;
                    rParams["FromDate"].Value = RptParameters.FromDate;
                    rParams["ToDate"].Value = RptParameters.ToDate;
                    rParams["Show"].Value = RptParameters.Show;
                    rParams["StoreName"].Value = RptParameters.StoreName;
                    rParams["StoreID"].Value = (int)RptParameters.StoreID.GetValueOrDefault();
                    rParams["parLogoUrl"].Value = Globals.ServerConfigSection.CommonItems.ReportLogoUrl;
                    break;
                case ReportName.PHARMACY_BANTHUOCTH:
                    ReportModel = null;
                    ReportModel = new BanThuocTongHopReportModel().PreviewModel;
                    rParams["Quarter"].Value = RptParameters.Quarter;
                    rParams["Month"].Value = RptParameters.Month;
                    rParams["Year"].Value = RptParameters.Year;
                    rParams["Flag"].Value = RptParameters.Flag;
                    rParams["FromDate"].Value = RptParameters.FromDate;
                    rParams["ToDate"].Value = RptParameters.ToDate;
                    rParams["Show"].Value = RptParameters.Show;
                    rParams["parLogoUrl"].Value = Globals.ServerConfigSection.CommonItems.ReportLogoUrl;
                    rParams["V_PaymentMode"].Value = RptParameters.V_PaymentMode != null ? RptParameters.V_PaymentMode.LookupID : 0; //20210321 QTD Thêm Param phương thức thanh toán   
                    if (RptParameters.V_PaymentMode != null && RptParameters.V_PaymentMode.LookupID > 0)
                    {
                        rParams["strPaymentMode"].Value = RptParameters.V_PaymentMode.ObjectValue;
                    }
                    else rParams["strPaymentMode"].Value = "Tất cả";
                    rParams["StoreID"].Value = RptParameters.StoreID.GetValueOrDefault();
                    rParams["parStaffFullName"].Value = Globals.AllStaffs.Where(x => x.StaffID == (int)Globals.LoggedUserAccount.StaffID).FirstOrDefault().FullName.ToString();
                    break;
                case ReportName.PHARMACY_TRATHUOCTH:
                    ReportModel = null;
                    ReportModel = new TraThuocTongHopReportModel().PreviewModel;
                    rParams["Quarter"].Value = RptParameters.Quarter;
                    rParams["Month"].Value = RptParameters.Month;
                    rParams["Year"].Value = RptParameters.Year;
                    rParams["Flag"].Value = RptParameters.Flag;
                    rParams["FromDate"].Value = RptParameters.FromDate;
                    rParams["ToDate"].Value = RptParameters.ToDate;
                    rParams["Show"].Value = RptParameters.Show;
                    rParams["parLogoUrl"].Value = Globals.ServerConfigSection.CommonItems.ReportLogoUrl;
                    break;
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
                case ReportName.PHARMACY_TONGHOPDUTRU_SOPHIEU:
                    ReportModel = null;
                    ReportModel = new PharmacyTongHopDuTruTheoSoPhieuReportModel().PreviewModel;
                    rParams["Quarter"].Value = RptParameters.Quarter;
                    rParams["Month"].Value = RptParameters.Month;
                    rParams["Year"].Value = RptParameters.Year;
                    rParams["Flag"].Value = RptParameters.Flag;
                    rParams["FromDate"].Value = RptParameters.FromDate;
                    rParams["ToDate"].Value = RptParameters.ToDate;
                    rParams["Show"].Value = RptParameters.Show;
                    rParams["Show"].Value = RptParameters.Show;
                    rParams["parLogoUrl"].Value = Globals.ServerConfigSection.CommonItems.ReportLogoUrl;
                    break;
                case ReportName.PHARMACY_TONGHOPDUTRU_TENTHUOC:
                    ReportModel = null;
                    ReportModel = new PharmacyTongHopDuTruTheoTenThuocReportModel().PreviewModel;
                    rParams["Quarter"].Value = RptParameters.Quarter;
                    rParams["Month"].Value = RptParameters.Month;
                    rParams["Year"].Value = RptParameters.Year;
                    rParams["Flag"].Value = RptParameters.Flag;
                    rParams["FromDate"].Value = RptParameters.FromDate;
                    rParams["ToDate"].Value = RptParameters.ToDate;
                    rParams["Show"].Value = RptParameters.Show;
                    rParams["parLogoUrl"].Value = Globals.ServerConfigSection.CommonItems.ReportLogoUrl;
                    break;
                case ReportName.PHARMACY_THEODOICONGNO:
                    ReportModel = null;
                    ReportModel = new PharmacyTheoDoiCongNoReportModel().PreviewModel;
                    rParams["Quarter"].Value = RptParameters.Quarter;
                    rParams["Month"].Value = RptParameters.Month;
                    rParams["Year"].Value = RptParameters.Year;
                    rParams["Flag"].Value = RptParameters.Flag;
                    rParams["FromDate"].Value = RptParameters.FromDate;
                    rParams["ToDate"].Value = RptParameters.ToDate;
                    rParams["Show"].Value = RptParameters.Show;
                    rParams["ShowTitle"].Value = RptParameters.ShowTitle;
                    rParams["Type"].Value = (int)RptParameters.TypCongNo.GetValueOrDefault(0);
                    rParams["StoreID"].Value = (int)RptParameters.StoreID.GetValueOrDefault(0);
                    rParams["StoreName"].Value = RptParameters.StoreName;
                    rParams["parLogoUrl"].Value = Globals.ServerConfigSection.CommonItems.ReportLogoUrl;
                    break;
                case ReportName.TEMP20_NGOAITRU:
                    ReportModel = null;
                    ReportModel = new TransactionTemp20NgoaiTru().PreviewModel;
                    rParams["Quarter"].Value = RptParameters.Quarter;
                    rParams["Month"].Value = RptParameters.Month;
                    rParams["Year"].Value = RptParameters.Year;
                    rParams["Flag"].Value = RptParameters.Flag;
                    rParams["FromDate"].Value = RptParameters.FromDate;
                    rParams["ToDate"].Value = RptParameters.ToDate;
                    rParams["Show"].Value = RptParameters.Show;
                    rParams["parLogoUrl"].Value = Globals.ServerConfigSection.CommonItems.ReportLogoUrl;
                    break;
                //▼====: #002
                case ReportName.BC_THUTIEN_NT_TAI_QUAY_BHYT:
                    ReportModel = null;
                    if (isDetail)
                    {
                        ReportModel = new XRpt_BCThuTienNTTaiQuayBHYTDetail().PreviewModel;
                    }
                    else ReportModel = new XRpt_BCThuTienNTTaiQuayBHYT().PreviewModel;
                    rParams["Quarter"].Value = RptParameters.Quarter;
                    rParams["Month"].Value = RptParameters.Month;
                    rParams["Year"].Value = RptParameters.Year;
                    rParams["Flag"].Value = RptParameters.Flag;
                    rParams["FromDate"].Value = RptParameters.FromDate;
                    rParams["ToDate"].Value = RptParameters.ToDate;
                    rParams["StoreID"].Value = (int)RptParameters.StoreID.GetValueOrDefault();
                    rParams["parLogoUrl"].Value = Globals.ServerConfigSection.CommonItems.ReportLogoUrl;
                    rParams["parStaffName"].Value = Globals.AllStaffs.Where(x => x.StaffID == (int)Globals.LoggedUserAccount.StaffID).FirstOrDefault().FullName.ToString();
                    rParams["Show"].Value = RptParameters.Show;
                    break;
                //▲====: #002
                //▼====: #003
                case ReportName.TKTheoDoiTTChiTietKH_NT:
                    ReportModel = null;
                    ReportModel = new XRpt_TKTheoDoiTTChiTietKH_NT().PreviewModel;
                    rParams["FromDate"].Value = RptParameters.FromDate;
                    rParams["ToDate"].Value = RptParameters.ToDate;
                    rParams["StoreID"].Value = (int)RptParameters.StoreID.GetValueOrDefault();
                    rParams["parHospitalName"].Value = Globals.ServerConfigSection.CommonItems.ReportHospitalName;
                    break;
                case ReportName.TKTheoDoiNXTThuocKhac_NT:
                    ReportModel = null;
                    ReportModel = new XRpt_TKTheoDoiNXTThuocKhac_NT().PreviewModel;
                    rParams["FromDate"].Value = RptParameters.FromDate;
                    rParams["ToDate"].Value = RptParameters.ToDate;
                    rParams["StoreID"].Value = (int)RptParameters.StoreID.GetValueOrDefault();
                    rParams["parHospitalName"].Value = Globals.ServerConfigSection.CommonItems.ReportHospitalName;
                    break;
                //▲====: #003
                //▲====: #004
                case ReportName.BCXuatDuocNoiBo_NT:
                    ReportModel = null;
                    ReportModel = new XRpt_BCXuatDuocNoiBo_NT().PreviewModel;
                    rParams["FromDate"].Value = RptParameters.FromDate;
                    rParams["ToDate"].Value = RptParameters.ToDate;
                    rParams["StoreID"].Value = (int)RptParameters.StoreID.GetValueOrDefault();
                    rParams["Show"].Value = RptParameters.Show;
                    break;
                //▲====: #004
                //▼====: #005
                case ReportName.NT_BCBanThuocLe:
                    ReportModel = null;
                    ReportModel = new XRpt_BCBanThuocLe().PreviewModel;
                    rParams["FromDate"].Value = RptParameters.FromDate;
                    rParams["ToDate"].Value = RptParameters.ToDate;
                    rParams["StoreID"].Value = RptParameters.StoreID.GetValueOrDefault();
                    rParams["parPharmacyName"].Value = Globals.ServerConfigSection.CommonItems.DQGUnitname;
                    rParams["parStaffName"].Value = Globals.AllStaffs.Where(x => x.StaffID == (int)Globals.LoggedUserAccount.StaffID).FirstOrDefault().FullName.ToString();
                    rParams["Show"].Value = RptParameters.Show;
                    break;
                //▲====: #005
                //▼====: #006
                case ReportName.BC_THUTIEN_NT_TAI_QUAY_BHYT_THEO_BIEN_LAI:
                    ReportModel = null;
                    ReportModel = new XRpt_BCThuTienNTTaiQuayBHYT_TheoBienLai().PreviewModel;
                    rParams["Quarter"].Value = RptParameters.Quarter;
                    rParams["Month"].Value = RptParameters.Month;
                    rParams["Year"].Value = RptParameters.Year;
                    rParams["Flag"].Value = RptParameters.Flag;
                    rParams["FromDate"].Value = RptParameters.FromDate;
                    rParams["ToDate"].Value = RptParameters.ToDate;
                    rParams["StoreID"].Value = (int)RptParameters.StoreID.GetValueOrDefault();
                    rParams["parLogoUrl"].Value = Globals.ServerConfigSection.CommonItems.ReportLogoUrl;
                    rParams["parStaffName"].Value = Globals.AllStaffs.Where(x => x.StaffID == (int)Globals.LoggedUserAccount.StaffID).FirstOrDefault().FullName.ToString();
                    rParams["Show"].Value = RptParameters.Show;
                    break;
                //▲====: #006
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
                case ReportName.PHARMACY_BCHuyHoan:
                    ReportModel = null;
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.RptPharmacies.XRptBCHuyHoanNhaThuoc").PreviewModel;
                    rParams["FromDate"].Value = RptParameters.FromDate;
                    rParams["ToDate"].Value = RptParameters.ToDate;
                    rParams["StoreID"].Value = (int)RptParameters.StoreID.GetValueOrDefault();
                    rParams["parHospitalName"].Value = Globals.ServerConfigSection.CommonItems.ReportHospitalName;
                    break;
                //▼====: #007
                case ReportName.BCHuyDQGReport:
                    ReportModel = null;
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.RptPharmacies.XtraReports.ReportGeneral.XRpt_BCHuyDQGReport").PreviewModel;
                    rParams["Quarter"].Value = RptParameters.Quarter;
                    rParams["Month"].Value = RptParameters.Month;
                    rParams["Year"].Value = RptParameters.Year;
                    rParams["Flag"].Value = RptParameters.Flag;
                    rParams["FromDate"].Value = RptParameters.FromDate;
                    rParams["ToDate"].Value = RptParameters.ToDate;
                    rParams["Show"].Value = RptParameters.Show;
                    rParams["parStaffName"].Value = Globals.AllStaffs.Where(x => x.StaffID == (int)Globals.LoggedUserAccount.StaffID).FirstOrDefault().FullName.ToString();
                    rParams["parLogoUrl"].Value = Globals.ServerConfigSection.CommonItems.ReportLogoUrl;
                    break;
                //▲====: #007
            }

            //  ReportModel.RequestDefaultParameterValues += new WeakEventHandler<EventArgs>(_reportModel_RequestDefaultParameterValues).Handler;
            // ReportModel.AutoShowParametersPanel = false;
            ReportModel.CreateDocument(rParams);
        }

        public void _reportModel_RequestDefaultParameterValues(object sender, System.EventArgs e)
        {
            //switch (_eItem)
            //{
            //    case ReportName.PHARMACY_NHAPTHUOCHANGTHANG:
            //        rParams["Quarter"].Value = RptParameters.Quarter;
            //        rParams["Month"].Value = RptParameters.Month;
            //        rParams["Year"].Value = RptParameters.Year;
            //        rParams["Flag"].Value = RptParameters.Flag;
            //        rParams["FromDate"].Value = RptParameters.FromDate;
            //        rParams["ToDate"].Value = RptParameters.ToDate;
            //        rParams["Show"].Value = RptParameters.Show;
            //        rParams["StoreName"].Value = RptParameters.StoreName;
            //        rParams["StoreID"].Value = (int)RptParameters.StoreID.GetValueOrDefault();
            //        break;
            //    case ReportName.PHARMACY_NHAPHANGTHANGTHEOSOPHIEU:
            //        goto case ReportName.PHARMACY_NHAPTHUOCHANGTHANG;
            //    case ReportName.PHARMACY_XUATNOIBOTHEONGUOIBAN:
            //        rParams["Quarter"].Value = RptParameters.Quarter;
            //        rParams["Month"].Value = RptParameters.Month;
            //        rParams["Year"].Value = RptParameters.Year;
            //        rParams["Flag"].Value = RptParameters.Flag;
            //        rParams["FromDate"].Value = RptParameters.FromDate;
            //        rParams["ToDate"].Value = RptParameters.ToDate;
            //        rParams["Show"].Value = RptParameters.Show;
            //        rParams["StoreName"].Value = RptParameters.StoreName;
            //        rParams["StoreID"].Value = (int)RptParameters.StoreID.GetValueOrDefault();
            //        rParams["OutTo"].Value = (int)RptParameters.OutTo.GetValueOrDefault(0);
            //        rParams["TypID"].Value = (int)RptParameters.TypID.GetValueOrDefault();
            //        break;
            //    case ReportName.PHARMACY_XUATNOIBOTHEOTENTHUOC:
            //        goto case ReportName.PHARMACY_XUATNOIBOTHEONGUOIBAN;
            //    case ReportName.PHARMACY_XUATTHUOCCHOBH:
            //        goto case ReportName.PHARMACY_NHAPTHUOCHANGTHANG;
            //    case ReportName.PHARMACY_BANTHUOCTH:
            //        rParams["Quarter"].Value = RptParameters.Quarter;
            //        rParams["Month"].Value = RptParameters.Month;
            //        rParams["Year"].Value = RptParameters.Year;
            //        rParams["Flag"].Value = RptParameters.Flag;
            //        rParams["FromDate"].Value = RptParameters.FromDate;
            //        rParams["ToDate"].Value = RptParameters.ToDate;
            //        rParams["Show"].Value = RptParameters.Show;
            //        break;
            //    case ReportName.PHARMACY_TRATHUOCTH:
            //        goto case ReportName.PHARMACY_BANTHUOCTH;
            //    case ReportName.PHARMACY_BAOCAOTONGHOPDOANHTHU:
            //        rParams["Month"].Value = RptParameters.Month;
            //        rParams["Year"].Value = RptParameters.Year;
            //        break;
            //    case ReportName.PHARMACY_TONGHOPDUTRU_SOPHIEU:
            //        goto case ReportName.PHARMACY_BANTHUOCTH;
            //    case ReportName.PHARMACY_TONGHOPDUTRU_TENTHUOC:
            //        goto case ReportName.PHARMACY_BANTHUOCTH;
            //    case ReportName.PHARMACY_THEODOICONGNO:
            //        rParams["Quarter"].Value = RptParameters.Quarter;
            //        rParams["Month"].Value = RptParameters.Month;
            //        rParams["Year"].Value = RptParameters.Year;
            //        rParams["Flag"].Value = RptParameters.Flag;
            //        rParams["FromDate"].Value = RptParameters.FromDate;
            //        rParams["ToDate"].Value = RptParameters.ToDate;
            //        rParams["Show"].Value = RptParameters.Show;
            //        rParams["ShowTitle"].Value = RptParameters.ShowTitle;
            //        rParams["Type"].Value = (int)RptParameters.TypCongNo.GetValueOrDefault(0);
            //        rParams["StoreID"].Value = (int)RptParameters.StoreID.GetValueOrDefault(0);
            //        rParams["StoreName"].Value = RptParameters.StoreName;
            //        break;
            //    case ReportName.TEMP20_NGOAITRU:
            //        rParams["Quarter"].Value = RptParameters.Quarter;
            //        rParams["Month"].Value = RptParameters.Month;
            //        rParams["Year"].Value = RptParameters.Year;
            //        rParams["Flag"].Value = RptParameters.Flag;
            //        rParams["FromDate"].Value = RptParameters.FromDate;
            //        rParams["ToDate"].Value = RptParameters.ToDate;
            //        rParams["Show"].Value = RptParameters.Show;
            //        break;

            //}
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

        public void btnExportExcel()
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

            if (RptParameters == null)
            {
                RptParameters = new ReportParameters();
            }

            RptParameters.ReportType = ReportType.BAOCAO_NHATHUOC;

            RptParameters.reportName = _eItem;

            if (GetParameters())
            {
                switch (_eItem)
                {
                    case ReportName.PHARMACY_SOKIEMNHAPTHUOC:
                        RptParameters.Show = "SoKiemNhapThuoc";
                        break;
                }

                ExportToExcelGeneric.Action(RptParameters, objSFD, this);
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


            if (RptParameters.TypID == (long)AllLookupValues.RefOutputType.XUATNOIBO)
            {
                RptParameters.ShowFirst = string.Format("{0} ", eHCMSResources.K1003_G1_Ban.ToUpper()) + Str;
            }
            else if (RptParameters.TypID == (long)AllLookupValues.RefOutputType.XUATNOIBO_CHOMUON)
            {
                RptParameters.ShowFirst = Str + string.Format(" {0} ", eHCMSResources.Z0940_G1_Muon);
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
                        Globals.ShowMessage(eHCMSResources.K0310_G1_ChonKhoCanXem, eHCMSResources.G0442_G1_TBao);
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
                    RptParameters.Show = RptParameters.ShowFirst + string.Format(" {0} ", eHCMSResources.G1933_G1_TuNg.ToUpper()) + RptParameters.FromDate.GetValueOrDefault().ToString("dd/MM/yyyy") + string.Format(" - {0}", eHCMSResources.K3192_G1_DenNg.ToUpper()) + RptParameters.ToDate.GetValueOrDefault().ToString("dd/MM/yyyy");
                }
            }

            if (CurrentInwardSource != null)
            {
                RptParameters.InwardSource = CurrentInwardSource.Value;
            }

            return result;
        }

        #region Radio Checked Member

        #region Xuat noi bo
        private string Str = "";
        public void chkKho_Checked(object sender, RoutedEventArgs e)
        {
            RptParameters.OutTo = 2;
            Str = string.Format(" {0} ", eHCMSResources.Z0943_G1_ChoKhoKhac);
            RptParameters.ShowFirst = string.Format("{0} ", eHCMSResources.Z0943_G1_ChoKhoKhac);
        }
        public void chkBacSi_Checked(object sender, RoutedEventArgs e)
        {
            RptParameters.OutTo = 1;
            Str = string.Format(" {0} ", eHCMSResources.Z0944_G1_ChoBSi);
            RptParameters.ShowFirst = string.Format("{0} ", eHCMSResources.Z0944_G1_ChoBSi);
        }
        public void chkBVBan_Checked(object sender, RoutedEventArgs e)
        {
            RptParameters.OutTo = 3;
            Str = string.Format(" {0} ", eHCMSResources.Z0945_G1_ChoBVBan);
            RptParameters.ShowFirst = eHCMSResources.Z0945_G1_ChoBVBan;
        }
        public void chkXuatDenAll_Checked(object sender, RoutedEventArgs e)
        {
            RptParameters.OutTo = 0;
        }

        //public void chkSell_Checked(object sender, RoutedEventArgs e)
        //{
        //    RptParameters.TypID = (long)AllLookupValues.RefOutputType.XUATNOIBO;
        //}

        //public void chkLoan_Checked(object sender, RoutedEventArgs e)
        //{
        //    RptParameters.TypID = (long)AllLookupValues.RefOutputType.XUATNOIBO_CHOMUON;
        //}

        //public void chkLoaiXNBAll_Checked(object sender, RoutedEventArgs e)
        //{
        //    RptParameters.TypID = 0;
        //}
        #endregion

        #region Theo doi cong no
        public void chkNotPay_Checked(object sender, RoutedEventArgs e)
        {
            RptParameters.TypCongNo = 0;
            RptParameters.ShowTitle = eHCMSResources.G0345_G1_TheoDoiChuaTraTienoanHDon;
        }
        public void chkPay_Checked(object sender, RoutedEventArgs e)
        {
            RptParameters.TypCongNo = 1;
            RptParameters.ShowTitle = eHCMSResources.G0352_G1_TheoDoiDaTToanHDon;
        }

        #endregion
        #endregion

        public void BtnXemChiTiet(object sender, EventArgs e)
        {
            if (GetParameters())
            {
                GetReport(true);
            }
        }

        //▼====: #004
        private bool _ShowTongKho = false;
        public bool ShowTongKho
        {
            get { return _ShowTongKho; }
            set
            {
                if (_ShowTongKho == value)
                    return;
                _ShowTongKho = value;
            }
        }

        public void ViewByDate()
        {
            ByDate();
            CurrentCondition = Conditions[2];
        }
        //▲====: #004
    }
}
