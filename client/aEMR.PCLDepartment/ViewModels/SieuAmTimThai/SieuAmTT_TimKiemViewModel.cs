using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;
using aEMR.ViewContracts;
using Caliburn.Micro;
using DataEntities;
using Castle.Windsor;
using Castle.Core.Logging;
using aEMR.Infrastructure.CachingUtils;

namespace aEMR.PCLDepartment.ViewModels
{
    [Export(typeof (ISieuAmTT_TimKiem)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class SieuAmTT_TimKiemViewModel : Conductor<object>, ISieuAmTT_TimKiem
    {
        private readonly ObservableCollection<string> LstYear;

        private ObservableCollection<string> _allYear;

        private UltraResParams_FetalEchocardioSearchCriterial _curURP_FESearchCriterial;
        private AutoCompleteBox aucboBirth;

        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public SieuAmTT_TimKiemViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            eventAggregator.Subscribe(this);
            LstYear = GetLstYear();
            allYear = new ObservableCollection<string>();
        }

        public ObservableCollection<string> allYear
        {
            get { return _allYear; }
            set
            {
                if (_allYear == value)
                    return;
                _allYear = value;
                NotifyOfPropertyChange(() => allYear);
            }
        }

        public UltraResParams_FetalEchocardioSearchCriterial curURP_FESearchCriterial
        {
            get { return _curURP_FESearchCriterial; }
            set
            {
                if (_curURP_FESearchCriterial == value)
                    return;
                _curURP_FESearchCriterial = value;
                NotifyOfPropertyChange(() => curURP_FESearchCriterial);
            }
        }


        private CheckBox fDate { get; set; }

        private CheckBox AllCheck { get; set; }
        private CheckBox fName { get; set; }
        private CheckBox fYear { get; set; }
        private CheckBox fAddress { get; set; }
        private CheckBox fPhoneNumber { get; set; }

        private ObservableCollection<string> GetLstYear()
        {
            var lstYear = new ObservableCollection<string>();
            for (int i = 1900; i <= DateTime.Now.Year; i++)
            {
                lstYear.Add(i.ToString());
            }
            return lstYear;
        }

        public void aucboBirth_Loaded(object sender, RoutedEventArgs e)
        {
            aucboBirth = sender as AutoCompleteBox;
            //aucboBirth.ItemsSource = lstYear;
        }

        public void aucboBirth_Populating(object sender, PopulatingEventArgs e)
        {
            allYear.Clear();
            string st = e.Parameter;
            foreach (string y in LstYear)
            {
                if (y.Contains(st))
                {
                    allYear.Add(y);
                }
            }
            NotifyOfPropertyChange(() => allYear);
        }

        public void AllCheck_Loaded(object sender, RoutedEventArgs e)
        {
            AllCheck = sender as CheckBox;
        }

        public void fName_Loaded(object sender, RoutedEventArgs e)
        {
            fName = sender as CheckBox;
        }

        public void fYear_Loaded(object sender, RoutedEventArgs e)
        {
            fYear = sender as CheckBox;
        }

        public void fAddress_Loaded(object sender, RoutedEventArgs e)
        {
            fAddress = sender as CheckBox;
        }

        public void fPhoneNumber_Loaded(object sender, RoutedEventArgs e)
        {
            fPhoneNumber = sender as CheckBox;
        }

        public void fDate_Loaded(object sender, RoutedEventArgs e)
        {
            fDate = sender as CheckBox;
        }

        public void AllCheck_Click(object sender, RoutedEventArgs e)
        {
            if (((CheckBox) sender).IsChecked == true)
            {
                fDate.IsChecked = true;
                fYear.IsChecked = true;
                fName.IsChecked = true;
                fPhoneNumber.IsChecked = true;
                fAddress.IsChecked = true;
            }
            else
            {
                fDate.IsChecked = false;
                fYear.IsChecked = false;
                fName.IsChecked = false;
                fPhoneNumber.IsChecked = false;
                fAddress.IsChecked = false;
            }
        }
    }
}