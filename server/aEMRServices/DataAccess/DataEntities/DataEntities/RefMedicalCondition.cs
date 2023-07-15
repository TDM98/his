using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{
    public partial class RefMedContraIndicationICD : NotifyChangedBase, IEditableObject
    {
        #region Factory Method
        /// Create a new RefMedicalCondition object.
        /// <param name="mCID">Initial value of the MCID property.</param>
        /// <param name="mCDescription">Initial value of the MCDescription property.</param>
        /// <param name="medConditionType">Initial value of the MedConditionType property.</param>
        public static RefMedContraIndicationICD CreateRefMedicalCondition(long mCID, String mCDescription)
        {
            RefMedContraIndicationICD refMedContraIndicationICD = new RefMedContraIndicationICD();
            refMedContraIndicationICD.MCID = mCID;
            refMedContraIndicationICD.MCDescription = mCDescription;
            return refMedContraIndicationICD;
        }

        #endregion
        #region Primitive Properties

        [DataMemberAttribute()]
        public long MCID
        {
            get
            {
                return _MCID;
            }
            set
            {
                if (_MCID != value)
                {
                    OnMCIDChanging(value);
                    _MCID = value;
                    RaisePropertyChanged("MCID");
                    OnMCIDChanged();
                }
            }
        }
        private long _MCID;
        partial void OnMCIDChanging(long value);
        partial void OnMCIDChanged();

     
        
     
        [DataMemberAttribute()]
        public Nullable<int> MedContraTypeID
        {
            get
            {
                return _MedContraTypeID;
            }
            set
            {
                _MedContraTypeID = value;
                RaisePropertyChanged("MedContraTypeID");
            }
        }
        private Nullable<int> _MedContraTypeID;

        [DataMemberAttribute()]
        public String MCDescription
        {
            get
            {
                return _MCDescription;
            }
            set
            {
                OnMCDescriptionChanging(value);
                _MCDescription = value;
                RaisePropertyChanged("MCDescription");
                OnMCDescriptionChanged();
            }
        }
        private String _MCDescription;
        partial void OnMCDescriptionChanging(String value);
        partial void OnMCDescriptionChanged();
        #endregion

        #region Navigation Properties
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
                    RaiseErrorsChanged("MedicalConditionRecords");
                }
            }
        }
        private ObservableCollection<MedicalConditionRecord> _MedicalConditionRecords;

       
        [DataMemberAttribute()]
        public ObservableCollection<RefMedContraIndicationTypes> RefMedicalConditionTypes
        {
            get
            {
                return _RefMedicalConditionTypes;
            }
            set
            {
                if (_RefMedicalConditionTypes != value)
                {
                    _RefMedicalConditionTypes = value;
                    RaisePropertyChanged("RefMedicalConditionTypes");
                }
            }
        }
        private ObservableCollection<RefMedContraIndicationTypes> _RefMedicalConditionTypes;

        public RefMedContraIndicationTypes RefMedicalConditionType
        {
            get
            {
                return _RefMedicalConditionType;
            }
            set
            {
                if (_RefMedicalConditionType != value)
                {
                    _RefMedicalConditionType = value;
                    RaisePropertyChanged("RefMedicalConditionType");
                }
            }
        }
        private RefMedContraIndicationTypes _RefMedicalConditionType;
        partial void OnRefMedicalConditionTypeChanging(RefMedContraIndicationTypes value);
        partial void OnRefMedicalConditionTypeChanged();

        [DataMemberAttribute()]
        public string ICD10Code
        {
            get
            {
                return _ICD10Code;
            }
            set
            {
                if (_ICD10Code != value)
                {
                    _ICD10Code = value;
                    RaisePropertyChanged("ICD10Code");
                }
            }
        }
        private string _ICD10Code;

        [DataMemberAttribute()]
        public string DiseaseNameVN
        {
            get
            {
                return _DiseaseNameVN;
            }
            set
            {
                if (_DiseaseNameVN != value)
                {
                    _DiseaseNameVN = value;
                    RaisePropertyChanged("DiseaseNameVN");
                }
            }
        }
        private string _DiseaseNameVN;
        #endregion
        public override bool Equals(object obj)
        {
            RefMedContraIndicationICD cond = obj as RefMedContraIndicationICD;
            if (cond == null)
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;

            return this.MCID== cond.MCID;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        
        #region IEditableObject Members
        private RefMedContraIndicationICD _tempRefMedicalCondition;
        public void BeginEdit()
        {
            _tempRefMedicalCondition = (RefMedContraIndicationICD)this.MemberwiseClone();
        }

        public void CancelEdit()
        {
            if (null != _tempRefMedicalCondition)
                CopyFrom(_tempRefMedicalCondition);
        }

        public void EndEdit()
        {
        }

        public void CopyFrom(RefMedContraIndicationICD p)
        {
            PropertyCopierHelper.CopyPropertiesTo(p, this);
        }

        #endregion

    }
}
