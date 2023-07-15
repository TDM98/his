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

namespace aEMR.Configuration.AdmissionCriteria.ViewModels
{
    [Export(typeof(IAdmissionCriteria_AddEdit)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class AdmissionCriteria_AddEditViewModel : Conductor<object>, IAdmissionCriteria_AddEdit
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public AdmissionCriteria_AddEditViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;
            GetAdmissionCriteriaTypes();
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




        private DataEntities.AdmissionCriteria _ObjAdmissionCriteria_Current;
        public DataEntities.AdmissionCriteria ObjAdmissionCriteria_Current
        {
            get { return _ObjAdmissionCriteria_Current; }
            set
            {
                _ObjAdmissionCriteria_Current = value;
                NotifyOfPropertyChange(() => ObjAdmissionCriteria_Current);
            }
        }
        private ObservableCollection<Lookup> _AdmissionCriteriaTypes;
        public ObservableCollection<Lookup> AdmissionCriteriaTypes
        {
            get { return _AdmissionCriteriaTypes; }
            set
            {
                _AdmissionCriteriaTypes = value;
                NotifyOfPropertyChange(() => AdmissionCriteriaTypes);
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
            ObjAdmissionCriteria_Current = new DataEntities.AdmissionCriteria();
        }

        public void btSave()
        {
            if (ObjAdmissionCriteria_Current.AdmissionCriteriaName != "")
            {
                if (CheckValid(ObjAdmissionCriteria_Current))
                {
                    AdmissionCriteria_InsertUpdate(ObjAdmissionCriteria_Current, true);
                }
            }
            else
            {
                MessageBox.Show("Nhập đầy đủ thông tin mã!", eHCMSResources.G0156_G1_Them, MessageBoxButton.OK);
            }
        }

        public bool CheckValid(object temp)
        {
            DataEntities.AdmissionCriteria p = temp as DataEntities.AdmissionCriteria;
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

        private void AdmissionCriteria_InsertUpdate(DataEntities.AdmissionCriteria Obj, bool SaveToDB)
        {
            var t = new Thread(() =>
            {
                IsLoading = true;
                using (var serviceFactory = new ConfigurationManagerServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    Obj.CreatedStaffID = (long)Globals.LoggedUserAccount.StaffID;
                    contract.BeginAdmissionCriteria_Save(Obj, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            string Result = "";
                            contract.EndAdmissionCriteria_Save(out Result, asyncResult);
                            switch (Result)
                            {
                                case "Duplex-Name":
                                    {
                                        MessageBox.Show(string.Format("{0} {1}!", "Mã tiêu chí đã tồn tại!", eHCMSResources.A1006_G1_Msg_DungTenKhac), eHCMSResources.T2937_G1_Luu, MessageBoxButton.OK);
                                        break;
                                    }
                                case "Update-0":
                                    {
                                        MessageBox.Show(eHCMSResources.A0608_G1_Msg_InfoHChinhFail, eHCMSResources.T2937_G1_Luu, MessageBoxButton.OK);
                                        break;
                                    }
                                case "Update-1":
                                    {
                                        Globals.EventAggregator.Publish(new AdmissionCriteria_Event_Save() { Result = true });
                                        TitleForm = string.Format("{0} ({1})", eHCMSResources.T1484_G1_HChinh, ObjAdmissionCriteria_Current.AdmissionCriteriaCode.Trim());
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
                                        Globals.EventAggregator.Publish(new AdmissionCriteria_Event_Save() { Result = true });
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
        private void GetAdmissionCriteriaTypes()
        {
            AdmissionCriteriaTypes = new ObservableCollection<Lookup>();
            foreach (var tmpLookup in Globals.AllLookupValueList)
            {
                if (tmpLookup.ObjectTypeID == (long)(LookupValues.V_AdmissionCriteriaType))
                {
                    AdmissionCriteriaTypes.Add(tmpLookup);
                }
            }
        }
    }
}
