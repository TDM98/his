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

namespace aEMR.Configuration.AdmissionCriterion_Mgnt.ViewModels
{
    [Export(typeof(ISymptomCategory_AddEdit)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class SymptomCategory_AddEditViewModel : Conductor<object>, ISymptomCategory_AddEdit
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public SymptomCategory_AddEditViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;
            GetSymptomCategoryTypes();
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




        private SymptomCategory _ObjSymptomCategory_Current;
        public SymptomCategory ObjSymptomCategory_Current
        {
            get { return _ObjSymptomCategory_Current; }
            set
            {
                _ObjSymptomCategory_Current = value;
                NotifyOfPropertyChange(() => ObjSymptomCategory_Current);
            }
        }
        private ObservableCollection<Lookup> _SymptomTypes;
        public ObservableCollection<Lookup> SymptomTypes
        {
            get { return _SymptomTypes; }
            set
            {
                _SymptomTypes = value;
                NotifyOfPropertyChange(() => SymptomTypes);
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
            ObjSymptomCategory_Current = new SymptomCategory();
        }

        public void btSave()
        {
            if (string.IsNullOrEmpty(ObjSymptomCategory_Current.SymptomCategoryName))
            {
                MessageBox.Show("Tên không được để trống!", eHCMSResources.G0156_G1_Them, MessageBoxButton.OK);
                return;
            }
            if(ObjSymptomCategory_Current.V_SymptomType == 0)
            {
                MessageBox.Show("Chưa chọn loại tiêu chuẩn!", eHCMSResources.G0156_G1_Them, MessageBoxButton.OK);
                return;
            }
            if (CheckValid(ObjSymptomCategory_Current))
            {
                SymptomCategory_InsertUpdate(ObjSymptomCategory_Current, true);
            }
        }

        public bool CheckValid(object temp)
        {
            SymptomCategory p = temp as SymptomCategory;
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

        private void SymptomCategory_InsertUpdate(SymptomCategory Obj, bool SaveToDB)
        {
            var t = new Thread(() =>
            {
                IsLoading = true;
                using (var serviceFactory = new ConfigurationManagerServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    Obj.CreatedStaffID = (long)Globals.LoggedUserAccount.StaffID;
                    contract.BeginSymptomCategory_Save(Obj, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            string Result = "";
                            contract.EndSymptomCategory_Save(out Result, asyncResult);
                            switch (Result)
                            {
                                case "Duplex-Name":
                                    {
                                        MessageBox.Show(string.Format("{0} {1}!", "Triệu chứng đã tồn tại!", eHCMSResources.A1006_G1_Msg_DungTenKhac), eHCMSResources.T2937_G1_Luu, MessageBoxButton.OK);
                                        break;
                                    }
                                case "Update-0":
                                    {
                                        MessageBox.Show(eHCMSResources.A0608_G1_Msg_InfoHChinhFail, eHCMSResources.T2937_G1_Luu, MessageBoxButton.OK);
                                        break;
                                    }
                                case "Update-1":
                                    {
                                        Globals.EventAggregator.Publish(new SymptomCategory_Event_Save() { Result = true });
                                        TitleForm = string.Format("{0}", eHCMSResources.T1484_G1_HChinh);
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
                                        Globals.EventAggregator.Publish(new SymptomCategory_Event_Save() { Result = true });
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
        private void GetSymptomCategoryTypes()
        {
            SymptomTypes = new ObservableCollection<Lookup>();
            foreach (var tmpLookup in Globals.AllLookupValueList)
            {
                if (tmpLookup.ObjectTypeID == (long)(LookupValues.V_SymptomType))
                {
                    SymptomTypes.Add(tmpLookup);
                }
            }
        }
    }
}
