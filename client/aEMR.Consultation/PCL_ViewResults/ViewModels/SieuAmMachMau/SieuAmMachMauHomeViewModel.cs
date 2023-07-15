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
    [Export(typeof (ISieuAmMachMauHome_Consultation)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class SieuAmMachMauHomeViewModel : Conductor<object>, ISieuAmMachMauHome_Consultation
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public SieuAmMachMauHomeViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            Globals.EventAggregator.Subscribe(this);
            
            var uc3 = Globals.GetViewModel<ISAMMDongMachCanh_Consultation>();
            UCSAMMDongMachCanh = uc3;
            ActivateItem(uc3);

            var uc4 = Globals.GetViewModel<ISAMMDongMachChu_Consultation>();
            UCSAMMDongMachChu = uc4;
            ActivateItem(uc4);

            var uc5 = Globals.GetViewModel<ISAMMChiDuoi_Consultation>();
            UCSAMMChiDuoi = uc5;
            ActivateItem(uc5);

            var uc6 = Globals.GetViewModel<ISAMMKhac_Consultation>();
            UCKhac = uc6;
            ActivateItem(uc6);
        }

   
        public object UCSieuAmTimTM { get; set; }
        public object UCSAMMDongMachCanh { get; set; }

        public object UCSAMMDongMachChu { get; set; }
        public object UCSAMMChiDuoi { get; set; }
        public object UCKhac { get; set; }


        public TabItem TabFirst { get; set; }
        public void TabFirst_Loaded(object sender, RoutedEventArgs e)
        {
            TabFirst = sender as TabItem;
            TabFirst.IsSelected = true;
        }
    }
}