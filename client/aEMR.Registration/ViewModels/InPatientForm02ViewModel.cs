using eHCMSLanguage;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading;
using System.Windows;
using aEMR.CommonTasks;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using Caliburn.Micro;
using DataEntities;
using aEMR.Common;
using aEMR.Common.Collections;
using System.Text;
using Castle.Windsor;
/*
 * 20180829 #001 TTM:   Dữ liệu (quyền lợi, lần khám hiện tại) không được truyền cho ViewModel con nên tick Tính BH (xem chi tiết bill) không hoạt động.
 *                      Truyền dữ liệu để tick hoạt động bình thường quyền lợi và lần đăng ký hiện tại để thực hiện các hàm kiểm tra.
 */
namespace aEMR.Registration.ViewModels
{
    [Export(typeof(IInPatientForm02)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class InPatientForm02ViewModel : Conductor<object>, IInPatientForm02                
        , IHandle<ItemSelected<PatientRegistration>>                  
    {
        IEventAggregator _eventArg;
        [ImportingConstructor]
        public InPatientForm02ViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg)
        {
            _eventArg = eventArg;
            var searchPatientAndRegVm = Globals.GetViewModel<ISearchPatientAndRegistration>();

            //searchPatientAndRegVm.InitButtonVisibility(SearchRegButtonsVisibility.SHOW_SEARCH_PATIENT_BTN | SearchRegButtonsVisibility.SHOW_NEW_PATIENT_BTN);
            searchPatientAndRegVm.InitButtonVisibility(SearchRegButtonsVisibility.SHOW_SEARCH_REG_BTN);
            searchPatientAndRegVm.SetDefaultButton(SearchRegistrationButtons.SEARCH_REGISTRATION);

            // TxD 05/01/2014: Changed 'CanSearhRegAllDept' default value to false and it will be reset again when UsedByTaiVuOffice property is set
            searchPatientAndRegVm.CanSearhRegAllDept = false;
            searchPatientAndRegVm.SearchAdmittedInPtRegOnly = true;

            SearchRegistrationContent = searchPatientAndRegVm;
            ActivateItem(searchPatientAndRegVm);

            //KMx: Chuyển từ View IInPatientBillingInvoiceListing -> IInPatientBillingInvoiceListingNew (13/09/2014 16:54).
            //var billingVm = Globals.GetViewModel<IInPatientBillingInvoiceListing>();
            var billingVm = Globals.GetViewModel<IInPatientBillingInvoiceListingNew>();
            BillingInvoiceListingContent = billingVm;
            BillingInvoiceListingContent.ShowEditColumn = false;
            BillingInvoiceListingContent.ShowInfoColumn = true;
            BillingInvoiceListingContent.ShowRecalcHiColumn = false;
            BillingInvoiceListingContent.ShowRecalcHiWithPriceListColumn = false;
            BillingInvoiceListingContent.ShowCheckItemColumn = true;
            //BillingInvoiceListingContent.InvoiceDetailsContent.ShowDeleteColumn = false;

            BillingInvoiceInForm02Content = Globals.GetViewModel<IInPatientBillingInvoiceListingNew>();
            BillingInvoiceInForm02Content.ShowEditColumn = false;
            BillingInvoiceInForm02Content.ShowInfoColumn = true;
            BillingInvoiceInForm02Content.ShowRecalcHiColumn = false;
            BillingInvoiceInForm02Content.ShowRecalcHiWithPriceListColumn = false;
            BillingInvoiceInForm02Content.ShowCheckItemColumn = true;


            var patientInfoVm = Globals.GetViewModel<IPatientSummaryInfoV2>();
            patientInfoVm.mInfo_CapNhatThongTinBN = false;
            patientInfoVm.mInfo_XacNhan = false;
            patientInfoVm.mInfo_XoaThe = false;
            patientInfoVm.mInfo_XemPhongKham = false;
            PatientSummaryInfoContent = patientInfoVm;
            ActivateItem(patientInfoVm);

            CurrentRptForm02 = new InPatientRptForm02();
            CurrentRptForm02.Department = new RefDepartment();

        }

        private AllLookupValues.PatientFindBy _PatientFindBy;
        public AllLookupValues.PatientFindBy PatientFindBy
        {
            get
            {
                return _PatientFindBy;
            }
            set
            {
                _PatientFindBy = value;
                NotifyOfPropertyChange(() => PatientFindBy);
                // Hpt 27/11/2015: Đã gán giá trị trong hàm khởi tạo rồi nhưng không có thời gian xem lại nên cứ để thêm một lần nữa ở đây, có thời gian sẽ xem lại và điều chỉnh 
                if (SearchRegistrationContent != null)
                {
                    SearchRegistrationContent.PatientFindBy = PatientFindBy;
                }
            }
        }



        private void InitRespDepts()
        {
            if (CurRegistration == null || CurRegistration.AdmissionInfo == null || CurRegistration.AdmissionInfo.InPatientDeptDetails.Count <= 0)
            {
                return;
            }

            ObservableCollection<long> ListDeptID = new ObservableCollection<long>();

            foreach (var inDeptItem in CurRegistration.AdmissionInfo.InPatientDeptDetails)
            {
                if (!ListDeptID.Any(x => x == inDeptItem.DeptLocation.DeptID))
                {
                    ListDeptID.Add(inDeptItem.DeptLocation.DeptID);
                }
            }

            RespDepartments = Globals.AllRefDepartmentList.Where(item => item.IsDeleted == false && ListDeptID.Any(x => x == item.DeptID)).ToObservableCollection();

            if (RespDepartments.Count() > 0)
            {
                RefDepartment firstItem = new RefDepartment();
                firstItem.DeptID = 0;
                firstItem.DeptName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.T0822_G1_TatCa);
                RespDepartments.Insert(0, firstItem);

                SelRespDepartments = RespDepartments[0];

                if (Globals.ObjRefDepartment != null && Globals.ObjRefDepartment.DeptID > 0)
                {
                    foreach (var deptItem in RespDepartments)
                    {
                        if (deptItem.DeptID == Globals.ObjRefDepartment.DeptID)
                        {
                            SelRespDepartments = deptItem;
                            break;
                        }
                    }
                }
            }

        }

        
        private ObservableCollection<RefDepartment> _RespDepartments;
        public ObservableCollection<RefDepartment> RespDepartments
        {
            get
            {
                return _RespDepartments;
            }
            set
            {
                _RespDepartments = value;
                NotifyOfPropertyChange(() => RespDepartments);
            }
        }

        private RefDepartment _SelRespDepartments;
        public RefDepartment SelRespDepartments
        {
            get
            {
                return _SelRespDepartments;
            }
            set
            {
                _SelRespDepartments = value;
                NotifyOfPropertyChange(() => SelRespDepartments);
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

        private string _DeptLocTitle;
        public string DeptLocTitle
        {
            get
            {
                return _DeptLocTitle;
            }
            set
            {
                _DeptLocTitle = value;
                NotifyOfPropertyChange(() => DeptLocTitle);
            }
        }

        private ISearchPatientAndRegistration _searchRegistrationContent;

        public ISearchPatientAndRegistration SearchRegistrationContent
        {
            get { return _searchRegistrationContent; }
            set
            {
                _searchRegistrationContent = value;
                NotifyOfPropertyChange(() => SearchRegistrationContent);
            }
        }

        private IPatientSummaryInfoV2 _patientSummaryInfoContent;

        public IPatientSummaryInfoV2 PatientSummaryInfoContent
        {
            get { return _patientSummaryInfoContent; }
            set
            {
                _patientSummaryInfoContent = value;
                NotifyOfPropertyChange(() => PatientSummaryInfoContent);
            }
        }

        private IInPatientBillingInvoiceListingNew _billingInvoiceListingContent;
        public IInPatientBillingInvoiceListingNew BillingInvoiceListingContent
        {
            get { return _billingInvoiceListingContent; }
            set
            {
                _billingInvoiceListingContent = value;
                NotifyOfPropertyChange(() => BillingInvoiceListingContent);
            }
        }

        private IInPatientBillingInvoiceListingNew _billingInvoiceInForm02Content;
        public IInPatientBillingInvoiceListingNew BillingInvoiceInForm02Content
        {
            get { return _billingInvoiceInForm02Content; }
            set
            {
                _billingInvoiceInForm02Content = value;
                NotifyOfPropertyChange(() => BillingInvoiceInForm02Content);
            }
        }

        private bool _isDischarged;
        public bool isDischarged
        {
            get { return _isDischarged; }
            set
            {
                _isDischarged = value;
                NotifyOfPropertyChange(() => isDischarged);                
            }
        }


        public bool IsProcessing
        {
            get
            {
                return true;
            }
        }

        private bool _CanPay;
        public bool CanPay
        {
            get
            {
                return _CanPay;
            }
            set
            {
                if (_CanPay != value)
                {
                    _CanPay = value;
                    NotifyOfPropertyChange(() => CanPay);
                }
            }
        }
        private bool _canChangePatientType = false;
        public bool CanChangePatientType
        {
            get
            {
                return _canChangePatientType;
            }
            set
            {
                if (_canChangePatientType != value)
                {
                    _canChangePatientType = value;
                    NotifyOfPropertyChange(() => CanChangePatientType);
                }
            }
        }
        private bool _canSaveRegistrationAndPay;
        public bool CanSaveRegistrationAndPay
        {
            get
            {
                return _canSaveRegistrationAndPay;
            }
            set
            {
                if (_canSaveRegistrationAndPay != value)
                {
                    _canSaveRegistrationAndPay = value;
                    NotifyOfPropertyChange(() => CanSaveRegistrationAndPay);
                }
            }
        }

        private bool _canSearchPatient = true;
        public bool CanSearchPatient
        {
            get
            {
                return _canSearchPatient;
            }
            set
            {
                if (_canSearchPatient != value)
                {
                    _canSearchPatient = value;
                    NotifyOfPropertyChange(() => CanSearchPatient);
                }
            }
        }

        private void NotifyButtonBehaviourChanges()
        {
            CanPay = !IsProcessing && CurrentRegMode == RegistrationFormMode.OLD_REGISTRATION_OPENED;

            CanSaveRegistrationAndPay = !IsProcessing;
        }
        /// <summary>
        /// Lấy lại những giá trị mặc định để đưa lên form
        /// </summary>


        public PatientClassification CurClassification
        {
            get
            {
                return PatientSummaryInfoContent.CurrentPatientClassification;
            }
            set
            {
                PatientSummaryInfoContent.CurrentPatientClassification = value;
                NotifyOfPropertyChange(() => CurClassification);
            }
        }

        private HealthInsurance _confirmedHiItem;
        /// <summary>
        /// Thông tin thẻ bảo hiểm đã được confirm
        /// </summary>
        public HealthInsurance ConfirmedHiItem
        {
            get
            {
                return _confirmedHiItem;
            }
            set
            {
                _confirmedHiItem = value;
                NotifyOfPropertyChange(() => ConfirmedHiItem);

                CurClassification = CreateDefaultClassification();
            }
        }

        private PatientRegistration _curRegistration;
        public PatientRegistration CurRegistration
        {
            get
            {
                return _curRegistration;
            }
            set
            {
                if (_curRegistration != value)
                {
                    _curRegistration = value;
                    NotifyOfPropertyChange(() => CurRegistration);
                    if (CurRegistration.AdmissionInfo.DischargeDate != null)
                    {
                        isDischarged = true;
                        NotifyOfPropertyChange(() => isDischarged);
                    }
                    else
                    {
                        isDischarged = false;
                        NotifyOfPropertyChange(() => isDischarged);
                    }
                }
            }
        }
        private Patient _curPatient;
        public Patient CurPatient
        {
            get
            {
                return _curPatient;
            }
            set
            {
                _curPatient = value;
                NotifyOfPropertyChange(() => CurPatient);
            }
        }

        private PaperReferal _confirmedPaperReferal;
        /// <summary>
        /// Thông tin giấy chuyển viện đã được confirm
        /// </summary>
        public PaperReferal ConfirmedPaperReferal
        {
            get
            {
                return _confirmedPaperReferal;
            }
            set
            {
                _confirmedPaperReferal = value;
                NotifyOfPropertyChange(() => ConfirmedPaperReferal);
            }
        }
        private bool _canRegister;
        public bool CanRegister
        {
            get
            {
                return _canRegister;
            }
            set
            {
                if (_canRegister != value)
                {
                    _canRegister = value;
                    NotifyOfPropertyChange(() => CanRegister);
                }
            }
        }
        private RegistrationFormMode _currentRegMode = RegistrationFormMode.PATIENT_NOT_SELECTED;
        public RegistrationFormMode CurrentRegMode
        {
            get
            {
                return _currentRegMode;
            }
            set
            {
                if (_currentRegMode != value)
                {
                    _currentRegMode = value;
                    NotifyOfPropertyChange(() => CurrentRegMode);
                    NotifyButtonBehaviourChanges();
                }
            }
        }

        private PatientClassification CreateDefaultClassification()
        {
            if (ConfirmedHiItem != null)
            {
                return PatientClassification.CreatePatientClassification((long)PatientType.INSUARED_PATIENT, "");
            }
            else
            {
                return PatientClassification.CreatePatientClassification((long)PatientType.NORMAL_PATIENT, "");
            }
        }

        private void InitFormData()
        {
            if (CurRegistration == null || CurRegistration.PtRegistrationID <= 0)
            {
                PatientSummaryInfoContent.CanConfirmHi = true;
                return;
            }

            PatientSummaryInfoContent.CanConfirmHi = false;

            //BillingInvoiceListingContent.BillingInvoices = CurRegistration.InPatientBillingInvoices;
        }


        private void ShowOldRegistration(PatientRegistration regInfo)
        {
            CurRegistration = regInfo;
            //Chuyen sang mode giong nhu mo lai dang ky cu
            CurrentRegMode = RegistrationFormMode.OLD_REGISTRATION_OPENED;
            _confirmedHiItem = CurRegistration.HealthInsurance;
            _confirmedPaperReferal = CurRegistration.PaperReferal;
            NotifyOfPropertyChange(() => ConfirmedHiItem);
            NotifyOfPropertyChange(() => ConfirmedPaperReferal);
            InitRegistration();

            CanRegister = false;

            if (PatientSummaryInfoContent != null)
            {
                PatientSummaryInfoContent.SetPatientHISumInfo(CurRegistration.PtHISumInfo);
            }

            //▼====== #001: 
            BillingInvoiceListingContent.HIBenefit = CurRegistration.PtInsuranceBenefit;
            BillingInvoiceListingContent.CurentRegistration = this.CurRegistration;

            BillingInvoiceInForm02Content.HIBenefit = CurRegistration.PtInsuranceBenefit;
            BillingInvoiceInForm02Content.CurentRegistration = this.CurRegistration;
            //▲====== #001
            if (CurRegistration.PatientClassification == null && CurRegistration.PatientClassID.HasValue)
            {
                CurClassification = PatientClassification.CreatePatientClassification(CurRegistration.PatientClassID.Value, "");
            }
            else
            {
                CurClassification = CurRegistration.PatientClassification;
            }
            CanChangePatientType = false;
        }


        /// <summary>
        /// Gọi hàm này khi tạo mới một đăng ký, hoặc load xong một đăng ký đã có.
        /// Khởi tạo những giá trị cần thiết để đưa lên form
        /// </summary>
        private void InitRegistration()
        {
            if (CurRegistration.PtRegistrationID <= 0)
            {
                CurRegistration.Patient = CurPatient;
            }
            else
            {
                _curPatient = CurRegistration.Patient;

                NotifyOfPropertyChange(() => CurPatient);
                if (_curPatient != null && _curPatient.PatientID > 0)
                {
                    CanRegister = true;
                }
                else
                {
                    CanRegister = false;
                }
            }
            InitFormData();
        }

        public IEnumerator<IResult> OpenRegistration(long regID)
        {
            //KMx: Chỉ lấy những thông tin cần thiết của đăng ký thôi, không load hết (17/09/2014 17:31).
            LoadRegistrationSwitch LoadRegisSwitch = new LoadRegistrationSwitch();
            LoadRegisSwitch.IsGetAdmissionInfo = true;

            var loadRegTask = new LoadRegistrationInfo_InPtTask(regID, (int)AllLookupValues.V_FindPatientType.NOI_TRU, LoadRegisSwitch);
            yield return loadRegTask;
            if (loadRegTask.Registration == null)
            {
                //Thong bao khong load duoc dang ky
                Globals.EventAggregator.Publish(new ShowMessageEvent { Message = "Error!(3)" });
            }
            else
            {
                CurRegistration = loadRegTask.Registration;
                PatientSummaryInfoContent.CurrentPatient = CurRegistration.Patient;
                ShowOldRegistration(CurRegistration);
            }

            InitCurrentRptForm02();

            InitBillingInvoice();

            InitRptForm02List();

            InitRespDepts();

        }


        public void Handle(ItemSelected<PatientRegistration> message)
        {
            if (message == null || message.Item == null)
            {
                return;
            }
            
            Coroutine.BeginExecute(OpenRegistration(message.Item.PtRegistrationID));
            
        }


        private void InitRptForm02List()
        {
            RptForm02List = new ObservableCollection<InPatientRptForm02>();
        }

        private void InitBillingInvoice()
        {
            BillingInvoiceListingContent.BillingInvoices = new ObservableCollection<InPatientBillingInvoice>();

            BillingInvoiceInForm02Content.BillingInvoices = new ObservableCollection<InPatientBillingInvoice>();
        }

        private void InitCurrentRptForm02()
        {
            if (CurRegistration == null || CurRegistration.PtRegistrationID <= 0)
            {
                MessageBox.Show(eHCMSResources.A0566_G1_Msg_InfoChonDK, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
            CurrentRptForm02 = new InPatientRptForm02();
            CurrentRptForm02.Department = new RefDepartment();
            CurrentRptForm02.PtRegistrationID = CurRegistration.PtRegistrationID;
            CurrentRptForm02.RecCreatedDate = Globals.GetCurServerDateTime();
            CurrentRptForm02.StaffID = Globals.LoggedUserAccount.Staff.StaffID;
            CurrentRptForm02.StaffName = Globals.LoggedUserAccount.Staff.FullName;

        }


        private IEnumerator<IResult> DoCreateRptForm02(List<InPatientBillingInvoice> listBillingInvoiceSelected)
        {
            yield return GenericCoRoutineTask.StartTask(CreateRptForm02_Action, listBillingInvoiceSelected);

            yield return GenericCoRoutineTask.StartTask(LoadBillingInvoices_Action);

            yield return GenericCoRoutineTask.StartTask(GetForm02_Action);

        }

        public void LoadBillingInvoices_Action(GenericCoRoutineTask genTask)
        {
            if (RespDepartments.Count() <= 0 || SelRespDepartments == null)
            {
                MessageBox.Show(string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.Z0442_G1_SomethingIsWrongHere));
                return;
            }

            List<long> listDeptIDs = new List<long>();


            if (SelRespDepartments.DeptID == 0)
            {
                listDeptIDs = RespDepartments.Where(x => x.DeptID > 0).Select(item => item.DeptID).ToList();
            }
            else
            {
                listDeptIDs.Add(SelRespDepartments.DeptID);
            }

            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new BillingPaymentWcfServiceLibClient())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginGetAllInPatientBillingInvoices_ForCreateForm02(0, CurRegistration.PtRegistrationID, listDeptIDs,
                        Globals.DispatchCallback((asyncresult) =>
                        {
                            try
                            {
                                var updListBillingInv = contract.EndGetAllInPatientBillingInvoices_ForCreateForm02(asyncresult);
                                if (updListBillingInv != null)
                                {
                                    //KMx: Khi load bill, nếu bill đó đã nằm bên mẫu 02 (lưu rồi hoặc chưa lưu), thì không hiện bill đó lên nữa (16/03/2015 17:38).
                                    if (BillingInvoiceInForm02Content != null && BillingInvoiceInForm02Content.BillingInvoices != null)
                                    {
                                        BillingInvoiceListingContent.BillingInvoices = updListBillingInv.Where(x => !BillingInvoiceInForm02Content.BillingInvoices.Any(y => y.InPatientBillingInvID == x.InPatientBillingInvID)).ToObservableCollection();
                                    }
                                    else
                                    {
                                        BillingInvoiceListingContent.BillingInvoices = updListBillingInv.ToObservableCollection();
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.ToString());
                                ClientLoggerHelper.LogError(ex.ToString());
                            }
                            finally
                            {
                                this.HideBusyIndicator();
                                if (genTask != null)
                                {
                                    genTask.ActionComplete(true);
                                }
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    if (genTask != null)
                    {
                        genTask.ActionComplete(true);
                    }
                    MessageBox.Show(ex.ToString());
                    ClientLoggerHelper.LogError(ex.ToString());
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }


        public void LoadBillingInvoicesInForm02_Action(GenericCoRoutineTask genTask)
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new BillingPaymentWcfServiceLibClient())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginGetAllInPatientBillingInvoices_ForCreateForm02(CurrentRptForm02.RptForm02_InPtID, CurrentRptForm02.PtRegistrationID, null,
                        Globals.DispatchCallback((asyncresult) =>
                        {
                            try
                            {
                                var updListBillingInv = contract.EndGetAllInPatientBillingInvoices_ForCreateForm02(asyncresult);
                                if (updListBillingInv != null)
                                {
                                    BillingInvoiceInForm02Content.BillingInvoices = updListBillingInv.ToObservableCollection();
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.ToString());
                                ClientLoggerHelper.LogError(ex.ToString());
                            }
                            finally
                            {
                                this.HideBusyIndicator();
                                if (genTask != null)
                                {
                                    genTask.ActionComplete(true);
                                }
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    if (genTask != null)
                    {
                        genTask.ActionComplete(true);
                    }
                    MessageBox.Show(ex.ToString());
                    ClientLoggerHelper.LogError(ex.ToString());
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }



        private bool CheckDeptID(IList<InPatientBillingInvoice> _billingInvoiceList)
        {

            if (CurrentRptForm02.RptForm02_InPtID <= 0)
            {
                if (_billingInvoiceList == null || _billingInvoiceList.Count <= 0)
                {
                    MessageBox.Show(eHCMSResources.A0576_G1_Msg_InfoChonBillTaoMau02, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                    return false;
                }
            }
            else
            {
                if (_billingInvoiceList == null || _billingInvoiceList.Count <= 0)
                {
                    MessageBox.Show(eHCMSResources.A0817_G1_Msg_InfoTuDongXoaMau02, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                }
            }

            if (_billingInvoiceList == null || _billingInvoiceList.Count <= 0)
            {
                CurrentRptForm02.V_Form02Type = (long)AllLookupValues.V_Form02Type.ALL_DEPT;
                CurrentRptForm02.Department.DeptID = 0;
            }
            else
            {
                long DeptIDTemp = _billingInvoiceList.FirstOrDefault().DeptID.GetValueOrDefault();

                if (_billingInvoiceList.Any(x => x.DeptID != DeptIDTemp))
                {
                    CurrentRptForm02.V_Form02Type = (long)AllLookupValues.V_Form02Type.ALL_DEPT;
                    CurrentRptForm02.Department.DeptID = 0;
                }
                else
                {
                    CurrentRptForm02.V_Form02Type = (long)AllLookupValues.V_Form02Type.ONE_DEPT;
                    CurrentRptForm02.Department.DeptID = DeptIDTemp;
                }
            }

            return true;
        }

        public void CreateRptForm02_Action(GenericCoRoutineTask genTask, object objListBillingInv)
        {
            IList<InPatientBillingInvoice> _billingInvoiceList = (IList<InPatientBillingInvoice>)objListBillingInv;

            this.ShowBusyIndicator();

            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new CommonService_V2Client())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginCreateForm02(CurrentRptForm02, _billingInvoiceList,
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                try
                                {
                                    contract.EndCreateForm02(asyncResult);

                                    if (_billingInvoiceList == null || _billingInvoiceList.Count <= 0)
                                    {
                                        InitCurrentRptForm02();
                                    }

                                    MessageBox.Show(eHCMSResources.A0468_G1_Msg_InfoLuuOK, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(ex.ToString());
                                    ClientLoggerHelper.LogError(ex.ToString());
                                }
                                finally
                                {
                                    this.HideBusyIndicator();
                                    genTask.ActionComplete(true);
                                }
                            }), null);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                    ClientLoggerHelper.LogError(ex.ToString());
                    this.HideBusyIndicator();
                    genTask.ActionComplete(true);
                }
            });
            t.Start();
        }

        public void RefreshBillingInvListCmd()
        {
            if (CurRegistration == null || CurRegistration.PtRegistrationID <= 0)
            {
                MessageBox.Show(eHCMSResources.A0573_G1_Msg_InfoChonBNDeLoadBill, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
            if (Globals.IsLockRegistration(CurRegistration.RegLockFlag, eHCMSResources.T2664_G1_Loadbill.ToLower()))
            {
                return;
            }
            LoadBillingInvoices_Action(null);

        }

        public void CreateRptForm02Cmd()
        {
            if (Globals.IsLockRegistration(CurRegistration.RegLockFlag, eHCMSResources.T3145_G1_LuuMau02.ToLower()))
            {
                return;
            }
            List<InPatientBillingInvoice> selBillingInv = new List<InPatientBillingInvoice>();

            if (BillingInvoiceInForm02Content != null && BillingInvoiceInForm02Content.BillingInvoices != null && BillingInvoiceInForm02Content.BillingInvoices.Count > 0)
            {
                selBillingInv = BillingInvoiceInForm02Content.BillingInvoices.ToList();
            }

            if (!CheckDeptID(selBillingInv))
            {
                return;
            }

            Coroutine.BeginExecute(DoCreateRptForm02(selBillingInv));

        }


        private InPatientRptForm02 _CurrentRptForm02;
        public InPatientRptForm02 CurrentRptForm02
        {
            get
            {
                return _CurrentRptForm02;
            }
            set
            {
                if (_CurrentRptForm02 != value)
                {
                    _CurrentRptForm02 = value;
                    NotifyOfPropertyChange(() => CurrentRptForm02);
                }
            }
        }

        private ObservableCollection<InPatientRptForm02> _RptForm02List;
        public ObservableCollection<InPatientRptForm02> RptForm02List
        {
            get
            {
                return _RptForm02List;
            }
            set
            {
                if (_RptForm02List != value)
                {
                    _RptForm02List = value;
                    NotifyOfPropertyChange(() => RptForm02List);
                }
            }
        }

        public void LoadRptForm02ListCmd()
        {
            if (CurRegistration == null || CurRegistration.PtRegistrationID <= 0)
            {
                MessageBox.Show(eHCMSResources.K0299_G1_ChonDK, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
            GetForm02_Action(null);
        }

        public void GetForm02_Action(GenericCoRoutineTask genTask)
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new CommonService_V2Client())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetForm02(CurRegistration.PtRegistrationID, Globals.DispatchCallback((asyncResult) =>
                        {

                            try
                            {
                                var results = contract.EndGetForm02(asyncResult);

                                RptForm02List = results.ToObservableCollection();

                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message);
                            }
                            finally
                            {
                                this.HideBusyIndicator();
                                if (genTask != null)
                                {
                                    genTask.ActionComplete(true);
                                }
                            }

                        }), null);

                    }
                }
                catch (Exception ex)
                {
                    if (genTask != null)
                    {
                        genTask.ActionComplete(true);
                    }
                    MessageBox.Show(ex.ToString());
                    ClientLoggerHelper.LogError(ex.ToString());
                    this.HideBusyIndicator();
                }
            });

            t.Start();
        }



        private string GetReportSelected()
        {
            StringBuilder sb = new StringBuilder();

            if (RptForm02List != null)
            {
                var items = RptForm02List.Where(x => x.Checked == true);
                if (items != null && items.Count() > 0)
                {
                    sb.Append("<RptForm02_InPtIDs>");
                    foreach (var details in items)
                    {
                        sb.Append("<IDs>");
                        sb.AppendFormat("<ID>{0}</ID>", details.RptForm02_InPtID);
                        sb.Append("</IDs>");
                    }
                    sb.Append("</RptForm02_InPtIDs>");
                }
                else
                {
                    return string.Empty;
                }
            }
            else
            {
                return string.Empty;
            }

            return sb.ToString();
        }


        public void PrintRptForm02Cmd()
        {
            if (CurRegistration == null)
            {
                Globals.ShowMessage(eHCMSResources.K0300_G1_ChonDK, eHCMSResources.G0442_G1_TBao);
                return;
            }
            string IDList = GetReportSelected();

            if (string.IsNullOrEmpty(IDList))
            {
                MessageBox.Show(eHCMSResources.K0346_G1_ChonMau02DeIn);
                return;
            }
            long DeptIDTemp = RptForm02List.Where(x => x.Checked).FirstOrDefault().Department.DeptID;
            string DeptNameTemp = RptForm02List.Where(x => x.Checked).FirstOrDefault().Department.DeptName;

            if (RptForm02List.Where(x => x.Checked).Any(y => y.Department != null && y.Department.DeptID != DeptIDTemp))
            {
                DeptNameTemp = "";
            }
            Action<ICommonPreviewView> onInitDlg = delegate (ICommonPreviewView proAlloc)
            {
                proAlloc.ID = CurRegistration.PtRegistrationID;
                proAlloc.strIDList = IDList;
                proAlloc.DeptName = DeptNameTemp;
                proAlloc.eItem = ReportName.FORM_02_NoiTru;
            };
            GlobalsNAV.ShowDialog<ICommonPreviewView>(onInitDlg);
        }

        public void NewCmd()
        {
            InitCurrentRptForm02();

            InitBillingInvoice();

        }

        private void SetRefDepartment()
        {
            if (RespDepartments == null || RespDepartments.Count <= 0)
            {
                return;
            }

            if (CurrentRptForm02.Department == null || CurrentRptForm02.Department.DeptID <= 0)
            {
                SelRespDepartments = RespDepartments[0];
                return;
            }

            if (RespDepartments.Any(x => x.DeptID == CurrentRptForm02.Department.DeptID))
            {
                SelRespDepartments = RespDepartments.Where(x => x.DeptID == CurrentRptForm02.Department.DeptID).FirstOrDefault();
            }
            else
            {
                SelRespDepartments = new RefDepartment();
            }

        }
        

        public void dataGrid1_DblClick(object sender, Common.EventArgs<object> e)
        {
            CurrentRptForm02 = (InPatientRptForm02)e.Value;
            //SetRefDepartment();
            LoadBillingInvoicesInForm02_Action(null);
        }

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

        private void AllCheckedfc()
        {
            if (RptForm02List != null && RptForm02List.Count > 0)
            {
                foreach (InPatientRptForm02 item in RptForm02List)
                {
                    item.Checked = true;
                }
            }
        }
        private void UnCheckedfc()
        {
            if (RptForm02List != null && RptForm02List.Count > 0)
            {
                foreach (InPatientRptForm02 item in RptForm02List)
                {
                    item.Checked = false;
                }
            }
        }

        public void AddBillToForm02Cmd()
        {
            List<InPatientBillingInvoice> selectBill = BillingInvoiceListingContent.GetSelectedItems();

            if (selectBill == null || selectBill.Count <= 0)
            {
                MessageBox.Show(eHCMSResources.A0576_G1_Msg_InfoChonBillTaoMau02, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }

            BillingInvoiceListingContent.BillingInvoices = BillingInvoiceListingContent.BillingInvoices.Where(x => !selectBill.Any(y => y.InPatientBillingInvID == x.InPatientBillingInvID)).ToObservableCollection();

            foreach (InPatientBillingInvoice item in selectBill)
            {
                item.IsChecked = false;
                BillingInvoiceInForm02Content.BillingInvoices.Add(item);
            }

        }


        public void RemoveBillFromForm02Cmd()
        {
            List<InPatientBillingInvoice> selectBill = BillingInvoiceInForm02Content.GetSelectedItems();

            if (selectBill == null || selectBill.Count <= 0)
            {
                MessageBox.Show(eHCMSResources.A0578_G1_Msg_InfoChonBillCanXoa02, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }

            BillingInvoiceInForm02Content.BillingInvoices = BillingInvoiceInForm02Content.BillingInvoices.Where(x => !selectBill.Any(y => y.InPatientBillingInvID == x.InPatientBillingInvID)).ToObservableCollection();

            foreach (InPatientBillingInvoice item in selectBill)
            {
                item.IsChecked = false;
                BillingInvoiceListingContent.BillingInvoices.Add(item);
            }

        }
    }
}