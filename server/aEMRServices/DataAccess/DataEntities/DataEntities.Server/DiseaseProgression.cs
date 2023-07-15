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
  
    public partial class DiseaseProgression : EntityBase
    {
        #region Primitive Properties

        [DataMemberAttribute()]
        public long DiseaseProgressionID
        {
            get
            {
                return _DiseaseProgressionID;
            }
            set
            {
                if (_DiseaseProgressionID == value)
                {
                    return;
                }
                _DiseaseProgressionID = value;
                RaisePropertyChanged("DiseaseProgressionID");
            }
        }
        private long _DiseaseProgressionID;

        [DataMemberAttribute]
        public string DiseaseProgressionName
        {
            get
            {
                return _DiseaseProgressionName;
            }
            set
            {
                if (_DiseaseProgressionName == value)
                {
                    return;
                }
                _DiseaseProgressionName = value;
                RaisePropertyChanged("DiseaseProgressionName");
            }
        }
        private string _DiseaseProgressionName;

        [DataMemberAttribute]
        public string DiseaseProgressionNameWithUFW
        {
            get
            {
                return _DiseaseProgressionNameWithUFW;
            }
            set
            {
                if (_DiseaseProgressionNameWithUFW == value)
                {
                    return;
                }
                _DiseaseProgressionNameWithUFW = value;
                RaisePropertyChanged("DiseaseProgressionNameWithUFW");
            }
        }
        private string _DiseaseProgressionNameWithUFW;

        [DataMemberAttribute]
        public bool IsDeleted
        {
            get
            {
                return _IsDeleted;
            }
            set
            {
                if (_IsDeleted == value)
                {
                    return;
                }
                _IsDeleted = value;
                RaisePropertyChanged("IsDeleted");
            }
        }
        private bool _IsDeleted;

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
        public DateTime DateModified
        {
            get
            {
                return _DateModified;
            }
            set
            {
                if (_DateModified == value)
                {
                    return;
                }
                _DateModified = value;
                RaisePropertyChanged("DateModified");
            }
        }
        private DateTime _DateModified;

        [DataMemberAttribute]
        public string ModifiedLog
        {
            get
            {
                return _ModifiedLog;
            }
            set
            {
                if (_ModifiedLog == value)
                {
                    return;
                }
                _ModifiedLog = value;
                RaisePropertyChanged("ModifiedLog");
            }
        }
        private string _ModifiedLog;

        [DataMemberAttribute]
        public List<DiseaseProgressionDetails> DiseaseProgressionDetails
        {
            get
            {
                return _DiseaseProgressionDetails;
            }
            set
            {
                if (_DiseaseProgressionDetails == value)
                {
                    return;
                }
                _DiseaseProgressionDetails = value;
                RaisePropertyChanged("DiseaseProgressionDetails");
            }
        }
        private List<DiseaseProgressionDetails> _DiseaseProgressionDetails;

        [DataMemberAttribute]
        public bool UseForWebsite
        {
            get
            {
                return _UseForWebsite;
            }
            set
            {
                if (_UseForWebsite == value)
                {
                    return;
                }
                _UseForWebsite = value;
                RaisePropertyChanged("UseForWebsite");
            }
        }
        private bool _UseForWebsite;
        #endregion
    }
}
