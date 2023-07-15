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

namespace aEMR.Configuration.CitiesProvinces.ViewModels
{
    [Export(typeof(ICitiesProvinces_AddEdit)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class CitiesProvinces_AddEditViewModel : Conductor<object>, ICitiesProvinces_AddEdit
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public CitiesProvinces_AddEditViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;
            LoadProvinces();

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
        private ObservableCollection<CitiesProvince> _provinces;
        public ObservableCollection<CitiesProvince> Provinces
        {
            get { return _provinces; }
            set
            {
                _provinces = value;
                NotifyOfPropertyChange(() => Provinces);
            }
        }
        private ObservableCollection<SuburbNames> _SuburbNames;
        public ObservableCollection<SuburbNames> SuburbNames
        {
            get { return _SuburbNames; }
            set
            {
                _SuburbNames = value;
                NotifyOfPropertyChange(() => SuburbNames);
            }
        }
        private CitiesProvince _ObjCitiesProvinces_Current;
        public CitiesProvince ObjCitiesProvinces_Current
        {
            get { return _ObjCitiesProvinces_Current; }
            set
            {
                _ObjCitiesProvinces_Current = value;
                NotifyOfPropertyChange(() => ObjCitiesProvinces_Current);
            }
        }
        private SuburbNames _ObjSuburbNames_Current;
        public SuburbNames ObjSuburbNames_Current
        {
            get { return _ObjSuburbNames_Current; }
            set
            {
                _ObjSuburbNames_Current = value;
                NotifyOfPropertyChange(() => ObjSuburbNames_Current);
            }
        }
        private WardNames _ObjWardNames_Current;
        public WardNames ObjWardNames_Current
        {
            get { return _ObjWardNames_Current; }
            set
            {
                _ObjWardNames_Current = value;
                NotifyOfPropertyChange(() => ObjWardNames_Current);
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
        private long _FormType;
        public long FormType
        {
            get { return _FormType; }
            set
            {
                _FormType = value;
                NotifyOfPropertyChange(() => FormType);
                CheckVisibility();
            }
        }
        private Visibility _CititesProvincesVisible = Visibility.Collapsed;
        public Visibility CititesProvincesVisible
        {
            get { return _CititesProvincesVisible; }
            set
            {
                _CititesProvincesVisible = value;
                NotifyOfPropertyChange(() => CititesProvincesVisible);
            }
        }
        private Visibility _SuburbNamesVisible = Visibility.Collapsed;
        public Visibility SuburbNamesVisible
        {
            get { return _SuburbNamesVisible; }
            set
            {
                _SuburbNamesVisible = value;
                NotifyOfPropertyChange(() => SuburbNamesVisible);
            }
        }
        private Visibility _WardNamesVisible = Visibility.Collapsed;
        public Visibility WardNamesVisible
        {
            get { return _WardNamesVisible; }
            set
            {
                _WardNamesVisible = value;
                NotifyOfPropertyChange(() => WardNamesVisible);
            }
        }
        public void CheckVisibility()
        {
            switch (FormType)
            {
                case 1:
                    CititesProvincesVisible = Visibility.Visible;
                    break;
                case 2:
                    SuburbNamesVisible = Visibility.Visible;
                    break;
                case 3:
                    WardNamesVisible = Visibility.Visible;
                    break;
            }
        }
        public void InitializeNewItem()
        {
            ObjCitiesProvinces_Current = new CitiesProvince();
            ObjSuburbNames_Current = new SuburbNames();
            ObjWardNames_Current = new WardNames();
        }

        public void btSave()
        {
            switch (FormType)
            {
                case 1:
                    if (ObjCitiesProvinces_Current.CityProvinceName != "" && ObjCitiesProvinces_Current.CityProviceCode != "" && ObjCitiesProvinces_Current.CityProviceHICode != "")
                    {
                        if (CheckValidCitiesProvinces(ObjCitiesProvinces_Current))
                        {
                            CitiesProvinces_InsertUpdate(ObjCitiesProvinces_Current, true);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Nhập đầy đủ thông tin mã Tỉnh!", eHCMSResources.G0156_G1_Them, MessageBoxButton.OK);
                    }
                    break;
                case 2:
                    if (ObjSuburbNames_Current.CityProvinceID != 0 && ObjSuburbNames_Current.SuburbName != "")
                    {
                        if (CheckValidSuburbNames(ObjSuburbNames_Current))
                        {
                            SuburbNames_InsertUpdate(ObjSuburbNames_Current, true);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Nhập đầy đủ thông tin!", eHCMSResources.G0156_G1_Them, MessageBoxButton.OK);
                    }
                    break;
                case 3:
                    if (ObjWardNames_Current.SuburbNameID != 0 && ObjWardNames_Current.WardName != "")
                    {
                        if (CheckValidWardNames(ObjWardNames_Current))
                        {
                            WardNames_InsertUpdate(ObjWardNames_Current, true);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Nhập đầy đủ thông tin!", eHCMSResources.G0156_G1_Them, MessageBoxButton.OK);
                    }
                    break;
            }

        }

        public bool CheckValidCitiesProvinces(object temp)
        {
            DataEntities.CitiesProvince p = temp as DataEntities.CitiesProvince;
            if (p == null)
            {
                return false;
            }
            return p.Validate();
        }
        public bool CheckValidSuburbNames(object temp)
        {
            DataEntities.SuburbNames p = temp as DataEntities.SuburbNames;
            if (p == null)
            {
                return false;
            }
            return p.Validate();
        }
        public bool CheckValidWardNames(object temp)
        {
            DataEntities.WardNames p = temp as DataEntities.WardNames;
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

        private void CitiesProvinces_InsertUpdate(CitiesProvince Obj, bool SaveToDB)
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "Đang Lưu..." });

            var t = new Thread(() =>
            {
                IsLoading = true;

                using (var serviceFactory = new ConfigurationManagerServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    Obj.DateModified = Convert.ToDateTime(Globals.ServerDate);
                    Obj.ModifiedLog = Globals.LoggedUserAccount.StaffID.ToString();

                    contract.BeginCitiesProvinces_Save(Obj, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            string Result = "";
                            contract.EndCitiesProvinces_Save(out Result, asyncResult);
                            switch (Result)
                            {
                                case "Duplex-Name":
                                    {
                                        MessageBox.Show(string.Format("{0} {1}!", "Tên tỉnh đã tồn tại", eHCMSResources.A1006_G1_Msg_DungTenKhac), eHCMSResources.T2937_G1_Luu, MessageBoxButton.OK);
                                        break;
                                    }
                                case "Update-0":
                                    {
                                        MessageBox.Show(eHCMSResources.A0608_G1_Msg_InfoHChinhFail, eHCMSResources.T2937_G1_Luu, MessageBoxButton.OK);
                                        break;
                                    }
                                case "Update-1":
                                    {
                                        Globals.EventAggregator.Publish(new CitiesProvinces_Event_Save() { FormType = FormType, Result = true });
                                        TitleForm = string.Format("{0} ({1})", eHCMSResources.T1484_G1_HChinh, ObjCitiesProvinces_Current.CityProvinceName.Trim());
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
                                        Globals.EventAggregator.Publish(new CitiesProvinces_Event_Save() { FormType = FormType, Result = true });
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
        private void SuburbNames_InsertUpdate(SuburbNames Obj, bool SaveToDB)
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "Đang Lưu..." });

            var t = new Thread(() =>
            {
                IsLoading = true;

                using (var serviceFactory = new ConfigurationManagerServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    Obj.DateModified = Convert.ToDateTime(Globals.ServerDate);
                    Obj.ModifiedLog = Globals.LoggedUserAccount.StaffID.ToString();

                    contract.BeginSuburbNames_Save(Obj, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            string Result = "";
                            contract.EndSuburbNames_Save(out Result, asyncResult);
                            switch (Result)
                            {
                                case "Duplex-Name":
                                    {
                                        MessageBox.Show(string.Format("{0} {1}!", "Tên quận huyện đã tồn tại", eHCMSResources.A1006_G1_Msg_DungTenKhac), eHCMSResources.T2937_G1_Luu, MessageBoxButton.OK);
                                        break;
                                    }
                                case "Update-0":
                                    {
                                        MessageBox.Show(eHCMSResources.A0608_G1_Msg_InfoHChinhFail, eHCMSResources.T2937_G1_Luu, MessageBoxButton.OK);
                                        break;
                                    }
                                case "Update-1":
                                    {
                                        Globals.EventAggregator.Publish(new CitiesProvinces_Event_Save() { FormType = FormType, Result = true });
                                        TitleForm = string.Format("{0} ({1})", eHCMSResources.T1484_G1_HChinh, ObjSuburbNames_Current.SuburbName.Trim());
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
                                        Globals.EventAggregator.Publish(new CitiesProvinces_Event_Save() { FormType = FormType, Result = true });
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
        private void WardNames_InsertUpdate(WardNames Obj, bool SaveToDB)
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "Đang Lưu..." });

            var t = new Thread(() =>
            {
                IsLoading = true;

                using (var serviceFactory = new ConfigurationManagerServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    Obj.DateModified = Convert.ToDateTime(Globals.ServerDate);
                    Obj.ModifiedLog = Globals.LoggedUserAccount.StaffID.ToString();

                    contract.BeginWardNames_Save(Obj, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            string Result = "";
                            contract.EndWardNames_Save(out Result, asyncResult);
                            switch (Result)
                            {
                                case "Duplex-Name":
                                    {
                                        MessageBox.Show(string.Format("{0} {1}!", "Tên phường xã đã tồn tại", eHCMSResources.A1006_G1_Msg_DungTenKhac), eHCMSResources.T2937_G1_Luu, MessageBoxButton.OK);
                                        break;
                                    }
                                case "Update-0":
                                    {
                                        MessageBox.Show(eHCMSResources.A0608_G1_Msg_InfoHChinhFail, eHCMSResources.T2937_G1_Luu, MessageBoxButton.OK);
                                        break;
                                    }
                                case "Update-1":
                                    {
                                        Globals.EventAggregator.Publish(new CitiesProvinces_Event_Save() { FormType = FormType, Result = true });
                                        TitleForm = string.Format("{0} ({1})", eHCMSResources.T1484_G1_HChinh, ObjWardNames_Current.WardName.Trim());
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
                                        Globals.EventAggregator.Publish(new CitiesProvinces_Event_Save() { FormType = FormType, Result = true });
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
                            TryClose();
                        }
                    }), null);
                }


            });
            t.Start();
        }
        public void LoadProvinces()
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new CommonService_V2Client())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginGetAllProvinces(Globals.DispatchCallback(asyncResult =>
                        {
                            IList<CitiesProvince> allItems = null;
                            try
                            {
                                allItems = contract.EndGetAllProvinces(asyncResult);
                                //if (Globals.allCitiesProvince == null)
                                //{
                                //    Globals.allCitiesProvince = new List<CitiesProvince>(allItems);
                                //}
                            }
                            catch (Exception ex)
                            {
                                ClientLoggerHelper.LogInfo(ex.ToString());
                                MessageBox.Show(eHCMSResources.A0693_G1_Msg_InfoKhTheLayDSTinhThanh);
                            }
                            finally
                            {
                                this.HideBusyIndicator();
                            }

                            Provinces = allItems != null ? new ObservableCollection<CitiesProvince>(allItems) : null;
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }
        private void LoadSuburbName()
        {

            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new CommonService_V2Client())
                    {

                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetAllSuburbNames(Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var allItems = contract.EndGetAllSuburbNames(asyncResult);
                                if (allItems != null)
                                {
                                    Globals.allSuburbNames = allItems.ToList();
                                }
                            }
                            catch (Exception ex)
                            {
                                ClientLoggerHelper.LogError(ex.ToString());
                                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                            }
                            finally
                            {

                            }

                        }), null);

                    }
                }
                catch (Exception ex)
                {
                    ClientLoggerHelper.LogError(ex.ToString());
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }
        public void cboCitiesProvince2SelectedItemChanged(object selectedItem)
        {
            long CitiesProvinces_ID = (selectedItem as DataEntities.CitiesProvince).CityProvinceID;
            SuburbNames = new ObservableCollection<SuburbNames>();
            SuburbNames firstItem = new SuburbNames();
            firstItem.SuburbNameID = 0;
            firstItem.SuburbName = "-- Tất cả --";
            SuburbNames.Add(firstItem);
            foreach (var item in Globals.allSuburbNames)
            {
                if (item.CityProvinceID == CitiesProvinces_ID)
                {
                    SuburbNames.Add(item);
                }
            }
        }
    }
}
