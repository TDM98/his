using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;
using System.ComponentModel.DataAnnotations;

namespace DataEntities
{
    public partial class GroupPCLs_Detail : EntityBase, IEditableObject
    {
        public static GroupPCLs_Detail GroupPCL_Detail()
        {
            GroupPCLs_Detail ac = new GroupPCLs_Detail();
           
            return ac;
        }
        #region Primitive Properties

        [DataMemberAttribute()]
        public long GroupPCLs_Detail_ID
        {
            get
            {
                return _GroupPCLs_Detail_ID;
            }
            set
            {
                if (_GroupPCLs_Detail_ID == value)
                {
                    return;
                }
                _GroupPCLs_Detail_ID = value;
                RaisePropertyChanged("GroupPCLs_Detail_ID");
            }
        }
        private long _GroupPCLs_Detail_ID;
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
        public long PCLExamTypeID
        {
            get
            {
                return _PCLExamTypeID;
            }
            set
            {
                if (_PCLExamTypeID == value)
                {
                    return;
                }
                _PCLExamTypeID = value;
                RaisePropertyChanged("PCLExamTypeID");
            }
        }
        private long _PCLExamTypeID;

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
        public PCLExamType PCLExamType
        {
            get
            {
                return _PCLExamType;
            }
            set
            {
                if (_PCLExamType == value)
                {
                    return;
                }
                _PCLExamType = value;
                RaisePropertyChanged("PCLExamType");
            }
        }
        private PCLExamType _PCLExamType;

        [DataMemberAttribute]
        public GroupPCLs GroupPCL
        {
            get
            {
                return _GroupPCL;
            }
            set
            {
                if (_GroupPCL == value)
                {
                    return;
                }
                _GroupPCL = value;
                RaisePropertyChanged("GroupPCL");
            }
        }
        private GroupPCLs _GroupPCL;

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
