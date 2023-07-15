using eHCMSLanguage;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.ServiceModel;
using System.Threading;
using System.Windows;
using aEMR.CommonTasks;
using aEMR.DataContracts;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using Caliburn.Micro;
using DataEntities;
using aEMR.Common;
using aEMR.Common.Collections;
using Castle.Windsor;
/*
 * 20180829 #001 TTM:   Dữ liệu (quyền lợi, lần khám hiện tại) không được truyền cho ViewModel con nên tick Tính BH (xem chi tiết bill) không hoạt động.
 *                      Truyền dữ liệu để tick hoạt động bình thường quyền lợi và lần đăng ký hiện tại để thực hiện các hàm kiểm tra.
 */
namespace aEMR.Registration.ViewModels
{
    [Export(typeof(IInPatientProcessPayment)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class InPatientProcessPaymentViewModel : Conductor<object>, IInPatientProcessPayment
        , IHandle<ResultFound<Patient>>
        , IHandle<ItemSelected<Patient>>
        , IHandle<ItemSelected<PatientRegistration>>
        , IHandle<CreateNewPatientEvent>
        , IHandle<ResultNotFound<Patient>>
        , IHandle<PayForRegistrationCompleted>
        , IHandle<LoadInPatientBillingInvoice>
    {
        IEventAggregator _eventArg;
        [ImportingConstructor]
        public InPatientProcessPaymentViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg)
        {
            _eventArg = eventArg;
            var searchPatientAndRegVm = Globals.GetViewModel<ISearchPatientAndRegistration>();
            searchPatientAndRegVm.mTimBN = mTinhTienNoiTru_Patient_TimBN;
            searchPatientAndRegVm.mThemBN = mTinhTienNoiTru_Patient_ThemBN;
            searchPatientAndRegVm.mTimDangKy = mTinhTienNoiTru_Patient_TimDangKy;

            //searchPatientAndRegVm.InitButtonVisibility(SearchRegButtonsVisibility.SHOW_SEARCH_PATIENT_BTN | SearchRegButtonsVisibility.SHOW_NEW_PATIENT_BTN);
            searchPatientAndRegVm.InitButtonVisibility(SearchRegButtonsVisibility.SHOW_SEARCH_REG_BTN);
            searchPatientAndRegVm.SetDefaultButton(SearchRegistrationButtons.SEARCH_REGISTRATION);

            searchPatientAndRegVm.CanSearhRegAllDept = true;
            searchPatientAndRegVm.SearchAdmittedInPtRegOnly = true;

            SearchRegistrationContent = searchPatientAndRegVm;
            ActivateItem(searchPatientAndRegVm);

            //KMx: Chuyển từ View IInPatientBillingInvoiceListing -> IInPatientBillingInvoiceListingNew (13/09/2014 16:54).
            //var billingVm = Globals.GetViewModel<IInPatientBillingInvoiceListing>();
            var billingNotYetPaidVm = Globals.GetViewModel<IInPatientBillingInvoiceListingNew>();
            BillingInvoiceListingNotYetPaidContent = billingNotYetPaidVm;
            BillingInvoiceListingNotYetPaidContent.ShowEditColumn = false;
            BillingInvoiceListingNotYetPaidContent.ShowInfoColumn = true;
            BillingInvoiceListingNotYetPaidContent.ShowRecalcHiColumn = false;
            BillingInvoiceListingNotYetPaidContent.ShowRecalcHiWithPriceListColumn = false;
            BillingInvoiceListingNotYetPaidContent.ShowCheckItemColumn = true;

            //BillingInvoiceListingNotYetPaidContent.InvoiceDetailsContent.ShowDeleteColumn = false;


            var billingPaidAlreadyVm = Globals.GetViewModel<IInPatientBillingInvoiceListingNew>();
            BillingInvoiceListingPaidAlreadyContent = billingPaidAlreadyVm;
            BillingInvoiceListingPaidAlreadyContent.ShowEditColumn = false;
            BillingInvoiceListingPaidAlreadyContent.ShowInfoColumn = true;
            BillingInvoiceListingPaidAlreadyContent.ShowRecalcHiColumn = false;
            BillingInvoiceListingPaidAlreadyContent.ShowRecalcHiWithPriceListColumn = false;


            var cashAdvanceBillVm = Globals.GetViewModel<ICashAdvanceBill>();
            CashAdvanceBillContent = cashAdvanceBillVm;

            var patientInfoVm = Globals.GetViewModel<IPatientSummaryInfoV2>();
            patientInfoVm.mInfo_CapNhatThongTinBN = mTinhTienNoiTru_Info_CapNhatThongTinBN;
            patientInfoVm.mInfo_XacNhan = mTinhTienNoiTru_Info_XacNhan;
            patientInfoVm.mInfo_XoaThe = mTinhTienNoiTru_Info_XoaThe;
            patientInfoVm.mInfo_XemPhongKham = mTinhTienNoiTru_Info_XemPhongKham;
            PatientSummaryInfoContent = patientInfoVm;
            ActivateItem(patientInfoVm);
            Authorization();
        }
        protected override void OnActivate()
        {
            base.OnActivate();
            Globals.EventAggregator.Subscribe(this);

            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                LoadDepartments();
                LoadPatientClassifications();
            }
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

        //private IInPatientBillingInvoiceListing _billingInvoiceListingNotYetPaidContent;
        //public IInPatientBillingInvoiceListing BillingInvoiceListingNotYetPaidContent
        //{
        //    get { return _billingInvoiceListingNotYetPaidContent; }
        //    set
        //    {
        //        _billingInvoiceListingNotYetPaidContent = value;
        //        NotifyOfPropertyChange(() => BillingInvoiceListingNotYetPaidContent);
        //    }
        //}

        private IInPatientBillingInvoiceListingNew _billingInvoiceListingNotYetPaidContent;
        public IInPatientBillingInvoiceListingNew BillingInvoiceListingNotYetPaidContent
        {
            get { return _billingInvoiceListingNotYetPaidContent; }
            set
            {
                _billingInvoiceListingNotYetPaidContent = value;
                NotifyOfPropertyChange(() => BillingInvoiceListingNotYetPaidContent);
            }
        }

        private ICashAdvanceBill _cashAdvanceBillContent;
        public ICashAdvanceBill CashAdvanceBillContent
        {
            get { return _cashAdvanceBillContent; }
            set
            {
                _cashAdvanceBillContent = value;
                NotifyOfPropertyChange(() => CashAdvanceBillContent);
            }
        }

        private IInPatientBillingInvoiceListingNew _billingInvoiceListingPaidAlreadyContent;
        public IInPatientBillingInvoiceListingNew BillingInvoiceListingPaidAlreadyContent
        {
            get { return _billingInvoiceListingPaidAlreadyContent; }
            set
            {
                _billingInvoiceListingPaidAlreadyContent = value;
                NotifyOfPropertyChange(() => BillingInvoiceListingPaidAlreadyContent);
            }
        }

        private ObservableCollection<RefDepartment> _departments;

        public ObservableCollection<RefDepartment> Departments
        {
            get { return _departments; }
            set
            {
                _departments = value;
                NotifyOfPropertyChange(() => Departments);
            }
        }

        private ObservableCollection<PatientClassification> _patientClassifications;

        public ObservableCollection<PatientClassification> PatientClassifications
        {
            get { return _patientClassifications; }
            set
            {
                _patientClassifications = value;
                NotifyOfPropertyChange(() => PatientClassifications);
            }
        }
        private RefDepartment _department;
        public RefDepartment Department
        {
            get
            {
                return _department;
            }
            set
            {
                if (_department != value)
                {
                    _department = value;
                    NotifyOfPropertyChange(() => Department);
                }
            }
        }

        private bool _isChangeDept = true;
        public bool isChangeDept
        {
            get { return _isChangeDept; }
            set
            {
                _isChangeDept = value;
                NotifyOfPropertyChange(() => isChangeDept);
                CanPay = CanPay && isChangeDept;
                NotifyOfPropertyChange(() => CanPay);
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
                isChangeDept = !isDischarged;
            }
        }


        private bool _registrationLoading = false;
        /// <summary>
        /// Dang trong qua trinh lay thong tin dang ky tu server.
        /// </summary>
        public bool RegistrationLoading
        {
            get
            {
                return _registrationLoading;
            }
            set
            {
                _registrationLoading = value;
                NotifyOfPropertyChange(() => RegistrationLoading);

                NotifyWhenBusy();
            }
        }
        private bool _patientLoading = false;
        /// <summary>
        /// Dang trong qua trinh lay thong tin benh nhan tu server.
        /// </summary>
        public bool PatientLoading
        {
            get
            {
                return _patientLoading;
            }
            set
            {
                _patientLoading = value;
                NotifyOfPropertyChange(() => PatientLoading);

                NotifyWhenBusy();
            }
        }

        private bool _patientSearching = false;
        /// <summary>
        /// Dang trong qua trinh tim kiem benh nhan co hay khong.
        /// </summary>
        public bool PatientSearching
        {
            get
            {
                return _patientSearching;
            }
            set
            {
                _patientSearching = value;
                NotifyOfPropertyChange(() => PatientSearching);

                NotifyWhenBusy();
            }
        }
        private void NotifyWhenBusy()
        {
            NotifyOfPropertyChange(() => IsProcessing);
            NotifyButtonBehaviourChanges();
        }
        public bool IsProcessing
        {
            get
            {
                return _patientLoading || _isPaying || _registrationLoading;
            }
        }
        private bool _isPaying;
        public bool IsPaying
        {
            get
            {
                return _isPaying;
            }
            set
            {
                _isPaying = value;
                NotifyOfPropertyChange(() => IsPaying);

                NotifyWhenBusy();
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



        private void NotifyButtonBehaviourChanges()
        {
            CanPay = !IsProcessing && CurrentRegMode == RegistrationFormMode.OLD_REGISTRATION_OPENED;

            CanSaveRegistrationAndPay = !IsProcessing;
        }
        /// <summary>
        /// Lấy lại những giá trị mặc định để đưa lên form
        /// </summary>
        public void ResetToDefaultValues()
        {
            ResetDepartmentToDefaultValue();
        }
        public void ResetDepartmentToDefaultValue()
        {
            var loginVm = Globals.GetViewModel<ILogin>();
            if (loginVm.DeptLocation != null)
            {
                Department = loginVm.DeptLocation.RefDepartment;
            }
            else
            {
                Department = null;
            }
        }
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
                NotifyOfPropertyChange(() => HiServiceBeingUsed);
            }
        }
        public bool HiServiceBeingUsed
        {
            get
            {
                if (CurClassification == null)
                {
                    return false;
                }
                return CurClassification.PatientType == PatientType.INSUARED_PATIENT;
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
                    if (CurRegistration.AdmissionInfo != null && CurRegistration.AdmissionInfo.DischargeDate != null)
                    {
                        isDischarged = true;
                        NotifyOfPropertyChange(() => isDischarged);
                    }
                    else
                    {
                        isDischarged = false;
                        NotifyOfPropertyChange(() => isDischarged);
                    }
                    if (BillingInvoiceListingNotYetPaidContent != null && BillingInvoiceListingPaidAlreadyContent != null)
                    {
                        if (CurRegistration.HisID.GetValueOrDefault() > 0 && CurRegistration.PtInsuranceBenefit.GetValueOrDefault(0) > 0)
                        {
                            BillingInvoiceListingNotYetPaidContent.ShowHIAppliedColumn = true;
                            BillingInvoiceListingPaidAlreadyContent.ShowHIAppliedColumn = true;

                        }
                        else
                        {
                            BillingInvoiceListingNotYetPaidContent.ShowHIAppliedColumn = false;
                            BillingInvoiceListingPaidAlreadyContent.ShowHIAppliedColumn = false;

                        }
                    }
                    PatientSummaryInfoContent.CurrentPatientRegistration = CurRegistration;
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
        public void ResetPatientClassificationToDefaultValue()
        {
            CurClassification = CreateDefaultClassification();
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

        private DateTime? _selectedDate;
        public DateTime? SelectedDate
        {
            get { return _selectedDate; }
            set
            {
                _selectedDate = value;
                NotifyOfPropertyChange(() => SelectedDate);
            }
        }
        public void LoadDepartments()
        {
            var t = new Thread(() =>
            {
                Globals.EventAggregator.Publish(new BusyEvent
                {
                    IsBusy = true,
                    Message =
                        eHCMSResources.Z0149_G1_DangLayDSCacKhoa
                });
                try
                {
                    using (var serviceFactory = new CommonService_V2Client())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginGetAllDepartments(false, 
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                var allDepts =
                                    contract.
                                        EndGetAllDepartments(
                                            asyncResult);

                                if (allDepts != null)
                                {
                                    Departments =
                                        new ObservableCollection
                                            <RefDepartment>(
                                            allDepts);
                                    ResetDepartmentToDefaultValue();
                                }
                                else
                                {
                                    Departments = null;
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
                }
            });
            t.Start();
        }
        public void LoadPatientClassifications()
        {
            var t = new Thread(() =>
            {
                Globals.EventAggregator.Publish(new BusyEvent
                {
                    IsBusy = true,
                    Message =
                        string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.Z0150_G1_DangLayDSLoaiBN)
                });
                try
                {
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginGetAllClassifications(
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                var allClassifications = contract.EndGetAllClassifications(asyncResult);

                                if (allClassifications != null)
                                {
                                    PatientClassifications = new ObservableCollection<PatientClassification>(allClassifications);
                                }
                                else
                                {
                                    PatientClassifications = null;
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
                }
            });
            t.Start();
        }
        private DateTime _RegDate;
        public DateTime RegistrationDate
        {
            get
            {
                return _RegDate;
            }
            set
            {
                _RegDate = value;
                NotifyOfPropertyChange(() => RegistrationDate);
            }
        }

        //private void InitFormData()
        //{
        //    if (_curRegistration != null && _curRegistration.PtRegistrationID > 0)
        //    {
        //        PatientSummaryInfoContent.CanConfirmHi = false;
        //    }
        //    else
        //    {
        //        PatientSummaryInfoContent.CanConfirmHi = true;
        //    }
        //    if (CurRegistration != null && CurRegistration.InPatientBillingInvoices != null && CurRegistration.InPatientBillingInvoices.Count > 0)
        //    {
        //        BillingInvoiceListingNotYetPaidContent.BillingInvoices = CurRegistration.InPatientBillingInvoices.Where(item => item.PaidTime == null && item.TotalPatientPayment > item.TotalPatientPaid).ToObservableCollection();

        //        BillingInvoiceListingPaidAlreadyContent.BillingInvoices = CurRegistration.InPatientBillingInvoices.Where(item => item.PaidTime == null && item.TotalPatientPayment <= item.TotalPatientPaid).ToObservableCollection();
        //    }
        //}

        private void InitFormData()
        {
            if (CurRegistration == null || CurRegistration.PtRegistrationID <= 0)
            {
                PatientSummaryInfoContent.CanConfirmHi = true;
                return;
            }

            PatientSummaryInfoContent.CanConfirmHi = false;

            if (CurRegistration.InPatientBillingInvoices != null && CurRegistration.InPatientBillingInvoices.Count > 0)
            {
                //BillingInvoiceListingNotYetPaidContent.BillingInvoices = CurRegistration.InPatientBillingInvoices.Where(item => item.PaidTime == null && item.TotalPatientPayment > item.TotalPatientPaid).ToObservableCollection();

                //BillingInvoiceListingPaidAlreadyContent.BillingInvoices = CurRegistration.InPatientBillingInvoices.Where(item => item.PaidTime == null && item.TotalPatientPayment <= item.TotalPatientPaid).ToObservableCollection();

                // TxD 08/12/2014 removed PaidTime condition because it's NOW USED to indicate that a Bill has been finalized.

                // Hpt 12/10/2015: Dang ky thuoc loai vang lai va tien giai phau thi nhin thay bill cua tat ca cac khoa (ca A & B)
                
                //KMx: Không hiển thị bill của khoa A và B (07/03/2015 14:45).
                if (Globals.ServerConfigSection.InRegisElements.ExcludeDeptAAndB && CurRegistration.AdmissionInfo != null)
                {
                    BillingInvoiceListingNotYetPaidContent.BillingInvoices = CurRegistration.InPatientBillingInvoices.Where(item => item.TotalPatientPayment > item.TotalPatientPaid + item.TotalSupportFund && item.DeptID != 97 && item.DeptID != 98).ToObservableCollection();
                }
                else
                {
                    BillingInvoiceListingNotYetPaidContent.BillingInvoices = CurRegistration.InPatientBillingInvoices.Where(item => item.TotalPatientPayment > item.TotalPatientPaid + item.TotalSupportFund).ToObservableCollection();
                }

                BillingInvoiceListingPaidAlreadyContent.BillingInvoices = CurRegistration.InPatientBillingInvoices.Where(item => item.TotalPatientPayment <= item.TotalPatientPaid + item.TotalSupportFund).ToObservableCollection();

            }

            CashAdvanceBillContent.GetCashAdvanceBill(CurRegistration.PtRegistrationID, (long)CurRegistration.V_RegistrationType);
        }

        public void InitRegistrationForPatient()
        {
            if (_curPatient == null)
                return;

            if (_curPatient.LatestRegistration_InPt == null || (//_curPatient.LatestRegistration_InPt.V_RegistrationType != AllLookupValues.RegistrationType.NGOAI_TRU_NOI_TRU &&
                _curPatient.LatestRegistration_InPt.V_RegistrationType != AllLookupValues.RegistrationType.NOI_TRU)) //Chưa có đăng ký lần nào hoac khong phai dang ky noi tru.
            {
                MessageBox.Show(eHCMSResources.A0234_G1_Msg_InfoBNChuaCoDKNoiTru);
                ResetView();
                return;
            }
            // Hpt 04/10/2015: Truoc day chi co dang ky noi tru da nhap vien (RegistrationStatus = OPENED) moi duoc tim thay trong man hinh thu tien tam ung bill
            // Vi bo sung them hai loai dang ky moi la Vang Lai va Tien Giai Phau ma hai loai nay khong can nhap vien van duoc phuc vu. 
            // Do do, phai kiem tra them truong hop dang ky thuoc loai Vang Lai/Tien Giai Phau chua nhap vien (RegistrationStatus = PENDING_INPT)

            if (_curPatient.LatestRegistration_InPt.RegistrationStatus == AllLookupValues.RegistrationStatus.OPENED 
                || (_curPatient.LatestRegistration_InPt.V_RegForPatientOfType != AllLookupValues.V_RegForPatientOfType.Unknown 
                && _curPatient.LatestRegistration_InPt.RegistrationStatus == AllLookupValues.RegistrationStatus.PENDING_INPT))
                   // || _curPatient.LatestRegistration_InPt.RegistrationStatus == AllLookupValues.RegistrationStatus.PROCESSING)
            {
                //Mở đăng ký còn đang sử dụng
                OpenRegistration(_curPatient.LatestRegistration_InPt.PtRegistrationID);
            }
            else
            {
                MessageBox.Show(eHCMSResources.A0703_G1_Msg_InfoKhTheMoDK);
            }
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
            //▼====== #001
            BillingInvoiceListingNotYetPaidContent.HIBenefit = CurRegistration.PtInsuranceBenefit;
            BillingInvoiceListingNotYetPaidContent.CurentRegistration = this.CurRegistration;

            BillingInvoiceListingPaidAlreadyContent.HIBenefit = CurRegistration.PtInsuranceBenefit;
            BillingInvoiceListingPaidAlreadyContent.CurentRegistration = this.CurRegistration;
            //▲====== #001
            if (PatientSummaryInfoContent != null)
            {                
                //PatientSummaryInfoContent.HiBenefit = null;
                //PatientSummaryInfoContent.ConfirmedPaperReferal = _confirmedPaperReferal;
                //PatientSummaryInfoContent.ConfirmedHiItem = _confirmedHiItem;
                //PatientSummaryInfoContent.IsCrossRegion = regInfo.IsCrossRegion.GetValueOrDefault(false);
                //if (CurRegistration.PtInsuranceBenefit.HasValue)
                //{
                //    PatientSummaryInfoContent.HiBenefit = CurRegistration.PtInsuranceBenefit;
                //}
                PatientSummaryInfoContent.SetPatientHISumInfo(regInfo.PtHISumInfo);
            }
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

        public IEnumerator<IResult> DoOpenRegistration(long regID)
        {
            //KMx: Chỉ lấy những thông tin cần thiết của đăng ký thôi, không load hết (17/09/2014 17:31).
            LoadRegistrationSwitch LoadRegisSwitch = new LoadRegistrationSwitch();
            LoadRegisSwitch.IsGetAdmissionInfo = true;
            LoadRegisSwitch.IsGetBillingInvoices = true;

            var loadRegTask = new LoadRegistrationInfo_InPtTask(regID, (int)AllLookupValues.V_FindPatientType.NOI_TRU, LoadRegisSwitch);
            yield return loadRegTask;
            if (loadRegTask.Registration == null)
            {
                //Thong bao khong load duoc dang ky
                Globals.EventAggregator.Publish(new ShowMessageEvent { Message = "Error!(4)" });
            }
            else
            {
                CurRegistration = loadRegTask.Registration;
                PatientSummaryInfoContent.CurrentPatient = CurRegistration.Patient;
                ShowOldRegistration(CurRegistration);
            }
        }

        public void OpenRegistration(long regID)
        {
            RegistrationLoading = true;
            Coroutine.BeginExecute(DoOpenRegistration(regID), null, (o, e) => { RegistrationLoading = false; });
        }

        public void Handle(ResultFound<Patient> message)
        {
            if (message != null)
            {
                CurPatient = message.Result;
                if (CurPatient != null)
                {
                    Globals.EventAggregator.Publish(new ItemSelected<Patient>() { Item = CurPatient });
                }
            }
        }

        public void SetCurrentPatient(object patient)
        {
            Patient p = patient as Patient;
            if (p == null || p.PatientID <= 0)
            {
                return;
            }
            ConfirmedHiItem = null;
            ConfirmedPaperReferal = null;

            if (p.PatientID > 0)
            {
                GetPatientByID(p.PatientID);
            }
            else
            {
                CurPatient = null;
                CurrentRegMode = RegistrationFormMode.PATIENT_NOT_SELECTED;
            }
        }
        private bool _patientLoaded;
        public bool PatientLoaded
        {
            get
            {
                return _patientLoaded;
            }
            set
            {
                _patientLoaded = value;
                NotifyOfPropertyChange(() => PatientLoaded);
            }
        }
        private void GetPatientByID(long patientID)
        {
            PatientLoading = true;
            var t = new Thread(() =>
            {
                Globals.EventAggregator.Publish(new BusyEvent
                {
                    IsBusy = true,
                    Message = eHCMSResources.Z0119_G1_DangLayTTinBN
                });
                try
                {
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginGetPatientByID(patientID, false, 
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                try
                                {
                                    var patient = contract.EndGetPatientByID(asyncResult);
                                    CurPatient = patient;

                                    PatientLoaded = true;
                                    PatientLoading = false;
                                    InitRegistrationForPatient();
                                }
                                catch (FaultException<AxException> fault)
                                {
                                    ClientLoggerHelper.LogInfo(fault.ToString());
                                }
                                catch (Exception ex)
                                {
                                    ClientLoggerHelper.LogInfo(ex.ToString());
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
                }
            });
            t.Start();
        }

        public void Handle(ItemSelected<Patient> message)
        {
            if (message != null)
            {
                CurPatient = message.Item;
                if (CurPatient != null)
                {
                    SetCurrentPatient(CurPatient);
                }
            }
        }

        public void Handle(CreateNewPatientEvent message)
        {
            if (message != null)
            {
                Action<IPatientDetails> onInitDlg = delegate (IPatientDetails vm)
                {
                    vm.CurrentAction = eHCMSResources.Z0037_G1_ThemBN;
                    vm.InitLoadControlData_FromExt(null);
                    vm.CreateNewPatient();
                };
                GlobalsNAV.ShowDialog<IPatientDetails>(onInitDlg);
            }
        }

        public void Handle(ItemSelected<PatientRegistration> message)
        {
            if (message != null && message.Item != null)
            {
                OpenRegistration(message.Item.PtRegistrationID);
            }
        }

        public void Handle(ResultNotFound<Patient> message)
        {
            if (message != null)
            {
                //Thông báo không tìm thấy bệnh nhân.
                MessageBoxResult result = MessageBox.Show(eHCMSResources.A0727_G1_Msg_ConfThemMoiBN, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel);
                if (result == MessageBoxResult.OK)
                {
                    Action<IPatientDetails> onInitDlg = delegate (IPatientDetails vm)
                    {
                        var criteria = message.SearchCriteria as PatientSearchCriteria;
                        vm.CurrentAction = eHCMSResources.Z0037_G1_ThemBN;
                        vm.CreateNewPatient();
                        if (criteria != null)
                        {
                            vm.CurrentPatient.FullName = criteria.FullName;
                        }
                    };
                    GlobalsNAV.ShowDialog<IPatientDetails>(onInitDlg);
                }
            }
        }

        public void Handle(PayForRegistrationCompleted message)
        {
            if (message != null)
            {
                //Load lai dang ky:
                var payment = message.Payment;
                if (payment != null && payment.PatientTransaction != null && payment.PatientTransaction.PtRegistrationID.HasValue)
                {
                    if (payment.PtTranPaymtID > 0 && payment.PayAmount != 0 && message.ObjectState is long)
                    {
                        //Show Report:
                        Action<IPaymentReport> onInitDlg = delegate (IPaymentReport reportVm)
                        {
                            reportVm.PaymentID = payment.PtTranPaymtID;
                            reportVm.FindPatient = (int)AllLookupValues.V_FindPatientType.NOI_TRU;
                            reportVm.CashAdvanceID = (long)message.ObjectState;
                        };
                        GlobalsNAV.ShowDialog<IPaymentReport>(onInitDlg);
                    }

                    OpenRegistration(payment.PatientTransaction.PtRegistrationID.Value);
                }
            }
        }


        public void GetAllInPatientBillingInvoices()
        {
            if (CurRegistration == null || CurRegistration.PtRegistrationID <= 0)
            {
                MessageBox.Show(eHCMSResources.A0635_G1_Msg_InfoKhCoDKDeThaoTac, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
        

            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new BillingPaymentWcfServiceLibClient())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginGetAllInPatientBillingInvoices(CurRegistration.PtRegistrationID, null, (long)AllLookupValues.RegistrationType.NOI_TRU,
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                try
                                {
                                    var result = contract.EndGetAllInPatientBillingInvoices(asyncResult);

                                    CurRegistration.InPatientBillingInvoices = result.ToObservableCollection();

                                    //BillingInvoiceListingNotYetPaidContent.BillingInvoices = CurRegistration.InPatientBillingInvoices.Where(item => item.PaidTime == null && item.TotalPatientPayment > item.TotalPatientPaid).ToObservableCollection();
                                    // Neu la bill cua Vang Lai Hoac Tien Giai Phau thi cho Hien thi bill khoa tat ca khoa (ca A va B) luon                                   
                                    //KMx: Không hiển thị bill của khoa A và B (07/03/2015 14:45).

                                    // Hpt 17/10/2015: Dang ky Vang Lai _ Tien Giai Phau co kha nang co bill cua khoa A-B truoc khi nhap vien 
                                    // Nhung bill nay phai thu tien, thanh toan, quyet toan duoc neu khong se dan den khong the tao tiep dang ky sau do vi ly do dang ky truoc con bill chua quyet toan
                                    // Vi vay phai cho dang ky Vang Lai - Tien Giai Phau nhin thay bill khoa A-B
                                    if (Globals.ServerConfigSection.InRegisElements.ExcludeDeptAAndB && CurRegistration.AdmissionInfo != null)
                                    {
                                        BillingInvoiceListingNotYetPaidContent.BillingInvoices = CurRegistration.InPatientBillingInvoices.Where(item => item.TotalPatientPayment > item.TotalPatientPaid && item.DeptID != 97 && item.DeptID != 98).ToObservableCollection();
                                    }
                                    else
                                    {
                                        BillingInvoiceListingNotYetPaidContent.BillingInvoices = CurRegistration.InPatientBillingInvoices.Where(item => item.TotalPatientPayment > item.TotalPatientPaid).ToObservableCollection();
                                    }

                                    BillingInvoiceListingPaidAlreadyContent.BillingInvoices = CurRegistration.InPatientBillingInvoices.Where(item => item.PaidTime == null && item.TotalPatientPayment <= item.TotalPatientPaid).ToObservableCollection();

                                    CashAdvanceBillContent.GetCashAdvanceBill(CurRegistration.PtRegistrationID, (long)CurRegistration.V_RegistrationType);

                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(ex.ToString(), eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
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
                    MessageBox.Show(ex.ToString(), eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                }
                finally
                {
                    this.HideBusyIndicator();
                }

            });
            t.Start();
        }

        public void Handle(LoadInPatientBillingInvoice message)
        {
            GetAllInPatientBillingInvoices();
        }

        public void PayCmd()
        {
            if (Globals.IsLockRegistration(CurRegistration.RegLockFlag, eHCMSResources.G0737_G1_ThuTienTUBill.ToLower()))
            {
                return;
            }
            //KMx: ConfigPayment = True thì khi tính tiền chỉ insert vào bảng tạm ứng, không insert vào bảng Payment, để có thể cập nhật bill sau khi tính tiền (20/10/2014 17:19).
            //Trường hợp ConfigPayment = False thì sử dụng lại cách cũ, nhưng cách cũ còn nhiều lỗi, phải sửa lại mới sử dụng được.
            bool ConfigPayment = Globals.ServerConfigSection.InRegisElements.OnlyInsertToCashAdvance;

            if (ConfigPayment)
            {
                Action<IInPatientPayBill> onInitDlg = delegate (IInPatientPayBill vm)
                {
                    List<InPatientBillingInvoice> items = BillingInvoiceListingNotYetPaidContent.GetSelectedItems();

                    if (items == null || items.Count <= 0)
                    {
                        MessageBox.Show(eHCMSResources.K0309_G1_ChonHDonTToan, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                        return;
                    }

                    vm.SetValues(CurRegistration, items);

                    vm.StartCalculating();
                };
                GlobalsNAV.ShowDialog<IInPatientPayBill>(onInitDlg);
            }
            else
            {
                //KMx: Trong đây còn lỗi, sửa lại mới sử dụng được.
                //Chọn 2 bill thanh toán (mỗi bill trị giá 100K). BN trả 300K, chương trình tự lấy thêm 1 bill ko được chọn để thanh toán luôn.
                Action<IInPatientPay> onInitDlg = delegate (IInPatientPay vm)
                {
                    vm.Registration = CurRegistration;
                    List<InPatientBillingInvoice> items = BillingInvoiceListingNotYetPaidContent.GetSelectedItems();

                    //vm.BillingInvoices = items;
                    vm.SetValues(CurRegistration, items);
                    vm.AutoPay = items.Count == 0;

                    vm.StartCalculating();
                };
                GlobalsNAV.ShowDialog<IInPatientPay>(onInitDlg);
            }


        }
        public void ResetView()
        {
            BillingInvoiceListingNotYetPaidContent.BillingInvoices = null;
            //BillingInvoiceListingNotYetPaidContent.BeingViewedItem = null;
            BillingInvoiceListingPaidAlreadyContent.BillingInvoices = null;


            //PatientSummaryInfoContent.ConfirmedHiItem = null;
            //PatientSummaryInfoContent.HiBenefit = null;
            //PatientSummaryInfoContent.ConfirmedPaperReferal = null;
            PatientSummaryInfoContent.SetPatientHISumInfo(null);
        }
        public void Authorization()
        {
            if (!Globals.isAccountCheck)
            {
                return;
            }

            mTinhTienNoiTru_XemChiTiet = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPatient
                                             , (int)ePatient.mInPatientProcessPayment,
                                             (int)oRegistrionEx.mTinhTienNoiTru_XemChiTiet, (int)ePermission.mView);
            mTinhTienNoiTru_TraTien = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPatient
                                             , (int)ePatient.mInPatientProcessPayment,
                                             (int)oRegistrionEx.mTinhTienNoiTru_TraTien, (int)ePermission.mView);

            //phan nay nam trong module chung ne
            mTinhTienNoiTru_Patient_TimBN = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPatient
                                             , (int)ePatient.mRegister,
                                             (int)oRegistrionEx.mTinhTienNoiTru_Patient_TimBN, (int)ePermission.mView);
            mTinhTienNoiTru_Patient_ThemBN = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPatient
                                                 , (int)ePatient.mRegister,
                                                 (int)oRegistrionEx.mTinhTienNoiTru_Patient_ThemBN, (int)ePermission.mView);
            mTinhTienNoiTru_Patient_TimDangKy = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPatient
                                                 , (int)ePatient.mRegister,
                                                 (int)oRegistrionEx.mTinhTienNoiTru_Patient_TimDangKy, (int)ePermission.mView);

            mTinhTienNoiTru_Info_CapNhatThongTinBN = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPatient
                                                 , (int)ePatient.mRegister,
                                                 (int)oRegistrionEx.mTinhTienNoiTru_Info_CapNhatThongTinBN, (int)ePermission.mView);
            mTinhTienNoiTru_Info_XacNhan = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPatient
                                                 , (int)ePatient.mRegister,
                                                 (int)oRegistrionEx.mTinhTienNoiTru_Info_XacNhan, (int)ePermission.mView);
            mTinhTienNoiTru_Info_XoaThe = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPatient
                                                 , (int)ePatient.mRegister,
                                                 (int)oRegistrionEx.mTinhTienNoiTru_Info_XoaThe, (int)ePermission.mView);
            mTinhTienNoiTru_Info_XemPhongKham = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPatient
                                                 , (int)ePatient.mRegister,
                                                 (int)oRegistrionEx.mTinhTienNoiTru_Info_XemPhongKham, (int)ePermission.mView);

        }
        #region checking account
        private bool _mTinhTienNoiTru_XemChiTiet = true;
        private bool _mTinhTienNoiTru_TraTien = true;

        public bool mTinhTienNoiTru_XemChiTiet
        {
            get
            {
                return _mTinhTienNoiTru_XemChiTiet;
            }
            set
            {
                if (_mTinhTienNoiTru_XemChiTiet == value)
                    return;
                _mTinhTienNoiTru_XemChiTiet = value;
                NotifyOfPropertyChange(() => mTinhTienNoiTru_XemChiTiet);
            }
        }


        public bool mTinhTienNoiTru_TraTien
        {
            get
            {
                return _mTinhTienNoiTru_TraTien;
            }
            set
            {
                if (_mTinhTienNoiTru_TraTien == value)
                    return;
                _mTinhTienNoiTru_TraTien = value;
                NotifyOfPropertyChange(() => mTinhTienNoiTru_TraTien);
            }
        }

        //phan nay nam trong module chung
        private bool _mTinhTienNoiTru_Patient_TimBN = true;
        private bool _mTinhTienNoiTru_Patient_ThemBN = true;
        private bool _mTinhTienNoiTru_Patient_TimDangKy = true;

        private bool _mTinhTienNoiTru_Info_CapNhatThongTinBN = true;
        private bool _mTinhTienNoiTru_Info_XacNhan = true;
        private bool _mTinhTienNoiTru_Info_XoaThe = true;
        private bool _mTinhTienNoiTru_Info_XemPhongKham = true;

        public bool mTinhTienNoiTru_Patient_TimBN
        {
            get
            {
                return _mTinhTienNoiTru_Patient_TimBN;
            }
            set
            {
                if (_mTinhTienNoiTru_Patient_TimBN == value)
                    return;
                _mTinhTienNoiTru_Patient_TimBN = value;
                NotifyOfPropertyChange(() => mTinhTienNoiTru_Patient_TimBN);
            }
        }

        public bool mTinhTienNoiTru_Patient_ThemBN
        {
            get
            {
                return _mTinhTienNoiTru_Patient_ThemBN;
            }
            set
            {
                if (_mTinhTienNoiTru_Patient_ThemBN == value)
                    return;
                _mTinhTienNoiTru_Patient_ThemBN = value;
                NotifyOfPropertyChange(() => mTinhTienNoiTru_Patient_ThemBN);
            }
        }

        public bool mTinhTienNoiTru_Patient_TimDangKy
        {
            get
            {
                return _mTinhTienNoiTru_Patient_TimDangKy;
            }
            set
            {
                if (_mTinhTienNoiTru_Patient_TimDangKy == value)
                    return;
                _mTinhTienNoiTru_Patient_TimDangKy = value;
                NotifyOfPropertyChange(() => mTinhTienNoiTru_Patient_TimDangKy);
            }
        }

        public bool mTinhTienNoiTru_Info_CapNhatThongTinBN
        {
            get
            {
                return _mTinhTienNoiTru_Info_CapNhatThongTinBN;
            }
            set
            {
                if (_mTinhTienNoiTru_Info_CapNhatThongTinBN == value)
                    return;
                _mTinhTienNoiTru_Info_CapNhatThongTinBN = value;
                NotifyOfPropertyChange(() => mTinhTienNoiTru_Info_CapNhatThongTinBN);
            }
        }

        public bool mTinhTienNoiTru_Info_XacNhan
        {
            get
            {
                return _mTinhTienNoiTru_Info_XacNhan;
            }
            set
            {
                if (_mTinhTienNoiTru_Info_XacNhan == value)
                    return;
                _mTinhTienNoiTru_Info_XacNhan = value;
                NotifyOfPropertyChange(() => mTinhTienNoiTru_Info_XacNhan);
            }
        }

        public bool mTinhTienNoiTru_Info_XoaThe
        {
            get
            {
                return _mTinhTienNoiTru_Info_XoaThe;
            }
            set
            {
                if (_mTinhTienNoiTru_Info_XoaThe == value)
                    return;
                _mTinhTienNoiTru_Info_XoaThe = value;
                NotifyOfPropertyChange(() => mTinhTienNoiTru_Info_XoaThe);
            }
        }

        public bool mTinhTienNoiTru_Info_XemPhongKham
        {
            get
            {
                return _mTinhTienNoiTru_Info_XemPhongKham;
            }
            set
            {
                if (_mTinhTienNoiTru_Info_XemPhongKham == value)
                    return;
                _mTinhTienNoiTru_Info_XemPhongKham = value;
                NotifyOfPropertyChange(() => mTinhTienNoiTru_Info_XemPhongKham);
            }
        }

        #endregion
    }
}

