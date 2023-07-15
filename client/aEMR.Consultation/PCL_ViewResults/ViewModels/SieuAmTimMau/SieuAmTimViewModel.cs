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
    [Export(typeof (ISieuAmTim_Consultation)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class SieuAmTimViewModel : Conductor<object>, ISieuAmTim_Consultation
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public SieuAmTimViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            //Globals.EventAggregator.Subscribe(this);
            //thay bằng
            //Globals.EventAggregator.Subscribe(this);

            var uc3 = Globals.GetViewModel<ISieuAmTim_TM_Consultation>();
            UCSieuAmTimTM = uc3;
            ActivateItem(uc3);
            //UCePrescription.Init();

            var uc4 = Globals.GetViewModel<ISieuAmTim_2D_Consultation>();
            UCSieuAmTimTwoD = uc4;
            ActivateItem(uc4);
            //UCePrescription.Init();


            var uc5 = Globals.GetViewModel<ISieuAmTim_Droppler_Consultation>();
            UCSieuAmTimDoppler = uc5;
            ActivateItem(uc5);
            //UCePrescription.Init();

            var uc7 = Globals.GetViewModel<ISieuAmTim_KetLuan_Consultation>();
            UCSieuAmTimKetLuan = uc7;
            ActivateItem(uc7);
            //UCePrescription.Init();

            var uc9 = Globals.GetViewModel<IPCLDeptImagingResult_Consultation>();
            UCPatientPCLImageResults = uc9;
            ActivateItem(uc9);
        }
        public object UCPatientPCLImageResults { get; set; }

        public object UCSieuAmTimTM { get; set; }

        public object UCSieuAmTimTwoD { get; set; }
        public object UCSieuAmTimDoppler { get; set; }
        public object UCSieuAmTimKetLuan { get; set; }
        public object UCHinhAnhSieuAm { get; set; }


        public TabItem TabFirst { get; set; }
        public void TabFirst_Loaded(object sender, RoutedEventArgs e)
        {
            TabFirst = sender as TabItem;
            TabFirst.IsSelected = true;
        }

    }
}