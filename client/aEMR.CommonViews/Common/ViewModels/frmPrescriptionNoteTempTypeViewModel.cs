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
using System.ServiceModel;
using aEMR.DataContracts;
using eHCMSLanguage;
using System.Windows.Controls;
/*
 * 20230629 #001 BLQ: Kiểm tra lời dặn có ký tự '<' và '>'
*/
namespace aEMR.Common.ViewModels
{
    [Export(typeof(IfrmPrescriptionNoteTempType)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class frmPrescriptionNoteTempTypeViewModel : Conductor<object>, IfrmPrescriptionNoteTempType
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public frmPrescriptionNoteTempTypeViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            _allPrescriptionNoteTemplates = new ObservableCollection<PrescriptionNoteTemplates>();
            UCPreNoteTemp = Globals.GetViewModel<IEnumListing>();
            UCPreNoteTemp.EnumType=typeof(AllLookupValues.V_PrescriptionNoteTempType);
            UCPreNoteTemp.AddSelectOneItem = false;
            UCPreNoteTemp.LoadData();
            curPrescriptionNoteTemplates = new PrescriptionNoteTemplates();
            if (UCPreNoteTemp != null)
            {
                PrescriptionNoteTemplates_GetAllIsActive(PrescriptionNoteTempType);
            }
            (UCPreNoteTemp as INotifyPropertyChangedEx).PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(frmPrescriptionNoteTempTypeViewModel_PropertyChanged);            
        }

        void frmPrescriptionNoteTempTypeViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SelectedItem")
            {
                PrescriptionNoteTemplates_GetAllIsActive(PrescriptionNoteTempType);
            }
        }

        private IEnumListing _UCPreNoteTemp;

        public IEnumListing UCPreNoteTemp
        {
            get { return _UCPreNoteTemp; }
            set
            {
                _UCPreNoteTemp = value;
                NotifyOfPropertyChange(() => UCPreNoteTemp);
            }
        }

        private bool _isPopup;
        public bool isPopup
        {
            get { return _isPopup; }
            set
            {
                _isPopup = value;
                NotifyOfPropertyChange(() => isPopup);
            }
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get { return _isLoading; }
            set
            {
                _isLoading = value;
                NotifyOfPropertyChange(() => IsLoading);
            }
        }

        private bool _mLoiDan=false;
        public bool mLoiDan
        {
            get { return _mLoiDan; }
            set
            {
                _mLoiDan = value;
                NotifyOfPropertyChange(() => mLoiDan);
            }
        }

        public AllLookupValues.V_PrescriptionNoteTempType? PrescriptionNoteTempType
        {
            get 
            {
                if (UCPreNoteTemp != null
                    &&UCPreNoteTemp.SelectedItem != null)
                {
                    if ((AllLookupValues.V_PrescriptionNoteTempType?)(UCPreNoteTemp.SelectedItem.EnumItem) ==
                        AllLookupValues.V_PrescriptionNoteTempType.PrescriptionNoteGen
                        ||(AllLookupValues.V_PrescriptionNoteTempType?)(UCPreNoteTemp.SelectedItem.EnumItem) ==
                        AllLookupValues.V_PrescriptionNoteTempType.AppointmentPCLTemplate
                        || (AllLookupValues.V_PrescriptionNoteTempType?)(UCPreNoteTemp.SelectedItem.EnumItem) ==
                        AllLookupValues.V_PrescriptionNoteTempType.SmallProceduresSequence)
                    {
                        mLoiDan = true;
                    }
                    else { mLoiDan = false; }
                    return UCPreNoteTemp.SelectedItem.EnumItem as AllLookupValues.V_PrescriptionNoteTempType?;                    
                }
                else 
                {
                    return null;
                }                
            }
            set
            {                
                NotifyOfPropertyChange(() => PrescriptionNoteTempType);
                if(UCPreNoteTemp!=null)
                {
                    if (value.HasValue)
                    {
                        UCPreNoteTemp.SetSelectedID(value.ToString());
                        PrescriptionNoteTemplates_GetAllIsActive(PrescriptionNoteTempType);
                    }
                    else 
                    {
                        UCPreNoteTemp.SetSelectedID(String.Empty);
                    }
                }
            }
        }

        private ObservableCollection<PrescriptionNoteTemplates> _allPrescriptionNoteTemplates;
        public ObservableCollection<PrescriptionNoteTemplates> allPrescriptionNoteTemplates
        {
            get
            {
                return _allPrescriptionNoteTemplates;
            }
            set
            {
                if (_allPrescriptionNoteTemplates == value)
                    return;
                _allPrescriptionNoteTemplates = value;
                NotifyOfPropertyChange(() => allPrescriptionNoteTemplates);
            }
        }

        private PrescriptionNoteTemplates _selectedPrescriptionNoteTemplates;
        public PrescriptionNoteTemplates selectedPrescriptionNoteTemplates
        {
            get
            {
                return _selectedPrescriptionNoteTemplates;
            }
            set
            {
                _selectedPrescriptionNoteTemplates = value;
                NotifyOfPropertyChange(() => selectedPrescriptionNoteTemplates);
                
            }
        }

        private PrescriptionNoteTemplates _curPrescriptionNoteTemplates;
        public PrescriptionNoteTemplates curPrescriptionNoteTemplates
        {
            get
            {
                return _curPrescriptionNoteTemplates;
            }
            set
            {
                _curPrescriptionNoteTemplates = value;
                NotifyOfPropertyChange(() => curPrescriptionNoteTemplates);

            }
        }

        private ObservableCollection<string> _ExclusionValues;
        public ObservableCollection<string> ExclusionValues
        {
            get { return _ExclusionValues; }
            set
            {
                if(_ExclusionValues!=value)
                {
                    _ExclusionValues = value;
                    UCPreNoteTemp.ExclusionValues = value;
                    UCPreNoteTemp.LoadData();
                }
                NotifyOfPropertyChange(() => ExclusionValues);
            }
        }

        public void butExit()
        {
            TryClose();
            Globals.EventAggregator.Publish(new PrescriptionNoteTempType_Change());
        }
    
        public void butSave()
        {
            if (curPrescriptionNoteTemplates.NoteDetails==null
                || curPrescriptionNoteTemplates.NoteDetails == "")
            {
                Globals.ShowMessage(eHCMSResources.Z1251_G1_ChuaNHapNDung,"");
                return;
            }
            //▼====: #001
            if (PrescriptionNoteTempType.Value == AllLookupValues.V_PrescriptionNoteTempType.PrescriptionNoteGen
                && (curPrescriptionNoteTemplates.DetailsTemplate.Contains("<") 
                || curPrescriptionNoteTemplates.DetailsTemplate.Contains(">")))
            {
                Globals.ShowMessage("Nội dung lời dặn không được sử dụng ký tự '<', '>'. Vui lòng kiểm tra lại!", eHCMSResources.G0442_G1_TBao);
                return;
            }
            //▲====: #001
            curPrescriptionNoteTemplates.V_PrescriptionNoteTempType = PrescriptionNoteTempType.Value;
            curPrescriptionNoteTemplates.Staff = Globals.LoggedUserAccount.Staff;
            AddPatientPrescriptionNoteTemplates(curPrescriptionNoteTemplates);
        }

        public void PrescriptionNoteTemplates_GetAllIsActive(AllLookupValues.V_PrescriptionNoteTempType? V_PrescriptionNoteTempType)
        {
            var t = new Thread(() =>
            {
                IsLoading = true;

                try
                {
                    using (var serviceFactory = new ePrescriptionsServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        var pnt = new PrescriptionNoteTemplates();
                        pnt.V_PrescriptionNoteTempType = (AllLookupValues.V_PrescriptionNoteTempType)V_PrescriptionNoteTempType;
                        contract.BeginPrescriptionNoteTemplates_GetAllIsActive(pnt, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var allItems = contract.EndPrescriptionNoteTemplates_GetAllIsActive(asyncResult);
                                allPrescriptionNoteTemplates = new ObservableCollection<PrescriptionNoteTemplates>(allItems);
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
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                }
                finally
                {
                    IsLoading = false;
                }
            });
            t.Start();
        }
        public void AddPatientPrescriptionNoteTemplates(PrescriptionNoteTemplates obj)
        {
            IsLoading = true;
            var t = new Thread(() =>
            {
                Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.K2871_G1_DangLoadDLieu });
                try
                {
                    using (var serviceFactory = new ePrescriptionsServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        string result = "";

                        contract.BeginPrescriptionNoteTemplates_Save(obj, Globals.DispatchCallback((asyncResult) =>
                        {
                            contract.EndPrescriptionNoteTemplates_Save(out result,asyncResult);
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                }
                finally
                {
                    IsLoading = false;
                    Globals.IsBusy = false;
                    curPrescriptionNoteTemplates = new PrescriptionNoteTemplates();
                    PrescriptionNoteTemplates_GetAllIsActive(PrescriptionNoteTempType);
                }
            });
            t.Start();
        }
        
        public Button lnkDelete { get; set; }
        public void lnkDelete_Loaded(object sender)
        {
            lnkDelete = sender as Button;            
        }
        public void lnkDeleteClick()
        {
            if (MessageBox.Show(string.Format("{0}!", eHCMSResources.Z0554_G1_CoChacMuonXoaDongNayKg), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
            {
                return;
            }
            else 
            {
                selectedPrescriptionNoteTemplates.IsActive=false;
                AddPatientPrescriptionNoteTemplates(selectedPrescriptionNoteTemplates);
            }
        }
    }
}
