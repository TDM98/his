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
using aEMR.Controls;

namespace aEMR.Pharmacy.ViewModels.InwardDrugs
{
    [Export(typeof(IInwardFromInternalExport)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class InwardFromInternalExportViewModel : Conductor<object>, IInwardFromInternalExport
        , IHandle<PharmacyCloseSearchInwardIncoiceEvent>
    {
        private readonly long TypID = (long)AllLookupValues.RefOutputType.NHAP_TU_LUANCHUYENKHO;

        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public InwardFromInternalExportViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            SearchCriteria = new InwardInvoiceSearchCriteria();
            InitInvoice();

            Coroutine.BeginExecute(DoGetStore_STORAGE());
        }

        private IEnumerator<IResult> DoGetStore_STORAGE()
        {
            var paymentTypeTask = new LoadStoreListTask((long)AllLookupValues.StoreType.STORAGE_EXTERNAL, false, null, false, false);
            yield return paymentTypeTask;
            StoreCbx = paymentTypeTask.LookupList.Where(x => x.StoreID != Globals.ServerConfigSection.PharmacyElements.HIStorageID).ToObservableCollection();
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
            InwardDrugInvoice = new InwardDrugInvoice
            {
                InvDateInvoice = DateTime.Now,
                DSPTModifiedDate = DateTime.Now,
                CurrencyID = 129,
                TypID = TypID
            };
            if (InwardDrugInvoice.SelectedStaff == null)
            {
                InwardDrugInvoice.SelectedStaff = new Staff();
            }
            InwardDrugInvoice.SelectedStaff.FullName = GetStaffLogin().FullName;
            InwardDrugInvoice.StaffID = GetStaffLogin().StaffID;
            IsEnableSaveBtn = false; //--28/01/2021 DatTB Apply cách của A.Vũ
        }

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

        private InwardDrugInvoice _InwardDrugInvoice;
        public InwardDrugInvoice InwardDrugInvoice
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

        private ObservableCollection<OutwardDrugInvoice> _OutwardInvoiceList;
        public ObservableCollection<OutwardDrugInvoice> OutwardInvoiceList
        {
            get
            {
                return _OutwardInvoiceList;
            }
            set
            {
                if (_OutwardInvoiceList != value)
                {
                    _OutwardInvoiceList = value;

                    NotifyOfPropertyChange(() => OutwardInvoiceList);
                }
            }
        }

        private InwardDrug _CurrentInwardDrug;
        public InwardDrug CurrentInwardDrug
        {
            get
            {
                return _CurrentInwardDrug;
            }
            set
            {
                if (_CurrentInwardDrug != value)
                {
                    _CurrentInwardDrug = value;

                    NotifyOfPropertyChange(() => CurrentInwardDrug);
                }
            }
        }

        private OutwardDrugInvoice _CurrentOutwardDrugInvoice;
        public OutwardDrugInvoice CurrentOutwardDrugInvoice
        {
            get
            {
                return _CurrentOutwardDrugInvoice;
            }
            set
            {
                if (_CurrentOutwardDrugInvoice != value)
                {
                    _CurrentOutwardDrugInvoice = value;

                    NotifyOfPropertyChange(() => CurrentOutwardDrugInvoice);
                }
            }
        }

        private DateTime? _DSPTModifiedDate_Outward;
        public DateTime? DSPTModifiedDate_Outward
        {
            get { return _DSPTModifiedDate_Outward; }
            set
            {
                if(_DSPTModifiedDate_Outward != value)
                {
                    _DSPTModifiedDate_Outward = value;
                    NotifyOfPropertyChange(() => DSPTModifiedDate_Outward);
                }
            }
        }

        #endregion

        private Staff GetStaffLogin()
        {
            return Globals.LoggedUserAccount.Staff;
        }

        private void LoadOutwardInvoiceList(long? StoreID)
        {
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyInwardDrugServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginOutwardInternalExportPharmacyInvoice_Cbx(StoreID, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndOutwardInternalExportPharmacyInvoice_Cbx(asyncResult);
                                OutwardInvoiceList = results.ToObservableCollection();
                            }
                            catch (Exception ex)
                            {
                                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                                ClientLoggerHelper.LogInfo(ex.ToString());
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

        public void CbxKho_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (InwardDrugInvoice != null && InwardDrugInvoice.CanAdd)
            {
                if (InwardDrugInvoice.SelectedStorage != null)
                {
                    LoadOutwardInvoiceList(InwardDrugInvoice.SelectedStorage.StoreID);
                    CurrentOutwardDrugInvoice = null;
                }
                else
                {
                    if (OutwardInvoiceList != null)
                    {
                        OutwardInvoiceList.Clear();
                    }
                }
            }
        }

        public void CbxOutInvID_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (InwardDrugInvoice != null && InwardDrugInvoice.CanAdd)
            {
                //--▼--28/01/2021 DatTB Apply cách của A.Vũ
                InwardDrugInvoice.InwardDrugs = null;
                IsEnableSaveBtn = false;
                //--▲--28/01/2021 DatTB Apply cách của A.Vũ

                if (((ComboBox)sender).SelectedItem != null)
                {
                    DSPTModifiedDate_Outward = ((OutwardDrugInvoice)((ComboBox)sender).SelectedItem).DSPTModifiedDate;
                    OutwardDrugDetails_Load(((OutwardDrugInvoice)((ComboBox)sender).SelectedItem).outiID);
                }
                else
                {
                    DSPTModifiedDate_Outward = null;
                    InwardDrugInvoice.SelectedStorageOut = null;
                    if (InwardDrugInvoice.InwardDrugs != null)
                    {
                        InwardDrugInvoice.InwardDrugs = null;
                    }
                }
            }
        }

        private void OutwardDrugDetails_Load(long outiID)
        {
            IsEnableSaveBtn = false;
            if (null != InwardDrugInvoice && null != InwardDrugInvoice.InwardDrugs)
            {
                InwardDrugInvoice.InwardDrugs = null;
            }

            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyInwardDrugServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetOutWardDrugInvoiceByID(outiID, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndGetOutWardDrugInvoiceByID(asyncResult);
                                if (results != null)
                                {
                                    if (InwardDrugInvoice.InwardDrugs == null)
                                    {
                                        InwardDrugInvoice.InwardDrugs = new ObservableCollection<InwardDrug>();
                                    }
                                    bool IsInCost = results.V_ByOutPrice == (long)AllLookupValues.V_ByOutPrice.GIAVON ? true : false;
                                    foreach (OutwardDrug p in results.OutwardDrugs)
                                    {
                                        InwardDrug item = new InwardDrug
                                        {
                                            InProductionDate = p.InwardDrug.InProductionDate,
                                            InExpiryDate = p.InExpiryDate,
                                            InBatchNumber = p.InBatchNumber,
                                            V_GoodsType = p.InwardDrug.V_GoodsType,
                                            NormalPrice = IsInCost ? p.InwardDrug.NormalPrice : p.OutPrice,
                                            HIPatientPrice = IsInCost ? p.InwardDrug.HIPatientPrice : p.OutPrice,
                                            HIAllowedPrice = IsInCost ? p.InwardDrug.HIAllowedPrice : 0,
                                            DrugID = p.DrugID,
                                            InQuantity = p.OutQuantity,
                                            InBuyingPrice = IsInCost ? p.InwardDrug.InBuyingPrice : p.OutPrice,
                                            InBuyingPriceActual = IsInCost ? p.OutPrice : p.InwardDrug.InBuyingPrice,
                                            SdlDescription = "",
                                            SelectedDrug = new RefGenericDrugDetail
                                            {
                                                DrugCode = p.GenMedProductItem.Code,
                                                BrandName = p.GenMedProductItem.BrandName,
                                                SeletedUnit = p.GenMedProductItem.SelectedUnit
                                            },
                                            OutID = p.OutID,
                                            VAT = p.VAT,
                                            IsNotVat = p.IsNotVat
                                        };
                                        InwardDrugInvoice.InwardDrugs.Add(item);
                                    }
                                    InwardDrugInvoice.SelectedStorageOut = results.SelectedStorage;
                                    InwardDrugInvoice.outiID = results.outiID;
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

        private void GetInwardInvoiceDrugByID(long ID)
        {
            IsEnableSaveBtn = false;
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
                                InwardDrugInvoice = contract.EndGetInwardInvoiceDrugByID(asyncResult);
                                if (InwardDrugInvoice != null)
                                {
                                    InwardDrugDetails_ByID(InwardDrugInvoice.inviID);
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

        private void InwardDrugInvoice_SaveXML()
        {
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyInwardDrugServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginInwardDrugInvoice_SaveXML(InwardDrugInvoice, DSPTModifiedDate_Outward, Globals.DispatchCallback((asyncResult) =>
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

        public void BtnSave()
        {
            if (InwardDrugInvoice.SelectedStorage == null || InwardDrugInvoice.SelectedStorage.StoreID <= 0)
            {
                Globals.ShowMessage(eHCMSResources.Z0566_G1_ChuaChonKhoNhap, eHCMSResources.G0442_G1_TBao);
                return;
            }

            if (InwardDrugInvoice.InwardDrugs != null && InwardDrugInvoice.InwardDrugs.Count > 0)
            {
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
                    InwardDrugInvoice_SaveXML();
                }
            }
            else
            {
                Globals.ShowMessage(eHCMSResources.Z1093_G1_Chua_NhapThuoc, eHCMSResources.G0442_G1_TBao);               
            }
        }

        //public void BtnAcceptAutoUpdate()
        //{
        //    this.ShowBusyIndicator();

        //    if (InwardDrugInvoice == null || InwardDrugInvoice.inviID <= 0)
        //    {
        //        this.HideBusyIndicator();
        //        MessageBox.Show(eHCMSResources.A0659_G1_Msg_InfoKhCoPhNhap, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
        //        return;
        //    }

        //    var t = new Thread(() =>
        //    {
        //        try
        //        {
        //            using (var serviceFactory = new PharmacyInwardDrugServiceClient())
        //            {
        //                var contract = serviceFactory.ServiceInstance;
        //                contract.BeginAcceptAutoUpdateInwardClinicInvoice(InwardDrugInvoice.inviID, Globals.DispatchCallback((asyncResult) =>
        //                {
        //                    try
        //                    {
        //                        bool result = contract.EndAcceptAutoUpdateInwardClinicInvoice(asyncResult);
        //                        if (result)
        //                        {
        //                            MessageBox.Show(eHCMSResources.A0286_G1_Msg_InfoChNhanChoK_DcCNhatOK, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
        //                            GetInwardInvoiceDrugByID(InwardDrugInvoice.inviID);
        //                        }
        //                        else
        //                        {
        //                            MessageBox.Show(eHCMSResources.A0287_G1_Msg_InfoChNhanChoK_DcCNhatFail, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
        //                        }
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        MessageBox.Show(ex.Message);
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

        DataGrid GridInwardDrug = null;
        public void GridInwardDrug_Loaded(object sender, RoutedEventArgs e)
        {
            GridInwardDrug = sender as DataGrid;
        }

        public void BtnNew()
        {
            InitInvoice();
        }

        public void Tbx_Search_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (SearchCriteria != null)
                {
                    SearchCriteria.InwardID = (sender as TextBox).Text;
                }
                BtnSearch();
            }
        }

        public void BtnSearch()
        {
            SearchInwardInvoiceDrug(0, Globals.PageSize);
        }

        public void OpenPopUpSearchInwardInvoice(IList<InwardDrugInvoice> results, int Totalcount)
        {
            void onInitDlg(IInwardFromInternalExportSearch proAlloc)
            {
                proAlloc.SearchCriteria = SearchCriteria.DeepCopy();
                proAlloc.TypID = TypID;
                proAlloc.InwardInvoiceList.Clear();
                proAlloc.InwardInvoiceList.TotalItemCount = Totalcount;
                proAlloc.InwardInvoiceList.PageIndex = 0;
                if (results != null && results.Count > 0)
                {
                    foreach (InwardDrugInvoice p in results)
                    {
                        proAlloc.InwardInvoiceList.Add(p);
                    }
                }
            }

            GlobalsNAV.ShowDialog<IInwardFromInternalExportSearch>(onInitDlg);
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
                    using (var serviceFactory = new PharmacyInwardDrugServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginSearchInwardInvoiceDrug(SearchCriteria, TypID, PageIndex, PageSize, true, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndSearchInwardInvoiceDrug(out int Totalcount, asyncResult);
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
                    InwardDrugInvoice.TotalPrice += InwardDrugInvoice.InwardDrugs[i].InBuyingPriceActual * (decimal)InwardDrugInvoice.InwardDrugs[i].InQuantity;
                }
            }
        }

        private void CountTotalPrice(decimal TongTienSPChuaVAT
           , decimal CKTrenSP
           , decimal TongTienTrenSPDaTruCK
           , decimal TongCKTrenHoaDon
           , decimal TongTienHoaDonCoThueNK
           , decimal TongTienHoaDonCoVAT
           , decimal TotalVATDifferenceAmount)
        {
            InwardDrugInvoice.TotalPriceNotVAT = TongTienSPChuaVAT;
            InwardDrugInvoice.TotalDiscountOnProduct = CKTrenSP;//tong ck tren hoan don
            InwardDrugInvoice.TotalDiscountInvoice = CKTrenSP + TongCKTrenHoaDon;
            InwardDrugInvoice.TotalPrice = TongTienSPChuaVAT - (CKTrenSP + TongCKTrenHoaDon);
            InwardDrugInvoice.TotalHaveCustomTax = TongTienHoaDonCoThueNK;
            InwardDrugInvoice.TotalPriceVAT = TongTienHoaDonCoVAT;
            InwardDrugInvoice.TotalVATDifferenceAmount = TotalVATDifferenceAmount;
            InwardDrugInvoice.TotalVATAmountActual = InwardDrugInvoice.TotalVATAmount + InwardDrugInvoice.TotalVATDifferenceAmount;
        }

        private void InwardDrugDetails_ByID(long ID)
        {
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyInwardDrugServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetspInwardDrugDetailsByID(ID, Globals.PageSize, 0, true, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndGetspInwardDrugDetailsByID(out int total, out decimal TongTienSPChuaVAT
                                    , out decimal CKTrenSP, out decimal TongTienTrenSPDaTruCK, out decimal TongCKTrenHoaDon, out decimal TongTienHoaDonCoThueNK
                                    , out decimal TongTienHoaDonCoVAT, out decimal TotalVATDifferenceAmount, asyncResult);
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
            InwardDrug P = item as InwardDrug;
            P.SdlDescription = Name;
        }

        AxAutoComplete Au;

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
            Au = sender as AxAutoComplete;
            SearchRefShelfLocation(e.Parameter);
            if (GridInwardDrug != null && GridInwardDrug.SelectedItem != null)
            {
                SetValueSdlDescription(e.Parameter, GridInwardDrug.SelectedItem);
            }
        }

        public void AutoLocation_Tex_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Au != null && CurrentInwardDrug != null)
            {
                if (Au.SelectedItem != null)
                {
                    CurrentInwardDrug.SelectedShelfDrugLocation = Au.SelectedItem as RefShelfDrugLocation;
                }
                else
                {
                    CurrentInwardDrug.SdlDescription = Au.Text;
                }
            }
        }

        #endregion

        #region IHandle<PharmacyCloseSearchInwardIncoiceEvent> Members
        public void Handle(PharmacyCloseSearchInwardIncoiceEvent message)
        {
            if (message != null && IsActive)
            {
                InwardDrugInvoice = message.SelectedInwardInvoice as InwardDrugInvoice;
                InwardDrugDetails_ByID(InwardDrugInvoice.inviID);
            }
        }
        #endregion

        #region printing member
        public void BtnPreview()
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
    }
}
