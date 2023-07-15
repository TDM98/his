using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ServiceModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using aEMR.DataContracts;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using Caliburn.Micro;
using aEMR.ViewContracts;
using System.ComponentModel.Composition;
using DataEntities;
using ResourcesManagementProxy;
using aEMR.Common;
using System.Linq;
using eHCMSLanguage;
using Castle.Windsor;

namespace aEMR.Registration.ViewModels
{
    [Export(typeof(IBaoCaoThuTien))]
    public class BaoCaoThuTienViewModel : Conductor<object>, IBaoCaoThuTien
    {
        IEventAggregator _eventArg;
        [ImportingConstructor]
        public BaoCaoThuTienViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg)
        {
            _eventArg = eventArg;
            var searchPatientAndRegVm = Globals.GetViewModel<ISearchPatientAndRegistration>();
            curPatientPaymentSearch = new PatientPaymentSearchCriteria();
            allOrgPatientPayment = new ObservableCollection<PatientTransactionPayment>();
            allTempPatientPayment = new ObservableCollection<PatientTransactionPayment>();
            curReportPaymentReceiptByStaff = new ReportPaymentReceiptByStaff();
            curReportPaymentReceiptByStaffSearch = new ReportPaymentReceiptByStaffSearchCriteria();
            GetStaffCategoriesByType();
            //GetStaffCategoriesByType((long)V_StaffCatType.NhanVienQuayDangKy);
            aucHoldConsultDoctor = Globals.GetViewModel<IAucHoldConsultDoctor>();
            curStaff = Globals.LoggedUserAccount.Staff;
            if (!Globals.isAccountCheck)
            {
                isAllStaff = true;
            }
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

        #region properties


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
                }
            }
        }

        private decimal _PaymentSum;
        public decimal PaymentSum
        {
            get
            {
                return _PaymentSum;
            }
            set
            {
                if (_PaymentSum != value)
                {
                    _PaymentSum = value;
                    NotifyOfPropertyChange(() => PaymentSum);
                }
            }
        }

        private bool _IsEdit = true;
        public bool IsEdit
        {
            get
            {
                return _IsEdit;
            }
            set
            {
                if (_IsEdit != value)
                {
                    _IsEdit = value;
                    IsUpdate = !IsEdit;
                    NotifyOfPropertyChange(() => IsEdit);
                }
            }
        }

        private bool _IsUpdate;
        public bool IsUpdate
        {
            get
            {
                return _IsUpdate;
            }
            set
            {
                if (_IsUpdate != value)
                {
                    _IsUpdate = value;
                    NotifyOfPropertyChange(() => IsUpdate);
                }
            }
        }

        private bool _IsLoading;
        public bool IsLoading
        {
            get
            {
                return IsLoading1 || IsLoading2 || IsLoading3 || IsLoading4 || IsLoading5;
            }
            set
            {
                if (_IsLoading != value)
                {
                    _IsLoading = IsLoading1 || IsLoading2 || IsLoading3 || IsLoading4 || IsLoading5;
                    NotifyOfPropertyChange(() => IsLoading);
                }
            }
        }

        private bool _IsLoading1;
        public bool IsLoading1
        {
            get
            {
                return _IsLoading1;
            }
            set
            {
                if (_IsLoading1 != value)
                {
                    _IsLoading1 = value;
                    NotifyOfPropertyChange(() => IsLoading1);
                    NotifyOfPropertyChange(() => IsLoading);
                }
            }
        }

        private bool _IsLoading2;
        public bool IsLoading2
        {
            get
            {
                return _IsLoading2;
            }
            set
            {
                if (_IsLoading2 != value)
                {
                    _IsLoading2 = value;
                    NotifyOfPropertyChange(() => IsLoading2);
                    NotifyOfPropertyChange(() => IsLoading);
                }
            }
        }

        private bool _IsLoading3;
        public bool IsLoading3
        {
            get
            {
                return _IsLoading3;
            }
            set
            {
                if (_IsLoading3 != value)
                {
                    _IsLoading3 = value;
                    NotifyOfPropertyChange(() => IsLoading3);
                    NotifyOfPropertyChange(() => IsLoading);
                }
            }
        }

        private bool _IsLoading4;
        public bool IsLoading4
        {
            get
            {
                return _IsLoading4;
            }
            set
            {
                if (_IsLoading4 != value)
                {
                    _IsLoading4 = value;
                    NotifyOfPropertyChange(() => IsLoading4);
                    NotifyOfPropertyChange(() => IsLoading);
                }
            }
        }

        private bool _IsLoading5;
        public bool IsLoading5
        {
            get
            {
                return _IsLoading5;
            }
            set
            {
                if (_IsLoading5 != value)
                {
                    _IsLoading5 = value;
                    NotifyOfPropertyChange(() => IsLoading5);
                    NotifyOfPropertyChange(() => IsLoading);
                }
            }
        }

        private PatientPaymentSearchCriteria _curPatientPaymentSearch;
        public PatientPaymentSearchCriteria curPatientPaymentSearch
        {
            get
            {
                return _curPatientPaymentSearch;
            }
            set
            {
                if (_curPatientPaymentSearch != value)
                {
                    _curPatientPaymentSearch = value;
                    NotifyOfPropertyChange(() => curPatientPaymentSearch);
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
                    calSumPayment();
                    NotifyOfPropertyChange(() => allPatientPayment);
                }
            }
        }

        private ObservableCollection<PatientTransactionPayment> _allTempPatientPayment;
        public ObservableCollection<PatientTransactionPayment> allTempPatientPayment
        {
            get
            {
                return _allTempPatientPayment;
            }
            set
            {
                if (_allTempPatientPayment != value)
                {
                    _allTempPatientPayment = value;
                    NotifyOfPropertyChange(() => allTempPatientPayment);
                }
            }
        }

        private ObservableCollection<PatientTransactionPayment> _allOrgPatientPayment;
        public ObservableCollection<PatientTransactionPayment> allOrgPatientPayment
        {
            get
            {
                return _allOrgPatientPayment;
            }
            set
            {
                if (_allOrgPatientPayment != value)
                {
                    _allOrgPatientPayment = value;
                    NotifyOfPropertyChange(() => allOrgPatientPayment);
                }
            }
        }

        private PatientTransactionPayment _SelectedPatientPayment;
        public PatientTransactionPayment SelectedPatientPayment
        {
            get
            {
                return _SelectedPatientPayment;
            }
            set
            {
                if (_SelectedPatientPayment != value)
                {
                    _SelectedPatientPayment = value;
                    NotifyOfPropertyChange(() => SelectedPatientPayment);

                }
            }
        }

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

        private ReportPaymentReceiptByStaff _SelectedReportPaymentReceiptByStaff;
        public ReportPaymentReceiptByStaff SelectedReportPaymentReceiptByStaff
        {
            get
            {
                return _SelectedReportPaymentReceiptByStaff;
            }
            set
            {
                if (_SelectedReportPaymentReceiptByStaff != value)
                {
                    _SelectedReportPaymentReceiptByStaff = value;
                    NotifyOfPropertyChange(() => SelectedReportPaymentReceiptByStaff);

                }
            }
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


        private ObservableCollection<Staff> _allStaff;
        public ObservableCollection<Staff> allStaff
        {
            get
            {
                return _allStaff;
            }
            set
            {
                if (_allStaff != value)
                {
                    _allStaff = value;
                    NotifyOfPropertyChange(() => allStaff);
                }
            }
        }


        private ObservableCollection<ReportPaymentReceiptByStaffDetails> _allReportPaymentReceiptByStaffDetails;
        public ObservableCollection<ReportPaymentReceiptByStaffDetails> allReportPaymentReceiptByStaffDetails
        {
            get
            {
                return _allReportPaymentReceiptByStaffDetails;
            }
            set
            {
                if (_allReportPaymentReceiptByStaffDetails != value)
                {
                    _allReportPaymentReceiptByStaffDetails = value;
                    NotifyOfPropertyChange(() => allReportPaymentReceiptByStaffDetails);
                }
            }
        }

        private ReportPaymentReceiptByStaffDetails _SelectedReportPaymentReceiptByStaffDetails;
        public ReportPaymentReceiptByStaffDetails SelectedReportPaymentReceiptByStaffDetails
        {
            get
            {
                return _SelectedReportPaymentReceiptByStaffDetails;
            }
            set
            {
                if (_SelectedReportPaymentReceiptByStaffDetails != value)
                {
                    _SelectedReportPaymentReceiptByStaffDetails = value;
                    NotifyOfPropertyChange(() => SelectedReportPaymentReceiptByStaffDetails);
                }
            }
        }
        #endregion

        int FindPatient = -1;
        public void rdtNgoaiTru_Checked(object sender, RoutedEventArgs e)
        {
            FindPatient = (int)AllLookupValues.V_FindPatientType.NGOAI_TRU;
        }

        public void rdtNoiTru_Checked(object sender, RoutedEventArgs e)
        {
            FindPatient = (int)AllLookupValues.V_FindPatientType.NOI_TRU;
        }

        public void rdtTatCa_Checked(object sender, RoutedEventArgs e)
        {
            FindPatient = -1;
        }

        public void SearchCmd()
        {
            IsEdit = true;
            curReportPaymentReceiptByStaff.RepTittle = "";
            curReportPaymentReceiptByStaff.RepNumCode = "";
            curReportPaymentReceiptByStaff.StaffID = aucHoldConsultDoctor.StaffID;
            GetPatientPaymentByDay(curPatientPaymentSearch, FindPatient);
        }

        public void ResetFilterCmd()
        {
            curPatientPaymentSearch = new PatientPaymentSearchCriteria();
        }

        public void ResetFilterRepCmd()
        {
            curReportPaymentReceiptByStaffSearch = new ReportPaymentReceiptByStaffSearchCriteria();
        }

        public void updateCmd()
        {
            ObservableCollection<System.ComponentModel.DataAnnotations.ValidationResult> validationResults;
            if (!SelectedReportPaymentReceiptByStaff.ValidateReportPaymentReceiptByStaff(SelectedReportPaymentReceiptByStaff, out validationResults))
            {
                return;
            }
            SelectedReportPaymentReceiptByStaff.RepTittle = curReportPaymentReceiptByStaff.RepTittle;
            SelectedReportPaymentReceiptByStaff.RepNumCode = curReportPaymentReceiptByStaff.RepNumCode;

            UpdateReportPaymentReceiptByStaff(SelectedReportPaymentReceiptByStaff);
        }

        public void SearchReportCmd()
        {
            GetReportPaymentReceiptByStaff(curReportPaymentReceiptByStaffSearch.FromDate, curReportPaymentReceiptByStaffSearch.ToDate);
        }

        public void BackCmd()
        {
            if (allTempPatientPayment.Count > 0)
            {
                allPatientPayment.Add(allTempPatientPayment[allTempPatientPayment.Count - 1]);
                allTempPatientPayment.RemoveAt(allTempPatientPayment.Count - 1);
                calSumPayment();
            }
        }

        public void ResetPageCmd()
        {
            allPatientPayment = allOrgPatientPayment;
            NotifyOfPropertyChange(() => allPatientPayment);
            calSumPayment();
        }

        public void calSumPayment()
        {
            PaymentSum = 0;
            if (allPatientPayment != null)
            {
                PaymentSum = allPatientPayment.Where(x => !x.IsDeleted.GetValueOrDefault(false)).Sum(x => x.PayAmount * x.CreditOrDebit);
            }
        }

        public void SaveAndPrint()
        {
            curReportPaymentReceiptByStaff.RepFromDate = curPatientPaymentSearch.FromDate;
            curReportPaymentReceiptByStaff.RepToDate = curPatientPaymentSearch.ToDate;
            if (curReportPaymentReceiptByStaff.StaffID == 0)
            {
                curReportPaymentReceiptByStaff.StaffID = (long)Globals.LoggedUserAccount.StaffID;
            }
            AddReportPaymentReceiptByStaff(curReportPaymentReceiptByStaff
                                           , allPatientPayment);
        }

        public void hplDelete_Click(object sender)
        {
            if (MessageBox.Show(eHCMSResources.A0164_G1_Msg_ConfXoaSth, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                allTempPatientPayment.Add(SelectedPatientPayment);
                allPatientPayment.Remove(SelectedPatientPayment);
                calSumPayment();
            }
        }

        public Button lnkDelete { get; set; }

        public void lnkDelete_Loaded(object sender)
        {
            lnkDelete = (Button)sender;
            lnkDelete.Visibility = Globals.convertVisibility(IsEdit);
        }

        public void hplPrint_Click(object sender)
        {
            //Xuat Report
            print(SelectedReportPaymentReceiptByStaff);
        }

        public void print(ReportPaymentReceiptByStaff curReportPaymentReceiptByStaff)
        {
            void onInitDlg(ICommonPreviewView proAlloc)
            {
                proAlloc.TieuDeRpt = eHCMSResources.Z0123_G1_BCThuTienHangNg
                + ((DateTime)curReportPaymentReceiptByStaff.RepFromDate).Month.ToString()
                + "/" + ((DateTime)curReportPaymentReceiptByStaff.RepToDate).Year.ToString();
                proAlloc.ID = curReportPaymentReceiptByStaff.RepPaymentRecvID;
                proAlloc.eItem = ReportName.RptThuTienHangNgay;
            }
            GlobalsNAV.ShowDialog<ICommonPreviewView>(onInitDlg);
        }

        public void hplPrint_Loaded(object sender)
        {

        }

        public void DoubleClick(object sender)
        {
            IsEdit = true;
            curReportPaymentReceiptByStaff.RepTittle = SelectedReportPaymentReceiptByStaff.RepTittle;
            curReportPaymentReceiptByStaff.RepNumCode = SelectedReportPaymentReceiptByStaff.RepNumCode;
            GetReportPaymentReceiptByStaffDetails(SelectedReportPaymentReceiptByStaff.RepPaymentRecvID);
        }

        #region function

        public void GetPatientPaymentByDay(PatientPaymentSearchCriteria PatientPaymentSearch, int FindPatient)
        {
            IsLoading1 = true;

            var t = new Thread(() =>
            {
                Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.Z0124_G1_DangLayDSTThaiDK) });
                try
                {
                    using (var serviceFactory = new CommonService_V2Client())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetPatientPaymentByDay_New(PatientPaymentSearch, FindPatient,
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                IList<PatientTransactionPayment> allItems = new ObservableCollection<PatientTransactionPayment>();
                                try
                                {
                                    allItems = contract.EndGetPatientPaymentByDay_New(asyncResult);
                                }
                                catch (FaultException<AxException> fault)
                                {
                                    ClientLoggerHelper.LogInfo(fault.ToString());
                                }
                                finally
                                {
                                    IsLoading1 = false;
                                }

                                allPatientPayment = new ObservableCollection<PatientTransactionPayment>(allItems);
                                allOrgPatientPayment = new ObservableCollection<PatientTransactionPayment>(allItems);
                                allTempPatientPayment = new ObservableCollection<PatientTransactionPayment>();
                                NotifyOfPropertyChange(() => allPatientPayment);
                            }), null);
                    }
                }
                catch (Exception ex)
                {
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                }
                finally
                {
                    Globals.IsBusy = false;
                    IsLoading1 = false;
                }
            });

            t.Start();
        }

        public void GetReportPaymentReceiptByStaff(DateTime FromDate, DateTime ToDate)
        {
            IsLoading3 = true;
            var t = new Thread(() =>
            {
                Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.Z0124_G1_DangLayDSTThaiDK) });
                try
                {
                    using (var serviceFactory = new CommonService_V2Client())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginGetReportPaymentReceiptByStaff(FromDate, ToDate, false, 0, Globals.LoggedUserAccount.StaffID.GetValueOrDefault(),
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
                                    IsLoading3 = false;
                                }
                                allReportPaymentReceiptByStaff = new ObservableCollection<ReportPaymentReceiptByStaff>(allItems);
                                NotifyOfPropertyChange(() => allReportPaymentReceiptByStaff);
                            }), null);
                    }
                }
                catch (Exception ex)
                {
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                }
                finally
                {
                    Globals.IsBusy = false;
                    IsLoading3 = false;
                }
            });

            t.Start();
        }

        public void AddReportPaymentReceiptByStaff(ReportPaymentReceiptByStaff curReportPaymentReceiptByStaff
                                            , ObservableCollection<PatientTransactionPayment> allPayment)
        {
            IsLoading2 = true;
            var t = new Thread(() =>
            {
                Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.T0432_G1_Error });
                try
                {
                    using (var serviceFactory = new CommonService_V2Client())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginAddReportPaymentReceiptByStaff(curReportPaymentReceiptByStaff, allPayment,
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                IList<PatientTransactionPayment> allItems = new ObservableCollection<PatientTransactionPayment>();
                                try
                                {
                                    long RepPaymentRecvID = 0;
                                    var res = contract.EndAddReportPaymentReceiptByStaff(out RepPaymentRecvID, asyncResult);
                                    curReportPaymentReceiptByStaff.RepPaymentRecvID = RepPaymentRecvID;
                                    print(curReportPaymentReceiptByStaff);
                                }
                                catch (FaultException<AxException> fault)
                                {
                                    ClientLoggerHelper.LogInfo(fault.ToString());
                                }
                                finally
                                {
                                    IsLoading2 = false;
                                    GetPatientPaymentByDay(curPatientPaymentSearch, FindPatient);
                                }
                            }), null);
                    }
                }
                catch (Exception ex)
                {
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                }
                finally
                {
                    Globals.IsBusy = false;
                    IsLoading2 = false;
                }
            });

            t.Start();
        }

        public void UpdateReportPaymentReceiptByStaff(ReportPaymentReceiptByStaff curReportPaymentReceiptByStaff)
        {
            IsLoading2 = true;
            var t = new Thread(() =>
            {
                Globals.EventAggregator.Publish(new BusyEvent
                {
                    IsBusy = true,
                    Message = "..."
                });
                try
                {
                    using (var serviceFactory = new CommonService_V2Client())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginUpdateReportPaymentReceiptByStaff(curReportPaymentReceiptByStaff,
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                try
                                {
                                    var res = contract.EndUpdateReportPaymentReceiptByStaff(asyncResult);
                                }
                                catch (FaultException<AxException> fault)
                                {
                                    ClientLoggerHelper.LogInfo(fault.ToString());
                                }
                                finally
                                {
                                    IsLoading2 = false;
                                    SearchReportCmd();
                                }
                            }), null);
                    }
                }
                catch (Exception ex)
                {
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                }
                finally
                {
                    Globals.IsBusy = false;
                    IsLoading2 = false;
                }
            });

            t.Start();
        }

        public void GetReportPaymentReceiptByStaffDetails(long RepPaymentRecvID)
        {
            IsLoading4 = true;
            IsEdit = false;
            var t = new Thread(() =>
            {
                Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.Z0124_G1_DangLayDSTThaiDK) });
                try
                {
                    using (var serviceFactory = new CommonService_V2Client())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetReportPaymentReceiptByStaffDetails(RepPaymentRecvID,
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                ReportPaymentReceiptByStaffDetails RptPayReceiptByStaffDetails = new ReportPaymentReceiptByStaffDetails();
                                try
                                {
                                    RptPayReceiptByStaffDetails = contract.EndGetReportPaymentReceiptByStaffDetails(asyncResult);
                                    if (RptPayReceiptByStaffDetails != null
                                        && RptPayReceiptByStaffDetails.allPatientPayment != null)
                                    {
                                        allPatientPayment = new ObservableCollection<PatientTransactionPayment>(RptPayReceiptByStaffDetails.allPatientPayment);
                                    }
                                }
                                catch (FaultException<AxException> fault)
                                {
                                    ClientLoggerHelper.LogInfo(fault.ToString());
                                }
                                finally
                                {
                                    calSumPayment();
                                    IsLoading4 = false;
                                }
                                NotifyOfPropertyChange(() => allPatientPayment);
                            }), null);
                    }
                }
                catch (Exception ex)
                {
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                }
                finally
                {
                    Globals.IsBusy = false;
                    IsLoading4 = false;
                }
            });

            t.Start();
        }

        //GetStaffCategoriesByType((long) AllLookupValues.StaffCatType.BAC_SI);

        #endregion

        public void authorization()
        {
            if (!Globals.isAccountCheck)
            {
                return;
            }
            mBaoCaoTT_Xem = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPatient
                                               , (int)ePatient.mReportPaymentReceipt,
                                               (int)oRegistrionEx.mBaoCaoTT_Xem, (int)ePermission.mView);
        }

        #region checking account

        private bool _mBaoCaoTT_Xem = true;

        public bool mBaoCaoTT_Xem
        {
            get
            {
                return _mBaoCaoTT_Xem;
            }
            set
            {
                if (_mBaoCaoTT_Xem == value)
                    return;
                _mBaoCaoTT_Xem = value;
                NotifyOfPropertyChange(() => mBaoCaoTT_Xem);
            }
        }



        #endregion

        private void GetStaffCategoriesByType()
        {
            //04092018 TTM: Gán cứng là 27 (nhân viên phòng tài vụ) để check lần này (04092018) rồi mới update service để sửa lại cho đàng hoàng
            allStaff = new ObservableCollection<Staff>();
            foreach (var item in Globals.AllStaffs)
            {
                if (item.StaffCatgID == (long)StaffCatg.NhanVienDangKy ||
                    item.StaffCatgID == 27)
                {
                    allStaff.Add(item);
                }
            }
            NotifyOfPropertyChange(() => allStaff);
        }
    }

    //private void GetStaffCategoriesByType(long V_StaffCatType)
    //{
    //    Globals.EventAggregator.Publish(new BusyEvent {IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi});
    //    var t = new Thread(() =>
    //        {
    //            using (var serviceFactory = new ResourcesManagementServiceClient())
    //            {
    //                IResourcesManagementService contract = serviceFactory.ServiceInstance;
    //                contract.BeginGetRefStaffCategoriesByType(V_StaffCatType,
    //                    Globals.DispatchCallback((asyncResult) =>
    //                {
    //                    try
    //                    {
    //                        List<RefStaffCategory> results =contract.EndGetRefStaffCategoriesByType(asyncResult);
    //                        if (results != null &&results.Count >0)
    //                        {
    //                            allStaff =new ObservableCollection<Staff>();
    //                            foreach (RefStaffCategory p in results)
    //                            {
    //                                GetAllStaff(p.StaffCatgID);
    //                            }
    //                            NotifyOfPropertyChange(() =>allStaff);
    //                        }
    //                    }
    //                    catch (Exception ex)
    //                    {
    //                        MessageBox.Show(ex.Message);
    //                    }
    //                    finally
    //                    {
    //                        Globals.IsBusy =
    //                            false;
    //                    }
    //                }), null);
    //            }
    //        });

    //    t.Start();
    //}

    //private void GetAllStaff(long StaffCatgID)
    //{
    //    IsLoading5 = true;
    //    Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
    //    var t = new Thread(() =>
    //    {
    //        using (var serviceFactory = new ResourcesManagementServiceClient())
    //        {
    //            IResourcesManagementService contract = serviceFactory.ServiceInstance;
    //            contract.BeginGetAllStaff(StaffCatgID,
    //            Globals.DispatchCallback((asyncResult) =>
    //            {
    //                try
    //                {
    //                    var results =contract.EndGetAllStaff(asyncResult);
    //                    if (results !=null && results.Count >0)
    //                    {
    //                        Staff temp=new Staff();
    //                        temp.FullName = string.Format("--{0--", eHCMSResources.Z0126_G1_Chon1NVien);
    //                        temp.StaffID = 0;
    //                        allStaff.Add(temp);
    //                        foreach (Staff p in results)
    //                        {
    //                            allStaff.Add(p);
    //                        }
    //                        NotifyOfPropertyChange(() =>allStaff);
    //                    }
    //                }
    //                catch (Exception ex)
    //                {
    //                    MessageBox.Show(ex.Message);
    //                }
    //                finally
    //                {
    //                    IsLoading5 = false;
    //                    Globals.IsBusy = false;
    //                }
    //            }), null);
    //        }
    //    });

    //    t.Start();
    //}
}
