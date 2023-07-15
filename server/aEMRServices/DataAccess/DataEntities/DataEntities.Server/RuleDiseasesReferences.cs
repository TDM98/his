using eHCMS.Services.Core.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DataEntities
{
    public partial class RuleDiseasesReferences : NotifyChangedBase
    {
        public static RuleDiseasesReferences CreateRuleDiseasesReferences(string MainICD10, string SubICD10, bool IsActive
            , bool IsException, string Desc, long StaffID)
        {
            RuleDiseasesReferences IBE = new RuleDiseasesReferences
            {
                MainICD10 = MainICD10,
                SubICD10 = SubICD10,
                IsActive = IsActive,
                IsException = IsException,
                Desc = Desc,
                StaffID = StaffID,
            };
            return IBE;
        }
        [DataMemberAttribute()]
        public string MainICD10
        {
            get
            {
                return _MainICD10;
            }
            set
            {
                if (_MainICD10 != value)
                {
                    _MainICD10 = value;
                    RaisePropertyChanged("MainICD10");
                }
            }
        }
        private string _MainICD10;

        [DataMemberAttribute()]
        public string SubICD10
        {
            get
            {
                return _SubICD10;
            }
            set
            {
                if (_SubICD10 != value)
                {
                    _SubICD10 = value;
                    RaisePropertyChanged("SubICD10");
                }
            }
        }
        private string _SubICD10;

        private bool _IsActive;
        [DataMemberAttribute]
        public bool IsActive
        {
            get
            {
                return _IsActive;
            }
            set
            {
                if (_IsActive != value)
                {
                    _IsActive = value;
                    RaisePropertyChanged("IsActive");
                }
            }
        }
        private bool _IsException;
        [DataMemberAttribute]
        public bool IsException
        {
            get
            {
                return _IsException;
            }
            set
            {
                if (_IsException != value)
                {
                    _IsException = value;
                    RaisePropertyChanged("IsException");
                }
            }
        }

        private string _ModifiedLog;
        [DataMemberAttribute]
        public string ModifiedLog
        {
            get
            {
                return _ModifiedLog;
            }
            set
            {
                if (_ModifiedLog != value)
                {
                    _ModifiedLog = value;
                    RaisePropertyChanged("ModifiedLog");
                }
            }
        }
        public string Desc { get; set; }
        public long StaffID { get; set; }
        public DiseasesReference SubICDInfo { get; set; }
        private bool _IsChoosed = false;
        public bool IsChoosed {
            get
            {
                return _IsChoosed;
            }
            set
            {
                if (_IsChoosed != value)
                {
                    _IsChoosed = value;
                    RaisePropertyChanged("IsChoosed");
                }
            }
        }
    }
}
