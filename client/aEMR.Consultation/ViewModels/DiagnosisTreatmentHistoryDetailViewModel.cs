using aEMR.Common.BaseModel;
using aEMR.ViewContracts;
using DataEntities;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Windows.Controls;
using System.Windows.Data;
using System.Xml.Linq;

namespace aEMR.ConsultantEPrescription.ViewModels
{
    [Export(typeof(IDiagnosisTreatmentHistoryDetail)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class DiagnosisTreatmentHistoryDetailViewModel : ViewModelBase, IDiagnosisTreatmentHistoryDetail
    {
        #region Properties
        private TabControl TCMain { get; set; }
        private Prescription _CurrentPrescription;
        public Prescription CurrentPrescription
        {
            get
            {
                return _CurrentPrescription;
            }
            set
            {
                if (_CurrentPrescription == value)
                {
                    return;
                }
                _CurrentPrescription = value;
                NotifyOfPropertyChange(() => CurrentPrescription);
            }
        }
        private DiagnosisTreatment _CurrentDiagnosisTreatment;
        public DiagnosisTreatment CurrentDiagnosisTreatment
        {
            get
            {
                return _CurrentDiagnosisTreatment;
            }
            set
            {
                if (_CurrentDiagnosisTreatment == value)
                {
                    return;
                }
                _CurrentDiagnosisTreatment = value;
                NotifyOfPropertyChange(() => CurrentDiagnosisTreatment);
                V_TreatmentType = ((AllLookupValues.V_TreatmentType)Enum.Parse(typeof(AllLookupValues.V_TreatmentType), Convert.ToString(CurrentDiagnosisTreatment.V_TreatmentType))).GetDescription();
            }
        }
        private ObservableCollection<DiagnosisIcd10Items> _CurrentIcd10Collection;
        public ObservableCollection<DiagnosisIcd10Items> CurrentIcd10Collection
        {
            get
            {
                return _CurrentIcd10Collection;
            }
            set
            {
                if (_CurrentIcd10Collection == value)
                {
                    return;
                }
                _CurrentIcd10Collection = value;
                NotifyOfPropertyChange(() => CurrentIcd10Collection);
            }
        }
        private ObservableCollection<DiagnosisICD9Items> _CurrentIcd9Collection;
        public ObservableCollection<DiagnosisICD9Items> CurrentIcd9Collection
        {
            get
            {
                return _CurrentIcd9Collection;
            }
            set
            {
                if (_CurrentIcd9Collection == value)
                {
                    return;
                }
                _CurrentIcd9Collection = value;
                NotifyOfPropertyChange(() => CurrentIcd9Collection);
                NotifyOfPropertyChange(() => IsHasIcd9);
            }
        }
        public bool IsHasIcd9
        {
            get
            {
                return CurrentIcd9Collection != null && CurrentIcd9Collection.Count > 0;
            }
        }
        private string _V_TreatmentType;
        public string V_TreatmentType
        {
            get
            {
                return _V_TreatmentType;
            }
            set
            {
                if (_V_TreatmentType == value)
                {
                    return;
                }
                _V_TreatmentType = value;
                NotifyOfPropertyChange(() => V_TreatmentType);
            }
        }
        #endregion
        #region Methods
        #endregion
        #region Events
        public void TCMain_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            TCMain = sender as TabControl;
        }
        protected override void OnActivate()
        {
            base.OnActivate();
        }
        public void grdPrescription_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString() + ". ";
        }
        #endregion
    }
}