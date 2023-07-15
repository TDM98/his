using System.ComponentModel.Composition;
using aEMR.ViewContracts;
using Caliburn.Micro;
using aEMR.Infrastructure.Events;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using DataEntities;
using aEMR.Infrastructure;
using aEMR.Common.BaseModel;

namespace aEMR.Common.ViewModels
{
    [Export(typeof(IPtRegDetailInfo)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class PtRegDetailInfoViewModel : ViewModelBase, IPtRegDetailInfo
        //, IHandle<ShowPatientInfo<Patient, PatientRegistration, PatientRegistrationDetail>>
        , IHandle<ShowPtRegDetailForDiagnosis>
        , IHandle<ShowPtRegDetailForPrescription>
    {
        #region Properties Member
        private PatientRegistrationDetail _PatientRegistrationDetail_Current;
        public PatientRegistrationDetail PatientRegistrationDetail_Current
        {
            get { return _PatientRegistrationDetail_Current; }
            set
            {
                _PatientRegistrationDetail_Current = value;
                NotifyOfPropertyChange(() => PatientRegistrationDetail_Current);
            }
        }

        // TxD 22/09/2018 Added the following to ALLOW child VM of each Tab to get ACCESS to the CENTRAL Data stored in the MainKB (Main KHAM BENH) Screen
        private ICS_DataStorage _CS_DS = null;
        public ICS_DataStorage CS_DS
        {
            get
            {
                return _CS_DS;
            }
            set
            {
                _CS_DS = value;
            }
        }

        private string _TextRegistrationDetail_Current;
        public string TextRegistrationDetail_Current
        {
            get { return _TextRegistrationDetail_Current; }
            set
            {
                _TextRegistrationDetail_Current = value;
                NotifyOfPropertyChange(() => TextRegistrationDetail_Current);
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
        #endregion
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;
        [ImportingConstructor]
        public PtRegDetailInfoViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAgr, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            eventAgr.Subscribe(this);

            //InitData();
            if (Registration_DataStorage != null)
            {
                ShowPtRegDetailInfo(Registration_DataStorage.CurrentPatientRegistration, Registration_DataStorage.CurrentPatientRegistrationDetail);
            }
        }

        public void Handle(ShowPtRegDetailForDiagnosis message)
        {
            ShowPtRegDetailInfo(message.PtRegistration, message.PtRegDetail);
        }

        public void Handle(ShowPtRegDetailForPrescription message)
        {
            ShowPtRegDetailInfo(message.PtRegistration, message.PtRegDetail);
        }

        public void ShowPtRegDetailInfo(PatientRegistration PtReg, PatientRegistrationDetail PtRegDetail)
        {
            if (PtReg == null || PtReg.PtRegistrationID <= 0)
            {
                TextRegistrationDetail_Current = "";
                return;
            }

            switch ((long)PtReg.V_RegistrationType)
            {
                case (long)AllLookupValues.RegistrationType.NGOAI_TRU:
                    {
                        if (PtRegDetail == null || PtRegDetail.RefMedicalServiceItem == null)
                        {
                            break;
                        }

                        TextRegistrationDetail_Current =
                            PtRegDetail.RefMedicalServiceItem.MedServiceName.Trim() + " - (" +
                            PtRegDetail.RefMedicalServiceItem.MedServiceCode.Trim() + ")";

                        break;
                    }
                case (long)AllLookupValues.RegistrationType.NOI_TRU:
                    //case (long)AllLookupValues.RegistrationType.DANGKY_VIP:
                    {
                        TextRegistrationDetail_Current = Globals.GetTextV_RegistrationType((long)PtReg.V_RegistrationType).Trim();
                        break;
                    }
            }
        }
    }
}