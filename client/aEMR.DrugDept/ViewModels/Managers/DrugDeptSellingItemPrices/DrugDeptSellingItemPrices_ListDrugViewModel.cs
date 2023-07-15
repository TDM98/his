using eHCMSLanguage;
using System.ComponentModel.Composition;
using aEMR.ViewContracts;
using Caliburn.Micro;
using System.Windows;
using System;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using System.Threading;
using DataEntities;
using aEMR.Common;
using aEMR.Common.Collections;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using System.Windows.Controls;

namespace aEMR.DrugDept.ViewModels
{
     [Export(typeof(IDrugDeptSellingItemPrices_ListDrug)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class DrugDeptSellingItemPrices_ListDrugViewModel : Conductor<object>, IDrugDeptSellingItemPrices_ListDrug
                 , IHandle<Drug_AddEditViewModel_Save_Event>
                , IHandle<ReLoadDataAfterU>

    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public DrugDeptSellingItemPrices_ListDrugViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            eventAggregator.Subscribe(this);
        }
        protected override void OnActivate()
        {
            authorization();
            base.OnActivate();

            Globals.EventAggregator.Subscribe(this);

            SearchCriteria = new RefGenMedProductDetailsSearchCriteria();
            SearchCriteria.V_MedProductType = V_MedProductType;
            ObjRefGenMedProductDetails_Paging = new PagedSortableCollectionView<RefGenMedProductDetails>();
            ObjRefGenMedProductDetails_Paging.OnRefresh += new EventHandler<RefreshEventArgs>(ObjRefGenMedProductDetails_Paging_OnRefresh);

            ObjRefGenMedProductDetails_Paging.PageIndex = 0;
            RefGenMedProductDetails_Paging(0, ObjRefGenMedProductDetails_Paging.PageSize, true);
        }

        public void authorization()
        {
            if (!Globals.isAccountCheck)
            {
                return;
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

        #region checking account

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
        public Button lnkView { get; set; }
        public void lnkDelete_Loaded(object sender)
        {
            lnkDelete = sender as Button;
            lnkDelete.Visibility = Globals.convertVisibility(bDelete);
        }
        public void lnkEdit_Loaded(object sender)
        {
            lnkEdit = sender as Button;
            lnkEdit.Visibility = Globals.convertVisibility(bEdit);
        }
        public void lnkView_Loaded(object sender)
        {
            lnkView = sender as Button;
            lnkView.Visibility = Globals.convertVisibility(bView);
        }
        #endregion
        //Prop Text
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
            RefGenMedProductDetails_Paging(ObjRefGenMedProductDetails_Paging.PageIndex,
                         ObjRefGenMedProductDetails_Paging.PageSize, false);
        }

        private DataEntities.RefGenMedProductDetailsSearchCriteria _SearchCriteria;
        public DataEntities.RefGenMedProductDetailsSearchCriteria SearchCriteria
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

        private PagedSortableCollectionView<DataEntities.RefGenMedProductDetails> _ObjRefGenMedProductDetails_Paging;
        public PagedSortableCollectionView<DataEntities.RefGenMedProductDetails> ObjRefGenMedProductDetails_Paging
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

            Globals.EventAggregator.Publish(new BusyEvent() { IsBusy = true, Message = TextLoading });
            IsLoading = true;
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
                    IsLoading = false;
                    Globals.IsBusy = false;
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
                            //var typeInfo = Globals.GetViewModel<IDrug_AddEdit>();
                            //typeInfo.ObjRefGenMedProductDetails_Current = ObjectCopier.DeepCopy(p);
                            //typeInfo.SupplierGenMedProduct_LoadDrugIDNotPaging(p.GenMedProductID);
                            //typeInfo.TitleForm = string.Format("{0} ", eHCMSResources.T1484_G1_HChinh) + TextType() + " : " + p.BrandName.Trim();

                            //var instance = typeInfo as Conductor<object>;

                            //Globals.ShowDialog(instance, (o) =>
                            //{
                            //    //lam gi do
                            //});

                            Action<IDrug_AddEdit> onInitDlg = (typeInfo) =>
                            {
                                typeInfo.ObjRefGenMedProductDetails_Current = ObjectCopier.DeepCopy(p);
                                typeInfo.SupplierGenMedProduct_LoadDrugIDNotPaging(p.GenMedProductID);
                                typeInfo.TitleForm = string.Format("{0} ", eHCMSResources.T1484_G1_HChinh) + TextType() + " : " + p.BrandName.Trim();
                            };
                            GlobalsNAV.ShowDialog<IDrug_AddEdit>(onInitDlg);

                            break;
                        }
                    case 11002:
                        {
                            //var typeInfo = Globals.GetViewModel<IMedicalDevices_Chemical_AddEdit>();
                            //typeInfo.V_MedProductType = V_MedProductType;
                            //typeInfo.ObjRefGenMedProductDetails_Current = ObjectCopier.DeepCopy(p);
                            //typeInfo.SupplierGenMedProduct_LoadDrugIDNotPaging(p.GenMedProductID);
                            //typeInfo.TitleForm = string.Format("{0} ", eHCMSResources.T1484_G1_HChinh) + TextType() + " : " + p.BrandName.Trim();
                            //var instance = typeInfo as Conductor<object>;
                            //Globals.ShowDialog(instance, (o) =>
                            //{
                            //    //lam gi do
                            //});

                            Action<IMedicalDevices_Chemical_AddEdit> onInitDlg = (typeInfo) =>
                            {
                                typeInfo.V_MedProductType = V_MedProductType;
                                typeInfo.ObjRefGenMedProductDetails_Current = ObjectCopier.DeepCopy(p);
                                typeInfo.SupplierGenMedProduct_LoadDrugIDNotPaging(p.GenMedProductID);
                                typeInfo.TitleForm = string.Format("{0} ", eHCMSResources.T1484_G1_HChinh) + TextType() + " : " + p.BrandName.Trim();
                            };
                            GlobalsNAV.ShowDialog<IMedicalDevices_Chemical_AddEdit>(onInitDlg);

                            break;
                        }
                    case 11003:
                        {
                            //var typeInfo = Globals.GetViewModel<IMedicalDevices_Chemical_AddEdit>();
                            //typeInfo.V_MedProductType = V_MedProductType;
                            //typeInfo.ObjRefGenMedProductDetails_Current = ObjectCopier.DeepCopy(p);
                            //typeInfo.SupplierGenMedProduct_LoadDrugIDNotPaging(p.GenMedProductID);
                            //typeInfo.TitleForm = string.Format("{0} ", eHCMSResources.T1484_G1_HChinh) + TextType() + " : " + p.BrandName.Trim();
                            //var instance = typeInfo as Conductor<object>;
                            //Globals.ShowDialog(instance, (o) =>
                            //{
                            //    //lam gi do
                            //});

                            Action<IMedicalDevices_Chemical_AddEdit> onInitDlg = (typeInfo) =>
                            {
                                typeInfo.V_MedProductType = V_MedProductType;
                                typeInfo.ObjRefGenMedProductDetails_Current = ObjectCopier.DeepCopy(p);
                                typeInfo.SupplierGenMedProduct_LoadDrugIDNotPaging(p.GenMedProductID);
                                typeInfo.TitleForm = string.Format("{0} ", eHCMSResources.T1484_G1_HChinh) + TextType() + " : " + p.BrandName.Trim();
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

                if (MessageBox.Show(string.Format(eHCMSResources.Z0476_G1_BanMuonXoa, p.BrandName), eHCMSResources.G2617_G1_Xoa, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    RefGenMedProductDetails_MarkDelete(p.GenMedProductID);
                }
            }
        }

        private void RefGenMedProductDetails_MarkDelete(Int64 GenMedProductID)
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.Z0492_G1_DangXoa) });
            IsLoading = true;
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
                                        MessageBox.Show(string.Format("{0}!", eHCMSResources.K0484_G1_XoaFail), eHCMSResources.G2617_G1_Xoa, MessageBoxButton.OK);
                                        break;
                                    }
                                case "Delete-1":
                                    {
                                        ObjRefGenMedProductDetails_Paging.PageIndex = 0;
                                        RefGenMedProductDetails_Paging(0, ObjRefGenMedProductDetails_Paging.PageSize, true);
                                        MessageBox.Show(eHCMSResources.A0478_G1_Msg_InfoXoaOK, eHCMSResources.G2617_G1_Xoa, MessageBoxButton.OK);
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
                //RefGenMedProductDetails p = (selectItem as RefGenMedProductDetails);
                //var typeInfo = Globals.GetViewModel<IDrugDeptSellingItemPrices>();
                //typeInfo.SearchCriteria = new DrugDeptSellingItemPricesSearchCriteria();
                //typeInfo.SearchCriteria.GenMedProductID = p.GenMedProductID;
                //typeInfo.SearchCriteria.PriceType = 1;//Hiện hành
                //typeInfo.ObjDrug_Current = p;
                //typeInfo.V_MedProductType = V_MedProductType;
                //typeInfo.TitleForm = string.Format("{0} ", eHCMSResources.G0597_G1_TTinGia) + TextType();

                //var instance = typeInfo as Conductor<object>;

                //Globals.ShowDialog(instance, (o) =>
                //{
                //    //làm gì đó
                //});

                Action<IDrugDeptSellingItemPrices> onInitDlg = (typeInfo) =>
                {
                    RefGenMedProductDetails p = (selectItem as RefGenMedProductDetails);
                    typeInfo.SearchCriteria = new DrugDeptSellingItemPricesSearchCriteria();
                    typeInfo.SearchCriteria.GenMedProductID = p.GenMedProductID;
                    typeInfo.SearchCriteria.PriceType = 1;//Hiện hành
                    typeInfo.ObjDrug_Current = p;
                    typeInfo.V_MedProductType = V_MedProductType;
                    typeInfo.TitleForm = string.Format("{0} ", eHCMSResources.G0597_G1_TTinGia) + TextType();
                };
                GlobalsNAV.ShowDialog<IDrugDeptSellingItemPrices>(onInitDlg);
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
