using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.ServiceModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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
//using aEMR.Common.PagedCollectionView;
using aEMR.Controls;
using System.Linq;
using aEMR.ViewContracts;
using Castle.Windsor;
using System.Windows.Data;
using aEMR.Common.BaseModel;
using aEMR.Infrastructure.CachingUtils;
using Castle.Core.Logging;

/*
 * 20181002 #001 TNHX: Apply BusyIndicator, fix error add duplication in the same date, set DateTime is ServerDateTime
 * 20181007 #002 TNHX: [BM0000104] Allow selected Effectiveday when edit and refactor code
 */

namespace aEMR.Configuration.PCLExamTypePriceList.ViewModels
{
    [Export(typeof(IPCLExamTypePriceList_AddEdit)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class PCLExamTypePriceList_AddEditViewModel : ViewModelBase, IPCLExamTypePriceList_AddEdit
        , IHandle<SelectedObjectEvent<PCLExamType>>
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public PCLExamTypePriceList_AddEditViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg)
        {
            /*▼====: #001*/
            SearchCriteria = new PCLExamTypeSearchCriteria();
            /*▲====: #001*/
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

        private DateTime EffectiveDate_tmp;
        private DateTime dtCurrentServer;
        private int VirList_IndexCurrent = -1;//ban đầu phân trang chạy từ 0
        protected override void OnActivate()
        {
            Globals.EventAggregator.Subscribe(this);

            base.OnActivate();
            VirList_IndexCurrent = -1;
            SearchCriteria = new PCLExamTypeSearchCriteria();

            ObjPCLExamTypesAndPriceIsActive_Paging_Virtual=new PagedSortableCollectionView<PCLExamType>();
            allPCLExamTypes_Paging = new ObservableCollection<PCLExamType>();
            ObjPCLExamTypesAndPriceIsActive_ByPCLGroupID_All_Virtual_Delete = new ObservableCollection<PCLExamType>();
            ObjPCLExamTypesAndPriceIsActive_Paging_Save = new PagedSortableCollectionView<PCLExamType>();

            if (ObjPCLExamTypePriceList_Current.PCLExamTypePriceListID > 0)//Update
            {
                btSave_IsEnabled = true;
                EffectiveDate_tmp = ObjPCLExamTypePriceList_Current.EffectiveDate.Value;

                btChooseItemFromDelete_IsEnabled = true;
                if (!ObjPCLExamTypePriceList_Current.IsActive && ((DateTime)ObjPCLExamTypePriceList_Current.EffectiveDate).Date <= BeginDate.Date)
                {
                    MessageBox.Show(eHCMSResources.A0205_G1_Msg_InfoKhDcSuaBGiaCu, eHCMSResources.Z0665_G1_SuaBGia, MessageBoxButton.OK);
                    btSave_IsEnabled = false;
                }
                this.PCLExamTypePriceList_Detail(0,1000,false);
            }
            else
            {
                //dpEffectiveDate_IsEnabled = true;
                btChooseItemFromDelete_IsEnabled = true;
                btSave_IsEnabled = true;
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

        private DataEntities.PCLExamTypePriceList _ObjPCLExamTypePriceList_Current;
        public DataEntities.PCLExamTypePriceList ObjPCLExamTypePriceList_Current
        {
            get { return _ObjPCLExamTypePriceList_Current; }
            set
            {
                _ObjPCLExamTypePriceList_Current = value;
                NotifyOfPropertyChange(() => ObjPCLExamTypePriceList_Current);
            }
        }

        private PagedSortableCollectionView<PCLExamType> _ObjPCLExamTypesAndPriceIsActive_Paging_Save;
        public PagedSortableCollectionView<PCLExamType> ObjPCLExamTypesAndPriceIsActive_Paging_Save
        {
            get { return _ObjPCLExamTypesAndPriceIsActive_Paging_Save; }
            set
            {
                _ObjPCLExamTypesAndPriceIsActive_Paging_Save = value;
                NotifyOfPropertyChange(() => ObjPCLExamTypesAndPriceIsActive_Paging_Save);
            }
        }

        private ObservableCollection<PCLExamType> _allPCLExamTypes_Paging;
        public ObservableCollection<PCLExamType> allPCLExamTypes_Paging
        {
            get { return _allPCLExamTypes_Paging; }
            set
            {
                _allPCLExamTypes_Paging = value;
                NotifyOfPropertyChange(() => allPCLExamTypes_Paging);
            }
        }

        private PagedSortableCollectionView<PCLExamType> _ObjPCLExamTypesAndPriceIsActive_Paging_Virtual;
        public PagedSortableCollectionView<PCLExamType> ObjPCLExamTypesAndPriceIsActive_Paging_Virtual
        {
            get { return _ObjPCLExamTypesAndPriceIsActive_Paging_Virtual; }
            set
            {
                _ObjPCLExamTypesAndPriceIsActive_Paging_Virtual = value;
                NotifyOfPropertyChange(() => ObjPCLExamTypesAndPriceIsActive_Paging_Virtual);
            }
        }

        private ObservableCollection<PCLExamType> _ObjPCLExamTypesAndPriceIsActive_ByPCLGroupID_All_Virtual_Delete;
        public ObservableCollection<PCLExamType> ObjPCLExamTypesAndPriceIsActive_ByPCLGroupID_All_Virtual_Delete
        {
            get { return _ObjPCLExamTypesAndPriceIsActive_ByPCLGroupID_All_Virtual_Delete; }
            set
            {
                _ObjPCLExamTypesAndPriceIsActive_ByPCLGroupID_All_Virtual_Delete = value;
                NotifyOfPropertyChange(() => ObjPCLExamTypesAndPriceIsActive_ByPCLGroupID_All_Virtual_Delete);
            }
        }

        private CollectionViewSource CVS_ObjPCLExamTypesAndPriceIsActive_Paging = null;
        public CollectionView CV_ObjPCLExamTypesAndPriceIsActive_Paging { get; set; }

        private PCLExamTypeSearchCriteria _SearchCriteria;
        public PCLExamTypeSearchCriteria SearchCriteria
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

        /*▼====: #001*/
        private void PCLExamTypesAndPriceIsActive_Paging(int PageIndex, int PageSize, bool CountTotal)
        {
            this.DlgShowBusyIndicator(eHCMSResources.K3014_G1_DSPCLExamType);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ConfigurationManagerServiceClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginPCLExamTypesAndPriceIsActive_Paging(SearchCriteria, PageIndex, PageSize, "", CountTotal, Globals.DispatchCallback((asyncResult) =>
                        {
                            int Total = 0;
                            IList<PCLExamType> allItems = null;
                            bool bOK = false;
                            try
                            {
                                allItems = client.EndPCLExamTypesAndPriceIsActive_Paging(out Total, asyncResult);
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
                            if (bOK)
                            {
                                allPCLExamTypes_Paging = new ObservableCollection<PCLExamType>(allItems);
                                LoadDataGrid();
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
            });
            t.Start();
        }
        /*▲====: #001*/

        DataPager pagerSellingList = null;
        public void pagerSellingList_Loaded(object sender, RoutedEventArgs e)
        {
            pagerSellingList = sender as DataPager;
        }

        CheckBox PagingChecked;
        public void Paging_Checked(object sender, RoutedEventArgs e)
        {
            //activate datapager
            PagingChecked = sender as CheckBox;
            pagerSellingList.Source = CV_ObjPCLExamTypesAndPriceIsActive_Paging.Cast<object>().ToObservableCollection();
            VisibilityPaging = Visibility.Visible;
        }

        public void Paging_Unchecked(object sender, RoutedEventArgs e)
        {
            //deactivate datapager
            pagerSellingList.Source = null;
            VisibilityPaging = Visibility.Collapsed;

            LoadDataGrid();
        }

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

        private void LoadDataGrid()
        {
            CVS_ObjPCLExamTypesAndPriceIsActive_Paging = null;
            CVS_ObjPCLExamTypesAndPriceIsActive_Paging = new CollectionViewSource { Source = allPCLExamTypes_Paging };
            CV_ObjPCLExamTypesAndPriceIsActive_Paging = (CollectionView)CVS_ObjPCLExamTypesAndPriceIsActive_Paging.View;
            NotifyOfPropertyChange(() => CV_ObjPCLExamTypesAndPriceIsActive_Paging);
            btnFilter();
        }

        public void btnFilter()
        {
            CV_ObjPCLExamTypesAndPriceIsActive_Paging.Filter = null;
            CV_ObjPCLExamTypesAndPriceIsActive_Paging.Filter = new Predicate<object>(DoFilter);
        }

        //Callback method
        private bool DoFilter(object o)
        {
            //it is not a case sensitive search
            PCLExamType emp = o as PCLExamType;
            if (emp != null)
            {
                if (string.IsNullOrEmpty(SearchKey))
                {
                    SearchKey = "";
                }
                if (Globals.RemoveVietnameseString(emp.PCLExamTypeName.ToLower()).IndexOf(SearchKey.Trim().ToLower()) >= 0
                    || Globals.RemoveVietnameseString(emp.PCLExamTypeCode.ToLower()).IndexOf(SearchKey.Trim().ToLower()) >= 0
                    || emp.PCLExamTypeName.ToLower().IndexOf(SearchKey.Trim().ToLower()) >= 0 
                    || emp.PCLExamTypeCode.ToLower().IndexOf(SearchKey.Trim().ToLower()) >= 0)
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

        private Visibility _VisibilityPaging = Visibility.Collapsed;
        public Visibility VisibilityPaging
        {
            get
            {
                return _VisibilityPaging;
            }
            set
            {
                if (_VisibilityPaging != value)
                {
                    _VisibilityPaging = value;
                    NotifyOfPropertyChange(() => VisibilityPaging);
                }
            }
        }

        private void UnCheckPaging()
        {
            if (PagingChecked != null && allPCLExamTypes_Paging != null)
            {
                PagingChecked.IsChecked = false;
                VisibilityPaging = Visibility.Collapsed;
            }
        }

        private int _PCVPageSize = 30;
        public int PCVPageSize
        {
            get
            {
                return _PCVPageSize;
            }
            set
            {
                if (_PCVPageSize != value)
                {
                    _PCVPageSize = value;
                    NotifyOfPropertyChange(() => PCVPageSize);
                }
            }
        }

        public void cbxPageSize_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((sender as ComboBox) != null && (sender as ComboBox).SelectedItem != null)
            {
                PCVPageSize = Convert.ToInt32(((sender as ComboBox).SelectedItem as ComboBoxItem).Content);
            }
        }

        public void hplDelete_Click(object selectItem)//Chỉ có trong TH Tạo Mới Bảng Giá
        {
            PCLExamType p = selectItem as PCLExamType;

            //if (ObjPCLExamTypePriceList_Current.PCLExamTypePriceListID <= 0) //TH Tạo Bảng Giá
            {
                MarkDeleteRow_VirList(p);
                //ObjPCLExamTypesAndPriceIsActive_Paging.Remove(p);

                btChooseItemFromDelete_IsEnabled = true;
            }
        }

        private void MarkDeleteRow_VirList(PCLExamType p)
        {
            foreach (PCLExamType item in ObjPCLExamTypesAndPriceIsActive_Paging_Virtual)
            {
                if (item.PCLExamTypeID== p.PCLExamTypeID)
                {
                    p.ObjPCLExamTypePrice.RowState = PCLExamTypePrice.RowStateValue.Delete;

                    //Add vào các mục đã xóa để cho chọn lại
                    AddMarkDeleteRow_VirList(p);
                    //Add vào các mục đã xóa để cho chọn lại

                    break;
                }
            }
        }

        private void UnMarkDeleteRow_VirList(PCLExamType p)
        {
            foreach (PCLExamType item in ObjPCLExamTypesAndPriceIsActive_Paging_Virtual)
            {
                if (item.PCLExamTypeID== p.PCLExamTypeID)
                {
                    //Check lại 
                    CheckValueChangeAfterUnMarkDelete(p);

                    ObjPCLExamTypesAndPriceIsActive_ByPCLGroupID_All_Virtual_Delete.Remove(p);
                    break;
                }
            }
           // PagingLinq(ObjPCLExamTypesAndPriceIsActive_Paging.PageIndex, ObjPCLExamTypesAndPriceIsActive_Paging.PageSize);
        }

        private void CheckValueChangeAfterUnMarkDelete(PCLExamType p)
        {
            decimal NormalPrice = p.ObjPCLExamTypePrice.NormalPrice;
            decimal NormalPrice_Old =p.ObjPCLExamTypePrice.NormalPrice_Old;

            Nullable<decimal> PriceForHIPatient = p.ObjPCLExamTypePrice.PriceForHIPatient;
            Nullable<decimal> PriceForHIPatient_Old = p.ObjPCLExamTypePrice.PriceForHIPatient_Old;

            Nullable<decimal> HIAllowedPrice = p.ObjPCLExamTypePrice.HIAllowedPrice;
            Nullable<decimal> HIAllowedPrice_Old = p.ObjPCLExamTypePrice.HIAllowedPrice_Old;
            
            //Đánh dấu trạng thái dòng--> so 3 cột

            if (NormalPrice == NormalPrice_Old && PriceForHIPatient == PriceForHIPatient_Old && HIAllowedPrice == HIAllowedPrice_Old)
            {
                p.ObjPCLExamTypePrice.RowState = PCLExamTypePrice.RowStateValue.NoChange;
            }
            else
            {
                p.ObjPCLExamTypePrice.RowState = PCLExamTypePrice.RowStateValue.Update;
            }
        }

        private void AddMarkDeleteRow_VirList(PCLExamType p)
        {
            ObjPCLExamTypesAndPriceIsActive_ByPCLGroupID_All_Virtual_Delete.Add(p);
        }

        /*▼====: #002*/
        public void InitializeNewItem()
        {
            dtCurrentServer = Globals.GetCurServerDateTime();
            ObjPCLExamTypePriceList_Current = new DataEntities.PCLExamTypePriceList();
            ObjPCLExamTypePriceList_Current.PCLExamTypePriceListID = 0;
            ObjPCLExamTypePriceList_Current.EffectiveDate = BeginDate;
            ObjPCLExamTypePriceList_Current.StaffID = Globals.LoggedUserAccount.Staff.StaffID;
            ObjPCLExamTypePriceList_Current.ApprovedStaffID = ObjPCLExamTypePriceList_Current.StaffID;
            BeginDate = dtCurrentServer;
            PCLExamTypesAndPriceIsActive_Paging(0,1000,false);
        }
        /*▲====: #002*/

        #region Color Row and Cell
        public void dtgList_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = e.Row.GetIndex() + 1;
            PCLExamType objRows = e.Row.DataContext as PCLExamType;
            if (objRows != null)
            {
                // TNHX: sPriceType always null so text will be black
                switch (objRows.ObjPCLExamTypePrice.PriceType)
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
                    default:
                        {
                            e.Row.Foreground = new SolidColorBrush(Colors.Black);
                            break;
                        }
                }
            }

            CheckCellValueChange_ChangeColor(sender, e);
        }

        private void CheckCellValueChange_ChangeColor(object sender, DataGridRowEventArgs e)
        {
            PCLExamType p = e.Row.DataContext as PCLExamType;

            for (int i = 3; i <= 5; i++)
            {
                TextBlock textBlock = (sender as DataGrid).Columns[i].GetCellContent(e.Row) as TextBlock;

                decimal NormalPrice = p.ObjPCLExamTypePrice.NormalPrice;
                decimal NormalPrice_Old = p.ObjPCLExamTypePrice.NormalPrice_Old;

                Nullable<decimal> PriceForHIPatient = p.ObjPCLExamTypePrice.PriceForHIPatient;
                Nullable<decimal> PriceForHIPatient_Old = p.ObjPCLExamTypePrice.PriceForHIPatient_Old;

                Nullable<decimal> HIAllowedPrice = p.ObjPCLExamTypePrice.HIAllowedPrice;
                Nullable<decimal> HIAllowedPrice_Old = p.ObjPCLExamTypePrice.HIAllowedPrice_Old;
                if (textBlock != null)
                {
                    switch (i)
                    {
                        case 3:
                            {
                                if (NormalPrice == NormalPrice_Old)
                                {
                                    ForeColorByPriceType(p, textBlock);
                                }
                                else
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
                                else
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
                                else
                                {
                                    textBlock.Foreground = new SolidColorBrush(Colors.Brown);
                                }
                                break;
                            }
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

        private void ForeColorByPriceType(PCLExamType objRows, TextBlock textBlock)
        {
            //20180712 TBL: Bao loi textBlock null
            //switch (objRows.ObjPCLExamTypePrice.PriceType)
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
            //    default:
            //        {
            //            textBlock.Foreground = new SolidColorBrush(Colors.Black);
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
            }
            return Result;
        }
        
        public void dtgList_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (column != null && row != null)
            {
                int IndexColumn = e.Column.DisplayIndex;

                TextBlock textBlock = (column.GetCellContent(e.Row) as TextBlock);
                
                if (CheckCellValueChange_UpdateRowStatus(sender, IndexColumn))
                {
                    //textBlock.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(250, 120, 150, 111));
                    if (textBlock != null)
                        textBlock.Foreground = new SolidColorBrush(Colors.Brown);
                }
                else
                {
                    PCLExamType objRows = ((PCLExamType)(((System.Windows.Controls.DataGrid)(sender)).SelectedItem));

                    switch (objRows.ObjPCLExamTypePrice.PriceType)
                    {
                        case "PriceCurrent":
                            {
                                if (textBlock != null)
                                    textBlock.Foreground = new SolidColorBrush(Colors.Green);
                                break;
                            }
                        case "PriceFuture-Active-1":
                            {
                                if (textBlock != null)
                                    textBlock.Foreground = new SolidColorBrush(Colors.Gray);
                                break;
                            }
                        case "PriceFuture-Active-0":
                            {
                                if (textBlock != null)
                                    textBlock.Foreground = new SolidColorBrush(Colors.Blue);
                                break;
                            }
                    }
                }
                row = null;
                column = null;
            }
        }

        private bool CheckCellValueChange_UpdateRowStatus(object sender, int IndexColumn)
        {
            bool Result = true;

            decimal NormalPrice = ((PCLExamType)(((System.Windows.Controls.DataGrid)(sender)).SelectedItem)).ObjPCLExamTypePrice.NormalPrice;
            decimal NormalPrice_Old = ((PCLExamType)(((System.Windows.Controls.DataGrid)(sender)).SelectedItem)).ObjPCLExamTypePrice.NormalPrice_Old;

            Nullable<decimal> PriceForHIPatient = ((PCLExamType)(((System.Windows.Controls.DataGrid)(sender)).SelectedItem)).ObjPCLExamTypePrice.PriceForHIPatient;
            Nullable<decimal> PriceForHIPatient_Old = ((PCLExamType)(((System.Windows.Controls.DataGrid)(sender)).SelectedItem)).ObjPCLExamTypePrice.PriceForHIPatient_Old;

            Nullable<decimal> HIAllowedPrice = ((PCLExamType)(((System.Windows.Controls.DataGrid)(sender)).SelectedItem)).ObjPCLExamTypePrice.HIAllowedPrice;
            Nullable<decimal> HIAllowedPrice_Old = ((PCLExamType)(((System.Windows.Controls.DataGrid)(sender)).SelectedItem)).ObjPCLExamTypePrice.HIAllowedPrice_Old;

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
                        break;
                    }
            }

            //Đánh dấu trạng thái dòng--> so 3 cột
            if (ObjPCLExamTypePriceList_Current.PCLExamTypePriceListID > 0)//Update 
            {
                if (NormalPrice == NormalPrice_Old && PriceForHIPatient == PriceForHIPatient_Old && HIAllowedPrice == HIAllowedPrice_Old)
                {
                    ((PCLExamType)(((System.Windows.Controls.DataGrid)(sender)).SelectedItem)).ObjPCLExamTypePrice.RowState = PCLExamTypePrice.RowStateValue.NoChange;
                }
                else
                {
                    ((PCLExamType)(((System.Windows.Controls.DataGrid)(sender)).SelectedItem)).ObjPCLExamTypePrice.RowState = PCLExamTypePrice.RowStateValue.Update;
                }
            }

            return Result;
        }
        #endregion

        public void btSave()
        {

            if (String.IsNullOrEmpty(ObjPCLExamTypePriceList_Current.PriceListTitle))
            {
                MessageBox.Show(eHCMSResources.A0882_G1_Msg_InfoNhapTieuDeBGia);
                return;
            }

            if (CheckValid(ObjPCLExamTypePriceList_Current))
            {
                if (ObjPCLExamTypePriceList_Current.PCLExamTypePriceListID <= 0)
                {
                    if (MessageBox.Show(eHCMSResources.A0805_G1_Msg_InfoDMChuaDuyetHet + Environment.NewLine + eHCMSResources.Z1086_G1_CoMuonTaoBGiaNayKg, eHCMSResources.T2937_G1_Luu, MessageBoxButton.OKCancel) ==
                            MessageBoxResult.OK)
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
        private void ThuTucAddNew()
        {
            if (allPCLExamTypes_Paging.Count > 0)
            {
                string message = "";
                if (CheckPriceBeforeSave(ref message))
                {
                    if (message != ""
                    && MessageBox.Show(message + string.Format("\n{0}", eHCMSResources.T3805_G1_BanCoMuonTiepTucKhong), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                    {
                        return;
                    }
                    if (ObjPCLExamTypePriceList_Current.PCLExamTypePriceListID <= 0)
                    {
                        if (CheckEffectiveDate_PriceList_AddNew())
                        {
                            PCLExamTypePriceList_AddNew();
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
            foreach (PCLExamType item in allPCLExamTypes_Paging)
            {
                decimal NormalPrice = item.ObjPCLExamTypePrice.NormalPrice;
                Nullable<decimal> PriceForHIPatient = item.ObjPCLExamTypePrice.PriceForHIPatient;
                Nullable<decimal> HIAllowedPrice = item.ObjPCLExamTypePrice.HIAllowedPrice;

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
                    message = "Đơn Giá < 1!";
                }

            }
            return true;
        }

        public bool CheckValid(object temp)
        {
            DataEntities.PCLExamTypePriceList p = temp as DataEntities.PCLExamTypePriceList;
            if (p == null)
            {
                return false;
            }
            return p.Validate();
        }

        public bool CheckEffectiveDate_PriceList_AddNew()
        {
            DateTime dtMaxInList = NgayApDungMaxInList();
            if (ObjPCLExamTypePriceList_Current.EffectiveDate.Value.Subtract(dtMaxInList).Days < 0)
            {
                MessageBox.Show(string.Format(eHCMSResources.Z0694_G1_I, dtMaxInList.ToString("dd/MM/yyyy")), eHCMSResources.T2937_G1_Luu, MessageBoxButton.OK);
                return false;
            }
            return true;
        }

        private DateTime NgayApDungMaxInList()
        {
            DateTime dtMaxInList = dtCurrentServer;

            foreach (PCLExamType item in CV_ObjPCLExamTypesAndPriceIsActive_Paging)
            {
                if (item.ObjPCLExamTypePrice.PriceType == "PriceCurrent")
                {
                    if (item.ObjPCLExamTypePrice.EffectiveDate != null)
                    {
                        dtMaxInList = item.ObjPCLExamTypePrice.EffectiveDate.Value;

                        if (item.ObjPCLExamTypePrice.EffectiveDate.Value.Subtract(dtMaxInList).Days > 0)
                        {
                            dtMaxInList = item.ObjPCLExamTypePrice.EffectiveDate.Value;
                        }
                    }
                }
            }

            return dtMaxInList;
        }

        public void PCLExamTypePriceList_AddNew()
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "Đang Ghi..." });
            this.DlgShowBusyIndicator(eHCMSResources.Z0645_G1_DangGhi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ConfigurationManagerServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginPCLExamTypePriceList_AddNew(ObjPCLExamTypePriceList_Current, allPCLExamTypes_Paging, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                string Result_PriceList = "";

                                contract.EndPCLExamTypePriceList_AddNew(out Result_PriceList, asyncResult);
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

                                            btChooseItemFromDelete_IsEnabled = false;
                                            btSave_IsEnabled = false;

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
                                            MessageBox.Show(string.Format("{0} ", eHCMSResources.A0445_G1_Msg_InfoDaCoBGiaHHanhChoThang) + ObjPCLExamTypePriceList_Current.EffectiveDate.Value.Month.ToString() + "! (Dùng Cập Nhật Bảng Giá Để Sửa)", eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
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
            });
            t.Start();
        }
        //AddNew
        
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

        #region "Update"
        private void PCLExamTypePriceList_Detail(int PageIndex, int PageSize, bool CountTotal)
        {
            this.DlgShowBusyIndicator(eHCMSResources.Z1007_G1_LoadDSDV);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ConfigurationManagerServiceClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginPCLExamTypePriceList_Detail(ObjPCLExamTypePriceList_Current.PCLExamTypePriceListID, PageIndex, PageSize, "", CountTotal, Globals.DispatchCallback((asyncResult) =>
                        {
                            int Total = 0;
                            IList<PCLExamType> allItems = null;
                            bool bOK = false;
                            try
                            {
                                allItems = client.EndPCLExamTypePriceList_Detail(out Total, asyncResult);
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
                            if (bOK)
                            {
                                allPCLExamTypes_Paging = new ObservableCollection<PCLExamType>(allItems);
                                LoadDataGrid();
                                AjustItemsToVirList(PageIndex);
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
            });
            t.Start();
        }

        private bool _dpEffectiveDate_IsEnabled = true;
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

        private void SetStatusRowForUpdate(PCLExamType item)
        {
            item.ObjPCLExamTypePrice.RowState = PCLExamTypePrice.RowStateValue.NoChange;
        }

        //Kiểm Tra Ngày Cho Update Bảng Giá
        private string CheckEffectiveDate_PriceList_Update()
        {
            if (ObjPCLExamTypePriceList_Current.EffectiveDate.Value.Subtract(EffectiveDate_tmp).Days <= 0)
            {
                return "Edit-PriceList-Current";
            }
            else
            {
                return "Edit-PriceList-Future";
            }
        }
        //Kiểm Tra Ngày Cho Update Bảng Giá
        private void AjustItemsToVirList(int pPageIndex)
        {
            if (pPageIndex > VirList_IndexCurrent - 1)
            {
                foreach (PCLExamType item in allPCLExamTypes_Paging)
                {
                    ObjPCLExamTypesAndPriceIsActive_Paging_Virtual.Add(item);
                }
            }
        }
        private void ListUpdate_Final()
        {
            ObjPCLExamTypesAndPriceIsActive_Paging_Save.Clear();

            foreach (PCLExamType item in ObjPCLExamTypesAndPriceIsActive_Paging_Virtual)
            {
                if (item.ObjPCLExamTypePrice.RowState == PCLExamTypePrice.RowStateValue.Update)
                {
                    ObjPCLExamTypesAndPriceIsActive_Paging_Save.Add(item);
                    item.ObjPCLExamTypePrice.RowState = PCLExamTypePrice.RowStateValue.NoChange;
                }
            }
        }
        private void ThuTucUpDate()
        {
            ListUpdate_Final();
            ObservableCollection<PCLExamType> ObjCollection_Update = new ObservableCollection<PCLExamType>();

            string message = "";
            if (CheckPriceBeforeSave(ref message))
            {
                if (message != ""
                    && MessageBox.Show(message + string.Format("\n{0}", eHCMSResources.T3805_G1_BanCoMuonTiepTucKhong), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                {
                    return;
                }
                string Loai = CheckEffectiveDate_PriceList_Update();

                switch (Loai)
                {
                    case "Edit-PriceList-Current":
                        {
                            PCLExamTypePriceList_Update(ObjPCLExamTypePriceList_Current, ObjPCLExamTypesAndPriceIsActive_Paging_Save);
                            break;
                        }
                    case "Edit-PriceList-Future":
                        {
                            if (ObjPCLExamTypePriceList_Current.EffectiveDate.Value.Subtract(dtCurrentServer).Days > 0)
                            {
                                PCLExamTypePriceList_Update(ObjPCLExamTypePriceList_Current, ObjPCLExamTypesAndPriceIsActive_Paging_Save);
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
        private void LoadList()
        {
            if (ObjPCLExamTypePriceList_Current.PCLExamTypePriceListID > 0)//TH Update
            {
                PCLExamTypePriceList_Detail(0, 1000, false);
            }
            else//When AddNew
            {
                PCLExamTypePriceList_Detail(0, 1000, false);
            }
        }

        public void PCLExamTypePriceList_Update(DataEntities.PCLExamTypePriceList Obj, ObservableCollection<PCLExamType> ObjCollection_Update)
        {
            this.DlgShowBusyIndicator(eHCMSResources.Z0645_G1_DangGhi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ConfigurationManagerServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginPCLExamTypePriceList_Update(Obj, ObjCollection_Update, Globals.DispatchCallback((asyncResult) =>
                         {
                             try
                             {
                                 string Result_PriceList = "";

                                 contract.EndPCLExamTypePriceList_Update(out Result_PriceList, asyncResult);
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
            });
            t.Start();
        }
        #endregion

        #region "btChooseItemFromDelete"
        public void btChooseItemFromDelete()
        {
            Action<IPCLExamTypePriceList_ChooseFromDelete> onInitDlg = delegate (IPCLExamTypePriceList_ChooseFromDelete typeInfo)
            {
                typeInfo.ObjPCLExamTypesAndPriceIsActive_ByPCLGroupID_All_Virtual_Delete = ObjPCLExamTypesAndPriceIsActive_ByPCLGroupID_All_Virtual_Delete;
            };
            GlobalsNAV.ShowDialog<IPCLExamTypePriceList_ChooseFromDelete>(onInitDlg);
        }
        #endregion
        
        #region LostFocus
        public void LostFocus_EffectiveDate(object EffectiveDate)
        {
            if (EffectiveDate != null)
            {
                /*▼====: #001*/
                DateTime V = Globals.GetCurServerDateTime();
                /*▲====: #001*/
                DateTime.TryParse(EffectiveDate.ToString(), out V);
            }
            else
            {
                ObjPCLExamTypePriceList_Current.EffectiveDate = dtCurrentServer;
            }
        }
        #endregion
        
        public void Handle(SelectedObjectEvent<PCLExamType> message)
        {
            if (message != null)
            {
                PCLExamType Objtmp = message.Result;
                UnMarkDeleteRow_VirList(Objtmp);
                SetbtChooseItemFromDelete_IsEnabled();
            }
        }

        private void SetbtChooseItemFromDelete_IsEnabled()
        {
            if (ObjPCLExamTypesAndPriceIsActive_ByPCLGroupID_All_Virtual_Delete.Count > 0)
            {
                btChooseItemFromDelete_IsEnabled = true;
            }
            else
            {
                btChooseItemFromDelete_IsEnabled = false;
            }
        }
    }
}
