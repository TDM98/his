using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using aEMR.ViewContracts;
using Caliburn.Micro;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using DataEntities;

namespace aEMR.Common.ViewModels
{
    [Export(typeof(IAppointmentErrorListing)), PartCreationPolicy(CreationPolicy.NonShared)] 
    public class AppointmentErrorListingViewModel : Conductor<object>, IAppointmentErrorListing
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;
        [ImportingConstructor]
        public AppointmentErrorListingViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAgr, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            eventAgr.Subscribe(this);
        }
        public void OkCmd()
        {
            TryClose();
        }

        private ObservableCollection<PatientApptServiceDetails> _requestSeqNoFailedList;
        public ObservableCollection<PatientApptServiceDetails> RequestSeqNoFailedList
        {
            get
            {
                return _requestSeqNoFailedList;
            }
            set
            {
                if (_requestSeqNoFailedList != value)
                {
                    _requestSeqNoFailedList = value;
                    NotifyOfPropertyChange(() => RequestSeqNoFailedList);
                }
            }
        }
        private ObservableCollection<PatientApptServiceDetails> _insertFailedList;
        public ObservableCollection<PatientApptServiceDetails> InsertFailedList
        {
            get
            {
                return _insertFailedList;
            }
            set
            {
                if (_insertFailedList != value)
                {
                    _insertFailedList = value;
                    NotifyOfPropertyChange(() => InsertFailedList);
                }
            }
        }
        private ObservableCollection<PatientApptServiceDetails> _deleteFailedList;
        public ObservableCollection<PatientApptServiceDetails> DeleteFailedList
        {
            get
            {
                return _deleteFailedList;
            }
            set
            {
                if (_deleteFailedList != value)
                {
                    _deleteFailedList = value;
                    NotifyOfPropertyChange(() => DeleteFailedList);
                }
            }
        }
    }
}
