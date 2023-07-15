using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{
    public partial class RefMedContraIndicationTypes : NotifyChangedBase, IEditableObject
    {
        #region Factory Method
        /// Create a new RefMedicalConditionType object.

        /// <param name="mCTypeID">Initial value of the MCTypeID property.</param>
        /// <param name="medConditionType">Initial value of the MedConditionType property.</param>
        public static RefMedContraIndicationTypes CreateRefMedicalConditionType(int mCTypeID, String medConditionType)
        {
            RefMedContraIndicationTypes refMedicalConditionType = new RefMedContraIndicationTypes();
            refMedicalConditionType.MedContraTypeID = mCTypeID;
            refMedicalConditionType.MedContraIndicationType = medConditionType;
            return refMedicalConditionType;
        }

        #endregion
        #region Primitive Properties

        [DataMemberAttribute()]
        public int MedContraTypeID
        {
            get
            {
                return _MedContraTypeID;
            }
            set
            {
                if (_MedContraTypeID != value)
                {
                    OnMedContraTypeIDChanging(value);
                    _MedContraTypeID = value;
                    RaisePropertyChanged("MedContraTypeID");
                    OnMedContraTypeIDChanged();
                }
            }
        }
        private int _MedContraTypeID;
        partial void OnMedContraTypeIDChanging(int value);
        partial void OnMedContraTypeIDChanged();

        [DataMemberAttribute()]
        public String MedContraIndicationType
        {
            get
            {
                return _MedContraIndicationType;
            }
            set
            {
                OnMedContraIndicationTypeChanging(value);
                _MedContraIndicationType = value;
                RaisePropertyChanged("MedContraIndicationType");
                OnMedContraIndicationTypeChanged();
            }
        }
        private String _MedContraIndicationType;
        partial void OnMedContraIndicationTypeChanging(String value);
        partial void OnMedContraIndicationTypeChanged();
        #endregion

        #region Navigation Properties
        [DataMemberAttribute()]
        public ObservableCollection<ContraIndicatorDrugsRelToMedCond> ContraIndicatorDrugsRelToMedConds
        {
            get
            {
                return _ContraIndicatorDrugsRelToMedConds;
            }
            set
            {
                _ContraIndicatorDrugsRelToMedConds = value;
                RaisePropertyChanged("ContraIndicatorDrugsRelToMedConds");
            }
        }
        private ObservableCollection<ContraIndicatorDrugsRelToMedCond> _ContraIndicatorDrugsRelToMedConds;
       
        [DataMemberAttribute()]
        public RefMedContraIndicationICD RefMedicalCondition
        {
            get
            {
                return _RefMedicalCondition;
            }
            set
            {
                _RefMedicalCondition = value;
                RaisePropertyChanged("RefMedicalCondition");
            }
        }
        private RefMedContraIndicationICD _RefMedicalCondition;

        [DataMemberAttribute()]
        public int? AgeFrom
        {
            get
            {
                return _AgeFrom;
            }
            set
            {
                _AgeFrom = value;
                RaisePropertyChanged("AgeFrom");
            }
        }
        private int? _AgeFrom;

        [DataMemberAttribute()]
        public int? AgeTo
        {
            get
            {
                return _AgeTo;
            }
            set
            {
                _AgeTo = value;
                RaisePropertyChanged("AgeTo");
            }
        }
        private int? _AgeTo;

        private long _V_AgeUnit;
        [DataMemberAttribute]
        public long V_AgeUnit
        {
            get
            {
                return _V_AgeUnit;
            }
            set
            {
                if (_V_AgeUnit == value)
                {
                    return;
                }
                _V_AgeUnit = value;
                RaisePropertyChanged("V_AgeUnit");
            }
        }
        [DataMemberAttribute()]
        public bool IsWarning
        {
            get
            {
                return _IsWarning;
            }
            set
            {
                _IsWarning = value;
                RaisePropertyChanged("IsWarning");
            }
        }
        private bool _IsWarning;
        #endregion

        public override bool Equals(object obj)
        {
            RefMedContraIndicationTypes cond = obj as RefMedContraIndicationTypes;
            if (cond == null)
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;

            return this.MedContraTypeID == cond.MedContraTypeID;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        private RefMedContraIndicationTypes _tempRefMedicalConditionType;
        #region IEditableObject Members

        public void BeginEdit()
        {
            _tempRefMedicalConditionType= (RefMedContraIndicationTypes)this.MemberwiseClone();
        }

        public void CancelEdit()
        {
            if (null != _tempRefMedicalConditionType)
                CopyFrom(_tempRefMedicalConditionType);
        }

        public void EndEdit()
        {
        }

        public void CopyFrom(RefMedContraIndicationTypes p)
        {
            PropertyCopierHelper.CopyPropertiesTo(p, this);
        }

        #endregion

    }
}
