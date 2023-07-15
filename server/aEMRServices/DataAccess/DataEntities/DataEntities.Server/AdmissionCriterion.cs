using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;
using System.ComponentModel.DataAnnotations;

namespace DataEntities
{
    public partial class AdmissionCriterion : EntityBase, IEditableObject
    {
        public static AdmissionCriterion CreateAdmissionCriterion(Int64 AdmissionCriterionID)
        {
            AdmissionCriterion ac = new AdmissionCriterion();
           
            return ac;
        }
        #region Primitive Properties

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
        public string AdmissionCriterionName
        {
            get
            {
                return _AdmissionCriterionName;
            }
            set
            {
                if (_AdmissionCriterionName == value)
                {
                    return;
                }
                _AdmissionCriterionName = value;
                RaisePropertyChanged("AdmissionCriterionName");
            }
        }
        private string _AdmissionCriterionName;

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
        public bool IsActive
        {
            get
            {
                return _IsActive;
            }
            set
            {
                if (_IsActive == value)
                {
                    return;
                }
                _IsActive = value;
                RaisePropertyChanged("IsActive");
            }
        }
        private bool _IsActive;

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
      
        public ObservableCollection<AdmissionCriterion> AdmissionCriterions
        {
            get;
            set;
        }
        #endregion
    }
}
