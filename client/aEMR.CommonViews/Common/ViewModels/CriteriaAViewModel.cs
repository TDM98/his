using eHCMSLanguage;
using System;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ViewContracts;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using Caliburn.Micro;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using aEMR.CommonTasks;
using DataEntities;
using System.Linq;
using aEMR.Common.Collections;
/*
* 20211103 #001 QTD:   Lọc kho theo cấu hình trách nhiệm
*/

namespace aEMR.Common.ViewModels
{
    [Export(typeof(ICriteriaA)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class CriteriaAViewModel : Conductor<object>, ICriteriaA
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;
        [ImportingConstructor]

        public CriteriaAViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            Init();
        }
        #region Loại xem Rpt, Tháng, Năm

        public class ClsXemTheo
        {
            private int _mValue;
            private string _mText;

            public int mValue
            {
                get { return _mValue; }
                set
                {
                    _mValue = value;
                }
            }

            public string mText
            {
                get { return _mText; }
                set
                {
                    _mText = value;
                }
            }

        }

        public class ClsQuy
        {
            private int _mValue;
            private string _mText;

            public int mValue
            {
                get { return _mValue; }
                set
                {
                    _mValue = value;
                }
            }

            public string mText
            {
                get { return _mText; }
                set
                {
                    _mText = value;
                }
            }

        }

        public class ClsMonth
        {
            private int _mValue;
            private string _mText;

            public int mValue
            {
                get { return _mValue; }
                set
                {
                    _mValue = value;
                }
            }

            public string mText
            {
                get { return _mText; }
                set
                {
                    _mText = value;
                }
            }

        }

        public class ClsYear
        {
            private int _mValue;
            private string _mText;

            public int mValue
            {
                get { return _mValue; }
                set
                {
                    _mValue = value;
                }
            }

            public string mText
            {
                get { return _mText; }
                set
                {
                    _mText = value;
                }
            }

        }

        private ObservableCollection<ClsXemTheo> _ObjListXemTheo;
        public ObservableCollection<ClsXemTheo> ObjListXemTheo
        {
            get
            {
                return _ObjListXemTheo;
            }
            set
            {
                _ObjListXemTheo = value;
                NotifyOfPropertyChange(() => ObjListXemTheo);
            }
        }

        private ObservableCollection<ClsQuy> _ObjListQuy;
        public ObservableCollection<ClsQuy> ObjListQuy
        {
            get
            {
                return _ObjListQuy;
            }
            set
            {
                _ObjListQuy = value;
                NotifyOfPropertyChange(() => ObjListQuy);
            }
        }

        private ObservableCollection<ClsMonth> _ObjListMonth;
        public ObservableCollection<ClsMonth> ObjListMonth
        {
            get
            {
                return _ObjListMonth;
            }
            set
            {
                _ObjListMonth = value;
                NotifyOfPropertyChange(() => ObjListMonth);
            }
        }

        private ObservableCollection<ClsYear> _ObjListNamThang;
        public ObservableCollection<ClsYear> ObjListNamThang
        {
            get
            {
                return _ObjListNamThang;
            }
            set
            {
                _ObjListNamThang = value;
                NotifyOfPropertyChange(() => ObjListNamThang);
            }
        }

        private ObservableCollection<ClsYear> _ObjListNamQuy;
        public ObservableCollection<ClsYear> ObjListNamQuy
        {
            get
            {
                return _ObjListNamQuy;
            }
            set
            {
                _ObjListNamQuy = value;
                NotifyOfPropertyChange(() => ObjListNamQuy);
            }
        }

        private bool _isStoreDept;
        public bool isStoreDept
        {
            get { return _isStoreDept; }
            set
            {
                if (_isStoreDept != value)
                {
                    _isStoreDept = value;
                    NotifyOfPropertyChange(() => isStoreDept);
                }
            }
        }

        private void LoadListXemTheo()
        {
            ClsXemTheo Ngay = new ClsXemTheo();
            Ngay.mValue = 1;
            Ngay.mText = eHCMSResources.N0045_G1_Ng;
            ObjListXemTheo.Add(Ngay);

            ClsXemTheo Quy = new ClsXemTheo();
            Quy.mValue = 2;
            Quy.mText = eHCMSResources.Q0495_G1_QuyNam;
            ObjListXemTheo.Add(Quy);

            ClsXemTheo Thang = new ClsXemTheo();
            Thang.mValue = 3;
            Thang.mText = eHCMSResources.Z1027_G1_ThNam;
            ObjListXemTheo.Add(Thang);

        }


        private void LoadListQuy()
        {
            for (int i = 1; i <= 4; i++)
            {
                ClsQuy ObjM = new ClsQuy();
                ObjM.mValue = i;
                ObjM.mText = string.Format("{0} ", eHCMSResources.Q0486_G1_Quy) + i.ToString();
                ObjListQuy.Add(ObjM);
            }
            //Default Item
            //ClsQuy DefaultItem = new ClsQuy();
            //DefaultItem.mValue = -1;
            //DefaultItem.mText = eHCMSResources.A0015_G1_Chon;
            //ObjListQuy.Insert(0, DefaultItem);
            //Default Item
            int Month = Globals.ServerDate.Value.Month;
            if (Month <= 3)
            {
                // 1st Quarter = January 1 to March 31
                Quy = 1;
            }
            else if ((Month >= 4) && (Month <= 6))
            {
                // 1st Quarter = January 1 to March 31
                Quy = 2;
            }
            else if ((Month >= 7) && (Month <= 9))
            {
                // 1st Quarter = January 1 to March 31
                Quy = 3;
            }
            else // 4th Quarter = October 1 to December 31
            {
                // 1st Quarter = January 1 to March 31
                Quy = 4;
            }
        }

        private void LoadListMonth()
        {
            for (int i = 1; i <= 12; i++)
            {
                ClsMonth ObjM = new ClsMonth();
                ObjM.mValue = i;
                ObjM.mText = i.ToString();
                ObjListMonth.Add(ObjM);
            }
            Thang = Globals.ServerDate.Value.Month;
            ////Default Item
            //ClsMonth DefaultItem = new ClsMonth();
            //DefaultItem.mValue = -1;
            //DefaultItem.mText = eHCMSResources.A0015_G1_Chon;
            //ObjListMonth.Insert(0, DefaultItem);
            ////Default Item
        }

        private void LoadListYear()
        {
            int curYear = Globals.ServerDate.Value.Year;
            for (int i = curYear - 3; i <= curYear; i++)
            {
                ClsYear ObjY = new ClsYear();
                ObjY.mValue = i;
                ObjY.mText = i.ToString();
                ObjListNamThang.Add(ObjY);
                ObjListNamQuy.Add(ObjY);
            }



            ////Default Item
            //ClsYear DefaultItem = new ClsYear();
            //DefaultItem.mValue = -1;
            //DefaultItem.mText = eHCMSResources.A0015_G1_Chon;
            //ObjListNamThang.Insert(0, DefaultItem);
            //ObjListNamQuy.Insert(0, DefaultItem);
            ////Default Item

            NamQuy = curYear;
            NamThang = curYear;
        }
        #endregion
        public void Init()
        {
            ObjListXemTheo = new ObservableCollection<ClsXemTheo>();
            ObjListQuy = new ObservableCollection<ClsQuy>();
            ObjListMonth = new ObservableCollection<ClsMonth>();
            ObjListNamThang = new ObservableCollection<ClsYear>();
            ObjListNamQuy = new ObservableCollection<ClsYear>();

            XemTheo = 1;

            CtrTheoNgayVisibility = true;
            CtrTheoThangVisibility = false;
            CtrTheoQuyVisibility = false;

            FromDate = Globals.ServerDate.Value;
            ToDate = Globals.ServerDate.Value;

            LoadListXemTheo();
            LoadListQuy();
            LoadListMonth();
            LoadListYear();

            V_RefOutputTypeCollection = Globals.RefOutputType.Where(x => x.IsSelected == true).ToObservableCollection();
            V_RefOutputTypeCollection.Insert(0, new RefOutputType { IsSelected = true, TypID = 0, TypName = eHCMSResources.A0015_G1_Chon });

            V_MedProductTypeCollection = Globals.AllLookupValueList.Where(x => x.ObjectTypeID == (long)LookupValues.V_MedProductType).ToObservableCollection();

            if (Globals.allRefStorageWarehouseLocation == null || Globals.allRefStorageWarehouseLocation.Count == 0)
            {
                Coroutine.BeginExecute(DoGetStoreDeptAll());
            }
        }

        private Int32 _XemTheo;
        public Int32 XemTheo
        {
            get { return _XemTheo; }
            set
            {
                _XemTheo = value;
                NotifyOfPropertyChange(() => XemTheo);
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

        private Int32 _Quy;
        public Int32 Quy
        {
            get { return _Quy; }
            set
            {
                _Quy = value;
                NotifyOfPropertyChange(() => Quy);
            }
        }

        private Int32 _Thang;
        public Int32 Thang
        {
            get { return _Thang; }
            set
            {
                _Thang = value;
                NotifyOfPropertyChange(() => Thang);
            }
        }


        private Int32 _NamQuy;
        public Int32 NamQuy
        {
            get { return _NamQuy; }
            set
            {
                _NamQuy = value;
                NotifyOfPropertyChange(() => NamQuy);
            }
        }

        private Int32 _NamThang;
        public Int32 NamThang
        {
            get { return _NamThang; }
            set
            {
                _NamThang = value;
                NotifyOfPropertyChange(() => NamThang);
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
                if (_VisibilityOutputType == value) return;
                _VisibilityOutputType = value;
                NotifyOfPropertyChange(() => VisibilityOutputType);
            }
        }

        private ObservableCollection<RefOutputType> _V_RefOutputTypeCollection;
        public ObservableCollection<RefOutputType> V_RefOutputTypeCollection
        {
            get
            {
                return _V_RefOutputTypeCollection;
            }
            set
            {
                _V_RefOutputTypeCollection = value;
                NotifyOfPropertyChange("V_RefOutputTypeCollection");
            }
        }

        private long _gSelectedTypID;
        public long gSelectedTypID
        {
            get
            {
                return _gSelectedTypID;
            }
            set
            {
                _gSelectedTypID = value;
                NotifyOfPropertyChange(() => gSelectedTypID);
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

        private bool _GetStoreFollowV_MedProductType;
        public bool GetStoreFollowV_MedProductType
        {
            get
            {
                return _GetStoreFollowV_MedProductType;
            }
            set
            {
                if(_GetStoreFollowV_MedProductType != value)
                {
                    _GetStoreFollowV_MedProductType = value;
                    NotifyOfPropertyChange(() => GetStoreFollowV_MedProductType);
                }
            }
        }

        #region Enable Ctr
        /*TMA 24/10/2017*/
        private Visibility _VisibilityExcel = Visibility.Collapsed;
        public Visibility VisibilityExcel
        {
            get { return _VisibilityExcel; }
            set
            {
                _VisibilityExcel = value;
                NotifyOfPropertyChange(() => VisibilityExcel);
            }
        }
        /*TMA 24/10/2017*/
        private Visibility _VisibilityStore=Visibility.Collapsed;
        public Visibility VisibilityStore
        {
            get { return _VisibilityStore; }
            set
            {
                _VisibilityStore = value;
                NotifyOfPropertyChange(() => VisibilityStore);
            }
        }

        private bool _CtrTheoNgayVisibility;
        public bool CtrTheoNgayVisibility
        {
            get { return _CtrTheoNgayVisibility; }
            set
            {
                _CtrTheoNgayVisibility = value;
                NotifyOfPropertyChange(() => CtrTheoNgayVisibility);
            }
        }

        private bool _CtrTheoThangVisibility;
        public bool CtrTheoThangVisibility
        {
            get { return _CtrTheoThangVisibility; }
            set
            {
                _CtrTheoThangVisibility = value;
                NotifyOfPropertyChange(() => CtrTheoThangVisibility);
            }
        }


        private bool _CtrTheoQuyVisibility;
        public bool CtrTheoQuyVisibility
        {
            get { return _CtrTheoQuyVisibility; }
            set
            {
                _CtrTheoQuyVisibility = value;
                NotifyOfPropertyChange(() => CtrTheoQuyVisibility);
            }
        }

        private ObservableCollection<Lookup> _V_MedProductTypeCollection;
        public ObservableCollection<Lookup> V_MedProductTypeCollection
        {
            get => _V_MedProductTypeCollection; set
            {
                _V_MedProductTypeCollection = value;
                NotifyOfPropertyChange(() => V_MedProductTypeCollection);
            }
        }

        private bool _MedProductTypeVisible = false;
        public bool MedProductTypeVisible
        {
            get => _MedProductTypeVisible; set
            {
                _MedProductTypeVisible = value;
                NotifyOfPropertyChange(() => MedProductTypeVisible);
            }
        }

        private bool _IsDrugDeptExportDetail = false;
        public bool IsDrugDeptExportDetail
        {
            get => _IsDrugDeptExportDetail; set
            {
                _IsDrugDeptExportDetail = value;
                NotifyOfPropertyChange(() => IsDrugDeptExportDetail);
            }
        }

        private ObservableCollection<RefStorageWarehouseLocation> _WarehouseCollection;
        public ObservableCollection<RefStorageWarehouseLocation> WarehouseCollection
        {
            get => _WarehouseCollection; set
            {
                _WarehouseCollection = value;
                NotifyOfPropertyChange(() => WarehouseCollection);
            }
        }

        private RefStorageWarehouseLocation _OutStore;
        public RefStorageWarehouseLocation OutStore
        {
            get => _OutStore; set
            {
                _OutStore = value;
                NotifyOfPropertyChange(() => OutStore);
            }
        }

        private bool _IsViewByVisible = true;
        public bool IsViewByVisible
        {
            get => _IsViewByVisible; set
            {
                _IsViewByVisible = value;
                NotifyOfPropertyChange(() => IsViewByVisible);
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
        #endregion

        #region Xem Theo

        public void GetListStore(long StoreType)
        {
            if (isStoreDept)
            {
                StoreCbx = Globals.checkStoreWareHouse(V_MedProductType, false, true);
                if (StoreCbx == null || StoreCbx.Count < 1)
                {
                    MessageBox.Show(eHCMSResources.A0110_G1_Msg_InfoChuaCauHinhTNKho);
                }
            }
            else
            {
                Coroutine.BeginExecute(DoGetStore_EXTERNAL(StoreType));
            }
            //
        }
 
        private IEnumerator<IResult> DoGetStore_EXTERNAL(long StoreType)
        {
            var paymentTypeTask = new LoadStoreListTask(StoreType, false, null,false, true);
            yield return paymentTypeTask;
            //if (GetStoreFollowV_MedProductType)
            //{
            //    StoreCbx = paymentTypeTask.LookupList.Where(x => (V_MedProductType != 0 && x.ListV_MedProductType != null && x.ListV_MedProductType.Contains(V_MedProductType.ToString()))).ToObservableCollection();
            //}
            //else
            //{
            //    StoreCbx = paymentTypeTask.LookupList;
            //}
            //InitForIsDrugDeptExportDetail();
            //▼===== #001
            var StoreTemp = new ObservableCollection<RefStorageWarehouseLocation>();
            if (GetStoreFollowV_MedProductType)
            {
                StoreTemp = paymentTypeTask.LookupList.Where(x => (V_MedProductType != 0 && x.ListV_MedProductType != null && x.ListV_MedProductType.Contains(V_MedProductType.ToString()))).ToObservableCollection();
                StoreCbx = Globals.CheckDrugMedStoreWareHouse(StoreTemp);
            }
            else
            {
                StoreTemp = paymentTypeTask.LookupList;
            }
            if (StoreCbx != null && StoreCbx.Count > 0)
            {
                InitForIsDrugDeptExportDetail();
            }
            else
            {
                MessageBox.Show(eHCMSResources.A0110_G1_Msg_InfoChuaCauHinhTNKho.Replace("\\n ", "\n"));
            }
            //▲===== #001
            yield break;
        }
        private IEnumerator<IResult> DoGetStoreDeptAll()
        {
            var paymentTypeTask = new LoadStoreListTask(null, false, null, false, false);
            yield return paymentTypeTask;
            Globals.allRefStorageWarehouseLocation = paymentTypeTask.LookupList;
            yield break;
        }
        public void cboXemTheo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SetVisibleControl(XemTheo);
        }

        private void SetVisibleControl(Int32 V)
        {
            switch (V)
            {
                case 1:
                    {
                        CtrTheoNgayVisibility=true;
                        CtrTheoThangVisibility=false;
                        CtrTheoQuyVisibility=false;
                        break;
                    }
                    case 2:
                    {
                        CtrTheoNgayVisibility=false;
                        CtrTheoQuyVisibility=true;
                        CtrTheoThangVisibility=false;
                        break;
                    }
                    case 3:
                    {
                        CtrTheoNgayVisibility=false;
                        CtrTheoQuyVisibility=false;
                        CtrTheoThangVisibility=true;
                        break;
                    }
            }
        }
        #endregion

        public void btViewPrint()
        {
            if (IsDrugDeptExportDetail || gReportName != null)
            {
                Globals.EventAggregator.Publish(new PrintEventActionView());
                return;
            }
            switch (XemTheo)
            {
                case 1:
                    if (FromDate == null)
                    {
                        MessageBox.Show(eHCMSResources.A0372_G1_Msg_InfoChonTuNg);
                        return;
                    }
                    if (ToDate == null)
                    {
                        MessageBox.Show(eHCMSResources.A0308_G1_Msg_InfoChonDenNg);
                        return;
                    }
                    if (FromDate > ToDate)
                    {
                        MessageBox.Show(eHCMSResources.Z0467_G1_TuNgNhoHonBangDenNg);
                        return;
                    }
                    Globals.EventAggregator.Publish(new SelectedObjectWithKey<Object, Object, Object, Object, Object, Object, Object, Object>() { ObjA = FromDate, ObjB = ToDate, ObjC = null, ObjD = null, ObjE = null, ObjF = CurStore, ObjKey = XemTheo, ObjG = gSelectedTypID });
                    break;
                case 2:
                    if (Quy <= 0)
                    {
                        MessageBox.Show(eHCMSResources.A0358_G1_Msg_InfoChonQuy);
                        return;
                    }
                    if (NamQuy <= 0)
                    {
                        MessageBox.Show(eHCMSResources.A0340_G1_Msg_InfoChonNam);
                        return;
                    }
                    Globals.EventAggregator.Publish(new SelectedObjectWithKey<Object, Object, Object, Object, Object, Object, Object, Object>() { ObjA = null, ObjB = null, ObjC = Quy, ObjD = null, ObjE = NamQuy, ObjF = CurStore, ObjKey = XemTheo, ObjG = gSelectedTypID });
                    break;
                case 3:
                    if (Thang <= 0)
                    {
                        MessageBox.Show(eHCMSResources.A0361_G1_Msg_InfoChonThang);
                        return;
                    }
                    if (NamThang <= 0)
                    {
                        MessageBox.Show(eHCMSResources.A0340_G1_Msg_InfoChonNam);
                        return;
                    }
                    Globals.EventAggregator.Publish(new SelectedObjectWithKey<Object, Object, Object, Object, Object, Object, Object, Object>() { ObjA = null, ObjB = null, ObjC = null, ObjD = Thang, ObjE = NamThang, ObjF = CurStore, ObjKey = XemTheo, ObjG = gSelectedTypID });
                    break;
            }
        }
        /*TMA 24/10/2017   - XUẤT EXCEL*/
        public void btExportExcel()
        {
            switch (XemTheo)
            {
                case 1:
                    if (FromDate == null)
                    {
                        MessageBox.Show(eHCMSResources.A0372_G1_Msg_InfoChonTuNg);
                        return;
                    }
                    if (ToDate == null)
                    {
                        MessageBox.Show(eHCMSResources.A0308_G1_Msg_InfoChonDenNg);
                        return;
                    }
                    if (FromDate > ToDate)
                    {
                        MessageBox.Show(eHCMSResources.Z0467_G1_TuNgNhoHonBangDenNg);
                        return;
                    }
                    Globals.EventAggregator.Publish(new SelectedObjectWithKeyExcel<Object, Object, Object, Object, Object, Object, Object, Object>() { ObjA = FromDate, ObjB = ToDate, ObjC = null, ObjD = null, ObjE = null, ObjF = CurStore, ObjKey = XemTheo, ObjG = gSelectedTypID });
                    break;
                case 2:
                    if (Quy <= 0)
                    {
                        MessageBox.Show(eHCMSResources.A0358_G1_Msg_InfoChonQuy);
                        return;
                    }
                    if (NamQuy <= 0)
                    {
                        MessageBox.Show(eHCMSResources.A0340_G1_Msg_InfoChonNam);
                        return;
                    }
                    Globals.EventAggregator.Publish(new SelectedObjectWithKeyExcel<Object, Object, Object, Object, Object, Object, Object, Object>() { ObjA = null, ObjB = null, ObjC = Quy, ObjD = null, ObjE = NamQuy, ObjF = CurStore, ObjKey = XemTheo, ObjG = gSelectedTypID });
                    break;
                case 3:
                    if (Thang <= 0)
                    {
                        MessageBox.Show(eHCMSResources.A0361_G1_Msg_InfoChonThang);
                        return;
                    }
                    if (NamThang <= 0)
                    {
                        MessageBox.Show(eHCMSResources.A0340_G1_Msg_InfoChonNam);
                        return;
                    }
                    Globals.EventAggregator.Publish(new SelectedObjectWithKeyExcel<Object, Object, Object, Object, Object, Object, Object, Object>() { ObjA = null, ObjB = null, ObjC = null, ObjD = Thang, ObjE = NamThang, ObjF = CurStore, ObjKey = XemTheo, ObjG = gSelectedTypID });
                    break;

            }
        }

        public void InitForIsDrugDeptExportDetail()
        {
            if (IsDrugDeptExportDetail && StoreCbx != null)
            {
                StoreCbx = StoreCbx.Where(x => x.StoreID == 1).ToObservableCollection();
                foreach(var item in GetStoresForDrugDeptExportDetail())
                {
                    StoreCbx.Add(item);
                }
                CurStore = StoreCbx.FirstOrDefault();
                cboStoreCbx_SelectionChanged(null, null);
            }
        }
        private ObservableCollection<RefStorageWarehouseLocation> GetStoresForDrugDeptExportDetail()
        {
            return new ObservableCollection<RefStorageWarehouseLocation> {
                new RefStorageWarehouseLocation { swhlName = "Kho lẻ thuốc nội trú", StoreID = (long)AllLookupValues.MedProductType.THUOC },
                new RefStorageWarehouseLocation { swhlName = "Kho lẻ vật tư y tế", StoreID = (long)AllLookupValues.MedProductType.Y_CU },
                new RefStorageWarehouseLocation { swhlName = "Kho lẻ thuốc BHYT ngoại trú", StoreID = 160 }
               };
        }
        public void cboStoreCbx_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CurStore != null && CurStore.StoreID == 1)
            {
                WarehouseCollection = GetStoresForDrugDeptExportDetail();
                if (WarehouseCollection != null)
                {
                    OutStore = WarehouseCollection.FirstOrDefault();
                }
            }
            else if (CurStore != null && CurStore.StoreID != 160)
            {
                WarehouseCollection = Globals.checkStoreWareHouse(V_MedProductType, false, false);
                if (WarehouseCollection != null)
                {
                    OutStore = WarehouseCollection.FirstOrDefault();
                }
            }
        }

        private Nullable<ReportName> _ReportName;
        public Nullable<ReportName> gReportName
        {
            get => _ReportName; set
            {
                _ReportName = value;
                NotifyOfPropertyChange(() => gReportName);
            }
        }
    }
}