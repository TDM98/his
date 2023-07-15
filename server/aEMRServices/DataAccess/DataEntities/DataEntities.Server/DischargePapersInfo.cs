using eHCMS.Services.Core;
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
    public partial class DischargePapersInfo: EntityBase, IEditableObject
    {
        public DischargePapersInfo()
            : base()
        {

        }

        private DischargePapersInfo _tempDischargePapersInfo;
        #region IEditableObject Members

        public void BeginEdit()
        {
            _tempDischargePapersInfo = (DischargePapersInfo)this.MemberwiseClone();
        }

        public void CancelEdit()
        {
            if (null != _tempDischargePapersInfo)
                CopyFrom(_tempDischargePapersInfo);
            //_tempPatient = null;
        }

        public void EndEdit()
        {
        }

        public void CopyFrom(DischargePapersInfo p)
        {
            PropertyCopierHelper.CopyPropertiesTo(p, this);
        }

        #endregion

        #region Primitive Properties
        private long _PaperID;
        [DataMemberAttribute()]
        public long PaperID
        {
            get
            {
                return _PaperID;
            }
            set
            {
                if (_PaperID == value)
                {
                    return;
                }
                _PaperID = value;
                RaisePropertyChanged("PaperID");
            }
        }

        private long _PtRegistrationID;
        [DataMemberAttribute()]
        public long PtRegistrationID
        {
            get
            {
                return _PtRegistrationID;
            }
            set
            {
                if(_PtRegistrationID == value)
                {
                    return;
                }
                _PtRegistrationID = value;
                RaisePropertyChanged("PtRegistrationID");
            }
        }

        private long _V_RegistrationType;
        [DataMemberAttribute()]
        public long V_RegistrationType
        {
            get
            {
                return _V_RegistrationType;
            }
            set
            {
                if (_V_RegistrationType == value)
                {
                    return;
                }
                _V_RegistrationType = value;
                RaisePropertyChanged("V_RegistrationType");
            }
        }

        private string _ReasonOfPregnancyTermination;
        [DataMemberAttribute()]
        public string ReasonOfPregnancyTermination
        {
            get
            {
                return _ReasonOfPregnancyTermination;
            }
            set
            {
                if (_ReasonOfPregnancyTermination == value)
                {
                    return;
                }
                _ReasonOfPregnancyTermination = value;
                RaisePropertyChanged("ReasonOfPregnancyTermination");
            }
        }

        private DateTime? _PregnancyTerminationDateTime;
        [DataMemberAttribute()]
        public DateTime? PregnancyTerminationDateTime
        {
            get
            {
                return _PregnancyTerminationDateTime;
            }
            set
            {
                if (_PregnancyTerminationDateTime == value)
                {
                    return;
                }
                _PregnancyTerminationDateTime = value;
                RaisePropertyChanged("PregnancyTerminationDateTime");
            }
        }

        private bool _IsPregnancyTermination;
        [DataMemberAttribute()]
        public bool IsPregnancyTermination
        {
            get
            {
                return _IsPregnancyTermination;
            }
            set
            {
                if (_IsPregnancyTermination == value)
                {
                    return;
                }
                _IsPregnancyTermination = value;
                RaisePropertyChanged("IsPregnancyTermination");
            }
        }

        private long? _HeadOfDepartmentDoctorStaffID;
        [DataMemberAttribute()]
        public long? HeadOfDepartmentDoctorStaffID
        {
            get
            {
                return _HeadOfDepartmentDoctorStaffID;
            }
            set
            {
                if (_HeadOfDepartmentDoctorStaffID == value)
                {
                    return;
                }
                _HeadOfDepartmentDoctorStaffID = value;
                RaisePropertyChanged("HeadOfDepartmentDoctorStaffID");
            }
        }

        private long? _UnitLeaderDoctorStaffID;
        [DataMemberAttribute()]
        public long? UnitLeaderDoctorStaffID
        {
            get
            {
                return _UnitLeaderDoctorStaffID;
            }
            set
            {
                if (_UnitLeaderDoctorStaffID == value)
                {
                    return;
                }
                _UnitLeaderDoctorStaffID = value;
                RaisePropertyChanged("UnitLeaderDoctorStaffID");
            }
        }

        private long? _DoctorStaffID;
        [DataMemberAttribute()]
        public long? DoctorStaffID
        {
            get
            {
                return _DoctorStaffID;
            }
            set
            {
                if (_DoctorStaffID == value)
                {
                    return;
                }
                _DoctorStaffID = value;
                RaisePropertyChanged("DoctorStaffID");
            }
        }

        private int? _NumberDayOfLeaveForTreatment;
        [DataMemberAttribute()]
        public int? NumberDayOfLeaveForTreatment
        {
            get
            {
                return _NumberDayOfLeaveForTreatment;
            }
            set
            {
                if (_NumberDayOfLeaveForTreatment == value)
                {
                    return;
                }
                _NumberDayOfLeaveForTreatment = value;
                RaisePropertyChanged("NumberDayOfLeaveForTreatment");
            }
        }

        private int? _FetalAge;
        [DataMemberAttribute()]
        public int? FetalAge
        {
            get
            {
                return _FetalAge;
            }
            set
            {
                if (_FetalAge == value)
                {
                    return;
                }
                _FetalAge = value;
                RaisePropertyChanged("FetalAge");
            }
        }

        private DateTime? _ToDateLeaveForTreatment;
        [DataMemberAttribute()]
        public DateTime? ToDateLeaveForTreatment
        {
            get
            {
                return _ToDateLeaveForTreatment;
            }
            set
            {
                if (_ToDateLeaveForTreatment == value)
                {
                    return;
                }
                _ToDateLeaveForTreatment = value;
                RaisePropertyChanged("ToDateLeaveForTreatment");
            }
        }

        private DateTime? _FromDateLeaveForTreatment;
        [DataMemberAttribute()]
        public DateTime? FromDateLeaveForTreatment
        {
            get
            {
                return _FromDateLeaveForTreatment;
            }
            set
            {
                if (_FromDateLeaveForTreatment == value)
                {
                    return;
                }
                _FromDateLeaveForTreatment = value;
                RaisePropertyChanged("FromDateLeaveForTreatment");
            }
        }

        private string _DischargeDiagnostic;
        [DataMemberAttribute()]
        public string DischargeDiagnostic
        {
            get
            {
                return _DischargeDiagnostic;
            }
            set
            {
                if (_DischargeDiagnostic == value)
                {
                    return;
                }
                _DischargeDiagnostic = value;
                RaisePropertyChanged("DischargeDiagnostic");
            }
        }

        private string _TreatmentMethod;
        [DataMemberAttribute()]
        public string TreatmentMethod
        {
            get
            {
                return _TreatmentMethod;
            }
            set
            {
                if (_TreatmentMethod == value)
                {
                    return;
                }
                _TreatmentMethod = value;
                RaisePropertyChanged("TreatmentMethod");
            }
        }

        private string _Notes;
        [DataMemberAttribute()]
        public string Notes
        {
            get
            {
                return _Notes;
            }
            set
            {
                if (_Notes == value)
                {
                    return;
                }
                _Notes = value;
                RaisePropertyChanged("Notes");
            }
        }
        #endregion
    }
}
