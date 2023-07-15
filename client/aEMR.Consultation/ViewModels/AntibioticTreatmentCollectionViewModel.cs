using aEMR.Common;
using aEMR.Common.BaseModel;
using aEMR.Common.Collections;
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
using System.Windows.Controls;
using System.Windows.Input;

namespace aEMR.Consultation.ViewModels
{
    [Export(typeof(IAntibioticTreatmentCollection)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class AntibioticTreatmentCollectionViewModel : ViewModelBase, IAntibioticTreatmentCollection
    {
        #region Properties
        private ObservableCollection<AntibioticTreatment> _AntibioticTreatmentCollection;
        public ObservableCollection<AntibioticTreatment> AntibioticTreatmentCollection
        {
            get
            {
                return _AntibioticTreatmentCollection;
            }
            set
            {
                if (_AntibioticTreatmentCollection == value)
                {
                    return;
                }
                _AntibioticTreatmentCollection = value;
                NotifyOfPropertyChange(() => AntibioticTreatmentCollection);
            }
        }
        public PatientRegistration CurrentRegistration { get; set; }
        public AntibioticTreatment SelectedAntibioticTreatment { get; set; } = null;
        #endregion
        #region Events
        [ImportingConstructor]
        public AntibioticTreatmentCollectionViewModel()
        {
        }
        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            GetAntibioticTreatmentsByPtRegID();
        }
        public void AntibioticTreatment_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            if ((sender as DataGrid).SelectedItem == null)
            {
                return;
            }
            SelectedAntibioticTreatment = (sender as DataGrid).SelectedItem as AntibioticTreatment;
            TryClose();
        }
        #endregion
        #region Methods
        private void GetAntibioticTreatmentsByPtRegID()
        {
            if (CurrentRegistration == null || CurrentRegistration.PtRegistrationID == 0)
            {
                return;
            }
            this.DlgShowBusyIndicator();
            var CurrentThread = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetAntibioticTreatmentsByPtRegID(CurrentRegistration.PtRegistrationID, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var GettedCollection = contract.EndGetAntibioticTreatmentsByPtRegID(asyncResult);
                                if (GettedCollection != null && GettedCollection.Count > 0)
                                {
                                    AntibioticTreatmentCollection = GettedCollection.Where(x => x.InfectionCaseID == 0).ToObservableCollection();
                                }
                                else
                                {
                                    AntibioticTreatmentCollection = new ObservableCollection<AntibioticTreatment>();
                                }
                            }
                            catch (Exception ex)
                            {
                                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
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