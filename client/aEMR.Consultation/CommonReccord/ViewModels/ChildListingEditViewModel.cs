using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.ServiceClient.Consultation_PCLs;
using aEMR.ViewContracts;
using Caliburn.Micro;
using Castle.Windsor;
using Castle.Core.Logging;
using aEMR.Infrastructure.CachingUtils;
using aEMR.Infrastructure;
using DataEntities;
using aEMR.Common.Collections;
using aEMR.Controls;
using eHCMSLanguage;
using System.Linq;
using aEMR.Common.BaseModel;
using aEMR.Common;
using aEMR.DataContracts;
using System.ServiceModel;
namespace aEMR.ConsultantEPrescription.CommonRecs.ViewModels
{
    [Export(typeof(IChildListingEdit)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class ChildListingEditViewModel : ViewModelBase, IChildListingEdit
    {
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
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;
        public IPatientInfo UCPatientProfileInfo { get; set; }
        [ImportingConstructor]
        public ChildListingEditViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;
      
            Globals.EventAggregator.Subscribe(this);
            authorization();
            SurgicalBirthCollection = Globals.AllLookupValueList.Where(x => x.ObjectTypeID == (long)LookupValues.V_SurgicalBirth).ToObservableCollection();
            BirthUnder32Collection = Globals.AllLookupValueList.Where(x => x.ObjectTypeID == (long)LookupValues.V_BirthUnder32).ToObservableCollection();

        }

        protected override void OnActivate()
        {
            base.OnActivate();
            ActivateItem(UCPatientProfileInfo);
        }
        public void authorization()
        {
            if (!Globals.isAccountCheck)
            {
                return;
            }

        }
        #region property

        private BirthCertificates _CurrentBirthCertificates = new BirthCertificates();
        public BirthCertificates CurrentBirthCertificates
        {
            get
            {
                return _CurrentBirthCertificates;
            }
            set
            {
                _CurrentBirthCertificates = value;
                NotifyOfPropertyChange(() => CurrentBirthCertificates);
            }
        }
        private ObservableCollection<Lookup> _SurgicalBirthCollection;
        public ObservableCollection<Lookup> SurgicalBirthCollection
        {
            get => _SurgicalBirthCollection; set
            {
                _SurgicalBirthCollection = value;
                NotifyOfPropertyChange(() => SurgicalBirthCollection);
            }
        }
        private ObservableCollection<Lookup> _BirthUnder32Collection;
        public ObservableCollection<Lookup> BirthUnder32Collection
        {
            get => _BirthUnder32Collection; set
            {
                _BirthUnder32Collection = value;
                NotifyOfPropertyChange(() => BirthUnder32Collection);
            }
        }
        #endregion

        public void btnSaveCL()
        {
            if (CurrentBirthCertificates.V_SurgicalBirth == 0)
            {
                MessageBox.Show("Chưa chọn sinh con phẫu thuật");
                return;
            }
            if (CurrentBirthCertificates.V_BirthUnder32 == 0)
            {
                MessageBox.Show("Chưa chọn sinh con dưới 32 tuần tuổi");
                return;
            }
            if (string.IsNullOrWhiteSpace(CurrentBirthCertificates.Note))
            {
                MessageBox.Show("Chưa nhập ghi chú");
                return;
            }
            if (string.IsNullOrWhiteSpace(CurrentBirthCertificates.ChildStatus))
            {
                MessageBox.Show("Chưa nhập tình trạng trẻ");
                return;
            }
            if (CurrentBirthCertificates.BirthCount < 1 || CurrentBirthCertificates.BirthCount > 15)
            {
                MessageBox.Show("Số lần sinh con phải nhập trong khoảng từ 1->15");
                return;
            }
            if (CurrentBirthCertificates.ChildCount < 1 || CurrentBirthCertificates.ChildCount > 30)
            {
                MessageBox.Show("Số con đang sống phải nhập trong khoảng từ 1->30");
                return;
            }
            SaveBirthCertificates(false);
        }
      

        public void btnCloseCL()
        {
            this.TryClose();
        }

        #region method
        private void SaveBirthCertificates(bool IsDelete)
        {
            //if (String.IsNullOrEmpty(Registration_DataStorage.CurrentPatient.IDNumber))
            //{
            //    MessageBox.Show("Bệnh nhân không có giấy chứng minh");
            //    return;
            //}
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PatientRegistrationServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginSaveBirthCertificates(CurrentBirthCertificates, (long)Globals.LoggedUserAccount.StaffID, Globals.GetCurServerDateTime().ToString("ddMMyyyy"), Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            bool result = contract.EndSaveBirthCertificates(asyncResult);
                            if (result)
                            {
                                if (IsDelete)
                                {
                                    MessageBox.Show(eHCMSResources.K0537_G1_XoaOk);
                                }
                                else
                                {
                                    MessageBox.Show(eHCMSResources.A0468_G1_Msg_InfoLuuOK);
                                }
                            }
                            this.HideBusyIndicator();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                            this.HideBusyIndicator();
                        }

                    }), null);
                }
            });
            t.Start();
        }
        #endregion
     
      
        private IRegistration_DataStorage _Registration_DataStorage;
        public IRegistration_DataStorage Registration_DataStorage
        {
            get
            {
                return _Registration_DataStorage;
            }
            set
            {
                if (_Registration_DataStorage == value)
                {
                    return;
                }
                _Registration_DataStorage = value;
                NotifyOfPropertyChange(() => Registration_DataStorage);
            }
        }

        public void cbo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CurrentBirthCertificates.V_SurgicalBirth > 0 && CurrentBirthCertificates.V_BirthUnder32 > 0)
            {
                if(CurrentBirthCertificates.V_SurgicalBirth == (long)AllLookupValues.V_SurgicalBirth.Sinh_Con_PT
                    && CurrentBirthCertificates.V_BirthUnder32 == (long)AllLookupValues.V_BirthUnder32.Sinh_Con_Duoi_32_Tuan)
                {
                    CurrentBirthCertificates.Note = "Phẫu thuật, sinh con dưới 32 tuần tuổi";
                }
                else if (CurrentBirthCertificates.V_SurgicalBirth == (long)AllLookupValues.V_SurgicalBirth.Sinh_Con_PT
                    && CurrentBirthCertificates.V_BirthUnder32 == (long)AllLookupValues.V_BirthUnder32.Sinh_Con_Khong_Duoi_32_Tuan)
                {
                    CurrentBirthCertificates.Note = "Sinh con phải phẫu thuật";
                }
                else if (CurrentBirthCertificates.V_SurgicalBirth == (long)AllLookupValues.V_SurgicalBirth.Sinh_Con_Khong_PT
                    && CurrentBirthCertificates.V_BirthUnder32 == (long)AllLookupValues.V_BirthUnder32.Sinh_Con_Duoi_32_Tuan)
                {
                    CurrentBirthCertificates.Note = "Sinh con dưới 32 tuần tuổi";
                }
                else if (CurrentBirthCertificates.V_SurgicalBirth == (long)AllLookupValues.V_SurgicalBirth.Sinh_Con_Khong_PT
                    && CurrentBirthCertificates.V_BirthUnder32 == (long)AllLookupValues.V_BirthUnder32.Sinh_Con_Khong_Duoi_32_Tuan)
                {
                    CurrentBirthCertificates.Note = "Sinh thường";
                }
            }
        }
    }
}