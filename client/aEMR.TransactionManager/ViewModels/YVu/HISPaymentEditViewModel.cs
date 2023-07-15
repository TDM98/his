using eHCMSLanguage;
using System.ComponentModel.Composition;
using Caliburn.Micro;
using aEMR.ViewContracts;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.Windows;
using DataEntities;
using System.Threading;
using aEMR.ServiceClient;
using aEMR.Infrastructure;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using System;
using System.Linq;
using aEMR.Common.Collections;

namespace aEMR.TransactionManager.ViewModels
{
    [Export(typeof(IHISPaymentEdit)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class HISPaymentEditViewModel : Conductor<object>, IHISPaymentEdit
    {
        #region Constructor
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public HISPaymentEditViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;
        }
        protected override void OnActivate()
        {
            base.OnActivate();
            LoadLookupValues();
            MonthCollection = new ObservableCollection<Lookup>();
            for (int i = 1; i <= 12; i++)
            {
                MonthCollection.Add(new Lookup { LookupID = i, ObjectValue = string.Format("Tháng {0}", i) });
            }
            YearCollection = new ObservableCollection<Lookup>();
            for (int i = DateTime.Now.Year; i >= DateTime.Now.Year - 3; i--)
            {
                YearCollection.Add(new Lookup { LookupID = i, ObjectValue = string.Format("Năm {0}", i) });
            }
            gPayYear = YearCollection.FirstOrDefault();
            if (gHOSPayment == null || gHOSPayment.HOSPaymentID == 0)
                gHOSPayment = new HOSPayment { PaymentDate = DateTime.Now, TransactionDate = DateTime.Now };
            else if (gHOSPayment != null)
            {
                if (gHOSPayment.V_PayReson == 71069)
                    IsCharitySupport = true;
                else
                    IsCharitySupport = false;
            }
            gPayMonth = MonthCollection.Where(x => x.LookupID == gHOSPayment.TransactionDate.Month).FirstOrDefault();
            gPayYear = YearCollection.Where(x => x.LookupID == gHOSPayment.TransactionDate.Year).FirstOrDefault();
            ChangeButtonText();
        }
        #endregion
        #region Properties
        private bool _IsLoading;
        public bool IsLoading
        {
            get
            {
                return _IsLoading;
            }
            set
            {
                _IsLoading = value;
                NotifyOfPropertyChange("IsLoading");
            }
        }
        private ObservableCollection<Lookup> _PayResonsCollection;
        public ObservableCollection<Lookup> PayResonsCollection
        {
            get
            {
                return _PayResonsCollection;
            }
            set
            {
                _PayResonsCollection = value;
                NotifyOfPropertyChange("PayResonsCollection");
            }
        }
        private ObservableCollection<Lookup> _MonthCollection;
        public ObservableCollection<Lookup> MonthCollection
        {
            get
            {
                return _MonthCollection;
            }
            set
            {
                _MonthCollection = value;
                NotifyOfPropertyChange("MonthCollection");
            }
        }
        private ObservableCollection<Lookup> _YearCollection;
        public ObservableCollection<Lookup> YearCollection
        {
            get
            {
                return _YearCollection;
            }
            set
            {
                _YearCollection = value;
                NotifyOfPropertyChange("YearCollection");
            }
        }
        private Lookup _SelectedPayReson;
        public Lookup SelectedPayReson
        {
            get
            {
                return _SelectedPayReson;
            }
            set
            {
                _SelectedPayReson = value;
                NotifyOfPropertyChange("SelectedPayReson");
            }
        }
        private HOSPayment _HOSPayment;
        public HOSPayment gHOSPayment
        {
            get
            {
                return _HOSPayment;
            }
            set
            {
                _HOSPayment = value;
                NotifyOfPropertyChange("gHOSPayment");
            }
        }
        private Lookup _gPayMonth;
        public Lookup gPayMonth
        {
            get
            {
                return _gPayMonth;
            }
            set
            {
                _gPayMonth = value;
                NotifyOfPropertyChange("gPayMonth");
            }
        }
        private Lookup _gPayYear;
        public Lookup gPayYear
        {
            get
            {
                return _gPayYear;
            }
            set
            {
                _gPayYear = value;
                NotifyOfPropertyChange("gPayYear");
            }
        }
        private string _ButtonText = eHCMSResources.G0156_G1_Them;
        public string ButtonText
        {
            get
            {
                return _ButtonText;
            }
            set
            {
                _ButtonText = value;
                NotifyOfPropertyChange("ButtonText");
            }
        }
        private ObservableCollection<Lookup> _gCharityObjectTypeCollection;
        public ObservableCollection<Lookup> gCharityObjectTypeCollection
        {
            get
            {
                return _gCharityObjectTypeCollection;
            }
            set
            {
                if (_gCharityObjectTypeCollection == value) return;
                _gCharityObjectTypeCollection = value;
                NotifyOfPropertyChange(() => gCharityObjectTypeCollection);
            }
        }
        private bool _IsCharitySupport = false;
        public bool IsCharitySupport
        {
            get
            {
                return _IsCharitySupport;
            }
            set
            {
                if (_IsCharitySupport == value) return;
                _IsCharitySupport = value;
                NotifyOfPropertyChange(() => IsCharitySupport);
            }
        }
        private ObservableCollection<Lookup> _gPatientSubjectCollection;
        public ObservableCollection<Lookup> gPatientSubjectCollection
        {
            get
            {
                return _gPatientSubjectCollection;
            }
            set
            {
                if (_gPatientSubjectCollection == value) return;
                _gPatientSubjectCollection = value;
                NotifyOfPropertyChange(() => gPatientSubjectCollection);
            }
        }
        private bool _gIsEnableNumbOfPatientComboBox = true;
        public bool gIsEnableNumbOfPatientComboBox
        {
            get
            {
                return _gIsEnableNumbOfPatientComboBox;
            }
            set
            {
                if (_gIsEnableNumbOfPatientComboBox == value) return;
                _gIsEnableNumbOfPatientComboBox = value;
                NotifyOfPropertyChange(() => gIsEnableNumbOfPatientComboBox);
            }
        }
        #endregion
        #region Events
        public void cboPayReson_Populating(object sender, PopulatingEventArgs e)
        {
            AutoCompleteBox cboContext = sender as AutoCompleteBox;
            e.Cancel = true;
            var AllItemsContext = new ObservableCollection<Lookup>(PayResonsCollection.Where(x => Globals.RemoveVietnameseString(x.ObjectValue).ToLower().Contains(Globals.RemoveVietnameseString(cboContext.SearchText).ToLower())).ToList());
            cboContext.ItemsSource = AllItemsContext;
            cboContext.PopulateComplete();
        }
        public void cboPayReson_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            SelectedPayReson = ((AutoCompleteBox)sender).SelectedItem as Lookup;
            if (SelectedPayReson != null && SelectedPayReson.LookupID == 71069)
                IsCharitySupport = true;
            else
                IsCharitySupport = false;
        }
        public void EditCmd()
        {
            if (gHOSPayment != null && SelectedPayReson != null && SelectedPayReson.LookupID == 71069 && (gHOSPayment.V_CharityObjectType == 0 || gHOSPayment.V_PatientSubject == 0 || gHOSPayment.NumbOfPerson == 0))
            {
                MessageBox.Show(eHCMSResources.Z2191_G1_NhapLyDoVaDoiTuong, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
            if (gHOSPayment.DOB.GetValueOrDefault(default(DateTime)) == default(DateTime) && !string.IsNullOrEmpty(gHOSPayment.PatientName))
            {
                MessageBox.Show(eHCMSResources.A0822_G1_Msg_InfoNSinhKhHopLe, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
            if (gHOSPayment != null && gHOSPayment.PaymentAmount == 0)
            {
                MessageBox.Show(eHCMSResources.Z0276_G1_HayNhapSoTien, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
            IsLoading = true;
            var t = new Thread(() =>
            {
                try
                {
                    bool mIsUpdate = gHOSPayment.HOSPaymentID > 0;
                    gHOSPayment.V_PayReson = SelectedPayReson.LookupID;
                    gHOSPayment.TransactionDate = new DateTime(Convert.ToInt32(gPayYear.LookupID), Convert.ToInt32(gPayMonth.LookupID), 1);
                    using (var serviceFactory = new TransactionServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginEditHOSPayment(gHOSPayment, Globals.DispatchCallback((asyncResult) =>
                        {
                            HOSPayment mHOSPayment = contract.EndEditHOSPayment(asyncResult);
                            if (mHOSPayment != null)
                            {
                                gHOSPayment = mHOSPayment;
                                ChangeButtonText();
                                if (!mIsUpdate)
                                    MessageBox.Show(eHCMSResources.A0079_G1_Msg_InfoThemMoiOK, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                else
                                    MessageBox.Show(eHCMSResources.A0279_G1_Msg_InfoCNhatOK, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                }
                finally
                {
                    IsLoading = false;
                }
            });
            t.Start();
        }
        public void PatientName_LostFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(gHOSPayment.PatientName))
            {
                gHOSPayment.NumbOfPerson = 1;
                gIsEnableNumbOfPatientComboBox = false;
            }
            else
            {
                gIsEnableNumbOfPatientComboBox = true;
            }
        }
        #endregion
        #region Methods
        public void ChangeButtonText()
        {
            bool mIsUpdate = gHOSPayment.HOSPaymentID > 0;
            if (!mIsUpdate)
                ButtonText = eHCMSResources.G0156_G1_Them;
            else
                ButtonText = eHCMSResources.K1599_G1_CNhat;
        }
        public void LoadLookupValues()
        {
            PayResonsCollection = Globals.AllLookupValueList.Where(x => x.ObjectTypeID == (long)LookupValues.V_PayReson).ToObservableCollection();
            if (gHOSPayment != null && gHOSPayment.V_PayReson > 0)
                SelectedPayReson = PayResonsCollection.Where(x => x.LookupID == gHOSPayment.V_PayReson).FirstOrDefault();
            gCharityObjectTypeCollection = Globals.AllLookupValueList.Where(x => x.ObjectTypeID == (long)LookupValues.V_CharityObjectType).ToObservableCollection();
            gPatientSubjectCollection = Globals.AllLookupValueList.Where(x => x.ObjectTypeID == (long)LookupValues.V_PatientSubject).ToObservableCollection();
        }
        #endregion
    }
}
