using System.ComponentModel.Composition;
using aEMR.Infrastructure;
using aEMR.Infrastructure.CachingUtils;
using Castle.Core.Logging;
using Castle.Windsor;
using aEMR.Infrastructure.Events;
using aEMR.ViewContracts;
using Caliburn.Micro;
using DataEntities;

namespace aEMR.PCLDepartment.ViewModels
{
    //phai su dung nonshared vi loi khi menu nay roi click cau hinh he thong 
    //sau do click lai no thi ko hien thi menu nay
    //[Export(typeof(ILaboratoryHome)), PartCreationPolicy(CreationPolicy.NonShared)]
    [Export(typeof(ILaboratoryHome)), PartCreationPolicy(CreationPolicy.Shared)]
    public class LaboratoryHomeViewModel : Conductor<object>, ILaboratoryHome
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public LaboratoryHomeViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

        }

        private object _mainContent;

        #region ILaboratoryHome Members

        public object MainContent
        {
            get { return _mainContent; }
            set
            {
                _mainContent = value;
                NotifyOfPropertyChange(() => MainContent);
            }
        }

        #endregion

        protected override void OnActivate()
        {
            MainContent = null;

            base.OnActivate();

            var shell = Globals.GetViewModel<IHome>();

            //var leftMenu = Globals.GetViewModel<ILaboratoryLeftMenu>();
            //shell.LeftMenu = leftMenu;

            var topMenu = Globals.GetViewModel<ILaboratoryTopMenu>();
            shell.TopMenuItems = topMenu;
            ActivateItem(topMenu);
            
            shell.OutstandingTaskContent = null;
            shell.IsExpandOST = false;
        }

        //public void LoadUC()
        //{
        //    var shell = Globals.GetViewModel<IHome>();
        //    var ResultHome = Globals.GetViewModel<ILaboratoryResultHome>();
        //    shell.ActiveContent = ResultHome;
        //    ActivateItem(ResultHome);

        //    var UCPCLDepartmentOutstandingTaskView = Globals.GetViewModel<IPCLDepartmentOutstandingTask>();
        //    shell.OutstandingTaskContent = UCPCLDepartmentOutstandingTaskView;
        //    (shell as Conductor<object>).ActivateItem(UCPCLDepartmentOutstandingTaskView);
        //}

        //private void LoadContentByParaEnum()
        //{
        //    if (Globals.PCLDepartment.ObjV_PCLMainCategory != null)
        //    {
        //        if (Globals.PCLDepartment.ObjV_PCLMainCategory.LookupID ==
        //            (long) AllLookupValues.V_PCLMainCategory.Laboratory)
        //        {
        //            var Module = Globals.GetViewModel<ILaboratoryHome>();
        //            var VM = Globals.GetViewModel<ILaboratoryResultHome>();

        //            Module.MainContent = VM;
        //            (Module as Conductor<object>).ActivateItem(VM);
        //        }
        //    }
        //}

        //public void Handle(LocationSelected message)
        //{
        //    if (message != null)
        //    {
        //        if (this.GetView() != null)
        //        {
        //            LoadUC();
        //        }
        //    }
        //}
 
    }
}