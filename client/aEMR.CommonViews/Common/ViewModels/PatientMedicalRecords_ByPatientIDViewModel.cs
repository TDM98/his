using System.ComponentModel.Composition;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.Infrastructure.CachingUtils;
using aEMR.ViewContracts;
using Castle.Windsor;
using Castle.Core.Logging;
using Caliburn.Micro;
using DataEntities;
using aEMR.Common.BaseModel;

namespace aEMR.Common.ViewModels
{
    [Export(typeof(IPatientMedicalRecords_ByPatientID)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class PatientMedicalRecords_ByPatientIDViewModel : ViewModelBase, IPatientMedicalRecords_ByPatientID
        , IHandle<ShowPMRForConsultation>
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;
        [ImportingConstructor]
        public PatientMedicalRecords_ByPatientIDViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAgr, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            eventAgr.Subscribe(this);

            Init();
        }

        //private PatientMedicalRecord _ObjPMRByPatientID;
        //public PatientMedicalRecord ObjPMRByPatientID
        //{
        //    get
        //    {
        //        if(Registration_DataStorage.CurrentPatient != null)
        //            return Globals.PatientMedicalRecordInfo;
        //        return null;
        //    }
        //    set
        //    {
        //        if (_ObjPMRByPatientID != value)
        //        {
        //            _ObjPMRByPatientID = value;
        //            NotifyOfPropertyChange(() => ObjPMRByPatientID);
        //        }
        //    }
        //}

        private PatientMedicalRecord _ObjPMRByPatientID;
        public PatientMedicalRecord ObjPMRByPatientID
        {
            get
            {
                return _ObjPMRByPatientID;
            }
            set
            {
                if (_ObjPMRByPatientID != value)
                {
                    _ObjPMRByPatientID = value;
                    NotifyOfPropertyChange(() => ObjPMRByPatientID);
                }
            }
        }

        public void Init()
        {
            if (Registration_DataStorage != null && Registration_DataStorage.CurrentPatient != null)
            {
                ObjPMRByPatientID = Globals.PatientMedicalRecordInfo;
            }
        }


        public void Handle(ShowPMRForConsultation message)
        {
            Init();
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