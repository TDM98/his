using aEMR.Common;
using aEMR.Common.BaseModel;
using aEMR.Common.Collections;
using aEMR.Controls;
using aEMR.Infrastructure;
using aEMR.Infrastructure.CachingUtils;
using aEMR.Infrastructure.Events;
using aEMR.ViewContracts;
using Caliburn.Micro;
using Castle.Windsor;
using DataEntities;
using eHCMSLanguage;
using Service.Core.Common;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Controls;

namespace aEMR.Appointment.ViewModels
{
    [Export(typeof(IHealthExaminationRecordServiceEdit)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class HealthExaminationRecordServiceEditViewModel : ViewModelBase, IHealthExaminationRecordServiceEdit
        , IHandle<ItemSelected<MedRegItemBase>>
    {
        #region Properties
        private IOutPtAddServiceAndPCL _UCOutPtAddServiceAndPCL;
        private bool _CanDelete = true;
        private PatientRegistration _CurrentRegistration = new PatientRegistration();
        private bool _AllowDuplicateMedicalServiceItems = false;
        private bool _RegistrationInfoHasChanged = false;
        private RegistrationFormMode _CurrentRegMode = RegistrationFormMode.PATIENT_NOT_SELECTED;
        private ObservableCollection<MedRegItemBase> _MedRegItemBaseCollection;
        private ObservableCollection<HosClientContractPatientGroup> _PatientGroupCollection;
        public ObservableCollection<HosClientContractPatientGroup> PatientGroupCollection
        {
            get
            {
                return _PatientGroupCollection;
            }
            set
            {
                if (_PatientGroupCollection == value)
                {
                    return;
                }
                _PatientGroupCollection = value;
                NotifyOfPropertyChange(() => PatientGroupCollection);
                NotifyOfPropertyChange(() => IsHasPatientGroup);
            }
        }
        private long _CurrentPatientGroupID;
        public long CurrentPatientGroupID
        {
            get
            {
                return _CurrentPatientGroupID;
            }
            set
            {
                if (_CurrentPatientGroupID == value)
                {
                    return;
                }
                _CurrentPatientGroupID = value;
                NotifyOfPropertyChange(() => CurrentPatientGroupID);
            }
        }
        public bool IsHasPatientGroup
        {
            get
            {
                return PatientGroupCollection != null && PatientGroupCollection.Count > 0;
            }
        }
        public IOutPtAddServiceAndPCL UCOutPtAddServiceAndPCL
        {
            get
            {
                return _UCOutPtAddServiceAndPCL;
            }
            set
            {
                _UCOutPtAddServiceAndPCL = value;
                NotifyOfPropertyChange(() => UCOutPtAddServiceAndPCL);
            }
        }
        public bool CanDelete
        {
            get
            {
                return _CanDelete;
            }
            set
            {
                _CanDelete = value;
                NotifyOfPropertyChange(() => CanDelete);
            }
        }
        public PatientRegistration CurrentRegistration
        {
            get
            {
                return _CurrentRegistration;
            }
            set
            {
                _CurrentRegistration = value;
                NotifyOfPropertyChange(() => CurrentRegistration);
            }
        }
        public bool AllowDuplicateMedicalServiceItems
        {
            get
            {
                return _AllowDuplicateMedicalServiceItems;
            }
            set
            {
                _AllowDuplicateMedicalServiceItems = value;
                NotifyOfPropertyChange(() => AllowDuplicateMedicalServiceItems);
            }
        }
        public bool RegistrationInfoHasChanged
        {
            get
            {
                return _RegistrationInfoHasChanged;
            }
            set
            {
                _RegistrationInfoHasChanged = value;
                NotifyOfPropertyChange(() => RegistrationInfoHasChanged);
            }
        }
        public RegistrationFormMode CurrentRegMode
        {
            get
            {
                return _CurrentRegMode;
            }
            set
            {
                if (_CurrentRegMode != value)
                {
                    _CurrentRegMode = value;
                    NotifyOfPropertyChange(() => CurrentRegMode);
                }
            }
        }
        public ObservableCollection<MedRegItemBase> MedRegItemBaseCollection
        {
            get
            {
                return _MedRegItemBaseCollection;
            }
            set
            {
                _MedRegItemBaseCollection = value;
                NotifyOfPropertyChange(() => MedRegItemBaseCollection);
            }
        }
        private MedRegItemBase EditingObject { get; set; }
        private bool _IsChoossingCase = false;
        public bool IsChoossingCase
        {
            get
            {
                return _IsChoossingCase;
            }
            set
            {
                if (_IsChoossingCase == value)
                {
                    return;
                }
                _IsChoossingCase = value;
                NotifyOfPropertyChange(() => IsChoossingCase);
            }
        }
        public bool IsConfirmed { get; set; } = false;
        public HospitalClientContract CurrentClientContract { get; set; }
        #endregion
        #region Events
        [ImportingConstructor]
        public HealthExaminationRecordServiceEditViewModel(IWindsorContainer aContainer, INavigationService aNavigation, IEventAggregator aEventAgr, ISalePosCaching aCaching)
        {
            UCOutPtAddServiceAndPCL = Globals.GetViewModel<IOutPtAddServiceAndPCL>();
            UCOutPtAddServiceAndPCL.OnItemAddedCallback = new OnItemAdded((aItem) =>
            {
                OnItemAdded(aItem);
            });
            ActivateItem(UCOutPtAddServiceAndPCL);
        }
        protected override void OnActivate()
        {
            base.OnActivate();
            Globals.EventAggregator.Subscribe(this);
            if (IsHasPatientGroup)
            {
                CurrentPatientGroupID = PatientGroupCollection.First().HosClientContractPatientGroupID;
            }
        }
        protected override void OnDeactivate(bool close)
        {
            Globals.EventAggregator.Unsubscribe(this);
            base.OnDeactivate(close);
        }
        public void RemoveItemCmd(MedRegItemBase aMedRegItem, object eventArgs)
        {
            if (aMedRegItem == null)
            {
                return;
            }
            if (aMedRegItem is PatientRegistrationDetail)
            {
                CurrentRegistration.PatientRegistrationDetails.Remove(CurrentRegistration.PatientRegistrationDetails.FirstOrDefault(x => x.MedServiceID == (aMedRegItem as PatientRegistrationDetail).MedServiceID));
            }
            else if (aMedRegItem is PatientPCLRequestDetail)
            {
                CurrentRegistration.PCLRequests[0].PatientPCLRequestIndicators.Remove(CurrentRegistration.PCLRequests[0].PatientPCLRequestIndicators.FirstOrDefault(x => x.PCLExamTypeID == (aMedRegItem as PatientPCLRequestDetail).PCLExamTypeID));
            }
            MedRegItemBaseCollection = CurrentRegistration.AllSavedInvoiceItem.Where(x => (x.DisplayID == 1 || x.DisplayID == 2) || !IsHasPatientGroup).ToObservableCollection();
        }
        public void btnSave()
        {
            IsConfirmed = true;
            TryClose();
        }
        public void gvMedRegItems_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            if (e.Row.DataContext == null || !(e.Row.DataContext is MedRegItemBase))
            {
                EditingObject = null;
            }
            else
            {
                EditingObject = (e.Row.DataContext as MedRegItemBase).DeepCopy();
            }
        }
        public void gvMedRegItems_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if ((sender as DataGrid).GetColumnByName("ItemPriceColumn") == e.Column &&
                (e.Row.DataContext as MedRegItemBase) != null &&
                (e.Row.DataContext as MedRegItemBase).DisplayID == 2)
            {
                if (EditingObject.InvoicePrice != (e.Row.DataContext as MedRegItemBase).InvoicePrice)
                {
                    Globals.ShowMessage(eHCMSResources.Z2937_G1_Msg, eHCMSResources.G0442_G1_TBao);
                }
            }
        }
        public void PatientGroupComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PatientGroupCollection == null)
            {
                return;
            }
            var CurrentPatientGroup = PatientGroupCollection.FirstOrDefault(x => CurrentPatientGroupID > 0 && x.HosClientContractPatientGroupID == CurrentPatientGroupID);
            var PatientCollection = CurrentClientContract.ContractPatientCollection.Where(x => x.PatientGroupCollection != null && x.PatientGroupCollection.Any(i => CurrentPatientGroup != null && i.HosClientContractPatientGroupID == CurrentPatientGroup.HosClientContractPatientGroupID)).ToList();
            var ServiceItemCollection = CurrentClientContract.ServiceItemPatientLinkCollection.Where(x => PatientCollection != null && PatientCollection.Any(i => i.PatientObj.Equals(x.ContractPatient.PatientObj))).Select(x => x.ContractMedRegItem.MedRegItem).Distinct().ToList();
            foreach (var Item in CurrentRegistration.AllSavedInvoiceItem)
            {
                if (ServiceItemCollection.Any(x => x.SameAsMedItemPrimary(Item)))
                {
                    Item.DisplayID = 1;
                }
                else
                {
                    Item.DisplayID = 0;
                }
            }
            InitCurrentRegistration(CurrentRegistration);
        }
        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            PatientGroupComboBox_SelectionChanged(null, null);
        }
        #endregion
        #region Methods
        public void InitCurrentRegistration(PatientRegistration aRegistration)
        {
            CurrentRegistration = aRegistration;
            if (CurrentRegistration == null || CurrentRegistration.AllSavedInvoiceItem == null || CurrentRegistration.AllSavedInvoiceItem.Count == 0)
            {
                return;
            }
            MedRegItemBaseCollection = CurrentRegistration.AllSavedInvoiceItem.Where(x => (x.DisplayID == 1 || x.DisplayID == 2) || !IsHasPatientGroup).ToObservableCollection();
        }
        private void OnItemAdded(MedRegItemBase message)
        {
            if (IsHasPatientGroup)
            {
                message.DisplayID = 1;
            }
            if (message is PatientRegistrationDetail
                && CurrentRegistration.PatientRegistrationDetails.Any(x => x.MedServiceID == (message as PatientRegistrationDetail).MedServiceID))
            {
                CurrentRegistration.PatientRegistrationDetails.First(x => x.MedServiceID == (message as PatientRegistrationDetail).MedServiceID).DisplayID = 2;
                MedRegItemBaseCollection = CurrentRegistration.AllSavedInvoiceItem.Where(x => (x.DisplayID == 1 || x.DisplayID == 2) || !IsHasPatientGroup).ToObservableCollection();
                return;
            }
            if (message is PatientPCLRequestDetail && CurrentRegistration.PCLRequests != null && CurrentRegistration.PCLRequests.Count > 0
                && CurrentRegistration.PCLRequests.First().PatientPCLRequestIndicators != null
                && CurrentRegistration.PCLRequests.First().PatientPCLRequestIndicators.Any(x => x.PCLExamTypeID == (message as PatientPCLRequestDetail).PCLExamTypeID))
            {
                CurrentRegistration.PCLRequests.First().PatientPCLRequestIndicators.First(x => x.PCLExamTypeID == (message as PatientPCLRequestDetail).PCLExamTypeID).DisplayID = 2;
                MedRegItemBaseCollection = CurrentRegistration.AllSavedInvoiceItem.Where(x => (x.DisplayID == 1 || x.DisplayID == 2) || !IsHasPatientGroup).ToObservableCollection();
                return;
            }
            if (message is PatientRegistrationDetail)
            {
                CurrentRegistration.PatientRegistrationDetails.Add(message as PatientRegistrationDetail);
            }
            else if (message is PatientPCLRequestDetail)
            {
                if (CurrentRegistration.PCLRequests == null)
                {
                    CurrentRegistration.PCLRequests = new ObservableCollection<PatientPCLRequest>();
                    CurrentRegistration.PCLRequests.Add(new PatientPCLRequest
                    {
                        PatientPCLRequestIndicators = new ObservableCollection<PatientPCLRequestDetail>(),
                        Diagnosis = eHCMSResources.Z1116_G1_ChuaXacDinh,
                        StaffID = Globals.LoggedUserAccount.StaffID,
                        V_PCLRequestType = AllLookupValues.V_PCLRequestType.NGOAI_TRU,
                        V_PCLRequestStatus = AllLookupValues.V_PCLRequestStatus.OPEN,
                        RecordState = RecordState.DETACHED,
                        EntityState = EntityState.DETACHED,
                        ReqFromDeptLocID = Globals.DeptLocation.DeptLocationID
                    });
                }
                else if (CurrentRegistration.PCLRequests.Count == 0)
                {
                    CurrentRegistration.PCLRequests.Add(new PatientPCLRequest());
                }
                CurrentRegistration.PCLRequests[0].PatientPCLRequestIndicators.Add(message as PatientPCLRequestDetail);
            }
            MedRegItemBaseCollection = CurrentRegistration.AllSavedInvoiceItem.Where(x => (x.DisplayID == 1 || x.DisplayID == 2) || !IsHasPatientGroup).ToObservableCollection();
        }
        #endregion
        #region Handles
        public void Handle(ItemSelected<MedRegItemBase> message)
        {
            if (message == null || GetView() == null || message.Item == null)
            {
                return;
            }
            OnItemAdded(message.Item);
            //if (message != null && this.GetView() != null && message.Item != null)
            //{
            //    if (IsHasPatientGroup)
            //    {
            //        message.Item.DisplayID = 1;
            //    }
            //    if (message.Item is PatientRegistrationDetail
            //        && CurrentRegistration.PatientRegistrationDetails.Any(x => x.MedServiceID == (message.Item as PatientRegistrationDetail).MedServiceID))
            //    {
            //        CurrentRegistration.PatientRegistrationDetails.First(x => x.MedServiceID == (message.Item as PatientRegistrationDetail).MedServiceID).DisplayID = 2;
            //        MedRegItemBaseCollection = CurrentRegistration.AllSavedInvoiceItem.Where(x => (x.DisplayID == 1 || x.DisplayID == 2) || !IsHasPatientGroup).ToObservableCollection();
            //        return;
            //    }
            //    if (message.Item is PatientPCLRequestDetail && CurrentRegistration.PCLRequests != null && CurrentRegistration.PCLRequests.Count > 0
            //        && CurrentRegistration.PCLRequests.First().PatientPCLRequestIndicators != null
            //        && CurrentRegistration.PCLRequests.First().PatientPCLRequestIndicators.Any(x => x.PCLExamTypeID == (message.Item as PatientPCLRequestDetail).PCLExamTypeID))
            //    {
            //        CurrentRegistration.PCLRequests.First().PatientPCLRequestIndicators.First(x => x.PCLExamTypeID == (message.Item as PatientPCLRequestDetail).PCLExamTypeID).DisplayID = 2;
            //        MedRegItemBaseCollection = CurrentRegistration.AllSavedInvoiceItem.Where(x => (x.DisplayID == 1 || x.DisplayID == 2) || !IsHasPatientGroup).ToObservableCollection();
            //        return;
            //    }
            //    if (message.Item is PatientRegistrationDetail)
            //    {
            //        CurrentRegistration.PatientRegistrationDetails.Add(message.Item as PatientRegistrationDetail);
            //    }
            //    else if (message.Item is PatientPCLRequestDetail)
            //    {
            //        if (CurrentRegistration.PCLRequests == null)
            //        {
            //            CurrentRegistration.PCLRequests = new ObservableCollection<PatientPCLRequest>();
            //            CurrentRegistration.PCLRequests.Add(new PatientPCLRequest
            //            {
            //                PatientPCLRequestIndicators = new ObservableCollection<PatientPCLRequestDetail>(),
            //                Diagnosis = eHCMSResources.Z1116_G1_ChuaXacDinh,
            //                StaffID = Globals.LoggedUserAccount.StaffID,
            //                V_PCLRequestType = AllLookupValues.V_PCLRequestType.NGOAI_TRU,
            //                V_PCLRequestStatus = AllLookupValues.V_PCLRequestStatus.OPEN,
            //                RecordState = RecordState.DETACHED,
            //                EntityState = EntityState.DETACHED,
            //                ReqFromDeptLocID = Globals.DeptLocation.DeptLocationID
            //            });
            //        }
            //        else if (CurrentRegistration.PCLRequests.Count == 0)
            //        {
            //            CurrentRegistration.PCLRequests.Add(new PatientPCLRequest());
            //        }
            //        CurrentRegistration.PCLRequests[0].PatientPCLRequestIndicators.Add(message.Item as PatientPCLRequestDetail);
            //    }
            //    MedRegItemBaseCollection = CurrentRegistration.AllSavedInvoiceItem.Where(x => (x.DisplayID == 1 || x.DisplayID == 2) || !IsHasPatientGroup).ToObservableCollection();
            //}
        }
        #endregion
    }
}