using System.ComponentModel.Composition;
using Caliburn.Micro;
using System.Windows;
using System;
using System.Threading;
using DataEntities;
using eHCMSLanguage;
using aEMR.ViewContracts;
using aEMR.Infrastructure.Events;
using aEMR.Common.Collections;
using aEMR.Infrastructure;
using System.Windows.Controls;
using aEMR.ServiceClient;
using Castle.Windsor;
using aEMR.Infrastructure.CachingUtils;

namespace aEMR.Common.ViewModels
{
    [Export(typeof(ICMDrugDeptSellingItemPrices_ListDrug)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class CMDrugDeptSellingItemPrices_ListDrugViewModel : Conductor<object>, ICMDrugDeptSellingItemPrices_ListDrug
        , IHandle<Drug_AddEditViewModel_Save_Event>
        , IHandle<ReLoadDataAfterU>
    {
        [ImportingConstructor]
        public CMDrugDeptSellingItemPrices_ListDrugViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg, ISalePosCaching salePosCaching)
        {
            eventArg.Subscribe(this);
        }
        protected override void OnActivate()
        {
            authorization();
            base.OnActivate();

            SearchCriteria = new RefGenMedProductDetailsSearchCriteria();
            SearchCriteria.V_MedProductType = V_MedProductType;

            ObjRefGenMedProductDetails_Paging = new PagedSortableCollectionView<RefGenMedProductDetails>();
            ObjRefGenMedProductDetails_Paging.OnRefresh += new EventHandler<RefreshEventArgs>(ObjRefGenMedProductDetails_Paging_OnRefresh);
            //ObjRefGenMedProductDetails_Paging.PageIndex = 0;
            //RefGenMedProductDetails_Paging(0, ObjRefGenMedProductDetails_Paging.PageSize, true);
        }
        public void authorization()
        {
            if (!Globals.isAccountCheck)
            {
                return;
            }
        }
        #region checking account

        private bool _mTim = true;
        private bool _mChinhSua = true;
        private bool _mXemDSGia = true;
        private bool _mTaoGiaMoi = true;
        private bool _mChinhSuaGia = true;
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
        public bool mXemDSGia
        {
            get
            {
                return _mXemDSGia;
            }
            set
            {
                if (_mXemDSGia == value)
                    return;
                _mXemDSGia = value;
                NotifyOfPropertyChange(() => mChinhSua);
            }
        }
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
        public Button hplListPrice { get; set; }
        public void lnkDelete_Loaded(object sender)
        {
            lnkDelete = sender as Button;
            lnkDelete.Visibility = Globals.convertVisibility(mChinhSua && IsEdit);
        }
        public void lnkEdit_Loaded(object sender)
        {
            lnkEdit = sender as Button;
            lnkEdit.Visibility = Globals.convertVisibility(mChinhSua && IsEdit);
        }
        public void hplListPrice_Loaded(object sender)
        {
            hplListPrice = sender as Button;
            hplListPrice.Visibility = Globals.convertVisibility(mXemDSGia);
        }
        #endregion
        //Prop Text
        private bool _IsEdit = true;
        public bool IsEdit
        {
            get
            {
                return _IsEdit;
            }
            set
            {
                if (_IsEdit == value)
                    return;
                _IsEdit = value;
                NotifyOfPropertyChange(() => IsEdit);
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
                if (IsStore)
                {
                    IsEdit = false;
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

        private string _TextGroupTimKiem;
        public string TextGroupTimKiem
        {
            get { return _TextGroupTimKiem; }
            set
            {
                _TextGroupTimKiem = value;
                NotifyOfPropertyChange(() => TextGroupTimKiem);
            }
        }

        private string _TextButtonThemMoi;
        public string TextButtonThemMoi
        {
            get { return _TextButtonThemMoi; }
            set
            {
                _TextButtonThemMoi = value;
                NotifyOfPropertyChange(() => TextButtonThemMoi);
            }
        }
        private string _TextDanhSach;
        public string TextDanhSach
        {
            get { return _TextDanhSach; }
            set
            {
                _TextDanhSach = value;
                NotifyOfPropertyChange(() => TextDanhSach);
            }
        }


        private Visibility _dgColumnExtOfThuoc_Visible;
        public Visibility dgColumnExtOfThuoc_Visible
        {
            get { return _dgColumnExtOfThuoc_Visible; }
            set
            {
                _dgColumnExtOfThuoc_Visible = value;
                NotifyOfPropertyChange(() => dgColumnExtOfThuoc_Visible);
            }
        }

        //Prop Text


        private Int64 _V_MedProductType;
        public Int64 V_MedProductType
        {
            get { return _V_MedProductType; }
            set
            {
                _V_MedProductType = value;
                NotifyOfPropertyChange(() => V_MedProductType);
            }

        }

        void ObjRefGenMedProductDetails_Paging_OnRefresh(object sender, RefreshEventArgs e)
        {
            RefGenMedProductDetails_Paging(ObjRefGenMedProductDetails_Paging.PageIndex, ObjRefGenMedProductDetails_Paging.PageSize, false);
        }

        private RefGenMedProductDetailsSearchCriteria _SearchCriteria;
        public RefGenMedProductDetailsSearchCriteria SearchCriteria
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

        private PagedSortableCollectionView<RefGenMedProductDetails> _ObjRefGenMedProductDetails_Paging;
        public PagedSortableCollectionView<RefGenMedProductDetails> ObjRefGenMedProductDetails_Paging
        {
            get { return _ObjRefGenMedProductDetails_Paging; }
            set
            {
                _ObjRefGenMedProductDetails_Paging = value;
                NotifyOfPropertyChange(() => ObjRefGenMedProductDetails_Paging);
            }
        }

        private string TextLoadList()
        {
            string TextLoad = "";
            switch (V_MedProductType)
            {
                case 11001:
                    {
                        TextLoad = string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.K3080_G1_DSThuoc);
                        break;
                    }
                case 11002:
                    {
                        TextLoad = string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.Z0657_G1_DSYCu);
                        break;
                    }
                case 11003:
                    {
                        TextLoad = string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.Z0658_G1_DSHChat);
                        break;
                    }
            }
            return TextLoad;
        }

        private void RefGenMedProductDetails_Paging(int PageIndex, int PageSize, bool CountTotal)
        {
            string TextLoading = TextLoadList();

            //Globals.EventAggregator.Publish(new BusyEvent() { IsBusy = true, Message = TextLoading });
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyGenMedDeptDetailsServiceClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginRefGenMedProductDetails_Paging(SearchCriteria, PageIndex, PageSize, "", CountTotal, Globals.DispatchCallback((asyncResult) =>
                        {
                            int Total = 0;
                            var allItems = client.EndRefGenMedProductDetails_Paging(out Total, asyncResult);
                            ObjRefGenMedProductDetails_Paging.Clear();

                            if (CountTotal)
                            {
                                ObjRefGenMedProductDetails_Paging.TotalItemCount = Total;
                            }
                            if (allItems != null)
                            {
                                foreach (var item in allItems)
                                {
                                    ObjRefGenMedProductDetails_Paging.Add(item);
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
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }

        public void btFind()
        {
            ObjRefGenMedProductDetails_Paging.PageIndex = 0;
            RefGenMedProductDetails_Paging(0, ObjRefGenMedProductDetails_Paging.PageSize, true);
        }


        //public void hplAddNew_Click()
        //{
        //    switch (V_MedProductType)
        //    {
        //        case 11001:
        //            {
        //                var typeInfo = Globals.GetViewModel<IDrug_AddEdit>();
        //                typeInfo.InitializeNewItem(V_MedProductType);

        //                typeInfo.TitleForm = string.Format("{0} ", eHCMSResources.G0276_G1_ThemMoi) + TextType();

        //                var instance = typeInfo as Conductor<object>;
        //                Globals.ShowDialog(instance, (o) =>
        //                {
        //                    //lam gi do
        //                });

        //                break;
        //            }
        //        case 11002:
        //            {
        //                var typeInfo = Globals.GetViewModel<IMedicalDevices_Chemical_AddEdit>();
        //                typeInfo.V_MedProductType = V_MedProductType;
        //                typeInfo.InitializeNewItem(V_MedProductType);

        //                typeInfo.TitleForm = string.Format("{0} ", eHCMSResources.G0276_G1_ThemMoi) + TextType();

        //                var instance = typeInfo as Conductor<object>;
        //                Globals.ShowDialog(instance, (o) =>
        //                {
        //                    //lam gi do
        //                });

        //                break;
        //            }
        //        case 11003:
        //            {
        //                var typeInfo = Globals.GetViewModel<IMedicalDevices_Chemical_AddEdit>();
        //                typeInfo.V_MedProductType = V_MedProductType;
        //                typeInfo.InitializeNewItem(V_MedProductType);

        //                typeInfo.TitleForm = string.Format("{0} ", eHCMSResources.G0276_G1_ThemMoi) + TextType();

        //                var instance = typeInfo as Conductor<object>;
        //                Globals.ShowDialog(instance, (o) =>
        //                {
        //                    //lam gi do
        //                });

        //                break;
        //            }
        //    }

        //}

        public void hplEdit_Click(object selectItem)
        {
            if (selectItem != null)
            {
                RefGenMedProductDetails p = (selectItem as RefGenMedProductDetails);

                switch (V_MedProductType)
                {
                    case 11001:
                        {
                            Action<ICMDrug_AddEdit> onInitDlg = delegate (ICMDrug_AddEdit locationVm)
                            {
                                locationVm.ObjRefGenMedProductDetails_Current = ObjectCopier.DeepCopy(p);
                                locationVm.SupplierGenMedProduct_LoadDrugIDNotPaging(p.GenMedProductID);
                                locationVm.TitleForm = string.Format("{0} ", eHCMSResources.T1484_G1_HChinh) + TextType() + " : " + p.BrandName.Trim();
                            };
                            GlobalsNAV.ShowDialog<ICMDrug_AddEdit>(onInitDlg);
                            break;
                        }
                    case 11002:
                        {
                            Action<IMedicalDevices_Chemical_AddEdit> onInitDlg = delegate (IMedicalDevices_Chemical_AddEdit locationVm)
                            {
                                locationVm.V_MedProductType = V_MedProductType;
                                locationVm.ObjRefGenMedProductDetails_Current = ObjectCopier.DeepCopy(p);
                                locationVm.SupplierGenMedProduct_LoadDrugIDNotPaging(p.GenMedProductID);
                                locationVm.TitleForm = string.Format("{0} ", eHCMSResources.T1484_G1_HChinh) + TextType() + " : " + p.BrandName.Trim();
                            };
                            GlobalsNAV.ShowDialog<IMedicalDevices_Chemical_AddEdit>(onInitDlg);
                            break;
                        }
                    case 11003:
                        {
                            Action<IMedicalDevices_Chemical_AddEdit> onInitDlg = delegate (IMedicalDevices_Chemical_AddEdit locationVm)
                            {
                                locationVm.V_MedProductType = V_MedProductType;
                                locationVm.ObjRefGenMedProductDetails_Current = ObjectCopier.DeepCopy(p);
                                locationVm.SupplierGenMedProduct_LoadDrugIDNotPaging(p.GenMedProductID);
                                locationVm.TitleForm = string.Format("{0} ", eHCMSResources.T1484_G1_HChinh) + TextType() + " : " + p.BrandName.Trim();
                            };
                            GlobalsNAV.ShowDialog<IMedicalDevices_Chemical_AddEdit>(onInitDlg);
                            break;
                        }
                }
            }
        }


        public void hplDelete_Click(object selectItem)
        {
            if (selectItem != null)
            {
                RefGenMedProductDetails p = (selectItem as RefGenMedProductDetails);
                if (MessageBox.Show(string.Format(eHCMSResources.Z0476_G1_BanMuonXoa, p.BrandName), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)

                {
                    RefGenMedProductDetails_MarkDelete(p.GenMedProductID);
                }
            }
        }

        private void RefGenMedProductDetails_MarkDelete(Int64 GenMedProductID)
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.Z0492_G1_DangXoa) });

            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyGenMedDeptDetailsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginRefGenMedProductDetails_MarkDelete(GenMedProductID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            string Res = "";

                            contract.EndRefGenMedProductDetails_MarkDelete(out Res, asyncResult);
                            switch (Res)
                            {
                                case "Delete-0":
                                    {
                                        MessageBox.Show(eHCMSResources.A0478_G1_Msg_InfoXoaOK, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                        break;
                                    }
                                case "Delete-1":
                                    {
                                        ObjRefGenMedProductDetails_Paging.PageIndex = 0;
                                        RefGenMedProductDetails_Paging(0, ObjRefGenMedProductDetails_Paging.PageSize, true);
                                        MessageBox.Show(eHCMSResources.A0478_G1_Msg_InfoXoaOK, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
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
                            Globals.IsBusy = false;
                        }
                    }), null);
                }


            });
            t.Start();
        }


        public void Handle(Drug_AddEditViewModel_Save_Event message)
        {
            if (message != null)
            {
                ObjRefGenMedProductDetails_Paging.PageIndex = 0;
                RefGenMedProductDetails_Paging(0, ObjRefGenMedProductDetails_Paging.PageSize, true);
            }
        }

        private string TextType()
        {
            string Result = "";
            switch (V_MedProductType)
            {
                case 11001:
                    {
                        Result = eHCMSResources.G0787_G1_Thuoc;
                        break;
                    }
                case 11002:
                    {
                        Result = eHCMSResources.G2907_G1_YCu;
                        break;
                    }
                case 11003:
                    {
                        Result = eHCMSResources.T1616_G1_HC;
                        break;
                    }
            }
            return Result;
        }

        //ny them 

        private bool _IsPopUp = false;
        public bool IsPopUp
        {
            get { return _IsPopUp; }
            set
            {
                _IsPopUp = value;
                NotifyOfPropertyChange(() => IsPopUp);
            }
        }

        public void griddrug_DblClick(object sender, Common.EventArgs<object> e)
        {
            if (IsPopUp)
            {
                TryClose();
                Globals.EventAggregator.Publish(new DrugDeptCloseSearchDrugEvent() { SupplierDrug = e.Value });
            }
        }


        #region DS Giá
        public void hplListPrice_Click(object selectItem)
        {
            if (selectItem != null)
            {
                Action<ICMDrugDeptSellingItemPrices> onInitDlg = delegate (ICMDrugDeptSellingItemPrices typeInfo)
                {
                    RefGenMedProductDetails p = (selectItem as RefGenMedProductDetails);
                    typeInfo.IsStore = IsStore;
                    typeInfo.SearchCriteria = new DrugDeptSellingItemPricesSearchCriteria();
                    typeInfo.SearchCriteria.GenMedProductID = p.GenMedProductID;
                    typeInfo.SearchCriteria.PriceType = 1;//Hiện hành
                    typeInfo.ObjDrug_Current = p;
                    typeInfo.V_MedProductType = V_MedProductType;
                    typeInfo.TitleForm = string.Format("{0} ", eHCMSResources.G0597_G1_TTinGia) + TextType();
                    typeInfo.mTaoGiaMoi = mTaoGiaMoi;
                    typeInfo.mChinhSuaGia = mChinhSuaGia;
                };
                GlobalsNAV.ShowDialog<ICMDrugDeptSellingItemPrices>(onInitDlg);
            }
        }
        public void Handle(ReLoadDataAfterU message)
        {
            if (message != null)
            {
                RefGenMedProductDetails_Paging(ObjRefGenMedProductDetails_Paging.PageIndex, ObjRefGenMedProductDetails_Paging.PageSize, true);
            }
        }
        #endregion
    }
}