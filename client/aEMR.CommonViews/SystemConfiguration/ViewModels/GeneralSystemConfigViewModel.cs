using System;
using System.ComponentModel.Composition;
using System.Threading;
using aEMR.Common.BaseModel;
using aEMR.Infrastructure;
using aEMR.Infrastructure.CachingUtils;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using Caliburn.Micro;
using Castle.Windsor;
using eHCMSLanguage;

namespace aEMR.SystemConfiguration.ViewModels
{
    [Export(typeof(IGeneralSystemConfig)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class GeneralSystemConfigViewModel : ViewModelBase, IGeneralSystemConfig
    {
        #region Events
        [ImportingConstructor]
        public GeneralSystemConfigViewModel(IWindsorContainer aContainer, INavigationService aNavigation, IEventAggregator aAggregator, ISalePosCaching aCaching)
        {
        }
        public void ReCachingCmd()
        {
            this.ShowBusyIndicator();
            var mThread = new Thread(() =>
            {
                using (var serviceFactory = new ConfigurationManagerServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginReCaching(Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            if (contract.EndReCaching(asyncResult))
                            {
                                GlobalsNAV.ShowMessagePopup(eHCMSResources.Z2715_G1_ThanhCong);
                            }
                        }
                        catch (Exception ex)
                        {
                            GlobalsNAV.ShowMessagePopup(ex.Message);
                        }
                        finally
                        {
                            this.HideBusyIndicator();
                        }
                    }), null);
                }
            });
            mThread.Start();
        }
        #endregion
    }
}