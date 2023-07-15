using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ViewContracts;
using Caliburn.Micro;
using DataEntities;
using Castle.Windsor;
using Castle.Core.Logging;
using aEMR.Infrastructure.CachingUtils;

namespace aEMR.ConsultantEPrescription.ViewModels
{
    [Export(typeof (ISieuAmTimThaiHome_Consultation)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class SieuAmTimThaiHomeViewModel : Conductor<object>, ISieuAmTimThaiHome_Consultation
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public SieuAmTimThaiHomeViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;
                        
            //Globals.EventAggregator.Subscribe(this);
            //Thay bằng
            //Globals.EventAggregator.Subscribe(this);

            var uc3 = Globals.GetViewModel<ISieuAmTT_TM2D_Consultation>();
            UCSieuAmTimTM = uc3;
            ActivateItem(uc3);
            //UCePrescription.Init();

            var uc5 = Globals.GetViewModel<ISieuAmTT_Doppler_Consultation>();
            UCSieuAmTimDoppler = uc5;
            ActivateItem(uc5);
            //UCePrescription.Init();

            var uc7 = Globals.GetViewModel<ISieuAmTT_KetLuan_Consultation>();
            UCSieuAmTimKetLuan = uc7;
            ActivateItem(uc7);
            //UCePrescription.Init();

            var uc8 = Globals.GetViewModel<ISieuAmTT_TimKiem_Consultation>();
            UCSieuAmTimTimKiem = uc8;
            ActivateItem(uc8);

            var uc9 = Globals.GetViewModel<ISieuAmTT_SauSinh_Consultation>();
            UCSieuAmTimSauSinh = uc9;
            ActivateItem(uc9);
        }

        public object UCSieuAmTimTM { get; set; }
        public object UCSieuAmTimDoppler { get; set; }
        public object UCSieuAmTimKetLuan { get; set; }
        public object UCSieuAmTimTimKiem { get; set; }
        public object UCSieuAmTimSauSinh { get; set; }

        public TabItem TabFirst { get; set; }
        public void TabFirst_Loaded(object sender, RoutedEventArgs e)
        {
            TabFirst = sender as TabItem;
            TabFirst.IsSelected = true;
        }
    }
}