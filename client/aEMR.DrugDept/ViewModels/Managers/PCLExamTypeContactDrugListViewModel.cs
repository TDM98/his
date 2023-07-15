using eHCMSLanguage;
using System;
using System.ComponentModel.Composition;
using System.Threading;
using System.Windows;
using Caliburn.Micro;
using DataEntities;
using aEMR.Infrastructure.Events;
using aEMR.ViewContracts;
using Castle.Windsor;
using aEMR.Infrastructure.CachingUtils;
using aEMR.Common.Collections;
using aEMR.Infrastructure;
using System.Windows.Controls;
using aEMR.ServiceClient;
using Microsoft.Win32;
using aEMR.CommonTasks;
using System.Collections.ObjectModel;
using aEMR.Common;
using System.Linq;

/*
 * 20190731 #001 TBL: BM  0013026. Tim theo nhom hoat chat cho danh muc thuoc cua khoa duoc
 * 20201109 #002 TNHX: BM Chỉnh màn hình quản lý danh mục máu.
 */
namespace aEMR.DrugDept.ViewModels
{
    [Export(typeof(IPCLExamTypeContactDrugList)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class PCLExamTypeContactDrugListViewModel : Conductor<object>, IPCLExamTypeContactDrugList
        //, IHandle<Drug_AddEditViewModel_Save_Event>
    {
        [ImportingConstructor]
        public PCLExamTypeContactDrugListViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg, ISalePosCaching salePosCaching)
        {
            eventArg.Subscribe(this);
        }

        protected override void OnActivate()
        {
            Authorization();
            base.OnActivate();

            SearchCriteria = new RefGenMedProductDetailsSearchCriteria();
            SearchCriteria.V_MedProductType = V_MedProductType;
            ObjRefGenMedProductDetails_Paging = new PagedSortableCollectionView<RefGenMedProductDetails>();
            ObjRefGenMedProductDetails_Paging.OnRefresh += new EventHandler<RefreshEventArgs>(ObjRefGenMedProductDetails_Paging_OnRefresh);

            ObjRefGenMedProductDetails_Paging.PageIndex = 0;
            ObjRefGenMedProductDetails_Paging.PageSize = Globals.PageSize;
            RefGenMedProductDetails_Paging(0, ObjRefGenMedProductDetails_Paging.PageSize, true);

            ListPCLExamTypeContactDrugs = new PagedSortableCollectionView<PCLExamTypeContactDrugs>();
            ListPCLExamTypeContactDrugs.OnRefresh += new EventHandler<RefreshEventArgs>(ListPCLExamTypeContactDrugs_OnRefresh);

            ListPCLExamTypeContactDrugs.PageIndex = 0;
            ListPCLExamTypeContactDrugs.PageSize = Globals.PageSize;
            ListPCLExamTypeContactDrugs_Paging(0, ListPCLExamTypeContactDrugs.PageSize, true);
            /*▼====: #001*/
            if (V_MedProductType == (long)AllLookupValues.MedProductType.THUOC)
            {
                GetParRefGeneric();
                ShowCatDrugType = true;
            }
            /*▲====: #001*/

            ListPCLExamTypeContactDrug_Add = new ObservableCollection<PCLExamTypeContactDrugs>();
            ListPCLExamTypeContactDrug_Del = new ObservableCollection<PCLExamTypeContactDrugs>();
        }

        public void Authorization()
        {
            if (!Globals.isAccountCheck)
            {
                return;
            }
        }

        #region checking account

        private bool _mTim = true;
        private bool _mThemMoi = false;
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

        public bool mThemMoi
        {
            get
            {
                return _mThemMoi;
            }
            set
            {
                if (_mThemMoi == value)
                    return;
                _mThemMoi = value;
                NotifyOfPropertyChange(() => mThemMoi);
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
            lnkDelete.Visibility = Globals.convertVisibility(mChinhSua && isEdit);
        }

        public void lnkEdit_Loaded(object sender)
        {
            lnkEdit = sender as Button;
            lnkEdit.Visibility = Globals.convertVisibility(mChinhSua && isEdit);
        }

        public void lnkView_Loaded(object sender)
        {
            lnkView = sender as Button;
            lnkView.Visibility = Globals.convertVisibility(bView);
        }
        #endregion

        //Prop Text
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
                    isEdit = false;
                }
            }
        }

        private bool _isEdit = true;
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
                NotifyOfPropertyChange(() => isEdit);
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
        private long _V_MedProductType;
        public long V_MedProductType
        {
            get { return _V_MedProductType; }
            set
            {
                _V_MedProductType = value;
                NotifyOfPropertyChange(() => V_MedProductType);
                NotifyOfPropertyChange(() => VisibilityConsult);
                NotifyOfPropertyChange(() => IsMatView);
            }
        }
        private long _PCLExamTypeID;
        public long PCLExamTypeID
        {
            get { return _PCLExamTypeID; }
            set
            {
                _PCLExamTypeID = value;
                NotifyOfPropertyChange(() => PCLExamTypeID);
            }
        }

        private ObservableCollection<RefGeneric> _LstRefGeneric;
        public ObservableCollection<RefGeneric> LstRefGeneric
        {
            get { return _LstRefGeneric; }
            set
            {
                if (_LstRefGeneric != value)
                {
                    _LstRefGeneric = value;
                    NotifyOfPropertyChange(() => LstRefGeneric);
                }
            }
        }

        private ObservableCollection<PCLExamTypeContactDrugs> _ListPCLExamTypeContactDrug_Add;
        public ObservableCollection<PCLExamTypeContactDrugs> ListPCLExamTypeContactDrug_Add
        {
            get { return _ListPCLExamTypeContactDrug_Add; }
            set
            {
                if (_ListPCLExamTypeContactDrug_Add != value)
                {
                    _ListPCLExamTypeContactDrug_Add = value;
                    NotifyOfPropertyChange(() => ListPCLExamTypeContactDrug_Add);
                }
            }
        }

        private ObservableCollection<PCLExamTypeContactDrugs> _ListPCLExamTypeContactDrug_Del;
        public ObservableCollection<PCLExamTypeContactDrugs> ListPCLExamTypeContactDrug_Del
        {
            get { return _ListPCLExamTypeContactDrug_Del; }
            set
            {
                if (_ListPCLExamTypeContactDrug_Del != value)
                {
                    _ListPCLExamTypeContactDrug_Del = value;
                    NotifyOfPropertyChange(() => ListPCLExamTypeContactDrug_Del);
                }
            }
        }

        private PagedSortableCollectionView<PCLExamTypeContactDrugs> _ListPCLExamTypeContactDrugs;
        public PagedSortableCollectionView<PCLExamTypeContactDrugs> ListPCLExamTypeContactDrugs
        {
            get { return _ListPCLExamTypeContactDrugs; }
            set
            {
                if (_ListPCLExamTypeContactDrugs != value)
                {
                    _ListPCLExamTypeContactDrugs = value;
                    NotifyOfPropertyChange(() => ListPCLExamTypeContactDrugs);
                }
            }
        }

        private RefGeneric _SelectedRefGeneric;
        public RefGeneric SelectedRefGeneric
        {
            get { return _SelectedRefGeneric; }
            set
            {
                if (_SelectedRefGeneric != value)
                {
                    _SelectedRefGeneric = value;
                    NotifyOfPropertyChange(() => SelectedRefGeneric);
                }
            }
        }

        public Visibility VisibilityConsult
        {
            get { return _V_MedProductType == (long)AllLookupValues.MedProductType.THUOC ? Visibility.Visible : Visibility.Collapsed; }
        }

        public bool IsMatView
        {
            get
            {
                return V_MedProductType == (long)AllLookupValues.MedProductType.Y_CU;
            }
        }

        void ObjRefGenMedProductDetails_Paging_OnRefresh(object sender, RefreshEventArgs e)
        {
            RefGenMedProductDetails_Paging(ObjRefGenMedProductDetails_Paging.PageIndex,
                         ObjRefGenMedProductDetails_Paging.PageSize, false);
        }
        void ListPCLExamTypeContactDrugs_OnRefresh(object sender, RefreshEventArgs e)
        {
            ListPCLExamTypeContactDrugs_Paging(ListPCLExamTypeContactDrugs.PageIndex,
                         ListPCLExamTypeContactDrugs.PageSize, false);
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

        private RefGenMedProductDetails _ObjRefGenMedProductDetails_Selected;
        public RefGenMedProductDetails ObjRefGenMedProductDetails_Selected
        {
            get { return _ObjRefGenMedProductDetails_Selected; }
            set
            {
                _ObjRefGenMedProductDetails_Selected = value;
                NotifyOfPropertyChange(() => ObjRefGenMedProductDetails_Selected);
            }
        }

        private string TextLoadList()
        {
            string TextLoad = "";
            switch (V_MedProductType)
            {
                case (long)AllLookupValues.MedProductType.THUOC:
                    {
                        TextLoad = eHCMSResources.K3080_G1_DSThuoc;
                        break;
                    }
                case (long)AllLookupValues.MedProductType.Y_CU:
                    {
                        TextLoad = eHCMSResources.Z0657_G1_DSYCu;
                        break;
                    }
                case (long)AllLookupValues.MedProductType.HOA_CHAT:
                    {
                        TextLoad = eHCMSResources.Z0658_G1_DSHChat;
                        break;
                    }
                case (long)AllLookupValues.MedProductType.VTYT_TIEUHAO:
                    {
                        TextLoad = eHCMSResources.Z2457_G1_DSachVTYTTH;
                        break;
                    }
                case (long)AllLookupValues.MedProductType.TIEM_NGUA:
                    {
                        TextLoad = eHCMSResources.Z2463_G1_DSachTiemNgua;
                        break;
                    }
                case (long)AllLookupValues.MedProductType.MAU:
                    {
                        TextLoad = eHCMSResources.Z2492_G1_DSachMau;
                        break;
                    }
                case (long)AllLookupValues.MedProductType.VAN_PHONG_PHAM:
                    {
                        TextLoad = eHCMSResources.Z2524_G1_DSachVPP;
                        break;
                    }
                case (long)AllLookupValues.MedProductType.VATTU_TIEUHAO:
                    {
                        TextLoad = eHCMSResources.Z2525_G1_DSachVTTH;
                        break;
                    }
                case (long)AllLookupValues.MedProductType.THANHTRUNG:
                    {
                        TextLoad = eHCMSResources.Z2498_G1_DSachThanhTrung;
                        break;
                    }
            }
            return TextLoad;
        }

        private void RefGenMedProductDetails_Paging(int PageIndex, int PageSize, bool CountTotal)
        {
            string TextLoading = TextLoadList();
            GetCondition();
            this.ShowBusyIndicator(TextLoading);

            if (SearchCriteria != null && !string.IsNullOrEmpty(SearchCriteria.Code))
            {
                SearchCriteria.Code = Globals.FormatCode(V_MedProductType, SearchCriteria.Code);
            }
            if (SelectedRefGeneric != null)
            {
                SearchCriteria.GenericID = SelectedRefGeneric.GenericID;
            }
            switch (IsCatDrugType)
            {
                case 0:
                    SearchCriteria.V_CatDrugType = (long)AllLookupValues.V_CatDrugType.All;
                    break;
                case 1:
                    SearchCriteria.V_CatDrugType = (long)AllLookupValues.V_CatDrugType.Shared;
                    break;
                case 2:
                    SearchCriteria.V_CatDrugType = (long)AllLookupValues.V_CatDrugType.Pharmacy;
                    break;
                case 3:
                    SearchCriteria.V_CatDrugType = (long)AllLookupValues.V_CatDrugType.DrugDept;
                    break;
            }
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyGenMedDeptDetailsServiceClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginRefGenMedProductDetails_Paging(SearchCriteria, PageIndex, PageSize, "", CountTotal, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
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
                            }
                            catch (Exception ex)
                            {
                                ClientLoggerHelper.LogInfo(ex.ToString());
                                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                            }
                            finally
                            {
                                this.HideBusyIndicator();
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    ClientLoggerHelper.LogInfo(ex.ToString());
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    this.HideBusyIndicator();
                }
            });

            t.Start();
        }
        private void ListPCLExamTypeContactDrugs_Paging(int PageIndex, int PageSize, bool CountTotal)
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyGenMedDeptDetailsServiceClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginListPCLExamTypeContactDrugs_Paging(PCLExamTypeID, PageIndex, PageSize, "", CountTotal, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                int Total = 0;
                                var allItems = client.EndListPCLExamTypeContactDrugs_Paging(out Total, asyncResult);
                                ListPCLExamTypeContactDrugs.Clear();

                                if (CountTotal)
                                {
                                    ListPCLExamTypeContactDrugs.TotalItemCount = Total;
                                }
                                if (allItems != null)
                                {
                                    foreach (var item in allItems)
                                    {
                                        ListPCLExamTypeContactDrugs.Add(item);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                ClientLoggerHelper.LogInfo(ex.ToString());
                                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                            }
                            finally
                            {
                                this.HideBusyIndicator();
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    ClientLoggerHelper.LogInfo(ex.ToString());
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
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

        private string strNameExcel = "";
        private void GetCondition()
        {
            if (SearchCriteria == null)
            {
                SearchCriteria = new RefGenMedProductDetailsSearchCriteria();
            }
            SearchCriteria.IsConsult = IsConsult;
            SearchCriteria.IsInsurance = IsInsurance;
            SearchCriteria.IsActive = IsActive;
            SearchCriteria.V_MedProductType = V_MedProductType;

            strNameExcel = "";
            if (string.IsNullOrEmpty(strBH))
            {
                strNameExcel = string.Format("{0} ", eHCMSResources.Z1044_G1_DMucThuoc) + strHoiChan;
            }
            else
            {
                if (!string.IsNullOrEmpty(strHoiChan))
                {
                    strNameExcel = string.Format("{0} ", eHCMSResources.Z1044_G1_DMucThuoc) + strBH + ", " + strHoiChan;
                }
                else
                {
                    strNameExcel = string.Format("{0} ", eHCMSResources.Z1044_G1_DMucThuoc) + strBH;
                }
            }
        }
        //Huyen 14/08/2015: áp dụng view mới khi thêm mới thuốc, y cụ, hóa chất nếu thuộc tính ApplyHINew_Report20150701 = true
        //Để sử dụng mẫu cũ, vào database --> table --> RefApplicationConfig --> chỉnh sửa giá trị dòng ApplyHINew_Report20150701 thành false
        //public void hplAddNew_Click()
        //{
        //    switch (V_MedProductType)
        //    {
        //        case (long)AllLookupValues.MedProductType.THUOC:
        //        case (long)AllLookupValues.MedProductType.NUTRITION:
        //            {
        //                if (Globals.ServerConfigSection.HealthInsurances.ApplyHINew_Report20150701 == true)
        //                {
        //                    //20191003 TBL: Sử dụng màn hình mới nhất 
        //                    //20180801 TTM neu mo chuc nang cho phep su dung kho BHYT thi su dung view moi
        //                    //if (Globals.ServerConfigSection.CommonItems.EnableHIStore)
        //                    //{
        //                    //    Action<ICMDrug_AddEdit_V3> onInitDlg_V3 = delegate (ICMDrug_AddEdit_V3 typeInfo)
        //                    //    {
        //                    //        typeInfo.InitializeNewItem(V_MedProductType);
        //                    //        typeInfo.TitleForm = string.Format("{0} ", eHCMSResources.G0276_G1_ThemMoi) + TextType();
        //                    //    };
        //                    //    GlobalsNAV.ShowDialog<ICMDrug_AddEdit_V3>(onInitDlg_V3);
        //                    //}
        //                    //else
        //                    //{
        //                    //    Action<ICMDrug_AddEdit_V2> onInitDlg = delegate (ICMDrug_AddEdit_V2 typeInfo)
        //                    //    {
        //                    //        typeInfo.InitializeNewItem(V_MedProductType);
        //                    //        typeInfo.TitleForm = string.Format("{0} ", eHCMSResources.G0276_G1_ThemMoi) + TextType();
        //                    //    };
        //                    //    GlobalsNAV.ShowDialog<ICMDrug_AddEdit_V2>(onInitDlg);
        //                    //}
        //                    void onInitDlg_V3(ICMDrug_AddEdit_V3 typeInfo)
        //                    {
        //                        typeInfo.InitializeNewItem(V_MedProductType);
        //                        typeInfo.TitleForm = string.Format("{0} ", eHCMSResources.G0276_G1_ThemMoi) + TextType();
        //                    }
        //                    GlobalsNAV.ShowDialog<ICMDrug_AddEdit_V3>(onInitDlg_V3, null, false, true, Globals.GetDefaultDialogViewSize());
        //                }
        //                else
        //                {
        //                    void onInitDlg(ICMDrug_AddEdit typeInfo)
        //                    {
        //                        typeInfo.InitializeNewItem(V_MedProductType);
        //                        typeInfo.TitleForm = string.Format("{0} ", eHCMSResources.G0276_G1_ThemMoi) + TextType();
        //                    }
        //                    GlobalsNAV.ShowDialog<ICMDrug_AddEdit>(onInitDlg, null, false, true, Globals.GetDefaultDialogViewSize());
        //                }
        //                break;
        //            }
        //        case (long)AllLookupValues.MedProductType.HOA_CHAT:
        //            {
        //                if (Globals.ServerConfigSection.HealthInsurances.ApplyHINew_Report20150701 == true)
        //                {
        //                    void onInitDlg(IDrugDept_MedDevAndChem_AddEdit typeInfo)
        //                    {
        //                        typeInfo.V_MedProductType = V_MedProductType;
        //                        typeInfo.IsShowDispenseVolume = true;
        //                        typeInfo.InitializeNewItem(V_MedProductType);
        //                        typeInfo.TitleForm = string.Format("{0} ", eHCMSResources.G0276_G1_ThemMoi) + TextType();
        //                    }
        //                    GlobalsNAV.ShowDialog<IDrugDept_MedDevAndChem_AddEdit>(onInitDlg, null, false, true, Globals.GetDefaultDialogViewSize());
        //                }
        //                else
        //                {
        //                    void onInitDlg(ICMMedicalDevices_Chemical_AddEdit typeInfo)
        //                    {
        //                        typeInfo.V_MedProductType = V_MedProductType;
        //                        typeInfo.IsShowDispenseVolume = true;
        //                        typeInfo.InitializeNewItem(V_MedProductType);
        //                        typeInfo.TitleForm = string.Format("{0} ", eHCMSResources.G0276_G1_ThemMoi) + TextType();
        //                    }
        //                    GlobalsNAV.ShowDialog<ICMMedicalDevices_Chemical_AddEdit>(onInitDlg, null, false, true, Globals.GetDefaultDialogViewSize());
        //                }
        //                break;
        //            }
        //        //▼====: #002
        //        case (long)AllLookupValues.MedProductType.MAU:
        //            {
        //                if (Globals.ServerConfigSection.HealthInsurances.ApplyHINew_Report20150701 == true)
        //                {
        //                    void onInitDlg(IDrugDept_Blood_AddEdit typeInfo)
        //                    {
        //                        typeInfo.V_MedProductType = V_MedProductType;
        //                        typeInfo.IsShowDispenseVolume = true;
        //                        typeInfo.InitializeNewItem(V_MedProductType);
        //                        typeInfo.TitleForm = string.Format("{0} ", eHCMSResources.G0276_G1_ThemMoi) + TextType();
        //                    }
        //                    GlobalsNAV.ShowDialog<IDrugDept_Blood_AddEdit>(onInitDlg, null, false, true, Globals.GetDefaultDialogViewSize());
        //                }
        //                else
        //                {
        //                    void onInitDlg(ICMMedicalDevices_Chemical_AddEdit typeInfo)
        //                    {
        //                        typeInfo.V_MedProductType = V_MedProductType;
        //                        typeInfo.IsShowDispenseVolume = true;
        //                        typeInfo.InitializeNewItem(V_MedProductType);
        //                        typeInfo.TitleForm = string.Format("{0} ", eHCMSResources.G0276_G1_ThemMoi) + TextType();
        //                    }
        //                    GlobalsNAV.ShowDialog<ICMMedicalDevices_Chemical_AddEdit>(onInitDlg, null, false, true, Globals.GetDefaultDialogViewSize());
        //                }
        //                break;
        //            }
        //        //▲====: #002
        //        case (long)AllLookupValues.MedProductType.Y_CU:
        //        case (long)AllLookupValues.MedProductType.VTYT_TIEUHAO:
        //        case (long)AllLookupValues.MedProductType.TIEM_NGUA:                
        //        case (long)AllLookupValues.MedProductType.THANHTRUNG:
        //        case (long)AllLookupValues.MedProductType.VAN_PHONG_PHAM:
        //        case (long)AllLookupValues.MedProductType.VATTU_TIEUHAO:
        //            {
        //                if (Globals.ServerConfigSection.HealthInsurances.ApplyHINew_Report20150701 == true)
        //                {
        //                    void onInitDlg(IDrugDept_MedDevAndChem_AddEdit typeInfo)
        //                    {
        //                        typeInfo.V_MedProductType = V_MedProductType;
        //                        typeInfo.IsShowDispenseVolume = true;
        //                        typeInfo.IsShowMedicalMaterial = true;
        //                        typeInfo.InitializeNewItem(V_MedProductType);
        //                        typeInfo.TitleForm = string.Format("{0} ", eHCMSResources.G0276_G1_ThemMoi) + TextType();
        //                    }
        //                    GlobalsNAV.ShowDialog<IDrugDept_MedDevAndChem_AddEdit>(onInitDlg, null, false, true, Globals.GetDefaultDialogViewSize());
        //                }
        //                else
        //                {
        //                    void onInitDlg(ICMMedicalDevices_Chemical_AddEdit typeInfo)
        //                    {
        //                        typeInfo.V_MedProductType = V_MedProductType;
        //                        typeInfo.IsShowDispenseVolume = true;
        //                        typeInfo.IsShowMedicalMaterial = true;
        //                        typeInfo.InitializeNewItem(V_MedProductType);
        //                        typeInfo.TitleForm = string.Format("{0} ", eHCMSResources.G0276_G1_ThemMoi) + TextType();
        //                    }
        //                    GlobalsNAV.ShowDialog<ICMMedicalDevices_Chemical_AddEdit>(onInitDlg, null, false, true, Globals.GetDefaultDialogViewSize());
        //                }
        //                break;
        //            }
        //    }
        //}
        //Huyen 14/08/2015: áp dụng view mới khi chỉnh sửa danh mục thuốc, y cụ, hóa chất nếu thuộc tính ApplyHINew_Report20150701 = true
        //Để sử dụng mẫu cũ, vào database --> table --> RefApplicationConfig --> chỉnh sửa giá trị dòng ApplyHINew_Report20150701 thành false
        //public void hplEdit_Click(object selectItem)
        //{
        //    if (selectItem != null)
        //    {
        //        RefGenMedProductDetails p = (selectItem as RefGenMedProductDetails);
        //        switch (V_MedProductType)
        //        {
        //            case (long)AllLookupValues.MedProductType.THUOC:
        //            case (long)AllLookupValues.MedProductType.NUTRITION:
        //                {
        //                    if (Globals.ServerConfigSection.HealthInsurances.ApplyHINew_Report20150701 == true)
        //                    {
        //                        //20191004 TBL: Sử dụng màn hình mới nhất
        //                        //if (Globals.ServerConfigSection.CommonItems.EnableHIStore)
        //                        //{
        //                        //    Action<ICMDrug_AddEdit_V3> onInitDlg_V3 = delegate (ICMDrug_AddEdit_V3 typeInfo)
        //                        //    {
        //                        //        typeInfo.ObjRefGenMedProductDetails_Current = ObjectCopier.DeepCopy(p);
        //                        //        typeInfo.SupplierGenMedProduct_LoadDrugIDNotPaging(p.GenMedProductID);
        //                        //        typeInfo.GetContraIndicatorDrugsRelToMedCondList(0, p.GenMedProductID);
        //                        //        typeInfo.TitleForm = string.Format("{0} ", eHCMSResources.T1484_G1_HChinh) + TextType() + " : " + p.BrandName.Trim();
        //                        //    };
        //                        //    GlobalsNAV.ShowDialog<ICMDrug_AddEdit_V3>(onInitDlg_V3);
        //                        //}
        //                        //else
        //                        //{
        //                        //    Action<ICMDrug_AddEdit_V2> onInitDlg = delegate (ICMDrug_AddEdit_V2 typeInfo)
        //                        //    {
        //                        //        typeInfo.ObjRefGenMedProductDetails_Current = ObjectCopier.DeepCopy(p);
        //                        //        typeInfo.SupplierGenMedProduct_LoadDrugIDNotPaging(p.GenMedProductID);
        //                        //        typeInfo.GetContraIndicatorDrugsRelToMedCondList(0, p.GenMedProductID);
        //                        //        typeInfo.TitleForm = string.Format("{0} ", eHCMSResources.T1484_G1_HChinh) + TextType() + " : " + p.BrandName.Trim();
        //                        //    };
        //                        //    GlobalsNAV.ShowDialog<ICMDrug_AddEdit_V2>(onInitDlg);
        //                        //}
        //                        void onInitDlg_V3(ICMDrug_AddEdit_V3 typeInfo)
        //                        {
        //                            typeInfo.ObjRefGenMedProductDetails_Current = ObjectCopier.DeepCopy(p);
        //                            typeInfo.SupplierGenMedProduct_LoadDrugIDNotPaging(p.GenMedProductID);
        //                            typeInfo.GetContraIndicatorDrugsRelToMedCondList(0, p.GenMedProductID);
        //                            typeInfo.GetRouteOfAdministrationList(0, p.GenMedProductID);
        //                            typeInfo.TitleForm = string.Format("{0} ", eHCMSResources.T1484_G1_HChinh) + TextType() + " : " + p.BrandName.Trim();
        //                        }
        //                        GlobalsNAV.ShowDialog<ICMDrug_AddEdit_V3>(onInitDlg_V3, null, false, true, Globals.GetDefaultDialogViewSize());
        //                    }
        //                    else
        //                    {
        //                        void onInitDlg(ICMDrug_AddEdit typeInfo)
        //                        {
        //                            typeInfo.ObjRefGenMedProductDetails_Current = ObjectCopier.DeepCopy(p);
        //                            typeInfo.SupplierGenMedProduct_LoadDrugIDNotPaging(p.GenMedProductID);
        //                            typeInfo.GetContraIndicatorDrugsRelToMedCondList(0, p.GenMedProductID);
        //                            typeInfo.TitleForm = string.Format("{0} ", eHCMSResources.T1484_G1_HChinh) + TextType() + " : " + p.BrandName.Trim();
        //                        }
        //                        GlobalsNAV.ShowDialog<ICMDrug_AddEdit>(onInitDlg, null, false, true, Globals.GetDefaultDialogViewSize());
        //                    }
        //                    break;
        //                }
        //            case (long)AllLookupValues.MedProductType.HOA_CHAT:
        //                {
        //                    if (Globals.ServerConfigSection.HealthInsurances.ApplyHINew_Report20150701 == true)
        //                    {
        //                        void onInitDlg(IDrugDept_MedDevAndChem_AddEdit typeInfo)
        //                        {
        //                            typeInfo.V_MedProductType = V_MedProductType;
        //                            typeInfo.ObjRefGenMedProductDetails_Current = ObjectCopier.DeepCopy(p);
        //                            typeInfo.IsShowDispenseVolume = true;
        //                            typeInfo.SupplierGenMedProduct_LoadDrugIDNotPaging(p.GenMedProductID);
        //                            typeInfo.TitleForm = string.Format("{0} ", eHCMSResources.T1484_G1_HChinh) + TextType() + " : " + p.BrandName.Trim();
        //                        }
        //                        GlobalsNAV.ShowDialog<IDrugDept_MedDevAndChem_AddEdit>(onInitDlg, null, false, true, Globals.GetDefaultDialogViewSize());
        //                    }
        //                    else
        //                    {
        //                        void onInitDlg(ICMMedicalDevices_Chemical_AddEdit typeInfo)
        //                        {
        //                            typeInfo.V_MedProductType = V_MedProductType;
        //                            typeInfo.ObjRefGenMedProductDetails_Current = ObjectCopier.DeepCopy(p);
        //                            typeInfo.IsShowDispenseVolume = true;
        //                            typeInfo.SupplierGenMedProduct_LoadDrugIDNotPaging(p.GenMedProductID);
        //                            typeInfo.TitleForm = string.Format("{0} ", eHCMSResources.T1484_G1_HChinh) + TextType() + " : " + p.BrandName.Trim();
        //                        }
        //                        GlobalsNAV.ShowDialog<ICMMedicalDevices_Chemical_AddEdit>(onInitDlg, null, false, true, Globals.GetDefaultDialogViewSize());
        //                    }
        //                    break;
        //                }
        //            //▼====: #002
        //            case (long)AllLookupValues.MedProductType.MAU:
        //                {
        //                    if (Globals.ServerConfigSection.HealthInsurances.ApplyHINew_Report20150701 == true)
        //                    {
        //                        void onInitDlg(IDrugDept_Blood_AddEdit typeInfo)
        //                        {
        //                            typeInfo.V_MedProductType = V_MedProductType;
        //                            typeInfo.ObjRefGenMedProductDetails_Current = ObjectCopier.DeepCopy(p);
        //                            typeInfo.IsShowDispenseVolume = true;
        //                            typeInfo.SupplierGenMedProduct_LoadDrugIDNotPaging(p.GenMedProductID);
        //                            typeInfo.TitleForm = string.Format("{0} ", eHCMSResources.T1484_G1_HChinh) + TextType() + " : " + p.BrandName.Trim();
        //                        }
        //                        GlobalsNAV.ShowDialog<IDrugDept_Blood_AddEdit>(onInitDlg, null, false, true, Globals.GetDefaultDialogViewSize());
        //                    }
        //                    else
        //                    {
        //                        void onInitDlg(ICMMedicalDevices_Chemical_AddEdit typeInfo)
        //                        {
        //                            typeInfo.V_MedProductType = V_MedProductType;
        //                            typeInfo.ObjRefGenMedProductDetails_Current = ObjectCopier.DeepCopy(p);
        //                            typeInfo.IsShowDispenseVolume = true;
        //                            typeInfo.SupplierGenMedProduct_LoadDrugIDNotPaging(p.GenMedProductID);
        //                            typeInfo.TitleForm = string.Format("{0} ", eHCMSResources.T1484_G1_HChinh) + TextType() + " : " + p.BrandName.Trim();
        //                        }
        //                        GlobalsNAV.ShowDialog<ICMMedicalDevices_Chemical_AddEdit>(onInitDlg, null, false, true, Globals.GetDefaultDialogViewSize());
        //                    }
        //                    break;
        //                }
        //            //▲====: #002
        //            case (long)AllLookupValues.MedProductType.Y_CU:
        //            case (long)AllLookupValues.MedProductType.VTYT_TIEUHAO:
        //            case (long)AllLookupValues.MedProductType.TIEM_NGUA:                    
        //            case (long)AllLookupValues.MedProductType.THANHTRUNG:
        //            case (long)AllLookupValues.MedProductType.VAN_PHONG_PHAM:
        //            case (long)AllLookupValues.MedProductType.VATTU_TIEUHAO:
        //                {
        //                    if (Globals.ServerConfigSection.HealthInsurances.ApplyHINew_Report20150701 == true)
        //                    {
        //                        void onInitDlg(IDrugDept_MedDevAndChem_AddEdit typeInfo)
        //                        {
        //                            typeInfo.V_MedProductType = V_MedProductType;
        //                            typeInfo.ObjRefGenMedProductDetails_Current = ObjectCopier.DeepCopy(p);
        //                            typeInfo.IsShowDispenseVolume = true;
        //                            typeInfo.IsShowMedicalMaterial = true;
        //                            typeInfo.SetRadioButton();
        //                            typeInfo.SupplierGenMedProduct_LoadDrugIDNotPaging(p.GenMedProductID);
        //                            typeInfo.TitleForm = string.Format("{0} ", eHCMSResources.T1484_G1_HChinh) + TextType() + " : " + p.BrandName.Trim();
        //                        }
        //                        GlobalsNAV.ShowDialog<IDrugDept_MedDevAndChem_AddEdit>(onInitDlg, null, false, true, Globals.GetDefaultDialogViewSize());
        //                    }
        //                    else
        //                    {
        //                        void onInitDlg(ICMMedicalDevices_Chemical_AddEdit typeInfo)
        //                        {
        //                            typeInfo.V_MedProductType = V_MedProductType;
        //                            typeInfo.ObjRefGenMedProductDetails_Current = ObjectCopier.DeepCopy(p);
        //                            typeInfo.IsShowDispenseVolume = true;
        //                            typeInfo.IsShowMedicalMaterial = true;
        //                            typeInfo.SetRadioButton();
        //                            typeInfo.SupplierGenMedProduct_LoadDrugIDNotPaging(p.GenMedProductID);
        //                            typeInfo.TitleForm = string.Format("{0} ", eHCMSResources.T1484_G1_HChinh) + TextType() + " : " + p.BrandName.Trim();
        //                        }
        //                        GlobalsNAV.ShowDialog<ICMMedicalDevices_Chemical_AddEdit>(onInitDlg, null, false, true, Globals.GetDefaultDialogViewSize());
        //                    }
        //                    break;
        //                }
        //        }
        //    }
        //}

        public void hplDelete_Click(object selectItem)
        {
            if (selectItem != null)
            {
                PCLExamTypeContactDrugs tempItem = (selectItem as PCLExamTypeContactDrugs);
                if(tempItem.PCLExamTypeContactDrugID != 0)
                {
                    ListPCLExamTypeContactDrug_Del.Add(tempItem);
                }
                else
                {
                    ListPCLExamTypeContactDrug_Add.Remove(tempItem);
                }
                ListPCLExamTypeContactDrugs.Remove(tempItem);
                //RefGenMedProductDetails p = (selectItem as RefGenMedProductDetails);

                //if (MessageBox.Show(string.Format(eHCMSResources.Z0476_G1_BanMuonXoa, p.BrandName), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                //{
                //    RefGenMedProductDetails_MarkDelete(p.GenMedProductID);
                //}
            }
        }

        //private void RefGenMedProductDetails_MarkDelete(Int64 GenMedProductID)
        //{
        //    this.ShowBusyIndicator(eHCMSResources.Z0492_G1_DangXoa);
        //    var t = new Thread(() =>
        //    {
        //        try
        //        {
        //            using (var serviceFactory = new PharmacyGenMedDeptDetailsServiceClient())
        //            {
        //                var contract = serviceFactory.ServiceInstance;
        //                contract.BeginRefGenMedProductDetails_MarkDelete(GenMedProductID, Globals.DispatchCallback((asyncResult) =>
        //                {
        //                    try
        //                    {
        //                        string Res = "";
        //                        contract.EndRefGenMedProductDetails_MarkDelete(out Res, asyncResult);
        //                        switch (Res)
        //                        {
        //                            case "Delete-0":
        //                                {
        //                                    MessageBox.Show(eHCMSResources.A0478_G1_Msg_InfoXoaOK, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
        //                                    break;
        //                                }
        //                            case "Delete-1":
        //                                {
        //                                    //ObjRefGenMedProductDetails_Paging.PageIndex = 0;
        //                                    RefGenMedProductDetails_Paging(ObjRefGenMedProductDetails_Paging.PageIndex, ObjRefGenMedProductDetails_Paging.PageSize, true);
        //                                    MessageBox.Show(eHCMSResources.A0478_G1_Msg_InfoXoaOK, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
        //                                    break;
        //                                }
        //                        }
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
        //                    }
        //                    finally
        //                    {
        //                        this.HideBusyIndicator();
        //                    }
        //                }), null);
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            this.HideBusyIndicator();
        //            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
        //        }
        //    });
        //    t.Start();
        //}

        //public void Handle(Drug_AddEditViewModel_Save_Event message)
        //{
        //    if (message != null)
        //    {
        //        RefGenMedProductDetails_Paging(ObjRefGenMedProductDetails_Paging.PageIndex, ObjRefGenMedProductDetails_Paging.PageSize, true);
        //    }
        //}

        private string TextType()
        {
            string Result = "";
            switch (V_MedProductType)
            {
                case (long)AllLookupValues.MedProductType.THUOC:
                    {
                        Result = eHCMSResources.G0787_G1_Thuoc;
                        break;
                    }
                case (long)AllLookupValues.MedProductType.Y_CU:
                    {
                        Result = eHCMSResources.G2907_G1_YCu;
                        break;
                    }
                case (long)AllLookupValues.MedProductType.HOA_CHAT:
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

        public void griddrug_DblClick(object sender, EventArgs<object> e)
        {
            //if (IsPopUp)
            //{
            //    TryClose();
            //    Globals.EventAggregator.Publish(new DrugDeptCloseSearchDrugEvent() { SupplierDrug = e.Value });
            //}
            AddDrugToPCL(ObjRefGenMedProductDetails_Selected);
        }

        private void AddDrugToPCL(RefGenMedProductDetails refGenMed)
        {
            CheckValidBeforeAdd(refGenMed);
            PCLExamTypeContactDrugs tempItem = new PCLExamTypeContactDrugs
            {
                PCLExamTypeID = PCLExamTypeID,
                GenMedProductID = refGenMed.GenMedProductID,
                GenMedProduct = refGenMed
            };
            ListPCLExamTypeContactDrugs.Add(tempItem);
            ListPCLExamTypeContactDrug_Add.Add(tempItem);
        }
        private void CheckValidBeforeAdd(RefGenMedProductDetails refGenMed)
        {
            foreach (var item in ListPCLExamTypeContactDrugs)
            {
                if(item.GenMedProductID  == refGenMed.GenMedProductID)
                {
                    MessageBox.Show("Đã có thuốc này rồi");
                    return;
                }
            }
        }
        private bool _ShowCatDrugType = false;
        public bool ShowCatDrugType
        {
            get { return _ShowCatDrugType; }
            set
            {
                _ShowCatDrugType = value;
                NotifyOfPropertyChange(() => ShowCatDrugType);
            }
        }

        public enum Insurance
        {
            All = 1,
            Yes = 2,
            No = 3
        }

        public enum Consult
        {
            All = 1,
            Yes = 2,
            No = 3
        }

        public enum DIsActive
        {
            All = 2,
            Yes = 0,
            No = 1
        }

        public enum CatDrugType
        {
            All = 0,
            Shared = 1,
            Pharmacy = 2,
            DrugDept = 3
        }

        private byte IsCatDrugType = (byte)CatDrugType.All;
        private byte IsInsurance = (byte)Insurance.All;
        private byte IsConsult = (byte)Consult.All;
        private new byte IsActive = (byte)DIsActive.Yes;
        public void CatV_MedProductType1_Checked(object sender, RoutedEventArgs e)
        {
            V_MedProductType = (long)AllLookupValues.MedProductType.THUOC;
        }

        public void CatV_MedProductType2_Checked(object sender, RoutedEventArgs e)
        {
            V_MedProductType = (long)AllLookupValues.MedProductType.Y_CU;
        }

        //public void CatDrugType1_Checked(object sender, RoutedEventArgs e)
        //{
        //    IsCatDrugType = (byte)CatDrugType.All;
        //}

        //public void CatDrugType2_Checked(object sender, RoutedEventArgs e)
        //{
        //    IsCatDrugType = (byte)CatDrugType.Shared;
        //}

        //public void CatDrugType3_Checked(object sender, RoutedEventArgs e)
        //{
        //    IsCatDrugType = (byte)CatDrugType.Pharmacy;
        //}

        //public void CatDrugType4_Checked(object sender, RoutedEventArgs e)
        //{
        //    IsCatDrugType = (byte)CatDrugType.DrugDept;
        //}

        private string strBH = "";
        //public void IsInsurance1_Checked(object sender, RoutedEventArgs e)
        //{
        //    IsInsurance = (byte)Insurance.All;
        //    strBH = "";
        //}

        //public void IsInsurance2_Checked(object sender, RoutedEventArgs e)
        //{
        //    IsInsurance = (byte)Insurance.Yes;
        //    strBH = eHCMSResources.K0791_G1_1BHYT.ToUpper();
        //}

        //public void IsInsurance3_Checked(object sender, RoutedEventArgs e)
        //{
        //    IsInsurance = (byte)Insurance.No;
        //    strBH = eHCMSResources.T2455_G1_KhongBH;
        //}

        private string strHoiChan = "";
        //public void IsConsult1_Checked(object sender, RoutedEventArgs e)
        //{
        //    IsConsult = (byte)Consult.All;
        //    strHoiChan = "";
        //}

        //public void IsConsult2_Checked(object sender, RoutedEventArgs e)
        //{
        //    IsConsult = (byte)Consult.Yes;
        //    strHoiChan = eHCMSResources.Z0049_G1_CanHoiChan;
        //}

        //public void IsConsult3_Checked(object sender, RoutedEventArgs e)
        //{
        //    IsConsult = (byte)Consult.No;
        //    strHoiChan = eHCMSResources.T2457_G1_KhongCanHChan;
        //}

        //public void IsActive1_Checked(object sender, RoutedEventArgs e)
        //{
        //    IsActive = (byte)DIsActive.All;
        //}

        //public void IsActive2_Checked(object sender, RoutedEventArgs e)
        //{
        //    IsActive = (byte)DIsActive.Yes;
        //}

        //public void IsActive3_Checked(object sender, RoutedEventArgs e)
        //{
        //    IsActive = (byte)DIsActive.No;
        //}

        //public void btnExportExcel()
        //{
        //    if (SearchCriteria == null)
        //    {
        //        return;
        //    }

        //    SaveFileDialog objSFD = new SaveFileDialog()
        //    {
        //        DefaultExt = ".xls",
        //        Filter = "Excel xls (*.xls)|*.xls",
        //        FilterIndex = 1
        //    };
        //    if (objSFD.ShowDialog() != true)
        //    {
        //        return;
        //    }

        //    ReportParameters RptParameters = new ReportParameters();
        //    RptParameters.ReportType = ReportType.DANH_MUC_KHOA_DUOC;

        //    RptParameters.SearchRefGenMedProduct = SearchCriteria;

        //    RptParameters.Show = "DanhMuc";

        //    switch (RptParameters.SearchRefGenMedProduct.V_MedProductType)
        //    {
        //        case (long)AllLookupValues.MedProductType.THUOC:
        //            RptParameters.Show = "DanhMucThuoc";
        //            break;
        //        case (long)AllLookupValues.MedProductType.Y_CU:
        //            RptParameters.Show = "DanhMucYCu";
        //            break;
        //        case (long)AllLookupValues.MedProductType.HOA_CHAT:
        //            RptParameters.Show = "DanhMucHoaChat";
        //            break;
        //        case (long)AllLookupValues.MedProductType.VTYT_TIEUHAO:
        //            RptParameters.Show = "DanhMucVTYTTH";
        //            break;
        //        case (long)AllLookupValues.MedProductType.TIEM_NGUA:
        //            RptParameters.Show = "DanhMucTiemNgua";
        //            break;
        //        case (long)AllLookupValues.MedProductType.MAU:
        //            RptParameters.Show = "DanhMucMau";
        //            break;
        //        case (long)AllLookupValues.MedProductType.VAN_PHONG_PHAM:
        //            RptParameters.Show = "DanhMucVPP";
        //            break;
        //        case (long)AllLookupValues.MedProductType.VATTU_TIEUHAO:
        //            RptParameters.Show = "DanhMucVTTH";
        //            break;
        //        case (long)AllLookupValues.MedProductType.THANHTRUNG:
        //            RptParameters.Show = "DanhMucThanhTrung";
        //            break;
        //        case (long)AllLookupValues.MedProductType.NUTRITION:
        //            RptParameters.Show = "DanhMucDinhDuong";
        //            break;

        //    }
        //    ExportToExcelGeneric.Action(RptParameters, objSFD, this);
        //}

        /*▼====: #001*/
        public void GetParRefGeneric()
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyDrugServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetParRefGeneric(Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndGetParRefGeneric(asyncResult);
                                if (results != null)
                                {
                                    if (LstRefGeneric == null)
                                    {
                                        LstRefGeneric = new ObservableCollection<RefGeneric>();
                                    }
                                    else
                                    {
                                        LstRefGeneric.Clear();
                                    }
                                    LstRefGeneric = results.ToObservableCollection();
                                    LstRefGeneric.Insert(0, new RefGeneric { GenericID = 0, GenericName = "" });
                                }
                            }
                            catch (Exception ex)
                            {
                                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                            }
                            finally
                            {
                                this.HideBusyIndicator();
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    this.HideBusyIndicator();
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                }
            });

            t.Start();
        }
        /*▲====: #001*/

        public void btSaveItems()
        {
            if(ListPCLExamTypeContactDrug_Add.Count == 0 && ListPCLExamTypeContactDrug_Del.Count == 0)
            {
                return;
            }
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyGenMedDeptDetailsServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginSaveListPCLExamTypeContactDrugs(ListPCLExamTypeContactDrug_Add.ToList(), ListPCLExamTypeContactDrug_Del.ToList(), (long)Globals.LoggedUserAccount.StaffID, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndSaveListPCLExamTypeContactDrugs(asyncResult);
                                if (results)
                                {
                                    ListPCLExamTypeContactDrugs_Paging(0, ListPCLExamTypeContactDrugs.PageSize, true);
                                    MessageBox.Show("Lưu thành công");
                                    //if (LstRefGeneric == null)
                                    //{
                                    //    LstRefGeneric = new ObservableCollection<RefGeneric>();
                                    //}
                                    //else
                                    //{
                                    //    LstRefGeneric.Clear();
                                    //}
                                    //LstRefGeneric = results.ToObservableCollection();
                                    //LstRefGeneric.Insert(0, new RefGeneric { GenericID = 0, GenericName = "" });
                                }
                                else
                                {
                                    MessageBox.Show("Lưu thất bại");
                                }
                            }
                            catch (Exception ex)
                            {
                                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                            }
                            finally
                            {
                                this.HideBusyIndicator();
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    this.HideBusyIndicator();
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                }
            });

            t.Start();
        }
    }
}
