using eHCMSLanguage;
using System.ComponentModel.Composition;
using aEMR.ViewContracts;
using Caliburn.Micro;
using System.Windows;
using System;
using aEMR.Infrastructure;
using aEMR.Infrastructure.CachingUtils;
using Castle.Core.Logging;
using Castle.Windsor;
using System.Collections.ObjectModel;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using System.Threading;
using DataEntities;
using aEMR.Common.Collections;
using aEMR.CommonTasks;
using System.Collections.Generic;
using aEMR.Common.BaseModel;
using aEMR.DataContracts;
using System.ServiceModel;
using System.Windows.Controls;
using aEMR.Controls;
using System.Linq;
using System.Text.RegularExpressions;

namespace aEMR.Registration.ViewModels
{
    [Export(typeof(IPerformSmallProcedureAuto)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class PerformSmallProcedureAutoViewModel : ViewModelBase, IPerformSmallProcedureAuto
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public PerformSmallProcedureAutoViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            GetResourcesForMedicalServices();
        }
        private ObservableCollection<RefMedicalServiceItem> _medicalServiceItems;
        public ObservableCollection<RefMedicalServiceItem> MedicalServiceItems
        {
            get { return _medicalServiceItems; }
            set
            {
                _medicalServiceItems = value;
                NotifyOfPropertyChange(() => MedicalServiceItems);
            }
        }

        private RefMedicalServiceItem _medServiceItem;
        public RefMedicalServiceItem MedServiceItem
        {
            get
            {
                return _medServiceItem;
            }
            set
            {
                if (_medServiceItem != value)
                {
                    _medServiceItem = value;
                    NotifyOfPropertyChange(() => MedServiceItem);
                }
            }
        }
        private ObservableCollection<Resources> _Resources;
        public ObservableCollection<Resources> Resources
        {
            get { return _Resources; }
            set
            {
                _Resources = value;
                NotifyOfPropertyChange(() => Resources);
            }
        }

        private Resources _ResourcesItem;
        public Resources ResourcesItem
        {
            get
            {
                return _ResourcesItem;
            }
            set
            {
                if (_ResourcesItem != value)
                {
                    _ResourcesItem = value;
                    NotifyOfPropertyChange(() => ResourcesItem);
                }
            }
        }
        private ObservableCollection<PatientRegistration> _RegistrationObj;
        public ObservableCollection<PatientRegistration> RegistrationObj
        {
            get { return _RegistrationObj; }
            set
            {
                _RegistrationObj = value;
                NotifyOfPropertyChange(() => RegistrationObj);
            }
        }
        private List<DiagnosisTreatment> _ListDiagnosis;
        public List<DiagnosisTreatment> ListDiagnosis
        {
            get { return _ListDiagnosis; }
            set
            {
                _ListDiagnosis = value;
                NotifyOfPropertyChange(() => ListDiagnosis);
            }
        }
        private PatientRegistration _SelectedRegistration;
        public PatientRegistration SelectedRegistration
        {
            get
            {
                return _SelectedRegistration;
            }
            set
            {
                if (_SelectedRegistration != value)
                {
                    _SelectedRegistration = value;
                    NotifyOfPropertyChange(() => SelectedRegistration);
                }
            }
        }
        private DateTime? _Fromdate = Globals.GetCurServerDateTime().Date;
        public DateTime? Fromdate
        {
            get { return _Fromdate; }
            set
            {
                if (_Fromdate != value)
                {
                    _Fromdate = value;
                    NotifyOfPropertyChange(() => Fromdate);
                }
            }
        }

        private DateTime? _ToDate = Globals.GetCurServerDateTime();
        public DateTime? ToDate
        {
            get { return _ToDate; }
            set
            {
                if (_ToDate != value)
                {
                    _ToDate = value;
                    NotifyOfPropertyChange(() => ToDate);
                }
            }
        }

        private DateTime _StartTime;
        public DateTime StartTime
        {
            get { return _StartTime; }
            set
            {
                if (_StartTime != value)
                {
                    _StartTime = value;
                    NotifyOfPropertyChange(() => StartTime);
                }
            }
        }

        private string _Minute;
        public string Minute
        {
            get { return _Minute; }
            set
            {
                if (_Minute != value)
                {
                    _Minute = value;
                    NotifyOfPropertyChange(() => Minute);
                }
            }
        }

        private string _HIRepResourceCode;
        public string HIRepResourceCode
        {
            get { return _HIRepResourceCode; }
            set
            {
                if (_HIRepResourceCode != value)
                {
                    _HIRepResourceCode = value;
                    NotifyOfPropertyChange(() => HIRepResourceCode);
                }
            }
        }
        public void btnSearch()
        {
            if (MedServiceItem == null)
            {
                return;
            }
            var t = new Thread(() =>
            {
                AxErrorEventArgs error = null;
                try
                {
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginSearchRegistration_ByServiceID(MedServiceItem.MedServiceID, Fromdate, ToDate, Globals.DispatchCallback((asyncResult) =>
                        {
                            IList<PatientRegistration> allItems = null;
                            List<DiagnosisTreatment> DiagnosisList = new List<DiagnosisTreatment>();
                            try
                            {
                                allItems = client.EndSearchRegistration_ByServiceID(out DiagnosisList, asyncResult);
                                RegistrationObj = allItems.ToObservableCollection();
                                ListDiagnosis = DiagnosisList;
                            }
                            catch (Exception ex)
                            {
                                error = new AxErrorEventArgs(ex);
                                _logger.Info(error.ToString());
                            }
                            finally
                            {
                                this.HideBusyIndicator();
                            }
                        }), null)
                            ;
                    }
                }
                catch (Exception ex)
                {
                    error = new AxErrorEventArgs(ex);
                    _logger.Info(error.ToString());
                    this.HideBusyIndicator();
                }
                if (error != null)
                {
                    Globals.ShowMessage(error.ToString(), eHCMSResources.T0432_G1_Error);
                }
            });
            t.Start();
        }

        private void GetResourcesForMedicalServices()
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PatientRegistrationServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetResourcesForMedicalServices(Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                IList<Resources> results = contract.EndGetResourcesForMedicalServices(asyncResult);
                                if (results != null)
                                {
                                    if (Resources == null)
                                    {
                                        Resources = new ObservableCollection<Resources>();
                                    }
                                    else
                                    {
                                        Resources.Clear();
                                    }
                                    Resources = results.ToObservableCollection();
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message);
                            }
                            finally
                            {
                                Globals.IsBusy = false;
                            }
                        }), null);
                }
            });

            t.Start();
        }
        AxComboBox Cbo;
        public void cboHiRepResourceCode_Loaded(object sender, SelectionChangedEventArgs e)
        {
            Cbo = sender as AxComboBox;
        }
        public void cboHiRepResourceCode_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Cbo.SelectedValue != null && !string.IsNullOrEmpty(Cbo.SelectedValue.ToString()))
            {
                HIRepResourceCode = Cbo.SelectedValue.ToString();
                GetMedicalServiceItemByHIRepResourceCode(Cbo.SelectedValue.ToString());
            }
        }
        public void GetMedicalServiceItemByHIRepResourceCode(string HIRepResourceCode)
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PatientRegistrationServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetMedicalServiceItemByHIRepResourceCode(HIRepResourceCode, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            IList<RefMedicalServiceItem> results = contract.EndGetMedicalServiceItemByHIRepResourceCode(asyncResult);
                            if (results != null)
                            {
                                if (MedicalServiceItems == null)
                                {
                                    MedicalServiceItems = new ObservableCollection<RefMedicalServiceItem>();
                                }
                                else
                                {
                                    MedicalServiceItems.Clear();
                                }
                                MedicalServiceItems = results.ToObservableCollection();
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                        finally
                        {
                            Globals.IsBusy = false;
                        }
                    }), null);
                }
            });

            t.Start();
        }

        public void btnPerform()
        {
            if (!CheckValidBeforePerform())
            {
                return;
            }
            DateTime TimeToStart = (DateTime)Fromdate.Value.Date;
            TimeToStart = TimeToStart.AddHours(StartTime.Hour);
            DateTime TimeToEnd = DateTime.MinValue;
            ObservableCollection<SmallProcedure> SmallProcedureList = new ObservableCollection<SmallProcedure>();
            SmallProcedure smallProcedure = new SmallProcedure();
            int ncount = 0;
            foreach (var item in RegistrationObj)
            {
                DiagnosisTreatment Diagnosis = null;
                foreach (var diagnosisdetail in ListDiagnosis)
                {
                    if (diagnosisdetail.PatientServiceRecord != null && item.PtRegistrationID == diagnosisdetail.PatientServiceRecord.PtRegistrationID)
                    {
                        Diagnosis = diagnosisdetail;
                    }
                }

                ncount++;
                foreach (var detail in item.PatientRegistrationDetails)
                {
                    smallProcedure = new SmallProcedure();
                    if (detail.PaidTime < TimeToStart && ncount == 1)
                    {
                        smallProcedure.ProcedureDateTime = TimeToStart;
                        smallProcedure.CompletedDateTime = TimeToStart.AddMinutes(Convert.ToDouble(Minute));
                        smallProcedure.Diagnosis = Diagnosis != null ? Diagnosis.DiagnosisFinal : "";
                        smallProcedure.ProcedureMethod = "";
                        smallProcedure.ProcedureDoctorStaff = new Staff();
                        smallProcedure.ProcedureDoctorStaff.StaffID = ProcedureDoctorStaff != null ? ProcedureDoctorStaff.StaffID : 0;
                        smallProcedure.PtRegDetailID = detail.PtRegDetailID;
                        smallProcedure.NurseStaff = new Staff();
                        smallProcedure.NurseStaff.StaffID = NurseStaff != null ? NurseStaff.StaffID : 0;
                        smallProcedure.V_RegistrationType = (long)AllLookupValues.RegistrationType.NGOAI_TRU;
                        smallProcedure.BeforeICD10 = new DiseasesReference();
                        smallProcedure.BeforeICD10.ICD10Code = Diagnosis != null ? Diagnosis.ICD10Code : "";
                        smallProcedure.BeforeICD10.DiagnosisFinal = Diagnosis != null ? Diagnosis.DiagnosisFinal : "";
                        smallProcedure.AfterICD10 = new DiseasesReference();
                        smallProcedure.AfterICD10.ICD10Code = Diagnosis != null ? Diagnosis.ICD10Code : "";
                        smallProcedure.AfterICD10.DiagnosisFinal = Diagnosis != null ? Diagnosis.DiagnosisFinal : "";
                        smallProcedure.ServiceRecID = Diagnosis != null ? (long)Diagnosis.ServiceRecID : 0;
                        smallProcedure.CreatedStaffID = Globals.LoggedUserAccount.Staff.StaffID;
                        smallProcedure.HIRepResourceCode = !string.IsNullOrEmpty(HIRepResourceCode) ? HIRepResourceCode : "";
                        TimeToEnd = smallProcedure.CompletedDateTime;
                    }
                    else if (detail.PaidTime > TimeToStart && ncount == 1)
                    {
                        smallProcedure.ProcedureDateTime = (DateTime)detail.PaidTime;
                        smallProcedure.CompletedDateTime = ((DateTime)detail.PaidTime).AddMinutes(Convert.ToDouble(Minute));
                        smallProcedure.Diagnosis = Diagnosis != null ? Diagnosis.DiagnosisFinal : "";
                        smallProcedure.ProcedureMethod = "";
                        smallProcedure.ProcedureDoctorStaff = new Staff();
                        smallProcedure.ProcedureDoctorStaff.StaffID = ProcedureDoctorStaff != null ? ProcedureDoctorStaff.StaffID : 0;
                        smallProcedure.PtRegDetailID = detail.PtRegDetailID;
                        smallProcedure.NurseStaff = new Staff();
                        smallProcedure.NurseStaff.StaffID = NurseStaff != null ? NurseStaff.StaffID : 0;
                        smallProcedure.V_RegistrationType = (long)AllLookupValues.RegistrationType.NGOAI_TRU;
                        smallProcedure.BeforeICD10 = new DiseasesReference();
                        smallProcedure.BeforeICD10.ICD10Code = Diagnosis != null ? Diagnosis.ICD10Code : "";
                        smallProcedure.BeforeICD10.DiagnosisFinal = Diagnosis != null ? Diagnosis.DiagnosisFinal : "";
                        smallProcedure.AfterICD10 = new DiseasesReference();
                        smallProcedure.AfterICD10.ICD10Code = Diagnosis != null ? Diagnosis.ICD10Code : "";
                        smallProcedure.AfterICD10.DiagnosisFinal = Diagnosis != null ? Diagnosis.DiagnosisFinal : "";
                        smallProcedure.ServiceRecID = Diagnosis != null ? (long)Diagnosis.ServiceRecID : 0;
                        smallProcedure.CreatedStaffID = Globals.LoggedUserAccount.Staff.StaffID;
                        smallProcedure.HIRepResourceCode = !string.IsNullOrEmpty(HIRepResourceCode) ? HIRepResourceCode : "";
                        TimeToEnd = smallProcedure.CompletedDateTime;
                    }
                    else if (detail.PaidTime < TimeToEnd)
                    {
                        smallProcedure.ProcedureDateTime = TimeToEnd.AddMinutes(1);
                        smallProcedure.CompletedDateTime = TimeToEnd.AddMinutes(Convert.ToDouble(Minute));
                        smallProcedure.Diagnosis = Diagnosis != null ? Diagnosis.DiagnosisFinal : "";
                        smallProcedure.ProcedureMethod = "";
                        smallProcedure.ProcedureDoctorStaff = new Staff();
                        smallProcedure.ProcedureDoctorStaff.StaffID = ProcedureDoctorStaff != null ? ProcedureDoctorStaff.StaffID : 0;
                        smallProcedure.PtRegDetailID = detail.PtRegDetailID;
                        smallProcedure.NurseStaff = new Staff();
                        smallProcedure.NurseStaff.StaffID = NurseStaff != null ? NurseStaff.StaffID : 0;
                        smallProcedure.V_RegistrationType = (long)AllLookupValues.RegistrationType.NGOAI_TRU;
                        smallProcedure.BeforeICD10 = new DiseasesReference();
                        smallProcedure.BeforeICD10.ICD10Code = Diagnosis != null ? Diagnosis.ICD10Code : "";
                        smallProcedure.BeforeICD10.DiagnosisFinal = Diagnosis != null ? Diagnosis.DiagnosisFinal : "";
                        smallProcedure.AfterICD10 = new DiseasesReference();
                        smallProcedure.AfterICD10.ICD10Code = Diagnosis != null ? Diagnosis.ICD10Code : "";
                        smallProcedure.AfterICD10.DiagnosisFinal = Diagnosis != null ? Diagnosis.DiagnosisFinal : "";
                        smallProcedure.ServiceRecID = Diagnosis != null ? (long)Diagnosis.ServiceRecID : 0;
                        smallProcedure.CreatedStaffID = Globals.LoggedUserAccount.Staff.StaffID;
                        smallProcedure.HIRepResourceCode = !string.IsNullOrEmpty(HIRepResourceCode) ? HIRepResourceCode : "";
                        TimeToEnd = smallProcedure.CompletedDateTime;
                    }
                    else if (detail.PaidTime > TimeToEnd)
                    {
                        smallProcedure.ProcedureDateTime = (DateTime)detail.PaidTime;
                        smallProcedure.CompletedDateTime = ((DateTime)detail.PaidTime).AddMinutes(Convert.ToDouble(Minute));
                        smallProcedure.Diagnosis = Diagnosis != null ? Diagnosis.DiagnosisFinal : "";
                        smallProcedure.ProcedureMethod = "";
                        smallProcedure.ProcedureDoctorStaff = new Staff();
                        smallProcedure.ProcedureDoctorStaff.StaffID = ProcedureDoctorStaff != null ? ProcedureDoctorStaff.StaffID : 0;
                        smallProcedure.PtRegDetailID = detail.PtRegDetailID;
                        smallProcedure.NurseStaff = new Staff();
                        smallProcedure.NurseStaff.StaffID = NurseStaff != null ? NurseStaff.StaffID : 0;
                        smallProcedure.V_RegistrationType = (long)AllLookupValues.RegistrationType.NGOAI_TRU;
                        smallProcedure.BeforeICD10 = new DiseasesReference();
                        smallProcedure.BeforeICD10.ICD10Code = Diagnosis != null ? Diagnosis.ICD10Code : "";
                        smallProcedure.BeforeICD10.DiagnosisFinal = Diagnosis != null ? Diagnosis.DiagnosisFinal : "";
                        smallProcedure.AfterICD10 = new DiseasesReference();
                        smallProcedure.AfterICD10.ICD10Code = Diagnosis != null ? Diagnosis.ICD10Code : "";
                        smallProcedure.AfterICD10.DiagnosisFinal = Diagnosis != null ? Diagnosis.DiagnosisFinal : "";
                        smallProcedure.ServiceRecID = Diagnosis != null ? (long)Diagnosis.ServiceRecID : 0;
                        smallProcedure.CreatedStaffID = Globals.LoggedUserAccount.Staff.StaffID;
                        smallProcedure.HIRepResourceCode = !string.IsNullOrEmpty(HIRepResourceCode) ? HIRepResourceCode : "";
                        TimeToEnd = smallProcedure.CompletedDateTime;
                    }
                    SmallProcedureList.Add(smallProcedure);
                }
            }

            var t = new Thread(() =>
            {
                using (var serviceFactory = new PatientRegistrationServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginSaveSmallProcedureAutomatic(SmallProcedureList, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            bool results = contract.EndSaveSmallProcedureAutomatic(asyncResult);
                            if (results)
                            {
                                MessageBox.Show(eHCMSResources.Z2788_G1_DaHoanTat);
                                btnSearch();
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                        finally
                        {
                            Globals.IsBusy = false;
                        }
                    }), null);
                }
            });

            t.Start();
        }

        private Staff _ProcedureDoctorStaff;
        public Staff ProcedureDoctorStaff
        {
            get
            {
                return _ProcedureDoctorStaff;
            }
            set
            {
                if (_ProcedureDoctorStaff == value)
                {
                    return;
                }
                _ProcedureDoctorStaff = value;
                NotifyOfPropertyChange(() => ProcedureDoctorStaff);
            }
        }
        AxAutoComplete cboProcedureDoctor { get; set; }
        public void ProcedureDoctor_Loaded(object sender, RoutedEventArgs e)
        {
            cboProcedureDoctor = (AxAutoComplete)sender;
        }
        public void ProcedureDoctor_Populating(object sender, PopulatingEventArgs e)
        {
            var mSearchText = (sender as AxAutoComplete).SearchText;
            if (!string.IsNullOrEmpty(mSearchText))
            {
                mSearchText = mSearchText.ToLower();
                mSearchText = Globals.RemoveVietnameseString(mSearchText);
            }
            (sender as AxAutoComplete).ItemsSource = Globals.AllStaffs.Where(x => x.RefStaffCategory != null
                && (x.RefStaffCategory.V_StaffCatType == (long)V_StaffCatType.BacSi)
                && !x.IsStopUsing
                && Globals.RemoveVietnameseString(x.FullName.ToLower()).Contains(mSearchText));
            (sender as AxAutoComplete).PopulateComplete();
        }
        public void ProcedureDoctor_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            if (ProcedureDoctorStaff == null)
            {
                ProcedureDoctorStaff = new Staff();
            }
            if ((sender as AxAutoComplete).SelectedItem == null)
            {
                ProcedureDoctorStaff.StaffID = 0;
                return;
            }
            ProcedureDoctorStaff.StaffID = ((sender as AxAutoComplete).SelectedItem as Staff).StaffID;
        }


        public void Nurse_Populating(object sender, PopulatingEventArgs e)
        {
            var mSearchText = (sender as AxAutoComplete).SearchText;
            if (!string.IsNullOrEmpty(mSearchText))
            {
                mSearchText = mSearchText.ToLower();
                mSearchText = Globals.RemoveVietnameseString(mSearchText);
            }
           (sender as AxAutoComplete).ItemsSource = Globals.AllStaffs.Where(x => (x.StaffCatgID == (long)StaffCatg.DieuDuong || x.StaffCatgID == (long)StaffCatg.NvHanhChanh)
                                                                            && !x.IsStopUsing
                                                                            && Globals.RemoveVietnameseString(x.FullName.ToLower()).Contains(mSearchText));
            (sender as AxAutoComplete).PopulateComplete();
        }
        private Staff _NurseStaff;
        public Staff NurseStaff
        {
            get
            {
                return _NurseStaff;
            }
            set
            {
                if (_NurseStaff == value)
                {
                    return;
                }
                _NurseStaff = value;
                NotifyOfPropertyChange(() => NurseStaff);
            }
        }
        AxAutoComplete cboNurse { get; set; }
        public void Nurse_Loaded(object sender, RoutedEventArgs e)
        {
            cboNurse = (AxAutoComplete)sender;
        }

        public void Nurse_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            if (NurseStaff == null)
            {
                NurseStaff = new Staff();
            }
            if ((sender as AxAutoComplete).SelectedItem == null)
            {
                NurseStaff.StaffID = 0;
                return;
            }
            NurseStaff.StaffID = ((sender as AxAutoComplete).SelectedItem as Staff).StaffID;
        }

        public bool CheckValidBeforePerform()
        {
            if (RegistrationObj == null || RegistrationObj.Count == 0)
            {
                MessageBox.Show(eHCMSResources.Z3047_G1_KhongCoDKDeThucHien);
                return false;
            }
            if (string.IsNullOrEmpty(Minute))
            {
                MessageBox.Show(eHCMSResources.Z3048_G1_KhongCoThoiGianThucHien);
                return false;
            }

            if (ProcedureDoctorStaff == null || ProcedureDoctorStaff.StaffID == 0)
            {
                MessageBox.Show(eHCMSResources.A0376_G1_Msg_InfoChuaChonBS);
                return false;
            }
            return true;
        }
        AxTextBox tbxDoTime = null;
        public void tbxDoTime_Loaded(object sender, RoutedEventArgs e)
        {
            tbxDoTime = (AxTextBox)sender;
        }
        public void tbxDoTime_LostFocus(object sender, RoutedEventArgs e)
        {
            if (tbxDoTime == null)
            {
                return;
            }
            Regex regEx = new Regex("^\\d+$", RegexOptions.IgnoreCase);
            Match match = regEx.Match(tbxDoTime.Text);
            if (!match.Success)
            {
                MessageBox.Show(eHCMSResources.Z3049_G1_ChiNhapSo);
                tbxDoTime.Text = "";
                return;
            }
        }
        public void GridRegistration_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString() + ". ";
        }
        public void RemoveCurrentRegistration_Click(object sender, RoutedEventArgs e)
        {
            Button CurrentButton = sender as Button;
            PatientRegistration CurrentRegistration = CurrentButton.DataContext as PatientRegistration;
            if (CurrentRegistration == null)
            {
                return;
            }
            RegistrationObj.Remove(CurrentRegistration);
        }
    }
}
