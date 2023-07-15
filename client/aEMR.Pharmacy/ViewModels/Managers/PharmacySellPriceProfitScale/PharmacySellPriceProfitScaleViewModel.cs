using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ServiceModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using aEMR.Common;
using aEMR.DataContracts;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using Caliburn.Micro;
using DataEntities;
using aEMR.Common.Collections;
using eHCMSLanguage;
using aEMR.Common.BaseModel;

namespace aEMR.Pharmacy.ViewModels
{
    [Export(typeof(IPharmacySellPriceProfitScale)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class PharmacySellPriceProfitScaleViewModel : ViewModelBase, IPharmacySellPriceProfitScale
        , IHandle<SaveEvent<bool>>
    {
        public string TitleForm { get; set; }

        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public PharmacySellPriceProfitScaleViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            eventAggregator.Subscribe(this);

            authorization();
            ObjPharmacySellPriceProfitScale_GetList_Paging=new PagedSortableCollectionView<PharmacySellPriceProfitScale>();
            ObjPharmacySellPriceProfitScale_GetList_Paging.OnRefresh += new EventHandler<RefreshEventArgs>(ObjPharmacySellPriceProfitScale_GetList_Paging_OnRefresh);

            IsActive = true;

            ObjPharmacySellPriceProfitScale_GetList_Paging.PageIndex = 0;
            PharmacySellPriceProfitScale_GetList_Paging(0, ObjPharmacySellPriceProfitScale_GetList_Paging.PageSize, true);

        }

        private bool _IsActive;
        public new bool IsActive
        {
            get { return _IsActive; }
            set
            {
                if(_IsActive!=value)
                {
                    _IsActive = value;
                    NotifyOfPropertyChange(()=>IsActive);
                }
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
            bTim = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPharmacy
                                               , (int)ePharmacy.mQuanLyNhaCungCap,
                                               (int)oPharmacyEx.mQuanLyNhaCungCap_Tim, (int)ePermission.mView);
            bThem = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPharmacy
                                               , (int)ePharmacy.mQuanLyNhaCungCap,
                                               (int)oPharmacyEx.mQuanLyNhaCungCap_Them, (int)ePermission.mView);
            bChinhSua = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPharmacy
                                               , (int)ePharmacy.mQuanLyNhaCungCap,
                                               (int)oPharmacyEx.mQuanLyNhaCungCap_ChinhSua, (int)ePermission.mView);
            bInMau = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPharmacy
                                               , (int)ePharmacy.mQuanLyNhaCungCap,
                                               (int)oPharmacyEx.mQuanLyNhaCungCap_InMau, (int)ePermission.mView);


        }

        #region checking account

        private bool _bThem = true;
        private bool _bChinhSua = true;
        private bool _bTim = true;
        private bool _bInMau = true;
        
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
        public bool bInMau
        {
            get
            {
                return _bInMau;
            }
            set
            {
                if (_bInMau == value)
                    return;
                _bInMau = value;
            }
        }
        
        #endregion
        #region binding visibilty

        public Button lnkDelete { get; set; }
        public Button lnkView { get; set; }

        public void lnkDelete_Loaded(object sender)
        {
            lnkDelete = sender as Button;
            if (lnkDelete != null)
            {
                lnkDelete.Visibility = Globals.convertVisibility(bChinhSua);
            }
        }
        public void lnkView_Loaded(object sender)
        {
            if (lnkView != null)
            {
                lnkView = sender as Button;
                lnkView.Visibility = Globals.convertVisibility(bChinhSua);
            }
        }
        #endregion

        void ObjPharmacySellPriceProfitScale_GetList_Paging_OnRefresh(object sender, RefreshEventArgs e)
        {
            PharmacySellPriceProfitScale_GetList_Paging(ObjPharmacySellPriceProfitScale_GetList_Paging.PageIndex, ObjPharmacySellPriceProfitScale_GetList_Paging.PageSize, false);
        }

        private PagedSortableCollectionView<PharmacySellPriceProfitScale> _ObjPharmacySellPriceProfitScale_GetList_Paging;
        public PagedSortableCollectionView<PharmacySellPriceProfitScale> ObjPharmacySellPriceProfitScale_GetList_Paging
        {
            get { return _ObjPharmacySellPriceProfitScale_GetList_Paging; }
            set
            {
                _ObjPharmacySellPriceProfitScale_GetList_Paging = value;
                NotifyOfPropertyChange(() => ObjPharmacySellPriceProfitScale_GetList_Paging);
            }
        }

        private void PharmacySellPriceProfitScale_GetList_Paging(int PageIndex, int PageSize, bool CountTotal)
        {
            ObjPharmacySellPriceProfitScale_GetList_Paging.Clear();

            Globals.EventAggregator.Publish(new BusyEvent() { IsBusy = true, Message = string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.Z0715_DSCgThuc) });
            IsLoading = true;
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyDrugServiceClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginPharmacySellPriceProfitScale_GetList_Paging(IsActive, PageIndex, PageSize, "", CountTotal, Globals.DispatchCallback((asyncResult) =>
                        {
                            int Total = 0;
                            IList<PharmacySellPriceProfitScale> allItems = null;
                            bool bOK = false;
                            try
                            {
                                allItems = client.EndPharmacySellPriceProfitScale_GetList_Paging(out Total, asyncResult);
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

                            if (bOK)
                            {
                                if (CountTotal)
                                {
                                    ObjPharmacySellPriceProfitScale_GetList_Paging.TotalItemCount = Total;
                                }
                                if (allItems != null)
                                {
                                    foreach (var item in allItems)
                                    {
                                        ObjPharmacySellPriceProfitScale_GetList_Paging.Add(item);
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

        public void hplDelete_Click(object selectedItem)
        {
            PharmacySellPriceProfitScale p = (selectedItem as PharmacySellPriceProfitScale);

            if (p != null && p.PharmacySellPriceProfitScaleID > 0)
            {
                if (MessageBox.Show(string.Format("{0}: {1}?", eHCMSResources.A0149_G1_Msg_ConfTatKichHoatCThuc, p.RecCreatedDate.ToString("dd/MM/yyyy")), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    PharmacySellPriceProfitScale_IsActive(p.PharmacySellPriceProfitScaleID,false);
                }
            }
        }

        private void PharmacySellPriceProfitScale_IsActive(Int64 PharmacySellPriceProfitScaleID, Boolean IsActive)
        {
            string Result = "";

            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.K2887_G1_DangXuLy });
            IsLoading = true;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyDrugServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginPharmacySellPriceProfitScale_IsActive(PharmacySellPriceProfitScaleID,IsActive, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            contract.EndPharmacySellPriceProfitScale_IsActive(out Result, asyncResult);
                            switch (Result)
                            {
                                case "0":
                                    {
                                        MessageBox.Show(eHCMSResources.Z0363_G1_KgThanhCong, eHCMSResources.G2617_G1_Xoa, MessageBoxButton.OK);
                                        break;
                                    }
                                case "1":
                                    {
                                        ObjPharmacySellPriceProfitScale_GetList_Paging.PageIndex = 0;
                                        PharmacySellPriceProfitScale_GetList_Paging(0, ObjPharmacySellPriceProfitScale_GetList_Paging.PageSize, true);
                                        MessageBox.Show(eHCMSResources.A0473_G1_Msg_InfoDaTatHieuLuc, eHCMSResources.G2617_G1_Xoa, MessageBoxButton.OK);
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
                    ObjPharmacySellPriceProfitScale_GetList_Paging.PageIndex = 0;
                    PharmacySellPriceProfitScale_GetList_Paging(0, ObjPharmacySellPriceProfitScale_GetList_Paging.PageSize, true);
                }
            }
        }


        public void hplAddNew_Click()
        {
            //var typeInfo = Globals.GetViewModel<IPharmacySellPriceProfitScaleAddEdit>();
            //typeInfo.TitleForm = eHCMSResources.G0276_G1_ThemMoi;

            //typeInfo.InitializeNewItem();
            //typeInfo.FormLoad();

            //var instance = typeInfo as Conductor<object>;

            //Globals.ShowDialog(instance, (o) =>
            //{
            //    //làm gì đó
            //});

            Action<IPharmacySellPriceProfitScaleAddEdit> onInitDlg = (typeInfo) =>
            {
                typeInfo.TitleForm = eHCMSResources.G0276_G1_ThemMoi;

                typeInfo.InitializeNewItem();
                typeInfo.FormLoad();
            };
            GlobalsNAV.ShowDialog<IPharmacySellPriceProfitScaleAddEdit>(onInitDlg);
        }


        public void hplEdit_Click(object selectedItem)
        {
            if (selectedItem != null)
            {
                //var typeInfo = Globals.GetViewModel<IPharmacySellPriceProfitScaleAddEdit>();

                //typeInfo.TitleForm = eHCMSResources.T1484_G1_HChinh;

                //typeInfo.ObjPharmacySellPriceProfitScale_Current = ObjectCopier.DeepCopy((selectedItem as DataEntities.PharmacySellPriceProfitScale));
                //typeInfo.FormLoad();

                //var instance = typeInfo as Conductor<object>;

                //Globals.ShowDialog(instance, (o) =>
                //{
                //    //làm gì đó
                //});

                Action<IPharmacySellPriceProfitScaleAddEdit> onInitDlg = (typeInfo) =>
                {
                    typeInfo.ObjPharmacySellPriceProfitScale_Current = ObjectCopier.DeepCopy((selectedItem as DataEntities.PharmacySellPriceProfitScale));
                    typeInfo.FormLoad();
                };
                GlobalsNAV.ShowDialog<IPharmacySellPriceProfitScaleAddEdit>(onInitDlg);
            }
        }

        public void DoubleClick(object sender, Common.EventArgs<object> e)
        {
            PharmacySellPriceProfitScale item = (e.Value as PharmacySellPriceProfitScale).DeepCopy();
            if (item != null)
            {
                //var typeInfo = Globals.GetViewModel<IPharmacySellPriceProfitScaleAddEdit>();

                //typeInfo.TitleForm = eHCMSResources.T1484_G1_HChinh;

                //typeInfo.ObjPharmacySellPriceProfitScale_Current = item;
                //typeInfo.FormLoad();

                //var instance = typeInfo as Conductor<object>;

                //Globals.ShowDialog(instance, (o) =>
                //{
                //    //làm gì đó
                //});

                Action<IPharmacySellPriceProfitScaleAddEdit> onInitDlg = (typeInfo) =>
                {
                    typeInfo.TitleForm = eHCMSResources.T1484_G1_HChinh;

                    typeInfo.ObjPharmacySellPriceProfitScale_Current = item;
                    typeInfo.FormLoad();
                };
                GlobalsNAV.ShowDialog<IPharmacySellPriceProfitScaleAddEdit>(onInitDlg);
            }
        }

        public void chkIsActive_Click(object sender, RoutedEventArgs e)
        {
            ObjPharmacySellPriceProfitScale_GetList_Paging.PageIndex = 0;
            PharmacySellPriceProfitScale_GetList_Paging(0, ObjPharmacySellPriceProfitScale_GetList_Paging.PageSize, true);            
        }

    }
}
