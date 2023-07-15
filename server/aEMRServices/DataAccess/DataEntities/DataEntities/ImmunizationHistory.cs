using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{
    public partial class ImmunizationHistory : EntityBase, IEditableObject
    {
        public ImmunizationHistory()
            : base()
        {

        }

        private ImmunizationHistory _tempImmunizationHistory;
        #region IEditableObject Members

        public void BeginEdit()
        {
            _tempImmunizationHistory = (ImmunizationHistory)this.MemberwiseClone();
        }

        public void CancelEdit()
        {
            if (null != _tempImmunizationHistory)
                CopyFrom(_tempImmunizationHistory);
            //_tempPatient = null;
        }

        public void EndEdit()
        {
        }

        public void CopyFrom(ImmunizationHistory p)
        {
            PropertyCopierHelper.CopyPropertiesTo(p, this);
        }

        #endregion
        #region Factory Method

        /// Create a new ImmunizationHistory object.

        /// <param name="iHID">Initial value of the IHID property.</param>
        /// <param name="iHCode">Initial value of the IHCode property.</param>
        /// <param name="commonMedRecID">Initial value of the CommonMedRecID property.</param>
        public static ImmunizationHistory CreateImmunizationHistory(long iHID, long iHCode, long commonMedRecID)
        {
            ImmunizationHistory immunizationHistory = new ImmunizationHistory();
            immunizationHistory.IHID = iHID;
            immunizationHistory.IHCode = iHCode;
            immunizationHistory.CommonMedRecID = commonMedRecID;
            return immunizationHistory;
        }

        #endregion
        #region Primitive Properties

        [DataMemberAttribute()]
        public long IHID
        {
            get
            {
                return _IHID;
            }
            set
            {
                if (_IHID != value)
                {
                    OnIHIDChanging(value);
                    _IHID = value;
                    RaisePropertyChanged("IHID");
                    OnIHIDChanged();
                }
            }
        }
        private long _IHID;
        partial void OnIHIDChanging(long value);
        partial void OnIHIDChanged();

        [DataMemberAttribute()]
        public long IHCode
        {
            get
            {
                return _IHCode;
            }
            set
            {
                OnIHCodeChanging(value);
                _IHCode = value;
                RaisePropertyChanged("IHCode");
                OnIHCodeChanged();
            }
        }
        private long _IHCode;
        partial void OnIHCodeChanging(long value);
        partial void OnIHCodeChanged();

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
        public Nullable<Boolean> IHYesNo
        {
            get
            {
                return _IHYesNo;
            }
            set
            {
                OnIHYesNoChanging(value);
                _IHYesNo = value;
                RaisePropertyChanged("IHYesNo");
                OnIHYesNoChanged();
            }
        }
        private Nullable<Boolean> _IHYesNo;
        partial void OnIHYesNoChanging(Nullable<Boolean> value);
        partial void OnIHYesNoChanged();

        [DataMemberAttribute()]
        public Nullable<DateTime> IHDate
        {
            get
            {
                return _IHDate;
            }
            set
            {
                OnIHDateChanging(value);
                _IHDate = value;
                RaisePropertyChanged("IHDate");
                OnIHDateChanged();
            }
        }
        private Nullable<DateTime> _IHDate=DateTime.Now;
        partial void OnIHDateChanging(Nullable<DateTime> value);
        partial void OnIHDateChanged();

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

        private RefImmunization _RefImmunization;
        [DataMemberAttribute()]
        public RefImmunization RefImmunization
        {
            get
            {
                return _RefImmunization;
            }
            set
            {
                if (_RefImmunization != value)
                {
                    _RefImmunization = value;
                    RaisePropertyChanged("RefImmunization");
                }
            }
        }

        #endregion
        public override bool Equals(object obj)
        {
            ImmunizationHistory cond = obj as ImmunizationHistory;
            if (cond == null)
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;

            return this.IHID == cond.IHID;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        [DataMemberAttribute()]
        public string MedServiceName
        {
            get
            {
                return _MedServiceName;
            }
            set
            {
                if (_MedServiceName != value)
                {
                    _MedServiceName = value;
                    RaisePropertyChanged("MedServiceName");
                }
            }
        }
        private string _MedServiceName;
    }
}
