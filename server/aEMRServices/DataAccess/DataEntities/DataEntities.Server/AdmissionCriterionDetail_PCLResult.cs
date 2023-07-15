using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;
using System.ComponentModel.DataAnnotations;
using System.IO;

namespace DataEntities
{
    public partial class AdmissionCriterionDetail_PCLResult : EntityBase, IEditableObject
    {
        public static AdmissionCriterionDetail_PCLResult CreateAdmissionCriterionDetail_PCLResult(Int64 AdmissionCriterionDetail_PCLResultID)
        {
            AdmissionCriterionDetail_PCLResult ac = new AdmissionCriterionDetail_PCLResult();
           
            return ac;
        }
        #region Primitive Properties

        [DataMemberAttribute()]
        public long AdmissionCriterionDetail_PCLResultID
        {
            get
            {
                return _AdmissionCriterionDetail_PCLResultID;
            }
            set
            {
                if (_AdmissionCriterionDetail_PCLResultID == value)
                {
                    return;
                }
                _AdmissionCriterionDetail_PCLResultID = value;
                RaisePropertyChanged("AdmissionCriterionDetail_PCLResultID");
            }
        }
        private long _AdmissionCriterionDetail_PCLResultID;

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

        [DataMemberAttribute()]
        public string ImageResultUrl
        {
            get
            {
                return _ImageResultUrl;
            }
            set
            {
                if (_ImageResultUrl == value)
                {
                    return;
                }
                _ImageResultUrl = value;
                RaisePropertyChanged("ImageResultUrl");
            }
        }
        private string _ImageResultUrl;

        [DataMemberAttribute()]
        public long V_ResultType
        {
            get
            {
                return _V_ResultType;
            }
            set
            {
                if (_V_ResultType == value)
                {
                    return;
                }
                _V_ResultType = value;
                RaisePropertyChanged("V_ResultType");
            }
        }
        private long _V_ResultType;

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

        [DataMemberAttribute()]
        public byte[] File
        {
            get
            {
                return _File;
            }
            set
            {
                if (_File == value)
                {
                    return;
                }
                _File = value;
                RaisePropertyChanged("File");
            }
        }
        private byte[] _File;

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
