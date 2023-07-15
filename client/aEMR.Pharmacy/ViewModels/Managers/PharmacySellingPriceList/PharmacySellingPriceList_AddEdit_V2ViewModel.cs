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
using eHCMSLanguage;
//using aEMR.Common.PagedCollectionView;
using System.Windows.Data;
using aEMR.Common;
using aEMR.Infrastructure.CachingUtils;
using Castle.Core.Logging;
using Castle.Windsor;
using aEMR.Common.BaseModel;
/*
 * 20181019 TNHX #001: [BM0003195] Refactor code
 */
namespace aEMR.Pharmacy.ViewModels
{
    [Export(typeof(IPharmacySellingPriceList_AddEdit_V2)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class PharmacySellingPriceList_AddEdit_V2ViewModel : ViewModelBase, IPharmacySellingPriceList_AddEdit_V2
    , IHandle<SelectedObjectEvent<PharmacySellingItemPrices>>
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;
        [ImportingConstructor]
        public PharmacySellingPriceList_AddEdit_V2ViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;
            eventArg.Subscribe(this);
            if (Globals.ServerConfigSection.PharmacyElements.CalForPriceProfitScale)
            {
                VisiUseSuggestPrice = Visibility.Visible;
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

        private PharmacySellingPriceList _ObjPharmacySellingPriceList_Current;
        public PharmacySellingPriceList ObjPharmacySellingPriceList_Current
        {
            get { return _ObjPharmacySellingPriceList_Current; }
            set
            {
                _ObjPharmacySellingPriceList_Current = value;
                NotifyOfPropertyChange(() => ObjPharmacySellingPriceList_Current);
            }
        }

        private DateTime EffectiveDate_tmp;
        protected override void OnActivate()
        {
            Globals.EventAggregator.Subscribe(this);

            base.OnActivate();

            ObjPharmacySellingPriceList_AutoCreate = new ObservableCollection<PharmacySellingItemPrices>();

            ObjPharmacySellingPriceList_AutoCreate_Copy = new ObservableCollection<PharmacySellingItemPrices>();

            ObjPharmacySellingPriceList_AutoCreate_Save = new ObservableCollection<PharmacySellingItemPrices>();

            ObjPharmacySellingItemPrices_All_Virtual_Delete = new ObservableCollection<PharmacySellingItemPrices>();

            if (ObjPharmacySellingPriceList_Current != null && ObjPharmacySellingPriceList_Current.PharmacySellingPriceListID > 0)/*Update*/
            {
                //ObjPharmacySellingPriceList_Current.EffectiveDate = BeginDate;
                PharmacySellingPriceList_Detail(ObjPharmacySellingPriceList_Current.PharmacySellingPriceListID);
                EffectiveDate_tmp = ObjPharmacySellingPriceList_Current.EffectiveDate;
                //dpEffectiveDate_IsEnabled = true;
                btChooseItemFromDelete_IsEnabled = false;

                //if (ObjPharmacySellingPriceList_Current.PriceListType == "PriceList-Old")
                if (!ObjPharmacySellingPriceList_Current.IsActive
                    && ObjPharmacySellingPriceList_Current.EffectiveDate <= BeginDate)
                {
                    MessageBox.Show(eHCMSResources.A0205_G1_Msg_InfoKhDcSuaBGiaCu, eHCMSResources.Z0665_G1_SuaBGia, MessageBoxButton.OK);
                    btSave_IsEnabled = false;
                }
                else
                {
                    btSave_IsEnabled = true;
                }
            }
            else/*AddNew*/
            {
                InitializeNewItem();

                PharmacySellingPriceList_AutoCreate();
                dpEffectiveDate_IsEnabled = false;
                btChooseItemFromDelete_IsEnabled = false;
                btSave_IsEnabled = true;
            }
        }

        //void ObjPharmacySellingPriceList_AutoCreate_Paging_OnRefresh(object sender, RefreshEventArgs e)
        //{
        //    PagingLinq(ObjPharmacySellingPriceList_AutoCreate_Paging.PageIndex, ObjPharmacySellingPriceList_AutoCreate_Paging.PageSize);
        //}


        //private void PagingLinq(int pIndex, int pPageSize)
        //{
        //    var ResultAll = from p in ObjPharmacySellingPriceList_AutoCreate.ToObservableCollection()
        //                    select p;
        //    List<PharmacySellingItemPrices> Items = ResultAll.Skip(pIndex * pPageSize).Take(pPageSize).ToList();
        //    ShowItemsOnList(Items);
        //}
        //private void ShowItemsOnList(List<PharmacySellingItemPrices> ObjCollect)
        //{
        //    ObjPharmacySellingPriceList_AutoCreate_Paging.Clear();
        //    foreach (PharmacySellingItemPrices item in ObjCollect)
        //    {
        //        if (item.RowState != PharmacySellingItemPrices.RowStateValue.Delete)
        //        {
        //            ObjPharmacySellingPriceList_AutoCreate_Paging.Add(item);
        //        }
        //    }
        //}

        private CollectionViewSource CVS_ObjPharmacySellingPriceList_AutoCreate_Paging = null;
        public CollectionView CV_ObjPharmacySellingPriceList_AutoCreate_Paging
        {
            get; set;
        }

        private ObservableCollection<PharmacySellingItemPrices> _ObjPharmacySellingPriceList_AutoCreate_Save;
        public ObservableCollection<PharmacySellingItemPrices> ObjPharmacySellingPriceList_AutoCreate_Save
        {
            get { return _ObjPharmacySellingPriceList_AutoCreate_Save; }
            set
            {
                _ObjPharmacySellingPriceList_AutoCreate_Save = value;
                NotifyOfPropertyChange(() => ObjPharmacySellingPriceList_AutoCreate_Save);
            }
        }

        private ObservableCollection<PharmacySellingItemPrices> _ObjPharmacySellingPriceList_AutoCreate;
        public ObservableCollection<PharmacySellingItemPrices> ObjPharmacySellingPriceList_AutoCreate
        {
            get { return _ObjPharmacySellingPriceList_AutoCreate; }
            set
            {
                _ObjPharmacySellingPriceList_AutoCreate = value;
                NotifyOfPropertyChange(() => ObjPharmacySellingPriceList_AutoCreate);
            }
        }

        private ObservableCollection<PharmacySellingItemPrices> _ObjPharmacySellingPriceList_AutoCreate_Copy;
        public ObservableCollection<PharmacySellingItemPrices> ObjPharmacySellingPriceList_AutoCreate_Copy
        {
            get { return _ObjPharmacySellingPriceList_AutoCreate_Copy; }
            set
            {
                _ObjPharmacySellingPriceList_AutoCreate_Copy = value;
                NotifyOfPropertyChange(() => ObjPharmacySellingPriceList_AutoCreate_Copy);
            }
        }
        private void PharmacySellingPriceList_AutoCreate()
        {
            ObjPharmacySellingPriceList_AutoCreate.Clear();
            this.DlgShowBusyIndicator(eHCMSResources.Z0667_G1_DSGiaBan);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyDrugServiceClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginPharmacySellingPriceList_AutoCreate_V2(Globals.DispatchCallback((asyncResult) =>
                        {
                            List<PharmacySellingItemPrices> allItems = null;
                            try
                            {
                                allItems = client.EndPharmacySellingPriceList_AutoCreate_V2(out string Result, asyncResult);

                                switch (Result)
                                {
                                    case "OK":
                                        {
                                            btSave_IsEnabled = true;
                                            dtgListIsEnabled = true;

                                            ObjPharmacySellingPriceList_AutoCreate = new ObservableCollection<PharmacySellingItemPrices>(allItems);
                                            ObjPharmacySellingPriceList_AutoCreate_Copy = ObjPharmacySellingPriceList_AutoCreate.DeepCopy();
                                            if (Globals.ServerConfigSection.PharmacyElements.CalForPriceProfitScale)
                                            {
                                                IsUseSuggestPrice = true;
                                            }
                                            else
                                            {
                                                LoadDataGrid();
                                            }
                                            //ObjPharmacySellingPriceList_AutoCreate_Paging.TotalItemCount = allItems.Count;
                                            //PagingLinq(0, ObjPharmacySellingPriceList_AutoCreate_Paging.PageSize);
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
                                            MessageBox.Show(eHCMSResources.A0434_G1_Msg_InfoLoiKhiTaoBGia, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
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
                    ClientLoggerHelper.LogInfo(ex.ToString());
                }
            });
            t.Start();
        }

        private void PharmacySellingPriceList_Detail(Int64 PharmacySellingPriceListID)
        {
            VisiUseSuggestPrice = Visibility.Collapsed;
            ObjPharmacySellingPriceList_AutoCreate.Clear();
            this.DlgShowBusyIndicator(eHCMSResources.Z0667_G1_DSGiaBan);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyDrugServiceClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginPharmacySellingPriceList_Detail_V2(PharmacySellingPriceListID, Globals.DispatchCallback((asyncResult) =>
                        {
                            List<PharmacySellingItemPrices> allItems = null;
                            try
                            {
                                allItems = client.EndPharmacySellingPriceList_Detail_V2(asyncResult);
                                if (allItems != null)
                                {
                                    dtgListIsEnabled = true;

                                    ObjPharmacySellingPriceList_AutoCreate = new ObservableCollection<PharmacySellingItemPrices>(allItems);
                                    LoadDataGrid();
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
                    ClientLoggerHelper.LogInfo(ex.ToString());
                }
            });
            t.Start();
        }

        private ObservableCollection<PharmacySellingItemPrices> _ObjPharmacySellingItemPrices_All_Virtual_Delete;
        public ObservableCollection<PharmacySellingItemPrices> ObjPharmacySellingItemPrices_All_Virtual_Delete
        {
            get { return _ObjPharmacySellingItemPrices_All_Virtual_Delete; }
            set
            {
                if (_ObjPharmacySellingItemPrices_All_Virtual_Delete != value)
                {
                    _ObjPharmacySellingItemPrices_All_Virtual_Delete = value;
                    NotifyOfPropertyChange(() => ObjPharmacySellingItemPrices_All_Virtual_Delete);
                }
            }
        }

        public void hplDelete_Click(object selectItem)//Chỉ có trong TH Tạo Mới Bảng Giá
        {
            PharmacySellingItemPrices p = selectItem as PharmacySellingItemPrices;
            if (ObjPharmacySellingPriceList_Current.PharmacySellingPriceListID <= 0) //TH Tạo Bảng Giá
            {
                MarkDeleteRow_VirList(p);
                ObjPharmacySellingPriceList_AutoCreate.Remove(p);
                btChooseItemFromDelete_IsEnabled = true;
            }
        }

        private void MarkDeleteRow_VirList(PharmacySellingItemPrices p)
        {
            foreach (PharmacySellingItemPrices item in ObjPharmacySellingPriceList_AutoCreate)
            {
                if (item.DrugID == p.DrugID)
                {
                    p.RowState = PharmacySellingItemPrices.RowStateValue.Delete;

                    //Add vào các mục đã xóa để cho chọn lại
                    AddMarkDeleteRow_VirList(p);
                    //Add vào các mục đã xóa để cho chọn lại

                    break;
                }
            }
        }

        private void AddMarkDeleteRow_VirList(PharmacySellingItemPrices p)
        {
            ObjPharmacySellingItemPrices_All_Virtual_Delete.Add(p);
        }

        private void UnMarkDeleteRow_VirList(PharmacySellingItemPrices p)
        {
            foreach (PharmacySellingItemPrices item in ObjPharmacySellingPriceList_AutoCreate)
            {
                if (item.DrugID == p.DrugID)
                {
                    //Check lại 
                    CheckValueChangeAfterUnMarkDelete(p);
                    ObjPharmacySellingItemPrices_All_Virtual_Delete.Remove(p);
                    break;
                }
            }

            LoadDataGrid();
            // PagingLinq(ObjPharmacySellingPriceList_AutoCreate_Paging.PageIndex, ObjPharmacySellingPriceList_AutoCreate_Paging.PageSize);
        }

        private void CheckValueChangeAfterUnMarkDelete(PharmacySellingItemPrices p)
        {
            decimal NormalPrice = p.NormalPrice;
            decimal NormalPrice_Old = p.NormalPrice_Old;

            decimal? PriceForHIPatient = p.PriceForHIPatient;
            decimal? PriceForHIPatient_Old = p.PriceForHIPatient_Old;
            decimal? HIAllowedPrice = p.HIAllowedPrice;
            decimal? HIAllowedPrice_Old = p.HIAllowedPrice_Old;

            //Đánh dấu trạng thái dòng--> so 3 cột

            if (NormalPrice == NormalPrice_Old && PriceForHIPatient == PriceForHIPatient_Old && HIAllowedPrice == HIAllowedPrice_Old)
            {
                p.RowState = PharmacySellingItemPrices.RowStateValue.NoChange;
            }
            else
            {
                p.RowState = PharmacySellingItemPrices.RowStateValue.Update;
            }
        }

        public void InitializeNewItem()
        {
            ObjPharmacySellingPriceList_Current = new PharmacySellingPriceList
            {
                PharmacySellingPriceListID = 0,
                EffectiveDate = BeginDate,
                StaffID = Globals.LoggedUserAccount.Staff.StaffID
            };
            ObjPharmacySellingPriceList_Current.ApprovedStaffID = ObjPharmacySellingPriceList_Current.StaffID;
            ObjPharmacySellingPriceList_Current.ObjPharmacySellingItemPrices = new ObservableCollection<PharmacySellingItemPrices>();
        }

        //#region Color Row and Cell
        public void dtgList_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = e.Row.GetIndex() + 1;
            if (e.Row.DataContext is PharmacySellingItemPrices objRows)
            {
                if (Math.Abs(objRows.ContractPriceAfterVAT - objRows.PriceForHIPatient) > 1)
                {
                    e.Row.Background = new SolidColorBrush(Colors.Yellow);
                }
                else if (Math.Abs(objRows.RefHIAllowedPrice - objRows.HIAllowedPrice) > 1)
                {
                    e.Row.Background = new SolidColorBrush(Colors.Purple);
                }
                else if (Math.Abs(objRows.ContractPriceAfterVAT - objRows.InCost) > 1)
                {
                    e.Row.Background = new SolidColorBrush(Colors.Cyan);
                }
                else
                {
                    e.Row.Background = new SolidColorBrush(Colors.White);
                }

                //if (objRows.SuggestPrice > objRows.NormalPrice)
                //    e.Row.Background = new SolidColorBrush(Colors.Red);
                //else
                //    if (Math.Abs(objRows.SuggestPrice - objRows.NormalPrice) > 1)
                //        e.Row.Background = new SolidColorBrush(Colors.Yellow);
                //    else
                //        e.Row.Background = new SolidColorBrush(Colors.White);

                //KMx: Chị Trúc yêu cầu nếu Giá vốn lần này khác giá vốn lần trước thì đổi background Giá vốn lần này (16/02/2014).
                //Colums[5] là "Giá vốn lần này".
                //TextBox txtInCost = ((sender as DataGrid)).Columns[5].GetCellContent(e.Row).FindName("tbInCost") as TextBox;
                //Lấy TextBox theo cách dynamic bên dưới hay hơn là lấy TextBox fixed cột như ở trên.
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
                //switch (objRows.PriceType)
                //{
                //    case "PriceCurrent":
                //        {
                //            e.Row.Foreground = new SolidColorBrush(Colors.Green);
                //            break;
                //        }
                //    case "PriceFuture-Active-1":
                //        {
                //            e.Row.Foreground = new SolidColorBrush(Colors.Gray);
                //            break;
                //        }
                //    case "PriceFuture-Active-0":
                //        {
                //            e.Row.Foreground = new SolidColorBrush(Colors.Blue);
                //            break;
                //        }
                //    default:
                //        {
                //            e.Row.Foreground = new SolidColorBrush(Colors.Black);
                //            break;
                //        }
                //}
            }

            //CheckCellValueChange_ChangeColor(sender, e);
        }

        //private void CheckCellValueChange_ChangeColor(object sender, DataGridRowEventArgs e)
        //{
        //    PharmacySellingItemPrices p = e.Row.DataContext as PharmacySellingItemPrices;

        //    for (int i = 3; i <= 5; i++)
        //    {
        //        TextBlock textBlock = (sender as DataGrid).Columns[i].GetCellContent(e.Row) as TextBlock;

        //        decimal NormalPrice = p.NormalPrice;
        //        decimal NormalPrice_Old = p.NormalPrice_Old;

        //        Nullable<decimal> PriceForHIPatient = p.PriceForHIPatient;
        //        Nullable<decimal> PriceForHIPatient_Old = p.PriceForHIPatient_Old;

        //        Nullable<decimal> HIAllowedPrice = p.HIAllowedPrice;
        //        Nullable<decimal> HIAllowedPrice_Old = p.HIAllowedPrice_Old;

        //        switch (i)
        //        {
        //            case 3:
        //                {
        //                    if (NormalPrice == NormalPrice_Old)
        //                    {
        //                        ForeColorByPriceType(p, textBlock);
        //                    }
        //                    else
        //                    {
        //                        textBlock.Foreground = new SolidColorBrush(Colors.Brown);
        //                    }
        //                    break;
        //                }
        //            case 4:
        //                {
        //                    if (PriceForHIPatient == PriceForHIPatient_Old)
        //                    {
        //                        ForeColorByPriceType(p, textBlock);
        //                    }
        //                    else
        //                    {
        //                        textBlock.Foreground = new SolidColorBrush(Colors.Brown);
        //                    }
        //                    break;
        //                }
        //            case 5:
        //                {
        //                    if (HIAllowedPrice == HIAllowedPrice_Old)
        //                    {
        //                        ForeColorByPriceType(p, textBlock);
        //                    }
        //                    else
        //                    {
        //                        textBlock.Foreground = new SolidColorBrush(Colors.Brown);
        //                    }
        //                    break;
        //                }
        //        }
        //    }
        //}
        //private FrameworkElement GetParent(FrameworkElement child, Type targetType)
        //{
        //    object parent = child.Parent;
        //    if (parent != null)
        //    {
        //        if (parent.GetType() == targetType)
        //        {
        //            return (FrameworkElement)parent;
        //        }
        //        else
        //        {
        //            return GetParent((FrameworkElement)parent, targetType);
        //        }
        //    }
        //    return null;
        //}

        private void ForeColorByPriceType(PharmacySellingItemPrices objRows, TextBlock textBlock)
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

            int IndexCol = e.Column.DisplayIndex;
            string StringNameControl = GetStringNameControl(IndexCol);
            if (((sender as DataGrid)).CurrentColumn.GetCellContent(e.Row).FindName(StringNameControl) is TextBox Ctrtb)
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
                case 5:
                    {
                        Result = "tbNormalPrice";
                        break;
                    }
                case 7:
                    {
                        Result = "tbPriceForHIPatient";
                        break;
                    }
                case 8:
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

                AxTextBox textBlock = (column.GetCellContent(row) as AxTextBox);

                var mCellCollection = (e.Row as DataGridRow).GetChildrenByType<DataGridCell>();
                if (mCellCollection != null && mCellCollection.Count > IndexColumn)
                {
                    var mCell = mCellCollection[IndexColumn];
                    if (mCell.Content != null && mCell.Content is ContentPresenter)
                    {
                        var mCellTemplate = (mCell.Content as ContentPresenter).ContentTemplate.LoadContent();
                        if (mCellTemplate is AxTextBox)
                            textBlock = mCellTemplate as AxTextBox;
                    }
                }

                if (textBlock != null)
                {
                    if (CheckCellValueChange_UpdateRowStatus(sender, IndexColumn))
                    {
                        //textBlock.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(250, 120, 150, 111));
                        textBlock.Foreground = new SolidColorBrush(Colors.Brown);
                    }
                    else
                    {
                        textBlock.Foreground = new SolidColorBrush(Colors.Black);

                        //PharmacySellingItemPrices objRows = ((PharmacySellingItemPrices)(((System.Windows.Controls.DataGrid)(sender)).SelectedItem));

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
                    row = null;
                    column = null;
                }
            }
        }

        private bool CheckCellValueChange_UpdateRowStatus(object sender, int IndexColumn)
        {
            bool Result = true;

            decimal NormalPrice = ((PharmacySellingItemPrices)(((DataGrid)(sender)).SelectedItem)).NormalPrice;
            decimal NormalPrice_Old = ((PharmacySellingItemPrices)(((DataGrid)(sender)).SelectedItem)).NormalPrice_Old;

            decimal? PriceForHIPatient = ((PharmacySellingItemPrices)(((DataGrid)(sender)).SelectedItem)).PriceForHIPatient;
            decimal? PriceForHIPatient_Old = ((PharmacySellingItemPrices)(((DataGrid)(sender)).SelectedItem)).PriceForHIPatient_Old;

            decimal? HIAllowedPrice = ((PharmacySellingItemPrices)(((DataGrid)(sender)).SelectedItem)).HIAllowedPrice;
            decimal? HIAllowedPrice_Old = ((PharmacySellingItemPrices)(((DataGrid)(sender)).SelectedItem)).HIAllowedPrice_Old;

            switch (IndexColumn)
            {
                case 5:
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
                case 7:
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
                case 8:
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
            if (ObjPharmacySellingPriceList_Current.PharmacySellingPriceListID > 0)//Update 
            {
                if (NormalPrice == NormalPrice_Old && PriceForHIPatient == PriceForHIPatient_Old &&
                    HIAllowedPrice == HIAllowedPrice_Old)
                {
                    ((PharmacySellingItemPrices)(((DataGrid)(sender)).SelectedItem)).RowState = PharmacySellingItemPrices.RowStateValue.NoChange;
                }
                else
                {
                    ((PharmacySellingItemPrices)(((DataGrid)(sender)).SelectedItem)).RowState = PharmacySellingItemPrices.RowStateValue.Update;
                }
            }

            return Result;
        }

        //#endregion

        public void dtgList_Loaded(object sender, RoutedEventArgs e)
        {
            //DataGrid dtgList = sender as DataGrid;
            //if (ObjPharmacySellingPriceList_Current.PharmacySellingPriceListID > 0)
            //{
            //    dtgList.Columns[0].Visibility = Visibility.Collapsed;
            //}
        }

        public void btSave()
        {
            if (String.IsNullOrEmpty(ObjPharmacySellingPriceList_Current.PriceListTitle))
            {
                MessageBox.Show(eHCMSResources.A0882_G1_Msg_InfoNhapTieuDeBGia);
                return;
            }

            //if (CheckValidList(ObjPharmacySellingPriceList_Current)==false)
            //{
            //    return;
            //}

            if (ObjPharmacySellingPriceList_Current.PharmacySellingPriceListID == 0)
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
            ObjPharmacySellingPriceList_AutoCreate_Save.Clear();

            foreach (PharmacySellingItemPrices item in ObjPharmacySellingPriceList_AutoCreate)
            {
                if (item.RowState != PharmacySellingItemPrices.RowStateValue.Delete)
                {
                    ObjPharmacySellingPriceList_AutoCreate_Save.Add(item);
                }
            }

            ObjPharmacySellingPriceList_Current.ObjPharmacySellingItemPrices = ObjPharmacySellingPriceList_AutoCreate_Save;
        }

        private void ThuTucAddNew()
        {
            if (CheckValidPrice(ObjPharmacySellingPriceList_Current) == false)
            {
                return;
            }

            if (ObjPharmacySellingPriceList_AutoCreate_Save.Count > 0)
            {
                string message = "";
                //KMX
                //if (CheckPriceBeforeSave(ref message))
                //{
                if (message != ""
                && MessageBox.Show(message + string.Format("\n{0}", eHCMSResources.T3805_G1_BanCoMuonTiepTucKhong), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                {
                    return;
                }
                //PharmacySellingItemPrices_EffectiveDateMax();
                PharmacySellingPriceList_AddNew();
                //}
            }
            else
            {
                MessageBox.Show(eHCMSResources.A0204_G1_Msg_BGiaChuaCoMucNao, eHCMSResources.T2937_G1_Luu, MessageBoxButton.OK);
            }
        }

        private bool CheckPriceBeforeSave(ref string message)
        {
            foreach (PharmacySellingItemPrices item in ObjPharmacySellingPriceList_AutoCreate_Save)
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
                            MessageBox.Show(item.BrandName + string.Format(": {0}", eHCMSResources.Z0593_G1_GiaBHChoPhep));
                            return false;
                        }
                    }
                    if (item.InsuranceCover.GetValueOrDefault())
                    {
                        if (HIAllowedPrice.GetValueOrDefault(0) <= 0)
                        {
                            MessageBox.Show(item.BrandName + string.Format(": {0}", eHCMSResources.T1980_G1_GiaBHKhongDuocNhoHon0));
                            return false;
                        }
                    }
                }
                else
                {
                    //MessageBox.Show("Đơn Giá Phải >= 1!");
                    message = eHCMSResources.Z0691_G1_DGiaNhoHonBang0;
                    //return false;
                }
            }

            return true;
        }

        public bool CheckValidList(PharmacySellingPriceList p)
        {
            if (p == null)
            {
                return false;
            }
            return p.Validate();
        }

        public bool CheckValidPrice(PharmacySellingPriceList p)
        {
            if (p.ObjPharmacySellingItemPrices == null)
            {
                MessageBox.Show(eHCMSResources.Z0692_G1_BGiaChuaCoDLieu, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return false;
            }

            double dem = 0;
            //double TrangError = 0;
            foreach (PharmacySellingItemPrices item in ObjPharmacySellingPriceList_AutoCreate)
            {
                dem++;
                if (CheckBiXoa(item) == false)/*Không tính mấy cái xóa*/
                {
                    //if (CheckChuaCoPhieuNhapHang(item))
                    //{
                    //    TrangError = (dem % ObjPharmacySellingPriceList_AutoCreate_Paging.PageSize == 0) ? ((dem / ObjPharmacySellingPriceList_AutoCreate_Paging.PageSize) == 0 ? 1 : dem / ObjPharmacySellingPriceList_AutoCreate_Paging.PageSize) : Math.Ceiling(dem / ObjPharmacySellingPriceList_AutoCreate_Paging.PageSize);
                    //    MessageBox.Show("'" + item.BrandName.Trim() + "' tại trang: '" + TrangError.ToString() + "' " + Environment.NewLine + "Chưa Có Đợt Nhập Hàng Nào -> Không Hợp Lệ! Kiểm Tra Lại!", eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                    //    return false;
                    //}

                    //KMx
                    //if (item.Validate() == false)
                    //{
                    //    TrangError = (dem % ObjPharmacySellingPriceList_AutoCreate_Paging.PageSize == 0) ? ((dem / ObjPharmacySellingPriceList_AutoCreate_Paging.PageSize) == 0 ? 1 : dem / ObjPharmacySellingPriceList_AutoCreate_Paging.PageSize) : Math.Ceiling(dem / ObjPharmacySellingPriceList_AutoCreate_Paging.PageSize);
                    //    MessageBox.Show(string.Format(eHCMSResources.Z1310_G1_I, item.BrandName.Trim(), TrangError.ToString(), eHCMSResources.A0070_G1_Msg_InfoGiaLonHon0), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                    //    return false;
                    //}
                }
            }
            return true;
        }

        private bool CheckBiXoa(PharmacySellingItemPrices p)
        {
            foreach (PharmacySellingItemPrices item in ObjPharmacySellingItemPrices_All_Virtual_Delete)
            {
                if (item.DrugID == p.DrugID)
                {
                    return true;
                }
            }
            return false;
        }

        private bool CheckChuaCoPhieuNhapHang(PharmacySellingItemPrices p)
        {
            if (p.inviID <= 0)
                return true;
            return false;
        }

        private void PharmacySellingItemPrices_EffectiveDateMax()
        {
            this.DlgShowBusyIndicator(eHCMSResources.Z0693_G1_KTraNgApDung);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyDrugServiceClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginPharmacySellingItemPrices_EffectiveDateMax(Globals.DispatchCallback((asyncResult) =>
                        {
                            DateTime? dt = new DateTime();
                            try
                            {
                                dt = client.EndPharmacySellingItemPrices_EffectiveDateMax(asyncResult);
                                if (dt == null)
                                {
                                    PharmacySellingPriceList_AddNew();
                                }
                                else
                                {
                                    DateTime dtMax = dt.Value;
                                    if (ObjPharmacySellingPriceList_Current.EffectiveDate.Subtract(dtMax).Days < 0)
                                    {
                                        MessageBox.Show(string.Format(eHCMSResources.Z0694_G1_I, dtMax.ToString("dd/MM/yyyy")), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                    }
                                    else
                                    {
                                        PharmacySellingPriceList_AddNew();
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                ClientLoggerHelper.LogInfo(ex.ToString());
                                PharmacySellingPriceList_AddNew();
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
                    ClientLoggerHelper.LogInfo(ex.ToString());
                }
            });
            t.Start();
        }

        public void PharmacySellingPriceList_AddNew()
        {
            this.DlgShowBusyIndicator(string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.Z0645_G1_DangGhi));
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyDrugServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginPharmacySellingPriceList_AddNew(ObjPharmacySellingPriceList_Current, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                contract.EndPharmacySellingPriceList_AddNew(out string Result, asyncResult);
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
                                            MessageBox.Show(string.Format("{0} {1}! {2}", eHCMSResources.A0445_G1_Msg_InfoDaCoBGiaHHanhChoThang, ObjPharmacySellingPriceList_Current.EffectiveDate.Month.ToString(), eHCMSResources.A0449_G1_Msg_InfoUseCNhatBGia), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
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
                                _logger.Info(ex.Message);
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
                    _logger.Info(ex.Message);
                    this.DlgHideBusyIndicator();
                }
            });
            t.Start();
        }
        //AddNew

        public void PharmacySellingPriceList_Update()
        {
            this.DlgShowBusyIndicator(string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.Z0726_G1_DangCNhat));
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyDrugServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginPharmacySellingPriceList_Update(ObjPharmacySellingPriceList_Current, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                contract.EndPharmacySellingPriceList_Update(out string Result, asyncResult);
                                switch (Result)
                                {
                                    case "Update-0":
                                        {
                                            MessageBox.Show(eHCMSResources.Z1482_G1_CNhatBGiaKgThCong, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                            break;
                                        }
                                    case "Update-1":
                                        {
                                            btChooseItemFromDelete_IsEnabled = false;
                                            btSave_IsEnabled = false;
                                            dtgListIsEnabled = false;

                                            Globals.EventAggregator.Publish(new SaveEvent<bool>() { Result = true });

                                            MessageBox.Show(eHCMSResources.K2782_G1_DaCNhat, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);

                                            break;
                                        }
                                }
                            }
                            catch (Exception ex)
                            {
                                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                                _logger.Info(ex.Message);
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
                    _logger.Info(ex.Message);
                }
            });
            t.Start();
        }

        //bt AddNew
        //private bool _btSave_IsEnabled;
        //public bool btSave_IsEnabled
        //{
        //    get { return _btSave_IsEnabled; }
        //    set
        //    {
        //        _btSave_IsEnabled = value;
        //        NotifyOfPropertyChange(() => btSave_IsEnabled);
        //    }
        //}
        //bt AddNew


        //#region "Update"
        //private void PharmacySellingItemPricesPriceList_Detail(int PageIndex, int PageSize, bool CountTotal)
        //{
        //    Globals.EventAggregator.Publish(new BusyEvent() { IsBusy = true, Message = "Danh Sách Dịch Vụ..." });

        //    var t = new Thread(() =>
        //    {
        //        try
        //        {
        //            using (var serviceFactory = new ConfigurationManagerServiceClient())
        //            {
        //                var client = serviceFactory.ServiceInstance;
        //                client.BeginPharmacySellingItemPricesPriceList_Detail(SearchCriteria, PageIndex, PageSize, "", CountTotal, Globals.DispatchCallback((asyncResult) =>
        //                {
        //                    int Total = 0;
        //                    IList<PharmacySellingItemPrices> allItems = null;
        //                    bool bOK = false;
        //                    try
        //                    {
        //                        allItems = client.EndPharmacySellingItemPricesPriceList_Detail(out Total, asyncResult);
        //                        bOK = true;
        //                    }
        //                    catch (FaultException<AxException> fault)
        //                    {

        //                    }
        //                    catch (Exception ex)
        //                    { }

        //                    ObjPharmacySellingItemPricessAndPriceIsActive_ByPCLGroupID_Paging.Clear();

        //                    if (bOK)
        //                    {
        //                        if (CountTotal)
        //                        {
        //                            ObjPharmacySellingItemPricessAndPriceIsActive_ByPCLGroupID_Paging.TotalItemCount = Total;
        //                        }
        //                        if (allItems != null)
        //                        {
        //                            foreach (var item in allItems)
        //                            {
        //                                //Update trạng thái khi Update Ban Đầu là NoChange
        //                                SetStatusRowForUpdate(item);
        //                                //Update trạng thái khi Update Ban Đầu là NoChange

        //                                ObjPharmacySellingItemPricessAndPriceIsActive_ByPCLGroupID_Paging.Add(item);
        //                            }

        //                            AjustItemsToVirList(PageIndex);
        //                        }
        //                    }
        //                }), null)
        //                    ;
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //        }
        //        finally
        //        {
        //            Globals.IsBusy = false;
        //        }
        //    });
        //    t.Start();
        //}

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

        private Visibility _VisiUseSuggestPrice = Visibility.Collapsed;
        public Visibility VisiUseSuggestPrice
        {
            get { return _VisiUseSuggestPrice; }
            set
            {
                _VisiUseSuggestPrice = value;
                NotifyOfPropertyChange(() => VisiUseSuggestPrice);
            }
        }

        private bool _IsUseSuggestPrice;
        public bool IsUseSuggestPrice
        {
            get { return _IsUseSuggestPrice; }
            set
            {
                _IsUseSuggestPrice = value;
                NotifyOfPropertyChange(() => IsUseSuggestPrice);
            }
        }

        public void chkUseSuggestPrice_Check(object sender, RoutedEventArgs e)
        {
            if (IsUseSuggestPrice && ObjPharmacySellingPriceList_AutoCreate != null && ObjPharmacySellingPriceList_AutoCreate.Count > 0)
            {
                foreach(var item in ObjPharmacySellingPriceList_AutoCreate)
                {
                    if (item.PharmacySellingItemPriceID == 0)
                    {
                        item.NormalPrice = item.SuggestPrice;
                        item.PriceForHIPatient = item.SuggestPriceForHI;
                    }
                }
                LoadDataGrid();
            }
        }

        public void chkUseSuggestPrice_UnCheck(object sender, RoutedEventArgs e)
        {
            if (!IsUseSuggestPrice && ObjPharmacySellingPriceList_AutoCreate_Copy != null && ObjPharmacySellingPriceList_AutoCreate_Copy.Count > 0)
            {
                ObjPharmacySellingPriceList_AutoCreate = ObjPharmacySellingPriceList_AutoCreate_Copy.DeepCopy();
                LoadDataGrid();
            }
        }

        //private Visibility _dgCellTemplate0_Visible = Visibility.Visible;
        //public Visibility dgCellTemplate0_Visible
        //{
        //    get { return _dgCellTemplate0_Visible; }
        //    set
        //    {
        //        _dgCellTemplate0_Visible = value;
        //        NotifyOfPropertyChange(() => dgCellTemplate0_Visible);
        //    }
        //}

        //private void SetStatusRowForUpdate(PharmacySellingItemPrices item)
        //{
        //    item.ObjPharmacySellingItemPricesPrice.RowState = PharmacySellingItemPricesPrice.RowStateValue.NoChange;
        //}

        //Kiểm Tra Ngày Cho Update Bảng Giá
        private string CheckEffectiveDate_PriceList_Update()
        {
            if (ObjPharmacySellingPriceList_Current.EffectiveDate.Subtract(EffectiveDate_tmp).Days <= 0)
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
            ObjPharmacySellingPriceList_AutoCreate_Save.Clear();

            foreach (PharmacySellingItemPrices item in ObjPharmacySellingPriceList_AutoCreate)
            {
                if (item.RowState == PharmacySellingItemPrices.RowStateValue.Update)
                {
                    ObjPharmacySellingPriceList_AutoCreate_Save.Add(item);
                }
            }
            ObjPharmacySellingPriceList_Current.ObjPharmacySellingItemPrices = ObjPharmacySellingPriceList_AutoCreate_Save;
        }

        private void ThuTucUpDate()
        {
            if (CheckValidPrice(ObjPharmacySellingPriceList_Current) == false)
            {
                return;
            }
            string message = "";
            if (CheckPriceBeforeSave(ref message))
            {
                if (message != ""
                    && MessageBox.Show(message + string.Format("\n{0}", eHCMSResources.T3805_G1_BanCoMuonTiepTucKhong), eHCMSResources.T3805_G1_BanCoMuonTiepTucKhong, MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                {
                    return;
                }
                string Loai = CheckEffectiveDate_PriceList_Update();

                switch (Loai)
                {
                    case "Edit-PriceList-Current":
                        {
                            PharmacySellingPriceList_Update();
                            break;
                        }
                    case "Edit-PriceList-Future":
                        {
                            if (ObjPharmacySellingPriceList_Current.EffectiveDate.Subtract(Globals.ServerDate.Value).Days > 0)
                            {
                                PharmacySellingPriceList_Update();
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

        //public void PharmacySellingItemPricesPriceList_Update(PharmacySellingItemPricesPriceList Obj, ObservableCollection<PharmacySellingItemPrices> ObjCollection_Update)
        //{
        //    Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "Đang Ghi..." });

        //    var t = new Thread(() =>
        //    {
        //        using (var serviceFactory = new ConfigurationManagerServiceClient())
        //        {
        //            var contract = serviceFactory.ServiceInstance;

        //            contract.BeginPharmacySellingItemPricesPriceList_Update(Obj, ObjCollection_Update, Globals.DispatchCallback((asyncResult) =>
        //            {
        //                try
        //                {
        //                    string Result_PriceList = "";

        //                    contract.EndPharmacySellingItemPricesPriceList_Update(out Result_PriceList, asyncResult);
        //                    switch (Result_PriceList)
        //                    {
        //                        case "Update-0":
        //                            {
        //                                MessageBox.Show("Hiệu Chỉnh Bảng Giá Không Thành Công!", "Hiệu Chỉnh Bảng Giá", MessageBoxButton.OK);
        //                                break;
        //                            }
        //                        case "Update-1":
        //                            {
        //                                Globals.EventAggregator.Publish(new SaveEvent<bool>() { Result = true });
        //                                MessageBox.Show(eHCMSResources.Z1562_G1_DaLuu, "Hiệu Chỉnh Bảng Giá", MessageBoxButton.OK);
        //                                break;
        //                            }
        //                    }
        //                }
        //                catch (Exception ex)
        //                {
        //                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
        //                }
        //                finally
        //                {
        //                    Globals.IsBusy = false;
        //                }
        //            }), null);
        //        }


        //    });
        //    t.Start();
        //}
        //#endregion

        #region "btChooseItemFromDelete"
        public void btChooseItemFromDelete()
        {
            Action<IPharmacySellingPriceList_ChooseFromDelete> onInitDlg = delegate (IPharmacySellingPriceList_ChooseFromDelete typeInfo)
            {
                typeInfo.ObjPharmacySellingItemPrices_All_Virtual_Delete = ObjPharmacySellingItemPrices_All_Virtual_Delete;
            };
            GlobalsNAV.ShowDialog<IPharmacySellingPriceList_ChooseFromDelete>(onInitDlg);
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
                ObjPharmacySellingPriceList_Current.EffectiveDate = BeginDate;
            }
        }
        #endregion

        public void Handle(SelectedObjectEvent<PharmacySellingItemPrices> message)
        {
            if (message != null)
            {
                PharmacySellingItemPrices Objtmp = message.Result;
                UnMarkDeleteRow_VirList(Objtmp);
                SetbtChooseItemFromDelete_IsEnabled();
            }
        }

        private void SetbtChooseItemFromDelete_IsEnabled()
        {
            if (ObjPharmacySellingItemPrices_All_Virtual_Delete.Count > 0)
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
            if (string.IsNullOrEmpty(ObjPharmacySellingPriceList_Current.PriceListTitle))
            {
                MessageBox.Show(eHCMSResources.A0882_G1_Msg_InfoNhapTieuDeBGia);
                return;
            }

            if (ObjPharmacySellingPriceList_Current != null && ObjPharmacySellingPriceList_Current.PharmacySellingPriceListID > 0)/*Update*/
            {
                Action<ICommonPreviewView> onInitDlg = delegate (ICommonPreviewView proAlloc)
                {
                    proAlloc.TieuDeRpt = string.Format(eHCMSResources.Z1483_G1_BGiaBanThuocThang, ObjPharmacySellingPriceList_Current.RecCreatedDate.Month.ToString(), ObjPharmacySellingPriceList_Current.RecCreatedDate.Year.ToString());
                    proAlloc.PharmacySellingPriceListID = ObjPharmacySellingPriceList_Current.PharmacySellingPriceListID;
                    //  proAlloc.eItem = ReportName.RptBangGiaThuocNhaThuoc;
                    proAlloc.eItem = ReportName.XRptPharmacySellingPriceList_Detail_Simple;
                };
                GlobalsNAV.ShowDialog<ICommonPreviewView>(onInitDlg);
            }
            else//Tạo mới
            {
                Action<ICommonPreviewView> onInitDlg = delegate (ICommonPreviewView proAlloc)
                {
                    proAlloc.TieuDeRpt = string.Format(eHCMSResources.Z1484_G1_BGiaThuoc0, ObjPharmacySellingPriceList_Current.PriceListTitle.Trim());
                    proAlloc.Result = "";
                    proAlloc.eItem = ReportName.RptBangGiaThuocNhaThuoc_AutoCreate;
                };
                GlobalsNAV.ShowDialog<ICommonPreviewView>(onInitDlg);
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
            //pagerSellingList.Source = ObjPharmacySellingPriceList_AutoCreate_Paging;
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
            CVS_ObjPharmacySellingPriceList_AutoCreate_Paging = null;
            CVS_ObjPharmacySellingPriceList_AutoCreate_Paging = new CollectionViewSource { Source = ObjPharmacySellingPriceList_AutoCreate };
            CV_ObjPharmacySellingPriceList_AutoCreate_Paging = (CollectionView)CVS_ObjPharmacySellingPriceList_AutoCreate_Paging.View;
            NotifyOfPropertyChange(() => CV_ObjPharmacySellingPriceList_AutoCreate_Paging);
            btnFilter();
        }

        public void btnFilter()
        {
            CV_ObjPharmacySellingPriceList_AutoCreate_Paging.Filter = null;
            CV_ObjPharmacySellingPriceList_AutoCreate_Paging.Filter = new Predicate<object>(DoFilter);
        }

        //Callback method
        private bool DoFilter(object o)
        {
            //it is not a case sensitive search
            PharmacySellingItemPrices emp = o as PharmacySellingItemPrices;

            if (emp == null)
            {
                return false;
            }

            if (string.IsNullOrEmpty(SearchKey))
            {
                SearchKey = "";
            }

            if (SearchKey.Length == 1)
            {
                if (emp.BrandName.ToLower().StartsWith(SearchKey.Trim().ToLower()) || emp.DrugCode.ToLower() == SearchKey.Trim().ToLower())
                {
                    return true;
                }
            }
            else
            {
                if (emp.BrandName.ToLower().IndexOf(SearchKey.Trim().ToLower()) >= 0 || emp.DrugCode.ToLower() == SearchKey.Trim().ToLower())
                {
                    return true;
                }
            }
            return false;
        }

        //Callback method
        //private bool DoFilter(object o)
        //{
        //    //it is not a case sensitive search
        //    PharmacySellingItemPrices emp = o as PharmacySellingItemPrices;
        //    if (emp != null)
        //    {
        //        if (string.IsNullOrEmpty(SearchKey))
        //        {
        //            SearchKey = "";
        //        }
        //        if (emp.BrandName.ToLower().IndexOf(SearchKey.Trim().ToLower()) >= 0)
        //        {
        //            return true;
        //        }
        //        else
        //        {
        //            return false;
        //        }
        //    }
        //    return false;
        //}

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
            if (PagingChecked != null && ObjPharmacySellingPriceList_AutoCreate != null)
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
