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
    [Export(typeof (ISieuAmTimGangSucDipyridamoleHome_Consultation)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class SieuAmTimGangSucDipyridamoleHomeViewModel : Conductor<object>, ISieuAmTimGangSucDipyridamoleHome_Consultation
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public SieuAmTimGangSucDipyridamoleHomeViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            Globals.EventAggregator.Subscribe(this);
            
            var uc3 = Globals.GetViewModel<ISATGSDipyQuyTrinh_Consultation>();
            SATGSDipyQuyTrinh = uc3;
            ActivateItem(uc3);

            var uc4 = Globals.GetViewModel<ISATGSDipy_Consultation>();
            SATGSDipy = uc4;
            ActivateItem(uc4);

            var uc5 = Globals.GetViewModel<ISATGSDipyBenhSu_Consultation>();
            SATGSDipyBenhSu = uc5;
            ActivateItem(uc5);

            var uc6 = Globals.GetViewModel<ISATGSDipyDienTamDo_Consultation>();
            SATGSDipyDienTamDo = uc6;
            ActivateItem(uc6);

            var uc7 = Globals.GetViewModel<ISATGSDipyKetQua_Consultation>();
            SATGSDipyKetQua = uc7;
            ActivateItem(uc7);

            var uc8 = Globals.GetViewModel<ISATGSDipyHinh_Consultation>();
            SATGSDipyHinh = uc8;
            ActivateItem(uc8);

            var uc9 = Globals.GetViewModel<IPCLDeptImagingResult_Consultation>();
            UCPatientPCLImageResults = uc9;
            ActivateItem(uc9);
        }
        public object UCPatientPCLImageResults { get; set; }

        public object SATGSDipyBenhSu { get; set; }
        public object SATGSDipyDienTamDo { get; set; }
        public object SATGSDipy { get; set; }
        public object SATGSDipyKetQua { get; set; }
        public object SATGSDipyHinh { get; set; }

        public object SATGSDipyQuyTrinh { get; set; }

        public TabItem TabFirst { get; set; }
        public void TabFirst_Loaded(object sender, RoutedEventArgs e)
        {
            TabFirst = sender as TabItem;
            TabFirst.IsSelected = true;
        }
    }
}