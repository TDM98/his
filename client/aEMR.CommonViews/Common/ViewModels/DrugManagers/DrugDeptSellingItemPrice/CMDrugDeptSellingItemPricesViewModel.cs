using eHCMSLanguage;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ServiceModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Caliburn.Micro;
using DataEntities;
using aEMR.ViewContracts;
using aEMR.Infrastructure.Events;
using aEMR.Common.Collections;
using Castle.Windsor;
using aEMR.Infrastructure.CachingUtils;
using aEMR.Infrastructure;
using aEMR.ServiceClient;
using aEMR.DataContracts;

namespace aEMR.Common.ViewModels
{
    [Export(typeof(ICMDrugDeptSellingItemPrices)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class CMDrugDeptSellingItemPricesViewModel : Conductor<object>, ICMDrugDeptSellingItemPrices
        , IHandle<ReLoadDataAfterCUD>
    {
        [ImportingConstructor]
        public CMDrugDeptSellingItemPricesViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg, ISalePosCaching salePosCaching)
        {
            eventArg.Subscribe(this);
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

        private bool _IsStore;
        public bool IsStore
        {
            get
            {
                return _IsStore;
            }
            set
            {
                if (_IsStore == value)
                    return;
                _IsStore = value;
                NotifyOfPropertyChange(() => IsStore);
                if(IsStore)
                {
                    isEdit = false;
                }
            }
        }

        private bool _isEdit=true;
        public bool isEdit
        {
            get
            {
                return _isEdit;
            }
            set
            {
                if (_isEdit == value)
                    return;
                _isEdit = value;
                NotifyOfPropertyChange(()=>isEdit);
            }
        }

        private bool _mTaoGiaMoi = true;
        public bool mTaoGiaMoi
        {
            get
            {
                //isEdit = isEdit && _mTaoGiaMoi;
                return _mTaoGiaMoi;
            }
            set
            {
                if (_mTaoGiaMoi == value)
                    return;
                _mTaoGiaMoi = value&& isEdit;
                

                NotifyOfPropertyChange(() => mTaoGiaMoi);
            }
        }

        private bool _mChinhSuaGia = true;
        public bool mChinhSuaGia
        {
            get
            {
                return _mChinhSuaGia;
            }
            set
            {
                if (_mChinhSuaGia == value)
                    return;
                _mChinhSuaGia = value;
                NotifyOfPropertyChange(() => mChinhSuaGia);
            }
        }
        protected override void OnActivate()
        {
            base.OnActivate();
            ObjDrugDeptSellingItemPrices_ByDrugID_Paging = new PagedSortableCollectionView<DrugDeptSellingItemPrices>();
            ObjDrugDeptSellingItemPrices_ByDrugID_Paging.OnRefresh += new EventHandler<RefreshEventArgs>(ObjDrugDeptSellingItemPrices_ByDrugID_Paging_OnRefresh);
            if (CheckDateValid())
            {
                ObjDrugDeptSellingItemPrices_ByDrugID_Paging.PageIndex = 0;
                DrugDeptSellingItemPrices_ByDrugID_Paging(0, ObjDrugDeptSellingItemPrices_ByDrugID_Paging.PageSize, true);
            }
        }
        void ObjDrugDeptSellingItemPrices_ByDrugID_Paging_OnRefresh(object sender, RefreshEventArgs e)
        {
            if (CheckDateValid())
            {
                DrugDeptSellingItemPrices_ByDrugID_Paging(ObjDrugDeptSellingItemPrices_ByDrugID_Paging.PageIndex,
                                ObjDrugDeptSellingItemPrices_ByDrugID_Paging.PageSize, false);
            }
        }
        private DrugDeptSellingItemPricesSearchCriteria _SearchCriteria;
        public DrugDeptSellingItemPricesSearchCriteria SearchCriteria
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

        private RefGenMedProductDetails _ObjDrug_Current;
        public RefGenMedProductDetails ObjDrug_Current
        {
            get { return _ObjDrug_Current; }
            set
            {
                _ObjDrug_Current = value;
                NotifyOfPropertyChange(() => ObjDrug_Current);
            }
        }

        private PagedSortableCollectionView<DrugDeptSellingItemPrices> _ObjDrugDeptSellingItemPrices_ByDrugID_Paging;
        public PagedSortableCollectionView<DrugDeptSellingItemPrices> ObjDrugDeptSellingItemPrices_ByDrugID_Paging
        {
            get { return _ObjDrugDeptSellingItemPrices_ByDrugID_Paging; }
            set
            {
                _ObjDrugDeptSellingItemPrices_ByDrugID_Paging = value;
                NotifyOfPropertyChange(() => ObjDrugDeptSellingItemPrices_ByDrugID_Paging);
            }
        }
        private void DrugDeptSellingItemPrices_ByDrugID_Paging(int PageIndex, int PageSize, bool CountTotal)
        {
            Globals.EventAggregator.Publish(new BusyEvent() { IsBusy = true, Message = string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.K2978_G1_DSGia) });
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyDrugServiceClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginDrugDeptSellingItemPrices_ByDrugID_Paging(SearchCriteria, PageIndex, PageSize, "", CountTotal, Globals.DispatchCallback((asyncResult) =>
                        {
                            int Total = 0;
                            IList<DrugDeptSellingItemPrices> allItems = null;
                            bool bOK = false;
                            try
                            {
                                allItems = client.EndDrugDeptSellingItemPrices_ByDrugID_Paging(out Total, asyncResult);
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

                            ObjDrugDeptSellingItemPrices_ByDrugID_Paging.Clear();
                            if (bOK)
                            {
                                if (CountTotal)
                                {
                                    ObjDrugDeptSellingItemPrices_ByDrugID_Paging.TotalItemCount = Total;
                                }
                                if (allItems != null)
                                {
                                    foreach (var item in allItems)
                                    {
                                        ObjDrugDeptSellingItemPrices_ByDrugID_Paging.Add(item);
                                    }
                                    //Do danh sách RefGenMedProductDetails cho xem giá hiện hành cho tiện, nên kết qua Giá. Khi vô Danh Sách Giá sẽ bị dòng rỗng nên check chỗ này
                                    if (ObjDrugDeptSellingItemPrices_ByDrugID_Paging.Count > 0)
                                    {
                                        if (ObjDrugDeptSellingItemPrices_ByDrugID_Paging[0].DrugDeptSellingItemPriceID <= 0)
                                        {
                                            ObjDrugDeptSellingItemPrices_ByDrugID_Paging.Clear();
                                        }
                                    }
                                    //Do danh sách RefGenMedProductDetails cho xem giá hiện hành cho tiện, nên kết qua Giá. Khi vô Danh Sách Giá sẽ bị dòng rỗng nên check chỗ này
                                }
                            }
                            else
                            {
                                //Do danh sách RefGenMedProductDetails cho xem giá hiện hành cho tiện, nên kết qua Giá. Khi vô Danh Sách Giá sẽ bị dòng rỗng nên check chỗ này
                                if (ObjDrugDeptSellingItemPrices_ByDrugID_Paging.Count > 0)
                                {
                                    if (ObjDrugDeptSellingItemPrices_ByDrugID_Paging[0].DrugDeptSellingItemPriceID <= 0)
                                    {
                                        ObjDrugDeptSellingItemPrices_ByDrugID_Paging.Clear();
                                    }
                                }
                                //Do danh sách RefGenMedProductDetails cho xem giá hiện hành cho tiện, nên kết qua Giá. Khi vô Danh Sách Giá sẽ bị dòng rỗng nên check chỗ này
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
                    Globals.IsBusy = false;
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
                    ObjDrugDeptSellingItemPrices_ByDrugID_Paging.PageIndex = 0;
                    DrugDeptSellingItemPrices_ByDrugID_Paging(0, ObjDrugDeptSellingItemPrices_ByDrugID_Paging.PageSize, true);
                }
            }
        }
        public void btFind()
        {
            if (CheckDateValid())
            {
                ObjDrugDeptSellingItemPrices_ByDrugID_Paging.PageIndex = 0;
                DrugDeptSellingItemPrices_ByDrugID_Paging(0, ObjDrugDeptSellingItemPrices_ByDrugID_Paging.PageSize, true);
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
                        MessageBox.Show(eHCMSResources.Z0467_G1_TuNgNhoHonBangDenNg, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                        return false;
                    }
                    return true;
                }
                else
                {
                    MessageBox.Show(eHCMSResources.A0885_G1_Msg_InfoNhapTuNgDenNg2, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
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

        #region binding visibilty

        public void spEdit_Loaded(object sender,RoutedEvent e)
        {
            ((StackPanel)sender).Visibility = Globals.convertVisibility(isEdit && mChinhSuaGia);
        }
        
        
        #endregion

        public void hplAddNewPrice()
        {
            Action<ICMDrugDeptSellingItemPrices_AddEdit> onInitDlg = delegate (ICMDrugDeptSellingItemPrices_AddEdit typeInfo)
            {
                typeInfo.ObjDrug_Current = ObjDrug_Current;
                typeInfo.TitleForm = string.Format(eHCMSResources.Z0663_G1_ThemGiaMoi0, ObjDrug_Current.BrandName.Trim());
                typeInfo.InitializeNewItem();
            };
            GlobalsNAV.ShowDialog<ICMDrugDeptSellingItemPrices_AddEdit>(onInitDlg);
        }
        public void hplEditPrice_Click(object datacontext)
        {
            DrugDeptSellingItemPrices p = (datacontext as DrugDeptSellingItemPrices);

            if (p.PriceType == "PriceOld")
            {
                MessageBox.Show(eHCMSResources.Z0661_G1_GiaCuTrongQuaKhu, eHCMSResources.Z0660_G1_SuaGia, MessageBoxButton.OK);
            }
            else
            {
                Action<ICMDrugDeptSellingItemPrices_AddEdit> onInitDlg = delegate (ICMDrugDeptSellingItemPrices_AddEdit typeInfo)
                {
                    typeInfo.ObjDrug_Current = ObjDrug_Current;
                    typeInfo.TitleForm = string.Format(eHCMSResources.Z0664_G1_HChinhGia0, p.BrandName.Trim());
                    typeInfo.ObjDrugDeptSellingItemPrices_Current = ObjectCopier.DeepCopy(p);
                };
                GlobalsNAV.ShowDialog<ICMDrugDeptSellingItemPrices_AddEdit>(onInitDlg);
            }
        }
        public void hplDeletePrice_Click(object datacontext)
        {
            DrugDeptSellingItemPrices p = (datacontext as DrugDeptSellingItemPrices);

            if (p.CanDelete.Value)
            {
                if (MessageBox.Show(eHCMSResources.A0159_G1_Msg_ConfXoaGia, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    DrugDeptSellingItemPricess_MarkDelete(p.DrugDeptSellingItemPriceID);
                }
            }
            else
            {
                if (p.PriceType == "PriceCurrent")
                {
                    MessageBox.Show(eHCMSResources.A0552_G1_Msg_InfoKhDcXoaGiaDangApDung, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                }
            }
        }
        private void DrugDeptSellingItemPricess_MarkDelete(Int64 DrugDeptSellingItemPricesID)
        {
            string Result = "";

            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.Z0492_G1_DangXoa) });

            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyDrugServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginDrugDeptSellingItemPrices_MarkDelete(DrugDeptSellingItemPricesID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            contract.EndDrugDeptSellingItemPrices_MarkDelete(out Result, asyncResult);
                            if (Result == "Delete-1")
                            {
                                ObjDrugDeptSellingItemPrices_ByDrugID_Paging.PageIndex = 0;
                                DrugDeptSellingItemPrices_ByDrugID_Paging(0, ObjDrugDeptSellingItemPrices_ByDrugID_Paging.PageSize, true);
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
                            Globals.IsBusy = false;
                        }
                    }), null);
                }


            });
            t.Start();
        }
        public void Handle(ReLoadDataAfterCUD message)
        {
            if (message != null)
            {
                ObjDrugDeptSellingItemPrices_ByDrugID_Paging.PageIndex = 0;
                DrugDeptSellingItemPrices_ByDrugID_Paging(0, ObjDrugDeptSellingItemPrices_ByDrugID_Paging.PageSize, true);
            }
        }
        public void dtgList_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            DrugDeptSellingItemPrices objRows = e.Row.DataContext as DrugDeptSellingItemPrices;
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
    }
}