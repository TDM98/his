using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
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
using DataEntities;
using aEMR.Common.Collections;
using aEMR.Common;
using eHCMSLanguage;
using aEMR.ViewContracts;
using Castle.Windsor;
using aEMR.Controls;
using System.Data;
using aEMR.Common.BaseModel;
using aEMR.Infrastructure.CachingUtils;
using Castle.Core.Logging;
using System.Windows.Data;
using System.Windows.Input;
/*
* 20170328 #001 CMN: Fix price must be greater than 0
* 20180720 #002 CMN: Get textblock from cell content
* 20181004 #003 TNHX: Add Busy Indicator and refactor code
* 20181007 #004 TNHX: [BM0000104] Allow selected Effectiveday when edit and refactor code
* 20190620 #005 TBL: BM 0006759. Lam chuc nang tim kiem 
*/
namespace aEMR.Configuration.MedServiceItemPriceList.ViewModels
{
    [Export(typeof(IMedServiceItemPriceList_AddEdit)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class MedServiceItemPriceList_AddEditViewModel : ViewModelBase, IMedServiceItemPriceList_AddEdit
        , IHandle<SelectedObjectEvent<MedServiceItemPrice>>
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public MedServiceItemPriceList_AddEditViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg)
        {
            allRefMedicalServiceType = new ObservableCollection<RefMedicalServiceType>();
            GetAllMedicalServiceTypes_SubtractPCL();
        }

        private DateTime _EndDate;
        public DateTime EndDate
        {
            get { return _EndDate; }
            set
            {
                if (_EndDate != value)
                {
                    _EndDate = value;
                    NotifyOfPropertyChange(() => EndDate);
                }
            }
        }

        private DateTime _BeginDate;
        public DateTime BeginDate
        {
            get { return _BeginDate; }
            set
            {
                if (_BeginDate != value)
                {
                    _BeginDate = value;
                    NotifyOfPropertyChange(() => BeginDate);
                }
            }
        }

        private string _MedServiceItemTypeName;
        public string MedServiceItemTypeName
        {
            get { return _MedServiceItemTypeName; }
            set
            {
                if (_MedServiceItemTypeName != value)
                {
                    _MedServiceItemTypeName = value;
                    NotifyOfPropertyChange(() => MedServiceItemTypeName);
                }
            }
        }

        private int VirList_IndexCurrent = -1;//ban đầu phân trang chạy từ 0
        private bool VirList_IsEndPage = false;

        private DateTime EffectiveDate_tmp;
        private DateTime dtCurrentServer;
        
        protected override void OnActivate()
        {
            Globals.EventAggregator.Subscribe(this);

            base.OnActivate();

            // TxD 02/08/2014: initialized dtCurrentServer because its value is ONLY set in GetCurrentDate and that method is not called anywhere thus get commented out
            dtCurrentServer = Globals.GetCurServerDateTime();

            VirList_IndexCurrent = -1;
            VirList_IsEndPage = false;

            SearchCriteria = new MedServiceItemPriceSearchCriteria();
            allMedServiceItemPrice_Paging = new ObservableCollection<MedServiceItemPrice>();
            //allMedServiceItemPrice_Paging.PageSize = 100;
            //allMedServiceItemPrice_Paging.OnRefresh += new EventHandler<RefreshEventArgs>(allMedServiceItemPrice_Paging_OnRefresh);
            LoadList();
            ObjGetDeptMedServiceItems_Paging_Virtual = new PagedSortableCollectionView<MedServiceItemPrice>();
            ObjGetDeptMedServiceItems_Paging_Save = new PagedSortableCollectionView<MedServiceItemPrice>();

            ObjGetDeptMedServiceItems_All_Virtual_Delete = new ObservableCollection<MedServiceItemPrice>();

            //RefDepartments_RecursiveByDeptID();

            if (ObjMedServiceItemPriceList_Current.MedServiceItemPriceListID > 0)//Update
            {
                btSave_IsEnabled = true;
                EffectiveDate_tmp = ObjMedServiceItemPriceList_Current.EffectiveDate;
                dgCellTemplate0_Visible = Visibility.Collapsed;
                btChooseItemFromDelete_IsEnabled = false;
                if (!ObjMedServiceItemPriceList_Current.IsActive
                    && ObjMedServiceItemPriceList_Current.EffectiveDate < BeginDate)
                {
                    MessageBox.Show(eHCMSResources.A0205_G1_Msg_InfoKhDcSuaBGiaCu, eHCMSResources.Z0665_G1_SuaBGia, MessageBoxButton.OK);
                    btSave_IsEnabled = false;
                }
                else
                {
                    btSave_IsEnabled = true;
                }
            }
            else
            {
                dgCellTemplate0_Visible = Visibility.Visible;
                /*▼====: #004*/
                //dpEffectiveDate_IsEnabled = false;
                /*▲====: #004*/
                btChooseItemFromDelete_IsEnabled = false;
                btSave_IsEnabled = true;
            }
        }
        
        private void PagingLinq(int pIndex, int pPageSize)
        {
            var ResultAll = from p in ObjGetDeptMedServiceItems_Paging_Virtual.ToObservableCollection()
                            select p;
            List<MedServiceItemPrice> Items = ResultAll.Skip(pIndex * pPageSize).Take(pPageSize).ToList();
            ShowItemsOnList(Items);
        }

        private void ShowItemsOnList(List<MedServiceItemPrice> ObjCollect)
        {
            allMedServiceItemPrice_Paging.Clear();
            foreach (MedServiceItemPrice item in ObjCollect)
            {
                if (item.RowState != MedServiceItemPrice.RowStateValue.Delete)
                {
                    allMedServiceItemPrice_Paging.Add(item);
                }
            }
        }

        private DataEntities.MedServiceItemPriceList _ObjMedServiceItemPriceList_Current;
        public DataEntities.MedServiceItemPriceList ObjMedServiceItemPriceList_Current
        {
            get { return _ObjMedServiceItemPriceList_Current; }
            set
            {
                _ObjMedServiceItemPriceList_Current = value;
                NotifyOfPropertyChange(() => ObjMedServiceItemPriceList_Current);
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

        private ObservableCollection<RefDepartments> _ObjRefDepartments_RecursiveByDeptID;
        public ObservableCollection<RefDepartments> ObjRefDepartments_RecursiveByDeptID
        {
            get { return _ObjRefDepartments_RecursiveByDeptID; }
            set
            {
                _ObjRefDepartments_RecursiveByDeptID = value;
                NotifyOfPropertyChange(() => ObjRefDepartments_RecursiveByDeptID);
            }
        }

        public void RefDepartments_RecursiveByDeptID()
        {
            /*▼====: #003*/
            this.ShowBusyIndicator(eHCMSResources.Z1168_G1_DSKhoa);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ConfigurationManagerServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginRefDepartments_RecursiveByDeptID(2, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var items = contract.EndRefDepartments_RecursiveByDeptID(asyncResult);
                                if (items != null)
                                {
                                    ObjRefDepartments_RecursiveByDeptID = new ObservableCollection<RefDepartments>(items);

                                    if (ObjMedServiceItemPriceList_Current.MedServiceItemPriceListID <= 0)
                                    {
                                        //Item Default
                                        RefDepartments ItemDefault = new RefDepartments();
                                        ItemDefault.DeptID = -1;
                                        ItemDefault.DeptName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.K1997_G1_ChonKhoa);
                                        ObjRefDepartments_RecursiveByDeptID.Insert(0, ItemDefault);
                                        //Item Default
                                    }
                                }
                                else
                                {
                                    ObjRefDepartments_RecursiveByDeptID = null;
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
                /*▲====: #003*/
            });
            t.Start();
        }
        //ds Khoa

        //Loại DV
        private ObservableCollection<RefMedicalServiceType> _ObjRefMedicalServiceTypes_GetAll;
        public ObservableCollection<RefMedicalServiceType> ObjRefMedicalServiceTypes_GetAll
        {
            get { return _ObjRefMedicalServiceTypes_GetAll; }
            set
            {
                _ObjRefMedicalServiceTypes_GetAll = value;
                NotifyOfPropertyChange(() => ObjRefMedicalServiceTypes_GetAll);
            }
        }

        public void RefMedicalServiceItems_GroupMedicalServiceTypeIDByDeptID(Int64 DeptID, int V)
        {
            /*▼====: #003*/
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
                                    ObjRefMedicalServiceTypes_GetAll = new ObservableCollection<DataEntities.RefMedicalServiceType>(items);

                                    //Item Default
                                    RefMedicalServiceType ItemDefault = new RefMedicalServiceType();
                                    ItemDefault.MedicalServiceTypeID = -1;
                                    ItemDefault.MedicalServiceTypeName = "--Chọn Loại Dịch Vụ--";
                                    //Item Default
                                    ObjRefMedicalServiceTypes_GetAll.Insert(0, ItemDefault);

                                    //////if (ObjRefMedicalServiceTypes_GetAll.Count > 1)
                                    //////{
                                    //////    //Tất Cả Để Tìm Cho Tiện
                                    //////    RefMedicalServiceType ItemAll = new RefMedicalServiceType();
                                    //////    ItemAll.MedicalServiceTypeID = 0;
                                    //////    ItemAll.MedicalServiceTypeName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.T0822_G1_TatCa);
                                    //////    ObjRefMedicalServiceTypes_GetAll.Insert(1, ItemAll);
                                    //////    //Tất Cả Để Tìm Cho Tiện
                                    //////}
                                }
                                else
                                {
                                    ObjRefMedicalServiceTypes_GetAll = null;
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
                /*▲====: #003 */
            });
            t.Start();
        }
        //Loại DV

        public void cboDeptID_SelectionChanged(object selectItem)
        {
            if (selectItem != null)
            {
                VirList_IndexCurrent = -1;
                VirList_IsEndPage = false;

                if (ObjMedServiceItemPriceList_Current.MedServiceItemPriceListID <= 0)
                {
                    RefMedicalServiceItems_GroupMedicalServiceTypeIDByDeptID(ObjMedServiceItemPriceList_Current.DeptID, 1); //Subtract loại PCL

                    //ObjMedServiceItemPriceList_Current.MedicalServiceTypeID = -1; //Đợi Chọn

                    allMedServiceItemPrice_Paging.Clear();
                    ObjGetDeptMedServiceItems_Paging_Virtual.Clear();
                }
                else
                {
                    SearchCriteria.MedServiceItemPriceListID = ObjMedServiceItemPriceList_Current.MedServiceItemPriceListID;
                    RefMedicalServiceItems_GroupMedicalServiceTypeIDByDeptID(ObjMedServiceItemPriceList_Current.DeptID, 1); //Subtract loại PCL
                }
            }
        }

        private void LoadList()
        {
            //SearchCriteria.MedicalServiceTypeID = ObjMedServiceItemPriceList_Current.MedicalServiceTypeID;
            if (ObjMedServiceItemPriceList_Current.MedServiceItemPriceListID > 0)//TH Update
            {
                SearchCriteria.MedicalServiceTypeID = ObjMedServiceItemPriceList_Current.MedicalServiceTypeID; //TBL: MedicalServiceTypeID de filter cac dich vu theo loai khi cap nhat, them moi se lay len het 
                SearchCriteria.MedServiceItemPriceListID = ObjMedServiceItemPriceList_Current.MedServiceItemPriceListID;
                //allMedServiceItemPrice_Paging.PageIndex = 0;
                MedServiceItemPriceList_Detail(0, 100, true);
            }
            else//When AddNew
            {
                //allMedServiceItemPrice_Paging.PageIndex = 0;
                SearchCriteria.MedServiceItemPriceListID = 0;
                //GetDeptMedServiceItems_Paging(0, allMedServiceItemPrice_Paging.PageSize, true);
                MedServiceItemPriceList_Detail(0, 100, true);
            }
        }

        private MedServiceItemPriceSearchCriteria _SearchCriteria;
        public MedServiceItemPriceSearchCriteria SearchCriteria
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

        private PagedSortableCollectionView<MedServiceItemPrice> _ObjGetDeptMedServiceItems_Paging_Save;
        public PagedSortableCollectionView<MedServiceItemPrice> ObjGetDeptMedServiceItems_Paging_Save
        {
            get { return _ObjGetDeptMedServiceItems_Paging_Save; }
            set
            {
                _ObjGetDeptMedServiceItems_Paging_Save = value;
                NotifyOfPropertyChange(() => ObjGetDeptMedServiceItems_Paging_Save);
            }
        }

        private PagedSortableCollectionView<MedServiceItemPrice> _ObjGetDeptMedServiceItems_Paging_Virtual;
        public PagedSortableCollectionView<MedServiceItemPrice> ObjGetDeptMedServiceItems_Paging_Virtual
        {
            get { return _ObjGetDeptMedServiceItems_Paging_Virtual; }
            set
            {
                _ObjGetDeptMedServiceItems_Paging_Virtual = value;
                NotifyOfPropertyChange(() => ObjGetDeptMedServiceItems_Paging_Virtual);
            }
        }

        private ObservableCollection<MedServiceItemPrice> _ObjGetDeptMedServiceItems_All_Virtual_Delete;
        public ObservableCollection<MedServiceItemPrice> ObjGetDeptMedServiceItems_All_Virtual_Delete
        {
            get { return _ObjGetDeptMedServiceItems_All_Virtual_Delete; }
            set
            {
                _ObjGetDeptMedServiceItems_All_Virtual_Delete = value;
                NotifyOfPropertyChange(() => ObjGetDeptMedServiceItems_All_Virtual_Delete);
            }
        }

        private ObservableCollection<MedServiceItemPrice> _allMedServiceItemPrice_Paging;
        public ObservableCollection<MedServiceItemPrice> allMedServiceItemPrice_Paging
        {
            get { return _allMedServiceItemPrice_Paging; }
            set
            {
                _allMedServiceItemPrice_Paging = value;
                NotifyOfPropertyChange(() => allMedServiceItemPrice_Paging);
            }
        }

        /*▼====: #005*/
        private string _SearchKey;
        public string SearchKey
        {
            get { return _SearchKey; }
            set
            {
                _SearchKey = value;
                NotifyOfPropertyChange(() => SearchKey);
            }
        }

        private CollectionViewSource CVS_ObjAllMedServiceItemPrice_Paging = null;
        public CollectionView CV_ObjAllMedServiceItemPrice_Paging
        {
            get;
            set;
        }

        private void LoadDataGrid()
        {
            CVS_ObjAllMedServiceItemPrice_Paging = null;
            CVS_ObjAllMedServiceItemPrice_Paging = new CollectionViewSource { Source = allMedServiceItemPrice_Paging };
            CV_ObjAllMedServiceItemPrice_Paging = (CollectionView)CVS_ObjAllMedServiceItemPrice_Paging.View;
            NotifyOfPropertyChange(() => CV_ObjAllMedServiceItemPrice_Paging);
            btnFilter();
        }

        public void btnFilter()
        {
            if (CV_ObjAllMedServiceItemPrice_Paging != null)
            {
                CV_ObjAllMedServiceItemPrice_Paging.Filter = null;
                CV_ObjAllMedServiceItemPrice_Paging.Filter = new Predicate<object>(DoFilter);
            }
        }

        private bool DoFilter(object o)
        {
            long MedicalServiceTypeID = -1;
            //it is not a case sensitive search
            MedServiceItemPrice emp = o as MedServiceItemPrice;
            if (emp != null)
            {
                if (string.IsNullOrEmpty(SearchKey))
                {
                    SearchKey = "";
                }
                if (curRefMedicalServiceTypes != null && curRefMedicalServiceTypes.MedicalServiceTypeID > 0)
                {
                    MedicalServiceTypeID = curRefMedicalServiceTypes.MedicalServiceTypeID;
                }
                if ((emp.ObjMedServiceID.MedicalServiceTypeID == MedicalServiceTypeID || MedicalServiceTypeID == -1) && (Globals.RemoveVietnameseString(emp.ObjMedServiceID.MedServiceName.ToLower()).IndexOf(SearchKey.Trim().ToLower()) >= 0
                   || Globals.RemoveVietnameseString(emp.ObjMedServiceID.MedServiceCode.ToLower()).IndexOf(SearchKey.Trim().ToLower()) >= 0
                   || emp.ObjMedServiceID.MedServiceName.ToLower().IndexOf(SearchKey.Trim().ToLower()) >= 0
                   || emp.ObjMedServiceID.MedServiceCode.ToLower().IndexOf(SearchKey.Trim().ToLower()) >= 0))
                {
                    return true;
                }

                else
                {
                    return false;
                }
            }
            return false;
        }
        public void SearchKey_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SearchKey = (sender as TextBox).Text;
                btnFilter();
            }
        }
        /*▲====: #004*/

        private void AjustItemsToVirList(int pPageIndex)
        {
            if (pPageIndex > VirList_IndexCurrent - 1)
            {
                //Cộng dồn vô
                foreach (MedServiceItemPrice item in allMedServiceItemPrice_Paging)
                {
                    ObjGetDeptMedServiceItems_Paging_Virtual.Add(item);
                }
            }
        }

        public void hplDelete_Click(object selectItem)//Chỉ có trong TH Tạo Mới Bảng Giá
        {
            MedServiceItemPrice p = selectItem as MedServiceItemPrice;

            if (ObjMedServiceItemPriceList_Current.MedServiceItemPriceListID <= 0) //TH Tạo Bảng Giá
            {
                MarkDeleteRow_VirList(p);
                allMedServiceItemPrice_Paging.Remove(p);

                btChooseItemFromDelete_IsEnabled = true;
            }
        }

        private void MarkDeleteRow_VirList(MedServiceItemPrice p)
        {
            foreach (MedServiceItemPrice item in ObjGetDeptMedServiceItems_Paging_Virtual)
            {
                if (item.MedServItemPriceID == p.MedServItemPriceID)
                {
                    p.RowState = MedServiceItemPrice.RowStateValue.Delete;

                    //Add vào các mục đã xóa để cho chọn lại
                    AddMarkDeleteRow_VirList(p);
                    //Add vào các mục đã xóa để cho chọn lại

                    break;
                }
            }
        }

        private void UnMarkDeleteRow_VirList(MedServiceItemPrice p)
        {
            foreach (MedServiceItemPrice item in ObjGetDeptMedServiceItems_Paging_Virtual)
            {
                if (item.MedServItemPriceID == p.MedServItemPriceID)
                {
                    //Check lại 
                    CheckValueChangeAfterUnMarkDelete(p);

                    ObjGetDeptMedServiceItems_All_Virtual_Delete.Remove(p);

                    break;
                }
            }

            PagingLinq(0, 100);
        }

        private void CheckValueChangeAfterUnMarkDelete(MedServiceItemPrice p)
        {
            decimal NormalPrice = p.NormalPrice;
            decimal NormalPrice_Old = p.NormalPrice_Old;

            Nullable<decimal> PriceForHIPatient = p.PriceForHIPatient;
            Nullable<decimal> PriceForHIPatient_Old = p.PriceForHIPatient_Old;

            Nullable<decimal> HIAllowedPrice = p.HIAllowedPrice;
            Nullable<decimal> HIAllowedPrice_Old = p.HIAllowedPrice_Old;

            Nullable<double> VATRate = p.VATRate;
            Nullable<double> VATRate_Old = p.VATRate_Old;

            //Đánh dấu trạng thái dòng--> so 3 cột

            if (NormalPrice == NormalPrice_Old && PriceForHIPatient == PriceForHIPatient_Old &&
                HIAllowedPrice == HIAllowedPrice_Old && VATRate == VATRate_Old)
            {
                p.RowState = MedServiceItemPrice.RowStateValue.NoChange;
            }
            else
            {
                p.RowState = MedServiceItemPrice.RowStateValue.Update;
            }
        }

        private void AddMarkDeleteRow_VirList(MedServiceItemPrice p)
        {
            ObjGetDeptMedServiceItems_All_Virtual_Delete.Add(p);
        }

        /*▼====: #004*/
        public void InitializeNewItem(Int64 pMedicalServiceTypeID)
        {
            ObjMedServiceItemPriceList_Current = new DataEntities.MedServiceItemPriceList();
            ObjMedServiceItemPriceList_Current.EffectiveDate = BeginDate;
            BeginDate = Globals.GetCurServerDateTime();
            ObjMedServiceItemPriceList_Current.MedicalServiceTypeID = pMedicalServiceTypeID;
            ObjMedServiceItemPriceList_Current.MedServiceItemPriceListID = 0;
            ObjMedServiceItemPriceList_Current.StaffID = Globals.LoggedUserAccount.Staff.StaffID;
            ObjMedServiceItemPriceList_Current.ApprovedStaffID = ObjMedServiceItemPriceList_Current.StaffID;
        }
        /*▲====: #004*/

        #region Color Row and Cell
        public void dtgList_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            MedServiceItemPrice objRows = e.Row.DataContext as MedServiceItemPrice;
            if (objRows != null)
            {
                switch (objRows.PriceType)
                {
                    case "PriceCurrent":
                        {
                            e.Row.Foreground = new SolidColorBrush(Colors.Green);
                            break;
                        }
                    case "PriceFuture-Active-1":
                        {
                            e.Row.Foreground = new SolidColorBrush(Colors.Gray);
                            break;
                        }
                    case "PriceFuture-Active-0":
                        {
                            e.Row.Foreground = new SolidColorBrush(Colors.Blue);
                            break;
                        }
                }
            }

            CheckCellValueChange_ChangeColor(sender, e);
        }

        private void CheckCellValueChange_ChangeColor(object sender, DataGridRowEventArgs e)
        {
            MedServiceItemPrice p = e.Row.DataContext as MedServiceItemPrice;

            for (int i = 3; i <= 6; i++)
            {
                TextBlock textBlock = (sender as DataGrid).Columns[i].GetCellContent(e.Row) as TextBlock;

                decimal NormalPrice = p.NormalPrice;
                decimal NormalPrice_Old = p.NormalPrice_Old;

                Nullable<decimal> PriceForHIPatient = p.PriceForHIPatient;
                Nullable<decimal> PriceForHIPatient_Old = p.PriceForHIPatient_Old;

                Nullable<decimal> HIAllowedPrice = p.HIAllowedPrice;
                Nullable<decimal> HIAllowedPrice_Old = p.HIAllowedPrice_Old;

                Nullable<double> VATRate = p.VATRate;
                Nullable<double> VATRate_Old = p.VATRate_Old;

                switch (i)
                {
                    case 3:
                        {
                            if (NormalPrice == NormalPrice_Old)
                            {
                                ForeColorByPriceType(p, textBlock);
                            }
                            else if (textBlock != null)
                            {
                                textBlock.Foreground = new SolidColorBrush(Colors.Brown);
                            }
                            break;
                        }
                    case 4:
                        {
                            if (PriceForHIPatient == PriceForHIPatient_Old)
                            {
                                ForeColorByPriceType(p, textBlock);
                            }
                            else if (textBlock != null)
                            {
                                textBlock.Foreground = new SolidColorBrush(Colors.Brown);
                            }
                            break;
                        }
                    case 5:
                        {
                            if (HIAllowedPrice == HIAllowedPrice_Old)
                            {
                                ForeColorByPriceType(p, textBlock);
                            }
                            else if (textBlock != null)
                            {
                                textBlock.Foreground = new SolidColorBrush(Colors.Brown);
                            }
                            break;
                        }
                    case 6:
                        {
                            if (VATRate == VATRate_Old)
                            {
                                ForeColorByPriceType(p, textBlock);
                            }
                            else if (textBlock != null)
                            {
                                textBlock.Foreground = new SolidColorBrush(Colors.Brown);
                            }
                            break;
                        }
                }
            }
        }

        private FrameworkElement GetParent(FrameworkElement child, Type targetType)
        {
            object parent = child.Parent;
            if (parent != null)
            {
                if (parent.GetType() == targetType)
                {
                    return (FrameworkElement)parent;
                }
                else
                {
                    return GetParent((FrameworkElement)parent, targetType);
                }
            }
            return null;
        }

        //ChangedWPF-CMN: textBlock is alway null
        private void ForeColorByPriceType(MedServiceItemPrice objRows, TextBlock textBlock)
        {
            //switch (objRows.PriceType)
            //{
            //    case "PriceCurrent":
            //        {
            //            textBlock.Foreground = new SolidColorBrush(Colors.Green);
            //            break;
            //        }
            //    case "PriceFuture-Active-1":
            //        {
            //            textBlock.Foreground = new SolidColorBrush(Colors.Gray);
            //            break;
            //        }
            //    case "PriceFuture-Active-0":
            //        {
            //            textBlock.Foreground = new SolidColorBrush(Colors.Blue);
            //            break;
            //        }
            //}
        }

        DataGridRow row = null;
        DataGridColumn column = null;
        public void dtgList_PreparingCellForEdit(object sender, DataGridPreparingCellForEditEventArgs e)
        {
            column = e.Column;
            row = e.Row;

            int IndexCol = e.Column.DisplayIndex;
            string StringNameControl = GetStringNameControl(IndexCol);
            if (!string.IsNullOrEmpty(StringNameControl))
            {
                //20191204 TBL: Khi đổi qua WPF thì phải đổi qua cách mới nếu không thì Ctrtb sẽ luôn = null
                //TextBox Ctrtb = ((sender as DataGrid)).CurrentColumn.GetCellContent(e.Row).FindName(StringNameControl) as TextBox;
                TextBox Ctrtb = ((ContentPresenter)e.Column.GetCellContent(e.Row)).ContentTemplate.FindName(StringNameControl, e.Column.GetCellContent(e.Row)) as TextBox;
                if (Ctrtb != null)
                {
                    Ctrtb.Focus();
                    Ctrtb.SelectAll();
                }
            }
        }

        private string GetStringNameControl(int IndexCol)
        {
            string Result = "";
            switch (IndexCol)
            {
                case 3:
                    {
                        Result = "tbNormalPrice";
                        break;
                    }
                case 4:
                    {
                        Result = "tbPriceForHIPatient";
                        break;
                    }
                case 5:
                    {
                        Result = "tbHIAllowedPrice";
                        break;
                    }
                case 6:
                    {
                        Result = "tbVATRate";
                        break;
                    }
            }
            return Result;
        }


        public void dtgList_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (column != null && row != null)
            {
                int IndexColumn = e.Column.DisplayIndex;

                TextBlock textBlock = (column.GetCellContent(e.Row) as TextBlock);

                //▼====: #002
                var mCellCollection = (e.Row as DataGridRow).GetChildrenByType<DataGridCell>();
                if (mCellCollection != null && mCellCollection.Count > IndexColumn)
                {
                    var mCell = mCellCollection[IndexColumn];
                    if (mCell.Content != null && mCell.Content is ContentPresenter)
                    {
                        var mCellTemplate = (mCell.Content as ContentPresenter).ContentTemplate.LoadContent();
                        if (mCellTemplate is TextBlock)
                            textBlock = mCellTemplate as TextBlock;
                    }
                }
                //▲====: #002

                if (textBlock != null)
                {
                    if (CheckCellValueChange_UpdateRowStatus(sender, IndexColumn))
                    {
                        //textBlock.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(250, 120, 150, 111));
                        textBlock.Foreground = new SolidColorBrush(Colors.Brown);
                    }
                    else
                    {
                        MedServiceItemPrice objRows = ((MedServiceItemPrice)(((System.Windows.Controls.DataGrid)(sender)).SelectedItem));

                        switch (objRows.PriceType)
                        {
                            case "PriceCurrent":
                                {
                                    textBlock.Foreground = new SolidColorBrush(Colors.Green);
                                    break;
                                }
                            case "PriceFuture-Active-1":
                                {
                                    textBlock.Foreground = new SolidColorBrush(Colors.Gray);
                                    break;
                                }
                            case "PriceFuture-Active-0":
                                {
                                    textBlock.Foreground = new SolidColorBrush(Colors.Blue);
                                    break;
                                }
                        }

                    }
                    row = null;
                    column = null;
                }
            }
        }

        private bool CheckCellValueChange_UpdateRowStatus(object sender, int IndexColumn)
        {
            bool Result = true;

            decimal NormalPrice = ((MedServiceItemPrice)(((System.Windows.Controls.DataGrid)(sender)).SelectedItem)).NormalPrice;
            decimal NormalPrice_Old = ((MedServiceItemPrice)(((System.Windows.Controls.DataGrid)(sender)).SelectedItem)).NormalPrice_Old;

            Nullable<decimal> PriceForHIPatient = ((MedServiceItemPrice)(((System.Windows.Controls.DataGrid)(sender)).SelectedItem)).PriceForHIPatient;
            Nullable<decimal> PriceForHIPatient_Old = ((MedServiceItemPrice)(((System.Windows.Controls.DataGrid)(sender)).SelectedItem)).PriceForHIPatient_Old;

            Nullable<decimal> HIAllowedPrice = ((MedServiceItemPrice)(((System.Windows.Controls.DataGrid)(sender)).SelectedItem)).HIAllowedPrice;
            Nullable<decimal> HIAllowedPrice_Old = ((MedServiceItemPrice)(((System.Windows.Controls.DataGrid)(sender)).SelectedItem)).HIAllowedPrice_Old;

            Nullable<double> VATRate = ((MedServiceItemPrice)(((System.Windows.Controls.DataGrid)(sender)).SelectedItem)).VATRate;
            Nullable<double> VATRate_Old = ((MedServiceItemPrice)(((System.Windows.Controls.DataGrid)(sender)).SelectedItem)).VATRate_Old;

            switch (IndexColumn)
            {
                case 3:
                    {
                        if (NormalPrice == NormalPrice_Old)
                        {
                            Result = false;
                        }
                        if (NormalPrice < PriceForHIPatient)
                        {
                            MessageBox.Show(eHCMSResources.A0520_G1_Msg_InfoDGiaLonHonGiaBNBH);
                        }
                        break;
                    }
                case 4:
                    {
                        if (PriceForHIPatient == PriceForHIPatient_Old)
                        {
                            Result = false;
                        }
                        if (NormalPrice < PriceForHIPatient)
                        {
                            MessageBox.Show(eHCMSResources.T1984_G1_GiaBHChoPhep3);
                        }
                        else
                        {
                            ((MedServiceItemPrice)(((System.Windows.Controls.DataGrid)(sender)).SelectedItem)).PriceDifference = PriceForHIPatient - PriceForHIPatient;
                        }
                        break;
                    }
                case 5:
                    {
                        if (HIAllowedPrice == HIAllowedPrice_Old)
                        {
                            Result = false;
                        }
                        if (HIAllowedPrice > PriceForHIPatient)
                        {
                            MessageBox.Show(eHCMSResources.T1982_G1_GiaBHChoPhep2);
                        }
                        else
                        {
                            ((MedServiceItemPrice)(((System.Windows.Controls.DataGrid)(sender)).SelectedItem)).PriceDifference = PriceForHIPatient - PriceForHIPatient;
                        }
                        break;
                    }
                case 6:
                    {
                        if (VATRate == VATRate_Old)
                        {
                            Result = false;
                        }
                        break;
                    }
            }

            //Đánh dấu trạng thái dòng--> so 3 cột
            if (ObjMedServiceItemPriceList_Current.MedServiceItemPriceListID > 0)//Update 
            {
                if (NormalPrice == NormalPrice_Old && PriceForHIPatient == PriceForHIPatient_Old &&
                    HIAllowedPrice == HIAllowedPrice_Old && VATRate == VATRate_Old)
                {
                    ((MedServiceItemPrice)(((System.Windows.Controls.DataGrid)(sender)).SelectedItem)).
                        RowState = MedServiceItemPrice.RowStateValue.NoChange;
                }
                else
                {
                    ((MedServiceItemPrice)(((System.Windows.Controls.DataGrid)(sender)).SelectedItem)).
                        RowState = MedServiceItemPrice.RowStateValue.Update;
                }
            }

            return Result;
        }
        #endregion

        public bool Check_Valid()
        {
            string Errors = "";
            //----------26/10/2017 DPT fix không tạo bảng giá khi dịch vụ bị trùng
            //int dem = 0;
            //for (int i = 0; i < allMedServiceItemPrice_Paging.Count;i++ )
            //{
            //    for (int j = i+1; j < allMedServiceItemPrice_Paging.Count; j++)
            //    {
            //        if (allMedServiceItemPrice_Paging[i].MedServiceID == allMedServiceItemPrice_Paging[j].MedServiceID)
            //            {
            //                Errors += " dịch vụ \n " + allMedServiceItemPrice_Paging[i].ObjMedServiceID.MedServiceName + " \n";
            //                dem = dem + 1;
            //            }
            //     }
            //}
            //if (dem > 0)
            //{
            //    Errors += "bị lập lại";
            //    MessageBox.Show(string.Format(Errors), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
            //    return false;
            //}
            //--------------------------------------------------------------------------------------------
            foreach (var item in allMedServiceItemPrice_Paging)
            {
                if (item.V_NewPriceType == (long)AllLookupValues.V_NewPriceType.Fixed_PriceType && item.NormalPrice <= 0)
                {
                    Errors += "\n -" + item.ObjMedServiceID.MedServiceName;
                }
            }
            if (Errors != "")
            {
                MessageBox.Show(string.Format(eHCMSResources.Z1398_G1_DVThuocLoaiGiaCoDinh, Errors), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return false;
            }

            return true;
        }

        public void btSave()
        {
            if (String.IsNullOrEmpty(ObjMedServiceItemPriceList_Current.PriceListTitle))
            {
                MessageBox.Show(eHCMSResources.A0882_G1_Msg_InfoNhapTieuDeBGia);
                return;
            }
            if (CheckValid(ObjMedServiceItemPriceList_Current))
            {
                if (!Check_Valid())
                {
                    return;
                }
                if (ObjMedServiceItemPriceList_Current.MedServiceItemPriceListID <= 0)
                {
                    //ChangedWPF-CMN
                    //if ((allMedServiceItemPrice_Paging).PageCount == 1)
                    {
                        VirList_IsEndPage = true;
                    }

                    if (VirList_IsEndPage == false)
                    {
                        if (MessageBox.Show(string.Format("{0}. {1}", eHCMSResources.A0805_G1_Msg_InfoDMChuaDuyetHet, eHCMSResources.Z1086_G1_CoMuonTaoBGiaNayKg), eHCMSResources.T2937_G1_Luu, MessageBoxButton.OKCancel) ==
                            MessageBoxResult.OK)
                        {
                            ThuTucAddNew();
                        }
                    }
                    else
                    {
                        ThuTucAddNew();
                    }
                }
                else//Update
                {
                    ThuTucUpDate();
                }
            }
        }

        public void btClose()
        {
            TryClose();
        }

        //AddNew
        private void ListAddNew_Final()
        {
            ObjGetDeptMedServiceItems_Paging_Save.Clear();

            foreach (MedServiceItemPrice item in ObjGetDeptMedServiceItems_Paging_Virtual)
            {
                if (item.RowState != MedServiceItemPrice.RowStateValue.Delete)
                {
                    ObjGetDeptMedServiceItems_Paging_Save.Add(item);
                }
            }
        }

        private void ThuTucAddNew()
        {
            ListAddNew_Final();

            if (ObjGetDeptMedServiceItems_Paging_Save.Count > 0)
            {
                string message = "";
                if (CheckPriceBeforeSave(ref message))
                {
                    //==== #001
                    //if (message != ""
                    //&& MessageBox.Show(message + string.Format("\n{0}", eHCMSResources.T3805_G1_BanCoMuonTiepTucKhong), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                    if (message != "")
                    {
                        MessageBox.Show(message, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                        //==== #001
                        return;
                    }
                    TinhGiaChenhLech();

                    if (ObjMedServiceItemPriceList_Current.MedServiceItemPriceListID <= 0)
                    {
                        //if (CheckEffectiveDate_PriceList_AddNew())
                        {
                            MedServiceItemPriceList_AddNew();
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show(eHCMSResources.A0204_G1_Msg_BGiaChuaCoMucNao, eHCMSResources.T2937_G1_Luu, MessageBoxButton.OK);
            }
        }

        private bool CheckPriceBeforeSave(ref string message)
        {
            foreach (MedServiceItemPrice item in ObjGetDeptMedServiceItems_Paging_Save)
            {
                decimal NormalPrice = item.NormalPrice;
                Nullable<decimal> PriceForHIPatient = item.PriceForHIPatient;
                Nullable<decimal> HIAllowedPrice = item.HIAllowedPrice;

                if (NormalPrice >= 1)
                {
                    if (PriceForHIPatient != null)
                    {
                        if (PriceForHIPatient > NormalPrice)
                        {
                            MessageBox.Show(eHCMSResources.T1984_G1_GiaBHChoPhep3);
                            return false;
                        }
                    }
                    if (HIAllowedPrice != null)
                    {
                        if (HIAllowedPrice > PriceForHIPatient)
                        {
                            MessageBox.Show(eHCMSResources.T1982_G1_GiaBHChoPhep2);
                            return false;
                        }
                    }
                }
                else
                {
                    //MessageBox.Show("Đơn Giá Phải >= 1!");
                    //return false;
                    //message = eHCMSResources.Z1122_G1_DGiaNhoHon1; //20181223 TBL: Hien tai gia 0 dong van cho luu
                }
            }

            return true;
        }

        private void TinhGiaChenhLech()
        {
            foreach (MedServiceItemPrice item in ObjGetDeptMedServiceItems_Paging_Save)
            {
                item.PriceDifference = item.PriceForHIPatient - item.HIAllowedPrice;
            }
        }

        public bool CheckValid(object temp)
        {
            if (!(temp is DataEntities.MedServiceItemPriceList p))
            {
                return false;
            }

            return p.Validate();
        }

        public void MedServiceItemPriceList_AddNew()
        {
            /*▼====: #003*/
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "Đang Ghi..." });
            this.DlgShowBusyIndicator(eHCMSResources.Z0645_G1_DangGhi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ConfigurationManagerServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginMedServiceItemPriceList_AddNew(ObjMedServiceItemPriceList_Current, ObjGetDeptMedServiceItems_Paging_Save, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                string Result_PriceList = "";
                                contract.EndMedServiceItemPriceList_AddNew(out Result_PriceList, asyncResult);
                                switch (Result_PriceList)
                                {
                                    case "Insert-0":
                                        {
                                            MessageBox.Show(eHCMSResources.A0997_G1_Msg_InfoTaoBGiaFail, eHCMSResources.T0780_G1_TaoBGiaMoi, MessageBoxButton.OK);
                                            break;
                                        }
                                    case "Insert-1":
                                        {
                                            Globals.EventAggregator.Publish(new SaveEvent<bool>() { Result = true });

                                            btSave_IsEnabled = false;
                                            btChooseItemFromDelete_IsEnabled = false;

                                            MessageBox.Show(eHCMSResources.A0468_G1_Msg_InfoLuuOK, eHCMSResources.T0780_G1_TaoBGiaMoi, MessageBoxButton.OK);
                                            break;
                                        }
                                    case "Has-PriceList-Future":
                                        {
                                            MessageBox.Show(eHCMSResources.A0447_G1_Msg_InfoDaCoBGiaTLai, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                            break;
                                        }
                                    case "Has-PriceList-Current":
                                        {
                                            MessageBox.Show(string.Format("{0} {1}! {2}", eHCMSResources.A0445_G1_Msg_InfoDaCoBGiaHHanhChoThang, ObjMedServiceItemPriceList_Current.EffectiveDate.Month.ToString(), eHCMSResources.A0449_G1_Msg_InfoUseCNhatBGia), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                            break;
                                        }
                                    case "DayPriceListNewMust>DayPriceListCurrent":
                                        {
                                            MessageBox.Show(eHCMSResources.A0832_G1_Msg_InfoNgBGia, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                            break;
                                        }
                                }
                            }
                            catch (Exception ex)
                            {
                                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                                _logger.Error(ex.Message);
                            }
                            finally
                            {
                                this.DlgHideBusyIndicator();
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    _logger.Error(ex.Message);
                    this.DlgHideBusyIndicator();
                }
                /*▲====: #003*/
            });
            t.Start();
        }
        //AddNew

        //bt AddNew
        private bool _btSave_IsEnabled;
        public bool btSave_IsEnabled
        {
            get { return _btSave_IsEnabled; }
            set
            {
                _btSave_IsEnabled = value;
                NotifyOfPropertyChange(() => btSave_IsEnabled);
            }
        }
        //bt AddNew

        #region "Update"
        private void MedServiceItemPriceList_Detail(int PageIndex, int PageSize, bool CountTotal)
        {
            /*▼====: #003*/
            //Globals.EventAggregator.Publish(new BusyEvent() { IsBusy = true, Message = "Danh Sách Dịch Vụ..." });
            this.DlgShowBusyIndicator(eHCMSResources.K2945_G1_DSDV);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ConfigurationManagerServiceClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginGetMedServiceItemPrice_Paging(SearchCriteria, PageIndex, PageSize, "", CountTotal, Globals.DispatchCallback((asyncResult) =>
                        {
                            int Total = 0;
                            IList<MedServiceItemPrice> allItems = null;
                            bool bOK = false;
                            try
                            {
                                allItems = client.EndGetMedServiceItemPrice_Paging(out Total, asyncResult);
                                bOK = true;
                            }
                            catch (FaultException<AxException> fault)
                            {
                                ClientLoggerHelper.LogInfo(fault.ToString());
                            }
                            catch (Exception ex)
                            {
                                ClientLoggerHelper.LogInfo(ex.ToString());
                                _logger.Error(ex.Message);
                            }
                            finally
                            {
                                this.DlgHideBusyIndicator();
                            }
                            allMedServiceItemPrice_Paging.Clear();

                            if (bOK)
                            {
                                //if (CountTotal)
                                //{
                                //    allMedServiceItemPrice_Paging.TotalItemCount = Total;
                                //}
                                if (allItems != null)
                                {
                                    foreach (var item in allItems)
                                    {
                                        //Update trạng thái khi Update Ban Đầu là NoChange
                                        SetStatusRowForUpdate(item);
                                        //Update trạng thái khi Update Ban Đầu là NoChange

                                        allMedServiceItemPrice_Paging.Add(item);
                                    }
                                    LoadDataGrid();
                                    AjustItemsToVirList(PageIndex);
                                }
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    _logger.Error(ex.Message);
                    this.DlgHideBusyIndicator();
                }
                /*▲====: #003*/
            });
            t.Start();
        }

        /*▼====: #004*/
        private bool _dpEffectiveDate_IsEnabled = true;
        /*▲====: #004*/
        public bool dpEffectiveDate_IsEnabled
        {
            get { return _dpEffectiveDate_IsEnabled; }
            set
            {
                _dpEffectiveDate_IsEnabled = value;
                NotifyOfPropertyChange(() => dpEffectiveDate_IsEnabled);
            }
        }

        private bool _btChooseItemFromDelete_IsEnabled;
        public bool btChooseItemFromDelete_IsEnabled
        {
            get { return _btChooseItemFromDelete_IsEnabled; }
            set
            {
                _btChooseItemFromDelete_IsEnabled = value;
                NotifyOfPropertyChange(() => btChooseItemFromDelete_IsEnabled);
            }
        }

        private Visibility _dgCellTemplate0_Visible = Visibility.Visible;
        public Visibility dgCellTemplate0_Visible
        {
            get { return _dgCellTemplate0_Visible; }
            set
            {
                _dgCellTemplate0_Visible = value;
                NotifyOfPropertyChange(() => dgCellTemplate0_Visible);
            }
        }

        private void SetStatusRowForUpdate(MedServiceItemPrice item)
        {
            item.RowState = MedServiceItemPrice.RowStateValue.NoChange;
        }

        //Kiểm Tra Ngày Cho Update Bảng Giá
        private string CheckEffectiveDate_PriceList_Update()
        {
            if (ObjMedServiceItemPriceList_Current.EffectiveDate.Subtract(EffectiveDate_tmp).Days <= 0)
            {
                return "Edit-PriceList-Current";
            }
            else
            {
                return "Edit-PriceList-Future";
            }
        }
        //Kiểm Tra Ngày Cho Update Bảng Giá

        private void ListUpdate_Final()
        {
            ObjGetDeptMedServiceItems_Paging_Save.Clear();

            foreach (MedServiceItemPrice item in ObjGetDeptMedServiceItems_Paging_Virtual)
            {
                if (item.RowState == MedServiceItemPrice.RowStateValue.Update)
                {
                    ObjGetDeptMedServiceItems_Paging_Save.Add(item);
                    item.RowState = MedServiceItemPrice.RowStateValue.NoChange;
                }
            }
        }

        private void ThuTucUpDate()
        {
            ListUpdate_Final();

            ObservableCollection<MedServiceItemPrice> ObjCollection_AllForGiaoDelete = new ObservableCollection<MedServiceItemPrice>();
            ObservableCollection<MedServiceItemPrice> ObjCollection_Insert = new ObservableCollection<MedServiceItemPrice>();
            ObservableCollection<MedServiceItemPrice> ObjCollection_Update = new ObservableCollection<MedServiceItemPrice>();

            string message = "";
            if (CheckPriceBeforeSave(ref message))
            {
                //==== #001
                //if (message != ""
                //&& MessageBox.Show(message + string.Format("\n{0}", eHCMSResources.T3805_G1_BanCoMuonTiepTucKhong), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                if (message != "")
                {
                    MessageBox.Show(message, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                    //==== #001
                    return;
                }
                TinhGiaChenhLech();

                string Loai = CheckEffectiveDate_PriceList_Update();

                switch (Loai)
                {
                    case "Edit-PriceList-Current":
                        {
                            MedServiceItemPriceList_Update(ObjMedServiceItemPriceList_Current,
                                                           ObjCollection_AllForGiaoDelete, ObjCollection_Insert,
                                                           ObjGetDeptMedServiceItems_Paging_Save);
                            break;
                        }
                    case "Edit-PriceList-Future":
                        {
                            if (ObjMedServiceItemPriceList_Current.EffectiveDate.Subtract(dtCurrentServer).Days > 0)
                            {
                                MedServiceItemPriceList_Update(ObjMedServiceItemPriceList_Current,
                                                               ObjCollection_AllForGiaoDelete, ObjCollection_Insert,
                                                               ObjCollection_Update);
                            }
                            else
                            {
                                MessageBox.Show(eHCMSResources.A0179_G1_Msg_InfoNgBGChuaApDungLonHonHnay);
                            }
                            break;
                        }
                }
            }
        }

        public void MedServiceItemPriceList_Update(DataEntities.MedServiceItemPriceList Obj, ObservableCollection<MedServiceItemPrice> ObjCollection, ObservableCollection<MedServiceItemPrice> ObjCollection_Insert, ObservableCollection<MedServiceItemPrice> ObjCollection_Update)
        {
            /*▼====: #003*/
            this.DlgShowBusyIndicator(eHCMSResources.Z0645_G1_DangGhi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ConfigurationManagerServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginMedServiceItemPriceList_Update(Obj, ObjCollection, ObjCollection_Insert, ObjCollection_Update, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                string Result_PriceList = "";

                                contract.EndMedServiceItemPriceList_Update(out Result_PriceList, asyncResult);
                                switch (Result_PriceList)
                                {
                                    case "Update-0":
                                        {
                                            MessageBox.Show(eHCMSResources.A0606_G1_Msg_InfoHChinhBGiaFail, eHCMSResources.A0470_G1_Msg_HChinhBGia, MessageBoxButton.OK);
                                            break;
                                        }
                                    case "Update-1":
                                        {
                                            Globals.EventAggregator.Publish(new SaveEvent<bool>() { Result = true });
                                            MessageBox.Show(eHCMSResources.A0468_G1_Msg_InfoLuuOK, eHCMSResources.A0470_G1_Msg_HChinhBGia, MessageBoxButton.OK);
                                            LoadList();
                                            break;
                                        }
                                }
                            }
                            catch (Exception ex)
                            {
                                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                                _logger.Error(ex.Message);
                            }
                            finally
                            {
                                this.DlgHideBusyIndicator();
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    _logger.Error(ex.Message);
                    this.DlgHideBusyIndicator();
                }
                /*▲====: #003*/
            });
            t.Start();
        }
        #endregion

        #region "btChooseItemFromDelete"
        public void btChooseItemFromDelete()
        {
            Action<IMedServiceItemPriceList_ChooseFromDelete> onInitDlg = delegate (IMedServiceItemPriceList_ChooseFromDelete typeInfo)
            {
                typeInfo.ObjGetDeptMedServiceItems_All_Virtual_Delete = ObjGetDeptMedServiceItems_All_Virtual_Delete;
            };
            GlobalsNAV.ShowDialog<IMedServiceItemPriceList_ChooseFromDelete>(onInitDlg);
        }
        #endregion

        #region LostFocus
        public void LostFocus_EffectiveDate(object EffectiveDate)
        {
            if (EffectiveDate != null)
            {
                DateTime V = DateTime.Now;
                DateTime.TryParse(EffectiveDate.ToString(), out V);
            }
            else
            {
                ObjMedServiceItemPriceList_Current.EffectiveDate = dtCurrentServer;
            }
        }
        #endregion

        public void Handle(SelectedObjectEvent<MedServiceItemPrice> message)
        {
            if (message != null)
            {
                MedServiceItemPrice Objtmp = message.Result;
                UnMarkDeleteRow_VirList(Objtmp);
                SetbtChooseItemFromDelete_IsEnabled();
            }
        }

        private void SetbtChooseItemFromDelete_IsEnabled()
        {
            if (ObjGetDeptMedServiceItems_All_Virtual_Delete.Count > 0)
            {
                btChooseItemFromDelete_IsEnabled = true;
            }
            else
            {
                btChooseItemFromDelete_IsEnabled = false;
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
                }
            }
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
                                    ItemDefault.MedicalServiceTypeName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.T0822_G1_TatCa);
                                    //Item Default

                                    allRefMedicalServiceType.Insert(0, ItemDefault);
                                    curRefMedicalServiceTypes = new RefMedicalServiceType();
                                    curRefMedicalServiceTypes = allRefMedicalServiceType[0];
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
            });
            t.Start();
        }
        public void cboMedicalServiceTypesSubTractPCL_SelectionChanged(object selectItem)
        {
            btnFilter();
        }
    }
}
