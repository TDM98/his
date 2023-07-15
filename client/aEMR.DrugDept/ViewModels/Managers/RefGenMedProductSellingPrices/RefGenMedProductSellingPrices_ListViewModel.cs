using eHCMSLanguage;
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
using aEMR.Infrastructure.CachingUtils;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using Caliburn.Micro;
using DataEntities;
using aEMR.Common;
using Castle.Windsor;
using Castle.Core.Logging;
using aEMR.Common.Collections;

namespace aEMR.DrugDept.ViewModels
{
    [Export(typeof(IRefGenMedProductSellingPrices_List)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class RefGenMedProductSellingPrices_ListViewModel : Conductor<object>, IRefGenMedProductSellingPrices_List
        , IHandle<RefGenMedProductSellingPrices_AddEditViewModel_Save_Event>
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public RefGenMedProductSellingPrices_ListViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            eventAggregator.Subscribe(this);

            ObjRefGenMedProductSellingPrices_Paging =new PagedSortableCollectionView<RefGenMedProductSellingPrices>();

            ObjRefGenMedProductSellingPrices_Paging.OnRefresh += new EventHandler<RefreshEventArgs>(ObjRefGenMedProductSellingPrices_Paging_OnRefresh);
            if (CheckDateValid())
            {
                ObjRefGenMedProductSellingPrices_Paging.PageIndex = 0;
                RefGenMedProductSellingPrices_ByGenMedProductID_Paging(0, ObjRefGenMedProductSellingPrices_Paging.PageSize, true);
            }
        }

        void ObjRefGenMedProductSellingPrices_Paging_OnRefresh(object sender, RefreshEventArgs e)
        {
            if (CheckDateValid())
            {
                RefGenMedProductSellingPrices_ByGenMedProductID_Paging(ObjRefGenMedProductSellingPrices_Paging.PageIndex,
                                ObjRefGenMedProductSellingPrices_Paging.PageSize, false);
            }
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

        private DataEntities.RefGenMedProductSellingPricesSearchCriteria _Criteria;
        public DataEntities.RefGenMedProductSellingPricesSearchCriteria Criteria
        {
            get
            {
                return _Criteria;
            }
            set
            {
                _Criteria = value;
                NotifyOfPropertyChange(()=>Criteria);
            }
        }
        
        private RefGenMedProductSellingPrices _ObjRefGenMedProductSellingPrices_Current;
        public RefGenMedProductSellingPrices ObjRefGenMedProductSellingPrices_Current
        {
            get { return _ObjRefGenMedProductSellingPrices_Current; }
            set 
            {   
                _ObjRefGenMedProductSellingPrices_Current = value;
                NotifyOfPropertyChange(()=>ObjRefGenMedProductSellingPrices_Current);
            }
        }

        private PagedSortableCollectionView<DataEntities.RefGenMedProductSellingPrices> _ObjRefGenMedProductSellingPrices_Paging;
        public PagedSortableCollectionView<DataEntities.RefGenMedProductSellingPrices> ObjRefGenMedProductSellingPrices_Paging
        {
            get { return _ObjRefGenMedProductSellingPrices_Paging; }
            set
            {
                _ObjRefGenMedProductSellingPrices_Paging = value;
                NotifyOfPropertyChange(() => ObjRefGenMedProductSellingPrices_Paging);
            }
        }

        private RefGenMedProductSellingPrices _ObjRefGenMedProductDetails_Info;
        public RefGenMedProductSellingPrices ObjRefGenMedProductDetails_Info
        {
            get { return _ObjRefGenMedProductDetails_Info; }
            set
            {
                _ObjRefGenMedProductDetails_Info = value;
                NotifyOfPropertyChange(()=>ObjRefGenMedProductDetails_Info);
            }
        }

        private void RefGenMedProductSellingPrices_ByGenMedProductID_Paging(int PageIndex, int PageSize, bool CountTotal)
        {
           Globals.EventAggregator.Publish(new BusyEvent() { IsBusy = true, Message = string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.K2978_G1_DSGia) });
           IsLoading = true;
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyGenMedDeptDetailsServiceClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginRefGenMedProductSellingPrices_ByGenMedProductID_Paging(Criteria, PageIndex, PageSize, "", CountTotal, Globals.DispatchCallback((asyncResult) =>
                        {
                            int Total = 0;
                            IList<DataEntities.RefGenMedProductSellingPrices> allItems = null;
                            bool bOK = false;
                            try
                            {
                                allItems = client.EndRefGenMedProductSellingPrices_ByGenMedProductID_Paging(out Total, asyncResult);
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

                            ObjRefGenMedProductSellingPrices_Paging.Clear();
                            if (bOK)
                            {
                                if (CountTotal)
                                {
                                    ObjRefGenMedProductSellingPrices_Paging.TotalItemCount = Total;
                                }
                                if (allItems != null)
                                {
                                    foreach (var item in allItems)
                                    {
                                        ObjRefGenMedProductSellingPrices_Paging.Add(item);
                                    }
                                    
                                    //Do việc lấy dữ liệu join 2 bảng nên thông tin luôn có, giá thì có khi chưa có
                                    if (ObjRefGenMedProductSellingPrices_Paging.Count > 0)
                                    {
                                        ObjRefGenMedProductDetails_Info = ObjRefGenMedProductSellingPrices_Paging[0];
                                        if(ObjRefGenMedProductDetails_Info.GenMedSellPriceID <= 0)
                                            ObjRefGenMedProductSellingPrices_Paging.Clear();
                                    }
                                    //Do việc lấy dữ liệu join 2 bảng nên thông tin luôn có, giá thì có khi chưa có
                                }
                            }
                            else
                            {
                                //Do việc lấy dữ liệu join 2 bảng nên thông tin luôn có, giá thì có khi chưa có
                                if (ObjRefGenMedProductSellingPrices_Paging.Count > 0)
                                {
                                    ObjRefGenMedProductDetails_Info = ObjRefGenMedProductSellingPrices_Paging[0];
                                    if (ObjRefGenMedProductDetails_Info.GenMedSellPriceID <= 0)
                                        ObjRefGenMedProductSellingPrices_Paging.Clear();
                                }
                                //Do việc lấy dữ liệu join 2 bảng nên thông tin luôn có, giá thì có khi chưa có
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

        public void cboPriceTypeSelectedItemChanged(object selectedIndex)
        {
            if (selectedIndex != null)
            {
                Criteria.PriceType = int.Parse(selectedIndex.ToString()) - 1;
                if (CheckDateValid())
                {
                    ObjRefGenMedProductSellingPrices_Paging.PageIndex = 0;
                    RefGenMedProductSellingPrices_ByGenMedProductID_Paging(0, ObjRefGenMedProductSellingPrices_Paging.PageSize, true);
                }
            }
        }

        public void btFind()
        {
            if (CheckDateValid())
            {
                ObjRefGenMedProductSellingPrices_Paging.PageIndex = 0;
                RefGenMedProductSellingPrices_ByGenMedProductID_Paging(0, ObjRefGenMedProductSellingPrices_Paging.PageSize, true);
            }
        }

        private bool CheckDateValid()
        {
            if (IStatusCheckFindDate)
            {
                if (Criteria.FromDate != null && Criteria.ToDate != null)
                {
                    if (Criteria.FromDate > Criteria.ToDate)
                    {
                        MessageBox.Show(string.Format("{0}!", eHCMSResources.Z0467_G1_TuNgNhoHonBangDenNg), eHCMSResources.G0981_G1_Tim, MessageBoxButton.OK);
                        return false;
                    }
                    return true;
                }
                else
                {
                    MessageBox.Show(string.Format("{0}!", eHCMSResources.A0885_G1_Msg_InfoNhapTuNgDenNg2), eHCMSResources.G0981_G1_Tim, MessageBoxButton.OK);
                    return false;
                }
            }
            return true;
        }

        private bool _IStatusFromDate = false;
        public bool IStatusFromDate
        {
            get { return _IStatusFromDate; }
            set
            {
                _IStatusFromDate = value;
                NotifyOfPropertyChange(() => IStatusFromDate);
            }
        }

        private bool _IStatusToDate = false;
        public bool IStatusToDate
        {
            get { return _IStatusToDate; }
            set
            {
                _IStatusToDate = value;
                NotifyOfPropertyChange(() => IStatusToDate);
            }
        }

        private bool _IStatusCheckFindDate;
        public bool IStatusCheckFindDate
        {
            get { return _IStatusCheckFindDate; }
            set
            {
                _IStatusCheckFindDate = value;
                NotifyOfPropertyChange(() => IStatusCheckFindDate);
            }
        }


        public void chkFindByDate_Click(object args)
        {
            IStatusCheckFindDate = (((System.Windows.Controls.Primitives.ToggleButton)(((System.Windows.RoutedEventArgs)(args)).OriginalSource)).IsChecked.Value);
            IStatusFromDate = IStatusCheckFindDate;
            IStatusToDate = IStatusCheckFindDate;
            if (IStatusCheckFindDate == false)
            {
                Criteria.FromDate = null;
                Criteria.ToDate = null;
            }
        }

        public void btClose()
        {
            TryClose();
        }

        public void hplDeletePrice_Click(object datacontext)
        {
            DataEntities.RefGenMedProductSellingPrices p = (datacontext as DataEntities.RefGenMedProductSellingPrices);

            if (p.CanDelete.Value)
            {
                if (MessageBox.Show(eHCMSResources.A0159_G1_Msg_ConfXoaGia, eHCMSResources.G2617_G1_Xoa, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    RefGenMedProductSellingPricess_MarkDelete(p.GenMedSellPriceID);
                }
            }
            else
            {
                if (p.PriceType == "PriceCurrent")
                {
                    MessageBox.Show(string.Format("{0}!", eHCMSResources.A0552_G1_Msg_InfoKhDcXoaGiaDangApDung), eHCMSResources.G2617_G1_Xoa, MessageBoxButton.OK);
                }
            }
        }
        private void RefGenMedProductSellingPricess_MarkDelete(Int64 GenMedSellPriceID)
        {
             string Result = "";

             Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.Z0492_G1_DangXoa) });
             IsLoading = true;
             var t = new Thread(() =>
             {
                 using (var serviceFactory = new PharmacyGenMedDeptDetailsServiceClient())
                 {
                     var contract = serviceFactory.ServiceInstance;

                     contract.BeginRefGenMedProductSellingPrices_MarkDelete(GenMedSellPriceID, Globals.DispatchCallback((asyncResult) =>
                     {
                         try
                         {
                             contract.EndRefGenMedProductSellingPrices_MarkDelete(out Result, asyncResult);
                             if (Result == "Delete-1")
                             {
                                 ObjRefGenMedProductSellingPrices_Paging.PageIndex = 0;
                                 RefGenMedProductSellingPrices_ByGenMedProductID_Paging(0, ObjRefGenMedProductSellingPrices_Paging.PageSize, true);
                                 MessageBox.Show(eHCMSResources.A0478_G1_Msg_InfoXoaOK, eHCMSResources.A0481_G1_Msg_XoaGia, MessageBoxButton.OK);
                             }
                             else
                             {
                                 MessageBox.Show(eHCMSResources.K0484_G1_XoaFail, eHCMSResources.A0481_G1_Msg_XoaGia, MessageBoxButton.OK);
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

        public void dtgList_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            DataEntities.RefGenMedProductSellingPrices objRows = e.Row.DataContext as DataEntities.RefGenMedProductSellingPrices;
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
                    case "PriceOld":
                        {
                            e.Row.Foreground = new SolidColorBrush(Colors.Orange);
                            break;
                        }
                }
            }
        }

        public void hplAddNewPrice()
        {
            //var typeInfo = Globals.GetViewModel<IRefGenMedProductSellingPrices_AddEdit>();
            //typeInfo.ObjRefGenMedProductDetails_Info = ObjRefGenMedProductDetails_Info;

            //typeInfo.TitleForm = string.Format(eHCMSResources.Z0663_G1_ThemGiaMoi0, ObjRefGenMedProductDetails_Info.ObjGenMedProductID.BrandName.Trim());
            //typeInfo.InitializeNewItem(ObjRefGenMedProductDetails_Info.GenMedProductID);

            //var instance = typeInfo as Conductor<object>;

            //Globals.ShowDialog(instance, (o) =>
            //{
            //    //lam gi do
            //});

            Action<IRefGenMedProductSellingPrices_AddEdit> onInitDlg = (typeInfo) =>
            {
                typeInfo.ObjRefGenMedProductDetails_Info = ObjRefGenMedProductDetails_Info;

                typeInfo.TitleForm = string.Format(eHCMSResources.Z0663_G1_ThemGiaMoi0, ObjRefGenMedProductDetails_Info.ObjGenMedProductID.BrandName.Trim());
                typeInfo.InitializeNewItem(ObjRefGenMedProductDetails_Info.GenMedProductID);
            };
            GlobalsNAV.ShowDialog<IRefGenMedProductSellingPrices_AddEdit>(onInitDlg);
        }

        public void hplEditPrice_Click(object datacontext)
        {
            DataEntities.RefGenMedProductSellingPrices p = (datacontext as DataEntities.RefGenMedProductSellingPrices);

            if (p.PriceType == "PriceOld")
            {
                MessageBox.Show(string.Format("{0}!", eHCMSResources.Z0661_G1_GiaCuTrongQuaKhu), eHCMSResources.Z0660_G1_SuaGia, MessageBoxButton.OK);
            }
            else
            {
                //var typeInfo = Globals.GetViewModel<IRefGenMedProductSellingPrices_AddEdit>();
                //typeInfo.ObjRefGenMedProductDetails_Info = ObjRefGenMedProductDetails_Info;

                //typeInfo.ObjRefGenMedProductSellingPrices_Current =ObjectCopier.DeepCopy(p);
                //typeInfo.TitleForm = string.Format(eHCMSResources.Z0664_G1_HChinhGia0, ObjRefGenMedProductDetails_Info.ObjGenMedProductID.BrandName);
                //var instance = typeInfo as Conductor<object>;

                //Globals.ShowDialog(instance, (o) =>
                //{
                //    //lam gi do
                //});

                Action<IRefGenMedProductSellingPrices_AddEdit> onInitDlg = (typeInfo) =>
                {
                    typeInfo.ObjRefGenMedProductDetails_Info = ObjRefGenMedProductDetails_Info;

                    typeInfo.ObjRefGenMedProductSellingPrices_Current = ObjectCopier.DeepCopy(p);
                    typeInfo.TitleForm = string.Format(eHCMSResources.Z0664_G1_HChinhGia0, ObjRefGenMedProductDetails_Info.ObjGenMedProductID.BrandName);
                };
                GlobalsNAV.ShowDialog<IRefGenMedProductSellingPrices_AddEdit>(onInitDlg);
            }
        }

        public void Handle(RefGenMedProductSellingPrices_AddEditViewModel_Save_Event message)
        {
            if(message!=null)
            {
                ObjRefGenMedProductSellingPrices_Paging.PageIndex = 0;
                RefGenMedProductSellingPrices_ByGenMedProductID_Paging(0, ObjRefGenMedProductSellingPrices_Paging.PageSize, true);
            }
        }
    }
}

