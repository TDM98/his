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
    public partial class RefRows : NotifyChangedBase, IEditableObject
    {
        public RefRows() : base()
        {
        }
        private RefRows _tempRefRows;
        #region IEditableObject Members

        public void BeginEdit()
        {
            _tempRefRows = (RefRows)this.MemberwiseClone();
        }

        public void CancelEdit()
        {
            if (null != _tempRefRows)
                CopyFrom(_tempRefRows);
            //_tempPatient = null;
        }

        public void EndEdit()
        {
        }

        public void CopyFrom(RefRows p)
        {
            PropertyCopierHelper.CopyPropertiesTo(p, this);
        }

        #endregion
        #region Primitive Properties
        [DataMemberAttribute()]
        public long RefRowID
        {
            get
            {
                return _RefRowID;
            }
            set
            {
                if (_RefRowID != value)
                {
                    OnRefRowIDChanging(value);
                    _RefRowID = value;
                    OnRefRowIDChanged();
                }
            }
        }
        private long _RefRowID;
        partial void OnRefRowIDChanging(long value);
        partial void OnRefRowIDChanged();

        [DataMemberAttribute()]
        public String RefRowCode
        {
            get
            {
                return _RefRowCode;
            }
            set
            {
                OnRefRowCodeChanging(value);
                ValidateProperty("RefRowCode", value);
                _RefRowCode = value;
                RaisePropertyChanged("RefRowCode");
                OnRefRowCodeChanged();
            }
        }
        private String _RefRowCode;
        partial void OnRefRowCodeChanging(String value);
        partial void OnRefRowCodeChanged();

        [DataMemberAttribute()]
        public String RefRowName
        {
            get
            {
                return _RefRowName;
            }
            set
            {
                OnRefRowNameChanging(value);
                ValidateProperty("RefRowName", value);
                _RefRowName = value;
                RaisePropertyChanged("RefRowName");
                OnRefRowNameChanged();
            }
        }
        private String _RefRowName;
        partial void OnRefRowNameChanging(String value);
        partial void OnRefRowNameChanged();

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
        public RefStorageWarehouseLocation RowStore
        {
            get
            {
                return _RowStore;
            }
            set
            {
                _RowStore = value;
                RaisePropertyChanged("RowStore");
            }
        }
        private RefStorageWarehouseLocation _RowStore;

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
            RefRows seletedUnit = obj as RefRows;
            if (seletedUnit == null)
                return false;
            if (Object.ReferenceEquals(this, obj))
                return true;
            return this.RefRowID == seletedUnit.RefRowID;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
