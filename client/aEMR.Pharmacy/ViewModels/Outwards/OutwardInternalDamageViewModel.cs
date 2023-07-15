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
using aEMR.Common.Printing;
using aEMR.CommonTasks;
using aEMR.Controls;
using eHCMSLanguage;
using Castle.Windsor;
/*
 * 20200903 #001 TNHX [BM]: Cho phép xuất thuốc nhà thuốc tìm thuốc bằng tên hoạt chất.
 */
namespace aEMR.Pharmacy.ViewModels
{
    [Export(typeof(IOutwardInternalDamage)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class OutwardInternalDamageViewModel : Conductor<object>, IOutwardInternalDamage
        , IHandle<PharmacyCloseSearchDemageDrugEvent>
        , IHandle<ChooseBatchNumberVisitorEvent>, IHandle<ChooseBatchNumberVisitorResetQtyEvent>
        , IHandle<PharmacyPayEvent>
    {

        public string TitleForm { get; set; }

        #region Indicator Member

        private bool _isLoadingCount = false;
        public bool isLoadingCount
        {
            get { return _isLoadingCount; }
            set
            {
                if (_isLoadingCount != value)
                {
                    _isLoadingCount = value;
                    NotifyOfPropertyChange(() => isLoadingCount);
                    NotifyOfPropertyChange(() => IsLoading);
                }
            }
        }

        private bool _isLoadingStaff = false;
        public bool isLoadingStaff
        {
            get { return _isLoadingStaff; }
            set
            {
                if (_isLoadingStaff != value)
                {
                    _isLoadingStaff = value;
                    NotifyOfPropertyChange(() => isLoadingStaff);
                    NotifyOfPropertyChange(() => IsLoading);
                }
            }
        }

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

        private bool _isLoadingHopital = false;
        public bool isLoadingHopital
        {
            get { return _isLoadingHopital; }
            set
            {
                if (_isLoadingHopital != value)
                {
                    _isLoadingHopital = value;
                    NotifyOfPropertyChange(() => isLoadingHopital);
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

        private bool _isLoadingStore1 = false;
        public bool isLoadingStore1
        {
            get { return _isLoadingStore1; }
            set
            {
                if (_isLoadingStore1 != value)
                {
                    _isLoadingStore1 = value;
                    NotifyOfPropertyChange(() => isLoadingStore1);
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




        public bool IsLoading
        {
            get { return (isLoadingCount || isLoadingStaff || isLoadingGetStore || isLoadingFullOperator || isLoadingStore1 || isLoadingHopital || isLoadingGetID || isLoadingSearch || isLoadingDetail); }
        }

        #endregion

        private enum DataGridCol
        {
            ColMultiDelete = 0,
            ColDelete = 1,
            MaThuoc = 2,
            TenThuoc = 3,
            HamLuong = 4,
            DVT = 5,
            LoSX = 6,
            SLYC = 7,
            ThucXuat = 8,
            DonGia = 9,
            ThanhTien = 10
        }
        [ImportingConstructor]
        public OutwardInternalDamageViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg)
        {
            authorization();

            Globals.EventAggregator.Subscribe(this);

            Coroutine.BeginExecute(DoGetStore_EXTERNAL());

            SearchCriteria = new SearchOutwardInfo();
            SearchCriteria.TypID = (long)AllLookupValues.RefOutputType.HUYHANG;

            RefeshData();
            SetDefaultForStore();
        }

        private void RefeshData()
        {
            SelectedOutInvoice = null;
            SelectedOutInvoice = new OutwardDrugInvoice();
            SelectedOutInvoice.OutDate = Globals.ServerDate.Value;
            SelectedOutInvoice.SelectedStaff = GetStaffLogin();
            ClearData();
            TotalPrice = 0;
            HideShowColumnDelete();
        }

        private void ClearData()
        {
            SelectedOutInvoice.outiID = 0;
            SelectedOutInvoice.OutInvID = "";

            OutwardDrugsCopy = null;

            if (GetDrugForSellVisitorList == null)
            {
                GetDrugForSellVisitorList = new ObservableCollection<GetDrugForSellVisitor>();
            }
            else
            {
                GetDrugForSellVisitorList.Clear();
            }
            if (GetDrugForSellVisitorListSum == null)
            {
                GetDrugForSellVisitorListSum = new ObservableCollection<GetDrugForSellVisitor>();
            }
            else
            {
                GetDrugForSellVisitorListSum.Clear();
            }

            if (GetDrugForSellVisitorTemp == null)
            {
                GetDrugForSellVisitorTemp = new ObservableCollection<GetDrugForSellVisitor>();
            }
            else
            {
                GetDrugForSellVisitorTemp.Clear();
            }
        }

        //protected override void OnDeactivate(bool close)
        //{
        //    Globals.EventAggregator.Unsubscribe(this);

        //    SelectedOutInvoice = null;
        //    SearchCriteria = null;
        //    OutwardDrugsCopy = null;
        //    GetDrugForSellVisitorList = null;
        //    GetDrugForSellVisitorTemp = null;
        //    GetDrugForSellVisitorListSum = null;
        //    StoreCbx = null;

        //    BatchNumberListTemp = null;
        //    BatchNumberListShow = null;
        //    OutwardDrugListByDrugID = null;
        //    OutwardDrugListByDrugIDFirst = null;
        //    SelectedOutwardDrug = null;
        //    if (au != null)
        //    {
        //        au.SetValue(AutoCompleteBox.ItemsSourceProperty, null);
        //    }
        //    if (grdPrescription != null)
        //    {
        //        grdPrescription.SetValue(DataGrid.ItemsSourceProperty, null);
        //    }

        //}

        private void SetDefaultForStore()
        {
            if (StoreCbx != null)
            {
                StoreID = StoreCbx.FirstOrDefault().StoreID;
                if (SearchCriteria != null)
                {
                    SearchCriteria.StoreID = StoreID;
                }
            }
        }

        public void authorization()
        {
            if (!Globals.isAccountCheck)
            {
                return;
            }
            mXuatHuy_Tim = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPharmacy
                                               , (int)ePharmacy.mXuatHuy,
                                               (int)oPharmacyEx.mXuatHuy_Tim, (int)ePermission.mView);
            mXuatHuy_Them = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPharmacy
                                               , (int)ePharmacy.mXuatHuy,
                                               (int)oPharmacyEx.mXuatHuy_Them, (int)ePermission.mView);
            mXuatHuy_ChinhSua = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPharmacy
                                               , (int)ePharmacy.mXuatHuy,
                                               (int)oPharmacyEx.mXuatHuy_ChinhSua, (int)ePermission.mView);
            mXuatHuy_XemIn = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPharmacy
                                               , (int)ePharmacy.mXuatHuy,
                                               (int)oPharmacyEx.mXuatHuy_XemIn, (int)ePermission.mView);
            mXuatHuy_In = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPharmacy
                                               , (int)ePharmacy.mXuatHuy,
                                               (int)oPharmacyEx.mXuatHuy_In, (int)ePermission.mView);
        }
        
        #region checking account

        private bool _mXuatHuy_Tim = true;
        private bool _mXuatHuy_Them = true;
        private bool _mXuatHuy_ChinhSua = true;
        private bool _mXuatHuy_XemIn = true;
        private bool _mXuatHuy_In = true;

        public bool mXuatHuy_Tim
        {
            get
            {
                return _mXuatHuy_Tim;
            }
            set
            {
                if (_mXuatHuy_Tim == value)
                    return;
                _mXuatHuy_Tim = value;
                NotifyOfPropertyChange(() => mXuatHuy_Tim);
            }
        }

        public bool mXuatHuy_Them
        {
            get
            {
                return _mXuatHuy_Them;
            }
            set
            {
                if (_mXuatHuy_Them == value)
                    return;
                _mXuatHuy_Them = value;
                NotifyOfPropertyChange(() => mXuatHuy_Them);
            }
        }

        public bool mXuatHuy_ChinhSua
        {
            get
            {
                return _mXuatHuy_ChinhSua;
            }
            set
            {
                if (_mXuatHuy_ChinhSua == value)
                    return;
                _mXuatHuy_ChinhSua = value;
                NotifyOfPropertyChange(() => mXuatHuy_ChinhSua);
            }
        }

        public bool mXuatHuy_XemIn
        {
            get
            {
                return _mXuatHuy_XemIn;
            }
            set
            {
                if (_mXuatHuy_XemIn == value)
                    return;
                _mXuatHuy_XemIn = value;
                NotifyOfPropertyChange(() => mXuatHuy_XemIn);
            }
        }

        public bool mXuatHuy_In
        {
            get
            {
                return _mXuatHuy_In;
            }
            set
            {
                if (_mXuatHuy_In == value)
                    return;
                _mXuatHuy_In = value;
                NotifyOfPropertyChange(() => mXuatHuy_In);
            }
        }


        #endregion

        #region Properties Member

        private bool _IsGetProductHuy = false;
        public bool IsGetProductHuy
        {
            get
            {
                return _IsGetProductHuy;
            }
            set
            {
                if (_IsGetProductHuy != value)
                {
                    _IsGetProductHuy = value;
                    NotifyOfPropertyChange(() => IsGetProductHuy);
                }
                if (_IsGetProductHuy)
                {
                    //goi ham load len ne!
                    if (Type == 0)
                    {
                        rdtExpiry_Checked(null, null);
                    }
                    else if (Type == 1)
                    {
                        rdtPreExpiry_Checked(null, null);
                    }
                    else
                    {
                        rdtAll_Checked(null, null);
                    }
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

        private long _StoreID;
        public long StoreID
        {
            get { return _StoreID; }
            set
            {
                if (_StoreID != value)
                {
                    _StoreID = value;
                    NotifyOfPropertyChange(() => StoreID);
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

        private SearchOutwardInfo _SearchCriteria;
        public SearchOutwardInfo SearchCriteria
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

        private ObservableCollection<OutwardDrug> OutwardDrugsCopy;

        private OutwardDrugInvoice SelectedOutInvoiceCoppy;

        private OutwardDrugInvoice _SelectedOutInvoice;
        public OutwardDrugInvoice SelectedOutInvoice
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

        private int TotalQty;

        #endregion

        private IEnumerator<IResult> DoGetStore_EXTERNAL()
        {
            isLoadingGetStore = true;
            var paymentTypeTask = new LoadStoreListTask((long)AllLookupValues.StoreType.STORAGE_EXTERNAL, false,null, false, false);
            yield return paymentTypeTask;
            StoreCbx = paymentTypeTask.LookupList;
            SetDefaultForStore();
            isLoadingGetStore = false;
            yield break;
        }

        public void grdPrescription_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString() + ". ";

            if (SelectedOutInvoice != null && (SelectedOutInvoice.outiID == 0))
            {
                Button colBatchNumber = grdPrescription.Columns[(int)DataGridCol.LoSX].GetCellContent(e.Row) as Button;
                if (colBatchNumber != null)
                {
                    colBatchNumber.IsEnabled = true;
                }
            }
            else
            {
                Button colBatchNumber = grdPrescription.Columns[(int)DataGridCol.LoSX].GetCellContent(e.Row) as Button;
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
                if (SelectedOutInvoice.OutwardDrugs == null)
                {
                    return false;
                }
                foreach (OutwardDrug item in SelectedOutInvoice.OutwardDrugs)
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

        private void GetOutWardDrugInvoiceVisitorByID(long OutwardID)
        {
            isLoadingGetID = true;
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyInwardDrugServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginGetOutWardDrugInvoiceVisitorByID(OutwardID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            SelectedOutInvoice = contract.EndGetOutWardDrugInvoiceVisitorByID(asyncResult);
                            //co khong cho vao cac su kien 
                            OutwardDrugDetails_Load(SelectedOutInvoice.outiID);

                            HideShowColumnDelete();
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
            OutwardDrugInvoice_Search(0, Globals.PageSize);
        }

        private void OutwardDrugInvoice_Search(int PageIndex, int PageSize)
        {
            if (SearchCriteria == null)
            {
                SearchCriteria = new SearchOutwardInfo();
            }
            SearchCriteria.StoreID = StoreID;

            isLoadingSearch = true;
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyInwardDrugServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginOutWardDrugInvoice_SearchByType(SearchCriteria, PageIndex, PageSize, true, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            int TotalCount = 0;
                            var results = contract.EndOutWardDrugInvoice_SearchByType(out TotalCount, asyncResult);
                            if (results != null && results.Count > 0)
                            {
                                if (results.Count > 1)
                                {
                                    //mo pop up tim
                                    Action<IPharmacyDamageExpiryDrugSearch> onInitDlg = delegate (IPharmacyDamageExpiryDrugSearch proAlloc)
                                    {
                                        proAlloc.SearchCriteria = SearchCriteria.DeepCopy();
                                        proAlloc.OutwardDrugInvoiceList.Clear();
                                        proAlloc.OutwardDrugInvoiceList.TotalItemCount = TotalCount;
                                        proAlloc.OutwardDrugInvoiceList.PageIndex = 0;
                                        proAlloc.pageTitle = eHCMSResources.G2893_G1_XuatNBoTimPhCu;
                                        foreach (OutwardDrugInvoice p in results)
                                        {
                                            proAlloc.OutwardDrugInvoiceList.Add(p);
                                        }
                                    };
                                    GlobalsNAV.ShowDialog<IPharmacyDamageExpiryDrugSearch>(onInitDlg);
                                }
                                else
                                {
                                    SelectedOutInvoice = results.FirstOrDefault();
                                    HideShowColumnDelete();
                                    //load detail
                                    OutwardDrugDetails_Load(SelectedOutInvoice.outiID);
                                }
                            }
                            else
                            {
                                Globals.ShowMessage(eHCMSResources.A0752_G1_Msg_InfoKhTimThay, eHCMSResources.G0442_G1_TBao);
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

        private void OutwardDrugDetails_Load(long outiID)
        {
            isLoadingDetail = true;
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyInwardDrugServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetOutwardDrugDetailsByOutwardInvoice(outiID, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            var results = contract.EndGetOutwardDrugDetailsByOutwardInvoice(asyncResult);
                            //load danh sach thuoc theo hoa don 
                            SelectedOutInvoice.OutwardDrugs = results.ToObservableCollection();
                            SumTotalPrice();
                            DeepCopyOutwardDrug();
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            isLoadingDetail = false;
                            //Globals.IsBusy = false;
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
                    SearchCriteria.OutInvID = (sender as TextBox).Text;
                }
                OutwardDrugInvoice_Search(0, Globals.PageSize);
            }
        }

        private void SumTotalPrice()
        {
            TotalPrice = 0;
            TotalQty = 0;
            if (SelectedOutInvoice != null && SelectedOutInvoice.OutwardDrugs != null)
            {
                for (int i = 0; i < SelectedOutInvoice.OutwardDrugs.Count; i++)
                {
                    TotalPrice += SelectedOutInvoice.OutwardDrugs[i].TotalPrice;
                    TotalQty += SelectedOutInvoice.OutwardDrugs[i].OutQuantity;
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
            IReportDocumentPreview DialogView = Globals.GetViewModel<IReportDocumentPreview>();
            DialogView.ID = SelectedOutInvoice.outiID;
            DialogView.eItem = ReportName.PHARMACY_XUATNOIBO;
            GlobalsNAV.ShowDialog_V3(DialogView, null, null, false, true, Globals.GetDefaultDialogViewSize());
        }

        public void btnPrint()
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                using (var serviceFactory = new ReportServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginGetOutwardInternalInPdfFormat(SelectedOutInvoice.outiID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndGetOutwardInternalInPdfFormat(asyncResult);
                            var printEvt = new ActiveXPrintEvt(this, PrinterType.IN_PHIEU, results, ActiveXPrintType.ByteArray);
                            Globals.EventAggregator.Publish(results);
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

        #endregion

        #region auto Drug For Prescription member
        private string BrandName;
        private bool IsHIPatient = false;

        private ObservableCollection<GetDrugForSellVisitor> GetDrugForSellVisitorList;

        private ObservableCollection<GetDrugForSellVisitor> _GetDrugForSellVisitorSum;
        public ObservableCollection<GetDrugForSellVisitor> GetDrugForSellVisitorListSum
        {
            get { return _GetDrugForSellVisitorSum; }
            set
            {
                if (_GetDrugForSellVisitorSum != value)
                    _GetDrugForSellVisitorSum = value;
                NotifyOfPropertyChange(() => GetDrugForSellVisitorListSum);
            }
        }

        private ObservableCollection<GetDrugForSellVisitor> GetDrugForSellVisitorTemp;

        private GetDrugForSellVisitor _SelectedSellVisitor;
        public GetDrugForSellVisitor SelectedSellVisitor
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
                //tim theo ten
                SearchGetDrugForSellVisitor(e.Parameter, false);
            }
        }
        public void AutoDrug_Text_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            if (au.SelectedItem != null)
            {
                SelectedSellVisitor = au.SelectedItem as GetDrugForSellVisitor;
            }
        }
        private void ListDisplayAutoComplete()
        {
            var hhh = from hd in GetDrugForSellVisitorList
                      group hd by new { hd.DrugID, hd.DrugCode, hd.BrandName, hd.UnitName, hd.Qty } into hdgroup
                      select new
                      {
                          Remaining = hdgroup.Sum(groupItem => groupItem.Remaining),
                          DrugID = hdgroup.Key.DrugID,
                          DrugCode = hdgroup.Key.DrugCode,
                          UnitName = hdgroup.Key.UnitName,
                          BrandName = hdgroup.Key.BrandName,
                          Qty = hdgroup.Key.Qty
                      };
            for (int i = 0; i < hhh.Count(); i++)
            {
                GetDrugForSellVisitor item = new GetDrugForSellVisitor();
                item.DrugID = hhh.ToList()[i].DrugID;
                item.DrugCode = hhh.ToList()[i].DrugCode;
                item.BrandName = hhh.ToList()[i].BrandName;
                item.UnitName = hhh.ToList()[i].UnitName;
                item.Remaining = hhh.ToList()[i].Remaining;
                item.Qty = hhh.ToList()[i].Qty;
                GetDrugForSellVisitorListSum.Add(item);
            }
            if (IsCode.GetValueOrDefault())
            {
                if (GetDrugForSellVisitorListSum != null && GetDrugForSellVisitorListSum.Count > 0)
                {
                    var item = GetDrugForSellVisitorListSum.Where(x => x.DrugCode == txt);
                    if (item != null && item.Count() > 0)
                    {
                        SelectedSellVisitor = item.ToList()[0];
                    }
                    else
                    {
                        MessageBox.Show(eHCMSResources.A0752_G1_Msg_InfoKhTimThay);
                    }
                }
                else
                {
                    MessageBox.Show(eHCMSResources.A0752_G1_Msg_InfoKhTimThay);
                }
            }
            else
            {
                if (au != null)
                {
                    au.ItemsSource = GetDrugForSellVisitorListSum;
                    au.PopulateComplete();
                }
            }
        }

        //▼====== #001
        private void SearchGetDrugForSellVisitor(string Name, bool? IsCode)
        {
            if (IsCode.GetValueOrDefault())
            {
                isLoadingDetail = true;
            }
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyInwardDrugServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetDrugForSellVisitorAutoComplete_ForRequestPharmacy(true, Name, StoreID, 0, IsCode
                        , IsSearchByGenericName, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndGetDrugForSellVisitorAutoComplete_ForRequestPharmacy(asyncResult);
                            GetDrugForSellVisitorList.Clear();
                            GetDrugForSellVisitorListSum.Clear();
                            GetDrugForSellVisitorTemp.Clear();
                            foreach (GetDrugForSellVisitor s in results)
                            {
                                GetDrugForSellVisitorTemp.Add(s);
                            }
                            if (OutwardDrugsCopy != null && OutwardDrugsCopy.Count > 0)
                            {
                                foreach (OutwardDrug d in OutwardDrugsCopy)
                                {
                                    var value = results.Where(x => x.DrugID == d.DrugID && x.InBatchNumber == d.InBatchNumber && x.InID == d.InID);
                                    if (value.Count() > 0)
                                    {
                                        foreach (GetDrugForSellVisitor s in value.ToList())
                                        {
                                            s.Remaining = s.Remaining + d.OutQuantityOld;
                                            s.RemainingFirst = s.RemainingFirst + d.OutQuantityOld;
                                        }
                                    }
                                    else
                                    {
                                        GetDrugForSellVisitor p = d.GetDrugForSellVisitor;
                                        p.Remaining = d.OutQuantity;
                                        p.RemainingFirst = d.OutQuantity;
                                        p.InBatchNumber = d.InBatchNumber;
                                        p.SellingPrice = d.OutPrice;
                                        p.InID = Convert.ToInt64(d.InID);
                                        p.STT = d.STT;
                                        GetDrugForSellVisitorTemp.Add(p);
                                        // d = null;
                                    }
                                }
                            }
                            foreach (GetDrugForSellVisitor s in GetDrugForSellVisitorTemp)
                            {
                                if (SelectedOutInvoice.OutwardDrugs != null && SelectedOutInvoice.OutwardDrugs.Count > 0)
                                {
                                    foreach (OutwardDrug d in SelectedOutInvoice.OutwardDrugs)
                                    {
                                        if (d.DrugID == s.DrugID && d.InBatchNumber == s.InBatchNumber && d.InID == s.InID)
                                        {
                                            s.Remaining = s.Remaining - d.OutQuantity;
                                        }
                                    }
                                }
                                GetDrugForSellVisitorList.Add(s);
                            }
                            ListDisplayAutoComplete();
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            if (IsCode.GetValueOrDefault())
                            {
                                isLoadingDetail = false;
                                if (AxQty != null)
                                {
                                    AxQty.Focus();
                                }
                            }
                        }
                    }), null);
                }
            });

            t.Start();
        }
        //▲====== #001

        private bool CheckValidDrugAuto(GetDrugForSellVisitor temp)
        {
            if (temp == null)
            {
                return false;
            }
            return !temp.HasErrors;
        }

        private void CheckBatchNumberExists(OutwardDrug p)
        {
            bool kq = false;
            if (SelectedOutInvoice.OutwardDrugs == null)
            {
                SelectedOutInvoice.OutwardDrugs = new ObservableCollection<OutwardDrug>();

            }
            foreach (OutwardDrug p1 in SelectedOutInvoice.OutwardDrugs)
            {
                if (p.DrugID == p1.DrugID && p.InBatchNumber == p1.InBatchNumber && p.InID == p1.InID)
                {
                    p1.OutQuantity += p.OutQuantity;
                    // p1.IsLoad = 0;
                    p1.QtyOffer += p.QtyOffer;
                    kq = true;
                    break;
                }
            }
            if (!kq)
            {
                p.HI = p.GetDrugForSellVisitor.InsuranceCover;

                if (p.InwardDrug == null)
                {
                    p.InwardDrug = new InwardDrug();
                    p.InwardDrug.InID = p.InID.GetValueOrDefault();
                    p.InwardDrug.DrugID = p.DrugID;
                }
                p.InvoicePrice = p.OutPrice;
                p.InwardDrug.NormalPrice = p.OutPrice;
                p.InwardDrug.HIPatientPrice = p.OutPrice;
                p.InwardDrug.HIAllowedPrice = p.HIAllowedPrice;

                SelectedOutInvoice.OutwardDrugs.Add(p);

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

        private void ChooseBatchNumber(GetDrugForSellVisitor value)
        {
            var items = GetDrugForSellVisitorList.Where(x => x.DrugID == value.DrugID).OrderBy(p => p.STT);
            foreach (GetDrugForSellVisitor item in items)
            {
                OutwardDrug p = new OutwardDrug();
                if (item.Remaining > 0)
                {
                    if (item.Remaining - value.RequiredNumber < 0)
                    {
                        if (value.Qty > item.Remaining)
                        {
                            p.QtyOffer = item.Remaining;
                            value.Qty = value.Qty - item.Remaining;
                        }
                        else
                        {
                            p.QtyOffer = value.Qty;
                            value.Qty = 0;
                        }
                        value.RequiredNumber = value.RequiredNumber - item.Remaining;
                        p.GetDrugForSellVisitor = item;
                        p.DrugID = item.DrugID;
                        p.InBatchNumber = item.InBatchNumber;
                        p.InID = item.InID;
                        p.OutPrice = item.OutPrice;
                        p.OutQuantity = item.Remaining;
                        p.InExpiryDate = item.InExpiryDate;
                        p.SdlDescription = item.SdlDescription;
                        CheckBatchNumberExists(p);
                        item.Remaining = 0;
                    }
                    else
                    {
                        p.GetDrugForSellVisitor = item;
                        p.DrugID = item.DrugID;
                        p.InBatchNumber = item.InBatchNumber;
                        p.InID = item.InID;
                        //if (value.Qty > (int)value.RequiredNumber)
                        //{
                        //    p.QtyOffer = (int)value.RequiredNumber;
                        //    value.Qty = value.Qty - (int)value.RequiredNumber;
                        //}
                        //else
                        {
                            p.QtyOffer = value.Qty;
                            value.Qty = 0;
                        }
                        p.OutQuantity = (int)value.RequiredNumber;
                        p.OutPrice = item.OutPrice;
                        p.InExpiryDate = item.InExpiryDate;
                        p.SdlDescription = item.SdlDescription;
                        CheckBatchNumberExists(p);
                        item.Remaining = item.Remaining - (int)value.RequiredNumber;
                        break;
                    }
                }
            }
            SumTotalPrice();
        }

        private void AddListOutwardDrug(GetDrugForSellVisitor value)
        {
            if (value != null)
            {
                if (value.RequiredNumber > 0)
                {
                    int intOutput = 0;
                    if (Int32.TryParse(value.RequiredNumber.ToString(), out intOutput))
                    {
                        int a = Convert.ToInt32(value.RequiredNumber);
                        if (CheckValidDrugAuto(value))
                        {
                            ChooseBatchNumber(value);
                        }
                    }
                    else
                    {
                        MessageBox.Show(eHCMSResources.A0972_G1_Msg_InfoSLgKhHopLe);
                    }
                }
                else
                {
                    MessageBox.Show(eHCMSResources.A0776_G1_Msg_InfoSLgLonHon0);
                }
            }
            else
            {
                MessageBox.Show(eHCMSResources.K0389_G1_ChonThuocCanBan);
            }
        }

        public void AddItem(object sender, RoutedEventArgs e)
        {
            AddListOutwardDrug(SelectedSellVisitor);
        }

        #region Properties member
        private ObservableCollection<GetDrugForSellVisitor> BatchNumberListTemp;
        private ObservableCollection<GetDrugForSellVisitor> BatchNumberListShow;
        private ObservableCollection<OutwardDrug> OutwardDrugListByDrugID;
        private ObservableCollection<OutwardDrug> OutwardDrugListByDrugIDFirst;

        private OutwardDrug _SelectedOutwardDrug;
        public OutwardDrug SelectedOutwardDrug
        {
            get { return _SelectedOutwardDrug; }
            set
            {
                if (_SelectedOutwardDrug != value)
                    _SelectedOutwardDrug = value;
                NotifyOfPropertyChange(() => SelectedOutwardDrug);
            }
        }
        #endregion

        private void GetDrugForSellVisitorBatchNumber(long DrugID)
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyInwardDrugServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginGetDrugForSellVisitorBatchNumber(DrugID, StoreID, IsHIPatient, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndGetDrugForSellVisitorBatchNumber(asyncResult);
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
                            Globals.IsBusy = false;
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
                long DrugID = (long)lnkBatchNumber.CommandParameter;
                OutwardDrugListByDrugID = SelectedOutInvoice.OutwardDrugs.Where(x => x.DrugID == DrugID).ToObservableCollection();
                if (OutwardDrugsCopy != null)
                {
                    OutwardDrugListByDrugIDFirst = OutwardDrugsCopy.Where(x => x.DrugID == DrugID).ToObservableCollection();
                }
                GetDrugForSellVisitorBatchNumber(DrugID);
            }
        }

        #region IHandle<ChooseBatchNumberVisitorEvent> Members

        public void Handle(ChooseBatchNumberVisitorEvent message)
        {
            if (message != null && this.IsActive)
            {
                SelectedOutwardDrug.GetDrugForSellVisitor = message.BatchNumberVisitorSelected;
                SelectedOutwardDrug.InBatchNumber = message.BatchNumberVisitorSelected.InBatchNumber;
                SelectedOutwardDrug.InExpiryDate = message.BatchNumberVisitorSelected.InExpiryDate;
                SelectedOutwardDrug.InID = message.BatchNumberVisitorSelected.InID;
                SelectedOutwardDrug.OutPrice = message.BatchNumberVisitorSelected.InCost;
                SelectedOutwardDrug.SdlDescription = message.BatchNumberVisitorSelected.SdlDescription;
                SumTotalPrice();
            }
        }

        #endregion

        #region IHandle<ChooseBatchNumberVisitorResetQtyEvent> Members

        public void Handle(ChooseBatchNumberVisitorResetQtyEvent message)
        {
            if (message != null && this.IsActive)
            {
                SelectedOutwardDrug.GetDrugForSellVisitor = message.BatchNumberVisitorSelected;
                SelectedOutwardDrug.InBatchNumber = message.BatchNumberVisitorSelected.InBatchNumber;
                SelectedOutwardDrug.InExpiryDate = message.BatchNumberVisitorSelected.InExpiryDate;
                SelectedOutwardDrug.InID = message.BatchNumberVisitorSelected.InID;
                SelectedOutwardDrug.OutPrice = message.BatchNumberVisitorSelected.InCost;

                SelectedOutwardDrug.SdlDescription = message.BatchNumberVisitorSelected.SdlDescription;
                SelectedOutwardDrug.OutQuantity = message.BatchNumberVisitorSelected.Remaining;
                SumTotalPrice();
            }
        }

        #endregion

        public void UpdateListToShow()
        {
            if (OutwardDrugListByDrugIDFirst != null)
            {
                foreach (OutwardDrug d in OutwardDrugListByDrugIDFirst)
                {
                    var value = BatchNumberListTemp.Where(x => x.DrugID == d.DrugID && x.InBatchNumber == d.InBatchNumber && x.InID == d.InID);
                    if (value.Count() > 0)
                    {
                        foreach (GetDrugForSellVisitor s in value.ToList())
                        {
                            s.Remaining = s.Remaining + d.OutQuantityOld;
                            s.RemainingFirst = s.RemainingFirst + d.OutQuantityOld;
                        }
                    }
                    else
                    {
                        GetDrugForSellVisitor p = d.GetDrugForSellVisitor;
                        p.Remaining = d.OutQuantity;
                        p.RemainingFirst = d.OutQuantity;
                        p.InBatchNumber = d.InBatchNumber;
                        p.SellingPrice = d.OutPrice;
                        p.InID = Convert.ToInt64(d.InID);
                        p.STT = d.STT;
                        BatchNumberListTemp.Add(p);
                    }
                }
            }
            foreach (GetDrugForSellVisitor s in BatchNumberListTemp)
            {
                if (OutwardDrugListByDrugID.Count > 0)
                {
                    foreach (OutwardDrug d in OutwardDrugListByDrugID)
                    {
                        //20200422 TBL: Trừ luôn số lượng dòng nhập đang được chọn để đổi lô
                        //if (d.DrugID == s.DrugID && d.InBatchNumber == s.InBatchNumber && d.InID == s.InID && d.InID != SelectedOutwardDrug.InID)
                        if (d.DrugID == s.DrugID && d.InBatchNumber == s.InBatchNumber && d.InID == s.InID)
                        {
                            s.Remaining = s.Remaining - d.OutQuantity;
                        }
                    }
                }
            }

            BatchNumberListShow = BatchNumberListTemp.Where(x => x.Remaining > 0).ToObservableCollection();

            if (BatchNumberListShow != null && BatchNumberListShow.Count > 0)
            {
                Action<IChooseBatchNumberVisitor> onInitDlg = delegate (IChooseBatchNumberVisitor proAlloc)
                {
                    proAlloc.SelectedOutwardDrug = SelectedOutwardDrug.DeepCopy();
                    if (BatchNumberListShow != null)
                    {
                        proAlloc.BatchNumberListShow = BatchNumberListShow.DeepCopy();
                    }
                    if (OutwardDrugListByDrugID != null)
                    {
                        proAlloc.OutwardDrugListByDrugID = OutwardDrugListByDrugID.DeepCopy();
                    }
                };
                GlobalsNAV.ShowDialog<IChooseBatchNumberVisitor>(onInitDlg);
            }
            else
            {
                Globals.ShowMessage(eHCMSResources.Z0891_G1_KgConLoNaoKhac, eHCMSResources.G0442_G1_TBao);
            }
        }
        #endregion

        private void DeleteInvoiceDrugInObject()
        {
            OutwardDrug p = SelectedOutwardDrug.DeepCopy();
            SelectedOutInvoice.OutwardDrugs.Remove(SelectedOutwardDrug);
            foreach (OutwardDrug item in SelectedOutInvoice.OutwardDrugs)
            {
                if (item.DrugID == p.DrugID)
                {
                    item.QtyOffer = item.QtyOffer + p.QtyOffer;
                    break;
                }
            }
            SelectedOutInvoice.OutwardDrugs = SelectedOutInvoice.OutwardDrugs.ToObservableCollection();
            SumTotalPrice();
        }

        public void lnkDelete_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedOutwardDrug != null && SelectedOutInvoice.CanSaveAndPaid)
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
                RefeshData();
                SelectedSellVisitor = new GetDrugForSellVisitor(); //20200418 TBL: Khi đổi kho thì dữ liệu trên autocomplete phải clear
            }
        }


        public void btnFindRequest(object sender, RoutedEventArgs e)
        {
            //IsChanged = true;
            Action<IRequestSearchPharmacy> onInitDlg = delegate (IRequestSearchPharmacy proAlloc)
            {
                if (proAlloc.SearchCriteria == null)
                {
                    proAlloc.SearchCriteria = new RequestSearchCriteria();
                }
                proAlloc.SearchCriteria.DaNhanHang = true;
                proAlloc.SearchRequestDrugInward(0, Globals.PageSize);
            };
            GlobalsNAV.ShowDialog<IRequestSearchPharmacy>(onInitDlg);
        }


        private void OutwardDrugInvoice_SaveByType(OutwardDrugInvoice OutwardInvoice, bool bThuTien)
        {
            isLoadingFullOperator = true;
            // Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                //using (var serviceFactory = new PharmacyInwardDrugServiceClient())
                using (var serviceFactory = new PharmacySaleAndOutwardClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    //contract.BeginListDrugExpiryDate_Save(OutwardInvoice, Globals.DispatchCallback((asyncResult) =>
                    contract.BeginListDrugExpiryDate_Save_Pst(OutwardInvoice, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            long OutID = 0;
                            string StrError;
                            //bool value = contract.EndListDrugExpiryDate_Save(out OutID, out StrError, asyncResult);
                            bool value = contract.EndListDrugExpiryDate_Save_Pst(out OutID, out StrError, asyncResult);
                            if (string.IsNullOrEmpty(StrError) && value)
                            {
                                Globals.ShowMessage(eHCMSResources.A0468_G1_Msg_InfoLuuOK, eHCMSResources.G0442_G1_TBao);
                                RefeshData();
                                GetOutWardDrugInvoiceVisitorByID(OutID);
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
                            isLoadingFullOperator = false;
                            // Globals.IsBusy = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }

        public void btnSave()
        {
            if (CheckData())
            {
                SelectedOutInvoice.V_OutDrugInvStatus = (long)AllLookupValues.V_OutDrugInvStatus.SAVE;
                SelectedOutInvoice.TypID = (long)AllLookupValues.RefOutputType.HUYHANG;
                OutwardDrugInvoice_SaveByType(SelectedOutInvoice, false);
            }
        }

        private bool CheckData()
        {
            if (SelectedOutInvoice != null)
            {
                SelectedOutInvoice.StaffID = GetStaffLogin().StaffID;
                SelectedOutInvoice.StoreID = StoreID;
            }
            if (string.IsNullOrEmpty(SelectedOutInvoice.Notes))
            {
                Globals.ShowMessage(eHCMSResources.Z1634_G1_NhapGChu, eHCMSResources.G0442_G1_TBao);
                return false;
            }

            if (SelectedOutInvoice.OutwardDrugs == null || SelectedOutInvoice.OutwardDrugs.Count == 0)
            {
                Globals.ShowMessage(eHCMSResources.A0640_G1_Msg_InfoKhCoCTietPhXuat, eHCMSResources.G0442_G1_TBao);
                return false;
            }
            else
            {
                for (int i = 0; i < SelectedOutInvoice.OutwardDrugs.Count; i++)
                {
                    if (SelectedOutInvoice.OutwardDrugs[i].RefGenericDrugDetail != null && SelectedOutInvoice.OutwardDrugs[i].OutQuantity <= 0)
                    {
                        Globals.ShowMessage(eHCMSResources.Z1174_G1_SLgXuatLonHon0, eHCMSResources.G0442_G1_TBao);
                        return false;
                    }
                }
            }
            return true;
        }

        public void OutWardDrugInvoice_Delete(long ID)
        {
            isLoadingFullOperator = true;
            //  Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                //using (var serviceFactory = new PharmacyInwardDrugServiceClient())
                using (var serviceFactory = new PharmacySaleAndOutwardClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    //contract.BeginOutWardDrugInvoice_Delete(ID, Globals.DispatchCallback((asyncResult) =>
                    contract.BeginOutWardDrugInvoice_Delete_Pst(ID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            //int results = contract.EndOutWardDrugInvoice_Delete(asyncResult);
                            int results = contract.EndOutWardDrugInvoice_Delete_Pst(asyncResult);
                            if (results == 0)
                            {
                                Globals.ShowMessage(eHCMSResources.K0537_G1_XoaOk, eHCMSResources.G0442_G1_TBao);
                                RefeshData();

                            }
                            else if (results == 1)
                            {
                                Globals.ShowMessage(eHCMSResources.Z1635_G1_PhKgTheXoa, eHCMSResources.T0074_G1_I);
                            }
                            else
                            {
                                Globals.ShowMessage(eHCMSResources.Z0577_G1_PhKgTonTai, eHCMSResources.T0074_G1_I);
                            }
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            isLoadingFullOperator = false;
                            // Globals.IsBusy = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }

        public void btnDeletePhieu()
        {
            if (MessageBox.Show(eHCMSResources.A0171_G1_Msg_ConfXoaPh, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                if (SelectedOutInvoice != null && SelectedOutInvoice.outiID > 0)
                {
                    OutWardDrugInvoice_Delete(SelectedOutInvoice.outiID);
                }
            }
        }

        public void btnNew()
        {
            if (MessageBox.Show(eHCMSResources.A0148_G1_Msg_ConfTaoMoiPhXuat, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                RefeshData();
            }
        }

        private void DeepCopyOutwardDrug()
        {
            if (SelectedOutInvoice.OutwardDrugs != null)
            {
                OutwardDrugsCopy = SelectedOutInvoice.OutwardDrugs.DeepCopy();
            }
            else
            {
                OutwardDrugsCopy = null;
            }
            if (SelectedOutInvoice != null)
            {
                SelectedOutInvoiceCoppy = SelectedOutInvoice.DeepCopy();
            }

        }

        //dung lai from tim kiem cua Demage
        #region IHandle<PharmacyCloseSearchDemageDrugEvent> Members

        public void Handle(PharmacyCloseSearchDemageDrugEvent message)
        {
            if (message != null && this.IsActive)
            {
                SelectedOutInvoice = message.SelectedOutwardDrugInvoice as OutwardDrugInvoice;
                HideShowColumnDelete();
                OutwardDrugDetails_Load(SelectedOutInvoice.outiID);
            }
        }

        #endregion

        #region IHandle<PharmacySaveAndPayEvent> Members

        //public void Handle(PharmacySaveAndPayEvent message)
        //{
        //    if (this.IsActive)
        //    {
        //        SelectedOutInvoice.V_OutDrugInvStatus = (long)AllLookupValues.V_OutDrugInvStatus.PAID;
        //        SaveAndInsertTransaction(SelectedOutInvoice);
        //    }
        //}

        #endregion

        #region IHandle<PharmacyPayEvent> Members

        private void AddTransactionVisitor(PatientTransactionPayment payment, OutwardDrugInvoice InvoiceDrug)
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyInwardDrugServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginAddTransactionVisitor(payment, InvoiceDrug, GetStaffLogin().StaffID, Globals.DeptLocation.DeptLocationID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            long PaymentID = 0;
                            bool value = contract.EndAddTransactionVisitor(out PaymentID, asyncResult);
                            if (value)
                            {
                                //show report
                                Action<IReportDocumentPreview> onInitDlg = delegate (IReportDocumentPreview proAlloc)
                                {
                                    proAlloc.ID = InvoiceDrug.outiID;
                                    proAlloc.PaymentID = PaymentID;
                                    proAlloc.StaffFullName = GetStaffLogin().FullName;
                                    proAlloc.V_TranRefType = (long)AllLookupValues.V_TranRefType.DRUG_NGOAITRU;
                                    if (payment.CreditOrDebit == 1)
                                    {
                                        proAlloc.LyDo = eHCMSResources.Z0335_G1_ThuTienBanThuocLe;
                                        proAlloc.eItem = ReportName.PHARMACY_VISITORPHIEUTHU;
                                    }
                                    else
                                    {
                                        proAlloc.LyDo = eHCMSResources.Z1192_G1_DoiThuoc;
                                        proAlloc.eItem = ReportName.PHARMACY_PHIEUCHI;
                                    }
                                };
                                GlobalsNAV.ShowDialog<IReportDocumentPreview>(onInitDlg);

                                GetOutWardDrugInvoiceVisitorByID(InvoiceDrug.outiID);
                            }
                            else
                            {
                                MessageBox.Show(eHCMSResources.A0766_G1_Msg_InfoLoHetThuoc);
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

        public void Handle(PharmacyPayEvent message)
        {
            if (this.IsActive)
            {
                AddTransactionVisitor(message.CurPatientPayment, SelectedOutInvoice);
            }
        }

        #endregion

        string txt = "";
        public void AxTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            txt = (sender as TextBox).Text;
            if (!string.IsNullOrEmpty(txt))
            {
                SearchGetDrugForSellVisitor(txt, true);
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
                    SearchGetDrugForSellVisitor((sender as TextBox).Text, true);
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
        private bool? IsCode = false;
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

        #region Checked All Member
        private bool _AllChecked;
        public bool AllChecked
        {
            get
            {
                return _AllChecked;
            }
            set
            {
                if (_AllChecked != value)
                {
                    _AllChecked = value;
                    NotifyOfPropertyChange(() => AllChecked);
                    if (_AllChecked)
                    {
                        AllCheckedfc();
                    }
                    else
                    {
                        UnCheckedfc();
                    }
                }
            }
        }

        private void HideShowColumnDelete()
        {
            if (grdPrescription != null)
            {
                if (SelectedOutInvoice.CanSaveAndPaid && _mXuatHuy_Them)
                {
                    grdPrescription.Columns[(int)DataGridCol.ColDelete].Visibility = Visibility.Visible;
                    grdPrescription.Columns[(int)DataGridCol.ColMultiDelete].Visibility = Visibility.Visible;
                }
                else
                {
                    grdPrescription.Columns[(int)DataGridCol.ColDelete].Visibility = Visibility.Collapsed;
                    grdPrescription.Columns[(int)DataGridCol.ColMultiDelete].Visibility = Visibility.Collapsed;
                }
            }
        }

        private void AllCheckedfc()
        {
            if (SelectedOutInvoice != null && SelectedOutInvoice.OutwardDrugs != null && SelectedOutInvoice.OutwardDrugs.Count > 0)
            {
                for (int i = 0; i < SelectedOutInvoice.OutwardDrugs.Count; i++)
                {
                    SelectedOutInvoice.OutwardDrugs[i].Checked = true;
                }
            }
        }

        private void UnCheckedfc()
        {
            if (SelectedOutInvoice != null && SelectedOutInvoice.OutwardDrugs != null && SelectedOutInvoice.OutwardDrugs.Count > 0)
            {
                for (int i = 0; i < SelectedOutInvoice.OutwardDrugs.Count; i++)
                {
                    SelectedOutInvoice.OutwardDrugs[i].Checked = false;
                }
            }
        }

        public void btnDeleteHang()
        {
            if (SelectedOutInvoice != null && SelectedOutInvoice.OutwardDrugs != null && SelectedOutInvoice.OutwardDrugs.Count > 0)
            {
                var items = SelectedOutInvoice.OutwardDrugs.Where(x => x.Checked == true);
                if (items != null && items.Count() > 0)
                {
                    var lstremaning = SelectedOutInvoice.OutwardDrugs.Where(x => x.Checked == false).ToObservableCollection().DeepCopy();
                    var lstDelete = SelectedOutInvoice.OutwardDrugs.Where(x => x.Checked == true).ToObservableCollection().DeepCopy();

                    if (MessageBox.Show(eHCMSResources.Z0565_G1_CoChacXoaHangDaChon, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                    {
                        foreach (OutwardDrug p in lstDelete)
                        {
                            foreach (OutwardDrug item in lstremaning)
                            {
                                if (item.DrugID == p.DrugID)
                                {
                                    item.QtyOffer = item.QtyOffer + p.QtyOffer;
                                    break;
                                }
                            }
                        }
                        SumTotalPrice();
                        ListDisplayAutoComplete();
                    }
                    SelectedOutInvoice.OutwardDrugs = lstremaning;
                }
                else
                {
                    MessageBox.Show(eHCMSResources.A0090_G1_Msg_InfoChuaChonHgCanXoa);
                }
            }
            else
            {
                MessageBox.Show(eHCMSResources.A0090_G1_Msg_InfoChuaChonHgCanXoa);
            }
        }
        #endregion

        public void grdPrescription_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.Column.DisplayIndex == (int)DataGridCol.DonGia || e.Column.DisplayIndex == (int)DataGridCol.ThucXuat)
            {
                SumTotalPrice();
            }
        }

        #region Huy Thuoc Het Han Dung
        private int Type = 0;

        public void rdtExpiry_Checked(object sender, RoutedEventArgs e)
        {
            if (IsGetProductHuy)
            {
                ClearData();
                Type = 0;
                SelectedOutInvoice.OutwardDrugs = null;
                OutwardDrugDetails_Get(StoreID);
            }
        }

        public void rdtPreExpiry_Checked(object sender, RoutedEventArgs e)
        {
            if (IsGetProductHuy)
            {
                ClearData();
                Type = 1;
                SelectedOutInvoice.OutwardDrugs = null;
                OutwardDrugDetails_Get(StoreID);
            }

        }
        public void rdtAll_Checked(object sender, RoutedEventArgs e)
        {
            if (IsGetProductHuy)
            {
                ClearData();
                Type = 2;
                SelectedOutInvoice.OutwardDrugs = null;
                OutwardDrugDetails_Get(StoreID);
            }
        }

        private void OutwardDrugDetails_Get(long ID)
        {
            isLoadingDetail = true;
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyInwardDrugServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetListDrugExpiryDate(ID, Type, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            var results = contract.EndGetListDrugExpiryDate(asyncResult);
                            //load danh sach thuoc theo hoa don 
                            if (results == null || results.Count == 0)
                            {
                                if (Type == 0)
                                {
                                    MessageBox.Show(eHCMSResources.A0651_G1_Msg_InfoKhCoHgHetHan);
                                }
                                else if (Type == 1)
                                {
                                    MessageBox.Show(eHCMSResources.A0649_G1_Msg_InfoKhCoHgDenHanHuy);
                                }
                                else
                                {
                                    MessageBox.Show(eHCMSResources.A0650_G1_Msg_InfoKhCoHgHetHoacDenHan);
                                }
                            }

                            SelectedOutInvoice.OutwardDrugs = results.ToObservableCollection();
                            //tinh tong tien 
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            isLoadingDetail = false;
                            //Globals.IsBusy = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }

        #endregion

        public void btnUpdate()
        {
            IEditOutwardInternalDamage DialogView = Globals.GetViewModel<IEditOutwardInternalDamage>();
            DialogView.TitleForm = "CẬP NHẬT PHIẾU XUẤT";
            DialogView.OutwardDrugListCopy = SelectedOutInvoice.OutwardDrugs.DeepCopy();
            DialogView.ListOutwardDrugFirstCopy = SelectedOutInvoice.OutwardDrugs.DeepCopy();
            DialogView.ListOutwardDrugFirst = SelectedOutInvoice.OutwardDrugs.DeepCopy();
            DialogView.SelectedOutInvoice = SelectedOutInvoice.DeepCopy();
            DialogView.SelectedOutInvoiceCoppy = SelectedOutInvoice.DeepCopy();
            DialogView.SumTotalPrice = SelectedOutInvoice.TotalInvoicePrice.DeepCopy();
            DialogView.IDFirst = SelectedOutInvoice.outiID.DeepCopy();
            GlobalsNAV.ShowDialog_V3(DialogView, null, null, false, true, Globals.GetDefaultDialogViewSize());
        }

        //▼====== #001
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

        private bool _visSearchByGenericName = Globals.ServerConfigSection.PharmacyElements.PharmacySearchByGenericName;
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

        public void chkSearchByGenericName_Loaded(object sender, RoutedEventArgs e)
        {
            var chkSearchByGenericName = sender as CheckBox;

            if (Globals.ServerConfigSection.PharmacyElements.PharmacySearchByGenericName)
            {
                chkSearchByGenericName.IsChecked = true;
            }
            else
            {
                chkSearchByGenericName.IsChecked = false;
            }
        }
        //▲======= #001
    }
}
