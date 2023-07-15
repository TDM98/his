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
    [Export(typeof (ISieuAmTimQuaThucQuanHome_Consultation)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class SieuAmTimQuaThucQuanHomeViewModel : Conductor<object>, ISieuAmTimQuaThucQuanHome_Consultation
    {

        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public SieuAmTimQuaThucQuanHomeViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            
            //Globals.EventAggregator.Subscribe(this);
            //Thay bằng
            //Globals.EventAggregator.Subscribe(this);

            var uc3 = Globals.GetViewModel<ISATQuaThucQuanChanDoan_Consultation>();
            UCChanDoan = uc3;
            ActivateItem(uc3);

            var uc4 = Globals.GetViewModel<ISATQuaThucQuanBangKiemTra_Consultation>();
            UCBangKiemTra = uc4;
            ActivateItem(uc4);

            var uc5 = Globals.GetViewModel<ISATQuaThucQuanQuyTrinh_Consultation>();
            UCQuyTrinh = uc5;
            ActivateItem(uc5);


            var uc6 = Globals.GetViewModel<ISATQuaThucQuanCD_Consultation>();
            UCChanDoanTQ = uc6;
            ActivateItem(uc6);

            //var uc7 = Globals.GetViewModel<ILinkInputPCLImaging>();
            //UCLinkInputPCLImagingView = uc7;
            //ActivateItem(uc7);

            var uc8 = Globals.GetViewModel<IPCLDeptImagingResult_Consultation>();
            UCPatientPCLImageResults = uc8;
            ActivateItem(uc8);
        }
        public object UCPatientPCLImageResults { get; set; }


        public object UCChanDoan { get; set; }
        public object UCBangKiemTra { get; set; }

        public object UCQuyTrinh { get; set; }
        public object UCChanDoanTQ { get; set; }

        public object UCLinkInputPCLImagingView { get; set; }

        //////public void resetPatientInfo()
        //////{
        //////    Globals.PCLDepartment.SetInfo(new Patient(), new PatientRegistration(), new PatientRegistrationDetail());

        //////    //Show Info
        //////    Globals.EventAggregator.Publish(
        //////        new ShowPatientInfo<Patient, PatientRegistration, PatientRegistrationDetail>
        //////            {
        //////                Pt = new Patient(),
        //////                PtReg = new PatientRegistration(),
        //////                PtRegDetail = new PatientRegistrationDetail()
        //////            });
        //////}


        public TabItem TabFirst { get; set; }
        public void TabFirst_Loaded(object sender, RoutedEventArgs e)
        {
            TabFirst = sender as TabItem;
            TabFirst.IsSelected = true;
        }
    }
}