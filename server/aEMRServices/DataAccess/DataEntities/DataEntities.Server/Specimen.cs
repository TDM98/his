using eHCMS.Services.Core.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DataEntities
{
    public partial class Specimen : EntityBase
    {
        #region Primitive Properties
        [DataMemberAttribute()]
        public long SpecimenID
        {
            get
            {
                return _SpecimenID;
            }
            set
            {
                if (_SpecimenID == value)
                {
                    return;
                }
                _SpecimenID = value;
                RaisePropertyChanged("SpecimenID");
            }
        }
        private long _SpecimenID;

        [DataMemberAttribute()]
        public string SpecimenName
        {
            get
            {
                return _SpecimenName;
            }
            set
            {
                if (_SpecimenName == value)
                {
                    return;
                }
                _SpecimenName = value;
                RaisePropertyChanged("SpecimenName");
            }
        }
        private string _SpecimenName;



        [DataMemberAttribute()]
        public string SpecimenNameEng
        {
            get
            {
                return _SpecimenNameEng;
            }
            set
            {
                if (_SpecimenNameEng == value)
                {
                    return;
                }
                _SpecimenNameEng = value;
                RaisePropertyChanged("SpecimenNameEng");
            }
        }
        private string _SpecimenNameEng;

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
        public Staff CreatedStaff
        {
            get
            {
                return _CreatedStaff;
            }
            set
            {
                if (_CreatedStaff == value)
                {
                    return;
                }
                _CreatedStaff = value;
                RaisePropertyChanged("CreatedStaff");
            }
        }
        private Staff _CreatedStaff;

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
        public Staff LastUpdateStaff
        {
            get
            {
                return _LastUpdateStaff;
            }
            set
            {
                if (_LastUpdateStaff == value)
                {
                    return;
                }
                _LastUpdateStaff = value;
                RaisePropertyChanged("LastUpdateStaff");
            }
        }
        private Staff _LastUpdateStaff;

        [DataMemberAttribute]
        public DateTime LastUpdateDate
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
        private DateTime _LastUpdateDate;
        #endregion
    }
}
