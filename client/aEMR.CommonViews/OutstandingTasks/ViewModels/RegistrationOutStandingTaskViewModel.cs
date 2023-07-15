using eHCMSLanguage;
using System.ComponentModel.Composition;
using aEMR.Infrastructure;
using aEMR.ViewContracts;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using Caliburn.Micro;

namespace aEMR.OutstandingTasks.ViewModels
{
    [Export(typeof(IRegistrationOutStandingTask)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class RegistrationOutStandingTaskViewModel : Conductor<object>, IRegistrationOutStandingTask
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        public enum ViewChoice
        {
            ClinicRoom=1,
            Appointment=2,
        }

        [ImportingConstructor]
        public RegistrationOutStandingTaskViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            //AppointmentOutstandingTaskContent = Globals.GetViewModel<IRegAppointmentOutStandingTask>();
            vChoice = ViewChoice.ClinicRoom;
            RegOutstandingTaskContent = Globals.GetViewModel<IRegOutStandingTask>();
            AppointmentOutstandingTaskContent = Globals.GetViewModel<IClinicRoomTask>();
        }

        //private IRegAppointmentOutStandingTask _appointmentOutstandingTaskContent;
        //public IRegAppointmentOutStandingTask AppointmentOutstandingTaskContent
        //{
        //    get
        //    {
        //        return _appointmentOutstandingTaskContent;
        //    }
        //    set
        //    {
        //        _appointmentOutstandingTaskContent = value;
        //        NotifyOfPropertyChange(() => AppointmentOutstandingTaskContent);
        //    }
        //}
        public void ChoiceView()
        {
            if (vChoice==ViewChoice.ClinicRoom)
            {
                vChoice = ViewChoice.Appointment;
            }
            else
            {
                vChoice = ViewChoice.ClinicRoom;
            }
        }

        private ViewChoice _vChoice;
        public ViewChoice vChoice
        {
            get
            {
                return _vChoice;
            }
            set
            {
                if (_vChoice == value)
                    return;
                _vChoice = value;
                NotifyOfPropertyChange(() => vChoice);
                switch (vChoice)
                {
                    //case ViewChoice.ClinicRoom:
                    //    textChoice = eHCMSResources.G1160_G1_TimCuocHen;
                    //    AppointmentOutstandingTaskContent = Globals.GetViewModel<IClinicRoomTask>();
                    //    break;
                    case ViewChoice.Appointment:
                        textChoice = eHCMSResources.G2461_G1_XemPK;
                        AppointmentOutstandingTaskContent = Globals.GetViewModel<IRegAppointmentOutStandingTask>();
                        break;
                }
            }
        }

        private string _textChoice;
        public string textChoice
        {
            get
            {
                return _textChoice;
            }
            set
            {
                if (_textChoice == value)
                    return;
                _textChoice = value;
                NotifyOfPropertyChange(()=>textChoice);
            }
        }

        private object _appointmentOutstandingTaskContent;
        public object AppointmentOutstandingTaskContent
        {
            get
            {
                return _appointmentOutstandingTaskContent;
            }
            set
            {
                _appointmentOutstandingTaskContent = value;
                NotifyOfPropertyChange(() => AppointmentOutstandingTaskContent);
            }
        }

        private IRegOutStandingTask _regOutstandingTaskContent;
        public IRegOutStandingTask RegOutstandingTaskContent
        {
            get
            {
                return _regOutstandingTaskContent;
            }
            set
            {
                _regOutstandingTaskContent = value;
                NotifyOfPropertyChange(()=> RegOutstandingTaskContent);
            }
        }
    }
}
