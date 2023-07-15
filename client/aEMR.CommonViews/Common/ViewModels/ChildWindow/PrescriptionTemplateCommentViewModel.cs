using System;
using System.ComponentModel.Composition;
using System.ServiceModel;
using aEMR.ViewContracts;
using Caliburn.Micro;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using DataEntities;

namespace aEMR.Common.ViewModels
{
    [Export(typeof(IPrescriptionTemplateComment)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class PrescriptionTemplateCommentViewModel : Conductor<object>, IPrescriptionTemplateComment
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public PrescriptionTemplateCommentViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;
        }

        private string _Comment;
        public string Comment
        {
            get { return _Comment; }
            set
            {
                _Comment = value;
                NotifyOfPropertyChange(() => Comment);
            }
        }

        
        public void OkCmd()
        {
            TryClose();
        }
    }
}
