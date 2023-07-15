using eHCMS.Services.Core.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DataEntities
{
    public partial class AgeOfTheArtery : EntityBase
    {
        #region Primitive Properties
        [DataMemberAttribute()]
        public long AgeOfTheArteryID
        {
            get
            {
                return _AgeOfTheArteryID;
            }
            set
            {
                if (_AgeOfTheArteryID == value)
                {
                    return;
                }
                _AgeOfTheArteryID = value;
                RaisePropertyChanged("AgeOfTheArteryID");
            }
        }
        private long _AgeOfTheArteryID;

        [DataMemberAttribute()]
        public Patient Patient
        {
            get
            {
                return _Patient;
            }
            set
            {
                if (_Patient == value)
                {
                    return;
                }
                _Patient = value;
                RaisePropertyChanged("Patient");
            }
        }
        private Patient _Patient;

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
        private long _V_RegistrationType;

        [DataMemberAttribute()]
        public long PatientClassID
        {
            get
            {
                return _PatientClassID;
            }
            set
            {
                if (_PatientClassID == value)
                {
                    return;
                }
                _PatientClassID = value;
                RaisePropertyChanged("PatientClassID");
            }
        }
        private long _PatientClassID;

        [DataMemberAttribute()]
        public int AgePoint
        {
            get
            {
                return _AgePoint;
            }
            set
            {
                if (_AgePoint == value)
                {
                    return;
                }
                _AgePoint = value;
                RaisePropertyChanged("AgePoint");
            }
        }
        private int _AgePoint;

        [DataMemberAttribute()]
        public float? BloodPressure
        {
            get
            {
                return _BloodPressure;
            }
            set
            {
                if (_BloodPressure == value)
                {
                    return;
                }
                _BloodPressure = value;
                RaisePropertyChanged("BloodPressure");
            }
        }
        private float? _BloodPressure;

        [DataMemberAttribute()]
        public bool IsTreatmentBloodPressure
        {
            get
            {
                return _IsTreatmentBloodPressure;
            }
            set
            {
                if (_IsTreatmentBloodPressure == value)
                {
                    return;
                }
                _IsTreatmentBloodPressure = value;
                RaisePropertyChanged("IsTreatmentBloodPressure");
            }
        }
        private bool _IsTreatmentBloodPressure;

        [DataMemberAttribute()]
        public int BloodPressureScore
        {
            get
            {
                return _BloodPressureScore;
            }
            set
            {
                if (_BloodPressureScore == value)
                {
                    return;
                }
                _BloodPressureScore = value;
                RaisePropertyChanged("BloodPressureScore");
            }
        }
        private int _BloodPressureScore;

        [DataMemberAttribute()]
        public float? HDL
        {
            get
            {
                return _HDL;
            }
            set
            {
                if (_HDL == value)
                {
                    return;
                }
                _HDL = value;
                RaisePropertyChanged("HDL");
            }
        }
        private float? _HDL;

        [DataMemberAttribute()]
        public int HDLScore
        {
            get
            {
                return _HDLScore;
            }
            set
            {
                if (_HDLScore == value)
                {
                    return;
                }
                _HDLScore = value;
                RaisePropertyChanged("HDLScore");
            }
        }
        private int _HDLScore;

        [DataMemberAttribute()]
        public float? Cholesterol
        {
            get
            {
                return _Cholesterol;
            }
            set
            {
                if (_Cholesterol == value)
                {
                    return;
                }
                _Cholesterol = value;
                RaisePropertyChanged("Cholesterol");
            }
        }
        private float? _Cholesterol;

        [DataMemberAttribute()]
        public int CholesterolScore
        {
            get
            {
                return _CholesterolScore;
            }
            set
            {
                if (_CholesterolScore == value)
                {
                    return;
                }
                _CholesterolScore = value;
                RaisePropertyChanged("CholesterolScore");
            }
        }
        private int _CholesterolScore;

        [DataMemberAttribute()]
        public bool IsSmoke
        {
            get
            {
                return _IsSmoke;
            }
            set
            {
                if (_IsSmoke == value)
                {
                    return;
                }
                _IsSmoke = value;
                RaisePropertyChanged("IsSmoke");
            }
        }
        private bool _IsSmoke;

        [DataMemberAttribute()]
        public int SmokeScore
        {
            get
            {
                return _SmokeScore;
            }
            set
            {
                if (_SmokeScore == value)
                {
                    return;
                }
                _SmokeScore = value;
                RaisePropertyChanged("SmokeScore");
            }
        }
        private int _SmokeScore;

        [DataMemberAttribute()]
        public bool Diabetes
        {
            get
            {
                return _Diabetes;
            }
            set
            {
                if (_Diabetes == value)
                {
                    return;
                }
                _Diabetes = value;
                RaisePropertyChanged("Diabetes");
            }
        }
        private bool _Diabetes;

        [DataMemberAttribute()]
        public int DiabetesScore
        {
            get
            {
                return _DiabetesScore;
            }
            set
            {
                if (_DiabetesScore == value)
                {
                    return;
                }
                _DiabetesScore = value;
                RaisePropertyChanged("DiabetesScore");
            }
        }
        private int _DiabetesScore;

        [DataMemberAttribute()]
        public int TotalScore
        {
            get
            {
                return _TotalScore;
            }
            set
            {
                if (_TotalScore == value)
                {
                    return;
                }
                _TotalScore = value;
                RaisePropertyChanged("TotalScore");
            }
        }
        private int _TotalScore;

        [DataMemberAttribute()]
        public int? AgePointOfTheArtery
        {
            get
            {
                return _AgePointOfTheArtery;
            }
            set
            {
                if (_AgePointOfTheArtery == value)
                {
                    return;
                }
                _AgePointOfTheArtery = value;
                RaisePropertyChanged("AgePointOfTheArtery");
            }
        }
        private int? _AgePointOfTheArtery;

        [DataMemberAttribute()]
        public float? Height
        {
            get
            {
                return _Height;
            }
            set
            {
                if (_Height == value)
                {
                    return;
                }
                _Height = value;
                RaisePropertyChanged("Height");
            }
        }
        private float? _Height;

        [DataMemberAttribute()]
        public float? Weight
        {
            get
            {
                return _Weight;
            }
            set
            {
                if (_Weight == value)
                {
                    return;
                }
                _Weight = value;
                RaisePropertyChanged("Weight");
            }
        }
        private float? _Weight;

        [DataMemberAttribute()]
        public float? Waist
        {
            get
            {
                return _Waist;
            }
            set
            {
                if (_Waist == value)
                {
                    return;
                }
                _Waist = value;
                RaisePropertyChanged("Waist");
            }
        }
        private float? _Waist;

        [DataMemberAttribute()]
        public decimal? BMI
        {
            get
            {
                return _BMI;
            }
            set
            {
                if (_BMI == value)
                {
                    return;
                }
                _BMI = value;
                RaisePropertyChanged("BMI");
            }
        }
        private decimal? _BMI;

        [DataMemberAttribute()]
        public string Diagnosic
        {
            get
            {
                return _Diagnosic;
            }
            set
            {
                if (_Diagnosic == value)
                {
                    return;
                }
                _Diagnosic = value;
                RaisePropertyChanged("Diagnosic");
            }
        }
        private string _Diagnosic;

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
