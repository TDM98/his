using System;
using System.ComponentModel.Composition;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using Caliburn.Micro;
using System.Collections.ObjectModel;
using eHCMSLanguage;
using aEMR.Infrastructure.Events;
using aEMR.ViewContracts;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using aEMR.Common.Collections;
using aEMR.Infrastructure;
using aEMR.ViewContracts.Configuration;
using aEMR.ServiceClient;
/*
 * 20181007 #001 TNHX: [BM0000104] Add busyindicator and refactor code (This class wasn't called)
 */
namespace aEMR.Common.ViewModels
{
    [Export(typeof(ISellingPriceList)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class SellingPriceListViewModel : Conductor<object>, ISellingPriceList
          , IHandle<SaveEvent<bool>>
    {
        public bool IsCheck { get; set; }
     
        private CheckPriceList checkPriceList = null;

        [ImportingConstructor]
        public SellingPriceListViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg, ISalePosCaching salePosCaching)
        {
            eventArg.Subscribe(this);
            authorization();
          
            ObjListMonth=new ObservableCollection<ClsMonth>();
            ObjListYear =new ObservableCollection<ClsYear>();

            LoadListMonth();
            LoadListYear();

            //SearchCriteria = new T3();
            //SearchCriteria.Month = -1;
            //SearchCriteria.Year = DateTime.Now.Year;

            //PriceList_GetList_Paging = new PagedSortableCollectionView<T1>();
            //PriceList_GetList_Paging.OnRefresh += new EventHandler<RefreshEventArgs>(PriceList_GetList_Paging_OnRefresh);

            //PriceList_GetList_Paging.PageIndex = 0;
            //SellingPriceList_GetList_Paging(0, PriceList_GetList_Paging.PageSize, true);
        }

        void PriceList_GetList_Paging_OnRefresh(object sender, RefreshEventArgs e)
        {
            //SellingPriceList_GetList_Paging(PriceList_GetList_Paging.PageIndex, PriceList_GetList_Paging.PageSize, false);
        }

        private long _V_MedProductType;
        public long V_MedProductType
        {
            get { return _V_MedProductType; }
            set
            {
                if (_V_MedProductType != value)
                {
                    _V_MedProductType = value;
                    NotifyOfPropertyChange(() => V_MedProductType);
                }
            }
        }

        private string _TitleForm;
        public string TitleForm
        {
            get { return _TitleForm; }
            set
            {
                if (_TitleForm != value)
                {
                    _TitleForm = value;
                    NotifyOfPropertyChange(() => TitleForm);
                }
            }
        }

        public void Init() {}

        private DateTime _EffectiveDay;
        public DateTime EffectiveDay
        {
            get { return _EffectiveDay; }
            set
            {
                if (_EffectiveDay != value)
                {
                    _EffectiveDay = value;
                    NotifyOfPropertyChange(() => EffectiveDay);
                }
            }
        }

        private DateTime _curDate;
        public DateTime curDate
        {
            get { return _curDate; }
            set
            {
                if (_curDate != value)
                {
                    _curDate = value;
                    NotifyOfPropertyChange(() => curDate);
                }
            }
        }

        #region Tháng, Năm
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

        private ObservableCollection<ClsYear> _ObjListYear;
        public ObservableCollection<ClsYear> ObjListYear
        {
            get
            {
                return _ObjListYear;
            }
            set
            {
                _ObjListYear = value;
                NotifyOfPropertyChange(() => ObjListYear);
            }
        }

        private void LoadListMonth()
        {
            for (int i = 1; i <= 12; i++)
            {
                ClsMonth ObjM=new ClsMonth();
                ObjM.mValue = i;
                ObjM.mText = i.ToString();
                ObjListMonth.Add(ObjM);
            }
            //Default Item
            ClsMonth DefaultItem = new ClsMonth();
            DefaultItem.mValue = -1;
            DefaultItem.mText = eHCMSResources.A0015_G1_Chon;
            ObjListMonth.Insert(0,DefaultItem);
            //Default Item
        }

        private void LoadListYear()
        {
            for (int i = 2010; i <= 2099; i++)
            {
                ClsYear ObjY = new ClsYear();
                ObjY.mValue = i;
                ObjY.mText = i.ToString();
                ObjListYear.Add(ObjY);
            }
            //Default Item
            ClsYear DefaultItem = new ClsYear();
            DefaultItem.mValue = -1;
            DefaultItem.mText = eHCMSResources.A0015_G1_Chon;
            ObjListYear.Insert(0, DefaultItem);
            //Default Item
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
        private bool _mXem = true;
        private bool _mChinhSua = true;
        private bool _mTaoBangGia = true;
        private bool _mPreView = true;
        private bool _mIn = true;
        public bool mXem
        {
            get
            {
                return _mXem;
            }
            set
            {
                if (_mXem == value)
                    return;
                _mXem = value;
            }
        }
        public bool mChinhSua
        {
            get
            {
                return _mChinhSua;
            }
            set
            {
                if (_mChinhSua == value)
                    return;
                _mChinhSua = value;
            }
        }
        public bool mTaoBangGia
        {
            get
            {
                return _mTaoBangGia;
            }
            set
            {
                if (_mTaoBangGia == value)
                    return;
                _mTaoBangGia = value;
            }
        }
        public bool mPreView
        {
            get
            {
                return _mPreView;
            }
            set
            {
                if (_mPreView == value)
                    return;
                _mPreView = value;
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
            }
        }
        #endregion

        #region binding visibilty
        public Button hplEdit { get; set; }
        public Button hplDelete { get; set; }
        public void hplEdit_Loaded(object sender)
        {
            hplEdit = sender as Button;
            hplEdit.Visibility = Globals.convertVisibility(mChinhSua);
        }
        public void hplDelete_Loaded(object sender)
        {
            hplDelete = sender as Button;
            hplDelete.Visibility = Globals.convertVisibility(mChinhSua);
        }
        #endregion

        public void btAddNew()
        {
            if (EffectiveDay > curDate
                && checkPriceList.hasFur)
            {
                MessageBox.Show(string.Format(eHCMSResources.Z1311_G1_I, checkPriceList.furTitle, checkPriceList.furDay.ToShortDateString()), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
            //if (EffectiveDay.Date == curDate.Date)
            {
                Action<IPCLExamTypePriceList_AddEdit> onInitDlg = delegate (IPCLExamTypePriceList_AddEdit typeInfo)
                {
                    typeInfo.TitleForm = eHCMSResources.Z1123_G1_TaoBGiaPCLExamTypeMoi;
                    typeInfo.BeginDate = EffectiveDay;
                    typeInfo.InitializeNewItem();
                    typeInfo.dpEffectiveDate_IsEnabled = false;
                };
                GlobalsNAV.ShowDialog<IPCLExamTypePriceList_AddEdit>(onInitDlg);
            }
        }

        public void hplAddNew()
        {
            PCLExamTypePriceList_CheckCanAddNew();
        }

        private void PCLExamTypePriceList_CheckCanAddNew()
        {
            bool CanAddNew = false;
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.Z0840_G1_DangKTra) });
            this.ShowBusyIndicator(eHCMSResources.Z0840_G1_DangKTra);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ConfigurationManagerServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginPCLExamTypePriceList_CheckCanAddNew(Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                contract.EndPCLExamTypePriceList_CheckCanAddNew(out CanAddNew, asyncResult);
                                if (CanAddNew)
                                {
                                    Action<IPCLExamTypePriceList_AddEdit> onInitDlg = delegate (IPCLExamTypePriceList_AddEdit typeInfo)
                                    {
                                        typeInfo.TitleForm = eHCMSResources.Z1123_G1_TaoBGiaPCLExamTypeMoi;
                                        typeInfo.InitializeNewItem();
                                    };
                                    GlobalsNAV.ShowDialog<IPCLExamTypePriceList_AddEdit>(onInitDlg);
                                }
                                else
                                {
                                    MessageBox.Show(string.Format("{0}!", eHCMSResources.A0476_G1_Msg_InfoDaCoBGiaMoi), eHCMSResources.T0780_G1_TaoBGiaMoi, MessageBoxButton.OK);
                                }
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
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }


        //private T3 _SearchCriteria;
        //public T3 SearchCriteria
        //{
        //    get
        //    {
        //        return _SearchCriteria;
        //    }
        //    set
        //    {
        //        _SearchCriteria = value;
        //        NotifyOfPropertyChange(() => SearchCriteria);
        //    }
        //}

        //private PagedSortableCollectionView<T1> _PriceList_GetList_Paging;
        //public PagedSortableCollectionView<T1> PriceList_GetList_Paging
        //{
        //    get { return _PriceList_GetList_Paging; }
        //    set
        //    {
        //        _PriceList_GetList_Paging = value;
        //        NotifyOfPropertyChange(() => PriceList_GetList_Paging);
        //    }
        //}

        private void SellingPriceList_GetList_Paging(int PageIndex, int PageSize, bool CountTotal)
        {
            
        }

        public void cboMonth_SelectionChanged(object selectItem)
        {
            ShowListBangGia();
        }

        public void cboYear_SelectionChanged(object selectItem)
        {
            ShowListBangGia();
        }

        private void ShowListBangGia()
        {
            
        }

        public void hplDelete_Click(object datacontext)
        {
            
        }
        private void PriceList_Delete(Int64 PriceListID)
        {
            
        }

        public void hplEdit_Click(object selectItem)
        {
            
        }

        public void dtgList_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            
        }
        public void Handle(SaveEvent<bool> message)
        {
            if (message != null)
            {
                if (message.Result)
                {
                    IsCheck = true;
                    //PriceList_GetList_Paging.PageIndex = 0;
                    //SellingPriceList_GetList_Paging(0, PriceList_GetList_Paging.PageSize, true);
                }
            }
        }

          public void btSearch()
          {
              ShowListBangGia();
          }
    }

      public class CheckPriceList
      {
          public DateTime curDay;
          public DateTime furDay;
          public bool hasCur;
          public bool hasFur;
          public string curTitle;
          public string furTitle;
      }
  }
