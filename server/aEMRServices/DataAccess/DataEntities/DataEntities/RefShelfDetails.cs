using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;
using System.ComponentModel.DataAnnotations;

namespace DataEntities
{
    public partial class RefShelfDetails : NotifyChangedBase, IEditableObject
    {
        public RefShelfDetails()
            : base()
        {
        }
        private RefShelfDetails _tempRefShelfDetail;
        #region IEditableObject Members

        public void BeginEdit()
        {
            _tempRefShelfDetail = (RefShelfDetails)this.MemberwiseClone();
        }

        public void CancelEdit()
        {
            if (null != _tempRefShelfDetail)
                CopyFrom(_tempRefShelfDetail);
            //_tempPatient = null;
        }

        public void EndEdit()
        {
        }

        public void CopyFrom(RefShelfDetails p)
        {
            PropertyCopierHelper.CopyPropertiesTo(p, this);
        }

        #endregion
        #region Primitive Properties
        [DataMemberAttribute()]
        public long RefShelfDetailID
        {
            get
            {
                return _RefShelfDetailID;
            }
            set
            {
                if (_RefShelfDetailID != value)
                {
                    OnRefShelfDetailIDChanging(value);
                    _RefShelfDetailID = value;
                    OnRefShelfDetailIDChanged();
                }
            }
        }
        private long _RefShelfDetailID;
        partial void OnRefShelfDetailIDChanging(long value);
        partial void OnRefShelfDetailIDChanged();

        [DataMemberAttribute()]
        public long RefShelfID
        {
            get
            {
                return _RefShelfID;
            }
            set
            {
                if (_RefShelfID != value)
                {
                    OnRefShelfIDChanging(value);
                    _RefShelfID = value;
                    OnRefShelfIDChanged();
                }
            }
        }
        private long _RefShelfID;
        partial void OnRefShelfIDChanging(long value);
        partial void OnRefShelfIDChanged();

        [DataMemberAttribute()]
        public String LocCode
        {
            get
            {
                return _LocCode;
            }
            set
            {
                OnLocCodeChanging(value);
                ValidateProperty("LocCode", value);
                _LocCode = value;
                RaisePropertyChanged("LocCode");
                OnLocCodeChanged();
            }
        }
        private String _LocCode;
        partial void OnLocCodeChanging(String value);
        partial void OnLocCodeChanged();

        [DataMemberAttribute()]
        public string LocName
        {
            get
            {
                return _LocName;
            }
            set
            {
                _LocName = value;
                RaisePropertyChanged("LocName");
            }
        }
        private string _LocName;
        
        [DataMemberAttribute()]
        public String Note
        {
            get
            {
                return _Note;
            }
            set
            {
                OnNoteChanging(value);
                ValidateProperty("Note", value);
                _Note = value;
                RaisePropertyChanged("Note");
                OnNoteChanged();
            }
        }
        private String _Note;
        partial void OnNoteChanging(String value);
        partial void OnNoteChanged();

        [DataMemberAttribute()]
        public DateTime CreatedDate
        {
            get
            {
                return _CreatedDate;
            }
            set
            {
                OnCreatedDateChanging(value);
                ValidateProperty("CreatedDate", value);
                _CreatedDate = value;
                RaisePropertyChanged("CreatedDate");
                OnCreatedDateChanged();
            }
        }
        private DateTime _CreatedDate;
        partial void OnCreatedDateChanging(DateTime value);
        partial void OnCreatedDateChanged();
        #endregion
        public override bool Equals(object obj)
        {
            RefShelfDetails seletedUnit = obj as RefShelfDetails;
            if (seletedUnit == null)
                return false;
            if (Object.ReferenceEquals(this, obj))
                return true;
            return this.RefShelfDetailID == seletedUnit.RefShelfDetailID;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
