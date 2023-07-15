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
    [Export(typeof(IRegOutStandingTask)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class RegOutStandingTaskViewModel : OutStandingTaskViewModelBase, IRegOutStandingTask
        , IHandle<AddCompleted<PatientRegistration>>
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public RegOutStandingTaskViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching) 
            :base(AllLookupValues.QueueType.CHO_DANG_KY)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            eventAggregator.Subscribe(this);
            OutstandingTitle = eHCMSResources.K1275_G1_BNDaDKBHYT;
            //aucHoldConsultDoctor = Globals.GetViewModel<IAucHoldConsultDoctor>();   
        }

        private IAucHoldConsultDoctor _aucHoldConsultDoctor;
        public IAucHoldConsultDoctor aucHoldConsultDoctor
        {
            get
            {
                return _aucHoldConsultDoctor;
            }
            set
            {
                if (_aucHoldConsultDoctor != value)
                {
                    _aucHoldConsultDoctor = value;
                    NotifyOfPropertyChange(() => aucHoldConsultDoctor);
                }
            }
        }

        public override void OnSelectOutstandingItem(object sender, SelectionChangedEventArgs eventArgs)
        {
            if (eventArgs.AddedItems.Count > 0)
            {
                ListBox lst = (ListBox)sender;
                PatientQueue p = eventArgs.AddedItems[0] as PatientQueue;
                if (p != null && p.RegistrationID > 0
                    && p.RegistrationID != OldPtRegistrationID)
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
                        OldPtRegistrationID = p.RegistrationID;
                        OldSelectedIndex = lst.SelectedIndex;
                        SelectedItem = (PatientQueue)lst.SelectedItem;
                    }
                }                
            }
        }

        public void Handle(AddCompleted<PatientRegistration> message)
        {
            if (message.Item != null)
            {
                RemoveOldItemFromList(message.Item.PtRegistrationID);   
            }
        }

        private void RemoveOldItemFromList(long regID)
        {
            if (OutstandingItems != null)
            {
                foreach (PatientQueue item in OutstandingItems)
                {
                    if (item.RegistrationID == regID)
                    {
                        OutstandingItems.Remove(item);
                        if (SelectedItem == item)
                        {
                            OldSelectedIndex = -1;
                        }
                        ItemsCount = OutstandingItems.Count;
                        break;
                    }
                }
            }
        }
    }
}
