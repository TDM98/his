using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ServiceModel;
using System.Threading;
using aEMR.DataContracts;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using aEMR.Common.Collections;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using Caliburn.Micro;
using DataEntities;
using System.Windows;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Linq;
using System.Windows.Input;
using aEMR.Common.BaseModel;

namespace aEMR.Common.ViewModels
{
    [Export(typeof(IPatientsInTreatmentListViewModel)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class PatientsInTreatmentListViewModel : ViewModelBase, IPatientsInTreatmentListViewModel
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;
        [ImportingConstructor]
        public PatientsInTreatmentListViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAgr, ISalePosCaching salePosCaching)
        {
            Globals.EventAggregator.Subscribe(this);
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;
            CreateSubVM();
        }
        public void CreateSubVM()
        {
            UCOutPatientRegistrationOST = Globals.GetViewModel<IConsultationOutstandingTask>();

            UCInPatientRegistrationOST = Globals.GetViewModel<IInPatientOutstandingTask>();
        }

        private bool _vUCOutPatientRegistrationOST = true;
        public bool vUCOutPatientRegistrationOST
        {
            get
            {
                return _vUCOutPatientRegistrationOST;
            }
            set
            {
                if (_vUCOutPatientRegistrationOST == value)
                    return;
                _vUCOutPatientRegistrationOST = value;
                NotifyOfPropertyChange(() => vUCOutPatientRegistrationOST);
            }
        }

        private bool _vUCInPatientRegistrationOST = false;
        public bool vUCInPatientRegistrationOST
        {
            get
            {
                return _vUCInPatientRegistrationOST;
            }
            set
            {
                if (_vUCInPatientRegistrationOST == value)
                    return;
                _vUCInPatientRegistrationOST = value;
                NotifyOfPropertyChange(() => vUCInPatientRegistrationOST);
            }
        }
        public void OnActivateSubVM()
        {
            ActivateItem(UCOutPatientRegistrationOST);
            ActivateItem(UCInPatientRegistrationOST);
        }
        public void OnDeactivateSubVM(bool close)
        {
            DeactivateItem(UCOutPatientRegistrationOST, close);
            DeactivateItem(UCInPatientRegistrationOST, close);
        }
        //protected override void OnActivate()
        //{
        //    OnActivateSubVM();
        //    base.OnActivate();
        //}

        protected override void OnDeactivate(bool close)
        {
            OnDeactivateSubVM(close);
            base.OnDeactivate(close);
            Globals.EventAggregator.Unsubscribe(this);
        }

        public IConsultationOutstandingTask UCOutPatientRegistrationOST { get; set; }

        public IInPatientOutstandingTask UCInPatientRegistrationOST { get; set; }
    }
}