using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.ServiceModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using aEMR.DataContracts;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using Caliburn.Micro;
using aEMR.Infrastructure;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using DataEntities;
using aEMR.Common;
using aEMR.Common.Collections;
using eHCMSLanguage;
using aEMR.Common.BaseModel;

namespace aEMR.Pharmacy.ViewModels
{
    [Export(typeof(ISupplierGenericDrugPrice_ListPrice)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class SupplierGenericDrugPrice_ListPriceViewModel : ViewModelBase, ISupplierGenericDrugPrice_ListPrice
        ,IHandle<SaveEvent<bool>>
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public SupplierGenericDrugPrice_ListPriceViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
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

            ObjSupplierGenericDrugPrice_ListPrice_Paging = new PagedSortableCollectionView<SupplierGenericDrugPrice>();
            ObjSupplierGenericDrugPrice_ListPrice_Paging.OnRefresh += new EventHandler<RefreshEventArgs>(ObjSupplierGenericDrugPrice_ListPrice_Paging_OnRefresh);
            if (CheckDateValid())
            {
                ObjSupplierGenericDrugPrice_ListPrice_Paging.PageIndex = 0;
                SupplierGenericDrugPrice_ListPrice_Paging(0, ObjSupplierGenericDrugPrice_ListPrice_Paging.PageSize, true);
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

        void ObjSupplierGenericDrugPrice_ListPrice_Paging_OnRefresh(object sender, RefreshEventArgs e)
        {
            if (CheckDateValid())
            {
                SupplierGenericDrugPrice_ListPrice_Paging(ObjSupplierGenericDrugPrice_ListPrice_Paging.PageIndex,ObjSupplierGenericDrugPrice_ListPrice_Paging.PageSize, false);
            }
        }
        public void authorization()
        {
            if (!Globals.isAccountCheck)
            {
                return;
            }
            bTim = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPharmacy
                                               , (int)ePharmacy.mQuanLyGiaTuNCC,
                                               (int)oPharmacyEx.mQuanLyGiaTuNCC_Tim, (int)ePermission.mView);
            bThem = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPharmacy
                                               , (int)ePharmacy.mQuanLyGiaTuNCC,
                                               (int)oPharmacyEx.mQuanLyGiaTuNCC_Them, (int)ePermission.mView);
            bChinhSua = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPharmacy
                                               , (int)ePharmacy.mQuanLyGiaTuNCC,
                                               (int)oPharmacyEx.mQuanLyGiaTuNCC_ChinhSua, (int)ePermission.mView);
            
        }
        #region checking account

        private bool _bTim = true;
        private bool _bThem = true;
        private bool _bChinhSua = true;

        public bool bTim
        {
            get
            {
                return _bTim;
            }
            set
            {
                if (_bTim == value)
                    return;
                _bTim = value;
            }
        }
        public bool bThem
        {
            get
            {
                return _bThem;
            }
            set
            {
                if (_bThem == value)
                    return;
                _bThem = value;
            }
        }
        public bool bChinhSua
        {
            get
            {
                return _bChinhSua;
            }
            set
            {
                if (_bChinhSua == value)
                    return;
                _bChinhSua = value;
            }
        }

        #endregion
        #region binding visibilty

        public Button hplEditPrice { get; set; }
        public Button hplDeletePrice { get; set; }
        
        public void hplEditPrice_Loaded(object sender)
        {
            hplEditPrice = sender as Button;
            hplEditPrice.Visibility = Globals.convertVisibility(bChinhSua);
        }
        public void hplDeletePrice_Loaded(object sender)
        {
            hplDeletePrice = sender as Button;
            hplDeletePrice.Visibility = Globals.convertVisibility(bChinhSua);
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

        private SupplierGenericDrugPrice _ObjDrugCurrent;
        public SupplierGenericDrugPrice ObjDrugCurrent
        {
            get { return _ObjDrugCurrent; }
            set
            {
                _ObjDrugCurrent = value;
                NotifyOfPropertyChange(() => ObjDrugCurrent);
            }
        }

        private SupplierGenericDrugPriceSearchCriteria _Criteria;
        public SupplierGenericDrugPriceSearchCriteria Criteria
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


        private PagedSortableCollectionView<DataEntities.SupplierGenericDrugPrice> _ObjSupplierGenericDrugPrice_ListPrice_Paging;
        public PagedSortableCollectionView<DataEntities.SupplierGenericDrugPrice> ObjSupplierGenericDrugPrice_ListPrice_Paging
        {
            get { return _ObjSupplierGenericDrugPrice_ListPrice_Paging; }
            set
            {
                _ObjSupplierGenericDrugPrice_ListPrice_Paging = value;
                NotifyOfPropertyChange(() => ObjSupplierGenericDrugPrice_ListPrice_Paging);
            }
        }

        private void SupplierGenericDrugPrice_ListPrice_Paging(int PageIndex, int PageSize, bool CountTotal)
        {
            //Globals.EventAggregator.Publish(new BusyEvent() { IsBusy = true, Message = string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.K2978_G1_DSGia) });
            this.DlgShowBusyIndicator(string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.K2978_G1_DSGia));
            IsLoading = true;
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacySuppliersServiceClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginSupplierGenericDrugPrice_ListPrice_Paging(Criteria, PageIndex, PageSize, "", CountTotal, Globals.DispatchCallback((asyncResult) =>
                        {
                            int Total = 0;
                            IList<DataEntities.SupplierGenericDrugPrice> allItems = null;
                            bool bOK = false;
                            try
                            {
                                allItems = client.EndSupplierGenericDrugPrice_ListPrice_Paging(out Total, asyncResult);
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
                                this.DlgHideBusyIndicator();
                            }
                            ObjSupplierGenericDrugPrice_ListPrice_Paging.Clear();

                            if (bOK)
                            {
                                if (CountTotal)
                                {
                                    ObjSupplierGenericDrugPrice_ListPrice_Paging.TotalItemCount = Total;
                                }
                                if (allItems != null)
                                {
                                    foreach (var item in allItems)
                                    {
                                        ObjSupplierGenericDrugPrice_ListPrice_Paging.Add(item);
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
                    this.DlgHideBusyIndicator();
                }
                finally
                {
                    IsLoading = false;
                    //Globals.IsBusy = false;
                }
            });
            t.Start();
        }

        public void dtgList_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            DataEntities.SupplierGenericDrugPrice objRows = e.Row.DataContext as DataEntities.SupplierGenericDrugPrice;
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
                    ObjSupplierGenericDrugPrice_ListPrice_Paging.PageIndex = 0;
                    SupplierGenericDrugPrice_ListPrice_Paging(0, ObjSupplierGenericDrugPrice_ListPrice_Paging.PageSize, true);
                }
            }
        }

        public void btFind()
        {
            if (CheckDateValid())
            {
                ObjSupplierGenericDrugPrice_ListPrice_Paging.PageIndex = 0;
                SupplierGenericDrugPrice_ListPrice_Paging(0, ObjSupplierGenericDrugPrice_ListPrice_Paging.PageSize, true);
            }
        }

        private bool CheckDateValid()
        {
            if (Criteria != null && Criteria.FromDate != null && Criteria.ToDate != null)
                {
                    if (Criteria.FromDate > Criteria.ToDate)
                    {
                        MessageBox.Show(eHCMSResources.Z0467_G1_TuNgNhoHonBangDenNg, eHCMSResources.G0981_G1_Tim, MessageBoxButton.OK);
                        return false;
                    }
                }
            return true;
        }

        public void btClose()
        {
            TryClose();
        }

        public void hplAddNewPrice()
        {
            //var typeInfo = Globals.GetViewModel<ISupplierGenericDrugPrice_AddEdit>();
            //typeInfo.ObjSupplierCurrent = ObjSupplierCurrent;
            //typeInfo.ObjDrugCurrent = ObjDrugCurrent;

            //typeInfo.TitleForm = string.Format(eHCMSResources.Z0663_G1_ThemGiaMoi0, ObjDrugCurrent.ObjRefGenericDrugDetail.BrandName.Trim());

            //typeInfo.InitializeNewItem();

            //var instance = typeInfo as Conductor<object>;

            //Globals.ShowDialog(instance, (o) =>
            //{
            //    //làm gì đó
            //});

            Action<ISupplierGenericDrugPrice_AddEdit> onInitDlg = (typeInfo) =>
            {
                typeInfo.ObjSupplierCurrent = ObjSupplierCurrent;
                typeInfo.ObjDrugCurrent = ObjDrugCurrent;

                typeInfo.TitleForm = string.Format(eHCMSResources.Z0663_G1_ThemGiaMoi0, ObjDrugCurrent.ObjRefGenericDrugDetail.BrandName.Trim());

                typeInfo.InitializeNewItem();
            };
            GlobalsNAV.ShowDialog<ISupplierGenericDrugPrice_AddEdit>(onInitDlg);

        }

        public void hplEditPrice_Click(object datacontext)
        {
            DataEntities.SupplierGenericDrugPrice p = (datacontext as DataEntities.SupplierGenericDrugPrice);

            if (p.PriceType == "PriceOld")
            {
                MessageBox.Show(eHCMSResources.Z0661_G1_GiaCuTrongQuaKhu, eHCMSResources.Z0660_G1_SuaGia, MessageBoxButton.OK);
            }
            else
            {
                //var typeInfo = Globals.GetViewModel<ISupplierGenericDrugPrice_AddEdit>();
                //typeInfo.ObjSupplierCurrent = ObjSupplierCurrent;
                //typeInfo.ObjDrugCurrent = ObjDrugCurrent;

                //typeInfo.ObjSupplierGenericDrugPrice_Current = ObjectCopier.DeepCopy(p);

                //typeInfo.TitleForm = string.Format(eHCMSResources.Z0664_G1_HChinhGia0, ObjDrugCurrent.ObjRefGenericDrugDetail.BrandName.Trim());

                //var instance = typeInfo as Conductor<object>;

                //Globals.ShowDialog(instance, (o) =>
                //{
                //    //làm gì đó
                //});

                Action<ISupplierGenericDrugPrice_AddEdit> onInitDlg = (typeInfo) =>
                {
                    typeInfo.ObjSupplierCurrent = ObjSupplierCurrent;
                    typeInfo.ObjDrugCurrent = ObjDrugCurrent;

                    typeInfo.ObjSupplierGenericDrugPrice_Current = ObjectCopier.DeepCopy(p);

                    typeInfo.TitleForm = string.Format(eHCMSResources.Z0664_G1_HChinhGia0, ObjDrugCurrent.ObjRefGenericDrugDetail.BrandName.Trim());
                };
                GlobalsNAV.ShowDialog<ISupplierGenericDrugPrice_AddEdit>(onInitDlg);

            }
        }

        public void DoubleClick(object sender, Common.EventArgs<object> e)
        {
            SupplierGenericDrugPrice p = (e.Value as SupplierGenericDrugPrice).DeepCopy();
            if (p.PriceType == "PriceOld")
            {
                MessageBox.Show(eHCMSResources.Z0661_G1_GiaCuTrongQuaKhu, eHCMSResources.Z0660_G1_SuaGia, MessageBoxButton.OK);
            }
            else
            {
                //var typeInfo = Globals.GetViewModel<ISupplierGenericDrugPrice_AddEdit>();
                //typeInfo.ObjSupplierCurrent = ObjSupplierCurrent;
                //typeInfo.ObjDrugCurrent = ObjDrugCurrent;

                //typeInfo.ObjSupplierGenericDrugPrice_Current = ObjectCopier.DeepCopy(p);

                //typeInfo.TitleForm = string.Format(eHCMSResources.Z0664_G1_HChinhGia0, ObjDrugCurrent.ObjRefGenericDrugDetail.BrandName.Trim());

                //var instance = typeInfo as Conductor<object>;

                //Globals.ShowDialog(instance, (o) =>
                //{
                //    //làm gì đó
                //});

                Action<ISupplierGenericDrugPrice_AddEdit> onInitDlg = (typeInfo) =>
                {
                    typeInfo.ObjSupplierCurrent = ObjSupplierCurrent;
                    typeInfo.ObjDrugCurrent = ObjDrugCurrent;

                    typeInfo.ObjSupplierGenericDrugPrice_Current = ObjectCopier.DeepCopy(p);

                    typeInfo.TitleForm = string.Format(eHCMSResources.Z0664_G1_HChinhGia0, ObjDrugCurrent.ObjRefGenericDrugDetail.BrandName.Trim());
                };
                GlobalsNAV.ShowDialog<ISupplierGenericDrugPrice_AddEdit>(onInitDlg);

            }
        }

        public void hplDeletePrice_Click(object datacontext)
        {
            DataEntities.SupplierGenericDrugPrice p = (datacontext as DataEntities.SupplierGenericDrugPrice);

            if (p.CanDelete.Value)
            {
                if (MessageBox.Show(eHCMSResources.A0159_G1_Msg_ConfXoaGia, eHCMSResources.G2617_G1_Xoa, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    SupplierGenericDrugPrice_MarkDelete(p.PKID);
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
        private void SupplierGenericDrugPrice_MarkDelete(Int64 PKID)
        {
             string Result = "";

             Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.Z0492_G1_DangXoa) });
             IsLoading = true;
             var t = new Thread(() =>
             {
                 using (var serviceFactory = new PharmacySuppliersServiceClient())
                 {
                     var contract = serviceFactory.ServiceInstance;

                     contract.BeginSupplierGenericDrugPrice_MarkDelete(PKID, Globals.DispatchCallback((asyncResult) =>
                     {
                         try
                         {
                             contract.EndSupplierGenericDrugPrice_MarkDelete(out Result, asyncResult);
                             if (Result == "Delete-1")
                             {
                                 Globals.EventAggregator.Publish(new DeleteEvent<bool>() { Result = true });
                                 
                                 ObjSupplierGenericDrugPrice_ListPrice_Paging.PageIndex = 0;
                                 SupplierGenericDrugPrice_ListPrice_Paging(0, ObjSupplierGenericDrugPrice_ListPrice_Paging.PageSize, true);

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
                    ObjSupplierGenericDrugPrice_ListPrice_Paging.PageIndex = 0;
                    SupplierGenericDrugPrice_ListPrice_Paging(0,
                                                                 ObjSupplierGenericDrugPrice_ListPrice_Paging.
                                                                     PageSize, true);
                    //Ny tao su kien nay
                    Globals.EventAggregator.Publish(new ClosePriceEvent { });
                }
            }
        }
    }
}
