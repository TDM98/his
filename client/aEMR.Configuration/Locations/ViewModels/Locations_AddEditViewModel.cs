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

namespace aEMR.Configuration.Locations.ViewModels
{
    [Export(typeof(ILocations_AddEdit)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class Locations_AddEditViewModel : Conductor<object>, ILocations_AddEdit
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public Locations_AddEditViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            GetAllV_SpecialistClinicType();
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


        private ObservableCollection<DataEntities.RoomType> _ObjRoomType_GetAll;
        public ObservableCollection<DataEntities.RoomType> ObjRoomType_GetAll
        {
            get { return _ObjRoomType_GetAll; }
            set
            {
                _ObjRoomType_GetAll = value;
                NotifyOfPropertyChange(() => ObjRoomType_GetAll);
            }
        }

        private DataEntities.Location _ObjLocation_Current;
        public DataEntities.Location ObjLocation_Current 
        {
            get { return _ObjLocation_Current; }
            set
            {
                _ObjLocation_Current = value;
                NotifyOfPropertyChange(() => ObjLocation_Current);
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
        private ObservableCollection<Lookup> _ObjSpecialistClinicType_GetAll;
        public ObservableCollection<Lookup> ObjSpecialistClinicType_GetAll
        {
            get { return _ObjSpecialistClinicType_GetAll; }
            set
            {
                _ObjSpecialistClinicType_GetAll = value;
                NotifyOfPropertyChange(() => ObjSpecialistClinicType_GetAll);
            }
        }

        public void InitializeNewItem(Int64 RmTypeID)
        {
            ObjLocation_Current =new Location();
            ObjLocation_Current.RmTypeID = RmTypeID;
        }

        public void GetAllV_SpecialistClinicType()
        {
            ObjSpecialistClinicType_GetAll = new ObservableCollection<Lookup>();
            foreach (var tmpLookup in Globals.AllLookupValueList)
            {
                if (tmpLookup.ObjectTypeID == (long)(LookupValues.V_SpecialistClinicType))
                {
                    ObjSpecialistClinicType_GetAll.Add(tmpLookup);
                }
            }
        }
        public void btSave()
        {
            if (ObjLocation_Current.RmTypeID > 0)
            {
                if (CheckValid(ObjLocation_Current))
                {
                    Locations_InsertUpdate(ObjLocation_Current,true);
                }
            }
            else
            {
                MessageBox.Show(eHCMSResources.A0332_G1_Msg_InfoChonLoaiPg, eHCMSResources.G0156_G1_Them, MessageBoxButton.OK);
            }
        }

        public bool CheckValid(object temp)
        {
            DataEntities.Location p = temp as DataEntities.Location;
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

        private void Locations_InsertUpdate(DataEntities.Location Obj,bool SaveToDB)
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "Đang Lưu..." });
          
            var t = new Thread(() =>
            {
                IsLoading = true;

                using (var serviceFactory = new ConfigurationManagerServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginLocations_InsertUpdate(Obj,SaveToDB, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            string Result = "";
                            contract.EndLocations_InsertUpdate(out Result, asyncResult);
                            switch (Result)
                            {
                                case "Duplex-Name":
                                    {
                                        MessageBox.Show(string.Format("{0} {1}!", eHCMSResources.A1009_G1_Msg_InfoTenPgDaTonTai, eHCMSResources.A1006_G1_Msg_DungTenKhac), eHCMSResources.T2937_G1_Luu, MessageBoxButton.OK);
                                        break;
                                    }
                                case "Update-0":
                                    {
                                        MessageBox.Show(eHCMSResources.A0608_G1_Msg_InfoHChinhFail, eHCMSResources.T2937_G1_Luu, MessageBoxButton.OK);
                                        break;
                                    }
                                case "Update-1":
                                    {
                                        Globals.EventAggregator.Publish(new Location_Event_Save() { Result = true });
                                        TitleForm = string.Format("{0} ({1})", eHCMSResources.T1484_G1_HChinh, ObjLocation_Current.LocationName.Trim());
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
                                        Globals.EventAggregator.Publish(new Location_Event_Save() { Result = true });

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
