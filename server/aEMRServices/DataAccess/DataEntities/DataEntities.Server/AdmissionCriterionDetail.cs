using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;
using System.ComponentModel.DataAnnotations;

namespace DataEntities
{
    public partial class AdmissionCriterionDetail : EntityBase, IEditableObject
    {
        public static AdmissionCriterionDetail CreateAdmissionCriterion(Int64 AdmissionCriterionDetailID)
        {
            AdmissionCriterionDetail ac = new AdmissionCriterionDetail();
           
            return ac;
        }
        #region Primitive Properties

        [DataMemberAttribute()]
        public long AdmissionCriterionDetailID
        {
            get
            {
                return _AdmissionCriterionDetailID;
            }
            set
            {
                if (_AdmissionCriterionDetailID == value)
                {
                    return;
                }
                _AdmissionCriterionDetailID = value;
                RaisePropertyChanged("AdmissionCriterionDetailID");
            }
        }
        private long _AdmissionCriterionDetailID;

        [DataMemberAttribute()]
        public long PtRegistrationID
        {
            get
            {
                return _PtRegistrationID;
            }
            set
            {
                if (_PtRegistrationID == value)
                {
                    return;
                }
                _PtRegistrationID = value;
                RaisePropertyChanged("PtRegistrationID");
            }
        }
        private long _PtRegistrationID;

        [DataMemberAttribute()]
        public string SymptomList
        {
            get
            {
                return _SymptomList;
            }
            set
            {
                if (_SymptomList == value)
                {
                    return;
                }
                _SymptomList = value;
                RaisePropertyChanged("SymptomList");
            }
        }
        private string _SymptomList;


        [DataMemberAttribute]
        public long? CreatedStaffID
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
        private long? _CreatedStaffID;

        [DataMemberAttribute]
        public DateTime? CreatedDate
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
        private DateTime? _CreatedDate;

        [DataMemberAttribute]
        public long? LastUpdateStaffID
        {
            get
            {
                return _LastUpdateStaffID;
            }
            set
            {
                if (_LastUpdateStaffID == value)
                {
                    return;
                }
                _LastUpdateStaffID = value;
                RaisePropertyChanged("LastUpdateStaffID");
            }
        }
        private long? _LastUpdateStaffID;

        [DataMemberAttribute]
        public DateTime? LastUpdateDate
        {
            get
            {
                return _LastUpdateDate;
            }
            set
            {
                if (_LastUpdateDate == value)
                {
                    return;
                }
                _LastUpdateDate = value;
                RaisePropertyChanged("LastUpdateDate");
            }
        }
        private DateTime? _LastUpdateDate;

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

    }
}
