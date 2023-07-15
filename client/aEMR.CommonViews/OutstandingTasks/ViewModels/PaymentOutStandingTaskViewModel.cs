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

namespace aEMR.OutstandingTasks.ViewModels
{
    [Export(typeof(IPaymentOutStandingTask)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class PaymentOutStandingTaskViewModel : OutStandingTaskViewModelBase, IPaymentOutStandingTask
        , IHandle<PayForRegistrationCompleted>
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public PaymentOutStandingTaskViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
            : base(AllLookupValues.QueueType.THANH_TOAN_TIEN_KHAM)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            eventAggregator.Subscribe(this);
            OutstandingTitle = eHCMSResources.K3886_G1_DsChoTToanTien;
        }
      
        public override void OnSelectOutstandingItem(object sender, SelectionChangedEventArgs eventArgs)
        {
            if (eventArgs.AddedItems.Count > 0)
            {
                ListBox lst = (ListBox)sender;
                if (lst.SelectedIndex != OldSelectedIndex)
                {
                    PatientQueue p = eventArgs.AddedItems[0] as PatientQueue;
                    if (p != null && p.RegistrationID > 0)
                    {
                        var item = new PatientRegistration();
                        item.PtRegistrationID = p.RegistrationID;

                        var evt = new ItemSelected<PatientRegistration>
                        {
                            Item = item
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

        public void Handle(PayForRegistrationCompleted message)
        {
            if(this.GetView() != null)
            {
                if(message.Payment != null && message.Payment.PatientTransaction != null)
                {
                    RemoveOldItemFromList(message.Payment.PatientTransaction.PtRegistrationID.GetValueOrDefault(-1));   
                }
            }
        }
        private void RemoveOldItemFromList(long regID)
        {
            if(OutstandingItems != null)
            {
                foreach (PatientQueue item in OutstandingItems)
                {
                    if (item.RegistrationID == regID)
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
