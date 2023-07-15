using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;
using System.ComponentModel.DataAnnotations;

namespace DataEntities
{
    public partial class GroupPCLs : EntityBase, IEditableObject
    {
        public static GroupPCLs GroupPCL(Int64 GroupPCLID)
        {
            GroupPCLs ac = new GroupPCLs();
           
            return ac;
        }
        #region Primitive Properties

        [DataMemberAttribute()]
        public long GroupPCLID
        {
            get
            {
                return _GroupPCLID;
            }
            set
            {
                if (_GroupPCLID == value)
                {
                    return;
                }
                _GroupPCLID = value;
                RaisePropertyChanged("GroupPCLID");
            }
        }
        private long _GroupPCLID;

        [DataMemberAttribute()]
        public string GroupPCLName
        {
            get
            {
                return _GroupPCLName;
            }
            set
            {
                if (_GroupPCLName == value)
                {
                    return;
                }
                _GroupPCLName = value;
                RaisePropertyChanged("GroupPCLName");
            }
        }
        private string _GroupPCLName;

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

        [DataMemberAttribute]
        public ObservableCollection<PCLExamType> PCLExamTypeItem
        {
            get
            {
                return _PCLExamTypeItem;
            }
            set
            {
                if (_PCLExamTypeItem == value)
                {
                    return;
                }
                _PCLExamTypeItem = value;
                RaisePropertyChanged("PCLExamTypeItem");
            }
        }
        private ObservableCollection<PCLExamType> _PCLExamTypeItem;
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
      
        public ObservableCollection<GroupPCLs> GroupPCLss
        {
            get;
            set;
        }
        #endregion
    }
}
