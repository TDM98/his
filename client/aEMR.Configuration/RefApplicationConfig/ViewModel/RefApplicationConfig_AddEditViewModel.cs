using eHCMSLanguage;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Threading;
using System.Windows;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.ViewContracts.Configuration;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using Caliburn.Micro;
using DataEntities;
using aEMR.ViewContracts;
using System.Collections.Generic;
using aEMR.Common.Collections;
using aEMR.Common;
using System.Linq;
/*
* 20221224 #001 BLQ: Create New
*/
namespace aEMR.Configuration.RefApplicationConfig.ViewModels
{
    [Export(typeof(IRefApplicationConfig_AddEdit)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class RefApplicationConfig_AddEditViewModel : Conductor<object>, IRefApplicationConfig_AddEdit
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public RefApplicationConfig_AddEditViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

        }
        private bool _isLoading;
        public bool IsLoading
        {
            get { return _isLoading; }
            set
            {
                if (_isLoading != value)
                {
                    _isLoading = value;
                    NotifyOfPropertyChange(() => IsLoading);
                }
            }
        }

        private DataEntities.RefApplicationConfig _ObjRefApplicationConfig_Current;
        public DataEntities.RefApplicationConfig ObjRefApplicationConfig_Current
        {
            get { return _ObjRefApplicationConfig_Current; }
            set
            {
                _ObjRefApplicationConfig_Current = value;
                NotifyOfPropertyChange(() => ObjRefApplicationConfig_Current);
            }
        }


        private string _TitleForm;
        public string TitleForm
        {
            get { return _TitleForm; }
            set
            {
                _TitleForm = value;
                NotifyOfPropertyChange(() => TitleForm);
            }
        }
   
        public void InitializeNewItem()
        {
            ObjRefApplicationConfig_Current = new DataEntities.RefApplicationConfig();
        }

        public void btSave()
        {

            if (!string.IsNullOrWhiteSpace(ObjRefApplicationConfig_Current.ConfigItemValue))
            {
                if (CheckValidRefApplicationConfig(ObjRefApplicationConfig_Current))
                {
                    RefApplicationConfig_InsertUpdate(ObjRefApplicationConfig_Current);
                }
            }
            else
            {
                MessageBox.Show("Chưa nhập cấu hình!", eHCMSResources.G0156_G1_Them, MessageBoxButton.OK);
            }
        }

        public bool CheckValidRefApplicationConfig(object temp)
        {
            DataEntities.RefApplicationConfig p = temp as DataEntities.RefApplicationConfig;
            if (p == null)
            {
                return false;
            }
            return p.Validate();
        }
      
        public void btClose()
        {
            TryClose();
        }

        private void RefApplicationConfig_InsertUpdate(DataEntities.RefApplicationConfig Obj)
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "Đang Lưu..." });

            var t = new Thread(() =>
            {
                IsLoading = true;

                using (var serviceFactory = new ConfigurationManagerServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    //Obj.CreatedStaffID = (long)Globals.LoggedUserAccount.StaffID;

                    contract.BeginRefApplicationConfig_Save(Obj, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            string Result = "";
                            contract.EndRefApplicationConfig_Save(out Result, asyncResult);
                            switch (Result)
                            {
                                case "Duplex-Name":
                                    {
                                        MessageBox.Show(string.Format("{0} {1}!", "Tên cấu hình đã tồn tại", eHCMSResources.A1006_G1_Msg_DungTenKhac), eHCMSResources.T2937_G1_Luu, MessageBoxButton.OK);
                                        break;
                                    }
                                case "Update-0":
                                    {
                                        MessageBox.Show(eHCMSResources.A0608_G1_Msg_InfoHChinhFail, eHCMSResources.T2937_G1_Luu, MessageBoxButton.OK);
                                        break;
                                    }
                                case "Update-1":
                                    {
                                        Globals.EventAggregator.Publish(new CRUDEvent());
                                        MessageBox.Show(eHCMSResources.A0468_G1_Msg_InfoLuuOK, eHCMSResources.T2937_G1_Luu, MessageBoxButton.OK);
                                        TryClose();
                                        break;
                                    }
                                case "Insert-0":
                                    {
                                        MessageBox.Show(eHCMSResources.A1026_G1_Msg_InfoThemFail, eHCMSResources.T2937_G1_Luu, MessageBoxButton.OK);
                                        break;
                                    }
                                case "Insert-1":
                                    {
                                        Globals.EventAggregator.Publish(new CRUDEvent());
                                        MessageBox.Show(eHCMSResources.A0468_G1_Msg_InfoLuuOK, eHCMSResources.T2937_G1_Luu, MessageBoxButton.OK);
                                        TryClose();
                                        break;
                                    }
                            }
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            //Globals.IsBusy = false;
                            IsLoading = false;
                        }
                    }), null);
                }


            });
            t.Start();
        }
   
    }
}
