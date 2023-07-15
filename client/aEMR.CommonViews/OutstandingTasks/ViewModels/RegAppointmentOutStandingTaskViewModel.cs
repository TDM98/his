using System.ComponentModel.Composition;
using System.Windows.Controls;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ViewContracts;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using Caliburn.Micro;
using DataEntities;
using eHCMSLanguage;
using aEMR.OutstandingTasks.ViewModels;

namespace aEMR.OutstandingTasks.ViewModels
{
    [Export(typeof(IRegAppointmentOutStandingTask)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class RegAppointmentOutStandingTaskViewModel : OutStandingTaskViewModelBase, IRegAppointmentOutStandingTask
         , IHandle<AddCompleted<PatientRegistration>>
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public RegAppointmentOutStandingTaskViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
            : base(AllLookupValues.QueueType.DANG_KY_HEN_BENH)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            eventAggregator.Subscribe(this);
            OutstandingTitle = eHCMSResources.K2989_G1_DSHen;
            //OutstandingTitle = eHCMSResources.K3054_G1_DSPg;
        }
      
        public override void OnSelectOutstandingItem(object sender, SelectionChangedEventArgs eventArgs)
        {
            if (eventArgs.AddedItems.Count > 0)
            {
                ListBox lst = (ListBox)sender;
                if (lst.SelectedIndex != OldSelectedIndex)
                {
                    PatientQueue p = eventArgs.AddedItems[0] as PatientQueue;
                    if (p != null && p.PatientAppointmentID.GetValueOrDefault(-1) > 0)
                    {
                        PatientAppointment appointment = new PatientAppointment();
                        appointment.AppointmentID = (long)p.PatientAppointmentID;
                        appointment.StaffID = Globals.LoggedUserAccount.StaffID.GetValueOrDefault(0);
                        appointment.Patient = new Patient();
                        appointment.Patient.PatientID = p.PatientID.GetValueOrDefault(0);
                        appointment.PatientID = appointment.Patient.PatientID;
                        appointment.Patient.FullName = p.FullName;

                        var evt = new ItemSelecting<object, PatientAppointment>
                        {
                            Sender = sender,
                            Item = appointment
                        };
                        Globals.EventAggregator.Publish(evt);


                        if (evt.Cancel)
                        {
                            lst.SelectedIndex = OldSelectedIndex;
                        }
                        else
                        {
                            OldSelectedIndex = lst.SelectedIndex;
                            SelectedItem = (PatientQueue)lst.SelectedItem;
                        }
                    }
                }
            }
        }

        public void Handle(AddCompleted<PatientRegistration> message)
        {
            if(this.GetView() != null)
            {
                if(message.Item.AppointmentID.HasValue && message.Item.AppointmentID.Value > 0)
                {
                    RemoveOldItemFromList(message.Item.AppointmentID.Value);   
                }
            }
        }
        private void RemoveOldItemFromList(long appointmentID)
        {
            if(OutstandingItems != null)
            {
                foreach (PatientQueue item in OutstandingItems)
                {
                    if (item.PatientAppointmentID == appointmentID)
                    {
                        OutstandingItems.Remove(item);
                        if(SelectedItem == item)
                        {
                            OldSelectedIndex = -1;
                        }
                        break;
                    }
                }
            }
        }
    }
}
