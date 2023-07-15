using eHCMSLanguage;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ServiceModel;
using System.Threading;
using System.Windows;
using aEMR.DataContracts;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.ViewContracts.Configuration;
using Caliburn.Micro;
using DataEntities;
using aEMR.Common;
using aEMR.Common.Collections;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using aEMR.ViewContracts;

namespace aEMR.Configuration.PCLExamTypePrices.ViewModels
{
    [Export(typeof(IPCLExamTypePrices)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class PCLExamTypePricesViewModel : Conductor<object>, IPCLExamTypePrices
        ,IHandle<PCLExamTypePricesEvent>
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public PCLExamTypePricesViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            eventAggregator.Subscribe(this);
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



        protected override void OnActivate()
        {
            base.OnActivate();

            Globals.EventAggregator.Subscribe(this);

            ObjPCLExamTypePrices_ByPCLExamTypeID_Paging = new PagedSortableCollectionView<PCLExamTypePrice>();
            ObjPCLExamTypePrices_ByPCLExamTypeID_Paging.OnRefresh += new EventHandler<RefreshEventArgs>(ObjPCLExamTypePrices_ByPCLExamTypeID_Paging_OnRefresh);
            if (CheckDateValid())
            {
                ObjPCLExamTypePrices_ByPCLExamTypeID_Paging.PageIndex = 0;
                SearchListPrice(0, ObjPCLExamTypePrices_ByPCLExamTypeID_Paging.PageSize, true);
            }
        }

        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);
        }

        void ObjPCLExamTypePrices_ByPCLExamTypeID_Paging_OnRefresh(object sender, RefreshEventArgs e)
        {
            if (CheckDateValid())
            {
                SearchListPrice(ObjPCLExamTypePrices_ByPCLExamTypeID_Paging.PageIndex,
                                ObjPCLExamTypePrices_ByPCLExamTypeID_Paging.PageSize, false);
            }
        }

        private DataEntities.PCLExamTypePriceSearchCriteria _SearchCriteria;
        public DataEntities.PCLExamTypePriceSearchCriteria SearchCriteria
        {
            get
            {
                return _SearchCriteria;
            }
            set
            {
                _SearchCriteria = value;
                NotifyOfPropertyChange(()=>SearchCriteria);
            }
        }

        private DataEntities.PCLExamType _ObjPCLExamType_Current;
        public DataEntities.PCLExamType ObjPCLExamType_Current
        {
            get { return _ObjPCLExamType_Current; }
            set
            {
                _ObjPCLExamType_Current = value;
                NotifyOfPropertyChange(() => ObjPCLExamType_Current);
            }
        }

        private PagedSortableCollectionView<DataEntities.PCLExamTypePrice> _ObjPCLExamTypePrices_ByPCLExamTypeID_Paging;
        public PagedSortableCollectionView<DataEntities.PCLExamTypePrice> ObjPCLExamTypePrices_ByPCLExamTypeID_Paging
        {
            get { return _ObjPCLExamTypePrices_ByPCLExamTypeID_Paging; }
            set
            {
                _ObjPCLExamTypePrices_ByPCLExamTypeID_Paging = value;
                NotifyOfPropertyChange(() => ObjPCLExamTypePrices_ByPCLExamTypeID_Paging);
            }
        }

        private void SearchListPrice(int PageIndex, int PageSize, bool CountTotal)
        {
           //Globals.EventAggregator.Publish(new BusyEvent() { IsBusy = true, Message = "Danh Sách Giá..." });

            var t = new Thread(() =>
            {
                IsLoading = true;

                try
                {
                    using (var serviceFactory = new ConfigurationManagerServiceClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginPCLExamTypePrices_ByPCLExamTypeID_Paging(SearchCriteria, PageIndex, PageSize, "", CountTotal, Globals.DispatchCallback((asyncResult) =>
                        {
                            int Total = 0;
                            IList<DataEntities.PCLExamTypePrice> allItems = null;
                            bool bOK = false;
                            try
                            {
                                allItems = client.EndPCLExamTypePrices_ByPCLExamTypeID_Paging(out Total, asyncResult);
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

                            ObjPCLExamTypePrices_ByPCLExamTypeID_Paging.Clear();
                            if (bOK)
                            {
                                if (CountTotal)
                                {
                                    ObjPCLExamTypePrices_ByPCLExamTypeID_Paging.TotalItemCount = Total;
                                }
                                if (allItems != null)
                                {
                                    foreach (var item in allItems)
                                    {
                                        ObjPCLExamTypePrices_ByPCLExamTypeID_Paging.Add(item);
                                    }
                                    //Do danh sách PCLExamType cho xem giá hiện hành cho tiện, nên kết qua Giá. Khi vô Danh Sách Giá sẽ bị dòng rỗng nên check chỗ này
                                    if (ObjPCLExamTypePrices_ByPCLExamTypeID_Paging.Count > 0)
                                    {
                                        if (ObjPCLExamTypePrices_ByPCLExamTypeID_Paging[0].PCLExamTypePriceID <= 0)
                                        {
                                            ObjPCLExamTypePrices_ByPCLExamTypeID_Paging.Clear();
                                        }
                                    }
                                    //Do danh sách PCLExamType cho xem giá hiện hành cho tiện, nên kết qua Giá. Khi vô Danh Sách Giá sẽ bị dòng rỗng nên check chỗ này
                                }
                            }
                            else
                            {
                                //Do danh sách PCLExamType cho xem giá hiện hành cho tiện, nên kết qua Giá. Khi vô Danh Sách Giá sẽ bị dòng rỗng nên check chỗ này
                                if (ObjPCLExamTypePrices_ByPCLExamTypeID_Paging.Count > 0)
                                {
                                    if (ObjPCLExamTypePrices_ByPCLExamTypeID_Paging[0].PCLExamTypePriceID <= 0)
                                    {
                                        ObjPCLExamTypePrices_ByPCLExamTypeID_Paging.Clear();
                                    }
                                }
                                //Do danh sách PCLExamType cho xem giá hiện hành cho tiện, nên kết qua Giá. Khi vô Danh Sách Giá sẽ bị dòng rỗng nên check chỗ này
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
    

        public void cboPriceTypeSelectedItemChanged(object selectedIndex)
        {
            if (selectedIndex != null)
            {
                SearchCriteria.PriceType = int.Parse(selectedIndex.ToString()) - 1;
                if (CheckDateValid())
                {
                    ObjPCLExamTypePrices_ByPCLExamTypeID_Paging.PageIndex = 0;
                    SearchListPrice(0, ObjPCLExamTypePrices_ByPCLExamTypeID_Paging.PageSize, true);
                }
            }
        }

        public void btFind()
        {
            if (CheckDateValid())
            {
                ObjPCLExamTypePrices_ByPCLExamTypeID_Paging.PageIndex = 0;
                SearchListPrice(0, ObjPCLExamTypePrices_ByPCLExamTypeID_Paging.PageSize, true);
            }
        }

        private bool CheckDateValid()
        {
            if (IStatusCheckFindDate)
            {
                if (SearchCriteria.FromDate != null && SearchCriteria.ToDate != null)
                {
                    if (SearchCriteria.FromDate > SearchCriteria.ToDate)
                    {
                        MessageBox.Show(eHCMSResources.Z0467_G1_TuNgNhoHonBangDenNg, eHCMSResources.G0981_G1_Tim, MessageBoxButton.OK);
                        return false;
                    }
                    return true;
                }
                else
                {
                    MessageBox.Show(eHCMSResources.A0885_G1_Msg_InfoNhapTuNgDenNg2, eHCMSResources.G0981_G1_Tim, MessageBoxButton.OK);
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
                SearchCriteria.FromDate = null;
                SearchCriteria.ToDate = null;
            }
        }

        public void btClose()
        {
            TryClose();
        }

        public void hplAddNewPrice()
        {
            //var typeInfo = Globals.GetViewModel<IPCLExamTypePrices_AddEdit>();
            //typeInfo.ObjPCLExamType_Current = ObjPCLExamType_Current;
            //typeInfo.TitleForm = string.Format(eHCMSResources.Z0663_G1_ThemGiaMoi0, ObjPCLExamType_Current.PCLExamTypeName.Trim());

            //typeInfo.InitializeNewItem();

            //var instance = typeInfo as Conductor<object>;

            //Globals.ShowDialog(instance, (o) =>
            //{
            //    //làm gì đó
            //});

            Action<IPCLExamTypePrices_AddEdit> onInitDlg = (typeInfo) =>
            {
                typeInfo.ObjPCLExamType_Current = ObjPCLExamType_Current;
                typeInfo.TitleForm = string.Format(eHCMSResources.Z0663_G1_ThemGiaMoi0, ObjPCLExamType_Current.PCLExamTypeName.Trim());
                typeInfo.InitializeNewItem();
            };
            GlobalsNAV.ShowDialog<IPCLExamTypePrices_AddEdit>(onInitDlg);
        }

        public void hplEditPrice_Click(object datacontext)
        {
            DataEntities.PCLExamTypePrice p = (datacontext as DataEntities.PCLExamTypePrice);

            if (p.PriceType == "PriceOld")
            {
                MessageBox.Show(eHCMSResources.Z0661_G1_GiaCuTrongQuaKhu, eHCMSResources.Z0660_G1_SuaGia, MessageBoxButton.OK);
            }
            else
            {
                //var typeInfo = Globals.GetViewModel<IPCLExamTypePrices_AddEdit>();
                //typeInfo.ObjPCLExamType_Current = ObjPCLExamType_Current;
                //typeInfo.TitleForm = string.Format(eHCMSResources.Z0664_G1_HChinhGia0, p.PCLExamTypeName.Trim());
                //typeInfo.ObjPCLExamTypePrice_Current =ObjectCopier.DeepCopy(p);

                //var instance = typeInfo as Conductor<object>;

                //Globals.ShowDialog(instance, (o) =>
                //{
                //    //làm gì đó
                //});

                Action<IPCLExamTypePrices_AddEdit> onInitDlg = (typeInfo) =>
                {
                    typeInfo.ObjPCLExamType_Current = ObjPCLExamType_Current;
                    typeInfo.TitleForm = string.Format(eHCMSResources.Z0664_G1_HChinhGia0, p.PCLExamTypeName.Trim());
                    typeInfo.ObjPCLExamTypePrice_Current = ObjectCopier.DeepCopy(p);
                };
                GlobalsNAV.ShowDialog<IPCLExamTypePrices_AddEdit>(onInitDlg);
            }
        }


        public void hplDeletePrice_Click(object datacontext)
        {
            DataEntities.PCLExamTypePrice p = (datacontext as DataEntities.PCLExamTypePrice);

            if (p.CanDelete.Value)
            {
                if (MessageBox.Show(eHCMSResources.A0159_G1_Msg_ConfXoaGia, eHCMSResources.G2617_G1_Xoa, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    PCLExamTypePrices_MarkDelete(p.PCLExamTypePriceID);
                }
            }
            else
            {
                if (p.PriceType == "PriceCurrent")
                {
                    MessageBox.Show(eHCMSResources.A0552_G1_Msg_InfoKhDcXoaGiaDangApDung, eHCMSResources.G2617_G1_Xoa, MessageBoxButton.OK);
                }
            }
        }
        private void PCLExamTypePrices_MarkDelete(Int64 PCLExamTypePriceID)
        {
             string Result = "";

             //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "Đang Xóa..." });

             var t = new Thread(() =>
             {
                 IsLoading = true;

                 using (var serviceFactory = new ConfigurationManagerServiceClient())
                 {
                     var contract = serviceFactory.ServiceInstance;

                     contract.BeginPCLExamTypePrices_MarkDelete(PCLExamTypePriceID, Globals.DispatchCallback((asyncResult) =>
                     {
                         try
                         {
                             contract.EndPCLExamTypePrices_MarkDelete(out Result, asyncResult);
                             if (Result == "Delete-1")
                             {
                                 ObjPCLExamTypePrices_ByPCLExamTypeID_Paging.PageIndex = 0;
                                 SearchListPrice(0, ObjPCLExamTypePrices_ByPCLExamTypeID_Paging.PageSize, true);
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
                             //Globals.IsBusy = false;
                             IsLoading = false;
                         }
                     }), null);
                 }


             });
             t.Start();
        }

        public void Handle(PCLExamTypePricesEvent message)
        {
            if(message!=null)
            {
                ObjPCLExamTypePrices_ByPCLExamTypeID_Paging.PageIndex = 0;
                SearchListPrice(0, ObjPCLExamTypePrices_ByPCLExamTypeID_Paging.PageSize, true);                
            }
        }
    }
}
