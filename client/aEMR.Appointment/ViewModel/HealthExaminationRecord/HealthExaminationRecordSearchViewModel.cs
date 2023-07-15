using aEMR.Common.BaseModel;
using aEMR.Infrastructure;
using aEMR.Infrastructure.CachingUtils;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using Caliburn.Micro;
using Castle.Windsor;
using DataEntities;
using eHCMSLanguage;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace aEMR.Appointment.ViewModels
{
    [Export(typeof(IHealthExaminationRecordSearch)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class HealthExaminationRecordSearchViewModel : ViewModelBase, IHealthExaminationRecordSearch
    {
        #region Properties
        private HospitalClientContract _SearchClientContract;
        private ObservableCollection<HospitalClientContract> _HospitalClientContractCollection;
        private HospitalClientContract _SelectedHospitalClientContract;
        public HospitalClientContract SearchClientContract
        {
            get
            {
                return _SearchClientContract;
            }
            set
            {
                _SearchClientContract = value;
                NotifyOfPropertyChange(() => SearchClientContract);
            }
        }

        public List<HospitalClient> HospitalClientCollection { get; set; }

        public ObservableCollection<HospitalClientContract> HospitalClientContractCollection
        {
            get
            {
                return _HospitalClientContractCollection;
            }
            set
            {
                _HospitalClientContractCollection = value;
                NotifyOfPropertyChange(() => HospitalClientContractCollection);
            }
        }

        public HospitalClientContract SelectedHospitalClientContract
        {
            get
            {
                return _SelectedHospitalClientContract;
            }
            set
            {
                _SelectedHospitalClientContract = value;
                NotifyOfPropertyChange(() => SelectedHospitalClientContract);
            }
        }

        public bool IsCompleted { get; set; }
        #endregion

        #region Events
        [ImportingConstructor]
        public HealthExaminationRecordSearchViewModel(IWindsorContainer aContainer, INavigationService aNavigation, IEventAggregator aEventAgr, ISalePosCaching aCaching) {
            DateTime mNow = Globals.GetCurServerDateTime();
            SearchClientContract = new HospitalClientContract { ValidDateFrom = mNow.AddMonths(-1), ValidDateTo = mNow };
            // 20191110 TNHX: Search default value
            btnSearch();
        }

        public void HosClient_Populating(object sender, PopulatingEventArgs e)
        {
            AutoCompleteBox cboContext = sender as AutoCompleteBox;
            e.Cancel = true;
            cboContext.ItemsSource = new ObservableCollection<HospitalClient>(HospitalClientCollection.Where(x => x.ClientName.ToLower().Contains(cboContext.SearchText.ToLower())));
            cboContext.PopulateComplete();
        }

        public void HosClient_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            SearchClientContract.HosClient = ((AutoCompleteBox)sender).SelectedItem as HospitalClient;
        }

        public void btnSearch()
        {
            this.DlgShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var mFactory = new AppointmentServiceClient())
                    {
                        var mContract = mFactory.ServiceInstance;
                        mContract.BeginGetHospitalClientContracts(SearchClientContract, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                HospitalClientContractCollection = mContract.EndGetHospitalClientContracts(asyncResult);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error);
                            }
                            finally
                            {
                                this.DlgHideBusyIndicator();
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error);
                    this.DlgHideBusyIndicator();
                }                
            });

            t.Start();
        }

        public void gvClientContracts_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            SelectCurrentItemAndClose();
        }

        public void btnSelect()
        {
            SelectCurrentItemAndClose();
        }
        #endregion

        #region Methods
        private void SelectCurrentItemAndClose()
        {
            if (SelectedHospitalClientContract == null || SelectedHospitalClientContract.HosClientContractID == 0)
            {
                return;
            }
            this.DlgShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var mFactory = new AppointmentServiceClient())
                    {
                        var mContract = mFactory.ServiceInstance;
                        mContract.BeginGetHospitalClientContractDetails(SelectedHospitalClientContract, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                SelectedHospitalClientContract = mContract.EndGetHospitalClientContractDetails(asyncResult);
                                IsCompleted = true;
                                TryClose();
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error);
                            }
                            finally
                            {
                                this.DlgHideBusyIndicator();
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error);
                    this.DlgHideBusyIndicator();
                }
            });

            t.Start();
        }
        #endregion
    }
}
