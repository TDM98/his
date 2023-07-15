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
using aEMR.CommonTasks;
using eHCMSLanguage;
using Microsoft.Win32;
using Castle.Windsor;
/*
 * 20180918 #001 TTM: 
 */
namespace aEMR.DrugDept.ViewModels
{
    [Export(typeof(IDrugDeptSellingPriceList)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class DrugDeptSellingPriceListViewModel : Conductor<object>, IDrugDeptSellingPriceList
        , IHandle<SaveEvent<bool>>
    {
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

        private string _TitleForm;
        public string TitleForm
        {
            get { return _TitleForm; }
            set
            {
                _TitleForm = value;
                NotifyOfPropertyChange(() => TitleForm);
            }
        }

        private CheckPriceList checkPriceList;

        public bool IsCheck { get; set; }

        #region check invisible

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
                NotifyOfPropertyChange(() => mXem);
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
                NotifyOfPropertyChange(() => mChinhSua);
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
                NotifyOfPropertyChange(() => mTaoBangGia);
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
                NotifyOfPropertyChange(() => mPreView);
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

        #endregion

        [ImportingConstructor]
        public DrugDeptSellingPriceListViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg)
        {
            IsCheck = true;
            Globals.EventAggregator.Subscribe(this);
        }

        public void Init()
        {
            LoadListMonth();
            LoadListYear();

            SearchCriteria = new DrugDeptSellingPriceListSearchCriteria
            {
                Month = -1,
                Year = Globals.GetCurServerDateTime().Year,
                V_MedProductType = V_MedProductType
            };

            ObjDrugDeptSellingPriceList_GetList_Paging = new PagedSortableCollectionView<DrugDeptSellingPriceList>();
            ObjDrugDeptSellingPriceList_GetList_Paging.OnRefresh += ObjDrugDeptSellingPriceList_GetList_Paging_OnRefresh;
            ObjDrugDeptSellingPriceList_GetList_Paging.PageIndex = 0;
            //20180721 TBL: Comment lai vi bi double dong
            DrugDeptSellingPriceList_GetList_Paging(0, ObjDrugDeptSellingPriceList_GetList_Paging.PageSize, true);
            IsCheck = true;
        }
        //▼====== #001  Đặt giá trị mặc định chi EffectiveDay bằng ngày hiện tại của hệ thống.
        //              Vì nếu không có giá trị mặc định thì nó sẽ lấy ngày null (01/01/0001) gắn vào DatePicker trên xaml => không đúng.
        private DateTime _EffectiveDay = Globals.GetCurServerDateTime();
        //▲====== #001
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

        private DateTime _curDate= DateTime.Now;
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
                void onInitDlg(IDrugDeptSellingPriceList_AddEdit typeInfo)
                {
                    typeInfo.TitleForm = eHCMSResources.Z0709_G1_TaoBGiaThuocMoi;
                    typeInfo.V_MedProductType = V_MedProductType;
                    typeInfo.TitleForm = string.Format(eHCMSResources.Z0710_G1_TaoBGia0Moi, Globals.GetTextV_MedProductType(V_MedProductType));
                    typeInfo.BeginDate = EffectiveDay;
                    typeInfo.dpEffectiveDate_IsEnabled = false;
                }
                GlobalsNAV.ShowDialog<IDrugDeptSellingPriceList_AddEdit>(onInitDlg, null, false, true, Globals.GetDefaultDialogViewSize());
            }
        }

        public void hplAddNew()
        {
            DateTime BeginDate = curDate;
            DateTime EndDate = BeginDate.AddMonths(1);
            if (checkPriceList.hasCur
                && checkPriceList.hasFur)
            {
                if (MessageBox.Show(string.Format(eHCMSResources.Z0711_G1_DaTonTaiBGiaChoTh0Nam0, checkPriceList.curDay.Month, checkPriceList.curDay.Year)
                   + "\n" + string.Format(eHCMSResources.Z0712_G1_BGiaMoiChoTh0Nam0, checkPriceList.furDay.Month, checkPriceList.furDay.Year) + "."
                   + "\n" + eHCMSResources.Z0713_G1_I + "!"
                   , eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
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
                if (MessageBox.Show(string.Format(eHCMSResources.Z0711_G1_DaTonTaiBGiaChoTh0Nam0, checkPriceList.curDay.Month, checkPriceList.curDay.Year)
                   + "."
                   + "\n" + eHCMSResources.Z0714_G1_I + "!"
                   , eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    return;
                }
                else
                {
                    BeginDate = curDate;
                }
            }
            //DrugDeptSellingPriceList_CheckCanAddNew();
            void onInitDlg(IDrugDeptSellingPriceList_AddEdit typeInfo)
            {
                typeInfo.TitleForm = string.Format(eHCMSResources.Z0710_G1_TaoBGia0Moi, Globals.GetTextV_MedProductType(V_MedProductType));
                typeInfo.V_MedProductType = V_MedProductType;
                typeInfo.BeginDate = BeginDate;
                typeInfo.EndDate = EndDate;
            }
            GlobalsNAV.ShowDialog<IDrugDeptSellingPriceList_AddEdit>(onInitDlg, null, false, true, Globals.GetDefaultDialogViewSize());
        }

        #region Tháng, Năm
        public class ClsMonth
        {
            public int mValue { get; set; }

            public string mText { get; set; }
        }

        public class ClsYear
        {
            public int mValue { get; set; }

            public string mText { get; set; }
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
            ObjListMonth = new ObservableCollection<ClsMonth>();

            for (int i = 1; i <= 12; i++)
            {
                ClsMonth ObjM = new ClsMonth
                {
                    mValue = i,
                    mText = i.ToString()
                };
                ObjListMonth.Add(ObjM);
            }
            //Default Item
            ClsMonth DefaultItem = new ClsMonth
            {
                mValue = -1,
                mText = eHCMSResources.A0015_G1_Chon
            };
            ObjListMonth.Insert(0, DefaultItem);
            //Default Item
        }

        private void LoadListYear()
        {
            ObjListYear = new ObservableCollection<ClsYear>();

            for (int i = 2010; i <= 2099; i++)
            {
                ClsYear ObjY = new ClsYear
                {
                    mValue = i,
                    mText = i.ToString()
                };
                ObjListYear.Add(ObjY);
            }
            //Default Item
            ClsYear DefaultItem = new ClsYear
            {
                mValue = -1,
                mText = eHCMSResources.A0015_G1_Chon
            };
            ObjListYear.Insert(0, DefaultItem);
            //Default Item
        }

        #region check authority link

        public Button lnkDelete { get; set; }
        public Button lnkEdit { get; set; }
        public Button lnkView { get; set; }
        public void hplDelete_Loaded(object sender)
        {
            lnkDelete = sender as Button;
            lnkDelete.Visibility = Globals.convertVisibility(mChinhSua );
        }
        public void hplEdit_Loaded(object sender)
        {
            lnkEdit = sender as Button;
            lnkEdit.Visibility = Globals.convertVisibility(mChinhSua );
        }
        public void hplViewPrint_Loaded(object sender)
        {
            lnkView = sender as Button;
            lnkView.Visibility = Globals.convertVisibility(mIn);
        }


        #endregion

        #endregion

        public void dtgList_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            DrugDeptSellingPriceList objRows = e.Row.DataContext as DrugDeptSellingPriceList;
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

        void ObjDrugDeptSellingPriceList_GetList_Paging_OnRefresh(object sender, RefreshEventArgs e)
        {
            //20180721 TBL: Doi parameter tu false thanh true de co the chuyen qua trang khac duoc
            //DrugDeptSellingPriceList_GetList_Paging(ObjDrugDeptSellingPriceList_GetList_Paging.PageIndex, ObjDrugDeptSellingPriceList_GetList_Paging.PageSize, false);
            DrugDeptSellingPriceList_GetList_Paging(ObjDrugDeptSellingPriceList_GetList_Paging.PageIndex, ObjDrugDeptSellingPriceList_GetList_Paging.PageSize, true);
        }

        private void DrugDeptSellingPriceList_CheckCanAddNew()
        {
            bool CanAddNew = false;

            this.ShowBusyIndicator(eHCMSResources.Z0840_G1_DangKTra);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyDrugServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginDrugDeptSellingPriceList_CheckCanAddNew(Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                contract.EndDrugDeptSellingPriceList_CheckCanAddNew(out CanAddNew, asyncResult);
                                if (CanAddNew)
                                {
                                    void onInitDlg(IDrugDeptSellingPriceList_AddEdit typeInfo)
                                    {
                                        typeInfo.TitleForm = string.Format(eHCMSResources.Z0710_G1_TaoBGia0Moi, Globals.GetTextV_MedProductType(V_MedProductType));
                                        typeInfo.V_MedProductType = V_MedProductType;
                                    }
                                    GlobalsNAV.ShowDialog<IDrugDeptSellingPriceList_AddEdit>(onInitDlg, null, false, true, Globals.GetDefaultDialogViewSize());
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

        private DrugDeptSellingPriceListSearchCriteria _SearchCriteria;
        public DrugDeptSellingPriceListSearchCriteria SearchCriteria
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

        private PagedSortableCollectionView<DrugDeptSellingPriceList> _ObjDrugDeptSellingPriceList_GetList_Paging;
        public PagedSortableCollectionView<DrugDeptSellingPriceList> ObjDrugDeptSellingPriceList_GetList_Paging
        {
            get { return _ObjDrugDeptSellingPriceList_GetList_Paging; }
            set
            {
                _ObjDrugDeptSellingPriceList_GetList_Paging = value;
                NotifyOfPropertyChange(() => ObjDrugDeptSellingPriceList_GetList_Paging);
            }
        }

        private void DrugDeptSellingPriceList_GetList_Paging(int PageIndex, int PageSize, bool CountTotal)
        {
            ObjDrugDeptSellingPriceList_GetList_Paging.Clear();

            this.ShowBusyIndicator(eHCMSResources.K2922_G1_DSBGia);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyDrugServiceClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginDrugDeptSellingPriceList_GetList_Paging(SearchCriteria, PageIndex, PageSize, "", CountTotal, Globals.DispatchCallback((asyncResult) =>
                        {
                            int Total = 0;
                            DateTime currentDate;
                            List<DrugDeptSellingPriceList> allItems = null;
                            bool bOK = false;
                            try
                            {
                                allItems = client.EndDrugDeptSellingPriceList_GetList_Paging(out Total,out currentDate, asyncResult);
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
                            finally
                            {
                                this.HideBusyIndicator();
                            }

                            if (bOK)
                            {
                                if (CountTotal)
                                {
                                    ObjDrugDeptSellingPriceList_GetList_Paging.TotalItemCount = Total;
                                }
                                if (allItems != null)
                                {
                                    foreach (var item in allItems)
                                    {
                                        ObjDrugDeptSellingPriceList_GetList_Paging.Add(item);
                                    }
                                }
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    ClientLoggerHelper.LogInfo(ex.ToString());
                    this.HideBusyIndicator();
                }
            });

            t.Start();
        }

        public void hplDelete_Click(object datacontext)
        {
            if (datacontext != null)
            {
                DrugDeptSellingPriceList p = (datacontext as DrugDeptSellingPriceList);

                switch (p.PriceListType)
                {
                    case "PriceList-InUse":
                        {
                            MessageBox.Show(eHCMSResources.A0552_G1_Msg_InfoKhDcXoaGiaDangApDung, eHCMSResources.A0480_G1_Msg_XoaBGia, MessageBoxButton.OK);
                            break;
                        }
                    case "PriceList-InFuture":
                        {
                            if (MessageBox.Show(string.Format(eHCMSResources.Z0476_G1_BanMuonXoa, eHCMSResources.K1021_G1_BGia), eHCMSResources.G2617_G1_Xoa, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                            {
                                DrugDeptSellingPriceList_Delete(p.DrugDeptSellingPriceListID);
                            }
                            break;
                        }
                    case "PriceList-Old":
                        {
                            MessageBox.Show(eHCMSResources.A0206_G1_Msg_InfoKhDcXoaBGiaCu, eHCMSResources.A0480_G1_Msg_XoaBGia, MessageBoxButton.OK);
                            break;
                        }
                }
            }
        }

        private void DrugDeptSellingPriceList_Delete(long DrugDeptSellingPriceListID)
        {
            string Result = "";
            this.ShowBusyIndicator(eHCMSResources.Z0492_G1_DangXoa);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyDrugServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginDrugDeptSellingPriceList_Delete(DrugDeptSellingPriceListID, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                contract.EndDrugDeptSellingPriceList_Delete(out Result, asyncResult);
                                switch (Result)
                                {
                                    case "Delete-1":
                                        {
                                            ObjDrugDeptSellingPriceList_GetList_Paging.PageIndex = 0;
                                            IsCheck = true;
                                            DrugDeptSellingPriceList_GetList_Paging(0, ObjDrugDeptSellingPriceList_GetList_Paging.PageSize, true);
                                            MessageBox.Show(eHCMSResources.A0478_G1_Msg_InfoXoaOK, eHCMSResources.A0480_G1_Msg_XoaBGia, MessageBoxButton.OK);
                                            break;
                                        }
                                    case "PriceList-InUse-Old":
                                        {
                                            MessageBox.Show(eHCMSResources.A0552_G1_Msg_InfoKhDcXoaGiaDangApDung, eHCMSResources.A0480_G1_Msg_XoaBGia, MessageBoxButton.OK);
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

        public void hplEdit_Click(object selectItem)
        {
            if (selectItem != null)
            {
                DrugDeptSellingPriceList Objtmp = (selectItem as DrugDeptSellingPriceList);

                string LoaiBangGia = "";
                string TieuDe = "";
                bool dpEffectiveDate_IsEnabled = true;
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
                void onInitDlg(IDrugDeptSellingPriceList_AddEdit typeInfo)
                {
                    typeInfo.TitleForm = TieuDe + " " + Objtmp.PriceListTitle.Trim() + " (" + LoaiBangGia + ")";
                    typeInfo.V_MedProductType = V_MedProductType;
                    typeInfo.ObjDrugDeptSellingPriceList_Current = ObjectCopier.DeepCopy(Objtmp);
                    typeInfo.BeginDate = curDate;
                    typeInfo.dpEffectiveDate_IsEnabled = dpEffectiveDate_IsEnabled;
                }
                GlobalsNAV.ShowDialog<IDrugDeptSellingPriceList_AddEdit>(onInitDlg, null, false, true, Globals.GetDefaultDialogViewSize());
            }
        }

        public void Handle(SaveEvent<bool> message)
        {
            if (message != null)
            {
                if (message.Result)
                {
                    ObjDrugDeptSellingPriceList_GetList_Paging.PageIndex = 0;
                    IsCheck = true;
                    DrugDeptSellingPriceList_GetList_Paging(0, ObjDrugDeptSellingPriceList_GetList_Paging.PageSize, true);
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
                ObjDrugDeptSellingPriceList_GetList_Paging.PageIndex = 0;
                DrugDeptSellingPriceList_GetList_Paging(0, ObjDrugDeptSellingPriceList_GetList_Paging.PageSize, true);
            }
            else
            {
                ObjDrugDeptSellingPriceList_GetList_Paging.Clear();
            }
        }

        public void hplViewPrint_Click(object selectItem)
        {
            if (selectItem != null)
            {
                DrugDeptSellingPriceList Objtmp = (selectItem as DrugDeptSellingPriceList);

                switch (Objtmp.V_MedProductType)
                {
                    case (long)AllLookupValues.MedProductType.THUOC:
                        {
                            void onInitDlg(ICommonPreviewView proAlloc)
                            {
                                proAlloc.V_MedProductType = Objtmp.V_MedProductType;
                                proAlloc.TieuDeRpt = string.Format("{0} ", eHCMSResources.Z0695_G1_BGiaThuocTh) + Objtmp.RecCreatedDate.Month.ToString() + "/" + Objtmp.RecCreatedDate.Year.ToString();
                                proAlloc.DrugDeptSellingPriceListID = Objtmp.DrugDeptSellingPriceListID;
                                proAlloc.eItem = ReportName.RptBangGiaThuocKhoaDuoc;
                            }
                            GlobalsNAV.ShowDialog<ICommonPreviewView>(onInitDlg, null, false, true, Globals.GetDefaultDialogViewSize());
                            break;
                        }
                    case (long)AllLookupValues.MedProductType.Y_CU:
                        {
                            void onInitDlg(ICommonPreviewView proAlloc)
                            {
                                proAlloc.TieuDeRpt = string.Format("{0} ", eHCMSResources.K1021_G1_BGia) + Globals.GetTextV_MedProductType(Objtmp.V_MedProductType) + string.Format(" {0} ", eHCMSResources.G0039_G1_Th) + Objtmp.RecCreatedDate.Month.ToString() + "/" + Objtmp.RecCreatedDate.Year.ToString();
                                proAlloc.TenYCuHoaChat = eHCMSResources.Z0696_G1_TenYCu;
                                proAlloc.TenYCuHoaChatTiengViet = eHCMSResources.Z0697_G1_TenYCuTiengViet;
                                proAlloc.V_MedProductType = Objtmp.V_MedProductType;
                                proAlloc.DrugDeptSellingPriceListID = Objtmp.DrugDeptSellingPriceListID;
                                proAlloc.eItem = ReportName.RptBangGiaYCuHoaChatKhoaDuoc;
                            }
                            GlobalsNAV.ShowDialog<ICommonPreviewView>(onInitDlg, null, false, true, Globals.GetDefaultDialogViewSize());
                            break;
                        }
                    case (long)AllLookupValues.MedProductType.HOA_CHAT:
                        {
                            void onInitDlg(ICommonPreviewView proAlloc)
                            {
                                proAlloc.TieuDeRpt = string.Format("{0} ", eHCMSResources.K1021_G1_BGia) + Globals.GetTextV_MedProductType(Objtmp.V_MedProductType) + string.Format(" {0} ", eHCMSResources.G0039_G1_Th) + Objtmp.RecCreatedDate.Month.ToString() + "/" + Objtmp.RecCreatedDate.Year.ToString();
                                proAlloc.TenYCuHoaChat = eHCMSResources.Z0698_G1_TenHChat;
                                proAlloc.TenYCuHoaChatTiengViet = eHCMSResources.Z0699_G1_TenHChatTiengViet;
                                proAlloc.V_MedProductType = Objtmp.V_MedProductType;
                                proAlloc.DrugDeptSellingPriceListID = Objtmp.DrugDeptSellingPriceListID;
                                proAlloc.eItem = ReportName.RptBangGiaYCuHoaChatKhoaDuoc;
                            }
                            GlobalsNAV.ShowDialog<ICommonPreviewView>(onInitDlg, null, false, true, Globals.GetDefaultDialogViewSize());
                            break;
                        }
                }
            }
        }

        public void hplExportExcel_Click(object selectItem)
        {
            if (selectItem == null)
            {
                return;
            }
            DrugDeptSellingPriceList ObjDrugDeptSellingPriceList_Current = (selectItem as DrugDeptSellingPriceList);
            if (ObjDrugDeptSellingPriceList_Current == null || ObjDrugDeptSellingPriceList_Current.DrugDeptSellingPriceListID <= 0)
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

            ReportParameters RptParameters = new ReportParameters();
            RptParameters.ReportType = ReportType.BANG_GIA;
            RptParameters.PriceList = new PriceList
            {
                PriceListID = ObjDrugDeptSellingPriceList_Current.DrugDeptSellingPriceListID,
                PriceListType = PriceListType.BANG_GIA_KHOA_DUOC
            };

            RptParameters.Show = "BangGia";

            switch (ObjDrugDeptSellingPriceList_Current.V_MedProductType)
            {
                case (long)AllLookupValues.MedProductType.THUOC:
                    RptParameters.Show = "BangGiaThuoc";
                    break;
                case (long)AllLookupValues.MedProductType.Y_CU:
                    RptParameters.Show = "BangGiaYCu";
                    break;
                case (long)AllLookupValues.MedProductType.HOA_CHAT:
                    RptParameters.Show = "BangGiaHoaChat";
                    break;                    
            }
            ExportToExcelGeneric.Action(RptParameters, objSFD, this);
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
