using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{
    public partial class ProfessionalSkill : NotifyChangedBase
    {
        #region Factory Method


        /// Create a new ProfessionalSkill object.

        /// <param name="skillID">Initial value of the SkillID property.</param>
        public static ProfessionalSkill CreateProfessionalSkill(long skillID)
        {
            ProfessionalSkill professionalSkill = new ProfessionalSkill();
            professionalSkill.SkillID = skillID;
            return professionalSkill;
        }

        #endregion
        #region Primitive Properties





        [DataMemberAttribute()]
        public long SkillID
        {
            get
            {
                return _SkillID;
            }
            set
            {
                if (_SkillID != value)
                {
                    OnSkillIDChanging(value);
                    ////ReportPropertyChanging("SkillID");
                    _SkillID = value;
                    RaisePropertyChanged("SkillID");
                    OnSkillIDChanged();
                }
            }
        }
        private long _SkillID;
        partial void OnSkillIDChanging(long value);
        partial void OnSkillIDChanged();





        [DataMemberAttribute()]
        public Nullable<long> ProfDegID
        {
            get
            {
                return _ProfDegID;
            }
            set
            {
                OnProfDegIDChanging(value);
                ////ReportPropertyChanging("ProfDegID");
                _ProfDegID = value;
                RaisePropertyChanged("ProfDegID");
                OnProfDegIDChanged();
            }
        }
        private Nullable<long> _ProfDegID;
        partial void OnProfDegIDChanging(Nullable<long> value);
        partial void OnProfDegIDChanged();





        [DataMemberAttribute()]
        public Nullable<Int64> StaffID
        {
            get
            {
                return _StaffID;
            }
            set
            {
                OnStaffIDChanging(value);
                ////ReportPropertyChanging("StaffID");
                _StaffID = value;
                RaisePropertyChanged("StaffID");
                OnStaffIDChanged();
            }
        }
        private Nullable<Int64> _StaffID;
        partial void OnStaffIDChanging(Nullable<Int64> value);
        partial void OnStaffIDChanged();





        [DataMemberAttribute()]
        public Nullable<DateTime> BeginningDate
        {
            get
            {
                return _BeginningDate;
            }
            set
            {
                OnBeginningDateChanging(value);
                ////ReportPropertyChanging("BeginningDate");
                _BeginningDate = value;
                RaisePropertyChanged("BeginningDate");
                OnBeginningDateChanged();
            }
        }
        private Nullable<DateTime> _BeginningDate;
        partial void OnBeginningDateChanging(Nullable<DateTime> value);
        partial void OnBeginningDateChanged();





        [DataMemberAttribute()]
        public Nullable<DateTime> GraduationDate
        {
            get
            {
                return _GraduationDate;
            }
            set
            {
                OnGraduationDateChanging(value);
                ////ReportPropertyChanging("GraduationDate");
                _GraduationDate = value;
                RaisePropertyChanged("GraduationDate");
                OnGraduationDateChanged();
            }
        }
        private Nullable<DateTime> _GraduationDate;
        partial void OnGraduationDateChanging(Nullable<DateTime> value);
        partial void OnGraduationDateChanged();





        [DataMemberAttribute()]
        public String UniversityInstitude
        {
            get
            {
                return _UniversityInstitude;
            }
            set
            {
                OnUniversityInstitudeChanging(value);
                ////ReportPropertyChanging("UniversityInstitude");
                _UniversityInstitude = value;
                RaisePropertyChanged("UniversityInstitude");
                OnUniversityInstitudeChanged();
            }
        }
        private String _UniversityInstitude;
        partial void OnUniversityInstitudeChanging(String value);
        partial void OnUniversityInstitudeChanged();





        [DataMemberAttribute()]
        public String Rank
        {
            get
            {
                return _Rank;
            }
            set
            {
                OnRankChanging(value);
                ////ReportPropertyChanging("Rank");
                _Rank = value;
                RaisePropertyChanged("Rank");
                OnRankChanged();
            }
        }
        private String _Rank;
        partial void OnRankChanging(String value);
        partial void OnRankChanged();





        [DataMemberAttribute()]
        public String ProfSkillNotes
        {
            get
            {
                return _ProfSkillNotes;
            }
            set
            {
                OnProfSkillNotesChanging(value);
                ////ReportPropertyChanging("ProfSkillNotes");
                _ProfSkillNotes = value;
                RaisePropertyChanged("ProfSkillNotes");
                OnProfSkillNotesChanged();
            }
        }
        private String _ProfSkillNotes;
        partial void OnProfSkillNotesChanging(String value);
        partial void OnProfSkillNotesChanged();





        [DataMemberAttribute()]
        public String TrainingType
        {
            get
            {
                return _TrainingType;
            }
            set
            {
                OnTrainingTypeChanging(value);
                ////ReportPropertyChanging("TrainingType");
                _TrainingType = value;
                RaisePropertyChanged("TrainingType");
                OnTrainingTypeChanged();
            }
        }
        private String _TrainingType;
        partial void OnTrainingTypeChanging(String value);
        partial void OnTrainingTypeChanged();

        #endregion

        #region Navigation Properties






        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_PROFESSI_REL_HR01_PROFESSI", "ProfessionalDegrees")]
        public ProfessionalDegree ProfessionalDegree
        {
            get;
            set;
        }


        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_PROFESSI_REL_HR03_STAFFS", "Staffs")]
        public Staff Staff
        {
            get;
            set;
        }

        #endregion
    }
}
