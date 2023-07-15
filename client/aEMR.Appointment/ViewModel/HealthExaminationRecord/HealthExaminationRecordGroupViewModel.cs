using aEMR.Common.BaseModel;
using aEMR.Infrastructure;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using DataEntities;
using eHCMSLanguage;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading;

namespace aEMR.Appointment.ViewModels
{
    [Export(typeof(IHealthExaminationRecordGroup)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class HealthExaminationRecordGroupViewModel : ViewModelBase, IHealthExaminationRecordGroup
    {
        #region Properties
        public long HosClientContractID { get; set; }
        public HosClientContractPatientGroup CurrentHosClientContractPatientGroup { get; set; } = new HosClientContractPatientGroup();
        public bool IsCompleted { get; set; } = false;
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
        #endregion
        #region Events
        [ImportingConstructor]
        public HealthExaminationRecordGroupViewModel()
        {
        }
        public void SaveButton()
        {
            if (!IsNewGroup && CurrentPatientGroupID > 0)
            {
                CurrentHosClientContractPatientGroup.HosClientContractPatientGroupID = CurrentPatientGroupID;
                IsCompleted = true;
                TryClose();
                return;
            }
            CurrentHosClientContractPatientGroup.HosClientContractID = HosClientContractID;
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
                                IsCompleted = true;
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