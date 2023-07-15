using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;


namespace DataEntities
{
    public partial class CommonMedicalRecord : NotifyChangedBase, IEditableObject
    {
        public CommonMedicalRecord()
            : base()
        {

        }

        private CommonMedicalRecord _tempCommonMedicalRecord;
        #region IEditableObject Members

        public void BeginEdit()
        {
            _tempCommonMedicalRecord = (CommonMedicalRecord)this.MemberwiseClone();
        }

        public void CancelEdit()
        {
            if (null != _tempCommonMedicalRecord)
                CopyFrom(_tempCommonMedicalRecord);
            //_tempPatient = null;
        }

        public void EndEdit()
        {
        }

        public void CopyFrom(CommonMedicalRecord p)
        {
            PropertyCopierHelper.CopyPropertiesTo(p, this);
        }

        #endregion
        #region Factory Method


        /// Create a new CommonMedicalRecord object.

        /// <param name="commonMedRecID">Initial value of the CommonMedRecID property.</param>
        /// <param name="cMRModifiedDate">Initial value of the CMRModifiedDate property.</param>
        /// <param name="v_ProcessingType">Initial value of the V_ProcessingType property.</param>
        public static CommonMedicalRecord CreateCommonMedicalRecord(long commonMedRecID, DateTime cMRModifiedDate, Int64 v_ProcessingType)
        {
            CommonMedicalRecord commonMedicalRecord = new CommonMedicalRecord();
            commonMedicalRecord.CommonMedRecID = commonMedRecID;
            commonMedicalRecord.CMRModifiedDate = cMRModifiedDate;
            commonMedicalRecord.V_ProcessingType = v_ProcessingType;
            return commonMedicalRecord;
        }

        #endregion
        #region Primitive Properties

        [DataMemberAttribute()]
        public long CommonMedRecID
        {
            get
            {
                return _CommonMedRecID;
            }
            set
            {
                if (_CommonMedRecID != value)
                {
                    OnCommonMedRecIDChanging(value);
                    _CommonMedRecID = value;
                    RaisePropertyChanged("CommonMedRecID");
                    OnCommonMedRecIDChanged();
                }
            }
        }
        private long _CommonMedRecID;
        partial void OnCommonMedRecIDChanging(long value);
        partial void OnCommonMedRecIDChanged();

        [DataMemberAttribute()]
        public Nullable<Int64> PtRegDetailID
        {
            get
            {
                return _PtRegDetailID;
            }
            set
            {
                OnPtRegDetailIDChanging(value);
                _PtRegDetailID = value;
                RaisePropertyChanged("PtRegDetailID");
                OnPtRegDetailIDChanged();
            }
        }
        private Nullable<Int64> _PtRegDetailID;
        partial void OnPtRegDetailIDChanging(Nullable<Int64> value);
        partial void OnPtRegDetailIDChanged();

     
        
     
        [DataMemberAttribute()]
        public Nullable<long> PatientID
        {
            get
            {
                return _PatientID;
            }
            set
            {
                OnPatientIDChanging(value);
                _PatientID = value;
                RaisePropertyChanged("PatientID");
                OnPatientIDChanged();
            }
        }
        private Nullable<long> _PatientID;
        partial void OnPatientIDChanging(Nullable<long> value);
        partial void OnPatientIDChanged();

        [DataMemberAttribute()]
        public Nullable<Int64> StaffID
        {
            get
            {
                return _StaffID;
            }
            set
            {
                OnStaffIDChanging(value);
                _StaffID = value;
                RaisePropertyChanged("StaffID");
                OnStaffIDChanged();
            }
        }
        private Nullable<Int64> _StaffID;
        partial void OnStaffIDChanging(Nullable<Int64> value);
        partial void OnStaffIDChanged();

        [DataMemberAttribute()]
        public Nullable<DateTime> CMRModifiedDate
        {
            get
            {
                return _CMRModifiedDate;
            }
            set
            {
                if (_CMRModifiedDate != value)
                {
                    OnCMRModifiedDateChanging(value);
                    _CMRModifiedDate = value;
                    RaisePropertyChanged("CMRModifiedDate");
                    OnCMRModifiedDateChanged();
                }
            }
        }
        private Nullable<DateTime> _CMRModifiedDate;
        partial void OnCMRModifiedDateChanging(Nullable<DateTime> value);
        partial void OnCMRModifiedDateChanged();

        [DataMemberAttribute()]
        public Int64 V_ProcessingType
        {
            get
            {
                return _V_ProcessingType;
            }
            set
            {
                OnV_ProcessingTypeChanging(value);
                _V_ProcessingType = value;
                RaisePropertyChanged("V_ProcessingType");
                OnV_ProcessingTypeChanged();
            }
        }
        private Int64 _V_ProcessingType;
        partial void OnV_ProcessingTypeChanging(Int64 value);
        partial void OnV_ProcessingTypeChanged();

        #endregion

        #region Navigation Properties
        private Staff _Staff;
        [DataMemberAttribute()]
        public Staff Staff
        {
            get
            {
                return _Staff;
            }
            set
            {
                if (_Staff != value)
                {
                    _Staff = value;
                    RaisePropertyChanged("Staff");
                }
            }
        }

        private PatientRegistrationDetail _PatientRegistrationDetail;
        [DataMemberAttribute()]
        public PatientRegistrationDetail PatientRegistrationDetail
        {
            get
            {
                return _PatientRegistrationDetail;
            }
            set
            {
                if (_PatientRegistrationDetail != value)
                {
                    _PatientRegistrationDetail = value;
                    RaisePropertyChanged("PatientRegistrationDetail");
                }
            }
        }

     
        
     
        private Patient _Patient;
        [DataMemberAttribute()]
        public Patient Patient
        {
            get
            {
                return _Patient;
            }
            set
            {
                if (_Patient != value)
                {
                    _Patient = value;
                    RaisePropertyChanged("Patient");
                }
            }
        }

        private ObservableCollection<DeceasedInfo> _DeceasedInfoes;
        [DataMemberAttribute()]
        public ObservableCollection<DeceasedInfo> DeceasedInfoes
        {
            get
            {
                return _DeceasedInfoes;
            }
            set
            {
                if (_DeceasedInfoes != value)
                {
                    _DeceasedInfoes = value;
                    RaisePropertyChanged("DeceasedInfoes");
                }
            }
        }

        private ObservableCollection<FamilyHistory> _FamilyHistories;
        [DataMemberAttribute()]
        public ObservableCollection<FamilyHistory> FamilyHistories
        {
            get
            {
                return _FamilyHistories;
            }
            set
            {
                if (_FamilyHistories != value)
                {
                    _FamilyHistories = value;
                    RaisePropertyChanged("FamilyHistories");
                }
            }
        }

        private ObservableCollection<HospitalizationHistory> _HospitalizationHistories;
        [DataMemberAttribute()]
        public ObservableCollection<HospitalizationHistory> HospitalizationHistories
        {
            get
            {
                return _HospitalizationHistories;
            }
            set
            {
                if (_HospitalizationHistories != value)
                {
                    _HospitalizationHistories = value;
                    RaisePropertyChanged("HospitalizationHistories");
                }
            }
        }

        private ObservableCollection<ImmunizationHistory> _ImmunizationHistories;
        [DataMemberAttribute()]
        public ObservableCollection<ImmunizationHistory> ImmunizationHistories
        {
            get
            {
                return _ImmunizationHistories;
            }
            set
            {
                if (_ImmunizationHistories != value)
                {
                    _ImmunizationHistories = value;
                    RaisePropertyChanged("ImmunizationHistories");
                }
            }
        }

        private ObservableCollection<MDAllergy> _MDAllergies;
        [DataMemberAttribute()]
        public ObservableCollection<MDAllergy> MDAllergies
        {
            get
            {
                return _MDAllergies;
            }
            set
            {
                if (_MDAllergies != value)
                {
                    _MDAllergies = value;
                    RaisePropertyChanged("MDAllergies");
                }
            }
        }

        private ObservableCollection<MDWarning> _MDWarnings;
        [DataMemberAttribute()]
        public ObservableCollection<MDWarning> MDWarnings
        {
            get
            {
                return _MDWarnings;
            }
            set
            {
                if (_MDWarnings != value)
                {
                    _MDWarnings = value;
                    RaisePropertyChanged("MDWarnings");
                }
            }
        }

        private ObservableCollection<MedicalConditionRecord> _MedicalConditionRecords;
        [DataMemberAttribute()]
        public ObservableCollection<MedicalConditionRecord> MedicalConditionRecords
        {
            get
            {
                return _MedicalConditionRecords;
            }
            set
            {
                if (_MedicalConditionRecords != value)
                {
                    _MedicalConditionRecords = value;
                    RaisePropertyChanged("MedicalConditionRecords");
                }
            }
        }

        private ObservableCollection<PastMedicalConditionHistory> _PastMedicalConditionHistories;
        [DataMemberAttribute()]
        public ObservableCollection<PastMedicalConditionHistory> PastMedicalConditionHistories
        {
            get
            {
                return _PastMedicalConditionHistories;
            }
            set
            {
                if (_PastMedicalConditionHistories != value)
                {
                    _PastMedicalConditionHistories = value;
                    RaisePropertyChanged("PastMedicalConditionHistories");
                }
            }
        }

        private ObservableCollection<PatientVitalSign> _PatientVitalSigns;
        [DataMemberAttribute()]
        public ObservableCollection<PatientVitalSign> PatientVitalSigns
        {
            get
            {
                return _PatientVitalSigns;
            }
            set
            {
                if (_PatientVitalSigns != value)
                {
                    _PatientVitalSigns = value;
                    RaisePropertyChanged("PatientVitalSigns");
                }
            }
        }

        private ObservableCollection<PhysicalExamination> _PhysicalExaminations;
        [DataMemberAttribute()]
        public ObservableCollection<PhysicalExamination> PhysicalExaminations
        {
            get
            {
                return _PhysicalExaminations;
            }
            set
            {
                if (_PhysicalExaminations != value)
                {
                    _PhysicalExaminations = value;
                    RaisePropertyChanged("PhysicalExaminations");
                }
            }
        }

     
        
     
        private Lookup _LookupProcessingType;
        [DataMemberAttribute()]
        public Lookup LookupProcessingType
        {
            get
            {
                return _LookupProcessingType;
            }
            set
            {
                if (_LookupProcessingType != value)
                {
                    _LookupProcessingType = value;
                    RaisePropertyChanged("LookupProcessingType");
                }
            }
        }
        #endregion

        /// <summary>
        /// 31-08-2012 Dinh
        /// Thêm trạng thái để phân biệt nội trú và ngoại trú
        /// </summary>
        private RegistrationType _RegistrationType;
        [DataMemberAttribute()]
        public RegistrationType RegistrationType
        {
            get
            {
                return _RegistrationType;
            }
            set
            {
                _RegistrationType = value;
                RaisePropertyChanged("RegistrationType");
            }
        }


        private AllLookupValues.RegistrationType _V_RegistrationType = AllLookupValues.RegistrationType.NGOAI_TRU;
        [DataMemberAttribute()]
        public AllLookupValues.RegistrationType V_RegistrationType
        {
            get
            {
                return _V_RegistrationType;
            }
            set
            {
                _V_RegistrationType = value;
                RaisePropertyChanged("V_RegistrationType");
            }
        }
    }
}
