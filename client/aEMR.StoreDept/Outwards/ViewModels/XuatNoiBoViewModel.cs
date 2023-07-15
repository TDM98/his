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
using aEMR.CommonTasks;
using aEMR.Controls;
using System.Globalization;
using eHCMSLanguage;
using Castle.Windsor;
/*
 * 20190805 #001 TTM:   BM 0006810: Set focus về textbox số lượng khi chọn thuốc/ y cụ/ hoá chất bằng auto complete
 * 20200116 #002 TNHX: Refactor code and SetDefault TypID = XuatSuDung
 */
namespace aEMR.StoreDept.Outwards.ViewModels
{
    [Export(typeof(IXuatNoiBoStoreDept)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class XuatNoiBoViewModel : Conductor<object>, IXuatNoiBoStoreDept
        , IHandle<DrugDeptCloseSearchOutClinicDeptInvoiceEvent>
        , IHandle<ClinicDeptChooseBatchNumberEvent>
        , IHandle<ClinicDeptChooseBatchNumberResetQtyEvent>
        , IHandle<ItemSelected<Patient>>
    //, IHandle<DrugDeptPayEvent>
    {
        #region Indicator Member

        private bool _isLoadingGetStore = false;
        public bool isLoadingGetStore
        {
            get { return _isLoadingGetStore; }
            set
            {
                if (_isLoadingGetStore != value)
                {
                    _isLoadingGetStore = value;
                    NotifyOfPropertyChange(() => isLoadingGetStore);
                    NotifyOfPropertyChange(() => IsLoading);
                }
            }
        }


        private bool _isLoadingFullOperator = false;
        public bool isLoadingFullOperator
        {
            get { return _isLoadingFullOperator; }
            set
            {
                if (_isLoadingFullOperator != value)
                {
                    _isLoadingFullOperator = value;
                    NotifyOfPropertyChange(() => isLoadingFullOperator);
                    NotifyOfPropertyChange(() => IsLoading);
                }
            }
        }



        private bool _isLoadingGetID = false;
        public bool isLoadingGetID
        {
            get { return _isLoadingGetID; }
            set
            {
                if (_isLoadingGetID != value)
                {
                    _isLoadingGetID = value;
                    NotifyOfPropertyChange(() => isLoadingGetID);
                    NotifyOfPropertyChange(() => IsLoading);
                }
            }
        }

        private bool _isLoadingSearch = false;
        public bool isLoadingSearch
        {
            get { return _isLoadingSearch; }
            set
            {
                if (_isLoadingSearch != value)
                {
                    _isLoadingSearch = value;
                    NotifyOfPropertyChange(() => isLoadingSearch);
                    NotifyOfPropertyChange(() => IsLoading);
                }
            }
        }

        private bool _isLoadingDetail = false;
        public bool isLoadingDetail
        {
            get { return _isLoadingDetail; }
            set
            {
                if (_isLoadingDetail != value)
                {
                    _isLoadingDetail = value;
                    NotifyOfPropertyChange(() => isLoadingDetail);
                    NotifyOfPropertyChange(() => IsLoading);
                }
            }
        }


        private bool _IsLoadingRefGenericDrugCategory = false;
        public bool IsLoadingRefGenericDrugCategory
        {
            get { return _IsLoadingRefGenericDrugCategory; }
            set
            {
                if (_IsLoadingRefGenericDrugCategory != value)
                {
                    _IsLoadingRefGenericDrugCategory = value;
                    NotifyOfPropertyChange(() => IsLoadingRefGenericDrugCategory);
                    NotifyOfPropertyChange(() => IsLoading);
                }
            }
        }

        //--▼-- 28/12/2020 DatTB
        private long _V_GroupTypes = (long)AllLookupValues.V_GroupTypes.TINH_GTGT;
        public long V_GroupTypes
        {
            get => _V_GroupTypes; set
            {
                _V_GroupTypes = value;
                NotifyOfPropertyChange(() => V_GroupTypes);
            }
        }
        //--▲-- 28/12/2020 DatTB

        //--▼--17/12/2020 DatTB
        private bool _Is_Enabled = false;
        public bool Is_Enabled
        {
            get
            {
                return _Is_Enabled;
            }
            set
            {
                if (_Is_Enabled != value)
                {
                    _Is_Enabled = value;
                    NotifyOfPropertyChange(() => Is_Enabled);
                }
            }
        }
        //--▲--17/12/2020 DatTB

        public bool IsLoading
        {
            get { return (isLoadingGetStore || isLoadingFullOperator || isLoadingGetID || isLoadingSearch || isLoadingDetail || IsLoadingRefGenericDrugCategory); }
        }

        #endregion

        [ImportingConstructor]
        public XuatNoiBoViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg)
        {
            authorization();

            Globals.EventAggregator.Subscribe(this);
            RefGenMedProductDetailsList = new ObservableCollection<RefGenMedProductDetails>();

            //Coroutine.BeginExecute(DoGetStoreToSell());
            //StoreCbx = Globals.checkStoreWareHouse(false, false);
            //if (StoreCbx == null || StoreCbx.Count < 1)
            //{
            //    MessageBox.Show(eHCMSResources.A0110_G1_Msg_InfoChuaCauHinhTNKho);
            //}
            //else
            //{
            //    SetDefaultForStore();
            //    isLoadingGetStore = false;
            //}
            SearchCriteria = new MedDeptInvoiceSearchCriteria();

            RefeshData();
            //SetDefaultForStore();
            //Coroutine.BeginExecute(DoGetStoreToReturn());
            //SetDefaultForStoreReturn();
            GetRefOutputTypeLst();
            OutputTos = Globals.AllLookupValueList.Where(x => x.ObjectTypeID == (long)LookupValues.V_OutputTo && (x.LookupID == (long)AllLookupValues.V_OutputTo.KHO_KHAC || x.LookupID == (long)AllLookupValues.V_OutputTo.BENHNHAN || x.LookupID == (long)AllLookupValues.V_OutputTo.KHACH_VANG_LAI)).ToObservableCollection();
            SetDefaultOutputTo();
            Coroutine.BeginExecute(DoGetRefGenericDrugCategory_1List());//--30/12/2020 DatTB
        }

        private void GetReceiptStore()
        {
            //if (V_MedProductType == AllLookupValues.MedProductType.THUOC)
            //{
            //    StoreCbxReturn = ObjectCopier.DeepCopy(Globals.allRefStorageWarehouseLocation.Where(x => x.IsMedicineStore).ToObservableCollection());
            //}
            //else if (V_MedProductType == AllLookupValues.MedProductType.Y_CU)
            //{
            //    StoreCbxReturn = ObjectCopier.DeepCopy(Globals.allRefStorageWarehouseLocation.Where(x => x.IsUtilStore).ToObservableCollection());
            //}
            //20190314 TBL: Bay gio loc kho theo V_MedProductType nen comment ra
            //else if (V_MedProductType == AllLookupValues.MedProductType.HOA_CHAT)
            //{
            //    StoreCbxReturn = ObjectCopier.DeepCopy(Globals.allRefStorageWarehouseLocation.Where(x => x.IsChemicalStore).ToObservableCollection());
            //}
            //--▼--25/12/2020 DatTB
            //StoreCbxReturn = ObjectCopier.DeepCopy(Globals.allRefStorageWarehouseLocation.Where(x => ((long)V_MedProductType != 0 && x.ListV_MedProductType != null && x.ListV_MedProductType.Contains(((long)V_MedProductType).ToString()))).ToObservableCollection());
            //StoreCbxReturn = ObjectCopier.DeepCopy(Globals.IsMainStorageWarehouseLocation.Where(x => ((long)V_MedProductType != 0 && x.ListV_MedProductType != null && x.ListV_MedProductType.Contains(((long)V_MedProductType).ToString()) && (V_GroupTypes == 0 || x.V_GroupTypes == V_GroupTypes) && !x.IsMain)).ToObservableCollection());
            //--▲--25/12/2020 DatTB

            //--▼--04/02/2021 DatTB Lọc không cho kho lẻ trả về kho Chẵn
            if (V_MedProductType == AllLookupValues.MedProductType.THUOC || V_MedProductType == AllLookupValues.MedProductType.Y_CU || V_MedProductType == AllLookupValues.MedProductType.NUTRITION)
            {
                StoreCbxReturn = ObjectCopier.DeepCopy(Globals.IsMainStorageWarehouseLocation.Where(x => ((long)V_MedProductType != 0 && x.ListV_MedProductType != null && x.ListV_MedProductType.Contains(((long)V_MedProductType).ToString()) && (V_GroupTypes == 0 || x.V_GroupTypes == V_GroupTypes) && x.IsSubStorage)).ToObservableCollection());
            }
            else
            {
                StoreCbxReturn = ObjectCopier.DeepCopy(Globals.IsMainStorageWarehouseLocation.Where(x => ((long)V_MedProductType != 0 && x.ListV_MedProductType != null && x.ListV_MedProductType.Contains(((long)V_MedProductType).ToString()) && (V_GroupTypes == 0 || x.V_GroupTypes == V_GroupTypes))).ToObservableCollection());
            }
            //--▲--04/02/2021 DatTB
            SetDefaultForStoreReturn();
        }

        public void InitData()
        {
            if (V_MedProductType == AllLookupValues.MedProductType.THUOC)
            {
                StoreCbx = Globals.checkStoreWareHouse((long)V_MedProductType, false, false, true, false, false);
            }
            else if (V_MedProductType == AllLookupValues.MedProductType.Y_CU)
            {
                StoreCbx = Globals.checkStoreWareHouse((long)V_MedProductType, false, false, false, true, false);
            }
            else if (V_MedProductType == AllLookupValues.MedProductType.NUTRITION)
            {
                StoreCbx = Globals.checkStoreWareHouse((long)V_MedProductType, false, false, true, false, false);
            }
            else if (V_MedProductType == AllLookupValues.MedProductType.HOA_CHAT)
            {
                StoreCbx = Globals.checkStoreWareHouse((long)V_MedProductType, false, false, false, false, true);
            }
            else
            {
                StoreCbx = Globals.checkStoreWareHouse((long)V_MedProductType, false, false, true, true, true);
            }
            if (V_MedProductType == AllLookupValues.MedProductType.MAU)
            {
                IsBloodView = true;
            }
            if (StoreCbx == null || StoreCbx.Count < 1)
            {
                MessageBox.Show(eHCMSResources.A0110_G1_Msg_InfoChuaCauHinhTNKho);
            }
            SetDefaultForStore();
            GetReceiptStore();
        }

        private void RefeshData()
        {
            //SelectedOutInvoice = null; //--30/01/2021 DatTB
            SelectedOutInvoice = new OutwardDrugClinicDeptInvoice();
            SelectedOutInvoice.OutDate = Globals.GetCurServerDateTime();
            SelectedOutInvoice.StaffID = GetStaffLogin().StaffID;
            SelectedOutInvoice.StaffName = GetStaffLogin().FullName;
            SetDefaultForStore();
            SetDefaultForStoreReturn();
            SetDefaultForRefOutputType();
            SetDefultRefGenericDrugCategory(); //--30/12/2020 DatTB
            ClearData();
            TotalPrice = 0;
            IsXuatChongDich = false;
            SetEnableCheckboxXCD();
        }

        private void ClearData()
        {
            SelectedOutInvoice.outiID = 0;
            SelectedOutInvoice.OutInvID = "";

            OutwardDrugClinicDeptsCopy = null;

            if (RefGenMedProductDetailsList == null)
            {
                RefGenMedProductDetailsList = new ObservableCollection<RefGenMedProductDetails>();
            }
            else
            {
                RefGenMedProductDetailsList.Clear();
            }
            if (RefGenMedProductDetailsListSum == null)
            {
                RefGenMedProductDetailsListSum = new ObservableCollection<RefGenMedProductDetails>();
            }
            else
            {
                RefGenMedProductDetailsListSum.Clear();
            }

            if (RefGenMedProductDetailsTemp == null)
            {
                RefGenMedProductDetailsTemp = new ObservableCollection<RefGenMedProductDetails>();
            }
            else
            {
                RefGenMedProductDetailsTemp.Clear();
            }
        }

        private ObservableCollection<Lookup> _OutputTos;
        public ObservableCollection<Lookup> OutputTos
        {
            get { return _OutputTos; }
            set
            {
                if (_OutputTos != value)
                {
                    _OutputTos = value;
                    NotifyOfPropertyChange(() => OutputTos);
                }
            }
        }

        private void SetDefaultOutputTo()
        {
            if (OutputTos != null && SelectedOutInvoice != null)
            {
                SelectedOutInvoice.V_OutputTo = OutputTos.FirstOrDefault().LookupID;
            }
        }

        private void ClearSelectedOutInvoice()
        {
            if (SelectedOutInvoice == null)
            {
                return;
            }

            SelectedOutInvoice.OutputToID = 0;
            SelectedOutInvoice.CustomerName = "";
            SelectedOutInvoice.Address = "";
            SelectedOutInvoice.PhoneNumber = "";
        }

        public void btnChoose()
        {
            if (SelectedOutInvoice != null)
            {
                //if (SelectedOutInvoice.V_OutputTo == (long)AllLookupValues.V_OutputTo.KHO_KHAC)
                //{
                //    var proAlloc1 = Globals.GetViewModel<IDrugDeptStorage>();
                //    proAlloc1.IsChildWindow = true;
                //    var instance1 = proAlloc1 as Conductor<object>;
                //    Globals.ShowDialog(instance1, (o) => { });
                //}
                //else if (SelectedOutInvoice.V_OutputTo == (long)AllLookupValues.V_OutputTo.BVBAN)
                //{
                //    var proAlloc2 = Globals.GetViewModel<IDrugDeptHospitals>();
                //    proAlloc2.IsChildWindow = true;
                //    var instance2 = proAlloc2 as Conductor<object>;
                //    Globals.ShowDialog(instance2, (o) => { });
                //}
                //if (SelectedOutInvoice.V_OutputTo == (long)AllLookupValues.V_OutputTo.BENHNHAN)
                if (SelectedOutInvoice.TypID == (long)AllLookupValues.RefOutputType.XUAT_BN)
                {
                    GlobalsNAV.ShowDialog<IFindPatient>();
                }
                //else if (SelectedOutInvoice.V_OutputTo == (long)AllLookupValues.V_OutputTo.BACSI)
                //{
                //    var proAlloc3 = Globals.GetViewModel<IDrugDeptStaffs>();
                //    proAlloc3.IsChildWindow = true;
                //    var instance3 = proAlloc3 as Conductor<object>;
                //    Globals.ShowDialog(instance3, (o) => { });
                //}
            }
        }

        protected override void OnDeactivate(bool close)
        {
            //KMx: Để Unsubcribe thì khi chuyển từ View Xuất Thuốc, sang Xuất Y Cụ thì double click vào tìm phiếu xuất, bắn event về đây không được (27/12/2014 16:57).
            //Globals.EventAggregator.Unsubscribe(this);

            SelectedOutInvoice = null;
            SearchCriteria = null;
            OutwardDrugClinicDeptsCopy = null;
            RefGenMedProductDetailsList = null;
            RefGenMedProductDetailsTemp = null;
            RefGenMedProductDetailsListSum = null;
            StoreCbx = null;

            BatchNumberListTemp = null;
            BatchNumberListShow = null;
            OutwardDrugMedDeptListByGenMedProductID = null;
            OutwardDrugMedDeptListByGenMedProductIDFirst = null;
            SelectedOutwardDrugClinicDept = null;
            if (au != null)
            {
                au.SetValue(AutoCompleteBox.ItemsSourceProperty, null);
            }
            if (grdPrescription != null)
            {
                grdPrescription.SetValue(DataGrid.ItemsSourceProperty, null);
            }
        }

        private void SetDefaultForStore()
        {
            if (StoreCbx != null && SelectedOutInvoice != null && StoreCbx.Count > 0)
            {
                SelectedOutInvoice.StoreID = StoreCbx.FirstOrDefault().StoreID;
                //--▼-- 29/12/2020 DatTB Gán biến mặc định 
                var selectedStore = (RefStorageWarehouseLocation)StoreCbx.FirstOrDefault();
                V_GroupTypes = selectedStore.V_GroupTypes;
                //--▲-- 29/12/2020 DatTB 
            }
        }

        private void SetDefaultForStoreReturn()
        {
            if (StoreCbxReturn != null && SelectedOutInvoice != null && StoreCbxReturn.Count > 0)
            {
                SelectedOutInvoice.OutputToID = StoreCbxReturn.FirstOrDefault().StoreID;
            }
        }

        private void SetDefaultForRefOutputType()
        {
            if (RefOutputTypeLst != null && SelectedOutInvoice != null)
            {
                if (!IsBalanceView)
                {
                    SelectedOutInvoice.TypID = RefOutputTypeLst.Where(x => x.TypID == (long)AllLookupValues.RefOutputType.XUAT_DUNGCHUNG).FirstOrDefault().TypID;
                    SetEnable();
                }
                else
                {
                    SelectedOutInvoice.TypID = RefOutputTypeLst.Where(x => x.TypID == (long)AllLookupValues.RefOutputType.CANBANGKHO).FirstOrDefault().TypID;
                    SelectedOutInvoice.OutputToID = 0;
                }

                //--▼--29/12/2020 DatTB
                if (SelectedOutInvoice.TypID == 3)
                {
                    Is_Enabled = true;
                }
                //--▲--29/12/2020 DatTB
            }
        }

        public void authorization()
        {
            if (!Globals.isAccountCheck)
            {
                return;
            }

            eCanChange_DatetimeExProduct = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mModulesGen,
      (int)eModuleGeneral.mCanChange_DatetimeExOrImProduct_StoreDept, (int)oModuleGeneralEX.mCanChange_DatetimeExProduct_StoreDept, (int)ePermission.mView);
        }

        #region checking account

        private bool _mXuatTraHang_Xem = true;
        private bool _mXuatTraHang_PhieuMoi = true;
        private bool _mXuatTraHang_XemIn = true;
        private bool _mXuatTraHang_In = true;

        public bool mXuatTraHang_Xem
        {
            get
            {
                return _mXuatTraHang_Xem;
            }
            set
            {
                if (_mXuatTraHang_Xem == value)
                    return;
                _mXuatTraHang_Xem = value;
                NotifyOfPropertyChange(() => mXuatTraHang_Xem);
            }
        }

        public bool mXuatTraHang_PhieuMoi
        {
            get
            {
                return _mXuatTraHang_PhieuMoi;
            }
            set
            {
                if (_mXuatTraHang_PhieuMoi == value)
                    return;
                _mXuatTraHang_PhieuMoi = value;
                NotifyOfPropertyChange(() => mXuatTraHang_PhieuMoi);
            }
        }

        public bool mXuatTraHang_XemIn
        {
            get
            {
                return _mXuatTraHang_XemIn;
            }
            set
            {
                if (_mXuatTraHang_XemIn == value)
                    return;
                _mXuatTraHang_XemIn = value;
                NotifyOfPropertyChange(() => mXuatTraHang_XemIn);
            }
        }

        public bool mXuatTraHang_In
        {
            get
            {
                return _mXuatTraHang_In;
            }
            set
            {
                if (_mXuatTraHang_In == value)
                    return;
                _mXuatTraHang_In = value;
                NotifyOfPropertyChange(() => mXuatTraHang_In);
            }
        }
        #endregion

        #region Properties Member

        private bool _IsBloodView = false;
        public bool IsBloodView
        {
            get
            {
                return _IsBloodView;
            }
            set
            {
                if (_IsBloodView == value)
                {
                    return;
                }
                _IsBloodView = value;
                NotifyOfPropertyChange(() => IsBloodView);
            }
        }

        private bool _eCanChange_DatetimeExProduct = true;
        public bool eCanChange_DatetimeExProduct
        {
            get
            {
                return _eCanChange_DatetimeExProduct;
            }
            set
            {
                if (_eCanChange_DatetimeExProduct == value)
                {
                    return;
                }
                _eCanChange_DatetimeExProduct = value;
                NotifyOfPropertyChange(() => eCanChange_DatetimeExProduct);
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

        private AllLookupValues.MedProductType _V_MedProductType = AllLookupValues.MedProductType.THUOC; //11001 : thuoc, 11002 : y cu , 11003 :hoa chat
        public AllLookupValues.MedProductType V_MedProductType
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
                    NotifyOfPropertyChange(() => CanSelectedRefGenDrugCatID_1); //--30/12/2020 DatTB
                }

            }
        }


        //KMx: Mặc định lấy giá bán, không lấy giá vốn (vì có loại xuất cho BN) (27/04/2015 17:47).
        //private bool? IsCost = true;
        private bool? IsCost = false;

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

        private ObservableCollection<RefStorageWarehouseLocation> _StoreCbxReturn;
        public ObservableCollection<RefStorageWarehouseLocation> StoreCbxReturn
        {
            get
            {
                return _StoreCbxReturn;
            }
            set
            {
                if (_StoreCbxReturn != value)
                {
                    _StoreCbxReturn = value;
                    NotifyOfPropertyChange(() => StoreCbxReturn);
                }
            }
        }

        private string _StaffName;
        public string StaffName
        {
            get { return _StaffName; }
            set
            {
                _StaffName = value;
                NotifyOfPropertyChange(() => StaffName);
            }
        }

        private Staff GetStaffLogin()
        {
            StaffName = Globals.LoggedUserAccount.Staff.FullName;
            return Globals.LoggedUserAccount.Staff;
        }

        private MedDeptInvoiceSearchCriteria _SearchCriteria;
        public MedDeptInvoiceSearchCriteria SearchCriteria
        {
            get
            {
                return _SearchCriteria;
            }
            set
            {
                if (_SearchCriteria != value)
                {
                    _SearchCriteria = value;
                    NotifyOfPropertyChange(() => SearchCriteria);
                }
            }
        }

        private ObservableCollection<OutwardDrugClinicDept> OutwardDrugClinicDeptsCopy;

        private OutwardDrugClinicDeptInvoice SelectedOutInvoiceCoppy;

        private OutwardDrugClinicDeptInvoice _SelectedOutInvoice;
        public OutwardDrugClinicDeptInvoice SelectedOutInvoice
        {
            get
            {
                return _SelectedOutInvoice;
            }
            set
            {
                if (_SelectedOutInvoice != value)
                {
                    _SelectedOutInvoice = value;
                }
                NotifyOfPropertyChange(() => SelectedOutInvoice);
                NotifyOfPropertyChange(() => VisibilityName);
                NotifyOfPropertyChange(() => VisibilityCode);
            }
        }


        private ObservableCollection<RefOutputType> _RefOutputTypeLst;
        public ObservableCollection<RefOutputType> RefOutputTypeLst
        {
            get
            {
                return _RefOutputTypeLst;
            }
            set
            {
                if (_RefOutputTypeLst != value)
                {
                    _RefOutputTypeLst = value;
                    NotifyOfPropertyChange(() => RefOutputTypeLst);
                }
            }
        }

        private decimal _TotalPrice;
        public decimal TotalPrice
        {
            get
            {
                return _TotalPrice;
            }
            set
            {
                _TotalPrice = value;
                NotifyOfPropertyChange(() => TotalPrice);
            }
        }

        //--▼--30/12/2020 DatTB
        private ObservableCollection<RefGenericDrugCategory_1> _RefGenericDrugCategory_1s;
        public ObservableCollection<RefGenericDrugCategory_1> RefGenericDrugCategory_1s
        {
            get
            {
                return _RefGenericDrugCategory_1s;
            }
            set
            {
                if (_RefGenericDrugCategory_1s != value)
                {
                    _RefGenericDrugCategory_1s = value;
                    NotifyOfPropertyChange(() => RefGenericDrugCategory_1s);
                }
            }
        }

        private bool flag = true;
        public void KeyEnabledComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (flag)
            {
                if (SelectedOutInvoice != null && SelectedOutInvoice.OutwardDrugClinicDepts != null)
                {
                    SelectedOutInvoice.OutwardDrugClinicDepts.Clear();
                }
            }
            flag = true;
        }
        //--▲--30/12/2020 DatTB

        #endregion

        //--▼--30/12/2020 DatTB
        private IEnumerator<IResult> DoGetRefGenericDrugCategory_1List()
        {
            var paymentTypeTask = new LoadRefGenericDrugCategory_1ListTask((long)V_MedProductType, false, false);
            yield return paymentTypeTask;
            RefGenericDrugCategory_1s = paymentTypeTask.RefGenericDrugCategory_1List;
            SetDefultRefGenericDrugCategory();
            yield break;
        }

        private void SetDefultRefGenericDrugCategory()
        {
            if (SelectedOutInvoice != null && RefGenericDrugCategory_1s != null)
            {
                SelectedOutInvoice.RefGenDrugCatID_1 = RefGenericDrugCategory_1s.FirstOrDefault().RefGenDrugCatID_1;
            }
        }

        public bool CanSelectedRefGenDrugCatID_1
        {
            get { return (long)V_MedProductType == (long)AllLookupValues.MedProductType.THUOC; }
        }

        //--▲--30/12/2020 DatTB

        private IEnumerator<IResult> DoGetStoreToSell()
        {
            isLoadingGetStore = true;
            var paymentTypeTask = new LoadStoreListTask((long)AllLookupValues.StoreType.STORAGE_CLINIC, false, null, false, false);
            yield return paymentTypeTask;
            StoreCbx = paymentTypeTask.LookupList;
            SetDefaultForStore();
            isLoadingGetStore = false;
            yield break;
        }
        //private IEnumerator<IResult> DoGetStoreToReturn()
        //{
        //    isLoadingGetStore = true;
        //    //var paymentTypeTask = new LoadStoreListTask((long)AllLookupValues.StoreType.STORAGE_DRUGDEPT, false, null,false, false);
        //    var paymentTypeTask = new LoadStoreListTask((long)AllLookupValues.StoreType.STORAGE_CLINIC, false, null, false, false);
        //    yield return paymentTypeTask;
        //    StoreCbxReturn = paymentTypeTask.LookupList;
        //    SetDefaultForStoreReturn();
        //    isLoadingGetStore = false;
        //    yield break;
        //}

        //private void GetRefOutputTypeLst()
        //{
        //    if (RefOutputTypeLst == null)
        //    {
        //        RefOutputTypeLst = new ObservableCollection<RefOutputType>();
        //    }
        //    RefOutputTypeLst.Clear();
        //    RefOutputType item1 = new RefOutputType();
        //    item1.TypID = (long)AllLookupValues.RefOutputType.XUATNOIBO;
        //    item1.TypName = "Xuất nội bộ";
        //    RefOutputType item2 = new RefOutputType();
        //    item2.TypID = (long)AllLookupValues.RefOutputType.HUYHANG;
        //    item2.TypName = "Xuất hủy hàng";
        //    RefOutputTypeLst.Add(item1);
        //    RefOutputTypeLst.Add(item2);

        //    SetDefaultForRefOutputType();
        //}

        public void GetRefOutputTypeLst()
        {
            if (RefOutputTypeLst == null)
            {
                RefOutputTypeLst = new ObservableCollection<RefOutputType>();
            }
            if (!IsBalanceView)
            {
                RefOutputTypeLst = Globals.RefOutputType.Where(x => x.IsSelectedClinicDept == true).ToObservableCollection();
            }
            else
            {
                RefOutputTypeLst = Globals.RefOutputType.Where(x => x.TypID == (long)AllLookupValues.RefOutputType.CANBANGKHO).ToObservableCollection();
            }
            SetDefaultForRefOutputType();
        }

        public void grdPrescription_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString() + ". ";

            if (SelectedOutInvoice != null && SelectedOutInvoice.CanSaveAndPaid)
            {
                Button colBatchNumber = grdPrescription.Columns[5].GetCellContent(e.Row) as Button;
                if (colBatchNumber != null)
                {
                    colBatchNumber.IsEnabled = true;
                }
            }
            else
            {
                Button colBatchNumber = grdPrescription.Columns[5].GetCellContent(e.Row) as Button;
                if (colBatchNumber != null)
                {
                    colBatchNumber.IsEnabled = false;
                }
            }
        }

        //ChangedWPF-CMN
        //public void grdPrescription_RowEditEnded(object sender, DataGridRowEditEndedEventArgs e)
        //{
        //    SumTotalPrice();
        //}

        private bool CheckValid()
        {
            bool result = true;
            if (SelectedOutInvoice != null)
            {
                if (SelectedOutInvoice.OutwardDrugClinicDepts == null)
                {
                    return false;
                }
                foreach (OutwardDrugClinicDept item in SelectedOutInvoice.OutwardDrugClinicDepts)
                {
                    if (item.Validate() == false)
                    {
                        result = false;
                        break;
                    }
                }
            }
            return result;
        }

        private void GetOutwardDrugClinicDeptInvoice(long OutwardID)
        {
            isLoadingGetID = true;
            // Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyClinicDeptServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetOutwardDrugClinicDeptInvoice_V2(OutwardID, (long)V_MedProductType, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            SelectedOutInvoice = contract.EndGetOutwardDrugClinicDeptInvoice_V2(asyncResult);
                            //co khong cho vao cac su kien 
                            OutwardDrugClinicDeptDetails_Load(SelectedOutInvoice.outiID);
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            isLoadingGetID = false;
                            // Globals.IsBusy = false;
                        }
                    }), null);
                }
            });

            t.Start();
        }

        public void btnSearch(object sender, RoutedEventArgs e)
        {
            OutwardDrugClinicDeptInvoice_Search(0, 20);
        }

        private void OutwardDrugClinicDeptInvoice_Search(int PageIndex, int PageSize)
        {
            if (SearchCriteria == null)
            {
                SearchCriteria = new MedDeptInvoiceSearchCriteria();
            }
            //SearchCriteria.StoreID =;
            if (!IsBalanceView)
            {
                SearchCriteria.TypID = (long)AllLookupValues.RefOutputType.XUATNOIBO;
            }
            else
            {
                SearchCriteria.TypID = (long)AllLookupValues.RefOutputType.CANBANGKHO;
            }
            SearchCriteria.V_MedProductType = (long)V_MedProductType;

            if (SelectedOutInvoice != null && SelectedOutInvoice.StoreID.HasValue && SelectedOutInvoice.StoreID > 0)
            {
                SearchCriteria.StoreID = SelectedOutInvoice.StoreID.Value;
            }

            isLoadingSearch = true;
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyClinicDeptServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginOutwardDrugClinicDeptInvoice_SearchByType(SearchCriteria, PageIndex, PageSize, true, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            int TotalCount = 0;
                            var results = contract.EndOutwardDrugClinicDeptInvoice_SearchByType(out TotalCount, asyncResult);
                            if (results != null && results.Count > 0)
                            {
                                if (results.Count > 1)
                                {
                                    //mo pop up tim
                                    void onInitDlg(IXuatNoiBoSearchClinicDept proAlloc)
                                    {
                                        proAlloc.SearchCriteria = SearchCriteria.DeepCopy();
                                        proAlloc.OutwardClinicDeptInvoiceList.Clear();
                                        proAlloc.OutwardClinicDeptInvoiceList.TotalItemCount = TotalCount;
                                        proAlloc.OutwardClinicDeptInvoiceList.PageIndex = 0;
                                        proAlloc.OutwardClinicDeptInvoiceList.PageSize = 20;
                                        foreach (OutwardDrugClinicDeptInvoice p in results)
                                        {
                                            proAlloc.OutwardClinicDeptInvoiceList.Add(p);
                                        }
                                    }
                                    GlobalsNAV.ShowDialog<IXuatNoiBoSearchClinicDept>(onInitDlg);
                                }
                                else
                                {
                                    SelectedOutInvoice = results.FirstOrDefault();
                                    //load detail
                                    OutwardDrugClinicDeptDetails_Load(SelectedOutInvoice.outiID);
                                }
                            }
                            else
                            {
                                Globals.ShowMessage(eHCMSResources.Z0863_G1_KgTimThay, eHCMSResources.G0442_G1_TBao);
                            }
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            isLoadingSearch = false;
                            //Globals.IsBusy = false;
                        }
                    }), null);
                }
            });

            t.Start();
        }

        private void OutwardDrugClinicDeptDetails_Load(long outiID)
        {
            isLoadingDetail = true;
            // Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyClinicDeptServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetOutwardDrugClinicDeptDetailByInvoice_V2(outiID, (long)V_MedProductType, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndGetOutwardDrugClinicDeptDetailByInvoice_V2(asyncResult);
                            //load danh sach thuoc theo hoa don 
                            SelectedOutInvoice.OutwardDrugClinicDepts = results.ToObservableCollection();
                            SumTotalPrice();
                            DeepCopyOutwardDrugMedDept();
                            IsEnableStorageCbx = false; //--07/01/2021 DatTB Disable chọn kho xuất
                            //if (SelectedOutInvoice.Notes != null)
                            if(SelectedOutInvoice.Notes != null && SelectedOutInvoice.Notes.Contains("[XCD]"))
                            {
                                if (SelectedOutInvoice.Notes.Contains("[XCD]"))
                                {
                                    IsXuatChongDich = true;
                                }
                                else
                                {
                                    IsXuatChongDich = false;
                                }
                            }
                            else
                            {
                                IsXuatChongDich = false;
                            }
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            isLoadingDetail = false;
                            // Globals.IsBusy = false;
                        }
                    }), null);
                }
            });

            t.Start();
        }

        public void Search_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (SearchCriteria != null)
                {
                    SearchCriteria.CodeInvoice = (sender as TextBox).Text;
                }
                OutwardDrugClinicDeptInvoice_Search(0, 20);
            }
        }

        private void SumTotalPrice()
        {
            TotalPrice = 0;
            if (SelectedOutInvoice != null)
            {
                SelectedOutInvoice.TotalInvoicePrice = 0;
                if (SelectedOutInvoice.OutwardDrugClinicDepts != null)
                {
                    for (int i = 0; i < SelectedOutInvoice.OutwardDrugClinicDepts.Count; i++)
                    {
                        TotalPrice += SelectedOutInvoice.OutwardDrugClinicDepts[i].OutAmount.GetValueOrDefault();
                    }
                    SelectedOutInvoice.TotalInvoicePrice = TotalPrice;
                }
            }
        }

        DataGrid grdPrescription = null;
        public void grdPrescription_Loaded(object sender, RoutedEventArgs e)
        {
            grdPrescription = sender as DataGrid;
        }

        public void grdPrescription_Unloaded(object sender, RoutedEventArgs e)
        {
            grdPrescription.SetValue(DataGrid.ItemsSourceProperty, null);
        }

        #region printing member

        public void btnPreview()
        {
            Action<IClinicDeptReportDocumentPreview> onInitDlg = delegate (IClinicDeptReportDocumentPreview proAlloc)
            {
                proAlloc.ID = SelectedOutInvoice.outiID;

                string V_MedProductTypeName = "";

                if (V_MedProductType == AllLookupValues.MedProductType.THUOC)
                {
                    V_MedProductTypeName = " THUỐC ";

                }
                else if (V_MedProductType == AllLookupValues.MedProductType.Y_CU)
                {
                    V_MedProductTypeName = " Y CỤ ";

                }
                else if (V_MedProductType == AllLookupValues.MedProductType.NUTRITION)
                {
                    V_MedProductTypeName = " DINH DƯỠNG ";

                }
                else if (V_MedProductType == AllLookupValues.MedProductType.VTYT_TIEUHAO)
                {
                    V_MedProductTypeName = " VẬT TƯ Y TẾ - TIÊU HAO ";

                }
                else if (V_MedProductType == AllLookupValues.MedProductType.THANHTRUNG)
                {
                    V_MedProductTypeName = " THANH TRÙNG ";

                }
                else if (V_MedProductType == AllLookupValues.MedProductType.MAU)
                {
                    V_MedProductTypeName = " MÁU ";

                }
                else if (V_MedProductType == AllLookupValues.MedProductType.TIEM_NGUA)
                {
                    V_MedProductTypeName = " VẮC - XIN ";

                }
                else
                {
                    V_MedProductTypeName = " HÓA CHẤT ";
                }

                if (SelectedOutInvoice.TypID == (long)AllLookupValues.RefOutputType.HUYHANG)
                {
                    proAlloc.LyDo = "XUẤT HỦY" + V_MedProductTypeName;
                }
                else if (SelectedOutInvoice.TypID == (long)AllLookupValues.RefOutputType.XUAT_BN)
                {
                    proAlloc.LyDo = "XUẤT BÁN LẺ" + V_MedProductTypeName;
                }
                else if (SelectedOutInvoice.TypID == (long)AllLookupValues.RefOutputType.XUAT_KHOPHONG)
                {
                    proAlloc.LyDo = "XUẤT NỘI BỘ" + V_MedProductTypeName;
                }
                else if (SelectedOutInvoice.TypID == (long)AllLookupValues.RefOutputType.XUATNOIBO_LUANCHUYENKHO)
                {
                    proAlloc.LyDo = "LUÂN CHUYỂN KHO" + V_MedProductTypeName;
                }
                else if (SelectedOutInvoice.TypID == (long)AllLookupValues.RefOutputType.XUAT_TRA_KHOADUOC)
                {
                    proAlloc.LyDo = "XUẤT TRẢ" + V_MedProductTypeName;
                }
                else if (SelectedOutInvoice.TypID == (long)AllLookupValues.RefOutputType.XUAT_DUNGCHUNG)
                {
                    proAlloc.LyDo = "XUẤT SỬ DỤNG" + V_MedProductTypeName;
                }
                proAlloc.V_MedProductType = (long)V_MedProductType;
                proAlloc.eItem = ReportName.CLINICDEPT_OUTWARD_DRUGDEPT;
            };
            GlobalsNAV.ShowDialog<IClinicDeptReportDocumentPreview>(onInitDlg);
        }

        public void btnPreviewPhieuTruyenMau()
        {
            void onInitDlg(IClinicDeptReportDocumentPreview proAlloc)
            {
                proAlloc.ID = SelectedOutInvoice.outiID;
                proAlloc.eItem = ReportName.PHIEU_TRUYEN_MAU;
            }
            GlobalsNAV.ShowDialog<IClinicDeptReportDocumentPreview>(onInitDlg);
        }

        public void btnPrint()
        {
            MessageBox.Show("Thoi gian toi se hoan thanh.Vui long doi!");
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            //var t = new Thread(() =>
            //{
            //    using (var serviceFactory = new ReportServiceClient())
            //    {
            //        var contract = serviceFactory.ServiceInstance;

            //        contract.BeginGetOutwardInternalInPdfFormat(SelectedOutInvoice.outiID, Globals.DispatchCallback((asyncResult) =>
            //        {
            //            try
            //            {
            //                var results = contract.EndGetOutwardInternalInPdfFormat(asyncResult);
            //                var printEvt = new ActiveXPrintEvt(this, PrinterType.IN_PHIEU, results, ActiveXPrintType.ByteArray);
            //                Globals.EventAggregator.Publish(results);
            //            }
            //            catch (Exception ex)
            //            {
            //                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
            //            }
            //            finally
            //            {
            //                Globals.IsBusy = false;
            //            }

            //        }), null);

            //    }

            //});
            //t.Start();
        }

        #endregion

        #region auto Drug For Prescription member
        private string BrandName;
        private bool IsHIPatient = false;

        private ObservableCollection<RefGenMedProductDetails> RefGenMedProductDetailsList;

        private ObservableCollection<RefGenMedProductDetails> _RefGenMedProductDetailsSum;
        public ObservableCollection<RefGenMedProductDetails> RefGenMedProductDetailsListSum
        {
            get { return _RefGenMedProductDetailsSum; }
            set
            {
                if (_RefGenMedProductDetailsSum != value)
                    _RefGenMedProductDetailsSum = value;
                NotifyOfPropertyChange(() => RefGenMedProductDetailsListSum);
            }
        }

        private ObservableCollection<RefGenMedProductDetails> RefGenMedProductDetailsTemp;

        private RefGenMedProductDetails _SelectedSellVisitor;
        public RefGenMedProductDetails SelectedSellVisitor
        {
            get { return _SelectedSellVisitor; }
            set
            {
                if (_SelectedSellVisitor != value)
                    _SelectedSellVisitor = value;
                NotifyOfPropertyChange(() => SelectedSellVisitor);
            }
        }

        AutoCompleteBox au;
        public void AutoDrug_Text_Loaded(object sender, RoutedEventArgs e)
        {
            au = sender as AutoCompleteBox;
        }

        public void AutoCompleteBox_Populating(object sender, PopulatingEventArgs e)
        {
            if (!IsCode.GetValueOrDefault())
            {
                BrandName = e.Parameter;
                //--▼--002--29/01/2021 DatTB Store có lọc theo V_MedProductType không cần phân ở đây
                //if (ViewCase == 0 || ViewCase == 1)
                if(ViewCase == 0)
                {
                    SearchRefGenMedProductDetails(e.Parameter, false);
                }
                else
                {
                    SearchRefGenMedProductDetails_Balance_FromCategory(e.Parameter, false);
                }
                //if (V_MedProductType == AllLookupValues.MedProductType.THUOC || V_MedProductType == AllLookupValues.MedProductType.Y_CU)
                //{
                //    if (ViewCase == 0 || ViewCase == 1)
                //    {
                //        SearchRefGenMedProductDetails(e.Parameter, false);
                //    }
                //    else
                //    {
                //        SearchRefGenMedProductDetails_Balance_FromCategory(e.Parameter, false);
                //    }
                //}
                //else if (V_MedProductType == AllLookupValues.MedProductType.VTYT_TIEUHAO)
                //{
                //    SearchRefGenMedProductDetails_VTYTTH(e.Parameter, false);
                //}
                //else if (V_MedProductType == AllLookupValues.MedProductType.MAU)
                //{
                //    SearchRefGenMedProductDetails_Blood(e.Parameter, false);
                //}
                //else if (V_MedProductType == AllLookupValues.MedProductType.THANHTRUNG)
                //{
                //    SearchRefGenMedProductDetails_ThanhTrung(e.Parameter, false);
                //}
                //else if (V_MedProductType == AllLookupValues.MedProductType.TIEM_NGUA)
                //{
                //    SearchRefGenMedProductDetails_TiemNgua(e.Parameter, false);
                //}
                //else if (V_MedProductType == AllLookupValues.MedProductType.VAN_PHONG_PHAM)
                //{
                //    SearchRefGenMedProductDetails_VPP(e.Parameter, false);
                //}
                //else if (V_MedProductType == AllLookupValues.MedProductType.HOA_CHAT)
                //{
                //    if (ViewCase == 0 || ViewCase == 1)
                //    {
                //        SearchRefGenMedProductDetails_Chemical(e.Parameter, false);
                //    }
                //    else
                //    {
                //        SearchRefGenMedProductDetails_Balance_FromCategory(e.Parameter, false);
                //    }

                //}
                //else if (V_MedProductType == AllLookupValues.MedProductType.VATTU_TIEUHAO)
                //{
                //    SearchRefGenMedProductDetails_VTTH(e.Parameter, false);
                //}
                //--▲--002--29/01/2021 DatTB Store có lọc theo V_MedProductType không cần phân ở đây
            }
        }

        public void AutoDrug_Text_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            if (au.SelectedItem != null)
            {
                SelectedSellVisitor = au.SelectedItem as RefGenMedProductDetails;
                //▼======== #001
                if (AxQty != null)
                {
                    AxQty.Focus();
                }
                //▲======= #001
            }
        }

        private void ListDisplayAutoComplete()
        {
            var hhh = from hd in RefGenMedProductDetailsList
                      group hd by new { hd.GenMedProductID, hd.BrandName, hd.SelectedUnit.UnitName, hd.RequestQty, hd.Code} into hdgroup
                      where hdgroup.Sum(i => i.Remaining) > 0 //20210402 QTD thêm điều kiện bỏ thuốc không còn remaining sử dụng khi giảm kiểm kê
                      select new
                      {
                          Remaining = hdgroup.Sum(groupItem => groupItem.Remaining),
                          GenMedProductID = hdgroup.Key.GenMedProductID,
                          UnitName = hdgroup.Key.UnitName,
                          BrandName = hdgroup.Key.BrandName,
                          Code = hdgroup.Key.Code,
                          Qty = hdgroup.Key.RequestQty
                      };

            //KMx: Phải new rồi mới add. Nếu clear rồi add thì bị chậm (09/07/2014 11:58).
            RefGenMedProductDetailsListSum = new ObservableCollection<RefGenMedProductDetails>();
            foreach (var i in hhh)
            {
                RefGenMedProductDetails item = new RefGenMedProductDetails();
                item.GenMedProductID = i.GenMedProductID;
                item.BrandName = i.BrandName;
                item.SelectedUnit = new RefUnit();
                item.SelectedUnit.UnitName = i.UnitName;
                item.Code = i.Code;
                item.Remaining = i.Remaining;
                item.RequestQty = i.Qty;
                RefGenMedProductDetailsListSum.Add(item);
            }

            if (IsCode.GetValueOrDefault())
            {
                if (RefGenMedProductDetailsListSum != null && RefGenMedProductDetailsListSum.Count > 0)
                {
                    //var item = RefGenMedProductDetailsListSum.Where(x => x.Code == txt);
                    //if (item != null && item.Count() > 0)
                    {
                        SelectedSellVisitor = RefGenMedProductDetailsListSum.ToList()[0];
                    }
                    //else
                    //{
                    //    MessageBox.Show(eHCMSResources.A0752_G1_Msg_InfoKhTimThay);
                    //}
                }
                else
                {
                    SelectedSellVisitor = null;

                    if (tbx != null)
                    {
                        txt = "";
                        tbx.Text = "";
                    }
                    if (au != null)
                    {
                        au.Text = "";
                    }
                    MessageBox.Show(eHCMSResources.A0752_G1_Msg_InfoKhTimThay);
                }
            }
            else
            {
                if (au != null)
                {
                    au.ItemsSource = RefGenMedProductDetailsListSum;
                    au.PopulateComplete();
                }
            }
        }

        private void SearchRefGenMedProductDetails(string Name, bool? IsCode)
        {
            if (SelectedOutInvoice == null)
            {
                SelectedOutInvoice = new OutwardDrugClinicDeptInvoice();
            }
            if (SelectedOutInvoice.StoreID == null || SelectedOutInvoice.StoreID.GetValueOrDefault(0) <= 0)
            {
                MessageBox.Show(eHCMSResources.K0333_G1_ChonKhoXuat);
                return;
            }
            if (IsCode.GetValueOrDefault())
            {
                isLoadingDetail = true;
            }

            //--▼--28/01/2021 DatTB
            long OutputID = 0;

            if (SelectedOutInvoice.TypID == (long)AllLookupValues.RefOutputType.XUAT_TRA_KHOADUOC)
            {
                OutputID = (long)cbo.SelectedValue; 
            }
            //--▲--28/01/2021 DatTB

            //--▼--30/12/2020 DatTB
            long? RequestID = null;
            long? RefGenDrugCatID_1 = null;

            RequestID = SelectedOutInvoice.ReqDrugInClinicDeptID;

            if ((long)V_MedProductType == (long)AllLookupValues.MedProductType.THUOC)
            {
                RefGenDrugCatID_1 = SelectedOutInvoice.RefGenDrugCatID_1;
            }
            //--▲--30/12/2020 DatTB

            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyClinicDeptServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetDrugForSellVisitorAutoComplete_ForRequestClinicDept(IsCost, Name, SelectedOutInvoice.StoreID.GetValueOrDefault(0), (long)V_MedProductType, RefGenDrugCatID_1, null, IsCode, null, null, OutputID /*--28/01/2021 DatTB Thêm biến*/, Globals.DispatchCallback((asyncResult) =>
                      {
                          try
                          {
                              var results = contract.EndGetDrugForSellVisitorAutoComplete_ForRequestClinicDept(asyncResult);

                              RefGenMedProductDetailsList = new ObservableCollection<RefGenMedProductDetails>();
                              RefGenMedProductDetailsTemp = new ObservableCollection<RefGenMedProductDetails>();

                              RefGenMedProductDetailsTemp = results.ToObservableCollection();

                              if (OutwardDrugClinicDeptsCopy != null && OutwardDrugClinicDeptsCopy.Count > 0)
                              {
                                  foreach (OutwardDrugClinicDept d in OutwardDrugClinicDeptsCopy)
                                  {
                                      var value = results.Where(x => x.GenMedProductID == d.GenMedProductID && x.InBatchNumber == d.InBatchNumber && x.InID == d.InID);
                                      if (value.Count() > 0)
                                      {
                                          foreach (RefGenMedProductDetails s in value.ToList())
                                          {
                                              s.Remaining = s.Remaining + d.OutQuantityOld;
                                              s.RemainingFirst = s.RemainingFirst + d.OutQuantityOld;
                                          }
                                      }
                                      //KMx: Nếu hàng trong grid không có trong AutoComplete thì thôi, bỏ vào AutoComplete cũng không hiển thị được (09/07/2014 11:56).
                                      //else
                                      //{
                                      //    RefGenMedProductDetails p = d.GenMedProductItem;
                                      //    p.Remaining = d.OutQuantity;
                                      //    p.RemainingFirst = d.OutQuantity;
                                      //    p.InBatchNumber = d.InBatchNumber;
                                      //    p.OutPrice = d.OutPrice;
                                      //    p.InID = Convert.ToInt64(d.InID);
                                      //    p.STT = d.STT;
                                      //    RefGenMedProductDetailsTemp.Add(p);
                                      //    // d = null;
                                      //}
                                  }
                              }
                              foreach (RefGenMedProductDetails s in RefGenMedProductDetailsTemp)
                              {
                                  if (SelectedOutInvoice.OutwardDrugClinicDepts != null && SelectedOutInvoice.OutwardDrugClinicDepts.Count > 0)
                                  {
                                      foreach (OutwardDrugClinicDept d in SelectedOutInvoice.OutwardDrugClinicDepts)
                                      {
                                          if (d.GenMedProductID == s.GenMedProductID && d.InBatchNumber == s.InBatchNumber && d.InID == s.InID)
                                          {
                                              s.Remaining = s.Remaining - d.OutQuantity;
                                          }
                                      }
                                  }
                                  RefGenMedProductDetailsList.Add(s);
                              }
                              ListDisplayAutoComplete();
                          }
                          catch (Exception ex)
                          {
                              Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                          }
                          finally
                          {
                              //if (IsCode.GetValueOrDefault())
                              //{
                              //    isLoadingDetail = false;
                              //    if (AxQty != null)
                              //    {
                              //        AxQty.Focus();
                              //    }
                              //}
                              //else
                              //{
                              //    if (au != null && !au.IsFocused)
                              //    {
                              //        au.Focus();
                              //    }
                              //}
                          }
                      }), null);
                }
            });

            t.Start();
        }

        private bool CheckValidDrugAuto(RefGenMedProductDetails temp)
        {
            if (temp == null)
            {
                return false;
            }
            return !temp.HasErrors;
        }

        private void CheckBatchNumberExists(OutwardDrugClinicDept p)
        {
            bool kq = false;
            if (SelectedOutInvoice.OutwardDrugClinicDepts != null)
            {
                foreach (OutwardDrugClinicDept p1 in SelectedOutInvoice.OutwardDrugClinicDepts)
                {
                    if (p.GenMedProductID == p1.GenMedProductID && p.InBatchNumber == p1.InBatchNumber && p.InID == p1.InID)
                    {
                        p1.OutQuantity += p.OutQuantity;
                        // p1.IsLoad = 0;
                        p1.RequestQty += p.RequestQty;
                        p1.Qty = p1.OutQuantity;
                        kq = true;
                        break;
                    }
                }
                if (!kq)
                {
                    //p.h= p.RefGenMedProductDetails.InsuranceCover;

                    //KMx: Dời set InwardDrugClinicDept ra ngoài hàm ChooseBatchNumber() để có thể lấy NormalPrice, HIPatientPrice, HIAllowedPrice từ RefGenMedProductDetails (20/05/2015 15:40).

                    //if (p.InwardDrugClinicDept == null)
                    //{
                    //    p.InwardDrugClinicDept = new InwardDrugClinicDept();
                    //    p.InwardDrugClinicDept.InID = p.InID.GetValueOrDefault();
                    //    p.InwardDrugClinicDept.GenMedProductID = p.GenMedProductID;
                    //}
                    //p.InwardDrugClinicDept.NormalPrice = p.OutPrice;
                    //p.InwardDrugClinicDept.HIPatientPrice = p.OutPrice;
                    //p.InwardDrugClinicDept.HIAllowedPrice = p.HIAllowedPrice;

                    p.InvoicePrice = p.OutPrice;
                    p.Qty = p.OutQuantity;
                    SelectedOutInvoice.OutwardDrugClinicDepts.Add(p);
                }
                txt = "";
                SelectedSellVisitor = null;
                if (IsCode.GetValueOrDefault())
                {
                    if (tbx != null)
                    {
                        tbx.Text = "";
                        tbx.Focus();
                    }

                }
                else
                {
                    if (au != null)
                    {
                        au.Text = "";
                        au.Focus();
                    }
                }
            }
        }

        private void ChooseBatchNumber(RefGenMedProductDetails value)
        {
            var items = RefGenMedProductDetailsList.Where(x => x.GenMedProductID == value.GenMedProductID && x.BrandName == value.BrandName).OrderBy(p => p.STT);
            foreach (RefGenMedProductDetails item in items)
            {
                OutwardDrugClinicDept p = new OutwardDrugClinicDept();
                if (item.Remaining > 0)
                {
                    //KMx: Set InwardDrugClinicDept để có thể đổi dòng xuất từ giá bán thành giá vốn và ngược lại (20/05/2015 15:38).
                    p.InwardDrugClinicDept = new InwardDrugClinicDept();
                    p.InwardDrugClinicDept.InID = item.InID;
                    p.InwardDrugClinicDept.GenMedProductID = item.GenMedProductID;
                    p.InwardDrugClinicDept.InCost = item.InCost;
                    p.InwardDrugClinicDept.NormalPrice = item.NormalPrice;
                    p.InwardDrugClinicDept.HIPatientPrice = item.HIPatientPrice;
                    p.InwardDrugClinicDept.HIAllowedPrice = item.HIAllowedPrice;
                    if (item.Remaining - value.RequiredNumber < 0)
                    {
                        if (value.RequestQty > item.Remaining)
                        {
                            p.RequestQty = item.Remaining;
                            value.RequestQty = value.RequestQty - item.Remaining;
                        }
                        else
                        {
                            p.RequestQty = value.RequestQty;
                            value.RequestQty = 0;
                        }
                        value.RequiredNumber = value.RequiredNumber - item.Remaining;
                        p.GenMedProductItem = item;
                        p.GenMedProductID = item.GenMedProductID;
                        p.InBatchNumber = item.InBatchNumber;
                        p.InID = item.InID;
                        //p.OutPrice = item.OutPrice;
                        SetOutPrice(p);
                        p.OutQuantity = item.Remaining;
                        p.InExpiryDate = item.InExpiryDate;
                        p.SdlDescription = item.SdlDescription;
                        p.VAT = item.VAT;
                        CheckBatchNumberExists(p);
                        item.Remaining = 0;
                    }
                    else
                    {
                        p.GenMedProductItem = item;
                        p.GenMedProductID = item.GenMedProductID;
                        p.InBatchNumber = item.InBatchNumber;
                        p.InID = item.InID;
                        p.RequestQty = value.RequestQty;
                        value.RequestQty = 0;
                        //p.OutQuantity = (int)value.RequiredNumber;
                        p.OutQuantity = value.RequiredNumber;
                        //p.OutPrice = item.OutPrice;
                        SetOutPrice(p);
                        p.InExpiryDate = item.InExpiryDate;
                        p.SdlDescription = item.SdlDescription;
                        p.VAT = item.VAT;
                        CheckBatchNumberExists(p);
                        //item.Remaining = item.Remaining - (int)value.RequiredNumber;
                        item.Remaining = item.Remaining - value.RequiredNumber;
                        break;
                    }
                    p.DrugDeptInIDOrig = item.DrugDeptInIDOrig;
                }
            }
            SumTotalPrice();
        }

        private void AddListOutwardDrugMedDept(RefGenMedProductDetails value)
        {
            if (value != null)
            {
                if (ViewCase == 0 || ViewCase == 1)
                {
                    if (value.RequiredNumber > 0 && int.TryParse(value.RequiredNumber.ToString(), out int intOutput))//20201228 QTD: Không cho xuất lẻ nữa
                    {
                        //KMx: Xuất hủy có thể là số lẻ. Cứ cho add số lẻ thoải mái, khi bấm lưu, nếu là xuất trả cho Khoa Dược thì không được trả số lẻ (28/01/2015 17:47).
                        //int intOutput = 0;
                        //if (Int32.TryParse(value.RequiredNumber.ToString(), out intOutput))
                        //{
                        // int a = Convert.ToInt32(value.RequiredNumber);
                        if (CheckValidDrugAuto(value))
                        {
                            ChooseBatchNumber(value);
                        }
                        //}
                        //else
                        //{
                        //    MessageBox.Show("Số lượng phải là số nguyên lớn hơn 0");
                        //}
                    }
                    else
                    {
                        MessageBox.Show("Số lượng phải là số nguyên lớn hơn 0");
                    }
                }
                else
                {
                    //▼======= 20201228 QTD:Thêm điều kiện check số lẻ View nhập cân bằng
                    if (int.TryParse(value.RequiredNumber.ToString(), out int intOutPutNhapCB))
                    {
                    //▲=======
                        var items = RefGenMedProductDetailsList.Where(x => x.GenMedProductID == value.GenMedProductID).OrderBy(p => p.STT);
                        foreach (RefGenMedProductDetails item in items)
                        {
                            OutwardDrugClinicDept p = new OutwardDrugClinicDept();

                            p.GenMedProductItem = item;
                            p.GenMedProductID = item.GenMedProductID;
                            p.InBatchNumber = item.InBatchNumber;
                            p.InID = item.InID;
                            p.RequestQty = value.RequestQty;
                            value.RequestQty = 0;
                            p.OutQuantity = value.RequiredNumber;
                            p.OutPrice = item.OutPrice;
                            p.InvoicePrice = p.OutPrice;
                            p.InExpiryDate = item.InExpiryDate;
                            p.SdlDescription = item.SdlDescription;
                            p.VAT = item.VAT;
                            SelectedOutInvoice.OutwardDrugClinicDepts.Add(p);
                            break;
                        }
                        txt = "";
                        SelectedSellVisitor = null;
                        if (IsCode.GetValueOrDefault())
                        {
                            if (tbx != null)
                            {
                                tbx.Text = "";
                                tbx.Focus();
                            }

                        }
                        else
                        {
                            if (au != null)
                            {
                                au.Text = "";
                                au.Focus();
                            }
                        }
                    }
                    //▼======= 20201228 QTD:Thêm điều kiện check số lẻ View nhập cân bằng
                    else
                    {
                        MessageBox.Show("Số lượng phải là số nguyên!");
                    }
                    //▲=======
                }
            }
            else
            {
                MessageBox.Show(eHCMSResources.K0389_G1_ChonThuocCanBan);
            }
        }

        private void ReCountQtyRequest()
        {
            if (SelectedOutInvoice != null && SelectedSellVisitor != null)
            {
                if (SelectedOutInvoice.OutwardDrugClinicDepts == null)
                {
                    SelectedOutInvoice.OutwardDrugClinicDepts = new ObservableCollection<OutwardDrugClinicDept>();
                }
                var results1 = SelectedOutInvoice.OutwardDrugClinicDepts.Where(x => x.GenMedProductID == SelectedSellVisitor.GenMedProductID);
                if (results1 != null && results1.Count() > 0)
                {
                    foreach (OutwardDrugClinicDept p in results1)
                    {
                        if (p.RequestQty > p.OutQuantity)
                        {
                            p.RequestQty = p.OutQuantity;
                        }
                        SelectedSellVisitor.RequestQty = SelectedSellVisitor.RequestQty - p.RequestQty;
                    }
                }
            }
        }

        public void AddItem(object sender, RoutedEventArgs e)
        {
            ReCountQtyRequest();
            AddListOutwardDrugMedDept(SelectedSellVisitor);
        }

        #region Properties member
        private ObservableCollection<RefGenMedProductDetails> BatchNumberListTemp;
        private ObservableCollection<RefGenMedProductDetails> BatchNumberListShow;
        private ObservableCollection<OutwardDrugClinicDept> OutwardDrugMedDeptListByGenMedProductID;
        private ObservableCollection<OutwardDrugClinicDept> OutwardDrugMedDeptListByGenMedProductIDFirst;

        private OutwardDrugClinicDept _SelectedOutwardDrugClinicDept;
        public OutwardDrugClinicDept SelectedOutwardDrugClinicDept
        {
            get { return _SelectedOutwardDrugClinicDept; }
            set
            {
                if (_SelectedOutwardDrugClinicDept != value)
                    _SelectedOutwardDrugClinicDept = value;
                NotifyOfPropertyChange(() => SelectedOutwardDrugClinicDept);
            }
        }
        #endregion

        private void RefGenMedProductDetailsBatchNumber(long GenMedProductID)
        {
            isLoadingDetail = true;
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyClinicDeptServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginspGetInBatchNumberAllClinicDept_ByGenMedProductID(GenMedProductID, (long)V_MedProductType, SelectedOutInvoice.StoreID.GetValueOrDefault(0), IsHIPatient, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndspGetInBatchNumberAllClinicDept_ByGenMedProductID(asyncResult);
                            BatchNumberListTemp = results.ToObservableCollection();
                            if (BatchNumberListTemp != null && BatchNumberListTemp.Count > 0)
                            {
                                UpdateListToShow();
                            }
                            else
                            {
                                Globals.ShowMessage(eHCMSResources.Z0891_G1_KgConLoNaoKhac, eHCMSResources.G0442_G1_TBao);
                            }

                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            isLoadingDetail = false;
                            // Globals.IsBusy = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }

        public void lnkChooseBatchNumber_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedOutInvoice != null)
            {
                Button lnkBatchNumber = sender as Button;
                long GenMedProductID = (long)lnkBatchNumber.CommandParameter;
                OutwardDrugMedDeptListByGenMedProductID = SelectedOutInvoice.OutwardDrugClinicDepts.Where(x => x.GenMedProductID == GenMedProductID).ToObservableCollection();
                if (OutwardDrugClinicDeptsCopy != null)
                {
                    OutwardDrugMedDeptListByGenMedProductIDFirst = OutwardDrugClinicDeptsCopy.Where(x => x.GenMedProductID == GenMedProductID).ToObservableCollection();
                }
                RefGenMedProductDetailsBatchNumber(GenMedProductID);
            }
        }

        public void UpdateListToShow()
        {
            if (OutwardDrugMedDeptListByGenMedProductIDFirst != null)
            {
                foreach (OutwardDrugClinicDept d in OutwardDrugMedDeptListByGenMedProductIDFirst)
                {
                    var value = BatchNumberListTemp.Where(x => x.GenMedProductID == d.GenMedProductID && x.InBatchNumber == d.InBatchNumber && x.InID == d.InID);
                    if (value.Count() > 0)
                    {
                        foreach (RefGenMedProductDetails s in value.ToList())
                        {
                            s.Remaining = s.Remaining + d.OutQuantityOld;
                            s.RemainingFirst = s.RemainingFirst + d.OutQuantityOld;
                        }
                    }
                    else
                    {
                        RefGenMedProductDetails p = d.GenMedProductItem;
                        p.Remaining = d.OutQuantity;
                        p.RemainingFirst = d.OutQuantity;
                        p.InBatchNumber = d.InBatchNumber;
                        p.OutPrice = d.OutPrice;
                        p.InID = Convert.ToInt64(d.InID);
                        p.STT = d.STT;
                        p.VAT = d.VAT;
                        BatchNumberListTemp.Add(p);
                    }
                }
            }
            foreach (RefGenMedProductDetails s in BatchNumberListTemp)
            {
                if (OutwardDrugMedDeptListByGenMedProductID.Count > 0)
                {
                    foreach (OutwardDrugClinicDept d in OutwardDrugMedDeptListByGenMedProductID)
                    {
                        //20200422 TBL: Trừ luôn số lượng dòng nhập đang được chọn để đổi lô
                        //if (d.GenMedProductID == s.GenMedProductID && d.InBatchNumber == s.InBatchNumber && d.InID == s.InID && d.InID != SelectedOutwardDrugClinicDept.InID)
                        if (d.GenMedProductID == s.GenMedProductID && d.InBatchNumber == s.InBatchNumber && d.InID == s.InID)
                        {
                            s.Remaining = s.Remaining - d.OutQuantity;
                        }
                    }
                }
            }

            BatchNumberListShow = BatchNumberListTemp.Where(x => x.Remaining > 0).ToObservableCollection();

            if (BatchNumberListShow != null && BatchNumberListShow.Count > 0)
            {
                //it bua moi len lam
                Action<IChooseBatchNumberClinicDept> onInitDlg = delegate (IChooseBatchNumberClinicDept proAlloc)
                {
                    proAlloc.SelectedOutwardDrug = SelectedOutwardDrugClinicDept.DeepCopy();
                    if (BatchNumberListShow != null)
                    {
                        proAlloc.BatchNumberListShow = BatchNumberListShow.DeepCopy();
                    }
                    if (OutwardDrugMedDeptListByGenMedProductID != null)
                    {
                        proAlloc.OutwardDrugListByDrugID = OutwardDrugMedDeptListByGenMedProductID.DeepCopy();
                    }
                };
                GlobalsNAV.ShowDialog<IChooseBatchNumberClinicDept>(onInitDlg);
            }
            else
            {
                Globals.ShowMessage(eHCMSResources.Z0891_G1_KgConLoNaoKhac, eHCMSResources.G0442_G1_TBao);
            }
        }

        #endregion

        private void DeleteInvoiceDrugInObject()
        {
            OutwardDrugClinicDept p = SelectedOutwardDrugClinicDept.DeepCopy();

            //KMx: Thêm ngày 24/04/2015 14:56.
            if (SelectedOutInvoice.OutwardDrugClinicDepts_Delete == null)
            {
                SelectedOutInvoice.OutwardDrugClinicDepts_Delete = new ObservableCollection<OutwardDrugClinicDept>();
            }

            if (SelectedOutwardDrugClinicDept.OutID > 0)
            {
                SelectedOutInvoice.OutwardDrugClinicDepts_Delete.Add(SelectedOutwardDrugClinicDept);
            }

            SelectedOutInvoice.OutwardDrugClinicDepts.Remove(SelectedOutwardDrugClinicDept);
            foreach (OutwardDrugClinicDept item in SelectedOutInvoice.OutwardDrugClinicDepts)
            {
                if (item.GenMedProductID == p.GenMedProductID)
                {
                    item.RequestQty = item.RequestQty + p.RequestQty;
                    break;
                }
            }
            SelectedOutInvoice.OutwardDrugClinicDepts = SelectedOutInvoice.OutwardDrugClinicDepts.ToObservableCollection();
            SumTotalPrice();
        }

        public void lnkDelete_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedOutwardDrugClinicDept != null && SelectedOutInvoice.CanSaveAndPaid)
            {
                DeleteInvoiceDrugInObject();
            }
            else
            {
                Globals.ShowMessage(eHCMSResources.A0915_G1_Msg_InfoPhChiXem, eHCMSResources.G0442_G1_TBao);
            }
        }

        public void comboBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cbx = sender as ComboBox;
            if (cbx.SelectedItem != null)
            {
                //--▼-- 28/12/2020 DatTB Gán biến Kho đã chọn để so sánh
                var selectedStore = (RefStorageWarehouseLocation)cbx.SelectedItem;
                V_GroupTypes = selectedStore.V_GroupTypes;
                //--▲-- 28/12/2020 DatTB 
                SetEnable();
                if (SelectedOutInvoice != null && SelectedOutInvoice.OutwardDrugClinicDepts != null)
                {
                    SelectedOutInvoice.OutwardDrugClinicDepts.Clear();
                    ClearData();
                }
                SelectedSellVisitor = new RefGenMedProductDetails(); //20190417 TBL: BM 0006750. Khi doi kho thi du lieu tren autocomplete cung phai clear
            }
        }

        private void OutwardDrugClinicDeptInvoice_SaveByType(OutwardDrugClinicDeptInvoice OutwardInvoice)
        {
            this.ShowBusyIndicator();
            //isLoadingFullOperator = true;
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyClinicDeptServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginOutwardDrugClinicDeptInvoice_SaveByType(OutwardInvoice, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            long OutID = 0;
                            string StrError;
                            bool value = contract.EndOutwardDrugClinicDeptInvoice_SaveByType(out OutID, out StrError, asyncResult);

                            if (string.IsNullOrEmpty(StrError) && value)
                            {
                                Globals.ShowMessage(eHCMSResources.Z1562_G1_DaLuu, eHCMSResources.G0442_G1_TBao);
                                RefeshData();
                                GetOutwardDrugClinicDeptInvoice(OutID);
                            }
                            else
                            {
                                MessageBox.Show(StrError);
                            }
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            //isLoadingFullOperator = false;
                            // Globals.IsBusy = false;
                            this.HideBusyIndicator();
                        }
                    }), null);
                }
            });

            t.Start();
        }

        private void CheckOutwardDrugClinicDeptModified(OutwardDrugClinicDeptInvoice OutwardInvoice)
        {
            if (OutwardInvoice == null || OutwardInvoice.OutwardDrugClinicDepts == null || OutwardInvoice.OutwardDrugClinicDepts.Count <= 0)
            {
                return;
            }

            ObservableCollection<OutwardDrugClinicDept> CheckList = OutwardInvoice.OutwardDrugClinicDepts.Where(x => x.RecordState == RecordState.UNCHANGED).ToObservableCollection();
            foreach (OutwardDrugClinicDept item in CheckList)
            {
                if (item.IsCountHI != item.IsCountHI_Orig || item.IsCountPatient != item.IsCountPatient_Orig || item.OutQuantity != item.OutQuantity_Orig || item.InID != item.InID_Orig || item.OutNotes != item.OutNotes_Orig)
                {
                    item.RecordState = RecordState.MODIFIED;
                }
            }
        }

        private void OutwardDrugClinicDeptInvoice_UpdateByType(OutwardDrugClinicDeptInvoice OutwardInvoice)
        {
            if (OutwardInvoice == null || OutwardInvoice.OutwardDrugClinicDepts == null)
            {
                return;
            }

            CheckOutwardDrugClinicDeptModified(OutwardInvoice);

            this.ShowBusyIndicator();

            OutwardInvoice.OutwardDrugClinicDepts_Add = OutwardInvoice.OutwardDrugClinicDepts.Where(x => x.RecordState == RecordState.DETACHED).ToObservableCollection();

            OutwardInvoice.OutwardDrugClinicDepts_Update = OutwardInvoice.OutwardDrugClinicDepts.Where(x => x.RecordState == RecordState.MODIFIED).ToObservableCollection();

            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyClinicDeptServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginOutwardDrugClinicDeptInvoice_UpdateByType(OutwardInvoice, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            long OutID = 0;
                            string StrError;
                            bool value = contract.EndOutwardDrugClinicDeptInvoice_UpdateByType(out OutID, out StrError, asyncResult);

                            if (string.IsNullOrEmpty(StrError) && value)
                            {
                                Globals.ShowMessage(eHCMSResources.Z1562_G1_DaLuu, eHCMSResources.G0442_G1_TBao);
                                GetOutwardDrugClinicDeptInvoice(OutID);
                            }
                            else
                            {
                                MessageBox.Show(StrError);
                            }
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            //isLoadingFullOperator = false;
                            // Globals.IsBusy = false;
                            this.HideBusyIndicator();
                        }
                    }), null);
                }
            });

            t.Start();
        }

        private bool CheckData()
        {
            if (SelectedOutInvoice == null)
            {
                return false;
            }
            if (SelectedOutInvoice != null)
            {
                SelectedOutInvoice.StaffID = GetStaffLogin().StaffID;
            }

            if (SelectedOutInvoice.TypID == null || SelectedOutInvoice.TypID <= 0)
            {
                MessageBox.Show("Hãy chọn loại xuất!", eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return false;
            }
            if (SelectedOutInvoice.TypID == (long)AllLookupValues.RefOutputType.XUAT_KHOPHONG || SelectedOutInvoice.TypID == (long)AllLookupValues.RefOutputType.XUATNOIBO_LUANCHUYENKHO)
            {
                if (SelectedOutInvoice.OutputToID == SelectedOutInvoice.StoreID)
                {
                    Globals.ShowMessage("Kho xuất và kho nhận không được giống nhau!", eHCMSResources.G0442_G1_TBao);
                    return false;
                }
            }

            if (SelectedOutInvoice.OutwardDrugClinicDepts == null || SelectedOutInvoice.OutwardDrugClinicDepts.Count == 0)
            {
                // TxD05/04/2015: Only existing outwardDrug invoice can be saved without any items to allow for modification to clean up the invoice
                if (SelectedOutInvoice.outiID > 0)
                {
                    if (MessageBox.Show("Không có dữ liệu trong phiếu xuất. Nhấn [OK] đễ tiếp tục Lưu, nhấn [Cancel] đễ nhập thêm dữ liệu", eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                    {
                        return false;
                    }
                }
                else
                {
                    MessageBox.Show("Không có dữ liệu trong phiếu xuất.", eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                    return false;
                }
            }
            else
            {

                for (int i = 0; i < SelectedOutInvoice.OutwardDrugClinicDepts.Count; i++)
                {
                    //if (SelectedOutInvoice.OutwardDrugClinicDepts[i].GenMedProductItem != null && SelectedOutInvoice.OutwardDrugClinicDepts[i].OutQuantity <= 0)
                    //{
                    //    Globals.ShowMessage("Số lượng xuất phải > 0", eHCMSResources.G0442_G1_TBao);
                    //    return false;
                    //}

                    OutwardDrugClinicDept item = SelectedOutInvoice.OutwardDrugClinicDepts[i];

                    //if (SelectedOutInvoice.TypID == (long)AllLookupValues.RefOutputType.XUATNOIBO)
                    if (SelectedOutInvoice.TypID == (long)AllLookupValues.RefOutputType.XUAT_TRA_KHOADUOC)
                    {
                        int intOutput = 0;
                        if (item.OutQuantity <= 0 || !Int32.TryParse(item.OutQuantity.ToString(), NumberStyles.Any, null, out intOutput))
                        {
                            MessageBox.Show(string.Format("{0} ", eHCMSResources.A0522_G1_Msg_Dong) + (i + 1).ToString() + ": Số lượng xuất phải > 0 và phải là số nguyên!", eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                            return false;
                        }
                    }
                    else
                    {
                        if (ViewCase == 0 || ViewCase == 1)
                        {
                            if (item.OutQuantity <= 0)
                            {
                                MessageBox.Show(string.Format("{0} ", eHCMSResources.A0522_G1_Msg_Dong) + (i + 1).ToString() + ": Số lượng xuất phải > 0!", eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                return false;
                            }
                        }
                    }
                }
            }
            return true;
        }

        private void SetDefaultContact()
        {
            if (SelectedOutInvoice == null)
            {
                return;
            }
            SelectedOutInvoice.CustomerName = "";
            SelectedOutInvoice.PhoneNumber = "";
            SelectedOutInvoice.Address = "";
        }

        public void btnUpdate()
        {
            if (MessageBox.Show("Bạn có chắc chỉnh sửa phiếu xuất này không?", "TB", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                btnSave();
            }
        }

        public void btnSave()
        {
            if (CheckData())
            {
                SelectedOutInvoice.MedProductType = V_MedProductType;

                if (SelectedOutInvoice.TypID == (long)AllLookupValues.RefOutputType.HUYHANG || SelectedOutInvoice.TypID == (long)AllLookupValues.RefOutputType.XUAT_DUNGCHUNG)
                {
                    SelectedOutInvoice.V_OutputTo = 0;
                    SelectedOutInvoice.OutputToID = 0;
                    SetDefaultContact();
                }
                else if (SelectedOutInvoice.TypID == (long)AllLookupValues.RefOutputType.XUAT_BN)
                {
                    SelectedOutInvoice.V_OutputTo = (long)AllLookupValues.V_OutputTo.KHACH_VANG_LAI;
                    SelectedOutInvoice.OutputToID = 0;
                }
                else if (SelectedOutInvoice.TypID == (long)AllLookupValues.RefOutputType.XUAT_KHOPHONG || SelectedOutInvoice.TypID == (long)AllLookupValues.RefOutputType.XUATNOIBO_LUANCHUYENKHO)
                {
                    SelectedOutInvoice.V_OutputTo = (long)AllLookupValues.V_OutputTo.KHO_KHAC;
                    SelectedOutInvoice.V_OutDrugInvStatus = (long)AllLookupValues.V_OutDrugInvStatus.SAVE;
                    SetDefaultContact();
                }
                else if (SelectedOutInvoice.TypID == (long)AllLookupValues.RefOutputType.XUAT_TRA_KHOADUOC)
                {
                    SelectedOutInvoice.V_OutputTo = (long)AllLookupValues.V_OutputTo.KHO_KHAC;
                    SelectedOutInvoice.V_OutDrugInvStatus = (long)AllLookupValues.V_OutDrugInvStatus.SAVE;
                    SetDefaultContact();
                    SelectedOutInvoice.OutputToID = (long)cbo.SelectedValue;
                }
                else if (SelectedOutInvoice.TypID == (long)AllLookupValues.RefOutputType.CANBANGKHO)
                {
                    SelectedOutInvoice.V_OutputTo = 0;
                    SelectedOutInvoice.V_OutDrugInvStatus = (long)AllLookupValues.V_OutDrugInvStatus.SAVE;
                    SelectedOutInvoice.OutputToID = 0;
                }

                //KMx: Khi chuyển thành xml đi lưu, dùng TotalInvoicePrice chứ không dùng OutAmount (người trước viết bên xuất cho BN như vậy, nên ở đây làm theo) (29/04/2015 09:05).
                foreach (OutwardDrugClinicDept item in SelectedOutInvoice.OutwardDrugClinicDepts)
                {
                    item.TotalInvoicePrice = item.OutAmount.GetValueOrDefault();
                }

                //OutwardDrugClinicDeptInvoice_SaveByType(SelectedOutInvoice);
                //KMx: Cập nhật phiếu theo cách mới (24/04/2015 14:46).
                if (!IsBalanceView)
                {
                    if (SelectedOutInvoice.outiID > 0)
                    {
                        OutwardDrugClinicDeptInvoice_UpdateByType(SelectedOutInvoice);
                    }
                    else
                    {
                        OutwardDrugClinicDeptInvoice_SaveByType(SelectedOutInvoice);
                    }
                }
                else
                {
                    OutwardDrugClinicDeptInvoice_SaveByType_Balance(SelectedOutInvoice);
                }
            }
        }

        KeyEnabledComboBox cbo = null;
        public void comboBox1_Loaded(object sender, RoutedEventArgs e)
        {
            //if (SelectedOutInvoice.CanSave == true) //--30/01/2021 DatTB CanSave false tương đương đang load phiếu cũ => không clear chi tiết thuốc
            //{ 
            //    //--▼--29/01/2021 DatTB Clear data khi chọn lại kho nhận, vì xuất trả lọc theo kho nhận
            //    if (SelectedOutInvoice != null && SelectedOutInvoice.OutwardDrugClinicDepts != null)
            //    {
            //        SelectedOutInvoice.OutwardDrugClinicDepts.Clear();
            //    }
            //    ClearData();
            //    TotalPrice = 0;
            //    SelectedSellVisitor = null;
            //    //--▲--29/01/2021 DatTB
            //}
            cbo = sender as KeyEnabledComboBox;
        }

        public void btnNew()
        {
            RefeshData();
        }

        private void DeepCopyOutwardDrugMedDept()
        {
            if (SelectedOutInvoice.OutwardDrugClinicDepts != null)
            {
                OutwardDrugClinicDeptsCopy = SelectedOutInvoice.OutwardDrugClinicDepts.DeepCopy();
            }
            else
            {
                OutwardDrugClinicDeptsCopy = null;
            }
            if (SelectedOutInvoice != null)
            {
                SelectedOutInvoiceCoppy = SelectedOutInvoice.DeepCopy();
            }
        }

        public void Handle(ItemSelected<Patient> message)
        {
            if (IsActive == false || message == null || message.Item == null)
            {
                return;
            }
            Patient selPatient = message.Item;
            SelectedOutInvoice.CustomerName = selPatient.FullName;
            SelectedOutInvoice.Address = selPatient.PatientStreetAddress + ", " + selPatient.SuburbName.CitiesProvince.CityProvinceName;
            SelectedOutInvoice.PhoneNumber = selPatient.PatientPhoneNumber;
        }

        //dung lai from tim kiem cua Demage
        #region IHandle<DrugDeptCloseSearchOutClinicDeptInvoiceEvent> Members

        public void Handle(DrugDeptCloseSearchOutClinicDeptInvoiceEvent message)
        {
            if (message != null && this.IsActive)
            {
                OutwardDrugClinicDeptInvoice temp = message.SelectedOutClinicDeptInvoice as OutwardDrugClinicDeptInvoice;
                if (temp != null)
                {
                    var OutputToID = temp.OutputToID; //--07/01/2021 DatTB Dùng cách đặt biến OutputToID, vì sau khi gán cả biến temp và biến truyền vào (message) đều bị thay đổi giá trị. Chưa rõ nguyên nhân
                    SelectedOutInvoice = temp;
                    SelectedOutInvoice.OutputToID = OutputToID;//--07/01/2021 DatTB Dùng cách đặt biến OutputToID, vì sau khi gán cả biến temp và biến truyền vào (message) đều bị thay đổi giá trị. Chưa rõ nguyên nhân
                    OutwardDrugClinicDeptDetails_Load(SelectedOutInvoice.outiID);
                }
            }
        }

        #endregion

        #region IHandle<ClinicDeptChooseBatchNumberEvent> Members

        public void Handle(ClinicDeptChooseBatchNumberEvent message)
        {
            if (message != null && this.IsActive)
            {
                SelectedOutwardDrugClinicDept.GenMedProductItem = message.BatchNumberSelected;
                SelectedOutwardDrugClinicDept.InBatchNumber = message.BatchNumberSelected.InBatchNumber;
                SelectedOutwardDrugClinicDept.InExpiryDate = message.BatchNumberSelected.InExpiryDate;
                SelectedOutwardDrugClinicDept.InID = message.BatchNumberSelected.InID;
                if (IsCost.GetValueOrDefault())
                {
                    SelectedOutwardDrugClinicDept.OutPrice = message.BatchNumberSelected.InCost;
                }
                else
                {
                    SelectedOutwardDrugClinicDept.OutPrice = message.BatchNumberSelected.NormalPrice;
                }
                SelectedOutwardDrugClinicDept.SdlDescription = message.BatchNumberSelected.SdlDescription;
                SelectedOutwardDrugClinicDept.VAT = message.BatchNumberSelected.VAT;
                SumTotalPrice();
            }
        }

        #endregion

        #region IHandle<ClinicDeptChooseBatchNumberResetQtyEvent> Members

        public void Handle(ClinicDeptChooseBatchNumberResetQtyEvent message)
        {
            if (message != null && this.IsActive)
            {
                SelectedOutwardDrugClinicDept.GenMedProductItem = message.BatchNumberSelected;
                SelectedOutwardDrugClinicDept.InBatchNumber = message.BatchNumberSelected.InBatchNumber;
                SelectedOutwardDrugClinicDept.InExpiryDate = message.BatchNumberSelected.InExpiryDate;
                SelectedOutwardDrugClinicDept.InID = message.BatchNumberSelected.InID;
                if (IsCost.GetValueOrDefault())
                {
                    SelectedOutwardDrugClinicDept.OutPrice = message.BatchNumberSelected.InCost;
                }
                else
                {
                    SelectedOutwardDrugClinicDept.OutPrice = message.BatchNumberSelected.NormalPrice;
                }
                SelectedOutwardDrugClinicDept.SdlDescription = message.BatchNumberSelected.SdlDescription;
                SelectedOutwardDrugClinicDept.OutQuantity = message.BatchNumberSelected.Remaining;
                SelectedOutwardDrugClinicDept.VAT = message.BatchNumberSelected.VAT;
                SumTotalPrice();
            }
        }

        #endregion

        #region View By Code Member

        private bool? IsCode = false;
        string txt = "";
        public void AxTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            txt = (sender as TextBox).Text;
            if (!string.IsNullOrEmpty(txt))
            {
                string Code = Globals.FormatCode((long)V_MedProductType, txt);
                if (ViewCase == 0 || ViewCase == 1)
                {
                    SearchRefGenMedProductDetails(Code, true);
                }
                else
                {
                    SearchRefGenMedProductDetails_Balance_FromCategory(Code, true);
                }
            }
        }

        public void DrugCode_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                // au.IsEnabled = false;
                string text = (sender as TextBox).Text;
                if (!string.IsNullOrEmpty(text))
                {
                    SearchRefGenMedProductDetails((sender as TextBox).Text, true);
                }
            }
        }

        TextBox tbx = null;
        public void AxTextBox_Loaded(object sender, RoutedEventArgs e)
        {
            tbx = sender as TextBox;
        }

        private bool _VisibilityName = true;
        public bool VisibilityName
        {
            get
            {
                if (SelectedOutInvoice != null)
                {
                    return _VisibilityName && SelectedOutInvoice.CanSaveAndPaid;
                }
                return _VisibilityName;
            }
            set
            {
                if (SelectedOutInvoice != null)
                {
                    _VisibilityName = value && SelectedOutInvoice.CanSaveAndPaid;
                    _VisibilityCode = !_VisibilityName && SelectedOutInvoice.CanSaveAndPaid;
                }
                else
                {
                    _VisibilityName = value;
                    _VisibilityCode = !_VisibilityName;
                }
                NotifyOfPropertyChange(() => VisibilityName);
                NotifyOfPropertyChange(() => VisibilityCode);

            }
        }

        private bool _VisibilityCode = false;
        public bool VisibilityCode
        {
            get
            {
                return _VisibilityCode;
            }
            set
            {
                if (_VisibilityCode != value)
                {
                    _VisibilityCode = value;
                    NotifyOfPropertyChange(() => VisibilityCode);
                }
            }
        }

        public void Code_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            IsCode = true;
            VisibilityName = false;
        }

        public void Name_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            IsCode = false;
            VisibilityName = true;
        }
        AxTextBox AxQty = null;
        public void Quantity_Loaded(object sender, RoutedEventArgs e)
        {
            AxQty = sender as AxTextBox;
        }

        #endregion

        #region Control enable Storage Combobox  and Customer Info

        private bool _IsEnableStorageCbx;
        public bool IsEnableStorageCbx
        {
            get
            {
                return _IsEnableStorageCbx;
            }
            set
            {
                if (_IsEnableStorageCbx != value)
                {
                    _IsEnableStorageCbx = value;
                    NotifyOfPropertyChange(() => IsEnableStorageCbx);
                }
            }
        }

        private bool _IsEnableCustomerInfo;
        public bool IsEnableCustomerInfo
        {
            get
            {
                return _IsEnableCustomerInfo;
            }
            set
            {
                if (_IsEnableCustomerInfo != value)
                {
                    _IsEnableCustomerInfo = value;
                    NotifyOfPropertyChange(() => IsEnableCustomerInfo);
                }
            }
        }

        private void SetEnable()
        {
            if (SelectedOutInvoice == null)
            {
                return;
            }

            long TypID = SelectedOutInvoice.TypID.GetValueOrDefault();

            if (TypID == (long)AllLookupValues.RefOutputType.HUYHANG || SelectedOutInvoice.TypID == (long)AllLookupValues.RefOutputType.XUAT_DUNGCHUNG)
            {
                IsEnableStorageCbx = false;
                IsEnableCustomerInfo = false;
                SetDefaultContact();
            }
            else if (TypID == (long)AllLookupValues.RefOutputType.XUAT_BN)
            {
                IsEnableStorageCbx = false;
                IsEnableCustomerInfo = true;
            }
            //--▼-- 25/12/2020 DatTB
            //else if (TypID == (long)AllLookupValues.RefOutputType.XUAT_KHOPHONG || SelectedOutInvoice.TypID == (long)AllLookupValues.RefOutputType.XUATNOIBO_LUANCHUYENKHO)
            //{
            //    IsEnableStorageCbx = true;
            //    IsEnableCustomerInfo = false;
            //    SetDefaultContact();
            //}
            //--▲-- 25/12/2020 DatTB
            else if (TypID == (long)AllLookupValues.RefOutputType.XUAT_TRA_KHOADUOC)
            {
                IsEnableStorageCbx = true;
                IsEnableCustomerInfo = false;
                //--▼--04/02/2021 DatTB Lọc không cho kho lẻ trả về kho Chẵn
                //StoreCbxReturn = ObjectCopier.DeepCopy(Globals.IsMainStorageWarehouseLocation.Where(x => ((long)V_MedProductType != 0 && x.ListV_MedProductType != null && x.ListV_MedProductType.Contains(((long)V_MedProductType).ToString()) && (V_GroupTypes == 0 || x.V_GroupTypes == V_GroupTypes))).ToObservableCollection());
                if (V_MedProductType == AllLookupValues.MedProductType.THUOC || V_MedProductType == AllLookupValues.MedProductType.Y_CU || V_MedProductType == AllLookupValues.MedProductType.NUTRITION)
                {
                    StoreCbxReturn = ObjectCopier.DeepCopy(Globals.IsMainStorageWarehouseLocation.Where(x => ((long)V_MedProductType != 0 && x.ListV_MedProductType != null && x.ListV_MedProductType.Contains(((long)V_MedProductType).ToString()) && (V_GroupTypes == 0 || x.V_GroupTypes == V_GroupTypes) && x.IsSubStorage)).ToObservableCollection());
                }
                else
                {
                    StoreCbxReturn = ObjectCopier.DeepCopy(Globals.IsMainStorageWarehouseLocation.Where(x => ((long)V_MedProductType != 0 && x.ListV_MedProductType != null && x.ListV_MedProductType.Contains(((long)V_MedProductType).ToString()) && (V_GroupTypes == 0 || x.V_GroupTypes == V_GroupTypes))).ToObservableCollection());
                }
                //--▲--04/02/2021 DatTB
                SetDefaultForStoreReturn();
                SetDefaultContact();
            }
            if (TypID != (long)AllLookupValues.RefOutputType.XUAT_TRA_KHOADUOC)
            {
                GetReceiptStore();
            }

            SetEnableCheckboxXCD();
        }

        private bool IsInCostType()
        {
            if (SelectedOutInvoice == null)
            {
                return false;
            }

            if (SelectedOutInvoice.TypID == (long)AllLookupValues.RefOutputType.HUYHANG || SelectedOutInvoice.TypID == (long)AllLookupValues.RefOutputType.XUATNOIBO_LUANCHUYENKHO
                || SelectedOutInvoice.TypID == (long)AllLookupValues.RefOutputType.XUAT_KHOPHONG || SelectedOutInvoice.TypID == (long)AllLookupValues.RefOutputType.XUAT_TRA_KHOADUOC
                || SelectedOutInvoice.TypID == (long)AllLookupValues.RefOutputType.XUAT_DUNGCHUNG)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void ChangeOutPrice()
        {
            if (SelectedOutInvoice == null || SelectedOutInvoice.OutwardDrugClinicDepts == null || SelectedOutInvoice.OutwardDrugClinicDepts.Count <= 0)
            {
                return;
            }

            if (IsInCostType())
            {
                foreach (OutwardDrugClinicDept outward in SelectedOutInvoice.OutwardDrugClinicDepts)
                {
                    outward.OutPrice = outward.InwardDrugClinicDept.InCost;
                }
            }
            else
            {
                foreach (OutwardDrugClinicDept outward in SelectedOutInvoice.OutwardDrugClinicDepts)
                {
                    outward.OutPrice = outward.InwardDrugClinicDept.NormalPrice;
                }
            }

            SumTotalPrice();
        }

        private void SetOutPrice(OutwardDrugClinicDept outward)
        {
            if (SelectedOutInvoice == null || outward == null || outward.InwardDrugClinicDept == null)
            {
                return;
            }

            if (IsInCostType())
            {
                outward.OutPrice = outward.InwardDrugClinicDept.InCost;
            }
            else
            {
                outward.OutPrice = outward.InwardDrugClinicDept.NormalPrice;
            }
        }

        public void CbxTypID_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SetEnable();
            ChangeOutPrice();
            //if (SelectedOutInvoice.CanSave == true) //--30/01/2021 DatTB CanSave false tương đương đang load phiếu cũ => không clear chi tiết thuốc
            //{
            //    //--▼--29/01/2021 DatTB Clear data khi chọn lại kho nhận, vì xuất trả lọc theo kho nhận
            //    if (SelectedOutInvoice != null && SelectedOutInvoice.OutwardDrugClinicDepts != null)
            //    {
            //        SelectedOutInvoice.OutwardDrugClinicDepts.Clear();
            //    }
            //    ClearData();
            //    TotalPrice = 0;
            //    SelectedSellVisitor = null;
            //    //--▲--29/01/2021 DatTB
            //}
        }
        #endregion

        private void SearchRefGenMedProductDetails_VTYTTH(string Name, bool? IsCode)
        {
            if (SelectedOutInvoice == null)
            {
                SelectedOutInvoice = new OutwardDrugClinicDeptInvoice();
            }
            if (SelectedOutInvoice.StoreID == null || SelectedOutInvoice.StoreID.GetValueOrDefault(0) <= 0)
            {
                MessageBox.Show(eHCMSResources.K0333_G1_ChonKhoXuat);
                return;
            }
            if (IsCode.GetValueOrDefault())
            {
                isLoadingDetail = true;
            }

            //--▼--28/01/2021 DatTB
            long OutputID = 0;

            if (SelectedOutInvoice.TypID == (long)AllLookupValues.RefOutputType.XUAT_TRA_KHOADUOC)
            {
                OutputID = (long)cbo.SelectedValue;
            }
            //--▲--28/01/2021 DatTB

            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyClinicDeptServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetVTYTTHForSellVisitorAutoComplete_ForRequestClinicDept(IsCost, Name, SelectedOutInvoice.StoreID.GetValueOrDefault(0), (long)V_MedProductType, null, null, IsCode, null, null, OutputID /*--28/01/2021 DatTB Thêm biến*/, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndGetVTYTTHForSellVisitorAutoComplete_ForRequestClinicDept(asyncResult);

                            RefGenMedProductDetailsList = new ObservableCollection<RefGenMedProductDetails>();
                            RefGenMedProductDetailsTemp = new ObservableCollection<RefGenMedProductDetails>();

                            RefGenMedProductDetailsTemp = results.ToObservableCollection();

                            if (OutwardDrugClinicDeptsCopy != null && OutwardDrugClinicDeptsCopy.Count > 0)
                            {
                                foreach (OutwardDrugClinicDept d in OutwardDrugClinicDeptsCopy)
                                {
                                    var value = results.Where(x => x.GenMedProductID == d.GenMedProductID && x.InBatchNumber == d.InBatchNumber && x.InID == d.InID);
                                    if (value.Count() > 0)
                                    {
                                        foreach (RefGenMedProductDetails s in value.ToList())
                                        {
                                            s.Remaining = s.Remaining + d.OutQuantityOld;
                                            s.RemainingFirst = s.RemainingFirst + d.OutQuantityOld;
                                        }
                                    }
                                }
                            }
                            foreach (RefGenMedProductDetails s in RefGenMedProductDetailsTemp)
                            {
                                if (SelectedOutInvoice.OutwardDrugClinicDepts != null && SelectedOutInvoice.OutwardDrugClinicDepts.Count > 0)
                                {
                                    foreach (OutwardDrugClinicDept d in SelectedOutInvoice.OutwardDrugClinicDepts)
                                    {
                                        if (d.GenMedProductID == s.GenMedProductID && d.InBatchNumber == s.InBatchNumber && d.InID == s.InID)
                                        {
                                            s.Remaining = s.Remaining - d.OutQuantity;
                                        }
                                    }
                                }
                                RefGenMedProductDetailsList.Add(s);
                            }
                            ListDisplayAutoComplete();
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

        private void SearchRefGenMedProductDetails_Blood(string Name, bool? IsCode)
        {
            if (SelectedOutInvoice == null)
            {
                SelectedOutInvoice = new OutwardDrugClinicDeptInvoice();
            }
            if (SelectedOutInvoice.StoreID == null || SelectedOutInvoice.StoreID.GetValueOrDefault(0) <= 0)
            {
                MessageBox.Show(eHCMSResources.K0333_G1_ChonKhoXuat);
                return;
            }
            if (IsCode.GetValueOrDefault())
            {
                isLoadingDetail = true;
            }

            //--▼--28/01/2021 DatTB
            long OutputID = 0;

            if (SelectedOutInvoice.TypID == (long)AllLookupValues.RefOutputType.XUAT_TRA_KHOADUOC)
            {
                OutputID = (long)cbo.SelectedValue;
            }
            //--▲--28/01/2021 DatTB

            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyClinicDeptServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetBloodForSellVisitorAutoComplete_ForRequestClinicDept(IsCost, Name, SelectedOutInvoice.StoreID.GetValueOrDefault(0), (long)V_MedProductType, null, null, IsCode, null, null, OutputID /*--28/01/2021 DatTB Thêm biến*/, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndGetBloodForSellVisitorAutoComplete_ForRequestClinicDept(asyncResult);

                            RefGenMedProductDetailsList = new ObservableCollection<RefGenMedProductDetails>();
                            RefGenMedProductDetailsTemp = new ObservableCollection<RefGenMedProductDetails>();

                            RefGenMedProductDetailsTemp = results.ToObservableCollection();

                            if (OutwardDrugClinicDeptsCopy != null && OutwardDrugClinicDeptsCopy.Count > 0)
                            {
                                foreach (OutwardDrugClinicDept d in OutwardDrugClinicDeptsCopy)
                                {
                                    var value = results.Where(x => x.GenMedProductID == d.GenMedProductID && x.InBatchNumber == d.InBatchNumber && x.InID == d.InID);
                                    if (value.Count() > 0)
                                    {
                                        foreach (RefGenMedProductDetails s in value.ToList())
                                        {
                                            s.Remaining = s.Remaining + d.OutQuantityOld;
                                            s.RemainingFirst = s.RemainingFirst + d.OutQuantityOld;
                                        }
                                    }
                                }
                            }
                            foreach (RefGenMedProductDetails s in RefGenMedProductDetailsTemp)
                            {
                                if (SelectedOutInvoice.OutwardDrugClinicDepts != null && SelectedOutInvoice.OutwardDrugClinicDepts.Count > 0)
                                {
                                    foreach (OutwardDrugClinicDept d in SelectedOutInvoice.OutwardDrugClinicDepts)
                                    {
                                        if (d.GenMedProductID == s.GenMedProductID && d.InBatchNumber == s.InBatchNumber && d.InID == s.InID)
                                        {
                                            s.Remaining = s.Remaining - d.OutQuantity;
                                        }
                                    }
                                }
                                RefGenMedProductDetailsList.Add(s);
                            }
                            ListDisplayAutoComplete();
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

        private void SearchRefGenMedProductDetails_ThanhTrung(string Name, bool? IsCode)
        {
            if (SelectedOutInvoice == null)
            {
                SelectedOutInvoice = new OutwardDrugClinicDeptInvoice();
            }
            if (SelectedOutInvoice.StoreID == null || SelectedOutInvoice.StoreID.GetValueOrDefault(0) <= 0)
            {
                MessageBox.Show(eHCMSResources.K0333_G1_ChonKhoXuat);
                return;
            }
            if (IsCode.GetValueOrDefault())
            {
                isLoadingDetail = true;
            }

            //--▼--28/01/2021 DatTB
            long OutputID = 0;

            if (SelectedOutInvoice.TypID == (long)AllLookupValues.RefOutputType.XUAT_TRA_KHOADUOC)
            {
                OutputID = (long)cbo.SelectedValue;
            }
            //--▲--28/01/2021 DatTB

            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyClinicDeptServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetThanhTrungForSellVisitorAutoComplete_ForRequestClinicDept(IsCost, Name, SelectedOutInvoice.StoreID.GetValueOrDefault(0), (long)V_MedProductType, null, null, IsCode, null, null, OutputID /*--28/01/2021 DatTB Thêm biến*/, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndGetThanhTrungForSellVisitorAutoComplete_ForRequestClinicDept(asyncResult);

                            RefGenMedProductDetailsList = new ObservableCollection<RefGenMedProductDetails>();
                            RefGenMedProductDetailsTemp = new ObservableCollection<RefGenMedProductDetails>();

                            RefGenMedProductDetailsTemp = results.ToObservableCollection();

                            if (OutwardDrugClinicDeptsCopy != null && OutwardDrugClinicDeptsCopy.Count > 0)
                            {
                                foreach (OutwardDrugClinicDept d in OutwardDrugClinicDeptsCopy)
                                {
                                    var value = results.Where(x => x.GenMedProductID == d.GenMedProductID && x.InBatchNumber == d.InBatchNumber && x.InID == d.InID);
                                    if (value.Count() > 0)
                                    {
                                        foreach (RefGenMedProductDetails s in value.ToList())
                                        {
                                            s.Remaining = s.Remaining + d.OutQuantityOld;
                                            s.RemainingFirst = s.RemainingFirst + d.OutQuantityOld;
                                        }
                                    }
                                }
                            }
                            foreach (RefGenMedProductDetails s in RefGenMedProductDetailsTemp)
                            {
                                if (SelectedOutInvoice.OutwardDrugClinicDepts != null && SelectedOutInvoice.OutwardDrugClinicDepts.Count > 0)
                                {
                                    foreach (OutwardDrugClinicDept d in SelectedOutInvoice.OutwardDrugClinicDepts)
                                    {
                                        if (d.GenMedProductID == s.GenMedProductID && d.InBatchNumber == s.InBatchNumber && d.InID == s.InID)
                                        {
                                            s.Remaining = s.Remaining - d.OutQuantity;
                                        }
                                    }
                                }
                                RefGenMedProductDetailsList.Add(s);
                            }
                            ListDisplayAutoComplete();
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

        private void SearchRefGenMedProductDetails_Chemical(string Name, bool? IsCode)
        {
            if (SelectedOutInvoice == null)
            {
                SelectedOutInvoice = new OutwardDrugClinicDeptInvoice();
            }
            if (SelectedOutInvoice.StoreID == null || SelectedOutInvoice.StoreID.GetValueOrDefault(0) <= 0)
            {
                MessageBox.Show(eHCMSResources.K0333_G1_ChonKhoXuat);
                return;
            }
            if (IsCode.GetValueOrDefault())
            {
                isLoadingDetail = true;
            }

            //--▼--28/01/2021 DatTB
            long OutputID = 0;

            if (SelectedOutInvoice.TypID == (long)AllLookupValues.RefOutputType.XUAT_TRA_KHOADUOC)
            {
                OutputID = (long)cbo.SelectedValue;
            }
            //--▲--28/01/2021 DatTB

            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyClinicDeptServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetChemicalForSellVisitorAutoComplete_ForRequestClinicDept(IsCost, Name, SelectedOutInvoice.StoreID.GetValueOrDefault(0), (long)V_MedProductType, null, null, IsCode, null, null, OutputID /*--28/01/2021 DatTB Thêm biến*/, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndGetChemicalForSellVisitorAutoComplete_ForRequestClinicDept(asyncResult);

                            RefGenMedProductDetailsList = new ObservableCollection<RefGenMedProductDetails>();
                            RefGenMedProductDetailsTemp = new ObservableCollection<RefGenMedProductDetails>();

                            RefGenMedProductDetailsTemp = results.ToObservableCollection();

                            if (OutwardDrugClinicDeptsCopy != null && OutwardDrugClinicDeptsCopy.Count > 0)
                            {
                                foreach (OutwardDrugClinicDept d in OutwardDrugClinicDeptsCopy)
                                {
                                    var value = results.Where(x => x.GenMedProductID == d.GenMedProductID && x.InBatchNumber == d.InBatchNumber && x.InID == d.InID);
                                    if (value.Count() > 0)
                                    {
                                        foreach (RefGenMedProductDetails s in value.ToList())
                                        {
                                            s.Remaining = s.Remaining + d.OutQuantityOld;
                                            s.RemainingFirst = s.RemainingFirst + d.OutQuantityOld;
                                        }
                                    }
                                }
                            }
                            foreach (RefGenMedProductDetails s in RefGenMedProductDetailsTemp)
                            {
                                if (SelectedOutInvoice.OutwardDrugClinicDepts != null && SelectedOutInvoice.OutwardDrugClinicDepts.Count > 0)
                                {
                                    foreach (OutwardDrugClinicDept d in SelectedOutInvoice.OutwardDrugClinicDepts)
                                    {
                                        if (d.GenMedProductID == s.GenMedProductID && d.InBatchNumber == s.InBatchNumber && d.InID == s.InID)
                                        {
                                            s.Remaining = s.Remaining - d.OutQuantity;
                                        }
                                    }
                                }
                                RefGenMedProductDetailsList.Add(s);
                            }
                            ListDisplayAutoComplete();
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
        private void SearchRefGenMedProductDetails_VPP(string Name, bool? IsCode)
        {
            if (SelectedOutInvoice == null)
            {
                SelectedOutInvoice = new OutwardDrugClinicDeptInvoice();
            }
            if (SelectedOutInvoice.StoreID == null || SelectedOutInvoice.StoreID.GetValueOrDefault(0) <= 0)
            {
                MessageBox.Show(eHCMSResources.K0333_G1_ChonKhoXuat);
                return;
            }
            if (IsCode.GetValueOrDefault())
            {
                isLoadingDetail = true;
            }

            //--▼--28/01/2021 DatTB
            long OutputID = 0;

            if (SelectedOutInvoice.TypID == (long)AllLookupValues.RefOutputType.XUAT_TRA_KHOADUOC)
            {
                OutputID = (long)cbo.SelectedValue;
            }
            //--▲--28/01/2021 DatTB

            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyClinicDeptServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetVPPForSellVisitorAutoComplete_ForRequestClinicDept(IsCost, Name, SelectedOutInvoice.StoreID.GetValueOrDefault(0), (long)V_MedProductType, null, null, IsCode, null, null, OutputID /*--28/01/2021 DatTB Thêm biến*/, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndGetVPPForSellVisitorAutoComplete_ForRequestClinicDept(asyncResult);

                            RefGenMedProductDetailsList = new ObservableCollection<RefGenMedProductDetails>();
                            RefGenMedProductDetailsTemp = new ObservableCollection<RefGenMedProductDetails>();

                            RefGenMedProductDetailsTemp = results.ToObservableCollection();

                            if (OutwardDrugClinicDeptsCopy != null && OutwardDrugClinicDeptsCopy.Count > 0)
                            {
                                foreach (OutwardDrugClinicDept d in OutwardDrugClinicDeptsCopy)
                                {
                                    var value = results.Where(x => x.GenMedProductID == d.GenMedProductID && x.InBatchNumber == d.InBatchNumber && x.InID == d.InID);
                                    if (value.Count() > 0)
                                    {
                                        foreach (RefGenMedProductDetails s in value.ToList())
                                        {
                                            s.Remaining = s.Remaining + d.OutQuantityOld;
                                            s.RemainingFirst = s.RemainingFirst + d.OutQuantityOld;
                                        }
                                    }
                                }
                            }
                            foreach (RefGenMedProductDetails s in RefGenMedProductDetailsTemp)
                            {
                                if (SelectedOutInvoice.OutwardDrugClinicDepts != null && SelectedOutInvoice.OutwardDrugClinicDepts.Count > 0)
                                {
                                    foreach (OutwardDrugClinicDept d in SelectedOutInvoice.OutwardDrugClinicDepts)
                                    {
                                        if (d.GenMedProductID == s.GenMedProductID && d.InBatchNumber == s.InBatchNumber && d.InID == s.InID)
                                        {
                                            s.Remaining = s.Remaining - d.OutQuantity;
                                        }
                                    }
                                }
                                RefGenMedProductDetailsList.Add(s);
                            }
                            ListDisplayAutoComplete();
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

        private void SearchRefGenMedProductDetails_TiemNgua(string Name, bool? IsCode)
        {
            if (SelectedOutInvoice == null)
            {
                SelectedOutInvoice = new OutwardDrugClinicDeptInvoice();
            }
            if (SelectedOutInvoice.StoreID == null || SelectedOutInvoice.StoreID.GetValueOrDefault(0) <= 0)
            {
                MessageBox.Show(eHCMSResources.K0333_G1_ChonKhoXuat);
                return;
            }
            if (IsCode.GetValueOrDefault())
            {
                isLoadingDetail = true;
            }

            //--▼--28/01/2021 DatTB
            long OutputID = 0;

            if (SelectedOutInvoice.TypID == (long)AllLookupValues.RefOutputType.XUAT_TRA_KHOADUOC)
            {
                OutputID = (long)cbo.SelectedValue;
            }
            //--▲--28/01/2021 DatTB

            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyClinicDeptServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetTiemNguaForSellVisitorAutoComplete_ForRequestClinicDept(IsCost, Name, SelectedOutInvoice.StoreID.GetValueOrDefault(0), (long)V_MedProductType, null, null, IsCode, null, null, OutputID /*--28/01/2021 DatTB Thêm biến*/, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndGetTiemNguaForSellVisitorAutoComplete_ForRequestClinicDept(asyncResult);

                            RefGenMedProductDetailsList = new ObservableCollection<RefGenMedProductDetails>();
                            RefGenMedProductDetailsTemp = new ObservableCollection<RefGenMedProductDetails>();

                            RefGenMedProductDetailsTemp = results.ToObservableCollection();

                            if (OutwardDrugClinicDeptsCopy != null && OutwardDrugClinicDeptsCopy.Count > 0)
                            {
                                foreach (OutwardDrugClinicDept d in OutwardDrugClinicDeptsCopy)
                                {
                                    var value = results.Where(x => x.GenMedProductID == d.GenMedProductID && x.InBatchNumber == d.InBatchNumber && x.InID == d.InID);
                                    if (value.Count() > 0)
                                    {
                                        foreach (RefGenMedProductDetails s in value.ToList())
                                        {
                                            s.Remaining = s.Remaining + d.OutQuantityOld;
                                            s.RemainingFirst = s.RemainingFirst + d.OutQuantityOld;
                                        }
                                    }
                                }
                            }
                            foreach (RefGenMedProductDetails s in RefGenMedProductDetailsTemp)
                            {
                                if (SelectedOutInvoice.OutwardDrugClinicDepts != null && SelectedOutInvoice.OutwardDrugClinicDepts.Count > 0)
                                {
                                    foreach (OutwardDrugClinicDept d in SelectedOutInvoice.OutwardDrugClinicDepts)
                                    {
                                        if (d.GenMedProductID == s.GenMedProductID && d.InBatchNumber == s.InBatchNumber && d.InID == s.InID)
                                        {
                                            s.Remaining = s.Remaining - d.OutQuantity;
                                        }
                                    }
                                }
                                RefGenMedProductDetailsList.Add(s);
                            }
                            ListDisplayAutoComplete();
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

        private void SearchRefGenMedProductDetails_VTTH(string Name, bool? IsCode)
        {
            if (SelectedOutInvoice == null)
            {
                SelectedOutInvoice = new OutwardDrugClinicDeptInvoice();
            }
            if (SelectedOutInvoice.StoreID == null || SelectedOutInvoice.StoreID.GetValueOrDefault(0) <= 0)
            {
                MessageBox.Show(eHCMSResources.K0333_G1_ChonKhoXuat);
                return;
            }
            if (IsCode.GetValueOrDefault())
            {
                isLoadingDetail = true;
            }

            //--▼--28/01/2021 DatTB
            long OutputID = 0;

            if (SelectedOutInvoice.TypID == (long)AllLookupValues.RefOutputType.XUAT_TRA_KHOADUOC)
            {
                OutputID = (long)cbo.SelectedValue;
            }
            //--▲--28/01/2021 DatTB

            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyClinicDeptServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetVTTHForSellVisitorAutoComplete_ForRequestClinicDept(IsCost, Name, SelectedOutInvoice.StoreID.GetValueOrDefault(0), (long)V_MedProductType, null, null, IsCode, null, null, OutputID /*--28/01/2021 DatTB Thêm biến*/, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndGetVTTHForSellVisitorAutoComplete_ForRequestClinicDept(asyncResult);

                            RefGenMedProductDetailsList = new ObservableCollection<RefGenMedProductDetails>();
                            RefGenMedProductDetailsTemp = new ObservableCollection<RefGenMedProductDetails>();

                            RefGenMedProductDetailsTemp = results.ToObservableCollection();

                            if (OutwardDrugClinicDeptsCopy != null && OutwardDrugClinicDeptsCopy.Count > 0)
                            {
                                foreach (OutwardDrugClinicDept d in OutwardDrugClinicDeptsCopy)
                                {
                                    var value = results.Where(x => x.GenMedProductID == d.GenMedProductID && x.InBatchNumber == d.InBatchNumber && x.InID == d.InID);
                                    if (value.Count() > 0)
                                    {
                                        foreach (RefGenMedProductDetails s in value.ToList())
                                        {
                                            s.Remaining = s.Remaining + d.OutQuantityOld;
                                            s.RemainingFirst = s.RemainingFirst + d.OutQuantityOld;
                                        }
                                    }
                                }
                            }
                            foreach (RefGenMedProductDetails s in RefGenMedProductDetailsTemp)
                            {
                                if (SelectedOutInvoice.OutwardDrugClinicDepts != null && SelectedOutInvoice.OutwardDrugClinicDepts.Count > 0)
                                {
                                    foreach (OutwardDrugClinicDept d in SelectedOutInvoice.OutwardDrugClinicDepts)
                                    {
                                        if (d.GenMedProductID == s.GenMedProductID && d.InBatchNumber == s.InBatchNumber && d.InID == s.InID)
                                        {
                                            s.Remaining = s.Remaining - d.OutQuantity;
                                        }
                                    }
                                }
                                RefGenMedProductDetailsList.Add(s);
                            }
                            ListDisplayAutoComplete();
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

        private int _ViewCase = 0;
        public int ViewCase
        {
            get
            {
                return _ViewCase;
            }
            set
            {
                _ViewCase = value;
                if (_ViewCase >= 1)
                {
                    _IsBalanceView = true;
                    GetRefOutputTypeLst();
                }
                NotifyOfPropertyChange(() => ViewCase);
                NotifyOfPropertyChange(() => IsBalanceView);
            }
        }
        private bool _IsBalanceView = false;
        public bool IsBalanceView
        {
            get { return _IsBalanceView; }
            set
            {
                if (_IsBalanceView != value)
                {
                    _IsBalanceView = value;
                    NotifyOfPropertyChange(() => IsBalanceView);
                }
            }
        }

        private void OutwardDrugClinicDeptInvoice_SaveByType_Balance(OutwardDrugClinicDeptInvoice OutwardInvoice)
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyClinicDeptServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginOutwardDrugClinicDeptInvoice_SaveByType_Balance(OutwardInvoice, ViewCase, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            long OutID = 0;
                            string StrError;
                            bool value = contract.EndOutwardDrugClinicDeptInvoice_SaveByType_Balance(out OutID, out StrError, asyncResult);

                            if (string.IsNullOrEmpty(StrError) && value)
                            {
                                Globals.ShowMessage(eHCMSResources.Z1562_G1_DaLuu, eHCMSResources.G0442_G1_TBao);
                                RefeshData();
                                GetOutwardDrugClinicDeptInvoice(OutID);
                            }
                            else
                            {
                                MessageBox.Show(StrError);
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
            });

            t.Start();
        }

        private void SearchRefGenMedProductDetails_Balance_FromCategory(string Name, bool? IsCode)
        {
            if (SelectedOutInvoice == null)
            {
                SelectedOutInvoice = new OutwardDrugClinicDeptInvoice();
            }

            if (IsCode.GetValueOrDefault())
            {
                isLoadingDetail = true;
            }

            //--▼--30/12/2020 DatTB
            long? RequestID = null;
            long? RefGenDrugCatID_1 = null;

            RequestID = SelectedOutInvoice.ReqDrugInClinicDeptID;

            if ((long)V_MedProductType == (long)AllLookupValues.MedProductType.THUOC)
            {
                RefGenDrugCatID_1 = SelectedOutInvoice.RefGenDrugCatID_1;
            }
            //--▲--30/12/2020 DatTB

            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyClinicDeptServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetDrugClinicDeptForBalance_FromCategory(IsCost, Name, SelectedOutInvoice.StoreID.GetValueOrDefault(0), (long)V_MedProductType, RefGenDrugCatID_1, null, IsCode, null, null, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndGetDrugClinicDeptForBalance_FromCategory(asyncResult);

                            RefGenMedProductDetailsList = new ObservableCollection<RefGenMedProductDetails>();
                            RefGenMedProductDetailsTemp = new ObservableCollection<RefGenMedProductDetails>();

                            RefGenMedProductDetailsTemp = results.ToObservableCollection();

                            if (OutwardDrugClinicDeptsCopy != null && OutwardDrugClinicDeptsCopy.Count > 0)
                            {
                                foreach (OutwardDrugClinicDept d in OutwardDrugClinicDeptsCopy)
                                {
                                    var value = results.Where(x => x.GenMedProductID == d.GenMedProductID && x.InBatchNumber == d.InBatchNumber && x.InID == d.InID);
                                    if (value.Count() > 0)
                                    {
                                        foreach (RefGenMedProductDetails s in value.ToList())
                                        {
                                            s.Remaining = s.Remaining + d.OutQuantityOld;
                                            s.RemainingFirst = s.RemainingFirst + d.OutQuantityOld;
                                        }
                                    }
                                }
                            }
                            foreach (RefGenMedProductDetails s in RefGenMedProductDetailsTemp)
                            {
                                if (SelectedOutInvoice.OutwardDrugClinicDepts != null && SelectedOutInvoice.OutwardDrugClinicDepts.Count > 0)
                                {
                                    foreach (OutwardDrugClinicDept d in SelectedOutInvoice.OutwardDrugClinicDepts)
                                    {
                                        if (d.GenMedProductID == s.GenMedProductID && d.InBatchNumber == s.InBatchNumber && d.InID == s.InID)
                                        {
                                            s.Remaining = s.Remaining - d.OutQuantity;
                                        }
                                    }
                                }
                                RefGenMedProductDetailsList.Add(s);
                            }
                            ListDisplayAutoComplete();
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

        private bool _IsEnableTextBoxNote = true;
        public bool IsEnableTextBoxNote
        {
            get
            {
                return _IsEnableTextBoxNote;
            }
            set
            {
                if (_IsEnableTextBoxNote != value)
                {
                    _IsEnableTextBoxNote = value;
                    NotifyOfPropertyChange(() => IsEnableTextBoxNote);
                }
            }
        }

        private bool _IsXuatChongDich = false;
        public bool IsXuatChongDich
        {
            get
            {
                return _IsXuatChongDich;
            }
            set
            {
                if (_IsXuatChongDich != value)
                {
                    _IsXuatChongDich = value;
                    if(_IsXuatChongDich == true)
                    {
                        IsEnableTextBoxNote = false;
                        SelectedOutInvoice.Notes = "[XCD]";
                    }
                    else
                    {
                        IsEnableTextBoxNote = true;
                        SelectedOutInvoice.Notes = "";
                    }
                    NotifyOfPropertyChange(() => IsXuatChongDich);
                }
            }
        }

        public bool IsEnableUpdateButton
        {
            get
            {
                return mXuatTraHang_PhieuMoi && SelectedOutInvoice.TypID != 3;
            }
        }

        private bool _IsEnableCheckboxXCD = false;
        public bool IsEnableCheckboxXCD
        {
            get
            {
                return _IsEnableCheckboxXCD;
            }
            set
            {
                if (_IsEnableCheckboxXCD != value)
                {
                    _IsEnableCheckboxXCD = value;
                    NotifyOfPropertyChange(() => IsEnableCheckboxXCD);
                }
            }
        }

        public void SetEnableCheckboxXCD()
        {
            if(SelectedOutInvoice == null)
            {
                return;
            }
            if (SelectedOutInvoice.TypID == (long)AllLookupValues.RefOutputType.XUAT_DUNGCHUNG && Globals.ServerConfigSection.CommonItems.EnableCheckboxXCD)
            {
                IsEnableCheckboxXCD = true;
            }
            else
            {
                IsEnableCheckboxXCD = false;
                IsXuatChongDich = false;
            }
        }
    }
}
