using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;
using System.ComponentModel.DataAnnotations;

namespace DataEntities
{
    public partial class AdmissionCriteria : EntityBase, IEditableObject
    {
        public static AdmissionCriteria CreateAdmissionCriteria(Int64 AdmissionCriteriaID, String AdmissionCriteriaCode, String AdmissionCriteriaName)
        {
            AdmissionCriteria ac = new AdmissionCriteria();
           
            return ac;
        }
        #region Primitive Properties

        [DataMemberAttribute()]
        public long AdmissionCriteriaID
        {
            get
            {
                return _AdmissionCriteriaID;
            }
            set
            {
                if (_AdmissionCriteriaID == value)
                {
                    return;
                }
                _AdmissionCriteriaID = value;
                RaisePropertyChanged("AdmissionCriteriaID");
            }
        }
        private long _AdmissionCriteriaID;

        [DataMemberAttribute()]
        public string AdmissionCriteriaCode
        {
            get
            {
                return _AdmissionCriteriaCode;
            }
            set
            {
                if (_AdmissionCriteriaCode == value)
                {
                    return;
                }
                _AdmissionCriteriaCode = value;
                RaisePropertyChanged("AdmissionCriteriaCode");
            }
        }
        private string _AdmissionCriteriaCode;

        [DataMemberAttribute()]
        public long V_AdmissionCriteriaType
        {
            get
            {
                return _V_AdmissionCriteriaType;
            }
            set
            {
                if (_V_AdmissionCriteriaType == value)
                {
                    return;
                }
                _V_AdmissionCriteriaType = value;
                RaisePropertyChanged("V_AdmissionCriteriaType");
            }
        }
        private long _V_AdmissionCriteriaType;

        [DataMemberAttribute()]
        public string AdmissionCriteriaTypeName
        {
            get
            {
                return _AdmissionCriteriaTypeName;
            }
            set
            {
                if (_AdmissionCriteriaTypeName == value)
                {
                    return;
                }
                _AdmissionCriteriaTypeName = value;
                RaisePropertyChanged("AdmissionCriteriaTypeName");
            }
        }
        private string _AdmissionCriteriaTypeName;

        [DataMemberAttribute]
        public string AdmissionCriteriaName
        {
            get
            {
                return _AdmissionCriteriaName;
            }
            set
            {
                if (_AdmissionCriteriaName == value)
                {
                    return;
                }
                _AdmissionCriteriaName = value;
                RaisePropertyChanged("AdmissionCriteriaName");
            }
        }
        private string _AdmissionCriteriaName;

        [DataMemberAttribute]
        public string AdmissionCriteriaName_Ax
        {
            get
            {
                return _AdmissionCriteriaName_Ax;
            }
            set
            {
                if (_AdmissionCriteriaName_Ax == value)
                {
                    return;
                }
                _AdmissionCriteriaName_Ax = value;
                RaisePropertyChanged("AdmissionCriteriaName_Ax");
            }
        }
        private string _AdmissionCriteriaName_Ax;

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
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_LOCATION_REL_RM29_ROOMTYPE", "Locations")]
        public ObservableCollection<ICD> ICDs
        {
            get;
            set;
        }
        #endregion
    }
}
