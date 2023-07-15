using System.ComponentModel.Composition;
using aEMR.ViewContracts;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.Common.Collections;
using aEMR.Common;
using aEMR.CommonTasks;
using aEMR.Infrastructure;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using Caliburn.Micro;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System;
using System.Collections.ObjectModel;
using System.Threading;
using DataEntities;
using System.Linq;
using System.Collections.Generic;
using eHCMSLanguage;
using aEMR.Controls;
using aEMR.Common.BaseModel;
/*
* 20181124 #001 TTM:    BM 0005309: Cho phép nhập hàng từ phiếu xuất thuốc của Kho BHYT - Nhà thuốc.
* 20181124 #002 TTM:    Chia trường hợp tìm kiếm dựa vào TypID (Cho trường hợp nhập trả từ kho BHYT - Nhà thuốc và Kho phòng).
* 20190522 #003 TNHX:   [BM0010766] Thêm trường ngày xuất của phiếu xuất để nhập vào kho + refactor code
* 20190529 #004 TTM:    BM 0010774: Phân quyền để chỉnh sửa thời gian nhập trả cho kho Dược.
* 20211102 #005 QTD:    Lọc kho theo cấu hình trách nhiệm
*/
namespace aEMR.DrugDept.ViewModels
{
    [Export(typeof(IClinicInwardFromDrugDept)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class ClinicInwardFromDrugDeptViewModel : ViewModelBase, IClinicInwardFromDrugDept
        , IHandle<DrugDeptCloseSearchInwardIncoiceEvent>
    {
        private long? TypID = (long)AllLookupValues.RefOutputType.NHAP_TU_NGUON_KHAC;
        //▼====== #002
        private long? TypID_V2 = (long)AllLookupValues.RefOutputType.NHAP_TU_KHO_BHYT_NHA_THUOC;
        //▲====== #002
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public ClinicInwardFromDrugDeptViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            //Globals.EventAggregator.Subscribe(this);
            //Thay bang
            //eventAggregator.Subscribe(this);
            SearchCriteria = new InwardInvoiceSearchCriteria();
            //▼====== #004
            authorization();
            //▲====== #004
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            Globals.EventAggregator.Subscribe(this);
            //20181124 TTM: Gán giá trị cho loại nhập là giá trị mới (nhập từ kho BHYT để phân biệt nhập với khoa phòng
            //              Do cả 2 outiID đều được bỏ vào cùng 1 cột outiID của bảng nhập khoa dược.)
            //if (vNhapTraKhoBHYT)
            //{
            //    CurrentInwardDrugMedDeptInvoice.TypID = (long)AllLookupValues.RefOutputType.NHAP_TU_KHO_BHYT_NHA_THUOC;
            //}
            InitInvoice();
            Coroutine.BeginExecute(DoGetStore_MedDept());
        }

        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);
            Globals.EventAggregator.Unsubscribe(this);
        }

        private void InitInvoice()
        {
            CurrentInwardDrugMedDeptInvoice = null;
            CurrentInwardDrugMedDeptInvoice = new InwardDrugMedDeptInvoice();
            CurrentInwardDrugMedDeptInvoice.InvDateInvoice = Globals.GetCurServerDateTime();
            CurrentInwardDrugMedDeptInvoice.DSPTModifiedDate = Globals.GetCurServerDateTime();
            CurrentInwardDrugMedDeptInvoice.CurrencyID = 129;
            if (vNhapTraKhoBHYT)
            {
                CurrentInwardDrugMedDeptInvoice.TypID = (long)AllLookupValues.RefOutputType.NHAP_TU_KHO_BHYT_NHA_THUOC;
            }
            else
            {
                CurrentInwardDrugMedDeptInvoice.TypID = (long)AllLookupValues.RefOutputType.NHAP_TU_NGUON_KHAC;
            }
            if (CurrentInwardDrugMedDeptInvoice.SelectedStaff == null)
            {
                CurrentInwardDrugMedDeptInvoice.SelectedStaff = new Staff();
            }
            CurrentInwardDrugMedDeptInvoice.SelectedStaff.FullName = GetStaffLogin().FullName;
            CurrentInwardDrugMedDeptInvoice.StaffID = GetStaffLogin().StaffID;

            //▼====== #001: Clear sạch dữ liệu trong cbb kho BHYT khi nhấn nút tạo phiếu mới.
            PharmacyInvoiceList = new ObservableCollection<OutwardDrugInvoice>();
            ClinicDeptInvoiceList = new ObservableCollection<OutwardDrugClinicDeptInvoice>();
            //▲====== #001
            IsEnableSaveBtn = false; //--28/01/2021 DatTB Apply cách của A.Vũ
        }

        #region Properties Member
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

        private InwardDrugMedDeptInvoice _CurrentInwardDrugMedDeptInvoice;
        public InwardDrugMedDeptInvoice CurrentInwardDrugMedDeptInvoice
        {
            get
            {
                return _CurrentInwardDrugMedDeptInvoice;
            }
            set
            {
                if (_CurrentInwardDrugMedDeptInvoice != value)
                {
                    _CurrentInwardDrugMedDeptInvoice = value;

                    NotifyOfPropertyChange(() => CurrentInwardDrugMedDeptInvoice);
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

        private ObservableCollection<OutwardDrugClinicDeptInvoice> _ClinicDeptInvoiceList;
        public ObservableCollection<OutwardDrugClinicDeptInvoice> ClinicDeptInvoiceList
        {
            get
            {
                return _ClinicDeptInvoiceList;
            }
            set
            {
                if (_ClinicDeptInvoiceList != value)
                {
                    _ClinicDeptInvoiceList = value;

                    NotifyOfPropertyChange(() => ClinicDeptInvoiceList);
                }
            }
        }

        private InwardDrugMedDept _CurrentInwardDrugMedDept;
        public InwardDrugMedDept CurrentInwardDrugMedDept
        {
            get
            {
                return _CurrentInwardDrugMedDept;
            }
            set
            {
                if (_CurrentInwardDrugMedDept != value)
                {
                    _CurrentInwardDrugMedDept = value;

                    NotifyOfPropertyChange(() => CurrentInwardDrugMedDept);
                }
            }
        }

        private DateTime _DSPTModifiedDate_Outward;
        public DateTime DSPTModifiedDate_Outward
        {
            get
            {
                return _DSPTModifiedDate_Outward;
            }
            set
            {
                if (_DSPTModifiedDate_Outward != value)
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

        private IEnumerator<IResult> DoGetStore_MedDept()
        {
            var paymentTypeTask = new LoadStoreListTask((long)AllLookupValues.StoreType.STORAGE_DRUGDEPT, false, null, true, false);
            yield return paymentTypeTask;
            //StoreCbx = paymentTypeTask.LookupList.Where(x => (V_MedProductType != 0 && x.ListV_MedProductType != null && x.ListV_MedProductType.Contains(V_MedProductType.ToString()))).ToObservableCollection();
            //▼===== #005
            var StoreTemp = paymentTypeTask.LookupList.Where(x => (V_MedProductType != 0 && x.ListV_MedProductType != null && x.ListV_MedProductType.Contains(V_MedProductType.ToString()))).ToObservableCollection();
            StoreCbx = Globals.CheckDrugMedStoreWareHouse(StoreTemp);
            if (StoreCbx != null && StoreCbx.Count > 0)
            {
                CurrentInwardDrugMedDeptInvoice.SelectedStorage = StoreCbx.FirstOrDefault();
            }
            else
            {
                MessageBox.Show(eHCMSResources.A0110_G1_Msg_InfoChuaCauHinhTNKho.Replace("\\n ", "\n"));
            }
            //▲===== #005
            yield break;
        }

        private void LoadClinicDeptInvoiceList(long? StoreID)
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            IsEnableSaveBtn = false;
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {

                    using (var serviceFactory = new PharmacyMedDeptServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginOutwardDrugClinicDeptInvoice_Cbx(StoreID, V_MedProductType, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndOutwardDrugClinicDeptInvoice_Cbx(asyncResult);
                                ClinicDeptInvoiceList = results.ToObservableCollection();
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

        //▼====== #001
        private bool _vNhapTraKhoBHYT = true;
        public bool vNhapTraKhoBHYT
        {
            get
            {
                return _vNhapTraKhoBHYT;
            }
            set
            {
                if (_vNhapTraKhoBHYT == value)
                {
                    return;
                }
                _vNhapTraKhoBHYT = value;
                NotifyOfPropertyChange(() => vNhapTraKhoBHYT);
            }
        }

        private ObservableCollection<OutwardDrugInvoice> _PharmacyInvoiceList;
        public ObservableCollection<OutwardDrugInvoice> PharmacyInvoiceList
        {
            get
            {
                return _PharmacyInvoiceList;
            }
            set
            {
                if (_PharmacyInvoiceList != value)
                {
                    _PharmacyInvoiceList = value;
                    NotifyOfPropertyChange(() => PharmacyInvoiceList);
                }
            }
        }

        private void LoadPharmacyInvoiceList(long? StoreID)
        {
            this.ShowBusyIndicator(eHCMSResources.K2871_G1_DangLoadDLieu);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyMedDeptServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginOutwardDrugPharmacyInvoice_Cbx(StoreID, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndOutwardDrugPharmacyInvoice_Cbx(asyncResult);
                                PharmacyInvoiceList = results.ToObservableCollection();
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
                    ClientLoggerHelper.LogInfo(ex.ToString());
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }

        public void cbxOutInvIDPharmacy_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbxOutInvIDPharmacy.SelectedIndex >= 0)
            {
                //--▼--28/01/2021 DatTB Apply cách của A.Vũ
                CurrentInwardDrugMedDeptInvoice.InwardDrugs = null;
                IsEnableSaveBtn = false;
                //--▲--28/01/2021 DatTB Apply cách của A.Vũ

                CurrentInwardDrugMedDeptInvoice.outiID = PharmacyInvoiceList[cbxOutInvIDPharmacy.SelectedIndex].outiID;
                //▼====: #003
                CurrentInwardDrugMedDeptInvoice.OutDate = PharmacyInvoiceList[cbxOutInvIDPharmacy.SelectedIndex].OutDate;
                //▲====: #003
                OutwardDrugDetails_Load(PharmacyInvoiceList[cbxOutInvIDPharmacy.SelectedIndex].outiID);
                //20200314 TBL: Khi chọn phiếu xuất từ kho BHYT thì lấy DSPTModifiedDate set cho DSPTModifiedDate_Outward để khi lưu đem xuống kiểm tra phiếu xuất đã có thay đổi khi chuẩn bị nhập
                DSPTModifiedDate_Outward = PharmacyInvoiceList[cbxOutInvIDPharmacy.SelectedIndex].DSPTModifiedDate;
            }
        }
        private void OutwardDrugDetails_Load(long outiID)
        {
            CurrentInwardDrugMedDeptInvoice.InwardDrugs = null;
            this.ShowBusyIndicator(eHCMSResources.K2871_G1_DangLoadDLieu);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyInwardDrugServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetOutwardDrugDetailsByOutwardInvoiceForDrugDept(outiID, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndGetOutwardDrugDetailsByOutwardInvoiceForDrugDept(asyncResult);
                                if (results != null && results.Count > 0)
                                {
                                    CurrentInwardDrugMedDeptInvoice.InwardDrugs = new ObservableCollection<InwardDrugMedDept>();
                                    
                                    foreach (OutwardDrug p in results)
                                    {
                                        InwardDrugMedDept item = new InwardDrugMedDept();
                                        item.RefGenMedProductDetails = p.GenMedProductItem;
                                        if (p.InwardDrug != null)
                                        {
                                            item.InProductionDate = p.InwardDrug.InProductionDate;
                                            item.InExpiryDate = p.InwardDrug.InExpiryDate;
                                            item.InBatchNumber = p.InwardDrug.InBatchNumber;
                                            item.V_GoodsType = p.InwardDrug.V_GoodsType;
                                            item.NormalPrice = p.GenMedProductItem.NormalPrice;
                                            item.HIPatientPrice = p.GenMedProductItem.HIPatientPrice;
                                            item.HIAllowedPrice = p.GenMedProductItem.HIAllowedPrice;
                                            item.GenMedVersionID = p.InwardDrug.DrugVersionID;
                                            item.InCost = p.InwardDrug.InCost;
                                            item.InBuyingPrice = p.InwardDrug.InBuyingPrice;
                                            item.InBuyingPriceActual = p.InwardDrug.InCost;
                                        }
                                        item.InQuantity = p.OutQuantity;
                                        item.ClinicDeptQty = p.OutQuantity;
                                        item.SdlDescription = "";
                                        item.OutID = p.OutID;
                                        item.DrugDeptInIDOrig = p.InwardDrug.DrugDeptInIDOrig;
                                        item.VAT = p.VAT;

                                        //20181124 TTM: Cần phải có StoreIDOut vì phiếu xuất này khác xuất của khoa phòng ở chỗ là từ kho BHYT nhà thuốc mà ra
                                        //Nếu không có dòng này sẽ không phân biệt đc phiếu xuất nào là từ khoa phòng, phiếu xuất nào là từ kho BHYT - Nhà thuốc.
                                        CurrentInwardDrugMedDeptInvoice.StoreIDOut = Globals.ServerConfigSection.PharmacyElements.HIStorageID;
                                        CurrentInwardDrugMedDeptInvoice.InwardDrugs.Add(item);
                                    }
                                    CountTotalPrice();
                                    IsEnableSaveBtn = true; //--28/01/2021 DatTB Apply cách của A.Vũ
                                }
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
                    ClientLoggerHelper.LogInfo(ex.ToString());
                }
                finally
                {
                    this.HideBusyIndicator();
                }
            });

            t.Start();
        }

        //20181124 TTM: Control Visible của 2 control PX kho phòng và PX kho BHYT do không biết vì lý do gì mà biding visible control ko hoạt động. 
        TextBlock txbOutInvIDPharmacy = null;
        TextBlock txbOutInvID = null;
        TextBox txtOutInvIDPharmacy = null;
        TextBox txtOutInvID = null;
        KeyEnabledComboBox cbxOutInvIDPharmacy = null;
        KeyEnabledComboBox cbxOutInvID = null;
        public void cbxOutInvID_Loaded(object sender, RoutedEventArgs e)
        {
            cbxOutInvID = sender as KeyEnabledComboBox;
        }
        public void txbOutInvID_Loaded(object sender, RoutedEventArgs e)
        {
            txbOutInvID = sender as TextBlock;
        }
        public void txtOutInvID_Loaded(object sender, RoutedEventArgs e)
        {
            txtOutInvID = sender as TextBox;
        }
        public void txbOutInvIDPharmacy_Loaded(object sender, RoutedEventArgs e)
        {
            txbOutInvIDPharmacy = sender as TextBlock;
        }
        public void txtOutInvIDPharmacy_Loaded(object sender, RoutedEventArgs e)
        {
            txtOutInvIDPharmacy = sender as TextBox;
        }
        public void cbxOutInvIDPharmacy_Loaded(object sender, RoutedEventArgs e)
        {
            cbxOutInvIDPharmacy = sender as KeyEnabledComboBox;
            if (!vNhapTraKhoBHYT)
            {
                cbxOutInvIDPharmacy.Visibility = Visibility.Collapsed;
                txbOutInvIDPharmacy.Visibility = Visibility.Collapsed;
                txtOutInvIDPharmacy.Visibility = Visibility.Collapsed;
            }
            else
            {
                cbxOutInvID.Visibility = Visibility.Collapsed;
                txbOutInvID.Visibility = Visibility.Collapsed;
                txtOutInvID.Visibility = Visibility.Collapsed;
            }
        }
        //▲====== #001

        public void cbxKho_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CurrentInwardDrugMedDeptInvoice != null && CurrentInwardDrugMedDeptInvoice.CanSave)
            {
                if (CurrentInwardDrugMedDeptInvoice.SelectedStorage != null)
                {
                    //20181124 TTM: Chỉ load Combobox nào đang đc sử dụng (Phiếu xuất kho phòng hoặc phiếu xuất kho BHYT - Nhà thuốc.
                    if (!vNhapTraKhoBHYT)
                    {
                        LoadClinicDeptInvoiceList(CurrentInwardDrugMedDeptInvoice.SelectedStorage.StoreID);
                    }
                    else
                    {
                        LoadPharmacyInvoiceList(CurrentInwardDrugMedDeptInvoice.SelectedStorage.StoreID);
                    }
                    CurrentInwardDrugMedDeptInvoice.CurrentOutwardDrugClinicDeptInvoice = null;
                }
                else
                {
                    if (ClinicDeptInvoiceList != null)
                    {
                        ClinicDeptInvoiceList = null;
                    }
                }
            }
        }

        public void cbxOutInvID_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CurrentInwardDrugMedDeptInvoice != null && CurrentInwardDrugMedDeptInvoice.CanSave)
            {
                //--▼--28/01/2021 DatTB Apply cách của A.Vũ
                CurrentInwardDrugMedDeptInvoice.InwardDrugs = null;
                IsEnableSaveBtn = false;
                //--▲--28/01/2021 DatTB Apply cách của A.Vũ

                if (CurrentInwardDrugMedDeptInvoice.CurrentOutwardDrugClinicDeptInvoice != null)
                {
                    CurrentInwardDrugMedDeptInvoice.outiID = CurrentInwardDrugMedDeptInvoice.CurrentOutwardDrugClinicDeptInvoice.outiID;
                    //▼====: #003
                    CurrentInwardDrugMedDeptInvoice.OutDate = (DateTime)CurrentInwardDrugMedDeptInvoice.CurrentOutwardDrugClinicDeptInvoice.OutDate;
                    //▲====: #003
                    OutwardDrugClinicDeptDetails_Load(CurrentInwardDrugMedDeptInvoice.CurrentOutwardDrugClinicDeptInvoice.outiID);
                }
                else
                {
                    if (CurrentInwardDrugMedDeptInvoice.InwardDrugs != null)
                    {
                        CurrentInwardDrugMedDeptInvoice.InwardDrugs = null;
                    }
                }
            }
        }

        private void OutwardDrugClinicDeptDetails_Load(long outiID)
        {
            this.ShowBusyIndicator();
            CurrentInwardDrugMedDeptInvoice.InwardDrugs = null;
            IsEnableSaveBtn = false;
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyClinicDeptServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetOutwardDrugClinicDeptDetailByInvoice_V2(outiID, V_MedProductType, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndGetOutwardDrugClinicDeptDetailByInvoice_V2(asyncResult);
                                if (results != null && results.Count > 0)
                                {
                                    CurrentInwardDrugMedDeptInvoice.InwardDrugs = new ObservableCollection<InwardDrugMedDept>();
                                    foreach (OutwardDrugClinicDept p in results)
                                    {
                                        InwardDrugMedDept item = new InwardDrugMedDept();
                                        item.RefGenMedProductDetails = p.GenMedProductItem;
                                        if (p.InwardDrugClinicDept != null)
                                        {
                                            item.InProductionDate = p.InwardDrugClinicDept.InProductionDate;
                                            item.InExpiryDate = p.InwardDrugClinicDept.InExpiryDate;
                                            item.InBatchNumber = p.InwardDrugClinicDept.InBatchNumber;
                                            item.V_GoodsType = p.InwardDrugClinicDept.V_GoodsType;
                                            item.NormalPrice = p.InwardDrugClinicDept.NormalPrice;
                                            item.HIPatientPrice = p.InwardDrugClinicDept.HIPatientPrice;
                                            item.HIAllowedPrice = p.InwardDrugClinicDept.HIAllowedPrice;
                                            item.GenMedVersionID = p.InwardDrugClinicDept.GenMedVersionID;
                                            item.InCost = p.InwardDrugClinicDept.InCost;
                                            item.InBuyingPrice = p.InwardDrugClinicDept.InBuyingPrice;
                                            item.InBuyingPriceActual = p.InwardDrugClinicDept.InCost;
                                            item.VAT = p.InwardDrugClinicDept.VAT;
                                        }
                                        item.InQuantity = p.OutQuantity;
                                        item.ClinicDeptQty = p.OutQuantity;
                                        item.SdlDescription = "";
                                        item.OutID = p.OutID;
                                        item.DrugDeptInIDOrig = p.DrugDeptInIDOrig;
                                        CurrentInwardDrugMedDeptInvoice.InwardDrugs.Add(item);
                                    }
                                    CountTotalPrice();
                                    IsEnableSaveBtn = true;
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
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyMedDeptServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetInwardDrugMedDeptInvoice_ByID_V2(ID, V_MedProductType, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                CurrentInwardDrugMedDeptInvoice = contract.EndGetInwardDrugMedDeptInvoice_ByID_V2(asyncResult);
                                if (CurrentInwardDrugMedDeptInvoice != null)
                                {
                                    InwardDrugDetails_ByID(CurrentInwardDrugMedDeptInvoice.inviID);
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

        private void InwardDrugMedDeptInvoice_SaveXML()
        {
            DateTime? DSPTModifiedDate = null;
            //DSPTModifiedDate = CurrentInwardDrugMedDeptInvoice.DSPTModifiedDate; //--◄-- 15/12/2020 DatTB Fix lỗi ngày giờ
            //--▼-- 15/12/2020 DatTB Fix lỗi ngày giờ
            //20200516 TBL: Vì viewmodel này sử dụng cho 2 menu là nhập từ kho phòng và nhập từ kho BHYT, nên CurrentInwardDrugMedDeptInvoice.CurrentOutwardDrugClinicDeptInvoice != null thì là từ màn hình nhập từ kho phòng
            if (CurrentInwardDrugMedDeptInvoice.CurrentOutwardDrugClinicDeptInvoice != null)
            {
                DSPTModifiedDate = CurrentInwardDrugMedDeptInvoice.CurrentOutwardDrugClinicDeptInvoice.DSPTModifiedDate;
            }
            else if (DSPTModifiedDate_Outward > DateTime.MinValue) //Nhập từ kho BHYT
            {
                DSPTModifiedDate = DSPTModifiedDate_Outward;
            }
            //--▲-- 15/12/2020 DatTB Fix lỗi ngày giờ

            this.DlgShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyMedDeptServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginInwardDrugMedDeptInvoice_SaveXML(CurrentInwardDrugMedDeptInvoice, DSPTModifiedDate, Globals.DispatchCallback((asyncResult) =>
                        {

                            try
                            {
                                int results = contract.EndInwardDrugMedDeptInvoice_SaveXML(out long inviID, asyncResult);
                                if (results > 0)
                                {
                                    Globals.ShowMessage(eHCMSResources.T0074_G1_I, eHCMSResources.T0074_G1_I);
                                }
                                else
                                {
                                    if (inviID > 0)
                                    {
                                        IsEnableSaveBtn = false;
                                        Globals.ShowMessage(eHCMSResources.A0468_G1_Msg_InfoLuuOK, eHCMSResources.G0442_G1_TBao);
                                        GetInwardInvoiceDrugByID(inviID);
                                    }
                                    else
                                    {
                                        Globals.ShowMessage(eHCMSResources.K2782_G1_DaCNhat, eHCMSResources.G0442_G1_TBao);
                                        if (CurrentInwardDrugMedDeptInvoice != null)
                                        {
                                            GetInwardInvoiceDrugByID(CurrentInwardDrugMedDeptInvoice.inviID);
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
                                this.DlgHideBusyIndicator();
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    this.DlgHideBusyIndicator();
                }
            });

            t.Start();
        }

        public void btnSave()
        {
            //22062018 TTM: Comment toàn bộ (làm bản orginal do DataGrid không có thuộc tính IsValid) xong chuyển toàn bộ thành chạy vô điều kiện

            //if (GridSuppliers.IsValid)
            //{
            //    if (CurrentInwardDrugMedDeptInvoice.SelectedStorage == null)
            //    {
            //        Globals.ShowMessage(string.Format("{0}!", eHCMSResources.Z0566_G1_ChuaChonKhoNhap), eHCMSResources.G0442_G1_TBao);
            //        return;
            //    }
            //    if (CurrentInwardDrugMedDeptInvoice.SelectedStorage.StoreID <= 0)
            //    {
            //        Globals.ShowMessage(string.Format("{0}!", eHCMSResources.Z0566_G1_ChuaChonKhoNhap), eHCMSResources.G0442_G1_TBao);
            //        return;
            //    }

            //    if (CurrentInwardDrugMedDeptInvoice.InwardDrugs != null && CurrentInwardDrugMedDeptInvoice.InwardDrugs.Count > 0)
            //    {
            //        CurrentInwardDrugMedDeptInvoice.V_MedProductType = V_MedProductType;
            //        if (CurrentInwardDrugMedDeptInvoice.SelectedStorageOut != null)
            //        {
            //            CurrentInwardDrugMedDeptInvoice.StoreIDOut = CurrentInwardDrugMedDeptInvoice.SelectedStorageOut.StoreID;
            //        }
            //        if (CurrentInwardDrugMedDeptInvoice.SelectedStorage != null)
            //        {
            //            CurrentInwardDrugMedDeptInvoice.StoreID = CurrentInwardDrugMedDeptInvoice.SelectedStorage.StoreID;
            //        }
            //        InwardDrugMedDeptInvoice_SaveXML();
            //    }
            //    else
            //    {
            //        if (V_MedProductType == (long)AllLookupValues.MedProductType.THUOC)
            //        {
            //            Globals.ShowMessage(string.Format(eHCMSResources.Z0580_G1_VuiLongNhap, eHCMSResources.G0787_G1_Thuoc), eHCMSResources.G0442_G1_TBao);
            //        }
            //        else if (V_MedProductType == (long)AllLookupValues.MedProductType.Y_CU)
            //        {
            //            Globals.ShowMessage(string.Format(eHCMSResources.Z0580_G1_VuiLongNhap, eHCMSResources.G2907_G1_YCu), eHCMSResources.G0442_G1_TBao);
            //        }
            //        else
            //        {
            //            Globals.ShowMessage(string.Format(eHCMSResources.Z0580_G1_VuiLongNhap, eHCMSResources.T1616_G1_HC), eHCMSResources.G0442_G1_TBao);
            //        }
            //    }
            //}
            //else
            //{
            //    Globals.ShowMessage(string.Format("{0}!", eHCMSResources.Z0567_G1_TTinKgHopLe), eHCMSResources.G0442_G1_TBao);
            //}

            if (CurrentInwardDrugMedDeptInvoice.SelectedStorage == null)
            {
                Globals.ShowMessage(string.Format("{0}!", eHCMSResources.Z0566_G1_ChuaChonKhoNhap), eHCMSResources.G0442_G1_TBao);
                return;
            }
            if (CurrentInwardDrugMedDeptInvoice.SelectedStorage.StoreID <= 0)
            {
                Globals.ShowMessage(string.Format("{0}!", eHCMSResources.Z0566_G1_ChuaChonKhoNhap), eHCMSResources.G0442_G1_TBao);
                return;
            }

            if (CurrentInwardDrugMedDeptInvoice.InwardDrugs != null && CurrentInwardDrugMedDeptInvoice.InwardDrugs.Count > 0)
            {
                CurrentInwardDrugMedDeptInvoice.V_MedProductType = V_MedProductType;
                if (CurrentInwardDrugMedDeptInvoice.SelectedStorageOut != null)
                {
                    CurrentInwardDrugMedDeptInvoice.StoreIDOut = CurrentInwardDrugMedDeptInvoice.SelectedStorageOut.StoreID;
                }
                if (CurrentInwardDrugMedDeptInvoice.SelectedStorage != null)
                {
                    CurrentInwardDrugMedDeptInvoice.StoreID = CurrentInwardDrugMedDeptInvoice.SelectedStorage.StoreID;
                }
                if (MessageBox.Show("Vui lòng kiểm tra nội dung phiếu nhập! Bạn muốn nhập phiếu này?",
                    eHCMSResources.G0442_G1_TBao, MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    InwardDrugMedDeptInvoice_SaveXML();
                }
            }
            else
            {
                if (V_MedProductType == (long)AllLookupValues.MedProductType.THUOC)
                {
                    Globals.ShowMessage(string.Format(eHCMSResources.Z0580_G1_VuiLongNhap, eHCMSResources.G0787_G1_Thuoc), eHCMSResources.G0442_G1_TBao);
                }
                else if (V_MedProductType == (long)AllLookupValues.MedProductType.Y_CU)
                {
                    Globals.ShowMessage(string.Format(eHCMSResources.Z0580_G1_VuiLongNhap, eHCMSResources.G2907_G1_YCu), eHCMSResources.G0442_G1_TBao);
                }
                else
                {
                    Globals.ShowMessage(string.Format(eHCMSResources.Z0580_G1_VuiLongNhap, eHCMSResources.T1616_G1_HC), eHCMSResources.G0442_G1_TBao);
                }
            }
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
            SearchInwardInvoiceDrug(0, 20);
        }

        public void OpenPopUpSearchInwardInvoice(IList<InwardDrugMedDeptInvoice> results, int Totalcount)
        {
            //mo popup tim
            //22062018 TTM: Đã thay thế bằng Action.
            //var proAlloc = Globals.GetViewModel<IDrugDeptInwardDrugSupplierSearch>();
            //proAlloc.SearchCriteria = SearchCriteria.DeepCopy();
            //proAlloc.TypID = TypID;
            //proAlloc.V_MedProductType = V_MedProductType;
            //proAlloc.InwardInvoiceList.Clear();
            //proAlloc.InwardInvoiceList.TotalItemCount = Totalcount;
            //proAlloc.InwardInvoiceList.PageIndex = 0;
            //if (results != null && results.Count > 0)
            //{
            //    foreach (InwardDrugMedDeptInvoice p in results)
            //    {
            //        proAlloc.InwardInvoiceList.Add(p);
            //    }
            //}
            //var instance = proAlloc as Conductor<object>;
            //Globals.ShowDialog(instance, (o) => { });

            Action<IDrugDeptInwardDrugSupplierSearch> onInitDlg = (proAlloc) =>
            {
                proAlloc.SearchCriteria = SearchCriteria.DeepCopy();
                //▼====== #002
                if (vNhapTraKhoBHYT)
                {
                    proAlloc.TypID = TypID_V2;
                }
                else
                {
                    proAlloc.TypID = TypID;
                }
                //▲====== #002
                proAlloc.V_MedProductType = V_MedProductType;
                proAlloc.InwardInvoiceList.Clear();
                proAlloc.InwardInvoiceList.TotalItemCount = Totalcount;
                proAlloc.InwardInvoiceList.PageIndex = 0;
                proAlloc.InwardInvoiceList.PageSize = 20;
                if (results != null && results.Count > 0)
                {
                    foreach (InwardDrugMedDeptInvoice p in results)
                    {
                        proAlloc.InwardInvoiceList.Add(p);
                    }
                }
            };
            GlobalsNAV.ShowDialog<IDrugDeptInwardDrugSupplierSearch>(onInitDlg);
        }

        private void SearchInwardInvoiceDrug(int PageIndex, int PageSize)
        {
            if (SearchCriteria == null)
            {
                return;
            }

            this.ShowBusyIndicator();
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
            //▼====== #002
            long? TypID_Search = 0;
            if (vNhapTraKhoBHYT)
            {
                TypID_Search = TypID_V2;
            }
            else
            {
                TypID_Search = TypID;
            }
            //▲====== #002
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyMedDeptServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginSearchInwardDrugMedDeptInvoice(SearchCriteria, TypID_Search, V_MedProductType, PageIndex, PageSize, true, false, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndSearchInwardDrugMedDeptInvoice(out int Totalcount, asyncResult);
                                if (results != null && results.Count > 0)
                                {
                                    if (results.Count > 1)
                                    {
                                        //mo pop up tim
                                        OpenPopUpSearchInwardInvoice(results.ToList(), Totalcount);
                                    }
                                    else
                                    {
                                        CurrentInwardDrugMedDeptInvoice = results.FirstOrDefault();
                                        InwardDrugDetails_ByID(CurrentInwardDrugMedDeptInvoice.inviID);
                                        //InitDetailInward();
                                        //DeepCopyInvoice();
                                        //LoadInfoThenSelectedInvoice();
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
            if (CurrentInwardDrugMedDeptInvoice != null && CurrentInwardDrugMedDeptInvoice.InwardDrugs != null)
            {
                CurrentInwardDrugMedDeptInvoice.TotalPrice = 0;
                for (int i = 0; i < CurrentInwardDrugMedDeptInvoice.InwardDrugs.Count; i++)
                {
                    CurrentInwardDrugMedDeptInvoice.TotalPrice += CurrentInwardDrugMedDeptInvoice.InwardDrugs[i].InBuyingPrice * (decimal)CurrentInwardDrugMedDeptInvoice.InwardDrugs[i].InQuantity;
                }
            }
        }

        private void InwardDrugDetails_ByID(long ID)
        {
            IsEnableSaveBtn = false;
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyMedDeptServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetInwardDrugMedDept_ByIDInvoiceNotPaging_V2(ID, V_MedProductType, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndGetInwardDrugMedDept_ByIDInvoiceNotPaging_V2(out decimal TongTienSPChuaVAT
                                        , out decimal CKTrenSP
                                        , out decimal TongTienTrenSPDaTruCK
                                        , out decimal TongCKTrenHoaDon
                                        , out decimal TongTienHoaDonCoThueNK
                                        , out decimal TongTienHoaDonCoVAT, asyncResult);

                                //load danh sach thuoc theo hoa don 
                                CurrentInwardDrugMedDeptInvoice.InwardDrugs = results.ToObservableCollection();
                                //tinh tong tien 
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

        #region account checking
        private bool _mNhapTraTuKhoPhong_Tim = true;
        private bool _mNhapTraTuKhoPhong_PhieuMoi = true;
        private bool _mNhapTraTuKhoPhong_XemIn = true;
        private bool _mNhapTraTuKhoPhong_In = true;

        public bool mNhapTraTuKhoPhong_Tim
        {
            get
            {
                return _mNhapTraTuKhoPhong_Tim;
            }
            set
            {
                if (_mNhapTraTuKhoPhong_Tim == value)
                    return;
                _mNhapTraTuKhoPhong_Tim = value;
                NotifyOfPropertyChange(() => mNhapTraTuKhoPhong_Tim);
            }
        }

        public bool mNhapTraTuKhoPhong_PhieuMoi
        {
            get
            {
                return _mNhapTraTuKhoPhong_PhieuMoi;
            }
            set
            {
                if (_mNhapTraTuKhoPhong_PhieuMoi == value)
                    return;
                _mNhapTraTuKhoPhong_PhieuMoi = value;
                NotifyOfPropertyChange(() => mNhapTraTuKhoPhong_PhieuMoi);
            }
        }

        public bool mNhapTraTuKhoPhong_XemIn
        {
            get
            {
                return _mNhapTraTuKhoPhong_XemIn;
            }
            set
            {
                if (_mNhapTraTuKhoPhong_XemIn == value)
                    return;
                _mNhapTraTuKhoPhong_XemIn = value;
                NotifyOfPropertyChange(() => mNhapTraTuKhoPhong_XemIn);
            }
        }

        public bool mNhapTraTuKhoPhong_In
        {
            get
            {
                return _mNhapTraTuKhoPhong_In;
            }
            set
            {
                if (_mNhapTraTuKhoPhong_In == value)
                    return;
                _mNhapTraTuKhoPhong_In = value;
                NotifyOfPropertyChange(() => mNhapTraTuKhoPhong_In);
            }
        }
        #endregion

        #region Auto For Location
        private void SetValueSdlDescription(string Name, object item)
        {
            InwardDrugMedDept P = item as InwardDrugMedDept;
            if (P != null)
            {
                P.SdlDescription = Name;
            }
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
            if (Au != null && CurrentInwardDrugMedDept != null)
            {
                if (Au.SelectedItem != null)
                {
                    CurrentInwardDrugMedDept.SelectedShelfDrugLocation = Au.SelectedItem as RefShelfDrugLocation;
                }
                else
                {
                    CurrentInwardDrugMedDept.SdlDescription = Au.Text;
                }
            }
        }
        #endregion

        #region IHandle<DrugDeptCloseSearchInwardIncoiceEvent> Members
        public void Handle(DrugDeptCloseSearchInwardIncoiceEvent message)
        {
            if (message != null && this.IsActive)
            {
                CurrentInwardDrugMedDeptInvoice = message.SelectedInwardInvoice as InwardDrugMedDeptInvoice;
                InwardDrugDetails_ByID(CurrentInwardDrugMedDeptInvoice.inviID);

            }
        }
        #endregion

        #region printing member
        public void btnPreview()
        {
            //22062018 TTM: Đã được thay thế bằng Action.
            //var proAlloc = Globals.GetViewModel<IDrugDeptReportDocumentPreview>();
            //proAlloc.ID = CurrentInwardDrugMedDeptInvoice.inviID;
            //proAlloc.V_MedProductType = V_MedProductType;
            //if (V_MedProductType == (long)AllLookupValues.MedProductType.THUOC)
            //{
            //    proAlloc.LyDo = eHCMSResources.Z0569_G1_PhNhapKhoThuoc.ToUpper();
            //}
            //else if (V_MedProductType == (long)AllLookupValues.MedProductType.Y_CU)
            //{
            //    proAlloc.LyDo = eHCMSResources.Z0570_G1_PhNhapKhoYCu.ToUpper();
            //}
            //else
            //{
            //    proAlloc.LyDo = eHCMSResources.Z0571_G1_PhNhapKhoHChat.ToUpper();
            //}
            //proAlloc.eItem = ReportName.DRUGDEPT_INWARD_MEDDEPTFROMCLINICDEPT;

            //var instance = proAlloc as Conductor<object>;
            //Globals.ShowDialog(instance, (o) => { });

            Action<IDrugDeptReportDocumentPreview> onInitDlg = (proAlloc) =>
            {
                proAlloc.ID = CurrentInwardDrugMedDeptInvoice.inviID;
                proAlloc.V_MedProductType = V_MedProductType;
                //if (V_MedProductType == (long)AllLookupValues.MedProductType.THUOC)
                //{
                //    proAlloc.LyDo = eHCMSResources.Z0569_G1_PhNhapKhoThuoc.ToUpper();
                //}
                //else if (V_MedProductType == (long)AllLookupValues.MedProductType.Y_CU)
                //{
                //    proAlloc.LyDo = eHCMSResources.Z0570_G1_PhNhapKhoYCu.ToUpper();
                //}
                //else
                //{
                //    proAlloc.LyDo = eHCMSResources.Z0571_G1_PhNhapKhoHChat.ToUpper();
                //}
                // 20190301 TNHX: Theo Mau chuan cua khoa duoc gui
                proAlloc.LyDo = "PHIẾU NHẬN HOÀN TRẢ";
                proAlloc.eItem = ReportName.DRUGDEPT_INWARD_MEDDEPTFROMCLINICDEPT;
            };
            GlobalsNAV.ShowDialog<IDrugDeptReportDocumentPreview>(onInitDlg);
        }
        #endregion

        //▼====== #004
        #region Biến để phân quyền
        private bool _eCanChange_DatetimeImProduct = true;
        public bool eCanChange_DatetimeImProduct
        {
            get
            {
                return _eCanChange_DatetimeImProduct;
            }
            set
            {
                if (_eCanChange_DatetimeImProduct == value)
                {
                    return;
                }
                _eCanChange_DatetimeImProduct = value;
                NotifyOfPropertyChange(() => eCanChange_DatetimeImProduct);
            }
        }
        #endregion
        #region Phân quyền
        public void authorization()
        {
            if (!Globals.isAccountCheck)
            {
                return;
            }

                eCanChange_DatetimeImProduct = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mModulesGen, 
                    (int)eModuleGeneral.mCanChange_DatetimeExOrImProduct_DrugDept, (int)oModuleGeneralEX.mCanChange_DatetimeImProduct_DrugDept, (int)ePermission.mView);
        }
        #endregion
        //▲====== #004
    }
}
