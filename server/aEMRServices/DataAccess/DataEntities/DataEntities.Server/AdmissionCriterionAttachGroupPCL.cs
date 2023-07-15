using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;
using System.ComponentModel.DataAnnotations;

namespace DataEntities
{
    public partial class AdmissionCriterionAttachGroupPCL : EntityBase, IEditableObject
    {
        public static AdmissionCriterionAttachGroupPCL CreateGroupPCLCategory(Int64 ACAG_ID)
        {
            AdmissionCriterionAttachGroupPCL ac = new AdmissionCriterionAttachGroupPCL();
           
            return ac;
        }
        #region Primitive Properties

        [DataMemberAttribute()]
        public long ACAG_ID
        {
            get
            {
                return _ACAG_ID;
            }
            set
            {
                if (_ACAG_ID == value)
                {
                    return;
                }
                _ACAG_ID = value;
                RaisePropertyChanged("ACAG_ID");
            }
        }
        private long _ACAG_ID;

        [DataMemberAttribute()]
        public long AdmissionCriterionID
        {
            get
            {
                return _AdmissionCriterionID;
            }
            set
            {
                if (_AdmissionCriterionID == value)
                {
                    return;
                }
                _AdmissionCriterionID = value;
                RaisePropertyChanged("AdmissionCriterionID");
            }
        }
        private long _AdmissionCriterionID;

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
      
        public ObservableCollection<AdmissionCriterionAttachICD> GroupPCLAttachICDs
        {
            get;
            set;
        }
        #endregion
    }
}
