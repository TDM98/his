using eHCMSLanguage;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Threading;
using System.Windows;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using aEMR.ViewContracts.Configuration;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using Caliburn.Micro;
using DataEntities;

namespace aEMR.Configuration.RoomType.ViewModels
{
    [Export(typeof(IRoomTypeInfo)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class RoomTypeInfoViewModel : Conductor<object>, IRoomTypeInfo
    {
        private bool _isLoading;
        public bool IsLoading
        {
            get { return _isLoading; }
            set
            {
                if (_isLoading!=value)
                {
                    _isLoading = value;
                    NotifyOfPropertyChange(() => IsLoading);
                }
            }
        }

        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public RoomTypeInfoViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;
        }

        private ObservableCollection<Lookup> _ObjV_RoomFunction;
        public ObservableCollection<Lookup> ObjV_RoomFunction
        {
            get { return _ObjV_RoomFunction; }
            set
            {
                _ObjV_RoomFunction = value;
                NotifyOfPropertyChange(() => ObjV_RoomFunction);
            }
        }

        private DataEntities.RoomType _ObjRoomType_Current;
        public DataEntities.RoomType ObjRoomType_Current
        {
            get { return _ObjRoomType_Current; }
            set
            {
                _ObjRoomType_Current = value;
                NotifyOfPropertyChange(()=>ObjRoomType_Current);
            }
        }

        private string _TitleForm;
        public string TitleForm
        {
            get { return _TitleForm; }
            set { _TitleForm = value;
            NotifyOfPropertyChange(() => TitleForm);
            }
        }

        public void InitializeNewItem(Int64 V_RoomFunction_Selected)
        {
            ObjRoomType_Current=new DataEntities.RoomType();
            ObjRoomType_Current.V_RoomFunction = V_RoomFunction_Selected;
        }

        public void btSave()
        {
            if (ObjRoomType_Current.V_RoomFunction > 0)
            {
                if(CheckValid(ObjRoomType_Current))
                {
                    RoomType_Save();
                }
            }
            else
            {
                MessageBox.Show(eHCMSResources.A0325_G1_Msg_InfoChonLoaiCNang, eHCMSResources.G0156_G1_Them, MessageBoxButton.OK);
            }

        }

        public bool CheckValid(object temp)
        {
            DataEntities.RoomType p = temp as DataEntities.RoomType;
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

        private void RoomType_Save()
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "Đang Lưu..." });

            var t = new Thread(() =>
            {
                IsLoading = true;

                using (var serviceFactory = new ConfigurationManagerServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginRoomType_Save(ObjRoomType_Current, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            string Res = "";
                            contract.EndRoomType_Save(out Res,asyncResult);
                            switch (Res)
                            {
                                case "Duplex-Name":
                                    {
                                        MessageBox.Show(string.Format("{0} {1}.", eHCMSResources.A1005_G1_Msg_InfoTenLoaiPgDaTonTai, eHCMSResources.A1006_G1_Msg_DungTenKhac), eHCMSResources.T2937_G1_Luu, MessageBoxButton.OK);
                                        break;
                                    }
                                case "Update-0":
                                    {
                                        MessageBox.Show(eHCMSResources.A0608_G1_Msg_InfoHChinhFail, eHCMSResources.T2937_G1_Luu, MessageBoxButton.OK);
                                        break;
                                    }
                                case "Update-1":
                                    {
                                        Globals.EventAggregator.Publish(new RoomTypeEvent_Save() { Result = true });
                                        TitleForm = string.Format("{0}: {1}", eHCMSResources.T1484_G1_HChinh, ObjRoomType_Current.RmTypeName.Trim());
                                        MessageBox.Show(eHCMSResources.A0468_G1_Msg_InfoLuuOK, eHCMSResources.T2937_G1_Luu, MessageBoxButton.OK);
                                        break;
                                    }
                                case "Insert-0":
                                    {
                                        MessageBox.Show(eHCMSResources.A1026_G1_Msg_InfoThemFail, eHCMSResources.T2937_G1_Luu, MessageBoxButton.OK);
                                        break;
                                    }
                                case "Insert-1":
                                    {
                                        Globals.EventAggregator.Publish(new RoomTypeEvent_Save() { Result = true });

                                        MessageBox.Show(eHCMSResources.A0468_G1_Msg_InfoLuuOK, eHCMSResources.T2937_G1_Luu, MessageBoxButton.OK);
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
