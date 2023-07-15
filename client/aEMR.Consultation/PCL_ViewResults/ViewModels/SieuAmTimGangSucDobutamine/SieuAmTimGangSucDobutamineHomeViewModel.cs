using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;
using aEMR.Infrastructure;
using aEMR.ViewContracts;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using Caliburn.Micro;

namespace aEMR.ConsultantEPrescription.ViewModels
{
    [Export(typeof (ISieuAmTimGangSucDobutamineHome_Consultation)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class SieuAmTimGangSucDobutamineHomeViewModel : Conductor<object>, ISieuAmTimGangSucDobutamineHome_Consultation
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public SieuAmTimGangSucDobutamineHomeViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            Globals.EventAggregator.Subscribe(this);
            
            var uc3 = Globals.GetViewModel<ISATGSDobuQuyTrinh_Consultation>();
            SATGSDobuQuyTrinh = uc3;
            ActivateItem(uc3);

            var uc4 = Globals.GetViewModel<ISATGSDobu_Consultation>();
            SATGSDobu = uc4;
            ActivateItem(uc4);

            var uc5 = Globals.GetViewModel<ISATGSDobuBenhSu_Consultation>();
            SATGSDobuBenhSu = uc5;
            ActivateItem(uc5);

            var uc6 = Globals.GetViewModel<ISATGSDobuDienTamDo_Consultation>();
            SATGSDobuDienTamDo = uc6;
            ActivateItem(uc6);

            var uc7 = Globals.GetViewModel<ISATGSDobuKetQua_Consultation>();
            SATGSDobuKetQua = uc7;
            ActivateItem(uc7);

            var uc8 = Globals.GetViewModel<ISATGSDobuHinh_Consultation>();
            SATGSDobuHinh = uc8;
            ActivateItem(uc8);

            var uc9 = Globals.GetViewModel<IPCLDeptImagingResult_Consultation>();
            UCPatientPCLImageResults = uc9;
            ActivateItem(uc9);
        }
        public object UCPatientPCLImageResults { get; set; }


        public object SATGSDobuBenhSu { get; set; }
        public object SATGSDobuDienTamDo { get; set; }
        public object SATGSDobu { get; set; }
        public object SATGSDobuKetQua { get; set; }
        public object SATGSDobuHinh { get; set; }

        public object SATGSDobuQuyTrinh { get; set; }

        public TabItem TabFirst { get; set; }
        public void TabFirst_Loaded(object sender, RoutedEventArgs e)
        {
            TabFirst = sender as TabItem;
            TabFirst.IsSelected = true;
        }
    }
}