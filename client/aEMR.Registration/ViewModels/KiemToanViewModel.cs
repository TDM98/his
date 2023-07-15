using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.ServiceModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using aEMR.DataContracts;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using Caliburn.Micro;
using aEMR.ViewContracts;
using System.ComponentModel.Composition;
using DataEntities;
using aEMR.Common;
using aEMR.Common.Collections;
using aEMR.Controls;
using eHCMS.Services.Core.Base;
using System.Linq;
using eHCMSLanguage;
using aEMR.Common.PagedCollectionView;
using Castle.Windsor;

namespace aEMR.Registration.ViewModels
{
    [Export(typeof(IKiemToan)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class KiemToanViewModel : Conductor<object>, IKiemToan
    {
        IEventAggregator _eventArg;
        [ImportingConstructor]
        public KiemToanViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg)
        {
            _eventArg = eventArg;
            ResetFilter();
            LoadallRegistrationStatus();

            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                LoadStaffs();
                LoadRegStatusList();
            }
            Registrations = new PagedSortableCollectionView<PatientRegistration>();
            Registrations.OnRefresh += Registrations_OnRefresh;
            
            var oldPaymentVm = Globals.GetViewModel<IPatientPayment>();
            PaymentContent = oldPaymentVm;
            ActivateItem(oldPaymentVm);
            _tempListRegistrations = new ObservableCollection<PatientRegistration>();
            _pageSelected=new ObservableCollection<PageSelect>();
            _selectedPageSelected=new PageSelect();
            _selectedPageSelected.tempPagePatientRegistration=new PagedSortableCollectionView<PatientRegistration>();
            _selectedPageSelected.tempPagePatientRegistration.OnRefresh += TempPagePatientRegistration_OnRefresh;
            _lstRegisStatus=new ObservableCollection<RegisStatus>();
            _lstDefaultRegisStatus=new ObservableCollection<RegisStatus>();
        }

        private void TempPagePatientRegistration_OnRefresh(object sender, Common.Collections.RefreshEventArgs e)
        {
            if (!CheckPage())
                GetSumRegistrations(selectedPageSelected.tempPagePatientRegistration.PageIndex
                    , selectedPageSelected.tempPagePatientRegistration.PageSize, true);
        }

        private void Registrations_OnRefresh(object sender, Common.Collections.RefreshEventArgs e)
        {
            GetSumRegistrations(Registrations.PageIndex, Registrations.PageSize, true);
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            //var FindRegistrationVM = Globals.GetViewModel<IFindRegistration>();
            //this.ActivateItem(FindRegistrationVM);
            //FindRegistrationCmd = FindRegistrationVM;

        }

        public object FindRegistrationCmd { get; set; }

        private bool _isNoiTru;
        public bool isNoiTru
        {
            get
            {
                return _isNoiTru;
            }
            set
            {
                if (_isNoiTru == value)
                    return;
                _isNoiTru = value;
            }
        }

        private int _FindPatient;
        public int FindPatient
        {
            get { return _FindPatient; }
            set
            {
                if (_FindPatient != value)
                {
                    _FindPatient = value;
                    NotifyOfPropertyChange(() => FindPatient);
                }
            }
        }

        private ObservableCollection<PageSelect> _pageSelected;
        public ObservableCollection<PageSelect> pageSelected
        {
            get
            {
                return _pageSelected;
            }
            set
            {
                if (_pageSelected == value)
                    return;
                _pageSelected = value;
            }
        }

        private PageSelect _selectedPageSelected;
        public PageSelect selectedPageSelected
        {
            get
            {
                return _selectedPageSelected;
            }
            set
            {
                if (_selectedPageSelected == value)
                    return;
                _selectedPageSelected = value;
            }
        }

        private bool _IsSearchGoToKhamBenh;
        public bool IsSearchGoToKhamBenh
        {
            get { return _IsSearchGoToKhamBenh; }
            set
            {
                _IsSearchGoToKhamBenh = value;
                NotifyOfPropertyChange(() => IsSearchGoToKhamBenh);
            }
        }
        public bool CheckPage()
        {
            foreach (var ps in pageSelected)
            {
                if (Registrations.PageIndex == ps.PageIndex)
                {
                    Registrations.Clear();
                    foreach (var sc in ps.tempPagePatientRegistration.SourceCollection)
                    {
                        Registrations.Add(sc);
                    }
                    NotifyOfPropertyChange(() => Registrations);
                    return true;
                }
            }
            //for (int i = 0; i < pageSelected.Count; i++)
            //{
            //    if (Registrations.PageIndex==pageSelected[i].PageIndex )
            //    {
            //        //int totalItem = selectedPageSelected.tempPagePatientRegistration.TotalItemCount;
            //        Registrations = ObjectCopier.DeepCopy(pageSelected[i].tempPagePatientRegistration);
            //        //selectedPageSelected.tempPagePatientRegistration.PageIndex = pageSelected[i].PageIndex;
            //        //selectedPageSelected.tempPagePatientRegistration.TotalItemCount = totalItem;
            //        NotifyOfPropertyChange(() => Registrations);
            //        return true;
            //    }
            //}
            return false;
        }
        protected void ResetFilter()
        {
            SearchCriteria = new SeachPtRegistrationCriteria();
            SearchCriteria.RegStatus = -1;
            SearchCriteria.StaffID = -1;
            SearchCriteria.FromDate = Globals.GetCurServerDateTime();
            SearchCriteria.ToDate = Globals.GetCurServerDateTime();
        }
        public void CancelCmd()
        {
            SelectedRegistration = null;
            TryClose();
            Globals.EventAggregator.Unsubscribe(this);
        }

        public void OkCmd()
        {
            SelectItemAndCloseFormIfNeeded();
        }
        private void SelectItemAndCloseFormIfNeeded()
        {
            Globals.EventAggregator.Publish(new ItemSelected<PatientRegistration>() { Item = SelectedRegistration });
            if (CloseAfterSelection)
            {
                TryClose();
                Globals.EventAggregator.Unsubscribe(this);
            }
        }

        private bool _closeAfterSelection = true;
        public bool CloseAfterSelection
        {
            get { return _closeAfterSelection; }
            set
            {
                _closeAfterSelection = value;
                NotifyOfPropertyChange(() => CloseAfterSelection);
            }
        }

        private ObservableCollection<PatientRegistration> _tempListRegistrations;
        public ObservableCollection<PatientRegistration> tempListRegistrations
        {
            get
            {
                return _tempListRegistrations;
            }
            set
            {
                if (_tempListRegistrations == value)
                    return;
                _tempListRegistrations = value;
            }
        }

        private PatientRegistration _selectedRegistration;

        public PatientRegistration SelectedRegistration
        {
            get { return _selectedRegistration; }
            set
            {
                _selectedRegistration = value;
                NotifyOfPropertyChange(() => SelectedRegistration);
            }
        }

        private PatientRegistration _viewRegistration;
        public PatientRegistration viewRegistration
        {
            get
            {
                return _viewRegistration;
            }
            set
            {
                if (_viewRegistration == value)
                    return;
                _viewRegistration = value;
                NotifyOfPropertyChange(() => viewRegistration);
            }
        }

        private SeachPtRegistrationCriteria _searchCriteria;
        public SeachPtRegistrationCriteria SearchCriteria
        {
            get { return _searchCriteria; }
            set
            {
                _searchCriteria = value;
                NotifyOfPropertyChange(() => SearchCriteria);
            }
        }

        private PagedSortableCollectionView<PatientRegistration> _registrations;
        public PagedSortableCollectionView<PatientRegistration> Registrations
        {
            get { return _registrations; }
            private set
            {
                _registrations = value;
                NotifyOfPropertyChange(() => Registrations);
            }
        }

        private ObservableCollection<Staff> _staffs;
        public ObservableCollection<Staff> Staffs
        {
            get { return _staffs; }
            set
            {
                _staffs = value;
                NotifyOfPropertyChange(() => Staffs);
            }
        }

        private ObservableCollection<Lookup> _regStatusList;
        public ObservableCollection<Lookup> RegStatusList
        {
            get { return _regStatusList; }
            set
            {
                _regStatusList = value;
                NotifyOfPropertyChange(() => RegStatusList);
            }
        }


        private ObservableCollection<PatientTransactionDetail> _curPatientTransactionDetail;
        public ObservableCollection<PatientTransactionDetail> curPatientTransactionDetail
        {
            get
            {
                return _curPatientTransactionDetail;
            }
            set
            {
                if (_curPatientTransactionDetail == value)
                    return;
                _curPatientTransactionDetail = value;
                NotifyOfPropertyChange(() => curPatientTransactionDetail);
            }
        }

        private ObservableCollection<AllLookupValues.RegistrationStatus> _allRegistrationStatus;
        public ObservableCollection<AllLookupValues.RegistrationStatus> allRegistrationStatus
        {
            get { return _allRegistrationStatus; }
            set
            {
                _allRegistrationStatus = value;
                NotifyOfPropertyChange(() => allRegistrationStatus);
            }
        }


        public IPatientPayment _paymentContent;

        public IPatientPayment PaymentContent
        {
            get { return _paymentContent; }
            set
            {
                _paymentContent = value;
                NotifyOfPropertyChange(() => PaymentContent);
            }
        }

        /// <summary>
        /// Bien nay dung de tim kiem dang ky.
        /// Khi nguoi dung thay doi thong tin tim kiem tren form thi khong set lai bien nay.
        /// Khi nao nguoi dung bat dau search thi moi lay gia tri cua SearchCriteria gan cho no.
        /// </summary>
        private SeachPtRegistrationCriteria _criteria;

        public void gridRegistrations_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            PatientRegistration objRows = e.Row.DataContext as PatientRegistration;
            if (objRows != null)
            {
                switch (objRows.CheckTransation)
                {
                    case 0:
                        {
                            e.Row.Foreground = new SolidColorBrush(Colors.Green);
                            break;
                        }
                    case 1:
                        {
                            e.Row.Foreground = new SolidColorBrush(Colors.Brown);
                            break;
                        }
                    case 2:
                        {
                            e.Row.Foreground = new SolidColorBrush(Colors.Red);
                            break;
                        }
                }
            }
        }

        public void CreatePatientCmd()
        {

        }

        public void SearchCmd()
        {
            _criteria = _searchCriteria.DeepCopy();
            if(isNoiTru)
            {
                _criteria.PatientFindBy = AllLookupValues.PatientFindBy.NOITRU;
                FindPatient = (int)AllLookupValues.V_FindPatientType.NOI_TRU;
            }
            else 
            {
                _criteria.PatientFindBy = AllLookupValues.PatientFindBy.NGOAITRU;
                FindPatient = (int)AllLookupValues.V_FindPatientType.NGOAI_TRU;
            }
            Registrations.PageIndex = 0;
            pageSelected.Clear();
            lstRegisStatus.Clear();
            lstDefaultRegisStatus.Clear();
            GetSumRegistrations(0, Registrations.PageSize, true);
        }

        private void GetSumRegistrations(int pageIndex, int pageSize, bool bCountTotal)
        {
            this.ShowBusyIndicator(eHCMSResources.Z0601_G1_DangTimDK);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginGetSumRegistrations(_criteria, pageIndex, pageSize, bCountTotal, Globals.DispatchCallback((asyncResult) =>
                        {
                            int totalCount = 0;
                            IList<PatientRegistration> allItems = null;
                            bool bOK = false;
                            try
                            {
                                allItems = client.EndGetSumRegistrations(out totalCount, asyncResult);
                                bOK = true;
                            }
                            catch (FaultException<AxException> fault)
                            {
                                ClientLoggerHelper.LogInfo(fault.ToString());
                            }
                            catch (Exception ex)
                            {
                                ClientLoggerHelper.LogInfo(ex.ToString());
                            }
                            finally
                            {
                                this.HideBusyIndicator();
                            }
                            PageSelect ps = new PageSelect();
                            ps.tempPagePatientRegistration=new PagedSortableCollectionView<PatientRegistration>();
                            Registrations.Clear();
                            if (bOK)
                            {
                                if (bCountTotal)
                                {
                                    Registrations.TotalItemCount = totalCount;
                                    
                                }
                                ps.tempPagePatientRegistration.TotalItemCount = Registrations.TotalItemCount;
                                if (allItems != null)
                                {
                                    foreach (var item in allItems)
                                    {
                                        Registrations.Add(item);
                                        ps.tempPagePatientRegistration.Add(item);
                                    }
                                    
                                    ps.PageIndex = Registrations.PageIndex;
                                    pageSelected.Add(ps);
                                }
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    ClientLoggerHelper.LogInfo(ex.ToString());
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }

        public void ResetFilterCmd()
        {
            ResetFilter();
        }

        public void OpenRegistration(long regID)
        {
            var t = new Thread(() =>
            {
                this.ShowBusyIndicator(eHCMSResources.Z0086_G1_DangLayTTinDK);
                //TranLoading = true;
                AxErrorEventArgs error = null;
                try
                {
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetRegistrationInfo(regID,FindPatient, false, false,
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                try
                                {
                                    var regInfo = contract.EndGetRegistrationInfo(asyncResult);
                                    SelectedRegistration= regInfo;
                                    
                                    if (SelectedRegistration.PatientTransaction==null)
                                    {
                                        //reset lai collection
                                        if (curPatientTransactionDetail != null)
                                        {
                                            curPatientTransactionDetail.Clear();
                                        }
                                        List<PatientTransactionPayment> patientPayments = new List<PatientTransactionPayment>();
                                        PaymentContent.PatientPayments = patientPayments.ToObservableCollection();
                                        //-------------
                                        Globals.ShowMessage(string.Format("{0}!", eHCMSResources.Z0602_G1_BNChuaCoTransaction), eHCMSResources.G0442_G1_TBao);
                                        return;
                                    }
                                    if(curPatientTransactionDetail!=null)
                                        curPatientTransactionDetail.Clear();
                                    GetTransactionSum(SelectedRegistration.PatientTransaction.TransactionID);
                                    InitViewForPayments();
                                }
                                catch (FaultException<AxException> fault)
                                {
                                    error = new AxErrorEventArgs(fault);
                                }
                                catch (Exception ex)
                                {
                                    error = new AxErrorEventArgs(ex);
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
                    Globals.EventAggregator.Publish(new ErrorOccurred() { CurrentError = new AxErrorEventArgs(ex) });
                }
            });
            t.Start();
        }

        public void OpenRegistration_InPt(long regID)
        {
            //KMx: Chỉ lấy những thông tin cần thiết của đăng ký thôi, không load hết (17/09/2014 17:31).
            LoadRegistrationSwitch LoadRegisSwitch = new LoadRegistrationSwitch();
            LoadRegisSwitch.IsGetPatientTransactions = true;
            this.ShowBusyIndicator(eHCMSResources.Z0086_G1_DangLayTTinDK);
            var t = new Thread(() =>
            {
                AxErrorEventArgs error = null;
                try
                {
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetRegistrationInfo_InPt(regID, FindPatient, LoadRegisSwitch, false,
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                try
                                {
                                    var regInfo = contract.EndGetRegistrationInfo_InPt(asyncResult);
                                    SelectedRegistration = regInfo;

                                    if (SelectedRegistration.PatientTransaction == null)
                                    {
                                        //reset lai collection
                                        if (curPatientTransactionDetail != null)
                                        {
                                            curPatientTransactionDetail.Clear();
                                        }
                                        List<PatientTransactionPayment> patientPayments = new List<PatientTransactionPayment>();
                                        PaymentContent.PatientPayments = patientPayments.ToObservableCollection();
                                        //-------------
                                        Globals.ShowMessage(string.Format("{0}!", eHCMSResources.Z0602_G1_BNChuaCoTransaction), eHCMSResources.G0442_G1_TBao); return;
                                    }
                                    if (curPatientTransactionDetail != null)
                                        curPatientTransactionDetail.Clear();
                                    GetTransactionSum_InPt(SelectedRegistration.PatientTransaction.TransactionID);
                                    InitViewForPayments();
                                }
                                catch (FaultException<AxException> fault)
                                {

                                    error = new AxErrorEventArgs(fault);
                                }
                                catch (Exception ex)
                                {

                                    error = new AxErrorEventArgs(ex);
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
                    Globals.EventAggregator.Publish(new ErrorOccurred() { CurrentError = new AxErrorEventArgs(ex) });
                }
            });
            t.Start();
        }

        public void DoubleClick(object args)
        {
            EventArgs<object> eventArgs = args as EventArgs<object>;
            SelectedRegistration = eventArgs.Value as PatientRegistration;
            SelectItemAndCloseFormIfNeeded();
            OpenRegistration(SelectedRegistration.PtRegistrationID);
            //    
            
            //InitViewForPayments();

            //PatientPayment payment = message.Payment as PatientPayment;
            //if (payment != null && payment.PatientTransaction != null && payment.PatientTransaction.PtRegistrationID.HasValue)
            //{
            //    //Show Report:
            //    var reportVm = Globals.GetViewModel<IPaymentReport>();
            //    reportVm.PaymentID = payment.PtPmtID;
            //    Globals.ShowDialog(reportVm as Conductor<object>);

            //    OpenRegistration(payment.PatientTransaction.PtRegistrationID.Value);
            //}
        }


        public void lnkView_Click(object sender,RoutedEventArgs e)
        {
            SelectItemAndCloseFormIfNeeded(); 
            viewRegistration = ObjectCopier.DeepCopy(SelectedRegistration);
            NotifyOfPropertyChange(() => viewRegistration);
            if (SelectedRegistration.V_RegistrationType == AllLookupValues.RegistrationType.NOI_TRU)
                //|| SelectedRegistration.V_RegistrationType == AllLookupValues.RegistrationType.NGOAI_TRU_NOI_TRU)
            {
                OpenRegistration_InPt(SelectedRegistration.PtRegistrationID);
            }
            else
            {
                OpenRegistration(SelectedRegistration.PtRegistrationID);
            }
        }

        public void lnkTest_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedRegistration != null && SelectedRegistration.PtRegistrationID > 0)
            {
                if (MessageBox.Show(eHCMSResources.A0435_G1_Msg_ConfDongDK, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    CloseRegistrationAll(SelectedRegistration.PtRegistrationID,FindPatient);
                }
            }
            else
            {
                MessageBox.Show(eHCMSResources.A0307_G1_Msg_InfoChonDKDeDong);
            }
        }

        public void lnkChiTiet_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedRegistration != null && SelectedRegistration.PtRegistrationID > 0)
            {
                Action<ICommonPreviewView> onInitDlg = delegate (ICommonPreviewView proAlloc)
                {
                    proAlloc.RegistrationID = SelectedRegistration.PtRegistrationID;
                    if (SelectedRegistration.V_RegistrationType == AllLookupValues.RegistrationType.NOI_TRU
                        //|| SelectedRegistration.V_RegistrationType == AllLookupValues.RegistrationType.NGOAI_TRU_NOI_TRU
                        )
                    {
                        proAlloc.FindPatient = 1;
                    }
                    else
                    {
                        proAlloc.FindPatient = 0;
                    }
                    proAlloc.eItem = ReportName.BAOCAOCHITIETTHANHTOAN;
                };
                GlobalsNAV.ShowDialog<ICommonPreviewView>(onInitDlg);
            }
        }

        #region method
        private void InitViewForPayments()
        {
            ObservableCollection<PatientTransactionPayment> patientPayments = new ObservableCollection<PatientTransactionPayment>();

            if (SelectedRegistration != null && SelectedRegistration.PatientTransaction != null)
            {
                if (SelectedRegistration.PatientTransaction.PatientTransactionPayments != null)
                {
                    patientPayments = SelectedRegistration.PatientTransaction.PatientTransactionPayments.Where(x=>x.PtPmtAccID != 3).ToObservableCollection();
                    //foreach (var item in SelectedRegistration.PatientTransaction.PatientTransactionPayments)
                    //{
                    //    patientPayments.Add(item);
                    //}
                }
            }

            PaymentContent.PatientPayments = patientPayments;
        }
        
        public void LoadStaffs()
        {
            //_patientCatalog.GetStaffsHaveRegistrations();
            this.ShowBusyIndicator(eHCMSResources.Z0603_G1_DangLayDSNVien);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetStaffsHaveRegistrations((byte)StaffRegistrationType.NORMAL,
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                IList<Staff> allItems = new ObservableCollection<Staff>();
                                try
                                {
                                    allItems = contract.EndGetStaffsHaveRegistrations(asyncResult);
                                }
                                catch (FaultException<AxException> fault)
                                {
                                    ClientLoggerHelper.LogInfo(fault.ToString());
                                }
                                catch (Exception ex)
                                {
                                    ClientLoggerHelper.LogInfo(ex.ToString());
                                }
                                finally
                                {
                                    this.HideBusyIndicator();
                                }
                                Staffs = new ObservableCollection<Staff>(allItems);
                                Staff firstItem = new Staff();
                                firstItem.StaffID = -1;
                                firstItem.FullName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.T0822_G1_TatCa);
                                Staffs.Insert(0, firstItem);
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

        public void LoadRegStatusList()
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new CommonService_V2Client())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginGetAllLookupValuesByType(LookupValues.REGISTRATION_STATUS,
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                IList<Lookup> allItems = new ObservableCollection<Lookup>();
                                try
                                {
                                    allItems = contract.EndGetAllLookupValuesByType(asyncResult);
                                }
                                catch (FaultException<AxException> fault)
                                {
                                    ClientLoggerHelper.LogInfo(fault.ToString());
                                }
                                catch (Exception ex)
                                {
                                    ClientLoggerHelper.LogInfo(ex.ToString());
                                }
                                finally
                                {
                                    this.HideBusyIndicator();
                                }

                                RegStatusList = new ObservableCollection<Lookup>(allItems);
                                Lookup firstItem = new Lookup();
                                firstItem.LookupID = -1;
                                firstItem.ObjectValue = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.T0822_G1_TatCa);
                                RegStatusList.Insert(0, firstItem);
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

        public void CopyExistingPatientList(IList<PatientRegistration> items, SeachPtRegistrationCriteria criteria, int total)
        {
            _criteria = criteria.DeepCopy();
            _registrations.Clear();
            if (items != null && items.Count > 0)
            {
                foreach (PatientRegistration p in items)
                {
                    _registrations.Add(p);
                }
                _registrations.TotalItemCount = total;
            }
            NotifyOfPropertyChange(() => Registrations);
        }

        public void GetTransactionSum(long TransactionID)
        {
            this.ShowBusyIndicator(eHCMSResources.Z0188_G1_DangLayTTinTransaction);
            var t = new Thread(() =>
            {
                AxErrorEventArgs error = null;
                try
                {
                    using (var serviceFactory = new CommonService_V2Client())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetTransactionSum(TransactionID,
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                try
                                {
                                    var results = contract.EndGetTransactionSum(asyncResult);
                                    //
                                    if (curPatientTransactionDetail == null)
                                    {
                                        curPatientTransactionDetail = new ObservableCollection<PatientTransactionDetail>();
                                    }
                                    else
                                    {
                                        curPatientTransactionDetail.Clear();
                                    }
                                    foreach (var p in results)
                                    {
                                        curPatientTransactionDetail.Add(p);
                                    }

                                    NotifyOfPropertyChange(() => curPatientTransactionDetail);
                                }
                                catch (FaultException<AxException> fault)
                                {

                                    error = new AxErrorEventArgs(fault);
                                }
                                catch (Exception ex)
                                {

                                    error = new AxErrorEventArgs(ex);
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
                    Globals.EventAggregator.Publish(new ErrorOccurred() { CurrentError = new AxErrorEventArgs(ex) });
                }
            });
            t.Start();
        }

        public void GetTransactionSum_InPt(long TransactionID)
        {
            this.ShowBusyIndicator(eHCMSResources.Z0188_G1_DangLayTTinTransaction);
            var t = new Thread(() =>
            {
                AxErrorEventArgs error = null;
                try
                {
                    using (var serviceFactory = new CommonService_V2Client())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetTransactionSum_InPt(TransactionID,
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                try
                                {
                                    var results = contract.EndGetTransactionSum_InPt(asyncResult);
                                    //
                                    if (curPatientTransactionDetail == null)
                                    {
                                        curPatientTransactionDetail = new ObservableCollection<PatientTransactionDetail>();
                                    }
                                    else
                                    {
                                        curPatientTransactionDetail.Clear();
                                    }
                                    foreach (var p in results)
                                    {
                                        curPatientTransactionDetail.Add(p);
                                    }

                                    NotifyOfPropertyChange(() => curPatientTransactionDetail);
                                }
                                catch (FaultException<AxException> fault)
                                {

                                    error = new AxErrorEventArgs(fault);
                                }
                                catch (Exception ex)
                                {

                                    error = new AxErrorEventArgs(ex);
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
                    error = new AxErrorEventArgs(ex);
                    Globals.EventAggregator.Publish(new ErrorOccurred() { CurrentError = error });
                }
            });
            t.Start();
        }

        public void CloseRegistrationAll(long PtRegistrationID, int FindPatient)
        {
            this.ShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
            var t = new Thread(() =>
            {
                AxErrorEventArgs error = null;
                try
                {
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        string Error="";
                        contract.BeginCloseRegistrationAll(PtRegistrationID,FindPatient,
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                try
                                {
                                    var Res = contract.EndCloseRegistrationAll(out Error, asyncResult);

                                    if(Res)
                                    {
                                        SearchCmd();
                                        MessageBox.Show(eHCMSResources.A0529_G1_Msg_InfoDongDKOK);
                                    }
                                    else
                                    {
                                        MessageBox.Show(Error);
                                    }
                                }
                                catch (FaultException<AxException> fault)
                                {
                                    error = new AxErrorEventArgs(fault);
                                }
                                catch (Exception ex)
                                {
                                    error = new AxErrorEventArgs(ex);
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
                    error = new AxErrorEventArgs(ex);
                    Globals.EventAggregator.Publish(new ErrorOccurred() { CurrentError = error });
                }
            });
            t.Start();
        }

        #endregion

        #region Update registration status
        public AxComboBox cboRegistrationStatus { get; set; }
        public void cboRegistrationStatus_Loaded(object sender)
        {
            cboRegistrationStatus = sender as AxComboBox;
            cboRegistrationStatus.ItemsSource = allRegistrationStatus;
        }

        public bool CheckExist()
        {
            return true;
        }

        public bool GetUpdate()
        {
            foreach (var registrations in tempListRegistrations)
            {
                if (registrations.PtRegistrationID == SelectedRegistration.PtRegistrationID)
                //registrations.RegistrationStatus == SelectedRegistration.RegistrationStatus)
                {
                    return false;
                }
            }
            return true;
        }

        private ObservableCollection<RegisStatus> _lstDefaultRegisStatus;
        public ObservableCollection<RegisStatus> lstDefaultRegisStatus
        {
            get
            {
                return _lstDefaultRegisStatus;
            }
            set
            {
                if (_lstDefaultRegisStatus == value)
                    return;
                _lstDefaultRegisStatus = value;
            }
        }

        private ObservableCollection<RegisStatus> _lstRegisStatus;
        public ObservableCollection<RegisStatus> lstRegisStatus
        {
            get
            {
                return _lstRegisStatus;
            }
            set
            {
                if (_lstRegisStatus == value)
                    return;
                _lstRegisStatus = value;
            }
        }

        public void cboRegistrationStatus_SelectionChanged(object sender, RoutedEventArgs e)
        {
            bool flag = false;
            for (int i = 0; i < lstRegisStatus.Count; i++)
            {
                if (SelectedRegistration!=null)
                {
                    if (lstRegisStatus[i].PtRegistrationID == SelectedRegistration.PtRegistrationID)
                    {
                        flag = true;
                        lstRegisStatus[i].V_RegistrationStatus = (long)(AllLookupValues.RegistrationStatus)((AxComboBox)sender).SelectedItem;
                        break;
                    }    
                }
            }
            if (flag == false)
            {
                if (SelectedRegistration != null)
                {
                    RegisStatus rsdefault = new RegisStatus(SelectedRegistration.PtRegistrationID
                       , (long)SelectedRegistration.RegistrationStatus, Registrations.PageIndex);
                    lstDefaultRegisStatus.Add(rsdefault);
                    RegisStatus rs = new RegisStatus(SelectedRegistration.PtRegistrationID
                        , (long)(AllLookupValues.RegistrationStatus)((AxComboBox)sender).SelectedItem
                        , Registrations.PageIndex);
                    lstRegisStatus.Add(rs);   
                }
            }
        }
        
        private ObservableCollection<RegisStatus> _tempLstPss;
        public ObservableCollection<RegisStatus> tempLstPss
        {
            get
            {
                return _tempLstPss;
            }
            set
            {
                if (_tempLstPss == value)
                    return;
                _tempLstPss = value;
            }
        }

        public void createTempList(int pageIndex)
        {
            try
            {
                tempLstPss = new ObservableCollection<RegisStatus>();
                foreach (var dgs in lstDefaultRegisStatus)
                {
                    if (dgs.Page == pageIndex)
                    {
                        tempLstPss.Add(dgs);
                    }
                }
            }
            catch { }
        }

        public void ClearList(int pageIndex)
        {
            for (int i = lstRegisStatus.Count - 1; i > -1; i--)
            {
                try
                {
                    if (lstRegisStatus[i].Page == pageIndex)
                    {
                        lstRegisStatus.Remove(lstRegisStatus[i]);
                    }
                }
                catch { }
            }
        }

        public void ResetAllCmd()
        {
            foreach (var ps in pageSelected)
            {
                createTempList(ps.PageIndex);
                foreach (var ptp in ps.tempPagePatientRegistration)
                {
                    foreach (var lp in tempLstPss)
                    {
                        if (lp.PtRegistrationID == ptp.PtRegistrationID)
                        {
                            ptp.RegistrationStatus = (AllLookupValues.RegistrationStatus)lp.V_RegistrationStatus;
                        }
                    }
                }
                NotifyOfPropertyChange(() => Registrations);
                ClearList(ps.PageIndex);
            }
            ResetPageCmd();
            Globals.ShowMessage(eHCMSResources.Z0187_G1_ResetAllPageSuccessful, "");
        }

        public void ResetPageCmd()
        {
            createTempList(Registrations.PageIndex);
            foreach (var ps in Registrations)
            {
                foreach (var lp in tempLstPss)
                {
                    if (lp.PtRegistrationID == ps.PtRegistrationID)
                    {
                        ps.RegistrationStatus = (AllLookupValues.RegistrationStatus)lp.V_RegistrationStatus;
                    }
                }
            }
            NotifyOfPropertyChange(() => Registrations);
            ClearList(Registrations.PageIndex);
            Globals.ShowMessage(eHCMSResources.Z0187_G1_ResetAllPageSuccessful, "");
        }

        public void SaveAll()
        {
            foreach (var item in lstRegisStatus)
            {
                UpdatePatientRegistration(item.PtRegistrationID, (int)item.V_RegistrationStatus);
            }
            lstRegisStatus.Clear();
            lstDefaultRegisStatus.Clear();
        }

        public void LoadallRegistrationStatus()
        {
            Type enumType = typeof(AllLookupValues.RegistrationStatus);

            // Set up a new collection  
            allRegistrationStatus = new ObservableCollection<AllLookupValues.RegistrationStatus>();

            // Retrieve the info for the type (it'd be nice to use Enum.GetNames here but alas, we're stuck with this)  
            FieldInfo[] infos;
            infos = enumType.GetFields(BindingFlags.Public | BindingFlags.Static);

            // Add each proper enum value to the collection  
            foreach (FieldInfo fi in infos)
            {
                allRegistrationStatus.Add((AllLookupValues.RegistrationStatus)Enum.Parse(enumType, fi.Name, true));
            }
        }

        public void UpdatePatientRegistration(long PtRegistrationID, long V_RegistrationStatus)
        {
            this.ShowBusyIndicator(eHCMSResources.Z0624_G1_DangUpdate);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginUpdatePatientRegistration(PtRegistrationID, V_RegistrationStatus,
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                try
                                {
                                    var allItems = contract.EndUpdatePatientRegistration(asyncResult);
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

        #endregion
        
        public void Handle(ItemSelected<PatientRegistration, PatientRegistrationDetail> message)
        {
            if (message != null)
            {
                TryClose();
                Globals.EventAggregator.Unsubscribe(this);
            }
        }
    }
    
    public class PageSelect: NotifyChangedBase
    {
        public PageSelect()
        {
            
        }

        public PageSelect(int pageIndex, PagedSortableCollectionView<PatientRegistration> ttempPagePatientRegistration)
        {
            PageIndex = pageIndex;
            tempPagePatientRegistration = ttempPagePatientRegistration;
        }

        private int _PageIndex;
        public int PageIndex
        {
            get
            {
                return _PageIndex;
            }
            set
            {
                if (_PageIndex == value)
                    return;
                _PageIndex = value;
            }
        }

        private PagedSortableCollectionView<PatientRegistration> _tempPagePatientRegistration;
        public PagedSortableCollectionView<PatientRegistration> tempPagePatientRegistration
        {
            get
            {
                return _tempPagePatientRegistration;
            }
            set
            {
                if (_tempPagePatientRegistration == value)
                    return;
                _tempPagePatientRegistration = value;
            }
        }
    }

    public class RegisStatus : NotifyChangedBase
    {
        private int _Page;
        public int Page
        {
            get
            {
                return _Page;
            }
            set
            {
                if (_Page == value)
                    return;
                _Page = value;
            }
        }

        public RegisStatus()
        {

        }

        public RegisStatus(long ptRegistrationID, long v_RegistrationStatus,int page)
        {
            PtRegistrationID = ptRegistrationID;
            V_RegistrationStatus = v_RegistrationStatus;
            Page = page;
        }

        private long _PtRegistrationID;
        public long PtRegistrationID
        {
            get
            {
                return _PtRegistrationID;
            }
            set
            {
                if (_PtRegistrationID == value)
                    return;
                _PtRegistrationID = value;
            }
        }

        private long _V_RegistrationStatus;
        public long V_RegistrationStatus
        {
            get
            {
                return _V_RegistrationStatus;
            }
            set
            {
                if (_V_RegistrationStatus == value)
                    return;
                _V_RegistrationStatus = value;
            }
        }

        public void authorization()
        {
            if (!Globals.isAccountCheck)
            {
                return;
            }
            mKiemToan_Xem = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPatient
                                               , (int)ePatient.mKiemToan,
                                               (int)oRegistrionEx.mKiemToan_Xem, (int)ePermission.mView);
            
        }

        #region checking account

        private bool _mKiemToan_Xem = true;

        public bool mKiemToan_Xem
        {
            get
            {
                return _mKiemToan_Xem;
            }
            set
            {
                if (_mKiemToan_Xem == value)
                    return;
                _mKiemToan_Xem = value;
                //NotifyOfPropertyChange(() => mKiemToan_Xem);
            }
        }

        #endregion
    }
}
