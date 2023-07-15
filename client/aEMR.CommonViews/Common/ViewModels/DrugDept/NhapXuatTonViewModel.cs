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
using System.Linq;
using aEMR.Common.Printing;
using System.Collections.Generic;
using aEMR.CommonTasks;
using eHCMSLanguage;
using aEMR.ReportModel.ReportModels;
using DevExpress.ReportServer.Printing;
using aEMR.Common.Collections;
using System.IO;
using DevExpress.XtraReports.UI;
using DevExpress.Xpf.Printing;
using aEMR.Common.BaseModel;
/*
* 20190514 #001 TNHX:  BM 0006872: Create BC_KhangSinh report
* 20190515 #002 TNHX:  BM 0006872: Create BC_SuDungThuoc, BC_SuDungHoaChat report
* 20190625 #003 TNHX:  BM 0011883: Create KT_BCHangTonNhieu report
* 20190828 #004 TNHX:  BM 0013246: update design NXT report for TV
* 20190917 #005 TNHX:  BM 0013247: Apply search for XRptInOutStockValueDrugDept_TV base on config "AllowSearchInReport"
* 20211103 #006 QTD:   Lọc kho theo cấu hình trách nhiệm
*/
namespace aEMR.Common.ViewModels
{
    [Export(typeof(IDrugDeptNhapXuatTon)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class NhapXuatTonViewModel : ViewModelBase, IDrugDeptNhapXuatTon
    {
        public NhapXuatTonViewModel()
        {
            Authorization();
            FromDate = DateTime.Now;
            ToDate = DateTime.Now;

            RefMedProductType = Globals.AllLookupValueList.Where(x => x.ObjectName == "V_MedProductType").ToList();
            SelectedMedProductType = RefMedProductType.FirstOrDefault();

            V_RangeOfWareHouses = Globals.AllLookupValueList.Where(x => x.ObjectName == "V_RangeOfWareHouses").ToList();
            SelectedRangeOfWareHouses = V_RangeOfWareHouses.FirstOrDefault();
            CommonGlobals.GetAllPositionInHospital(this);
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            Coroutine.BeginExecute(DoGetStore_DrugDept());
        }
        private List<Lookup> _RefMedProductType;
        public List<Lookup> RefMedProductType
        {
            get
            {
                return _RefMedProductType;
            }
            set
            {
                if (_RefMedProductType != value)
                {
                    _RefMedProductType = value;
                    NotifyOfPropertyChange(() => RefMedProductType);
                }
            }
        }

        private Lookup _SelectedMedProductType;
        public Lookup SelectedMedProductType
        {
            get
            {
                return _SelectedMedProductType;
            }
            set
            {
                if (_SelectedMedProductType != value)
                {
                    _SelectedMedProductType = value;
                    if (SelectedMedProductType != null)
                    {
                        V_MedProductType = SelectedMedProductType.LookupID;
                    }
                    NotifyOfPropertyChange(() => SelectedMedProductType);
                }
            }
        }

        private List<Lookup> _V_RangeOfWareHouses;
        public List<Lookup> V_RangeOfWareHouses
        {
            get
            {
                return _V_RangeOfWareHouses;
            }
            set
            {
                if (_V_RangeOfWareHouses != value)
                {
                    _V_RangeOfWareHouses = value;
                    NotifyOfPropertyChange(() => V_RangeOfWareHouses);
                }
            }
        }

        private Lookup _SelectedRangeOfWareHouses;
        public Lookup SelectedRangeOfWareHouses
        {
            get
            {
                return _SelectedRangeOfWareHouses;
            }
            set
            {
                if (_SelectedRangeOfWareHouses != value)
                {
                    _SelectedRangeOfWareHouses = value;
                    NotifyOfPropertyChange(() => SelectedRangeOfWareHouses);
                }
            }
        }

        private long _V_MedProductType;
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

        private bool _CanSelectedRefGenDrugCatID_1;
        public bool CanSelectedRefGenDrugCatID_1
        {
            get
            {
                return _CanSelectedRefGenDrugCatID_1;
            }
            set
            {
                if (_CanSelectedRefGenDrugCatID_1 != value)
                {
                    _CanSelectedRefGenDrugCatID_1 = value;
                    NotifyOfPropertyChange(() => CanSelectedRefGenDrugCatID_1);
                }
            }
        }

        private bool _ShowBid = false;
        public bool ShowBid
        {
            get
            {
                return _ShowBid;
            }
            set
            {
                if (_ShowBid != value)
                {
                    _ShowBid = value;
                    NotifyOfPropertyChange(() => ShowBid);
                }
            }
        }

        private bool _ShowRangeOfHospital = false;
        public bool ShowRangeOfHospital
        {
            get
            {
                return _ShowRangeOfHospital;
            }
            set
            {
                if (_ShowRangeOfHospital != value)
                {
                    _ShowRangeOfHospital = value;
                    NotifyOfPropertyChange(() => ShowRangeOfHospital);
                }
            }
        }

        public string strHienThi { get; set; }

        private bool _IsGetValue = false;
        public bool IsGetValue
        {
            get { return _IsGetValue; }
            set
            {
                if (_IsGetValue != value)
                {
                    _IsGetValue = value;
                    NotifyOfPropertyChange(() => IsGetValue);
                }
            }
        }

        private bool _IsShowClinic = false;
        public bool IsShowClinic
        {
            get { return _IsShowClinic; }
            set
            {
                if (_IsShowClinic != value)
                {
                    _IsShowClinic = value;
                    NotifyOfPropertyChange(() => IsShowClinic);
                }
            }
        }

        private bool _IsShowBHYT = false;
        public bool IsShowBHYT
        {
            get { return _IsShowBHYT; }
            set
            {
                if (_IsShowBHYT != value)
                {
                    _IsShowBHYT = value;
                    NotifyOfPropertyChange(() => IsShowBHYT);
                }
            }
        }

        private bool _IsBHYT = false;
        public bool IsBHYT
        {
            get
            {
                return _IsBHYT;
            }
            set
            {
                if (_IsBHYT != value)
                {
                    _IsBHYT = value;
                    NotifyOfPropertyChange(() => IsBHYT);
                }
            }
        }

        private Bid _SelectedBid;
        public Bid SelectedBid
        {
            get
            {
                return _SelectedBid;
            }
            set
            {
                if (_SelectedBid == value)
                {
                    return;
                }
                _SelectedBid = value;
                NotifyOfPropertyChange(() => SelectedBid);
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

        private DateTime? _FromDate;
        public DateTime? FromDate
        {
            get { return _FromDate; }
            set
            {
                _FromDate = value;
                NotifyOfPropertyChange(() => FromDate);
            }
        }
        private DateTime? _ToDate;
        public DateTime? ToDate
        {
            get { return _ToDate; }
            set
            {
                _ToDate = value;
                NotifyOfPropertyChange(() => ToDate);
            }
        }

        private RefStorageWarehouseLocation _Store;
        public RefStorageWarehouseLocation Store
        {
            get { return _Store; }
            set
            {
                _Store = value;
                NotifyOfPropertyChange(() => Store);
            }
        }

        private string DateShow;
        private string StorageName;

        private string _CheckPointName;
        public string CheckPointName
        {
            get { return _CheckPointName; }
            set
            {
                _CheckPointName = value;
                NotifyOfPropertyChange(() => CheckPointName);
            }
        }

        private bool _IsCheck = true;
        public bool IsCheck
        {
            get { return _IsCheck; }
            set
            {
                _IsCheck = value;
                NotifyOfPropertyChange(() => IsCheck);
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

        #endregion

        public void Authorization()
        {
        }
        #region checking account

        private bool _mXemIn = true;
        private bool _mKetChuyen = true;
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

        private bool _canPrint = false;
        public bool CanPrint
        {
            get
            {
                return _canPrint;
            }
            set
            {
                if (_canPrint == value)
                    return;
                _canPrint = value;
                NotifyOfPropertyChange(() => CanPrint);
            }
        }

        public bool mKetChuyen
        {
            get
            {
                return _mKetChuyen;
            }
            set
            {
                if (_mKetChuyen == value)
                    return;
                _mKetChuyen = value;
                NotifyOfPropertyChange(() => mKetChuyen);
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
        #endregion

        public void LoadRefGenericDrugCategory_1()
        {
            Coroutine.BeginExecute(DoGetRefGenericDrugCategory_1List());
        }

        private IEnumerator<IResult> DoGetRefGenericDrugCategory_1List()
        {
            this.ShowBusyIndicator();
            var paymentTypeTask = new LoadRefGenericDrugCategory_1ListTask(V_MedProductType, false, true);
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

        private IEnumerator<IResult> DoGetStore_DrugDept()
        {
            long StoreTypeID = (long)AllLookupValues.StoreType.STORAGE_DRUGDEPT;
            if (IsShowClinic)
                StoreTypeID = (long)AllLookupValues.StoreType.STORAGE_CLINIC;
            var paymentTypeTask = new LoadStoreListTask(StoreTypeID, false, null, false, false);
            yield return paymentTypeTask;
            //StoreCbx = paymentTypeTask.LookupList.Where(x => (V_MedProductType != 0 && x.ListV_MedProductType != null && x.ListV_MedProductType.Contains(V_MedProductType.ToString()))).ToObservableCollection();
            //▼===== #006
            var StoreTemp = paymentTypeTask.LookupList.Where(x => (V_MedProductType != 0 && x.ListV_MedProductType != null && x.ListV_MedProductType.Contains(V_MedProductType.ToString()))).ToObservableCollection();
            StoreCbx = Globals.CheckDrugMedStoreWareHouse(StoreTemp);
            if (StoreCbx == null || StoreCbx.Count == 0)
            {
                MessageBox.Show(eHCMSResources.A0110_G1_Msg_InfoChuaCauHinhTNKho.Replace("\\n ", "\n"));
            }
            //▲===== #006
            yield break;
        }

        public void btn_View(object sender, RoutedEventArgs e)
        {
            if (CheckData())
            {
                DevExpress.DocumentServices.ServiceModel.DefaultValueParameterContainer rParams = new DevExpress.DocumentServices.ServiceModel.DefaultValueParameterContainer();
                ReportModel = null;
                string ReportTitle = "";
                switch (eItem)
                {
                    case ReportName.BC_NXT_THUOC_TONGHOP:
                        ReportModel = new InOutStocksDrugsGeneralReportModal().PreviewModel;
                        rParams["FromDate"].Value = FromDate;
                        rParams["ToDate"].Value = ToDate;
                        rParams["StorageName"].Value = StorageName;
                        rParams["StoreID"].Value = StoreID;
                        rParams["DateShow"].Value = DateShow;
                        rParams["V_MedProductType"].Value = V_MedProductType;
                        rParams["parHospitalName"].Value = Globals.ServerConfigSection.CommonItems.ReportHospitalName;
                        break;

                    case ReportName.BC_NXT_THUOC_TONGHOP_V2:
                        ReportModel = new InOutStocksDrugsGeneralReportModal_V2().PreviewModel;
                        rParams["FromDate"].Value = FromDate;
                        rParams["ToDate"].Value = ToDate;
                        rParams["StorageName"].Value = StorageName;
                        rParams["StoreID"].Value = StoreID;
                        rParams["DateShow"].Value = DateShow;
                        rParams["V_MedProductType"].Value = V_MedProductType;
                        rParams["IsBHYT"].Value = Convert.ToBoolean(IsBHYT);
                        rParams["parHospitalName"].Value = Globals.ServerConfigSection.CommonItems.ReportHospitalName;
                        break;

                    case ReportName.TT22_BC_KhangSinh:
                        ReportModel = new KhangSinhReportModal().PreviewModel;
                        rParams["FromDate"].Value = FromDate;
                        rParams["ToDate"].Value = ToDate;
                        rParams["StoreID"].Value = StoreID;
                        rParams["DateShow"].Value = "Từ ngày " + FromDate.GetValueOrDefault().ToShortDateString() + " đến ngày " + ToDate.GetValueOrDefault().ToShortDateString();
                        rParams["V_MedProductType"].Value = V_MedProductType;
                        rParams["parHospitalName"].Value = Globals.ServerConfigSection.CommonItems.ReportHospitalName;
                        rParams["parDepartmentOfHealth"].Value = Globals.ServerConfigSection.CommonItems.ReportDepartmentOfHealth;
                        break;
                    //▲====: #001
                    //▼====: #002
                    case ReportName.TT22_BC_SuDungThuoc:
                    case ReportName.TT22_BC_SuDungHoaChat:
                        ReportModel = new HospitalUseOfDrugsReportModal().PreviewModel;
                        rParams["FromDate"].Value = FromDate;
                        rParams["ToDate"].Value = ToDate;
                        rParams["StoreID"].Value = StoreID;
                        rParams["DateShow"].Value = "Thời gian từ " + FromDate.GetValueOrDefault().ToShortDateString() + " - " + ToDate.GetValueOrDefault().ToShortDateString();
                        rParams["V_MedProductType"].Value = V_MedProductType;
                        rParams["parHospitalName"].Value = Globals.ServerConfigSection.CommonItems.ReportHospitalName;
                        rParams["parDepartmentOfHealth"].Value = Globals.ServerConfigSection.CommonItems.ReportDepartmentOfHealth;
                        var CurrentModule = Globals.GetViewModel<IDrugModule>();
                        if (Globals.ServerConfigSection.MedDeptElements.UseDrugDeptAs2DistinctParts &&
                            Globals.AllPositionInHospital != null && CurrentModule != null && CurrentModule.MenuVisibleCollection[1] &&
                            Globals.AllPositionInHospital.Any(x => x.PositionRefID == (int)AllLookupValues.StaffPositions_Enum.TRUONG_KHOA_VATTU))
                        {
                            rParams["pDeptDirectorSignTitle"].Value = Globals.AllPositionInHospital.First(x => x.PositionRefID == (int)AllLookupValues.StaffPositions_Enum.TRUONG_KHOA_VATTU).PositionName.ToUpper();
                        }
                        break;
                    //▲====: #002
                    case ReportName.DrugDeptInOutStockByBid:
                        ReportModel = new GenericReportModel("eHCMS.ReportLib.RptDrugDepts.XtraReports.Reports.XRptDrugDeptInOutStockByBid").PreviewModel;
                        rParams["StartDate"].Value = FromDate;
                        rParams["EndDate"].Value = ToDate;
                        rParams["StoreID"].Value = StoreID;
                        rParams["V_MedProductType"].Value = V_MedProductType;
                        rParams["BidID"].Value = SelectedBid == null ? 0 : SelectedBid.BidID;
                        break;
                    default:
                        if (V_MedProductType == (long)AllLookupValues.MedProductType.THUOC)
                        {
                            if (RefGenDrugCatID_1 == (long)AllLookupValues.RefGenDrugCatID_1.GAYNGHIEN)
                            {
                                ReportTitle = eHCMSResources.N0227_G1_NXTThuocGN.ToUpper();
                            }
                            else if (RefGenDrugCatID_1 == (long)AllLookupValues.RefGenDrugCatID_1.HUONGTHAN)
                            {
                                ReportTitle = eHCMSResources.N0228_G1_NXTThuocHTT.ToUpper();
                            }
                            else
                            {
                                ReportTitle = eHCMSResources.N0226_G1_NXTThuoc.ToUpper();
                            }
                        }
                        else if (V_MedProductType == (long)AllLookupValues.MedProductType.Y_CU)
                        {
                            ReportTitle = eHCMSResources.N0229_G1_NXTYCu.ToUpper();
                        }
                        else if (V_MedProductType == (long)AllLookupValues.MedProductType.NUTRITION)
                        {
                            ReportTitle = eHCMSResources.Z3221_G1_NXTDDuong.ToUpper();
                        }
                        else if (V_MedProductType == (long)AllLookupValues.MedProductType.HOA_CHAT)
                        {
                            ReportTitle = eHCMSResources.N0224_G1_NXTHoaChat.ToUpper();
                        }

                        if (IsGetValue)
                        {
                            //▼====: #004
                            ReportModel = new DrugDeptInOutStockValuesReportModal_TV().PreviewModel;
                            rParams["parHospitalName"].Value = Globals.ServerConfigSection.CommonItems.ReportHospitalName;
                            DateShow = "Từ ngày " + FromDate.GetValueOrDefault().ToShortDateString() + " đến ngày " + ToDate.GetValueOrDefault().ToShortDateString();
                            if (SelectedBid != null)
                            {
                                rParams["BidID"].Value = SelectedBid.BidID;
                            }
                            //▲====: #004
                        }
                        else
                        {
                            ReportModel = new DrugDeptInOutStocksReportModal().PreviewModel;
                            if (SelectedBid != null)
                            {
                                rParams["BidID"].Value = SelectedBid.BidID;
                            }
                        }
                        rParams["ReportTitle"].Value = ReportTitle;
                        rParams["FromDate"].Value = FromDate;
                        rParams["ToDate"].Value = ToDate;
                        rParams["StorageName"].Value = StorageName;
                        rParams["StoreID"].Value = (int)StoreID;
                        rParams["DateShow"].Value = DateShow;
                        rParams["V_MedProductType"].Value = Convert.ToInt32(V_MedProductType);
                        rParams["RefGenDrugCatID_1"].Value = (int)RefGenDrugCatID_1;
                        rParams["DrugDeptProductGroupReportTypeID"].Value = (int)SelectedDrugDeptProductGroupReportType;
                        rParams["parLogoUrl"].Value = Globals.ServerConfigSection.CommonItems.ReportLogoUrl;
                        // ReportModel.AutoShowParametersPanel = false;
                        break;
                    //▼====: #003
                    case ReportName.KT_BCHangTonNhieu:
                        ReportModel = new GenericReportModel("eHCMS.ReportLib.RptTransactions.XtraReports.Accountant.XRpt_HangTonNhieu").PreviewModel;
                        rParams["FromDate"].Value = FromDate;
                        rParams["ToDate"].Value = ToDate;
                        rParams["StorageName"].Value = StorageName;
                        rParams["StoreID"].Value = StoreID;
                        rParams["DateShow"].Value = DateShow;
                        rParams["V_MedProductType"].Value = V_MedProductType;
                        rParams["parHospitalName"].Value = Globals.ServerConfigSection.CommonItems.ReportHospitalName;
                        rParams["DrugDeptProductGroupReportTypeID"].Value = (int)SelectedDrugDeptProductGroupReportType;
                        break;
                        //▲====: #003
                }
                //▼====: #005
                if (Globals.ServerConfigSection.CommonItems.AllowSearchInReport && eItem == ReportName.BC_NXT_THUOC_TONGHOP)
                {
                    this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
                    DateShow = "Từ ngày " + FromDate.GetValueOrDefault().ToShortDateString() + " đến ngày " + ToDate.GetValueOrDefault().ToShortDateString();
                    var t = new Thread(() =>
                    {
                        try
                        {
                            using (var serviceFactory = new ReportServiceClient())
                            {
                                var contract = serviceFactory.ServiceInstance;
                                contract.BeginGetXRptInOutStockGeneral(
                                    FromDate, ToDate, StorageName, StoreID
                                    , DateShow, V_MedProductType
                                    , Globals.ServerConfigSection.CommonItems.ReportHospitalName
                                    , Globals.DispatchCallback((asyncResult) =>
                                    {
                                        try
                                        {
                                            var results = contract.EndGetXRptInOutStockGeneral(asyncResult);
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

                //▼====: QTD NXT Thuốc toàn Khoa DƯỢC
                else if (Globals.ServerConfigSection.CommonItems.AllowSearchInReport && eItem == ReportName.BC_NXT_THUOC_TONGHOP_V2)
                {
                    this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
                    DateShow = "Từ ngày " + FromDate.GetValueOrDefault().ToShortDateString() + " đến ngày " + ToDate.GetValueOrDefault().ToShortDateString();
                    var t = new Thread(() =>
                    {
                        try
                        {
                            using (var serviceFactory = new ReportServiceClient())
                            {
                                var contract = serviceFactory.ServiceInstance;
                                contract.BeginGetXRptInOutStockGeneral_BHYT(
                                    FromDate, ToDate, StorageName, StoreID
                                    , DateShow, V_MedProductType
                                    , IsBHYT
                                    , Globals.ServerConfigSection.CommonItems.ReportHospitalName
                                    , Globals.DispatchCallback((asyncResult) =>
                                    {
                                        try
                                        {
                                            var results = contract.EndGetXRptInOutStockGeneral_BHYT(asyncResult);
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

                else if (Globals.ServerConfigSection.CommonItems.AllowSearchInReport && IsGetValue)
                {
                    this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
                    DateShow = "Từ ngày " + FromDate.GetValueOrDefault().ToShortDateString() + " đến ngày " + ToDate.GetValueOrDefault().ToShortDateString();
                    var t = new Thread(() =>
                    {
                        try
                        {
                            using (var serviceFactory = new ReportServiceClient())
                            {
                                var contract = serviceFactory.ServiceInstance;
                                contract.BeginGetXRptInOutStockValueDrugDept_TV(
                                    ReportTitle, FromDate, ToDate, StorageName, StoreID
                                    , DateShow, V_MedProductType, RefGenDrugCatID_1, SelectedDrugDeptProductGroupReportType
                                    , Globals.ServerConfigSection.CommonItems.ReportHospitalName
                                    , Globals.ServerConfigSection.CommonItems.ReportLogoUrl
                                    , SelectedBid != null ? SelectedBid.BidID : 0
                                    , Globals.DispatchCallback((asyncResult) =>
                                    {
                                        try
                                        {
                                            var results = contract.EndGetXRptInOutStockValueDrugDept_TV(asyncResult);
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
                //▲====: #005
            }
        }

        private void PrintInOutStocksRpt(DateTime FromDate, DateTime ToDate, string StorageName, long StoreID, string dateshow)
        {
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ReportServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetInOutStocksInPdfFormat(FromDate, ToDate, StorageName, StoreID, dateshow, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndGetInOutStocksInPdfFormat(asyncResult);
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

        private string GetStorageName(object value)
        {
            RefStorageWarehouseLocation p = value as RefStorageWarehouseLocation;
            if (p != null)
                return p.swhlName;
            else
                return "";
        }

        private long StoreID = 0;
        private bool CheckData()
        {
            if (FromDate == null || ToDate == null)
            {
                MessageBox.Show(eHCMSResources.K0366_G1_ChonNgThCanXem);
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
            if (IsCheck)
            {
                if (Store == null && CanSelectedWareHouse)
                {
                    MessageBox.Show(eHCMSResources.K0310_G1_ChonKhoCanXem);
                    return false;
                }
                else
                {
                    StoreID = Store.StoreID;
                    StorageName = Store.swhlName;
                    DateShow = StorageName + "( " + FromDate.GetValueOrDefault().ToShortDateString() + " - " + ToDate.GetValueOrDefault().ToShortDateString() + " )";
                    return true;
                }
            }
            else
            {
                StoreID = 0;
                StorageName = eHCMSResources.Z0936_G1_TgKho;
                DateShow = StorageName + "( " + FromDate.GetValueOrDefault().ToShortDateString() + " - " + ToDate.GetValueOrDefault().ToShortDateString() + " )";
                return true;
            }
        }

        public void btn_Print(object sender, RoutedEventArgs e)
        {
            if (CheckData())
            {
                PrintInOutStocksRpt(FromDate.GetValueOrDefault(), ToDate.GetValueOrDefault(), StorageName, StoreID, DateShow);
            }
        }

        private Staff GetStaffLogin()
        {
            return Globals.LoggedUserAccount.Staff;
        }

        public void btnKetChuyenTonDauKy()
        {
            if (Store == null)
            {
                Globals.ShowMessage(string.Format("{0}!", eHCMSResources.Z0937_G1_ChonKhoKC), eHCMSResources.G0442_G1_TBao);
            }
            else
            {
                this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
                var t = new Thread(() =>
                {
                    try
                    {
                        using (var serviceFactory = new PharmacyMedDeptServiceClient())
                        {
                            var contract = serviceFactory.ServiceInstance;
                            contract.BeginKetChuyenTonKho_DrugDept(Store.StoreID, GetStaffLogin().StaffID, CheckPointName, V_MedProductType, Globals.DispatchCallback((asyncResult) =>
                            {
                                try
                                {
                                    contract.EndKetChuyenTonKho_DrugDept(asyncResult);
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

        public void RefMedProductType_DropDownClosed(object sender, EventArgs e)
        {
            if (V_MedProductType == (long)AllLookupValues.MedProductType.THUOC && _eItem != ReportName.BC_NXT_THUOC_TONGHOP_V2)
            {
                CanSelectedRefGenDrugCatID_1 = true;
                IsShowGroupReportType = false;
            }
            /*▼====: #001*/
            else if (V_MedProductType == (long)AllLookupValues.MedProductType.Y_CU && _eItem != ReportName.BC_NXT_THUOC_TONGHOP_V2)
            {
                CanSelectedRefGenDrugCatID_1 = false;
                if (DrugDeptProductGroupReportTypeCollection == null || DrugDeptProductGroupReportTypeCollection.Count == 0)
                {
                    LoadDrugDeptProductGroupReportTypes();
                }
                else
                    IsShowGroupReportType = true;
            }
            /*▲====: #001*/
            else
            {
                CanSelectedRefGenDrugCatID_1 = false;
                IsShowGroupReportType = false;
            }
        }

        /*▼====: #001*/
        private bool _IsShowGroupReportType = false;
        public bool IsShowGroupReportType
        {
            get
            {
                return _IsShowGroupReportType;
            }
            set
            {
                if (_IsShowGroupReportType != value)
                {
                    _IsShowGroupReportType = value;
                    NotifyOfPropertyChange(() => IsShowGroupReportType);
                }
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
        /*▲====: #001*/

        private bool _CanSelectedWareHouse = true;
        public bool CanSelectedWareHouse
        {
            get
            {
                return _CanSelectedWareHouse;
            }
            set
            {
                if (_CanSelectedWareHouse != value)
                {
                    _CanSelectedWareHouse = value;
                    NotifyOfPropertyChange(() => CanSelectedWareHouse);
                }
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

        private ObservableCollection<Bid> _gBidCollection;
        public ObservableCollection<Bid> gBidCollection
        {
            get
            {
                return _gBidCollection;
            }
            set
            {
                if (_gBidCollection != value)
                {
                    _gBidCollection = value;
                    NotifyOfPropertyChange(() => gBidCollection);
                }
            }
        }
        private void LoadBids()
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                using (var mServiceFactory = new PharmacyGenMedDeptDetailsServiceClient())
                {
                    var contract = mServiceFactory.ServiceInstance;
                    try
                    {
                        contract.BeginGetAllBids(V_MedProductType, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                gBidCollection = new ObservableCollection<Bid>();
                                var ItemCollection = contract.EndGetAllBids(asyncResult);
                                if (ItemCollection != null)
                                {
                                    gBidCollection = new ObservableCollection<Bid>(ItemCollection);
                                }
                                gBidCollection.Insert(0, new Bid { BidID = 0, BidName = eHCMSResources.A0015_G1_Chon });
                                SelectedBid = gBidCollection.FirstOrDefault();
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
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                        this.HideBusyIndicator();
                    }
                }
            });
            t.Start();
        }
        public void btnGetBidInfo()
        {
            LoadBids();
        }
    }
}