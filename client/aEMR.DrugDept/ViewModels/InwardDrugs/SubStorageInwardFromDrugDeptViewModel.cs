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
/*
 * 20190522 #001 TNHX:   [BM0010766] Thêm trường ngày xuất của phiếu xuất để nhập vào kho + refactor code
 * 20210825 #002 QTD: Thêm loại dinh dưỡng
 * 20211102 #003 QTD: Lọc kho theo cấu hình trách nhiệm
 */
namespace aEMR.DrugDept.ViewModels
{
    [Export(typeof(ISubStorageInwardFromDrugDept)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class SubStorageInwardFromDrugDeptViewModel : Conductor<object>, ISubStorageInwardFromDrugDept
        , IHandle<ClinicDrugDeptCloseSearchInwardIncoiceEvent>
    {
        private long TypID = (long)AllLookupValues.RefOutputType.NHAP_TU_LUANCHUYENKHO;
        [ImportingConstructor]
        public SubStorageInwardFromDrugDeptViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            Globals.EventAggregator.Subscribe(this);

            SearchCriteria = new InwardInvoiceSearchCriteria();
            InitInvoice();
            StoreCbx = new ObservableCollection<RefStorageWarehouseLocation>();
            Coroutine.BeginExecute(DoGetSubStore());
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
            //▼===== #003
            StoreCbx = Globals.CheckDrugMedStoreWareHouse(StoreCbx);
            if(StoreCbx == null || StoreCbx.Count == 0)
            {
                MessageBox.Show(eHCMSResources.A0110_G1_Msg_InfoChuaCauHinhTNKho.Replace("\\n ", "\n"));
            }
            //▲===== #003
            yield break;
        }

        private void InitInvoice()
        {
            CurrentInwardDrugClinicDeptInvoice = new InwardDrugClinicDeptInvoice
            {
                InvDateInvoice = DateTime.Now,
                DSPTModifiedDate = DateTime.Now,
                CurrencyID = 129,
                TypID = (long)AllLookupValues.RefOutputType.NHAP_TU_NGUON_KHAC
            };
            if (CurrentInwardDrugClinicDeptInvoice.SelectedStaff == null)
            {
                CurrentInwardDrugClinicDeptInvoice.SelectedStaff = new Staff();
            }
            CurrentInwardDrugClinicDeptInvoice.SelectedStaff.FullName = GetStaffLogin().FullName;
            CurrentInwardDrugClinicDeptInvoice.StaffID = GetStaffLogin().StaffID;
            MedDeptInvoiceList = new ObservableCollection<OutwardDrugMedDeptInvoice>();
            IsEnableSaveBtn = false; //--28/01/2021 DatTB Apply cách của A.Vũ
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
            if (null == StoreID || 0 == StoreID)
            {
                return;
            }
            IsEnableSaveBtn = false;
            MedDeptInvoiceList = null;
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
                                 if (null != results)
                                 {
                                    MedDeptInvoiceList = results.ToObservableCollection();
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

        public void cbxKho_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CurrentInwardDrugClinicDeptInvoice != null && CurrentInwardDrugClinicDeptInvoice.CanSave)
            {
                if (CurrentInwardDrugClinicDeptInvoice.SelectedStorage != null)
                {
                    LoadMedDeptInvoiceList(CurrentInwardDrugClinicDeptInvoice.SelectedStorage.StoreID);
                    CurrentInwardDrugClinicDeptInvoice.CurrentOutwardDrugMedDeptInvoice = null;
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
            if (CurrentInwardDrugClinicDeptInvoice != null && CurrentInwardDrugClinicDeptInvoice.CanSave)
            {
                //--▼--28/01/2021 DatTB Apply cách của A.Vũ
                CurrentInwardDrugClinicDeptInvoice.InwardDrugs = null;
                IsEnableSaveBtn = false;
                //--▲--28/01/2021 DatTB Apply cách của A.Vũ

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
                        CurrentInwardDrugClinicDeptInvoice.InwardDrugs = null;
                    }
                }
            }
        }

        private void OutwardDrugMedDeptDetails_Load(long outiID, bool FromClinicDept)
        {
            IsEnableSaveBtn = false;
            CurrentInwardDrugClinicDeptInvoice.InwardDrugs = null;
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
                                    CurrentInwardDrugClinicDeptInvoice.InwardDrugs = new ObservableCollection<InwardDrugClinicDept>();
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
                                            item.InBuyingPrice = p.InwardDrugMedDept.InBuyingPrice;
                                            item.InCost = p.InwardDrugMedDept.InCost;
                                            item.InBuyingPriceActual = p.InwardDrugMedDept.InCost;
                                            item.VAT = p.InwardDrugMedDept.VAT;
                                        }
                                        item.InQuantity = p.OutQuantity;
                                        item.MedDeptQty = p.OutQuantity;
                                        item.SdlDescription = "";
                                        item.OutID = p.OutID;
                                        item.DrugDeptInIDOrig = p.DrugDeptInIDOrig;
                                        CurrentInwardDrugClinicDeptInvoice.InwardDrugs.Add(item);
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

        private void GetInwardInvoiceDrugByID(long ID, long V_MedProductType) //--02/01/2021 DatTB Thêm biến V_MedProductType
        {
            //cbxKho_SelectionChanged(null, null);
            //return;
            IsEnableSaveBtn = false;
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

        private void InwardDrugClinicDeptInvoice_SaveXML()
        {
            DateTime? DSPTModifiedDate = null;
            if (CurrentInwardDrugClinicDeptInvoice.CurrentOutwardDrugMedDeptInvoice != null)
            {
                //DSPTModifiedDate = CurrentInwardDrugClinicDeptInvoice.DSPTModifiedDate;
                DSPTModifiedDate = CurrentInwardDrugClinicDeptInvoice.CurrentOutwardDrugMedDeptInvoice.DSPTModifiedDate; // 19/12/2020 DatTB
            }
            this.ShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyClinicDeptServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginInwardDrugClinicDeptInvoice_SaveXML(CurrentInwardDrugClinicDeptInvoice, DSPTModifiedDate, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                int results = contract.EndInwardDrugClinicDeptInvoice_SaveXML(out long inviID, asyncResult);
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
                                this.HideBusyIndicator();
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
                //▼====== #002
                else if (V_MedProductType == (long)AllLookupValues.MedProductType.NUTRITION)
                {
                    Globals.ShowMessage(eHCMSResources.Z1094_G1_ChuaNhapYCu, eHCMSResources.G0442_G1_TBao);
                }
                //▲====== #002
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
            IsEnableSaveBtn = false;
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
                //▼====== #008
                else if (V_MedProductType == (long)AllLookupValues.MedProductType.NUTRITION)
                {
                    proAlloc.LyDo = eHCMSResources.Z3208_G1_PhNhapKhoDDuong;
                }
                //▲====== #008
                else
                {
                    proAlloc.LyDo = eHCMSResources.Z0571_G1_PhNhapKhoHChat;
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
    }
}
