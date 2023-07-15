using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{
    public partial class MedicalConditionRecord : EntityBase, IEditableObject
    {
        #region Factory Method
        /// Create a new MedicalConditionRecord object.

        /// <param name="mCRecID">Initial value of the MCRecID property.</param>
        /// <param name="commonMedRecID">Initial value of the CommonMedRecID property.</param>
        /// <param name="mCID">Initial value of the MCID property.</param>
        public static MedicalConditionRecord CreateMedicalConditionRecord(long mCRecID, long commonMedRecID, long mCID)
        {
            MedicalConditionRecord medicalConditionRecord = new MedicalConditionRecord();
            medicalConditionRecord.MCRecID = mCRecID;
            medicalConditionRecord.CommonMedRecID = commonMedRecID;
            medicalConditionRecord.MCID = mCID;
            return medicalConditionRecord;
        }

        #endregion
        #region Primitive Properties
        [DataMemberAttribute()]
        public long MCRecID
        {
            get
            {
                return _MCRecID;
            }
            set
            {
                if (_MCRecID != value)
                {
                    OnMCRecIDChanging(value);
                    _MCRecID = value;
                    RaisePropertyChanged("MCRecID");
                    OnMCRecIDChanged();
                }
            }
        }
        private long _MCRecID;
        partial void OnMCRecIDChanging(long value);
        partial void OnMCRecIDChanged();

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
        public long MCID
        {
            get
            {
                return _MCID;
            }
            set
            {
                OnMCIDChanging(value);
                _MCID = value;
                RaisePropertyChanged("MCID");
                OnMCIDChanged();
            }
        }
        private long _MCID;
        partial void OnMCIDChanging(long value);
        partial void OnMCIDChanged();

        [DataMemberAttribute()]
        public Nullable<Boolean> MCYesNo
        {
            get
            {
                return _MCYesNo;
            }
            set
            {
                OnMCYesNoChanging(value);
                _MCYesNo = value;
                RaisePropertyChanged("MCYesNo");
                OnMCYesNoChanged();
            }
        }
        private Nullable<Boolean> _MCYesNo;
        partial void OnMCYesNoChanging(Nullable<Boolean> value);
        partial void OnMCYesNoChanged();

        [DataMemberAttribute()]
        public String MCTextValue
        {
            get
            {
                return _MCTextValue;
            }
            set
            {
                OnMCTextValueChanging(value);
                _MCTextValue = value;
                RaisePropertyChanged("MCTextValue");
                OnMCTextValueChanged();
            }
        }
        private String _MCTextValue;
        partial void OnMCTextValueChanging(String value);
        partial void OnMCTextValueChanged();

        [DataMemberAttribute()]
        public String MCExplainOrNotes
        {
            get
            {
                return _MCExplainOrNotes;
            }
            set
            {
                OnMCExplainOrNotesChanging(value);
                _MCExplainOrNotes = value;
                RaisePropertyChanged("MCExplainOrNotes");
                OnMCExplainOrNotesChanged();
            }
        }
        private String _MCExplainOrNotes;
        partial void OnMCExplainOrNotesChanging(String value);
        partial void OnMCExplainOrNotesChanged();

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

        private RefMedContraIndicationICD _RefMedicalCondition;
        [DataMemberAttribute()]
        public RefMedContraIndicationICD RefMedicalCondition
        {
            get
            {
                return _RefMedicalCondition;
            }
            set
            {
                if (_RefMedicalCondition != value)
                {
                    _RefMedicalCondition = value;
                    RaisePropertyChanged("RefMedicalCondition");
                }
            }
        }
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

        private MedicalConditionRecord _tempMedicalConditionRecord;
        #region IEditableObject Members

        public void BeginEdit()
        {
            _tempMedicalConditionRecord = (MedicalConditionRecord)this.MemberwiseClone();
        }

        public void CancelEdit()
        {
            if (null != _tempMedicalConditionRecord)
                CopyFrom(_tempMedicalConditionRecord);
        }

        public void EndEdit()
        {
        }

        public void CopyFrom(MedicalConditionRecord p)
        {
            PropertyCopierHelper.CopyPropertiesTo(p, this);
        }

        #endregion

    }
}
