using System.ComponentModel.Composition;
using aEMR.ViewContracts;
using Caliburn.Micro;
using aEMR.Infrastructure;
using Castle.Windsor;
/*
* 20180330 #001 CMN: Added DeActive Method to dispose  MainContent cause of error on non shared
*/
namespace aEMR.TransactionManager.ViewModels
{
    [Export(typeof(ITransactionModule)), PartCreationPolicy(CreationPolicy.Shared)]
    public class TransactionModuleViewModel : Conductor<object>, ITransactionModule
    {
        [ImportingConstructor]
        public TransactionModuleViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg)
        {
            this.Activated += new System.EventHandler<ActivationEventArgs>(TransactionModuleViewModel_Activated);
        }
        void TransactionModuleViewModel_Activated(object sender, ActivationEventArgs e)
        {
            //Khi khoi tao module thi load menu ben trai luon.
            var shell = Globals.GetViewModel<IHome>();
            var leftMenu = Globals.GetViewModel<ITransactionLeftMenu>();
            shell.LeftMenu = leftMenu;
            (shell as Conductor<object>).ActivateItem(leftMenu);
            // 20183107 TNHX: Add Top Menu
            var topMenu = Globals.GetViewModel<ITransactionTopMenu>();
            shell.TopMenuItems = topMenu;
            shell.OutstandingTaskContent = null;
            shell.IsExpandOST = false;
            (shell as Conductor<object>).ActivateItem(topMenu);
        }
        private object _mainContent;
        public object MainContent
        {
            get { return _mainContent; }
            set
            {
                _mainContent = value;
                NotifyOfPropertyChange(() => MainContent);
            }
        }
        //▼====: #001
        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);
            MainContent = null;
        }
        //▲====: #001
    }
}
