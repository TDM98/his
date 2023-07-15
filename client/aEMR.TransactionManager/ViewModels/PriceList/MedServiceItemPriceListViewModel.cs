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
using aEMR.ViewContracts.Configuration;
using Caliburn.Micro;
using System.Collections.ObjectModel;
using DataEntities;
using aEMR.Common.Collections;
using aEMR.Common;
using aEMR.CommonTasks;
using eHCMSLanguage;
using aEMR.ViewContracts;
using Microsoft.Win32;
using Castle.Windsor;
using aEMR.Infrastructure.CachingUtils;
using Castle.Core.Logging;

/*
 * 20181004 #001 TNHX: Apply BusyIndicator, refactor code, add condition: have only one "bang gia tuong lai"
 * 20181007 #002 TNHX: [BM0000104] Allow selected Effectiveday when edit and refactor code
 * 20190818 #003 TNHX: [BM0013190] Add button "View/Print" for PCLExamTypePriceList
 */

namespace aEMR.Configuration.MedServiceItemPriceList.ViewModels
{
    [Export(typeof(IMedServiceItemPriceList)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class MedServiceItemPriceListViewModel : Conductor<object>, IMedServiceItemPriceList
        , IHandle<RefDepartments_BystrV_DeptTypeEvent.TreeSelectionChanged_Event>
        , IHandle<SaveEvent<bool>>
    {
        private long MedicalServiceTypeID = 0;
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

        private object _leftContent;
        public object leftContent
        {
            get
            {
                return _leftContent;
            }
            set
            {
                if (_leftContent == value)
                    return;
                _leftContent = value;
                NotifyOfPropertyChange(() => leftContent);
            }
        }

        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public MedServiceItemPriceListViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            eventArg.Subscribe(this);
            //Globals.EventAggregator.Subscribe(this);
            authorization();
            //Load UC
            var UCRefDepartments_BystrV_DeptTypeViewModel = Globals.GetViewModel<IRefDepartments_BystrV_DeptType>();
            UCRefDepartments_BystrV_DeptTypeViewModel.strV_DeptType = "7000";
            UCRefDepartments_BystrV_DeptTypeViewModel.ShowDeptLocation = false;
            UCRefDepartments_BystrV_DeptTypeViewModel.Parent = this;
            UCRefDepartments_BystrV_DeptTypeViewModel.RefDepartments_Tree();
            leftContent = UCRefDepartments_BystrV_DeptTypeViewModel;
            (this as Conductor<object>).ActivateItem(leftContent);
            //Load UC

            ObjListMonth = new ObservableCollection<ClsMonth>();
            ObjListYear = new ObservableCollection<ClsYear>();

            LoadListMonth();
            LoadListYear();

            curRefMedicalServiceTypes = new RefMedicalServiceType();
            curRefMedicalServiceTypes.MedicalServiceTypeID = -1;
            allRefMedicalServiceType = new ObservableCollection<RefMedicalServiceType>();

            SearchCriteria = new MedServiceItemPriceListSearchCriteria();
            SearchCriteria.Month = -1;
            SearchCriteria.Year = DateTime.Now.Year;

            ObjTreeNodeRefDepartments_Current = new RefDepartmentsTree();

            ObjMedServiceItemPriceList_GetList_Paging = new PagedSortableCollectionView<DataEntities.MedServiceItemPriceList>();
            ObjMedServiceItemPriceList_GetList_Paging.OnRefresh += new EventHandler<RefreshEventArgs>(ObjMedServiceItemPriceList_GetList_Paging_OnRefresh);

            //GetAllMedicalServiceTypes_SubtractPCL();
            ShowListBangGia();
        }

        void ObjMedServiceItemPriceList_GetList_Paging_OnRefresh(object sender, RefreshEventArgs e)
        {
            MedServiceItemPriceList_GetList_Paging(ObjMedServiceItemPriceList_GetList_Paging.PageIndex, ObjMedServiceItemPriceList_GetList_Paging.PageSize, false);
        }
        /*▼====: #002*/
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
        /*▲====: #002*/

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

        private RefMedicalServiceType _curRefMedicalServiceTypes;
        public RefMedicalServiceType curRefMedicalServiceTypes
        {
            get { return _curRefMedicalServiceTypes; }
            set
            {
                _curRefMedicalServiceTypes = value;
                NotifyOfPropertyChange(() => curRefMedicalServiceTypes);
                if (curRefMedicalServiceTypes != null && curRefMedicalServiceTypes.MedicalServiceTypeID > 0)
                {
                    SearchCriteria.MedicalServiceTypeID = curRefMedicalServiceTypes.MedicalServiceTypeID;
                    //ShowListBangGia();
                }
            }
        }

        private ObservableCollection<RefMedicalServiceType> _allRefMedicalServiceType;
        public ObservableCollection<RefMedicalServiceType> allRefMedicalServiceType
        {
            get { return _allRefMedicalServiceType; }
            set
            {
                _allRefMedicalServiceType = value;
                NotifyOfPropertyChange(() => allRefMedicalServiceType);
            }
        }

        public void RefMedicalServiceItems_GroupMedicalServiceTypeIDByDeptID(Int64 DeptID, int V)
        {
            allRefMedicalServiceType.Clear();
            /*▼====: #001*/
            this.ShowBusyIndicator(eHCMSResources.Z0604_G1_DangLayDSLoaiDV);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ConfigurationManagerServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginRefMedicalServiceItems_GroupMedicalServiceTypeIDByDeptID(DeptID, V, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var items = contract.EndRefMedicalServiceItems_GroupMedicalServiceTypeIDByDeptID(asyncResult);

                                if (items != null)
                                {
                                    allRefMedicalServiceType = new ObservableCollection<DataEntities.RefMedicalServiceType>(items);

                                    //Item Default
                                    RefMedicalServiceType ItemDefault = new RefMedicalServiceType();
                                    ItemDefault.MedicalServiceTypeID = -1;
                                    ItemDefault.MedicalServiceTypeName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.K2027_G1_ChonLoaiDV);
                                    //Item Default
                                    allRefMedicalServiceType.Insert(0, ItemDefault);
                                }
                                else
                                {
                                    allRefMedicalServiceType = null;
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
                /*▲====: #001*/
            });
            t.Start();
        }

        private RefDepartmentsTree _ObjTreeNodeRefDepartments_Current;
        public RefDepartmentsTree ObjTreeNodeRefDepartments_Current
        {
            get { return _ObjTreeNodeRefDepartments_Current; }
            set
            {
                _ObjTreeNodeRefDepartments_Current = value;
                NotifyOfPropertyChange(() => ObjTreeNodeRefDepartments_Current);
            }
        }

        public void Handle(RefDepartments_BystrV_DeptTypeEvent.TreeSelectionChanged_Event message)
        {
            if (message != null)
            {
                ObjTreeNodeRefDepartments_Current = message.ObjRefDepartments_Current as DataEntities.RefDepartmentsTree;

                SearchCriteria.DeptID = ObjTreeNodeRefDepartments_Current.NodeID;

                curRefMedicalServiceTypes.MedicalServiceTypeID = -1;

                SearchCriteria.MedicalServiceTypeID = -1;
                RefMedicalServiceItems_GroupMedicalServiceTypeIDByDeptID(SearchCriteria.DeptID, 1);//Subtract loại PCL
            }
        }

        private CheckPriceList checkPriceList;

        private MedServiceItemPriceListSearchCriteria _SearchCriteria;
        public MedServiceItemPriceListSearchCriteria SearchCriteria
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

        private PagedSortableCollectionView<DataEntities.MedServiceItemPriceList> _ObjMedServiceItemPriceList_GetList_Paging;
        public PagedSortableCollectionView<DataEntities.MedServiceItemPriceList> ObjMedServiceItemPriceList_GetList_Paging
        {
            get { return _ObjMedServiceItemPriceList_GetList_Paging; }
            set
            {
                _ObjMedServiceItemPriceList_GetList_Paging = value;
                NotifyOfPropertyChange(() => ObjMedServiceItemPriceList_GetList_Paging);
            }
        }

        public void authorization()
        {
            if (!Globals.isAccountCheck)
            {
                return;
            }
            bhplEdit = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mTransaction_Management
                                               , (int)eTransaction_Management.mBangGiaDichVu,
                                               (int)oTransaction_ManagementEx.mEdit, (int)ePermission.mView);
            bhplDelete = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mTransaction_Management
                                               , (int)eTransaction_Management.mBangGiaDichVu,
                                               (int)oTransaction_ManagementEx.mDelete, (int)ePermission.mView);
            bAddNew = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mTransaction_Management
                                               , (int)eTransaction_Management.mBangGiaDichVu,
                                               (int)oTransaction_ManagementEx.mAddNew, (int)ePermission.mView);
            bhplExportExcel = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mTransaction_Management
                                               , (int)eTransaction_Management.mBangGiaDichVu,
                                               (int)oTransaction_ManagementEx.mExportToExcel, (int)ePermission.mView);
        }

        #region checking account
        private bool _bhplEdit = true;
        private bool _bhplDelete = true;
        private bool _bAddNew = true;
        private bool _bhplExportExcel = true;
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

        public bool bAddNew
        {
            get
            {
                return _bAddNew;
            }
            set
            {
                if (_bAddNew == value)
                    return;
                _bAddNew = value;
            }
        }

        public bool bhplExportExcel
        {
            get
            {
                return _bhplExportExcel;
            }
            set
            {
                if (_bhplExportExcel == value)
                    return;
                _bhplExportExcel = value;
            }
        }
        #endregion

        #region binding visibilty
        public Button hplEdit { get; set; }
        public Button hplDelete { get; set; }
        public Button hplExportExcel { get; set; }

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

        public void hplExportExcel_Loaded(object sender)
        {
            hplExportExcel = sender as Button;
            hplExportExcel.Visibility = Globals.convertVisibility(bhplExportExcel);
        }
        #endregion

        private void MedServiceItemPriceList_GetList_Paging(int PageIndex, int PageSize, bool CountTotal)
        {
            /*▼====: #001*/
            this.ShowBusyIndicator(eHCMSResources.K2922_G1_DSBGia);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ConfigurationManagerServiceClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginMedServiceItemPriceList_GetList_Paging(SearchCriteria, PageIndex, PageSize, "", CountTotal, Globals.DispatchCallback((asyncResult) =>
                        {
                            int Total = 0;
                            IList<DataEntities.MedServiceItemPriceList> allItems = null;
                            bool bOK = false;
                            try
                            {
                                allItems = client.EndMedServiceItemPriceList_GetList_Paging(out Total, out DateTime currentDate, asyncResult);
                                // for some reason the output currentDate is always 01/01/0001
                                currentDate = Globals.GetCurServerDateTime();
                                curDate = currentDate;
                                NotifyOfPropertyChange(() => curDate);
                                EffectiveDay = currentDate;
                                if (MedicalServiceTypeID != SearchCriteria.MedicalServiceTypeID)
                                {
                                    checkPriceList = new CheckPriceList();
                                    foreach (var item in allItems)
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
                                    MedicalServiceTypeID = SearchCriteria.MedicalServiceTypeID;
                                }
                                bOK = true;
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

                            ObjMedServiceItemPriceList_GetList_Paging.Clear();

                            if (bOK)
                            {
                                if (CountTotal)
                                {
                                    ObjMedServiceItemPriceList_GetList_Paging.TotalItemCount = Total;
                                }
                                if (allItems != null)
                                {
                                    foreach (var item in allItems)
                                    {
                                        ObjMedServiceItemPriceList_GetList_Paging.Add(item);
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

        public void GetAllMedicalServiceTypes_SubtractPCL()
        {
            this.ShowBusyIndicator(eHCMSResources.Z0604_G1_DangLayDSLoaiDV);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ConfigurationManagerServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetAllMedicalServiceTypes_SubtractPCL(Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var items = contract.EndGetAllMedicalServiceTypes_SubtractPCL(asyncResult);

                                if (items != null)
                                {
                                    allRefMedicalServiceType = new ObservableCollection<DataEntities.RefMedicalServiceType>(items);

                                    //Item Default
                                    RefMedicalServiceType ItemDefault = new RefMedicalServiceType();
                                    ItemDefault.MedicalServiceTypeID = -1;
                                    ItemDefault.MedicalServiceTypeName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.K2027_G1_ChonLoaiDV);
                                    //Item Default

                                    allRefMedicalServiceType.Insert(0, ItemDefault);
                                }
                                else
                                {
                                    allRefMedicalServiceType = null;
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
                /*▲====: #001*/
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

        public void cboMedicalServiceTypesSubTractPCL_SelectionChanged(object selectItem)
        {
            //SearchCriteria.MedicalServiceTypeID = curRefMedicalServiceTypes.MedicalServiceTypeID;
            //ShowListBangGia();
        }

        private void ShowListBangGia()
        {
            ObjMedServiceItemPriceList_GetList_Paging.PageIndex = 0;
            MedServiceItemPriceList_GetList_Paging(0, ObjMedServiceItemPriceList_GetList_Paging.PageSize, true);
        }

        public void btAddNew()
        {
            if (EffectiveDay.Date < curDate.Date)
            {
                MessageBox.Show(eHCMSResources.A0682_G1_Msg_InfoKhTaoDcBGiaChoQKhu
                    , eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
            //if (curRefMedicalServiceTypes.MedicalServiceTypeID < 0)
            //{
            //    MessageBox.Show(eHCMSResources.A0388_G1_Msg_InfoChuaChonLoaiDV
            //        , eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
            //    return;
            //}
            if (EffectiveDay > curDate && checkPriceList != null && checkPriceList.hasFur)
            {
                MessageBox.Show(string.Format("Đã tồn tại bảng giá /{0}/, ngày áp dụng: {1} \nKhông được tạo bảng giá mới khác! Vui lòng chọn giá và cập nhật lại!"
                    , checkPriceList.furTitle, checkPriceList.furDay.ToShortDateString()), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
            Action<IMedServiceItemPriceList_AddEdit> onInitDlg = delegate (IMedServiceItemPriceList_AddEdit typeInfo)
            {
                typeInfo.TitleForm = eHCMSResources.T0780_G1_TaoBGiaMoi;
                typeInfo.BeginDate = EffectiveDay;
                typeInfo.InitializeNewItem(1); //TBL: Gan cung du lieu de khong kiem tra co chon loai dich vu chua
                typeInfo.MedServiceItemTypeName = curRefMedicalServiceTypes.MedicalServiceTypeName;
                /*▼====: #002*/
                if (EffectiveDay.Date == curDate.Date)
                {
                    typeInfo.dpEffectiveDate_IsEnabled = false;
                }
                /*▲====: #002*/
            };
            GlobalsNAV.ShowDialog<IMedServiceItemPriceList_AddEdit>(onInitDlg);
        }

        public void hplAddNew()
        {
            Int64 DeptID = -1;
            Int64 MedicalServiceTypeID = -1;

            if (ObjTreeNodeRefDepartments_Current.NodeID > 0 && ObjTreeNodeRefDepartments_Current.ParentID > 0)
                DeptID = ObjTreeNodeRefDepartments_Current.NodeID;

            if (curRefMedicalServiceTypes.MedicalServiceTypeID > 0)
                MedicalServiceTypeID = curRefMedicalServiceTypes.MedicalServiceTypeID;

            if (DeptID > 0 && MedicalServiceTypeID > 0)
            {
                MedServiceItemPriceList_CheckCanAddNew(DeptID, MedicalServiceTypeID);
            }
            else
            {
                MessageBox.Show(eHCMSResources.A0319_G1_Msg_ChonKhoa_LoaiDVDetaoBGia, eHCMSResources.T0780_G1_TaoBGiaMoi, MessageBoxButton.OK);
            }
        }

        private void MedServiceItemPriceList_CheckCanAddNew(Int64 DeptID, Int64 MedicalServiceTypeID)
        {
            bool CanAddNew = false;
            /*▼====: #001*/
            this.ShowBusyIndicator(eHCMSResources.Z0840_G1_DangKTra);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ConfigurationManagerServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginMedServiceItemPriceList_CheckCanAddNew(DeptID, MedicalServiceTypeID, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                contract.EndMedServiceItemPriceList_CheckCanAddNew(out CanAddNew, asyncResult);
                                if (CanAddNew)
                                {
                                    Action<IMedServiceItemPriceList_AddEdit> onInitDlg = delegate (IMedServiceItemPriceList_AddEdit typeInfo)
                                    {
                                        typeInfo.TitleForm = eHCMSResources.T0780_G1_TaoBGiaMoi;
                                        // 20181007 TNHX: [BM0000104] Allow selected Effectiveday when edit and refactor code
                                        typeInfo.InitializeNewItem(MedicalServiceTypeID);
                                    };
                                    GlobalsNAV.ShowDialog<IMedServiceItemPriceList_AddEdit>(onInitDlg);
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
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    this.HideBusyIndicator();
                }
                /*▲====: #001*/
            });
            t.Start();
        }

        public void hplDelete_Click(object datacontext)
        {
            if (datacontext != null)
            {
                DataEntities.MedServiceItemPriceList p = (datacontext as DataEntities.MedServiceItemPriceList);

                switch (p.PriceListType)
                {
                    case "PriceList-InUse":
                        {
                            MessageBox.Show(eHCMSResources.A0206_G1_Msg_InfoKhDcXoaBGiaCu, eHCMSResources.A0480_G1_Msg_XoaBGia, MessageBoxButton.OK);
                            break;
                        }
                    case "PriceList-InFuture":
                        {
                            if (
                                MessageBox.Show(eHCMSResources.A0152_G1_Msg_ConfXoaBGia, eHCMSResources.G2617_G1_Xoa, MessageBoxButton.OKCancel) ==
                                MessageBoxResult.OK)
                            {
                                MedServiceItemPriceList_MarkDelete(p.MedServiceItemPriceListID);
                            }
                            break;
                        }
                    case "PriceList-Old":
                        {
                            MessageBox.Show("Bảng Giá Cũ! Không Được Phép Xóa!", eHCMSResources.A0480_G1_Msg_XoaBGia, MessageBoxButton.OK);
                            break;
                        }
                }
            }
        }
        private void MedServiceItemPriceList_MarkDelete(Int64 MedServiceItemPriceListID)
        {
            string Result = "";
            /*▼====: #001*/
            this.ShowBusyIndicator(eHCMSResources.Z0492_G1_DangXoa);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ConfigurationManagerServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginMedServiceItemPriceList_MarkDelete(MedServiceItemPriceListID, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                contract.EndMedServiceItemPriceList_MarkDelete(out Result, asyncResult);
                                switch (Result)
                                {
                                    case "Delete-1":
                                        {
                                            ObjMedServiceItemPriceList_GetList_Paging.PageIndex = 0;
                                            MedServiceItemPriceList_GetList_Paging(0, ObjMedServiceItemPriceList_GetList_Paging.PageSize, true);
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
                /*▲====: #001*/
            });
            t.Start();
        }

        public void btSearch()
        {
            if (ObjTreeNodeRefDepartments_Current.NodeID > 0)
            {
                if (SearchCriteria.MedicalServiceTypeID > 0)
                {
                    ObjMedServiceItemPriceList_GetList_Paging.PageIndex = 0;
                    MedServiceItemPriceList_GetList_Paging(0, ObjMedServiceItemPriceList_GetList_Paging.PageSize, true);
                }
                else
                {
                    MessageBox.Show(eHCMSResources.A0327_G1_Msg_InfoChonLoaiDV, eHCMSResources.G1174_G1_TimKiem, MessageBoxButton.OK);
                }
            }
            else
            {
                MessageBox.Show(eHCMSResources.A0320_G1_Msg_InfoChonKhoaKhacNutGoc, eHCMSResources.G1174_G1_TimKiem, MessageBoxButton.OK);
            }
        }

        public void hplEdit_Click(object selectItem)
        {
            if (selectItem != null)
            {
                DataEntities.MedServiceItemPriceList Objtmp = (selectItem as DataEntities.MedServiceItemPriceList);

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
                Action<IMedServiceItemPriceList_AddEdit> onInitDlg = delegate (IMedServiceItemPriceList_AddEdit typeInfo)
                {
                    typeInfo.TitleForm = TieuDe + " " + Objtmp.PriceListTitle.Trim() + " (" + LoaiBangGia + ")";
                    typeInfo.ObjMedServiceItemPriceList_Current = ObjectCopier.DeepCopy(Objtmp);
                    //TBL: Luc truoc do khi chon loai dich vu da load san cac dich vu cua loai do nen khong can
                    typeInfo.ObjMedServiceItemPriceList_Current.MedicalServiceTypeID = -1;
                    typeInfo.BeginDate = curDate;
                    typeInfo.dpEffectiveDate_IsEnabled = dpEffectiveDate_IsEnabled;
                    typeInfo.MedServiceItemTypeName = Objtmp.ObjRefMedicalServiceType.MedicalServiceTypeName;
                };
                GlobalsNAV.ShowDialog<IMedServiceItemPriceList_AddEdit>(onInitDlg);
            }
        }

        public void Handle(SaveEvent<bool> message)
        {
            if (message != null)
            {
                if (message.Result)
                {
                    ObjMedServiceItemPriceList_GetList_Paging.PageIndex = 0;
                    MedServiceItemPriceList_GetList_Paging(0, ObjMedServiceItemPriceList_GetList_Paging.PageSize, true);
                }
            }
        }
        public void dtgList_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            DataEntities.MedServiceItemPriceList objRows = e.Row.DataContext as DataEntities.MedServiceItemPriceList;
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

        public void hplExportExcel_Click(object selectItem)
        {
            if (selectItem == null)
            {
                return;
            }
            DataEntities.MedServiceItemPriceList ObjMedServiceItemPriceList_Current = (selectItem as DataEntities.MedServiceItemPriceList);
            if (ObjMedServiceItemPriceList_Current == null || ObjMedServiceItemPriceList_Current.MedServiceItemPriceListID <= 0)
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
                PriceListID = ObjMedServiceItemPriceList_Current.MedServiceItemPriceListID,
                PriceListType = PriceListType.BANG_GIA_DV
            };
            RptParameters.Show = "BangGia";

            ExportToExcelGeneric.Action(RptParameters, objSFD, this);
            //Coroutine.BeginExecute(DoSaveExcel(RptParameters, objSFD));
        }

        //▼====: #003
        public void ViewPrint_Click(object selectItem)
        {
            if (selectItem == null)
            {
                return;
            }
            DataEntities.MedServiceItemPriceList ObjMedServiceItemPriceList_Current = (selectItem as DataEntities.MedServiceItemPriceList);
            if (ObjMedServiceItemPriceList_Current == null || ObjMedServiceItemPriceList_Current.MedServiceItemPriceListID <= 0)
            {
                MessageBox.Show(eHCMSResources.Z2799_G1_ChonBangGiaDeXemIn);
                return;
            }

            Action<ICommonPreviewView> onInitDlg = delegate (ICommonPreviewView proAlloc)
            {
                proAlloc.ID = ObjMedServiceItemPriceList_Current.MedServiceItemPriceListID;
                proAlloc.eItem = ReportName.RptBangGiaDV;
            };
            GlobalsNAV.ShowDialog(onInitDlg);
        }
        //▲====: #003
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
