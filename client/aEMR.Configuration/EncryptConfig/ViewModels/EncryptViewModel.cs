using eHCMSLanguage;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ServiceModel;
using System.Threading;
using AppConfigKeys;
using aEMR.DataContracts;
using aEMR.Infrastructure;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using aEMR.ViewContracts.Configuration;
using Caliburn.Micro;
using aEMR.Common;
using Castle.Windsor;
using Castle.Core.Logging;
using aEMR.Infrastructure.CachingUtils;

namespace aEMR.Configuration.EncryptConfig.ViewModels
{
    [Export(typeof(IEncrypt)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class EncryptViewModel : Conductor<object>, IEncrypt
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public EncryptViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

        }
        protected override void OnActivate()
        {
            base.OnActivate();
        }

        public string Pass { get; set; }
        public string ConfirmPass { get; set; }
        public string txtUser { get; set; }

        public string LocalFolderName { get; set; }
        public string NetWorkMapDriver { get; set; }
        public string PCLImageStoragePath { get; set; }
        

        private bool Validate()
        {
            if (txtUser=="")
            {
                Globals.ShowMessage(eHCMSResources.Z1721_G1_ChuaNhapTenUser, "");
                return false;
            }
            
            
            if (Pass != ConfirmPass)
            {
                Globals.ShowMessage(eHCMSResources.Z1722_G1_ConfirmPassSai, "");
                return false;
            }
            
            return true;
        }

        public void butSave()
        {
            if (Validate())
            {
                string user = EncryptExtension.Encrypt(txtUser, Globals.AxonKey,Globals.AxonPass);
                string password = EncryptExtension.Encrypt(Pass, Globals.AxonKey, Globals.AxonPass);
                //insert vao database
                UpdateRefApplicationConfigs(ConfigItemKey.NWMDUser.ToString(), user, "");
                UpdateRefApplicationConfigs(ConfigItemKey.NWMDPass.ToString(), password, "");
            }
        }
        public void Save()
        {
            try
            {
                if (LocalFolderName!=null && LocalFolderName != "")
                {
                    string localFolderName = EncryptExtension.Encrypt(LocalFolderName, Globals.AxonKey, Globals.AxonPass);
                    UpdateRefApplicationConfigs(ConfigItemKey.LocalFolderName.ToString(), localFolderName, "");
                }
            }catch{}
            
            try
            {
                if (NetWorkMapDriver!=null && NetWorkMapDriver != "")
                {
                    string netWorkMapDriver = EncryptExtension.Encrypt(NetWorkMapDriver, Globals.AxonKey, Globals.AxonPass);
                    UpdateRefApplicationConfigs(ConfigItemKey.NetWorkMapDriver.ToString(), netWorkMapDriver, "");
                }
            }
            catch { }
            
            try
            {
                if (PCLImageStoragePath!=null && PCLImageStoragePath != "")
                {
                    string pCLImageStoragePath = EncryptExtension.Encrypt(PCLImageStoragePath, Globals.AxonKey, Globals.AxonPass);
                    UpdateRefApplicationConfigs(ConfigItemKey.PCLImageStoragePath.ToString(), pCLImageStoragePath, "");
                } 
            }
            catch { }
            
            
        }
        public void UpdateRefApplicationConfigs(string ConfigItemKey, string ConfigItemValue,
                                                      string ConfigItemNotes)
        {
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new CommonService_V2Client())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginUpdateRefApplicationConfigs(ConfigItemKey, ConfigItemValue,
                                                      ConfigItemNotes,
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                
                                try
                                {
                                    var res= contract.EndUpdateRefApplicationConfigs(asyncResult);
                                    if(res)
                                    {
                                        Globals.ShowMessage(eHCMSResources.A0296_G1_Msg_InfoSuaOK, "");
                                    }else
                                    {
                                        Globals.ShowMessage(eHCMSResources.Z1723_G1_ChSuaThatBai, "");
                                    }
                                }
                                catch (FaultException<AxException> fault)
                                {
                                    ClientLoggerHelper.LogInfo(fault.ToString());
                                }
                                catch (Exception ex)
                                {
                                    ClientLoggerHelper.LogInfo(ex.ToString());
                                }
                            }), null);
                    }
                }
                catch (Exception ex)
                {
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0074_G1_I);
                }
            });
            t.Start();
        }
    }
}
