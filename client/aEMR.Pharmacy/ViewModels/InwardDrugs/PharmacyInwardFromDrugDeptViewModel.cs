using eHCMSLanguage;
using System.ComponentModel.Composition;
using aEMR.ViewContracts;
using Caliburn.Micro;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System;
using aEMR.Infrastructure;
using System.Collections.ObjectModel;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using System.Threading;
using DataEntities;
using aEMR.Common.Collections;
using System.Linq;
using System.Collections.Generic;
using aEMR.Common;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using aEMR.CommonTasks;

namespace aEMR.Pharmacy.ViewModels.InwardDrugs
{
    [Export(typeof(IPharmacyInwardFromDrugDept)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class PharmacyInwardFromDrugDeptViewModel : Conductor<object>, IPharmacyInwardFromDrugDept
        , IHandle<ClinicDrugDeptCloseSearchInwardIncoiceEvent>
    {
        private long TypID = (long)AllLookupValues.RefOutputType.NHAP_TU_KHO_DUOC_CHO_KHO_BHYT_NHA_THUOC;

        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public PharmacyInwardFromDrugDeptViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;


            SearchCriteria = new InwardInvoiceSearchCriteria();
            InitInvoice();

            Coroutine.BeginExecute(DoGetStore_STORAGE_HIDRUGs());
        }
        private IEnumerator<IResult> DoGetStore_STORAGE_HIDRUGs()
        {
            var paymentTypeTask = new LoadStoreListTask((long)AllLookupValues.StoreType.STORAGE_HIDRUGs, false, null, false, false);
            yield return paymentTypeTask;
            StoreCbx = paymentTypeTask.LookupList;
            yield break;
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            Globals.EventAggregator.Subscribe(this);

        }

        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);
            Globals.EventAggregator.Unsubscribe(this);
        }

        private void InitInvoice()
        {
            InwardDrugInvoice = null;
            InwardDrugInvoice = new InwardDrugClinicDeptInvoice();
            InwardDrugInvoice.InvDateInvoice = DateTime.Now;
            InwardDrugInvoice.DSPTModifiedDate = DateTime.Now;
            InwardDrugInvoice.CurrencyID = 129;
            InwardDrugInvoice.TypID = TypID;
            if (InwardDrugInvoice.SelectedStaff == null)
            {
                InwardDrugInvoice.SelectedStaff = new Staff();
            }
            InwardDrugInvoice.SelectedStaff.FullName = GetStaffLogin().FullName;
            InwardDrugInvoice.StaffID = GetStaffLogin().StaffID;
            MedDeptInvoiceList = new ObservableCollection<OutwardDrugMedDeptInvoice>();
            IsEnableSaveBtn = false; //--28/01/2021 DatTB Apply cách của A.Vũ
        }

        //KMx: Để Unsubcribe thì khi chuyển từ View Nhập Thuốc, sang Nhập Y Cụ thì double click vào tìm phiếu xuất, bắn event về đây không được (27/12/2014 16:57).
        //protected override void OnDeactivate(bool close)
        //{
        //    Globals.EventAggregator.Unsubscribe(this);
        //}

        #region Properties Member
        private bool _ShowInCost = Globals.ServerConfigSection.CommonItems.ShowInCostInInternalInwardPharmacy;
        public bool ShowInCost
        {
            get
            {
                return _ShowInCost;
            }
            set
            {
                _ShowInCost = value;
                NotifyOfPropertyChange(() => ShowInCost);
            }
        }

        private string _strHienThi;
        public string strHienThi
        {
            get
            {
                return _strHienThi;
            }
            set
            {
                _strHienThi = value;
                NotifyOfPropertyChange(() => strHienThi);
            }
        }

        private InwardInvoiceSearchCriteria _searchCriteria;
        public InwardInvoiceSearchCriteria SearchCriteria
        {
            get
            {
                return _searchCriteria;
            }
            set
            {
                if (_searchCriteria != value)
                {
                    _searchCriteria = value;
                    NotifyOfPropertyChange(() => SearchCriteria);
                }
            }
        }

        private int _CDC = 5;
        public int CDC
        {
            get
            {
                return _CDC;
            }
            set
            {
                _CDC = value;
                NotifyOfPropertyChange(() => CDC);
            }
        }

        private InwardDrugClinicDeptInvoice _InwardDrugInvoice;
        public InwardDrugClinicDeptInvoice InwardDrugInvoice
        {
            get
            {
                return _InwardDrugInvoice;
            }
            set
            {
                if (_InwardDrugInvoice != value)
                {
                    _InwardDrugInvoice = value;

                    NotifyOfPropertyChange(() => InwardDrugInvoice);
                }

            }
        }

        //--▼--28/01/2021 DatTB Apply cách của A.Vũ
        private bool _IsEnableSaveBtn;
        public bool IsEnableSaveBtn
        {
            get
            {
                return _IsEnableSaveBtn;
            }
            set
            {
                if (_IsEnableSaveBtn != value)
                {
                    _IsEnableSaveBtn = value;
                    NotifyOfPropertyChange(() => IsEnableSaveBtn);
                }

            }
        }
        //--▲--28/01/2021 DatTB Apply cách của A.Vũ

        private ObservableCollection<RefStorageWarehouseLocation> _StoreCbx;
        public ObservableCollection<RefStorageWarehouseLocation> StoreCbx
        {
            get
            {
                return _StoreCbx;
            }
            set
            {
                if (_StoreCbx != value)
                {
                    _StoreCbx = value;
                    NotifyOfPropertyChange(() => StoreCbx);
                }
            }
        }

        private long _V_MedProductType = (long)AllLookupValues.MedProductType.THUOC;
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

        private ObservableCollection<OutwardDrugMedDeptInvoice> _MedDeptInvoiceList;
        public ObservableCollection<OutwardDrugMedDeptInvoice> MedDeptInvoiceList
        {
            get
            {
                return _MedDeptInvoiceList;
            }
            set
            {
                if (_MedDeptInvoiceList != value)
                {
                    _MedDeptInvoiceList = value;

                    NotifyOfPropertyChange(() => MedDeptInvoiceList);
                }

            }
        }

        private InwardDrugClinicDept _CurrentInwardDrugClinicDept;
        public InwardDrugClinicDept CurrentInwardDrugClinicDept
        {
            get
            {
                return _CurrentInwardDrugClinicDept;
            }
            set
            {
                if (_CurrentInwardDrugClinicDept != value)
                {
                    _CurrentInwardDrugClinicDept = value;

                    NotifyOfPropertyChange(() => CurrentInwardDrugClinicDept);
                }

            }
        }

        #endregion

        private Staff GetStaffLogin()
        {
            return Globals.LoggedUserAccount.Staff;
        }

        private void LoadMedDeptInvoiceList(long? StoreID)
        {
            IsEnableSaveBtn = false;
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyMedDeptServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginOutwardDrugMedDeptInvoice_Cbx(StoreID, V_MedProductType, false, Globals.DispatchCallback((asyncResult) =>
                         {
                             try
                             {
                                 var results = contract.EndOutwardDrugMedDeptInvoice_Cbx(asyncResult);
                                 MedDeptInvoiceList = results.ToObservableCollection();
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
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    this.HideBusyIndicator();
                }
            });

            t.Start();
        }

        private void LoadMedDeptInvoiceList_V2(long? StoreID)
        {
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyMedDeptServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginOutwardDrugMedDeptInvoice_Cbx_V2(StoreID, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndOutwardDrugMedDeptInvoice_Cbx_V2(asyncResult);
                                MedDeptInvoiceList = results.ToObservableCollection();
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
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    this.HideBusyIndicator();
                }
            });

            t.Start();
        }

        public void cbxKho_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (InwardDrugInvoice != null && InwardDrugInvoice.CanSave)
            {
                if (InwardDrugInvoice.SelectedStorage != null)
                {
                    LoadMedDeptInvoiceList_V2(InwardDrugInvoice.SelectedStorage.StoreID);

                    InwardDrugInvoice.CurrentOutwardDrugMedDeptInvoice = null;
                }
                else
                {
                    if (MedDeptInvoiceList != null)
                    {
                        MedDeptInvoiceList = null;
                    }
                }
            }
        }

        public void cbxOutInvID_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (InwardDrugInvoice != null && InwardDrugInvoice.CanSave)
            {
                //--▼--28/01/2021 DatTB Apply cách của A.Vũ
                InwardDrugInvoice.InwardDrugs = null;
                IsEnableSaveBtn = false;
                //--▲--28/01/2021 DatTB Apply cách của A.Vũ
                
                if (InwardDrugInvoice.CurrentOutwardDrugMedDeptInvoice != null)
                {
                    if (InwardDrugInvoice.CurrentOutwardDrugMedDeptInvoice.FromClinicDept)
                    {
                        InwardDrugInvoice.outiID_Clinic = InwardDrugInvoice.CurrentOutwardDrugMedDeptInvoice.outiID;
                    }
                    else
                    {
                        InwardDrugInvoice.outiID = InwardDrugInvoice.CurrentOutwardDrugMedDeptInvoice.outiID;
                    }
                    OutwardDrugMedDeptDetails_Load(InwardDrugInvoice.CurrentOutwardDrugMedDeptInvoice.outiID, InwardDrugInvoice.CurrentOutwardDrugMedDeptInvoice.FromClinicDept);
                }
                else
                {
                    if (InwardDrugInvoice.InwardDrugs != null)
                    {
                        InwardDrugInvoice.InwardDrugs = null;
                    }
                }
            }
        }

        private void OutwardDrugMedDeptDetails_Load(long outiID, bool FromClinicDept)
        {
            InwardDrugInvoice.InwardDrugs = null;
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyMedDeptServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetOutwardDrugMedDeptDetailByInvoice(outiID, V_MedProductType, FromClinicDept, Globals.DispatchCallback((asyncResult) =>
                        {

                            try
                            {
                                var results = contract.EndGetOutwardDrugMedDeptDetailByInvoice(asyncResult);
                                if (results != null && results.Count > 0)
                                {
                                    InwardDrugInvoice.InwardDrugs = new ObservableCollection<InwardDrugClinicDept>();
                                    foreach (OutwardDrugMedDept p in results)
                                    {
                                        InwardDrugClinicDept item = new InwardDrugClinicDept();
                                        item.RefGenMedProductDetails = p.RefGenericDrugDetail;
                                        if (p.InwardDrugMedDept != null)
                                        {
                                            item.InProductionDate = p.InwardDrugMedDept.InProductionDate;
                                            item.InExpiryDate = p.InwardDrugMedDept.InExpiryDate;
                                            item.InBatchNumber = p.InwardDrugMedDept.InBatchNumber;
                                            item.V_GoodsType = p.InwardDrugMedDept.V_GoodsType;
                                            item.InBuyingPrice = p.InwardDrugMedDept.InBuyingPrice;
                                            item.NormalPrice = p.InwardDrugMedDept.NormalPrice;
                                            item.HIPatientPrice = p.InwardDrugMedDept.HIPatientPrice;
                                            item.HIAllowedPrice = p.InwardDrugMedDept.HIAllowedPrice;
                                            item.GenMedVersionID = p.InwardDrugMedDept.GenMedVersionID;
                                            item.InBuyingPrice = p.InwardDrugMedDept.InBuyingPrice;
                                            item.InCost = p.InwardDrugMedDept.InCost;
                                            item.InBuyingPriceActual = p.InwardDrugMedDept.InCost;
                                            item.VAT = p.InwardDrugMedDept.VAT;
                                            item.IsNotVat = p.InwardDrugMedDept.IsNotVat;
                                        }
                                        item.InQuantity = p.OutQuantity;
                                        item.MedDeptQty = p.OutQuantity;
                                        item.SdlDescription = "";
                                        item.OutID = p.OutID;
                                        InwardDrugInvoice.InwardDrugs.Add(item);
                                    }
                                    CountTotalPrice();
                                    IsEnableSaveBtn = true; //--28/01/2021 DatTB Apply cách của A.Vũ
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
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    this.HideBusyIndicator();
                }
            });

            t.Start();
        }

        //private void GetInwardInvoiceDrugByID(long ID)
        //{
        //    this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
        //    var t = new Thread(() =>
        //    {
        //        try
        //        {
        //            using (var serviceFactory = new PharmacyClinicDeptServiceClient())
        //            {
        //                var contract = serviceFactory.ServiceInstance;
        //                contract.BeginGetInwardDrugClinicDeptInvoice_ByID(ID, Globals.DispatchCallback((asyncResult) =>
        //                {

        //                    try
        //                    {
        //                        InwardDrugInvoice = contract.EndGetInwardDrugClinicDeptInvoice_ByID(asyncResult);
        //                        if (InwardDrugInvoice != null)
        //                        {
        //                            InwardDrugDetails_ByID(InwardDrugInvoice.inviID);
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
        //            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
        //            this.HideBusyIndicator();
        //        }
        //    });

        //    t.Start();
        //}

        private void InwardDrugClinicDeptInvoice_SaveXML()
        {
            DateTime? DSPTModifiedDate = null;
            if (InwardDrugInvoice.CurrentOutwardDrugMedDeptInvoice != null)
            {
                DSPTModifiedDate = InwardDrugInvoice.CurrentOutwardDrugMedDeptInvoice.DSPTModifiedDate;
            }
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyClinicDeptServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginInwardDrugInvoice_SaveXML(InwardDrugInvoice, DSPTModifiedDate, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                int results = contract.EndInwardDrugInvoice_SaveXML(out long inviID, asyncResult);
                                if (results > 0)
                                {
                                    Globals.ShowMessage(eHCMSResources.T0074_G1_I, eHCMSResources.T0074_G1_I);
                                }
                                else
                                {
                                    IsEnableSaveBtn = false;
                                    if (inviID > 0)
                                    {
                                        Globals.ShowMessage(eHCMSResources.A0468_G1_Msg_InfoLuuOK, eHCMSResources.G0442_G1_TBao);
                                        GetInwardInvoiceDrugByID(inviID);
                                    }
                                    else
                                    {
                                        Globals.ShowMessage(eHCMSResources.K2782_G1_DaCNhat, eHCMSResources.G0442_G1_TBao);
                                        if (InwardDrugInvoice != null)
                                        {
                                            GetInwardInvoiceDrugByID(InwardDrugInvoice.inviID);
                                        }
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                IsEnableSaveBtn = false;
                                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                            }
                            finally
                            {
                                IsEnableSaveBtn = false;
                                this.HideBusyIndicator();
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    IsEnableSaveBtn = false;
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    this.HideBusyIndicator();
                }
            });

            t.Start();
        }

        public void btnSave()
        {
            if (InwardDrugInvoice.SelectedStorage == null)
            {
                Globals.ShowMessage(eHCMSResources.Z0566_G1_ChuaChonKhoNhap, eHCMSResources.G0442_G1_TBao);
                return;
            }
            if (InwardDrugInvoice.SelectedStorage.StoreID <= 0)
            {
                Globals.ShowMessage(eHCMSResources.Z0566_G1_ChuaChonKhoNhap, eHCMSResources.G0442_G1_TBao);
                return;
            }

            if (InwardDrugInvoice.InwardDrugs != null && InwardDrugInvoice.InwardDrugs.Count > 0)
            {
                InwardDrugInvoice.V_MedProductType = V_MedProductType;
                if (InwardDrugInvoice.SelectedStorageOut != null)
                {
                    InwardDrugInvoice.StoreIDOut = InwardDrugInvoice.SelectedStorageOut.StoreID;
                }
                if (InwardDrugInvoice.SelectedStorage != null)
                {
                    InwardDrugInvoice.StoreID = InwardDrugInvoice.SelectedStorage.StoreID;
                }
                if (MessageBox.Show("Vui lòng kiểm tra nội dung phiếu nhập! Bạn muốn nhập phiếu này?",
                    eHCMSResources.G0442_G1_TBao, MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    InwardDrugClinicDeptInvoice_SaveXML();
                }
            }
            else
            {
                if (V_MedProductType == (long)AllLookupValues.MedProductType.THUOC)
                {
                    Globals.ShowMessage(eHCMSResources.Z1093_G1_Chua_NhapThuoc, eHCMSResources.G0442_G1_TBao);
                }
                else if (V_MedProductType == (long)AllLookupValues.MedProductType.Y_CU)
                {
                    Globals.ShowMessage(eHCMSResources.Z1094_G1_ChuaNhapYCu, eHCMSResources.G0442_G1_TBao);
                }
                else
                {
                    Globals.ShowMessage(eHCMSResources.Z1095_G1_ChuaNhapHChat, eHCMSResources.G0442_G1_TBao);
                }
            }
        }

        public void btnAcceptAutoUpdate()
        {
            this.ShowBusyIndicator();

            if (InwardDrugInvoice == null || InwardDrugInvoice.inviID <= 0)
            {
                this.HideBusyIndicator();
                MessageBox.Show(eHCMSResources.A0659_G1_Msg_InfoKhCoPhNhap, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }

            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyClinicDeptServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginAcceptAutoUpdateInwardClinicInvoice(InwardDrugInvoice.inviID, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                bool result = contract.EndAcceptAutoUpdateInwardClinicInvoice(asyncResult);
                                if (result)
                                {
                                    MessageBox.Show(eHCMSResources.A0286_G1_Msg_InfoChNhanChoK_DcCNhatOK, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                    GetInwardInvoiceDrugByID(InwardDrugInvoice.inviID);
                                }
                                else
                                {
                                    MessageBox.Show(eHCMSResources.A0287_G1_Msg_InfoChNhanChoK_DcCNhatFail, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message);
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
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    this.HideBusyIndicator();
                }
            });

            t.Start();
        }

        DataGrid GridSuppliers = null;
        public void GridInwardDrug_Loaded(object sender, RoutedEventArgs e)
        {
            GridSuppliers = sender as DataGrid;
        }

        public void btnNew()
        {
            InitInvoice();
        }

        public void tbx_Search_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (SearchCriteria != null)
                {
                    SearchCriteria.InwardID = (sender as TextBox).Text;
                }
                btnSearch();
            }
        }

        public void btnSearch()
        {
            SearchInwardInvoiceDrug(0, Globals.PageSize);
        }

        public void OpenPopUpSearchInwardInvoice(IList<InwardDrugClinicDeptInvoice> results, int Totalcount)
        {

            void onInitDlg(IPharmacyInwardFromDrugDeptSearch proAlloc)
            {
                proAlloc.SearchCriteria = SearchCriteria.DeepCopy();
                proAlloc.TypID = TypID;
                proAlloc.V_MedProductType = V_MedProductType;
                proAlloc.InwardInvoiceList.Clear();
                proAlloc.InwardInvoiceList.TotalItemCount = Totalcount;
                proAlloc.InwardInvoiceList.PageIndex = 0;
                if (results != null && results.Count > 0)
                {
                    foreach (InwardDrugClinicDeptInvoice p in results)
                    {
                        proAlloc.InwardInvoiceList.Add(p);
                    }
                }
            }

            GlobalsNAV.ShowDialog<IPharmacyInwardFromDrugDeptSearch>(onInitDlg);
        }

        private void SearchInwardInvoiceDrug(int PageIndex, int PageSize)
        {
            if (SearchCriteria == null)
            {
                return;
            }

            //KMx: Hiện tại không có thời gian làm combobox cho user chọn khoa, nên mặc định lấy khoa trách nhiệm đầu tiên của user để tìm kiếm.
            //Khi nào có thời gian thì làm combobox chọn khoa (21/01/2015 14:18).
            if (!Globals.isAccountCheck)
            {
                SearchCriteria.InDeptID = 0;
            }
            else
            {
                if (Globals.LoggedUserAccount.DeptIDResponsibilityList == null || Globals.LoggedUserAccount.DeptIDResponsibilityList.Count <= 0)
                {
                    MessageBox.Show(eHCMSResources.A0107_G1_Msg_InfoKhTheTimKiem, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                    return;
                }
                SearchCriteria.InDeptID = Globals.LoggedUserAccount.DeptIDResponsibilityList.FirstOrDefault();
            }
            

            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);

            if (string.IsNullOrEmpty(SearchCriteria.InwardID))
            {
                SearchCriteria.FromDate = Globals.GetCurServerDateTime().AddDays(-7);
                SearchCriteria.ToDate = Globals.GetCurServerDateTime();
            }
            else
            {
                SearchCriteria.FromDate = null;
                SearchCriteria.ToDate = null;
            }

            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyClinicDeptServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginSearchInwardDrugInvoiceForPharmacy(SearchCriteria, TypID, PageIndex, PageSize, true, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndSearchInwardDrugInvoiceForPharmacy(out int Totalcount, asyncResult);
                                if (results != null && results.Count > 0)
                                {
                                    if (results.Count > 1)
                                    {
                                    //mo pop up tim
                                    OpenPopUpSearchInwardInvoice(results.ToList(), Totalcount);
                                    }
                                    else
                                    {
                                        InwardDrugInvoice = results.FirstOrDefault();
                                        InwardDrugDetails_ByID(InwardDrugInvoice.inviID);
                                    }
                                }
                                else
                                {
                                    if (MessageBox.Show(eHCMSResources.Z0568_G1_KgTimThay, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                                    {
                                        SearchCriteria.FromDate = Globals.GetCurServerDateTime().AddDays(-7);
                                        SearchCriteria.ToDate = Globals.GetCurServerDateTime();
                                        OpenPopUpSearchInwardInvoice(null, 0);
                                    }
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
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    this.HideBusyIndicator();
                }
            });

            t.Start();
        }

        private void CountTotalPrice()
        {
            if (InwardDrugInvoice != null && InwardDrugInvoice.InwardDrugs != null)
            {
                InwardDrugInvoice.TotalPrice = 0;
                for (int i = 0; i < InwardDrugInvoice.InwardDrugs.Count; i++)
                {
                    InwardDrugInvoice.TotalPrice += InwardDrugInvoice.InwardDrugs[i].InBuyingPriceActual * InwardDrugInvoice.InwardDrugs[i].InQuantity;
                }
            }
        }

        private void InwardDrugDetails_ByID(long ID)
        {
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyClinicDeptServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetInwardDrugPharmacy_ByIDInvoiceNotPaging(ID, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndGetInwardDrugPharmacy_ByIDInvoiceNotPaging(out decimal TongTienSPChuaVAT
                                    , out decimal CKTrenSP, out decimal TongTienTrenSPDaTruCK, out decimal TongCKTrenHoaDon, out decimal TongTienHoaDonCoVAT, asyncResult);
                                InwardDrugInvoice.InwardDrugs = results.ToObservableCollection();
                                CountTotalPrice();
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
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    this.HideBusyIndicator();
                }
            });

            t.Start();
        }

        #region Auto For Location

        private void SetValueSdlDescription(string Name, object item)
        {
            InwardDrugClinicDept P = item as InwardDrugClinicDept;
            P.SdlDescription = Name;
        }

        AutoCompleteBox Au;

        private void SearchRefShelfLocation(string Name)
        {
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyInwardDrugServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetRefShelfDrugLocationAutoComplete(Name, 0, Globals.PageSize, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            var results = contract.EndGetRefShelfDrugLocationAutoComplete(asyncResult);
                            Au.ItemsSource = results.ToObservableCollection();
                            Au.PopulateComplete();
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                        }
                    }), null);
                }
            });

            t.Start();
        }

        public void AutoLocation_Text_Populating(object sender, PopulatingEventArgs e)
        {
            Au = sender as AutoCompleteBox;
            SearchRefShelfLocation(e.Parameter);
            if (GridSuppliers != null && GridSuppliers.SelectedItem != null)
            {
                SetValueSdlDescription(e.Parameter, GridSuppliers.SelectedItem);
            }
        }

        public void AutoLocation_Tex_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Au != null && CurrentInwardDrugClinicDept != null)
            {
                if (Au.SelectedItem != null)
                {
                    CurrentInwardDrugClinicDept.SelectedShelfDrugLocation = Au.SelectedItem as RefShelfDrugLocation;
                }
                else
                {
                    CurrentInwardDrugClinicDept.SdlDescription = Au.Text;
                }
            }
        }

        #endregion

        #region IHandle<ClinicDrugDeptCloseSearchInwardIncoiceEvent> Members

        public void Handle(ClinicDrugDeptCloseSearchInwardIncoiceEvent message)
        {
            if (message != null && this.IsActive)
            {
                InwardDrugInvoice = message.SelectedInwardInvoice as InwardDrugClinicDeptInvoice;
                InwardDrugDetails_ByID(InwardDrugInvoice.inviID);

            }
        }

        #endregion

        #region printing member

        public void btnPreview()
        {
            IReportDocumentPreview DialogView = Globals.GetViewModel<IReportDocumentPreview>();
            DialogView.ID = InwardDrugInvoice.inviID;
            DialogView.eItem = ReportName.Pharmacy_InwardFromInternal;
            GlobalsNAV.ShowDialog_V3(DialogView, null, null, false, true, Globals.GetDefaultDialogViewSize());
        }

        #endregion

        public void Authorization()
        {
            if (!Globals.isAccountCheck)
            {
                return;
            }
        }

        #region checking account
        private bool _mNhapHangTuKhoDuoc_Tim = true;
        private bool _mNhapHangTuKhoDuoc_Them = true;
        private bool _mNhapHangTuKhoDuoc_XemIn = true;
        private bool _mNhapHangTuKhoDuoc_In = true;

        public bool mNhapHangTuKhoDuoc_Tim
        {
            get
            {
                return _mNhapHangTuKhoDuoc_Tim;
            }
            set
            {
                if (_mNhapHangTuKhoDuoc_Tim == value)
                    return;
                _mNhapHangTuKhoDuoc_Tim = value;
                NotifyOfPropertyChange(() => mNhapHangTuKhoDuoc_Tim);
            }
        }

        public bool mNhapHangTuKhoDuoc_Them
        {
            get
            {
                return _mNhapHangTuKhoDuoc_Them;
            }
            set
            {
                if (_mNhapHangTuKhoDuoc_Them == value)
                    return;
                _mNhapHangTuKhoDuoc_Them = value;
                NotifyOfPropertyChange(() => mNhapHangTuKhoDuoc_Them);
            }
        }

        public bool mNhapHangTuKhoDuoc_XemIn
        {
            get
            {
                return _mNhapHangTuKhoDuoc_XemIn;
            }
            set
            {
                if (_mNhapHangTuKhoDuoc_XemIn == value)
                    return;
                _mNhapHangTuKhoDuoc_XemIn = value;
                NotifyOfPropertyChange(() => mNhapHangTuKhoDuoc_XemIn);
            }
        }

        public bool mNhapHangTuKhoDuoc_In
        {
            get
            {
                return _mNhapHangTuKhoDuoc_In;
            }
            set
            {
                if (_mNhapHangTuKhoDuoc_In == value)
                    return;
                _mNhapHangTuKhoDuoc_In = value;
                NotifyOfPropertyChange(() => mNhapHangTuKhoDuoc_In);
            }
        }

        #endregion

        private void GetInwardInvoiceDrugByID(long ID)
        {
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyInwardDrugServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetInwardInvoiceDrugByID(ID, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var tmpInwardDrugInvoice = contract.EndGetInwardInvoiceDrugByID(asyncResult);
                                //▼===== 20200305 TTM:  Phải làm chuyện ruồi bâu này. Lý do
                                //                      Màn hình nhập hàng từ kho dược vào kho BHYT đc clone ra từ màn hình của khoa nội trú nên biến đang sử dụng thuộc
                                //                      InwardDrugClinicDeptInvoice => Không thể chạy hàm để lấy thông tin phiếu nhập đc vì thế nên phải tạo biến lấy thông tin phiếu nhập
                                //                      Rồi gán lại để hiển thị chính xác.
                                string InvID = tmpInwardDrugInvoice.InvID;
                                if (InwardDrugInvoice != null)
                                {
                                    InwardDrugInvoice.InvID = InvID;
                                    InwardDrugDetails_ByID(tmpInwardDrugInvoice.inviID);
                                }
                                //▲===== 20200305 TTM:
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
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    this.HideBusyIndicator();
                }
            });

            t.Start();
        }
    }
}
