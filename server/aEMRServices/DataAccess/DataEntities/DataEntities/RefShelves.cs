using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace DataEntities
{
    public partial class RefShelves : NotifyChangedBase, IEditableObject
    {
        public RefShelves() : base()
        {
        }
        private RefShelves _tempRefShelf;
        #region IEditableObject Members

        public void BeginEdit()
        {
            _tempRefShelf = (RefShelves)this.MemberwiseClone();
        }

        public void CancelEdit()
        {
            if (null != _tempRefShelf)
                CopyFrom(_tempRefShelf);
            //_tempPatient = null;
        }

        public void EndEdit()
        {
        }

        public void CopyFrom(RefShelves p)
        {
            PropertyCopierHelper.CopyPropertiesTo(p, this);
        }

        #endregion
        #region Primitive Properties
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
        public String RefShelfCode
        {
            get
            {
                return _RefShelfCode;
            }
            set
            {
                OnRefShelfCodeChanging(value);
                ValidateProperty("RefShelfCode", value);
                _RefShelfCode = value;
                RaisePropertyChanged("RefShelfCode");
                OnRefShelfCodeChanged();
            }
        }
        private String _RefShelfCode;
        partial void OnRefShelfCodeChanging(String value);
        partial void OnRefShelfCodeChanged();

        [DataMemberAttribute()]
        public String RefShelfName
        {
            get
            {
                return _RefShelfName;
            }
            set
            {
                OnRefShelfNameChanging(value);
                ValidateProperty("RefShelfName", value);
                _RefShelfName = value;
                RaisePropertyChanged("RefShelfName");
                OnRefShelfNameChanged();
            }
        }
        private String _RefShelfName;
        partial void OnRefShelfNameChanging(String value);
        partial void OnRefShelfNameChanged();

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

        [DataMemberAttribute()]
        public long StoreID
        {
            get
            {
                return _StoreID;
            }
            set
            {
                _StoreID = value;
                RaisePropertyChanged("StoreID");
            }
        }
        private long _StoreID;

        [DataMemberAttribute()]
        public string swhlName
        {
            get
            {
                return _swhlName;
            }
            set
            {
                _swhlName = value;
                RaisePropertyChanged("swhlName");
            }
        }
        private string _swhlName;

        [DataMemberAttribute()]
        public List<RefStorageWarehouseLocation> AllStores
        {
            get
            {
                return _AllStores;
            }
            set
            {
                _AllStores = value;
                RaisePropertyChanged("AllStores");
            }
        }
        private List<RefStorageWarehouseLocation> _AllStores;

        [DataMemberAttribute()]
        public RefStorageWarehouseLocation ShelfStore
        {
            get
            {
                return _ShelfStore;
            }
            set
            {
                _ShelfStore = value;
                RaisePropertyChanged("ShelfStore");
            }
        }
        private RefStorageWarehouseLocation _ShelfStore;

        [DataMemberAttribute()]
        public int TotalFiles
        {
            get
            {
                return _TotalFiles;
            }
            set
            {
                _TotalFiles = value;
                RaisePropertyChanged("TotalFiles");
            }
        }
        private int _TotalFiles;
        #endregion
        public override bool Equals(object obj)
        {
            RefShelves seletedUnit = obj as RefShelves;
            if (seletedUnit == null)
                return false;
            if (Object.ReferenceEquals(this, obj))
                return true;
            return this.RefShelfID == seletedUnit.RefShelfID;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        [DataMemberAttribute()]
        public long RefRowID
        {
            get
            {
                return _RefRowID;
            }
            set
            {
                _RefRowID = value;
                RaisePropertyChanged("RefRowID");
            }
        }
        private long _RefRowID;
    }
}
