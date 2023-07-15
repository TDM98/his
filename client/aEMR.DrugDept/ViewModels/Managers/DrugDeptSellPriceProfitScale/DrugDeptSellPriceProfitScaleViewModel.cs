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
using aEMR.ViewContracts;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using Caliburn.Micro;
using DataEntities;
using aEMR.Common;
using aEMR.Common.Collections;
using eHCMSLanguage;
using System.Windows.Controls;

namespace aEMR.DrugDept.ViewModels
{
    [Export(typeof(IDrugDeptSellPriceProfitScale)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class DrugDeptSellPriceProfitScaleViewModel : Conductor<object>, IDrugDeptSellPriceProfitScale
        , IHandle<SaveEvent<bool>>
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

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

        [ImportingConstructor]
        public DrugDeptSellPriceProfitScaleViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            eventAggregator.Subscribe(this);
            //Globals.EventAggregator.Subscribe(this);
        }

        public void Init()
        {
            ObjDrugDeptSellPriceProfitScale_GetList_Paging = new PagedSortableCollectionView<DrugDeptSellPriceProfitScale>();
            //20180705 TBL: Anh Tuan noi DataServicePagedCollectionView hoat dong khong tot nen comment ra
            //ObjDrugDeptSellPriceProfitScale_GetList_Paging.OnRefresh += new EventHandler<RefreshEventArgs>(ObjDrugDeptSellPriceProfitScale_GetList_Paging_OnRefresh);

            IsActive = true;

            ObjDrugDeptSellPriceProfitScale_GetList_Paging.PageIndex = 0;
            DrugDeptSellPriceProfitScale_GetList_Paging(0, ObjDrugDeptSellPriceProfitScale_GetList_Paging.PageSize, true);
        }

        #region check invisible

        private bool _mTim = true;
        private bool _mTaoMoiCTGia = true;
        private bool _mChinhSua = true;

        public bool mTim
        {
            get
            {
                return _mTim;
            }
            set
            {
                if (_mTim == value)
                    return;
                _mTim = value;
                NotifyOfPropertyChange(() => mTim);
            }
        }
        public bool mTaoMoiCTGia
        {
            get
            {
                return _mTaoMoiCTGia;
            }
            set
            {
                if (_mTaoMoiCTGia == value)
                    return;
                _mTaoMoiCTGia = value;
                NotifyOfPropertyChange(() => mTaoMoiCTGia);
            }
        }
        public bool mChinhSua
        {
            get
            {
                return _mChinhSua;
            }
            set
            {
                if (_mChinhSua == value)
                    return;
                _mChinhSua = value;
                NotifyOfPropertyChange(() => mChinhSua);
            }
        }
        #endregion

        #region binding visibilty

        public Button lnkDelete { get; set; }
        public Button lnkEdit { get; set; }
        public Button lnkView { get; set; }
        public void lplDelete_Loaded(object sender)
        {
            lnkDelete = sender as Button;
            lnkDelete.Visibility = Globals.convertVisibility(mChinhSua );
        }
        public void lplEdit_Loaded(object sender)
        {
            lnkEdit = sender as Button;
            lnkEdit.Visibility = Globals.convertVisibility(mChinhSua );
        }
        #endregion

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


        void ObjDrugDeptSellPriceProfitScale_GetList_Paging_OnRefresh(object sender, RefreshEventArgs e)
        {
            DrugDeptSellPriceProfitScale_GetList_Paging(ObjDrugDeptSellPriceProfitScale_GetList_Paging.PageIndex, ObjDrugDeptSellPriceProfitScale_GetList_Paging.PageSize, false);
        }

        private PagedSortableCollectionView<DrugDeptSellPriceProfitScale> _ObjDrugDeptSellPriceProfitScale_GetList_Paging;
        public PagedSortableCollectionView<DrugDeptSellPriceProfitScale> ObjDrugDeptSellPriceProfitScale_GetList_Paging
        {
            get { return _ObjDrugDeptSellPriceProfitScale_GetList_Paging; }
            set
            {
                _ObjDrugDeptSellPriceProfitScale_GetList_Paging = value;
                NotifyOfPropertyChange(() => ObjDrugDeptSellPriceProfitScale_GetList_Paging);
            }
        }

        private void DrugDeptSellPriceProfitScale_GetList_Paging(int PageIndex, int PageSize, bool CountTotal)
        {
            ObjDrugDeptSellPriceProfitScale_GetList_Paging.Clear();
            
            Globals.EventAggregator.Publish(new BusyEvent() { IsBusy = true, Message = string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.Z0715_DSCgThuc) });
            IsLoading = true;
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyDrugServiceClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginDrugDeptSellPriceProfitScale_GetList_Paging(V_MedProductType, IsActive, PageIndex, PageSize, "", CountTotal, Globals.DispatchCallback((asyncResult) =>
                        {
                            int Total = 0;
                            IList<DrugDeptSellPriceProfitScale> allItems = null;
                            bool bOK = false;
                            try
                            {
                                allItems = client.EndDrugDeptSellPriceProfitScale_GetList_Paging(out Total, asyncResult);
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
                                    ObjDrugDeptSellPriceProfitScale_GetList_Paging.TotalItemCount = Total;
                                }
                                if (allItems != null)
                                {
                                    foreach (var item in allItems)
                                    {
                                        ObjDrugDeptSellPriceProfitScale_GetList_Paging.Add(item);
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
            DrugDeptSellPriceProfitScale p = (selectedItem as DrugDeptSellPriceProfitScale);

            if (p != null && p.DrugDeptSellPriceProfitScaleID > 0)
            {
                if (MessageBox.Show(string.Format("{0}: ", eHCMSResources.A0149_G1_Msg_ConfTatKichHoatCThuc) + p.RecCreatedDate.ToString("dd/MM/yyyy") + string.Format(", {0}", eHCMSResources.Z0355_G1_NayKhong), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    DrugDeptSellPriceProfitScale_IsActive(p.DrugDeptSellPriceProfitScaleID,false);
                }
            }
        }

        private void DrugDeptSellPriceProfitScale_IsActive(Int64 DrugDeptSellPriceProfitScaleID, Boolean IsActive)
        {
            string Result = "";

            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.K2887_G1_DangXuLy });
            IsLoading = true;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyDrugServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginDrugDeptSellPriceProfitScale_IsActive(DrugDeptSellPriceProfitScaleID,IsActive, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            contract.EndDrugDeptSellPriceProfitScale_IsActive(out Result, asyncResult);
                            switch (Result)
                            {
                                case "0":
                                    {
                                        MessageBox.Show(eHCMSResources.Z0363_G1_KgThanhCong, eHCMSResources.G2617_G1_Xoa, MessageBoxButton.OK);
                                        break;
                                    }
                                case "1":
                                    {
                                        ObjDrugDeptSellPriceProfitScale_GetList_Paging.PageIndex = 0;
                                        DrugDeptSellPriceProfitScale_GetList_Paging(0, ObjDrugDeptSellPriceProfitScale_GetList_Paging.PageSize, true);
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
                    ObjDrugDeptSellPriceProfitScale_GetList_Paging.PageIndex = 0;
                    DrugDeptSellPriceProfitScale_GetList_Paging(0, ObjDrugDeptSellPriceProfitScale_GetList_Paging.PageSize, true);
                }
            }
        }


        public void hplAddNew_Click()
        {
            //var typeInfo = Globals.GetViewModel<IDrugDeptSellPriceProfitScaleAddEdit>();
            //typeInfo.TitleForm = string.Format("{0} ", eHCMSResources.Z0716_G1_ThemMoiThangGia) + Globals.GetTextV_MedProductType(V_MedProductType);
            //typeInfo.V_MedProductType = V_MedProductType;
            //typeInfo.InitializeNewItem();
            //typeInfo.FormLoad();

            //var instance = typeInfo as Conductor<object>;

            //Globals.ShowDialog(instance, (o) =>
            //{
            //    //làm gì đó
            //});

            Action<IDrugDeptSellPriceProfitScaleAddEdit> onInitDlg = (typeInfo) =>
            {
                typeInfo.TitleForm = string.Format("{0} ", eHCMSResources.Z0716_G1_ThemMoiThangGia) + Globals.GetTextV_MedProductType(V_MedProductType);
                typeInfo.V_MedProductType = V_MedProductType;
                typeInfo.InitializeNewItem();
                typeInfo.FormLoad();
            };
            GlobalsNAV.ShowDialog<IDrugDeptSellPriceProfitScaleAddEdit>(onInitDlg);
        }


        public void hplEdit_Click(object selectedItem)
        {
            if (selectedItem != null)
            {
                //var typeInfo = Globals.GetViewModel<IDrugDeptSellPriceProfitScaleAddEdit>();

                //typeInfo.TitleForm = string.Format("{0} ", eHCMSResources.Z0717_G1_HChinhThangGia) + Globals.GetTextV_MedProductType(V_MedProductType);
                //typeInfo.V_MedProductType = V_MedProductType;
                //typeInfo.ObjDrugDeptSellPriceProfitScale_Current = ObjectCopier.DeepCopy((selectedItem as DataEntities.DrugDeptSellPriceProfitScale));
                //typeInfo.FormLoad();

                //var instance = typeInfo as Conductor<object>;

                //Globals.ShowDialog(instance, (o) =>
                //{
                //    //làm gì đó
                //});

                Action<IDrugDeptSellPriceProfitScaleAddEdit> onInitDlg = (typeInfo) =>
                {
                    typeInfo.TitleForm = string.Format("{0} ", eHCMSResources.Z0717_G1_HChinhThangGia) + Globals.GetTextV_MedProductType(V_MedProductType);
                    typeInfo.V_MedProductType = V_MedProductType;
                    typeInfo.ObjDrugDeptSellPriceProfitScale_Current = ObjectCopier.DeepCopy((selectedItem as DataEntities.DrugDeptSellPriceProfitScale));
                    typeInfo.FormLoad();
                };
                GlobalsNAV.ShowDialog<IDrugDeptSellPriceProfitScaleAddEdit>(onInitDlg);
            }
        }

        public void chkIsActive_Click(object sender, RoutedEventArgs e)
        {
            ObjDrugDeptSellPriceProfitScale_GetList_Paging.PageIndex = 0;
            DrugDeptSellPriceProfitScale_GetList_Paging(0, ObjDrugDeptSellPriceProfitScale_GetList_Paging.PageSize, true);            
        }

    }
}
