using eHCMSLanguage;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using aEMR.CommonTasks;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.Infrastructure.CachingUtils;
using aEMR.ViewContracts;
using Castle.Windsor;
using Castle.Core.Logging;
using Caliburn.Micro;
using System.Collections.ObjectModel;
using DataEntities;
using System.Linq;
using aEMR.Common.Collections;
/*
* 20211103 #001 QTD:   Lọc kho theo cấu hình trách nhiệm
*/

namespace aEMR.Common.ViewModels
{
    [Export(typeof(ICriteriaB)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class CriteriaBViewModel : Conductor<object>, ICriteriaB
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;
        [ImportingConstructor]
        public CriteriaBViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {

            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            Init();
            Store = new RefStorageWarehouseLocation();
            StoreClinic = new RefStorageWarehouseLocation();
        }

        private RefStorageWarehouseLocation _StoreClinic;
        public RefStorageWarehouseLocation StoreClinic
        {
            get { return _StoreClinic; }
            set
            {
                _StoreClinic = value;
                NotifyOfPropertyChange(() => StoreClinic);
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

        private bool? _IsShowHave=true;
        public bool? IsShowHave
        {
            get
            {
                return _IsShowHave;
            }
            set
            {
                if (_IsShowHave != value)
                {
                    _IsShowHave = value;
                    NotifyOfPropertyChange(() => IsShowHave);
                }
            }
        }

        private bool? _IsShowHaveMedProduct=true;
        public bool? IsShowHaveMedProduct
        {
            get
            {
                return _IsShowHaveMedProduct;
            }
            set
            {
                if (_IsShowHaveMedProduct != value)
                {
                    _IsShowHaveMedProduct = value;
                    NotifyOfPropertyChange(() => IsShowHaveMedProduct);
                }
            }
        }

        private string _HienThi1;
        public string HienThi1
        {
            get
            {
                return _HienThi1;
            }
            set
            {
                _HienThi1 = value;
                NotifyOfPropertyChange(() => HienThi1);
            }
        }

        private string _HienThi2;
        public string HienThi2
        {
            get
            {
                return _HienThi2;
            }
            set
            {
                _HienThi2 = value;
                NotifyOfPropertyChange(() => HienThi2);
            }
        }

        private ObservableCollection<RefStorageWarehouseLocation> _StoreClinics;
        public ObservableCollection<RefStorageWarehouseLocation> StoreClinics
        {
            get
            {
                return _StoreClinics;
            }
            set
            {
                if (_StoreClinics != value)
                {
                    _StoreClinics = value;
                    NotifyOfPropertyChange(() => StoreClinics);
                }
            }
        }
        public void GetStore()
        {
            Coroutine.BeginExecute(DoGetStore_DrugDept());
            Coroutine.BeginExecute(DoGetStore_ClinicDept());
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
                _V_MedProductType = value;
                NotifyOfPropertyChange(() => V_MedProductType);
            }
        }
        private IEnumerator<IResult> DoGetStore_DrugDept()
        {
            var paymentTypeTask = new LoadStoreListTask((long)AllLookupValues.StoreType.STORAGE_DRUGDEPT, false, null, false, false);
            yield return paymentTypeTask;
            //StoreCbx = paymentTypeTask.LookupList.Where(x => (V_MedProductType != 0 && x.ListV_MedProductType != null && x.ListV_MedProductType.Contains(V_MedProductType.ToString()))).ToObservableCollection();
            //▼===== #001
            var StoreTemp = paymentTypeTask.LookupList.Where(x => (V_MedProductType != 0 && x.ListV_MedProductType != null && x.ListV_MedProductType.Contains(V_MedProductType.ToString()))).ToObservableCollection();
            StoreCbx = Globals.CheckDrugMedStoreWareHouse(StoreTemp);
            if(StoreCbx == null || StoreCbx.Count == 0)
            {
                MessageBox.Show(eHCMSResources.A0110_G1_Msg_InfoChuaCauHinhTNKho.Replace("\\n ", "\n"));
            }
            //▲===== #001
            yield break;
        }

        private IEnumerator<IResult> DoGetStore_ClinicDept()
        {
            var paymentTypeTask = new LoadStoreListTask((long)AllLookupValues.StoreType.STORAGE_CLINIC, false, null, false, true);
            yield return paymentTypeTask;
            StoreClinics = paymentTypeTask.LookupList.Where(x => (V_MedProductType != 0 && x.ListV_MedProductType != null && x.ListV_MedProductType.Contains(V_MedProductType.ToString()))).ToObservableCollection();
            yield break;
        }
        public string TextbtViewPrint { get; set; }

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

        private void LoadListXemTheo()
        {
            ClsXemTheo Ngay = new ClsXemTheo();
            Ngay.mValue = 1;
            Ngay.mText = eHCMSResources.N0045_G1_Ng;
            ObjListXemTheo.Add(Ngay);

            ClsXemTheo Thang = new ClsXemTheo();
            Thang.mValue = 2;
            Thang.mText = eHCMSResources.Z1027_G1_ThNam;
            ObjListXemTheo.Add(Thang);

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
            //Default Item
            Thang = Globals.ServerDate.Value.Month;
            //Default Item
        }

        private void LoadListYear()
        {
            int year = Globals.ServerDate.Value.Year;
            for (int i = year; i > year - 3; i--)
            {
                ClsYear ObjY = new ClsYear();
                ObjY.mValue = i;
                ObjY.mText = i.ToString();
                ObjListNamThang.Add(ObjY);
            }
            //Default Item
            NamThang = year;
            //Default Item
        }
        #endregion

        public void Init()
        {
            ObjListXemTheo = new ObservableCollection<ClsXemTheo>();
            ObjListMonth = new ObservableCollection<ClsMonth>();
            ObjListNamThang = new ObservableCollection<ClsYear>();

            XemTheo = 1;
            Thang = -1;
            NamThang = -1;

            CtrTheoNgayVisibility = true;
            CtrTheoThangVisibility = false;

            LoadListXemTheo();
            LoadListMonth();
            LoadListYear();
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


        private DateTime? _FromDate = Globals.ServerDate.Value;
        public DateTime? FromDate
        {
            get { return _FromDate; }
            set
            {
                _FromDate = value;
                NotifyOfPropertyChange(() => FromDate);
            }
        }

        private DateTime? _ToDate = Globals.ServerDate.Value;
        public DateTime? ToDate
        {
            get { return _ToDate; }
            set
            {
                _ToDate = value;
                NotifyOfPropertyChange(() => ToDate);
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


        #region Enable Ctr
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

        #endregion

        #region Xem Theo
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
                        CtrTheoNgayVisibility = true;
                        CtrTheoThangVisibility = false;
                        break;
                    }
                case 2:
                    {
                        CtrTheoNgayVisibility = false;
                        CtrTheoThangVisibility = true;
                        break;
                    }
            }
        }
        #endregion

        public void btViewPrint()
        {
            switch (XemTheo)
            {
                case 1:
                    {
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

                        if (Store == null || Store.StoreID <= 0)
                        {
                            MessageBox.Show(eHCMSResources.K1973_G1_ChonKho);
                            return;
                        }

                        Globals.EventAggregator.Publish(new SelectedObjectWithKey<Object, Object, Object, Object, Object, Object, Object, Object, Object, Object>() { ObjA = FromDate, ObjB = ToDate, ObjC = null, ObjD = null, ObjE = null, ObjF = Store, ObjG = StoreClinic, ObjH = IsShowHave, ObjK = IsShowHaveMedProduct, ObjKey = XemTheo });
                        break;
                    }
                case 2:
                    {
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

                        if (Store == null || Store.StoreID <= 0)
                        {
                            MessageBox.Show(eHCMSResources.K1973_G1_ChonKho);
                            return;
                        }

                        Globals.EventAggregator.Publish(new SelectedObjectWithKey<Object, Object, Object, Object, Object, Object, Object, Object, Object, Object>() { ObjA = null, ObjB = null, ObjC = null, ObjD = Thang, ObjE = NamThang, ObjF = Store, ObjG = StoreClinic, ObjH = IsShowHave, ObjK = IsShowHaveMedProduct, ObjKey = XemTheo });
                        break;
                    }

            }

        }


    }
}
