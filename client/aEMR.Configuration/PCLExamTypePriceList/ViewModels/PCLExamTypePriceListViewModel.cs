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
using aEMR.ViewContracts.Configuration;
using Caliburn.Micro;
using System.Collections.ObjectModel;
using DataEntities;
using aEMR.Common.Collections;
using aEMR.Common;
using eHCMSLanguage;
using aEMR.CommonTasks;
using Microsoft.Win32;
using Castle.Windsor;

namespace aEMR.Configuration.PCLExamTypePriceList.ViewModels
{
    [Export(typeof(IPCLExamTypePriceList)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class PCLExamTypePriceListViewModel : Conductor<object>, IPCLExamTypePriceList
          , IHandle<SaveEvent<bool>>
    {
        private bool _isLoading;
        public bool IsLoading
        {
            get { return _isLoading; }
            set
            {
                if (_isLoading != value)
                {
                    _isLoading = value;
                    NotifyOfPropertyChange(() => IsLoading);
                }
            }
        }
        public bool IsCheck { get; set; }

        private CheckPriceList checkPriceList;
        [ImportingConstructor]
        public PCLExamTypePriceListViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg)
        {
            Globals.EventAggregator.Subscribe(this);
            authorization();

            ObjListMonth = new ObservableCollection<ClsMonth>();
            ObjListYear = new ObservableCollection<ClsYear>();

            LoadListMonth();
            LoadListYear();

            SearchCriteria = new PCLExamTypePriceListSearchCriteria();
            SearchCriteria.Month = -1;
            SearchCriteria.Year = DateTime.Now.Year;

            ObjPCLExamTypePriceList_GetList_Paging = new PagedSortableCollectionView<DataEntities.PCLExamTypePriceList>();
            ObjPCLExamTypePriceList_GetList_Paging.OnRefresh += new EventHandler<RefreshEventArgs>(ObjPCLExamTypePriceList_GetList_Paging_OnRefresh);

            ObjPCLExamTypePriceList_GetList_Paging.PageIndex = 0;
            PCLExamTypePriceList_GetList_Paging(0, ObjPCLExamTypePriceList_GetList_Paging.PageSize, true);

        }

        void ObjPCLExamTypePriceList_GetList_Paging_OnRefresh(object sender, RefreshEventArgs e)
        {
            PCLExamTypePriceList_GetList_Paging(ObjPCLExamTypePriceList_GetList_Paging.PageIndex, ObjPCLExamTypePriceList_GetList_Paging.PageSize, false);
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
            bhplEdit = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConfiguration_Management
                                               , (int)eConfiguration_Management.mBangGiaPCLExamType,
                                               (int)oConfigurationEx.mQuanLyBGPCLExamType, (int)ePermission.mEdit);
            bhplDelete = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConfiguration_Management
                                               , (int)eConfiguration_Management.mBangGiaPCLExamType,
                                               (int)oConfigurationEx.mQuanLyBGPCLExamType, (int)ePermission.mDelete);
            bhplAddNew = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConfiguration_Management
                                            , (int)eConfiguration_Management.mBangGiaPCLExamType,
                                            (int)oConfigurationEx.mQuanLyBGPCLExamType, (int)ePermission.mAdd);
            bbtSearch = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConfiguration_Management
                                            , (int)eConfiguration_Management.mBangGiaPCLExamType,
                                            (int)oConfigurationEx.mQuanLyBGPCLExamType, (int)ePermission.mView);
        }
        #region checking account

        private bool _bhplEdit = true;
        private bool _bhplDelete = true;
        private bool _bhplAddNew = true;
        private bool _bbtSearch = true;
        public bool bhplEdit
        {
            get
            {
                return _bhplEdit;
            }
            set
            {
                if (_bhplEdit == value)
                    return;
                _bhplEdit = value;
            }
        }
        public bool bhplDelete
        {
            get
            {
                return _bhplDelete;
            }
            set
            {
                if (_bhplDelete == value)
                    return;
                _bhplDelete = value;
            }
        }
        public bool bhplAddNew
        {
            get
            {
                return _bhplAddNew;
            }
            set
            {
                if (_bhplAddNew == value)
                    return;
                _bhplAddNew = value;
            }
        }
        public bool bbtSearch
        {
            get
            {
                return _bbtSearch;
            }
            set
            {
                if (_bbtSearch == value)
                    return;
                _bbtSearch = value;
            }
        }


        #endregion
        #region binding visibilty

        public Button hplEdit { get; set; }
        public Button hplDelete { get; set; }
        public void hplEdit_Loaded(object sender)
        {
            hplEdit = sender as Button;
            hplEdit.Visibility = Globals.convertVisibility(bhplEdit);
        }
        public void hplDelete_Loaded(object sender)
        {
            hplDelete = sender as Button;
            hplDelete.Visibility = Globals.convertVisibility(bhplDelete);
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
                    // 20181007 TNHX: [BM0000104] Allow selected Effectiveday when edit and refactor code
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

            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "Đang Kiểm Tra..." });

            var t = new Thread(() =>
            {
                IsLoading = true;

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
                                    // 20181007 TNHX: [BM0000104] Allow selected Effectiveday when edit and refactor code
                                    typeInfo.InitializeNewItem();
                                };
                                GlobalsNAV.ShowDialog<IPCLExamTypePriceList_AddEdit>(onInitDlg);
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
                            //Globals.IsBusy = false;
                            IsLoading = false;
                        }
                    }), null);
                }


            });
            t.Start();
        }


        private PCLExamTypePriceListSearchCriteria _SearchCriteria;
        public PCLExamTypePriceListSearchCriteria SearchCriteria
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

        private PagedSortableCollectionView<DataEntities.PCLExamTypePriceList> _ObjPCLExamTypePriceList_GetList_Paging;
        public PagedSortableCollectionView<DataEntities.PCLExamTypePriceList> ObjPCLExamTypePriceList_GetList_Paging
        {
            get { return _ObjPCLExamTypePriceList_GetList_Paging; }
            set
            {
                _ObjPCLExamTypePriceList_GetList_Paging = value;
                NotifyOfPropertyChange(() => ObjPCLExamTypePriceList_GetList_Paging);
            }
        }

        private void PCLExamTypePriceList_GetList_Paging(int PageIndex, int PageSize, bool CountTotal)
        {
            //Globals.EventAggregator.Publish(new BusyEvent() { IsBusy = true, Message = "Danh Sách Bảng Giá..." });

            var t = new Thread(() =>
            {
                IsLoading = true;

                try
                {
                    using (var serviceFactory = new ConfigurationManagerServiceClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginPCLExamTypePriceList_GetList_Paging(SearchCriteria, PageIndex, PageSize, "", CountTotal, Globals.DispatchCallback((asyncResult) =>
                        {
                            int Total = 0;
                            IList<DataEntities.PCLExamTypePriceList> allItems = null;
                            bool bOK = false;
                            try
                            {
                                DateTime currentDate;
                                allItems = client.EndPCLExamTypePriceList_GetList_Paging(out Total, out currentDate, asyncResult);
                                curDate = currentDate;
                                NotifyOfPropertyChange(() => curDate);
                                EffectiveDay = currentDate;
                                bOK = true;
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
                                                checkPriceList.curDay = (DateTime)item.EffectiveDate;
                                                checkPriceList.curTitle = item.PriceListTitle;
                                            }
                                            else if (((DateTime)item.EffectiveDate).Date > curDate.Date)
                                            {
                                                checkPriceList.hasFur = true;
                                                checkPriceList.furDay = (DateTime)item.EffectiveDate;
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

                            ObjPCLExamTypePriceList_GetList_Paging.Clear();

                            if (bOK)
                            {
                                if (CountTotal)
                                {
                                    ObjPCLExamTypePriceList_GetList_Paging.TotalItemCount = Total;
                                }
                                if (allItems != null)
                                {
                                    foreach (var item in allItems)
                                    {
                                        ObjPCLExamTypePriceList_GetList_Paging.Add(item);
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
                    //Globals.IsBusy = false;
                    IsLoading = false;
                }
            });
            t.Start();
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
                ObjPCLExamTypePriceList_GetList_Paging.PageIndex = 0;
                PCLExamTypePriceList_GetList_Paging(0, ObjPCLExamTypePriceList_GetList_Paging.PageSize, true);
            }
            else
            {
                ObjPCLExamTypePriceList_GetList_Paging.Clear();
            }
        }

        public void hplDelete_Click(object datacontext)
        {
            if (datacontext != null)
            {
                DataEntities.PCLExamTypePriceList p = (datacontext as DataEntities.PCLExamTypePriceList);

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
                                PCLExamTypePriceList_Delete(p.PCLExamTypePriceListID);
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
        private void PCLExamTypePriceList_Delete(Int64 PCLExamTypePriceListID)
        {
            string Result = "";

            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "Đang Xóa..." });

            var t = new Thread(() =>
            {
                IsLoading = true;

                using (var serviceFactory = new ConfigurationManagerServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginPCLExamTypePriceList_Delete(PCLExamTypePriceListID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            contract.EndPCLExamTypePriceList_Delete(out Result, asyncResult);
                            switch (Result)
                            {
                                case "Delete-1":
                                    {
                                        ObjPCLExamTypePriceList_GetList_Paging.PageIndex = 0;
                                        IsCheck = true;
                                        PCLExamTypePriceList_GetList_Paging(0, ObjPCLExamTypePriceList_GetList_Paging.PageSize, true);
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
                            //Globals.IsBusy = false;
                            IsLoading = false;
                        }
                    }), null);
                }


            });
            t.Start();
        }

        public void hplEdit_Click(object selectItem)
        {
            if (selectItem != null)
            {
                DataEntities.PCLExamTypePriceList Objtmp = (selectItem as DataEntities.PCLExamTypePriceList);
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
                Action<IPCLExamTypePriceList_AddEdit> onInitDlg = delegate (IPCLExamTypePriceList_AddEdit typeInfo)
                {
                    typeInfo.TitleForm = TieuDe + " " + Objtmp.PriceListTitle.Trim() + " (" + LoaiBangGia + ")";
                    typeInfo.ObjPCLExamTypePriceList_Current = ObjectCopier.DeepCopy(Objtmp);
                    typeInfo.BeginDate = curDate;
                    typeInfo.dpEffectiveDate_IsEnabled = dpEffectiveDate_IsEnabled;
                };
                GlobalsNAV.ShowDialog<IPCLExamTypePriceList_AddEdit>(onInitDlg);
            }
        }
        public void dtgList_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            DataEntities.PCLExamTypePriceList objRows = e.Row.DataContext as DataEntities.PCLExamTypePriceList;
            if (objRows != null)
            {
                if (((DateTime)objRows.EffectiveDate).Date > curDate.Date)
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
        public void Handle(SaveEvent<bool> message)
        {
            if (message != null)
            {
                if (message.Result)
                {
                    IsCheck = true;
                    ObjPCLExamTypePriceList_GetList_Paging.PageIndex = 0;
                    PCLExamTypePriceList_GetList_Paging(0, ObjPCLExamTypePriceList_GetList_Paging.PageSize, true);
                }
            }
        }

        public void btSearch()
        {
            ShowListBangGia();
        }

        public void hplExportExcel_Click(object selectItem)
        {
            if (selectItem == null)
            {
                return;
            }

            DataEntities.PCLExamTypePriceList ObjPCLExamTypeItemPriceList_Current = (selectItem as DataEntities.PCLExamTypePriceList);

            if (ObjPCLExamTypeItemPriceList_Current == null || ObjPCLExamTypeItemPriceList_Current.PCLExamTypePriceListID <= 0)
            {
                MessageBox.Show(eHCMSResources.K0275_G1_ChonBGiaDeXuatExcel);
                return;
            }
            SaveFileDialog objSFD = new SaveFileDialog();
            if (Globals.ServerConfigSection.CommonItems.ApplyNewFuncExportExcel)
            {
                objSFD = new SaveFileDialog()
                {
                    DefaultExt = ".xlsx",
                    //Filter = "Excel xls (*.xls)|*.xls",
                    //Filter = "Excel (2003)(.xls)|*.xls|Excel (2010) (.xlsx)|*.xlsx |RichText File (.rtf)|*.rtf |Pdf File (.pdf)|*.pdf |Html File (.html)|*.html",
                    Filter = "Excel(2003) (.xls)|*.xls|Excel(2010) (.xlsx)|*.xlsx",
                    FilterIndex = 2
                };
            }
            else
            {
                objSFD = new SaveFileDialog()
                {
                    DefaultExt = ".xls",
                    Filter = "Excel xls (*.xls)|*.xls",
                    //Filter = "Excel (2003)(.xls)|*.xls|Excel (2010) (.xlsx)|*.xlsx |RichText File (.rtf)|*.rtf |Pdf File (.pdf)|*.pdf |Html File (.html)|*.html",
                    FilterIndex = 1
                };
            }
            if (objSFD.ShowDialog() != true)
            {
                return;
            }

            ReportParameters RptParameters = new ReportParameters();
            RptParameters.ReportType = ReportType.BANG_GIA;
            RptParameters.PriceList = new PriceList
            {
                PriceListID = ObjPCLExamTypeItemPriceList_Current.PCLExamTypePriceListID,
                PriceListType = PriceListType.BANG_GIA_PCL
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

