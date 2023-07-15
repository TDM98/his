using System;
using eHCMS.Services.Core.Base;
using System.Runtime.Serialization;

namespace DataEntities
{
    public class SurgeryHiTechProcStatusHistory : NotifyChangedBase
    {
        private long _SurgeryHiTechProcStatusHistoryID;
        [DataMemberAttribute]
        public long SurgeryHiTechProcStatusHistoryID
        {
            get
            {
                return _SurgeryHiTechProcStatusHistoryID;
            }
            set
            {
                _SurgeryHiTechProcStatusHistoryID = value;
                RaisePropertyChanged("SurgeryHiTechProcStatusHistoryID");
            }
        }

        private long _ConsultingDiagnosysID;
        [DataMemberAttribute]
        public long ConsultingDiagnosysID
        {
            get
            {
                return _ConsultingDiagnosysID;
            }
            set
            {
                _ConsultingDiagnosysID = value;
                RaisePropertyChanged("ConsultingDiagnosysID");
            }
        }

        private DateTime _ProcessDate;
        [DataMemberAttribute]
        public DateTime ProcessDate
        {
            get
            {
                return _ProcessDate;
            }
            set
            {
                _ProcessDate = value;
                RaisePropertyChanged("ProcessDate");
            }
        }

        private Lookup _V_ProcessStep;
        [DataMemberAttribute]
        public Lookup V_ProcessStep
        {
            get
            {
                return _V_ProcessStep;
            }
            set
            {
                _V_ProcessStep = value;
                RaisePropertyChanged("V_ProcessStep");
            }
        }

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
                _IsActive = value;
                RaisePropertyChanged("IsActive");
            }
        }

        private bool _IsCurrent;
        [DataMemberAttribute]
        public bool IsCurrent
        {
            get
            {
                return _IsCurrent;
            }
            set
            {
                _IsCurrent = value;
                RaisePropertyChanged("IsCurrent");
            }
        }

        private string _Note;
        [DataMemberAttribute]
        public string Note
        {
            get
            {
                return _Note;
            }
            set
            {
                _Note = value;
                RaisePropertyChanged("Note");
            }
        }
    }
}
