using System.ComponentModel.Composition;
using aEMR.ViewContracts;
using Caliburn.Micro;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System;
using System.IO;
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
using eHCMSLanguage;
using Microsoft.Win32;
using Castle.Windsor;
using System.Windows.Data;
using OfficeOpenXml;
/*
* 20181006 #001 TTM:   Thay đổi cách lấy giá trị cột từ e.Column.GetValue(FrameworkElement.NameProperty).ToString()
*                      => e.Column.Equals("Tên Grid".GetColumnByName("Tên cột") vì cách cũ bị sai => Lấy giá trị cột không đc. 
* 20181008 #002 TTM: Chuyển từ PageCollectionView => CollectionViewSource. Do WPF ko sử dụng đc PagedCollectionView (hoặc do mình không biết xài PagedCollectionView).
*/

/*--▼--09/01/2021 DatTB

//--001 Dùng GetLastDrugDeptStockTakes() lấy phiếu mới nhất để cập nhật Button thay vì gán biến trực tiếp.
//--001 Vì khi tính lại lấy phiếu mới nhất để so sánh.

//--002 Khi chọn "Tính Lại Tồn" Disable hết các nút Mở/khóa, Tính tồn 
//--002 => Vì lúc này phiếu Tính lại có ID = 0 (Phiếu mới), hạn chế thao tác sai.

--▲--09/01/2021 DatTB*/

/* 
* 20211103 #003 QTD: Lọc kho theo cấu hình trách nhiệm
*/
namespace eHCMS.DrugDept.ViewModels
{
    [Export(typeof(IDrugDeptStockTakes)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class DrugDeptStockTakesViewModel : Conductor<object>, IDrugDeptStockTakes
        , IHandle<DrugDeptCloseSearchStockTakesEvent>
    {
        [ImportingConstructor]
        public DrugDeptStockTakesViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg)
        {
            CurrentDrugDeptStockTakes = new DrugDeptStockTakes();
            SearchCriteria = new DrugDeptStockTakesSearchCriteria();
            GetStaffLogin();
            UnCheckPaging();

            V_StockTakeTypeList = Globals.AllLookupValueList.Where(x => x.ObjectTypeID == (long)LookupValues.V_StockTakeType).ToObservableCollection();
            SetDefaultStockTakeType();

        }

        protected override void OnActivate()
        {
            base.OnActivate();
            Globals.EventAggregator.Subscribe(this);
            Coroutine.BeginExecute(DoGetStore_DrugDept());
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

        private DrugDeptStockTakes _CurrentDrugDeptStockTakes;
        public DrugDeptStockTakes CurrentDrugDeptStockTakes
        {
            get
            {
                return _CurrentDrugDeptStockTakes;
            }
            set
            {
                if (_CurrentDrugDeptStockTakes != value)
                {
                    _CurrentDrugDeptStockTakes = value;
                    NotifyOfPropertyChange(() => CurrentDrugDeptStockTakes);
                }
            }
        }



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

        private ObservableCollection<DrugDeptStockTakeDetails> DrugDeptStockTakeDetailList;

        private ObservableCollection<DrugDeptStockTakeDetails> DrugDeptStockTakeDetailList_Hide;

        private DrugDeptStockTakesSearchCriteria _SearchCriteria;
        public DrugDeptStockTakesSearchCriteria SearchCriteria
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
            if (CurrentDrugDeptStockTakes != null)
            {
                CurrentDrugDeptStockTakes.StaffID = Globals.LoggedUserAccount.Staff.StaffID;
                CurrentDrugDeptStockTakes.FullName = Globals.LoggedUserAccount.Staff.FullName;
            }
            return Globals.LoggedUserAccount.Staff;
        }

        #endregion

        #region Check Invisible

        private bool _mTim = true;
        private bool _mThemMoi = true;
        private bool _mXemIn = true;
        private bool _mXuatExcel = true;

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
        public bool mXemIn
        {
            get
            {
                return _mXemIn;
            }
            set
            {
                if (_mXemIn == value)
                    return;
                _mXemIn = value;
                NotifyOfPropertyChange(() => mXemIn);
            }
        }
        public bool mXuatExcel
        {
            get
            {
                return _mXuatExcel;
            }
            set
            {
                if (_mXuatExcel == value)
                    return;
                _mXuatExcel = value;
                NotifyOfPropertyChange(() => mXuatExcel);
            }
        }
        #endregion

        #region Function Member

        private IEnumerator<IResult> DoGetStore_DrugDept()
        {
            var paymentTypeTask = new LoadStoreListTask((long)AllLookupValues.StoreType.STORAGE_DRUGDEPT, false, null, false, false);
            yield return paymentTypeTask;
            //StoreCbx = paymentTypeTask.LookupList.Where(x => (V_MedProductType != 0 && x.ListV_MedProductType != null && x.ListV_MedProductType.Contains(V_MedProductType.ToString()))).ToObservableCollection();
            //▼===== #003
            var StoreTemp = paymentTypeTask.LookupList.Where(x => (V_MedProductType != 0 && x.ListV_MedProductType != null && x.ListV_MedProductType.Contains(V_MedProductType.ToString()))).ToObservableCollection();
            StoreCbx = Globals.CheckDrugMedStoreWareHouse(StoreTemp);
            if(StoreCbx == null || StoreCbx.Count == 0)
            {
                MessageBox.Show(eHCMSResources.A0110_G1_Msg_InfoChuaCauHinhTNKho.Replace("\\n ", "\n"));
            }
            //▲===== #003
            yield break;
        }

        private void DrugDeptStockTakes_Search(int PageIndex, int PageSize)
        {
            if (SearchCriteria == null)
            {
                SearchCriteria = new DrugDeptStockTakesSearchCriteria();
            }
            SearchCriteria.V_MedProductType = V_MedProductType;

            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyMedDeptServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginDrugDeptStockTakes_Search(SearchCriteria, PageIndex, PageSize, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                int TotalCount = 0;
                                var results = contract.EndDrugDeptStockTakes_Search(out TotalCount, asyncResult);
                                if (results != null && results.Count > 0)
                                {
                                    if (results.Count > 1)
                                    {
                                        //mo pop up tim
                                        void onInitDlg(IDrugDeptStockTakesSearch proAlloc)
                                        {
                                            proAlloc.SearchCriteria = SearchCriteria.DeepCopy();
                                            proAlloc.V_MedProductType = V_MedProductType;
                                            if (V_MedProductType == (long)AllLookupValues.MedProductType.THUOC)
                                            {
                                                proAlloc.strHienThi = eHCMSResources.Z1305_G1_TimPhKKeThuoc;
                                            }
                                            else if (V_MedProductType == (long)AllLookupValues.MedProductType.Y_CU)
                                            {
                                                proAlloc.strHienThi = eHCMSResources.Z1306_G1_TimPhKKeYCu;
                                            }
                                            else if (V_MedProductType == (long)AllLookupValues.MedProductType.NUTRITION)
                                            {
                                                proAlloc.strHienThi = eHCMSResources.Z3224_G1_TimPhKKeDDuong;
                                            }
                                            else if (V_MedProductType == (long)AllLookupValues.MedProductType.VTYT_TIEUHAO)
                                            {
                                                proAlloc.strHienThi = eHCMSResources.Z2611_G1_TimPhKKeVTYTTH;
                                            }
                                            else if (V_MedProductType == (long)AllLookupValues.MedProductType.TIEM_NGUA)
                                            {
                                                proAlloc.strHienThi = eHCMSResources.Z2612_G1_TimPhKKeVaccine;
                                            }
                                            else if (V_MedProductType == (long)AllLookupValues.MedProductType.MAU)
                                            {
                                                proAlloc.strHienThi = eHCMSResources.Z2613_G1_TimPhKKeMau;
                                            }
                                            else if (V_MedProductType == (long)AllLookupValues.MedProductType.VAN_PHONG_PHAM)
                                            {
                                                proAlloc.strHienThi = eHCMSResources.Z2614_G1_TimPhKKeVPP;
                                            }
                                            else if (V_MedProductType == (long)AllLookupValues.MedProductType.VTYT_TIEUHAO)
                                            {
                                                proAlloc.strHienThi = eHCMSResources.Z2615_G1_TimPhKKeVTTH;
                                            }
                                            else if (V_MedProductType == (long)AllLookupValues.MedProductType.THANHTRUNG)
                                            {
                                                proAlloc.strHienThi = eHCMSResources.Z2616_G1_TimPhKKeThanhTrung;
                                            }
                                            else
                                            {
                                                proAlloc.strHienThi = eHCMSResources.Z1307_G1_TimPhKKeHChat;
                                            }
                                            proAlloc.DrugDeptStockTakeList.Clear();
                                            proAlloc.DrugDeptStockTakeList.TotalItemCount = TotalCount;
                                            proAlloc.DrugDeptStockTakeList.PageIndex = 0;
                                            foreach (DrugDeptStockTakes p in results)
                                            {
                                                proAlloc.DrugDeptStockTakeList.Add(p);
                                            }
                                        }
                                        GlobalsNAV.ShowDialog<IDrugDeptStockTakesSearch>(onInitDlg);
                                    }
                                    else
                                    {
                                        // ChangeValue(CurrentDrugDeptStockTakes.StoreID, results.FirstOrDefault().StoreID);
                                        CurrentDrugDeptStockTakes = results.FirstOrDefault();
                                        //load detail
                                        UnCheckPaging();
                                        DrugDeptStockTakeDetails_Load(CurrentDrugDeptStockTakes.DrugDeptStockTakeID);
                                    }
                                    GetLastDrugDeptStockTakes(CurrentDrugDeptStockTakes.StoreID);
                                }
                                else
                                {
                                    Globals.ShowMessage(eHCMSResources.Z0863_G1_KgTimThay, eHCMSResources.G0442_G1_TBao);
                                }
                                UpdateGroupBtn1StatusAfterChangedStore();
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

        public void txtSearch_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                //if (SearchCriteria != null)
                //{
                //    try
                //    {
                //        SearchCriteria.DrugDeptStockTakeID = Convert.ToInt64((sender as TextBox).Text);
                //    }
                //    catch { SearchCriteria.DrugDeptStockTakeID = null; }
                //}
                btnSearch();
            }
        }

        private void UnCheckPaging()
        {
            if (PagingChecked != null && DrugDeptStockTakeDetailList != null)
            {
                PagingChecked.IsChecked = false;
                VisibilityPaging = Visibility.Collapsed;
            }
        }

        private void DrugDeptStockTakeDetails_Load(long DrugDeptStockTakeID)
        {
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyMedDeptServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginDrugDeptStockTakeDetails_Load(DrugDeptStockTakeID, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndDrugDeptStockTakeDetails_Load(asyncResult);
                                //load danh sach thuoc theo hoa don 
                                DrugDeptStockTakeDetailList = results.ToObservableCollection();
                                LoadDataGrid();
                                Count_DifferenceValueInventory();
                                CanGetStockTake = false;
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

        public bool CheckCaculatedQty()
        {
            if (DrugDeptStockTakeDetailList == null || DrugDeptStockTakeDetailList.Count <= 0)
            {
                return false;
            }

            string error = "";

            int limitRow = 10;

            int count = 0;
            foreach (DrugDeptStockTakeDetails item in DrugDeptStockTakeDetailList)
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

        private void DrugDeptStockTakeDetails_Get(long ID, bool IsAlreadyRefresh = false)
        {
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyMedDeptServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginDrugDeptStockTakeDetails_Get(ID, V_MedProductType, CurrentDrugDeptStockTakes.StockTakingDate, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndDrugDeptStockTakeDetails_Get(asyncResult);
                                if (CurrentDrugDeptStockTakes.V_StockTakeType == (long)AllLookupValues.V_StockTakeType.KIEMKE_KETCHUYEN)
                                {
                                    DrugDeptStockTakeDetailList = results.ToObservableCollection();

                                    CheckCaculatedQty();
                                }
                                else
                                {
                                    //Nếu là kiểm kê bổ sung thì lưu kết quả vào DrugDeptStockTakeDetailList_Hide (không hiển thị lên giao diện), để khi user tìm thuốc bằng autocomplete, thì copy số lượng lý thuyết từ list này (07/05/2015 14:32).
                                    DrugDeptStockTakeDetailList_Hide = results.ToObservableCollection();
                                }
                                IsAlreadyRefresh = true;
                                Count_DifferenceValueInventory();
                                LoadDataGrid();

                                CanGetStockTake = false;
                                GetLastDrugDeptStockTakes(ID);
                            }
                            catch (Exception ex)
                            {
                                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                                IsAlreadyRefresh = false;
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

        //--▼--02-- 11/12/2020 DatTB
        private void ReGetDrugDeptStockTakeDetails(long ID, bool IsAlreadyRefresh = false)
        {
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyMedDeptServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginReGetDrugDeptStockTakeDetails(ID, V_MedProductType, CurrentDrugDeptStockTakes.StockTakingDate, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndReGetDrugDeptStockTakeDetails(asyncResult);
                                if (CurrentDrugDeptStockTakes.V_StockTakeType == (long)AllLookupValues.V_StockTakeType.KIEMKE_KETCHUYEN)
                                {
                                    DrugDeptStockTakeDetailList = results.ToObservableCollection();

                                    CheckCaculatedQty();
                                }
                                else
                                {
                                    DrugDeptStockTakeDetailList_Hide = results.ToObservableCollection();
                                }

                                if (null != results)
                                {
                                    CurrentDrugDeptStockTakes.DrugDeptStockTakeID = 0;
                                }

                                IsAlreadyRefresh = true;
                                Count_DifferenceValueInventory();
                                LoadDataGrid();
                                _mThemMoi = true;
                                IsReInsert = true;

                                //--▼--002--09/01/2021 DatTB
                                CanGetStockTake = false;
                                CanLockStore = false;
                                CanUnLockStore = false;
                                CanReGetStockTake = false;
                                //--▲--002--09/01/2021 DatTB
                            }
                            catch (Exception ex)
                            {
                                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                                IsAlreadyRefresh = false;
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
        //--▲--02-- 11/12/2020 DatTB

        // TxD 19/04/2015: Changed to adapt new rule for Removing Rows With All Zeros
        private void CheckFilteringOutRowWithAllZeroValues()
        {
            foreach (var stItem in CurrentDrugDeptStockTakes.StockTakeDetails)
            {
                // If a row marked by Stored Proc to be deleted and user added adjustment qty then it should not be deleted
                if (stItem.RowActionStatusFlag > 0 && stItem.AdjustQty != 0)
                {
                    stItem.RowActionStatusFlag = 0;
                }
            }
        }

        private void DrugDeptStockTakeDetails_Save(bool IsConfirmFinished = false)
        {
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            CurrentDrugDeptStockTakes.V_MedProductType = V_MedProductType;
            CurrentDrugDeptStockTakes.StockTakingDate = CurrentDrugDeptStockTakes.StockTakingDate.Date.AddHours(23).AddMinutes(59).AddSeconds(59);
            CheckFilteringOutRowWithAllZeroValues();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyMedDeptServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginDrugDeptStockTake_Save(CurrentDrugDeptStockTakes, IsConfirmFinished, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                long ID = 0;
                                string StrError;
                                var results = contract.EndDrugDeptStockTake_Save(out ID, out StrError, asyncResult);
                                if (string.IsNullOrEmpty(StrError))
                                {
                                    MessageBox.Show(eHCMSResources.A0756_G1_Msg_InfoKKOK, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                    //load danh sach thuoc theo hoa don 
                                    CurrentDrugDeptStockTakes.DrugDeptStockTakeID = ID;
                                    GetLastDrugDeptStockTakes(CurrentDrugDeptStockTakes.StoreID); //--12/12/2020 DatTB
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
                }
                catch (Exception ex)
                {
                    this.HideBusyIndicator();
                    MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                }
            });

            t.Start();
        }

        //--▼--03-- 12/12/2020 DatTB
        private void DrugDeptStockTakeDetails_Resave(bool IsConfirmFinished = false)
        {
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            CurrentDrugDeptStockTakes.V_MedProductType = V_MedProductType;
            CurrentDrugDeptStockTakes.StockTakingDate = CurrentDrugDeptStockTakes.StockTakingDate.Date.AddHours(23).AddMinutes(59).AddSeconds(59);

            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyMedDeptServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginDrugDeptStockTake_Resave(CurrentDrugDeptStockTakes, IsConfirmFinished, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                long ID = 0;
                                string StrError;
                                var results = contract.EndDrugDeptStockTake_Resave(out ID, out StrError, asyncResult);
                                if (string.IsNullOrEmpty(StrError))
                                {
                                    MessageBox.Show(eHCMSResources.A0756_G1_Msg_InfoKKOK, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                    //load danh sach thuoc theo hoa don 
                                    CurrentDrugDeptStockTakes.DrugDeptStockTakeID = ID;
                                    IsReInsert = false;
                                    GetLastDrugDeptStockTakes(CurrentDrugDeptStockTakes.StoreID);
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
                }
                catch (Exception ex)
                {
                    this.HideBusyIndicator();
                    MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                }
            });

            t.Start();
        }
        //--▲--03-- 12/12/2020 DatTB

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

        public void Paging_Unchecked(object sender, RoutedEventArgs e)
        {
            //deavtivate datapager
            pagerStockTakes.Source = null;
            VisibilityPaging = Visibility.Collapsed;

            LoadDataGrid();
        }

        //▼====== #002
        private CollectionViewSource _CVS_DrugDeptStockTakeDetails;
        public CollectionViewSource CVS_DrugDeptStockTakeDetails
        {
            get
            {
                return _CVS_DrugDeptStockTakeDetails;
            }
            set
            {
                _CVS_DrugDeptStockTakeDetails = value;
                NotifyOfPropertyChange(() => CVS_DrugDeptStockTakeDetails);
            }
        }

        public CollectionView CV_DrugDeptStockTakeDetails { get; set; }

        private void LoadDataGrid()
        {
            if (DrugDeptStockTakeDetailList == null)
            {
                DrugDeptStockTakeDetailList = new ObservableCollection<DrugDeptStockTakeDetails>();
            }

            CVS_DrugDeptStockTakeDetails = new CollectionViewSource { Source = DrugDeptStockTakeDetailList };
            CV_DrugDeptStockTakeDetails = (CollectionView)CVS_DrugDeptStockTakeDetails.View;
            NotifyOfPropertyChange(() => CV_DrugDeptStockTakeDetails);
            btnFilter();
        }
        //▲====== #002
        public void cbxPageSize_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((sender as ComboBox) != null && (sender as ComboBox).SelectedItem != null)
            {
                PCVPageSize = Convert.ToInt32(((sender as ComboBox).SelectedItem as ComboBoxItem).Content);
            }
        }

        public void btnFilter()
        {
            //▼====== #002
            CV_DrugDeptStockTakeDetails.Filter = null;
            CV_DrugDeptStockTakeDetails.Filter = new Predicate<object>(DoFilter);
            //▲====== #002
        }

        //Callback method
        private bool DoFilter(object o)
        {
            string Code = Globals.FormatCode(V_MedProductType, SearchKey);

            //it is not a case sensitive search
            DrugDeptStockTakeDetails emp = o as DrugDeptStockTakeDetails;
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
            if (DrugDeptStockTakeDetailList == null || DrugDeptStockTakeDetailList.Count <= 0)
            {
                MessageBox.Show(eHCMSResources.A0646_G1_Msg_InfoKhCoHgDeLuuKK, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return false;
            }

            ObservableCollection<DrugDeptStockTakeDetails> InvalidProduct = new ObservableCollection<DrugDeptStockTakeDetails>();
            InvalidProduct = DrugDeptStockTakeDetailList.Where(x => x.CaculatedQty < 0 || x.ActualQty < 0).ToObservableCollection();

            if (InvalidProduct.Count > 0)
            {
                string strBrandName = "";
                int limitRow = 10;
                int count = 0;

                foreach (DrugDeptStockTakeDetails temp in InvalidProduct)
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

                MessageBox.Show(string.Format("{0} \n{1}", eHCMSResources.A0891_G1_Msg_InfoKKFail, strBrandName), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);

                return false;
            }

            return true;
        }

        private void SaveDrugDeptStockTakeAndDetails(bool IsConfirmFinished = false)
        {
            if (CurrentDrugDeptStockTakes == null)
            {
                return;
            }
            //if (string.IsNullOrEmpty((CurrentDrugDeptStockTakes.StockTakePeriodName).Trim())) 12/12/2020 DatTB
            if (string.IsNullOrEmpty((CurrentDrugDeptStockTakes.StockTakePeriodName)))
            {
                MessageBox.Show(eHCMSResources.Z2803_G1_ChuaNhapTenKK, eHCMSResources.G0442_G1_TBao);
                return;
            }
            if (CurrentDrugDeptStockTakes.StoreID <= 0)
            {
                MessageBox.Show(eHCMSResources.A0582_G1_Msg_InfoChonKhoKK, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
            if (!CheckDataSave())
            {
                return;
            }
            if (CurrentDrugDeptStockTakes.V_StockTakeType == (long)AllLookupValues.V_StockTakeType.KIEMKE_KETCHUYEN)
            {
                if (MessageBox.Show(string.Format("{0} {1}", eHCMSResources.A0965_G1_Msg_ConfKC, CurrentDrugDeptStockTakes.StockTakingDate.ToShortDateString()), eHCMSResources.G2363_G1_XNhan, MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                {
                    return;
                }
            }
            //if (MessageBox.Show("Bạn phải kiểm tra kỹ số lượng thực tế trước khi lưu. Nếu đã lưu rồi sẽ không được chỉnh sửa gì nữa.", eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            //KMx: Chấp nhận số lượng lý thuyết và thực tế = 0 (24/07/2014 17:07)
            //var temp = DrugDeptStockTakeDetailList.Where(x => x.CaculatedQty > 0 && x.ActualQty > 0);
            var temp = DrugDeptStockTakeDetailList.Where(x => x.CaculatedQty >= 0 && x.ActualQty >= 0);
            CurrentDrugDeptStockTakes.StockTakeDetails = temp.ToObservableCollection();

            //--▼--04-- 12/12/2020 DatTB
            if (IsReInsert)
            {
                DrugDeptStockTakeDetails_Resave(IsConfirmFinished);
                return;
            }
            //--▲--04-- 12/12/2020 DatTB

            DrugDeptStockTakeDetails_Save(IsConfirmFinished);
        }

        public void btnSave()
        {
            SaveDrugDeptStockTakeAndDetails();
        }

        public void btnUpdate()
        {
            if (CurrentDrugDeptStockTakes == null || CurrentDrugDeptStockTakes.DrugDeptStockTakeID == 0)
            {
                return;
            }
            SaveDrugDeptStockTakeAndDetails();
        }

        public void btnConfirmFinished()
        {
            if (CurrentDrugDeptStockTakes == null || CurrentDrugDeptStockTakes.DrugDeptStockTakeID == 0)
            {
                return;
            }
            if (CurrentDrugDeptStockTakes.IsRefresh)
            {
                Coroutine.BeginExecute(DoReloadAndUpdateStockTake(true));
                return;
            }

            SaveDrugDeptStockTakeAndDetails(true);
        }

        public void btnRemove()
        {
            if (CurrentDrugDeptStockTakes == null || CurrentDrugDeptStockTakes.DrugDeptStockTakeID == 0 || Globals.LoggedUserAccount == null)
            {
                return;
            }
            if (CurrentDrugDeptStockTakes.IsFinished)
            {
                MessageBox.Show(eHCMSResources.Z2609_G1_KiemKeKetChuyenHoanTat, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyMedDeptServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginDrugDeptStockTake_Remove(CurrentDrugDeptStockTakes.DrugDeptStockTakeID, Globals.LoggedUserAccount.StaffID.GetValueOrDefault(0), Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                if (contract.EndDrugDeptStockTake_Remove(asyncResult))
                                {
                                    MessageBox.Show(eHCMSResources.Z2610_G1_HuyKiemKeThanhCong, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                    CurrentDrugDeptStockTakes = new DrugDeptStockTakes();
                                    DrugDeptStockTakeDetailList = new ObservableCollection<DrugDeptStockTakeDetails>();
                                    LoadDataGrid();
                                }
                                else
                                {
                                    MessageBox.Show(eHCMSResources.K0484_G1_XoaFail, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
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
                }
                catch (Exception ex)
                {
                    this.HideBusyIndicator();
                    MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                }
            });

            t.Start();
        }

        public void KeyEnabledComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ////if (flag)
            ////{
            //if (CurrentDrugDeptStockTakes != null && CurrentDrugDeptStockTakes.DrugDeptStockTakeID <= 0)
            //{
            //    if ((sender as ComboBox).SelectedItem != null)
            //    {
            //        if (CurrentDrugDeptStockTakes != null)
            //        {
            //            CurrentDrugDeptStockTakes.DrugDeptStockTakeID = 0;
            //            CurrentDrugDeptStockTakes.StockTakeNotes = "";
            //            CurrentDrugDeptStockTakes.StockTakePeriodName = "";
            //            CurrentDrugDeptStockTakes.StockTakingDate = DateTime.Now;
            //            CurrentDrugDeptStockTakes.StockTakeDetails = null;
            //            DrugDeptStockTakeDetailList = null;
            //            PCVDrugDeptStockTakeDetails = null;

            //            CurrentDrugDeptStockTakes.StoreID = (long)(sender as ComboBox).SelectedValue;
            //            DrugDeptStockTakeDetails_Get(CurrentDrugDeptStockTakes.StoreID);
            //        }
            //    }
            //}
            ////}
            ////flag = true;
            if ((sender as ComboBox).SelectedItem != null)
            {
                long storeId = (long)(sender as ComboBox).SelectedValue;
                GetLastDrugDeptStockTakes(storeId);
            }
        }

        //--▼--01-- 11/12/2020 DatTB
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

        public void UpdateGroupBtn1StatusAfterChangedStore()
        {
            CanReGetStockTake = false;
            CanLockStore = false;
            CanUnLockStore = false;
            if (null == CurrentDrugDeptStockTakes
                || null == LastDrugDeptStockTakes
                || CurrentDrugDeptStockTakes.DrugDeptStockTakeID != LastDrugDeptStockTakes.DrugDeptStockTakeID)
            {
                return;
            }
            if (CurrentDrugDeptStockTakes.DrugDeptStockTakeID == LastDrugDeptStockTakes.DrugDeptStockTakeID)
            {
                CanReGetStockTake = !LastDrugDeptStockTakes.IsLocked;
            }
            if (LastDrugDeptStockTakes.IsLocked)
            {
                CanUnLockStore = true;
            }
            else
            {
                CanLockStore = true;
            }
        }

        /// <summary>
        /// Update status of group btn affer locked and unlocked store
        /// VuTTM
        /// </summary>
        /// <param name="isLocked"></param>
        public void UpdateGroupBtn1StatusAfterChangedStore(bool isLocked)
        {
            CanReGetStockTake = false;
            CanLockStore = false;
            CanUnLockStore = false;
            if (null == CurrentDrugDeptStockTakes
                || null == LastDrugDeptStockTakes
                || CurrentDrugDeptStockTakes.DrugDeptStockTakeID != LastDrugDeptStockTakes.DrugDeptStockTakeID)
            {
                return;
            }
            if (CurrentDrugDeptStockTakes.DrugDeptStockTakeID == LastDrugDeptStockTakes.DrugDeptStockTakeID)
            {
                CanReGetStockTake = !isLocked;
            }
            if (isLocked)
            {
                CanUnLockStore = true;
            }
            else
            {
                CanLockStore = true;
            }
        }

        private void GetLastDrugDeptStockTakes(long storeId)
        {
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyMedDeptServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetLastDrugDeptStockTakes(storeId, V_MedProductType, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndGetLastDrugDeptStockTakes(asyncResult);
                                LastDrugDeptStockTakes = null;
                                if (null != results)
                                {
                                    LastDrugDeptStockTakes = results;
                                }
                                UpdateGroupBtn1StatusAfterChangedStore();
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

        private DrugDeptStockTakes _LastDrugDeptStockTakes;
        public DrugDeptStockTakes LastDrugDeptStockTakes
        {
            get
            {
                return _LastDrugDeptStockTakes;
            }
            set
            {
                if (_LastDrugDeptStockTakes != value)
                {
                    _LastDrugDeptStockTakes = value;
                    NotifyOfPropertyChange(() => LastDrugDeptStockTakes);
                }
            }
        }
        //--▲--01-- 11/12/2020 DatTB

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
            if (CurrentDrugDeptStockTakes.StockTakingDate.Year.CompareTo(Globals.GetCurServerDateTime().Year) != 0)
            {
                MessageBox.Show("Không thể tính tồn đầu kỳ. Ngày thực hiện không hợp lệ.",
                    eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
            //--▲-- 06/02/2021 DatTB
            if (CurrentDrugDeptStockTakes == null)
            {
                return;
            }
            if (string.IsNullOrEmpty((CurrentDrugDeptStockTakes.StockTakePeriodName)))
            {
                MessageBox.Show(eHCMSResources.Z2803_G1_ChuaNhapTenKK, eHCMSResources.G0442_G1_TBao);
                return;
            }
            //--▼-- 06/02/2021 DatTB
            if ((null != CurrentDrugDeptStockTakes.StockTakingDate
                    && (CurrentDrugDeptStockTakes.StockTakingDate.DayOfYear >= Globals.GetCurServerDateTime().DayOfYear)
                    && (CurrentDrugDeptStockTakes.StockTakingDate.DayOfYear != _FirstStockTakingDate.DayOfYear))
                || (null != LastDrugDeptStockTakes && null != LastDrugDeptStockTakes.StockTakingDate
                    && (DateTime.Compare(CurrentDrugDeptStockTakes.StockTakingDate, LastDrugDeptStockTakes.StockTakingDate) <= 0)))
            {
                MessageBox.Show("Không thể tính tồn đầu kỳ. Ngày thực hiện phải trước ngày hiện tại và lớn hơn ngày tính tồn gần nhất.",
                    eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
            //--▲-- 06/02/2021 DatTB
            if (CurrentDrugDeptStockTakes.DrugDeptStockTakeID > 0)
            {
                MessageBox.Show(eHCMSResources.A0600_G1_Msg_InfoLoadKK, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
            if (cbxStore == null || cbxStore.SelectedItem == null || ((RefStorageWarehouseLocation)cbxStore.SelectedItem).StoreID <= 0)
            {
                MessageBox.Show(string.Format("{0}!", eHCMSResources.K0338_G1_ChonKho), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }

            CurrentDrugDeptStockTakes.StoreID = ((RefStorageWarehouseLocation)cbxStore.SelectedItem).StoreID;
            DrugDeptStockTakeDetails_Get(CurrentDrugDeptStockTakes.StoreID);
        }

        public void btnRegetStockTake()
        {
            if (CurrentDrugDeptStockTakes == null)
            {
                MessageBox.Show("Không thể tính lại tồn đầu. Vui lòng chọn kho và chọn thời điểm tính tồn gần nhất.",
                    eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
            if (null != LastDrugDeptStockTakes
                && !(LastDrugDeptStockTakes.DrugDeptStockTakeID == CurrentDrugDeptStockTakes.DrugDeptStockTakeID
                    && !LastDrugDeptStockTakes.IsLocked))
            {
                MessageBox.Show("Không thể tính lại tồn đầu. Vui lòng mở kho và chọn thời điểm tính tồn gần nhất.",
                    eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }

            CurrentDrugDeptStockTakes.StoreID = ((RefStorageWarehouseLocation)cbxStore.SelectedItem).StoreID;
            ReGetDrugDeptStockTakeDetails(CurrentDrugDeptStockTakes.StoreID);
        }

        public void btnSearch()
        {
            DrugDeptStockTakes_Search(0, Globals.PageSize);
        }

        public void btnNew()
        {
            TotalValueInventory = 0;
            DifferenceValueInventory = 0;

            CurrentDrugDeptStockTakes.DrugDeptStockTakeID = 0;
            CurrentDrugDeptStockTakes.StockTakeNotes = "";
            CurrentDrugDeptStockTakes.StockTakePeriodName = "";
            CurrentDrugDeptStockTakes.StockTakingDate = Globals.GetCurServerDateTime();
            CurrentDrugDeptStockTakes.StoreID = 0;
            CurrentDrugDeptStockTakes.StockTakeDetails = null;
            CurrentDrugDeptStockTakes.FullName = Globals.LoggedUserAccount.Staff.FullName;
            CurrentDrugDeptStockTakes.StaffID = Globals.LoggedUserAccount.Staff.StaffID;

            //▼====== #002: Không thể xét null CollectionView/Source => phải new mới rs giá trị đc.
            DrugDeptStockTakeDetailList = new ObservableCollection<DrugDeptStockTakeDetails>();
            CVS_DrugDeptStockTakeDetails = new CollectionViewSource { Source = DrugDeptStockTakeDetailList };
            CV_DrugDeptStockTakeDetails = (CollectionView)CVS_DrugDeptStockTakeDetails.View;
            NotifyOfPropertyChange(() => CV_DrugDeptStockTakeDetails);
            //▲====== #002

            SelectedProductStockTake = null;
            DrugDeptStockTakeDetailList_Hide = null;
            CanGetStockTake = true;
            CanReGetStockTake = false;
            CanLockStore = false;
            CanUnLockStore = false;
            IsReInsert = false;
            UnCheckPaging();
        }

        //private void ChangeValue(long value1, long value2)
        //{
        //    if (value1 != value2)
        //    {
        //        flag = false;
        //    }
        //    else
        //    {
        //        flag = true;
        //    }
        //}

        //private bool flag = true;

        public void btnExportExcel()
        {
            if (CurrentDrugDeptStockTakes == null || CurrentDrugDeptStockTakes.StoreID <= 0)
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
            RptParameters.StoreID = CurrentDrugDeptStockTakes.StoreID;
            RptParameters.StockTake = new StockTake()
            {
                StockTakeID = CurrentDrugDeptStockTakes.DrugDeptStockTakeID,
                StockTakeType = StockTakeType.KIEM_KE_KHOA_DUOC,
                V_MedProductType = V_MedProductType,
                StockTakeDate = CurrentDrugDeptStockTakes.StockTakingDate
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
                if (file != null)
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
                        List<DrugDeptStockTakeDetails> invoicedrug = new List<DrugDeptStockTakeDetails>();
                        using (ExcelPackage package = new ExcelPackage(ms))
                        {
                            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                            ExcelWorksheet workSheet = package.Workbook.Worksheets.FirstOrDefault();
                            int startRow = workSheet.Dimension.Start.Row + 1;
                            int endRow = workSheet.Dimension.End.Row;                           
                            if (workSheet.Dimension.Columns != 12)
                            {
                                MessageBox.Show("File excel không đúng định dạng");
                                return;
                            }
                            else
                            {                                
                                for (int i = startRow; i <= endRow; i++)
                                {
                                    DrugDeptStockTakeDetails drugs = new DrugDeptStockTakeDetails();
                                    int j = 1;
                                    try
                                    {
                                        drugs.GenMedProductID = Convert.ToInt32(workSheet.Cells[i, j++].Value);
                                        drugs.Code = workSheet.Cells[i, j++].Value.ToString();
                                        drugs.BrandName = workSheet.Cells[i, j++].Value.ToString();
                                        drugs.ProductCodeRefNum = workSheet.Cells[i, j++].Value.ToString();
                                        drugs.UnitName = workSheet.Cells[i, j++].Value.ToString();
                                        drugs.NewestInwardPrice = Convert.ToDecimal(workSheet.Cells[i, j++].Value);
                                        drugs.Price = Convert.ToDecimal(workSheet.Cells[i, j++].Value);
                                        drugs.CaculatedQty = Convert.ToInt32(workSheet.Cells[i, j++].Value);
                                        drugs.ActualQty = Convert.ToInt32(workSheet.Cells[i, j++].Value);
                                        drugs.AdjustQty = Convert.ToInt32(workSheet.Cells[i, j++].Value);
                                        drugs.FinalAmount = Convert.ToDecimal(workSheet.Cells[i, j++].Value);
                                        drugs.Notes = workSheet.Cells[i, j++].Value.ToString();
                                        invoicedrug.Add(drugs);
                                    }
                                    catch (Exception ex)
                                    {
                                        MessageBox.Show("Giá trị tại dòng " + i + ", Cột " + j + " trong file Excel không đúng định dạng!");
                                        return;
                                    }
                                }
                                DrugDeptStockTakeDetailList = invoicedrug.ToObservableCollection();
                                CheckCaculatedQty();
                                if (null != DrugDeptStockTakeDetailList)
                                {
                                    CurrentDrugDeptStockTakes.DrugDeptStockTakeID = 0;
                                }
                                if (DrugDeptStockTakeDetailList == null)
                                {
                                    MessageBox.Show("Chưa có dữ liệu");
                                }
                                Count_DifferenceValueInventory();
                                LoadDataGrid();
                                IsAlreadyRefresh = true;
                                _mThemMoi = true;
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
                        IsAlreadyRefresh = false;
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

        public void Handle(DrugDeptCloseSearchStockTakesEvent message)
        {
            if (message != null && this.IsActive)
            {
                DrugDeptStockTakes temp = message.SelectedDrugDeptStockTakes as DrugDeptStockTakes;
                // ChangeValue(CurrentDrugDeptStockTakes.StoreID, temp.StoreID);
                CurrentDrugDeptStockTakes = temp;
                if (CurrentDrugDeptStockTakes != null && CurrentDrugDeptStockTakes.IsRefresh)
                {
                    IsRefresh = true;
                }
                else
                {
                    IsRefresh = false;
                }
                UnCheckPaging();
                //load detail
                DrugDeptStockTakeDetails_Load(CurrentDrugDeptStockTakes.DrugDeptStockTakeID);
            }
        }

        #endregion

        #region printing member
        KeyEnabledComboBox cbxStore;

        public void KeyEnabledComboBox_Loaded(object sender, RoutedEventArgs e)
        {
            cbxStore = sender as KeyEnabledComboBox;
        }

        public void btnPreviewStock()
        {
            if (CurrentDrugDeptStockTakes.StoreID <= 0)
            {
                MessageBox.Show(string.Format("{0}!", eHCMSResources.Z0941_G1_ChonKhoCanXem));
                return;
            }
            Action<IDrugDeptReportDocumentPreview> onInitDlg = delegate (IDrugDeptReportDocumentPreview proAlloc)
            {

                proAlloc.ID = CurrentDrugDeptStockTakes.DrugDeptStockTakeID;
                proAlloc.TitleRpt = string.Format("{0} ", eHCMSResources.K2919_G1_DS) + Globals.GetTextV_MedProductType(V_MedProductType) + string.Format(" {0} ", eHCMSResources.G1461_G1_TKho) + CurrentDrugDeptStockTakes.StockTakingDate.ToString("dd/MM/yyyy");
                proAlloc.TitleRpt1 = string.Format("{0}: ", eHCMSResources.T2144_G1_Kho) + ((DataEntities.RefStorageWarehouseLocation)(((aEMR.Controls.AxComboBox)(cbxStore)).SelectedItemEx)).swhlName.Trim();
                proAlloc.StoreID = CurrentDrugDeptStockTakes.StoreID;
                proAlloc.V_MedProductType = V_MedProductType;
                proAlloc.eItem = ReportName.RptDrugDeptStockTakes_ThuocYCuHoaChatKhoaDuoc;
            };
            GlobalsNAV.ShowDialog<IDrugDeptReportDocumentPreview>(onInitDlg);
        }

        public void btnPreview()
        {
            Action<IDrugDeptReportDocumentPreview> onInitDlg = delegate (IDrugDeptReportDocumentPreview proAlloc)
            {
                proAlloc.ID = CurrentDrugDeptStockTakes.DrugDeptStockTakeID;
                proAlloc.V_MedProductType = V_MedProductType;
                proAlloc.eItem = ReportName.DRUGDEPT_KIEMKE_KHOADUOC;
            };
            GlobalsNAV.ShowDialog<IDrugDeptReportDocumentPreview>(onInitDlg);
        }

        #endregion

        #region DifferenceValueInventory Member

        private decimal _DifferenceValueInventory = 0;
        public decimal DifferenceValueInventory
        {
            get { return _DifferenceValueInventory; }
            set
            {
                _DifferenceValueInventory = value;
                NotifyOfPropertyChange(() => DifferenceValueInventory);
            }
        }

        private decimal _TotalValueInventory = 0;
        public decimal TotalValueInventory
        {
            get { return _TotalValueInventory; }
            set
            {
                _TotalValueInventory = value;
                NotifyOfPropertyChange(() => TotalValueInventory);
            }
        }

        private void Count_DifferenceValueInventory()
        {
            DifferenceValueInventory = 0;
            if (CurrentDrugDeptStockTakes != null && DrugDeptStockTakeDetailList != null)
            {
                DifferenceValueInventory = DrugDeptStockTakeDetailList.Sum(x => x.AdjustQty * x.Price);
                //TotalValueInventory = DrugDeptStockTakeDetailList.Sum(x => x.ActualQty * x.Price);
                TotalValueInventory = DrugDeptStockTakeDetailList.Sum(x => x.FinalAmount);
            }
        }

        public void GridStockTakes_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            //KMx: Không dựa vào index, dựa vào Column Name là luôn đúng (27/02/2015 10:03).
            //if (e.Column.DisplayIndex == 6)
            //▼====== #001
            //if (e.Column.GetValue(FrameworkElement.NameProperty).ToString() == "colActualQty")
            if (e.Column.Equals(GridStockTakes.GetColumnByName("colActualQty")))
            //▲====== #001
            {
                Count_DifferenceValueInventory();
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

        public void Code_Checked(object sender, RoutedEventArgs e)
        {
            IsCode = true;
            VisibilityName = false;
        }

        public void Name_Checked(object sender, RoutedEventArgs e)
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
                MessageBox.Show(string.Format("{0}!", eHCMSResources.A0572_G1_Msg_InfoChonBatDauKK), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);

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
                SelectedProductStockTake = au.SelectedItem as DrugDeptStockTakeDetails;
            }
        }

        public void AutoCompleteBox_Populating(object sender, PopulatingEventArgs e)
        {
            if (CanGetStockTake)
            {
                MessageBox.Show(string.Format("{0}!", eHCMSResources.A0572_G1_Msg_InfoChonBatDauKK), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);

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

        private ObservableCollection<DrugDeptStockTakeDetails> _searchProductList;
        public ObservableCollection<DrugDeptStockTakeDetails> SearchProductList
        {
            get { return _searchProductList; }
            set
            {
                if (_searchProductList != value)
                    _searchProductList = value;
                NotifyOfPropertyChange(() => SearchProductList);
            }
        }

        private DrugDeptStockTakeDetails _selectedProductStockTake;
        public DrugDeptStockTakeDetails SelectedProductStockTake
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
            if (SearchProductList == null || SearchProductList.Count <= 0 || DrugDeptStockTakeDetailList_Hide == null || DrugDeptStockTakeDetailList_Hide.Count <= 0)
            {
                return;
            }

            foreach (DrugDeptStockTakeDetails item in SearchProductList)
            {
                DrugDeptStockTakeDetails StockTakeDetail = DrugDeptStockTakeDetailList_Hide.Where(x => x.GenMedProductID == item.GenMedProductID).FirstOrDefault();

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

            if (CurrentDrugDeptStockTakes == null)
            {
                return;
            }

            if (CurrentDrugDeptStockTakes.StoreID <= 0)
            {
                MessageBox.Show(eHCMSResources.K0322_G1_ChonKhoDeTimHgCanKK, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyMedDeptServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetProductForDrugDeptStockTake(Name, CurrentDrugDeptStockTakes.StoreID, IsCode, V_MedProductType, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndGetProductForDrugDeptStockTake(asyncResult);
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
                            //20180710 TBL: Comment lai vi khi ra ket qua no mat focus vao AutoCompleteBox
                            //if (IsCode)
                            //{
                            //    if (AxQty != null)
                            //    {
                            //        AxQty.Focus();
                            //    }
                            //}
                            //else
                            //{
                            //    if (au != null)
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

        public void AddItem()
        {
            if (CurrentDrugDeptStockTakes == null || DrugDeptStockTakeDetailList == null || SelectedProductStockTake == null)
            {
                return;
            }

            if (CurrentDrugDeptStockTakes.DrugDeptStockTakeID > 0)
            {
                MessageBox.Show(eHCMSResources.Z1289_G1_PhKKeDaLuu, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }

            if (DrugDeptStockTakeDetailList != null && DrugDeptStockTakeDetailList.Where(x => x.GenMedProductID == SelectedProductStockTake.GenMedProductID).Count() > 0)
            {
                MessageBox.Show(string.Format(eHCMSResources.K0019_G1_DaCoTrongDSKK, SelectedProductStockTake.BrandName), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }

            if (SelectedProductStockTake.CaculatedQty < 0)
            {
                MessageBox.Show(eHCMSResources.A0564_G1_Msg_InfoSLgLyThuyetNhoHon0, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }

            if (SelectedProductStockTake.ActualQty < 0)
            {
                MessageBox.Show(eHCMSResources.A0565_G1_Msg_InfoSLgThucTeNhoHon0, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }

            DrugDeptStockTakeDetailList.Add(SelectedProductStockTake);

            //KMx: Tính lại tổng tiền (27/02/2015 10:02).
            Count_DifferenceValueInventory();

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
            if (V_StockTakeTypeList != null && CurrentDrugDeptStockTakes != null)
            {
                CurrentDrugDeptStockTakes.V_StockTakeType = V_StockTakeTypeList.FirstOrDefault().LookupID;
            }
        }

        private bool _IsRefresh = false;
        public bool IsRefresh
        {
            get { return _IsRefresh; }
            set
            {
                if (_IsRefresh != value)
                    _IsRefresh = value;
                NotifyOfPropertyChange(() => IsRefresh);
            }
        }


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

        private bool _IsAlreadyRefresh = false;
        public bool IsAlreadyRefresh
        {
            get { return _IsAlreadyRefresh; }
            set
            {
                if (_IsAlreadyRefresh != value)
                    _IsAlreadyRefresh = value;
                NotifyOfPropertyChange(() => IsAlreadyRefresh);
            }
        }
        public void btnRefresh()
        {
            if (CurrentDrugDeptStockTakes == null)
            {
                return;
            }
            if (DrugDeptStockTakeDetailList == null || DrugDeptStockTakeDetailList.Count() == 0)
            {
                return;
            }
            DrugDeptStockTakeDetailList.Clear();
            CurrentDrugDeptStockTakes.StoreID = ((RefStorageWarehouseLocation)cbxStore.SelectedItem).StoreID;
            Coroutine.BeginExecute(DoReloadAndUpdateStockTake());
        }
        private void DrugDeptStockTakeDetails_SaveNew(GenericCoRoutineTask genTask, object IsConfirmFinished)
        {
            bool _IsConfirmFinished = Convert.ToBoolean(IsConfirmFinished);
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            CurrentDrugDeptStockTakes.V_MedProductType = V_MedProductType;
            CurrentDrugDeptStockTakes.StockTakingDate = CurrentDrugDeptStockTakes.StockTakingDate.Date.AddHours(23).AddMinutes(59).AddSeconds(59);
            CheckFilteringOutRowWithAllZeroValues();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyMedDeptServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginDrugDeptStockTake_SaveNew(CurrentDrugDeptStockTakes, _IsConfirmFinished, IsAlreadyRefresh, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                long ID = 0;
                                string StrError;
                                var results = contract.EndDrugDeptStockTake_SaveNew(out ID, out StrError, asyncResult);
                                if (string.IsNullOrEmpty(StrError))
                                {
                                    MessageBox.Show(eHCMSResources.A0756_G1_Msg_InfoKKOK, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                    CurrentDrugDeptStockTakes.DrugDeptStockTakeID = ID;
                                    if (IsAlreadyRefresh)
                                    {
                                        CurrentDrugDeptStockTakes.IsRefresh = false;
                                        IsRefresh = false;
                                    }
                                    genTask.ActionComplete(true);
                                }
                                else
                                {
                                    MessageBox.Show(StrError, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                    genTask.ActionComplete(false);
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                                genTask.ActionComplete(false);
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
                    MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                    genTask.ActionComplete(false);
                }
            });

            t.Start();
        }
        private void DrugDeptStockTakeDetails_GetNew(GenericCoRoutineTask genTask, object ID)
        {
            long _ID = Convert.ToInt64(ID);
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyMedDeptServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginDrugDeptStockTakeDetails_Get(_ID, V_MedProductType, CurrentDrugDeptStockTakes.StockTakingDate, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndDrugDeptStockTakeDetails_Get(asyncResult);
                                if (CurrentDrugDeptStockTakes.V_StockTakeType == (long)AllLookupValues.V_StockTakeType.KIEMKE_KETCHUYEN)
                                {
                                    DrugDeptStockTakeDetailList = results.ToObservableCollection();

                                    CheckCaculatedQty();
                                }
                                else
                                {
                                    //Nếu là kiểm kê bổ sung thì lưu kết quả vào DrugDeptStockTakeDetailList_Hide (không hiển thị lên giao diện), để khi user tìm thuốc bằng autocomplete, thì copy số lượng lý thuyết từ list này (07/05/2015 14:32).
                                    DrugDeptStockTakeDetailList_Hide = results.ToObservableCollection();
                                }
                                IsAlreadyRefresh = true;

                                Count_DifferenceValueInventory();
                                LoadDataGrid();

                                CanGetStockTake = false;
                                genTask.ActionComplete(true);
                            }
                            catch (Exception ex)
                            {
                                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                                IsAlreadyRefresh = false;
                                genTask.ActionComplete(false);
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
                    genTask.ActionComplete(false);
                }
            });

            t.Start();
        }
        private IEnumerator<IResult> DoReloadAndUpdateStockTake(bool IsConfirmFinished = false)
        {
            var DrugDeptStockTakeDetails_GetNewTask = new GenericCoRoutineTask(DrugDeptStockTakeDetails_GetNew, CurrentDrugDeptStockTakes.StoreID);
            yield return DrugDeptStockTakeDetails_GetNewTask;

            var temp = DrugDeptStockTakeDetailList.Where(x => x.CaculatedQty >= 0 && x.ActualQty >= 0);
            CurrentDrugDeptStockTakes.StockTakeDetails = temp.ToObservableCollection();

            var DrugDeptStockTakeDetails_SaveNewTask = new GenericCoRoutineTask(DrugDeptStockTakeDetails_SaveNew, IsConfirmFinished);
            yield return DrugDeptStockTakeDetails_SaveNewTask;

            yield break;
        }
        public void btnLockStore()
        {
            if (CurrentDrugDeptStockTakes == null)
            {
                MessageBox.Show("Không thể khóa kho. Vui lòng chọn thời điểm tính tồn gần nhất.", eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
            if (CurrentDrugDeptStockTakes.StoreID <= 0)
            {
                MessageBox.Show("Không thể khóa kho. Hãy chọn kho.", eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
            if (null != LastDrugDeptStockTakes
                && (CurrentDrugDeptStockTakes.DrugDeptStockTakeID < LastDrugDeptStockTakes.DrugDeptStockTakeID)
                && !LastDrugDeptStockTakes.IsLocked)
            {
                MessageBox.Show("Không thể khóa kho. Vui lòng chọn thời điểm tính tồn gần nhất.", eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }

            DrugDeptLockAndUnlockStore(CurrentDrugDeptStockTakes.StoreID, true);
        }

        public void btnUnlockStore()
        {
            if (CurrentDrugDeptStockTakes == null)
            {
                MessageBox.Show("Không thể mở kho. Vui lòng chọn thời điểm tính tồn gần nhất.", eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
            if (CurrentDrugDeptStockTakes.StoreID <= 0)
            {
                MessageBox.Show("Không thể mở kho. Hãy chọn kho.", eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
            if (null != LastDrugDeptStockTakes
                && (CurrentDrugDeptStockTakes.DrugDeptStockTakeID < LastDrugDeptStockTakes.DrugDeptStockTakeID)
                && LastDrugDeptStockTakes.IsLocked)
            {
                MessageBox.Show("Không thể mở kho. Vui lòng chọn thời điểm tính tồn gần nhất.", eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }

            DrugDeptLockAndUnlockStore(CurrentDrugDeptStockTakes.StoreID, false);
        }
        private void DrugDeptLockAndUnlockStore(long StoreID, bool IsLock)
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyMedDeptServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginDrugDeptLockAndUnlockStore(StoreID, V_MedProductType, IsLock, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            bool results = contract.EndDrugDeptLockAndUnlockStore(asyncResult);

                            if (results)
                            {
                                if (IsLock)
                                {
                                    MessageBox.Show(eHCMSResources.A0620_G1_Msg_InfoKhoaKhoOK, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                }
                                else
                                {
                                    MessageBox.Show(eHCMSResources.A0819_G1_Msg_InfoMoKhoaKhoOK, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                }
                                //--▼--001--09/01/2021 DatTB
                                GetLastDrugDeptStockTakes(StoreID);
                                //UpdateGroupBtn1StatusAfterChangedStore(IsLock);
                                //--▲--001--09/01/2021 DatTB
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
