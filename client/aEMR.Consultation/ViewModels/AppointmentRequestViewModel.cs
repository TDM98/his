//using System.ComponentModel.Composition;
//using aEMR.Infrastructure;
//using aEMR.ViewContracts;
//using Caliburn.Micro;
//using DataEntities;
//using System;
//using Castle.Windsor;

//namespace aEMR.ConsultantEPrescription.ViewModels
//{
//    [Export(typeof(IAppointmentRequest)),PartCreationPolicy(CreationPolicy.NonShared)]
//    public class AppointmentRequestViewModel : Conductor<object>, IAppointmentRequest
//    {
//        [ImportingConstructor]
//        public AppointmentRequestViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg)
//        {
//            var loggedUserVm = Globals.GetViewModel<ILoginInfo>();
//            LoggedUserInfoContent = loggedUserVm;
//            ActivateItem(loggedUserVm);

//            var patientInfoVm = Globals.GetViewModel<IPatientInfo>();
//            PatientInfoContent = patientInfoVm;
//            ActivateItem(patientInfoVm);
//            authorization();
//        }

//        private PatientRegistration _currentRegistration;
//        public PatientRegistration CurrentRegistration
//        {
//            get { return _currentRegistration; }
//            set
//            {
//                _currentRegistration = value;
//                NotifyOfPropertyChange(()=>CurrentRegistration);
//            }
//        }

//        private ILoginInfo _loggedUserInfoContent;
//        public ILoginInfo LoggedUserInfoContent
//        {
//            get { return _loggedUserInfoContent; }
//            set
//            {
//                _loggedUserInfoContent = value;
//                NotifyOfPropertyChange(()=>LoggedUserInfoContent);
//            }
//        }

//        private IPatientInfo _patientInfoContent;
//        public IPatientInfo PatientInfoContent
//        {
//            get { return _patientInfoContent; }
//            set
//            {
//                _patientInfoContent = value;
//                NotifyOfPropertyChange(()=>PatientInfoContent);
//            }
//        }

//        public void CreateNewAppointmentRequestCmd()
//        {
//            if (_currentRegistration != null)
//            {
//                //Globals.EventAggregator.Publish(new ItemSelected<PatientAppointment>() { Item = SelectedAppointment });
//                Action<IAddEditAppointment> onInitDlg = delegate (IAddEditAppointment apptVm)
//                {
//                    apptVm.SetCurrentPatient(_currentRegistration.Patient);
//                    apptVm.CreateNewAppointment();
//                };
//                GlobalsNAV.ShowDialog<IAddEditAppointment>(onInitDlg);
//            }
//        }
//        public bool CanCreateNewAppointmentRequestCmd
//        {
//            get
//            {
//                if (!Globals.isAccountCheck)
//                {
//                    return CurrentRegistration!=null && CurrentRegistration.PtRegistrationID>0;
//                }
//                else
//                {
//                    return (Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConsultation
//                                               , (int)eConsultation.mPtPMRConsultationNew,
//                                               (int)oConsultationEx.mChanDoan_tabLanKhamTruoc_HieuChinh, (int)ePermission.mEdit))
//                                && CurrentRegistration != null && CurrentRegistration.PtRegistrationID > 0;
                    
//                }
//            }
//        }

//        public void authorization()
//        {
//            if (!Globals.isAccountCheck)
//            {
//                return;
//            }

//            mHenBenh_Xem = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConsultation
//                                               , (int)eConsultation.mPteAppointmentTab,
//                                               (int)oConsultationEx.mHenBenh_Xem, (int)ePermission.mView);
            

//        }
//        #region checking account

//        private bool _mHenBenh_Xem = true;

//        public bool mHenBenh_Xem
//        {
//            get
//            {
//                return _mHenBenh_Xem;
//            }
//            set
//            {
//                if (_mHenBenh_Xem == value)
//                    return;
//                _mHenBenh_Xem = value;
//                NotifyOfPropertyChange(() => mHenBenh_Xem);
//            }
//        }



//        #endregion
        
//    }
//}
