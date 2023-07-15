using eHCMS.Services.Core.Base;
using System;
using System.Collections.ObjectModel;

namespace DataEntities
{
    public partial class RefTreatmentRegimen : NotifyChangedBase
    {
        private long _TreatmentRegimenID;
        private string _TreatmentRegimenCode;
        private string _TreatmentRegimenName;
        private string _ICD10Code;
        private string _TreatmentRegimenNote;
        private bool _IsDeleted;
        private DateTime _RecCreatedDate;
        private long _CreatedStaffID;
        private DateTime _LastUpdatedDate;
        private long _LastUpdatedStaffID;
        ObservableCollection<RefTreatmentRegimenDrugDetail> _RefTreatmentRegimenDrugDetails = new ObservableCollection<RefTreatmentRegimenDrugDetail>();
        ObservableCollection<RefTreatmentRegimenPCLDetail> _RefTreatmentRegimenPCLDetails = new ObservableCollection<RefTreatmentRegimenPCLDetail>();
        ObservableCollection<RefTreatmentRegimenServiceDetail> _RefTreatmentRegimenServiceDetails = new ObservableCollection<RefTreatmentRegimenServiceDetail>();
        public long TreatmentRegimenID
        {
            get
            {
                return _TreatmentRegimenID;
            }
            set
            {
                _TreatmentRegimenID = value;
                RaisePropertyChanged("TreatmentRegimenID");
            }
        }
        public string TreatmentRegimenCode
        {
            get
            {
                return _TreatmentRegimenCode;
            }
            set
            {
                _TreatmentRegimenCode = value;
                RaisePropertyChanged("TreatmentRegimenCode");
            }
        }
        public string TreatmentRegimenName
        {
            get
            {
                return _TreatmentRegimenName;
            }
            set
            {
                _TreatmentRegimenName = value;
                RaisePropertyChanged("TreatmentRegimenName");
            }
        }
        public string ICD10Code
        {
            get
            {
                return _ICD10Code;
            }
            set
            {
                _ICD10Code = value;
                RaisePropertyChanged("ICD10Code");
            }
        }
        public string TreatmentRegimenNote
        {
            get
            {
                return _TreatmentRegimenNote;
            }
            set
            {
                _TreatmentRegimenNote = value;
                RaisePropertyChanged("TreatmentRegimenNote");
            }
        }
        public bool IsDeleted
        {
            get
            {
                return _IsDeleted;
            }
            set
            {
                _IsDeleted = value;
                RaisePropertyChanged("IsDeleted");
            }
        }
        public DateTime RecCreatedDate
        {
            get
            {
                return _RecCreatedDate;
            }
            set
            {
                _RecCreatedDate = value;
                RaisePropertyChanged("RecCreatedDate");
            }
        }
        public long CreatedStaffID
        {
            get
            {
                return _CreatedStaffID;
            }
            set
            {
                _CreatedStaffID = value;
                RaisePropertyChanged("CreatedStaffID");
            }
        }
        public DateTime LastUpdatedDate
        {
            get
            {
                return _LastUpdatedDate;
            }
            set
            {
                _LastUpdatedDate = value;
                RaisePropertyChanged("LastUpdatedDate");
            }
        }
        public long LastUpdatedStaffID
        {
            get
            {
                return _LastUpdatedStaffID;
            }
            set
            {
                _LastUpdatedStaffID = value;
                RaisePropertyChanged("LastUpdatedStaffID");
            }
        }
        public ObservableCollection<RefTreatmentRegimenDrugDetail> RefTreatmentRegimenDrugDetails
        {
            get
            {
                return _RefTreatmentRegimenDrugDetails;
            }
            set
            {
                _RefTreatmentRegimenDrugDetails = value;
                RaisePropertyChanged("RefTreatmentRegimenDrugDetails");
            }
        }
        public ObservableCollection<RefTreatmentRegimenPCLDetail> RefTreatmentRegimenPCLDetails
        {
            get
            {
                return _RefTreatmentRegimenPCLDetails;
            }
            set
            {
                _RefTreatmentRegimenPCLDetails = value;
                RaisePropertyChanged("RefTreatmentRegimenPCLDetails");
            }
        }

        public ObservableCollection<RefTreatmentRegimenServiceDetail> RefTreatmentRegimenServiceDetails
        {
            get
            {
                return _RefTreatmentRegimenServiceDetails;
            }
            set
            {
                _RefTreatmentRegimenServiceDetails = value;
                RaisePropertyChanged("RefTreatmentRegimenServiceDetails");
            }
        }
    }
}