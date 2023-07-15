using System;
using System.ComponentModel.Composition;
using aEMR.Infrastructure;
using aEMR.Infrastructure.CachingUtils;
using aEMR.ViewContracts;
using Castle.Windsor;
using Castle.Core.Logging;
using Caliburn.Micro;
using aEMR.Common.BaseModel;

namespace aEMR.Common.ViewModels
{
    [Export(typeof(IAllergiesWarning_ByPatientID)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class AllergiesWarning_ByPatientIDViewModel : ViewModelBase, IAllergiesWarning_ByPatientID
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;
        [ImportingConstructor]
        public AllergiesWarning_ByPatientIDViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAgr, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;
            eventAgr.Subscribe(this);
        }
        public String Allergies
        {
            get
            {
                if (Registration_DataStorage.CurrentPatient != null)
                    return Globals.Allergies;
                return null;
            }
        }

        public String Warning
        {
            get
            {
                if (Registration_DataStorage.CurrentPatient != null)
                    return Globals.Warning;
                return null;
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
    }
}