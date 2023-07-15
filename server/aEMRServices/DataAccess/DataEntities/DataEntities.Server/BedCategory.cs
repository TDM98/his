using eHCMS.Services.Core;
using eHCMS.Services.Core.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DataEntities
{
    public partial class BedCategory: NotifyChangedBase, IEditableObject
    {
        public BedCategory()
          : base()
        {

        }
        private BedCategory _tempBedCategory;
        public override bool Equals(object obj)
        {
            BedCategory info = obj as BedCategory;
            if (info == null)
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;

            return this.BedCategoryID > 0 && this.BedCategoryID == info.BedCategoryID;
        }

        public void BeginEdit()
        {
            _tempBedCategory = (BedCategory)this.MemberwiseClone();
        }

        public void CancelEdit()
        {
            if (null != _tempBedCategory)
                CopyFrom(_tempBedCategory);
            //_tempPatient = null;
        }

        public void EndEdit()
        {
        }

        public void CopyFrom(BedCategory p)
        {
            PropertyCopierHelper.CopyPropertiesTo(p, this);
        }

        //[DataMemberAttribute()]
        //public BedCategory ObjBedCategory
        //{
        //    get { return _ObjBedCategory; }
        //    set
        //    {
        //        if (_ObjBedCategory != value)
        //        {
        //            OnObjBedCategoryChanging(value);
        //            _ObjBedCategory = value;
        //            RaisePropertyChanged("ObjBedCategory");
        //            OnObjBedCategoryChanged();
        //        }
        //    }
        //}
        //private BedCategory _ObjBedCategory;
        //partial void OnObjBedCategoryChanging(BedCategory value);
        //partial void OnObjBedCategoryChanged();

        [DataMemberAttribute()]
        public long BedCategoryID
        {
            get
            {
                return _BedCategoryID;
            }
            set
            {
                OnBedCategoryIDChanging(value);
                _BedCategoryID = value;
                RaisePropertyChanged("BedCategoryID");
                OnBedCategoryIDChanged();
            }
        }
        private long _BedCategoryID;
        partial void OnBedCategoryIDChanging(long value);
        partial void OnBedCategoryIDChanged();

        [DataMemberAttribute()]
        public string HIBedCode
        {
            get
            {
                return _HIBedCode;
            }
            set
            {
                OnHIBedCodeChanging(value);
                _HIBedCode = value;
                RaisePropertyChanged("HIBedCode");
                OnHIBedCodeChanged();
            }
        }
        private string _HIBedCode;
        partial void OnHIBedCodeChanging(string value);
        partial void OnHIBedCodeChanged();

        [DataMemberAttribute()]
        public string HIBedName
        {
            get
            {
                return _HIBedName;
            }
            set
            {
                OnHIBedNameChanging(value);
                _HIBedName = value;
                RaisePropertyChanged("HIBedName");
                OnHIBedNameChanged();
            }
        }
        private string _HIBedName;
        partial void OnHIBedNameChanging(string value);
        partial void OnHIBedNameChanged();

        [DataMemberAttribute()]
        public string HosBedCode
        {
            get
            {
                return _HosBedCode;
            }
            set
            {
                OnHosBedCodeChanging(value);
                _HosBedCode = value;
                RaisePropertyChanged("HosBedCode");
                OnHosBedCodeChanged();
            }
        }
        private string _HosBedCode;
        partial void OnHosBedCodeChanging(string value);
        partial void OnHosBedCodeChanged();

        [DataMemberAttribute()]
        public string HosBedName
        {
            get
            {
                return _HosBedName;
            }
            set
            {
                OnHosBedNameChanging(value);
                _HosBedName = value;
                RaisePropertyChanged("HosBedName");
                OnHosBedNameChanged();
            }
        }
        private string _HosBedName;
        partial void OnHosBedNameChanging(string value);
        partial void OnHosBedNameChanged();

        [DataMemberAttribute()]
        public Lookup BedType
        {
            get
            {
                return _BedType;
            }
            set
            {
                _BedType = value;
                RaisePropertyChanged("BedType");
            }
        }
        private Lookup _BedType;

        [DataMemberAttribute()]
        public bool IsActive
        {
            get
            {
                return _IsActive;
            }
            set
            {
                OnIsActiveChanging(value);
                _IsActive = value;
                RaisePropertyChanged("IsActive");
                OnIsActiveChanged();
            }
        }
        private bool _IsActive;
        partial void OnIsActiveChanging(bool value);
        partial void OnIsActiveChanged();
        [DataMemberAttribute()]
        
        public Int64 DeptLocID
        {
            get { return _DeptLocID; }
            set
            {
                OnDeptLocIDChanging(value);
                _DeptLocID = value;
                RaisePropertyChanged("DeptLocID");
                OnDeptLocIDChanged();
            }
        }
        private Int64 _DeptLocID;
        partial void OnDeptLocIDChanging(Int64 value);
        partial void OnDeptLocIDChanged();

        [DataMemberAttribute()]
        public long V_BedType
        {
            get { return _V_BedType; }
            set
            {
                _V_BedType = value;
                RaisePropertyChanged("V_BedType");
            }
        }
        private long _V_BedType;
    }
}
