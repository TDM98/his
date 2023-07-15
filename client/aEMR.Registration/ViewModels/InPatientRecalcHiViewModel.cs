using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using aEMR.CommonTasks;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using Caliburn.Micro;
using DataEntities;
using aEMR.Common;
using eHCMSLanguage;
using aEMR.Common.Converters;
using Castle.Windsor;
/*
 * 20221020 #001 QTD: Thêm danh sách BN bên phải
 */
namespace aEMR.Registration.ViewModels
{
    [Export(typeof(IInPatientRecalcHi)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class InPatientRecalcHiViewModel : Conductor<object>, IInPatientRecalcHi
            //, IHandle<ResultFound<Patient>>
            //, IHandle<ItemSelected<Patient>>
            , IHandle<ItemSelected<PatientRegistration>>
            //, IHandle<ResultNotFound<Patient>>
            , IHandle<PayForRegistrationCompleted>
            , IHandle<SaveAndPayForRegistrationCompleted>
        , IHandle<HiCardConfirmedEvent>
        , IHandle<ConfirmHiBenefit>
        , IHandle<RemoveConfirmedHiCard>
        , IHandle<ItemChanged<InPatientBillingInvoice,IInPatientBillingInvoiceListing>>
        , IHandle<ReloadRegisAfterRecalcBillInvoiceEvent>
        , IHandle<InPatientRegistrationSelectedForInPatientBillingInvoiceListing>
    {
        IEventAggregator _eventArg;
        [ImportingConstructor]
        public InPatientRecalcHiViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg)
        {
            _eventArg = eventArg;
            var searchPatientAndRegVm = Globals.GetViewModel<ISearchPatientAndRegistration>();
            // Hpt 20/10/2015: Man hinh nay khong can hien nut Tim Benh Nhan
            searchPatientAndRegVm.InitButtonVisibility(SearchRegButtonsVisibility.SHOW_SEARCH_REG_BTN);
            searchPatientAndRegVm.SetDefaultButton(SearchRegistrationButtons.SEARCH_REGISTRATION);
            searchPatientAndRegVm.PatientFindByVisibility = false;
            searchPatientAndRegVm.PatientFindBy = AllLookupValues.PatientFindBy.NOITRU;
            searchPatientAndRegVm.mThemBN = false;

            searchPatientAndRegVm.CanSearhRegAllDept = true;
            searchPatientAndRegVm.SearchAdmittedInPtRegOnly = true;

            SearchRegistrationContent = searchPatientAndRegVm;
            ActivateItem(searchPatientAndRegVm);

            var patientInfoVm = Globals.GetViewModel<IPatientSummaryInfoV2>();

            // TxD 09/07/2014 : Added the following to enable ReConfirm HI Benefit for InPatient ONLY
            patientInfoVm.Enable_ReConfirmHI_InPatientOnly = true;
            patientInfoVm.mInfo_CapNhatThongTinBN = isChangeDept;
            //KMx: Không còn xác nhận BHYT ở đây nữa (21/12/2014 10:05).
            //patientInfoVm.mInfo_XacNhan = isChangeDept;
            patientInfoVm.mInfo_XacNhan = false;
            patientInfoVm.mInfo_XoaThe = false;//mXacNhanLaiBH_ThongTinBN_XoaThe;
            
            PatientSummaryInfoContent = patientInfoVm;
            ActivateItem(patientInfoVm);
            PatientSummaryInfoContent.CanConfirmHi = false;
            PatientSummaryInfoContent.DisplayButtons = true;
            
            
            var newBillingVm = Globals.GetViewModel<IInPatientBillingInvoiceDetailsListing>();
            EditingInvoiceDetailsContent = newBillingVm;
            ActivateItem(newBillingVm);

            //KMx: Chuyển từ View IInPatientBillingInvoiceListing -> IInPatientBillingInvoiceListingNew (13/09/2014 16:54).
            //var oldBillingVm = Globals.GetViewModel<IInPatientBillingInvoiceListing>();
            var oldBillingVm = Globals.GetViewModel<IInPatientBillingInvoiceListingNew>();
            OldBillingInvoiceContent = oldBillingVm;
            OldBillingInvoiceContent.ShowEditColumn = false;
            OldBillingInvoiceContent.ShowInfoColumn = true;
            OldBillingInvoiceContent.ShowRecalcHiColumn = true;
            ActivateItem(oldBillingVm);
            
        }
        protected override void OnActivate()
        {
            base.OnActivate();
            Globals.EventAggregator.Subscribe(this);
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                LoadPatientClassifications();
            }
            if (PatientFindBy == AllLookupValues.PatientFindBy.NGOAITRU)
            {
                SearchRegistrationContent.InitButtonVisibility(SearchRegButtonsVisibility.SHOW_SEARCH_REG_BTN);
                SearchRegistrationContent.SetDefaultButton(SearchRegistrationButtons.SEARCH_REGISTRATION);
                SearchRegistrationContent.mTimBN = false;
                SearchRegistrationContent.mThemBN = false;
                SearchRegistrationContent.mTimDangKy = true;
                SearchRegistrationContent.PatientFindByVisibility = false;
                SearchRegistrationContent.PatientFindBy = AllLookupValues.PatientFindBy.NGOAITRU;
                SearchRegistrationContent.IsSearchOutPtRegistrationOnly = true;
            }
            //▼==== #001
            var homeVm = Globals.GetViewModel<IHome>();
            IInPatientOutstandingTask ostvm = Globals.GetViewModel<IInPatientOutstandingTask>();
            ostvm.WhichVM = SetOutStandingTask.TINH_LAI_BILL_VP;
            ostvm.IsShowListOutPatientList = false;
            homeVm.OutstandingTaskContent = ostvm;
            homeVm.IsExpandOST = true;
            //ActivateItem(ostvm);
            //▲==== #001
        }

        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);
            Globals.EventAggregator.Unsubscribe(this);
            var homeVm = Globals.GetViewModel<IHome>();
            homeVm.OutstandingTaskContent = null;
            homeVm.IsExpandOST = false;
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

        private bool _isChangeDept = true;
        public bool isChangeDept
        {
            get { return _isChangeDept; }
            set
            {
                _isChangeDept = value;
                NotifyOfPropertyChange(() => isChangeDept);
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
                PatientSummaryInfoContent.mInfo_CapNhatThongTinBN = isChangeDept;
                //KMx: Không còn xác nhận BHYT ở đây nữa (21/12/2014 10:05).
                //PatientSummaryInfoContent.mInfo_XacNhan = mXacNhanLaiBH_ThongTinBN_XacNhanBH && isChangeDept;
            }
        }

        private bool _isEditing;
        public bool IsEditing
        {
            get { return _isEditing; }
            set 
            {
                if (_isEditing != value)
                {
                    _isEditing = value;
                    NotifyOfPropertyChange(() => IsEditing);
                    NotifyOfPropertyChange(() => CanStartEditRegistrationCmd);
                    NotifyOfPropertyChange(() => CanRegister);

                    EditingInvoiceDetailsContent.CanDelete = _isEditing;
                    EditingInvoiceDetailsContent.CanEditOnGrid = _isEditing || IsBillOfUsedItems;
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


        public string EditingBillingInvoiceTitle
        {
            get
            {
                if (_editingBillingInvoice == null)
                {
                    return string.Empty;
                }
                if (_editingBillingInvoice.InPatientBillingInvID > 0)
                {
                    return string.Format("{0} ", eHCMSResources.Z0152_G1_CNhatBill) + _editingBillingInvoice.BillingInvNum;
                }
                else
                {
                    return eHCMSResources.Z0014_G1_ThemBillMoi;
                }
            }
        }
        
        private IInPatientBillingInvoiceDetailsListing _editingInvoiceDetailsContent;
        public IInPatientBillingInvoiceDetailsListing EditingInvoiceDetailsContent
        {
            get { return _editingInvoiceDetailsContent; }
            set
            {
                _editingInvoiceDetailsContent = value;
                NotifyOfPropertyChange(() => EditingInvoiceDetailsContent);
            }
        }
        private IInPatientBillingInvoiceListingNew _oldBillingInvoiceContent;
        public IInPatientBillingInvoiceListingNew OldBillingInvoiceContent
        {
            get { return _oldBillingInvoiceContent; }
            set
            {
                _oldBillingInvoiceContent = value;
                NotifyOfPropertyChange(() => OldBillingInvoiceContent);
            }
        }

        public bool MedRegItemConfirmed
        {
            get
            {
                //Hiện tại là vậy.
                return true;
            }
        }
        private InPatientBillingInvoice _tempBillingInvoice;
        private InPatientBillingInvoice _editingBillingInvoice;
        public InPatientBillingInvoice EditingBillingInvoice
        {
            get
            {
                return _editingBillingInvoice;
            }
            set
            {
                if (_editingBillingInvoice != value)
                {
                    _editingBillingInvoice = value;
                    NotifyOfPropertyChange(() => EditingBillingInvoice);
                    NotifyOfPropertyChange(() => EditingBillingInvoiceTitle);
                    EditingInvoiceDetailsContent.BillingInvoice = _editingBillingInvoice;
                    EditingInvoiceDetailsContent.ResetView();

                    NotifyOfPropertyChange(() => CanStartEditRegistrationCmd);
                }
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

        private bool _calcPaymentToEndOfDay;

        public bool CalcPaymentToEndOfDay
        {
            get { return _calcPaymentToEndOfDay; }
            set
            {
                _calcPaymentToEndOfDay = value;
                NotifyOfPropertyChange(() => CalcPaymentToEndOfDay);
            }
        }
        private bool _canCalcPaymentToEndOfDay;

        public bool CanCalcPaymentToEndOfDay
        {
            get { return _canCalcPaymentToEndOfDay; }
            set
            {
                _canCalcPaymentToEndOfDay = value;
                NotifyOfPropertyChange(() => CanCalcPaymentToEndOfDay);
                if(!_canCalcPaymentToEndOfDay)
                {
                    CalcPaymentToEndOfDay = false;
                }
            }
        }

        /// <summary>
        /// Neu nguoi dung chon benh nhan, dang ky. Va dang ky hop le thi moi set bien nay true.
        /// </summary>
        public bool CanAddEditBill
        {
            get
            {
                if (CurRegistration != null && CurRegistration.PtRegistrationID > 0 && CurRegistration.V_RegistrationType == AllLookupValues.RegistrationType.NOI_TRU)
                {
                    switch (CurRegistration.V_RegForPatientOfType)
                    {
                        case AllLookupValues.V_RegForPatientOfType.Unknown:
                            return CurRegistration.RegistrationStatus == AllLookupValues.RegistrationStatus.OPENED && CurRegistration.AdmissionInfo != null && CurRegistration.AdmissionInfo.DischargeDate == null;
                        case AllLookupValues.V_RegForPatientOfType.DKBN_VANG_LAI:
                            return (CurRegistration.RegistrationStatus == AllLookupValues.RegistrationStatus.PENDING_INPT && Globals.Check_CasualAndPreOpReg_StillValid(CurRegistration));
                        case AllLookupValues.V_RegForPatientOfType.NBNT_BN_TIEN_PHAU_CO_BHYT:
                        case AllLookupValues.V_RegForPatientOfType.NBNT_BN_TIEN_PHAU_KHONG_BHYT:
                        case AllLookupValues.V_RegForPatientOfType.DKBN_DT_NGOAI_TRU:
                            return ((CurRegistration.RegistrationStatus == AllLookupValues.RegistrationStatus.OPENED && CurRegistration.AdmissionInfo != null && CurRegistration.AdmissionInfo.DischargeDate == null)
                                || (CurRegistration.RegistrationStatus == AllLookupValues.RegistrationStatus.PENDING_INPT && Globals.Check_CasualAndPreOpReg_StillValid(CurRegistration)));
                        default:
                            return false;
                    }
                }
                else if (CurRegistration != null && CurRegistration.PtRegistrationID > 0 && CurRegistration.V_RegistrationType == AllLookupValues.RegistrationType.NGOAI_TRU)
                {
                    switch (CurRegistration.V_RegistrationStatus)
                    {
                        case (long)AllLookupValues.RegistrationStatus.COMPLETED:
                        case (long)AllLookupValues.RegistrationStatus.REFUND:
                            return false;
                        default:
                            return true;
                    }
                }
                return false;
            } 
        }
       
        private ObservableCollection<RefStorageWarehouseLocation> _warehouseList;
        public ObservableCollection<RefStorageWarehouseLocation> WarehouseList
        {
            get
            {
                return _warehouseList;
            }
            set
            {
                _warehouseList = value;
                NotifyOfPropertyChange(()=>WarehouseList);
            }
        }

        //private RefStorageWarehouseLocation _selectedWarehouse;
        //public RefStorageWarehouseLocation SelectedWarehouse
        //{
        //    get
        //    {
        //        return _selectedWarehouse;
        //    }
        //    set
        //    {
        //        _selectedWarehouse = value;
        //        NotifyOfPropertyChange(() => SelectedWarehouse);

        //        SelectDrugContent.SearchCriteria.Storage = _selectedWarehouse;
        //        SelectDrugContent.Clear();
        //        ChemicalItemContent.SearchCriteria.Storage = _selectedWarehouse;
        //        ChemicalItemContent.Clear();
        //        MedItemContent.SearchCriteria.Storage = _selectedWarehouse;
        //        MedItemContent.Clear();
        //    }
        //}

        private bool _registrationLoading;
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
                NotifyOfPropertyChange(()=>RegistrationLoading);

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
                NotifyOfPropertyChange(()=>PatientLoading);

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
                NotifyOfPropertyChange(()=>PatientSearching);

                NotifyWhenBusy();
            }
        }
        private void NotifyWhenBusy()
        {
            NotifyOfPropertyChange(() => IsProcessing);
        }

        public string StatusText
        {
            get
            {
                if(_isSaving)
                {
                    return eHCMSResources.Z0172_G1_DangLuuDLieu;
                }
                if(_patientLoading)
                {
                    return eHCMSResources.Z0119_G1_DangLayTTinBN;
                }
                if (_registrationLoading)
                {
                    return eHCMSResources.Z0086_G1_DangLayTTinDK;
                }
                if(_isLoadingBill)
                {
                    return eHCMSResources.Z0151_G1_DangLayTTinBill;
                }
                return eHCMSResources.Z0153_G1_Pleasewait;
            }
        }
        public bool IsProcessing
        {
            get
            {
                return _patientLoading || _isSaving || _registrationLoading;
            }
        }

        private bool _isSaving;
        public bool IsSaving
        {
            get
            {
                return _isSaving;
            }
            set
            {
                _isSaving = value;
                NotifyOfPropertyChange(()=>IsSaving);

                NotifyWhenBusy();
            }
        }

        private bool _isLoadingBill;
        public bool IsLoadingBill
        {
            get
            {
                return _isLoadingBill;
            }
            set
            {
                _isLoadingBill = value;
                NotifyOfPropertyChange(() => IsLoadingBill);

                NotifyWhenBusy();
            }
        }

        //private bool _canChangePatientType = false;
        //public bool CanChangePatientType
        //{
        //    get
        //    {
        //        return _canChangePatientType;
        //    }
        //    set
        //    {
        //        if (_canChangePatientType != value)
        //        {
        //            _canChangePatientType = value;
        //            NotifyOfPropertyChange(() => CanChangePatientType);
        //        }
        //    }
        //}

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
                    NotifyOfPropertyChange(()=>CanSaveRegistrationAndPay);
                }
            }
        }
        private bool _canSaveRegistration;
        public bool CanSaveRegistration
        {
            get
            {
                return _canSaveRegistration;
            }
            set
            {
                if (_canSaveRegistration != value)
                {
                    _canSaveRegistration = value;
                    NotifyOfPropertyChange(() => CanSaveRegistration);
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
        private bool _registrationInfoHasChanged;
        /// <summary>
        /// Cho biet thong tin dang ky tren form da duoc thay doi chua.
        /// </summary>
        public bool RegistrationInfoHasChanged
        {
            get
            {
                return _registrationInfoHasChanged;
            }
            set
            {
                if (_registrationInfoHasChanged != value)
                {
                    _registrationInfoHasChanged = value;
                    NotifyOfPropertyChange(()=>RegistrationInfoHasChanged);
                    NotifyOfPropertyChange(()=> CanSaveCmd);
                    //ApplicationViewModel.Instance.IsProcessing = _registrationInfoHasChanged;
                    CanSearchPatient = !_registrationInfoHasChanged;

                    NotifyOfPropertyChange(() => CanCancelChangesCmd);
                    NotifyOfPropertyChange(() => CanSaveBillingInvoiceCmd);
                    NotifyOfPropertyChange(() => CanCreateNewBillCmd);
                    NotifyOfPropertyChange(() => CanStartEditRegistrationCmd);
                    //NotifyOfPropertyChange(() => CanCreateBillingInvoiceFromExistingItemsCmd);
                    NotifyOfPropertyChange(() => CanLoadBillCmd);
                }
            }
        }
       

        private PatientClassification _curClassification;
        public PatientClassification CurClassification
        {
            get
            {
                return _curClassification;
            }
            set
            {
                if (_curClassification != value)
                {
                    _curClassification = value;
                    NotifyOfPropertyChange(() => CurClassification);
                    NotifyOfPropertyChange(() => HiServiceBeingUsed);
                    //RegistrationSummaryVM.HIServiceBeingUsed = HIServiceBeingUsed;
                }
            }
        }
        public bool HiServiceBeingUsed
        {
            get
            {
                if (_curClassification == null)
                {
                    return false;
                }
                return _curClassification.PatientType == PatientType.INSUARED_PATIENT;
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

                PatientSummaryInfoContent.CurrentPatientClassification = CreateDefaultClassification();
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
                    NotifyOfPropertyChange(()=>CurRegistration);

                    NotifyOfPropertyChange(() => CanLoadBillCmd);
                    NotifyOfPropertyChange(() => CanAddEditBill);
                    if (CurRegistration != null)
                    {
                        if (CurRegistration.AdmissionInfo !=null && CurRegistration.AdmissionInfo.DischargeDate != null)
                        {
                            isDischarged = true;
                            NotifyOfPropertyChange(() => isDischarged);
                        }
                        else
                        {
                            isDischarged = false;
                            NotifyOfPropertyChange(() => isDischarged);
                        }
                        if (OldBillingInvoiceContent != null)
                        {
                            OldBillingInvoiceContent.CurentRegistration = CurRegistration;
                            OldBillingInvoiceContent.RegLockFlag = CurRegistration.RegLockFlag;
                            if (CurRegistration.HisID.GetValueOrDefault() > 0 && CurRegistration.PtInsuranceBenefit.GetValueOrDefault(0) > 0)
                            {
                                OldBillingInvoiceContent.ShowHIAppliedColumn = true;
                            }
                            else
                            {
                                OldBillingInvoiceContent.ShowHIAppliedColumn = false;
                            }
                        }
                    }
                    OldBillingInvoiceContent.CurentRegistration = CurRegistration;
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
        public bool CanRegister
        {
            get
            {
                return _isEditing && SelectedTabIndex == (int)InPatientRegistrationViewTab.EDITING_BILLING_INVOICE;
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
                    NotifyOfPropertyChange(()=>CurrentRegMode);
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
                NotifyOfPropertyChange(()=>SelectedDate);
            }
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

                                                             PatientClassifications = allClassifications != null ? new ObservableCollection<PatientClassification>(allClassifications) : null;
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
        private DateTime _regDate;
        public DateTime RegistrationDate
        {
            get
            {
                return _regDate;
            }
            set
            {
                _regDate = value;
                NotifyOfPropertyChange(() => RegistrationDate);
                if (CurRegistration != null && CurRegistration.PtRegistrationID <= 0)
                {
                    CurRegistration.ExamDate = _regDate;
                    if (_tempRegistration != null && _tempRegistration.PtRegistrationID == CurRegistration.PtRegistrationID)
                    {
                        _tempRegistration.ExamDate = _regDate;
                    }
                }
            }
        }

        private void InitFormData()
        {
            if (CurRegistration == null)
            {
                return;
            }
           
            //PatientRegItemsContent.SetRegistration(CurRegistration);
            var newBillingInvoiceList = new ObservableCollection<InPatientBillingInvoice>();
            var oldBillingInvoiceList = new ObservableCollection<InPatientBillingInvoice>();

            if(CurRegistration.InPatientBillingInvoices != null)
            {
                foreach (var inv in CurRegistration.InPatientBillingInvoices)
                {
                    if(inv.InPatientBillingInvID > 0)
                    {
                        /*==== #001 ====*/
                        inv.HIBenefit = this.CurRegistration.PtInsuranceBenefit;
                        inv.IsHICard_FiveYearsCont_NoPaid = this.CurRegistration.IsHICard_FiveYearsCont_NoPaid;
                        /*==== #001 ====*/
                        oldBillingInvoiceList.Add(inv);
                    }
                    else
                    {
                        newBillingInvoiceList.Add(inv);
                    }
                }
            }

            OldBillingInvoiceContent.BillingInvoices = oldBillingInvoiceList;
        }

        protected void ConfirmHi_OnBegin()
        {
            PatientSummaryInfoContent.CanConfirmHi = true;
            RegistrationInfoHasChanged = false;
        }

        protected void ConfirmHi_OnEnd()
        {
            PatientSummaryInfoContent.CanConfirmHi = false;
            RegistrationInfoHasChanged = false;
        }

        private void ShowOldRegistration(PatientRegistration regInfo)
        {
            if (//regInfo.V_RegistrationType != AllLookupValues.RegistrationType.NGOAI_TRU_NOI_TRU
                //&&
                regInfo.V_RegistrationType != AllLookupValues.RegistrationType.NOI_TRU && PatientFindBy == AllLookupValues.PatientFindBy.NOITRU)
            {
                MessageBox.Show(eHCMSResources.Z0085_G1_DayKhongPhaiDKNoiTru);
                return;
            }
            // Hpt 20/10/2015: Dang ky co trang thai REFUND (da huy) va da dong lai (COMPLETED) khong duoc tinh lai bill
            if (regInfo.RegistrationStatus == AllLookupValues.RegistrationStatus.REFUND || regInfo.RegistrationStatus == AllLookupValues.RegistrationStatus.COMPLETED)
            {
                MessageBox.Show(eHCMSResources.A0484_G1_Msg_InfoKhTheTinhLaiBill_Status);
                return;
            }
            // Dang ky noi tru chua nhap vien (AdmissionDate == null) hoac da nhap vien ma trang thai khac OPENED (= REFUND) thi khong hop le 
            if ((regInfo.AdmissionInfo != null && regInfo.RegistrationStatus != AllLookupValues.RegistrationStatus.OPENED)
                || (regInfo.AdmissionInfo == null && Globals.IsCasuaOrPreOpPt(regInfo.V_RegForPatientOfType) && regInfo.RegistrationStatus != AllLookupValues.RegistrationStatus.PENDING_INPT))
            {
                var converter = new EnumValueToStringConverter();
                var enumDescription = (string)converter.Convert(regInfo.RegistrationStatus, typeof(AllLookupValues.RegistrationStatus), null, Thread.CurrentThread.CurrentCulture);
                MessageBox.Show(string.Format("{0} {1}", eHCMSResources.A0684_G1_Msg_InfoKhThaoTacVoiDKNay, enumDescription));
                return;
            }  
            // Dang ky da qua han khong the tinh lai bill
            if (Globals.IsCasuaOrPreOpPt(regInfo.V_RegForPatientOfType) && regInfo.AdmissionInfo == null
                && regInfo.RegistrationStatus == AllLookupValues.RegistrationStatus.PENDING_INPT && !Globals.Check_CasualAndPreOpReg_StillValid(regInfo))
            {
                MessageBox.Show(eHCMSResources.A0492_G1_Msg_InfoKhTheTinhLaiBill2, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
            ConfirmHi_OnBegin();
            CurRegistration = regInfo;
            
            CurrentRegMode = RegistrationFormMode.OLD_REGISTRATION_OPENED;
            _confirmedHiItem = CurRegistration.HealthInsurance;
            _confirmedPaperReferal = CurRegistration.PaperReferal;
            NotifyOfPropertyChange(()=>ConfirmedHiItem);
            NotifyOfPropertyChange(()=>ConfirmedPaperReferal);
            InitRegistration();

            if (PatientSummaryInfoContent != null)
            {
                PatientSummaryInfoContent.CurrentPatient = CurPatient;

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
            //CanChangePatientType = false;

            bool readOnly = false;
            if (EditingInvoiceDetailsContent.BillingInvoice != null && CurRegistration.InPatientBillingInvoices != null
                && EditingInvoiceDetailsContent.BillingInvoice.PtRegistrationID == CurRegistration.PtRegistrationID)
            {
                foreach (var inv in CurRegistration.InPatientBillingInvoices)
                {
                    if(EditingInvoiceDetailsContent.BillingInvoice.InPatientBillingInvID == inv.InPatientBillingInvID)
                    {
                        EditingInvoiceDetailsContent.LoadDetails(UpdateBillingInvoiceCompletedCallback);
                        readOnly = true;
                    }
                }
            }
            if(!readOnly)
            {
                BeginEdit();    
            }
        }
        /// <summary>
        /// Gọi hàm này khi tạo mới một đăng ký, hoặc load xong một đăng ký đã có.
        /// Khởi tạo những giá trị cần thiết để đưa lên form
        /// </summary>
        private void InitRegistration()
        {
            _curPatient = CurRegistration.Patient;
            NotifyOfPropertyChange(() => CurPatient);

            InitFormData();
        }
        // HPT: Thêm hàm kiểm tra đăng ký được load lên có phải là đăng ký Vãng Lai Hoặc Tiền Giải Phẫu đã quá hạn hay không
        public IEnumerator<IResult> DoOpenRegistration(long regID)
        {
            //KMx: Chỉ lấy những thông tin cần thiết của đăng ký thôi, không load hết (17/09/2014 17:31).
            LoadRegistrationSwitch LoadRegisSwitch = new LoadRegistrationSwitch();
            LoadRegisSwitch.IsGetAdmissionInfo = true;
            LoadRegisSwitch.IsGetBillingInvoices = true;

            var loadRegTask = new LoadRegistrationInfo_InPtTask(regID, (int)PatientFindBy, LoadRegisSwitch);
            yield return loadRegTask;
            if (loadRegTask.Registration == null)
            {
                //Thong bao khong load duoc dang ky
                Globals.EventAggregator.Publish(new ShowMessageEvent { Message = "Error!(5)" });
            }
            else
            {
                ShowOldRegistration(loadRegTask.Registration);
            }
        }

        public void OpenRegistration(long regID)
        {
            RegistrationLoading = true;
            Coroutine.BeginExecute(DoOpenRegistration(regID), null, (o, e) => { RegistrationLoading = false; });
        }

        private PatientRegistration _tempRegistration;

        /// <summary>
        /// Chuan bi cong viec bat dau edit. Backup lai may cai object.
        /// </summary>
        public void BeginEdit()
        {
            RegistrationInfoHasChanged = false;
            _tempRegistration = CurRegistration.DeepCopy();
            if (EditingBillingInvoice == null)
            {
                EditingBillingInvoice = new InPatientBillingInvoice(); 
            }
            if (EditingBillingInvoice.InPatientBillingInvID <= 0)
            {
                EditingBillingInvoice.InvDate = RegistrationDate != DateTime.MinValue ? RegistrationDate : DateTime.Now;
                EditingBillingInvoice.V_InPatientBillingInvStatus = AllLookupValues.V_InPatientBillingInvStatus.NEW;
            }
            _tempBillingInvoice = EditingBillingInvoice.DeepCopy();
            IsEditing = true;
        }
        public void CancelEdit()
        {
            CurRegistration = _tempRegistration;
            _tempRegistration = null;
            EditingBillingInvoice = _tempBillingInvoice;
            IsBillOfUsedItems = false;
            EditingInvoiceDetailsContent.ResetView();

            _tempBillingInvoice = null;
            InitFormData();
            RegistrationInfoHasChanged = false;
            IsEditing = false;
        }

        public void EndEdit()
        {
            _tempRegistration = null;
            IsBillOfUsedItems = false;
        }

        // Hpt 20/10/2015: Trong Man hinh tinh lai bill khong can tim benh nhan ma chi can tim dang ky
        // Da xoa nut TIM BENH NHAN nen cac ham nay khong con can thiet nua. Trong lan toi se neu phan mem van hoat dong on dinh 

        //public void Handle(ResultFound<Patient> message)
        //{
        //    if (this.GetView() != null && message != null)
        //    {
        //        CurPatient = message.Result;
        //        CurRegistration = null;

        //        if (CurPatient != null)
        //        {
        //            SetCurrentPatient(CurPatient);
        //        }
        //    }
        //}

        //public void SetCurrentPatient(object patient)
        //{
        //    var p = patient as Patient;
        //    if (p == null || p.PatientID <= 0)
        //    {
        //        return;
        //    }
        //    ConfirmedHiItem = null;
        //    ConfirmedPaperReferal = null;

        //    EditingBillingInvoice = null;
        //    OldBillingInvoiceContent.BillingInvoices = null;

        //    if (p.PatientID > 0)
        //    {
        //        GetPatientByID(p.PatientID);
        //    }
        //    else
        //    {
        //        CurPatient = null;
        //        CurrentRegMode = RegistrationFormMode.PATIENT_NOT_SELECTED;
        //    }
        //}

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

        public void Handle(ItemSelected<PatientRegistration> message)
        {
            if (this.GetView() != null && message != null && message.Item != null)
            {
                OpenRegistration(message.Item.PtRegistrationID);
            }
        }        
        public void Handle(PayForRegistrationCompleted message)
        {
            if (this.GetView() == null || message == null)
            {
                return;
            }
            var payment = message.Payment;
            if (payment != null && payment.PatientTransaction != null && payment.PatientTransaction.PtRegistrationID.HasValue)
            {
                Action<IPaymentReport> onInitDlg = delegate (IPaymentReport reportVm)
                {
                    reportVm.PaymentID = payment.PtTranPaymtID;
                };
                GlobalsNAV.ShowDialog<IPaymentReport>(onInitDlg);

                OpenRegistration(payment.PatientTransaction.PtRegistrationID.Value);
            }
        }

        public void Handle(SaveAndPayForRegistrationCompleted message)
        {
            if (GetView() != null && message != null)
            {
                var payment = message.Payment;
                if (payment != null && payment.PatientTransaction != null && payment.PatientTransaction.PtRegistrationID.HasValue)
                {
                    //Show Report:
                    Action<IPaymentReport> onInitDlg = delegate (IPaymentReport reportVm)
                    {
                        reportVm.PaymentID = payment.PtTranPaymtID;
                    };
                    GlobalsNAV.ShowDialog<IPaymentReport>(onInitDlg);

                    if (message.RegistrationInfo != null)
                    {
                        ShowOldRegistration(message.RegistrationInfo);
                    }
                    else
                    {
                        OpenRegistration(payment.PatientTransaction.PtRegistrationID.Value);
                    }
                }
            }
        }
    
        public bool ValidateRegistrationInfo(out ObservableCollection<System.ComponentModel.DataAnnotations.ValidationResult> result)
        {
            result = new ObservableCollection<System.ComponentModel.DataAnnotations.ValidationResult>();

            if (PatientSummaryInfoContent.CurrentPatient == null)
            {
                var item = new System.ComponentModel.DataAnnotations.ValidationResult(eHCMSResources.Z0148_G1_HayChon1BN, new[] { "CurrentPatient" });
                result.Add(item);
            }

            if (CurClassification == null)
            {
                var item = new System.ComponentModel.DataAnnotations.ValidationResult(eHCMSResources.Z0154_G1_HayChonLoaiBN, new[] { "CurClassification" });
                result.Add(item);
            }
            if (CurRegistration.ExamDate == DateTime.MinValue)
            {
                var item = new System.ComponentModel.DataAnnotations.ValidationResult(eHCMSResources.Z0154_G1_NgDKKhongHopLe, new[] { "ExamDate" });
                result.Add(item);
            }

            if ((long)EditingBillingInvoice.V_BillingInvType < 0)
            {
                var item = new System.ComponentModel.DataAnnotations.ValidationResult(eHCMSResources.Z0155_G1_HayChonLoaiTToan, new[] { "BillingType" });
                result.Add(item);
            }

            CurRegistration.PatientClassification = CurClassification;
            if (ConfirmedPaperReferal != null)
            {
                CurRegistration.PaperReferal = ConfirmedPaperReferal;
            }


            if (EditingInvoiceDetailsContent.BillingInvoice == null)
            {
                var item = new System.ComponentModel.DataAnnotations.ValidationResult(eHCMSResources.Z0156_G1_Chon1DV, new string[] { "AllRegistrationItems" });
                result.Add(item);
            }

            if (CurRegistration.V_RegistrationType != AllLookupValues.RegistrationType.NOI_TRU)
            {
                var item = new System.ComponentModel.DataAnnotations.ValidationResult(eHCMSResources.Z0085_G1_DayKhongPhaiDKNoiTru, new string[] { "AllRegistrationItems" });
                result.Add(item);
            }

            CurRegistration.StaffID = Globals.LoggedUserAccount.StaffID;

            if (HiServiceBeingUsed)
            {
                //Dang la benh nhan bao hiem.
                //Kiem tra neu chua confirm the bao hiem thi thong bao loi.
                if (ConfirmedHiItem == null)
                {
                    var item = new System.ComponentModel.DataAnnotations.ValidationResult(eHCMSResources.Z0157_G1_ChuaKTraTheBH, new[] { "ConfirmedHiItem" });
                    result.Add(item);
                }
                CurRegistration.HealthInsurance = ConfirmedHiItem;

                long? hisID = null;
                if (ConfirmedHiItem != null
                    && ConfirmedHiItem.HealthInsuranceHistories != null
                    && ConfirmedHiItem.HealthInsuranceHistories.Count > 0)
                {
                    hisID = ConfirmedHiItem.HealthInsuranceHistories[0].HisID;
                }

                foreach (PatientRegistrationDetail d in CurRegistration.PatientRegistrationDetails)
                {
                    d.HisID = hisID;
                }
                CurRegistration.HisID = hisID;
            }

            CurRegistration.PatientID = PatientSummaryInfoContent.CurrentPatient.PatientID;
            CurRegistration.RegTypeID = (byte)PatientRegistrationType.DK_KHAM_BENH_NOI_TRU;
            CurRegistration.RefDepartment = Globals.ObjRefDepartment == null ? null : Globals.ObjRefDepartment;

            if (result.Count > 0)
            {
                return false;
            }
            return true;
        }
        public bool CanSaveBillingInvoiceCmd
        {
            get
            {
                return RegistrationInfoHasChanged;
            }
        }
        public void SaveBillingInvoiceCmd()
        {
            SaveBillingInvoice();
        }
        private void SaveBillingInvoice()
        {
            if(!CanAddEditBill)
            {
                MessageBox.Show(eHCMSResources.A0701_G1_Msg_InfoLuuFail);
                return;
            }
            if(EditingBillingInvoice == null)
            {
                return;
            }
            ObservableCollection<System.ComponentModel.DataAnnotations.ValidationResult> validationResults;
            if (!EditingInvoiceDetailsContent.ValidateInfo(out validationResults))
            {
                Action<IValidationError> onInitDlg = delegate (IValidationError errorVm)
                {
                    errorVm.SetErrors(validationResults);
                };
                GlobalsNAV.ShowDialog<IValidationError>(onInitDlg);
                return;
            }
            if(EditingBillingInvoice.InPatientBillingInvID <= 0)
            {
                //AddBillingInvoice();
            }
            else
            {
                UpdateBillingInvoice();
            }
        }
        private void UpdateBillingInvoice()
        {
            //IsSaving = true;

            //if (EditingBillingInvoice.OutwardDrugClinicDeptInvoices != null)
            //{
            //    foreach (var inv in EditingBillingInvoice.OutwardDrugClinicDeptInvoices)
            //    {
            //        if (inv.RecordState == RecordState.DETACHED)
            //        {
            //            inv.Confirmed = MedRegItemConfirmed;
            //        }
            //    }
            //}

            //var newRegDetails = new List<PatientRegistrationDetail>();
            //var modifiedRegDetails = new List<PatientRegistrationDetail>();
            //var deletedRegDetails = new List<PatientRegistrationDetail>();

            //if(EditingBillingInvoice.RegistrationDetails != null)
            //{
            //    foreach (var registrationDetail in EditingBillingInvoice.RegistrationDetails)
            //    {
            //        switch (registrationDetail.RecordState)
            //        {
            //            case RecordState.DELETED:
            //                deletedRegDetails.Add(registrationDetail);
            //                break;
            //            case RecordState.MODIFIED:
            //                modifiedRegDetails.Add(registrationDetail);
            //                break;
            //            case RecordState.DETACHED:
            //                newRegDetails.Add(registrationDetail);
            //                break;
            //        }
            //    }
            //}
            
            //var newPclRequests = new List<PatientPCLRequest>();
            //var newPclRequestDetails = new List<PatientPCLRequestDetail>();
            //var deletedPclRequestDetails = new List<PatientPCLRequestDetail>();
            //var modifiedPclRequestDetails = new List<PatientPCLRequestDetail>();
            //if(EditingBillingInvoice.PclRequests != null)
            //{
            //    foreach (var request in EditingBillingInvoice.PclRequests)
            //    {
            //        if (request.RecordState == RecordState.DETACHED)
            //        {
            //            newPclRequests.Add(request);
            //        }
            //        else if (request.RecordState == RecordState.MODIFIED)
            //        {
            //            if(request.PatientPCLRequestIndicators != null)
            //            {
            //                foreach (var requestDetail in request.PatientPCLRequestIndicators)
            //                {
            //                    if(requestDetail.RecordState == RecordState.DELETED)
            //                    {
            //                        deletedPclRequestDetails.Add(requestDetail);
            //                    }
            //                    else if(requestDetail.RecordState == RecordState.DETACHED)
            //                    {
            //                        newPclRequestDetails.Add(requestDetail);
            //                    }
            //                    else if (requestDetail.RecordState == RecordState.MODIFIED)
            //                    {
            //                        modifiedPclRequestDetails.Add(requestDetail);
            //                    }
            //                }
            //            }
            //        }
            //    }
            //}
            //var newOutwardDrugClinicDeptInvoices = new List<OutwardDrugClinicDeptInvoice>();
            //var modifiedOutwardDrugClinicDeptInvoices = new List<OutwardDrugClinicDeptInvoice>();
            ////KMx: List những invoices xóa khỏi bill để kho phòng chỉnh sửa phiếu xuất (20/08/2014 11:00).
            //var deleteOutwardDrugClinicDeptInvoices = new List<OutwardDrugClinicDeptInvoice>();

            //if (EditingBillingInvoice.OutwardDrugClinicDeptInvoices != null)
            //{
            //    foreach (var inv in EditingBillingInvoice.OutwardDrugClinicDeptInvoices)
            //    {
            //        if (inv.RecordState == RecordState.DETACHED)
            //        {
            //            newOutwardDrugClinicDeptInvoices.Add(inv);
            //        }
            //        else if (inv.RecordState == RecordState.MODIFIED)
            //        {
            //            modifiedOutwardDrugClinicDeptInvoices.Add(inv);
            //        }
            //        else if (inv.RecordState == RecordState.DELETED)
            //        {
            //            deleteOutwardDrugClinicDeptInvoices.Add(inv);
            //        }
            //    }
            //}

            //var bCanSave = false;
            //if(newRegDetails.Count > 0 || deletedRegDetails.Count > 0 || newPclRequests.Count > 0
            //|| newPclRequestDetails.Count > 0 || deletedPclRequestDetails.Count >0 || modifiedRegDetails.Count > 0||modifiedPclRequestDetails.Count >0)
            //{
            //    bCanSave = true;
            //}

            //if (!bCanSave && newOutwardDrugClinicDeptInvoices.Count > 0)
            //{
            //    if (newOutwardDrugClinicDeptInvoices.Any(inv => inv.OutwardDrugClinicDepts != null && inv.OutwardDrugClinicDepts.Count > 0))
            //    {
            //        bCanSave = true;
            //    }
            //}
            //if (!bCanSave && modifiedOutwardDrugClinicDeptInvoices.Count > 0)
            //{
            //    if (modifiedOutwardDrugClinicDeptInvoices.Any(inv => inv.OutwardDrugClinicDepts != null && inv.OutwardDrugClinicDepts.Count > 0))
            //    {
            //        bCanSave = true;
            //    }
            //}
            //if (!bCanSave && deleteOutwardDrugClinicDeptInvoices.Count > 0)
            //{
            //    bCanSave = true;
            //}

            //if (!bCanSave)
            //{
            //    MessageBox.Show("Không thể lưu bill.");
            //    IsSaving = false;
            //    return;
            //}

            //var t = new Thread(() =>
            //{
            //    AxErrorEventArgs error = null;
            //    IsSaving = true;
            //    try
            //    {
            //        using (var serviceFactory = new CommonServiceClient())
            //        {
            //            var contract = serviceFactory.ServiceInstance;

            //            var updatedInvoice = new InPatientBillingInvoice
            //                {
            //                    InPatientBillingInvID = EditingBillingInvoice.InPatientBillingInvID,
            //                    InvDate = EditingBillingInvoice.InvDate,
            //                    PaidTime = EditingBillingInvoice.PaidTime,
            //                    V_BillingInvType = EditingBillingInvoice.V_BillingInvType,
            //                    V_InPatientBillingInvStatus = EditingBillingInvoice.V_InPatientBillingInvStatus,
            //                    PtRegistrationID = EditingBillingInvoice.PtRegistrationID
            //                };

            //            contract.BeginUpdateInPatientBillingInvoice(Globals.LoggedUserAccount.StaffID, updatedInvoice,newRegDetails,deletedRegDetails,
            //                newPclRequests,newPclRequestDetails,deletedPclRequestDetails,newOutwardDrugClinicDeptInvoices,
            //                modifiedOutwardDrugClinicDeptInvoices, deleteOutwardDrugClinicDeptInvoices, modifiedRegDetails, modifiedPclRequestDetails,
            //                Globals.DispatchCallback(asyncResult =>
            //                {
            //                    var bOK = false;
            //                    //PatientRegistration registration = null;
            //                    Dictionary<long, List<long>> drugIDList_Error = null;
            //                    var regID = updatedInvoice.PtRegistrationID;
            //                    try
            //                    {
            //                        contract.EndUpdateInPatientBillingInvoice(out drugIDList_Error, asyncResult);
            //                        if (drugIDList_Error == null || drugIDList_Error.Count == 0)
            //                        {
            //                            bOK = true;
            //                            RegistrationInfoHasChanged = false;
            //                        }
            //                        else
            //                        {
            //                            bOK = false;
            //                        }
            //                    }
            //                    catch (FaultException<AxException> fault)
            //                    {
            //                        bOK = false;
            //                        error = new AxErrorEventArgs(fault);
            //                    }
            //                    catch (Exception ex)
            //                    {
            //                        bOK = false;
            //                        error = new AxErrorEventArgs(ex);
            //                    }
            //                    finally
            //                    {
            //                        if (drugIDList_Error != null && drugIDList_Error.Count > 0)
            //                        {
            //                            var vm = Globals.GetViewModel<IMedProductRemaingListing>();
            //                            vm.StartLoadingByIdList(drugIDList_Error);
            //                            vm.Title = eHCMSResources.K2948_G1_DSLgoaiThuocKhongTheLuu;
            //                            Globals.ShowDialog(vm as Conductor<object>);
            //                        }
            //                        if (bOK && regID > 0)
            //                        {
            //                            IsSaving = false;
            //                            EditingInvoiceDetailsContent.LoadDetails(UpdateBillingInvoiceCompletedCallback);

            //                            IsEditing = false;
            //                            RegistrationInfoHasChanged = false;
            //                        }
            //                    }


            //                }), updatedInvoice);
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        error = new AxErrorEventArgs(ex);
            //    }
            //    finally
            //    {
            //        IsSaving = false;
            //        if (error != null)
            //        {
            //            Globals.EventAggregator.Publish(new ErrorOccurred() { CurrentError = error });
            //        }
            //    }

            //});
            //t.Start();
        }

        public bool CanCancelChangesCmd
        {
            get
            {
                return RegistrationInfoHasChanged;
            }
        }
        public void CancelChangesCmd()
        {
            CancelEdit();
            BeginEdit();
        }

        public bool CanStartEditRegistrationCmd
        {
            get
            {
                return !RegistrationInfoHasChanged 
                    && (EditingBillingInvoice != null && EditingBillingInvoice.InPatientBillingInvID > 0)
                    && !IsEditing;
            }
        }
        public void StartEditRegistrationCmd()
        {
            BeginEdit();
        }
#region COROUTINES
        public IEnumerator<IResult> DoCalcHiBenefit(HealthInsurance hiItem, PaperReferal referal)
        {
            bool isEmergency = (CurRegistration.EmergRecID > 0 ? true : false);
            var calcHiTask = new CalcHiBenefitTask(hiItem, referal, (long)AllLookupValues.RegistrationType.NOI_TRU, isEmergency);
            yield return calcHiTask;
            if (calcHiTask.Error == null)
            {                                
                Action<IConfirmHiBenefit> onInitDlg = delegate (IConfirmHiBenefit vm)
                {
                    vm.VisibilityCbxAllowCrossRegion = Globals.ServerConfigSection.HealthInsurances.AllowInPtCrossRegion;
                    vm.OriginalHiBenefit = calcHiTask.HiBenefit;
                    vm.HiBenefit = calcHiTask.HiBenefit;
                    vm.HiId = calcHiTask.HiItem.HIID;
                    vm.PatientId = CurPatient.PatientID;
                    vm.OriginalIsCrossRegion = calcHiTask.IsCrossRegion;
                    vm.SetCrossRegion(calcHiTask.IsCrossRegion);
                };
                GlobalsNAV.ShowDialog<IConfirmHiBenefit>(onInitDlg);
            }
        }
#endregion

        public void ShowBillingReport(long inPatientBillingInvId)
        {
            Action<IBillingInvoiceDetailsReport> onInitDlg = delegate (IBillingInvoiceDetailsReport reportVm)
            {
                reportVm.InPatientBillingInvID = inPatientBillingInvId;
            };
            GlobalsNAV.ShowDialog<IBillingInvoiceDetailsReport>(onInitDlg, null, false, true);
        }

        public bool CanPrintOldBillingInvoiceCmd
        {
            get
            {
                return true;
            }
        }
        public void PrintOldBillingInvoiceCmd()
        {
            List<long> ids = OldBillingInvoiceContent.GetSelectedIds();
            if(ids != null && ids.Count > 0)
            {
                ShowBillingReport(ids[0]);
            }
            else
            {
                MessageBox.Show(eHCMSResources.A0591_G1_Msg_InfoChonDVDeIn);
            }
        }

        public void AddBillingInvoiceCompletedCallback(InPatientBillingInvoice inv)
        {
            if(CurRegistration != null && CurRegistration.PtRegistrationID > 0
                &&inv.PtRegistrationID == CurRegistration.PtRegistrationID)
            {
                var bExists = false;
                if(CurRegistration.InPatientBillingInvoices != null)
                {
                    if (CurRegistration.InPatientBillingInvoices.Any(temp => inv == temp))
                    {
                        bExists = true;
                    }
                    if (!bExists)
                    {
                        CurRegistration.InPatientBillingInvoices.Add(inv);

                        if (OldBillingInvoiceContent.BillingInvoices != null)
                        {
                            OldBillingInvoiceContent.BillingInvoices.Add(inv);
                        }
                    }
                }
                if (EditingBillingInvoice != null && EditingBillingInvoice.InPatientBillingInvID == inv.InPatientBillingInvID)
                {
                    EditingBillingInvoice = inv;    
                }
            }
        }

        public void UpdateBillingInvoiceCompletedCallback(InPatientBillingInvoice inv)
        {
            if (CurRegistration != null && CurRegistration.PtRegistrationID > 0
                && inv.PtRegistrationID == CurRegistration.PtRegistrationID)
            {
                int idx = -1;
                if (CurRegistration.InPatientBillingInvoices != null)
                {
                    for (int i = 0; i < CurRegistration.InPatientBillingInvoices.Count;i++ )
                    {
                        if (inv.InPatientBillingInvID == CurRegistration.InPatientBillingInvoices[i].InPatientBillingInvID)
                        {
                            idx = i;
                            break;
                        }
                    }
                    if (idx >= 0)
                    {
                        CurRegistration.InPatientBillingInvoices[idx] = inv;
                        if (OldBillingInvoiceContent.BillingInvoices != null
                            && OldBillingInvoiceContent.BillingInvoices.Count > idx
                            && OldBillingInvoiceContent.BillingInvoices[idx].InPatientBillingInvID == inv.InPatientBillingInvID)//Cho chac an.
                        {
                            OldBillingInvoiceContent.BillingInvoices[idx] = inv;

                            //KMx: Khi Update xong bill thì không cần gán lại, vì khi xem chi tiết thì cũng phải đi load lại (13/09/2014 15:20).
                            //if(OldBillingInvoiceContent.InvoiceDetailsContent.BillingInvoice != null
                            //    &&OldBillingInvoiceContent.InvoiceDetailsContent.BillingInvoice.InPatientBillingInvID == inv.InPatientBillingInvID)
                            //{
                            //    OldBillingInvoiceContent.RefreshDetailsView(inv);
                            //}
                        }
                        
                        if(EditingBillingInvoice != null && EditingBillingInvoice.InPatientBillingInvID == inv.InPatientBillingInvID)
                        {
                            EditingBillingInvoice = inv;    
                        }
                    }
                }
            }
        }

        public void EditBillingInvoice(InPatientBillingInvoice inv)
        {
            if(inv != null)
            {
                EditingBillingInvoice = inv;
                BeginEdit();
            }
        }
        public bool CanCreateNewBillCmd
        {
            get
            {
                return !RegistrationInfoHasChanged;
            }
        }
        public void CreateNewBillCmd()
        {
            EditingBillingInvoice = new InPatientBillingInvoice();
            if(EditingBillingInvoice.V_BillingInvType == 0)
            {
                EditingBillingInvoice.V_BillingInvType = AllLookupValues.V_BillingInvType.TINH_TIEN_NOI_TRU;
            }
            BeginEdit();
        }

        private int _selectedTabIndex;
        public int SelectedTabIndex
        {
            get
            {
                return _selectedTabIndex;
            }
            set
            {
                if (_selectedTabIndex != value)
                {
                    _selectedTabIndex = value;
                    NotifyOfPropertyChange(() => SelectedTabIndex);
                    NotifyOfPropertyChange(() => CanRegister);
                }
            }
        }
        public void tabBillingInvoiceInfo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(RegistrationInfoHasChanged)
            {
                //Giu lai tab index cu.
                var tabCtrl = sender as TabControl;
                if(tabCtrl != null && tabCtrl.SelectedIndex != SelectedTabIndex)
                {
                    MessageBoxResult result = MessageBox.Show(string.Format("{0}. {1}", eHCMSResources.Z0446_G1_TTinDaThayDoi, eHCMSResources.A0138_G1_Msg_ConfBoQua), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel);
                    if (result == MessageBoxResult.OK)
                    {
                        CancelEdit();
                    }
                    else
                    {
                        tabCtrl.SelectedIndex = SelectedTabIndex;
                    }
                }
            }
        }

        public void ReturnDrugCmd()
        {
            if (GetView() != null)
            {
                Action<IInPatientReturnDrug> onInitDlg = delegate (IInPatientReturnDrug vm)
                {
                    vm.MedProductType = AllLookupValues.MedProductType.THUOC;
                    vm.Registration = CurRegistration;
                    vm.InitData(null);
                };
                GlobalsNAV.ShowDialog<IInPatientReturnDrug>(onInitDlg);
            }
        }
        //public void hplkNhapVien()
        //{
        //    {
        //        var vm = Globals.GetViewModel<IfrmPatientAdmission>();
        //        vm.curPatientRegistration= CurRegistration;
                
        //        Globals.ShowDialog(vm as Conductor<object>);
        //    }
        //}
        public bool CanLoadBillCmd
        {
            get
            {
                return (CurRegistration != null && CurRegistration.PtRegistrationID > 0)
                    && !RegistrationInfoHasChanged;
            }
        }
        /// <summary>
        /// La bill moi (chua insert vo db) cua nhung item da su dung.
        /// </summary>
        private bool _isBillOfUsedItems;
        public bool IsBillOfUsedItems
        {
            get
            {
                return _isBillOfUsedItems;
            }
            set
            {
                _isBillOfUsedItems = value;
                NotifyOfPropertyChange(() => IsBillOfUsedItems);
                EditingInvoiceDetailsContent.CanEditOnGrid = _isEditing || IsBillOfUsedItems;

                CanCalcPaymentToEndOfDay = _isBillOfUsedItems;
            }
        }
        //public void LoadBillCmd()
        //{
        //    var t = new Thread(() =>
        //    {
        //        AxErrorEventArgs error = null;
        //        IsLoadingBill = true;
        //        try
        //        {
        //            using (var serviceFactory = new CommonServiceClient())
        //            {
        //                var contract = serviceFactory.ServiceInstance;

        //                contract.BeginLoadInPatientRegItemsIntoBill(CurRegistration.PtRegistrationID,
        //                    Globals.DispatchCallback(asyncResult =>
        //                    {
        //                        try
        //                        {
        //                            var inv = contract.EndLoadInPatientRegItemsIntoBill(asyncResult);
        //                            if(inv != null)
        //                            {
        //                                if(CheckIfInvoiceIsEmpty(inv))
        //                                {
        //                                    MessageBox.Show(eHCMSResources.A0729_G1_Msg_InfoKhTimThayBill);   
        //                                }
        //                                else
        //                                {
        //                                    EditingBillingInvoice = inv;
        //                                    IsEditing = false;
        //                                    RegistrationInfoHasChanged = true;
        //                                    IsBillOfUsedItems = true;
        //                                }
        //                            }
        //                        }
        //                        catch (FaultException<AxException> fault)
        //                        {
        //                            error = new AxErrorEventArgs(fault);
        //                        }
        //                        catch (Exception ex)
        //                        {
        //                            error = new AxErrorEventArgs(ex);
        //                        }
        //                        finally
        //                        {
        //                            Globals.IsBusy = false;
        //                        }


        //                    }), null);
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            error = new AxErrorEventArgs(ex);
        //        }
        //        finally
        //        {
        //            IsLoadingBill = false;
        //            if (error != null)
        //            {
        //                Globals.EventAggregator.Publish(new ErrorOccurred { CurrentError = error });
        //            }
        //        }

        //    });
        //    t.Start();
        //}

        //private bool CheckIfInvoiceIsEmpty(InPatientBillingInvoice inv)
        //{
        //    if(inv.RegistrationDetails != null && inv.RegistrationDetails.Count > 0)
        //    {
        //        return false;
        //    }
        //    if (inv.PclRequests != null && inv.PclRequests.Count > 0)
        //    {
        //        if (inv.PclRequests.Any(req => req.PatientPCLRequestIndicators != null && req.PatientPCLRequestIndicators.Count > 0))
        //        {
        //            return false;
        //        }
        //    }
        //    if (inv.OutwardDrugClinicDeptInvoices != null && inv.OutwardDrugClinicDeptInvoices.Count > 0)
        //    {
        //        return inv.OutwardDrugClinicDeptInvoices.All(drugInv => drugInv.OutwardDrugClinicDepts == null || drugInv.OutwardDrugClinicDepts.Count <= 0);
        //    }
        //    return true;
        //}

        
        public void ConfirmHIItem(object hiItem)
        {
            ConfirmedHiItem = hiItem as HealthInsurance;
            if (CurRegistration != null)
            {
                CurRegistration.HealthInsurance = ConfirmedHiItem;
            }            
        }
        public void ConfirmPaperReferal(object referal)
        {
            ConfirmedPaperReferal = referal as PaperReferal;
            if (CurRegistration != null)
            {
                CurRegistration.PaperReferal = ConfirmedPaperReferal;
            }            
        }
        public void Handle(HiCardConfirmedEvent message)
        {
            if (message != null)
            {
                ConfirmHIItem(message.HiProfile);
                
                //PatientSummaryInfoContent.ConfirmedHiItem = message.HiProfile;
                ConfirmPaperReferal(message.PaperReferal);
                RegistrationInfoHasChanged = true;
                if (message.HiProfile != null)
                {
                    //Tinh lai quyen loi bao hiem.
                    Coroutine.BeginExecute(DoCalcHiBenefit(CurRegistration.HealthInsurance, CurRegistration.PaperReferal), null, (o, e) =>
                    {
                        var vm = message.Source as IPatientDetails;
                        if (vm != null)
                        {
                            vm.Close();
                        }
                    });
                }
            }
        }
        public void Handle(RemoveConfirmedHiCard message)
        {
            if (GetView() == null || message.HiId <= 0)
            {
                return;
            }

            if (CurRegistration != null && CurRegistration.PtRegistrationID > 0)
            {
                return;
            }
            if (ConfirmedHiItem != null && ConfirmedHiItem.HIID == message.HiId)
            {
                ConfirmedHiItem = null;
                ConfirmedPaperReferal = null;

                PatientSummaryInfoContent.SetPatientHISumInfo(null);

                if (CurRegistration != null)
                {
                    CurRegistration.HealthInsurance = null;
                    CurRegistration.PaperReferal = null;
                }
            }

        }
        public void Handle(ConfirmHiBenefit message)
        {
            if (CurRegistration == null || ConfirmedHiItem == null || ConfirmedHiItem.HIID != message.HiId)
            {
                return;
            }

            CurRegistration.IsCrossRegion = message.IsCrossRegion;
            
        }
        
        public bool CanSaveCmd
        {
            get
            {
                return RegistrationInfoHasChanged;
            }
        }

        // Hpt 08/12/2015: Hàm SaveCmd dưới đây được gọi khi xác click nút Lưu của ViewModel tính lại bill. Tuy nhiên nút này đã được enable nên hàm không còn sử dụng nữa. Comment lại test xong sẽ xóa
        //public void SaveCmd()
        //{
            //if(!CanSaveCmd)
            //{
            //    MessageBox.Show("Không thể xác nhận thẻ BH");
            //    return;
            //}
            //if(ConfirmedHiItem == null)
            //{
            //    MessageBox.Show(eHCMSResources.A0399_G1_Msg_InfoChuaChonTheBHYTDeXN);
            //    return;
            //}
            //long? PaperReferalID = null;
            //if (ConfirmedPaperReferal != null)
            //{
            //    PaperReferalID = ConfirmedPaperReferal.RefID;
            //}
            //var benefit = PatientSummaryInfoContent.HiBenefit;
            //// HPT 24/08/2015:hàm này sử dụng lại hàm ApplyHiToInPatientRegistration- được gọi khi xác nhận BHYT nên khi thêm parameter vào ApplyHiToInPatientRegistration để tính quyền lợi, phải sửa luôn hàm này
            //// Khai báo biến cục bộ IsHICard_FiveYearsCont để lưu giá trị thuộc tính này của đối tượng CurRegistration, truyền vào cho parameter của hàm ApplyHiToInPatientRegistration
            //// Nếu sử dụng CurRegistration.IsHICard_FiveYearsCont để truyền vào hàm ApplyHiToInPatientRegistration, trong trường hợp CurRegistration = null, sẽ gây ra lỗi
            //// Do đó cần khai báo biến tạm IsHICard_FiveYearsCont. Khi CurRegistration = null, biến này sẽ được mặc định bằng false để tránh gây ra lỗi
            //bool IsHICard_FiveYearsCont = false;
            //if (CurRegistration != null)
            //{
            //    IsHICard_FiveYearsCont = CurRegistration.IsHICard_FiveYearsCont;
            //}

            //var t = new Thread(() =>
            //{
            //    try
            //    {
            //        using (var serviceFactory = new PatientRegistrationServiceClient())
            //        {
            //            var contract = serviceFactory.ServiceInstance;

            //            contract.BeginApplyHiToInPatientRegistration(CurRegistration.PtRegistrationID,
            //                //HIID,HisID,HiBenefit,IsCrossRegion,PaperReferalID
            //                ConfirmedHiItem.HIID, benefit.Value, PatientSummaryInfoContent.IsCrossRegion, PaperReferalID,(int)AllLookupValues.V_FindPatientType.NOI_TRU,
            //                (CurRegistration.EmergRecID > 0 ? true : false ), Globals.LoggedUserAccount.StaffID.GetValueOrDefault(), IsHICard_FiveYearsCont, 
            //                Globals.DispatchCallback((asyncResult) =>
            //                {
            //                    try
            //                    {
            //                        var bOK = contract.EndApplyHiToInPatientRegistration(asyncResult);
            //                        MessageBox.Show(eHCMSResources.K0461_G1_XNhanBHOk);
            //                        //ConfirmHi_OnEnd();
            //                        //KMx: Sau khi xác nhận BH thành công phải load lại Registration để có HisID, mới in giấy xác nhận BHYT được (02/12/2014 16:05).
            //                        OpenRegistration(CurRegistration.PtRegistrationID);
            //                    }
            //                    catch (Exception ex)
            //                    {
            //                        ClientLoggerHelper.LogInfo(ex.ToString());
            //                    }
                                
            //                }), null);
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
            //    }
            //});
            //t.Start();
        //}

        public void ReportRegistrationInfoInsuranceCmd()
        {
            if (CurRegistration == null || CurRegistration.PtRegistrationID <= 0)
            {
                MessageBox.Show(eHCMSResources.A0733_G1_Msg_InfoKhTimThayDK, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
            if (CurRegistration.HisID.GetValueOrDefault() <= 0)
            {
                MessageBox.Show(eHCMSResources.A0493_G1_Msg_InfoKhTheInGiayXNhanBHYT, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
            Action<ICommonPreviewView> onInitDlg = delegate (ICommonPreviewView reportVm)
            {
                reportVm.RegistrationID = CurRegistration.PtRegistrationID;
                reportVm.eItem = ReportName.REGISTRATION_IN_PATIENT_HI_CONFIRMATION;
            };
            GlobalsNAV.ShowDialog<ICommonPreviewView>(onInitDlg);
        }

        public void Handle(ItemChanged<InPatientBillingInvoice, IInPatientBillingInvoiceListing> message)
        {
            if(message.Item != null)
            {
                OpenRegistration(message.Item.OutPtRegistrationID > 0 ? message.Item.OutPtRegistrationID : message.Item.PtRegistrationID);
            }
        }

        public void Handle(ReloadRegisAfterRecalcBillInvoiceEvent message)
        {
            if (message.Item != null)
            {
                OpenRegistration(message.Item.OutPtRegistrationID > 0 ? message.Item.OutPtRegistrationID : message.Item.PtRegistrationID);
            }
        }
        public void OldRegistrationsCmd()
        {
            if (CurPatient != null)
            {
                Action<IRegistrationList> onInitDlg = delegate (IRegistrationList vm)
                {
                    vm.IsInPtRegistration = true;
                    vm.CurrentPatient = CurPatient;
                };
                GlobalsNAV.ShowDialog<IRegistrationList>(onInitDlg);
            }
        }
        //▼==== #001
        public void Handle(InPatientRegistrationSelectedForInPatientBillingInvoiceListing message)
        {
            OpenRegistration(message.Source.PtRegistrationID);
        }
        //▲==== #001
    }
}
