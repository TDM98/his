using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{
    public partial class PastMedicalConditionHistory : EntityBase, IEditableObject
    {
        #region Factory Method

        /// Create a new PastMedicalConditionHistory object.

        /// <param name="pMHID">Initial value of the PMHID property.</param>
        /// <param name="commonMedRecID">Initial value of the CommonMedRecID property.</param>
        /// <param name="medHistCode">Initial value of the MedHistCode property.</param>
        public static PastMedicalConditionHistory CreatePastMedicalConditionHistory(long pMHID, long commonMedRecID, long medHistCode)
        {
            PastMedicalConditionHistory pastMedicalConditionHistory = new PastMedicalConditionHistory();
            pastMedicalConditionHistory.PMHID = pMHID;
            pastMedicalConditionHistory.CommonMedRecID = commonMedRecID;
            pastMedicalConditionHistory.MedHistCode = medHistCode;
            return pastMedicalConditionHistory;
        }

        #endregion
        #region Primitive Properties

        [DataMemberAttribute()]
        public long PMHID
        {
            get
            {
                return _PMHID;
            }
            set
            {
                if (_PMHID != value)
                {
                    OnPMHIDChanging(value);
                    _PMHID = value;
                    RaisePropertyChanged("PMHID");
                    OnPMHIDChanged();
                }
            }
        }
        private long _PMHID;
        partial void OnPMHIDChanging(long value);
        partial void OnPMHIDChanged();

        [DataMemberAttribute()]
        public long CommonMedRecID
        {
            get
            {
                return _CommonMedRecID;
            }
            set
            {
                OnCommonMedRecIDChanging(value);
                _CommonMedRecID = value;
                RaisePropertyChanged("CommonMedRecID");
                OnCommonMedRecIDChanged();
            }
        }
        private long _CommonMedRecID;
        partial void OnCommonMedRecIDChanging(long value);
        partial void OnCommonMedRecIDChanged();

        [DataMemberAttribute()]
        public long MedHistCode
        {
            get
            {
                return _MedHistCode;
            }
            set
            {
                OnMedHistCodeChanging(value);
                _MedHistCode = value;
                RaisePropertyChanged("MedHistCode");
                OnMedHistCodeChanged();
            }
        }
        private long _MedHistCode;
        partial void OnMedHistCodeChanging(long value);
        partial void OnMedHistCodeChanged();

        [DataMemberAttribute()]
        public Nullable<Boolean> PMHYesNo
        {
            get
            {
                return _PMHYesNo;
            }
            set
            {
                OnPMHYesNoChanging(value);
                _PMHYesNo = value;
                RaisePropertyChanged("PMHYesNo");
                OnPMHYesNoChanged();
            }
        }
        private Nullable<Boolean> _PMHYesNo;
        partial void OnPMHYesNoChanging(Nullable<Boolean> value);
        partial void OnPMHYesNoChanged();

        [DataMemberAttribute()]
        public String PMHExplainReason
        {
            get
            {
                return _PMHExplainReason;
            }
            set
            {
                OnPMHExplainReasonChanging(value);
                _PMHExplainReason = value;
                RaisePropertyChanged("PMHExplainReason");
                OnPMHExplainReasonChanged();
            }
        }
        private String _PMHExplainReason;
        partial void OnPMHExplainReasonChanging(String value);
        partial void OnPMHExplainReasonChanged();

        [DataMemberAttribute()]
        public Nullable<Int64> V_PMHStatus
        {
            get
            {
                return _V_PMHStatus;
            }
            set
            {
                OnV_PMHStatusChanging(value);
                _V_PMHStatus = value;
                RaisePropertyChanged("V_PMHStatus");
                OnV_PMHStatusChanged();
            }
        }
        private Nullable<Int64> _V_PMHStatus;
        partial void OnV_PMHStatusChanging(Nullable<Int64> value);
        partial void OnV_PMHStatusChanged();

        [DataMemberAttribute()]
        private bool _isDeleted = true;
        public bool isDeleted
        {
            get
            {
                return _isDeleted;
            }
            set
            {
                if (_isDeleted == value)
                    return;
                _isDeleted = value;
                RaisePropertyChanged("isDeleted");
            }
        }

        [DataMemberAttribute()]
        private bool _isEdit = true;
        public bool isEdit
        {
            get
            {
                return _isEdit;
            }
            set
            {
                if (_isEdit == value)
                    return;
                _isEdit = value;
                RaisePropertyChanged("isEdit");
                if (isEdit == false)
                {
                    isSave = true;
                    isCancel = true;
                }
                else
                {
                    isSave = false;
                    isCancel = false;
                }

            }
        }

        [DataMemberAttribute()]
        private bool _isCancel = false;
        public bool isCancel
        {
            get
            {
                return _isCancel;
            }
            set
            {
                if (_isCancel == value)
                    return;
                _isCancel = value;
                RaisePropertyChanged("isCancel");
            }
        }

        [DataMemberAttribute()]
        private bool _isSave = false;
        public bool isSave
        {
            get
            {
                return _isSave;
            }
            set
            {
                if (_isSave == value)
                    return;
                _isSave = value;
                RaisePropertyChanged("isSave");
            }
        }
        #endregion

        #region Navigation Properties
        private CommonMedicalRecord _CommonMedicalRecord;
        [DataMemberAttribute()]
        public CommonMedicalRecord CommonMedicalRecord
        {
            get
            {
                return _CommonMedicalRecord;
            }
            set
            {
                if (_CommonMedicalRecord != value)
                {
                    _CommonMedicalRecord = value;
                    RaisePropertyChanged("CommonMedicalRecord");
                }
            }
        }

        private RefMedicalHistory _RefMedicalHistory;
        [DataMemberAttribute()]
        public RefMedicalHistory RefMedicalHistory
        {
            get
            {
                return _RefMedicalHistory;
            }
            set
            {
                if (_RefMedicalHistory != value)
                {
                    _RefMedicalHistory = value;
                    RaisePropertyChanged("RefMedicalHistory");
                }
            }
        }

     
        
     
        private Lookup _LookupPMHStatus;
        [DataMemberAttribute()]
        public Lookup LookupPMHStatus
        {
            get
            {
                return _LookupPMHStatus;
            }
            set
            {
                if (_LookupPMHStatus != value)
                {
                    _LookupPMHStatus = value;
                    RaisePropertyChanged("LookupPMHStatus");
                }
            }
        }
        #endregion

        #region IEditableObject Members
        private PastMedicalConditionHistory _tempPastMedicalConditionHistory;
        public void BeginEdit()
        {
            _tempPastMedicalConditionHistory = (PastMedicalConditionHistory)this.MemberwiseClone();
        }

        public void CancelEdit()
        {
            if (null != _tempPastMedicalConditionHistory)
                CopyFrom(_tempPastMedicalConditionHistory);
        }

        public void EndEdit()
        {
        }

        public void CopyFrom(PastMedicalConditionHistory p)
        {
            PropertyCopierHelper.CopyPropertiesTo(p, this);
        }

        #endregion

        public override bool Equals(object obj)
        {
            PastMedicalConditionHistory cond = obj as PastMedicalConditionHistory;
            if (cond == null)
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;

            return this.PMHID == cond.PMHID;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

    }
}
