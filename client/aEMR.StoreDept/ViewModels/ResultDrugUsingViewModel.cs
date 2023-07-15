using System.Windows;
using System.Windows.Controls;
using System.ComponentModel.Composition;
using aEMR.ViewContracts;
using Caliburn.Micro;
using System.Windows.Input;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;

namespace aEMR.StoreDept.ViewModels
{
    [Export(typeof(IDrugResult)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class ResultDrugUsingViewModel : Conductor<object>, IDrugResult
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public ResultDrugUsingViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;
        }
        public void StackPanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            StackPanel mStackPanel = sender as StackPanel;
            switch (mStackPanel.GetValue(FrameworkElement.NameProperty).ToString())
            {
                case "ShockCmd":
                    Globals.EventAggregator.Publish(new DrugResultEvent { Result = 1 });
                    this.TryClose();
                    break;
                case "EffectCmd":
                    Globals.EventAggregator.Publish(new DrugResultEvent { Result = 2 });
                    this.TryClose();
                    break;
                default:
                    Globals.EventAggregator.Publish(new DrugResultEvent { Result = 0 });
                    this.TryClose();
                    break;
            }
        }
    }
}
