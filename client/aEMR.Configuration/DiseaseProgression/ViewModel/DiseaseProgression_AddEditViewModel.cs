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

namespace aEMR.Configuration.DiseaseProgression.ViewModels
{
    [Export(typeof(IDiseaseProgression_AddEdit)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class DiseaseProgression_AddEditViewModel : Conductor<object>, IDiseaseProgression_AddEdit
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public DiseaseProgression_AddEditViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
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
        private ObservableCollection<DataEntities.DiseaseProgression> _DiseaseProgression;
        public ObservableCollection<DataEntities.DiseaseProgression> DiseaseProgression
        {
            get { return _DiseaseProgression; }
            set
            {
                _DiseaseProgression = value;
                NotifyOfPropertyChange(() => DiseaseProgression);
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
        private DataEntities.DiseaseProgression _ObjDiseaseProgression_Current;
        public DataEntities.DiseaseProgression ObjDiseaseProgression_Current
        {
            get { return _ObjDiseaseProgression_Current; }
            set
            {
                _ObjDiseaseProgression_Current = value;
                NotifyOfPropertyChange(() => ObjDiseaseProgression_Current);
                if (ObjDiseaseProgression_Current.UseForWebsite)
                {
                    WebChecked = true;
                }
                else
                {
                    HisChecked = true;
                }
            }
        }
        private DiseaseProgressionDetails _ObjDiseaseProgressionDetails_Current;
        public DiseaseProgressionDetails ObjDiseaseProgressionDetails_Current
        {
            get { return _ObjDiseaseProgressionDetails_Current; }
            set
            {
                _ObjDiseaseProgressionDetails_Current = value;
                NotifyOfPropertyChange(() => ObjDiseaseProgressionDetails_Current);
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
        private Visibility _DiseaseProgressionVisible = Visibility.Collapsed;
        public Visibility DiseaseProgressionVisible
        {
            get { return _DiseaseProgressionVisible; }
            set
            {
                _DiseaseProgressionVisible = value;
                NotifyOfPropertyChange(() => DiseaseProgressionVisible);
            }
        }
        private Visibility _DiseaseProgressionDetailVisible = Visibility.Collapsed;
        public Visibility DiseaseProgressionDetailVisible
        {
            get { return _DiseaseProgressionDetailVisible; }
            set
            {
                _DiseaseProgressionDetailVisible = value;
                NotifyOfPropertyChange(() => DiseaseProgressionDetailVisible);
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
                    DiseaseProgressionVisible = Visibility.Visible;
                    break;
                case 2:
                    DiseaseProgressionDetailVisible = Visibility.Visible;
                    break;
            }
        }
        public void InitializeNewItem()
        {
            ObjDiseaseProgression_Current = new DataEntities.DiseaseProgression();
            ObjDiseaseProgressionDetails_Current = new DiseaseProgressionDetails();
        }

        public void btSave()
        {
            switch (FormType)
            {
                case 1:
                    if (ObjDiseaseProgression_Current.DiseaseProgressionName != "")
                    {
                        if (CheckValidDiseaseProgression(ObjDiseaseProgression_Current))
                        {
                            DiseaseProgression_InsertUpdate(ObjDiseaseProgression_Current);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Chưa nhập tên diễn tiến!", eHCMSResources.G0156_G1_Them, MessageBoxButton.OK);
                    }
                    break;
                case 2:
                    if (ObjDiseaseProgressionDetails_Current.DiseaseProgressionID != 0 && ObjDiseaseProgressionDetails_Current.DiseaseProgressionDetailName != "")
                    {
                        if (CheckValidDiseaseProgressionDetails(ObjDiseaseProgressionDetails_Current))
                        {
                            DiseaseProgressionDetails_InsertUpdate(ObjDiseaseProgressionDetails_Current);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Nhập đầy đủ thông tin!", eHCMSResources.G0156_G1_Them, MessageBoxButton.OK);
                    }
                    break;
            }
        }

        public bool CheckValidDiseaseProgression(object temp)
        {
            DataEntities.DiseaseProgression p = temp as DataEntities.DiseaseProgression;
            if (p == null)
            {
                return false;
            }
            return p.Validate();
        }
        public bool CheckValidDiseaseProgressionDetails(object temp)
        {
            DiseaseProgressionDetails p = temp as DiseaseProgressionDetails;
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

        private void DiseaseProgression_InsertUpdate(DataEntities.DiseaseProgression Obj)
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "Đang Lưu..." });

            var t = new Thread(() =>
            {
                IsLoading = true;

                using (var serviceFactory = new ConfigurationManagerServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    Obj.CreatedStaffID = (long)Globals.LoggedUserAccount.StaffID;

                    contract.BeginDiseaseProgression_Save(Obj, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            string Result = "";
                            contract.EndDiseaseProgression_Save(out Result, asyncResult);
                            switch (Result)
                            {
                                case "Duplex-Name":
                                    {
                                        MessageBox.Show(string.Format("{0} {1}!", "Tên diễn tiến đã tồn tại", eHCMSResources.A1006_G1_Msg_DungTenKhac), eHCMSResources.T2937_G1_Luu, MessageBoxButton.OK);
                                        break;
                                    }
                                case "Update-0":
                                    {
                                        MessageBox.Show(eHCMSResources.A0608_G1_Msg_InfoHChinhFail, eHCMSResources.T2937_G1_Luu, MessageBoxButton.OK);
                                        break;
                                    }
                                case "Update-1":
                                    {
                                        Globals.EventAggregator.Publish(new DiseaseProgression_Event_Save() { FormType = FormType, Result = true });
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
                                        Globals.EventAggregator.Publish(new DiseaseProgression_Event_Save() { FormType = FormType, Result = true });
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
        private void DiseaseProgressionDetails_InsertUpdate(DiseaseProgressionDetails Obj)
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "Đang Lưu..." });

            var t = new Thread(() =>
            {
                IsLoading = true;

                using (var serviceFactory = new ConfigurationManagerServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    
                    Obj.CreatedStaffID = (long)Globals.LoggedUserAccount.StaffID;

                    contract.BeginDiseaseProgressionDetails_Save(Obj, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            string Result = "";
                            contract.EndDiseaseProgressionDetails_Save(out Result, asyncResult);
                            switch (Result)
                            {
                                case "Duplex-Name":
                                    {
                                        MessageBox.Show(string.Format("{0} {1}!", "Tên chi tiết đã tồn tại trong diễn tiến", eHCMSResources.A1006_G1_Msg_DungTenKhac), eHCMSResources.T2937_G1_Luu, MessageBoxButton.OK);
                                        break;
                                    }
                                case "Update-0":
                                    {
                                        MessageBox.Show(eHCMSResources.A0608_G1_Msg_InfoHChinhFail, eHCMSResources.T2937_G1_Luu, MessageBoxButton.OK);
                                        break;
                                    }
                                case "Update-1":
                                    {
                                        Globals.EventAggregator.Publish(new DiseaseProgression_Event_Save() { FormType = FormType, Result = true });
                                        TitleForm = string.Format("{0} ({1})", eHCMSResources.T1484_G1_HChinh, ObjDiseaseProgressionDetails_Current.DiseaseProgressionDetailName.Trim());
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
                                        Globals.EventAggregator.Publish(new DiseaseProgression_Event_Save() { FormType = FormType, Result = true });
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
        private bool _HisChecked;
        public bool HisChecked
        {
            get { return _HisChecked; }
            set
            {
                _HisChecked = value;
                NotifyOfPropertyChange(() => HisChecked);
            }
        }
        private bool _WebChecked;
        public bool WebChecked
        {
            get { return _WebChecked; }
            set
            {
                _WebChecked = value;
                NotifyOfPropertyChange(() => WebChecked);
            }
        }

        public void RadioButtonUseForWeb_Checked(object sender, RoutedEventArgs e)
        {
            if (HisChecked)
            {
                ObjDiseaseProgression_Current.UseForWebsite = false;
            }
            else if (WebChecked)
            {
                ObjDiseaseProgression_Current.UseForWebsite = true;
            }
        }
      
    }
}
