using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ServiceModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using aEMR.DataContracts;
using aEMR.Infrastructure;
using aEMR.ServiceClient;
using Caliburn.Micro;
using aEMR.ViewContracts;
using System.ComponentModel.Composition;
using DataEntities;
using aEMR.Common;
using aEMR.Common.Collections;
using aEMR.Common.BaseModel;
using aEMR.CommonTasks;
using System.Linq;
using eHCMSLanguage;
using aEMR.Infrastructure.GlobalFuncs;
using aEMR.Common.PagedCollectionView;
using Castle.Windsor;
/*
* 20170619 #001 CMN: Service for payment report OutPt with large data (Rollback)
*/
namespace aEMR.Registration.ViewModels
{
    [Export(typeof(IThongKeDoanhThuDangKy)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class ThongKeDoanhThuDangKyViewModel : ViewModelBase, IThongKeDoanhThuDangKy
    {
        [ImportingConstructor]
        public ThongKeDoanhThuDangKyViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg)
        {
            curReportPaymentReceiptByStaff = new ReportPaymentReceiptByStaff();
            aucHoldConsultDoctor = Globals.GetViewModel<IAucHoldConsultDoctor>();
            FromDate = Globals.GetViewModel<IMinHourDateControl>();
            FromDate.DateTime = Globals.GetCurServerDateTime();
            ToDate = Globals.GetViewModel<IMinHourDateControl>();
            ToDate.DateTime = Globals.GetCurServerDateTime();

            SearchCriteria = new SearchOutwardReport
            {
                IsReport = 0,
                IsDeleted = 0,
                V_TradingPlaces = (long)AllLookupValues.V_TradingPlaces.DANG_KY
            };

            Coroutine.BeginExecute(LoadStaffHavePaymentList());
            curReportPaymentReceiptByStaffSearch = new ReportPaymentReceiptByStaffSearchCriteria();

            //--▼--02/02/2021 DatTB Thêm PTTT vào select
            Lookup firstItem = new Lookup
            {
                LookupID = 0,
                ObjectValue = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.T0822_G1_TatCa)
            };

            AllPaymentMode = Globals.AllLookupValueList.Where(x => x.ObjectTypeID == (long)LookupValues.PAYMENT_MODE).ToObservableCollection();

            AllPaymentMode.Insert(0, firstItem);

            SetDefaultPaymentMode();
            //--▲--02/02/2021 DatTB Thêm PTTT vào select

            IsEnableRoleUser = PermissionManager.IsAdminUser();
            curStaff = Globals.LoggedUserAccount.Staff;
            //KMx: Nếu không để Ax.Infrastructure, thì chương trình không hiểu mình muốn lấy Globals từ Ax.Infrastructure hay từ System.Runtime.Serialization. (04/03/2014 09:42)
            if (!Globals.isAccountCheck)
            {
                isAllStaff = true;
            }
            //==== CMN: Add Group Report for AllStaff
            if (Globals.CheckFunction(Globals.listRefModule, (int)eModules.mTransaction_Management, (int)eTransaction_Management.mPaymentReport))
                isAllStaff = true;
            //====
        }

        //--▼--02/02/2021 DatTB 
        private ObservableCollection<Lookup> _AllPaymentMode;
        public ObservableCollection<Lookup> AllPaymentMode
        {
            get
            {
                return _AllPaymentMode;
            }
            set
            {
                if (_AllPaymentMode != value)
                {
                    _AllPaymentMode = value;
                    NotifyOfPropertyChange(() => AllPaymentMode);
                }
            }
        }

        private bool _IsShowPaymentMode;
        public bool IsShowPaymentMode
        {
            get
            {
                return _IsShowPaymentMode;
            }
            set
            {
                if (_IsShowPaymentMode == value)
                    return;
                _IsShowPaymentMode = value;
                NotifyOfPropertyChange(() => IsShowPaymentMode);
            }
        }

        private Lookup _V_PaymentMode;
        public Lookup V_PaymentMode
        {
            get
            {
                return _V_PaymentMode;
            }
            set
            {
                if (_V_PaymentMode == value)
                    return;
                _V_PaymentMode = value;
                NotifyOfPropertyChange(() => V_PaymentMode);
            }
        }

        private void SetDefaultPaymentMode()
        {
            if (AllPaymentMode != null)
            {
                V_PaymentMode = AllPaymentMode.FirstOrDefault();
            }
        }
        //--▲--02/02/2021 DatTB

        private IAucHoldConsultDoctor _aucHoldConsultDoctor;
        public IAucHoldConsultDoctor aucHoldConsultDoctor
        {
            get
            {
                return _aucHoldConsultDoctor;
            }
            set
            {
                if (_aucHoldConsultDoctor != value)
                {
                    _aucHoldConsultDoctor = value;
                    NotifyOfPropertyChange(() => aucHoldConsultDoctor);
                }
            }
        }

        private IMinHourDateControl _FromDate;
        public IMinHourDateControl FromDate
        {
            get { return _FromDate; }
            set
            {
                _FromDate = value;
                NotifyOfPropertyChange(() => _FromDate);
            }
        }

        private IMinHourDateControl _ToDate;
        public IMinHourDateControl ToDate
        {
            get { return _ToDate; }
            set
            {
                _ToDate = value;
                NotifyOfPropertyChange(() => ToDate);
            }
        }

        private Staff _curStaff;
        public Staff curStaff
        {
            get
            {
                return _curStaff;
            }
            set
            {
                if (_curStaff != value)
                {
                    _curStaff = value;
                    NotifyOfPropertyChange(() => curStaff);
                }
            }
        }

        private bool _isAllStaff;
        public bool isAllStaff
        {
            get
            {
                return _isAllStaff;
            }
            set
            {
                if (_isAllStaff != value)
                {
                    _isAllStaff = value;
                    NotifyOfPropertyChange(() => isAllStaff);
                }
            }
        }

        private bool _isAucStaff;
        public bool isAucStaff
        {
            get
            {
                return _isAucStaff;
            }
            set
            {
                if (_isAucStaff != value)
                {
                    _isAucStaff = value;
                    NotifyOfPropertyChange(() => isAucStaff);
                    if (isAucStaff)
                    {
                        SearchCriteria.StaffID = null;
                    }
                    else
                    {
                        SearchCriteria.StaffID = curStaff.StaffID;
                    }
                }
            }
        }

        private bool _isSearchAllStaff = false;
        public bool IsSearchAllStaff
        {
            get
            {
                return _isSearchAllStaff;
            }
            set
            {
                if (_isSearchAllStaff != value)
                {
                    _isSearchAllStaff = value;
                    NotifyOfPropertyChange(() => _isSearchAllStaff);
                }
            }
        }

        private bool _IsEnableRoleUser;
        public bool IsEnableRoleUser
        {
            get { return _IsEnableRoleUser; }
            set
            {
                _IsEnableRoleUser = value;
                NotifyOfPropertyChange(() => IsEnableRoleUser);
            }
        }

        private decimal _SumMoneyNotReported;
        public decimal SumMoneyNotReported
        {
            get { return _SumMoneyNotReported; }
            set
            {
                _SumMoneyNotReported = value;
                NotifyOfPropertyChange(() => SumMoneyNotReported);
            }
        }

        private decimal _DeletedMoneyNotReported;
        public decimal DeletedMoneyNotReported
        {
            get { return _DeletedMoneyNotReported; }
            set
            {
                _DeletedMoneyNotReported = value;
                NotifyOfPropertyChange(() => DeletedMoneyNotReported);
            }
        }

        private decimal _SumMoneyReported;
        public decimal SumMoneyReported
        {
            get { return _SumMoneyReported; }
            set
            {
                _SumMoneyReported = value;
                NotifyOfPropertyChange(() => SumMoneyReported);
            }
        }

        private decimal _DeletedSumMoneyReported;
        public decimal DeletedSumMoneyReported
        {
            get { return _DeletedSumMoneyReported; }
            set
            {
                _DeletedSumMoneyReported = value;
                NotifyOfPropertyChange(() => DeletedSumMoneyReported);
            }
        }
        private ObservableCollection<PatientTransactionPayment> PaymentXML;

        private ObservableCollection<PatientTransactionPayment> _allPatientPayment;
        public ObservableCollection<PatientTransactionPayment> allPatientPayment
        {
            get
            {
                return _allPatientPayment;
            }
            set
            {
                if (_allPatientPayment != value)
                {
                    _allPatientPayment = value;
                    NotifyOfPropertyChange(() => allPatientPayment);
                }
            }
        }

        private ObservableCollection<ReportOutPatientCashReceipt_Payments> _allReportOutPatientCashReceipt_Payments;
        public ObservableCollection<ReportOutPatientCashReceipt_Payments> allReportOutPatientCashReceipt_Payments
        {
            get
            {
                return _allReportOutPatientCashReceipt_Payments;
            }
            set
            {
                if (_allReportOutPatientCashReceipt_Payments != value)
                {
                    _allReportOutPatientCashReceipt_Payments = value;
                    NotifyOfPropertyChange(() => allReportOutPatientCashReceipt_Payments);
                }
            }
        }

        private PagedCollectionView _ReportOutPatientCashReceipts;
        public PagedCollectionView ReportOutPatientCashReceipts
        {
            get
            {
                return _ReportOutPatientCashReceipts;
            }
            set
            {
                if (_ReportOutPatientCashReceipts == value)
                    return;
                _ReportOutPatientCashReceipts = value;
                NotifyOfPropertyChange(() => ReportOutPatientCashReceipts);
            }
        }

        private ObservableCollection<Staff> _StaffList;
        public ObservableCollection<Staff> StaffList
        {
            get
            {
                return _StaffList;
            }
            set
            {
                if (_StaffList == value)
                    return;
                _StaffList = value;
                NotifyOfPropertyChange(() => StaffList);
            }
        }

        private SearchOutwardReport _SearchCriteria;
        public SearchOutwardReport SearchCriteria
        {
            get
            {
                return _SearchCriteria;
            }
            set
            {
                _SearchCriteria = value;
                NotifyOfPropertyChange(() => SearchCriteria);
            }
        }

        private bool _IsReported = false;
        public bool IsReported
        {
            get
            {
                return _IsReported;
            }
            set
            {
                _IsReported = value;
                NotifyOfPropertyChange(() => IsReported);
            }
        }

        private IEnumerator<IResult> LoadStaffHavePaymentList()
        {
            var paymentTypeTask = new LoadStaffHavePaymentListTask(false, true, (long)AllLookupValues.V_TradingPlaces.DANG_KY);
            yield return paymentTypeTask;
            StaffList = paymentTypeTask.StaffList;
            if (SearchCriteria != null)
            {
                SearchCriteria.StaffID = Globals.LoggedUserAccount.StaffID;
            }
            yield break;
        }

        private void GetReportOutPatientCashReceipt(SearchOutwardReport searchcriate)
        {
            //isLoadingSearch = true;
            if (SearchCriteria == null || SearchCriteria.StaffID == null)
            {
                MessageBox.Show(eHCMSResources.A0813_G1_Msg_InfoMaNhVienKhHopLe, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
            else
            {
                if (SearchCriteria.StaffID <= 0)
                {
                    if (isAllStaff && isAucStaff)
                    {
                        //KMx: -8998 là mã để tìm receipt của tất cả nhân viên đã đăng ký. (09/05/2014 17:52)
                        //Nếu StaffID > 0 thì tìm theo StaffID. Nếu StaffID = -8998 thì tìm tất cả. Nếu không thỏa 1 trong 2 điều kiện đó thì không tìm gì cả.
                        SearchCriteria.StaffID = -8998;
                    }
                    else
                    {
                        MessageBox.Show(eHCMSResources.A0813_G1_Msg_InfoMaNhVienKhHopLe, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                        return;
                    }
                }
            }

            /*▼====: #001*/
            var t = new Thread(() =>
            {
                try
                {
                    this.ShowBusyIndicator();
                    using (var serviceFactory = new CommonService_V2Client())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetReportOutPatientCashReceipt_TongHop(searchcriate, IsTongHop, Globals.LoggedUserAccount.StaffID.GetValueOrDefault(), Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                List<PatientTransactionPayment> outlst;
                                var results = contract.EndGetReportOutPatientCashReceipt_TongHop(out outlst, asyncResult);
                                allReportOutPatientCashReceipt_Payments = results.ToObservableCollection();
                                allPatientPayment = outlst.ToObservableCollection();
                                GetReportDetails();
                                if (LstPaymentUpdate != null)
                                {
                                    LstPaymentUpdate.Clear();
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
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
                    MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
                    this.HideBusyIndicator();
                }
            });

            //var t = new Thread(() =>
            //{
            //    using (var serviceFactory = new CommonServiceClient())
            //    {
            //        var contract = serviceFactory.ServiceInstance;
            //        contract.BeginGetReportOutPatientCashReceipt_TongHop_Async(searchcriate, IsTongHop, Globals.LoggedUserAccount.StaffID.GetValueOrDefault(), Globals.DispatchCallback((asyncResult) =>
            //        {
            //            try
            //            {
            //                List<PatientTransactionPayment> outlst;
            //                int AsyncKey;
            //                var results = contract.EndGetReportOutPatientCashReceipt_TongHop_Async(out outlst, out AsyncKey, asyncResult);
            //                allReportOutPatientCashReceipt_Payments = results.ToObservableCollection();
            //                if (AsyncKey == 0)
            //                {
            //                    allPatientPayment = outlst.ToObservableCollection();
            //                    GetReportDetails();
            //                    if (LstPaymentUpdate != null)
            //                    {
            //                        LstPaymentUpdate.Clear();
            //                    }
            //                    this.HideBusyIndicator();
            //                }
            //                else
            //                {
            //                    allPatientPayment = new ObservableCollection<PatientTransactionPayment>();
            //                    LoadMorePatientCashReceipt(AsyncKey);
            //                }
            //            }
            //            catch (Exception ex)
            //            {
            //                MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
            //                this.HideBusyIndicator();
            //            }
            //        }), null);

            //    }

            //});

            t.Start();
            /*▲====: #001*/
        }
        /*▼====: #001*/
        private void LoadMorePatientCashReceipt(int RefAsyncKey)
        {
            var t = new Thread(() =>
            {
                using (var serviceFactory = new CommonService_V2Client())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetMoreReportOutPatientCashReceipt_TongHop_Async(RefAsyncKey, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            List<PatientTransactionPayment> outlst;
                            int AsyncKey;
                            var results = contract.EndGetMoreReportOutPatientCashReceipt_TongHop_Async(out outlst, out AsyncKey, asyncResult);
                            foreach (var item in results)
                                allReportOutPatientCashReceipt_Payments.Add(item);
                            if (outlst != null)
                            {
                                foreach (var item in outlst)
                                    allPatientPayment.Add(item);
                            }
                            if (AsyncKey == 0)
                            {
                                GetReportDetails();
                                if (LstPaymentUpdate != null)
                                    LstPaymentUpdate.Clear();
                                this.HideBusyIndicator();
                            }
                            else
                                LoadMorePatientCashReceipt(AsyncKey);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
                            this.HideBusyIndicator();
                        }
                    }), null);
                }
            });

            t.Start();
        }
        /*▲====: #001*/
        private void GetReportDetails()
        {
            ReportOutPatientCashReceipts = new PagedCollectionView(allReportOutPatientCashReceipt_Payments);

            if (PaymentXML != null)
            {
                PaymentXML.Clear();
            }

            var lstreport = allPatientPayment.Where(x => x.Reported == false);
            if (lstreport != null)
            {
                SumMoneyNotReported = lstreport.Where(x => !x.IsDeleted.GetValueOrDefault()).Sum(x => x.PayAmount);
                DeletedMoneyNotReported = lstreport.Where(x => x.IsDeleted.GetValueOrDefault()).Sum(x => x.PayAmount);
                //lay danh sach de lam bao cao
                var lst = lstreport.Distinct();
                if (lst != null && lst.Count() > 0)
                {
                    if (PaymentXML == null)
                    {
                        PaymentXML = new ObservableCollection<PatientTransactionPayment>();
                    }
                    foreach (var ite in lst)
                    {
                        if (!PaymentXML.Any(x => x.PtTranPaymtID == ite.PtTranPaymtID && x.OutPtCashAdvanceID == ite.OutPtCashAdvanceID))
                        {
                            PaymentXML.Add(new PatientTransactionPayment() { PtTranPaymtID = ite.PtTranPaymtID, OutPtCashAdvanceID = ite.OutPtCashAdvanceID });
                        }
                    }
                }
            }

            if (PaymentXML == null || PaymentXML.Count == 0)
            {
                IsReported = false;
            }
            else
            {
                IsReported = true;
            }

            SumMoneyReported = allPatientPayment.Where(x => x.Reported == true && !x.IsDeleted.GetValueOrDefault()).Sum(x => x.PayAmount);
            DeletedSumMoneyReported = allPatientPayment.Where(x => x.Reported == true && x.IsDeleted.GetValueOrDefault()).Sum(x => x.PayAmount);

            ReportOutPatientCashReceipts.GroupDescriptions.Clear();
            ReportOutPatientCashReceipts.GroupDescriptions.Add(new Common.PagedCollectionView.PropertyGroupDescription("CurPatientTransactionPayment"));
        }

        private bool _IsTongHop = true;
        public bool IsTongHop
        {
            get { return _IsTongHop; }
            set
            {
                _IsTongHop = value;
                NotifyOfPropertyChange(() => IsTongHop);
                NotifyOfPropertyChange(() => IsChiTiet);
            }
        }

        public bool IsChiTiet
        {
            get { return !IsTongHop; }
        }

        public void rdtTongHop_Checked(object sender, RoutedEventArgs e)
        {
            IsTongHop = true;
        }

        public void rdtChiTiet_Checked(object sender, RoutedEventArgs e)
        {
            IsTongHop = false;
        }

        public void rdtAll_Checked(object sender, RoutedEventArgs e)
        {
            SearchCriteria.IsReport = 2;
        }

        public void rdtNoReport_Checked(object sender, RoutedEventArgs e)
        {
            SearchCriteria.IsReport = 0;
        }

        public void rdtReport_Checked(object sender, RoutedEventArgs e)
        {
            SearchCriteria.IsReport = 1;
        }

        public void rdtAllDeleted_Checked(object sender, RoutedEventArgs e)
        {
            SearchCriteria.IsDeleted = 2;
        }

        public void rdtNoDeleted_Checked(object sender, RoutedEventArgs e)
        {
            SearchCriteria.IsDeleted = 0;
        }

        public void rdtDeleted_Checked(object sender, RoutedEventArgs e)
        {
            SearchCriteria.IsDeleted = 1;
        }

        bool AllowReport = false;
        public void SearchCmd()
        {
            if (CheckDate())
            {
                if (SearchCriteria.IsDeleted == 2)
                {
                    AllowReport = false;
                }
                else
                {
                    AllowReport = true;
                }
                if (curReportPaymentReceiptByStaff == null)
                {
                    curReportPaymentReceiptByStaff = new ReportPaymentReceiptByStaff();
                }

                if (SearchCriteria.IsDeleted == 1)
                {
                    curReportPaymentReceiptByStaff.IsDeleted = true;
                }
                if (SearchCriteria.IsDeleted == 0)
                {
                    curReportPaymentReceiptByStaff.IsDeleted = false;
                }
                if (isAucStaff)
                {
                    SearchCriteria.StaffID = aucHoldConsultDoctor.StaffID;
                }

                if (SearchCriteria.StaffID < 1)
                {
                    aucHoldConsultDoctor.setDefault();
                }

                SearchCriteria.V_PaymentMode = V_PaymentMode.LookupID; //--02/02/2021 DatTB 

                //SearchCriteria.fromdate = FromDate.DateTime;
                SearchCriteria.todate = ToDate.DateTime;
                GetReportOutPatientCashReceipt(SearchCriteria);
            }
        }

        private bool CheckDate()
        {
            if (SearchCriteria == null)
            {
                return false;
            }
            if (SearchCriteria.fromdate == null || ToDate == null || ToDate.DateTime == null)
            {
                MessageBox.Show(eHCMSResources.K0426_G1_NhapDayDuNgTh);
                return false;
            }
            else
            {
                if (eHCMS.Services.Core.AxHelper.CompareDate(SearchCriteria.fromdate.GetValueOrDefault(), (DateTime)ToDate.DateTime) == 1)
                {
                    MessageBox.Show(eHCMSResources.A0856_G1_Msg_InfoNgThangKhHopLe);
                    return false;
                }
                else if (ToDate.DateTime > Globals.GetCurServerDateTime())
                {
                    MessageBox.Show(eHCMSResources.A0856_G1_Msg_InfoNgThangKhHopLe);
                    return false;
                }
            }
            return true;
        }

        private ReportPaymentReceiptByStaff _curReportPaymentReceiptByStaff;
        public ReportPaymentReceiptByStaff curReportPaymentReceiptByStaff
        {
            get
            {
                return _curReportPaymentReceiptByStaff;
            }
            set
            {
                if (_curReportPaymentReceiptByStaff != value)
                {
                    _curReportPaymentReceiptByStaff = value;
                    NotifyOfPropertyChange(() => curReportPaymentReceiptByStaff);
                }
            }
        }

        public void btnUpdateNote()
        {
            if (LstPaymentUpdate != null && LstPaymentUpdate.Count > 0)
            {
                PatientTransactionPayment_UpdateNote(LstPaymentUpdate);
            }
        }

        public void ReportPaymentCmd()
        {
            if (CheckDate())
            {
                if (AllowReport)
                {
                    var item = PaymentXML.Where(x => x.Reported == true);
                    if (item != null && item.Count() > 0)
                    {
                        MessageBox.Show(eHCMSResources.Z0513_G1_KgBCLaiPhDaBC);
                        return;
                    }
                    if (string.IsNullOrEmpty(curReportPaymentReceiptByStaff.RepTittle))
                    {
                        MessageBox.Show(eHCMSResources.K0453_G1_NhapTieuDeBC);
                        return;
                    }

                    // 20191226 TNHX: Bỏ kiểm tra dữ liệu này
                    //if (string.IsNullOrEmpty(curReportPaymentReceiptByStaff.ReceiptIssueMauSo))
                    //{
                    //    MessageBox.Show(eHCMSResources.K0437_G1_NhapMauSoBC);
                    //    return;
                    //}

                    //if (string.IsNullOrEmpty(curReportPaymentReceiptByStaff.ReceiptIssueKyHieu))
                    //{
                    //    MessageBox.Show(eHCMSResources.K0436_G1_NhapKiHieuBC);
                    //    return;
                    //}

                    //if (string.IsNullOrEmpty(curReportPaymentReceiptByStaff.ReceiptNumberFrom))
                    //{
                    //    MessageBox.Show(eHCMSResources.K0447_G1_NhapSoTu);
                    //    return;
                    //}

                    //if (string.IsNullOrEmpty(curReportPaymentReceiptByStaff.ReceiptNumberTo))
                    //{
                    //    MessageBox.Show(eHCMSResources.K0441_G1_NhapSoDen);
                    //    return;
                    //}

                    curReportPaymentReceiptByStaff.RepFromDate = SearchCriteria.fromdate.GetValueOrDefault();
                    curReportPaymentReceiptByStaff.RepToDate = ToDate.DateTime;
                    if (curReportPaymentReceiptByStaff.StaffID == 0)
                    {
                        curReportPaymentReceiptByStaff.StaffID = (long)Globals.LoggedUserAccount.StaffID;
                    }
                    AddReportPaymentReceiptByStaff(curReportPaymentReceiptByStaff, PaymentXML);
                }
                else
                {
                    MessageBox.Show(eHCMSResources.K0274_G1_BCPhHuyRieng);
                }
            }
        }

        public void AddReportPaymentReceiptByStaff(ReportPaymentReceiptByStaff curRPtRByStaff
                                          , ObservableCollection<PatientTransactionPayment> allPayment)
        {
            this.ShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new CommonService_V2Client())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginAddReportPaymentReceiptByStaff(curRPtRByStaff, allPayment,
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                IList<PatientTransactionPayment> allItems = new ObservableCollection<PatientTransactionPayment>();
                                try
                                {
                                    long RepPaymentRecvID = 0;
                                    var res = contract.EndAddReportPaymentReceiptByStaff(out RepPaymentRecvID, asyncResult);
                                    curReportPaymentReceiptByStaff = new ReportPaymentReceiptByStaff();
                                    curRPtRByStaff.RepPaymentRecvID = RepPaymentRecvID;
                                    print(curRPtRByStaff);
                                    SearchCmd();
                                }
                                catch (FaultException<AxException> fault)
                                {
                                    ClientLoggerHelper.LogInfo(fault.ToString());
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

        public void PatientTransactionPayment_UpdateNote(ObservableCollection<PatientTransactionPayment> allPayment)
        {
            this.ShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new CommonService_V2Client())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginPatientTransactionPayment_UpdateNote(allPayment,
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                try
                                {
                                    var res = contract.EndPatientTransactionPayment_UpdateNote(asyncResult);
                                    if (res)
                                    {
                                        MessageBox.Show(eHCMSResources.A0279_G1_Msg_InfoCNhatOK);
                                        SearchCmd();
                                    }
                                }
                                catch (FaultException<AxException> fault)
                                {
                                    ClientLoggerHelper.LogInfo(fault.ToString());
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

        public void print(ReportPaymentReceiptByStaff curReportPaymentReceiptByStaff)
        {
            void onInitDlg(ICommonPreviewView proAlloc)
            {
                proAlloc.TieuDeRpt = eHCMSResources.Z0123_G1_BCThuTienHangNg
                    + ((DateTime)curReportPaymentReceiptByStaff.RepFromDate).Month.ToString()
                    + "/" + ((DateTime)curReportPaymentReceiptByStaff.RepToDate).Year.ToString();
                proAlloc.ID = curReportPaymentReceiptByStaff.RepPaymentRecvID;
                if (type == 0)
                {
                    if (curReportPaymentReceiptByStaff.IsDeleted)
                    {
                        proAlloc.eItem = ReportName.REGISTRATION_HUY_HOADON;
                    }
                    else
                    {
                        proAlloc.eItem = ReportName.PHIEUTHUTIENTONGHOP;
                        proAlloc.RepPaymentRecvID = curReportPaymentReceiptByStaff.RepPaymentRecvID;
                        proAlloc.StaffName = curStaff.FullName;
                    }
                }
                else if (type == 1)
                {
                    if (curReportPaymentReceiptByStaff.IsDeleted)
                    {
                        proAlloc.eItem = ReportName.REGISTRATION_HUY_HOADON_CHITIET;
                    }
                    else
                    {
                        proAlloc.eItem = ReportName.RptThuTienHangNgay;
                    }
                }
            }
            GlobalsNAV.ShowDialog<ICommonPreviewView>(onInitDlg);
        }

        public void hplDelete_Click(object sender)
        {
            if (MessageBox.Show(eHCMSResources.A0118_G1_Msg_ConfXoaDong, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                PatientTransactionPayment test = sender as PatientTransactionPayment;
                if (test != null)
                {
                    allPatientPayment.Remove(test);

                    allReportOutPatientCashReceipt_Payments = allReportOutPatientCashReceipt_Payments.Where(x => x.CurPatientTransactionPayment.PtTranPaymtID != test.PtTranPaymtID).ToObservableCollection();
                    GetReportDetails();
                }
                //allTempPatientPayment.Add(SelectedPatientPayment);
                //allPatientPayment.Remove(SelectedPatientPayment);
                //calSumPayment();
            }
        }

        private ObservableCollection<PatientTransactionPayment> LstPaymentUpdate;
        public void gridPayment_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (LstPaymentUpdate == null)
            {
                LstPaymentUpdate = new ObservableCollection<PatientTransactionPayment>();
            }
            PatientTransactionPayment item = e.Row.DataContext as PatientTransactionPayment;
            if (item != null)
            {
                var val = LstPaymentUpdate.Where(x => x.PtTranPaymtID == item.PtTranPaymtID);
                if (val != null && val.Count() > 0)
                {
                    return;
                }
                else
                {
                    LstPaymentUpdate.Add(item);
                }
            }
        }

        private ReportPaymentReceiptByStaffSearchCriteria _curReportPaymentReceiptByStaffSearch;
        public ReportPaymentReceiptByStaffSearchCriteria curReportPaymentReceiptByStaffSearch
        {
            get
            {
                return _curReportPaymentReceiptByStaffSearch;
            }
            set
            {
                if (_curReportPaymentReceiptByStaffSearch != value)
                {
                    _curReportPaymentReceiptByStaffSearch = value;
                    NotifyOfPropertyChange(() => curReportPaymentReceiptByStaffSearch);
                }
            }
        }

        public void SearchReportCmd()
        {
            GetReportPaymentReceiptByStaff(curReportPaymentReceiptByStaffSearch.FromDate, curReportPaymentReceiptByStaffSearch.ToDate);
        }

        public void RepPhiKhamBenhCDHACmd()
        {
            ReportPatientFeeByCat(true);
        }

        public void RepPhiXNCmd()
        {
            ReportPatientFeeByCat(false);
        }

        private void ReportPatientFeeByCat(bool isCDHARep)
        {
            if (AllReportGrid != null)
            {
                if (AllReportGrid.SelectedIndex < 0)
                {
                    MessageBox.Show(eHCMSResources.Z0522_G1_ChonBC);
                    return;
                }
                ReportPaymentReceiptByStaff selReport = allReportPaymentReceiptByStaff[AllReportGrid.SelectedIndex];
                Action<ICommonPreviewView> onInitDlg = delegate (ICommonPreviewView comRepPreviewVm)
                {
                    if (isCDHARep)
                    {
                        comRepPreviewVm.eItem = ReportName.BANGLETHUPHI_KB_CDHA_THEONGAY;
                    }
                    else
                    {
                        comRepPreviewVm.eItem = ReportName.BANGKETHUPHI_XN_THEONGAY;
                    }

                    if (selReport.staff.FullName != null && selReport.staff.FullName.Length > 0)
                    {
                        comRepPreviewVm.StaffName = selReport.staff.FullName;
                    }
                    else
                    {
                        // This is JUST IN CASE selReport.staff.FullName is NULL or EMPTY WHICH IS A BUG
                        comRepPreviewVm.StaffName = Globals.LoggedUserAccount.Staff.FullName;
                    }
                    comRepPreviewVm.TieuDeRpt = string.Format(" {0}: [{1}] - {2}: [{3}]", eHCMSResources.Z0236_G1_TuPhieuSo, (selReport.ReceiptNumberFrom.Length > 0 ? selReport.ReceiptNumberFrom : "---")
                                            , eHCMSResources.Z0237_G1_DenSo, (selReport.ReceiptNumberTo.Length > 0 ? selReport.ReceiptNumberTo : "---"));

                    comRepPreviewVm.FromDate = selReport.RepFromDate;
                    comRepPreviewVm.ToDate = selReport.RepToDate;
                    comRepPreviewVm.StaffID = selReport.StaffID;
                    comRepPreviewVm.RepPaymtRecptByStaffID = selReport.RepPaymentRecvID;
                };
                GlobalsNAV.ShowDialog(onInitDlg);
            }
        }

        DataGrid AllReportGrid = null;

        private ObservableCollection<ReportPaymentReceiptByStaff> _allReportPaymentReceiptByStaff;
        public ObservableCollection<ReportPaymentReceiptByStaff> allReportPaymentReceiptByStaff
        {
            get
            {
                return _allReportPaymentReceiptByStaff;
            }
            set
            {
                if (_allReportPaymentReceiptByStaff != value)
                {
                    _allReportPaymentReceiptByStaff = value;
                    NotifyOfPropertyChange(() => allReportPaymentReceiptByStaff);
                }
            }
        }

        public void GetReportPaymentReceiptByStaff(DateTime FromDate, DateTime ToDate)
        {
            var t = new Thread(() =>
            {
                this.ShowBusyIndicator();
                try
                {
                    using (var serviceFactory = new CommonService_V2Client())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        //KMx: Nếu là admin thì không cần lọc theo StaffID để xem báo cáo của tất cả mọi người. Ngược lại thì chỉ xem được báo cáo của người đang đăng nhập (04/03/2013 9:17).
                        bool IsFilterByStaffID = true;
                        if (IsSearchAllStaff)
                        {
                            IsFilterByStaffID = false;
                        }
                        contract.BeginGetReportPaymentReceiptByStaff(FromDate, ToDate, IsFilterByStaffID, Globals.LoggedUserAccount.Staff.StaffID, Globals.LoggedUserAccount.StaffID.GetValueOrDefault(),
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                IList<ReportPaymentReceiptByStaff> allItems = new ObservableCollection<ReportPaymentReceiptByStaff>();
                                try
                                {
                                    allItems = contract.EndGetReportPaymentReceiptByStaff(asyncResult);
                                }
                                catch (FaultException<AxException> fault)
                                {
                                    ClientLoggerHelper.LogInfo(fault.ToString());
                                }
                                finally
                                {
                                    this.HideBusyIndicator();
                                }

                                allReportPaymentReceiptByStaff = new ObservableCollection<ReportPaymentReceiptByStaff>(allItems);
                                NotifyOfPropertyChange(() => allReportPaymentReceiptByStaff);

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

        private int type = 0;
        public void hplPrint_Click(object sender)
        {
            //Xuat Report
            type = 0;
            print(sender as ReportPaymentReceiptByStaff);
        }

        public void hplPrintDetails_Click(object sender)
        {
            type = 1;
            print(sender as ReportPaymentReceiptByStaff);
        }

        public void gridReport_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            if (sender == null)
                return;
            AllReportGrid = (DataGrid)sender;
            e.Row.Header = (e.Row.GetIndex() + 1).ToString() + ". ";
            ReportPaymentReceiptByStaff item = e.Row.DataContext as ReportPaymentReceiptByStaff;
            if (item != null && item.IsDeleted)
            {
                e.Row.Background = new SolidColorBrush(Colors.Yellow);
                e.Row.Foreground = new SolidColorBrush(Colors.Black);
            }
            else
            {
                e.Row.Background = new SolidColorBrush(Colors.Transparent);
                e.Row.Foreground = new SolidColorBrush(Colors.Black);
            }
        }

        public void gridRegistrations_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString() + ". ";

            ReportOutPatientCashReceipt_Payments objRows = e.Row.DataContext as ReportOutPatientCashReceipt_Payments;
            if (objRows != null)
            {
                e.Row.Foreground = new SolidColorBrush(Colors.Black);
                if (SearchCriteria.IsDeleted == 2)
                {
                    switch (objRows.CurPatientTransactionPayment.IsDeleted)
                    {
                        case true:
                            {
                                e.Row.Foreground = new SolidColorBrush(Colors.Red);
                                break;
                            }
                    }
                }
            }
        }

        public void gridPayment_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString() + ". ";
            PatientTransactionPayment objRows = e.Row.DataContext as PatientTransactionPayment;
            if (objRows != null)
            {
                e.Row.Foreground = new SolidColorBrush(Colors.Black);
                if (SearchCriteria.IsDeleted == 2)
                {
                    switch (objRows.IsDeleted)
                    {
                        case true:
                            {
                                e.Row.Foreground = new SolidColorBrush(Colors.Red);
                                break;
                            }
                    }
                }
            }
        }

        public void btnPaymentPrintHI(object sender)
        {
            GlobalsNAV.ShowDialog<ICommonPreviewView>((mView) =>
            {
                mView.ID = (sender as ReportPaymentReceiptByStaff).RepPaymentRecvID;
                mView.eItem = ReportName.BAO_CAO_VIEN_PHI_BHYT;
            });
        }

        public void btnPaymentPrint(object sender)
        {
            GlobalsNAV.ShowDialog<ICommonPreviewView>((mView) =>
            {
                mView.ID = (sender as ReportPaymentReceiptByStaff).RepPaymentRecvID;
                mView.eItem = ReportName.BAO_CAO_VIEN_PHI_NGOAI_TRU;
            });
        }

        //▼====== 20190114 TTM: Thêm sự kiện để ngăn chặn người dùng click liên tục => tạo nhiều báo cáo làm báo cáo tổng tiền trong ngày bị sai
        public void ReportPaymentCmd_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ClickCount > 1)
            {
                MessageBox.Show(eHCMSResources.Z2450_G1_KhongDuocClick2Lan);
                return;
            }
            ReportPaymentCmd();
        }
    }
}
