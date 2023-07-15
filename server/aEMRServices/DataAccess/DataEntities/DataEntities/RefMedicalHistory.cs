using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{
    public partial class RefMedicalHistory : NotifyChangedBase, IEditableObject
    {
        #region Factory Method


        /// Create a new RefMedicalHistory object.

        /// <param name="medHistCode">Initial value of the MedHistCode property.</param>
        /// <param name="medHistTreatment">Initial value of the MedHistTreatment property.</param>
        public static RefMedicalHistory CreateRefMedicalHistory(long medHistCode, String medHistTreatment)
        {
            RefMedicalHistory refMedicalHistory = new RefMedicalHistory();
            refMedicalHistory.MedHistCode = medHistCode;
            refMedicalHistory.MedHistTreatment = medHistTreatment;
            return refMedicalHistory;
        }

        #endregion
        #region Primitive Properties
        [DataMemberAttribute()]
        public long MedHistCode
        {
            get
            {
                return _MedHistCode;
            }
            set
            {
                if (_MedHistCode != value)
                {
                    OnMedHistCodeChanging(value);
                    _MedHistCode = value;
                    RaisePropertyChanged("MedHistCode");
                    OnMedHistCodeChanged();
                }
            }
        }
        private long _MedHistCode;
        partial void OnMedHistCodeChanging(long value);
        partial void OnMedHistCodeChanged();

        [DataMemberAttribute()]
        public String MedHistTreatment
        {
            get
            {
                return _MedHistTreatment;
            }
            set
            {
                OnMedHistTreatmentChanging(value);
                _MedHistTreatment = value;
                RaisePropertyChanged("MedHistTreatment");
                OnMedHistTreatmentChanged();
            }
        }
        private String _MedHistTreatment;
        partial void OnMedHistTreatmentChanging(String value);
        partial void OnMedHistTreatmentChanged();

        #endregion

        #region Navigation Properties
        [DataMemberAttribute()]
        public ObservableCollection<DonorsMedicalCondition> DonorsMedicalConditions
        {
            get
            {
                return _DonorsMedicalConditions;
            }
            set
            {
                if (_DonorsMedicalConditions != value)
                {
                    _DonorsMedicalConditions = value;
                    RaiseErrorsChanged("DonorsMedicalConditions");
                }
            }
        }
        private ObservableCollection<DonorsMedicalCondition> _DonorsMedicalConditions;
        
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
        private ObservableCollection<PastMedicalConditionHistory> _PastMedicalConditionHistories;

        #endregion

        private RefMedicalHistory _tempRefMedicalHistory;
        #region IEditableObject Members

        public void BeginEdit()
        {
            _tempRefMedicalHistory = (RefMedicalHistory)this.MemberwiseClone();
        }

        public void CancelEdit()
        {
            if (null != _tempRefMedicalHistory)
                CopyFrom(_tempRefMedicalHistory);
        }

        public void EndEdit()
        {
        }

        public void CopyFrom(RefMedicalHistory p)
        {
            PropertyCopierHelper.CopyPropertiesTo(p, this);
        }

        #endregion

    }
}
