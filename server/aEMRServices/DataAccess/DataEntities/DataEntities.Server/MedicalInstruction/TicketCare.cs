using eHCMS.Services.Core.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DataEntities.MedicalInstruction
{
    public partial class TicketCare: NotifyChangedBase
    {
        #region Primitive Properties
        [DataMemberAttribute()]
        public long TicketCareID
        {
            get
            {
                return _TicketCareID;
            }
            set
            {
                if (_TicketCareID != value)
                {
                    _TicketCareID = value;
                    RaisePropertyChanged("TicketCareID");
                }
            }
        }
        private long _TicketCareID;
        [DataMemberAttribute()]
        public long StaffID
        {
            get
            {
                return _StaffID;
            }
            set
            {
                if (_StaffID != value)
                {
                    _StaffID = value;
                    RaisePropertyChanged("StaffID");
                }
            }
        }
        private long _StaffID;
        [DataMemberAttribute()]
        public long CreatedStaffID
        {
            get
            {
                return _CreatedStaffID;
            }
            set
            {
                if (_CreatedStaffID != value)
                {
                    _CreatedStaffID = value;
                    RaisePropertyChanged("CreatedStaffID");
                }
            }
        }
        private long _CreatedStaffID;
        [DataMemberAttribute()]
        public Staff CreatedStaff
        {
            get
            {
                return _CreatedStaff;
            }
            set
            {
                _CreatedStaff = value;
                RaisePropertyChanged("CreatedStaff");
            }
        }
        private Staff _CreatedStaff;
        [DataMemberAttribute()]
        public bool MarkAsDeleted
        {
            get
            {
                return _MarkAsDeleted;
            }
            set
            {
                _MarkAsDeleted = value;
                RaisePropertyChanged("MarkAsDeleted");
            }
        }
        private bool _MarkAsDeleted;
        [DataMemberAttribute()]
        public string OrientedTreatment
        {
            get
            {
                return _OrientedTreatment;
            }
            set
            {
                _OrientedTreatment = value;
                RaisePropertyChanged("OrientedTreatment");
            }
        }
        private string _OrientedTreatment;
        [DataMemberAttribute()]
        public string ExcuteInstruction
        {
            get
            {
                return _ExcuteInstruction;
            }
            set
            {
                _ExcuteInstruction = value;
                RaisePropertyChanged("ExcuteInstruction");
            }
        }
        private string _ExcuteInstruction;
        [DataMemberAttribute()]
        public DateTime DateExcute
        {
            get
            {
                return _DateExcute;
            }
            set
            {
                _DateExcute = value;
                RaisePropertyChanged("DateExcute");
            }
        }
        private DateTime _DateExcute;
        [DataMemberAttribute()]
        public long IntPtDiagDrInstructionID
        {
            get
            {
                return _IntPtDiagDrInstructionID;
            }
            set
            {
                _IntPtDiagDrInstructionID = value;
                RaisePropertyChanged("IntPtDiagDrInstructionID");
            }
        }
        private long _IntPtDiagDrInstructionID;
        [DataMemberAttribute()]
        public long PtRegistrationID
        {
            get
            {
                return _PtRegistrationID;
            }
            set
            {
                _PtRegistrationID = value;
                RaisePropertyChanged("PtRegistrationID");
            }
        }
        private long _PtRegistrationID;
        [DataMemberAttribute()]
        public long V_LevelCare
        {
            get
            {
                return _V_LevelCare;
            }
            set
            {
                _V_LevelCare = value;
                RaisePropertyChanged("V_LevelCare");
            }
        }
        private long _V_LevelCare;
        [DataMemberAttribute()]
        public DateTime ExamDate
        {
            get
            {
                return _ExamDate;
            }
            set
            {
                _ExamDate = value;
                RaisePropertyChanged("ExamDate");
            }
        }
        private DateTime _ExamDate;
        [DataMemberAttribute()]
        public string DoctorName
        {
            get
            {
                return _DoctorName;
            }
            set
            {
                _DoctorName = value;
                RaisePropertyChanged("DoctorName");
            }
        }
        private string _DoctorName;
        [DataMemberAttribute()]
        public long V_RegistrationType
        {
            get
            {
                return _V_RegistrationType;
            }
            set
            {
                _V_RegistrationType = value;
                RaisePropertyChanged("V_RegistrationType");
            }
        }
        private long _V_RegistrationType;
        #endregion
        public static TicketCare CreateTicketCare(long TicketCareID, long StaffID, long CreatedStaffID, string OrientedTreatment
            , string ExcuteInstruction, bool MarkAsDeleted)
        {
            TicketCare TicketCare = new TicketCare
            {
                TicketCareID = TicketCareID,
                StaffID = StaffID,
                CreatedStaffID = CreatedStaffID,
                OrientedTreatment = OrientedTreatment,
                ExcuteInstruction = ExcuteInstruction,
                MarkAsDeleted = MarkAsDeleted
            };
            return TicketCare;
        }
    }
}
