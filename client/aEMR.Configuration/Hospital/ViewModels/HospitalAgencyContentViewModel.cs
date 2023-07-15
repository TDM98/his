using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using aEMR.Infrastructure;
using aEMR.ViewContracts.Configuration;
using Caliburn.Micro;
using Castle.Windsor;
using Castle.Core.Logging;
using aEMR.Infrastructure.CachingUtils;
using aEMR.ViewContracts;

namespace aEMR.Configuration.Hospitals.ViewModels
{
    [Export(typeof(IHospitalAgencyContent)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class HospitalAgencyContentViewModel : Conductor<object>, IHospitalAgencyContent
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public HospitalAgencyContentViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            UCHospitalAgency = Globals.GetViewModel<IHospitalAgency>();
            UCPclAgency = Globals.GetViewModel<IPclAgency>();

        }

        private bool _isLoading;
        public bool IsLoading
        {
            get { return _isLoading; }
            set
            {
                if (_isLoading != value)
                {
                    _isLoading = value;
                    NotifyOfPropertyChange(() => IsLoading);
                }
            }
        }


        private object _leftContent;
        public object leftContent
        {
            get
            {
                return _leftContent;
            }
            set
            {
                if (_leftContent == value)
                    return;
                _leftContent = value;
                NotifyOfPropertyChange(() => leftContent);
            }
        }


        private object _UCPclAgency;
        public object UCPclAgency
        {
            get
            {
                return _UCPclAgency;
            }
            set
            {
                if (_UCPclAgency == value)
                    return;
                _UCPclAgency = value;
                NotifyOfPropertyChange(() => UCPclAgency);
            }
        }

        private object _UCHospitalAgency;
        public object UCHospitalAgency
        {
            get
            {
                return _UCHospitalAgency;
            }
            set
            {
                if (_UCHospitalAgency == value)
                    return;
                _UCHospitalAgency = value;
                NotifyOfPropertyChange(() => UCHospitalAgency);
            }
        }

    }
}



