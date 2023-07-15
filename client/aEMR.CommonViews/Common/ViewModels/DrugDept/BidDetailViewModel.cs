using System.ComponentModel.Composition;
using aEMR.ViewContracts;
using Caliburn.Micro;
using System.Collections.ObjectModel;
using DataEntities;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Input;
using System.Threading;
using aEMR.ServiceClient;
using aEMR.Infrastructure;
using System;
using eHCMSLanguage;
using System.Linq;
using System.Collections.Generic;
using aEMR.Common.Collections;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using aEMR.CommonTasks;
using System.Windows.Data;

/*
 * 20190909 #001 TBL:   BM 0014319: Thêm chức năng tìm kiếm thuốc theo tên hoặc code
 * 20190910 #002 TBL:   BM 0014321: Người dùng có thể tìm bằng tên thuốc
 * 20191106 #003 TNHX:  [BM 0013306]: separate V_MedProductType
 * 20200203 #004 TBL:   BM 0019592: Fix lỗi khi tạo đợt thầu mới sau đó thêm thuốc vào đợt thầu thì danh sách không hiển thị thuốc mới thêm vào
 * 20200402 #005 TTM:   BM 0029074: Fix lỗi chức năng lấy số lượng tự động sai thông tin kho truyền vào (Code cũ luôn mặc định = 1).
 * 20200625 #006 TNHX: BM : Thêm màn hình quản lý thầu cho Hóa chất
 * 20210122 #007 TNHX: BM : Tách danh sách thêm/xóa/ sửa. Thêm mã gói thầu. chỉnh hiện thị thông tin thầu, thêm người cập nhật
 * 20230323 #008 QTD:   Thêm trường năm chi tiết thầu
 */

namespace aEMR.Common.ViewModels
{
    [Export(typeof(IBidDetail)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class BidDetailViewModel : Conductor<object>, IBidDetail
    {
        #region Controls
        //21072018 TTM
        TextBox txtDrugName;
        #endregion

        #region Properties
        private bool _IsAllowEdit = false;
        public bool IsAllowEdit
        {
            get
            {
                return _IsAllowEdit;
            }
            set
            {
                if (_IsAllowEdit != value)
                {
                    _IsAllowEdit = value;
                    NotifyOfPropertyChange(() => IsAllowEdit);
                }
            }
        }

        private ObservableCollection<Bid> _gBidCollection;
        public ObservableCollection<Bid> gBidCollection
        {
            get
            {
                return _gBidCollection;
            }
            set
            {
                if (_gBidCollection != value)
                {
                    _gBidCollection = value;
                    NotifyOfPropertyChange(() => gBidCollection);
                }
            }
        }

        private ObservableCollection<Bid> _gBids;
        public ObservableCollection<Bid> gBids
        {
            get
            {
                return _gBids;
            }
            set
            {
                if (_gBids != value)
                {
                    _gBids = value;
                    NotifyOfPropertyChange(() => gBids);
                }
            }
        }

        private Bid _gSelectedBid;
        public Bid gSelectedBid
        {
            get
            {
                return _gSelectedBid;
            }
            set
            {
                if (_gSelectedBid != value)
                {
                    _gSelectedBid = value;
                    NotifyOfPropertyChange(() => gSelectedBid);
                    gBidDetails.Clear();
                }
            }
        }

        private ObservableCollection<BidDetail> _gBidDetails = new ObservableCollection<BidDetail>();
        public ObservableCollection<BidDetail> gBidDetails
        {
            get
            {
                return _gBidDetails;
            }
            set
            {
                if (_gBidDetails != value)
                {
                    _gBidDetails = value;
                    NotifyOfPropertyChange(() => gBidDetails);
                }
            }
        }

        private ObservableCollection<BidDetail> _gDelBidDetails = new ObservableCollection<BidDetail>();
        public ObservableCollection<BidDetail> gDelBidDetails
        {
            get
            {
                return _gDelBidDetails;
            }
            set
            {
                if (_gDelBidDetails != value)
                {
                    _gDelBidDetails = value;
                    NotifyOfPropertyChange(() => gDelBidDetails);
                }
            }
        }

        //▼====: #007
        private ObservableCollection<BidDetail> _gOrgBidDetails = new ObservableCollection<BidDetail>();
        public ObservableCollection<BidDetail> gOrgBidDetails
        {
            get
            {
                return _gOrgBidDetails;
            }
            set
            {
                if (_gOrgBidDetails != value)
                {
                    _gOrgBidDetails = value;
                    NotifyOfPropertyChange(() => gOrgBidDetails);
                }
            }
        }

        private ObservableCollection<BidDetail> _gAddBidDetails = new ObservableCollection<BidDetail>();
        public ObservableCollection<BidDetail> gAddBidDetails
        {
            get
            {
                return _gAddBidDetails;
            }
            set
            {
                if (_gAddBidDetails != value)
                {
                    _gAddBidDetails = value;
                    NotifyOfPropertyChange(() => gAddBidDetails);
                }
            }
        }

        private ObservableCollection<BidDetail> _gEditBidDetails = new ObservableCollection<BidDetail>();
        public ObservableCollection<BidDetail> gEditBidDetails
        {
            get
            {
                return _gEditBidDetails;
            }
            set
            {
                if (_gEditBidDetails != value)
                {
                    _gEditBidDetails = value;
                    NotifyOfPropertyChange(() => gEditBidDetails);
                }
            }
        }
        //▲====: #007

        private BidDetail _gAddBidDetail;
        public BidDetail gAddBidDetail
        {
            get
            {
                return _gAddBidDetail;
            }
            set
            {
                if (_gAddBidDetail != value)
                {
                    _gAddBidDetail = value;
                    NotifyOfPropertyChange(() => gAddBidDetail);
                }
            }
        }

        private BidDetail _gSelectedBidDetail;
        public BidDetail gSelectedBidDetail
        {
            get
            {
                return _gSelectedBidDetail;
            }
            set
            {
                if (_gSelectedBidDetail != value)
                {
                    _gSelectedBidDetail = value;
                    NotifyOfPropertyChange(() => gSelectedBidDetail);
                }
            }
        }

        public bool IsMedDept { get; set; }

        public Button btnAddDrug { get; set; }

        private long _V_MedProductType;
        public long V_MedProductType
        {
            get { return _V_MedProductType; }
            set
            {
                if (_V_MedProductType != value)
                {
                    _V_MedProductType = value;
                    NotifyOfPropertyChange(() => V_MedProductType);
                }
            }
        }

        private float _gFactor = 1.3f;
        public float gFactor
        {
            get
            {
                return _gFactor;
            }
            set
            {
                _gFactor = value;
                NotifyOfPropertyChange(() => gFactor);
            }
        }

        private ObservableCollection<DrugDeptSupplier> _SupplierCollection;
        public ObservableCollection<DrugDeptSupplier> SupplierCollection
        {
            get
            {
                return _SupplierCollection;
            }
            set
            {
                if (_SupplierCollection != value)
                {
                    _SupplierCollection = value;
                    NotifyOfPropertyChange(() => SupplierCollection);
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

        private CollectionViewSource CVS_ObjBidDetail = null;
        public CollectionView CV_ObjBidDetail
        {
            get;
            set;
        }
        #endregion

        #region Events
        [ImportingConstructor]
        public BidDetailViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg, ISalePosCaching salePosCaching)
        {
            Coroutine.BeginExecute(LoadSupplierCollection());
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            LoadBids();
            //▼===== #005
            Coroutine.BeginExecute(DoGetStore_ForBid());
            //▲===== #005
        }

        AutoCompleteBox cboContext;
        public void cboBid_Loaded(object sender, PopulatingEventArgs e)
        {
            cboContext = sender as AutoCompleteBox;
        }

        public void cboBid_Populating(object sender, PopulatingEventArgs e)
        {
            e.Cancel = true;
            if (cboContext != null && !string.IsNullOrEmpty(cboContext.Text))
            {
                var AllItemsContext = new ObservableCollection<Bid>(gBidCollection.Where(x => Globals.RemoveVietnameseString(x.BidName.ToLower()).Contains(Globals.RemoveVietnameseString(cboContext.Text.ToLower()))));
                cboContext.ItemsSource = AllItemsContext;
            }
            cboContext.PopulateComplete();
        }

        public void cboBid_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            gSelectedBid = ((AutoCompleteBox)sender).SelectedItem as Bid;
            if (gSelectedBid != null)
            {
                LoadBidDetails("*", IsMedDept);
            }
        }

        public void txtDrugCode_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (IsMedDept)
                {
                    (sender as TextBox).Text = Globals.FormatCode(V_MedProductType, (sender as TextBox).Text);
                }
                LoadBidDetails((sender as TextBox).Text, IsMedDept, true);
                //21072018 TTM
                //Nếu người dùng tìm kiếm 1 thuốc không có thì sẽ tự động clear tên thuốc vừa nhập
                //txtDrugName.Clear();
            }
        }

        public void txtDrugCode_LostFocus(object sender, RoutedEventArgs e)
        {
            //LoadBidDetails((sender as TextBox).Text, IsMedDept);
        }

        public void txtDrugName_Loaded(object sender, RoutedEventArgs e)
        {
            txtDrugName = sender as TextBox;
        }

        public void btnAddDrug_Loaded(object sender, RoutedEventArgs e)
        {
            btnAddDrug = sender as Button;
        }

        //▼====: #007
        public void btnAddDrug_Click(object sender, RoutedEventArgs e)
        {
            if (gAddBidDetail != null && !gBidDetails.Any(x => x.DrugID == gAddBidDetail.DrugID && !x.IsDeleted) && !gDelBidDetails.Any(x => x.DrugID == gAddBidDetail.DrugID))
            {
                gAddBidDetail.BidCode = gSelectedBid.PermitNumber;
                //▼====: #008
                if(Globals.ServerConfigSection.CommonItems.ApplyReport130)
                {
                    gAddBidDetail.BidCodeStr = gSelectedBid.PermitNumber + ';' + gSelectedBid.BidGroupName + ';' + gAddBidDetail.TCKTAndTCCNGroup + ';' + gSelectedBid.YearStr;
                    gAddBidDetail.YearStr = gSelectedBid.YearStr;
                }
                else
                {
                    gAddBidDetail.BidCodeStr = gSelectedBid.PermitNumber + ';' + gSelectedBid.BidGroupName + ';' + gAddBidDetail.TCKTAndTCCNGroup;
                }
                //▲====: #008
                gAddBidDetail.BidGroupName = gSelectedBid.BidGroupName;
                gAddBidDetail.IsAddNew = true;
                gBidDetails.Add(gAddBidDetail.DeepCopy());
                /*▼====: #004*/
                LoadDataGrid();
                /*▲====: #004*/
            }
            else if (gAddBidDetail != null)
            {
                MessageBox.Show(string.Format(eHCMSResources.Z2828_G1_DaTonTaiKhongTheThem, gAddBidDetail.ReportBrandName), eHCMSResources.G0442_G1_TBao);
            }
        }
        //▲====: #007

        public void RemoveItemCmd(object source, object eventArgs)
        {
            if (!gSelectedBidDetail.IsEditable)
            {
                return;
            }
            if (gSelectedBidDetail != null && gSelectedBidDetail.BidID == 0)
            {
                gBidDetails.Remove(gSelectedBidDetail);
            }
            else
            {
                if (MessageBox.Show(eHCMSResources.Z2110_G1_XoaMatHangKhoiDotThau, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    var DelItem = gSelectedBidDetail.DeepCopy();
                    DelItem.IsDeleted = true;
                    gDelBidDetails.Add(DelItem);
                    gBidDetails.Remove(gSelectedBidDetail);
                }
            }
        }

        public void btnSave()
        {
            if ((gBidDetails == null || gBidDetails.Count == 0 || !gBidDetails.Any(x => !x.IsDeleted)) && (gDelBidDetails == null || gDelBidDetails.Count == 0))
                return;
            if (gBidDetails != null && gBidDetails.Any(x => !x.IsDeleted && x.ApprovedQty == 0))
            {
                GlobalsNAV.ShowMessagePopup(eHCMSResources.Z2736_G1_SoLuongCNTKoHopLe);
                return;
            }
            //▼====: #007
            SeparateListBeforeSave();

            this.ShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
            var t = new Thread(() =>
            {
                using (var mServiceFactory = new PharmacyGenMedDeptDetailsServiceClient())
                {
                    var contract = mServiceFactory.ServiceInstance;
                    try
                    {
                        contract.BeginSaveBidDetails(gSelectedBid.BidID
                            , gAddBidDetails.ToList()
                            , gEditBidDetails.ToList()
                            , gDelBidDetails.ToList(), IsMedDept
                            , (long)Globals.LoggedUserAccount.StaffID
                            , Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var mBidDetails = contract.EndSaveBidDetails(asyncResult);
                                if (mBidDetails)
                                {
                                    MessageBox.Show(eHCMSResources.A0279_G1_Msg_InfoCNhatOK, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                    gAddBidDetail = null;
                                    gBidDetails.Clear();
                                    gDelBidDetails.Clear();
                                    gEditBidDetails.Clear();
                                    if (gSelectedBid != null)
                                    {
                                        LoadBidDetails("*", IsMedDept);
                                    }
                                    //▲====: #007
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
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                        this.HideBusyIndicator();
                    }
                }
            });
            t.Start();
        }

        public void btnAddNewBid()
        {
            gSelectedBid = new Bid
            {
                ValidDateFrom = Globals.GetCurServerDateTime(),
                ValidDateTo = Globals.GetCurServerDateTime(),
                StaffID = (long)Globals.LoggedUserAccount.StaffID,
                IsMedDept = IsMedDept,
                V_MedProductType = V_MedProductType
            };
            void onInitDlg(IBidEdit Alloc)
            {
                Alloc.ObjBid = gSelectedBid;
            }
            GlobalsNAV.ShowDialog<IBidEdit>(onInitDlg, (o, s) => { LoadBids(); });
        }

        public void btnSearchBid()
        {
            IBidCollection mChildView = Globals.GetViewModel<IBidCollection>();
            mChildView.V_MedProductType = V_MedProductType;
            GlobalsNAV.ShowDialog_V3(mChildView, null, null, false, true, new Size(900, 650));
            if (mChildView.SelectedBid != null)
            {
                gSelectedBid = mChildView.SelectedBid;
                cboContext.SelectedItem = gSelectedBid;
                cboContext.PopulateComplete();
                LoadBidDetails("*", IsMedDept);
            }
        }

        public void btnAutoGetContent()
        {
            if (gSelectedBid == null || gSelectedBid.BidID == 0
                || gSelectedBid.ValidDateFrom <= new DateTime(2010, 1, 1)
                || gSelectedBid.ValidDateTo <= new DateTime(2010, 1, 1)
                || gFactor <= 0 || SelectedStorage == null)
            {
                return;
            }

            DateTime mEndDate = gSelectedBid.ValidDateFrom.Date.AddDays(-1);
            DateTime mStartDate = mEndDate.AddDays(-(gSelectedBid.ValidDateTo - gSelectedBid.ValidDateFrom).TotalDays);

            this.ShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
            var t = new Thread(() =>
            {
                using (var mServiceFactory = new PharmacyDrugServiceClient())
                {
                    var contract = mServiceFactory.ServiceInstance;
                    try
                    {
                        contract.BeginGetAutoBidDetailCollection(V_MedProductType, mStartDate, mEndDate, SelectedStorage.StoreID, gFactor, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var mAutoBidDetailCollection = contract.EndGetAutoBidDetailCollection(asyncResult);
                                if (mAutoBidDetailCollection != null && mAutoBidDetailCollection.Count > 0
                                    && mAutoBidDetailCollection.Any(x => gBidDetails == null || gBidDetails.Count == 0 || !gBidDetails.Any(o => o.DrugID == x.DrugID)))
                                {
                                    foreach (var aItem in mAutoBidDetailCollection.Where(x => gBidDetails == null || gBidDetails.Count == 0 || !gBidDetails.Any(o => o.DrugID == x.DrugID)).ToList())
                                    {
                                        if (gSelectedBid != null)
                                        {
                                            aItem.BidCode = gSelectedBid.PermitNumber;
                                        }
                                        gBidDetails.Add(aItem);
                                    }
                                }
                                LoadDataGrid();
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
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                        this.HideBusyIndicator();
                    }
                }
            });

            t.Start();
        }
        #endregion

        #region Methods
        private void LoadBids()
        {
            this.ShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
            var t = new Thread(() =>
            {
                using (var mServiceFactory = new PharmacyGenMedDeptDetailsServiceClient())
                {
                    var contract = mServiceFactory.ServiceInstance;
                    try
                    {
                        contract.BeginGetAllBids(V_MedProductType, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                gBidCollection = new ObservableCollection<Bid>(contract.EndGetAllBids(asyncResult));
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
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                        this.HideBusyIndicator();
                    }
                }
            });
            t.Start();
        }

        private void LoadBidDetails(string DrugCode, bool IsMedDept, bool IsAddNew = false)
        {
            if (gSelectedBid == null)
            {
                MessageBox.Show(eHCMSResources.Z2113_G1_ChonDotThau, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
            //if (IsMedDept)
            //{
            //    DrugCode = Globals.FormatCode(V_MedProductType, DrugCode);
            //}
            this.ShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
            var t = new Thread(() =>
            {
                using (var mServiceFactory = new PharmacyGenMedDeptDetailsServiceClient())
                {
                    var contract = mServiceFactory.ServiceInstance;
                    try
                    {
                        contract.BeginGetBidDetails(gSelectedBid.BidID, DrugCode, IsMedDept, V_MedProductType, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                List<BidDetail> mBidDetails = contract.EndGetBidDetails(asyncResult);
                                //20072018 TTM
                                //Thêm điều kiện kiểm tra xem mBidDetails được trả về giá trị nào không. Vì nếu không có giá trị nào đc trả về
                                //từ End thì gAddBidDetail sẽ = null dẫn đến bị lỗi.
                                if (mBidDetails.Count > 0)
                                {
                                    if (IsAddNew)
                                    {
                                        gAddBidDetail = mBidDetails.OrderByDescending(x => x.VersionID).FirstOrDefault();
                                        //▼====: #007
                                        gAddBidDetail.IsAddNew = true;
                                        //▲====: #007
                                    }
                                    if (!IsAddNew)
                                    {
                                        foreach (var aItem in mBidDetails.Where(x => gBidDetails == null || gBidDetails.Count == 0 || !gBidDetails.Any(o => o.DrugID == x.DrugID)).ToList())
                                        {
                                            if (aItem.IsDeleted)
                                            {
                                                aItem.IsEditable = !aItem.IsDeleted;
                                            }
                                            gBidDetails.Add(aItem);
                                        }
                                        //gBidDetails = mBidDetails.ToObservableCollection();
                                    }
                                    else if (!gBidDetails.Any(x => x.DrugID == gAddBidDetail.DrugID))
                                    {
                                        if (gSelectedBid != null)
                                        {
                                            gAddBidDetail.BidCode = gSelectedBid.PermitNumber;
                                            //▼====: #007
                                            gAddBidDetail.BidGroupName = gSelectedBid.BidGroupName;
                                            gAddBidDetail.BidCodeStr = gSelectedBid.PermitNumber + ';' + gSelectedBid.BidGroupName + ';' + gAddBidDetail.TCKTAndTCCNGroup;
                                            //▲====: #007
                                        }
                                        gBidDetails.Add(gAddBidDetail);
                                    }
                                    else
                                    {
                                        MessageBox.Show(string.Format(eHCMSResources.Z2828_G1_DaTonTaiKhongTheThem, gAddBidDetail.ReportBrandName), eHCMSResources.G0442_G1_TBao);
                                    }
                                    //▼====: #007
                                    gOrgBidDetails = gBidDetails.DeepCopy();
                                    //▲====: #007
                                    gDelBidDetails.Clear();
                                }
                                else
                                {
                                    //Cần 1 thông báo để người dùng biết được là mã thuốc vừa nhập không có.
                                    if (DrugCode != "*")
                                    {
                                        MessageBox.Show(string.Format(eHCMSResources.Z2878_G1_KhongTimThayThuoc, DrugCode), eHCMSResources.G0442_G1_TBao);
                                    }
                                }
                                LoadDataGrid();
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
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                        this.HideBusyIndicator();
                    }
                }
            });

            t.Start();
        }

        private IEnumerator<IResult> LoadSupplierCollection()
        {
            var LoadSupplierListTask = new LoadDrugDeptSupplierListTask((int)AllLookupValues.SupplierType.CUNG_CAP_THIET_BI_Y_TE, false, false);
            yield return LoadSupplierListTask;
            SupplierCollection = LoadSupplierListTask.DrugDeptSupplierList;
        }

        /*▼====: #001*/
        private void LoadDataGrid()
        {
            CVS_ObjBidDetail = null;
            CVS_ObjBidDetail = new CollectionViewSource { Source = gBidDetails };
            CV_ObjBidDetail = (CollectionView)CVS_ObjBidDetail.View;
            NotifyOfPropertyChange(() => CV_ObjBidDetail);
            btnFilter();
        }

        public void btnFilter()
        {
            CV_ObjBidDetail.Filter = null;
            CV_ObjBidDetail.Filter = new Predicate<object>(DoFilter);
        }

        private bool DoFilter(object o)
        {
            BidDetail emp = o as BidDetail;
            if (emp != null)
            {
                if (string.IsNullOrEmpty(SearchKey))
                {
                    SearchKey = "";
                }
                if ((!string.IsNullOrEmpty(emp.ReportBrandName) && Globals.RemoveVietnameseString(emp.ReportBrandName.ToLower()).IndexOf(SearchKey.Trim().ToLower()) >= 0)
                    || Globals.RemoveVietnameseString(emp.DrugCode.ToLower()).IndexOf(SearchKey.Trim().ToLower()) >= 0
                    || (!string.IsNullOrEmpty(emp.ReportBrandName) && emp.ReportBrandName.ToLower().IndexOf(SearchKey.Trim().ToLower()) >= 0)
                    || emp.DrugCode.ToLower().IndexOf(SearchKey.Trim().ToLower()) >= 0)
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

        /*▲====: #001*/
        /*▼====: #002*/
        public void gvBidDetail_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString() + ". ";
        }

        private string BrandName;
        AutoCompleteBox au;
        public void AutoCompleteBox_Populating(object sender, PopulatingEventArgs e)
        {
            au = sender as AutoCompleteBox;
            BrandName = e.Parameter;
            GetRefGenMedProductDetails_ForBid(BrandName, IsMedDept);
        }

        public void AutoDrug_Text_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            BidDetail obj = (sender as AutoCompleteBox).SelectedItem as BidDetail;
            if (gAddBidDetail != null)
            {
                gAddBidDetail = obj;
            }
        }

        private void GetRefGenMedProductDetails_ForBid(string BrandName, bool IsMedDept)
        {
            if (gSelectedBid == null)
            {
                MessageBox.Show(eHCMSResources.Z2113_G1_ChonDotThau, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
            var t = new Thread(() =>
            {
                using (var mServiceFactory = new PharmacyGenMedDeptDetailsServiceClient())
                {
                    var contract = mServiceFactory.ServiceInstance;
                    try
                    {
                        contract.BeginGetRefGenMedProductDetails_ForBid(gSelectedBid.BidID, BrandName, IsMedDept, V_MedProductType, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                List<BidDetail> tmpList = new List<BidDetail>();
                                List<BidDetail> mBidDetails = contract.EndGetRefGenMedProductDetails_ForBid(asyncResult);
                                tmpList = mBidDetails;
                                if (tmpList.Count > 0)
                                {
                                    au.ItemsSource = tmpList;
                                    au.PopulateComplete();
                                }
                                //▼===== 20191214 TTM: Clear dữ liệu drop down list nếu không tìm ra được dữ liệu cho autocomplete.
                                else
                                {
                                    if (tmpList != null)
                                    {
                                        tmpList.Clear();
                                        au.ItemsSource = tmpList;
                                        au.PopulateComplete();
                                    }
                                }
                                //▲===== 
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                            }
                            finally
                            {

                            }
                        }), null);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                    }
                }
            });

            t.Start();
        }
        /*▲====: #002*/

        public void BtnExportExcel()
        {
            if (gSelectedBid == null)
            {
                MessageBox.Show(eHCMSResources.Z2113_G1_ChonDotThau, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
            this.ShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
            var t = new Thread(() =>
            {
                using (var mServiceFactory = new PharmacyGenMedDeptDetailsServiceClient())
                {
                    var contract = mServiceFactory.ServiceInstance;
                    try
                    {
                        contract.BeginGetRefGenMedProductDetails_ForBid(gSelectedBid.BidID, BrandName, IsMedDept, V_MedProductType, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                List<BidDetail> tmpList = new List<BidDetail>();
                                List<BidDetail> mBidDetails = contract.EndGetRefGenMedProductDetails_ForBid(asyncResult);
                                tmpList = mBidDetails;
                                if (tmpList.Count > 0)
                                {
                                    au.ItemsSource = tmpList;
                                    au.PopulateComplete();
                                }
                                else
                                {
                                    if (tmpList != null)
                                    {
                                        tmpList.Clear();
                                        au.ItemsSource = tmpList;
                                        au.PopulateComplete();
                                    }
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
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                        this.HideBusyIndicator();
                    }
                }
            });

            t.Start();
        }
        #endregion
        //▼===== #005
        #region Kho
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
        private RefStorageWarehouseLocation _SelectedStorage;
        public RefStorageWarehouseLocation SelectedStorage
        {
            get { return _SelectedStorage; }
            set
            {
                if (_SelectedStorage != value)
                {
                    _SelectedStorage = value;
                    NotifyOfPropertyChange(() => SelectedStorage);
                }
            }
        }
        private IEnumerator<IResult> DoGetStore_ForBid()
        {
            if (IsMedDept)
            {
                var paymentTypeTask = new LoadStoreListTask((long)AllLookupValues.StoreType.STORAGE_DRUGDEPT, false, null, false, false, true);
                yield return paymentTypeTask;
                if (V_MedProductType == (long)AllLookupValues.MedProductType.HOA_CHAT)
                {
                    StoreCbx = paymentTypeTask.LookupList.Where(x => x.ListV_MedProductType.Contains(((long)AllLookupValues.MedProductType.HOA_CHAT).ToString()) && x.StoreTypeID == (long?)AllLookupValues.StoreType.STORAGE_DRUGDEPT).OrderBy(y => y.swhlName).ToObservableCollection();
                }
                else
                {
                    StoreCbx = paymentTypeTask.LookupList.Where(x => x.StoreTypeID == (long?)AllLookupValues.StoreType.STORAGE_DRUGDEPT).OrderBy(y => y.swhlName).ToObservableCollection();
                }
            }
            else
            {
                var paymentTypeTask = new LoadStoreListTask((long)AllLookupValues.StoreType.STORAGE_EXTERNAL, false, null, false, false, true);
                yield return paymentTypeTask;
                StoreCbx = paymentTypeTask.LookupList.Where(x => x.IsMain == true).ToObservableCollection();
            }
            if (StoreCbx != null)
            {
                SelectedStorage = StoreCbx.FirstOrDefault();
            }
            yield break;
        }
        #endregion
        //▲===== #005

        //▼====: #007
        public bool IsShowYear
        {
            get
            {
                return Globals.ServerConfigSection.CommonItems.ApplyReport130 
                    && (V_MedProductType == (long)AllLookupValues.MedProductType.THUOC || V_MedProductType == (long)AllLookupValues.MedProductType.Y_CU);
            }
        }

        public void SeparateListBeforeSave()
        {
            gAddBidDetails.Clear();
            gEditBidDetails.Clear();
            // lấy danh sách cập nhật
            foreach (BidDetail item in gBidDetails)
            {
                if (item.IsAddNew)
                {
                    gAddBidDetails.Add(item);
                }
                foreach (BidDetail itemOrg in gOrgBidDetails)
                {
                    if (item.BidDetailID == itemOrg.BidDetailID && !item.IsAddNew)
                    {
                        if ((item.Visa != itemOrg.Visa) || (item.HICode != itemOrg.HICode) || (item.ReportBrandName != itemOrg.ReportBrandName)
                            || (item.ApprovedQty != itemOrg.ApprovedQty) || ((item.Supplier != null && (item.Supplier.SupplierID != itemOrg.Supplier.SupplierID)))
                            || (item.InCost != itemOrg.InCost))
                        {
                            gEditBidDetails.Add(item.DeepCopy());
                        }
                    }
                }
            }
        }
        //▲====: #007
    }
}
