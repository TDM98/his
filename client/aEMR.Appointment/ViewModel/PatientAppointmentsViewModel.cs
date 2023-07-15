using System.ComponentModel.Composition;
using System.Windows;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ViewContracts;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Caliburn.Micro;
using DataEntities;
using aEMR.Common;
using System.Collections.Generic;
using aEMR.CommonTasks;
using eHCMSLanguage;
using System;
using aEMR.Common.BaseModel;
using aEMR.Common.ViewModels;
using System.Threading;
using aEMR.ServiceClient;
using System.Collections.ObjectModel;
using System.Linq;
using aEMR.Common.Collections;

namespace aEMR.Appointment.ViewModels
{
    [Export(typeof(IPatientAppointments)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class PatientAppointmentsViewModel : ViewModelBase, IPatientAppointments
        , IHandle<ResultFound<Patient>>
        , IHandle<ResultNotFound<Patient>>
        , IHandle<ItemSelected<Patient>>
        , IHandle<CreateNewPatientEvent>
        , IHandle<AddCompleted<Patient>>
        , IHandle<AppointmentAddEditCloseEvent>
        , IHandle<SearchAppointmentResultEvent>
        , IHandle<ItemSelected<PatientRegistration>>
    {
        [ImportingConstructor]
        public PatientAppointmentsViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            Globals.EventAggregator.Subscribe(this);
            //Globals.EventAggregator.Subscribe(this);
            var searchPatientAndRegVm = Globals.GetViewModel<ISearchPatientAndRegistration>();
            searchPatientAndRegVm.InitButtonVisibility(SearchRegButtonsVisibility.SHOW_SEARCH_PATIENT_BTN | SearchRegButtonsVisibility.SHOW_NEW_PATIENT_BTN);
            searchPatientAndRegVm.SetDefaultButton(SearchRegistrationButtons.SEARCH_PATIENT);
            searchPatientAndRegVm.mTimDangKy = false;
            searchPatientAndRegVm.PatientFindBy = AllLookupValues.PatientFindBy.NGOAITRU;
            SearchRegistrationContent = searchPatientAndRegVm;
            ActivateItem(searchPatientAndRegVm);
            var appointmentListingVm = Globals.GetViewModel<IAppointmentListing>();
            AppointmentListingContent = appointmentListingVm;
            ActivateItem(appointmentListingVm);

            PatientSummaryInfoContent = Globals.GetViewModel<IPatientSummaryInfoV2>();
            PatientSummaryInfoContent.mInfo_CapNhatThongTinBN = true;
            PatientSummaryInfoContent.mInfo_XacNhan = false;
            PatientSummaryInfoContent.mInfo_XoaThe = false;
            PatientSummaryInfoContent.mInfo_XemPhongKham = false;
        }
        protected override void OnActivate()
        {
            base.OnActivate();
            Authorization();
            if (IsShowSearchRegistrationButton)
            {
                Globals.PatientFindBy_ForConsultation = (long)AllLookupValues.PatientFindBy.NGOAITRU;
                SearchRegistrationContent.InitButtonVisibility(SearchRegButtonsVisibility.SHOW_SEARCH_REG_BTN);
                SearchRegistrationContent.SetDefaultButton(SearchRegistrationButtons.SEARCH_REGISTRATION);
                SearchRegistrationContent.mTimDangKy = true;
                ActivateItem(PatientSummaryInfoContent);
            }
        }
        #region Properties
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

        private IAppointmentListing _appointmentListingContent;
        public IAppointmentListing AppointmentListingContent
        {
            get { return _appointmentListingContent; }
            set
            {
                _appointmentListingContent = value;
                NotifyOfPropertyChange(() => AppointmentListingContent);
            }
        }

        private bool _isAppointment;
        public bool isAppointment
        {
            get { return _isAppointment; }
            set
            {
                _isAppointment = value;
                NotifyOfPropertyChange(() => isAppointment);
            }
        }

        private bool _HasPatient = false;
        public bool HasPatient
        {
            get { return _HasPatient; }
            set
            {
                _HasPatient = value;
                NotifyOfPropertyChange(() => HasPatient);
            }
        }

        private AppointmentSearchCriteria _searchCriteria;
        public AppointmentSearchCriteria SearchCriteria
        {
            get { return _searchCriteria; }
            set
            {
                _searchCriteria = value;
                NotifyOfPropertyChange(() => SearchCriteria);
            }
        }
        private Patient _currentPatient;
        public Patient CurrentPatient
        {
            get
            {
                return _currentPatient;
            }
            set
            {
                if (_currentPatient != value)
                {
                    _currentPatient = value;
                    NotifyOfPropertyChange(() => CurrentPatient);
                    //Load lai du lieu hen cho benh nhan nay.
                    //_AllAppointmentVM.PageIndex = 0;
                    //_AllAppointmentVM.CountTotal = true;
                    //if (_currentPatient != null)
                    //{
                    //    _AllAppointmentVM.SearchCriteria.PatientID = _currentPatient.PatientID;
                    //    _AllAppointmentVM.LoadData();
                    //}
                    //else
                    //{
                    //    _AllAppointmentVM.ClearData();
                    //}
                }
            }
        }
        //1: Hẹn bệnh CLS sổ, 0: Hẹn tái khám
        private bool _IsPCLBookingView = false;
        public bool IsPCLBookingView
        {
            get
            {
                return _IsPCLBookingView;
            }
            set
            {
                if (_IsPCLBookingView == value)
                {
                    return;
                }
                _IsPCLBookingView = value;
                NotifyOfPropertyChange(() => IsPCLBookingView);
                NotifyOfPropertyChange(() => IsVisibleUCPatientInfo);
                NotifyOfPropertyChange(() => IsVisibleSearchContent);
            }
        }
        private IRegistration_DataStorage _Registration_DataStorage;
        public IRegistration_DataStorage Registration_DataStorage
        {
            get
            {
                return _Registration_DataStorage;
            }
            set
            {
                if (_Registration_DataStorage == value)
                {
                    return;
                }
                _Registration_DataStorage = value;
                NotifyOfPropertyChange(() => Registration_DataStorage);
            }
        }
        //CMN: Cờ cuộc hẹn tái khám khi load tại màn hình toa thuốc để lấy các dữ liệu cần thiết mang đi làm các điều kiện kiểm tra
        private long _CurrentPtRegDetailAppointmentID;
        public long CurrentPtRegDetailAppointmentID
        {
            get
            {
                return _CurrentPtRegDetailAppointmentID;
            }
            set
            {
                if (_CurrentPtRegDetailAppointmentID == value)
                {
                    return;
                }
                _CurrentPtRegDetailAppointmentID = value;
                NotifyOfPropertyChange(() => CurrentPtRegDetailAppointmentID);
            }
        }
        private Prescription CurrentPrescription;
        private bool _IsShowSearchRegistrationButton = false;
        public bool IsShowSearchRegistrationButton
        {
            get
            {
                return _IsShowSearchRegistrationButton;
            }
            set
            {
                if (_IsShowSearchRegistrationButton == value)
                {
                    return;
                }
                _IsShowSearchRegistrationButton = value;
                NotifyOfPropertyChange(() => IsShowSearchRegistrationButton);
                NotifyOfPropertyChange(() => IsVisibleUCPatientInfo);
                NotifyOfPropertyChange(() => IsVisibleSearchContent);
            }
        }
        private IPatientSummaryInfoV2 _PatientSummaryInfoContent;
        public IPatientSummaryInfoV2 PatientSummaryInfoContent
        {
            get { return _PatientSummaryInfoContent; }
            set
            {
                if (_PatientSummaryInfoContent == value)
                {
                    return;
                }
                _PatientSummaryInfoContent = value;
                NotifyOfPropertyChange(() => PatientSummaryInfoContent);
            }
        }
        public bool IsVisibleUCPatientInfo
        {
            get
            {
                return !IsPCLBookingView && !IsShowSearchRegistrationButton;
            }
        }
        public bool IsVisibleSearchContent
        {
            get
            {
                return !IsPCLBookingView || IsShowSearchRegistrationButton;
            }
        }
        #endregion
        private void CallSetCurrentPatient(Patient patient, Prescription aCurrentPrescription = null)
        {
            CurrentPrescription = aCurrentPrescription;
            CurrentPatient = patient;
            //CMN: Dời bên trong hàm gán CurrentPatient ra bên ngoài để tạo thành riêng một method
            _searchCriteria = new AppointmentSearchCriteria();
            _searchCriteria.PatientID = _currentPatient.PatientID;
            if (_currentPatient != null)
            {
                AppointmentListingContent.SearchCriteria = _searchCriteria;
                AppointmentListingContent.SearchCriteria.OrderBy = "RecDateCreated";
                /*
                 * CMN: Kiểm tra thấy danh sách cuộc hẹn được load tại đây, sau khi gán bệnh nhân.
                 * Biến Registration_DataStorage luôn có giá trị khi màn hình được mở từ toa thuốc.
                 * Dựa vào cờ nếu là màn hình load từ toa thuốc thì thêm biến để load chỉ các cuộc hẹn được tạo ra từ đăng ký hiện tại.
                */
                if (IsPCLBookingView)
                {
                    AppointmentListingContent.SearchCriteria.PtRegistrationID = Registration_DataStorage.CurrentPatientRegistration.PtRegistrationID;
                }
                AppointmentListingContent.StartSearching();
            }
        }
        public void SetCurrentPatient(Patient patient, Prescription aCurrentPrescription = null)
        {
            CallSetCurrentPatient(patient, aCurrentPrescription);
        }
        public void Handle(ResultFound<Patient> message)
        {
            if (message != null && this.GetView() != null)
            {
                HasPatient = true;
                SetCurrentPatient(message.Result);
            }
        }
        public void Handle(ResultNotFound<Patient> message)
        {
            if (message != null && this.GetView() != null)
            {
                //Thông báo không tìm thấy bệnh nhân.
                HasPatient = false;
                AppointmentListingContent.ClearItemSource();
                if (Globals.HomeModuleActive == HomeModuleActive.HENBENH)
                {
                    MessageBox.Show(eHCMSResources.Z0478_G1_KhongTimThayBenhNhan, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                }
            }
        }
        public void Handle(ItemSelected<Patient> message)
        {
            if (message != null && this.GetView() != null)
            {
                SetCurrentPatient(message.Item as Patient);
            }
        }
        public void CreateNewAppointmentCmd()
        {
            Coroutine.BeginExecute(CreateNewAppointmentCoroutine());
        }
        //CMN: Hàm Show dialog xác nhận chẩn đoán cho một ca đăng ký có 2 dịch vụ khám và sử dụng màn hình hẹn bệnh của điều dưỡng thao tác
        private bool GetConfirmDiagnosisTreatment()
        {
            if (Registration_DataStorage != null && Registration_DataStorage.PatientServiceRecordCollection != null && Registration_DataStorage.PatientServiceRecordCollection.Any(x => x.DiagnosisTreatments != null))
            {
                DiagnosisTreatment CurrentDiagnosisTreatment = Registration_DataStorage.PatientServiceRecordCollection.LastOrDefault().DiagnosisTreatments.FirstOrDefault();
                if (Registration_DataStorage.PatientServiceRecordCollection.Count > 1)
                {
                    IConfirmDiagnosisTreatment DialogView = Globals.GetViewModel<IConfirmDiagnosisTreatment>();
                    DialogView.ApplyDiagnosisTreatmentCollection(Registration_DataStorage.PatientServiceRecordCollection.SelectMany(x => x.DiagnosisTreatments).ToList());
                    DialogView.V_RegistrationType = (long)AllLookupValues.RegistrationType.NGOAI_TRU;
                    GlobalsNAV.ShowDialog_V3(DialogView, null, null, false, true, new Size(1200, 600));
                    if (DialogView.CurrentDiagnosisTreatment == null)
                    {
                        return false;
                    }
                    if (Registration_DataStorage.PatientServiceRecordCollection.Any(x => x.ServiceRecID == DialogView.CurrentDiagnosisTreatment.ServiceRecID) &&
                        Registration_DataStorage.PatientServiceRecordCollection.FirstOrDefault(x => x.ServiceRecID == DialogView.CurrentDiagnosisTreatment.ServiceRecID).PrescriptionIssueHistories != null &&
                        Registration_DataStorage.PatientServiceRecordCollection.FirstOrDefault(x => x.ServiceRecID == DialogView.CurrentDiagnosisTreatment.ServiceRecID).PrescriptionIssueHistories.Any(x => x.Prescription != null))
                    {
                        var CurrentIssue = Registration_DataStorage.PatientServiceRecordCollection.FirstOrDefault(x => x.ServiceRecID == DialogView.CurrentDiagnosisTreatment.ServiceRecID).PrescriptionIssueHistories.LastOrDefault(x => x.Prescription != null);
                        CurrentPrescription = CurrentIssue.Prescription;
                        if (Registration_DataStorage.CurrentPatientRegistration != null && Registration_DataStorage.CurrentPatientRegistration.PatientRegistrationDetails != null &&
                            Registration_DataStorage.CurrentPatientRegistration.PatientRegistrationDetails.Any(x => x.PtRegDetailID == CurrentIssue.PtRegDetailID))
                        {
                            Registration_DataStorage.CurrentPatientRegistrationDetail = Registration_DataStorage.CurrentPatientRegistration.PatientRegistrationDetails.FirstOrDefault(x => x.PtRegDetailID == CurrentIssue.PtRegDetailID);
                        }
                    }
                }
            }
            return true;
        }
        public void CreateNewAppointment()
        {
            if (IsShowSearchRegistrationButton && !GetConfirmDiagnosisTreatment())
            {
                return;
            }
            //var apptVm = Globals.GetViewModel<IAddEditAppointment>();
            //apptVm.SetCurrentPatient(CurrentPatient);
            //apptVm.CreateNewAppointment();
            //Globals.ShowDialog(apptVm as Conductor<object>);
            if (Registration_DataStorage != null && Registration_DataStorage.CurrentPatientRegistration != null &&
                Registration_DataStorage.CurrentPatientRegistration.PtRegistrationID > 0)
            {
                PatientAppointment CurrentAppointment = new PatientAppointment();
                CurrentAppointment.DoctorStaffID = Globals.LoggedUserAccount.StaffID;
                CurrentAppointment.DoctorStaff = Globals.LoggedUserAccount.Staff;
                CurrentAppointment.ServiceRecID = CurrentPrescription == null ? null : CurrentPrescription.ServiceRecID;
                CurrentAppointment.ApptDate = CurrentPrescription == null ? (DateTime?)null : CurrentPrescription.IssuedDateTime.GetValueOrDefault(Globals.GetCurServerDateTime()).AddDays((int)CurrentPrescription.NDay);
                CurrentAppointment.Patient = Registration_DataStorage.CurrentPatient;
                Action<IAddEditAppointment> onInitDlg = delegate (IAddEditAppointment apptVm)
                {
                    apptVm.Registration_DataStorage = Registration_DataStorage;
                    apptVm.IsCreateApptFromConsultation = true;
                    apptVm.IsCreateApptFromNurseModule = true;
                    apptVm.TreatmentAppointmentSameReg = true;
                    apptVm.RegistrationID = Registration_DataStorage.CurrentPatientRegistration.PtRegistrationID;
                    apptVm.SetCurrentAppointment(CurrentAppointment, CurrentPrescription == null ? (long?)null : CurrentPrescription.IssueID);
                };
                GlobalsNAV.ShowDialog<IAddEditAppointment>(onInitDlg);
            }
            else
            {
                Action<IAddEditAppointment> onInitDlg = (apptVm) =>
                {
                    apptVm.TreatmentAppointmentSameReg = true;
                    apptVm.SetCurrentPatient(CurrentPatient);
                    apptVm.CreateNewAppointment();
                };
                GlobalsNAV.ShowDialog<IAddEditAppointment>(onInitDlg);
            }
        }
        public void CreateNewPCLAppointmentCmd()
        {
            if (IsShowSearchRegistrationButton && !GetConfirmDiagnosisTreatment())
            {
                return;
            }
            Action<IAddEditAppointment> onInitDlg = (apptVm) =>
            {
                apptVm.IsCreateApptFromConsultation = true;
                apptVm.Registration_DataStorage = Registration_DataStorage;
                apptVm.RegistrationID = Registration_DataStorage.CurrentPatientRegistration.PtRegistrationID;
                apptVm.IsPCLBookingView = IsPCLBookingView;
                apptVm.CurrentPtRegDetailAppointmentID = CurrentPtRegDetailAppointmentID;
                apptVm.SetCurrentPatient(CurrentPatient);
                apptVm.CreateNewAppointment((long)AllLookupValues.AppointmentType.HEN_CAN_LAM_SANG_SO, Globals.LoggedUserAccount.StaffID, CurrentPrescription == null || CurrentPtRegDetailAppointmentID > 0 ? (DateTime?)null : CurrentPrescription.IssuedDateTime.GetValueOrDefault(Globals.GetCurServerDateTime()).AddDays((int)CurrentPrescription.NDay), CurrentPrescription == null ? 0 : CurrentPrescription.ServiceRecID);
            };
            GlobalsNAV.ShowDialog<IAddEditAppointment>(onInitDlg);
        }
        public IEnumerator<IResult> CreateNewAppointmentCoroutine()
        {
            if (!isAppointment)
            {
                var dialog = new MessageWarningShowDialogTask("Bệnh Nhân Có Cuộc Hẹn Hiện Tại Đang Chờ Xác Nhận.", "Thêm Mới Cuộc Hẹn");
                yield return dialog;
                if (dialog.IsAccept)
                {
                    CreateNewAppointment();
                }
            }
            else
            {
                CreateNewAppointment();
            }

            yield break;
        }
        //CMN: Lấy thông tin toa thuốc để gợi ý ngày hẹn cho màn hình điều dưỡng
        private void GetPtServiceRecordForKhamBenhAction(PatientRegistration aRegistration)
        {
            this.ShowBusyIndicator();
            PatientServiceRecord SearchCriteria = new PatientServiceRecord();
            SearchCriteria.PtRegistrationID = aRegistration.PtRegistrationID;
            SearchCriteria.V_RegistrationType = aRegistration.V_RegistrationType;
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new CommonUtilsServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginPatientServiceRecordsGetForKhamBenh(SearchCriteria, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var psr = contract.EndPatientServiceRecordsGetForKhamBenh(asyncResult);
                                Registration_DataStorage.PatientServiceRecordCollection = new ObservableCollection<PatientServiceRecord>(psr);
                                GetAllRegistrationDetails_ForGoToKhamBenh(aRegistration.PtRegistrationID);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
                                ClientLoggerHelper.LogError(ex.Message);
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
                    ClientLoggerHelper.LogError(ex.Message);
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }
        //CMN: Lấy thông tin dịch vụ để gợi ý ngày hẹn cho từng dịch vụ lựa chọn được hẹn cho màn hình điều dưỡng
        private void GetAllRegistrationDetails_ForGoToKhamBenh(long PtRegistrationID)
        {
            this.ShowBusyIndicator();
            var CurrentThread = new Thread(() =>
            {
                using (var serviceFactory = new PatientRegistrationServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetAllRegistrationDetails_ForGoToKhamBenh(PtRegistrationID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var items = contract.EndGetAllRegistrationDetails_ForGoToKhamBenh(asyncResult);
                            if (items != null)
                            {
                                Registration_DataStorage.CurrentPatientRegistration.PatientRegistrationDetails = items.ToObservableCollection();
                            }
                            if (Registration_DataStorage.PatientServiceRecordCollection != null &&
                                Registration_DataStorage.PatientServiceRecordCollection.Count > 0 &&
                                Registration_DataStorage.PatientServiceRecordCollection.Last().PrescriptionIssueHistories != null &&
                                Registration_DataStorage.PatientServiceRecordCollection.Last().PrescriptionIssueHistories.Any(x => x.Prescription != null))
                            {
                                var CurrentIssue = Registration_DataStorage.PatientServiceRecordCollection.Where(x => x.PrescriptionIssueHistories != null).OrderByDescending(x => x.PrescriptionIssueHistories.First().IssueID).First().PrescriptionIssueHistories.Last(x => x.Prescription != null);
                                CurrentPrescription = CurrentIssue.Prescription;
                                if (Registration_DataStorage.CurrentPatientRegistration.PatientRegistrationDetails != null &&
                                    Registration_DataStorage.CurrentPatientRegistration.PatientRegistrationDetails.Count > 0)
                                {
                                    Registration_DataStorage.CurrentPatientRegistrationDetail = Registration_DataStorage.CurrentPatientRegistration.PatientRegistrationDetails.FirstOrDefault(x => x.PtRegDetailID == CurrentIssue.PtRegDetailID);
                                }
                            }
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
            CurrentThread.Start();
        }
        public void Handle(CreateNewPatientEvent message)
        {
            if (message != null && this.GetView() != null)
            {
                //var vm = Globals.GetViewModel<IPatientDetails>();
                //vm.CurrentAction = eHCMSResources.Z0037_G1_ThemBN;
                //vm.IsChildWindow = true;
                //vm.CreateNewPatient();
                //Globals.ShowDialog(vm as Conductor<object>);

                Action<IPatientDetails> onInitDlg = (vm) =>
                {
                    vm.CurrentAction = eHCMSResources.Z0037_G1_ThemBN;
                    vm.IsChildWindow = true;
                    vm.CreateNewPatient();
                };
                GlobalsNAV.ShowDialog<IPatientDetails>(onInitDlg);
            }
        }
        public void Handle(AddCompleted<Patient> message)
        {
            if (message != null && this.GetView() != null)
            {
                SetCurrentPatient(message.Item);
            }
        }
        public void Handle(AppointmentAddEditCloseEvent message)
        {
            SearchRegistrationContent.SetDefaultValue();

        }
        public void Handle(SearchAppointmentResultEvent message)
        {
            isAppointment = message.result;
            HasPatient = true;
            NotifyOfPropertyChange(() => isAppointment);
            if (IsShowSearchRegistrationButton && AppointmentListingContent.Appointments.Any(x => x.V_AppointmentType == (long)AllLookupValues.AppointmentType.HEN_TAI_KHAM))
            {
                CurrentPtRegDetailAppointmentID = AppointmentListingContent.Appointments.First(x => x.V_AppointmentType == (long)AllLookupValues.AppointmentType.HEN_TAI_KHAM).AppointmentID;
            }
        }
        public void Handle(ItemSelected<PatientRegistration> message)
        {
            if (message == null || message.Item == null)
            {
                return;
            }
            PatientSummaryInfoContent.CurrentPatientRegistration = message.Item;
            PatientSummaryInfoContent.CurrentPatient = message.Item.Patient;
            Registration_DataStorage = new Registration_DataStorage { CurrentPatientRegistration = message.Item };
            CallSetCurrentPatient(message.Item.Patient, null);
            GetPtServiceRecordForKhamBenhAction(message.Item);
        }
        public void Authorization()
        {
            if (!Globals.isAccountCheck || IsPCLBookingView)
            {
                return;
            }
            mQuanLyHenBenh = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mAppointment_System
                , (int)eAppointment_System.mPatientAppointment
                , (int)oAppointmentEx.mQuanLyHenBenh, (int)ePermission.mView);
        }
        #region checking account
        private bool _mQuanLyHenBenh = true;
        public bool mQuanLyHenBenh
        {
            get
            {
                return _mQuanLyHenBenh;
            }
            set
            {
                if (_mQuanLyHenBenh == value)
                    return;
                _mQuanLyHenBenh = value;
                NotifyOfPropertyChange(() => mQuanLyHenBenh);
            }
        }
        #endregion
    }
}