using eHCMSLanguage;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Net;
using System.ServiceModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using aEMR.DataContracts;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using Caliburn.Micro;
using DataEntities;
using aEMR.Common;
using aEMR.Common.Collections;

namespace aEMR.DrugDept.ViewModels
{
    [Export(typeof(ISupplierGenMedProductsPrice_ListPrice)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class SupplierGenMedProductsPrice_ListPriceViewModel : Conductor<object>, ISupplierGenMedProductsPrice_ListPrice
        ,IHandle<SaveEvent<bool>>
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public SupplierGenMedProductsPrice_ListPriceViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            eventAggregator.Subscribe(this);

        }

        protected override void OnActivate()
        {
            base.OnActivate();
            authorization();

            Globals.EventAggregator.Subscribe(this);

            ObjSupplierGenMedProductsPrice_ListPrice_Paging = new PagedSortableCollectionView<SupplierGenMedProductsPrice>();
            ObjSupplierGenMedProductsPrice_ListPrice_Paging.OnRefresh += new EventHandler<RefreshEventArgs>(ObjSupplierGenMedProductsPrice_ListPrice_Paging_OnRefresh);
            if (CheckDateValid())
            {
                ObjSupplierGenMedProductsPrice_ListPrice_Paging.PageIndex = 0;
                SupplierGenMedProductsPrice_ListPrice_Paging(0, ObjSupplierGenMedProductsPrice_ListPrice_Paging.PageSize, true);
            }
        }

        void ObjSupplierGenMedProductsPrice_ListPrice_Paging_OnRefresh(object sender, RefreshEventArgs e)
        {
            if (CheckDateValid())
            {
                SupplierGenMedProductsPrice_ListPrice_Paging(ObjSupplierGenMedProductsPrice_ListPrice_Paging.PageIndex,ObjSupplierGenMedProductsPrice_ListPrice_Paging.PageSize, false);
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

        public void authorization()
        {
            if (!Globals.isAccountCheck)
            {
                return;
            }
        }
        #region checking account

        private bool _mTaoGiaMoi = true;
        private bool _mSuaGia = true;
        public bool mTaoGiaMoi
        {
            get
            {
                return _mTaoGiaMoi;
            }
            set
            {
                if (_mTaoGiaMoi == value)
                    return;
                _mTaoGiaMoi = value;
                NotifyOfPropertyChange(() => mTaoGiaMoi);
            }
        }
        public bool mSuaGia
        {
            get
            {
                return _mSuaGia;
            }
            set
            {
                if (_mSuaGia == value)
                    return;
                _mSuaGia = value;
                NotifyOfPropertyChange(() => mSuaGia);
            }
        }

        private bool _bEdit = true;
        private bool _bAdd = true;
        private bool _bDelete = true;
        private bool _bView = true;
        private bool _bPrint = true;
        private bool _bReport = true;

        public bool bEdit
        {
            get
            {
                return _bEdit;
            }
            set
            {
                if (_bEdit == value)
                    return;
                _bEdit = value;
            }
        }
        public bool bAdd
        {
            get
            {
                return _bAdd;
            }
            set
            {
                if (_bAdd == value)
                    return;
                _bAdd = value;
            }
        }
        public bool bDelete
        {
            get
            {
                return _bDelete;
            }
            set
            {
                if (_bDelete == value)
                    return;
                _bDelete = value;
            }
        }
        public bool bView
        {
            get
            {
                return _bView;
            }
            set
            {
                if (_bView == value)
                    return;
                _bView = value;
            }
        }
        public bool bPrint
        {
            get
            {
                return _bPrint;
            }
            set
            {
                if (_bPrint == value)
                    return;
                _bPrint = value;
            }
        }

        public bool bReport
        {
            get
            {
                return _bReport;
            }
            set
            {
                if (_bReport == value)
                    return;
                _bReport = value;
            }
        }
        #endregion
        #region binding visibilty

        public Button lnkDelete { get; set; }
        public Button lnkEdit { get; set; }

        public void lnkDelete_Loaded(object sender)
        {
            lnkDelete = sender as Button;
            lnkDelete.Visibility = Globals.convertVisibility(mSuaGia);
        }
        public void lnkEdit_Loaded(object sender)
        {
            lnkEdit = sender as Button;
            lnkEdit.Visibility = Globals.convertVisibility(mSuaGia);
        }
        #endregion

        private Supplier _ObjSupplierCurrent;
        public Supplier ObjSupplierCurrent
        {
            get { return _ObjSupplierCurrent; }
            set
            {
                _ObjSupplierCurrent = value;
                NotifyOfPropertyChange(() => ObjSupplierCurrent);
            }
        }

        private SupplierGenMedProductsPrice _ObjDrugCurrent;
        public SupplierGenMedProductsPrice ObjDrugCurrent
        {
            get { return _ObjDrugCurrent; }
            set
            {
                _ObjDrugCurrent = value;
                NotifyOfPropertyChange(() => ObjDrugCurrent);
            }
        }

        private SupplierGenMedProductsPriceSearchCriteria _Criteria;
        public SupplierGenMedProductsPriceSearchCriteria Criteria
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


        private PagedSortableCollectionView<DataEntities.SupplierGenMedProductsPrice> _ObjSupplierGenMedProductsPrice_ListPrice_Paging;
        public PagedSortableCollectionView<DataEntities.SupplierGenMedProductsPrice> ObjSupplierGenMedProductsPrice_ListPrice_Paging
        {
            get { return _ObjSupplierGenMedProductsPrice_ListPrice_Paging; }
            set
            {
                _ObjSupplierGenMedProductsPrice_ListPrice_Paging = value;
                NotifyOfPropertyChange(() => ObjSupplierGenMedProductsPrice_ListPrice_Paging);
            }
        }

        private void SupplierGenMedProductsPrice_ListPrice_Paging(int PageIndex, int PageSize, bool CountTotal)
        {
            Globals.EventAggregator.Publish(new BusyEvent() { IsBusy = true, Message = string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.K2978_G1_DSGia) });
            IsLoading = true;
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacySuppliersServiceClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginSupplierGenMedProductsPrice_ListPrice_Paging(Criteria, PageIndex, PageSize, "", CountTotal, Globals.DispatchCallback((asyncResult) =>
                        {
                            int Total = 0;
                            IList<DataEntities.SupplierGenMedProductsPrice> allItems = null;
                            bool bOK = false;
                            try
                            {
                                allItems = client.EndSupplierGenMedProductsPrice_ListPrice_Paging(out Total, asyncResult);
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

                            ObjSupplierGenMedProductsPrice_ListPrice_Paging.Clear();

                            if (bOK)
                            {
                                if (CountTotal)
                                {
                                    ObjSupplierGenMedProductsPrice_ListPrice_Paging.TotalItemCount = Total;
                                }
                                if (allItems != null)
                                {
                                    foreach (var item in allItems)
                                    {
                                        ObjSupplierGenMedProductsPrice_ListPrice_Paging.Add(item);
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
                    IsLoading = false;
                    Globals.IsBusy = false;
                }
            });
            t.Start();
        }

        public void dtgList_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            DataEntities.SupplierGenMedProductsPrice objRows = e.Row.DataContext as DataEntities.SupplierGenMedProductsPrice;
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
                    default:
                        {
                            e.Row.Foreground = new SolidColorBrush(Colors.Black);
                            break;
                        }
                }
            }
        }

        public void cboPriceTypeSelectedItemChanged(object selectedIndex)
        {
            if (selectedIndex != null)
            {
                Criteria.PriceType = int.Parse(selectedIndex.ToString()) - 1;
                if (CheckDateValid())
                {
                    ObjSupplierGenMedProductsPrice_ListPrice_Paging.PageIndex = 0;
                    SupplierGenMedProductsPrice_ListPrice_Paging(0, ObjSupplierGenMedProductsPrice_ListPrice_Paging.PageSize, true);
                }
            }
        }

        public void btFind()
        {
            if (CheckDateValid())
            {
                ObjSupplierGenMedProductsPrice_ListPrice_Paging.PageIndex = 0;
                SupplierGenMedProductsPrice_ListPrice_Paging(0, ObjSupplierGenMedProductsPrice_ListPrice_Paging.PageSize, true);
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

        public void hplAddNewPrice()
        {
            //var typeInfo = Globals.GetViewModel<ISupplierGenMedProductsPrice_AddEdit>();
            //typeInfo.ObjSupplierCurrent = ObjSupplierCurrent;
            //typeInfo.ObjDrugCurrent = ObjDrugCurrent;

            //typeInfo.TitleForm = string.Format(eHCMSResources.Z0663_G1_ThemGiaMoi0, ObjDrugCurrent.ObjRefGenMedProductDetails.BrandName.Trim());

            //typeInfo.InitializeNewItem();

            //var instance = typeInfo as Conductor<object>;

            //Globals.ShowDialog(instance, (o) =>
            //{
            //    //làm gì đó
            //});

            Action<ISupplierGenMedProductsPrice_AddEdit> onInitDlg = (typeInfo) =>
            {
                typeInfo.ObjSupplierCurrent = ObjSupplierCurrent;
                typeInfo.ObjDrugCurrent = ObjDrugCurrent;

                typeInfo.TitleForm = string.Format(eHCMSResources.Z0663_G1_ThemGiaMoi0, ObjDrugCurrent.ObjRefGenMedProductDetails.BrandName.Trim());

                typeInfo.InitializeNewItem();
            };

            GlobalsNAV.ShowDialog<ISupplierGenMedProductsPrice_AddEdit>(onInitDlg);

        }

        public void hplEditPrice_Click(object datacontext)
        {
            DataEntities.SupplierGenMedProductsPrice p = (datacontext as DataEntities.SupplierGenMedProductsPrice);
            
            if (p.PriceType == "PriceOld")
            {
                MessageBox.Show(string.Format("{0}!", eHCMSResources.Z0661_G1_GiaCuTrongQuaKhu), eHCMSResources.Z0660_G1_SuaGia, MessageBoxButton.OK);
            }
            else
            {
                //var typeInfo = Globals.GetViewModel<ISupplierGenMedProductsPrice_AddEdit>();
                //typeInfo.ObjSupplierCurrent = ObjSupplierCurrent;
                //typeInfo.ObjDrugCurrent = ObjDrugCurrent;
                
                //typeInfo.ObjSupplierGenMedProductsPrice_Current =ObjectCopier.DeepCopy(p);

                //typeInfo.TitleForm = string.Format(eHCMSResources.Z0664_G1_HChinhGia0, ObjDrugCurrent.ObjRefGenMedProductDetails.BrandName.Trim());

                //var instance = typeInfo as Conductor<object>;

                //Globals.ShowDialog(instance, (o) =>
                //{
                //    //làm gì đó
                //});

                Action<ISupplierGenMedProductsPrice_AddEdit> onInitDlg = (typeInfo) =>
                {
                    typeInfo.ObjSupplierCurrent = ObjSupplierCurrent;
                    typeInfo.ObjDrugCurrent = ObjDrugCurrent;

                    typeInfo.ObjSupplierGenMedProductsPrice_Current = ObjectCopier.DeepCopy(p);

                    typeInfo.TitleForm = string.Format(eHCMSResources.Z0664_G1_HChinhGia0, ObjDrugCurrent.ObjRefGenMedProductDetails.BrandName.Trim());
                };

                GlobalsNAV.ShowDialog<ISupplierGenMedProductsPrice_AddEdit>(onInitDlg);

            }
        }


        public void hplDeletePrice_Click(object datacontext)
        {
            DataEntities.SupplierGenMedProductsPrice p = (datacontext as DataEntities.SupplierGenMedProductsPrice);

            if (p.CanDelete.Value)
            {
                if (MessageBox.Show(eHCMSResources.A0159_G1_Msg_ConfXoaGia, eHCMSResources.G2617_G1_Xoa, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    SupplierGenMedProductsPrice_MarkDelete(p.PKID);
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
        private void SupplierGenMedProductsPrice_MarkDelete(Int64 PKID)
        {
             string Result = "";

             Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.Z0492_G1_DangXoa) });
             IsLoading = true;
             var t = new Thread(() =>
             {
                 using (var serviceFactory = new PharmacySuppliersServiceClient())
                 {
                     var contract = serviceFactory.ServiceInstance;

                     contract.BeginSupplierGenMedProductsPrice_MarkDelete(PKID, Globals.DispatchCallback((asyncResult) =>
                     {
                         try
                         {
                             contract.EndSupplierGenMedProductsPrice_MarkDelete(out Result, asyncResult);
                             if (Result == "Delete-1")
                             {
                                 Globals.EventAggregator.Publish(new DeleteEvent<bool>() { Result = true });
                                 
                                 ObjSupplierGenMedProductsPrice_ListPrice_Paging.PageIndex = 0;
                                 SupplierGenMedProductsPrice_ListPrice_Paging(0, ObjSupplierGenMedProductsPrice_ListPrice_Paging.PageSize, true);

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
       

        public void Handle(SaveEvent<bool> message)
        {
            if (message != null)
            {
                if (message.Result)
                {
                    ObjSupplierGenMedProductsPrice_ListPrice_Paging.PageIndex = 0;
                    SupplierGenMedProductsPrice_ListPrice_Paging(0,
                                                                 ObjSupplierGenMedProductsPrice_ListPrice_Paging.
                                                                     PageSize, true);
                }
            }
        }
    }
}
