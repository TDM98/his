using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.ComponentModel.DataAnnotations;
using aEMR.ViewContracts;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using Caliburn.Micro;

namespace aEMR.Common.ViewModels
{
    [Export(typeof(IValidationError))]
    public class ValidationErrorViewModel : Conductor<object>, IValidationError
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public ValidationErrorViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;
        }
        public string ErrorTitle
        {
            get; set;
        }

        private ObservableCollection<ValidationResult> _validationErrors;

        public ObservableCollection<ValidationResult> ValidationErrors
        {
            get { return _validationErrors; }
            set
            {
                _validationErrors = value;
                NotifyOfPropertyChange(()=>ValidationErrors);
            }
        }

        public void SetErrors(ObservableCollection<ValidationResult> validationErrorCollection)
        {
            ValidationErrors = validationErrorCollection;
        }
        public void OkCmd()
        {
            TryClose();
        }
    }
}
