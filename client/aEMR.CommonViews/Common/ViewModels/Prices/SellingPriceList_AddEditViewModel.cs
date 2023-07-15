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
using Caliburn.Micro;
using DataEntities;
using eHCMSLanguage;
using aEMR.ViewContracts;
using aEMR.Infrastructure.Events;
using Castle.Windsor;
using aEMR.Infrastructure.CachingUtils;
using aEMR.Common.Collections;
using aEMR.ServiceClient;
using aEMR.Infrastructure;
using aEMR.DataContracts;
using aEMR.Common;
using aEMR.ViewContracts.Configuration;

namespace aEMR.Configuration.PCLExamTypePriceList.ViewModels
{
    [Export(typeof(ISellingPriceList_AddEdit)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class SellingPriceList_AddEditViewModel : Conductor<object>, ISellingPriceList_AddEdit
        , IHandle<SelectedObjectEvent<PCLExamType>>
    {
        IEventAggregator _eventArg;
        [ImportingConstructor]
        public SellingPriceList_AddEditViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg, ISalePosCaching salePosCaching)
        {
            _eventArg = eventArg;
        }
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
        private int VirList_IndexCurrent = -1;//ban đầu phân trang chạy từ 0
        private bool VirList_IsEndPage = false;

        private DateTime EffectiveDate_tmp;

        private DateTime dtCurrentServer;

        //public void GetCurrentDate(bool ForAddNew)
        //{
        //    var t = new Thread(() =>
        //    {
        //        try
        //        {
        //            using (var serviceFactory = new CommonServiceClient())
        //            {
        //                var contract = serviceFactory.ServiceInstance;

        //                contract.BeginGetDate(Globals.DispatchCallback((asyncResult) =>
        //                {
        //                    try
        //                    {
        //                        dtCurrentServer = contract.EndGetDate(asyncResult);
        //                        if(ForAddNew)
        //                        {
        //                            InitializeNewItem();
        //                        }
        //                    }
        //                    catch (FaultException<AxException> fault)
        //                    {
        //                        ClientLoggerHelper.LogInfo(fault.ToString());
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        ClientLoggerHelper.LogInfo(ex.ToString());
        //                    }

        //                }), null);
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
        //        }
        //    });
        //    t.Start();
        //}

        protected override void OnActivate()
        {
            _eventArg.Subscribe(this);

            base.OnActivate();

            VirList_IndexCurrent = -1;
            VirList_IsEndPage = false;

            SearchCriteria = new PCLExamTypeSearchCriteria();
            //SearchCriteria.V_PCLCategory = -1;
            //SearchCriteria.PCLGroupID =-1;

            ObjPCLExamTypesAndPriceIsActive_Paging = new PagedSortableCollectionView<PCLExamType>();
            ObjPCLExamTypesAndPriceIsActive_Paging.PageSize = 100;
            ObjPCLExamTypesAndPriceIsActive_Paging.OnRefresh += new EventHandler<RefreshEventArgs>(ObjPCLExamTypesAndPriceIsActive_Paging_OnRefresh);

            ObjPCLExamTypesAndPriceIsActive_Paging_Virtual=new PagedSortableCollectionView<PCLExamType>();
            ObjPCLExamTypesAndPriceIsActive_Paging_Save=new PagedSortableCollectionView<PCLExamType>();
            ObjPCLExamTypesAndPriceIsActive_ByPCLGroupID_All_Virtual_Delete = new ObservableCollection<PCLExamType>();

            LoadList();
            

            if (ObjPCLExamTypePriceList_Current.PCLExamTypePriceListID > 0)//Update
            {
                btSave_IsEnabled = true;
                EffectiveDate_tmp = ObjPCLExamTypePriceList_Current.EffectiveDate.Value;

                dgCellTemplate0_Visible = Visibility.Collapsed;

                //if (ObjPCLExamTypePriceList_Current.PriceListType == "PriceList-InFuture")
                //{
                //    dpEffectiveDate_IsEnabled = true;
                //}
                //else
                //{
                //    dpEffectiveDate_IsEnabled = false;
                //}

                btChooseItemFromDelete_IsEnabled = false;

                //if (ObjPCLExamTypePriceList_Current.PriceListType == "PriceList-Old")
                if (!ObjPCLExamTypePriceList_Current.IsActive
                    && ((DateTime)ObjPCLExamTypePriceList_Current.EffectiveDate).Date < BeginDate.Date)
                {
                    MessageBox.Show(eHCMSResources.A0205_G1_Msg_InfoKhDcSuaBGiaCu, eHCMSResources.Z0665_G1_SuaBGia, MessageBoxButton.OK);
                    btSave_IsEnabled = false;
                }

            }
            else
            {
                dgCellTemplate0_Visible = Visibility.Visible;
                dpEffectiveDate_IsEnabled = true;
                btChooseItemFromDelete_IsEnabled = true;
                btSave_IsEnabled = true;
            }
        }

        void ObjPCLExamTypesAndPriceIsActive_Paging_OnRefresh(object sender, RefreshEventArgs e)
        {
            if (VirList_IsEndPage == false)
            {
                if (ObjPCLExamTypesAndPriceIsActive_Paging.PageIndex > VirList_IndexCurrent)
                {
                    PCLExamTypesAndPriceIsActive_Paging(ObjPCLExamTypesAndPriceIsActive_Paging.PageIndex, ObjPCLExamTypesAndPriceIsActive_Paging.PageSize, false);
                    VirList_IndexCurrent = ObjPCLExamTypesAndPriceIsActive_Paging.PageIndex;

                    if ((ObjPCLExamTypesAndPriceIsActive_Paging).PageCount == VirList_IndexCurrent + 1)
                    {
                        VirList_IsEndPage = true;
                    }

                }
                else //Đọc từ ObjGetDeptMedServiceItems_Paging_Virtual
                {
                    PagingLinq(ObjPCLExamTypesAndPriceIsActive_Paging.PageIndex, ObjPCLExamTypesAndPriceIsActive_Paging.PageSize);
                }
            }
            else
            {
                PagingLinq(ObjPCLExamTypesAndPriceIsActive_Paging.PageIndex, ObjPCLExamTypesAndPriceIsActive_Paging.PageSize);
            }
        }

        private void PagingLinq(int pIndex, int pPageSize)
        {
            var ResultAll = from p in ObjPCLExamTypesAndPriceIsActive_Paging_Virtual.ToObservableCollection()
                            select p;
            List<PCLExamType> Items = ResultAll.Skip(pIndex * pPageSize).Take(pPageSize).ToList();
            ShowItemsOnList(Items);
        }

        private void ShowItemsOnList(List<PCLExamType> ObjCollect)
        {
            ObjPCLExamTypesAndPriceIsActive_Paging.Clear();
            foreach (PCLExamType item in ObjCollect)
            {
                if (item.ObjPCLExamTypePrice.RowState != PCLExamTypePrice.RowStateValue.Delete)
                {
                    ObjPCLExamTypesAndPriceIsActive_Paging.Add(item);
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

        private void LoadList()
        {
            if (ObjPCLExamTypePriceList_Current.PCLExamTypePriceListID > 0)//TH Update
            {
                SearchCriteria.PCLExamTypePriceListID = ObjPCLExamTypePriceList_Current.PCLExamTypePriceListID;
                ObjPCLExamTypesAndPriceIsActive_Paging.PageIndex = 0;
                PCLExamTypePriceList_Detail(0, ObjPCLExamTypesAndPriceIsActive_Paging.PageSize, true);
            }
            else//When AddNew
            {
                ObjPCLExamTypesAndPriceIsActive_Paging.PageIndex = 0;
                PCLExamTypesAndPriceIsActive_Paging(0, ObjPCLExamTypesAndPriceIsActive_Paging.PageSize, true);
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


        private PagedSortableCollectionView<PCLExamType> _ObjPCLExamTypesAndPriceIsActive_Paging;
        public PagedSortableCollectionView<PCLExamType> ObjPCLExamTypesAndPriceIsActive_Paging
        {
            get { return _ObjPCLExamTypesAndPriceIsActive_Paging; }
            set
            {
                _ObjPCLExamTypesAndPriceIsActive_Paging = value;
                NotifyOfPropertyChange(() => ObjPCLExamTypesAndPriceIsActive_Paging);
            }
        }

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

        private void PCLExamTypesAndPriceIsActive_Paging(int PageIndex, int PageSize, bool CountTotal)
        {
            //Globals.EventAggregator.Publish(new BusyEvent() { IsBusy = true, Message = string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.K3014_G1_DSPCLExamType) });

            var t = new Thread(() =>
            {
                IsLoading = true;

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
                            }

                            ObjPCLExamTypesAndPriceIsActive_Paging.Clear();
                            if (bOK)
                            {
                                if (CountTotal)
                                {
                                    ObjPCLExamTypesAndPriceIsActive_Paging.TotalItemCount = Total;
                                }
                                if (allItems != null)
                                {
                                    foreach (var item in allItems)
                                    {
                                        ObjPCLExamTypesAndPriceIsActive_Paging.Add(item);
                                    }
                                    AjustItemsToVirList(PageIndex);
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

        private void AjustItemsToVirList(int pPageIndex)
        {
            if (pPageIndex > VirList_IndexCurrent - 1)
            {
                //Cộng dồn vô
                foreach (PCLExamType item in ObjPCLExamTypesAndPriceIsActive_Paging)
                {
                    ObjPCLExamTypesAndPriceIsActive_Paging_Virtual.Add(item);
                }
            }
        }

        public void hplDelete_Click(object selectItem)//Chỉ có trong TH Tạo Mới Bảng Giá
        {
            PCLExamType p = selectItem as PCLExamType;

            if (ObjPCLExamTypePriceList_Current.PCLExamTypePriceListID <= 0) //TH Tạo Bảng Giá
            {
                MarkDeleteRow_VirList(p);
                ObjPCLExamTypesAndPriceIsActive_Paging.Remove(p);

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

            PagingLinq(ObjPCLExamTypesAndPriceIsActive_Paging.PageIndex, ObjPCLExamTypesAndPriceIsActive_Paging.PageSize);
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

        public void KhoiTaoForAddNew()
        {
            ObjPCLExamTypePriceList_Current = new DataEntities.PCLExamTypePriceList();
            ObjPCLExamTypePriceList_Current.EffectiveDate = BeginDate;
            
            // TxD 02/08/2014 Use Global Server Date instead
            //GetCurrentDate(true);
            dtCurrentServer = Globals.GetCurServerDateTime();
            InitializeNewItem();
        }

        public void InitializeNewItem()
        {
            
            ObjPCLExamTypePriceList_Current.PCLExamTypePriceListID = 0;
            //ObjPCLExamTypePriceList_Current.EffectiveDate = dtCurrentServer;
            ObjPCLExamTypePriceList_Current.EffectiveDate = BeginDate;
            ObjPCLExamTypePriceList_Current.StaffID = Globals.LoggedUserAccount.Staff.StaffID;
            ObjPCLExamTypePriceList_Current.ApprovedStaffID = ObjPCLExamTypePriceList_Current.StaffID;

        }

        #region Color Row and Cell
        public void dtgList_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            PCLExamType objRows = e.Row.DataContext as PCLExamType;
            if (objRows != null)
            {
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
            switch (objRows.ObjPCLExamTypePrice.PriceType)
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
                default:
                    {
                        textBlock.Foreground = new SolidColorBrush(Colors.Black);
                        break;
                    }
            }
        }

        DataGridRow row = null;
        DataGridColumn column = null;
        public void dtgList_PreparingCellForEdit(object sender, DataGridPreparingCellForEditEventArgs e)
        {
            column = e.Column;
            row = e.Row;

            int IndexCol = e.Column.DisplayIndex;
            string StringNameControl = GetStringNameControl(IndexCol);
            TextBox Ctrtb = ((sender as DataGrid)).CurrentColumn.GetCellContent(e.Row).FindName(StringNameControl) as TextBox;
            if (Ctrtb != null)
            {
                Ctrtb.Focus();
                Ctrtb.SelectAll();
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
                int IndexColumn = (((System.Windows.Controls.DataGrid)(sender)).CurrentColumn).DisplayIndex;

                TextBlock textBlock = (column.GetCellContent(row) as TextBlock);

                if (textBlock != null)
                {
                    if (CheckCellValueChange_UpdateRowStatus(sender, IndexColumn))
                    {
                        //textBlock.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(250, 120, 150, 111));
                        textBlock.Foreground = new SolidColorBrush(Colors.Brown);
                    }
                    else
                    {
                        PCLExamType objRows = ((PCLExamType)(((System.Windows.Controls.DataGrid)(sender)).SelectedItem));

                        switch (objRows.ObjPCLExamTypePrice.PriceType)
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
                            MessageBox.Show(string.Format("{0}!", eHCMSResources.A0520_G1_Msg_InfoDGiaLonHonGiaBNBH));
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
                if (NormalPrice == NormalPrice_Old && PriceForHIPatient == PriceForHIPatient_Old &&
                    HIAllowedPrice == HIAllowedPrice_Old)
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
                    if ((ObjPCLExamTypesAndPriceIsActive_Paging).PageCount==1)
                    {
                        VirList_IsEndPage = true;
                    }

                    if (VirList_IsEndPage == false)
                    {
                        if (MessageBox.Show(eHCMSResources.A0805_G1_Msg_InfoDMChuaDuyetHet + Environment.NewLine + eHCMSResources.Z1086_G1_CoMuonTaoBGiaNayKg, eHCMSResources.T2937_G1_Luu, MessageBoxButton.OKCancel) ==
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
            ObjPCLExamTypesAndPriceIsActive_Paging_Save.Clear();

            foreach (PCLExamType item in ObjPCLExamTypesAndPriceIsActive_Paging_Virtual)
            {
                //if (item.ObjPCLExamTypePrice.RowState != PCLExamTypePrice.RowStateValue.Delete)
                {
                    ObjPCLExamTypesAndPriceIsActive_Paging_Save.Add(item);
                }
            }
        }

        private void ThuTucAddNew()
        {
            ListAddNew_Final();

            if (ObjPCLExamTypesAndPriceIsActive_Paging_Save.Count > 0)
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
            foreach (PCLExamType item in ObjPCLExamTypesAndPriceIsActive_Paging_Save)
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
                    message = string.Format("{0}!", eHCMSResources.Z1122_G1_DGiaNhoHon1);
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

            foreach (PCLExamType item in ObjPCLExamTypesAndPriceIsActive_Paging)
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
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.Z0645_G1_DangGhi) });

            var t = new Thread(() =>
            {
                IsLoading = true;

                using (var serviceFactory = new ConfigurationManagerServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginPCLExamTypePriceList_AddNew(ObjPCLExamTypePriceList_Current, ObjPCLExamTypesAndPriceIsActive_Paging_Save, Globals.DispatchCallback((asyncResult) =>
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

                                        MessageBox.Show(string.Format("{0}!", eHCMSResources.A0468_G1_Msg_InfoLuuOK), eHCMSResources.T0780_G1_TaoBGiaMoi, MessageBoxButton.OK);
                                        break;
                                    }
                                case "Has-PriceList-Future":
                                    {
                                        MessageBox.Show(eHCMSResources.A0447_G1_Msg_InfoDaCoBGiaTLai + eHCMSResources.A0449_G1_Msg_InfoUseCNhatBGia, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                        break;
                                    }
                                case "Has-PriceList-Current":
                                    {
                                        MessageBox.Show(string.Format("{0} ", eHCMSResources.A0445_G1_Msg_InfoDaCoBGiaHHanhChoThang) + ObjPCLExamTypePriceList_Current.EffectiveDate.Value.Month.ToString() + string.Format("! {0}", eHCMSResources.A0449_G1_Msg_InfoUseCNhatBGia), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
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
        private void PCLExamTypePriceList_Detail(int PageIndex, int PageSize, bool CountTotal)
        {
            //Globals.EventAggregator.Publish(new BusyEvent() { IsBusy = true, Message = string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.K2945_G1_DSDV) });

            var t = new Thread(() =>
            {
                IsLoading = true;

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
                            }

                            ObjPCLExamTypesAndPriceIsActive_Paging.Clear();

                            if (bOK)
                            {
                                if (CountTotal)
                                {
                                    ObjPCLExamTypesAndPriceIsActive_Paging.TotalItemCount = Total;
                                }
                                if (allItems != null)
                                {
                                    foreach (var item in allItems)
                                    {
                                        //Update trạng thái khi Update Ban Đầu là NoChange
                                        SetStatusRowForUpdate(item);
                                        //Update trạng thái khi Update Ban Đầu là NoChange

                                        ObjPCLExamTypesAndPriceIsActive_Paging.Add(item);
                                    }

                                    AjustItemsToVirList(PageIndex);
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


        private bool _dpEffectiveDate_IsEnabled;
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

        private void SetStatusRowForUpdate(PCLExamType item)
        {
            item.ObjPCLExamTypePrice.RowState = PCLExamTypePrice.RowStateValue.NoChange;
        }

        //Kiểm Tra Ngày Cho Update Bảng Giá
        private string CheckEffectiveDate_PriceList_Update()
        {
            if (ObjPCLExamTypePriceList_Current.EffectiveDate.Value.Subtract(EffectiveDate_tmp).Days <= 0)
            {
                return eHCMSResources.Z1247_G1_EditPriceListCurrent;
            }
            else
            {
                return eHCMSResources.Z1248_G1_EditPriceListFuture;
            }
        }
        //Kiểm Tra Ngày Cho Update Bảng Giá

        private void ListUpdate_Final()
        {
            ObjPCLExamTypesAndPriceIsActive_Paging_Save.Clear();

            foreach (PCLExamType item in ObjPCLExamTypesAndPriceIsActive_Paging_Virtual)
            {
                if (item.ObjPCLExamTypePrice.RowState == PCLExamTypePrice.RowStateValue.Update)
                {
                    ObjPCLExamTypesAndPriceIsActive_Paging_Save.Add(item);
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
                            PCLExamTypePriceList_Update(ObjPCLExamTypePriceList_Current,ObjPCLExamTypesAndPriceIsActive_Paging_Save);
                            break;
                        }
                    case "Edit-PriceList-Future":
                        {
                            if (ObjPCLExamTypePriceList_Current.EffectiveDate.Value.Subtract(dtCurrentServer).Days > 0)
                            {
                                PCLExamTypePriceList_Update(ObjPCLExamTypePriceList_Current,ObjPCLExamTypesAndPriceIsActive_Paging_Save);
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

        public void PCLExamTypePriceList_Update(DataEntities.PCLExamTypePriceList Obj, ObservableCollection<PCLExamType> ObjCollection_Update)
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.Z0645_G1_DangGhi) });

            var t = new Thread(() =>
            {
                IsLoading = true;

                using (var serviceFactory = new ConfigurationManagerServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginPCLExamTypePriceList_Update(Obj,ObjCollection_Update, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            string Result_PriceList = "";

                            contract.EndPCLExamTypePriceList_Update(out Result_PriceList, asyncResult);
                            switch (Result_PriceList)
                            {
                                case "Update-0":
                                    {
                                        MessageBox.Show(string.Format("{0}!", eHCMSResources.A0606_G1_Msg_InfoHChinhBGiaFail), eHCMSResources.A0470_G1_Msg_HChinhBGia, MessageBoxButton.OK);
                                        break;
                                    }
                                case "Update-1":
                                    {
                                        Globals.EventAggregator.Publish(new SaveEvent<bool>() { Result = true });
                                        MessageBox.Show(string.Format("{0}!", eHCMSResources.A0468_G1_Msg_InfoLuuOK), eHCMSResources.A0470_G1_Msg_HChinhBGia, MessageBoxButton.OK);
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
                DateTime V = Globals.ServerDate.Value;
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