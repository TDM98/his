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
using aEMR.CommonTasks;
using aEMR.Controls;
using aEMR.Common.PagedCollectionView;
using Microsoft.Win32;
using Castle.Windsor;
using System.Windows.Data;
using System.IO;
using OfficeOpenXml;
/*
* 20181008 #001 TTM: Chuyển từ PageCollectionView => CollectionViewSource. Do WPF ko sử dụng đc PagedCollectionView (hoặc do mình không biết xài PagedCollectionView).
* 20210916 #002 QTD: Thêm cấu hình mở khóa kho
*/
namespace aEMR.StoreDept.StockTakes.ViewModels
{
    [Export(typeof(IStoreDeptStockTakes)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class StoreDeptStockTakesViewModel : Conductor<object>, IStoreDeptStockTakes
        , IHandle<ClinicDeptCloseSearchStockTakesEvent>
    {
        [ImportingConstructor]
        public StoreDeptStockTakesViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg)
        {
            Globals.EventAggregator.Subscribe(this);
            CurrentClinicDeptStockTakes = new ClinicDeptStockTakes();

            //Coroutine.BeginExecute(DoGetStore_ClinicDept());
            //StoreCbx = Globals.checkStoreWareHouse(false, false);
            //if (StoreCbx == null || StoreCbx.Count < 1)
            //{
            //    MessageBox.Show(eHCMSResources.A0110_G1_Msg_InfoChuaCauHinhTNKho);
            //}
            //else
            //{
            //    RefStorageWarehouseLocation firstItem = new RefStorageWarehouseLocation();
            //    firstItem.StoreID = 0;
            //    firstItem.swhlName = "-- Hãy chọn kho --";
            //    StoreCbx.Insert(0, firstItem);
            //}

            SearchCriteria = new ClinicDeptStockTakesSearchCriteria();

            GetStaffLogin();
            UnCheckPaging();

            V_StockTakeTypeList = Globals.AllLookupValueList.Where(x => x.ObjectTypeID == (long)LookupValues.V_StockTakeType).ToObservableCollection();
            SetDefaultStockTakeType();
        }

        public void InitData()
        {
            if (V_MedProductType == (long)AllLookupValues.MedProductType.THUOC)
            {
                StoreCbx = Globals.checkStoreWareHouse(V_MedProductType, false, false, true, false, false);
            }
            else if (V_MedProductType == (long)AllLookupValues.MedProductType.Y_CU)
            {
                StoreCbx = Globals.checkStoreWareHouse(V_MedProductType, false, false, false, true, false);
            }
            else if (V_MedProductType == (long)AllLookupValues.MedProductType.HOA_CHAT)
            {
                StoreCbx = Globals.checkStoreWareHouse(V_MedProductType, false, false, false, false, true);
            }

            else if (V_MedProductType == (long)AllLookupValues.MedProductType.VTYT_TIEUHAO)
            {
                StoreCbx = Globals.checkStoreWareHouse(V_MedProductType, false, false, false, false, true);
            }

            else if (V_MedProductType == (long)AllLookupValues.MedProductType.TIEM_NGUA)
            {
                StoreCbx = Globals.checkStoreWareHouse(V_MedProductType, false, false, false, false, true);
            }

            else if (V_MedProductType == (long)AllLookupValues.MedProductType.MAU)
            {
                StoreCbx = Globals.checkStoreWareHouse(V_MedProductType, false, false, false, false, true);
            }

            else if (V_MedProductType == (long)AllLookupValues.MedProductType.VATTU_TIEUHAO)
            {
                StoreCbx = Globals.checkStoreWareHouse(V_MedProductType, false, false, false, false, true);
            }

            else if (V_MedProductType == (long)AllLookupValues.MedProductType.VAN_PHONG_PHAM)
            {
                StoreCbx = Globals.checkStoreWareHouse(V_MedProductType, false, false, false, false, true);
            }

            else if (V_MedProductType == (long)AllLookupValues.MedProductType.THANHTRUNG)
            {
                StoreCbx = Globals.checkStoreWareHouse(V_MedProductType, false, false, false, false, true);
            }

            else if (V_MedProductType == (long)AllLookupValues.MedProductType.NUTRITION)
            {
                StoreCbx = Globals.checkStoreWareHouse(V_MedProductType, false, false, false, true, false);
            }

            if (StoreCbx == null || StoreCbx.Count < 1)
            {
                MessageBox.Show(eHCMSResources.A0110_G1_Msg_InfoChuaCauHinhTNKho);
            }
            else
            {
                RefStorageWarehouseLocation firstItem = new RefStorageWarehouseLocation();
                firstItem.StoreID = 0;
                firstItem.swhlName = "-- Hãy chọn kho --";
                StoreCbx.Insert(0, firstItem);
            }
        }

        protected override void OnDeactivate(bool close)
        {
            CurrentClinicDeptStockTakes = null;
            ClinicDeptStockTakeDetailList = null;
            //PCVClinicDeptStockTakeDetails = null;
            SearchCriteria = null;

        }
        #region Propeties Member

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

        private long _V_MedProductType = (long)AllLookupValues.MedProductType.THUOC; //11001 : thuoc, 11002 : y cu , 11003 :hoa chat
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

        private string _SearchKey;
        public string SearchKey
        {
            get { return _SearchKey; }
            set
            {
                _SearchKey = value;
                NotifyOfPropertyChange(() => SearchKey);
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

        private ClinicDeptStockTakes _CurrentClinicDeptStockTakes;
        public ClinicDeptStockTakes CurrentClinicDeptStockTakes
        {
            get
            {
                return _CurrentClinicDeptStockTakes;
            }
            set
            {
                if (_CurrentClinicDeptStockTakes != value)
                {
                    _CurrentClinicDeptStockTakes = value;
                    NotifyOfPropertyChange(() => CurrentClinicDeptStockTakes);
                }
            }
        }

        //private PagedCollectionView _PCVClinicDeptStockTakeDetails;
        //public PagedCollectionView PCVClinicDeptStockTakeDetails
        //{
        //    get
        //    {
        //        return _PCVClinicDeptStockTakeDetails;
        //    }
        //    set
        //    {
        //        if (_PCVClinicDeptStockTakeDetails != value)
        //        {
        //            _PCVClinicDeptStockTakeDetails = value;
        //            NotifyOfPropertyChange(() => PCVClinicDeptStockTakeDetails);
        //        }
        //    }
        //}

        private int _PCVPageSize = 15;
        public int PCVPageSize
        {
            get
            {
                return _PCVPageSize;
            }
            set
            {
                if (_PCVPageSize != value)
                {
                    _PCVPageSize = value;
                    NotifyOfPropertyChange(() => PCVPageSize);
                }
            }
        }

        private Visibility _VisibilityPaging = Visibility.Collapsed;
        public Visibility VisibilityPaging
        {
            get
            {
                return _VisibilityPaging;
            }
            set
            {
                if (_VisibilityPaging != value)
                {
                    _VisibilityPaging = value;
                    NotifyOfPropertyChange(() => VisibilityPaging);
                }
            }
        }

        private ObservableCollection<ClinicDeptStockTakeDetails> ClinicDeptStockTakeDetailList;

        private ObservableCollection<ClinicDeptStockTakeDetails> ClinicDeptStockTakeDetailList_Hide;

        private ClinicDeptStockTakesSearchCriteria _SearchCriteria;
        public ClinicDeptStockTakesSearchCriteria SearchCriteria
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

        private Staff GetStaffLogin()
        {
            if (CurrentClinicDeptStockTakes != null)
            {
                CurrentClinicDeptStockTakes.StaffID = Globals.LoggedUserAccount.Staff.StaffID;
                CurrentClinicDeptStockTakes.FullName = Globals.LoggedUserAccount.Staff.FullName;
            }
            return Globals.LoggedUserAccount.Staff;
        }

        #endregion


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

        public bool IsLoading
        {
            get { return (isLoadingGetStore || isLoadingFullOperator || isLoadingGetID || isLoadingSearch || isLoadingDetail || IsLoadingRefGenericDrugCategory); }
        }

        #endregion


        #region Function Member

        private IEnumerator<IResult> DoGetStore_ClinicDept()
        {
            isLoadingGetStore = true;
            var paymentTypeTask = new LoadStoreListTask((long)AllLookupValues.StoreType.STORAGE_CLINIC, false, null, false, false);
            yield return paymentTypeTask;
            StoreCbx = paymentTypeTask.LookupList;
            isLoadingGetStore = false;
            yield break;
        }

        private void ClinicDeptStockTakes_Search(int PageIndex, int PageSize)
        {
            if (SearchCriteria == null)
            {
                SearchCriteria = new ClinicDeptStockTakesSearchCriteria();
            }
            SearchCriteria.V_MedProductType = V_MedProductType;

            // TxD 04/04/2015 : Added StoreID to Search Criteria
            SearchCriteria.StoreID = CurrentClinicDeptStockTakes.StoreID;

            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            isLoadingSearch = true;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyMedDeptServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginClinicDeptStockTakes_Search(SearchCriteria, PageIndex, PageSize, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            int TotalCount = 0;
                            var results = contract.EndClinicDeptStockTakes_Search(out TotalCount, asyncResult);
                            if (results != null && results.Count > 0)
                            {
                                if (results.Count > 1)
                                {
                                    //mo pop up tim
                                    Action<IStoreDeptStockTakesSearch> onInitDlg = delegate (IStoreDeptStockTakesSearch proAlloc)
                                    {
                                        proAlloc.SearchCriteria = SearchCriteria.DeepCopy();
                                        proAlloc.ClinicDeptStockTakeList.Clear();
                                        proAlloc.ClinicDeptStockTakeList.TotalItemCount = TotalCount;
                                        proAlloc.ClinicDeptStockTakeList.PageIndex = 0;
                                        foreach (ClinicDeptStockTakes p in results)
                                        {
                                            proAlloc.ClinicDeptStockTakeList.Add(p);
                                        }
                                    };
                                    GlobalsNAV.ShowDialog<IStoreDeptStockTakesSearch>(onInitDlg);
                                }
                                else
                                {
                                    CurrentClinicDeptStockTakes = results.FirstOrDefault();
                                    //load detail
                                    UnCheckPaging();
                                    ClinicDeptStockTakeDetails_Load(CurrentClinicDeptStockTakes.ClinicDeptStockTakeID);
                                }
                                GetLastClinicDeptStockTakes(CurrentClinicDeptStockTakes.StoreID);
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
                            Globals.IsBusy = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }

        private void UnCheckPaging()
        {
            if (PagingChecked != null && ClinicDeptStockTakeDetailList != null)
            {
                PagingChecked.IsChecked = false;
                VisibilityPaging = Visibility.Collapsed;
            }
        }

        private void ClinicDeptStockTakeDetails_Load(long ClinicDeptStockTakeID)
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            //isLoadingDetail = true;
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyMedDeptServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginClinicDeptStockTakeDetails_Load(ClinicDeptStockTakeID, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            var results = contract.EndClinicDeptStockTakeDetails_Load(asyncResult);
                            //load danh sach thuoc theo hoa don 
                            ClinicDeptStockTakeDetailList = results.ToObservableCollection();
                            LoadDataGrid();

                            CanGetStockTake = false;
                        }
                        catch (Exception ex)
                        {
                            //isLoadingDetail = false;
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            //isLoadingDetail = false;
                            //Globals.IsBusy = false;
                            this.HideBusyIndicator();
                        }

                    }), null);

                }

            });

            t.Start();
        }

        public bool CheckCaculatedQty()
        {
            if (ClinicDeptStockTakeDetailList == null || ClinicDeptStockTakeDetailList.Count <= 0)
            {
                return false;
            }

            string error = "";

            int limitRow = 10;

            int count = 0;
            foreach (ClinicDeptStockTakeDetails item in ClinicDeptStockTakeDetailList)
            {
                if (item.CaculatedQty < 0)
                {
                    if (count < limitRow)
                    {
                        error += "\t" + (count + 1).ToString() + ". " + item.Code + " - " + item.BrandName + Environment.NewLine;
                        count++;
                    }
                    else
                    {
                        error += "\t..." + Environment.NewLine;
                        break;
                    }
                }
            }

            if (!string.IsNullOrEmpty(error))
            {
                MessageBox.Show(string.Format(eHCMSResources.Z1285_G1_I, error), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return false;
            }

            return true;
        }

        private void ClinicDeptStockTakeDetails_Get(long ID)
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            //isLoadingDetail = true;
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyMedDeptServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginClinicDeptStockTakeDetails_Get(ID, V_MedProductType, CurrentClinicDeptStockTakes.StockTakingDate, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            var results = contract.EndClinicDeptStockTakeDetails_Get(asyncResult);

                            if (CurrentClinicDeptStockTakes.V_StockTakeType == (long)AllLookupValues.V_StockTakeType.KIEMKE_KETCHUYEN)
                            {
                                ClinicDeptStockTakeDetailList = results.ToObservableCollection();
                                CheckCaculatedQty();
                            }
                            else
                            {
                                //Nếu là kiểm kê bổ sung thì lưu kết quả vào ClinicDeptStockTakeDetailList_Hide (không hiển thị lên giao diện), để khi user tìm thuốc bằng autocomplete, thì copy số lượng lý thuyết từ list này (07/05/2015 14:32).
                                ClinicDeptStockTakeDetailList_Hide = results.ToObservableCollection();
                            }
                            LoadDataGrid();
                            CanGetStockTake = false;
                            GetLastClinicDeptStockTakes(CurrentClinicDeptStockTakes.StoreID);
                            //tinh tong tien 
                        }
                        catch (Exception ex)
                        {
                            //isLoadingDetail = false;
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            //isLoadingDetail = false;
                            //Globals.IsBusy = false;
                            this.HideBusyIndicator();
                        }

                    }), null);

                }

            });

            t.Start();
        }

        /// <summary>
        /// VuTTM
        /// </summary>
        /// <param name="storeId"></param>
        private void ReGetClinicDeptStockTakeDetails(long storeId)
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyMedDeptServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginReGetClinicDeptStockTakeDetails(storeId, V_MedProductType, CurrentClinicDeptStockTakes.StockTakingDate, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndReGetClinicDeptStockTakeDetails(asyncResult);
                            if (CurrentClinicDeptStockTakes.V_StockTakeType == (long)AllLookupValues.V_StockTakeType.KIEMKE_KETCHUYEN)
                            {
                                ClinicDeptStockTakeDetailList = results.ToObservableCollection();
                                CheckCaculatedQty();
                            }
                            else
                            {
                                ClinicDeptStockTakeDetailList_Hide = results.ToObservableCollection();
                            }
                            if (null != results)
                            {
                                CurrentClinicDeptStockTakes.ClinicDeptStockTakeID = 0;
                            }
                            GetLastClinicDeptStockTakes(CurrentClinicDeptStockTakes.StoreID);

                            LoadDataGrid();
                            CanGetStockTake = false;
                            mKiemKe_ThemMoi = true;
                            IsReInsert = true;
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

        // TxD 19/04/2015: Changed to adapt new rule for Removing Rows With All Zeros
        private void CheckFilteringOutRowWithAllZeroValues()
        {
            foreach (var stItem in CurrentClinicDeptStockTakes.StockTakeDetails)
            {
                // If a row marked by Stored Proc to be deleted and user added adjustment qty then it should not be deleted
                if (stItem.RowActionStatusFlag > 0 && stItem.AdjustQty != 0)
                {
                    stItem.RowActionStatusFlag = 0;
                }
            }
        }

        private void ClinicDeptStockTakeDetails_Resave()
        {
            this.ShowBusyIndicator();
            CurrentClinicDeptStockTakes.V_MedProductType = V_MedProductType;
            CurrentClinicDeptStockTakes.StockTakingDate = CurrentClinicDeptStockTakes.StockTakingDate.Date.AddHours(23).AddMinutes(59).AddSeconds(59);

            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyMedDeptServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginClinicDeptStockTake_Resave(CurrentClinicDeptStockTakes, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            long ID = 0;
                            string StrError;
                            var results = contract.EndClinicDeptStockTake_Resave(out ID, out StrError, asyncResult);
                            if (string.IsNullOrEmpty(StrError))
                            {
                                MessageBox.Show(eHCMSResources.A0756_G1_Msg_InfoKKOK, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                //load danh sach thuoc theo hoa don 
                                CurrentClinicDeptStockTakes.ClinicDeptStockTakeID = ID;
                                IsReInsert = false;
                                GetLastClinicDeptStockTakes(CurrentClinicDeptStockTakes.StoreID);
                                btnLockStore();
                            }
                            else
                            {
                                MessageBox.Show(StrError, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
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

        private void ClinicDeptStockTakeDetails_Save()
        {
            this.ShowBusyIndicator();
            CurrentClinicDeptStockTakes.V_MedProductType = V_MedProductType;

            CurrentClinicDeptStockTakes.StockTakingDate = CurrentClinicDeptStockTakes.StockTakingDate.Date.AddHours(23).AddMinutes(59).AddSeconds(59);

            CheckFilteringOutRowWithAllZeroValues();

            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            //isLoadingFullOperator = true;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyMedDeptServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginClinicDeptStockTake_Save(CurrentClinicDeptStockTakes, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            long ID = 0;
                            string StrError;
                            var results = contract.EndClinicDeptStockTake_Save(out ID, out StrError, asyncResult);
                            if (string.IsNullOrEmpty(StrError))
                            {
                                MessageBox.Show(eHCMSResources.A0756_G1_Msg_InfoKKOK, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                //load danh sach thuoc theo hoa don 
                                CurrentClinicDeptStockTakes.ClinicDeptStockTakeID = ID;
                                GetLastClinicDeptStockTakes(CurrentClinicDeptStockTakes.StoreID);
                                btnLockStore();
                            }
                            else
                            {
                                MessageBox.Show(StrError, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                            }
                        }
                        catch (Exception ex)
                        {
                            //isLoadingFullOperator = false;
                            MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                        }
                        finally
                        {
                            //isLoadingFullOperator = false;
                            //Globals.IsBusy = false;
                            this.HideBusyIndicator();
                        }

                    }), null);

                }

            });

            t.Start();
        }

        #endregion
        DataGrid GridStockTakes = null;
        public void GridStockTakes_Loaded(object sender, RoutedEventArgs e)
        {
            GridStockTakes = sender as DataGrid;
        }

        DataPager pagerStockTakes = null;
        public void pagerStockTakes_Loaded(object sender, RoutedEventArgs e)
        {
            pagerStockTakes = sender as DataPager;
        }
        public void GridStockTakes_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = e.Row.GetIndex() + 1;
        }

        public void GridStockTakes_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            if (!CheckCaculatedQty())
            {
                e.Cancel = true;
            }
        }

        CheckBox PagingChecked;
        public void Paging_Checked(object sender, RoutedEventArgs e)
        {
            //avtivate datapager
            PagingChecked = sender as CheckBox;
            pagerStockTakes.Source = GridStockTakes.ItemsSource.Cast<object>().ToObservableCollection();
            VisibilityPaging = Visibility.Visible;
        }
        //▼====== #001
        public void Paging_Unchecked(object sender, RoutedEventArgs e)
        {
            //deavtivate datapager
            pagerStockTakes.Source = null;
            VisibilityPaging = Visibility.Collapsed;

            LoadDataGrid();
        }
        private CollectionViewSource _CVS_StoreDeptStockTakeDetails;
        public CollectionViewSource CVS_StoreDeptStockTakeDetails
        {
            get
            {
                return _CVS_StoreDeptStockTakeDetails;
            }
            set
            {
                _CVS_StoreDeptStockTakeDetails = value;
                NotifyOfPropertyChange(() => CVS_StoreDeptStockTakeDetails);
            }
        }

        public CollectionView CV_StoreDeptStockTakeDetails { get; set; }
        private void LoadDataGrid()
        {
            if (ClinicDeptStockTakeDetailList == null)
            {
                ClinicDeptStockTakeDetailList = new ObservableCollection<ClinicDeptStockTakeDetails>();
            }


            CVS_StoreDeptStockTakeDetails = new CollectionViewSource { Source = ClinicDeptStockTakeDetailList };
            CV_StoreDeptStockTakeDetails = (CollectionView)CVS_StoreDeptStockTakeDetails.View;
            NotifyOfPropertyChange(() => CV_StoreDeptStockTakeDetails);

            btnFilter();
        }
        //▲====== #001
        public void cbxPageSize_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((sender as ComboBox) != null && (sender as ComboBox).SelectedItem != null)
            {
                PCVPageSize = Convert.ToInt32(((sender as ComboBox).SelectedItem as ComboBoxItem).Content);
            }
        }

        public void btnFilter()
        {
            //if (PCVClinicDeptStockTakeDetails == null)
            //    return;
            //PCVClinicDeptStockTakeDetails.Filter = null;
            //PCVClinicDeptStockTakeDetails.Filter = new Predicate<object>(DoFilter);
            //▼====== #001
            CV_StoreDeptStockTakeDetails.Filter = null;
            CV_StoreDeptStockTakeDetails.Filter = new Predicate<object>(DoFilter);
            //▲====== #001
        }

        //Callback method
        private bool DoFilter(object o)
        {
            string Code = Globals.FormatCode(V_MedProductType, SearchKey);

            //it is not a case sensitive search
            ClinicDeptStockTakeDetails emp = o as ClinicDeptStockTakeDetails;
            if (emp != null)
            {
                if (string.IsNullOrEmpty(SearchKey))
                {
                    SearchKey = "";
                }
                if (emp.BrandName.ToLower().IndexOf(SearchKey.Trim().ToLower()) >= 0 || emp.Code.ToLower() == Code.Trim().ToLower())
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return false;
        }

        public void SearchKey_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SearchKey = (sender as TextBox).Text;
                btnFilter();
            }
        }

        private bool CheckDataSave()
        {
            if (ClinicDeptStockTakeDetailList == null || ClinicDeptStockTakeDetailList.Count <= 0)
            {
                MessageBox.Show(eHCMSResources.A0646_G1_Msg_InfoKhCoHgDeLuuKK, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return false;
            }

            ObservableCollection<ClinicDeptStockTakeDetails> InvalidProduct = new ObservableCollection<ClinicDeptStockTakeDetails>();
            InvalidProduct = ClinicDeptStockTakeDetailList.Where(x => x.CaculatedQty < 0 || x.ActualQty < 0).ToObservableCollection();

            if (InvalidProduct.Count > 0)
            {
                string strBrandName = "";

                int limitRow = 10;
                int count = 0;

                foreach (ClinicDeptStockTakeDetails temp in InvalidProduct)
                {
                    //strBrandName += temp.BrandName + " (" + temp.Code + ") " + Environment.NewLine;
                    if (count < limitRow)
                    {
                        strBrandName += "\t" + (count + 1).ToString() + ". " + temp.Code + " - " + temp.BrandName + Environment.NewLine;
                        count++;
                    }
                    else
                    {
                        strBrandName += "\t..." + Environment.NewLine;
                        break;
                    }
                }

                MessageBox.Show(string.Format(eHCMSResources.Z1286_G1_I, strBrandName) + Environment.NewLine, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);

                return false;

            }

            return true;
        }

        public void btnSave()
        {
            if (CurrentClinicDeptStockTakes == null)
            {
                return;
            }
            if (CurrentClinicDeptStockTakes.StoreID <= 0)
            {
                MessageBox.Show(eHCMSResources.A0582_G1_Msg_InfoChonKhoKK, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
            if (!CheckDataSave())
            {
                return;
            }

            if (CurrentClinicDeptStockTakes.V_StockTakeType == (long)AllLookupValues.V_StockTakeType.KIEMKE_KETCHUYEN)
            {
                if (MessageBox.Show(string.Format(eHCMSResources.Z1287_G1_I, CurrentClinicDeptStockTakes.StockTakingDate.ToShortDateString()), eHCMSResources.G2363_G1_XNhan, MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                {
                    return;
                }
            }

            //if (MessageBox.Show("Bạn phải kiểm tra kỹ số lượng thực tế trước khi lưu. Nếu đã lưu rồi sẽ không được chỉnh sửa gì nữa.", eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)

            //KMx: Chấp nhận số lượng lý thuyết và thực tế = 0 (26/07/2014 17:11)
            //var temp = ClinicDeptStockTakeDetailList.Where(x => x.CaculatedQty > 0 && x.ActualQty > 0);
            var temp = ClinicDeptStockTakeDetailList.Where(x => x.CaculatedQty >= 0 && x.ActualQty >= 0);
            CurrentClinicDeptStockTakes.StockTakeDetails = temp.ToObservableCollection();
            if (IsReInsert)
            {
                ClinicDeptStockTakeDetails_Resave();
                return;
            }
            ClinicDeptStockTakeDetails_Save();
        }

        /// <summary>
        /// Process for changed store event
        /// VuTTM
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void KeyEnabledComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // VuTTM
            if ((sender as ComboBox).SelectedItem != null)
            {
                long storeId = (long)(sender as ComboBox).SelectedValue;
                GetLastClinicDeptStockTakes(storeId);
                IsLockedClinicWarehouse(storeId);
            }
        }

        /// <summary>
        /// Update status of group button 1 after changed store
        /// </summary>
        public void UpdateGroupBtn1StatusAfterChangedStore()
        {
            CanReGetStockTake = false;
            CanLockStore = false;
            CanUnLockStore = false;
            if (null == CurrentClinicDeptStockTakes)
            {
                return;
            }
            if (null == LastClinicDeptStockTakes
                || CurrentClinicDeptStockTakes.ClinicDeptStockTakeID != LastClinicDeptStockTakes.ClinicDeptStockTakeID)
            {
                return;
            }
            if (CurrentClinicDeptStockTakes.ClinicDeptStockTakeID == LastClinicDeptStockTakes.ClinicDeptStockTakeID)
            {
                CanReGetStockTake = !CanLockStore;
            }
            if (LastClinicDeptStockTakes.IsLocked)
            {
                CanUnLockStore = true;
            }
            else
            {
                CanLockStore = true;
            }
        }

        /// <summary>
        /// Get the last clinic dept stock takes
        /// </summary>
        /// <param name="storeId"></param>
        private void GetLastClinicDeptStockTakes(long storeId)
        {
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyMedDeptServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetLastClinicDeptStockTakes(storeId, V_MedProductType, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndGetLastClinicDeptStockTakes(asyncResult);
                                LastClinicDeptStockTakes = null;
                                if (null != results)
                                {
                                    LastClinicDeptStockTakes = results;
                                    UpdateGroupBtn1StatusAfterChangedStore();
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
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
                    MessageBox.Show(ex.Message, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                }
            });

            t.Start();
        }

        /// <summary>
        /// Global variable: _LastClinicDeptStockTakes
        /// VuTTM
        /// </summary>
        private ClinicDeptStockTakes _LastClinicDeptStockTakes;
        public ClinicDeptStockTakes LastClinicDeptStockTakes
        {
            get
            {
                return _LastClinicDeptStockTakes;
            }
            set
            {
                if (_LastClinicDeptStockTakes != value)
                {
                    _LastClinicDeptStockTakes = value;
                    NotifyOfPropertyChange(() => LastClinicDeptStockTakes);
                }
            }
        }

        KeyEnabledComboBox cbxStore = new KeyEnabledComboBox();
        public void cbxStore_Loaded(object sender, RoutedEventArgs e)
        {
            cbxStore = sender as KeyEnabledComboBox;
        }

        private bool _CanGetStockTake = true;
        public bool CanGetStockTake
        {
            get { return _CanGetStockTake; }
            set
            {
                if (_CanGetStockTake != value)
                    _CanGetStockTake = value;
                NotifyOfPropertyChange(() => CanGetStockTake);
            }
        }

        private bool _CanReGetStockTake = false;
        public bool CanReGetStockTake
        {
            get { return _CanReGetStockTake; }
            set
            {
                if (_CanReGetStockTake != value)
                    _CanReGetStockTake = value;
                NotifyOfPropertyChange(() => CanReGetStockTake);
            }
        }

        private bool _CanLockStore = false;
        public bool CanLockStore
        {
            get { return _CanLockStore; }
            set
            {
                if (_CanLockStore != value)
                    _CanLockStore = value;
                NotifyOfPropertyChange(() => CanLockStore);
            }
        }

        private bool _CanUnLockStore = false;
        public bool CanUnLockStore
        {
            get { return _CanUnLockStore; }
            set
            {
                if (_CanUnLockStore != value)
                    _CanUnLockStore = value;
                NotifyOfPropertyChange(() => CanUnLockStore);
            }
        }

        public void btnGetStockTake()
        {
            //--▼-- 06/02/2021 DatTB
            DateTime _FirstStockTakingDate = Convert.ToDateTime(Globals.ServerConfigSection.CommonItems.NgayNhapLaiTDK.ToString());
            if (null == _FirstStockTakingDate)
            {
                MessageBox.Show("Không thể tính tồn đầu kỳ. Không có thông tin cấu hình ngày đầu tiên thực hiện tính tồn.",
                    eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
            if (CurrentClinicDeptStockTakes.StockTakingDate.Year.CompareTo(Globals.GetCurServerDateTime().Year) != 0)
            {
                MessageBox.Show("Không thể tính tồn đầu kỳ. Ngày thực hiện không hợp lệ.",
                    eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
            //--▲-- 06/02/2021 DatTB
            if (CurrentClinicDeptStockTakes == null)
            {
                MessageBox.Show("Không thể tính tồn đầu kỳ. Vui lòng chọn kho.", eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
            if (String.IsNullOrEmpty(CurrentClinicDeptStockTakes.StockTakePeriodName))
            {
                MessageBox.Show("Không thể tính tồn đầu kỳ. Vui lòng nhập tên phiếu.", eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
            //--▼-- 06/02/2021 DatTB
            if ((null != CurrentClinicDeptStockTakes.StockTakingDate
                    && (CurrentClinicDeptStockTakes.StockTakingDate.DayOfYear >= Globals.GetCurServerDateTime().DayOfYear)
                    && (CurrentClinicDeptStockTakes.StockTakingDate.DayOfYear != _FirstStockTakingDate.DayOfYear))
                || (null != LastClinicDeptStockTakes && null != LastClinicDeptStockTakes.StockTakingDate
                    && (DateTime.Compare(CurrentClinicDeptStockTakes.StockTakingDate, LastClinicDeptStockTakes.StockTakingDate) <= 0)))
            {
                MessageBox.Show("Không thể tính tồn đầu kỳ. Ngày thực hiện phải trước ngày hiện tại và lớn hơn ngày tính tồn gần nhất.",
                    eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
            //--▲-- 06/02/2021 DatTB
            if (CurrentClinicDeptStockTakes.ClinicDeptStockTakeID > 0)
            {
                MessageBox.Show(eHCMSResources.A0600_G1_Msg_InfoLoadKK, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
            if (cbxStore == null || cbxStore.SelectedItem == null || ((RefStorageWarehouseLocation)cbxStore.SelectedItem).StoreID <= 0)
            {
                MessageBox.Show(eHCMSResources.K1973_G1_ChonKho, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }

            CurrentClinicDeptStockTakes.StoreID = ((RefStorageWarehouseLocation)cbxStore.SelectedItem).StoreID;
            ClinicDeptStockTakeDetails_Get(CurrentClinicDeptStockTakes.StoreID);
        }

        public void ReGetStockTake()
        {
            if (CurrentClinicDeptStockTakes == null
                || null == LastClinicDeptStockTakes)
            {
                MessageBox.Show("Không thể tính lại tồn đầu. Vui lòng chọn kho và chọn thời điểm tính tồn gần nhất.",
                    eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
            if (null != LastClinicDeptStockTakes
                && !(LastClinicDeptStockTakes.ClinicDeptStockTakeID == CurrentClinicDeptStockTakes.ClinicDeptStockTakeID
                    && !LastClinicDeptStockTakes.IsLocked))
            {
                MessageBox.Show("Không thể tính lại tồn đầu. Vui lòng mở kho và chọn thời điểm tính tồn gần nhất.",
                    eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
            
            ReGetClinicDeptStockTakeDetails(CurrentClinicDeptStockTakes.StoreID);
        }

        private void IsLockedClinicWarehouse(long storeID)
        {
            if (0 == storeID)
            {
                return;
            }

            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyMedDeptServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginIsLockedClinicWarehouse(storeID, V_MedProductType, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            IsLockStore = contract.EndIsLockedClinicWarehouse(asyncResult);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
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

        private void ClinicDeptLockAndUnlockStore(long StoreID, bool IsLock)
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyMedDeptServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginClinicDeptLockAndUnlockStore(StoreID, V_MedProductType, Globals.LoggedUserAccount.Staff.StaffID, IsLock, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            bool results = contract.EndClinicDeptLockAndUnlockStore(out string msg, asyncResult);

                            if (results)
                            {
                                if (string.IsNullOrEmpty(msg))
                                {
                                    MessageBox.Show(eHCMSResources.A0620_G1_Msg_InfoKhoaKhoOK, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                }

                                if (IsLock)
                                {
                                    MessageBox.Show(eHCMSResources.A0620_G1_Msg_InfoKhoaKhoOK, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                }
                                else
                                {
                                    MessageBox.Show(eHCMSResources.A0819_G1_Msg_InfoMoKhoaKhoOK, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                }
                                GetLastClinicDeptStockTakes(StoreID);
                                IsLockedClinicWarehouse(StoreID);
                            }
                            else
                            {
                                if (IsLock)
                                {
                                    MessageBox.Show("Khóa kho thất bại", eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                }
                                else
                                {
                                    MessageBox.Show(eHCMSResources.A0820_G1_Msg_InfoMoKhoaKhoFail, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
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

        /// <summary>
        /// Implement lock store
        /// VuTTM
        /// </summary>
        public void btnLockStore()
        {
            if (mKiemKe_KhoaTatCa && IsLockAll)
            {
                ClinicDeptLockAndUnlockAllStore(true);
                return;
            }

            if (CurrentClinicDeptStockTakes == null)
            {
                MessageBox.Show("Không thể khóa kho. Vui lòng chọn thời điểm tính tồn gần nhất.", eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
            if (CurrentClinicDeptStockTakes.StoreID <= 0)
            {
                MessageBox.Show("Không thể khóa kho. Hãy chọn kho.", eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
            // VuTTM
            //if (null != LastClinicDeptStockTakes
            //    && (CurrentClinicDeptStockTakes.ClinicDeptStockTakeID < LastClinicDeptStockTakes.ClinicDeptStockTakeID)
            //    && !LastClinicDeptStockTakes.IsLocked)
            //{
            //    MessageBox.Show("Không thể khóa kho. Vui lòng chọn thời điểm tính tồn gần nhất.", eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
            //    return;
            //}
                
            ClinicDeptLockAndUnlockStore(CurrentClinicDeptStockTakes.StoreID, true);
        }

        /// <summary>
        /// Implement unlock store
        /// VuTTM
        /// </summary>
        public void btnUnlockStore()
        {
            //if (CurrentClinicDeptStockTakes == null)
            //{
            //    MessageBox.Show("Không thể mở kho. Vui lòng chọn thời điểm tính tồn gần nhất.", eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
            //    return;
            //}
            if (null == CurrentClinicDeptStockTakes
                || CurrentClinicDeptStockTakes.StoreID <= 0)
            {
                MessageBox.Show("Không thể mở kho. Hãy chọn kho.", eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
            //if (null != LastClinicDeptStockTakes
            //    && (CurrentClinicDeptStockTakes.ClinicDeptStockTakeID < LastClinicDeptStockTakes.ClinicDeptStockTakeID)
            //    && LastClinicDeptStockTakes.IsLocked)
            //{
            //    MessageBox.Show("Không thể mở kho. Vui lòng chọn thời điểm tính tồn gần nhất.", eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
            //    return;
            //}

            if (IsLockAll)
            {
                ClinicDeptLockAndUnlockAllStore(false);
            }
            else
            {
                ClinicDeptLockAndUnlockStore(CurrentClinicDeptStockTakes.StoreID, false);
            }
        }

        public void Search_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                btnSearch();
            }
        }

        public void btnSearch()
        {
            if (CurrentClinicDeptStockTakes == null)
                return;
            // TxD 04/04/2015 : Added StoreID to Search Criteria
            if (CurrentClinicDeptStockTakes.StoreID <= 0)
            {
                MessageBox.Show("Hãy chọn kho cần tìm kiếm.", eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }

            ClinicDeptStockTakes_Search(0, Globals.PageSize);
        }

        public void btnNew()
        {
            CurrentClinicDeptStockTakes.ClinicDeptStockTakeID = 0;
            CurrentClinicDeptStockTakes.StockTakeNotes = "";
            CurrentClinicDeptStockTakes.StockTakePeriodName = "";
            CurrentClinicDeptStockTakes.StockTakingDate = Globals.GetCurServerDateTime();
            CurrentClinicDeptStockTakes.StoreID = StoreCbx.FirstOrDefault().StoreID;
            CurrentClinicDeptStockTakes.StockTakeDetails = null;
            ClinicDeptStockTakeDetailList = null;

            //▼====== #001: không thể xét null cho collectionviewsource mà phải new. 
            //PCVClinicDeptStockTakeDetails = null;
            ClinicDeptStockTakeDetailList = new ObservableCollection<ClinicDeptStockTakeDetails>();
            CVS_StoreDeptStockTakeDetails = new CollectionViewSource { Source = ClinicDeptStockTakeDetailList };
            CV_StoreDeptStockTakeDetails = (CollectionView)CVS_StoreDeptStockTakeDetails.View;
            NotifyOfPropertyChange(() => CV_StoreDeptStockTakeDetails);
            //▲====== #001
            SelectedProductStockTake = null;
            ClinicDeptStockTakeDetailList_Hide = null;
            CanGetStockTake = true;
            CanReGetStockTake = false;
            CanLockStore = false;
            CanUnLockStore = false;
            IsReInsert = false; 
            UnCheckPaging();
            IsLockAll = false;
            IsLockStore = false;
            IsUnLockStore = false;
        }

        public void btnExportExcel()
        {
            if (CurrentClinicDeptStockTakes == null || CurrentClinicDeptStockTakes.StoreID <= 0)
            {
                MessageBox.Show(eHCMSResources.K0327_G1_ChonKhoHoacPhKKDeXuatExcel, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }

            SaveFileDialog objSFD = new SaveFileDialog()
            {
                DefaultExt = ".xls",
                Filter = "Excel (2003) (*.xls)|*.xls|Excel (2010) (.xlsx)|*.xlsx",
                //Filter = "Excel (2003)(.xls)|*.xls|Excel (2010) (.xlsx)|*.xlsx |RichText File (.rtf)|*.rtf |Pdf File (.pdf)|*.pdf |Html File (.html)|*.html",
                FilterIndex = 1
            };
            if (objSFD.ShowDialog() != true)
            {
                return;
            }

            ReportParameters RptParameters = new ReportParameters();
            RptParameters.ReportType = ReportType.KIEM_KE;
            RptParameters.StoreID = CurrentClinicDeptStockTakes.StoreID;
            RptParameters.StockTake = new StockTake()
            {
                StockTakeID = CurrentClinicDeptStockTakes.ClinicDeptStockTakeID,
                StockTakeType = StockTakeType.KIEM_KE_KHO_NOI_TRU,
                V_MedProductType = V_MedProductType,
                StockTakeDate = CurrentClinicDeptStockTakes.StockTakingDate
            };
            RptParameters.Show = "KiemKe";

            ExportToExcelGeneric.Action(RptParameters, objSFD, this);
            //Coroutine.BeginExecute(DoSaveExcel(RptParameters, objSFD));
        }


        //▼======20210112 QTD Thêm nút Import từ file Excell
        public byte[] ReadAllBytes(string fileName)
        {
            byte[] buffer = null;
            try
            {
                using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
                {
                    buffer = new byte[fs.Length];
                    fs.Read(buffer, 0, (int)fs.Length);
                }
            }
            catch
            {
                MessageBox.Show("File Excel đang được sử dụng, vui lòng đóng trước khi nhập!");
            }
            return buffer;
        }

        public void btnImportFromExcell()
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "XLS files (*.xls)|*.xls|XLSX files (*.xlsx)|*.xlsx";
            openFileDialog.FilterIndex = 2;
            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;
                byte[] file = ReadAllBytes(filePath);
                if(file != null)
                {
                    ImportFromExcell(file);
                }
                return;
            }
        }

        private void ImportFromExcell(byte[] file)
        {
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            try
            {
                using (MemoryStream ms = new MemoryStream(file))
                {
                    try
                    {
                        List<ClinicDeptStockTakeDetails> invoicedrug = new List<ClinicDeptStockTakeDetails>();
                        using (ExcelPackage package = new ExcelPackage(ms))
                        {
                            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                            ExcelWorksheet workSheet = package.Workbook.Worksheets.FirstOrDefault();
                            int startRow = workSheet.Dimension.Start.Row + 1;
                            int endRow = workSheet.Dimension.End.Row;
                            if (workSheet.Dimension.Columns != 8)
                            {
                                MessageBox.Show("File excel không đúng định dạng");
                                return;
                            }
                            else
                            {                                
                                for (int i = startRow; i <= endRow; i++)
                                {
                                    ClinicDeptStockTakeDetails drugs = new ClinicDeptStockTakeDetails();
                                    int j = 1;
                                    try
                                    {
                                        drugs.GenMedProductID = Convert.ToInt32(workSheet.Cells[i, j++].Value);
                                        drugs.Code = workSheet.Cells[i, j++].Value.ToString();
                                        drugs.BrandName = workSheet.Cells[i, j++].Value.ToString();
                                        drugs.UnitName = workSheet.Cells[i, j++].Value.ToString();
                                        drugs.CaculatedQty = Convert.ToInt32(workSheet.Cells[i, j++].Value);
                                        drugs.ActualQty = Convert.ToInt32(workSheet.Cells[i, j++].Value);
                                        drugs.AdjustQty = Convert.ToInt32(workSheet.Cells[i, j++].Value);
                                        drugs.FinalAmount = Convert.ToDecimal(workSheet.Cells[i, j++].Value);
                                        invoicedrug.Add(drugs);
                                    }
                                    catch (Exception ex)
                                    {
                                        MessageBox.Show("Giá trị tại dòng " + i + ", Cột " + j + " trong file Excel không đúng định dạng!");
                                        return;
                                    }
                                }
                                ClinicDeptStockTakeDetailList = invoicedrug.ToObservableCollection();
                                CheckCaculatedQty();
                                if (null != ClinicDeptStockTakeDetailList)
                                {
                                    CurrentClinicDeptStockTakes.ClinicDeptStockTakeID = 0;
                                }
                                if (ClinicDeptStockTakeDetailList == null)
                                {
                                    MessageBox.Show("Chưa có dữ liệu");
                                }
                                GetLastClinicDeptStockTakes(CurrentClinicDeptStockTakes.StoreID);
                                LoadDataGrid();
                                CanGetStockTake = false;
                                mKiemKe_ThemMoi = true;
                                IsReInsert = true; //Cho phép tính lại

                                CanGetStockTake = false;
                                CanLockStore = false;
                                CanUnLockStore = false;
                                CanReGetStockTake = false;
                            }
                        }
                        
                    }
                    catch (Exception ex)
                    {
                        Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        IsReInsert = true;
                    }
                    finally
                    {
                        this.HideBusyIndicator();
                    }
                }
            }
            catch (Exception ex)
            {
                this.HideBusyIndicator();
                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
            }
        }
        //▲======

        //private IEnumerator<IResult> DoSaveExcel(ReportParameters rptParameters, SaveFileDialog objSFD)
        //{
        //    var res = new ExportToExcellAllGenericTask(rptParameters, objSFD);
        //    yield return res;
        //    //IsProcessing = false;
        //    yield break;
        //}

        #region IHandle<DrugDeptCloseSearchStockTakesEvent> Members

        public void Handle(ClinicDeptCloseSearchStockTakesEvent message)
        {
            if (message != null && this.IsActive)
            {
                CurrentClinicDeptStockTakes = message.SelectedClinicDeptStockTakes as ClinicDeptStockTakes;
                UnCheckPaging();
                //load detail
                ClinicDeptStockTakeDetails_Load(CurrentClinicDeptStockTakes.ClinicDeptStockTakeID);
            }
        }

        #endregion

        #region printing member

        public void btnPreview()
        {
            Action<IClinicDeptReportDocumentPreview> onInitDlg = delegate (IClinicDeptReportDocumentPreview proAlloc)
            {
                proAlloc.ID = CurrentClinicDeptStockTakes.ClinicDeptStockTakeID;
                proAlloc.V_MedProductType = V_MedProductType;
                proAlloc.eItem = ReportName.CLINICDEPT_KIEMKE_KHOAPHONG;
            };
            GlobalsNAV.ShowDialog<IClinicDeptReportDocumentPreview>(onInitDlg);
        }

        public void btnPrint()
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            //isLoadingSearch = true;
            //var t = new Thread(() =>
            //{
            //    using (var serviceFactory = new ReportServiceClient())
            //    {
            //        var contract = serviceFactory.ServiceInstance;

            //        contract.BeginGetPhieuKiemKeInPdfFormat(CurrentClinicDeptStockTakes.ClinicDeptStockTakeID, Globals.DispatchCallback((asyncResult) =>
            //        {
            //            try
            //            {
            //                var results = contract.EndGetPhieuKiemKeInPdfFormat(asyncResult);
            //                var printEvt = new ActiveXPrintEvt(this, PrinterType.IN_PHIEU, results, ActiveXPrintType.ByteArray);
            //                Globals.EventAggregator.Publish(results);
            //            }
            //            catch (Exception ex)
            //            {
            //                isLoadingSearch = false;
            //                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
            //            }
            //            finally
            //            {
            //                isLoadingSearch = false;
            //                Globals.IsBusy = false;
            //            }

            //        }), null);

            //    }

            //});
            //t.Start();
        }

        #endregion

        #region checking acount
        private bool _mKiemKe_Tim = true;
        private bool _mKiemKe_ThemMoi = true;
        private bool _mKiemKe_XuatExcel = true;
        private bool _mKiemKe_XemIn = true;
        private bool _IsReInsert = false;
        public bool IsReInsert
        {
            get
            {
                return _IsReInsert;
            }
            set
            {
                if (_IsReInsert == value)
                    return;
                _IsReInsert = value;
                NotifyOfPropertyChange(() => IsReInsert);
            }
        }

        public bool mKiemKe_Tim
        {
            get
            {
                return _mKiemKe_Tim;
            }
            set
            {
                if (_mKiemKe_Tim == value)
                    return;
                _mKiemKe_Tim = value;
                NotifyOfPropertyChange(() => mKiemKe_Tim);
            }
        }

        public bool mKiemKe_ThemMoi
        {
            get
            {
                return _mKiemKe_ThemMoi;
            }
            set
            {
                if (_mKiemKe_ThemMoi == value)
                    return;
                _mKiemKe_ThemMoi = value;
                NotifyOfPropertyChange(() => mKiemKe_ThemMoi);
            }
        }

        public bool mKiemKe_XuatExcel
        {
            get
            {
                return _mKiemKe_XuatExcel;
            }
            set
            {
                if (_mKiemKe_XuatExcel == value)
                    return;
                _mKiemKe_XuatExcel = value;
                NotifyOfPropertyChange(() => mKiemKe_XuatExcel);
            }
        }

        public bool mKiemKe_XemIn
        {
            get
            {
                return _mKiemKe_XemIn;
            }
            set
            {
                if (_mKiemKe_XemIn == value)
                    return;
                _mKiemKe_XemIn = value;
                NotifyOfPropertyChange(() => mKiemKe_XemIn);
            }
        }

        #endregion

        //KMx: Thêm AutoComplete tìm thuốc (26/02/2015 16:30).
        #region AutoComplete Search Drug

        private bool _VisibilityName = true;
        public bool VisibilityName
        {
            get
            {
                return _VisibilityName;
            }
            set
            {
                _VisibilityName = value;
                _VisibilityCode = !_VisibilityName;
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

        TextBox tbx = null;
        public void AxTextBox_Loaded(object sender, RoutedEventArgs e)
        {
            tbx = sender as TextBox;
        }

        string txt = "";
        public void AxTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (CanGetStockTake)
            {
                MessageBox.Show("Hãy chọn bắt đầu kiểm kê!", eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);

                SelectedProductStockTake = null;

                return;
            }
            if (txt != (sender as TextBox).Text)
            {
                txt = (sender as TextBox).Text;

                if (!string.IsNullOrEmpty(txt))
                {
                    string Code = Globals.FormatCode(V_MedProductType, txt);
                    SearchGetDrugForStockTake(Code, true);
                }
            }
        }

        AxTextBox AxQty = null;
        public void Quantity_Loaded(object sender, RoutedEventArgs e)
        {
            AxQty = sender as AxTextBox;
        }

        AutoCompleteBox au;
        public void AutoDrug_Text_Loaded(object sender, RoutedEventArgs e)
        {
            au = sender as AutoCompleteBox;
        }

        public void AutoDrug_Text_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            if (au.SelectedItem != null)
            {
                SelectedProductStockTake = au.SelectedItem as ClinicDeptStockTakeDetails;
            }
        }

        public void AutoCompleteBox_Populating(object sender, PopulatingEventArgs e)
        {
            if (CanGetStockTake)
            {
                MessageBox.Show("Hãy chọn bắt đầu kiểm kê!", eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);

                SelectedProductStockTake = null;

                return;
            }
            if (!IsCode.GetValueOrDefault())
            {
                BrandName = e.Parameter;
                SearchGetDrugForStockTake(e.Parameter, false);
            }
        }


        private void ListDisplayAutoComplete()
        {
            if (IsCode.GetValueOrDefault())
            {
                if (SearchProductList != null && SearchProductList.Count > 0)
                {
                    SelectedProductStockTake = SearchProductList.FirstOrDefault();
                }
                else
                {
                    SelectedProductStockTake = null;

                    MessageBox.Show(eHCMSResources.A0752_G1_Msg_InfoKhTimThay);
                }
            }
            else
            {
                if (au != null)
                {
                    au.ItemsSource = SearchProductList;
                    au.PopulateComplete();
                }
            }
        }

        private bool? IsCode = false;
        private string BrandName;

        private ObservableCollection<ClinicDeptStockTakeDetails> _searchProductList;
        public ObservableCollection<ClinicDeptStockTakeDetails> SearchProductList
        {
            get { return _searchProductList; }
            set
            {
                if (_searchProductList != value)
                    _searchProductList = value;
                NotifyOfPropertyChange(() => SearchProductList);
            }
        }

        private ClinicDeptStockTakeDetails _selectedProductStockTake;
        public ClinicDeptStockTakeDetails SelectedProductStockTake
        {
            get { return _selectedProductStockTake; }
            set
            {
                if (_selectedProductStockTake != value)
                    _selectedProductStockTake = value;
                NotifyOfPropertyChange(() => SelectedProductStockTake);
            }
        }


        private void CopyQty()
        {
            if (SearchProductList == null || SearchProductList.Count <= 0 || ClinicDeptStockTakeDetailList_Hide == null || ClinicDeptStockTakeDetailList_Hide.Count <= 0)
            {
                return;
            }

            foreach (ClinicDeptStockTakeDetails item in SearchProductList)
            {
                ClinicDeptStockTakeDetails StockTakeDetail = ClinicDeptStockTakeDetailList_Hide.Where(x => x.GenMedProductID == item.GenMedProductID).FirstOrDefault();

                if (StockTakeDetail == null)
                {
                    continue;
                }

                item.CaculatedQty = StockTakeDetail.CaculatedQty;
                item.ActualQty = StockTakeDetail.ActualQty;
            }
        }

        private void SearchGetDrugForStockTake(string Name, bool IsCode)
        {

            if (CurrentClinicDeptStockTakes == null)
            {
                return;
            }

            if (CurrentClinicDeptStockTakes.StoreID <= 0)
            {
                MessageBox.Show(eHCMSResources.K0322_G1_ChonKhoDeTimHgCanKK, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyMedDeptServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetProductForClinicDeptStockTake(Name, CurrentClinicDeptStockTakes.StoreID, IsCode, V_MedProductType, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndGetProductForClinicDeptStockTake(asyncResult);
                            SearchProductList = results.ToObservableCollection();

                            CopyQty();

                            ListDisplayAutoComplete();

                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            if (IsCode)
                            {
                                if (AxQty != null)
                                {
                                    AxQty.Focus();
                                }
                            }
                            else
                            {
                                if (au != null)
                                {
                                    au.Focus();
                                }
                            }
                        }

                    }), null);

                }

            });

            t.Start();
        }

        public void AddItem()
        {
            if (CurrentClinicDeptStockTakes == null || ClinicDeptStockTakeDetailList == null || SelectedProductStockTake == null)
            {
                return;
            }

            if (CurrentClinicDeptStockTakes.ClinicDeptStockTakeID > 0)
            {
                MessageBox.Show("Phiếu kiểm kê này đã lưu rồi, không thể thêm hàng được nữa.", eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }

            if (ClinicDeptStockTakeDetailList != null && ClinicDeptStockTakeDetailList.Where(x => x.GenMedProductID == SelectedProductStockTake.GenMedProductID).Count() > 0)
            {
                MessageBox.Show("Loại hàng \"" + SelectedProductStockTake.BrandName + "\" đã có trong danh sách kiểm kê rồi nên không thể thêm nữa.", eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }

            if (SelectedProductStockTake.CaculatedQty < 0)
            {
                MessageBox.Show("Hàng này có số lượng lý thuyết < 0 nên không thể thêm. Hãy điều chỉnh ngày nhập trên phiếu nhập hoặc ngày xuất trên phiếu xuất có liên quan đến mặt hàng này để số lượng lý thuyết không < 0.", eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }

            if (SelectedProductStockTake.ActualQty < 0)
            {
                MessageBox.Show("Hàng này có số lượng thực tế < 0 nên không thể thêm.", eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }

            ClinicDeptStockTakeDetailList.Add(SelectedProductStockTake);

            txt = "";
            SelectedProductStockTake = null;
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
        #endregion


        private ObservableCollection<Lookup> _V_StockTakeTypeList;
        public ObservableCollection<Lookup> V_StockTakeTypeList
        {
            get { return _V_StockTakeTypeList; }
            set
            {
                if (_V_StockTakeTypeList != value)
                {
                    _V_StockTakeTypeList = value;
                    NotifyOfPropertyChange(() => V_StockTakeTypeList);
                }
            }
        }

        private void SetDefaultStockTakeType()
        {
            if (V_StockTakeTypeList != null && CurrentClinicDeptStockTakes != null)
            {
                CurrentClinicDeptStockTakes.V_StockTakeType = V_StockTakeTypeList.FirstOrDefault().LookupID;
            }
        }

        //▼====== #002
        private bool _mKiemKe_MoKho = true;
        private bool _mKiemKe_KhoaKho = true;
        private bool _mKiemKe_KhoaTatCa = true;

        public bool mKiemKe_MoKho
        {
            get
            {
                return _mKiemKe_MoKho;
            }
            set
            {
                if (_mKiemKe_MoKho == value)
                    return;
                _mKiemKe_MoKho = value;
                NotifyOfPropertyChange(() => mKiemKe_MoKho);
            }
        }


        public bool mKiemKe_KhoaKho
        {
            get
            {
                return _mKiemKe_KhoaKho;
            }
            set
            {
                if (_mKiemKe_KhoaKho == value)
                    return;
                _mKiemKe_KhoaKho = value;
                NotifyOfPropertyChange(() => mKiemKe_KhoaKho);
            }
        }


        public bool mKiemKe_KhoaTatCa
        {
            get
            {
                return _mKiemKe_KhoaTatCa;
            }
            set
            {
                if (_mKiemKe_KhoaTatCa == value)
                    return;
                _mKiemKe_KhoaTatCa = value;
                NotifyOfPropertyChange(() => mKiemKe_KhoaTatCa);
            }
        }

        private bool _IsLockAll = false;
        public bool IsLockAll
        {
            get
            {
                return _IsLockAll;
            }
            set
            {
                if (_IsLockAll != value)
                {
                    _IsLockAll = value;                  
                    NotifyOfPropertyChange(() => IsLockAll);
                    NotifyOfPropertyChange(() => IsEnableCbxStore);
                }
            }
        }

        private bool _IsLockStore = false;
        public bool IsLockStore
        {
            get
            {
                return _IsLockStore;
            }
            set
            {
                if (_IsLockStore != value)
                {
                    _IsLockStore = value;
                    NotifyOfPropertyChange(() => IsLockStore);
                }
            }
        }

        private bool _IsUnLockStore = false;
        public bool IsUnLockStore
        {
            get
            {
                return _IsUnLockStore;
            }
            set
            {
                if (_IsUnLockStore != value)
                {
                    _IsUnLockStore = value;
                    NotifyOfPropertyChange(() => IsUnLockStore);
                }
            }
        }

        public bool IsEnableCbxStore
        {
            get { return !IsLockAll && CanGetStockTake; }          
        }

        private void ClinicDeptLockAndUnlockAllStore(bool IsLock)
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyMedDeptServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginClinicDeptLockAndUnlockStore(0, V_MedProductType, Globals.LoggedUserAccount.Staff.StaffID, IsLock, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            bool results = contract.EndClinicDeptLockAndUnlockStore(out string msg, asyncResult);
                            if (results)
                            {
                                if (string.IsNullOrEmpty(msg))
                                {
                                    MessageBox.Show(eHCMSResources.A0620_G1_Msg_InfoKhoaKhoOK, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                }

                                if (IsLock)
                                {
                                    MessageBox.Show(eHCMSResources.A0620_G1_Msg_InfoKhoaKhoOK, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                }
                                else
                                {
                                    MessageBox.Show(eHCMSResources.A0819_G1_Msg_InfoMoKhoaKhoOK, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                }
                                if (null != CurrentClinicDeptStockTakes)
                                {
                                    IsLockedClinicWarehouse(CurrentClinicDeptStockTakes.StoreID);
                                }
                            }
                            else
                            {
                                if (IsLock)
                                {
                                    MessageBox.Show("Khóa kho thất bại", eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                }
                                else
                                {
                                    MessageBox.Show(eHCMSResources.A0820_G1_Msg_InfoMoKhoaKhoFail, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
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
    }
}
