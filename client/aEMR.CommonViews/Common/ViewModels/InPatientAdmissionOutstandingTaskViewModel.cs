using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ServiceModel;
using System.Threading;
using aEMR.DataContracts;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Caliburn.Micro;
using DataEntities;
using System.Windows;
using System.Collections.ObjectModel;
using System.Windows.Input;
using aEMR.Controls;
using eHCMSLanguage;
using aEMR.Common.BaseModel;
using System.Windows.Controls;
namespace aEMR.Common.ViewModels
{
    [Export(typeof(IInPatientAdmissionOutstandingTask)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class InPatientAdmissionOutstandingTaskViewModel : ViewModelBase, IInPatientAdmissionOutstandingTask
    {
        [ImportingConstructor]
        public InPatientAdmissionOutstandingTaskViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAgr, ISalePosCaching salePosCaching)
        {
            Globals.PatientFindBy_ForConsultation = AllLookupValues.PatientFindBy.NOITRU;
        }
        protected override void OnActivate()
        {
            base.OnActivate();
            Globals.EventAggregator.Subscribe(this);
        }
        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);
            Globals.EventAggregator.Unsubscribe(this);
        }

        private ObservableCollection<PatientRegistrationDetail> _InPatientList;
        public ObservableCollection<PatientRegistrationDetail> InPatientList
        {
            get { return _InPatientList; }
            private set
            {
                _InPatientList = value;
                if (_InPatientList != null || _InPatientList.Count > 0)
                {
                    IsEnableSearch = true;
                }
                else
                {
                    IsEnableSearch = false;
                }
                NotifyOfPropertyChange(() => InPatientList);
            }
        }
      
        private PatientRegistrationDetail _SelectedItem ;
        public PatientRegistrationDetail SelectedItem
        {
            get { return _SelectedItem; }
            set
            {
                if (_SelectedItem != value)
                {
                    _SelectedItem = value;
                }
                NotifyOfPropertyChange(() => SelectedItem);
            }
        }
        private DateTime? _FromDate = Globals.GetCurServerDateTime();

        public DateTime? FromDate
        {
            get { return _FromDate; }
            set
            {
                _FromDate = value;
                NotifyOfPropertyChange(() => FromDate);
            }
        }
        private DateTime? _ToDate = Globals.GetCurServerDateTime();
        public DateTime? ToDate
        {
            get { return _ToDate; }
            set
            {
                _ToDate = value;
                NotifyOfPropertyChange(() => ToDate);
            }
        }
        public void SearchInPatientRegistrationListForOST()
        {
            this.ShowBusyIndicator(eHCMSResources.K2871_G1_DangLoadDLieu);
            var t = new Thread(() =>
            {
                AxErrorEventArgs error = null;
                try
                {
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginSearchInPatientRequestAdmissionListForOST(FromDate, ToDate, Globals.DispatchCallback((asyncResult) =>
                        {
                            IList<PatientRegistrationDetail> allItems = null;
                            InPatientList = new ObservableCollection<PatientRegistrationDetail>();
                            OutPatientList = new ObservableCollection<PatientRegistrationDetail>();
                            try
                            {
                                allItems = client.EndSearchInPatientRequestAdmissionListForOST(asyncResult);
                            }
                            catch (FaultException<AxException> fault)
                            {
                                error = new AxErrorEventArgs(fault);
                                this.HideBusyIndicator();
                            }
                            catch (Exception ex)
                            {
                                error = new AxErrorEventArgs(ex);
                                this.HideBusyIndicator();
                            }
                            if (allItems != null && allItems.Count > 0)
                            {
                                foreach (var tmpList in allItems)
                                {
                                    if (tmpList.PatientRegistration.V_RegForPatientOfType == AllLookupValues.V_RegForPatientOfType.Unknown)
                                    {
                                        InPatientList.Add(tmpList);
                                    }
                                    else if(tmpList.PatientRegistration.V_RegForPatientOfType == AllLookupValues.V_RegForPatientOfType.DKBN_VANG_LAI && tmpList.PatientRegistration.DeptID == (long)AllLookupValues.DeptID.CAP_CUU)
                                    {
                                        OutPatientList.Add(tmpList);
                                    }
                                }
                            }
                            tmpInPatientList = InPatientList.DeepCopy();
                            tmpOutPatientList = OutPatientList.DeepCopy();
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    error = new AxErrorEventArgs(ex);
                }
                finally
                {
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }
        public void DoubleClick(object args)
        {
            EventArgs<object> eventArgs = args as EventArgs<object>;
            if (SelectedItem != null && SelectedItem.PatientRegistration != null
                 && SelectedItem.PatientRegistration.PatientID > 0 && SelectedItem.PatientRegistration.Patient != null
                 && eventArgs != null && eventArgs.Value != null && (eventArgs.Value as PatientRegistrationDetail).PatientRegistration != null
                 && (eventArgs.Value as PatientRegistrationDetail).PatientRegistration.PatientID > 0 && (eventArgs.Value as PatientRegistrationDetail).PatientRegistration.Patient != null)
            {
                if (!EqualsPatient(SelectedItem, (eventArgs.Value as PatientRegistrationDetail)))
                {
                    var str = string.Format("{0} '{1}'.\n{2} '{3}'", eHCMSResources.Z0175_G1_BanDangThaoTacBN, SelectedItem.PatientRegistration.Patient.FullName, eHCMSResources.Z0191_G1_BanCoMuonChSangDKBN, (eventArgs.Value as PatientRegistrationDetail).PatientRegistration.Patient.FullName);

                    if (MessageBox.Show(str, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                    {
                        return;
                    }
                }
            }
            SelectedItem = eventArgs.Value as PatientRegistrationDetail;

            if (SelectedItem != null)
            {
                Globals.EventAggregator.Publish(new InPatientSelectedForInPatientAdmission() { Source = SelectedItem.PatientRegistration });
            }
        }
        private DateTime DateTimeToCompare = DateTime.MinValue;
        public void hplRefresh()
        {
            if (FromDate == null || ToDate == null)
            {
                MessageBox.Show(eHCMSResources.A0371_G1_Msg_InfoChonTu_DenNg);
                return;
            }
            if (FromDate > ToDate)
            {
                MessageBox.Show(eHCMSResources.A0850_G1_Msg_InfoNgKhHopLe2);
                return;
            }
            SearchInPatientRegistrationListForOST();
        }
        private bool _IsEnableSearch = false;
        public bool IsEnableSearch
        {
            get { return _IsEnableSearch; }
            set
            {
                _IsEnableSearch = value;
                NotifyOfPropertyChange(() => IsEnableSearch);
            }
        }
        private ObservableCollection<PatientRegistrationDetail> _tmpInPatientList;
        public ObservableCollection<PatientRegistrationDetail> tmpInPatientList
        {
            get { return _tmpInPatientList; }
            private set
            {
                _tmpInPatientList = value;
                NotifyOfPropertyChange(() => tmpInPatientList);
            }
        }
        public void hplSearch()
        {
            if (string.IsNullOrEmpty(txtSearchName.Text))
            {
                if (InPatientList.Count < tmpInPatientList.Count)
                {
                    InPatientList = tmpInPatientList;
                }
                txtSearchName.Clear();
                return;
            }
            if (TCRegistrationInfo == null)
            {
                return;
            }
            if (TCRegistrationInfo.SelectedIndex == 0)
            {
                if (InPatientList == null)
                {
                    return;
                }
                else
                {
                    string tmpStrEqual = Globals.RemoveVietnameseString(txtSearchName.Text);
                    if (InPatientList.Count < tmpInPatientList.Count)
                    {
                        InPatientList = tmpInPatientList;
                    }
                    ObservableCollection<PatientRegistrationDetail> tmpListPatient = new ObservableCollection<PatientRegistrationDetail>();
                    foreach (var tmpPatient in InPatientList)
                    {
                        string tmpName = Globals.RemoveVietnameseString(tmpPatient.PatientRegistration.Patient.FullName.ToUpper());
                        if (tmpName.ToUpper().Contains(tmpStrEqual.ToUpper()))
                        {
                            tmpListPatient.Add(tmpPatient);
                        }
                    }
                    InPatientList = tmpListPatient;
                }
            }
            else
            {
                if (OutPatientList == null)
                {
                    return;
                }
                else
                {
                    string tmpStrEqualOutPatient = Globals.RemoveVietnameseString(txtSearchName.Text);
                    if (OutPatientList.Count < tmpOutPatientList.Count)
                    {
                        OutPatientList = tmpOutPatientList;
                    }
                    ObservableCollection<PatientRegistrationDetail> tmpListPatientOutPatient = new ObservableCollection<PatientRegistrationDetail>();
                    foreach (var tmpPatient in OutPatientList)
                    {
                        string tmpName = Globals.RemoveVietnameseString(tmpPatient.PatientRegistration.Patient.FullName.ToUpper());
                        if (tmpName.ToUpper().Contains(tmpStrEqualOutPatient.ToUpper()))
                        {
                            tmpListPatientOutPatient.Add(tmpPatient);
                        }
                    }
                    OutPatientList = tmpListPatientOutPatient;
                }
            }
        }

        AxTextBox txtSearchName = null;
        public void txtSearchName_Loaded(object sender, RoutedEventArgs e)
        {
            txtSearchName = sender as AxTextBox;
        }

        public void txtSearchName_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                hplSearch();
            }
        }
        public bool EqualsPatient(PatientRegistrationDetail PatientRegistrationDetail_1st, PatientRegistrationDetail PatientRegistrationDetail_2nd)
        {
            if (PatientRegistrationDetail_1st.PatientRegistration.PatientID != PatientRegistrationDetail_2nd.PatientRegistration.PatientID)
            {
                return false;
            }
            return true;
        }
        #region Danh sách bệnh nhân vãng lai cấp cứu chờ nhập viện.
        private ObservableCollection<PatientRegistrationDetail> _tmpOutPatientList;
        public ObservableCollection<PatientRegistrationDetail> tmpOutPatientList
        {
            get { return _tmpOutPatientList; }
            private set
            {
                _tmpOutPatientList = value;
                NotifyOfPropertyChange(() => tmpOutPatientList);
            }
        }
        private ObservableCollection<PatientRegistrationDetail> _OutPatientList;
        public ObservableCollection<PatientRegistrationDetail> OutPatientList
        {
            get { return _OutPatientList; }
            private set
            {
                _OutPatientList = value;
                if (_OutPatientList != null || _OutPatientList.Count > 0)
                {
                    IsEnableSearch = true;
                }
                else
                {
                    IsEnableSearch = false;
                }
                NotifyOfPropertyChange(() => OutPatientList);
            }
        }
        public void DoubleClickForOutPatientList(object args)
        {
            EventArgs<object> eventArgs = args as EventArgs<object>;
            if (SelectedItem != null && SelectedItem.PatientRegistration != null
                 && SelectedItem.PatientRegistration.PatientID > 0 && SelectedItem.PatientRegistration.Patient != null
                 && eventArgs != null && eventArgs.Value != null && (eventArgs.Value as PatientRegistrationDetail).PatientRegistration != null
                 && (eventArgs.Value as PatientRegistrationDetail).PatientRegistration.PatientID > 0 && (eventArgs.Value as PatientRegistrationDetail).PatientRegistration.Patient != null)
            {
                if (!EqualsPatient(SelectedItem, (eventArgs.Value as PatientRegistrationDetail)))
                {
                    var str = string.Format("{0} '{1}'.\n{2} '{3}'", eHCMSResources.Z0175_G1_BanDangThaoTacBN, SelectedItem.PatientRegistration.Patient.FullName, eHCMSResources.Z0191_G1_BanCoMuonChSangDKBN, (eventArgs.Value as PatientRegistrationDetail).PatientRegistration.Patient.FullName);

                    if (MessageBox.Show(str, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                    {
                        return;
                    }
                }
            }
            SelectedItem = eventArgs.Value as PatientRegistrationDetail;

            if (SelectedItem != null)
            {
                Globals.EventAggregator.Publish(new InPatientSelectedForInPatientAdmission() { Source = SelectedItem.PatientRegistration });
            }
        }
        TabControl TCRegistrationInfo { get; set; }
        public void TCRegistrationInfo_Loaded(object sender, RoutedEventArgs e)
        {
            TCRegistrationInfo = sender as TabControl;
        }
        #endregion
    }
}