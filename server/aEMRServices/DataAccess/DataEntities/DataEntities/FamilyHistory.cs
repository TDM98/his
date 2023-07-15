using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;
namespace DataEntities
{
    public partial class FamilyHistory : EntityBase, IEditableObject
    {
        #region Factory Method


        /// Create a new FamilyHistory object.

        /// <param name="fHCode">Initial value of the FHCode property.</param>
        public static FamilyHistory CreateFamilyHistory(long fHCode)
        {
            FamilyHistory familyHistory = new FamilyHistory();
            familyHistory.FHCode = fHCode;
            return familyHistory;
        }

        #endregion
        #region Primitive Properties
        [DataMemberAttribute()]
        public long FHCode
        {
            get
            {
                return _FHCode;
            }
            set
            {
                if (_FHCode != value)
                {
                    OnFHCodeChanging(value);
                    _FHCode = value;
                    RaisePropertyChanged("FHCode");
                    OnFHCodeChanged();
                }
            }
        }
        private long _FHCode;
        partial void OnFHCodeChanging(long value);
        partial void OnFHCodeChanged();

        [DataMemberAttribute()]
        public Nullable<Int64> IDCode
        {
            get
            {
                return _IDCode;
            }
            set
            {
                OnIDCodeChanging(value);
                _IDCode = value;
                RaisePropertyChanged("IDCode");
                OnIDCodeChanged();
            }
        }
        private Nullable<Int64> _IDCode;
        partial void OnIDCodeChanging(Nullable<Int64> value);
        partial void OnIDCodeChanged();

        [DataMemberAttribute()]
        public Nullable<long> CommonMedRecID
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
        private Nullable<long> _CommonMedRecID;
        partial void OnCommonMedRecIDChanging(Nullable<long> value);
        partial void OnCommonMedRecIDChanged();

        [DataMemberAttribute()]
        public String FHFullName
        {
            get
            {
                return _FHFullName;
            }
            set
            {
                OnFHFullNameChanging(value);
                _FHFullName = value;
                RaisePropertyChanged("FHFullName");
                OnFHFullNameChanged();
            }
        }
        private String _FHFullName;
        partial void OnFHFullNameChanging(String value);
        partial void OnFHFullNameChanged();

        [DataMemberAttribute()]
        public Nullable<Int64> V_FamilyRelationship
        {
            get
            {
                return _V_FamilyRelationship;
            }
            set
            {
                OnV_FamilyRelationshipChanging(value);
                _V_FamilyRelationship = value;
                RaisePropertyChanged("V_FamilyRelationship");
                OnV_FamilyRelationshipChanged();
            }
        }
        private Nullable<Int64> _V_FamilyRelationship;
        partial void OnV_FamilyRelationshipChanging(Nullable<Int64> value);
        partial void OnV_FamilyRelationshipChanged();

        [DataMemberAttribute()]
        public String FHNotes
        {
            get
            {
                return _FHNotes;
            }
            set
            {
                OnFHNotesChanging(value);
                _FHNotes = value;
                RaisePropertyChanged("FHNotes");
                OnFHNotesChanged();
            }
        }
        private String _FHNotes;
        partial void OnFHNotesChanging(String value);
        partial void OnFHNotesChanged();

        [DataMemberAttribute()]
        public Nullable<Boolean> Decease
        {
            get
            {
                return _Decease;
            }
            set
            {
                OnDeceaseChanging(value);
                _Decease = value;
                RaisePropertyChanged("Decease");
                OnDeceaseChanged();
            }
        }
        private Nullable<Boolean> _Decease;
        partial void OnDeceaseChanging(Nullable<Boolean> value);
        partial void OnDeceaseChanged();

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

        private String _DiseaseNameVN;
        [DataMemberAttribute()]
        public String DiseaseNameVN
        {
            get
            {
                return _DiseaseNameVN;
            }
            set
            {
                _DiseaseNameVN = value;
                RaisePropertyChanged("DiseaseNameVN");
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
                    OnCommonMedicalRecordChanging(value);
                    _CommonMedicalRecord = value;
                    RaisePropertyChanged("CommonMedicalRecord");
                    OnCommonMedicalRecordChanged();
                }
            }
        }
        partial void OnCommonMedicalRecordChanging(CommonMedicalRecord value);
        partial void OnCommonMedicalRecordChanged();

        private DiseasesReference _DiseasesReference;
        [DataMemberAttribute()]
        public DiseasesReference DiseasesReference
        {
            get
            {
                return _DiseasesReference;
            }
            set
            {
                if(_DiseasesReference !=value)
                {
                    _DiseasesReference= value;
                    RaisePropertyChanged("DiseasesReference");
                }
            }
        }
     
        private Lookup _LookupFamilyRelationship;
        [DataMemberAttribute()]
        public Lookup LookupFamilyRelationship
        {
            get
            {
                return _LookupFamilyRelationship;
            }
            set
            {
                if (_LookupFamilyRelationship != value)
                {
                    _LookupFamilyRelationship = value;
                    RaisePropertyChanged("LookupFamilyRelationship");
                }
            }
        }
        #endregion

        private FamilyHistory _tempFamilyHistory;
        #region IEditableObject Members

        public void BeginEdit()
        {
            _tempFamilyHistory = (FamilyHistory)this.MemberwiseClone();
        }

        public void CancelEdit()
        {
            if (null != _tempFamilyHistory)
                CopyFrom(_tempFamilyHistory);
        }

        public void EndEdit()
        {
        }

        public void CopyFrom(FamilyHistory p)
        {
            PropertyCopierHelper.CopyPropertiesTo(p, this);
        }

        #endregion


    }
}
