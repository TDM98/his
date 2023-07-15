using System;
using System.ComponentModel.Composition;
using aEMR.Infrastructure;
using aEMR.ViewContracts;
using Caliburn.Micro;
using DataEntities;
using aEMR.ViewContracts.Configuration;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using System.Collections.ObjectModel;

namespace aEMR.Common.ViewModels
{
    [Export(typeof(ILoginInfo)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class LoginInfoViewModel : Conductor<object>, ILoginInfo
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public LoginInfoViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;
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

        public Staff CurrentStaff
        {
            get { return Globals.LoggedUserAccount.Staff; }
            set
            {
            }
        }
        private bool _isPreNoteTemp=false;
        public bool isPreNoteTemp 
        {
            get { return _isPreNoteTemp;}
            set 
            {
                if (_isPreNoteTemp != value)
                    _isPreNoteTemp = value;
                NotifyOfPropertyChange(() => isPreNoteTemp);
            }
        }

        public void PreNoteTemp()
        {
            //var proAlloc = Globals.GetViewModel<IfrmPrescriptionNoteTempType>();
            //proAlloc.isPopup = true;
            //proAlloc.ExclusionValues = new ObservableCollection<string> { AllLookupValues.V_PrescriptionNoteTempType.AppointmentPCLTemplate.ToString() };
            ////proAlloc.ExclusionValues.Add(AllLookupValues.V_PrescriptionNoteTempType.AppointmentPCLTemplate.ToString());
            //var instance = proAlloc as Conductor<object>;
            //Globals.ShowDialog(instance, (o) => { });
            Action<IfrmPrescriptionNoteTempType> onInitDlg = (Alloc) =>
            {
                Alloc.isPopup = true;
                Alloc.ExclusionValues = new ObservableCollection<string> { AllLookupValues.V_PrescriptionNoteTempType.AppointmentPCLTemplate.ToString() };
            };
            GlobalsNAV.ShowDialog<IfrmPrescriptionNoteTempType>(onInitDlg);
        }
    }
}
