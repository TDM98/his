using aEMR.Infrastructure;
using Caliburn.Micro;
using aEMR.ViewContracts;
using System.ComponentModel.Composition;
using Castle.Windsor;
/*
 * 20191104 #001 TTM:   BM 0018528: [Left Menu] Bổ sung cây y lệnh chẩn đoán vào màn hình quản lý bệnh nhân nội trú
 */
namespace aEMR.Registration.ViewModels
{
    [Export(typeof(IRegistrationModule)), PartCreationPolicy(CreationPolicy.Shared)]
    public class RegistrationModuleViewModel : Conductor<object>, IRegistrationModule
    {
        [ImportingConstructor]
        public RegistrationModuleViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg)
        {

        }
        protected override void OnActivate()
        {
            base.OnActivate();
            //Khi khoi tao module thi load menu ben trai luon.
            var shell = Globals.GetViewModel<IHome>();
            var leftMenu = Globals.GetViewModel<IRegistrationLeftMenu>();
            var topMenu = Globals.GetViewModel<IRegistrationTopMenu>();
            
            shell.LeftMenu = leftMenu;
            shell.TopMenuItems = topMenu;

            //((Conductor<object>)shell).ActivateItem(leftMenu);

            shell.OutstandingTaskContent = null;
            ((Conductor<object>)shell).ActivateItem(leftMenu);
            shell.IsExpandOST = false;
            //ActivateItem(leftMenu);
        }
        //▼===== #001   Khởi tạo hàm OnDeactivate để deactivateItem maincontent đi. Lý do khi set maincontent null thì mới chỉ clear viewmodel ở RegistrationModule
        //              Riêng thằng ActivateItem ở TopMenu vẫn giữ lại view nên dẫn đến việc ondeactivate ở trong viewmodel sót => Vừa vào module lại bị
        //              Base.OnActivate activate lên làm hiển thị LeftMenu.
        protected override void OnDeactivate(bool close)
        {
            DeactivateItem(MainContent, close);
            MainContent = null;
            base.OnDeactivate(close);
        }
        //▲===== 
        private object _mainContent;

        public object MainContent
        {
            get { return _mainContent; }
            set
            {
                _mainContent = value;
                NotifyOfPropertyChange(()=>MainContent);
            }
        }
    }
}
