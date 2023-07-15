using System.ComponentModel.Composition;
using aEMR.Infrastructure;
using aEMR.ViewContracts;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using Caliburn.Micro;
using DataEntities;

namespace aEMR.Appointment.ViewModels
{
    //phai su dung nonshared vi loi khi menu nay roi click cau hinh he thong 
    //sau do click lai no thi ko hien thi menu nay
    //[Export(typeof(IAppointmentModule)), PartCreationPolicy(CreationPolicy.NonShared)]
    [Export(typeof(IAppointmentModule))]
    public class AppointmentModuleViewModel : Conductor<object>, IAppointmentModule
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public AppointmentModuleViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;
            Authorization();
        }
        protected override void OnActivate()
        {
            MainContent = null;

            base.OnActivate();

            //Khi khoi tao module thi load menu ben trai luon.
            var shell = Globals.GetViewModel<IHome>();
            var leftMenu = Globals.GetViewModel<IAppointmentLeftMenu>();
            var topMenu = Globals.GetViewModel<IAppointmentTopMenu>();
            shell.LeftMenu = leftMenu;
            shell.TopMenuItems = topMenu;
            (shell as Conductor<object>).ActivateItem(leftMenu);
            (shell as Conductor<object>).ActivateItem(topMenu);

            shell.OutstandingTaskContent = null;
            shell.IsExpandOST = false;
        }

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

        public void Authorization()
        {
            if (!Globals.isAccountCheck)
            {
                return;
            }

            mDanhSachHenBenh = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mAppointment_System
                                             , (int)eAppointment_System.mAppointmentList,
                                             (int)oAppointmentEx.mDanhSachHenBenh, (int)ePermission.mView);
           

        }
        #region checking account



        private bool _mDanhSachHenBenh = true;

        public bool mDanhSachHenBenh
        {
            get
            {
                return _mDanhSachHenBenh;
            }
            set
            {
                if (_mDanhSachHenBenh == value)
                    return;
                _mDanhSachHenBenh = value;
                NotifyOfPropertyChange(() => mDanhSachHenBenh);
            }
        }

        
        #endregion
    }
}
