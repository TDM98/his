using System;
using System.ComponentModel.Composition;
using System.Windows;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ViewContracts;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using Caliburn.Micro;
using DataEntities;
using System.Collections.Generic;
using aEMR.CommonTasks;
using System.Linq;
using eHCMSLanguage;

namespace aEMR.Common.ViewModels
{
    [Export(typeof(IPatientAppointments_PCL_InConsultation)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class PatientAppointments_PCL_InConsultationViewModel : Conductor<object>, IPatientAppointments_PCL_InConsultation
        , IHandle<ResultFound<Patient>>
        , IHandle<ResultNotFound<Patient>>, IHandle<ItemSelected<Patient>>
        , IHandle<AddCompleted<Patient>>
        , IHandle<ShowPatientInfo_KHAMBENH_HENCLS_HENCLS<Patient, PatientRegistration, PatientRegistrationDetail>>
        , IHandle<SearchAppointmentResultEvent>
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;
        [ImportingConstructor]
        public PatientAppointments_PCL_InConsultationViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAgr, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            eventAgr.Subscribe(this);

            authorization();
            var appointmentListingVm = Globals.GetViewModel<IAppointmentListing>();
            AppointmentListingContent = appointmentListingVm;
            ActivateItem(appointmentListingVm);
            //InitPatientInfo();
        }

        public void InitPatientInfo(Patient aPatient)
        {
            if (aPatient != null)/*Làm CLS chỉ cần kiểm tra BN !=null*/
            {
                SetCurrentPatient(aPatient);
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

        private bool _isCreateApptFromConsultation;
        public bool IsCreateApptFromConsultation
        {
            get
            {
                return _isCreateApptFromConsultation;
            }
            set
            {
                if (_isCreateApptFromConsultation != value)
                {
                    _isCreateApptFromConsultation = value;
                    if (AppointmentListingContent != null)
                    {
                        AppointmentListingContent.IsCreateApptFromConsultation = _isCreateApptFromConsultation;
                    }
                    NotifyOfPropertyChange(() => IsCreateApptFromConsultation);
                }
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

                    _searchCriteria = new AppointmentSearchCriteria();
                    _searchCriteria.PatientID = _currentPatient.PatientID;

                    if (_currentPatient != null)
                    {
                        AppointmentListingContent.SearchCriteria = _searchCriteria;
                        AppointmentListingContent.SearchCriteria.OrderBy = "RecDateCreated";
                        AppointmentListingContent.StartSearching();
                    }

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
        public void SetCurrentPatient(Patient patient)
        {
            CurrentPatient = patient;
        }

        public void Handle(ResultFound<Patient> message)
        {
            if (message != null && this.GetView() != null)
            {
                SetCurrentPatient(message.Result);
            }
        }
        public void Handle(ResultNotFound<Patient> message)
        {
            //if (message != null && this.GetView() != null)
            //{
            //    //Thông báo không tìm thấy bệnh nhân.
            //    MessageBox.Show(eHCMSResources.Z0478_G1_KhongTimThayBenhNhan, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
            //}
            AppointmentListingContent.ClearItemSource();
        }
        public void Handle(SearchAppointmentResultEvent message)
        {
            isAppointment = message.result;
            //HasPatient = true;
            NotifyOfPropertyChange(() => isAppointment);
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
            if (IsCreateApptFromConsultation)
            {
                //if (Globals.PatientAllDetails.PtRegistrationInfo == null || Globals.PatientAllDetails.PtRegistrationInfo.PtRegistrationID <= 0)
                //{
                //    MessageBox.Show(eHCMSResources.A0634_G1_Msg_InfoKhCoDKDeHen, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                //    return;
                //}
                //if (Globals.IsLockRegistration(Globals.PatientAllDetails.PtRegistrationInfo.RegLockFlag, "tạo cuộc hẹn"))
                //{
                //    return;
                //}
                //if (Globals.PatientAllDetails.PtRegistrationInfo.V_RegistrationType == AllLookupValues.RegistrationType.NGOAI_TRU)
                //{
                //    if (Registration_DataStorage.PatientServiceRecordCollection == null || Registration_DataStorage.PatientServiceRecordCollection.Count < 1)
                //    {
                //        MessageBox.Show(eHCMSResources.A0405_G1_Msg_InfoChuaCoCD, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                //        return;
                //    }
                //}
                //else
                {
                    if (Registration_DataStorage.PatientServiceRecordCollection == null || Registration_DataStorage.PatientServiceRecordCollection.Count <= 0 || Registration_DataStorage.PatientServiceRecordCollection[0].DiagnosisTreatments.Count <= 0
                        || !Registration_DataStorage.PatientServiceRecordCollection[0].DiagnosisTreatments.Any(x => x.V_DiagnosisType == (long)AllLookupValues.V_DiagnosisType.DIAGNOSIS_OUTHOS))
                    {
                        MessageBox.Show(string.Format("{0}!", eHCMSResources.Z0381_G1_ChuaCoCDoanXV), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                        return;
                    }
                }
            }

            Coroutine.BeginExecute(CreateNewAppointmentCoroutine());
        }

        public IEnumerator<IResult> CreateNewAppointmentCoroutine()
        {
            if (!isAppointment)
            {
                var dialog = new MessageWarningShowDialogTask(string.Format("{0}.", eHCMSResources.Z1260_G1_BNCoCuocHenDangChoXNhan), eHCMSResources.G0296_G1_ThemMoiCuocHen);
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
        public void CreateNewAppointment()
        {
            Action<IAddEditAppointment> onInitDlg = (Alloc) =>
            {
                //20190929 TTM: Không được deepCopy interface.
                //Alloc.Registration_DataStorage = Registration_DataStorage.DeepCopy();
                Alloc.Registration_DataStorage = Registration_DataStorage;
                Alloc.SetCurrentPatient(CurrentPatient);
                Alloc.CreateNewAppointment();
                Alloc.IsCreateApptFromConsultation = IsCreateApptFromConsultation;
            };
            GlobalsNAV.ShowDialog<IAddEditAppointment>(onInitDlg);
        }
        //public void Handle(CreateNewPatientEvent message)
        //{
        //    if (message != null && this.GetView() != null)
        //    {
        //        var vm = Globals.GetViewModel<IPatientDetails>();
        //        vm.CurrentAction = eHCMSResources.Z0037_G1_ThemBN;
        //        vm.CreateNewPatient();
        //        Globals.ShowDialog(vm as Conductor<object>);
        //    }
        //}

        public void Handle(AddCompleted<Patient> message)
        {
            if (message != null && this.GetView() != null)
            {
                SetCurrentPatient(message.Item);
            }
        }

        public void authorization()
        {
            if (!Globals.isAccountCheck)
            {
                return;
            }
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

        public void Handle(ShowPatientInfo_KHAMBENH_HENCLS_HENCLS<Patient, PatientRegistration, PatientRegistrationDetail> message)
        {
            if (message == null)
            {
                return;
            }
            InitPatientInfo(message.Pt);
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
                AppointmentListingContent.Registration_DataStorage = Registration_DataStorage;
            }
        }
    }
}
