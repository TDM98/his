using System;
using aEMR.ViewContracts;
using System.ComponentModel.Composition;
using DataEntities;
using Caliburn.Micro;
using aEMR.Infrastructure;
using aEMR.ServiceClient;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using System.Threading;
using System.Windows;
using eHCMSLanguage;
namespace aEMR.ConsultantEPrescription.ViewModels
{
    [Export(typeof(ISurgeryScheduleEdit)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class SurgeryScheduleEditViewModel : Conductor<object>, ISurgeryScheduleEdit
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        #region Constructor
        public SurgeryScheduleEditViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            DateTime mToday = Globals.GetCurServerDateTime();
            while (mToday.DayOfWeek != DayOfWeek.Saturday)
                mToday = mToday.AddDays(1);
            gSurgerySchedule = new SurgerySchedule { SSFromDate = mToday.AddDays(-5), SSToDate = mToday };
        }
        #endregion
        #region Properties
        private SurgerySchedule _gSurgerySchedule;
        public SurgerySchedule gSurgerySchedule
        {
            get { return _gSurgerySchedule; }
            set
            {
                _gSurgerySchedule = value;
                NotifyOfPropertyChange(() => gSurgerySchedule);
            }
        }
        #endregion
        #region Events
        public void btnAddSurgerySchedule()
        {
            var t = new Thread(() =>
            {
                this.ShowBusyIndicator();
                try
                {
                    using (var serviceFactory = new CommonUtilsServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginEditSurgerySchedule(gSurgerySchedule, Globals.DispatchCallback(asyncResult =>
                        {
                            try
                            {
                                long SurgeryScheduleID = 0;
                                if (contract.EndEditSurgerySchedule(out SurgeryScheduleID, asyncResult))
                                {
                                    if (gSurgerySchedule.SurgeryScheduleID == 0)
                                        MessageBox.Show(eHCMSResources.A0079_G1_Msg_InfoThemMoiOK, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                    else
                                        MessageBox.Show(eHCMSResources.A0279_G1_Msg_InfoCNhatOK, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                }
                                else
                                    if (gSurgerySchedule.SurgeryScheduleID == 0)
                                        MessageBox.Show(eHCMSResources.A1026_G1_Msg_InfoThemFail, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                    else
                                        MessageBox.Show(eHCMSResources.A0272_G1_Msg_InfoCNhatFail, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                            }
                            finally
                            {
                                this.HideBusyIndicator();
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }
        #endregion
    }
}
