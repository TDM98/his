using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;
using System.ComponentModel.DataAnnotations;

namespace DataEntities
{
    public partial class SymptomCategory : EntityBase, IEditableObject
    {
        public static SymptomCategory CreateSymptomCategory(Int64 SymptomCategoryID, String SymptomCategoryCode, String SymptomCategoryName)
        {
            SymptomCategory ac = new SymptomCategory();
           
            return ac;
        }
        #region Primitive Properties

        [DataMemberAttribute()]
        public long SymptomCategoryID
        {
            get
            {
                return _SymptomCategoryID;
            }
            set
            {
                if (_SymptomCategoryID == value)
                {
                    return;
                }
                _SymptomCategoryID = value;
                RaisePropertyChanged("SymptomCategoryID");
            }
        }
        private long _SymptomCategoryID;

        //[DataMemberAttribute()]
        //public string SymptomCategoryCode
        //{
        //    get
        //    {
        //        return _SymptomCategoryCode;
        //    }
        //    set
        //    {
        //        if (_SymptomCategoryCode == value)
        //        {
        //            return;
        //        }
        //        _SymptomCategoryCode = value;
        //        RaisePropertyChanged("SymptomCategoryCode");
        //    }
        //}
        //private string _SymptomCategoryCode;

        [DataMemberAttribute()]
        public long V_SymptomType
        {
            get
            {
                return _V_SymptomType;
            }
            set
            {
                if (_V_SymptomType == value)
                {
                    return;
                }
                _V_SymptomType = value;
                RaisePropertyChanged("V_SymptomType");
            }
        }
        private long _V_SymptomType;

        [DataMemberAttribute()]
        public string SymptomTypeName
        {
            get
            {
                return _SymptomTypeName;
            }
            set
            {
                if (_SymptomTypeName == value)
                {
                    return;
                }
                _SymptomTypeName = value;
                RaisePropertyChanged("SymptomTypeName");
            }
        }
        private string _SymptomTypeName;

        [DataMemberAttribute]
        public string SymptomCategoryName
        {
            get
            {
                return _SymptomCategoryName;
            }
            set
            {
                if (_SymptomCategoryName == value)
                {
                    return;
                }
                _SymptomCategoryName = value;
                RaisePropertyChanged("SymptomCategoryName");
            }
        }
        private string _SymptomCategoryName;


        [DataMemberAttribute]
        public bool IsDelete
        {
            get
            {
                return _IsDelete;
            }
            set
            {
                if (_IsDelete == value)
                {
                    return;
                }
                _IsDelete = value;
                RaisePropertyChanged("IsDelete");
            }
        }
        private bool _IsDelete;

        [DataMemberAttribute]
        public long CreatedStaffID
        {
            get
            {
                return _CreatedStaffID;
            }
            set
            {
                if (_CreatedStaffID == value)
                {
                    return;
                }
                _CreatedStaffID = value;
                RaisePropertyChanged("CreatedStaffID");
            }
        }
        private long _CreatedStaffID;

        [DataMemberAttribute]
        public DateTime CreatedDate
        {
            get
            {
                return _CreatedDate;
            }
            set
            {
                if (_CreatedDate == value)
                {
                    return;
                }
                _CreatedDate = value;
                RaisePropertyChanged("CreatedDate");
            }
        }
        private DateTime _CreatedDate;

        [DataMemberAttribute]
        public string LogModified
        {
            get
            {
                return _LogModified;
            }
            set
            {
                if (_LogModified == value)
                {
                    return;
                }
                _LogModified = value;
                RaisePropertyChanged("LogModified");
            }
        }
        private string _LogModified;
        public void BeginEdit()
        {
            throw new NotImplementedException();
        }

        public void EndEdit()
        {
            throw new NotImplementedException();
        }

        public void CancelEdit()
        {
            throw new NotImplementedException();
        }


        #endregion
        #region Navigation Properties
        [DataMemberAttribute()]
      
        public ObservableCollection<SymptomCategory> SymptomCategorys
        {
            get;
            set;
        }
        #endregion
    }
}
