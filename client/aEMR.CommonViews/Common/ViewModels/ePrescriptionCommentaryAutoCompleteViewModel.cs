using System.ComponentModel.Composition;
using System.Windows.Controls;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using aEMR.ViewContracts;
using Caliburn.Micro;

namespace aEMR.Common.ViewModels
{
    [Export(typeof(IePrescriptionCommentaryAutoComplete)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class ePrescriptionCommentaryAutoCompleteViewModel : Conductor<object>, IePrescriptionCommentaryAutoComplete
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public ePrescriptionCommentaryAutoCompleteViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;
        }

        public string DisplayText
        {
            get
            {
                var currentView = this.GetView() as IAutoCompleteView;
                if (currentView != null && currentView.AutoCompleteBox != null)
                {
                    return currentView.AutoCompleteBox.SearchText;
                }
                return string.Empty;
            }
            set
            {
                var currentView = this.GetView() as IAutoCompleteView;
                if (currentView != null && currentView.AutoCompleteBox != null)
                {
                    currentView.AutoCompleteBox.Text = value;
                }
            }
        }

       
        private bool _isLoading;
        public bool IsLoading
        {
            get { return _isLoading; }
            set
            {
                if (!_isLoading)
                {
                    _isLoading = value;
                    NotifyOfPropertyChange(() => IsLoading);
                }
            }
        }

        
        public void ClearItems()
        {
            
        }

        public void PopulatingCmd(object source, PopulatingEventArgs eventArgs)
        {
            eventArgs.Cancel = true;
            var currentView = this.GetView() as IAutoCompleteView;
            if (currentView != null)
            {
                
            }
        }
    }
    //public class ePrescriptionCommentary 
    //{
    //    public string ePC { get; set; }
    //    public int ePCID { get; set; }
    //}

}