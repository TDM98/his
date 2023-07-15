using aEMR.Common;
using aEMR.Common.BaseModel;
using aEMR.Common.Collections;
using aEMR.Infrastructure;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using DataEntities;
using eHCMSLanguage;
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading;
using System.Windows.Controls;

namespace aEMR.Appointment.ViewModels
{
    [Export(typeof(IHospitalClientImportPatientCollection)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class HospitalClientImportPatientCollectionViewModel : ViewModelBase, IHospitalClientImportPatientCollection
    {
        #region Properties
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
                if (PatientGroupCollection == null || PatientGroupCollection.Count == 0)
                {
                    CurrentPatientGroupID = 0;
                }
                else
                {
                    CurrentPatientGroupID = PatientGroupCollection.First().HosClientContractPatientGroupID;
                }
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
        private string _CurrentFileName;
        public string CurrentFileName
        {
            get
            {
                return _CurrentFileName;
            }
            set
            {
                if (_CurrentFileName == value)
                {
                    return;
                }
                _CurrentFileName = value;
                NotifyOfPropertyChange(() => CurrentFileName);
            }
        }
        public bool IsConfirmed { get; set; } = false;
        private IHealthExaminationRecordServiceEdit _RecordServiceEditContent;
        public IHealthExaminationRecordServiceEdit RecordServiceEditContent
        {
            get
            {
                return _RecordServiceEditContent;
            }
            set
            {
                if (_RecordServiceEditContent == value)
                {
                    return;
                }
                _RecordServiceEditContent = value;
                NotifyOfPropertyChange(() => RecordServiceEditContent);
            }
        }
        private bool _IsNewGroup = true;
        public bool IsNewGroup
        {
            get
            {
                return _IsNewGroup;
            }
            set
            {
                if (_IsNewGroup == value)
                {
                    return;
                }
                _IsNewGroup = value;
                NotifyOfPropertyChange(() => IsNewGroup);
                GroupComboBox_SelectionChanged(null, null);
            }
        }
        private string _PatientGroupName;
        public string PatientGroupName
        {
            get
            {
                return _PatientGroupName;
            }
            set
            {
                if (_PatientGroupName == value)
                {
                    return;
                }
                _PatientGroupName = value;
                NotifyOfPropertyChange(() => PatientGroupName);
            }
        }
        public ObservableCollection<MedRegItemBase> MedRegItemBaseCollection
        {
            get
            {
                return RecordServiceEditContent == null ? null : RecordServiceEditContent.MedRegItemBaseCollection;
            }
        }
        public long HosClientContractID { get; set; }
        public HospitalClientContract CurrentClientContract { get; set; }
        private PatientRegistration _CurrentRegistration;
        public PatientRegistration CurrentRegistration
        {
            get
            {
                return _CurrentRegistration;
            }
            set
            {
                if (_CurrentRegistration == value)
                {
                    return;
                }
                _CurrentRegistration = value;
                NotifyOfPropertyChange(() => CurrentRegistration);
            }
        }
        public HosClientContractPatientGroup AddedNewHosClientContractPatientGroup { get; set; }
        #endregion
        #region Events
        [ImportingConstructor]
        public HospitalClientImportPatientCollectionViewModel()
        {
            RecordServiceEditContent = Globals.GetViewModel<IHealthExaminationRecordServiceEdit>();
            RecordServiceEditContent.IsChoossingCase = true;
        }
        public void ConfirmButton()
        {
            if (string.IsNullOrEmpty(CurrentFileName))
            {
                return;
            }
            if (RecordServiceEditContent == null || RecordServiceEditContent.MedRegItemBaseCollection == null ||
                RecordServiceEditContent.MedRegItemBaseCollection.Count == 0)
            {
                Globals.ShowMessage(eHCMSResources.T1278_G1_ChuaDKDVKBNao, eHCMSResources.G0442_G1_TBao);
                return;
            }
            if (!IsNewGroup && CurrentPatientGroupID <= 0)
            {
                Globals.ShowMessage(eHCMSResources.Z2938_G1_ThieuThongTinNhom, eHCMSResources.G0442_G1_TBao);
                return;
            }
            if (IsNewGroup && string.IsNullOrEmpty(PatientGroupName))
            {
                Globals.ShowMessage(eHCMSResources.Z2938_G1_ThieuThongTinNhom, eHCMSResources.G0442_G1_TBao);
                return;
            }
            if (!IsNewGroup)
            {
                IsConfirmed = true;
                TryClose();
            }
            else
            {
                SaveHosClientContractPatientGroup(PatientGroupName);
            }
        }
        public void BrowseButton()
        {
            OpenFileDialog objSFD = new OpenFileDialog()
            {
                DefaultExt = ".csv",
                Filter = "CSV|*.csv",
                FilterIndex = 1
            };
            if (objSFD.ShowDialog() != true)
            {
                return;
            }
            CurrentFileName = objSFD.FileName;
        }
        public void GroupComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PatientGroupCollection == null)
            {
                return;
            }
            var SelectedPatientGroup = PatientGroupCollection.FirstOrDefault(x => x.HosClientContractPatientGroupID == CurrentPatientGroupID);
            if (IsNewGroup || SelectedPatientGroup == null)
            {
                RecordServiceEditContent.MedRegItemBaseCollection = new ObservableCollection<MedRegItemBase>();
                return;
            }
            if (!CurrentClientContract.ContractPatientCollection.Any(x => x.PatientGroupCollection != null && x.PatientGroupCollection.Any(i => i.HosClientContractPatientGroupID == SelectedPatientGroup.HosClientContractPatientGroupID)))
            {
                return;
            }
            var PatientCollection = CurrentClientContract.ContractPatientCollection.Where(x => x.PatientGroupCollection != null && x.PatientGroupCollection.Any(i => i.HosClientContractPatientGroupID == SelectedPatientGroup.HosClientContractPatientGroupID)).ToList();
            if (PatientCollection == null || PatientCollection.Count == 0)
            {
                return;
            }
            var ItemLinkCollection = CurrentClientContract.ServiceItemPatientLinkCollection.Where(x => PatientCollection.Any(i => i.PatientObj == x.ContractPatient.PatientObj)).Select(x => x.ContractMedRegItem.MedRegItem).Distinct().ToList();
            if (ItemLinkCollection == null || ItemLinkCollection.Count == 0)
            {
                return;
            }
            RecordServiceEditContent.MedRegItemBaseCollection = CurrentClientContract.ContractServiceItemCollection.Where(x => ItemLinkCollection.Any(i => i == x.MedRegItem)).Select(x => x.MedRegItem).ToObservableCollection();
            var SelectedRegistration = CurrentRegistration.DeepCopy();
            SelectedRegistration.PatientRegistrationDetails = SelectedRegistration.PatientRegistrationDetails.Where(x => MedRegItemBaseCollection.Any(i => x is PatientRegistrationDetail && i is PatientRegistrationDetail && (i as PatientRegistrationDetail).MedServiceID == (x as PatientRegistrationDetail).MedServiceID)).ToObservableCollection();
            if (SelectedRegistration.PCLRequests != null && SelectedRegistration.PCLRequests.Count > 0)
            {
                SelectedRegistration.PCLRequests[0].PatientPCLRequestIndicators = SelectedRegistration.PCLRequests[0].PatientPCLRequestIndicators.Where(x => MedRegItemBaseCollection.Any(i => x is PatientPCLRequestDetail && i is PatientPCLRequestDetail && (i as PatientPCLRequestDetail).PCLExamTypeID == (x as PatientPCLRequestDetail).PCLExamTypeID)).ToObservableCollection();
            }
            RecordServiceEditContent.CurrentRegistration = SelectedRegistration;
        }
        #endregion
        #region Methods
        private void SaveHosClientContractPatientGroup(string PatientGroupName)
        {
            HosClientContractPatientGroup CurrentHosClientContractPatientGroup = new HosClientContractPatientGroup { HosClientContractID = HosClientContractID, HosClientContractPatientGroupName = PatientGroupName };
            this.DlgShowBusyIndicator();
            var CurrentThread = new Thread(() =>
            {
                try
                {
                    using (var CurrentFactory = new AppointmentServiceClient())
                    {
                        var Currentontract = CurrentFactory.ServiceInstance;
                        Currentontract.BeginSaveHosClientContractPatientGroup(CurrentHosClientContractPatientGroup, Globals.DispatchCallback(asyncResult =>
                        {
                            try
                            {
                                long OutHosClientContractPatientGroupID;
                                Currentontract.EndSaveHosClientContractPatientGroup(out OutHosClientContractPatientGroupID, asyncResult);
                                CurrentHosClientContractPatientGroup.HosClientContractPatientGroupID = OutHosClientContractPatientGroupID;
                                AddedNewHosClientContractPatientGroup = CurrentHosClientContractPatientGroup;
                                if (PatientGroupCollection == null)
                                {
                                    PatientGroupCollection = new ObservableCollection<HosClientContractPatientGroup>();
                                }
                                PatientGroupCollection.Add(AddedNewHosClientContractPatientGroup);
                                _CurrentPatientGroupID = OutHosClientContractPatientGroupID;
                                IsConfirmed = true;
                                TryClose();
                            }
                            catch (Exception ex)
                            {
                                Globals.ShowMessage(ex.Message, eHCMSResources.G0442_G1_TBao);
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
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    this.DlgHideBusyIndicator();
                }
            });
            CurrentThread.Start();
        }
        #endregion
    }
}
