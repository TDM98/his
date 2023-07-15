using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using Caliburn.Micro;
using DataEntities;
using aEMR.Controls;
using aEMR.Common;
using eHCMSLanguage;
using Castle.Windsor;
using System.Windows.Data;
using aEMR.Common.BaseModel;
using aEMR.Infrastructure.CachingUtils;
using Castle.Core.Logging;
/*
 * 20180402 #001 CMN: Allow HIAllowedPrice equal 0 with InsuranceCover
 * 20181128 #002 TTM: Thay đổi cách lấy dữ liệu trong CellEditEnding.
 */
namespace aEMR.DrugDept.ViewModels
{
    [Export(typeof(IDrugDeptSellingPriceList_AddEdit)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class DrugDeptSellingPriceList_AddEditViewModel : ViewModelBase, IDrugDeptSellingPriceList_AddEdit
    , IHandle<SelectedObjectEvent<DrugDeptSellingItemPrices>>
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        IEventAggregator _eventArg;
        [ImportingConstructor]
        public DrugDeptSellingPriceList_AddEditViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg)
        {
            _eventArg = eventArg;
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
        private DrugDeptSellingPriceList _ObjDrugDeptSellingPriceList_Current;
        public DrugDeptSellingPriceList ObjDrugDeptSellingPriceList_Current
        {
            get { return _ObjDrugDeptSellingPriceList_Current; }
            set
            {
                _ObjDrugDeptSellingPriceList_Current = value;
                NotifyOfPropertyChange(() => ObjDrugDeptSellingPriceList_Current);
            }
        }

        private DateTime EffectiveDate_tmp;

        protected override void OnActivate()
        {
            Globals.EventAggregator.Subscribe(this);

            base.OnActivate();


            ObjDrugDeptSellingPriceList_AutoCreate = new ObservableCollection<DrugDeptSellingItemPrices>();

            ObjDrugDeptSellingPriceList_AutoCreate_Save = new ObservableCollection<DrugDeptSellingItemPrices>();

            //ObjDrugDeptSellingPriceList_AutoCreate_Paging = new PagedSortableCollectionView<DrugDeptSellingItemPrices>();
            //ObjDrugDeptSellingPriceList_AutoCreate_Paging.PageSize = 100;

            //ObjDrugDeptSellingPriceList_AutoCreate_Paging.OnRefresh += new EventHandler<RefreshEventArgs>(ObjDrugDeptSellingPriceList_AutoCreate_Paging_OnRefresh);

            ObjDrugDeptSellingItemPrices_All_Virtual_Delete = new ObservableCollection<DrugDeptSellingItemPrices>();


            if (ObjDrugDeptSellingPriceList_Current != null && ObjDrugDeptSellingPriceList_Current.DrugDeptSellingPriceListID > 0)/*Update*/
            {
                DrugDeptSellingPriceList_Detail(ObjDrugDeptSellingPriceList_Current.DrugDeptSellingPriceListID);

                EffectiveDate_tmp = ObjDrugDeptSellingPriceList_Current.EffectiveDate;

                //dpEffectiveDate_IsEnabled = false;
                btChooseItemFromDelete_IsEnabled = false;


                //if (ObjDrugDeptSellingPriceList_Current.PriceListType == "PriceList-Old")
                if (!ObjDrugDeptSellingPriceList_Current.IsActive
                    && ObjDrugDeptSellingPriceList_Current.EffectiveDate.Date <= BeginDate.Date)
                {
                    btSave_IsEnabled = false;
                    MessageBox.Show(eHCMSResources.A0205_G1_Msg_InfoKhDcSuaBGiaCu, eHCMSResources.Z0665_G1_SuaBGia, MessageBoxButton.OK);
                }
                else
                {
                    btSave_IsEnabled = true;
                }

            }
            else/*AddNew*/
            {
                InitializeNewItem();

                DrugDeptSellingPriceList_AutoCreate();

                //dpEffectiveDate_IsEnabled = true;
                btChooseItemFromDelete_IsEnabled = false;
                btSave_IsEnabled = true;
            }
        }

        //void ObjDrugDeptSellingPriceList_AutoCreate_Paging_OnRefresh(object sender, RefreshEventArgs e)
        //{
        //    PagingLinq(ObjDrugDeptSellingPriceList_AutoCreate_Paging.PageIndex, ObjDrugDeptSellingPriceList_AutoCreate_Paging.PageSize);
        //}


        //private void PagingLinq(int pIndex, int pPageSize)
        //{
        //    var ResultAll = from p in ObjDrugDeptSellingPriceList_AutoCreate.ToObservableCollection()
        //                    select p;
        //    List<DrugDeptSellingItemPrices> Items = ResultAll.Skip(pIndex * pPageSize).Take(pPageSize).ToList();
        //    ShowItemsOnList(Items);
        //}
        //private void ShowItemsOnList(List<DrugDeptSellingItemPrices> ObjCollect)
        //{
        //    ObjDrugDeptSellingPriceList_AutoCreate_Paging.Clear();
        //    foreach (DrugDeptSellingItemPrices item in ObjCollect)
        //    {
        //        if (item.RowState != DrugDeptSellingItemPrices.RowStateValue.Delete)
        //        {
        //            ObjDrugDeptSellingPriceList_AutoCreate_Paging.Add(item);
        //        }
        //    }
        //}

        private CollectionViewSource CVS_ObjDrugDeptSellingPriceList_AutoCreate_Paging = null;
        public CollectionView CV_ObjDrugDeptSellingPriceList_AutoCreate_Paging
        {
            get; set;
        }

        //private PagedCollectionView _ObjDrugDeptSellingPriceList_AutoCreate_Paging;
        //public PagedCollectionView ObjDrugDeptSellingPriceList_AutoCreate_Paging
        //{
        //    get { return _ObjDrugDeptSellingPriceList_AutoCreate_Paging; }
        //    set
        //    {
        //        _ObjDrugDeptSellingPriceList_AutoCreate_Paging = value;
        //        NotifyOfPropertyChange(() => ObjDrugDeptSellingPriceList_AutoCreate_Paging);
        //    }
        //}

        private ObservableCollection<DrugDeptSellingItemPrices> _ObjDrugDeptSellingPriceList_AutoCreate_Save;
        public ObservableCollection<DrugDeptSellingItemPrices> ObjDrugDeptSellingPriceList_AutoCreate_Save
        {
            get { return _ObjDrugDeptSellingPriceList_AutoCreate_Save; }
            set
            {
                _ObjDrugDeptSellingPriceList_AutoCreate_Save = value;
                NotifyOfPropertyChange(() => ObjDrugDeptSellingPriceList_AutoCreate_Save);
            }
        }


        private ObservableCollection<DrugDeptSellingItemPrices> _ObjDrugDeptSellingPriceList_AutoCreate;
        public ObservableCollection<DrugDeptSellingItemPrices> ObjDrugDeptSellingPriceList_AutoCreate
        {
            get { return _ObjDrugDeptSellingPriceList_AutoCreate; }
            set
            {
                _ObjDrugDeptSellingPriceList_AutoCreate = value;
                NotifyOfPropertyChange(() => ObjDrugDeptSellingPriceList_AutoCreate);
            }
        }

        private void DrugDeptSellingPriceList_AutoCreate()
        {
            ObjDrugDeptSellingPriceList_AutoCreate.Clear();
            this.DlgShowBusyIndicator(string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.Z0667_G1_DSGiaBan));
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyDrugServiceClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginDrugDeptSellingPriceList_AutoCreate(V_MedProductType, Globals.DispatchCallback((asyncResult) =>
                        {
                            List<DrugDeptSellingItemPrices> allItems = null;
                            try
                            {
                                string Result = "";
                                allItems = client.EndDrugDeptSellingPriceList_AutoCreate(out Result, asyncResult);
                                switch (Result)
                                {
                                    case "OK":
                                        {
                                            btSave_IsEnabled = true;
                                            dtgListIsEnabled = true;

                                            ObjDrugDeptSellingPriceList_AutoCreate = new ObservableCollection<DrugDeptSellingItemPrices>(allItems);
                                            LoadDataGrid();
                                            //ObjDrugDeptSellingPriceList_AutoCreate_Paging.TotalItemCount = allItems.Count;
                                            //PagingLinq(0, ObjDrugDeptSellingPriceList_AutoCreate_Paging.PageSize);
                                            break;
                                        }
                                    case "ChuaCoCongThucGia":
                                        {
                                            btSave_IsEnabled = false;
                                            dtgListIsEnabled = false;
                                            MessageBox.Show(eHCMSResources.Z0336_G1_ChuaCoCThucKgTheTaoBGia, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                            break;
                                        }
                                    case "Error":
                                        {
                                            btSave_IsEnabled = false;
                                            dtgListIsEnabled = false;
                                            MessageBox.Show(string.Format("{0}!", eHCMSResources.Z0666_G1_CoLoiTaoBGia), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
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

        private void DrugDeptSellingPriceList_Detail(Int64 DrugDeptSellingPriceListID)
        {
            ObjDrugDeptSellingPriceList_AutoCreate.Clear();
            this.DlgShowBusyIndicator(string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.Z0667_G1_DSGiaBan));
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyDrugServiceClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginDrugDeptSellingPriceList_Detail(DrugDeptSellingPriceListID, Globals.DispatchCallback((asyncResult) =>
                        {
                            List<DrugDeptSellingItemPrices> allItems = null;
                            try
                            {
                                allItems = client.EndDrugDeptSellingPriceList_Detail(asyncResult);
                                if (allItems != null)
                                {
                                    dtgListIsEnabled = true;

                                    ObjDrugDeptSellingPriceList_AutoCreate = new ObservableCollection<DrugDeptSellingItemPrices>(allItems);
                                    LoadDataGrid();
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

        private ObservableCollection<DrugDeptSellingItemPrices> _ObjDrugDeptSellingItemPrices_All_Virtual_Delete;
        public ObservableCollection<DrugDeptSellingItemPrices> ObjDrugDeptSellingItemPrices_All_Virtual_Delete
        {
            get { return _ObjDrugDeptSellingItemPrices_All_Virtual_Delete; }
            set
            {
                if (_ObjDrugDeptSellingItemPrices_All_Virtual_Delete != value)
                {
                    _ObjDrugDeptSellingItemPrices_All_Virtual_Delete = value;
                    NotifyOfPropertyChange(() => ObjDrugDeptSellingItemPrices_All_Virtual_Delete);
                }
            }
        }

        public void hplDelete_Click(object selectItem)//Chỉ có trong TH Tạo Mới Bảng Giá
        {
            DataEntities.DrugDeptSellingItemPrices p = selectItem as DataEntities.DrugDeptSellingItemPrices;

            if (ObjDrugDeptSellingPriceList_Current.DrugDeptSellingPriceListID <= 0) //TH Tạo Bảng Giá
            {
                MarkDeleteRow_VirList(p);
                ObjDrugDeptSellingPriceList_AutoCreate.Remove(p);
                btChooseItemFromDelete_IsEnabled = true;
            }
        }

        private void MarkDeleteRow_VirList(DataEntities.DrugDeptSellingItemPrices p)
        {
            foreach (DataEntities.DrugDeptSellingItemPrices item in ObjDrugDeptSellingPriceList_AutoCreate)
            {
                if (item.GenMedProductID == p.GenMedProductID)
                {
                    p.RowState = DataEntities.DrugDeptSellingItemPrices.RowStateValue.Delete;
                    //Add vào các mục đã xóa để cho chọn lại
                    AddMarkDeleteRow_VirList(p);
                    //Add vào các mục đã xóa để cho chọn lại
                    break;
                }
            }
        }
        private void AddMarkDeleteRow_VirList(DataEntities.DrugDeptSellingItemPrices p)
        {
            ObjDrugDeptSellingItemPrices_All_Virtual_Delete.Add(p);
        }
        //private void UnMarkDeleteRow_VirList(DataEntities.DrugDeptSellingItemPrices p)
        //{
        //    foreach (DataEntities.DrugDeptSellingItemPrices item in ObjDrugDeptSellingPriceList_AutoCreate)
        //    {
        //        if (item.GenMedProductID == p.GenMedProductID)
        //        {
        //            //Check lại 
        //            CheckValueChangeAfterUnMarkDelete(p);

        //            ObjDrugDeptSellingItemPrices_All_Virtual_Delete.Remove(p);

        //            break;
        //        }
        //    }

        //    PagingLinq(ObjDrugDeptSellingPriceList_AutoCreate_Paging.PageIndex, ObjDrugDeptSellingPriceList_AutoCreate_Paging.PageSize);
        //}

        //private void CheckValueChangeAfterUnMarkDelete(DataEntities.DrugDeptSellingItemPrices p)
        //{
        //    decimal NormalPrice = p.NormalPrice;
        //    decimal NormalPrice_Old = p.NormalPrice_Old;

        //    decimal? PriceForHIPatient = p.PriceForHIPatient;
        //    decimal? PriceForHIPatient_Old = p.PriceForHIPatient_Old;

        //    decimal? HIAllowedPrice = p.HIAllowedPrice;
        //    decimal? HIAllowedPrice_Old = p.HIAllowedPrice_Old;

        //    //Đánh dấu trạng thái dòng--> so 3 cột

        //    if (NormalPrice == NormalPrice_Old && PriceForHIPatient == PriceForHIPatient_Old && HIAllowedPrice == HIAllowedPrice_Old)
        //    {
        //        p.RowState = DataEntities.DrugDeptSellingItemPrices.RowStateValue.NoChange;
        //    }
        //    else
        //    {
        //        p.RowState = DataEntities.DrugDeptSellingItemPrices.RowStateValue.Update;
        //    }
        //}        

        public void InitializeNewItem()
        {
            ObjDrugDeptSellingPriceList_Current = new DrugDeptSellingPriceList
            {
                DrugDeptSellingPriceListID = 0,
                EffectiveDate = BeginDate,
                V_MedProductType = V_MedProductType,
                StaffID = Globals.LoggedUserAccount.Staff.StaffID,
                ApprovedStaffID = Globals.LoggedUserAccount.Staff.StaffID,
                ObjDrugDeptSellingItemPrices = new ObservableCollection<DrugDeptSellingItemPrices>()
            };
        }

        //#region Color Row and Cell
        public void dtgList_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = e.Row.GetIndex() + 1;
            DrugDeptSellingItemPrices objRows = e.Row.DataContext as DrugDeptSellingItemPrices;
            if (objRows != null)
            {
                if (objRows.SuggestPrice > objRows.NormalPrice)
                    e.Row.Background = new SolidColorBrush(Colors.Red);
                else
                if (Math.Abs(objRows.SuggestPrice - objRows.NormalPrice) > 1)
                    e.Row.Background = new SolidColorBrush(Colors.Yellow);
                else
                    e.Row.Background = new SolidColorBrush(Colors.White);

                //KMx: Giá vốn đợt này khác giá vốn đợt trước (19/11/2014 17:50).
                //TextBox txtInCost = ((DataGridColumn)(sender as DataGrid).GetColumnByName("clInCost")).GetCellContent(e.Row).FindName("tbInCost") as TextBox;
                //if (objRows.InCost != objRows.InCostBefore)
                //{
                //    txtInCost.BorderBrush = new SolidColorBrush(Colors.Cyan);
                //    txtInCost.Background = new SolidColorBrush(Colors.Cyan);
                //}
                //else
                //{
                //    //Color mặc định ở file xaml (#E5E5E8).
                //    txtInCost.BorderBrush = new SolidColorBrush(Color.FromArgb(255, 0xE5, 0xE5, 0xE8));
                //    txtInCost.Background = new SolidColorBrush(Color.FromArgb(255, 0xE5, 0xE5, 0xE8));
                //}
            }
        }

        private void ForeColorByPriceType(DrugDeptSellingItemPrices objRows, TextBlock textBlock)
        {
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

            //TBR: 20181128 TTM: Dòng code này không hoạt động, comment lại 
            //int IndexCol = e.Column.DisplayIndex;
            //string StringNameControl = GetStringNameControl(IndexCol);
            //TextBox Ctrtb = ((sender as DataGrid)).CurrentColumn.GetCellContent(e.Row).FindName(StringNameControl) as TextBox;
            //if (Ctrtb != null)
            //{
            //    Ctrtb.Focus();
            //    Ctrtb.SelectAll();
            //}
            //
        }

        private string GetStringNameControl(int IndexCol)
        {
            string Result = "";
            switch (IndexCol)
            {
                case 6:
                    {
                        Result = "tbNormalPrice";
                        break;
                    }
                case 9:
                    {
                        Result = "tbPriceForHIPatient";
                        break;
                    }
                case 10:
                    {
                        Result = "tbHIAllowedPrice";
                        break;
                    }
            }
            return Result;
        }

        public void dtgList_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            //if (column != null && row != null)
            //{
            //    int IndexColumn = (((System.Windows.Controls.DataGrid)(sender)).CurrentColumn).DisplayIndex;

            //    TextBlock textBlock = (column.GetCellContent(row) as TextBlock);

            //    if (textBlock != null)
            //    {
            //        if (CheckCellValueChange_UpdateRowStatus(sender, IndexColumn))
            //        {
            //            //textBlock.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(250, 120, 150, 111));
            //            textBlock.Foreground = new SolidColorBrush(Colors.Brown);
            //        }
            //        else
            //        {
            //            textBlock.Foreground = new SolidColorBrush(Colors.Black);

            //            //DataEntities.DrugDeptSellingItemPrices objRows = ((DataEntities.DrugDeptSellingItemPrices)(((System.Windows.Controls.DataGrid)(sender)).SelectedItem));

            //            //switch (objRows.PriceType)
            //            //{
            //            //    case "PriceCurrent":
            //            //        {
            //            //            textBlock.Foreground = new SolidColorBrush(Colors.Green);
            //            //            break;
            //            //        }
            //            //    case "PriceFuture-Active-1":
            //            //        {
            //            //            textBlock.Foreground = new SolidColorBrush(Colors.Gray);
            //            //            break;
            //            //        }
            //            //    case "PriceFuture-Active-0":
            //            //        {
            //            //            textBlock.Foreground = new SolidColorBrush(Colors.Blue);
            //            //            break;
            //            //        }
            //            //}

            //        }
            //        row = null;
            //        column = null;
            //    }
            //}

            //▼====== #002: 
            if (column != null && row != null)
            {
                string colName = "";
                if (e.Column.Equals(dtgList.GetColumnByName("tbNormalPrice")))
                {
                    colName = "tbNormalPrice";
                }
                else if (e.Column.Equals(dtgList.GetColumnByName("tbPriceForHIPatient")))
                {
                    colName = "tbPriceForHIPatient";
                }
                else if (e.Column.Equals(dtgList.GetColumnByName("tbHIAllowedPrice")))
                {
                    colName = "tbHIAllowedPrice";
                }
                if (!string.IsNullOrEmpty(colName))
                {
                    //20181128 TTM: Thay đổi cấu trúc hàm CheckCellValueChange_UpdateRowStatus truyền vào string colName thay cho index vì nếu dùng index khi thay đổi thứ tự cột sẽ làm ảnh hưởng
                    //              đến logic.
                    CheckCellValueChange_UpdateRowStatus(sender, colName);
                }
                row = null;
                column = null;
            }
            //▲====== #002
        }

        private bool CheckCellValueChange_UpdateRowStatus(object sender, string colName)
        {
            bool Result = true;

            decimal NormalPrice = ((DrugDeptSellingItemPrices)(((DataGrid)(sender)).SelectedItem)).NormalPrice;
            decimal NormalPrice_Old = ((DrugDeptSellingItemPrices)(((DataGrid)(sender)).SelectedItem)).NormalPrice_Old;

            decimal? PriceForHIPatient = ((DrugDeptSellingItemPrices)(((DataGrid)(sender)).SelectedItem)).PriceForHIPatient;
            decimal? PriceForHIPatient_Old = ((DrugDeptSellingItemPrices)(((DataGrid)(sender)).SelectedItem)).PriceForHIPatient_Old;

            decimal? HIAllowedPrice = ((DrugDeptSellingItemPrices)(((DataGrid)(sender)).SelectedItem)).HIAllowedPrice;
            decimal? HIAllowedPrice_Old = ((DrugDeptSellingItemPrices)(((DataGrid)(sender)).SelectedItem)).HIAllowedPrice_Old;

            switch (colName)
            {
                case "tbNormalPrice":
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
                case "tbPriceForHIPatient":
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
                case "tbHIAllowedPrice":
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
            if (ObjDrugDeptSellingPriceList_Current.DrugDeptSellingPriceListID > 0)//Update 
            {
                if (NormalPrice == NormalPrice_Old && PriceForHIPatient == PriceForHIPatient_Old &&
                    HIAllowedPrice == HIAllowedPrice_Old)
                {
                    ((DrugDeptSellingItemPrices)(((DataGrid)(sender)).SelectedItem)).RowState = DrugDeptSellingItemPrices.RowStateValue.NoChange;
                }
                else
                {
                    ((DrugDeptSellingItemPrices)(((DataGrid)(sender)).SelectedItem)).RowState = DrugDeptSellingItemPrices.RowStateValue.Update;
                }
            }

            return Result;
        }

        //#endregion
        DataGrid dtgList = null;
        public void dtgList_Loaded(object sender, RoutedEventArgs e)
        {

            dtgList = sender as DataGrid;
            //if (ObjDrugDeptSellingPriceList_Current.DrugDeptSellingPriceListID > 0)
            //{
            //    dtgList.Columns[0].Visibility = Visibility.Collapsed;
            //}
        }

        public void btSave()
        {
            if (string.IsNullOrEmpty(ObjDrugDeptSellingPriceList_Current.PriceListTitle))
            {
                MessageBox.Show(eHCMSResources.A0882_G1_Msg_InfoNhapTieuDeBGia);
                return;
            }

            //if (CheckValidList(ObjDrugDeptSellingPriceList_Current)==false)
            //{
            //    return;
            //}

            if (ObjDrugDeptSellingPriceList_Current.DrugDeptSellingPriceListID == 0)
            {
                ListAddNew_Final();

                ThuTucAddNew();
            }
            else//Update
            {
                ListUpdate_Final();

                ThuTucUpDate();
            }
        }

        public void btClose()
        {
            TryClose();
        }

        //AddNew
        private void ListAddNew_Final()
        {
            ObjDrugDeptSellingPriceList_AutoCreate_Save.Clear();

            foreach (DrugDeptSellingItemPrices item in ObjDrugDeptSellingPriceList_AutoCreate)
            {
                if (item.RowState != DrugDeptSellingItemPrices.RowStateValue.Delete)
                {
                    ObjDrugDeptSellingPriceList_AutoCreate_Save.Add(item);
                }
            }

            ObjDrugDeptSellingPriceList_Current.ObjDrugDeptSellingItemPrices = ObjDrugDeptSellingPriceList_AutoCreate_Save;
        }

        private void ThuTucAddNew()
        {
            if (CheckValidPrice(ObjDrugDeptSellingPriceList_Current) == false)
            {
                return;
            }

            if (ObjDrugDeptSellingPriceList_AutoCreate_Save.Count > 0)
            {
                string message = "";
                if (CheckPriceBeforeSave(ref message))
                {
                    if (message != ""
                    && MessageBox.Show(message + string.Format("\n{0}", eHCMSResources.T3805_G1_BanCoMuonTiepTucKhong)
                    , string.Format("{0}!", eHCMSResources.G0442_G1_TBao), MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                    {
                        return;
                    }
                    //DrugDeptSellingItemPrices_EffectiveDateMax();
                    DrugDeptSellingPriceList_AddNew();
                }
            }
            else
            {
                MessageBox.Show(eHCMSResources.A0204_G1_Msg_BGiaChuaCoMucNao, eHCMSResources.T2937_G1_Luu, MessageBoxButton.OK);
            }
        }

        private bool CheckPriceBeforeSave(ref string message)
        {
            foreach (DrugDeptSellingItemPrices item in ObjDrugDeptSellingPriceList_AutoCreate_Save)
            {
                decimal NormalPrice = item.NormalPrice;
                decimal? PriceForHIPatient = item.PriceForHIPatient;
                decimal? HIAllowedPrice = item.HIAllowedPrice;

                if (NormalPrice > 0)
                {
                    if (PriceForHIPatient != null)
                    {
                        if (PriceForHIPatient > NormalPrice)
                        {
                            MessageBox.Show(item.BrandName + string.Format(": {0}", eHCMSResources.T1984_G1_GiaBHChoPhep3));
                            return false;
                        }
                    }
                    if (HIAllowedPrice != null)
                    {
                        if (HIAllowedPrice > PriceForHIPatient)
                        {
                            MessageBox.Show(item.BrandName + string.Format(": {0}!", eHCMSResources.Z0593_G1_GiaBHChoPhep));
                            return false;
                        }
                    }
                    //▼====: #001
                    //if (item.InsuranceCover.GetValueOrDefault())
                    if (item.InsuranceCover.GetValueOrDefault() && !Globals.ServerConfigSection.CommonItems.AllowZeroHIPriceWithFlag)
                    {
                        if (HIAllowedPrice.GetValueOrDefault(0) <= 0)
                        {
                            MessageBox.Show(item.BrandName + string.Format(": {0} !", eHCMSResources.T1980_G1_GiaBHKhongDuocNhoHon0));
                            return false;
                        }
                    }
                    //▲====: #001
                }
                else
                {
                    //MessageBox.Show(string.Format("{0}!", eHCMSResources.Z0644_G1_DGiaPhaiLonHonBang1));
                    message = string.Format("{0}!", eHCMSResources.Z0691_G1_DGiaNhoHonBang0);
                    //flag= false;
                }
            }

            return true;
        }

        public bool CheckValidList(DrugDeptSellingPriceList p)
        {
            if (p == null)
            {
                return false;
            }
            return p.Validate();
        }

        //Kiểm tra rõ ràng Template
        public bool CheckValidPrice(DrugDeptSellingPriceList p)
        {
            if (p.ObjDrugDeptSellingItemPrices == null)
            {
                MessageBox.Show(string.Format("{0}!", eHCMSResources.Z0692_G1_BGiaChuaCoDLieu), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return false;
            }

            double dem = 0;
            double TrangError = 0;
            foreach (DrugDeptSellingItemPrices item in ObjDrugDeptSellingPriceList_AutoCreate)
            {
                dem++;
                if (CheckBiXoa(item) == false)/*Không tính mấy cái xóa*/
                {
                    //if(CheckChuaCoPhieuNhapHang(item))
                    //{
                    //    TrangError = (dem % ObjDrugDeptSellingPriceList_AutoCreate_Paging.PageSize == 0) ? ((dem / ObjDrugDeptSellingPriceList_AutoCreate_Paging.PageSize == 0) ? 1 : dem / ObjDrugDeptSellingPriceList_AutoCreate_Paging.PageSize) : Math.Ceiling(dem / ObjDrugDeptSellingPriceList_AutoCreate_Paging.PageSize);
                    //    MessageBox.Show("'" + item.BrandName.Trim() +   "' tại trang: '" + TrangError.ToString() +"' " + Environment.NewLine + "Chưa Có Đợt Nhập Hàng Nào -> Không Hợp Lệ! Kiểm Tra Lại!", eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                    //    return false;
                    //}

                    // TxD 09/07/2018 BEGIN - Commented OUT the following because CollectionViewSource replacing PagedCollectionView DOES NOT HAVE PageSize property
                    //                       Also NOT QUITE SURE if the following code is actually required. If it is then we'll have to DO SOMETHING ELSE HERE ...
                    //if (item.Validate() == false)
                    //{
                    //    TrangError = (dem % CV_ObjDrugDeptSellingPriceList_AutoCreate_Paging.PageSize == 0) ? ((dem / ObjDrugDeptSellingPriceList_AutoCreate_Paging.PageSize == 0) ? 1 : dem / ObjDrugDeptSellingPriceList_AutoCreate_Paging.PageSize) : Math.Ceiling(dem / ObjDrugDeptSellingPriceList_AutoCreate_Paging.PageSize);
                    //    MessageBox.Show(string.Format(eHCMSResources.Z1310_G1_I, item.BrandName.Trim(), TrangError.ToString(), eHCMSResources.A0070_G1_Msg_InfoGiaLonHon0), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                    //    return false;
                    //}
                    // TxD 09/07/2018 END
                }
            }
            return true;
        }

        private bool CheckBiXoa(DrugDeptSellingItemPrices p)
        {
            foreach (DrugDeptSellingItemPrices item in ObjDrugDeptSellingItemPrices_All_Virtual_Delete)
            {
                if (item.GenMedProductID == p.GenMedProductID)
                {
                    return true;
                }
            }
            return false;
        }

        private bool CheckChuaCoPhieuNhapHang(DrugDeptSellingItemPrices p)
        {
            if (p.inviID <= 0)
                return true;
            return false;
        }
        //Kiểm tra rõ ràng Template

        private void DrugDeptSellingItemPrices_EffectiveDateMax()
        {
            this.DlgShowBusyIndicator(eHCMSResources.Z0693_G1_KTraNgApDung);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyDrugServiceClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginDrugDeptSellingItemPrices_EffectiveDateMax(Globals.DispatchCallback((asyncResult) =>
                        {
                            DateTime? dt = new DateTime();
                            try
                            {
                                dt = client.EndDrugDeptSellingItemPrices_EffectiveDateMax(asyncResult);
                                if (dt == null)
                                {
                                    DrugDeptSellingPriceList_AddNew();
                                }
                                else
                                {
                                    DateTime dtMax = dt.Value;
                                    if (ObjDrugDeptSellingPriceList_Current.EffectiveDate.Subtract(dtMax).Days < 0)
                                    {
                                        MessageBox.Show(string.Format(eHCMSResources.Z0694_G1_I, dtMax.ToString("dd/MM/yyyy")), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                    }
                                    else
                                    {
                                        DrugDeptSellingPriceList_AddNew();
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                ClientLoggerHelper.LogInfo(ex.ToString());
                                DrugDeptSellingPriceList_AddNew();
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
                    ClientLoggerHelper.LogInfo(ex.ToString());
                    this.DlgHideBusyIndicator();
                }
            });

            t.Start();
        }

        public void DrugDeptSellingPriceList_AddNew()
        {
            this.DlgShowBusyIndicator(eHCMSResources.Z0645_G1_DangGhi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyDrugServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginDrugDeptSellingPriceList_AddNew(ObjDrugDeptSellingPriceList_Current, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                string Result = "";
                                contract.EndDrugDeptSellingPriceList_AddNew(out Result, asyncResult);
                                switch (Result)
                                {
                                    case "Insert-0":
                                        {
                                            MessageBox.Show(eHCMSResources.A0997_G1_Msg_InfoTaoBGiaFail, eHCMSResources.T0780_G1_TaoBGiaMoi, MessageBoxButton.OK);
                                            break;
                                        }
                                    case "Insert-1":
                                        {
                                            btChooseItemFromDelete_IsEnabled = false;
                                            btSave_IsEnabled = false;
                                            dtgListIsEnabled = false;

                                            Globals.EventAggregator.Publish(new SaveEvent<bool>() { Result = true });

                                            MessageBox.Show(string.Format("{0}!", eHCMSResources.A0468_G1_Msg_InfoLuuOK), eHCMSResources.T0780_G1_TaoBGiaMoi, MessageBoxButton.OK);

                                            break;
                                        }
                                    case "Has-PriceList-Future":
                                        {
                                            MessageBox.Show(eHCMSResources.A0447_G1_Msg_InfoDaCoBGiaTLai + " " + eHCMSResources.A0449_G1_Msg_InfoUseCNhatBGia, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                            break;
                                        }
                                    case "Has-PriceList-Current":
                                        {
                                            MessageBox.Show(string.Format("{0} ", eHCMSResources.A0445_G1_Msg_InfoDaCoBGiaHHanhChoThang) + ObjDrugDeptSellingPriceList_Current.EffectiveDate.Month.ToString() + string.Format("! {0}", eHCMSResources.A0449_G1_Msg_InfoUseCNhatBGia), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
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
                                this.DlgHideBusyIndicator();
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    this.DlgHideBusyIndicator();
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                }
            });

            t.Start();
        }
        //AddNew


        public void DrugDeptSellingPriceList_Update()
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.Z0726_G1_DangCNhat) });
            this.DlgShowBusyIndicator(string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.Z0726_G1_DangCNhat));
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyDrugServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginDrugDeptSellingPriceList_Update(ObjDrugDeptSellingPriceList_Current, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                string Result = "";
                                contract.EndDrugDeptSellingPriceList_Update(out Result, asyncResult);
                                this.DlgHideBusyIndicator();
                                switch (Result)
                                {
                                    case "Update-0":
                                        {
                                            MessageBox.Show(string.Format("{0}!", eHCMSResources.A0272_G1_Msg_InfoCNhatFail), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                            break;
                                        }
                                    case "Update-1":
                                        {
                                            btChooseItemFromDelete_IsEnabled = false;
                                            btSave_IsEnabled = false;
                                            dtgListIsEnabled = false;

                                            Globals.EventAggregator.Publish(new SaveEvent<bool>() { Result = true });

                                            MessageBox.Show(string.Format("{0}!", eHCMSResources.K2782_G1_DaCNhat), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);

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

        private bool _dtgListIsEnabled;
        public bool dtgListIsEnabled
        {
            get { return _dtgListIsEnabled; }
            set
            {
                _dtgListIsEnabled = value;
                NotifyOfPropertyChange(() => dtgListIsEnabled);
            }
        }

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

        private string CheckEffectiveDate_PriceList_Update()
        {
            if (ObjDrugDeptSellingPriceList_Current.EffectiveDate.Subtract(EffectiveDate_tmp).Days <= 0)
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
            ObjDrugDeptSellingPriceList_AutoCreate_Save.Clear();

            foreach (DrugDeptSellingItemPrices item in ObjDrugDeptSellingPriceList_AutoCreate)
            {
                if (item.RowState == DrugDeptSellingItemPrices.RowStateValue.Update)
                {
                    ObjDrugDeptSellingPriceList_AutoCreate_Save.Add(item);
                }
            }
            ObjDrugDeptSellingPriceList_Current.ObjDrugDeptSellingItemPrices = ObjDrugDeptSellingPriceList_AutoCreate_Save;
        }

        private void ThuTucUpDate()
        {
            if (CheckValidPrice(ObjDrugDeptSellingPriceList_Current) == false)
            {
                return;
            }
            string message = "";
            if (CheckPriceBeforeSave(ref message))
            {
                if (message != ""
                    && MessageBox.Show(message + string.Format("\n{0}", eHCMSResources.T3805_G1_BanCoMuonTiepTucKhong)
                    , string.Format("{0}!", eHCMSResources.G0442_G1_TBao), MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                {
                    return;
                }
                string Loai = CheckEffectiveDate_PriceList_Update();

                switch (Loai)
                {
                    case "Edit-PriceList-Current":
                        {
                            DrugDeptSellingPriceList_Update();
                            break;
                        }
                    case "Edit-PriceList-Future":
                        {
                            if (ObjDrugDeptSellingPriceList_Current.EffectiveDate.Subtract(DateTime.Now).Days > 0)
                            {
                                DrugDeptSellingPriceList_Update();
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

        #region "btChooseItemFromDelete"
        public void btChooseItemFromDelete()
        {
            void onInitDlg(IDrugDeptSellingPriceList_ChooseFromDelete typeInfo)
            {
                typeInfo.ObjDrugDeptSellingItemPrices_All_Virtual_Delete = ObjDrugDeptSellingItemPrices_All_Virtual_Delete;
            }
            GlobalsNAV.ShowDialog<IDrugDeptSellingPriceList_ChooseFromDelete>(onInitDlg);
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
                ObjDrugDeptSellingPriceList_Current.EffectiveDate = DateTime.Now;
            }
        }
        #endregion

        public void Handle(SelectedObjectEvent<DrugDeptSellingItemPrices> message)
        {
            //if (message != null)
            //{
            //    DataEntities.DrugDeptSellingItemPrices Objtmp = message.Result;
            //    UnMarkDeleteRow_VirList(Objtmp);
            //    SetbtChooseItemFromDelete_IsEnabled();
            //}
        }

        private void SetbtChooseItemFromDelete_IsEnabled()
        {
            if (ObjDrugDeptSellingItemPrices_All_Virtual_Delete.Count > 0)
            {
                btChooseItemFromDelete_IsEnabled = true;
            }
            else
            {
                btChooseItemFromDelete_IsEnabled = false;
            }
        }

        public void btPrint()
        {
            if (string.IsNullOrEmpty(ObjDrugDeptSellingPriceList_Current.PriceListTitle))
            {
                MessageBox.Show(eHCMSResources.A0882_G1_Msg_InfoNhapTieuDeBGia);
                return;
            }

            if (ObjDrugDeptSellingPriceList_Current != null && ObjDrugDeptSellingPriceList_Current.DrugDeptSellingPriceListID > 0)/*Update*/
            {
                switch (ObjDrugDeptSellingPriceList_Current.V_MedProductType)
                {
                    case (long)AllLookupValues.MedProductType.THUOC:
                        {
                            void onInitDlg(ICommonPreviewView proAlloc)
                            {
                                proAlloc.TieuDeRpt = string.Format("{0} ", eHCMSResources.Z0695_G1_BGiaThuocTh) + ObjDrugDeptSellingPriceList_Current.RecCreatedDate.Month.ToString() + "/" + ObjDrugDeptSellingPriceList_Current.RecCreatedDate.Year.ToString();
                                proAlloc.DrugDeptSellingPriceListID = ObjDrugDeptSellingPriceList_Current.DrugDeptSellingPriceListID;
                                proAlloc.eItem = ReportName.RptBangGiaThuocKhoaDuoc;
                            }
                            GlobalsNAV.ShowDialog<ICommonPreviewView>(onInitDlg, null, false, true, Globals.GetDefaultDialogViewSize());
                            break;
                        }
                    case (long)AllLookupValues.MedProductType.Y_CU:
                        {
                            void onInitDlg(ICommonPreviewView proAlloc)
                            {
                                proAlloc.TieuDeRpt = string.Format("{0} ", eHCMSResources.K1021_G1_BGia) + Globals.GetTextV_MedProductType(ObjDrugDeptSellingPriceList_Current.V_MedProductType) + string.Format(" {0} ", eHCMSResources.G0039_G1_Th) + ObjDrugDeptSellingPriceList_Current.RecCreatedDate.Month.ToString() + "/" + ObjDrugDeptSellingPriceList_Current.RecCreatedDate.Year.ToString();
                                proAlloc.TenYCuHoaChat = eHCMSResources.Z0696_G1_TenYCu;
                                proAlloc.TenYCuHoaChatTiengViet = eHCMSResources.Z0697_G1_TenYCuTiengViet;
                                proAlloc.DrugDeptSellingPriceListID = ObjDrugDeptSellingPriceList_Current.DrugDeptSellingPriceListID;
                                proAlloc.eItem = ReportName.RptBangGiaYCuHoaChatKhoaDuoc;
                            }
                            GlobalsNAV.ShowDialog<ICommonPreviewView>(onInitDlg, null, false, true, Globals.GetDefaultDialogViewSize());
                            break;
                        }
                    case (long)AllLookupValues.MedProductType.HOA_CHAT:
                        {
                            void onInitDlg(ICommonPreviewView proAlloc)
                            {
                                proAlloc.TieuDeRpt = string.Format("{0} ", eHCMSResources.K1021_G1_BGia) + Globals.GetTextV_MedProductType(ObjDrugDeptSellingPriceList_Current.V_MedProductType) + string.Format(" {0} ", eHCMSResources.G0039_G1_Th) + ObjDrugDeptSellingPriceList_Current.RecCreatedDate.Month.ToString() + "/" + ObjDrugDeptSellingPriceList_Current.RecCreatedDate.Year.ToString();
                                proAlloc.TenYCuHoaChat = eHCMSResources.Z0698_G1_TenHChat;
                                proAlloc.TenYCuHoaChatTiengViet = eHCMSResources.Z0699_G1_TenHChatTiengViet;
                                proAlloc.DrugDeptSellingPriceListID = ObjDrugDeptSellingPriceList_Current.DrugDeptSellingPriceListID;
                                proAlloc.eItem = ReportName.RptBangGiaYCuHoaChatKhoaDuoc;
                            }
                            GlobalsNAV.ShowDialog<ICommonPreviewView>(onInitDlg, null, false, true, Globals.GetDefaultDialogViewSize());
                            break;
                        }
                }
            }
            else//Tạo mới
            {
                switch (ObjDrugDeptSellingPriceList_Current.V_MedProductType)
                {
                    case (long)AllLookupValues.MedProductType.THUOC:
                        {
                            void onInitDlg(ICommonPreviewView proAlloc)
                            {
                                proAlloc.TieuDeRpt = string.Format("{0} ", eHCMSResources.Z0700_G1_BGiaThuoc) + ObjDrugDeptSellingPriceList_Current.PriceListTitle.Trim();
                                proAlloc.V_MedProductType = ObjDrugDeptSellingPriceList_Current.V_MedProductType;
                                proAlloc.Result = "";
                                proAlloc.eItem = ReportName.RptBangGiaThuocKhoaDuoc_AutoCreate;
                            }
                            GlobalsNAV.ShowDialog<ICommonPreviewView>(onInitDlg, null, false, true, Globals.GetDefaultDialogViewSize());
                            break;
                        }
                    case (long)AllLookupValues.MedProductType.Y_CU:
                        {
                            void onInitDlg(ICommonPreviewView proAlloc)
                            {
                                proAlloc.TieuDeRpt = string.Format("{0} ", eHCMSResources.Z0701_G1_BGiaYCu) + ObjDrugDeptSellingPriceList_Current.PriceListTitle.Trim();
                                proAlloc.TenYCuHoaChat = eHCMSResources.Z0696_G1_TenYCu;
                                proAlloc.TenYCuHoaChatTiengViet = eHCMSResources.Z0697_G1_TenYCuTiengViet;
                                proAlloc.V_MedProductType = ObjDrugDeptSellingPriceList_Current.V_MedProductType;
                                proAlloc.Result = "";
                                proAlloc.eItem = ReportName.RptBangGiaYCuHoaChatKhoaDuoc_AutoCreate;
                            }
                            GlobalsNAV.ShowDialog<ICommonPreviewView>(onInitDlg, null, false, true, Globals.GetDefaultDialogViewSize());
                            break;
                        }
                    case (long)AllLookupValues.MedProductType.HOA_CHAT:
                        {
                            void onInitDlg(ICommonPreviewView proAlloc)
                            {
                                proAlloc.TieuDeRpt = string.Format("{0} ", eHCMSResources.Z0704_G1_BGiaHChat) + ObjDrugDeptSellingPriceList_Current.PriceListTitle.Trim();
                                proAlloc.TenYCuHoaChat = eHCMSResources.Z0698_G1_TenHChat;
                                proAlloc.TenYCuHoaChatTiengViet = eHCMSResources.Z0699_G1_TenHChatTiengViet;
                                proAlloc.V_MedProductType = ObjDrugDeptSellingPriceList_Current.V_MedProductType;
                                proAlloc.Result = "";
                                proAlloc.DrugDeptSellingPriceListID = ObjDrugDeptSellingPriceList_Current.DrugDeptSellingPriceListID;
                                proAlloc.eItem = ReportName.RptBangGiaYCuHoaChatKhoaDuoc_AutoCreate;
                            }
                            GlobalsNAV.ShowDialog<ICommonPreviewView>(onInitDlg, null, false, true, Globals.GetDefaultDialogViewSize());
                            break;
                        }
                }
            }
        }

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
            //ChangedWPF-CMN
            //pagerSellingList.Source = ObjDrugDeptSellingPriceList_AutoCreate_Paging;
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
            if (ObjDrugDeptSellingPriceList_AutoCreate == null)
            {
                ObjDrugDeptSellingPriceList_AutoCreate = new ObservableCollection<DrugDeptSellingItemPrices>();
            }
            if (CVS_ObjDrugDeptSellingPriceList_AutoCreate_Paging == null)
            {
                CVS_ObjDrugDeptSellingPriceList_AutoCreate_Paging = new CollectionViewSource { Source = ObjDrugDeptSellingPriceList_AutoCreate };
                CV_ObjDrugDeptSellingPriceList_AutoCreate_Paging = (CollectionView)CVS_ObjDrugDeptSellingPriceList_AutoCreate_Paging.View;
                NotifyOfPropertyChange(() => CV_ObjDrugDeptSellingPriceList_AutoCreate_Paging);
            }
            else
            {
                CV_ObjDrugDeptSellingPriceList_AutoCreate_Paging.Refresh();
            }
            btnFilter();
        }

        public void btnFilter()
        {
            CV_ObjDrugDeptSellingPriceList_AutoCreate_Paging.Filter = null;
            CV_ObjDrugDeptSellingPriceList_AutoCreate_Paging.Filter = new Predicate<object>(DoFilter);
        }

        //Callback method
        private bool DoFilter(object o)
        {
            string Code = Globals.FormatCode(V_MedProductType, SearchKey);

            //it is not a case sensitive search
            DrugDeptSellingItemPrices emp = o as DrugDeptSellingItemPrices;
            if (emp != null)
            {
                if (string.IsNullOrEmpty(SearchKey))
                {
                    SearchKey = "";
                }
                //if (emp.BrandName.ToLower().IndexOf(SearchKey.Trim().ToLower()) >= 0)
                //KMx: Lọc theo Code hoặc Tên (22/06/2014 11:15)
                if (emp.BrandName.ToLower().IndexOf(SearchKey.Trim().ToLower()) >= 0 || emp.Code.ToLower() == Code.Trim().ToLower())
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
            if (PagingChecked != null && ObjDrugDeptSellingPriceList_AutoCreate != null)
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
    }
}
