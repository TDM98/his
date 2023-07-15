using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ServiceModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using aEMR.DataContracts;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using Caliburn.Micro;
using DataEntities;
using aEMR.Common;
using aEMR.Common.Collections;
using System.Collections.ObjectModel;
using eHCMSLanguage;
using aEMR.CommonTasks;
using aEMR.Infrastructure.CachingUtils;
using Castle.Core.Logging;
using Castle.Windsor;
using Microsoft.Win32;

namespace aEMR.Pharmacy.ViewModels
{
    [Export(typeof(IPharmacySellingPriceList)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class PharmacySellingPriceListViewModel : Conductor<object>, IPharmacySellingPriceList
        , IHandle<SaveEvent<bool>>
        , IHandle<EventSaveRefItemPriceSuccess>
    {
        public string TitleForm { get; set; }

        private bool _IsReport = false;
        public bool IsReport
        {
            get { return _IsReport; }
            set
            {
                if (_IsReport != value)
                {
                    _IsReport = value;
                    NotifyOfPropertyChange(() => IsReport);
                    if (dtgList != null)
                    {
                        HideColumnDataGrid();
                    }
                }

            }
        }

        private void HideColumnDataGrid()
        {
            if (_IsReport)
            {
                dtgList.Columns[0].Visibility = Visibility.Collapsed;
            }
            else
            {
                dtgList.Columns[0].Visibility = Visibility.Visible;
            }
        }

        public bool IsCheck { get; set; }

        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public PharmacySellingPriceListViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;
            eventAggregator.Subscribe(this);

            ObjListMonth = new ObservableCollection<ClsMonth>();
            ObjListYear = new ObservableCollection<ClsYear>();

            LoadListMonth();
            LoadListYear();

            authorization();
            SearchCriteria = new PharmacySellingPriceListSearchCriteria();
            SearchCriteria.Month = -1;
            SearchCriteria.Year = Globals.GetCurServerDateTime().Year;

            SearchRefPriceListCriteria = new PharmacySellingPriceListSearchCriteria();
            SearchRefPriceListCriteria.Month = -1;
            SearchRefPriceListCriteria.Year = Globals.GetCurServerDateTime().Year;

            ObjPharmacySellingPriceList_GetList_Paging = new PagedSortableCollectionView<DataEntities.PharmacySellingPriceList>();
            ObjPharmacySellingPriceList_GetList_Paging.OnRefresh += new EventHandler<RefreshEventArgs>(ObjPharmacySellingPriceList_GetList_Paging_OnRefresh);

            ObjPharmacySellingPriceList_GetList_Paging.PageIndex = 0;
            IsCheck = true;
            PharmacySellingPriceList_GetList_Paging(0, ObjPharmacySellingPriceList_GetList_Paging.PageSize, true);

            ReferencePriceList = new PagedSortableCollectionView<PharmacyReferencePriceList>();
            ReferencePriceList.OnRefresh += ReferencePriceList_OnRefresh;

            ReferencePriceList.PageIndex = 0;
            GetReferencePriceList(0, ReferencePriceList.PageSize, true);
        }

        public void btAddNew()
        {
            if (EffectiveDay.Date < curDate.Date)
            {
                MessageBox.Show(eHCMSResources.A0682_G1_Msg_InfoKhTaoDcBGiaChoQKhu
                    , eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
            if (EffectiveDay > curDate
                && checkPriceList.hasFur)
            {
                MessageBox.Show(string.Format(eHCMSResources.Z1311_G1_I, checkPriceList.furTitle, checkPriceList.furDay.ToShortDateString()), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
            //if (EffectiveDay.Date == curDate.Date)
            {
                //var typeInfo = Globals.GetViewModel<IPharmacySellingPriceList_AddEdit_V2>();
                //typeInfo.TitleForm = eHCMSResources.Z0709_G1_TaoBGiaThuocMoi;
                //typeInfo.BeginDate = EffectiveDay;
                //typeInfo.dpEffectiveDate_IsEnabled = false;
                //var instance = typeInfo as Conductor<object>;
                //Globals.ShowDialog(instance, (o) =>
                //{
                //    //lam gi do
                //});

                Action<IPharmacySellingPriceList_AddEdit_V2> onInitDlg = (typeInfo) =>
                {
                    typeInfo.TitleForm = eHCMSResources.Z0709_G1_TaoBGiaThuocMoi;
                    typeInfo.BeginDate = EffectiveDay;
                    typeInfo.dpEffectiveDate_IsEnabled = false;
                };
                GlobalsNAV.ShowDialog<IPharmacySellingPriceList_AddEdit_V2>(onInitDlg);

            }
        }

        public void hplAddNew()
        {
            //PharmacySellingPriceList_CheckCanAddNew();
            DateTime BeginDate = curDate;
            if (checkPriceList.hasCur
                && checkPriceList.hasFur)
            {
                if (MessageBox.Show(string.Format(eHCMSResources.Z1485_G1_I, checkPriceList.curDay.Month, checkPriceList.curDay.Year, checkPriceList.furDay.Month, checkPriceList.furDay.Year)
                                , eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    return;
                }
                else
                {
                    BeginDate = curDate;
                }
            }
            if (checkPriceList.hasCur
                && checkPriceList.hasFur)
            {
                if (MessageBox.Show(string.Format(eHCMSResources.Z1485_G1_I, checkPriceList.curDay.Month, checkPriceList.curDay.Year, checkPriceList.furDay.Month, checkPriceList.furDay.Year)
                    ,eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    return;
                }
                else
                {
                    BeginDate = curDate;
                }
            }
            if (checkPriceList.hasCur && !checkPriceList.hasFur)
            {
                if (MessageBox.Show(string.Format(eHCMSResources.Z1486_G1_I, checkPriceList.curDay.Month, checkPriceList.curDay.Year)
                    , eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    return;
                }
                else
                {
                    BeginDate = curDate;
                }
            }
            //var typeInfo = Globals.GetViewModel<IPharmacySellingPriceList_AddEdit_V2>();
            //typeInfo.TitleForm = eHCMSResources.Z0709_G1_TaoBGiaThuocMoi;
            //typeInfo.BeginDate = BeginDate;
            //var instance = typeInfo as Conductor<object>;
            //Globals.ShowDialog(instance, (o) =>
            //{
            //    //lam gi do
            //});


            Action<IPharmacySellingPriceList_AddEdit_V2> onInitDlg = (typeInfo) =>
            {
                typeInfo.TitleForm = eHCMSResources.Z0709_G1_TaoBGiaThuocMoi;
                typeInfo.BeginDate = BeginDate;
            };
            GlobalsNAV.ShowDialog<IPharmacySellingPriceList_AddEdit_V2>(onInitDlg);
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


        public void dtgList_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            PharmacySellingPriceList objRows = e.Row.DataContext as PharmacySellingPriceList;
            if (objRows != null)
            {
                if (objRows.EffectiveDate.Date > curDate.Date)
                {
                    e.Row.Foreground = new SolidColorBrush(Colors.Purple);
                }
                else
                    if (objRows.IsActive)
                    {
                        e.Row.Foreground = new SolidColorBrush(Colors.Green);
                    }
                    else
                    {
                        e.Row.Foreground = new SolidColorBrush(Colors.Gray);
                    }
            }
        }

        public void authorization()
        {
            if (!Globals.isAccountCheck)
            {
                return;
            }
            bTim = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPharmacy
                                               , (int)ePharmacy.mQuanLyKho,
                                               (int)oPharmacyEx.mQuanLyKho_Tim, (int)ePermission.mView);
            bThem = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPharmacy
                                               , (int)ePharmacy.mQuanLyKho,
                                               (int)oPharmacyEx.mQuanLyKho_Them, (int)ePermission.mView);
            bChinhSua = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPharmacy
                                               , (int)ePharmacy.mQuanLyKho,
                                               (int)oPharmacyEx.mQuanLyKho_ChinhSua, (int)ePermission.mView);

        }
        #region checking account

        private bool _bTim = true;
        private bool _bThem = true;
        private bool _bChinhSua = true;

        public bool bTim
        {
            get
            {
                return _bTim;
            }
            set
            {
                if (_bTim == value)
                    return;
                _bTim = value;
            }
        }
        public bool bThem
        {
            get
            {
                return _bThem;
            }
            set
            {
                if (_bThem == value)
                    return;
                _bThem = value;
            }
        }
        public bool bChinhSua
        {
            get
            {
                return _bChinhSua;
            }
            set
            {
                if (_bChinhSua == value)
                    return;
                _bChinhSua = value;
            }
        }

        #endregion
        #region binding visibilty

        public Button lnkDelete { get; set; }
        public Button lnkView { get; set; }

        public void lnkDelete_Loaded(object sender)
        {
            lnkDelete = sender as Button;
            if (lnkDelete != null)
            {
                lnkDelete.Visibility = Globals.convertVisibility(bChinhSua);
            }
        }
        public void lnkView_Loaded(object sender)
        {
            lnkView = sender as Button;
            if (lnkView != null)
            {
                lnkView.Visibility = Globals.convertVisibility(bChinhSua);
            }
        }
        #endregion

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
                ClsMonth ObjM = new ClsMonth();
                ObjM.mValue = i;
                ObjM.mText = i.ToString();
                ObjListMonth.Add(ObjM);
            }
            //Default Item
            ClsMonth DefaultItem = new ClsMonth();
            DefaultItem.mValue = -1;
            DefaultItem.mText = eHCMSResources.A0015_G1_Chon;
            ObjListMonth.Insert(0, DefaultItem);
            //Default Item
        }

        private void LoadListYear()
        {
            for (int i = Globals.ServerDate.Value.Year - 5; i <= Globals.ServerDate.Value.Year; i++)
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

        void ObjPharmacySellingPriceList_GetList_Paging_OnRefresh(object sender, RefreshEventArgs e)
        {
            PharmacySellingPriceList_GetList_Paging(ObjPharmacySellingPriceList_GetList_Paging.PageIndex, ObjPharmacySellingPriceList_GetList_Paging.PageSize, false);
        }

        private void PharmacySellingPriceList_CheckCanAddNew()
        {
            bool CanAddNew = false;

            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.Z0840_G1_DangKTra) });
            IsLoading = true;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyDrugServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginPharmacySellingPriceList_CheckCanAddNew(Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            contract.EndPharmacySellingPriceList_CheckCanAddNew(out CanAddNew, asyncResult);
                            if (CanAddNew)
                            {
                                //var typeInfo = Globals.GetViewModel<IPharmacySellingPriceList_AddEdit_V2>();
                                //typeInfo.TitleForm = eHCMSResources.Z0709_G1_TaoBGiaThuocMoi;

                                //var instance = typeInfo as Conductor<object>;

                                //Globals.ShowDialog(instance, (o) =>
                                //{
                                //    //lam gi do
                                //});

                                Action<IPharmacySellingPriceList_AddEdit_V2> onInitDlg = (typeInfo) =>
                                {
                                    typeInfo.TitleForm = eHCMSResources.Z0709_G1_TaoBGiaThuocMoi;
                                };
                                GlobalsNAV.ShowDialog<IPharmacySellingPriceList_AddEdit_V2>(onInitDlg);

                            }
                            else
                            {
                                MessageBox.Show(eHCMSResources.A0476_G1_Msg_InfoDaCoBGiaMoi, eHCMSResources.T0780_G1_TaoBGiaMoi, MessageBoxButton.OK);
                            }
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


        private PharmacySellingPriceListSearchCriteria _SearchCriteria;
        public PharmacySellingPriceListSearchCriteria SearchCriteria
        {
            get
            {
                return _SearchCriteria;
            }
            set
            {
                _SearchCriteria = value;
                NotifyOfPropertyChange(() => SearchCriteria);
            }
        }

        private CheckPriceList checkPriceList;


        private PagedSortableCollectionView<DataEntities.PharmacySellingPriceList> _ObjPharmacySellingPriceList_GetList_Paging;
        public PagedSortableCollectionView<DataEntities.PharmacySellingPriceList> ObjPharmacySellingPriceList_GetList_Paging
        {
            get { return _ObjPharmacySellingPriceList_GetList_Paging; }
            set
            {
                _ObjPharmacySellingPriceList_GetList_Paging = value;
                NotifyOfPropertyChange(() => ObjPharmacySellingPriceList_GetList_Paging);
            }
        }

        private void PharmacySellingPriceList_GetList_Paging(int PageIndex, int PageSize, bool CountTotal)
        {
            ObjPharmacySellingPriceList_GetList_Paging.Clear();

            Globals.EventAggregator.Publish(new BusyEvent() { IsBusy = true, Message = string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.G2422_G1_XemDSGia) });
            IsLoading = true;
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyDrugServiceClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginPharmacySellingPriceList_GetList_Paging(SearchCriteria, PageIndex, PageSize, "", CountTotal, Globals.DispatchCallback((asyncResult) =>
                        {
                            int Total = 0;
                            List<PharmacySellingPriceList> allItems = null;
                            bool bOK = false;
                            DateTime currentDate;
                            try
                            {
                                allItems = client.EndPharmacySellingPriceList_GetList_Paging(out Total, out currentDate, asyncResult);
                                bOK = true;
                                curDate = currentDate;
                                NotifyOfPropertyChange(() => curDate);
                                EffectiveDay = currentDate;
                                if (IsCheck)
                                {
                                    checkPriceList = new CheckPriceList();
                                    foreach (var item in allItems)
                                    {
                                        //if (item.EffectiveDate>curDate)
                                        {
                                            if (item.EffectiveDate == curDate)
                                            {
                                                checkPriceList.hasCur = true;
                                                checkPriceList.curDay = item.EffectiveDate;
                                                checkPriceList.curTitle = item.PriceListTitle;
                                            }
                                            else if (item.EffectiveDate.Date > curDate.Date)
                                            {
                                                checkPriceList.hasFur = true;
                                                checkPriceList.furDay = item.EffectiveDate;
                                                checkPriceList.furTitle = item.PriceListTitle;
                                            }
                                        }
                                        //checkPriceList
                                    }
                                }
                                IsCheck = false;
                            }
                            catch (FaultException<AxException> fault)
                            {
                                ClientLoggerHelper.LogInfo(fault.ToString());
                            }
                            catch (Exception ex)
                            {
                                ClientLoggerHelper.LogInfo(ex.ToString());
                            }


                            if (bOK)
                            {
                                if (CountTotal)
                                {
                                    ObjPharmacySellingPriceList_GetList_Paging.TotalItemCount = Total;
                                }
                                if (allItems != null)
                                {
                                    foreach (var item in allItems)
                                    {
                                        ObjPharmacySellingPriceList_GetList_Paging.Add(item);
                                    }

                                }
                            }
                        }), null)
                            ;
                    }
                }
                catch (Exception ex)
                {
                    ClientLoggerHelper.LogInfo(ex.ToString());
                }
                finally
                {
                    IsLoading = false;
                    Globals.IsBusy = false;
                }
            });
            t.Start();
        }


        public void hplDelete_Click(object datacontext)
        {
            if (datacontext != null)
            {
                PharmacySellingPriceList p = (datacontext as PharmacySellingPriceList);

                switch (p.PriceListType)
                {
                    case "PriceList-InUse":
                        {
                            MessageBox.Show(eHCMSResources.A0206_G1_Msg_InfoKhDcXoaBGiaCu, eHCMSResources.A0480_G1_Msg_XoaBGia, MessageBoxButton.OK);
                            break;
                        }
                    case "PriceList-InFuture":
                        {
                            if (MessageBox.Show(eHCMSResources.A0152_G1_Msg_ConfXoaBGia, eHCMSResources.G2617_G1_Xoa, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                            {
                                PharmacySellingPriceList_Delete(p.PharmacySellingPriceListID);
                            }
                            break;
                        }
                    case "PriceList-Old":
                        {
                            MessageBox.Show(eHCMSResources.Z1256_BGiaCuKgDcXoa, eHCMSResources.A0480_G1_Msg_XoaBGia, MessageBoxButton.OK);
                            break;
                        }
                }
            }
        }
        private void PharmacySellingPriceList_Delete(Int64 PharmacySellingPriceListID)
        {
            string Result = "";

            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.Z0492_G1_DangXoa) });
            IsLoading = true;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyDrugServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginPharmacySellingPriceList_Delete(PharmacySellingPriceListID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            contract.EndPharmacySellingPriceList_Delete(out Result, asyncResult);
                            switch (Result)
                            {
                                case "Delete-1":
                                    {
                                        ObjPharmacySellingPriceList_GetList_Paging.PageIndex = 0;
                                        IsCheck = true;
                                        PharmacySellingPriceList_GetList_Paging(0, ObjPharmacySellingPriceList_GetList_Paging.PageSize, true);
                                        MessageBox.Show(eHCMSResources.A0478_G1_Msg_InfoXoaOK, eHCMSResources.A0480_G1_Msg_XoaBGia, MessageBoxButton.OK);
                                        break;
                                    }
                                case "PriceList-InUse-Old":
                                    {
                                        MessageBox.Show(eHCMSResources.A0206_G1_Msg_InfoKhDcXoaBGiaCu, eHCMSResources.A0480_G1_Msg_XoaBGia, MessageBoxButton.OK);
                                        break;
                                    }
                                case "Delete-0":
                                    {
                                        MessageBox.Show(eHCMSResources.K0484_G1_XoaFail, eHCMSResources.A0480_G1_Msg_XoaBGia, MessageBoxButton.OK);
                                        break;
                                    }
                            }
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

        public void hplEdit_Click(object selectItem)
        {
            PharmacySellingPriceList Objtmp = (selectItem as PharmacySellingPriceList).DeepCopy();
            FormEdit(Objtmp);
        }

        private void FormEdit(PharmacySellingPriceList Objtmp)
        {
            string LoaiBangGia = "";
            string TieuDe = "";
            bool dpEffectiveDate_IsEnabled = true;
            if (Objtmp != null)
            {
                switch (Objtmp.PriceListType)
                {
                    case "PriceList-Old":
                        {
                            LoaiBangGia = eHCMSResources.Z0728_G1_Cu;
                            TieuDe = eHCMSResources.G2386_G1_Xem;
                            break;
                        }
                    case "PriceList-InFuture":
                        {
                            LoaiBangGia = eHCMSResources.Z0729_G1_ChuaApDung;
                            TieuDe = eHCMSResources.T1484_G1_HChinh;
                            dpEffectiveDate_IsEnabled = true;
                            break;
                        }
                    case "PriceList-InUse":
                        {
                            LoaiBangGia = eHCMSResources.Z0730_G1_DangApDung;
                            TieuDe = eHCMSResources.T1484_G1_HChinh;
                            break;
                        }
                }
                if (Objtmp.IsActive)
                {
                    dpEffectiveDate_IsEnabled = false;
                }
                //var typeInfo = Globals.GetViewModel<IPharmacySellingPriceList_AddEdit_V2>();
                //typeInfo.TitleForm = TieuDe + " " + Objtmp.PriceListTitle.Trim() + " (" + LoaiBangGia + ")";

                //typeInfo.ObjPharmacySellingPriceList_Current = ObjectCopier.DeepCopy(Objtmp);
                //typeInfo.BeginDate = curDate;
                //typeInfo.dpEffectiveDate_IsEnabled = dpEffectiveDate_IsEnabled;
                //var instance = typeInfo as Conductor<object>;

                //Globals.ShowDialog(instance, (o) =>
                //{
                //    //lam gi do
                //});

                Action<IPharmacySellingPriceList_AddEdit_V2> onInitDlg = (typeInfo) =>
                {
                    typeInfo.TitleForm = TieuDe + " " + Objtmp.PriceListTitle.Trim() + " (" + LoaiBangGia + ")";

                    typeInfo.ObjPharmacySellingPriceList_Current = ObjectCopier.DeepCopy(Objtmp);
                    typeInfo.BeginDate = curDate;
                    typeInfo.dpEffectiveDate_IsEnabled = dpEffectiveDate_IsEnabled;
                };
                GlobalsNAV.ShowDialog<IPharmacySellingPriceList_AddEdit_V2>(onInitDlg);

            }
        }

        public void DoubleClick(object sender, Common.EventArgs<object> e)
        {
            PharmacySellingPriceList Objtmp = (e.Value as PharmacySellingPriceList).DeepCopy();
            FormEdit(Objtmp);
        }

        public void Handle(SaveEvent<bool> message)
        {
            if (message != null)
            {
                if (message.Result)
                {
                    ObjPharmacySellingPriceList_GetList_Paging.PageIndex = 0;
                    IsCheck = true;
                    PharmacySellingPriceList_GetList_Paging(0, ObjPharmacySellingPriceList_GetList_Paging.PageSize, true);
                }
            }
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
            if (SearchCriteria.Month > 0 || SearchCriteria.Year > 0)
            {
                ObjPharmacySellingPriceList_GetList_Paging.PageIndex = 0;
                PharmacySellingPriceList_GetList_Paging(0, ObjPharmacySellingPriceList_GetList_Paging.PageSize, true);
            }
            else
            {
                ObjPharmacySellingPriceList_GetList_Paging.Clear();
            }
        }


        public void hplViewPrint_Click(object selectItem)
        {
            if (selectItem != null)
            {
                PharmacySellingPriceList Objtmp = (selectItem as PharmacySellingPriceList);

               // var proAlloc = Globals.GetViewModel<ICommonPreviewView>();
               // proAlloc.TieuDeRpt = string.Format(eHCMSResources.Z1483_G1_BGiaBanThuocThang, Objtmp.RecCreatedDate.Month.ToString(), Objtmp.RecCreatedDate.Year.ToString());
               // proAlloc.PharmacySellingPriceListID = Objtmp.PharmacySellingPriceListID;
               //// proAlloc.eItem = ReportName.RptBangGiaThuocNhaThuoc;
               // proAlloc.eItem = ReportName.XRptPharmacySellingPriceList_Detail_Simple;

               // var instance = proAlloc as Conductor<object>;
               // Globals.ShowDialog(instance, (o) => { });


                Action<ICommonPreviewView> onInitDlg = (proAlloc) =>
                {
                    proAlloc.TieuDeRpt = string.Format(eHCMSResources.Z1483_G1_BGiaBanThuocThang, Objtmp.RecCreatedDate.Month.ToString(), Objtmp.RecCreatedDate.Year.ToString());
                    proAlloc.PharmacySellingPriceListID = Objtmp.PharmacySellingPriceListID;
                    proAlloc.eItem = ReportName.XRptPharmacySellingPriceList_Detail_Simple;
                };
                GlobalsNAV.ShowDialog<ICommonPreviewView>(onInitDlg);
            }
        }

        public void hplExportExcel_Click(object selectItem)
        {
            if (selectItem == null)
            {
                return;
            }
            PharmacySellingPriceList ObjPharmacySellingPriceList_Current = (selectItem as PharmacySellingPriceList);
            if (ObjPharmacySellingPriceList_Current == null || ObjPharmacySellingPriceList_Current.PharmacySellingPriceListID <= 0)
            {
                MessageBox.Show(eHCMSResources.K0275_G1_ChonBGiaDeXuatExcel);
                return;
            }
            SaveFileDialog objSFD = new SaveFileDialog()
            {
                DefaultExt = ".xls",
                Filter = "Excel xls (*.xls)|*.xls",
                //Filter = "Excel (2003)(.xls)|*.xls|Excel (2010) (.xlsx)|*.xlsx |RichText File (.rtf)|*.rtf |Pdf File (.pdf)|*.pdf |Html File (.html)|*.html",
                FilterIndex = 1
            };
            if (objSFD.ShowDialog() != true)
            {
                return;
            }
            //Coroutine.BeginExecute(ExportAllItemsPriceListToExcel(ObjPharmacySellingPriceList_Current.PharmacySellingPriceListID, (int)AllLookupValues.PriceListType.BANG_GIA_THUOC, objSFD));   
            
            ReportParameters RptParameters = new ReportParameters();
            RptParameters.ReportType = ReportType.BANG_GIA;
            RptParameters.PriceList = new PriceList
            {
                PriceListID = ObjPharmacySellingPriceList_Current.PharmacySellingPriceListID,
                PriceListType = PriceListType.BANG_GIA_NHA_THUOC
            };
            RptParameters.Show = "BangGia";
            ExportToExcelGeneric.Action(RptParameters, objSFD, this);
            //Coroutine.BeginExecute(DoSaveExcel(RptParameters, objSFD));
        }

        //private IEnumerator<IResult> DoSaveExcel(ReportParameters rptParameters, SaveFileDialog objSFD)
        //{
        //    var res = new ExportToExcellAllGenericTask(rptParameters, objSFD);
        //    yield return res;
        //    //IsProcessing = false;
        //    yield break;
        //}

        //private IEnumerator<IResult> ExportAllItemsPriceListToExcel(long PharmacySellingPriceListID, int PriceListType, SaveFileDialog objSFD)
        //{
        //    var res = new ExportToExcelAllItemsPriceListTask(PharmacySellingPriceListID, PriceListType, objSFD);
        //    yield return res;
        //    //IsProcessing = false;
        //    yield break;
        //}

        DataGrid dtgList = null;
        public void dtgList_Loaded(object sender, RoutedEventArgs e)
        {
            dtgList = sender as DataGrid;
            HideColumnDataGrid();
        }

        #region Reference Price List

        private PharmacySellingPriceListSearchCriteria _SearchRefPriceListCriteria;
        public PharmacySellingPriceListSearchCriteria SearchRefPriceListCriteria
        {
            get
            {
                return _SearchRefPriceListCriteria;
            }
            set
            {
                _SearchRefPriceListCriteria = value;
                NotifyOfPropertyChange(() => SearchRefPriceListCriteria);
            }
        }

        private PagedSortableCollectionView<PharmacyReferencePriceList> _ReferencePriceList;
        public PagedSortableCollectionView<PharmacyReferencePriceList> ReferencePriceList
        {
            get { return _ReferencePriceList; }
            set
            {
                _ReferencePriceList = value;
                NotifyOfPropertyChange(() => ReferencePriceList);
            }
        }

        void ReferencePriceList_OnRefresh(object sender, RefreshEventArgs e)
        {
            GetReferencePriceList(ReferencePriceList.PageIndex, ReferencePriceList.PageSize, false);
        }

        public void btAddNewRefPriceList()
        {
            //var typeInfo = Globals.GetViewModel<IPharmacyReferenceItemPrice>();
            //typeInfo.TitleForm = eHCMSResources.Z0709_G1_TaoBGiaThuocMoi;
            //var instance = typeInfo as Conductor<object>;
            //Globals.ShowDialog(instance, (o) =>
            //{
            //    //lam gi do
            //});
            Action<IPharmacyReferenceItemPrice> onInitDlg = (typeInfo) =>
            {
                typeInfo.TitleForm = eHCMSResources.Z0709_G1_TaoBGiaThuocMoi;
            };
            GlobalsNAV.ShowDialog<IPharmacyReferenceItemPrice>(onInitDlg);
        }

        private void GetReferencePriceList(int PageIndex, int PageSize, bool CountTotal)
        {
            ReferencePriceList.Clear();

            this.ShowBusyIndicator();
            
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyDrugServiceClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginGetReferencePriceList(SearchRefPriceListCriteria, PageIndex, PageSize, "", CountTotal, Globals.DispatchCallback((asyncResult) =>
                        {
                            int Total = 0;
                            try
                            {
                                ReferencePriceList.SourceCollection = client.EndGetReferencePriceList(out Total, asyncResult);

                                ReferencePriceList.TotalItemCount = Total;
                            }
                            catch (Exception ex)
                            {
                                ClientLoggerHelper.LogInfo(ex.ToString());
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
                    ClientLoggerHelper.LogInfo(ex.ToString());
                }
                finally
                {
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }

        public void hplEditReferencePriceList_Click(object selectItem)
        {
            PharmacyReferencePriceList Objtmp = (selectItem as PharmacyReferencePriceList).DeepCopy();

            string LoaiBangGia = "Đang áp dụng";
            string TieuDe = "Xem";

            //var typeInfo = Globals.GetViewModel<IPharmacyReferenceItemPrice>();
            //typeInfo.TitleForm = TieuDe + " " + Objtmp.Title.Trim() + " (" + LoaiBangGia + ")";

            //typeInfo.CurrentRefPriceList = ObjectCopier.DeepCopy(Objtmp);
            //var instance = typeInfo as Conductor<object>;

            //Globals.ShowDialog(instance, (o) =>
            //{
            //    //lam gi do
            //});

            Action<IPharmacyReferenceItemPrice> onInitDlg = (typeInfo) =>
            {
                typeInfo.TitleForm = TieuDe + " " + Objtmp.Title.Trim() + " (" + LoaiBangGia + ")";

                typeInfo.CurrentRefPriceList = ObjectCopier.DeepCopy(Objtmp);
            };
            GlobalsNAV.ShowDialog<IPharmacyReferenceItemPrice>(onInitDlg);
        }

        public void cboMonthOfRefPriceList_SelectionChanged(object selectItem)
        {
            if (SearchRefPriceListCriteria.Month > 0 || SearchRefPriceListCriteria.Year > 0)
            {
                GetReferencePriceList(0, ReferencePriceList.PageSize, true);
            }
            else
            {
                ReferencePriceList.Clear();
            }
        }

        public void cboYearOfRefPriceList_SelectionChanged(object selectItem)
        {
            if (SearchRefPriceListCriteria.Month > 0 || SearchRefPriceListCriteria.Year > 0)
            {
                GetReferencePriceList(0, ReferencePriceList.PageSize, true);
            }
            else
            {
                ReferencePriceList.Clear();
            }
        }


        public void Handle(EventSaveRefItemPriceSuccess message)
        {
            GetReferencePriceList(0, ReferencePriceList.PageSize, true);
        }

        #endregion


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
