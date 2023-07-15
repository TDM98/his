using eHCMSLanguage;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Windows.Data;
using aEMR.CommonTasks;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ViewContracts;
using aEMR.Common.PagedCollectionView;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using Caliburn.Micro;
using DataEntities;
using aEMR.Common.Collections;

namespace aEMR.Common.ViewModels
{
    [Export(typeof(IRegistrationFullDetailsView)),PartCreationPolicy(CreationPolicy.NonShared)]
    public class RegistrationFullDetailsViewModel : Conductor<object>, IRegistrationFullDetailsView
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;
        [ImportingConstructor]
        public RegistrationFullDetailsViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            var serviceVm = Globals.GetViewModel<IOutPatientServiceManage>();
            ServiceContent = serviceVm;

            var pclVm = Globals.GetViewModel<IOutPatientPclRequestManage>();
            PclContent = pclVm;

            var drugVm = Globals.GetViewModel<IOutPatientDrugManage>();
            DrugContent = drugVm;

            var paymentVm = Globals.GetViewModel<IPatientPayment>();
            PaymentContent = paymentVm;
            
         
            ShowCheckBoxColumn = false;
        }
        
        #region PROPERTIES

        private IOutPatientDrugManage _drugContent;
        public IOutPatientDrugManage DrugContent
        {
            get { return _drugContent; }
            set { _drugContent = value; }
        }


        private IOutPatientPclRequestManage _pclContent;
        public IOutPatientPclRequestManage PclContent
        {
            get { return _pclContent; }
            set { _pclContent = value; }
        }

        private IOutPatientServiceManage _serviceContent;
        public IOutPatientServiceManage ServiceContent
        {
            get { return _serviceContent; }
            set { _serviceContent = value; }
        }


        private IPatientPayment _paymentContent;
        public IPatientPayment PaymentContent
        {
            get { return _paymentContent; }
            set { _paymentContent = value; }
        }
        
        private PatientRegistration _currentRegistration;

        public PatientRegistration CurrentRegistration
        {
            get { return _currentRegistration; }
            set
            {
                if (_currentRegistration != value)
                {
                    _currentRegistration = value;
                    NotifyOfPropertyChange(() => CurrentRegistration);
                    InitRegistration();
                }
            }
        }

        private bool _showButtonList = true;
        public bool ShowButtonList
        {
            get { return _showButtonList; }
            set
            {
                _showButtonList = value;
                NotifyOfPropertyChange(()=>ShowButtonList);
            }
        }

        private int _FindPatient ;
        public int FindPatient
        {
            get { return _FindPatient; }
            set
            {
                _FindPatient = value;
                NotifyOfPropertyChange(() => FindPatient);
            }
        }

        private bool _showCheckBoxColumn;
        public bool ShowCheckBoxColumn
        {
            get { return _showCheckBoxColumn; }
            set
            {
                _showCheckBoxColumn = value;
                NotifyOfPropertyChange(()=>ShowCheckBoxColumn);

                PclContent.ShowCheckBoxColumn = _showCheckBoxColumn;
                ServiceContent.ShowCheckBoxColumn = _showCheckBoxColumn;
            }
        }

        private bool _registrationLoading;

        public bool RegistrationLoading
        {
            get { return _registrationLoading; }
            set
            {
                _registrationLoading = value;
                NotifyOfPropertyChange(() => RegistrationLoading);
            }
        }
        
        private bool _hiServiceBeingUsed;
        public bool HiServiceBeingUsed
        {
            get { return _hiServiceBeingUsed; }
            set
            {
                if (_hiServiceBeingUsed != value)
                {
                    _hiServiceBeingUsed = value;
                    NotifyOfPropertyChange(() => HiServiceBeingUsed);

                    ServiceContent.HiServiceBeingUsed = value;

                    DrugContent.HiServiceBeingUsed = value;

                    PclContent.HiServiceBeingUsed = value;
                }
            }
        }

        #endregion
        private void InitRegistration()
        {
            if (CurrentRegistration != null)
            {
                InitViewForServiceItems();
                InitViewForPCLRequests();
                InitViewForDrugItems();
                InitViewForPayments();
            }
        }

        /// <summary>
        /// Gọi hàm này khi thông tin về danh sách Cận Lâm Sàng bị thay đổi
        /// </summary>
        public void RefreshPCLRequestDetailsView()
        {
            InitViewForPCLRequests();
        }

        /// <summary>
        /// Gọi hàm này khi thông tin về danh sách Dịch vụ bị thay đổi
        /// </summary>
        public void RefreshServicesView()
        {
            InitViewForServiceItems();
        }
        
        private void InitViewForServiceItems()
        {
            var newServiceList = new List<PatientRegistrationDetail>();

            if (CurrentRegistration != null && CurrentRegistration.PatientRegistrationDetails != null)
            {
                foreach (var item in CurrentRegistration.PatientRegistrationDetails)
                {
                    if (item.RecordState != RecordState.DELETED)
                    {
                        newServiceList.Add(item);
                    }
                }
            }

            //ServiceContent.RegistrationDetails = new aEMR.Common.PagedCollectionView.PagedCollectionView(newServiceList);
            //ServiceContent.RegistrationDetails.GroupDescriptions.Add(new  aEMR.Common.PagedCollectionView.PropertyGroupDescription("RefMedicalServiceItem.RefMedicalServiceType"));

            if (ServiceContent.RegistrationDetails == null)
            {
                ServiceContent.RegistrationDetails = new ObservableCollection<PatientRegistrationDetail>();
            }
            else
            {
                ServiceContent.RegistrationDetails.Clear();
            }
            newServiceList.ForEach((regDetItem) => { ServiceContent.RegistrationDetails.Add(regDetItem); });
        }
        
        private void InitViewForPayments()
        {
            List<PatientTransactionPayment> patientPayments = new List<PatientTransactionPayment>();

            if (CurrentRegistration != null && CurrentRegistration.PatientTransaction != null)
            {
                if (CurrentRegistration.PatientTransaction.PatientTransactionPayments != null)
                {
                     foreach (var item in CurrentRegistration.PatientTransaction.PatientTransactionPayments)
                     {
                         patientPayments.Add(item);
                     }
                }
            }

            PaymentContent.PatientPayments = patientPayments.ToObservableCollection();
        }

        private void InitViewForPCLRequests()
        {
            var newList = new ObservableCollection<PatientPCLRequest>();

            if (CurrentRegistration != null && CurrentRegistration.PCLRequests != null)
            {
                var requestDetails = new List<PatientPCLRequestDetail>();
                foreach (PatientPCLRequest request in CurrentRegistration.PCLRequests)
                {
                    if (request.RecordState != RecordState.DELETED && request.PatientPCLRequestIndicators != null)
                    {
                        newList.Add(request);
                    }
                }
            }
            PclContent.PCLRequests = newList;
        }

        private void InitViewForDrugItems()
        {
            var newDrugList = new List<OutwardDrug>();

            if (CurrentRegistration.DrugInvoices != null)
            {
                foreach (var inv in CurrentRegistration.DrugInvoices)
                {
                    if (inv.RecordState != RecordState.DELETED && inv.OutwardDrugs != null)
                    {
                        newDrugList.AddRange(inv.OutwardDrugs);
                    }
                }
            }

            DrugContent.DrugItems = new aEMR.Common.PagedCollectionView.PagedCollectionView(newDrugList);
            DrugContent.DrugItems.GroupDescriptions.Add(new aEMR.Common.PagedCollectionView.PropertyGroupDescription("OutwardDrugInvoice"));
        }


        public void SetRegistration(PatientRegistration registrationInfo)
        {
            CurrentRegistration = registrationInfo;
        }

       public void CloseCmd()
       {
           TryClose();
       }
        
       public void LoadRegistrationById(RegistrationSummaryInfo RegistrationSummaryInfo)
        {
            RegistrationLoading = true;
            if (RegistrationSummaryInfo.V_RegistrationType == AllLookupValues.RegistrationType.NOI_TRU)
                FindPatient = 1;
            else FindPatient = 0;
            Coroutine.BeginExecute(DoOpenRegistration(RegistrationSummaryInfo.RegistrationID), null, (s, e) =>
                                                                               {
                                                                                   RegistrationLoading = false;
                                                                               });
        }
        public IEnumerator<IResult> DoOpenRegistration(long regID)
        {
            var loadRegTask = new LoadRegistrationInfoTask(regID, FindPatient);
            yield return loadRegTask;

            if (loadRegTask.Registration == null)
            {
                //Thong bao khong load duoc dang ky
                Globals.EventAggregator.Publish(new ShowMessageEvent() { Message = eHCMSResources.A0733_G1_Msg_InfoKhTimThayDK });
                Globals.EventAggregator.Publish(new ItemLoaded<PatientRegistration, long>() { Item = null, ID = regID });
            }
            else
            {
                SetRegistration(loadRegTask.Registration);
            }
        }
    }

}
