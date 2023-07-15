using System.ComponentModel.Composition;
using aEMR.ViewContracts;
using DataEntities;
using aEMR.Common.BaseModel;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using aEMR.ViewContracts.Configuration;

namespace aEMR.Common.ViewModels
{
    [Export(typeof(IDrugInformation)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class DrugInformationViewModel : ViewModelBase, IDrugInformation
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;
        [ImportingConstructor]
        public DrugInformationViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching) 
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;
            SelectedDrugInformation = new PrescriptionDetail();
        }

        private PrescriptionDetail _SelectedDrugInformation;
        public PrescriptionDetail SelectedDrugInformation
        {
            get
            {
                return _SelectedDrugInformation;
            }
            set
            {
                if (_SelectedDrugInformation != value)
                {
                    _SelectedDrugInformation = value;
                    if (_SelectedDrugInformation != null 
                        && !string.IsNullOrEmpty(_SelectedDrugInformation.BrandName) 
                        && !string.IsNullOrEmpty(_SelectedDrugInformation.RefGenericDrugDetail.GenericName))
                    {
                        ViewTitle = string.Format("{0} ({1})", _SelectedDrugInformation.BrandName, _SelectedDrugInformation.RefGenericDrugDetail.GenericName);
                    }
                    NotifyOfPropertyChange(() => SelectedDrugInformation);
                }
            }
        }

        private string _ViewTitle;
        public string ViewTitle
        {
            get
            {
                return _ViewTitle;
            }
            set
            {
                if (_ViewTitle != value)
                {
                    _ViewTitle = value;
                    NotifyOfPropertyChange(() => ViewTitle);
                }
            }
        }

        public void btnClose()
        {
            TryClose();
        }
    }
}
