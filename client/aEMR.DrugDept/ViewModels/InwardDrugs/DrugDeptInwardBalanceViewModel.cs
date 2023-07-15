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
using aEMR.CommonTasks;
using aEMR.Controls;
/*
* 20190522 #001 TNHX:   [BM0010766] Thêm trường ngày xuất của phiếu xuất để nhập vào kho + refactor code
* 20211103 #002 QTD:    Lọc kho theo cấu hình trách nhiệm
*/
namespace aEMR.DrugDept.ViewModels
{
    [Export(typeof(IDrugDeptInwardBalance)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class DrugDeptInwardBalanceViewModel : Conductor<object>, IDrugDeptInwardBalance
        , IHandle<ClinicDrugDeptCloseSearchInwardIncoiceEvent>
    {
        private long TypID = (long)AllLookupValues.RefOutputType.CANBANGKHO;
        [ImportingConstructor]
        public DrugDeptInwardBalanceViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            Globals.EventAggregator.Subscribe(this);

            SearchCriteria = new InwardInvoiceSearchCriteria();
            InitInvoice();
            StoreCbx = new ObservableCollection<RefStorageWarehouseLocation>();
            LoadRefOutputType();
            Coroutine.BeginExecute(DoGetStore());
        }

        private IEnumerator<IResult> DoGetSubStore()
        {
            var paymentTypeTask = new LoadStoreListTask((long)AllLookupValues.StoreType.STORAGE_DRUGDEPT, false, null, true, false);
            yield return paymentTypeTask;
            if ((V_MedProductType == (long)AllLookupValues.MedProductType.THUOC || V_MedProductType == (long)AllLookupValues.MedProductType.Y_CU || V_MedProductType == (long)AllLookupValues.MedProductType.NUTRITION) 
                && Globals.ServerConfigSection.MedDeptElements.IsEnableMedSubStorage)
            {
                foreach (var storedetails in paymentTypeTask.LookupList)
                {
                    if (storedetails.IsSubStorage || IsImportFromSubStorage)
                    {
                        StoreCbx.Add(storedetails);
                    }
                }
            }
            else
            {
                foreach (var storedetails in paymentTypeTask.LookupList)
                {
                    if (!storedetails.IsSubStorage || IsImportFromSubStorage)
                    {
                        StoreCbx.Add(storedetails);
                    }
                }
            }
            yield break;
        }

        private IEnumerator<IResult> DoGetStore()
        {
            var paymentTypeTask = new LoadStoreListTask((long)AllLookupValues.StoreType.STORAGE_DRUGDEPT, false, null, false, false);
            yield return paymentTypeTask;
            //StoreCbx = paymentTypeTask.LookupList.Where(x => (V_MedProductType != 0 && x.StoreTypeID == (long)AllLookupValues.StoreType.STORAGE_DRUGDEPT && x.ListV_MedProductType != null && x.ListV_MedProductType.Contains(V_MedProductType.ToString()))).ToObservableCollection();
            var StoreTemp = paymentTypeTask.LookupList.Where(x => (V_MedProductType != 0 && x.StoreTypeID == (long)AllLookupValues.StoreType.STORAGE_DRUGDEPT && x.ListV_MedProductType != null && x.ListV_MedProductType.Contains(V_MedProductType.ToString()))).ToObservableCollection();
            StoreCbx = Globals.CheckDrugMedStoreWareHouse(StoreTemp);
            if(StoreCbx == null || StoreCbx.Count == 0)
            {
                MessageBox.Show(eHCMSResources.A0110_G1_Msg_InfoChuaCauHinhTNKho.Replace("\\n ", "\n"));
            }
            yield break;
        }

        private void InitInvoice()
        {
            CurrentInwardDrugClinicDeptInvoice = new InwardDrugClinicDeptInvoice
            {
                InvDateInvoice = DateTime.Now,
                DSPTModifiedDate = DateTime.Now,
                CurrencyID = 129,
                TypID = (long)AllLookupValues.RefOutputType.CANBANGKHO
            };
            if (CurrentInwardDrugClinicDeptInvoice.SelectedStaff == null)
            {
                CurrentInwardDrugClinicDeptInvoice.SelectedStaff = new Staff();
            }
            CurrentInwardDrugClinicDeptInvoice.SelectedStaff.FullName = GetStaffLogin().FullName;
            CurrentInwardDrugClinicDeptInvoice.StaffID = GetStaffLogin().StaffID;
            MedDeptInvoiceList = new ObservableCollection<OutwardDrugMedDeptInvoice>();
        }

        //KMx: Để Unsubcribe thì khi chuyển từ View Nhập Thuốc, sang Nhập Y Cụ thì double click vào tìm phiếu xuất, bắn event về đây không được (27/12/2014 16:57).
        //protected override void OnDeactivate(bool close)
        //{
        //    Globals.EventAggregator.Unsubscribe(this);
        //}

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

        private InwardDrugClinicDeptInvoice _CurrentInwardDrugClinicDeptInvoice;
        public InwardDrugClinicDeptInvoice CurrentInwardDrugClinicDeptInvoice
        {
            get
            {
                return _CurrentInwardDrugClinicDeptInvoice;
            }
            set
            {
                if (_CurrentInwardDrugClinicDeptInvoice != value)
                {
                    _CurrentInwardDrugClinicDeptInvoice = value;

                    NotifyOfPropertyChange(() => CurrentInwardDrugClinicDeptInvoice);
                }
            }
        }

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

        private bool _IsImportFromSubStorage;
        public bool IsImportFromSubStorage
        {
            get => _IsImportFromSubStorage; set
            {
                _IsImportFromSubStorage = value;
                NotifyOfPropertyChange(() => IsImportFromSubStorage);
            }
        }
        #endregion

        private Staff GetStaffLogin()
        {
            return Globals.LoggedUserAccount.Staff;
        }

        private void LoadMedDeptInvoiceList(long? StoreID)
        {
            this.ShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
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

        public void cbxKho_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CurrentInwardDrugClinicDeptInvoice != null && CurrentInwardDrugClinicDeptInvoice.CanSave)
            {
                if (CurrentInwardDrugClinicDeptInvoice.SelectedStorage != null)
                {
                    CurrentInwardDrugClinicDeptInvoice.CurrentOutwardDrugMedDeptInvoice = null;
                }
                else
                {
                    if (MedDeptInvoiceList != null)
                    {
                        MedDeptInvoiceList.Clear();
                    }
                }
            }
        }

        public void cbxOutInvID_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CurrentInwardDrugClinicDeptInvoice != null && CurrentInwardDrugClinicDeptInvoice.CanSave)
            {
                if (CurrentInwardDrugClinicDeptInvoice.CurrentOutwardDrugMedDeptInvoice != null)
                {
                    if (CurrentInwardDrugClinicDeptInvoice.CurrentOutwardDrugMedDeptInvoice.FromClinicDept)
                    {
                        CurrentInwardDrugClinicDeptInvoice.outiID_Clinic = CurrentInwardDrugClinicDeptInvoice.CurrentOutwardDrugMedDeptInvoice.outiID;
                    }
                    else
                    {
                        CurrentInwardDrugClinicDeptInvoice.outiID = CurrentInwardDrugClinicDeptInvoice.CurrentOutwardDrugMedDeptInvoice.outiID;
                    }
                    CurrentInwardDrugClinicDeptInvoice.OutDate = (DateTime)CurrentInwardDrugClinicDeptInvoice.CurrentOutwardDrugMedDeptInvoice.OutDate;
                    OutwardDrugMedDeptDetails_Load(CurrentInwardDrugClinicDeptInvoice.CurrentOutwardDrugMedDeptInvoice.outiID, CurrentInwardDrugClinicDeptInvoice.CurrentOutwardDrugMedDeptInvoice.FromClinicDept);
                }
                else
                {
                    if (CurrentInwardDrugClinicDeptInvoice.InwardDrugs != null)
                    {
                        CurrentInwardDrugClinicDeptInvoice.InwardDrugs.Clear();
                    }
                }
            }
        }

        private void OutwardDrugMedDeptDetails_Load(long outiID, bool FromClinicDept)
        {
            this.ShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
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
                                    if (CurrentInwardDrugClinicDeptInvoice.InwardDrugs == null)
                                    {
                                        CurrentInwardDrugClinicDeptInvoice.InwardDrugs = new ObservableCollection<InwardDrugClinicDept>();
                                    }
                                    CurrentInwardDrugClinicDeptInvoice.InwardDrugs.Clear();
                                    foreach (OutwardDrugMedDept p in results)
                                    {
                                        InwardDrugClinicDept item = new InwardDrugClinicDept
                                        {
                                            RefGenMedProductDetails = p.RefGenericDrugDetail
                                        };
                                        if (p.InwardDrugMedDept != null)
                                        {
                                            item.InProductionDate = p.InwardDrugMedDept.InProductionDate;
                                            item.InExpiryDate = p.InwardDrugMedDept.InExpiryDate;
                                            item.InBatchNumber = p.InwardDrugMedDept.InBatchNumber;
                                            item.V_GoodsType = p.InwardDrugMedDept.V_GoodsType;
                                            item.NormalPrice = p.InwardDrugMedDept.NormalPrice;
                                            item.HIPatientPrice = p.InwardDrugMedDept.HIPatientPrice;
                                            item.HIAllowedPrice = p.InwardDrugMedDept.HIAllowedPrice;
                                            item.GenMedVersionID = p.InwardDrugMedDept.GenMedVersionID;
                                            item.VAT = p.InwardDrugMedDept.VAT;
                                        }
                                        item.InQuantity = p.OutQuantity;
                                        item.MedDeptQty = p.OutQuantity;
                                        item.InBuyingPrice = p.OutPrice;
                                        item.InBuyingPriceActual = p.OutPrice;
                                        item.SdlDescription = "";
                                        item.OutID = p.OutID;
                                        item.DrugDeptInIDOrig = p.DrugDeptInIDOrig;
                                        CurrentInwardDrugClinicDeptInvoice.InwardDrugs.Add(item);
                                    }
                                    CountTotalPrice();
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

        private void GetInwardInvoiceDrugByID(long ID,long V_MedProductType)//--02/01/2021 DatTB Thêm biến V_MedProductType
        {
            //cbxKho_SelectionChanged(null, null);
            //return;
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyClinicDeptServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetInwardDrugMedDeptInvoice_ByID(ID, V_MedProductType, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                CurrentInwardDrugClinicDeptInvoice = contract.EndGetInwardDrugMedDeptInvoice_ByID(asyncResult);
                                if (CurrentInwardDrugClinicDeptInvoice != null)
                                {
                                    InwardDrugDetails_ByID(CurrentInwardDrugClinicDeptInvoice.inviID);
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

        private void InwardDrugInternalMedDeptInvoice_SaveXML() // InwardDrugClinicDeptInvoice_SaveXML() 02/01/2021 DatTB đổi func mới
        {
            DateTime? DSPTModifiedDate = null;
            if (CurrentInwardDrugClinicDeptInvoice.CurrentOutwardDrugMedDeptInvoice != null)
            {
                DSPTModifiedDate = CurrentInwardDrugClinicDeptInvoice.CurrentOutwardDrugMedDeptInvoice.DSPTModifiedDate;
            }
            this.ShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyClinicDeptServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginInwardDrugInternalMedDeptInvoice_SaveXML(CurrentInwardDrugClinicDeptInvoice, DSPTModifiedDate, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                int results = contract.EndInwardDrugInternalMedDeptInvoice_SaveXML(out long inviID, asyncResult);
                                if (results > 0)
                                {
                                    Globals.ShowMessage(eHCMSResources.T0074_G1_I, eHCMSResources.T0074_G1_I);
                                }
                                else
                                {
                                    if (inviID > 0)
                                    {
                                        Globals.ShowMessage(eHCMSResources.A0468_G1_Msg_InfoLuuOK, eHCMSResources.G0442_G1_TBao);
                                        GetInwardInvoiceDrugByID(inviID, V_MedProductType);//--02/01/2021 DatTB Thêm biến V_MedProductType
                                    }
                                    else
                                    {
                                        Globals.ShowMessage(eHCMSResources.K2782_G1_DaCNhat, eHCMSResources.G0442_G1_TBao);
                                        if (CurrentInwardDrugClinicDeptInvoice != null)
                                        {
                                            GetInwardInvoiceDrugByID(CurrentInwardDrugClinicDeptInvoice.inviID, V_MedProductType);//--02/01/2021 DatTB Thêm biến V_MedProductType
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

        public void btnSave()
        {
            //26062018 TTM: Do dataGrid van chua sua khong co isvalid nen comment dieu kien if va else de ham save tro thanh ham vo dieu kien. 
            //Sau nay se sua lai
            //if (GridSuppliers.IsValid)
            //{
            //    if (CurrentInwardDrugClinicDeptInvoice.SelectedStorage == null)
            //    {
            //        Globals.ShowMessage(eHCMSResources.Z0566_G1_ChuaChonKhoNhap, eHCMSResources.G0442_G1_TBao);
            //        return;
            //    }
            //    if (CurrentInwardDrugClinicDeptInvoice.SelectedStorage.StoreID <= 0)
            //    {
            //        Globals.ShowMessage(eHCMSResources.Z0566_G1_ChuaChonKhoNhap, eHCMSResources.G0442_G1_TBao);
            //        return;
            //    }

            //    if (CurrentInwardDrugClinicDeptInvoice.InwardDrugs != null && CurrentInwardDrugClinicDeptInvoice.InwardDrugs.Count > 0)
            //    {
            //        CurrentInwardDrugClinicDeptInvoice.V_MedProductType = V_MedProductType;
            //        if (CurrentInwardDrugClinicDeptInvoice.SelectedStorageOut != null)
            //        {
            //            CurrentInwardDrugClinicDeptInvoice.StoreIDOut = CurrentInwardDrugClinicDeptInvoice.SelectedStorageOut.StoreID;
            //        }
            //        if (CurrentInwardDrugClinicDeptInvoice.SelectedStorage != null)
            //        {
            //            CurrentInwardDrugClinicDeptInvoice.StoreID = CurrentInwardDrugClinicDeptInvoice.SelectedStorage.StoreID;
            //        }
            //        InwardDrugClinicDeptInvoice_SaveXML();
            //    }
            //    else
            //    {
            //        if (V_MedProductType == (long)AllLookupValues.MedProductType.THUOC)
            //        {
            //            Globals.ShowMessage(eHCMSResources.Z1093_G1_Chua_NhapThuoc, eHCMSResources.G0442_G1_TBao);
            //        }
            //        else if (V_MedProductType == (long)AllLookupValues.MedProductType.Y_CU)
            //        {
            //            Globals.ShowMessage(eHCMSResources.Z1094_G1_ChuaNhapYCu, eHCMSResources.G0442_G1_TBao);
            //        }
            //        else
            //        {
            //            Globals.ShowMessage(eHCMSResources.Z1095_G1_ChuaNhapHChat, eHCMSResources.G0442_G1_TBao);
            //        }
            //    }
            //}
            //else
            //{
            //    Globals.ShowMessage(eHCMSResources.Z0567_G1_TTinKgHopLe, eHCMSResources.G0442_G1_TBao);
            //}

            if (CurrentInwardDrugClinicDeptInvoice.SelectedStorage == null)
            {
                Globals.ShowMessage(eHCMSResources.Z0566_G1_ChuaChonKhoNhap, eHCMSResources.G0442_G1_TBao);
                return;
            }
            if (CurrentInwardDrugClinicDeptInvoice.SelectedStorage.StoreID <= 0)
            {
                Globals.ShowMessage(eHCMSResources.Z0566_G1_ChuaChonKhoNhap, eHCMSResources.G0442_G1_TBao);
                return;
            }

            if (CurrentInwardDrugClinicDeptInvoice.InwardDrugs != null && CurrentInwardDrugClinicDeptInvoice.InwardDrugs.Count > 0)
            {
                CurrentInwardDrugClinicDeptInvoice.V_MedProductType = V_MedProductType;
                if (CurrentInwardDrugClinicDeptInvoice.SelectedStorageOut != null)
                {
                    CurrentInwardDrugClinicDeptInvoice.StoreIDOut = CurrentInwardDrugClinicDeptInvoice.SelectedStorageOut.StoreID;
                }
                if (CurrentInwardDrugClinicDeptInvoice.SelectedStorage != null)
                {
                    CurrentInwardDrugClinicDeptInvoice.StoreID = CurrentInwardDrugClinicDeptInvoice.SelectedStorage.StoreID;
                }
                InwardDrugInternalMedDeptInvoice_SaveXML(); // InwardDrugClinicDeptInvoice_SaveXML() 02/01/2021 DatTB đổi func mới
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
                else if (V_MedProductType == (long)AllLookupValues.MedProductType.NUTRITION)
                {
                    Globals.ShowMessage("Chưa nhập Dinh dưỡng", eHCMSResources.G0442_G1_TBao);
                }
                else
                {
                    Globals.ShowMessage(eHCMSResources.Z1095_G1_ChuaNhapHChat, eHCMSResources.G0442_G1_TBao);
                }
            }
        }

        public void btnAcceptAutoUpdate()
        {
            this.ShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);

            if (CurrentInwardDrugClinicDeptInvoice == null || CurrentInwardDrugClinicDeptInvoice.inviID <= 0)
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
                        contract.BeginAcceptAutoUpdateInwardClinicInvoice(CurrentInwardDrugClinicDeptInvoice.inviID, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                bool result = contract.EndAcceptAutoUpdateInwardClinicInvoice(asyncResult);
                                if (result)
                                {
                                    MessageBox.Show(eHCMSResources.A0286_G1_Msg_InfoChNhanChoK_DcCNhatOK, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                    GetInwardInvoiceDrugByID(CurrentInwardDrugClinicDeptInvoice.inviID, V_MedProductType);//--02/01/2021 DatTB Thêm biến V_MedProductType
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
            SearchInwardInvoiceDrug(0, 20);
        }

        public void OpenPopUpSearchInwardInvoice(IList<InwardDrugClinicDeptInvoice> results, int Totalcount)
        {
            //mo popup tim
            //var proAlloc = Globals.GetViewModel<IStoreDeptClinicInwardDrugSupplierSearch>();
            //proAlloc.SearchCriteria = SearchCriteria.DeepCopy();
            //proAlloc.TypID = TypID;
            //proAlloc.V_MedProductType = V_MedProductType;
            //proAlloc.InwardInvoiceList.Clear();
            //proAlloc.InwardInvoiceList.TotalItemCount = Totalcount;
            //proAlloc.InwardInvoiceList.PageIndex = 0;
            //if (results != null && results.Count > 0)
            //{
            //    foreach (InwardDrugClinicDeptInvoice p in results)
            //    {
            //        proAlloc.InwardInvoiceList.Add(p);
            //    }
            //}
            //var instance = proAlloc as Conductor<object>;
            //Globals.ShowDialog(instance, (o) => { });

            void onInitDlg(IStoreDeptClinicInwardDrugSupplierSearch proAlloc)
            {
                proAlloc.SearchCriteria = SearchCriteria.DeepCopy();
                proAlloc.TypID = TypID;
                proAlloc.V_MedProductType = V_MedProductType;
                proAlloc.InwardInvoiceList.Clear();
                proAlloc.InwardInvoiceList.TotalItemCount = Totalcount;
                proAlloc.InwardInvoiceList.PageIndex = 0;
                proAlloc.InwardInvoiceList.PageSize = 20;
                if (results != null && results.Count > 0)
                {
                    foreach (InwardDrugClinicDeptInvoice p in results)
                    {
                        proAlloc.InwardInvoiceList.Add(p);
                    }
                }
            }

            GlobalsNAV.ShowDialog<IStoreDeptClinicInwardDrugSupplierSearch>(onInitDlg);
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

            this.ShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
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

            SearchCriteria.IsMedDeptSubStorage = true;

            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyClinicDeptServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginSearchInwardDrugClinicDeptInvoice(SearchCriteria, TypID, V_MedProductType, PageIndex, PageSize, true, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndSearchInwardDrugClinicDeptInvoice(out int Totalcount, asyncResult);
                                if (results != null && results.Count > 0)
                                {
                                    if (results.Count > 1)
                                    {
                                        //mo pop up tim
                                        OpenPopUpSearchInwardInvoice(results.ToList(), Totalcount);
                                    }
                                    else
                                    {
                                        CurrentInwardDrugClinicDeptInvoice = results.FirstOrDefault();
                                        InwardDrugDetails_ByID(CurrentInwardDrugClinicDeptInvoice.inviID);
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
            if (CurrentInwardDrugClinicDeptInvoice != null && CurrentInwardDrugClinicDeptInvoice.InwardDrugs != null)
            {
                CurrentInwardDrugClinicDeptInvoice.TotalPrice = 0;
                for (int i = 0; i < CurrentInwardDrugClinicDeptInvoice.InwardDrugs.Count; i++)
                {
                    CurrentInwardDrugClinicDeptInvoice.TotalPrice += CurrentInwardDrugClinicDeptInvoice.InwardDrugs[i].InBuyingPrice * (decimal)CurrentInwardDrugClinicDeptInvoice.InwardDrugs[i].InQuantity;
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
                        contract.BeginGetInwardDrugClinicDept_ByIDInvoiceNotPaging_V2(ID, V_MedProductType, true, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndGetInwardDrugClinicDept_ByIDInvoiceNotPaging_V2(out decimal TongTienSPChuaVAT
                                        , out decimal CKTrenSP
                                        , out decimal TongTienTrenSPDaTruCK
                                        , out decimal TongCKTrenHoaDon
                                        , out decimal TongTienHoaDonCoVAT, asyncResult);

                                //load danh sach thuoc theo hoa don 
                                CurrentInwardDrugClinicDeptInvoice.InwardDrugs = results.ToObservableCollection();
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
            if (message != null && IsActive)
            {
                CurrentInwardDrugClinicDeptInvoice = message.SelectedInwardInvoice as InwardDrugClinicDeptInvoice;
                InwardDrugDetails_ByID(CurrentInwardDrugClinicDeptInvoice.inviID);
            }
        }
        #endregion

        #region printing member
        public void btnPreview()
        {
            void onInitDlg(IDrugDeptReportDocumentPreview proAlloc)
            {
                proAlloc.ID = CurrentInwardDrugClinicDeptInvoice.inviID;
                proAlloc.V_MedProductType = V_MedProductType;
                if (V_MedProductType == (long)AllLookupValues.MedProductType.THUOC)
                {
                    proAlloc.LyDo = eHCMSResources.Z0569_G1_PhNhapKhoThuoc;
                }
                else if (V_MedProductType == (long)AllLookupValues.MedProductType.Y_CU)
                {
                    proAlloc.LyDo = eHCMSResources.Z0570_G1_PhNhapKhoYCu;
                }
                else if (V_MedProductType == (long)AllLookupValues.MedProductType.NUTRITION)
                {
                    proAlloc.LyDo = eHCMSResources.Z3208_G1_PhNhapKhoDDuong;
                }
                else if (V_MedProductType == (long)AllLookupValues.MedProductType.HOA_CHAT)
                {
                    proAlloc.LyDo = eHCMSResources.Z0571_G1_PhNhapKhoHChat;
                }
                else if (V_MedProductType == (long)AllLookupValues.MedProductType.TIEM_NGUA)
                {
                    proAlloc.LyDo = eHCMSResources.Z2486_G1_PhieuNhapKhoTiemNgua;
                }
                else if (V_MedProductType == (long)AllLookupValues.MedProductType.VATTU_TIEUHAO)
                {
                    proAlloc.LyDo = eHCMSResources.Z2551_G1_PhNhapVTTH;
                }
                else if (V_MedProductType == (long)AllLookupValues.MedProductType.VTYT_TIEUHAO)
                {
                    proAlloc.LyDo = eHCMSResources.Z2477_G1_PhNhapVTYTTH;
                }
                else if (V_MedProductType == (long)AllLookupValues.MedProductType.VAN_PHONG_PHAM)
                {
                    proAlloc.LyDo = eHCMSResources.Z2515_G1_PhNhapVPP;
                }
                else if (V_MedProductType == (long)AllLookupValues.MedProductType.MAU)
                {
                    proAlloc.LyDo = eHCMSResources.Z2504_G1_PhieuNhapKhoMau;
                }
                else
                {
                    proAlloc.LyDo = eHCMSResources.Z2503_G1_PhieuNhapKhoThanhTrung;
                }

                proAlloc.eItem = ReportName.DRUGDEPT_INWARD_MEDDEPTFROMCLINICDEPT;
            }
            GlobalsNAV.ShowDialog<IDrugDeptReportDocumentPreview>(onInitDlg);
        }
        #endregion

        public void authorization()
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


        #region AutoComplete for balance inward 
        AxGrid RootAxGrid;
        TextBox tbx = null;
        AxTextBox AxQty = null;
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


        private bool? IsCode = false;
        private bool _VisibilityName = true;
        string txt = "";
        private string BrandName;
        private bool? IsCost = true;
        private bool _isSearchByGenericName = false;
        public bool IsSearchByGenericName
        {
            get { return _isSearchByGenericName; }
            set
            {
                if (_isSearchByGenericName != value)
                {
                    _isSearchByGenericName = value;
                    NotifyOfPropertyChange(() => IsSearchByGenericName);
                }
            }
        }
        private bool _visSearchByGenericName = false;
        public bool vIsSearchByGenericName
        {
            get { return _visSearchByGenericName; }
            set
            {
                if (_visSearchByGenericName != value)
                {
                    _visSearchByGenericName = value;
                    NotifyOfPropertyChange(() => vIsSearchByGenericName);
                }
            }
        }
        public bool VisibilityName
        {
            get
            {
                if (CurrentInwardDrugClinicDeptInvoice != null)
                {
                    return _VisibilityName && CurrentInwardDrugClinicDeptInvoice.CanSave;
                }
                return _VisibilityName;
            }
            set
            {
                if (CurrentInwardDrugClinicDeptInvoice != null)
                {
                    _VisibilityName = value && CurrentInwardDrugClinicDeptInvoice.CanSave;
                    _VisibilityCode = !_VisibilityName && CurrentInwardDrugClinicDeptInvoice.CanSave;
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

        public void AxGrid_Loaded(object sender, RoutedEventArgs e)
        {
            RootAxGrid = sender as AxGrid;
        }
        public void AxTextBox_Loaded(object sender, RoutedEventArgs e)
        {
            tbx = sender as TextBox;
        }
        public void AxTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            txt = (sender as TextBox).Text;
            if (!string.IsNullOrEmpty(txt))
            {
                string Code = Globals.FormatCode(V_MedProductType, txt);
                GetDrugForBalanceCompleteFromCategory(Code, true);
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
                GetDrugForBalanceCompleteFromCategory(e.Parameter, false);
            }
        }

        public void AutoDrug_Text_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            if (au.SelectedItem != null)
            {
                SelectedSellVisitor = au.SelectedItem as RefGenMedProductDetails;
            }
        }
        
        public void Quantity_Loaded(object sender, RoutedEventArgs e)
        {
            AxQty = sender as AxTextBox;
        }
        public void AddItem_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (RootAxGrid != null)
                {
                    RootAxGrid.DisableFirstNextFocus = true;
                }
            }
        }
        public void AddItem_Click(object sender, object e)
        {
            AddListOutwardDrugMedDept(SelectedSellVisitor);
        }
        private void AddListOutwardDrugMedDept(RefGenMedProductDetails value)
        {
            if (CurrentInwardDrugClinicDeptInvoice.InwardDrugs == null)
            {
                CurrentInwardDrugClinicDeptInvoice.InwardDrugs = new ObservableCollection<InwardDrugClinicDept>();
            }
            if (value != null)
            {
                if (value.RequiredNumber > 0 && int.TryParse(value.RequiredNumber.ToString(), out int intOutput))
                {
                    var items = RefGenMedProductDetailsList.Where(x => x.GenMedProductID == value.GenMedProductID && x.BrandName.Equals(value.BrandName)).OrderBy(p => p.STT);
                    foreach (RefGenMedProductDetails item in items)
                    {
                        InwardDrugClinicDept p = new InwardDrugClinicDept();
                        p.RefGenMedProductDetails = item;
                        p.GenMedProductID = item.GenMedProductID;
                        p.InBatchNumber = item.InBatchNumber;
                        p.InID = item.InID;
                        //p.NormalPrice = item.OutPrice;    
                        p.NormalPrice = item.NormalPrice;                   
                        p.HIAllowedPrice = item.HIAllowedPrice;
                        p.HIPatientPrice = item.HIPatientPrice;
                        p.InQuantity = value.RequiredNumber;
                        p.InExpiryDate = item.InExpiryDate;
                        p.SdlDescription = item.SdlDescription;
                        p.InCost = item.InCost;
                        //20200111 QTD thêm biến InBuyingPriceActual để lấy InCost vì trong stored truyền giá trị InCost = InBuyingPriceActual
                        p.InBuyingPriceActual = item.InCost;
                        //QTD
                        p.VAT = Convert.ToDecimal(item.VAT);
                        p.GenMedVersionID = Convert.ToInt64(item.GenMedVersionID);
                        CurrentInwardDrugClinicDeptInvoice.InwardDrugs.Add(p);
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
                        break;
                    }
                }
                else
                {
                    MessageBox.Show("Số lượng phải là số nguyên lớn hơn 0!");
                }
            }
            else
            {
                MessageBox.Show(eHCMSResources.K0389_G1_ChonThuocCanBan);
            }
        }
        private void GetDrugForBalanceCompleteFromCategory(string Name, bool? IsCode)
        {
            if (CurrentInwardDrugClinicDeptInvoice == null)
            {
                return;
            }
            if (IsCode == false && Name.Length < 1)
            {
                return;
            }

            long? RequestID = null;
            long? RefGenDrugCatID_1 = null;

            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyMedDeptServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetDrugForBalanceCompleteFromCategory(IsSearchByGenericName, IsCost, Name, CurrentInwardDrugClinicDeptInvoice.StoreID
                            , V_MedProductType, RefGenDrugCatID_1, RequestID, IsCode, 0, null, null, true
                            , Globals.DispatchCallback((asyncResult) =>
                            {
                                try
                                {
                                    var results = contract.EndGetDrugForBalanceCompleteFromCategory(asyncResult);

                                    RefGenMedProductDetailsList = new ObservableCollection<RefGenMedProductDetails>(results);
                                    ListDisplayAutoComplete();
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
                    this.HideBusyIndicator();
                    MessageBox.Show(ex.Message);
                }
            });

            t.Start();
        }
        public void chkSearchByGenericName_Loaded(object sender, RoutedEventArgs e)
        {
            var chkSearchByGenericName = sender as CheckBox;

            if (Globals.ServerConfigSection.ConsultationElements.DefSearchByGenericName && V_MedProductType == (long)AllLookupValues.MedProductType.THUOC)
            {
                chkSearchByGenericName.IsChecked = true;
                vIsSearchByGenericName = true;
            }
            else
            {
                chkSearchByGenericName.IsChecked = false;
            }
        }

        private void ListDisplayAutoComplete()
        {
            var hhh = from hd in RefGenMedProductDetailsList
                      group hd by new { hd.GenMedProductID, hd.BrandName, hd.SelectedUnit.UnitName, hd.RequestQty, hd.Code, hd.ProductCodeRefNum } into hdgroup
                      select new
                      {
                          Remaining = hdgroup.Sum(groupItem => groupItem.Remaining),
                          GenMedProductID = hdgroup.Key.GenMedProductID,
                          UnitName = hdgroup.Key.UnitName,
                          BrandName = hdgroup.Key.BrandName,
                          Code = hdgroup.Key.Code,
                          Qty = hdgroup.Key.RequestQty,
                          ProductCodeRefNum = hdgroup.Key.ProductCodeRefNum
                      };
 
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
                item.ProductCodeRefNum = i.ProductCodeRefNum;
                RefGenMedProductDetailsListSum.Add(item);
            }

            if (IsCode.GetValueOrDefault())
            {
                if (RefGenMedProductDetailsListSum != null && RefGenMedProductDetailsListSum.Count > 0)
                {
                    SelectedSellVisitor = RefGenMedProductDetailsListSum.ToList()[0];
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
        #endregion

        #region RefOutputType
        private ObservableCollection<RefOutputType> _RefOutputTypes;
        public ObservableCollection<RefOutputType> RefOutputTypes
        {
            get { return _RefOutputTypes; }
            set
            {
                if (_RefOutputTypes != value)
                {
                    _RefOutputTypes = value;
                    NotifyOfPropertyChange(() => RefOutputTypes);
                }
            }
        }
        public void LoadRefOutputType()
        {
            RefOutputTypes = Globals.RefOutputType.Where(x => x.TypID == (long)AllLookupValues.RefOutputType.CANBANGKHO).ToObservableCollection();
            if (CurrentInwardDrugClinicDeptInvoice != null && RefOutputTypes != null && RefOutputTypes.Count() > 0)
            {
                CurrentInwardDrugClinicDeptInvoice.TypID = RefOutputTypes.FirstOrDefault().TypID;
            }
        }
        #endregion
    }
}
