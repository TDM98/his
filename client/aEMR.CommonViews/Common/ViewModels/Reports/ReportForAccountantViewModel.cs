using eHCMSLanguage;
using System.ComponentModel.Composition;
using Caliburn.Micro;
using System.Windows;
using System.Windows.Controls;
using System;
using System.Collections.ObjectModel;
using System.Threading;
using DataEntities;
using System.Linq;
using System.Collections.Generic;
using aEMR.ViewContracts;
using aEMR.Infrastructure;
using aEMR.Common.Collections;
using aEMR.CommonTasks;
using aEMR.ReportModel.ReportModels;
using aEMR.ServiceClient;
using Microsoft.Win32;
using Castle.Windsor;
using aEMR.Infrastructure.CachingUtils;
using DevExpress.ReportServer.Printing;
using DevExpress.XtraReports.UI;
using DevExpress.Xpf.Printing;
using System.IO;
/*
* 20190603 #001 TNHX: [BM0011782] Init new view for Report NXTheoMucDich
* 20190827 #002 TNHX: [BM0013276] Create report InOut of DrugDept + ClinicDept for Accountant
* 20191003 #003 TNHX: [BM 0013292] Update report BCChiTietNhapTuNCC
* 20191029 #004 TNHX: [BM 0006884] : Add report TT22_BienBanKiemKe
* 20200314 #005 TNHX: [BM] : Add report Pharmacy_BKXuatThuocTheoBN
* 20200407 #006 TNHX: [BM] : Add condition MedProduct all for accountant
* 20200915 #007 TNHX: [BM] : Add export excel for report InOut of DrugDept + ClinicDept
* 20210908 #008 QTD:  Add condition Store all for accountant
* 20210911 #009 QTD:  Add new Report BC_KHO_TH
* 20211103 #010 QTD:  Lọc kho theo cấu hình trách nhiệm
* 20220211 #011 QTD:  Thêm BC NXT tổng hợp kho cơ số
* 20220517 #012 DatTB: Báo cáo máu sắp hết hạn dùng
* 20220613 #013 QTD:  Hiển thị nút Search cho Báo cáo tình hình nhập NCC chi tiết
* 20230210 #014 QTD:  Thêm BC quản lý SD thuốc cản quang
*/
namespace aEMR.Common.ViewModels
{
    [Export(typeof(ICommonReportForAccountant)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class ReportForAccountantViewModel : Conductor<object>, ICommonReportForAccountant
    {
        [ImportingConstructor]
        public ReportForAccountantViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg, ISalePosCaching salePosCaching)
        {
            RptParameters = new ReportParameters();
            FillCondition();
            FillMonth();
            FillQuarter();
            FillYear();

            SelectedStaff = Globals.LoggedUserAccount.Staff;

            LoadRefPurposeForAccountant_All();
            LoadDrugDeptProductGroupReportTypes();
            //▼====: #006
            LoadMedProductTypeCollection();
            //▲====: #006
        }

        #region Properties Member

        public void LoadListStaff(byte type)
        {
            Coroutine.BeginExecute(LoadStaffHaveRegistrationList(type));
        }

        private IEnumerator<IResult> LoadStaffHaveRegistrationList(byte type)
        {
            var paymentTypeTask = new LoadStaffHaveRegistrationListTask(false, true, type);
            yield return paymentTypeTask;
            StaffList = paymentTypeTask.StaffList;
            yield break;
        }

        private ObservableCollection<Staff> _StaffList;
        public ObservableCollection<Staff> StaffList
        {
            get
            {
                return _StaffList;
            }
            set
            {
                if (_StaffList == value)
                    return;
                _StaffList = value;
                NotifyOfPropertyChange(() => StaffList);
            }
        }

        private Staff _SelectedStaff;
        public Staff SelectedStaff
        {
            get
            {
                return _SelectedStaff;
            }
            set
            {
                if (_SelectedStaff == value)
                    return;
                _SelectedStaff = value;
                NotifyOfPropertyChange(() => SelectedStaff);
            }
        }

        private string _strHienThi = Globals.PageName;
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

        public class Condition
        {
            public string Text { get; }
            public long Value { get; }
            public Condition(string theText, long theValue)
            {
                Text = theText;
                Value = theValue;
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
                //▼====: #012
                if (_eItem == ReportName.BCMauSapHetHanDung)
                {
                    AllMedProductTypeCollection = Globals.AllLookupValueList.Where(x => x.LookupID == (long)AllLookupValues.MedProductType.MAU).ToObservableCollection();

                    if (AllMedProductTypeCollection != null)
                    {
                        CurProductType = AllMedProductTypeCollection.FirstOrDefault();
                    }

                    IsExportVisible = false;
                    IsMedProductTypeVisible = false;
                    strHienThi = eHCMSResources.Z3242_G1_BCMauSapHetHanDung;
                }
                //▲====: #012

                if (_eItem == ReportName.BC_KHO_TONGHOP)
                {
                    IsExportVisible = false;
                    IsMedProductTypeVisible = false;
                    strHienThi = "BÁO CÁO KHO TỔNG HỢP";
                }
                if (_eItem == ReportName.BCChiTietNhapTuNCC)
                {
                    AllMedProductTypeCollection = Globals.AllLookupValueList.Where(x => x.ObjectTypeID == (long)LookupValues.V_MedProductType).ToObservableCollection();
                    if (AllMedProductTypeCollection != null)
                    {
                        CurProductType = AllMedProductTypeCollection.Where(x => x.LookupID != 0).FirstOrDefault();
                    }
                    IsExportVisible = false;
                }

                //▼====: #014
                if (_eItem == ReportName.BCXuatSDThuocCanQuang)
                {
                    IsMedProductTypeVisible = false;
                    strHienThi = eHCMSResources.Z3298_G1_BCXuatSDThuocCanQuang;
                    CurProductType = Globals.AllLookupValueList.Where(x => x.LookupID == (long)AllLookupValues.MedProductType.THUOC).FirstOrDefault();
                }
                //▲====: #014
                NotifyOfPropertyChange(() => eItem);
                //IsExportVisible = false;
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

            Conditions.Add(new Condition(eHCMSResources.Z0938_G1_TheoQuy, 0));
            Conditions.Add(new Condition(eHCMSResources.Z0939_G1_TheoTh, 1));
            Conditions.Add(new Condition(eHCMSResources.G0375_G1_TheoNg, 2));

            CurrentCondition = Conditions.LastOrDefault();
            ByDate();
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

        private bool _mXemChiTiet = false;
        public bool mXemChiTiet
        {
            get
            {
                return _mXemChiTiet;
            }
            set
            {
                if (_mXemChiTiet == value)
                    return;
                _mXemChiTiet = value;
                NotifyOfPropertyChange(() => mXemChiTiet);
            }
        }

        private bool _mXemChiTietTheoThuoc = false;
        public bool mXemChiTietTheoThuoc
        {
            get
            {
                return _mXemChiTietTheoThuoc;
            }
            set
            {
                if (_mXemChiTietTheoThuoc == value)
                    return;
                _mXemChiTietTheoThuoc = value;
                NotifyOfPropertyChange(() => mXemChiTietTheoThuoc);
            }
        }

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

        private bool _ViewBy = true;
        public bool ViewBy
        {
            get { return _ViewBy; }
            set
            {
                _ViewBy = value;
                NotifyOfPropertyChange(() => ViewBy);
            }
        }

        //▼====: #002
        private bool _IsPurpose = true;
        public bool IsPurpose
        {
            get { return _IsPurpose; }
            set
            {
                _IsPurpose = value;
                NotifyOfPropertyChange(() => IsPurpose);
            }
        }

        private bool _IsShowGroupReportType = false;
        public bool IsShowGroupReportType
        {
            get { return _IsShowGroupReportType; }
            set
            {
                _IsShowGroupReportType = value;
                NotifyOfPropertyChange(() => IsShowGroupReportType);
            }
        }

        private bool _CanSelectedRefGenDrugCatID_1 = false;
        public bool CanSelectedRefGenDrugCatID_1
        {
            get { return _CanSelectedRefGenDrugCatID_1; }
            set
            {
                _CanSelectedRefGenDrugCatID_1 = value;
                NotifyOfPropertyChange(() => CanSelectedRefGenDrugCatID_1);
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

        private Lookup _CurProductType;
        public Lookup CurProductType
        {
            get => _CurProductType; set
            {
                _CurProductType = value;
                LoadRefGenericDrugCategory_1();
                NotifyOfPropertyChange(() => CurProductType);
            }
        }

        private ObservableCollection<Lookup> _AllMedProductTypeCollection;
        public ObservableCollection<Lookup> AllMedProductTypeCollection
        {
            get
            {
                return _AllMedProductTypeCollection;
            }
            set
            {
                //▼====: #006
                _AllMedProductTypeCollection = value;
                Lookup firstItem = new Lookup
                {
                    LookupID = 0,
                    ObjectValue = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.T0822_G1_TatCa)
                };
                //▼====: #012
                if (_eItem != ReportName.BCMauSapHetHanDung)
                    _AllMedProductTypeCollection.Insert(0, firstItem);
                //▲====: #012
                //▲====: #006
                NotifyOfPropertyChange(() => AllMedProductTypeCollection);
            }
        }

        private long _SelectedDrugDeptProductGroupReportType;
        public long SelectedDrugDeptProductGroupReportType
        {
            get
            {
                return _SelectedDrugDeptProductGroupReportType;
            }
            set
            {
                if (_SelectedDrugDeptProductGroupReportType != value)
                {
                    _SelectedDrugDeptProductGroupReportType = value;
                    NotifyOfPropertyChange(() => SelectedDrugDeptProductGroupReportType);
                }
            }
        }

        private ObservableCollection<DrugDeptProductGroupReportType> _DrugDeptProductGroupReportTypeCollection;
        public ObservableCollection<DrugDeptProductGroupReportType> DrugDeptProductGroupReportTypeCollection
        {
            get
            {
                return _DrugDeptProductGroupReportTypeCollection;
            }
            set
            {
                if (_DrugDeptProductGroupReportTypeCollection != value)
                {
                    _DrugDeptProductGroupReportTypeCollection = value;
                    NotifyOfPropertyChange(() => DrugDeptProductGroupReportTypeCollection);
                }
            }
        }

        //▼====: #005
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
        //▲====: #005

        public void LoadDrugDeptProductGroupReportTypes()
        {
            this.ShowBusyIndicator();
            IsShowGroupReportType = true;
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new CommonService_V2Client())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetDrugDeptProductGroupReportTypes(Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                DrugDeptProductGroupReportTypeCollection = new ObservableCollection<DrugDeptProductGroupReportType>(contract.EndGetDrugDeptProductGroupReportTypes(asyncResult));
                                if (DrugDeptProductGroupReportTypeCollection == null)
                                {
                                    DrugDeptProductGroupReportTypeCollection = new ObservableCollection<DrugDeptProductGroupReportType>();
                                }
                                DrugDeptProductGroupReportTypeCollection.Insert(0, new DrugDeptProductGroupReportType { DrugDeptProductGroupReportTypeID = 0, DrugDeptProductGroupReportTypeCode = eHCMSResources.K2122_G1_ChonTatCa });
                                NotifyOfPropertyChange(() => DrugDeptProductGroupReportTypeCollection);
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
                    MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                    this.HideBusyIndicator();
                }
            });
            t.Start();
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

        public void LoadRefGenericDrugCategory_1()
        {
            Coroutine.BeginExecute(DoGetRefGenericDrugCategory_1List());
        }

        private IEnumerator<IResult> DoGetRefGenericDrugCategory_1List()
        {
            this.ShowBusyIndicator();
            var paymentTypeTask = new LoadRefGenericDrugCategory_1ListTask(CurProductType.LookupID, false, true);
            yield return paymentTypeTask;
            RefGenericDrugCategory_1s = paymentTypeTask.RefGenericDrugCategory_1List;
            SetDefultRefGenericDrugCategory();
            this.HideBusyIndicator();
            yield break;
        }

        private void SetDefultRefGenericDrugCategory()
        {
            if (RefGenericDrugCategory_1s != null)
            {
                RefGenDrugCatID_1 = RefGenericDrugCategory_1s.FirstOrDefault().RefGenDrugCatID_1;
            }
        }
        //▲====: #002

        //▼====: #006
        public void LoadMedProductTypeCollection()
        {
            AllMedProductTypeCollection = Globals.AllLookupValueList.Where(x => x.ObjectTypeID == (long)LookupValues.V_MedProductType).ToObservableCollection();

            if (AllMedProductTypeCollection != null)
            {
                CurProductType = AllMedProductTypeCollection.FirstOrDefault();
            }
        }
        //▲====: #006

        private void GetReport(ReportName _eItem, bool isDetail = false, bool isDetailV2 = false)
        {
            DevExpress.DocumentServices.ServiceModel.DefaultValueParameterContainer rParams = new DevExpress.DocumentServices.ServiceModel.DefaultValueParameterContainer();
            string reportHospitalName = Globals.ServerConfigSection.CommonItems.ReportHospitalName;
            string reportDepartmentOfHealth = Globals.ServerConfigSection.CommonItems.ReportDepartmentOfHealth;
            string reportLogoUrl = Globals.ServerConfigSection.CommonItems.ReportLogoUrl;
            string reportHospitalAddress = Globals.ServerConfigSection.CommonItems.ReportHospitalAddress;
            switch (_eItem)
            {
                //▼====: #004
                case ReportName.TT22_BienBanKiemKe:
                    ReportModel = null;
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.RptDrugDepts.XtraReports.Circulars22.RptInventoryRecords").PreviewModel;
                    string tempTitle = eHCMSResources.Z2898_G1_BienBanKiemKe;
                    if (CurProductType != null)
                    {
                        switch (CurProductType.LookupID)
                        {
                            case (long)AllLookupValues.MedProductType.THUOC:
                                tempTitle = tempTitle + " " + eHCMSResources.G0787_G1_Thuoc;
                                break;
                            case (long)AllLookupValues.MedProductType.HOA_CHAT:
                                tempTitle = tempTitle + " " + eHCMSResources.T1616_G1_HC;
                                break;
                            case (long)AllLookupValues.MedProductType.VTYT_TIEUHAO:
                                tempTitle = tempTitle + " " + eHCMSResources.G2323_G1_VTYTTieuHao;
                                break;
                            default:
                                tempTitle = tempTitle + " " + eHCMSResources.G0787_G1_Thuoc;
                                break;
                        }
                    }
                    rParams["FromDate"].Value = RptParameters.FromDate;
                    rParams["ToDate"].Value = RptParameters.ToDate;
                    rParams["StoreID"].Value = CurStore != null ? CurStore.StoreID : 1;
                    rParams["Title"].Value = tempTitle;
                    //rParams["DateShow"].Value = "Thời gian từ " + RptParameters.FromDate.GetValueOrDefault().ToShortDateString() + " đến " + RptParameters.ToDate.GetValueOrDefault().ToShortDateString();
                    //rParams["parHospitalName"].Value = Globals.ServerConfigSection.CommonItems.ReportHospitalName;
                    rParams["V_MedProductType"].Value = CurProductType != null ? CurProductType.LookupID : (long)AllLookupValues.MedProductType.THUOC;
                    break;
                //▲====: #004
                case ReportName.TK_NX_THEOMUCDICH:
                    ReportModel = null;
                    if (_StoreType != null)
                    {
                        switch (_StoreType)
                        {
                            case (long)AllLookupValues.StoreType.STORAGE_CLINIC:
                                if (isDetail)
                                {
                                    ReportModel = new GenericReportModel("eHCMS.ReportLib.RptTransactions.XRpt_ClinicDeptInOutStatisticsDetail").PreviewModel;
                                }
                                else if (isDetailV2)
                                {
                                    ReportModel = new GenericReportModel("eHCMS.ReportLib.RptTransactions.XRpt_ClinicDeptInOutStatisticsDetail_V2").PreviewModel;
                                }
                                else ReportModel = new GenericReportModel("eHCMS.ReportLib.RptTransactions.XRpt_ClinicDeptInOutStatistics").PreviewModel;
                                rParams["parV_MedProductType"].Value = CurProductType != null ? CurProductType.LookupID : (long)AllLookupValues.MedProductType.THUOC;
                                break;
                            case (long)AllLookupValues.StoreType.STORAGE_DRUGDEPT:
                                if (isDetail)
                                {
                                    ReportModel = new GenericReportModel("eHCMS.ReportLib.RptTransactions.XRpt_MedDeptInOutStatisticsDetail").PreviewModel;
                                }
                                else if (isDetailV2)
                                {
                                    ReportModel = new GenericReportModel("eHCMS.ReportLib.RptTransactions.XRpt_MedDeptInOutStatisticsDetail_V2").PreviewModel;
                                }
                                else ReportModel = new GenericReportModel("eHCMS.ReportLib.RptTransactions.XRpt_MedDeptInOutStatistics").PreviewModel;
                                rParams["parV_MedProductType"].Value = CurProductType != null ? CurProductType.LookupID : (long)AllLookupValues.MedProductType.THUOC;
                                break;
                            case (long)AllLookupValues.StoreType.STORAGE_EXTERNAL:
                                if (isDetail)
                                {
                                    ReportModel = new GenericReportModel("eHCMS.ReportLib.RptTransactions.XRpt_DrugInOutStatisticsDetail").PreviewModel;
                                }
                                else if (isDetailV2)
                                {
                                    ReportModel = new GenericReportModel("eHCMS.ReportLib.RptTransactions.XRpt_DrugInOutStatisticsDetail_V2").PreviewModel;
                                }
                                else ReportModel = new GenericReportModel("eHCMS.ReportLib.RptTransactions.XRpt_DrugInOutStatistics").PreviewModel;
                                break;
                            default:
                                if (isDetail)
                                {
                                    ReportModel = new GenericReportModel("eHCMS.ReportLib.RptTransactions.XRpt_MedDeptInOutStatisticsDetail").PreviewModel;
                                }
                                else if (isDetailV2)
                                {
                                    ReportModel = new GenericReportModel("eHCMS.ReportLib.RptTransactions.XRpt_MedDeptInOutStatisticsDetail_V2").PreviewModel;
                                }
                                else ReportModel = new GenericReportModel("eHCMS.ReportLib.RptTransactions.XRpt_MedDeptInOutStatistics").PreviewModel;
                                rParams["parV_MedProductType"].Value = CurProductType != null ? CurProductType.LookupID : (long)AllLookupValues.MedProductType.THUOC;
                                break;
                        }
                        rParams["parFromDate"].Value = RptParameters.FromDate;
                        rParams["parToDate"].Value = RptParameters.ToDate;
                        rParams["StoreID"].Value = CurStore.StoreID;
                        rParams["StoreName"].Value = CurStore.swhlName;
                        rParams["parHospitalName"].Value = reportHospitalName;
                        rParams["parDepartmentOfHealth"].Value = reportDepartmentOfHealth;
                        rParams["parStoreOut"].Value = CurStoreOut.StoreID;
                        rParams["parStoreIn"].Value = CurStoreIn.StoreID;
                        rParams["parPurposeIn"].Value = SelectedPurposeIn.PurposeID;
                        rParams["parPurposeOut"].Value = SelectedPurposeOut.PurposeID;
                    }
                    break;
                //▼====: #003
                //case ReportName.BCChiTietNhapTuNCC:
                //    ReportModel = null;
                //    ReportModel = new GenericReportModel("eHCMS.ReportLib.RptTransactions.XtraReports.Accountant.XRpt_BCNhapTuNCC").PreviewModel;
                //    rParams["FromDate"].Value = RptParameters.FromDate;
                //    rParams["ToDate"].Value = RptParameters.ToDate;
                //    rParams["StorageName"].Value = CurStore != null ? CurStore.swhlName : "";
                //    rParams["StoreID"].Value = CurStore != null ? CurStore.StoreID : 1;
                //    rParams["DateShow"].Value = "Thời gian từ " + RptParameters.FromDate.GetValueOrDefault().ToShortDateString() + " đến " + RptParameters.ToDate.GetValueOrDefault().ToShortDateString();
                //    rParams["parHospitalName"].Value = Globals.ServerConfigSection.CommonItems.ReportHospitalName;
                //    rParams["V_MedProductType"].Value = CurProductType != null ? CurProductType.LookupID : (long)AllLookupValues.MedProductType.THUOC;
                //    break;
                //▲====: #003                
                //▼====: #003
                case ReportName.BC_ThuocSapHetHanDung:
                    ReportModel = null;
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.RptTransactions.XtraReports.XRpt_ThuocSapHetHanSD").PreviewModel;
                    rParams["FromDate"].Value = RptParameters.FromDate;
                    rParams["ToDate"].Value = RptParameters.ToDate;
                    rParams["StorageName"].Value = CurStore != null ? CurStore.swhlName : "";
                    rParams["StoreID"].Value = CurStore != null ? CurStore.StoreID : 1;
                    rParams["DateShow"].Value = "Thời gian từ " + RptParameters.FromDate.GetValueOrDefault().ToShortDateString() + " đến " + RptParameters.ToDate.GetValueOrDefault().ToShortDateString();
                    rParams["parHospitalName"].Value = Globals.ServerConfigSection.CommonItems.ReportHospitalName;
                    rParams["parDepartmentOfHealth"].Value = Globals.ServerConfigSection.CommonItems.ReportDepartmentOfHealth;
                    rParams["V_MedProductType"].Value = CurProductType != null ? CurProductType.LookupID : (long)AllLookupValues.MedProductType.THUOC;
                    break;
                //▲====: #003
                //▼====: #012
                case ReportName.BCMauSapHetHanDung:
                    ReportModel = null;
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.RptTransactions.XtraReports.XRpt_MauSapHetHanDung").PreviewModel;
                    rParams["FromDate"].Value = RptParameters.FromDate;
                    rParams["ToDate"].Value = RptParameters.ToDate;
                    rParams["StorageName"].Value = CurStore != null ? CurStore.swhlName : "";
                    rParams["StoreID"].Value = CurStore != null ? CurStore.StoreID : 1;
                    rParams["DateShow"].Value = "Thời gian từ " + RptParameters.FromDate.GetValueOrDefault().ToShortDateString() + " đến " + RptParameters.ToDate.GetValueOrDefault().ToShortDateString();
                    rParams["parHospitalName"].Value = Globals.ServerConfigSection.CommonItems.ReportHospitalName;
                    rParams["parDepartmentOfHealth"].Value = Globals.ServerConfigSection.CommonItems.ReportDepartmentOfHealth;
                    rParams["V_MedProductType"].Value = CurProductType != null ? CurProductType.LookupID : (long)AllLookupValues.MedProductType.MAU;
                    break;
                //▲====: #012
                //▼====: #004
                case ReportName.KT_BCHangKhongXuat:
                    ReportModel = null;
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.RptDrugDepts.XtraReports.Reports.XRptInOutStockValueDrugDeptNonOutward_TV").PreviewModel;
                    rParams["FromDate"].Value = RptParameters.FromDate;
                    rParams["ToDate"].Value = RptParameters.ToDate;
                    rParams["StorageName"].Value = CurStore != null ? CurStore.swhlName : "";
                    rParams["StoreID"].Value = CurStore != null ? CurStore.StoreID : 1;
                    rParams["DateShow"].Value = "Thời gian từ " + RptParameters.FromDate.GetValueOrDefault().ToShortDateString() + " đến " + RptParameters.ToDate.GetValueOrDefault().ToShortDateString();
                    rParams["parHospitalName"].Value = Globals.ServerConfigSection.CommonItems.ReportHospitalName;
                    //rParams["parDepartmentOfHealth"].Value = Globals.ServerConfigSection.CommonItems.ReportDepartmentOfHealth;
                    rParams["V_MedProductType"].Value = CurProductType != null ? CurProductType.LookupID : (long)AllLookupValues.MedProductType.THUOC;
                    break;
                //▲====: #004
                //▼====: #005
                case ReportName.Pharmacy_BKXuatThuocTheoBN:
                    ReportModel = null;
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.RptPharmacies.XRpt_BKXuatThuocTheoBN").PreviewModel;
                    rParams["FromDate"].Value = RptParameters.FromDate;
                    rParams["ToDate"].Value = RptParameters.ToDate;
                    rParams["StorageName"].Value = CurStore != null ? CurStore.swhlName : "";
                    rParams["StoreID"].Value = CurStore != null ? CurStore.StoreID : 1;
                    rParams["DateShow"].Value = "Thời gian từ " + RptParameters.FromDate.GetValueOrDefault().ToShortDateString() + " đến " + RptParameters.ToDate.GetValueOrDefault().ToShortDateString();
                    rParams["parHospitalName"].Value = Globals.ServerConfigSection.CommonItems.ReportHospitalName;
                    rParams["parStaffName"].Value = Globals.AllStaffs.Where(x => x.StaffID == (int)Globals.LoggedUserAccount.StaffID).FirstOrDefault().FullName.ToString();
                    //rParams["parDepartmentOfHealth"].Value = Globals.ServerConfigSection.CommonItems.ReportDepartmentOfHealth;
                    //rParams["V_MedProductType"].Value = CurProductType != null ? CurProductType.LookupID : (long)AllLookupValues.MedProductType.THUOC;
                    break;
                //▲====: #005
                //▼====: #002
                case ReportName.BC_NXT_THUOC_TONGHOP:
                    if (_StoreType != null)
                    {
                        switch (_StoreType)
                        {
                            case (long)AllLookupValues.StoreType.STORAGE_CLINIC:
                                ReportModel = new ClinicDeptInOutStocksReportModal_KT().PreviewModel;
                                rParams["V_MedProductType"].Value = CurProductType.LookupID;
                                rParams["RefGenDrugCatID_1"].Value = RefGenDrugCatID_1;
                                rParams["DrugDeptProductGroupReportTypeID"].Value = SelectedDrugDeptProductGroupReportType;
                                break;
                            case (long)AllLookupValues.StoreType.STORAGE_DRUGDEPT:
                                ReportModel = new DrugDeptInOutStockValuesReportModal_KT().PreviewModel;
                                rParams["V_MedProductType"].Value = CurProductType.LookupID;
                                rParams["RefGenDrugCatID_1"].Value = RefGenDrugCatID_1;
                                rParams["DrugDeptProductGroupReportTypeID"].Value = SelectedDrugDeptProductGroupReportType;
                                break;
                            case (long)AllLookupValues.StoreType.STORAGE_EXTERNAL:
                                ReportModel = new PharmacyInOutStocksReportModal_KT().PreviewModel;
                                break;
                        }

                        rParams["DateShow"].Value = "Từ ngày " + RptParameters.FromDate.GetValueOrDefault().ToShortDateString() + " đến ngày " + RptParameters.ToDate.GetValueOrDefault().ToShortDateString();
                        rParams["FromDate"].Value = RptParameters.FromDate;
                        rParams["ToDate"].Value = RptParameters.ToDate;
                        rParams["StorageName"].Value = CurStore.swhlName;
                        rParams["StoreID"].Value = CurStore.StoreID;
                        rParams["parLogoUrl"].Value = Globals.ServerConfigSection.CommonItems.ReportLogoUrl;
                        rParams["parHospitalName"].Value = Globals.ServerConfigSection.CommonItems.ReportHospitalName;
                    }
                    break;
                //▲====: #002

                //▼====: #011
                //case ReportName.BC_NXT_THUOC_TONGHOP_COSO:
                //    ReportModel = new ClinicDeptInOutStocksReportModal_KT_V2().PreviewModel;
                //    rParams["V_MedProductType"].Value = CurProductType.LookupID;
                //    rParams["RefGenDrugCatID_1"].Value = RefGenDrugCatID_1;
                //    rParams["DrugDeptProductGroupReportTypeID"].Value = SelectedDrugDeptProductGroupReportType;
                //    rParams["DateShow"].Value = "Từ ngày " + RptParameters.FromDate.GetValueOrDefault().ToShortDateString() + " đến ngày " + RptParameters.ToDate.GetValueOrDefault().ToShortDateString();
                //    rParams["FromDate"].Value = RptParameters.FromDate;
                //    rParams["ToDate"].Value = RptParameters.ToDate;
                //    rParams["StorageName"].Value = CurStore.swhlName;
                //    rParams["StoreID"].Value = 0;
                //    rParams["parLogoUrl"].Value = Globals.ServerConfigSection.CommonItems.ReportLogoUrl;
                //    rParams["parHospitalName"].Value = Globals.ServerConfigSection.CommonItems.ReportHospitalName;
                    break;
                //▲====: #011

                //▼====: #002
                default:
                    if (_StoreType != null)
                    {
                        switch (_StoreType)
                        {
                            case (long)AllLookupValues.StoreType.STORAGE_CLINIC:
                                ReportModel = new ClinicDeptInOutStocksDetailsReportModal_KT().PreviewModel;
                                break;
                            case (long)AllLookupValues.StoreType.STORAGE_DRUGDEPT:
                                ReportModel = new DrugDeptInOutStockValuesDetailsReportModal_KT().PreviewModel;
                                break;
                            case (long)AllLookupValues.StoreType.STORAGE_EXTERNAL:
                                break;
                        }

                        rParams["DateShow"].Value = "Từ ngày " + RptParameters.FromDate.GetValueOrDefault().ToShortDateString() + " đến ngày " + RptParameters.ToDate.GetValueOrDefault().ToShortDateString();
                        rParams["FromDate"].Value = RptParameters.FromDate;
                        rParams["ToDate"].Value = RptParameters.ToDate;
                        rParams["StorageName"].Value = CurStore.swhlName;
                        rParams["StoreID"].Value = CurStore.StoreID;
                        rParams["V_MedProductType"].Value = CurProductType.LookupID;
                        rParams["RefGenDrugCatID_1"].Value = RefGenDrugCatID_1;
                        rParams["DrugDeptProductGroupReportTypeID"].Value = SelectedDrugDeptProductGroupReportType;
                        rParams["parLogoUrl"].Value = Globals.ServerConfigSection.CommonItems.ReportLogoUrl;
                        rParams["parHospitalName"].Value = Globals.ServerConfigSection.CommonItems.ReportHospitalName;
                    }
                    break;
                    //▲====: #002
            }
            if (Globals.ServerConfigSection.CommonItems.AllowSearchInReport && eItem == ReportName.BC_NXT_THUOC_TONGHOP)
            {
                this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
                var DateShow = "Từ ngày " + RptParameters.FromDate.GetValueOrDefault().ToShortDateString() + " đến ngày " + RptParameters.ToDate.GetValueOrDefault().ToShortDateString();
                if (_StoreType == (long)AllLookupValues.StoreType.STORAGE_DRUGDEPT)
                {
                    var t = new Thread(() =>
                    {
                        try
                        {
                            using (var serviceFactory = new ReportServiceClient())
                            {
                                var contract = serviceFactory.ServiceInstance;
                                contract.BeginGetXRptInOutStockValueDrugDept_KT(
                                    RptParameters.FromDate, RptParameters.ToDate, CurStore.swhlName, CurStore.StoreID
                                    , DateShow, CurProductType.LookupID, RefGenDrugCatID_1
                                    , Globals.ServerConfigSection.CommonItems.ReportHospitalName
                                    , Globals.DispatchCallback((asyncResult) =>
                                    {
                                        try
                                        {
                                            var results = contract.EndGetXRptInOutStockValueDrugDept_KT(asyncResult);
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
                    var t = new Thread(() =>
                    {
                        try
                        {
                            using (var serviceFactory = new ReportServiceClient())
                            {
                                var contract = serviceFactory.ServiceInstance;
                                contract.BeginGetXRptInOutStockValueClinicDept_KT(
                                    RptParameters.FromDate, RptParameters.ToDate, CurStore.swhlName, CurStore.StoreID
                                    , DateShow, CurProductType.LookupID, RefGenDrugCatID_1
                                    , Globals.ServerConfigSection.CommonItems.ReportHospitalName
                                    , Globals.DispatchCallback((asyncResult) =>
                                    {
                                        try
                                        {
                                            var results = contract.EndGetXRptInOutStockValueClinicDept_KT(asyncResult);
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
                                contract.BeginGetXRptInOutStockValue_KT(
                                    RptParameters.FromDate, RptParameters.ToDate, CurStore.swhlName, CurStore.StoreID
                                    , DateShow
                                    , Globals.ServerConfigSection.CommonItems.ReportHospitalName
                                    , Globals.DispatchCallback((asyncResult) =>
                                    {
                                        try
                                        {
                                            var results = contract.EndGetXRptInOutStockValue_KT(asyncResult);
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

            else if (Globals.ServerConfigSection.CommonItems.AllowSearchInReport && eItem == ReportName.TK_NX_THEOMUCDICH && (isDetail || isDetailV2))
            {
                this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
                if (_StoreType == (long)AllLookupValues.StoreType.STORAGE_DRUGDEPT)
                {
                    var t = new Thread(() =>
                    {
                        try
                        {
                            using (var serviceFactory = new ReportServiceClient())
                            {
                                var contract = serviceFactory.ServiceInstance;
                                if (isDetail)
                                {
                                    contract.BeginGetXRpt_MedDeptInOutStatisticsDetail(
                                    RptParameters.FromDate, RptParameters.ToDate, CurStore.swhlName, CurStore.StoreID
                                    , CurStoreIn.StoreID, CurStoreOut.StoreID, SelectedPurposeIn.PurposeID, SelectedPurposeOut.PurposeID
                                    , CurProductType != null ? CurProductType.LookupID : (long)AllLookupValues.MedProductType.THUOC
                                    , Globals.ServerConfigSection.CommonItems.ReportHospitalName
                                    , reportDepartmentOfHealth
                                    , Globals.DispatchCallback((asyncResult) =>
                                    {
                                        try
                                        {
                                            var results = contract.EndGetXRpt_MedDeptInOutStatisticsDetail(asyncResult);
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

                                else
                                {
                                    contract.BeginGetXRpt_MedDeptInOutStatisticsDetail_V2(
                                    RptParameters.FromDate, RptParameters.ToDate, CurStore.swhlName, CurStore.StoreID
                                    , CurStoreIn.StoreID, CurStoreOut.StoreID, SelectedPurposeIn.PurposeID, SelectedPurposeOut.PurposeID
                                    , CurProductType != null ? CurProductType.LookupID : (long)AllLookupValues.MedProductType.THUOC
                                    , Globals.ServerConfigSection.CommonItems.ReportHospitalName
                                    , reportDepartmentOfHealth
                                    , Globals.DispatchCallback((asyncResult) =>
                                    {
                                        try
                                        {
                                            var results = contract.EndGetXRpt_MedDeptInOutStatisticsDetail_V2(asyncResult);
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
                    var t = new Thread(() =>
                    {
                        try
                        {
                            using (var serviceFactory = new ReportServiceClient())
                            {
                                var contract = serviceFactory.ServiceInstance;
                                if (isDetail)
                                {
                                    contract.BeginGetXRpt_ClinicDeptInOutStatisticsDetail(
                                    RptParameters.FromDate, RptParameters.ToDate, CurStore.swhlName, CurStore.StoreID
                                    , CurStoreIn.StoreID, CurStoreOut.StoreID, SelectedPurposeIn.PurposeID, SelectedPurposeOut.PurposeID
                                    , CurProductType != null ? CurProductType.LookupID : (long)AllLookupValues.MedProductType.THUOC
                                    , Globals.ServerConfigSection.CommonItems.ReportHospitalName
                                    , reportDepartmentOfHealth
                                    , Globals.DispatchCallback((asyncResult) =>
                                    {
                                        try
                                        {
                                            var results = contract.EndGetXRpt_ClinicDeptInOutStatisticsDetail(asyncResult);
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
                                else
                                {
                                    contract.BeginGetXRpt_ClinicDeptInOutStatisticsDetail_V2(
                                    RptParameters.FromDate, RptParameters.ToDate, CurStore.swhlName, CurStore.StoreID
                                    , CurStoreIn.StoreID, CurStoreOut.StoreID, SelectedPurposeIn.PurposeID, SelectedPurposeOut.PurposeID
                                    , CurProductType != null ? CurProductType.LookupID : (long)AllLookupValues.MedProductType.THUOC
                                    , Globals.ServerConfigSection.CommonItems.ReportHospitalName
                                    , reportDepartmentOfHealth
                                    , Globals.DispatchCallback((asyncResult) =>
                                    {
                                        try
                                        {
                                            var results = contract.EndGetXRpt_ClinicDeptInOutStatisticsDetail_V2(asyncResult);
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
                                if (isDetail)
                                {
                                    contract.BeginGetXRpt_DrugInOutStatisticsDetail(
                                    RptParameters.FromDate, RptParameters.ToDate, CurStore.swhlName, CurStore.StoreID
                                    , CurStoreIn.StoreID, CurStoreOut.StoreID, SelectedPurposeIn.PurposeID, SelectedPurposeOut.PurposeID
                                    , Globals.ServerConfigSection.CommonItems.ReportHospitalName
                                    , reportDepartmentOfHealth
                                    , Globals.DispatchCallback((asyncResult) =>
                                    {
                                        try
                                        {
                                            var results = contract.EndGetXRpt_DrugInOutStatisticsDetail(asyncResult);
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

                                else
                                {
                                    contract.BeginGetXRpt_DrugInOutStatisticsDetail_V2(
                                    RptParameters.FromDate, RptParameters.ToDate, CurStore.swhlName, CurStore.StoreID
                                    , CurStoreIn.StoreID, CurStoreOut.StoreID, SelectedPurposeIn.PurposeID, SelectedPurposeOut.PurposeID
                                    , Globals.ServerConfigSection.CommonItems.ReportHospitalName
                                    , reportDepartmentOfHealth
                                    , Globals.DispatchCallback((asyncResult) =>
                                    {
                                        try
                                        {
                                            var results = contract.EndGetXRpt_DrugInOutStatisticsDetail_V2(asyncResult);
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
            else if (Globals.ServerConfigSection.CommonItems.AllowSearchInReport && _eItem == ReportName.None)
            {
                this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
                var DateShow = "Từ ngày " + RptParameters.FromDate.GetValueOrDefault().ToShortDateString() + " đến ngày " + RptParameters.ToDate.GetValueOrDefault().ToShortDateString();
                if (_StoreType == (long)AllLookupValues.StoreType.STORAGE_DRUGDEPT)
                {
                    var t = new Thread(() =>
                    {
                        try
                        {
                            using (var serviceFactory = new ReportServiceClient())
                            {
                                var contract = serviceFactory.ServiceInstance;
                                contract.BeginGetXRptInOutStockValueDrugDeptDetails_KT(
                                    RptParameters.FromDate, RptParameters.ToDate, CurStore.swhlName, CurStore.StoreID
                                    , DateShow, CurProductType.LookupID, RefGenDrugCatID_1
                                    , Globals.ServerConfigSection.CommonItems.ReportHospitalName
                                    , Globals.DispatchCallback((asyncResult) =>
                                    {
                                        try
                                        {
                                            var results = contract.EndGetXRptInOutStockValueDrugDeptDetails_KT(asyncResult);
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
                    var t = new Thread(() =>
                    {
                        try
                        {
                            using (var serviceFactory = new ReportServiceClient())
                            {
                                var contract = serviceFactory.ServiceInstance;
                                contract.BeginGetXRptInOutStockClinicDeptDetails_KT(
                                    RptParameters.FromDate, RptParameters.ToDate, CurStore.swhlName, CurStore.StoreID
                                    , DateShow, CurProductType.LookupID, RefGenDrugCatID_1
                                    , Globals.ServerConfigSection.CommonItems.ReportHospitalName
                                    , Globals.DispatchCallback((asyncResult) =>
                                    {
                                        try
                                        {
                                            var results = contract.EndGetXRptInOutStockClinicDeptDetails_KT(asyncResult);
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

            //▼====: #009
            else if (Globals.ServerConfigSection.CommonItems.AllowSearchInReport && _eItem == ReportName.BC_KHO_TONGHOP)
            {
                this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
                var DateShow = "Từ ngày " + RptParameters.FromDate.GetValueOrDefault().ToShortDateString() + " đến ngày " + RptParameters.ToDate.GetValueOrDefault().ToShortDateString();

                var t = new Thread(() =>
                {
                    try
                    {
                        using (var serviceFactory = new ReportServiceClient())
                        {
                            var contract = serviceFactory.ServiceInstance;
                            contract.BeginGetXRptGeneralInOutStatistics_V3(
                                RptParameters.FromDate, RptParameters.ToDate, DateShow
                                , Globals.ServerConfigSection.CommonItems.ReportHospitalName
                                , Globals.ServerConfigSection.CommonItems.ReportHospitalAddress
                                , Globals.DispatchCallback((asyncResult) =>
                                {
                                    try
                                    {
                                        var results = contract.EndGetXRptGeneralInOutStatistics_V3(asyncResult);
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
            //▲====: #009

            else if (Globals.ServerConfigSection.CommonItems.AllowSearchInReport && eItem == ReportName.BC_NXT_THUOC_TONGHOP_COSO)
            {
                this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
                var DateShow = "Từ ngày " + RptParameters.FromDate.GetValueOrDefault().ToShortDateString() + " đến ngày " + RptParameters.ToDate.GetValueOrDefault().ToShortDateString();
                var t = new Thread(() =>
                {
                    try
                    {
                        using (var serviceFactory = new ReportServiceClient())
                        {
                            var contract = serviceFactory.ServiceInstance;
                            contract.BeginGetXRptInOutStockValueClinicDept_KT_V2(
                                RptParameters.FromDate, RptParameters.ToDate, CurStore.swhlName, 0
                                , DateShow, CurProductType.LookupID, RefGenDrugCatID_1
                                , Globals.ServerConfigSection.CommonItems.ReportHospitalName
                                , Globals.DispatchCallback((asyncResult) =>
                                {
                                    try
                                    {
                                        var results = contract.EndGetXRptInOutStockValueClinicDept_KT_V2(asyncResult);
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

            //▼====: #013
            else if (Globals.ServerConfigSection.CommonItems.AllowSearchInReport && eItem == ReportName.BCChiTietNhapTuNCC)
            {
                this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
                var DateShow = "Từ ngày " + RptParameters.FromDate.GetValueOrDefault().ToShortDateString() + " đến ngày " + RptParameters.ToDate.GetValueOrDefault().ToShortDateString();
                var t = new Thread(() =>
                {
                    try
                    {
                        using (var serviceFactory = new ReportServiceClient())
                        {
                            var contract = serviceFactory.ServiceInstance;
                            contract.BeginGetXRptBCNhapTuNCC(RptParameters.FromDate, RptParameters.ToDate, CurStore.swhlName
                                , CurStore.StoreID, DateShow, CurProductType.LookupID
                                , Globals.ServerConfigSection.CommonItems.ReportHospitalName
                                , Globals.DispatchCallback((asyncResult) =>
                                {
                                    try
                                    {
                                        var results = contract.EndGetXRptBCNhapTuNCC(asyncResult);
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
            //▲====: #013
            else
            {
                ReportModel.CreateDocument(rParams);
            }
        }

        #region Print Member
        public void btnXemIn(object sender, EventArgs e)
        {
            if (GetParameters())
            {
                GetReport(_eItem);
            }
        }

        public void btnXemChiTiet(object sender, EventArgs e)
        {
            if (GetParameters())
            {
                GetReport(_eItem, true);
            }
        }

        public void btnXemCTTheoThuoc(object sender, EventArgs e)
        {
            if(GetParameters())
            {
                GetReport(_eItem, false, true);
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
                RptParameters.Show = RptParameters.ShowFirst + string.Format(" {0} ", eHCMSResources.G0039_G1_Th.ToUpper()) + RptParameters.Month.ToString() + string.Format(" {0} ", eHCMSResources.N0033_G1_Nam) + RptParameters.Year.ToString();
            }
            else
            {
                RptParameters.Flag = 2;
                if (RptParameters.FromDate == null || RptParameters.ToDate == null)
                {
                    Globals.ShowMessage(string.Format("{0}!", eHCMSResources.K0364_G1_ChonNgThCanXemBC), eHCMSResources.G0442_G1_TBao);
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
            //▼====: #006
            if (CurStore == null || CurStoreIn == null || CurStoreOut == null)
            {
                result = false;
            }
            //▲====: #006

            //▼====: #008
            if (CurStore != null && CurStore.StoreID == 0 && CurProductType != null && CurProductType.LookupID == 0)
            {
                Globals.ShowMessage(string.Format("{0}!", "Không thể chọn loại --Tất cả-- cho TẤT CẢ kho"), eHCMSResources.G0442_G1_TBao);
                result = false;
            }
            //▲====: #008

            return result;
        }

        public bool IsEnabledToDatePicker { get; set; } = true;

        private RefPurposeForAccountant _SelectedPurposeIn;
        public RefPurposeForAccountant SelectedPurposeIn
        {
            get => _SelectedPurposeIn; set
            {
                _SelectedPurposeIn = value;
                NotifyOfPropertyChange(() => SelectedPurposeIn);
            }
        }

        private RefPurposeForAccountant _SelectedPurposeOut;
        public RefPurposeForAccountant SelectedPurposeOut
        {
            get => _SelectedPurposeOut; set
            {
                _SelectedPurposeOut = value;
                NotifyOfPropertyChange(() => SelectedPurposeOut);
            }
        }

        private ObservableCollection<RefPurposeForAccountant> _AllPurposeForAccountant;
        public ObservableCollection<RefPurposeForAccountant> AllPurposeForAccountant
        {
            get { return _AllPurposeForAccountant; }
            set
            {
                _AllPurposeForAccountant = value;
                NotifyOfPropertyChange(() => AllPurposeForAccountant);
            }
        }

        private ObservableCollection<RefPurposeForAccountant> _ListPurposeIn;
        public ObservableCollection<RefPurposeForAccountant> ListPurposeIn
        {
            get { return _ListPurposeIn; }
            set
            {
                _ListPurposeIn = value;
                NotifyOfPropertyChange(() => ListPurposeIn);
            }
        }

        private ObservableCollection<RefPurposeForAccountant> _ListPurposeOut;
        public ObservableCollection<RefPurposeForAccountant> ListPurposeOut
        {
            get { return _ListPurposeOut; }
            set
            {
                _ListPurposeOut = value;
                NotifyOfPropertyChange(() => ListPurposeOut);
            }
        }

        private void LoadRefPurposeForAccountant_All()
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ConfigurationManagerServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetAllRefPurposeForAccountant(Globals.DispatchCallback((asyncResult) =>
                        {
                            AllPurposeForAccountant = contract.EndGetAllRefPurposeForAccountant(asyncResult).ToObservableCollection();
                            ListPurposeOut = AllPurposeForAccountant.Where(x => x.PurposeType == 2).ToObservableCollection();
                            ListPurposeIn = AllPurposeForAccountant.Where(x => x.PurposeType == 1).ToObservableCollection();

                            var itemDefault = new RefPurposeForAccountant
                            {
                                PurposeID = 0,
                                PurposeName = "--Tất cả--"
                            };

                            ListPurposeIn.Insert(0, itemDefault);
                            ListPurposeOut.Insert(0, itemDefault);

                            SelectedPurposeIn = itemDefault;
                            SelectedPurposeOut = itemDefault;
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
                }
                finally
                {
                    this.HideBusyIndicator();
                }
            });

            t.Start();
        }

        private bool _IsExportVisible = true;
        public bool IsExportVisible
        {
            get
            {
                return _IsExportVisible;
            }
            set
            {
                _IsExportVisible = value;
                NotifyOfPropertyChange(() => IsExportVisible);
            }
        }

        public void btnExportExcel()
        {
            DevExpress.DocumentServices.ServiceModel.DefaultValueParameterContainer rParams = new DevExpress.DocumentServices.ServiceModel.DefaultValueParameterContainer();
            if (eItem == ReportName.REPORT_IMPORT_EXPORT_DEPARTMENT || eItem == ReportName.BC_NXT_THUOC_TONGHOP || eItem == ReportName.BCXuatSDThuocCanQuang)
            {
                //▼====: #014
                if (eItem == ReportName.REPORT_IMPORT_EXPORT_DEPARTMENT || eItem == ReportName.BC_NXT_THUOC_TONGHOP)
                {
                    rParams["Quarter"].Value = RptParameters.Quarter;
                    rParams["Month"].Value = RptParameters.Month;
                    rParams["Year"].Value = RptParameters.Year;
                    rParams["flag"].Value = RptParameters.Flag;
                    rParams["FromDate"].Value = RptParameters.FromDate;
                    rParams["ToDate"].Value = RptParameters.ToDate;
                    rParams["ReportDate"].Value = RptParameters.Show;
                    rParams["ReportTitle"].Value = eHCMSResources.Z1142_G1_BCBNNpXuatKhoa.ToUpper();
                }
                else
                {
                    rParams["FromDate"].Value = RptParameters.FromDate;
                    rParams["ToDate"].Value = RptParameters.ToDate;
                    rParams["ReportDate"].Value = RptParameters.Show;
                    rParams["ReportTitle"].Value = eHCMSResources.Z3298_G1_BCXuatSDThuocCanQuang.ToUpper();
                }
                //▲====: #014

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
                RptParameters.reportName = eItem;
                RptParameters.ReportType = ReportType.BAOCAO_TONGHOP;

                if (GetParameters())
                {
                    switch (eItem)
                    {
                        case ReportName.REPORT_IMPORT_EXPORT_DEPARTMENT:
                            RptParameters.Show = "IMPORT_EXPORT_DEPARTMENT";
                            break;
                        //▼====: #007
                        case ReportName.BC_NXT_THUOC_TONGHOP:
                            if (_StoreType != null)
                            {
                                RptParameters.ReportType = ReportType.BAOCAO_TONGHOP_KT;
                                RptParameters.StoreID = CurStore.StoreID;
                                RptParameters.V_MedProductType = CurProductType.LookupID;
                                RptParameters.RefGenDrugCatID_1 = RefGenDrugCatID_1;
                                RptParameters.SelectedDrugDeptProductGroupReportType = SelectedDrugDeptProductGroupReportType;
                                RptParameters.StoreType = _StoreType;
                                RptParameters.Show = "BC_NXT_THUOC_TONGHOP";
                            }
                            break;
                        //▲====: #007
                        //▼====: #014
                        case ReportName.BCXuatSDThuocCanQuang:
                            RptParameters.StoreID = CurStore.StoreID;
                            RptParameters.V_MedProductType = CurProductType.LookupID;
                            break;
                        //▲====: #014
                    }
                    ExportToExcelGeneric.Action(RptParameters, objSFD, this);
                }
            }
        }

        private bool _ChonKho = false;
        public bool ChonKho
        {
            get
            {
                return _ChonKho;
            }
            set
            {
                if (_ChonKho == value)
                    return;
                _ChonKho = value;
                NotifyOfPropertyChange(() => ChonKho);
            }
        }

        private long? _StoreType = 0;
        public void GetListStore(long? StoreType)
        {
            _StoreType = StoreType;
            Coroutine.BeginExecute(DoGetStore(StoreType));
            //▼====: #006
            if (_StoreType == (long)AllLookupValues.StoreType.STORAGE_DRUGDEPT)
            {
                GetListStoreInOut((long)AllLookupValues.StoreType.STORAGE_CLINIC);
            }
            else
            {
                GetListStoreInOut((long)AllLookupValues.StoreType.STORAGE_DRUGDEPT);
            }
            //▲====: #006
        }

        private IEnumerator<IResult> DoGetStore(long? StoreType)
        {
            var paymentTypeTask = new LoadStoreListTask(StoreType, false, null, false, false);
            yield return paymentTypeTask;
            //▼====: #006
            //if (CurProductType.LookupID != 0)
            //{
            //    StoreCbx = paymentTypeTask.LookupList.Where(x => (CurProductType != null && CurProductType.LookupID != 0 && x.ListV_MedProductType != null && x.ListV_MedProductType.Contains(CurProductType.LookupID.ToString()))).ToObservableCollection();
            //}
            //else
            //{
            //    StoreCbx = paymentTypeTask.LookupList.ToObservableCollection();
            //}
            //▲====: #006

            //▼====: #010
            var StoreTemp = new ObservableCollection<RefStorageWarehouseLocation>();
            if (CurProductType.LookupID != 0)
            {
                StoreTemp = paymentTypeTask.LookupList.Where(x => (CurProductType != null && CurProductType.LookupID != 0 && x.ListV_MedProductType != null && x.ListV_MedProductType.Contains(CurProductType.LookupID.ToString()))).ToObservableCollection();
                StoreCbx = Globals.CheckDrugMedStoreWareHouse(StoreTemp);
            }
            else
            {
                StoreTemp = paymentTypeTask.LookupList.ToObservableCollection();
                StoreCbx = Globals.CheckDrugMedStoreWareHouse(StoreTemp);
            }
            //▲====: #010

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

        public void GetListStoreInOut(long? StoreType)
        {
            Coroutine.BeginExecute(DoGetStoreInOut(StoreType));
        }

        private IEnumerator<IResult> DoGetStoreInOut(long? StoreType)
        {
            var paymentTypeTask = new LoadStoreListTask(StoreType, false, null, false, true);
            yield return paymentTypeTask;
            //▼====: #006
            if (CurProductType.LookupID != 0)
            {
                StoreInCbx = paymentTypeTask.LookupList.Where(x => (CurProductType != null && CurProductType.LookupID != 0 && x.ListV_MedProductType != null && x.ListV_MedProductType.Contains(CurProductType.LookupID.ToString()))).ToObservableCollection();
                StoreOutCbx = paymentTypeTask.LookupList.Where(x => (CurProductType != null && CurProductType.LookupID != 0 && x.ListV_MedProductType != null && x.ListV_MedProductType.Contains(CurProductType.LookupID.ToString()))).ToObservableCollection();
            }
            else
            {
                StoreInCbx = paymentTypeTask.LookupList.ToObservableCollection();
                StoreOutCbx = paymentTypeTask.LookupList.ToObservableCollection();
            }
            //▲====: #006
            if (CurProductType != null && CurProductType.LookupID == (long)AllLookupValues.MedProductType.THUOC && _StoreType == (long)AllLookupValues.StoreType.STORAGE_DRUGDEPT)
            {
                StoreInCbx.Add(new RefStorageWarehouseLocation { swhlName = "Kho lẻ thuốc nội trú", StoreID = (long)AllLookupValues.StoreID.KHO_LE_THUOC_NOI_TRU });
                StoreInCbx.Add(new RefStorageWarehouseLocation { swhlName = "Kho lẻ thuốc BHYT ngoại trú", StoreID = Globals.ServerConfigSection.PharmacyElements.HIStorageID });
                StoreOutCbx.Add(new RefStorageWarehouseLocation { swhlName = "Kho lẻ thuốc nội trú", StoreID = (long)AllLookupValues.StoreID.KHO_LE_THUOC_NOI_TRU });
                StoreOutCbx.Add(new RefStorageWarehouseLocation { swhlName = "Kho lẻ thuốc BHYT ngoại trú", StoreID = Globals.ServerConfigSection.PharmacyElements.HIStorageID });

            }
            else if (CurProductType != null && CurProductType.LookupID == (long)AllLookupValues.MedProductType.Y_CU)
            {
                StoreInCbx.Add(new RefStorageWarehouseLocation { swhlName = "Kho lẻ vật tư y tế", StoreID = (long)AllLookupValues.StoreID.KHO_LE_VTYT });
                StoreOutCbx.Add(new RefStorageWarehouseLocation { swhlName = "Kho lẻ vật tư y tế", StoreID = (long)AllLookupValues.StoreID.KHO_LE_VTYT });
            }

            var itemDefault = new RefStorageWarehouseLocation
            {
                StoreID = 0,
                swhlName = "--Tất cả--"
            };

            StoreInCbx.Insert(0, itemDefault);
            StoreOutCbx.Insert(0, itemDefault);

            CurStoreIn = itemDefault;
            CurStoreOut = itemDefault;

            yield break;
        }

        public void cboStoreCbx_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        public void CbxV_MedProductType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CurProductType != null)
            {
                //▼====: #002
                if (CurProductType.LookupID == (long)AllLookupValues.MedProductType.THUOC)
                {
                    IsShowGroupReportType = false;
                    CanSelectedRefGenDrugCatID_1 = true;
                    if (RefGenericDrugCategory_1s == null)
                    {
                        LoadRefGenericDrugCategory_1();
                    }
                }
                else if (CurProductType.LookupID == (long)AllLookupValues.MedProductType.Y_CU)
                {
                    IsShowGroupReportType = true;
                    CanSelectedRefGenDrugCatID_1 = false;
                }
                else
                {
                    IsShowGroupReportType = false;
                    CanSelectedRefGenDrugCatID_1 = false;
                }
                //▲====: #002
                GetListStore(_StoreType);
            }
            if(_eItem == ReportName.BCChiTietNhapTuNCC)
            {
                IsShowGroupReportType = false;
                CanSelectedRefGenDrugCatID_1 = false;
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
                    //▼====: #008
                    _StoreCbx = value;
                    if (_StoreType == (long)AllLookupValues.StoreType.STORAGE_DRUGDEPT && eItem == ReportName.TK_NX_THEOMUCDICH)
                    { 
                        RefStorageWarehouseLocation firstItem = new RefStorageWarehouseLocation
                        {
                            StoreID = 0,
                            swhlName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.T0822_G1_TatCa)
                        };
                        _StoreCbx.Insert(0, firstItem);
                    }
                    //▲====: #008
                    //▼====: #014
                    else if (eItem == ReportName.BCXuatSDThuocCanQuang)
                    {
                        _StoreCbx = _StoreCbx.Where(x => x.DeptID == (long)AllLookupValues.DeptID.KhoaKham).ToObservableCollection();
                        RefStorageWarehouseLocation firstItem = new RefStorageWarehouseLocation
                        {
                            StoreID = 0,
                            swhlName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.T0822_G1_TatCa)
                        };
                        _StoreCbx.Insert(0, firstItem);
                    }
                    //▲====: #014
                    NotifyOfPropertyChange(() => StoreCbx);
                }
            }
        }

        private ObservableCollection<RefStorageWarehouseLocation> _StoreInCbx;
        public ObservableCollection<RefStorageWarehouseLocation> StoreInCbx
        {
            get
            {
                return _StoreInCbx;
            }
            set
            {
                if (_StoreInCbx != value)
                {
                    _StoreInCbx = value;
                    NotifyOfPropertyChange(() => StoreInCbx);
                }
            }
        }

        private ObservableCollection<RefStorageWarehouseLocation> _StoreOutCbx;
        public ObservableCollection<RefStorageWarehouseLocation> StoreOutCbx
        {
            get
            {
                return _StoreOutCbx;
            }
            set
            {
                if (_StoreOutCbx != value)
                {
                    _StoreOutCbx = value;
                    NotifyOfPropertyChange(() => StoreOutCbx);
                }
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

        private RefStorageWarehouseLocation _CurStoreIn;
        public RefStorageWarehouseLocation CurStoreIn
        {
            get { return _CurStoreIn; }
            set
            {
                _CurStoreIn = value;
                NotifyOfPropertyChange(() => CurStoreIn);
            }
        }

        private RefStorageWarehouseLocation _CurStoreOut;
        public RefStorageWarehouseLocation CurStoreOut
        {
            get { return _CurStoreOut; }
            set
            {
                _CurStoreOut = value;
                NotifyOfPropertyChange(() => CurStoreOut);
            }
        }

        private bool CheckStoreCbx()
        {
            if(CurStore != null && CurStore.StoreID == 0 && CurProductType != null && CurProductType.LookupID == 0)
            {
                Globals.ShowMessage(string.Format("{0}!", "Không thể chọn loại --Tất cả-- cho TẤT CẢ kho"), eHCMSResources.G0442_G1_TBao);
                return false;
            }
            return true;
        }

        private bool _IsMedProductTypeVisible = true;
        public bool IsMedProductTypeVisible
        {
            get
            {
                return _IsMedProductTypeVisible;
            }
            set
            {
                _IsMedProductTypeVisible = value;
                NotifyOfPropertyChange(() => IsMedProductTypeVisible);
            }
        }
    }
}
